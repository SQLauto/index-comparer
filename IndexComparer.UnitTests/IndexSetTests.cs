using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using IndexComparer.BusinessObjects;

namespace IndexComparer.UnitTests
{
    [TestClass]
    public class IndexSetTests
    {
        /********************************************************************************************************
        *   Greetings, test runner!  To get the integration tests to work, you'll obviously need to provide     *
        *   a legitimate SQL Server instance name to which you have access.  Change the below parameter         *
        *   and then you should be able to go for the green.                                                    *
        *                                                                                                       *
        *   SPECIAL NOTE:  Windows Authentication MUST be enabled for your test instance.  Otherwise,           *
        *   the tests using the default connection string will fail.                                            *
        ********************************************************************************************************/
        private string InstanceName = "localhost";
        private string DatabaseName = "master";
        //This should be an instance which does NOT exist on your network.
        private string BadInstanceName = "badinstancename";
        //This should be an instance which does NOT exist on your network.
        private string BadDatabaseName = "baddatabasename";

        #region General Equality Tests

        [TestMethod]
        public void IndexSet_IsEqual_False()
        {
            IndexComparer.BusinessObjects.IndexSet isp3 = new IndexComparer.BusinessObjects.IndexSet();
            isp3.ServerName = "Server 1";
            isp3.DatabaseName = "Database 1";
            isp3.SchemaName = "dbo";
            isp3.TableName = "Table1";
            isp3.IndexName = "Index3";
            isp3.IndexType = "NONCLUSTERED";
            isp3.IndexIsUnique = true;
            isp3.Columns = "SomeColumn,SomeColumn2";
            isp3.IncludedColumns = "SomeColumn4,SomeColumn3";
            isp3.HasFilter = true;
            isp3.FilterDefinition = "(ID < 500)";

            IndexComparer.BusinessObjects.IndexSet iss4 = new IndexComparer.BusinessObjects.IndexSet();
            iss4.ServerName = "Server 1";
            iss4.DatabaseName = "Database 1";
            iss4.SchemaName = "dbo";
            iss4.TableName = "Table3";
            iss4.IndexName = "Index1";
            iss4.IndexType = "HEAP";

            Assert.IsFalse(isp3.Equals(iss4));
            Assert.AreNotEqual(isp3.GetHashCode(), iss4.GetHashCode());
            Assert.IsFalse(isp3.TotallyEquals(iss4));
        }

        [TestMethod]
        public void IndexSet_IsEqual_True()
        {
            IndexComparer.BusinessObjects.IndexSet isp3 = new IndexComparer.BusinessObjects.IndexSet();
            isp3.ServerName = "Server 1";
            isp3.DatabaseName = "Database 1";
            isp3.SchemaName = "dbo";
            isp3.TableName = "Table1";
            isp3.IndexName = "Index3";
            isp3.IndexType = "NONCLUSTERED";
            isp3.IndexIsUnique = true;
            isp3.Columns = "SomeColumn,SomeColumn2";
            isp3.IncludedColumns = "SomeColumn4,SomeColumn3";
            isp3.HasFilter = true;
            isp3.FilterDefinition = "(ID < 500)";

            IndexComparer.BusinessObjects.IndexSet iss4 = new IndexComparer.BusinessObjects.IndexSet();
            iss4.ServerName = "Server 1";
            iss4.DatabaseName = "Database 1";
            iss4.SchemaName = "dbo";
            iss4.TableName = "Table1";
            iss4.IndexName = "Index3";
            iss4.IndexType = "NONCLUSTERED";
            iss4.IndexIsUnique = true;
            iss4.Columns = "SomeColumn,SomeColumn2";
            iss4.IncludedColumns = "SomeColumn4,SomeColumn3";
            iss4.HasFilter = true;
            iss4.FilterDefinition = "(ID < 500)";

            Assert.IsTrue(isp3.Equals(iss4));
            Assert.AreEqual(isp3.GetHashCode(), iss4.GetHashCode());
            Assert.IsTrue(isp3.TotallyEquals(iss4));
        }

