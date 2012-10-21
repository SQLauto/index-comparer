using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;

namespace IndexComparer.BusinessObjects
{
    public class IndexGroup: IEquatable<IndexGroup>
    {
        #region Properties

        public string SchemaName { get; set; }
        public string TableName { get; set; }
        public string IndexName { get; set; }
        public IndexSet PrimaryIndexSet { get; set; }
        public IndexSet SecondaryIndexSet { get; set; }

        #endregion

        #region Additional Properties

        public string SchemaAndTableName
        {
            get
            {
                return String.Format("{0}.{1}", SchemaName, TableName);
            }
        }

        public string FriendlyIndexName
        {
            get
            {
                return String.IsNullOrWhiteSpace(IndexName) ? "HEAP" : IndexName;
            }
        }

        public string DropScript
        {
            get
            {
                if (IndexExistsOnPrimary)
                    return PrimaryIndexSet.DropScript;
                else
                    return SecondaryIndexSet.DropScript;
            }
        }

        #region Comparison Properties

        public bool ComparisonDiffers
        {
            get
            {
                if (IndexExistsOnPrimary && IndexExistsOnSecondary)
                    return !PrimaryIndexSet.TotallyEquals(SecondaryIndexSet);
                else
                    return false;
            }
        }

        public bool ComparisonDiffersOrNull
        {
            get
            {
                if (IndexExistsOnPrimary && IndexExistsOnSecondary)
                    return !PrimaryIndexSet.TotallyEquals(SecondaryIndexSet);
                else
                    return true;
            }
        }

        public string ComparisonUnique
        {
            get
            {
                if (!IndexExistsOnPrimary)
                    return SecondaryIndexIsUnique ? "UNIQUE" : "NOT";
                if (!IndexExistsOnSecondary)
                    return PrimaryIndexIsUnique ? "UNIQUE" : "NOT";

                if (PrimaryIndexIsUnique && SecondaryIndexIsUnique)
                    return "UNIQUE";
                else if (PrimaryIndexIsUnique)
                    return "PU / SN";
                else if (SecondaryIndexIsUnique)
                    return "PN / SU";
                else
                    return "NOT";
            }
        }

        public string ComparisonType
        {
            get
            {
                if (!IndexExistsOnPrimary)
                    return SecondaryIndexType;
                if (!IndexExistsOnSecondary)
                    return PrimaryIndexType;

                if (PrimaryIndexType == SecondaryIndexType)
                    return PrimaryIndexType;
                else
                    return String.Format("P{0} / S{1}", PrimaryIndexTypeCode, SecondaryIndexTypeCode);
            }
        }

        public string ComparisonTypeCode
        {
            get
            {
                if (!IndexExistsOnPrimary)
                    return SecondaryIndexTypeCode;
                if (!IndexExistsOnSecondary)
                    return PrimaryIndexTypeCode;

                if (PrimaryIndexTypeCode == SecondaryIndexTypeCode)
                    return PrimaryIndexTypeCode;
                else
                    return String.Format("{0} / {1}", PrimaryIndexTypeCode, SecondaryIndexTypeCode);
            }
        }

        public string ComparisonColumns
        {
            get
            {
                if (!IndexExistsOnPrimary)
                    return SecondaryIndexColumns;
                if (!IndexExistsOnSecondary)
                    return PrimaryIndexColumns;

                if (PrimaryIndexColumns == SecondaryIndexColumns)
                    return PrimaryIndexColumns;
                else
                    return String.Format("{0} / {1}", PrimaryIndexColumns, SecondaryIndexColumns);
            }
        }

        public string ComparisonIncludedColumns
        {
            get
            {
                if (!IndexExistsOnPrimary)
                    return SecondaryIndexIncludedColumns;
                if (!IndexExistsOnSecondary)
                    return PrimaryIndexIncludedColumns;

                if (PrimaryIndexIncludedColumns == SecondaryIndexIncludedColumns)
                    return PrimaryIndexIncludedColumns;
                else
                    return String.Format("{0} / {1}", PrimaryIndexIncludedColumns, SecondaryIndexIncludedColumns);
            }
        }

        public string ComparisonFilter
        {
            get
            {
                if (!IndexExistsOnPrimary)
                    return SecondaryIndexFilter;
                if (!IndexExistsOnSecondary)
                    return PrimaryIndexFilter;

                if (PrimaryIndexFilter == SecondaryIndexFilter)
                    return PrimaryIndexFilter;
                else
                    return String.Format("{0} / {1}", PrimaryIndexFilter, SecondaryIndexFilter);
            }
        }

        #endregion

        #region Primary Index Helper Properties

