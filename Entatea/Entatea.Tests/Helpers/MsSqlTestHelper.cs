﻿using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Reflection;

using Microsoft.Data.SqlClient;

using Entatea.Tests.Configuration;

namespace Entatea.Tests.Helpers
{
    /// <summary>
    /// Class that contains helper method for creating/deleting MsSql databases whilst testing
    /// </summary>
    public class MsSqlTestHelper
    {
        private static readonly Dictionary<string, string> testName2DbName = new();

        private static readonly string tempFolder = Path.Combine(Path.GetTempPath(), Assembly.GetExecutingAssembly().GetName().Name, "LocalDb");

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
            string dbName = Path.GetFileNameWithoutExtension(tempFile);
            testName2DbName[testName] = dbName;

            // check whether our temp folder exists
            if (!Directory.Exists(tempFolder))
            {
                Directory.CreateDirectory(tempFolder);
            }

            // create the database
            using (SqlConnection conn = new (GetMsSqlConnectionString()))
            {
                conn.Open();

                string sql = $"CREATE DATABASE [{dbName}]";
                //sql += $" ON PRIMARY (NAME={dbName}, FILENAME = '{tempFolder}\\{dbName}.mdf')";
                //sql += $" LOG ON (NAME={dbName}_log, FILENAME = '{tempFolder}\\{dbName}_log.ldf')";

                SqlCommand command = new (sql, conn);
                command.ExecuteNonQuery();
            }

            // create the database schema
            FluentMigrationsRunner.UpSqlServer(GetMsSqlConnectionString(dbName));
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
            return GetMsSqlConnectionString(dbName);
        }

        public static string GetMsSqlConnectionString(string dbName)
        {
            return $"{GetMsSqlConnectionString()}Initial Catalog={dbName};";
        }

        public static string GetMsSqlConnectionString()
        {
            TestConfiguration config = ConfigurationHelper.GetTestConfiguration();
            return $"Data Source={config.MsSqlServer},{config.MsSqlPort};User Id={config.MsSqlUsername};Password={config.MsSqlPassword};TrustServerCertificate=True;";
        }

        public static IDbConnection OpenConnection(string connectionString)
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

                using IDbConnection conn = OpenConnection(GetMsSqlConnectionString("master"));
                IDbCommand cmd = conn.CreateCommand();
                cmd.CommandText = dropDbSql;
                cmd.ExecuteNonQuery();
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
