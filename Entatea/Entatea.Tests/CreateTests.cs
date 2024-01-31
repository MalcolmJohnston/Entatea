using System;
using System.Threading.Tasks;

using Entatea.InMemory;
using Entatea.MySql;
using Entatea.SqlServer;
using Entatea.Sqlite;

using Entatea.Tests.Helpers;
using Entatea.Tests.Entities;

using NUnit.Framework;

namespace Entatea.Tests
{
    [TestFixture]
    public class CreateTests : BaseTest
    {
        /// <summary>
        /// Test that we can insert an entity that has single key which is an identity column.
        /// </summary>
        [TestCase(typeof(InMemoryDataContext))]
        [TestCase(typeof(SqlServerDataContext))]
        [TestCase(typeof(MySqlDataContext))]
        [TestCase(typeof(SqliteDataContext))]
        public async Task Insert_With_Identity(Type dataContextType)
        {
            // Arrange
            using IDataContext dataContext = DataContextTestHelper.SetupDataContext(dataContextType);

            // Act
            City city = await dataContext.Create(new City() { CityCode = "BRI", CityName = "Brighton", Area = "Sussex" });

            // Assert
            Assert.Greater(city.CityId, 0);
            Assert.AreEqual("BRI", city.CityCode);
            Assert.AreEqual("Brighton", city.CityName);
            Assert.AreEqual("Sussex", city.Area);
        }

        /// <summary>
        /// Test that we can insert an entity which has a single key that is manually derived
        /// </summary>
        [TestCase(typeof(InMemoryDataContext))]
        [TestCase(typeof(SqlServerDataContext))]
        [TestCase(typeof(MySqlDataContext))]
        [TestCase(typeof(SqliteDataContext))]
        public async Task Insert_With_Assigned_Key(Type dataContextType)
        {
            // Arrange
            using IDataContext dataContext = DataContextTestHelper.SetupDataContext(dataContextType);

            // Act
            CityManual city = await dataContext.Create(new CityManual() { CityCode = "BRI", CityName = "Brighton" });

            // Assert
            Assert.AreEqual("BRI", city.CityCode);
            Assert.AreEqual("Brighton", city.CityName);
        }

        /// <summary>
        /// Test that we can insert an entity which has a sequentially calculated key
        /// </summary>
        [TestCase(typeof(InMemoryDataContext))]
        [TestCase(typeof(SqlServerDataContext))]
        [TestCase(typeof(MySqlDataContext))]
        [TestCase(typeof(SqliteDataContext))]
        public async Task Insert_With_Sequential_Key(Type dataContextType)
        {
            // Arrange
            using IDataContext dataContext = DataContextTestHelper.SetupDataContext(dataContextType);

            // Act
            CitySequential city = await dataContext.Create(new CitySequential() { CityCode = "BRI", CityName = "Brighton" });

            // Assert
            Assert.Greater(city.CityId, 0);
            Assert.AreEqual("BRI", city.CityCode);
            Assert.AreEqual("Brighton", city.CityName);
        }

        /// <summary>
        /// Test that we can insert an entity which has one manual key and one sequential key
        /// </summary>
        /// <returns></returns>
        [TestCase(typeof(InMemoryDataContext))]
        [TestCase(typeof(SqlServerDataContext))]
        [TestCase(typeof(MySqlDataContext))]
        [TestCase(typeof(SqliteDataContext))]
        public async Task Insert_With_Composite_Key_One_Assigned_And_One_Sequential(Type dataContextType)
        {
            // Arrange
            using IDataContext dataContext = DataContextTestHelper.SetupDataContext(dataContextType);

            // Act
            AssignedAndSequential one = await dataContext.Create(new AssignedAndSequential() { AssignedId = 1, Heading = "One" });
            AssignedAndSequential two = await dataContext.Create(new AssignedAndSequential() { AssignedId = 1, Heading = "Two" });

            // Assert
            Assert.AreEqual(1, one.SequentialId);
            Assert.AreEqual(2, two.SequentialId);
        }

