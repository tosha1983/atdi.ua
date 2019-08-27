using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Atdi.Platform.AppComponent;
using Atdi.Platform.Logging;
using WcfContract = Atdi.Contracts.WcfServices.Sdrn.Server;
using SdrnsDataModels = Atdi.DataModels.Sdrns.Server;

namespace Atdi.WcfServices.Sdrn.Server
{
    public static class MapperForMeasTask
    {
        public static SdrnsDataModels.MeasFreqParam GetMeasFreqParam(this WcfContract.MeasTask task)
        {
            var measFreqParam = new SdrnsDataModels.MeasFreqParam();
            if (task.MeasFreqParam != null)
            {
                var freqs = task.MeasFreqParam.MeasFreqs;
                if ((freqs != null) && (freqs.Length > 0))
                {
                    measFreqParam.MeasFreqs = new SdrnsDataModels.MeasFreq[freqs.Length];
                    var measTaskFreqs = measFreqParam.MeasFreqs;
                    for (int i = 0; i < freqs.Length; i++)
                    {
                        measTaskFreqs[i] = new SdrnsDataModels.MeasFreq();
                        measTaskFreqs[i].Freq = freqs[i].Freq;
                    }
                    measFreqParam.MeasFreqs = measTaskFreqs;
                }
                switch (task.MeasFreqParam.Mode)
                {
                    case WcfContract.FrequencyMode.FrequencyList:
                        measFreqParam.Mode = SdrnsDataModels.FrequencyMode.FrequencyList;
                        break;
                    case WcfContract.FrequencyMode.FrequencyRange:
                        measFreqParam.Mode = SdrnsDataModels.FrequencyMode.FrequencyRange;
                        break;
                    case WcfContract.FrequencyMode.SingleFrequency:
                        measFreqParam.Mode = SdrnsDataModels.FrequencyMode.SingleFrequency;
                        break;
                }
                measFreqParam.RgL = task.MeasFreqParam.RgL;
                measFreqParam.RgU = task.MeasFreqParam.RgU;
                measFreqParam.Step = task.MeasFreqParam.Step;
            }
            return measFreqParam;
        }
        public static SdrnsDataModels.MeasDtParam GetMeasDtParam(this WcfContract.MeasTask task)
        {
            var measDtParam = new SdrnsDataModels.MeasDtParam();
            if (task.MeasDtParam != null)
            {
                measDtParam = new SdrnsDataModels.MeasDtParam();
                measDtParam.Demod = task.MeasDtParam.Demod;
                switch (task.MeasDtParam.DetectType)
                {
                    case WcfContract.DetectingType.Auto:
                        measDtParam.DetectType = SdrnsDataModels.DetectingType.Auto;
                        break;
                    case WcfContract.DetectingType.Average:
                        measDtParam.DetectType = SdrnsDataModels.DetectingType.Average;
                        break;
                    case WcfContract.DetectingType.MaxPeak:
                        measDtParam.DetectType = SdrnsDataModels.DetectingType.MaxPeak;
                        break;
                    case WcfContract.DetectingType.MinPeak:
                        measDtParam.DetectType = SdrnsDataModels.DetectingType.MinPeak;
                        break;
                    case WcfContract.DetectingType.Peak:
                        measDtParam.DetectType = SdrnsDataModels.DetectingType.Peak;
                        break;
                    case WcfContract.DetectingType.RMS:
                        measDtParam.DetectType = SdrnsDataModels.DetectingType.RMS;
                        break;
                }
                measDtParam.IfAttenuation = task.MeasDtParam.IfAttenuation;
                measDtParam.MeasTime = task.MeasDtParam.MeasTime;
                switch (task.MeasDtParam.Mode)
                {
                    case WcfContract.MeasurementMode.Cont:
                        measDtParam.Mode = SdrnsDataModels.MeasurementMode.Cont;
                        break;
                    case WcfContract.MeasurementMode.Gate:
                        measDtParam.Mode = SdrnsDataModels.MeasurementMode.Gate;
                        break;
                    case WcfContract.MeasurementMode.Normal:
                        measDtParam.Mode = SdrnsDataModels.MeasurementMode.Normal;
                        break;
                }
                measDtParam.Preamplification = task.MeasDtParam.Preamplification;
                measDtParam.RBW = task.MeasDtParam.RBW;
                measDtParam.RfAttenuation = task.MeasDtParam.RfAttenuation;
                measDtParam.VBW = task.MeasDtParam.VBW;
            }
            return measDtParam;
        }

