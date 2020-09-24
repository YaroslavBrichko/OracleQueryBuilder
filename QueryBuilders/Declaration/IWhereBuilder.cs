

using System.Collections.Generic;

namespace Algotec.MSTCommon.CommonUtilities.DBAccess.QueryBuilders
{
    public interface IWhereBuilder : IQueryBuilder, IOrderBy, ILimitBuilder, IGrouping
    {
        /// <summary>
        /// puts 'AND' expression at the beginning of the filter
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        IWhereBuilder And(string filter);
        /// <summary>
        /// puts 'AND' expression at the beginning of the filter
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        IWhereBuilder And(IFilterBuilder filter);
        /// <summary>
        /// puts 'OR' expression at the beginning of the filter
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        IWhereBuilder Or(string filter);
        /// <summary>
        /// puts 'OR' expression at the beginning of the filter
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        IWhereBuilder Or(IFilterBuilder filter);
        /// <summary>
        /// puts 'AND WHERE NOT EXISTS(filter)' expression at the beginning of the filter
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        IWhereBuilder AndNotExists(string filter);
        /// <summary>
        /// puts 'AND EXISTS(filter)' expression at the beginning of the filter
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        IWhereBuilder AndExists(string filter);
        /// <summary>
        /// puts 'AND NOT EXISTS(filter)' expression at the beginning of the filter
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        IWhereBuilder AndNotExists(IQueryBuilder filter);
        /// <summary>
        /// puts 'AND EXISTS(filter)' expression at the beginning of the filter
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        IWhereBuilder AndExists(IQueryBuilder filter);
        /// <summary>
        /// add 'IN (comma)'
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="filter"></param>
        /// <returns></returns>
        IWhereBuilder In<T>(IEnumerable<T> filter);
        /// <summary>
        /// add 'NOT IN (comma)'
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="filter"></param>
        /// <returns></returns>
        IWhereBuilder NotIn<T>(IEnumerable<T> filter);
        /// <summary>
        /// add 'AND {column} IN (comma)'
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="column"></param>
        /// <param name="filter"></param>
        /// <returns></returns>
        IWhereBuilder AndIn<T>(string column,IEnumerable<T> filter);
        /// <summary>
        /// add 'AND {column} NOT IN (comma)'
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="column"></param>
        /// <param name="filter"></param>
        /// <returns></returns>
        IWhereBuilder AndNotIn<T>(string column, IEnumerable<T> filter);
        /// <summary>
        /// add 'OR {column} IN (comma)'
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="column"></param>
        /// <param name="filter"></param>
        /// <returns></returns>
        IWhereBuilder OrIn<T>(string column, IEnumerable<T> filter);
        /// <summary>
        /// add 'OR {column} NOT IN (comma)'
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="column"></param>
        /// <param name="filter"></param>
        /// <returns></returns>
        IWhereBuilder OrNotIn<T>(string column, IEnumerable<T> filter);
    }
}
