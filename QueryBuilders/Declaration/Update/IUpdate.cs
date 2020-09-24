

namespace Algotec.MSTCommon.CommonUtilities.DBAccess.QueryBuilders.Update
{
    public interface IUpdate : IQueryBuilder
    {
        /// <summary>
        /// expression taking part on 'SET' clause
        /// add 'SET' clause at the beginig of the expression
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        ISet Set(string query);
    }
}
