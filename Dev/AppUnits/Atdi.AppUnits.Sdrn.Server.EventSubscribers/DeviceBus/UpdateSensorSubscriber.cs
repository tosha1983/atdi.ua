﻿using Atdi.DataModels.Api.EventSystem;
using DM = Atdi.DataModels.Sdrns.Device;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.Contracts.Sdrn.Server.DevicesBus;
using Atdi.Platform.Logging;
using Atdi.Contracts.CoreServices.DataLayer;
using Atdi.Contracts.CoreServices.EntityOrm;
using Atdi.Contracts.Sdrn.Server;
using MD = Atdi.DataModels.Sdrns.Server.Entities;
using Dev = Atdi.DataModels.Sdrns.Device;
using Atdi.DataModels.DataConstraint;
using MSG = Atdi.DataModels.Sdrns.BusMessages;
using Atdi.Platform;
using Atdi.DataModels.Sdrns.Server.Events;
using Atdi.Contracts.Api.EventSystem;
using Atdi.Platform.Workflows;




namespace Atdi.AppUnits.Sdrn.Server.EventSubscribers.DeviceBus
{
    [SubscriptionEvent(EventName = "OnUpdateSensorDeviceBusEvent", SubscriberName = "UpdateSensorSubscriber")]
    public class UpdateSensorSubscriber : SubscriberBase<DM.Sensor>
    {

        private readonly IDataLayer<EntityDataOrm> _dataLayer;
        private readonly ISdrnServerEnvironment _environment;
        private readonly ISdrnMessagePublisher _messagePublisher;
        private readonly IStatistics _statistics;
        private readonly IEventEmitter _eventEmitter;
        private readonly IPipelineSite _pipelineSite;

        private readonly IStatisticCounter _messageProcessingHitsCounter;
        private readonly IStatisticCounter _updateSensorHitsCounter;
        private readonly IStatisticCounter _updateSensorErrorsCounter;

        public UpdateSensorSubscriber(
            ISdrnMessagePublisher messagePublisher, 
            IMessagesSite messagesSite, 
            IDataLayer<EntityDataOrm> dataLayer, 
            ISdrnServerEnvironment environment,
            IStatistics statistics,
            IEventEmitter eventEmitter,
            IPipelineSite pipelineSite,
            ILogger logger) 
            : base(messagesSite, logger)
        {
            this._pipelineSite = pipelineSite;
            this._eventEmitter = eventEmitter;
            this._messagePublisher = messagePublisher;
            this._dataLayer = dataLayer;
            this._environment = environment;
            this._statistics = statistics;
            if (this._statistics != null)
            {
                this._messageProcessingHitsCounter = _statistics.Counter(Monitoring.Counters.MessageProcessingHits);
                this._updateSensorHitsCounter = _statistics.Counter(Monitoring.Counters.UpdateSensorHits);
                this._updateSensorErrorsCounter = _statistics.Counter(Monitoring.Counters.UpdateSensorErrors);
            }
        }

