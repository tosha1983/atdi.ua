using Atdi.Contracts.Api.EventSystem;
using Atdi.Contracts.CoreServices.DataLayer;
using Atdi.Contracts.CoreServices.EntityOrm;
using Atdi.Contracts.Sdrn.Server;
using Atdi.DataModels.DataConstraint;
using Atdi.DataModels.Sdrns.Device;
using Atdi.Platform.Logging;
using System;
using MD = Atdi.DataModels.Sdrns.Server.Entities;
using MSG = Atdi.DataModels.Sdrns.BusMessages;
using MDE = Atdi.Modules.Sdrn.Server.Events;
using Atdi.Common;
using System.Collections.Generic;

namespace Atdi.AppUnits.Sdrn.Server.PrimaryHandlers.Handlers
{
    public class SendMeasResultsHandler : ISdrnMessageHandler<MSG.Device.SendMeasResultsMessage, MeasResults>
    {
        private readonly ISdrnMessagePublisher _messagePublisher;
        private readonly IDataLayer<EntityDataOrm> _dataLayer;
        private readonly ISdrnServerEnvironment _environment;
        private readonly IEventEmitter _eventEmitter;
        private readonly ILogger _logger;
        private readonly Configs _configs;

        public SendMeasResultsHandler(
            ISdrnMessagePublisher messagePublisher, 
            IDataLayer<EntityDataOrm> dataLayer, 
            ISdrnServerEnvironment environment,
            Configs configs,
            IEventEmitter eventEmitter, ILogger logger)
        {
            this._messagePublisher = messagePublisher;
            this._dataLayer = dataLayer;
            this._environment = environment;
            this._eventEmitter = eventEmitter;
            this._logger = logger;
            this._configs = configs;
        }

        public void GetMeasTaskSDRIdentifier(string ResultIds, string TaskId, string SensorName, string SensorTechId, out int SubTaskId, out int SubTaskStationId, out int SensorId, out int ResultId, out int TaskIdOut)
        {
            TaskIdOut = -1;
            SubTaskId = -1;
            SubTaskStationId = -1;
            SensorId = -1;
            ResultId = -1;
            int SensorIdTemp = -1;

            var queryExecuter = this._dataLayer.Executor<SdrnServerDataContext>();

            if (TaskId != null)
            {
                TaskId = TaskId.Replace("||", "|");
                string[] word = TaskId.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
                if ((word != null) && (word.Length == 4))
                {
                    TaskIdOut = int.Parse(word[0]);
                    SubTaskId = int.Parse(word[1]);
                    SubTaskStationId = int.Parse(word[2]);
                    SensorId = int.Parse(word[3]);
                }
            }

            if (ResultIds != null)
            {
                ResultIds = ResultIds.Replace("||", "|");
                string[] word = ResultIds.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
                if ((word != null) && (word.Length == 5))
                {
                    SubTaskId = int.Parse(word[1]);
                    SubTaskStationId = int.Parse(word[2]);
                    SensorId = int.Parse(word[3]);
                    ResultId = int.Parse(word[4]);
                }
                else
                {
                    int SubTaskIdTemp = -1;
                    int SubTaskStationIdTemp = -1;
                    int taskId = -1;
                    if (int.TryParse(TaskId, out taskId))
                    {
                        var builderFromIMeasSubTaskSta = this._dataLayer.GetBuilder<MD.IMeasSubTaskSta>().From();
                        builderFromIMeasSubTaskSta.Select(c => c.Id, c => c.MEASSUBTASK.Id, c => c.SensorId, c => c.MEASSUBTASK.TimeStart);
                        builderFromIMeasSubTaskSta.Where(c => c.MEASSUBTASK.MEASTASK.Id, ConditionOperator.Equal, taskId);
                        builderFromIMeasSubTaskSta.OrderByDesc(c => c.MEASSUBTASK.TimeStart);
                        queryExecuter.Fetch(builderFromIMeasSubTaskSta, reader =>
                        {
                            while (reader.Read())
                            {
                                SubTaskIdTemp = reader.GetValue(c => c.MEASSUBTASK.Id);
                                SubTaskStationIdTemp = reader.GetValue(c => c.Id);
                                if (reader.GetValue(c => c.SensorId).HasValue)
                                {
                                    SensorIdTemp = reader.GetValue(c => c.SensorId).Value;
                                }
                                break;
                            }
                            return true;
                        });

                        SubTaskId = SubTaskIdTemp;
                        SubTaskStationId = SubTaskStationIdTemp;
                        SensorId = SensorIdTemp;
                    }
                }
            }

            if (SensorId == -1)
            {
                var builderISensor = this._dataLayer.GetBuilder<MD.ISensor>().From();
                builderISensor.Select(c => c.Id, c => c.Name, c => c.TechId);
                builderISensor.Where(c => c.Name, ConditionOperator.Equal, SensorName);
                builderISensor.Where(c => c.TechId, ConditionOperator.Equal, SensorTechId);
                queryExecuter.Fetch(builderISensor, reader =>
                {
                    while (reader.Read())
                    {
                        SensorIdTemp = reader.GetValue(c => c.Id);
                    }
                    return true;
                });
                SensorId = SensorIdTemp;
            }
        }

