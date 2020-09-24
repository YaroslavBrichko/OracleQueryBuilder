using System.Collections.Generic;
using System.Linq;

namespace Algotec.MSTCommon.CommonUtilities.DBAccess.QueryBuilders
{
    public class FilterBuilder : IFilter
    {
        private List<string> _filters = new List<string>();

        #region .ctor
        /// <summary>
        /// 
        /// </summary>
        /// <param name="filter">initial filter</param>
        public FilterBuilder(string filter)
        {
            _filters.Add(filter);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="filter">initial filter</param>
        public FilterBuilder(IFilterBuilder filter)
        {
            _filters.Add(filter.BuildFilter());
        }
        #endregion


        #region IFilter implementation
        public IFilter And(string filter)
        {
            _filters.Add($"AND {VerifyEmptyFilter(filter)}");
            return this;
        }

        public IFilter Or(string filter)
        {
            _filters.Add($"OR {VerifyEmptyFilter(filter)}");
            return this;
        }

        public IFilter And(IFilterBuilder filter)
        {
            _filters = alignList();
             _filters.Add($"AND {filter.BuildFilter()}");
            return this;
        }

        public IFilter Or(IFilterBuilder filter)
        {
            _filters = alignList();
            _filters.Add($"OR {filter.BuildFilter()}");
            return this;
        }
        public IFilter AndNotExists(IQueryBuilder filter)
        {
            _filters = alignList();
            _filters.Add($"AND NOT EXISTS ({filter.BuildQuery()})");
            return this;
        }
        public IFilter AndExists(IQueryBuilder filter)
        {
            _filters = alignList();
            _filters.Add($"AND EXISTS ({filter.BuildQuery()})");
            return this;
        }
        public IFilter In<T>(IEnumerable<T> filter)
        {
            _filters.Add($"IN {ToString(filter)}");
            return this;
        }
        public IFilter NotIn<T>(IEnumerable<T> filter)
        {
            _filters.Add($"NOT IN {ToString(filter)}");
            return this;
        }
        public IFilter AndIn<T>(string column, IEnumerable<T> filter)
        {
            _filters.Add($"AND {column} IN {ToString(filter)}");
            return this;
        }
        public IFilter AndNotIn<T>(string column, IEnumerable<T> filter)
        {
            _filters.Add($"AND {column} NOT IN {ToString(filter)}");
            return this;
        }
        public IFilter OrIn<T>(string column, IEnumerable<T> filter)
        {
            _filters.Add($"OR {column} IN {ToString(filter)}");
            return this;
        }
        public IFilter OrNotIn<T>(string column, IEnumerable<T> filter)
        {
            _filters.Add($"OR {column} NOT IN {ToString(filter)}");
            return this;
        }
        #endregion

        #region IFilterBuilder implementation
        string IFilterBuilder.BuildFilter()
        {
            return $"({string.Join(" ", _filters)})";
        }
        #endregion

        private List<string> alignList()
        {
            string actual = $"({ string.Join(" ", _filters)})";
            return new List<string> { actual };
        }
        /// <summary>
        /// replace empty string with "1 = 1" expression
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        private string VerifyEmptyFilter(string filter)
        {
            if (string.IsNullOrEmpty(filter))
                return " 1 = 1 ";
            return filter;
        }
        private string ToString<T>(IEnumerable<T> collection)
        {
            if (collection?.Any() == false)
                return string.Empty;
            return $"({ string.Join(",", collection)})";
        }
    }
}
