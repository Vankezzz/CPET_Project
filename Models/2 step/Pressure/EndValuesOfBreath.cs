
using Microsoft.Extensions.Logging;
using Respiratory_Analysis_CPET.AnyModelsForProjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Respiratory_Analysis_CPET
{
    public class EndValuesOfBreath
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EndValuesOfBreath"/> class.
        /// </summary>
        /// <param name="fETCO2">Конечное значение CO2 на выдохе считается как среднее по 10 значениям и если из них будет, что то меньше 2 процентов, то они выкидываются</param>
        /// <param name="fETO2">Конечное значение O2 на выдохе считается как среднее по 10 значениям и если из них будет, что то больще процентов, то они выкидываются</param>
        /// <param name="pETCO2">FETCO2 переведенное в PETCO2  в мм.рт.ст с помощью коэффициента (Pв -47)=713</param>
        /// <param name="pETO2">FETO2 переведенное в PETO2  в мм.рт.ст с помощью коэффициента (Pв -47)=713</param>
        public EndValuesOfBreath(double fETCO2, double fETO2, double pETCO2, double pETO2)
        {
            FETCO2 = new Parameter("FETCO2", new ParameterValue(Math.Round(fETCO2, 2), "Vol.%"));
            FETO2 = new Parameter("FETO2", new ParameterValue(Math.Round(fETO2, 2), "Vol.%"));
            PETCO2 = new Parameter("PETCO2", new ParameterValue(Math.Round(pETCO2, 2), "Pa"));
            PETO2 = new Parameter("PETO2", new ParameterValue(Math.Round(pETO2, 2), "Pa"));
        }

        public Parameter FETCO2 { get; private set; }
        public Parameter FETO2 { get; private set; }
        public Parameter PETCO2 { get; private set; }
        public Parameter PETO2 { get; private set; }
        public static EndValuesOfBreath GetData(List<double> FECO2DATA, List<double> FEO2DATA, int bufferFET, ILogger logger)
        {
            try
            {
                List<double> FECO2list = new List<double>() { };
                List<double> FEO2list = new List<double>() { };
                byte iterator = 0;
                for (int i = FECO2DATA.Count - bufferFET + 1; i < FECO2DATA.Count; i++)
                {
                    if (FECO2DATA[i] < 2)
                    {
                        while (FECO2DATA[i] < 2)
                        {
                            iterator++;
                            FECO2list.Add(FECO2DATA[(FECO2DATA.Count - bufferFET + 1) - iterator]);
                        }
                    }
                    else
                    {
                        FECO2list.Add(FECO2DATA[i]);
                    }
                }
                for (int i = FEO2DATA.Count - bufferFET - 1; i < FEO2DATA.Count - 1; i++)
                {
                    if (FEO2DATA[i] < 20)
                    {
                        FEO2list.Add(FEO2DATA[i]);
                    }
                }
                return new EndValuesOfBreath(FECO2list.Average(), FEO2list.Average(), FECO2list.Average() * 0.01 * 713, FEO2list.Average() * 0.01 * 713);
            }
            catch (Exception ex)
            {
                logger.LogError(string.Format("Error: {0}, Stack: {1}", ex.Message, ex.StackTrace));
                return new EndValuesOfBreath(-1, -1, -1, -1);
            }

        }
    }
}
