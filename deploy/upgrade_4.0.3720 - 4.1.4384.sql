/*
   Montag, 19. Dezember 201109:17:01
   User: 
   Server: (local)
   Database: EventSite
   Application: 
*/

/* To prevent any potential data loss issues, you should review this script in detail before running it outside the context of the database designer.*/
BEGIN TRANSACTION
SET QUOTED_IDENTIFIER ON
SET ARITHABORT ON
SET NUMERIC_ROUNDABORT OFF
SET CONCAT_NULL_YIELDS_NULL ON
SET ANSI_NULLS ON
SET ANSI_PADDING ON
SET ANSI_WARNINGS ON
COMMIT
BEGIN TRANSACTION
GO
ALTER TABLE dbo.ES_Mandators
	DROP CONSTRAINT DF_ES_Mandators_NotifyDeletableSubscriptionStates
GO
ALTER TABLE dbo.ES_Mandators
	DROP CONSTRAINT DF_ES_Mandators_NoSmsCreditNotified
GO
CREATE TABLE dbo.Tmp_ES_Mandators
	(
	MandatorId char(32) NOT NULL,
	MandatorName varchar(50) NOT NULL,
	MandatorShortName varchar(16) NOT NULL,
	MandatorMail varchar(50) NOT NULL,
	EntryPointUrl varchar(100) NULL,
	SiteTitle varchar(50) NOT NULL,
	EventName varchar(50) NOT NULL,
	FeatureAssembly varchar(100) NULL,
	FeatureAssemblyClassName varchar(50) NULL,
	EventNotificationAddressesDefault varchar(800) NULL,
	ShowEventsAsList bit NOT NULL,
	UseEventCategories bit NOT NULL,
	UseEventUrl bit NOT NULL,
	UseMinMaxSubscriptions bit NOT NULL,
	UseSubscriptions bit NOT NULL,
	SmsNotifications bit NOT NULL,
	OnNewEventNotifyContacts bit NOT NULL,
	OnEditEventNotifyContacts bit NOT NULL,
	OnNewSubscriptionNotifyContacts bit NOT NULL,
	OnEditSubscriptionNotifyContacts bit NOT NULL,
	OnDeleteSubscriptionNotifyContacts bit NOT NULL,
	IsLiftManagementEnabled bit NOT NULL,
	NotifyDeletableSubscriptionStates bit NOT NULL,
	NoSmsCreditNotified bit NOT NULL,
	UseExternalAuth bit NOT NULL,
	AuthTable varchar(50) NULL,
	AuthIdColumn varchar(50) NULL,
	AuthLoginColumn varchar(50) NULL,
	AuthPasswordColumn varchar(50) NULL,
	Helptext varchar(1000) NULL,
	UnsubscribeAllowedFromNumSubscriptions int NULL,
	UnsubscribeAllowedTillNumSubscriptions int NULL,
	SmsPurchased int NOT NULL,
	SmsLog int NOT NULL
	)  ON [PRIMARY]
GO
ALTER TABLE dbo.Tmp_ES_Mandators ADD CONSTRAINT
	DF_ES_Mandators_NotifyDeletableSubscriptionStates DEFAULT ((0)) FOR NotifyDeletableSubscriptionStates
GO
ALTER TABLE dbo.Tmp_ES_Mandators ADD CONSTRAINT
	DF_ES_Mandators_NoSmsCreditNotified DEFAULT ((0)) FOR NoSmsCreditNotified
GO
ALTER TABLE dbo.Tmp_ES_Mandators ADD CONSTRAINT
	DF_ES_Mandators_UseExternalAuth DEFAULT 0 FOR UseExternalAuth