        public bool UpdateSensor(DM.Sensor sensor)
        {
            var resultValue = false;
            try
            {
                using (var scope = this._dataLayer.CreateScope<SdrnServerDataContext>())
                {
                    scope.BeginTran();

                    var sensorData = sensor;
                    long idSensor = -1;
                    var query = this._dataLayer.GetBuilder<MD.ISensor>()
                    .From()
                    .Select(c => c.Name)
                    .Select(c => c.Id)
                    .Where(c => c.Name, ConditionOperator.Equal, sensorData.Name)
                    .Where(c => c.TechId, ConditionOperator.Equal, sensorData.Equipment.TechId)
                    .OrderByAsc(c => c.Id);
                    scope.Executor
                    .Fetch(query, reader =>
                    {
                        var result = reader.Read();
                        if (result)
                        {
                            idSensor = reader.GetValue(c => c.Id);
                        }
                        return result;
                    });

                    if (idSensor > 0)
                    {
                        var builderUpdateSensor = this._dataLayer.GetBuilder<MD.ISensor>().Update();
                        builderUpdateSensor.SetValue(c => c.Administration, sensorData.Administration);
                        builderUpdateSensor.SetValue(c => c.BiuseDate, sensorData.BiuseDate);
                        builderUpdateSensor.SetValue(c => c.CreatedBy, sensorData.CreatedBy);
                        builderUpdateSensor.SetValue(c => c.CustData1, sensorData.CustDate1);
                        builderUpdateSensor.SetValue(c => c.CustNbr1, sensorData.CustNbr1);
                        builderUpdateSensor.SetValue(c => c.CustTxt1, sensorData.CustTxt1);
                        builderUpdateSensor.SetValue(c => c.DateCreated, sensorData.Created);
                        builderUpdateSensor.SetValue(c => c.EouseDate, sensorData.EouseDate);
                        builderUpdateSensor.SetValue(c => c.NetworkId, sensorData.NetworkId);
                        builderUpdateSensor.SetValue(c => c.Remark, sensorData.Remark);
                        builderUpdateSensor.SetValue(c => c.RxLoss, sensorData.RxLoss);
                        builderUpdateSensor.SetValue(c => c.Status, sensorData.Status);
                        builderUpdateSensor.SetValue(c => c.StepMeasTime, sensorData.StepMeasTime);
                        builderUpdateSensor.SetValue(c => c.TypeSensor, sensorData.Type);
                        builderUpdateSensor.SetValue(c => c.Agl, sensorData.AGL);
                        builderUpdateSensor.Where(c => c.Id, ConditionOperator.Equal, idSensor);
                        if (scope.Executor
                        .Execute(builderUpdateSensor) > 0)
                        {
                            if (sensorData.Antenna != null)
                            {
                                long idSensorAntenna = -1;
                                var querySensorAntenna = this._dataLayer.GetBuilder<MD.ISensorAntenna>()
                                .From()
                                .Select(c => c.Id)
                                .Where(c => c.SENSOR.Id, ConditionOperator.Equal, idSensor)
                                .OrderByAsc(c => c.Id)
                                ;

                                scope.Executor
                                 .Fetch(querySensorAntenna, reader =>
                             {
                                 var result = reader.Read();
                                 if (result)
                                 {
                                     idSensorAntenna = reader.GetValue(c => c.Id);
                                 }
                                 return result;
                             });


                                if (idSensorAntenna == -1)
                                {
                                    var builderInsertAntenna = this._dataLayer.GetBuilder<MD.ISensorAntenna>().Insert();
                                    builderInsertAntenna.SetValue(c => c.AddLoss, sensorData.Antenna.AddLoss);
                                    builderInsertAntenna.SetValue(c => c.AntClass, sensorData.Antenna.Class);
                                    builderInsertAntenna.SetValue(c => c.AntDir, sensorData.Antenna.Direction.ToString());
                                    builderInsertAntenna.SetValue(c => c.Category, sensorData.Antenna.Category);
                                    builderInsertAntenna.SetValue(c => c.Code, sensorData.Antenna.Code);
                                    builderInsertAntenna.SetValue(c => c.CustData1, sensorData.Antenna.CustDate1);
                                    builderInsertAntenna.SetValue(c => c.CustNbr1, sensorData.Antenna.CustNbr1);
                                    builderInsertAntenna.SetValue(c => c.CustTxt1, sensorData.Antenna.CustTxt1);
                                    builderInsertAntenna.SetValue(c => c.GainMax, sensorData.Antenna.GainMax);
                                    builderInsertAntenna.SetValue(c => c.GainType, sensorData.Antenna.GainType);
                                    builderInsertAntenna.SetValue(c => c.HbeamWidth, sensorData.Antenna.HBeamwidth);
                                    builderInsertAntenna.SetValue(c => c.LowerFreq, sensorData.Antenna.LowerFreq_MHz);
                                    builderInsertAntenna.SetValue(c => c.Manufacturer, sensorData.Antenna.Manufacturer);
                                    builderInsertAntenna.SetValue(c => c.Name, sensorData.Antenna.Name);
                                    builderInsertAntenna.SetValue(c => c.Polarization, sensorData.Antenna.Polarization.ToString());
                                    builderInsertAntenna.SetValue(c => c.Remark, sensorData.Antenna.Remark);
                                    builderInsertAntenna.SetValue(c => c.Slewang, sensorData.Antenna.SlewAng);
                                    builderInsertAntenna.SetValue(c => c.TechId, sensorData.Antenna.TechId);
                                    builderInsertAntenna.SetValue(c => c.UpperFreq, sensorData.Antenna.UpperFreq_MHz);
                                    builderInsertAntenna.SetValue(c => c.UseType, sensorData.Antenna.UseType);
                                    builderInsertAntenna.SetValue(c => c.VbeamWidth, sensorData.Antenna.VBeamwidth);
                                    builderInsertAntenna.SetValue(c => c.Xpd, sensorData.Antenna.XPD);
                                    builderInsertAntenna.SetValue(c => c.SENSOR.Id, idSensor);
                                    

                                    var sensorAntenna_PK = scope.Executor.Execute<MD.ISensorAntenna_PK>(builderInsertAntenna);
                                    idSensorAntenna = sensorAntenna_PK.Id;
                                }

                                if (idSensorAntenna > 0)
                                {
                                    var builderUpdateAntenna = this._dataLayer.GetBuilder<MD.ISensorAntenna>().Update();
                                    builderUpdateAntenna.SetValue(c => c.AddLoss, sensorData.Antenna.AddLoss);
                                    builderUpdateAntenna.SetValue(c => c.AntClass, sensorData.Antenna.Class);
                                    builderUpdateAntenna.SetValue(c => c.AntDir, sensorData.Antenna.Direction.ToString());
                                    builderUpdateAntenna.SetValue(c => c.Category, sensorData.Antenna.Category);
                                    builderUpdateAntenna.SetValue(c => c.Code, sensorData.Antenna.Code);
                                    builderUpdateAntenna.SetValue(c => c.CustData1, sensorData.Antenna.CustDate1);
                                    builderUpdateAntenna.SetValue(c => c.CustNbr1, sensorData.Antenna.CustNbr1);
                                    builderUpdateAntenna.SetValue(c => c.CustTxt1, sensorData.Antenna.CustTxt1);
                                    builderUpdateAntenna.SetValue(c => c.GainMax, sensorData.Antenna.GainMax);
                                    builderUpdateAntenna.SetValue(c => c.GainType, sensorData.Antenna.GainType);
                                    builderUpdateAntenna.SetValue(c => c.HbeamWidth, sensorData.Antenna.HBeamwidth);
                                    builderUpdateAntenna.SetValue(c => c.LowerFreq, sensorData.Antenna.LowerFreq_MHz);
                                    builderUpdateAntenna.SetValue(c => c.Manufacturer, sensorData.Antenna.Manufacturer);
                                    builderUpdateAntenna.SetValue(c => c.Name, sensorData.Antenna.Name);
                                    builderUpdateAntenna.SetValue(c => c.Polarization, sensorData.Antenna.Polarization.ToString());
                                    builderUpdateAntenna.SetValue(c => c.Remark, sensorData.Antenna.Remark);
                                    builderUpdateAntenna.SetValue(c => c.Slewang, sensorData.Antenna.SlewAng);
                                    builderUpdateAntenna.SetValue(c => c.TechId, sensorData.Antenna.TechId);
                                    builderUpdateAntenna.SetValue(c => c.UpperFreq, sensorData.Antenna.UpperFreq_MHz);
                                    builderUpdateAntenna.SetValue(c => c.UseType, sensorData.Antenna.UseType);
                                    builderUpdateAntenna.SetValue(c => c.VbeamWidth, sensorData.Antenna.VBeamwidth);
                                    builderUpdateAntenna.SetValue(c => c.Xpd, sensorData.Antenna.XPD);
                                    builderUpdateAntenna.Where(c => c.Id, ConditionOperator.Equal, idSensorAntenna);
                                    if (scope.Executor
                                     .Execute(builderUpdateAntenna) > 0)
                                    {
                                        if (sensorData.Antenna.Patterns != null)
                                        {
                                            long idSensorAntennaPattern = -1;

                                            var listIdSensorAntennaPattern = new List<long>();
                                            for (int b = 0; b < sensorData.Antenna.Patterns.Length; b++)
                                            {
                                                idSensorAntennaPattern = -1;

                                                var patt = sensorData.Antenna.Patterns[b];

                                                var querySensorAntennaPatterns = this._dataLayer.GetBuilder<MD.IAntennaPattern>()
                                                .From()
                                                .Select(c => c.Id)
                                                .Where(c => c.SENSOR_ANTENNA.Id, ConditionOperator.Equal, idSensorAntenna)
                                                .Where(c => c.Freq, ConditionOperator.Equal, patt.Freq_MHz)
                                                .Where(c => c.Gain, ConditionOperator.Equal, patt.Gain)
                                                .Where(c => c.DiagA, ConditionOperator.Equal, patt.DiagA)
                                                .Where(c => c.DiagH, ConditionOperator.Equal, patt.DiagH)
                                                .Where(c => c.DiagV, ConditionOperator.Equal, patt.DiagV)
                                                .OrderByAsc(c => c.Id)
                                                ;

                                                scope.Executor
                                                .Fetch(querySensorAntennaPatterns, readerAntennaPattern =>
                                                {
                                                    var result = readerAntennaPattern.Read();
                                                    if (result)
                                                    {
                                                        idSensorAntennaPattern = readerAntennaPattern.GetValue(c => c.Id);
                                                    }
                                                    return result;
                                                });

                                                if (idSensorAntennaPattern > 0)
                                                {
                                                    listIdSensorAntennaPattern.Add(idSensorAntennaPattern);
                                                }


                                                querySensorAntennaPatterns = this._dataLayer.GetBuilder<MD.IAntennaPattern>()
                                                .From()
                                                .Select(c => c.Id)
                                                .Where(c => c.SENSOR_ANTENNA.Id, ConditionOperator.NotEqual, idSensorAntenna)
                                                .OrderByAsc(c => c.Id)
                                                ;

                                                scope.Executor
                                                .Fetch(querySensorAntennaPatterns, readerAntennaPattern =>
                                                {
                                                    while (readerAntennaPattern.Read())
                                                    {
                                                        if (!listIdSensorAntennaPattern.Contains(readerAntennaPattern.GetValue(c => c.Id)))
                                                        {
                                                            listIdSensorAntennaPattern.Add(readerAntennaPattern.GetValue(c => c.Id));
                                                        }
                                                    }
                                                    return true;
                                                });
                                            }

                                            if (listIdSensorAntennaPattern.Count > 0)
                                            {
                                                var queryDelAntennaPattern = this._dataLayer.GetBuilder<MD.IAntennaPattern>()
                                              .Delete()
                                              .Where(c => c.Id, ConditionOperator.NotIn, listIdSensorAntennaPattern.ToArray());
                                                scope.Executor.Execute(queryDelAntennaPattern);
                                            }

                                            for (int b = 0; b < sensorData.Antenna.Patterns.Length; b++)
                                            {
                                                var patt = sensorData.Antenna.Patterns[b];

                                                var querySensorAntennaPatterns = this._dataLayer.GetBuilder<MD.IAntennaPattern>()
                                                .From()
                                                .Select(c => c.Id)
                                                .Where(c => c.SENSOR_ANTENNA.Id, ConditionOperator.Equal, idSensorAntenna)
                                                .Where(c => c.Freq, ConditionOperator.Equal, patt.Freq_MHz)
                                                .Where(c => c.Gain, ConditionOperator.Equal, patt.Gain)
                                                .Where(c => c.DiagA, ConditionOperator.Equal, patt.DiagA)
                                                .Where(c => c.DiagH, ConditionOperator.Equal, patt.DiagH)
                                                .Where(c => c.DiagV, ConditionOperator.Equal, patt.DiagV)
                                                .OrderByAsc(c => c.Id)
                                                ;

                                                scope.Executor
                                                .Fetch(querySensorAntennaPatterns, reader =>
                                            {
                                                var result = reader.Read();
                                                if (result)
                                                {
                                                    idSensorAntennaPattern = reader.GetValue(c => c.Id);
                                                }
                                                return result;
                                            });

                                                if (idSensorAntennaPattern > 0)
                                                {
                                                    var builderUpdateAntennaPattern = this._dataLayer.GetBuilder<MD.IAntennaPattern>().Update();
                                                    builderUpdateAntennaPattern.SetValue(c => c.DiagA, patt.DiagA);
                                                    builderUpdateAntennaPattern.SetValue(c => c.DiagH, patt.DiagH);
                                                    builderUpdateAntennaPattern.SetValue(c => c.DiagV, patt.DiagV);
                                                    builderUpdateAntennaPattern.SetValue(c => c.Freq, patt.Freq_MHz);
                                                    builderUpdateAntennaPattern.SetValue(c => c.Gain, patt.Gain);
                                                    builderUpdateAntennaPattern.Where(c => c.Id, ConditionOperator.Equal, idSensorAntennaPattern);

                                                    scope.Executor
                                                     .Execute(builderUpdateAntennaPattern);
                                                }
                                                else
                                                {
                                                    var builderInsertAntennaPattern = this._dataLayer.GetBuilder<MD.IAntennaPattern>().Insert();
                                                    builderInsertAntennaPattern.SetValue(c => c.DiagA, patt.DiagA);
                                                    builderInsertAntennaPattern.SetValue(c => c.DiagH, patt.DiagH);
                                                    builderInsertAntennaPattern.SetValue(c => c.DiagV, patt.DiagV);
                                                    builderInsertAntennaPattern.SetValue(c => c.Freq, patt.Freq_MHz);
                                                    builderInsertAntennaPattern.SetValue(c => c.Gain, patt.Gain);
                                                    builderInsertAntennaPattern.SetValue(c => c.SENSOR_ANTENNA.Id, idSensorAntenna);
                                                    


                                                    var antennaPattern_PK = scope.Executor.Execute<MD.IAntennaPattern_PK>(builderInsertAntennaPattern);
                                                    idSensorAntennaPattern = antennaPattern_PK.Id;
                                                }
                                            }
                                        }
                                    }
                                }
                            }

                            if (sensorData.Equipment != null)
                            {
                                long idSensorEquipment = -1;
                                var querySensorEquipment = this._dataLayer.GetBuilder<MD.ISensorEquipment>()
                                           .From()
                                           .Select(c => c.Id)
                                           .Where(c => c.SENSOR.Id, ConditionOperator.Equal, idSensor)
                                           .OrderByAsc(c => c.Id)
                                           ;

                                scope.Executor
                                 .Fetch(querySensorEquipment, reader =>
                             {
                                 var result = reader.Read();
                                 if (result)
                                 {
                                     idSensorEquipment = reader.GetValue(c => c.Id);
                                 }
                                 return result;
                             });

                                if (idSensorEquipment == -1)
                                {
                                    var builderInsertEquipment = this._dataLayer.GetBuilder<MD.ISensorEquipment>().Insert();
                                    builderInsertEquipment.SetValue(c => c.Category, sensorData.Equipment.Category);
                                    builderInsertEquipment.SetValue(c => c.Code, sensorData.Equipment.Code);
                                    builderInsertEquipment.SetValue(c => c.CustData1, sensorData.Equipment.CustDate1);
                                    builderInsertEquipment.SetValue(c => c.CustNbr1, sensorData.Equipment.CustNbr1);
                                    builderInsertEquipment.SetValue(c => c.CustTxt1, sensorData.Equipment.CustTxt1);
                                    builderInsertEquipment.SetValue(c => c.EquipClass, sensorData.Equipment.Class);
                                    builderInsertEquipment.SetValue(c => c.Family, sensorData.Equipment.Family);
                                    builderInsertEquipment.SetValue(c => c.FftPointMax, sensorData.Equipment.FFTPointMax);
                                    builderInsertEquipment.SetValue(c => c.LowerFreq, sensorData.Equipment.LowerFreq_MHz);
                                    builderInsertEquipment.SetValue(c => c.Manufacturer, sensorData.Equipment.Manufacturer);
                                    builderInsertEquipment.SetValue(c => c.Mobility, sensorData.Equipment.Mobility);
                                    builderInsertEquipment.SetValue(c => c.Name, sensorData.Equipment.Name);
                                    builderInsertEquipment.SetValue(c => c.OperationMode, sensorData.Equipment.OperationMode);
                                    builderInsertEquipment.SetValue(c => c.RbwMax, sensorData.Equipment.RBWMax_kHz);
                                    builderInsertEquipment.SetValue(c => c.RbwMin, sensorData.Equipment.RBWMin_kHz);
                                    builderInsertEquipment.SetValue(c => c.RefLevelDbm, sensorData.Equipment.MaxRefLevel_dBm);
                                    builderInsertEquipment.SetValue(c => c.Remark, sensorData.Equipment.Remark);
                                    builderInsertEquipment.SetValue(c => c.TechId, sensorData.Equipment.TechId);
                                    builderInsertEquipment.SetValue(c => c.TuningStep, sensorData.Equipment.TuningStep_Hz);
                                    builderInsertEquipment.SetValue(c => c.Type, sensorData.Equipment.Type);
                                    builderInsertEquipment.SetValue(c => c.UpperFreq, sensorData.Equipment.UpperFreq_MHz);
                                    builderInsertEquipment.SetValue(c => c.UserType, sensorData.Equipment.UseType);
                                    builderInsertEquipment.SetValue(c => c.VbwMax, sensorData.Equipment.VBWMax_kHz);
                                    builderInsertEquipment.SetValue(c => c.VbwMin, sensorData.Equipment.VBWMin_kHz);
                                    builderInsertEquipment.SetValue(c => c.Version, sensorData.Equipment.Version);
                                    builderInsertEquipment.SetValue(c => c.SENSOR.Id, idSensor);
                                    

                                    var sensorEquipment_PK = scope.Executor.Execute<MD.ISensorEquipment_PK>(builderInsertEquipment);
                                    idSensorEquipment = sensorEquipment_PK.Id;
                                }

                                if (idSensorEquipment > 0)
                                {

                                    var builderUpdateEquipment = this._dataLayer.GetBuilder<MD.ISensorEquipment>().Update();
                                    builderUpdateEquipment.SetValue(c => c.Category, sensorData.Equipment.Category);
                                    builderUpdateEquipment.SetValue(c => c.Code, sensorData.Equipment.Code);
                                    builderUpdateEquipment.SetValue(c => c.CustData1, sensorData.Equipment.CustDate1);
                                    builderUpdateEquipment.SetValue(c => c.CustNbr1, sensorData.Equipment.CustNbr1);
                                    builderUpdateEquipment.SetValue(c => c.CustTxt1, sensorData.Equipment.CustTxt1);
                                    builderUpdateEquipment.SetValue(c => c.EquipClass, sensorData.Equipment.Class);
                                    builderUpdateEquipment.SetValue(c => c.Family, sensorData.Equipment.Family);
                                    builderUpdateEquipment.SetValue(c => c.FftPointMax, sensorData.Equipment.FFTPointMax);
                                    builderUpdateEquipment.SetValue(c => c.LowerFreq, sensorData.Equipment.LowerFreq_MHz);
                                    builderUpdateEquipment.SetValue(c => c.Manufacturer, sensorData.Equipment.Manufacturer);
                                    builderUpdateEquipment.SetValue(c => c.Mobility, sensorData.Equipment.Mobility);
                                    builderUpdateEquipment.SetValue(c => c.Name, sensorData.Equipment.Name);
                                    builderUpdateEquipment.SetValue(c => c.OperationMode, sensorData.Equipment.OperationMode);
                                    builderUpdateEquipment.SetValue(c => c.RbwMax, sensorData.Equipment.RBWMax_kHz);
                                    builderUpdateEquipment.SetValue(c => c.RbwMin, sensorData.Equipment.RBWMin_kHz);
                                    builderUpdateEquipment.SetValue(c => c.RefLevelDbm, sensorData.Equipment.MaxRefLevel_dBm);
                                    builderUpdateEquipment.SetValue(c => c.Remark, sensorData.Equipment.Remark);
                                    builderUpdateEquipment.SetValue(c => c.TuningStep, sensorData.Equipment.TuningStep_Hz);
                                    builderUpdateEquipment.SetValue(c => c.Type, sensorData.Equipment.Type);
                                    builderUpdateEquipment.SetValue(c => c.UpperFreq, sensorData.Equipment.UpperFreq_MHz);
                                    builderUpdateEquipment.SetValue(c => c.UserType, sensorData.Equipment.UseType);
                                    builderUpdateEquipment.SetValue(c => c.VbwMax, sensorData.Equipment.VBWMax_kHz);
                                    builderUpdateEquipment.SetValue(c => c.VbwMin, sensorData.Equipment.VBWMin_kHz);
                                    builderUpdateEquipment.SetValue(c => c.Version, sensorData.Equipment.Version);
                                    builderUpdateEquipment.Where(c => c.Id, ConditionOperator.Equal, idSensorEquipment);
                                    if (scope.Executor
                                    .Execute(builderUpdateEquipment) > 0)
                                    {
                                        if (sensorData.Equipment.Sensitivities != null)
                                        {
                                            var listIdSensorEquipmentSensitivities = new List<long>();
                                            for (int g = 0; g < sensorData.Equipment.Sensitivities.Length; g++)
                                            {
                                                var senseqps = sensorData.Equipment.Sensitivities[g];

                                                long idSensorEquipmentSensitivities = -1;
                                                var queryDeleteSensitivites = this._dataLayer.GetBuilder<MD.ISensorSensitivites>()
                                               .From()
                                               .Select(c => c.Id)
                                               .Where(c => c.SENSOR_EQUIP.Id, ConditionOperator.Equal, idSensorEquipment)
                                               .Where(c => c.AddLoss, ConditionOperator.Equal, senseqps.AddLoss)
                                               .Where(c => c.Freq, ConditionOperator.Equal, senseqps.Freq_MHz)
                                               .Where(c => c.FreqStability, ConditionOperator.Equal, senseqps.FreqStability)
                                               .Where(c => c.Ktbf, ConditionOperator.Equal, senseqps.KTBF_dBm)
                                               .Where(c => c.Noisef, ConditionOperator.Equal, senseqps.NoiseF)
                                               .OrderByAsc(c => c.Id);

                                                scope.Executor
                                                .Fetch(queryDeleteSensitivites, readerDeleteSensitivites =>
                                                {
                                                    var result = readerDeleteSensitivites.Read();
                                                    if (result)
                                                    {
                                                        idSensorEquipmentSensitivities = readerDeleteSensitivites.GetValue(c => c.Id);
                                                    }
                                                    return result;
                                                });

                                                if (idSensorEquipmentSensitivities > 0)
                                                {
                                                    listIdSensorEquipmentSensitivities.Add(idSensorEquipmentSensitivities);
                                                }

                                                queryDeleteSensitivites = this._dataLayer.GetBuilder<MD.ISensorSensitivites>()
                                                .From()
                                                .Select(c => c.Id)
                                                .Where(c => c.SENSOR_EQUIP.Id, ConditionOperator.NotEqual, idSensorEquipment)
                                                .OrderByAsc(c => c.Id);

                                                scope.Executor
                                                .Fetch(queryDeleteSensitivites, readerDeleteSensitivites =>
                                                {
                                                    var result = readerDeleteSensitivites.Read();
                                                    if (result)
                                                    {
                                                        if (!listIdSensorEquipmentSensitivities.Contains(readerDeleteSensitivites.GetValue(c => c.Id)))
                                                        {
                                                            listIdSensorEquipmentSensitivities.Add(readerDeleteSensitivites.GetValue(c => c.Id));
                                                        }
                                                    }
                                                    return result;
                                                });
                                            }
                                        

                                            if (listIdSensorEquipmentSensitivities.Count > 0)
                                            {
                                                var querySensorDeleteSensitivites = this._dataLayer.GetBuilder<MD.ISensorSensitivites>()
                                                .Delete()
                                                .Where(c => c.Id, ConditionOperator.NotIn, listIdSensorEquipmentSensitivities.ToArray());
                                                scope.Executor.Execute(querySensorDeleteSensitivites);
                                            }


                                            for (int g = 0; g < sensorData.Equipment.Sensitivities.Length; g++)
                                            {
                                                var senseqps = sensorData.Equipment.Sensitivities[g];

                                                long idSensorEquipmentSensitivities = -1;
                                                var querySensorSensitivites = this._dataLayer.GetBuilder<MD.ISensorSensitivites>()
                                               .From()
                                               .Select(c => c.Id)
                                               .Where(c => c.SENSOR_EQUIP.Id, ConditionOperator.Equal, idSensorEquipment)
                                               .Where(c => c.AddLoss, ConditionOperator.Equal, senseqps.AddLoss)
                                               .Where(c => c.Freq, ConditionOperator.Equal, senseqps.Freq_MHz)
                                               .Where(c => c.FreqStability, ConditionOperator.Equal, senseqps.FreqStability)
                                               .Where(c => c.Ktbf, ConditionOperator.Equal, senseqps.KTBF_dBm)
                                               .Where(c => c.Noisef, ConditionOperator.Equal, senseqps.NoiseF)
                                               .OrderByAsc(c => c.Id);

                                                scope.Executor
                                                .Fetch(querySensorSensitivites, reader =>
                                            {
                                                var result = reader.Read();
                                                if (result)
                                                {
                                                    idSensorEquipmentSensitivities = reader.GetValue(c => c.Id);
                                                }
                                                return result;
                                            });

                                                if (idSensorEquipmentSensitivities > 0)
                                                {
                                                    var builderUpdateSensorEquipmentSensitivities = this._dataLayer.GetBuilder<MD.ISensorSensitivites>().Update();
                                                    builderUpdateSensorEquipmentSensitivities.SetValue(c => c.AddLoss, senseqps.AddLoss);
                                                    builderUpdateSensorEquipmentSensitivities.SetValue(c => c.Freq, senseqps.Freq_MHz);
                                                    builderUpdateSensorEquipmentSensitivities.SetValue(c => c.FreqStability, senseqps.FreqStability);
                                                    builderUpdateSensorEquipmentSensitivities.SetValue(c => c.Ktbf, senseqps.KTBF_dBm);
                                                    builderUpdateSensorEquipmentSensitivities.SetValue(c => c.Noisef, senseqps.NoiseF);
                                                    builderUpdateSensorEquipmentSensitivities.Where(c => c.Id, ConditionOperator.Equal, idSensorEquipmentSensitivities);
                                                    scope.Executor
                                                   .Execute(builderUpdateSensorEquipmentSensitivities);
                                                }
                                                else
                                                {
                                                    var builderInsertSensorEquipmentSensitivities = this._dataLayer.GetBuilder<MD.ISensorSensitivites>().Insert();
                                                    builderInsertSensorEquipmentSensitivities.SetValue(c => c.AddLoss, senseqps.AddLoss);
                                                    builderInsertSensorEquipmentSensitivities.SetValue(c => c.Freq, senseqps.Freq_MHz);
                                                    builderInsertSensorEquipmentSensitivities.SetValue(c => c.FreqStability, senseqps.FreqStability);
                                                    builderInsertSensorEquipmentSensitivities.SetValue(c => c.Ktbf, senseqps.KTBF_dBm);
                                                    builderInsertSensorEquipmentSensitivities.SetValue(c => c.Noisef, senseqps.NoiseF);
                                                    builderInsertSensorEquipmentSensitivities.SetValue(c => c.SENSOR_EQUIP.Id, idSensorEquipment);
                                                    

                                                    
                                                    var sensorSensitivites_PK =  scope.Executor.Execute<MD.ISensorSensitivites_PK>(builderInsertSensorEquipmentSensitivities);
                                                    idSensorEquipmentSensitivities = sensorSensitivites_PK.Id;
                                                }
                                            }
                                        }
                                    }
                                    if (sensorData.Polygon != null)
                                    {
                                        var sensPolygon = sensorData.Polygon;
                                        for (int f = 0; f < sensPolygon.Points.Length; f++)
                                        {
                                            Atdi.DataModels.Sdrns.GeoPoint geo = sensPolygon.Points[f];

                                            long idSensorPolygon = -1;
                                            var querySensorPolygon = this._dataLayer.GetBuilder<MD.ISensorPolygon>()
                                            .From()
                                            .Select(c => c.Id)
                                            .Where(c => c.SENSOR.Id, ConditionOperator.Equal, idSensor)
                                            .Where(c => c.Lat, ConditionOperator.Equal, geo.Lat)
                                            .Where(c => c.Lon, ConditionOperator.Equal, geo.Lon)
                                            .OrderByAsc(c => c.Id);

                                            scope.Executor
                                            .Fetch(querySensorPolygon, reader =>
                                        {
                                            var result = reader.Read();
                                            if (result)
                                            {
                                                idSensorPolygon = reader.GetValue(c => c.Id);
                                            }
                                            return result;
                                        });

                                            if (idSensorPolygon == -1)
                                            {
                                                var builderInsertSensorPolygons = this._dataLayer.GetBuilder<MD.ISensorPolygon>().Insert();
                                                builderInsertSensorPolygons.SetValue(c => c.Lat, geo.Lat);
                                                builderInsertSensorPolygons.SetValue(c => c.Lon, geo.Lon);
                                                builderInsertSensorPolygons.SetValue(c => c.SENSOR.Id, idSensor);
                                                

                                                scope.Executor.Execute(builderInsertSensorPolygons);
                                            }
                                            else
                                            {

                                                var builderUpdateSensorPolygons = this._dataLayer.GetBuilder<MD.ISensorPolygon>().Update();
                                                builderUpdateSensorPolygons.SetValue(c => c.Lat, geo.Lat);
                                                builderUpdateSensorPolygons.SetValue(c => c.Lon, geo.Lon);
                                                builderUpdateSensorPolygons.Where(c => c.Id, ConditionOperator.Equal, idSensorPolygon);
                                                scope.Executor
                                                .Execute(builderUpdateSensorPolygons);
                                            }
                                        }
                                    }

                                    for (int f = 0; f < sensorData.Locations.Length; f++)
                                    {
                                        var location = sensorData.Locations[f];
                                        var queryCheck = this._dataLayer.GetBuilder<MD.ISensorLocation>()
                                       .From()
                                       .Select(c => c.Id)
                                       .Where(c => c.Lon, ConditionOperator.Equal, location.Lon)
                                       .Where(c => c.Lat, ConditionOperator.Equal, location.Lat)
                                       .Where(c => c.Asl, ConditionOperator.Equal, location.ASL);
                                        var cnt = scope.Executor.Execute(queryCheck);
                                        if (cnt == 0)
                                        {
                                            var builderUpdateSensLocations = this._dataLayer.GetBuilder<MD.ISensorLocation>().Update();
                                            builderUpdateSensLocations.Where(c => c.SENSOR.Id, ConditionOperator.Equal, idSensor);
                                            builderUpdateSensLocations.Where(c => c.Status, ConditionOperator.NotEqual, "Z");
                                            builderUpdateSensLocations.SetValue(c => c.Status, "Z");
                                            scope.Executor
                                             .Execute(builderUpdateSensLocations);

                                            var builderInsertSensLocations = this._dataLayer.GetBuilder<MD.ISensorLocation>().Insert();
                                            builderInsertSensLocations.SetValue(c => c.Lat, location.Lat);
                                            builderInsertSensLocations.SetValue(c => c.Lon, location.Lon);
                                            builderInsertSensLocations.SetValue(c => c.Asl, location.ASL);
                                            builderInsertSensLocations.SetValue(c => c.DateFrom, location.From);
                                            builderInsertSensLocations.SetValue(c => c.DateTo, location.To);
                                            builderInsertSensLocations.SetValue(c => c.Status, "A");
                                            builderInsertSensLocations.SetValue(c => c.SENSOR.Id, idSensor);

                                            scope.Executor
                                            .Execute(builderInsertSensLocations);

                                        }
                                    }
                                }
                            }
                        }
                    }
                    scope.Commit();
                    resultValue = true;
                }
            }
            catch (Exception ex)
            {
                this._logger.Exception(Contexts.ThisComponent, Categories.MessageProcessing, ex, this);
                resultValue = false;
            }
            return resultValue;
        }

