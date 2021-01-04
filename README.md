# db-migration
Database migration utility using DBUp

## Getting Started
- Replace .sql files in Scripts folder
- Add variables to dictionary in ```.WithVariables()``` statement
- Update connection string in appsettings.json

## Usage
- Run without parameters to perform database migration
- Options:
    - --dry-run : run without performing actual database migration to check for errors
    - --output : output migration script to file

## Customization
- Default target database is SQL Server. Replace ```dbup-sqlserver.dll``` with  ```dbup-postgresql.dll```, ```dbup-mysql.dll```, ```dbup-oracle.dll```, etc. from nuget based on target database
- Views, Functions and StoredProcedures are dropped and re-created with every migration. Update ```ScriptType``` to ```RunOnce``` on lines starting with ```.WithScriptsEmbeddedInAssembly()``` if execution plan or stats are important for such objects 
