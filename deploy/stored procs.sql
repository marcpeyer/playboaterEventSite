
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ES_ListEvents]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'


-- listet alle Events eines Mandanten auf
-- Returns:
-- 0 = ok
CREATE PROCEDURE [dbo].[ES_ListEvents]
 @MandatorId CHAR(32),
 @OnlyFuture BIT = 0
AS

DECLARE @CurrentDate DATETIME
DECLARE @MinDate DATETIME
SELECT @CurrentDate = CONVERT(DATETIME, CONVERT(VARCHAR(10), GETDATE(), 112))
SELECT @MinDate = CONVERT(DATETIME, ''1900-01-01'')

 SELECT *
  FROM ES_Events
  WHERE MandatorId = @MandatorId
    AND StartDate >= CASE WHEN @OnlyFuture = 1 THEN @CurrentDate ELSE @MinDate END

 RETURN 0

' 
END
GO
SET ANSI_NULLS OFF
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ES_ListEventsByCategory]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'


-- listet alle Events einer Category auf
-- Returns:
-- 0 = ok
CREATE PROCEDURE [dbo].[ES_ListEventsByCategory]
 @EventCategoryId INT,
 @OnlyFuture BIT = 0
AS

DECLARE @CurrentDate DATETIME
DECLARE @MinDate DATETIME
SELECT @CurrentDate = CONVERT(DATETIME, CONVERT(VARCHAR(10), GETDATE(), 112))
SELECT @MinDate = CONVERT(DATETIME, ''1900-01-01'')

 SELECT *
  FROM ES_Events
  WHERE EventCategoryId = @EventCategoryId
    AND StartDate >= CASE WHEN @OnlyFuture = 1 THEN @CurrentDate ELSE @MinDate END

 RETURN 0

' 
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ES_GetEvent]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'


-- gibt die gewünschten event daten aus der tab ES_Events zurück
-- Returns:
-- 0 = ok
-- 100 = gewünschter event wurde nicht gefunden
CREATE PROCEDURE [dbo].[ES_GetEvent]
 @EventId INT
AS

 SELECT *
  FROM ES_Events
  WHERE EventId = @EventId
 IF @@ROWCOUNT = 1
  RETURN 0
 ELSE
  RETURN 100





' 
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ES_EditEvent]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'


-- ändert einen event in der tab ES_Events
-- Returns:
-- 0 = ok
-- 100 = existiert bereits mit diesem Titel und Datum
-- 101 = event nicht gefunden
-- 102 = fehler beim update
CREATE PROCEDURE [dbo].[ES_EditEvent]
 @EventId INT,
 @LocationId INT,
 @StartDate VARCHAR(19),
 @Duration VARCHAR(50),
 @EventTitle VARCHAR(50),
 @EventDescription VARCHAR(8000),
 @EventUrl VARCHAR(100),
 @MinSubscriptions INT,
 @MaxSubscriptions INT
AS

DECLARE @MandatorId CHAR(32)
SELECT @MandatorId = MandatorId FROM ES_Events WHERE EventId = @EventId
IF @@ROWCOUNT=0 BEGIN
  RETURN 101
END

DECLARE @StartDateTmp DATETIME
--CHECK ONLY DATE NOT TIME FOR DUPLICATE EVENTS:
SELECT @StartDateTmp = CONVERT(DATETIME, LEFT(@StartDate, 10), 104)

IF NOT EXISTS(SELECT * FROM ES_Events WHERE EventTitle = @EventTitle AND CONVERT(DATETIME, CONVERT(VARCHAR(10), StartDate, 112)) = @StartDateTmp AND MandatorId = @MandatorId AND EventId <> @EventId) BEGIN
  UPDATE ES_Events
   SET LocationId = @LocationId, StartDate = CONVERT(DATETIME, @StartDate, 104), Duration = @Duration, EventTitle = @EventTitle, EventDescription = @EventDescription, EventUrl = @EventUrl, MinSubscriptions = @MinSubscriptions, MaxSubscriptions = @MaxSubscriptions
    WHERE @EventTitle IS NOT NULL AND @EventTitle <> ''''
      AND @StartDateTmp IS NOT NULL
      AND EventId = @EventId
  IF @@ROWCOUNT=1 BEGIN
    RETURN 0
  END ELSE
    RETURN 102
END ELSE BEGIN
  RETURN 100
END


' 
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ES_AddEvent]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'


-- fügt einen neuen event in die tab ES_Events ein
-- Returns:
-- 0 = ok
-- 100 = existiert bereits in dieser Kategorie, mit diesem Titel und Datum
-- 101 = fehler beim insert
CREATE PROCEDURE [dbo].[ES_AddEvent]
 @MandatorId CHAR(32),
 @LocationId INT,
 @EventCreatorId INT,
 @EventCategoryId INT,
 @StartDate VARCHAR(19),
 @Duration VARCHAR(50),
 @EventTitle VARCHAR(50),
 @EventDescription VARCHAR(8000),
 @EventUrl VARCHAR(100),
 @MinSubscriptions INT,
 @MaxSubscriptions INT
AS

DECLARE @StartDateTmp DATETIME
--CHECK ONLY DATE NOT TIME FOR DUPLICATE EVENTS:
SELECT @StartDateTmp = CONVERT(DATETIME, LEFT(@StartDate, 10), 104)

IF NOT EXISTS(SELECT * FROM ES_Events WHERE EventTitle = @EventTitle AND CONVERT(DATETIME, CONVERT(VARCHAR(10), StartDate, 112)) = @StartDateTmp AND MandatorId = @MandatorId AND EventCategoryId = @EventCategoryId) BEGIN
  INSERT ES_Events(MandatorId, LocationId, EventCreatorId, EventCategoryId, StartDate, Duration, EventTitle, EventDescription, EventUrl, MinSubscriptions, MaxSubscriptions)
   SELECT @MandatorId, @LocationId, @EventCreatorId, @EventCategoryId, CONVERT(DATETIME, @StartDate, 104), @Duration, @EventTitle, @EventDescription, @EventUrl, @MinSubscriptions, @MaxSubscriptions
    WHERE @EventTitle IS NOT NULL AND @EventTitle <> ''''
      AND @StartDateTmp IS NOT NULL
  IF @@ROWCOUNT=1 BEGIN
    SELECT @@IDENTITY
    RETURN 0
  END ELSE
    RETURN 101
END ELSE BEGIN
  RETURN 100
END


' 
END
GO
SET ANSI_NULLS OFF
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ES_GetSubscription]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'


-- gibt eine Anmeldung aufgrund dessen id zurück
-- Returns:
-- 0 = ok
-- 10 = not found
CREATE PROCEDURE [dbo].[ES_GetSubscription]
 @SubscriptionId INT
