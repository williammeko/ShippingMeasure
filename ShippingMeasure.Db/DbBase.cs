using ShippingMeasure.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShippingMeasure.Db
{
    public abstract class DbBase
    {
        private string connectionString;

        public string ConnectionString
        {
            get
            {
                if (String.IsNullOrEmpty(this.connectionString))
                {
                    this.connectionString = Config.DataConnectionString;
                }

                return this.connectionString;
            }

            set
            {
                this.connectionString = value;
            }
        }
    }
}
