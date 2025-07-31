"""
Defines data models.
"""

from typing import List, Optional
import datetime
import decimal

from sqlalchemy import (
    Boolean,
    Column,
    DECIMAL,
    Date,
    DateTime,
    ForeignKeyConstraint,
    Identity,
    Index,
    Integer,
    PrimaryKeyConstraint,
    String,
    Table,
)
from sqlalchemy.orm import DeclarativeBase, Mapped, mapped_column, relationship


class Base(DeclarativeBase):
    pass


class TAppUsageTypes(Base):
    __tablename__ = 'TAppUsageTypes'
    __table_args__ = (
        PrimaryKeyConstraint('intAppUsageTypeID', name='TAppUsageTypes_PK'),
    )

    intAppUsageTypeID: Mapped[int] = mapped_column(Integer, Identity(start=1, increment=1), primary_key=True)
    strAppUsageType: Mapped[str] = mapped_column(String(250, 'SQL_Latin1_General_CP1_CI_AS'))


class TChatEventTypes(Base):
    __tablename__ = 'TChatEventTypes'
    __table_args__ = (
        PrimaryKeyConstraint('intChatEventTypeID', name='TChatEventTypes_PK'),
    )

    intChatEventTypeID: Mapped[int] = mapped_column(Integer, Identity(start=1, increment=1), primary_key=True)
    strChatEvent: Mapped[str] = mapped_column(String(250, 'SQL_Latin1_General_CP1_CI_AS'))

    TChatEvents: Mapped[List['TChatEvents']] = relationship('TChatEvents', back_populates='TChatEventTypes_')


class TGenders(Base):
    __tablename__ = 'TGenders'
    __table_args__ = (
        PrimaryKeyConstraint('intGenderID', name='TGenders_PK'),
    )

    intGenderID: Mapped[int] = mapped_column(Integer, Identity(start=1, increment=1), primary_key=True)
    strGender: Mapped[str] = mapped_column(String(250, 'SQL_Latin1_General_CP1_CI_AS'))

    TUsers: Mapped[List['TUsers']] = relationship('TUsers', back_populates='TGenders_')


class TInterestCategories(Base):
    __tablename__ = 'TInterestCategories'
    __table_args__ = (
        PrimaryKeyConstraint('intInterestCategoryID', name='TInterestCategories_PK'),
    )

    intInterestCategoryID: Mapped[int] = mapped_column(Integer, Identity(start=1, increment=1), primary_key=True)
    strInterestCategory: Mapped[str] = mapped_column(String(250, 'SQL_Latin1_General_CP1_CI_AS'))

    TInterests: Mapped[List['TInterests']] = relationship('TInterests', back_populates='TInterestCategories_')


class TInterestSubCategories(Base):
    __tablename__ = 'TInterestSubCategories'
    __table_args__ = (
        PrimaryKeyConstraint('intInterestSubCategoryID', name='TInterestSubCategories_PK'),
    )

    intInterestSubCategoryID: Mapped[int] = mapped_column(Integer, Identity(start=1, increment=1), primary_key=True)
    strInterestSubCategory: Mapped[str] = mapped_column(String(250, 'SQL_Latin1_General_CP1_CI_AS'))

    TInterests: Mapped[List['TInterests']] = relationship('TInterests', back_populates='TInterestSubCategories_')


class TReportReasons(Base):
    __tablename__ = 'TReportReasons'
    __table_args__ = (
        PrimaryKeyConstraint('intReportReasonID', name='TReportReasons_PK'),
    )

    intReportReasonID: Mapped[int] = mapped_column(Integer, Identity(start=1, increment=1), primary_key=True)
    strReportReason: Mapped[str] = mapped_column(String(250, 'SQL_Latin1_General_CP1_CI_AS'))

    TReports: Mapped[List['TReports']] = relationship('TReports', back_populates='TReportReasons_')


class TStates(Base):
    __tablename__ = 'TStates'
    __table_args__ = (
        PrimaryKeyConstraint('intStateID', name='TStates_PK'),
    )

    intStateID: Mapped[int] = mapped_column(Integer, primary_key=True)
    strStateCode: Mapped[str] = mapped_column(String(5, 'SQL_Latin1_General_CP1_CI_AS'))
    strState: Mapped[str] = mapped_column(String(250, 'SQL_Latin1_General_CP1_CI_AS'))

    TCities: Mapped[List['TCities']] = relationship('TCities', back_populates='TStates_')


