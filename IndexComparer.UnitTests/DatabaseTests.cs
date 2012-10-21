using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using IndexComparer.BusinessObjects;

namespace IndexComparer.UnitTests
{
    [TestClass]
    public class DatabaseTests
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
        //This should be an instance which does NOT exist on your network.
        private string BadInstanceName = "badinstancename";


        #region Negative Tests

        [TestMethod]
        public void Database_BadInstanceName_Failure()
        {
            try
            {
                IEnumerable<Database> databases = Database.GetByServerName(BadInstanceName);
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

        #endregion

        #region Getting Lists

        [TestMethod]
        public void Database_CanGetList_Success()
        {
            IEnumerable<Database> databases = Database.GetByServerName(InstanceName);

            Assert.IsNotNull(databases);
            Assert.AreNotEqual(0, databases.Count());
            Assert.AreEqual(1, databases.Where(x => x.Name.ToLower() == "master").Count());
        }

        [TestMethod]
        public void Database_CanGetList_WithConnectionString_Success()
        {
            string ConnectionString = String.Format("server={0};database=tempdb;trusted_connection=yes", InstanceName);
            IEnumerable<Database> databases = Database.GetByServerName(InstanceName, ConnectionString);

            Assert.IsNotNull(databases);
            Assert.AreNotEqual(0, databases.Count());
            Assert.AreEqual(1, databases.Where(x => x.Name.ToLower() == "master").Count());
        }

        [TestMethod]
        public void Database_ResultsInAlphabeticalOrder_Success()
        {
            IEnumerable<Database> databases = Database.GetByServerName(InstanceName);

            for (int i = 0; i < databases.Count() - 1; i++)
            {
                string s1 = databases.ElementAt(i).Name;
                string s2 = databases.ElementAt(i + 1).Name;
                Assert.IsTrue(s1.CompareTo(s2) < 0, String.Format("{0} doesn't come before {1}", s1, s2));
            }

        }

        #endregion
    }
}
