using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.DataModels.Sdrn.DeviceServer.Processing;

namespace Atdi.AppUnits.Sdrn.DeviceServer.Processing.Measurements
{
    static class BandWidthEstimation
    {
        public enum BandwidthEstimationType { beta, xFromCentr, xFromEdge }
        /// <summary>
        /// Function estimate bandwidth according ITU 443 
        /// </summary>
        /// <param name="SpecrtumArrdBm">Array with spectrum in dBm</param>
        /// <param name="bandwidthEstimationType">Bandwidth Estimation Type according ITU 443 beta - Annex1; xFromCentr, xFromEdge - Annex2 </param>
        /// <param name="X_Beta">Present of beta for Bandwidth Estimation Type beta; level x dB for Bandwidth Estimation Type xFromCentr, xFromEdge </param>
        /// <returns>Index of point with start signal, Index of point with end signal, Index of point with maximum Level, Correctness of estimation of bandwidth</returns>
        static public MeasBandwidthResult GetBandwidthPoint(float[] SpecrtumArrdBm, BandwidthEstimationType bandwidthEstimationType = BandwidthEstimationType.beta, double X_Beta = 1, int MaximumIgnorPoint = 1)
        {
            
            double LevelOfSuspiciousJumpdB = 10;
            // end constant
            int T1 = 0; int T2 = 0; int M1 =0 ; bool CorrectEstimation =false;
            switch (bandwidthEstimationType)
            {
                case BandwidthEstimationType.xFromCentr:
                    GetBandwidthPointMethodX(ref SpecrtumArrdBm, X_Beta, out T1, out T2, out M1, out CorrectEstimation, MaximumIgnorPoint, LevelOfSuspiciousJumpdB, true);
                    break;
                case BandwidthEstimationType.xFromEdge:
                    GetBandwidthPointMethodX(ref SpecrtumArrdBm, X_Beta, out T1, out T2, out M1, out CorrectEstimation, MaximumIgnorPoint, LevelOfSuspiciousJumpdB, false);
                    break;
                case BandwidthEstimationType.beta:
                    GetBandwidthPointMethodBeta(ref SpecrtumArrdBm, X_Beta, out T1, out T2, out M1, out CorrectEstimation, MaximumIgnorPoint, LevelOfSuspiciousJumpdB);
                    break;
            }
            MeasBandwidthResult measSdrBandwidthResults = new MeasBandwidthResult
            {
                T1 = T1,
                T2 = T2,
                MarkerIndex = M1,
                СorrectnessEstimations = CorrectEstimation
            };
            return (measSdrBandwidthResults);
        }
        static private void GetBandwidthPointMethodBeta(ref float[] SpecrtumArrdBm, double beta, out int T1, out int T2, out int M1, out bool CorrectEstimation, int MaximumIgnorPoint, double LevelOfSuspiciousJumpdB)
        { // тестированно Макисм 21.08.2018.
            T1 = 0; T2 = SpecrtumArrdBm.Length -1; M1 = 0; CorrectEstimation = false;

            double SumPow = 0; //int imax = 0;
            // проверка возможности использовать данный метод по уровню 30дБ, а также определение общей суммы 
            CorrectEstimation = CheckCorrectionInputSignalForBandwidthEstimation(ref SpecrtumArrdBm, MaximumIgnorPoint, LevelOfSuspiciousJumpdB, out SumPow, out M1);
            // суть метода мы идем от края до края шаг за шагом оценивая сумму эелементов (т.е. площадь елементов) Смысл найти позицию где у нас будет минимальная BW
            double TriggerPowmW = Math.Pow(10, SumPow / 10) * (100 - beta) / 100;
            double currentSummW = 0; int currentT1 = 0; int currentT2 = 0; 
            for (int i = 0; i < SpecrtumArrdBm.Length; i++)
            {
                currentSummW = currentSummW + Math.Pow(10, SpecrtumArrdBm[i] / 10.0);
                currentT2 = i;
                if (currentSummW > TriggerPowmW)
                {// только что превысили
                    // начинаем отнимать с конца 
                    do
                    {
                        currentSummW = currentSummW - Math.Pow(10, SpecrtumArrdBm[currentT1] / 10.0);
                        currentT1++;
                    }
                    while (currentSummW >= TriggerPowmW);
                    if (T2 - T1 > currentT2 - currentT1)
                    {
                        T2 = currentT2; T1 = currentT1;
                    }
                }
            }

        }
        static private void GetBandwidthPointMethodX(ref float[] SpecrtumArrdBm, double x, out int T1, out int T2, out int M1, out bool CorrectEstimation, int MaximumIgnorPoint, double LevelOfSuspiciousJumpdB, bool FindFromCentr)
        {
            double SumPow = 0; //int imax = 0;
            // проверка возможности использовать данный метод по уровню 30дБ, а также определение общей суммы 
            CorrectEstimation = CheckCorrectionInputSignalForBandwidthEstimation(ref SpecrtumArrdBm, MaximumIgnorPoint, LevelOfSuspiciousJumpdB, out SumPow, out M1);
            double TrigerLeveldBm = SpecrtumArrdBm[M1] - x;
            //int cT1, cT2, eT1, eT2;
            if (FindFromCentr)
            {
                GetBandwidthPointMethodXFromTheCenter(ref SpecrtumArrdBm, M1, TrigerLeveldBm, MaximumIgnorPoint, out T1, out T2);
            }
            else
            {
                GetBandwidthPointMethodXFromTheEdge(ref SpecrtumArrdBm, M1, TrigerLeveldBm, MaximumIgnorPoint, out T1, out T2);
            }

            
            
        }
        static private void GetBandwidthPointMethodXFromTheCenter(ref float[] SpecrtumArrdBm, int M1, double TrigerLevelmW, int MaximumIgnorPoint, out int T1, out int T2)
        {
            T1 = 0; T2 = SpecrtumArrdBm.Length - 1;
            MoveInArr(ref SpecrtumArrdBm, TrigerLevelmW, M1, 0, MaximumIgnorPoint, true, out T1);
            MoveInArr(ref SpecrtumArrdBm, TrigerLevelmW, M1, SpecrtumArrdBm.Length-1, MaximumIgnorPoint, true, out T2);
        }
        static private void GetBandwidthPointMethodXFromTheEdge(ref float[] SpecrtumArrdBm, int M1, double TrigerLeveldBm, int MaximumIgnorPoint, out int T1, out int T2)
        {
            T1 = 0; T2 = SpecrtumArrdBm.Length - 1;
            MoveInArr(ref SpecrtumArrdBm, TrigerLeveldBm, 0, M1, MaximumIgnorPoint, false, out T1);
            MoveInArr(ref SpecrtumArrdBm, TrigerLeveldBm, SpecrtumArrdBm.Length - 1, M1, MaximumIgnorPoint, false, out T2);
        }
        static private void MoveInArr(ref float[] SpecrtumArrdBm, double TrigerLeveldBm, int from, int to, int MaximumIgnorPoint, bool downTrigLevel, out int point)
        { // Идем по масиву от from до to пока не встретем падение или возрастание уровня 
            point = to; int step = 1; bool conv = true;
            if (from > to) { step = -1; conv = false; }
            int countPoint = 1;
            for (int i = from; ((i <= to) && (conv)) || ((i >= to) && (!conv)); i = i + step)
            {
                if (((TrigerLeveldBm > SpecrtumArrdBm[i]) && (downTrigLevel)) || ((TrigerLeveldBm < SpecrtumArrdBm[i]) && (!downTrigLevel)))
                {
                    if (countPoint > MaximumIgnorPoint)
                    {
                        point = i - step * (countPoint - 1);
                        break;
                    }
                    else
                    {
                        countPoint++;
                    }
                }
                else
                {
                    countPoint = 1;
                }
            }

        }
        static private bool CheckCorrectionInputSignalForBandwidthEstimation(ref float[] SpecrtumArrdBm, int MaximumIgnorPoint, double LevelOfSuspiciousJumpdB, out double SumPow, out int imax)
        {
            // constant 
            double LevelDiffForChackCorrection = 30;

            imax = 0; SumPow = 0;
            CalcSumAndMaxFromArry(ref SpecrtumArrdBm, MaximumIgnorPoint, LevelOfSuspiciousJumpdB, out SumPow, out imax);

            if (SpecrtumArrdBm.Length < MaximumIgnorPoint + 2)
                return false;

            // расчет области вначале 
            double SumStart = 0; double SumFinish = 0;
            for (int i = 0; i < MaximumIgnorPoint + 1; i++)
            {

                SumStart = SumStart + Math.Pow(10, SpecrtumArrdBm[i] / 10.0);
                SumFinish = SumFinish + Math.Pow(10, SpecrtumArrdBm[SpecrtumArrdBm.Length - 1 - i] / 10.0);
            }
            SumStart = 10.0 * Math.Log10(SumStart / (MaximumIgnorPoint + 1));
            SumFinish = 10.0 * Math.Log10(SumFinish / (MaximumIgnorPoint + 1));
            if (((SpecrtumArrdBm[imax] - SumStart) > LevelDiffForChackCorrection) && ((SpecrtumArrdBm[imax] - SumFinish) > LevelDiffForChackCorrection))
            {
                return true;
            }
            return false;
        }
        static private void CalcSumAndMaxFromArry(ref float[] SpecrtumArrdBm, int MaximumIgnorPoint, double LevelOfSuspiciousJumpdB, out double SumPow, out int imax)
        { // не отлаженно на поиск скачков, не отлаженно без скачков

            SumPow = Math.Pow(10, SpecrtumArrdBm[0] / 10.0);
            imax = 0;
            double MaxLeveldBm = SpecrtumArrdBm[0];
            for (int i = 1; i < SpecrtumArrdBm.Length; i++)
            {
                SumPow = SumPow + Math.Pow(10, SpecrtumArrdBm[i] / 10.0);
                if (MaxLeveldBm < SpecrtumArrdBm[i])
                {
                    if (SpecrtumArrdBm.Length > i + MaximumIgnorPoint + 1) // условие что это у нас не конец масива
                    {
                        if ((SpecrtumArrdBm[i] - SpecrtumArrdBm[i - 1]) > LevelOfSuspiciousJumpdB)
                        {
                            // Т.е. резкий скачек теперь проверяем что он единичный
                            bool Jump = false;
                            for (int j = i; j <= i + MaximumIgnorPoint; j++)
                            {
                                if ((SpecrtumArrdBm[j] - SpecrtumArrdBm[j + 1]) > LevelOfSuspiciousJumpdB)
                                {
                                    Jump = true;
                                    break;
                                }
                            }
                            if (!Jump)
                            {
                                MaxLeveldBm = SpecrtumArrdBm[i];
                                imax = i;
                            }
                        }
                        else
                        {
                            MaxLeveldBm = SpecrtumArrdBm[i];
                            imax = i;
                        }
                    }
                }
            }
            SumPow = 10.0 * Math.Log10(SumPow);
        }
    }
}
