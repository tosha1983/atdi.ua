﻿using Atdi.Contracts.Sdrn.DeviceServer;
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
    public static class EmissionCounting
    {   /// <summary>
        /// По заддоному масиву Level_dBm функция возвращает в масив StartStop значения начала и конца излучений. 
        /// </summary>
        /// <param name="Level_dBm"></param>
        /// <param name="PointStart"></param>
        /// <param name="PointStop"></param>
        /// <param name="StartStop"></param>
        /// <param name="DifferenceMaxMax"></param>
        /// <param name="FiltrationTrace"></param>
        /// <returns></returns>
        public static int Counting(float[] Level_dBm, int length, int PointStart, int PointStop, out int[] StartStop, double DifferenceMaxMax = 20, bool FiltrationTrace = true)
        { // НЕ ТЕСТИРОВАЛОСЬ
          // проверка коректности принимаемых данных
            if (PointStop < PointStart) { int T = PointStop; PointStop = PointStart; PointStart = T; }
            if (PointStart < 0) { PointStart = 0; }
            if (PointStop > length - 1) { PointStop = length - 1; }
            if (DifferenceMaxMax < 5) { DifferenceMaxMax = 5; }
            // конец проверки корректности 

            var Level = new float[PointStop - PointStart + 1];
            Array.Copy(Level_dBm, PointStart, Level, 0, PointStop - PointStart + 1);
            if (FiltrationTrace) { Level = SmoothTrace.blackman(Level, Level.Length, false); } // провели сглаживание массива

            double LocMax1; int IndexLocMax1;
            double LocMin1; int IndexLocMin1;
            bool gotoMax;

            var MinMax = new List<int>();

            LocMin1 = Level[0]; IndexLocMin1 = 0;
            LocMax1 = Level[0]; IndexLocMax1 = 0;
            gotoMax = true;

            for (var i = 1; Level.Length > i; i++)
            {
                if (gotoMax)
                {
                    if (Level[i] > LocMax1)
                    {
                        LocMax1 = Level[i]; IndexLocMax1 = i;
                    }
                    if (Level[i] < LocMin1)
                    {
                        LocMin1 = Level[i]; IndexLocMin1 = i;
                    }
                    if (LocMax1 - LocMin1 >= DifferenceMaxMax)
                        if ((IndexLocMin1 < IndexLocMax1))
                        { // мы превысили уровень. Теперь точно можно зафиксировать минимум. 
                            if (MinMax.Count == 0)
                            {
                                MinMax.Add(IndexLocMin1);
                            }
                            else
                            {
                                MinMax.Add(IndexLocMin1);
                                MinMax.Add(IndexLocMin1);
                            }
                            LocMin1 = Level[i]; IndexLocMin1 = i;
                            gotoMax = false;
                        }
                        else
                        {
                            LocMax1 = Level[i]; IndexLocMax1 = i;
                        }
                }
                else
                {
                    if (Level[i] > LocMax1)
                    {
                        LocMax1 = Level[i]; IndexLocMax1 = i;
                    }
                    if (Level[i] < LocMin1)
                    {
                        LocMin1 = Level[i]; IndexLocMin1 = i;
                    }
                    if (LocMax1 - LocMin1 >= DifferenceMaxMax)
                    {
                        if (IndexLocMin1 > IndexLocMax1)
                        {
                            // мы превысили уровень. Теперь точно можно зафиксировать максимум. 
                            LocMax1 = Level[i]; IndexLocMax1 = i;
                            gotoMax = true;
                        }
                        else
                        {
                            LocMin1 = Level[i]; IndexLocMin1 = i;
                        }
                    }
                }
            }
            if (MinMax.Count == 0)
            {
                MinMax.Add(0); MinMax.Add(Level.Length - 1);
            }
            else
            {
                if (gotoMax) { MinMax.Add(IndexLocMin1); }
                else { MinMax.RemoveAt(MinMax.Count - 1); }
            }
            StartStop = new int[MinMax.Count];
            for (var i = 0; MinMax.Count > i; i++)
            {
                StartStop[i] = MinMax[i] + PointStart;
            }
            return (int)(StartStop.Length / 2);
        }
    }
}
