using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SDR = Atdi.Contracts.WcfServices.Sdrn.Server;
using SDRI = Atdi.Contracts.WcfServices.Sdrn.Server.IeStation;
using VM = XICSM.ICSControlClient.Models.Views;
using M = XICSM.ICSControlClient.Models;
using XICSM.ICSControlClient.Environment.Wpf;
using SVC = XICSM.ICSControlClient.WcfServiceClients;
using XICSM.ICSControlClient.ViewModels.Reports;
using XICSM.ICSControlClient.ViewModels.Coordinates;

namespace XICSM.ICSControlClient.Models.WcfDataApadters
{
    public static class Mappers
    {
        public static VM.SensorViewModel Map(SDR.Sensor source)
        {
            if (source == null)
            {
                return null;
            }

            return new VM.SensorViewModel
            {
                Administration = source.Administration,
                AGL = source.AGL.ToNull(),
                Antenna = source.Antenna,
                Azimuth = source.Azimuth.ToNull(),
                BiuseDate = source.BiuseDate.ToNull(),
                CreatedBy = source.CreatedBy,
                CustData1 = source.CustData1.ToNull(),
                CustNbr1 = source.CustNbr1.ToNull(),
                CustTxt1 = source.CustTxt1,
                DateCreated = source.DateCreated.ToNull(),
                Elevation = source.Elevation.ToNull(),
                EouseDate = source.EouseDate.ToNull(),
                Equipment = source.Equipment,
                Id = source.Id.Value,
                IdSysARGUS = source.IdSysARGUS,
                Locations = source.Locations,
                Name = source.Name,
                NetworkId = source.NetworkId,
                OpDays = source.OpDays,
                OpHHFr = source.OpHHFr.ToNull(),
                OpHHTo = source.OpHHTo.ToNull(),
                Remark = source.Remark,
                RxLoss = source.RxLoss.ToNull(),
                Status = source.Status,
                StepMeasTime = source.StepMeasTime.ToNull(),
                TypeSensor = source.TypeSensor
            };
        }
        public static VM.ShortSensorViewModel Map(SDR.ShortSensor source)
        {
            if (source == null)
            {
                return null;
            }

            return new VM.ShortSensorViewModel
            {
                Administration = source.Administration,
                AntGainMax = source.AntGainMax.ToNull(),
                Id = source.Id.Value,
                AntManufacturer = source.AntManufacturer,
                AntName = source.AntName,
                BiuseDate = source.BiuseDate.ToNull(),
                CreatedBy = source.CreatedBy,
                DateCreated = source.DateCreated.ToNull(),
                EouseDate = source.EouseDate.ToNull(),
                EquipCode = source.EquipCode,
                EquipManufacturer = source.EquipManufacturer,
                EquipName = source.EquipName,
                LowerFreq = source.LowerFreq.ToNull(),
                Name = source.Name,
                NetworkId = source.NetworkId,
                RxLoss = source.RxLoss.ToNull(),
                Status = source.Status,
                UpperFreq = source.UpperFreq.ToNull()
            };
        }
        public static VM.SignSysInfoViewModel Map(SDR.SignalingSysInfo source)
        {
            if (source == null)
            {
                return null;
            }

            return new VM.SignSysInfoViewModel
            {
                BandWidth_Hz = source.BandWidth_Hz,
                BSIC = source.BSIC,
                ChannelNumber = source.ChannelNumber,
                CID = source.CID,
                CtoI = source.CtoI,
                Freq_Hz = source.Freq_Hz,
                LAC = source.LAC,
                Level_dBm = source.Level_dBm,
                MCC = source.MCC,
                MNC = source.MNC,
                Power = source.Power,
                RNC = source.RNC,
                Standart = source.Standart,
                WorkTimes = source.WorkTimes
            };
        }
        public static VM.MeasTaskDetailStationViewModel Map(SDR.StationDataForMeasurements source)
        {
            if (source == null)
            {
                return null;
            }

            return new VM.MeasTaskDetailStationViewModel
            {
                GlobalSID = source.GlobalSID,
                LicenseCloseDate = source.LicenseParameter.CloseDate.ToNull(),
                LicenseDozvilName = source.LicenseParameter.DozvilName,
                LicenseEndDate = source.LicenseParameter.EndDate.ToNull(),
                LicenseIcsmId = source.LicenseParameter.Id.ToNull(),
                LicenseStartDate = source.LicenseParameter.StartDate.ToNull(),
                OwnerAddres = source.Owner.Addres,
                OwnerCode = source.Owner.Code,
                OwnerId = source.Owner.Id,
                OwnerName = source.Owner.OwnerName,
                OwnerOKPO = source.Owner.OKPO,
                OwnerZip = source.Owner.Zip,
                Sectors = source.Sectors,
                SiteAdress = source.Site.Adress,
                SiteLat = source.Site.Lat.ToNull(),
                SiteLon = source.Site.Lon.ToNull(),
                SiteRegion = source.Site.Region,
                Standart = source.Standart,
                StationId = source.IdStation,
                Status = source.Status
            };
        }

