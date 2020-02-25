using System.Collections.Generic;
using Atdi.Contracts.Api.EventSystem;
using Atdi.Contracts.CoreServices.DataLayer;
using Atdi.Contracts.CoreServices.EntityOrm;
using Atdi.Contracts.Sdrn.Server;
using Atdi.DataModels.DataConstraint;
using Atdi.Platform.Logging;
using System;
using MD = Atdi.DataModels.Sdrns.Server.Entities;
using Atdi.Contracts.WcfServices.Sdrn.Server;
using System.Xml;
using System.Linq;
using Atdi.Common;
using Calculation = Atdi.Modules.Sdrn.Calculation;


namespace Atdi.WcfServices.Sdrn.Server
{
    public class RunSynchroProcess
    {
        private readonly IDataLayer<EntityDataOrm> _dataLayer;
        private readonly ILogger _logger;



        public RunSynchroProcess(IDataLayer<EntityDataOrm> dataLayer, ILogger logger)
        {
            this._dataLayer = dataLayer;
            this._logger = logger;
        }

        public bool SynchroAreas(Area[] areas)
        {
            bool isSuccess = false;
            try
            {
                this._logger.Info(Contexts.ThisComponent, Categories.ImportData, Events.HandlerSynchroAreasMethod.Text);
                using (var scope = this._dataLayer.CreateScope<SdrnServerDataContext>())
                {
                    scope.BeginTran();
                    for (int i = 0; i < areas.Length; i++)
                    {
                        bool isFindIdentifierFromICSM = false;
                        var queryAreaFrom = this._dataLayer.GetBuilder<MD.IArea>()
                        .From()
                        .Select(c => c.Id, c => c.CreatedBy, c => c.CreatedDate, c => c.IdentifierFromICSM, c => c.Name, c => c.TypeOfArea)
                        .Where(c => c.Id, ConditionOperator.GreaterThan, 0);
                        scope.Executor.Fetch(queryAreaFrom, readerAreaFrom =>
                        {
                            while (readerAreaFrom.Read())
                            {
                                if (readerAreaFrom.GetValue(c => c.IdentifierFromICSM) == areas[i].IdentifierFromICSM)
                                {
                                    isFindIdentifierFromICSM = true;

                                    var builderUpdateArea = this._dataLayer.GetBuilder<MD.IArea>().Update();
                                    builderUpdateArea.Where(c => c.IdentifierFromICSM, ConditionOperator.Equal, areas[i].IdentifierFromICSM);

                                    if (readerAreaFrom.GetValue(c => c.CreatedBy) != areas[i].CreatedBy)
                                    {
                                        builderUpdateArea.SetValue(c => c.CreatedBy, areas[i].CreatedBy);
                                    }
                                    if (readerAreaFrom.GetValue(c => c.CreatedDate) != areas[i].DateCreated)
                                    {
                                        builderUpdateArea.SetValue(c => c.CreatedDate, areas[i].DateCreated);
                                    }

                                    if (readerAreaFrom.GetValue(c => c.Name) != areas[i].Name)
                                    {
                                        builderUpdateArea.SetValue(c => c.Name, areas[i].Name);
                                    }

                                    if (readerAreaFrom.GetValue(c => c.TypeOfArea) != areas[i].TypeArea)
                                    {
                                        builderUpdateArea.SetValue(c => c.TypeOfArea, areas[i].TypeArea);
                                    }

                                    scope.Executor.Execute(builderUpdateArea);


                                    for (int j = 0; j < areas[i].Location.Length; j++)
                                    {
                                        var location = areas[i].Location[j];

                                        bool isFindLocation = false;
                                        var queryAreaLocation = this._dataLayer.GetBuilder<MD.IAreaLocation>()
                                        .From()
                                        .Select(c => c.Id, c => c.Latitude, c => c.Longitude, c => c.AREA.Id, c => c.AREA.IdentifierFromICSM)
                                        .Where(c => c.AREA.Id, ConditionOperator.Equal, readerAreaFrom.GetValue(c => c.Id));
                                        scope.Executor.Fetch(queryAreaLocation, readerAreaLocation =>
                                        {
                                            while (readerAreaLocation.Read())
                                            {
                                                if (Math.Abs(readerAreaLocation.GetValue(c => c.Latitude) - location.Latitude) < 0.000001 && Math.Abs(readerAreaLocation.GetValue(c => c.Longitude) - location.Longitude) < 0.000001)
                                                {
                                                    isFindLocation = true;
                                                    break;
                                                }
                                            }
                                            return true;
                                        });
                                        // Если заданніе координаты в таблице не найдены, тогда добавляем их
                                        if (isFindLocation == false)
                                        {
                                            var builderAreaLocation = this._dataLayer.GetBuilder<MD.IAreaLocation>().Insert();
                                            builderAreaLocation.SetValue(c => c.Latitude, location.Latitude);
                                            builderAreaLocation.SetValue(c => c.Longitude, location.Longitude);
                                            builderAreaLocation.SetValue(c => c.AREA.Id, readerAreaFrom.GetValue(c => c.Id));
                                            scope.Executor.Execute<MD.IAreaLocation_PK>(builderAreaLocation);
                                        }

                                    }

                                    var forDeleteIdsLocation = new List<long>();
                                    var listLocations = areas[i].Location.ToList();
                                    var queryAreaLocationForDelete = this._dataLayer.GetBuilder<MD.IAreaLocation>()
                                    .From()
                                    .Select(c => c.Id, c => c.Latitude, c => c.Longitude, c => c.AREA.Id, c => c.AREA.IdentifierFromICSM)
                                    .Where(c => c.AREA.Id, ConditionOperator.Equal, readerAreaFrom.GetValue(c => c.Id));
                                    scope.Executor.Fetch(queryAreaLocationForDelete, readerAreaLocation =>
                                    {
                                        while (readerAreaLocation.Read())
                                        {
                                            var findLocation = listLocations.Find(x => Math.Abs(x.Latitude - readerAreaLocation.GetValue(c => c.Latitude))<0.000001 && (x.Longitude - readerAreaLocation.GetValue(c => c.Longitude))< 0.000001);
                                            if (findLocation == null)
                                            {
                                                forDeleteIdsLocation.Add(readerAreaLocation.GetValue(c => c.Id));
                                            }
                                        }
                                        return true;
                                    });

                                    for (int k = 0; k < forDeleteIdsLocation.Count; k++)
                                    {
                                        var builderDeleteAreaLocation = this._dataLayer.GetBuilder<MD.IAreaLocation>().Delete();
                                        builderDeleteAreaLocation.Where(c => c.Id, ConditionOperator.Equal, forDeleteIdsLocation[k]);
                                        scope.Executor.Execute(builderDeleteAreaLocation);
                                    }


                                    break;
                                }
                            }
                            return true;
                        });


                        if (isFindIdentifierFromICSM == false)
                        {

                            var builderAreaInsert = this._dataLayer.GetBuilder<MD.IArea>().Insert();
                            builderAreaInsert.SetValue(c => c.CreatedBy, areas[i].CreatedBy);
                            builderAreaInsert.SetValue(c => c.CreatedDate, areas[i].DateCreated);
                            builderAreaInsert.SetValue(c => c.IdentifierFromICSM, areas[i].IdentifierFromICSM);
                            builderAreaInsert.SetValue(c => c.Name, areas[i].Name);
                            builderAreaInsert.SetValue(c => c.TypeOfArea, areas[i].TypeArea);
                            var areaId = scope.Executor.Execute<MD.IArea_PK>(builderAreaInsert);
                            if (areaId != null)
                            {
                                for (int k = 0; k < areas[i].Location.Length; k++)
                                {
                                    var loc = areas[i].Location[k];
                                    var builderInsAreaLocation = this._dataLayer.GetBuilder<MD.IAreaLocation>().Insert();
                                    builderInsAreaLocation.SetValue(c => c.Latitude, loc.Latitude);
                                    builderInsAreaLocation.SetValue(c => c.Longitude, loc.Longitude);
                                    builderInsAreaLocation.SetValue(c => c.AREA.Id, areaId.Id);
                                    scope.Executor.Execute<MD.IAreaLocation_PK>(builderInsAreaLocation);
                                }
                            }
                        }
                    }
                    scope.Commit();
                }
                isSuccess = true;
            }
            catch (Exception e)
            {
                isSuccess = false;
                this._logger.Exception(Contexts.ThisComponent, e);
            }
            return isSuccess;
        }

