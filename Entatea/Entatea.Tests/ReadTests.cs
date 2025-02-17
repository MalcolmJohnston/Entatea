using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Entatea.InMemory;
using Entatea.MySql;
using Entatea.SqlServer;
using Entatea.Sqlite;

using static Entatea.Predicate.PredicateBuilder;

using Entatea.Tests.Helpers;
using Entatea.Tests.Entities;

using NUnit.Framework;

namespace Entatea.Tests
{
    [TestFixture]
    public class ReadTests : BaseTest
    {
        /// <summary>
        /// Test that we can execute Get All when the Model is mapped with an Identity column.
        /// </summary>
        [TestCase(typeof(InMemoryDataContext))]
        [TestCase(typeof(SqlServerDataContext))]
        [TestCase(typeof(MySqlDataContext))]
        [TestCase(typeof(SqliteDataContext))]
        public async Task Read_All_With_Identity(Type dataContextType)
        {
            // Arrange
            using IDataContext dataContext = DataContextTestHelper.SetupDataContext(dataContextType);
            await dataContext.Create(new City() { CityCode = "PUP", CityName = "Portsmouth", Area = "Hampshire" });
            await dataContext.Create(new City() { CityCode = "BOU", CityName = "Bournemouth", Area = "Dorset" });

            // Act
            IEnumerable<City> cities = await dataContext.ReadAll<City>();

            // Assert
            Assert.That(cities.Count(), Is.EqualTo(2));
            Assert.That(cities.ElementAt(0).CityId, Is.GreaterThan(0));
            Assert.That(cities.ElementAt(1).CityId, Is.GreaterThan(cities.ElementAt(0).CityId));
        }

        /// <summary>
        /// Test that we can execute Get All when the Model is mapped with a Manual key column.
        /// </summary>
        /// <returns></returns>
        [TestCase(typeof(InMemoryDataContext))]
        [TestCase(typeof(SqlServerDataContext))]
        [TestCase(typeof(MySqlDataContext))]
        [TestCase(typeof(SqliteDataContext))]
        public async Task Read_All_With_Assigned(Type dataContextType)
        {
            // Arrange
            using IDataContext dataContext = DataContextTestHelper.SetupDataContext(dataContextType);
            await dataContext.Create(new CityManual() { CityCode = "PUP", CityName = "Portsmouth" });
            await dataContext.Create(new CityManual() { CityCode = "NYC", CityName = "New York City" });

            // Act
            IEnumerable<CityManual> cities = dataContext.ReadAll<CityManual>().GetAwaiter().GetResult();

            // Assert
            Assert.That(cities.Count(), Is.EqualTo(2));
        }

        /// <summary>
        /// Test that we can get an entity with a single identity key using the property bag approach.
        /// </summary>
        [TestCase(typeof(InMemoryDataContext))]
        [TestCase(typeof(SqlServerDataContext))]
        [TestCase(typeof(MySqlDataContext))]
        [TestCase(typeof(SqliteDataContext))]
        public async Task Read_By_Id_With_Identity_Property_Bag(Type dataContextType)
        {
            // Arrange
            using IDataContext dataContext = DataContextTestHelper.SetupDataContext(dataContextType);
            City pup = await dataContext.Create(new City() { CityCode = "PUP", CityName = "Portsmouth", Area = "Hampshire" });
            await dataContext.Create(new City() { CityCode = "NYC", CityName = "New York City", Area = "New York" });

            // Act
            City city = await dataContext.Read<City>(new { pup.CityId });

            // Assert
            Assert.That(city.CityId, Is.EqualTo(pup.CityId));
            Assert.That(city.CityCode, Is.EqualTo("PUP"));
            Assert.That(city.CityName, Is.EqualTo("Portsmouth"));
        }

        /// <summary>
        /// Test that we can get an entity with a single identity key by passing a single typed argument
        /// rather than using a property bag.
        /// </summary>
        [TestCase(typeof(InMemoryDataContext))]
        [TestCase(typeof(SqlServerDataContext))]
        [TestCase(typeof(MySqlDataContext))]
        [TestCase(typeof(SqliteDataContext))]
        public async Task Read_By_Id_With_Identity_Single_Typed_Argument(Type dataContextType)
        {
            // Arrange
            using IDataContext dataContext = DataContextTestHelper.SetupDataContext(dataContextType);
            City pup = await dataContext.Create(new City() { CityCode = "PUP", CityName = "Portsmouth", Area = "Hampshire" });
            await dataContext.Create(new City() { CityCode = "NYC", CityName = "New York City", Area = "New York" });

            // Act
            City city = await dataContext.Read<City>(pup.CityId);

            // Assert
            Assert.That(city.CityId, Is.EqualTo(pup.CityId));
            Assert.That(city.CityCode, Is.EqualTo("PUP"));
            Assert.That(city.CityName, Is.EqualTo("Portsmouth"));
        }

