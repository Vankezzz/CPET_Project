
using Microsoft.Extensions.Logging;
using Respiratory_Analysis_CPET.AnyModelsForProjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Respiratory_Analysis_CPET
{
    public class Coefficient
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Coefficient"/> class.
        /// </summary>
        /// <param name="eQO2">Ventilatory Equivalent for O2 - Респираторный эквивалент для O2 безразмерный. Вычисляется: 𝐸𝑄𝑂2=(𝑉𝐸(𝐵𝑇𝑃𝑆)−𝑉𝑑𝑚∗𝐵𝐹)/(𝑉𝑂2(𝑆𝑇𝑃𝐷)). Источник: Principles of Exercise Testing and Interpretation [Формулы и текст]: Wasserman K, Hansen JE, Sue DY, 5th Ed.  Lippincott Williams  Wilkins – Америка, 2012. – 713стр. ISBN-13: 978-1-60913-899-8.</param>
        /// <param name="eQCO2">Ventilatory Equivalent for CO2 – Респираторный эквивалент для CO2 безразмерный. Вычисляется: 𝐸𝑄𝑂2=(𝑉𝐸(𝐵𝑇𝑃𝑆)−𝑉𝑑𝑚∗𝐵𝐹)/(𝑉𝑂2 (𝑆𝑇𝑃𝐷)). Источник: Principles of Exercise Testing and Interpretation [Формулы и текст]: Wasserman K, Hansen JE, Sue DY, 5th Ed.  Lippincott Williams  Wilkins – Америка, 2012. – 713стр. ISBN-13: 978-1-60913-899-8</param>
        /// <param name="vO2_Kg">Потребление кислорона кг . Вычисляется: V𝑂2_Kg=VO2/BW. Источник: </param>
        /// <param name="vCO2_Kg">Выделение диоксида углерода на кг . Вычисляется: VC𝑂2_Kg=VCO2/BW. Источник: </param>
        /// <param name="mETS">METs - Метаболический эквивалент. Вычисляется: METS=𝑉𝑂2/(3.5∗𝐵𝑊). Источник:</param>
        /// <param name="rER">RQ(RER) Respiratory Quotient – Коэффициент дыхания, безразмерный. Вычисляется (по первой формуле): RQ (RER)=𝑉𝐶𝑂2/𝑉𝑂2 =𝐹𝐴𝐶𝑂2/(𝐹𝐼𝑂2−𝐹𝐴𝑂2)=𝑃𝐴𝐶𝑂2/(𝑃𝐼𝑂2−𝑃𝐴𝑂2). Источник: The physiological basis of pulmonary gas exchange: implications for clinical interpretation of arterial blood gases [Формулы и текст]: Peter D. Wagner - 4 страница</param>
        /// <param name="vO2pred">Прогназируемое значение VO2. Вычисляется:VO2pred=10.3*W+5.8*BW+151 (или есть еще VO2=2*W+3.5*BW). Источник: Дипломная работа раздел 1.2.3 </param>
        /// <param name="o2Pulse">(Режим с Нагр пробами)Пульсовое О2 - макс. отношение потребления О2 к ЧСС . Вычисляется:𝑂2𝑝𝑢𝑙𝑠𝑒=𝑉𝑂2/𝐻𝑅. Источник:Making Sense of Exercise Testing  - стр 60 </param>
        public Coefficient(double eQO2, double eQCO2, double vO2_Kg, double vCO2_Kg, double mETS, double rER, double vO2pred, double o2Pulse)
        {
            EQO2 = new Parameter("EQO2", new ParameterValue(Math.Round(eQO2, 2),"None"));
            EQCO2 = new Parameter("EQCO2", new ParameterValue(Math.Round(eQCO2, 2), "None"));
            VO2_Kg = new Parameter("VO2/Kg ", new ParameterValue(Math.Round(vO2_Kg, 2), "L/(M*Kg)"));
            VCO2_Kg = new Parameter("VCO2/Kg", new ParameterValue(Math.Round(vCO2_Kg, 2), "L/(M*Kg)"));
            METS = new Parameter("METS", new ParameterValue(Math.Round(mETS, 2), "None"));
            RER = new Parameter("RER", new ParameterValue(Math.Round(rER, 2), "None"));
            VO2pred = new Parameter("VO2pred", new ParameterValue(Math.Round(vO2pred, 2), "None"));
            O2pulse = new Parameter("O2pulse", new ParameterValue(Math.Round(o2Pulse, 2), "None"));
        }
        public IEnumerable<Parameter> GetParameters()
        {
            return new Parameter[] { EQO2,EQCO2,VO2_Kg,VCO2_Kg,METS,RER,VO2pred,O2pulse };
        }
        public Parameter EQO2 { get; private set; }
        public Parameter EQCO2 { get; private set; }
        public Parameter VO2_Kg { get; private set; }
        public Parameter VCO2_Kg { get; private set; }
        public Parameter METS { get; private set; }
        public Parameter RER { get; private set; }
        public Parameter VO2pred { get; private set; }
        public Parameter O2pulse { get; private set; }
        public static Coefficient Calculate(double VE,double VO2,double VCO2,double BF,double BW,int W,double VDm, ILogger logger)
        {
            return new Coefficient(Calculate_EQO2(VE,VO2,BF,VDm, logger), Calculate_EQCO2(VE,BF,VCO2,VDm, logger), Calculate_VO2_Kg(VO2,BW,logger), Calculate_VCO2_Kg(VCO2,BW,logger), Calculate_METS(VO2,BW,logger), Calculate_RER(VO2,VCO2,logger), Calculate_VO2pred(BW,W,logger), Calculate_O2Pulse(1,1,logger));
        }
        public static double Calculate_EQO2(double VE, double VO2, double BF, double VDm, ILogger logger)
        {
            try
            {
                return (VE - VDm * BF) / VO2;
            }
            catch (Exception ex)
            {
                logger.LogError(string.Format("Error: {0}, Stack: {1}", ex.Message, ex.StackTrace));
                return -1;
            }
           
        }
        public static double  Calculate_EQCO2(double VE, double BF, double VCO2, double VDm, ILogger logger)
        {
            try
            {
                return (VE - VDm * BF) / VCO2;
                // второй способ посчитать return 1 / (_PaCO2 * (1 - _VD / _VT));
            }
            catch (Exception ex)
            {
                logger.LogError(string.Format("Error: {0}, Stack: {1}", ex.Message, ex.StackTrace));
                return -1;
            }

        }
        public static double  Calculate_VCO2_Kg(double VCO2, double BW, ILogger logger)
        {
            try
            {
                return VCO2/BW;
            }
            catch (Exception ex)
            {
                logger.LogError(string.Format("Error: {0}, Stack: {1}", ex.Message, ex.StackTrace));
                return -1;
            }

        }
        public static double  Calculate_VO2_Kg(double VO2, double BW, ILogger logger)
        {
            try
            {
                return VO2 / BW;
            }
            catch (Exception ex)
            {
                logger.LogError(string.Format("Error: {0}, Stack: {1}", ex.Message, ex.StackTrace));
                return -1;
            }

        }
        public static double  Calculate_METS(double VO2, double BW, ILogger logger)
        {
            try
            {
                return VO2 * 1000 / (3.5 * BW);
            }
            catch (Exception ex)
            {
                logger.LogError(string.Format("Error: {0}, Stack: {1}", ex.Message, ex.StackTrace));
                return -1;
            }

        }
        public static double  Calculate_RER(double VO2, double VCO2, ILogger logger)
        {
            try
            {
                return VCO2 / VO2;
            }
            catch (Exception ex)
            {
                logger.LogError(string.Format("Error: {0}, Stack: {1}", ex.Message, ex.StackTrace));
                return -1;
            }

        }
       
        public static double Calculate_VO2pred(double BW, int W, ILogger logger)
        {
            try
            {
                return 5.8 * BW + 151 + 10.3 * W;
                //return 2 * W + 3.5 * BW; Второй способ
            }
            catch (Exception ex)
            {
                logger.LogError(string.Format("Error: {0}, Stack: {1}", ex.Message, ex.StackTrace));
                return -1;
            }


        }
        public static double  Calculate_O2Pulse(double VO2, int HR, ILogger logger)
        {
            try
            {
                return VO2/HR;
                
            }
            catch (Exception ex)
            {
                logger.LogError(string.Format("Error: {0}, Stack: {1}", ex.Message, ex.StackTrace));
                return -1;
            }


        }
    }
}
