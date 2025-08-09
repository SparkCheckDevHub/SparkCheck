#!/bin/bash

# -----------------------------------------------------
# Name: setup-nginx.sh
#
# Sets up the Nginx web server on first run.
#
# Usage: sudo ./setup-nginx.sh
#
# Author: James
# Date: 2025-07-02
# -----------------------------------------------------

set -e

# Copy the nginx config file to sites-available
cp nginx.conf /etc/nginx/sites-available/sparkcheck.date.conf

# Enable the site
if [ ! -L /etc/nginx/sites-enabled/sparkcheck.date.conf ]; then
  ln -s /etc/nginx/sites-available/sparkcheck.date.conf /etc/nginx/sites-enabled/sparkcheck.date.conf
fi

# Restart Nginx
sudo nginx -t
systemctl restart nginx

# Invoke certbot to install certificates
sudo certbot --nginx -d sparkcheck.date -d www.sparkcheck.date
sudo systemctl enable certbot.timer
sudo systemctl start certbot.timer
