--This script will run on every deployment and will re-created the view

DROP VIEW IF EXISTS [Data].[PersonName]
GO

CREATE VIEW [Data].[PersonName]
AS
SELECT Name FROM [Data].[Person]
GO