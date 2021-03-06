/* ------------------------------------------------------------

   DESCRIPTION:  Schema Synchronization Script for Object(s) 

    tables:
        [dbo].[ES_Contacts], [dbo].[ES_ContactSettings], [dbo].[ES_ContactsRoles], [dbo].[ES_LiftSubscriptions], [dbo].[ES_Mandators], [dbo].[ES_SimpleCache], [dbo].[ES_Subscriptions]

     Make DEATHSTAR\SQL2K5.kcm_live Equal DEATHSTAR\SQL2K5.EventSite

   AUTHOR:	[Insert Author Name]

   DATE:	22.12.2008 13:18:36

   LEGAL:	2008 [Insert Company Name]

   ------------------------------------------------------------ */

SET NOEXEC OFF
SET ANSI_WARNINGS ON
SET XACT_ABORT ON
SET IMPLICIT_TRANSACTIONS OFF
SET ARITHABORT ON
SET NOCOUNT ON
SET QUOTED_IDENTIFIER ON
SET NUMERIC_ROUNDABORT OFF
SET CONCAT_NULL_YIELDS_NULL ON
SET ANSI_NULLS ON
SET ANSI_PADDING ON
GO

BEGIN TRAN
GO

-- Drop Reference FK_ES_Contacts_ES_Mandators from ES_Mandators
Print 'Drop Reference FK_ES_Contacts_ES_Mandators from ES_Mandators'
GO
	ALTER TABLE [dbo].[ES_Contacts] DROP CONSTRAINT [FK_ES_Contacts_ES_Mandators]
GO

IF @@ERROR<>0 OR @@TRANCOUNT=0 BEGIN IF @@TRANCOUNT>0 ROLLBACK SET NOEXEC ON END
GO
-- Drop Reference FK_ES_EventCategories_ES_Mandators from ES_Mandators
Print 'Drop Reference FK_ES_EventCategories_ES_Mandators from ES_Mandators'
GO
	ALTER TABLE [dbo].[ES_EventCategories] DROP CONSTRAINT [FK_ES_EventCategories_ES_Mandators]
GO

IF @@ERROR<>0 OR @@TRANCOUNT=0 BEGIN IF @@TRANCOUNT>0 ROLLBACK SET NOEXEC ON END
GO
-- Drop Reference FK_ES_Events_ES_Mandators from ES_Mandators
Print 'Drop Reference FK_ES_Events_ES_Mandators from ES_Mandators'
GO
	ALTER TABLE [dbo].[ES_Events] DROP CONSTRAINT [FK_ES_Events_ES_Mandators]
GO

IF @@ERROR<>0 OR @@TRANCOUNT=0 BEGIN IF @@TRANCOUNT>0 ROLLBACK SET NOEXEC ON END
GO
-- Drop Default Constraint DF_ES_Mandators_NotifyDeletableSubscriptionStates from ES_Mandators
Print 'Drop Default Constraint DF_ES_Mandators_NotifyDeletableSubscriptionStates from ES_Mandators'
GO
ALTER TABLE [dbo].[ES_Mandators] DROP CONSTRAINT [DF_ES_Mandators_NotifyDeletableSubscriptionStates]
GO

IF @@ERROR<>0 OR @@TRANCOUNT=0 BEGIN IF @@TRANCOUNT>0 ROLLBACK SET NOEXEC ON END
GO
-- Drop Primary Key PK_ES_Mandators from ES_Mandators
Print 'Drop Primary Key PK_ES_Mandators from ES_Mandators'
GO
ALTER TABLE [dbo].[ES_Mandators] DROP CONSTRAINT [PK_ES_Mandators]
GO

IF @@ERROR<>0 OR @@TRANCOUNT=0 BEGIN IF @@TRANCOUNT>0 ROLLBACK SET NOEXEC ON END
GO
-- Preserving data from [dbo].[ES_Mandators] into a temporary table temp807673925
EXEC sp_rename @objname = N'[dbo].[ES_Mandators]', @newname = N'temp807673925', @objtype = 'OBJECT'
GO

