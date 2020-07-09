using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Testadal.InMemory;
using Testadal.MySql;
using static Testadal.Predicate.PredicateBuilder;
using Testadal.SqlServer;

using Testadal.Tests.Helpers;
using Testadal.Tests.Models;

using NUnit.Framework;

namespace Testadal.Tests
{
    public class ReadPredicateTests
    {
        /// <summary>
        /// Tests that we can read with an Equal predicate
        /// </summary>
        /// <returns></returns>
        [TestCase(typeof(InMemoryDataContext))]
        [TestCase(typeof(SqlServerDataContext))]
        [TestCase(typeof(MySqlDataContext))]
        public async Task Read_With_Equal_Predicate(Type dataContextType)
        {
            // Arrange
            IDataContext dataContext = DataContextProvider.SetupDataContext(dataContextType);
            await dataContext.Create(new City() { CityCode = "PUP", CityName = "Portsmouth", Area = "Hampshire" });
            await dataContext.Create(new City() { CityCode = "SOU", CityName = "Southampton", Area = "Hampshire" });
            await dataContext.Create(new City() { CityCode = "BOU", CityName = "Bournemouth", Area = "Dorset" });

            // Act
            IEnumerable<City> cities = await dataContext.ReadList<City>(Equal<City>(x => x.Area, "Hampshire"));

            // Assert
            // NOTE: there is only one team in Hampshire
            Assert.AreEqual(2, cities.Count());
        }

        /// <summary>
        /// Tests that we can read with an In predicate
        /// </summary>
        /// <param name="dataContextType"></param>
        /// <returns></returns>
        [TestCase(typeof(InMemoryDataContext))]
        [TestCase(typeof(SqlServerDataContext))]
        [TestCase(typeof(MySqlDataContext))]
        public async Task Read_With_In_Predicate(Type dataContextType)
        {
            // Arrange
            IDataContext dataContext = DataContextProvider.SetupDataContext(dataContextType);
            await dataContext.Create(new City() { CityCode = "PUP", CityName = "Portsmouth", Area = "Hampshire" });
            await dataContext.Create(new City() { CityCode = "SOU", CityName = "Southampton", Area = "Hampshire" });
            await dataContext.Create(new City() { CityCode = "BOU", CityName = "Bournemouth", Area = "Dorset" });

            // Act
            string[] cityCodes = new string[] { "PUP", "BOU" };
            IEnumerable<City> cities = await dataContext.ReadList<City>(In<City>(x => x.CityCode, cityCodes));

            // Assert
            Assert.That(cityCodes, Is.EquivalentTo(cities.Select(x => x.CityCode)));
        }
    }
}
