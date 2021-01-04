-- This post deployment script will run exactly once. Add more scripts to PostDeploymentScripts folder to perform post deployment steps prior to each deployment

INSERT INTO [Data].[Person] (Name)
VALUES 
	('Clark Kent')
	,('Bruce Wayne')
	,('Diana Prince')
	,('Peter Parker')