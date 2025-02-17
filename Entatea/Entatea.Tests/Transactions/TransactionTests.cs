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
            using IDataContext dataContext = DataContextTestHelper.SetupDataContext(dataContextType);

            // Act
            dataContext.BeginTransaction();
            await dataContext.Create(new Product() { Name = "Product 1" });
            await dataContext.Create(new Product() { Name = "Product 2" });
            dataContext.Commit();

            // Assert
            var products = await dataContext.ReadAll<Product>();
            Assert.That(products.Count(), Is.EqualTo(2));
        }

        [TestCase(typeof(SqlServerDataContext))]
        [TestCase(typeof(MySqlDataContext))]
        [TestCase(typeof(SqliteDataContext))]
        public async Task Test_Created_Records_Rolledback_Explicit(Type dataContextType)
        {
            // Arrange
            using IDataContext dataContext = DataContextTestHelper.SetupDataContext(dataContextType);

            // Act
            dataContext.BeginTransaction();
            await dataContext.Create(new Product() { Name = "Product 1" });
            await dataContext.Create(new Product() { Name = "Product 2" });
            dataContext.Rollback();

            // Assert
            var products = await dataContext.ReadAll<Product>();
            Assert.That(products.Count(), Is.EqualTo(0));
        }

        [TestCase(typeof(SqlServerDataContext))]
        [TestCase(typeof(MySqlDataContext))]
        [TestCase(typeof(SqliteDataContext))]
        public async Task Test_Created_Records_Rolledback_Implicit(Type dataContextType)
        {
            // Arrange
            using (IDataContext dc1 = DataContextTestHelper.SetupDataContext(dataContextType))
            {
                // Act
                dc1.BeginTransaction();
                await dc1.Create(new Product() { Name = "Product 1" });
                await dc1.Create(new Product() { Name = "Product 2" });
            }

            // Assert
            using IDataContext dc2 = DataContextTestHelper.SetupDataContext(dataContextType);
            var products = await dc2.ReadAll<Product>();
            Assert.That(products.Count(), Is.EqualTo(0));
        }

        [TestCase(typeof(SqlServerDataContext))]
        [TestCase(typeof(MySqlDataContext))]
        [TestCase(typeof(SqliteDataContext))]
        public async Task Test_Updated_Records_Comitted(Type dataContextType)
        {
            // Arrange
            using IDataContext dataContext = DataContextTestHelper.SetupDataContext(dataContextType);
            Product product1 = await dataContext.Create(new Product() { Name = "Product 1" });
            Product product2 = await dataContext.Create(new Product() { Name = "Product 1" });
            
            // Act
            dataContext.BeginTransaction();
            product1.Name = "Product 3";
            product2.Name = "Product 4";

            await dataContext.Update<Product>(new { product1.Id, product1.Name });
            await dataContext.Update<Product>(new { product2.Id, product2.Name });
            dataContext.Commit();

            // Assert
            Product updated1 = await dataContext.Read<Product>(product1.Id);
            Product updated2 = await dataContext.Read<Product>(product2.Id);
            Assert.That(updated1.Name, Is.EqualTo(product1.Name));
            Assert.That(updated2.Name, Is.EqualTo(product2.Name));
        }

        [TestCase(typeof(SqlServerDataContext))]
        [TestCase(typeof(MySqlDataContext))]
        [TestCase(typeof(SqliteDataContext))]
        public async Task Test_Updated_Records_Rolledback_Explicit(Type dataContextType)
        {
            // Arrange
            using IDataContext dataContext = DataContextTestHelper.SetupDataContext(dataContextType);
            Product original1 = await dataContext.Create(new Product() { Name = "Product 1" });
            Product original2 = await dataContext.Create(new Product() { Name = "Product 2" });

            // Act
            dataContext.BeginTransaction();
            Product updated1 = await dataContext.Update<Product>(new { original1.Id, Name = "Updated 1" });
            Product updated2 = await dataContext.Update<Product>(new { original2.Id, Name = "Updated 2" });
            dataContext.Rollback();

            // Assert
            Product read1 = await dataContext.Read<Product>(original1.Id);
            Product read2 = await dataContext.Read<Product>(original2.Id);
            
            Assert.That(read1.Name, Is.EqualTo(original1.Name));
            Assert.That(read2.Name, Is.EqualTo(original2.Name));

            Assert.That(read1.Name, Is.Not.EqualTo(updated1.Name));
            Assert.That(read2.Name, Is.Not.EqualTo(updated2.Name));
        }

        [TestCase(typeof(SqlServerDataContext))]
        [TestCase(typeof(MySqlDataContext))]
        [TestCase(typeof(SqliteDataContext))]
        public async Task Test_Updated_Records_Rolledback_Implicit(Type dataContextType)
        {
            // Arrange
            Product original1, original2, updated1, updated2;
            using (IDataContext dataContext = DataContextTestHelper.SetupDataContext(dataContextType))
            {
                original1 = await dataContext.Create(new Product() { Name = "Product 1" });
                original2 = await dataContext.Create(new Product() { Name = "Product 2" });

                // Act
                dataContext.BeginTransaction();
                updated1 = await dataContext.Update<Product>(new { original1.Id, Name = "Updated 1" });
                updated2 = await dataContext.Update<Product>(new { original2.Id, Name = "Updated 2" });
            }

            // Assert
            using IDataContext dataContext2 = DataContextTestHelper.SetupDataContext(dataContextType);
            Product read1 = await dataContext2.Read<Product>(original1.Id);
            Product read2 = await dataContext2.Read<Product>(original2.Id);

            Assert.That(read1.Name, Is.EqualTo(original1.Name));
            Assert.That(read2.Name, Is.EqualTo(original2.Name));

            Assert.That(read1.Name, Is.Not.EqualTo(updated1.Name));
            Assert.That(read2.Name, Is.Not.EqualTo(updated2.Name));
        }

        [TestCase(typeof(SqlServerDataContext))]
        [TestCase(typeof(MySqlDataContext))]
        [TestCase(typeof(SqliteDataContext))]
        public async Task Test_Nested_Transactions_Both_Committed(Type dataContextType)
        {
            // Arrange
            DataContextTestHelper.CreateTestDatabase(dataContextType);
            IConnectionProvider connectionProvider = DataContextTestHelper.GetConnectionProvider(dataContextType);

            // Act
            using (IDataContext dataContext1 = DataContextTestHelper.GetDataContext(dataContextType, connectionProvider))
            {
                dataContext1.BeginTransaction();  // creates new transaction
                await dataContext1.Create(new Product() { Name = "Product 1" });

                // create second data context with same connection provider
                using (IDataContext dataContext2 = DataContextTestHelper.GetDataContext(dataContextType, connectionProvider))
                {
                    dataContext2.BeginTransaction(); // adds a reference to the outer transaction
                    await dataContext2.Create(new Product() { Name = "Product 2" });
                    await dataContext2.Create(new Product() { Name = "Product 3" });

                    dataContext2.Commit(); // removes a reference from the outer transaction but doesn't commit
                }

                await dataContext1.Create(new Product() { Name = "Product 3" });

                dataContext1.Commit(); // commits all updates
            }

            // Assert
            using IDataContext dataContext3 = DataContextTestHelper.GetDataContext(dataContextType, connectionProvider);
            var products = await dataContext3.ReadAll<Product>();
            
            Assert.That(products.Count(), Is.EqualTo(4));
        }

        [TestCase(typeof(SqlServerDataContext))]
        [TestCase(typeof(MySqlDataContext))]
        [TestCase(typeof(SqliteDataContext))]
        public async Task Test_Nested_Transactions_Inner_Rollback(Type dataContextType)
        {
            // Arrange
            DataContextTestHelper.CreateTestDatabase(dataContextType);
            IConnectionProvider connectionProvider = DataContextTestHelper.GetConnectionProvider(dataContextType);

            // Act / Assert
            using (IDataContext dataContext1 = DataContextTestHelper.GetDataContext(dataContextType, connectionProvider))
            {
                dataContext1.BeginTransaction();  // creates new transaction
                await dataContext1.Create(new Product() { Name = "Product 1" });

                // create second data context with same connection provider
                using (IDataContext dataContext2 = DataContextTestHelper.GetDataContext(dataContextType, connectionProvider))
                {
                    dataContext2.BeginTransaction(); // adds a reference to the outer transaction
                    await dataContext2.Create(new Product() { Name = "Product 2" });
                    await dataContext2.Create(new Product() { Name = "Product 3" });

                    // rollsback the transaction and throws exception
                    try
                    {
                        dataContext2.Rollback();
                    }
                    catch (Exception ex)
                    {
                        Assert.That(ex.GetType().IsAssignableFrom(typeof(InvalidOperationException)), Is.True);
                    }
                }
            }

            // Assert
            using IDataContext dataContext3 = DataContextTestHelper.GetDataContext(dataContextType, connectionProvider);
            var products = await dataContext3.ReadAll<Product>();
            
            Assert.That(products.Count(), Is.EqualTo(0));
        }

        [TestCase(typeof(SqlServerDataContext))]
        [TestCase(typeof(MySqlDataContext))]
        [TestCase(typeof(SqliteDataContext))]
        public async Task Test_Nested_Transactions_Outer_Rollback(Type dataContextType)
        {
            // Arrange
            DataContextTestHelper.CreateTestDatabase(dataContextType);
            IConnectionProvider connectionProvider = DataContextTestHelper.GetConnectionProvider(dataContextType);

            // Act
            using (IDataContext dataContext1 = DataContextTestHelper.GetDataContext(dataContextType, connectionProvider))
            {
                dataContext1.BeginTransaction();  // creates new transaction
                await dataContext1.Create(new Product() { Name = "Product 1" });

                // create second data context with same connection provider
                using (IDataContext dataContext2 = DataContextTestHelper.GetDataContext(dataContextType, connectionProvider))
                {
                    dataContext2.BeginTransaction(); // adds a reference to the outer transaction
                    await dataContext2.Create(new Product() { Name = "Product 2" });
                    await dataContext2.Create(new Product() { Name = "Product 3" });

                    dataContext2.Commit(); // removes a refernce from the outer transaction but doesn't commit
                }

                await dataContext1.Create(new Product() { Name = "Product 3" });

                dataContext1.Rollback();
            }

            // Assert
            using IDataContext dataContext3 = DataContextTestHelper.GetDataContext(dataContextType, connectionProvider);
            var products = await dataContext3.ReadAll<Product>();
            
            Assert.That(products.Count(), Is.EqualTo(0));
        }
    }
}
