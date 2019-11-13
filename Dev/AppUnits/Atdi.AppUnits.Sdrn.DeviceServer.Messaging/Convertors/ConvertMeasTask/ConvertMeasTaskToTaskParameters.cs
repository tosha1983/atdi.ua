using Atdi.Contracts.Sdrn.DeviceServer;
using Proc = Atdi.DataModels.Sdrn.DeviceServer.Processing;
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

        public static Proc.SignalingMeasTask GetSignalizationParameters(this MeasTask taskSDR, ConfigMessaging configMessaging)
        {
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
            double? windowBW = null;
            double? DiffLevelForCalcBW = null;
            double? nDbLevel_dB = null;
            int? NumberIgnoredPoints = null;
            double? MinExcessNoseLevel_dB = null;
            int? TimeBetweenWorkTimes_sec = null;
            int? TypeJoinSpectrum = null;
            double? CrossingBWPercentageForGoodSignals = null;
            double? CrossingBWPercentageForBadSignals = null;
            double? MaxFreqDeviation = null;
            bool? CheckLevelChannel = null;
            int? MinPointForDetailBW = null;



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

                if (taskSDR.SignalingMeasTaskParameters.InterruptionParameters != null)
                {

                    ///////////// AutoDivisionEmitting /////////////
                    if (taskSDR.SignalingMeasTaskParameters.InterruptionParameters.MaxFreqDeviation != null)
                    {
                        MaxFreqDeviation = taskSDR.SignalingMeasTaskParameters.InterruptionParameters.MaxFreqDeviation;
                    }
                    else
                    {
                        MaxFreqDeviation = configMessaging.MaxFreqDeviation;
                    }

                    ///////////// CheckLevelChannel /////////////
                    if (taskSDR.SignalingMeasTaskParameters.InterruptionParameters.CheckLevelChannel != null)
                    {
                        CheckLevelChannel = taskSDR.SignalingMeasTaskParameters.InterruptionParameters.CheckLevelChannel;
                    }
                    else
                    {
                        CheckLevelChannel = configMessaging.CheckLevelChannel;
                    }

                    ///////////// CheckLevelChannel /////////////
                    if (taskSDR.SignalingMeasTaskParameters.InterruptionParameters.MinPointForDetailBW != null)
                    {
                        MinPointForDetailBW = taskSDR.SignalingMeasTaskParameters.InterruptionParameters.MinPointForDetailBW;
                    }
                    else
                    {
                        MinPointForDetailBW = configMessaging.MinPointForDetailBW;
                    }


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
                    DiffLevelForCalcBW = configMessaging.DiffLevelForCalcBW;
                    MinExcessNoseLevel_dB = configMessaging.MinExcessNoseLevel_dB;
                    nDbLevel_dB = configMessaging.nDbLevel_dB;
                    NumberIgnoredPoints = configMessaging.NumberIgnoredPoints;
                    NumberPointForChangeExcess = configMessaging.NumberPointForChangeExcess;
                    windowBW = configMessaging.windowBW;
                    MaxFreqDeviation = configMessaging.MaxFreqDeviation;
                    CheckLevelChannel = configMessaging.CheckLevelChannel;
                    MinPointForDetailBW = configMessaging.MinPointForDetailBW;
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

            var signalingMeasTask = new Proc.SignalingMeasTask();
            signalingMeasTask = new DataModels.Sdrn.DeviceServer.Processing.SignalingMeasTask();
            signalingMeasTask.GroupingParameters = new DataModels.Sdrn.DeviceServer.Processing.SignalingGroupingParameters();
            signalingMeasTask.InterruptionParameters = new DataModels.Sdrn.DeviceServer.Processing.SignalingInterruptionParameters();

            if (CompareTraceJustWithRefLevels != null)
            {
                signalingMeasTask.CompareTraceJustWithRefLevels = CompareTraceJustWithRefLevels.Value;
            }
            if (FiltrationTrace != null)
            {
                signalingMeasTask.FiltrationTrace = FiltrationTrace.Value;
            }
            if (allowableExcess_dB != null)
            {
                signalingMeasTask.allowableExcess_dB = allowableExcess_dB.Value;
            }
            if (CorrelationAnalize != null)
            {
                signalingMeasTask.CorrelationAnalize = CorrelationAnalize.Value;
            }
            if (CorrelationFactor != null)
            {
                signalingMeasTask.CorrelationFactor = CorrelationFactor.Value;
            }
            if (CheckFreqChannel != null)
            {
                signalingMeasTask.CheckFreqChannel = CheckFreqChannel.Value;
            }
            if (AnalyzeByChannel != null)
            {
                signalingMeasTask.AnalyzeByChannel = AnalyzeByChannel.Value;
            }
            if (AnalyzeSysInfoEmission != null)
            {
                signalingMeasTask.AnalyzeSysInfoEmission = AnalyzeSysInfoEmission.Value;
            }
            if (DetailedMeasurementsBWEmission != null)
            {
                signalingMeasTask.DetailedMeasurementsBWEmission = DetailedMeasurementsBWEmission.Value;
            }
            if (Standard != null)
            {
                signalingMeasTask.Standard = Standard;
            }
            if (triggerLevel_dBm_Hz != null)
            {
                signalingMeasTask.triggerLevel_dBm_Hz = triggerLevel_dBm_Hz.Value;
            }
            if (triggerLevel_dBm_Hz != null)
            {
                signalingMeasTask.triggerLevel_dBm_Hz = triggerLevel_dBm_Hz.Value;
            }

            if (AutoDivisionEmitting != null)
            {
                signalingMeasTask.InterruptionParameters.AutoDivisionEmitting = AutoDivisionEmitting.Value;
            }
            if (DifferenceMaxMax != null)
            {
                signalingMeasTask.InterruptionParameters.DifferenceMaxMax = DifferenceMaxMax.Value;
            }
            if (NumberPointForChangeExcess != null)
            {
                signalingMeasTask.InterruptionParameters.NumberPointForChangeExcess = NumberPointForChangeExcess.Value;
            }
            if (windowBW != null)
            {
                signalingMeasTask.InterruptionParameters.windowBW = windowBW.Value;
            }
            if (DiffLevelForCalcBW != null)
            {
                signalingMeasTask.InterruptionParameters.DiffLevelForCalcBW = DiffLevelForCalcBW.Value;
            }
            if (nDbLevel_dB != null)
            {
                signalingMeasTask.InterruptionParameters.nDbLevel_dB = nDbLevel_dB.Value;
            }
            if (NumberIgnoredPoints != null)
            {
                signalingMeasTask.InterruptionParameters.NumberIgnoredPoints = NumberIgnoredPoints.Value;
            }
            if (MinExcessNoseLevel_dB != null)
            {
                signalingMeasTask.InterruptionParameters.MinExcessNoseLevel_dB = MinExcessNoseLevel_dB.Value;
            }
            if (MaxFreqDeviation != null)
            {
                signalingMeasTask.InterruptionParameters.MaxFreqDeviation = MaxFreqDeviation.Value;
            }
            if (CheckLevelChannel != null)
            {
                signalingMeasTask.InterruptionParameters.CheckLevelChannel = CheckLevelChannel.Value;
            }
            if (MinPointForDetailBW != null)
            {
                signalingMeasTask.InterruptionParameters.MinPointForDetailBW = MinPointForDetailBW.Value;
            }


            if (TimeBetweenWorkTimes_sec != null)
            {
                signalingMeasTask.GroupingParameters.TimeBetweenWorkTimes_sec = TimeBetweenWorkTimes_sec.Value;
            }
            if (TypeJoinSpectrum != null)
            {
                signalingMeasTask.GroupingParameters.TypeJoinSpectrum = TypeJoinSpectrum.Value;
            }
            if (CrossingBWPercentageForGoodSignals != null)
            {
                signalingMeasTask.GroupingParameters.CrossingBWPercentageForGoodSignals = CrossingBWPercentageForGoodSignals.Value;
            }
            if (CrossingBWPercentageForBadSignals != null)
            {
                signalingMeasTask.GroupingParameters.CrossingBWPercentageForBadSignals = CrossingBWPercentageForBadSignals.Value;
            }
            if (SignalizationNChenal != null)
            {
                signalingMeasTask.SignalizationNChenal = SignalizationNChenal.Value;
            }
            if (SignalizationNCount != null)
            {
                signalingMeasTask.SignalizationNCount = SignalizationNCount.Value;
            }


            if (signalingMeasTask.CorrelationFactor != null)
            {
                if (signalingMeasTask.CorrelationFactor.Value >= 0.99)
                {
                    signalingMeasTask.CorrelationAdaptation = true;
                }
                else
                {
                    signalingMeasTask.CorrelationAdaptation = false;
                }
            }

            signalingMeasTask.MaxNumberEmitingOnFreq = configMessaging.MaxNumberEmitingOnFreq;
            signalingMeasTask.MinCoeffCorrelation = configMessaging.MinCoeffCorrelation;
            signalingMeasTask.UkraineNationalMonitoring = configMessaging.UkraineNationalMonitoring;
            return signalingMeasTask;
        }

        public static Proc.TaskParameters Convert(this MeasTask taskSDR, ConfigMessaging configMessaging)
        {
            const bool Smooth = true; // из клиента для BW
            double PercentForCalcNoise = configMessaging.PercentForCalcNoise;
            // сигнализация

            const int SO_Ncount = 10000;
            const int Signalization_Ncount = 1000000;
            const int OtherNCount = 1000;
            const int SO_NChenal = 10;


            var taskParameters = new Proc.TaskParameters();
            if (taskSDR.Measurement == DataModels.Sdrns.MeasurementType.Signaling)
            {
                var signalizationParams = taskSDR.GetSignalizationParameters(configMessaging);
                taskParameters.SignalingMeasTaskParameters = signalizationParams;
                taskParameters.NChenal = taskParameters.SignalingMeasTaskParameters.SignalizationNChenal.Value;
                taskParameters.NCount = taskParameters.SignalingMeasTaskParameters.SignalizationNCount.Value;
            }
            taskParameters.Smooth = Smooth;

            //
            //taskParameters.NCount = необходимо вичислить
            if (taskSDR.DeviceParam != null)
            {
                taskParameters.Preamplification_dB = taskSDR.DeviceParam.Preamplification_dB;
                taskParameters.RefLevel_dBm = taskSDR.DeviceParam.RefLevel_dBm;
                taskParameters.RfAttenuation_dB = taskSDR.DeviceParam.RfAttenuation_dB;
                if (taskSDR.DeviceParam.RBW_kHz <= 0)
                {
                    taskParameters.RBW_Hz = -1;
                }
                else
                {
                    taskParameters.RBW_Hz = taskSDR.DeviceParam.RBW_kHz * 1000;
                }
                if (taskSDR.DeviceParam.VBW_kHz <= 0)
                {
                    taskParameters.VBW_Hz = -1;
                }
                else
                {
                    taskParameters.VBW_Hz = taskSDR.DeviceParam.VBW_kHz * 1000;
                }

                if (taskSDR.Measurement == DataModels.Sdrns.MeasurementType.SpectrumOccupation)
                {
                    if (taskSDR.DeviceParam.NumberTotalScan != null)
                    {
                        taskParameters.NCount = taskSDR.DeviceParam.NumberTotalScan.Value;
                    }
                }
                taskParameters.DetectType = taskSDR.DeviceParam.DetectType;
            }

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

            if (taskSDR.DeviceParam.MeasTime_sec != null) { taskParameters.SweepTime_s = taskSDR.DeviceParam.MeasTime_sec.Value; } else { taskParameters.SweepTime_s = 0.0001; }
            if (taskSDR.SOParam != null)
            {
                switch (taskSDR.Measurement)
                {
                    case DataModels.Sdrns.MeasurementType.SpectrumOccupation:
                        if ((taskParameters.NCount <= 0) || (taskParameters.NCount > SO_Ncount))
                        {
                            taskParameters.NCount = SO_Ncount;
                        }
                        break;
                    case DataModels.Sdrns.MeasurementType.Signaling:
                        taskParameters.NCount = taskParameters.SignalingMeasTaskParameters.SignalizationNCount.Value;
                        if ((taskParameters.NCount <= 0) || (taskParameters.NCount > Signalization_Ncount))
                        {
                            taskParameters.NCount = Signalization_Ncount;
                        }
                        break;
                    default:
                        taskParameters.NCount = OtherNCount;
                        break;
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
                var listChCentrFreqs_Mhz = new List<double>();
                if (taskSDR.Frequencies != null)
                {
                    if (taskSDR.Frequencies.Values_MHz != null)
                    {
                        for (int i = 0; i < taskSDR.Frequencies.Values_MHz.Length; i++)
                        {
                            var value_MHz = taskSDR.Frequencies.Values_MHz[i];
                            if (!listChCentrFreqs_Mhz.Contains(value_MHz))
                            {
                                listChCentrFreqs_Mhz.Add(value_MHz);
                            }
                        }
                    }
                    listChCentrFreqs_Mhz.Sort();
                    taskParameters.ChCentrFreqs_Mhz = listChCentrFreqs_Mhz;
                }
                if (taskSDR.Frequencies.Step_kHz != null)
                {
                    taskParameters.BWChalnel_kHz = taskSDR.Frequencies.Step_kHz.Value;
                }

                taskParameters.PercentForCalcNoise = PercentForCalcNoise;

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

            taskParameters.TypeTechnology = Proc.TypeTechnology.Unknown;
            // до конца не определенные блоки
            taskParameters.ReceivedIQStreemDuration_sec = 1.0;
            taskParameters.SensorId = taskSDR.SensorId;
            return taskParameters;
        }

        private static Proc.SOType GetSOTypeFromSpectrumOccupationType(DataModels.Sdrns.SpectrumOccupationType spectrumOccupationType)
        {
            Proc.SOType sOType;
            switch (spectrumOccupationType)
            {
                case DataModels.Sdrns.SpectrumOccupationType.FreqBandOccupancy:
                    sOType = Proc.SOType.FreqBandwidthOccupation;
                    break;
                case DataModels.Sdrns.SpectrumOccupationType.FreqChannelOccupancy:
                    sOType = Proc.SOType.FreqChannelOccupation;
                    break;
                default:
                    sOType = Proc.SOType.FreqChannelOccupation;
                    break;
            }
            return (sOType);
        }

        private static Proc.MeasType GetMeasTypeFromMeasurementType(DataModels.Sdrns.MeasurementType measurementType)
        {
            Proc.MeasType measType;
            switch (measurementType)
            {
                case DataModels.Sdrns.MeasurementType.BandwidthMeas:
                    measType = Proc.MeasType.BandwidthMeas;
                    break;
                case DataModels.Sdrns.MeasurementType.Level:
                    measType = Proc.MeasType.Level;
                    break;
                case DataModels.Sdrns.MeasurementType.SpectrumOccupation:
                    measType = Proc.MeasType.SpectrumOccupation;
                    break;
                case DataModels.Sdrns.MeasurementType.MonitoringStations:
                    measType = Proc.MeasType.MonitoringStations;
                    break;
                case DataModels.Sdrns.MeasurementType.Signaling:
                    measType = Proc.MeasType.Signaling;
                    break;
                default:
                    measType = Proc.MeasType.Level;
                    break;
            }
            return (measType);
        }

    }
}