        public static VM.MeasTaskViewModel Map(SDR.MeasTask source)
        {
            if (source == null)
            {
                return null;
            }

            if (source.MeasDtParam == null)
            {
                source.MeasDtParam = new SDR.MeasDtParam();
            }

            if (source.MeasFreqParam == null)
            {
                source.MeasFreqParam = new SDR.MeasFreqParam();
            }

            if (source.MeasOther == null)
            {
                source.MeasOther = new SDR.MeasOther();
            }

            if (source.MeasTimeParamList == null)
            {
                source.MeasTimeParamList = new SDR.MeasTimeParamList();
            }

            if (source.SignalingMeasTaskParameters == null)
            {
                source.SignalingMeasTaskParameters = new SDR.SignalingMeasTask();
            }

            if (source.SignalingMeasTaskParameters.GroupingParameters == null)
            {
                source.SignalingMeasTaskParameters.GroupingParameters = new SDR.SignalingGroupingParameters();
            }

            if (source.SignalingMeasTaskParameters.InterruptionParameters == null)
            {
                source.SignalingMeasTaskParameters.InterruptionParameters = new SDR.SignalingInterruptionParameters();
            }

            string statusFull = PluginHelper.GetFullTaskStatus(source.Status);
            string measOtherTypeSpectrumOccupationFull = "";
            switch (source.MeasOther.TypeSpectrumOccupation)
            {
                case SDR.SpectrumOccupationType.FreqBandwidthOccupation:
                    measOtherTypeSpectrumOccupationFull = "Frequency band occupancy";
                    break;
                case SDR.SpectrumOccupationType.FreqChannelOccupation:
                    measOtherTypeSpectrumOccupationFull = "Frequency channel occupancy";
                    break;
                default:
                    break;
            }

            string measFreqParamModeFull = "";
            switch (source.MeasFreqParam.Mode)
            {
                case SDR.FrequencyMode.FrequencyList:
                    measFreqParamModeFull = "Frequency List";
                    break;
                case SDR.FrequencyMode.FrequencyRange:
                    measFreqParamModeFull = "Frequency Range";
                    break;
                case SDR.FrequencyMode.SingleFrequency:
                    measFreqParamModeFull = "Single Frequency";
                    break;
                default:
                    break;
            }

            return new VM.MeasTaskViewModel
            {
                CreatedBy = source.CreatedBy,
                DateCreated = source.DateCreated.ToNull(),
                ExecutionMode = source.ExecutionMode,
                Id = source.Id.Value,
                MaxTimeBs = source.MaxTimeBs.ToNull(),

                MeasDtParamDemod = source.MeasDtParam.Demod,
                MeasDtParamDetectType = source.MeasDtParam.DetectType,
                MeasDtParamIfAttenuation = source.MeasDtParam.IfAttenuation ?? 0,
                MeasDtParamMode = source.MeasDtParam.Mode,

                MeasDtParamMeasTime = (source.MeasDtParam.MeasTime.ToNull().HasValue && source.MeasDtParam.MeasTime.Value == 0.001) ? null : source.MeasDtParam.MeasTime.ToNull(),
                IsAutoMeasDtParamMeasTime = (source.MeasDtParam.MeasTime.ToNull().HasValue && source.MeasDtParam.MeasTime.Value == 0.001),
                MeasDtParamMeasTimeView = (source.MeasDtParam.MeasTime.HasValue && source.MeasDtParam.MeasTime.Value == 0.001) ? "Auto" : source.MeasDtParam.MeasTime.ToNull().ToString(),

                MeasDtParamPreamplification = (source.MeasDtParam.Preamplification.HasValue && source.MeasDtParam.Preamplification.Value == -1) ? (int?)null : source.MeasDtParam.Preamplification ?? 0,
                IsAutoMeasDtParamPreamplification = (source.MeasDtParam.Preamplification.HasValue && source.MeasDtParam.Preamplification.Value == -1),
                MeasDtParamPreamplificationView = (source.MeasDtParam.Preamplification.HasValue && source.MeasDtParam.Preamplification.Value == -1) ? "Auto" : source.MeasDtParam.Preamplification.ToNull().ToString(),

                MeasDtParamRBW = (source.MeasDtParam.RBW.HasValue && source.MeasDtParam.RBW.Value == -1) ? null : source.MeasDtParam.RBW.ToNull(),
                IsAutoMeasDtParamRBW = (source.MeasDtParam.RBW.HasValue && source.MeasDtParam.RBW.Value == -1),
                MeasDtParamRBWView = (source.MeasDtParam.RBW.HasValue && source.MeasDtParam.RBW.Value == -1) ? "Auto" : source.MeasDtParam.RBW.ToNull().ToString(),

                MeasDtParamRfAttenuation = (source.MeasDtParam.RfAttenuation.HasValue && source.MeasDtParam.RfAttenuation.Value == -1) ? (double?)null : source.MeasDtParam.RfAttenuation.GetValueOrDefault(),
                IsAutoMeasDtParamRfAttenuation = (source.MeasDtParam.RfAttenuation.HasValue && source.MeasDtParam.RfAttenuation.Value == -1),
                MeasDtParamRfAttenuationView = (source.MeasDtParam.RfAttenuation.HasValue && source.MeasDtParam.RfAttenuation.Value == -1) ? "Auto" : source.MeasDtParam.RfAttenuation.ToNull().ToString(),

                MeasDtParamTypeMeasurements = source.MeasDtParam.TypeMeasurements,

                MeasDtParamVBW = (source.MeasDtParam.VBW.HasValue && source.MeasDtParam.VBW.Value == -1) ? null : source.MeasDtParam.VBW.ToNull(),
                IsAutoMeasDtParamVBW = (source.MeasDtParam.VBW.HasValue && source.MeasDtParam.VBW.Value == -1),
                MeasDtParamVBWView = (source.MeasDtParam.VBW.HasValue && source.MeasDtParam.VBW.Value == -1) ? "Auto" : source.MeasDtParam.VBW.ToNull().ToString(),

                MeasDtParamReferenceLevel = (source.MeasDtParam.ReferenceLevel.HasValue && source.MeasDtParam.ReferenceLevel.Value == 1000000000) ? null : source.MeasDtParam.ReferenceLevel.ToNull(),
                IsAutoMeasDtParamReferenceLevel = (source.MeasDtParam.ReferenceLevel.HasValue && source.MeasDtParam.ReferenceLevel.Value == 1000000000),
                MeasDtParamReferenceLevelView = (source.MeasDtParam.ReferenceLevel.HasValue && source.MeasDtParam.ReferenceLevel.Value == 1000000000) ? "Auto" : source.MeasDtParam.ReferenceLevel.ToNull().ToString(),

                AllowableExcess_dB = source.SignalingMeasTaskParameters.allowableExcess_dB.ToNull(),
                CompareTraceJustWithRefLevels = source.SignalingMeasTaskParameters.CompareTraceJustWithRefLevels,
                FiltrationTrace = source.SignalingMeasTaskParameters.FiltrationTrace,
                SignalizationNChenal = source.SignalingMeasTaskParameters.SignalizationNChenal.ToNull(),
                SignalizationNCount = source.SignalingMeasTaskParameters.SignalizationNCount.ToNull(),
                AnalyzeByChannel = source.SignalingMeasTaskParameters.AnalyzeByChannel,
                AnalyzeSysInfoEmission = source.SignalingMeasTaskParameters.AnalyzeSysInfoEmission,
                CheckFreqChannel = source.SignalingMeasTaskParameters.CheckFreqChannel,
                CorrelationAnalize = source.SignalingMeasTaskParameters.CorrelationAnalize,
                CorrelationFactor = source.SignalingMeasTaskParameters.CorrelationFactor.ToNull(),
                DetailedMeasurementsBWEmission = source.SignalingMeasTaskParameters.DetailedMeasurementsBWEmission,
                Standard = source.SignalingMeasTaskParameters.Standard,
                IsUseRefSpectrum = source.SignalingMeasTaskParameters.IsUseRefSpectrum.GetValueOrDefault(false),

                triggerLevel_dBm_Hz = (source.SignalingMeasTaskParameters.triggerLevel_dBm_Hz.HasValue && source.SignalingMeasTaskParameters.triggerLevel_dBm_Hz.Value == -999) ? null : source.SignalingMeasTaskParameters.triggerLevel_dBm_Hz,
                IsAutoTriggerLevel_dBm_Hz = (source.SignalingMeasTaskParameters.triggerLevel_dBm_Hz.HasValue && source.SignalingMeasTaskParameters.triggerLevel_dBm_Hz.Value == -999),
                triggerLevel_dBm_HzView = (source.SignalingMeasTaskParameters.triggerLevel_dBm_Hz.HasValue && source.SignalingMeasTaskParameters.triggerLevel_dBm_Hz.Value == -999) ? "Auto" : source.SignalingMeasTaskParameters.triggerLevel_dBm_Hz.ToNull().ToString(),

                CollectEmissionInstrumentalEstimation = source.SignalingMeasTaskParameters.CollectEmissionInstrumentalEstimation,

                CrossingBWPercentageForBadSignals = source.SignalingMeasTaskParameters.GroupingParameters.CrossingBWPercentageForBadSignals.ToNull(),
                CrossingBWPercentageForGoodSignals = source.SignalingMeasTaskParameters.GroupingParameters.CrossingBWPercentageForGoodSignals.ToNull(),
                TimeBetweenWorkTimes_sec = source.SignalingMeasTaskParameters.GroupingParameters.TimeBetweenWorkTimes_sec.ToNull(),
                TypeJoinSpectrum = source.SignalingMeasTaskParameters.GroupingParameters.TypeJoinSpectrum.ToNull(),

                AutoDivisionEmitting = source.SignalingMeasTaskParameters.InterruptionParameters.AutoDivisionEmitting,
                DifferenceMaxMax = source.SignalingMeasTaskParameters.InterruptionParameters.DifferenceMaxMax.ToNull(),
                CheckLevelChannel = source.SignalingMeasTaskParameters.InterruptionParameters.CheckLevelChannel,
                DiffLevelForCalcBW = source.SignalingMeasTaskParameters.InterruptionParameters.DiffLevelForCalcBW.ToNull(),
                MaxFreqDeviation = source.SignalingMeasTaskParameters.InterruptionParameters.MaxFreqDeviation.ToNull(),
                MinExcessNoseLevel_dB = source.SignalingMeasTaskParameters.InterruptionParameters.MinExcessNoseLevel_dB.ToNull(),
                MinPointForDetailBW = source.SignalingMeasTaskParameters.InterruptionParameters.MinPointForDetailBW.ToNull(),
                nDbLevel_dB = source.SignalingMeasTaskParameters.InterruptionParameters.nDbLevel_dB.ToNull(),
                NumberIgnoredPoints = source.SignalingMeasTaskParameters.InterruptionParameters.NumberIgnoredPoints.ToNull(),
                NumberPointForChangeExcess = source.SignalingMeasTaskParameters.InterruptionParameters.NumberPointForChangeExcess.ToNull(),
                windowBW = source.SignalingMeasTaskParameters.InterruptionParameters.windowBW.ToNull(),

                MeasFreqParamMeasFreqs = (source.MeasFreqParam.MeasFreqs ?? (new SDR.MeasFreq[] { })).Select(i => i.Freq).ToArray(),
                MeasFreqParamMode = source.MeasFreqParam.Mode,
                MeasFreqParamModeFull = measFreqParamModeFull,
                MeasFreqParamRgL = source.MeasFreqParam.RgL.ToNull(),
                MeasFreqParamRgU = source.MeasFreqParam.RgU.ToNull(),
                MeasFreqParamStep = source.MeasFreqParam.Step.ToNull(),

                MeasOtherLevelMinOccup = source.MeasOther.LevelMinOccup.ToNull(),
                MeasOtherNCount = source.MeasDtParam.NumberTotalScan.ToNull(),
                MeasOtherNChenal = source.MeasOther.NChenal.ToNull(),
                MeasOtherSwNumber = source.MeasOther.SwNumber.ToNull(),
                MeasOtherTypeSpectrumOccupation = source.MeasOther.TypeSpectrumOccupation,
                MeasOtherTypeSpectrumOccupationFull = measOtherTypeSpectrumOccupationFull,
                MeasOtherTypeSpectrumScan = source.MeasOther.TypeSpectrumScan,
                SupportMultyLevel = source.MeasOther.SupportMultyLevel ?? false,

                MeasTimeParamListDays = source.MeasTimeParamList.Days,
                MeasTimeParamListPerInterval = source.MeasTimeParamList.PerInterval.ToNull(),
                MeasTimeParamListPerStart = source.MeasTimeParamList.PerStart,
                MeasTimeParamListPerStop = source.MeasTimeParamList.PerStop,
                MeasTimeParamListTimeStart = source.MeasTimeParamList.TimeStart.ToNull(),
                MeasTimeParamListTimeStop = source.MeasTimeParamList.TimeStop.ToNull(),

                Name = source.Name,
                OrderId = source.OrderId,
                Prio = source.Prio.ToNull(),
                ResultType = source.ResultType,
                Status = source.Status,
                StatusFull = statusFull,
                Task = source.Task,
                Type = source.Type,
                StationsForMeasurements = source.StationsForMeasurements,
                Sensors = source.Sensors
            };
        }

