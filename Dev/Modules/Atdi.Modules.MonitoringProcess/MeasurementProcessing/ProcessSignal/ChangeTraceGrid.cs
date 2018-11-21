using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.AppServer.Contracts.Sdrns;

namespace Atdi.SDR.Server.MeasurementProcessing.SingleHound.ProcessSignal
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
                CountNewFreq = (int)Math.Floor((StartOldFreq_MHz - (OldStep_MHz / 2.0) - StartNewFreq_MHz - (NewStep_MHz / 2.0)) / NewStep_MHz);
                for (int i = 0; i<CountNewFreq; i++)
                {
                    NewLevels[i] = LevelForEmptySteps_dBm_Hz + NewStep_dBHz;
                }
                NewLevels[CountNewFreq] = LevelForEmptySteps_dBm_Hz + 10 * Math.Log10(StartOldFreq_MHz - (OldStep_MHz / 2.0) - (StartNewFreq_MHz + ((CountNewFreq - 0.5) * NewStep_MHz))) + 30;
            }
            else
            {
                CountOldFreq = (int)Math.Floor((StartNewFreq_MHz - (NewStep_MHz / 2.0) - StartOldFreq_MHz - (OldStep_MHz / 2.0)) / OldStep_MHz);
            }
            while ((CountNewFreq >= PointsInNewLevelsArr)|| (CountOldFreq >= Levels.Length))
            {
 //               CurLevel_mW = 
            }
            

            return null;
        }
    }
}