        [TestMethod]
        public void IndexSet_IsEqual_DifferentNonCountedFields_True()
        {
            IndexComparer.BusinessObjects.IndexSet isp3 = new IndexComparer.BusinessObjects.IndexSet();
            isp3.ServerName = "Server 1";
            isp3.DatabaseName = "Database 1";
            isp3.SchemaName = "dbo";
            isp3.TableName = "Table1";
            isp3.IndexName = "Index3";
            isp3.IndexType = "NONCLUSTERED";
            isp3.IndexIsUnique = true;
            isp3.Columns = "SomeColumn,SomeColumn2";
            isp3.IncludedColumns = "SomeColumn4,SomeColumn3";
            isp3.HasFilter = true;
            isp3.FilterDefinition = "(ID < 500)";

            IndexComparer.BusinessObjects.IndexSet iss4 = new IndexComparer.BusinessObjects.IndexSet();
            iss4.ServerName = "Server 1";
            iss4.DatabaseName = "Database 1";
            iss4.SchemaName = "dbo";
            iss4.TableName = "Table1";
            iss4.IndexName = "Index3";
            iss4.IndexType = "HEAP";

            Assert.IsTrue(isp3.Equals(iss4));
            Assert.AreEqual(isp3.GetHashCode(), iss4.GetHashCode());
            Assert.IsFalse(isp3.TotallyEquals(iss4));
        }

        #endregion

        #region Minor Changes To Equality Tests

        #region Equality Tests

        [TestMethod]
        public void IndexSet_IsEqual_SchemaNameDiffers_False()
        {
            IndexComparer.BusinessObjects.IndexSet isp3 = new IndexComparer.BusinessObjects.IndexSet();
            isp3.ServerName = "Server 1";
            isp3.DatabaseName = "Database 1";
            isp3.SchemaName = "dbo";
            isp3.TableName = "Table1";
            isp3.IndexName = "Index3";
            isp3.IndexType = "NONCLUSTERED";
            isp3.IndexIsUnique = true;
            isp3.Columns = "SomeColumn,SomeColumn2";
            isp3.IncludedColumns = "SomeColumn4,SomeColumn3";
            isp3.HasFilter = true;
            isp3.FilterDefinition = "(ID < 500)";

            IndexComparer.BusinessObjects.IndexSet iss4 = new IndexComparer.BusinessObjects.IndexSet();
            iss4.ServerName = "Server 1";
            iss4.DatabaseName = "Database 1";
            iss4.SchemaName = "bogus";
            iss4.TableName = "Table1";
            iss4.IndexName = "Index3";
            iss4.IndexType = "NONCLUSTERED";
            iss4.IndexIsUnique = true;
            iss4.Columns = "SomeColumn,SomeColumn2";
            iss4.IncludedColumns = "SomeColumn4,SomeColumn3";
            iss4.HasFilter = true;
            iss4.FilterDefinition = "(ID < 500)";

            Assert.IsFalse(isp3.Equals(iss4));
            Assert.AreNotEqual(isp3.GetHashCode(), iss4.GetHashCode());
            Assert.IsFalse(isp3.TotallyEquals(iss4));
        }

        [TestMethod]
        public void IndexSet_IsEqual_TableNameDiffers_False()
        {
            IndexComparer.BusinessObjects.IndexSet isp3 = new IndexComparer.BusinessObjects.IndexSet();
            isp3.ServerName = "Server 1";
            isp3.DatabaseName = "Database 1";
            isp3.SchemaName = "dbo";
            isp3.TableName = "Table1";
            isp3.IndexName = "Index3";
            isp3.IndexType = "NONCLUSTERED";
            isp3.IndexIsUnique = true;
            isp3.Columns = "SomeColumn,SomeColumn2";
            isp3.IncludedColumns = "SomeColumn4,SomeColumn3";
            isp3.HasFilter = true;
            isp3.FilterDefinition = "(ID < 500)";

            IndexComparer.BusinessObjects.IndexSet iss4 = new IndexComparer.BusinessObjects.IndexSet();
            iss4.ServerName = "Server 1";
            iss4.DatabaseName = "Database 1";
            iss4.SchemaName = "dbo";
            iss4.TableName = "bogus";
            iss4.IndexName = "Index3";
            iss4.IndexType = "NONCLUSTERED";
            iss4.IndexIsUnique = true;
            iss4.Columns = "SomeColumn,SomeColumn2";
            iss4.IncludedColumns = "SomeColumn4,SomeColumn3";
            iss4.HasFilter = true;
            iss4.FilterDefinition = "(ID < 500)";

            Assert.IsFalse(isp3.Equals(iss4));
            Assert.AreNotEqual(isp3.GetHashCode(), iss4.GetHashCode());
            Assert.IsFalse(isp3.TotallyEquals(iss4));
        }