-- Create Table ES_Mandators
Print 'Create Table ES_Mandators'
GO
CREATE TABLE [dbo].[ES_Mandators] (
		[MandatorId]                                 char(32) NOT NULL,
		[MandatorName]                               varchar(50) NOT NULL,
		[MandatorShortName]                          varchar(16) NOT NULL,
		[MandatorMail]                               varchar(50) NOT NULL,
		[EntryPointUrl]                              varchar(100) NULL,
		[SiteTitle]                                  varchar(50) NOT NULL,
		[EventName]                                  varchar(50) NOT NULL,
		[FeatureAssembly]                            varchar(100) NULL,
		[FeatureAssemblyClassName]                   varchar(50) NULL,
		[EventNotificationAddressesDefault]          varchar(800) NULL,
		[ShowEventsAsList]                           bit NOT NULL,
		[UseEventCategories]                         bit NOT NULL,
		[UseEventUrl]                                bit NOT NULL,
		[UseMinMaxSubscriptions]                     bit NOT NULL,
		[UseSubscriptions]                           bit NOT NULL,
		[SmsNotifications]                           bit NOT NULL,
		[OnNewEventNotifyContacts]                   bit NOT NULL,
		[OnEditEventNotifyContacts]                  bit NOT NULL,
		[OnNewSubscriptionNotifyContacts]            bit NOT NULL,
		[OnEditSubscriptionNotifyContacts]           bit NOT NULL,
		[OnDeleteSubscriptionNotifyContacts]         bit NOT NULL,
		[IsLiftManagementEnabled]                    bit NOT NULL,
		[NotifyDeletableSubscriptionStates]          bit NOT NULL,
		[NoSmsCreditNotified]                        bit NOT NULL,
		[Helptext]                                   varchar(1000) NULL,
		[UnsubscribeAllowedFromNumSubscriptions]     int NULL,
		[UnsubscribeAllowedTillNumSubscriptions]     int NULL,
		[SmsPurchased]                               int NOT NULL,
		[SmsLog]                                     int NOT NULL
)
GO

IF @@ERROR<>0 OR @@TRANCOUNT=0 BEGIN IF @@TRANCOUNT>0 ROLLBACK SET NOEXEC ON END
GO
-- Add Primary Key PK_ES_Mandators to ES_Mandators
Print 'Add Primary Key PK_ES_Mandators to ES_Mandators'
GO
ALTER TABLE [dbo].[ES_Mandators]
	ADD
	CONSTRAINT [PK_ES_Mandators]
	PRIMARY KEY
	([MandatorId])
GO

IF @@ERROR<>0 OR @@TRANCOUNT=0 BEGIN IF @@TRANCOUNT>0 ROLLBACK SET NOEXEC ON END
GO
-- Add Default Constraint DF_ES_Mandators_NoSmsCreditNotified to ES_Mandators
Print 'Add Default Constraint DF_ES_Mandators_NoSmsCreditNotified to ES_Mandators'
GO
ALTER TABLE [dbo].[ES_Mandators]
	ADD
	CONSTRAINT [DF_ES_Mandators_NoSmsCreditNotified]
	DEFAULT ((0)) FOR [NoSmsCreditNotified]
GO

IF @@ERROR<>0 OR @@TRANCOUNT=0 BEGIN IF @@TRANCOUNT>0 ROLLBACK SET NOEXEC ON END
GO
-- Add Default Constraint DF_ES_Mandators_NotifyDeletableSubscriptionStates to ES_Mandators
Print 'Add Default Constraint DF_ES_Mandators_NotifyDeletableSubscriptionStates to ES_Mandators'
GO
ALTER TABLE [dbo].[ES_Mandators]
	ADD
	CONSTRAINT [DF_ES_Mandators_NotifyDeletableSubscriptionStates]
	DEFAULT ((0)) FOR [NotifyDeletableSubscriptionStates]
GO

IF @@ERROR<>0 OR @@TRANCOUNT=0 BEGIN IF @@TRANCOUNT>0 ROLLBACK SET NOEXEC ON END
GO
-- Disabling constraints
ALTER TABLE [dbo].[ES_Mandators] NOCHECK CONSTRAINT ALL
GO

