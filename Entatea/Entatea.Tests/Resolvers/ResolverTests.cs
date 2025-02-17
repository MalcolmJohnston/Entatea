using System;
using System.Linq;
using System.Threading.Tasks;

using Entatea.Model;
using Entatea.MySql;
using Entatea.SqlServer;
using Entatea.Resolvers;
using Entatea.Tests.Entities;
using Entatea.Tests.Helpers;
using Entatea.Sqlite;
using NUnit.Framework;

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
            ClassMap classMap = ClassMapper.GetClassMap<TestResolver>();

            // Act
            string tableName = resolver.GetTableName(classMap);

            // Assert
            Assert.That(tableName, Is.EqualTo("TestResolvers"));
        }

        [TestCase]
        public void Default_Column_Name_Resolver()
        {
            // Arrange
            IColumnNameResolver resolver = new DefaultColumnNameResolver();
            ClassMap classMap = ClassMapper.GetClassMap<TestResolver>();

            // Act
            string columnName = resolver.GetColumnName(classMap, "TestResolverId");

            // Assert
            Assert.That(columnName, Is.EqualTo("TestResolverId"));
        }

        [TestCase]
        public void Underscore_Table_Name_Resolver_Test()
        {
            // Arrange
            ITableNameResolver resolver = new UnderscoreTableNameResolver();
            ClassMap classMap = ClassMapper.GetClassMap<TestResolver>();

            // Act
            string tableName = resolver.GetTableName(classMap);

            // Assert
            Assert.That(tableName, Is.EqualTo("test_resolvers"));
        }

        [TestCase]
        public void Underscore_Column_Name_Resolver_Test()
        {
            // Arrange
            IColumnNameResolver resolver = new UnderscoreColumnNameResolver();
            ClassMap classMap = ClassMapper.GetClassMap<TestResolver>();


            // Act
            string columnName = resolver.GetColumnName(classMap, "TestResolverId");

            // Assert
            Assert.That(columnName, Is.EqualTo("test_resolver_id"));
        }

        [TestCase(typeof(SqlServerDataContext))]
        [TestCase(typeof(MySqlDataContext))]
        [TestCase(typeof(SqliteDataContext))]
        public async Task Read_Test_Resolver_With_Default_Resolver(Type dataContextType)
        {
            // Arrange
            using IDataContext dataContext = DataContextTestHelper.SetupDataContext(dataContextType);

            // Act
            TestResolver testResolver = (await dataContext.ReadAll<TestResolver>()).Single();


            // Assert
            Assert.That(testResolver.ResolverValue, Is.EqualTo("Default"));
        }

        [TestCase(typeof(SqlServerDataContext))]
        [TestCase(typeof(MySqlDataContext))]
        [TestCase(typeof(SqliteDataContext))]
        public async Task Read_Test_Resolver_With_Underscore_Resolver(Type dataContextType)
        {
            // Arrange
            IDataContext dataContext = DataContextTestHelper.SetupDataContext(
                dataContextType,
                new UnderscoreTableNameResolver(),
                new UnderscoreColumnNameResolver());

            // Act
            TestResolver testResolver = (await dataContext.ReadAll<TestResolver>()).Single();

            // Assert
            Assert.That(testResolver.ResolverValue, Is.EqualTo("Underscore"));
        }
    }
}
