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
using System.Collections.Generic;

namespace Atdi.AppUnits.Sdrn.Server.PrimaryHandlers.Handlers
{
    public class RegisterSensorHandler : ISdrnMessageHandler<MSG.Device.RegisterSensorMessage, Sensor>
    {
        private readonly ISdrnMessagePublisher _messagePublisher;
        private readonly IDataLayer<EntityDataOrm> _dataLayer;
        private readonly ISdrnServerEnvironment _environment;
        private readonly IEventEmitter _eventEmitter;
        private readonly ILogger _logger;

        public RegisterSensorHandler(ISdrnMessagePublisher messagePublisher, IDataLayer<EntityDataOrm> dataLayer, ISdrnServerEnvironment environment, IEventEmitter eventEmitter, ILogger logger)
        {
            this._messagePublisher = messagePublisher;
            this._dataLayer = dataLayer;
            this._environment = environment;
            this._eventEmitter = eventEmitter;
            this._logger = logger;
        }

        public bool CreateSensor(ISdrnIncomingEnvelope<Sensor> incomingEnvelope)
        {
            var resultValue = false;
            var queryExecuter = this._dataLayer.Executor<SdrnServerDataContext>();
            try
            {
                var sensorData = incomingEnvelope.DeliveryObject;
                queryExecuter.BeginTransaction();

                int idSensor = -1;
                var builderInsertSensor = this._dataLayer.GetBuilder<MD.ISensor>().Insert();
                if (sensorData.Administration != null) builderInsertSensor.SetValue(c => c.Administration, sensorData.Administration);
                if (sensorData.BiuseDate != null) builderInsertSensor.SetValue(c => c.BiuseDate, sensorData.BiuseDate);
                if (sensorData.CreatedBy != null) builderInsertSensor.SetValue(c => c.CreatedBy, sensorData.CreatedBy);
                if (sensorData.CustDate1 != null) builderInsertSensor.SetValue(c => c.CustData1, sensorData.CustDate1);
                if (sensorData.CustNbr1 != null) builderInsertSensor.SetValue(c => c.CustNbr1, sensorData.CustNbr1);
                if (sensorData.CustTxt1 != null) builderInsertSensor.SetValue(c => c.CustTxt1, sensorData.CustTxt1);
                if (sensorData.Created != null) builderInsertSensor.SetValue(c => c.DateCreated, sensorData.Created);
                if (sensorData.EouseDate != null) builderInsertSensor.SetValue(c => c.EouseDate, sensorData.EouseDate);
                if (sensorData.Name != null) builderInsertSensor.SetValue(c => c.Name, sensorData.Name);
                if (sensorData.NetworkId != null) builderInsertSensor.SetValue(c => c.NetworkId, sensorData.NetworkId);
                if (sensorData.Remark != null) builderInsertSensor.SetValue(c => c.Remark, sensorData.Remark);
                if (sensorData.RxLoss != null) builderInsertSensor.SetValue(c => c.RxLoss, sensorData.RxLoss);
                if (sensorData.Status != null) builderInsertSensor.SetValue(c => c.Status, sensorData.Status);
                if (sensorData.StepMeasTime != null) builderInsertSensor.SetValue(c => c.StepMeasTime, sensorData.StepMeasTime);
                if (sensorData.Type != null) builderInsertSensor.SetValue(c => c.TypeSensor, sensorData.Type);
                if (sensorData.BiuseDate != null) builderInsertSensor.SetValue(c => c.ApiVersion, "2.0");
                if (sensorData.Equipment.TechId != null) builderInsertSensor.SetValue(c => c.TechId, sensorData.Equipment.TechId);
                builderInsertSensor.Select(c => c.Id);
                builderInsertSensor.Select(c => c.Name);

                queryExecuter.ExecuteAndFetch(builderInsertSensor, reader =>
                 {
                     var result = reader.Read();
                     if (result)
                     {
                         idSensor = reader.GetValue(c => c.Id);
                     }
                     return result;
                 });

                if (idSensor > -1)
                {
                    var builderUpdateSensor = this._dataLayer.GetBuilder<MD.ISensor>().Update();
                    builderUpdateSensor.SetValue(t => t.SensorIdentifierId, idSensor);
                    builderUpdateSensor.Where(t => t.Id, ConditionOperator.Equal, idSensor);
                    queryExecuter
                   .Execute(builderUpdateSensor);


                    if (sensorData.Antenna != null)
                    {
                        int idSensorAntenna = -1;
                        var builderInsertAntenna = this._dataLayer.GetBuilder<MD.ISensorAntenna>().Insert();
                        builderInsertAntenna.SetValue(c => c.AddLoss, sensorData.Antenna.AddLoss);
                        if (sensorData.Antenna.Class != null) builderInsertAntenna.SetValue(c => c.AntClass, sensorData.Antenna.Class);
                        builderInsertAntenna.SetValue(c => c.AntDir, sensorData.Antenna.Direction.ToString());
                        if (sensorData.Antenna.Category != null) builderInsertAntenna.SetValue(c => c.Category, sensorData.Antenna.Category);
                        if (sensorData.Antenna.Code != null) builderInsertAntenna.SetValue(c => c.Code, sensorData.Antenna.Code);
                        if (sensorData.Antenna.CustDate1 != null) builderInsertAntenna.SetValue(c => c.CustData1, sensorData.Antenna.CustDate1);
                        builderInsertAntenna.SetValue(c => c.CustNbr1, sensorData.Antenna.CustNbr1);
                        if (sensorData.Antenna.CustTxt1 != null) builderInsertAntenna.SetValue(c => c.CustTxt1, sensorData.Antenna.CustTxt1);
                        builderInsertAntenna.SetValue(c => c.GainMax, sensorData.Antenna.GainMax);
                        if (sensorData.Antenna.GainType != null) builderInsertAntenna.SetValue(c => c.GainType, sensorData.Antenna.GainType);
                        if (sensorData.Antenna.HBeamwidth != null) builderInsertAntenna.SetValue(c => c.HbeamWidth, sensorData.Antenna.HBeamwidth);
                        if (sensorData.Antenna.LowerFreq_MHz != null) builderInsertAntenna.SetValue(c => c.LowerFreq, sensorData.Antenna.LowerFreq_MHz);
                        if (sensorData.Antenna.Manufacturer != null) builderInsertAntenna.SetValue(c => c.Manufacturer, sensorData.Antenna.Manufacturer);
                        if (sensorData.Antenna.Name != null) builderInsertAntenna.SetValue(c => c.Name, sensorData.Antenna.Name);
                        builderInsertAntenna.SetValue(c => c.Polarization, sensorData.Antenna.Polarization.ToString());
                        if (sensorData.Antenna.Remark != null) builderInsertAntenna.SetValue(c => c.Remark, sensorData.Antenna.Remark);
                        if (sensorData.Antenna.SlewAng != null) builderInsertAntenna.SetValue(c => c.Slewang, sensorData.Antenna.SlewAng);
                        if (sensorData.Antenna.TechId != null) builderInsertAntenna.SetValue(c => c.TechId, sensorData.Antenna.TechId);
                        if (sensorData.Antenna.UpperFreq_MHz != null) builderInsertAntenna.SetValue(c => c.UpperFreq, sensorData.Antenna.UpperFreq_MHz);
                        if (sensorData.Antenna.UseType != null) builderInsertAntenna.SetValue(c => c.UseType, sensorData.Antenna.UseType);
                        if (sensorData.Antenna.VBeamwidth != null) builderInsertAntenna.SetValue(c => c.VbeamWidth, sensorData.Antenna.VBeamwidth);
                        if (sensorData.Antenna.XPD != null) builderInsertAntenna.SetValue(c => c.Xpd, sensorData.Antenna.XPD);
                        builderInsertAntenna.SetValue(c => c.SensorId, idSensor);
                        builderInsertAntenna.Select(c => c.Id);
                        queryExecuter
                        .ExecuteAndFetch(builderInsertAntenna, reader =>
                        {
                            var result = reader.Read();
                            if (result)
                            {
                                idSensorAntenna = reader.GetValue(c => c.Id);
                            }
                            return result;
                        });


                        if (idSensorAntenna > -1)
                        {
                            if (sensorData.Antenna.Patterns != null)
                            {
                                int idSensorAntennaPattern = -1;
                                foreach (AntennaPattern patt in sensorData.Antenna.Patterns)
                                {
                                    var builderInsertAntennaPattern = this._dataLayer.GetBuilder<MD.IAntennaPattern>().Insert();
                                    if (patt.DiagA != null) builderInsertAntennaPattern.SetValue(c => c.DiagA, patt.DiagA);
                                    if (patt.DiagH != null) builderInsertAntennaPattern.SetValue(c => c.DiagH, patt.DiagH);
                                    if (patt.DiagV != null) builderInsertAntennaPattern.SetValue(c => c.DiagV, patt.DiagV);
                                    builderInsertAntennaPattern.SetValue(c => c.Freq, patt.Freq_MHz);
                                    builderInsertAntennaPattern.SetValue(c => c.Gain, patt.Gain);
                                    builderInsertAntennaPattern.SetValue(c => c.SensorAntennaId, idSensorAntenna);
                                    builderInsertAntennaPattern.Select(c => c.Id);
                                    queryExecuter
                                    .ExecuteAndFetch(builderInsertAntennaPattern, reader =>
                                    {
                                        var result = reader.Read();
                                        if (result)
                                        {
                                            idSensorAntennaPattern = reader.GetValue(c => c.Id);
                                        }
                                        return result;
                                    });
                                }
                            }
                        }
                    }

                    if (sensorData.Equipment != null)
                    {
                        int idSensorEquipment = -1;
                        var builderInsertEquipment = this._dataLayer.GetBuilder<MD.ISensorEquipment>().Insert();
                        if (sensorData.Equipment.Category != null) builderInsertEquipment.SetValue(c => c.Category, sensorData.Equipment.Category);
                        if (sensorData.Equipment.Code != null) builderInsertEquipment.SetValue(c => c.Code, sensorData.Equipment.Code);
                        if (sensorData.Equipment.CustDate1 != null) builderInsertEquipment.SetValue(c => c.CustData1, sensorData.Equipment.CustDate1);
                        if (sensorData.Equipment.CustNbr1 != null) builderInsertEquipment.SetValue(c => c.CustNbr1, sensorData.Equipment.CustNbr1);
                        if (sensorData.Equipment.CustTxt1 != null) builderInsertEquipment.SetValue(c => c.CustTxt1, sensorData.Equipment.CustTxt1);
                        if (sensorData.Equipment.Class != null) builderInsertEquipment.SetValue(c => c.EquipClass, sensorData.Equipment.Class);
                        if (sensorData.Equipment.Family != null) builderInsertEquipment.SetValue(c => c.Family, sensorData.Equipment.Family);
                        if (sensorData.Equipment.FFTPointMax != null) builderInsertEquipment.SetValue(c => c.FftPointMax, sensorData.Equipment.FFTPointMax);
                        if (sensorData.Equipment.LowerFreq_MHz != null) builderInsertEquipment.SetValue(c => c.LowerFreq, sensorData.Equipment.LowerFreq_MHz);
                        if (sensorData.Equipment.Manufacturer != null) builderInsertEquipment.SetValue(c => c.Manufacturer, sensorData.Equipment.Manufacturer);
                        if (sensorData.Equipment.Mobility != null) builderInsertEquipment.SetValue(c => c.Mobility, sensorData.Equipment.Mobility);
                        if (sensorData.Equipment.Name != null) builderInsertEquipment.SetValue(c => c.Name, sensorData.Equipment.Name);
                        if (sensorData.Equipment.OperationMode != null) builderInsertEquipment.SetValue(c => c.OperationMode, sensorData.Equipment.OperationMode);
                        if (sensorData.Equipment.RBWMax_kHz != null) builderInsertEquipment.SetValue(c => c.RbwMax, sensorData.Equipment.RBWMax_kHz);
                        if (sensorData.Equipment.RBWMin_kHz != null) builderInsertEquipment.SetValue(c => c.RbwMin, sensorData.Equipment.RBWMin_kHz);
                        if (sensorData.Equipment.MaxRefLevel_dBm != null) builderInsertEquipment.SetValue(c => c.RefLevelDbm, sensorData.Equipment.MaxRefLevel_dBm);
                        if (sensorData.Equipment.Remark != null) builderInsertEquipment.SetValue(c => c.Remark, sensorData.Equipment.Remark);
                        if (sensorData.Equipment.TechId != null) builderInsertEquipment.SetValue(c => c.TechId, sensorData.Equipment.TechId);
                        if (sensorData.Equipment.TuningStep_Hz != null) builderInsertEquipment.SetValue(c => c.TuningStep, sensorData.Equipment.TuningStep_Hz);
                        if (sensorData.Equipment.Type != null) builderInsertEquipment.SetValue(c => c.Type, sensorData.Equipment.Type);
                        if (sensorData.Equipment.UpperFreq_MHz != null) builderInsertEquipment.SetValue(c => c.UpperFreq, sensorData.Equipment.UpperFreq_MHz);
                        if (sensorData.Equipment.UseType != null) builderInsertEquipment.SetValue(c => c.UserType, sensorData.Equipment.UseType);
                        if (sensorData.Equipment.VBWMax_kHz != null) builderInsertEquipment.SetValue(c => c.VbwMax, sensorData.Equipment.VBWMax_kHz);
                        if (sensorData.Equipment.VBWMin_kHz != null) builderInsertEquipment.SetValue(c => c.VbwMin, sensorData.Equipment.VBWMin_kHz);
                        if (sensorData.Equipment.Version != null) builderInsertEquipment.SetValue(c => c.Version, sensorData.Equipment.Version);
                        builderInsertEquipment.SetValue(c => c.SensorId, idSensor);
                        builderInsertEquipment.Select(c => c.Id);
                        queryExecuter
                               .ExecuteAndFetch(builderInsertEquipment, reader =>
                               {
                                   var result = reader.Read();
                                   if (result)
                                   {
                                       idSensorEquipment = reader.GetValue(c => c.Id);
                                   }
                                   return result;
                               });

                        if (sensorData.Equipment.Sensitivities != null)
                        {
                            foreach (EquipmentSensitivity senseqps in sensorData.Equipment.Sensitivities)
                            {
                                int idSensorEquipmentSensitivities = -1;
                                var builderInsertSensorEquipmentSensitivities = this._dataLayer.GetBuilder<MD.ISensorSensitivites>().Insert();
                                if (senseqps.AddLoss != null) builderInsertSensorEquipmentSensitivities.SetValue(c => c.AddLoss, senseqps.AddLoss);
                                builderInsertSensorEquipmentSensitivities.SetValue(c => c.Freq, senseqps.Freq_MHz);
                                if (senseqps.FreqStability != null) builderInsertSensorEquipmentSensitivities.SetValue(c => c.FreqStability, senseqps.FreqStability);
                                if (senseqps.KTBF_dBm != null) builderInsertSensorEquipmentSensitivities.SetValue(c => c.Ktbf, senseqps.KTBF_dBm);
                                if (senseqps.NoiseF != null) builderInsertSensorEquipmentSensitivities.SetValue(c => c.Noisef, senseqps.NoiseF);
                                builderInsertSensorEquipmentSensitivities.SetValue(c => c.SensorEquipId, idSensorEquipment);
                                builderInsertSensorEquipmentSensitivities.Select(c => c.Id);
                                queryExecuter
                                .ExecuteAndFetch(builderInsertSensorEquipmentSensitivities, reader =>
                                   {
                                       var result = reader.Read();
                                       if (result)
                                       {
                                           idSensorEquipmentSensitivities = reader.GetValue(c => c.Id);
                                       }
                                       return result;
                                   });
                            }
                        }
                        if (sensorData.Polygon != null)
                        {
                            SensorPolygon sensPolygon = sensorData.Polygon;
                            int idsensPolygon = -1;
                            foreach (Atdi.DataModels.Sdrns.GeoPoint geo in sensPolygon.Points)
                            {
                                var builderInsertSensorPolygons = this._dataLayer.GetBuilder<MD.ISensorPolygon>().Insert();
                                if (geo.Lat != null) builderInsertSensorPolygons.SetValue(c => c.Lat, geo.Lat);
                                if (geo.Lon != null) builderInsertSensorPolygons.SetValue(c => c.Lon, geo.Lon);
                                builderInsertSensorPolygons.SetValue(c => c.SensorId, idSensor);
                                builderInsertSensorPolygons.Select(c => c.Id);
                                queryExecuter
                                .ExecuteAndFetch(builderInsertSensorPolygons, reader =>
                                {
                                    var result = reader.Read();
                                    if (result)
                                    {
                                        idsensPolygon = reader.GetValue(c => c.Id);
                                    }
                                    return result;
                                });
                            }
                        }

                        if (sensorData.Locations != null)
                        {
                            int idsensLocations = -1;
                            foreach (var location in sensorData.Locations)
                            {
                                var builderInsertSensLocations = this._dataLayer.GetBuilder<MD.ISensorLocation>().Insert();
                                builderInsertSensLocations.SetValue(c => c.Lat, location.Lat);
                                builderInsertSensLocations.SetValue(c => c.Lon, location.Lon);
                                if (location.ASL!=null) builderInsertSensLocations.SetValue(c => c.Asl, location.ASL);
                                if (location.From != null) builderInsertSensLocations.SetValue(c => c.DateFrom, location.From);
                                if (location.To != null) builderInsertSensLocations.SetValue(c => c.DateTo, location.To);
                                builderInsertSensLocations.SetValue(c => c.Status, location.Status);
                                builderInsertSensLocations.SetValue(c => c.SensorId, idSensor);
                                builderInsertSensLocations.Select(c => c.Id);
                                queryExecuter
                                .ExecuteAndFetch(builderInsertSensLocations, reader =>
                                {
                                    var result = reader.Read();
                                    if (result)
                                    {
                                        idsensLocations = reader.GetValue(c => c.Id);
                                    }
                                    return result;
                                });
                            }
                        }
                    }
                }
                queryExecuter.CommitTransaction();
                resultValue = true;
            }
            catch (Exception ex)
            {
                queryExecuter.RollbackTransaction();
                this._logger.Exception(Contexts.PrimaryHandler, Categories.MessageProcessing, ex, this);
                resultValue = false;
            }
            return resultValue;
        }
       

