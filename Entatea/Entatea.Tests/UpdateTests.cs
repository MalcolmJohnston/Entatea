using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using Entatea.InMemory;
using Entatea.MySql;
using Entatea.Sqlite;
using Entatea.SqlServer;

using Entatea.Tests.Helpers;
using Entatea.Tests.Entities;

using NUnit.Framework;
using System.Collections;
using System.Dynamic;

namespace Entatea.Tests
{
    [TestFixture]
    public class UpdateTests : BaseTest
    {
        /// <summary>
        /// Test that we can update a single editable property
        /// </summary>
        [TestCase(typeof(InMemoryDataContext))]
        [TestCase(typeof(SqlServerDataContext))]
        [TestCase(typeof(MySqlDataContext))]
        [TestCase(typeof(SqliteDataContext))]
        public async Task Update_Editable_Property(Type dataContextType)
        {
            // Arrange
            IDataContext dataContext = DataContextProvider.SetupDataContext(dataContextType);
            City city = await dataContext.Create<City>(new City() { CityCode = "BAS", CityName = "Basingstoke", Area = "Hampshire" });

            // Act
            City basVegas = await dataContext.Update<City>(new { city.CityId, CityName = "Bas Vegas!" });

            // Assert
            Assert.AreEqual(city.CityId, basVegas.CityId);
            Assert.AreEqual(city.CityCode, basVegas.CityCode);
            Assert.AreEqual(city.Area, basVegas.Area);
            Assert.AreEqual("Bas Vegas!", basVegas.CityName);
        }

        /// <summary>
        /// Test that we can update a single editable property with an Expando Object
        /// </summary>
        [TestCase(typeof(InMemoryDataContext))]
        [TestCase(typeof(SqlServerDataContext))]
        [TestCase(typeof(MySqlDataContext))]
        [TestCase(typeof(SqliteDataContext))]
        public async Task Update_Editable_Property_Expando_Object(Type dataContextType)
        {
            // Arrange
            IDataContext dataContext = DataContextProvider.SetupDataContext(dataContextType);
            City city = await dataContext.Create<City>(new City() { CityCode = "BAS", CityName = "Basingstoke", Area = "Hampshire" });

            // Act
            IDictionary<string, object> eo = new ExpandoObject();
            eo.Add(nameof(City.CityId), city.CityId);
            eo.Add(nameof(City.CityName), "Bas Vegas!");
            City basVegas = await dataContext.Update<City>(eo);

            // Assert
            Assert.AreEqual(city.CityId, basVegas.CityId);
            Assert.AreEqual(city.CityCode, basVegas.CityCode);
            Assert.AreEqual(city.Area, basVegas.Area);
            Assert.AreEqual("Bas Vegas!", basVegas.CityName);
        }

        /// <summary>
        /// Test that we can update multiple editable properties.
        /// </summary>
        [TestCase(typeof(InMemoryDataContext))]
        [TestCase(typeof(SqlServerDataContext))]
        [TestCase(typeof(MySqlDataContext))]
        [TestCase(typeof(SqliteDataContext))]
        public async Task Update_Editable_Properties(Type dataContextType)
        {
            // Arrange
            IDataContext dataContext = DataContextProvider.SetupDataContext(dataContextType);
            City city = await dataContext.Create<City>(new City() { CityCode = "BAS", CityName = "Basingstoke", Area = "Hampshire" });

            // Act
            City basVegas = await dataContext.Update<City>(new
            {
                city.CityId,
                CityCode = "BV",
                CityName = "Bas Vegas!",
                Area = "The Strip"
            });

            // Assert
            Assert.AreEqual(city.CityId, basVegas.CityId);
            Assert.AreEqual("BV", basVegas.CityCode);
            Assert.AreEqual("The Strip", basVegas.Area);
            Assert.AreEqual("Bas Vegas!", basVegas.CityName);
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
        public async Task Update_With_Datestamp(Type dataContextType)
        {
            // Arrange
            IDataContext dataContext = DataContextProvider.SetupDataContext(dataContextType);

            // insert row
            DateStamp row = await dataContext.Create(new DateStamp() { Name = "Key", Value = "Value" });

            // sleep so that insert date and update date will be different when update called
            Thread.Sleep(TimeSpan.FromMilliseconds(1000));

            // Act
            DateTime updateDate = DateTime.Now;
            DateStamp updatedRow = await dataContext.Update<DateStamp>(new { row.Name, Value = "New Value" });

            // Assert
            Assert.AreEqual(row.Name, updatedRow.Name);
            Assert.AreEqual("New Value", updatedRow.Value);
            Assert.AreEqual(0, (row.InsertDate - updatedRow.InsertDate).Seconds);
            Assert.AreNotEqual(row.InsertDate, updatedRow.UpdateDate);
            Assert.That(updatedRow.UpdateDate, Is.EqualTo(updateDate).Within(TimeSpan.FromSeconds(1)));
        }

        /// <summary>
        /// Test that when we try to update a soft delete column that the value is ignored.
        /// </summary>
        /// <returns></returns>
        [TestCase(typeof(InMemoryDataContext))]
        [TestCase(typeof(SqlServerDataContext))]
        [TestCase(typeof(MySqlDataContext))]
        [TestCase(typeof(SqliteDataContext))]
        public async Task Update_Soft_Delete_Column(Type dataContextType)
        {
            // Arrange
            IDataContext dataContext = DataContextProvider.SetupDataContext(dataContextType);
            SoftDelete softDelete = await dataContext.Create<SoftDelete>(new SoftDelete());

            // Act / Assert
            Assert.ThrowsAsync<ArgumentException>(async () =>
            {
                await dataContext.Update<SoftDelete>(new { softDelete.SoftDeleteId, RecordStatus = 999 });
            });
        }

        /// <summary>
        /// Test that when we try to update a read only column that our update is not persisted.
        /// </summary>
        /// <returns></returns>
        [TestCase(typeof(InMemoryDataContext))]
        [TestCase(typeof(SqlServerDataContext))]
        [TestCase(typeof(MySqlDataContext))]
        [TestCase(typeof(SqliteDataContext))]
        public async Task Update_Read_Only_Column(Type dataContextType)
        {
            // Arrange
            IDataContext dataContext = DataContextProvider.SetupDataContext(dataContextType);
            ReadOnly readOnly = await dataContext.Create<ReadOnly>(new ReadOnly()
            {
                Editable = "Hello",
                ReadOnlyProperty = "World"
            });

            // Act
            readOnly = await dataContext.Update<ReadOnly>(new
            {
                readOnly.SequentialId,
                Editable = "Goodbye",
                ReadOnlyProperty = "Yesterday"
            });

            // Assert
            Assert.AreEqual("Goodbye", readOnly.Editable);
            Assert.AreEqual("World", readOnly.ReadOnlyProperty);
        }
    }
}
