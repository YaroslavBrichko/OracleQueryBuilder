using System;


namespace Algotec.MSTCommon.CommonUtilities.DBAccess.QueryBuilders
{
    [AttributeUsage(AttributeTargets.Property)]
    public class DbColumnAttribute : Attribute
    {
        public string ColumnName { get; }
        public DbColumnAttribute(string columnName)
        {
            ColumnName = columnName;
        }
    }

    /// <summary>
    /// the marked property will not appear in the column list
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class DbIgnoreAttribute : Attribute
    { }

}
