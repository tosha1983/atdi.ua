using System.Collections.Generic;
using Atdi.Contracts.CoreServices.DataLayer;
using Atdi.Contracts.CoreServices.EntityOrm;
using Atdi.Platform.Logging;
using System;
using MD = Atdi.DataModels.Sdrn.DeviceServer.Entities;
using Atdi.DataModels.EntityOrm;
using Atdi.Modules.Sdrn.DeviceServer;
using System.Xml;
using System.Linq;
using Atdi.DataModels.Sdrn.DeviceServer.Processing;
using Atdi.DataModels.Sdrns.Device;



namespace Atdi.AppUnits.Sdrn.DeviceServer.Repositories
{
    public sealed class MeasTaskRepository : IRepository<MeasTask, long?>
    {
        private readonly IDataLayer<EntityDataOrm> _dataLayer;
        private readonly ILogger _logger;


        public MeasTaskRepository(IDataLayer<EntityDataOrm> dataLayer, ILogger logger)
        {
            this._dataLayer = dataLayer;
            this._logger = logger;
        }


        public long? Create(MeasTask item)
        {
            long? ID = null;
            if (item != null)
            {
                try
                {
                    using (var scope = this._dataLayer.CreateScope<SdrnServerDeviceDataContext>())
                    {
                        scope.BeginTran();

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
                        
                        var measTask_PK = scope.Executor.Execute<MD.IMeasTask_PK>(builderInsertMeasTask);
                        ID = measTask_PK.Id;

                        if ((ID != null) && (ID.Value > 0))
                        {
                            if (item.ScanParameters != null)
                            {
                                for (int i = 0; i < item.ScanParameters.Length; i++)
                                {
                                    var itemScanParameters = item.ScanParameters[i];
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
                                    

                                    var standardScanParameter_PK = scope.Executor.Execute<MD.IStandardScanParameter_PK>(builderInsertStandardScanParameter);
                                }
                            }


                            if (item.Stations != null)
                            {
                                for (int i = 0; i < item.Stations.Length; i++)
                                {
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
                                        

                                        var stationLicenseInfo_PK = scope.Executor.Execute<MD.IStationLicenseInfo_PK>(builderInsertStationLicenseInfo);

                                        var owner = station.Owner;
                                        var builderInsertIOwnerData = this._dataLayer.GetBuilder<MD.IOwnerData>().Insert();
                                        builderInsertIOwnerData.SetValue(c => c.Address, owner.Address);
                                        builderInsertIOwnerData.SetValue(c => c.CODE, owner.Code);
                                        builderInsertIOwnerData.SetValue(c => c.OKPO, owner.OKPO);
                                        builderInsertIOwnerData.SetValue(c => c.OwnerName, owner.OwnerName);
                                        builderInsertIOwnerData.SetValue(c => c.ZIP, owner.Zip);
                                        

                                        var ownerData_PK = scope.Executor.Execute<MD.IOwnerData_PK>(builderInsertIOwnerData);

                                        long? idStationSite = null;
                                        var site = station.Site;
                                        var builderStationSite = this._dataLayer.GetBuilder<MD.IStationSite>().Insert();
                                        builderStationSite.SetValue(c => c.Address, site.Adress);
                                        builderStationSite.SetValue(c => c.Lat, site.Lat);
                                        builderStationSite.SetValue(c => c.Lon, site.Lon);
                                        builderStationSite.SetValue(c => c.Region, site.Region);
                                        

                                        var stationSite_PK = scope.Executor.Execute<MD.IStationSite_PK>(builderStationSite);
                                        idStationSite = stationSite_PK.Id;

                                        long? idMeasStation = null;
                                        var builderInsertMeasStation = this._dataLayer.GetBuilder<MD.IMeasStation>().Insert();
                                        builderInsertMeasStation.SetValue(c => c.GlobalSid, station.GlobalSid);
                                        builderInsertMeasStation.SetValue(c => c.MeasTaskId, ID.Value);
                                        builderInsertMeasStation.SetValue(c => c.OwnerGlobalSid, station.OwnerGlobalSid);
                                        builderInsertMeasStation.SetValue(c => c.Standard, station.Standard);
                                        builderInsertMeasStation.SetValue(c => c.StationId, station.StationId);
                                        builderInsertMeasStation.SetValue(c => c.Status, station.Status);
                                        builderInsertMeasStation.SetValue(c => c.LicenceId, stationLicenseInfo_PK.Id);
                                        builderInsertMeasStation.SetValue(c => c.OwnerId, ownerData_PK.Id);
                                        builderInsertMeasStation.SetValue(c => c.SiteId, idStationSite);
                                        
                                        var measStation_PK = scope.Executor.Execute<MD.IMeasStation_PK>(builderInsertMeasStation);
                                        idMeasStation = measStation_PK.Id;

                                        var sectors = station.Sectors;

                                        if ((idMeasStation != null) && (idMeasStation > 0) && (sectors != null))
                                        {
                                            for (int j = 0; j < sectors.Length; j++)
                                            {
                                                long? idSector = null;
                                                var builderInsertISector = this._dataLayer.GetBuilder<MD.ISector>().Insert();
                                                builderInsertISector.SetValue(c => c.Agl, sectors[j].AGL);
                                                builderInsertISector.SetValue(c => c.Azimut, sectors[j].Azimuth);
                                                builderInsertISector.SetValue(c => c.Bw, sectors[j].BW_kHz);
                                                builderInsertISector.SetValue(c => c.ClassEmission, sectors[j].ClassEmission);
                                                builderInsertISector.SetValue(c => c.Eirp, sectors[j].EIRP_dBm);
                                                builderInsertISector.SetValue(c => c.SectorId, sectors[j].SectorId);
                                                builderInsertISector.SetValue(c => c.StationId, idMeasStation);
                                                
                                                var sector_PK = scope.Executor.Execute<MD.ISector_PK>(builderInsertISector);
                                                idSector = sector_PK.Id;

                                                var frequencies = sectors[j].Frequencies;
                                                if ((idSector != null) && (idSector > 0) && (frequencies != null))
                                                {
                                                    for (int l = 0; l < frequencies.Length; l++)
                                                    {
                                                        var builderInsertSectorFreq = this._dataLayer.GetBuilder<MD.ISectorFreq>().Insert();
                                                        builderInsertSectorFreq.SetValue(c => c.ChannelNumber, frequencies[l].ChannelNumber);
                                                        builderInsertSectorFreq.SetValue(c => c.Frequency, (double?)frequencies[l].Frequency_MHz);
                                                        builderInsertSectorFreq.SetValue(c => c.IdSector, idSector);
                                                        builderInsertSectorFreq.SetValue(c => c.PlanId, frequencies[l].PlanId);
                                                        
                                                        scope.Executor.Execute(builderInsertSectorFreq);
                                                    }
                                                }

                                                var sector = sectors[j];
                                                if ((idSector != null) && (idSector > 0) && (sector.BWMask != null))
                                                {
                                                    for (int l = 0; l < sector.BWMask.Length; l++)
                                                    {
                                                        var bWMask = sector.BWMask[l];
                                                        var builderInsertSectorMaskElement = this._dataLayer.GetBuilder<MD.ISectorMaskElement>().Insert();
                                                        builderInsertSectorMaskElement.SetValue(c => c.Bw, bWMask.BW_kHz);
                                                        builderInsertSectorMaskElement.SetValue(c => c.Level, bWMask.Level_dB);
                                                        builderInsertSectorMaskElement.SetValue(c => c.IdSector, idSector);
                                                        
                                                        scope.Executor.Execute(builderInsertSectorMaskElement);
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        scope.Commit();
                    }
                }
                catch (Exception e)
                {
                    this._logger.Exception(Contexts.ThisComponent, e);
                }
            }
            return ID;
        }

        public bool Update(MeasTask item)
        {
            throw new NotImplementedException();
        }

        public bool Delete(long? id)
        {
            throw new NotImplementedException();
        }
  

        MeasTask IRepository<MeasTask, long?>.LoadObject(long? id)
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

        public int GetCountObjectsWithRestrict()
        {
            throw new NotImplementedException();
        }

        public Dictionary<string, string> GetDictionaryStatusObjects()
        {
            throw new NotImplementedException();
        }
    }

}