        [TestMethod]
        public void IndexSet_IsEqual_IndexNameDiffers_False()
        {
            IndexComparer.BusinessObjects.IndexSet isp3 = new IndexComparer.BusinessObjects.IndexSet();
            isp3.ServerName = "Server 1";
            isp3.DatabaseName = "Database 1";
            isp3.SchemaName = "dbo";
            isp3.TableName = "Table1";
            isp3.IndexName = "Index3";
            isp3.IndexType = "NONCLUSTERED";
            isp3.IndexIsUnique = true;
            isp3.Columns = "SomeColumn,SomeColumn2";
            isp3.IncludedColumns = "SomeColumn4,SomeColumn3";
            isp3.HasFilter = true;
            isp3.FilterDefinition = "(ID < 500)";

            IndexComparer.BusinessObjects.IndexSet iss4 = new IndexComparer.BusinessObjects.IndexSet();
            iss4.ServerName = "Server 1";
            iss4.DatabaseName = "Database 1";
            iss4.SchemaName = "dbo";
            iss4.TableName = "Table1";
            iss4.IndexName = "bogus";
            iss4.IndexType = "NONCLUSTERED";
            iss4.IndexIsUnique = true;
            iss4.Columns = "SomeColumn,SomeColumn2";
            iss4.IncludedColumns = "SomeColumn4,SomeColumn3";
            iss4.HasFilter = true;
            iss4.FilterDefinition = "(ID < 500)";

            Assert.IsFalse(isp3.Equals(iss4));
            Assert.AreNotEqual(isp3.GetHashCode(), iss4.GetHashCode());
            Assert.IsFalse(isp3.TotallyEquals(iss4));
        }

        #endregion

        #region Total Equality Tests

        [TestMethod]
        public void IndexSet_IsTotallyEqual_IndexIsUniqueDiffers_False()
        {
            IndexComparer.BusinessObjects.IndexSet isp3 = new IndexComparer.BusinessObjects.IndexSet();
            isp3.ServerName = "Server 1";
            isp3.DatabaseName = "Database 1";
            isp3.SchemaName = "dbo";
            isp3.TableName = "Table1";
            isp3.IndexName = "Index3";
            isp3.IndexType = "NONCLUSTERED";
            isp3.IndexIsUnique = true;
            isp3.Columns = "SomeColumn,SomeColumn2";
            isp3.IncludedColumns = "SomeColumn4,SomeColumn3";
            isp3.HasFilter = true;
            isp3.FilterDefinition = "(ID < 500)";

            IndexComparer.BusinessObjects.IndexSet iss4 = new IndexComparer.BusinessObjects.IndexSet();
            iss4.ServerName = "Server 1";
            iss4.DatabaseName = "Database 1";
            iss4.SchemaName = "dbo";
            iss4.TableName = "Table1";
            iss4.IndexName = "Index3";
            iss4.IndexType = "NONCLUSTERED";
            iss4.IndexIsUnique = false;
            iss4.Columns = "SomeColumn,SomeColumn2";
            iss4.IncludedColumns = "SomeColumn4,SomeColumn3";
            iss4.HasFilter = true;
            iss4.FilterDefinition = "(ID < 500)";

            Assert.IsTrue(isp3.Equals(iss4));
            Assert.AreEqual(isp3.GetHashCode(), iss4.GetHashCode());
            Assert.IsFalse(isp3.TotallyEquals(iss4));
        }

        [TestMethod]
        public void IndexSet_IsTotallyEqual_IndexTypeDiffers_False()
        {
            IndexComparer.BusinessObjects.IndexSet isp3 = new IndexComparer.BusinessObjects.IndexSet();
            isp3.ServerName = "Server 1";
            isp3.DatabaseName = "Database 1";
            isp3.SchemaName = "dbo";
            isp3.TableName = "Table1";
            isp3.IndexName = "Index3";
            isp3.IndexType = "NONCLUSTERED";
            isp3.IndexIsUnique = true;
            isp3.Columns = "SomeColumn,SomeColumn2";
            isp3.IncludedColumns = "SomeColumn4,SomeColumn3";
            isp3.HasFilter = true;
            isp3.FilterDefinition = "(ID < 500)";

            IndexComparer.BusinessObjects.IndexSet iss4 = new IndexComparer.BusinessObjects.IndexSet();
            iss4.ServerName = "Server 1";
            iss4.DatabaseName = "Database 1";
            iss4.SchemaName = "dbo";
            iss4.TableName = "Table1";
            iss4.IndexName = "Index3";
            iss4.IndexType = "CLUSTERED";
            iss4.IndexIsUnique = true;
            iss4.Columns = "SomeColumn,SomeColumn2";
            iss4.IncludedColumns = "SomeColumn4,SomeColumn3";
            iss4.HasFilter = true;
            iss4.FilterDefinition = "(ID < 500)";

            Assert.IsTrue(isp3.Equals(iss4));
            Assert.AreEqual(isp3.GetHashCode(), iss4.GetHashCode());
            Assert.IsFalse(isp3.TotallyEquals(iss4));
        }