        public bool SynchroStationsExtended(StationExtended[] stationsExtended)
        {
            bool isSuccess = false;
            try
            {
                this._logger.Info(Contexts.ThisComponent, Categories.ImportData, Events.HandlerSynchroStationsExtendedMethod.Text);
                using (var scope = this._dataLayer.CreateScope<SdrnServerDataContext>())
                {
                    scope.BeginTran();
                    for (int i = 0; i < stationsExtended.Length; i++)
                    {
                        bool isFindIdentifierFromICSM = false;
                        var queryStationExtendedFrom = this._dataLayer.GetBuilder<MD.IStationExtended>()
                        .From()
                        .Select(c => c.Id, c => c.Address, c => c.BandWidth, c => c.DesigEmission, c => c.Latitude, c => c.Longitude, c => c.OwnerName, c => c.PermissionNumber, c => c.PermissionStart, c => c.PermissionStop, c => c.Province, c => c.Standard, c => c.StandardName, c => c.TableId, c => c.TableName)
                        .Where(c => c.Id, ConditionOperator.GreaterThan, 0);
                        scope.Executor.Fetch(queryStationExtendedFrom, readerStationExtendedFrom =>
                        {
                            while (readerStationExtendedFrom.Read())
                            {
                                if ((readerStationExtendedFrom.GetValue(c => c.TableId) == stationsExtended[i].TableId) &&
                                    (readerStationExtendedFrom.GetValue(c => c.TableName) == stationsExtended[i].TableName))
                                {
                                    isFindIdentifierFromICSM = true;

                                    stationsExtended[i].Id = readerStationExtendedFrom.GetValue(c => c.Id);

                                    var builderUpdateStationExtended = this._dataLayer.GetBuilder<MD.IStationExtended>().Update();
                                    builderUpdateStationExtended.Where(c => c.TableId, ConditionOperator.Equal, stationsExtended[i].TableId);
                                    builderUpdateStationExtended.Where(c => c.TableName, ConditionOperator.Equal, stationsExtended[i].TableName);

                                    if (readerStationExtendedFrom.GetValue(c => c.Address) != stationsExtended[i].Address)
                                    {
                                        builderUpdateStationExtended.SetValue(c => c.Address, stationsExtended[i].Address);
                                    }

                                    if (readerStationExtendedFrom.GetValue(c => c.BandWidth) != stationsExtended[i].BandWidth)
                                    {
                                        builderUpdateStationExtended.SetValue(c => c.BandWidth, stationsExtended[i].BandWidth);
                                    }

                                    if (readerStationExtendedFrom.GetValue(c => c.DesigEmission) != stationsExtended[i].DesigEmission)
                                    {
                                        builderUpdateStationExtended.SetValue(c => c.DesigEmission, stationsExtended[i].DesigEmission);
                                    }

                                    if (readerStationExtendedFrom.GetValue(c => c.Latitude) != stationsExtended[i].Location.Latitude)
                                    {
                                        builderUpdateStationExtended.SetValue(c => c.Latitude, stationsExtended[i].Location.Latitude);
                                    }

                                    if (readerStationExtendedFrom.GetValue(c => c.Longitude) != stationsExtended[i].Location.Longitude)
                                    {
                                        builderUpdateStationExtended.SetValue(c => c.Longitude, stationsExtended[i].Location.Longitude);
                                    }

                                    if (readerStationExtendedFrom.GetValue(c => c.OwnerName) != stationsExtended[i].OwnerName)
                                    {
                                        builderUpdateStationExtended.SetValue(c => c.OwnerName, stationsExtended[i].OwnerName);
                                    }

                                    if (readerStationExtendedFrom.GetValue(c => c.PermissionNumber) != stationsExtended[i].PermissionNumber)
                                    {
                                        builderUpdateStationExtended.SetValue(c => c.PermissionNumber, stationsExtended[i].PermissionNumber);
                                    }

                                    if (readerStationExtendedFrom.GetValue(c => c.PermissionStart) != stationsExtended[i].PermissionStart)
                                    {
                                        builderUpdateStationExtended.SetValue(c => c.PermissionStart, stationsExtended[i].PermissionStart);
                                    }

                                    if (readerStationExtendedFrom.GetValue(c => c.PermissionStop) != stationsExtended[i].PermissionStop)
                                    {
                                        builderUpdateStationExtended.SetValue(c => c.PermissionStop, stationsExtended[i].PermissionStop);
                                    }

                                    if (readerStationExtendedFrom.GetValue(c => c.Province) != stationsExtended[i].Province)
                                    {
                                        builderUpdateStationExtended.SetValue(c => c.Province, stationsExtended[i].Province);
                                    }

                                    scope.Executor.Execute(builderUpdateStationExtended);

                                    break;
                                }
                            }
                            return true;
                        });


                        if (isFindIdentifierFromICSM == false)
                        {

                            var builderStationExtendedInsert = this._dataLayer.GetBuilder<MD.IStationExtended>().Insert();
                            builderStationExtendedInsert.SetValue(c => c.Province, stationsExtended[i].Province);
                            builderStationExtendedInsert.SetValue(c => c.Address, stationsExtended[i].Address);
                            builderStationExtendedInsert.SetValue(c => c.BandWidth, stationsExtended[i].BandWidth);
                            builderStationExtendedInsert.SetValue(c => c.DesigEmission, stationsExtended[i].DesigEmission);
                            builderStationExtendedInsert.SetValue(c => c.Latitude, stationsExtended[i].Location.Latitude);
                            builderStationExtendedInsert.SetValue(c => c.Longitude, stationsExtended[i].Location.Longitude);
                            builderStationExtendedInsert.SetValue(c => c.OwnerName, stationsExtended[i].OwnerName);
                            builderStationExtendedInsert.SetValue(c => c.PermissionNumber, stationsExtended[i].PermissionNumber);
                            builderStationExtendedInsert.SetValue(c => c.PermissionStart, stationsExtended[i].PermissionStart);
                            builderStationExtendedInsert.SetValue(c => c.PermissionStop, stationsExtended[i].PermissionStop);
                            builderStationExtendedInsert.SetValue(c => c.Standard, stationsExtended[i].Standard);
                            builderStationExtendedInsert.SetValue(c => c.StandardName, stationsExtended[i].StandardName);
                            builderStationExtendedInsert.SetValue(c => c.TableId, stationsExtended[i].TableId);
                            builderStationExtendedInsert.SetValue(c => c.TableName, stationsExtended[i].TableName);
                            var stationExtendedId = scope.Executor.Execute<MD.IStationExtended_PK>(builderStationExtendedInsert);
                        }
                    }
                    scope.Commit();
                }
                isSuccess = true;
            }
            catch (Exception e)
            {
                isSuccess = false;
                this._logger.Exception(Contexts.ThisComponent, e);
            }
            return isSuccess;
        }

        public long? CreateDataSynchronizationProcess(DataSynchronizationBase dataSynchronization, long[] headRefSpectrumIdsBySDRN, long[] sensorIdsBySDRN, Area[] areas)
        {
            long? synchroProcessInsertId = null;
            try
            {
                this._logger.Info(Contexts.ThisComponent, Categories.Processing, Events.HandlerCreateDataSynchronizationProcessMethod.Text);
                using (var scope = this._dataLayer.CreateScope<SdrnServerDataContext>())
                {
                    scope.BeginTran();


                    var builderSynchroProcessInsert = this._dataLayer.GetBuilder<MD.ISynchroProcess>().Insert();
                    builderSynchroProcessInsert.SetValue(c => c.DateStart, dataSynchronization.DateStart);
                    builderSynchroProcessInsert.SetValue(c => c.DateEnd, dataSynchronization.DateEnd);
                    builderSynchroProcessInsert.SetValue(c => c.CreatedBy, dataSynchronization.CreatedBy);
                    builderSynchroProcessInsert.SetValue(c => c.CreatedDate, dataSynchronization.DateCreated);
                    builderSynchroProcessInsert.SetValue(c => c.Status, Status.A.ToString());

                    var synchroProcessIdValue = scope.Executor.Execute<MD.ISynchroProcess_PK>(builderSynchroProcessInsert);
                    synchroProcessInsertId = synchroProcessIdValue.Id;

                    for (int k = 0; k < headRefSpectrumIdsBySDRN.Length; k++)
                    {
                        var builderLinkHeadRefSpectrumInsert = this._dataLayer.GetBuilder<MD.ILinkHeadRefSpectrum>().Insert();
                        builderLinkHeadRefSpectrumInsert.SetValue(c => c.HEAD_REF_SPECTRUM.Id, headRefSpectrumIdsBySDRN[k]);
                        builderLinkHeadRefSpectrumInsert.SetValue(c => c.SYNCHRO_PROCESS.Id, synchroProcessInsertId);
                        var builderLinkHeadRefSpectrumInsertIdValue = scope.Executor.Execute<MD.ILinkHeadRefSpectrum_PK>(builderLinkHeadRefSpectrumInsert);
                    }

                    for (int k = 0; k < sensorIdsBySDRN.Length; k++)
                    {
                        var builderLinkSensorsWithSynchroProcessInsert = this._dataLayer.GetBuilder<MD.ILinkSensorsWithSynchroProcess>().Insert();
                        builderLinkSensorsWithSynchroProcessInsert.SetValue(c => c.SensorId, sensorIdsBySDRN[k]);
                        builderLinkSensorsWithSynchroProcessInsert.SetValue(c => c.SYNCHRO_PROCESS.Id, synchroProcessInsertId);
                        var builderLinkSensorsWithSynchroProcessInsertIdValue = scope.Executor.Execute<MD.ILinkSensorsWithSynchroProcess_PK>(builderLinkSensorsWithSynchroProcessInsert);
                    }

                    for (int k = 0; k < areas.Length; k++)
                    {
                        var builderLinkAreaInsert = this._dataLayer.GetBuilder<MD.ILinkArea>().Insert();
                        builderLinkAreaInsert.SetValue(c => c.AREA.Id, GetAreaIdByICSMId(areas[k].IdentifierFromICSM));
                        builderLinkAreaInsert.SetValue(c => c.SYNCHRO_PROCESS.Id, synchroProcessInsertId);
                        var builderLinkAreaInsertIdValue = scope.Executor.Execute<MD.ILinkArea_PK>(builderLinkAreaInsert);
                    }

                    scope.Commit();
                }
            }
            catch (Exception e)
            {
                this._logger.Exception(Contexts.ThisComponent, e);
            }
            return synchroProcessInsertId;
        }

        public long? GetAreaIdByICSMId(int IdentifierFromICSM)
        {
            long? areaId = null;
            try
            {
                var queryExecuter = this._dataLayer.Executor<SdrnServerDataContext>();
                var queryLinkAreaFrom = this._dataLayer.GetBuilder<MD.IArea>()
                .From()
                .Select(c => c.Id, c => c.IdentifierFromICSM)
                .Where(c => c.IdentifierFromICSM, ConditionOperator.Equal, IdentifierFromICSM);
                queryExecuter.Fetch(queryLinkAreaFrom, readerLinkAreaFrom =>
                {
                    while (readerLinkAreaFrom.Read())
                    {
                        areaId = readerLinkAreaFrom.GetValue(c => c.Id);
                        break;
                    }
                    return true;
                });
            }
            catch (Exception e)
            {
                this._logger.Exception(Contexts.ThisComponent, e);
            }
            return areaId;
        }
        public List<DataLocation[]> GetPolygonFromArea(Area[] areas)
        {
            var queryExecuter = this._dataLayer.Executor<SdrnServerDataContext>();
            var polygonAll = new List<DataLocation[]>();
            var polygon = new List<DataLocation>();
            for (int i = 0; i < areas.Length; i++)
            {
                var queryAreaLocationFrom = this._dataLayer.GetBuilder<MD.IAreaLocation>()
                .From()
                .Select(c => c.Id, c => c.Latitude, c => c.Longitude, c => c.AREA.IdentifierFromICSM, c => c.AREA.Id)
                .Where(c => c.AREA.Id, ConditionOperator.Equal, GetAreaIdByICSMId(areas[i].IdentifierFromICSM));
                queryExecuter.Fetch(queryAreaLocationFrom, readerAreaLocationFrom =>
                {
                    while (readerAreaLocationFrom.Read())
                    {

                        polygon.Add(new DataLocation()
                        {
                            Longitude = readerAreaLocationFrom.GetValue(c => c.Longitude),
                            Latitude = readerAreaLocationFrom.GetValue(c => c.Latitude)
                        });
                    }
                    return true;
                });
                polygonAll.Add(polygon.ToArray());
            }
            return polygonAll;
        }