-- Restoring data
IF OBJECT_ID('[dbo].temp807673925') IS NOT NULL AND EXISTS(SELECT * FROM sys.objects WHERE [object_id] = OBJECT_ID(N'[dbo].[ES_Mandators]') AND [type]='U')
EXEC sp_executesql N'
INSERT INTO [dbo].[ES_Mandators] ([MandatorId], [MandatorName], [MandatorShortName], [MandatorMail], [EntryPointUrl], [SiteTitle], [EventName], [FeatureAssembly], [FeatureAssemblyClassName], [EventNotificationAddressesDefault], [ShowEventsAsList], [UseEventCategories], [UseEventUrl], [UseMinMaxSubscriptions], [UseSubscriptions], [SmsNotifications], [OnNewEventNotifyContacts], [OnEditEventNotifyContacts], [OnNewSubscriptionNotifyContacts], [OnEditSubscriptionNotifyContacts], [OnDeleteSubscriptionNotifyContacts], [IsLiftManagementEnabled], [NotifyDeletableSubscriptionStates], [Helptext], [SmsPurchased], [SmsLog])
   SELECT [MandatorId], [MandatorName], [MandatorShortName], [MandatorMail], [EntryPointUrl], [SiteTitle], [EventName], [FeatureAssembly], [FeatureAssemblyClassName], [EventNotificationAddressesDefault], [ShowEventsAsList], [UseEventCategories], [UseEventUrl], [UseMinMaxSubscriptions], [UseSubscriptions], [SmsNotifications], [OnNewEventNotifyContacts], [OnEditEventNotifyContacts], [OnNewSubscriptionNotifyContacts], [OnEditSubscriptionNotifyContacts], [OnDeleteSubscriptionNotifyContacts], [IsLiftManagementEnabled], [NotifyDeletableSubscriptionStates], [Helptext], [SmsPurchased], [SmsLog] FROM [dbo].temp807673925
'
GO

IF @@ERROR<>0 OR @@TRANCOUNT=0 BEGIN IF @@TRANCOUNT>0 ROLLBACK SET NOEXEC ON END
GO
-- Enabling backward constraints
ALTER TABLE [dbo].[ES_Mandators] CHECK CONSTRAINT ALL
GO

-- Dropping the temporary table temp807673925
IF OBJECT_ID('[dbo].temp807673925') IS NOT NULL DROP TABLE [dbo].temp807673925
GO

-- Create Index IX_Unique_ES_Subscriptions_ContactId_EventId on ES_Subscriptions
Print 'Create Index IX_Unique_ES_Subscriptions_ContactId_EventId on ES_Subscriptions'
GO
CREATE UNIQUE INDEX [IX_Unique_ES_Subscriptions_ContactId_EventId]
	ON [dbo].[ES_Subscriptions] ([ContactId], [EventId])
GO

IF @@ERROR<>0 OR @@TRANCOUNT=0 BEGIN IF @@TRANCOUNT>0 ROLLBACK SET NOEXEC ON END
GO
-- Create Index IX_Unique_ES_ContactSettings_ContactId_EventCategoryId on ES_ContactSettings
Print 'Create Index IX_Unique_ES_ContactSettings_ContactId_EventCategoryId on ES_ContactSettings'
GO
CREATE UNIQUE INDEX [IX_Unique_ES_ContactSettings_ContactId_EventCategoryId]
	ON [dbo].[ES_ContactSettings] ([ContactId], [EventCategoryId])
GO

IF @@ERROR<>0 OR @@TRANCOUNT=0 BEGIN IF @@TRANCOUNT>0 ROLLBACK SET NOEXEC ON END
GO
-- Create Index IX_Unique_ES_Contacts_ContactId_RoleId on ES_ContactsRoles
Print 'Create Index IX_Unique_ES_Contacts_ContactId_RoleId on ES_ContactsRoles'
GO
CREATE UNIQUE INDEX [IX_Unique_ES_Contacts_ContactId_RoleId]
	ON [dbo].[ES_ContactsRoles] ([ContactId], [RoleId])
GO

IF @@ERROR<>0 OR @@TRANCOUNT=0 BEGIN IF @@TRANCOUNT>0 ROLLBACK SET NOEXEC ON END
GO
-- Create Index IX_Unique_ES_LiftSubscriptions_SubscriptionId on ES_LiftSubscriptions
Print 'Create Index IX_Unique_ES_LiftSubscriptions_SubscriptionId on ES_LiftSubscriptions'
GO
CREATE UNIQUE INDEX [IX_Unique_ES_LiftSubscriptions_SubscriptionId]
	ON [dbo].[ES_LiftSubscriptions] ([SubscriptionId])
GO

IF @@ERROR<>0 OR @@TRANCOUNT=0 BEGIN IF @@TRANCOUNT>0 ROLLBACK SET NOEXEC ON END
GO
-- Add Column NoSmsCreditNotified to ES_Contacts
Print 'Add Column NoSmsCreditNotified to ES_Contacts'
GO
ALTER TABLE [dbo].[ES_Contacts]
	ADD [NoSmsCreditNotified] bit NOT NULL

