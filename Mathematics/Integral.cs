using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Respiratory_Analysis_CPET
{
    public static class Integral
    {
         static public double Trap(double y0,double y, double n)  //трапеций
        {
            return (y + y0) * n * 0.5;
        }

        static public double Calculate_Volume(List<double> volumelist, double sampletime, double C)  //трапеций
        {
            double result = 0;
            for (int i = 1; i < volumelist.Count; i++)
            {
                result += Trap(volumelist[i - 1], volumelist[i], sampletime) * C;
            }
            return result;
        }
        static public List<double> Calculate_VolumeInList(List<double> volumelist, double sampletime, double C)  //трапеций
        {
            double resultForStep = 0;
            List<double> results = new List<double>();
            for (int i = 1; i < volumelist.Count; i++)
            {
                resultForStep += Trap(volumelist[i - 1], volumelist[i], sampletime) * C;
                results.Add(resultForStep);
            }
            return results;
        }
    }
}
