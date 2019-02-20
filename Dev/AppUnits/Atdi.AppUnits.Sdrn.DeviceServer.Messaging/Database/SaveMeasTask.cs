using System.Collections.Generic;
using Atdi.Contracts.CoreServices.DataLayer;
using Atdi.Contracts.CoreServices.EntityOrm;
using Atdi.DataModels.DataConstraint;
using Atdi.Platform.Logging;
using System;
using MD = Atdi.DataModels.Sdrns.DeviceServer.Entities;
using Atdi.DataModels.EntityOrm;
using Atdi.Modules.Sdrn.DeviceServer;
using System.Xml;
using System.Linq;
using Atdi.DataModels.Sdrns.Device;



namespace Atdi.AppUnits.Sdrn.DeviceServer.Messaging.Database
{ 
    public class SaveMeasTask
    {
        private readonly IDataLayer<EntityDataOrm> _dataLayer;
        private readonly ILogger _logger;


        public SaveMeasTask(IDataLayer<EntityDataOrm> dataLayer, ILogger logger)
        {
            this._dataLayer = dataLayer;
            this._logger = logger;
        }




        public int? SaveMeasTaskInDB(MeasTask value)
        {
            int? ID = null;
            var queryExecuter = this._dataLayer.Executor<SdrnServerDeviceDataContext>();
            if (value != null)
            {
                try
                {
                    queryExecuter.BeginTransaction();

                    var builderInsertMeasTask = this._dataLayer.GetBuilder<MD.IMeasTask>().Insert();
                    builderInsertMeasTask.SetValue(c => c.EquipmentTechId, value.EquipmentTechId);
                    if ((value.MobEqipmentMeasurements != null) && (value.MobEqipmentMeasurements.Length > 0))
                    {
                        builderInsertMeasTask.SetValue(c => c.MobEqipmentMeasurements, string.Join(",", value.MobEqipmentMeasurements));
                    }
                    builderInsertMeasTask.SetValue(c => c.Priority, value.Priority);
                    builderInsertMeasTask.SetValue(c => c.ScanPerTaskNumber, value.ScanPerTaskNumber);
                    builderInsertMeasTask.SetValue(c => c.SdrnServer, value.SdrnServer);
                    builderInsertMeasTask.SetValue(c => c.SensorName, value.SensorName);
                    builderInsertMeasTask.SetValue(c => c.Status, value.Status);
                    builderInsertMeasTask.SetValue(c => c.TaskId, value.TaskId);
                    builderInsertMeasTask.SetValue(c => c.TimeStart, value.StartTime);
                    builderInsertMeasTask.SetValue(c => c.TimeStop, value.StopTime);
                    builderInsertMeasTask.Select(c => c.Id);
                    queryExecuter.ExecuteAndFetch(builderInsertMeasTask, readerMeasTask =>
                    {
                        while (readerMeasTask.Read())
                        {
                            ID = readerMeasTask.GetValue(c => c.Id);
                        }
                        return true;
                    });

                    if ((ID != null) && (ID.Value > 0))
                    {
                        if (value.ScanParameters != null)
                        {
                            for (int i = 0; i < value.ScanParameters.Length; i++)
                            {
                                int? idStandardScanParameter = null;
                                var builderInsertStandardScanParameter = this._dataLayer.GetBuilder<MD.IStandardScanParameter>().Insert();
                                builderInsertStandardScanParameter.SetValue(c => c.DetectLevelDbm, value.ScanParameters[i].DetectionLevel_dBm);
                                if (value.ScanParameters[i].DeviceParam != null)
                                {
                                    var deviceParam = value.ScanParameters[i].DeviceParam;
                                    builderInsertStandardScanParameter.SetValue(c => c.DetectType, deviceParam.DetectType.ToString());
                                    builderInsertStandardScanParameter.SetValue(c => c.MeasTime_sec, deviceParam.MeasTime_sec);
                                    builderInsertStandardScanParameter.SetValue(c => c.Preamplification_dB, deviceParam.Preamplification_dB);
                                    builderInsertStandardScanParameter.SetValue(c => c.RBW_kHz, deviceParam.RBW_kHz);
                                    builderInsertStandardScanParameter.SetValue(c => c.RefLevel_dBm, deviceParam.RefLevel_dBm);
                                    builderInsertStandardScanParameter.SetValue(c => c.RfAttenuation_dB, deviceParam.RfAttenuation_dB);
                                    builderInsertStandardScanParameter.SetValue(c => c.ScanBW_kHz, deviceParam.ScanBW_kHz);
                                    builderInsertStandardScanParameter.SetValue(c => c.VBW_kHz, deviceParam.VBW_kHz);
                                }
                                builderInsertStandardScanParameter.SetValue(c => c.MeasFreqRelative, value.ScanParameters[i].MaxFrequencyRelativeOffset_mk);
                                builderInsertStandardScanParameter.SetValue(c => c.MaxPermissBW, value.ScanParameters[i].MaxPermissionBW_kHz);
                                builderInsertStandardScanParameter.SetValue(c => c.Standard, value.ScanParameters[i].Standard);
                                builderInsertStandardScanParameter.SetValue(c => c.LevelDbm, value.ScanParameters[i].XdBLevel_dB);
                                builderInsertStandardScanParameter.SetValue(c => c.MeasTaskId, ID);
                                builderInsertStandardScanParameter.Select(c => c.Id);
                                queryExecuter.ExecuteAndFetch(builderInsertStandardScanParameter, readerStandardScanParameter =>
                                {
                                    while (readerStandardScanParameter.Read())
                                    {
                                        idStandardScanParameter = readerStandardScanParameter.GetValue(c => c.Id);
                                    }
                                    return true;
                                });
                            }
                        }


                        if (value.Stations != null)
                        {
                            for (int i = 0; i < value.Stations.Length; i++)
                            {
                                int? idIStationLicenseInfo = null;
                                var builderInsertStationLicenseInfo = this._dataLayer.GetBuilder<MD.IStationLicenseInfo>().Insert();
                                builderInsertStationLicenseInfo.SetValue(c => c.IcsmId, value.Stations[i].License.IcsmId);
                                builderInsertStationLicenseInfo.SetValue(c => c.CloseDate, value.Stations[i].License.CloseDate);
                                builderInsertStationLicenseInfo.SetValue(c => c.EndDate, value.Stations[i].License.EndDate);
                                builderInsertStationLicenseInfo.SetValue(c => c.Name, value.Stations[i].License.Name);
                                builderInsertStationLicenseInfo.SetValue(c => c.StartDate, value.Stations[i].License.StartDate);
                                builderInsertStationLicenseInfo.Select(c => c.Id);
                                queryExecuter.ExecuteAndFetch(builderInsertStationLicenseInfo, readerStationLicenseInfo =>
                                {
                                    while (readerStationLicenseInfo.Read())
                                    {
                                        idIStationLicenseInfo = readerStationLicenseInfo.GetValue(c => c.Id);
                                    }
                                    return true;
                                });

                                int? idOwnerData = null;
                                var builderInsertIOwnerData = this._dataLayer.GetBuilder<MD.IOwnerData>().Insert();
                                builderInsertIOwnerData.SetValue(c => c.Address, value.Stations[i].Owner.Address);
                                builderInsertIOwnerData.SetValue(c => c.CODE, value.Stations[i].Owner.Code);
                                builderInsertIOwnerData.SetValue(c => c.OKPO, value.Stations[i].Owner.OKPO);
                                builderInsertIOwnerData.SetValue(c => c.OwnerName, value.Stations[i].Owner.OwnerName);
                                builderInsertIOwnerData.SetValue(c => c.ZIP, value.Stations[i].Owner.Zip);
                                builderInsertIOwnerData.Select(c => c.Id);
                                queryExecuter.ExecuteAndFetch(builderInsertIOwnerData, readerOwnerData =>
                                {
                                    while (readerOwnerData.Read())
                                    {
                                        idOwnerData = readerOwnerData.GetValue(c => c.Id);
                                    }
                                    return true;
                                });

                                int? idStationSite = null;
                                var builderStationSite = this._dataLayer.GetBuilder<MD.IStationSite>().Insert();
                                builderStationSite.SetValue(c => c.Address, value.Stations[i].Site.Adress);
                                builderStationSite.SetValue(c => c.Lat, value.Stations[i].Site.Lat);
                                builderStationSite.SetValue(c => c.Lon, value.Stations[i].Site.Lon);
                                builderStationSite.SetValue(c => c.Region, value.Stations[i].Site.Region);
                                builderStationSite.Select(c => c.Id);
                                queryExecuter.ExecuteAndFetch(builderStationSite, readerStationSite =>
                                {
                                    while (readerStationSite.Read())
                                    {
                                        idStationSite = readerStationSite.GetValue(c => c.Id);
                                    }
                                    return true;
                                });


                                int? idMeasStation = null;
                                var builderInsertMeasStation = this._dataLayer.GetBuilder<MD.IMeasStation>().Insert();
                                builderInsertMeasStation.SetValue(c => c.GlobalSid, value.Stations[i].GlobalSid);
                                builderInsertMeasStation.SetValue(c => c.MeasTaskId, ID.Value);
                                builderInsertMeasStation.SetValue(c => c.OwnerGlobalSid, value.Stations[i].OwnerGlobalSid);
                                builderInsertMeasStation.SetValue(c => c.Standard, value.Stations[i].Standard);
                                builderInsertMeasStation.SetValue(c => c.StationId, value.Stations[i].StationId);
                                builderInsertMeasStation.SetValue(c => c.Status, value.Stations[i].Status);
                                builderInsertMeasStation.SetValue(c => c.LicenceId, idIStationLicenseInfo);
                                builderInsertMeasStation.SetValue(c => c.OwnerId, idOwnerData);
                                builderInsertMeasStation.SetValue(c => c.SiteId, idStationSite);
                                builderInsertMeasStation.Select(c => c.Id);
                                queryExecuter.ExecuteAndFetch(builderInsertMeasStation, readerMeasStation =>
                                {
                                    while (readerMeasStation.Read())
                                    {
                                        idMeasStation = readerMeasStation.GetValue(c => c.Id);
                                    }
                                    return true;
                                });

                                if ((idMeasStation != null) && (idMeasStation > 0) && (value.Stations[i].Sectors!=null))
                                {
                                    for (int j = 0; j < value.Stations[i].Sectors.Length; j++)
                                    {
                                        int? idSector = null;
                                        var builderInsertISector = this._dataLayer.GetBuilder<MD.ISector>().Insert();
                                        builderInsertISector.SetValue(c => c.Agl, value.Stations[i].Sectors[j].AGL);
                                        builderInsertISector.SetValue(c => c.Azimut, value.Stations[i].Sectors[j].Azimuth);
                                        builderInsertISector.SetValue(c => c.Bw, value.Stations[i].Sectors[j].BW_kHz);
                                        builderInsertISector.SetValue(c => c.ClassEmission, value.Stations[i].Sectors[j].ClassEmission);
                                        builderInsertISector.SetValue(c => c.Eirp, value.Stations[i].Sectors[j].EIRP_dBm);
                                        builderInsertISector.SetValue(c => c.SectorId, value.Stations[i].Sectors[j].SectorId);
                                        builderInsertISector.SetValue(c => c.StationId, idMeasStation);
                                        builderInsertISector.Select(c => c.Id);
                                        queryExecuter.ExecuteAndFetch(builderInsertISector, readerSector =>
                                        {
                                            while (readerSector.Read())
                                            {
                                                idSector = readerSector.GetValue(c => c.Id);
                                            }
                                            return true;
                                        });


                                        if ((idSector != null) && (idSector > 0) && (value.Stations[i].Sectors[j].Frequencies!=null))
                                        {
                                            var queryStatements = new List<IQueryInsertStatement<MD.ISectorFreq>>();
                                            for (int l = 0; l < value.Stations[i].Sectors[j].Frequencies.Length; l++)
                                            {
                                                var builderInsertSectorFreq = this._dataLayer.GetBuilder<MD.ISectorFreq>().Insert();
                                                builderInsertSectorFreq.SetValue(c => c.ChannelNumber, value.Stations[i].Sectors[j].Frequencies[l].ChannelNumber);
                                                builderInsertSectorFreq.SetValue(c => c.Frequency, (double?)value.Stations[i].Sectors[j].Frequencies[l].Frequency_MHz);
                                                builderInsertSectorFreq.SetValue(c => c.IdSector, idSector);
                                                builderInsertSectorFreq.SetValue(c => c.PlanId, value.Stations[i].Sectors[j].Frequencies[l].PlanId);
                                                builderInsertSectorFreq.Select(c => c.Id);
                                                queryStatements.Add(builderInsertSectorFreq);
                                            }
                                            queryExecuter.ExecuteAndFetch(queryStatements.ToArray(), readerSectorFreq =>
                                            {
                                                return true;
                                            });
                                        }


                                        if ((idSector != null) && (idSector > 0) && (value.Stations[i].Sectors[j].BWMask != null))
                                        {
                                            var queryStatements = new List<IQueryInsertStatement<MD.ISectorMaskElement>>();
                                            for (int l = 0; l < value.Stations[i].Sectors[j].BWMask.Length; l++)
                                            {
                                                var builderInsertSectorMaskElement = this._dataLayer.GetBuilder<MD.ISectorMaskElement>().Insert();
                                                builderInsertSectorMaskElement.SetValue(c => c.Bw, value.Stations[i].Sectors[j].BWMask[l].BW_kHz);
                                                builderInsertSectorMaskElement.SetValue(c => c.Level, value.Stations[i].Sectors[j].BWMask[l].Level_dB);
                                                builderInsertSectorMaskElement.SetValue(c => c.IdSector, idSector);
                                                builderInsertSectorMaskElement.Select(c => c.Id);
                                                queryStatements.Add(builderInsertSectorMaskElement);
                                            }
                                            if ((queryStatements != null) && (queryStatements.Count > 0))
                                            {
                                                queryExecuter.ExecuteAndFetch(queryStatements.ToArray(), readerSectorMaskElement =>
                                                {
                                                    return true;
                                                });
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    queryExecuter.CommitTransaction();
                }
                catch (Exception)
                {
                    queryExecuter.RollbackTransaction();
                }
            }
            return ID;
        }

    }
}


