﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

using NUnit.Framework;

using Entatea.InMemory;
using Entatea.MySql;
using Entatea.SqlServer;
using Entatea.Sqlite;

using Entatea.Tests.Helpers;
using Entatea.Tests.Entities;

namespace Entatea.Tests
{
    [TestFixture]
    public  class PagingTests : BaseTest
    {
        /// <summary>
        /// Test that we can get the first page when not specifying where conditions or a sort order.
        /// </summary>
        [TestCase(typeof(InMemoryDataContext))]
        [TestCase(typeof(SqlServerDataContext))]
        [TestCase(typeof(MySqlDataContext))]
        [TestCase(typeof(SqliteDataContext))]
        public async Task Read_Paged_First_Page(Type dataContextType)
        {
            // Arrange
            using IDataContext dataContext = DataContextTestHelper.SetupDataContext(dataContextType);
            await dataContext.Create(new City() { CityCode = "PUP", CityName = "Portsmouth", Area = "Hampshire" });
            await dataContext.Create(new City() { CityCode = "SOU", CityName = "Southampton", Area = "Hampshire" });
            await dataContext.Create(new City() { CityCode = "BAS", CityName = "Basingstoke", Area = "Hampshire" });
            await dataContext.Create(new City() { CityCode = "WIN", CityName = "Winchester", Area = "Hampshire" });
            await dataContext.Create(new City() { CityCode = "PTR", CityName = "Petersfield", Area = "Hampshire" });
            await dataContext.Create(new City() { CityCode = "CHI", CityName = "Chichester", Area = "West Sussex" });
            await dataContext.Create(new City() { CityCode = "BOG", CityName = "Bognor Regis", Area = "West Sussex" });
            await dataContext.Create(new City() { CityCode = "BOU", CityName = "Bournemouth", Area = "Dorset" });

            // Act
            PagedList<City> pagedList = await dataContext.ReadList<City>(null, null, 4, 1);

            // Assert
            Assert.That(pagedList.Rows.Count(), Is.EqualTo(4));
            Assert.That(pagedList.HasPrevious, Is.False);
            Assert.That(pagedList.HasNext, Is.True);
            Assert.That(pagedList.TotalPages, Is.EqualTo(2));
            Assert.That(pagedList.TotalRows, Is.EqualTo(8));
        }

        [TestCase(typeof(InMemoryDataContext))]
        [TestCase(typeof(SqlServerDataContext))]
        [TestCase(typeof(MySqlDataContext))]
        [TestCase(typeof(SqliteDataContext))]
        public async Task Read_Paged_Neither_First_Or_Last_Page(Type dataContextType)
        {
            // Arrange
            using IDataContext dataContext = DataContextTestHelper.SetupDataContext(dataContextType);
            await dataContext.Create(new City() { CityCode = "PUP", CityName = "Portsmouth", Area = "Hampshire" });
            await dataContext.Create(new City() { CityCode = "SOU", CityName = "Southampton", Area = "Hampshire" });
            await dataContext.Create(new City() { CityCode = "BAS", CityName = "Basingstoke", Area = "Hampshire" });
            await dataContext.Create(new City() { CityCode = "WIN", CityName = "Winchester", Area = "Hampshire" });
            await dataContext.Create(new City() { CityCode = "PTR", CityName = "Petersfield", Area = "Hampshire" });
            await dataContext.Create(new City() { CityCode = "CHI", CityName = "Chichester", Area = "West Sussex" });
            await dataContext.Create(new City() { CityCode = "BOG", CityName = "Bognor Regis", Area = "West Sussex" });
            await dataContext.Create(new City() { CityCode = "BOU", CityName = "Bournemouth", Area = "Dorset" });

            // Act
            PagedList<City> pagedList = await dataContext.ReadList<City>(null, null, 2, 2);

            // Assert
            Assert.That(pagedList.Rows.Count(), Is.EqualTo(2));
            Assert.That(pagedList.HasPrevious, Is.True);
            Assert.That(pagedList.HasNext, Is.True);
            Assert.That(pagedList.TotalPages, Is.EqualTo(4));
            Assert.That(pagedList.TotalRows, Is.EqualTo(8));
        }

