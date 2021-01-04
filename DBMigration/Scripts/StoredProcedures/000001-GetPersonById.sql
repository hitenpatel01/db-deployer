--This script will run on every deployment and will re-created the proc

DROP PROCEDURE IF EXISTS [Data].[GetPersonById]
GO

CREATE PROCEDURE [Data].[GetPersonById]
(@Id AS INT)
AS
SELECT * FROM Person WHERE Id = @Id
GO