"""
Package initialization for py_verification.
Starts the verification endpoint when the package is imported.
"""

from fastapi import FastAPI
from py_verification.verification import router

app = FastAPI()
app.include_router(router)