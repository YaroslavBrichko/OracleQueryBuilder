
namespace Algotec.MSTCommon.CommonUtilities.DBAccess.QueryBuilders
{
    public interface IQueryBuilder
    {
        /// <summary>
        ///  final build the query by its rules
        /// </summary>
        /// <returns></returns>
        string BuildQuery();
    }
}
