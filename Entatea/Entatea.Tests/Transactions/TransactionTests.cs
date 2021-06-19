using System;
using System.Linq;
using System.Threading.Tasks;

using Entatea.MySql;
using Entatea.Sqlite;
using Entatea.SqlServer;

using Entatea.Tests.Entities;
using Entatea.Tests.Helpers;

using NUnit.Framework;

namespace Entatea.Tests.Transactions
{
    public class TransactionTests : BaseTest
    {
        [TestCase(typeof(SqlServerDataContext))]
        [TestCase(typeof(MySqlDataContext))]
        [TestCase(typeof(SqliteDataContext))]
        public async Task Test_Created_Records_Commited(Type dataContextType)
        {
            // Arrange
            using IDataContext dataContext = DataContextProvider.SetupDataContext(dataContextType);

            // Act
            dataContext.BeginTransaction();
            await dataContext.Create(new Product() { Name = "Product 1" }).ConfigureAwait(false);
            await dataContext.Create(new Product() { Name = "Product 2" }).ConfigureAwait(false);
            dataContext.Commit();

            // Assert
            var products = await dataContext.ReadAll<Product>();
            Assert.AreEqual(2, products.Count());
        }

        [TestCase(typeof(SqlServerDataContext))]
        [TestCase(typeof(MySqlDataContext))]
        [TestCase(typeof(SqliteDataContext))]
        public async Task Test_Created_Records_Rolledback_Explicit(Type dataContextType)
        {
            // Arrange
            using IDataContext dataContext = DataContextProvider.SetupDataContext(dataContextType);

            // Act
            dataContext.BeginTransaction();
            await dataContext.Create(new Product() { Name = "Product 1" }).ConfigureAwait(false);
            await dataContext.Create(new Product() { Name = "Product 2" }).ConfigureAwait(false);
            dataContext.Rollback();

            // Assert
            var products = await dataContext.ReadAll<Product>();
            Assert.AreEqual(0, products.Count());
        }

        [TestCase(typeof(SqlServerDataContext))]
        [TestCase(typeof(MySqlDataContext))]
        [TestCase(typeof(SqliteDataContext))]
        public async Task Test_Created_Records_Rolledback_Implicit(Type dataContextType)
        {
            // Arrange
            using (IDataContext dc1 = DataContextProvider.SetupDataContext(dataContextType))
            {
                // Act
                dc1.BeginTransaction();
                await dc1.Create(new Product() { Name = "Product 1" }).ConfigureAwait(false);
                await dc1.Create(new Product() { Name = "Product 2" }).ConfigureAwait(false);
            }

            // Assert
            using IDataContext dc2 = DataContextProvider.SetupDataContext(dataContextType);
            var products = await dc2.ReadAll<Product>();
            Assert.AreEqual(0, products.Count());
        }

        [TestCase(typeof(SqlServerDataContext))]
        [TestCase(typeof(MySqlDataContext))]
        [TestCase(typeof(SqliteDataContext))]
        public async Task Test_Updated_Records_Comitted(Type dataContextType)
        {
            // Arrange
            using IDataContext dataContext = DataContextProvider.SetupDataContext(dataContextType);
            Product product1 = await dataContext.Create(new Product() { Name = "Product 1" }).ConfigureAwait(false);
            Product product2 = await dataContext.Create(new Product() { Name = "Product 1" }).ConfigureAwait(false);
            
            // Act
            dataContext.BeginTransaction();
            product1.Name = "Product 3";
            product2.Name = "Product 4";
            
            await dataContext.Update<Product>(new { product1.Id, product1.Name }).ConfigureAwait(false);
            await dataContext.Update<Product>(new { product2.Id, product2.Name }).ConfigureAwait(false);
            dataContext.Commit();

            // Assert
            Product updated1 = await dataContext.Read<Product>(product1.Id);
            Product updated2 = await dataContext.Read<Product>(product2.Id);
            Assert.AreEqual(product1.Name, updated1.Name);
            Assert.AreEqual(product2.Name, updated2.Name);
        }

        [TestCase(typeof(SqlServerDataContext))]
        [TestCase(typeof(MySqlDataContext))]
        [TestCase(typeof(SqliteDataContext))]
        public async Task Test_Updated_Records_Rolledback_Explicit(Type dataContextType)
        {
            // Arrange
            using IDataContext dataContext = DataContextProvider.SetupDataContext(dataContextType);
            Product original1 = await dataContext.Create(new Product() { Name = "Product 1" }).ConfigureAwait(false);
            Product original2 = await dataContext.Create(new Product() { Name = "Product 2" }).ConfigureAwait(false);

            // Act
            dataContext.BeginTransaction();
            Product updated1 = await dataContext.Update<Product>(new { original1.Id, Name = "Updated 1" }).ConfigureAwait(false);
            Product updated2 = await dataContext.Update<Product>(new { original2.Id, Name = "Updated 2" }).ConfigureAwait(false);
            dataContext.Rollback();

            // Assert
            Product read1 = await dataContext.Read<Product>(original1.Id);
            Product read2 = await dataContext.Read<Product>(original2.Id);
            Assert.AreEqual(original1.Name, read1.Name);
            Assert.AreEqual(original2.Name, read2.Name);
            Assert.AreNotEqual(updated1.Name, read1.Name);
            Assert.AreNotEqual(updated2.Name, read2.Name);
        }

