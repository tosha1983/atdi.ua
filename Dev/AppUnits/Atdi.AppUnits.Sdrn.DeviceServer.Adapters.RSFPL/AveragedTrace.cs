using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MEN = Atdi.DataModels.Sdrn.DeviceServer.Adapters.Enums;

namespace Atdi.AppUnits.Sdrn.DeviceServer.Adapters.RSFPL
{
    public class AveragedTrace
    {
        public int TracePoints;

        public bool CalcAfterAdd;

        public float[] AveragedLevels;

        private double freqStart = 0, freqStep = 0;
        //данные на усреднение, хранятся в миливатах
        //private List<double[]> tracesToAverage;
        private double[] tracesToAverageSumm;
        private double[] tracesToAverage2;
        /// <summary>
        /// Количевство на усреднение меньше двух нечего усреднять, больше 500 долго
        /// </summary>
        public int AveragingCount
        {
            get { return _AveragingCount; }
            set
            {
                if (value < 2)
                {
                    _AveragingCount = 2;
                }
                else if (value > 10000)
                {
                    _AveragingCount = 10000;
                }
                else
                {
                    _AveragingCount = value;
                }
            }
        }
        private int _AveragingCount = 10;

        public MEN.LevelUnit InputlevelUnit { get; private set; }
        public MEN.LevelUnit OutputlevelUnit { get; private set; }

        /// <summary>
        /// текущее значение (сколько трейсов усреднено)
        /// </summary>
        public int NumberOfSweeps { get; private set; }


        public AveragedTrace()
        {
            TracePoints = 500005;
            CalcAfterAdd = false;
            AveragedLevels = new float[TracePoints];
            tracesToAverageSumm = new double[TracePoints];
            //tracesToAverage = new List<double[]> { };//В W
            InputlevelUnit = MEN.LevelUnit.dBm;
            OutputlevelUnit = MEN.LevelUnit.dBm;
            NumberOfSweeps = 0;
        }

        public void AddTraceToAverade(double newFreqStart, double newFreqStep, float[] newTraceLevels, int newTraceLevelsPoints, MEN.LevelUnit inputLevelUnit, MEN.LevelUnit outputLevelUnit)
        {
            //сбросит трейс если че
            if (NumberOfSweeps == 0 || //Если данных еще нет
                TracePoints != newTraceLevelsPoints || //несовпали длины
                newFreqStart != freqStart || //несовпали длины
                newFreqStep != freqStep ||//несовпали начальные частоты                
                InputlevelUnit != inputLevelUnit || //несовпали входные типы уровней
                OutputlevelUnit != outputLevelUnit) //несовпали входные типы уровней
            {
                freqStart = newFreqStart;
                freqStep = newFreqStep;
                InputlevelUnit = inputLevelUnit;
                OutputlevelUnit = outputLevelUnit;
                NumberOfSweeps = 0;
                TracePoints = newTraceLevelsPoints;

                //AveragedLevels = new float[TracePoints];

                for (int i = 0; i < TracePoints; i++)
                {
                    tracesToAverageSumm[i] = newTraceLevels[i];
                }
                NumberOfSweeps = 0;
                if (inputLevelUnit == MEN.LevelUnit.dBm)
                {
                    for (int i = 0; i < TracePoints; i++)
                    {
                        tracesToAverageSumm[i] = (double)(Math.Pow(10, newTraceLevels[i] / 10) / 1000.0);
                    }
                }
                else if (inputLevelUnit == MEN.LevelUnit.dBµV)
                {
                    for (int i = 0; i < TracePoints; i++)
                    {
                        tracesToAverageSumm[i] = (double)(Math.Pow(10, (newTraceLevels[i] - 107) / 10) / 1000.0);
                    }
                }
                else if (inputLevelUnit == MEN.LevelUnit.Watt)
                {
                    for (int i = 0; i < TracePoints; i++)
                    {
                        tracesToAverageSumm[i] = newTraceLevels[i];
                    }
                }
                NumberOfSweeps = 1;
            }
            else if (NumberOfSweeps < _AveragingCount)
            {
                int thisNumberOfSweeps = NumberOfSweeps + 1;
                if (inputLevelUnit == MEN.LevelUnit.dBm)
                {
                    for (int i = 0; i < TracePoints; i++)
                    {
                        tracesToAverageSumm[i] = (tracesToAverageSumm[i] * NumberOfSweeps + Math.Pow(10, newTraceLevels[i] / 10) / 1000.0) / thisNumberOfSweeps;
                    }
                }
                else if (inputLevelUnit == MEN.LevelUnit.dBµV)
                {
                    for (int i = 0; i < TracePoints; i++)
                    {
                        tracesToAverageSumm[i] = (tracesToAverageSumm[i] * NumberOfSweeps + Math.Pow(10, (newTraceLevels[i] - 107) / 10) / 1000.0) / thisNumberOfSweeps;
                    }
                }
                else if (inputLevelUnit == MEN.LevelUnit.Watt)
                {
                    for (int i = 0; i < TracePoints; i++)
                    {
                        tracesToAverageSumm[i] = (tracesToAverageSumm[i] * NumberOfSweeps + newTraceLevels[i]) / thisNumberOfSweeps;
                    }
                }
                NumberOfSweeps = thisNumberOfSweeps;


                if (CalcAfterAdd || NumberOfSweeps == _AveragingCount)
                {
                    Calc();
                }
            }
            if (NumberOfSweeps == _AveragingCount)
            {
                Calc();
            }

        }

        public void Reset()
        {
            NumberOfSweeps = 0;
        }

        private void Calc()
        {
            //NumberOfSweeps = tracesToAverage.Count;

            if (NumberOfSweeps > 1)
            {
                if (OutputlevelUnit == MEN.LevelUnit.dBm)
                {
                    for (int x = 0; x < TracePoints; x++)
                    {
                        AveragedLevels[x] = (float)(10.0 * Math.Log10(1000.0 * tracesToAverageSumm[x]));
                    }
                }
                else if (OutputlevelUnit == MEN.LevelUnit.dBµV)
                {
                    for (int x = 0; x < TracePoints; x++)
                    {

                        AveragedLevels[x] = (float)(107.0 + 10.0 * Math.Log10(1000.0 * tracesToAverageSumm[x]));
                    }
                }
            }
        }
    }
}

