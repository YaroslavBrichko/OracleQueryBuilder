using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Algotec.MSTCommon.CommonUtilities.DBAccess.QueryBuilders
{
    public class QueryBuilder : ISelectBuilder, IFromBuilder, IWhereBuilder, ICompleteBuilder
    {
        private List<string> _columns = new List<string>();
        private List<string> _from = new List<string>();
        private List<string> _where = new List<string>();
        private List<string> _innerJoin = new List<string>();
        private List<string> _leftJoin = new List<string>();
        private bool _selectDistinct = false;
        private List<string> _orderBy = new List<string>();
        private List<string> _orderByDesc = new List<string>();
        private List<string> _gropBy = new List<string>();
        private int _rowNum = 0;
        private bool _forUpdate = false;
        private bool _skipLocked = false;
        private string _hint = string.Empty;

        #region .ctor
        public QueryBuilder() { }
        #endregion

        #region initialization public methods
        public ISelectBuilder SelectDistinct(params string[] columns)
        {
            _selectDistinct = true;
            Select(columns);
            return this;
        }
        public ISelectBuilder Select(params string[] columns)
        {
            _columns.AddRange(columns);
            return this;
        }
        public ISelectBuilder Select(IEnumerable<string> columns)
        {
            _columns.AddRange(columns);
            return this;
        }
        /// <summary>
        /// adding a hint to the beginning of the query. Tags '/*' and '*/' are not necessary
        /// </summary>
        /// <param name="hint"></param>
        /// <returns></returns>
        public ISelectBuilder Hint(string hint)
        {
            string temp = hint;
            if (!temp.StartsWith("/*"))
                temp = $"/*{temp}";
            if (!hint.EndsWith("*/"))
                temp = $"{temp}*/";
            _hint = temp;
            return this;
        }
        public ISelectBuilder SelectCount(string count="*")
        {
            _columns.Add($" COUNT({count}) ");
            return this;
        }

        public ISelectBuilder SelectDistinct<T>()
        {
            _selectDistinct = true;
            return Select<T>();
        }

        public ISelectBuilder Select<T>()
        {
            return Select(typeof(T));
        }

        public ISelectBuilder Select(Type objectType)
        {
            var properties = objectType.GetProperties();
            foreach(var prop in properties)
            {
                var ignoreAttrib = prop.GetCustomAttribute<DbIgnoreAttribute>();
                //if property marked with 'DbIgnore' attribute then  skip
                if (ignoreAttrib == null)
                {
                    var attrib = prop.GetCustomAttribute<DbColumnAttribute>();
                    if (attrib != null)
                        _columns.Add(attrib.ColumnName);
                    else
                        _columns.Add(prop.Name);
                }
            }
            return this;
        }
        #endregion


        #region ISelectBuilder implementation

        ISelectBuilder ISelectBuilder.Select(params string[] columns)
        {
            _columns.AddRange(columns);
            return this;
        }

        IFromBuilder ISelectBuilder.From(string from)
        {
            _from.Add(from);
            return this;
        }

        #endregion

        #region IFormBuilder implementation 

        IWhereBuilder IFromBuilder.Where(string where)
        {
            _where.Add(VerifyEmptyFilter(where));
            return this;
        }
        IWhereBuilder IFromBuilder.Where(IFilterBuilder filter)
        {
            _where.Add(filter.BuildFilter());
            return this;
        }
        IWhereBuilder IFromBuilder.WhereNotExists(string where)
        {
            _where.Add($" NOT EXISTS ({where})");
            return this;
        }
        IWhereBuilder IFromBuilder.WhereExists(string where)
        {
            _where.Add($" EXISTS ({where})");
            return this;
        }
        IWhereBuilder IFromBuilder.WhereNotExists(IQueryBuilder filter)
        {
            _where.Add($" NOT EXISTS ({filter.BuildQuery()})");
            return this;
        }

        IWhereBuilder IFromBuilder.WhereExists(IQueryBuilder filter)
        {
            _where.Add($" EXISTS ({filter.BuildQuery()})");
            return this;
        }

        IWhereBuilder IFromBuilder.WhereIn<T>(string column, IEnumerable<T> filter)
        {
            _where.Add($"{column} IN {ToString(filter)}");
            return this;
        }

        IWhereBuilder IFromBuilder.WhereNotIn<T>(string column, IEnumerable<T> filter)
        {
            _where.Add($"{column} NOT IN {ToString(filter)}");
            return this;
        }

        IFromBuilder IFromBuilder.From(string from)
        {
            _from.Add(from);
            return this;
        }

        IFromBuilder IFromBuilder.InnerJoin(string query)
        {
            _innerJoin.Add(query);
            return this;
        }

        IFromBuilder IFromBuilder.LeftJoin(string query)
        {
            _leftJoin.Add(query);
            return this;
        }
        #endregion

        #region ILimitBuilder implementation
        ILimitBuilder ILimitBuilder.Take(int num)
        {
            _rowNum = num;
            return this;
        }
        #endregion

        #region   IWhereBuilder implementation
        IWhereBuilder IWhereBuilder.And(string filter)
        {
            _where.Add($"AND {VerifyEmptyFilter(filter)}");
            return this;
        }
        IWhereBuilder IWhereBuilder.And(IFilterBuilder filter)
        {
            _where.Add($"AND {filter.BuildFilter()}");
            return this;
        }
        IWhereBuilder IWhereBuilder.Or(string filter)
        {
            _where.Add($" OR {VerifyEmptyFilter(filter)}");
            return this;
        }
        IWhereBuilder IWhereBuilder.Or(IFilterBuilder filter)
        {
            _where.Add($"OR {filter.BuildFilter()}");
            return this;
        }
        IWhereBuilder IWhereBuilder.AndNotExists(string filter)
        {
            _where.Add($" AND NOT EXISTS ({filter})");
            return this;
        }
        IWhereBuilder IWhereBuilder.AndExists(string filter)
        {
            _where.Add($" AND EXISTS ({filter})");
            return this;
        }
        IWhereBuilder IWhereBuilder.AndNotExists(IQueryBuilder filter)
        {
            _where.Add($" AND NOT EXISTS ({filter.BuildQuery()})");
            return this;
        }
        IWhereBuilder IWhereBuilder.AndExists(IQueryBuilder filter)
        {
            _where.Add($" AND EXISTS ({filter.BuildQuery()})");
            return this;
        }

        IWhereBuilder IWhereBuilder.In<T>(IEnumerable<T> filter)
        {
            _where.Add($"IN {ToString(filter)}");
            return this;
        }
        IWhereBuilder IWhereBuilder.NotIn<T>(IEnumerable<T> filter)
        {
            _where.Add($"NOT IN {ToString(filter)}");
            return this;
        }

        IWhereBuilder IWhereBuilder.AndIn<T>(string column, IEnumerable<T> filter)
        {
            _where.Add($"AND {column} IN {ToString(filter)}");
            return this;
        }
        IWhereBuilder IWhereBuilder.AndNotIn<T>(string column, IEnumerable<T> filter)
        {
            _where.Add($"AND {column} NOT IN {ToString(filter)}");
            return this;
        }
        IWhereBuilder IWhereBuilder.OrIn<T>(string column, IEnumerable<T> filter)
        {
            _where.Add($"OR {column} IN {ToString(filter)}");
            return this;
        }
        IWhereBuilder IWhereBuilder.OrNotIn<T>(string column, IEnumerable<T> filter)
        {
            _where.Add($"OR {column} NOT IN {ToString(filter)}");
            return this;
        }

        #endregion

        #region IOrderBy implementation
        IOrderBy IOrderBy.OrderBy(string filter)
        {
            _orderBy.Add(filter);
            return this;
        }

        IOrderBy IOrderBy.OrderByDesc(string filter)
        {
            _orderByDesc.Add(filter);
            return this;
        }

        #endregion
        #region IGrouping implementation       
        ICompleteBuilder IGrouping.GroupBy(string filter)
        {
            _gropBy.Add(filter);
            return this;
        }
        #endregion

        #region ICompleteBuilder implementation
        ICompleteBuilder ICompleteBuilder.ForUpdate()
        {
            _forUpdate = true;
            return this;
        }
        ICompleteBuilder ICompleteBuilder.SkipLocked()
        {
            _skipLocked = true;
            return this;
        }
        #endregion

        #region IQueryBuilder implementation
        string IQueryBuilder.BuildQuery()
        {
            string columns = string.Join(",", _columns);
            string from = string.Join(",", _from);

            return RemoveEmptyLines(
                     $@"SELECT {Distinct()} {Hint()} {columns}
                        FROM {from} 
                        {InnerJoin()}
                        {LeftJoin()}
                        {Where()}
                        {RowNum()} {OrderBy()} {OrderByDesc()} {GroupBy()} {ForUpdate()} {SkipLocked()}".TrimEnd());
        }
        #endregion



        private string Distinct() => _selectDistinct ? " DISTINCT " : string.Empty;
        private string Hint() => string.IsNullOrEmpty(_hint) ? string.Empty : _hint;
        private string OrderBy() => _orderBy.Any() ? $"ORDER BY {string.Join(",", _orderBy)} ASC" : string.Empty;
        private string OrderByDesc()
        {
            if (_orderByDesc.Any())
            {
                string command = _orderBy.Any() ? "," : " ORDER BY ";
                return $" {command} {string.Join(",", _orderByDesc)} DESC";
            }
            return string.Empty;
        }

        public string GroupBy()=>_gropBy.Any() ? $"GROUP BY {string.Join(",", _gropBy)}" : string.Empty;

        private string RowNum()
        {
            if (_rowNum > 0)
            {
                string comparation = _rowNum == 1 ? "=" : "<";
                if (_where.Any())
                    return $" AND ROWNUM {comparation} {_rowNum}";
                return $" WHERE ROWNUM {comparation} {_rowNum}";
            }
            return string.Empty;
        }

        private string InnerJoin()
        {
            if (_innerJoin.Any())
                return $" INNER JOIN {string.Join(",", _innerJoin)}";
            return string.Empty;
        }
        private string LeftJoin()
        {
            if (_leftJoin.Any())
                return $" LEFT JOIN {string.Join(",", _leftJoin)}";
            return string.Empty;
        }

        private string Where()
        {
            if (_where.Any())
                return $" WHERE {string.Join(" ", _where)}";
            return string.Empty;
        }

        private string ForUpdate()
        {
            if (_forUpdate)
                return " FOR UPDATE ";
            return string.Empty;
        }

        private string SkipLocked()
        {
            if (_skipLocked)
                return " SKIP LOCKED ";
            return string.Empty;
        }

        private string RemoveEmptyLines(string lines)
        {
            return Regex.Replace(lines, @"^\s*$\n|\r", string.Empty, RegexOptions.Multiline).TrimEnd();
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
