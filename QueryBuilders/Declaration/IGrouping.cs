

namespace Algotec.MSTCommon.CommonUtilities.DBAccess.QueryBuilders
{
    public interface IGrouping : ICompleteBuilder
    {
        /// <summary>
        /// add GROUP BY expressionat the beginig of the filter
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        ICompleteBuilder GroupBy(string filter);
    }
}