        [TestMethod]
        public void IndexSet_IsTotallyEqual_ColumnsDiffers_False()
        {
            IndexComparer.BusinessObjects.IndexSet isp3 = new IndexComparer.BusinessObjects.IndexSet();
            isp3.ServerName = "Server 1";
            isp3.DatabaseName = "Database 1";
            isp3.SchemaName = "dbo";
            isp3.TableName = "Table1";
            isp3.IndexName = "Index3";
            isp3.IndexType = "NONCLUSTERED";
            isp3.IndexIsUnique = true;
            isp3.Columns = "SomeColumn,SomeColumn2";
            isp3.IncludedColumns = "SomeColumn4,SomeColumn3";
            isp3.HasFilter = true;
            isp3.FilterDefinition = "(ID < 500)";

            IndexComparer.BusinessObjects.IndexSet iss4 = new IndexComparer.BusinessObjects.IndexSet();
            iss4.ServerName = "Server 1";
            iss4.DatabaseName = "Database 1";
            iss4.SchemaName = "dbo";
            iss4.TableName = "Table1";
            iss4.IndexName = "Index3";
            iss4.IndexType = "NONCLUSTERED";
            iss4.IndexIsUnique = true;
            iss4.Columns = "SomeColumn,SomeColumn5";
            iss4.IncludedColumns = "SomeColumn4,SomeColumn3";
            iss4.HasFilter = true;
            iss4.FilterDefinition = "(ID < 500)";

            Assert.IsTrue(isp3.Equals(iss4));
            Assert.AreEqual(isp3.GetHashCode(), iss4.GetHashCode());
            Assert.IsFalse(isp3.TotallyEquals(iss4));
        }

        [TestMethod]
        public void IndexSet_IsTotallyEqual_IncludedColumnsDiffers_False()
        {
            IndexComparer.BusinessObjects.IndexSet isp3 = new IndexComparer.BusinessObjects.IndexSet();
            isp3.ServerName = "Server 1";
            isp3.DatabaseName = "Database 1";
            isp3.SchemaName = "dbo";
            isp3.TableName = "Table1";
            isp3.IndexName = "Index3";
            isp3.IndexType = "NONCLUSTERED";
            isp3.IndexIsUnique = true;
            isp3.Columns = "SomeColumn,SomeColumn2";
            isp3.IncludedColumns = "SomeColumn4,SomeColumn3";
            isp3.HasFilter = true;
            isp3.FilterDefinition = "(ID < 500)";

            IndexComparer.BusinessObjects.IndexSet iss4 = new IndexComparer.BusinessObjects.IndexSet();
            iss4.ServerName = "Server 1";
            iss4.DatabaseName = "Database 1";
            iss4.SchemaName = "dbo";
            iss4.TableName = "Table1";
            iss4.IndexName = "Index3";
            iss4.IndexType = "NONCLUSTERED";
            iss4.IndexIsUnique = true;
            iss4.Columns = "SomeColumn,SomeColumn2";
            iss4.IncludedColumns = "SomeColumn5,SomeColumn3";
            iss4.HasFilter = true;
            iss4.FilterDefinition = "(ID < 500)";

            Assert.IsTrue(isp3.Equals(iss4));
            Assert.AreEqual(isp3.GetHashCode(), iss4.GetHashCode());
            Assert.IsFalse(isp3.TotallyEquals(iss4));
        }

        [TestMethod]
        public void IndexSet_IsTotallyEqual_HasFilterDiffers_False()
        {
            IndexComparer.BusinessObjects.IndexSet isp3 = new IndexComparer.BusinessObjects.IndexSet();
            isp3.ServerName = "Server 1";
            isp3.DatabaseName = "Database 1";
            isp3.SchemaName = "dbo";
            isp3.TableName = "Table1";
            isp3.IndexName = "Index3";
            isp3.IndexType = "NONCLUSTERED";
            isp3.IndexIsUnique = true;
            isp3.Columns = "SomeColumn,SomeColumn2";
            isp3.IncludedColumns = "SomeColumn4,SomeColumn3";
            isp3.HasFilter = true;
            isp3.FilterDefinition = "(ID < 500)";

            IndexComparer.BusinessObjects.IndexSet iss4 = new IndexComparer.BusinessObjects.IndexSet();
            iss4.ServerName = "Server 1";
            iss4.DatabaseName = "Database 1";
            iss4.SchemaName = "dbo";
            iss4.TableName = "Table1";
            iss4.IndexName = "Index3";
            iss4.IndexType = "NONCLUSTERED";
            iss4.IndexIsUnique = true;
            iss4.Columns = "SomeColumn,SomeColumn2";
            iss4.IncludedColumns = "SomeColumn4,SomeColumn3";
            iss4.HasFilter = false;
            iss4.FilterDefinition = "(ID < 500)";

            Assert.IsTrue(isp3.Equals(iss4));
            Assert.AreEqual(isp3.GetHashCode(), iss4.GetHashCode());
            Assert.IsFalse(isp3.TotallyEquals(iss4));
        }

