using System.Collections.Generic;

namespace Algotec.MSTCommon.CommonUtilities.DBAccess.QueryBuilders
{
    public interface IFromBuilder : IQueryBuilder, IOrderBy, ILimitBuilder
    {
        /// <summary>
        /// Puts 'WHERE' clause at the begining of the expression
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        IWhereBuilder Where(string filter);
        /// <summary>
        ///  Puts 'WHERE' clause at the begining of the expression
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        IWhereBuilder Where(IFilterBuilder filter);
        /// <summary>
        ///  Puts 'WHERE NOT EXISTS' clause at the begining of the expression
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        IWhereBuilder WhereNotExists(string filter);
        /// <summary>
        ///  Puts 'WHERE EXISTS' clause at the begining of the expression
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        IWhereBuilder WhereExists(string filter);
        /// <summary>
        ///  Puts 'WHERE NOT EXISTS' clause at the begining of the expression
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        IWhereBuilder WhereNotExists(IQueryBuilder filter);
        /// <summary>
        ///  Puts 'WHERE EXISTS' clause at the begining of the expression
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        IWhereBuilder WhereExists(IQueryBuilder filter);
        /// <summary>
        /// adding 'WHERE {column} IN (comma)'
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="column"></param>
        /// <param name="filter"></param>
        /// <returns></returns>
        IWhereBuilder WhereIn<T>(string column, IEnumerable<T> filter);
        /// <summary>
        /// adding 'WHERE {column} NOT IN (comma)'
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="column"></param>
        /// <param name="filter"></param>
        /// <returns></returns>
        IWhereBuilder WhereNotIn<T>(string column, IEnumerable<T> filter);
        /// <summary>
        ///  Puts 'FROM' clause at the begining of the expression
        /// </summary>
        /// <param name="from"></param>
        /// <returns></returns>
        IFromBuilder From(string from);
        /// <summary>
        ///  Puts 'INNER JOIN' clause at the begining of the expression
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        IFromBuilder InnerJoin(string query);
        /// <summary>
        ///  Puts 'LEFT JOIN' clause at the begining of the expression
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        IFromBuilder LeftJoin(string query);
    }
}
