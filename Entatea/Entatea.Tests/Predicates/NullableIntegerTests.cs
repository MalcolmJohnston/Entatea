using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text;

using NUnit.Framework;

using Entatea.InMemory;
using Entatea.MySql;
using static Entatea.Predicate.PredicateBuilder;
using Entatea.SqlServer;

using Entatea.Tests.Helpers;
using Entatea.Tests.Models;

namespace Entatea.Tests.Predicates
{
    [TestFixture]
    public class NullableIntegerTests : BaseTest
    {
        [TestCase(typeof(InMemoryDataContext))]
        [TestCase(typeof(SqlServerDataContext))]
        [TestCase(typeof(MySqlDataContext))]
        public async Task Read_With_NullableInt_Equal_Predicate(Type dataContextType)
        {
            // Arrange
            IDataContext dataContext = DataContextProvider.SetupDataContext(dataContextType);
            await dataContext.Create(new Product2() { Stock = 10 });
            await dataContext.Create(new Product2() { Stock = 11 });

            // Act
            IEnumerable<Product2> products = await dataContext.ReadList<Product2>(Equal<Product2>(x => x.Stock, 10));

            // Assert
            Assert.AreEqual(1, products.Count());
            Assert.AreEqual(10, products.ElementAt(0).Stock);
        }

        [TestCase(typeof(InMemoryDataContext))]
        [TestCase(typeof(SqlServerDataContext))]
        [TestCase(typeof(MySqlDataContext))]
        public async Task Read_With_NullableInt_Equal_Null_Predicate(Type dataContextType)
        {
            // Arrange
            IDataContext dataContext = DataContextProvider.SetupDataContext(dataContextType);
            await dataContext.Create(new Product2() { Stock = null });
            await dataContext.Create(new Product2() { Stock = 11 });

            // Act
            IEnumerable<Product2> products = await dataContext.ReadList<Product2>(Equal<Product2>(x => x.Stock, null));

            // Assert
            Assert.AreEqual(1, products.Count());
            Assert.AreEqual(null, products.ElementAt(0).Stock);
        }

        [TestCase(typeof(InMemoryDataContext))]
        [TestCase(typeof(SqlServerDataContext))]
        [TestCase(typeof(MySqlDataContext))]
        public async Task Read_With_NullableInt_NotEqual_Predicate(Type dataContextType)
        {
            // Arrange
            IDataContext dataContext = DataContextProvider.SetupDataContext(dataContextType);
            await dataContext.Create(new Product2() { Stock = 10 });
            await dataContext.Create(new Product2() { Stock = 11 });

            // Act
            IEnumerable<Product2> products = await dataContext.ReadList<Product2>(NotEqual<Product2>(x => x.Stock, 10));

            // Assert
            Assert.AreEqual(1, products.Count());
            Assert.AreEqual(11, products.ElementAt(0).Stock);
        }

        [TestCase(typeof(InMemoryDataContext))]
        [TestCase(typeof(SqlServerDataContext))]
        [TestCase(typeof(MySqlDataContext))]
        public async Task Read_With_NullableInt_In_Predicate(Type dataContextType)
        {
            // Arrange
            IDataContext dataContext = DataContextProvider.SetupDataContext(dataContextType);
            await dataContext.Create(new Product2() { Stock = 10 });
            await dataContext.Create(new Product2() { Stock = 10 });
            await dataContext.Create(new Product2() { Stock = 11 });
            await dataContext.Create(new Product2() { Stock = 12 });

            // Act
            int[] stock = new[] { 10, 12 };
            IEnumerable<Product2> products = await dataContext.ReadList<Product2>(In<Product2>(x => x.Stock, stock));

            // Assert
            Assert.AreEqual(3, products.Count());
            Assert.That(stock, Is.EquivalentTo(products.Select(x => x.Stock).Distinct()));
        }

        [TestCase(typeof(InMemoryDataContext))]
        [TestCase(typeof(SqlServerDataContext))]
        [TestCase(typeof(MySqlDataContext))]
        public async Task Read_With_NullableInt_NotIn_Predicate(Type dataContextType)
        {
            // Arrange
            IDataContext dataContext = DataContextProvider.SetupDataContext(dataContextType);
            await dataContext.Create(new Product2() { Stock = 10 });
            await dataContext.Create(new Product2() { Stock = 10 });
            await dataContext.Create(new Product2() { Stock = 11 });
            await dataContext.Create(new Product2() { Stock = 12 });

            // Act
            int[] stock = new[] { 10, 12 };
            IEnumerable<Product2> products = await dataContext.ReadList<Product2>(NotIn<Product2>(x => x.Stock, stock));

            // Assert
            Assert.AreEqual(1, products.Count());
            Assert.AreEqual(11, products.ElementAt(0).Stock);
        }

        [TestCase(typeof(InMemoryDataContext))]
        [TestCase(typeof(SqlServerDataContext))]
        [TestCase(typeof(MySqlDataContext))]
        public async Task Read_With_NullableInt_GreaterThan_Predicate(Type dataContextType)
        {
            // Arrange
            IDataContext dataContext = DataContextProvider.SetupDataContext(dataContextType);
            await dataContext.Create(new Product2() { Stock = 10 });
            await dataContext.Create(new Product2() { Stock = 10 });
            await dataContext.Create(new Product2() { Stock = 11 });
            await dataContext.Create(new Product2() { Stock = 12 });

            // Act
            IEnumerable<Product2> products = await dataContext.ReadList<Product2>(GreaterThan<Product2>(x => x.Stock, 11));

            // Assert
            Assert.AreEqual(1, products.Count());
            Assert.AreEqual(12, products.ElementAt(0).Stock);
        }