        [TestMethod]
        public void IndexSet_IsTotallyEqual_FilterDefinitionDiffers_False()
        {
            IndexComparer.BusinessObjects.IndexSet isp3 = new IndexComparer.BusinessObjects.IndexSet();
            isp3.ServerName = "Server 1";
            isp3.DatabaseName = "Database 1";
            isp3.SchemaName = "dbo";
            isp3.TableName = "Table1";
            isp3.IndexName = "Index3";
            isp3.IndexType = "NONCLUSTERED";
            isp3.IndexIsUnique = true;
            isp3.Columns = "SomeColumn,SomeColumn2";
            isp3.IncludedColumns = "SomeColumn4,SomeColumn3";
            isp3.HasFilter = true;
            isp3.FilterDefinition = "(ID < 500)";

            IndexComparer.BusinessObjects.IndexSet iss4 = new IndexComparer.BusinessObjects.IndexSet();
            iss4.ServerName = "Server 1";
            iss4.DatabaseName = "Database 1";
            iss4.SchemaName = "dbo";
            iss4.TableName = "Table1";
            iss4.IndexName = "Index3";
            iss4.IndexType = "NONCLUSTERED";
            iss4.IndexIsUnique = true;
            iss4.Columns = "SomeColumn,SomeColumn2";
            iss4.IncludedColumns = "SomeColumn4,SomeColumn3";
            iss4.HasFilter = true;
            iss4.FilterDefinition = "(ID < 501)";

            Assert.IsTrue(isp3.Equals(iss4));
            Assert.AreEqual(isp3.GetHashCode(), iss4.GetHashCode());
            Assert.IsFalse(isp3.TotallyEquals(iss4));
        }

        #endregion

        #region Indifference Tests

        [TestMethod]
        public void IndexSet_IsTotallyEqual_CanRebuildOnlineDiffers_True()
        {
            IndexComparer.BusinessObjects.IndexSet isp3 = new IndexComparer.BusinessObjects.IndexSet();
            isp3.ServerName = "Server 1";
            isp3.DatabaseName = "Database 1";
            isp3.SchemaName = "dbo";
            isp3.TableName = "Table1";
            isp3.IndexName = "Index3";
            isp3.IndexType = "NONCLUSTERED";
            isp3.IndexIsUnique = true;
            isp3.Columns = "SomeColumn,SomeColumn2";
            isp3.IncludedColumns = "SomeColumn4,SomeColumn3";
            isp3.HasFilter = true;
            isp3.FilterDefinition = "(ID < 500)";
            isp3.CanRebuildOnline = true;

            IndexComparer.BusinessObjects.IndexSet iss4 = new IndexComparer.BusinessObjects.IndexSet();
            iss4.ServerName = "Server 1";
            iss4.DatabaseName = "Database 1";
            iss4.SchemaName = "dbo";
            iss4.TableName = "Table1";
            iss4.IndexName = "Index3";
            iss4.IndexType = "NONCLUSTERED";
            iss4.IndexIsUnique = true;
            iss4.Columns = "SomeColumn,SomeColumn2";
            iss4.IncludedColumns = "SomeColumn4,SomeColumn3";
            iss4.HasFilter = true;
            iss4.FilterDefinition = "(ID < 500)";
            isp3.CanRebuildOnline = false;

            Assert.IsTrue(isp3.Equals(iss4));
            Assert.AreEqual(isp3.GetHashCode(), iss4.GetHashCode());
            Assert.IsTrue(isp3.TotallyEquals(iss4));
        }

        #endregion

        #endregion

        #region Negative Tests

        [TestMethod]
        public void IndexSet_BadInstanceName_Failure()
        {
            try
            {
                IEnumerable<IndexSet> indexes = IndexSet.RetrieveIndexData(BadInstanceName, "master");
            }
            catch (System.Data.SqlClient.SqlException ex)
            {
                Assert.IsTrue(ex.Message.Contains("A network-related or instance-specific error occurred while establishing a connection to SQL Server."),
                    "An unexpected SqlException occurred:  " + ex.Message);
                return;
            }
            catch (Exception ex)
            {
                Assert.Fail("Some unexpected exception occurred:  " + ex.Message);
            }

            Assert.Fail("The test should not have gotten this far--it should have caught an exception.  Make sure that BadInstanceName really doesn't exist.");
        }

