using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ICSM;
using XICSM.ICSControlClient.Environment;
using MD = XICSM.ICSControlClient.Metadata;
using DM = XICSM.ICSControlClient.Models.SynchroInspections;
using WCF = XICSM.ICSControlClient.WcfServiceClients;
using SDR = Atdi.Contracts.WcfServices.Sdrn.Server;

namespace XICSM.ICSControlClient.Handlers.TourCommnads
{
    public class SynchroInspectionsCommand
    {
        public static bool Handle(IMQueryMenuNode.Context context)
        {
            return
                context.ExecuteContextMenuAction(
                        PluginMetadata.Processes.SynchroInspections,
                        SynchroInspectionsByTour
                    );
        }

        private static bool SynchroInspectionsByTour(int tourId)
        {
            var tour = Repository.ReadEntityById<DM.Tour>(tourId);
            if (!ValidateTour(tour))
            {
                return false;
            }

            var inspactions = FindInspections(tour);
            if(inspactions == null || inspactions.Length == 0)
            {
                throw new InvalidOperationException($"Not found any inspection records by the tour #{tour.Id}");
            }

            var measTask = WCF.SdrnsControllerWcfClient.GetMeasTaskHeaderById(tour.MeasTaskId);

            if (measTask == null)
            {
                throw new InvalidOperationException($"Not found the meas task with id #{tour.MeasTaskId} by the tour #{tour.Id}");
            }

            var measTaskResult = WCF.SdrnsControllerWcfClient.GetMeasResultsByTask(tour.MeasTaskId);

            if (measTaskResult == null || measTaskResult.Length == 0)
            {
                throw new InvalidOperationException($"Not found any results of the meas task with id #{tour.MeasTaskId} by the tour #{tour.Id}");
            }

            inspactions.ToList().ForEach(inspaction =>
            {
                if (measTaskResult.Where(r => r.ResultsMeasStation != null && r.ResultsMeasStation.Where(s=> 
                {
                    if (string.IsNullOrEmpty(s.Idstation))
                    {
                        return false;
                    }

                    int stationId = 0;

                    if (!int.TryParse(inspaction.StationRef.Name, out stationId))
                    {
                        return false;
                    }
                    return s.Idstation == stationId.ToString();

                }).Count() > 0 ).Count() > 0)
                {
                    inspaction.Status = MD.Inspection.Statuses.Done;
                    Repository.UpdateEntity(inspaction);
                }
            });

            return true;
        }

        private static DM.Inspection[] FindInspections(DM.Tour tour)
        {
            return Repository.GetEntities<DM.Inspection>(source =>
            {
                source.SetWhere(MD.Inspection.Fields.TourId, IMRecordset.Operation.Eq, tour.Id);
                source.SetWhere(MD.Inspection.Fields.Status, IMRecordset.Operation.Like, MD.Inspection.Statuses.New);
            });
        }

        private static void SaveTaskToLog(SDR.MeasTask measTask)
        {
            try
            {
                var data = new StringBuilder();

                data.AppendLine($"Created meas task:");
                data.AppendLine($"Name: {measTask.Name}");
                data.AppendLine($"Mode: {measTask.ExecutionMode}");
                data.AppendLine($"Created: {measTask.DateCreated}");
                data.AppendLine($"Stations: {measTask.Stations.Length}");
                data.AppendLine($"StationsForMeasurements: {measTask.StationsForMeasurements.Length}");

                Logger.WriteInfo(PluginMetadata.Processes.CreateMeasTask, data.ToString());
            }
            catch(Exception e)
            {
                Logger.WriteExeption(PluginMetadata.Processes.SaveTaskToLog, e);
            }

        }
        
        private static bool ValidateTour(DM.Tour tour)
        {
            var messages = new StringBuilder();
            bool result = true;


            if (tour.MeasTaskId == 0 || tour.MeasTaskId == IM.NullI)
            {
                messages.AppendLine($"Undefined a task ID  of the tour #{tour.Id}");
                result = false;
            }

            

            if (!result)
            {
                Logger.WriteWarning(PluginMetadata.Processes.CreateMeasTask, messages.ToString(), true);
            }

            return result;
        }
    }
}
