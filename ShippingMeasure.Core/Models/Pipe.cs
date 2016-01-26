﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShippingMeasure.Core.Models
{
    public class Pipe
    {
        public string Name { get; set; }

        public decimal Volume { get; set; }

        public override string ToString()
        {
            return String.Format("{0} ({1} m³)", this.Name, this.Volume);
        }
    }
}
