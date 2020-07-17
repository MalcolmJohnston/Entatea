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

namespace Testadal.Tests.Predicates
{
    [TestFixture]
    public class StringTests : BaseTest
    {
        [TestCase(typeof(InMemoryDataContext))]
        [TestCase(typeof(SqlServerDataContext))]
        [TestCase(typeof(MySqlDataContext))]
        public async Task Read_With_String_Equal_Predicate(Type dataContextType)
        {
            // Arrange
            IDataContext dataContext = DataContextProvider.SetupDataContext(dataContextType);
            await dataContext.Create(new Product() { Name = "Spanner" });
            await dataContext.Create(new Product() { Name = "Hammer" });

            // Act
            IEnumerable<Product> products = await dataContext.ReadList<Product>(Equal<Product>(x => x.Name, "Spanner"));

            // Assert
            Assert.AreEqual(1, products.Count());
            Assert.AreEqual("Spanner", products.ElementAt(0).Name);
        }

        [TestCase(typeof(InMemoryDataContext))]
        [TestCase(typeof(SqlServerDataContext))]
        [TestCase(typeof(MySqlDataContext))]
        public async Task Read_With_String_Equal_Null_Predicate(Type dataContextType)
        {
            // Arrange
            IDataContext dataContext = DataContextProvider.SetupDataContext(dataContextType);
            await dataContext.Create(new Product() { Name = null });
            await dataContext.Create(new Product() { Name = "Hammer" });

            // Act
            IEnumerable<Product> products = await dataContext.ReadList<Product>(Equal<Product>(x => x.Name, null));

            // Assert
            Assert.AreEqual(1, products.Count());
            Assert.AreEqual(null, products.ElementAt(0).Name);
        }

        [TestCase(typeof(InMemoryDataContext))]
        [TestCase(typeof(SqlServerDataContext))]
        [TestCase(typeof(MySqlDataContext))]
        public async Task Read_With_String_NotEqual_Predicate(Type dataContextType)
        {
            // Arrange
            IDataContext dataContext = DataContextProvider.SetupDataContext(dataContextType);
            await dataContext.Create(new Product() { Name = "Spanner" });
            await dataContext.Create(new Product() { Name = "Hammer" });

            // Act
            IEnumerable<Product> products = await dataContext.ReadList<Product>(NotEqual<Product>(x => x.Name, "Spanner"));

            // Assert
            Assert.AreEqual(1, products.Count());
            Assert.AreEqual("Hammer", products.ElementAt(0).Name);
        }

        [TestCase(typeof(InMemoryDataContext))]
        [TestCase(typeof(SqlServerDataContext))]
        [TestCase(typeof(MySqlDataContext))]
        public async Task Read_With_String_In_Predicate(Type dataContextType)
        {
            // Arrange
            IDataContext dataContext = DataContextProvider.SetupDataContext(dataContextType);
            await dataContext.Create(new Product() { Name = "Spanner" });
            await dataContext.Create(new Product() { Name = "Hammer" });
            await dataContext.Create(new Product() { Name = "Nail" });

            // Act
            string[] productNames = new[] { "Hammer", "Nail" };
            IEnumerable<Product> products = await dataContext.ReadList<Product>(In<Product>(x => x.Name, productNames));
            
            // Assert
            Assert.That(productNames, Is.EquivalentTo(products.Select(x => x.Name)));
        }

        [TestCase(typeof(InMemoryDataContext))]
        [TestCase(typeof(SqlServerDataContext))]
        [TestCase(typeof(MySqlDataContext))]
        public async Task Read_With_String_NotIn_Predicate(Type dataContextType)
        {
            // Arrange
            IDataContext dataContext = DataContextProvider.SetupDataContext(dataContextType);
            await dataContext.Create(new Product() { Name = "Spanner" });
            await dataContext.Create(new Product() { Name = "Hammer" });
            await dataContext.Create(new Product() { Name = "Nail" });

            // Act
            string[] productNames = new[] { "Hammer", "Nail" };
            IEnumerable<Product> products = await dataContext.ReadList<Product>(NotIn<Product>(x => x.Name, productNames));

            // Assert
            Assert.AreEqual(1, products.Count());
            Assert.AreEqual("Spanner", products.ElementAt(0).Name);
        }

        [TestCase(typeof(InMemoryDataContext))]
        [TestCase(typeof(SqlServerDataContext))]
        [TestCase(typeof(MySqlDataContext))]
        public async Task Read_With_String_Contains_Predicate(Type dataContextType)
        {
            // Arrange
            IDataContext dataContext = DataContextProvider.SetupDataContext(dataContextType);
            await dataContext.Create(new Product() { Name = "Bosch Hammer I" });
            await dataContext.Create(new Product() { Name = "Black and Decker Hammer" });
            await dataContext.Create(new Product() { Name = "Hammer B&Q" });
            await dataContext.Create(new Product() { Name = "Something Else" });

            // Act
            IEnumerable<Product> products = await dataContext.ReadList<Product>(Contains<Product>(x => x.Name, "Hammer"));

            // Assert
            Assert.AreEqual(3, products.Count());
        }

        [TestCase(typeof(InMemoryDataContext))]
        [TestCase(typeof(SqlServerDataContext))]
        [TestCase(typeof(MySqlDataContext))]
        public async Task Read_With_String_StartsWith_Predicate(Type dataContextType)
        {
            // Arrange
            IDataContext dataContext = DataContextProvider.SetupDataContext(dataContextType);
            await dataContext.Create(new Product() { Name = "Bosch Hammer I" });
            await dataContext.Create(new Product() { Name = "Black and Decker Hammer" });
            await dataContext.Create(new Product() { Name = "Hammer B&Q" });
            await dataContext.Create(new Product() { Name = "Something Else" });

            // Act
            IEnumerable<Product> products = await dataContext.ReadList<Product>(StartsWith<Product>(x => x.Name, "Hammer"));

            // Assert
            Assert.AreEqual(1, products.Count());
            Assert.AreEqual("Hammer B&Q", products.ElementAt(0).Name);
        }

        [TestCase(typeof(InMemoryDataContext))]
        [TestCase(typeof(SqlServerDataContext))]
        [TestCase(typeof(MySqlDataContext))]
        public async Task Read_With_String_EndsWith_Predicate(Type dataContextType)
        {
            // Arrange
            IDataContext dataContext = DataContextProvider.SetupDataContext(dataContextType);
            await dataContext.Create(new Product() { Name = "Bosch Hammer I" });
            await dataContext.Create(new Product() { Name = "Black and Decker Hammer" });
            await dataContext.Create(new Product() { Name = "Hammer B&Q" });
            await dataContext.Create(new Product() { Name = "Something Else" });

            // Act
            IEnumerable<Product> products = await dataContext.ReadList<Product>(EndsWith<Product>(x => x.Name, "Hammer"));

            // Assert
            Assert.AreEqual(1, products.Count());
            Assert.AreEqual("Black and Decker Hammer", products.ElementAt(0).Name);
        }
    }
}
