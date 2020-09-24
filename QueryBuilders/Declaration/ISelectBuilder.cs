
namespace Algotec.MSTCommon.CommonUtilities.DBAccess.QueryBuilders
{
    public interface ISelectBuilder
    {
        /// <summary>
        /// Columns taking part of the 'select' statement.
        /// May be used in chain calls
        /// </summary>
        /// <param name="column"></param>
        /// <returns></returns>
        ISelectBuilder Select( params string[] column);
        /// <summary>
        /// Puts 'FROM' at the begining of the expression
        /// </summary>
        /// <param name="from"></param>
        /// <returns></returns>
        IFromBuilder From(string from);
    }
}
