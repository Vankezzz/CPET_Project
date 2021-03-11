using Microsoft.Extensions.Logging;
using Respiratory_Analysis_CPET.AnyModelsForProjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Respiratory_Analysis_CPET
{
    public class Volume
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Volume"/> class.
        /// </summary>
        /// <param name="vE">Минутная вентиляция легких STPD. Вычисляется: VE=𝐵𝐹∗𝑉𝑇*C2 , C2 - это коэффициент перевода из BTPS в STPD. Источник:Principles of Exercise Testing and Interpretation [Формулы и текст]: Wasserman K, Hansen JE, Sue DY, 5th Ed.  Lippincott Williams  Wilkins – Америка, 2012. – 711стр. ISBN-13: 978-1-60913-899-8 </param>
        /// <param name="vA">VA(BTPS) - Альвеолярная вентиляция. Вычисляется: 𝑉𝐴=𝑉𝐸−𝐵𝐹∗𝑉𝐷 Источник: Instruction for innovision by Innocor. Breath-by-Breath method [Формулы и текст]: компания «Innovission». – Германия, 2013. – 7стр.</param>
        /// <param name="vD">Мертвое пространство. Вычисляется: Алгоритм Pre-Interface Expirate (PIE) - просто взял половину от FETCO2 и просчитал весь обьем,который выдохнули по достижении значения концентрации FETCO2/2 . Источник:VD.docx</param>
        /// <param name="vO2_ET">Потребление кислорода. Расчитывается по преобразованию Эшенбахера: VO2=VE*(FIO2-FEO2 )*((1-FECO2+FICO2))/((1-FIO2+FEO2)), где FIO2 и FIСO2 - средние значения концентрации на вдохе, FEO2 и FEСO2 - средние значения концентрации на выдохе Источник: Дипломная работа раздел 1.2.2</param>
        /// <param name="vCO2_ET">Выделение диоксида углерода. Расчитывается по преобразованию Эшенбахера: VCO2=VE*FECO2. Источник: Дипломная работа раздел 1.2.2 </param>
        public Volume(double vE, double vA, double vD, double vO2_ET, double vCO2_ET)
        {
            VE = new Parameter("VE", new ParameterValue(Math.Round(vE, 2),"L/M"));
            VA = new Parameter("VA", new ParameterValue(Math.Round(vA, 2), "L/M"));
            VD = new Parameter("VD", new ParameterValue(Math.Round(vD, 2), "L"));
            VO2_ET = new Parameter("VO2", new ParameterValue(Math.Round(vO2_ET, 2), "L/M"));
            VCO2_ET = new Parameter("VCO2", new ParameterValue(Math.Round(vCO2_ET, 2), "L/M"));
        }
        public IEnumerable<Parameter> GetParameters()
        {
            return new Parameter[] { VE, VA, VD, VO2_ET, VCO2_ET };
        }
        public Parameter VE { get; private set; }
        public Parameter VA { get; private set; }
        public Parameter VD { get; private set; }
        public Parameter VO2_ET { get; private set; }
        public Parameter VCO2_ET { get; private set; }
        public static Volume GetData(List<double> flowexp, List<double> fio2data, List<double> feo2data, List<double> feco2data,double sampletime,Basics bC,int N, ILogger logger)
        {
            var ve = Calculate_VE(bC.VT.ParameterValue.Value, bC.BF.ParameterValue.Value, logger);
            var vd = Calculate_VD(flowexp,feco2data, sampletime,N, logger);
            var va = Calculate_VA(ve, bC.BF.ParameterValue.Value, vd, logger);
            Eschenbacher_transformation(ve, fio2data, feo2data, feco2data, out var vco2_et, out var vo2_et, logger);
            return new Volume(ve, va, vd, vo2_et, vco2_et);
        }
        /// <summary>
        /// Put C2_fromBTPStoSTPD() instead of C
        /// </summary>
        /// <param name="_Flowexp"></param>
        /// <param name="SampleTime"></param>
        /// <param name="C"></param>
        /// <param name="logger"></param>
        /// <returns></returns>
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
        /// <summary>
        /// Put C1_fromATPtoSTPD() instead of C
        /// </summary>
        /// <param name="_Flowins"></param>
        /// <param name="SampleTime"></param>
        /// <param name="C"></param>
        /// <param name="logger"></param>
        /// <returns></returns>
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
        /// <summary>
        /// VT - Tidal Volume [liters]
        /// BF - Breathing frequency [Breathes per  minute]
        /// </summary>
        /// <param name="VT"></param>
        /// <param name="BF"></param>
        ///<param name="logger"></param>
        /// <returns></returns>
        public static double Calculate_VE(double VT, double BF, ILogger logger)
        {
            try
            {
                return VT * BF;
                // дополнительный метод вычисления return (_VCO2 * 863) / (_PaCO2 * (1 - _VD / _VT));
            }
            catch (Exception ex)
            {
                logger.LogError(string.Format("Error: {0}, Stack: {1}", ex.Message, ex.StackTrace));
                return -1;
            }

        }
        /// <summary>
        /// VE - Minute ventilation VE = VT*BR [liters/minute]
        /// BF - Breathing frequency [Breathes per  minute]
        /// VD - Death volume [liters]
        /// </summary>
        /// <param name="VE"></param>
        /// <param name="BF"></param>
        /// <param name="VD"></param>
        /// <param name="logger"></param>
        /// <returns></returns>
        public static double Calculate_VA(double VE, double BF, double VD, ILogger logger)
        {
            try
            {
                return VE - BF * VD;
                //2 способ return _VO2 / (FIO2DATA.Average() - FAO2);
                //3 способ return _VE - _BF * (_VD + _VDm);
            }
            catch (Exception ex)
            {
                logger.LogError(string.Format("Error: {0}, Stack: {1}", ex.Message, ex.StackTrace));
                return -1;
            }



        }
        //public static List<double> Calculate_derivateCO2exp(List<double> FECO2, double SampleTime, ILogger logger)
        //{
        //    try
        //    {
        //        List<double> dCO2exp = new List<double> { };
        //        for (int i = 1; i < FECO2.Count; i++)
        //        {
        //            dCO2exp.Add(Derivate.CommonDifference(FECO2[i - 1], FECO2[i], SampleTime));
        //        }
        //        return dCO2exp;
        //    }
        //    catch (Exception ex)
        //    {
        //        logger.LogError(string.Format("Error: {0}, Stack: {1}", ex.Message, ex.StackTrace));
        //        return new List<double>(FECO2.Count);
        //    }
        //}

        public static void Eschenbacher_transformation(double VE, List<double> FIO2DATA, List<double> FEO2DATA, List<double> FECO2DATA, out double VCO2_ET, out double VO2_ET, ILogger logger)
        {
            try
            {
                double kE;
                VCO2_ET = 0;
                VO2_ET = 0;
                if (FIO2DATA.Count!= 0)
                {
                    kE = (1 - FECO2DATA.Average() * 0.01) / (1 - FIO2DATA.Average() * 0.01 + FEO2DATA.Average() * 0.01);
                    VCO2_ET = VE * FECO2DATA.Average() * 0.01;
                    VO2_ET = VE * kE * (FIO2DATA.Average() * 0.01 - FEO2DATA.Average() * 0.01);
                }
            }
            catch (Exception ex)
            {
                logger.LogError(string.Format("Error: {0}, Stack: {1}", ex.Message, ex.StackTrace));
                VCO2_ET = 0;
                VO2_ET = 0;

            }
        }

        public static double Calculate_VD(List<double> flowexp, List<double> cO2exp,double sampleTime,int bufferFET, ILogger logger)
        {
            int index=0,flag = 0;
            List<double> flowexp_buffer = new List<double>();
            List<double> FETCO2_buffer = new List<double>();
            try
            {
                byte iterator = 0;
                for (int i = cO2exp.Count - bufferFET + 1; i < cO2exp.Count; i++)
                {
                    if (cO2exp[i] < 2)
                    {
                        while (cO2exp[i] < 2)
                        {
                            iterator++;
                            FETCO2_buffer.Add(cO2exp[(cO2exp.Count - bufferFET + 1) - iterator]);
                        }
                    }
                    else
                    {
                        FETCO2_buffer.Add(cO2exp[i]);
                    }
                }
                for (int i = 0; i < cO2exp.Count; i++)
                {
                    if (flag == 0 && cO2exp[i] > (FETCO2_buffer.Average() / 2))
                    {
                        index = i;
                    }
                }
                for (int i = 0; i <= index; i++)
                {
                    flowexp_buffer.Add(flowexp[i]);
                }
                return Integral.Calculate_Volume(flowexp_buffer, sampleTime, PulmonaryAnalysis.C2_fromBTPStoSTPD());
            }
            catch (Exception ex)
            {
                logger.LogError(string.Format("Error: {0}, Stack: {1}", ex.Message, ex.StackTrace));
                return -1;

            }
            
        }
        //public static double VD_Calcilations(List<double> FECO2DATAY, List<double> FICO2DATA, List<double> flowexp, double SampleTime, ILogger logger)
        //{
        //    try
        //    {
        //        double VD = 0;
        //        int index = 0;
        //        //double A1, B1 = 0;//для вспомогательной регрессионной линии
        //        List<double> dCO2exp = Calculate_derivateCO2exp(FECO2DATAY, SampleTime, logger);
        //        List<double> time = new List<double>() { };//Буфер для основной кривой FECO2DATA, которая будет откладываться по оси Х
        //        List<double> FECO2DATA = new List<double>() { };// Буфер для данных по которым пойдет расчет - основная кривая по оси Y

        //        double _average_dCO2exp = (dCO2exp.Average() + dCO2exp.Max()) / 2;

        //        //Добавляем 2 последних значения по концентрации CO2 на вдохе и их время соответственно
        //        FECO2DATA.Add(FICO2DATA[FICO2DATA.Count- 2]);
        //        FECO2DATA.Add(FICO2DATA[FICO2DATA.Count- 1]);

        //        for (int i = 0; i < FECO2DATAY.Count+ 2; i++)
        //        {
        //            time.Add(i * SampleTime);
        //        }


        //        int trigger = 0;
        //        //Инициализация листов данных для вторичной кривой - регрессионной - после половины подьема в виде сигмоиды
        //        List<double> LMSCO2DATA_Y = new List<double> { };
        //        List<double> LMSCO2DATA_X = new List<double> { };
        //        //Инициализация листов данных для третьей кривой
        //        List<double> LMSCO2DATA_Y1 = new List<double> { };
        //        List<double> LMSCO2DATA_X1 = new List<double> { };

        //        List<double> LSM_DATAX = new List<double> { };
        //        List<double> LSM_DATAY = new List<double> { };


        //        for (int i = 0; i < FECO2DATAY.Count- 2; i++)
        //        {
        //            FECO2DATA.Add(FECO2DATAY[i]);//дозаполнение листа основной кривой данными концентрации на выдохе
        //            if (trigger == 0)
        //            {
        //                if (FECO2DATAY[i + 1] != 0)
        //                {
        //                    LMSCO2DATA_Y1.Add(FECO2DATAY[i]);
        //                    LMSCO2DATA_X1.Add(time[i]);
        //                }
        //            }
        //            //Срабатывание тригера на увеличение производной - примерно чуть выше середины сигмоиды
        //            if (dCO2exp[i] > _average_dCO2exp && trigger == 0)
        //            {
        //                trigger = 1;
        //            }
        //            //Сбор данных для вторичнойкривой после срабатывания тригера на производную
        //            if (dCO2exp[i] < _average_dCO2exp && trigger == 1)
        //            {
        //                LMSCO2DATA_Y.Add(FECO2DATAY[i - 1]);
        //                LMSCO2DATA_X.Add(time[i]);
        //            }

        //        }

        //        //Определение регрессионой функции
        //        if (LMSCO2DATA_Y.Count!= 0)
        //        {
        //            LineRegress.LSM(LMSCO2DATA_X, LMSCO2DATA_Y, out double A, out double B);
        //            //LineRegress.LSM(LMSCO2DATA_X1, LMSCO2DATA_Y1, out A1, out B1);//Вспомогательная регрессионная линия
        //            LineRegress.LSM_DATAXY(A, B, 0, 4, SampleTime, out LSM_DATAX, out LSM_DATAY);
        //            index = LineRegress.FindSquare_LineRegress(time, FECO2DATA, A, B, SampleTime);
        //        }
        //        if (index - 1 < 0)
        //        {
        //            return 0;
        //        }
        //        else
        //        {
        //            for (int i = 1; i < index; i++)
        //                VD += Integral.Trap(flowexp[i - 1], flowexp[i], SampleTime);
        //            return VD;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        logger.LogError(string.Format("Error: {0}, Stack: {1}", ex.Message, ex.StackTrace));
        //        return -1;
        //    }

        //}
        //public double VD(double VT, double PaCO2, double PECO2, double VDm)
        //{
        //    
        //    return (VT * (PaCO2 - PECO2) / PaCO2) - VDm;
        //}

    }
}
