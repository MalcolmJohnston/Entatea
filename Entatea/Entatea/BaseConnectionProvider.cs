using System;
using System.Collections.Concurrent;
using System.Data;
using System.Threading;

namespace Entatea
{
    public abstract class BaseConnectionProvider : IConnectionProvider
    {
        protected readonly string connectionString;
        
        protected IDbConnection connection;
        protected IDbTransaction transaction;

        protected readonly ConcurrentStack<IDataContext> enlistedDataContexts = new ConcurrentStack<IDataContext>();

        protected readonly SemaphoreSlim connectionSemaphore = new SemaphoreSlim(1);
        protected readonly SemaphoreSlim transactionSemaphore = new SemaphoreSlim(1);

        public BaseConnectionProvider(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public IDbConnection GetConnection()
        {
            try
            {
                this.transactionSemaphore.Wait();

                if (this.transaction != null)
                {
                    return this.transaction.Connection;
                }
            }
            finally
            {
                this.transactionSemaphore.Release();
            }

            try
            {
                this.connectionSemaphore.Wait();

                this.CloseConnection(this.connection);
                this.connection = this.GetOpenConnection();

                return this.connection;
            }
            finally
            {
                this.connectionSemaphore.Release();
            }
        }

        public IDbTransaction GetTransaction()
        {
            try
            {
                this.transactionSemaphore.Wait();
                return this.transaction;
            }
            finally
            {
                this.transactionSemaphore.Release();
            }
        }

        public void CloseConnection()
        {
            try
            {
                this.connectionSemaphore.Wait();
                this.CloseConnection(this.connection);
            }
            finally
            {
                this.connectionSemaphore.Release();
            }
        }

        public void BeginTransaction(IDataContext dataContext)
        {
            try
            {
                this.transactionSemaphore.Wait();

                this.CloseConnection();

                if (this.transaction == null)
                {
                    IDbConnection conn = this.GetOpenConnection();
                    this.transaction = conn.BeginTransaction();
                }
            }
            finally
            {
                this.EnlistDataContext(dataContext);
                this.transactionSemaphore.Release();
            }
        }

        public void Commit(IDataContext dataContext)
        {
            try
            {
                this.transactionSemaphore.Wait();

                if (this.transaction == null)
                {
                    throw new InvalidOperationException("Cannot commit when no transaction");
                }

                if (this.IsLastIn(dataContext))
                {
                    if (this.enlistedDataContexts.Count == 1)
                    {
                        this.transaction.Commit();
                        this.DisposeTransaction();
                    }

                    this.DelistDataContext(dataContext);
                }
                else
                {
                    throw new InvalidOperationException("The last data context to enter the transaction must commit first.");
                }
            }
            finally
            {
                this.transactionSemaphore.Release();
            }
        }

        public void Rollback(IDataContext dataContext)
        {
            bool throwException = false;
            try
            {
                this.transactionSemaphore.Wait();

                throwException = this.enlistedDataContexts.Count != 1;

                if (transaction != null)
                {
                    this.transaction.Rollback();
                    this.DisposeTransaction();
                }
            }
            catch
            {
            }
            finally
            {
                this.enlistedDataContexts.Clear();
                this.transactionSemaphore.Release();
            }

            if (throwException)
            {
                throw new InvalidOperationException("Inner transaction rolled back, outer transactions cannot continue");
            }
        }

        public void NotifyDisposed(IDataContext dataContext)
        {
            if (this.IsLastIn(dataContext))
            {
                this.Rollback(dataContext);
            }
        }

        protected abstract IDbConnection GetOpenConnection();

        private void CloseConnection(IDbConnection connection)
        {
            if (connection != null && connection.State == ConnectionState.Open)
            {
                connection.Close();
            }
        }

        private void DisposeTransaction()
        {
            transaction.Dispose();
            transaction = null;
        }

        private void EnlistDataContext(IDataContext dataContext)
        {
            this.enlistedDataContexts.Push(dataContext);
        }

        private void DelistDataContext(IDataContext dataContext)
        {
            if (this.IsLastIn(dataContext))
            {
                this.enlistedDataContexts.TryPop(out dataContext);
            }
        }

        private bool IsLastIn(IDataContext dataContext)
        {
            this.enlistedDataContexts.TryPeek(out IDataContext lastDataContext);
            return object.ReferenceEquals(dataContext, lastDataContext);
        }
    }
}
