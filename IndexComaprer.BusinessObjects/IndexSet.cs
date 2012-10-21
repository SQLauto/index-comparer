using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Security.Cryptography;

namespace IndexComparer.BusinessObjects
{
    public class IndexSet : IEquatable<IndexSet>
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

        #endregion

        #region Additional Properties

        public string IndexTypeCode
        {
            get
            {
                switch(IndexType.Trim())
                {
                    case "HEAP": return "H";
                    case "NONCLUSTERED": return "N";
                    case "CLUSTERED": return "C";
                    default: return "?";
                }
            }
        }

        public string SchemaAndTable
        {
            get
            {
                return String.Format("{0}.{1}", SchemaName, TableName);
            }
        }

        public string DropScript
        {
            get
            {
                if (IsHeap)
                    return "This is a heap.  That's not something you can go out of your way to drop.";
                else
                    return String.Format("IF EXISTS(SELECT * FROM sys.indexes WHERE name = '{0}' and OBJECT_SCHEMA_NAME(object_id) = '{1}' and OBJECT_NAME(object_id) = '{2}'){3}\tDROP INDEX [{0}] ON [{1}].[{2}]", IndexName, SchemaName, TableName, Environment.NewLine);
            }
        }

        public string CreateScript
        {
            get
            {
                if (IsHeap)
                    return "This is a heap.  That's not something you can go out of your way to create.";
                else
                    return String.Format("{9}{10}CREATE {3}{4} INDEX [{0}] ON [{1}].[{2}]({5}){6}{7}{8}",
                        IndexName,
                        SchemaName,
                        TableName,
                        IndexIsUnique ? "UNIQUE " : String.Empty,
                        IndexType,
                        Columns,
                        String.IsNullOrWhiteSpace(IncludedColumns) ? String.Empty : " INCLUDE (" + IncludedColumns + ")",
                        HasFilter ? " WHERE " + FilterDefinition : String.Empty,
                        CanRebuildOnline ? " WITH (ONLINE = ON ) " : String.Empty,
                        DropScript,
                        Environment.NewLine
                        );
            }
        }

        public bool IsHeap
        {
            get
            {
                return IndexType.Trim() == "HEAP";
            }
        }

        #endregion

        #region IEquatable Implementation

        public bool Equals(IndexSet other)
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

        #region Object Methods

