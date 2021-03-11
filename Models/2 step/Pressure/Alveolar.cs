using Microsoft.Extensions.Logging;
using Respiratory_Analysis_CPET.AnyModelsForProjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Respiratory_Analysis_CPET
{
    public class Alveolar
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AlveolarValue"/> class.
        /// </summary>
        /// <param name="pACO2">PACO2 -Альвеолярное напряжение CO2. Вычисление: PACO2=PETCO2 в покое!. Источник: Making Sense of Exercise Testing - 45стр.</param>
        /// <param name="pAO2">PAO2 - Альвеолярное напряжение O2. Вычисляется: 𝑃𝐴𝑂2 [𝑚𝑚𝐻𝑔]=𝐹𝐼𝑂2∗(𝑃𝐵−47)−𝑃𝑎𝐶𝑂2/𝑅. Источник: The physiological basis of pulmonary gas - стр 4</param>
        public Alveolar(double pACO2, double pAO2)
        {
            PACO2 = new Parameter("PACO2", new ParameterValue(Math.Round(pACO2, 2),"Pa"));
            PAO2 = new Parameter("PAO2", new ParameterValue(Math.Round(pAO2, 2), "Pa"));
        }

        public Parameter PACO2 { get; private set; }
        public Parameter PAO2 { get; private set; }
        public static Alveolar GetData(List<double> fio2data, double petco2, double PaCO2, double rer, ILogger logger)
        {
            return new Alveolar(Calculate_PACO2(petco2,logger), Calculate_PAO2(fio2data, PaCO2, rer,logger));
        }
        public static double Calculate_PAO2(List<double> fio2data, double PaCO2, double rer, ILogger logger)
        {
            try
            {
                return fio2data.Average() * 0.01 * 713 - PaCO2 / rer;
            }
            catch (Exception ex)
            {
                logger.LogError(string.Format("Error: {0}, Stack: {1}", ex.Message, ex.StackTrace));
                return -1;
            }
            
        }
        public static double Calculate_PACO2(double petco2, ILogger logger)
        {
            try
            {
                return petco2 ;
            }
            catch (Exception ex)
            {
                logger.LogError(string.Format("Error: {0}, Stack: {1}", ex.Message, ex.StackTrace));
                return -1;
            }
            
        }
    }
}
