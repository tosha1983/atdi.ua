using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using Atdi.AppServer.Contracts.Sdrns;

namespace Atdi.Modules.MonitoringProcess.ProcessSignal
{
    static public class ChangeTraceGrid
    {
        /// <summary>
        /// Function for change freq grid
        /// </summary>
        /// <param name="Levels">Arr with levels in dBm</param>
        /// <param name="StartOldFreq_MHz">Central Freq</param>
        /// <param name="StartOldStep_kHz"></param>
        /// <param name="StartNewFreq_MHz">Central Freq</param>
        /// <param name="StartNewStep_kHz"></param>
        /// <returns></returns>
        static public double[] ChangeGrid(ref double[] Levels, double StartOldFreq_MHz, double OldStep_kHz, double StartNewFreq_MHz, double NewStep_kHz, int PointsInNewLevelsArr, double LevelForEmptySteps_dBm_Hz = -158)
        {
            int CountOldFreq = 0;
            int CountNewFreq = 0;
            double[] NewLevels = new double[PointsInNewLevelsArr];
            for (int i = 0; i < PointsInNewLevelsArr; i++) { NewLevels[i] = -99999; }
            double CurLevel_mW = 0;
            double OldStep_MHz = OldStep_kHz / 1000.0;
            double NewStep_MHz = NewStep_kHz / 1000.0;
            double NewStep_dBHz = 10 * Math.Log10(NewStep_kHz) + 30;

            // простановка стартовых индексов идем от меньших к большим
            if ((StartNewFreq_MHz - (NewStep_MHz / 2.0)) <= (StartOldFreq_MHz - (OldStep_MHz / 2.0)))
            {
                // новый массив необходимо заполнить от начала 
                CountNewFreq = (int)Math.Floor((StartOldFreq_MHz - (OldStep_MHz / 2.0) - StartNewFreq_MHz + (NewStep_MHz / 2.0)) / NewStep_MHz);
                for (int i = 0; i < CountNewFreq; i++)
                {
                    NewLevels[i] = LevelForEmptySteps_dBm_Hz + NewStep_dBHz;
                }
                NewLevels[CountNewFreq] = LevelForEmptySteps_dBm_Hz + 10 * Math.Log10(StartOldFreq_MHz - (OldStep_MHz / 2.0) - (StartNewFreq_MHz + ((CountNewFreq - 0.5) * NewStep_MHz))) + 60;
            }
            else
            {
                CountOldFreq = (int)Math.Floor((StartNewFreq_MHz - (NewStep_MHz / 2.0) - StartOldFreq_MHz + (OldStep_MHz / 2.0)) / OldStep_MHz);
            }

            double LowNewFreq_MHz = StartNewFreq_MHz + ((CountNewFreq - 0.5) * NewStep_MHz);
            double UpNewFreq_MHz = LowNewFreq_MHz + NewStep_MHz;
            double LowOldFreq_MHz = StartOldFreq_MHz + ((CountOldFreq - 0.5) * OldStep_MHz);
            double UpOldFreq_MHz = LowOldFreq_MHz + OldStep_MHz;
            double DeltaFreqInterseption_MHz;
            CurLevel_mW = Math.Pow(10, NewLevels[CountNewFreq] / 10);

            while ((CountNewFreq < PointsInNewLevelsArr) && (CountOldFreq < Levels.Length))
            {
                //Расчет полосы пересечения 
                if (UpOldFreq_MHz > UpNewFreq_MHz)
                {
                    DeltaFreqInterseption_MHz = UpNewFreq_MHz - Math.Max(LowNewFreq_MHz, LowOldFreq_MHz);
                    CurLevel_mW = CurLevel_mW + (Math.Pow(10, Levels[CountOldFreq] / 10) * DeltaFreqInterseption_MHz / OldStep_MHz);
                    NewLevels[CountNewFreq] = 10 * Math.Log10(CurLevel_mW);
                    CurLevel_mW = 0;
                    CountNewFreq++;
                    LowNewFreq_MHz = UpNewFreq_MHz;
                    UpNewFreq_MHz = UpNewFreq_MHz + NewStep_MHz;
                }
                else
                {
                    DeltaFreqInterseption_MHz = UpOldFreq_MHz - Math.Max(LowNewFreq_MHz, LowOldFreq_MHz);
                    CurLevel_mW = CurLevel_mW + (Math.Pow(10, Levels[CountOldFreq] / 10) * DeltaFreqInterseption_MHz / OldStep_MHz);
                    CountOldFreq++;
                    LowOldFreq_MHz = UpOldFreq_MHz;
                    UpOldFreq_MHz = UpOldFreq_MHz + OldStep_MHz;
                }
            }
            if (CountNewFreq < PointsInNewLevelsArr)
            {
                DeltaFreqInterseption_MHz = UpNewFreq_MHz - LowOldFreq_MHz;
                NewLevels[CountNewFreq] = 10 * Math.Log10(CurLevel_mW + DeltaFreqInterseption_MHz * Math.Pow(10, (LevelForEmptySteps_dBm_Hz+60) / 10));
                CountNewFreq++;
                for (int i = CountNewFreq; i < PointsInNewLevelsArr; i++)
                {
                    NewLevels[i] = LevelForEmptySteps_dBm_Hz + NewStep_dBHz;
                }
            }
            return NewLevels;
        }
    }
}
