namespace Testadal
{
    public interface ISqlProvider
    {
        string GetSelectCountSql<T>(object whereConditions);

        string GetSelectAllSql<T>();

        string GetSelectByIdSql<T>();

        string GetSelectWhereSql<T>(object whereConditions);

        string GetSelectWhereSql<T>(object whereConditions, object sortOrders, int firstRow, int lastRow);

        string GetInsertSql<T>();

        string GetUpdateSql<T>(object updateProperties);

        string GetDeleteByIdSql<T>();

        string GetDeleteWhereSql<T>(object whereConditions);

        string GetSelectNextIdSql<T>();
    }
}
