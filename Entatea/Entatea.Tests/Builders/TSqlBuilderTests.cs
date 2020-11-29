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
            TSqlBuilder sqlBuilder = new TSqlBuilder();

            // Act
            string tableName = sqlBuilder.GetTableIdentifier<NoSchema>();

            // Assert
            Assert.AreEqual("[dbo].[NoSchemas]", tableName);
        }

        [Test]
        public void Get_Table_Name_With_Specified_Default_Schema()
        {
            // Arrange
            TSqlBuilder sqlBuilder = new TSqlBuilder(
                new Entatea.Resolvers.DefaultTableNameResolver(),
                new Entatea.Resolvers.DefaultColumnNameResolver(),
                "abc");

            // Act
            string tableName = sqlBuilder.GetTableIdentifier<NoSchema>();

            // Assert
            Assert.AreEqual("[abc].[NoSchemas]", tableName);
        }

        [Test]
        public void Get_Table_Name_With_Explicit_Schema()
        {
            // Arrange
            TSqlBuilder sqlBuilder = new TSqlBuilder(
                new Entatea.Resolvers.DefaultTableNameResolver(),
                new Entatea.Resolvers.DefaultColumnNameResolver(),
                "abc");

            // Act
            string tableName = sqlBuilder.GetTableIdentifier<Schema>();

            // Assert
            Assert.AreEqual("[explicit].[Schemas]", tableName);
        }
    }
}
