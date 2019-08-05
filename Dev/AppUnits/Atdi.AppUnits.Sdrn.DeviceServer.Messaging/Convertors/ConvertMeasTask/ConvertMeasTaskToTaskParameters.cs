using Atdi.Contracts.Sdrn.DeviceServer;
using Atdi.DataModels.Sdrn.DeviceServer.Processing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.DataModels.Sdrns.Device;
using Atdi.DataModels.Sdrn.DeviceServer;

namespace Atdi.AppUnits.Sdrn.DeviceServer.Messaging.Convertor
{
    public static class ConvertMeasTaskToTaskParameters
    {
        public static TaskParameters Convert(this MeasTask taskSDR, ConfigMessaging configMessaging)
        {
            const bool Smooth = true; // из клиента для BW
         
            // сигнализация
            bool? CompareTraceJustWithRefLevels = null;
            bool? AutoDivisionEmitting = null;
            double? DifferenceMaxMax = null;
            bool? FiltrationTrace = null;
            double? allowableExcess_dB = null;
            int? SignalizationNCount = null;
            int? SignalizationNChenal = null;

            bool? CorrelationAnalize = null;
            double? CorrelationFactor = null;
            bool? CheckFreqChannel = null;
            bool? AnalyzeByChannel = null;
            bool? AnalyzeSysInfoEmission = null;
            bool? DetailedMeasurementsBWEmission = null;
            string Standard = null;
            double? triggerLevel_dBm_Hz = null;
            int? NumberPointForChangeExcess = null;
            double? InterruptionllowableExcess_dB = null;
            double? windowBW = null;
            double? DiffLevelForCalcBW = null;
            double? nDbLevel_dB = null;
            int? NumberIgnoredPoints = null;
            double? MinExcessNoseLevel_dB = null;
            int? TimeBetweenWorkTimes_sec = null;
            int? TypeJoinSpectrum = null;
            double? CrossingBWPercentageForGoodSignals = null;
            double? CrossingBWPercentageForBadSignals = null;


            if (taskSDR.SignalingMeasTaskParameters != null)
            {
                ///////////// CompareTraceJustWithRefLevels /////////////
                if (taskSDR.SignalingMeasTaskParameters.CompareTraceJustWithRefLevels != null)
                {
                    CompareTraceJustWithRefLevels = taskSDR.SignalingMeasTaskParameters.CompareTraceJustWithRefLevels;
                }
                else
                {
                    CompareTraceJustWithRefLevels = configMessaging.CompareTraceJustWithRefLevels;
                }

                ///////////// AnalyzeByChannel /////////////
                if (taskSDR.SignalingMeasTaskParameters.AnalyzeByChannel != null)
                {
                    AnalyzeByChannel = taskSDR.SignalingMeasTaskParameters.AnalyzeByChannel;
                }
                else
                {
                    AnalyzeByChannel = configMessaging.AnalyzeByChannel;
                }

                ///////////// AnalyzeSysInfoEmission /////////////
                if (taskSDR.SignalingMeasTaskParameters.AnalyzeSysInfoEmission != null)
                {
                    AnalyzeSysInfoEmission = taskSDR.SignalingMeasTaskParameters.AnalyzeSysInfoEmission;
                }
                else
                {
                    AnalyzeSysInfoEmission = configMessaging.AnalyzeSysInfoEmission;
                }

                ///////////// CheckFreqChannel /////////////
                if (taskSDR.SignalingMeasTaskParameters.CheckFreqChannel != null)
                {
                    CheckFreqChannel = taskSDR.SignalingMeasTaskParameters.CheckFreqChannel;
                }
                else
                {
                    CheckFreqChannel = configMessaging.CheckFreqChannel;
                }

                ///////////// CorrelationAnalize /////////////
                if (taskSDR.SignalingMeasTaskParameters.CorrelationAnalize != null)
                {
                    CorrelationAnalize = taskSDR.SignalingMeasTaskParameters.CorrelationAnalize;
                }
                else
                {
                    CorrelationAnalize = configMessaging.CorrelationAnalize;
                }

                ///////////// CorrelationFactor /////////////
                if (taskSDR.SignalingMeasTaskParameters.CorrelationFactor != null)
                {
                    CorrelationFactor = taskSDR.SignalingMeasTaskParameters.CorrelationFactor;
                }
                else
                {
                    CorrelationFactor = configMessaging.CorrelationFactor;
                }

                ///////////// DetailedMeasurementsBWEmission /////////////
                if (taskSDR.SignalingMeasTaskParameters.DetailedMeasurementsBWEmission != null)
                {
                    DetailedMeasurementsBWEmission = taskSDR.SignalingMeasTaskParameters.DetailedMeasurementsBWEmission;
                }
                else
                {
                    DetailedMeasurementsBWEmission = configMessaging.DetailedMeasurementsBWEmission;
                }

                ///////////// Standard /////////////
                if (taskSDR.SignalingMeasTaskParameters.Standard != null)
                {
                    Standard = taskSDR.SignalingMeasTaskParameters.Standard;
                }
                else
                {
                    Standard = configMessaging.Standard;
                }

                ///////////// triggerLevel_dBm_Hz /////////////
                if (taskSDR.SignalingMeasTaskParameters.triggerLevel_dBm_Hz != null)
                {
                    triggerLevel_dBm_Hz = taskSDR.SignalingMeasTaskParameters.triggerLevel_dBm_Hz;
                }
                else
                {
                    triggerLevel_dBm_Hz = configMessaging.triggerLevel_dBm_Hz;
                }

                if (taskSDR.SignalingMeasTaskParameters.GroupingParameters != null)
                {
                    ///////////// CrossingBWPercentageForBadSignals /////////////
                    if (taskSDR.SignalingMeasTaskParameters.GroupingParameters.CrossingBWPercentageForBadSignals != null)
                    {
                        CrossingBWPercentageForBadSignals = taskSDR.SignalingMeasTaskParameters.GroupingParameters.CrossingBWPercentageForBadSignals;
                    }
                    else
                    {
                        CrossingBWPercentageForBadSignals = configMessaging.CrossingBWPercentageForBadSignals;
                    }

                    ///////////// CrossingBWPercentageForGoodSignals /////////////
                    if (taskSDR.SignalingMeasTaskParameters.GroupingParameters.CrossingBWPercentageForGoodSignals != null)
                    {
                        CrossingBWPercentageForGoodSignals = taskSDR.SignalingMeasTaskParameters.GroupingParameters.CrossingBWPercentageForGoodSignals;
                    }
                    else
                    {
                        CrossingBWPercentageForGoodSignals = configMessaging.CrossingBWPercentageForGoodSignals;
                    }

                    ///////////// TimeBetweenWorkTimes_sec /////////////
                    if (taskSDR.SignalingMeasTaskParameters.GroupingParameters.TimeBetweenWorkTimes_sec != null)
                    {
                        TimeBetweenWorkTimes_sec = taskSDR.SignalingMeasTaskParameters.GroupingParameters.TimeBetweenWorkTimes_sec;
                    }
                    else
                    {
                        TimeBetweenWorkTimes_sec = configMessaging.TimeBetweenWorkTimes_sec;
                    }

                    ///////////// TypeJoinSpectrum /////////////
                    if (taskSDR.SignalingMeasTaskParameters.GroupingParameters.TypeJoinSpectrum != null)
                    {
                        TypeJoinSpectrum = taskSDR.SignalingMeasTaskParameters.GroupingParameters.TypeJoinSpectrum;
                    }
                    else
                    {
                        TypeJoinSpectrum = configMessaging.TypeJoinSpectrum;
                    }
                }
                else
                {
                    TypeJoinSpectrum = configMessaging.TypeJoinSpectrum;
                    TimeBetweenWorkTimes_sec = configMessaging.TimeBetweenWorkTimes_sec;
                    CrossingBWPercentageForGoodSignals = configMessaging.CrossingBWPercentageForGoodSignals;
                    CrossingBWPercentageForBadSignals = configMessaging.CrossingBWPercentageForBadSignals;
                }

                if (taskSDR.SignalingMeasTaskParameters.InterruptionParameters!= null)
                {
                    ///////////// AutoDivisionEmitting /////////////
                    if (taskSDR.SignalingMeasTaskParameters.InterruptionParameters.AutoDivisionEmitting != null)
                    {
                        AutoDivisionEmitting = taskSDR.SignalingMeasTaskParameters.InterruptionParameters.AutoDivisionEmitting;
                    }
                    else
                    {
                        AutoDivisionEmitting = configMessaging.AutoDivisionEmitting;
                    }
                    
                    ///////////// DifferenceMaxMax /////////////
                    if (taskSDR.SignalingMeasTaskParameters.InterruptionParameters.DifferenceMaxMax != null)
                    {
                        DifferenceMaxMax = taskSDR.SignalingMeasTaskParameters.InterruptionParameters.DifferenceMaxMax;
                    }
                    else
                    {
                        DifferenceMaxMax = configMessaging.DifferenceMaxMax;
                    }
                    
                    ///////////// InterruptionllowableExcess_dB /////////////
                    if (taskSDR.SignalingMeasTaskParameters.InterruptionParameters.allowableExcess_dB != null)
                    {
                        InterruptionllowableExcess_dB = taskSDR.SignalingMeasTaskParameters.InterruptionParameters.allowableExcess_dB;
                    }
                    else
                    {
                        InterruptionllowableExcess_dB = configMessaging.allowableExcess_dB;
                    }

                    ///////////// DiffLevelForCalcBW /////////////
                    if (taskSDR.SignalingMeasTaskParameters.InterruptionParameters.DiffLevelForCalcBW != null)
                    {
                        DiffLevelForCalcBW = taskSDR.SignalingMeasTaskParameters.InterruptionParameters.DiffLevelForCalcBW;
                    }
                    else
                    {
                        DiffLevelForCalcBW = configMessaging.DiffLevelForCalcBW;
                    }

                    ///////////// MinExcessNoseLevel_dB /////////////
                    if (taskSDR.SignalingMeasTaskParameters.InterruptionParameters.MinExcessNoseLevel_dB != null)
                    {
                        MinExcessNoseLevel_dB = taskSDR.SignalingMeasTaskParameters.InterruptionParameters.MinExcessNoseLevel_dB;
                    }
                    else
                    {
                        MinExcessNoseLevel_dB = configMessaging.MinExcessNoseLevel_dB;
                    }

                    ///////////// nDbLevel_dB /////////////
                    if (taskSDR.SignalingMeasTaskParameters.InterruptionParameters.nDbLevel_dB != null)
                    {
                        nDbLevel_dB = taskSDR.SignalingMeasTaskParameters.InterruptionParameters.nDbLevel_dB;
                    }
                    else
                    {
                        nDbLevel_dB = configMessaging.nDbLevel_dB;
                    }

                    ///////////// NumberIgnoredPoints /////////////
                    if (taskSDR.SignalingMeasTaskParameters.InterruptionParameters.NumberIgnoredPoints != null)
                    {
                        NumberIgnoredPoints = taskSDR.SignalingMeasTaskParameters.InterruptionParameters.NumberIgnoredPoints;
                    }
                    else
                    {
                        NumberIgnoredPoints = configMessaging.NumberIgnoredPoints;
                    }

                    ///////////// NumberPointForChangeExcess /////////////
                    if (taskSDR.SignalingMeasTaskParameters.InterruptionParameters.NumberPointForChangeExcess != null)
                    {
                        NumberPointForChangeExcess = taskSDR.SignalingMeasTaskParameters.InterruptionParameters.NumberPointForChangeExcess;
                    }
                    else
                    {
                        NumberPointForChangeExcess = configMessaging.NumberPointForChangeExcess;
                    }

                    ///////////// windowBW /////////////
                    if (taskSDR.SignalingMeasTaskParameters.InterruptionParameters.windowBW != null)
                    {
                        windowBW = taskSDR.SignalingMeasTaskParameters.InterruptionParameters.windowBW;
                    }
                    else
                    {
                        windowBW = configMessaging.windowBW;
                    }

                    
                }
                else
                {
                    AutoDivisionEmitting = configMessaging.AutoDivisionEmitting;
                    DifferenceMaxMax = configMessaging.DifferenceMaxMax;
                    InterruptionllowableExcess_dB = configMessaging.InterruptionAllowableExcess_dB;
                    DiffLevelForCalcBW = configMessaging.DiffLevelForCalcBW;
                    MinExcessNoseLevel_dB = configMessaging.MinExcessNoseLevel_dB;
                    nDbLevel_dB = configMessaging.nDbLevel_dB;
                    NumberIgnoredPoints = configMessaging.NumberIgnoredPoints;
                    NumberPointForChangeExcess = configMessaging.NumberPointForChangeExcess;
                    windowBW = configMessaging.windowBW;
                }

                

                if (taskSDR.SignalingMeasTaskParameters.FiltrationTrace != null)
                {
                    FiltrationTrace = taskSDR.SignalingMeasTaskParameters.FiltrationTrace;
                }
                else
                {
                    FiltrationTrace = configMessaging.FiltrationTrace;
                }


                if (taskSDR.SignalingMeasTaskParameters.allowableExcess_dB != null)
                {
                    allowableExcess_dB = taskSDR.SignalingMeasTaskParameters.allowableExcess_dB;
                }
                else
                {
                    allowableExcess_dB = configMessaging.allowableExcess_dB;
                }

                if (taskSDR.SignalingMeasTaskParameters.SignalizationNCount != null)
                {
                    SignalizationNCount = taskSDR.SignalingMeasTaskParameters.SignalizationNCount;
                }
                else
                {
                    SignalizationNCount = configMessaging.SignalizationNCount;
                }


                if (taskSDR.SignalingMeasTaskParameters.SignalizationNChenal != null)
                {
                    SignalizationNChenal = taskSDR.SignalingMeasTaskParameters.SignalizationNChenal;
                }
                else
                {
                    SignalizationNChenal = configMessaging.SignalizationNChenal;
                }

            }
            else
            {
                CompareTraceJustWithRefLevels = configMessaging.CompareTraceJustWithRefLevels;
                FiltrationTrace = configMessaging.FiltrationTrace;
                allowableExcess_dB = configMessaging.allowableExcess_dB;
                SignalizationNCount = configMessaging.SignalizationNCount;
                SignalizationNChenal = configMessaging.SignalizationNChenal;
                AnalyzeByChannel = configMessaging.AnalyzeByChannel;
                AnalyzeSysInfoEmission = configMessaging.AnalyzeSysInfoEmission;
                CheckFreqChannel = configMessaging.CheckFreqChannel;
                CorrelationAnalize = configMessaging.CorrelationAnalize;
                CorrelationFactor = configMessaging.CorrelationFactor;
                DetailedMeasurementsBWEmission = configMessaging.DetailedMeasurementsBWEmission;
                Standard = configMessaging.Standard;
                triggerLevel_dBm_Hz = configMessaging.triggerLevel_dBm_Hz;
            }

            double PercentForCalcNoise = configMessaging.PercentForCalcNoise;

            /*
            bool? CompareTraceJustWithRefLevels = false; // из клиента
            bool?  AutoDivisionEmitting = true; // из клиента
            double? DifferenceMaxMax = 20; // из клиента
            bool? FiltrationTrace = true; // из клиента 
            double? allowableExcess_dB = 10; // из клиента
            int? SignalizationNCount = 1000000; // из клиента
            int? SignalizationNChenal = 50; // из клиента
            */
           
            // сигнализация



            const int SO_Ncount = 10000;
            const int OtherNCount = 1000;
            const int SO_NChenal = 10;


            var taskParameters = new TaskParameters();
            taskParameters.SignalingMeasTaskParameters = new DataModels.Sdrn.DeviceServer.Processing.SignalingMeasTask();
            taskParameters.SignalingMeasTaskParameters.GroupingParameters = new DataModels.Sdrn.DeviceServer.Processing.SignalingGroupingParameters();
            taskParameters.SignalingMeasTaskParameters.InterruptionParameters = new DataModels.Sdrn.DeviceServer.Processing.SignalingInterruptionParameters();
            taskParameters.Smooth = Smooth;

            //
            //taskParameters.NCount = необходимо вичислить

            taskParameters.StartTime = taskSDR.StartTime;
            taskParameters.StopTime = taskSDR.StopTime;
            if (taskSDR.Frequencies != null)
            {
                if (taskSDR.Frequencies.Values_MHz != null)
                {
                    taskParameters.ListFreqCH = new List<double>();
                    taskParameters.ListFreqCH.Clear();
                    for (int i = 0; i < taskSDR.Frequencies.Values_MHz.Length; i++)
                    {
                        var freq = taskSDR.Frequencies.Values_MHz[i];
                        taskParameters.ListFreqCH.Add(freq);
                    }
                }
                if ((taskSDR.Frequencies.RgL_MHz == null) || (taskSDR.Frequencies.RgU_MHz == null)) // если вдруг отсутвуют начало и конец
                {
                    if (taskParameters.ListFreqCH != null)
                    {
                        if (taskParameters.ListFreqCH.Count > 0)
                        {
                            taskParameters.ListFreqCH.Sort();
                            taskParameters.MinFreq_MHz = taskParameters.ListFreqCH[0];
                            taskParameters.MaxFreq_MHz = taskParameters.ListFreqCH[taskParameters.ListFreqCH.Count - 1];
                        }
                    }
                    else { taskParameters.MinFreq_MHz = 100; taskParameters.MaxFreq_MHz = 110; }
                }
                else
                {
                    taskParameters.MaxFreq_MHz = taskSDR.Frequencies.RgU_MHz.Value;
                    taskParameters.MinFreq_MHz = taskSDR.Frequencies.RgL_MHz.Value;
                }
                if (taskSDR.Frequencies.Step_kHz != null) { taskParameters.StepSO_kHz = taskSDR.Frequencies.Step_kHz.Value; } // обязательный параметер для SO (типа ширина канала или шаг сетки частот)
            }
            if (taskSDR.DeviceParam != null)
            {
                if (taskSDR.DeviceParam.RBW_kHz != 0) { taskParameters.RBW_Hz = taskSDR.DeviceParam.RBW_kHz * 1000; } else { taskParameters.RBW_Hz = 10000; }
                if (taskSDR.DeviceParam.VBW_kHz != 0) { taskParameters.VBW_Hz = taskSDR.DeviceParam.VBW_kHz.Value * 1000; } else { taskParameters.VBW_Hz = 10000; }
            }
            if (taskSDR.DeviceParam.MeasTime_sec != null) { taskParameters.SweepTime_s = taskSDR.DeviceParam.MeasTime_sec.Value; } else { taskParameters.SweepTime_s = 0.0001; }

            if (taskSDR.SOParam != null)
            {
                if ((taskSDR.DeviceParam.MeasTime_sec == null) || (taskSDR.Interval_sec == 0) || ((taskSDR.DeviceParam.MeasTime_sec != null) && (taskSDR.DeviceParam.MeasTime_sec == 0)))
                {
                    switch (taskSDR.Measurement)
                    {
                        case DataModels.Sdrns.MeasurementType.SpectrumOccupation:
                            taskParameters.NCount = SO_Ncount;
                            break;
                        case DataModels.Sdrns.MeasurementType.Signaling:
                            taskParameters.NCount = SignalizationNCount.Value;
                            break;
                        default:
                            taskParameters.NCount = OtherNCount;
                            break;
                    }
                }
                else
                {
                    taskParameters.NCount = (int)((double)taskSDR.Interval_sec / (double)taskSDR.DeviceParam.MeasTime_sec);
                }
                var sOtype = GetSOTypeFromSpectrumOccupationType(taskSDR.SOParam.Type);
                if (taskSDR.Measurement == DataModels.Sdrns.MeasurementType.SpectrumOccupation)
                {
                    if ((taskSDR.SOParam.Type == DataModels.Sdrns.SpectrumOccupationType.FreqBandOccupancy) || (taskSDR.SOParam.Type == DataModels.Sdrns.SpectrumOccupationType.FreqChannelOccupancy))
                    {
                        if ((taskSDR.SOParam.MeasurmentNumber > 0) && (taskSDR.SOParam.MeasurmentNumber < 1000)) { taskParameters.NChenal = taskSDR.SOParam.MeasurmentNumber; } else { taskParameters.NChenal = SO_NChenal; }
                        if (taskSDR.SOParam.LevelMinOccup_dBm <= 0) { taskParameters.LevelMinOccup_dBm = taskSDR.SOParam.LevelMinOccup_dBm; } else { taskParameters.LevelMinOccup_dBm = -80; }
                        taskParameters.TypeOfSO = sOtype;
                        if ((taskParameters.ListFreqCH != null) && (taskParameters.ListFreqCH.Count > 0))
                        {
                            // формируем начало и конец для измерений 
                            taskParameters.ListFreqCH.Sort();
                            taskParameters.MinFreq_MHz = taskParameters.ListFreqCH[0] - taskParameters.StepSO_kHz / 2000;
                            taskParameters.MaxFreq_MHz = taskParameters.ListFreqCH[taskParameters.ListFreqCH.Count - 1] + taskParameters.StepSO_kHz / 2000;
                        }
                        // расчитываем желаемое RBW и VBW
                        taskParameters.VBW_Hz = taskParameters.StepSO_kHz * 1000 / taskParameters.NChenal;
                        taskParameters.RBW_Hz = taskParameters.StepSO_kHz * 1000 / taskParameters.NChenal;
                    }
                }
                else if (taskSDR.Measurement == DataModels.Sdrns.MeasurementType.Signaling)
                {
                    //if ((taskSDR.SOParam.MeasurmentNumber > 0) && (taskSDR.SOParam.MeasurmentNumber < 1000)) { taskParameters.NChenal = taskSDR.SOParam.MeasurmentNumber; } else { taskParameters.NChenal = SignalizationNChenal; }
                    //taskParameters.NChenal = SignalizationNChenal.Value;
                    if ((taskParameters.ListFreqCH != null) && (taskParameters.ListFreqCH.Count > 0))
                    {
                        // формируем начало и конец для измерений 
                        taskParameters.ListFreqCH.Sort();
                        taskParameters.MinFreq_MHz = taskParameters.ListFreqCH[0] - taskParameters.StepSO_kHz / 2000;
                        taskParameters.MaxFreq_MHz = taskParameters.ListFreqCH[taskParameters.ListFreqCH.Count - 1] + taskParameters.StepSO_kHz / 2000;
                    }
                }
            }
            // коректировка режима измерения 
            taskParameters.SDRTaskId = taskSDR.TaskId;
            taskParameters.MeasurementType = GetMeasTypeFromMeasurementType(taskSDR.Measurement);
            taskParameters.status = taskSDR.Status;

            if (taskSDR.Measurement == DataModels.Sdrns.MeasurementType.Signaling)
            {
                if (CompareTraceJustWithRefLevels != null)
                {
                    taskParameters.SignalingMeasTaskParameters.CompareTraceJustWithRefLevels = CompareTraceJustWithRefLevels.Value;
                }
                if (FiltrationTrace != null)
                {
                    taskParameters.SignalingMeasTaskParameters.FiltrationTrace = FiltrationTrace.Value;
                }
                if (allowableExcess_dB != null)
                {
                    taskParameters.SignalingMeasTaskParameters.allowableExcess_dB = allowableExcess_dB.Value;
                }
                if (CorrelationAnalize != null)
                {
                    taskParameters.SignalingMeasTaskParameters.CorrelationAnalize = CorrelationAnalize.Value;
                }
                if (CorrelationFactor != null)
                {
                    taskParameters.SignalingMeasTaskParameters.CorrelationFactor = CorrelationFactor.Value;
                }
                if (CheckFreqChannel != null)
                {
                    taskParameters.SignalingMeasTaskParameters.CheckFreqChannel = CheckFreqChannel.Value;
                }
                if (AnalyzeByChannel != null)
                {
                    taskParameters.SignalingMeasTaskParameters.AnalyzeByChannel = AnalyzeByChannel.Value;
                }
                if (AnalyzeSysInfoEmission != null)
                {
                    taskParameters.SignalingMeasTaskParameters.AnalyzeSysInfoEmission = AnalyzeSysInfoEmission.Value;
                }
                if (DetailedMeasurementsBWEmission != null)
                {
                    taskParameters.SignalingMeasTaskParameters.DetailedMeasurementsBWEmission = DetailedMeasurementsBWEmission.Value;
                }
                if (Standard != null)
                {
                    taskParameters.SignalingMeasTaskParameters.Standard = Standard;
                }
                if (triggerLevel_dBm_Hz != null)
                {
                    taskParameters.SignalingMeasTaskParameters.triggerLevel_dBm_Hz = triggerLevel_dBm_Hz.Value;
                }
                if (triggerLevel_dBm_Hz != null)
                {
                    taskParameters.SignalingMeasTaskParameters.triggerLevel_dBm_Hz = triggerLevel_dBm_Hz.Value;
                }

                if (AutoDivisionEmitting != null)
                {
                    taskParameters.SignalingMeasTaskParameters.InterruptionParameters.AutoDivisionEmitting = AutoDivisionEmitting.Value;
                }
                if (DifferenceMaxMax != null)
                {
                    taskParameters.SignalingMeasTaskParameters.InterruptionParameters.DifferenceMaxMax = DifferenceMaxMax.Value;
                }
                if (NumberPointForChangeExcess != null)
                {
                    taskParameters.SignalingMeasTaskParameters.InterruptionParameters.NumberPointForChangeExcess = NumberPointForChangeExcess.Value;
                }
                if (InterruptionllowableExcess_dB != null)
                {
                    taskParameters.SignalingMeasTaskParameters.InterruptionParameters.allowableExcess_dB = InterruptionllowableExcess_dB.Value;
                }
                if (windowBW != null)
                {
                    taskParameters.SignalingMeasTaskParameters.InterruptionParameters.windowBW = windowBW.Value;
                }
                if (DiffLevelForCalcBW != null)
                {
                    taskParameters.SignalingMeasTaskParameters.InterruptionParameters.DiffLevelForCalcBW = DiffLevelForCalcBW.Value;
                }
                if (nDbLevel_dB != null)
                {
                    taskParameters.SignalingMeasTaskParameters.InterruptionParameters.nDbLevel_dB = nDbLevel_dB.Value;
                }
                if (NumberIgnoredPoints != null)
                {
                    taskParameters.SignalingMeasTaskParameters.InterruptionParameters.NumberIgnoredPoints = NumberIgnoredPoints.Value;
                }
                if (MinExcessNoseLevel_dB != null)
                {
                    taskParameters.SignalingMeasTaskParameters.InterruptionParameters.MinExcessNoseLevel_dB = MinExcessNoseLevel_dB.Value;
                }


                if (TimeBetweenWorkTimes_sec != null)
                {
                    taskParameters.SignalingMeasTaskParameters.GroupingParameters.TimeBetweenWorkTimes_sec = TimeBetweenWorkTimes_sec.Value;
                }
                if (TypeJoinSpectrum != null)
                {
                    taskParameters.SignalingMeasTaskParameters.GroupingParameters.TypeJoinSpectrum = TypeJoinSpectrum.Value;
                }
                if (CrossingBWPercentageForGoodSignals != null)
                {
                    taskParameters.SignalingMeasTaskParameters.GroupingParameters.CrossingBWPercentageForGoodSignals = CrossingBWPercentageForGoodSignals.Value;
                }
                if (CrossingBWPercentageForBadSignals != null)
                {
                    taskParameters.SignalingMeasTaskParameters.GroupingParameters.CrossingBWPercentageForBadSignals = CrossingBWPercentageForBadSignals.Value;
                }


                taskParameters.PercentForCalcNoise = PercentForCalcNoise;
                if (SignalizationNChenal != null)
                {
                    taskParameters.NChenal = SignalizationNChenal.Value;
                }
                if (SignalizationNCount != null)
                {
                    taskParameters.NCount = SignalizationNCount.Value;
                }
                
              

                //taskParameters.NCount = SignalizationNCount.Value;

                if (taskSDR.RefSituation != null)
                {
                    var listReferenceSituation = new List<ReferenceSituation>();
                    {
                        var refSituation = new ReferenceSituation();
                        var refSituationTemp = taskSDR.RefSituation;
                        refSituation.SensorId = refSituationTemp.SensorId;
                        var referenceSignal = refSituationTemp.ReferenceSignal;
                        if (referenceSignal.Length > 0)
                        {
                            refSituation.ReferenceSignal = new ReferenceSignal[referenceSignal.Length];
                            for (int l = 0; l < referenceSignal.Length; l++)
                            {
                                var refSituationReferenceSignal = refSituation.ReferenceSignal[l];
                                refSituationReferenceSignal = new ReferenceSignal();
                                refSituationReferenceSignal.Bandwidth_kHz = referenceSignal[l].Bandwidth_kHz;
                                refSituationReferenceSignal.Frequency_MHz = referenceSignal[l].Frequency_MHz;
                                refSituationReferenceSignal.LevelSignal_dBm = referenceSignal[l].LevelSignal_dBm;
                                refSituationReferenceSignal.SignalMask = new SignalMask();
                                if (referenceSignal[l].SignalMask != null)
                                {
                                    refSituationReferenceSignal.SignalMask.Freq_kHz = referenceSignal[l].SignalMask.Freq_kHz;
                                    refSituationReferenceSignal.SignalMask.Loss_dB = referenceSignal[l].SignalMask.Loss_dB;
                                }
                                refSituation.ReferenceSignal[l] = refSituationReferenceSignal;
                            }
                        }
                        listReferenceSituation.Add(refSituation);
                    }
                    if (listReferenceSituation.Count > 0)
                    {
                        taskParameters.RefSituation = listReferenceSituation[0];
                    }
                }
            }

            taskParameters.TypeTechnology = TypeTechnology.Unknown;
            // до конца не определенные блоки
            taskParameters.ReceivedIQStreemDuration_sec = 1.0;
            taskParameters.SensorId = taskSDR.SensorId;
            return taskParameters;
        }

