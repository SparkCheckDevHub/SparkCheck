# Use Debian image
FROM debian:bullseye-slim

# Set up Python, Asterisk PBX, and git
RUN apt-get update && apt-get install -y \
    build-essential \
    git \
    asterisk-dev \
    flite-dev \
    automake \
    autoconf \
    libtool \
    pkg-config \
    python3 \
    python3-pip \
    gettext \
 && rm -rf /var/lib/apt/lists/*

# Install Asterisk-Flite module
RUN git clone https://github.com/zaf/Asterisk-Flite.git /usr/src/Asterisk-Flite && \
    cd /usr/src/Asterisk-Flite && \
    make && \
    make install

# Set working directory
WORKDIR /app

# Copy requirements.txt and install dependencies
COPY requirements.txt .
RUN pip install --no-cache-dir -r requirements.txt

# Copy the application code
COPY . .

# Expose uvicorn port
EXPOSE 9977

# Run the application
COPY start.sh /start.sh
RUN chmod +x /start.sh
CMD ["/start.sh"]