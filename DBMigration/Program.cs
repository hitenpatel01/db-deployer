using System;
using System.Collections.Generic;
using System.Reflection;
using DbUp;
using DbUp.Engine;
using DbUp.Support;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.CommandLine.DragonFruit;
using System.IO;

namespace DBMigration
{
    class Program
    {
        public static void Main(bool dryRun = false, string output = null)
        {
            // Setup application configuration from appsettings.json and environment variables (optional)
            // TODO: update appsettings.json 
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();


            // Bind configuration to an object for ease of access
            var options = new DBMigrationOptions();
            configuration.GetSection(nameof(DBMigrationOptions))
                .Bind(options);


            // Setup simple console logging
            var logger = LoggerFactory.Create(builder =>
            {
                builder.AddSimpleConsole(options =>
                {
                    options.SingleLine = true;
                    options.TimestampFormat = "yyyy-MM-dd hh:mm:ss ";
                });
                //builder.AddConsole();
            }).CreateLogger<Program>();

            // This step will create database if it does not exists. Its advisable to create database before hand based on database best practices
            EnsureDatabase.For.SqlDatabase(options.ConnectionString);


            // Setup database migration builder
            var builder = DeployChanges.To

            // Perform database migration on SQL Server database. Add dbup-postgresql, dbup-mysql, dbup-oracle, etc. library based on database
            .SqlDatabase(options.ConnectionString)

            // Table to keep track of scripts that are already executed
            .JournalToSqlTable(options.JournalSchemaName, options.JournalTableName)

            // Run scripts in specific order and always run Views, Functions and StoredProcedures
            // TODO: If script execution plan or stats are important for such objects then change ScriptType to RunOnce
            .WithScriptsEmbeddedInAssembly(Assembly.GetExecutingAssembly(), options => options.StartsWith($"{nameof(DBMigration)}.Scripts.PreDeploymentScripts"), new SqlScriptOptions { RunGroupOrder = 10, ScriptType = ScriptType.RunOnce })
            .WithScriptsEmbeddedInAssembly(Assembly.GetExecutingAssembly(), options => options.StartsWith($"{nameof(DBMigration)}.Scripts.Schemas"), new SqlScriptOptions { RunGroupOrder = 20, ScriptType = ScriptType.RunOnce })
            .WithScriptsEmbeddedInAssembly(Assembly.GetExecutingAssembly(), options => options.StartsWith($"{nameof(DBMigration)}.Scripts.Tables"), new SqlScriptOptions { RunGroupOrder = 30, ScriptType = ScriptType.RunOnce })
            .WithScriptsEmbeddedInAssembly(Assembly.GetExecutingAssembly(), options => options.StartsWith($"{nameof(DBMigration)}.Scripts.Views"), new SqlScriptOptions { RunGroupOrder = 40, ScriptType = ScriptType.RunAlways })
            .WithScriptsEmbeddedInAssembly(Assembly.GetExecutingAssembly(), options => options.StartsWith($"{nameof(DBMigration)}.Scripts.Functions"), new SqlScriptOptions { RunGroupOrder = 50, ScriptType = ScriptType.RunAlways })
            .WithScriptsEmbeddedInAssembly(Assembly.GetExecutingAssembly(), options => options.StartsWith($"{nameof(DBMigration)}.Scripts.StoredProcedures"), new SqlScriptOptions { RunGroupOrder = 60, ScriptType = ScriptType.RunAlways })
            .WithScriptsEmbeddedInAssembly(Assembly.GetExecutingAssembly(), options => options.StartsWith($"{nameof(DBMigration)}.Scripts.PostDeploymentScripts"), new SqlScriptOptions { RunGroupOrder = 70, ScriptType = ScriptType.RunOnce })

            // Use configured logging to output migration
            .LogToAutodetectedLog()

            // Capture script output in log
            .LogScriptOutput()

            // Add variables that should be used in scripts. See 000001-CreatePersonTable.sql how to use variables in a script
            .WithVariables(new Dictionary<string, string>
            {
                // TODO: add more variables here...
                ["PersonTableName"] = "Person"
            });

            if (dryRun)
            {
                // Migration will be encapsulated within a transaction that will be rollback for dry run purpose
                builder.WithTransactionAlwaysRollback();
            }
            else
            {
                // Migration will be encapsulated within a transaction to ensure consistency. This can be changed to .WithoutTransaction() or .WithTransactionPerScript()
                builder.WithTransaction();
            }

            // Build the builder (Builder Pattern)
            var upgradeEngine = builder.Build();

            if (string.IsNullOrEmpty(output))
            {
                // Perform database migration
                var result = upgradeEngine.PerformUpgrade();

                // Verify migration result
                if (false == result.Successful)
                {
                    logger.LogError("Database migration failed!!!");
                    logger.LogError($"Script Name: {result.ErrorScript.Name}");
                    logger.LogError(result.Error, "Error");
                    Environment.ExitCode = -1;
                }
                else
                {
                    logger.LogInformation("Database migration successful");
                    Environment.ExitCode = 0;
                }
            }
            else
            {
                var scriptsToExecute = upgradeEngine.GetScriptsToExecute();

                if (File.Exists(output))
                {
                    logger.LogError($"Output file {output} already exists");
                }
                else
                {
                    using (var fileStream = File.Open(output, FileMode.CreateNew, FileAccess.Write))
                    using (var textStream = new StreamWriter(fileStream))
                    {
                        scriptsToExecute.ForEach(script =>
                        {
                            textStream.WriteLine($"-- Script: {script.Name}");
                            textStream.WriteLine(script.Contents);
                            textStream.WriteLine();
                        });
                    }
                    logger.LogInformation($"Output written to file {output}");
                }
            }
        }
    }
}