        public static SdrnsDataModels.SpectrumOccupationParameters GetSpectrumOccupationParameters(this WcfContract.MeasTask task)
        {
            var spectrumOccupationParameters = new SdrnsDataModels.SpectrumOccupationParameters();
            if (task.MeasOther != null)
            {
                spectrumOccupationParameters = new SdrnsDataModels.SpectrumOccupationParameters();
                spectrumOccupationParameters.LevelMinOccup = task.MeasOther.LevelMinOccup;
                spectrumOccupationParameters.NChenal = task.MeasOther.NChenal;
                switch (task.MeasOther.TypeSpectrumOccupation)
                {
                    case WcfContract.SpectrumOccupationType.FreqBandwidthOccupation:
                        spectrumOccupationParameters.TypeSpectrumOccupation = SdrnsDataModels.SpectrumOccupationType.FreqBandwidthOccupation;
                        break;
                    case WcfContract.SpectrumOccupationType.FreqChannelOccupation:
                        spectrumOccupationParameters.TypeSpectrumOccupation = SdrnsDataModels.SpectrumOccupationType.FreqChannelOccupation;
                        break;
                }
            }
            return spectrumOccupationParameters;
        }

        public static SdrnsDataModels.MeasTaskSpectrumOccupation ToMapForMeasTaskSpectrumOccupation(this WcfContract.MeasTask task)
        {
            var baseObjectMeasTask = task.ToMapForBaseObject();
            var measTask = new SdrnsDataModels.MeasTaskSpectrumOccupation()
            {
                CreatedBy = baseObjectMeasTask.CreatedBy,
                DateCreated = baseObjectMeasTask.DateCreated,
                ExecutionMode = baseObjectMeasTask.ExecutionMode,
                Id = baseObjectMeasTask.Id,
                MeasSubTasks = baseObjectMeasTask.MeasSubTasks,
                MeasTimeParamList = baseObjectMeasTask.MeasTimeParamList,
                Name = baseObjectMeasTask.Name,
                Prio = baseObjectMeasTask.Prio,
                Sensors = baseObjectMeasTask.Sensors,
                Status = baseObjectMeasTask.Status,
                TypeMeasurements = baseObjectMeasTask.TypeMeasurements
            };
            measTask.MeasDtParam = task.GetMeasDtParam();
            measTask.MeasFreqParam = task.GetMeasFreqParam();
            measTask.SpectrumOccupationParameters = task.GetSpectrumOccupationParameters();
            return measTask;
        }


        public static SdrnsDataModels.MeasTaskBandWidth ToMapForMeasTaskBandWidth(this WcfContract.MeasTask task)
        {
            var baseObjectMeasTask = task.ToMapForBaseObject();
            var measTask = new SdrnsDataModels.MeasTaskBandWidth()
            {
                CreatedBy = baseObjectMeasTask.CreatedBy,
                DateCreated = baseObjectMeasTask.DateCreated,
                ExecutionMode = baseObjectMeasTask.ExecutionMode,
                Id = baseObjectMeasTask.Id,
                MeasSubTasks = baseObjectMeasTask.MeasSubTasks,
                MeasTimeParamList = baseObjectMeasTask.MeasTimeParamList,
                Name = baseObjectMeasTask.Name,
                Prio = baseObjectMeasTask.Prio,
                Sensors = baseObjectMeasTask.Sensors,
                Status = baseObjectMeasTask.Status,
                TypeMeasurements = baseObjectMeasTask.TypeMeasurements
            };
            measTask.MeasDtParam = task.GetMeasDtParam();
            measTask.MeasFreqParam = task.GetMeasFreqParam(); 
            return measTask;
        }