        public static VM.MeasurementResultsViewModel Map(SDR.MeasurementResults source)
        {
            if (source == null)
            {
                return null;
            }

            return new VM.MeasurementResultsViewModel
            {
                AntVal = source.AntVal.ToNull(),
                DataRank = source.DataRank.ToNull(),
                FrequenciesMeasurements = source.FrequenciesMeasurements,
                Status = source.Status,
                MeasSdrResultsId = source.Id.MeasSdrResultsId,
                MeasTaskId = source.Id.MeasTaskId.Value,
                LocationSensorMeasurement = source.LocationSensorMeasurement,
                MeasurementsResults = source.MeasurementsResults,
                N = source.N.ToNull(),
                ResultsMeasStation = source.ResultsMeasStation,
                StationMeasurementsStationId = source.StationMeasurements == null ? 0 : source.StationMeasurements.StationId.Value,
                SubMeasTaskId = source.Id.SubMeasTaskId,
                SubMeasTaskStationId = source.Id.SubMeasTaskStationId,
                TimeMeas = source.TimeMeas,
                TypeMeasurements = source.TypeMeasurements,
                //LowFreq = source.FrequenciesMeasurements == null ? (double?)null : (source.FrequenciesMeasurements.Length == 0 ? 0 : source.FrequenciesMeasurements.Min(f => f.Freq)),
                //UpFreq = source.FrequenciesMeasurements == null ? (double?)null : (source.FrequenciesMeasurements.Length == 0 ? 0 : source.FrequenciesMeasurements.Max(f => f.Freq)),
                LowFreq = source.LowFreq,
                UpFreq = source.UpFreq,
                MeasDeviceId = source.StationMeasurements == null ? (long?)null : source.StationMeasurements.StationId.Value,
                StationsNumber = source.ResultsMeasStation == null ? (int?)null : source.ResultsMeasStation.Length,
                PointsNumber = source.MeasurementsResults == null ? (int?)null : source.MeasurementsResults.Length,
                SensorName = source.SensorName,
                SensorTitle = source.SensorTitle,
                SensorTechId = source.SensorTechId,
                CountStationMeasurements = source.CountStationMeasurements,
                CountUnknownStationMeasurements = source.CountUnknownStationMeasurements,
                StartTime = source.StartTime,
                StopTime = source.StopTime,
                ScansNumber = source.ScansNumber
            };
        }