        public bool DeleteDuplicateRefSpectrumRecords(DataSynchronizationBase dataSynchronization, long[] headRefSpectrumIdsBySDRN, long[] sensorIdsBySDRN, Area[] areas, StationExtended[] stationsExtended)
        {
            bool isSuccess = false;
            try
            {
                var listDataRefSpectrum = new List<DataRefSpectrum>();
                var listDataRefSpectrumForDelete = new List<DataRefSpectrum>();
                var loadSynchroProcessData = new LoadSynchroProcessData(this._dataLayer, this._logger);
                var listRefSpectrum = loadSynchroProcessData.GetRefSpectrumByIds(headRefSpectrumIdsBySDRN, sensorIdsBySDRN);
            
                // Заполняем список listDataRefSpectrum перечнем всех DataRefSpectrum
                for (int i = 0; i < listRefSpectrum.Length; i++)
                {
                    var refSpectrum = listRefSpectrum[i];
                    if (refSpectrum.DataRefSpectrum != null)
                    {
                        for (int h = 0; h < refSpectrum.DataRefSpectrum.Length; h++)
                        {
                            listDataRefSpectrum.Add(refSpectrum.DataRefSpectrum[h]);
                        }
                    }
                }

                // удаляются записи, которые повторяться (т.е. одинаковые параметры Table ICSM Name, ID ICSM, ID Sensor, Global SID, Freq MHz) при этом остаётся только последняя по времени запись.
                for (int h = 0; h < listDataRefSpectrum.Count; h++)
                {
                    var findData = listDataRefSpectrum.FindAll(x => x.TableId == listDataRefSpectrum[h].TableId && x.TableName == listDataRefSpectrum[h].TableName && x.SensorId == listDataRefSpectrum[h].SensorId && x.GlobalSID == listDataRefSpectrum[h].GlobalSID && x.Freq_MHz == listDataRefSpectrum[h].Freq_MHz);
                    if (findData != null)
                    {
                        var orderByDateMeas = from z in findData orderby z.DateMeas descending select z;
                        if ((orderByDateMeas != null) && (orderByDateMeas.Count() > 1))
                        {
                            var orderByDateMeasArray = orderByDateMeas.ToArray();
                            for (int g = 1; g < orderByDateMeasArray.Length; g++)
                            {
                                listDataRefSpectrumForDelete.Add(orderByDateMeasArray[g]);
                            }
                        }
                    }
                }

                // Записи, дата которых (Date Meas) находиться за пределами даты начала отчета, даты конца отчета удаляются 
                var findDataRep = listDataRefSpectrum.FindAll(x => (x.DateMeas >= dataSynchronization.DateStart && x.DateMeas <= dataSynchronization.DateEnd) == false);
                if (findDataRep != null)
                {
                    if ((findDataRep != null) && (findDataRep.Count>0))
                    {
                        listDataRefSpectrumForDelete.AddRange(findDataRep);
                    }
                }

                // Записи где станция находиться за пределами контура удаляються тоже 
                var lstPolygons = GetPolygonFromArea(areas);
                var lstStationsExtended = stationsExtended.ToList();
                for (int h = 0; h < listDataRefSpectrum.Count; h++)
                {
                    var fndStation = lstStationsExtended.Find(z => z.TableId == listDataRefSpectrum[h].TableId && z.TableName == listDataRefSpectrum[h].TableName);
                    if (fndStation != null)
                    {
                        for (int f = 0; f < lstPolygons.Count; f++)
                        {
                            if ((lstPolygons[f] != null) && (lstPolygons[f].Length>0))
                            {
                                var checkLocation = new CheckLocation(fndStation.Location.Longitude, fndStation.Location.Latitude);
                                var listPolyg = lstPolygons[f].ToList();
                                if (!checkLocation.CheckHitting(listPolyg))
                                {
                                    var fnd = listDataRefSpectrumForDelete.Find(z => z.TableId == listDataRefSpectrum[h].TableId && z.TableName == listDataRefSpectrum[h].TableName);
                                    if (fnd == null)
                                    {
                                        listDataRefSpectrumForDelete.Add(listDataRefSpectrum[h]);
                                    }
                                }
                            }
                        }
                    }
                }

                 // непосредственное удаление записей из БД
                var arr = listDataRefSpectrumForDelete.ToArray();
                using (var scope = this._dataLayer.CreateScope<SdrnServerDataContext>())
                {
                    scope.BeginTran();
                    for (int h = 0; h < arr.Length; h++)
                    {
                        var builderDeleteRefSpectrum = this._dataLayer.GetBuilder<MD.IRefSpectrum>().Delete();
                        builderDeleteRefSpectrum.Where(c => c.Id, ConditionOperator.Equal, arr[h].Id);
                        scope.Executor.Execute(builderDeleteRefSpectrum);
                    }
                    scope.Commit();
                }
                isSuccess = true;
            }
            catch (Exception e)
            {
                isSuccess = false;
                this._logger.Exception(Contexts.ThisComponent, e);
            }
            return isSuccess;
        }

        public GroupSensors[] GetGroupSensors(RefSpectrum[] refSpectrums)
        {
            var dataGroupSensors = new List<GroupSensors>();
            for (int i=0; i< refSpectrums.Length; i++)
            {
                for (int j = 0; j < refSpectrums[i].DataRefSpectrum.Length; j++)
                {
                    var dataRefSpectrum = refSpectrums[i].DataRefSpectrum[j];
                    var fndValue = dataGroupSensors.Find(v => v.SensorId == dataRefSpectrum.SensorId && v.Freq_MHz == dataRefSpectrum.Freq_MHz);
                    if (fndValue==null)
                    {
                        dataGroupSensors.Add(new GroupSensors()
                        {
                             SensorId = dataRefSpectrum.SensorId,
                             Freq_MHz = dataRefSpectrum.Freq_MHz
                        });
                    }
                }
            }
            return dataGroupSensors.ToArray();
        }


        public RefSpectrum[] SelectGroupSensor(GroupSensors group, RefSpectrum[] refSpectrums)
        {
            var listDataRefSpectrum = new List<RefSpectrum>();
            for (int i = 0; i < refSpectrums.Length; i++)
            {
                var RefSpectrum = new RefSpectrum();
                RefSpectrum.CreatedBy = refSpectrums[i].CreatedBy;
                RefSpectrum.DateCreated = refSpectrums[i].DateCreated;
                RefSpectrum.FileName = refSpectrums[i].FileName;
                RefSpectrum.Id = refSpectrums[i].Id;
                var lstDataRefSpectrums = new List<DataRefSpectrum>();
                for (int j = 0; j < refSpectrums[i].DataRefSpectrum.Length; j++)
                {
                    var dataRefSpectrum = refSpectrums[i].DataRefSpectrum[j];
                    if ((dataRefSpectrum.SensorId==group.SensorId) && (dataRefSpectrum.Freq_MHz == group.Freq_MHz))
                    {
                        lstDataRefSpectrums.Add(dataRefSpectrum);
                    }
                }
                RefSpectrum.DataRefSpectrum = lstDataRefSpectrums.ToArray();
                listDataRefSpectrum.Add(RefSpectrum);
            }
            return listDataRefSpectrum.ToArray();
        }

        /// <summary>
        /// Синхронизация излучений с записями группы сенсора
        /// </summary>
        /// <param name="refSpectrums">массив RefSpectrum, который соответствует группе сенсора groupSensor</param>
        /// <param name="emittings">массив Emitting, соответствующий  группе сенсора groupSensor</param>
        /// <returns></returns>
        public Protocols[] SynchroEmittings(RefSpectrum[] refSpectrums, Emitting[] emittings)
        {
            var lstProtocols = new List<Protocols>();
           
            // Ниже приведен пример цикла, в котором идет последовательная обработка записей RefSpectrum
            
            for (int i = 0; i < refSpectrums.Length; i++)
            {
                var RefSpectrum = new RefSpectrum();
                for (int j = 0; j < refSpectrums[i].DataRefSpectrum.Length; j++)
                {
                    // получение очередного значения dataRefSpectrum
                    var dataRefSpectrum = refSpectrums[i].DataRefSpectrum[j];

                    // обявляем очередной экземпляр переменной Protocols
                    var protocol = new Protocols();
                    protocol.DataRefSpectrum = new DataRefSpectrum();
                    // копируем данные с переменной dataRefSpectrum
                    protocol.DataRefSpectrum = dataRefSpectrum;

                    
                    
                    // объявление новой промежуточной переменной, которая должна заполняться в блоке ниже:
                    var calculatedEmitting = new Emitting();
                    /////////////////////////////////////////////////////////////////////////////////////////////////////////
                    /////
                    /////
                    /////         ЗДЕСЬ НЕКОТОРЫЙ АЛГОРИТМ ОПРЕДЕЛЕНИЯ EMITTING: 
                    /////         нужно заполнить поля переменной CalculatedEmitting, которая затем будет использована для формирования очередной записи ProtocolsWithEmittings (ниже)
                    /////
                    /////////////////////////////////////////////////////////////////////////////////////////////////////////


                    var emit = emittings.ToList();
                    var  fndEmit = emit.Find(x => x.StartFrequency_MHz <= dataRefSpectrum.Freq_MHz && x.StopFrequency_MHz >= dataRefSpectrum.Freq_MHz);
                    if (fndEmit != null)
                    {
                        calculatedEmitting = fndEmit;

                        // если связь с Emitting обнаружена - тогда необходимо заполнить свосйтво ProtocolsLinkedWithEmittings:
                        protocol.ProtocolsLinkedWithEmittings = new ProtocolsWithEmittings();
                        protocol.ProtocolsLinkedWithEmittings.Count = calculatedEmitting.LevelsDistribution.Count;
                        protocol.ProtocolsLinkedWithEmittings.CurentPower_dBm = calculatedEmitting.CurentPower_dBm;

                        protocol.ProtocolsLinkedWithEmittings.Levels = calculatedEmitting.LevelsDistribution.Levels;

                        if (calculatedEmitting.SignalMask != null)
                        {
                            protocol.ProtocolsLinkedWithEmittings.Freq_kHz = calculatedEmitting.SignalMask.Freq_kHz;
                            protocol.ProtocolsLinkedWithEmittings.Loss_dB = calculatedEmitting.SignalMask.Loss_dB;
                        }
                        protocol.ProtocolsLinkedWithEmittings.MeanDeviationFromReference = calculatedEmitting.MeanDeviationFromReference;
                        //protocol.ProtocolsLinkedWithEmittings.Probability = здесь заполнить вероятность;
                        protocol.ProtocolsLinkedWithEmittings.ReferenceLevel_dBm = calculatedEmitting.ReferenceLevel_dBm;
                        if (calculatedEmitting.EmittingParameters != null)
                        {
                            protocol.ProtocolsLinkedWithEmittings.RollOffFactor = calculatedEmitting.EmittingParameters.RollOffFactor;
                            protocol.ProtocolsLinkedWithEmittings.StandardBW = calculatedEmitting.EmittingParameters.StandardBW;
                        }
                        if (calculatedEmitting.Spectrum != null)
                        {
                            protocol.ProtocolsLinkedWithEmittings.SignalLevel_dBm = calculatedEmitting.Spectrum.SignalLevel_dBm;
                            protocol.ProtocolsLinkedWithEmittings.SpectrumStartFreq_MHz = calculatedEmitting.Spectrum.SpectrumStartFreq_MHz;
                            protocol.ProtocolsLinkedWithEmittings.SpectrumSteps_kHz = calculatedEmitting.Spectrum.SpectrumSteps_kHz;
                            protocol.ProtocolsLinkedWithEmittings.T1 = calculatedEmitting.Spectrum.T1;
                            protocol.ProtocolsLinkedWithEmittings.T2 = calculatedEmitting.Spectrum.T2;
                            protocol.ProtocolsLinkedWithEmittings.TraceCount = calculatedEmitting.Spectrum.TraceCount;
                            protocol.ProtocolsLinkedWithEmittings.MarkerIndex = calculatedEmitting.Spectrum.MarkerIndex;
                            protocol.ProtocolsLinkedWithEmittings.Levels_dBm = calculatedEmitting.Spectrum.Levels_dBm;
                            protocol.ProtocolsLinkedWithEmittings.Bandwidth_kHz = calculatedEmitting.Spectrum.Bandwidth_kHz;
                            protocol.ProtocolsLinkedWithEmittings.Contravention = calculatedEmitting.Spectrum.Contravention;
                            protocol.ProtocolsLinkedWithEmittings.CorrectnessEstimations = calculatedEmitting.Spectrum.CorrectnessEstimations;
                        }
                        protocol.ProtocolsLinkedWithEmittings.StartFrequency_MHz = calculatedEmitting.StartFrequency_MHz;
                        protocol.ProtocolsLinkedWithEmittings.StopFrequency_MHz = calculatedEmitting.StopFrequency_MHz;
                        protocol.ProtocolsLinkedWithEmittings.TriggerDeviationFromReference = calculatedEmitting.TriggerDeviationFromReference;
                        if (calculatedEmitting.WorkTimes != null)
                        {
                            var workTimes = calculatedEmitting.WorkTimes.ToList();
                            var minStart = workTimes.Min(z => z.StartEmitting);
                            var maxStop = workTimes.Min(z => z.StopEmitting);
                            protocol.ProtocolsLinkedWithEmittings.WorkTimeStart = minStart;
                            protocol.ProtocolsLinkedWithEmittings.WorkTimeStop = maxStop;
                        }
                        lstProtocols.Add(protocol);
                    }
                }
            }
            return lstProtocols.ToArray();
        }