class TUserLogTypes(Base):
    __tablename__ = 'TUserLogTypes'
    __table_args__ = (
        PrimaryKeyConstraint('intUserLogTypeID', name='TUserLogTypes_PK'),
    )

    intUserLogTypeID: Mapped[int] = mapped_column(Integer, Identity(start=1, increment=1), primary_key=True)
    strUserLogType: Mapped[str] = mapped_column(String(250, 'SQL_Latin1_General_CP1_CI_AS'))


class TCities(Base):
    __tablename__ = 'TCities'
    __table_args__ = (
        ForeignKeyConstraint(['intStateID'], ['TStates.intStateID'], name='TCities_TStates_FK'),
        PrimaryKeyConstraint('intCityID', name='TCities_PK')
    )

    intCityID: Mapped[int] = mapped_column(Integer, primary_key=True)
    strCity: Mapped[str] = mapped_column(String(250, 'SQL_Latin1_General_CP1_CI_AS'))
    intStateID: Mapped[int] = mapped_column(Integer)

    TStates_: Mapped['TStates'] = relationship('TStates', back_populates='TCities')
    TZipCodes: Mapped[List['TZipCodes']] = relationship('TZipCodes', back_populates='TCities_')


class TInterests(Base):
    __tablename__ = 'TInterests'
    __table_args__ = (
        ForeignKeyConstraint(['intInterestCategoryID'], ['TInterestCategories.intInterestCategoryID'], name='TInterests_TInterestCategories_FK'),
        ForeignKeyConstraint(['intInterestSubCategoryID'], ['TInterestSubCategories.intInterestSubCategoryID'], name='TInterests_TInterestSubcategories_FK'),
        PrimaryKeyConstraint('intInterestID', name='TInterests_PK')
    )

    intInterestID: Mapped[int] = mapped_column(Integer, Identity(start=1, increment=1), primary_key=True)
    strInterest: Mapped[str] = mapped_column(String(250, 'SQL_Latin1_General_CP1_CI_AS'))
    intInterestCategoryID: Mapped[int] = mapped_column(Integer)
    intInterestSubCategoryID: Mapped[Optional[int]] = mapped_column(Integer)

    TInterestCategories_: Mapped['TInterestCategories'] = relationship('TInterestCategories', back_populates='TInterests')
    TInterestSubCategories_: Mapped[Optional['TInterestSubCategories']] = relationship('TInterestSubCategories', back_populates='TInterests')
    TUserInterests: Mapped[List['TUserInterests']] = relationship('TUserInterests', back_populates='TInterests_')


class TZipCodes(Base):
    __tablename__ = 'TZipCodes'
    __table_args__ = (
        ForeignKeyConstraint(['intCityID'], ['TCities.intCityID'], name='TZipCodes_TCities_FK'),
        PrimaryKeyConstraint('intZipCodeID', name='TZipCodes_PK')
    )

    intZipCodeID: Mapped[int] = mapped_column(Integer, Identity(start=1, increment=1), primary_key=True)
    strZipCode: Mapped[str] = mapped_column(String(15, 'SQL_Latin1_General_CP1_CI_AS'))
    intCityID: Mapped[int] = mapped_column(Integer)
    decLatitude: Mapped[decimal.Decimal] = mapped_column(DECIMAL(9, 6))
    decLongitude: Mapped[decimal.Decimal] = mapped_column(DECIMAL(9, 6))

    TCities_: Mapped['TCities'] = relationship('TCities', back_populates='TZipCodes')
    TUsers: Mapped[List['TUsers']] = relationship('TUsers', back_populates='TZipCodes_')


