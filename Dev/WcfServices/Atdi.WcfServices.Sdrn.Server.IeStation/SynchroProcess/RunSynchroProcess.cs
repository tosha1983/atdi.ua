using System.Collections.Generic;
using Atdi.Contracts.Api.EventSystem;
using Atdi.Contracts.CoreServices.DataLayer;
using Atdi.Contracts.CoreServices.EntityOrm;
using Atdi.Contracts.Sdrn.Server;
using Atdi.DataModels.DataConstraint;
using Atdi.Platform.Logging;
using System;
using MD = Atdi.DataModels.Sdrns.Server.Entities.IeStation;
using MDBase = Atdi.DataModels.Sdrns.Server.Entities;
using Atdi.Contracts.WcfServices.Sdrn.Server.IeStation;
using System.Xml;
using System.Linq;
using Atdi.Common;
using Calculation = Atdi.Modules.Sdrn.Calculation;



namespace Atdi.WcfServices.Sdrn.Server.IeStation
{
    public class RunSynchroProcess
    {
        private static bool IsAlreadyRunProcess;
        private readonly IDataLayer<EntityDataOrm> _dataLayer;
        private readonly ILogger _logger;

        public RunSynchroProcess(IDataLayer<EntityDataOrm> dataLayer, ILogger logger)
        {
            this._dataLayer = dataLayer;
            this._logger = logger;
        }


        /// <summary>
        /// Синхронизация массива Area[]
        /// </summary>
        /// <param name="areas"></param>
        /// <returns></returns>
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
                                    bool isChanged = false;

                                    var builderUpdateArea = this._dataLayer.GetBuilder<MD.IArea>().Update();
                                    builderUpdateArea.Where(c => c.IdentifierFromICSM, ConditionOperator.Equal, areas[i].IdentifierFromICSM);

                                    if (readerAreaFrom.GetValue(c => c.CreatedBy) != areas[i].CreatedBy)
                                    {
                                        builderUpdateArea.SetValue(c => c.CreatedBy, areas[i].CreatedBy);
                                        isChanged = true;
                                    }
                                    if (readerAreaFrom.GetValue(c => c.CreatedDate) != areas[i].DateCreated)
                                    {
                                        builderUpdateArea.SetValue(c => c.CreatedDate, areas[i].DateCreated);
                                        isChanged = true;
                                    }

                                    if (readerAreaFrom.GetValue(c => c.Name) != areas[i].Name)
                                    {
                                        builderUpdateArea.SetValue(c => c.Name, areas[i].Name);
                                        isChanged = true;
                                    }

                                    if (readerAreaFrom.GetValue(c => c.TypeOfArea) != areas[i].TypeArea)
                                    {
                                        builderUpdateArea.SetValue(c => c.TypeOfArea, areas[i].TypeArea);
                                        isChanged = true;
                                    }

                                    if (isChanged)
                                    {
                                        scope.Executor.Execute(builderUpdateArea);
                                    }


                                    for (int j = 0; j < areas[i].Location.Length; j++)
                                    {
                                        var location = areas[i].Location[j];
                                        if (location != null)
                                        {
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
                                            var findLocation = listLocations.Find(x => Math.Abs(x.Latitude - readerAreaLocation.GetValue(c => c.Latitude)) < 0.000001 && (x.Longitude - readerAreaLocation.GetValue(c => c.Longitude)) < 0.000001);
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
                            if ((areas[i].Name != "Вся Україна") && (areas[i].Name != "Київ") && (areas[i].Name != "Севастополь"))
                            {
                                var builderAreaInsert = this._dataLayer.GetBuilder<MD.IArea>().Insert();
                                if (areas[i].CreatedBy != null)
                                {
                                    builderAreaInsert.SetValue(c => c.CreatedBy, areas[i].CreatedBy);
                                }
                                if (areas[i].DateCreated != null)
                                {
                                    builderAreaInsert.SetValue(c => c.CreatedDate, areas[i].DateCreated);
                                }
                                if (areas[i].Name != null)
                                {
                                    builderAreaInsert.SetValue(c => c.Name, areas[i].Name);
                                }
                                if (areas[i].TypeArea != null)
                                {
                                    builderAreaInsert.SetValue(c => c.TypeOfArea, areas[i].TypeArea);
                                }
                                builderAreaInsert.SetValue(c => c.IdentifierFromICSM, areas[i].IdentifierFromICSM);
                                var areaId = scope.Executor.Execute<MD.IArea_PK>(builderAreaInsert);
                                if (areaId != null)
                                {
                                    if (areas[i].Location != null)
                                    {
                                        for (int k = 0; k < areas[i].Location.Length; k++)
                                        {
                                            var loc = areas[i].Location[k];
                                            if (loc != null)
                                            {
                                                var builderInsAreaLocation = this._dataLayer.GetBuilder<MD.IAreaLocation>().Insert();
                                                builderInsAreaLocation.SetValue(c => c.Latitude, loc.Latitude);
                                                builderInsAreaLocation.SetValue(c => c.Longitude, loc.Longitude);
                                                builderInsAreaLocation.SetValue(c => c.AREA.Id, areaId.Id);
                                                scope.Executor.Execute<MD.IAreaLocation_PK>(builderInsAreaLocation);
                                            }
                                        }
                                    }
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

        /// <summary>
        /// Синхронизация станций
        /// </summary>
        /// <param name="stationsExtended"></param>
        /// <returns></returns>
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
                        .Select(c => c.Id, c => c.Address, c => c.BandWidth, c => c.DesigEmission, c => c.Latitude, c => c.Longitude, c => c.OwnerName, c => c.PermissionNumber, c => c.PermissionStart, c => c.PermissionStop, c => c.Province, c => c.Standard, c => c.StandardName, c => c.TableId, c => c.TableName,
                                c => c.DocNum, c => c.StationName, c => c.StationChannel, c => c.StationTxFreq, c => c.StationRxFreq, c => c.PermissionCancelDate, c => c.TestStartDate, c => c.TestStopDate, c => c.PermissionGlobalSID, c => c.OKPO, c => c.StatusMeas, c => c.CurentStatusStation)
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

                                    bool isChanged = false;

                                    var builderUpdateStationExtended = this._dataLayer.GetBuilder<MD.IStationExtended>().Update();
                                    builderUpdateStationExtended.Where(c => c.TableId, ConditionOperator.Equal, stationsExtended[i].TableId);
                                    builderUpdateStationExtended.Where(c => c.TableName, ConditionOperator.Equal, stationsExtended[i].TableName);

                                    if (readerStationExtendedFrom.GetValue(c => c.Address) != stationsExtended[i].Address)
                                    {
                                        isChanged = true;
                                        builderUpdateStationExtended.SetValue(c => c.Address, stationsExtended[i].Address);
                                    }

                                    if (readerStationExtendedFrom.GetValue(c => c.BandWidth) != stationsExtended[i].BandWidth)
                                    {
                                        isChanged = true;
                                        builderUpdateStationExtended.SetValue(c => c.BandWidth, stationsExtended[i].BandWidth);
                                    }

                                    if (readerStationExtendedFrom.GetValue(c => c.DesigEmission) != stationsExtended[i].DesigEmission)
                                    {
                                        isChanged = true;
                                        builderUpdateStationExtended.SetValue(c => c.DesigEmission, stationsExtended[i].DesigEmission);
                                    }

                                    if (readerStationExtendedFrom.GetValue(c => c.Latitude) != stationsExtended[i].Location.Latitude)
                                    {
                                        isChanged = true;
                                        builderUpdateStationExtended.SetValue(c => c.Latitude, stationsExtended[i].Location.Latitude);
                                    }

                                    if (readerStationExtendedFrom.GetValue(c => c.Longitude) != stationsExtended[i].Location.Longitude)
                                    {
                                        isChanged = true;
                                        builderUpdateStationExtended.SetValue(c => c.Longitude, stationsExtended[i].Location.Longitude);
                                    }

                                    if (readerStationExtendedFrom.GetValue(c => c.OwnerName) != stationsExtended[i].OwnerName)
                                    {
                                        isChanged = true;
                                        builderUpdateStationExtended.SetValue(c => c.OwnerName, stationsExtended[i].OwnerName);
                                    }

                                    if (readerStationExtendedFrom.GetValue(c => c.PermissionNumber) != stationsExtended[i].PermissionNumber)
                                    {
                                        isChanged = true;
                                        builderUpdateStationExtended.SetValue(c => c.PermissionNumber, stationsExtended[i].PermissionNumber);
                                    }

                                    if (readerStationExtendedFrom.GetValue(c => c.PermissionStart) != stationsExtended[i].PermissionStart)
                                    {
                                        isChanged = true;
                                        builderUpdateStationExtended.SetValue(c => c.PermissionStart, stationsExtended[i].PermissionStart);
                                    }

                                    if (readerStationExtendedFrom.GetValue(c => c.PermissionStop) != stationsExtended[i].PermissionStop)
                                    {
                                        isChanged = true;
                                        builderUpdateStationExtended.SetValue(c => c.PermissionStop, stationsExtended[i].PermissionStop);
                                    }

                                    if (readerStationExtendedFrom.GetValue(c => c.Province) != stationsExtended[i].Province)
                                    {
                                        isChanged = true;
                                        builderUpdateStationExtended.SetValue(c => c.Province, stationsExtended[i].Province);
                                    }

                                    if (readerStationExtendedFrom.GetValue(c => c.DocNum) != stationsExtended[i].DocNum)
                                    {
                                        isChanged = true;
                                        builderUpdateStationExtended.SetValue(c => c.DocNum, stationsExtended[i].DocNum);
                                    }

                                    if (readerStationExtendedFrom.GetValue(c => c.StatusMeas) != stationsExtended[i].StatusMeas)
                                    {
                                        isChanged = true;
                                        builderUpdateStationExtended.SetValue(c => c.StatusMeas, stationsExtended[i].StatusMeas);
                                    }

                                    if (readerStationExtendedFrom.GetValue(c => c.CurentStatusStation) != stationsExtended[i].CurentStatusStation)
                                    {
                                        isChanged = true;
                                        builderUpdateStationExtended.SetValue(c => c.CurentStatusStation, stationsExtended[i].CurentStatusStation);
                                    }

                                    var permissionGlobalSID = GetGlobalSID(stationsExtended[i].OKPO, stationsExtended[i].StationName);
                                    if (readerStationExtendedFrom.GetValue(c => c.PermissionGlobalSID) != permissionGlobalSID)
                                    {
                                        if (!string.IsNullOrEmpty(permissionGlobalSID))
                                        {
                                            isChanged = true;
                                            builderUpdateStationExtended.SetValue(c => c.PermissionGlobalSID, permissionGlobalSID);
                                        }
                                    }

                                    if (readerStationExtendedFrom.GetValue(c => c.TestStartDate) != stationsExtended[i].TestStartDate)
                                    {
                                        isChanged = true;
                                        builderUpdateStationExtended.SetValue(c => c.TestStartDate, stationsExtended[i].TestStartDate);
                                    }

                                    if (readerStationExtendedFrom.GetValue(c => c.TestStopDate) != stationsExtended[i].TestStopDate)
                                    {
                                        isChanged = true;
                                        builderUpdateStationExtended.SetValue(c => c.TestStopDate, stationsExtended[i].TestStopDate);
                                    }

                                    if (readerStationExtendedFrom.GetValue(c => c.StationTxFreq) != stationsExtended[i].StationTxFreq)
                                    {
                                        isChanged = true;
                                        builderUpdateStationExtended.SetValue(c => c.StationTxFreq, stationsExtended[i].StationTxFreq);
                                    }

                                    if (readerStationExtendedFrom.GetValue(c => c.StationRxFreq) != stationsExtended[i].StationRxFreq)
                                    {
                                        isChanged = true;
                                        builderUpdateStationExtended.SetValue(c => c.StationRxFreq, stationsExtended[i].StationRxFreq);
                                    }

                                    if (readerStationExtendedFrom.GetValue(c => c.StationChannel) != stationsExtended[i].StationChannel)
                                    {
                                        isChanged = true;
                                        builderUpdateStationExtended.SetValue(c => c.StationChannel, stationsExtended[i].StationChannel);
                                    }

                                    if (readerStationExtendedFrom.GetValue(c => c.OKPO) != stationsExtended[i].OKPO)
                                    {
                                        isChanged = true;
                                        builderUpdateStationExtended.SetValue(c => c.OKPO, stationsExtended[i].OKPO);
                                    }

                                    if (readerStationExtendedFrom.GetValue(c => c.StationName) != stationsExtended[i].StationName)
                                    {
                                        isChanged = true;
                                        builderUpdateStationExtended.SetValue(c => c.StationName, stationsExtended[i].StationName);
                                    }

                                    if (readerStationExtendedFrom.GetValue(c => c.PermissionCancelDate) != stationsExtended[i].PermissionCancelDate)
                                    {
                                        isChanged = true;
                                        builderUpdateStationExtended.SetValue(c => c.PermissionCancelDate, stationsExtended[i].PermissionCancelDate);
                                    }


                                    if (isChanged)
                                    {
                                        scope.Executor.Execute(builderUpdateStationExtended);
                                    }

                                    break;
                                }
                            }
                            return true;
                        });


