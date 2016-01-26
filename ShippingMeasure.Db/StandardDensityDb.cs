using ShippingMeasure.Common;
using ShippingMeasure.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShippingMeasure.Db
{
    public class StandardDensityDb : DbBase
    {
        private IEnumerable<StandardDensity> all = null;

        public IEnumerable<StandardDensity> GetAll()
        {
            if (this.all == null)
            {
                var items = new List<StandardDensity>();

                new QueryContext(this.ConnectionString, "SELECT * FROM StandardDensity").ExecuteReader(r => items.Add(new StandardDensity
                {
                    Temp = r["Temp"].TryToDecimal(),
                    DensityMeasured = r["DensityMeasured"].TryToDecimal(),
                    DensityOfStandard = r["DensityOfStandard"].TryToDecimal()
                }));

                this.all = items;
            }

            return this.all;
        }
    }
}