        [TestMethod]
        public void IndexSet_BadDatabaseName_Failure()
        {
            try
            {
                IEnumerable<IndexSet> indexes = IndexSet.RetrieveIndexData(InstanceName, BadDatabaseName);
            }
            catch (System.Data.SqlClient.SqlException ex)
            {
                Assert.IsTrue(ex.Message.Contains("Cannot open database \"" + BadDatabaseName + "\" requested by the login. The login failed."), 
                    "An unexpected SqlException occurred:  " + ex.Message);
                return;
            }
            catch (Exception ex)
            {
                Assert.Fail("Some unexpected exception occurred:  " + ex.Message);
            }

            Assert.Fail("The test should not have gotten this far--it should have caught an exception.  Make sure that BadInstanceName really doesn't exist.");
        }       

        #endregion

        #region Data Retrieval

        [TestMethod]
        public void IndexSet_GoodData_Success()
        {
            IEnumerable<IndexSet> indexes = IndexSet.RetrieveIndexData(InstanceName, "master");

            Assert.IsNotNull(indexes);
            Assert.AreNotEqual(0, indexes.Count());
            Assert.AreEqual("master", indexes.ElementAt(0).DatabaseName);
            Assert.AreEqual(InstanceName, indexes.ElementAt(0).ServerName);
        }

        [TestMethod]
        public void IndexSet_GoodData_ConnectionString_Success()
        {
            string ConnectionString = String.Format("server={0};database={1};trusted_connection=yes", InstanceName, DatabaseName);
            IEnumerable<IndexSet> indexes = IndexSet.RetrieveIndexData(InstanceName, DatabaseName, ConnectionString);

            Assert.IsNotNull(indexes);
            Assert.AreNotEqual(0, indexes.Count());
            Assert.AreEqual("master", indexes.ElementAt(0).DatabaseName);
            Assert.AreEqual(InstanceName, indexes.ElementAt(0).ServerName);
        }

        #endregion

        #region Primary and Unique Key Constraints

        //Primary key constraint should result in ALTER TABLE ADD CONSTRAINT
        [TestMethod]
        public void IndexSet_PK_AlterTable_False()
        {
            IndexComparer.BusinessObjects.IndexSet isp = new IndexComparer.BusinessObjects.IndexSet();
            isp.ServerName = "Server 1";
            isp.DatabaseName = "Database 1";
            isp.SchemaName = "dbo";
            isp.TableName = "Table1";
            isp.IndexName = "PK_Table";
            isp.IndexType = "NONCLUSTERED";
            isp.IndexIsUnique = true;
            isp.IsKeyConstraint = false;        //Force a negative test.
            isp.Columns = "SomeColumn,SomeColumn2";
            isp.IncludedColumns = "SomeColumn4,SomeColumn3";
            isp.HasFilter = true;
            isp.FilterDefinition = "(ID < 500)";

            Assert.IsTrue(isp.CreateScript.Contains("CREATE UNIQUE NONCLUSTERED INDEX"));
            Assert.IsFalse(isp.CreateScript.Contains("ADD CONSTRAINT"));
        }

        [TestMethod]
        public void IndexSet_PK_AlterTable_True()
        {
            IndexComparer.BusinessObjects.IndexSet isp = new IndexComparer.BusinessObjects.IndexSet();
            isp.ServerName = "Server 1";
            isp.DatabaseName = "Database 1";
            isp.SchemaName = "dbo";
            isp.TableName = "Table1";
            isp.IndexName = "PK_Table";
            isp.IndexType = "NONCLUSTERED";
            isp.IndexIsUnique = true;
            isp.IsKeyConstraint = true;        //Primary keys are key constraints
            isp.KeyConstraintType = "PK";      //Primary key constraint code
            isp.Columns = "SomeColumn,SomeColumn2";
            isp.IncludedColumns = "SomeColumn4,SomeColumn3";
            isp.HasFilter = true;
            isp.FilterDefinition = "(ID < 500)";

            Assert.IsFalse(isp.CreateScript.Contains("CREATE UNIQUE NONCLUSTERED INDEX"));
            Assert.IsTrue(isp.CreateScript.Contains("ADD CONSTRAINT"));
        }

