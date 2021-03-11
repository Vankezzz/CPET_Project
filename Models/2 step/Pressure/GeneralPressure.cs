using Microsoft.Extensions.Logging;
using Respiratory_Analysis_CPET.AnyModelsForProjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Respiratory_Analysis_CPET
{
    public class GeneralPressure
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GeneralPressure"/> class.
        /// </summary>
        /// <param name="pECO2">PECO2 - Представляет собой среднее значение PCO2 в выдыхаемом воздухе. Вычисляется:𝑃𝐸𝐶𝑂2=(𝑉𝐶𝑂2(𝑆𝑇𝑃𝐷))/(𝑉𝐸(𝑆𝑇𝑃𝐷))∗(𝑃𝐵−47(𝑚𝑚𝐻𝑔)). Источник:  Principles ofExerciseTestingandInterpretation [Формулы и текст]: Wasserman K, Hansen JE, Sue DY, 5th Ed.  Lippincott Williams  Wilkins – Америка, 2012. – 715стр. ISBN-13: 978-1-60913-899-8</param>
        public GeneralPressure(double pECO2)
        {
            PECO2 = new Parameter("PECO2", new ParameterValue(Math.Round(pECO2, 2),"Pa"));
        }

        public Parameter PECO2 { get; private set; }//  [mmHg]

        public static GeneralPressure Calculate(double vco2, double ve, ILogger logger)
        {
            return new GeneralPressure(Calculate_PECO2(vco2, ve,logger));
        }
        public static  double Calculate_PECO2(double vco2, double ve, ILogger logger)
        {
            try
            {
                return vco2 * 713 / ve;
            }
            catch (Exception ex)
            {
                logger.LogError(string.Format("Error: {0}, Stack: {1}", ex.Message, ex.StackTrace));
                return -1;
            }
           
        }
    }
}
