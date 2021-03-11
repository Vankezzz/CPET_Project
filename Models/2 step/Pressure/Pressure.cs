
using Microsoft.Extensions.Logging;
using Respiratory_Analysis_CPET.AnyModelsForProjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Respiratory_Analysis_CPET
{
    /// <summary>
    /// Класс содержит в себе расчитанные давления респираторной системы
    /// </summary>
    public class Pressure
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Pressure"/> class.
        /// </summary>
        /// <param name="alveolar">Алеволярные давления О2 и СО2</param>
        /// <param name="arterial">Артериальное давления О2 и СО2 </param>
        /// <param name="endvaluesofbreath">Конечные давления газов</param>
        /// <param name="gasCoefficient">Диффузионные давления газов</param>
        public Pressure(Alveolar alveolar, Arterial arterial, EndValuesOfBreath endvaluesofbreath, GasCoefficient gasCoefficient)
        {
            Alveolar = alveolar;
            Arterial = arterial;
            Endvaluesofbreath = endvaluesofbreath;
            GasCoefficient = gasCoefficient;
        }

        public Alveolar Alveolar { get; private set; }
        public Arterial Arterial { get; private set; }
        public EndValuesOfBreath Endvaluesofbreath { get; private set; }
        public GasCoefficient GasCoefficient { get; private set; }
        public IEnumerable<Parameter> GetParameters()
        {
            return new Parameter[] { Alveolar.PACO2,Alveolar.PAO2, Arterial.PaCO2,Arterial.PaO2,Endvaluesofbreath.FETCO2,Endvaluesofbreath.FETO2,Endvaluesofbreath.PETCO2,Endvaluesofbreath.PETO2, GasCoefficient.Aa_DCO2,GasCoefficient.Aa_DO2,GasCoefficient.AET_DCO2,GasCoefficient.DLO2};
        }

        public static Pressure GetData(List<double> FIO2, List<double> FECO2DATA, List<double> FEO2DATA, double VO2, double VCO2, double VA, double RER,int N, ILogger logger)
        {
            EndValuesOfBreath endvaluesofbreath = EndValuesOfBreath.GetData(FECO2DATA, FEO2DATA,N,logger);
            Arterial arterial = Arterial.GetData(VCO2, VA,logger);
            Alveolar alveolar = Alveolar.GetData(FIO2, endvaluesofbreath.PETCO2.ParameterValue.Value, arterial.PaCO2.ParameterValue.Value, RER,logger);
            GasCoefficient gasCoefficient = GasCoefficient.GetData(VO2, alveolar.PACO2.ParameterValue.Value, arterial.PaCO2.ParameterValue.Value,alveolar.PAO2.ParameterValue.Value,arterial.PaO2.ParameterValue.Value,endvaluesofbreath.PETCO2.ParameterValue.Value,logger);
            return new Pressure(alveolar, arterial, endvaluesofbreath,gasCoefficient);
        }
    }
}
