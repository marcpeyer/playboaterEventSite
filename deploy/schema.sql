
--BEGIN TRAN DBMigration

/* ---------------------------------------------------------------------------- */
/* ---------------------------------------------------------------------------- */
/* ---------------------------------------------------------------------------- */
/* ---------------------------------------------------------------------------- */
/* ------------------------ TABLE SCHEMA -------------------------------------- */
/* ---------------------------------------------------------------------------- */
/* ---------------------------------------------------------------------------- */
/* ---------------------------------------------------------------------------- */
/* ---------------------------------------------------------------------------- */
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ES_Log]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[ES_Log](
	[LogId] [int] IDENTITY(1,1) NOT NULL,
	[MandatorId] [char](32) NULL,
	[UserIP] [nvarchar](15) NULL,
	[Date] [smalldatetime] NULL,
	[Login] [varchar](50) NULL,
	[Password] [varchar](50) NULL,
	[Successful] [bit] NOT NULL,
 CONSTRAINT [PK_ES_Log] PRIMARY KEY CLUSTERED 
(
	[LogId] ASC
)WITH (PAD_INDEX  = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ES_Mandators]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[ES_Mandators](
	[MandatorId] [char](32) NOT NULL,
	[MandatorName] [varchar](50) NOT NULL,
	[MandatorShortName] [varchar](16) NOT NULL,
	[MandatorMail] [varchar](50) NOT NULL,
	[EntryPointUrl] [varchar](100) NULL,
	[SiteTitle] [varchar](50) NOT NULL,
	[EventName] [varchar](50) NOT NULL,
	[FeatureAssembly] [varchar](100) NULL,
	[FeatureAssemblyClassName] [varchar](50) NULL,
	[EventNotificationAddressesDefault] [varchar](800) NULL,
	[ShowEventsAsList] [bit] NOT NULL,
	[UseEventCategories] [bit] NOT NULL,
	[UseEventUrl] [bit] NOT NULL,
	[UseMinMaxSubscriptions] [bit] NOT NULL,
	[UseSubscriptions] [bit] NOT NULL,
	[SmsNotifications] [bit] NOT NULL,
	[OnNewEventNotifyContacts] [bit] NOT NULL,
	[OnEditEventNotifyContacts] [bit] NOT NULL,
	[OnNewSubscriptionNotifyContacts] [bit] NOT NULL,
	[OnEditSubscriptionNotifyContacts] [bit] NOT NULL,
	[OnDeleteSubscriptionNotifyContacts] [bit] NOT NULL,
	[IsLiftManagementEnabled] [bit] NOT NULL,
	[NotifyDeletableSubscriptionStates] [bit] NOT NULL CONSTRAINT [DF_ES_Mandators_NotifyDeletableSubscriptionStates]  DEFAULT ((0)),
	[NoSmsCreditNotified] [bit] NOT NULL CONSTRAINT [DF_ES_Mandators_NoSmsCreditNotified]  DEFAULT ((0)),
	[UseExternalAuth] [bit] NOT NULL,
	[AuthTable] [varchar](50) NULL,
	[AuthIdColumn] [varchar](50) NULL,
	[AuthLoginColumn] [varchar](50) NULL,
	[AuthPasswordColumn] [varchar](50) NULL,
	[Helptext] [varchar](1000) NULL,
	[UnsubscribeAllowedFromNumSubscriptions] [int] NULL,
	[UnsubscribeAllowedTillNumSubscriptions] [int] NULL,
	[SmsPurchased] [int] NOT NULL,
	[SmsLog] [int] NOT NULL,
 CONSTRAINT [PK_ES_Mandators] PRIMARY KEY CLUSTERED 
(
	[MandatorId] ASC
)WITH (PAD_INDEX  = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ES_Roles]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[ES_Roles](
	[RoleId] [int] IDENTITY(1,1) NOT NULL,
	[Role] [varchar](20) NOT NULL,
 CONSTRAINT [PK_ES_Roles] PRIMARY KEY CLUSTERED 
(
	[RoleId] ASC
)WITH (PAD_INDEX  = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ES_Subscriptions]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[ES_Subscriptions](
	[SubscriptionId] [int] IDENTITY(1,1) NOT NULL,
	[EventId] [int] NOT NULL,
	[ContactId] [int] NOT NULL,
	[SubscriptionStateId] [int] NOT NULL,
	[SubscriptionTime] [varchar](50) NULL,
	[Comment] [varchar](8000) NULL,
	[NumLifts] [int] NULL,
	[MaxNotifications] [int] NULL,
 CONSTRAINT [PK_ES_Subscriptions] PRIMARY KEY CLUSTERED 
(
	[SubscriptionId] ASC
)WITH (PAD_INDEX  = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[ES_Subscriptions]') AND name = N'IX_Unique_ES_Subscriptions_ContactId_EventId')
CREATE UNIQUE NONCLUSTERED INDEX [IX_Unique_ES_Subscriptions_ContactId_EventId] ON [dbo].[ES_Subscriptions] 
(
	[ContactId] ASC,
	[EventId] ASC
)WITH (PAD_INDEX  = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ES_NotifSubscriptions]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[ES_NotifSubscriptions](
	[NotifSubscriptionId] [int] IDENTITY(1,1) NOT NULL,
	[EventId] [int] NOT NULL,
	[ContactId] [int] NOT NULL,
	[NumSmsNotifications] [int] NULL,
 CONSTRAINT [PK_ES_NotifSubscriptions] PRIMARY KEY CLUSTERED 
(
	[NotifSubscriptionId] ASC
)WITH (PAD_INDEX  = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ES_ContactSettings]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[ES_ContactSettings](
	[ContactSettingId] [int] IDENTITY(1,1) NOT NULL,
	[ContactId] [int] NOT NULL,
	[EventCategoryId] [int] NOT NULL,
	[NotifyBySMS] [bit] NOT NULL,
	[NotifyByEmail] [bit] NOT NULL,
	[SmsNotifSubscriptionsOn] [bit] NOT NULL CONSTRAINT [DF_ES_ContactSettings_SmsNotifSubscriptionsOn]  DEFAULT ((1)),
	[AutoNotifSubscription] [int] NULL,
 CONSTRAINT [PK_ES_ContactEventCategorySettings] PRIMARY KEY CLUSTERED 
(
	[ContactSettingId] ASC
)WITH (PAD_INDEX  = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[ES_ContactSettings]') AND name = N'IX_Unique_ES_ContactSettings_ContactId_EventCategoryId')
CREATE UNIQUE NONCLUSTERED INDEX [IX_Unique_ES_ContactSettings_ContactId_EventCategoryId] ON [dbo].[ES_ContactSettings] 
(
	[ContactId] ASC,
	[EventCategoryId] ASC
)WITH (PAD_INDEX  = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ES_Locations]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[ES_Locations](
	[LocationId] [int] IDENTITY(1,1) NOT NULL,
	[EventCategoryId] [int] NOT NULL,
	[LocationText] [varchar](50) NOT NULL,
	[LocationShort] [varchar](15) NOT NULL,
	[LocationDescription] [varchar](200) NULL,
	[IsDeleted] [bit] NOT NULL CONSTRAINT [DF_ES_Locations_IsDeleted]  DEFAULT ((0)),
 CONSTRAINT [PK_ES_Locations] PRIMARY KEY CLUSTERED 
(
	[LocationId] ASC
)WITH (PAD_INDEX  = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ES_SubscriptionStates]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[ES_SubscriptionStates](
	[SubscriptionStateId] [int] IDENTITY(1,1) NOT NULL,
	[EventCategoryId] [int] NOT NULL,
	[StateText] [varchar](50) NOT NULL,
	[Fontcolor] [char](7) NULL,
	[IsDeletable] [bit] NOT NULL CONSTRAINT [DF_ES_SubscriptionStates_IsDeletable]  DEFAULT ((0)),
	--SubscriptionStateCode: 1 = attend, 2 = later, 3 = NOT attend
	[SubscriptionStateCode] [tinyint] NULL,
 CONSTRAINT [PK_ES_SubscriptionStates] PRIMARY KEY CLUSTERED 
(
	[SubscriptionStateId] ASC
)WITH (PAD_INDEX  = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY],
 CONSTRAINT [IX_Unique_ES_SubscriptionStates_EventCategoryId_StateText] UNIQUE NONCLUSTERED 
(
	[EventCategoryId] ASC,
	[StateText] ASC
)WITH (PAD_INDEX  = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ES_Events]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[ES_Events](
	[EventId] [int] IDENTITY(1,1) NOT NULL,
	[MandatorId] [char](32) NOT NULL,
	[EventCategoryId] [int] NOT NULL,
	[LocationId] [int] NOT NULL,
	[EventCreatorId] [int] NOT NULL,
	[StartDate] [smalldatetime] NOT NULL,
	[Duration] [varchar](50) NULL,
	[EventTitle] [varchar](50) NOT NULL,
	[EventDescription] [varchar](8000) NULL,
	[EventUrl] [varchar](100) NULL,
	[MinSubscriptions] [int] NULL,
	[MaxSubscriptions] [int] NULL,
 CONSTRAINT [PK_ES_Events] PRIMARY KEY CLUSTERED 
(
	[EventId] ASC
)WITH (PAD_INDEX  = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ES_SmsLocations]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[ES_SmsLocations](
	[SmsLocationId] [int] IDENTITY(1,1) NOT NULL,
	[LocationId] [int] NOT NULL,
	[ContactId] [int] NOT NULL,
 CONSTRAINT [PK_ES_SmsLocations] PRIMARY KEY CLUSTERED 
(
	[SmsLocationId] ASC
)WITH (PAD_INDEX  = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ES_ContactsRoles]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[ES_ContactsRoles](
	[ContactsRoleId] [int] IDENTITY(1,1) NOT NULL,
	[ContactId] [int] NOT NULL,
	[RoleId] [int] NOT NULL,
 CONSTRAINT [PK_ES_ContactsRoles] PRIMARY KEY CLUSTERED 
(
	[ContactsRoleId] ASC
)WITH (PAD_INDEX  = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[ES_ContactsRoles]') AND name = N'IX_Unique_ES_Contacts_ContactId_RoleId')
CREATE UNIQUE NONCLUSTERED INDEX [IX_Unique_ES_Contacts_ContactId_RoleId] ON [dbo].[ES_ContactsRoles] 
(
	[ContactId] ASC,
	[RoleId] ASC
)WITH (PAD_INDEX  = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ES_JourneyStations]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[ES_JourneyStations](
	[JourneyStationId] [int] IDENTITY(1,1) NOT NULL,
	[SubscriptionId] [int] NOT NULL,
	[Station] [varchar](100) NOT NULL,
	[StationTime] [varchar](50) NULL,
	[SortOrder] [int] NOT NULL,
 CONSTRAINT [PK_ES_JourneyStations] PRIMARY KEY CLUSTERED 
(
	[JourneyStationId] ASC
)WITH (PAD_INDEX  = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ES_LiftSubscriptions]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[ES_LiftSubscriptions](
	[LiftSubscriptionId] [int] IDENTITY(1,1) NOT NULL,
	[SubscriptionId] [int] NOT NULL,
	[JourneyStationId] [int] NOT NULL,
 CONSTRAINT [PK_ES_LiftSubscriptions] PRIMARY KEY CLUSTERED 
(
	[LiftSubscriptionId] ASC
)WITH (PAD_INDEX  = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[ES_LiftSubscriptions]') AND name = N'IX_Unique_ES_LiftSubscriptions_SubscriptionId')
CREATE UNIQUE NONCLUSTERED INDEX [IX_Unique_ES_LiftSubscriptions_SubscriptionId] ON [dbo].[ES_LiftSubscriptions] 
(
	[SubscriptionId] ASC
)WITH (PAD_INDEX  = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ES_EventCategories]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[ES_EventCategories](
	[EventCategoryId] [int] IDENTITY(1,1) NOT NULL,
	[MandatorId] [char](32) NOT NULL,
	[EventCategory] [varchar](40) NOT NULL,
	[CategoryDescription] [varchar](200) NULL,
	[FeatureAssembly] [varchar](100) NULL,
	[FeatureAssemblyClassName] [varchar](50) NULL,
	[FreeEventSmsNotifications] [bit] NOT NULL CONSTRAINT [DF_ES_EventCategories_FreeEventSmsNotifications]  DEFAULT ((0)),
 CONSTRAINT [PK_ES_EventCategories] PRIMARY KEY CLUSTERED 
(
	[EventCategoryId] ASC
)WITH (PAD_INDEX  = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ES_Contacts]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[ES_Contacts](
	[ContactId] [int] IDENTITY(1,1) NOT NULL,
	[CustomId] [int] NULL,
	[MandatorId] [char](32) NOT NULL,
	[Name] [varchar](50) NOT NULL,
	[Email] [varchar](50) NOT NULL,
	[Login] [varchar](50) NULL,
	[Password] [varchar](50) NULL,
	[MobilePhone] [varchar](20) NULL,
	[LiftMgmtSmsOn] [bit] NOT NULL,
	[EventMgmtSmsOn] [bit] NOT NULL,
	[UseTwoWaySms] [bit] NOT NULL,
	[SmsLog] [int] NOT NULL,
	[SmsPurchased] [int] NOT NULL,
	[IsDeleted] [bit] NOT NULL CONSTRAINT [DF_ES_Contacts_IsDeleted]  DEFAULT ((0)),
	[NoSmsCreditNotified] [bit] NOT NULL CONSTRAINT [DF_ES_Contacts_NoSmsCreditNotified]  DEFAULT ((0)),
 CONSTRAINT [PK_ES_Contacts] PRIMARY KEY CLUSTERED 
(
	[ContactId] ASC
)WITH (PAD_INDEX  = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[ES_Contacts]') AND name = N'IX_Unique_ES_Contacts_Email_MandatorId')
CREATE UNIQUE NONCLUSTERED INDEX [IX_Unique_ES_Contacts_Email_MandatorId] ON [dbo].[ES_Contacts] 
(
	[Email] ASC,
	[MandatorId] ASC
)WITH (PAD_INDEX  = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]
GO

/****** Object:  Table [dbo].[ES_SimpleCache]    Script Date: 11/05/2008 17:15:02 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ES_SimpleCache]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[ES_SimpleCache](
	[Key] [varchar](255) NOT NULL,
	[Value] [varchar](8000) NULL,
	ExpiryDate [datetime] NOT NULL,
 CONSTRAINT [PK_ES_SimpleCache] PRIMARY KEY CLUSTERED 
(
	[Key] ASC
)WITH (PAD_INDEX  = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
END
GO

/* ---------------------------------------------------------------------------- */
/* ---------------------------------------------------------------------------- */
/* ---------------------------------------------------------------------------- */
/* ---------------------------------------------------------------------------- */
/* ------------------- DATA --------------------------------------------------- */
/* ---------------------------------------------------------------------------- */
/* ---------------------------------------------------------------------------- */
/* ---------------------------------------------------------------------------- */
/* ---------------------------------------------------------------------------- */


--SET IDENTITY_INSERT _ES_Events OFF
GO
INSERT INTO ES_Mandators (MandatorId, MandatorName, MandatorShortName, MandatorMail, EntryPointUrl, SiteTitle, EventName, FeatureAssembly, FeatureAssemblyClassName, EventNotificationAddressesDefault, ShowEventsAsList, UseEventCategories, UseEventUrl, UseMinMaxSubscriptions, UseSubscriptions, SmsNotifications, OnNewEventNotifyContacts, OnEditEventNotifyContacts, OnNewSubscriptionNotifyContacts, OnEditSubscriptionNotifyContacts, OnDeleteSubscriptionNotifyContacts, IsLiftManagementEnabled, NotifyDeletableSubscriptionStates, NoSmsCreditNotified, Helptext, UnsubscribeAllowedFromNumSubscriptions, UnsubscribeAllowedTillNumSubscriptions, SmsPurchased, SmsLog, UseExternalAuth)
 SELECT '19C0AE03DC1845669BB65AC79E66ACAF', 'KCM', 'Vermietungen', 'webmaster@kcm.ch', 'http://vermietung.kcm.ch', 'Vermietungen des Kanu Club Murgenthal', 'Vermietung', 'D:\Private\Subversion\playboaterEventSite\bin\LetOutFeature.dll', 'kcm.ch.EventSite.LetOutFeature.Feature', NULL, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 'keine', NULL, NULL, 0, 0, 1
 UNION ALL
 SELECT 'F228DE88AC744AB2919D531714F2EFC8', 'KCM', 'KCM Anlässe', 'webmaster@kcm.ch', 'http://event.kcm.ch', 'Anlässe des Kanu Club Murgenthal', 'Anlass', 'D:\Private\Subversion\playboaterEventSite\bin\RiverLevels.dll', 'playboater.ch.RiverLevels.RiverLevelManager', 'ektreuhand@tiscali.ch;mburato@bwbberatung.ch;buerle.merminod@hispeed.ch;mheger@gmx.ch;PhilipRieder@access.unizh.ch;binu@gmx.ch', 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, '- Das Anmelden an Anlässen ist grundsätzlich verbindlich<br>
- Es ist jedoch möglich Anmeldungen zu löschen solange sich nicht zwei oder mehr Personen für denselben Anlass angemeldet haben<br>
- Die Abfluss-Vorhersagen beruhen u.a. auf Abfluss-Daten der im Flussabschnitt vorher gelegenen Messstationen. Wenn eine Solche Messstation alte Daten liefert, wird die Vorhersage falsch berechnet. In solchen Fällen wird das Datum der veralteten Messdaten <span style="FONT-WEIGHT:bold;COLOR:red">rot</span> markiert.', 4, 1, 475, 210, 1
GO

SET IDENTITY_INSERT ES_Roles ON
GO
INSERT INTO ES_Roles (RoleId, Role)
SELECT 1, 'Reader'
UNION ALL
SELECT 2, 'User'
UNION ALL
SELECT 3, 'EventCreator'
UNION ALL
SELECT 4, 'Manager'
UNION ALL
SELECT 5, 'Administrator'
GO
SET IDENTITY_INSERT ES_Roles OFF
GO

SET IDENTITY_INSERT ES_EventCategories ON
GO
INSERT INTO ES_EventCategories (EventCategoryId, MandatorId, EventCategory, CategoryDescription, FeatureAssembly, FeatureAssemblyClassName, FreeEventSmsNotifications)
 SELECT 1, '19C0AE03DC1845669BB65AC79E66ACAF', 'Clubhausvermietungen', 'Für Vermietungen unseres Clubhauses', NULL, NULL, 0
 UNION ALL
 SELECT 2, '19C0AE03DC1845669BB65AC79E66ACAF', 'Materialvermietungen', 'Für Vermietungen von Material wie Schwimmwesten, Boote ect.', NULL, NULL, 0
 UNION ALL
 SELECT 3, 'F228DE88AC744AB2919D531714F2EFC8', 'Klettern', NULL, 'D:\private\webs\kcm.ch\EventSite\bin\ClimbingFeature.dll', 'kcm.ch.EventSite.ClimbingFeature.Feature', 0
 UNION ALL
 SELECT 4, 'F228DE88AC744AB2919D531714F2EFC8', 'Anlässe vom Verein', 'Anlässe welche vom Verein organisiert werden und in keine andere Kategorie passen wie z.B. Ski-Weekend', NULL, NULL, 1
 UNION ALL
 SELECT 5, 'F228DE88AC744AB2919D531714F2EFC8', 'Paddeln ab WW 3', NULL, NULL, NULL, 1
 UNION ALL
 SELECT 6, 'F228DE88AC744AB2919D531714F2EFC8', 'Paddeln bis WW 3', 'Kann auch Schulung sein wie z.B. Hüningen oder Flachwasserpaddeln auf einem See', NULL, NULL, 1
 UNION ALL
 SELECT 7, 'F228DE88AC744AB2919D531714F2EFC8', 'Paddeln Freestyle', NULL, NULL, NULL, 0
 UNION ALL
 SELECT 8, 'F228DE88AC744AB2919D531714F2EFC8', 'Anlässe von Mitgliedern', 'Anlässe welche von Mitgliedern organisiert werden und in keine andere Kategorie passen wie z.B. spontaner Ski-Tag oder Bräteln', NULL, NULL, 0
GO
SET IDENTITY_INSERT ES_EventCategories OFF
GO

/*SET IDENTITY_INSERT ES_Locations ON
GO
INSERT INTO ES_Locations (LocationId, EventCategoryId, LocationText, LocationShort, LocationDescription, IsDeleted)
 SELECT 1, CASE MandatorId WHEN '19C0AE03DC1845669BB65AC79E66ACAF' THEN 1 WHEN 'F228DE88AC744AB2919D531714F2EFC8' THEN 5 ELSE 4 END, LocationText, LocationShort, LocationDescription, IsDeleted
  FROM ES_Locations
SET IDENTITY_INSERT ES_Locations OFF
GO*/

/*SET IDENTITY_INSERT _ES_Events ON
GO
INSERT INTO ES_Events (EventId, MandatorId, EventCategoryId, LocationId, EventCreatorId, StartDate, Duration, EventTitle, EventDescription, EventUrl, MinSubscriptions, MaxSubscriptions)
 SELECT EventId, CASE e.MandatorId WHEN 'ED7630CABE7B496599EEC2D2753867EE' THEN 'F228DE88AC744AB2919D531714F2EFC8' ELSE e.MandatorId END MandatorId, CASE e.MandatorId WHEN '19C0AE03DC1845669BB65AC79E66ACAF' THEN 1 WHEN 'F228DE88AC744AB2919D531714F2EFC8' THEN 5 ELSE 4 END EventCategoryId, LocationId, CASE WHEN c.MandatorId = 'ED7630CABE7B496599EEC2D2753867EE' THEN (SELECT t.ContactId FROM _ES_Contacts t WHERE t.Email = c.Email AND t.MandatorId = 'F228DE88AC744AB2919D531714F2EFC8') ELSE e.EventCreatorId END EventCreatorId, StartDate, Duration, EventTitle, EventDescription, EventUrl, MinSubscriptions, MaxSubscriptions
  FROM ES_Events e
   INNER JOIN ES_Contacts c ON c.ContactId = e.EventCreatorId
SET IDENTITY_INSERT _ES_Events OFF
GO*/

SET IDENTITY_INSERT ES_SubscriptionStates ON
GO
INSERT INTO ES_SubscriptionStates (SubscriptionStateId, EventCategoryId, StateText, Fontcolor, IsDeletable, SubscriptionStateCode)
 SELECT 1, 3, 'komme an die Wand', '#009900', 0, 1
  UNION SELECT
    2, 3, 'komme nur zum Bier', '#CCCC00', 1, 2
  UNION SELECT
    3, 3, 'komme nicht', '#666666', 1, 3
  UNION SELECT
    4, 4, 'bin dabei', '#009900', 0, 1
  UNION SELECT
    5, 4, 'kann leider nicht kommen', '#666666', 1, 3
  UNION SELECT
    6, 4, 'komme erst später', '#CCCC00', 1, 2
  UNION SELECT
    7, 5, 'komme zum Paddeln', '#009900', 0, 1
  UNION SELECT
    8, 5, 'komme nicht', '#666666', 1, 3
  UNION SELECT
    9, 5, 'komme nur zum Bier', '#CCCC00', 1, 2
  UNION SELECT
    10, 5, 'komme nur zuschauen', '#CCCC00', 1, NULL
  UNION SELECT
    11, 6, 'komme zum Paddeln', '#009900', 0, 1
  UNION SELECT
    12, 6, 'komme nicht', '#666666', 1, 3
  UNION SELECT
    13, 6, 'komme nur zum Bier', '#CCCC00', 1, 2
  UNION SELECT
    14, 6, 'komme nur zuschauen', '#CCCC00', 1, NULL
  UNION SELECT
    15, 7, 'komme zum Paddeln', '#009900', 0, 1
  UNION SELECT
    16, 7, 'komme nicht', '#666666', 1, 3
  UNION SELECT
    17, 7, 'komme nur zum Bier', '#CCCC00', 1, 2
  UNION SELECT
    18, 7, 'komme nur zuschauen', '#CCCC00', 1, NULL
  UNION SELECT
    19, 8, 'bin dabei', '#009900', 0, 1
  UNION SELECT
    20, 8, 'kann leider nicht kommen', '#666666', 1, 3
  UNION SELECT
    21, 8, 'komme erst später', '#CCCC00', 1, 2
SET IDENTITY_INSERT ES_SubscriptionStates OFF
GO

/*SET IDENTITY_INSERT _ES_Subscriptions ON
GO
INSERT INTO ES_Subscriptions (SubscriptionId, EventId, ContactId, SubscriptionStateId, SubscriptionTime, Comment, NumLifts, MaxNotifications)
 SELECT SubscriptionId, EventId, CASE WHEN c.MandatorId = 'ED7630CABE7B496599EEC2D2753867EE' THEN (SELECT t.ContactId FROM _ES_Contacts t WHERE t.Email = c.Email AND t.MandatorId = 'F228DE88AC744AB2919D531714F2EFC8') ELSE s.ContactId END ContactId, CASE SubscriptionStateId WHEN 1 THEN 7 WHEN 2 THEN 8 WHEN 3 THEN 9 WHEN 7 THEN 10 WHEN 4 THEN 4 WHEN 5 THEN 5 WHEN 6 THEN 6 ELSE NULL END, SubscriptionTime, Comment, NumLifts, MaxNotifications
  FROM ES_Subscriptions s
   INNER JOIN ES_Contacts c ON c.ContactId = s.ContactId
SET IDENTITY_INSERT _ES_Subscriptions OFF
GO*/

/*SET IDENTITY_INSERT _ES_JourneyStations ON
GO
INSERT INTO ES_JourneyStations (JourneyStationId, SubscriptionId, Station, StationTime, SortOrder)
 SELECT JourneyStationId, SubscriptionId, Station, StationTime, SortOrder
  FROM ES_JourneyStations
SET IDENTITY_INSERT _ES_JourneyStations OFF
GO*/

/*SET IDENTITY_INSERT _ES_LiftSubscriptions ON
GO
INSERT INTO ES_LiftSubscriptions (LiftSubscriptionId, SubscriptionId, JourneyStationId)
 SELECT LiftSubscriptionId, SubscriptionId, JourneyStationId
  FROM ES_LiftSubscriptions
SET IDENTITY_INSERT _ES_LiftSubscriptions OFF
GO*/

/*SET IDENTITY_INSERT _ES_NotifSubscriptions ON
GO
INSERT INTO ES_NotifSubscriptions (NotifSubscriptionId, EventId, ContactId, NumSmsNotifications)
 SELECT NotifSubscriptionId, EventId, CASE WHEN c.MandatorId = 'ED7630CABE7B496599EEC2D2753867EE' THEN (SELECT t.ContactId FROM _ES_Contacts t WHERE t.Email = c.Email AND t.MandatorId = 'F228DE88AC744AB2919D531714F2EFC8') ELSE s.ContactId END ContactId, NumSmsNotifications
  FROM ES_NotifSubscriptions s
   INNER JOIN ES_Contacts c ON c.ContactId = s.ContactId
SET IDENTITY_INSERT _ES_NotifSubscriptions OFF
GO*/

/*INSERT INTO ES_Log (MandatorId, UserIP, [Date], Login, [Password], Successful)
 SELECT MandatorId, UserIP, [Date], Login, [Password], Successful
  FROM ES_Log
GO*/


/* ---------------------------------------------------------------------------- */
/* ---------------------------------------------------------------------------- */
/* ---------------------------------------------------------------------------- */
/* ---------------------------------------------------------------------------- */
/* ------------------------ DROP OLD TABLES ----------------------------------- */
/* ---------------------------------------------------------------------------- */
/* ---------------------------------------------------------------------------- */
/* ---------------------------------------------------------------------------- */
/* ---------------------------------------------------------------------------- */

/*if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[FK_ES_ContactsRoles_ES_Contacts]') and OBJECTPROPERTY(id, N'IsForeignKey') = 1)
ALTER TABLE [dbo].[ES_ContactsRoles] DROP CONSTRAINT FK_ES_ContactsRoles_ES_Contacts
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[FK_ES_NotifSubscriptions_ES_Contacts]') and OBJECTPROPERTY(id, N'IsForeignKey') = 1)
ALTER TABLE [dbo].[ES_NotifSubscriptions] DROP CONSTRAINT FK_ES_NotifSubscriptions_ES_Contacts
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[FK_ES_SmsLocations_ES_Contacts]') and OBJECTPROPERTY(id, N'IsForeignKey') = 1)
ALTER TABLE [dbo].[ES_SmsLocations] DROP CONSTRAINT FK_ES_SmsLocations_ES_Contacts
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[FK_ES_Subscriptions_ES_Contacts]') and OBJECTPROPERTY(id, N'IsForeignKey') = 1)
ALTER TABLE [dbo].[ES_Subscriptions] DROP CONSTRAINT FK_ES_Subscriptions_ES_Contacts
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[FK_ES_NotifSubscriptions_ES_Events]') and OBJECTPROPERTY(id, N'IsForeignKey') = 1)
ALTER TABLE [dbo].[ES_NotifSubscriptions] DROP CONSTRAINT FK_ES_NotifSubscriptions_ES_Events
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[FK_ES_Subscriptions_ES_Events]') and OBJECTPROPERTY(id, N'IsForeignKey') = 1)
ALTER TABLE [dbo].[ES_Subscriptions] DROP CONSTRAINT FK_ES_Subscriptions_ES_Events
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[FK_ES_LiftSubscriptions_ES_JourneyStations]') and OBJECTPROPERTY(id, N'IsForeignKey') = 1)
ALTER TABLE [dbo].[ES_LiftSubscriptions] DROP CONSTRAINT FK_ES_LiftSubscriptions_ES_JourneyStations
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[FK_ES_Events_ES_Locations]') and OBJECTPROPERTY(id, N'IsForeignKey') = 1)
ALTER TABLE [dbo].[ES_Events] DROP CONSTRAINT FK_ES_Events_ES_Locations
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[FK_ES_SmsLocations_ES_Locations]') and OBJECTPROPERTY(id, N'IsForeignKey') = 1)
ALTER TABLE [dbo].[ES_SmsLocations] DROP CONSTRAINT FK_ES_SmsLocations_ES_Locations
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[FK_ES_Contacts_ES_Mandators]') and OBJECTPROPERTY(id, N'IsForeignKey') = 1)
ALTER TABLE [dbo].[ES_Contacts] DROP CONSTRAINT FK_ES_Contacts_ES_Mandators
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[FK_ES_Events_ES_Mandators]') and OBJECTPROPERTY(id, N'IsForeignKey') = 1)
ALTER TABLE [dbo].[ES_Events] DROP CONSTRAINT FK_ES_Events_ES_Mandators
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[FK_ES_Locations_ES_Mandators]') and OBJECTPROPERTY(id, N'IsForeignKey') = 1)
ALTER TABLE [dbo].[ES_Locations] DROP CONSTRAINT FK_ES_Locations_ES_Mandators
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[FK_ES_SubscriptionStates_ES_Mandators]') and OBJECTPROPERTY(id, N'IsForeignKey') = 1)
ALTER TABLE [dbo].[ES_SubscriptionStates] DROP CONSTRAINT FK_ES_SubscriptionStates_ES_Mandators
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[FK_ES_ContactsRoles_ES_Roles]') and OBJECTPROPERTY(id, N'IsForeignKey') = 1)
ALTER TABLE [dbo].[ES_ContactsRoles] DROP CONSTRAINT FK_ES_ContactsRoles_ES_Roles
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[FK_ES_Subscriptions_ES_SubscriptionStates]') and OBJECTPROPERTY(id, N'IsForeignKey') = 1)
ALTER TABLE [dbo].[ES_Subscriptions] DROP CONSTRAINT FK_ES_Subscriptions_ES_SubscriptionStates
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[FK_ES_JourneyStations_ES_Subscriptions]') and OBJECTPROPERTY(id, N'IsForeignKey') = 1)
ALTER TABLE [dbo].[ES_JourneyStations] DROP CONSTRAINT FK_ES_JourneyStations_ES_Subscriptions
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[FK_ES_LiftSubscriptions_ES_Subscriptions]') and OBJECTPROPERTY(id, N'IsForeignKey') = 1)
ALTER TABLE [dbo].[ES_LiftSubscriptions] DROP CONSTRAINT FK_ES_LiftSubscriptions_ES_Subscriptions
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[ES_Contacts]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[ES_Contacts]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[ES_ContactsRoles]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[ES_ContactsRoles]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[ES_Events]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[ES_Events]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[ES_JourneyStations]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[ES_JourneyStations]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[ES_LiftSubscriptions]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[ES_LiftSubscriptions]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[ES_Locations]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[ES_Locations]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[ES_Log]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[ES_Log]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[ES_Mandators]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[ES_Mandators]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[ES_NotifSubscriptions]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[ES_NotifSubscriptions]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[ES_Roles]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[ES_Roles]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[ES_SmsLocations]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[ES_SmsLocations]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[ES_SubscriptionStates]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[ES_SubscriptionStates]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[ES_Subscriptions]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[ES_Subscriptions]
GO
*/

/* ---------------------------------------------------------------------------- */
/* ---------------------------------------------------------------------------- */
/* ---------------------------------------------------------------------------- */
/* ---------------------------------------------------------------------------- */
/* ------------------------ RENAME TABLES ------------------------------------- */
/* ---------------------------------------------------------------------------- */
/* ---------------------------------------------------------------------------- */
/* ---------------------------------------------------------------------------- */
/* ---------------------------------------------------------------------------- */

/*EXECUTE sp_rename N'dbo._ES_ContactSettings', N'ES_ContactSettings', 'OBJECT'
GO
EXECUTE sp_rename N'dbo._ES_Contacts', N'ES_Contacts', 'OBJECT'
GO
EXECUTE sp_rename N'dbo._ES_ContactsRoles', N'ES_ContactsRoles', 'OBJECT'
GO
EXECUTE sp_rename N'dbo._ES_EventCategories', N'ES_EventCategories', 'OBJECT'
GO
EXECUTE sp_rename N'dbo._ES_Events', N'ES_Events', 'OBJECT'
GO
EXECUTE sp_rename N'dbo._ES_JourneyStations', N'ES_JourneyStations', 'OBJECT'
GO
EXECUTE sp_rename N'dbo._ES_LiftSubscriptions', N'ES_LiftSubscriptions', 'OBJECT'
GO
EXECUTE sp_rename N'dbo._ES_Locations', N'ES_Locations', 'OBJECT'
GO
EXECUTE sp_rename N'dbo._ES_Log', N'ES_Log', 'OBJECT'
GO
EXECUTE sp_rename N'dbo._ES_Mandators', N'ES_Mandators', 'OBJECT'
GO
EXECUTE sp_rename N'dbo._ES_NotifSubscriptions', N'ES_NotifSubscriptions', 'OBJECT'
GO
EXECUTE sp_rename N'dbo._ES_Roles', N'ES_Roles', 'OBJECT'
GO
EXECUTE sp_rename N'dbo._ES_SmsLocations', N'ES_SmsLocations', 'OBJECT'
GO
EXECUTE sp_rename N'dbo._ES_SubscriptionStates', N'ES_SubscriptionStates', 'OBJECT'
GO
EXECUTE sp_rename N'dbo._ES_Subscriptions', N'ES_Subscriptions', 'OBJECT'
GO*/

/* ---------------------------------------------------------------------------- */
/* ---------------------------------------------------------------------------- */
/* ---------------------------------------------------------------------------- */
/* ---------------------------------------------------------------------------- */
/* ------------------------ PRIMARY KEYS -------------------------------------- */
/* ---------------------------------------------------------------------------- */
/* ---------------------------------------------------------------------------- */
/* ---------------------------------------------------------------------------- */
/* ---------------------------------------------------------------------------- */
/*
ALTER TABLE [dbo].[ES_ContactSettings] WITH NOCHECK ADD 
	CONSTRAINT [PK_ES_ContactEventCategorySettings] PRIMARY KEY  CLUSTERED 
	(
		[ContactSettingId]
	)  ON [PRIMARY] 
GO

ALTER TABLE [dbo].[ES_Contacts] WITH NOCHECK ADD 
	CONSTRAINT [PK_ES_Contacts] PRIMARY KEY  CLUSTERED 
	(
		[ContactId]
	)  ON [PRIMARY] 
GO

ALTER TABLE [dbo].[ES_ContactsRoles] WITH NOCHECK ADD 
	CONSTRAINT [PK_ES_ContactsRoles] PRIMARY KEY  CLUSTERED 
	(
		[ContactsRoleId]
	)  ON [PRIMARY] 
GO

ALTER TABLE [dbo].[ES_EventCategories] WITH NOCHECK ADD 
	CONSTRAINT [PK_ES_EventCategories] PRIMARY KEY  CLUSTERED 
	(
		[EventCategoryId]
	)  ON [PRIMARY] 
GO

ALTER TABLE [dbo].[ES_Events] WITH NOCHECK ADD 
	CONSTRAINT [PK_ES_Events] PRIMARY KEY  CLUSTERED 
	(
		[EventId]
	)  ON [PRIMARY] 
GO

ALTER TABLE [dbo].[ES_JourneyStations] WITH NOCHECK ADD 
	CONSTRAINT [PK_ES_JourneyStations] PRIMARY KEY  CLUSTERED 
	(
		[JourneyStationId]
	)  ON [PRIMARY] 
GO

ALTER TABLE [dbo].[ES_LiftSubscriptions] WITH NOCHECK ADD 
	CONSTRAINT [PK_ES_LiftSubscriptions] PRIMARY KEY  CLUSTERED 
	(
		[LiftSubscriptionId]
	)  ON [PRIMARY] 
GO

ALTER TABLE [dbo].[ES_Locations] WITH NOCHECK ADD 
	CONSTRAINT [PK_ES_Locations] PRIMARY KEY  CLUSTERED 
	(
		[LocationId]
	)  ON [PRIMARY] 
GO

ALTER TABLE [dbo].[ES_Log] WITH NOCHECK ADD 
	CONSTRAINT [PK_ES_Log] PRIMARY KEY  CLUSTERED 
	(
		[LogId]
	)  ON [PRIMARY] 
GO

ALTER TABLE [dbo].[ES_Mandators] WITH NOCHECK ADD 
	CONSTRAINT [PK_ES_Mandators] PRIMARY KEY  CLUSTERED 
	(
		[MandatorId]
	)  ON [PRIMARY] 
GO

ALTER TABLE [dbo].[ES_NotifSubscriptions] WITH NOCHECK ADD 
	CONSTRAINT [PK_ES_NotifSubscriptions] PRIMARY KEY  CLUSTERED 
	(
		[NotifSubscriptionId]
	)  ON [PRIMARY] 
GO

ALTER TABLE [dbo].[ES_Roles] WITH NOCHECK ADD 
	CONSTRAINT [PK_ES_Roles] PRIMARY KEY  CLUSTERED 
	(
		[RoleId]
	)  ON [PRIMARY] 
GO

ALTER TABLE [dbo].[ES_SmsLocations] WITH NOCHECK ADD 
	CONSTRAINT [PK_ES_SmsLocations] PRIMARY KEY  CLUSTERED 
	(
		[SmsLocationId]
	)  ON [PRIMARY] 
GO

ALTER TABLE [dbo].[ES_SubscriptionStates] WITH NOCHECK ADD 
	CONSTRAINT [PK_ES_SubscriptionStates] PRIMARY KEY  CLUSTERED 
	(
		[SubscriptionStateId]
	)  ON [PRIMARY] 
GO

ALTER TABLE [dbo].[ES_Subscriptions] WITH NOCHECK ADD 
	CONSTRAINT [PK_ES_Subscriptions] PRIMARY KEY  CLUSTERED 
	(
		[SubscriptionId]
	)  ON [PRIMARY] 
GO
*/

/* ---------------------------------------------------------------------------- */
/* ---------------------------------------------------------------------------- */
/* ---------------------------------------------------------------------------- */
/* ---------------------------------------------------------------------------- */
/* ----------------------- INDEXES ------------------------------------------- */
/* ---------------------------------------------------------------------------- */
/* ---------------------------------------------------------------------------- */
/* ---------------------------------------------------------------------------- */
/* ---------------------------------------------------------------------------- */

/*
 CREATE  UNIQUE  INDEX [IX_Unique_ES_ContactSettings_ContactId_EventCategoryId] ON [dbo].[ES_ContactSettings]([ContactId], [EventCategoryId]) ON [PRIMARY]
GO

 CREATE  UNIQUE  INDEX [IX_Unique_ES_Contacts_Email_MandatorId] ON [dbo].[ES_Contacts]([Email], [MandatorId]) ON [PRIMARY]
GO

 CREATE  UNIQUE  INDEX [IX_Unique_ES_Contacts_ContactId_RoleId] ON [dbo].[ES_ContactsRoles]([ContactId], [RoleId]) ON [PRIMARY]
GO
 CREATE  UNIQUE  INDEX [IX_Unique_ES_LiftSubscriptions_SubscriptionId] ON [dbo].[ES_LiftSubscriptions]([SubscriptionId]) ON [PRIMARY]
GO

ALTER TABLE [dbo].[ES_SubscriptionStates] ADD 
	CONSTRAINT [IX_Unique_ES_SubscriptionStates_EventCategoryId_StateText] UNIQUE  NONCLUSTERED 
	(
		[EventCategoryId],
		[StateText]
	)  ON [PRIMARY] 
GO

 CREATE  UNIQUE  INDEX [IX_Unique_ES_Subscriptions_ContactId_EventId] ON [dbo].[ES_Subscriptions]([ContactId], [EventId]) ON [PRIMARY]
GO
*/


/* ---------------------------------------------------------------------------- */
/* ---------------------------------------------------------------------------- */
/* ---------------------------------------------------------------------------- */
/* ---------------------------------------------------------------------------- */
/* ----------------------- FOREIGN KEY CONSTRAINTS ---------------------------- */
/* ---------------------------------------------------------------------------- */
/* ---------------------------------------------------------------------------- */
/* ---------------------------------------------------------------------------- */
/* ---------------------------------------------------------------------------- */


ALTER TABLE [dbo].[ES_ContactSettings] ADD 
	CONSTRAINT [FK_ES_ContactSettings_ES_Contacts] FOREIGN KEY 
	(
		[ContactId]
	) REFERENCES [dbo].[ES_Contacts] (
		[ContactId]
	),
	CONSTRAINT [FK_ES_ContactSettings_ES_EventCategories1] FOREIGN KEY 
	(
		[EventCategoryId]
	) REFERENCES [dbo].[ES_EventCategories] (
		[EventCategoryId]
	)
GO

ALTER TABLE [dbo].[ES_Contacts] ADD 
	CONSTRAINT [FK_ES_Contacts_ES_Mandators] FOREIGN KEY 
	(
		[MandatorId]
	) REFERENCES [dbo].[ES_Mandators] (
		[MandatorId]
	)
GO

ALTER TABLE [dbo].[ES_ContactsRoles] ADD 
	CONSTRAINT [FK_ES_ContactsRoles_ES_Contacts] FOREIGN KEY 
	(
		[ContactId]
	) REFERENCES [dbo].[ES_Contacts] (
		[ContactId]
	),
	CONSTRAINT [FK_ES_ContactsRoles_ES_Roles] FOREIGN KEY 
	(
		[RoleId]
	) REFERENCES [dbo].[ES_Roles] (
		[RoleId]
	)
GO

ALTER TABLE [dbo].[ES_EventCategories] ADD 
	CONSTRAINT [FK_ES_EventCategories_ES_Mandators] FOREIGN KEY 
	(
		[MandatorId]
	) REFERENCES [dbo].[ES_Mandators] (
		[MandatorId]
	)
GO

ALTER TABLE [dbo].[ES_Events] ADD 
	CONSTRAINT [FK_ES_Events_ES_EventCategories] FOREIGN KEY 
	(
		[EventCategoryId]
	) REFERENCES [dbo].[ES_EventCategories] (
		[EventCategoryId]
	),
	CONSTRAINT [FK_ES_Events_ES_Locations] FOREIGN KEY 
	(
		[LocationId]
	) REFERENCES [dbo].[ES_Locations] (
		[LocationId]
	),
	CONSTRAINT [FK_ES_Events_ES_Mandators] FOREIGN KEY 
	(
		[MandatorId]
	) REFERENCES [dbo].[ES_Mandators] (
		[MandatorId]
	),
	CONSTRAINT [FK_ES_Events_ES_Contacts] FOREIGN KEY 
	(
		[EventCreatorId]
	) REFERENCES [dbo].[ES_Contacts] (
		[ContactId]
	)
GO

ALTER TABLE [dbo].[ES_JourneyStations] ADD 
	CONSTRAINT [FK_ES_JourneyStations_ES_Subscriptions] FOREIGN KEY 
	(
		[SubscriptionId]
	) REFERENCES [dbo].[ES_Subscriptions] (
		[SubscriptionId]
	)
GO

ALTER TABLE [dbo].[ES_LiftSubscriptions] ADD 
	CONSTRAINT [FK_ES_LiftSubscriptions_ES_JourneyStations] FOREIGN KEY 
	(
		[JourneyStationId]
	) REFERENCES [dbo].[ES_JourneyStations] (
		[JourneyStationId]
	),
	CONSTRAINT [FK_ES_LiftSubscriptions_ES_Subscriptions] FOREIGN KEY 
	(
		[SubscriptionId]
	) REFERENCES [dbo].[ES_Subscriptions] (
		[SubscriptionId]
	)
GO

ALTER TABLE [dbo].[ES_Locations] ADD 
	CONSTRAINT [FK_ES_Locations_ES_EventCategories] FOREIGN KEY 
	(
		[EventCategoryId]
	) REFERENCES [dbo].[ES_EventCategories] (
		[EventCategoryId]
	)
GO

ALTER TABLE [dbo].[ES_NotifSubscriptions] ADD 
	CONSTRAINT [FK_ES_NotifSubscriptions_ES_Contacts] FOREIGN KEY 
	(
		[ContactId]
	) REFERENCES [dbo].[ES_Contacts] (
		[ContactId]
	),
	CONSTRAINT [FK_ES_NotifSubscriptions_ES_Events] FOREIGN KEY 
	(
		[EventId]
	) REFERENCES [dbo].[ES_Events] (
		[EventId]
	)
GO

ALTER TABLE [dbo].[ES_SmsLocations] ADD 
	CONSTRAINT [FK_ES_SmsLocations_ES_Contacts] FOREIGN KEY 
	(
		[ContactId]
	) REFERENCES [dbo].[ES_Contacts] (
		[ContactId]
	),
	CONSTRAINT [FK_ES_SmsLocations_ES_Locations] FOREIGN KEY 
	(
		[LocationId]
	) REFERENCES [dbo].[ES_Locations] (
		[LocationId]
	)
GO

ALTER TABLE [dbo].[ES_SubscriptionStates] ADD 
	CONSTRAINT [FK_ES_SubscriptionStates_ES_EventCategories] FOREIGN KEY 
	(
		[EventCategoryId]
	) REFERENCES [dbo].[ES_EventCategories] (
		[EventCategoryId]
	)
GO

ALTER TABLE [dbo].[ES_Subscriptions] ADD 
	CONSTRAINT [FK_ES_Subscriptions_ES_Contacts] FOREIGN KEY 
	(
		[ContactId]
	) REFERENCES [dbo].[ES_Contacts] (
		[ContactId]
	),
	CONSTRAINT [FK_ES_Subscriptions_ES_Events] FOREIGN KEY 
	(
		[EventId]
	) REFERENCES [dbo].[ES_Events] (
		[EventId]
	),
	CONSTRAINT [FK_ES_Subscriptions_ES_SubscriptionStates] FOREIGN KEY 
	(
		[SubscriptionStateId]
	) REFERENCES [dbo].[ES_SubscriptionStates] (
		[SubscriptionStateId]
	)
GO

--ROLLBACK TRAN DBMigration
GO
/* ---------------------------------------------------------------------------- */
/* ---------------------------------------------------------------------------- */
/* ---------------------------------------------------------------------------- */
/* ---------------------------------------------------------------------------- */
/* ----------------------- PERMISSIONS --------------------------------------- */
/* ---------------------------------------------------------------------------- */
/* ---------------------------------------------------------------------------- */
/* ---------------------------------------------------------------------------- */
/* ---------------------------------------------------------------------------- */

GRANT SELECT ON [dbo].[ES_Contacts] TO [EventSiteUsers]
GO

/*
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_ES_Subscriptions_ES_Contacts]') AND parent_object_id = OBJECT_ID(N'[dbo].[ES_Subscriptions]'))
ALTER TABLE [dbo].[ES_Subscriptions]  WITH NOCHECK ADD  CONSTRAINT [FK_ES_Subscriptions_ES_Contacts] FOREIGN KEY([ContactId])
REFERENCES [dbo].[ES_Contacts] ([ContactId])
GO
ALTER TABLE [dbo].[ES_Subscriptions] CHECK CONSTRAINT [FK_ES_Subscriptions_ES_Contacts]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_ES_Subscriptions_ES_Events]') AND parent_object_id = OBJECT_ID(N'[dbo].[ES_Subscriptions]'))
ALTER TABLE [dbo].[ES_Subscriptions]  WITH NOCHECK ADD  CONSTRAINT [FK_ES_Subscriptions_ES_Events] FOREIGN KEY([EventId])
REFERENCES [dbo].[ES_Events] ([EventId])
GO
ALTER TABLE [dbo].[ES_Subscriptions] CHECK CONSTRAINT [FK_ES_Subscriptions_ES_Events]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_ES_Subscriptions_ES_SubscriptionStates]') AND parent_object_id = OBJECT_ID(N'[dbo].[ES_Subscriptions]'))
ALTER TABLE [dbo].[ES_Subscriptions]  WITH NOCHECK ADD  CONSTRAINT [FK_ES_Subscriptions_ES_SubscriptionStates] FOREIGN KEY([SubscriptionStateId])
REFERENCES [dbo].[ES_SubscriptionStates] ([SubscriptionStateId])
GO
ALTER TABLE [dbo].[ES_Subscriptions] CHECK CONSTRAINT [FK_ES_Subscriptions_ES_SubscriptionStates]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_ES_NotifSubscriptions_ES_Contacts]') AND parent_object_id = OBJECT_ID(N'[dbo].[ES_NotifSubscriptions]'))
ALTER TABLE [dbo].[ES_NotifSubscriptions]  WITH NOCHECK ADD  CONSTRAINT [FK_ES_NotifSubscriptions_ES_Contacts] FOREIGN KEY([ContactId])
REFERENCES [dbo].[ES_Contacts] ([ContactId])
GO
ALTER TABLE [dbo].[ES_NotifSubscriptions] CHECK CONSTRAINT [FK_ES_NotifSubscriptions_ES_Contacts]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_ES_NotifSubscriptions_ES_Events]') AND parent_object_id = OBJECT_ID(N'[dbo].[ES_NotifSubscriptions]'))
ALTER TABLE [dbo].[ES_NotifSubscriptions]  WITH NOCHECK ADD  CONSTRAINT [FK_ES_NotifSubscriptions_ES_Events] FOREIGN KEY([EventId])
REFERENCES [dbo].[ES_Events] ([EventId])
GO
ALTER TABLE [dbo].[ES_NotifSubscriptions] CHECK CONSTRAINT [FK_ES_NotifSubscriptions_ES_Events]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_ES_ContactSettings_ES_Contacts1]') AND parent_object_id = OBJECT_ID(N'[dbo].[ES_ContactSettings]'))
ALTER TABLE [dbo].[ES_ContactSettings]  WITH NOCHECK ADD  CONSTRAINT [FK_ES_ContactSettings_ES_Contacts1] FOREIGN KEY([ContactId])
REFERENCES [dbo].[ES_Contacts] ([ContactId])
GO
ALTER TABLE [dbo].[ES_ContactSettings] CHECK CONSTRAINT [FK_ES_ContactSettings_ES_Contacts1]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_ES_ContactSettings_ES_EventCategories1]') AND parent_object_id = OBJECT_ID(N'[dbo].[ES_ContactSettings]'))
ALTER TABLE [dbo].[ES_ContactSettings]  WITH NOCHECK ADD  CONSTRAINT [FK_ES_ContactSettings_ES_EventCategories1] FOREIGN KEY([EventCategoryId])
REFERENCES [dbo].[ES_EventCategories] ([EventCategoryId])
GO
ALTER TABLE [dbo].[ES_ContactSettings] CHECK CONSTRAINT [FK_ES_ContactSettings_ES_EventCategories1]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_ES_Locations_ES_EventCategories]') AND parent_object_id = OBJECT_ID(N'[dbo].[ES_Locations]'))
ALTER TABLE [dbo].[ES_Locations]  WITH NOCHECK ADD  CONSTRAINT [FK_ES_Locations_ES_EventCategories] FOREIGN KEY([EventCategoryId])
REFERENCES [dbo].[ES_EventCategories] ([EventCategoryId])
GO
ALTER TABLE [dbo].[ES_Locations] CHECK CONSTRAINT [FK_ES_Locations_ES_EventCategories]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_ES_SubscriptionStates_ES_EventCategories]') AND parent_object_id = OBJECT_ID(N'[dbo].[ES_SubscriptionStates]'))
ALTER TABLE [dbo].[ES_SubscriptionStates]  WITH NOCHECK ADD  CONSTRAINT [FK_ES_SubscriptionStates_ES_EventCategories] FOREIGN KEY([EventCategoryId])
REFERENCES [dbo].[ES_EventCategories] ([EventCategoryId])
GO
ALTER TABLE [dbo].[ES_SubscriptionStates] CHECK CONSTRAINT [FK_ES_SubscriptionStates_ES_EventCategories]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_ES_Events_ES_Contacts]') AND parent_object_id = OBJECT_ID(N'[dbo].[ES_Events]'))
ALTER TABLE [dbo].[ES_Events]  WITH NOCHECK ADD  CONSTRAINT [FK_ES_Events_ES_Contacts] FOREIGN KEY([EventCreatorId])
REFERENCES [dbo].[ES_Contacts] ([ContactId])
GO
ALTER TABLE [dbo].[ES_Events] CHECK CONSTRAINT [FK_ES_Events_ES_Contacts]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_ES_Events_ES_EventCategories]') AND parent_object_id = OBJECT_ID(N'[dbo].[ES_Events]'))
ALTER TABLE [dbo].[ES_Events]  WITH NOCHECK ADD  CONSTRAINT [FK_ES_Events_ES_EventCategories] FOREIGN KEY([EventCategoryId])
REFERENCES [dbo].[ES_EventCategories] ([EventCategoryId])
GO
ALTER TABLE [dbo].[ES_Events] CHECK CONSTRAINT [FK_ES_Events_ES_EventCategories]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_ES_Events_ES_Locations]') AND parent_object_id = OBJECT_ID(N'[dbo].[ES_Events]'))
ALTER TABLE [dbo].[ES_Events]  WITH NOCHECK ADD  CONSTRAINT [FK_ES_Events_ES_Locations] FOREIGN KEY([LocationId])
REFERENCES [dbo].[ES_Locations] ([LocationId])
GO
ALTER TABLE [dbo].[ES_Events] CHECK CONSTRAINT [FK_ES_Events_ES_Locations]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_ES_Events_ES_Mandators]') AND parent_object_id = OBJECT_ID(N'[dbo].[ES_Events]'))
ALTER TABLE [dbo].[ES_Events]  WITH NOCHECK ADD  CONSTRAINT [FK_ES_Events_ES_Mandators] FOREIGN KEY([MandatorId])
REFERENCES [dbo].[ES_Mandators] ([MandatorId])
GO
ALTER TABLE [dbo].[ES_Events] CHECK CONSTRAINT [FK_ES_Events_ES_Mandators]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_ES_SmsLocations_ES_Contacts]') AND parent_object_id = OBJECT_ID(N'[dbo].[ES_SmsLocations]'))
ALTER TABLE [dbo].[ES_SmsLocations]  WITH CHECK ADD  CONSTRAINT [FK_ES_SmsLocations_ES_Contacts] FOREIGN KEY([ContactId])
REFERENCES [dbo].[ES_Contacts] ([ContactId])
GO
ALTER TABLE [dbo].[ES_SmsLocations] CHECK CONSTRAINT [FK_ES_SmsLocations_ES_Contacts]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_ES_SmsLocations_ES_Locations]') AND parent_object_id = OBJECT_ID(N'[dbo].[ES_SmsLocations]'))
ALTER TABLE [dbo].[ES_SmsLocations]  WITH CHECK ADD  CONSTRAINT [FK_ES_SmsLocations_ES_Locations] FOREIGN KEY([LocationId])
REFERENCES [dbo].[ES_Locations] ([LocationId])
GO
ALTER TABLE [dbo].[ES_SmsLocations] CHECK CONSTRAINT [FK_ES_SmsLocations_ES_Locations]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_ES_ContactsRoles_ES_Contacts]') AND parent_object_id = OBJECT_ID(N'[dbo].[ES_ContactsRoles]'))
ALTER TABLE [dbo].[ES_ContactsRoles]  WITH NOCHECK ADD  CONSTRAINT [FK_ES_ContactsRoles_ES_Contacts] FOREIGN KEY([ContactId])
REFERENCES [dbo].[ES_Contacts] ([ContactId])
GO
ALTER TABLE [dbo].[ES_ContactsRoles] CHECK CONSTRAINT [FK_ES_ContactsRoles_ES_Contacts]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_ES_ContactsRoles_ES_Roles]') AND parent_object_id = OBJECT_ID(N'[dbo].[ES_ContactsRoles]'))
ALTER TABLE [dbo].[ES_ContactsRoles]  WITH NOCHECK ADD  CONSTRAINT [FK_ES_ContactsRoles_ES_Roles] FOREIGN KEY([RoleId])
REFERENCES [dbo].[ES_Roles] ([RoleId])
GO
ALTER TABLE [dbo].[ES_ContactsRoles] CHECK CONSTRAINT [FK_ES_ContactsRoles_ES_Roles]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_ES_JourneyStations_ES_Subscriptions]') AND parent_object_id = OBJECT_ID(N'[dbo].[ES_JourneyStations]'))
ALTER TABLE [dbo].[ES_JourneyStations]  WITH NOCHECK ADD  CONSTRAINT [FK_ES_JourneyStations_ES_Subscriptions] FOREIGN KEY([SubscriptionId])
REFERENCES [dbo].[ES_Subscriptions] ([SubscriptionId])
GO
ALTER TABLE [dbo].[ES_JourneyStations] CHECK CONSTRAINT [FK_ES_JourneyStations_ES_Subscriptions]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_ES_LiftSubscriptions_ES_JourneyStations]') AND parent_object_id = OBJECT_ID(N'[dbo].[ES_LiftSubscriptions]'))
ALTER TABLE [dbo].[ES_LiftSubscriptions]  WITH NOCHECK ADD  CONSTRAINT [FK_ES_LiftSubscriptions_ES_JourneyStations] FOREIGN KEY([JourneyStationId])
REFERENCES [dbo].[ES_JourneyStations] ([JourneyStationId])
GO
ALTER TABLE [dbo].[ES_LiftSubscriptions] CHECK CONSTRAINT [FK_ES_LiftSubscriptions_ES_JourneyStations]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_ES_LiftSubscriptions_ES_Subscriptions]') AND parent_object_id = OBJECT_ID(N'[dbo].[ES_LiftSubscriptions]'))
ALTER TABLE [dbo].[ES_LiftSubscriptions]  WITH NOCHECK ADD  CONSTRAINT [FK_ES_LiftSubscriptions_ES_Subscriptions] FOREIGN KEY([SubscriptionId])
REFERENCES [dbo].[ES_Subscriptions] ([SubscriptionId])
GO
ALTER TABLE [dbo].[ES_LiftSubscriptions] CHECK CONSTRAINT [FK_ES_LiftSubscriptions_ES_Subscriptions]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_ES_EventCategories_ES_Mandators]') AND parent_object_id = OBJECT_ID(N'[dbo].[ES_EventCategories]'))
ALTER TABLE [dbo].[ES_EventCategories]  WITH NOCHECK ADD  CONSTRAINT [FK_ES_EventCategories_ES_Mandators] FOREIGN KEY([MandatorId])
REFERENCES [dbo].[ES_Mandators] ([MandatorId])
GO
ALTER TABLE [dbo].[ES_EventCategories] CHECK CONSTRAINT [FK_ES_EventCategories_ES_Mandators]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_ES_Contacts_ES_Mandators]') AND parent_object_id = OBJECT_ID(N'[dbo].[ES_Contacts]'))
ALTER TABLE [dbo].[ES_Contacts]  WITH NOCHECK ADD  CONSTRAINT [FK_ES_Contacts_ES_Mandators] FOREIGN KEY([MandatorId])
REFERENCES [dbo].[ES_Mandators] ([MandatorId])
GO
ALTER TABLE [dbo].[ES_Contacts] CHECK CONSTRAINT [FK_ES_Contacts_ES_Mandators]
*/