using Microsoft.Extensions.Logging;
using Respiratory_Analysis_CPET.AnyModelsForProjects;
using System;
using System.Collections.Generic;

namespace Respiratory_Analysis_CPET
{
    public class Basics
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BasicsCalculations"/> class.
        /// </summary>
        /// <param name="bF">Частота дыхательных движений. Вычисляется: 60 / (countInBreath * sampletime), где countInBreath = колличество семплов, пришедших в течении 1 дыхательного цикла, sampletime - период дискретизации</param>
        /// <param name="vT">Tidal volume - Жизненый обьем [Л.] — это объем воздуха , выдыхаемого во время выдоха STPD.Вычисляется: 𝑉𝑇=интеграл методом трапеций для 𝑉𝑒𝑥𝑝(𝑡) - выдохнутый поток через бесконечно малый интервал времени dt в момент времени t и умножается на коэффициент перевода из состояния BTPS в STPD. Источник:Principles of Exercise Testing and Interpretation [Формулы и текст]: Wasserman K, Hansen JE, Sue DY, 5th Ed.  Lippincott Williams  Wilkins – Америка, 2012. – 711стр. ISBN-13: 978-1-60913-899-8 (</param>
        /// <param name="vI">Inspiration volume - это объем воздуха , вдыхаемого во время вдоха STPD.Вычисляется: 𝑉𝑇=интеграл методом трапеций для 𝑉ins(𝑡) - вдохнутый поток через бесконечно малый интервал времени dt в момент времени t и умножается на коэффициент перевода из состояния ATP в STPD</param>
        public Basics(double bF, double vT, double vI)
        {
            BF = new Parameter("BF", new ParameterValue(Math.Round(bF,2),"BR"));
            VT = new Parameter("VT", new ParameterValue(Math.Round(vT,2),"L"));
            VI = new Parameter("VT", new ParameterValue(Math.Round(vI,2), "L"));
        }

        public Parameter BF { get; private set; }
        public Parameter VT { get; private set; }
        public Parameter VI { get; private set; }
        public IEnumerable<Parameter> GetParameters()
        {
            return new Parameter[] { BF,VI,VT };
        }
        public static Basics GetData(List<double> flowexp, List<double> flowins,double C1,double C2,double countInBreath, double sampletime, ILogger logger)
        {

            return new Basics(Calculate_BF(countInBreath, sampletime,logger),Calculate_VT(flowexp,sampletime,C2,logger), Calculate_VI(flowins,sampletime,C1,logger));
        }
        public static double Calculate_BF(double countInBreath, double sampletime, ILogger logger) 
        {
            try
            {
                return 60 / (countInBreath * sampletime);
            }
            catch (Exception ex)
            {
                logger.LogError(string.Format("Error: {0}, Stack: {1}", ex.Message, ex.StackTrace));
                return -1;
            }
        }
        public static double Calculate_VT(List<double> _Flowexp, double SampleTime, double C, ILogger logger)
        {
            try
            {
                return Integral.Calculate_Volume(_Flowexp, SampleTime, C);
            }
            catch (Exception ex)
            {
                logger.LogError(string.Format("Error: {0}, Stack: {1}", ex.Message, ex.StackTrace));
                return -1;
            }

        }
        public static double Calculate_VI(List<double> _Flowins, double SampleTime, double C, ILogger logger)
        {
            try
            {
                return Integral.Calculate_Volume(_Flowins, SampleTime, C);
            }
            catch (Exception ex)
            {
                logger.LogError(string.Format("Error: {0}, Stack: {1}", ex.Message, ex.StackTrace));
                return -1;
            }

        }
    }
}
