using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.Data.SqlClient;

using Entatea.InMemory;
using Entatea.MySql;
using static Entatea.Predicate.PredicateBuilder;
using Entatea.Sqlite;
using Entatea.SqlServer;

using Entatea.Tests.Helpers;
using Entatea.Tests.Entities;

using Dapper;
using MySql.Data.MySqlClient;
using NUnit.Framework;
using System.Net.WebSockets;

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
            using IDataContext dataContext = DataContextTestHelper.SetupDataContext(dataContextType);
            City city = await dataContext.Create<City>(new City() { CityCode = "BAS", CityName = "Basingstoke", Area = "Hampshire" });

            // Act
            await dataContext.Delete<City>(new { city.CityId, });

            // Assert
            Assert.That(await dataContext.Read<City>(new { city.CityId }), Is.Null);
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
            using IDataContext dataContext = DataContextTestHelper.SetupDataContext(dataContextType);
            City city = await dataContext.Create<City>(new City() { CityCode = "BAS", CityName = "Basingstoke", Area = "Hampshire" });

            // Act
            await dataContext.Delete<City>(city.CityId);

            // Assert
            Assert.That(await dataContext.Read<City>(new { city.CityId }), Is.Null);
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
            using IDataContext dataContext = DataContextTestHelper.SetupDataContext(dataContextType);
            await dataContext.Create<City>(new City() { CityCode = "BAS", CityName = "Basingstoke", Area = "Hampshire" });
            await dataContext.Create<City>(new City() { CityCode = "PUP", CityName = "Portsmouth", Area = "Hampshire" });
            await dataContext.Create<City>(new City() { CityCode = "BOU", CityName = "Bournemouth", Area = "Dorset" });
            await dataContext.Create<City>(new City() { CityCode = "HAV", CityName = "Havant", Area = "Hampshire" });

            // Act
            await dataContext.DeleteList<City>(new { Area = "Hampshire" });

            // Assert
            Assert.That((await dataContext.ReadList<City>(new { Area = "Hampshire" })).Count(), Is.EqualTo(0));
            Assert.That((await dataContext.ReadAll<City>()).Count(), Is.EqualTo(1));   
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
            using IDataContext dataContext = DataContextTestHelper.SetupDataContext(dataContextType);
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
            using IDataContext dataContext = DataContextTestHelper.SetupDataContext(dataContextType);
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
            using IDataContext dataContext = DataContextTestHelper.SetupDataContext(dataContextType);
            SoftDelete softDelete = await dataContext.Create(new SoftDelete());

            // Act
            await dataContext.Delete<SoftDelete>(new { softDelete.SoftDeleteId, });

            // Assert
            Assert.That(await dataContext.Read<SoftDelete>(new { softDelete.SoftDeleteId }), Is.Null);

            if (dataContextType == typeof(InMemoryDataContext))
            {
                InMemoryDataContext inMemoryData = (InMemoryDataContext)dataContext;

                var noAttResult = inMemoryData.GetRawData<SoftDelete>().SingleOrDefault(x => x.SoftDeleteId == softDelete.SoftDeleteId);
                Assert.That(noAttResult, Is.Not.Null);
                Assert.That(noAttResult.RecordStatus, Is.EqualTo(0));
            }
            else
            {
                var noAttResult = await dataContext.Read<SoftDeleteNoAttribute>(softDelete.SoftDeleteId);
                Assert.That(noAttResult.RecordStatus, Is.EqualTo(0));
            }
        }

        /// <summary>
        /// Test that we can hard delete a single entity.
        /// </summary>
        [TestCase(typeof(InMemoryDataContext))]
        [TestCase(typeof(SqlServerDataContext))]
        [TestCase(typeof(MySqlDataContext))]
        [TestCase(typeof(SqliteDataContext))]
        public async Task Hard_Delete_Entity(Type dataContextType)
        {
            // Arrange
            using IDataContext dataContext = DataContextTestHelper.SetupDataContext(dataContextType);
            SoftDeleteNoAttribute value = await dataContext.Create(new SoftDeleteNoAttribute());

            // Act
            await dataContext.Delete<SoftDeleteNoAttribute>(new { value.SoftDeleteId, });

            // Assert
            var result = await dataContext.Read<SoftDeleteNoAttribute>(new { value.SoftDeleteId });
            Assert.That(result, Is.Null);       
        }

        /// <summary>
        /// Test that we can soft delete a single entity.
        /// </summary>
        [TestCase(typeof(InMemoryDataContext))]
        [TestCase(typeof(SqlServerDataContext))]
        [TestCase(typeof(MySqlDataContext))]
        [TestCase(typeof(SqliteDataContext))]
        public async Task Soft_Delete_Short_Entity(Type dataContextType)
        {
            // Arrange
            using IDataContext dataContext = DataContextTestHelper.SetupDataContext(dataContextType);
            SoftDeleteShort softDelete = await dataContext.Create(new SoftDeleteShort());

            // Act
            await dataContext.Delete<SoftDeleteShort>(new { softDelete.SoftDeleteId, });

            // Assert
            Assert.That(await dataContext.Read<SoftDeleteShort>(new { softDelete.SoftDeleteId }), Is.Null);

            if (dataContextType == typeof(InMemoryDataContext))
            {
                InMemoryDataContext inMemoryData = (InMemoryDataContext)dataContext;

                var noAttResult = inMemoryData.GetRawData<SoftDeleteShort>().SingleOrDefault(x => x.SoftDeleteId == softDelete.SoftDeleteId);
                Assert.That(noAttResult, Is.Not.Null);
                Assert.That(noAttResult.RecordStatus, Is.EqualTo(0));
            }
            else
            {
                var noAttResult = await dataContext.Read<SoftDeleteShortNoAttribute>(softDelete.SoftDeleteId);
                Assert.That(noAttResult.RecordStatus, Is.EqualTo(0));
            }
        }

        /// <summary>
        /// Test that we can hard delete a single entity.
        /// </summary>
        [TestCase(typeof(InMemoryDataContext))]
        [TestCase(typeof(SqlServerDataContext))]
        [TestCase(typeof(MySqlDataContext))]
        [TestCase(typeof(SqliteDataContext))]
        public async Task Hard_Delete_Short_Entity(Type dataContextType)
        {
            // Arrange
            using IDataContext dataContext = DataContextTestHelper.SetupDataContext(dataContextType);
            SoftDeleteShortNoAttribute value = await dataContext.Create(new SoftDeleteShortNoAttribute());

            // Act
            await dataContext.Delete<SoftDeleteShortNoAttribute>(new { value.SoftDeleteId, });

            // Assert
            Assert.That(await dataContext.Read<SoftDeleteShortNoAttribute>(new { value.SoftDeleteId }), Is.Null);
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
            using IDataContext dataContext = DataContextTestHelper.SetupDataContext(dataContextType);
            SoftDelete sd1 = await dataContext.Create(new SoftDelete());
            SoftDelete sd2 = await dataContext.Create(new SoftDelete());
            SoftDelete sd3 = await dataContext.Create(new SoftDelete());

            // Act
            await dataContext.DeleteList<SoftDelete>(
                In<SoftDelete>(x => x.SoftDeleteId, new int[] { sd2.SoftDeleteId, sd3.SoftDeleteId }));

            // Assert
            Assert.That((await dataContext.ReadList<SoftDelete>(
                In<SoftDelete>(x => x.SoftDeleteId, new int[] { sd2.SoftDeleteId, sd3.SoftDeleteId }))).Count(),
                Is.EqualTo(0));
            Assert.That((await dataContext.ReadAll<SoftDelete>()).Count(), Is.EqualTo(1));

            if (DataContextTestHelper.IsInMemory(dataContextType)) 
            {
                InMemoryDataContext inMemory = (InMemoryDataContext)dataContext;
                var softDeleted = inMemory.GetRawData<SoftDelete>()
                    .Where(x => x.SoftDeleteId == sd2.SoftDeleteId || x.SoftDeleteId == sd3.SoftDeleteId)
                    .AsList();

                Assert.That(softDeleted.Select(x => x.RecordStatus), Is.All.EqualTo(0));
            }
            else
            {
                IEnumerable<SoftDeleteNoAttribute> deleted = await dataContext.ReadList<SoftDeleteNoAttribute>(
                    In<SoftDeleteNoAttribute>(x => x.SoftDeleteId, new[] { sd2.SoftDeleteId, sd3.SoftDeleteId }));
                Assert.That(deleted.Select(x => x.RecordStatus), Is.All.EqualTo(0));
            }
        }

        /// <summary>
        /// Test that we can hard delete a multiple entities using a where condition.
        /// </summary>
        [TestCase(typeof(InMemoryDataContext))]
        [TestCase(typeof(SqlServerDataContext))]
        [TestCase(typeof(MySqlDataContext))]
        [TestCase(typeof(SqliteDataContext))]
        public async Task Hard_Delete_List_Of_Entities(Type dataContextType)
        {
            // Arrange
            using IDataContext dataContext = DataContextTestHelper.SetupDataContext(dataContextType);
            SoftDeleteNoAttribute sd1 = await dataContext.Create(new SoftDeleteNoAttribute());
            SoftDeleteNoAttribute sd2 = await dataContext.Create(new SoftDeleteNoAttribute());
            SoftDeleteNoAttribute sd3 = await dataContext.Create(new SoftDeleteNoAttribute());

            // Act
            await dataContext.DeleteList<SoftDeleteNoAttribute>(
                In<SoftDeleteNoAttribute>(x => x.SoftDeleteId, new int[] { sd2.SoftDeleteId, sd3.SoftDeleteId }));

            // Assert
            Assert.That((await dataContext.ReadList<SoftDeleteNoAttribute>(
                In<SoftDeleteNoAttribute>(x => x.SoftDeleteId, new int[] { sd2.SoftDeleteId, sd3.SoftDeleteId }))).Count(),
                Is.EqualTo(0));
            Assert.That((await dataContext.ReadAll<SoftDeleteNoAttribute>()).Count(), Is.EqualTo(1));

        }

        /// <summary>
        /// Test that when we have two items with the same name and different discriminator
        /// When we delete by name the discriminator is taken into account and only the correct record is deleted
        /// </summary>
        /// <param name="dataContextType"></param>
        /// <returns></returns>
        [TestCase(typeof(InMemoryDataContext))]
        [TestCase(typeof(SqlServerDataContext))]
        [TestCase(typeof(MySqlDataContext))]
        [TestCase(typeof(SqliteDataContext))]
        public async Task Delete_Discriminator_Entity(Type dataContextType)
        {
            // Arrange
            using IDataContext dataContext = DataContextTestHelper.SetupDataContext(dataContextType);
            DiscriminatorContact contact = await dataContext.Create(new DiscriminatorContact() { Name = "Paul" });
            DiscriminatorCompany company = await dataContext.Create(new DiscriminatorCompany() { Name = "Paul" });

            // Act
            await dataContext.DeleteList<DiscriminatorContact>(Equal<DiscriminatorContact>(x => x.Name, "Paul"));

            // Assert
            var temp = await dataContext.ReadList<DiscriminatorContact>(In<DiscriminatorContact>(x => x.Name, "Paul"));
            Assert.That((await dataContext.ReadList<DiscriminatorContact>(In<DiscriminatorContact>(x => x.Name, "Paul"))).Count(),
                Is.EqualTo(0));
            Assert.That((await dataContext.ReadList<DiscriminatorCompany>(In<DiscriminatorCompany>(x => x.Name, "Paul"))).Count(),
                Is.EqualTo(1));
        }

        /// <summary>
        /// Test that when we delete rows with a predicate that has candidates in more than one partition, that only 
        /// the rows from the requested partition are deleted.
        /// </summary>
        /// <returns></returns>
        [TestCase(typeof(InMemoryDataContext))]
        [TestCase(typeof(SqlServerDataContext))]
        [TestCase(typeof(MySqlDataContext))]
        [TestCase(typeof(SqliteDataContext))]
        public async Task Delete_With_Sequential_Partition_Key(Type dataContextType)
        {
            // Arrange
            using IDataContext dataContext = DataContextTestHelper.SetupDataContext(dataContextType);
            await dataContext.Create(new ProductPartition1() { Name = "Test", IsForSale = true });
            await dataContext.Create(new ProductPartition1() { Name = "Test 2", IsForSale = true });
            await dataContext.Create(new ProductPartition2() { Name = "Test", IsForSale = true });
            await dataContext.Create(new ProductPartition2() { Name = "Test 2", IsForSale = true });

            // Act
            await dataContext.DeleteList<ProductPartition2>(new { IsForSale = true });

            // Assert
            Assert.That((await dataContext.ReadAll<ProductPartition2>()).Any(), Is.False);
            Assert.That((await dataContext.ReadAll<ProductPartition1>()).Any(), Is.True);
        }
    }
}