class TUsers(Base):
    __tablename__ = 'TUsers'
    __table_args__ = (
        ForeignKeyConstraint(['intGenderID'], ['TGenders.intGenderID'], name='TUsers_TGenders_FK'),
        ForeignKeyConstraint(['intZipCodeID'], ['TZipCodes.intZipCodeID'], name='TUsers_TZipCodes_FK'),
        PrimaryKeyConstraint('intUserID', name='TUsers_PK'),
        Index('TUsers_UQ_Email', 'strEmail', unique=True),
        Index('TUsers_UQ_Phone', 'strPhone', unique=True),
        Index('TUsers_UQ_Username', 'strUsername', unique=True)
    )

    intUserID: Mapped[int] = mapped_column(Integer, Identity(start=1, increment=1), primary_key=True)
    strEmail: Mapped[str] = mapped_column(String(250, 'SQL_Latin1_General_CP1_CI_AS'))
    strPhone: Mapped[str] = mapped_column(String(250, 'SQL_Latin1_General_CP1_CI_AS'))
    strUsername: Mapped[str] = mapped_column(String(250, 'SQL_Latin1_General_CP1_CI_AS'))
    strFirstName: Mapped[str] = mapped_column(String(250, 'SQL_Latin1_General_CP1_CI_AS'))
    strLastName: Mapped[str] = mapped_column(String(250, 'SQL_Latin1_General_CP1_CI_AS'))
    dtmDateOfBirth: Mapped[datetime.date] = mapped_column(Date)
    intGenderID: Mapped[int] = mapped_column(Integer)
    blnIsActive: Mapped[bool] = mapped_column(Boolean)
    blnIsOnline: Mapped[bool] = mapped_column(Boolean)
    blnInQueue: Mapped[bool] = mapped_column(Boolean)
    dtmCreatedDate: Mapped[datetime.date] = mapped_column(Date)
    decLatitude: Mapped[Optional[decimal.Decimal]] = mapped_column(DECIMAL(9, 6))
    decLongitude: Mapped[Optional[decimal.Decimal]] = mapped_column(DECIMAL(9, 6))
    intZipCodeID: Mapped[Optional[int]] = mapped_column(Integer)
    dtmQueuedDate: Mapped[Optional[datetime.date]] = mapped_column(Date)
    strUserToken: Mapped[Optional[str]] = mapped_column(String(250, 'SQL_Latin1_General_CP1_CI_AS'))

    TGenders_: Mapped['TGenders'] = relationship('TGenders', back_populates='TUsers')
    TZipCodes_: Mapped[Optional['TZipCodes']] = relationship('TZipCodes', back_populates='TUsers')
    TLoginAttempts: Mapped[List['TLoginAttempts']] = relationship('TLoginAttempts', back_populates='TUsers_')
    TMatchRequests: Mapped[List['TMatchRequests']] = relationship('TMatchRequests', foreign_keys='[TMatchRequests.intFirstUserID]', back_populates='TUsers_')
    TMatchRequests_: Mapped[List['TMatchRequests']] = relationship('TMatchRequests', foreign_keys='[TMatchRequests.intSecondUserID]', back_populates='TUsers1')
    TReports: Mapped[List['TReports']] = relationship('TReports', back_populates='TUsers_')
    TUserInterests: Mapped[List['TUserInterests']] = relationship('TUserInterests', back_populates='TUsers_')
    TUserLogs: Mapped[List['TUserLogs']] = relationship('TUserLogs', back_populates='TUsers_')
    TUserMedia: Mapped[List['TUserMedia']] = relationship('TUserMedia', back_populates='TUsers_')
    TChatEvents: Mapped[List['TChatEvents']] = relationship('TChatEvents', back_populates='TUsers_')
    TChatMessages: Mapped[List['TChatMessages']] = relationship('TChatMessages', back_populates='TUsers_')