        /// <summary>
        /// Заполнение протокола сведениями о станицях StationExtended
        /// </summary>
        /// <param name="protocolsOutput">итоговый массив ProtocolsOutput, на основе которого будет выполняться генерация отчета</param>
        /// <returns></returns>
        public void FillingProtocolStationData(ref Protocols[] protocolsOutput, Sensor[] sensors, StationExtended[] stationsExtended, long? synchroProcessId)
        {
            var loadSynchroProcessData = new LoadSynchroProcessData(this._dataLayer, this._logger);
            Protocols[] protocolsInput = protocolsOutput;
            var lstProtocols = new List<Protocols>();
            var lstStationsExtended = stationsExtended.ToList();
            var lstSensors = sensors.ToList();
            for (int h = 0; h < protocolsOutput.Length; h++)
            {
                var protocol = protocolsOutput[h];
                
                protocol.DataSynchronizationProcess = new DataSynchronizationProcess();
                protocol.DataSynchronizationProcess = loadSynchroProcessData.CurrentSynchronizationProcesByIds(synchroProcessId);
                if (protocol.DataRefSpectrum!=null)
                {
                    var fndSensor = lstSensors.Find(z => z.Id.Value == protocol.DataRefSpectrum.SensorId);
                    if (fndSensor!=null)
                    {
                        protocol.Sensor = fndSensor;
                    }
                }
                protocol.StationExtended = new StationExtended();
                var fndStation = lstStationsExtended.Find(z => z.TableId == protocolsInput[h].DataRefSpectrum.TableId && z.TableName == protocolsInput[h].DataRefSpectrum.TableName);
                if (fndStation != null)
                {
                    protocol.StationExtended.Address = fndStation.Address;
                    protocol.StationExtended.BandWidth = fndStation.BandWidth;
                    protocol.StationExtended.DesigEmission = fndStation.DesigEmission;
                    protocol.StationExtended.OwnerName = fndStation.OwnerName;
                    protocol.StationExtended.PermissionNumber = fndStation.PermissionNumber;
                    protocol.StationExtended.PermissionStart = fndStation.PermissionStart;
                    protocol.StationExtended.PermissionStop = fndStation.PermissionStop;
                    protocol.StationExtended.Province = fndStation.Province;
                    protocol.StationExtended.Standard = fndStation.Standard;
                    protocol.StationExtended.StandardName = fndStation.StandardName;
                    protocol.StationExtended.TableId = fndStation.TableId;
                    protocol.StationExtended.TableName = fndStation.TableName;
                    protocol.StationExtended.Location = fndStation.Location;
                    protocol.StationExtended.Id = fndStation.Id;
                }
                lstProtocols.Add(protocol);
            }
            protocolsOutput = lstProtocols.ToArray();
        }

        /// <summary>
        /// Группировка данных 
        /// </summary>
        /// <param name="protocolsOutput">итоговый массив ProtocolsOutput, на основе которого будет выполняться генерация отчета</param>
        /// <returns></returns>
        public void OrderProtocols(ref Protocols[] protocolsOutput)
        {
            Protocols[] listProtocols = null;
            var orderByProtocolsOutput = from z in protocolsOutput orderby z.StationExtended.PermissionNumber, z.StationExtended.OwnerName ascending select z;
            listProtocols = orderByProtocolsOutput.ToArray();
        }

