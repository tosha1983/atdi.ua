using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.SDR.Server.MeasurementProcessing.SingleHound.ProcessSignal
{
    /// <summary>
    /// Клас обрабатывает поток и вычисляет индессы возрастающего фронта и падающего фронта начала и конца сигнала 
    /// </summary>
    class IQStreamStartAndStopFlange
    {
        #region parameter

        //configuration parameter
        // параметры для IQStreamStartAndStopFlangeCalcByTriggerLeve
        public Double PersentOfAmplitudeForTrigger = 10;
        public int TriggerIndexIntervalmksec = 3; //длительность которую можно считать паузой расчитана на 1 км распространения в микросекундах.
        // параметры для IQStreamStartAndStopFlangeCalcByRateOfChangeOfFlanks
        public Double CriticlaRateChangeFlankedBinmksec = 10;
        public Double CriticlaLevelChangeFlankedB = 10;
        public Double TimeAfterFlancmks = 0.5;
        // end configuration parameter

        public int[] IndexStartFlange;
        public int[] IndexStopFlange;
        Double[] TimeDurationSignal; // в милисекундах
        Double[] TimeDurationPause; // в милисекундах
        int samples_per_sec;
        Double TriggerLevel;
        int TriggerIndexInterval;
        public List<BlockOfSignal> BlockOfSignals;

        #endregion
        public IQStreamStartAndStopFlange(SetConfigurationForReceivIQStream IQparameters)
        {
            samples_per_sec = IQparameters.samples_per_sec;
        }
        /// <summary>
        /// Определяем начала и концы передачи при переходк через тригерный уровень.
        /// </summary>
        /// <param name="IQStream"></param>
        private void IQStreamStartAndStopFlangeCalcByTriggerLevel(ReceiveIQStream IQStream)
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
            IndexStartFlange = _IndexStartFlange.ToArray();
            IndexStopFlange = _IndexStopFlange.ToArray();
        }
        /// <summary>
        /// Определяем начало и конец передачи делением на равные интервалы
        /// </summary>
        /// <param name="IQStream"></param>
        private void IQStreamStartAndStopFlangeCalcByEqualTimeIntervals(ReceiveIQStream IQStream, double BlockDurationmks)
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
        private void IQStreamStartAndStopFlangeCalcByRateOfChangeOfFlanks(ReceiveIQStream IQStream, bool FilteringForFindSignalAndPause, Double bandwidthKhz)
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
            }

            IndexStartFlange = _IndexStartFlange.ToArray();
            IndexStopFlange = _IndexStopFlange.ToArray();
        }
        /// <summary>
        /// Расчитываем длительности сигналов/пауз
        /// </summary>
        public void CalcDurationSignalPause()
        {
            List<Double> _TimeDurationSignal = new List<double>();
            List<Double> _TimeDurationPause = new List<double>();
            for (int i = 1; i < IndexStartFlange.Length-1; i++)
            {
                Double TimeSignal = (IndexStopFlange[i] - IndexStartFlange[i]) * (1000000.0 / samples_per_sec);
                _TimeDurationSignal.Add(TimeSignal);
                Double TimePause = (IndexStartFlange[i] - IndexStopFlange[i - 1]) * (1000000.0 / samples_per_sec);
                _TimeDurationPause.Add(TimePause);
            }
            TimeDurationSignal = _TimeDurationSignal.ToArray();
            TimeDurationPause = _TimeDurationPause.ToArray();
        }
        /// <summary>
        ///  формируем блоки сигналов начиная от некой временной продолжительности в основе формирования лежит IndexStartFlange
        /// </summary>
        /// <param name="IQStream"></param>
        /// <param name="MinTimeDuration"></param>
        public void CreateBlockSignal(ReceiveIQStream IQStream, Double MinTimeDurationmks)
        {
            BlockOfSignals = new List<BlockOfSignal>();
            // создание одного большого массива
            int IndexesInBigArr = IQStream.iq_samples.Count*IQStream.iq_samples[0].Length;
            float[] GlobalStream = new float[IndexesInBigArr];
            for (int i = 0; i<IQStream.iq_samples.Count; i++)
            {
                Array.Copy(IQStream.iq_samples[i], 0, GlobalStream, IQStream.iq_samples[i].Length*i,IQStream.iq_samples[i].Length);
            }
            for (int i = 0; i < IndexStartFlange.Length; i++)
            {
                if (IndexStopFlange.Length <= i) { break; }
                BlockOfSignal Signal = new BlockOfSignal();
                int SizeBlock = (IndexStopFlange[i] - IndexStartFlange[i]) * 2;
                Signal.IQStream = new float [SizeBlock];
                Signal.StartIndexIQ = IndexStartFlange[i] * 2;
                Array.Copy(GlobalStream, Signal.StartIndexIQ, Signal.IQStream, 0, SizeBlock);
                Signal.Durationmks = (IndexStopFlange[i] - IndexStartFlange[i]) * (1000000.0 / samples_per_sec);
                if (Signal.Durationmks >= MinTimeDurationmks) 
                {
                    BlockOfSignals.Add(Signal); 
                }
            }
        }
        public void IQStreamStartAndStopFlangeCalc(IQStreamTimeStampBloks.MethodForTimeDivision methodForTimeDivision, ReceiveIQStream IQStream, double BlockDurationmks, bool FilteringForFindSignalAndPause, Double bandwidthKhz)
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
