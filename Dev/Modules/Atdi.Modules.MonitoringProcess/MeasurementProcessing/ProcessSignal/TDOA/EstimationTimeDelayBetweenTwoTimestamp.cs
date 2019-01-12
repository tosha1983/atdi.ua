using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using Atdi.Modules.MonitoringProcess.MeasurementProcessing;

namespace Atdi.Modules.MonitoringProcess.ProcessSignal
{
    public class EstimationTimeDelayBetweenTwoTimestamp
    {
        public double Delay_s;
        public GetTimeStamp.TypeTechnology SignalTechnology;
        public EstimationTimeDelayBetweenTwoTimestamp(IQStreamTimeStampBloks IQStreamTimeStampBloks1, IQStreamTimeStampBloks IQStreamTimeStampBloks2)
        {
            //ComparerTwoBlockSignals(IQStreamTimeStampBloks1.TimeStampBlocks[0], IQStreamTimeStampBloks2.TimeStampBlocks[0], 15);
            SimulateTimeShtamp simulateTimeShtamp = new SimulateTimeShtamp();
            double MetricAngle = 0;
            double MetricIndex = 0;
            //TimeStampBlock WrongTimeStampBlock = simulateTimeShtamp.SimulateTimeStamp(IQStreamTimeStampBloks1.TimeStampBlocks[0], 30, 15, 10, 0);
            //ComparerTwoBlockSignals(IQStreamTimeStampBloks1.TimeStampBlocks[0], WrongTimeStampBlock, 0, out MetricAngle, out MetricIndex);
            List<double> MetricAngles;
            List<double> MetricIndexs;
            List<double> BestList = new List<double>();
            List<double> BestListAngle = new List<double>();
            List<int> IndexPoint = new List<int>();
            List<int> IndexOtherBloks = new List<int>();
            List<double> TimeDiff = new List<double>();
            double best = 999999;
            int best_j = -1;
            int best_i = -1;
            double best_angle = -1;
            int best_timeshift = -1;
            int PPSdiff = IQStreamTimeStampBloks1.IndexPPS[0] - IQStreamTimeStampBloks2.IndexPPS[0];
            for (int k = 0; k < IQStreamTimeStampBloks1.TimeStampBlocks.Count; k++)
            {

                for (int j = 0; j < IQStreamTimeStampBloks2.TimeStampBlocks.Count; j++)
                {
                    // подбор блоков исходя из возможной дистанции до 1000 км 
                    int MaxDistanceInSemples = (int) ((1000.0 * 1000.0 / (3.0 * 100000000.0)) * IQStreamTimeStampBloks1.SemplePerSecond);
                    int index_min_block1 = IQStreamTimeStampBloks1.TimeStampBlocks[k].StartIndexOfBlock - PPSdiff; 
                    int index_max_block1 = index_min_block1 + IQStreamTimeStampBloks1.TimeStampBlocks[k].RotationIndex[IQStreamTimeStampBloks1.TimeStampBlocks[k].RotationIndex.Count-1] - PPSdiff;
                    int index_min_block2 = IQStreamTimeStampBloks2.TimeStampBlocks[j].StartIndexOfBlock;
                    int index_max_block2 = index_min_block2 + IQStreamTimeStampBloks2.TimeStampBlocks[j].RotationIndex[IQStreamTimeStampBloks2.TimeStampBlocks[j].RotationIndex.Count - 1];
                    bool hit = false;
                    if (index_min_block1 > index_min_block2)
                    {
                        if (index_max_block2 + MaxDistanceInSemples > index_min_block1) { hit = true; }
                    }
                    else
                    {
                        if (index_max_block1 + MaxDistanceInSemples > index_min_block2) { hit = true; }
                    }

                    if (hit)
                    {
                        MetricAngles = new List<double>();
                        MetricIndexs = new List<double>();
                        for (int i = 0; i < 1000; i++)
                        {
                            int shift = i - 500;
                            //if (k != j)
                            //{
                            //if (i == 334)
                            //{
                            //    int kkk = 0;
                            //}
                            ComparerTwoBlockSignals(IQStreamTimeStampBloks1.TimeStampBlocks[k], IQStreamTimeStampBloks2.TimeStampBlocks[j], shift, out MetricAngle, out MetricIndex);
                            if (MetricIndex < best)
                            {
                                best = MetricIndex;
                                best_j = j;
                                best_i = i;
                                best_angle = MetricAngle;
                                best_timeshift = IQStreamTimeStampBloks1.TimeStampBlocks[k].StartIndexOfBlock + shift - IQStreamTimeStampBloks2.TimeStampBlocks[j].StartIndexOfBlock - PPSdiff*2;
                            }
                            MetricAngles.Add(MetricAngle);
                            MetricIndexs.Add(MetricIndex);
                            //}
                        }
                    }
                }
                BestList.Add(best);
                BestListAngle.Add(best_angle);
                IndexPoint.Add(best_i);
                IndexOtherBloks.Add(best_j);
                TimeDiff.Add(best_timeshift);
;
                best = 999999;
            }
        }
        /// <summary>
        /// Производиться сравнение двух блоков результат степень корреляции и время рассинхронизации
        /// </summary>
        /// <param name="timeStampBlock1"></param>
        /// <param name="timeStampBlock2"></param>
        private void ComparerTwoBlockSignals(TimeStampBlock timeStampBlock1, TimeStampBlock timeStampBlock2, int SempleShift, out double SumMetricAngle, out double SumMetricIndex)
        {
            //Константы 
            int IndexAccuracy = 1;
            double AngleAccuracy = 1;
            int count_for_correlation = 4;
            List<double> MetricAngle = new List<double>(); // метрика штрафов Пусть это будет разность углов прыжка
            List<int> MetricIndex = new List<int>(); // метрика штрафов Пусть это будет разница между пиками
            for (int i = 0; i < timeStampBlock1.RotationIndex.Count - 1; i++)
            {// Идем по индексам главного TimeShtamp
                int val_rotation_index1 = timeStampBlock1.RotationIndex[i];
                int val_rotation_index2 = timeStampBlock1.RotationIndex[i+1];
                int j1 = CalcDiffIndex(timeStampBlock2.RotationIndex, val_rotation_index1 + SempleShift); // индекс ближайшего хита другой последовательности
                if (j1 == -1) { MetricAngle.Add(-1); MetricIndex.Add(-1); }
                else
                {
                    int j2 = CalcDiffIndex(timeStampBlock2.RotationIndex, val_rotation_index2 + SempleShift);
                    if (j2 == -1) { MetricAngle.Add(-1); MetricIndex.Add(-1); }
                    else
                    {
                        double val_rotation_main = CalcAngleRotation(timeStampBlock1.RotationPhase[i], timeStampBlock1.RotationPhase[i + 1]);
                        double val_rotation_second = CalcAngleRotation(timeStampBlock2.RotationPhase[j1], timeStampBlock2.RotationPhase[j2]);
                        double MetrAngle = Math.Abs(val_rotation_main - val_rotation_second); if (MetrAngle <= AngleAccuracy) { MetrAngle = 0;}
                        MetricAngle.Add(MetrAngle);
                        int MetrIndex = Math.Abs(val_rotation_index1 + SempleShift - timeStampBlock2.RotationIndex[j1])+ Math.Abs(val_rotation_index2 + SempleShift - timeStampBlock2.RotationIndex[j2]);
                        if (MetrIndex <= IndexAccuracy) { MetrIndex = 0; }
                        MetricIndex.Add(MetrIndex);
                    }
                }
            }
            // результат два масива метрик 
            int count = 0;
            SumMetricAngle = 0; 
            for (int i = 0; i < MetricAngle.Count; i++)
            {
                if (MetricAngle[i] != -1) { SumMetricAngle = SumMetricAngle + MetricAngle[i];
                    count++;
                }
            }
            if (count > count_for_correlation) { SumMetricAngle = SumMetricAngle / count; }
            else { SumMetricAngle = 9999999; }
            count = 0;
            SumMetricIndex = 0;
            for (int i = 0; i < MetricAngle.Count; i++)
            {
                if (MetricIndex[i] != -1) { SumMetricIndex = SumMetricIndex + MetricIndex[i]; count++; }
            }
            if (count > count_for_correlation) { SumMetricIndex = SumMetricIndex / count; }
            else { SumMetricIndex = 9999999; }
        }
        private double CalcAngleRotation(double angl1, double angl2)
        {
            double res = angl2 - angl1;
            if (res < 0) { res = -res;}
            if (res > 180) { res = 360 - res; }
            return res;
        }
        private int CalcDiffIndex(List<int>RotationIndex, int val)
        {
            int res = -1;
            if (val > RotationIndex[RotationIndex.Count - 1]) { return res; }
            for (int i = 0; i < RotationIndex.Count-1; i++)
            {
                if (RotationIndex[i] <= val)
                {
                    if (RotationIndex[i + 1] >= val)
                    {
                        if ((val - RotationIndex[i]) > (RotationIndex[i + 1] - val))
                        {
                            res = i + 1;
                        }
                        else
                        {
                            res = i;
                        }
                        return res;
                    }
                }
                else
                {
                    return res;
                }
            }
            return res;
        }


    }
}
