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