class TLoginAttempts(Base):
    __tablename__ = 'TLoginAttempts'
    __table_args__ = (
        ForeignKeyConstraint(['intUserID'], ['TUsers.intUserID'], name='TLoginAttempts_TUsers_FK'),
        PrimaryKeyConstraint('intLoginAttemptID', name='TLoginAttempts_PK')
    )

    intLoginAttemptID: Mapped[int] = mapped_column(Integer, Identity(start=1, increment=1), primary_key=True)
    strPhone: Mapped[str] = mapped_column(String(20, 'SQL_Latin1_General_CP1_CI_AS'))
    strVerificationCode: Mapped[str] = mapped_column(String(20, 'SQL_Latin1_General_CP1_CI_AS'))
    dtmLoginDate: Mapped[datetime.datetime] = mapped_column(DateTime)
    intUserID: Mapped[int] = mapped_column(Integer)
    blnIsActive: Mapped[bool] = mapped_column(Boolean)
    intAttempts: Mapped[int] = mapped_column(Integer)
    strIPAddress: Mapped[Optional[str]] = mapped_column(String(250, 'SQL_Latin1_General_CP1_CI_AS'))
    strUserAgent: Mapped[Optional[str]] = mapped_column(String(250, 'SQL_Latin1_General_CP1_CI_AS'))

    TUsers_: Mapped['TUsers'] = relationship('TUsers', back_populates='TLoginAttempts')


class TMatchRequests(Base):
    __tablename__ = 'TMatchRequests'
    __table_args__ = (
        ForeignKeyConstraint(['intFirstUserID'], ['TUsers.intUserID'], name='TMatchRequests_TUsers_FK'),
        ForeignKeyConstraint(['intSecondUserID'], ['TUsers.intUserID'], name='TMatchRequests_TUsers_FK2'),
        PrimaryKeyConstraint('intMatchRequestID', name='TMatchRequests_PK')
    )

    intMatchRequestID: Mapped[int] = mapped_column(Integer, primary_key=True, autoincrement=False)
    intFirstUserID: Mapped[int] = mapped_column(Integer)
    intSecondUserID: Mapped[int] = mapped_column(Integer)
    blnFirstUserDeclined: Mapped[bool] = mapped_column(Boolean)
    blnSecondUserDeclined: Mapped[bool] = mapped_column(Boolean)
    dtmRequestStarted: Mapped[datetime.datetime] = mapped_column(DateTime)
    dtmRequestEnded: Mapped[Optional[datetime.datetime]] = mapped_column(DateTime)

    TUsers_: Mapped['TUsers'] = relationship('TUsers', foreign_keys=[intFirstUserID], back_populates='TMatchRequests')
    TUsers1: Mapped['TUsers'] = relationship('TUsers', foreign_keys=[intSecondUserID], back_populates='TMatchRequests_')
    TMatches: Mapped[List['TMatches']] = relationship('TMatches', back_populates='TMatchRequests_')


class TReports(Base):
    __tablename__ = 'TReports'
    __table_args__ = (
        ForeignKeyConstraint(['intReportReasonID'], ['TReportReasons.intReportReasonID'], name='TReports_TReportReasons_FK'),
        ForeignKeyConstraint(['intUserID'], ['TUsers.intUserID'], name='TReports_TUsers_FK'),
        PrimaryKeyConstraint('intReportID', name='TReports_PK')
    )

    intReportID: Mapped[int] = mapped_column(Integer, Identity(start=1, increment=1), primary_key=True)
    intMatchID: Mapped[int] = mapped_column(Integer)
    intUserID: Mapped[int] = mapped_column(Integer)
    intMatchRequestID: Mapped[int] = mapped_column(Integer)
    strComment: Mapped[str] = mapped_column(String(250, 'SQL_Latin1_General_CP1_CI_AS'))
    dtmReportDate: Mapped[datetime.datetime] = mapped_column(DateTime)
    intReportReasonID: Mapped[int] = mapped_column(Integer)

    TReportReasons_: Mapped['TReportReasons'] = relationship('TReportReasons', back_populates='TReports')
    TUsers_: Mapped['TUsers'] = relationship('TUsers', back_populates='TReports')


t_TUserAppSettings = Table(
    'TUserAppSettings', Base.metadata,
    Column('intUserID', Integer, nullable=False),
    Column('strAppLanguage', String(50, 'SQL_Latin1_General_CP1_CI_AS'), nullable=False),
    Column('strThemePreference', String(25, 'SQL_Latin1_General_CP1_CI_AS'), nullable=False),
    ForeignKeyConstraint(['intUserID'], ['TUsers.intUserID'], name='TUserAppSettings_TUsers_FK')
)