        public void Handle(ISdrnIncomingEnvelope<Sensor> incomingEnvelope, ISdrnMessageHandlingResult result)
        {
            using (this._logger.StartTrace(Contexts.PrimaryHandler, Categories.MessageProcessing, this))
            {
                this._eventEmitter.Emit("OnEvent1", "RegisterSensorProcess");
                result.Status = SdrnMessageHandlingStatus.Rejected;
                var sensorRegistration = false;
                var sensorExistsInDb = false;
                try
                {
                    var query = this._dataLayer.GetBuilder<MD.ISensor>()
                    .From()
                    .Select(c => c.Name)
                    .Select(c => c.Id)
                    .Where(c => c.Name, ConditionOperator.Equal, incomingEnvelope.DeliveryObject.Name)
                    .Where(c => c.TechId, ConditionOperator.Equal, incomingEnvelope.DeliveryObject.Equipment.TechId)
                    .OrderByAsc(c => c.Id)
                    ;
                    
                    sensorExistsInDb = this._dataLayer.Executor<SdrnServerDataContext>()
                    .Execute(query) == 1;

                    if (sensorExistsInDb == false)
                    {
                        sensorRegistration = CreateSensor(incomingEnvelope);
                    }

                    // с этого момента нужно считать что сообщение удачно обработано
                    result.Status = SdrnMessageHandlingStatus.Confirmed;

                    // отправка события если новый сенсор создан в БД
                    if (sensorRegistration)
                    {
                        this._eventEmitter.Emit("OnNewSensorRegistartion", "RegisterSensorProccesing");
                    }
                }
                catch (Exception e)
                {
                    this._logger.Exception(Contexts.PrimaryHandler, Categories.MessageProcessing, e, this);
                    if (result.Status == SdrnMessageHandlingStatus.Unprocessed)
                    {
                        result.Status = SdrnMessageHandlingStatus.Error;
                        result.ReasonFailure = e.ToString();
                    }
                }
                finally
                {
                    // независимо упали мы по ошибке мы обязаны отправить ответ клиенту
                    // формируем объект подтверждения регистрации
                    var registrationResult = new SensorRegistrationResult
                    {
                        EquipmentTechId = incomingEnvelope.SensorTechId,
                        SensorName = incomingEnvelope.SensorName,
                        SdrnServer = this._environment.ServerInstance,
                    };

                    if (result.Status == SdrnMessageHandlingStatus.Error)
                    {
                        registrationResult.Status = "ERROR";
                        registrationResult.Message = "Something went wrong on the server";
                    }
                    else if (sensorExistsInDb)
                    {
                        registrationResult.Status = "REJECT";
                        registrationResult.Message = string.Format("The sensor has already been registered earlier Name = {0}, TechId = {1}", incomingEnvelope.DeliveryObject.Name, incomingEnvelope.DeliveryObject.Equipment.TechId);
                    }
                    else if (sensorRegistration)
                    {
                        registrationResult.Status = "Success";
                        registrationResult.Message = string.Format("Confirm success registration sensor Name = {0}, TechId = {1}", incomingEnvelope.DeliveryObject.Name, incomingEnvelope.DeliveryObject.Equipment.TechId);
                    }
                    else
                    {
                        registrationResult.Status = "ERROR";
                        registrationResult.Message = "Something went wrong on the server during the registration of a new sensor";
                    }

                    var envelop = _messagePublisher.CreateOutgoingEnvelope<MSG.Server.SendRegistrationResultMessage, SensorRegistrationResult>();
                    envelop.SensorName = incomingEnvelope.SensorName;
                    envelop.SensorTechId = incomingEnvelope.SensorTechId;
                    envelop.DeliveryObject = registrationResult;
                    _messagePublisher.Send(envelop);
                }
            }
        }
    }
}