        /// <summary>
        /// Test that we can get the last page when not specifying where conditions or a sort order.
        /// </summary>
        [TestCase(typeof(InMemoryDataContext))]
        [TestCase(typeof(SqlServerDataContext))]
        [TestCase(typeof(MySqlDataContext))]
        [TestCase(typeof(SqliteDataContext))]
        public async Task Read_Paged_Last_Page(Type dataContextType)
        {
            // Arrange
            using IDataContext dataContext = DataContextTestHelper.SetupDataContext(dataContextType);
            await dataContext.Create(new City() { CityCode = "PUP", CityName = "Portsmouth", Area = "Hampshire" });
            await dataContext.Create(new City() { CityCode = "SOU", CityName = "Southampton", Area = "Hampshire" });
            await dataContext.Create(new City() { CityCode = "BAS", CityName = "Basingstoke", Area = "Hampshire" });
            await dataContext.Create(new City() { CityCode = "WIN", CityName = "Winchester", Area = "Hampshire" });
            await dataContext.Create(new City() { CityCode = "PTR", CityName = "Petersfield", Area = "Hampshire" });
            await dataContext.Create(new City() { CityCode = "CHI", CityName = "Chichester", Area = "West Sussex" });
            await dataContext.Create(new City() { CityCode = "BOG", CityName = "Bognor Regis", Area = "West Sussex" });
            await dataContext.Create(new City() { CityCode = "BOU", CityName = "Bournemouth", Area = "Dorset" });

            // Arrange - read the first page to see how many pages there are
            int lastPage = (await dataContext.ReadList<City>(null, null, 3, 1)).TotalPages;

            // Act - read the last page
            PagedList<City> pagedList = await dataContext.ReadList<City>(null, null, 3, lastPage);

            // Assert
            Assert.That(pagedList.Rows.Count(), Is.EqualTo(pagedList.TotalRows - ((lastPage - 1) * 3)));
            Assert.That(pagedList.HasPrevious, Is.True);
            Assert.That(pagedList.HasNext, Is.False);
            Assert.That(pagedList.TotalPages, Is.EqualTo(3));
            Assert.That(pagedList.TotalRows, Is.EqualTo(8));
        }

        /// <summary>
        /// Test that we can get the first page when specifying where conditions but no sort order.
        /// </summary>
        [TestCase(typeof(InMemoryDataContext))]
        [TestCase(typeof(SqlServerDataContext))]
        [TestCase(typeof(MySqlDataContext))]
        [TestCase(typeof(SqliteDataContext))]
        public async Task Read_Paged_With_Where(Type dataContextType)
        {
            // Arrange
            using IDataContext dataContext = DataContextTestHelper.SetupDataContext(dataContextType);
            await dataContext.Create(new City() { CityCode = "PUP", CityName = "Portsmouth", Area = "Hampshire" });
            await dataContext.Create(new City() { CityCode = "SOU", CityName = "Southampton", Area = "Hampshire" });
            await dataContext.Create(new City() { CityCode = "BAS", CityName = "Basingstoke", Area = "Hampshire" });
            await dataContext.Create(new City() { CityCode = "WIN", CityName = "Winchester", Area = "Hampshire" });
            await dataContext.Create(new City() { CityCode = "PTR", CityName = "Petersfield", Area = "Hampshire" });
            await dataContext.Create(new City() { CityCode = "CHI", CityName = "Chichester", Area = "West Sussex" });
            await dataContext.Create(new City() { CityCode = "BOG", CityName = "Bognor Regis", Area = "West Sussex" });
            await dataContext.Create(new City() { CityCode = "BOU", CityName = "Bournemouth", Area = "Dorset" });

            // Act
            PagedList<City> pagedList = await dataContext.ReadList<City>(new { Area = "Hampshire" }, null, 4, 1);

            // Assert
            Assert.That(pagedList.Rows.Count(), Is.EqualTo(4));
            Assert.That(pagedList.HasPrevious, Is.False);
            Assert.That(pagedList.HasNext, Is.True);
            Assert.That(pagedList.TotalPages, Is.EqualTo(2));
            Assert.That(pagedList.TotalRows, Is.EqualTo(5));
        }