        /// <summary>
        /// Test we can retrieve an entity with a single assigned key.
        /// </summary>
        /// <returns></returns>
        [TestCase(typeof(InMemoryDataContext))]
        [TestCase(typeof(SqlServerDataContext))]
        [TestCase(typeof(MySqlDataContext))]
        [TestCase(typeof(SqliteDataContext))]
        public async Task Read_By_Id_With_Assigned_Property_Bag(Type dataContextType)
        {
            // Arrange
            using IDataContext dataContext = DataContextTestHelper.SetupDataContext(dataContextType);
            await dataContext.Create(new CityManual() { CityCode = "PUP", CityName = "Portsmouth" });
            await dataContext.Create(new CityManual() { CityCode = "NYC", CityName = "New York City" });

            // Act
            CityManual city = await dataContext.Read<CityManual>(new { CityCode = "NYC" });

            // Assert
            Assert.That(city.CityCode, Is.EqualTo("NYC"));
            Assert.That(city.CityName, Is.EqualTo("New York City"));
        }

        /// <summary>
        /// Test we can retrive an entity with a single assigned key using a single typed argument.
        /// </summary>
        /// <returns></returns>
        [TestCase(typeof(InMemoryDataContext))]
        [TestCase(typeof(SqlServerDataContext))]
        [TestCase(typeof(MySqlDataContext))]
        [TestCase(typeof(SqliteDataContext))]
        public async Task Read_By_Id_With_Assigned_Single_Typed_Argument(Type dataContextType)
        {
            // Arrange
            using IDataContext dataContext = DataContextTestHelper.SetupDataContext(dataContextType);
            await dataContext.Create(new CityManual() { CityCode = "PUP", CityName = "Portsmouth" });
            await dataContext.Create(new CityManual() { CityCode = "NYC", CityName = "New York City" });

            // Act
            CityManual city = await dataContext.Read<CityManual>("NYC");

            // Assert
            Assert.That(city.CityCode, Is.EqualTo("NYC"));
            Assert.That(city.CityName, Is.EqualTo("New York City"));
        }

        /// <summary>
        /// Tests that we can read with a simple equality comparison.
        /// </summary>
        /// <returns></returns>
        [TestCase(typeof(InMemoryDataContext))]
        [TestCase(typeof(SqlServerDataContext))]
        [TestCase(typeof(MySqlDataContext))]
        [TestCase(typeof(SqliteDataContext))]
        public async Task Read_List_By_Where_Condition(Type dataContextType)
        {
            // Arrange
            using IDataContext dataContext = DataContextTestHelper.SetupDataContext(dataContextType);
            await dataContext.Create(new City() { CityCode = "PUP", CityName = "Portsmouth", Area = "Hampshire" });
            await dataContext.Create(new City() { CityCode = "SOU", CityName = "Southampton", Area = "Hampshire" });
            await dataContext.Create(new City() { CityCode = "BOU", CityName = "Bournemouth", Area = "Dorset" });

            // Act
            IEnumerable<City> cities = await dataContext.ReadList<City>(new { Area = "Hampshire" });

            // Assert
            // NOTE: there is only one team in Hampshire
            Assert.That(cities.Count(), Is.EqualTo(2));
        }

        /// <summary>
        /// Tests that we can read with a simple (and implied) In comparison
        /// </summary>
        /// <param name="dataContextType"></param>
        /// <returns></returns>
        [TestCase(typeof(InMemoryDataContext))]
        [TestCase(typeof(SqlServerDataContext))]
        [TestCase(typeof(MySqlDataContext))]
        [TestCase(typeof(SqliteDataContext))]
        public async Task Read_List_By_Where_Condition_Implied_In(Type dataContextType)
        {
            // Arrange
            using IDataContext dataContext = DataContextTestHelper.SetupDataContext(dataContextType);
            await dataContext.Create(new City() { CityCode = "PUP", CityName = "Portsmouth", Area = "Hampshire" });
            await dataContext.Create(new City() { CityCode = "SOU", CityName = "Southampton", Area = "Hampshire" });
            await dataContext.Create(new City() { CityCode = "BOU", CityName = "Bournemouth", Area = "Dorset" });

            // Act
            string[] cityCodes = new string[] { "PUP", "BOU" };
            IEnumerable<City> cities = await dataContext.ReadList<City>(new { CityCode = cityCodes });

            // Assert
            Assert.That(cities.Select(x => x.CityCode), Is.EquivalentTo(cityCodes));
        }

