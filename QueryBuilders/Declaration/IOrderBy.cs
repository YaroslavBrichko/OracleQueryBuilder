

namespace Algotec.MSTCommon.CommonUtilities.DBAccess.QueryBuilders
{
    public interface IOrderBy : IGrouping
    {
        /// <summary>
        /// order by asc
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        IOrderBy OrderBy(string filter);
        /// <summary>
        /// order by desc
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        IOrderBy OrderByDesc(string filter);
    }
}
