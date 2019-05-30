using Atdi.Contracts.Api.EventSystem;
using Atdi.Platform.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.Contracts.Sdrn.Server;
using MSG = Atdi.DataModels.Sdrns.BusMessages;
using DEV = Atdi.DataModels.Sdrns.Device;
using Atdi.Contracts.CoreServices.DataLayer;
using Atdi.Contracts.CoreServices.EntityOrm;
using Atdi.DataModels.DataConstraint;
using MD = Atdi.DataModels.Sdrns.Server.Entities;
using Atdi.Contracts.WcfServices.Sdrn.Server;




namespace Atdi.AppUnits.Sdrn.Server.PrimaryHandlers.Subscribes
{

    public class SaveMeasTask
    {
        private readonly ILogger _logger;
        private readonly IDataLayer<EntityDataOrm> _dataLayer;

        public SaveMeasTask( IDataLayer<EntityDataOrm> dataLayer,  ILogger logger)
        {
            this._logger = logger;
            this._dataLayer = dataLayer;
        }

        

        public  Atdi.DataModels.Sdrns.Device.MeasTask[] CreateeasTaskSDRsApi(MeasTask task, string SensorName, string SdrnServer, string EquipmentTechId, int? MeasTaskId, int? SensorId, string Type = "New")
        {
            List<Atdi.DataModels.Sdrns.Device.MeasTask> ListMTSDR = new List<Atdi.DataModels.Sdrns.Device.MeasTask>();
            if (task.MeasSubTasks == null) return null;

            for (int f = 0; f < task.MeasSubTasks.Length; f++)
            {
                var SubTask = task.MeasSubTasks[f];

                if (SubTask.MeasSubTaskStations != null)
                {
                    for (int g = 0; g < SubTask.MeasSubTaskStations.Length; g++)
                    {
                        var SubTaskStation = SubTask.MeasSubTaskStations[g];

                        if ((Type == "New") || ((Type == "Stop") && ((SubTaskStation.Status == "F") || (SubTaskStation.Status == "P"))) || ((Type == "Run") && ((SubTaskStation.Status == "O") || (SubTaskStation.Status == "A"))) ||
                            ((Type == "Del") && (SubTaskStation.Status == "Z")))
                        {
                            if (SensorId!= SubTaskStation.StationId.Value)
                            {
                                continue;
                            }

                            Atdi.DataModels.Sdrns.Device.MeasTask MTSDR = new Atdi.DataModels.Sdrns.Device.MeasTask();
                            MTSDR.TaskId = string.Format("{0}|{1}|{2}|{3}", MeasTaskId, SubTask.Id.Value, SubTaskStation.Id, SubTaskStation.StationId.Value);
                            if (task.Id == null) task.Id = new MeasTaskIdentifier();
                            if (task.MeasOther == null) task.MeasOther = new MeasOther();
                            if (task.MeasDtParam == null) { task.MeasDtParam = new MeasDtParam(); }
                            if (task.Prio != null) { MTSDR.Priority = task.Prio.GetValueOrDefault(); } else { MTSDR.Priority = 10; }
                            MTSDR.SensorName = SensorName;
                            MTSDR.SdrnServer = SdrnServer;
                            MTSDR.EquipmentTechId = EquipmentTechId;
                            if (Type == "New")
                            {
                                MTSDR.SOParam = new DEV.SpectrumOccupationMeasParam();
                                if (task.MeasOther.LevelMinOccup != null) { MTSDR.SOParam.LevelMinOccup_dBm = task.MeasOther.LevelMinOccup.GetValueOrDefault(); } else { MTSDR.SOParam.LevelMinOccup_dBm = -70; }
                                if (task.MeasOther.NChenal != null) { MTSDR.SOParam.MeasurmentNumber = task.MeasOther.NChenal.GetValueOrDefault(); } else { MTSDR.SOParam.MeasurmentNumber = 10; }
                                switch (task.MeasOther.TypeSpectrumOccupation)
                                {
                                    case SpectrumOccupationType.FreqBandwidthOccupation:
                                        MTSDR.SOParam.Type = DataModels.Sdrns.SpectrumOccupationType.FreqBandOccupancy;
                                        break;
                                    case SpectrumOccupationType.FreqChannelOccupation:
                                        MTSDR.SOParam.Type = DataModels.Sdrns.SpectrumOccupationType.FreqChannelOccupancy;
                                        break;
                                    default:
                                        throw new NotImplementedException($"Type '{task.MeasOther.TypeSpectrumOccupation}' not supported");
                                }
                                switch (task.MeasDtParam.TypeMeasurements)
                                {
                                    case MeasurementType.MonitoringStations:
                                        MTSDR.Measurement = DataModels.Sdrns.MeasurementType.MonitoringStations;
                                        break;
                                    case MeasurementType.Level:
                                        MTSDR.Measurement = DataModels.Sdrns.MeasurementType.Level;
                                        break;
                                    case MeasurementType.SpectrumOccupation:
                                        MTSDR.Measurement = DataModels.Sdrns.MeasurementType.SpectrumOccupation;
                                        break;
                                    case MeasurementType.BandwidthMeas:
                                        MTSDR.Measurement = DataModels.Sdrns.MeasurementType.BandwidthMeas;
                                        break;
                                    case MeasurementType.Signaling:
                                        MTSDR.Measurement = DataModels.Sdrns.MeasurementType.Signaling;
                                        break;

                                    default:
                                        throw new NotImplementedException($"Type '{task.MeasDtParam.TypeMeasurements}' not supported");
                                }
                                if (SubTask.Interval != null) { MTSDR.Interval_sec = SubTask.Interval.GetValueOrDefault(); }
                                MTSDR.DeviceParam = new DEV.DeviceMeasParam();
                                if (task.MeasDtParam.MeasTime != null) { MTSDR.DeviceParam.MeasTime_sec = task.MeasDtParam.MeasTime.GetValueOrDefault(); } else { MTSDR.DeviceParam.MeasTime_sec = 0.001; }


                                switch (task.MeasDtParam.DetectType)
                                {
                                    case DetectingType.Avarage:
                                        MTSDR.DeviceParam.DetectType = DataModels.Sdrns.DetectingType.Average;
                                        break;
                                    case DetectingType.MaxPeak:
                                        MTSDR.DeviceParam.DetectType = DataModels.Sdrns.DetectingType.MaxPeak;
                                        break;
                                    case DetectingType.MinPeak:
                                        MTSDR.DeviceParam.DetectType = DataModels.Sdrns.DetectingType.MinPeak;
                                        break;
                                    case DetectingType.Peak:
                                        MTSDR.DeviceParam.DetectType = DataModels.Sdrns.DetectingType.Peak;
                                        break;
                                    default:
                                        throw new NotImplementedException($"Type '{task.MeasDtParam.DetectType}' not supported");
                                }

                                MTSDR.DeviceParam.Preamplification_dB = task.MeasDtParam.Preamplification;
                                if (task.MeasDtParam.RBW != null) { MTSDR.DeviceParam.RBW_kHz = task.MeasDtParam.RBW.GetValueOrDefault(); } else { MTSDR.DeviceParam.RBW_kHz= 10; }
                                MTSDR.DeviceParam.RfAttenuation_dB = (int)task.MeasDtParam.RfAttenuation;
                                if (task.MeasDtParam.VBW != null) { MTSDR.DeviceParam.VBW_kHz = task.MeasDtParam.VBW.GetValueOrDefault(); } else { MTSDR.DeviceParam.VBW_kHz = 10; }
                                MTSDR.DeviceParam.RefLevel_dBm = -30;

                                MTSDR.Frequencies = new DEV.MeasuredFrequencies();
                                if (task.MeasFreqParam != null)
                                {
                                    var freqs = task.MeasFreqParam.MeasFreqs;
                                    if (freqs != null)
                                    {
                                        Double[] listFreqs = new double[freqs.Length];
                                        for (int j = 0; j < freqs.Length; j++)
                                        {
                                            listFreqs[j] = freqs[j].Freq;
                                        }
                                        MTSDR.Frequencies.Values_MHz = listFreqs;
                                    }

                                    if (task.MeasFreqParam.Mode == FrequencyMode.FrequencyList)
                                    {
                                        MTSDR.Frequencies.Mode = DataModels.Sdrns.FrequencyMode.FrequencyList;
                                    }
                                    else if (task.MeasFreqParam.Mode == FrequencyMode.FrequencyRange)
                                    {
                                        MTSDR.Frequencies.Mode = DataModels.Sdrns.FrequencyMode.FrequencyRange;
                                    }
                                    else if (task.MeasFreqParam.Mode == FrequencyMode.SingleFrequency)
                                    {
                                        MTSDR.Frequencies.Mode = DataModels.Sdrns.FrequencyMode.SingleFrequency;
                                    }
                                    else
                                    {
                                        throw new NotImplementedException($"Type '{MTSDR.Frequencies.Mode}' not supported");
                                    }
                                    MTSDR.Frequencies.RgL_MHz = task.MeasFreqParam.RgL;
                                    MTSDR.Frequencies.RgU_MHz = task.MeasFreqParam.RgU;
                                    MTSDR.Frequencies.Step_kHz = task.MeasFreqParam.Step;
                                }

                                if (task.SignalingMeasTaskParameters != null)
                                {
                                    MTSDR.SignalingMeasTaskParameters = new DEV.SignalingMeasTask();
                                    MTSDR.SignalingMeasTaskParameters.allowableExcess_dB = task.SignalingMeasTaskParameters.allowableExcess_dB;
                                    MTSDR.SignalingMeasTaskParameters.AutoDivisionEmitting = task.SignalingMeasTaskParameters.AutoDivisionEmitting;
                                    MTSDR.SignalingMeasTaskParameters.CompareTraceJustWithRefLevels = task.SignalingMeasTaskParameters.CompareTraceJustWithRefLevels;
                                    MTSDR.SignalingMeasTaskParameters.DifferenceMaxMax = task.SignalingMeasTaskParameters.DifferenceMaxMax;
                                    MTSDR.SignalingMeasTaskParameters.FiltrationTrace = task.SignalingMeasTaskParameters.FiltrationTrace;
                                    MTSDR.SignalingMeasTaskParameters.SignalizationNChenal = task.SignalingMeasTaskParameters.SignalizationNChenal;
                                    MTSDR.SignalingMeasTaskParameters.SignalizationNCount = task.SignalingMeasTaskParameters.SignalizationNCount;
                                }

                                    if (task.RefSituation!=null)
                                {
                                    if (task.RefSituation != null)
                                    {
                                        var listReferenceSituation = new List<DEV.ReferenceSituation>();
                                        for (int k = 0; k < task.RefSituation.Length; k++)
                                        {
                                            var refSituation = new DEV.ReferenceSituation();
                                            var refSituationTemp = task.RefSituation[k];
                                            refSituation.SensorId = refSituationTemp.SensorId;

                                            var referenceSignal = refSituationTemp.ReferenceSignal;
                                            if (referenceSignal.Length > 0)
                                            {
                                                refSituation.ReferenceSignal = new DEV.ReferenceSignal[referenceSignal.Length];
                                                for (int l = 0; l < referenceSignal.Length; l++)
                                                {
                                                    var refSituationReferenceSignal = refSituation.ReferenceSignal[l];
                                                    refSituationReferenceSignal = new DEV.ReferenceSignal();
                                                    refSituationReferenceSignal.Bandwidth_kHz = referenceSignal[l].Bandwidth_kHz;
                                                    refSituationReferenceSignal.Frequency_MHz = referenceSignal[l].Frequency_MHz;
                                                    refSituationReferenceSignal.LevelSignal_dBm = referenceSignal[l].LevelSignal_dBm;
                                                    refSituationReferenceSignal.IcsmId = referenceSignal[l].IcsmId;
                                                    refSituationReferenceSignal.SignalMask = new DEV.SignalMask();
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
                                            //MTSDR.RefSituation = listReferenceSituation.ToArray();
                                            MTSDR.RefSituation = listReferenceSituation[0];
                                        }
                                    }
                                }

                                double subFreqMaxMin = 0;
                                if ((MTSDR.Frequencies != null) && (MTSDR.Frequencies.Values_MHz != null) && (MTSDR.Frequencies.Values_MHz.Length > 0))
                                {
                                    var minFreq = MTSDR.Frequencies.Values_MHz.Min();
                                    var maxFreq = MTSDR.Frequencies.Values_MHz.Max();
                                    subFreqMaxMin = maxFreq - minFreq;
                                }
                                if ((subFreqMaxMin >= 0) && (MTSDR.Frequencies.Step_kHz>0))
                                {
                                    MTSDR.DeviceParam.ScanBW_kHz = subFreqMaxMin * 1000 + MTSDR.Frequencies.Step_kHz;
                                }

                                MTSDR.ScanParameters = new DataModels.Sdrns.Device.StandardScanParameter[] { };
                                MTSDR.StartTime = SubTask.TimeStart;
                                MTSDR.StopTime = SubTask.TimeStop;
                                MTSDR.Status = SubTask.Status;
                                MTSDR.MobEqipmentMeasurements = new DataModels.Sdrns.MeasurementType[3];
                                MTSDR.MobEqipmentMeasurements[0] = DataModels.Sdrns.MeasurementType.MonitoringStations;
                                MTSDR.MobEqipmentMeasurements[1] = DataModels.Sdrns.MeasurementType.BandwidthMeas;
                                MTSDR.MobEqipmentMeasurements[2] = DataModels.Sdrns.MeasurementType.Level;
                                if (task.MeasOther.SwNumber != null) { MTSDR.ScanPerTaskNumber = task.MeasOther.SwNumber.GetValueOrDefault(); }
                                if (task.StationsForMeasurements != null)
                                {
                                    MTSDR.Stations = new DataModels.Sdrns.Device.MeasuredStation[task.StationsForMeasurements.Count()];
                                    if (task.MeasDtParam.TypeMeasurements == MeasurementType.MonitoringStations)
                                    { // 21_02_2018 в данном случае мы передаем станции  исключительно для системы мониторинга станций т.е. один таск на месяц Надо проверить.
                                        if (task.StationsForMeasurements != null)
                                        {
                                            ///MTSDR.StationsForMeasurements = task.StationsForMeasurements;
                                            // далее сформируем переменную GlobalSID 
                                            for (int i = 0; i < task.StationsForMeasurements.Count(); i++)
                                            {
                                                MTSDR.Stations[i] = new DataModels.Sdrns.Device.MeasuredStation();
                                                var stationI = MTSDR.Stations[i];
                                                string CodeOwener = "0";
                                                stationI.Owner = new DataModels.Sdrns.Device.StationOwner();
                                                var station = task.StationsForMeasurements[i];
                                                if (station.Owner != null)
                                                {
                                                    var owner = stationI.Owner;
                                                    owner.Address = station.Owner.Addres;
                                                    owner.Code = station.Owner.Code;
                                                    owner.Id = station.Owner.Id;
                                                    owner.OKPO = station.Owner.OKPO;
                                                    owner.OwnerName = station.Owner.OwnerName;
                                                    owner.Zip = station.Owner.Zip;


                                                    if (owner.OKPO == "14333937") { CodeOwener = "1"; };
                                                    if (owner.OKPO == "22859846") { CodeOwener = "6"; };
                                                    if (owner.OKPO == "21673832") { CodeOwener = "3"; };
                                                    if (owner.OKPO == "37815221") { CodeOwener = "7"; };
                                                }
                                                stationI.GlobalSid = "255 " + CodeOwener + " 00000 " + string.Format("{0:00000}", station.IdStation);
                                                station.GlobalSID = stationI.GlobalSid;

                                                stationI.OwnerGlobalSid = task.StationsForMeasurements[i].GlobalSID;//работать с таблицей (доп. создасть в БД по GlobalSID и Standard)
                                                                                                                    //
                                                stationI.License = new DataModels.Sdrns.Device.StationLicenseInfo();
                                                if (station.LicenseParameter != null)
                                                {
                                                    stationI.License.CloseDate = station.LicenseParameter.CloseDate;
                                                    stationI.License.EndDate = station.LicenseParameter.EndDate;
                                                    stationI.License.IcsmId = station.LicenseParameter.Id;
                                                    stationI.License.Name = station.LicenseParameter.DozvilName;
                                                    stationI.License.StartDate = station.LicenseParameter.StartDate;
                                                }

                                                stationI.Site = new DataModels.Sdrns.Device.StationSite();
                                                if (station.Site != null)
                                                {
                                                    stationI.Site.Adress = station.Site.Adress;
                                                    stationI.Site.Lat = station.Site.Lat;
                                                    stationI.Site.Lon = station.Site.Lon;
                                                    stationI.Site.Region = station.Site.Region;
                                                }
                                                stationI.Standard = station.Standart;
                                                stationI.StationId = station.IdStation.ToString();
                                                stationI.Status = station.Status;


                                                if (station.Sectors != null)
                                                {
                                                    stationI.Sectors = new DataModels.Sdrns.Device.StationSector[station.Sectors.Length];
                                                    for (int j = 0; j < station.Sectors.Length; j++)
                                                    {
                                                        var sector = station.Sectors[j];
                                                        stationI.Sectors[j] = new DataModels.Sdrns.Device.StationSector();
                                                        var statSector = stationI.Sectors[j];
                                                        statSector.AGL = sector.AGL;
                                                        statSector.Azimuth = sector.Azimut;

                                                        if (sector.MaskBW != null)
                                                        {
                                                            statSector.BWMask = new DataModels.Sdrns.Device.ElementsMask[sector.MaskBW.Length];
                                                            for (int k = 0; k < sector.MaskBW.Length; k++)
                                                            {
                                                                statSector.BWMask[k] = new DataModels.Sdrns.Device.ElementsMask();
                                                                statSector.BWMask[k].BW_kHz = sector.MaskBW[k].BW;
                                                                statSector.BWMask[k].Level_dB = sector.MaskBW[k].level;
                                                            }
                                                        }
                                                        statSector.BW_kHz = sector.BW;
                                                        statSector.ClassEmission = sector.ClassEmission;
                                                        statSector.EIRP_dBm = sector.EIRP;

                                                        if (sector.Frequencies != null)
                                                        {
                                                            statSector.Frequencies = new DataModels.Sdrns.Device.SectorFrequency[sector.Frequencies.Length];
                                                            for (int k = 0; k < sector.Frequencies.Length; k++)
                                                            {
                                                                statSector.Frequencies[k] = new DataModels.Sdrns.Device.SectorFrequency();
                                                                statSector.Frequencies[k].ChannelNumber = sector.Frequencies[k].ChannalNumber;
                                                                statSector.Frequencies[k].Frequency_MHz = sector.Frequencies[k].Frequency;
                                                                statSector.Frequencies[k].Id = sector.Frequencies[k].Id;
                                                                statSector.Frequencies[k].PlanId = sector.Frequencies[k].IdPlan;
                                                            }
                                                        }
                                                        statSector.SectorId = sector.IdSector.ToString();
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                            ListMTSDR.Add(MTSDR);
                        }
                    }
                }
            }
            return ListMTSDR.ToArray();
        }
       
    }
}