        public static VM.ResultsMeasurementsStationViewModel Map(SDR.ResultsMeasurementsStation source)
        {
            if (source == null)
            {
                return null;
            }

            return new VM.ResultsMeasurementsStationViewModel
            {
                Id = source.Id,
                GlobalSID = source.GlobalSID,
                LevelMeasurements = source.LevelMeasurements,
                MeasGlobalSID = source.MeasGlobalSID,
                SectorId = source.IdSector.ToNull(),
                StationId = source.Idstation,
                Status = source.Status,
                StationSysInfo = source.StationSysInfo,
                Standard = source.Standard,
                //CentralFrequencyMeas_MHz = (source.GeneralResult != null && source.GeneralResult.Length > 0) ? source.GeneralResult.OrderByDescending(c => c.TimeStartMeas).ToArray()[0].CentralFrequencyMeas : (double?)null,
                CentralFrequencyMHz = source.CentralFrequencyMeas_MHz,
                ////CentralFrequencyMHz = (source.GeneralResult != null && source.GeneralResult.Length > 0) ? source.GeneralResult.OrderByDescending(c => c.TimeStartMeas).ToArray()[0].CentralFrequency : (double?)null,
                //GeneralResults = (source.GeneralResult != null && source.GeneralResult.Length > 0) ? source.GeneralResult.OrderByDescending(c => c.TimeStartMeas).ToArray() : null,
                //CountLevelMeas = source.LevelMeasurements == null ? (int?)null : source.LevelMeasurements.Length,
                //CountSpectrums = (source.GeneralResult != null && source.GeneralResult.Length > 0  && source.GeneralResult[0].LevelsSpecrum != null) ? source.GeneralResult[0].LevelsSpecrum.Length : (int?)null 
            };
        }
        public static VM.GeneralResultViewModel Map(SDR.MeasurementsParameterGeneral source)
        {
            if (source == null)
            {
                return null;
            }

            return new VM.GeneralResultViewModel
            {
                CentralFrequency = source.CentralFrequency,
                CentralFrequencyMeas = source.CentralFrequencyMeas,
                DurationMeas = source.DurationMeas,
                LevelsSpecrum = source.LevelsSpecrum,
                MaskBW = source.MaskBW,
                OffsetFrequency = source.OffsetFrequency,
                SpecrumStartFreq = source.SpecrumStartFreq,
                SpecrumSteps = source.SpecrumSteps,
                TimeFinishMeas = source.TimeFinishMeas,
                TimeStartMeas = source.TimeStartMeas,
                NumberPointsOfSpectrum = source.LevelsSpecrum == null ? (int?)null : source.LevelsSpecrum.Length,
                T1 = Convert.ToDouble((source.SpecrumStartFreq ?? 0) + (source.T1.ToNull() ?? 0) * (source.SpecrumSteps ?? 0) / 1000),
                T2 = Convert.ToDouble((source.SpecrumStartFreq ?? 0) + (source.T2.ToNull() ?? 0) * (source.SpecrumSteps ?? 0) / 1000),
                MarkerIndex = Convert.ToDouble((source.SpecrumStartFreq ?? 0) + (source.MarkerIndex.ToNull() ?? 0) * (source.SpecrumSteps ?? 0) / 1000)
            };
        }
        public static VM.LevelMeasurementsCarViewModel Map(SDR.LevelMeasurementsCar source)
        {
            if (source == null)
            {
                return null;
            }

            return new VM.LevelMeasurementsCarViewModel
            {
                Altitude = source.Altitude.ToNull(),
                BW = source.BW.ToNull(),
                CentralFrequency = source.CentralFrequency,
                DifferenceTimestamp = source.DifferenceTimestamp.ToNull(),
                Lat = source.Lat.ToNull(),
                LeveldBm = source.LeveldBm.ToNull(),
                LeveldBmkVm = source.LeveldBmkVm.ToNull(),
                Lon = source.Lon.ToNull(),
                RBW = source.RBW.ToNull(),
                TimeOfMeasurements = source.TimeOfMeasurements,
                VBW = source.VBW.ToNull()
            };
        }
        public static VM.EmittingViewModel Map(SDR.Emitting source)
        {
            if (source == null)
            {
                return null;
            }

            return new VM.EmittingViewModel
            {
                Id = source.Id,
                StartFrequency_MHz = source.StartFrequency_MHz,
                StopFrequency_MHz = source.StopFrequency_MHz,
                CurentPower_dBm = source.CurentPower_dBm,
                ReferenceLevel_dBm = source.ReferenceLevel_dBm,
                MeanDeviationFromReference = source.MeanDeviationFromReference,
                TriggerDeviationFromReference = source.TriggerDeviationFromReference,
                Bandwidth_kHz = source.Spectrum == null ? 0 : source.Spectrum.Bandwidth_kHz,
                CorrectnessEstimations = source.Spectrum == null ? true : source.Spectrum.CorrectnessEstimations,
                Contravention = source.Spectrum == null ? true : source.Spectrum.Contravention,
                TraceCount = source.Spectrum == null ? 0 : source.Spectrum.TraceCount,
                SignalLevel_dBm = source.Spectrum == null ? 0 : source.Spectrum.SignalLevel_dBm,
                RollOffFactor = source.EmittingParameters.RollOffFactor,
                StandardBW = source.EmittingParameters.StandardBW,
                WorkTimes = source.WorkTimes,
                Spectrum = source.Spectrum,
                LevelsDistribution = source.LevelsDistribution,
                SensorName = source.SensorName,
                SensorTitle = source.SensorTitle,
                SumHitCount = source.WorkTimes == null ? 0 : source.WorkTimes.Sum(c => c.HitCount),
                EmissionFreqMHz = source.Spectrum == null ? 0 : source.Spectrum.SpectrumSteps_kHz * (source.Spectrum.T2 + source.Spectrum.T1)/2000 + source.Spectrum.SpectrumStartFreq_MHz,
                IcsmID = source.AssociatedStationID,
                IcsmTable = source.AssociatedStationTableName,
                MeasResultId = source.MeasResultId
            };
        }
        public static VM.EmittingWorkTimeViewModel Map(SDR.WorkTime source)
        {
            if (source == null)
            {
                return null;
            }

            return new VM.EmittingWorkTimeViewModel
            {
                StartEmitting = source.StartEmitting,
                StopEmitting = source.StopEmitting,
                HitCount = source.HitCount,
                PersentAvailability = source.PersentAvailability
            };
        }
        public static VM.MeasStationsSignalizationViewModel Map(M.MeasStationsSignalization source)
        {
            if (source == null)
            {
                return null;
            }

            return new VM.MeasStationsSignalizationViewModel
            {
                IcsmId = source.IcsmId,
                IcsmTable = source.IcsmTable,
                Agl = source.Agl,
                Bw = source.Bw,
                Lat = source.Lat,
                Lon = source.Lon,
                Distance = source.Distance,
                Eirp = source.Eirp,
                Freq = source.Freq,
                Owner = source.Owner,
                RelivedLevel = source.RelivedLevel,
                Standart = source.Standart,
                StationName = source.StationName,
                Status = source.Status
            };
        }
        public static VM.StationsEquipmentViewModel Map(M.StationsEquipment source)
        {
            if (source == null)
            {
                return null;
            }
             return new VM.StationsEquipmentViewModel
            {
                Code = source.Code,
                DesigEmission = source.DesigEmission,
                Freq_MHz = source.Freq_MHz,
                LowerFreq = source.LowerFreq,
                UpperFreq = source.UpperFreq,
                MaxPower = source.MaxPower,
                Name = source.Name,
                Manufacturer = source.Manufacturer,
                Loss = source.Loss,
                Freq = source.Freq,
                IcsmId = source.IcsmId,
                IcsmTable = source.IcsmTable,
                Status = source.Status
            };
        }
        public static VM.MeasTaskShortViewModel Map(M.MeasTask source)
        {
            if (source == null)
            {
                return null;
            }
            return new VM.MeasTaskShortViewModel
            {
                MeasTaskId = source.MeasTaskId,
                TaskType = source.TaskType,
                TaskName = source.TaskName,
                FqMin = source.FqMin,
                FqMax = source.FqMax,
                DateStart = source.DateStart,
                DateStop = source.DateStop,
                DateCreated = source.DateCreated,
                CreatedBy = source.CreatedBy,
                Status = source.Status,
                SensorIds = source.SensorIds
            };
        }
        public static VM.DataSynchronizationProcessProtocolsViewModel Map(SDRI.DetailProtocols source)
        {
            if (source == null)
                return null;

            string statusMeasFull = "-";
            switch (source.StatusMeas)
            {
                case "T":
                    statusMeasFull = Properties.Resources.Status_OperatingAccordingToTest;
                    break;
                case "A":
                    statusMeasFull = Properties.Resources.Status_OperatingAccordingToLicense;
                    break;
                case "U":
                    statusMeasFull = Properties.Resources.Status_TransmitterOperationNotFixed;
                    break;
                case "I":
                    statusMeasFull = Properties.Resources.Status_IllegallyOperatedTransmitter;
                    break;
                default:
                    break;
            }

            return new VM.DataSynchronizationProcessProtocolsViewModel
            {
                
                Id = source.Id,
                Address = source.Address,
                BandWidth = source.BandWidth,
                CreatedBy = source.CreatedBy,
                CurentStatusStation = source.CurentStatusStation,
                DateCreated = source.DateCreated,
                DateMeas = source.DateMeas,
                DateMeas_OnlyDate = source.DateMeas_OnlyDate,
                DateMeas_OnlyTime = source.DateMeas_OnlyTime,
                DurationMeasurement = source.DurationMeasurement,
                FieldStrength = source.FieldStrength,
                Freq_MHz = source.Freq_MHz,
                GlobalSID = source.GlobalSID,
                Latitude = source.Latitude,
                Longitude = source.Longitude,
                //Coordinates = (source.Longitude.HasValue ? source.Longitude.Value.ToString() : "") + ", " + (source.Latitude.HasValue ? source.Latitude.Value.ToString() : ""),
                Coordinates = ConvertCoordinates.DecToDmsToString2(source.Longitude.GetValueOrDefault(), EnumCoordLine.Lon) + ", " + ConvertCoordinates.DecToDmsToString2(source.Latitude.GetValueOrDefault(), EnumCoordLine.Lat),
                CoordinatesLat = ConvertCoordinates.DecToDmsToString2(source.Latitude.GetValueOrDefault(), EnumCoordLine.Lat),
                CoordinatesLon = ConvertCoordinates.DecToDmsToString2(source.Longitude.GetValueOrDefault(), EnumCoordLine.Lon),
                Level_dBm = source.Level_dBm,
                OwnerName = source.OwnerName,
                PermissionGlobalSID = source.PermissionGlobalSID,
                PermissionNumber = source.PermissionNumber,
                PermissionStart = source.PermissionStart,
                PermissionStop = source.PermissionStop,
                RadioControlBandWidth_KHz = source.RadioControlBandWidth_KHz,
                RadioControlDeviationFreq_MHz = source.RadioControlDeviationFreq_MHz,
                RadioControlMeasFreq_MHz = source.RadioControlMeasFreq_MHz,
                SensorLatitude = source.SensorLatitude,
                SensorLongitude = source.SensorLongitude,
                //SensorCoordinates = (source.SensorLongitude.HasValue ? source.SensorLongitude.Value.ToString() : "") + ", " + (source.SensorLatitude.HasValue ? source.SensorLatitude.Value.ToString() : ""),
                SensorCoordinates = ConvertCoordinates.DecToDmsToString2(source.SensorLongitude.GetValueOrDefault(), EnumCoordLine.Lon) + ", " + ConvertCoordinates.DecToDmsToString2(source.SensorLatitude.GetValueOrDefault(), EnumCoordLine.Lat),
                SensorCoordinatesLat = ConvertCoordinates.DecToDmsToString2(source.SensorLatitude.GetValueOrDefault(), EnumCoordLine.Lat),
                SensorCoordinatesLon = ConvertCoordinates.DecToDmsToString2(source.SensorLongitude.GetValueOrDefault(), EnumCoordLine.Lon),
                SensorName = source.SensorName,
                Standard = source.Standard,
                StandardName = source.StandardName,
                StationChannel = source.StationChannel,
                StationTxFreq = source.StationTxFreq,
                StatusMeas = source.StatusMeas,
                StatusMeasFull = statusMeasFull,
                TitleSensor = source.TitleSensor,
                ProtocolsLinkedWithEmittings = source.ProtocolsLinkedWithEmittings
            };
        }
        public static VM.DataSynchronizationProcessViewModel Map(SDRI.HeadProtocols source)
        {
            if (source == null)
                return null;

            string statusMeasStationFull = "-";
            switch (source.StatusMeasStation)
            {
                case "T":
                    statusMeasStationFull = Properties.Resources.Status_OperatingAccordingToTest;
                    break;
                case "A":
                    statusMeasStationFull = Properties.Resources.Status_OperatingAccordingToLicense;
                    break;
                case "U":
                    statusMeasStationFull = Properties.Resources.Status_TransmitterOperationNotFixed;
                    break;
                case "I":
                    statusMeasStationFull = Properties.Resources.Status_IllegallyOperatedTransmitter;
                    break;
                default:
                    break;
            }

            return new VM.DataSynchronizationProcessViewModel
            {
                GSID = source.PermissionGlobalSID,
                DateMeas = source.DateMeas,
                Owner = source.OwnerName,
                StationAddress = source.Address,
                //Coordinates = source.Longitude.ToString() + ", " + source.Latitude.ToString(),
                Coordinates = ConvertCoordinates.DecToDmsToString2(source.Longitude.GetValueOrDefault(), EnumCoordLine.Lon) + ", " + ConvertCoordinates.DecToDmsToString2(source.Latitude.GetValueOrDefault(), EnumCoordLine.Lat),
                CoordinatesLon = ConvertCoordinates.DecToDmsToString2(source.Longitude.GetValueOrDefault(), EnumCoordLine.Lon),
                CoordinatesLat = ConvertCoordinates.DecToDmsToString2(source.Latitude.GetValueOrDefault(), EnumCoordLine.Lat),
                NumberPermission = source.PermissionNumber,
                PermissionStart = source.PermissionStart,
                PermissionPeriod = source.PermissionStop,
                SensorName = source.TitleSensor,
                StatusMeasStation = source.StatusMeasStation,
                StatusMeasStationFull = statusMeasStationFull,
                DetailProtocols = source.DetailProtocols
            };
        }