GO

IF @@ERROR<>0 OR @@TRANCOUNT=0 BEGIN IF @@TRANCOUNT>0 ROLLBACK SET NOEXEC ON END
GO
-- Add Default Constraint DF_ES_Contacts_NoSmsCreditNotified to ES_Contacts
Print 'Add Default Constraint DF_ES_Contacts_NoSmsCreditNotified to ES_Contacts'
GO
ALTER TABLE [dbo].[ES_Contacts]
	ADD
	CONSTRAINT [DF_ES_Contacts_NoSmsCreditNotified]
	DEFAULT ((0)) FOR [NoSmsCreditNotified]
GO

IF @@ERROR<>0 OR @@TRANCOUNT=0 BEGIN IF @@TRANCOUNT>0 ROLLBACK SET NOEXEC ON END
GO
-- Create Index IX_Unique_ES_Contacts_Email_MandatorId on ES_Contacts
Print 'Create Index IX_Unique_ES_Contacts_Email_MandatorId on ES_Contacts'
GO
CREATE UNIQUE INDEX [IX_Unique_ES_Contacts_Email_MandatorId]
	ON [dbo].[ES_Contacts] ([Email], [MandatorId])
GO

IF @@ERROR<>0 OR @@TRANCOUNT=0 BEGIN IF @@TRANCOUNT>0 ROLLBACK SET NOEXEC ON END
GO
-- Create Table ES_SimpleCache
Print 'Create Table ES_SimpleCache'
GO
CREATE TABLE [dbo].[ES_SimpleCache] (
		[Key]            varchar(255) NOT NULL,
		[Value]          varchar(8000) NULL,
		[ExpiryDate]     datetime NOT NULL
)
GO

IF @@ERROR<>0 OR @@TRANCOUNT=0 BEGIN IF @@TRANCOUNT>0 ROLLBACK SET NOEXEC ON END
GO
-- Add Primary Key PK_ES_SimpleCache to ES_SimpleCache
Print 'Add Primary Key PK_ES_SimpleCache to ES_SimpleCache'
GO
ALTER TABLE [dbo].[ES_SimpleCache]
	ADD
	CONSTRAINT [PK_ES_SimpleCache]
	PRIMARY KEY
	([Key])
GO

IF @@ERROR<>0 OR @@TRANCOUNT=0 BEGIN IF @@TRANCOUNT>0 ROLLBACK SET NOEXEC ON END
GO
-- Create Reference FK_ES_Contacts_ES_Mandators on ES_Mandators
Print 'Create Reference FK_ES_Contacts_ES_Mandators on ES_Mandators'
GO
ALTER TABLE [dbo].[ES_Contacts]
	ADD CONSTRAINT [FK_ES_Contacts_ES_Mandators]
	FOREIGN KEY ([MandatorId]) REFERENCES [dbo].[ES_Mandators] ([MandatorId])
GO

IF @@ERROR<>0 OR @@TRANCOUNT=0 BEGIN IF @@TRANCOUNT>0 ROLLBACK SET NOEXEC ON END
GO
-- Create Reference FK_ES_EventCategories_ES_Mandators on ES_Mandators
Print 'Create Reference FK_ES_EventCategories_ES_Mandators on ES_Mandators'
GO
ALTER TABLE [dbo].[ES_EventCategories]
	ADD CONSTRAINT [FK_ES_EventCategories_ES_Mandators]
	FOREIGN KEY ([MandatorId]) REFERENCES [dbo].[ES_Mandators] ([MandatorId])
GO

IF @@ERROR<>0 OR @@TRANCOUNT=0 BEGIN IF @@TRANCOUNT>0 ROLLBACK SET NOEXEC ON END
GO
-- Create Reference FK_ES_Events_ES_Mandators on ES_Mandators
Print 'Create Reference FK_ES_Events_ES_Mandators on ES_Mandators'
GO
ALTER TABLE [dbo].[ES_Events]
	ADD CONSTRAINT [FK_ES_Events_ES_Mandators]
	FOREIGN KEY ([MandatorId]) REFERENCES [dbo].[ES_Mandators] ([MandatorId])
GO

IF @@ERROR<>0 OR @@TRANCOUNT=0 BEGIN IF @@TRANCOUNT>0 ROLLBACK SET NOEXEC ON END
GO

IF @@TRANCOUNT>0
	COMMIT

SET NOEXEC OFF

