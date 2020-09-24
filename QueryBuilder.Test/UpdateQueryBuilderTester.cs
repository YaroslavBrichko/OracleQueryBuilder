using System.Collections.Generic;
using Algotec.MSTCommon.CommonUtilities.DBAccess.QueryBuilders;
using NUnit.Framework;

namespace Orchestrator.Test.MSTCommon.CommonUtilities
{
    [TestFixture]
    public class UpdateQueryBuilderTester
    {
 

        [Test, TestCaseSource(nameof(UpdateCaseData))]
        public void UpdateBuilderTest(int count, string expected, string actual)
        {
            Assert.AreEqual(ToComparableString(expected), ToComparableString(actual));
        }


        private string ToComparableString(string query)
        {
            return query.Replace("\r\n", string.Empty)
                        .Replace("\n", string.Empty)
                        .Replace("\t", string.Empty)
                        .Replace(" ", string.Empty)
                        .ToLower();
        }

        private static IEnumerable<TestCaseData> UpdateCaseData
        {
            get
            {
                string expected;
                string actual;

                #region case 1
                expected = @"UPDATE customers SET last_name = 'Anderson' WHERE customer_id = 5000";

                actual = new UpdateQueryBuilder().Update("customers")
                                                 .Set("last_name = 'Anderson'")
                                                 .Where("customer_id = 5000")
                                                 .BuildQuery();

                yield return new TestCaseData(1, expected, actual);
                #endregion

                #region case 2
                expected = "update didb_studies  set study_date = sysdate, sla = 5, study_status = 'UNREAD', original_storing_ae = 'ddffgg', study_has_dicom_data = 'Y', wi_handled = 'N' where study_db_uid in (17)";

                actual = new UpdateQueryBuilder().Update("didb_studies")
                                                 .Set("study_date = sysdate")
                                                 .Set("sla = 5")
                                                 .Set("study_status = 'UNREAD'")
                                                 .Set("original_storing_ae = 'ddffgg'")
                                                 .Set("study_has_dicom_data = 'Y'")
                                                 .Set("wi_handled = 'N'")
                                                 .WhereIn("study_db_uid",new[] { 17 })
                                                 .BuildQuery();
                yield return new TestCaseData(2, expected, actual);
                #endregion

                #region case 3
                expected = @" update didb_studies s set s.wi_handled = 'N'
	                          where exists ( SELECT t.column_value FROM TABLE(WI_DB_UID_Tbl) t
	                          inner join ORC_WORKLIST_ITEMS i on i.wi_db_uid = column_value
	                          and s.STUDY_DB_UID = i.object_db_uid)";

                actual = new UpdateQueryBuilder().Update("didb_studies s")
                                                 .Set("s.wi_handled = 'N'")
                                                 .WhereExists(new QueryBuilder().Select("t.column_value")
                                                                                .From("TABLE(WI_DB_UID_Tbl) t")
                                                                                .InnerJoin("ORC_WORKLIST_ITEMS i on i.wi_db_uid = column_value and s.STUDY_DB_UID = i.object_db_uid"))
                                                 .BuildQuery();

                yield return new TestCaseData(3, expected, actual);
                #endregion

                #region case 4
                expected = @"UPDATE dest_tab tt
                            SET    (tt.code, tt.description) = (SELECT st.code, st.description
                                                                FROM   source_tab st
                                                                WHERE  st.id = tt.id)
                            WHERE  EXISTS (SELECT 1
                                           FROM   source_tab
                                           WHERE  id = tt.id)
                            ";

                string set = new QueryBuilder().Select("st.code")
                                               .Select("st.description")
                                               .From("source_tab st")
                                               .Where("st.id = tt.id")
                                               .BuildQuery();

                actual = new UpdateQueryBuilder().Update("dest_tab tt")
                                                 .Set($@"(tt.code, tt.description) = ({set})")
                                                 .WhereExists(new QueryBuilder().Select("1")
                                                                                .From("source_tab")
                                                                                .Where("id = tt.id"))
                                                 .BuildQuery();
                yield return new TestCaseData(4, expected, actual);
                #endregion

                #region case 5
                expected = @"UPDATE /*+ PARALLEL(8) */ dest_tab tt
                           SET    (tt.code, tt.description) = (SELECT st.code, st.description
                                    FROM   source_tab st
                                    WHERE  st.id = tt.id)
                           WHERE  EXISTS (SELECT 1 FROM   source_tab  WHERE  id = tt.id)";

                set = new QueryBuilder().Select("st.code")
                                              .Select("st.description")
                                              .From("source_tab st")
                                              .Where("st.id = tt.id")
                                              .BuildQuery();

                actual = new UpdateQueryBuilder().Hint("+ PARALLEL(8)")
                                                 .Update("dest_tab tt")
                                                 .Set($@"(tt.code, tt.description) = ({set})")
                                                 .WhereExists(new QueryBuilder().Select("1").From("source_tab").Where("id = tt.id"))
                                                 .BuildQuery();
                yield return new TestCaseData(5, expected, actual);
                #endregion

                #region case 6

                expected = @"UPDATE (SELECT tt.id, tt.code,tt.description,st.code AS st_code,
                                     st.description AS st_description
                                     FROM   dest_tab tt, source_tab st
                                     WHERE  tt.id = st.id) ilv
                        SET    ilv.code = ilv.st_code,
                        ilv.description = ilv.st_description";

                actual = new UpdateQueryBuilder().Update(new QueryBuilder().Select("tt.id, tt.code,tt.description,st.code AS st_code")
                                                                           .Select("st.description AS st_description")
                                                                           .From("dest_tab tt, source_tab st")
                                                                           .Where("tt.id = st.id"), "ilv")
                                                 .Set("ilv.code = ilv.st_code")
                                                 .Set("ilv.description = ilv.st_description")
                                                 .BuildQuery();
                 
                yield return new TestCaseData(6, expected, actual);
                #endregion

                #region case 7

                expected = @"UPDATE (SELECT tt.id, tt.code, tt.description, st.code AS st_code,  st.description AS st_description
                                     FROM   dest_tab tt, source_tab st
                                     WHERE  tt.id = st.id) ilv
                            SET    ilv.code = ilv.st_code,
                                   ilv.description = ilv.st_description
                            WHERE  ilv.id  <= 2500
                            AND ilv.code IN (1,2,3,4,5)";

                actual = new UpdateQueryBuilder().Update(new QueryBuilder().Select("tt.id, tt.code,tt.description,st.code AS st_code")
                                                                           .Select("st.description AS st_description")
                                                                           .From("dest_tab tt, source_tab st")
                                                                           .Where("tt.id = st.id"), "ilv")
                                                .Set("ilv.code = ilv.st_code")
                                                .Set("ilv.description = ilv.st_description")
                                                .Where("ilv.id  <= 2500")
                                                .AndIn("ilv.code",new[] { 1, 2, 3, 4, 5 })
                                                .BuildQuery();
                yield return new TestCaseData(7, expected, actual);
                #endregion
            }
        }





    }
}
