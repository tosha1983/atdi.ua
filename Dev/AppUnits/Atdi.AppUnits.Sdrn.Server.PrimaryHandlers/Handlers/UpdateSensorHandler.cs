
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

                if (idSensor > -1)
                {
                    var builderUpdateSensor = this._dataLayer.GetBuilder<MD.ISensor>().Update();
                    if (sensorData.Administration != null) builderUpdateSensor.SetValue(c => c.Administration, sensorData.Administration);
                    if (sensorData.BiuseDate != null) builderUpdateSensor.SetValue(c => c.BiuseDate, sensorData.BiuseDate);
                    if (sensorData.CreatedBy != null) builderUpdateSensor.SetValue(c => c.CreatedBy, sensorData.CreatedBy);
                    if (sensorData.CustDate1 != null) builderUpdateSensor.SetValue(c => c.CustData1, sensorData.CustDate1);
                    if (sensorData.CustNbr1 != null) builderUpdateSensor.SetValue(c => c.CustNbr1, sensorData.CustNbr1);
                    if (sensorData.CustTxt1 != null) builderUpdateSensor.SetValue(c => c.CustTxt1, sensorData.CustTxt1);
                    if (sensorData.Created != null) builderUpdateSensor.SetValue(c => c.DateCreated, sensorData.Created);
                    if (sensorData.EouseDate != null) builderUpdateSensor.SetValue(c => c.EouseDate, sensorData.EouseDate);
                    if (sensorData.NetworkId != null) builderUpdateSensor.SetValue(c => c.NetworkId, sensorData.NetworkId);
                    if (sensorData.Remark != null) builderUpdateSensor.SetValue(c => c.Remark, sensorData.Remark);
                    if (sensorData.RxLoss != null) builderUpdateSensor.SetValue(c => c.RxLoss, sensorData.RxLoss);
                    if (sensorData.Status != null) builderUpdateSensor.SetValue(c => c.Status, sensorData.Status);
                    if (sensorData.StepMeasTime != null) builderUpdateSensor.SetValue(c => c.StepMeasTime, sensorData.StepMeasTime);
                    if (sensorData.Type != null) builderUpdateSensor.SetValue(c => c.TypeSensor, sensorData.Type);
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
                            }

                            if (idSensorAntenna > -1)
                            {
                                var builderUpdateAntenna = this._dataLayer.GetBuilder<MD.ISensorAntenna>().Update();
                                builderUpdateAntenna.SetValue(c => c.AddLoss, sensorData.Antenna.AddLoss);
                                if (sensorData.Antenna.Class != null) builderUpdateAntenna.SetValue(c => c.AntClass, sensorData.Antenna.Class);
                                builderUpdateAntenna.SetValue(c => c.AntDir, sensorData.Antenna.Direction.ToString());
                                if (sensorData.Antenna.Category != null) builderUpdateAntenna.SetValue(c => c.Category, sensorData.Antenna.Category);
                                if (sensorData.Antenna.Code != null) builderUpdateAntenna.SetValue(c => c.Code, sensorData.Antenna.Code);
                                if (sensorData.Antenna.CustDate1 != null) builderUpdateAntenna.SetValue(c => c.CustData1, sensorData.Antenna.CustDate1);
                                builderUpdateAntenna.SetValue(c => c.CustNbr1, sensorData.Antenna.CustNbr1);
                                if (sensorData.Antenna.CustTxt1 != null) builderUpdateAntenna.SetValue(c => c.CustTxt1, sensorData.Antenna.CustTxt1);
                                builderUpdateAntenna.SetValue(c => c.GainMax, sensorData.Antenna.GainMax);
                                if (sensorData.Antenna.GainType != null) builderUpdateAntenna.SetValue(c => c.GainType, sensorData.Antenna.GainType);
                                if (sensorData.Antenna.HBeamwidth != null) builderUpdateAntenna.SetValue(c => c.HbeamWidth, sensorData.Antenna.HBeamwidth);
                                if (sensorData.Antenna.LowerFreq_MHz != null) builderUpdateAntenna.SetValue(c => c.LowerFreq, sensorData.Antenna.LowerFreq_MHz);
                                if (sensorData.Antenna.Manufacturer != null) builderUpdateAntenna.SetValue(c => c.Manufacturer, sensorData.Antenna.Manufacturer);
                                if (sensorData.Antenna.Name != null) builderUpdateAntenna.SetValue(c => c.Name, sensorData.Antenna.Name);
                                builderUpdateAntenna.SetValue(c => c.Polarization, sensorData.Antenna.Polarization.ToString());
                                if (sensorData.Antenna.Remark != null) builderUpdateAntenna.SetValue(c => c.Remark, sensorData.Antenna.Remark);
                                if (sensorData.Antenna.SlewAng != null) builderUpdateAntenna.SetValue(c => c.Slewang, sensorData.Antenna.SlewAng);
                                if (sensorData.Antenna.TechId != null) builderUpdateAntenna.SetValue(c => c.TechId, sensorData.Antenna.TechId);
                                if (sensorData.Antenna.UpperFreq_MHz != null) builderUpdateAntenna.SetValue(c => c.UpperFreq, sensorData.Antenna.UpperFreq_MHz);
                                if (sensorData.Antenna.UseType != null) builderUpdateAntenna.SetValue(c => c.UseType, sensorData.Antenna.UseType);
                                if (sensorData.Antenna.VBeamwidth != null) builderUpdateAntenna.SetValue(c => c.VbeamWidth, sensorData.Antenna.VBeamwidth);
                                if (sensorData.Antenna.XPD != null) builderUpdateAntenna.SetValue(c => c.Xpd, sensorData.Antenna.XPD);
                                builderUpdateAntenna.Where(c => c.Id, ConditionOperator.Equal, idSensorAntenna);
                                if (queryExecuter
                                 .Execute(builderUpdateAntenna) > 0)
                                {

                                    if (sensorData.Antenna.Patterns != null)
                                    {
                                        int idSensorAntennaPattern = -1;
                                        foreach (AntennaPattern patt in sensorData.Antenna.Patterns)
                                        {
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

                                            if (idSensorAntennaPattern > -1)
                                            {
                                                var builderUpdateAntennaPattern = this._dataLayer.GetBuilder<MD.IAntennaPattern>().Update();
                                                if (patt.DiagA != null) builderUpdateAntennaPattern.SetValue(c => c.DiagA, patt.DiagA);
                                                if (patt.DiagH != null) builderUpdateAntennaPattern.SetValue(c => c.DiagH, patt.DiagH);
                                                if (patt.DiagV != null) builderUpdateAntennaPattern.SetValue(c => c.DiagV, patt.DiagV);
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
                            }

                            if (idSensorEquipment > -1)
                            {

                                var builderUpdateEquipment = this._dataLayer.GetBuilder<MD.ISensorEquipment>().Update();
                                if (sensorData.Equipment.Category != null) builderUpdateEquipment.SetValue(c => c.Category, sensorData.Equipment.Category);
                                if (sensorData.Equipment.Code != null) builderUpdateEquipment.SetValue(c => c.Code, sensorData.Equipment.Code);
                                if (sensorData.Equipment.CustDate1 != null) builderUpdateEquipment.SetValue(c => c.CustData1, sensorData.Equipment.CustDate1);
                                if (sensorData.Equipment.CustNbr1 != null) builderUpdateEquipment.SetValue(c => c.CustNbr1, sensorData.Equipment.CustNbr1);
                                if (sensorData.Equipment.CustTxt1 != null) builderUpdateEquipment.SetValue(c => c.CustTxt1, sensorData.Equipment.CustTxt1);
                                if (sensorData.Equipment.Class != null) builderUpdateEquipment.SetValue(c => c.EquipClass, sensorData.Equipment.Class);
                                if (sensorData.Equipment.Family != null) builderUpdateEquipment.SetValue(c => c.Family, sensorData.Equipment.Family);
                                if (sensorData.Equipment.FFTPointMax != null) builderUpdateEquipment.SetValue(c => c.FftPointMax, sensorData.Equipment.FFTPointMax);
                                if (sensorData.Equipment.LowerFreq_MHz != null) builderUpdateEquipment.SetValue(c => c.LowerFreq, sensorData.Equipment.LowerFreq_MHz);
                                if (sensorData.Equipment.Manufacturer != null) builderUpdateEquipment.SetValue(c => c.Manufacturer, sensorData.Equipment.Manufacturer);
                                if (sensorData.Equipment.Mobility != null) builderUpdateEquipment.SetValue(c => c.Mobility, sensorData.Equipment.Mobility);
                                if (sensorData.Equipment.Name != null) builderUpdateEquipment.SetValue(c => c.Name, sensorData.Equipment.Name);
                                if (sensorData.Equipment.OperationMode != null) builderUpdateEquipment.SetValue(c => c.OperationMode, sensorData.Equipment.OperationMode);
                                if (sensorData.Equipment.RBWMax_kHz != null) builderUpdateEquipment.SetValue(c => c.RbwMax, sensorData.Equipment.RBWMax_kHz);
                                if (sensorData.Equipment.RBWMin_kHz != null) builderUpdateEquipment.SetValue(c => c.RbwMin, sensorData.Equipment.RBWMin_kHz);
                                if (sensorData.Equipment.MaxRefLevel_dBm != null) builderUpdateEquipment.SetValue(c => c.RefLevelDbm, sensorData.Equipment.MaxRefLevel_dBm);
                                if (sensorData.Equipment.Remark != null) builderUpdateEquipment.SetValue(c => c.Remark, sensorData.Equipment.Remark);
                                if (sensorData.Equipment.TuningStep_Hz != null) builderUpdateEquipment.SetValue(c => c.TuningStep, sensorData.Equipment.TuningStep_Hz);
                                if (sensorData.Equipment.Type != null) builderUpdateEquipment.SetValue(c => c.Type, sensorData.Equipment.Type);
                                if (sensorData.Equipment.UpperFreq_MHz != null) builderUpdateEquipment.SetValue(c => c.UpperFreq, sensorData.Equipment.UpperFreq_MHz);
                                if (sensorData.Equipment.UseType != null) builderUpdateEquipment.SetValue(c => c.UserType, sensorData.Equipment.UseType);
                                if (sensorData.Equipment.VBWMax_kHz != null) builderUpdateEquipment.SetValue(c => c.VbwMax, sensorData.Equipment.VBWMax_kHz);
                                if (sensorData.Equipment.VBWMin_kHz != null) builderUpdateEquipment.SetValue(c => c.VbwMin, sensorData.Equipment.VBWMin_kHz);
                                if (sensorData.Equipment.Version != null) builderUpdateEquipment.SetValue(c => c.Version, sensorData.Equipment.Version);
                                builderUpdateEquipment.Where(c => c.Id, ConditionOperator.Equal, idSensorEquipment);
                                if (queryExecuter
                                .Execute(builderUpdateEquipment) > 0)
                                {
                                    if (sensorData.Equipment.Sensitivities != null)
                                    {
                                        foreach (EquipmentSensitivity senseqps in sensorData.Equipment.Sensitivities)
                                        {
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

                                            if (idSensorEquipmentSensitivities > -1)
                                            {
                                                var builderUpdateSensorEquipmentSensitivities = this._dataLayer.GetBuilder<MD.ISensorSensitivites>().Update();
                                                if (senseqps.AddLoss != null) builderUpdateSensorEquipmentSensitivities.SetValue(c => c.AddLoss, senseqps.AddLoss);
                                                builderUpdateSensorEquipmentSensitivities.SetValue(c => c.Freq, senseqps.Freq_MHz);
                                                if (senseqps.FreqStability != null) builderUpdateSensorEquipmentSensitivities.SetValue(c => c.FreqStability, senseqps.FreqStability);
                                                if (senseqps.KTBF_dBm != null) builderUpdateSensorEquipmentSensitivities.SetValue(c => c.Ktbf, senseqps.KTBF_dBm);
                                                if (senseqps.NoiseF != null) builderUpdateSensorEquipmentSensitivities.SetValue(c => c.Noisef, senseqps.NoiseF);
                                                builderUpdateSensorEquipmentSensitivities.SetValue(c => c.SensorEquipId, idSensorEquipment);
                                                builderUpdateSensorEquipmentSensitivities.Where(c => c.Id, ConditionOperator.Equal, idSensorEquipmentSensitivities);
                                                queryExecuter
                                               .Execute(builderUpdateSensorEquipmentSensitivities);
                                            }
                                            else
                                            {
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
                                    }
                                }
                                if (sensorData.Polygon != null)
                                {
                                    SensorPolygon sensPolygon = sensorData.Polygon;
                                    foreach (Atdi.DataModels.Sdrns.GeoPoint geo in sensPolygon.Points)
                                    {
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
                                        else
                                        {

                                            var builderUpdateSensorPolygons = this._dataLayer.GetBuilder<MD.ISensorPolygon>().Update();
                                            if (geo.Lat != null) builderUpdateSensorPolygons.SetValue(c => c.Lat, geo.Lat);
                                            if (geo.Lon != null) builderUpdateSensorPolygons.SetValue(c => c.Lon, geo.Lon);
                                            builderUpdateSensorPolygons.SetValue(c => c.SensorId, idSensor);
                                            builderUpdateSensorPolygons.Where(c => c.Id, ConditionOperator.Equal, idSensorPolygon);
                                            queryExecuter
                                            .Execute(builderUpdateSensorPolygons);
                                        }
                                    }
                                }

                                if (sensorData.Locations != null)
                                {
                                    foreach (var location in sensorData.Locations)
                                    {

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
                                        if (location.ASL != null) builderInsertSensLocations.SetValue(c => c.Asl, location.ASL);
                                        if (location.From != null) builderInsertSensLocations.SetValue(c => c.DateFrom, location.From);
                                        if (location.To != null) builderInsertSensLocations.SetValue(c => c.DateTo, location.To);
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
                this._eventEmitter.Emit("OnEvent6", "UpdateSensorProcess");
                result.Status = SdrnMessageHandlingStatus.Confirmed;

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
                    if (sensorUpdate)
                    {
                        this._eventEmitter.Emit("OnSensorUpdate", "UpdateSensorProccesing");
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
                    // формируем объект подтвержденяи о обновлении данных о сенсоре
                    var updateSensor= new SensorUpdatingResult
                    {
                        EquipmentTechId = incomingEnvelope.SensorTechId,
                        SensorName = incomingEnvelope.SensorName,
                        SdrnServer = this._environment.ServerInstance
                    };

                    if (result.Status == SdrnMessageHandlingStatus.Error)
                    {
                        updateSensor.Status = "ERROR";
                        updateSensor.Message = "Something went wrong on the server";
                    }
                    else if (sensorUpdate)
                    {
                        updateSensor.Status = "Success";
                        updateSensor.Message = string.Format("Confirm success updated sensor Name = {0}, TechId = {1}", incomingEnvelope.DeliveryObject.Name, incomingEnvelope.DeliveryObject.Equipment.TechId);
                    }
                    else
                    {
                        updateSensor.Status = "ERROR";
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