class TUserInterests(Base):
    __tablename__ = 'TUserInterests'
    __table_args__ = (
        ForeignKeyConstraint(['intInterestID'], ['TInterests.intInterestID'], name='TUserInterests_TInterests_FK'),
        ForeignKeyConstraint(['intUserID'], ['TUsers.intUserID'], name='TUserInterests_TUsers_FK'),
        PrimaryKeyConstraint('intUserInterestID', name='TUserInterests_PK')
    )

    intUserInterestID: Mapped[int] = mapped_column(Integer, Identity(start=1, increment=1), primary_key=True)
    intUserID: Mapped[int] = mapped_column(Integer)
    intInterestID: Mapped[int] = mapped_column(Integer)

    TInterests_: Mapped['TInterests'] = relationship('TInterests', back_populates='TUserInterests')
    TUsers_: Mapped['TUsers'] = relationship('TUsers', back_populates='TUserInterests')


class TUserLogs(Base):
    __tablename__ = 'TUserLogs'
    __table_args__ = (
        ForeignKeyConstraint(['intUserID'], ['TUsers.intUserID'], name='TUserLogs_TUsers_FK'),
        PrimaryKeyConstraint('intUserLogID', name='TUserLogs_PK')
    )

    intUserLogID: Mapped[int] = mapped_column(Integer, Identity(start=1, increment=1), primary_key=True)
    intUserID: Mapped[int] = mapped_column(Integer)
    intUserLogTypeID: Mapped[int] = mapped_column(Integer)
    dtmLoginDate: Mapped[datetime.datetime] = mapped_column(DateTime)
    strIPAddress: Mapped[str] = mapped_column(String(250, 'SQL_Latin1_General_CP1_CI_AS'))
    strUserAgent: Mapped[str] = mapped_column(String(250, 'SQL_Latin1_General_CP1_CI_AS'))
    intAttributeName: Mapped[int] = mapped_column(Integer)
    strComment: Mapped[Optional[str]] = mapped_column(String(1000, 'SQL_Latin1_General_CP1_CI_AS'))

    TUsers_: Mapped['TUsers'] = relationship('TUsers', back_populates='TUserLogs')


class TUserMedia(Base):
    __tablename__ = 'TUserMedia'
    __table_args__ = (
        ForeignKeyConstraint(['intUserID'], ['TUsers.intUserID'], name='TUserMedia_TUsers_FK'),
        ForeignKeyConstraint(['intUserMediaID'], ['TUserMedia.intUserMediaID'], name='TUserMedia_TUserMedia_FK'),
        PrimaryKeyConstraint('intUserMediaID', name='TUserMedia_PK')
    )

    intUserMediaID: Mapped[int] = mapped_column(Integer, Identity(start=1, increment=1), primary_key=True)
    intUserID: Mapped[int] = mapped_column(Integer)
    strMediaURL: Mapped[str] = mapped_column(String(250, 'SQL_Latin1_General_CP1_CI_AS'))
    blnOnProfile: Mapped[bool] = mapped_column(Boolean)
    blnIsFace: Mapped[bool] = mapped_column(Boolean)
    blnIsActive: Mapped[bool] = mapped_column(Boolean)
    dtmUploadDate: Mapped[datetime.datetime] = mapped_column(DateTime)

    TUsers_: Mapped['TUsers'] = relationship('TUsers', back_populates='TUserMedia')


t_TUserPreferences = Table(
    'TUserPreferences', Base.metadata,
    Column('intUserID', Integer, nullable=False),
    Column('intMatchDistance', Integer),
    Column('intMinAge', Integer),
    Column('intMaxAge', Integer),
    Column('blnReceiveEmails', Boolean, nullable=False),
    Column('blnShowProfile', Boolean, nullable=False),
    Column('strBio', String(350, 'SQL_Latin1_General_CP1_CI_AS'), nullable=False),
    Column('intAppUsageTypeID', Integer, nullable=False),
    Column('intGenderPreferenceID', Integer, nullable=False),
    ForeignKeyConstraint(['intAppUsageTypeID'], ['TAppUsageTypes.intAppUsageTypeID'], name='TUserPreferences_TGenders_FK'),
    ForeignKeyConstraint(['intGenderPreferenceID'], ['TGenders.intGenderPreferenceID'], name='TUserPreferences_TAppUsageTypes_FK'),
    ForeignKeyConstraint(['intUserID'], ['TUsers.intUserID'], name='TUserPreferences_TUsers_FK')
)


