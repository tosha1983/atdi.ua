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
                builderInsertSensor.SetValue(c => c.Administration, sensorData.Administration);
                builderInsertSensor.SetValue(c => c.BiuseDate, sensorData.BiuseDate);
                builderInsertSensor.SetValue(c => c.CreatedBy, sensorData.CreatedBy);
                builderInsertSensor.SetValue(c => c.CustData1, sensorData.CustDate1);
                builderInsertSensor.SetValue(c => c.CustNbr1, sensorData.CustNbr1);
                builderInsertSensor.SetValue(c => c.CustTxt1, sensorData.CustTxt1);
                builderInsertSensor.SetValue(c => c.DateCreated, sensorData.Created);
                builderInsertSensor.SetValue(c => c.EouseDate, sensorData.EouseDate);
                builderInsertSensor.SetValue(c => c.Name, sensorData.Name);
                builderInsertSensor.SetValue(c => c.NetworkId, sensorData.NetworkId);
                builderInsertSensor.SetValue(c => c.Remark, sensorData.Remark);
                builderInsertSensor.SetValue(c => c.RxLoss, sensorData.RxLoss);
                builderInsertSensor.SetValue(c => c.Status, "A");
                builderInsertSensor.SetValue(c => c.StepMeasTime, sensorData.StepMeasTime);
                builderInsertSensor.SetValue(c => c.TypeSensor, sensorData.Type);
                builderInsertSensor.SetValue(c => c.ApiVersion, "2.0");
                builderInsertSensor.SetValue(c => c.TechId, sensorData.Equipment.TechId);
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

                if (idSensor > 0)
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


                        if (idSensorAntenna > 0)
                        {
                            if (sensorData.Antenna.Patterns != null)
                            {
                                int idSensorAntennaPattern = -1;

                                for (int l= 0; l < sensorData.Antenna.Patterns.Length; l++)
                                {
                                    var patt = sensorData.Antenna.Patterns[l];

                                    var builderInsertAntennaPattern = this._dataLayer.GetBuilder<MD.IAntennaPattern>().Insert();
                                    builderInsertAntennaPattern.SetValue(c => c.DiagA, patt.DiagA);
                                    builderInsertAntennaPattern.SetValue(c => c.DiagH, patt.DiagH);
                                    builderInsertAntennaPattern.SetValue(c => c.DiagV, patt.DiagV);
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
                            for (int l = 0; l < sensorData.Equipment.Sensitivities.Length; l++)
                            {
                                var senseqps = sensorData.Equipment.Sensitivities[l];

                                int idSensorEquipmentSensitivities = -1;
                                var builderInsertSensorEquipmentSensitivities = this._dataLayer.GetBuilder<MD.ISensorSensitivites>().Insert();
                                builderInsertSensorEquipmentSensitivities.SetValue(c => c.AddLoss, senseqps.AddLoss);
                                builderInsertSensorEquipmentSensitivities.SetValue(c => c.Freq, senseqps.Freq_MHz);
                                builderInsertSensorEquipmentSensitivities.SetValue(c => c.FreqStability, senseqps.FreqStability);
                                builderInsertSensorEquipmentSensitivities.SetValue(c => c.Ktbf, senseqps.KTBF_dBm);
                                builderInsertSensorEquipmentSensitivities.SetValue(c => c.Noisef, senseqps.NoiseF);
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

                            for (int l = 0; l < sensPolygon.Points.Length; l++)
                            {
                                var geo = sensPolygon.Points[l];

                                var builderInsertSensorPolygons = this._dataLayer.GetBuilder<MD.ISensorPolygon>().Insert();
                                builderInsertSensorPolygons.SetValue(c => c.Lat, geo.Lat);
                                builderInsertSensorPolygons.SetValue(c => c.Lon, geo.Lon);
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

                            for (int l = 0; l < sensorData.Locations.Length; l++)
                            {
                                var location = sensorData.Locations[l];

                                var builderInsertSensLocations = this._dataLayer.GetBuilder<MD.ISensorLocation>().Insert();
                                builderInsertSensLocations.SetValue(c => c.Lat, location.Lat);
                                builderInsertSensLocations.SetValue(c => c.Lon, location.Lon);
                                builderInsertSensLocations.SetValue(c => c.Asl, location.ASL);
                                builderInsertSensLocations.SetValue(c => c.DateFrom, location.From);
                                builderInsertSensLocations.SetValue(c => c.DateTo, location.To);
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
                    //if (sensorRegistration)
                    //{
                    //    this._eventEmitter.Emit("OnNewSensorRegistartion", "RegisterSensorProccesing");
                    //}
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
                        EquipmentTechId = incomingEnvelope.DeliveryObject.Equipment.TechId,
                        SensorName = incomingEnvelope.DeliveryObject.Name,
                        SdrnServer = this._environment.ServerInstance,
                    };

                    if (result.Status == SdrnMessageHandlingStatus.Error)
                    {
                        registrationResult.Status = "Error";
                        registrationResult.Message = "Something went wrong on the server";
                    }
                    else if (sensorExistsInDb)
                    {
                        registrationResult.Status = "Reject";
                        registrationResult.Message = string.Format("The sensor has already been registered earlier Name = {0}, TechId = {1}", incomingEnvelope.DeliveryObject.Name, incomingEnvelope.DeliveryObject.Equipment.TechId);
                    }
                    else if (sensorRegistration)
                    {
                        registrationResult.Status = "Success";
                        registrationResult.Message = string.Format("Confirm success registration sensor Name = {0}, TechId = {1}", incomingEnvelope.DeliveryObject.Name, incomingEnvelope.DeliveryObject.Equipment.TechId);
                    }
                    else
                    {
                        registrationResult.Status = "Error";
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
