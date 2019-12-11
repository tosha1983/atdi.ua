using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SDR = Atdi.Contracts.WcfServices.Sdrn.Server;
using VM = XICSM.ICSControlClient.Models.Views;
using M = XICSM.ICSControlClient.Models;
using XICSM.ICSControlClient.Environment.Wpf;
using SVC = XICSM.ICSControlClient.WcfServiceClients;

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

                MeasDtParamPreamplification = (source.MeasDtParam.Preamplification.HasValue && source.MeasDtParam.Preamplification.Value == -1) ? (int?)null : source.MeasDtParam.Preamplification ?? 0,
                IsAutoMeasDtParamPreamplification = (source.MeasDtParam.Preamplification.HasValue && source.MeasDtParam.Preamplification.Value == -1),

                MeasDtParamRBW = (source.MeasDtParam.RBW.HasValue && source.MeasDtParam.RBW.Value == -1) ? null : source.MeasDtParam.RBW.ToNull(),
                IsAutoMeasDtParamRBW = (source.MeasDtParam.RBW.HasValue && source.MeasDtParam.RBW.Value == -1),

                MeasDtParamRfAttenuation = (source.MeasDtParam.RfAttenuation.HasValue && source.MeasDtParam.RfAttenuation.Value == -1) ? (double?)null : source.MeasDtParam.RfAttenuation.GetValueOrDefault(),
                IsAutoMeasDtParamRfAttenuation = (source.MeasDtParam.RfAttenuation.HasValue && source.MeasDtParam.RfAttenuation.Value == -1),

                MeasDtParamTypeMeasurements = source.MeasDtParam.TypeMeasurements,

                MeasDtParamVBW = (source.MeasDtParam.VBW.HasValue && source.MeasDtParam.VBW.Value == -1) ? null : source.MeasDtParam.VBW.ToNull(),
                IsAutoMeasDtParamVBW = (source.MeasDtParam.VBW.HasValue && source.MeasDtParam.VBW.Value == -1),

                MeasDtParamReferenceLevel = (source.MeasDtParam.ReferenceLevel.HasValue && source.MeasDtParam.ReferenceLevel.Value == 1000000000) ? null : source.MeasDtParam.ReferenceLevel.ToNull(),
                IsAutoMeasDtParamReferenceLevel = (source.MeasDtParam.ReferenceLevel.HasValue && source.MeasDtParam.ReferenceLevel.Value == 1000000000),

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
                triggerLevel_dBm_Hz = (source.SignalingMeasTaskParameters.triggerLevel_dBm_Hz.HasValue && source.SignalingMeasTaskParameters.triggerLevel_dBm_Hz.Value == -999) ? null : source.SignalingMeasTaskParameters.triggerLevel_dBm_Hz,
                IsAutoTriggerLevel_dBm_Hz = (source.SignalingMeasTaskParameters.triggerLevel_dBm_Hz.HasValue && source.SignalingMeasTaskParameters.triggerLevel_dBm_Hz.Value == -999),

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
                MeasFreqParamRgL = source.MeasFreqParam.RgL.ToNull(),
                MeasFreqParamRgU = source.MeasFreqParam.RgU.ToNull(),
                MeasFreqParamStep = source.MeasFreqParam.Step.ToNull(),

                MeasOtherLevelMinOccup = source.MeasOther.LevelMinOccup.ToNull(),
                MeasOtherNChenal = source.MeasOther.NChenal.ToNull(),
                MeasOtherSwNumber = source.MeasOther.SwNumber.ToNull(),
                MeasOtherTypeSpectrumOccupation = source.MeasOther.TypeSpectrumOccupation,
                MeasOtherTypeSpectrumScan = source.MeasOther.TypeSpectrumScan,

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
                LowFreq = source.FrequenciesMeasurements == null ? (double?)null : (source.FrequenciesMeasurements.Length == 0 ? 0 : source.FrequenciesMeasurements.Min(f => f.Freq)),
                UpFreq = source.FrequenciesMeasurements == null ? (double?)null : (source.FrequenciesMeasurements.Length == 0 ? 0 : source.FrequenciesMeasurements.Max(f => f.Freq)),
                MeasDeviceId = source.StationMeasurements == null ? (long?)null : source.StationMeasurements.StationId.Value,
                StationsNumber = source.ResultsMeasStation == null ? (int?)null : source.ResultsMeasStation.Length,
                PointsNumber = source.MeasurementsResults == null ? (int?)null : source.MeasurementsResults.Length,
                SensorName = source.SensorName,
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
    }
}
