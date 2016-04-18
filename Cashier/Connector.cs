using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks; 

namespace Cashier
{
    class Connector
    {
        /// <summary>
        /// Member that holds the built ConnectionString
        /// </summary>
        private SqlConnectionStringBuilder ConnectionStringBuilder;


        /// <summary>
        /// Builds a ConnectionString using SqlConnectionStringBuilder and returns it in 
        /// a format that SqlConnection recognizes
        /// </summary>
        /// <returns>A ConnectionString</returns>
        public String getConnectionString() {
            return ConnectionStringBuilder.ConnectionString;
        }


        /// <summary>
        /// Constructs a connection object and builds a ConnectionString
        /// </summary>
        /// <param name="DataSource">DataSource string to use with the connection (hostname)</param>
        /// <param name="InitialCatalog">Database string to use with the connection</param>
        /// <param name="UserID">The UserID to use with the connection</param>
        /// <param name="Password">The Password to use with the connection</param>
        public Connector(String DataSource, String InitialCatalog, String UserID, String Password)
        {
            this.ConnectionStringBuilder = new SqlConnectionStringBuilder();
            this.ConnectionStringBuilder.DataSource = DataSource;
            this.ConnectionStringBuilder.InitialCatalog = InitialCatalog;
            this.ConnectionStringBuilder.UserID = UserID;
            this.ConnectionStringBuilder.Password = Password;
        }


        /// <summary>
        /// Constructs a connection object using the InitialCatalog as the parameter for the InitialCatalog
        /// and UserID. Handy for databases that use the same database and username
        /// </summary>
        /// <param name="DataSource">DataSource string to use with the connection (hostname)</param>
        /// <param name="InitialCatalog">Database string and UserID string to use with the connection</param>
        /// <param name="Password">The Password to use with the connection</param>
        public Connector(String DataSource, String InitialCatalog, String Password) : this(DataSource, InitialCatalog, InitialCatalog, Password) { }
    }
}
