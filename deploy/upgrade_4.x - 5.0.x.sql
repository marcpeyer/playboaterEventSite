USE [EventSite]
GO
/****** Object:  StoredProcedure [dbo].[ES_ListContacts]    Script Date: 20.12.2016 00:47:52 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO



-- listet alle Mandanten auf
-- Returns:
-- 0 = ok
CREATE PROCEDURE [dbo].[ES_ListMandators]
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	 SELECT *
	  FROM ES_Mandators

	RETURN 0

END
GO
GRANT EXECUTE ON [dbo].[ES_ListMandators] TO [EventSiteUsers]
GO


-- gibt das gewünschte Passwort aus der tab ES_Contacts zurück
-- Returns:
-- 0 = ok
-- 100 = weder email noch contactid als parameter übergeben
-- 101 = Kontakt wurde nicht gefunden
CREATE PROCEDURE [dbo].[ES_GetPassword]
 @MandatorId CHAR(32),
 @Email VARCHAR(50) = NULL,
 @ContactId INT = NULL
AS

IF @ContactId IS NOT NULL BEGIN
 SELECT [Password]
  FROM ES_Contacts
   WHERE ContactId = @ContactId
     AND MandatorId = @MandatorId
     AND IsDeleted = 0
 IF @@ROWCOUNT < 1
  RETURN 101
 ELSE
  RETURN 0
END

IF @Email IS NOT NULL BEGIN
 SELECT [Password]
  FROM ES_Contacts
   WHERE Email = @Email
     AND MandatorId = @MandatorId
     AND IsDeleted = 0
 IF @@ROWCOUNT < 1
  RETURN 101
 ELSE
  RETURN 0
END

RETURN 100

GO
GRANT EXECUTE ON [dbo].[ES_GetPassword] TO [EventSiteUsers]
GO
