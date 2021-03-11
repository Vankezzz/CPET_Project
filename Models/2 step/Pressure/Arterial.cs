
using Microsoft.Extensions.Logging;
using Respiratory_Analysis_CPET.AnyModelsForProjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Respiratory_Analysis_CPET
{
    public  class Arterial
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="ArterialValue"/> class.
        /// </summary>
        /// <param name="paCO2">PaCO2 - артериальное напряжение CO2. Вычисляется: 𝑃𝑎𝐶𝑂2=𝑉𝐶𝑂2/𝑉𝐴∗863 . Источник: Cardiopulmonary Exercise Testing [Формулы и текст]: Darcy D. Marciniuk, Bruce D. Johnson, J. Alberto Neder, and Denis E. O’Donnell. – Америка, 2013. – 89 стр.</param>
        /// <param name="paO2">PaO2 - артериальное напряжение O2</param>
        public Arterial(double paCO2, double paO2)
        {
            PaCO2 = new Parameter("PaCO2", new ParameterValue(Math.Round(paCO2, 2),"Pa"));
            PaO2 = new Parameter("PaO2", new ParameterValue(Math.Round(paO2, 2), "Pa"));
        }

        public Parameter PaCO2 { get; private set; }
        public Parameter PaO2 { get; private set; }
        public static Arterial GetData( double VCO2, double VA, ILogger logger)
        {
            return new Arterial(Calculate_PaCO2(VCO2, VA,logger), Calculate_PaO2());
        }
        public static double Calculate_PaCO2(double VCO2, double VA, ILogger logger)
        {
            try
            {
                return VCO2 * 863 / VA;
                //return 5.5 + (0.9 * PETCO2) - (0.0021 * VT) Второй способ
            }
            catch (Exception ex)
            {
                logger.LogError(string.Format("Error: {0}, Stack: {1}", ex.Message, ex.StackTrace));
                return -1;
            }
               
        }
        public static double Calculate_PaO2()
        {
            return 0;
        }
    }
}
