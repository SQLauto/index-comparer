using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;

namespace IndexComparer.BusinessObjects
{
    public class Database
    {
        public string Name { get; set; }

        /// <summary>
        /// Returns a list of valid datbases based on the server name.  If no connection string is passed in,
        /// assume Windows authentication.
        /// </summary>
        /// <param name="ServerName">A string with the server name.</param>
        /// <param name="ConnectionString">A string with the connection string information.  This is an optional parameter; if it is
        /// left empty, we will assume Windows authentication is valid.</param>
        /// <returns>A set of databases associated with that server.  If there are no results, an empty list is returned.</returns>
        public static IEnumerable<Database> GetByServerName(string ServerName, string ConnectionString = null)
        {
            if (String.IsNullOrWhiteSpace(ServerName))
                throw new ApplicationException("You must enter a valid server name.");

            if (ConnectionString == null)
                ConnectionString = String.Format("server={0};database=tempdb;trusted_connection=yes", ServerName);

            List<Database> results = new List<Database>();

            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                string sql = "select name from sys.databases order by name";

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.CommandType = System.Data.CommandType.Text;
                    cmd.CommandTimeout = 30;
                    conn.Open();

                    SqlDataReader dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        Database db = new Database();
                        db.Name = dr["name"].ToString();

                        results.Add(db);
                    }
                }
            }

            return results;
        }
    }
}
