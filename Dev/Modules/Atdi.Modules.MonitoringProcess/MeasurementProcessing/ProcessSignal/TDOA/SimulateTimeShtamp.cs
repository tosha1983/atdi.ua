using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Modules.MonitoringProcess.ProcessSignal
{
    // Исключительно для симуляции похожих TimeShtamp
    public class SimulateTimeShtamp
    {

        public TimeStampBlock SimulateTimeStamp(TimeStampBlock timeStampBlock, int ShiftIndex, int IndexNoise, double PhaseNoise, double PercentExeption)
        {
            TimeStampBlock NewTimeStampBlock  = new TimeStampBlock();
            List<double> Phases = timeStampBlock.RotationPhase;
            List<double> NewPhases = new List<double>();
            List<int> Index= timeStampBlock.RotationIndex;
            List<int> NewIndexs = new List<int>();
            Random rnd = new Random();
            for (int i = 0; i < Index.Count; i++)
            {
                int NewInd = Index[i]+ ShiftIndex + (int)(IndexNoise*(1 - 2*rnd.NextDouble()));
                double NewPhase = Phases[i] + PhaseNoise*(1 - 2 * rnd.NextDouble());
                if (rnd.NextDouble() * 100 < PercentExeption)
                {
                    if (rnd.NextDouble() > 0.5)
                    {
                        NewIndexs.Add(NewInd);
                        NewPhases.Add(NewPhase);
                        // добавляем новый 
                        if ((i != 0) && (i != Index.Count-1))
                        {
                            NewIndexs.Add(0);
                            NewPhases.Add(0);
                        }
                    }
                }
                else
                {
                    NewIndexs.Add(NewInd);
                    NewPhases.Add(NewPhase);
                }
            }
            for (int i = 1; i < NewIndexs.Count-1; i++)
            {
                if (NewIndexs[i] == 0)
                {
                    NewIndexs[i] = rnd.Next(NewIndexs[i - 1], NewIndexs[i + 1]);
                    NewPhases[i] = rnd.Next(0, 359);
                }
            }
            NewTimeStampBlock.RotationIndex = NewIndexs;
            NewTimeStampBlock.RotationPhase = NewPhases;
            NewTimeStampBlock.DurationBlockmks = timeStampBlock.DurationBlockmks;
            NewTimeStampBlock.StartIndexOfBlock = timeStampBlock.StartIndexOfBlock;
            NewTimeStampBlock.TimeStampToneParameters = timeStampBlock.TimeStampToneParameters;
            return NewTimeStampBlock;
        }
    }
}
