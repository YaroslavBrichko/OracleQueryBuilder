using System;
using System.Collections.Generic;
using System.Text;
using Algotec.MST.Orchestrator;
using Algotec.MST.Orchestrator.Access;
using Algotec.MST.Orchestrator.Access.DBModel;
using Algotec.MSTCommon.CommonUtilities.DBAccess.MedistoreDbEntities;
using Algotec.MSTCommon.CommonUtilities.DBAccess.QueryBuilders;
using NUnit.Framework;

namespace Orchestrator.Test.MSTCommon.CommonUtilities
{
    [TestFixture]
    public class QueryBuilderTester
    {
        private QueryBuilder _builder;



        [SetUp]
        public void SetUp()
        {
            _builder = new QueryBuilder();
        }

        [Test]
        public void QueryBuilderSelect()
        {
            string expected = @"select firstname,lastname,age 
                                from persons 
                                where age > 1";

            var query = _builder.Select("firstname", "lastname", "age")
                                .From("persons")
                                .Where("age > 1")
                                .BuildQuery();


            Assert.AreEqual(ToComparableString(expected), ToComparableString(query));
        }

        [Test]
        public void QueryBuilderSelectWithHint()
        {
            string hint = "INDEX (persons persons_PRIOR_INX)";
            string expected = $@"select /*{hint}*/ firstname,lastname,age 
                                from persons 
                                where age > 1";

            var query = _builder.Hint(hint+"*/")
                                .Select("firstname", "lastname", "age")
                                .From("persons")
                                .Where("age > 1")
                                .BuildQuery();


            Assert.AreEqual(ToComparableString(expected), ToComparableString(query));
        }


        [Test]
        public void QueryBuilderSelectTyped()
        {
            string expected = @"select firstname,lastname,age 
                                from persons 
                                where age > 1";

            var query = _builder.Select<Person>()
                                .From("persons")
                                .Where("age > 1")
                                .BuildQuery();

            Assert.IsTrue(query.IndexOf("ID") == -1,"column 'ID' shoud not be appeared in the columns lis");

            Assert.AreEqual(ToComparableString(expected), ToComparableString(query));
        }

        [Test]
        public void QueryBuilderSelectDistinct()
        {
            string expected = @"select distinct firstname,lastname,age 
                                from persons 
                                where age > 1";

            var query = _builder.SelectDistinct("firstname", "lastname", "age")
                                .From("persons")
                                .Where("age > 1")
                                .BuildQuery();


            Assert.AreEqual(ToComparableString(expected), ToComparableString(query));
        }

        [Test]
        public void QueryBuilderSelectCount()
        {
            string expected = @"select count(*) 
                                from persons 
                                where age > 1";

            var query = _builder.SelectCount()
                                .From("persons")
                                .Where("age > 1")
                                .BuildQuery();


            Assert.AreEqual(ToComparableString(expected), ToComparableString(query));
        }

        [Test]
        public void QueryBuilderSelectOrderBy()
        {
            string expected = @"select firstname,lastname,age  
                                from persons 
                                where age > 1
                                order by firstname asc,age desc";

            var query = _builder.Select("firstname")
                                .Select("lastname")
                                .Select("age")
                                .From("persons")
                                .Where("age > 1")
                                .OrderBy("firstname")
                                .OrderByDesc("age")
                                .BuildQuery();


            Assert.AreEqual(ToComparableString(expected), ToComparableString(query));
        }


        [Test]
        public void QueryBuilderSelectJoin()
        {
            string expected = @"select firstname,lastname,age 
                                from persons p
                                inner join employee e on e.depId = p.DepId
                                left join Department d on d.Id =  e.depId
                                where age > 1 and d is not null";


            var query = _builder.Select("firstname", "lastname", "age")
                          .From("persons p")
                          .InnerJoin("employee e on e.depId = p.DepId")
                          .LeftJoin("Department d on d.Id =  e.depId")
                          .Where("age > 1")
                          .And("d is not null")
                          .BuildQuery();

            Assert.AreEqual(ToComparableString(expected), ToComparableString(query));
        }

