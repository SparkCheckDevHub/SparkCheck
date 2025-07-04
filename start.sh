#!/bin/bash

# -----------------------------------------------------
# Name: start.sh 
#
# Starts the Docker Compose stack.
#
# Usage: sudo ./start.sh
#
# Author: James
# Date: 2025-07-02
# -----------------------------------------------------

docker-compose build
docker-compose up -d