        public void Handle(ISdrnIncomingEnvelope<MeasResults> incomingEnvelope, ISdrnMessageHandlingResult result)
        {
            using (this._logger.StartTrace(Contexts.PrimaryHandler, Categories.MessageProcessing, this))
            {
                var resSysInfoData = this._configs.ResSysInfoData;
                var queryExecuter = this._dataLayer.Executor<SdrnServerDataContext>();
                result.Status = SdrnMessageHandlingStatus.Unprocessed;
                int valInsResMeas = 0;
                try
                {
                    bool isCancelled = false;
                    queryExecuter.BeginTransaction();
                    var resObject = incomingEnvelope.DeliveryObject;
                    

                    GetMeasTaskSDRIdentifier(resObject.ResultId, resObject.TaskId, incomingEnvelope.SensorName, incomingEnvelope.SensorTechId, out int SubMeasTaskId, out int SubMeasTaskStationId, out int SensorId, out int resultId, out int taskIdOut);

                    if (resObject.Measurement== DataModels.Sdrns.MeasurementType.MonitoringStations)
                    {
                        var queryIResMeasRaw = this._dataLayer.GetBuilder<MD.IResMeasRaw>()
                        .From()
                        .Select(c => c.Id, c => c.MeasResultSID)
                        .Where(c => c.MeasResultSID, ConditionOperator.Equal, resObject.ResultId)
                        .Where(c => c.MeasTaskId, ConditionOperator.Equal, taskIdOut.ToString())
                        .Where(c => c.MeasSubTaskId, ConditionOperator.Equal, SubMeasTaskId)
                        .Where(c => c.MeasSubTaskStationId, ConditionOperator.Equal, SubMeasTaskStationId)
                        .Where(c => c.SensorId, ConditionOperator.Equal, SensorId);
                        queryExecuter.Fetch(queryIResMeasRaw, readerResMeasRaw =>
                        {
                            while (readerResMeasRaw.Read())
                            {
                                var builderInsertLogs = this._dataLayer.GetBuilder<MD.ILogs>().Insert();
                                builderInsertLogs.SetValue(c => c.Lcount, 1);
                                builderInsertLogs.SetValue(c => c.TableName, "IResMeasRaw");
                                builderInsertLogs.SetValue(c => c.When, DateTime.Now);
                                builderInsertLogs.SetValue(c => c.Event, Categories.MessageProcessing.ToString());
                                builderInsertLogs.SetValue(c => c.Who, Contexts.PrimaryHandler.ToString());
                                builderInsertLogs.SetValue(c => c.Info, Events.IsAlreadySaveResults.With(resObject.ResultId, readerResMeasRaw.GetValue(c => c.Id)).ToString());
                                builderInsertLogs.Select(c => c.Id);
                                queryExecuter.ExecuteAndFetch(builderInsertLogs, readerLogs =>
                                {
                                    return true;
                                });

                                this._logger.Warning(Contexts.PrimaryHandler, Categories.MessageProcessing, Events.IsAlreadySaveResults.With(resObject.ResultId, readerResMeasRaw.GetValue(c => c.Id)).ToString());
                                isCancelled = true;
                                break;
                            }
                            return true;
                        });

                        result.Status = SdrnMessageHandlingStatus.Confirmed;
                    }

                    if (isCancelled)
                    {
                        queryExecuter.CommitTransaction();
                        return;
                    }


                    var builderInsertIResMeas = this._dataLayer.GetBuilder<MD.IResMeasRaw>().Insert();
                    builderInsertIResMeas.SetValue(c => c.TimeMeas, resObject.Measured);
                    if (taskIdOut == -1)
                    {
                        builderInsertIResMeas.SetValue(c => c.MeasTaskId, resObject.TaskId);
                    }
                    else
                    {
                        builderInsertIResMeas.SetValue(c => c.MeasTaskId, taskIdOut.ToString());
                    }
                    builderInsertIResMeas.SetValue(c => c.SensorId, SensorId);
                    builderInsertIResMeas.SetValue(c => c.MeasSubTaskId, SubMeasTaskId);
                    builderInsertIResMeas.SetValue(c => c.MeasSubTaskStationId, SubMeasTaskStationId);
                    builderInsertIResMeas.SetValue(c => c.DataRank, resObject.SwNumber);
                    //builderInsertIResMeas.SetValue(c => c.Status, resObject.Status);
                    builderInsertIResMeas.SetValue(c => c.Status, "N");
                    builderInsertIResMeas.SetValue(c => c.TypeMeasurements, resObject.Measurement.ToString());
                    builderInsertIResMeas.SetValue(c => c.MeasResultSID, resObject.ResultId);
                    builderInsertIResMeas.SetValue(c => c.StartTime, resObject.StartTime);
                    builderInsertIResMeas.SetValue(c => c.StopTime, resObject.StopTime);
                    builderInsertIResMeas.SetValue(c => c.ScansNumber, resObject.ScansNumber);
                    builderInsertIResMeas.Select(c => c.Id);
                    queryExecuter
                    .ExecuteAndFetch(builderInsertIResMeas, reader =>
                    {
                        var res = reader.Read();
                        if (res)
                        {
                            valInsResMeas = reader.GetValue(c => c.Id);
                        }
                        return res;
                    });

                    if (valInsResMeas > 0)
                    {
                        if (resObject.RefLevels != null)
                        {
                            // пишем RefLevels только для первого результата
                            //if (resultId == 1)
                            {
                                int valInsReferenceLevelsRaw = 0;
                                var refLevels = resObject.RefLevels;
                                var builderInsertReferenceLevelsRaw = this._dataLayer.GetBuilder<MD.IReferenceLevelsRaw>().Insert();
                                builderInsertReferenceLevelsRaw.SetValue(c => c.StartFrequency_Hz, refLevels.StartFrequency_Hz);
                                builderInsertReferenceLevelsRaw.SetValue(c => c.StepFrequency_Hz, refLevels.StepFrequency_Hz);
                                builderInsertReferenceLevelsRaw.SetValue(c => c.ResMeasId, valInsResMeas);
                                if (refLevels.levels != null)
                                {
                                    builderInsertReferenceLevelsRaw.SetValue(c => c.ReferenceLevels, BinaryDecoder.ObjectToByteArray(refLevels.levels));
                                }
                                builderInsertReferenceLevelsRaw.Select(c => c.Id);
                                queryExecuter
                                .ExecuteAndFetch(builderInsertReferenceLevelsRaw, readerReferenceLevelsRaw =>
                                {
                                    var res = readerReferenceLevelsRaw.Read();
                                    if (res)
                                    {
                                        valInsReferenceLevelsRaw = readerReferenceLevelsRaw.GetValue(c => c.Id);
                                    }
                                    return true;
                                });
                            }
                        }

                        if (resObject.Emittings != null)
                        {
                            var emittings = resObject.Emittings;
                            for (int l = 0; l < emittings.Length; l++)
                            {
                                int valInsReferenceEmittingRaw = 0;
                                var builderInsertEmittingRaw = this._dataLayer.GetBuilder<MD.IEmittingRaw>().Insert();
                                builderInsertEmittingRaw.SetValue(c => c.CurentPower_dBm, emittings[l].CurentPower_dBm);
                                builderInsertEmittingRaw.SetValue(c => c.MeanDeviationFromReference, emittings[l].MeanDeviationFromReference);
                                builderInsertEmittingRaw.SetValue(c => c.ReferenceLevel_dBm, emittings[l].ReferenceLevel_dBm);
                                builderInsertEmittingRaw.SetValue(c => c.ResMeasId, valInsResMeas);
                                builderInsertEmittingRaw.SetValue(c => c.SensorId, emittings[l].SensorId);
                                if (emittings[l].EmittingParameters != null)
                                {
                                    builderInsertEmittingRaw.SetValue(c => c.RollOffFactor, emittings[l].EmittingParameters.RollOffFactor);
                                    builderInsertEmittingRaw.SetValue(c => c.StandardBW, emittings[l].EmittingParameters.StandardBW);
                                }
                                builderInsertEmittingRaw.SetValue(c => c.StartFrequency_MHz, emittings[l].StartFrequency_MHz);
                                builderInsertEmittingRaw.SetValue(c => c.StopFrequency_MHz, emittings[l].StopFrequency_MHz);
                                builderInsertEmittingRaw.SetValue(c => c.TriggerDeviationFromReference, emittings[l].TriggerDeviationFromReference);


                                var levelsDistribution = emittings[l].LevelsDistribution;
                                if (levelsDistribution != null)
                                {
                                    var outListStrings = new List<string>();
                                    for (int p=0; p< levelsDistribution.Levels.Length; p++)
                                    {
                                        outListStrings.Add(string.Format("{0} {1}", levelsDistribution.Levels[p], levelsDistribution.Count[p]));
                                    }
                                    var outString = string.Join(";", outListStrings);
                                    builderInsertEmittingRaw.SetValue(c => c.LevelsDistribution, BinaryDecoder.ObjectToByteArray(outString));
                                }

                                builderInsertEmittingRaw.Select(c => c.Id);
                                queryExecuter
                                .ExecuteAndFetch(builderInsertEmittingRaw, readerEmittingRaw =>
                                {
                                    var res = readerEmittingRaw.Read();
                                    if (res)
                                    {
                                        valInsReferenceEmittingRaw = readerEmittingRaw.GetValue(c=>c.Id);
                                        if (valInsReferenceEmittingRaw > 0)
                                        {
                                            var workTimes = emittings[l].WorkTimes;
                                            if (workTimes != null)
                                            {
                                                var lstInsWorkTimeRaw = new IQueryInsertStatement<MD.IWorkTimeRaw>[workTimes.Length];
                                                for (int r = 0; r < workTimes.Length; r++)
                                                {
                                                    var builderInsertIWorkTimeRaw = this._dataLayer.GetBuilder<MD.IWorkTimeRaw>().Insert();
                                                    builderInsertIWorkTimeRaw.SetValue(c => c.EmittingId, valInsReferenceEmittingRaw);
                                                    builderInsertIWorkTimeRaw.SetValue(c => c.HitCount, workTimes[r].HitCount);
                                                    builderInsertIWorkTimeRaw.SetValue(c => c.PersentAvailability, workTimes[r].PersentAvailability);
                                                    builderInsertIWorkTimeRaw.SetValue(c => c.StartEmitting, workTimes[r].StartEmitting);
                                                    builderInsertIWorkTimeRaw.SetValue(c => c.StopEmitting, workTimes[r].StopEmitting);
                                                    builderInsertIWorkTimeRaw.Select(c => c.Id);
                                                    lstInsWorkTimeRaw[r] = builderInsertIWorkTimeRaw;
                                                }
                                                queryExecuter.ExecuteAndFetch(lstInsWorkTimeRaw, readerWorkTimeRaw =>
                                                {
                                                    return true;
                                                });
                                            }

                                            var spectrum = emittings[l].Spectrum;
                                            if (spectrum != null)
                                            {
                                                int valInsSpectrumRaw = 0;

                                                var builderInsertISpectrumRaw = this._dataLayer.GetBuilder<MD.ISpectrumRaw>().Insert();
                                                builderInsertISpectrumRaw.SetValue(c => c.EmittingId, valInsReferenceEmittingRaw);
                                                builderInsertISpectrumRaw.SetValue(c => c.CorrectnessEstimations, spectrum.СorrectnessEstimations == true ? 1: 0);
                                                builderInsertISpectrumRaw.SetValue(c => c.Contravention, spectrum.Contravention == true ? 1 : 0);
                                                builderInsertISpectrumRaw.SetValue(c => c.Bandwidth_kHz, spectrum.Bandwidth_kHz);
                                                builderInsertISpectrumRaw.SetValue(c => c.MarkerIndex, spectrum.MarkerIndex);
                                                builderInsertISpectrumRaw.SetValue(c => c.SignalLevel_dBm, spectrum.SignalLevel_dBm);
                                                builderInsertISpectrumRaw.SetValue(c => c.SpectrumStartFreq_MHz, spectrum.SpectrumStartFreq_MHz);
                                                builderInsertISpectrumRaw.SetValue(c => c.SpectrumSteps_kHz, spectrum.SpectrumSteps_kHz);
                                                builderInsertISpectrumRaw.SetValue(c => c.T1, spectrum.T1);
                                                builderInsertISpectrumRaw.SetValue(c => c.T2, spectrum.T2);
                                                builderInsertISpectrumRaw.SetValue(c => c.TraceCount, spectrum.TraceCount);
                                                if (spectrum.Levels_dBm != null)
                                                {
                                                    builderInsertISpectrumRaw.SetValue(c => c.LevelsdBm, BinaryDecoder.ObjectToByteArray(spectrum.Levels_dBm));
                                                }
                                                builderInsertISpectrumRaw.Select(c => c.Id);
                                                queryExecuter
                                                .ExecuteAndFetch(builderInsertISpectrumRaw, readerISpectrumRaw =>
                                                {
                                                    var resSpectrumRaw = readerISpectrumRaw.Read();
                                                    if (resSpectrumRaw)
                                                    {
                                                        valInsSpectrumRaw = readerISpectrumRaw.GetValue(c => c.Id);
                                                    }
                                                    return true;
                                                });
                                            }

                                            var signalMask = emittings[l].SignalMask;
                                            if (signalMask != null)
                                            {
                                                var lstInsSignalMaskRaw = new IQueryInsertStatement<MD.ISignalMaskRaw>[signalMask.Freq_kHz.Length];
                                                for (int k = 0; k < signalMask.Freq_kHz.Length; k++)
                                                {
                                                    var freq_kH = signalMask.Freq_kHz[k];
                                                    var loss_dB = signalMask.Loss_dB[k];

                                                    var builderInsertSignalMaskRaw = this._dataLayer.GetBuilder<MD.ISignalMaskRaw>().Insert();
                                                    builderInsertSignalMaskRaw.SetValue(c => c.Freq_kHz, freq_kH);
                                                    builderInsertSignalMaskRaw.SetValue(c => c.Loss_dB, loss_dB);
                                                    builderInsertSignalMaskRaw.SetValue(c => c.EmittingId, valInsReferenceEmittingRaw);
                                                    builderInsertSignalMaskRaw.Select(c => c.Id);
                                                    lstInsSignalMaskRaw[k] = builderInsertSignalMaskRaw;
                                                }
                                                queryExecuter.ExecuteAndFetch(lstInsSignalMaskRaw, readerSignalMaskRaw =>
                                                {
                                                    return true;
                                                });
                                            }
                                        }
                                    }
                                    return true;
                                });
                            }
                        }



                        if (resObject.BandwidthResult != null)
                        {
                            int valInsBWMeasResultRaw = 0;
                            var builderInsertBWMeasResultRaw = this._dataLayer.GetBuilder<MD.IBWMeasResultRaw>().Insert();
                            builderInsertBWMeasResultRaw.SetValue(c => c.BW_kHz, resObject.BandwidthResult.Bandwidth_kHz);
                            builderInsertBWMeasResultRaw.SetValue(c => c.MarkerIndex, resObject.BandwidthResult.MarkerIndex);
                            builderInsertBWMeasResultRaw.SetValue(c => c.T1, resObject.BandwidthResult.T1);
                            builderInsertBWMeasResultRaw.SetValue(c => c.T2, resObject.BandwidthResult.T2);
                            builderInsertBWMeasResultRaw.SetValue(c => c.TraceCount, resObject.BandwidthResult.TraceCount);
                            builderInsertBWMeasResultRaw.SetValue(c => c.Сorrectnessestim, resObject.BandwidthResult.СorrectnessEstimations == true ? 1 : 0);
                            builderInsertBWMeasResultRaw.SetValue(c => c.ResMeasId, valInsResMeas);
                            builderInsertBWMeasResultRaw.Select(c => c.Id);
                            queryExecuter
                            .ExecuteAndFetch(builderInsertBWMeasResultRaw, reader =>
                            {
                                var res = reader.Read();
                                if (res)
                                {
                                    valInsBWMeasResultRaw = reader.GetValue(c => c.Id);
                                }
                                return res;
                            });
                        }

                        if (resObject.Frequencies != null)
                        {
                            var lstIns = new IQueryInsertStatement<MD.IResFreqLevelsRaw>[resObject.Frequencies.Length];
                            for (int i = 0; i < resObject.Frequencies.Length; i++)
                            {
                                var builderInsertResFreqLevelsRaw = this._dataLayer.GetBuilder<MD.IResFreqLevelsRaw>().Insert();
                                builderInsertResFreqLevelsRaw.SetValue(c => c.Freq_MHz, resObject.Frequencies[i]);
                                builderInsertResFreqLevelsRaw.SetValue(c => c.ResMeasId, valInsResMeas);
                                builderInsertResFreqLevelsRaw.Select(c => c.Id);
                                lstIns[i] = builderInsertResFreqLevelsRaw;

                            }
                            queryExecuter.ExecuteAndFetch(lstIns, reader =>
                            {
                                return true;
                            });
                            
                        }


                        if (resObject.Levels_dBm != null)
                        {
                            var lstIns = new IQueryInsertStatement<MD.IResFreqLevelsRaw>[resObject.Levels_dBm.Length];
                            for (int i = 0; i < resObject.Levels_dBm.Length; i++)
                            {
                                var builderInsertResFreqLevelsRaw = this._dataLayer.GetBuilder<MD.IResFreqLevelsRaw>().Insert();
                                builderInsertResFreqLevelsRaw.SetValue(c => c.Level_dBm, resObject.Levels_dBm[i]);
                                builderInsertResFreqLevelsRaw.SetValue(c => c.ResMeasId, valInsResMeas);
                                builderInsertResFreqLevelsRaw.Select(c => c.Id);
                                lstIns[i] = builderInsertResFreqLevelsRaw;
                            }
                            queryExecuter.ExecuteAndFetch(lstIns, reader =>
                            {
                                return true;
                            });
                        }

                        if (resObject.Location != null)
                        {
                            int valInsResLocSensorMeas = 0;
                            var builderInsertResLocSensorMeas = this._dataLayer.GetBuilder<MD.IResLocSensorRaw>().Insert();
                            builderInsertResLocSensorMeas.SetValue(c => c.Agl, resObject.Location.AGL);
                            builderInsertResLocSensorMeas.SetValue(c => c.Asl, resObject.Location.ASL);
                            builderInsertResLocSensorMeas.SetValue(c => c.Lon, resObject.Location.Lon);
                            builderInsertResLocSensorMeas.SetValue(c => c.Lat, resObject.Location.Lat);
                            builderInsertResLocSensorMeas.SetValue(c => c.ResMeasId, valInsResMeas);
                            builderInsertResLocSensorMeas.Select(c => c.Id);
                            queryExecuter
                            .ExecuteAndFetch(builderInsertResLocSensorMeas, reader =>
                            {
                                var res = reader.Read();
                                if (res)
                                {
                                    valInsResLocSensorMeas = reader.GetValue(c => c.Id);
                                }
                                return res;
                            });
                        }

                        if (resObject.FrequencySamples != null)
                        {
                            var lstIns = new IQueryInsertStatement<MD.IFreqSampleRaw>[resObject.FrequencySamples.Length];
                            for (int i = 0; i < resObject.FrequencySamples.Length; i++)
                            {
                                var item = resObject.FrequencySamples[i];
                                var builderInsertFreqSampleRaw = this._dataLayer.GetBuilder<MD.IFreqSampleRaw>().Insert();
                                builderInsertFreqSampleRaw.SetValue(c => c.Freq_MHz, item.Freq_MHz);
                                builderInsertFreqSampleRaw.SetValue(c => c.LevelMax_dBm, item.LevelMax_dBm);
                                builderInsertFreqSampleRaw.SetValue(c => c.LevelMin_dBm, item.LevelMin_dBm);
                                builderInsertFreqSampleRaw.SetValue(c => c.Level_dBm, item.Level_dBm);
                                builderInsertFreqSampleRaw.SetValue(c => c.Level_dBmkVm, item.Level_dBmkVm);
                                builderInsertFreqSampleRaw.SetValue(c => c.OccupationPt, item.Occupation_Pt);
                                builderInsertFreqSampleRaw.SetValue(c => c.ResMeasId, valInsResMeas);
                                builderInsertFreqSampleRaw.Select(c => c.Id);
                                lstIns[i] = builderInsertFreqSampleRaw;
                            }
                            queryExecuter.ExecuteAndFetch(lstIns, reader =>
                            {
                                return true;
                            });
                        }

                        if (resObject.Routes != null)
                        {

                            for (int l = 0; l < resObject.Routes.Length; l++)
                            {
                                Route route = resObject.Routes[l];

                                if (route.RoutePoints != null)
                                {
                                    var lstIns = new IQueryInsertStatement<MD.IResRoutesRaw>[route.RoutePoints.Length];
                                    for (int j = 0; j < route.RoutePoints.Length; j++)
                                    {
                                        var routePoint = route.RoutePoints[j];
                                        var builderInsertroutePoints = this._dataLayer.GetBuilder<MD.IResRoutesRaw>().Insert();
                                        builderInsertroutePoints.SetValue(c => c.Agl, routePoint.AGL);
                                        builderInsertroutePoints.SetValue(c => c.Asl, routePoint.ASL);
                                        builderInsertroutePoints.SetValue(c => c.FinishTime, routePoint.FinishTime);
                                        builderInsertroutePoints.SetValue(c => c.StartTime, routePoint.StartTime);
                                        builderInsertroutePoints.SetValue(c => c.RouteId, route.RouteId);
                                        builderInsertroutePoints.SetValue(c => c.PointStayType, routePoint.PointStayType.ToString());
                                        builderInsertroutePoints.SetValue(c => c.Lat, routePoint.Lat);
                                        builderInsertroutePoints.SetValue(c => c.Lon, routePoint.Lon);
                                        builderInsertroutePoints.SetValue(c => c.ResMeasId, valInsResMeas);
                                        builderInsertroutePoints.Select(c => c.Id);
                                        lstIns[j] = builderInsertroutePoints;
                                    }
                                    queryExecuter.ExecuteAndFetch(lstIns, reader =>
                                    {
                                        return true;
                                    });
                                }
                            }
                        }




                        if (resObject.StationResults != null)
                        {
                            for (int n = 0; n < resObject.StationResults.Length; n++)
                            {
                                int valInsResMeasStation = 0;
                                StationMeasResult station = resObject.StationResults[n];
                                var builderInsertResMeasStation = this._dataLayer.GetBuilder<MD.IResMeasStaRaw>().Insert();
                                builderInsertResMeasStation.SetValue(c => c.Status, station.Status);
                                builderInsertResMeasStation.SetValue(c => c.MeasGlobalSID, station.RealGlobalSid);
                                builderInsertResMeasStation.SetValue(c => c.GlobalSID, station.TaskGlobalSid);
                                builderInsertResMeasStation.SetValue(c => c.ResMeasId, valInsResMeas);
                                builderInsertResMeasStation.SetValue(c => c.Standard, station.Standard);
                                if (int.TryParse(station.StationId, out int Idstation))
                                {
                                    builderInsertResMeasStation.SetValue(c => c.StationId, Idstation);
                                }
                                if (int.TryParse(station.SectorId, out int IdSector))
                                {
                                    builderInsertResMeasStation.SetValue(c => c.SectorId, IdSector);
                                }
                                builderInsertResMeasStation.Select(c => c.Id);

                                queryExecuter
                               .ExecuteAndFetch(builderInsertResMeasStation, reader =>
                                {
                                    var res = reader.Read();
                                    if (res)
                                    {
                                        valInsResMeasStation = reader.GetValue(c => c.Id);
                                    }
                                    return res;
                                });


                                if (valInsResMeasStation > 0)
                                {
                                    if (station.Bearings != null)
                                    {
                                        if (station.Bearings.Length > 0)
                                        {
                                            var listBearings = station.Bearings;
                                            var lstInsBearingRaw = new IQueryInsertStatement<MD.IBearingRaw>[listBearings.Length];
                                            for (int p = 0; p < listBearings.Length; p++)
                                            {
                                                DirectionFindingData directionFindingData = listBearings[p];
                                                var builderInsertBearingRaw = this._dataLayer.GetBuilder<MD.IBearingRaw>().Insert();
                                                builderInsertBearingRaw.SetValue(c => c.ResMeasStaId, valInsResMeasStation);
                                                if (directionFindingData.Location != null)
                                                {
                                                    builderInsertBearingRaw.SetValue(c => c.Agl, directionFindingData.Location.AGL);
                                                    builderInsertBearingRaw.SetValue(c => c.Asl, directionFindingData.Location.ASL);
                                                    builderInsertBearingRaw.SetValue(c => c.Lon, directionFindingData.Location.Lon);
                                                    builderInsertBearingRaw.SetValue(c => c.Lat, directionFindingData.Location.Lat);
                                                }

                                                builderInsertBearingRaw.SetValue(c => c.Level_dBm, directionFindingData.Level_dBm);
                                                builderInsertBearingRaw.SetValue(c => c.Level_dBmkVm, directionFindingData.Level_dBmkVm);
                                                builderInsertBearingRaw.SetValue(c => c.MeasurementTime, directionFindingData.MeasurementTime);
                                                builderInsertBearingRaw.SetValue(c => c.Quality, directionFindingData.Quality);
                                                builderInsertBearingRaw.SetValue(c => c.AntennaAzimut, directionFindingData.AntennaAzimut);
                                                builderInsertBearingRaw.SetValue(c => c.Bandwidth_kHz, directionFindingData.Bandwidth_kHz);
                                                builderInsertBearingRaw.SetValue(c => c.Bearing, directionFindingData.Bearing);
                                                builderInsertBearingRaw.SetValue(c => c.CentralFrequency_MHz, directionFindingData.CentralFrequency_MHz);
                                                builderInsertBearingRaw.Select(c => c.Id);
                                                lstInsBearingRaw[p] = builderInsertBearingRaw;
                                            }

                                            queryExecuter.ExecuteAndFetch(lstInsBearingRaw, reader =>
                                            {
                                                return true;
                                            });
                                        }
                                    }


                                    int idLinkRes = -1;

                                    var stationIdTemp = SensorId;

                                    //if (int.TryParse(station.StationId, out int StationId))
                                    {
                                        var builderLinkResSensorRaw = this._dataLayer.GetBuilder<MD.ILinkResSensorRaw>().From();
                                        builderLinkResSensorRaw.Select(c => c.Id);
                                        builderLinkResSensorRaw.Where(c => c.ResMeasStaId, ConditionOperator.Equal, valInsResMeasStation);
                                        builderLinkResSensorRaw.Where(c => c.SensorId, ConditionOperator.Equal, stationIdTemp);
                                        queryExecuter.Fetch(builderLinkResSensorRaw, readerLinkResSensorRaw =>
                                        {
                                            while (readerLinkResSensorRaw.Read())
                                            {
                                                idLinkRes = readerLinkResSensorRaw.GetValue(c => c.Id);
                                                break;
                                            }
                                            return true;
                                        });
                                    }

                                    if (idLinkRes == -1)
                                    {
                                        var builderInsertLinkResSensor = this._dataLayer.GetBuilder<MD.ILinkResSensorRaw>().Insert();
                                        builderInsertLinkResSensor.SetValue(c => c.ResMeasStaId, valInsResMeasStation);
                                        //if (int.TryParse(station.StationId, out int StationId))
                                        //{
                                            builderInsertLinkResSensor.SetValue(c => c.SensorId, stationIdTemp);
                                        //}
                                        builderInsertLinkResSensor.Select(c => c.Id);
                                        queryExecuter
                                        .ExecuteAndFetch(builderInsertLinkResSensor, reader =>
                                        {
                                            var res = reader.Read();
                                            if (res)
                                            {
                                                idLinkRes = reader.GetValue(c => c.Id);
                                            }
                                            return res;
                                        });
                                    }


                                    var generalResult = station.GeneralResult;
                                    if (generalResult != null)
                                    {
                                        int IDResGeneral = -1;
                                        var builderInsertResStGeneral = this._dataLayer.GetBuilder<MD.IResStGeneralRaw>().Insert();
                                        builderInsertResStGeneral.SetValue(c => c.CentralFrequencyMeas, generalResult.CentralFrequencyMeas_MHz);
                                        builderInsertResStGeneral.SetValue(c => c.CentralFrequency, generalResult.CentralFrequency_MHz);
                                        builderInsertResStGeneral.SetValue(c => c.DurationMeas, generalResult.MeasDuration_sec);
                                        builderInsertResStGeneral.SetValue(c => c.Rbw, generalResult.RBW_kHz);
                                        builderInsertResStGeneral.SetValue(c => c.Vbw, generalResult.VBW_kHz);

                                        if (generalResult.BandwidthResult != null)
                                        {
                                            var bandwidthResult = generalResult.BandwidthResult;
                                            builderInsertResStGeneral.SetValue(c => c.MarkerIndex, bandwidthResult.MarkerIndex);
                                            builderInsertResStGeneral.SetValue(c => c.T1, bandwidthResult.T1);
                                            builderInsertResStGeneral.SetValue(c => c.T2, bandwidthResult.T2);
                                            builderInsertResStGeneral.SetValue(c => c.BW, bandwidthResult.Bandwidth_kHz);
                                            builderInsertResStGeneral.SetValue(c => c.TraceCount, bandwidthResult.TraceCount);
                                            builderInsertResStGeneral.SetValue(c => c.Correctnessestim, bandwidthResult.СorrectnessEstimations==true ? 1 : 0);
                                        }
                                        builderInsertResStGeneral.SetValue(c => c.OffsetFrequency, generalResult.OffsetFrequency_mk);
                                        builderInsertResStGeneral.SetValue(c => c.SpecrumStartFreq, generalResult.SpectrumStartFreq_MHz);
                                        builderInsertResStGeneral.SetValue(c => c.SpecrumSteps, generalResult.SpectrumSteps_kHz);
                                        builderInsertResStGeneral.SetValue(c => c.TimeFinishMeas, generalResult.MeasFinishTime);
                                        builderInsertResStGeneral.SetValue(c => c.TimeStartMeas, generalResult.MeasStartTime);
                                        builderInsertResStGeneral.SetValue(c => c.ResMeasStaId, valInsResMeasStation);
                                        builderInsertResStGeneral.Select(c => c.Id);
                                        queryExecuter
                                        .ExecuteAndFetch(builderInsertResStGeneral, reader =>
                                        {
                                            var res = reader.Read();
                                            if (res)
                                            {
                                                IDResGeneral = reader.GetValue(c => c.Id);
                                            }
                                            return res;
                                        });


                                        if (IDResGeneral > 0)
                                        {
                                            if (station.GeneralResult.StationSysInfo != null)
                                            {
                                                var stationSysInfo = station.GeneralResult.StationSysInfo;
                                                int IDResSysInfoGeneral = -1;
                                                var builderInsertResSysInfo = this._dataLayer.GetBuilder<MD.IResSysInfoRaw>().Insert();
                                                if (stationSysInfo.Location != null)
                                                {
                                                    var stationSysInfoLocation = stationSysInfo.Location;
                                                    builderInsertResSysInfo.SetValue(c => c.Agl, stationSysInfoLocation.AGL);
                                                    builderInsertResSysInfo.SetValue(c => c.Asl, stationSysInfoLocation.ASL);
                                                    builderInsertResSysInfo.SetValue(c => c.Lat, stationSysInfoLocation.Lat);
                                                    builderInsertResSysInfo.SetValue(c => c.Lon, stationSysInfoLocation.Lon);
                                                }
                                                builderInsertResSysInfo.SetValue(c => c.Bandwidth, stationSysInfo.BandWidth);
                                                builderInsertResSysInfo.SetValue(c => c.BaseId, stationSysInfo.BaseID);
                                                builderInsertResSysInfo.SetValue(c => c.Bsic, stationSysInfo.BSIC);
                                                builderInsertResSysInfo.SetValue(c => c.ChannelNumber, stationSysInfo.ChannelNumber);
                                                builderInsertResSysInfo.SetValue(c => c.Cid, stationSysInfo.CID);
                                                builderInsertResSysInfo.SetValue(c => c.Code, stationSysInfo.Code);
                                                builderInsertResSysInfo.SetValue(c => c.Ctoi, stationSysInfo.CtoI);
                                                builderInsertResSysInfo.SetValue(c => c.Eci, stationSysInfo.ECI);
                                                builderInsertResSysInfo.SetValue(c => c.Enodebid, stationSysInfo.eNodeBId);
                                                builderInsertResSysInfo.SetValue(c => c.Freq, stationSysInfo.Freq);
                                                builderInsertResSysInfo.SetValue(c => c.Icio, stationSysInfo.IcIo);
                                                builderInsertResSysInfo.SetValue(c => c.InbandPower, stationSysInfo.INBAND_POWER);
                                                builderInsertResSysInfo.SetValue(c => c.Iscp, stationSysInfo.ISCP);
                                                builderInsertResSysInfo.SetValue(c => c.Lac, stationSysInfo.LAC);
                                                builderInsertResSysInfo.SetValue(c => c.Mcc, stationSysInfo.MCC);
                                                builderInsertResSysInfo.SetValue(c => c.Mnc, stationSysInfo.MNC);
                                                builderInsertResSysInfo.SetValue(c => c.Nid, stationSysInfo.NID);
                                                builderInsertResSysInfo.SetValue(c => c.Pci, stationSysInfo.PCI);
                                                builderInsertResSysInfo.SetValue(c => c.Pn, stationSysInfo.PN);
                                                builderInsertResSysInfo.SetValue(c => c.Power, stationSysInfo.Power);
                                                builderInsertResSysInfo.SetValue(c => c.Ptotal, stationSysInfo.Ptotal);
                                                builderInsertResSysInfo.SetValue(c => c.Rnc, stationSysInfo.RNC);
                                                builderInsertResSysInfo.SetValue(c => c.Rscp, stationSysInfo.RSCP);
                                                builderInsertResSysInfo.SetValue(c => c.Rsrp, stationSysInfo.RSRP);
                                                builderInsertResSysInfo.SetValue(c => c.Rsrq, stationSysInfo.RSRQ);
                                                builderInsertResSysInfo.SetValue(c => c.Sc, stationSysInfo.SC);
                                                builderInsertResSysInfo.SetValue(c => c.Sid, stationSysInfo.SID);
                                                builderInsertResSysInfo.SetValue(c => c.Tac, stationSysInfo.TAC);
                                                builderInsertResSysInfo.SetValue(c => c.TypeCdmaevdo, stationSysInfo.TypeCDMAEVDO);
                                                builderInsertResSysInfo.SetValue(c => c.Ucid, stationSysInfo.UCID);
                                                builderInsertResSysInfo.SetValue(c => c.ResStGeneralId, IDResGeneral);
                                                builderInsertResSysInfo.Select(c => c.Id);

                                                queryExecuter
                                                .ExecuteAndFetch(builderInsertResSysInfo, reader =>
                                                {
                                                    var res = reader.Read();
                                                    if (res)
                                                    {
                                                        IDResSysInfoGeneral = reader.GetValue(c => c.Id);
                                                    }
                                                    return res;
                                                });


                                                if (IDResSysInfoGeneral > 0)
                                                {
                                                    if (stationSysInfo.InfoBlocks != null)
                                                    {
                                                        for (int b=0; b< stationSysInfo.InfoBlocks.Length; b++)
                                                        {
                                                            StationSysInfoBlock blocks = stationSysInfo.InfoBlocks[b];

                                                            int IDResSysInfoBlocks = -1;
                                                            var builderInsertStationSysInfoBlock = this._dataLayer.GetBuilder<MD.IResSysInfoBlsRaw>().Insert();
                                                            if (resSysInfoData==true)
                                                            {
                                                                builderInsertStationSysInfoBlock.SetValue(c => c.BinData, BinaryDecoder.ObjectToByteArray(blocks.Data));
                                                            }
                                                            builderInsertStationSysInfoBlock.SetValue(c => c.Type, blocks.Type);
                                                            builderInsertStationSysInfoBlock.SetValue(c => c.ResSysInfoId, IDResSysInfoGeneral);
                                                            builderInsertStationSysInfoBlock.Select(c => c.Id);
                                                            queryExecuter
                                                            .ExecuteAndFetch(builderInsertStationSysInfoBlock, reader =>
                                                            {
                                                                var res = reader.Read();
                                                                if (res)
                                                                {
                                                                    IDResSysInfoBlocks = reader.GetValue(c => c.Id);
                                                                }
                                                                return res;
                                                            });
                                                        }
                                                    }
                                                }
                                            }

                                            if (station.GeneralResult.BWMask != null)
                                            {
                                                if (station.GeneralResult.BWMask.Length > 0)
                                                {
                                                    var lstIns = new IQueryInsertStatement<MD.IResStMaskElementRaw>[station.GeneralResult.BWMask.Length];
                                                    for (int l = 0; l < station.GeneralResult.BWMask.Length; l++)
                                                    {
                                                        ElementsMask maskElem = station.GeneralResult.BWMask[l];
                                                        var builderInsertmaskElem = this._dataLayer.GetBuilder<MD.IResStMaskElementRaw>().Insert();
                                                        builderInsertmaskElem.SetValue(c => c.Bw, maskElem.BW_kHz);
                                                        builderInsertmaskElem.SetValue(c => c.Level, maskElem.Level_dB);
                                                        builderInsertmaskElem.SetValue(c => c.ResStGeneralId, IDResGeneral);
                                                        builderInsertmaskElem.Select(c => c.Id);
                                                        lstIns[l] = builderInsertmaskElem;
                                                    }
                                                    queryExecuter.ExecuteAndFetch(lstIns, reader =>
                                                    {
                                                        return true;
                                                    });
                                                }
                                            }


                                            if (station.GeneralResult.LevelsSpectrum_dBm != null)
                                            {
                                                if (station.GeneralResult.LevelsSpectrum_dBm.Length > 0)
                                                {
                                                    var lstIns = new IQueryInsertStatement<MD.IResStLevelsSpectRaw>[station.GeneralResult.LevelsSpectrum_dBm.Length];
                                                    for (int l = 0; l < station.GeneralResult.LevelsSpectrum_dBm.Length; l++)
                                                    {
                                                        var lvl = station.GeneralResult.LevelsSpectrum_dBm[l];
                                                        var builderInsertResStLevelsSpect = this._dataLayer.GetBuilder<MD.IResStLevelsSpectRaw>().Insert();
                                                        builderInsertResStLevelsSpect.SetValue(c => c.LevelSpecrum, lvl);
                                                        builderInsertResStLevelsSpect.SetValue(c => c.ResStGeneralId, IDResGeneral);
                                                        builderInsertResStLevelsSpect.Select(c => c.Id);
                                                        lstIns[l] = builderInsertResStLevelsSpect;
                                                    }
                                                    queryExecuter.ExecuteAndFetch(lstIns, reader =>
                                                    {
                                                        return true;
                                                    });
                                                }
                                            }


                                            if (station.LevelResults != null)
                                            {
                                                if (station.LevelResults.Length > 0)
                                                {
                                                    var lstIns = new IQueryInsertStatement<MD.IResStLevelCarRaw>[station.LevelResults.Length];
                                                    for (int l = 0; l < station.LevelResults.Length; l++)
                                                    {
                                                        LevelMeasResult car = station.LevelResults[l];
                                                        var builderInsertResStLevelCar = this._dataLayer.GetBuilder<MD.IResStLevelCarRaw>().Insert();
                                                        if (car.Location != null)
                                                        {
                                                            builderInsertResStLevelCar.SetValue(c => c.Agl, car.Location.AGL);
                                                            builderInsertResStLevelCar.SetValue(c => c.Altitude, car.Location.ASL);
                                                            builderInsertResStLevelCar.SetValue(c => c.Lon, car.Location.Lon);
                                                            builderInsertResStLevelCar.SetValue(c => c.Lat, car.Location.Lat);
                                                        }
                                                        builderInsertResStLevelCar.SetValue(c => c.DifferenceTimeStamp, car.DifferenceTimeStamp_ns);
                                                        builderInsertResStLevelCar.SetValue(c => c.LevelDbm, car.Level_dBm);
                                                        builderInsertResStLevelCar.SetValue(c => c.LevelDbmkvm, car.Level_dBmkVm);
                                                        builderInsertResStLevelCar.SetValue(c => c.TimeOfMeasurements, car.MeasurementTime);

                                                        if (station.GeneralResult != null)
                                                        {
                                                            var generalResults = station.GeneralResult;


                                                            builderInsertResStLevelCar.SetValue(c => c.CentralFrequency, generalResults.CentralFrequency_MHz);
                                                            if (generalResults.BandwidthResult != null)
                                                            {
                                                                 builderInsertResStLevelCar.SetValue(c => c.Bw, generalResults.BandwidthResult.Bandwidth_kHz);
                                                            }
                                                        }
                                                        builderInsertResStLevelCar.SetValue(c => c.ResStationId, valInsResMeasStation);
                                                        builderInsertResStLevelCar.Select(c => c.Id);
                                                        lstIns[l] = builderInsertResStLevelCar;

                                                    }
                                                    queryExecuter.ExecuteAndFetch(lstIns, reader =>
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
                    // с этого момента нужно считать что сообщение удачно обработано
                    result.Status = SdrnMessageHandlingStatus.Confirmed;
                    this._eventEmitter.Emit(new MDE.OnReceivedNewSOResultEvent() { ResultId = valInsResMeas }, new EventEmittingOptions { Rule = EventEmittingRule.Default });
                }
                catch (Exception e)
                {
                    queryExecuter.RollbackTransaction();
                    this._logger.Exception(Contexts.PrimaryHandler, Categories.MessageProcessing, e, this);
                     result.Status = SdrnMessageHandlingStatus.Error;
                     result.ReasonFailure = e.ToString();
                }
                finally
                {
                    // независимо упали мы по ошибке мы обязаны отправить ответ клиенту
                    // формируем объект подтвержденяи о обновлении данных о сенсоре
                    var deviceCommandResult = new DeviceCommand
                    {
                        EquipmentTechId = incomingEnvelope.SensorTechId,
                        SensorName = incomingEnvelope.SensorName,
                        SdrnServer = this._environment.ServerInstance,
                        Command = "SendMeasResultsConfirmed",
                        CommandId = "SendCommand",
                        CustTxt1 = "Success"
                    };

                    if (result.Status == SdrnMessageHandlingStatus.Error)
                    {
                        deviceCommandResult.CustTxt1 = "{ " + string.Format("{0}: {1}, {2}: {3}, {4}: {5}, {6}: {7} ", "\"Status\"", "\"Fault\"", "\"ResultId\"", "\"" + incomingEnvelope.DeliveryObject.ResultId + "\"", "\"Message\"", "\"" + result.ReasonFailure + "\"", "\"DateCreated\"", "\"" + DateTime.Now.ToString("dd.MM.yyyyTHH:mm:ss") + "\"") + " }";
                    }
                    else if (valInsResMeas > 0)
                    {
                        deviceCommandResult.CustTxt1 = "{ " + string.Format("{0}: {1}, {2}: {3}, {4}: {5}, {6}: {7} ", "\"Status\"", "\"Success\"", "\"ResultId\"", "\"" + incomingEnvelope.DeliveryObject.ResultId + "\"", "\"Message\"", "\"\"", "\"DateCreated\"", "\"" + DateTime.Now.ToString("dd.MM.yyyyTHH:mm:ss") + "\"") + " }";
                    }
                    else
                    {
                        deviceCommandResult.CustTxt1 = "{ " + string.Format("{0}: {1}, {2}: {3}, {4}: {5}, {6}: {7} ", "\"Status\"", "\"Fault\"", "\"ResultId\"", "\"" + incomingEnvelope.DeliveryObject.ResultId + "\"", "\"Message\"", "\"" + result.ReasonFailure + "\"", "\"DateCreated\"", "\"" + DateTime.Now.ToString("dd.MM.yyyyTHH:mm:ss") + "\"") + " }";
                    }
                    var envelop = _messagePublisher.CreateOutgoingEnvelope<MSG.Server.SendCommandMessage, DeviceCommand>();
                    envelop.SensorName = incomingEnvelope.SensorName;
                    envelop.SensorTechId = incomingEnvelope.SensorTechId;
                    envelop.DeliveryObject = deviceCommandResult;
                    _messagePublisher.Send(envelop);
                }

            }
        }
    }
}


