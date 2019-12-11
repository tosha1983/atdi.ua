using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MEN = Atdi.DataModels.Sdrn.DeviceServer.Adapters.Enums;

namespace Atdi.AppUnits.Sdrn.DeviceServer.Adapters.SignalHound
{
    public class AveragedTrace
    {
        private int tracePoints;

        public bool CalcAfterAdd;

        public float[] AveragedLevels;

        private double freqStart = 0, freqStep = 0;
        //private double[] Freqs;
        //данные на усреднение, хранятся в миливатах
        private List<float[]> tracesToAverage;

        /// <summary>
        /// Количевство на усреднение меньше двух нечего усреднять, больше 500 долго
        /// </summary>
        public int AveragingCount
        {
            get { return averagingCount; }
            set
            {
                if (value < 2)
                {
                    averagingCount = 2;
                }
                else if (value > 500)
                {
                    averagingCount = 500;
                }
                else
                {
                    averagingCount = value;
                }
            }
        }
        private int averagingCount = 10;

        public MEN.LevelUnit LevelUnit { get; private set; }

        /// <summary>
        /// текущее значение (сколько трейсов усреднено)
        /// </summary>
        public int NumberOfSweeps { get; private set; }


        public AveragedTrace()
        {
            tracePoints = 1601;
            CalcAfterAdd = false;
            
            tracesToAverage = new List<float[]> { };
            LevelUnit = MEN.LevelUnit.dBm;
            NumberOfSweeps = 0;
        }

        public void AddTraceToAverade(double newFreqStart, double newFreqStep, float[] newTraceLevels, MEN.LevelUnit levelUnit)
        {
            //сбросит трейс если че
            if (tracesToAverage.Count == 0 || //Если данных еще нет
                newTraceLevels.Length != tracesToAverage[0].Length || //несовпали длины
                newFreqStart != freqStart ||//несовпали начальные частоты
                newFreqStep != freqStep || //несовпали шаги частоты
                LevelUnit != levelUnit) //несовпали типы уровней
            {
                freqStart = newFreqStart;
                freqStep = newFreqStep;
                LevelUnit = levelUnit;
                tracesToAverage.Clear();
                NumberOfSweeps = 0;
                tracePoints = newTraceLevels.Length;

                
                AveragedLevels = new float[tracePoints];

                for (int i = 0; i < tracePoints; i++)
                {
                    AveragedLevels[i] = newTraceLevels[i];
                }
                NumberOfSweeps = 0;
                //добавить в TracesToAverage новый элемент пересчитаный
                float[] tr = new float[tracePoints];
                if (LevelUnit == MEN.LevelUnit.dBm)
                {
                    for (int i = 0; i < tracePoints; i++)
                    {
                        tr[i] = (float)Math.Pow(10, newTraceLevels[i] / 10);
                    }
                }
                else if (LevelUnit == MEN.LevelUnit.dBµV)
                {
                    for (int i = 0; i < tracePoints; i++)
                    {
                        tr[i] = (float)Math.Pow(10, (newTraceLevels[i] - 107) / 10);
                    }
                }
                tracesToAverage.Add(tr);
            }
            else if (tracesToAverage.Count < averagingCount)
            {
                float[] tr = new float[tracePoints];
                if (LevelUnit == MEN.LevelUnit.dBm)
                {
                    for (int i = 0; i < tracePoints; i++)
                    {
                        tr[i] = (float)Math.Pow(10, newTraceLevels[i] / 10);
                    }
                }
                else if (LevelUnit == MEN.LevelUnit.dBµV)
                {
                    for (int i = 0; i < tracePoints; i++)
                    {
                        tr[i] = (float)Math.Pow(10, (newTraceLevels[i] - 107) / 10);
                    }
                }
                tracesToAverage.Add(tr);
                if (CalcAfterAdd || tracesToAverage.Count == averagingCount)
                {
                    Calc();
                }
            }
            else if (tracesToAverage.Count >= averagingCount)
            {
                while (tracesToAverage.Count > averagingCount - 1)
                {
                    tracesToAverage.RemoveAt(0);
                }
                float[] tr = new float[tracePoints];
                if (LevelUnit == MEN.LevelUnit.dBm)
                {
                    for (int i = 0; i < tracePoints; i++)
                    {
                        tr[i] = (float)Math.Pow(10, newTraceLevels[i] / 10);
                    }
                }
                else if (LevelUnit == MEN.LevelUnit.dBµV)
                {
                    for (int i = 0; i < tracePoints; i++)
                    {
                        tr[i] = (float)Math.Pow(10, (newTraceLevels[i] - 107) / 10);
                    }
                }
                tracesToAverage.Add(tr);
                if (CalcAfterAdd || tracesToAverage.Count == averagingCount)
                {
                    Calc();
                }
            }

        }

        public void Reset()
        {
            tracesToAverage.Clear();
            NumberOfSweeps = 0;
        }

        private void Calc()
        {
            NumberOfSweeps = tracesToAverage.Count;

            if (NumberOfSweeps > 1)
            {
                for (int x = 0; x < tracePoints; x++)
                {
                    double tl = 0;
                    for (int i = 0; i < tracesToAverage.Count; i++)
                    {
                        if (i == 0)
                        {
                            tl = tracesToAverage[0][x];
                        }
                        else
                        {
                            tl += tracesToAverage[i][x];
                        }
                    }
                    if (LevelUnit == MEN.LevelUnit.dBm)
                    {
                        AveragedLevels[x] = (float)(10 * Math.Log10(tl / tracesToAverage.Count));
                    }
                    else if (LevelUnit == MEN.LevelUnit.dBµV)
                    {
                        AveragedLevels[x] = (float)(107 + 10 * Math.Log10(tl / tracesToAverage.Count));
                    }
                }
            }
        }
    }
}
