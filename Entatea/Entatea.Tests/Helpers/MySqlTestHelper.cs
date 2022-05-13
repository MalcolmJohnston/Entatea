using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Reflection;

using Microsoft.Extensions.DependencyInjection;

using MySql.Data.MySqlClient;
using NUnit.Framework;
using Entatea.Tests.Configuration;

namespace Entatea.Tests.Helpers
{
    /// <summary>
    /// Class that contains helper method for creating/deleting LocalDb databases whilst testing
    /// </summary>
    public class MySqlTestHelper
    {
        private static readonly Dictionary<string, string> testName2DbName = new Dictionary<string, string>();

        private static readonly string tempFolder = Path.Combine(Path.GetTempPath(), Assembly.GetExecutingAssembly().GetName().Name, "MySQL");

        public static void CreateTestDatabase(string testName)
        {
            // check whether database has already been created
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
            using (MySqlConnection conn = new MySqlConnection(GetMySqlConnectionString()))
            {
                conn.Open();

                string sql = $"CREATE DATABASE {dbName}";

                MySqlCommand command = new MySqlCommand(sql, conn);
                command.ExecuteNonQuery();
                conn.Close();
            }

            FluentMigrationsRunner.UpMySql(GetMySqlConnectionString(dbName));
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
            return GetMySqlConnectionString(dbName);
        }

        internal static string GetMySqlConnectionString()
        {
            TestConfiguration cfg = ConfigurationHelper.GetTestConfiguration();
            return $"Server={cfg.MySqlServer};Port={cfg.MySqlPort};Database=sys;UId={cfg.MySqlUsername};Pwd={cfg.MySqlPassword};";
        }

        private static string GetMySqlConnectionString(string dbName)
        {
            TestConfiguration cfg = ConfigurationHelper.GetTestConfiguration();
            return $"Server={cfg.MySqlServer};Database={dbName};Port={cfg.MySqlPort};UId={cfg.MySqlUsername};Pwd={cfg.MySqlPassword};";
        }

        internal static IDbConnection OpenConnection(string connectionString)
        {
            // open the connection
            IDbConnection conn = new MySqlConnection(connectionString);
            conn.Open();

            return conn;
        }

        private static bool DropDatabase(string dbName)
        {
            try
            {
                string sql = $"DROP DATABASE {dbName}";
                
                using (IDbConnection conn = OpenConnection(GetMySqlConnectionString()))
                {
                    IDbCommand cmd = conn.CreateCommand();
                    cmd.CommandText = sql;
                    cmd.ExecuteNonQuery();
                    conn.Close();
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
