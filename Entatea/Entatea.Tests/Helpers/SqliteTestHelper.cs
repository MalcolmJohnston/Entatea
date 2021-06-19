using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Reflection;

using Microsoft.Extensions.DependencyInjection;

using NUnit.Framework;
using Entatea.Tests.Configuration;
using Microsoft.Data.Sqlite;
using FluentMigrator.Runner.Processors.SQLite;

namespace Entatea.Tests.Helpers
{
    /// <summary>
    /// Class that contains helper method for creating/deleting LocalDb databases whilst testing
    /// </summary>
    public class SqliteTestHelper
    {
        private static readonly Dictionary<string, string> testName2DbName = new Dictionary<string, string>();

        private static readonly string tempFolder = Path.Combine(Path.GetTempPath(), Assembly.GetExecutingAssembly().GetName().Name, "SqlLite");

        public static void CreateTestDatabase(string testName)
        {
            if (testName2DbName.ContainsKey(testName))
            {
                return;
            }

            // get the temporary file name and delete the temporary file
            string tempFile = Path.GetTempFileName();
            File.Delete(tempFile);

            // get the database name and add to cache
            string dbName = Path.Combine(tempFolder, Path.GetFileNameWithoutExtension(tempFile) + ".db");
            testName2DbName[testName] = dbName;

            // check whether our temp folder exists
            if (!Directory.Exists(tempFolder))
            {
                Directory.CreateDirectory(tempFolder);
            }

            FluentMigrationsRunner.UpSqlite(GetSqliteConnectionString(dbName));
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
            return GetSqliteConnectionString(dbName);
        }

        private static string GetSqliteConnectionString(string dbName)
        {
            return new SqliteConnectionStringBuilder() { DataSource = dbName }
                .ConnectionString;
        }

        private static IDbConnection OpenConnection(string connectionString)
        {
            // open the connection
            IDbConnection conn = new SqliteConnection(connectionString);
            conn.Open();

            return conn;
        }

        private static bool DropDatabase(string dbName)
        {
            try
            {
                File.Delete(dbName);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
