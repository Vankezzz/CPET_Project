using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Respiratory_Analysis_CPET.AnyModelsForProjects
{
    public record ParameterValue
    {
        public ParameterValue(double value, string unit)
        {
            Value = value;
            Unit = unit;
        }

        public double Value { get; set; }
        public string Unit { get;  }
    }
}
