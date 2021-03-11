using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Respiratory_Analysis_CPET.PulmonaryAnalysis;

namespace Respiratory_Analysis_CPET
{

    public class Filtration
    {
        public ConcurrentQueue<SampleBreathing> q = new ConcurrentQueue<SampleBreathing>();
        private object lockObject = new object();
        public delegate void FilterHandler(SampleBreathing sampleBreathing, StepOfAnalysis flag, DateTime dateTime);
        public event FilterHandler DataFiltred;
        SampleBreathing overflow;


        public Filtration(int limit, int sizePack)
        {
            Limit = limit;
            SizePack = sizePack;
            BuffLimit = Limit;
        }

        public int Limit { get; set; }
        public int BuffLimit { get; set; }
        public int SizePack { get; set; }


        public void FilterData(SampleBreathing data, StepOfAnalysis flag)
        {
            q.Enqueue(data);
            lock (lockObject)
            {

                if (q.Count % SizePack == 0)
                {
                    var result = UseMedianaFilter(q);
                    DataFiltred?.Invoke(result, flag, DateTime.Now);
                }
                while (q.Count >= Limit && q.TryDequeue(out overflow))
                {
                    Limit = BuffLimit - SizePack + 1;
                    Console.WriteLine($"\n Overflow = " + (overflow as SampleBreathing).Flow.ParameterValue.Value);
                }
                Limit = BuffLimit;

            }
            static SampleBreathing UseMedianaFilter(ConcurrentQueue<SampleBreathing> q)
            {
                var listFlow = q.AsParallel().Select(x => x.Flow.ParameterValue.Value).ToList();
                var listO2 = q.AsParallel().Select(x => x.O2.ParameterValue.Value).ToList();
                var listCO2 = q.AsParallel().Select(x => x.CO2.ParameterValue.Value).ToList();
                listFlow.Sort();
                listO2.Sort();
                listCO2.Sort();
                return new SampleBreathing(listFlow[listFlow.Count / 2], listO2[listO2.Count / 2], listCO2[listCO2.Count / 2]);
            }
        }
    }

}
