using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using IndexComparer.BusinessObjects;

namespace IndexComparer.UnitTests
{
    [TestClass]
    public class IndexGroupTests
    {
        #region Variable Initialization

        private List<IndexSet> PrimaryIndexes;
        private List<IndexSet> SecondaryIndexes;
        private IndexSet isp1;
        private IndexSet iss1;
        private IndexSet isp2;
        private IndexSet iss2;
        private IndexSet isp3;
        private IndexSet iss3;
        private IndexSet isp4;
        private IndexSet iss4;

        [TestInitialize()]
        public void GenerateTestData()
        {
            //Quick description of indexes:
            //isp1 = iss1
            //isp1 is for the Primary list and iss1 for the Secondary list
            //isp2 is unique
            //iss2 is unique
            //isp3 ~= iss3
            //The primary columns match, but there are secondary column differences
            //isp4 ~= iss4
            //The primary columns match, but there are secondary column differences

            PrimaryIndexes = new List<BusinessObjects.IndexSet>();
            SecondaryIndexes = new List<BusinessObjects.IndexSet>();

            isp1 = new IndexSet();
            isp1.ServerName = "Server 1";
            isp1.DatabaseName = "Database 1";
            isp1.SchemaName = "dbo";
            isp1.TableName = "Table1";
            isp1.IndexName = "Index1";
            isp1.IndexType = "CLUSTERED";
            isp1.IndexIsUnique = true;
            isp1.Columns = "ID";
            isp1.IncludedColumns = String.Empty;
            isp1.HasFilter = false;
            isp1.FilterDefinition = String.Empty;

            isp2 = new IndexSet();
            isp2.ServerName = "Server 1";
            isp2.DatabaseName = "Database 1";
            isp2.SchemaName = "dbo";
            isp2.TableName = "Table1";
            isp2.IndexName = "Index6";
            isp2.IndexType = "CLUSTERED";
            isp2.IndexIsUnique = true;
            isp2.Columns = "ID";
            isp2.IncludedColumns = String.Empty;
            isp2.HasFilter = false;
            isp2.FilterDefinition = String.Empty;

            isp3 = new IndexSet();
            isp3.ServerName = "Server 1";
            isp3.DatabaseName = "Database 1";
            isp3.SchemaName = "dbo";
            isp3.TableName = "Table2";
            isp3.IndexName = "Index6";
            isp3.IndexType = "NONCLUSTERED";
            isp3.IndexIsUnique = true;
            isp3.Columns = "ID,Column6";
            isp3.IncludedColumns = "SomeColumn";
            isp3.HasFilter = false;
            isp3.FilterDefinition = "(ID < 50)";

            isp4 = new IndexSet();
            isp4.ServerName = "Server 1";
            isp4.DatabaseName = "Database 1";
            isp4.SchemaName = "dbo";
            isp4.TableName = "Table2";
            isp4.IndexName = "IndexX";
            isp4.IndexType = "CLUSTERED";
            isp4.IndexIsUnique = true;
            isp4.Columns = "ID";
            isp4.IncludedColumns = String.Empty;
            isp4.HasFilter = false;
            isp4.FilterDefinition = String.Empty;

            iss1 = new IndexSet();
            iss1.ServerName = "Server 1";
            iss1.DatabaseName = "Database 1";
            iss1.SchemaName = "dbo";
            iss1.TableName = "Table1";
            iss1.IndexName = "Index1";
            iss1.IndexType = "CLUSTERED";
            iss1.IndexIsUnique = true;
            iss1.Columns = "ID";
            iss1.IncludedColumns = String.Empty;
            iss1.HasFilter = false;
            iss1.FilterDefinition = String.Empty;

            iss2 = new IndexSet();
            iss2.ServerName = "Server 1";
            iss2.DatabaseName = "Database 1";
            iss2.SchemaName = "dbo";
            iss2.TableName = "Table1";
            iss2.IndexName = "Index51";
            iss2.IndexType = "NONCLUSTERED";
            iss2.IndexIsUnique = true;
            iss2.Columns = "ID";
            iss2.IncludedColumns = String.Empty;
            iss2.HasFilter = false;
            iss2.FilterDefinition = String.Empty;

            iss3 = new IndexSet();
            iss3.ServerName = "Server 1";
            iss3.DatabaseName = "Database 1";
            iss3.SchemaName = "dbo";
            iss3.TableName = "Table2";
            iss3.IndexName = "Index6";
            iss3.IndexType = "NONCLUSTERED";
            iss3.IndexIsUnique = true;
            iss3.Columns = "ID,Column4";
            iss3.IncludedColumns = "SomeColumn2";
            iss3.HasFilter = false;
            iss3.FilterDefinition = "(ID < 50)";

            iss4 = new IndexSet();
            iss4.ServerName = "Server 1";
            iss4.DatabaseName = "Database 1";
            iss4.SchemaName = "dbo";
            iss4.TableName = "Table2";
            iss4.IndexName = "IndexX";
            iss4.IndexType = "HEAP";
            iss4.IndexIsUnique = true;
            iss4.Columns = "ID";
            iss4.IncludedColumns = String.Empty;
            iss4.HasFilter = false;
            iss4.FilterDefinition = String.Empty;
        }