                        if (isFindIdentifierFromICSM == false)
                        {
                            var isChangedSuccess = false;
                            var builderStationExtendedInsert = this._dataLayer.GetBuilder<MD.IStationExtended>().Insert();
                            if (stationsExtended[i].Province != null)
                            {
                                builderStationExtendedInsert.SetValue(c => c.Province, stationsExtended[i].Province);
                                isChangedSuccess = true;
                            }
                            if (stationsExtended[i].Address != null)
                            {
                                builderStationExtendedInsert.SetValue(c => c.Address, stationsExtended[i].Address);
                                isChangedSuccess = true;
                            }
                            if (stationsExtended[i].BandWidth != null)
                            {
                                builderStationExtendedInsert.SetValue(c => c.BandWidth, stationsExtended[i].BandWidth);
                                isChangedSuccess = true;
                            }
                            if (stationsExtended[i].DesigEmission != null)
                            {
                                builderStationExtendedInsert.SetValue(c => c.DesigEmission, stationsExtended[i].DesigEmission);
                                isChangedSuccess = true;
                            }
                            if (stationsExtended[i].Location != null)
                            {
                                builderStationExtendedInsert.SetValue(c => c.Latitude, stationsExtended[i].Location.Latitude);
                                builderStationExtendedInsert.SetValue(c => c.Longitude, stationsExtended[i].Location.Longitude);
                                isChangedSuccess = true;
                            }
                            if (stationsExtended[i].OwnerName != null)
                            {
                                builderStationExtendedInsert.SetValue(c => c.OwnerName, stationsExtended[i].OwnerName);
                                isChangedSuccess = true;
                            }
                            if (stationsExtended[i].PermissionNumber != null)
                            {
                                builderStationExtendedInsert.SetValue(c => c.PermissionNumber, stationsExtended[i].PermissionNumber);
                                isChangedSuccess = true;
                            }
                            if (stationsExtended[i].PermissionStart != null)
                            {
                                builderStationExtendedInsert.SetValue(c => c.PermissionStart, stationsExtended[i].PermissionStart);
                                isChangedSuccess = true;
                            }
                            if (stationsExtended[i].PermissionStop != null)
                            {
                                builderStationExtendedInsert.SetValue(c => c.PermissionStop, stationsExtended[i].PermissionStop);
                                isChangedSuccess = true;
                            }
                            if (stationsExtended[i].Standard != null)
                            {
                                builderStationExtendedInsert.SetValue(c => c.Standard, stationsExtended[i].Standard);
                                isChangedSuccess = true;
                            }
                            if (stationsExtended[i].StandardName != null)
                            {
                                builderStationExtendedInsert.SetValue(c => c.StandardName, stationsExtended[i].StandardName);
                                isChangedSuccess = true;
                            }
                            if (stationsExtended[i].TableName != null)
                            {
                                builderStationExtendedInsert.SetValue(c => c.TableName, stationsExtended[i].TableName);
                                isChangedSuccess = true;
                            }
                            if (stationsExtended[i].DocNum != null)
                            {
                                builderStationExtendedInsert.SetValue(c => c.DocNum, stationsExtended[i].DocNum);
                                isChangedSuccess = true;
                            }
                            if (stationsExtended[i].TestStartDate != null)
                            {
                                builderStationExtendedInsert.SetValue(c => c.TestStartDate, stationsExtended[i].TestStartDate);
                                isChangedSuccess = true;
                            }
                            if (stationsExtended[i].TestStopDate != null)
                            {
                                builderStationExtendedInsert.SetValue(c => c.TestStopDate, stationsExtended[i].TestStopDate);
                                isChangedSuccess = true;
                            }
                            if (stationsExtended[i].PermissionCancelDate != null)
                            {
                                builderStationExtendedInsert.SetValue(c => c.PermissionCancelDate, stationsExtended[i].PermissionCancelDate);
                                isChangedSuccess = true;
                            }

                            var permissionGlobalSID = GetGlobalSID(stationsExtended[i].OKPO, stationsExtended[i].StationName);
                            if (!string.IsNullOrEmpty(permissionGlobalSID))
                            {
                                builderStationExtendedInsert.SetValue(c => c.PermissionGlobalSID, permissionGlobalSID);
                                isChangedSuccess = true;
                            }

                            if (stationsExtended[i].StationTxFreq != null)
                            {
                                builderStationExtendedInsert.SetValue(c => c.StationTxFreq, stationsExtended[i].StationTxFreq);
                                isChangedSuccess = true;
                            }
                            if (stationsExtended[i].StationRxFreq != null)
                            {
                                builderStationExtendedInsert.SetValue(c => c.StationRxFreq, stationsExtended[i].StationRxFreq);
                                isChangedSuccess = true;
                            }
                            if (stationsExtended[i].StationChannel != null)
                            {
                                builderStationExtendedInsert.SetValue(c => c.StationChannel, stationsExtended[i].StationChannel);
                                isChangedSuccess = true;
                            }
                            //if (stationsExtended[i].StatusMeas != null)
                            //{
                            //builderStationExtendedInsert.SetValue(c => c.StatusMeas, stationsExtended[i].StatusMeas);
                            //isChangedSuccess = true;
                            //}
                            builderStationExtendedInsert.SetValue(c => c.StatusMeas, "N");
                            isChangedSuccess = true;

                            if (stationsExtended[i].CurentStatusStation != null)
                            {
                                builderStationExtendedInsert.SetValue(c => c.CurentStatusStation, stationsExtended[i].CurentStatusStation);
                                isChangedSuccess = true;
                            }

                            if (stationsExtended[i].OKPO != null)
                            {
                                builderStationExtendedInsert.SetValue(c => c.OKPO, stationsExtended[i].OKPO);
                                isChangedSuccess = true;
                            }

                            if (stationsExtended[i].StationName != null)
                            {
                                builderStationExtendedInsert.SetValue(c => c.StationName, stationsExtended[i].StationName);
                                isChangedSuccess = true;
                            }




                            builderStationExtendedInsert.SetValue(c => c.TableId, stationsExtended[i].TableId);

                            if (isChangedSuccess)
                            {
                                var stationExtendedId = scope.Executor.Execute<MD.IStationExtended_PK>(builderStationExtendedInsert);
                                if (stationExtendedId != null)
                                {
                                    stationsExtended[i].Id = stationExtendedId.Id;
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

        /// <summary>
        /// Создание нового процесса синхронизации эмиттингов
        /// </summary>
        /// <param name="dataSynchronization"></param>
        /// <param name="headRefSpectrumIdsBySDRN"></param>
        /// <param name="sensorIdsBySDRN"></param>
        /// <param name="areas"></param>
        /// <returns></returns>
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

                    var lstHeadRef = headRefSpectrumIdsBySDRN.ToList();
                    for (int k = 0; k < headRefSpectrumIdsBySDRN.Length; k++)
                    {
                        bool isFindHeadRefSpectrumInDB = false;
                        var queryHeadRefSpectrumFrom = this._dataLayer.GetBuilder<MD.IHeadRefSpectrum>()
                        .From()
                        .Select(c => c.Id)
                        .Where(c => c.Id, ConditionOperator.Equal, headRefSpectrumIdsBySDRN[k]);
                        scope.Executor.Fetch(queryHeadRefSpectrumFrom, readerHeadRefSpectrum =>
                        {
                            while (readerHeadRefSpectrum.Read())
                            {
                                isFindHeadRefSpectrumInDB = true;
                            }
                            return true;
                        });

                        if (isFindHeadRefSpectrumInDB)
                        {
                            var builderLinkHeadRefSpectrumInsert = this._dataLayer.GetBuilder<MD.ILinkHeadRefSpectrum>().Insert();
                            builderLinkHeadRefSpectrumInsert.SetValue(c => c.HEAD_REF_SPECTRUM.Id, headRefSpectrumIdsBySDRN[k]);
                            builderLinkHeadRefSpectrumInsert.SetValue(c => c.SYNCHRO_PROCESS.Id, synchroProcessInsertId);
                            var builderLinkHeadRefSpectrumInsertIdValue = scope.Executor.Execute<MD.ILinkHeadRefSpectrum_PK>(builderLinkHeadRefSpectrumInsert);
                        }
                        else
                        {
                            lstHeadRef.Remove(headRefSpectrumIdsBySDRN[k]);
                        }
                    }

                    headRefSpectrumIdsBySDRN = lstHeadRef.ToArray();

                    for (int k = 0; k < sensorIdsBySDRN.Length; k++)
                    {
                        var builderLinkSensorsWithSynchroProcessInsert = this._dataLayer.GetBuilder<MD.ILinkSensorsWithSynchroProcess>().Insert();
                        builderLinkSensorsWithSynchroProcessInsert.SetValue(c => c.SensorId, sensorIdsBySDRN[k]);
                        builderLinkSensorsWithSynchroProcessInsert.SetValue(c => c.SYNCHRO_PROCESS.Id, synchroProcessInsertId);
                        var builderLinkSensorsWithSynchroProcessInsertIdValue = scope.Executor.Execute<MD.ILinkSensorsWithSynchroProcess_PK>(builderLinkSensorsWithSynchroProcessInsert);
                    }

                    for (int k = 0; k < areas.Length; k++)
                    {
                        if (GetAreaIdByICSMId(areas[k].IdentifierFromICSM) != null)
                        {
                            var builderLinkAreaInsert = this._dataLayer.GetBuilder<MD.ILinkArea>().Insert();
                            builderLinkAreaInsert.SetValue(c => c.AREA.Id, GetAreaIdByICSMId(areas[k].IdentifierFromICSM));
                            builderLinkAreaInsert.SetValue(c => c.SYNCHRO_PROCESS.Id, synchroProcessInsertId);
                            var builderLinkAreaInsertIdValue = scope.Executor.Execute<MD.ILinkArea_PK>(builderLinkAreaInsert);
                        }
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

        /// <summary>
        /// Извлечение идентфикатора таблицы Area по значению поля IdentifierFromICSM
        /// </summary>
        /// <param name="IdentifierFromICSM"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Извлечение перечня координат по заданным areas
        /// </summary>
        /// <param name="areas"></param>
        /// <returns></returns>
        public List<DataLocation[]> GetPolygonFromArea(Area[] areas)
        {
            var queryExecuter = this._dataLayer.Executor<SdrnServerDataContext>();
            var polygonAll = new List<DataLocation[]>();
            for (int i = 0; i < areas.Length; i++)
            {
                var polygon = new List<DataLocation>();
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

      

        /// <summary>
        /// Метод, выполняющий "прорежение" RefSpectrum в БД
        /// </summary>
        /// <param name="dataSynchronization"></param>
        /// <param name="headRefSpectrumIdsBySDRN"></param>
        /// <param name="sensorIdsBySDRN"></param>
        /// <param name="areas"></param>
        /// <param name="stationsExtended"></param>
        /// <returns></returns>
        public bool DeleteDuplicateRefSpectrumRecords(DataSynchronizationBase dataSynchronization, long[] headRefSpectrumIdsBySDRN, long[] sensorIdsBySDRN, Area[] areas, StationExtended[] stationsExtended, ref List<RefSpectrum> refSpectrums)
        {
            bool isSuccess = false;
            try
            {
                this._logger.Info(Contexts.ThisComponent, Categories.Processing, Events.DeleteDuplicateRefSpectrumRecords.Text);
                var listDataRefSpectrum = new List<DataRefSpectrum>();
                var listDataRefSpectrumForDelete = new List<DataRefSpectrum>();
                var utils = new Utils(this._dataLayer, this._logger);
                utils.DeleteLinkSensors(sensorIdsBySDRN, dataSynchronization.Id.Value);
                //utils.DeleteRefSpectrumBySensorId(sensorIdsBySDRN, headRefSpectrumIdsBySDRN);

                var listRefSpectrum = utils.GetRefSpectrumByIds(headRefSpectrumIdsBySDRN, sensorIdsBySDRN);

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

                var lstStations = stationsExtended.ToList();
                var fndorderByDateMeas = from z in listDataRefSpectrum orderby z.DateMeas descending select z;
                listDataRefSpectrum = fndorderByDateMeas.ToList();
                for (int h = 0; h < listDataRefSpectrum.Count; h++)
                {
                    var fndStation = lstStations.Find(z => z.TableId == listDataRefSpectrum[h].TableId && z.TableName == listDataRefSpectrum[h].TableName);
                    if (fndStation != null)
                    {
                        listDataRefSpectrum[h].StatusMeas = CalcStatus.CalcStatusMeasForRefSpectrum(fndStation.PermissionCancelDate, fndStation.PermissionStop, fndStation.PermissionStart, fndStation.DocNum, fndStation.TestStartDate, fndStation.TestStopDate, listDataRefSpectrum[h].DateMeas);
                        utils.UpdateStatusRefSpectrum(listDataRefSpectrum[h]);
                        for (int i = 0; i < listRefSpectrum.Length; i++)
                        {
                            var refSpectrum = listRefSpectrum[i];
                            if (refSpectrum.DataRefSpectrum != null)
                            {
                                for (int k = 0; k < refSpectrum.DataRefSpectrum.Length; k++)
                                {
                                    if (refSpectrum.DataRefSpectrum[k].Id == listDataRefSpectrum[h].Id)
                                    {
                                        refSpectrum.DataRefSpectrum[k].StatusMeas = listDataRefSpectrum[h].StatusMeas;
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }

                // удаляются записи, которые повторяться (т.е. одинаковые параметры Table ICSM Name, ID ICSM, ID Sensor, Global SID, Freq MHz) при этом остаётся только последняя по времени запись.
                for (int h = 0; h < listDataRefSpectrum.Count; h++)
                {
                    var findData = listDataRefSpectrum.FindAll(x => x.TableId == listDataRefSpectrum[h].TableId && x.TableName == listDataRefSpectrum[h].TableName && x.SensorId == listDataRefSpectrum[h].SensorId && x.GlobalSID == listDataRefSpectrum[h].GlobalSID && x.Freq_MHz == listDataRefSpectrum[h].Freq_MHz && (x.StatusMeas == "U" || x.StatusMeas == "N"));
                    if (findData != null)
                    {
                        var orderByDateMeas = from z in findData orderby z.HeadId descending select z;
                        if ((orderByDateMeas != null) && (orderByDateMeas.Count() > 1))
                        {
                            var fndValorderByDateMeas = from z in orderByDateMeas orderby z.DateMeas descending select z;
                            var orderByDateMeasArray = fndValorderByDateMeas.ToArray();
                            for (int g = 1; g < orderByDateMeasArray.Length; g++)
                            {
                                if (!listDataRefSpectrumForDelete.Contains(orderByDateMeasArray[g]))
                                {
                                    listDataRefSpectrumForDelete.Add(orderByDateMeasArray[g]);
                                }
                            }
                        }
                    }
                }

                // Записи, дата которых (Date Meas) находиться за пределами даты начала отчета, даты конца отчета удаляются 
                var findDataRep = listDataRefSpectrum.FindAll(x => (x.DateMeas >= dataSynchronization.DateStart && x.DateMeas <= dataSynchronization.DateEnd) == false);
                if (findDataRep != null)
                {
                    if ((findDataRep != null) && (findDataRep.Count > 0))
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
                        var checkLocation = new CheckLocation(fndStation.Location.Longitude, fndStation.Location.Latitude);
                        bool isCheckedLocation = false;
                        for (int f = 0; f < lstPolygons.Count; f++)
                        {
                            if ((lstPolygons[f] != null) && (lstPolygons[f].Length > 0))
                            {
                                var listPolyg = lstPolygons[f].ToList();
                                if (checkLocation.CheckHitting(listPolyg))
                                {
                                    isCheckedLocation = true;
                                    break;
                                }
                            }
                        }
                        if ((isCheckedLocation == false) && (lstPolygons.Count > 0))
                        {
                            var fnd = listDataRefSpectrumForDelete.Find(z => z.TableId == listDataRefSpectrum[h].TableId && z.TableName == listDataRefSpectrum[h].TableName);
                            if (fnd == null)
                            {
                                listDataRefSpectrumForDelete.Add(listDataRefSpectrum[h]);
                            }
                        }
                    }
                }

                var arr = listDataRefSpectrumForDelete.ToArray();
                if ((arr != null) && (arr.Length > 0))
                {
                    //for (int i = 0; i < arr.Length; i++)
                    //{
                        //listDataRefSpectrum.RemoveAll(x => x.Id == arr[i].Id);
                    //}

                    var delIndex = new List<long?>();
                    for (int i = 0; i < listRefSpectrum.Length; i++)
                    {
                        var refSpectrum = listRefSpectrum[i];
                        if (refSpectrum.DataRefSpectrum != null)
                        {
                            for (int j = 0; j < arr.Length; j++)
                            {
                                var tempRefSpectrumDataRefSpectrum = refSpectrum.DataRefSpectrum.ToList();
                                tempRefSpectrumDataRefSpectrum.RemoveAll(x => x.Id == arr[j].Id);
                                refSpectrum.DataRefSpectrum = tempRefSpectrumDataRefSpectrum.ToArray();
                                if (refSpectrum.DataRefSpectrum.Length == 0)
                                {
                                    delIndex.Add(refSpectrum.Id);
                                }
                            }
                        }
                        listRefSpectrum[i] = refSpectrum;
                    }
                    var lst = listRefSpectrum.ToList();
                    for (int i = 0; i < delIndex.Count; i++)
                    {
                        lst.RemoveAll(x => x.Id == delIndex[i]);
                    }
                }

                refSpectrums = listRefSpectrum.ToList();
                // непосредственное удаление записей из БД
                /*
                var arr = listDataRefSpectrumForDelete.ToArray();
                if ((arr != null) && (arr.Length > 0))
                {
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
                }
                */

                //utils.RemoveEmptyHeadRefSpectrum(headRefSpectrumIdsBySDRN, sensorIdsBySDRN);
                isSuccess = true;
            }
            catch (Exception e)
            {
                isSuccess = false;
                this._logger.Exception(Contexts.ThisComponent, e);
            }
            return isSuccess;
        }

        /// <summary>
        /// Извлечение групп сенсоров по переданному массиву RefSpectrum
        /// </summary>
        /// <param name="refSpectrums"></param>
        /// <returns></returns>
        public GroupSensors[] GetGroupSensors(RefSpectrum[] refSpectrums)
        {
            this._logger.Info(Contexts.ThisComponent, Categories.Processing, Events.GetGroupSensors.Text);
            var dataGroupSensors = new List<GroupSensors>();
            for (int i = 0; i < refSpectrums.Length; i++)
            {
                for (int j = 0; j < refSpectrums[i].DataRefSpectrum.Length; j++)
                {
                    var dataRefSpectrum = refSpectrums[i].DataRefSpectrum[j];
                    var fndValue = dataGroupSensors.Find(v => v.SensorId == dataRefSpectrum.SensorId && v.Freq_MHz == dataRefSpectrum.Freq_MHz);
                    if (fndValue == null)
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

        /// <summary>
        /// Извлечение только RefSpectrum,  которые соответсвуют группе GroupSensors
        /// </summary>
        /// <param name="group"></param>
        /// <param name="refSpectrums"></param>
        /// <returns></returns>
        public RefSpectrum[] SelectGroupSensor(GroupSensors group, RefSpectrum[] refSpectrums)
        {
            this._logger.Info(Contexts.ThisComponent, Categories.Processing, Events.SelectGroupSensor.Text);
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
                    if ((dataRefSpectrum.SensorId == group.SensorId) && (dataRefSpectrum.Freq_MHz == group.Freq_MHz))
                    {
                        lstDataRefSpectrums.Add(new DataRefSpectrum()
                        {
                            DateMeas = dataRefSpectrum.DateMeas,
                            DispersionLow = dataRefSpectrum.DispersionLow,
                            DispersionUp = dataRefSpectrum.DispersionUp,
                            Freq_MHz = dataRefSpectrum.Freq_MHz,
                            GlobalSID = dataRefSpectrum.GlobalSID,
                            Id = dataRefSpectrum.Id,
                            IdNum = dataRefSpectrum.IdNum,
                            Level_dBm = dataRefSpectrum.Level_dBm,
                            Percent = dataRefSpectrum.Percent,
                            SensorId = dataRefSpectrum.SensorId,
                            TableId = dataRefSpectrum.TableId,
                            TableName = dataRefSpectrum.TableName,
                            StatusMeas = dataRefSpectrum.StatusMeas,
                            HeadId = dataRefSpectrum.HeadId
                        });
                    }
                }
                RefSpectrum.DataRefSpectrum = lstDataRefSpectrums.ToArray();
                if (lstDataRefSpectrums.Count > 0)
                {
                    listDataRefSpectrum.Add(RefSpectrum);
                }
            }
            return listDataRefSpectrum.ToArray();
        }
        //

        /// <summary>
        /// Генерация PermissionGlobalSID по заданному значению okpo и stationName
        /// </summary>
        /// <param name="okpo"></param>
        /// <param name="stationName"></param>
        /// <returns></returns>
        public static string GetGlobalSID(string okpo, string stationName)
        {
            if (!string.IsNullOrEmpty(stationName))
            {
                string CodeOwener = "0";
                if (okpo == "14333937") { CodeOwener = "1"; };
                if (okpo == "22859846") { CodeOwener = "6"; };
                if (okpo == "21673832") { CodeOwener = "3"; };
                if (okpo == "37815221") { CodeOwener = "7"; };
                return "255 " + CodeOwener + " 00000 " + string.Format("{0:00000}", stationName);
            }
            else return "";
        }

        /// <summary>
        /// Вычисление числа уникальных RefSpetrum по полю "GlobalSID"
        /// </summary>
        /// <param name="refSpectrums"></param>
        /// <returns></returns>
        private int CountUniqueStations(RefSpectrum[] refSpectrums)
        {
            this._logger.Info(Contexts.ThisComponent, Categories.Processing, Events.CountUniqueStations.Text);
            var uniqueStations = new List<string>();
            for (int i = 0; i < refSpectrums.Length; i++)
            {
                for (int j = 0; j < refSpectrums[i].DataRefSpectrum.Length; j++)
                {
                    if (!uniqueStations.Exists(x => x.Equals(refSpectrums[i].DataRefSpectrum[j].GlobalSID)))
                    {
                        uniqueStations.Add(refSpectrums[i].DataRefSpectrum[j].GlobalSID);
                    }
                }
            }

            return uniqueStations.Count;
        }

        private List<StationDataToSort> FillStationDataToCorrespond(RefSpectrum[] refSpectrums)
        {
            this._logger.Info(Contexts.ThisComponent, Categories.Processing, Events.FillStationDataToCorrespond.Text);
            var staionsDataToCorrespond = new List<StationDataToSort>();
            StationDataToSort stationDataToCorrespond = new StationDataToSort();
            for (int i = 0; i < refSpectrums.Length; i++)
            {
                for (int j = 0; j < refSpectrums[i].DataRefSpectrum.Length; j++)
                {
                    stationDataToCorrespond.RefSpectrumId = refSpectrums[i].Id;
                    stationDataToCorrespond.DataRefSpectrumId = refSpectrums[i].DataRefSpectrum[j].Id;
                    stationDataToCorrespond.Freq_MHz = refSpectrums[i].DataRefSpectrum[j].Freq_MHz;
                    stationDataToCorrespond.GlobalSID = refSpectrums[i].DataRefSpectrum[j].GlobalSID;
                    stationDataToCorrespond.Level_dBm = refSpectrums[i].DataRefSpectrum[j].Level_dBm;
                    staionsDataToCorrespond.Add(stationDataToCorrespond);
                }
            }

            return staionsDataToCorrespond;
        }

        private Emitting[] DeleteUnestimatedEmittings(Emitting[] emittings)
        {
            this._logger.Info(Contexts.ThisComponent, Categories.Processing, Events.DeleteUnestimatedEmittings.Text);
            var estimatedEmitting = new List<Emitting>();
            for (int i = 0; i < emittings.Length; i++)
            {
                if (emittings[i].Spectrum.CorrectnessEstimations)
                {
                    estimatedEmitting.Add(emittings[i]);
                }
            }
            return estimatedEmitting.ToArray();
        }

        private List<EmittingDataToSort> FillEmittingDataToCorrespond(Calculation.Emitting[] emittings)
        {
            this._logger.Info(Contexts.ThisComponent, Categories.Processing, Events.FillEmittingDataToCorrespond.Text);
            var emittingsDataToCorrespond = new List<EmittingDataToSort>();
            EmittingDataToSort emittingDataToCorrespond = new EmittingDataToSort();
            for (int i = 0; i < emittings.Length; i++)
            {
                emittingDataToCorrespond.Id = emittings[i].Id;
                emittingDataToCorrespond.StartFrequency_MHz = emittings[i].StartFrequency_MHz;
                emittingDataToCorrespond.StopFrequency_MHz = emittings[i].StopFrequency_MHz;
                emittingDataToCorrespond.CurrentPower_dBm = emittings[i].CurentPower_dBm;
                for (int j = 0; j < emittings[i].WorkTimes.Length; j++)
                {
                    emittingDataToCorrespond.worktimeHitsCount += emittings[i].WorkTimes[j].HitCount;
                }
                emittingsDataToCorrespond.Add(emittingDataToCorrespond);
            }
            return emittingsDataToCorrespond;
        }

        private bool CompareEmittingSpectrums(Spectrum spectrumA, Spectrum spectrumB)
        {
            if ((spectrumA.SpectrumStartFreq_MHz == spectrumB.SpectrumStartFreq_MHz) && (spectrumA.Levels_dBm.Length == spectrumB.Levels_dBm.Length))
            {
                float levelsDifference = 0;
                for (int k = 0; k < spectrumA.Levels_dBm.Length; k++)
                {
                    levelsDifference += spectrumA.Levels_dBm[k] - spectrumB.Levels_dBm[k];
                    if (levelsDifference < 0.000001) break;
                }
                if (levelsDifference < 0.000001)
                {
                    return true;
                    //break;
                }
            }
            return false;
        }

        private void FillProtocolsDataWithOutEmittings(RefSpectrum[] refSpectrums, ref List<Protocols> lstProtocols)
        {
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

                    var foundProtocol = lstProtocols.Find(x => x.DataRefSpectrum.Id == dataRefSpectrum.Id);

                    if (foundProtocol == null) 
                    {
                        lstProtocols.Add(protocol);
                    }
                }
            }
        }
        

        private void FillProtocolsDataWithEmittings(RefSpectrum[] refSpectrums, StationDataToSort[] stationsDataToCorrespond, EmittingDataToSort[] emittingsDataToCorrespond, Emitting[] emittings, ref List<Protocols> lstProtocols)
        {
            var cntAllEmitters = 0;
            if ((emittings != null) && (emittings.Length > 0))
            {
                cntAllEmitters = emittings.Length;
            }

            for (int i = 0; i < refSpectrums.Length; i++)
            {
                var RefSpectrum = new RefSpectrum();
                for (int j = 0; j < refSpectrums[i].DataRefSpectrum.Length; j++)
                {
                    // получение очередного значения dataRefSpectrum
                    var dataRefSpectrum = refSpectrums[i].DataRefSpectrum[j];

                    // объявление новой промежуточной переменной, которая должна заполняться в блоке ниже:
                    int stationLink = -1;
                    for (int k = 0; k < stationsDataToCorrespond.Length; k++)
                    {
                        if (stationsDataToCorrespond[k].RefSpectrumId == refSpectrums[i].Id && stationsDataToCorrespond[k].DataRefSpectrumId == refSpectrums[i].DataRefSpectrum[j].Id)
                        {
                            stationLink = k;
                            break;
                        }
                    }
                    if (stationLink >= 0)
                    {
                        if ((stationLink > (cntAllEmitters - 1)) == false)
                        {
                            //continue;

                            var corrEmittingId = emittingsDataToCorrespond[stationLink].Id;//////////

                            var emitFnd = emittings.ToList();
                            var findValueIndex = emitFnd.FindIndex(x => x.Id == corrEmittingId);
                            if (findValueIndex >= 0)
                            {
                                // обявляем очередной экземпляр переменной Protocols
                                var protocol = new Protocols();
                                protocol.DataRefSpectrum = new DataRefSpectrum();
                                // копируем данные с переменной dataRefSpectrum
                                protocol.DataRefSpectrum = dataRefSpectrum;
                                // если связь с Emitting обнаружена - тогда необходимо заполнить свосйтво ProtocolsLinkedWithEmittings:
                                protocol.ProtocolsLinkedWithEmittings = new ProtocolsWithEmittings();
                                if (emittings[findValueIndex].LevelsDistribution != null)
                                {
                                    protocol.ProtocolsLinkedWithEmittings.Count = emittings[findValueIndex].LevelsDistribution.Count;
                                    protocol.ProtocolsLinkedWithEmittings.Levels = emittings[findValueIndex].LevelsDistribution.Levels;
                                }
                                protocol.ProtocolsLinkedWithEmittings.CurentPower_dBm = emittings[findValueIndex].CurentPower_dBm;
                                if (emittings[findValueIndex].SignalMask != null)
                                {
                                    protocol.ProtocolsLinkedWithEmittings.Freq_kHz = emittings[findValueIndex].SignalMask.Freq_kHz;
                                    protocol.ProtocolsLinkedWithEmittings.Loss_dB = emittings[findValueIndex].SignalMask.Loss_dB;
                                }
                                protocol.ProtocolsLinkedWithEmittings.MeanDeviationFromReference = emittings[findValueIndex].MeanDeviationFromReference;
                                //protocol.ProtocolsLinkedWithEmittings.Probability = здесь заполнить вероятность;
                                protocol.ProtocolsLinkedWithEmittings.ReferenceLevel_dBm = emittings[findValueIndex].ReferenceLevel_dBm;
                                if (emittings[findValueIndex].EmittingParameters != null)
                                {
                                    protocol.ProtocolsLinkedWithEmittings.RollOffFactor = emittings[findValueIndex].EmittingParameters.RollOffFactor;
                                    protocol.ProtocolsLinkedWithEmittings.StandardBW = emittings[findValueIndex].EmittingParameters.StandardBW;
                                }
                                protocol.ProtocolsLinkedWithEmittings.StartFrequency_MHz = emittings[findValueIndex].StartFrequency_MHz;
                                protocol.ProtocolsLinkedWithEmittings.StopFrequency_MHz = emittings[findValueIndex].StopFrequency_MHz;
                                if (emittings[findValueIndex].Spectrum != null)
                                {
                                    protocol.ProtocolsLinkedWithEmittings.SignalLevel_dBm = emittings[findValueIndex].Spectrum.SignalLevel_dBm;
                                    protocol.ProtocolsLinkedWithEmittings.SpectrumStartFreq_MHz = emittings[findValueIndex].Spectrum.SpectrumStartFreq_MHz;
                                    protocol.ProtocolsLinkedWithEmittings.SpectrumSteps_kHz = emittings[findValueIndex].Spectrum.SpectrumSteps_kHz;
                                    protocol.ProtocolsLinkedWithEmittings.Levels_dBm = emittings[findValueIndex].Spectrum.Levels_dBm;
                                    protocol.ProtocolsLinkedWithEmittings.MarkerIndex = emittings[findValueIndex].Spectrum.MarkerIndex;
                                    protocol.ProtocolsLinkedWithEmittings.Bandwidth_kHz = emittings[findValueIndex].Spectrum.Bandwidth_kHz;
                                    protocol.ProtocolsLinkedWithEmittings.Contravention = emittings[findValueIndex].Spectrum.Contravention;
                                    protocol.ProtocolsLinkedWithEmittings.CorrectnessEstimations = emittings[findValueIndex].Spectrum.CorrectnessEstimations;
                                    protocol.ProtocolsLinkedWithEmittings.T1 = emittings[findValueIndex].Spectrum.T1;
                                    protocol.ProtocolsLinkedWithEmittings.T2 = emittings[findValueIndex].Spectrum.T2;
                                    protocol.ProtocolsLinkedWithEmittings.TraceCount = emittings[findValueIndex].Spectrum.TraceCount;
                                }
                                protocol.ProtocolsLinkedWithEmittings.TriggerDeviationFromReference = emittings[findValueIndex].TriggerDeviationFromReference;

                                if ((emittings[findValueIndex].WorkTimes != null) && (emittings[findValueIndex].WorkTimes.Length > 0))
                                {
                                    var workTimes = emittings[findValueIndex].WorkTimes.ToList();
                                    var minStart = workTimes.Min(z => z.StartEmitting);
                                    var maxStop = workTimes.Min(z => z.StopEmitting);
                                    protocol.ProtocolsLinkedWithEmittings.WorkTimeStart = minStart;
                                    protocol.ProtocolsLinkedWithEmittings.WorkTimeStop = maxStop;
                                }

                                var foundProtocol = lstProtocols.Find(x => x.DataRefSpectrum.Id == dataRefSpectrum.Id);

                                if (foundProtocol == null)
                                {
                                    lstProtocols.Add(protocol);
                                }
                            }
                        }
                    }
                }
            }
        }


        /// <summary>
        /// Синхронизация излучений с записями группы сенсора
        /// </summary>
        /// <param name="refSpectrums">массив RefSpectrum, который соответствует группе сенсора groupSensor</param>
        /// <param name="emittings">массив Emitting, соответствующий  группе сенсора groupSensor</param>
        /// <returns></returns>
        public void SynchroEmittings(RefSpectrum[] refSpectrums, Emitting[] emittings, Calculation.EmitParams[] emittingParameters, ref List<Protocols> lstProtocols)
        {
            // ---- СОХРАНИТЬ ИСХОДНЫЕ ЗНАЧЕНИЯ refSpectrums
            List<RefSpectrum> initialRefSpectrums = new List<RefSpectrum>();
            for (int i = 0; i < refSpectrums.Length; i++)
            {
                initialRefSpectrums.Add(refSpectrums[i]);
            }
            List<Emitting> initialEmittings = new List<Emitting>();
            for (int i = 0; i < emittings.Length; i++)
            {
                initialEmittings.Add(emittings[i]);
            }
            List<Spectrum> uncorrespondEmittingsSpectrum = new List<Spectrum>();

            //
            this._logger.Info(Contexts.ThisComponent, Categories.Processing, Events.SynchroEmittings.Text);
            
            // count 
            int desiredNumberOfEmittings = CountUniqueStations(refSpectrums);
            DeleteUnestimatedEmittings(emittings);

            var listOfEmitings = ConvertEmittings.ConvertArray(emittings).ToList();

            int StartLevelsForLevelDistribution = -150;
            int NumberPointForLevelDistribution = 200;

            foreach (var emitting in listOfEmitings)
            {
                var levelsDistribution = new Calculation.LevelsDistribution();
                levelsDistribution.Count = new int[NumberPointForLevelDistribution];
                levelsDistribution.Levels = new int[NumberPointForLevelDistribution];
                for (var i = 0; i < NumberPointForLevelDistribution; i++)
                {
                    levelsDistribution.Levels[i] = StartLevelsForLevelDistribution + i;
                    levelsDistribution.Count[i] = 0;
                }

                for (var i = 0; i < levelsDistribution.Levels.Length; i++)
                {
                    for (var j = 0; j < emitting.LevelsDistribution.Levels.Length; j++)
                    {
                        if (levelsDistribution.Levels[i] == emitting.LevelsDistribution.Levels[j])
                        {
                            levelsDistribution.Count[i] = emitting.LevelsDistribution.Count[j];
                        }
                    }
                }
                emitting.LevelsDistribution = levelsDistribution;
            }


            if ((emittingParameters != null) && (emittingParameters.Length > 0))
            {
                var corelFactor = emittingParameters[0].CorrelationFactor.Value;
                Calculation.CalcGroupingEmitting.DeleteRedundantUncorrelatedEmitting(listOfEmitings, desiredNumberOfEmittings, emittingParameters[0], ref corelFactor, 0.95);
            }


            var emittingsDataToCorrespondUnsorted = new List<EmittingDataToSort>();
            emittingsDataToCorrespondUnsorted = FillEmittingDataToCorrespond(listOfEmitings.ToArray());
            var emittingsDataToCorrespondSorted = from z in emittingsDataToCorrespondUnsorted orderby z.worktimeHitsCount descending select z;

            var staionsDataToCorrespondUnsorted = new List<StationDataToSort>();
            staionsDataToCorrespondUnsorted = FillStationDataToCorrespond(refSpectrums);
            var stationsDataToCorrespondSorted = from z in staionsDataToCorrespondUnsorted orderby z.Level_dBm descending select z;

            var stationsDataToCorrespondList = stationsDataToCorrespondSorted.ToList();
            var emittingsDataToCorrespondList = emittingsDataToCorrespondSorted.ToList();

            // Emitting should be correspondent to the level only if difference between emitting pover and station level dont exceeds 30 dB
            int numOfIterations = Math.Min(stationsDataToCorrespondList.Count(), emittingsDataToCorrespondList.Count());
            for (int i = 0; i < numOfIterations; i++)
            {
                var corrEmittingBuffer = new EmittingDataToSort();
                double levelDifference_dB = stationsDataToCorrespondList[i].Level_dBm - emittingsDataToCorrespondList[i].CurrentPower_dBm;
                //возожно следует внести в конфиг, DifferenceBetweenStationAndEmittingLevel_dB = -30
                if (levelDifference_dB < -30)
                {
                    for (int j = i; j < numOfIterations; j++)
                    {
                        levelDifference_dB = stationsDataToCorrespondList[i].Level_dBm - emittingsDataToCorrespondList[j].CurrentPower_dBm;
                        if (levelDifference_dB > -30)
                        {
                            corrEmittingBuffer = emittingsDataToCorrespondList[i];
                            emittingsDataToCorrespondList[i] = emittingsDataToCorrespondList[j];
                            emittingsDataToCorrespondList[j] = corrEmittingBuffer;
                            break;
                        }
                        else if ((levelDifference_dB < -30) && (j == numOfIterations - 1))
                        {
                                                           
                            stationsDataToCorrespondList.RemoveRange(i, 1);
                            numOfIterations = Math.Min(stationsDataToCorrespondList.Count(), emittingsDataToCorrespondList.Count());
                            i--;
                        }
                    }
                }
            }

            // ---- ВАЛИДАЦИЯ ПО СООТВНТСТВИЮ WorkTime (любое измерение, которое приведено как соответствующее, если в соответствие не найден ворктйм - удаляется)
            for (int i = 0; i < numOfIterations; i++)
            {
                long emittingId = emittingsDataToCorrespondList[i].Id.Value;
                long refSpectId = stationsDataToCorrespondList[i].RefSpectrumId.Value;
                long dRefSpectId = stationsDataToCorrespondList[i].DataRefSpectrumId.Value;

                var foundEmitting = emittings.ToList().Find(x => x.Id == emittingId);
                if (foundEmitting != null)
                {
                    var foundRefSpectrum = refSpectrums.ToList().Find(x => x.Id == refSpectId);
                    if (foundRefSpectrum != null)
                    {
                        var foundDRefSpectrum = foundRefSpectrum.DataRefSpectrum.ToList().Find(x => x.Id == dRefSpectId);
                        if (foundDRefSpectrum != null)
                        {
                            for (int j = 0; j < foundEmitting.WorkTimes.Length; j++)
                            {
                                if ((foundDRefSpectrum.DateMeas < foundEmitting.WorkTimes[j].StartEmitting.Date)
                                    || (foundDRefSpectrum.DateMeas > foundEmitting.WorkTimes[j].StopEmitting.Date))
                                {
                                    uncorrespondEmittingsSpectrum.Add(foundEmitting.Spectrum);
                                    stationsDataToCorrespondList.RemoveRange(i, 1);
                                    emittingsDataToCorrespondList.RemoveRange(i, 1);
                                    numOfIterations = Math.Min(stationsDataToCorrespondList.Count(), emittingsDataToCorrespondList.Count());
                                    i--;
                                    break;
                                }
                            }
                        }
                    }
                }
                
            }

            if (stationsDataToCorrespondList.Count > emittingsDataToCorrespondList.Count)
            {
                stationsDataToCorrespondList.RemoveRange(emittingsDataToCorrespondList.Count, stationsDataToCorrespondList.Count - emittingsDataToCorrespondList.Count);
            }
            else if (stationsDataToCorrespondList.Count < emittingsDataToCorrespondList.Count)
            {
                emittingsDataToCorrespondList.RemoveRange(stationsDataToCorrespondList.Count, emittingsDataToCorrespondList.Count - stationsDataToCorrespondList.Count);
            }

            //Convert data to correspond into array
            var stationsDataToCorrespond = stationsDataToCorrespondList.ToArray();
            var emittingsDataToCorrespond = emittingsDataToCorrespondList.ToArray();




            // ---- ПУСКАТЬ ТОЛЬКО ВАЛИДНЫЕ ИЗУЧЕНИЯ !!!
            // Ниже приведен пример цикла, в котором идет последовательная обработка записей RefSpectrum
            if (numOfIterations > 0)
            {
                FillProtocolsDataWithEmittings(refSpectrums, stationsDataToCorrespond, emittingsDataToCorrespond, emittings, ref lstProtocols);

                // ---- Удаление использованных станций
                for (int i = 0; i < initialRefSpectrums.Count; i++)
                {
                    if ((stationsDataToCorrespondList.Count - 1) >= i)
                    {
                        var listDataSpectrum = initialRefSpectrums[i].DataRefSpectrum.ToList();
                        for (int j = 0; j < numOfIterations; j++)
                        {
                            listDataSpectrum.RemoveAll(x => x.Id == stationsDataToCorrespondList[i].DataRefSpectrumId);
                        }
                        initialRefSpectrums[i].DataRefSpectrum = listDataSpectrum.ToArray();
                    }
                }
                    

                // ---- Удаление использованных излучений (Поиск соответствия по спектру)
                for (int i = 0; i < numOfIterations; i++)
                {
                    
                    long emittingId = emittingsDataToCorrespondList[i].Id.Value;
                    for (int j = 0; j < initialEmittings.Count; j++)
                    {
                        var foundEmitting = emittings.ToList().Find(x => x.Id == emittingId);
                        if (foundEmitting != null)
                        {
                            // если спект соответствует - излучения удляются
                            if(CompareEmittingSpectrums(initialEmittings[j].Spectrum, foundEmitting.Spectrum))
                            {
                                initialEmittings.RemoveRange(j, 1);
                                j--;
                                break;
                            }
                                
                            ////
                            //if (initialEmittings[j].Spectrum.Levels_dBm.Length == foundEmitting.Spectrum.Levels_dBm.Length)
                            //{
                            //    float levelsDifference = 0;
                            //    for (int k = 0; k < initialEmittings[j].Spectrum.Levels_dBm.Length; k++)
                            //    {
                            //        levelsDifference += initialEmittings[j].Spectrum.Levels_dBm[k] - foundEmitting.Spectrum.Levels_dBm[k];
                            //        if (levelsDifference != 0) break;
                            //    }
                            //    if (levelsDifference < 0.000001)
                            //    {
                            //        initialEmittings.RemoveRange(j, 1);
                            //        j--;
                            //        break;
                            //    }
                            //}////
                        }
                        
                    }
                }
                // ---- рекурсивный вызов функции длл невалидных соответствий (валидные измерения и излучения убраны)
                SynchroEmittings(initialRefSpectrums.ToArray(), initialEmittings.ToArray(), emittingParameters, ref lstProtocols);
            }
            else if (numOfIterations == 0 && uncorrespondEmittingsSpectrum.Count > 0)
            {
                // ---- Удаление невалидных излучений (Поиск соответствия по спектру)
                for (int i = 0; i < uncorrespondEmittingsSpectrum.Count; i++)
                {
                    for (int j = 0; j < initialEmittings.Count; j++)
                    {
                        // если спект соответствует - излучения удляются
                        if (CompareEmittingSpectrums(initialEmittings[j].Spectrum, uncorrespondEmittingsSpectrum[i]))
                        {
                            initialEmittings.RemoveRange(j, 1);
                            j--;
                            break;
                        }
                    }
                }
                SynchroEmittings(initialRefSpectrums.ToArray(), initialEmittings.ToArray(), emittingParameters, ref lstProtocols);
            }
            else
            {
                FillProtocolsDataWithOutEmittings(refSpectrums, ref lstProtocols);
            }
        }


        /// <summary>
        /// Заполнение протокола сведениями о станицях StationExtended
        /// </summary>
        /// <param name="protocolsOutput">итоговый массив ProtocolsOutput, на основе которого будет выполняться генерация отчета</param>
        /// <returns></returns>
        public void FillingProtocolStationData(Protocols[] protocolsOutput, Sensor[] sensors, StationExtended[] stationsExtended, long? synchroProcessId)
        {
            this._logger.Info(Contexts.ThisComponent, Categories.Processing, Events.FillingProtocolStationData.Text);
            var utils = new Utils(this._dataLayer, this._logger);
            var protocolsInput = protocolsOutput;
            var lstProtocols = new List<Protocols>();
            var lstStationsExtended = stationsExtended.ToList();
            var lstSensors = sensors.ToList();
            for (int h = 0; h < protocolsOutput.Length; h++)
            {
                var protocol = protocolsOutput[h];

                protocol.DataSynchronizationProcess = new DataSynchronizationProcess();
                protocol.DataSynchronizationProcess = utils.CurrentSynchronizationProcesByIds(synchroProcessId);
                if (protocol.DataRefSpectrum != null)
                {
                    var fndSensor = lstSensors.Find(z => z.Id.Value == protocol.DataRefSpectrum.SensorId);
                    if (fndSensor != null)
                    {
                        protocol.Sensor = fndSensor;
                    }
                }
                var fndStation = lstStationsExtended.Find(z => z.TableId == protocolsInput[h].DataRefSpectrum.TableId && z.TableName == protocolsInput[h].DataRefSpectrum.TableName);
                if (fndStation != null)
                {
                    protocol.StationExtended = new StationExtended();
                    var stationExtended = protocol.StationExtended;
                    stationExtended.Address = fndStation.Address;
                    stationExtended.BandWidth = fndStation.BandWidth;
                    stationExtended.DesigEmission = fndStation.DesigEmission;
                    stationExtended.OwnerName = fndStation.OwnerName;
                    stationExtended.PermissionNumber = fndStation.PermissionNumber;
                    stationExtended.PermissionStart = fndStation.PermissionStart;
                    stationExtended.PermissionStop = fndStation.PermissionStop;
                    stationExtended.Province = fndStation.Province;
                    stationExtended.Standard = fndStation.Standard;
                    stationExtended.StandardName = fndStation.StandardName;
                    stationExtended.TableId = fndStation.TableId;
                    stationExtended.TableName = fndStation.TableName;
                    stationExtended.Location = fndStation.Location;
                    stationExtended.Id = fndStation.Id;
                    protocol.StationExtended = stationExtended;

                    if (protocol.ProtocolsLinkedWithEmittings != null)
                    {
                        var protocolsLinkedWithEmittings = protocol.ProtocolsLinkedWithEmittings;
                        if ((protocolsLinkedWithEmittings.SpectrumStartFreq_MHz != null) && (protocolsLinkedWithEmittings.SpectrumSteps_kHz != null) && (protocolsLinkedWithEmittings.T1 != null) && (protocolsLinkedWithEmittings.T2 != null) && (protocolsLinkedWithEmittings.StartFrequency_MHz != null) && (protocolsLinkedWithEmittings.StopFrequency_MHz != null))
                        {
                            GetFreqAndBandWidthByEmittingParameters(protocol);
                        }
                        fndStation.StatusMeas = CalcStatus.CalcStatusMeasForStationExtended(true, fndStation.PermissionCancelDate, fndStation.PermissionStop, fndStation.PermissionStart, fndStation.DocNum, fndStation.TestStartDate, fndStation.TestStopDate, protocol.DataRefSpectrum.DateMeas);
                    }
                    else
                    {
                        fndStation.StatusMeas = CalcStatus.CalcStatusMeasForStationExtended(false, fndStation.PermissionCancelDate, fndStation.PermissionStop, fndStation.PermissionStart, fndStation.DocNum, fndStation.TestStartDate, fndStation.TestStopDate, protocol.DataRefSpectrum.DateMeas);
                    }
                    utils.UpdateStatusStationExtended(fndStation);
                    protocol.StationExtended.StatusMeas = fndStation.StatusMeas;
                }
                lstProtocols.Add(protocol);
            }
            protocolsOutput = lstProtocols.ToArray();
        }



        public static void GetFreqAndBandWidthByEmittingParameters(Protocols protocols)
        {
            if (protocols.ProtocolsLinkedWithEmittings != null)
            {
                if (protocols.ProtocolsLinkedWithEmittings.Bandwidth_kHz != null)
                {
                    var freqMiddle_MHz = (double)(protocols.ProtocolsLinkedWithEmittings.SpectrumStartFreq_MHz.Value + protocols.ProtocolsLinkedWithEmittings.SpectrumSteps_kHz * ((protocols.ProtocolsLinkedWithEmittings.T1.Value + protocols.ProtocolsLinkedWithEmittings.T2.Value) / 2000.0));
                    protocols.RadioControlParams = new RadioControlParams();
                    protocols.RadioControlParams.RadioControlMeasFreq_MHz = freqMiddle_MHz;
                    protocols.RadioControlParams.RadioControlBandWidth = protocols.ProtocolsLinkedWithEmittings.Bandwidth_kHz;
                }
                else
                {
                    var freqMiddle_MHz = (double)((protocols.ProtocolsLinkedWithEmittings.StopFrequency_MHz.Value + protocols.ProtocolsLinkedWithEmittings.StartFrequency_MHz.Value) / 2);
                    protocols.RadioControlParams = new RadioControlParams();
                    protocols.RadioControlParams.RadioControlMeasFreq_MHz = freqMiddle_MHz;
                    var bandWidth = (protocols.ProtocolsLinkedWithEmittings.StopFrequency_MHz.Value - protocols.ProtocolsLinkedWithEmittings.StartFrequency_MHz) * 1000;
                    protocols.RadioControlParams.RadioControlBandWidth = bandWidth;
                }
            }
        }

        /// <summary>
        /// Группировка данных 
        /// </summary>
        /// <param name="protocolsOutput">итоговый массив ProtocolsOutput, на основе которого будет выполняться генерация отчета</param>
        /// <returns></returns>
        public void OrderProtocols(Protocols[] protocolsOutput)
        {
            this._logger.Info(Contexts.ThisComponent, Categories.Processing, Events.OrderProtocols.Text);
            Protocols[] listProtocols = null;
            var lstProtocolsOrdered = new List<Protocols>();
            var lstProtocols = protocolsOutput.ToList();
            var lstProtocolsWIthoutNullStation = lstProtocols.FindAll(x => x.StationExtended != null);
            var lstProtocolsWIthNullStation = lstProtocols.FindAll(x => x.StationExtended == null);
            if (lstProtocolsWIthoutNullStation != null)
            {
                var orderByProtocolsOutput = from z in lstProtocolsWIthoutNullStation orderby z.StationExtended.PermissionNumber, z.StationExtended.OwnerName ascending select z;
                lstProtocolsOrdered.AddRange(orderByProtocolsOutput);
            }
            if (lstProtocolsWIthNullStation != null)
            {
                lstProtocolsOrdered.AddRange(lstProtocolsWIthNullStation);
            }
            listProtocols = lstProtocolsOrdered.ToArray();
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
                        if (protocol.StationExtended != null)
                        {

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
                                builderProtocolsInsert.SetValue(c => c.DateMeasDay, (short)protocol.DataRefSpectrum.DateMeas.Day);
                                builderProtocolsInsert.SetValue(c => c.DateMeasMonth, (short)protocol.DataRefSpectrum.DateMeas.Month);
                                builderProtocolsInsert.SetValue(c => c.DateMeasYear, (short)protocol.DataRefSpectrum.DateMeas.Year);
                                builderProtocolsInsert.SetValue(c => c.DispersionLow, protocol.DataRefSpectrum.DispersionLow);
                                builderProtocolsInsert.SetValue(c => c.DispersionUp, protocol.DataRefSpectrum.DispersionUp);
                                builderProtocolsInsert.SetValue(c => c.Freq_MHz, protocol.DataRefSpectrum.Freq_MHz);
                                builderProtocolsInsert.SetValue(c => c.GlobalSID, protocol.DataRefSpectrum.GlobalSID);
                                builderProtocolsInsert.SetValue(c => c.Level_dBm, protocol.DataRefSpectrum.Level_dBm);
                                builderProtocolsInsert.SetValue(c => c.Percent, protocol.DataRefSpectrum.Percent);

                            }
                            if (protocol.RadioControlParams != null)
                            {
                                if ((protocol.RadioControlParams.RadioControlBandWidth != null) && (protocol.RadioControlParams.RadioControlMeasFreq_MHz != null))
                                {
                                    builderProtocolsInsert.SetValue(c => c.RadioControlBandWidth, protocol.RadioControlParams.RadioControlBandWidth);
                                    builderProtocolsInsert.SetValue(c => c.RadioControlMeasFreq_MHz, protocol.RadioControlParams.RadioControlMeasFreq_MHz);
                                }
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
                        else
                        {
                            this._logger.Warning(Contexts.ThisComponent, Categories.Processing, (EventText)$"For DataSynchronizationProcess.Id='{protocol.DataSynchronizationProcess.Id}' StationExtended property is null!" );
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

        /// <summary>
        /// Очистка протокола
        /// </summary>
        /// <param name="synchroProcessId"></param>
        /// <returns></returns>
        public bool ClearProtocol(long synchroProcessId)
        {
            bool isSuccess = false;
            try
            {
                this._logger.Info(Contexts.ThisComponent, Categories.Processing, Events.HandlerClearProtocolMethod.Text);

                using (var scope = this._dataLayer.CreateScope<SdrnServerDataContext>())
                {
                    scope.BeginTran();

                    var builderLinkProtocolsWithEmittingsClear = this._dataLayer.GetBuilder<MD.ILinkProtocolsWithEmittings>().Delete();
                    builderLinkProtocolsWithEmittingsClear.Where(c => c.PROTOCOLS.SYNCHRO_PROCESS.Id, ConditionOperator.Equal, synchroProcessId);
                    scope.Executor.Execute(builderLinkProtocolsWithEmittingsClear);

                    var builderProtocolsClear = this._dataLayer.GetBuilder<MD.IProtocols>().Delete();
                    builderProtocolsClear.Where(c => c.SYNCHRO_PROCESS.Id, ConditionOperator.Equal, synchroProcessId);
                    scope.Executor.Execute(builderProtocolsClear);

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

        /// <summary>
        ///  Сохранение общих сведений о сохраненных протоколах в БД
        /// </summary>
        /// <param name="protocolsOutput"></param>
        /// <param name="refSpectrums"></param>
        /// <param name="synchroProcessId"></param>
        /// <returns></returns>
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

      
        /// <summary>
        /// Извлечение данных об эмитингах с БД
        /// </summary>
        /// <param name="refSpectrums"></param>
        /// <param name="stationsExtended"></param>
        /// <param name="startDate"></param>
        /// <param name="stopDate"></param>
        /// <param name="groupSensor"></param>
        /// <param name="emittingParameters"></param>
        /// <returns></returns>
        public Emitting[] GetEmittings(RefSpectrum[] refSpectrums, StationExtended[] stationsExtended, DateTime startDate, DateTime stopDate, GroupSensors groupSensor, out Calculation.EmitParams[] emittingParameters)
        {
            this._logger.Info(Contexts.ThisComponent, Categories.Processing, Events.GetEmittings.Text);
            var queryExecuter = this._dataLayer.Executor<SdrnServerDataContext>();
            emittingParameters = null;
            var lstEmittingParameters = new List<Calculation.EmitParams>();
            var listIdsEmittings = new List<long>();
            var listEmitings = new List<KeyValuePair<long, Emitting>>();
            var listWorkTimes = new List<KeyValuePair<long, WorkTime>>();
            var listSpectrum = new List<KeyValuePair<long, Spectrum>>();
            var listSensors = new List<Sensor>();
            var listEmitting = new List<Emitting>();

            try
            {
                DateTime? startDateVal = null;
                DateTime? stopDateVal = null;
                var lstStations = stationsExtended.ToList();
                var listRefSpectrum = refSpectrums.ToList();
                bool isFindDatePeriod = false;
                for (int i = 0; i < refSpectrums.Length; i++)
                {
                    var RefSpectrum = new RefSpectrum();

                    var orderByProtocolsOutputVal = from z in refSpectrums[i].DataRefSpectrum orderby z.StatusMeas descending select z;
                    var orderByProtocolsOutput = orderByProtocolsOutputVal.ToArray();
                    for (int j = 0; j < orderByProtocolsOutput.Length; j++)
                    {
                        // получение очередного значения dataRefSpectrum
                        var dataRefSpectrum = orderByProtocolsOutput[j];
                        if ((groupSensor.Freq_MHz == dataRefSpectrum.Freq_MHz) && (groupSensor.SensorId == dataRefSpectrum.SensorId))
                        {
                            var statusMeas = dataRefSpectrum.StatusMeas;
                            stopDateVal = dataRefSpectrum.DateMeas;
                            var fndStation = lstStations.Find(z => z.TableId == dataRefSpectrum.TableId && z.TableName == dataRefSpectrum.TableName);
                            if (fndStation != null)
                            {
                                startDateVal = CalcStatus.CalcDateStart(statusMeas, fndStation.PermissionCancelDate, fndStation.PermissionStop, fndStation.PermissionStart, fndStation.DocNum, fndStation.TestStartDate, fndStation.TestStopDate, startDate);
                                isFindDatePeriod = true;
                                break;
                            }
                        }
                    }
                    if (isFindDatePeriod)
                    {
                        break;
                    }
                }

                if ((startDateVal != null) && (stopDateVal != null))
                {
                    startDateVal = new DateTime(startDateVal.Value.Year, startDateVal.Value.Month, startDateVal.Value.Day, 0, 0, 0, 0);
                    stopDateVal = new DateTime(stopDateVal.Value.Year, stopDateVal.Value.Month, stopDateVal.Value.Day, 23, 59, 59, 999);

                    var queryEmitting = this._dataLayer.GetBuilder<MDBase.IEmitting>()
                   .From()
                   .Select(c => c.Id, c => c.CurentPower_dBm, c => c.MeanDeviationFromReference, c => c.ReferenceLevel_dBm, c => c.RollOffFactor, c => c.StandardBW, c => c.StartFrequency_MHz, c => c.StopFrequency_MHz, c => c.TriggerDeviationFromReference, c => c.LevelsDistributionLvl, c => c.LevelsDistributionCount, c => c.SensorId, c => c.StationID, c => c.StationTableName, c => c.Loss_dB, c => c.Freq_kHz, c => c.RES_MEAS.SUBTASK_SENSOR.SUBTASK.MEAS_TASK.Id)
                   .OrderByAsc(c => c.StartFrequency_MHz)
                   .Where(c => c.RES_MEAS.TimeMeas, ConditionOperator.GreaterEqual, startDateVal)
                   .Where(c => c.RES_MEAS.TimeMeas, ConditionOperator.LessEqual, stopDateVal)
                   .Where(c => c.StartFrequency_MHz, ConditionOperator.LessEqual, groupSensor.Freq_MHz)
                   .Where(c => c.StopFrequency_MHz, ConditionOperator.GreaterEqual, groupSensor.Freq_MHz)
                   .Where(c => c.SensorId, ConditionOperator.Equal, groupSensor.SensorId);
                    queryExecuter.Fetch(queryEmitting, reader =>
                    {
                        while (reader.Read())
                        {
                            bool? collectEmissionInstrumentalEstimation = false;
                            var queryMeasTaskSignaling = this._dataLayer.GetBuilder<MDBase.IMeasTaskSignaling>()
                            .From()
                            .Select(c => c.Id, c => c.CollectEmissionInstrumentalEstimation, c => c.MEAS_TASK.Id, c => c.CrossingBWPercentageForGoodSignals, c => c.CrossingBWPercentageForBadSignals, c => c.AnalyzeByChannel, c => c.CorrelationAnalize, c => c.CorrelationFactor, c => c.MaxFreqDeviation, c => c.TimeBetweenWorkTimes_sec, c => c.TypeJoinSpectrum)
                            .Where(c => c.MEAS_TASK.Id, ConditionOperator.Equal, reader.GetValue(c => c.RES_MEAS.SUBTASK_SENSOR.SUBTASK.MEAS_TASK.Id));
                            queryExecuter.Fetch(queryMeasTaskSignaling, readerMeasTaskSignaling =>
                            {
                                while (readerMeasTaskSignaling.Read())
                                {
                                    var measTaskId = readerMeasTaskSignaling.GetValue(x => x.MEAS_TASK.Id);
                                    collectEmissionInstrumentalEstimation = readerMeasTaskSignaling.GetValue(x => x.CollectEmissionInstrumentalEstimation);
                                    lstEmittingParameters.Add(new Calculation.EmitParams()
                                    {
                                        EmittingId = reader.GetValue(c => c.Id),
                                        CrossingBWPercentageForGoodSignals = readerMeasTaskSignaling.GetValue(x => x.CrossingBWPercentageForGoodSignals),
                                        CrossingBWPercentageForBadSignals = readerMeasTaskSignaling.GetValue(x => x.CrossingBWPercentageForBadSignals),
                                        AnalyzeByChannel = readerMeasTaskSignaling.GetValue(x => x.AnalyzeByChannel),
                                        CorrelationAnalize = readerMeasTaskSignaling.GetValue(x => x.CorrelationAnalize),
                                        CorrelationFactor = readerMeasTaskSignaling.GetValue(x => x.CorrelationFactor),
                                        MaxFreqDeviation = readerMeasTaskSignaling.GetValue(x => x.MaxFreqDeviation),
                                        TimeBetweenWorkTimes_sec = readerMeasTaskSignaling.GetValue(x => x.TimeBetweenWorkTimes_sec),
                                        TypeJoinSpectrum = readerMeasTaskSignaling.GetValue(x => x.TypeJoinSpectrum)
                                    });
                                }
                                return true;
                            });

                            if (((collectEmissionInstrumentalEstimation != null) && (collectEmissionInstrumentalEstimation == false)))
                            {
                                lstEmittingParameters.RemoveAt(lstEmittingParameters.Count - 1);
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
                                    var querySensor = this._dataLayer.GetBuilder<MDBase.ISensor>()
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
                }

                var listIntEmittingWorkTime = BreakDownElemBlocks.BreakDown(listIdsEmittings.ToArray());
                for (int i = 0; i < listIntEmittingWorkTime.Count; i++)
                {
                    var queryTime = this._dataLayer.GetBuilder<MDBase.IWorkTime>()
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
                    var querySpectrum = this._dataLayer.GetBuilder<MDBase.ISpectrum>()
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
                if (lstEmittingParameters != null)
                {
                    emittingParameters = lstEmittingParameters.ToArray();
                }
            }
            catch (Exception e)
            {
                this._logger.Exception(Contexts.ThisComponent, e);
            }
            return listEmitting.ToArray();
        }




       

        public Area[] GetAreas(DataSynchronizationBase dataSynchronization)
        {
            this._logger.Info(Contexts.ThisComponent, Categories.Processing, Events.GetAreas.Text);
            var areas = new List<Area>();
            try
            {
                var areaIds = new List<long>();

                var queryExecuter = this._dataLayer.Executor<SdrnServerDataContext>();
                var queryLinkAreaFrom = this._dataLayer.GetBuilder<MD.ILinkArea>()
                .From()
                .Select(c => c.Id, c => c.AREA.Id)
                .Where(c => c.SYNCHRO_PROCESS.Id, ConditionOperator.Equal, dataSynchronization.Id);
                queryExecuter.Fetch(queryLinkAreaFrom, readerLinkAreaFrom =>
                {
                    while (readerLinkAreaFrom.Read())
                    {
                        var areaId = readerLinkAreaFrom.GetValue(c => c.AREA.Id);
                        areaIds.Add(areaId);
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
            this._logger.Info(Contexts.ThisComponent, Categories.Processing, Events.GetStationExtended.Text);
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
                        .Select(c => c.Id, c => c.Address, c => c.BandWidth, c => c.DesigEmission, c => c.Latitude, c => c.Longitude, c => c.OwnerName, c => c.PermissionNumber, c => c.PermissionStart, c => c.PermissionStop, c => c.Province, c => c.Standard, c => c.StandardName, c => c.TableId, c => c.TableName,
                         c => c.DocNum, c => c.StationName, c => c.StationChannel, c => c.StationTxFreq, c => c.StationRxFreq, c => c.PermissionCancelDate, c => c.TestStartDate, c => c.TestStopDate, c => c.PermissionGlobalSID, c => c.OKPO, c => c.StatusMeas, c => c.CurentStatusStation)
                        .Where(c => c.TableId, ConditionOperator.Equal, readerRefSpectrumFrom.GetValue(c => c.TableId))
                        .Where(c => c.TableName, ConditionOperator.Equal, readerRefSpectrumFrom.GetValue(c => c.TableName));
                        queryExecuter.Fetch(queryStationExtendedFrom, readerStationExtendedFrom =>
                        {
                            while (readerStationExtendedFrom.Read())
                            {
                                var stationExtended = new StationExtended();
                                stationExtended.Address = readerStationExtendedFrom.GetValue(c => c.Address);
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
                                stationExtended.Id = readerStationExtendedFrom.GetValue(c => c.Id);
                                stationExtended.DocNum = readerStationExtendedFrom.GetValue(c => c.DocNum);
                                stationExtended.StationName = readerStationExtendedFrom.GetValue(c => c.StationName);
                                stationExtended.StationChannel = readerStationExtendedFrom.GetValue(c => c.StationChannel);
                                stationExtended.StationTxFreq = readerStationExtendedFrom.GetValue(c => c.StationTxFreq);
                                stationExtended.StationRxFreq = readerStationExtendedFrom.GetValue(c => c.StationRxFreq);
                                stationExtended.PermissionCancelDate = readerStationExtendedFrom.GetValue(c => c.PermissionCancelDate);
                                stationExtended.TestStartDate = readerStationExtendedFrom.GetValue(c => c.TestStartDate);
                                stationExtended.TestStopDate = readerStationExtendedFrom.GetValue(c => c.TestStopDate);
                                stationExtended.PermissionGlobalSID = readerStationExtendedFrom.GetValue(c => c.PermissionGlobalSID);
                                stationExtended.OKPO = readerStationExtendedFrom.GetValue(c => c.OKPO);
                                stationExtended.StatusMeas = readerStationExtendedFrom.GetValue(c => c.StatusMeas);
                                stationExtended.CurentStatusStation = readerStationExtendedFrom.GetValue(c => c.CurentStatusStation);

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





        public void RecoveryDataSynchronizationProcess()
        {
            try
            {
                if (RunSynchroProcess.IsAlreadyRunProcess)
                {
                    return;
                }
                this._logger.Info(Contexts.ThisComponent, Categories.Processing, Events.RecoveryDataSynchronizationProcess.Text);
                var loadSensor = new LoadSensor(this._dataLayer, this._logger);
                var utils = new Utils(this._dataLayer, this._logger);
                var currentDataSynchronizationProcess = utils.CurrentDataSynchronizationProcess();

                //если процесс синхронизации не запущен, тогда создаем новый
                if (currentDataSynchronizationProcess != null)
                {

                    // текущий процесс
                    var dataSynchronization = currentDataSynchronizationProcess;

                    // набор идентификаторов headRefSpectrumIdsBySDRN
                    var headRefSpectrumIdsBySDRN = utils.GetHeadRefSpectrumIdsBySDRN(dataSynchronization);

                    // набор идентификаторов sensorIdsBySDRN
                    var sensorIdsBySDRN = utils.GetSensorIdsBySDRN(dataSynchronization);

                    // набор areas
                    var areas = GetAreas(dataSynchronization);

                    // набор stationsExtended
                    var stationsExtended = GetStationExtended(dataSynchronization, headRefSpectrumIdsBySDRN);

                    var listRefSpectrum = new List<RefSpectrum>();
                    var isSuccessDeleteRefSpectrum = DeleteDuplicateRefSpectrumRecords(dataSynchronization, headRefSpectrumIdsBySDRN, sensorIdsBySDRN, areas, stationsExtended, ref listRefSpectrum);
                    if (isSuccessDeleteRefSpectrum == true)
                    {
                        var sensors = new Sensor[sensorIdsBySDRN.Length];
                        for (int j = 0; j < sensorIdsBySDRN.Length; j++)
                        {
                            sensors[j] = loadSensor.LoadBaseDateSensor(sensorIdsBySDRN[j]);
                        }

                        var arrRefSpectrum = listRefSpectrum.ToArray();
                        // заново вычитываем из БД все RefSpectrum (после удаления дубликатов) 
                        //var listRefSpectrum = utils.GetRefSpectrumByIds(headRefSpectrumIdsBySDRN, sensorIdsBySDRN);

                        // Весь массив разбивается на группы (далее группа сенсора) по следующему признаку: Одинаковые ID Sensor, Freq MHz
                        var groupsSensors = GetGroupSensors(arrRefSpectrum);

                        var listProtocolsOutput = new List<Protocols>();
                        for (int h = 0; h < groupsSensors.Length; h++)
                        {
                            //здесь получаем массив RefSpectrum, который соответсвует группе groupsSensors[h]
                            var refSpectrum = SelectGroupSensor(groupsSensors[h], arrRefSpectrum);

                            // Подготовка данных для синхронизации группы сенсора. Из БД ICSControl выбираются все Emitting для которых:
                            //-таск имеет Collect emission for instrumental estimation = true;
                            // -результат пришел в рамках даты начала отчета, даты конца отчета;
                            // -сенсор где был получен результат совпадает с ID Sensor
                            //-частота Freq MHz(из группы сенсора) находиться в пределах начальной и конечной частоты Emitting
                            var emittings = GetEmittings(refSpectrum, stationsExtended, dataSynchronization.DateStart, dataSynchronization.DateEnd, groupsSensors[h], out Calculation.EmitParams[] emittingParameters);
                            if ((emittings != null) && (emittings.Length > 0))
                            {
                                // Синхронизация излучений с записями группы сенсора
                                var protocol = new List<Protocols>();
                                SynchroEmittings(refSpectrum, emittings, emittingParameters, ref protocol);
                                listProtocolsOutput.AddRange(protocol);
                            }
                            else
                            {
                                for (int i = 0; i < refSpectrum.Length; i++)
                                {
                                    var RefSpectrum = new RefSpectrum();
                                    for (int j = 0; j < refSpectrum[i].DataRefSpectrum.Length; j++)
                                    {
                                        // получение очередного значения dataRefSpectrum
                                        var dataRefSpectrum = refSpectrum[i].DataRefSpectrum[j];

                                        // обявляем очередной экземпляр переменной Protocols
                                        var protocol = new Protocols();
                                        protocol.DataRefSpectrum = new DataRefSpectrum();
                                        // копируем данные с переменной dataRefSpectrum
                                        protocol.DataRefSpectrum = dataRefSpectrum;
                                        listProtocolsOutput.Add(protocol);
                                    }
                                }
                            }
                        }

                        // 
                        var arrayProtocols = listProtocolsOutput.ToArray();

                        // заполнение полным перечнем данных (расширенными сведениями о станциях и сенсорах)
                        FillingProtocolStationData(arrayProtocols, sensors, stationsExtended, dataSynchronization.Id);

                        // Группировка данных
                        OrderProtocols(arrayProtocols);
                        // предварительная очистка таблиц Protocols и LinkProtocolsWithEmittings для текущего synchroProcessId
                        if (ClearProtocol(dataSynchronization.Id.Value))
                        {
                            // здесь вызов процецедуры сохранения массива значений protocol
                            var isSuccessOperation = SaveOutputProtocolsToDB(arrayProtocols);
                            if (isSuccessOperation)
                            {
                                // запись итоговой информации в ISynchroProcess (и установка статуса в "С" - Completed)
                                var isSuccesFinalOperationSynchro = SaveDataSynchronizationProcessToDB(arrayProtocols, arrRefSpectrum, dataSynchronization.Id.Value);
                                if (isSuccesFinalOperationSynchro)
                                {
                                    utils.ClearAllLinksByProcessId(currentDataSynchronizationProcess.Id.Value);
                                    this._logger.Info(Contexts.ThisComponent, Categories.Processing, Events.DataSynchronizationProcessCompleted.Text);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                this._logger.Exception(Contexts.ThisComponent, e);
            }
        }

        public bool RunDataSynchronizationProcess(DataSynchronizationBase dataSynchronization, long[] headRefSpectrumIdsBySDRN, long[] sensorIdsBySDRN, Area[] areas, StationExtended[] stationsExtended)
        {
            bool isSuccess = false;
            try
            {
                this._logger.Info(Contexts.ThisComponent, Categories.Processing, Events.RunDataSynchronizationProcess.Text);
                RunSynchroProcess.IsAlreadyRunProcess = true;
                var utils = new Utils(this._dataLayer, this._logger);

                bool isSuccessCheckRunSynchroProcess = utils.CheckRunSynchroProcess(dataSynchronization.DateStart, dataSynchronization.DateEnd);
                if (isSuccessCheckRunSynchroProcess == false)
                {
                    return false;
                }
                else
                {
                    System.Threading.Tasks.Task.Run(() =>
                    {
                        var currentDataSynchronizationProcess = utils.CurrentDataSynchronizationProcess();

                        //если процесс синхронизации не запущен, тогда создаем новый
                        if (currentDataSynchronizationProcess == null)
                        {
                            //areas = GetAreas(dataSynchronization);
                            // создаем запись в таблице SynchroProcess
                            var synchroProcessId = CreateDataSynchronizationProcess(dataSynchronization, headRefSpectrumIdsBySDRN, sensorIdsBySDRN, areas);
                            if (synchroProcessId != null)
                            {
                                dataSynchronization.Id = synchroProcessId;
                                var loadSensor = new LoadSensor(this._dataLayer, this._logger);
                                var sensors = new Sensor[sensorIdsBySDRN.Length];
                                for (int j = 0; j < sensorIdsBySDRN.Length; j++)
                                {
                                    sensors[j] = loadSensor.LoadBaseDateSensor(sensorIdsBySDRN[j]);
                                }

                                // обновление перечня станций
                                //stationsExtended = GetStationExtended(dataSynchronization, headRefSpectrumIdsBySDRN);
                                var isSuccessUpdateStationExtended = SynchroStationsExtended(stationsExtended);
                                var listStationsExtended = new List<StationExtended>();
                                if ((stationsExtended != null) && (stationsExtended.Length > 0))
                                {
                                    for (int i = 0; i < stationsExtended.Length; i++)
                                    {
                                        if ((!string.IsNullOrEmpty(stationsExtended[i].TableName)) && (stationsExtended[i].TableId > 0))
                                        {
                                            listStationsExtended.Add(stationsExtended[i]);
                                        }
                                    }
                                }
                                stationsExtended = listStationsExtended.ToArray();
                                // обновление областей и контуров

                                var isSuccessUpdateAreas = SynchroAreas(areas);
                                if ((isSuccessUpdateAreas == true) && (isSuccessUpdateStationExtended == true))
                                {
                                    var listRefSpectrum = new List<RefSpectrum>();
                                    var isSuccessDeleteRefSpectrum = DeleteDuplicateRefSpectrumRecords(dataSynchronization, headRefSpectrumIdsBySDRN, sensorIdsBySDRN, areas, stationsExtended, ref listRefSpectrum);
                                    if (isSuccessDeleteRefSpectrum == true)
                                    {
                                        // заново вычитываем из БД все RefSpectrum (после удаления дубликатов) 
                                        //var listRefSpectrum = utils.GetRefSpectrumByIds(headRefSpectrumIdsBySDRN, sensorIdsBySDRN);

                                        var arrRefSpectrum = listRefSpectrum.ToArray();

                                        // Весь массив разбивается на группы (далее группа сенсора) по следующему признаку: Одинаковые ID Sensor, Freq MHz
                                        var groupsSensors = GetGroupSensors(arrRefSpectrum);

                                        var listProtocolsOutput = new List<Protocols>();
                                        for (int h = 0; h < groupsSensors.Length; h++)
                                        {
                                            //здесь получаем массив RefSpectrum, который соответсвует группе groupsSensors[h]
                                            var refSpectrum = SelectGroupSensor(groupsSensors[h], arrRefSpectrum);

                                            // Подготовка данных для синхронизации группы сенсора. Из БД ICSControl выбираются все Emitting для которых:
                                            //-таск имеет Collect emission for instrumental estimation = true;
                                            // -результат пришел в рамках даты начала отчета, даты конца отчета;
                                            // -сенсор где был получен результат совпадает с ID Sensor
                                            //-частота Freq MHz(из группы сенсора) находиться в пределах начальной и конечной частоты Emitting

                                            var emittings = GetEmittings(refSpectrum, stationsExtended, dataSynchronization.DateStart, dataSynchronization.DateEnd, groupsSensors[h], out Calculation.EmitParams[] emittingParameters);
                                            if ((emittings != null) && (emittings.Length > 0))
                                            {
                                                // Синхронизация излучений с записями группы сенсора
                                                var protocol = new List<Protocols>();
                                                SynchroEmittings(refSpectrum, emittings, emittingParameters, ref protocol);

                                                listProtocolsOutput.AddRange(protocol);
                                            }
                                            else
                                            {
                                                for (int i = 0; i < refSpectrum.Length; i++)
                                                {
                                                    var RefSpectrum = new RefSpectrum();
                                                    for (int j = 0; j < refSpectrum[i].DataRefSpectrum.Length; j++)
                                                    {
                                                        // получение очередного значения dataRefSpectrum
                                                        var dataRefSpectrum = refSpectrum[i].DataRefSpectrum[j];

                                                        // обявляем очередной экземпляр переменной Protocols
                                                        var protocol = new Protocols();
                                                        protocol.DataRefSpectrum = new DataRefSpectrum();
                                                        // копируем данные с переменной dataRefSpectrum
                                                        protocol.DataRefSpectrum = dataRefSpectrum;
                                                        listProtocolsOutput.Add(protocol);
                                                    }
                                                }
                                            }
                                        }


                                        // 
                                        var arrayProtocols = listProtocolsOutput.ToArray();

                                        // заполнение полным перечнем данных (расширенными сведениями о станциях и сенсорах)
                                        FillingProtocolStationData(arrayProtocols, sensors, stationsExtended, synchroProcessId);

                                        // Группировка данных
                                        OrderProtocols(arrayProtocols);
                                        // предварительная очистка таблиц Protocols и LinkProtocolsWithEmittings для текущего synchroProcessId
                                        if (ClearProtocol(synchroProcessId.Value))
                                        {
                                            // здесь вызов процецедуры сохранения массива значений protocol
                                            var isSuccessOperation = SaveOutputProtocolsToDB(arrayProtocols);
                                            if (isSuccessOperation)
                                            {
                                                // запись итоговой информации в ISynchroProcess (и установка статуса в "С" - Completed)
                                                var isSuccesFinalOperationSynchro = SaveDataSynchronizationProcessToDB(arrayProtocols, arrRefSpectrum, synchroProcessId.Value);
                                                if (isSuccesFinalOperationSynchro)
                                                {
                                                    utils.ClearAllLinksByProcessId(synchroProcessId.Value);
                                                    this._logger.Info(Contexts.ThisComponent, Categories.Processing, Events.DataSynchronizationProcessCompleted.Text);
                                                }
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    this._logger.Warning(Contexts.ThisComponent, Categories.Processing, Events.OccurredUnexpectedErrorsStationsOrAreas);
                                }
                            }
                        }
                        isSuccess = true;
                    });
                    return true;
                }
            }
            catch (Exception e)
            {
                isSuccess = false;
                this._logger.Exception(Contexts.ThisComponent, e);
            }
            return isSuccess;
        }

    }
}




