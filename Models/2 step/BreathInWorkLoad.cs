using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Respiratory_Analysis_CPET
{
    /// <summary>
    /// Класс содержит в себе 1 цикл дыхания
    /// </summary>
    public class BreathInWorkLoad
    {
        /// <summary>
        /// Инициализация класса расчитанными значениями за 1 дыхательный цикл
        /// </summary>
        /// <param name="expiration">Данные-точки Потока,О2,СО2 в течении фазы выдоха</param>
        /// <param name="inspiration">Данные-точки Потока,О2,СО2 в течении фазы вдоха</param>
        /// <param name="basics">Содержит частоту дыхательных движений </param>
        /// <param name="volume">Содержит измеряемые обьемы легких</param>
        /// <param name="coefficient">Содержит дыхательные коэффициенты</param>
        /// <param name="pressure">Содержит измеряемые давления</param>
        public BreathInWorkLoad(List<SampleBreathing> expiration, List<SampleBreathing> inspiration, Basics basics, Volume volume, Coefficient coefficient, Pressure pressure)
        {
            Expiration = expiration;
            Inspiration = inspiration;
            Basicscalculations = basics;
            Volume = volume;
            Coefficient = coefficient;
            Pressure = pressure;
        }


        /// <value>
        /// Данные-точки Потока,О2,СО2 в течении фазы выдоха
        /// </value>
        public List<SampleBreathing> Expiration { get; private set; }


        /// <value>
        /// Данные-точки Потока,О2,СО2 в течении фазы вдоха
        /// </value>
        /// 
        public List<SampleBreathing> Inspiration { get; private set; }
      

        /// <value>
        /// Содержит частоту дыхательных движений, дыхательный обьем, вдохнутый обьем
        /// </value>
        public Basics Basicscalculations { get; private set; }
       

        /// <value>
        /// Содержит измеряемые обьемы легких
        /// </value>
        public Volume Volume { get; private set; }
        

        /// <value>
        /// Содержит дыхательные коэффициенты
        /// </value>
        public Coefficient Coefficient { get; private set; }
      

        /// <value>
        /// Содержит измеряемые давления
        /// </value>
        public Pressure Pressure { get; private set; }

    }
}