        public static SdrnsDataModels.MeasTaskLevel ToMapForMeasTaskLevel(this WcfContract.MeasTask task)
        {
            var baseObjectMeasTask = task.ToMapForBaseObject();
            var measTask = new SdrnsDataModels.MeasTaskLevel()
            {
                CreatedBy = baseObjectMeasTask.CreatedBy,
                DateCreated = baseObjectMeasTask.DateCreated,
                ExecutionMode = baseObjectMeasTask.ExecutionMode,
                Id = baseObjectMeasTask.Id,
                MeasSubTasks = baseObjectMeasTask.MeasSubTasks,
                MeasTimeParamList = baseObjectMeasTask.MeasTimeParamList,
                Name = baseObjectMeasTask.Name,
                Prio = baseObjectMeasTask.Prio,
                Sensors = baseObjectMeasTask.Sensors,
                Status = baseObjectMeasTask.Status,
                TypeMeasurements = baseObjectMeasTask.TypeMeasurements
            };

            measTask.MeasDtParam = task.GetMeasDtParam();
            measTask.MeasFreqParam = task.GetMeasFreqParam();
            return measTask;
        }


        public static SdrnsDataModels.MeasTaskSignaling ToMapForSignalization(this WcfContract.MeasTask task)
        {
            var baseObjectMeasTask = task.ToMapForBaseObject();
            var measTask = new SdrnsDataModels.MeasTaskSignaling()
            {
                CreatedBy = baseObjectMeasTask.CreatedBy,
                DateCreated = baseObjectMeasTask.DateCreated,
                ExecutionMode = baseObjectMeasTask.ExecutionMode,
                Id = baseObjectMeasTask.Id,
                MeasSubTasks = baseObjectMeasTask.MeasSubTasks,
                MeasTimeParamList = baseObjectMeasTask.MeasTimeParamList,
                Name = baseObjectMeasTask.Name,
                Prio = baseObjectMeasTask.Prio,
                Sensors = baseObjectMeasTask.Sensors,
                Status = baseObjectMeasTask.Status,
                TypeMeasurements = baseObjectMeasTask.TypeMeasurements
            };
            if ((task.RefSituation != null) && (task.RefSituation.Length > 0))
            {
                measTask.RefSituation = new SdrnsDataModels.ReferenceSituation[task.RefSituation.Length];
                for (int i = 0; i < task.RefSituation.Length; i++)
                {
                    measTask.RefSituation[i] = new SdrnsDataModels.ReferenceSituation();
                    measTask.RefSituation[i].SensorId = task.RefSituation[i].SensorId;
                    if ((task.RefSituation[i].ReferenceSignal != null) && (task.RefSituation[i].ReferenceSignal.Length > 0))
                    {
                        measTask.RefSituation[i].ReferenceSignal = new SdrnsDataModels.ReferenceSignal[task.RefSituation[i].ReferenceSignal.Length];
                        var measTaskReferenceSignal = measTask.RefSituation[i].ReferenceSignal;
                        var taskReferenceSignal = task.RefSituation[i].ReferenceSignal;
                        for (int j = 0; j < taskReferenceSignal.Length; j++)
                        {
                            measTaskReferenceSignal[j] = new SdrnsDataModels.ReferenceSignal();
                            measTaskReferenceSignal[j].Bandwidth_kHz = taskReferenceSignal[j].Bandwidth_kHz;
                            measTaskReferenceSignal[j].Frequency_MHz = taskReferenceSignal[j].Frequency_MHz;
                            measTaskReferenceSignal[j].IcsmId = taskReferenceSignal[j].IcsmId;
                            measTaskReferenceSignal[j].IcsmTable = taskReferenceSignal[j].IcsmTable;
                            measTaskReferenceSignal[j].LevelSignal_dBm = taskReferenceSignal[j].LevelSignal_dBm;
                            if (measTaskReferenceSignal[j].SignalMask != null)
                            {
                                measTaskReferenceSignal[j].SignalMask = new SdrnsDataModels.SignalMask();
                                measTaskReferenceSignal[j].SignalMask.Freq_kHz = taskReferenceSignal[j].SignalMask.Freq_kHz;
                                measTaskReferenceSignal[j].SignalMask.Loss_dB = taskReferenceSignal[j].SignalMask.Loss_dB;
                            }
                        }
                        measTask.RefSituation[i].ReferenceSignal = measTaskReferenceSignal;
                    }
                }
            }

            measTask.SignalingMeasTaskParameters = new SdrnsDataModels.SignalingMeasTaskParameters();
            if (task.SignalingMeasTaskParameters != null)
            {
                measTask.SignalingMeasTaskParameters.allowableExcess_dB = task.SignalingMeasTaskParameters.allowableExcess_dB;
                measTask.SignalingMeasTaskParameters.AnalyzeByChannel = task.SignalingMeasTaskParameters.AnalyzeByChannel;
                measTask.SignalingMeasTaskParameters.AnalyzeSysInfoEmission = task.SignalingMeasTaskParameters.AnalyzeSysInfoEmission;
                measTask.SignalingMeasTaskParameters.CheckFreqChannel = task.SignalingMeasTaskParameters.CheckFreqChannel;
                measTask.SignalingMeasTaskParameters.CompareTraceJustWithRefLevels = task.SignalingMeasTaskParameters.CompareTraceJustWithRefLevels;
                measTask.SignalingMeasTaskParameters.CorrelationAnalize = task.SignalingMeasTaskParameters.CorrelationAnalize;
                measTask.SignalingMeasTaskParameters.CorrelationFactor = task.SignalingMeasTaskParameters.CorrelationFactor;
                measTask.SignalingMeasTaskParameters.DetailedMeasurementsBWEmission = task.SignalingMeasTaskParameters.DetailedMeasurementsBWEmission;
                measTask.SignalingMeasTaskParameters.FiltrationTrace = task.SignalingMeasTaskParameters.FiltrationTrace;

                var tskGroupingParameters = task.SignalingMeasTaskParameters.GroupingParameters;
                var measTaskGroupingParameters = measTask.SignalingMeasTaskParameters.GroupingParameters;

                if (tskGroupingParameters != null)
                {
                    measTaskGroupingParameters = new SdrnsDataModels.SignalingGroupingParameters();
                    measTaskGroupingParameters.CrossingBWPercentageForBadSignals = tskGroupingParameters.CrossingBWPercentageForBadSignals;
                    measTaskGroupingParameters.CrossingBWPercentageForGoodSignals = tskGroupingParameters.CrossingBWPercentageForGoodSignals;
                    measTaskGroupingParameters.TimeBetweenWorkTimes_sec = tskGroupingParameters.TimeBetweenWorkTimes_sec;
                    measTaskGroupingParameters.TypeJoinSpectrum = tskGroupingParameters.TypeJoinSpectrum;
                    measTask.SignalingMeasTaskParameters.GroupingParameters = measTaskGroupingParameters;
                }

                var tskInterruption = task.SignalingMeasTaskParameters.InterruptionParameters;
                var measTaskInterruption = measTask.SignalingMeasTaskParameters.InterruptionParameters;
                if (tskInterruption != null)
                {
                    measTaskInterruption = new SdrnsDataModels.SignalingInterruptionParameters();
                    measTaskInterruption.allowableExcess_dB = tskInterruption.allowableExcess_dB;
                    measTaskInterruption.AutoDivisionEmitting = tskInterruption.AutoDivisionEmitting;
                    measTaskInterruption.DifferenceMaxMax = tskInterruption.DifferenceMaxMax;
                    measTaskInterruption.DiffLevelForCalcBW = tskInterruption.DiffLevelForCalcBW;
                    measTaskInterruption.MinExcessNoseLevel_dB = tskInterruption.MinExcessNoseLevel_dB;
                    measTaskInterruption.nDbLevel_dB = tskInterruption.nDbLevel_dB;
                    measTaskInterruption.NumberIgnoredPoints = tskInterruption.NumberIgnoredPoints;
                    measTaskInterruption.NumberPointForChangeExcess = tskInterruption.NumberPointForChangeExcess;
                    measTaskInterruption.windowBW = tskInterruption.windowBW;
                    measTask.SignalingMeasTaskParameters.InterruptionParameters = measTaskInterruption;
                }
                measTask.SignalingMeasTaskParameters.SignalizationNChenal = task.SignalingMeasTaskParameters.SignalizationNChenal;
                measTask.SignalingMeasTaskParameters.SignalizationNCount = task.SignalingMeasTaskParameters.SignalizationNCount;
                measTask.SignalingMeasTaskParameters.Standard = task.SignalingMeasTaskParameters.Standard;
                measTask.SignalingMeasTaskParameters.triggerLevel_dBm_Hz = task.SignalingMeasTaskParameters.triggerLevel_dBm_Hz;
            }
            return measTask;
        }