AS

 SELECT
  su.*,
  ls.JourneyStationId LiftSubscriptionJourneyStationId,
  ls.LiftSubscriptionId,
  ss.StateText SubscriptionStateText,
  ss.IsDeletable SubscriptionStateIsDeletable,
  ss.Fontcolor,
  CASE WHEN ls.LiftSubscriptionId IS NOT NULL
       THEN 3 --lift
       ELSE CASE WHEN (SELECT COUNT(*) FROM ES_JourneyStations jsCount INNER JOIN ES_Subscriptions suCount ON suCount.SubscriptionId = jsCount.SubscriptionId WHERE jsCount.SubscriptionId = su.SubscriptionId) > 0
                 THEN CASE WHEN (SELECT COUNT(*) FROM ES_LiftSubscriptions lsCount INNER JOIN ES_JourneyStations js2 ON js2.JourneyStationId = lsCount.JourneyStationId WHERE js2.SubscriptionId = su.SubscriptionId) >= su.NumLifts
                           THEN 2 --full
                           ELSE 1 --drives and has free places
                      END
                 ELSE 0 --undefined
            END
  END LiftState
  FROM ES_Subscriptions su
   INNER JOIN ES_SubscriptionStates ss ON ss.SubscriptionStateId = su.SubscriptionStateId
   INNER JOIN ES_Events ev ON ev.EventId = su.EventId
   INNER JOIN ES_Mandators m ON m.MandatorId = ev.MandatorId
   LEFT JOIN ES_LiftSubscriptions ls ON ls.SubscriptionId = su.SubscriptionId
  WHERE su.SubscriptionId = @SubscriptionId

 IF @@ROWCOUNT=0
  RETURN 10
 ELSE
  RETURN 0



' 
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ES_ListSubscriptions]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'


-- listet alle Anmeldungen eines Events auf
-- Returns:
-- 0 = ok
CREATE  PROCEDURE [dbo].[ES_ListSubscriptions]
 @EventId INT
AS

 SELECT count(*) NumRealSubscriptions
  FROM ES_Subscriptions su
   INNER JOIN ES_SubscriptionStates ss ON ss.SubscriptionStateId = su.SubscriptionStateId
  WHERE su.EventId = @EventId
    AND ss.IsDeletable = 0

 SELECT
  su.*,
  ls.JourneyStationId LiftSubscriptionJourneyStationId,
  ls.LiftSubscriptionId,
  ss.StateText SubscriptionStateText,
  ss.IsDeletable SubscriptionStateIsDeletable,
  ss.Fontcolor,
  CASE WHEN ls.LiftSubscriptionId IS NOT NULL
       THEN 3 --lift
       ELSE CASE WHEN (SELECT COUNT(*) FROM ES_JourneyStations jsCount INNER JOIN ES_Subscriptions suCount ON suCount.SubscriptionId = jsCount.SubscriptionId AND suCount.SubscriptionId = su.SubscriptionId) > 0
                 THEN CASE WHEN (SELECT COUNT(*) FROM ES_LiftSubscriptions lsCount INNER JOIN ES_JourneyStations js2 ON js2.JourneyStationId = lsCount.JourneyStationId WHERE js2.SubscriptionId = su.SubscriptionId) >= su.NumLifts
                           THEN 2 --full
                           ELSE 1 --drives and has free places
                      END
                 ELSE 0 --undefined
            END
  END LiftState
  FROM ES_Subscriptions su
   INNER JOIN ES_Contacts co ON co.ContactId = su.ContactId
   INNER JOIN ES_SubscriptionStates ss ON ss.SubscriptionStateId = su.SubscriptionStateId
   INNER JOIN ES_Events ev ON ev.EventId = su.EventId
   INNER JOIN ES_Mandators m ON m.MandatorId = ev.MandatorId
   LEFT JOIN ES_LiftSubscriptions ls ON ls.SubscriptionId = su.SubscriptionId
  WHERE su.EventId = @EventId
  ORDER BY ss.SubscriptionStateId, co.[Name]

 RETURN 0



' 
END
GO
SET ANSI_NULLS OFF
GO
SET QUOTED_IDENTIFIER OFF
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ES_GetSubscriptionFromContact]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'


-- gibt eine Anmeldung eines Kontaktes an einem Event zurück
-- Returns:
-- 0 = ok
-- 10 = not found
CREATE PROCEDURE [dbo].[ES_GetSubscriptionFromContact]
 @EventId INT,
 @ContactId INT
AS

 SELECT
  su.*,
  ls.JourneyStationId LiftSubscriptionJourneyStationId,
  ls.LiftSubscriptionId,
  ss.StateText SubscriptionStateText,
  ss.IsDeletable SubscriptionStateIsDeletable,
  ss.Fontcolor,
  CASE WHEN ls.LiftSubscriptionId IS NOT NULL
       THEN 3 --lift
       ELSE CASE WHEN (SELECT COUNT(*) FROM ES_JourneyStations jsCount INNER JOIN ES_Subscriptions suCount ON suCount.SubscriptionId = jsCount.SubscriptionId) > 0
                 THEN CASE WHEN (SELECT COUNT(*)) >= su.NumLifts
                           THEN 2 --full
                           ELSE 1 --drives and has free places
                      END
                 ELSE 0 --undefined
            END
  END LiftState
  FROM ES_Subscriptions su
   INNER JOIN ES_Contacts co ON co.ContactId = su.ContactId
   INNER JOIN ES_SubscriptionStates ss ON ss.SubscriptionStateId = su.SubscriptionStateId
   INNER JOIN ES_Events ev ON ev.EventId = su.EventId
   INNER JOIN ES_Mandators m ON m.MandatorId = ev.MandatorId
   LEFT JOIN ES_LiftSubscriptions ls ON ls.SubscriptionId = su.SubscriptionId
  WHERE su.EventId = @EventId
    AND co.ContactId = @ContactId

 IF @@ROWCOUNT=0
  RETURN 10
 ELSE
  RETURN 0



' 
END
GO
SET ANSI_NULLS OFF
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ES_ListEventCategories]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'


-- gibt alle EventCategories für einen Mandanten aus der tab ES_EventCategories zurück
-- Returns:
-- 0 = ok
CREATE PROCEDURE [dbo].[ES_ListEventCategories]
 @MandatorId CHAR(32)
AS

 SELECT *
  FROM ES_EventCategories
  WHERE MandatorId = @MandatorId

  RETURN 0



' 
END
GO
SET ANSI_NULLS OFF
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ES_ListContactSettings]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'


-- listet alle ContactSettings eines Contacts auf
-- Returns:
-- 0 = ok
CREATE PROCEDURE [dbo].[ES_ListContactSettings]
 @MandatorId CHAR(32),
 @ContactId INT
AS

 SELECT
   cs.ContactSettingId,
   co.ContactId,
   ec.EventCategoryId,
   cs.NotifyBySMS,
   cs.NotifyByEmail,
   cs.SmsNotifSubscriptionsOn,
   cs.AutoNotifSubscription
  FROM ES_Contacts co
    CROSS JOIN ES_EventCategories ec
    LEFT JOIN ES_ContactSettings cs ON cs.ContactId = co.ContactId AND cs.EventCategoryId = ec.EventCategoryId
   WHERE co.ContactId = @ContactId AND ec.MandatorId = @MandatorId

 RETURN 0


' 
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ES_DelContact]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'


-- löscht einen Contact aus der ES_Contacts
-- Returns:
-- 0 = ok
-- 100 = fehler beim löschen
CREATE PROCEDURE [dbo].[ES_DelContact]
 @ContactId INT
AS

BEGIN TRANSACTION t1
  UPDATE ES_Contacts
   SET IsDeleted = 1
   WHERE ContactId = @ContactId
  IF @@ROWCOUNT=1 BEGIN
    COMMIT TRANSACTION t1
    RETURN 0
  END ELSE BEGIN
    ROLLBACK TRANSACTION t1
    RETURN 100
  END





