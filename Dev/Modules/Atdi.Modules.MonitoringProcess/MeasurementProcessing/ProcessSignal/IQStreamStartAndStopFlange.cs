﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Modules.MonitoringProcess.ProcessSignal
{
    /// <summary>
    /// Клас обрабатывает поток и вычисляет индессы возрастающего фронта и падающего фронта начала и конца сигнала 
    /// </summary>
    class IQStreamStartAndStopFlange
    {
        #region parameter

        //configuration parameter
        // параметры для IQStreamStartAndStopFlangeCalcByTriggerLeve
        public double PersentOfAmplitudeForTrigger = 10;
        public int TriggerIndexIntervalmksec = 3; //длительность которую можно считать паузой расчитана на 1 км распространения в микросекундах.
        // параметры для IQStreamStartAndStopFlangeCalcByRateOfChangeOfFlanks
        public double CriticlaRateChangeFlankedBinmksec = 10;
        public double CriticlaLevelChangeFlankedB = 10;
        public double TimeAfterFlancmks = 0.5;
        // end configuration parameter

        public int[] IndexStartFlange;
        public int[] IndexStopFlange;
        double[] TimeDurationSignal; // в милисекундах
        double[] TimeDurationPause; // в милисекундах
        int samples_per_sec;
        double TriggerLevel;
        int TriggerIndexInterval;
        public List<BlockOfSignal> BlockOfSignals;

        #endregion
        public IQStreamStartAndStopFlange(int _semple_per_second)
        {
            samples_per_sec = _semple_per_second;
        }
        /// <summary>
        /// Определяем начала и концы передачи при переходк через тригерный уровень.
        /// </summary>
        /// <param name="IQStream"></param>
        private void IQStreamStartAndStopFlangeCalcByTriggerLevel(ReceivedIQStream IQStream)
        {
            TriggerIndexInterval = TriggerIndexIntervalmksec * samples_per_sec / 1000000;
            if (TriggerIndexInterval < 5) { TriggerIndexInterval = 5; }
            TriggerLevel = IQStream.MinLevel + (IQStream.MaxLevel - IQStream.MinLevel) * PersentOfAmplitudeForTrigger / 100.0;
            List<int> _IndexStartFlange = new List<int>();
            List<int> _IndexStopFlange = new List<int>();
            int NumberPointSignal = 0;
            int NumberPointPause = 0;
            bool flag = false; // signal = true, pause = false.
            int number = 0;
            for (int i = 0; i < IQStream.Ampl.Count; i++)
            {
                for (int j = 0; j < IQStream.Ampl[i].Length; j = j + 1)
                {
                    if (IQStream.Ampl[i][j] > TriggerLevel) { NumberPointSignal++; NumberPointPause = 0; } else { NumberPointPause++; NumberPointSignal = 0; }
                    if (!flag)
                    {
                        if (NumberPointSignal > TriggerIndexInterval)
                        {// начало сигнала  
                            _IndexStartFlange.Add(number - NumberPointSignal);
                            flag = true;
                        }
                    }
                    else
                    {
                        if (NumberPointPause > TriggerIndexInterval)
                        {// начало паузы  
                            _IndexStopFlange.Add(number - NumberPointPause);
                            flag = false;
                        }
                    }
                    number++;
                }
            }
            if (flag) { _IndexStopFlange.Add(number); } 
            if (_IndexStartFlange.Count > 0)
            {
                if (_IndexStartFlange[0] < 0) {_IndexStartFlange[0] = 0; }
            }
            IndexStartFlange = _IndexStartFlange.ToArray();
            IndexStopFlange = _IndexStopFlange.ToArray();
        }
        /// <summary>
        /// Определяем начало и конец передачи делением на равные интервалы
        /// </summary>
        /// <param name="IQStream"></param>
        private void IQStreamStartAndStopFlangeCalcByEqualTimeIntervals(ReceivedIQStream IQStream, double BlockDurationmks)
        {
            //константа 
            int StartPause = 300;
            int NumberSempleInBlock = (int)(BlockDurationmks*samples_per_sec/1000000.0);
            List<int> _IndexStartFlange = new List<int>();
            List<int> _IndexStopFlange = new List<int>();
            int NumberBloks = (int)(Math.Floor((IQStream.Ampl.Count * IQStream.Ampl[0].Length - StartPause - 1.0)/NumberSempleInBlock)) ;
            for (int i = 0; i < NumberBloks; i++)
            {
                _IndexStartFlange.Add(i* NumberSempleInBlock + StartPause);
                _IndexStopFlange.Add((i+1) * NumberSempleInBlock + StartPause-1);
            }
            IndexStartFlange = _IndexStartFlange.ToArray();
            IndexStopFlange = _IndexStopFlange.ToArray();
        }
        private void ClalLevelAfretAndBefor(ref List<float>TempIQAMPL, int  i, int NumberPointInBlockAfterUP, int NumberPointForUP,  ref Double AverageLevelBefor, ref Double AverageLevelAfter)
        {
            AverageLevelBefor = 0;
            AverageLevelAfter = 0;
            for (int j = i - NumberPointInBlockAfterUP; j < i; j++)
            {
                AverageLevelBefor = AverageLevelBefor + TempIQAMPL[j];
            }
            for (int j = i + NumberPointForUP; j < i + NumberPointForUP + NumberPointInBlockAfterUP; j++)
            {
                AverageLevelAfter = AverageLevelAfter + TempIQAMPL[j];
            }
            AverageLevelBefor = AverageLevelBefor / NumberPointInBlockAfterUP;
            AverageLevelAfter = AverageLevelAfter / NumberPointInBlockAfterUP;
        }
        /// <summary>
        /// Определяем начала и концы передачи при скачках амплитуды.
        /// </summary>
        /// <param name="IQStream"></param>
        private void IQStreamStartAndStopFlangeCalcByRateOfChangeOfFlanks(ReceivedIQStream IQStream, bool FilteringForFindSignalAndPause, Double bandwidthKhz)
        {
            CriticlaRateChangeFlankedBinmksec = 5.0 * bandwidthKhz/1000.0;

            List<int> _IndexStartFlange = new List<int>();
            List<int> _IndexStopFlange = new List<int>();
            int NumberPointInBlockAfterUP = (int)(TimeAfterFlancmks * samples_per_sec / 1000000);
            int NumberPointForUP = (int)((CriticlaLevelChangeFlankedB / CriticlaRateChangeFlankedBinmksec) * samples_per_sec / 1000000);
            List<float> TempIQAMPL = new List<float>();
            // перевинем все в один массив для удобства работы
            for (int i = 0; i < IQStream.Ampl.Count; i++)
            {
                for (int j = 0; j < IQStream.Ampl[i].Length; j = j + 1)
                { TempIQAMPL.Add(IQStream.Ampl[i][j]); }
            }
            Double AverageLevelBefor = 0;
            Double AverageLevelAfter = 0;
            ClalLevelAfretAndBefor(ref TempIQAMPL, NumberPointInBlockAfterUP, NumberPointInBlockAfterUP, NumberPointForUP, ref AverageLevelBefor, ref AverageLevelAfter);
            int ii = NumberPointInBlockAfterUP;
            do
            {
                if (ii != NumberPointInBlockAfterUP) 
                {
                    AverageLevelBefor = AverageLevelBefor - TempIQAMPL[ii - NumberPointInBlockAfterUP - 1] / NumberPointInBlockAfterUP + TempIQAMPL[ii] / NumberPointInBlockAfterUP;
                    AverageLevelAfter = AverageLevelAfter - TempIQAMPL[ii + NumberPointForUP - 1] / NumberPointInBlockAfterUP + TempIQAMPL[ii + NumberPointForUP + NumberPointInBlockAfterUP] / NumberPointInBlockAfterUP;
                }
                if (AverageLevelAfter/AverageLevelBefor >  Math.Pow(10, CriticlaLevelChangeFlankedB / 10) ) 
                { //зафиксирован фланг возрастания идет первая точка пика 
                    _IndexStartFlange.Add(ii + NumberPointForUP);
                    ii = ii + NumberPointForUP + NumberPointInBlockAfterUP;
                    if (ii > TempIQAMPL.Count - NumberPointInBlockAfterUP - NumberPointForUP) { break; }
                    ClalLevelAfretAndBefor(ref TempIQAMPL, ii-1, NumberPointInBlockAfterUP, NumberPointForUP, ref AverageLevelBefor, ref AverageLevelAfter);
                }
                else if (AverageLevelBefor/AverageLevelAfter  >  Math.Pow(10, CriticlaLevelChangeFlankedB / 10))
                { //зафиксирован фланг падения идет первая точка пика 
                    _IndexStopFlange.Add(ii + NumberPointForUP);
                    ii = ii + NumberPointForUP + NumberPointInBlockAfterUP;
                    if (ii > TempIQAMPL.Count - NumberPointInBlockAfterUP - NumberPointForUP) { break; }
                    ClalLevelAfretAndBefor(ref TempIQAMPL, ii - 1, NumberPointInBlockAfterUP, NumberPointForUP, ref AverageLevelBefor, ref AverageLevelAfter);
                }
                ii++;
            } while (ii < TempIQAMPL.Count - NumberPointInBlockAfterUP - NumberPointForUP);

            if ((FilteringForFindSignalAndPause) && (_IndexStartFlange.Count > 0) && (_IndexStopFlange.Count>0))
            {
                int i = 0 ;
                do
                {
                    if (_IndexStopFlange.Exists(x => (x > _IndexStartFlange[i]) && (x < _IndexStartFlange[i + 1])))
                    { i++; }
                    else { _IndexStartFlange.RemoveAt(i + 1); }
                }
                while (i < _IndexStartFlange.Count-1);
                i = 0;
                do
                {
                    if (_IndexStartFlange.Exists(x => (x > _IndexStopFlange[i]) && (x < _IndexStopFlange[i + 1])))
                    { i++; }
                    else { _IndexStopFlange.RemoveAt(i + 1); }
                }
                while (i < _IndexStopFlange.Count - 1);
                if (_IndexStopFlange[0] < _IndexStartFlange[0])
                {
                    _IndexStopFlange.RemoveAt(0);
                }
                if (_IndexStopFlange.Count < _IndexStartFlange.Count)
                {
                    _IndexStartFlange.RemoveAt(_IndexStartFlange.Count-1);
                }

            }

            IndexStartFlange = _IndexStartFlange.ToArray();
            IndexStopFlange = _IndexStopFlange.ToArray();
        }
        /// <summary>
        /// Расчитываем длительности сигналов/пауз
        /// </summary>
        public void CalcDurationSignalPause()
        {
            List<double> _TimeDurationSignal = new List<double>();
            List<double> _TimeDurationPause = new List<double>();
            for (int i = 0; i < IndexStartFlange.Length; i++)
            {
                double TimeSignal = (IndexStopFlange[i] - IndexStartFlange[i]) * (1000000.0 / samples_per_sec);
                _TimeDurationSignal.Add(TimeSignal);
                if (i != 0)
                {
                    double TimePause = (IndexStartFlange[i] - IndexStopFlange[i - 1]) * (1000000.0 / samples_per_sec);
                    _TimeDurationPause.Add(TimePause);
                }
            }
            TimeDurationSignal = _TimeDurationSignal.ToArray();
            TimeDurationPause = _TimeDurationPause.ToArray();
        }
        /// <summary>
        ///  формируем блоки сигналов начиная от некой временной продолжительности в основе формирования лежит IndexStartFlange
        /// </summary>
        /// <param name="IQStream"></param>
        /// <param name="MinTimeDuration"></param>
        public void CreateBlockSignal(ReceivedIQStream IQStream, Double MinTimeDurationmks)
        {
            // константа
            double MaxDurationSequence_mks = 10000; //0.01 c это 3 тыс км
            // 
            BlockOfSignals = new List<BlockOfSignal>();
            // создание одного большого массива
            int IndexesInBigArr = IQStream.iq_samples.Count*IQStream.iq_samples[0].Length;
            float[] GlobalStream = new float[IndexesInBigArr];
            for (int i = 0; i<IQStream.iq_samples.Count; i++)
            {
                Array.Copy(IQStream.iq_samples[i], 0, GlobalStream, IQStream.iq_samples[i].Length*i,IQStream.iq_samples[i].Length);
            }
            // Создаем новые фланги для укорачивания длительности импульсов
            double MaxNumberIndexForBlock = samples_per_sec * MaxDurationSequence_mks / 1000000.0;
            List<int> _IndexStartFlange = new List<int>();
            List<int> _IndexStopFlange = new List<int>();

            for (int i = 0; i < IndexStartFlange.Length; i++)
            {
                if (IndexStopFlange.Length <= i) { break; }
                if (IndexStopFlange[i] - IndexStartFlange[i] > MaxNumberIndexForBlock)
                {
                    int s = (int)Math.Floor((IndexStopFlange[i] - IndexStartFlange[i]) / MaxNumberIndexForBlock);
                    for (int j = 0; j <= s-1; j++)
                    {
                        _IndexStartFlange.Add(IndexStartFlange[i] + (int)MaxNumberIndexForBlock*j);
                        _IndexStopFlange.Add((IndexStartFlange[i] + (int)MaxNumberIndexForBlock * (j+1)));
                    }
                    _IndexStartFlange.Add(IndexStartFlange[i] + (int)MaxNumberIndexForBlock * s - 1);
                    _IndexStopFlange.Add(IndexStopFlange[i]);

                }
                else
                {
                    _IndexStartFlange.Add(IndexStartFlange[i]);
                    _IndexStopFlange.Add(IndexStopFlange[i]);
                }

            }


            for (int i = 0; i < _IndexStartFlange.Count; i++)
            {
                if (_IndexStopFlange.Count <= i) { break; }
                BlockOfSignal Signal = new BlockOfSignal();
                int SizeBlock = (_IndexStopFlange[i] - _IndexStartFlange[i]) * 2;
                Signal.IQStream = new float [SizeBlock];
                Signal.StartIndexIQ = _IndexStartFlange[i];
                Array.Copy(GlobalStream, Signal.StartIndexIQ*2, Signal.IQStream, 0, SizeBlock);
                Signal.Durationmks = (_IndexStopFlange[i] - _IndexStartFlange[i]) * (1000000.0 / samples_per_sec);
                if (Signal.Durationmks >= MinTimeDurationmks) 
                {
                    BlockOfSignals.Add(Signal); 
                }
            }
        }
        public void IQStreamStartAndStopFlangeCalc(IQStreamTimeStampBloks.MethodForTimeDivision methodForTimeDivision, ReceivedIQStream IQStream, double BlockDurationmks, bool FilteringForFindSignalAndPause, Double bandwidthKhz)
        {
            // константы которые нужны для исполнения методов
            switch (methodForTimeDivision)
            {
                case IQStreamTimeStampBloks.MethodForTimeDivision.TriggerLevel:
                    IQStreamStartAndStopFlangeCalcByTriggerLevel(IQStream);
                    break;
                case IQStreamTimeStampBloks.MethodForTimeDivision.ChangeOfFlanks:
                    IQStreamStartAndStopFlangeCalcByRateOfChangeOfFlanks(IQStream, FilteringForFindSignalAndPause, bandwidthKhz);
                    break;
                case IQStreamTimeStampBloks.MethodForTimeDivision.EqualTimeIntervals:
                    IQStreamStartAndStopFlangeCalcByEqualTimeIntervals(IQStream, BlockDurationmks * 1.1);
                    break;
                default:
                    IQStreamStartAndStopFlangeCalcByTriggerLevel(IQStream);
                    break;
            }
        }
    }
}
