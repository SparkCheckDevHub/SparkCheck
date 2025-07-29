"""
Package initialization for my_package.
Starts the matcher when the package is imported.
"""

from fastapi import FastAPI
from py_matcher.matcher import router, start_matcher

app = FastAPI()
app.include_router(router)

@app.on_event("startup")
def run_matcher():
    start_matcher()