        /// <summary>
        /// Сохранение конечного массива в таблицы БД
        /// </summary>
        /// <param name="protocolsOutput">итоговый массив ProtocolsOutput, который нужно сохранить в БД</param>
        /// <returns></returns>
        public bool SaveOutputProtocolsToDB(Protocols[] protocolsOutput)
        {
            bool isSuccess = false;
            try
            {
                this._logger.Info(Contexts.ThisComponent, Categories.Processing, Events.HandlerSaveOutputProtocolsToDBMethod.Text);
                using (var scope = this._dataLayer.CreateScope<SdrnServerDataContext>())
                {
                    scope.BeginTran();

                    for (int i = 0; i < protocolsOutput.Length; i++)
                    {
                        var protocol = protocolsOutput[i];

                        var lstSensors = protocol.Sensor.Locations.ToList();
                        var sensorData = lstSensors.Find(x => x.Status == Status.A.ToString());

                        var builderProtocolsInsert = this._dataLayer.GetBuilder<MD.IProtocols>().Insert();
                        builderProtocolsInsert.SetValue(c => c.SYNCHRO_PROCESS.Id, protocol.DataSynchronizationProcess.Id);
                        builderProtocolsInsert.SetValue(c => c.STATION_EXTENDED.Id, protocol.StationExtended.Id);

                        if (protocol.Sensor != null)
                        {
                            if (protocol.Sensor.Id != null)
                            {
                                builderProtocolsInsert.SetValue(c => c.SensorId, protocol.Sensor.Id.Value);
                            }
                        }
                        if (protocol.DataRefSpectrum != null)
                        {
                            builderProtocolsInsert.SetValue(c => c.DateMeas, protocol.DataRefSpectrum.DateMeas);
                            builderProtocolsInsert.SetValue(c => c.DispersionLow, protocol.DataRefSpectrum.DispersionLow);
                            builderProtocolsInsert.SetValue(c => c.DispersionUp, protocol.DataRefSpectrum.DispersionUp);
                            builderProtocolsInsert.SetValue(c => c.Freq_MHz, protocol.DataRefSpectrum.Freq_MHz);
                            builderProtocolsInsert.SetValue(c => c.GlobalSID, protocol.DataRefSpectrum.GlobalSID);
                            builderProtocolsInsert.SetValue(c => c.Level_dBm, protocol.DataRefSpectrum.Level_dBm);
                            builderProtocolsInsert.SetValue(c => c.Percent, protocol.DataRefSpectrum.Percent);
                           
                            
                        }
                        if (protocol.StationExtended != null)
                        {
                            builderProtocolsInsert.SetValue(c => c.PermissionNumber, protocol.StationExtended.PermissionNumber);
                            builderProtocolsInsert.SetValue(c => c.PermissionStart, protocol.StationExtended.PermissionStart);
                            builderProtocolsInsert.SetValue(c => c.PermissionStop, protocol.StationExtended.PermissionStop);
                        }
                        builderProtocolsInsert.SetValue(c => c.SensorName, protocol.Sensor.Name);
                        if (sensorData != null)
                        {
                            builderProtocolsInsert.SetValue(c => c.SensorLat, sensorData.Lat);
                            builderProtocolsInsert.SetValue(c => c.SensorLon, sensorData.Lon);
                        }
                        var synchroProcessIdValue = scope.Executor.Execute<MD.IProtocols_PK>(builderProtocolsInsert);
                        if (synchroProcessIdValue != null)
                        {
                            long protocolInsertId = synchroProcessIdValue.Id;

                            if (protocol.ProtocolsLinkedWithEmittings != null)
                            {
                                var builderLinkProtocolsWithEmittingsInsert = this._dataLayer.GetBuilder<MD.ILinkProtocolsWithEmittings>().Insert();
                                builderLinkProtocolsWithEmittingsInsert.SetValue(c => c.Bandwidth_kHz, protocol.ProtocolsLinkedWithEmittings.Bandwidth_kHz);
                                builderLinkProtocolsWithEmittingsInsert.SetValue(c => c.Contravention, protocol.ProtocolsLinkedWithEmittings.Contravention);
                                builderLinkProtocolsWithEmittingsInsert.SetValue(c => c.CorrectnessEstimations, protocol.ProtocolsLinkedWithEmittings.CorrectnessEstimations);
                                builderLinkProtocolsWithEmittingsInsert.SetValue(c => c.LevelsDistributionCount, protocol.ProtocolsLinkedWithEmittings.Count);
                                builderLinkProtocolsWithEmittingsInsert.SetValue(c => c.LevelsDistributionLvl, protocol.ProtocolsLinkedWithEmittings.Levels);
                                builderLinkProtocolsWithEmittingsInsert.SetValue(c => c.CurentPower_dBm, protocol.ProtocolsLinkedWithEmittings.CurentPower_dBm);
                                builderLinkProtocolsWithEmittingsInsert.SetValue(c => c.Freq_kHz, protocol.ProtocolsLinkedWithEmittings.Freq_kHz);
                                builderLinkProtocolsWithEmittingsInsert.SetValue(c => c.Levels_dBm, protocol.ProtocolsLinkedWithEmittings.Levels_dBm);
                                builderLinkProtocolsWithEmittingsInsert.SetValue(c => c.Loss_dB, protocol.ProtocolsLinkedWithEmittings.Loss_dB);
                                builderLinkProtocolsWithEmittingsInsert.SetValue(c => c.MarkerIndex, protocol.ProtocolsLinkedWithEmittings.MarkerIndex);
                                builderLinkProtocolsWithEmittingsInsert.SetValue(c => c.MeanDeviationFromReference, protocol.ProtocolsLinkedWithEmittings.MeanDeviationFromReference);
                                builderLinkProtocolsWithEmittingsInsert.SetValue(c => c.Probability, protocol.ProtocolsLinkedWithEmittings.Probability);
                                builderLinkProtocolsWithEmittingsInsert.SetValue(c => c.ReferenceLevel_dBm, protocol.ProtocolsLinkedWithEmittings.ReferenceLevel_dBm);
                                builderLinkProtocolsWithEmittingsInsert.SetValue(c => c.RollOffFactor, protocol.ProtocolsLinkedWithEmittings.RollOffFactor);
                                builderLinkProtocolsWithEmittingsInsert.SetValue(c => c.SignalLevel_dBm, protocol.ProtocolsLinkedWithEmittings.SignalLevel_dBm);
                                builderLinkProtocolsWithEmittingsInsert.SetValue(c => c.SpectrumStartFreq_MHz, protocol.ProtocolsLinkedWithEmittings.SpectrumStartFreq_MHz);
                                builderLinkProtocolsWithEmittingsInsert.SetValue(c => c.SpectrumSteps_kHz, protocol.ProtocolsLinkedWithEmittings.SpectrumSteps_kHz);
                                builderLinkProtocolsWithEmittingsInsert.SetValue(c => c.StandardBW, protocol.ProtocolsLinkedWithEmittings.StandardBW);
                                builderLinkProtocolsWithEmittingsInsert.SetValue(c => c.StartFrequency_MHz, protocol.ProtocolsLinkedWithEmittings.StartFrequency_MHz);
                                builderLinkProtocolsWithEmittingsInsert.SetValue(c => c.StopFrequency_MHz, protocol.ProtocolsLinkedWithEmittings.StopFrequency_MHz);
                                builderLinkProtocolsWithEmittingsInsert.SetValue(c => c.T1, protocol.ProtocolsLinkedWithEmittings.T1);
                                builderLinkProtocolsWithEmittingsInsert.SetValue(c => c.T2, protocol.ProtocolsLinkedWithEmittings.T2);
                                builderLinkProtocolsWithEmittingsInsert.SetValue(c => c.TraceCount, protocol.ProtocolsLinkedWithEmittings.TraceCount);
                                builderLinkProtocolsWithEmittingsInsert.SetValue(c => c.TriggerDeviationFromReference, protocol.ProtocolsLinkedWithEmittings.TriggerDeviationFromReference);
                                builderLinkProtocolsWithEmittingsInsert.SetValue(c => c.WorkTimeStart, protocol.ProtocolsLinkedWithEmittings.WorkTimeStart);
                                builderLinkProtocolsWithEmittingsInsert.SetValue(c => c.WorkTimeStop, protocol.ProtocolsLinkedWithEmittings.WorkTimeStop);
                                builderLinkProtocolsWithEmittingsInsert.SetValue(c => c.PROTOCOLS.Id, protocolInsertId);
                                var linkProtocolsWithEmittingsIdValue = scope.Executor.Execute<MD.ILinkProtocolsWithEmittings_PK>(builderLinkProtocolsWithEmittingsInsert);
                            }
                        }
                    }
                    scope.Commit();
                }
                isSuccess = true;
            }
            catch (Exception e)
            {
                isSuccess = false;
                this._logger.Exception(Contexts.ThisComponent, e);
            }
            return isSuccess;
        }


        public bool ClearProtocol(long synchroProcessId)
        {
            bool isSuccess = false;
            try
            {
                this._logger.Info(Contexts.ThisComponent, Categories.Processing, Events.HandlerClearProtocolMethod.Text);

                using (var scope = this._dataLayer.CreateScope<SdrnServerDataContext>())
                {
                    scope.BeginTran();

                    var builderProtocolsClear = this._dataLayer.GetBuilder<MD.IProtocols>().Delete();
                    builderProtocolsClear.Where(c => c.SYNCHRO_PROCESS.Id, ConditionOperator.Equal, synchroProcessId);
                    scope.Executor.Execute(builderProtocolsClear);

                    var builderLinkProtocolsWithEmittingsClear = this._dataLayer.GetBuilder<MD.ILinkProtocolsWithEmittings>().Delete();
                    builderLinkProtocolsWithEmittingsClear.Where(c => c.PROTOCOLS.SYNCHRO_PROCESS.Id, ConditionOperator.Equal, synchroProcessId);
                    scope.Executor.Execute(builderLinkProtocolsWithEmittingsClear);
                 

                    scope.Commit();
                }
                isSuccess = true;
            }
            catch (Exception e)
            {
                isSuccess = false;
                this._logger.Exception(Contexts.ThisComponent, e);
            }
            return isSuccess;
        }

        public bool SaveDataSynchronizationProcessToDB(Protocols[] protocolsOutput, RefSpectrum[] refSpectrums, long synchroProcessId)
        {
            bool isSuccess = false;
            var CountRecordsImported = 0;
            var CountRecordsOutput = protocolsOutput.Count();
            var CountRecordsOutputWithoutEmitting = 0;
            try
            {
                this._logger.Info(Contexts.ThisComponent, Categories.Processing, Events.HandlerSaveDataSynchronizationProcessToDBMethod.Text);
                using (var scope = this._dataLayer.CreateScope<SdrnServerDataContext>())
                {
                    scope.BeginTran();

                    for (int i=0; i< refSpectrums.Count(); i++)
                    {
                        CountRecordsImported += refSpectrums[i].DataRefSpectrum.Length;
                    }

                    for (int i = 0; i < protocolsOutput.Count(); i++)
                    {
                        if (protocolsOutput[i].ProtocolsLinkedWithEmittings==null)
                        {
                            CountRecordsOutputWithoutEmitting = CountRecordsOutputWithoutEmitting + 1;
                        }
                    }

                    var builderSynchroProcessInsert = this._dataLayer.GetBuilder<MD.ISynchroProcess>().Update();
                    builderSynchroProcessInsert.SetValue(c => c.CountRecordsImported, CountRecordsImported);
                    builderSynchroProcessInsert.SetValue(c => c.CountRecordsOutput, CountRecordsOutput);
                    builderSynchroProcessInsert.SetValue(c => c.CountRecordsOutputWithoutEmitting, CountRecordsOutputWithoutEmitting);
                    builderSynchroProcessInsert.SetValue(c => c.Status, Status.C.ToString());
                    builderSynchroProcessInsert.Where(c => c.Id, ConditionOperator.Equal, synchroProcessId);
                    scope.Executor.Execute(builderSynchroProcessInsert);

                    scope.Commit();
                }
                isSuccess = true;
            }
            catch (Exception e)
            {
                isSuccess = false;
                this._logger.Exception(Contexts.ThisComponent, e);
            }
            return isSuccess;
        }
        

