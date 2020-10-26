using System;
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

namespace Entatea.Tests
{
    [TestFixture]
    public class DeleteTests : BaseTest
    {
        /// <summary>
        /// Test that we can delete a single entity.
        /// </summary>
        [TestCase(typeof(InMemoryDataContext))]
        [TestCase(typeof(SqlServerDataContext))]
        [TestCase(typeof(MySqlDataContext))]
        [TestCase(typeof(SqliteDataContext))]
        public async Task Delete_Entity(Type dataContextType)
        {
            // Arrange
            IDataContext dataContext = DataContextProvider.SetupDataContext(dataContextType);
            City city = await dataContext.Create<City>(new City() { CityCode = "BAS", CityName = "Basingstoke", Area = "Hampshire" });

            // Act
            await dataContext.Delete<City>(new { city.CityId, });

            // Assert
            Assert.IsNull(await dataContext.Read<City>(new { city.CityId }));
        }

        /// <summary>
        /// Test that we can delete a single entity with an identity column by passing a single
        /// typed argument rather than a property bag.
        /// </summary>
        [TestCase(typeof(InMemoryDataContext))]
        [TestCase(typeof(SqlServerDataContext))]
        [TestCase(typeof(MySqlDataContext))]
        [TestCase(typeof(SqliteDataContext))]
        public async Task Delete_Entity_Single_Typed_Argument(Type dataContextType)
        {
            // Arrange
            IDataContext dataContext = DataContextProvider.SetupDataContext(dataContextType);
            City city = await dataContext.Create<City>(new City() { CityCode = "BAS", CityName = "Basingstoke", Area = "Hampshire" });

            // Act
            await dataContext.Delete<City>(city.CityId);

            // Assert
            Assert.IsNull(await dataContext.Read<City>(new { city.CityId }));
        }

        /// <summary>
        /// Test that we can delete a multiple entities using a where condition.
        /// </summary>
        [TestCase(typeof(InMemoryDataContext))]
        [TestCase(typeof(SqlServerDataContext))]
        [TestCase(typeof(MySqlDataContext))]
        [TestCase(typeof(SqliteDataContext))]
        public async Task Delete_List_Of_Entities(Type dataContextType)
        {
            // Arrange
            IDataContext dataContext = DataContextProvider.SetupDataContext(dataContextType);
            await dataContext.Create<City>(new City() { CityCode = "BAS", CityName = "Basingstoke", Area = "Hampshire" });
            await dataContext.Create<City>(new City() { CityCode = "PUP", CityName = "Portsmouth", Area = "Hampshire" });
            await dataContext.Create<City>(new City() { CityCode = "BOU", CityName = "Bournemouth", Area = "Dorset" });
            await dataContext.Create<City>(new City() { CityCode = "HAV", CityName = "Havant", Area = "Hampshire" });

            // Act
            await dataContext.DeleteList<City>(new { Area = "Hampshire" });

            // Assert
            Assert.AreEqual(0, (await dataContext.ReadList<City>(new { Area = "Hampshire" })).Count());
            Assert.AreEqual(1, (await dataContext.ReadAll<City>()).Count());   
        }

        /// <summary>
        /// Test that if we try to delete a list of entities without specifying a condition that and exception is thrown.
        /// </summary>
        /// <returns></returns>
        [TestCase(typeof(InMemoryDataContext))]
        [TestCase(typeof(SqlServerDataContext))]
        [TestCase(typeof(MySqlDataContext))]
        [TestCase(typeof(SqliteDataContext))]
        public async Task Delete_List_No_Conditions(Type dataContextType)
        {
            // Arrange
            IDataContext dataContext = DataContextProvider.SetupDataContext(dataContextType);
            await dataContext.Create<City>(new City() { CityCode = "BAS", CityName = "Basingstoke", Area = "Hampshire" });
            await dataContext.Create<City>(new City() { CityCode = "PUP", CityName = "Portsmouth", Area = "Hampshire" });
            await dataContext.Create<City>(new City() { CityCode = "BOU", CityName = "Bournemouth", Area = "Dorset" });
            await dataContext.Create<City>(new City() { CityCode = "HAV", CityName = "Havant", Area = "Hampshire" });

            // Act / Assert
            Assert.ThrowsAsync<ArgumentException>(async () => { await dataContext.DeleteList<City>(null); });
            Assert.ThrowsAsync<ArgumentException>(async () => { await dataContext.DeleteList<City>(new object()); });
        }

