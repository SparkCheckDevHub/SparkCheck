-- --------------------------------------------------------------------------------
-- Name: Seth Niefield
-- Project: SparkCheck
-- Abstract: SparkCheck Stored Procedures
-- --------------------------------------------------------------------------------

-- --------------------------------------------------------------------------------
-- Options
-- --------------------------------------------------------------------------------

-- Wait for DB to be created
WHILE DB_ID('dbSparkCheck') IS NULL
BEGIN
    PRINT 'Waiting for database dbSparkCheck...'
    WAITFOR DELAY '00:00:05'
END

-- Switch to DB
USE dbSparkCheck;
GO

SET NOCOUNT ON;
GO

-- TODO: Add stored procedures