
using Microsoft.Extensions.Logging;
using Respiratory_Analysis_CPET.AnyModelsForProjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Respiratory_Analysis_CPET
{
    public class GasCoefficient
    {
        /// <summary>
        /// Газовые коэффициенты дыхательной системы
        /// </summary>
        /// <summary>
        /// Initializes a new instance of the <see cref="GasCoefficient"/> class.
        /// </summary>
        /// <param name="aa_DO2">Альвеолярная - артериальная разница давления кислорода. Вычисляется:Aa_DO2= PACO2 - PaCO2 Источник: Principles of Exercise Testing and Interpretation [Формулы и текст]: Wasserman K, Hansen JE, Sue DY, 5th Ed.  Lippincott Williams  Wilkins – Америка, 2012. – 714стр. ISBN-13: 978-1-60913-899-8</param>
        /// <param name="aa_DCO2">Альвеолярная - артериальная разница давления диоксида углерода. Вычисляется:Aa_DO2= PACO2 - PaCO2 Источник: Principles of Exercise Testing and Interpretation [Формулы и текст]: Wasserman K, Hansen JE, Sue DY, 5th Ed.  Lippincott Williams  Wilkins – Америка, 2012. – 714стр. ISBN-13: 978-1-60913-899-8</param>
        /// <param name="aET_DCO2">Артериально-конечная разница давления диоксида углерода. Вычисляется:AET_DO2= PaCO2 - PETCO2 Источник: Principles of Exercise Testing and Interpretation [Формулы и текст]: Wasserman K, Hansen JE, Sue DY, 5th Ed.  Lippincott Williams  Wilkins – Америка, 2012. – 714стр. ISBN-13: 978-1-60913-899-8</param>
        /// <param name="dLO2">DLO2 - Определение диффузионной способности легких. Вычисляется: 𝐷𝐿𝑂2=𝑉𝑂2/(𝑃𝐴𝑂2−𝑃𝑎𝑂2) . Источник:  Cardiopulmonary Exercise Testing [Формулы и текст]: Darcy D. Marciniuk, Bruce D. Johnson, J. Alberto Neder, and Denis E. O’Donnell. – Америка, 2013. – 6стр.</param>
        public GasCoefficient(double aa_DO2, double aa_DCO2, double aET_DCO2, double dLO2)
        {
            Aa_DO2 = new Parameter("Aa_DO2", new ParameterValue(Math.Round(aa_DO2, 2),"Pa"));
            Aa_DCO2 = new Parameter("Aa_DCO2", new ParameterValue(Math.Round(aa_DCO2, 2), "Pa"));
            AET_DCO2 = new Parameter("AET_DCO2", new ParameterValue(Math.Round(aET_DCO2, 2), "Pa"));
            DLO2 = new Parameter("DLO2", new ParameterValue(Math.Round(dLO2, 2), "L/Pa*M"));
        }

        public Parameter Aa_DO2 { get; private set; }
        public Parameter Aa_DCO2 { get; private set; }
        public Parameter AET_DCO2 { get; private set; }
        public Parameter DLO2 { get; private set; }
        public static GasCoefficient GetData(double VO2, double PACO2, double PaCO2, double PAO2, double PaO2, double PETCO2, ILogger logger)
        {
            return new GasCoefficient(Calculate_Aa_DO2(PAO2, PaO2, logger), Calculate_Aa_DCO2(PACO2, PaCO2, logger), Calculate_aET_DCO2(PaCO2, PETCO2, logger), Calculate_DLO2(VO2, Calculate_Aa_DO2(PAO2, PaO2, logger), logger));
        }
        static double Calculate_Aa_DO2(double PAO2, double PaO2, ILogger logger)
        {
            try
            {
                return PAO2 - PaO2;
            }
            catch (Exception ex)
            {
                logger.LogError(string.Format("Error: {0}, Stack: {1}", ex.Message, ex.StackTrace));
                return -1;
            }

        }
        static double Calculate_Aa_DCO2(double PACO2, double PaCO2, ILogger logger)
        {
            try
            {
                return PACO2 - PaCO2;
            }
            catch (Exception ex)
            {
                logger.LogError(string.Format("Error: {0}, Stack: {1}", ex.Message, ex.StackTrace));
                return -1;
            }

        }
        static double Calculate_aET_DCO2(double PaCO2, double PETCO2, ILogger logger)
        {
            try
            {
                return PaCO2 - PETCO2;
            }
            catch (Exception ex)
            {
                logger.LogError(string.Format("Error: {0}, Stack: {1}", ex.Message, ex.StackTrace));
                return -1;
            }

        }
        static double Calculate_DLO2(double VO2, double AaDO2, ILogger logger)
        {
            try
            {
                return VO2 / AaDO2;
            }
            catch (Exception ex)
            {
                logger.LogError(string.Format("Error: {0}, Stack: {1}", ex.Message, ex.StackTrace));
                return -1;
            }

        }
    }
}
