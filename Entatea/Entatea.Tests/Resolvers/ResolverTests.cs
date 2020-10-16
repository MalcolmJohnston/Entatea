using System;
using System.Linq;
using System.Threading.Tasks;

using Entatea.MySql;
using Entatea.SqlServer;
using Entatea.Resolvers;
using Entatea.Tests.Entities;
using Entatea.Tests.Helpers;
using NUnit.Framework;
using Entatea.Sqlite;

namespace Entatea.Tests.Resolvers
{
    [TestFixture]
    public class ResolverTests
    {
        /// <summary>
        /// Test that we can delete a single entity.
        /// </summary>
        
        [TestCase]
        public void Default_Table_Name_Resolver()
        {
            // Arrange
            ITableNameResolver resolver = new DefaultTableNameResolver();

            // Act
            string tableName = resolver.GetTableName(typeof(TestResolver).Name);

            // Assert
            Assert.AreEqual("TestResolvers", tableName);
        }

        [TestCase]
        public void Default_Column_Name_Resolver()
        {
            // Arrange
            IColumnNameResolver resolver = new DefaultColumnNameResolver();

            // Act
            string columnName = resolver.GetColumnName("TestResolverId");

            // Assert
            Assert.AreEqual("TestResolverId", columnName);
        }

        [TestCase]
        public void Underscore_Table_Name_Resolver_Test()
        {
            // Arrange
            ITableNameResolver resolver = new UnderscoreTableNameResolver();

            // Act
            string tableName = resolver.GetTableName(typeof(TestResolver).Name);

            // Assert
            Assert.AreEqual("test_resolvers", tableName);
        }

        [TestCase]
        public void Underscore_Column_Name_Resolver_Test()
        {
            // Arrange
            IColumnNameResolver resolver = new UnderscoreColumnNameResolver();

            // Act
            string columnName = resolver.GetColumnName("TestResolverId");

            // Assert
            Assert.AreEqual("test_resolver_id", columnName);
        }

        [TestCase(typeof(SqlServerDataContext))]
        [TestCase(typeof(MySqlDataContext))]
        [TestCase(typeof(SqliteDataContext))]
        public async Task Read_Test_Resolver_With_Default_Resolver(Type dataContextType)
        {
            // Arrange
            IDataContext dataContext = DataContextProvider.SetupDataContext(dataContextType);

            // Act
            TestResolver testResolver = (await dataContext.ReadAll<TestResolver>()).Single();


            // Assert
            Assert.AreEqual("Default", testResolver.ResolverValue);
        }

        [TestCase(typeof(SqlServerDataContext))]
        [TestCase(typeof(MySqlDataContext))]
        [TestCase(typeof(SqliteDataContext))]
        public async Task Read_Test_Resolver_With_Underscore_Resolver(Type dataContextType)
        {
            // Arrange
            IDataContext dataContext = DataContextProvider.SetupDataContext(
                dataContextType,
                new UnderscoreTableNameResolver(),
                new UnderscoreColumnNameResolver());

            // Act
            TestResolver testResolver = (await dataContext.ReadAll<TestResolver>()).Single();

            // Assert
            Assert.AreEqual("Underscore", testResolver.ResolverValue);
        }
    }
}