        /// <summary>
        /// Tests that we can read with more than one implied In comparison
        /// </summary>
        /// <param name="dataContextType"></param>
        /// <returns></returns>
        [TestCase(typeof(InMemoryDataContext))]
        [TestCase(typeof(SqlServerDataContext))]
        [TestCase(typeof(MySqlDataContext))]
        [TestCase(typeof(SqliteDataContext))]
        public async Task Read_List_By_Where_Condition_Implied_Ins(Type dataContextType)
        {
            // Arrange
            using IDataContext dataContext = DataContextTestHelper.SetupDataContext(dataContextType);
            await dataContext.Create(new City() { CityCode = "PUP", CityName = "Portsmouth", Area = "Hampshire" });
            await dataContext.Create(new City() { CityCode = "SOU", CityName = "Southampton", Area = "Hampshire" });
            await dataContext.Create(new City() { CityCode = "CHI", CityName = "Chichester", Area = "West Sussex" });
            await dataContext.Create(new City() { CityCode = "BOU", CityName = "Bournemouth", Area = "Dorset" });

            // Act
            string[] cityCodes = new[] { "PUP", "BOU", "CHI" };
            string[] areas = new[] { "Hampshire", "West Sussex" };
            IEnumerable<City> cities = await dataContext.ReadList<City>(new 
            { 
                CityCode = cityCodes,
                Area = areas 
            });

            // Assert
            Assert.That(new[] { "PUP", "CHI" }, Is.EquivalentTo(cities.Select(x => x.CityCode)));
        }

        /// <summary>
        /// Tests that we can read with an implied in and an Equal
        /// </summary>
        /// <param name="dataContextType"></param>
        /// <returns></returns>
        [TestCase(typeof(InMemoryDataContext))]
        [TestCase(typeof(SqlServerDataContext))]
        [TestCase(typeof(MySqlDataContext))]
        [TestCase(typeof(SqliteDataContext))]
        public async Task Read_List_By_Where_Condition_Implied_In_And_Equal(Type dataContextType)
        {
            // Arrange
            using IDataContext dataContext = DataContextTestHelper.SetupDataContext(dataContextType);
            await dataContext.Create(new City() { CityCode = "PUP", CityName = "Portsmouth", Area = "Hampshire" });
            await dataContext.Create(new City() { CityCode = "SOU", CityName = "Southampton", Area = "Hampshire" });
            await dataContext.Create(new City() { CityCode = "CHI", CityName = "Chichester", Area = "West Sussex" });
            await dataContext.Create(new City() { CityCode = "BOU", CityName = "Bournemouth", Area = "Dorset" });

            // Act
            string[] cityCodes = new[] { "PUP", "BOU", "CHI" };
            IEnumerable<City> cities = await dataContext.ReadList<City>(new
            {
                CityCode = cityCodes,
                Area = "West Sussex"
            });

            // Assert
            Assert.That(new[] { "CHI" }, Is.EquivalentTo(cities.Select(x => x.CityCode)));
        }

        /// <summary>
        /// Tests that we read data from the same table, we only get the data for the given discriminator
        /// </summary>
        /// <param name="dataContextType"></param>
        /// <returns></returns>
        [TestCase(typeof(InMemoryDataContext))]
        [TestCase(typeof(SqlServerDataContext))]
        [TestCase(typeof(MySqlDataContext))]
        [TestCase(typeof(SqliteDataContext))]
        public async Task Read_List_Discriminator_Entities(Type dataContextType)
        {
            // Arrange
            using IDataContext dataContext = DataContextTestHelper.SetupDataContext(dataContextType);
            await dataContext.Create(new DiscriminatorContact() { Name = "Paul" });
            await dataContext.Create(new DiscriminatorContact() { Name = "John" });
            
            await dataContext.Create(new DiscriminatorCompany() { Name = "Apple Music Ltd" });
            await dataContext.Create(new DiscriminatorCompany() { Name = "Linda McCartneys Ltd" });
            await dataContext.Create(new DiscriminatorCompany() { Name = "Stella McCartney Fashion Ltd" });

            // Act
            IEnumerable<DiscriminatorCompany> companies = await dataContext.ReadAll<DiscriminatorCompany>();

            // Assert
            Assert.That(companies.Count(), Is.EqualTo(3));
            Assert.That(companies.Select(x => x.DiscriminatorType), Is.All.EqualTo(DiscriminatorType.Company));
        }