        /// <summary>
        /// Test that we can get the first page when specifying where conditions but no sort order.
        /// </summary>
        [TestCase(typeof(InMemoryDataContext))]
        [TestCase(typeof(SqlServerDataContext))]
        [TestCase(typeof(MySqlDataContext))]
        [TestCase(typeof(SqliteDataContext))]
        public async Task Read_Paged_With_Where_Dictionary(Type dataContextType)
        {
            // Arrange
            using IDataContext dataContext = DataContextTestHelper.SetupDataContext(dataContextType);
            await dataContext.Create(new City() { CityCode = "PUP", CityName = "Portsmouth", Area = "Hampshire" });
            await dataContext.Create(new City() { CityCode = "SOU", CityName = "Southampton", Area = "Hampshire" });
            await dataContext.Create(new City() { CityCode = "BAS", CityName = "Basingstoke", Area = "Hampshire" });
            await dataContext.Create(new City() { CityCode = "WIN", CityName = "Winchester", Area = "Hampshire" });
            await dataContext.Create(new City() { CityCode = "PTR", CityName = "Petersfield", Area = "Hampshire" });
            await dataContext.Create(new City() { CityCode = "CHI", CityName = "Chichester", Area = "West Sussex" });
            await dataContext.Create(new City() { CityCode = "BOG", CityName = "Bognor Regis", Area = "West Sussex" });
            await dataContext.Create(new City() { CityCode = "BOU", CityName = "Bournemouth", Area = "Dorset" });

            // Act
            PagedList<City> pagedList = await dataContext.ReadList<City>(
                new Dictionary<string, object>() { { "Area", "Hampshire" } },
                null,
                4,
                1);

            // Assert
            Assert.That(pagedList.Rows.Count(), Is.EqualTo(4));
            Assert.That(pagedList.HasPrevious, Is.False);
            Assert.That(pagedList.HasNext, Is.True);
            Assert.That(pagedList.TotalPages, Is.EqualTo(2));
            Assert.That(pagedList.TotalRows, Is.EqualTo(5));
        }

        /// <summary>
        /// Test that we can get the rows in the required order.
        /// </summary>
        [TestCase(typeof(InMemoryDataContext))]
        [TestCase(typeof(SqlServerDataContext))]
        [TestCase(typeof(MySqlDataContext))]
        [TestCase(typeof(SqliteDataContext))]
        public async Task Read_Paged_With_OrderBy(Type dataContextType)
        {
            // Arrange
            using IDataContext dataContext = DataContextTestHelper.SetupDataContext(dataContextType);
            await dataContext.Create(new City() { CityCode = "PUP", CityName = "Portsmouth", Area = "Hampshire" });
            await dataContext.Create(new City() { CityCode = "SOU", CityName = "Southampton", Area = "Hampshire" });
            await dataContext.Create(new City() { CityCode = "BAS", CityName = "Basingstoke", Area = "Hampshire" });
            await dataContext.Create(new City() { CityCode = "WIN", CityName = "Winchester", Area = "Hampshire" });
            await dataContext.Create(new City() { CityCode = "PTR", CityName = "Petersfield", Area = "Hampshire" });
            await dataContext.Create(new City() { CityCode = "CHI", CityName = "Chichester", Area = "West Sussex" });
            await dataContext.Create(new City() { CityCode = "BOG", CityName = "Bognor Regis", Area = "West Sussex" });
            await dataContext.Create(new City() { CityCode = "BOU", CityName = "Bournemouth", Area = "Dorset" });

            // Act
            PagedList<City> pagedList = await dataContext.ReadList<City>(
                null,
                new { Area = SortOrder.Descending, CityName = SortOrder.Ascending },
                8,
                1);

            // Assert
            string[] expectedOrder = new string[]
            {
                    "Bognor Regis",
                    "Chichester",
                    "Basingstoke",
                    "Petersfield",
                    "Portsmouth",
                    "Southampton",
                    "Winchester",
                    "Bournemouth"
            };

            Assert.That(pagedList.Rows.Select(x => x.CityName).SequenceEqual(expectedOrder), Is.True);
        }

