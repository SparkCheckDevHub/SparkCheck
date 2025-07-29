"""
Matcher logic module.
"""

__all__ = ["router", "start_matcher"]

import os
import logging
import time
import asyncio
from datetime import datetime
from typing import Any, Optional
from threading import Lock, Thread

from fastapi import APIRouter, HTTPException, Query, Depends
from sqlalchemy.orm import Session, sessionmaker
from sqlalchemy import create_engine, select, update, insert, delete, func
from sqlalchemy.exc import SQLAlchemyError

from .models import TUsers, TMatchRequests, TMatches

logger = logging.getLogger("uvicorn")
router = APIRouter()
connection_string = os.getenv("SC_MATCHER_CONN")
if not connection_string:
    raise ValueError("SC_MATCHER_CONN environment variable is not set")
engine = create_engine(connection_string)
SessionLocal = sessionmaker(autocommit=False, autoflush=False, bind=engine)
user_updates_lock = Lock()
user_updates = {}
queue_remover_lock = Lock()
queue_remover = {}


def match_score(db, user1: TUsers, user2: TUsers):
    """
    Calculates the match score between two users.
    """
    return 0


def find_match(db, user: TUsers):
    """
    Attempts to find a match for a user.
    """
    # Get all other queuing users
    users = (
        db.query(TUsers)
        .filter(TUsers.blnInQueue)
        .filter(TUsers.intUserID != user.intUserID)
        .filter(TUsers.blnIsActive)
        .all()
    )

    # Loop through users to find a match
    highest_score = -1
    current_match = None    
    for u in users:
        if u.blnInQueue:
            score = match_score(db, user, u)            
            if score > highest_score:
                highest_score = score
                current_match = u

    # Check if match was found
    if current_match is None:
        logger.info(f"No match found for user {user.intUserID}")
        return None
        
    # Create a new TMatchRequests and TMatches row
    logger.info(f"Matched users {user.intUserID} + {current_match.intUserID}")
    max_id = db.query(func.max(TMatchRequests.intMatchRequestID)).scalar()
    next_id = (max_id or 0) + 1
    match_request = TMatchRequests(
        intMatchRequestID=next_id,
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
                "success": True,
                "match_request_id": match_request.intMatchRequestID
            }
            user_updates[user.intUserID][0].set()
            logger.info(f"HEHEHEHEHE")
        if current_match.intUserID in user_updates:
            user_updates[current_match.intUserID][1] = {
                "success": True,
                "match_request_id": match_request.intMatchRequestID
            }
            user_updates[current_match.intUserID][0].set()
            logger.info(f"HEHEHEHEHE")
    return match_request


def matcher_loop():
    """
    The main loop for finding matches. Flushes every two seconds.
    """
    global queue_remover
    while True:
        #try:
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

                # Loop through all queuing users, ignoring removed users
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
                        
                        # Check if second user can be removed from map
                        if match:
                            user_map[match.intSecondUserID] = None
                db.commit()

        #except Exception as e:
        #    pass
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
async def enter_queue(
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