        /// <summary>
        /// Test that we can read a row that has a GUID key
        /// </summary>
        /// <param name="dataContextType"></param>
        /// <returns></returns>
        [TestCase(typeof(InMemoryDataContext))]
        [TestCase(typeof(SqlServerDataContext))]
        public async Task Read_With_Guid_Key(Type dataContextType)
        {
            // Arrange
            using IDataContext dataContext = DataContextTestHelper.SetupDataContext(dataContextType);
            Uid unique1 = await dataContext.Create(new Uid() { Value = "It's Unique" });

            // Act
            Uid read = await dataContext.Read<Uid>(unique1.Id);

            // Assert
            Assert.That(read.Id, Is.EqualTo(unique1.Id));
            Assert.That(read.Value, Is.EqualTo(unique1.Value));
        }

        /// <summary>
        /// Test that we can read a single row using a predicate.
        /// </summary>
        /// <param name="dataContextType"></param>
        /// <returns></returns>
        [TestCase(typeof(InMemoryDataContext))]
        [TestCase(typeof(SqlServerDataContext))]
        [TestCase(typeof(MySqlDataContext))]
        [TestCase(typeof(SqliteDataContext))]
        public async Task Read_With_Predicates(Type dataContextType)
        {
            // Arrange
            using IDataContext dataContext = DataContextTestHelper.SetupDataContext(dataContextType);
            await dataContext.Create(new City() { CityCode = "PUP", CityName = "Portsmouth", Area = "Hampshire" });
            await dataContext.Create(new City() { CityCode = "SOU", CityName = "Southampton", Area = "Hampshire" });
            await dataContext.Create(new City() { CityCode = "CHI", CityName = "Chichester", Area = "West Sussex" });
            await dataContext.Create(new City() { CityCode = "BOU", CityName = "Bournemouth", Area = "Dorset" });

            // Act
            City city = await dataContext.Read<City>(Equal<City>(x => x.Area, "West Sussex"));

            // Assert
            Assert.That(city.CityCode, Is.EqualTo("CHI"));
        }

        /// <summary>
        /// Test that when we try to read a single row with predicates that if the row does not exist null is returned.
        /// </summary>
        /// <param name="dataContextType"></param>
        /// <returns></returns>
        [TestCase(typeof(InMemoryDataContext))]
        [TestCase(typeof(SqlServerDataContext))]
        [TestCase(typeof(MySqlDataContext))]
        [TestCase(typeof(SqliteDataContext))]
        public async Task Read_With_Predicates_Return_Null(Type dataContextType)
        {
            // Arrange
            using IDataContext dataContext = DataContextTestHelper.SetupDataContext(dataContextType);
            await dataContext.Create(new City() { CityCode = "PUP", CityName = "Portsmouth", Area = "Hampshire" });
            await dataContext.Create(new City() { CityCode = "SOU", CityName = "Southampton", Area = "Hampshire" });
            await dataContext.Create(new City() { CityCode = "CHI", CityName = "Chichester", Area = "West Sussex" });
            await dataContext.Create(new City() { CityCode = "BOU", CityName = "Bournemouth", Area = "Dorset" });

            // Act
            City city = await dataContext.Read<City>(Equal<City>(x => x.Area, "East Sussex"));

            // Assert
            Assert.That(city, Is.Null);
        }

        /// <summary>
        /// Test that when we try to read a single row with predicates that if multiple rows are returned then an
        /// <see cref="ArgumentException"/> is thrown
        /// </summary>
        /// <param name="dataContextType"></param>
        /// <returns></returns>
        [TestCase(typeof(InMemoryDataContext))]
        [TestCase(typeof(SqlServerDataContext))]
        [TestCase(typeof(MySqlDataContext))]
        [TestCase(typeof(SqliteDataContext))]
        public async Task Read_With_Predicates_Multiple_Results_Throws_Argument_Exception(Type dataContextType)
        {
            // Arrange
            using IDataContext dataContext = DataContextTestHelper.SetupDataContext(dataContextType);
            await dataContext.Create(new City() { CityCode = "PUP", CityName = "Portsmouth", Area = "Hampshire" });
            await dataContext.Create(new City() { CityCode = "SOU", CityName = "Southampton", Area = "Hampshire" });
            await dataContext.Create(new City() { CityCode = "CHI", CityName = "Chichester", Area = "West Sussex" });
            await dataContext.Create(new City() { CityCode = "BOU", CityName = "Bournemouth", Area = "Dorset" });

            // Act / Assert
            Assert.ThrowsAsync<ArgumentException>(async() => await dataContext.Read<City>(Equal<City>(x => x.Area, "Hampshire")));
        }