GO
IF EXISTS(SELECT * FROM dbo.ES_Mandators)
	 EXEC('INSERT INTO dbo.Tmp_ES_Mandators (MandatorId, MandatorName, MandatorShortName, MandatorMail, EntryPointUrl, SiteTitle, EventName, FeatureAssembly, FeatureAssemblyClassName, EventNotificationAddressesDefault, ShowEventsAsList, UseEventCategories, UseEventUrl, UseMinMaxSubscriptions, UseSubscriptions, SmsNotifications, OnNewEventNotifyContacts, OnEditEventNotifyContacts, OnNewSubscriptionNotifyContacts, OnEditSubscriptionNotifyContacts, OnDeleteSubscriptionNotifyContacts, IsLiftManagementEnabled, NotifyDeletableSubscriptionStates, NoSmsCreditNotified, Helptext, UnsubscribeAllowedFromNumSubscriptions, UnsubscribeAllowedTillNumSubscriptions, SmsPurchased, SmsLog)
		SELECT MandatorId, MandatorName, MandatorShortName, MandatorMail, EntryPointUrl, SiteTitle, EventName, FeatureAssembly, FeatureAssemblyClassName, EventNotificationAddressesDefault, ShowEventsAsList, UseEventCategories, UseEventUrl, UseMinMaxSubscriptions, UseSubscriptions, SmsNotifications, OnNewEventNotifyContacts, OnEditEventNotifyContacts, OnNewSubscriptionNotifyContacts, OnEditSubscriptionNotifyContacts, OnDeleteSubscriptionNotifyContacts, IsLiftManagementEnabled, NotifyDeletableSubscriptionStates, NoSmsCreditNotified, Helptext, UnsubscribeAllowedFromNumSubscriptions, UnsubscribeAllowedTillNumSubscriptions, SmsPurchased, SmsLog FROM dbo.ES_Mandators WITH (HOLDLOCK TABLOCKX)')
GO
ALTER TABLE dbo.ES_Contacts
	DROP CONSTRAINT FK_ES_Contacts_ES_Mandators
GO
ALTER TABLE dbo.ES_EventCategories
	DROP CONSTRAINT FK_ES_EventCategories_ES_Mandators
GO
ALTER TABLE dbo.ES_Events
	DROP CONSTRAINT FK_ES_Events_ES_Mandators
GO
DROP TABLE dbo.ES_Mandators
GO
EXECUTE sp_rename N'dbo.Tmp_ES_Mandators', N'ES_Mandators', 'OBJECT' 
GO
ALTER TABLE dbo.ES_Mandators ADD CONSTRAINT
	PK_ES_Mandators PRIMARY KEY CLUSTERED 
	(
	MandatorId
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO
COMMIT
BEGIN TRANSACTION
GO
ALTER TABLE dbo.ES_EventCategories ADD CONSTRAINT
	FK_ES_EventCategories_ES_Mandators FOREIGN KEY
	(
	MandatorId
	) REFERENCES dbo.ES_Mandators
	(
	MandatorId
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	
GO
COMMIT
BEGIN TRANSACTION
GO
ALTER TABLE dbo.ES_Contacts
	DROP CONSTRAINT DF_ES_Contacts_IsDeleted
GO
ALTER TABLE dbo.ES_Contacts
	DROP CONSTRAINT DF_ES_Contacts_NoSmsCreditNotified
GO
CREATE TABLE dbo.Tmp_ES_Contacts
	(
	ContactId int NOT NULL IDENTITY (1, 1),
	CustomId int NULL,
	MandatorId char(32) NOT NULL,
	Name varchar(50) NOT NULL,
	Email varchar(50) NOT NULL,
	Login varchar(50) NULL,
	Password varchar(50) NULL,
	MobilePhone varchar(20) NULL,
	LiftMgmtSmsOn bit NOT NULL,
	EventMgmtSmsOn bit NOT NULL,
	UseTwoWaySms bit NOT NULL,
	SmsLog int NOT NULL,
	SmsPurchased int NOT NULL,
	IsDeleted bit NOT NULL,
	NoSmsCreditNotified bit NOT NULL
	)  ON [PRIMARY]
GO
GRANT SELECT ON dbo.Tmp_ES_Contacts TO EventSiteUsers  AS dbo
GO
ALTER TABLE dbo.Tmp_ES_Contacts ADD CONSTRAINT
	DF_ES_Contacts_IsDeleted DEFAULT ((0)) FOR IsDeleted
GO
ALTER TABLE dbo.Tmp_ES_Contacts ADD CONSTRAINT
	DF_ES_Contacts_NoSmsCreditNotified DEFAULT ((0)) FOR NoSmsCreditNotified
GO
SET IDENTITY_INSERT dbo.Tmp_ES_Contacts ON
GO
IF EXISTS(SELECT * FROM dbo.ES_Contacts)
	 EXEC('INSERT INTO dbo.Tmp_ES_Contacts (ContactId, CustomId, MandatorId, Name, Email, MobilePhone, LiftMgmtSmsOn, EventMgmtSmsOn, UseTwoWaySms, SmsLog, SmsPurchased, IsDeleted, NoSmsCreditNotified)
		SELECT ContactId, CustomId, MandatorId, Name, Email, MobilePhone, LiftMgmtSmsOn, EventMgmtSmsOn, UseTwoWaySms, SmsLog, SmsPurchased, IsDeleted, NoSmsCreditNotified FROM dbo.ES_Contacts WITH (HOLDLOCK TABLOCKX)')
GO
SET IDENTITY_INSERT dbo.Tmp_ES_Contacts OFF
GO
ALTER TABLE dbo.ES_ContactSettings
	DROP CONSTRAINT FK_ES_ContactSettings_ES_Contacts
GO
ALTER TABLE dbo.ES_ContactsRoles
	DROP CONSTRAINT FK_ES_ContactsRoles_ES_Contacts
GO
ALTER TABLE dbo.ES_Events
	DROP CONSTRAINT FK_ES_Events_ES_Contacts
GO
ALTER TABLE dbo.ES_NotifSubscriptions
	DROP CONSTRAINT FK_ES_NotifSubscriptions_ES_Contacts
GO
ALTER TABLE dbo.ES_SmsLocations
	DROP CONSTRAINT FK_ES_SmsLocations_ES_Contacts
GO
ALTER TABLE dbo.ES_Subscriptions
	DROP CONSTRAINT FK_ES_Subscriptions_ES_Contacts
GO
DROP TABLE dbo.ES_Contacts
GO
EXECUTE sp_rename N'dbo.Tmp_ES_Contacts', N'ES_Contacts', 'OBJECT' 
GO
ALTER TABLE dbo.ES_Contacts ADD CONSTRAINT
	PK_ES_Contacts PRIMARY KEY CLUSTERED 
	(
	ContactId
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO
CREATE UNIQUE NONCLUSTERED INDEX IX_Unique_ES_Contacts_Email_MandatorId ON dbo.ES_Contacts
	(
	Email,
	MandatorId
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
ALTER TABLE dbo.ES_Contacts ADD CONSTRAINT
	FK_ES_Contacts_ES_Mandators FOREIGN KEY
	(
	MandatorId
	) REFERENCES dbo.ES_Mandators
	(
	MandatorId
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	
GO
COMMIT
BEGIN TRANSACTION
GO
ALTER TABLE dbo.ES_Subscriptions ADD CONSTRAINT
	FK_ES_Subscriptions_ES_Contacts FOREIGN KEY
	(
	ContactId
	) REFERENCES dbo.ES_Contacts
	(
	ContactId
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	
GO
COMMIT
BEGIN TRANSACTION
GO
ALTER TABLE dbo.ES_SmsLocations ADD CONSTRAINT
	FK_ES_SmsLocations_ES_Contacts FOREIGN KEY
	(
	ContactId
	) REFERENCES dbo.ES_Contacts
	(
	ContactId
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	
GO
COMMIT
BEGIN TRANSACTION
GO
ALTER TABLE dbo.ES_NotifSubscriptions ADD CONSTRAINT
	FK_ES_NotifSubscriptions_ES_Contacts FOREIGN KEY
	(
	ContactId
	) REFERENCES dbo.ES_Contacts
	(
	ContactId
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	
GO
COMMIT
BEGIN TRANSACTION
GO
ALTER TABLE dbo.ES_Events ADD CONSTRAINT
	FK_ES_Events_ES_Mandators FOREIGN KEY
	(
	MandatorId
	) REFERENCES dbo.ES_Mandators
	(
	MandatorId
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	
GO
ALTER TABLE dbo.ES_Events ADD CONSTRAINT
	FK_ES_Events_ES_Contacts FOREIGN KEY
	(
	EventCreatorId
	) REFERENCES dbo.ES_Contacts
	(
	ContactId
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	
GO
COMMIT
BEGIN TRANSACTION
GO
ALTER TABLE dbo.ES_ContactsRoles ADD CONSTRAINT
	FK_ES_ContactsRoles_ES_Contacts FOREIGN KEY
	(
	ContactId
	) REFERENCES dbo.ES_Contacts
	(
	ContactId
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	
GO
COMMIT
BEGIN TRANSACTION
GO
ALTER TABLE dbo.ES_ContactSettings ADD CONSTRAINT
	FK_ES_ContactSettings_ES_Contacts FOREIGN KEY
	(
	ContactId
	) REFERENCES dbo.ES_Contacts
	(
	ContactId
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	
GO
COMMIT

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ES_GetContactLogin]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'


-- ändert einen kontakt in der tab ES_Contacts
-- Returns:
-- 0 = ok
-- 100 = contact nicht gefunden
CREATE PROCEDURE [dbo].[ES_GetContactLogin] 
 @ContactId INT
AS

SELECT Login FROM ES_Contacts WHERE ContactId = @ContactId AND IsDeleted = 0
IF @@ROWCOUNT=0 BEGIN
  RETURN 100
END

RETURN 0
' 
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ES_ChangePassword]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'


-- ändert einen kontakt in der tab ES_Contacts
-- Returns:
-- 0 = ok
-- 100 = contact nicht gefunden
-- 101 = fehler beim update
CREATE PROCEDURE [dbo].[ES_ChangePassword] 
 @ContactId INT,
 @NewLogin VARCHAR(50),
 @NewPassword VARCHAR(50)
AS

DECLARE @MandatorId CHAR(32)
SELECT @MandatorId = MandatorId FROM ES_Contacts WHERE ContactId = @ContactId AND IsDeleted = 0
IF @@ROWCOUNT=0 BEGIN
  RETURN 100
END

  UPDATE ES_Contacts
  SET Login=@NewLogin, Password=@NewPassword
    WHERE ContactId = @ContactId
      AND IsDeleted = 0
      AND @NewLogin IS NOT NULL AND @NewLogin <> ''''
      AND @NewPassword IS NOT NULL AND @NewPassword <> ''''
  IF @@ROWCOUNT=1 BEGIN
    RETURN 0
  END ELSE
    RETURN 101
' 
END
GO
GRANT EXECUTE ON [dbo].[ES_ChangePassword] TO [EventSiteUsers]
GO
GRANT EXECUTE ON [dbo].[ES_GetContactLogin] TO [EventSiteUsers]
GO