        public bool TotallyEquals(IndexSet other)
        {
            if (Object.ReferenceEquals(other, null)) return false;
            if (Object.ReferenceEquals(this, other)) return true;

            return Equals(other) && IndexIsUnique.Equals(other.IndexIsUnique) && IndexType.Equals(other.IndexType) &&
                Columns.Equals(other.Columns) && IncludedColumns.Equals(other.IncludedColumns) &&
                HasFilter.Equals(other.HasFilter) && FilterDefinition.Equals(other.FilterDefinition);
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// This method returns the set of indexes which exist in Base but not in Comp.  This does NOT check whether the index columns are the same.
        /// </summary>
        /// <param name="Base">The primary index set.</param>
        /// <param name="Comp">The comaprison index set.</param>
        /// <returns>A list of indexes which exist in Base but not in Comp.</returns>
        public static IEnumerable<IndexSet> GetNonexistentIndexes(IEnumerable<IndexSet> Base, IEnumerable<IndexSet> Comp)
        {
            return Base.Except(Comp);
        }

        /// <summary>
        /// Returns a set of index data based on the server name and database name.  If no connection string is passed in,
        /// assume Windows authentication.
        /// </summary>
        /// <param name="ServerName">A string with the server name.</param>
        /// <param name="DatabaseName">A string with the database name.</param>
        /// <returns>A set of indexes associated with that database.  If there are no results, an empty list is returned.</returns>
        public static List<IndexSet> RetrieveIndexData(string ServerName, string DatabaseName, string ConnectionString = null)
        {
            if (ConnectionString == null)
                ConnectionString = String.Format("server={0};database={1};trusted_connection=yes", ServerName, DatabaseName);
            return pRetrieveIndexData(ServerName, DatabaseName, ConnectionString);
        }

        private static List<IndexSet> pRetrieveIndexData(string ServerName, string DatabaseName, string ConnectionString)
        {
            List<IndexSet> results = new List<IndexSet>();

            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                string sql = @"
declare @sql nvarchar(max);
set @sql = N'
with indexcolumns as
(
    select
        object_schema_name(si.object_id) as SchemaName,
        st.object_id,
        st.name as TableName,
        si.index_id,
        si.name as IndexName,
        si.type_desc as IndexType,
        si.is_unique as IndexIsUnique,
        sc.name as ColumnName,
        sic.key_ordinal as ColumnOrderInIndex,
        sic.is_descending_key as ColumnOrderIsDescending,
        sic.is_included_column as IsIncludedColumn, ' +
case
	when @@microsoftversion / 0x01000000 > 9 then N'
        si.has_filter as HasFilter,
        coalesce(si.filter_definition, '''') as FilterDefinition, '
    else
		'0 as HasFilter,
		'''' as FilterDefinition, ' 
end + N'
        cast
        (
			case 
				when 
					(@@Version like ''%Enterprise%'' or @@Version like ''%Developer%'')
					AND si.type_desc not like ''XML''
					AND NOT (si.type_desc = ''CLUSTERED'' and si.is_unique = 1)
					AND NOT (si.type_desc = ''CLUSTERED'' and si.is_disabled = 1)
					AND NOT EXISTS (
										select * 
										from sys.columns sci 
											inner join sys.types st on sci.system_type_id = st.system_type_id 
										where sci.object_id = si.object_id 
											and st.name in (''image'', ''text'', ''ntext'', ''geometry'', ''geography'')
											and si.type_desc = ''CLUSTERED''
									)
					then 1
				else 0
			end
        as bit ) as CanRebuildOnline
    from 
        sys.indexes si 
        inner join sys.tables st on si.object_id = st.object_id
        left outer join sys.index_columns sic on sic.object_id = si.object_id and sic.index_id = si.index_id
        left outer join sys.columns sc on sic.column_id = sc.column_id and sic.object_id = sc.object_id
)
select distinct
    SchemaName,
    TableName,
    IndexName,
    IndexType,
    IndexIsUnique,
    coalesce(substring((SELECT '', '' + ColumnName + case when ColumnOrderIsDescending = 1 then '' DESC'' else '''' end FROM indexcolumns ic2 WHERE ic1.object_id = ic2.object_id and ic1.index_id = ic2.index_id and IsIncludedColumn = 0 ORDER BY ColumnOrderInIndex FOR XML PATH ( '''' )), 3, 1000), '''') as [Columns],
    coalesce(substring((SELECT '', '' + ColumnName + case when ColumnOrderIsDescending = 1 then '' DESC'' else '''' end FROM indexcolumns ic3 WHERE ic1.object_id = ic3.object_id and ic1.index_id = ic3.index_id and IsIncludedColumn = 1 ORDER BY ColumnOrderInIndex FOR XML PATH ( '''' )), 3, 1000), '''') as [IncludedColumns],
    HasFilter,
    FilterDefinition,
    CanRebuildOnline
from
    indexcolumns ic1;
';

exec sp_executesql @sql;                    
";

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.CommandType = System.Data.CommandType.Text;
                    cmd.CommandTimeout = 30;
                    conn.Open();

                    SqlDataReader dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        IndexSet ix = new IndexSet();
                        ix.ServerName = ServerName;
                        ix.DatabaseName = DatabaseName;
                        ix.SchemaName = dr["SchemaName"].ToString();
                        ix.TableName = dr["TableName"].ToString();
                        ix.IndexName = dr["IndexName"].ToString();
                        ix.IndexType = dr["IndexType"].ToString();
                        ix.IndexIsUnique = Convert.ToBoolean(dr["IndexIsUnique"]);
                        ix.Columns = dr["Columns"].ToString();
                        ix.IncludedColumns = dr["IncludedColumns"].ToString();
                        ix.HasFilter = Convert.ToBoolean(dr["HasFilter"]);
                        ix.FilterDefinition = dr["FilterDefinition"].ToString();
                        ix.CanRebuildOnline = Convert.ToBoolean(dr["CanRebuildOnline"]);

                        results.Add(ix);
                    }
                }
            }

            return results;
        }

        #endregion
    }
}
