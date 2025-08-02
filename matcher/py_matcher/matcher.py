"""
Matcher logic module.
"""

__all__ = ["router", "start_matcher"]

import os
import logging
import time
import asyncio
from datetime import datetime, date
from typing import Any, Optional
from threading import Lock, Thread

from fastapi import APIRouter, HTTPException, Query, Depends
from sqlalchemy.orm import Session, sessionmaker
from sqlalchemy import (
    create_engine,
    select,
    update,
    insert,
    delete,
    func,
    or_,
    and_,
    not_,
    exists,
)
from sqlalchemy.exc import SQLAlchemyError
from geopy import distance

from .models import (
    TUsers,
    TMatchRequests,
    TMatches,
    TUserPreferences,
    TGenders,
    TUserInterests
)

logger = logging.getLogger("uvicorn")
router = APIRouter()

connection_string = os.getenv("SC_MATCHER_CONN")
if not connection_string:
    raise ValueError("SC_MATCHER_CONN environment variable is not set")

user_updates_lock = Lock()
user_updates = {}
queue_remover_lock = Lock()
queue_remover = {}


def wait_for_connection():
    """
    Waits for a database connection to be available.
    """
    logger.info("Waiting for database connection...")
    engine = create_engine(connection_string)
    while True:
        try:
            with engine.connect() as conn:
                # Check for TUsers table
                conn.execute(select(TUsers))
                logger.info("Connected to database successfully.")
                return engine
        except SQLAlchemyError as e:
            time.sleep(1)
            

engine = wait_for_connection()
SessionLocal = sessionmaker(autocommit=False, autoflush=False, bind=engine)


def calculate_age(date):
    """
    Calculates the age of a given date.
    """
    today = date.today()
    return today.year - date.year - ((today.month, today.day) < (date.month, date.day))


def calculate_distance(db, user1: TUsers, user2: TUsers):
    """
    Calculates the distance between two users.
    """
    # If no coordinates are available, matching preferences are ignored
    if (
        user1.decLatitude is None or user1.decLongitude is None
        or user2.decLatitude is None or user2.decLongitude is None
    ):
        return 0
    
    # Calculate the distance between the two users
    dist = distance.distance(
        (user1.decLatitude, user1.decLongitude),
        (user2.decLatitude, user2.decLongitude)
    ).miles

    return dist

def match_score(db, user1, user2, shared_interests, interests1, interests2):
    """
    Calculates the match score between two users.
    """
    # Get each user's preferences
    pref1 = (
        db.query(TUserPreferences)
        .filter(TUserPreferences.intUserID == user1.intUserID)
        .first()
    )
    pref2 = (
        db.query(TUserPreferences)
        .filter(TUserPreferences.intUserID == user2.intUserID)
        .first()
    )

    # Disqualify users with mismatching gender preferences (except unspecified)
    gender_pref1 = pref1.intGenderPreferenceID
    gender_pref2 = pref2.intGenderPreferenceID
    if gender_pref1 == 1:
        gender_pref1 = user2.intGenderID
    if gender_pref2 == 1:
        gender_pref2 = user1.intGenderID

    if (
        gender_pref1 != user2.intGenderID 
        or gender_pref2 != user1.intGenderID
    ):
        return -1

    # Disqualify users with mismatching age preferences
    age1 = calculate_age(user1.dtmDateOfBirth)
    age2 = calculate_age(user2.dtmDateOfBirth)
    if (
        pref1.intMinAge > age2
        or pref1.intMaxAge < age2
        or pref2.intMinAge > age1
        or pref2.intMaxAge < age1
    ):
        return -1

    # Disqualify users outside of matching distance
    dist = calculate_distance(db, user1, user2)

    if dist > pref1.intMatchDistance:
        return -1
    if dist > pref2.intMatchDistance:
        return -1
     
    # Calculate interests score (1 = all interests shared)
    interests_score1 = 0 if interests1 == 0 else shared_interests / interests1
    interests_score2 = 0 if interests2 == 0 else shared_interests / interests2
    interests_score = (interests_score1 + interests_score2) / 2

    # Calculate distance score (0 = max match distance)
    dist_score1 = 0
    dist_score2 = 0

    if pref1.intMatchDistance != 0:
        dist_score1 = (pref1.intMatchDistance - dist) / pref1.intMatchDistance
    if pref2.intMatchDistance != 0:
        dist_score2 = (pref2.intMatchDistance - dist) / pref2.intMatchDistance

    dist_score = (dist_score1 + dist_score2) / 2

    # Calculate age score (1 = closest age to median range)
    age_score1 = 0
    age_score2 = 0

    preferred_age1 = (pref1.intMinAge + pref1.intMaxAge) / 2
    preferred_age2 = (pref2.intMinAge + pref2.intMaxAge) / 2

    diff1 = preferred_age1 - pref1.intMinAge
    diff2 = preferred_age2 - pref2.intMinAge

    age_score1 = 0 if diff1 == 0 else abs(age2 - preferred_age1) / diff1
    age_score2 = 0 if diff2 == 0 else abs(age1 - preferred_age2) / diff2
    age_score = 1 - (age_score1 + age_score2) / 2

    # Calculate overall score (weighted average)
    score = (interests_score + dist_score + age_score) / 3

    return score