        //Unique key constraint should result in ALTER TABLE ADD CONSTRAINT
        [TestMethod]
        public void IndexSet_UKC_AlterTable_False()
        {
            IndexComparer.BusinessObjects.IndexSet isp = new IndexComparer.BusinessObjects.IndexSet();
            isp.ServerName = "Server 1";
            isp.DatabaseName = "Database 1";
            isp.SchemaName = "dbo";
            isp.TableName = "Table1";
            isp.IndexName = "UKC_Table";
            isp.IndexType = "NONCLUSTERED";
            isp.IndexIsUnique = true;
            isp.IsKeyConstraint = false;        //Force a negative test.
            isp.Columns = "SomeColumn,SomeColumn2";
            isp.IncludedColumns = "SomeColumn4,SomeColumn3";
            isp.HasFilter = true;
            isp.FilterDefinition = "(ID < 500)";

            Assert.IsTrue(isp.CreateScript.Contains("CREATE UNIQUE NONCLUSTERED INDEX"));
            Assert.IsFalse(isp.CreateScript.Contains("ADD CONSTRAINT"));
        }

        [TestMethod]
        public void IndexSet_UKC_AlterTable_True()
        {
            IndexComparer.BusinessObjects.IndexSet isp = new IndexComparer.BusinessObjects.IndexSet();
            isp.ServerName = "Server 1";
            isp.DatabaseName = "Database 1";
            isp.SchemaName = "dbo";
            isp.TableName = "Table1";
            isp.IndexName = "UKC_Table";
            isp.IndexType = "NONCLUSTERED";
            isp.IndexIsUnique = true;
            isp.IsKeyConstraint = true;        //Unique keys are key constraints
            isp.KeyConstraintType = "UQ";      //Unique key constraint code
            isp.Columns = "SomeColumn,SomeColumn2";
            isp.IncludedColumns = "SomeColumn4,SomeColumn3";
            isp.HasFilter = true;
            isp.FilterDefinition = "(ID < 500)";

            Assert.IsFalse(isp.CreateScript.Contains("CREATE UNIQUE NONCLUSTERED INDEX"));
            Assert.IsTrue(isp.CreateScript.Contains("ADD CONSTRAINT"));
        }

        //Index comparison between PK and equivalent NCI should DIFFER
        [TestMethod]
        public void IndexSet_PK_NCI_IsEqual_False()
        {
            IndexComparer.BusinessObjects.IndexSet isp3 = new IndexComparer.BusinessObjects.IndexSet();
            isp3.ServerName = "Server 1";
            isp3.DatabaseName = "Database 1";
            isp3.SchemaName = "dbo";
            isp3.TableName = "Table1";
            isp3.IndexName = "Index3";
            isp3.IndexType = "NONCLUSTERED";
            isp3.IndexIsUnique = true;
            isp3.IsKeyConstraint = true;
            isp3.KeyConstraintType = "PK";
            isp3.Columns = "SomeColumn,SomeColumn2";
            isp3.IncludedColumns = "SomeColumn4,SomeColumn3";
            isp3.HasFilter = true;
            isp3.FilterDefinition = "(ID < 500)";

            IndexComparer.BusinessObjects.IndexSet iss4 = new IndexComparer.BusinessObjects.IndexSet();
            iss4.ServerName = "Server 1";
            iss4.DatabaseName = "Database 1";
            iss4.SchemaName = "dbo";
            iss4.TableName = "Table1";
            iss4.IndexName = "Index3";
            iss4.IndexType = "NONCLUSTERED";
            iss4.IndexIsUnique = true;
            iss4.Columns = "SomeColumn,SomeColumn2";
            iss4.IncludedColumns = "SomeColumn4,SomeColumn3";
            iss4.HasFilter = true;
            iss4.FilterDefinition = "(ID < 500)";

            Assert.IsTrue(isp3.Equals(iss4));       //Server, database, schema, table, and index name match.
            Assert.AreEqual(isp3.GetHashCode(), iss4.GetHashCode());
            Assert.IsFalse(isp3.TotallyEquals(iss4));   //Key constraint does NOT match.
        }

        //Index comparison between UKC and equivalent NCI should DIFFER
        [TestMethod]
        public void IndexSet_UQ_NCI_IsEqual_False()
        {
            IndexComparer.BusinessObjects.IndexSet isp3 = new IndexComparer.BusinessObjects.IndexSet();
            isp3.ServerName = "Server 1";
            isp3.DatabaseName = "Database 1";
            isp3.SchemaName = "dbo";
            isp3.TableName = "Table1";
            isp3.IndexName = "Index3";
            isp3.IndexType = "NONCLUSTERED";
            isp3.IndexIsUnique = true;
            isp3.IsKeyConstraint = true;
            isp3.KeyConstraintType = "UQ";
            isp3.Columns = "SomeColumn,SomeColumn2";
            isp3.IncludedColumns = "SomeColumn4,SomeColumn3";
            isp3.HasFilter = true;
            isp3.FilterDefinition = "(ID < 500)";

            IndexComparer.BusinessObjects.IndexSet iss4 = new IndexComparer.BusinessObjects.IndexSet();
            iss4.ServerName = "Server 1";
            iss4.DatabaseName = "Database 1";
            iss4.SchemaName = "dbo";
            iss4.TableName = "Table1";
            iss4.IndexName = "Index3";
            iss4.IndexType = "NONCLUSTERED";
            iss4.IndexIsUnique = true;
            iss4.Columns = "SomeColumn,SomeColumn2";
            iss4.IncludedColumns = "SomeColumn4,SomeColumn3";
            iss4.HasFilter = true;
            iss4.FilterDefinition = "(ID < 500)";

            Assert.IsTrue(isp3.Equals(iss4));       //Server, database, schema, table, and index name match.
            Assert.AreEqual(isp3.GetHashCode(), iss4.GetHashCode());
            Assert.IsFalse(isp3.TotallyEquals(iss4));   //Key constraint does NOT match.
        }

