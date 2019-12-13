using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.DataModels.Sdrn.DeviceServer.Processing;

namespace Atdi.AppUnits.Sdrn.DeviceServer.Processing.Measurements
{
    public static class СorrelationСoefficient
    {
        public enum MethodCalcCorrelation {Person, PersonLinear, Spearman, SpearmanSmmetric}
        public static double CalcCorrelation(float[] arr1, float[] arr2, MethodCalcCorrelation methodCalcCorrelation = MethodCalcCorrelation.Person)
        { // НЕ ТЕСТИРОВАННО 
            if (arr1.Length != arr2.Length) { return -2; }//Выход с ошибкой
            if (arr1.Length < 2) { return -2; }//Выход с ошибкой
            switch (methodCalcCorrelation)
            {
                case (MethodCalcCorrelation.Person):
                    return (Pearson(arr1, arr2));
                case (MethodCalcCorrelation.PersonLinear):
                    return (PearsonLinear(arr1, arr2));
                case (MethodCalcCorrelation.Spearman):
                    return (Spearman(arr1, arr2));
                case (MethodCalcCorrelation.SpearmanSmmetric):
                    return (SpearmanSmmetric(arr1, arr2));
                default:
                    return (Pearson(arr1, arr2));
            }
        }
        public static double CalcCorrelation(float[] arr1, double freq1_Hz, double BW1_Hz, float[] arr2, double freq2_Hz, double BW2_Hz, MethodCalcCorrelation methodCalcCorrelation = MethodCalcCorrelation.Person)
        {// НЕ ТЕСТИРОВАННО
            float[] arr1corr; float[] arr2corr;
            bool a = ArrAdaptation(arr1, freq1_Hz, BW1_Hz, arr2, freq2_Hz, BW2_Hz, out arr1corr, out arr2corr, 30);
            if (!a) { return -2; }
            switch (methodCalcCorrelation)
            {
                case (MethodCalcCorrelation.Person):
                    return (Pearson(arr1corr, arr2corr));
                case (MethodCalcCorrelation.PersonLinear):
                    return (PearsonLinear(arr1corr, arr2corr));
                case (MethodCalcCorrelation.Spearman):
                    return (Spearman(arr1corr, arr2corr));
                case (MethodCalcCorrelation.SpearmanSmmetric):
                    return (SpearmanSmmetric(arr1corr, arr2corr));
                default:
                    return (Pearson(arr1corr, arr2corr));
            }
        }
        private static double Pearson(float[] arr1, float[] arr2)
        { // НЕ ТЕСТИРОВАННО
            if (arr1.Length != arr2.Length) { return -2; }//Выход с ошибкой
            int n = arr1.Length;
            double sumArr1 = 0; double sumArr2 = 0;
            for (int i = 0; n > i; i++)
            {
                sumArr1 = sumArr1 + arr1[i];
                sumArr2 = sumArr2 + arr2[i];
            }
            sumArr1 = sumArr1 / n;
            sumArr2 = sumArr2 / n;
            double a1 = 0; double a2 = 0; double a3 = 0;
            for (var i = 0; n > i; i++)
            {
                a1 = a1 + ((arr1[i] - sumArr1) * (arr2[i] - sumArr2));
                a2 = a2 + ((arr1[i] - sumArr1) * (arr1[i] - sumArr1));
                a3 = a3 + ((arr2[i] - sumArr2) * (arr2[i] - sumArr2));
            }
            return (a1 / (Math.Sqrt(a2 * a3)));
        }
        private static double PearsonLinear(float[] arr1, float[] arr2)
        {// НЕ ТЕСТИРОВАННО
            var arr1Lin = FromdBmtomW(arr1);
            var arr2Lin = FromdBmtomW(arr2);
            return Pearson(arr1Lin, arr2Lin);
        }
        private static double Spearman(float[] arr1, float[] arr2)
        { // НЕ ТЕСТИРОВАННО
            if (arr1.Length != arr2.Length) { return -2; }//Выход с ошибкой
            var rang1 = rangArrCalc(arr1);
            var rang2 = rangArrCalc(arr2);
            double dd = 0;
            for (var i =0; arr1.Length>i; i++)
            {
                dd = (rang1[i] - rang2[i]) * (rang1[i] - rang2[i])+dd;
            }
            return 1-(6*dd/((arr1.Length* arr1.Length-1)* arr1.Length));
        }
        private static double SpearmanSmmetric(float[] arr1, float[] arr2)
        { // НЕ ТЕСТИРОВАННО
            if (arr1.Length != arr2.Length) { return -2; }//Выход с ошибкой
            int indexCenter = (int)Math.Round(arr1.Length / 2.0);
            var arr11 = new float[indexCenter];
            Array.Copy(arr1, 0, arr11, 0, indexCenter);
            var arr21 = new float[indexCenter];
            Array.Copy(arr2, 0, arr21, 0, indexCenter);
            double part1 = Spearman(arr11, arr21);
            var arr12 = new float[arr1.Length - indexCenter];
            Array.Copy(arr1, indexCenter, arr12, 0, arr1.Length - indexCenter);
            var arr22 = new float[arr1.Length - indexCenter];
            Array.Copy(arr2, indexCenter, arr22, 0, arr1.Length - indexCenter);
            double part2 = Spearman(arr12, arr22);
            return (part1+part2)/2.0;
        }
        private static int [] rangArrCalc(float[] arr1)
        { // Проверенно
            if (arr1 is null) { return null;}
            var arr = new float[arr1.Length];
            for (var i = 0; arr.Length > i; i++) { arr[i] = arr1[i];}
            var rangArr = new int[arr.Length];
            for (var i = 0; arr.Length > i; i++)
            {
                double minValue = 99999999;
                int IndexMinValue = 0;
                for (var j = 0; arr.Length > j; j++)
                {
                    if (arr[j] < minValue)
                    {
                        minValue = arr[j];
                        IndexMinValue = j;
                    }
                }
                rangArr[IndexMinValue] = i;
                arr[IndexMinValue] = 99999999;
            }
            return rangArr;
        }
        private static float[] FromdBmtomW(float[] arr_dBm)
        {// НЕ ТЕСТИРОВАННО
            var arr_mW = new float[arr_dBm.Length];
            for (var i = 0; arr_dBm.Length > i; i++)
            {
                arr_mW[i] = (float)Math.Pow(10, arr_dBm[i] / 10);
            }
            return arr_mW; 
        }
        public static int CalcShiftSpectrum(float[] arr1, float[] arr2, MethodCalcCorrelation methodCalcCorrelation, out double BestCorrelation, double PersentMaxShift = 10)
        {// НЕ ТЕСТИРОВАННО
            // пока реализовано простое смещение без ускорений В дальнейшем это можно принципиально оптимизировать.
            // смещение идет второго масива относительно первого.
            BestCorrelation = -3;
            int shift = 0;
            if (arr1.Length != arr2.Length) { return -1000000; }//Выход с ошибкой
            if (arr1.Length < 2) { return -1000000; }//Выход с ошибкой
            int MaxPointShift = (int)Math.Round(arr1.Length * PersentMaxShift / 100.0);
            var Corr = new List<double>();
            for (var i = -MaxPointShift; MaxPointShift >= i; i++)
            {
                int l = arr1.Length - Math.Abs(i);
                var arr1sh = new float[l];
                var arr2sh = new float[l];
                if (i > 0)
                {
                    Array.Copy(arr1, i, arr1sh, 0, arr1.Length - i);
                    Array.Copy(arr2, 0, arr2sh, 0, arr2.Length - i);
                }
                else
                {
                    Array.Copy(arr1, 0, arr1sh, 0, arr1.Length + i);
                    Array.Copy(arr2, - i, arr2sh, 0, arr2.Length + i);
                }
                double Correlation = CalcCorrelation(arr1sh, arr2sh, methodCalcCorrelation);
                Corr.Add(Correlation);
                if (Correlation > BestCorrelation) { BestCorrelation = Correlation; shift = i; }
            }
            return shift;
        }
        public static int CalcShiftSpectrum(float[] arr1, double freq1_Hz, double BW1_Hz, float[] arr2, double freq2_Hz, double BW2_Hz, MethodCalcCorrelation methodCalcCorrelation, out double BestCorrelation, double PersentMaxShift = 10)
        {// НЕ ТЕСТИРОВАННО предварительно тестированно
         // пока реализовано простое смещение без ускорений В дальнейшем это можно принципиально оптимизировать.
         // смещение идет второго масива относительно первого.
            BestCorrelation = -3;
            int shift = 0;
            float[] arr1corr; float[] arr2corr;
            bool a = ArrAdaptation(arr1, freq1_Hz, BW1_Hz, arr2, freq2_Hz, BW2_Hz, out arr1corr, out arr2corr, 30);
            if (!a) { return -1000000; }
            shift = CalcShiftSpectrum(arr1corr, arr2corr, methodCalcCorrelation, out BestCorrelation, PersentMaxShift);
            return shift;
        }
        private static bool ArrAdaptation(float[] arr1, double freq1_Hz, double BW1_Hz,  float[] arr2, double freq2_Hz, double BW2_Hz, out float[] arr1corr, out float[] arr2corr, double MinPersentOverlap = 90)
        {// НЕ ТЕСТИРОВАННО
            arr1corr = null;
            arr2corr = null;
            // определим степень пересечения частот
            double BWMax = Math.Max(freq1_Hz + arr1.Length*BW1_Hz, freq2_Hz + arr2.Length * BW2_Hz) - Math.Min(freq1_Hz, freq2_Hz);
            double BWMin = Math.Min(freq1_Hz + arr1.Length * BW1_Hz, freq2_Hz + arr2.Length * BW2_Hz) - Math.Max(freq1_Hz, freq2_Hz);
            if ((BWMin)/BWMax < MinPersentOverlap/100) { return false;}
            // степень пересечения удовлетворительная. Можно идти дальше.
            if (BW1_Hz >= BW2_Hz)
            {
                ArrTransform(arr1, freq1_Hz, BW1_Hz, arr2, freq2_Hz, BW2_Hz, out arr1corr, out arr2corr);
            }
            else
            {
                ArrTransform(arr2, freq2_Hz, BW2_Hz, arr1, freq1_Hz, BW1_Hz, out arr2corr, out arr1corr);
            }
            return true; 
        }
        private static void ArrTransform(float[] arr1, double freq1_Hz, double BW1_Hz, float[] arr2, double freq2_Hz, double BW2_Hz, out float[] arr1corr, out float[] arr2corr)
        {// НЕ ТЕСТИРОВАННО
            // считаем индексы первого масива 
            int indexArr1Start; int indexArr1Stop;
            if (freq1_Hz >= freq2_Hz) { indexArr1Start = 0; } else { indexArr1Start = (int)Math.Ceiling((freq2_Hz - freq1_Hz) / BW1_Hz); }
            if (freq1_Hz + BW1_Hz * arr1.Length <= freq2_Hz + BW2_Hz * arr2.Length)
            { indexArr1Stop = arr1.Length - 1; }
            else { indexArr1Stop = (int)Math.Floor((freq2_Hz + BW2_Hz * arr2.Length - freq1_Hz) / BW1_Hz); }
            arr1corr = new float[indexArr1Stop - indexArr1Start + 1];
            arr2corr = new float[indexArr1Stop - indexArr1Start + 1];
            Array.Copy(arr1, indexArr1Start, arr1corr, 0, indexArr1Stop - indexArr1Start + 1);
            // преобразуем второй массив
            double freq1corr = freq1_Hz + indexArr1Start * BW1_Hz;
            for (int i = 0; arr2corr.Length > i; i++)
            {
                double freq = freq1corr + BW1_Hz * i;
                int index_low = (int)Math.Floor((freq - freq2_Hz) / BW2_Hz);
                if (index_low < 0) { arr2corr[i] = arr2[0]; }
                else if (index_low >= arr2.Length - 1) { arr2corr[i] = arr2[arr2.Length - 1]; }
                else
                {
                    double freqLow = freq2_Hz + BW2_Hz * index_low;
                    arr2corr[i] = (float)(arr2[index_low] + (arr2[index_low + 1] - arr2[index_low]) * (freq - freqLow) / (BW2_Hz));
                }
            }
        }
    }
}