        [TestCase(typeof(InMemoryDataContext))]
        [TestCase(typeof(SqlServerDataContext))]
        [TestCase(typeof(MySqlDataContext))]
        public async Task Read_With_NullableInt_GreaterThanOrEqual_Predicate(Type dataContextType)
        {
            // Arrange
            IDataContext dataContext = DataContextProvider.SetupDataContext(dataContextType);
            await dataContext.Create(new Product2() { Stock = 10 });
            await dataContext.Create(new Product2() { Stock = 10 });
            await dataContext.Create(new Product2() { Stock = 11 });
            await dataContext.Create(new Product2() { Stock = 12 });

            // Act
            IEnumerable<Product2> products = await dataContext.ReadList<Product2>(GreaterThanOrEqual<Product2>(x => x.Stock, 11));

            // Assert
            Assert.AreEqual(2, products.Count());
            Assert.That(new[] { 11, 12 }, Is.EquivalentTo(products.Select(x => x.Stock)));
        }

        [TestCase(typeof(InMemoryDataContext))]
        [TestCase(typeof(SqlServerDataContext))]
        [TestCase(typeof(MySqlDataContext))]
        public async Task Read_With_NullableInt_LessThanPredicate(Type dataContextType)
        {
            // Arrange
            IDataContext dataContext = DataContextProvider.SetupDataContext(dataContextType);
            await dataContext.Create(new Product2() { Stock = 10 });
            await dataContext.Create(new Product2() { Stock = 10 });
            await dataContext.Create(new Product2() { Stock = 11 });
            await dataContext.Create(new Product2() { Stock = 12 });

            // Act
            IEnumerable<Product2> products = await dataContext.ReadList<Product2>(LessThan<Product2>(x => x.Stock, 11));

            // Assert
            Assert.AreEqual(2, products.Count());
            Assert.That(new[] { 10 }, Is.EquivalentTo(products.Select(x => x.Stock).Distinct()));
        }

        [TestCase(typeof(InMemoryDataContext))]
        [TestCase(typeof(SqlServerDataContext))]
        [TestCase(typeof(MySqlDataContext))]
        public async Task Read_With_NullableInt_LessThanOrEqual_Predicate(Type dataContextType)
        {
            // Arrange
            IDataContext dataContext = DataContextProvider.SetupDataContext(dataContextType);
            await dataContext.Create(new Product2() { Stock = 10 });
            await dataContext.Create(new Product2() { Stock = 10 });
            await dataContext.Create(new Product2() { Stock = 11 });
            await dataContext.Create(new Product2() { Stock = 12 });

            // Act
            IEnumerable<Product2> products = await dataContext.ReadList<Product2>(LessThanOrEqual<Product2>(x => x.Stock, 11));

            // Assert
            Assert.AreEqual(3, products.Count());
            Assert.That(new[] { 10, 11 }, Is.EquivalentTo(products.Select(x => x.Stock).Distinct()));
        }

        [TestCase(typeof(InMemoryDataContext))]
        [TestCase(typeof(SqlServerDataContext))]
        [TestCase(typeof(MySqlDataContext))]
        public async Task Read_With_NullableInt_Between_Predicate(Type dataContextType)
        {
            // Arrange
            IDataContext dataContext = DataContextProvider.SetupDataContext(dataContextType);
            await dataContext.Create(new Product2() { Stock = 5 });
            await dataContext.Create(new Product2() { Stock = 10 });
            await dataContext.Create(new Product2() { Stock = 15 });
            await dataContext.Create(new Product2() { Stock = 20 });

            // Act
            IEnumerable<Product2> products = await dataContext.ReadList<Product2>(Between<Product2>(x => x.Stock, 11, 20));

            // Assert
            Assert.AreEqual(2, products.Count());
            Assert.That(new[] { 15, 20 }, Is.EquivalentTo(products.Select(x => x.Stock).Distinct()));
        }

        [TestCase(typeof(InMemoryDataContext))]
        [TestCase(typeof(SqlServerDataContext))]
        [TestCase(typeof(MySqlDataContext))]
        public async Task Read_With_NullableInt_NotBetween_Predicate(Type dataContextType)
        {
            // Arrange
            IDataContext dataContext = DataContextProvider.SetupDataContext(dataContextType);
            await dataContext.Create(new Product2() { Stock = 5 });
            await dataContext.Create(new Product2() { Stock = 10 });
            await dataContext.Create(new Product2() { Stock = 15 });
            await dataContext.Create(new Product2() { Stock = 20 });

            // Act
            IEnumerable<Product2> products = await dataContext.ReadList<Product2>(NotBetween<Product2>(x => x.Stock, 11, 20));

            // Assert
            Assert.AreEqual(2, products.Count());
            Assert.That(new[] { 5, 10 }, Is.EquivalentTo(products.Select(x => x.Stock).Distinct()));
        }
    }
}
