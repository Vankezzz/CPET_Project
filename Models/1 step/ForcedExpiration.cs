using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Extensions.Logging;
using Respiratory_Analysis_CPET.AnyModelsForProjects;

namespace Respiratory_Analysis_CPET._1_step
{
    public  class ForcedExpiration
    {
        static ILogger _logger;
        /// <summary>
        /// Initializes a new instance of the <see cref="ForcedExpirationValue"/> class.
        /// </summary>
        /// <param name="vC">ЖЕЛ - жизненная емкость легких.  </param>
        /// <param name="fVC">ФЖЕЛ -  Форсированная жизненная ёмкость легких - объём воздуха, выдыхаемый при максимально быстром и сильном выдохе.</param>
        /// <param name="fEV05">ОФВ05 - Объем форсированного выдоха за 0,5 секунды </param>
        /// <param name="fEV1">ОФВ1 - Объем форсированного выдоха за 1 секунды  </param>
        /// <param name="fEV3">ОФВ3 - Объем форсированного выдоха за 3 секунды  </param>
        /// <param name="fEV6">ОФВ6 - Объем форсированного выдоха за 6 секунды  </param>
        /// <param name="pEF">ПОСвыд - пиковая объемная  форсированная скорость выдоха </param>
        /// <param name="fEF25">МОС25% - объемная форсированная скорость выдоха интервале 25% ФЖЕЛ </param>
        /// <param name="fEF50">МОС50% - объемная форсированная скорость выдоха интервале 50% ФЖЕЛ </param>
        /// <param name="fEF75">МОС75% - объемная форсированная скорость выдоха интервале 75% ФЖЕЛ </param>
        /// <param name="mEF25_75">СОС25-75 средняя объемная скорость выдоха, определяемая в процессе выдоха от 25% до 75% ФЖЕЛвыд </param>
        /// <param name="mEF75_85">СОС75-85 -  средняя объемная скорость выдоха, определяемая в процессе выдоха от 75% до 85% ФЖЕЛвыд.</param>
        /// <param name="mEF02_12">СОС0.2-1.2 - средняя объемная скорость выдоха от точки достижения объема 0.2 л до точки достижения объема 1.2 л.</param>
        /// <param name="pIF">ПОСвд - пиковая объемная  форсированная скорость вдоха </param>
        /// <param name="fIF25">Объемная форсированная скорость вдоха интервале 25% ФЖЕЛ </param>
        /// <param name="fIF50">Объемная форсированная скорость вдоха интервале 50% ФЖЕЛ </param>
        /// <param name="fIF75">Объемная форсированная скорость вдоха интервале 75% ФЖЕЛ</param>
        /// <param name="fIV05">Объем форсированного вдоха за 0.5 секунды</param>
        /// <param name="fIV1">Объем форсированного вдоха за 1 секунды</param>
        /// <param name="fIV3">Объем форсированного вдоха за 3 секунды</param>
        public ForcedExpiration(double vC, double fVC, double[] FEV, double pEF, double[] FEF, double mEF02_12, double pIF, double[] FIF, double[] FIV)
        {
            FEV05 = new Parameter("FEV05",new ParameterValue(Math.Round(FEV[0], 2),"L"));
            FEV1 = new Parameter("FEV1", new ParameterValue(Math.Round(FEV[1], 2), "L"));
            FEV3 = new Parameter("FEV3", new ParameterValue(Math.Round(FEV[2], 2), "L"));
            FEV6 = new Parameter("FEV6", new ParameterValue(Math.Round(FEV[3], 2), "L"));
            PEF = new Parameter("PEF", new ParameterValue(Math.Round(pEF, 2), "L/S"));
            FEF25 = new Parameter("FEF25", new ParameterValue(Math.Round(FEF[0], 2), "L/S"));
            FEF50 = new Parameter("FEF50", new ParameterValue(Math.Round(FEF[1], 2), "L/S"));
            FEF75 = new Parameter("FEF75", new ParameterValue(Math.Round(FEF[2], 2), "L/S"));
            MEF25_75 = new Parameter("MEF25-75", new ParameterValue(Math.Round(FEF[3], 2), "L/S"));
            MEF75_85 = new Parameter("MEF75-85", new ParameterValue(Math.Round(FEF[4], 2), "L/S"));
            MEF02_12 = new Parameter("MEF0.2-1.2", new ParameterValue(Math.Round(mEF02_12, 2), "L/S"));
            FVC = new Parameter("FVC", new ParameterValue(Math.Round(fVC, 2), "L"));
            PIF = new Parameter("PIF", new ParameterValue(Math.Round(pIF, 2), "L/S"));
            FIF25 = new Parameter("FIF25", new ParameterValue(Math.Round(FIF[0], 2), "L/S"));
            FIF50 = new Parameter("FIF50", new ParameterValue(Math.Round(FIF[1], 2), "L/S"));
            FIF75 = new Parameter("FIF75", new ParameterValue(Math.Round(FIF[2], 2), "L/S"));
            FIV05 = new Parameter("FIV05", new ParameterValue(Math.Round(FIV[0], 2), "L/S"));
            FIV1 = new Parameter("FIV1", new ParameterValue(Math.Round(FIV[1], 2), "L"));
            FIV3 = new Parameter("FIV3", new ParameterValue(Math.Round(FIV[2], 2), "L"));
            FEV05_FVC = new Parameter("FEV05/FVC", new ParameterValue(Math.Round((FEV05.ParameterValue.Value / FVC.ParameterValue.Value), 2), "None"));
            FEV1_FVC = new Parameter("FEV1/FVC", new ParameterValue(Math.Round((FEV1.ParameterValue.Value / FVC.ParameterValue.Value), 2), "None"));
            FEV05_VC = new Parameter("FEV05/VC", new ParameterValue(Math.Round((FEV05.ParameterValue.Value / vC), 2), "None"));
            FEV1_VC = new Parameter("FEV1/VC", new ParameterValue(Math.Round((FEV1.ParameterValue.Value / vC), 2), "None"));
        }
        public IEnumerable<Parameter> GetParameters()
        {
            return new Parameter[] { FEV05,FEV1,FEV3,FEV6,PEF,FEF25,FEF50,FEF75,MEF25_75,MEF75_85,MEF02_12,FVC,PIF,FIF25,FIF50,FIF75,FIV05,FIV1,FIV1,FIV3, FEV05_FVC, FEV1_FVC, FEV05_VC, FEV1_VC };
        }
        public Parameter FEV05 { get; private set; }
        public Parameter FEV1 { get; private set; }
        public Parameter FEV3 { get; private set; }
        public Parameter FEV6 { get; private set; }
        public Parameter PEF { get; private set; }
        public Parameter FEF25 { get; private set; }
        public Parameter FEF50 { get; private set; }
        public Parameter FEF75 { get; private set; }
        public Parameter MEF25_75 { get; private set; }
        public Parameter MEF75_85 { get; private set; }
        public Parameter MEF02_12 { get; private set; }
        public Parameter FVC { get; private set; }