        //Index comparison between PK and equivalent CI should DIFFER
        [TestMethod]
        public void IndexSet_PK_CI_IsEqual_False()
        {
            IndexComparer.BusinessObjects.IndexSet isp3 = new IndexComparer.BusinessObjects.IndexSet();
            isp3.ServerName = "Server 1";
            isp3.DatabaseName = "Database 1";
            isp3.SchemaName = "dbo";
            isp3.TableName = "Table1";
            isp3.IndexName = "Index3";
            isp3.IndexType = "CLUSTERED";
            isp3.IndexIsUnique = true;
            isp3.IsKeyConstraint = true;
            isp3.KeyConstraintType = "PK";
            isp3.Columns = "SomeColumn,SomeColumn2";
            isp3.IncludedColumns = "SomeColumn4,SomeColumn3";
            isp3.HasFilter = true;
            isp3.FilterDefinition = "(ID < 500)";

            IndexComparer.BusinessObjects.IndexSet iss4 = new IndexComparer.BusinessObjects.IndexSet();
            iss4.ServerName = "Server 1";
            iss4.DatabaseName = "Database 1";
            iss4.SchemaName = "dbo";
            iss4.TableName = "Table1";
            iss4.IndexName = "Index3";
            iss4.IndexType = "CLUSTERED";
            iss4.IndexIsUnique = true;
            iss4.Columns = "SomeColumn,SomeColumn2";
            iss4.IncludedColumns = "SomeColumn4,SomeColumn3";
            iss4.HasFilter = true;
            iss4.FilterDefinition = "(ID < 500)";

            Assert.IsTrue(isp3.Equals(iss4));       //Server, database, schema, table, and index name match.
            Assert.AreEqual(isp3.GetHashCode(), iss4.GetHashCode());
            Assert.IsFalse(isp3.TotallyEquals(iss4));   //Key constraint does NOT match.
        }

        //Index comparison between UKC and equivalent CI should DIFFER
        [TestMethod]
        public void IndexSet_UQ_CI_IsEqual_False()
        {
            IndexComparer.BusinessObjects.IndexSet isp3 = new IndexComparer.BusinessObjects.IndexSet();
            isp3.ServerName = "Server 1";
            isp3.DatabaseName = "Database 1";
            isp3.SchemaName = "dbo";
            isp3.TableName = "Table1";
            isp3.IndexName = "Index3";
            isp3.IndexType = "CLUSTERED";
            isp3.IndexIsUnique = true;
            isp3.IsKeyConstraint = true;
            isp3.KeyConstraintType = "PK";
            isp3.Columns = "SomeColumn,SomeColumn2";
            isp3.IncludedColumns = "SomeColumn4,SomeColumn3";
            isp3.HasFilter = true;
            isp3.FilterDefinition = "(ID < 500)";

            IndexComparer.BusinessObjects.IndexSet iss4 = new IndexComparer.BusinessObjects.IndexSet();
            iss4.ServerName = "Server 1";
            iss4.DatabaseName = "Database 1";
            iss4.SchemaName = "dbo";
            iss4.TableName = "Table1";
            iss4.IndexName = "Index3";
            iss4.IndexType = "CLUSTERED";
            iss4.IndexIsUnique = true;
            iss4.Columns = "SomeColumn,SomeColumn2";
            iss4.IncludedColumns = "SomeColumn4,SomeColumn3";
            iss4.HasFilter = true;
            iss4.FilterDefinition = "(ID < 500)";

            Assert.IsTrue(isp3.Equals(iss4));       //Server, database, schema, table, and index name match.
            Assert.AreEqual(isp3.GetHashCode(), iss4.GetHashCode());
            Assert.IsFalse(isp3.TotallyEquals(iss4));   //Key constraint does NOT match.
        }

        #endregion
    }
}
