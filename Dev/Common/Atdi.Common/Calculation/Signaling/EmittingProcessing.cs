using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Modules.Sdrn.Calculation
{
    public class EmittingProcessing
    {
        public static double CalcPartialPow(float[] arr, int start, int stop)
        {
            if (start > stop) { int k = start; start = stop; stop = k; }
            if (start < 0) { start = 0; }
            if (stop >= arr.Length) { stop = arr.Length - 1; }
            double Pow = 0;
            for (var i = start; stop >= i; i++)
            {
                Pow = Pow + Math.Pow(10, arr[i] / 10);
            }
            Pow = Pow / (stop - start + 1);
            Pow = 10 * Math.Log10(Pow);
            return Pow;
        }

        public static int[] BedCorrelationEmitting(int[] ArrHit, double[] CoordinationFactors, int[] EmittingOnFreq, int deleted)
        {
            // нужно составить список из худших излучкений которые стоит убить
            int count = ArrHit.Length;
            int mark = 0;
            var resArr = new List<int>();
            var RangeArr = new double[count];
            int Max_Count_On_Freq = 0;
            for (var i = 0; i < count; i++)
            {
                if (EmittingOnFreq[i] > Max_Count_On_Freq) { Max_Count_On_Freq = EmittingOnFreq[i]; }
                RangeArr[i] = (1 - CoordinationFactors[i]) * Math.Pow(ArrHit[i], 0.5);
            }
            if (Max_Count_On_Freq < deleted) { return null; }
            while (mark < deleted)
            {
                // Ищем мининум в массиве RangeArr[i] 
                double min = 1000000; int minIndex = -1;
                for (int i = 0; i < count; i++)
                {
                    if ((Max_Count_On_Freq == EmittingOnFreq[i]) && (min > RangeArr[i]))
                    {
                        min = RangeArr[i]; minIndex = i;
                    }
                }
                if (minIndex > -1) { resArr.Add(minIndex); RangeArr[minIndex] = 9999999; mark++; } else { break; }
            }
            if (resArr.Count == 0) { return null; }
            var distinctList = resArr.Distinct();
            resArr = distinctList.ToList();
            resArr.Sort();
            resArr.Reverse();
            var resArr1 = resArr.ToArray();
            return resArr1;
        }
    }
}