        /// <summary>
        /// Test that when we read rows with a predicate that has candidates in more than one partition, that only 
        /// the rows from the requested partition are returned.
        /// </summary>
        /// <returns></returns>
        [TestCase(typeof(InMemoryDataContext))]
        [TestCase(typeof(SqlServerDataContext))]
        [TestCase(typeof(MySqlDataContext))]
        [TestCase(typeof(SqliteDataContext))]
        public async Task Read_By_Id_With_Sequential_Partition_Key(Type dataContextType)
        {
            // Arrange
            using IDataContext dataContext = DataContextTestHelper.SetupDataContext(dataContextType);
            ProductPartition1 p1 = await dataContext.Create(new ProductPartition1() { Name = "Test", IsForSale = true });
            ProductPartition2 p2 = await dataContext.Create(new ProductPartition2() { Name = "Test", IsForSale = true });

            // Act
            ProductPartition1 one = await dataContext.Read<ProductPartition1>(p1.Id);
            ProductPartition2 two = await dataContext.Read<ProductPartition2>(p2.Id);

            // Assert
            Assert.That(one, Is.Not.Null);
            Assert.That(one.Id, Is.EqualTo(p1.Id));

            Assert.That(two, Is.Not.Null);
            Assert.That(two.Id, Is.EqualTo(p2.Id));
        }

        [TestCase(typeof(InMemoryDataContext))]
        [TestCase(typeof(SqlServerDataContext))]
        [TestCase(typeof(MySqlDataContext))]
        [TestCase(typeof(SqliteDataContext))]
        public async Task Read_By_Id_With_Soft_Delete_And_Sequential_Partition_Key(Type dataContextType)
        {
            // Arrange
            using IDataContext dataContext = DataContextTestHelper.SetupDataContext(dataContextType);
            SoftDeletePartition1 sd1 = await dataContext.Create(new SoftDeletePartition1() { Value = "Test 1" });
            SoftDeletePartition2 sd2 = await dataContext.Create(new SoftDeletePartition2() { Value = "Test 2" });

            // Act
            SoftDeletePartition1 one = await dataContext.Read<SoftDeletePartition1>(sd1.Id);
            SoftDeletePartition2 two = await dataContext.Read<SoftDeletePartition2>(sd2.Id);

            // Assert
            Assert.That(one, Is.Not.Null);
            Assert.That(one.Id, Is.EqualTo(sd1.Id));

            Assert.That(two, Is.Not.Null);
            Assert.That(two.Id, Is.EqualTo(sd2.Id));
        }

        /// <summary>
        /// Test that when we read rows with a predicate that has candidates in more than one partition, that only 
        /// the rows from the requested partition are returned.
        /// </summary>
        /// <returns></returns>
        [TestCase(typeof(InMemoryDataContext))]
        [TestCase(typeof(SqlServerDataContext))]
        [TestCase(typeof(MySqlDataContext))]
        [TestCase(typeof(SqliteDataContext))]
        public async Task Read_With_Sequential_Partition_Key(Type dataContextType)
        {
            // Arrange
            using IDataContext dataContext = DataContextTestHelper.SetupDataContext(dataContextType);
            await dataContext.Create(new ProductPartition1() { Name = "Test", IsForSale = true });
            await dataContext.Create(new ProductPartition1() { Name = "Test 2", IsForSale = true });
            await dataContext.Create(new ProductPartition2() { Name = "Test", IsForSale = true });
            await dataContext.Create(new ProductPartition2() { Name = "Test 2", IsForSale = true });

            // Act
            IEnumerable<ProductPartition2> twos = await dataContext.ReadList<ProductPartition2>(new { IsForSale = true });

            // Assert
            Assert.That(twos.Count(), Is.EqualTo(2));
            Assert.That(twos.All(x => x.Id >= 100001));
        }
    }
}
