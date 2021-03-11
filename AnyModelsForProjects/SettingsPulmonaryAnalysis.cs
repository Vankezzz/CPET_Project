using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Respiratory_Analysis_CPET.AnyModelsForProjects
{
     public class SettingsPulmonaryAnalysis
    {
        /// <summary>
        /// Класс с необходимыми начальными параметрами <see cref="SettingsPulmonaryAnalysis"/> class.
        /// </summary>
        /// <param name="vdm">Обьем мунштука - это называется мертвое пространство - задать в литрах</param>
        /// <param name="person">The person.</param>
        /// <param name="sampleTime">Период дискретизации в секундах (к примеру, частота получения данных этой библиотекой 100 ГЦ = 0.01секунда)</param>
        /// <param name="sizeOfPack_filter">Размер пакетов для фильтра (преобразователь частоты дискретизации)</param>
        /// <param name="limitOfWindow_filter">The limit of window filter.</param>
        /// <param name="logger">Обьект логгирования</param>
        /// <param name="rangeForDataBreathWriter">Через сколько циклов дыхания начать запись в файл</param>
        /// <param name="pathDirectoryForDataBreath">Путь сохранения файла с циклами дыхания типа Breath  в колличестве, которое задается rangeForDataBreathWriter (путь к папке)</param>
        public SettingsPulmonaryAnalysis(ILogger logger, Person person,  double sampleTime, string pathDirectoryForDataBreath,  int sizeOfPack_filter = 5, int limitOfWindow_filter = 105, byte rangeForDataBreathWriter=10, double vdm = 0)
        {
            Person = person;
            Vdm = vdm;
            SampleTime = sampleTime;
            SizeOfPack_filter = sizeOfPack_filter;
            LimitOfWindow_filter = limitOfWindow_filter;
            Logger = logger;
            PathDirectoryForDataBreath = pathDirectoryForDataBreath;
            RangeForDataBreathWriter = rangeForDataBreathWriter;
        }

        public Person Person { get; private set; }
        public double Vdm { get; private set; }
        public double SampleTime { get; private set; }
        public int SizeOfPack_filter { get; private set; }
        public int LimitOfWindow_filter { get; private set; }
        public ILogger Logger { get; private set; }
        public string PathDirectoryForDataBreath { get; private set; }
        public byte RangeForDataBreathWriter { get; private set; }
        
        
    }
}
