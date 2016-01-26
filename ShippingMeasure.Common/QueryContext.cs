using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Linq;
using System.Text;

namespace ShippingMeasure.Common
{
    public class QueryContext
    {
        public string CommandText { get; set; }

        public OleDbConnection Connection { get; set; } = null;

        public OleDbTransaction Transaction { get; set; }

        public string ConnectionString { get; set; }

        public List<OleDbParameter> Parameters { get; } = new List<OleDbParameter>();

        public QueryContext(string connectionString, string commandText)
        {
            this.ConnectionString = connectionString;
            this.CommandText = commandText;
        }
    }
}
