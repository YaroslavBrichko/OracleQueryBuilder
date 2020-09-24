
namespace Algotec.MSTCommon.CommonUtilities.DBAccess.QueryBuilders
{
    public interface ILimitBuilder : IOrderBy
    {
        /// <summary>
        /// Puts ROWNUM(num) expression at the end of the query
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        ILimitBuilder Take(int num);
    }
}