' 
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ES_ListContacts]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'


-- listet alle Contacts eines Mandanten auf
-- Returns:
-- 0 = ok
CREATE PROCEDURE [dbo].[ES_ListContacts]
 @MandatorId CHAR(32)
AS

 SELECT *
  FROM ES_Contacts
  WHERE MandatorId = @MandatorId
    AND IsDeleted = 0

 RETURN 0



' 
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ES_AddContact]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'


-- fügt einen neuen kontakt in die tab ES_Contacts ein
-- Returns:
-- 0 = ok
-- 10 = ok (wiederhergestellt)
-- 100 = existiert bereits auf diesem Mandanten
-- 101 = fehler beim insert
-- 102 = fehler beim update (wiederherstellen)
CREATE PROCEDURE [dbo].[ES_AddContact] 
 @MandatorId CHAR(32),
 @Name VARCHAR(50),
 @Email VARCHAR(50),
 @MobilePhone VARCHAR(20),
 @LiftMgmtSmsOn BIT,
 @EventMgmtSmsOn BIT,
 @SmsLog INT,
 @SmsPurchased INT
AS

IF NOT EXISTS(SELECT * FROM ES_Contacts WHERE Email = @Email AND MandatorId = @MandatorId AND IsDeleted = 0) BEGIN
 IF EXISTS (SELECT * FROM ES_Contacts WHERE Email = @Email AND MandatorId = @MandatorId AND IsDeleted = 1) BEGIN
  -- gelöschte version dieses Contacts besteht bereits --> wiederherstellen
  UPDATE ES_Contacts
   SET IsDeleted = 0
   WHERE Email = @Email
     AND MandatorId = @MandatorId
  IF @@ROWCOUNT=1 BEGIN
    SELECT TOP 1 ContactId FROM ES_Contacts WHERE Email = @Email AND MandatorId = @MandatorId
    RETURN 10
  END ELSE BEGIN
    RETURN 102
  END
 END ELSE BEGIN
  -- keine version gefunden --> neu erstellen
  INSERT ES_Contacts(MandatorId, [Name], Email, MobilePhone, LiftMgmtSmsOn, EventMgmtSmsOn, UseTwoWaySms, SmsLog, SmsPurchased)
   SELECT @MandatorId, @Name, @Email, @MobilePhone, @LiftMgmtSmsOn, @EventMgmtSmsOn, 0 UseTwoWaySms, @SmsLog, @SmsPurchased
    WHERE @Email IS NOT NULL AND @Email <> ''''
      AND @Name IS NOT NULL AND @Name <> ''''
  IF @@ROWCOUNT=1 BEGIN
    DECLARE @contactId INT
    SELECT @contactId = @@IDENTITY
    INSERT ES_ContactsRoles(ContactId, RoleId)
     VALUES(@contactId, 2)
    SELECT @contactId
    RETURN 0
  END ELSE BEGIN
    RETURN 101
  END
 END
END ELSE BEGIN
  RETURN 100
END



' 
END
GO
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
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ES_EditContact]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'


-- ändert einen kontakt in der tab ES_Contacts
-- Returns:
-- 0 = ok
-- 100 = existiert bereits
-- 101 = contact nicht gefunden
-- 102 = fehler beim update
CREATE PROCEDURE [dbo].[ES_EditContact] 
 @ContactId INT,
 @Name VARCHAR(50),
 @Email VARCHAR(50),
 @MobilePhone VARCHAR(20),
 @LiftMgmtSmsOn BIT,
 @EventMgmtSmsOn BIT,
 @SmsPurchased INT,
 @NoSmsCreditNotified BIT
AS

DECLARE @MandatorId CHAR(32)
SELECT @MandatorId = MandatorId FROM ES_Contacts WHERE ContactId = @ContactId AND IsDeleted = 0
IF @@ROWCOUNT=0 BEGIN
  RETURN 101
END

IF NOT EXISTS(SELECT * FROM ES_Contacts WHERE Email = @Email AND ContactId <> @ContactId AND MandatorId = @MandatorId) BEGIN
  UPDATE ES_Contacts
  SET [Name]=@Name, Email=@Email, MobilePhone=@MobilePhone, LiftMgmtSmsOn=@LiftMgmtSmsOn, EventMgmtSmsOn=@EventMgmtSmsOn, SmsPurchased=@SmsPurchased,  NoSmsCreditNotified=@NoSmsCreditNotified
    WHERE ContactId = @ContactId
      AND IsDeleted = 0
      AND @Email IS NOT NULL AND @Email <> ''''
      AND @Name IS NOT NULL AND @Name <> ''''
  IF @@ROWCOUNT=1 BEGIN
    RETURN 0
  END ELSE
    RETURN 102
END ELSE BEGIN
  RETURN 100
END

' 
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ES_GetContact]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'


-- gibt die gewünschten contact daten aus der tab ES_Contacts zurück
-- Returns:
-- 0 = ok
-- 100 = weder email noch contactid als parameter übergeben
-- 101 = Kontakt wurde nicht gefunden
CREATE PROCEDURE [dbo].[ES_GetContact]
 @MandatorId CHAR(32),
 @Email VARCHAR(50) = NULL,
 @ContactId INT = NULL
AS

IF @ContactId IS NOT NULL BEGIN
 SELECT *
  FROM ES_Contacts
   WHERE ContactId = @ContactId
     AND MandatorId = @MandatorId
--     AND IsDeleted = 0
 IF @@ROWCOUNT < 1
  RETURN 101
 ELSE
  RETURN 0
END

IF @Email IS NOT NULL BEGIN
 SELECT *
  FROM ES_Contacts
   WHERE Email = @Email
     AND MandatorId = @MandatorId
--     AND IsDeleted = 0
 IF @@ROWCOUNT < 1
  RETURN 101
 ELSE
  RETURN 0
END

RETURN 100



' 
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ES_LogSms]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'


-- logged ein gesendetes sms in der tab ES_Contacts
-- Returns:
-- 0 = ok
-- 100 = kontakt nicht gefunden
-- 101 = fehler beim loggen
CREATE PROCEDURE [dbo].[ES_LogSms]
 @ContactId INT
AS

  DECLARE @MandatorId CHAR(32)
  SELECT @MandatorId = MandatorId FROM ES_Contacts WHERE ContactId = @ContactId
  IF @@ROWCOUNT=0 BEGIN
    RETURN 100
  END

  UPDATE ES_Contacts
  SET SmsLog = SmsLog + 1
    WHERE ContactId = @ContactId
  IF @@ROWCOUNT=1 BEGIN
    RETURN 0
  END ELSE
    RETURN 101




' 
END
GO
SET ANSI_NULLS OFF
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ES_GetContactsRoles]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'


-- gibt die zugewiesenen Rollen aus der tab ES_ContactsRoles zurück
-- Returns:
CREATE PROCEDURE [dbo].[ES_GetContactsRoles]
 @ContactId INT
AS

 SELECT cr.*, ro.Role
  FROM ES_ContactsRoles cr
   INNER JOIN ES_Roles ro ON ro.RoleId = cr.RoleId
   WHERE cr.ContactId = @ContactId

 RETURN 0



' 
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ES_AddSubscription]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'


