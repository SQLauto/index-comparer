using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IndexComparer.BusinessObjects
{
    public class IndexDifference
    {
        #region Properties

        public string ServerName { get; set; }
        public string DatabaseName { get; set; }
        public string SchemaName { get; set; }
        public string TableName { get; set; }
        public string IndexName { get; set; }
        public string IndexType { get; set; }
        public bool IndexIsUnique { get; set; }
        public string Columns { get; set; }
        public string IncludedColumns { get; set; }
        public bool HasFilter { get; set; }
        public string FilterDefinition { get; set; }
        public bool CanRebuildOnline { get; set; }
        public string OtherIndexType { get; set; }
        public bool OtherIndexIsUnique { get; set; }
        public string OtherColumns { get; set; }
        public string OtherIncludedColumns { get; set; }
        public bool OtherHasFilter { get; set; }
        public string OtherFilterDefinition { get; set; }
        public bool OtherCanRebuildOnline { get; set; }
        public string DropScript { get; set; }
        public string PrimaryIndexCreateScript { get; set; }
        public string SecondaryIndexCreateScript { get; set; }

        #endregion Properties

        #region Additional Properties

        public bool IsHeap
        {
            get
            {
                return IndexType.Trim() == "HEAP";
            }
        }

        #endregion

        #region Constructors

        public IndexDifference(string serverName, string databaseName, string schemaName, string tableName, string indexName,
            string indexType, bool indexIsUnique, string columns, string includedColumns, bool hasFilter, string filterDefinition, bool canRebuildOnline,
            string otherIndexType, bool otherIndexIsUnique, string otherColumns, string otherIncludedColumns, bool otherHasFilter, 
            string otherFilterDefinition, bool otherCanRebuildOnline, string dropScript, string primaryIndexCreateScript, string secondaryIndexCreateScript)
        {
            this.ServerName = serverName;
            this.DatabaseName = databaseName;
            this.SchemaName = schemaName;
            this.TableName = tableName;
            this.IndexName = indexName;
            this.IndexType = indexType;
            this.Columns = columns;
            this.IncludedColumns = includedColumns;
            this.CanRebuildOnline = canRebuildOnline;
            this.OtherIndexType = otherIndexType;
            this.OtherIndexIsUnique = otherIndexIsUnique;
            this.OtherColumns = otherColumns;
            this.OtherIncludedColumns = otherIncludedColumns;
            this.OtherHasFilter = otherHasFilter;
            this.OtherFilterDefinition = otherFilterDefinition;
            this.OtherCanRebuildOnline = otherCanRebuildOnline;
            this.DropScript = dropScript;
            this.PrimaryIndexCreateScript = primaryIndexCreateScript;
            this.SecondaryIndexCreateScript = secondaryIndexCreateScript;
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// This method returns the set of indexes which have a difference
        /// </summary>
        /// <param name="Base">The "base" index set.</param>
        /// <param name="Comp">The indexes you would like to compare to the base indexes.</param>
        /// <returns>A set of IndexDifference objects containing the indexes which have the same name but different definitions.  
        /// If there are no results, this returns an empty IEnumerable.</returns>
        public static IEnumerable<IndexDifference> GetDifferences(IEnumerable<IndexSet> Base, IEnumerable<IndexSet> Comp)
        {
            var query = from b in Base
                        join c in Comp on b.GetHashCode() equals c.GetHashCode()
                        where !b.TotallyEquals(c)
                        select new IndexDifference(b.ServerName, b.DatabaseName, b.SchemaName, b.TableName,
                            b.IndexName, b.IndexType, b.IndexIsUnique, b.Columns, b.IncludedColumns, b.HasFilter, b.FilterDefinition, b.CanRebuildOnline,
                            c.IndexType, c.IndexIsUnique, c.Columns, c.IncludedColumns, c.HasFilter, c.FilterDefinition, c.CanRebuildOnline,
                            b.DropScript, b.CreateScript, c.CreateScript);

            return query;
        }

        #endregion
    }
}
