using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MEN = Atdi.DataModels.Sdrn.DeviceServer.Adapters.Enums;

namespace Atdi.AppUnits.Sdrn.DeviceServer.Adapters.SpectrumAnalyzer
{
    public class AveragedTrace
    {
        private int TracePoints;

        public bool CalcAfterAdd;

        public float[] AveragedLevels;

        private double freqStart = 0, freqStep = 0;
        //данные на усреднение, хранятся в миливатах
        private List<double[]> tracesToAverage;

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
                else if (value > 500)
                {
                    _AveragingCount = 500;
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
            TracePoints = 1601;
            CalcAfterAdd = false;
            tracesToAverage = new List<double[]> { };//В W
            InputlevelUnit = MEN.LevelUnit.dBm;
            OutputlevelUnit = MEN.LevelUnit.dBm;
            NumberOfSweeps = 0;
        }

        public void AddTraceToAverade(double newFreqStart, double newFreqStep, float[] newTraceLevels, MEN.LevelUnit inputLevelUnit, MEN.LevelUnit outputLevelUnit)
        {
            //сбросит трейс если че
            if (tracesToAverage.Count == 0 || //Если данных еще нет
                newTraceLevels.Length != tracesToAverage[0].Length || //несовпали длины
                newFreqStart != freqStart || //несовпали длины
                newFreqStep != freqStep ||//несовпали начальные частоты                
                InputlevelUnit != inputLevelUnit || //несовпали входные типы уровней
                OutputlevelUnit != outputLevelUnit) //несовпали входные типы уровней
            {
                freqStart = newFreqStart;
                freqStep = newFreqStep;
                InputlevelUnit = inputLevelUnit;
                OutputlevelUnit = outputLevelUnit;
                tracesToAverage.Clear();
                NumberOfSweeps = 0;
                TracePoints = newTraceLevels.Length;

                AveragedLevels = new float[TracePoints];

                for (int i = 0; i < TracePoints; i++)
                {
                    AveragedLevels[i] = newTraceLevels[i];
                }
                NumberOfSweeps = 0;
                //добавить в TracesToAverage новый элемент пересчитаный
                double[] tr = new double[TracePoints];
                if (inputLevelUnit == MEN.LevelUnit.dBm)
                {
                    for (int i = 0; i < TracePoints; i++)
                    {
                        tr[i] = (double)(Math.Pow(10, newTraceLevels[i] / 10) / 1000.0);
                    }
                }
                else if (inputLevelUnit == MEN.LevelUnit.dBµV)
                {
                    for (int i = 0; i < TracePoints; i++)
                    {
                        tr[i] = (double)(Math.Pow(10, (newTraceLevels[i] - 107) / 10)/1000.0);
                    }
                }
                else if (inputLevelUnit == MEN.LevelUnit.Watt)
                {
                    for (int i = 0; i < TracePoints; i++)
                    {
                        tr[i] = newTraceLevels[i];
                    }
                }
                //if (inputLevelUnit == MEN.LevelUnit.dBm)
                //{
                //    for (int i = 0; i < TracePoints; i++)
                //    {
                //        tr[i] = (float)Math.Pow(10, NewTraceLevels[i] / 10);
                //    }
                //}
                //else if (inputLevelUnit == MEN.LevelUnit.dBµV)
                //{
                //    for (int i = 0; i < TracePoints; i++)
                //    {
                //        tr[i] = (float)Math.Pow(10, (NewTraceLevels[i] - 107) / 10);
                //    }
                //}
                //else if (inputLevelUnit == MEN.LevelUnit.Watt)
                //{
                //    for (int i = 0; i < TracePoints; i++)
                //    {
                //        tr[i] = NewTraceLevels[i] * 1000;
                //    }
                //}
                tracesToAverage.Add(tr);
            }
            else if (tracesToAverage.Count < _AveragingCount)
            {
                double[] tr = new double[TracePoints];
                if (inputLevelUnit == MEN.LevelUnit.dBm)
                {
                    for (int i = 0; i < TracePoints; i++)
                    {
                        tr[i] = (double)(Math.Pow(10, newTraceLevels[i] / 10) / 1000.0);
                    }
                }
                else if (inputLevelUnit == MEN.LevelUnit.dBµV)
                {
                    for (int i = 0; i < TracePoints; i++)
                    {
                        tr[i] = (double)(Math.Pow(10, (newTraceLevels[i] - 107) / 10) / 1000.0);
                    }
                }
                else if (inputLevelUnit == MEN.LevelUnit.Watt)
                {
                    for (int i = 0; i < TracePoints; i++)
                    {
                        tr[i] = newTraceLevels[i];
                    }
                }
                //if (inputLevelUnit == MEN.LevelUnit.dBm)
                //{
                //    for (int i = 0; i < TracePoints; i++)
                //    {
                //        tr[i] = (float)Math.Pow(10, NewTraceLevels[i] / 10);
                //    }
                //}
                //else if (inputLevelUnit == MEN.LevelUnit.dBµV)
                //{
                //    for (int i = 0; i < TracePoints; i++)
                //    {
                //        tr[i] = (float)Math.Pow(10, (NewTraceLevels[i] - 107) / 10);
                //    }
                //}
                //else if (inputLevelUnit == MEN.LevelUnit.Watt)
                //{
                //    for (int i = 0; i < TracePoints; i++)
                //    {
                //        tr[i] = NewTraceLevels[i] * 1000;
                //    }
                //}
                tracesToAverage.Add(tr);
                if (CalcAfterAdd || tracesToAverage.Count == _AveragingCount)
                {
                    Calc();
                }
            }
            else if (tracesToAverage.Count >= _AveragingCount)
            {
                while (tracesToAverage.Count > _AveragingCount - 1)
                {
                    tracesToAverage.RemoveAt(0);
                }
                double[] tr = new double[TracePoints];
                if (inputLevelUnit == MEN.LevelUnit.dBm)
                {
                    for (int i = 0; i < TracePoints; i++)
                    {
                        tr[i] = (double)(Math.Pow(10, newTraceLevels[i] / 10) / 1000.0);
                    }
                }
                else if (inputLevelUnit == MEN.LevelUnit.dBµV)
                {
                    for (int i = 0; i < TracePoints; i++)
                    {
                        tr[i] = (double)(Math.Pow(10, (newTraceLevels[i] - 107) / 10) / 1000.0);
                    }
                }
                else if (inputLevelUnit == MEN.LevelUnit.Watt)
                {
                    for (int i = 0; i < TracePoints; i++)
                    {
                        tr[i] = newTraceLevels[i];
                    }
                }
                //if (inputLevelUnit == MEN.LevelUnit.dBm)
                //{
                //    for (int i = 0; i < TracePoints; i++)
                //    {
                //        tr[i] = (float)Math.Pow(10, NewTraceLevels[i] / 10);
                //    }
                //}
                //else if (inputLevelUnit == MEN.LevelUnit.dBµV)
                //{
                //    for (int i = 0; i < TracePoints; i++)
                //    {
                //        tr[i] = (float)Math.Pow(10, (NewTraceLevels[i] - 107) / 10);
                //    }
                //}
                //else if (inputLevelUnit == MEN.LevelUnit.Watt)
                //{
                //    for (int i = 0; i < TracePoints; i++)
                //    {
                //        tr[i] = NewTraceLevels[i] * 1000;
                //    }
                //}
                tracesToAverage.Add(tr);
                if (CalcAfterAdd || tracesToAverage.Count == _AveragingCount)
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
                for (int x = 0; x < TracePoints; x++)
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
                    tl = tl / tracesToAverage.Count;
                    if (OutputlevelUnit == MEN.LevelUnit.dBm)
                    {
                        AveragedLevels[x] = (float)(10 * Math.Log10(1000.0 * tl));
                    }
                    else if (OutputlevelUnit == MEN.LevelUnit.dBµV)
                    {
                        AveragedLevels[x] = (float)(107 + 10 * Math.Log10(1000.0 * tl));
                    }
                    //if (OutputlevelUnit == MEN.LevelUnit.dBm)
                    //{
                    //    AveragedLevels[x] = (float)(10 * Math.Log10(tl / TracesToAverage.Count));
                    //}
                    //else if (OutputlevelUnit == MEN.LevelUnit.dBµV)
                    //{
                    //    AveragedLevels[x] = (float)(107 + 10 * Math.Log10(tl / TracesToAverage.Count));
                    //}
                }
            }
        }
    }
}

