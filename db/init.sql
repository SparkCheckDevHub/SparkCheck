-- --------------------------------------------------------------------------------
-- Name: Seth Niefield
-- Project: SparkCheck
-- Abstract: SparkCheck Database
-- --------------------------------------------------------------------------------

-- --------------------------------------------------------------------------------
-- Create Database
-- --------------------------------------------------------------------------------
IF EXISTS (SELECT * FROM sys.databases WHERE name = 'dbSparkCheck')
BEGIN
  THROW 50000, 'Database already exists', 1;
END
GO

CREATE DATABASE dbSparkCheck;
GO

-- --------------------------------------------------------------------------------
-- Options
-- --------------------------------------------------------------------------------
USE dbSparkCheck;
GO

SET NOCOUNT ON;
GO

-- --------------------------------------------------------------------------------
-- Drop Tables
-- --------------------------------------------------------------------------------
IF OBJECT_ID('TUserInterests') IS NOT NULL DROP TABLE TUserInterests;
IF OBJECT_ID('TInterests') IS NOT NULL DROP TABLE TInterests;
IF OBJECT_ID('TUserLogs') IS NOT NULL DROP TABLE TUserLogs;
IF OBJECT_ID('TLoginAttempts') IS NOT NULL DROP TABLE TLoginAttempts;
IF OBJECT_ID('TReports') IS NOT NULL DROP TABLE TReports;
IF OBJECT_ID('TUserPreferences') IS NOT NULL DROP TABLE TUserPreferences;
IF OBJECT_ID('TUserAppSettings') IS NOT NULL DROP TABLE TUserAppSettings;
IF OBJECT_ID('TChatMessages') IS NOT NULL DROP TABLE TChatMessages;
IF OBJECT_ID('TUserMedia') IS NOT NULL DROP TABLE TUserMedia;
IF OBJECT_ID('TChatEvents') IS NOT NULL DROP TABLE TChatEvents;
IF OBJECT_ID('TMatches') IS NOT NULL DROP TABLE TMatches;
IF OBJECT_ID('TMatchRequests') IS NOT NULL DROP TABLE TMatchRequests;
IF OBJECT_ID('TUserLogTypes') IS NOT NULL DROP TABLE TUserLogTypes;
IF OBJECT_ID('TReportReasons') IS NOT NULL DROP TABLE TReportReasons;
IF OBJECT_ID('TChatEventTypes') IS NOT NULL DROP TABLE TChatEventTypes;
IF OBJECT_ID('TAppUsageTypes') IS NOT NULL DROP TABLE TAppUsageTypes;
IF OBJECT_ID('TInterestSubCategories') IS NOT NULL DROP TABLE TInterestSubCategories;
IF OBJECT_ID('TInterestCategories') IS NOT NULL DROP TABLE TInterestCategories;
IF OBJECT_ID('TUsers') IS NOT NULL DROP TABLE TUsers;
IF OBJECT_ID('TZipCodes') IS NOT NULL DROP TABLE TZipCodes;
IF OBJECT_ID('TCities') IS NOT NULL DROP TABLE TCities;
IF OBJECT_ID('TStates') IS NOT NULL DROP TABLE TStates;
IF OBJECT_ID('TGenders') IS NOT NULL DROP TABLE TGenders;

-- --------------------------------------------------------------------------------
-- Establish Tables
-- --------------------------------------------------------------------------------
CREATE TABLE TUsers (
  intUserID           INTEGER IDENTITY(1,1) NOT NULL,
  strEmail            VARCHAR(250)  NOT NULL,
  strPhone            VARCHAR(250)  NOT NULL,
  strUsername         VARCHAR(250)  NOT NULL,
  strFirstName        VARCHAR(250)  NOT NULL,
  strLastName         VARCHAR(250)  NOT NULL,
  dtmDateOfBirth      DATE          NOT NULL,
  intGenderID         INTEGER       NOT NULL,
  decLatitude         DECIMAL(9,6)  NULL,
  decLongitude        DECIMAL(9,6)  NULL,
  intZipCodeID        INTEGER       NULL,
  blnIsActive         BIT           NOT NULL,
  blnIsOnline         BIT           NOT NULL,
  blnInQueue          BIT           NOT NULL,
  dtmCreatedDate      DATE          NOT NULL,
  dtmQueuedDate       DATE          NULL,
  strUserToken        VARCHAR(250)  NULL,
  CONSTRAINT TUsers_PK PRIMARY KEY (intUserID),
  CONSTRAINT TUsers_UQ_Username UNIQUE (strUsername),
  CONSTRAINT TUsers_UQ_Email UNIQUE (strEmail),
  CONSTRAINT TUsers_UQ_Phone UNIQUE (strPhone)
);

