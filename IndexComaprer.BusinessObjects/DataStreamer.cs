using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IndexComparer.BusinessObjects
{
    public class DataStreamer
    {
        /// <summary>
        /// Stream out a difference file.
        /// </summary>
        /// <param name="DisplayHeader">Whether you want to display an ASCII header for each section.</param>
        /// <param name="ScriptDrops">Whether you want to script out the drop statement for each index.</param>
        /// <param name="ScriptCreates">Whether you want to script out the create statement for each index.</param>
        /// <param name="writer">The stream to which you wish to write this output.</param>
        /// <param name="PrimaryServerName"></param>
        /// <param name="PrimaryDatabaseName"></param>
        /// <param name="SecondaryServerName"></param>
        /// <param name="SecondaryDatabaseName"></param>
        /// <param name="PrimaryResults"></param>
        /// <param name="SecondaryResults"></param>
        public static void StreamFile(bool DisplayHeader, bool ScriptDrops, bool ScriptCreates, System.IO.StreamWriter writer,
            string PrimaryServerName, string PrimaryDatabaseName, string SecondaryServerName, string SecondaryDatabaseName,
            IEnumerable<IndexSet> PrimaryResults, IEnumerable<IndexSet> SecondaryResults)
        {
            StreamFileHeader(writer, PrimaryServerName, PrimaryDatabaseName, SecondaryServerName, SecondaryDatabaseName);
            StreamTopLevelComparison(DisplayHeader, writer, PrimaryServerName, PrimaryDatabaseName, SecondaryServerName, SecondaryDatabaseName, PrimaryResults, SecondaryResults);
            StreamNotInSecondaryComparison(DisplayHeader, ScriptDrops, ScriptCreates, writer, PrimaryServerName, PrimaryDatabaseName, SecondaryServerName, SecondaryDatabaseName, PrimaryResults, SecondaryResults);
            StreamNotInPrimaryComparison(DisplayHeader, ScriptDrops, ScriptCreates, writer, PrimaryServerName, PrimaryDatabaseName, SecondaryServerName, SecondaryDatabaseName, PrimaryResults, SecondaryResults);
            StreamDifferences(DisplayHeader, ScriptDrops, ScriptCreates, writer, PrimaryServerName, PrimaryDatabaseName, SecondaryServerName, SecondaryDatabaseName, PrimaryResults, SecondaryResults);
        }

        /// <summary>
        /// Stream the file header.  This writes out the servers and databases you are comparing, and the time this process was run.
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="PrimaryServerName"></param>
        /// <param name="PrimaryDatabaseName"></param>
        /// <param name="SecondaryServerName"></param>
        /// <param name="SecondaryDatabaseName"></param>
        public static void StreamFileHeader(System.IO.StreamWriter writer,
            string PrimaryServerName, string PrimaryDatabaseName, string SecondaryServerName, string SecondaryDatabaseName)
        {
            if (writer == null)
                throw new Exception("No stream available.");

            writer.WriteLine(String.Format("INDEX COMPARISON FOR {0}.{1} VERSUS {2}.{3}", PrimaryServerName, PrimaryDatabaseName,
                    SecondaryServerName, SecondaryDatabaseName));
            writer.WriteLine(String.Format("Comparison performed {0}", DateTime.Now));
            writer.WriteLine(String.Empty);

            writer.Flush();
        }
        
        /// <summary>
        /// Stream the top-level comparison.  This includes the index counts for each database.
        /// </summary>
        /// <param name="DisplayHeader"></param>
        /// <param name="writer"></param>
        /// <param name="PrimaryServerName"></param>
        /// <param name="PrimaryDatabaseName"></param>
        /// <param name="SecondaryServerName"></param>
        /// <param name="SecondaryDatabaseName"></param>
        /// <param name="PrimaryResults"></param>
        /// <param name="SecondaryResults"></param>
        public static void StreamTopLevelComparison(bool DisplayHeader, System.IO.StreamWriter writer,
            string PrimaryServerName, string PrimaryDatabaseName, string SecondaryServerName, string SecondaryDatabaseName,
            IEnumerable<IndexSet> PrimaryResults, IEnumerable<IndexSet> SecondaryResults)
        {
            if (writer == null)
                throw new Exception("No stream available.");

            if (DisplayHeader)
            {
                writer.WriteLine("=================================================================");
                writer.WriteLine("================SECTION 1:  TOP-LEVEL COMPARISON=================");
                writer.WriteLine("=================================================================");
            }

            writer.WriteLine(String.Format("{0}.{1} has {4} indexes; {2}.{3} has {5} indexes.", PrimaryServerName, PrimaryDatabaseName,
                        SecondaryServerName, SecondaryDatabaseName, PrimaryResults.Count(), SecondaryResults.Count()));
            writer.WriteLine(String.Empty);

            writer.Flush();
        }

        /// <summary>
        /// Look for indexes which exist in the primary database but not the secondary database.
        /// </summary>
        /// <param name="DisplayHeader"></param>
        /// <param name="ScriptDrops"></param>
        /// <param name="ScriptCreates"></param>
        /// <param name="writer"></param>
        /// <param name="PrimaryServerName"></param>
        /// <param name="PrimaryDatabaseName"></param>
        /// <param name="SecondaryServerName"></param>
        /// <param name="SecondaryDatabaseName"></param>
        /// <param name="PrimaryResults"></param>
        /// <param name="SecondaryResults"></param>
        public static void StreamNotInSecondaryComparison(bool DisplayHeader, bool ScriptDrops, bool ScriptCreates, System.IO.StreamWriter writer,
            string PrimaryServerName, string PrimaryDatabaseName, string SecondaryServerName, string SecondaryDatabaseName,
            IEnumerable<IndexSet> PrimaryResults, IEnumerable<IndexSet> SecondaryResults)
        {
            if (writer == null)
                throw new Exception("No stream available.");

            if (DisplayHeader)
            {
                writer.WriteLine("=================================================================");
                writer.WriteLine("==================SECTION 2:  NOT IN SECONDARY===================");
                writer.WriteLine("=================================================================");
            }

            IEnumerable<IndexSet> NotInSecondary = IndexSet.GetNonexistentIndexes(PrimaryResults, SecondaryResults);
            foreach (IndexSet ix in NotInSecondary)
            {
                writer.WriteLine(String.Format("{0} on {1}.{2}", String.IsNullOrWhiteSpace(ix.IndexName) ? "HEAP" : ix.IndexName, ix.SchemaName, ix.TableName));
            }
            writer.WriteLine(String.Empty);

            if (ScriptDrops)
            {
                WriteDropStatements(writer, NotInSecondary);
                writer.WriteLine(String.Empty);
            }

            if (ScriptCreates)
            {
                WriteCreateStatements(writer, NotInSecondary);
                writer.WriteLine(String.Empty);
            }

            writer.Flush();
        }

        /// <summary>
        /// Look for indexes which exist in the secondary database but not in the primary.
        /// </summary>
        /// <param name="DisplayHeader"></param>
        /// <param name="ScriptDrops"></param>
        /// <param name="ScriptCreates"></param>
        /// <param name="writer"></param>
        /// <param name="PrimaryServerName"></param>
        /// <param name="PrimaryDatabaseName"></param>
        /// <param name="SecondaryServerName"></param>
        /// <param name="SecondaryDatabaseName"></param>
        /// <param name="PrimaryResults"></param>
        /// <param name="SecondaryResults"></param>
        public static void StreamNotInPrimaryComparison(bool DisplayHeader, bool ScriptDrops, bool ScriptCreates, System.IO.StreamWriter writer,
            string PrimaryServerName, string PrimaryDatabaseName, string SecondaryServerName, string SecondaryDatabaseName,
            IEnumerable<IndexSet> PrimaryResults, IEnumerable<IndexSet> SecondaryResults)
        {
            if (writer == null)
                throw new Exception("No stream available.");

            if (DisplayHeader)
            {
                writer.WriteLine("=================================================================");
                writer.WriteLine("====================SECTION 3:  NOT IN PRIMARY===================");
                writer.WriteLine("=================================================================");
            }

            IEnumerable<IndexSet> NotInPrimary = IndexSet.GetNonexistentIndexes(SecondaryResults, PrimaryResults);
            foreach (IndexSet ix in NotInPrimary)
            {
                writer.WriteLine(String.Format("{0} on {1}.{2}", String.IsNullOrWhiteSpace(ix.IndexName) ? "HEAP" : ix.IndexName, ix.SchemaName, ix.TableName));
            }
            writer.WriteLine(String.Empty);

            if (ScriptDrops)
            {
                WriteDropStatements(writer, NotInPrimary);
                writer.WriteLine(String.Empty);
            }

            if (ScriptCreates)
            {
                WriteCreateStatements(writer, NotInPrimary);
                writer.WriteLine(String.Empty);
            }

            writer.Flush();
        }

        /// <summary>
        /// Look for indexes which exist in both databases but whose definitions differ in some regard.
        /// </summary>
        /// <param name="DisplayHeader"></param>
        /// <param name="ScriptDrops"></param>
        /// <param name="ScriptCreates"></param>
        /// <param name="writer"></param>
        /// <param name="PrimaryServerName"></param>
        /// <param name="PrimaryDatabaseName"></param>
        /// <param name="SecondaryServerName"></param>
        /// <param name="SecondaryDatabaseName"></param>
        /// <param name="PrimaryResults"></param>
        /// <param name="SecondaryResults"></param>
        public static void StreamDifferences(bool DisplayHeader, bool ScriptDrops, bool ScriptCreates, System.IO.StreamWriter writer,
            string PrimaryServerName, string PrimaryDatabaseName, string SecondaryServerName, string SecondaryDatabaseName,
            IEnumerable<IndexSet> PrimaryResults, IEnumerable<IndexSet> SecondaryResults)
        {
            if (writer == null)
                throw new Exception("No stream available.");

            if (DisplayHeader)
            {
                writer.WriteLine("=================================================================");
                writer.WriteLine("======================SECTION 4:  DIFFERENCES====================");
                writer.WriteLine("=================================================================");
            }

            IEnumerable<IndexDifference> Differences = IndexDifference.GetDifferences(PrimaryResults, SecondaryResults);
            foreach (IndexDifference ix in Differences)
            {
                writer.WriteLine(String.Format("PRIMARY:   {0}{1} [{2}] ON [{3}].[{4}].[{5}].[{6}] ({7}) {8}{9}{20}SECONDARY: {10}{11} [{12}] ON [{13}].[{14}].[{15}].[{16}] ({17}) {18}{19}",
                    ix.IndexIsUnique ? "UNIQUE " : String.Empty,
                    ix.IndexType.Trim(),
                    ix.IndexName,
                    ix.ServerName,
                    ix.DatabaseName,
                    ix.SchemaName,
                    ix.TableName,
                    ix.Columns,
                    String.IsNullOrWhiteSpace(ix.IncludedColumns) ? String.Empty : " INCLUDE (" + ix.IncludedColumns + ")",
                    ix.HasFilter ? " WHERE " + ix.FilterDefinition : String.Empty,
                    ix.OtherIndexIsUnique ? "UNIQUE " : String.Empty,
                    ix.OtherIndexType.Trim(),
                    ix.IndexName,
                    SecondaryServerName,
                    SecondaryDatabaseName,
                    ix.SchemaName,
                    ix.TableName,
                    ix.OtherColumns,
                    String.IsNullOrWhiteSpace(ix.OtherIncludedColumns) ? String.Empty : " INCLUDE (" + ix.OtherIncludedColumns + ")",
                    ix.OtherHasFilter ? " WHERE " + ix.OtherFilterDefinition : String.Empty,
                    Environment.NewLine
                    ));
                writer.WriteLine(String.Empty);

                if (ScriptDrops && !ix.IsHeap)
                {
                    writer.WriteLine("--Drop Index");
                    writer.WriteLine(ix.DropScript);
                    writer.WriteLine(String.Empty);
                }

                if (ScriptCreates && !ix.IsHeap)
                {
                    writer.WriteLine("--Create Primary Index");
                    writer.WriteLine(ix.PrimaryIndexCreateScript);
                    writer.WriteLine(String.Empty);
                    writer.WriteLine(String.Empty);

                    writer.WriteLine("--Create Secondary Index");
                    writer.WriteLine(ix.SecondaryIndexCreateScript);
                    writer.WriteLine(String.Empty);
                    writer.WriteLine(String.Empty);
                }
            }

            writer.Flush();
        }

        #region Private Helper Methods

        private static void WriteDropStatements(System.IO.StreamWriter writer, IEnumerable<IndexSet> set)
        {
            foreach (IndexSet ix in set.Where(x => !x.IsHeap))
            {
                writer.WriteLine(ix.DropScript);
                writer.WriteLine(String.Empty);
            }
        }

        private static void WriteCreateStatements(System.IO.StreamWriter writer, IEnumerable<IndexSet> set)
        {
            foreach (IndexSet ix in set.Where(x => !x.IsHeap))
            {
                writer.WriteLine(ix.CreateScript);
                writer.WriteLine(String.Empty);
            }
        }

        #endregion
    }
}