-- fügt eine Anmeldung in die tab ES_Subscriptions ein
-- Returns:
-- 0 = ok
-- 100 = an diesem tag existiert schon eine anmeldung dieses contacts für diesen event
-- 101 = fehler beim insert
CREATE PROCEDURE [dbo].[ES_AddSubscription]
 @EventId INT,
 @ContactId INT,
 @SubscriptionStateId INT,
 @SubscriptionTime VARCHAR(50),
 @Comment VARCHAR(8000),
 @NumLifts INT
AS

IF NOT EXISTS(SELECT *
               FROM ES_Subscriptions
                WHERE ContactId = @ContactId
                  AND EventId = @EventId) BEGIN
  INSERT ES_Subscriptions(EventId, ContactId, SubscriptionStateId, SubscriptionTime, Comment, NumLifts)
   VALUES(@EventId, @ContactId, @SubscriptionStateId, @SubscriptionTime, @Comment, @NumLifts)
  IF @@ROWCOUNT=1 BEGIN
    SELECT @@IDENTITY
    RETURN 0
  END ELSE
    RETURN 101
END ELSE BEGIN
  RETURN 100
END



' 
END
GO
SET ANSI_NULLS OFF
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ES_ListContactsJourneys]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'


-- listet alle bestehenden Routen eines Kontaktes auf
-- Returns:
-- 0 = ok
CREATE PROCEDURE [dbo].[ES_ListContactsJourneys]
 @ContactId INT
AS

SELECT js.SubscriptionId, su.ContactId, js.Station, js.StationTime, js.SortOrder
 FROM ES_JourneyStations js
  INNER JOIN ES_Subscriptions su ON su.SubscriptionId = js.SubscriptionId
 WHERE su.ContactId = @ContactId
 ORDER BY js.SubscriptionId, js.SortOrder

 RETURN 0



' 
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ES_EditSubscription]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'


-- ändert eine Anmeldung in der tab ES_Subscriptions
-- Returns:
-- 0 = ok
-- 100 = event nicht gefunden
-- 101 = fehler beim ändern der bestehenden liftSubscription
-- 102 = fehler beim hinzufügen einer neuen liftSubscription
-- 103 = fehler beim update
CREATE PROCEDURE [dbo].[ES_EditSubscription]
 @SubscriptionId INT,
 @SubscriptionStateId INT,
 @SubscriptionTime VARCHAR(50),
 @Comment VARCHAR(8000),
 @NumLifts INT,
 @LiftSubscriptionJourneyStationId INT
AS

IF EXISTS(SELECT *
               FROM ES_Subscriptions
                WHERE SubscriptionId = @SubscriptionId) BEGIN
 BEGIN TRANSACTION t1
  UPDATE ES_Subscriptions
   SET
    SubscriptionTime = @SubscriptionTime,
		SubscriptionStateId = @SubscriptionStateId,
    Comment = @Comment,
    NumLifts = @NumLifts
   WHERE SubscriptionId = @SubscriptionId
  IF @@ROWCOUNT=1 BEGIN
-- case 1 und 2: keine liftSubsc oder liftSubsc löschen
    IF @LiftSubscriptionJourneyStationId IS NULL BEGIN
      DELETE ES_LiftSubscriptions
       WHERE SubscriptionId = @SubscriptionId
    END ELSE BEGIN
-- case 3: bestehende liftSubsc ändern
      IF EXISTS(SELECT * FROM ES_LiftSubscriptions WHERE SubscriptionId = @SubscriptionId AND JourneyStationId <> @LiftSubscriptionJourneyStationId) BEGIN
        UPDATE ES_LiftSubscriptions
         SET JourneyStationId = @LiftSubscriptionJourneyStationId
         WHERE SubscriptionId = @SubscriptionId
        IF @@ROWCOUNT<>1 BEGIN
          ROLLBACK TRANSACTION t1
          RETURN 101
        End
      END
-- case 4: neue liftSubsc hinzufügen
      IF NOT EXISTS(SELECT * FROM ES_LiftSubscriptions WHERE SubscriptionId = @SubscriptionId AND JourneyStationId = @LiftSubscriptionJourneyStationId) BEGIN
        INSERT ES_LiftSubscriptions(SubscriptionId, JourneyStationId)
         SELECT @SubscriptionId, @LiftSubscriptionJourneyStationId
        IF @@ROWCOUNT<>1 BEGIN
          ROLLBACK TRANSACTION t1
          RETURN 102
        End
      END
-- case 5: bestehende liftSubsc so belassen wie bisher --> erfordert keine aktion
    END
    COMMIT TRANSACTION t1
    RETURN 0
  END ELSE BEGIN
    ROLLBACK TRANSACTION t1
    RETURN 103
  END
END ELSE BEGIN
  RETURN 100
END



' 
END
GO
SET ANSI_NULLS OFF
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ES_GetNumDefinedLifts]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'


-- gibt zurück wieviele Mitfahrten auf einer Anmeldung oder einem Routenpunkt definiert sind
-- Returns:
-- 0 = ok
-- 100 = subscription nicht gefunden
-- 101 = journeystation nicht gefunden
CREATE PROCEDURE [dbo].[ES_GetNumDefinedLifts]
 @SubscriptionId INT = NULL,
 @JourneyStationId INT = NULL
AS

IF @SubscriptionId IS NOT NULL BEGIN
 IF EXISTS(SELECT *
               FROM ES_Subscriptions
                WHERE SubscriptionId = @SubscriptionId) BEGIN
  SELECT COUNT(*)
   FROM ES_Subscriptions su
    INNER JOIN ES_JourneyStations js ON js.SubscriptionId = su.SubscriptionId
    INNER JOIN ES_LiftSubscriptions ls ON ls.JourneyStationId = js.JourneyStationId
   WHERE su.SubscriptionId = @SubscriptionId

 END ELSE BEGIN
  RETURN 100
 END
END

IF @JourneyStationId IS NOT NULL BEGIN
 IF EXISTS(SELECT *
               FROM ES_JourneyStations
                WHERE JourneyStationId = @JourneyStationId) BEGIN
  SELECT COUNT(*)
   FROM ES_JourneyStations js
    INNER JOIN ES_LiftSubscriptions ls ON ls.JourneyStationId = js.JourneyStationId
   WHERE js.JourneyStationId = @JourneyStationId

 END ELSE BEGIN
  RETURN 101
 END
END



' 
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ES_AddJourneyStation]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'


-- fügt eine neue JourneyStation in eine Route ein (tab ES_JourneyStations)
-- Returns:
-- 0 = ok
-- 100 = fehler beim insert
CREATE PROCEDURE [dbo].[ES_AddJourneyStation]
 @SubscriptionId INT,
 @Station VARCHAR(100),
 @StationTime VARCHAR(50),
 @SortOrder INT