        /// <summary>
        /// Test that if we try to delete a list of entities without specifying a condition that and exception is thrown.
        /// </summary>
        /// <returns></returns>
        [TestCase(typeof(InMemoryDataContext))]
        [TestCase(typeof(SqlServerDataContext))]
        [TestCase(typeof(MySqlDataContext))]
        [TestCase(typeof(SqliteDataContext))]
        public async Task Delete_List_Non_Existing_Conditions(Type dataContextType)
        {
            // Arrange
            IDataContext dataContext = DataContextProvider.SetupDataContext(dataContextType);
            await dataContext.Create<City>(new City() { CityCode = "BAS", CityName = "Basingstoke", Area = "Hampshire" });
            await dataContext.Create<City>(new City() { CityCode = "PUP", CityName = "Portsmouth", Area = "Hampshire" });
            await dataContext.Create<City>(new City() { CityCode = "BOU", CityName = "Bournemouth", Area = "Dorset" });
            await dataContext.Create<City>(new City() { CityCode = "HAV", CityName = "Havant", Area = "Hampshire" });

            // Act / Assert
            Assert.ThrowsAsync<ArgumentException>(async () => { await dataContext.DeleteList<City>(new { Code = "BAS" }); });
            Assert.ThrowsAsync<ArgumentException>(async () =>
            {
                await dataContext.DeleteList<City>(new
                {
                    Name = "Portsmouth",
                    Area = "Hampshire"
                });
            });
        }

        /// <summary>
        /// Test that we can soft delete a single entity.
        /// </summary>
        [TestCase(typeof(InMemoryDataContext))]
        [TestCase(typeof(SqlServerDataContext))]
        [TestCase(typeof(MySqlDataContext))]
        [TestCase(typeof(SqliteDataContext))]
        public async Task Soft_Delete_Entity(Type dataContextType)
        {
            // Arrange
            IDataContext dataContext = DataContextProvider.SetupDataContext(dataContextType);
            SoftDelete softDelete = await dataContext.Create(new SoftDelete());

            // Act
            await dataContext.Delete<SoftDelete>(new { softDelete.SoftDeleteId, });

            // Assert
            Assert.IsNull(await dataContext.Read<SoftDelete>(new { softDelete.SoftDeleteId }));
        }

        /// <summary>
        /// Test that we can soft delete a multiple entities using a where condition.
        /// </summary>
        [TestCase(typeof(InMemoryDataContext))]
        [TestCase(typeof(SqlServerDataContext))]
        [TestCase(typeof(MySqlDataContext))]
        [TestCase(typeof(SqliteDataContext))]
        public async Task Soft_Delete_List_Of_Entities(Type dataContextType)
        {
            // Arrange
            IDataContext dataContext = DataContextProvider.SetupDataContext(dataContextType);
            SoftDelete sd1 = await dataContext.Create(new SoftDelete());
            SoftDelete sd2 = await dataContext.Create(new SoftDelete());
            SoftDelete sd3 = await dataContext.Create(new SoftDelete());

            // Act
            await dataContext.DeleteList<SoftDelete>(
                In<SoftDelete>(x => x.SoftDeleteId, new int[] { sd2.SoftDeleteId, sd3.SoftDeleteId }));

            // Assert
            Assert.AreEqual(0, (await dataContext.ReadList<SoftDelete>(
                In<SoftDelete>(x => x.SoftDeleteId, new int[] { sd2.SoftDeleteId, sd3.SoftDeleteId }))).Count());
            Assert.AreEqual(1, (await dataContext.ReadAll<SoftDelete>()).Count());
        }

        /// <summary>
        /// Test that when we have two items with the same name and different discriminator
        /// When we delete by name the discriminator is taken into account and only the correct record is deleted
        /// </summary>
        /// <param name="dataContextType"></param>
        /// <returns></returns>
        public async Task Delete_Discriminator_Entity(Type dataContextType)
        {
            // Arrange
            IDataContext dataContext = DataContextProvider.SetupDataContext(dataContextType);
            DiscriminatorContact contact = await dataContext.Create(new DiscriminatorContact() { Name = "Paul" });
            DiscriminatorCompany company = await dataContext.Create(new DiscriminatorCompany() { Name = "Paul" });

            // Act
            await dataContext.DeleteList<DiscriminatorContact>(Equal<DiscriminatorContact>(x => x.Name, "Paul"));

            // Assert
            Assert.AreEqual(0, (await dataContext.ReadList<DiscriminatorContact>(In<DiscriminatorContact>(x => x.Name, "Paul"))).Count());
            Assert.AreEqual(1, (await dataContext.ReadList<DiscriminatorCompany>(In<DiscriminatorCompany>(x => x.Name, "Paul"))).Count());
        }
    }
}
