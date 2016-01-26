using ShippingMeasure.Common;
using ShippingMeasure.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShippingMeasure.Db
{
    public class VcfDb : DbBase
    {
        private List<VolumeCorrectionFactor> all = null;

        public List<VolumeCorrectionFactor> GetAll()
        {
            if (this.all == null)
            {
                var items = new List<VolumeCorrectionFactor>();

                new QueryContext(this.ConnectionString, "SELECT * FROM VolumeCorrectionFactors").ExecuteReader(r => items.Add(new VolumeCorrectionFactor
                {
                    Temp = r["Temp"].TryToDecimal(),
                    DensityOfStandard = r["DensityOfStandard"].TryToDecimal(),
                    Factor = r["Factor"].TryToDecimal(),
                }));

                this.all = items;
            }

            return this.all;
        }
    }
}
