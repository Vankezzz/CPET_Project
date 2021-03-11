using Microsoft.Extensions.Logging;
using Respiratory_Analysis_CPET._1_step;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static Respiratory_Analysis_CPET.PulmonaryAnalysis;

namespace Respiratory_Analysis_CPET
{
    /// <summary>
    /// StepOfAnalysis.TestInRest - Данный флаг надо использовать, когда в тестирование включен тест покоя на 1 этапе (идет он раньше теста TestInForcedExpiration и TestInMVV)
    /// StepOfAnalysis.TestInForcedExpiration - Данный флаг надо использовать, когда в тестирование включен тест на форсирующий выдох на 1 этапе ( идет он после теста TestInForcedExpiration, но перед TestInMVV
    /// StepOfAnalysis.TestInMVV - Данный флаг надо использовать, когда в тестирование включен тест на максимальную минутную вентиляцию легких на 1 этапе (идет он после теста TestInRest,TestInForcedExpiration)
    /// </summary>
    public  class TestInRest
    {
        readonly List<List<double>> expirationList = new List<List<double>>();
        readonly List<List<double>> inspirationList = new List<List<double>>();
        readonly List<Basics> Basics_Rest = new List<Basics>();
        readonly List<Basics> Basics_ForcedExp = new List<Basics>();
        readonly List<Basics> Basics_MVV = new List<Basics>();
        ILogger _logger;
        double _sampleTime;
        /// <summary>
        /// (Не надо им пользоваться)Данный метод записывает в режиме реального времени данные в буфер (формирует листы с BasicsCalculations), чтобы по окончании теста провести анализ
        /// </summary>
        /// <param name="basics">Базовые вычисления</param>
        /// <param name="logger">The logger.</param>
        public void WriteData(List<double> expList, List<double> insList, double sampleTime, Basics basics, StepOfAnalysis flag, ILogger logger)
        {
            _logger = logger;
            _sampleTime = sampleTime;
            switch (flag)
            {
                case StepOfAnalysis.TestInRest:
                    try
                    {
                        Basics_Rest.Add(basics);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(string.Format("Error: {0}, Stack: {1}", ex.Message, ex.StackTrace));
                    }
                    break;
                case StepOfAnalysis.TestInForcedExpiration:
                    try
                    {
                        expirationList.Add(expList);
                        inspirationList.Add(insList);
                        Basics_ForcedExp.Add(basics);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(string.Format("Error: {0}, Stack: {1}", ex.Message, ex.StackTrace));
                    }
                    break;
                case StepOfAnalysis.TestInMVV:
                    try
                    {
                        Basics_MVV.Add(basics);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(string.Format("Error: {0}, Stack: {1}", ex.Message, ex.StackTrace));
                    }
                    break;
                default:
                    break;

            }
        }
        /// <summary>
        /// Данными методом нужно воспользоваться после завершения тестов
        /// </summary>
        /// <returns></returns>
        public  Rest GetDataRest()
        {
            return Rest.GetData(Basics_Rest, _logger);
        }
        public  MVV GetDataMVV()
        {
            return MVV.GetData(Basics_MVV.Select(i => i.VT.ParameterValue.Value).ToList(), Basics_MVV.Select(i => i.BF.ParameterValue.Value).ToList(), _logger);
        }
        /// <summary>
        /// Данным методом нужно воспользоваться после завершения теста, также введите значение VC, которое получается в процессе теста покоя TestInRest
        /// </summary>
        /// <param name="vC">The v c.</param>
        /// <returns></returns>
        public  ForcedExpiration GetDataForcedExpiration(double vC)
        {
            var index = Basics_ForcedExp.Select(i => i.VT).ToList().IndexOf(Basics_ForcedExp.Select(i => i.VT).ToList().Max());//screamer

            return ForcedExpiration.GetData(inspirationList[index], expirationList[index], vC, _sampleTime, _logger);
        }
    }
}
