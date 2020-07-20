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
    public class IntegerTests : BaseTest
    {
        [TestCase(typeof(InMemoryDataContext))]
        [TestCase(typeof(SqlServerDataContext))]
        [TestCase(typeof(MySqlDataContext))]
        public async Task Read_With_Int_Equal_Predicate(Type dataContextType)
        {
            // Arrange
            IDataContext dataContext = DataContextProvider.SetupDataContext(dataContextType);
            await dataContext.Create(new Product() { Stock = 10 });
            await dataContext.Create(new Product() { Stock = 11 });

            // Act
            IEnumerable<Product> products = await dataContext.ReadList<Product>(Equal<Product>(x => x.Stock, 10));

            // Assert
            Assert.AreEqual(1, products.Count());
            Assert.AreEqual(10, products.ElementAt(0).Stock);
        }

        [TestCase(typeof(InMemoryDataContext))]
        [TestCase(typeof(SqlServerDataContext))]
        [TestCase(typeof(MySqlDataContext))]
        public async Task Read_With_Int_NotEqual_Predicate(Type dataContextType)
        {
            // Arrange
            IDataContext dataContext = DataContextProvider.SetupDataContext(dataContextType);
            await dataContext.Create(new Product() { Stock = 10 });
            await dataContext.Create(new Product() { Stock = 11 });

            // Act
            IEnumerable<Product> products = await dataContext.ReadList<Product>(NotEqual<Product>(x => x.Stock, 10));

            // Assert
            Assert.AreEqual(1, products.Count());
            Assert.AreEqual(11, products.ElementAt(0).Stock);
        }

        [TestCase(typeof(InMemoryDataContext))]
        [TestCase(typeof(SqlServerDataContext))]
        [TestCase(typeof(MySqlDataContext))]
        public async Task Read_With_Int_In_Predicate(Type dataContextType)
        {
            // Arrange
            IDataContext dataContext = DataContextProvider.SetupDataContext(dataContextType);
            await dataContext.Create(new Product() { Stock = 10 });
            await dataContext.Create(new Product() { Stock = 10 });
            await dataContext.Create(new Product() { Stock = 11 });
            await dataContext.Create(new Product() { Stock = 12 });

            // Act
            int[] stock = new[] { 10, 12 };
            IEnumerable<Product> products = await dataContext.ReadList<Product>(In<Product>(x => x.Stock, stock));

            // Assert
            Assert.AreEqual(3, products.Count());
            Assert.That(stock, Is.EquivalentTo(products.Select(x => x.Stock).Distinct()));
        }

        [TestCase(typeof(InMemoryDataContext))]
        [TestCase(typeof(SqlServerDataContext))]
        [TestCase(typeof(MySqlDataContext))]
        public async Task Read_With_Int_NotIn_Predicate(Type dataContextType)
        {
            // Arrange
            IDataContext dataContext = DataContextProvider.SetupDataContext(dataContextType);
            await dataContext.Create(new Product() { Stock = 10 });
            await dataContext.Create(new Product() { Stock = 10 });
            await dataContext.Create(new Product() { Stock = 11 });
            await dataContext.Create(new Product() { Stock = 12 });

            // Act
            int[] stock = new[] { 10, 12 };
            IEnumerable<Product> products = await dataContext.ReadList<Product>(NotIn<Product>(x => x.Stock, stock));

            // Assert
            Assert.AreEqual(1, products.Count());
            Assert.AreEqual(11, products.ElementAt(0).Stock);
        }

        [TestCase(typeof(InMemoryDataContext))]
        [TestCase(typeof(SqlServerDataContext))]
        [TestCase(typeof(MySqlDataContext))]
        public async Task Read_With_Int_GreaterThan_Predicate(Type dataContextType)
        {
            // Arrange
            IDataContext dataContext = DataContextProvider.SetupDataContext(dataContextType);
            await dataContext.Create(new Product() { Stock = 10 });
            await dataContext.Create(new Product() { Stock = 10 });
            await dataContext.Create(new Product() { Stock = 11 });
            await dataContext.Create(new Product() { Stock = 12 });

            // Act
            IEnumerable<Product> products = await dataContext.ReadList<Product>(GreaterThan<Product>(x => x.Stock, 11));

            // Assert
            Assert.AreEqual(1, products.Count());
            Assert.AreEqual(12, products.ElementAt(0).Stock);
        }

        [TestCase(typeof(InMemoryDataContext))]
        [TestCase(typeof(SqlServerDataContext))]
        [TestCase(typeof(MySqlDataContext))]
        public async Task Read_With_Int_GreaterThanOrEqual_Predicate(Type dataContextType)
        {
            // Arrange
            IDataContext dataContext = DataContextProvider.SetupDataContext(dataContextType);
            await dataContext.Create(new Product() { Stock = 10 });
            await dataContext.Create(new Product() { Stock = 10 });
            await dataContext.Create(new Product() { Stock = 11 });
            await dataContext.Create(new Product() { Stock = 12 });

            // Act
            IEnumerable<Product> products = await dataContext.ReadList<Product>(GreaterThanOrEqual<Product>(x => x.Stock, 11));

            // Assert
            Assert.AreEqual(2, products.Count());
            Assert.That(new[] { 11, 12 }, Is.EquivalentTo(products.Select(x => x.Stock)));
        }

        [TestCase(typeof(InMemoryDataContext))]
        [TestCase(typeof(SqlServerDataContext))]
        [TestCase(typeof(MySqlDataContext))]
        public async Task Read_With_Int_LessThanPredicate(Type dataContextType)
        {
            // Arrange
            IDataContext dataContext = DataContextProvider.SetupDataContext(dataContextType);
            await dataContext.Create(new Product() { Stock = 10 });
            await dataContext.Create(new Product() { Stock = 10 });
            await dataContext.Create(new Product() { Stock = 11 });
            await dataContext.Create(new Product() { Stock = 12 });

            // Act
            IEnumerable<Product> products = await dataContext.ReadList<Product>(LessThan<Product>(x => x.Stock, 11));

            // Assert
            Assert.AreEqual(2, products.Count());
            Assert.That(new[] { 10 }, Is.EquivalentTo(products.Select(x => x.Stock).Distinct()));
        }

        [TestCase(typeof(InMemoryDataContext))]
        [TestCase(typeof(SqlServerDataContext))]
        [TestCase(typeof(MySqlDataContext))]
        public async Task Read_With_Int_LessThanOrEqual_Predicate(Type dataContextType)
        {
            // Arrange
            IDataContext dataContext = DataContextProvider.SetupDataContext(dataContextType);
            await dataContext.Create(new Product() { Stock = 10 });
            await dataContext.Create(new Product() { Stock = 10 });
            await dataContext.Create(new Product() { Stock = 11 });
            await dataContext.Create(new Product() { Stock = 12 });

            // Act
            IEnumerable<Product> products = await dataContext.ReadList<Product>(LessThanOrEqual<Product>(x => x.Stock, 11));

            // Assert
            Assert.AreEqual(3, products.Count());
            Assert.That(new[] { 10, 11 }, Is.EquivalentTo(products.Select(x => x.Stock).Distinct()));
        }

        [TestCase(typeof(InMemoryDataContext))]
        [TestCase(typeof(SqlServerDataContext))]
        [TestCase(typeof(MySqlDataContext))]
        public async Task Read_With_Int_Between_Predicate(Type dataContextType)
        {
            // Arrange
            IDataContext dataContext = DataContextProvider.SetupDataContext(dataContextType);
            await dataContext.Create(new Product() { Stock = 5 });
            await dataContext.Create(new Product() { Stock = 10 });
            await dataContext.Create(new Product() { Stock = 15 });
            await dataContext.Create(new Product() { Stock = 20 });

            // Act
            IEnumerable<Product> products = await dataContext.ReadList<Product>(Between<Product>(x => x.Stock, 11, 20));

            // Assert
            Assert.AreEqual(2, products.Count());
            Assert.That(new[] { 15, 20 }, Is.EquivalentTo(products.Select(x => x.Stock).Distinct()));
        }

        [TestCase(typeof(InMemoryDataContext))]
        [TestCase(typeof(SqlServerDataContext))]
        [TestCase(typeof(MySqlDataContext))]
        public async Task Read_With_Int_NotBetween_Predicate(Type dataContextType)
        {
            // Arrange
            IDataContext dataContext = DataContextProvider.SetupDataContext(dataContextType);
            await dataContext.Create(new Product() { Stock = 5 });
            await dataContext.Create(new Product() { Stock = 10 });
            await dataContext.Create(new Product() { Stock = 15 });
            await dataContext.Create(new Product() { Stock = 20 });

            // Act
            IEnumerable<Product> products = await dataContext.ReadList<Product>(NotBetween<Product>(x => x.Stock, 11, 20));

            // Assert
            Assert.AreEqual(2, products.Count());
            Assert.That(new[] { 5, 10 }, Is.EquivalentTo(products.Select(x => x.Stock).Distinct()));
        }
    }
}
