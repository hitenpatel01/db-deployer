--This script will run exactly once. Add more scripts to Tables folder to make schema changes for the table
--Note: $PersonTableName$ is example to show how variables can be used in scripts

CREATE TABLE [Data].[$PersonTableName$](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](255) NOT NULL,
CONSTRAINT [PK_$PersonTableName$] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]