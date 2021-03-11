using Respiratory_Analysis_CPET.AnyModelsForProjects;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Respiratory_Analysis_CPET
{
    /// <summary>
    /// Класс представляет из себя обьект набора данных, которые приходят от прибора
    /// </summary>
    public class SampleBreathing
    {
        public IEnumerable<Parameter> GetParameters()
        {
            return new Parameter[] { Flow,O2,CO2 };
        }
        public Parameter Flow { get; }
        public Parameter O2 { get; }
        private Parameter _cO2;
        public Parameter CO2 
        { 
            get => _cO2; 
            private set
            {
                if (value.ParameterValue.Value < 0)
                    value.ParameterValue.Value = 0;
                _cO2 = value;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SampleBreathing"/> class.
        /// </summary>
        /// <param name="flow">Данные по потоку в л/с</param>
        /// <param name="o2">даные по О2 в Об.%</param>
        /// <param name="cO2">даные по СО2 в Об.%</param>
        public SampleBreathing(double flow, double o2, double cO2)
        {
            Flow = new Parameter("Flow", new ParameterValue(Math.Round(flow, 2), "L/S"));
            O2 = new Parameter("O2", new ParameterValue(Math.Round(o2, 2), "Vol.%"));
            CO2 = new Parameter("CO2", new ParameterValue(Math.Round(cO2,2),"Vol.%"));
        }
    }
}
