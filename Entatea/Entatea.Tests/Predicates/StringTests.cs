using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Entatea.InMemory;
using Entatea.MySql;
using static Entatea.Predicate.PredicateBuilder;
using Entatea.Sqlite;
using Entatea.SqlServer;

using Entatea.Tests.Helpers;
using Entatea.Tests.Entities;

using NUnit.Framework;

namespace Entatea.Tests.Predicates
{
    [TestFixture]
    public class StringTests : BaseTest
    {
        [TestCase(typeof(InMemoryDataContext))]
        [TestCase(typeof(SqlServerDataContext))]
        [TestCase(typeof(MySqlDataContext))]
        [TestCase(typeof(SqliteDataContext))]
        public async Task Read_With_String_Equal_Predicate(Type dataContextType)
        {
            // Arrange
            using IDataContext dataContext = DataContextTestHelper.SetupDataContext(dataContextType);
            await dataContext.Create(new Product() { Name = "Spanner" });
            await dataContext.Create(new Product() { Name = "Hammer" });

            // Act
            IEnumerable<Product> products = await dataContext.ReadList<Product>(Equal<Product>(x => x.Name, "Spanner"));

            // Assert
            Assert.That(products.Count(), Is.EqualTo(1));
            Assert.That(products.ElementAt(0).Name, Is.EqualTo("Spanner"));
        }

        [TestCase(typeof(InMemoryDataContext))]
        [TestCase(typeof(SqlServerDataContext))]
        [TestCase(typeof(MySqlDataContext))]
        [TestCase(typeof(SqliteDataContext))]
        public async Task Read_With_String_Equal_Null_Predicate(Type dataContextType)
        {
            // Arrange
            using IDataContext dataContext = DataContextTestHelper.SetupDataContext(dataContextType);
            await dataContext.Create(new Product() { Name = null });
            await dataContext.Create(new Product() { Name = "Hammer" });

            // Act
            IEnumerable<Product> products = await dataContext.ReadList<Product>(Equal<Product>(x => x.Name, null));

            // Assert
            Assert.That(products.Count(), Is.EqualTo(1));
            Assert.That(products.ElementAt(0).Name, Is.Null);
        }

        [TestCase(typeof(InMemoryDataContext))]
        [TestCase(typeof(SqlServerDataContext))]
        [TestCase(typeof(MySqlDataContext))]
        [TestCase(typeof(SqliteDataContext))]
        public async Task Read_With_String_NotEqual_Predicate(Type dataContextType)
        {
            // Arrange
            using IDataContext dataContext = DataContextTestHelper.SetupDataContext(dataContextType);
            await dataContext.Create(new Product() { Name = "Spanner" });
            await dataContext.Create(new Product() { Name = "Hammer" });

            // Act
            IEnumerable<Product> products = await dataContext.ReadList<Product>(NotEqual<Product>(x => x.Name, "Spanner"));

            // Assert
            Assert.That(products.Count(), Is.EqualTo(1));
            Assert.That(products.ElementAt(0).Name, Is.EqualTo("Hammer"));
        }

        [TestCase(typeof(InMemoryDataContext))]
        [TestCase(typeof(SqlServerDataContext))]
        [TestCase(typeof(MySqlDataContext))]
        [TestCase(typeof(SqliteDataContext))]
        public async Task Read_With_String_In_Predicate(Type dataContextType)
        {
            // Arrange
            using IDataContext dataContext = DataContextTestHelper.SetupDataContext(dataContextType);
            await dataContext.Create(new Product() { Name = "Spanner" });
            await dataContext.Create(new Product() { Name = "Hammer" });
            await dataContext.Create(new Product() { Name = "Nail" });

            // Act
            string[] productNames = new[] { "Hammer", "Nail" };
            IEnumerable<Product> products = await dataContext.ReadList<Product>(In<Product>(x => x.Name, productNames));
            
            // Assert
            Assert.That(products.Select(x => x.Name), Is.EquivalentTo(productNames));
        }

