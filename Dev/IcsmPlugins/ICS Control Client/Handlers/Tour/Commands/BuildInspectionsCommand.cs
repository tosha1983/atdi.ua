using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ICSM;
using XICSM.ICSControlClient.Environment;
using MD = XICSM.ICSControlClient.Metadata;
using DM = XICSM.ICSControlClient.Models.BuildInspections;
using DTO = XICSM.ICSControlClient.Models.DTO;
using MDC = XICSM.ICSControlClient.Models;

namespace XICSM.ICSControlClient.Handlers.TourCommnads
{
    public class BuildInspectionsCommand
    {
        public static bool Handle(IMQueryMenuNode.Context context)
        {
            return
                context.ExecuteContextMenuAction(
                        PluginMetadata.Processes.BuildInspections,
                        BuildInspectionsByTour
                    );
        }

        private static bool BuildInspectionsByTour(int tourId)
        {
            var tour = Repository.ReadEntityById<DM.Tour>(tourId);
            if (!ValidateTour(tour))
            {
                return false;
            }

            var stations = FindStations(tour);
            if (!stations.ExistsItems())
            {
                Logger.WriteWarning(PluginMetadata.Processes.BuildInspections, $"Not found any stations by the tour #{tourId}", true);
                return true;
            }

            stations.ForEach(station =>
            {
                Repository.CreateEntity<DM.Inspection>(inspection =>
                {
                    inspection.Type = MD.Inspection.Types.C;
                    inspection.Status = MD.Inspection.Statuses.New;
                    inspection.DoItAfter = tour.StartDate;
                    inspection.DoItBefore = tour.StopDate;
                    inspection.TourId = tourId;
                    inspection.StationTableName = station.TableName;
                    inspection.StationTableId = station.TableId;
                });
            });


            tour.Status = MD.Tours.Statuses.New;
            Repository.UpdateEntity(tour);

            return true;
        }

        private static bool ValidateTour(DM.Tour tour)
        {
            var messages = new StringBuilder();
            bool result = true;

            if (!string.IsNullOrEmpty(tour.Status))
            {
                messages.AppendLine($"Incorrect the status of the tour #{tour.Id}");
                result = false;
            }

            if (tour.StartDate.IsNull())
            {
                messages.AppendLine($"Undefined a start date of the tour #{tour.Id}");
                result = false;
            }

            if (tour.StopDate.IsNull())
            {
                messages.AppendLine($"Undefined a stop date of the tour #{tour.Id}");
                result = false;
            }

            if (string.IsNullOrEmpty(tour.LocationList))
            {
                messages.AppendLine($"Undefined a location list of the tour #{tour.Id}");
                result = false;
            }

            if (string.IsNullOrEmpty(tour.RadioTechList))
            {
                messages.AppendLine($"Undefined a radio system list of the tour #{tour.Id}");
                result = false;
            }

            if (!result)
            {
                Logger.WriteWarning(PluginMetadata.Processes.BuildInspections, messages.ToString(), true);
            }

            return result;
        }

        private static List<DTO.Station> FindStations(DM.Tour tour)
        {
            var result = new List<DTO.Station>();

            var polygon = new MDC.LocationPolygon(tour.LocationList);
            if (polygon.Count <= 0)
            {
                throw new InvalidOperationException("Location list is empty"); 
            }
            var polygonRect = polygon.GetRect();

            using (var allStationsReader = Repository.ReadEntities<DM.AllStation>(source =>
             {
                 source.SetAdditional($"([{MD.AllStations.Fields.Standart}] in ('{tour.RadioTechList.Replace(",", "','")}'))");
                 source.SetWhere(MD.AllStations.Fields.Latitude, IMRecordset.Operation.Ge, polygonRect.Min.Lat);
                 source.SetWhere(MD.AllStations.Fields.Latitude, IMRecordset.Operation.Le, polygonRect.Max.Lat);
                 source.SetWhere(MD.AllStations.Fields.Longitude, IMRecordset.Operation.Ge, polygonRect.Min.Lon);
                 source.SetWhere(MD.AllStations.Fields.Longitude, IMRecordset.Operation.Le, polygonRect.Max.Lon);
             }))
            {
                while(allStationsReader.Read())
                {
                    var allStation = allStationsReader.GetEntity();

                    var checkLocation = new MDC.Location(allStation.Longitude, allStation.Latitude);
                    if (checkLocation.CheckHitting(polygon))
                    {
                        var station = new DTO.Station
                        {
                            Id = allStation.Id,
                            TableName = allStation.TableName,
                            TableId = allStation.TableId,
                        };

                        if (CheckStationApplicationCurrentState(tour, station))
                        {
                            result.Add(station);
                        }
                    }
                }
            }

            return result;
        }

        private static bool CheckStationApplicationCurrentState(DM.Tour tour, DTO.Station station)
        {
            using (var applicationsReader = Repository.ReadEntities<DM.NfraApplication>(source =>
             {
                 source.SetWhere(MD.NfraApplication.Fields.ObjTable, IMRecordset.Operation.Eq, station.TableName);
                 source.SetAdditional($"({station.TableId} in ([{MD.NfraApplication.Fields.ObjId1}], [{MD.NfraApplication.Fields.ObjId2}], [{MD.NfraApplication.Fields.ObjId3}], [{MD.NfraApplication.Fields.ObjId4}], [{MD.NfraApplication.Fields.ObjId5}], [{MD.NfraApplication.Fields.ObjId6}]))");
             }))
            {
                while(applicationsReader.Read())
                {
                    var application = applicationsReader.GetEntity();

                    var dateFrom = application.DozvDateFrom; // applicationRs.GetT(MD.NfraApplication.Fields.DozvDateFrom);
                    var dateTo = application.DozvDateTo; // applicationRs.GetT(MD.NfraApplication.Fields.DozvDateTo);
                    var dateCancel = application.DozvDateCancel; // applicationRs.GetT(MD.NfraApplication.Fields.DozvDateCancel);

                    if (!dateFrom.IsNull() && !dateTo.IsNull())
                    {
                        if (!dateCancel.IsNull() && dateCancel < dateTo)
                        {
                            dateTo = dateCancel;
                        }
                        if (dateFrom <= tour.StartDate && tour.StopDate <= dateTo)
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }
    }
}
