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

-- --------------------------------------------------------------------------------
-- Stored Procedures
-- --------------------------------------------------------------------------------

-- Procedure: Get All Interests
IF OBJECT_ID('uspGetAllInterests', 'P') IS NOT NULL
    DROP PROCEDURE uspGetAllInterests;
GO

CREATE PROCEDURE uspGetAllInterests
AS
BEGIN
    SET NOCOUNT ON;

    SELECT  
        intInterestID,
        strInterest,
        intInterestCategoryID,
        intInterestSubCategoryID
    FROM TInterests
    ORDER BY strInterest;
END;
GO

-- --------------------------------------------------------------------------------
-- Procedure: Get User by ID
IF OBJECT_ID('uspGetUser', 'P') IS NOT NULL
    DROP PROCEDURE uspGetUser;
GO

CREATE PROCEDURE uspGetUser
    @intUserID INT
AS
BEGIN
    SET NOCOUNT ON;

    IF EXISTS (SELECT 1 FROM TUsers WHERE intUserID = @intUserID)
    BEGIN
        SELECT * FROM TUsers WHERE intUserID = @intUserID;
    END
    ELSE
    BEGIN
        SELECT 'Error: User not found.' AS Result;
    END
END;
GO

-- --------------------------------------------------------------------------------
-- Procedure: Get User Media
IF OBJECT_ID('uspGetUserMedia', 'P') IS NOT NULL
    DROP PROCEDURE uspGetUserMedia;
GO

CREATE PROCEDURE uspGetUserMedia
    @intUserID INT
AS
BEGIN
    SET NOCOUNT ON;

    IF EXISTS (SELECT 1 FROM TUsers WHERE intUserID = @intUserID)
    SELECT *
    FROM TUserMedia
    WHERE intUserID = @intUserID
    ORDER BY dtmUploadDate DESC;
END;
GO

-- --------------------------------------------------------------------------------
-- Procedure: Get User Interests
IF OBJECT_ID('uspGetUserInterests', 'P') IS NOT NULL
    DROP PROCEDURE uspGetUserInterests;
GO

CREATE PROCEDURE uspGetUserInterests
    @intUserID INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        I.intInterestID,
        I.strInterest
    FROM TUserInterests as UI
    INNER JOIN TInterests I ON UI.intInterestID = I.intInterestID
    WHERE UI.intUserID = @intUserID
    ORDER BY I.strInterest;
END;
GO

-- --------------------------------------------------------------------------------
-- Procedure: Update User
IF OBJECT_ID('uspUpdateUser', 'P') IS NOT NULL
    DROP PROCEDURE uspUpdateUser;
GO

CREATE PROCEDURE uspUpdateUser
     @intUserID INT,
    @strFirstName VARCHAR(250),
    @strLastName VARCHAR(250),
    @dtmDateOfBirth DATE,
    @strGender VARCHAR(50),
    @decLatitude DECIMAL(9,6),
    @decLongitude DECIMAL(9,6),
    @strZipCode VARCHAR(20)
AS
BEGIN
    SET NOCOUNT ON;

     DECLARE @intGenderID INT;
    SELECT @intGenderID = intGenderID FROM TGenders WHERE strGender = @strGender;

    IF @intGenderID IS NULL
    BEGIN
        SELECT 'Error: Gender not found in TGenders table.' AS Result;
        RETURN;
    END

    -- Look up Zip Code ID
    DECLARE @intZipCodeID INT;
    SELECT @intZipCodeID = intZipCodeID FROM TZipCodes WHERE strZipCode = @strZipCode;

    IF @intZipCodeID IS NULL
    BEGIN
        SELECT 'Error: Zip Code not found in TZipCodes table.' AS Result;
        RETURN;
    END

    -- Check if user exists and update
    IF EXISTS (SELECT 1 FROM TUsers WHERE intUserID = @intUserID)
    BEGIN
        UPDATE TUsers
        SET 
            strFirstName = @strFirstName,
            strLastName = @strLastName,
            dtmDateOfBirth = @dtmDateOfBirth,
            intGenderID = @intGenderID,
            decLatitude = @decLatitude,
            decLongitude = @decLongitude,
            intZipCodeID = @intZipCodeID
        WHERE intUserID = @intUserID;

        SELECT 'Success: User updated.' AS Result;
    END
    ELSE
    BEGIN
        SELECT 'Error: User not found in TUsers table.' AS Result;
    END