AS

  INSERT ES_JourneyStations(SubscriptionId, Station, StationTime, SortOrder)
   SELECT @SubscriptionId, @Station, @StationTime, @SortOrder
    WHERE @Station IS NOT NULL AND @Station <> ''''
  IF @@ROWCOUNT=1 BEGIN
    SELECT @@IDENTITY
    RETURN 0
  END ELSE
    RETURN 100



' 
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ES_DelJourneyStation]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'


-- löscht eine JourneyStation aus einer Route (tab ES_JourneyStations)
-- Returns:
-- 0 = ok
-- 100 = fehler beim löschen
CREATE PROCEDURE [dbo].[ES_DelJourneyStation] 
 @JourneyStationId INT
AS

BEGIN TRANSACTION t1
  DELETE ES_JourneyStations
   WHERE JourneyStationId = @JourneyStationId
  IF @@ROWCOUNT=1 BEGIN
    COMMIT TRANSACTION t1
    RETURN 0
  END ELSE BEGIN
    ROLLBACK TRANSACTION t1
    RETURN 100
  END



' 
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ES_EditJourneyStation]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'


-- ändert eine bestehende JourneyStation auf einer Route (tab ES_JourneyStations)
-- Returns:
-- 0 = ok
-- 100 = fehler beim update
CREATE PROCEDURE [dbo].[ES_EditJourneyStation]
 @JourneyStationId INT,
 @Station VARCHAR(100),
 @StationTime VARCHAR(50),
 @SortOrder INT
AS

  UPDATE ES_JourneyStations
   SET Station=@Station , StationTime=@StationTime , SortOrder=@SortOrder
    WHERE JourneyStationId=@JourneyStationId
      AND @Station IS NOT NULL AND @Station <> ''''
  IF @@ROWCOUNT=1 BEGIN
    RETURN 0
  END ELSE
    RETURN 100



' 
END
GO
SET ANSI_NULLS OFF
GO
SET QUOTED_IDENTIFIER OFF
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ES_ListJourneyStations]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'


-- listet alle RoutenPunkte einer Anmeldung auf
-- Returns:
-- 0 = ok
CREATE PROCEDURE [dbo].[ES_ListJourneyStations]
 @SubscriptionId INT
AS

 SELECT
  js.*
  FROM ES_JourneyStations js
  WHERE js.SubscriptionId = @SubscriptionId

 RETURN 0



' 
END
GO
SET ANSI_NULLS OFF
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ES_EditContactSetting]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'


-- ändert ein setting in der tab ES_ContactSettings
-- Returns:
-- 0 = ok
-- 101 = setting nicht gefunden
-- 102 = fehler beim update
CREATE PROCEDURE [dbo].[ES_EditContactSetting]
 @ContactSettingId INT,
 @NotifyBySMS BIT,
 @NotifyByEmail BIT,
 @SmsNotifSubscriptionsOn BIT,
 @AutoNotifSubscription INT
AS

IF EXISTS(SELECT * FROM ES_ContactSettings WHERE ContactSettingId = @ContactSettingId) BEGIN
  UPDATE ES_ContactSettings
   SET NotifyBySMS = @NotifyBySMS, NotifyByEmail = @NotifyByEmail, SmsNotifSubscriptionsOn = @SmsNotifSubscriptionsOn, AutoNotifSubscription = @AutoNotifSubscription
    WHERE ContactSettingId = @ContactSettingId
  IF @@ROWCOUNT=1 BEGIN
    RETURN 0
  END ELSE
    RETURN 102
END ELSE BEGIN
  RETURN 101
END


' 
END
GO
SET ANSI_NULLS OFF
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ES_AddContactSetting]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'


-- fügt ein neues Contact Setting ein (tab ES_ContactSettings)
-- Returns:
-- 0 = ok
-- 100 = fehler beim insert
CREATE PROCEDURE [dbo].[ES_AddContactSetting]
 @ContactId INT,
 @EventCategoryId INT,
 @NotifyBySMS BIT,
 @NotifyByEmail BIT,
 @SmsNotifSubscriptionsOn BIT,
 @AutoNotifSubscription INT
AS

  INSERT ES_ContactSettings(ContactId, EventCategoryId, NotifyBySMS, NotifyByEmail, SmsNotifSubscriptionsOn, AutoNotifSubscription)
   SELECT @ContactId, @EventCategoryId, @NotifyBySMS, @NotifyByEmail, @SmsNotifSubscriptionsOn, @AutoNotifSubscription
  IF @@ROWCOUNT=1 BEGIN
    SELECT @@IDENTITY
    RETURN 0
  END ELSE
    RETURN 100


' 
END
GO
SET ANSI_NULLS OFF
GO
SET QUOTED_IDENTIFIER OFF
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ES_AddNotifSubscription]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'


-- fügt eine Abonnierung in die tab ES_NotifSubscriptions ein
-- Returns:
-- 0 = ok
-- 100 = für diesen Event besteht schon ein Abo dieses contacts
-- 101 = fehler beim insert
CREATE PROCEDURE [dbo].[ES_AddNotifSubscription]
 @EventId INT,
 @ContactId INT,
 @NumSmsNotifications INT
AS

IF NOT EXISTS(SELECT *
               FROM ES_NotifSubscriptions
                WHERE ContactId = @ContactId
                  AND EventId = @EventId) BEGIN
  INSERT ES_NotifSubscriptions(EventId, ContactId, NumSmsNotifications)
   VALUES(@EventId, @ContactId, @NumSmsNotifications)
  IF @@ROWCOUNT=1 BEGIN
    SELECT @@IDENTITY
    RETURN 0
  END ELSE
    RETURN 101
END ELSE BEGIN
  RETURN 100
END



' 
END
GO
SET ANSI_NULLS OFF
GO
SET QUOTED_IDENTIFIER OFF
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ES_DelNotifSubscription]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'


-- ändert eine Abonnierung in der tab ES_NotifSubscriptions
CREATE PROCEDURE [dbo].[ES_DelNotifSubscription]
 @NotifSubscriptionId INT
AS

  DELETE
    FROM ES_NotifSubscriptions
    WHERE NotifSubscriptionId = @NotifSubscriptionId

  RETURN 0



' 
END
GO
SET ANSI_NULLS OFF
GO
SET QUOTED_IDENTIFIER OFF
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ES_EditNotifSubscription]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'


-- ändert eine Abonnierung in der tab ES_NotifSubscriptions
-- Returns:
-- 0 = ok
-- 100 = abonnierung nicht gefunden
-- 101 = fehler beim update
CREATE PROCEDURE [dbo].[ES_EditNotifSubscription]
 @NotifSubscriptionId INT,
 @NumSmsNotifications INT
AS

IF EXISTS(SELECT *
               FROM ES_NotifSubscriptions
                WHERE NotifSubscriptionId = @NotifSubscriptionId) BEGIN
  UPDATE ES_NotifSubscriptions
    SET NumSmsNotifications = @NumSmsNotifications
    WHERE NotifSubscriptionId = @NotifSubscriptionId
  IF @@ROWCOUNT=1 BEGIN
    SELECT @@IDENTITY
    RETURN 0
  END ELSE
    RETURN 101
END ELSE BEGIN
  RETURN 100
END



' 
END
GO
SET ANSI_NULLS OFF
GO
SET QUOTED_IDENTIFIER OFF
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ES_ListNotifSubscriptions]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'


-- listet alle Notifizierungsabonnierungen eines Events auf
-- Returns:
-- 0 = ok
CREATE PROCEDURE [dbo].[ES_ListNotifSubscriptions]
 @EventId INT
AS

 SELECT
  ns.*
  FROM ES_NotifSubscriptions ns
   WHERE ns.EventId = @EventId

 RETURN 0



' 
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ES_EditLocation]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'


