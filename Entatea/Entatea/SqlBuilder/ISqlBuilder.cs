using System.Collections.Generic;

using Entatea.Model;
using Entatea.Predicate;
using Entatea.Resolvers;

namespace Entatea.SqlBuilder
{
    public interface ISqlBuilder
    {
        string EncapsulationFormat { get; }

        string IsNullFunctionName { get; }

        string GetDateFunctionCall { get; }

        string OrderByAscending { get; }

        string OrderByDescending { get; }

        string GetTableIdentifier<T>() where T : class;

        string GetColumnIdentifier(ClassMap classMap, PropertyMap propertyName);

        string Encapsulate(string identifier);

        string EncapsulateSelect(ClassMap classMap, PropertyMap propertyMap);

        string CallConcatenate(params object[] parameters);

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