        public Emitting[] GetEmittings(DateTime startDate, DateTime stopDate, GroupSensors groupSensor)
        {
            var listIdsEmittings = new List<long>();
            var listEmitings = new List<KeyValuePair<long, Emitting>>();
            var listWorkTimes = new List<KeyValuePair<long, WorkTime>>();
            var listSpectrum = new List<KeyValuePair<long, Spectrum>>();
            var queryExecuter = this._dataLayer.Executor<SdrnServerDataContext>();
            var listSensors = new List<Sensor>();
            var listEmitting = new List<Emitting>();

            var queryEmitting = this._dataLayer.GetBuilder<MD.IEmitting>()
            .From()
            .Select(c => c.Id, c => c.CurentPower_dBm, c => c.MeanDeviationFromReference, c => c.ReferenceLevel_dBm, c => c.RollOffFactor, c => c.StandardBW, c => c.StartFrequency_MHz, c => c.StopFrequency_MHz, c => c.TriggerDeviationFromReference, c => c.LevelsDistributionLvl, c => c.LevelsDistributionCount, c => c.SensorId, c => c.StationID, c => c.StationTableName, c => c.Loss_dB, c => c.Freq_kHz, c => c.RES_MEAS.SUBTASK_SENSOR.SUBTASK.MEAS_TASK.Id)
            .OrderByAsc(c => c.StartFrequency_MHz)
            .Where(c => c.RES_MEAS.TimeMeas, ConditionOperator.GreaterEqual, startDate)
            .Where(c => c.RES_MEAS.TimeMeas, ConditionOperator.LessEqual, stopDate)
            .Where(c => c.StartFrequency_MHz, ConditionOperator.LessEqual, groupSensor.Freq_MHz)
            .Where(c => c.StopFrequency_MHz, ConditionOperator.GreaterEqual, groupSensor.Freq_MHz)
            .Where(c => c.SensorId, ConditionOperator.Equal, groupSensor.SensorId);
            queryExecuter.Fetch(queryEmitting, reader =>
            {
                while (reader.Read())
                {
                    bool? collectEmissionInstrumentalEstimation = false;
                    var queryMeasTaskSignaling = this._dataLayer.GetBuilder<MD.IMeasTaskSignaling>()
                    .From()
                    .Select(c => c.Id, c => c.CollectEmissionInstrumentalEstimation, c => c.MEAS_TASK.Id)
                    .Where(c => c.MEAS_TASK.Id, ConditionOperator.Equal, reader.GetValue(c => c.RES_MEAS.SUBTASK_SENSOR.SUBTASK.MEAS_TASK.Id));
                    queryExecuter.Fetch(queryMeasTaskSignaling, readerMeasTaskSignaling =>
                    {
                        while (readerMeasTaskSignaling.Read())
                        {
                             var measTaskId = readerMeasTaskSignaling.GetValue(x => x.MEAS_TASK.Id);
                             collectEmissionInstrumentalEstimation = readerMeasTaskSignaling.GetValue(x => x.CollectEmissionInstrumentalEstimation);
                        }
                        return true;
                    });

                    if (((collectEmissionInstrumentalEstimation != null) && (collectEmissionInstrumentalEstimation == false)) || (collectEmissionInstrumentalEstimation==null))
                    {
                        continue;
                    }

                    var emitting = new Emitting();
                    emitting.Id = reader.GetValue(c => c.Id);
                    if (reader.GetValue(c => c.StationID).HasValue)
                    {
                        emitting.AssociatedStationID = reader.GetValue(c => c.StationID).Value;
                    }
                    emitting.AssociatedStationTableName = reader.GetValue(c => c.StationTableName);

                    if (reader.GetValue(c => c.StartFrequency_MHz).HasValue)
                        emitting.StartFrequency_MHz = reader.GetValue(c => c.StartFrequency_MHz).Value;
                    if (reader.GetValue(c => c.StopFrequency_MHz).HasValue)
                        emitting.StopFrequency_MHz = reader.GetValue(c => c.StopFrequency_MHz).Value;
                    if (reader.GetValue(c => c.CurentPower_dBm).HasValue)
                        emitting.CurentPower_dBm = reader.GetValue(c => c.CurentPower_dBm).Value;
                    if (reader.GetValue(c => c.ReferenceLevel_dBm).HasValue)
                        emitting.ReferenceLevel_dBm = reader.GetValue(c => c.ReferenceLevel_dBm).Value;
                    if (reader.GetValue(c => c.MeanDeviationFromReference).HasValue)
                        emitting.MeanDeviationFromReference = reader.GetValue(c => c.MeanDeviationFromReference).Value;
                    if (reader.GetValue(c => c.TriggerDeviationFromReference).HasValue)
                        emitting.TriggerDeviationFromReference = reader.GetValue(c => c.TriggerDeviationFromReference).Value;

                    if (reader.GetValue(c => c.SensorId).HasValue)
                    {
                        if (listSensors.Find(x => x.Id.Value == reader.GetValue(c => c.SensorId).Value) == null)
                        {
                            var querySensor = this._dataLayer.GetBuilder<MD.ISensor>()
                            .From()
                            .Select(c => c.Id, c => c.Name, c => c.TechId)
                            .Where(c => c.Id, ConditionOperator.Equal, reader.GetValue(c => c.SensorId).Value);
                            queryExecuter.Fetch(querySensor, readerSensor =>
                            {
                                while (readerSensor.Read())
                                {
                                    emitting.SensorName = readerSensor.GetValue(c => c.Name);
                                    emitting.SensorTechId = readerSensor.GetValue(c => c.TechId);
                                    listSensors.Add(new Sensor()
                                    {
                                        Id = new SensorIdentifier()
                                        {
                                            Value = reader.GetValue(c => c.SensorId).Value
                                        },
                                        Name = readerSensor.GetValue(c => c.Name),
                                        Equipment = new SensorEquip()
                                        {
                                            TechId = readerSensor.GetValue(c => c.TechId)
                                        }
                                    });
                                    break;
                                }
                                return true;
                            });
                        }
                        else
                        {
                            var fndSensor = listSensors.Find(x => x.Id.Value == reader.GetValue(c => c.SensorId).Value);
                            if (fndSensor != null)
                            {
                                emitting.SensorName = fndSensor.Name;
                                emitting.SensorTechId = fndSensor.Equipment.TechId;
                            }
                        }
                    }

                    var emittingParam = new EmittingParameters();

                    if (reader.GetValue(c => c.RollOffFactor).HasValue)
                        emittingParam.RollOffFactor = reader.GetValue(c => c.RollOffFactor).Value;
                    if (reader.GetValue(c => c.StandardBW).HasValue)
                    {
                        emittingParam.StandardBW = reader.GetValue(c => c.StandardBW).Value;
                    }


                    var levelDist = new LevelsDistribution();
                    if (reader.GetValue(c => c.LevelsDistributionLvl) != null)
                    {
                        levelDist.Levels = reader.GetValue(c => c.LevelsDistributionLvl);
                    }
                    if (reader.GetValue(c => c.LevelsDistributionCount) != null)
                    {
                        levelDist.Count = reader.GetValue(c => c.LevelsDistributionCount);
                    }

                    emitting.LevelsDistribution = levelDist;
                    emitting.EmittingParameters = emittingParam;

                    var signalMask = new SignalMask();
                    if (reader.GetValue(c => c.Loss_dB) != null)
                    {
                        signalMask.Loss_dB = reader.GetValue(c => c.Loss_dB);
                    }
                    if (reader.GetValue(c => c.Freq_kHz) != null)
                    {
                        signalMask.Freq_kHz = reader.GetValue(c => c.Freq_kHz);
                    }
                    emitting.SignalMask = signalMask;
                    listIdsEmittings.Add(reader.GetValue(c => c.Id));
                    listEmitings.Add(new KeyValuePair<long, Emitting>(reader.GetValue(c => c.Id), emitting));
                }
                return true;
            });

            var listIntEmittingWorkTime = BreakDownElemBlocks.BreakDown(listIdsEmittings.ToArray());
            for (int i = 0; i < listIntEmittingWorkTime.Count; i++)
            {
                var queryTime = this._dataLayer.GetBuilder<MD.IWorkTime>()
                      .From()
                      .Select(c => c.Id, c => c.StartEmitting, c => c.StopEmitting, c => c.HitCount, c => c.PersentAvailability, c => c.EMITTING.Id)
                      .Where(c => c.EMITTING.Id, ConditionOperator.In, listIntEmittingWorkTime[i]);
                queryExecuter.Fetch(queryTime, readerTime =>
                {
                    while (readerTime.Read())
                    {
                        var workTime = new WorkTime();
                        workTime.StartEmitting = readerTime.GetValue(c => c.StartEmitting);
                        workTime.StopEmitting = readerTime.GetValue(c => c.StopEmitting);
                        workTime.HitCount = readerTime.GetValue(c => c.HitCount);
                        workTime.PersentAvailability = readerTime.GetValue(c => c.PersentAvailability);

                        listWorkTimes.Add(new KeyValuePair<long, WorkTime>(readerTime.GetValue(c => c.EMITTING.Id), workTime));
                    }
                    return true;
                });
            }



            var listIntEmittingSpectrum = BreakDownElemBlocks.BreakDown(listIdsEmittings.ToArray());
            for (int i = 0; i < listIntEmittingSpectrum.Count; i++)
            {
                var querySpectrum = this._dataLayer.GetBuilder<MD.ISpectrum>()
                       .From()
                       .Select(c => c.Id, c => c.Levels_dBm, c => c.SpectrumStartFreq_MHz, c => c.SpectrumSteps_kHz, c => c.Bandwidth_kHz, c => c.TraceCount, c => c.SignalLevel_dBm, c => c.MarkerIndex, c => c.CorrectnessEstimations, c => c.T1, c => c.T2, c => c.EMITTING.Id, c => c.Contravention)
                       .Where(c => c.EMITTING.Id, ConditionOperator.In, listIntEmittingSpectrum[i]);
                queryExecuter.Fetch(querySpectrum, readerSpectrum =>
                {
                    while (readerSpectrum.Read())
                    {
                        var spectrum = new Spectrum();
                        if (readerSpectrum.GetValue(c => c.Levels_dBm) != null)
                        {
                            spectrum.Levels_dBm = readerSpectrum.GetValue(c => c.Levels_dBm);
                        }

                        if (spectrum.SpectrumStartFreq_MHz == 0)
                        {
                            if (readerSpectrum.GetValue(c => c.SpectrumStartFreq_MHz).HasValue)
                            {
                                spectrum.SpectrumStartFreq_MHz = readerSpectrum.GetValue(c => c.SpectrumStartFreq_MHz).Value;
                            }


                            if (readerSpectrum.GetValue(c => c.SpectrumSteps_kHz).HasValue)
                            {
                                spectrum.SpectrumSteps_kHz = readerSpectrum.GetValue(c => c.SpectrumSteps_kHz).Value;
                            }


                            if (readerSpectrum.GetValue(c => c.Bandwidth_kHz).HasValue)
                            {
                                spectrum.Bandwidth_kHz = readerSpectrum.GetValue(c => c.Bandwidth_kHz).Value;
                            }


                            if (readerSpectrum.GetValue(c => c.TraceCount).HasValue)
                            {
                                spectrum.TraceCount = readerSpectrum.GetValue(c => c.TraceCount).Value;
                            }


                            if (readerSpectrum.GetValue(c => c.SignalLevel_dBm).HasValue)
                            {
                                spectrum.SignalLevel_dBm = readerSpectrum.GetValue(c => c.SignalLevel_dBm).Value;
                            }


                            if (readerSpectrum.GetValue(c => c.MarkerIndex).HasValue)
                            {
                                spectrum.MarkerIndex = readerSpectrum.GetValue(c => c.MarkerIndex).Value;
                            }


                            if (readerSpectrum.GetValue(c => c.T1).HasValue)
                            {
                                spectrum.T1 = readerSpectrum.GetValue(c => c.T1).Value;
                            }


                            if (readerSpectrum.GetValue(c => c.T2).HasValue)
                            {
                                spectrum.T2 = readerSpectrum.GetValue(c => c.T2).Value;
                            }

                            if (readerSpectrum.GetValue(c => c.CorrectnessEstimations).HasValue)
                            {
                                spectrum.CorrectnessEstimations = readerSpectrum.GetValue(c => c.CorrectnessEstimations).Value == 1 ? true : false;
                            }

                            if (readerSpectrum.GetValue(c => c.Contravention).HasValue)
                            {
                                spectrum.Contravention = readerSpectrum.GetValue(c => c.Contravention).Value == 1 ? true : false;
                            }
                        }
                        listSpectrum.Add(new KeyValuePair<long, Spectrum>(readerSpectrum.GetValue(c => c.EMITTING.Id), spectrum));
                    }
                    return true;
                });
            }

            var arrayListIdsEmittings = listIdsEmittings.ToArray();
            for (int i = 0; i < arrayListIdsEmittings.Length; i++)
            {
                var id = arrayListIdsEmittings[i];
                var fndEmitings = listEmitings.Find(c => c.Key == id);
                if (fndEmitings.Value != null)
                {
                    var emitting = fndEmitings.Value;

                    var fndWorkTime = listWorkTimes.FindAll(c => c.Key == id);
                    if (fndWorkTime != null)
                    {
                        var arrayFndWorkTime = fndWorkTime.ToArray();
                        var listWorkTime = new List<WorkTime>();
                        for (int t = 0; t < arrayFndWorkTime.Length; t++)
                        {
                            listWorkTime.Add(arrayFndWorkTime[t].Value);
                        }
                        if (listWorkTime.Count > 0)
                        {
                            emitting.WorkTimes = listWorkTime.ToArray();
                        }
                    }

                    listWorkTimes.RemoveAll(c => c.Key == id);

                    var fndSpectrum = listSpectrum.FindAll(c => c.Key == id);
                    if (fndSpectrum != null)
                    {
                        var arrayFndSpectrum = fndSpectrum.ToArray();
                        var listLevelsdBm = new List<float>();
                        for (int t = 0; t < arrayFndSpectrum.Length; t++)
                        {
                            var x = arrayFndSpectrum[t];
                            if (x.Value != null)
                            {
                                if (x.Value.Levels_dBm != null)
                                {
                                    listLevelsdBm.AddRange(x.Value.Levels_dBm);
                                    emitting.Spectrum = x.Value;
                                }
                            }
                        }
                        if (listLevelsdBm.Count > 0)
                        {
                            emitting.Spectrum.Levels_dBm = listLevelsdBm.ToArray();
                        }
                    }
                    listSpectrum.RemoveAll(c => c.Key == id);
                    listEmitting.Add(emitting);
                }
                listEmitings.RemoveAll(c => c.Key == id);
            }
            return listEmitting.ToArray();
        }



