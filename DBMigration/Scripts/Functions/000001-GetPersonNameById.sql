--This script will run on every deployment and will re-created the proc

DROP FUNCTION IF EXISTS [Data].[GetPersonNameById]
GO

CREATE FUNCTION [Data].[GetPersonNameById]
(
	@Id INT
)
RETURNS NVARCHAR(255)
AS
BEGIN
	DECLARE @PersonName NVARCHAR(255)

	SELECT @PersonName = [Name] FROM [Data].[Person] WHERE Id = @Id;

	RETURN @PersonName;

END
GO