        /// <summary>
        /// Test that we can insert an entity which has two manual keys and one sequential key
        /// </summary>
        /// <returns></returns>
        [TestCase(typeof(InMemoryDataContext))]
        [TestCase(typeof(SqlServerDataContext))]
        [TestCase(typeof(MySqlDataContext))]
        [TestCase(typeof(SqliteDataContext))]
        public async Task Insert_With_Composite_Key_Two_Assigned_And_One_Sequential(Type dataContextType)
        {
            // Arrange
            using IDataContext dataContext = DataContextTestHelper.SetupDataContext(dataContextType);

            // Act
            AssignedPairAndSequential oneOneOne = await dataContext.Create(new AssignedPairAndSequential() { FirstAssignedId = 1, SecondAssignedId = 1, Heading = "One" });
            AssignedPairAndSequential oneOneTwo = await dataContext.Create(new AssignedPairAndSequential() { FirstAssignedId = 1, SecondAssignedId = 1, Heading = "Two" });
            AssignedPairAndSequential oneTwoOne = await dataContext.Create(new AssignedPairAndSequential() { FirstAssignedId = 1, SecondAssignedId = 2, Heading = "One" });

            // Assert
            Assert.AreEqual(1, oneOneOne.SequentialId);
            Assert.AreEqual(2, oneOneTwo.SequentialId);
            Assert.AreEqual(1, oneTwoOne.SequentialId);
        }

        /// <summary>
        /// Test that insert and update date properties that have been marked as Date Stamps are automatically set
        /// to the time now on insert.
        /// </summary>
        /// <returns></returns>
        [TestCase(typeof(InMemoryDataContext))]
        [TestCase(typeof(SqlServerDataContext))]
        [TestCase(typeof(MySqlDataContext))]
        [TestCase(typeof(SqliteDataContext))]
        public async Task Insert_With_Datestamp(Type dataContextType)
        {
            // Arrange
            using IDataContext dataContext = DataContextTestHelper.SetupDataContext(dataContextType);

            // Act
            DateTime now = DateTime.Now;
            DateStamp row = await dataContext.Create(new DateStamp() { Name = "Key", Value = "Value" });

            // Assert
            Assert.AreEqual("Key", row.Name);
            Assert.AreEqual("Value", row.Value);
            Assert.AreEqual(row.InsertDate, row.UpdateDate);
            Assert.That(row.InsertDate, Is.EqualTo(now).Within(TimeSpan.FromSeconds(1)));
        }

        [TestCase(typeof(InMemoryDataContext))]
        [TestCase(typeof(SqlServerDataContext))]
        [TestCase(typeof(MySqlDataContext))]
        [TestCase(typeof(SqliteDataContext))]
        public async Task Insert_With_Soft_Delete(Type dataContextType)
        {
            // Arrange
            using IDataContext dataContext = DataContextTestHelper.SetupDataContext(dataContextType);

            // Act
            SoftDelete softDelete = await dataContext.Create(new SoftDelete()).ConfigureAwait(false);

            // Assert
            Assert.Greater(softDelete.SoftDeleteId, 0);
            Assert.AreEqual(1, softDelete.RecordStatus);
        }

        [TestCase(typeof(InMemoryDataContext))]
        [TestCase(typeof(SqlServerDataContext))]
        [TestCase(typeof(MySqlDataContext))]
        [TestCase(typeof(SqliteDataContext))]
        public async Task Insert_With_Soft_Delete_Short(Type dataContextType)
        {
            // Arrange
            using IDataContext dataContext = DataContextTestHelper.SetupDataContext(dataContextType);

            // Act
            SoftDeleteShort softDelete = await dataContext.Create(new SoftDeleteShort()).ConfigureAwait(false);

            // Assert
            Assert.Greater(softDelete.SoftDeleteId, 0);
            Assert.AreEqual(1, softDelete.RecordStatus);
        }

        /// <summary>
        /// Test that we can create records with a discriminator attribute and their discriminator property is set accordingly
        /// </summary>
        /// <param name="dataContextType"></param>
        /// <returns></returns>
        [TestCase(typeof(InMemoryDataContext))]
        [TestCase(typeof(SqlServerDataContext))]
        [TestCase(typeof(MySqlDataContext))]
        [TestCase(typeof(SqliteDataContext))]
        public async Task Insert_With_Discriminator(Type dataContextType)
        {
            // Arrange
            using IDataContext dataContext = DataContextTestHelper.SetupDataContext(dataContextType);

            // Act
            DiscriminatorContact contact = await dataContext.Create(new DiscriminatorContact() { Name = "Paul" });
            DiscriminatorCompany company = await dataContext.Create(new DiscriminatorCompany() { Name = "The Beatles" });

            // Assert
            Assert.Greater(contact.ContactId, 0);
            Assert.AreEqual(DiscriminatorType.Contact, contact.DiscriminatorType);
            Assert.AreEqual("Paul", contact.Name);

            Assert.Greater(company.CompanyId, 0);
            Assert.AreEqual(DiscriminatorType.Company, company.DiscriminatorType);
            Assert.AreEqual("The Beatles", company.Name);
        }