        public long[] GetHeadRefSpectrumIdsBySDRN(DataSynchronizationBase dataSynchronization)
        {
            var headRefSpectrumIds = new List<long>();
            if (dataSynchronization != null)
            {
                var queryExecuter = this._dataLayer.Executor<SdrnServerDataContext>();
                var queryLinkHeadRefSpectrumFrom = this._dataLayer.GetBuilder<MD.ILinkHeadRefSpectrum>()
                .From()
                .Select(c => c.Id, c => c.SYNCHRO_PROCESS.DateStart, c => c.SYNCHRO_PROCESS.DateEnd, c => c.SYNCHRO_PROCESS.CreatedBy, c => c.SYNCHRO_PROCESS.CreatedDate, c => c.HEAD_REF_SPECTRUM.Id)
                .Where(c => c.SYNCHRO_PROCESS.DateStart, ConditionOperator.Equal, dataSynchronization.DateStart)
                .Where(c => c.SYNCHRO_PROCESS.DateEnd, ConditionOperator.Equal, dataSynchronization.DateEnd)
                .Where(c => c.SYNCHRO_PROCESS.CreatedBy, ConditionOperator.Equal, dataSynchronization.CreatedBy)
                .Where(c => c.SYNCHRO_PROCESS.CreatedDate, ConditionOperator.Equal, dataSynchronization.DateCreated);
                queryExecuter.Fetch(queryLinkHeadRefSpectrumFrom, readerLinkHeadRefSpectrum =>
                {
                    while (readerLinkHeadRefSpectrum.Read())
                    {
                        headRefSpectrumIds.Add(readerLinkHeadRefSpectrum.GetValue(c => c.HEAD_REF_SPECTRUM.Id));
                    }
                    return true;
                });
            }
            return headRefSpectrumIds.ToArray();
        }

        public long[] GetSensorIdsBySDRN(DataSynchronizationBase dataSynchronization)
        {
            var sensorIdsBySDRN = new List<long>();
            if (dataSynchronization != null)
            {
                var queryExecuter = this._dataLayer.Executor<SdrnServerDataContext>();
                var queryLinkSensorsWithSynchroProcessFrom = this._dataLayer.GetBuilder<MD.ILinkSensorsWithSynchroProcess>()
                .From()
                .Select(c => c.Id, c => c.SYNCHRO_PROCESS.DateStart, c => c.SYNCHRO_PROCESS.DateEnd, c => c.SYNCHRO_PROCESS.CreatedBy, c => c.SYNCHRO_PROCESS.CreatedDate, c => c.SensorId)
                .Where(c => c.SYNCHRO_PROCESS.DateStart, ConditionOperator.Equal, dataSynchronization.DateStart)
                .Where(c => c.SYNCHRO_PROCESS.DateEnd, ConditionOperator.Equal, dataSynchronization.DateEnd)
                .Where(c => c.SYNCHRO_PROCESS.CreatedBy, ConditionOperator.Equal, dataSynchronization.CreatedBy)
                .Where(c => c.SYNCHRO_PROCESS.CreatedDate, ConditionOperator.Equal, dataSynchronization.DateCreated);
                queryExecuter.Fetch(queryLinkSensorsWithSynchroProcessFrom, readerLinkSensorsWithSynchroProcess =>
                {
                    while (readerLinkSensorsWithSynchroProcess.Read())
                    {
                        sensorIdsBySDRN.Add(readerLinkSensorsWithSynchroProcess.GetValue(c => c.SensorId));
                    }
                    return true;
                });
            }
            return sensorIdsBySDRN.ToArray();
        }

        public Area[] GetAreas(DataSynchronizationBase dataSynchronization)
        {
            var areas = new List<Area>();
            try
            {
                var areaIds = new List<long>();

                var queryExecuter = this._dataLayer.Executor<SdrnServerDataContext>();
                var queryLinkAreaFrom = this._dataLayer.GetBuilder<MD.ILinkArea>()
                .From()
                .Select(c => c.Id, c => c.AREA.Id, c => c.SYNCHRO_PROCESS.DateStart, c => c.SYNCHRO_PROCESS.DateEnd, c => c.SYNCHRO_PROCESS.CreatedBy, c => c.SYNCHRO_PROCESS.CreatedDate)
                .Where(c => c.SYNCHRO_PROCESS.DateStart, ConditionOperator.Equal, dataSynchronization.DateStart)
                .Where(c => c.SYNCHRO_PROCESS.DateEnd, ConditionOperator.Equal, dataSynchronization.DateEnd)
                .Where(c => c.SYNCHRO_PROCESS.CreatedBy, ConditionOperator.Equal, dataSynchronization.CreatedBy)
                .Where(c => c.SYNCHRO_PROCESS.CreatedDate, ConditionOperator.Equal, dataSynchronization.DateCreated);
                queryExecuter.Fetch(queryLinkAreaFrom, readerLinkAreaFrom =>
                {
                    while (readerLinkAreaFrom.Read())
                    {
                        areaIds.Add(readerLinkAreaFrom.GetValue(c => c.AREA.Id));
                    }
                    return true;
                });


                for (int i = 0; i < areaIds.Count; i++)
                {
                    var area = new Area();
                    var queryAreaFrom = this._dataLayer.GetBuilder<MD.IArea>()
                    .From()
                    .Select(c => c.Id, c => c.CreatedBy, c => c.CreatedDate, c => c.IdentifierFromICSM, c => c.Name, c => c.TypeOfArea)
                    .Where(c => c.Id, ConditionOperator.Equal, areaIds[i]) ;
                    queryExecuter.Fetch(queryAreaFrom, readerAreaFrom =>
                    {
                        while (readerAreaFrom.Read())
                        {
                            area.CreatedBy = readerAreaFrom.GetValue(c=>c.CreatedBy);
                            area.DateCreated = readerAreaFrom.GetValue(c => c.CreatedDate);
                            area.IdentifierFromICSM = readerAreaFrom.GetValue(c => c.IdentifierFromICSM);
                            area.Name = readerAreaFrom.GetValue(c => c.Name);
                            area.TypeArea = readerAreaFrom.GetValue(c => c.TypeOfArea);

                            var dataLocations = new List<DataLocation>();
                            var queryAreaLocationFrom = this._dataLayer.GetBuilder<MD.IAreaLocation>()
                            .From()
                            .Select(c => c.Id, c => c.Latitude, c => c.Longitude)
                            .Where(c => c.AREA.Id, ConditionOperator.Equal, areaIds[i]);
                            queryExecuter.Fetch(queryAreaLocationFrom, readerAreaLocationFrom =>
                            {
                                while (readerAreaLocationFrom.Read())
                                {
                                    var dataLocation = new DataLocation();
                                    dataLocation.Latitude = readerAreaLocationFrom.GetValue(c => c.Latitude);
                                    dataLocation.Longitude = readerAreaLocationFrom.GetValue(c => c.Longitude);
                                    dataLocations.Add(dataLocation);
                                }
                                return true;
                            });
                            area.Location = dataLocations.ToArray();
                        }
                        return true;
                    });
                    areas.Add(area);
                }
            }
            catch (Exception e)
            {
                this._logger.Exception(Contexts.ThisComponent, e);
            }
            return areas.ToArray();
        }

