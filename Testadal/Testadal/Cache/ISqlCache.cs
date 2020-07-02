using System;
using System.Collections.Generic;
using System.Text;

namespace Testadal.Cache
{
    public interface ISqlCache
    {
        string GetSelectAllSql<T>();

        string GetSelectByIdSql<T>();

        string GetSelectCountSql<T>();

        string GetInsertSql<T>();

        string GetDeleteByIdSql<T>();

        string GetSelectNextIdSql<T>();

        void SetSelectAllSql<T>(string sql);

        void SetSelectByIdSql<T>(string sql);

        void SetSelectCountSql<T>(string sql);

        void SetInsertSql<T>(string sql);

        void SetDeleteByIdSql<T>(string sql);

        void SetSelectNextIdSql<T>(string sql);
    }
}
