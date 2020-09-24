

using System.Collections.Generic;

namespace Algotec.MSTCommon.CommonUtilities.DBAccess.QueryBuilders.Update
{
    public interface IWhere : IQueryBuilder
    {
        /// <summary>
        /// adds 'AND' at the begining of the expression
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        IWhere And(string filter);
        /// <summary>
        /// adds 'OR' at the begining of the expression
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        IWhere Or(string filter);
        /// <summary>
        /// adds 'AND' at the begining of the expression
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        IWhere And(IFilterBuilder filter);
        /// <summary>
        /// adds 'OR' at the begining of the expression
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        IWhere Or(IFilterBuilder filter);
        /// <summary>
        /// puts 'AND WHERE NOT EXISTS(filter)' expression at the beginning of the filter
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        IWhere AndNotExists(string filter);
        /// <summary>
        /// puts 'AND EXISTS(filter)' expression at the beginning of the filter
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        IWhere AndExists(string filter);
        /// <summary>
        /// puts 'AND NOT EXISTS(filter)' expression at the beginning of the filter
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        IWhere AndNotExists(IQueryBuilder filter);
        /// <summary>
        /// puts 'AND EXISTS(filter)' expression at the beginning of the filter
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        IWhere AndExists(IQueryBuilder filter);
        /// <summary>
        /// add 'IN (comma)'
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="filter"></param>
        /// <returns></returns>
        IWhere In<T>(IEnumerable<T> filter);
        /// <summary>
        /// add 'NOT IN (comma)'
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="filter"></param>
        /// <returns></returns>
        IWhere NotIn<T>(IEnumerable<T> filter);
        /// <summary>
        /// add 'AND {column} IN (comma)'
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="column"></param>
        /// <param name="filter"></param>
        /// <returns></returns>
        IWhere AndIn<T>(string column, IEnumerable<T> filter);
        /// <summary>
        ///  add 'AND {column} NOT IN (comma)'
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="column"></param>
        /// <param name="filter"></param>
        /// <returns></returns>
        IWhere AndNotIn<T>(string column, IEnumerable<T> filter);
        /// <summary>
        ///  add 'OR {column} IN (comma)'
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="column"></param>
        /// <param name="filter"></param>
        /// <returns></returns>
        IWhere OrIn<T>(string column, IEnumerable<T> filter);
        /// <summary>
        ///  add 'OR {column} NOT IN (comma)'
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="column"></param>
        /// <param name="filter"></param>
        /// <returns></returns>
        IWhere OrNotIn<T>(string column, IEnumerable<T> filter);
    }
}