        public StationExtended[] GetStationExtended(DataSynchronizationBase dataSynchronization, long[] headRefSpectrumIdsBySDRN)
        {
            var stationsExtended = new List<StationExtended>();
            try
            {
                var queryExecuter = this._dataLayer.Executor<SdrnServerDataContext>();
                var queryRefSpectrumFrom = this._dataLayer.GetBuilder<MD.IRefSpectrum>()
                .From()
                .Select(c => c.Id, c => c.TableId, c => c.TableName)
                .Where(c => c.HEAD_REF_SPECTRUM.Id, ConditionOperator.In, headRefSpectrumIdsBySDRN);
                queryExecuter.Fetch(queryRefSpectrumFrom, readerRefSpectrumFrom =>
                {
                    while (readerRefSpectrumFrom.Read())
                    {
                      
                        var queryStationExtendedFrom = this._dataLayer.GetBuilder<MD.IStationExtended>()
                        .From()
                        .Select(c => c.Id, c => c.Address, c => c.BandWidth, c => c.DesigEmission, c => c.Latitude, c => c.Longitude, c => c.OwnerName, c => c.PermissionNumber, c => c.PermissionStart, c => c.PermissionStop, c => c.Province, c => c.Standard, c => c.StandardName, c => c.TableId, c => c.TableName)
                        .Where(c => c.TableId, ConditionOperator.Equal, readerRefSpectrumFrom.GetValue(c=>c.TableId))
                        .Where(c => c.TableName, ConditionOperator.Equal, readerRefSpectrumFrom.GetValue(c => c.TableName));
                        queryExecuter.Fetch(queryStationExtendedFrom, readerStationExtendedFrom =>
                        {
                            while (readerStationExtendedFrom.Read())
                            {
                                var stationExtended = new StationExtended();
                                stationExtended.Address = readerStationExtendedFrom.GetValue(c=>c.Address);
                                stationExtended.BandWidth = readerStationExtendedFrom.GetValue(c => c.BandWidth);
                                stationExtended.DesigEmission = readerStationExtendedFrom.GetValue(c => c.DesigEmission);
                                stationExtended.Location = new DataLocation();
                                stationExtended.Location.Longitude = readerStationExtendedFrom.GetValue(c => c.Longitude);
                                stationExtended.Location.Latitude = readerStationExtendedFrom.GetValue(c => c.Latitude);
                                stationExtended.OwnerName = readerStationExtendedFrom.GetValue(c => c.OwnerName);
                                stationExtended.PermissionNumber = readerStationExtendedFrom.GetValue(c => c.PermissionNumber);
                                stationExtended.PermissionStart = readerStationExtendedFrom.GetValue(c => c.PermissionStart);
                                stationExtended.PermissionStop = readerStationExtendedFrom.GetValue(c => c.PermissionStop);
                                stationExtended.Province = readerStationExtendedFrom.GetValue(c => c.Province);
                                stationExtended.Standard = readerStationExtendedFrom.GetValue(c => c.Standard);
                                stationExtended.StandardName = readerStationExtendedFrom.GetValue(c => c.StandardName);
                                stationExtended.TableId = readerStationExtendedFrom.GetValue(c => c.TableId);
                                stationExtended.TableName = readerStationExtendedFrom.GetValue(c => c.TableName);
                                stationsExtended.Add(stationExtended);
                            }
                            return true;
                        });
                    }
                    return true;
                });
            }
            catch (Exception e)
            {
                this._logger.Exception(Contexts.ThisComponent, e);
            }
            return stationsExtended.ToArray();
        }


        public  void RecoveryDataSynchronizationProcess()
        {
            var loadSensor = new LoadSensor(this._dataLayer, this._logger);
            var loadSynchroProcessData = new LoadSynchroProcessData(this._dataLayer, this._logger);
            var currentDataSynchronizationProcess = loadSynchroProcessData.CurrentDataSynchronizationProcess();
            
            //если процесс синхронизации не запущен, тогда создаем новый
            if (currentDataSynchronizationProcess != null)
            {

                // текущий процесс
                var dataSynchronization = currentDataSynchronizationProcess;

                // набор идентификаторов headRefSpectrumIdsBySDRN
                var headRefSpectrumIdsBySDRN = GetHeadRefSpectrumIdsBySDRN(dataSynchronization);

                // набор идентификаторов sensorIdsBySDRN
                var sensorIdsBySDRN = GetSensorIdsBySDRN(dataSynchronization);

                // набор areas
                var areas = GetAreas(dataSynchronization);

                // набор stationsExtended
                var stationsExtended = GetStationExtended(dataSynchronization, headRefSpectrumIdsBySDRN);

                var isSuccessDeleteRefSpectrum = DeleteDuplicateRefSpectrumRecords(dataSynchronization, headRefSpectrumIdsBySDRN, sensorIdsBySDRN, areas, stationsExtended);
                if (isSuccessDeleteRefSpectrum == true)
                {
                    var sensors = new Sensor[sensorIdsBySDRN.Length];
                    for (int j = 0; j < sensorIdsBySDRN.Length; j++)
                    {
                        sensors[j] = loadSensor.LoadBaseDateSensor(sensorIdsBySDRN[j]);
                    }

                    // заново вычитываем из БД все RefSpectrum (после удаления дубликатов) 
                    var listRefSpectrum = loadSynchroProcessData.GetRefSpectrumByIds(headRefSpectrumIdsBySDRN, sensorIdsBySDRN);

                    // Весь массив разбивается на группы (далее группа сенсора) по следующему признаку: Одинаковые ID Sensor, Freq MHz
                    var groupsSensors = GetGroupSensors(listRefSpectrum);

                    var listProtocolsOutput = new List<Protocols>();
                    for (int h = 0; h < groupsSensors.Length; h++)
                    {
                        //здесь получаем массив RefSpectrum, который соответсвует группе groupsSensors[h]
                        var refSpectrum = SelectGroupSensor(groupsSensors[h], listRefSpectrum);

                        // Подготовка данных для синхронизации группы сенсора. Из БД ICSControl выбираются все Emitting для которых:
                        //-таск имеет Collect emission for instrumental estimation = true;
                        // -результат пришел в рамках даты начала отчета, даты конца отчета;
                        // -сенсор где был получен результат совпадает с ID Sensor
                        //-частота Freq MHz(из группы сенсора) находиться в пределах начальной и конечной частоты Emitting

                        var emittings = GetEmittings(dataSynchronization.DateStart, dataSynchronization.DateEnd, groupsSensors[h]);

                        // Синхронизация излучений с записями группы сенсора
                        var protocol = SynchroEmittings(refSpectrum, emittings);

                        listProtocolsOutput.AddRange(protocol);
                    }

                    // 
                    var arrayProtocols = listProtocolsOutput.ToArray();

                    // заполнение полным перечнем данных (расширенными сведениями о станциях и сенсорах)
                    FillingProtocolStationData(ref arrayProtocols, sensors, stationsExtended, dataSynchronization.Id);

                    // Группировка данных
                    OrderProtocols(ref arrayProtocols);
                    // предварительная очистка таблиц Protocols и LinkProtocolsWithEmittings для текущего synchroProcessId
                    if (ClearProtocol(dataSynchronization.Id.Value))
                    {
                        // здесь вызов процецедуры сохранения массива значений protocol
                        var isSuccessOperation = SaveOutputProtocolsToDB(arrayProtocols);
                        // запись итоговой информации в ISynchroProcess (и установка статуса в "С" - Completed)
                        var isSuccesFinalOperationSynchro = SaveDataSynchronizationProcessToDB(arrayProtocols, listRefSpectrum, dataSynchronization.Id.Value);
                    }
                }
            }
        }

        public bool RunDataSynchronizationProcess(DataSynchronizationBase dataSynchronization, long[] headRefSpectrumIdsBySDRN, long[] sensorIdsBySDRN, Area[] areas, StationExtended[] stationsExtended)
        {
            var loadSynchroProcessData = new LoadSynchroProcessData(this._dataLayer, this._logger);
            var currentDataSynchronizationProcess = loadSynchroProcessData.CurrentDataSynchronizationProcess();
            
            //если процесс синхронизации не запущен, тогда создаем новый
            if (currentDataSynchronizationProcess == null)
            {
                var loadSensor = new LoadSensor(this._dataLayer, this._logger);

                var sensors = new Sensor[sensorIdsBySDRN.Length];
                for (int j = 0; j < sensorIdsBySDRN.Length; j++)
                {
                    sensors[j] = loadSensor.LoadBaseDateSensor(sensorIdsBySDRN[j]);
                }

                // обновление областей и контуров
                var isSuccessUpdateAreas = SynchroAreas(areas);
                // обновление перечня станций
                var isSuccessUpdateStationExtended = SynchroStationsExtended(stationsExtended);

                if ((isSuccessUpdateAreas == true) && (isSuccessUpdateStationExtended == true))
                { 
                // создаем запись в таблице SynchroProcess
                var synchroProcessId = CreateDataSynchronizationProcess(dataSynchronization, headRefSpectrumIdsBySDRN, sensorIdsBySDRN, areas);
                    if (synchroProcessId != null)
                    {
                        var isSuccessDeleteRefSpectrum = DeleteDuplicateRefSpectrumRecords(dataSynchronization, headRefSpectrumIdsBySDRN, sensorIdsBySDRN, areas, stationsExtended);
                        if (isSuccessDeleteRefSpectrum == true)
                        {
                            // заново вычитываем из БД все RefSpectrum (после удаления дубликатов) 
                            var listRefSpectrum = loadSynchroProcessData.GetRefSpectrumByIds(headRefSpectrumIdsBySDRN, sensorIdsBySDRN);

                            // Весь массив разбивается на группы (далее группа сенсора) по следующему признаку: Одинаковые ID Sensor, Freq MHz
                            var groupsSensors = GetGroupSensors(listRefSpectrum);

                            var listProtocolsOutput = new List<Protocols>();
                            for (int h = 0; h < groupsSensors.Length; h++)
                            {
                                //здесь получаем массив RefSpectrum, который соответсвует группе groupsSensors[h]
                                var refSpectrum = SelectGroupSensor(groupsSensors[h], listRefSpectrum);

                                // Подготовка данных для синхронизации группы сенсора. Из БД ICSControl выбираются все Emitting для которых:
                                //-таск имеет Collect emission for instrumental estimation = true;
                                // -результат пришел в рамках даты начала отчета, даты конца отчета;
                                // -сенсор где был получен результат совпадает с ID Sensor
                                //-частота Freq MHz(из группы сенсора) находиться в пределах начальной и конечной частоты Emitting

                                var emittings = GetEmittings(dataSynchronization.DateStart, dataSynchronization.DateEnd, groupsSensors[h]);

                                // Синхронизация излучений с записями группы сенсора
                                var protocol = SynchroEmittings(refSpectrum, emittings);

                                listProtocolsOutput.AddRange(protocol);
                            }

                            // 
                            var arrayProtocols = listProtocolsOutput.ToArray();

                            // заполнение полным перечнем данных (расширенными сведениями о станциях и сенсорах)
                            FillingProtocolStationData(ref arrayProtocols, sensors, stationsExtended, synchroProcessId);

                            // Группировка данных
                            OrderProtocols(ref arrayProtocols);
                            // предварительная очистка таблиц Protocols и LinkProtocolsWithEmittings для текущего synchroProcessId
                            if (ClearProtocol(synchroProcessId.Value))
                            {
                                // здесь вызов процецедуры сохранения массива значений protocol
                                var isSuccessOperation = SaveOutputProtocolsToDB(arrayProtocols);
                                // запись итоговой информации в ISynchroProcess (и установка статуса в "С" - Completed)
                                var isSuccesFinalOperationSynchro = SaveDataSynchronizationProcessToDB(arrayProtocols, listRefSpectrum, synchroProcessId.Value);
                            }
                        }
                    }
                }
            }
            return true;
        }
    }
}