CREATE TABLE TZipCodes (
  intZipCodeID  INTEGER IDENTITY(1,1) NOT NULL,
  strZipCode    VARCHAR(15)   NOT NULL,
  intCityID     INTEGER       NOT NULL,
  decLatitude   DECIMAL(9,6)  NOT NULL,
  decLongitude  DECIMAL(9,6)  NOT NULL,
  CONSTRAINT TZipCodes_PK PRIMARY KEY (intZipCodeID)
);

CREATE TABLE TStates (
  intStateID     INTEGER      NOT NULL,
  strStateCode   VARCHAR(5)   NOT NULL,
  strState       VARCHAR(250) NOT NULL,
  CONSTRAINT TStates_PK PRIMARY KEY (intStateID)
);

CREATE TABLE TCities (
  intCityID    INTEGER      NOT NULL,
  strCity      VARCHAR(250) NOT NULL,
  intStateID   INTEGER      NOT NULL,
  CONSTRAINT TCities_PK PRIMARY KEY (intCityID)
);

CREATE TABLE TGenders (
  intGenderID  INTEGER IDENTITY(1,1) NOT NULL,
  strGender    VARCHAR(250) NOT NULL,
  CONSTRAINT TGenders_PK PRIMARY KEY (intGenderID)
);

CREATE TABLE TMatches (
  intMatchID            INTEGER IDENTITY(1,1) NOT NULL,
  intMatchRequestID     INTEGER     NOT NULL,
  blnFirstUserDeleted   BIT         NOT NULL,
  blnSecondUserDeleted  BIT         NOT NULL,
  dtmMatchStarted       DATETIME    NOT NULL,
  dtmMatchEnded         DATETIME    NULL,
  CONSTRAINT TMatches_PK PRIMARY KEY (intMatchID)
);

CREATE TABLE TMatchRequests (
  intMatchRequestID     INTEGER     NOT NULL,
  intFirstUserID        INTEGER     NOT NULL,
  intSecondUserID       INTEGER     NOT NULL,
  blnFirstUserDeclined  BIT         NOT NULL,
  blnSecondUserDeclined BIT         NOT NULL,
  dtmRequestStarted     DATETIME    NOT NULL,
  dtmRequestEnded       DATETIME    NULL,
  CONSTRAINT TMatchRequests_PK PRIMARY KEY (intMatchRequestID)
);

CREATE TABLE TLoginAttempts (
  intLoginAttemptID     INTEGER IDENTITY(1,1) NOT NULL,
  strPhone              VARCHAR(20)  NOT NULL,
  strVerificationCode   VARCHAR(20)  NOT NULL,
  dtmLoginDate          DATETIME     NOT NULL,
  strIPAddress          VARCHAR(250) NULL,
  strUserAgent          VARCHAR(250) NULL,
  intUserID             INTEGER      NOT NULL,
  blnIsActive           BIT          NOT NULL,
  intAttempts           INTEGER      NOT NULL,
  CONSTRAINT TLoginAttempts_PK PRIMARY KEY (intLoginAttemptID)
);

CREATE TABLE TUserLogs (
  intUserLogID      INTEGER IDENTITY(1,1) NOT NULL,
  intUserID         INTEGER       NOT NULL,
  intUserLogTypeID  INTEGER       NOT NULL,
  dtmLoginDate      DATETIME      NOT NULL,
  strIPAddress      VARCHAR(250)  NOT NULL,
  strUserAgent      VARCHAR(250)  NOT NULL,
  strComment        VARCHAR(1000) NULL,
  intAttributeName  INTEGER       NOT NULL,
  CONSTRAINT TUserLogs_PK PRIMARY KEY (intUserLogID)
);

CREATE TABLE TUserLogTypes (
  intUserLogTypeID  INTEGER IDENTITY(1,1) NOT NULL,
  strUserLogType    VARCHAR(250) NOT NULL,
  CONSTRAINT TUserLogTypes_PK PRIMARY KEY (intUserLogTypeID)
);

