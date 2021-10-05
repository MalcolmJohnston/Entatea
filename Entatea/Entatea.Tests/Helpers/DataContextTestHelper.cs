using System;
using System.Collections.Generic;
using System.Text;

using NUnit.Framework;
using Entatea.InMemory;
using Entatea.MySql;
using Entatea.Resolvers;
using Entatea.SqlServer;
using Entatea.Sqlite;

namespace Entatea.Tests.Helpers
{
    public class DataContextTestHelper
    {
        public static IDataContext SetupDataContext(
            Type dataContextType,
            ITableNameResolver tableNameResolver = null,
            IColumnNameResolver columnNameResolver = null)
        {
            // check whether we are dealing with a type that implement IDataContext
            if (!IsDataContext(dataContextType))
            {
                throw new ArgumentException($"Type {dataContextType.Name} does not implement IDataContext.");
            }

            // return the data context for the given type
            if (IsInMemory(dataContextType))
            {
                return new InMemoryDataContext();
            }
            else if (IsSqlServer(dataContextType))
            {
                MsSqlTestHelper.CreateTestDatabase(TestContext.CurrentContext.Test.FullName);
                return new SqlServerDataContext(
                    MsSqlTestHelper.GetTestConnectionString(TestContext.CurrentContext.Test.FullName),
                    tableNameResolver ?? new DefaultTableNameResolver(),
                    columnNameResolver ?? new DefaultColumnNameResolver());
            }
            else if (IsMySql(dataContextType))
            {
                MySqlTestHelper.CreateTestDatabase(TestContext.CurrentContext.Test.FullName);
                return new MySqlDataContext(
                    MySqlTestHelper.GetTestConnectionString(TestContext.CurrentContext.Test.FullName),
                    tableNameResolver ?? new DefaultTableNameResolver(),
                    columnNameResolver ?? new DefaultColumnNameResolver());
            }
            else if (IsSqlite(dataContextType))
            {
                SqliteTestHelper.CreateTestDatabase(TestContext.CurrentContext.Test.FullName);
                return new SqliteDataContext(
                    SqliteTestHelper.GetTestConnectionString(TestContext.CurrentContext.Test.FullName),
                    tableNameResolver ?? new DefaultTableNameResolver(),
                    columnNameResolver ?? new DefaultColumnNameResolver());
            }
            else
            {
                throw new ArgumentException($"Type {dataContextType} is not supported, add support in {nameof(Helpers.DataContextTestHelper)}.cs");
            }
        }

        public static void DeleteDataContext(Type dataContextType)
        {
            // check whether we are dealing with a type that we need to dispose of
            if (IsSqlServer(dataContextType))
            {
                MsSqlTestHelper.DeleteTestDatabase(TestContext.CurrentContext.Test.FullName);
            } 
            else if (IsMySql(dataContextType))
            {
                MySqlTestHelper.DeleteTestDatabase(TestContext.CurrentContext.Test.FullName);
            }
            else if(IsSqlite(dataContextType))
            {
                SqliteTestHelper.DeleteTestDatabase(TestContext.CurrentContext.Test.FullName);
            }
        }

        public static void CreateTestDatabase(Type dataContextType)
        {
            if (IsSqlServer(dataContextType))
            {
                MsSqlTestHelper.CreateTestDatabase(TestContext.CurrentContext.Test.FullName);
            }
            else if (IsMySql(dataContextType))
            {
                MySqlTestHelper.CreateTestDatabase(TestContext.CurrentContext.Test.FullName);
            }
            else if (IsSqlite(dataContextType))
            {
                SqliteTestHelper.CreateTestDatabase(TestContext.CurrentContext.Test.FullName);
            }
        }

        public static string GetTestConnectionString(Type dataContextType)
        {
            if (IsSqlServer(dataContextType))
            {
                return MsSqlTestHelper.GetTestConnectionString(TestContext.CurrentContext.Test.FullName);
            }
            else if (IsMySql(dataContextType))
            {
                return MySqlTestHelper.GetTestConnectionString(TestContext.CurrentContext.Test.FullName);
            }
            else if (IsSqlite(dataContextType))
            {
                return SqliteTestHelper.GetTestConnectionString(TestContext.CurrentContext.Test.FullName);
            }

            return string.Empty;
        }

        public static IConnectionProvider GetConnectionProvider(Type dataContextType)
        {
            string connectionString = GetTestConnectionString(dataContextType);
            if (IsSqlServer(dataContextType))
            {
                return new SqlServerConnectionProvider(connectionString);
            }
            else if (IsMySql(dataContextType))
            {
                return new MySqlConnectionProvider(connectionString);
            }
            else if (IsSqlite(dataContextType))
            {
                return new SqliteConnectionProvider(connectionString);
            }

            return null;
        }

        public static IDataContext GetDataContext(Type dataContextType, IConnectionProvider connectionProvider)
        {
            if (IsSqlServer(dataContextType))
            {
                return new SqlServerDataContext((SqlServerConnectionProvider)connectionProvider);
            }
            else if (IsMySql(dataContextType))
            {
                return new MySqlDataContext((MySqlConnectionProvider)connectionProvider);
            }
            else if (IsSqlite(dataContextType))
            {
                return new SqliteDataContext((SqliteConnectionProvider)connectionProvider);
            }

            return null;
        }

        private static bool IsDataContext(Type type)
        {
            return typeof(IDataContext).IsAssignableFrom(type);
        }

        private static bool IsSqlServer(Type dataContextType)
        {
            return typeof(SqlServerDataContext).IsAssignableFrom(dataContextType);
        }

        private static bool IsMySql(Type dataContextType)
        {
            return typeof(MySqlDataContext).IsAssignableFrom(dataContextType);
        }

        private static bool IsSqlite(Type dataContextType)
        {
            return typeof(SqliteDataContext).IsAssignableFrom(dataContextType);
        }

        private static bool IsInMemory(Type dataContextType)
        {
            return typeof(InMemoryDataContext).IsAssignableFrom(dataContextType);
        }
    }
}
