using Microsoft.Extensions.Logging;
using Respiratory_Analysis_CPET.AnyModelsForProjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Respiratory_Analysis_CPET._1_step
{
    /// <summary>
    /// Тест на минутную вентиляцию легких (Идет после теста покоя и на форсированный выдох). Источник: Папка с книгами по спирометрии 
    /// </summary>
    public class MVV
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MVV"/> class.
        /// </summary>
        /// <param name="mVV">МВЛ - Максимальная вентиляция лёгких (предел вентиляции) - это максимальный объём воздуха, проходящий через лёгкие при форсированном дыхании за одну минуту</param>
        /// <param name="bF_MVV">Частота дыханий при МВЛ</param>
        public MVV(double mVV, double bF_MVV)
        {
            Mvv = new Parameter("MVV",new ParameterValue(Math.Round(mVV,2),"L"));
            BF_MVV = new Parameter("BF\\MVV", new ParameterValue(Math.Round(bF_MVV,2),"BR/L"));
        }
        public Parameter Mvv { get; private set; }
        public Parameter BF_MVV { get; private set; }
        public IEnumerable<Parameter> GetParameters()
        {
            return new Parameter[] { Mvv,BF_MVV };
        }
        public static MVV GetData(List<double> listVT, List<double> listBF, ILogger logger)
        {
            double currentMVV;
            double BF_MVV = 0;
            double MVV = 0;
            try
            {
                for (int i = 0; i < listVT.Count; i++)
                {
                    currentMVV = listVT[i] * listBF[i];
                    if (currentMVV > MVV)
                    {
                        MVV = currentMVV;
                        BF_MVV = listBF[i];
                    }
                }
            }
            catch (Exception ex)
            {
                BF_MVV = -1;
                MVV = -1;
                logger.LogError(string.Format("Error: {0}, Stack: {1}", ex.Message, ex.StackTrace));
            }
            return new MVV(MVV, BF_MVV);
        }
    }
}

