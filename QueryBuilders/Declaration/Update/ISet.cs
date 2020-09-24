
using System.Collections.Generic;

namespace Algotec.MSTCommon.CommonUtilities.DBAccess.QueryBuilders.Update
{
    public interface ISet :IQueryBuilder
    {
        /// <summary>
        /// expression taking part on 'SET' clause
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        ISet Set(string query);
        /// <summary>
        /// adds 'WHERE' at the begining of the expression
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        IWhere Where(string filter);
        /// <summary>
        ///  adds 'WHERE' at the begining of the expression
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        IWhere Where(IFilterBuilder filter);
        /// <summary>
        ///  adds 'WHERE EXISTS' at the begining of the expression
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        IWhere WhereNotExists(string filter);
        /// <summary>
        ///  adds 'WHERE EXISTS' clause at the begining of the expression
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        IWhere WhereExists(string filter);
        /// <summary>
        ///  adds 'WHERE NOT EXISTS' clause at the begining of the expression
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        IWhere WhereNotExists(IQueryBuilder filter);
        /// <summary>
        ///  adds 'WHERE EXISTS' clause at the begining of the expression
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        IWhere WhereExists(IQueryBuilder filter);
        /// <summary>
        /// adding 'WHERE {column} IN (comma)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="column"></param>
        /// <param name="filter"></param>
        /// <returns></returns>
        IWhere WhereIn<T>(string column,IEnumerable<T> filter);
        /// <summary>
        /// adding 'WHERE {column} NOT IN (comma)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="column"></param>
        /// <param name="filter"></param>
        /// <returns></returns>
        IWhere WhereNotIn<T>(string column, IEnumerable<T> filter);
    }
}