        /// <summary>
        /// Test that we can get the rows in the required order.
        /// </summary>
        [TestCase(typeof(InMemoryDataContext))]
        [TestCase(typeof(SqlServerDataContext))]
        [TestCase(typeof(MySqlDataContext))]
        [TestCase(typeof(SqliteDataContext))]
        public async Task Read_Paged_With_OrderBy_Dictionary(Type dataContextType)
        {
            // Arrange
            using IDataContext dataContext = DataContextTestHelper.SetupDataContext(dataContextType);
            await dataContext.Create(new City() { CityCode = "PUP", CityName = "Portsmouth", Area = "Hampshire" });
            await dataContext.Create(new City() { CityCode = "SOU", CityName = "Southampton", Area = "Hampshire" });
            await dataContext.Create(new City() { CityCode = "BAS", CityName = "Basingstoke", Area = "Hampshire" });
            await dataContext.Create(new City() { CityCode = "WIN", CityName = "Winchester", Area = "Hampshire" });
            await dataContext.Create(new City() { CityCode = "PTR", CityName = "Petersfield", Area = "Hampshire" });
            await dataContext.Create(new City() { CityCode = "CHI", CityName = "Chichester", Area = "West Sussex" });
            await dataContext.Create(new City() { CityCode = "BOG", CityName = "Bognor Regis", Area = "West Sussex" });
            await dataContext.Create(new City() { CityCode = "BOU", CityName = "Bournemouth", Area = "Dorset" });

            // Act
            PagedList<City> pagedList = await dataContext.ReadList<City>(
                null,
                new Dictionary<string, SortOrder>() { { "Area", SortOrder.Descending }, { "CityName", SortOrder.Ascending } },
                8,
                1);

            // Assert
            string[] expectedOrder = new string[]
            {
                    "Bognor Regis",
                    "Chichester",
                    "Basingstoke",
                    "Petersfield",
                    "Portsmouth",
                    "Southampton",
                    "Winchester",
                    "Bournemouth"
            };

            Assert.That(pagedList.Rows.Select(x => x.CityName).SequenceEqual(expectedOrder), Is.True);
        }

        /// <summary>
        /// Test that we can get the first page when specifying where conditions and sort order.
        /// </summary>
        [TestCase(typeof(InMemoryDataContext))]
        [TestCase(typeof(SqlServerDataContext))]
        [TestCase(typeof(MySqlDataContext))]
        [TestCase(typeof(SqliteDataContext))]
        public async Task Read_Paged_With_Where_And_OrderBy(Type dataContextType)
        {
            // Arrange
            using IDataContext dataContext = DataContextTestHelper.SetupDataContext(dataContextType);
            await dataContext.Create(new City() { CityCode = "PUP", CityName = "Portsmouth", Area = "Hampshire" });
            await dataContext.Create(new City() { CityCode = "SOU", CityName = "Southampton", Area = "Hampshire" });
            await dataContext.Create(new City() { CityCode = "BAS", CityName = "Basingstoke", Area = "Hampshire" });
            await dataContext.Create(new City() { CityCode = "WIN", CityName = "Winchester", Area = "Hampshire" });
            await dataContext.Create(new City() { CityCode = "PTR", CityName = "Petersfield", Area = "Hampshire" });
            await dataContext.Create(new City() { CityCode = "CHI", CityName = "Chichester", Area = "West Sussex" });
            await dataContext.Create(new City() { CityCode = "BOG", CityName = "Bognor Regis", Area = "West Sussex" });
            await dataContext.Create(new City() { CityCode = "BOU", CityName = "Bournemouth", Area = "Dorset" });

            // Act
            PagedList<City> pagedList = await dataContext.ReadList<City>(
                new { Area = "Hampshire" },
                new { CityName = SortOrder.Descending },
                5,
                1);

            // Assert
            string[] expectedOrder = { "Winchester", "Southampton", "Portsmouth", "Petersfield", "Basingstoke" };
            Assert.That(pagedList.Rows.Select(x => x.CityName).SequenceEqual(expectedOrder), Is.True);
        }

