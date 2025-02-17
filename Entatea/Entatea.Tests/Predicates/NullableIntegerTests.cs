using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text;

using NUnit.Framework;

using Entatea.InMemory;
using Entatea.MySql;
using static Entatea.Predicate.PredicateBuilder;
using Entatea.Sqlite;
using Entatea.SqlServer;

using Entatea.Tests.Helpers;
using Entatea.Tests.Entities;

namespace Entatea.Tests.Predicates
{
    [TestFixture]
    public class NullableIntegerTests : BaseTest
    {
        [TestCase(typeof(InMemoryDataContext))]
        [TestCase(typeof(SqlServerDataContext))]
        [TestCase(typeof(MySqlDataContext))]
        [TestCase(typeof(SqliteDataContext))]
        public async Task Read_With_NullableInt_Equal_Predicate(Type dataContextType)
        {
            // Arrange
            using IDataContext dataContext = DataContextTestHelper.SetupDataContext(dataContextType);
            await dataContext.Create(new Product2() { Stock = 10 });
            await dataContext.Create(new Product2() { Stock = 11 });

            // Act
            IEnumerable<Product2> products = await dataContext.ReadList<Product2>(Equal<Product2>(x => x.Stock, 10));

            // Assert
            Assert.That(products.Count(), Is.EqualTo(1));
            Assert.That(products.ElementAt(0).Stock, Is.EqualTo(10));
        }

        [TestCase(typeof(InMemoryDataContext))]
        [TestCase(typeof(SqlServerDataContext))]
        [TestCase(typeof(MySqlDataContext))]
        [TestCase(typeof(SqliteDataContext))]
        public async Task Read_With_NullableInt_Equal_Null_Predicate(Type dataContextType)
        {
            // Arrange
            using IDataContext dataContext = DataContextTestHelper.SetupDataContext(dataContextType);
            await dataContext.Create(new Product2() { Stock = null });
            await dataContext.Create(new Product2() { Stock = 11 });

            // Act
            IEnumerable<Product2> products = await dataContext.ReadList<Product2>(Equal<Product2>(x => x.Stock, null));

            // Assert
            Assert.That(products.Count(), Is.EqualTo(1));
            Assert.That(products.ElementAt(0).Stock, Is.Null);
        }

        [TestCase(typeof(InMemoryDataContext))]
        [TestCase(typeof(SqlServerDataContext))]
        [TestCase(typeof(MySqlDataContext))]
        [TestCase(typeof(SqliteDataContext))]
        public async Task Read_With_NullableInt_NotEqual_Predicate(Type dataContextType)
        {
            // Arrange
            using IDataContext dataContext = DataContextTestHelper.SetupDataContext(dataContextType);
            await dataContext.Create(new Product2() { Stock = 10 });
            await dataContext.Create(new Product2() { Stock = 11 });

            // Act
            IEnumerable<Product2> products = await dataContext.ReadList<Product2>(NotEqual<Product2>(x => x.Stock, 10));

            // Assert
            Assert.That(products.Count(), Is.EqualTo(1));
            Assert.That(products.ElementAt(0).Stock, Is.EqualTo(11));
        }

        [TestCase(typeof(InMemoryDataContext))]
        [TestCase(typeof(SqlServerDataContext))]
        [TestCase(typeof(MySqlDataContext))]
        [TestCase(typeof(SqliteDataContext))]
        public async Task Read_With_NullableInt_In_Predicate(Type dataContextType)
        {
            // Arrange
            using IDataContext dataContext = DataContextTestHelper.SetupDataContext(dataContextType);
            await dataContext.Create(new Product2() { Stock = 10 });
            await dataContext.Create(new Product2() { Stock = 10 });
            await dataContext.Create(new Product2() { Stock = 11 });
            await dataContext.Create(new Product2() { Stock = 12 });

            // Act
            int[] stock = new[] { 10, 12 };
            IEnumerable<Product2> products = await dataContext.ReadList<Product2>(In<Product2>(x => x.Stock, stock));

            // Assert
            Assert.That(products.Count(), Is.EqualTo(3));
            Assert.That(products.Select(x => x.Stock).Distinct(), Is.EquivalentTo(stock));
        }

        [TestCase(typeof(InMemoryDataContext))]
        [TestCase(typeof(SqlServerDataContext))]
        [TestCase(typeof(MySqlDataContext))]
        [TestCase(typeof(SqliteDataContext))]
        public async Task Read_With_NullableInt_NotIn_Predicate(Type dataContextType)
        {
            // Arrange
            using IDataContext dataContext = DataContextTestHelper.SetupDataContext(dataContextType);
            await dataContext.Create(new Product2() { Stock = 10 });
            await dataContext.Create(new Product2() { Stock = 10 });
            await dataContext.Create(new Product2() { Stock = 11 });
            await dataContext.Create(new Product2() { Stock = 12 });

            // Act
            int[] stock = new[] { 10, 12 };
            IEnumerable<Product2> products = await dataContext.ReadList<Product2>(NotIn<Product2>(x => x.Stock, stock));

            // Assert
            Assert.That(products.Count(), Is.EqualTo(1));
            Assert.That(products.ElementAt(0).Stock, Is.EqualTo(11));
        }

