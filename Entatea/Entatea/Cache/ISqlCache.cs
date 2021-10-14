namespace Entatea.Cache
{
    public interface ISqlCache
    {
        string GetSelectAllSql<T>();

        string GetSelectCountSql<T>();

        string GetInsertSql<T>();

        string GetSelectNextIdSql<T>();

        void SetSelectAllSql<T>(string sql);

        void SetSelectCountSql<T>(string sql);

        void SetInsertSql<T>(string sql);

        void SetSelectNextIdSql<T>(string sql);
    }
}
