using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Algotec.MSTCommon.CommonUtilities.DBAccess.QueryBuilders
{
    public class UpdateQueryBuilder : Update.IUpdate, Update.ISet, Update.IWhere
    {
        private string _tableName;
        private List<string> _set;
        private List<string> _where;
        private string _hint = null;


        public Update.IUpdate Update(string tableName)
        {
            _tableName = tableName;
            return this;
        }
        public Update.IUpdate Update(IQueryBuilder query)
        {
            _tableName = $"({query.BuildQuery()})";
            return this;
        }

        public Update.IUpdate Update(IQueryBuilder query,string alias)
        {
            _tableName = $"({query.BuildQuery()}) {alias}";
            return this;
        }

        public UpdateQueryBuilder Hint(string hint)
        {
            string temp = hint;
            if (!temp.StartsWith("/*"))
                temp = $"/*{temp}";
            if (!hint.EndsWith("*/"))
                temp = $"{temp}*/";
            _hint = temp;
            return this;
        }

        #region IUpdate implementation
        Update.ISet Update.IUpdate.Set(string query)
        {
            _set = new List<string>() { query };
            return this;
        }
        #endregion

        #region ISet implementation
        Update.ISet Update.ISet.Set(string query)
        {
            _set.Add(query);
            return this;
        }

        Update.IWhere Update.ISet.Where(string filter)
        {
            _where = new List<string>() { filter };
            return this;
        }

        Update.IWhere Update.ISet.Where(IFilterBuilder filter)
        {
            _where = new List<string>() { filter.BuildFilter() };
            return this;
        }
        Update.IWhere Update.ISet.WhereNotExists(string filter)
        {
            _where = new List<string>() { $"NOT EXISTS ({filter})" };
            return this;
        }

        Update.IWhere Update.ISet.WhereExists(string filter)
        {
            _where = new List<string>() { $"EXISTS ({filter})" };
            return this;
        }

        Update.IWhere Update.ISet.WhereNotExists(IQueryBuilder filter)
        {
            _where = new List<string>() { $"NOT EXISTS ({filter.BuildQuery()})" };
            return this;
        }

        Update.IWhere Update.ISet.WhereExists(IQueryBuilder filter)
        {
            _where = new List<string>() { $"EXISTS ({filter.BuildQuery()})" };
            return this;
        }
        Update.IWhere Update.ISet.WhereIn<T>(string column, IEnumerable<T> filter)
        {
            _where = new List<string>() { $"{column} IN  {ToString(filter)}" };
            return this;
        }

        Update.IWhere Update.ISet.WhereNotIn<T>(string column, IEnumerable<T> filter)
        {
            _where = new List<string>() { $"{column} NOT IN  {ToString(filter)}" };
            return this;
        }
        #endregion

        #region IWhere implementation
        Update.IWhere Update.IWhere.And(string filter)
        {
            _where.Add($"AND {filter}");
            return this;
        }
        Update.IWhere Update.IWhere.Or(string filter)
        {
            _where.Add($"OR {filter}");
            return this;
        }

        Update.IWhere Update.IWhere.And(IFilterBuilder filter)
        {
            _where.Add($"AND {filter.BuildFilter()}");
            return this;
        }
        Update.IWhere Update.IWhere.Or(IFilterBuilder filter)
        {
            _where.Add($"OR {filter.BuildFilter()}");
            return this;
        }
        Update.IWhere Update.IWhere.AndNotExists(string filter)
        {
            _where.Add($"AND NOT EXISTS ({filter})");
            return this;
        }

        Update.IWhere Update.IWhere.AndExists(string filter)
        {
            _where.Add($"AND EXISTS ({filter})");
            return this;
        }

        Update.IWhere Update.IWhere.AndNotExists(IQueryBuilder filter)
        {
            _where.Add($"AND NOT EXISTS ({filter.BuildQuery()})");
            return this;
        }

        Update.IWhere Update.IWhere.AndExists(IQueryBuilder filter)
        {
            _where.Add($"AND EXISTS ({filter.BuildQuery()})");
            return this;
        }
        Update.IWhere Update.IWhere.In<T>(IEnumerable<T> filter)
        {
            _where.Add($"IN {ToString(filter)}");
            return this;
        }
        Update.IWhere Update.IWhere.NotIn<T>(IEnumerable<T> filter)
        {
            _where.Add($"NOT IN {ToString(filter)}");
            return this;
        }
        Update.IWhere Update.IWhere.AndIn<T>(string column, IEnumerable<T> filter)
        {
            _where.Add($"AND {column} IN {ToString(filter)}");
            return this;
        }
        Update.IWhere Update.IWhere.AndNotIn<T>(string column, IEnumerable<T> filter)
        {
            _where.Add($"AND {column} NOT IN {ToString(filter)}");
            return this;
        }
        Update.IWhere Update.IWhere.OrIn<T>(string column, IEnumerable<T> filter)
        {
            _where.Add($"OR {column} IN {ToString(filter)}");
            return this;
        }
        Update.IWhere Update.IWhere.OrNotIn<T>(string column, IEnumerable<T> filter)
        {
            _where.Add($"OR {column} NOT IN {ToString(filter)}");
            return this;
        }
        #endregion

        #region IQueryBuilder implementation
        string IQueryBuilder.BuildQuery()
        {
            return RemoveEmptyLines($@"UPDATE {Hint()} {_tableName} 
                                       SET {string.Join(",", _set)}
                                       {Where()}".TrimEnd());
        }

        #endregion

        private string Hint() => string.IsNullOrEmpty(_hint) ? string.Empty : _hint;
        private string Where() => _where?.Any() == true ? $"WHERE {string.Join(" ", _where)}" : string.Empty;
        private string RemoveEmptyLines(string lines)
        {
            return Regex.Replace(lines, @"^\s*$\n|\r", string.Empty, RegexOptions.Multiline).TrimEnd();
        }
        private string ToString<T>(IEnumerable<T> collection)
        {
            if (collection?.Any() == false)
                return string.Empty;
            return $"({ string.Join(",", collection)})";
        }
    }
}