        [Test]
        public void QueryBuilderSelectWhere()
        {
            string expected = @"select firstname,lastname,age 
                                from persons p
                                inner join employee e on e.depId = p.DepId
                                where age > 1 and
                                not exists (select 1 from Department d where d = e.depId)";



            var query = _builder.Select("firstname", "lastname", "age")
                          .From("persons p")
                          .InnerJoin("employee e on e.depId = p.DepId")
                          .Where("age > 1")
                          .AndNotExists("select 1 from Department d where d = e.depId")
                          .BuildQuery();

            Assert.AreEqual(ToComparableString(expected), ToComparableString(query));
        }

        [Test]
        public void QueryBuilderSelectWithEmptyFilter()
        {
            string expected = @"select firstname,lastname,age 
                                from persons p
                                inner join employee e on e.depId = p.DepId
                                where 1 = 1 and age > 1 ";



            var query = _builder.Select("firstname", "lastname", "age")
                          .From("persons p")
                          .InnerJoin("employee e on e.depId = p.DepId")
                          .Where(string.Empty)
                          .And("age > 1")
                          .BuildQuery();

            Assert.AreEqual(ToComparableString(expected), ToComparableString(query));
        }

        [Test]
        public void QueryBuilderSelectWhereWithInnerQuery()
        {
            string expected = @"select firstname,lastname,age 
                                from persons p
                                inner join employee e on e.depId = p.DepId
                                where age > 1 and
                                not exists (select 1 from Department d where d = e.depId)";


            var notExists = new QueryBuilder().Select("1")
                                                .From("Department d")
                                                .Where("d = e.depId");

            var query = _builder.Select("firstname", "lastname", "age")
                          .From("persons p")
                          .InnerJoin("employee e on e.depId = p.DepId")
                          .Where("age > 1")
                          .AndNotExists(notExists)
                          .BuildQuery();

            Assert.AreEqual(ToComparableString(expected), ToComparableString(query));
        }

        [Test]
        public void QueryBuilderSelectForUpdateWithMaxRows()
        {
            string expected = @"select firstname,lastname,age 
                                from persons 
                                where age > 1
                                and rownum < 10
                                FOR UPDATE SKIP LOCKED";

            var query = _builder.Select("firstname", "lastname", "age")
                                .From("persons")
                                .Where("age > 1")
                                .Take(10)
                                .ForUpdate()
                                .SkipLocked()
                                .BuildQuery();


            Assert.AreEqual(ToComparableString(expected), ToComparableString(query));
        }

        [Test, TestCaseSource(nameof(FilterCaseData))]
        public void FilterBuilderTest(int count, string expected,string actual)
        {
            Assert.AreEqual(ToComparableString(expected), ToComparableString(actual));
        }


        [Test, TestCaseSource(nameof(Sanity))]
        public void Sanity_TestCases(int count, string expected, string actual)
        {
            Assert.AreEqual(ToComparableString(expected), ToComparableString(actual));
        }


