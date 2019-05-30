using Atdi.Contracts.Sdrn.DeviceServer;
using Atdi.DataModels.Sdrn.DeviceServer.Processing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.DataModels.Sdrns.Device;
using Atdi.DataModels.Sdrn.DeviceServer;
using Atdi.DataModels.Sdrn.DeviceServer.Commands.Parameters;
using Atdi.DataModels.Sdrn.DeviceServer.Commands.Results;

namespace Atdi.AppUnits.Sdrn.DeviceServer.Processing.Measurements
{
    public static class CalcBWSignalization
    {
        // коснстанты 
        private const double fluctuationCoef = 20;
        // конец констант


        public static bool CalcBW(float[] levels, int start, int stop, double nDbLevel_dB, double NoiseLevel_dBm, double MinExcessNoseLevel_dB, int NumberIgnoredPoints, ref int IndexStart, ref int IndexStop)
        {
            IndexStart = SearchEdgeIndex(levels, start, stop, nDbLevel_dB, NoiseLevel_dBm, MinExcessNoseLevel_dB, false, NumberIgnoredPoints);
            IndexStop = SearchEdgeIndex(levels, start, stop, nDbLevel_dB, NoiseLevel_dBm, MinExcessNoseLevel_dB, true, NumberIgnoredPoints);
            if ((IndexStart == -1) || (IndexStop == -1)) { return false; }
            return true;
        }
        /// <summary>
        /// Ищем индекс масива где наблюдается уровень nDbLevel_dB относительно максимального значения. Уровень должен продержаться не менее точек чем NumberIgnoredPoints
        /// Возращаем -1 если это не удалось
        /// </summary>
        /// <param name="levels"></param>
        /// <param name="start"></param>
        /// <param name="stop"></param>
        /// <param name="nDbLevel_dB"></param>
        /// <param name="NoiseLevel_dBm"></param>
        /// <param name="MinExcessNoseLevel_dB"></param>
        /// <param name="MoveRight"></param>
        /// <param name="NumberIgnoredPoints"></param>
        /// <returns></returns>
        private static int SearchEdgeIndex(float[] levels, int start, int stop, double nDbLevel_dB, double NoiseLevel_dBm, double MinExcessNoseLevel_dB, bool MoveRight, int NumberIgnoredPoints)
        {
            // конверсия если перепутали местами
            if (start > stop)
            {
                int temp = stop;
                stop = start;
                start = temp;
            }
            // определим уровень флуктуаций на графике
            double everage_fluct = 0;
            for (int i = start; i < stop; i++)
            {
                everage_fluct = everage_fluct + Math.Abs(levels[i + 1] - levels[i]);
            }
            everage_fluct = everage_fluct / (stop - start);
            // найдем максимальный уровень излучения в диапазоне притом исключая резкие скачки
            int index_max;
            double level_max;
            if (levels[start] > levels[stop])
            {
                index_max = start;
                level_max = levels[start];
            }
            else
            {
                index_max = stop;
                level_max = levels[stop];
            }
            for (int i = start + 1; i <= stop - 1; i++)
            {
                if (levels[i] >= level_max)
                {
                    double difference;
                    if (Math.Abs(levels[i] - levels[i - 1]) > Math.Abs(levels[i] - levels[i + 1]))
                    { difference = Math.Abs(levels[i] - levels[i + 1]); }
                    else { difference = Math.Abs(levels[i] - levels[i - 1]); }
                    if (difference < fluctuationCoef * everage_fluct)
                    {
                        index_max = i;
                        level_max = levels[i];
                    }

                }
            }
            // определяем достаточно ли максимума для шума
            if (level_max - nDbLevel_dB < NoiseLevel_dBm + MinExcessNoseLevel_dB) { return -1; }
            {
            }
            // идем от максимума по направлению 
            int k = -1;
            int limit = 0;
            if (MoveRight) { k = 1; limit = levels.Length - 1; }
            int CountPoint = 0;
            for (int i = index_max; i * k <= limit * k; i = i + k)
            {// цикл обеспечивающий движение по спектру в нужную сторону от максимального уровня
                if (levels[i] < level_max - nDbLevel_dB)
                {
                    CountPoint++;
                    if (CountPoint > NumberIgnoredPoints)
                    {
                        return (i - k * CountPoint);
                    }
                }
                else
                {
                    CountPoint = 0;
                }
            }
            if (CountPoint > 0) { return limit; }
            return -1;
        }
    }
}