            /// <summary>
            /// Map for MonitoringStations
            /// </summary>
            /// <param name="task"></param>
            /// <returns></returns>
        public static SdrnsDataModels.MeasTaskMonitoringStations ToMapForMonitoringStations(this WcfContract.MeasTask task)
        {
            var baseObjectMeasTask = task.ToMapForBaseObject();
            var measTask = new SdrnsDataModels.MeasTaskMonitoringStations()
            {
                CreatedBy = baseObjectMeasTask.CreatedBy,
                DateCreated = baseObjectMeasTask.DateCreated,
                ExecutionMode = baseObjectMeasTask.ExecutionMode,
                Id = baseObjectMeasTask.Id,
                MeasSubTasks = baseObjectMeasTask.MeasSubTasks,
                MeasTimeParamList = baseObjectMeasTask.MeasTimeParamList,
                Name = baseObjectMeasTask.Name,
                Prio = baseObjectMeasTask.Prio,
                Sensors = baseObjectMeasTask.Sensors,
                Status = baseObjectMeasTask.Status,
                TypeMeasurements = baseObjectMeasTask.TypeMeasurements
            };
            if ((task.StationsForMeasurements != null) && (task.StationsForMeasurements.Length > 0))
            {
                measTask.StationsForMeasurements = new SdrnsDataModels.StationDataForMeasurements[task.StationsForMeasurements.Length];
                for (int i = 0; i < task.StationsForMeasurements.Length; i++)
                {
                    measTask.StationsForMeasurements[i] = new SdrnsDataModels.StationDataForMeasurements();
                    var measTaskStations = measTask.StationsForMeasurements[i];
                    var taskStations = task.StationsForMeasurements[i];
                    measTaskStations.GlobalSID = taskStations.GlobalSID;
                    measTaskStations.IdOwner = taskStations.IdOwner;
                    measTaskStations.IdSite = taskStations.IdSite;
                    measTaskStations.IdStation = taskStations.IdStation;
                    if (taskStations.LicenseParameter != null)
                    {
                        measTaskStations.LicenseParameter = new SdrnsDataModels.PermissionForAssignment();
                        measTaskStations.LicenseParameter.CloseDate = taskStations.LicenseParameter.CloseDate;
                        measTaskStations.LicenseParameter.DozvilName = taskStations.LicenseParameter.DozvilName;
                        measTaskStations.LicenseParameter.EndDate = taskStations.LicenseParameter.EndDate;
                        measTaskStations.LicenseParameter.Id = taskStations.LicenseParameter.Id;
                        measTaskStations.LicenseParameter.StartDate = taskStations.LicenseParameter.StartDate;
                    }
                    if (taskStations.Owner != null)
                    {
                        if (taskStations.Owner != null)
                        {
                            measTaskStations.Owner = new SdrnsDataModels.OwnerData();
                            measTaskStations.Owner.Addres = taskStations.Owner.Addres;
                            measTaskStations.Owner.Code = taskStations.Owner.Code;
                            measTaskStations.Owner.Id = taskStations.Owner.Id;
                            measTaskStations.Owner.OKPO = taskStations.Owner.OKPO;
                            measTaskStations.Owner.OwnerName = taskStations.Owner.OwnerName;
                            measTaskStations.Owner.Zip = taskStations.Owner.Zip;
                        }
                    }
                    if ((taskStations.Sectors != null) && (taskStations.Sectors.Length > 0))
                    {
                        measTaskStations.Sectors = new SdrnsDataModels.SectorStationForMeas[taskStations.Sectors.Length];
                        for (int j = 0; j < taskStations.Sectors.Length; j++)
                        {
                            if (taskStations.Sectors[j] != null)
                            {
                                measTaskStations.Sectors[j] = new SdrnsDataModels.SectorStationForMeas();
                                measTaskStations.Sectors[j].AGL = taskStations.Sectors[j].AGL;
                                measTaskStations.Sectors[j].Azimut = taskStations.Sectors[j].Azimut;
                                measTaskStations.Sectors[j].BW = taskStations.Sectors[j].BW;
                                measTaskStations.Sectors[j].ClassEmission = taskStations.Sectors[j].ClassEmission;
                                measTaskStations.Sectors[j].EIRP = taskStations.Sectors[j].EIRP;
                                if ((taskStations.Sectors[j].Frequencies != null) && (taskStations.Sectors[j].Frequencies.Length > 0))
                                {
                                    measTaskStations.Sectors[j].Frequencies = new SdrnsDataModels.FrequencyForSectorFormICSM[taskStations.Sectors[j].Frequencies.Length];
                                    for (int k = 0; k < taskStations.Sectors[j].Frequencies.Length; k++)
                                    {
                                        if (taskStations.Sectors[j].Frequencies[k] != null)
                                        {
                                            measTaskStations.Sectors[j].Frequencies[k] = new SdrnsDataModels.FrequencyForSectorFormICSM();
                                            measTaskStations.Sectors[j].Frequencies[k].ChannalNumber = taskStations.Sectors[j].Frequencies[k].ChannalNumber;
                                            measTaskStations.Sectors[j].Frequencies[k].Frequency = taskStations.Sectors[j].Frequencies[k].Frequency;
                                            measTaskStations.Sectors[j].Frequencies[k].Id = taskStations.Sectors[j].Frequencies[k].Id;
                                            measTaskStations.Sectors[j].Frequencies[k].IdPlan = taskStations.Sectors[j].Frequencies[k].IdPlan;
                                        }
                                    }
                                }
                                measTaskStations.Sectors[j].IdSector = taskStations.Sectors[j].IdSector;

                                if ((measTaskStations.Sectors[j].MaskBW != null) && (taskStations.Sectors[j].MaskBW.Length > 0))
                                {
                                    measTaskStations.Sectors[j].MaskBW = new SdrnsDataModels.MaskElements[taskStations.Sectors[j].MaskBW.Length];
                                    for (int k = 0; k < taskStations.Sectors[j].MaskBW.Length; k++)
                                    {
                                        if (taskStations.Sectors[j].MaskBW[k] != null)
                                        {
                                            measTaskStations.Sectors[j].MaskBW[k] = new SdrnsDataModels.MaskElements();
                                            measTaskStations.Sectors[j].MaskBW[k].BW = taskStations.Sectors[j].MaskBW[k].BW;
                                            measTaskStations.Sectors[j].MaskBW[k].level = taskStations.Sectors[j].MaskBW[k].level;
                                        }
                                    }
                                }
                            }
                        }
                    }
                    if (taskStations.Site != null)
                    {
                        measTaskStations.Site = new SdrnsDataModels.SiteStationForMeas();
                        measTaskStations.Site.Adress = taskStations.Site.Adress;
                        measTaskStations.Site.Id = taskStations.Site.Id;
                        measTaskStations.Site.Lat = taskStations.Site.Lat;
                        measTaskStations.Site.Lon = taskStations.Site.Lon;
                        measTaskStations.Site.Region = taskStations.Site.Region;
                    }

                    measTaskStations.Standart = taskStations.Standart;
                    measTaskStations.Status = taskStations.Status;
                    measTask.StationsForMeasurements[i] = measTaskStations;
                }
            }
            return measTask;
        }


