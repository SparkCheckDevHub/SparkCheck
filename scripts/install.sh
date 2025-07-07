#!/bin/bash

# -----------------------------------------------------
# Name: install.sh
#
# Installs all dependencies on an Ubuntu 22.04 Server
# installation. Dependencies include:
#  - Docker
#  - Docker Compose
#  - Git
#  - Certbot
#  - Nginx
#
# Usage: sudo ./install.sh
#
# Author: James
# Date: 2025-07-02
# -----------------------------------------------------

# Docker
sudo apt remove --yes docker docker-engine docker.io containerd runc || true
sudo apt update
sudo apt install -y ca-certificates curl gnupg lsb-release
sudo mkdir -p /etc/apt/keyrings
curl -fsSL https://download.docker.com/linux/ubuntu/gpg | sudo gpg --dearmor -o /etc/apt/keyrings/docker.gpg
echo \
  "deb [arch=$(dpkg --print-architecture) signed-by=/etc/apt/keyrings/docker.gpg] https://download.docker.com/linux/ubuntu \
  $(lsb_release -cs) stable" | sudo tee /etc/apt/sources.list.d/docker.list > /dev/null
sudo apt update
sudo apt install -y docker-ce docker-ce-cli containerd.io
if ! getent group docker > /dev/null; then
  sudo groupadd docker
fi
sudo systemctl enable docker
sudo systemctl start docker

# Docker Compose
sudo rm /usr/local/bin/docker-compose
DOCKER_COMPOSE_VERSION=$(curl --silent "https://api.github.com/repos/docker/compose/releases/latest" | grep -Po '"tag_name": "\K.*?(?=")')
sudo curl -L "https://github.com/docker/compose/releases/download/$DOCKER_COMPOSE_VERSION/docker-compose-$(uname -s)-$(uname -m)" -o /usr/local/bin/docker-compose
sudo chmod +x /usr/local/bin/docker-compose
printf '\nDocker Compose installed successfully\n\n'

# Git
sudo apt --yes --no-install-recommends install git
printf '\nGit installed successfully\n\n'

# Certbot
sudo apt --yes --no-install-recommends install certbot python3-certbot-nginx
printf '\nCertbot installed successfully\n\n'

# Nginx
sudo apt --yes --no-install-recommends install nginx
printf '\nNginx installed successfully\n\n'

# Socat
sudo apt --yes --no-install-recommends install socat
printf '\nSocat installed successfully\n\n'