CREATE TABLE TReports (
  intReportID        INTEGER IDENTITY(1,1) NOT NULL,
  intMatchID         INTEGER      NOT NULL,
  intUserID          INTEGER      NOT NULL,
  intMatchRequestID  INTEGER      NOT NULL,
  strComment         VARCHAR(250) NOT NULL,
  dtmReportDate      DATETIME     NOT NULL,
  intReportReasonID  INTEGER      NOT NULL,
  CONSTRAINT TReports_PK PRIMARY KEY (intReportID)
);

CREATE TABLE TReportReasons (
  intReportReasonID  INTEGER IDENTITY(1,1) NOT NULL,
  strReportReason    VARCHAR(250) NOT NULL,
  CONSTRAINT TReportReasons_PK PRIMARY KEY (intReportReasonID)
);

CREATE TABLE TChatMessages (
  intChatMessageID  INTEGER IDENTITY(1,1) NOT NULL,
  intSenderUserID   INTEGER       NOT NULL,
  intUserMediaID    INTEGER       NULL,
  intMatchID        INTEGER       NOT NULL,
  strMessageText    VARCHAR(1000) NOT NULL,
  dtmSentAt         DATETIME      NOT NULL,
  blnIsActive       BIT           NOT NULL,
  CONSTRAINT TChatMessages_PK PRIMARY KEY (intChatMessageID)
);

CREATE TABLE TUserMedia (
  intUserMediaID  INTEGER IDENTITY(1,1) NOT NULL,
  intUserID       INTEGER      NOT NULL,
  strMediaURL     VARCHAR(250) NOT NULL,
  blnOnProfile    BIT          NOT NULL,
  blnIsFace       BIT          NOT NULL,
  blnIsActive     BIT          NOT NULL,
  dtmUploadDate   DATETIME     NOT NULL,
  CONSTRAINT TUserMedia_PK PRIMARY KEY (intUserMediaID)
);

CREATE TABLE TUserPreferences (
  intUserID             INTEGER      NOT NULL,
  intGenderPreferenceID INTEGER,
  intMatchDistance      INTEGER,
  intMinAge             INTEGER,
  intMaxAge             INTEGER,
  blnReceiveEmails      BIT          NOT NULL,
  blnShowProfile        BIT          NOT NULL,
  strBio                VARCHAR(350) NOT NULL,
  intAppUsageTypeID     INTEGER      NOT NULL
);

CREATE TABLE TUserAppSettings (
  intUserID           INTEGER      NOT NULL,
  strAppLanguage      VARCHAR(50)  NOT NULL,
  strThemePreference  VARCHAR(25)  NOT NULL
);

CREATE TABLE TChatEvents (
  intChatEventID       INTEGER IDENTITY(1,1) NOT NULL,
  intMatchID           INTEGER      NOT NULL,
  intUserID            INTEGER      NOT NULL,
  intChatEventTypeID   INTEGER      NOT NULL,
  strChatEvent         VARCHAR(250) NOT NULL,
  CONSTRAINT TChatEvents_PK PRIMARY KEY (intChatEventID)
);

CREATE TABLE TChatEventTypes (
  intChatEventTypeID  INTEGER IDENTITY(1,1) NOT NULL,
  strChatEvent        VARCHAR(250) NOT NULL,
  CONSTRAINT TChatEventTypes_PK PRIMARY KEY (intChatEventTypeID)
);

CREATE TABLE TInterests (
  intInterestID             INTEGER IDENTITY(1,1) NOT NULL,
  strInterest               VARCHAR(250) NOT NULL,
  intInterestCategoryID     INTEGER      NOT NULL,
  intInterestSubCategoryID  INTEGER,
  CONSTRAINT TInterests_PK PRIMARY KEY (intInterestID)
);

CREATE TABLE TInterestCategories (
  intInterestCategoryID  INTEGER IDENTITY(1,1) NOT NULL,
  strInterestCategory    VARCHAR(250) NOT NULL,
  CONSTRAINT TInterestCategories_PK PRIMARY KEY (intInterestCategoryID)
);

CREATE TABLE TInterestSubCategories (
  intInterestSubCategoryID  INTEGER IDENTITY(1,1) NOT NULL,
  strInterestSubCategory    VARCHAR(250) NOT NULL,
  CONSTRAINT TInterestSubCategories_PK PRIMARY KEY (intInterestSubCategoryID)
);