        public static SdrnsDataModels.MeasTask ToMapForBaseObject(this WcfContract.MeasTask task)
        {
            var measTask = new DataModels.Sdrns.Server.MeasTask();
            switch (task.MeasDtParam.TypeMeasurements)
            {
                case WcfContract.MeasurementType.AmplModulation:
                    measTask.TypeMeasurements = SdrnsDataModels.MeasurementType.AmplModulation;
                    break;
                case WcfContract.MeasurementType.BandwidthMeas:
                    measTask.TypeMeasurements = SdrnsDataModels.MeasurementType.BandwidthMeas;
                    break;
                case WcfContract.MeasurementType.Bearing:
                    measTask.TypeMeasurements = SdrnsDataModels.MeasurementType.Bearing;
                    break;
                case WcfContract.MeasurementType.FreqModulation:
                    measTask.TypeMeasurements = SdrnsDataModels.MeasurementType.FreqModulation;
                    break;
                case WcfContract.MeasurementType.Frequency:
                    measTask.TypeMeasurements = SdrnsDataModels.MeasurementType.Frequency;
                    break;
                case WcfContract.MeasurementType.Level:
                    measTask.TypeMeasurements = SdrnsDataModels.MeasurementType.Level;
                    break;
                case WcfContract.MeasurementType.Location:
                    measTask.TypeMeasurements = SdrnsDataModels.MeasurementType.Location;
                    break;
                case WcfContract.MeasurementType.MonitoringStations:
                    measTask.TypeMeasurements = SdrnsDataModels.MeasurementType.MonitoringStations;
                    break;
                case WcfContract.MeasurementType.Offset:
                    measTask.TypeMeasurements = SdrnsDataModels.MeasurementType.Offset;
                    break;
                case WcfContract.MeasurementType.PICode:
                    measTask.TypeMeasurements = SdrnsDataModels.MeasurementType.PICode;
                    break;
                case WcfContract.MeasurementType.Program:
                    measTask.TypeMeasurements = SdrnsDataModels.MeasurementType.Program;
                    break;
                case WcfContract.MeasurementType.Signaling:
                    measTask.TypeMeasurements = SdrnsDataModels.MeasurementType.Signaling;
                    break;
                case WcfContract.MeasurementType.SoundID:
                    measTask.TypeMeasurements = SdrnsDataModels.MeasurementType.SoundID;
                    break;
                case WcfContract.MeasurementType.SpectrumOccupation:
                    measTask.TypeMeasurements = SdrnsDataModels.MeasurementType.SpectrumOccupation;
                    break;
                case WcfContract.MeasurementType.SubAudioTone:
                    measTask.TypeMeasurements = SdrnsDataModels.MeasurementType.SubAudioTone;
                    break;
            }

            measTask.CreatedBy = task.CreatedBy;
            measTask.DateCreated = task.DateCreated;
            switch (task.ExecutionMode)
            {
                case WcfContract.MeasTaskExecutionMode.Automatic:
                    measTask.ExecutionMode = SdrnsDataModels.MeasTaskExecutionMode.Automatic;
                    break;
                case WcfContract.MeasTaskExecutionMode.Manual:
                    measTask.ExecutionMode = SdrnsDataModels.MeasTaskExecutionMode.Manual;
                    break;
            }
            measTask.Id = new SdrnsDataModels.MeasTaskIdentifier();
            if (task.Id != null)
            {
                measTask.Id.Value = task.Id.Value;
            }
            if ((task.MeasSubTasks != null) && (task.MeasSubTasks.Length > 0))
            {
                measTask.MeasSubTasks = new SdrnsDataModels.MeasSubTask[task.MeasSubTasks.Length];
                for (int i = 0; i < task.MeasSubTasks.Length; i++)
                {
                    measTask.MeasSubTasks[i] = new SdrnsDataModels.MeasSubTask();
                    measTask.MeasSubTasks[i].Id = new SdrnsDataModels.MeasTaskIdentifier();
                    if (task.MeasSubTasks[i].Id != null)
                    {
                        measTask.MeasSubTasks[i].Id.Value = task.MeasSubTasks[i].Id.Value;
                    }
                    measTask.MeasSubTasks[i].Interval = task.MeasSubTasks[i].Interval;
                    measTask.MeasSubTasks[i].Status = task.MeasSubTasks[i].Status;
                    measTask.MeasSubTasks[i].TimeStart = task.MeasSubTasks[i].TimeStart;
                    measTask.MeasSubTasks[i].TimeStop = task.MeasSubTasks[i].TimeStop;

                    var taskMeasSubTaskSensors = task.MeasSubTasks[i].MeasSubTaskSensors;
                    var measTaskMeasSubTaskSensors = measTask.MeasSubTasks[i].MeasSubTaskSensors;

                    if ((taskMeasSubTaskSensors != null) && (taskMeasSubTaskSensors.Length > 0))
                    {
                        measTaskMeasSubTaskSensors = new SdrnsDataModels.MeasSubTaskSensor[taskMeasSubTaskSensors.Length];
                        for (int j = 0; j < taskMeasSubTaskSensors.Length; j++)
                        {
                            measTaskMeasSubTaskSensors[j] = new SdrnsDataModels.MeasSubTaskSensor();
                            measTaskMeasSubTaskSensors[j].Count = taskMeasSubTaskSensors[j].Count;
                            measTaskMeasSubTaskSensors[j].Id = taskMeasSubTaskSensors[j].Id;
                            if (taskMeasSubTaskSensors[j].SensorId != null)
                            {
                                measTaskMeasSubTaskSensors[j].SensorId = taskMeasSubTaskSensors[j].SensorId.Value;
                            }
                            measTaskMeasSubTaskSensors[j].Status = taskMeasSubTaskSensors[j].Status;
                            measTaskMeasSubTaskSensors[j].TimeNextTask = taskMeasSubTaskSensors[j].TimeNextTask;
                        }
                    }
                    measTask.MeasSubTasks[i].MeasSubTaskSensors = measTaskMeasSubTaskSensors;
                }
            }

            if (task.MeasTimeParamList != null)
            {
                measTask.MeasTimeParamList = new SdrnsDataModels.MeasTimeParamList();
                measTask.MeasTimeParamList.Days = task.MeasTimeParamList.Days;
                measTask.MeasTimeParamList.PerInterval = task.MeasTimeParamList.PerInterval;
                measTask.MeasTimeParamList.PerStart = task.MeasTimeParamList.PerStart;
                measTask.MeasTimeParamList.PerStop = task.MeasTimeParamList.PerStop;
                measTask.MeasTimeParamList.TimeStart = task.MeasTimeParamList.TimeStart;
                measTask.MeasTimeParamList.TimeStop = task.MeasTimeParamList.TimeStop;
            }
            measTask.Name = task.Name;
            measTask.Prio = task.Prio;

            if ((task.Sensors != null) && (task.Sensors.Length > 0))
            {
                measTask.Sensors = new SdrnsDataModels.MeasSensor[task.Sensors.Length];
                for (int i = 0; i < task.Sensors.Length; i++)
                {
                    measTask.Sensors[i] = new SdrnsDataModels.MeasSensor();
                    measTask.Sensors[i].SensorId = new SdrnsDataModels.MeasSensorIdentifier();
                    if (task.Sensors[i].SensorId != null)
                    {
                        measTask.Sensors[i].SensorId.Value = task.Sensors[i].SensorId.Value;
                    }
                }
            }

            measTask.Status = task.Status;
            return measTask;
        }

