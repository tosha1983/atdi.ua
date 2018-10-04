using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SDR = Atdi.AppServer.Contracts.Sdrns;
using VM = XICSM.ICSControlClient.Models.Views;
using XICSM.ICSControlClient.Environment.Wpf;


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

            return new VM.MeasTaskViewModel
            {
                CreatedBy = source.CreatedBy,
                DateCreated = source.DateCreated.ToNull(),
                ExecutionMode = source.ExecutionMode,
                Id = source.Id.Value,
                MaxTimeBs = source.MaxTimeBs.ToNull(),

                MeasDtParamDemod = source.MeasDtParam.Demod,
                MeasDtParamDetectType = source.MeasDtParam.DetectType,
                MeasDtParamIfAttenuation = source.MeasDtParam.IfAttenuation,
                MeasDtParamMeasTime = source.MeasDtParam.MeasTime.ToNull(),
                MeasDtParamMode = source.MeasDtParam.Mode,
                MeasDtParamPreamplification = source.MeasDtParam.Preamplification,
                MeasDtParamRBW = source.MeasDtParam.RBW.ToNull(),
                MeasDtParamRfAttenuation = source.MeasDtParam.RfAttenuation,
                MeasDtParamTypeMeasurements = source.MeasDtParam.TypeMeasurements,
                MeasDtParamVBW = source.MeasDtParam.VBW.ToNull(),

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
                Stations = source.Stations
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
                StationMeasurementsStationId = source.StationMeasurements.StationId.Value,
                SubMeasTaskId = source.Id.SubMeasTaskId,
                SubMeasTaskStationId = source.Id.SubMeasTaskStationId,
                TimeMeas = source.TimeMeas,
                TypeMeasurements = source.TypeMeasurements,
                LowFreq = source.FrequenciesMeasurements == null ? (double?)null : (source.FrequenciesMeasurements.Length == 0 ? 0 : source.FrequenciesMeasurements.Min(f => f.Freq)),
                UpFreq = source.FrequenciesMeasurements == null ? (double?)null : (source.FrequenciesMeasurements.Length == 0 ? 0 : source.FrequenciesMeasurements.Max(f => f.Freq)),
                MeasDeviceId = source.StationMeasurements == null ? (int?)null : source.StationMeasurements.StationId.Value,
                StationsNumber = source.ResultsMeasStation == null ? (int?)null : source.ResultsMeasStation.Length,
                PointsNumber = source.MeasurementsResults == null ? (int?)null : source.MeasurementsResults.Length,
            };
        }

        public static VM.ResultsMeasurementsStationViewModel Map(SDR.ResultsMeasurementsStation source)
        {
            if (source == null)
            {
                return null;
            }

            var generalResult = source.GeneralResult ?? new SDR.MeasurementsParameterGeneral();

            return new VM.ResultsMeasurementsStationViewModel
            {
                GeneralResultCentralFrequency = generalResult.CentralFrequency.ToNull(),
                GeneralResultCentralFrequencyMeas = generalResult.CentralFrequencyMeas.ToNull(),
                GeneralResultDurationMeas = generalResult.DurationMeas.ToNull(),
                GeneralResultLevelsSpecrum = generalResult.LevelsSpecrum,
                GeneralResultMarkerIndex = generalResult.MarkerIndex.ToNull(),
                GeneralResultMaskBW = generalResult.MaskBW,
                GeneralResultOffsetFrequency = generalResult.OffsetFrequency.ToNull(),
                GeneralResultSpecrumStartFreq = generalResult.SpecrumStartFreq,
                GeneralResultSpecrumSteps = generalResult.SpecrumSteps,
                GeneralResultT1 = generalResult.T1.ToNull(),
                GeneralResultT2 = generalResult.T2.ToNull(),
                GeneralResultTimeFinishMeas = generalResult.TimeFinishMeas.ToNull(),
                GeneralResultTimeStartMeas = generalResult.TimeStartMeas.ToNull(),

                NumberPointsOfSpectrum = generalResult.LevelsSpecrum == null ? (int?)null : generalResult.LevelsSpecrum.Length,
                T1 = Convert.ToDouble((generalResult.SpecrumStartFreq ?? 0) + (generalResult.T1.ToNull() ?? 0) * (generalResult.SpecrumSteps ?? 0) / 1000),
                T2 = Convert.ToDouble((generalResult.SpecrumStartFreq ?? 0) + (generalResult.T2.ToNull() ?? 0) * (generalResult.SpecrumSteps ?? 0) / 1000),
                Marker = Convert.ToDouble((generalResult.SpecrumStartFreq ?? 0) + (generalResult.MarkerIndex.ToNull() ?? 0) * (generalResult.SpecrumSteps ?? 0) / 1000),

                GlobalSID = source.GlobalSID,
                LevelMeasurements = source.LevelMeasurements,
                LevelMeasurementsLength = source.LevelMeasurements == null ? (int?)null : source.LevelMeasurements.Length,
                MeasGlobalSID = source.MeasGlobalSID,
                SectorId = source.IdSector.ToNull(),
                StationId = source.Idstation,
                Status = source.Status
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
    }
}