-- ändert eine Location in der tab ES_Locations
-- Returns:
-- 0 = ok
-- 100 = existiert bereits
-- 101 = location nicht gefunden
-- 102 = fehler beim update
CREATE PROCEDURE [dbo].[ES_EditLocation]
 @LocationId INT,
 @LocationText VARCHAR(50),
 @LocationShort VARCHAR(15),
 @LocationDescription VARCHAR(200)
AS

DECLARE @EventCategoryId INT
SELECT @EventCategoryId = EventCategoryId FROM ES_Locations WHERE LocationId = @LocationId AND IsDeleted = 0
IF @@ROWCOUNT=0 BEGIN
  RETURN 101
END

IF NOT EXISTS(SELECT * FROM ES_Locations WHERE LocationText = @LocationText AND LocationId <> @LocationId AND EventCategoryId = @EventCategoryId) BEGIN
  UPDATE ES_Locations
   SET LocationText=@LocationText, LocationShort=@LocationShort, LocationDescription=@LocationDescription
    WHERE LocationId = @LocationId
      AND IsDeleted = 0
      AND @LocationText IS NOT NULL AND @LocationText <> ''''
      AND @LocationShort IS NOT NULL AND @LocationShort <> ''''
  IF @@ROWCOUNT=1 BEGIN
    RETURN 0
  END ELSE
    RETURN 102
END ELSE BEGIN
  RETURN 100
END



' 
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ES_ListLocations]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'


-- listet alle Locations eines Mandanten auf
-- Returns:
-- 0 = ok
CREATE PROCEDURE [dbo].[ES_ListLocations]
 @EventCategoryId INT
AS

 SELECT *
  FROM ES_Locations
  WHERE EventCategoryId = @EventCategoryId
    AND IsDeleted = 0
  ORDER BY LocationText

 RETURN 0



' 
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ES_DelLocation]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'


-- löscht eine Location aus der ES_Locations
-- Returns:
-- 0 = ok
-- 100 = fehler beim löschen
CREATE PROCEDURE [dbo].[ES_DelLocation]
 @LocationId INT
AS

BEGIN TRANSACTION t1
  UPDATE ES_Locations
   SET IsDeleted = 1
   WHERE LocationId = @LocationId
  IF @@ROWCOUNT=1 BEGIN
    COMMIT TRANSACTION t1
    RETURN 0
  END ELSE BEGIN
    ROLLBACK TRANSACTION t1
    RETURN 100
  END





' 
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ES_AddLocation]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'


-- fügt eine neue Location in die tab ES_Locations ein
-- Returns:
-- 0 = ok
-- 10 = ok (wiederhergestellt)
-- 100 = existiert bereits auf dieser Kategorie
-- 101 = fehler beim insert
-- 102 = fehler beim update (wiederherstellen)
CREATE PROCEDURE [dbo].[ES_AddLocation] 
 @EventCategoryId INT,
 @LocationText VARCHAR(50),
 @LocationShort VARCHAR(15),
 @LocationDescription VARCHAR(200)
AS

IF NOT EXISTS(SELECT * FROM ES_Locations WHERE LocationText = @LocationText AND EventCategoryId = @EventCategoryId AND IsDeleted = 0) BEGIN
 IF EXISTS (SELECT * FROM ES_Locations WHERE LocationText = @LocationText AND EventCategoryId = @EventCategoryId AND IsDeleted = 1) BEGIN
  -- gelöschte version dieser Location besteht bereits --> wiederherstellen
  UPDATE ES_Locations
   SET IsDeleted = 0
   WHERE LocationText = @LocationText
     AND EventCategoryId = @EventCategoryId
  IF @@ROWCOUNT=1 BEGIN
    SELECT TOP 1 LocationId FROM ES_Locations WHERE LocationText = @LocationText AND EventCategoryId = @EventCategoryId
    RETURN 10
  END ELSE BEGIN
    RETURN 102
  END
 END ELSE BEGIN
  -- keine version gefunden --> neu erstellen
  INSERT ES_Locations(EventCategoryId, LocationText, LocationShort, LocationDescription)
   SELECT @EventCategoryId, @LocationText, @LocationShort, @LocationDescription
    WHERE @LocationText IS NOT NULL AND @LocationText <> ''''
      AND @LocationShort IS NOT NULL AND @LocationShort <> ''''
  IF @@ROWCOUNT=1 BEGIN
    SELECT @@IDENTITY
    RETURN 0
  END ELSE BEGIN
    RETURN 101
  END
 END
END ELSE BEGIN
  RETURN 100
END



' 
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ES_ListSubscriptionStates]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'

-- listet alle SubscriptionStates einer Kategorie auf
-- Returns:
-- 0 = ok
CREATE PROCEDURE [dbo].[ES_ListSubscriptionStates]
 @EventCategoryId INT
AS

 SELECT *
  FROM ES_SubscriptionStates
  WHERE EventCategoryId = @EventCategoryId
   ORDER BY SubscriptionStateId

 RETURN 0

' 
END
GO
SET ANSI_NULLS OFF
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ES_Login]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'


-- Abhandlung des ganzen Login-Vorgangs. WICHTIG: CustomId darf nie NULL sein, auch nicht bei internem login
-- 0 = ok
-- 100 = fehler beim insert in ES_Log
-- 101 = zu oft fehlgeschlagen in letzten 30 min
-- 102 = zugriff verweigert
-- 103 = keine registrierung auf diesem mandanten (auf anderem mandanten registrierung vorhanden)
CREATE PROCEDURE [dbo].[ES_Login]
@UserIp VARCHAR(15),
@Login VARCHAR(50),
@Password VARCHAR(50),
@MandatorId CHAR(32),
@CustTable VARCHAR(50),
@CustIdCol VARCHAR(50),
@CustLoginCol VARCHAR(50),
@CustPwCol VARCHAR(50)

AS

DECLARE @Allowed TINYINT -- 0 = not allowed; 1 = allowed; 2 = allowed but on other mandator
DECLARE @Rowcount INT

  -- security-check
  IF (SELECT COUNT(*)
       FROM ES_Log
       WHERE UserIP = @UserIp
         AND Successful = 0
         AND DATEDIFF(MINUTE,[Date],GetDate())<30)>=10
   RETURN 101

  -- check login
  EXECUTE(
  ''SELECT co.*
    FROM ES_Contacts co
     INNER JOIN '' + @CustTable + '' cust ON cust.'' + @CustIdCol + '' = co.CustomId
   WHERE co.CustomId IS NOT NULL
     AND co.IsDeleted = 0
     AND co.MandatorId = '''''' + @MandatorId + ''''''
     AND cust.IsDeleted = 0
     AND cust.'' + @CustLoginCol + '' = '''''' + @Login + ''''''
     AND cust.'' + @CustPwCol + '' = '''''' + @Password + ''''''''
         )
  SET @Rowcount = @@ROWCOUNT
  IF @Rowcount = 1
   SET @Allowed = 1
  ELSE BEGIN
   EXECUTE(
  ''SELECT co.*
    FROM ES_Contacts co
     INNER JOIN '' + @CustTable + '' cust ON cust.'' + @CustIdCol + '' = co.CustomId
   WHERE co.CustomId IS NOT NULL
     AND co.IsDeleted = 0
     AND co.MandatorId <> '''''' + @MandatorId + ''''''
     AND cust.IsDeleted = 0
     AND cust.'' + @CustLoginCol + '' = '''''' + @Login + ''''''
     AND cust.'' + @CustPwCol + '' = '''''' + @Password + ''''''''
           )
   SET @Rowcount = @@ROWCOUNT
    print ''rowcount:''
    print @Rowcount
   IF @Rowcount > 0  BEGIN
    SET @Allowed = 2
   END ELSE BEGIN
    SET @Allowed = 0
   END
  END

  print ''allowed''
  print @allowed

  -- insert into tabLog
  INSERT INTO ES_Log (UserIP, MandatorId, [Date], [Login], [Password], Successful)
  SELECT @UserIp, @MandatorId, GetDate(), @Login, @Password, CASE @Allowed WHEN 1 THEN 1 ELSE 0 END
  IF @@ROWCOUNT = 1
   IF @Allowed = 1
    RETURN 0
   ELSE BEGIN
    IF @Allowed = 2 BEGIN
     RETURN 103
    END ELSE BEGIN
     RETURN 102
    END
  END ELSE
   RETURN 100



