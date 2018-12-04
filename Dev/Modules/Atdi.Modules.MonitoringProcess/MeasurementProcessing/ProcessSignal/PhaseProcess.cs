using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Modules.MonitoringProcess.SingleHound.ProcessSignal
{
    static class PhaseProcess
    {
        #region FindFreqAndPhase
        static private double CalcErrForRotationIndexByPhaseAndFreq(ref List<int> RotationIndex, double Phase, double Freq, int indexStart, int indexEnd)
        {
            Double Penalty = 0;
            if (indexEnd > RotationIndex.Count) { indexEnd = RotationIndex.Count; }
            for (int i = indexStart; i < indexEnd; i++)
            {
                Double ost = (RotationIndex[i] - Phase) % Freq;
                ost = Math.Min(ost, Freq - ost) / Freq;
                Penalty = Penalty + ost;
            }
            return Penalty / (indexEnd - indexStart);
        }
        static private void CalcBestPhaseForUnimodalFunction(ref List<int> RotationIndex, int StartIndex, int CountIndexForFind, double Freq, double _phaseMin, double _phaseMax, double accuracy, out double BestPhase, out double Penalty)
        {// Используем метод золотого сечения
            Double PenaltyMinPhase; Double PenaltyMaxPhase;
            Double PhaseMin = _phaseMin; Double PhaseMax = _phaseMax;
            PenaltyMinPhase = CalcErrForRotationIndexByPhaseAndFreq(ref RotationIndex, PhaseMin, Freq, StartIndex, StartIndex + CountIndexForFind);
            PenaltyMaxPhase = CalcErrForRotationIndexByPhaseAndFreq(ref RotationIndex, PhaseMax, Freq, StartIndex, StartIndex + CountIndexForFind);
            Double x1Phase; Double x2Phase;
            Double Penaltyx1Phase = 0; Double Penaltyx2Phase = 0;
            x1Phase = PhaseMax - (PhaseMax - PhaseMin) / 1.618;
            x2Phase = PhaseMin + (PhaseMax - PhaseMin) / 1.618;
            bool calcx1 = true;
            Penaltyx2Phase = CalcErrForRotationIndexByPhaseAndFreq(ref RotationIndex, x2Phase, Freq, StartIndex, StartIndex + CountIndexForFind);
            do
            {
                if (calcx1)
                {
                    Penaltyx1Phase = CalcErrForRotationIndexByPhaseAndFreq(ref RotationIndex, x1Phase, Freq, StartIndex, StartIndex + CountIndexForFind);
                }
                else
                {
                    Penaltyx2Phase = CalcErrForRotationIndexByPhaseAndFreq(ref RotationIndex, x2Phase, Freq, StartIndex, StartIndex + CountIndexForFind);
                }

                if (Penaltyx2Phase > Penaltyx1Phase)
                {
                    PenaltyMaxPhase = Penaltyx2Phase;
                    PhaseMax = x2Phase;
                    calcx1 = true;
                    x2Phase = x1Phase;
                    Penaltyx2Phase = Penaltyx1Phase;
                    x1Phase = PhaseMax - (PhaseMax - PhaseMin) / 1.618;
                }
                else
                {
                    PenaltyMinPhase = Penaltyx1Phase;
                    PhaseMin = x1Phase;
                    calcx1 = false;
                    x1Phase = x2Phase;
                    Penaltyx1Phase = Penaltyx2Phase;
                    x1Phase = x2Phase;
                    x2Phase = PhaseMin + (PhaseMax - PhaseMin) / 1.618;
                }
            }
            while (accuracy < PhaseMax - PhaseMin);
            BestPhase = PhaseMax;
            Penalty = PenaltyMaxPhase;
        }
        static private void CalcBestPhase(ref List<int> RotationIndex, int StartIndex, int CountIndexForFind, double Freq, double accuracy, int NumberSubPoint, out double BestPhase, out double Penalty)
        {// коекак работает не факт что оптимально
            BestPhase = 0;
            Penalty = 99999999;
            if (NumberSubPoint < 1) { NumberSubPoint = 1; }
            for (int i = 0; i < NumberSubPoint; i++)
            {
                double LocalPhase; double LocalPenalty;
                double _phaseMin = i * (Freq / NumberSubPoint);
                double _phaseMax = (i + 1) * (Freq / NumberSubPoint);
                CalcBestPhaseForUnimodalFunction(ref RotationIndex, StartIndex, CountIndexForFind, Freq, _phaseMin, _phaseMax, accuracy, out LocalPhase, out LocalPenalty);
                if (LocalPenalty < Penalty) { Penalty = LocalPenalty; BestPhase = LocalPhase; }
            }

        }
        static private void CalcSimbolFrequencyAndPhaceStartIndexFromIndexesListUnimodal(ref List<int> RotationIndex, double SimbFreqMin, double SimbFreqMax, double AccuracyFreq, double AccuracyPhase, int NumberSubPoint, int StartIndex, int CountIndexForFind, out Double Freq, out Double Phase, out Double bestPenalty)
        {
            Double PenaltyMinFreq; Double PenaltyMaxFreq;
            Double PhaseMin; Double PhaseMax;
            CalcBestPhase(ref RotationIndex, StartIndex, CountIndexForFind, SimbFreqMin, AccuracyPhase, NumberSubPoint, out PhaseMin, out PenaltyMinFreq);
            CalcBestPhase(ref RotationIndex, StartIndex, CountIndexForFind, SimbFreqMax, AccuracyPhase, NumberSubPoint, out PhaseMax, out PenaltyMaxFreq);
            Double x1Freq; Double x2Freq;
            Double Penaltyx1Freq = 0; Double Penaltyx2Freq = 0;
            Double Phasex1Freq = 0; Double Phasex2Freq = 0;
            x1Freq = SimbFreqMax - (SimbFreqMax - SimbFreqMin) / 1.618;
            x2Freq = SimbFreqMin + (SimbFreqMax - SimbFreqMin) / 1.618;
            bool calcx1 = true;
            CalcBestPhase(ref RotationIndex, StartIndex, CountIndexForFind, x2Freq, AccuracyPhase, NumberSubPoint, out Phasex2Freq, out Penaltyx2Freq);
            do
            {
                if (calcx1)
                {
                    CalcBestPhase(ref RotationIndex, StartIndex, CountIndexForFind, x1Freq, AccuracyPhase, NumberSubPoint, out Phasex1Freq, out Penaltyx1Freq);
                }
                else
                {
                    CalcBestPhase(ref RotationIndex, StartIndex, CountIndexForFind, x2Freq, AccuracyPhase, NumberSubPoint, out Phasex2Freq, out Penaltyx2Freq);
                }

                if (Penaltyx2Freq > Penaltyx1Freq)
                {
                    SimbFreqMax = x2Freq;
                    PenaltyMaxFreq = Penaltyx2Freq;
                    PhaseMax = Phasex2Freq;
                    calcx1 = true;
                    x2Freq = x1Freq;
                    Penaltyx2Freq = Penaltyx1Freq;
                    Phasex2Freq = Phasex1Freq;
                    x1Freq = SimbFreqMax - (SimbFreqMax - SimbFreqMin) / 1.618;
                }
                else
                {
                    SimbFreqMin = x1Freq;
                    PenaltyMinFreq = Penaltyx1Freq;
                    PhaseMin = Phasex1Freq;
                    calcx1 = false;
                    x1Freq = x2Freq;
                    Penaltyx1Freq = Penaltyx2Freq;
                    Phasex1Freq = Phasex2Freq;
                    x2Freq = SimbFreqMin + (SimbFreqMax - SimbFreqMin) / 1.618;
                }
            }
            while (AccuracyFreq < SimbFreqMax - SimbFreqMin);
            Freq = SimbFreqMax;
            Phase = PhaseMax;
            bestPenalty = PenaltyMaxFreq;
        }
        static private void CalcSimbolFrequencyAndPhaceStartIndexFromIndexesList(ref List<int> RotationIndex, double SamplesPerSec, double BWkHz, int StartIndex, int CountIndexForFind, out Double Freq, out Double Phase, out Double bestPenalty)
        {
            int NumberSubFreq = 10;
            int NumberSubPhase = 10;
            double AccuracyFreq = 0.01;
            double AccuracyPhase = 0.1;
            bestPenalty = 9999999;
            Freq = 0;
            Phase = 0;
            double SimbFreqMin = 0.7 * SamplesPerSec / (1000 * BWkHz);
            double SimbFreqMax = 1.3 * SamplesPerSec / (1000 * BWkHz);
            bestPenalty = 10000000000;
            for (int i = 0; i < NumberSubFreq; i++)
            {
                double FreqMin = SimbFreqMin + i * (SimbFreqMax - SimbFreqMin) / NumberSubFreq;
                double FreqMax = SimbFreqMin + (i + 1) * (SimbFreqMax - SimbFreqMin) / NumberSubFreq;
                double LocalFreq; double LocalPhase; double LocalPenalty;
                CalcSimbolFrequencyAndPhaceStartIndexFromIndexesListUnimodal(ref RotationIndex, FreqMin, FreqMax, AccuracyFreq, AccuracyPhase, NumberSubPhase, StartIndex, CountIndexForFind, out LocalFreq, out LocalPhase, out LocalPenalty);
                if (LocalPenalty < bestPenalty) { bestPenalty = LocalPenalty; Freq = LocalFreq; Phase = LocalPhase; }
            }
        }

        static private void CalcSimbolFrequencyAndPhaceStartIndexFromIndexesListForMultiTone(ref List<int> RotationIndex, double SamplesPerSec, double BWkHz, double DurationBlockmks, int StartIndexIQ, int StartIndex, int CountIndexForFind, out List<TimeStampToneParameter> timeStampToneParameters)
        { // Не проверенно - сложный метод для поиска TimeStampTone НЕ ДОРАБОТАННЫЙ
          // Константы 
            bool CreatLastTone = true;
            double DistansToChangeTonePersent = 20;
            double SempleFrequencyFromBW = SamplesPerSec / (BWkHz * 1000.0);
            //
            double Freq = RotationIndex[1] - RotationIndex[0];
            double Phase = RotationIndex[0];
            timeStampToneParameters = new List<TimeStampToneParameter>();
            int StartPointOfTone = 0;
            double MaxPenalty = 0; double Penalty = 0;
            int additional_index = 0;
            for (int i = 1; i < RotationIndex.Count-2; i++)
            {
                double Distanse = RotationIndex[i + 1] - RotationIndex[i];
                if (Math.Abs(Distanse - Freq)/Distanse > DistansToChangeTonePersent/100.0)
                {// new tone
                    // chack on 1 hole 
                    if ((Math.Abs(Distanse - 2 * Freq) / Distanse < DistansToChangeTonePersent / 100.0))
                    {
                        // exist hole in 1 point
                        additional_index++;
                        Freq = (RotationIndex[i + 1] - RotationIndex[StartPointOfTone]) / (i + 1.0 - StartPointOfTone + additional_index);
                        Penalty = CalcErrForRotationIndexByPhaseAndFreq(ref RotationIndex, Phase, Freq, StartPointOfTone, i + 1);
                        if (MaxPenalty < Penalty) { MaxPenalty = Penalty; }
                    }
                    else
                    {
                        // create timestamp tone parameter for old tone
                        TimeStampToneParameter timeStampToneParameter = new TimeStampToneParameter();
                        timeStampToneParameter.NumberSempleInSimbol = Freq;
                        timeStampToneParameter.Penalty = Penalty;
                        timeStampToneParameter.SempleShiftOfSimbol = Phase;
                        timeStampToneParameters.Add(timeStampToneParameter);
                        timeStampToneParameter.NumberHit = i - StartPointOfTone;
                        //
                        additional_index = 0;
                        StartPointOfTone = i;

                        if ((RotationIndex.Count - 2) > i)
                        {
                            Freq = (RotationIndex[i + 1] - RotationIndex[i]);
                            Phase = RotationIndex[i];
                            Penalty = 0;
                        }
                        else
                        {
                            CreatLastTone = false;
                            break;
                        }
                    }
                }
                else
                {//old tone 
                    // corection freq
                    Freq = (RotationIndex[i + 1] - RotationIndex[StartPointOfTone])/ (i + 1.0 - StartPointOfTone + additional_index);
                    Penalty = CalcErrForRotationIndexByPhaseAndFreq(ref RotationIndex, Phase, Freq, StartPointOfTone, i+1);
                    if (MaxPenalty < Penalty) { MaxPenalty = Penalty;}
                }
            }
            if (CreatLastTone)
            {
                TimeStampToneParameter timeStampToneParameter = new TimeStampToneParameter();
                timeStampToneParameter.NumberSempleInSimbol = Freq;
                timeStampToneParameter.Penalty = Penalty;
                timeStampToneParameter.SempleShiftOfSimbol = Phase;
                timeStampToneParameter.NumberHit = RotationIndex.Count - StartPointOfTone-1;
                timeStampToneParameters.Add(timeStampToneParameter);
            }

        }
        static public void CalcTimestampBloks(ref List<int> RotationIndex, double SamplesPerSec, double BWkHz, double DurationBlockmks, int StartIndexIQ, IQStreamTimeStampBloks.MethodForCalcFreqFromCriticalPoint MethodForCalcFreqFromCriticalPoint, bool CalcFreqTone, out TimeStampBlock TimeStampBlock)
        {
            TimeStampBlock = new TimeStampBlock();
            TimeStampBlock.TimeStampToneParameters = new List<TimeStampToneParameter>();
            TimeStampBlock.RotationIndex = RotationIndex;
            TimeStampBlock.DurationBlockmks = DurationBlockmks;
            TimeStampBlock.StartIndexOfBlock = StartIndexIQ;
            if (!CalcFreqTone) { return;}
            if (MethodForCalcFreqFromCriticalPoint == IQStreamTimeStampBloks.MethodForCalcFreqFromCriticalPoint.SingleToneByBlock)
            {
                // используем метод оптимизации мультистарта по двум параметрам, фаза и частота.
                int StartIndex = 2;
                int CountIndexForFind = RotationIndex.Count - 2;
                CalcSimbolFrequencyAndPhaceStartIndexFromIndexesList(ref RotationIndex, SamplesPerSec, BWkHz, StartIndex, CountIndexForFind, out Double Freq, out Double Phase, out Double bestPenalty);
                TimeStampToneParameter timeStampToneParameter = new TimeStampToneParameter();
                timeStampToneParameter.Penalty = bestPenalty;
                timeStampToneParameter.SempleShiftOfSimbol = Phase;
                timeStampToneParameter.NumberSempleInSimbol = Freq;
                TimeStampBlock.TimeStampToneParameters.Add(timeStampToneParameter);
            }
            if (MethodForCalcFreqFromCriticalPoint == IQStreamTimeStampBloks.MethodForCalcFreqFromCriticalPoint.MultyToneByBlock)
            {
                int StartIndex = 0;
                int CountIndexForFind = RotationIndex.Count;
                CalcSimbolFrequencyAndPhaceStartIndexFromIndexesListForMultiTone(ref RotationIndex, SamplesPerSec, BWkHz, DurationBlockmks, StartIndexIQ, StartIndex, CountIndexForFind, out TimeStampBlock.TimeStampToneParameters);
            }
        }

        #endregion
        #region FindCriticalPoint
        static private void CalcAmplitudeBlock(BlockOfSignal block, out double[] AmplBlock, out double MinAmpl, out double MaxAmpl)
        {
            MinAmpl = 9999;
            MaxAmpl = -9999;
            AmplBlock = new double[block.IQStream.Length / 2];
            for (int j = 0; j < block.IQStream.Length; j = j + 2)
            {
                double _arrAmpl = Math.Sqrt(block.IQStream[j] * block.IQStream[j] + block.IQStream[j + 1] * block.IQStream[j + 1]);
                if (MinAmpl > _arrAmpl) { MinAmpl = _arrAmpl; }
                if (MaxAmpl < _arrAmpl) { MaxAmpl = _arrAmpl; }
                AmplBlock[j / 2] = _arrAmpl;
            }
        }
        static private void CalcPhaseAndRotation(double LastAngl, double NewAngl, ref bool RotationR, ref double DeltaPhase)
        {// До конца не тестил
            DeltaPhase = NewAngl - LastAngl;
            if (DeltaPhase > 0) { RotationR = false; } else { RotationR = true; DeltaPhase = -DeltaPhase; }
            if (DeltaPhase > 180)
            {
                RotationR = !RotationR;
                DeltaPhase = 360 - DeltaPhase;
            }

        }
        static private double CalcAnglFromIQDegree(float I, float Q)
        {
            Double Angl = Math.Atan(I / Q) * 180 / Math.PI;
            if (Q < 0) { Angl = Angl + 180; }
            if (Angl < 0) { Angl = Angl + 360; }
            return Angl;
        }


        static private void CalcSpeedChangeIQChangeCalculationPhase(ref BlockOfSignal block, out List<int> RotationIndex, bool Min = true)
        {  // Расчет с корости с которой происходит изменения фазы при этом мы имеем максимальную скорость и относительно ее мы определяем участки гда скорость меняется медленно. 
            RotationIndex = new List<int>();
            List<double> DistansesIQ; 
            CalcDistanceIQBetweenPoint(ref block, out DistansesIQ);
            List<int> IndexMax;
            List<int> IndexMin;
            CalcMaxMinPereodicFunction(ref DistansesIQ, out IndexMax, out IndexMin);
            //IndexMax.Distinct();
            //IndexMin.Distinct();
            if (Min) { RotationIndex = IndexMin; } else { RotationIndex = IndexMax; }
            
        }
        static private void CalcDistanceIQBetweenPoint(ref BlockOfSignal block, out List<double> DistansesIQ)
        {
            DistansesIQ = new List<double>();
            for (int i = 2; i < block.IQStream.Length; i = i + 2)
            {
                double dist = (block.IQStream[i] - block.IQStream[i - 2]) * (block.IQStream[i] - block.IQStream[i - 2]) + (block.IQStream[i + 1] - block.IQStream[i - 1]) * (block.IQStream[i + 1] - block.IQStream[i - 1]);
                DistansesIQ.Add(1000000.0*Math.Sqrt(dist));
            }
        }
        static private void CalcMaxMinPereodicFunction(ref List<double> DistansesIQ, out List<int> IndexMax, out List<int> IndexMin, int PointMinMaxBase = 3)
        { 
            IndexMax = new List<int>();
            IndexMin = new List<int>();
            
            bool goUP;
            int indexLastMin; int indexLastMax; double LastMin; double LastMax;
            if (DistansesIQ[0] < DistansesIQ[1])
            {
                goUP = true;
                indexLastMin = 0;
                LastMin = DistansesIQ[0];
                indexLastMax = 1;
                LastMax = DistansesIQ[1];
            }
            else
            {
                goUP = false;
                indexLastMax = 0;
                LastMax = DistansesIQ[0];
                indexLastMin = 1;
                LastMin = DistansesIQ[1];
            }
            int CountForMin = 0; int CountForMax = 0;
            for (int i = 2; i < DistansesIQ.Count - 1; i = i + 1)
            {
                if (!goUP)
                {
                    // Идем на спад ищем минимум 
                    if (DistansesIQ[i] < LastMin) { LastMin = DistansesIQ[i]; indexLastMin = i;  CountForMin = 0; }
                    else { CountForMin++; }
                    if (CountForMin > PointMinMaxBase)
                    {// прошли через минимум 
                        goUP = true;
                        LastMax = LastMin;
                        IndexMin.Add(indexLastMin);
                        i = indexLastMin + 1;
                    }
                }
                else
                {
                    // Идем на возростание функции 
                    if (DistansesIQ[i] > LastMax) { LastMax = DistansesIQ[i]; indexLastMax = i; CountForMax = 0; }
                    else { CountForMax++; }
                    if (CountForMax > PointMinMaxBase)
                    {// прошли через максимум 
                        goUP = false;
                        LastMin = LastMax;
                        IndexMax.Add(indexLastMax);
                        i = indexLastMax + 1;
                    }
                }
            }
        }



        static private void RotationPhaseCalculation(ref BlockOfSignal block, out List<int> RotationIndex, out List<double> RotationPhase, double TriggerAngleForChangeRotationDegree = 20)
        {
            RotationIndex = new List<int>();
            RotationPhase = new List<Double>();

            Double PointStart;
            Double LastPhase;
            bool lastRotationR = false;
            Double PointWithMaxAngleDistanse = 0;
            int IndexWithMaxDistanse = 0;

            PointStart = CalcAnglFromIQDegree(block.IQStream[0], block.IQStream[1]);
            LastPhase = PointStart;
            Double CurrentAnglDistanse = 0;
            for (int i = 2; i < block.IQStream.Length - 1; i = i + 2)
            {
                Double Phase = CalcAnglFromIQDegree(block.IQStream[i], block.IQStream[i + 1]);
                Double CurrentDeltaPhase = 0; bool CurrentRotationR = true;
                CalcPhaseAndRotation(LastPhase, Phase, ref CurrentRotationR, ref CurrentDeltaPhase);
                if (lastRotationR == CurrentRotationR) { CurrentAnglDistanse = CurrentAnglDistanse + CurrentDeltaPhase; } else { CurrentAnglDistanse = CurrentAnglDistanse - CurrentDeltaPhase; }
                if (PointWithMaxAngleDistanse < CurrentAnglDistanse) { PointWithMaxAngleDistanse = CurrentAnglDistanse; IndexWithMaxDistanse = i; }
                if (PointWithMaxAngleDistanse - CurrentAnglDistanse > TriggerAngleForChangeRotationDegree)
                { // есть новый скачок фазы
                    lastRotationR = !lastRotationR;
                    int IndexOfChangePhase = IndexWithMaxDistanse;
                    RotationIndex.Add(IndexOfChangePhase / 2);
                    PointStart = CalcAnglFromIQDegree(block.IQStream[IndexWithMaxDistanse], block.IQStream[IndexWithMaxDistanse + 1]);
                    RotationPhase.Add(PointStart);
                    CalcPhaseAndRotation(PointStart, Phase, ref CurrentRotationR, ref CurrentDeltaPhase);
                    CurrentAnglDistanse = PointWithMaxAngleDistanse - CurrentAnglDistanse;
                    PointWithMaxAngleDistanse = CurrentAnglDistanse; IndexWithMaxDistanse = i;
                }
                LastPhase = Phase;
            }

        }
        static private double CalcAngleBetweenPoints(double I1, double Q1, double I2, double Q2)
        {
            double angle = (180/Math.PI)*Math.Atan((I2 - I1) / (Q2 - Q1));
            if (Q2 - Q1 < 0) { angle = angle + 180; }
            if (angle > 360) { angle = angle - 360; }
            return angle; 
        }
        static private bool HitPointIQinLine(ref BlockOfSignal block, int indexPoint1, int indexPoint2, double MaxPersentDeviation)
        {
            double sumH = 0;
            double TrigerrAngle = CalcAngleBetweenPoints(block.IQStream[indexPoint1], block.IQStream[indexPoint1 + 1], block.IQStream[indexPoint2], block.IQStream[indexPoint2 + 1]);
            double z = 0;
            for (int i = indexPoint1 + 2 ; i < indexPoint2; i = i + 2)
            {
                double LocalAngle = CalcAngleBetweenPoints(block.IQStream[indexPoint1], block.IQStream[indexPoint1 + 1], block.IQStream[i], block.IQStream[i + 1]);
                double delI = block.IQStream[indexPoint1] - block.IQStream[i];
                double delQ = block.IQStream[indexPoint1+1] - block.IQStream[i+1];
                z = Math.Sqrt((delI) * (delI) + (delQ) * (delQ));
                double h = Math.Abs(z * Math.Sin((LocalAngle - TrigerrAngle)*Math.PI/180));
                sumH = sumH + h;
            }
            if (sumH *2 / (indexPoint2 - indexPoint1) > z * MaxPersentDeviation / 100)
            {
                return false;
            }
            return true;
        }
        static private void AmplitudePhaseCalculation(ref BlockOfSignal block, out List<int> RotationIndex, double PointMinMaxBase = 5, double TriggerAmplitideForFixRotationInProcent = 20, bool CheckLine = true, double AngleDeviationDegreeForAmplitude = 10)
        {
            // constant
            double MaxPersentDeviation = 5;

            int indexOldMax =-9999;
            RotationIndex = new List<int>();
            double[] AmplBlock; double MinAmpl; double MaxAmpl;
            // Формирование амплитуд
            CalcAmplitudeBlock(block, out AmplBlock, out MinAmpl, out MaxAmpl);
            double TriggerAmplitide = MaxAmpl * TriggerAmplitideForFixRotationInProcent / 100;
            // Решение о первой точке 
            bool CurrentPointIsMaximum;
            int indexLastMin; int indexLastMax; double LastMin; double LastMax;
            if (AmplBlock[0] < AmplBlock[1])
            {
                CurrentPointIsMaximum = false;
                indexLastMin = 0;
                LastMin = AmplBlock[0];
                indexLastMax = 1;
                LastMax = AmplBlock[1];
            }
            else
            {
                CurrentPointIsMaximum = true;
                indexLastMax = 0;
                LastMax = AmplBlock[0];
                indexLastMin = 1;
                LastMin = AmplBlock[1];
            }
            int CountForMin = 0; int CountForMax = 0;
            for (int i = 2; i < AmplBlock.Length - 1; i = i + 1)
            {
                if (CurrentPointIsMaximum)
                {
                    // Идем на спад ищем минимум 
                    if (AmplBlock[i] < LastMin) { LastMin = AmplBlock[i]; indexLastMin = i;  CountForMin = 0; }
                    else { CountForMin++; }
                    if (CountForMin > PointMinMaxBase)
                    {// прошли через минимум 
                        CurrentPointIsMaximum = false;
                        LastMax = LastMin;
                    }
                }
                else
                {
                    // Идем на возростание функции 
                    if (AmplBlock[i] > LastMax) { LastMax = AmplBlock[i]; indexLastMax = i; CountForMax = 0; }
                    else { CountForMax++; }
                    if (CountForMax > PointMinMaxBase)
                    {// прошли через максимум 
                        CurrentPointIsMaximum = true;
                        if (LastMax - LastMin> TriggerAmplitide)
                        {
                            // должно быть условие попадания на линию
                            if (indexOldMax > 0) {
                                if (HitPointIQinLine(ref block, indexOldMax * 2, indexLastMax * 2, MaxPersentDeviation))
                                {
                                    if (RotationIndex.Count >= 1)
                                    {
                                        if (RotationIndex[RotationIndex.Count - 1] != indexOldMax)
                                        {
                                            RotationIndex.Add(indexOldMax);
                                        }
                                    }
                                    else
                                    {
                                        RotationIndex.Add(indexOldMax);
                                    }
                                    RotationIndex.Add(indexLastMax);
                                }
                            }
                        }
                        LastMin = LastMax;
                        indexOldMax = indexLastMax; 
                    }
                }
            }
        }

        static public void RotationCalculation(IQStreamTimeStampBloks.MethodForSelectCriticalPoint methodForSelectCriticalPoint, BlockOfSignal block, out List<int> RotationIndex, out List<double>RotationApml)
        {
            // определение констант для работы с методами 
            double PointMinMaxBase = 5; double TriggerAmplitideForFixRotationInProcent = 20; // амплитудный метод
            double TriggerAngleForChangeRotationDegree = 20; // фазовый метод
            bool CheckLine = true; double AngleDeviationDegreeForAmplitude = 10;// амплитудный метод
            RotationApml = new List<double>();

            RotationIndex = new List<int>();
            switch (methodForSelectCriticalPoint)
            {
                case IQStreamTimeStampBloks.MethodForSelectCriticalPoint.PhaseRotation:
                    RotationPhaseCalculation(ref block, out RotationIndex, out List<double> RotationPhase, TriggerAngleForChangeRotationDegree);
                    break;
                case IQStreamTimeStampBloks.MethodForSelectCriticalPoint.MaxLevel:
                    AmplitudePhaseCalculation(ref block, out RotationIndex, PointMinMaxBase, TriggerAmplitideForFixRotationInProcent, CheckLine, AngleDeviationDegreeForAmplitude);
                    break;
                case IQStreamTimeStampBloks.MethodForSelectCriticalPoint.SpeedChangeIQ:
                    CalcSpeedChangeIQChangeCalculationPhase(ref block, out RotationIndex);
                    break;
                case IQStreamTimeStampBloks.MethodForSelectCriticalPoint.TransitionByZero:
                    break;
                default:
                    break;
            }
            for (int i = 0; i <RotationIndex.Count; i++)
            {
                RotationApml.Add(Math.Sqrt(block.IQStream[2*RotationIndex[i]] * block.IQStream[2*RotationIndex[i]] + block.IQStream[2*RotationIndex[i] + 1] * block.IQStream[2*RotationIndex[i] + 1]));
            }
        }
        #endregion

        
       
        
        
      
        
    }
}