        private static IEnumerable<TestCaseData> FilterCaseData 
        {
            get
            {
                #region case 0
                string expected = "((Name = 'fedor' and age > 10) or (id <2)) ";
                string actual = new FilterBuilder("Name = 'fedor'").And("age > 10").Or(new FilterBuilder("id <2")).BuildFilter();
                yield return new TestCaseData(0,expected, actual);
                #endregion

                #region case 1
                expected = "(UPPER(STUDY_STATUS) in (UPPER('FINAL'),UPPER('READ')) OR STUDY_DATE < SYSDATE - 21)";
                actual = new FilterBuilder("UPPER(STUDY_STATUS)").In(new[] { "UPPER('FINAL')", "UPPER('READ')" }).Or("STUDY_DATE < SYSDATE - 21").BuildFilter();
                yield return new TestCaseData(1,expected, actual);
                #endregion

                #region case 2
                expected = @"((SHIFT IN (1,5,2)  AND WI_TYPE = 'REPORTING')
                  AND ((WI_LOCKED IS NULL) OR(WI_LOCK_TIMEOUT IS NOT NULL AND WI_LOCK_TIMEOUT < SYSDATE)))";

                actual = new FilterBuilder("SHIFT").In(new[] { 1, 5, 2 }).And("WI_TYPE = 'REPORTING'")
                    .And(new FilterBuilder("WI_LOCKED IS NULL").Or(new FilterBuilder("WI_LOCK_TIMEOUT IS NOT NULL").And("WI_LOCK_TIMEOUT < SYSDATE")))
                    .BuildFilter();
                yield return new TestCaseData(2,expected, actual);
                #endregion

                #region case 3
                {
                    /* @"OBJECT_TYPE = 1 AND STUDY_DB_UID = OBJECT_DB_UID
                    AND (UPPER(STUDY_STATUS) in (UPPER('FINAL'),UPPER('READ')) OR (STUDY_DATE < SYSDATE - 21))
                    AND NOT EXISTS (SELECT 1 FROM DATA_MUTATION_TABLE data_mut WHERE data_mut.OBJECT_TYPE = 2 AND data_mut.OBJECT_DB_UID = WI_DB_UID)
                    AND SHIFT IN (1,5,2)
                    AND WI_TYPE = 'REPORTING' 
                    AND (WI_LOCKED IS NULL OR (WI_LOCK_TIMEOUT IS NOT NULL AND WI_LOCK_TIMEOUT < SYSDATE))";
                    */
                    expected = @"((((OBJECT_TYPE = 1 AND STUDY_DB_UID = OBJECT_DB_UID)
                            AND (UPPER(STUDY_STATUS) in (UPPER('FINAL'),UPPER('READ')) OR STUDY_DATE < SYSDATE - 21))
                            AND NOT EXISTS (SELECT 1 FROM DATA_MUTATION_TABLE data_mut WHERE data_mut.OBJECT_TYPE = 2 AND data_mut.OBJECT_DB_UID = WI_DB_UID)
                            AND SHIFT IN (1,5,2)
                            AND WI_TYPE = 'REPORTING') 
                            AND ((WI_LOCKED IS NULL) OR (WI_LOCK_TIMEOUT IS NOT NULL AND WI_LOCK_TIMEOUT < SYSDATE)))";
                    


                    var query = new QueryBuilder().Select("1")
                                                    .From("DATA_MUTATION_TABLE data_mut")
                                                    .Where("data_mut.OBJECT_TYPE = 2")
                                                    .And("data_mut.OBJECT_DB_UID = WI_DB_UID");

                    actual = new FilterBuilder("OBJECT_TYPE = 1")
                                .And("STUDY_DB_UID = OBJECT_DB_UID")
                                .And(new FilterBuilder("UPPER(STUDY_STATUS) in (UPPER('FINAL'),UPPER('READ'))").Or("STUDY_DATE < SYSDATE - 21"))
                                .AndNotExists(query)
                                .And("SHIFT IN (1,5,2)")
                                .And(" WI_TYPE = 'REPORTING'")
                                .And(new FilterBuilder("WI_LOCKED IS NULL").Or(new FilterBuilder("WI_LOCK_TIMEOUT IS NOT NULL").And("WI_LOCK_TIMEOUT < SYSDATE")))
                                .BuildFilter();

