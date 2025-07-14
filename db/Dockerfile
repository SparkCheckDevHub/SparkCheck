# This Dockerfile copies and runs the required files to initialize the database
# schema and procedures.

# Use the Microsoft SQL Server Docker image
FROM mcr.microsoft.com/mssql/server:2022-latest

# Copy the initialization scripts
USER root
COPY init.sh /usr/src/app/init.sh
COPY init.sql /usr/src/app/init.sql
COPY procedures.sql /usr/src/app/procedures.sql
COPY zipcodes.sql /usr/src/app/zipcodes.sql

# Make the scripts executable and run
RUN chmod +x /usr/src/app/init.sh
ENTRYPOINT ["/bin/bash", "/usr/src/app/init.sh"]