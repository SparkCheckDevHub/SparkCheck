# Use official Python image
FROM python:3.13-slim-bookworm

# Install pyodbc and ODBC driver for SQL Server
RUN apt-get update \
    && apt-get install -y --no-install-recommends \
        gnupg \
        g++ \
        unixodbc-dev \
        curl \
    && curl -fsSL https://packages.microsoft.com/keys/microsoft.asc | gpg --dearmor -o /usr/share/keyrings/microsoft-prod.gpg \
    && curl https://packages.microsoft.com/config/debian/12/prod.list > /etc/apt/sources.list.d/mssql-release.list \
    && apt-get update \
    && ACCEPT_EULA=Y apt-get install -y --no-install-recommends msodbcsql18 \
    && apt-get clean \
    && rm -rf /var/lib/apt/lists/*

# Set working directory
WORKDIR /app

# Copy requirements.txt and install dependencies
COPY requirements.txt .
RUN pip install --no-cache-dir -r requirements.txt

# Copy the application code
COPY . .

# Expose uvicorn port
EXPOSE 9988

# Run the application
CMD ["uvicorn", "main:app", "--host", "0.0.0.0", "--port", "9988"]