        #endregion

        [TestMethod]
        public void IndexGroup_MatchingSet_Success()
        {
            PrimaryIndexes.Add(isp1);
            SecondaryIndexes.Add(iss1);

            IEnumerable<IndexGroup> Group = IndexGroup.PopulateIndexGroups(PrimaryIndexes, SecondaryIndexes);

            Assert.IsNotNull(Group);
            Assert.AreEqual(1, Group.Count());
            Assert.AreEqual(0, Group.Where(x => x.ComparisonDiffers).Count());
        }

        [TestMethod]
        public void IndexGroup_EmptyFirstSet_Success()
        {
            SecondaryIndexes.Add(iss1);

            IEnumerable<IndexGroup> Group = IndexGroup.PopulateIndexGroups(PrimaryIndexes, SecondaryIndexes);

            Assert.IsNotNull(Group);
            Assert.AreEqual(1, Group.Count());
            Assert.AreEqual(0, Group.Where(x => x.ComparisonDiffers).Count());
        }

        [TestMethod]
        public void IndexGroup_EmptySecondSet_Success()
        {
            PrimaryIndexes.Add(isp1);

            IEnumerable<IndexGroup> Group = IndexGroup.PopulateIndexGroups(PrimaryIndexes, SecondaryIndexes);

            Assert.IsNotNull(Group);
            Assert.AreEqual(1, Group.Count());
            Assert.AreEqual(0, Group.Where(x => x.ComparisonDiffers).Count());
        }

        [TestMethod]
        public void IndexGroup_ExtraInFirstSet_MatchingGroup_Success()
        {
            PrimaryIndexes.Add(isp1);
            PrimaryIndexes.Add(isp2);
            SecondaryIndexes.Add(iss1);

            IEnumerable<IndexGroup> Group = IndexGroup.PopulateIndexGroups(PrimaryIndexes, SecondaryIndexes);

            Assert.IsNotNull(Group);
            Assert.AreEqual(2, Group.Count());
            Assert.AreEqual(0, Group.Where(x => x.ComparisonDiffers).Count());
        }

        [TestMethod]
        public void IndexGroup_ExtraInSecondSet_MatchingGroup_Success()
        {
            PrimaryIndexes.Add(isp1);
            SecondaryIndexes.Add(iss1);
            SecondaryIndexes.Add(iss2);

            IEnumerable<IndexGroup> Group = IndexGroup.PopulateIndexGroups(PrimaryIndexes, SecondaryIndexes);

            Assert.IsNotNull(Group);
            Assert.AreEqual(2, Group.Count());
            Assert.AreEqual(0, Group.Where(x => x.ComparisonDiffers).Count());
        }

        [TestMethod]
        public void IndexGroup_TwoMatches_OneDifferent_Success()
        {
            PrimaryIndexes.Add(isp1);
            PrimaryIndexes.Add(isp3);
            SecondaryIndexes.Add(iss1);
            SecondaryIndexes.Add(iss2);
            SecondaryIndexes.Add(iss3);

            IEnumerable<IndexGroup> Group = IndexGroup.PopulateIndexGroups(PrimaryIndexes, SecondaryIndexes);

            Assert.IsNotNull(Group);
            Assert.AreEqual(3, Group.Count());
            Assert.AreEqual(1, Group.Where(x => x.ComparisonDiffers).Count());
        }

        [TestMethod]
        public void IndexGroup_ThreeMatches_TwoDifferent_Success()
        {
            PrimaryIndexes.Add(isp1);
            PrimaryIndexes.Add(isp2);
            PrimaryIndexes.Add(isp3);
            PrimaryIndexes.Add(isp4);
            SecondaryIndexes.Add(iss1);
            SecondaryIndexes.Add(iss2);
            SecondaryIndexes.Add(iss3);
            SecondaryIndexes.Add(iss4);

            IEnumerable<IndexGroup> Group = IndexGroup.PopulateIndexGroups(PrimaryIndexes, SecondaryIndexes);

            Assert.IsNotNull(Group);
            Assert.AreEqual(5, Group.Count());      //1=1, p2, s2, 3~=3, 4~=4
            Assert.AreEqual(2, Group.Where(x => x.ComparisonDiffers).Count());  //3 & 4 differ
        }
    }
}
