
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

namespace Atdi.AppUnits.Sdrn.Server.PrimaryHandlers.Handlers
{
    public class UpdateSensorHandler : ISdrnMessageHandler<MSG.Device.UpdateSensorMessage, Sensor>
    {
        private readonly ISdrnMessagePublisher _messagePublisher;
        private readonly IDataLayer<EntityDataOrm> _dataLayer;
        private readonly ISdrnServerEnvironment _environment;
        private readonly IEventEmitter _eventEmitter;
        private readonly ILogger _logger;

        public UpdateSensorHandler(
            ISdrnMessagePublisher messagePublisher, 
            IDataLayer<EntityDataOrm> dataLayer, 
            ISdrnServerEnvironment environment, 
            IEventEmitter eventEmitter, ILogger logger)
        {
            this._messagePublisher = messagePublisher;
            this._dataLayer = dataLayer;
            this._environment = environment;
            this._eventEmitter = eventEmitter;
            this._logger = logger;
        }

     

        public bool UpdateSensor(ISdrnIncomingEnvelope<Sensor> incomingEnvelope)
        {
            var resultValue = false;
            var queryExecuter = this._dataLayer.Executor<SdrnServerDataContext>();
            try
            {
                queryExecuter.BeginTransaction();
                var sensorData = incomingEnvelope.DeliveryObject;
                var idSensor = -1;
                var query = this._dataLayer.GetBuilder<MD.ISensor>()
                .From()
                .Select(c => c.Name)
                .Select(c => c.Id)
                .Where(c => c.Name, ConditionOperator.Equal, incomingEnvelope.DeliveryObject.Name)
                .Where(c => c.TechId, ConditionOperator.Equal, incomingEnvelope.DeliveryObject.Equipment.TechId)
                .OrderByAsc(c => c.Id);

                queryExecuter
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
                    builderUpdateSensor.Where(c => c.Id, ConditionOperator.Equal, idSensor);
                    if (queryExecuter
                    .Execute(builderUpdateSensor) > 0)
                    {
                        if (sensorData.Antenna != null)
                        {
                            int idSensorAntenna = -1;
                            var querySensorAntenna = this._dataLayer.GetBuilder<MD.ISensorAntenna>()
                            .From()
                            .Select(c => c.Id)
                            .Where(c => c.SensorId, ConditionOperator.Equal, idSensor)
                            .OrderByAsc(c => c.Id)
                            ;

                            queryExecuter
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
                                if (queryExecuter
                                 .Execute(builderUpdateAntenna) > 0)
                                {
                                    if (sensorData.Antenna.Patterns != null)
                                    {
                                        int idSensorAntennaPattern = -1;

                                        for (int b = 0; b < sensorData.Antenna.Patterns.Length; b++)
                                        {
                                            AntennaPattern patt = sensorData.Antenna.Patterns[b];

                                            var querySensorAntennaPatterns = this._dataLayer.GetBuilder<MD.IAntennaPattern>()
                                            .From()
                                            .Select(c => c.Id)
                                            .Where(c => c.SensorAntennaId, ConditionOperator.Equal, idSensorAntenna)
                                            .Where(c => c.Freq, ConditionOperator.Equal, patt.Freq_MHz)
                                            .Where(c => c.Gain, ConditionOperator.Equal, patt.Gain)
                                            .OrderByAsc(c => c.Id)
                                            ;

                                            queryExecuter
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
                                                builderUpdateAntennaPattern.SetValue(c => c.SensorAntennaId, idSensorAntenna);
                                                builderUpdateAntennaPattern.Where(c => c.Id, ConditionOperator.Equal, idSensorAntennaPattern);

                                                queryExecuter
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
                            }
                        }

                        if (sensorData.Equipment != null)
                        {
                            int idSensorEquipment = -1;
                            var querySensorEquipment = this._dataLayer.GetBuilder<MD.ISensorEquipment>()
                                       .From()
                                       .Select(c => c.Id)
                                       .Where(c => c.SensorId, ConditionOperator.Equal, idSensor)
                                       .OrderByAsc(c => c.Id)
                                       ;

                            queryExecuter
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
                                if (queryExecuter
                                .Execute(builderUpdateEquipment) > 0)
                                {
                                    if (sensorData.Equipment.Sensitivities != null)
                                    {
                                        for (int g = 0; g < sensorData.Equipment.Sensitivities.Length; g++)
                                        {
                                            EquipmentSensitivity senseqps = sensorData.Equipment.Sensitivities[g];

                                            int idSensorEquipmentSensitivities = -1;
                                            var querySensorSensitivites = this._dataLayer.GetBuilder<MD.ISensorSensitivites>()
                                           .From()
                                           .Select(c => c.Id)
                                           .Where(c => c.SensorEquipId, ConditionOperator.Equal, idSensorEquipment)
                                           .Where(c => c.AddLoss, ConditionOperator.Equal, senseqps.AddLoss)
                                           .Where(c => c.Freq, ConditionOperator.Equal, senseqps.Freq_MHz)
                                           .Where(c => c.FreqStability, ConditionOperator.Equal, senseqps.FreqStability)
                                           .Where(c => c.Ktbf, ConditionOperator.Equal, senseqps.KTBF_dBm)
                                           .Where(c => c.Noisef, ConditionOperator.Equal, senseqps.NoiseF)
                                           .OrderByAsc(c => c.Id);

                                            queryExecuter
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
                                                builderUpdateSensorEquipmentSensitivities.SetValue(c => c.SensorEquipId, idSensorEquipment);
                                                builderUpdateSensorEquipmentSensitivities.Where(c => c.Id, ConditionOperator.Equal, idSensorEquipmentSensitivities);
                                                queryExecuter
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
                                    }
                                }
                                if (sensorData.Polygon != null)
                                {
                                    SensorPolygon sensPolygon = sensorData.Polygon;
                                    for (int f = 0; f < sensPolygon.Points.Length; f++)
                                    {
                                        Atdi.DataModels.Sdrns.GeoPoint geo = sensPolygon.Points[f];

                                        var idSensorPolygon = -1;
                                        var querySensorPolygon = this._dataLayer.GetBuilder<MD.ISensorPolygon>()
                                        .From()
                                        .Select(c => c.Id)
                                        .Where(c => c.SensorId, ConditionOperator.Equal, idSensor)
                                        .Where(c => c.Lat, ConditionOperator.Equal, geo.Lat)
                                        .Where(c => c.Lon, ConditionOperator.Equal, geo.Lon)
                                        .OrderByAsc(c => c.Id);

                                        queryExecuter
                                        .Fetch(querySensorPolygon, reader =>
                                        {
                                            var result = reader.Read();
                                            if (result)
                                            {
                                                idSensorPolygon = reader.GetValue(c => c.Id);
                                            }
                                            return result;
                                        });

                                        int idsensPolygon = -1;

                                        if (idSensorPolygon == -1)
                                        {
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
                                        else
                                        {

                                            var builderUpdateSensorPolygons = this._dataLayer.GetBuilder<MD.ISensorPolygon>().Update();
                                            builderUpdateSensorPolygons.SetValue(c => c.Lat, geo.Lat);
                                            builderUpdateSensorPolygons.SetValue(c => c.Lon, geo.Lon);
                                            builderUpdateSensorPolygons.SetValue(c => c.SensorId, idSensor);
                                            builderUpdateSensorPolygons.Where(c => c.Id, ConditionOperator.Equal, idSensorPolygon);
                                            queryExecuter
                                            .Execute(builderUpdateSensorPolygons);
                                        }
                                    }
                                }

                                if (sensorData.Locations != null)
                                {

                                    for (int f = 0; f < sensorData.Locations.Length; f++)
                                    {
                                        var location = sensorData.Locations[f];

                                        var idSensorlocation = -1;
                                        var querySensorPolygon = this._dataLayer.GetBuilder<MD.ISensorLocation>()
                                        .From()
                                        .Select(c => c.Id)
                                        .Where(c => c.SensorId, ConditionOperator.Equal, idSensor)
                                        .Where(c => c.Status, ConditionOperator.Equal, "A")
                                        .OrderByAsc(c => c.Id);

                                        queryExecuter
                                        .Fetch(querySensorPolygon, reader =>
                                        {
                                            var result = false;
                                            while (reader.Read())
                                            { 
                                                idSensorlocation = reader.GetValue(c => c.Id);
                                                var builderUpdateSensLocations = this._dataLayer.GetBuilder<MD.ISensorLocation>().Update();
                                                builderUpdateSensLocations.SetValue(c => c.Status, "Z");
                                                builderUpdateSensLocations.Where(c => c.Id, ConditionOperator.Equal, idSensorlocation);
                                                queryExecuter
                                                .Execute(builderUpdateSensLocations);
                                                result = true;
                                            }
                                            return result;
                                        });

                                        int sensorLocation = -1;
                                        var builderInsertSensLocations = this._dataLayer.GetBuilder<MD.ISensorLocation>().Insert();
                                        builderInsertSensLocations.SetValue(c => c.Lat, location.Lat);
                                        builderInsertSensLocations.SetValue(c => c.Lon, location.Lon);
                                        builderInsertSensLocations.SetValue(c => c.Asl, location.ASL);
                                        builderInsertSensLocations.SetValue(c => c.DateFrom, location.From);
                                        builderInsertSensLocations.SetValue(c => c.DateTo, location.To);
                                        builderInsertSensLocations.SetValue(c => c.Status, "A");
                                        builderInsertSensLocations.SetValue(c => c.SensorId, idSensor);
                                        builderInsertSensLocations.Select(c => c.Id);
                                        queryExecuter
                                        .ExecuteAndFetch(builderInsertSensLocations, reader =>
                                        {
                                            var result = reader.Read();
                                            if (result)
                                            {
                                                sensorLocation = reader.GetValue(c => c.Id);
                                            }
                                            return result;
                                        });

                                    }
                                }
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
                result.Status = SdrnMessageHandlingStatus.Unprocessed;
                var sensorUpdate = false;
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

                    if (sensorExistsInDb == true)
                    {
                        sensorUpdate = UpdateSensor(incomingEnvelope);
                    }

                    // с этого момента нужно считать что сообщение удачно обработано
                    result.Status = SdrnMessageHandlingStatus.Confirmed;

                    // отправка события если новый сенсор создан в БД
                    //if (sensorUpdate)
                    //{
                    //    this._eventEmitter.Emit("OnSensorUpdate", "UpdateSensorProccesing");
                    //}
                }
                catch (Exception e)
                {
                    this._logger.Exception(Contexts.PrimaryHandler, Categories.MessageProcessing, e, this);
                    result.Status = SdrnMessageHandlingStatus.Error;
                    result.ReasonFailure = e.ToString();
                }
                finally
                {
                    // независимо упали мы по ошибке мы обязаны отправить ответ клиенту
                    // формируем объект подтвержденяи о обновлении данных о сенсоре
                    var updateSensor = new SensorUpdatingResult
                    {
                        EquipmentTechId = incomingEnvelope.DeliveryObject.Equipment.TechId,
                        SensorName = incomingEnvelope.DeliveryObject.Name,
                        SdrnServer = this._environment.ServerInstance
                    };

                    if (result.Status == SdrnMessageHandlingStatus.Error)
                    {
                        updateSensor.Status = "Error";
                        updateSensor.Message = "Something went wrong on the server";
                    }
                    else if (sensorUpdate)
                    {
                        updateSensor.Status = "Success";
                        updateSensor.Message = string.Format("Confirm success updated sensor Name = {0}, TechId = {1}", incomingEnvelope.DeliveryObject.Name, incomingEnvelope.DeliveryObject.Equipment.TechId);
                    }
                    else
                    {
                        updateSensor.Status = "Error";
                        updateSensor.Message = "Something went wrong on the server during the updating of a new sensor";
                    }

                    var envelop = _messagePublisher.CreateOutgoingEnvelope<MSG.Server.SendSensorUpdatingResultMessage, SensorUpdatingResult>();
                    envelop.SensorName = incomingEnvelope.SensorName;
                    envelop.SensorTechId = incomingEnvelope.SensorTechId;
                    envelop.DeliveryObject = updateSensor;
                    _messagePublisher.Send(envelop);
                }

            }
        }

    }
}