        [TestCase(typeof(InMemoryDataContext))]
        [TestCase(typeof(SqlServerDataContext))]
        [TestCase(typeof(MySqlDataContext))]
        [TestCase(typeof(SqliteDataContext))]
        public async Task Read_With_String_NotIn_Predicate(Type dataContextType)
        {
            // Arrange
            using IDataContext dataContext = DataContextTestHelper.SetupDataContext(dataContextType);
            await dataContext.Create(new Product() { Name = "Spanner" });
            await dataContext.Create(new Product() { Name = "Hammer" });
            await dataContext.Create(new Product() { Name = "Nail" });

            // Act
            string[] productNames = new[] { "Hammer", "Nail" };
            IEnumerable<Product> products = await dataContext.ReadList<Product>(NotIn<Product>(x => x.Name, productNames));

            // Assert
            Assert.That(products.Count(), Is.EqualTo(1));
            Assert.That(products.ElementAt(0).Name, Is.EqualTo("Spanner"));
        }

        [TestCase(typeof(InMemoryDataContext))]
        [TestCase(typeof(SqlServerDataContext))]
        [TestCase(typeof(MySqlDataContext))]
        [TestCase(typeof(SqliteDataContext))]
        public async Task Read_With_String_Contains_Predicate(Type dataContextType)
        {
            // Arrange
            using IDataContext dataContext = DataContextTestHelper.SetupDataContext(dataContextType);
            await dataContext.Create(new Product() { Name = "Bosch Hammer I" });
            await dataContext.Create(new Product() { Name = "Black and Decker Hammer" });
            await dataContext.Create(new Product() { Name = "Hammer B&Q" });
            await dataContext.Create(new Product() { Name = "Something Else" });

            // Act
            IEnumerable<Product> products = await dataContext.ReadList<Product>(Contains<Product>(x => x.Name, "Hammer"));

            // Assert
            Assert.That(products.Count(), Is.EqualTo(3));
        }

        [TestCase(typeof(InMemoryDataContext))]
        [TestCase(typeof(SqlServerDataContext))]
        [TestCase(typeof(MySqlDataContext))]
        [TestCase(typeof(SqliteDataContext))]
        public async Task Read_With_String_StartsWith_Predicate(Type dataContextType)
        {
            // Arrange
            using IDataContext dataContext = DataContextTestHelper.SetupDataContext(dataContextType);
            await dataContext.Create(new Product() { Name = "Bosch Hammer I" });
            await dataContext.Create(new Product() { Name = "Black and Decker Hammer" });
            await dataContext.Create(new Product() { Name = "Hammer B&Q" });
            await dataContext.Create(new Product() { Name = "Something Else" });

            // Act
            IEnumerable<Product> products = await dataContext.ReadList<Product>(StartsWith<Product>(x => x.Name, "Hammer"));

            // Assert
            Assert.That(products.Count(), Is.EqualTo(1));
            Assert.That(products.ElementAt(0).Name, Is.EqualTo("Hammer B&Q"));
        }

        [TestCase(typeof(InMemoryDataContext))]
        [TestCase(typeof(SqlServerDataContext))]
        [TestCase(typeof(MySqlDataContext))]
        [TestCase(typeof(SqliteDataContext))]
        public async Task Read_With_String_EndsWith_Predicate(Type dataContextType)
        {
            // Arrange
            using IDataContext dataContext = DataContextTestHelper.SetupDataContext(dataContextType);
            await dataContext.Create(new Product() { Name = "Bosch Hammer I" });
            await dataContext.Create(new Product() { Name = "Black and Decker Hammer" });
            await dataContext.Create(new Product() { Name = "Hammer B&Q" });
            await dataContext.Create(new Product() { Name = "Something Else" });

            // Act
            IEnumerable<Product> products = await dataContext.ReadList<Product>(EndsWith<Product>(x => x.Name, "Hammer"));

            // Assert
            Assert.That(products.Count(), Is.EqualTo(1));
            Assert.That(products.ElementAt(0).Name, Is.EqualTo("Black and Decker Hammer"));
        }
    }
}