' 
END
GO
SET ANSI_NULLS OFF
GO
SET QUOTED_IDENTIFIER OFF
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ES_RemoteLogin]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'

-- Abhandlung des ganzen Remote-Login-Vorgangs
-- 0 = ok
-- 100 = fehler beim insert in ES_Log
-- 101 = zu oft fehlgeschlagen in letzten 30 min
-- 102 = id nicht gefunden
-- 104 = ungültige Parameter übergeben
CREATE PROCEDURE [dbo].[ES_RemoteLogin]
@UserIp VARCHAR(15),
@CustomUserId INT = NULL,
@ContactId INT = NULL,
@MandatorId CHAR(32),
@CustTable VARCHAR(50),
@CustIdCol VARCHAR(50),
@CustLoginCol VARCHAR(50),
@CustPwCol VARCHAR(50)

AS

DECLARE @Rowcount INT
DECLARE @CustomUserIdString VARCHAR(10)
DECLARE @ContactIdString VARCHAR(10)

SET @CustomUserIdString = CONVERT(VARCHAR(20), ISNULL(@CustomUserId, ''''))
SET @ContactIdString = CONVERT(VARCHAR(20), ISNULL(@ContactId, ''''))

  IF @CustomUserId IS NULL AND @ContactId IS NULL
   RETURN 104

  -- security-check
  IF (SELECT COUNT(*)
       FROM ES_Log
       WHERE UserIP = @UserIp
         AND Successful = 0
         AND DATEDIFF(MINUTE,[Date],GetDate())<30)>=10
   RETURN 101

  -- check login
  EXECUTE(
  ''SELECT co.*, cust.'' + @CustLoginCol + ''
    FROM ES_Contacts co
     INNER JOIN '' + @CustTable + '' cust ON cust.'' + @CustIdCol + '' = co.CustomId
   WHERE co.CustomId IS NOT NULL
     AND co.IsDeleted = 0
     AND co.MandatorId = '''''' + @MandatorId + ''''''
     AND cust.IsDeleted = 0
     AND (cust.'' + @CustIdCol + '' = '' + @CustomUserIdString + '' OR co.ContactId = '' + @ContactIdString + '')''
         )
  SET @Rowcount = @@ROWCOUNT


  DECLARE @Login VARCHAR(50)
  DECLARE @sql NVARCHAR(1000)

  SET @sql = ''SELECT @custLogin = cust.'' + @CustLoginCol + ''
    FROM ES_Contacts co
     INNER JOIN '' + @CustTable + '' cust ON cust.'' + @CustIdCol + '' = co.CustomId
   WHERE co.CustomId IS NOT NULL
     AND co.IsDeleted = 0
     AND co.MandatorId = '''''' + @MandatorId + ''''''
     AND cust.IsDeleted = 0
     AND (cust.'' + @CustIdCol + '' = '' + @CustomUserIdString + '' OR co.ContactId = '' + @ContactIdString + '')''

  EXEC sp_executesql @sql, N''@custLogin varchar(10) output'', @custLogin = @Login output
  -- select @Login -- @Login is assigned value of @custLogin.Scope

  -- insert into tabLog
  INSERT INTO ES_Log (UserIP, MandatorId, [Date], Login, [Password], Successful)
  SELECT @UserIp, @MandatorId, GetDate(), @Login, NULL, CASE @Rowcount WHEN 1 THEN 1 ELSE 0 END
  IF @@ROWCOUNT = 1 BEGIN
   IF @Rowcount = 1
    RETURN 0
   ELSE
    RETURN 102
  END ELSE
   RETURN 100

' 
END
GO
SET ANSI_NULLS OFF
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ES_LogMandatorSms]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'


-- logged ein gesendetes sms in der tab ES_Mandators
-- Returns:
-- 0 = ok
-- 101 = fehler beim loggen
CREATE PROCEDURE [dbo].[ES_LogMandatorSms]
 @MandatorId CHAR(32)
AS

  UPDATE ES_Mandators
  SET SmsLog = SmsLog + 1
    WHERE MandatorId = @MandatorId
  IF @@ROWCOUNT=1 BEGIN
    RETURN 0
  END ELSE
    RETURN 101



' 
END
GO
SET ANSI_NULLS OFF
GO
SET QUOTED_IDENTIFIER OFF
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ES_EditMandatorDefaultNotificationAddresses]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'


-- ändert die default event notification mail addresses eines Mandanten in der tab ES_Mandators
-- Returns:
-- 0 = ok
-- 100 = fehler beim update
CREATE PROCEDURE [dbo].[ES_EditMandatorDefaultNotificationAddresses]
 @MandatorId CHAR(32),
 @MailAddresses VARCHAR(800)
AS

 UPDATE ES_Mandators
  SET EventNotificationAddressesDefault = @MailAddresses
  WHERE MandatorId = @MandatorId

 IF @@ROWCOUNT = 1
  RETURN 0
 ELSE
  RETURN 100



' 
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ES_GetMandator]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'


-- gibt den gewünschten mandanten aus der tab ES_Mandators zurück
-- Returns:
-- 0 = ok
-- 100 = gewünschter mandant wurde nicht gefunden
CREATE PROCEDURE [dbo].[ES_GetMandator]
 @MandatorId CHAR(32)
AS

 SELECT *
  FROM ES_Mandators
  WHERE MandatorId = @MandatorId
 IF @@ROWCOUNT = 1
  RETURN 0
 ELSE
  RETURN 100



' 
END
GO

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ES_GetContactByMobileNumber]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'

-- gibt die gewünschten contact daten aus der tab ES_Contacts zurück
-- Returns:
-- 0 = ok
-- 101 = Kontakt wurde nicht gefunden
CREATE PROCEDURE [dbo].[ES_GetContactByMobileNumber]
 @MobileNumber VARCHAR(20),
 @MandatorId CHAR(32)
AS

 SELECT *
  FROM ES_Contacts
   WHERE MobilePhone = LTRIM(RTRIM(@MobileNumber))
     AND MandatorId = @MandatorId
     AND IsDeleted = 0
 IF @@ROWCOUNT < 1
  RETURN 101
 ELSE
  RETURN 0
'
END
GO

SET ANSI_NULLS OFF
GO
SET QUOTED_IDENTIFIER OFF
GO

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ES_EditMandatorAlertMailSettings]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'

