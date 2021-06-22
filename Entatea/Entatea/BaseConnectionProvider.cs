using System;
using System.Collections.Concurrent;
using System.Data;
using System.Threading;

namespace Entatea
{
    public abstract class BaseConnectionProvider : IConnectionProvider
    {
        protected readonly string connectionString;
        
        protected IDbTransaction sharedTransaction;

        protected readonly ConcurrentDictionary<IDataContext, IDbConnection> dataContextConnections = new ConcurrentDictionary<IDataContext, IDbConnection>(); 
        protected readonly ConcurrentStack<IDataContext> dataContextsInTransaction = new ConcurrentStack<IDataContext>();

        protected readonly SemaphoreSlim transactionSemaphore = new SemaphoreSlim(1);

        public BaseConnectionProvider(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public IDbConnection GetConnection(IDataContext dataContext)
        {
            if (dataContext == null)
            {
                throw new NullReferenceException();
            }

            if (dataContext.State == DataContextState.InTransaction)
            {
                return this.GetTransactionConnection();
            }

            return this.GetOrAddConnection(dataContext);
        }

        public IDbTransaction GetTransaction()
        {
            try
            {
                this.transactionSemaphore.Wait();
                return this.sharedTransaction;
            }
            finally
            {
                this.transactionSemaphore.Release();
            }
        }

        public void BeginTransaction(IDataContext dataContext)
        {
            try
            {
                this.transactionSemaphore.Wait();

                if (this.sharedTransaction == null)
                {
                    if (!this.dataContextConnections.TryRemove(dataContext, out IDbConnection conn))
                    {
                        conn = this.GetOpenConnection();
                    }
                    this.sharedTransaction = conn.BeginTransaction();
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

                if (this.sharedTransaction == null)
                {
                    throw new InvalidOperationException("Cannot commit when no transaction");
                }

                if (this.IsLastIn(dataContext))
                {
                    if (this.dataContextsInTransaction.Count == 1)
                    {
                        this.sharedTransaction.Commit();
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

                throwException = this.dataContextsInTransaction.Count > 1;

                if (sharedTransaction != null)
                {
                    this.sharedTransaction.Rollback();
                    this.DisposeTransaction();
                }
            }
            catch
            {
            }
            finally
            {
                this.dataContextsInTransaction.Clear();
                this.transactionSemaphore.Release();
            }

            if (throwException)
            {
                throw new InvalidOperationException("Inner transaction rolled back, outer transactions cannot continue");
            }
        }

        public void NotifyDisposed(IDataContext dataContext)
        {
            if (dataContext.State == DataContextState.InTransaction)
            {
                this.Rollback(dataContext);
            }

            if (dataContext.State == DataContextState.NoTransaction)
            {
                this.DisposeConnection(dataContext);
            }
        }

        protected abstract IDbConnection GetOpenConnection();

        private IDbConnection GetTransactionConnection()
        {
            IDbTransaction tran = this.GetTransaction();
            return tran.Connection;
        }

        private IDbConnection GetOrAddConnection(IDataContext dataContext)
        {
            if (this.dataContextConnections.ContainsKey(dataContext))
            {
                return this.dataContextConnections[dataContext];
            }

            if (this.dataContextConnections.TryAdd(dataContext, this.GetOpenConnection()))
            {
                return this.dataContextConnections[dataContext];
            }

            throw new DataException("Failed to open connection.");
        }

        private void DisposeTransaction()
        {
            sharedTransaction.Dispose();
            sharedTransaction = null;
        }

        private void DisposeConnection(IDataContext dataContext)
        {
            if (this.dataContextConnections.TryRemove(dataContext, out IDbConnection conn) && conn != null)
            {
                conn.Dispose();
            }
        }

        private void EnlistDataContext(IDataContext dataContext)
        {
            this.dataContextsInTransaction.Push(dataContext);
        }

        private void DelistDataContext(IDataContext dataContext)
        {
            if (this.IsLastIn(dataContext))
            {
                this.dataContextsInTransaction.TryPop(out IDataContext _);
            }
        }

        private bool IsLastIn(IDataContext dataContext)
        {
            this.dataContextsInTransaction.TryPeek(out IDataContext lastDataContext);
            return object.ReferenceEquals(dataContext, lastDataContext);
        }
    }
}
