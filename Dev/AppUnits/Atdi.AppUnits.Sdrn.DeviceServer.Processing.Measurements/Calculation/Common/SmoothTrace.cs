using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.DataModels.Sdrn.DeviceServer.Processing;

namespace Atdi.AppUnits.Sdrn.DeviceServer.Processing.Measurements
{
    public static class SmoothTrace
    {
        private const double PersentLongFilterFromTrace = 3;
        public static float[] blackman (float[] ArrIn, bool LogInterpolation = false)
        { // НЕ ТЕСТИРОВАННО
            
            int N = (int)Math.Floor(ArrIn.Length * PersentLongFilterFromTrace / 200); // Длина фильтра
            N = N * 2 + 1;
            if (N <= 2) { return ArrIn;}
            var H = new double[N]; // Импульсная характеристика фильтра
            // Расчет импульсной характеристики фильтра Блекмана
            for (var i = 0; N > i; i++)
            {
                H[i] = 0.42 - 0.5 * Math.Cos((2 * Math.PI * (i - 1)) / (N - 1)) + 0.08 * Math.Cos((4 * Math.PI * (i - 1)) / (N - 1));
            }
            //Нормировка импульсной характеристики
            double SUM = 0;
            for (var i = 0; N > i; i++)
            {
                SUM = SUM + H[i];
            }
            for (int i = 0; N > i; i++)
            {
                H[i] = H[i] / SUM;  // сумма коэффициентов равна 1
            }

            int z = ArrIn.Length;
            var outArr = new double[z];
            var ArrIn1 = new double[z];
            // Фильтрация входных данных
            if (!LogInterpolation)
            {
                for (int i = 0; z > i; i++)
                {
                    ArrIn1[i] = Math.Pow(10,ArrIn[i] / 10);
                }
            }
            int st = (int)(Math.Floor(N/2.0)); // Индекс точки с максимальной характеристикой 
            for (var i = st; z + st > i; i++)
            {
                outArr[i - st] = 0;
                for (var j = 0; N > j; j++)
                {
                    if (i - j >= 0)
                    {
                        if (i - j <= ArrIn1.Length-1)
                        {
                            outArr[i - st] = outArr[i - st] + H[j] * ArrIn1[i - j]; // самая формула фильтра
                        }
                        else
                        {
                            outArr[i - st] = outArr[i - st] + H[j] * ArrIn1[ArrIn1.Length - 1];
                        }
                    }
                    else
                    {
                        outArr[i - st] = outArr[i - st] + H[j] * ArrIn1[0];
                    }
                }
            }
            if (!LogInterpolation)
            {
                for (int i = 0; z > i; i++)
                {
                    outArr[i] = 10*Math.Log10(outArr[i]);
                }
            }
            var ArrOut = new float[outArr.Length];
            for (var i = 0; z > i; i++)
            {
                ArrOut[i] = (float)outArr[i];
            }
            return ArrOut as float[];
        }
    }
}