        /// <summary>
        /// Test that we can insert a row that has a GUID key
        /// </summary>
        /// <param name="dataContextType"></param>
        /// <returns></returns>
        [TestCase(typeof(InMemoryDataContext))]
        [TestCase(typeof(SqlServerDataContext))]
        public async Task Insert_With_Guid_Key(Type dataContextType)
        {
            // Arrange
            using IDataContext dataContext = DataContextTestHelper.SetupDataContext(dataContextType);

            // Act
            Uid unique1 = await dataContext.Create(new Uid() { Value = "It's Unique" });
            Uid unique2 = await dataContext.Create(new Uid() { Value = "So is this." });

            // Assert
            Assert.NotNull(unique1.Id);
            Assert.AreEqual("It's Unique", unique1.Value);

            Assert.NotNull(unique2.Id);
            Assert.AreEqual("So is this.", unique2.Value);
            Assert.AreNotEqual(unique1.Id, unique2.Id);
        }

        /// <summary>
        /// Test that we can insert with a sequential partition key when there are no rows
        /// </summary>
        /// <param name="dataContextType"></param>
        /// <returns></returns>
        [TestCase(typeof(InMemoryDataContext))]
        [TestCase(typeof(MySqlDataContext))]
        [TestCase(typeof(SqliteDataContext))]
        [TestCase(typeof(SqlServerDataContext))]
        public async Task Insert_With_Sequential_Partition_Key_No_Existing_Rows(Type dataContextType)
        {
            // Arrange
            using IDataContext dataContext = DataContextTestHelper.SetupDataContext(dataContextType);

            // Act
            ProductPartition1 partition1 = await dataContext.Create(new ProductPartition1() { Name = "1" });
            ProductPartition2 partition2 = await dataContext.Create(new ProductPartition2() { Name = "100001" });
            ProductPartition3 partition3 = await dataContext.Create(new ProductPartition3() { Name = "30000" });

            // Assert
            Assert.AreEqual(1, partition1.Id);
            Assert.AreEqual(100001, partition2.Id);
            Assert.AreEqual(30000, partition3.Id);
        }

        // <summary>
        /// Test that we can insert with a sequential partition key when there are already rows
        /// </summary>
        /// <param name="dataContextType"></param>
        /// <returns></returns>
        [TestCase(typeof(InMemoryDataContext))]
        [TestCase(typeof(MySqlDataContext))]
        [TestCase(typeof(SqliteDataContext))]
        [TestCase(typeof(SqlServerDataContext))]
        public async Task Insert_With_Sequential_Partition_Key_Existing_Rows(Type dataContextType)
        {
            // Arrange
            int existingRows = 5;
            using IDataContext dataContext = DataContextTestHelper.SetupDataContext(dataContextType);

            for (int i = 1; i <= existingRows; i++)
            {
                await dataContext.Create(new ProductPartition1() { Name = i.ToString() });
                await dataContext.Create(new ProductPartition2() { Name = (i + 100000).ToString() });
                await dataContext.Create(new ProductPartition3() { Name = (i + 29999).ToString() });
            }
            
            // Act
            ProductPartition1 p1 = await dataContext.Create(new ProductPartition1() { Name = (existingRows + 1).ToString() });
            ProductPartition2 p2 = await dataContext.Create(new ProductPartition2() { Name = (existingRows + 100001).ToString() });
            ProductPartition3 p3 = await dataContext.Create(new ProductPartition3() { Name = (existingRows + 30000).ToString() });

            // Assert
            Assert.AreEqual(existingRows + 1, p1.Id);
            Assert.AreEqual(existingRows + 100001, p2.Id);
            Assert.AreEqual(existingRows + + 30000, p3.Id);
        }
    }
}