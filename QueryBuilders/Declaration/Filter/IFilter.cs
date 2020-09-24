

using System.Collections.Generic;

namespace Algotec.MSTCommon.CommonUtilities.DBAccess.QueryBuilders
{
    public interface IFilterBuilder 
    {
        string BuildFilter();
    }

    public interface IFilter : IFilterBuilder
    {
        IFilter And(string filter);
        IFilter Or(string filter);
        IFilter And(IFilterBuilder filter);
        IFilter Or(IFilterBuilder filter);
        IFilter AndNotExists(IQueryBuilder filter);
        IFilter AndExists(IQueryBuilder filter);
        IFilter In<T>(IEnumerable<T> filter);
        IFilter NotIn<T>(IEnumerable<T> filter);
        IFilter AndIn<T>(string column, IEnumerable<T> filter);
        IFilter AndNotIn<T>(string column, IEnumerable<T> filter);
        IFilter OrIn<T>(string column, IEnumerable<T> filter);
        IFilter OrNotIn<T>(string column, IEnumerable<T> filter);
    }
}