        [TestCase(typeof(InMemoryDataContext))]
        [TestCase(typeof(SqlServerDataContext))]
        [TestCase(typeof(MySqlDataContext))]
        [TestCase(typeof(SqliteDataContext))]
        public async Task Read_With_NullableInt_GreaterThan_Predicate(Type dataContextType)
        {
            // Arrange
            using IDataContext dataContext = DataContextTestHelper.SetupDataContext(dataContextType);
            await dataContext.Create(new Product2() { Stock = 10 });
            await dataContext.Create(new Product2() { Stock = 10 });
            await dataContext.Create(new Product2() { Stock = 11 });
            await dataContext.Create(new Product2() { Stock = 12 });

            // Act
            IEnumerable<Product2> products = await dataContext.ReadList<Product2>(GreaterThan<Product2>(x => x.Stock, 11));

            // Assert
            Assert.That(products.Count(), Is.EqualTo(1));
            Assert.That(products.ElementAt(0).Stock, Is.EqualTo(12));
        }

        [TestCase(typeof(InMemoryDataContext))]
        [TestCase(typeof(SqlServerDataContext))]
        [TestCase(typeof(MySqlDataContext))]
        [TestCase(typeof(SqliteDataContext))]
        public async Task Read_With_NullableInt_GreaterThanOrEqual_Predicate(Type dataContextType)
        {
            // Arrange
            using IDataContext dataContext = DataContextTestHelper.SetupDataContext(dataContextType);
            await dataContext.Create(new Product2() { Stock = 10 });
            await dataContext.Create(new Product2() { Stock = 10 });
            await dataContext.Create(new Product2() { Stock = 11 });
            await dataContext.Create(new Product2() { Stock = 12 });

            // Act
            IEnumerable<Product2> products = await dataContext.ReadList<Product2>(GreaterThanOrEqual<Product2>(x => x.Stock, 11));

            // Assert
            Assert.That(products.Count(), Is.EqualTo(2));
            Assert.That(products.Select(x => x.Stock), Is.EquivalentTo(new[] { 11, 12 }));
        }

        [TestCase(typeof(InMemoryDataContext))]
        [TestCase(typeof(SqlServerDataContext))]
        [TestCase(typeof(MySqlDataContext))]
        [TestCase(typeof(SqliteDataContext))]
        public async Task Read_With_NullableInt_LessThanPredicate(Type dataContextType)
        {
            // Arrange
            using IDataContext dataContext = DataContextTestHelper.SetupDataContext(dataContextType);
            await dataContext.Create(new Product2() { Stock = 10 });
            await dataContext.Create(new Product2() { Stock = 10 });
            await dataContext.Create(new Product2() { Stock = 11 });
            await dataContext.Create(new Product2() { Stock = 12 });

            // Act
            IEnumerable<Product2> products = await dataContext.ReadList<Product2>(LessThan<Product2>(x => x.Stock, 11));

            // Assert
            Assert.That(products.Count(), Is.EqualTo(2));
            Assert.That(products.Select(x => x.Stock), Is.All.EqualTo(10));
        }

        [TestCase(typeof(InMemoryDataContext))]
        [TestCase(typeof(SqlServerDataContext))]
        [TestCase(typeof(MySqlDataContext))]
        [TestCase(typeof(SqliteDataContext))]
        public async Task Read_With_NullableInt_LessThanOrEqual_Predicate(Type dataContextType)
        {
            // Arrange
            using IDataContext dataContext = DataContextTestHelper.SetupDataContext(dataContextType);
            await dataContext.Create(new Product2() { Stock = 10 });
            await dataContext.Create(new Product2() { Stock = 10 });
            await dataContext.Create(new Product2() { Stock = 11 });
            await dataContext.Create(new Product2() { Stock = 12 });

            // Act
            IEnumerable<Product2> products = await dataContext.ReadList<Product2>(LessThanOrEqual<Product2>(x => x.Stock, 11));

            // Assert
            Assert.That(products.Count(), Is.EqualTo(3));
            Assert.That(products.Select(x => x.Stock).Distinct(), Is.EquivalentTo(new[] { 10, 11 }));
        }

        [TestCase(typeof(InMemoryDataContext))]
        [TestCase(typeof(SqlServerDataContext))]
        [TestCase(typeof(MySqlDataContext))]
        [TestCase(typeof(SqliteDataContext))]
        public async Task Read_With_NullableInt_Between_Predicate(Type dataContextType)
        {
            // Arrange
            using IDataContext dataContext = DataContextTestHelper.SetupDataContext(dataContextType);
            await dataContext.Create(new Product2() { Stock = 5 });
            await dataContext.Create(new Product2() { Stock = 10 });
            await dataContext.Create(new Product2() { Stock = 15 });
            await dataContext.Create(new Product2() { Stock = 20 });

            // Act
            IEnumerable<Product2> products = await dataContext.ReadList<Product2>(Between<Product2>(x => x.Stock, 11, 20));

            // Assert
            Assert.That(products.Count(), Is.EqualTo(2));
            Assert.That(products.Select(x => x.Stock).Distinct(), Is.EquivalentTo(new[] { 15, 20 }));
        }

        [TestCase(typeof(InMemoryDataContext))]
        [TestCase(typeof(SqlServerDataContext))]
        [TestCase(typeof(MySqlDataContext))]
        [TestCase(typeof(SqliteDataContext))]
        public async Task Read_With_NullableInt_NotBetween_Predicate(Type dataContextType)
        {
            // Arrange
            using IDataContext dataContext = DataContextTestHelper.SetupDataContext(dataContextType);
            await dataContext.Create(new Product2() { Stock = 5 });
            await dataContext.Create(new Product2() { Stock = 10 });
            await dataContext.Create(new Product2() { Stock = 15 });
            await dataContext.Create(new Product2() { Stock = 20 });

            // Act
            IEnumerable<Product2> products = await dataContext.ReadList<Product2>(NotBetween<Product2>(x => x.Stock, 11, 20));

            // Assert
            Assert.That(products.Count(), Is.EqualTo(2));
            Assert.That(products.Select(x => x.Stock).Distinct(), Is.EquivalentTo(new[] { 5, 10 }));
        }
    }
}