def find_match(db, user: TUsers):
    """
    Attempts to find a match for a user.
    """
    # Find users who have previously matched with this user
    matched_subq = (
        db.query(TMatchRequests)
        .filter(
            or_(
                and_(
                    TMatchRequests.intFirstUserID == user.intUserID,
                    TMatchRequests.intSecondUserID == TUsers.intUserID
                ),
                and_(
                    TMatchRequests.intSecondUserID == user.intUserID,
                    TMatchRequests.intFirstUserID == TUsers.intUserID
                )
            )
        )
        .exists()
    )

    # Find count of own interests
    interests1 = (
        db.query(func.count(TUserInterests.intInterestID))
        .filter(TUserInterests.intUserID == user.intUserID)
        .scalar()
    )

    # Find count of other users's interests
    other_interests_subq = (
        db.query(
            TUserInterests.intUserID.label("user_id"),
            func.count(TUserInterests.intInterestID).label("other_interests")
        )
        .group_by(TUserInterests.intUserID)
        .subquery()
    )

    # Find count of shared interests
    user_interests_ids_subq = (
        db.query(TUserInterests.intInterestID)
        .filter(TUserInterests.intUserID == user.intUserID)
        .subquery()
    )
    shared_interests_subq = (
        db.query(
            TUserInterests.intUserID.label("user_id"),
            func.count(TUserInterests.intInterestID).label("shared_interests")
        )
        .filter(TUserInterests.intInterestID.in_(user_interests_ids_subq))
        .filter(TUserInterests.intUserID != user.intUserID)
        .group_by(TUserInterests.intUserID)
        .subquery()
    )

    # Combine to find relevant match list
    users = (
        db.query(
            TUsers,
            func.coalesce(
                shared_interests_subq.c.shared_interests, 0
            ).label("shared_interests"),
            func.coalesce(
                other_interests_subq.c.other_interests, 0
            ).label("other_interests"),
        )
        .outerjoin(
            shared_interests_subq,
            TUsers.intUserID == shared_interests_subq.c.user_id
        )
        .outerjoin(
            other_interests_subq,
            TUsers.intUserID == other_interests_subq.c.user_id
        )
        .filter(TUsers.blnInQueue)
        .filter(TUsers.intUserID != user.intUserID)
        .filter(TUsers.blnIsActive)
        .filter(not_(matched_subq))
        .all()
    )

    # Loop through users to find a match
    highest_score = -1
    current_match = None    
    for u, shared, interests2 in users:
        if u.blnInQueue:
            score = match_score(db, user, u, shared, interests1, interests2)
            if score > highest_score:
                highest_score = score
                current_match = u

    # Check if match was found
    if current_match is None:
        return None
        
    # Create a new TMatchRequests and TMatches row 
    logger.info(
        f"{user.strFirstName} {user.strLastName} and "
        f"{current_match.strFirstName} {current_match.strLastName} "
        f"matched with score {highest_score}"
    )
    match_request = TMatchRequests(
        intFirstUserID=user.intUserID,
        intSecondUserID=current_match.intUserID,
        blnFirstUserDeclined=False,
        blnSecondUserDeclined=False,
        dtmRequestStarted=datetime.now(),
        dtmRequestEnded=None
    )
    db.add(match_request)
    db.flush()
    match = TMatches(
        intMatchRequestID=match_request.intMatchRequestID,
        blnFirstUserDeleted=False,
        blnSecondUserDeleted=False,
        dtmMatchStarted=datetime.now(),
        dtmMatchEnded=None
    )
    db.add(match)
    db.flush()
    user.blnInQueue = False
    current_match.blnInQueue = False
    
    # Send update to matched users
    with user_updates_lock:
        if user.intUserID in user_updates:
            user_updates[user.intUserID][1] = {
                "status": "success",
                "match_request_id": match_request.intMatchRequestID
            }
            user_updates[user.intUserID][0].set()
        if current_match.intUserID in user_updates:
            user_updates[current_match.intUserID][1] = {
                "status": "success",
                "match_request_id": match_request.intMatchRequestID
            }
            user_updates[current_match.intUserID][0].set()
    return match_request


