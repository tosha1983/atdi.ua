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
    [SubscriptionEvent(EventName = "OnReceivedNewSOResult", SubscriberName = "SubscriberSendMeasResultsProcess")]
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
            var queryExecuter = this._dataLayer.Executor<SdrnServerDataContext>();
            bool validationResult = true;
            string MeasTaskId = "";
            int? MeasSubTaskId = null;
            int? MeasSubTaskStationId = null;
            int? SensorId = null;
            double? AntVal;
            DateTime? TimeMeas = null;
            int? DataRank = 0;
            int? N;
            string Status = "";
            DataModels.Sdrns.MeasurementType TypeMeasurements = DataModels.Sdrns.MeasurementType.MonitoringStations;
            string MeasResultSID = "";
            bool Synchronized;
            DateTime? StartTime = null;
            DateTime? StopTime = null;
            int? ScansNumber = null;

            try
            {
                queryExecuter.BeginTransaction();
                var queryResMeas = this._dataLayer.GetBuilder<MD.IResMeasRaw>()
                .From()
                .Select(c => c.Id, c => c.MeasTaskId, c => c.MeasSubTaskId, c => c.MeasSubTaskStationId, c => c.SensorId, c => c.AntVal, c => c.TimeMeas, c => c.DataRank, c => c.N, c => c.Status, c => c.MeasResultSID)
                .Select(c => c.TypeMeasurements, c => c.Synchronized, c => c.StartTime, c => c.StopTime, c => c.ScansNumber, c => c.SENSOR.Status)
                .Where(c => c.Id, ConditionOperator.Equal, @event.ResultId);
                queryExecuter.Fetch(queryResMeas, reader =>
                {
                    var result = reader.Read();
                    if (result)
                    {
                        MeasTaskId = reader.GetValue(c => c.MeasTaskId);
                        MeasSubTaskId = reader.GetValue(c => c.MeasSubTaskId);
                        MeasSubTaskStationId = reader.GetValue(c => c.MeasSubTaskStationId);
                        SensorId = reader.GetValue(c => c.SensorId);
                        AntVal = reader.GetValue(c => c.AntVal);
                        TimeMeas = reader.GetValue(c => c.TimeMeas);
                        DataRank = reader.GetValue(c => c.DataRank);
                        N = reader.GetValue(c => c.N);
                        Status = reader.GetValue(c => c.Status);
                        TypeMeasurements = (DataModels.Sdrns.MeasurementType)Enum.Parse(typeof(DataModels.Sdrns.MeasurementType), reader.GetValue(c => c.TypeMeasurements), true);
                        MeasResultSID = reader.GetValue(c => c.MeasResultSID);
                        Synchronized = reader.GetValue(c => c.Synchronized);
                        StartTime = reader.GetValue(c => c.StartTime);
                        StopTime = reader.GetValue(c => c.StopTime);
                        ScansNumber = reader.GetValue(c => c.ScansNumber);
                    }
                    return result;
                });

                if (TypeMeasurements == DataModels.Sdrns.MeasurementType.SpectrumOccupation)
                {
                    if (string.IsNullOrEmpty(MeasResultSID))
                    {
                        WriteLog("Undefined value ResultId");
                        validationResult = false;
                    }
                    else if (MeasResultSID.Length > 50)
                    {
                        MeasResultSID.Substring(0, 50);
                    }
                    if (string.IsNullOrEmpty(MeasTaskId))
                    {
                        WriteLog("Undefined value TaskId");
                        validationResult = false;
                    }
                    else if (MeasTaskId.Length > 200)
                    {
                        MeasTaskId.Substring(0, 200);
                    }
                    if (Status.Length > 5)
                    {
                        Status = "";
                    }
                    if (StartTime.HasValue && StopTime.HasValue && StartTime.Value > StopTime.Value)
                    {
                        WriteLog("StartTime must be less than StopTime");
                    }
                    if (ScansNumber.HasValue && (ScansNumber < 1 || ScansNumber > 10000000))
                    {
                        ScansNumber = null;
                        WriteLog("Incorrect value ScansNumber");
                    }

                    int valInsResMeas = 0;
                    var builderInsertIResMeas = this._dataLayer.GetBuilder<MD.IResMeas>().Insert();
                    builderInsertIResMeas.SetValue(c => c.TimeMeas, TimeMeas);
                    builderInsertIResMeas.SetValue(c => c.MeasTaskId, MeasTaskId);
                    builderInsertIResMeas.SetValue(c => c.SensorId, SensorId);
                    builderInsertIResMeas.SetValue(c => c.MeasSubTaskId, MeasSubTaskId);
                    builderInsertIResMeas.SetValue(c => c.MeasSubTaskStationId, MeasSubTaskStationId);
                    builderInsertIResMeas.SetValue(c => c.DataRank, DataRank);
                    builderInsertIResMeas.SetValue(c => c.Status, Status);
                    builderInsertIResMeas.SetValue(c => c.TypeMeasurements, TypeMeasurements.ToString());
                    builderInsertIResMeas.SetValue(c => c.MeasResultSID, MeasResultSID);
                    builderInsertIResMeas.SetValue(c => c.StartTime, StartTime);
                    builderInsertIResMeas.SetValue(c => c.StopTime, StopTime);
                    builderInsertIResMeas.SetValue(c => c.ScansNumber, ScansNumber);
                    builderInsertIResMeas.Select(c => c.Id);
                    queryExecuter.ExecuteAndFetch(builderInsertIResMeas, reader =>
                    {
                        var res = reader.Read();
                        if (res)
                            valInsResMeas = reader.GetValue(c => c.Id);
                        return res;
                    });

                    if (valInsResMeas > -1)
                    {
                        int recordCount = 0;
                        var queryFreqSample = this._dataLayer.GetBuilder<MD.IFreqSampleRaw>()
                        .From()
                        .Select(c => c.Id, c => c.Freq_MHz, c => c.Level_dBm, c => c.Level_dBmkVm, c => c.LevelMin_dBm, c => c.LevelMax_dBm, c => c.OccupationPt)
                        .Where(c => c.ResMeasId, ConditionOperator.Equal, @event.ResultId);
                        queryExecuter.Fetch(queryFreqSample, reader =>
                        {
                            var result = reader.Read();
                            if (result)
                            {
                                var Freq_MHz = reader.GetValue(c => c.Freq_MHz);
                                var OccupationPt = reader.GetValue(c => c.OccupationPt);
                                var LevelMax_dBm = reader.GetValue(c => c.LevelMax_dBm);
                                var LevelMin_dBm = reader.GetValue(c => c.LevelMin_dBm);
                                var Level_dBm = reader.GetValue(c => c.Level_dBm);
                                var Level_dBmkVm = reader.GetValue(c => c.Level_dBmkVm);

                                if (Freq_MHz.HasValue && (Freq_MHz < 0 || Freq_MHz > 400000))
                                {
                                    WriteLog("Incorrect value Freq_MHz");
                                    return false;
                                }
                                if (OccupationPt.HasValue && (OccupationPt < 0 || OccupationPt > 100))
                                {
                                    WriteLog("Incorrect value OccupationPt");
                                    return false;
                                }
                                if (LevelMax_dBm.HasValue && (LevelMax_dBm < -150 || LevelMax_dBm > 20))
                                {
                                    WriteLog("Incorrect value LevelMax_dBm");
                                }
                                if (LevelMin_dBm.HasValue && (LevelMin_dBm < -150 || LevelMin_dBm > 20))
                                {
                                    WriteLog("Incorrect value LevelMin_dBm");
                                }
                                if (Level_dBm.HasValue && (Level_dBm < -150 || Level_dBm > 20))
                                {
                                    WriteLog("Incorrect value Level_dBm");
                                }
                                if (Level_dBmkVm.HasValue && (Level_dBmkVm < 10 || Level_dBmkVm > 140))
                                {
                                    WriteLog("Incorrect value Level_dBmkVm");
                                }

                                recordCount++;
                                var builderInsertFreqSample = this._dataLayer.GetBuilder<MD.IFreqSample>().Insert();
                                builderInsertFreqSample.SetValue(c => c.Freq_MHz, Freq_MHz);
                                builderInsertFreqSample.SetValue(c => c.LevelMax_dBm, LevelMax_dBm);
                                builderInsertFreqSample.SetValue(c => c.LevelMin_dBm, LevelMin_dBm);
                                builderInsertFreqSample.SetValue(c => c.Level_dBm, Level_dBm);
                                builderInsertFreqSample.SetValue(c => c.Level_dBmkVm, Level_dBmkVm);
                                builderInsertFreqSample.SetValue(c => c.OccupationPt, OccupationPt);
                                builderInsertFreqSample.SetValue(c => c.ResMeasId, valInsResMeas);
                                queryExecuter.Execute(builderInsertFreqSample);
                            }
                            return result;
                        });
                        if (recordCount == 0)
                            validationResult = false;

                        var queryLoc = this._dataLayer.GetBuilder<MD.IResLocSensorRaw>()
                        .From()
                        .Select(c => c.Id, c => c.Lon, c => c.Lat, c => c.Agl, c => c.Asl)
                        .Where(c => c.ResMeasId, ConditionOperator.Equal, @event.ResultId);
                        queryExecuter.Fetch(queryLoc, reader =>
                        {
                            var result = reader.Read();
                            if (result)
                            {
                                var Lon = reader.GetValue(c => c.Lon);
                                var Lat = reader.GetValue(c => c.Lat);
                                var Asl = reader.GetValue(c => c.Asl);
                                var Agl = reader.GetValue(c => c.Agl);

                                if (Lon.HasValue && (Lon < -180 || Lon > 180))
                                {
                                    WriteLog("Incorrect value Lon");
                                    return false;
                                }
                                if (Lat.HasValue && (Lat < -90 || Lat > 90))
                                {
                                    WriteLog("Incorrect value Lat");
                                    return false;
                                }
                                if (Asl.HasValue && (Asl < -1000 || Asl > 9000))
                                {
                                    WriteLog("Incorrect value Asl");
                                }
                                if (Agl.HasValue && (Agl < -100 || Agl > 500))
                                {
                                    WriteLog("Incorrect value Agl");
                                }

                                var builderInsertResLocSensorMeas = this._dataLayer.GetBuilder<MD.IResLocSensorMeas>().Insert();
                                builderInsertResLocSensorMeas.SetValue(c => c.Agl, Agl);
                                builderInsertResLocSensorMeas.SetValue(c => c.Asl, Asl);
                                builderInsertResLocSensorMeas.SetValue(c => c.Lon, Lon);
                                builderInsertResLocSensorMeas.SetValue(c => c.Lat, Lat);
                                builderInsertResLocSensorMeas.SetValue(c => c.ResMeasId, valInsResMeas);
                                builderInsertResLocSensorMeas.Select(c => c.Id);
                                queryExecuter.Execute(builderInsertResLocSensorMeas);
                            }
                            return result;
                        });
                    }

                    if (validationResult)
                    {
                        queryExecuter.CommitTransaction();
                    }
                    else
                    {
                        queryExecuter.RollbackTransaction();
                    }

                    var builderDelMeas = this._dataLayer.GetBuilder<MD.IResMeasRaw>().Delete();
                    builderDelMeas.Where(c => c.Id, ConditionOperator.Equal, @event.ResultId);
                    queryExecuter.Execute(builderDelMeas);

                    var builderDelFreqSample = this._dataLayer.GetBuilder<MD.IFreqSampleRaw>().Delete();
                    builderDelFreqSample.Where(c => c.ResMeasId, ConditionOperator.Equal, @event.ResultId);
                    queryExecuter.Execute(builderDelFreqSample);

                    var builderDelLocSensor = this._dataLayer.GetBuilder<MD.IResLocSensorMeas>().Delete();
                    builderDelLocSensor.Where(c => c.ResMeasId, ConditionOperator.Equal, @event.ResultId);
                    queryExecuter.Execute(builderDelLocSensor);
                }

                    //if (TypeMeasurements == DataModels.Sdrns.MeasurementType.MonitoringStations)
                    //{
                    //    if (string.IsNullOrEmpty(MeasResultSID))
                    //        WriteLog("Undefined value ResultId");
                    //    if (string.IsNullOrEmpty(MeasTaskId))
                    //        WriteLog("Undefined value TaskId");
                    //    if (MeasResultSID.Length > 50)
                    //        MeasResultSID.Substring(0, 50);
                    //    if (MeasTaskId.Length > 200)
                    //        MeasTaskId.Substring(0, 200);
                    //    if (Status.Length > 5)
                    //        Status = "";
                    //    if (DataRank < 0 || DataRank > 10000)
                    //        WriteLog("Incorrect value SwNumber");

                    //    var querySta = this._dataLayer.GetBuilder<MD.IResMeasStaRaw>()
                    //    .From()
                    //    .Select(c => c.Id)
                    //    .Select(c => c.GlobalSID)
                    //    .Select(c => c.MeasGlobalSID)
                    //    .Select(c => c.SectorId)
                    //    .Select(c => c.IdStation)
                    //    .Select(c => c.Status)
                    //    .Select(c => c.ResMeasId)
                    //    .Select(c => c.Standard)
                    //    .Select(c => c.StationId)
                    //    .Where(c => c.ResMeasId, ConditionOperator.Equal, @event.ResultId)
                    //    .OrderByAsc(c => c.Id);
                    //    queryExecuter.Fetch(querySta, reader =>
                    //    {
                    //        var result = reader.Read();
                    //        if (result)
                    //        {
                    //            MeasTaskId = reader.GetValue(c => c.MeasTaskId);
                    //            MeasSubTaskId = reader.GetValue(c => c.MeasSubTaskId);
                    //            MeasSubTaskStationId = reader.GetValue(c => c.MeasSubTaskStationId);
                    //            SensorId = reader.GetValue(c => c.SensorId);
                    //            AntVal = reader.GetValue(c => c.AntVal);
                    //            TimeMeas = reader.GetValue(c => c.TimeMeas);
                    //            DataRank = reader.GetValue(c => c.DataRank);
                    //            N = reader.GetValue(c => c.N);
                    //            Status = reader.GetValue(c => c.Status);
                    //            TypeMeasurements = (DataModels.Sdrns.MeasurementType)Enum.Parse(typeof(DataModels.Sdrns.MeasurementType), reader.GetValue(c => c.TypeMeasurements), true);
                    //            MeasResultSID = reader.GetValue(c => c.MeasResultSID);
                    //            Synchronized = reader.GetValue(c => c.Synchronized);
                    //            StartTime = reader.GetValue(c => c.StartTime);
                    //            StopTime = reader.GetValue(c => c.StopTime);
                    //            ScansNumber = reader.GetValue(c => c.ScansNumber);
                    //        }
                    //        return result;
                    //    });

                    //    for (int i = 0; i < measResult.StationResults.Count(); i++)
                    //    {
                    //        if (measResult.StationResults[i].StationId.Length > 50)
                    //            measResult.StationResults[i].StationId.Substring(0, 50);
                    //        if (measResult.StationResults[i].TaskGlobalSid.Length > 50)
                    //            measResult.StationResults[i].TaskGlobalSid.Substring(0, 50);
                    //        if (measResult.StationResults[i].RealGlobalSid.Length > 50)
                    //            measResult.StationResults[i].RealGlobalSid.Substring(0, 50);
                    //        if (measResult.StationResults[i].SectorId.Length > 50)
                    //            measResult.StationResults[i].SectorId.Substring(0, 50);
                    //        if (measResult.StationResults[i].Status.Length > 5)
                    //            measResult.StationResults[i].Status.Substring(0, 5);
                    //        if (measResult.StationResults[i].Standard.Length > 10)
                    //            measResult.StationResults[i].Standard.Substring(0, 10);

                    //        var failedLevelResults = new List<int>();

                    //        for (int j = 0; j < measResult.StationResults[i].LevelResults.Count(); j++)
                    //        {
                    //            if (!measResult.StationResults[i].LevelResults[j].Level_dBm.HasValue && measResult.StationResults[i].LevelResults[j].Level_dBm.Value < -150 && measResult.StationResults[i].LevelResults[j].Level_dBm.Value > 20)
                    //            {
                    //                failedLevelResults.Add(j);
                    //                continue;
                    //            }
                    //            if (!measResult.StationResults[i].LevelResults[j].Level_dBmkVm.HasValue && measResult.StationResults[i].LevelResults[j].Level_dBmkVm.Value < -10 && measResult.StationResults[i].LevelResults[j].Level_dBmkVm.Value > 140)
                    //            {
                    //                failedLevelResults.Add(j);
                    //                continue;
                    //            }
                    //            if (!measResult.StationResults[i].LevelResults[j].DifferenceTimeStamp_ns.HasValue && measResult.StationResults[i].LevelResults[j].DifferenceTimeStamp_ns.Value < 0 && measResult.StationResults[i].LevelResults[j].Level_dBmkVm.Value > 999999999)
                    //            {
                    //                WriteLog("Incorrect value SwNumber");
                    //            }
                    //        }
                    //    }
            }
            catch (Exception)
            {
                queryExecuter.RollbackTransaction();
            }
        }
        private void WriteLog(string msg)
        {

        }
    }
}
