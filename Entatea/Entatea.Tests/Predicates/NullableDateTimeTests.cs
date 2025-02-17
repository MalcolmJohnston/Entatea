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
    public class NullableDateTimeTests : BaseTest
    {
        [TestCase(typeof(InMemoryDataContext))]
        [TestCase(typeof(SqlServerDataContext))]
        [TestCase(typeof(MySqlDataContext))]
        [TestCase(typeof(SqliteDataContext))]
        public async Task Read_With_NullableDateTime_Equal_Predicate(Type dataContextType)
        {
            // Arrange
            using IDataContext dataContext = DataContextTestHelper.SetupDataContext(dataContextType);
            DateTime now = DateTime.Now.Date;
            await dataContext.Create(new Product2() { Updated = now });
            await dataContext.Create(new Product2() { Updated = now.AddDays(-1) });

            // Act
            IEnumerable<Product2> products = await dataContext.ReadList<Product2>(Equal<Product2>(x => x.Updated, now));

            // Assert
            Assert.That(products.Count(), Is.EqualTo(1));
            Assert.That(products.ElementAt(0).Updated, Is.EqualTo(now));
        }

        [TestCase(typeof(InMemoryDataContext))]
        [TestCase(typeof(SqlServerDataContext))]
        [TestCase(typeof(MySqlDataContext))]
        [TestCase(typeof(SqliteDataContext))]
        public async Task Read_With_NullableDateTime_Equal_Null_Predicate(Type dataContextType)
        {
            // Arrange
            using IDataContext dataContext = DataContextTestHelper.SetupDataContext(dataContextType);
            DateTime now = DateTime.Now.Date;
            await dataContext.Create(new Product2() { Updated = null });
            await dataContext.Create(new Product2() { Updated = now });

            // Act
            IEnumerable<Product2> products = await dataContext.ReadList<Product2>(Equal<Product2>(x => x.Updated, null));

            // Assert
            Assert.That(products.Count(), Is.EqualTo(1));
            Assert.That(products.ElementAt(0).Updated, Is.Null);
        }

        [TestCase(typeof(InMemoryDataContext))]
        [TestCase(typeof(SqlServerDataContext))]
        [TestCase(typeof(MySqlDataContext))]
        [TestCase(typeof(SqliteDataContext))]
        public async Task Read_With_NullableDateTime_NotEqual_Predicate(Type dataContextType)
        {
            // Arrange
            using IDataContext dataContext = DataContextTestHelper.SetupDataContext(dataContextType);
            DateTime now = DateTime.Now.Date;
            await dataContext.Create(new Product2() { Updated = now });
            await dataContext.Create(new Product2() { Updated = now.AddDays(-1) });

            // Act
            IEnumerable<Product2> products = await dataContext.ReadList<Product2>(NotEqual<Product2>(x => x.Updated, now));

            // Assert
            Assert.That(products.Count(), Is.EqualTo(1));
            Assert.That(products.ElementAt(0).Updated, Is.Not.EqualTo(now));
        }

        [TestCase(typeof(InMemoryDataContext))]
        [TestCase(typeof(SqlServerDataContext))]
        [TestCase(typeof(MySqlDataContext))]
        [TestCase(typeof(SqliteDataContext))]
        public async Task Read_With_NullableDateTime_In_Predicate(Type dataContextType)
        {
            // Arrange
            using IDataContext dataContext = DataContextTestHelper.SetupDataContext(dataContextType);
            DateTime today = DateTime.Now.Date;
            DateTime yesterday = today.AddDays(-1);
            DateTime tomorrow = today.AddDays(1);
            DateTime tomorrowTomorrow = today.AddDays(2);

            await dataContext.Create(new Product2() { Updated = today });
            await dataContext.Create(new Product2() { Updated = yesterday });
            await dataContext.Create(new Product2() { Updated = tomorrow });
            await dataContext.Create(new Product2() { Updated = tomorrow });

            // Act
            DateTime[] dates = new[] { yesterday, tomorrow };
            IEnumerable<Product2> products = await dataContext.ReadList<Product2>(In<Product2>(x => x.Updated, dates));

            // Assert
            Assert.That(products.Count(), Is.EqualTo(3));
            Assert.That(dates, Is.EquivalentTo(products.Select(x => x.Updated).Distinct()));
        }

        [TestCase(typeof(InMemoryDataContext))]
        [TestCase(typeof(SqlServerDataContext))]
        [TestCase(typeof(MySqlDataContext))]
        [TestCase(typeof(SqliteDataContext))]
        public async Task Read_With_NullableDateTime_NotIn_Predicate(Type dataContextType)
        {
            // Arrange
            using IDataContext dataContext = DataContextTestHelper.SetupDataContext(dataContextType);
            DateTime today = DateTime.Now.Date;
            DateTime yesterday = today.AddDays(-1);
            DateTime tomorrow = today.AddDays(1);
            DateTime tomorrowTomorrow = today.AddDays(2);

            await dataContext.Create(new Product2() { Updated = today });
            await dataContext.Create(new Product2() { Updated = yesterday });
            await dataContext.Create(new Product2() { Updated = tomorrow });
            await dataContext.Create(new Product2() { Updated = tomorrow });

            // Act
            DateTime[] dates = new[] { yesterday, tomorrow };
            IEnumerable<Product2> products = await dataContext.ReadList<Product2>(NotIn<Product2>(x => x.Updated, dates));

            // Assert
            Assert.That(products.Count(), Is.EqualTo(1));
            Assert.That(products.ElementAt(0).Updated, Is.EqualTo(today));
        }

        [TestCase(typeof(InMemoryDataContext))]
        [TestCase(typeof(SqlServerDataContext))]
        [TestCase(typeof(MySqlDataContext))]
        [TestCase(typeof(SqliteDataContext))]
        public async Task Read_With_NullableDateTime_GreaterThan_Predicate(Type dataContextType)
        {
            // Arrange
            using IDataContext dataContext = DataContextTestHelper.SetupDataContext(dataContextType);
            DateTime today = DateTime.Now.Date;
            DateTime yesterday = today.AddDays(-1);
            DateTime tomorrow = today.AddDays(1);
            DateTime tomorrowTomorrow = today.AddDays(2);

            await dataContext.Create(new Product2() { Updated = today });
            await dataContext.Create(new Product2() { Updated = yesterday });
            await dataContext.Create(new Product2() { Updated = tomorrow });
            await dataContext.Create(new Product2() { Updated = tomorrow });

            // Act
            IEnumerable<Product2> products = await dataContext.ReadList<Product2>(GreaterThan<Product2>(x => x.Updated, today));

            // Assert
            Assert.That(products.Count(), Is.EqualTo(2));
            Assert.That(products.Select(x => x.Updated).Distinct(), Is.All.EqualTo(tomorrow));
        }

        [TestCase(typeof(InMemoryDataContext))]
        [TestCase(typeof(SqlServerDataContext))]
        [TestCase(typeof(MySqlDataContext))]
        [TestCase(typeof(SqliteDataContext))]
        public async Task Read_With_NullableDateTime_GreaterThanOrEqual_Predicate(Type dataContextType)
        {
            // Arrange
            using IDataContext dataContext = DataContextTestHelper.SetupDataContext(dataContextType);
            DateTime today = DateTime.Now.Date;
            DateTime yesterday = today.AddDays(-1);
            DateTime tomorrow = today.AddDays(1);
            DateTime tomorrowTomorrow = today.AddDays(2);

            await dataContext.Create(new Product2() { Updated = today });
            await dataContext.Create(new Product2() { Updated = yesterday });
            await dataContext.Create(new Product2() { Updated = tomorrow });
            await dataContext.Create(new Product2() { Updated = tomorrow });

            // Act
            IEnumerable<Product2> products = await dataContext.ReadList<Product2>(GreaterThanOrEqual<Product2>(x => x.Updated, today));

            // Assert
            Assert.That(products.Count(), Is.EqualTo(3));
            Assert.That(products.Select(x => x.Updated).Distinct(), Is.EquivalentTo(new[] { today, tomorrow }));
        }

        [TestCase(typeof(InMemoryDataContext))]
        [TestCase(typeof(SqlServerDataContext))]
        [TestCase(typeof(MySqlDataContext))]
        [TestCase(typeof(SqliteDataContext))]
        public async Task Read_With_NullableDateTime_LessThanPredicate(Type dataContextType)
        {
            // Arrange
            using IDataContext dataContext = DataContextTestHelper.SetupDataContext(dataContextType);
            DateTime today = DateTime.Now.Date;
            DateTime yesterday = today.AddDays(-1);
            DateTime tomorrow = today.AddDays(1);
            DateTime tomorrowTomorrow = today.AddDays(2);

            await dataContext.Create(new Product2() { Updated = today });
            await dataContext.Create(new Product2() { Updated = yesterday });
            await dataContext.Create(new Product2() { Updated = tomorrow });
            await dataContext.Create(new Product2() { Updated = tomorrow });

            // Act
            IEnumerable<Product2> products = await dataContext.ReadList<Product2>(LessThan<Product2>(x => x.Updated, today));

            // Assert
            Assert.That(products.Count(), Is.EqualTo(1));
            Assert.That(products.ElementAt(0).Updated, Is.EqualTo(yesterday));
        }

        [TestCase(typeof(InMemoryDataContext))]
        [TestCase(typeof(SqlServerDataContext))]
        [TestCase(typeof(MySqlDataContext))]
        [TestCase(typeof(SqliteDataContext))]
        public async Task Read_With_NullableDateTime_LessThanOrEqual_Predicate(Type dataContextType)
        {
            // Arrange
            using IDataContext dataContext = DataContextTestHelper.SetupDataContext(dataContextType);
            DateTime today = DateTime.Now.Date;
            DateTime yesterday = today.AddDays(-1);
            DateTime tomorrow = today.AddDays(1);
            DateTime tomorrowTomorrow = today.AddDays(2);

            await dataContext.Create(new Product2() { Updated = today });
            await dataContext.Create(new Product2() { Updated = yesterday });
            await dataContext.Create(new Product2() { Updated = tomorrow });
            await dataContext.Create(new Product2() { Updated = tomorrow });

            // Act
            IEnumerable<Product2> products = await dataContext.ReadList<Product2>(LessThanOrEqual<Product2>(x => x.Updated, today));

            // Assert
            Assert.That(products.Count(), Is.EqualTo(2));
            Assert.That(products.Select(x => x.Updated).Distinct(), Is.EquivalentTo(new[] { today, yesterday }));
        }
    }
}