    public static VM.RefSpectrumViewModel Map(SDRI.RefSpectrum source)
        {
            if (source == null)
                return null;

            return new VM.RefSpectrumViewModel
            {
                Id = source.Id,
                FileName = source.FileName,
                DateCreated = source.DateCreated,
                CreatedBy = source.CreatedBy,
                CountImportRecords = source.CountImportRecords,
                MinFreqMHz = source.MinFreqMHz,
                MaxFreqMHz = source.MaxFreqMHz,
                CountSensors = source.CountSensors,
                DataRefSpectrum = source.DataRefSpectrum
            };

        }
        public static VM.ShortMeasTaskViewModel Map(SDR.ShortMeasTask source)
        {
            if (source == null)
                return null;
            string statusFull = PluginHelper.GetFullTaskStatus(source.Status);

            return new VM.ShortMeasTaskViewModel
            {
                CreatedBy = source.CreatedBy,
                DateCreated = source.DateCreated.ToNull(),
                ExecutionMode = source.ExecutionMode,
                Id = source.Id.Value,
                MaxTimeBs = source.MaxTimeBs.ToNull(),
                Name = source.Name,
                OrderId = source.OrderId,
                Prio = source.Prio.ToNull(),
                ResultType = source.ResultType,
                Status = source.Status,
                StatusFull = statusFull,
                Task = source.Task,
                Type = source.Type,
                TypeMeasurements = source.TypeMeasurements,
                TypeMeasurementsString = source.TypeMeasurements.ToString()
            };
        }
    };
}
