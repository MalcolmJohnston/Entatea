using Entatea.SqlServer;
using NUnit.Framework;

namespace Entatea.Tests.Builders
{
    [TestFixture]
    public class TSqlBuilderTests
    {
        [Test]
        public void Get_Table_Name_With_Default_Schema()
        {
            // Arrange
            TSqlBuilder sqlBuilder = new();

            // Act
            string tableName = sqlBuilder.GetTableIdentifier<NoSchema>();

            // Assert
            Assert.That(tableName, Is.EqualTo("[dbo].[NoSchemas]"));
        }

        [Test]
        public void Get_Table_Name_With_Specified_Default_Schema()
        {
            // Arrange
            TSqlBuilder sqlBuilder = new(
                new Entatea.Resolvers.DefaultTableNameResolver(),
                new Entatea.Resolvers.DefaultColumnNameResolver(),
                "abc");

            // Act
            string tableName = sqlBuilder.GetTableIdentifier<NoSchema>();

            // Assert
            Assert.That(tableName, Is.EqualTo("[abc].[NoSchemas]"));
        }

        [Test]
        public void Get_Table_Name_With_Explicit_Schema()
        {
            // Arrange
            TSqlBuilder sqlBuilder = new(
                new Entatea.Resolvers.DefaultTableNameResolver(),
                new Entatea.Resolvers.DefaultColumnNameResolver(),
                "abc");

            // Act
            string tableName = sqlBuilder.GetTableIdentifier<Schema>();

            // Assert
            Assert.That(tableName, Is.EqualTo("[explicit].[Schemas]"));
        }
    }
}