class TMatches(Base):
    __tablename__ = 'TMatches'
    __table_args__ = (
        ForeignKeyConstraint(['intMatchRequestID'], ['TMatchRequests.intMatchRequestID'], name='TMatches_TMatchRequests_FK'),
        PrimaryKeyConstraint('intMatchID', name='TMatches_PK')
    )

    intMatchID: Mapped[int] = mapped_column(Integer, Identity(start=1, increment=1), primary_key=True)
    intMatchRequestID: Mapped[int] = mapped_column(Integer)
    blnFirstUserDeleted: Mapped[bool] = mapped_column(Boolean)
    blnSecondUserDeleted: Mapped[bool] = mapped_column(Boolean)
    dtmMatchStarted: Mapped[datetime.datetime] = mapped_column(DateTime)
    dtmMatchEnded: Mapped[Optional[datetime.datetime]] = mapped_column(DateTime)

    TMatchRequests_: Mapped['TMatchRequests'] = relationship('TMatchRequests', back_populates='TMatches')
    TChatEvents: Mapped[List['TChatEvents']] = relationship('TChatEvents', back_populates='TMatches_')
    TChatMessages: Mapped[List['TChatMessages']] = relationship('TChatMessages', back_populates='TMatches_')


class TChatEvents(Base):
    __tablename__ = 'TChatEvents'
    __table_args__ = (
        ForeignKeyConstraint(['intChatEventTypeID'], ['TChatEventTypes.intChatEventTypeID'], name='TChatEvents_TChatEventTypes_FK'),
        ForeignKeyConstraint(['intMatchID'], ['TMatches.intMatchID'], name='TChatEvents_TMatches_FK'),
        ForeignKeyConstraint(['intUserID'], ['TUsers.intUserID'], name='TChatEvents_TUsers_FK'),
        PrimaryKeyConstraint('intChatEventID', name='TChatEvents_PK')
    )

    intChatEventID: Mapped[int] = mapped_column(Integer, Identity(start=1, increment=1), primary_key=True)
    intMatchID: Mapped[int] = mapped_column(Integer)
    intUserID: Mapped[int] = mapped_column(Integer)
    intChatEventTypeID: Mapped[int] = mapped_column(Integer)
    strChatEvent: Mapped[str] = mapped_column(String(250, 'SQL_Latin1_General_CP1_CI_AS'))

    TChatEventTypes_: Mapped['TChatEventTypes'] = relationship('TChatEventTypes', back_populates='TChatEvents')
    TMatches_: Mapped['TMatches'] = relationship('TMatches', back_populates='TChatEvents')
    TUsers_: Mapped['TUsers'] = relationship('TUsers', back_populates='TChatEvents')


class TChatMessages(Base):
    __tablename__ = 'TChatMessages'
    __table_args__ = (
        ForeignKeyConstraint(['intMatchID'], ['TMatches.intMatchID'], name='TChatMessages_TMatches_FK'),
        ForeignKeyConstraint(['intSenderUserID'], ['TUsers.intUserID'], name='TChatMessages_TUsers_FK'),
        PrimaryKeyConstraint('intChatMessageID', name='TChatMessages_PK')
    )

    intChatMessageID: Mapped[int] = mapped_column(Integer, Identity(start=1, increment=1), primary_key=True)
    intSenderUserID: Mapped[int] = mapped_column(Integer)
    intMatchID: Mapped[int] = mapped_column(Integer)
    strMessageText: Mapped[str] = mapped_column(String(1000, 'SQL_Latin1_General_CP1_CI_AS'))
    dtmSentAt: Mapped[datetime.datetime] = mapped_column(DateTime)
    blnIsActive: Mapped[bool] = mapped_column(Boolean)
    intUserMediaID: Mapped[Optional[int]] = mapped_column(Integer)

    TMatches_: Mapped['TMatches'] = relationship('TMatches', back_populates='TChatMessages')
    TUsers_: Mapped['TUsers'] = relationship('TUsers', back_populates='TChatMessages')
