using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShippingMeasure.Core.Models
{
    public class Vessel
    {
        public string Name { get; set; }

        public string CertNo { get; set; }

        public List<Pipe> Pipes { get; } = new List<Pipe>();
    }
}