-- ändert die alert mail settings eines Mandanten in der tab ES_Mandators
-- Returns:
-- 0 = ok
-- 100 = fehler beim update
CREATE PROCEDURE [dbo].[ES_EditMandatorAlertMailSettings]
 @MandatorId CHAR(32),
 @NoSmsCreditNotified BIT
AS

 UPDATE ES_Mandators
  SET NoSmsCreditNotified = @NoSmsCreditNotified
  WHERE MandatorId = @MandatorId

 IF @@ROWCOUNT = 1
  RETURN 0
 ELSE
  RETURN 100
'
END
GO

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ES_AddSimpleCache]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'

-- Fügt einen simple cache eintrag hinzu
-- Returns:
-- 0 = ok
CREATE PROCEDURE [dbo].[ES_AddSimpleCache]
 @CacheKey VARCHAR(255),
 @CacheVal VARCHAR(8000),
 @CacheDuration INT
AS

DECLARE @CurrentDate DATETIME
SELECT @CurrentDate = GETDATE()

--delete all expired items
 DELETE FROM ES_SimpleCache
  WHERE ExpiryDate < @CurrentDate OR [Key] = @CacheKey
	
--insert new cache item
 INSERT ES_SimpleCache
  VALUES (@CacheKey, @CacheVal, DATEADD(minute, @CacheDuration, @CurrentDate)) 

 RETURN 0
'
END
GO

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ES_GetSimpleCache]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'

-- Gibt einen simple cache eintrag zurück
-- Returns:
-- 0 = ok, 1 = not found
CREATE PROCEDURE [dbo].[ES_GetSimpleCache]
 @CacheKey VARCHAR(255)
AS

DECLARE @CurrentDate DATETIME
SELECT @CurrentDate = GETDATE()

--delete all expired items
 DELETE FROM ES_SimpleCache
  WHERE ExpiryDate < @CurrentDate
	
--insert new cache item
 SELECT TOP 1 [Value]
  FROM ES_SimpleCache
   WHERE [Key] = @CacheKey
	 
 IF @@ROWCOUNT < 1
  RETURN 1
 ELSE
  RETURN 0
'
END
GO


GRANT EXECUTE ON [dbo].[ES_ListEvents] TO [EventSiteUsers]
GO
GRANT EXECUTE ON [dbo].[ES_ListEventsByCategory] TO [EventSiteUsers]
GO
GRANT EXECUTE ON [dbo].[ES_GetEvent] TO [EventSiteUsers]
GO
GRANT EXECUTE ON [dbo].[ES_EditEvent] TO [EventSiteUsers]
GO
GRANT EXECUTE ON [dbo].[ES_AddEvent] TO [EventSiteUsers]
GO
GRANT EXECUTE ON [dbo].[ES_GetSubscription] TO [EventSiteUsers]
GO
GRANT EXECUTE ON [dbo].[ES_ListSubscriptions] TO [EventSiteUsers]
GO
GRANT EXECUTE ON [dbo].[ES_GetSubscriptionFromContact] TO [EventSiteUsers]
GO
GRANT EXECUTE ON [dbo].[ES_ListEventCategories] TO [EventSiteUsers]
GO
GRANT EXECUTE ON [dbo].[ES_ListContactSettings] TO [EventSiteUsers]
GO
GRANT EXECUTE ON [dbo].[ES_DelContact] TO [EventSiteUsers]
GO
GRANT EXECUTE ON [dbo].[ES_ListContacts] TO [EventSiteUsers]
GO
GRANT EXECUTE ON [dbo].[ES_AddContact] TO [EventSiteUsers]
GO
GRANT EXECUTE ON [dbo].[ES_EditContact] TO [EventSiteUsers]
GO
GRANT EXECUTE ON [dbo].[ES_ChangePassword] TO [EventSiteUsers]
GO
GRANT EXECUTE ON [dbo].[ES_GetContactLogin] TO [EventSiteUsers]
GO
GRANT EXECUTE ON [dbo].[ES_GetContact] TO [EventSiteUsers]
GO
GRANT EXECUTE ON [dbo].[ES_LogSms] TO [EventSiteUsers]
GO
GRANT EXECUTE ON [dbo].[ES_GetContactsRoles] TO [EventSiteUsers]
GO
GRANT EXECUTE ON [dbo].[ES_AddSubscription] TO [EventSiteUsers]
GO
GRANT EXECUTE ON [dbo].[ES_ListContactsJourneys] TO [EventSiteUsers]
GO
GRANT EXECUTE ON [dbo].[ES_EditSubscription] TO [EventSiteUsers]
GO
GRANT EXECUTE ON [dbo].[ES_GetNumDefinedLifts] TO [EventSiteUsers]
GO
GRANT EXECUTE ON [dbo].[ES_AddJourneyStation] TO [EventSiteUsers]
GO
GRANT EXECUTE ON [dbo].[ES_DelJourneyStation] TO [EventSiteUsers]
GO
GRANT EXECUTE ON [dbo].[ES_EditJourneyStation] TO [EventSiteUsers]
GO
GRANT EXECUTE ON [dbo].[ES_ListJourneyStations] TO [EventSiteUsers]
GO
GRANT EXECUTE ON [dbo].[ES_EditContactSetting] TO [EventSiteUsers]
GO
GRANT EXECUTE ON [dbo].[ES_AddContactSetting] TO [EventSiteUsers]
GO
GRANT EXECUTE ON [dbo].[ES_AddNotifSubscription] TO [EventSiteUsers]
GO
GRANT EXECUTE ON [dbo].[ES_DelNotifSubscription] TO [EventSiteUsers]
GO
GRANT EXECUTE ON [dbo].[ES_EditNotifSubscription] TO [EventSiteUsers]
GO
GRANT EXECUTE ON [dbo].[ES_ListNotifSubscriptions] TO [EventSiteUsers]
GO
GRANT EXECUTE ON [dbo].[ES_EditLocation] TO [EventSiteUsers]
GO
GRANT EXECUTE ON [dbo].[ES_ListLocations] TO [EventSiteUsers]
GO
GRANT EXECUTE ON [dbo].[ES_DelLocation] TO [EventSiteUsers]
GO
GRANT EXECUTE ON [dbo].[ES_AddLocation] TO [EventSiteUsers]
GO
GRANT EXECUTE ON [dbo].[ES_ListSubscriptionStates] TO [EventSiteUsers]
GO
GRANT EXECUTE ON [dbo].[ES_Login] TO [EventSiteUsers]
GO
GRANT EXECUTE ON [dbo].[ES_RemoteLogin] TO [EventSiteUsers]
GO
GRANT EXECUTE ON [dbo].[ES_LogMandatorSms] TO [EventSiteUsers]
GO
GRANT EXECUTE ON [dbo].[ES_EditMandatorDefaultNotificationAddresses] TO [EventSiteUsers]
GO
GRANT EXECUTE ON [dbo].[ES_GetMandator] TO [EventSiteUsers]
GO
GRANT EXECUTE ON [dbo].[ES_GetContactByMobileNumber] TO [EventSiteUsers]
GO
GRANT EXECUTE ON [dbo].[ES_EditMandatorAlertMailSettings] TO [EventSiteUsers]
GO
GRANT EXECUTE ON [dbo].[ES_AddSimpleCache] TO [EventSiteUsers]
GO
GRANT EXECUTE ON [dbo].[ES_GetSimpleCache] TO [EventSiteUsers]
GO

