using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Reflection;

using Microsoft.Extensions.DependencyInjection;

using FluentMigrator.Runner;
using FluentMigrator.Runner.Initialization;
using NUnit.Framework;

namespace TdDb.Tests.Helpers
{
    /// <summary>
    /// Class that contains helper method for creating/deleting LocalDb databases whilst testing/
    /// </summary>
    [SetUpFixture]
    public class LocalDbTestHelper
    {
        private const string localDbDataSource = @"(localdb)\MSSQLLocalDB";

        private static readonly Dictionary<string, string> testName2DbName = new Dictionary<string, string>();

        private static readonly string tempFolder = Path.Combine(Path.GetTempPath(), Assembly.GetExecutingAssembly().GetName().Name);

        public static void CreateTestDatabase(string testName)
        {
            // get the temporary file name and delete the temporary file
            string tempFile = Path.GetTempFileName();
            File.Delete(tempFile);

            // get the database name and add to cache
            string dbName = Path.GetFileNameWithoutExtension(tempFile);
            testName2DbName[testName] = dbName;

            // check whether our temp folder exists
            if (!Directory.Exists(tempFolder))
            {
                Directory.CreateDirectory(tempFolder);
            }

            // create the database
            using (SqlConnection conn = new SqlConnection($"Server={localDbDataSource}"))
            {
                conn.Open();

                string sql = $"CREATE DATABASE [{dbName}]";
                sql += $" ON PRIMARY (NAME={dbName}, FILENAME = '{tempFolder}\\{dbName}.mdf')";
                sql += $" LOG ON (NAME={dbName}_log, FILENAME = '{tempFolder}\\{dbName}_log.ldf')";

                SqlCommand command = new SqlCommand(sql, conn);
                command.ExecuteNonQuery();
            }

            // then migrate up!
            var runner = new ServiceCollection()
                // Add common FluentMigrator services
                .AddFluentMigratorCore()
                .ConfigureRunner(rb => rb
                    // Add SQL Server support to FluentMigrator
                    .AddSqlServer()
                    // Set the connection string
                    .WithGlobalConnectionString(GetLocalDbConnectionString(dbName, GetMdfPath(tempFolder, dbName)))
                    // Define the assembly containing the migrations
                    .ScanIn(typeof(Migrations.CreateSchema).Assembly).For.Migrations())
                // Enable logging to console in the FluentMigrator way
                .AddLogging(lb => lb.AddFluentMigratorConsole())
                // Build the service provider
                .BuildServiceProvider(false)
                .GetRequiredService<IMigrationRunner>();

            runner.MigrateUp();
        }

        public static IDbConnection OpenTestConnection(string testName)
        {
            if (!testName2DbName.ContainsKey(testName))
            {
                CreateTestDatabase(testName);
            }

            return OpenConnection(GetTestConnectionString(testName));
        }

        public static void DeleteTestDatabase(string testName)
        {
            if (testName2DbName.ContainsKey(testName))
            {
                // get the database name
                string databaseName = testName2DbName[testName];

                // detach database and drop
                DropDatabase(databaseName);

                testName2DbName.Remove(testName);
            }
        }

        public static string GetTestConnectionString(string testName)
        {
            string dbName = testName2DbName[testName];
            return GetLocalDbConnectionString(dbName, GetMdfPath(tempFolder, dbName));
        }

        private static string GetLocalDbConnectionString(string dbName, string dbFileName)
        {
            return $"Data Source={localDbDataSource};Database={dbName};AttachDbFileName={dbFileName};Integrated Security=True;";
        }

        private static string GetLocalDbConnectionString(string dbName)
        {
            return $"Data Source={localDbDataSource};Initial Catalog={dbName};Integrated Security=True;";
        }

        private static string GetMdfPath(string folder, string dbName)
        {
            return Path.Combine(folder, $"{dbName}.mdf");
        }

        private static IDbConnection OpenConnection(string connectionString)
        {
            // open the connection
            IDbConnection conn = new SqlConnection(connectionString);
            conn.Open();

            return conn;
        }

        private static bool DropDatabase(string dbName)
        {
            try
            {
                string dropDbSql =
                    $@"IF (SELECT DB_ID('{dbName}')) IS NOT NULL
                        BEGIN
                            ALTER DATABASE [{dbName}] SET OFFLINE WITH ROLLBACK IMMEDIATE;
                            ALTER DATABASE [{dbName}] SET ONLINE;
                            DROP DATABASE [{dbName}];
                        END";

                using (IDbConnection conn = OpenConnection(GetLocalDbConnectionString("master")))
                {
                    IDbCommand cmd = conn.CreateCommand();
                    cmd.CommandText = dropDbSql;
                    cmd.ExecuteNonQuery();

                    return true;
                }
            }
            catch
            {
                return false;
            }
        }
    }
}
