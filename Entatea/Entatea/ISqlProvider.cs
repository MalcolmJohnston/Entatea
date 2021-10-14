using System.Collections.Generic;

using Entatea.Predicate;

namespace Entatea
{
    public interface ISqlProvider
    {
        string GetSelectCountSql<T>(IEnumerable<IPredicate> whereConditions) where T : class;

        string GetSelectAllSql<T>() where T : class;

        string GetSelectWhereSql<T>(IEnumerable<IPredicate> whereConditions) where T : class;

        string GetSelectWhereSql<T>(IEnumerable<IPredicate> whereConditions, object sortOrders, int firstRow, int lastRow) where T : class;

        string GetInsertSql<T>() where T : class;

        string GetUpdateSql<T>(object updateProperties) where T : class;

        string GetDeleteWhereSql<T>(IEnumerable<IPredicate> whereConditions) where T : class;

        string GetSelectNextIdSql<T>() where T : class;
    }
}
