#!/bin/sh

# Substitute sip configuration template
envsubst < ./asterisk/pjsip.conf.template > /etc/asterisk/pjsip.conf

# Start Asterisk and Uvicorn if enabled
if [ "$SC_USE_ASTERISK" = "True" ]; then
  asterisk &
  uvicorn main:app --host 0.0.0.0 --port 9977
fi