        /// <summary>
        /// Test that we can get the first page when specifying where conditions and sort order.
        /// </summary>
        [TestCase(typeof(InMemoryDataContext))]
        [TestCase(typeof(SqlServerDataContext))]
        [TestCase(typeof(MySqlDataContext))]
        [TestCase(typeof(SqliteDataContext))]
        public async Task Read_Paged_With_Where_Dictionary_And_OrderBy_Dictionary(Type dataContextType)
        {
            // Arrange
            using IDataContext dataContext = DataContextTestHelper.SetupDataContext(dataContextType);
            await dataContext.Create(new City() { CityCode = "PUP", CityName = "Portsmouth", Area = "Hampshire" });
            await dataContext.Create(new City() { CityCode = "SOU", CityName = "Southampton", Area = "Hampshire" });
            await dataContext.Create(new City() { CityCode = "BAS", CityName = "Basingstoke", Area = "Hampshire" });
            await dataContext.Create(new City() { CityCode = "WIN", CityName = "Winchester", Area = "Hampshire" });
            await dataContext.Create(new City() { CityCode = "PTR", CityName = "Petersfield", Area = "Hampshire" });
            await dataContext.Create(new City() { CityCode = "CHI", CityName = "Chichester", Area = "West Sussex" });
            await dataContext.Create(new City() { CityCode = "BOG", CityName = "Bognor Regis", Area = "West Sussex" });
            await dataContext.Create(new City() { CityCode = "BOU", CityName = "Bournemouth", Area = "Dorset" });

            // Act
            PagedList<City> pagedList = await dataContext.ReadList<City>(
                new Dictionary<string, object>() { { "Area", "Hampshire" } },
                new Dictionary<string, SortOrder>() { { "CityName", SortOrder.Descending } },
                5,
                1);

            // Assert
            string[] expectedOrder = { "Winchester", "Southampton", "Portsmouth", "Petersfield", "Basingstoke" };
            Assert.That(pagedList.Rows.Select(x => x.CityName).SequenceEqual(expectedOrder), Is.True);
        }

        /// <summary>
        /// Test that we get an ArgumentException thrown when we send an invalid whereConditions object.
        /// </summary>
        [TestCase(typeof(InMemoryDataContext))]
        [TestCase(typeof(SqlServerDataContext))]
        [TestCase(typeof(MySqlDataContext))]
        [TestCase(typeof(SqliteDataContext))]
        public async Task Read_Paged_With_Invalid_Where(Type dataContextType)
        {
            // Arrange
            using IDataContext dataContext = DataContextTestHelper.SetupDataContext(dataContextType);
            await dataContext.Create(new City() { CityCode = "PUP", CityName = "Portsmouth", Area = "Hampshire" });
            await dataContext.Create(new City() { CityCode = "SOU", CityName = "Southampton", Area = "Hampshire" });
            await dataContext.Create(new City() { CityCode = "BAS", CityName = "Basingstoke", Area = "Hampshire" });
            await dataContext.Create(new City() { CityCode = "WIN", CityName = "Winchester", Area = "Hampshire" });
            await dataContext.Create(new City() { CityCode = "PTR", CityName = "Petersfield", Area = "Hampshire" });
            await dataContext.Create(new City() { CityCode = "CHI", CityName = "Chichester", Area = "West Sussex" });
            await dataContext.Create(new City() { CityCode = "BOG", CityName = "Bognor Regis", Area = "West Sussex" });
            await dataContext.Create(new City() { CityCode = "BOU", CityName = "Bournemouth", Area = "Dorset" });

            // Act / Assert
            Assert.ThrowsAsync<ArgumentException>(async () => { await dataContext.ReadList<City>(new { City = "PUP" }, null, 1, 1); });
        }

        /// <summary>
        /// Test that we get an ArgumentException thrown when we send an invalid whereConditions object.
        /// </summary>
        [TestCase(typeof(InMemoryDataContext))]
        [TestCase(typeof(SqlServerDataContext))]
        [TestCase(typeof(MySqlDataContext))]
        [TestCase(typeof(SqliteDataContext))]
        public async Task Read_Paged_With_Invalid_Where_Dictionary(Type dataContextType)
        {
            // Arrange
            using IDataContext dataContext = DataContextTestHelper.SetupDataContext(dataContextType);
            await dataContext.Create(new City() { CityCode = "PUP", CityName = "Portsmouth", Area = "Hampshire" });
            await dataContext.Create(new City() { CityCode = "SOU", CityName = "Southampton", Area = "Hampshire" });
            await dataContext.Create(new City() { CityCode = "BAS", CityName = "Basingstoke", Area = "Hampshire" });
            await dataContext.Create(new City() { CityCode = "WIN", CityName = "Winchester", Area = "Hampshire" });
            await dataContext.Create(new City() { CityCode = "PTR", CityName = "Petersfield", Area = "Hampshire" });
            await dataContext.Create(new City() { CityCode = "CHI", CityName = "Chichester", Area = "West Sussex" });
            await dataContext.Create(new City() { CityCode = "BOG", CityName = "Bognor Regis", Area = "West Sussex" });
            await dataContext.Create(new City() { CityCode = "BOU", CityName = "Bournemouth", Area = "Dorset" });

            // Act / Assert
            Assert.ThrowsAsync<ArgumentException>(async () =>
            {
                await dataContext.ReadList<City>(new Dictionary<string, object> { { "City", "PUP" } }, null, 1, 1);
            });
        }