                    yield return new TestCaseData(3,expected, actual);
                }
                #endregion

                #region case 4
                {

                    expected = @"SELECT  WI_DB_UID,OBJECT_TYPE,OBJECT_DB_UID,WI_TYPE,WI_INSTANCE_UID,WI_INSERT_TIME, WI_SLA_DEADLINE  
                                 FROM ORC_WORKLIST_ITEMS  , DIDB_STUDIES_VIEW 
                                 WHERE OBJECT_TYPE = 1 
                                 AND STUDY_DB_UID = OBJECT_DB_UID
                                 AND (UPPER(STUDY_STATUS) in (UPPER('FINAL'),UPPER('READ')) OR STUDY_DATE < SYSDATE - 21)
                                 AND NOT EXISTS (SELECT 1 FROM DATA_MUTATION_TABLE data_mut WHERE data_mut.OBJECT_TYPE = 2 AND data_mut.OBJECT_DB_UID = WI_DB_UID)
                                 AND SHIFT IN (1,5,2)
                                 AND WI_TYPE = 'REPORTING' 
                                 AND ((WI_LOCKED IS NULL) OR (WI_LOCK_TIMEOUT IS NOT NULL AND WI_LOCK_TIMEOUT < SYSDATE))
                                 AND rownum < 101 ";

             
                    actual = new QueryBuilder().Select("WI_DB_UID,OBJECT_TYPE,OBJECT_DB_UID,WI_TYPE")
                                                          .Select("WI_INSTANCE_UID,WI_INSERT_TIME, WI_SLA_DEADLINE")
                                                          .From("ORC_WORKLIST_ITEMS")
                                                          .From("DIDB_STUDIES_VIEW")
                                                          .Where("OBJECT_TYPE = 1 ")
                                                          .And("STUDY_DB_UID = OBJECT_DB_UID")
                                                          .And(new FilterBuilder("UPPER(STUDY_STATUS)").In(new[] { "UPPER('FINAL')", "UPPER('READ')" }).Or("STUDY_DATE < SYSDATE - 21"))
                                                          .AndNotExists(new QueryBuilder().Select("1").From("DATA_MUTATION_TABLE data_mut").Where("data_mut.OBJECT_TYPE = 2").And("data_mut.OBJECT_DB_UID = WI_DB_UID"))
                                                          .AndIn("SHIFT",new[] { 1, 5, 2 })
                                                          .And("WI_TYPE = 'REPORTING'")
                                                          .And(new FilterBuilder("WI_LOCKED IS NULL").Or(new FilterBuilder("WI_LOCK_TIMEOUT IS NOT NULL").And("WI_LOCK_TIMEOUT < SYSDATE")))
                                                          .Take(101)
                                                          .BuildQuery();

                    yield return new TestCaseData(4,expected, actual);
                }


                #endregion

            }
        }



        private static IEnumerable<TestCaseData> Sanity
        {
            get
            {
                
                string expected = $"select {DBStudy.ColumnsList} from {DBStudy.DB_TABLE_NAME} where {DBStudy.DB_UID_COLUMN}= :{DBStudy.DB_UID_COLUMN}";

                var query = new QueryBuilder()
                            .Select(DBStudy.ColumnsList)
                            .From(DBStudy.DB_TABLE_NAME)
                            .Where($"{DBStudy.DB_UID_COLUMN}= :{DBStudy.DB_UID_COLUMN}")
                            .BuildQuery();

                yield return new TestCaseData(0,expected, query);
                /********************************************************/

                expected = $"select {string.Join(",", DBWorkListItem.column_type_dictionary.Keys)} from {DBWorkListItem.DB_TABLE_NAME} where WI_DB_UID = :WI_DB_UID";
                query = new QueryBuilder()
                            .Select(DBWorkListItem.column_type_dictionary.Keys)
                            .From(DBWorkListItem.DB_TABLE_NAME)
                            .Where("WI_DB_UID = :WI_DB_UID")
                            .BuildQuery();
                yield return new TestCaseData(1,expected, query);
                /********************************************************/


                expected = "select  WI_DB_UID from Medistore.ORC_WORKLIST_ITEMS where object_db_uid =:resourceDbUid AND OBJECT_TYPE =:object_type";
                query = new QueryBuilder()
                            .Select("WI_DB_UID")
                            .From("Medistore.ORC_WORKLIST_ITEMS")
                            .Where("object_db_uid =:resourceDbUid")
                            .And("OBJECT_TYPE =:object_type")
                            .BuildQuery();
                yield return new TestCaseData(2,expected, query);
                /********************************************************/

                var WI = new DBWorkListItem();
                WI.ObjectDbUid = 123;
                string wiFromClause = DBStudy.DB_TABLE_NAME;
                string wiWhereClause = DBStudy.DB_UID_COLUMN + " = " + WI.ObjectDbUid.Value + " AND ROWNUM = 1";
                expected = BuildSelectStatementString(wiWhereClause, DBStudy.column_type_dictionary.Keys, wiFromClause);

                query = new QueryBuilder()
                            .Select(DBStudy.column_type_dictionary.Keys)
                            .From(DBStudy.DB_TABLE_NAME)
                            .Where($"{DBStudy.DB_UID_COLUMN} = { WI.ObjectDbUid.Value}")
                            .Take(1)
                            .BuildQuery();
                yield return new TestCaseData(3,expected, query);
                /********************************************************/


                string[] adminShifts = new[] { "1,2,3" };
                wiFromClause = @"Medistore.ORC_WORKLIST_ITEMS";
                int study_db_uid = 12;
                wiWhereClause = @"object_db_uid='" + study_db_uid + "' AND OBJECT_TYPE='" + CfgDefs.STUDY_OBJECT_TYPE + "' AND SHIFT in (" + string.Join(",", adminShifts) + ") AND BUCKET_ID <> " +
                    CfgDefs.GENERAL_BUCKET_ID;
                expected = BuildSelectStatementString(wiWhereClause, DBWorkListItem.column_type_dictionary.Keys, wiFromClause);

                query = new QueryBuilder()
                            .Select(DBWorkListItem.column_type_dictionary.Keys)
                            .From("Medistore.ORC_WORKLIST_ITEMS")
                            .Where($"object_db_uid='{study_db_uid}'")
                            .And($"OBJECT_TYPE='{ CfgDefs.STUDY_OBJECT_TYPE}'")
                            .And("SHIFT").In(adminShifts)
                            .And($"BUCKET_ID <> {CfgDefs.GENERAL_BUCKET_ID}")
                            .BuildQuery();
                yield return new TestCaseData(4,expected, query);
                /********************************************************/


                adminShifts = new[] { "1,2,3" };
                wiFromClause = @"Medistore.ORC_WORKLIST_ITEMS";
                wiWhereClause = @"object_db_uid='" + study_db_uid + "' AND OBJECT_TYPE='" + CfgDefs.STUDY_OBJECT_TYPE + "' AND SHIFT in (" + string.Join(",", adminShifts) + ") AND WI_HOTLINE = 'Y' AND BUCKET_ID = " +
                    HotlineConfiguration.TARGET_ADMIN_BUCKET;
                expected = BuildSelectStatementString(wiWhereClause, DBWorkListItem.column_type_dictionary.Keys, wiFromClause);

                query = new QueryBuilder()
                          .Select(DBWorkListItem.column_type_dictionary.Keys)
                          .From("Medistore.ORC_WORKLIST_ITEMS")
                          .Where($"object_db_uid = '{ study_db_uid}'")
                          .And($"OBJECT_TYPE='{ CfgDefs.STUDY_OBJECT_TYPE}'")
                          .AndIn("SHIFT", adminShifts)
                          .And("WI_HOTLINE = 'Y'")
                          .And($"BUCKET_ID = {HotlineConfiguration.TARGET_ADMIN_BUCKET}")
                          .BuildQuery();

                yield return new TestCaseData(5,expected, query);
                /********************************************************/


                StringBuilder sb_wiWhereClause = new StringBuilder(@"object_db_uid='").Append(study_db_uid).Append("' AND OBJECT_TYPE in (").Append(CfgDefs.STUDY_OBJECT_TYPE).Append(",");
                sb_wiWhereClause.Append(OrchestratorAccess.invisible_wi_type_db_uid).Append(") AND SHIFT in (").Append(HotlineConfiguration.GetReportingShiftsListForQuery());
                sb_wiWhereClause.Append(" ) AND WI_HOTLINE = 'Y'");
                expected = BuildSelectStatementString(sb_wiWhereClause.ToString(), DBWorkListItem.column_type_dictionary.Keys, @"Medistore.ORC_WORKLIST_ITEMS");

                query = new QueryBuilder()
                              .Select(DBWorkListItem.column_type_dictionary.Keys)
                              .From("Medistore.ORC_WORKLIST_ITEMS")
                              .Where($"object_db_uid='{study_db_uid}'")
                              .AndIn("OBJECT_TYPE",new[]{CfgDefs.STUDY_OBJECT_TYPE,OrchestratorAccess.invisible_wi_type_db_uid.Value})
                              .AndIn($"SHIFT",HotlineConfiguration.GetReportingShiftsListArrayForQuery())
                              .And($"WI_HOTLINE = 'Y'")
                              .BuildQuery();

                yield return new TestCaseData(6,expected, query);
                /********************************************************/


                string reportingShifts = HotlineConfiguration.GetReportingShiftsListForQuery();
                wiFromClause = @"Medistore.ORC_WORKLIST_ITEMS";
                wiWhereClause = @"object_db_uid='" + study_db_uid + "' AND OBJECT_TYPE in (" + CfgDefs.STUDY_OBJECT_TYPE + "," + OrchestratorAccess.invisible_wi_type_db_uid + ") AND SHIFT in (" + reportingShifts + ")";
                expected = BuildSelectStatementString(wiWhereClause, DBWorkListItem.column_type_dictionary.Keys, wiFromClause);

                query = new QueryBuilder().Select(DBWorkListItem.column_type_dictionary.Keys)
                 .From("Medistore.ORC_WORKLIST_ITEMS")
                 .Where($"object_db_uid='{study_db_uid}'")
                 .AndIn("OBJECT_TYPE",new[]{CfgDefs.STUDY_OBJECT_TYPE,OrchestratorAccess.invisible_wi_type_db_uid})
                 .And($"SHIFT").In(HotlineConfiguration.GetReportingShiftsListArrayForQuery())
                 .BuildQuery();
                yield return new TestCaseData(7,expected, query);
                /********************************************************/

                string where_clause = string.Format(@"{0} = '{1}'",
                  DBStudy.DB_UID_COLUMN, study_db_uid);
                expected = BuildSelectStatementString(where_clause, new List<String> { DBStudy.SUBSPECIALTY_ID }, DBStudy.DB_TABLE_NAME);

                query = new QueryBuilder().Select(DBStudy.SUBSPECIALTY_ID)
                    .From(DBStudy.DB_TABLE_NAME)
                    .Where(where_clause)
                    .BuildQuery();
                yield return new TestCaseData(8,expected, query);
                /********************************************************/
                expected = $"select {string.Join(",", DBWorkListItem.column_type_dictionary.Keys)} from {DBWorkListItem.DB_TABLE_NAME} where WI_DB_UID IN (1,2,3,4)";
                query = new QueryBuilder()
                            .Select(DBWorkListItem.column_type_dictionary.Keys)
                            .From(DBWorkListItem.DB_TABLE_NAME)
                            .WhereIn("WI_DB_UID",new long[] {1,2,3,4})
                            .BuildQuery();
                yield return new TestCaseData(9, expected, query);

                /*************************************************************/
                expected = $"select {DBStudy.ColumnsList} from {DBStudy.DB_TABLE_NAME} where {DBStudy.DB_UID_COLUMN}= :{DBStudy.DB_UID_COLUMN} and STUDY_BODY_PART not in ('a','b','c')";

                 query = new QueryBuilder()
                            .Select(DBStudy.ColumnsList)
                            .From(DBStudy.DB_TABLE_NAME)
                            .Where($"{DBStudy.DB_UID_COLUMN}= :{DBStudy.DB_UID_COLUMN}")
                            .And("STUDY_BODY_PART").NotIn(new[] { "'a'","'b'","'c'"})
                            .BuildQuery();
                yield return new TestCaseData(10, expected, query);
            }
        }


        private string ToComparableString(string query)
        {
            return query.Replace("\r\n", string.Empty)
                        .Replace("\n", string.Empty)
                        .Replace(" ", string.Empty)
                        .ToLower();
        }

        class HotlineConfiguration
        {
            public static int TARGET_ADMIN_BUCKET => 1;
            public static string GetReportingShiftsListForQuery() => "1,2,3,4,5";
            public static string[] GetReportingShiftsListArrayForQuery() => new[] { "1,2,3,4,5" };
        }



        public static string BuildSelectStatementString(string where_clause, IEnumerable<string> select_columns, string from_clause, bool distinct = false)
        {
            StringBuilder query_sb = new StringBuilder();
            query_sb.Append("SELECT ");
            if (distinct)
            {
                query_sb.Append("DISTINCT ");
            }
            query_sb.Append(string.Join(",", select_columns));
            query_sb.Append(" FROM ");
            query_sb.Append(from_clause);
            if (!string.IsNullOrEmpty(where_clause))
            {
                query_sb.Append(" WHERE ");
                query_sb.Append(where_clause);
            }

            return query_sb.ToString();
        }

        class Person {
            [DbIgnore]
            public int ID { get; set; }
            [DbColumn("firstname")]
            public string firstname { get; set; }
            [DbColumn("lastname")]
            public string lastname { get; set; }
            public int age { get; set; }
        }

    }
}
