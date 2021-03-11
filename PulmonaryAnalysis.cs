using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Respiratory_Analysis_CPET._1_step;
using Respiratory_Analysis_CPET.AnyModelsForProjects;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace Respiratory_Analysis_CPET
{
    /// <summary>
    /// Главное тело вычислений респираторных данных
    /// </summary>
    public class PulmonaryAnalysis
    {
        private double previous_dataflow;//предыдущие значение dataFlow для определения фазы End inspiration,End Expiration (переход через линию 0)
        private readonly double _flowThreshold = 0.3;//_flowThreshold - порог срабатывания в 0.3 Л/С (все дыхание, что в диапозоне от -0.3 до 0.3 Л/С в помехах и мы им пренебрегаем)
        private readonly Filtration _filter;
        private readonly SettingsPulmonaryAnalysis _settingsPulmonary_Analysis;
        public event EventHandler<IEnumerable<Parameter>> BreathEnded;
        public event EventHandler QRSRequested;
        public event EventHandler<IEnumerable<Parameter>> DataCalculated;
        private readonly TestInRest _testInRest;
        private readonly ILogger Logger;
        private readonly string _nameFileDataWorkLoad = @"\DataBreath.txt";
        private readonly string _nameFileDataRest = @"\Rest.txt";
        Parameter currentQRS;

        /// <summary>
        /// Хранятся данные c цикламами дыхания Breath и с периодичностью, которая задается в методе Instance (rangeForDataBreathWriter), записывается в файл
        /// </summary>
        private readonly List<BreathInWorkLoad> _breathInWorkLoad = new List<BreathInWorkLoad>();

        private readonly List<SampleBreathing> spiroData = new List<SampleBreathing>() { };//Массив всех точек потока и концентраций для 1 цикла дыхания,обновляется с каждым новым циклом дыхания
        private Rest _rest;
        private MVV _mVV;
        private ForcedExpiration _forcedExpiration;


        private List<int> indexOfPhase = new List<int>(4) { 0, 0, 0, 0 };//indexOfPhase - это массив индикаторов фаз из 4 точек :элемент с 0 индексом - Inspiration,элемент с 1 индексом - End inspiration, элемент с 2 индексом - Expiration, элемент с 3 индексом - End Expiration


        private List<SampleBreathing> Expiration;//Лист точек потока и концентрация на выдохе (часть spiroData)
        private List<SampleBreathing> Inspiration;//Лист точек потока и концентрация на вдохе (часть spiroData)


        public enum StepOfAnalysis : byte
        {
            TestInRest = 1,
            TestInForcedExpiration = 2,
            TestInMVV = 3,
            EndStep1 = 4,
            TestInWorkload = 5,
            EndStep2 = 6
        }

        public PulmonaryAnalysis(SettingsPulmonaryAnalysis stp)
        {
            _settingsPulmonary_Analysis = stp;
            _filter = new Filtration(_settingsPulmonary_Analysis.SizeOfPack_filter, _settingsPulmonary_Analysis.LimitOfWindow_filter);
            _filter.DataFiltred += DetectPhaseOfBreath;
            _filter.DataFiltred += DetectPhaseOfBreath;
            _testInRest = new TestInRest();
            Logger = _settingsPulmonary_Analysis.Logger;
            if (!Directory.Exists(_settingsPulmonary_Analysis.PathDirectoryForDataBreath)) Directory.CreateDirectory(_settingsPulmonary_Analysis.PathDirectoryForDataBreath);
        }



        /// <summary>
        /// Основной метод, в который мы посылаем наши данные с периодом дискретизации, который задается в в методе Instance (sampleTime). В данном методе реализована логика разделения по данным Потока на фазы дыхания: 1. Вдох  (после преодоления порога в 0.3 л/с  и до первого пересечения с нулевой линией). Почему был выбран порог в 0.3 л.с-  при попадании в диапозон [-0.3;0.3] л/с идет резкий перепад значений концентраций, также нам надо избавиться от помех [-0.3;0.3] л/с. 2. Фаза конца вдоха - это первый переход через нулевую линию. 3. Фаза выдоха - от -0.3 л.с и до первого перехода через нулевую линию. 4. Фаза конца вдоха - переход через нулевую линию
        /// </summary>
        /// <param name="dataFlow">текущее значение потока в л/с</param>
        /// <param name="dataO2">текущее значение О2 в Об.% (обьемные проценты)</param>
        /// <param name="dataCO2">текущее значение СО2 в Об.% (обьемные проценты).</param>
        /// <param name="flag"> Указывает на какой стадии сейчас программа</param>
        public void AnalyzeSpiroData(double dataFlow, double dataO2, double dataCO2, StepOfAnalysis flag)
        {
            _filter.FilterData(new SampleBreathing(dataFlow, dataO2, dataCO2), flag);
        }
        private void DetectPhaseOfBreath(SampleBreathing sampleBreathing, StepOfAnalysis flag, DateTime dateTime)
        {
            try
            {
                spiroData.Add(sampleBreathing);
                switch (indexOfPhase.IndexOf(0))
                {
                    case 0:
                        if (sampleBreathing.Flow.ParameterValue.Value >= _flowThreshold)//Inspiration flow
                        {
                            try
                            {
                                indexOfPhase[0] = spiroData.Count;
                                Logger.LogDebug($"Phase Inspiration dataFlow : {sampleBreathing.Flow.ParameterValue.Value} - dataO2 : {sampleBreathing.O2} - dataCO2 : {sampleBreathing.CO2}");
                            }
                            catch (Exception ex)
                            {
                                Logger.LogError(string.Format("Error: {0}, Stack: {1}", ex.Message, ex.StackTrace));
                            }

                        }
                        break;
                    case 1:
                        if (sampleBreathing.Flow.ParameterValue.Value <= 0 && previous_dataflow >= 0)//End inspiration
                        {
                            try
                            {
                                indexOfPhase[1] = spiroData.Count;
                                Logger.LogDebug($"Phase End inspiration dataFlow : {sampleBreathing.Flow.ParameterValue.Value} - previous_dataflow : {previous_dataflow} - dataO2 : {sampleBreathing.O2} - dataCO2 : {sampleBreathing.CO2}");
                            }
                            catch (Exception ex)
                            {
                                Logger.LogError(string.Format("Error: {0}, Stack: {1}", ex.Message, ex.StackTrace));
                            }

                        }
                        break;
                    case 2:
                        if (sampleBreathing.Flow.ParameterValue.Value <= -_flowThreshold)//Expiration 
                        {
                            try
                            {
                                indexOfPhase[2] = spiroData.Count;
                                Logger.LogDebug($"Phase Expiration dataFlow : {sampleBreathing.Flow.ParameterValue.Value} - dataO2 : {sampleBreathing.O2} - dataCO2 : {sampleBreathing.CO2}");
                            }
                            catch (Exception ex)
                            {
                                Logger.LogError(string.Format("Error: {0}, Stack: {1}", ex.Message, ex.StackTrace));
                            }
                        }
                        break;
                    case 3:
                        if (sampleBreathing.Flow.ParameterValue.Value >= 0 & previous_dataflow <= 0)//End Expiration 
                        {
                            try
                            {
                                Logger.LogDebug($"Phase End Expiration  dataFlow : {sampleBreathing.Flow.ParameterValue.Value} - previous_dataflow : {previous_dataflow} - dataO2 : {sampleBreathing.O2} - dataCO2 : {sampleBreathing.CO2}");

                                indexOfPhase[3] = spiroData.Count;
                                Calculate_Params_in_EndBreath(flag);
                                spiroData.Clear();
                                indexOfPhase = new List<int>(4) { 0, 0, 0, 0 };
                            }
                            catch (Exception ex)
                            {
                                Logger.LogError(string.Format("Error: {0}, Stack: {1}", ex.Message, ex.StackTrace));
                            }

                        }
                        break;
                    default:
                        break;
                }
                CalculateVolume(sampleBreathing, dateTime);
                previous_dataflow = sampleBreathing.Flow.ParameterValue.Value;
            }
            catch (Exception ex)
            {
                Logger.LogError(string.Format("Error: {0}, Stack: {1}", ex.Message, ex.StackTrace));
            }

        }


        private void Calculate_Params_in_EndBreath(StepOfAnalysis flag)
        {
            QRSRequested?.Invoke(this, EventArgs.Empty);
            Expiration = spiroData.GetRange(indexOfPhase[2], spiroData.Count - indexOfPhase[2]);
            Inspiration = spiroData.GetRange(indexOfPhase[0], indexOfPhase[1] - indexOfPhase[0]);
            var basics = Basics.GetData(Expiration.Select(i => i.Flow.ParameterValue.Value).ToList(), Inspiration.Select(i => i.Flow.ParameterValue.Value).ToList(), C1_fromATPtoSTPD(), C2_fromBTPStoSTPD(), indexOfPhase[3] - indexOfPhase[0], _settingsPulmonary_Analysis.SampleTime, Logger);


            if (flag == StepOfAnalysis.TestInRest || flag == StepOfAnalysis.TestInForcedExpiration || flag == StepOfAnalysis.TestInMVV)
            {
                _testInRest.WriteData(Expiration.Select(i => i.Flow.ParameterValue.Value).ToList(), Inspiration.Select(i => i.Flow.ParameterValue.Value).ToList(), _settingsPulmonary_Analysis.SampleTime, basics, flag, Logger);

            }
            else if (flag == StepOfAnalysis.TestInWorkload)
            {
                try
                {
                    var bufferFET = 10;
                    var volume = Volume.GetData(Expiration.Select(x => x.Flow.ParameterValue.Value).ToList(), Inspiration.Select(x => x.O2.ParameterValue.Value).ToList(), Expiration.Select(x => x.O2.ParameterValue.Value).ToList(), Expiration.Select(x => x.CO2.ParameterValue.Value).ToList(), _settingsPulmonary_Analysis.SampleTime, basics, bufferFET, Logger);

                    var coeff = (Coefficient.Calculate(volume.VE.ParameterValue.Value, volume.VO2_ET.ParameterValue.Value, volume.VCO2_ET.ParameterValue.Value, basics.BF.ParameterValue.Value, _settingsPulmonary_Analysis.Person.Bodyweight, 0, _settingsPulmonary_Analysis.Vdm, Logger));
                    var pressure = (Pressure.GetData(Inspiration.Select(x => x.O2.ParameterValue.Value).ToList(), Expiration.Select(x => x.CO2.ParameterValue.Value).ToList(), Expiration.Select(x => x.O2.ParameterValue.Value).ToList(), volume.VO2_ET.ParameterValue.Value, volume.VCO2_ET.ParameterValue.Value, volume.VA.ParameterValue.Value, coeff.RER.ParameterValue.Value, bufferFET, Logger));

                    _breathInWorkLoad.Add(new BreathInWorkLoad(Expiration, Inspiration, basics, volume, coeff, pressure));

                    IEnumerable<Parameter> parameters = new List<Parameter>();
                    foreach (var parameter in basics.GetParameters())
                    {
                        (parameters as List<Parameter>).Add(parameter);
                    }
                    foreach (var parameter in volume.GetParameters())
                    {
                        (parameters as List<Parameter>).Add(parameter);
                    }
                    foreach (var parameter in coeff.GetParameters())
                    {
                        (parameters as List<Parameter>).Add(parameter);
                    }

                    foreach (var parameter in pressure.GetParameters())
                    {
                        (parameters as List<Parameter>).Add(parameter);
                    }
                    (parameters as List<Parameter>).Add(currentQRS);
                    RaiseBreathEnd(parameters);

                    if (_breathInWorkLoad.Count == _settingsPulmonary_Analysis.RangeForDataBreathWriter)
                    {
                        WriterDataBreath(flag);
                    }
                }
                catch (Exception ex)
                {
                    Logger.LogError(string.Format("Error: {0}, Stack: {1}", ex.Message, ex.StackTrace));
                }
            }
        }
        protected void RaiseBreathEnd(IEnumerable<Parameter> parameters)
        {
            BreathEnded?.Invoke(this, parameters);
        }

        private void CalculateVolume(SampleBreathing sampleBreathing, DateTime dateTime)
        {
            var currentVolume = new Parameter("CurrentVolume", new ParameterValue(-1, "L"));
            if ((indexOfPhase.IndexOf(0) == 0 || indexOfPhase.IndexOf(0) == 1) && spiroData.Count > 0)
            {
                currentVolume.ParameterValue.Value = Integral.Calculate_Volume(spiroData.Select(i => i.Flow.ParameterValue.Value).ToList(), _settingsPulmonary_Analysis.SampleTime, C1_fromATPtoSTPD());
            }
            else if (indexOfPhase.IndexOf(0) == 2)
            {
                currentVolume.ParameterValue.Value = Integral.Calculate_Volume(spiroData.GetRange(indexOfPhase[1], spiroData.Count - indexOfPhase[1]).Select(i => i.Flow.ParameterValue.Value).ToList(), _settingsPulmonary_Analysis.SampleTime, C2_fromBTPStoSTPD());
            }
            else if (indexOfPhase.IndexOf(0) == 0 && spiroData.Count == 0)
            {

                currentVolume.ParameterValue.Value = Integral.Calculate_Volume(Expiration.Select(i => i.Flow.ParameterValue.Value).ToList(), _settingsPulmonary_Analysis.SampleTime, C2_fromBTPStoSTPD());
            }
            else currentVolume.ParameterValue.Value = -99;

            IEnumerable<Parameter> parameters = new List<Parameter>();
            foreach (var parameter in sampleBreathing.GetParameters())
            {
                (parameters as List<Parameter>).Add(parameter);
            }
            (parameters as List<Parameter>).Add(currentVolume);
            (parameters as List<Parameter>).Add(new Parameter("Time", new ParameterValue((double)dateTime.Second, "s")));
            RaiseDataCalculated(parameters);

        }
        protected void RaiseDataCalculated(IEnumerable<Parameter> parameters)
        {
            DataCalculated?.Invoke(this, parameters);
        }
        public Parameter SetQRS(List<double> ListQRS)
        {
            try
            {
                double qrs = 0;
                for (int i = 1; i < ListQRS.Count; i++)
                {
                    qrs += 60000/(ListQRS[i] - ListQRS[i-1]);//60*1000
                }
                qrs /= (ListQRS.Count-1);
                currentQRS = new Parameter("QRS", new ParameterValue(Math.Round(qrs, 2), "beat/min"));
                return currentQRS;

            }
            catch (Exception ex)
            {
                Logger.LogError(string.Format("Error: {0}, Stack: {1}", ex.Message, ex.StackTrace));
                return new Parameter("QRS", new ParameterValue(-1, "beat/min"));
            }

        }

        public IEnumerable<Parameter> EndStep(StepOfAnalysis flag)
        {
            try
            {
                switch (flag)
                {
                    case StepOfAnalysis.TestInRest:
                        _rest = _testInRest.GetDataRest();
                        WriterDataBreath(flag);
                        return _rest.GetParameters();
                    case StepOfAnalysis.TestInForcedExpiration:
                        if (_rest == null)
                        {
                            _forcedExpiration = _testInRest.GetDataForcedExpiration(0);
                        }
                        else _forcedExpiration = _testInRest.GetDataForcedExpiration(_rest.VC.ParameterValue.Value);
                        WriterDataBreath(flag);
                        return _forcedExpiration.GetParameters();
                    case StepOfAnalysis.TestInMVV:
                        _mVV = _testInRest.GetDataMVV();
                        WriterDataBreath(flag);
                        return _mVV.GetParameters();
                    case StepOfAnalysis.EndStep2:
                        WriterDataBreath(flag);
                        IEnumerable<Parameter> parameters = new List<Parameter>();
                        (parameters as List<Parameter>).Add(new Parameter("none", new ParameterValue(0, "none")));
                        return parameters;
                    default:
                        return null;
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(string.Format("Error: {0}, Stack: {1}", ex.Message, ex.StackTrace));
                return null;
            }

        }


        private async Task WriterDataBreath(StepOfAnalysis flag)
        {
            try
            {
                switch (flag)
                {
                    case StepOfAnalysis.TestInRest:
                        await WriteDataInJSON(Path.Combine(_settingsPulmonary_Analysis.PathDirectoryForDataBreath, _nameFileDataRest), _rest);
                        GetTestData(spiroData);
                        _breathInWorkLoad.Clear();
                        Logger.LogInformation("data {Breath} written successfully", typeof(Rest).Name);
                        break;
                    case StepOfAnalysis.TestInForcedExpiration:
                        await WriteDataInJSON(Path.Combine(_settingsPulmonary_Analysis.PathDirectoryForDataBreath, _nameFileDataRest), _forcedExpiration);
                        GetTestData(spiroData);
                        _breathInWorkLoad.Clear();
                        Logger.LogInformation("data {Breath} written successfully", typeof(ForcedExpiration).Name);
                        break;
                    case StepOfAnalysis.TestInMVV:
                        await WriteDataInJSON(Path.Combine(_settingsPulmonary_Analysis.PathDirectoryForDataBreath, _nameFileDataRest), _mVV);
                        GetTestData(spiroData);
                        _breathInWorkLoad.Clear();
                        Logger.LogInformation("data {Breath} written successfully", typeof(MVV).Name);
                        break;
                    case StepOfAnalysis.TestInWorkload:
                        await WriteDataInJSON(Path.Combine(_settingsPulmonary_Analysis.PathDirectoryForDataBreath, _nameFileDataWorkLoad), _breathInWorkLoad);
                        GetTestData(spiroData);
                        _breathInWorkLoad.Clear();
                        Logger.LogInformation("data {Breath}={_rangeForDataBreathWriter} written successfully", typeof(BreathInWorkLoad).Name, _settingsPulmonary_Analysis.RangeForDataBreathWriter);
                        break;
                    case StepOfAnalysis.EndStep2:
                        await WriteDataInJSON(Path.Combine(_settingsPulmonary_Analysis.PathDirectoryForDataBreath, _nameFileDataWorkLoad), _breathInWorkLoad);
                        GetTestData(spiroData);
                        _breathInWorkLoad.Clear();
                        Logger.LogInformation("data is fulled {Breath} written successfully", typeof(BreathInWorkLoad).Name);
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(string.Format("Error: {0}, Stack: {1}", ex.Message, ex.StackTrace));
            }

        }

        private static async Task WriteDataInJSON(string path, object item)
        {
            using StreamWriter sw = new StreamWriter(path, true, System.Text.Encoding.Default);
            string p = JsonConvert.SerializeObject(item, Formatting.Indented);
            await sw.WriteAsync(p);
        }




        public static double C1_fromATPtoSTPD(double Ta = 37)
        {
            return (273 * (760 - 47)) / ((273 + Ta) * 760);//0.826
        }
        public static double C2_fromBTPStoSTPD(double RH = 100, double PH2O = 47, double Ta = 37)
        {
            return (273 * (760 - RH * PH2O / 100)) / ((273 + Ta) * 760);//0.826
        }
        public static double C3_fromATPStoSTPD(double PH2O, double Ta)
        {
            return (273 * (760 - PH2O)) / ((273 + Ta) * 760);
        }
        [Conditional("Debug")]
        private async void GetTestData(List<SampleBreathing> BreathData)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Start method GetTestData");
            var stopwatch = new Stopwatch();
            try
            {
                stopwatch.Start();

                using (StreamWriter sw_flow = new StreamWriter(@"C:\SPIRODATE_ForTest\User1\Flow.txt", true, System.Text.Encoding.Default))
                    for (int i = 0; i < BreathData.Count; i++)
                    {
                        await sw_flow.WriteAsync($"{BreathData[i].Flow} ");
                    }

                using (StreamWriter sw_o2 = new StreamWriter(@"C:\SPIRODATE_ForTest\User1\o2.txt", true, System.Text.Encoding.Default))
                    for (int i = 0; i < BreathData.Count; i++)
                    {
                        await sw_o2.WriteAsync($"{BreathData[i].O2} ");
                    }


                using StreamWriter sw_co2 = new StreamWriter(@"C:\SPIRODATE_ForTest\User1\co2.txt", true, System.Text.Encoding.Default);
                for (int i = 0; i < BreathData.Count; i++)
                {
                    await sw_co2.WriteAsync($"{BreathData[i].CO2} ");
                }

            }
            catch (Exception ex)
            {
                Logger.LogError(string.Format("Error: {0}, Stack: {1}", ex.Message, ex.StackTrace));
            }

        }

    }
}
