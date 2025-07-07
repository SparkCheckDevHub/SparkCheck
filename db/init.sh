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
until /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P "$SA_PASSWORD" -Q "SELECT 1" -C &> /dev/null
do
  echo "Waiting for SQL Server to start..."
  sleep 1
done

# Run the initialization script, and the procedures script
/opt/mssql-tools18/bin/sqlcmd -S localhost -U SA -P "$SA_PASSWORD" -i /usr/src/app/init.sql -C
/opt/mssql-tools18/bin/sqlcmd -S localhost -U SA -P "$SA_PASSWORD" -i /usr/src/app/procedures.sql -C

# Keep the container running
wait