        private static SOType GetSOTypeFromSpectrumOccupationType(DataModels.Sdrns.SpectrumOccupationType spectrumOccupationType)
        {
            SOType sOType;
            switch (spectrumOccupationType)
            {
                case DataModels.Sdrns.SpectrumOccupationType.FreqBandOccupancy:
                    sOType = SOType.FreqBandwidthOccupation;
                    break;
                case DataModels.Sdrns.SpectrumOccupationType.FreqChannelOccupancy:
                    sOType = SOType.FreqChannelOccupation;
                    break;
                default:
                    sOType = SOType.FreqChannelOccupation;
                    break;
            }
            return (sOType);
        }

        private static MeasType GetMeasTypeFromMeasurementType(DataModels.Sdrns.MeasurementType measurementType)
        {
            MeasType measType;
            switch (measurementType)
            {
                case DataModels.Sdrns.MeasurementType.BandwidthMeas:
                    measType = MeasType.BandwidthMeas;
                    break;
                case DataModels.Sdrns.MeasurementType.Level:
                    measType = MeasType.Level;
                    break;
                case DataModels.Sdrns.MeasurementType.SpectrumOccupation:
                    measType = MeasType.SpectrumOccupation;
                    break;
                case DataModels.Sdrns.MeasurementType.MonitoringStations:
                    measType = MeasType.MonitoringStations;
                    break;
                case DataModels.Sdrns.MeasurementType.Signaling:
                    measType = MeasType.Signaling;
                    break;
                default:
                    measType = MeasType.Level;
                    break;
            }
            return (measType);
        }

    }
}
