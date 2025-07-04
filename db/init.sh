#!/bin/bash

# -----------------------------------------------------
# Name: init.sh
#
# Sets up the database on first run.
#
# Author: James
# Date: 2025-07-03
# -----------------------------------------------------

# Start SQL Server in the background
/opt/mssql/bin/sqlservr &

# Wait for SQL Server to start
until /opt/mssql-tools/bin/sqlcmd -S localhost -U SA -P "$SA_PASSWORD" -Q "SELECT 1" &> /dev/null
do
  sleep 1
done

# Run the initialization script
/opt/mssql-tools/bin/sqlcmd -S localhost -U SA -P "$SA_PASSWORD" -i /docker-entrypoint-initdb.d/init.sql