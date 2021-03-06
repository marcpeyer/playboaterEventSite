/* ------------------------------------------------------------

   DESCRIPTION:  Schema Synchronization Script for Object(s) 

    tables:
        [dbo].[ES_Contacts], [dbo].[ES_ContactSettings], [dbo].[ES_ContactsRoles], [dbo].[ES_LiftSubscriptions], [dbo].[ES_Mandators], [dbo].[ES_SimpleCache], [dbo].[ES_Subscriptions]

     Make DEATHSTAR\SQL2K5.kcm_live Equal DEATHSTAR\SQL2K5.EventSite

   AUTHOR:	[Insert Author Name]

   DATE:	22.12.2008 10:52:16

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
ALTER TABLE dbo.ES_Contacts
	DROP CONSTRAINT FK_ES_Contacts_ES_Mandators
GO

IF @@ERROR<>0 OR @@TRANCOUNT=0 BEGIN IF @@TRANCOUNT>0 ROLLBACK SET NOEXEC ON END
GO
ALTER TABLE dbo.ES_Contacts
	DROP CONSTRAINT DF_ES_Contacts_IsDeleted
GO

IF @@ERROR<>0 OR @@TRANCOUNT=0 BEGIN IF @@TRANCOUNT>0 ROLLBACK SET NOEXEC ON END
GO
ALTER TABLE dbo.ES_Contacts
	DROP CONSTRAINT DF_ES_Contacts_NoSmsCreditNotified
GO

IF @@ERROR<>0 OR @@TRANCOUNT=0 BEGIN IF @@TRANCOUNT>0 ROLLBACK SET NOEXEC ON END
GO
CREATE TABLE dbo.Tmp_ES_Contacts
	(
	ContactId int NOT NULL IDENTITY (1, 1),
	CustomId int NULL,
	MandatorId char(32) NOT NULL,
	Name varchar(50) NOT NULL,
	Email varchar(50) NOT NULL,
	MobilePhone varchar(20) NULL,
	LiftMgmtSmsOn bit NOT NULL,
	EventMgmtSmsOn bit NOT NULL,
	SmsLog int NOT NULL,
	SmsPurchased int NOT NULL,
	IsDeleted bit NOT NULL,
	NoSmsCreditNotified bit NOT NULL
	)  ON [PRIMARY]
GO

IF @@ERROR<>0 OR @@TRANCOUNT=0 BEGIN IF @@TRANCOUNT>0 ROLLBACK SET NOEXEC ON END
GO
GRANT SELECT ON dbo.Tmp_ES_Contacts TO EventSiteUsers  AS dbo
GO

IF @@ERROR<>0 OR @@TRANCOUNT=0 BEGIN IF @@TRANCOUNT>0 ROLLBACK SET NOEXEC ON END
GO
ALTER TABLE dbo.Tmp_ES_Contacts ADD CONSTRAINT
	DF_ES_Contacts_IsDeleted DEFAULT ((0)) FOR IsDeleted
GO

IF @@ERROR<>0 OR @@TRANCOUNT=0 BEGIN IF @@TRANCOUNT>0 ROLLBACK SET NOEXEC ON END
GO
ALTER TABLE dbo.Tmp_ES_Contacts ADD CONSTRAINT
	DF_ES_Contacts_NoSmsCreditNotified DEFAULT ((0)) FOR NoSmsCreditNotified
GO

IF @@ERROR<>0 OR @@TRANCOUNT=0 BEGIN IF @@TRANCOUNT>0 ROLLBACK SET NOEXEC ON END
GO
SET IDENTITY_INSERT dbo.Tmp_ES_Contacts ON
GO
IF EXISTS(SELECT * FROM dbo.ES_Contacts)
	 EXEC('INSERT INTO dbo.Tmp_ES_Contacts (ContactId, CustomId, MandatorId, Name, Email, MobilePhone, LiftMgmtSmsOn, EventMgmtSmsOn, SmsLog, SmsPurchased, IsDeleted, NoSmsCreditNotified)
		SELECT ContactId, CustomId, MandatorId, Name, Email, MobilePhone, LiftMgmtSmsOn, CASE WHEN MobilePhone IS NULL OR SmsPurchased <= SmsLog THEN 0 ELSE 1 END EventMgmtSmsOn, SmsLog, SmsPurchased, IsDeleted, NoSmsCreditNotified FROM dbo.ES_Contacts WITH (HOLDLOCK TABLOCKX)')
GO

IF @@ERROR<>0 OR @@TRANCOUNT=0 BEGIN IF @@TRANCOUNT>0 ROLLBACK SET NOEXEC ON END
GO
SET IDENTITY_INSERT dbo.Tmp_ES_Contacts OFF
GO

IF @@ERROR<>0 OR @@TRANCOUNT=0 BEGIN IF @@TRANCOUNT>0 ROLLBACK SET NOEXEC ON END
GO
ALTER TABLE dbo.ES_SmsLocations
	DROP CONSTRAINT FK_ES_SmsLocations_ES_Contacts
GO

IF @@ERROR<>0 OR @@TRANCOUNT=0 BEGIN IF @@TRANCOUNT>0 ROLLBACK SET NOEXEC ON END
GO
ALTER TABLE dbo.ES_Events
	DROP CONSTRAINT FK_ES_Events_ES_Contacts
GO

IF @@ERROR<>0 OR @@TRANCOUNT=0 BEGIN IF @@TRANCOUNT>0 ROLLBACK SET NOEXEC ON END
GO
ALTER TABLE dbo.ES_ContactsRoles
	DROP CONSTRAINT FK_ES_ContactsRoles_ES_Contacts
GO

IF @@ERROR<>0 OR @@TRANCOUNT=0 BEGIN IF @@TRANCOUNT>0 ROLLBACK SET NOEXEC ON END
GO
ALTER TABLE dbo.ES_Subscriptions
	DROP CONSTRAINT FK_ES_Subscriptions_ES_Contacts
GO

IF @@ERROR<>0 OR @@TRANCOUNT=0 BEGIN IF @@TRANCOUNT>0 ROLLBACK SET NOEXEC ON END
GO
ALTER TABLE dbo.ES_ContactSettings
	DROP CONSTRAINT FK_ES_ContactSettings_ES_Contacts1
GO

IF @@ERROR<>0 OR @@TRANCOUNT=0 BEGIN IF @@TRANCOUNT>0 ROLLBACK SET NOEXEC ON END
GO
ALTER TABLE dbo.ES_NotifSubscriptions
	DROP CONSTRAINT FK_ES_NotifSubscriptions_ES_Contacts
GO

IF @@ERROR<>0 OR @@TRANCOUNT=0 BEGIN IF @@TRANCOUNT>0 ROLLBACK SET NOEXEC ON END
GO
DROP TABLE dbo.ES_Contacts
GO

IF @@ERROR<>0 OR @@TRANCOUNT=0 BEGIN IF @@TRANCOUNT>0 ROLLBACK SET NOEXEC ON END
GO
EXECUTE sp_rename N'dbo.Tmp_ES_Contacts', N'ES_Contacts', 'OBJECT' 
GO

IF @@ERROR<>0 OR @@TRANCOUNT=0 BEGIN IF @@TRANCOUNT>0 ROLLBACK SET NOEXEC ON END
GO
ALTER TABLE dbo.ES_Contacts ADD CONSTRAINT
	PK_ES_Contacts PRIMARY KEY CLUSTERED 
	(
	ContactId
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO

IF @@ERROR<>0 OR @@TRANCOUNT=0 BEGIN IF @@TRANCOUNT>0 ROLLBACK SET NOEXEC ON END
GO
CREATE UNIQUE NONCLUSTERED INDEX IX_Unique_ES_Contacts_Email_MandatorId ON dbo.ES_Contacts
	(
	Email,
	MandatorId
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO

IF @@ERROR<>0 OR @@TRANCOUNT=0 BEGIN IF @@TRANCOUNT>0 ROLLBACK SET NOEXEC ON END
GO
ALTER TABLE dbo.ES_Contacts WITH NOCHECK ADD CONSTRAINT
	FK_ES_Contacts_ES_Mandators FOREIGN KEY
	(
	MandatorId
	) REFERENCES dbo.ES_Mandators
	(
	MandatorId
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	
GO

IF @@ERROR<>0 OR @@TRANCOUNT=0 BEGIN IF @@TRANCOUNT>0 ROLLBACK SET NOEXEC ON END
GO
ALTER TABLE dbo.ES_NotifSubscriptions WITH NOCHECK ADD CONSTRAINT
	FK_ES_NotifSubscriptions_ES_Contacts FOREIGN KEY
	(
	ContactId
	) REFERENCES dbo.ES_Contacts
	(
	ContactId
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	
GO

IF @@ERROR<>0 OR @@TRANCOUNT=0 BEGIN IF @@TRANCOUNT>0 ROLLBACK SET NOEXEC ON END
GO
ALTER TABLE dbo.ES_ContactSettings WITH NOCHECK ADD CONSTRAINT
	FK_ES_ContactSettings_ES_Contacts FOREIGN KEY
	(
	ContactId
	) REFERENCES dbo.ES_Contacts
	(
	ContactId
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	
GO

IF @@ERROR<>0 OR @@TRANCOUNT=0 BEGIN IF @@TRANCOUNT>0 ROLLBACK SET NOEXEC ON END
GO
ALTER TABLE dbo.ES_Subscriptions WITH NOCHECK ADD CONSTRAINT
	FK_ES_Subscriptions_ES_Contacts FOREIGN KEY
	(
	ContactId
	) REFERENCES dbo.ES_Contacts
	(
	ContactId
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	
GO

IF @@ERROR<>0 OR @@TRANCOUNT=0 BEGIN IF @@TRANCOUNT>0 ROLLBACK SET NOEXEC ON END
GO
ALTER TABLE dbo.ES_ContactsRoles WITH NOCHECK ADD CONSTRAINT
	FK_ES_ContactsRoles_ES_Contacts FOREIGN KEY
	(
	ContactId
	) REFERENCES dbo.ES_Contacts
	(
	ContactId
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	
GO

IF @@ERROR<>0 OR @@TRANCOUNT=0 BEGIN IF @@TRANCOUNT>0 ROLLBACK SET NOEXEC ON END
GO
ALTER TABLE dbo.ES_Events WITH NOCHECK ADD CONSTRAINT
	FK_ES_Events_ES_Contacts FOREIGN KEY
	(
	EventCreatorId
	) REFERENCES dbo.ES_Contacts
	(
	ContactId
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	
GO

IF @@ERROR<>0 OR @@TRANCOUNT=0 BEGIN IF @@TRANCOUNT>0 ROLLBACK SET NOEXEC ON END
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

IF @@ERROR<>0 OR @@TRANCOUNT=0 BEGIN IF @@TRANCOUNT>0 ROLLBACK SET NOEXEC ON END
GO

EXEC dbo.sp_executesql @statement = N'

-- fügt einen neuen kontakt in die tab ES_Contacts ein
-- Returns:
-- 0 = ok
-- 10 = ok (wiederhergestellt)
-- 100 = existiert bereits auf diesem Mandanten
-- 101 = fehler beim insert
-- 102 = fehler beim update (wiederherstellen)
ALTER PROCEDURE [dbo].[ES_AddContact] 
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
  INSERT ES_Contacts(MandatorId, [Name], Email, MobilePhone, LiftMgmtSmsOn, EventMgmtSmsOn, SmsLog, SmsPurchased)
   SELECT @MandatorId, @Name, @Email, @MobilePhone, @LiftMgmtSmsOn, @EventMgmtSmsOn, @SmsLog, @SmsPurchased
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
GO

IF @@ERROR<>0 OR @@TRANCOUNT=0 BEGIN IF @@TRANCOUNT>0 ROLLBACK SET NOEXEC ON END
GO

EXEC dbo.sp_executesql @statement = N'

-- ändert einen kontakt in der tab ES_Contacts
-- Returns:
-- 0 = ok
-- 100 = existiert bereits
-- 101 = contact nicht gefunden
-- 102 = fehler beim update
ALTER PROCEDURE [dbo].[ES_EditContact] 
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
GO

IF @@ERROR<>0 OR @@TRANCOUNT=0 BEGIN IF @@TRANCOUNT>0 ROLLBACK SET NOEXEC ON END
GO

IF @@TRANCOUNT>0
	COMMIT

SET NOEXEC OFF

