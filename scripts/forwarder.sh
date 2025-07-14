#!/bin/bash

# -----------------------------------------------------
# Name: forwarder.sh
#
# Sets up the forwarder for accessing the database.
#
# Usage: sudo ./forwarder.sh [start|stop]
#
# Note: This script is used for development only. It is
# not recommended for production use.
#
# Author: James
# Date: 2025-07-07
# -----------------------------------------------------

# Start the socat forwarder
function start_forwarder() {
  if [ -f /tmp/sparkcheck_socat.pid ]; then
    kill $(cat /tmp/sparkcheck_socat.pid)
    rm /tmp/sparkcheck_socat.pid
  fi
  sudo socat TCP-LISTEN:9933,fork TCP:127.0.0.1:1433 &
  echo $! > /tmp/sparkcheck_socat.pid
  echo "Forwarding public port 9933 to 1433 (Microsoft SQL Server)"
}

# Stop the socat forwarder
function stop_forwarder() {
  if [ -f /tmp/sparkcheck_socat.pid ]; then
    kill $(cat /tmp/sparkcheck_socat.pid)
    rm /tmp/sparkcheck_socat.pid
    echo "Forwarding stopped"
  else
    echo "Forwarding not running"
  fi
}

# Check for start or stop argument. If not provided, start the forwarder.
if [ "$1" != "start" ] && [ "$1" != "stop" ]; then
  start_forwarder
fi

if [ "$1" == "start" ]; then
  start_forwarder
elif [ "$1" == "stop" ]; then
  stop_forwarder
fi