        /// <summary>
        /// Test that we get an ArgumentException thrown when we send an invalid sort orders object.
        /// </summary>
        [TestCase(typeof(InMemoryDataContext))]
        [TestCase(typeof(SqlServerDataContext))]
        [TestCase(typeof(MySqlDataContext))]
        [TestCase(typeof(SqliteDataContext))]
        public async Task Read_Paged_With_Invalid_OrderBy(Type dataContextType)
        {
            // Arrange
            using IDataContext dataContext = DataContextTestHelper.SetupDataContext(dataContextType);
            await dataContext.Create(new City() { CityCode = "PUP", CityName = "Portsmouth", Area = "Hampshire" });
            await dataContext.Create(new City() { CityCode = "SOU", CityName = "Southampton", Area = "Hampshire" });
            await dataContext.Create(new City() { CityCode = "BAS", CityName = "Basingstoke", Area = "Hampshire" });
            await dataContext.Create(new City() { CityCode = "WIN", CityName = "Winchester", Area = "Hampshire" });
            await dataContext.Create(new City() { CityCode = "PTR", CityName = "Petersfield", Area = "Hampshire" });
            await dataContext.Create(new City() { CityCode = "CHI", CityName = "Chichester", Area = "West Sussex" });
            await dataContext.Create(new City() { CityCode = "BOG", CityName = "Bognor Regis", Area = "West Sussex" });
            await dataContext.Create(new City() { CityCode = "BOU", CityName = "Bournemouth", Area = "Dorset" });

            // Act / Assert
            Assert.ThrowsAsync<ArgumentException>(async () =>
            {
                await dataContext.ReadList<City>(null, new { County = SortOrder.Descending }, 1, 1);
            });
        }

        /// <summary>
        /// Test that we get an ArgumentException thrown when we send an invalid sort orders object.
        /// </summary>
        [TestCase(typeof(InMemoryDataContext))]
        [TestCase(typeof(SqlServerDataContext))]
        [TestCase(typeof(MySqlDataContext))]
        [TestCase(typeof(SqliteDataContext))]
        public async Task Read_Paged_With_Invalid_OrderBy_Dictionary(Type dataContextType)
        {
            // Arrange
            using IDataContext dataContext = DataContextTestHelper.SetupDataContext(dataContextType);
            await dataContext.Create(new City() { CityCode = "PUP", CityName = "Portsmouth", Area = "Hampshire" });
            await dataContext.Create(new City() { CityCode = "SOU", CityName = "Southampton", Area = "Hampshire" });
            await dataContext.Create(new City() { CityCode = "BAS", CityName = "Basingstoke", Area = "Hampshire" });
            await dataContext.Create(new City() { CityCode = "WIN", CityName = "Winchester", Area = "Hampshire" });
            await dataContext.Create(new City() { CityCode = "PTR", CityName = "Petersfield", Area = "Hampshire" });
            await dataContext.Create(new City() { CityCode = "CHI", CityName = "Chichester", Area = "West Sussex" });
            await dataContext.Create(new City() { CityCode = "BOG", CityName = "Bognor Regis", Area = "West Sussex" });
            await dataContext.Create(new City() { CityCode = "BOU", CityName = "Bournemouth", Area = "Dorset" });

            // Act / Assert
            Assert.ThrowsAsync<ArgumentException>(async () =>
            {
                await dataContext.ReadList<City>(null, new Dictionary<string, SortOrder> { { "County", SortOrder.Descending } }, 1, 1);
            });
        }
    }
}