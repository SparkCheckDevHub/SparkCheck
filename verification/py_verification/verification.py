"""
Verification logic module.
"""

__all__ = ["router"]

import os
import logging
import time
import asyncio
import re
from typing import Any, Optional

from fastapi import APIRouter, HTTPException, Query, Depends
from asterisk.ami import AMIClient, SimpleAction

logger = logging.getLogger("uvicorn")
router = APIRouter()


@router.get("/sendVerificationMessage")
async def send_verification_message(
        strPhone: str = Query(..., description="User's phone number."),
        strCode: str = Query(..., description="A code to send to the user."),
):
    """
    Endpoint for sending a verification message to a number.
    """
    client = AMIClient(address='127.0.0.1',port=5038)
    client.login(username='verification',secret='py_verification')
    action = SimpleAction(
        'Originate',
        Channel=f'PJSIP/{strPhone}@verification-endpoint',
        Exten=strPhone,
        Priority=1,
        Context='outbound',
        CallerID='SparkCheck',
        Timeout=30000,
        Async="true",
        Variable='CODE=' + strCode,
    )
    future = client.send_action(action)
    return str(future.response)