namespace Algotec.MSTCommon.CommonUtilities.DBAccess.QueryBuilders
{
    public interface ICompleteBuilder : IQueryBuilder
    {
        /// <summary>
        /// put 'FOR UPDATE' expression at the end of the query
        /// The method is the last one in the chain
        /// </summary>
        /// <returns></returns>
        ICompleteBuilder ForUpdate();
        /// <summary>
        /// put 'SKIP LOCKED' expression at the end of the query
        /// The method is the last one in the chain
        /// </summary>
        /// <returns></returns>
        ICompleteBuilder SkipLocked();
    }
}