        protected override void Handle(string sensorName, string sensorTechId, DM.Sensor deliveryObject, long messageId)
        {
            using (this._logger.StartTrace(Contexts.ThisComponent, Categories.MessageProcessing, this))
            {
                this._messageProcessingHitsCounter?.Increment();
                this._updateSensorHitsCounter?.Increment();

                var status = SdrnMessageHandlingStatus.Unprocessed;
                var sensorUpdate = false;
                var sensorExistsInDb = false;
                try
                {
                    var query = this._dataLayer.GetBuilder<MD.ISensor>()
                    .From()
                    .Select(c => c.Name)
                    .Select(c => c.Id)
                    .Where(c => c.Name, ConditionOperator.Equal, deliveryObject.Name)
                    .Where(c => c.TechId, ConditionOperator.Equal, deliveryObject.Equipment.TechId)
                    .OrderByAsc(c => c.Id)
                    ;

                    sensorExistsInDb = this._dataLayer.Executor<SdrnServerDataContext>()
                       .Execute(query) == 1;
          

                    if (sensorExistsInDb == true)
                    {
                        sensorUpdate = UpdateSensor(deliveryObject);
                    }

                    // с этого момента нужно считать что сообщение удачно обработано
                    status = SdrnMessageHandlingStatus.Confirmed;

                }
                catch (Exception e)
                {
                    this._updateSensorErrorsCounter?.Increment();
                    this._logger.Exception(Contexts.ThisComponent, Categories.MessageProcessing, e, this);
                    status = SdrnMessageHandlingStatus.Error;
                }
                finally
                {
                    // независимо упали мы по ошибке мы обязаны отправить ответ клиенту
                    // формируем объект подтвержденяи о обновлении данных о сенсоре
                    var updateSensor = new Dev.SensorUpdatingResult
                    {
                        EquipmentTechId = deliveryObject.Equipment.TechId,
                        SensorName = deliveryObject.Name,
                        SdrnServer = this._environment.ServerInstance
                    };

                    if (status == SdrnMessageHandlingStatus.Error)
                    {
                        updateSensor.Status = "Error";
                        updateSensor.Message = "Something went wrong on the server";
                    }
                    else if (sensorUpdate)
                    {
                        updateSensor.Status = "Success";
                        updateSensor.Message = string.Format("Confirm success updated sensor Name = {0}, TechId = {1}", deliveryObject.Name, deliveryObject.Equipment.TechId);
                    }
                    else
                    {
                        updateSensor.Status = "Error";
                        updateSensor.Message = "Something went wrong on the server during the updating of a new sensor";
                    }

                    var envelop = _messagePublisher.CreateOutgoingEnvelope<MSG.Server.SendSensorUpdatingResultMessage, Dev.SensorUpdatingResult>();
                    envelop.SensorName = sensorName;
                    envelop.SensorTechId = sensorTechId;
                    envelop.DeliveryObject = updateSensor;
                    _messagePublisher.Send(envelop);



                    var site = this._pipelineSite.TryGetByName<Atdi.DataModels.Sdrns.Server.RegisterSensorSendEvent, Atdi.DataModels.Sdrns.Server.RegisterSensorSendEvent>(Atdi.DataModels.Sdrns.Server.Pipelines.ClientRegisterAggregationServer, out IPipeline<Atdi.DataModels.Sdrns.Server.RegisterSensorSendEvent, Atdi.DataModels.Sdrns.Server.RegisterSensorSendEvent> pipeline);
                    if (site == true)
                    {
                        var sensor = new DataModels.Sdrns.Server.RegisterSensorSendEvent();
                        sensor.SensorName = deliveryObject.Name;
                        sensor.EquipmentTechId = deliveryObject.Equipment.TechId;
                        var resultSendEvent = pipeline.Execute(sensor);
                    }
                }
            }
        }
    }
}
