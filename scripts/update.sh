#!/bin/bash

# -----------------------------------------------------
# Name: update.sh
#
# Pulls the latest changes from the repo and rebuilds
# the Docker images.
#
# Usage: bash ./scripts/update.sh
# Run from the root of the repo
#
# Author: James
# Date: 2025-07-02
# -----------------------------------------------------

# Pull the latest changes
git pull origin main

# Build and start the Docker images
sudo docker-compose build
sudo docker-compose up -d