END;
GO

-- --------------------------------------------------------------------------------
-- Procedure: Upload User media
IF OBJECT_ID('uspCreateUserMedia', 'P') IS NOT NULL
    DROP PROCEDURE uspCreateUserMedia;
GO

CREATE PROCEDURE uspCreateUserMedia
    @intUserID INT,
    @strMediaURL VARBINARY(MAX),
    @blnOnProfile BIT,
    @blnIsFace BIT,
    @blnIsActive BIT
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO TUserMedia (
        intUserID, Photo, blnOnProfile, blnIsFace, blnIsActive, dtmUploadDate
    )
    VALUES (
        @intUserID, @strMediaURL, @blnOnProfile, @blnIsFace, @blnIsActive, GETDATE()
    );

    SELECT SCOPE_IDENTITY() AS NewMediaID, 'User media inserted successfully.' AS Result;
END;
GO

-- --------------------------------------------------------------------------------
-- Procedure: Update User Preferences
IF OBJECT_ID('uspUpdateUserPreferences', 'P') IS NOT NULL
    DROP PROCEDURE uspUpdateUserPreferences;
GO

CREATE PROCEDURE uspUpdateUserPreferences
    @intUserID INT,
    @intMatchDistance INT,
    @intMinAge INT,
    @intMaxAge INT,
    @intGenderPreferenceID INT,
    @blnReceiveEmails BIT,
    @blnShowProfile BIT,
    @strBio VARCHAR(350),
    @intAppUsageTypeID INT
AS
BEGIN
    SET NOCOUNT ON;

    IF EXISTS (SELECT 1 FROM TUserPreferences WHERE intUserID = @intUserID)
    BEGIN
        UPDATE TUserPreferences
        SET 
            intMatchDistance = @intMatchDistance,
            intMinAge = @intMinAge,
            intMaxAge = @intMaxAge,
            blnReceiveEmails = @blnReceiveEmails,
            blnShowProfile = @blnShowProfile,
            strBio = @strBio,
            intAppUsageTypeID = @intAppUsageTypeID
        WHERE intUserID = @intUserID;
    END
    ELSE
    BEGIN
        INSERT INTO TUserPreferences (
            intUserID, intMatchDistance, intMinAge, intMaxAge, blnReceiveEmails, blnShowProfile, strBio, intAppUsageTypeID
        )
        VALUES (
            @intUserID, @intMatchDistance, @intMinAge, @intMaxAge, @blnReceiveEmails, @blnShowProfile, @strBio, @intAppUsageTypeID
        );
    END

    SELECT 'User preferences updated successfully.' AS Result;
END;
GO

-- --------------------------------------------------------------------------------
-- Procedure: Add User Interest
IF OBJECT_ID('uspAddUserInterest', 'P') IS NOT NULL
    DROP PROCEDURE uspAddUserInterest;
GO

CREATE PROCEDURE uspAddUserInterest
    @intUserID INT,
    @intInterestID INT
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT EXISTS (
        SELECT 1 FROM TUserInterests
        WHERE intUserID = @intUserID AND intInterestID = @intInterestID
    )
    BEGIN
        INSERT INTO TUserInterests (intUserID, intInterestID)
        VALUES (@intUserID, @intInterestID);

        SELECT 'Success: Interest added for user.' AS Result;
    END
    ELSE
    BEGIN
        SELECT 'Info: Interest already exists for this user.' AS Result;
    END
END;
GO

-- --------------------------------------------------------------------------------
-- Procedure: Remove User Interest
IF OBJECT_ID('uspRemoveUserInterest', 'P') IS NOT NULL
    DROP PROCEDURE uspRemoveUserInterest;
GO

CREATE PROCEDURE uspRemoveUserInterest
    @intUserID INT,
    @intInterestID INT
AS
BEGIN
    SET NOCOUNT ON;

    IF EXISTS (
        SELECT 1 FROM TUserInterests
        WHERE intUserID = @intUserID AND intInterestID = @intInterestID
    )
    BEGIN
        DELETE FROM TUserInterests
        WHERE intUserID = @intUserID AND intInterestID = @intInterestID;

        SELECT 'Success: Interest removed for user.' AS Result;
    END
    ELSE
    BEGIN
        SELECT 'Error: User does not have this interest.' AS Result;
    END
END;
GO