CREATE TABLE TUserInterests (
  intUserInterestID  INTEGER IDENTITY(1,1) NOT NULL,
  intUserID          INTEGER NOT NULL,
  intInterestID      INTEGER NOT NULL,
  CONSTRAINT TUserInterests_PK PRIMARY KEY (intUserInterestID)
);

CREATE TABLE TAppUsageTypes (
  intAppUsageTypeID  INTEGER IDENTITY(1,1) NOT NULL,
  strAppUsageType    VARCHAR(250) NOT NULL,
  CONSTRAINT TAppUsageTypes_PK PRIMARY KEY (intAppUsageTypeID)
);

-- --------------------------------------------------------------------------------
-- Step #1.2: Identify and Create Foreign Keys
-- --------------------------------------------------------------------------------
--
-- #	  Child										Parent							    Column(s)
-- -	  -----										------							    ---------
-- 1	  TUserInterests					TUsers							    intUserID --
-- 2	  TUserInterests					TInterests						  intInterestID --
-- 3	  TInterests							TInterestCategories			intInterestCategoryID --
-- 4	  TInterests							TInterestSubcategories	intInterestSubCategoryID --
-- 5	  TLoginAttempts					TUsers							    intUserID --
-- 6	  TUserLogs								TUsers							    intUserID --
-- 7	  TReports								TUsers							    intUserID --
-- 8	  TMatchRequests					TUsers							    intFirstUserID --
-- 8.5	TMatchRequests			  	TUsers							    intSecondUserID --
-- 9	  TUserPreferences				TUsers							    intUserID --
-- 10	  TUserAppSettings				TUsers							    intUserID --
-- 11	  TChatMessages						TUsers							    intUserID --
-- 12	  TUserMedia							TUsers							    intUserID --
-- 13	  TChatEvents							TUsers							    intUserID --
-- 14	  TReports								TReportReasons					intReportReasonID --
-- 15	  TMatches								TMatchRequests					intMatchRequestID --
-- 16	  TChatMessages						TMatches						    intMatchID --
-- 17	  TUserMedia							TUserMedia						  intUserMediaID --
-- 18	  TUserPreferences				TAppUsageTypes					intAppUsageTypeID --
-- 19	  TChatEvents							TMatches						    intMatchID --
-- 20	  TChatEvents							ChatEventTypes					intChatEventTypeID --
-- 21	  TUser										TZipCodes						    intZipCodeID
-- 22	  TCities									TStates							    intStateID
-- 23	  TZipCodes								TCities							    intCityID
-- 24	  TUser										TGenders						    intGenderID
-- 25	  TUserPreferences				TGenders						    intGenderPreferenceID

-- #25
ALTER TABLE TUserPreferences ADD CONSTRAINT TUserPreferences_TGenders_FK
FOREIGN KEY ( intGenderPreferenceID ) REFERENCES TGenders ( intGenderID );

-- #24
ALTER TABLE TUsers ADD CONSTRAINT TUsers_TGenders_FK
FOREIGN KEY ( intGenderID ) REFERENCES TGenders ( intGenderID );

-- #21
ALTER TABLE TUsers ADD CONSTRAINT TUsers_TZipCodes_FK
FOREIGN KEY ( intZipCodeID ) REFERENCES TZipCodes ( intZipCodeID );

-- #22
ALTER TABLE TCities ADD CONSTRAINT TCities_TStates_FK
FOREIGN KEY ( intStateID ) REFERENCES TStates ( intStateID );

-- #23
ALTER TABLE TZipCodes ADD CONSTRAINT TZipCodes_TCities_FK
FOREIGN KEY ( intCityID ) REFERENCES TCities ( intCityID );

-- #3
ALTER TABLE TInterests ADD CONSTRAINT TInterests_TInterestCategories_FK
FOREIGN KEY ( intInterestCategoryID ) REFERENCES TInterestCategories ( intInterestCategoryID );

-- #4
ALTER TABLE TInterests ADD CONSTRAINT TInterests_TInterestSubcategories_FK
FOREIGN KEY ( intInterestSubCategoryID ) REFERENCES TInterestSubCategories ( intInterestSubCategoryID );

