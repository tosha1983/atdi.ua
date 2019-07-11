using Atdi.Contracts.Sdrn.DeviceServer;
using Atdi.DataModels.Sdrn.DeviceServer.Processing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.DataModels.Sdrns.Device;
using Atdi.DataModels.Sdrn.DeviceServer;
using Atdi.DataModels.Sdrn.DeviceServer.Commands.Parameters;
using Atdi.DataModels.Sdrn.DeviceServer.Commands.Results;
using Atdi.Platform.Logging;

namespace Atdi.AppUnits.Sdrn.DeviceServer.Processing.Measurements
{
    public static class CalcEmittingSummuryByEmittingDetailed
    {
        //Константа 
        private const double PersentForJoinDetailEmToSummEm = 20;

        // заготовка для SysInfo
        public static bool GetEmittingDetailedForSysInfo(ref Emitting[] emittingSummary, List<SysInfoResult> listSysinfoResult, ReferenceLevels referenceLevels, ILogger logger)
        {
            // List<SysInfoResult> listSysinfoResult - почему на входе список?
            // обновляем emittingSummary результатами новых измерений 
            for (int i = 0; listSysinfoResult.Count>i; i++)
            {
                var SysInfo = listSysinfoResult[i];

                for (int k = 0; SysInfo.signalingSysInfo.Length > k; k++)
                {
                    var oneSysInfo = SysInfo.signalingSysInfo[k];
                    double BWRes = 0;
                    if (oneSysInfo.BandWidth_Hz != null) {BWRes = oneSysInfo.BandWidth_Hz.Value; }
                    // далее цыкл для определения излучения которое больше всего подходит
                    double Min_penalty = 9999;
                    int CountInEmittingSummary = 0;
                    for (int j = 0; emittingSummary.Length > j; j++)
                    {
                        var emiting = emittingSummary[j];
                        double StartFreq;
                        double StopFreq;
                        if (emiting.Spectrum.СorrectnessEstimations == true)
                        {
                            if (emiting.Spectrum.T1 != 0)
                            {
                                StartFreq = emiting.Spectrum.SpectrumStartFreq_MHz + emiting.Spectrum.T1 * emiting.Spectrum.SpectrumSteps_kHz / 1000;
                            }
                            else
                            {
                                StartFreq = emiting.StartFrequency_MHz;
                            }
                            if (emiting.Spectrum.T2 != 0)
                            {
                                StopFreq = emiting.Spectrum.SpectrumStartFreq_MHz + emiting.Spectrum.T2 * emiting.Spectrum.SpectrumSteps_kHz / 1000;
                            }
                            else
                            {
                                StopFreq = emiting.StopFrequency_MHz;
                            }
                        }
                        else
                        {
                            StartFreq = emiting.StartFrequency_MHz;
                            StopFreq = emiting.StopFrequency_MHz;
                        }
                        double CentralFreq = (StartFreq + StopFreq) / 2;
                        double StartFreqD = ((double)oneSysInfo.Freq_Hz - BWRes/2)/1000000;
                        double StopFreqD = ((double)oneSysInfo.Freq_Hz + BWRes/2)/ 1000000;
                        double CentralFreqD = (StartFreqD + StopFreqD) / 2;
                        double BW = 1000000*(StopFreq - StartFreq);
                        if ((StartFreq < StopFreqD) && (StartFreqD < StopFreq))
                        {
                            double CurPenalty = Math.Abs(BWRes - BW) / BW + Math.Abs(CentralFreq - CentralFreqD) / (BW/1000000);
                            if (CurPenalty < Min_penalty) { Min_penalty = CurPenalty; CountInEmittingSummary = j; }
                        }
                    }
                    if (Min_penalty < PersentForJoinDetailEmToSummEm*3 / 100)
                    {
                        // присоединяемся к существующему емитингу
                        bool r = JoinSysInfoToEmitting(ref emittingSummary[CountInEmittingSummary], oneSysInfo);
                    }
                }
            }
            return true;
        }

