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
using Atdi.Modules.Sdrn.Server.Events;

namespace Atdi.AppUnits.Sdrn.Server.PrimaryHandlers.Subscribes
{
    [SubscriptionEvent(EventName = "OnReceivedNewSOResult", SubscriberName = "SubscriberMeasTaskProcess")]
    public class OnReceivedNewSOResult : IEventSubscriber<OnReceivedNewSOResultEvent>
    {
        private readonly ILogger _logger;
        private readonly ISdrnMessagePublisher _messagePublisher;
        private readonly IDataLayer<EntityDataOrm> _dataLayer;
        private readonly ISdrnServerEnvironment _environment;

        public OnReceivedNewSOResult(ISdrnMessagePublisher messagePublisher, IDataLayer<EntityDataOrm> dataLayer, ISdrnServerEnvironment environment, ILogger logger)
        {
            this._logger = logger;
            this._messagePublisher = messagePublisher;
            this._dataLayer = dataLayer;
            this._environment = environment;
        }
        public void Notify(OnReceivedNewSOResultEvent @event)
        {
            using (this._logger.StartTrace(Contexts.PrimaryHandler, Categories.Notify, this))
            {
                var measResult = ReadMeasResult(@event.ResultId);

                if (measResult.Measurement == DataModels.Sdrns.MeasurementType.MonitoringStations)
                {
                    this.ValidateMeasResultMonitoringStations(measResult);
                }
            }
        }
        private void ValidateMeasResultMonitoringStations(DEV.MeasResults measResult)
        {
            if (string.IsNullOrEmpty(measResult.ResultId))
                WriteLog("Undefined value ResultId");
            if (string.IsNullOrEmpty(measResult.TaskId))
                WriteLog("Undefined value TaskId");
            if (measResult.ResultId.Length > 50)
                measResult.ResultId.Substring(0, 50);
            if (measResult.TaskId.Length > 200)
                measResult.TaskId.Substring(0, 200);
            if (measResult.Status.Length > 5)
                measResult.Status = "";
            if (measResult.SwNumber < 0 || measResult.SwNumber > 10000)
                WriteLog("Incorrect value SwNumber");

            for (int i = 0; i < measResult.StationResults.Count(); i++)
            {
                if (measResult.StationResults[i].StationId.Length > 50)
                    measResult.StationResults[i].StationId.Substring(0, 50);
                if (measResult.StationResults[i].TaskGlobalSid.Length > 50)
                    measResult.StationResults[i].TaskGlobalSid.Substring(0, 50);
                if (measResult.StationResults[i].RealGlobalSid.Length > 50)
                    measResult.StationResults[i].RealGlobalSid.Substring(0, 50);
                if (measResult.StationResults[i].SectorId.Length > 50)
                    measResult.StationResults[i].SectorId.Substring(0, 50);
                if (measResult.StationResults[i].Status.Length > 5)
                    measResult.StationResults[i].Status.Substring(0, 5);
                if (measResult.StationResults[i].Standard.Length > 10)
                    measResult.StationResults[i].Standard.Substring(0, 10);

                var failedLevelResults = new List<int>();

                for (int j = 0; j < measResult.StationResults[i].LevelResults.Count(); j++)
                {
                    if (!measResult.StationResults[i].LevelResults[j].Level_dBm.HasValue && measResult.StationResults[i].LevelResults[j].Level_dBm.Value < -150 && measResult.StationResults[i].LevelResults[j].Level_dBm.Value > 20)
                    {
                        failedLevelResults.Add(j);
                        continue;
                    }
                    if (!measResult.StationResults[i].LevelResults[j].Level_dBmkVm.HasValue && measResult.StationResults[i].LevelResults[j].Level_dBmkVm.Value < -10 && measResult.StationResults[i].LevelResults[j].Level_dBmkVm.Value > 140)
                    {
                        failedLevelResults.Add(j);
                        continue;
                    }
                    if (!measResult.StationResults[i].LevelResults[j].DifferenceTimeStamp_ns.HasValue && measResult.StationResults[i].LevelResults[j].DifferenceTimeStamp_ns.Value < 0 && measResult.StationResults[i].LevelResults[j].Level_dBmkVm.Value > 999999999)
                    {
                        WriteLog("Incorrect value SwNumber");
                    }
                }

                //var couples = from res in measResult.StationResults[i].LevelResults.Where(z => z.id).ToArray();
            }
        }
        private DEV.MeasResults ReadMeasResult(int id)
        {
            var measResult = new DEV.MeasResults();
            return measResult;
        }
        private void WriteLog(string msg)
        {

        }
    }
}
