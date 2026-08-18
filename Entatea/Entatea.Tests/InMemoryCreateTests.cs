using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using NUnit.Framework;

using Entatea.InMemory;

using Entatea.Tests.Entities;

namespace Entatea.InMemoryTests
{
    [TestFixture]
    public class InMemoryCreateTests
    {
        [Test]
        public async Task Create_Does_Not_Set_The_Generated_Key_On_The_Entity_Passed_In()
        {
            // Arrange
            using InMemoryDataContext dataContext = new InMemoryDataContext();
            City city = new City() { CityCode = "BRI", CityName = "Brighton", Area = "Sussex" };

            // Act
            City created = await dataContext.Create(city);

            // Assert
            Assert.That(city.CityId, Is.EqualTo(0));
            Assert.That(created.CityId, Is.GreaterThan(0));
        }

        [Test]
        public async Task Create_Does_Not_Set_Date_Stamps_On_The_Entity_Passed_In()
        {
            // Arrange
            using InMemoryDataContext dataContext = new InMemoryDataContext();
            DateStamp dateStamp = new DateStamp() { Name = "Key", Value = "Value" };

            // Act
            DateStamp created = await dataContext.Create(dateStamp);

            // Assert
            Assert.That(dateStamp.InsertDate, Is.EqualTo(default(DateTime)));
            Assert.That(dateStamp.UpdateDate, Is.EqualTo(default(DateTime)));
            Assert.That(created.InsertDate, Is.EqualTo(DateTime.Now).Within(TimeSpan.FromSeconds(30)));
            Assert.That(created.UpdateDate, Is.EqualTo(DateTime.Now).Within(TimeSpan.FromSeconds(30)));
        }


        [Test]
        public async Task Mutating_The_Entity_Passed_To_Create_Does_Not_Change_The_Store()
        {
            // Arrange
            using InMemoryDataContext dataContext = new InMemoryDataContext();

            // Assert / Assert
            City city = new City() { CityCode = "BRI", CityName = "Brighton", Area = "Sussex" };
            City created = await dataContext.Create(city);

            city.CityName = "Mutated";

            City read = await dataContext.Read<City>(created.CityId);

            Assert.That(read.CityName, Is.EqualTo("Brighton"));
        }

        [Test]
        public async Task Mutating_The_Entity_Returned_By_Create_Does_Not_Change_The_Store()
        {
            using InMemoryDataContext dataContext = new InMemoryDataContext();
            City created = await dataContext.Create(
                new City() { CityCode = "BRI", CityName = "Brighton", Area = "Sussex" });

            created.CityName = "Mutated";

            City read = await dataContext.Read<City>(created.CityId);
            Assert.That(read.CityName, Is.EqualTo("Brighton"));
        }


        [Test]
        public async Task Copying_Rows_To_A_New_Area_Leaves_The_Source_Rows_Intact()
        {
            using InMemoryDataContext dataContext = new InMemoryDataContext();

            List<City> sussex = new List<City>();
            foreach (string cityName in new[] { "Brighton", "London", "Manchester" })
            {
                sussex.Add(await dataContext.Create(
                    new City() { CityCode = cityName.Substring(0, 3).ToUpper(), CityName = cityName, Area = "Sussex" }));
            }

            foreach (City source in sussex)
            {
                source.Area = "Kent";
                await dataContext.Create(source);
            }

            List<City> sourceRows = (await dataContext.ReadList<City>(new { Area = "Sussex" }))
                                    .OrderBy(x => x.CityId)
                                    .ToList();
            List<City> copiedRows = (await dataContext.ReadList<City>(new { Area = "Kent" }))
                                    .OrderBy(x => x.CityId)
                                    .ToList();

            Assert.Multiple(() =>
            {
                Assert.That(sourceRows.Select(x => x.CityId), Is.EqualTo(sussex.Select(x => x.CityId)));
                Assert.That(sourceRows.Select(x => x.CityName), Is.EqualTo(new[] { "Brighton", "London", "Manchester" }));

                Assert.That(copiedRows.Select(x => x.CityName), Is.EqualTo(new[] { "Brighton", "London", "Manchester" }));
                Assert.That(copiedRows.Select(x => x.CityCode), Is.EqualTo(new[] { "BRI", "LON", "MAN" }));
                Assert.That(copiedRows.Select(x => x.CityId).Intersect(sourceRows.Select(x => x.CityId)), Is.Empty);
            });
        }
    }
}
