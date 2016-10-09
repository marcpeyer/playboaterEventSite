IF NOT EXISTS (SELECT * FROM sys.database_principals WHERE name = N'EventSiteUsers' AND type = 'R')
CREATE ROLE [EventSiteUsers] AUTHORIZATION [dbo]
GO
IF NOT EXISTS (SELECT * FROM sys.database_principals WHERE name = N'KCMWebUsers' AND type = 'R')
CREATE ROLE [KCMWebUsers] AUTHORIZATION [dbo]
GO
