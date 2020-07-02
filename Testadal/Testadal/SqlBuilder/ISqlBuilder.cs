using Testadal.Model;
using Testadal.Predicate;

namespace Testadal.SqlBuilder
{
    public interface ISqlBuilder
    {
        string EncapsulationFormat { get; }

        string IsNullFunctionName { get; }

        string GetDateFunctionCall { get; }

        string OrderByAscending { get; }

        string OrderByDescending { get; }

        string GetTableIdentifier<T>();

        string Encapsulate(string identifier);

        string EncapsulateSelect(PropertyMap propertyMap);

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

        string PredicateToSql(IPredicate predicate);
    }
}