def matcher_loop():
    """
    The main loop for finding matches. Flushes every two seconds.
    """
    global queue_remover
    
    # Reset all users to non-queueing
    try:
        with SessionLocal() as db:
            db.execute(
                update(TUsers)
                .where(TUsers.blnInQueue == True)
                .values(blnInQueue=False)
            )
            db.commit()
    except SQLAlchemyError:
        pass

    # Begin finding matches
    while True:
        try:
            with SessionLocal() as db:
                # Check for users in queue remover
                with queue_remover_lock:
                    for user_id in queue_remover:
                        user = db.query(TUsers).filter(
                            TUsers.intUserID == user_id
                        ).first()
                        if user is None:
                            continue
                        logger.info(f"User {user_id} removed from queue")
                        if user.intUserID in user_updates:
                            user_updates[user.intUserID][1] = {
                                "status": "stopped",
                                "message": "User removed from queue."
                            }
                            user_updates[user.intUserID][0].set()
                        user.blnInQueue = False      
                    queue_remover = {}          
                    db.commit()

                    # Loop through all queuing users
                    user_map = {}
                    users = (
                        db.query(TUsers)
                        .filter(TUsers.blnInQueue)
                        .filter(TUsers.blnIsActive)
                        .all()
                    )
                    for u in users:
                        user_map[u.intUserID] = u
                    for id, user in user_map.items():
                        if not user:
                            continue
                        if user.blnInQueue:
                            match = find_match(db, user)
                            
                            # Remove second user from queue
                            if match:
                                user_map[match.intSecondUserID] = None
                    db.commit()

        except Exception as e:
            info.exception(e)
        time.sleep(2)


def start_matcher():
    """
    Function that starts the matcher thread.
    """
    logger.info(f"Matcher thread started")
    thread = Thread(target=matcher_loop, daemon=True)
    thread.start()


def get_db():
    """
    Helper function for asynchronous database access.
    """
    db = SessionLocal()
    try:
        yield db
    finally:
        db.close()


@router.get("/enterQueue")
async def enter_queue(
        intUserID: int = Query(..., description="User ID to enter the queue"),
        db: Session = Depends(get_db)):
    """
    Endpoint for a user to enter the match-making queue.
    """
    # Check that user exists and is active
    user = db.query(TUsers).filter(TUsers.intUserID == intUserID).first()    
    if user is None or not user.blnIsActive:
        return {
            "status": "error",
            "message": f"User {intUserID} does not exist or is inactive",
        }

    # Check if user is already in queue
    if user.blnInQueue:
        return {
            "status": "error",
            "message": f"User {intUserID} is already in the queue",
        }

    # Add user to queue
    user.blnInQueue = True
    db.commit()
    
    # Wait until a match is found or user exits queue
    user_event = [asyncio.Event(),None]
    with user_updates_lock:
        user_updates[intUserID] = user_event
    try:
        await user_event[0].wait()
    finally:
        del user_updates[intUserID]
    if user_event[1] is not None:
        return user_event[1]
    return {"status": "error", message: "Failed to find match"}


@router.get("/exitQueue")
async def exit_queue(
        intUserID: int = Query(..., description="User ID to exit the queue"),
        db: Session = Depends(get_db)):
    """
    Endpoint for a user to exit the match-making queue.
    """
    # Check that user exists and is active
    user = db.query(TUsers).filter(TUsers.intUserID == intUserID).first()    
    if user is None or not user.blnIsActive:
        return {
            "status": "error",
            "message": f"User {intUserID} does not exist or is inactive",
        }
    # Add this user to the queue remover
    with queue_remover_lock:
        queue_remover[intUserID] = True
    return {
        "status": "success",
        "message": f"User {intUserID} exited the queue"
    }