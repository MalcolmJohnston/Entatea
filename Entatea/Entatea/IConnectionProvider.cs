﻿using System.Data;

namespace Entatea
{
    public interface IConnectionProvider
    {
        IDbConnection GetConnection(IDataContext dataContext);

        IDbTransaction GetTransaction();

        void BeginTransaction(IDataContext dataContext);

        bool Commit(IDataContext dataContext);

        void Rollback(IDataContext dataContext);

        void NotifyDisposed(IDataContext dataContext);
    }
}
