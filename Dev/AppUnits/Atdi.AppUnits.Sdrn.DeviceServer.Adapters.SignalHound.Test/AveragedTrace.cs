﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MEN = Atdi.DataModels.Sdrn.DeviceServer.Adapters.Enums;

namespace Atdi.AppUnits.Sdrn.DeviceServer.Adapters.SignalHound.Test
{
    public class AveragedTrace
    {
        private int TracePoints;

        public bool CalcAfterAdd;

        public float[] AveragedLevels;

        private double[] Freqs;
        //данные на усреднение, хранятся в миливатах
        private List<float[]> TracesToAverage;

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

        public MEN.LevelUnit LevelUnit { get; private set; }

        /// <summary>
        /// текущее значение (сколько трейсов усреднено)
        /// </summary>
        public int NumberOfSweeps { get; private set; }


        public AveragedTrace()
        {
            TracePoints = 1601;
            CalcAfterAdd = false;
            Freqs = new double[] { };
            TracesToAverage = new List<float[]> { };
            LevelUnit = MEN.LevelUnit.dBm;
            NumberOfSweeps = 0;
        }

        public void AddTraceToAverade(double[] NewTraceFreqs, float[] NewTraceLevels, MEN.LevelUnit levelUnit)
        {
            //сбросит трейс если че
            if (TracesToAverage.Count == 0 || //Если данных еще нет
                NewTraceFreqs.Length != TracesToAverage[0].Length || //несовпали длины
                NewTraceFreqs[0] != Freqs[0] ||//несовпали начальные частоты
                NewTraceFreqs[NewTraceFreqs.Length - 1] != Freqs[Freqs.Length - 1] || //несовпали конечные частоты
                LevelUnit != levelUnit) //несовпали типы уровней
            {
                LevelUnit = levelUnit;
                TracesToAverage.Clear();
                NumberOfSweeps = 0;
                TracePoints = NewTraceFreqs.Length;

                Freqs = new double[TracePoints];
                AveragedLevels = new float[TracePoints];

                for (int i = 0; i < TracePoints; i++)
                {
                    Freqs[i] = NewTraceFreqs[i];
                    AveragedLevels[i] = NewTraceLevels[i];
                }
                NumberOfSweeps = 0;
                //добавить в TracesToAverage новый элемент пересчитаный
                float[] tr = new float[TracePoints];
                if (LevelUnit == MEN.LevelUnit.dBm)
                {
                    for (int i = 0; i < TracePoints; i++)
                    {
                        tr[i] = (float)Math.Pow(10, NewTraceLevels[i] / 10);
                    }
                }
                else if (LevelUnit == MEN.LevelUnit.dBµV)
                {
                    for (int i = 0; i < TracePoints; i++)
                    {
                        tr[i] = (float)Math.Pow(10, (NewTraceLevels[i] - 107) / 10);
                    }
                }
                TracesToAverage.Add(tr);
            }
            else if (TracesToAverage.Count < _AveragingCount)
            {
                float[] tr = new float[TracePoints];
                if (LevelUnit == MEN.LevelUnit.dBm)
                {
                    for (int i = 0; i < TracePoints; i++)
                    {
                        tr[i] = (float)Math.Pow(10, NewTraceLevels[i] / 10);
                    }
                }
                else if (LevelUnit == MEN.LevelUnit.dBµV)
                {
                    for (int i = 0; i < TracePoints; i++)
                    {
                        tr[i] = (float)Math.Pow(10, (NewTraceLevels[i] - 107) / 10);
                    }
                }
                TracesToAverage.Add(tr);
                if (CalcAfterAdd || TracesToAverage.Count == _AveragingCount)
                {
                    Calc();
                }
            }
            else if (TracesToAverage.Count >= _AveragingCount)
            {
                while (TracesToAverage.Count > _AveragingCount - 1)
                {
                    TracesToAverage.RemoveAt(0);
                }
                float[] tr = new float[TracePoints];
                if (LevelUnit == MEN.LevelUnit.dBm)
                {
                    for (int i = 0; i < TracePoints; i++)
                    {
                        tr[i] = (float)Math.Pow(10, NewTraceLevels[i] / 10);
                    }
                }
                else if (LevelUnit == MEN.LevelUnit.dBµV)
                {
                    for (int i = 0; i < TracePoints; i++)
                    {
                        tr[i] = (float)Math.Pow(10, (NewTraceLevels[i] - 107) / 10);
                    }
                }
                TracesToAverage.Add(tr);
                if (CalcAfterAdd || TracesToAverage.Count == _AveragingCount)
                {
                    Calc();
                }
            }

        }

        public void Reset()
        {
            TracesToAverage.Clear();
            NumberOfSweeps = 0;
        }

        private void Calc()
        {
            NumberOfSweeps = TracesToAverage.Count;

            if (NumberOfSweeps > 1)
            {
                for (int x = 0; x < TracePoints; x++)
                {
                    double tl = 0;
                    for (int i = 0; i < TracesToAverage.Count; i++)
                    {
                        if (i == 0)
                        {
                            tl = TracesToAverage[0][x];
                        }
                        else
                        {
                            tl += TracesToAverage[i][x];
                        }
                    }
                    if (LevelUnit == MEN.LevelUnit.dBm)
                    {
                        AveragedLevels[x] = (float)(10 * Math.Log10(tl / TracesToAverage.Count));
                    }
                    else if (LevelUnit == MEN.LevelUnit.dBµV)
                    {
                        AveragedLevels[x] = (float)(107 + 10 * Math.Log10(tl / TracesToAverage.Count));
                    }
                }
            }
        }
    }
}