        public Parameter PIF { get; private set; }
        public Parameter FIF25 { get; private set; }
        public Parameter FIF50 { get; private set; }
        public Parameter FIF75 { get; private set; }
        public Parameter FIV05 { get; private set; }
        public Parameter FIV1 { get; private set; }
        public Parameter FIV3 { get; private set; }

        public Parameter FEV05_FVC { get; private set; }
        public Parameter FEV1_FVC { get; private set; }

        public Parameter FEV05_VC { get; private set; }
        public Parameter FEV1_VC { get; private set; }
        private static double _sampleTime;

        public static ForcedExpiration GetData (List<double> flowins,List<double> flowexp,double vC,double _sampleTime,ILogger logger)
        {
            _logger = logger;
            ForcedExpiration._sampleTime = _sampleTime;
            var volumeListInIns = Integral.Calculate_VolumeInList(flowins,PulmonaryAnalysis.C1_fromATPtoSTPD(),_sampleTime);
            var volumeListInExp = Integral.Calculate_VolumeInList(flowexp,PulmonaryAnalysis.C2_fromBTPStoSTPD(),_sampleTime);
            double FVC = DefinitionFVC(volumeListInExp);
            var FEV = DefinitionFEV(volumeListInExp);
            var FEF = DefinitionFEF(volumeListInExp,flowexp, FVC);
            double MEF02_12 = DefinitionMEF02_12(flowexp);
            var FIV = DefinitionFIV(volumeListInIns);
            var FIF = DefinitionFIF(volumeListInIns, flowins, FVC);
            double PEF = DefinitionPEF(flowexp);
            double PIF = DefinitionPIF(flowins);
            return new ForcedExpiration(vC,FVC,FEV,PEF,FEF,MEF02_12,PIF,FIF,FIV);
        }
        static double[] DefinitionFEV(List<double> volumeListExp)
        {
            double[] FEV = new double[4];
            try
            {
                FEV[0] = 0; FEV[1] = 0; FEV[2] = 0; FEV[3] = 0;
                int pointsperseconds = Convert.ToInt32(1 / _sampleTime);
                if (volumeListExp.Count > pointsperseconds * 0.5)
                {
                    FEV[0] = volumeListExp[(int)(pointsperseconds * 0.5)];
                }
                else FEV[0] = 0;
                if (volumeListExp.Count > pointsperseconds)
                {

                    FEV[1] = volumeListExp[pointsperseconds];

                }
                else FEV[1] = 0;
                if (volumeListExp.Count > pointsperseconds * 3)
                {
                    FEV[2] = volumeListExp[(int)(pointsperseconds * 3)];
                }
                else FEV[2] = 0;
                if (volumeListExp.Count > pointsperseconds * 6)
                {
                    FEV[3] = volumeListExp[(int)(pointsperseconds * 6)];
                }
                else FEV[3] = 0;
            }
            catch (Exception ex)
            {
                FEV[0] = -1;
                FEV[1] = -1;
                FEV[2] = -1;
                FEV[3] = -1;
                _logger.LogError(string.Format("Error: {0}, Stack: {1}", ex.Message, ex.StackTrace));
            }
            return FEV;
        }
        static double[] DefinitionFIV(List<double> volumeListIns)
        {
            double[] FIV = new double[3];
            try
            {
                int pointsperseconds = Convert.ToInt32(1 / _sampleTime);
                if (volumeListIns.Count> pointsperseconds * 0.5)
                {
                    FIV[0] = volumeListIns[(int)(pointsperseconds * 0.5)];
                }
                else FIV[0] = 0;
                if (volumeListIns.Count> pointsperseconds)
                {

                    FIV[1] = volumeListIns[pointsperseconds];

                }
                else FIV[1] = 0;
                if (volumeListIns.Count> pointsperseconds * 3)
                {
                    FIV[2] = volumeListIns[(int)(pointsperseconds * 3)];
                }
                else FIV[2] = 0;

            }
            catch (Exception ex)
            {
                FIV[0] = -1;
                FIV[1] = -1;
                FIV[2] = -1;
                _logger.LogError(string.Format("Error: {0}, Stack: {1}", ex.Message, ex.StackTrace));
            }
            return FIV;
        }
        static double[] DefinitionFEF(List<double> volumeListExp, List<double> flowListExp, double FVC)
        {
            double[] FEF = new double[5];
            try
            {
                int index25 = 0, index50 = 0, index75 = 0, index85 = 0;
                byte statusFEF = 0;
                for (int i = 0; i < volumeListExp.Count; i++)
                {
                    if (volumeListExp[i] >= 0.25 * FVC && statusFEF == 0)
                    {
                        FEF[0] = flowListExp[i];
                        index25 = i;
                        statusFEF = 25;
                    }
                    else if (volumeListExp[i] >= 0.5 * FVC && statusFEF == 25)
                    {
                        FEF[1] = flowListExp[i];
                        index50 = i;
                        statusFEF = 50;
                    }
                    else if (volumeListExp[i] >= 0.75 * FVC && statusFEF == 50)
                    {
                        FEF[2] = flowListExp[i];
                        index75 = i;
                        FEF[3] = flowListExp.GetRange(index25, (index75 - index25)).Average();
                        statusFEF = 85;
                    }
                    else if (volumeListExp[i] >= 0.75 * FVC && statusFEF == 85)
                    {
                        index85 = i;
                        FEF[4] = flowListExp.GetRange(index75, (index85 - index75)).Average();
                        statusFEF = 99;
                    }
                }
            }
            catch (Exception ex)
            {
                FEF[0] = -1;
                FEF[1] = -1;
                FEF[2] = -1;
                FEF[3] = -1;
                FEF[4] = -1;
                _logger.LogError(string.Format("Error: {0}, Stack: {1}", ex.Message, ex.StackTrace));
            }
            return FEF;
        }
        static double[] DefinitionFIF(List<double> volumeListIns, List<double> flowListIns, double FVC)
        {
            double[] FIF = new double[3];
            try
            {
                int index25 = 0, index50 = 0, index75 = 0;
                byte statusFEF = 0;
                for (int i = 0; i < volumeListIns.Count; i++)
                {
                    if (volumeListIns[i] >= 0.25 * FVC && statusFEF == 0)
                    {
                        FIF[0] = flowListIns[i];
                        index25 = i;
                        statusFEF = 25;
                    }
                    else if (volumeListIns[i] >= 0.5 * FVC && statusFEF == 25)
                    {
                        FIF[1] = flowListIns[i];
                        index50 = i;
                        statusFEF = 50;
                    }
                    else if (volumeListIns[i] >= 0.75 * FVC && statusFEF == 50)
                    {
                        FIF[2] = flowListIns[i];
                        index75 = i;
                        statusFEF = 99;
                    }
                }
            }
            catch (Exception ex)
            {
                FIF[0] = -1;
                FIF[1] = -1;
                FIF[2] = -1;
                _logger.LogError(string.Format("Error: {0}, Stack: {1}", ex.Message, ex.StackTrace));
            }
            return FIF;
        }
        static double DefinitionPEF(List<double> flowExp)
        {
            double PEF;
            try
            {
                PEF = flowExp.Max();
            }
            catch (Exception ex)
            {
                PEF = -1;
                _logger.LogError(string.Format("Error: {0}, Stack: {1}", ex.Message, ex.StackTrace));
            }
            return PEF;
        }
        static double DefinitionPIF(List<double> flowIns)
        {
            double PIF;
            try
            {
                PIF = flowIns.Max();
            }
            catch (Exception ex)
            {
                PIF = -1;
                _logger.LogError(string.Format("Error: {0}, Stack: {1}", ex.Message, ex.StackTrace));
            }
            return PIF;
        }
        static double DefinitionFVC(List<double> volumeListExp)
        {
            double FVC;
            try
            {
                FVC = volumeListExp.Max();
            }
            catch (Exception ex)
            {
                FVC = -1;
                _logger.LogError(string.Format("Error: {0}, Stack: {1}", ex.Message, ex.StackTrace));
            }
            return FVC;
        }
        static double DefinitionMEF02_12(List<double> flowExp)
        {
            double MEF02_12;
            try
            {
                double resultForStep = 0;
                int indexOfvolume02 = 0, indexOfvolume12 = 0;
                MEF02_12 = 0;
                for (int i = 1; i < flowExp.Count; i++)
                {
                    resultForStep += (flowExp[i] + flowExp[i - 1]) * 0.5 * _sampleTime * PulmonaryAnalysis.C2_fromBTPStoSTPD();
                    if (resultForStep >= 0.2)
                    {
                        indexOfvolume02 = i;
                    }
                    else if (resultForStep >= 1.2)
                    {
                        indexOfvolume12 = i;
                        MEF02_12 = flowExp.GetRange(indexOfvolume02, indexOfvolume12 - indexOfvolume02).Average();
                    }
                }
            }
            catch (Exception ex)
            {
                MEF02_12 = -1;
                _logger.LogError(string.Format("Error: {0}, Stack: {1}", ex.Message, ex.StackTrace));
            }
            return MEF02_12;
        }
    }
}