        public static bool GetEmittingDetailed(ref Emitting[] emittingSummary, List<BWResult> listBWResult, ReferenceLevels referenceLevels, ILogger logger)
        {
            // обновляем emittingSummary результатами новых измерений
            for (int i = 0; listBWResult.Count > i; i++)
            {
                // на первом шаге мы должны определить индекс того излучения которому более всего соответсвует измерение
                var BWRes = listBWResult[i];
                double Min_penalty = 9999;
                int CountInEmittingSummary = 0;
                for (int j = 0; emittingSummary.Length > j; j++)
                {
                    var emiting = emittingSummary[j];
                    double StartFreq = emiting.Spectrum.SpectrumStartFreq_MHz + emiting.Spectrum.T1 * emiting.Spectrum.SpectrumSteps_kHz / 1000;
                    double StopFreq = emiting.Spectrum.SpectrumStartFreq_MHz + emiting.Spectrum.T2 * emiting.Spectrum.SpectrumSteps_kHz / 1000;
                    double CentralFreq = (StartFreq + StopFreq) / 2;
                    double BW = StopFreq - StartFreq;
                    double stepBW_Hz = (BWRes.Freq_Hz[BWRes.Freq_Hz.Length - 1] - BWRes.Freq_Hz[0]) / (BWRes.Freq_Hz.Length - 1);
                    double StartFreqD = (BWRes.Freq_Hz[0] + stepBW_Hz * BWRes.T1)/1000000;
                    double StopFreqD = (BWRes.Freq_Hz[0] + stepBW_Hz * BWRes.T2)/1000000;
                    double CentralFreqD = (StartFreqD + StopFreqD) / 2;
                    double BWD = StopFreqD - StartFreqD;
                    if (!((StartFreq > StopFreqD)|| (StartFreqD > StopFreq)))
                    {
                        double CurPenalty = Math.Abs(BWD - BW) / BWD + Math.Abs(CentralFreq - CentralFreqD) / BWD;
                        if (CurPenalty < Min_penalty) { Min_penalty = CurPenalty; CountInEmittingSummary = j;}
                    }
                }
                if (Min_penalty < PersentForJoinDetailEmToSummEm / 100)
                {
                    // присоединяемся к существующему емитингу
                    bool r = JoinBWResToEmitting(ref emittingSummary[CountInEmittingSummary], BWRes, referenceLevels);
                }
                else
                {
                    // создаем новый емитинг
                    var emitting = CreateEmittingFromBWRes(BWRes, referenceLevels, logger);
                    var listEmittingSummary = emittingSummary.ToList();
                    listEmittingSummary.Add(emitting);
                    emittingSummary = listEmittingSummary.ToArray();
                }
            }
            return true;
        }
        private static Emitting CreateEmittingFromBWRes(BWResult BWResult, ReferenceLevels referenceLevels, ILogger logger)
        {// НЕ ПРОВЕРЕННО

            Emitting emitting = new Emitting();
            double stepBW_Hz = (BWResult.Freq_Hz[BWResult.Freq_Hz.Length - 1] - BWResult.Freq_Hz[0]) / (BWResult.Freq_Hz.Length - 1);
            emitting.StartFrequency_MHz = (BWResult.Freq_Hz[0] + stepBW_Hz * BWResult.T1) / 1000000.0;
            emitting.StopFrequency_MHz = (BWResult.Freq_Hz[0] + stepBW_Hz * BWResult.T2) / 1000000.0;
            emitting.SpectrumIsDetailed = true;
            emitting.LastDetaileMeas = BWResult.TimeMeas;
            emitting.CurentPower_dBm = CommonCalcPowFromTrace.GetPow_dBm(BWResult);
            emitting.ReferenceLevel_dBm = CommonCalcPowFromTrace.GetPow_dBm(referenceLevels.levels, referenceLevels.StartFrequency_Hz, referenceLevels.StepFrequency_Hz, emitting.StartFrequency_MHz * 1000000, emitting.StopFrequency_MHz * 1000000);
            emitting.Spectrum = new Spectrum();
            emitting.Spectrum.Bandwidth_kHz = BWResult.Bandwidth_kHz;
            emitting.Spectrum.Levels_dBm = new float[BWResult.Levels_dBm.Length];
            BWResult.Levels_dBm.CopyTo(emitting.Spectrum.Levels_dBm, 0);
            emitting.Spectrum.MarkerIndex = BWResult.MarkerIndex;
            emitting.Spectrum.SignalLevel_dBm = (float)emitting.CurentPower_dBm;
            emitting.Spectrum.SpectrumStartFreq_MHz = BWResult.Freq_Hz[0] / 1000000.0;
            emitting.Spectrum.SpectrumSteps_kHz = stepBW_Hz/1000.0;
            emitting.Spectrum.T1 = BWResult.T1;
            emitting.Spectrum.T2 = BWResult.T2;
            emitting.Spectrum.TraceCount  = BWResult.TraceCount;
            emitting.Spectrum.СorrectnessEstimations = BWResult.СorrectnessEstimations;

            //вставка от 05.05.2019
            emitting.WorkTimes = new WorkTime[1];
            emitting.WorkTimes[0] = new WorkTime();
            emitting.WorkTimes[0].StartEmitting = BWResult.TimeMeas;
            emitting.WorkTimes[0].StopEmitting = BWResult.TimeMeas;
            emitting.WorkTimes[0].HitCount = 1;
            emitting.WorkTimes[0].ScanCount = 1;
            emitting.WorkTimes[0].TempCount = 0;
            emitting.WorkTimes[0].PersentAvailability = 100;

            Spectrum spectrum = emitting.Spectrum;
            bool chackSpecter = CalcSignalization.CheckContravention(ref spectrum, referenceLevels);
            emitting.Spectrum = spectrum;
            CalcSignalization.FillEmittingForStorage(emitting, logger);
            return emitting;
        }
        private static bool JoinBWResToEmitting(ref Emitting emitting, BWResult BWResult, ReferenceLevels referenceLevels)
        {
            double New_CurentPow = CommonCalcPowFromTrace.GetPow_dBm(BWResult);
            bool Use_new_spectr = false;
            
            // условие обновление спектра
            if (emitting.SpectrumIsDetailed == false) { Use_new_spectr = true; }
            else if (New_CurentPow > emitting.CurentPower_dBm) { Use_new_spectr = true; }
            // конец условиям обновления спектра
            if (Use_new_spectr)
            {
                double stepBW_Hz = (BWResult.Freq_Hz[BWResult.Freq_Hz.Length - 1] - BWResult.Freq_Hz[0]) / (BWResult.Freq_Hz.Length - 1);
                emitting.StartFrequency_MHz = (BWResult.Freq_Hz[0] + stepBW_Hz * BWResult.T1) / 1000000.0;
                emitting.StopFrequency_MHz = (BWResult.Freq_Hz[0] + stepBW_Hz * BWResult.T2) / 1000000.0;
                emitting.SpectrumIsDetailed = true;
                emitting.CurentPower_dBm = New_CurentPow;
                emitting.ReferenceLevel_dBm = CommonCalcPowFromTrace.GetPow_dBm(referenceLevels.levels, referenceLevels.StartFrequency_Hz, referenceLevels.StepFrequency_Hz, emitting.StartFrequency_MHz * 1000000, emitting.StopFrequency_MHz * 1000000);
                emitting.Spectrum = new Spectrum();
                emitting.Spectrum.Bandwidth_kHz = BWResult.Bandwidth_kHz;
                emitting.Spectrum.Levels_dBm = new float[BWResult.Levels_dBm.Length];
                BWResult.Levels_dBm.CopyTo(emitting.Spectrum.Levels_dBm, 0);
                emitting.Spectrum.MarkerIndex = BWResult.MarkerIndex;
                emitting.Spectrum.SignalLevel_dBm = (float)emitting.CurentPower_dBm;
                emitting.Spectrum.SpectrumStartFreq_MHz = BWResult.Freq_Hz[0] / 1000000.0;
                emitting.Spectrum.SpectrumSteps_kHz = stepBW_Hz / 1000.0;
                emitting.Spectrum.T1 = BWResult.T1;
                emitting.Spectrum.T2 = BWResult.T2;
                emitting.Spectrum.TraceCount = BWResult.TraceCount;
                emitting.Spectrum.СorrectnessEstimations = BWResult.СorrectnessEstimations;
                Spectrum spectrum = emitting.Spectrum;
                bool chackSpecter = CalcSignalization.CheckContravention(ref spectrum, referenceLevels);
                emitting.Spectrum = spectrum;
            }
            emitting.LastDetaileMeas = BWResult.TimeMeas;
            int indexLevel = (int)Math.Floor(emitting.CurentPower_dBm) - emitting.LevelsDistribution.Levels[0];
            if ((indexLevel >= 0) && (indexLevel < emitting.LevelsDistribution.Levels.Length)) { emitting.LevelsDistribution.Count[indexLevel]++; }
            if ((emitting.WorkTimes != null) && (emitting.WorkTimes.Length > 0))
            {
                emitting.WorkTimes[emitting.WorkTimes.Length - 1].StopEmitting = BWResult.TimeMeas;
                emitting.WorkTimes[emitting.WorkTimes.Length - 1].HitCount++;
                emitting.WorkTimes[emitting.WorkTimes.Length - 1].ScanCount = emitting.WorkTimes[emitting.WorkTimes.Length - 1].ScanCount + emitting.WorkTimes[emitting.WorkTimes.Length - 1].TempCount + 1;
                emitting.WorkTimes[emitting.WorkTimes.Length - 1].TempCount = 0;
                emitting.WorkTimes[emitting.WorkTimes.Length - 1].PersentAvailability = 100 * emitting.WorkTimes[emitting.WorkTimes.Length - 1].HitCount / emitting.WorkTimes[emitting.WorkTimes.Length - 1].ScanCount;
            }
            return true;
        }
        private static bool JoinSysInfoToEmitting(ref Emitting emitting, SignalingSysInfo sysInfoResult)
        { // Не тестированно 
            if (sysInfoResult == null) { return false; }
            // проверка на наличие такогоже излучения
            if (emitting.SysInfos != null)
            {
                bool ExistSysInfo = false;
                int indexSysInfo = 0;
                for (int i = 0; emitting.SysInfos.Length>i;  i++)
                {
                    if ((emitting.SysInfos[i].CID != null) &&(emitting.SysInfos[i].CID.Value == sysInfoResult.CID) && (Math.Abs(emitting.SysInfos[i].Freq_Hz - sysInfoResult.Freq_Hz)<10))
                    { indexSysInfo = i; ExistSysInfo = true; break; }
                }
                if (ExistSysInfo)
                {
                    // Такой елемент уже есть нужно откоректировать время.
                    var WorkTime = emitting.SysInfos[indexSysInfo].WorkTimes;
                    // Проверка на попадение в существующий диапазон времен
                    bool hit = false; 
                    for (int i = 0; WorkTime.Length >i; i++)
                    {
                        if ((sysInfoResult.WorkTimes[0].StartEmitting >= WorkTime[i].StartEmitting) && (sysInfoResult.WorkTimes[0].StartEmitting <= WorkTime[i].StopEmitting))
                        {
                            hit = true;
                            break;
                        }
                    }
                    if (!hit)
                    {
                        // вводим новое время и сортируем массив
                        var WorkTimeList = WorkTime.ToList();
                        WorkTimeList.Add(sysInfoResult.WorkTimes[0]);
                        WorkTimeList.Sort(delegate (WorkTime x, WorkTime y)
                        {
                            if (x.StartEmitting == y.StartEmitting)
                            {
                                if (x.StopEmitting > y.StopEmitting)
                                {
                                    return 1;
                                }
                            }
                            else
                            {
                                if (x.StartEmitting > y.StartEmitting)
                                {
                                    return 1;
                                }
                            }
                            return -1;
                        });
                        // Укрупняем список
                        for (int i = 0; WorkTimeList.Count - 1 > i; i++)
                        {
                            TimeSpan diff =  WorkTimeList[i + 1].StartEmitting - WorkTimeList[i].StopEmitting;
                            if (diff.TotalSeconds < 60)
                            {
                                // маленькое временное растояние следовательно производим укрупнение
                                WorkTimeList[i].StopEmitting = WorkTimeList[i + 1].StopEmitting;
                                WorkTimeList.RemoveAt(i + 1);
                                i = i - 1;
                            }

                        }
                    }
                }
                else
                {
                    var SysInfoAll = emitting.SysInfos.ToList();
                    SysInfoAll.Add(sysInfoResult);
                    emitting.SysInfos = SysInfoAll.ToArray();
                    // добавляем сис инфор как новый елемент масива 
                }
            }
            else
            {
                //вобще ничего нет создаем новое
                emitting.SysInfos = new SignalingSysInfo[1];
                emitting.SysInfos[0] = sysInfoResult;
            }
            return true;
        }
    }
}