        public bool IndexExistsOnPrimary
        {
            get
            {
                return PrimaryIndexSet != null;
            }
        }
        public bool PrimaryIndexIsUnique 
        {
            get
            {
                return IndexExistsOnPrimary ? PrimaryIndexSet.IndexIsUnique : false;
            }
        }
        public string PrimaryIndexType 
        {
            get
            {
                return IndexExistsOnPrimary ? PrimaryIndexSet.IndexType : String.Empty;
            }
        }
        public string PrimaryIndexTypeCode
        {
            get
            {
                return IndexExistsOnPrimary ? PrimaryIndexSet.IndexTypeCode : String.Empty;
            }
        }
        public string PrimaryIndexColumns 
        {
            get
            {
                return IndexExistsOnPrimary ? PrimaryIndexSet.Columns : String.Empty;
            }
        }
        public string PrimaryIndexIncludedColumns 
        {
            get
            {
                return IndexExistsOnPrimary ? PrimaryIndexSet.IncludedColumns : String.Empty;
            }
        }
        public bool PrimaryIndexHasFilter 
        {
            get
            {
                return IndexExistsOnPrimary ? PrimaryIndexSet.HasFilter : false;
            }
        }
        public string PrimaryIndexFilter 
        {
            get
            {
                return IndexExistsOnPrimary ? PrimaryIndexSet.FilterDefinition : String.Empty;
            }
        }
        public string PrimaryIndexCreateScript
        {
            get
            {
                return IndexExistsOnPrimary ? PrimaryIndexSet.CreateScript : String.Empty;
            }
        }
        public bool PrimaryIndexCanRebuildOnline
        {
            get
            {
                return IndexExistsOnPrimary ? PrimaryIndexSet.CanRebuildOnline : false;
            }
        }

        #endregion

        #region Secondary Index Helper Properties

        public bool IndexExistsOnSecondary
        {
            get
            {
                return SecondaryIndexSet != null;
            }
        }
        public bool SecondaryIndexIsUnique
        {
            get
            {
                return IndexExistsOnSecondary ? SecondaryIndexSet.IndexIsUnique : false;
            }
        }
        public string SecondaryIndexType
        {
            get
            {
                return IndexExistsOnSecondary ? SecondaryIndexSet.IndexType : String.Empty;
            }
        }
        public string SecondaryIndexTypeCode
        {
            get
            {
                return IndexExistsOnSecondary ? SecondaryIndexSet.IndexTypeCode : String.Empty;
            }
        }
        public string SecondaryIndexColumns
        {
            get
            {
                return IndexExistsOnSecondary ? SecondaryIndexSet.Columns : String.Empty;
            }
        }
        public string SecondaryIndexIncludedColumns
        {
            get
            {
                return IndexExistsOnSecondary ? SecondaryIndexSet.IncludedColumns : String.Empty;
            }
        }
        public bool SecondaryIndexHasFilter
        {
            get
            {
                return IndexExistsOnSecondary ? SecondaryIndexSet.HasFilter : false;
            }
        }
        public string SecondaryIndexFilter
        {
            get
            {
                return IndexExistsOnSecondary ? SecondaryIndexSet.FilterDefinition : String.Empty;
            }
        }
        public string SecondaryIndexCreateScript
        {
            get
            {
                return IndexExistsOnSecondary ? SecondaryIndexSet.CreateScript : String.Empty;
            }
        }
        public bool SecondaryIndexCanRebuildOnline
        {
            get
            {
                return IndexExistsOnSecondary ? SecondaryIndexSet.CanRebuildOnline : false;
            }
        }

        #endregion

        #endregion

        #region Constructors

        public IndexGroup(IndexSet Primary, IndexSet Secondary)
        {
            if (Primary == null && Secondary == null)
                throw new ApplicationException("At least one index set must be non-null.");

            PrimaryIndexSet = Primary;
            SecondaryIndexSet = Secondary;

            if (PrimaryIndexSet != null)
            {
                SchemaName = PrimaryIndexSet.SchemaName;
                TableName = PrimaryIndexSet.TableName;
                IndexName = PrimaryIndexSet.IndexName;
            }
            else
            {
                SchemaName = SecondaryIndexSet.SchemaName;
                TableName = SecondaryIndexSet.TableName;
                IndexName = SecondaryIndexSet.IndexName;
            }
        }

        #endregion

        #region IEquatable Implementation

        public bool Equals(IndexGroup other)
        {
            if (Object.ReferenceEquals(other, null)) return false;
            if (Object.ReferenceEquals(this, other)) return true;

            return SchemaName.Equals(other.SchemaName) && TableName.Equals(other.TableName) &&
                IndexName.Equals(other.IndexName);
        }

        public override int GetHashCode()
        {
            using (MD5 md5 = new MD5CryptoServiceProvider())
            {
                return BitConverter.ToString(md5.ComputeHash(ASCIIEncoding.Default.GetBytes(SchemaName + TableName + IndexName))).GetHashCode();
            }
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// This method returns the set of indexes which have a difference
        /// </summary>
        /// <param name="Base">The "base" index set group.</param>
        /// <param name="Comp">The indexes you would like to compare to the base indexes.</param>
        /// <returns>A set of IndexDifference objects containing the indexes which have the same name but different definitions.  
        /// If there are no results, this returns an empty IEnumerable.</returns>
        public static IEnumerable<IndexGroup> PopulateIndexGroups(IEnumerable<IndexSet> Base, IEnumerable<IndexSet> Comp)
        {
            var query = (
                            from b in Base
                            join c in Comp on b.GetHashCode() equals c.GetHashCode() into tempComp
                            from newc in tempComp.DefaultIfEmpty()
                            select new IndexGroup(b, newc)
                        ).Union
                        (
                            from c2 in Comp
                            join b2 in Base on c2.GetHashCode() equals b2.GetHashCode() into tempBase
                            from newb in tempBase.DefaultIfEmpty()
                            select new IndexGroup(newb, c2)
                        );

            return query;
        }

        #endregion
    }
}
