using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Extensions.Logging;
using Respiratory_Analysis_CPET.AnyModelsForProjects;

namespace Respiratory_Analysis_CPET._1_step
{
    public class Rest
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RestValue"/> class.
        /// </summary>
        /// <param name="vC">ЖЕЛ - Жизненная емкость легких</param>
        /// <param name="iRV">РОвд - Резервный объем вдоха — это тот объём воздуха, который можно вдохнуть при максимальном вдохе после обычного вдоха</param>
        /// <param name="eRV">РОвыд - Резервный объём выдоха (резервный воздух) - это тот объём воздуха, который можно выдохнуть при максимальном выдохе после обычного выдоха</param>
        /// <param name="bF">Частота дыхания  средняя при спокойном дыхании</param>
        /// <param name="vT">Средний дыхательный обьем за тест</param>
        public Rest(double vC, double iRV, double eRV, double bF, double vT)
        {
            VC = new Parameter("VC", new ParameterValue(Math.Round(vC,2),"L"));
            IRV = new Parameter("IRV", new ParameterValue(Math.Round(iRV,2), "L"));
            ERV = new Parameter("ERV ", new ParameterValue(Math.Round(eRV,2), "L"));
            BF = new Parameter("BF", new ParameterValue(Math.Round(bF,2), "BR"));
            VT = new Parameter("VT", new ParameterValue(Math.Round(vT,2), "L"));
        }
        public IEnumerable<Parameter> GetParameters()
        {
            return new Parameter[] { VC,IRV,ERV,BF,VT };
        }
        public Parameter VC { get; private set; }
        public Parameter IRV { get; private set; }
        public Parameter ERV { get; private set; }
        public Parameter BF { get; private set; }
        public Parameter VT { get; private set; }

        static ILogger _logger;
        public static Rest GetData(List<Basics> basics, ILogger logger)
        {
            _logger=logger;
            DefenitionVC(basics.Select(i=>i.VT.ParameterValue.Value).ToList(), out double VC);
            var VI = basics[basics.Select(i => i.VT.ParameterValue.Value).ToList().IndexOf(VC)].VI;
            var VTavr = basics.Select(i => i.VT.ParameterValue.Value).ToList().Average();
            var BFavr = basics.Select(i => i.BF.ParameterValue.Value).ToList().Average();
            basics.RemoveAt(basics.Select(i => i.VT.ParameterValue.Value).ToList().IndexOf(VC));//убрали VC значение для подсчета среднего VT
            DefenitionIRV(VI.ParameterValue.Value,VTavr , out double IRV);
            DefenitionERV(VC, VTavr, out double ERV);
            return new Rest(VC, IRV, ERV, BFavr, VTavr);
        }
        static void DefenitionVC(List<double> volumeListExp,out double VC)
        {
            try
            {
                VC =volumeListExp.Max();
            }
            catch (Exception ex)
            {
                VC = -1;
                _logger.LogError(string.Format("Error: {0}, Stack: {1}", ex.Message, ex.StackTrace));
            }
        }
        static void DefenitionIRV(double VI, double VT, out double IRV)
        {
            try
            {
                IRV = VI - VT;
            }
            catch (Exception ex)
            {
                IRV = -1;
                _logger.LogError(string.Format("Error: {0}, Stack: {1}", ex.Message, ex.StackTrace));
            }
        }
        static void DefenitionERV(double VC,double VT, out double ERV)
        {
            try
            {
                ERV = VC - VT;
            }
            catch (Exception ex)
            {
                ERV = -1;
                _logger.LogError(string.Format("Error: {0}, Stack: {1}", ex.Message, ex.StackTrace));
            }
        }
    }
}
