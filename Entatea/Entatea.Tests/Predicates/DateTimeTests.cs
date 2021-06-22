using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
    public class DateTimeTests : BaseTest
    {
        [TestCase(typeof(InMemoryDataContext))]
        [TestCase(typeof(SqlServerDataContext))]
        [TestCase(typeof(MySqlDataContext))]
        [TestCase(typeof(SqliteDataContext))]
        public async Task Read_With_DateTime_Equal_Predicate(Type dataContextType)
        {
            // Arrange
            using IDataContext dataContext = DataContextTestHelper.SetupDataContext(dataContextType);
            DateTime now = DateTime.Now.Date;
            await dataContext.Create(new Product() { Updated = now });
            await dataContext.Create(new Product() { Updated = now.AddDays(-1) });

            // Act
            IEnumerable<Product> products = await dataContext.ReadList<Product>(Equal<Product>(x => x.Updated, now));

            // Assert
            Assert.AreEqual(1, products.Count());
            Assert.AreEqual(now, products.ElementAt(0).Updated);
        }

        [TestCase(typeof(InMemoryDataContext))]
        [TestCase(typeof(SqlServerDataContext))]
        [TestCase(typeof(MySqlDataContext))]
        [TestCase(typeof(SqliteDataContext))]
        public async Task Read_With_DateTime_NotEqual_Predicate(Type dataContextType)
        {
            // Arrange
            using IDataContext dataContext = DataContextTestHelper.SetupDataContext(dataContextType);
            DateTime now = DateTime.Now.Date;
            await dataContext.Create(new Product() { Updated = now });
            await dataContext.Create(new Product() { Updated = now.AddDays(-1) });

            // Act
            IEnumerable<Product> products = await dataContext.ReadList<Product>(NotEqual<Product>(x => x.Updated, now));

            // Assert
            Assert.AreEqual(1, products.Count());
            Assert.AreNotEqual(now, products.ElementAt(0).Updated);
        }

        [TestCase(typeof(InMemoryDataContext))]
        [TestCase(typeof(SqlServerDataContext))]
        [TestCase(typeof(MySqlDataContext))]
        [TestCase(typeof(SqliteDataContext))]
        public async Task Read_With_DateTime_In_Predicate(Type dataContextType)
        {
            // Arrange
            using IDataContext dataContext = DataContextTestHelper.SetupDataContext(dataContextType);
            DateTime today = DateTime.Now.Date;
            DateTime yesterday = today.AddDays(-1);
            DateTime tomorrow = today.AddDays(1);
            DateTime tomorrowTomorrow = today.AddDays(2);

            await dataContext.Create(new Product() { Updated = today });
            await dataContext.Create(new Product() { Updated = yesterday });
            await dataContext.Create(new Product() { Updated = tomorrow });
            await dataContext.Create(new Product() { Updated = tomorrow });

            // Act
            DateTime[] dates = new[] { yesterday, tomorrow };
            IEnumerable<Product> products = await dataContext.ReadList<Product>(In<Product>(x => x.Updated, dates));

            // Assert
            Assert.AreEqual(3, products.Count());
            Assert.That(dates, Is.EquivalentTo(products.Select(x => x.Updated).Distinct()));
        }

        [TestCase(typeof(InMemoryDataContext))]
        [TestCase(typeof(SqlServerDataContext))]
        [TestCase(typeof(MySqlDataContext))]
        [TestCase(typeof(SqliteDataContext))]
        public async Task Read_With_DateTime_NotIn_Predicate(Type dataContextType)
        {
            // Arrange
            using IDataContext dataContext = DataContextTestHelper.SetupDataContext(dataContextType);
            DateTime today = DateTime.Now.Date;
            DateTime yesterday = today.AddDays(-1);
            DateTime tomorrow = today.AddDays(1);
            DateTime tomorrowTomorrow = today.AddDays(2);

            await dataContext.Create(new Product() { Updated = today });
            await dataContext.Create(new Product() { Updated = yesterday });
            await dataContext.Create(new Product() { Updated = tomorrow });
            await dataContext.Create(new Product() { Updated = tomorrow });

            // Act
            DateTime[] dates = new[] { yesterday, tomorrow };
            IEnumerable<Product> products = await dataContext.ReadList<Product>(NotIn<Product>(x => x.Updated, dates));

            // Assert
            Assert.AreEqual(1, products.Count());
            Assert.AreEqual(today, products.ElementAt(0).Updated);
        }

        [TestCase(typeof(InMemoryDataContext))]
        [TestCase(typeof(SqlServerDataContext))]
        [TestCase(typeof(MySqlDataContext))]
        [TestCase(typeof(SqliteDataContext))]
        public async Task Read_With_DateTime_GreaterThan_Predicate(Type dataContextType)
        {
            // Arrange
            using IDataContext dataContext = DataContextTestHelper.SetupDataContext(dataContextType);
            DateTime today = DateTime.Now.Date;
            DateTime yesterday = today.AddDays(-1);
            DateTime tomorrow = today.AddDays(1);
            DateTime tomorrowTomorrow = today.AddDays(2);

            await dataContext.Create(new Product() { Updated = today });
            await dataContext.Create(new Product() { Updated = yesterday });
            await dataContext.Create(new Product() { Updated = tomorrow });
            await dataContext.Create(new Product() { Updated = tomorrow });

            // Act
            IEnumerable<Product> products = await dataContext.ReadList<Product>(GreaterThan<Product>(x => x.Updated, today));

            // Assert
            Assert.AreEqual(2, products.Count());
            Assert.AreEqual(new[] { tomorrow }, products.Select(x => x.Updated).Distinct());
        }

        [TestCase(typeof(InMemoryDataContext))]
        [TestCase(typeof(SqlServerDataContext))]
        [TestCase(typeof(MySqlDataContext))]
        [TestCase(typeof(SqliteDataContext))]
        public async Task Read_With_DateTime_GreaterThanOrEqual_Predicate(Type dataContextType)
        {
            // Arrange
            using IDataContext dataContext = DataContextTestHelper.SetupDataContext(dataContextType);
            DateTime today = DateTime.Now.Date;
            DateTime yesterday = today.AddDays(-1);
            DateTime tomorrow = today.AddDays(1);
            DateTime tomorrowTomorrow = today.AddDays(2);

            await dataContext.Create(new Product() { Updated = today });
            await dataContext.Create(new Product() { Updated = yesterday });
            await dataContext.Create(new Product() { Updated = tomorrow });
            await dataContext.Create(new Product() { Updated = tomorrow });

            // Act
            IEnumerable<Product> products = await dataContext.ReadList<Product>(GreaterThanOrEqual<Product>(x => x.Updated, today));

            // Assert
            Assert.AreEqual(3, products.Count());
            Assert.AreEqual(new[] { today, tomorrow }, products.Select(x => x.Updated).Distinct());
        }

        [TestCase(typeof(InMemoryDataContext))]
        [TestCase(typeof(SqlServerDataContext))]
        [TestCase(typeof(MySqlDataContext))]
        [TestCase(typeof(SqliteDataContext))]
        public async Task Read_With_DateTime_LessThanPredicate(Type dataContextType)
        {
            // Arrange
            using IDataContext dataContext = DataContextTestHelper.SetupDataContext(dataContextType);
            DateTime today = DateTime.Now.Date;
            DateTime yesterday = today.AddDays(-1);
            DateTime tomorrow = today.AddDays(1);
            DateTime tomorrowTomorrow = today.AddDays(2);

            await dataContext.Create(new Product() { Updated = today });
            await dataContext.Create(new Product() { Updated = yesterday });
            await dataContext.Create(new Product() { Updated = tomorrow });
            await dataContext.Create(new Product() { Updated = tomorrow });

            // Act
            IEnumerable<Product> products = await dataContext.ReadList<Product>(LessThan<Product>(x => x.Updated, today));

            // Assert
            Assert.AreEqual(1, products.Count());
            Assert.AreEqual(yesterday, products.ElementAt(0).Updated);
        }

        [TestCase(typeof(InMemoryDataContext))]
        [TestCase(typeof(SqlServerDataContext))]
        [TestCase(typeof(MySqlDataContext))]
        [TestCase(typeof(SqliteDataContext))]
        public async Task Read_With_DateTime_LessThanOrEqual_Predicate(Type dataContextType)
        {
            // Arrange
            using IDataContext dataContext = DataContextTestHelper.SetupDataContext(dataContextType);
            DateTime today = DateTime.Now.Date;
            DateTime yesterday = today.AddDays(-1);
            DateTime tomorrow = today.AddDays(1);
            DateTime tomorrowTomorrow = today.AddDays(2);

            await dataContext.Create(new Product() { Updated = today });
            await dataContext.Create(new Product() { Updated = yesterday });
            await dataContext.Create(new Product() { Updated = tomorrow });
            await dataContext.Create(new Product() { Updated = tomorrow });

            // Act
            IEnumerable<Product> products = await dataContext.ReadList<Product>(LessThanOrEqual<Product>(x => x.Updated, today));

            // Assert
            Assert.AreEqual(2, products.Count());
            Assert.AreEqual(new[] { today, yesterday }, products.Select(x => x.Updated).Distinct());
        }
    }
}