        [TestCase(typeof(SqlServerDataContext))]
        [TestCase(typeof(MySqlDataContext))]
        [TestCase(typeof(SqliteDataContext))]
        public async Task Test_Updated_Records_Rolledback_Implicit(Type dataContextType)
        {
            // Arrange
            Product original1, original2, updated1, updated2;
            using (IDataContext dataContext = DataContextProvider.SetupDataContext(dataContextType))
            {
                original1 = await dataContext.Create(new Product() { Name = "Product 1" }).ConfigureAwait(false);
                original2 = await dataContext.Create(new Product() { Name = "Product 2" }).ConfigureAwait(false);

                // Act
                dataContext.BeginTransaction();
                updated1 = await dataContext.Update<Product>(new { original1.Id, Name = "Updated 1" }).ConfigureAwait(false);
                updated2 = await dataContext.Update<Product>(new { original2.Id, Name = "Updated 2" }).ConfigureAwait(false);
            }

            // Assert
            using IDataContext dataContext2 = DataContextProvider.SetupDataContext(dataContextType);
            Product read1 = await dataContext2.Read<Product>(original1.Id);
            Product read2 = await dataContext2.Read<Product>(original2.Id);
            Assert.AreEqual(original1.Name, read1.Name);
            Assert.AreEqual(original2.Name, read2.Name);
            Assert.AreNotEqual(updated1.Name, read1.Name);
            Assert.AreNotEqual(updated2.Name, read2.Name);
        }

        [Test]
        public async Task Test_Nested_Transactions_Both_Committed_Sql_Server()
        {
            // Arrange
            LocalDbTestHelper.CreateTestDatabase(TestContext.CurrentContext.Test.FullName);
            string connectionString = LocalDbTestHelper.GetTestConnectionString(TestContext.CurrentContext.Test.FullName);
            SqlServerConnectionProvider connectionProvider = new SqlServerConnectionProvider(connectionString);

            // Act
            using (IDataContext dataContext1 = new SqlServerDataContext(connectionProvider))
            {
                dataContext1.BeginTransaction();  // creates new transaction
                await dataContext1.Create(new Product() { Name = "Product 1" }).ConfigureAwait(false);

                // create second data context with same connection provider
                using (IDataContext dataContext2 = new SqlServerDataContext(connectionProvider))
                {
                    dataContext2.BeginTransaction(); // adds a reference to the outer transaction
                    await dataContext2.Create(new Product() { Name = "Product 2" }).ConfigureAwait(false);
                    await dataContext2.Create(new Product() { Name = "Product 3" }).ConfigureAwait(false);

                    dataContext2.Commit(); // removes a refernce from the outer transaction but doesn't commit
                }

                await dataContext1.Create(new Product() { Name = "Product 3" }).ConfigureAwait(false);

                dataContext1.Commit(); // commits all updates
            }

            // Assert
            using IDataContext dataContext3 = new SqlServerDataContext(connectionString);
            var products = await dataContext3.ReadAll<Product>();
            Assert.AreEqual(4, products.Count());
        }

        [Test]
        public async Task Test_Nested_Transactions_Inner_Rollback_SqlServer()
        {
            // Arrange
            LocalDbTestHelper.CreateTestDatabase(TestContext.CurrentContext.Test.FullName);
            string connectionString = LocalDbTestHelper.GetTestConnectionString(TestContext.CurrentContext.Test.FullName);
            SqlServerConnectionProvider connectionProvider = new SqlServerConnectionProvider(connectionString);

            // Act / Assert
            using (IDataContext dataContext1 = new SqlServerDataContext(connectionProvider))
            {
                dataContext1.BeginTransaction();  // creates new transaction
                await dataContext1.Create(new Product() { Name = "Product 1" }).ConfigureAwait(false);

                // create second data context with same connection provider
                using (IDataContext dataContext2 = new SqlServerDataContext(connectionProvider))
                {
                    dataContext2.BeginTransaction(); // adds a reference to the outer transaction
                    await dataContext2.Create(new Product() { Name = "Product 2" }).ConfigureAwait(false);
                    await dataContext2.Create(new Product() { Name = "Product 3" }).ConfigureAwait(false);

                    // rollsback the transaction and throws exception
                    Assert.Throws<InvalidOperationException>(() => { dataContext2.Rollback(); }); 
                }
            }

            // Assert
            using IDataContext dataContext3 = new SqlServerDataContext(connectionString);
            var products = await dataContext3.ReadAll<Product>();
            Assert.AreEqual(0, products.Count());
        }

        [Test]
        public async Task Test_Nested_Transactions_Outer_Rollback_SqlServer()
        {
            // Arrange
            LocalDbTestHelper.CreateTestDatabase(TestContext.CurrentContext.Test.FullName);
            string connectionString = LocalDbTestHelper.GetTestConnectionString(TestContext.CurrentContext.Test.FullName);
            SqlServerConnectionProvider connectionProvider = new SqlServerConnectionProvider(connectionString);

            // Act
            using (IDataContext dataContext1 = new SqlServerDataContext(connectionProvider))
            {
                dataContext1.BeginTransaction();  // creates new transaction
                await dataContext1.Create(new Product() { Name = "Product 1" }).ConfigureAwait(false);

                // create second data context with same connection provider
                using (IDataContext dataContext2 = new SqlServerDataContext(connectionProvider))
                {
                    dataContext2.BeginTransaction(); // adds a reference to the outer transaction
                    await dataContext2.Create(new Product() { Name = "Product 2" }).ConfigureAwait(false);
                    await dataContext2.Create(new Product() { Name = "Product 3" }).ConfigureAwait(false);

                    dataContext2.Commit(); // removes a refernce from the outer transaction but doesn't commit
                }

                await dataContext1.Create(new Product() { Name = "Product 3" }).ConfigureAwait(false);

                dataContext1.Rollback();
            }

            // Assert
            using IDataContext dataContext3 = new SqlServerDataContext(connectionString);
            var products = await dataContext3.ReadAll<Product>();
            Assert.AreEqual(0, products.Count());
        }
    }
}