        public static DataModels.Sdrns.Server.MeasTask ToMap(this WcfContract.MeasTask task)
        {
            var measTask = new DataModels.Sdrns.Server.MeasTask();
            if (task != null)
            {
                switch (task.MeasDtParam.TypeMeasurements)
                {
                    case WcfContract.MeasurementType.AmplModulation:
                    case WcfContract.MeasurementType.Bearing:
                    case WcfContract.MeasurementType.FreqModulation:
                    case WcfContract.MeasurementType.Frequency:
                    case WcfContract.MeasurementType.Location:
                    case WcfContract.MeasurementType.Offset:
                    case WcfContract.MeasurementType.PICode:
                    case WcfContract.MeasurementType.Program:
                    case WcfContract.MeasurementType.SoundID:
                    case WcfContract.MeasurementType.SubAudioTone:
                        throw new NotImplementedException($"Not supported type {task.MeasDtParam.TypeMeasurements}");
                    case WcfContract.MeasurementType.BandwidthMeas:
                        return task.ToMapForMeasTaskBandWidth();
                    case WcfContract.MeasurementType.Level:
                        return task.ToMapForMeasTaskLevel();
                    case WcfContract.MeasurementType.MonitoringStations:
                        return task.ToMapForMonitoringStations();
                    case WcfContract.MeasurementType.Signaling:
                        return task.ToMapForSignalization();
                    case WcfContract.MeasurementType.SpectrumOccupation:
                        return task.ToMapForMeasTaskSpectrumOccupation();
                }
            }
            return measTask;
        }
     
    }
}