-- #18
ALTER TABLE TUserPreferences ADD CONSTRAINT TUserPreferences_TAppUsageTypes_FK
FOREIGN KEY ( intAppUsageTypeID ) REFERENCES TAppUsageTypes ( intAppUsageTypeID );

-- #20
ALTER TABLE TChatEvents ADD CONSTRAINT TChatEvents_TChatEventTypes_FK
FOREIGN KEY ( intChatEventTypeID ) REFERENCES TChatEventTypes ( intChatEventTypeID );

-- #15
ALTER TABLE TMatches ADD CONSTRAINT TMatches_TMatchRequests_FK
FOREIGN KEY ( intMatchRequestID ) REFERENCES TMatchRequests ( intMatchRequestID );

-- #16
ALTER TABLE TChatMessages ADD CONSTRAINT TChatMessages_TMatches_FK
FOREIGN KEY ( intMatchID ) REFERENCES TMatches ( intMatchID );

-- #19
ALTER TABLE TChatEvents ADD CONSTRAINT TChatEvents_TMatches_FK
FOREIGN KEY ( intMatchID ) REFERENCES TMatches ( intMatchID );

-- #17
ALTER TABLE TUserMedia ADD CONSTRAINT TUserMedia_TUserMedia_FK
FOREIGN KEY ( intUserMediaID ) REFERENCES TUserMedia ( intUserMediaID );

-- #1
ALTER TABLE TUserInterests ADD CONSTRAINT TUserInterests_TUsers_FK
FOREIGN KEY ( intUserID ) REFERENCES TUsers ( intUserID );

-- #2
ALTER TABLE TUserInterests ADD CONSTRAINT TUserInterests_TInterests_FK
FOREIGN KEY ( intInterestID ) REFERENCES TInterests ( intInterestID );

-- #5
ALTER TABLE TLoginAttempts ADD CONSTRAINT TLoginAttempts_TUsers_FK
FOREIGN KEY ( intUserID ) REFERENCES TUsers ( intUserID );

-- #6
ALTER TABLE TUserLogs ADD CONSTRAINT TUserLogs_TUsers_FK
FOREIGN KEY ( intUserID ) REFERENCES TUsers ( intUserID );

-- #7
ALTER TABLE TReports ADD CONSTRAINT TReports_TUsers_FK
FOREIGN KEY ( intUserID ) REFERENCES TUsers ( intUserID );

-- #8
ALTER TABLE TMatchRequests ADD CONSTRAINT TMatchRequests_TUsers_FK
FOREIGN KEY ( intFirstUserID ) REFERENCES TUsers ( intUserID );

-- #8.5
ALTER TABLE TMatchRequests ADD CONSTRAINT TMatchRequests_TUsers_FK2
FOREIGN KEY ( intSecondUserID ) REFERENCES TUsers ( intUserID );

-- #9
ALTER TABLE TUserPreferences ADD CONSTRAINT TUserPreferences_TUsers_FK
FOREIGN KEY ( intUserID ) REFERENCES TUsers ( intUserID );

-- #10
ALTER TABLE TUserAppSettings ADD CONSTRAINT TUserAppSettings_TUsers_FK
FOREIGN KEY ( intUserID ) REFERENCES TUsers ( intUserID );

-- #11
ALTER TABLE TChatMessages ADD CONSTRAINT TChatMessages_TUsers_FK
FOREIGN KEY ( intSenderUserID ) REFERENCES TUsers ( intUserID );

-- #12
ALTER TABLE TUserMedia ADD CONSTRAINT TUserMedia_TUsers_FK
FOREIGN KEY ( intUserID ) REFERENCES TUsers ( intUserID );

-- #13
ALTER TABLE TChatEvents ADD CONSTRAINT TChatEvents_TUsers_FK
FOREIGN KEY ( intUserID ) REFERENCES TUsers ( intUserID );

-- #14
ALTER TABLE TReports ADD CONSTRAINT TReports_TReportReasons_FK
FOREIGN KEY ( intReportReasonID ) REFERENCES TReportReasons ( intReportReasonID );

-- --------------------------------------------------------------------------------
-- Add Records into Genders
-- --------------------------------------------------------------------------------

INSERT INTO TGenders ( strGender )
VALUES ( 'Unspecified' )
      ,( 'Male' )
      ,( 'Female' )
      ,( 'Non-Binary' )