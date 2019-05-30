using System.Collections.Generic;
using Atdi.Contracts.CoreServices.DataLayer;
using Atdi.Contracts.CoreServices.EntityOrm;
using Atdi.Platform.Logging;
using System;
using MD = Atdi.DataModels.Sdrns.DeviceServer.Entities;
using Atdi.DataModels.EntityOrm;
using Atdi.Modules.Sdrn.DeviceServer;
using System.Xml;
using System.Linq;
using Atdi.DataModels.Sdrn.DeviceServer.Processing;
using Atdi.DataModels.Sdrns.Device;



namespace Atdi.AppUnits.Sdrn.DeviceServer.Repositories
{
    public sealed class MeasTaskRepository : IRepository<MeasTask, int?>
    {
        private readonly IDataLayer<EntityDataOrm> _dataLayer;
        private readonly ILogger _logger;


        public MeasTaskRepository(IDataLayer<EntityDataOrm> dataLayer, ILogger logger)
        {
            this._dataLayer = dataLayer;
            this._logger = logger;
        }


        public int? Create(MeasTask item)
        {
            int? ID = null;
            var queryExecuter = this._dataLayer.Executor<SdrnServerDeviceDataContext>();
            if (item != null)
            {
                try
                {
                    queryExecuter.BeginTransaction();

                    var builderInsertMeasTask = this._dataLayer.GetBuilder<MD.IMeasTask>().Insert();
                    builderInsertMeasTask.SetValue(c => c.EquipmentTechId, item.EquipmentTechId);
                    if ((item.MobEqipmentMeasurements != null) && (item.MobEqipmentMeasurements.Length > 0))
                    {
                        builderInsertMeasTask.SetValue(c => c.MobEqipmentMeasurements, string.Join(",", item.MobEqipmentMeasurements));
                    }
                    builderInsertMeasTask.SetValue(c => c.Priority, item.Priority);
                    builderInsertMeasTask.SetValue(c => c.ScanPerTaskNumber, item.ScanPerTaskNumber);
                    builderInsertMeasTask.SetValue(c => c.SdrnServer, item.SdrnServer);
                    builderInsertMeasTask.SetValue(c => c.SensorName, item.SensorName);
                    builderInsertMeasTask.SetValue(c => c.Status, item.Status);
                    builderInsertMeasTask.SetValue(c => c.TaskId, item.TaskId);
                    builderInsertMeasTask.SetValue(c => c.TimeStart, item.StartTime);
                    builderInsertMeasTask.SetValue(c => c.TimeStop, item.StopTime);
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
                        if (item.ScanParameters != null)
                        {
                            for (int i = 0; i < item.ScanParameters.Length; i++)
                            {
                                var itemScanParameters = item.ScanParameters[i];
                                int? idStandardScanParameter = null;
                                var builderInsertStandardScanParameter = this._dataLayer.GetBuilder<MD.IStandardScanParameter>().Insert();
                                builderInsertStandardScanParameter.SetValue(c => c.DetectLevelDbm, itemScanParameters.DetectionLevel_dBm);
                                if (itemScanParameters.DeviceParam != null)
                                {
                                    var deviceParam = itemScanParameters.DeviceParam;
                                    builderInsertStandardScanParameter.SetValue(c => c.DetectType, deviceParam.DetectType.ToString());
                                    builderInsertStandardScanParameter.SetValue(c => c.MeasTime_sec, deviceParam.MeasTime_sec);
                                    builderInsertStandardScanParameter.SetValue(c => c.Preamplification_dB, deviceParam.Preamplification_dB);
                                    builderInsertStandardScanParameter.SetValue(c => c.RBW_kHz, deviceParam.RBW_kHz);
                                    builderInsertStandardScanParameter.SetValue(c => c.RefLevel_dBm, deviceParam.RefLevel_dBm);
                                    builderInsertStandardScanParameter.SetValue(c => c.RfAttenuation_dB, deviceParam.RfAttenuation_dB);
                                    builderInsertStandardScanParameter.SetValue(c => c.ScanBW_kHz, deviceParam.ScanBW_kHz);
                                    builderInsertStandardScanParameter.SetValue(c => c.VBW_kHz, deviceParam.VBW_kHz);
                                }

                                builderInsertStandardScanParameter.SetValue(c => c.MeasFreqRelative, itemScanParameters.MaxFrequencyRelativeOffset_mk);
                                builderInsertStandardScanParameter.SetValue(c => c.MaxPermissBW, itemScanParameters.MaxPermissionBW_kHz);
                                builderInsertStandardScanParameter.SetValue(c => c.Standard, itemScanParameters.Standard);
                                builderInsertStandardScanParameter.SetValue(c => c.LevelDbm, itemScanParameters.XdBLevel_dB);
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


                        if (item.Stations != null)
                        {
                            for (int i = 0; i < item.Stations.Length; i++)
                            {
                                int? idIStationLicenseInfo = null;
                                var station = item.Stations[i];
                                if (station != null)
                                {

                                    var license = station.License;
                                    var builderInsertStationLicenseInfo = this._dataLayer.GetBuilder<MD.IStationLicenseInfo>().Insert();
                                    builderInsertStationLicenseInfo.SetValue(c => c.IcsmId, license.IcsmId);
                                    builderInsertStationLicenseInfo.SetValue(c => c.CloseDate, license.CloseDate);
                                    builderInsertStationLicenseInfo.SetValue(c => c.EndDate, license.EndDate);
                                    builderInsertStationLicenseInfo.SetValue(c => c.Name, license.Name);
                                    builderInsertStationLicenseInfo.SetValue(c => c.StartDate, license.StartDate);
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
                                    var owner = station.Owner;
                                    var builderInsertIOwnerData = this._dataLayer.GetBuilder<MD.IOwnerData>().Insert();
                                    builderInsertIOwnerData.SetValue(c => c.Address, owner.Address);
                                    builderInsertIOwnerData.SetValue(c => c.CODE, owner.Code);
                                    builderInsertIOwnerData.SetValue(c => c.OKPO, owner.OKPO);
                                    builderInsertIOwnerData.SetValue(c => c.OwnerName, owner.OwnerName);
                                    builderInsertIOwnerData.SetValue(c => c.ZIP, owner.Zip);
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
                                    var site = station.Site;
                                    var builderStationSite = this._dataLayer.GetBuilder<MD.IStationSite>().Insert();
                                    builderStationSite.SetValue(c => c.Address, site.Adress);
                                    builderStationSite.SetValue(c => c.Lat, site.Lat);
                                    builderStationSite.SetValue(c => c.Lon, site.Lon);
                                    builderStationSite.SetValue(c => c.Region, site.Region);
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
                                    builderInsertMeasStation.SetValue(c => c.GlobalSid, station.GlobalSid);
                                    builderInsertMeasStation.SetValue(c => c.MeasTaskId, ID.Value);
                                    builderInsertMeasStation.SetValue(c => c.OwnerGlobalSid, station.OwnerGlobalSid);
                                    builderInsertMeasStation.SetValue(c => c.Standard, station.Standard);
                                    builderInsertMeasStation.SetValue(c => c.StationId, station.StationId);
                                    builderInsertMeasStation.SetValue(c => c.Status, station.Status);
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

                                    var sectors = station.Sectors;

                                    if ((idMeasStation != null) && (idMeasStation > 0) && (sectors != null))
                                    {
                                        for (int j = 0; j < sectors.Length; j++)
                                        {
                                            int? idSector = null;
                                            var builderInsertISector = this._dataLayer.GetBuilder<MD.ISector>().Insert();
                                            builderInsertISector.SetValue(c => c.Agl, sectors[j].AGL);
                                            builderInsertISector.SetValue(c => c.Azimut, sectors[j].Azimuth);
                                            builderInsertISector.SetValue(c => c.Bw, sectors[j].BW_kHz);
                                            builderInsertISector.SetValue(c => c.ClassEmission, sectors[j].ClassEmission);
                                            builderInsertISector.SetValue(c => c.Eirp, sectors[j].EIRP_dBm);
                                            builderInsertISector.SetValue(c => c.SectorId, sectors[j].SectorId);
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

                                            var frequencies = sectors[j].Frequencies;
                                            if ((idSector != null) && (idSector > 0) && (frequencies != null))
                                            {
                                                var queryStatements = new List<IQueryInsertStatement<MD.ISectorFreq>>();
                                                for (int l = 0; l < frequencies.Length; l++)
                                                {
                                                    var builderInsertSectorFreq = this._dataLayer.GetBuilder<MD.ISectorFreq>().Insert();
                                                    builderInsertSectorFreq.SetValue(c => c.ChannelNumber, frequencies[l].ChannelNumber);
                                                    builderInsertSectorFreq.SetValue(c => c.Frequency, (double?)frequencies[l].Frequency_MHz);
                                                    builderInsertSectorFreq.SetValue(c => c.IdSector, idSector);
                                                    builderInsertSectorFreq.SetValue(c => c.PlanId, frequencies[l].PlanId);
                                                    builderInsertSectorFreq.Select(c => c.Id);
                                                    queryStatements.Add(builderInsertSectorFreq);
                                                }
                                                queryExecuter.ExecuteAndFetch(queryStatements.ToArray(), readerSectorFreq =>
                                                {
                                                    return true;
                                                });
                                            }

                                            var sector = sectors[j];
                                            if ((idSector != null) && (idSector > 0) && (sector.BWMask != null))
                                            {
                                                var queryStatements = new List<IQueryInsertStatement<MD.ISectorMaskElement>>();
                                                for (int l = 0; l < sector.BWMask.Length; l++)
                                                {
                                                    var bWMask = sector.BWMask[l];
                                                    var builderInsertSectorMaskElement = this._dataLayer.GetBuilder<MD.ISectorMaskElement>().Insert();
                                                    builderInsertSectorMaskElement.SetValue(c => c.Bw, bWMask.BW_kHz);
                                                    builderInsertSectorMaskElement.SetValue(c => c.Level, bWMask.Level_dB);
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
                    }
                    queryExecuter.CommitTransaction();
                }
                catch (Exception e)
                {
                    queryExecuter.RollbackTransaction();
                    this._logger.Exception(Contexts.ThisComponent, e);
                }
            }
            return ID;
        }

        public bool Update(MeasTask item)
        {
            throw new NotImplementedException();
        }

        public bool Delete(int? id)
        {
            throw new NotImplementedException();
        }
  

        MeasTask IRepository<MeasTask,int?>.LoadObject(int? id)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
           // throw new NotImplementedException();
        }

        public MeasTask[] LoadAllObjects()
        {
            throw new NotImplementedException();
        }

        public MeasTask[] LoadObjectsWithRestrict()
        {
            throw new NotImplementedException();
        }
    }

}
