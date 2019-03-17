using System.Collections.Generic;
using Atdi.Contracts.CoreServices.DataLayer;
using Atdi.Contracts.CoreServices.EntityOrm;
using Atdi.Platform.Logging;
using System;
using MD = Atdi.DataModels.Sdrns.DeviceServer.Entities;
using Atdi.DataModels.EntityOrm;
using Atdi.Modules.Sdrn.DeviceServer;
using System.Xml;
using System.Linq;
using Atdi.DataModels.Sdrns.Device;
using Atdi.DataModels.Sdrns;
using Atdi.DataModels;
using Atdi.DataModels.DataConstraint;


namespace Atdi.AppUnits.Sdrn.DeviceServer.Repositories
{ 
    public sealed class SensorsRepository : IRepository<Sensor,int?>
    {
        private readonly IDataLayer<EntityDataOrm> _dataLayer;
        private readonly ILogger _logger;


        public SensorsRepository(IDataLayer<EntityDataOrm> dataLayer, ILogger logger)
        {
            this._dataLayer = dataLayer;
            this._logger = logger;
        }

        public int? Create(Sensor item)
        {
            int? idSensor = -1;
            var queryExecuter = this._dataLayer.Executor<SdrnServerDeviceDataContext>();
            try
            {
                var sensorData = item;
                queryExecuter.BeginTransaction();
               
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
                builderInsertSensor.SetValue(c => c.Status, sensorData.Status);
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


                        if (idSensorAntenna > -1)
                        {
                            if (sensorData.Antenna.Patterns != null)
                            {
                                int idSensorAntennaPattern = -1;
                                foreach (AntennaPattern patt in sensorData.Antenna.Patterns)
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
                            foreach (EquipmentSensitivity senseqps in sensorData.Equipment.Sensitivities)
                            {
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
                            foreach (Atdi.DataModels.Sdrns.GeoPoint geo in sensPolygon.Points)
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
                        }

                        if (sensorData.Locations != null)
                        {
                            int idsensLocations = -1;
                            foreach (var location in sensorData.Locations)
                            {
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
            }
            catch (Exception e)
            {
                queryExecuter.RollbackTransaction();
                this._logger.Exception(Contexts.ThisComponent, e);
            }
            return idSensor;
        }

        public bool Delete(int? id)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }


     
        public Sensor LoadObject(int? id)
        {
            throw new NotImplementedException();
        }

        public Sensor[] LoadObjectsWithRestrict()
        {
            throw new NotImplementedException();
        }

        public bool Update(Sensor item)
        {
            bool isSuccess = false;
            int? idSensor = -1;
            var queryExecuter = this._dataLayer.Executor<SdrnServerDeviceDataContext>();
            try
            {
                var sensorData = item;
                var builderSelectSensLocations = this._dataLayer.GetBuilder<MD.ISensor>().From();
                builderSelectSensLocations.Where(c => c.TechId, ConditionOperator.Equal, sensorData.Equipment.TechId);
                builderSelectSensLocations.Where(c => c.Name, ConditionOperator.Equal, sensorData.Name);
                builderSelectSensLocations.Select(c => c.Id);
                queryExecuter
                       .Fetch(builderSelectSensLocations, reader =>
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
                    queryExecuter.BeginTransaction();

                    var builderUpdateSens = this._dataLayer.GetBuilder<MD.ISensor>().Update();
                    builderUpdateSens.SetValue(c => c.Status, item.Status);
                    builderUpdateSens.Where(c => c.Id, ConditionOperator.Equal, idSensor);
                    if (queryExecuter.Execute(builderUpdateSens) > 0)
                    {
                        isSuccess = true;
                    }


                    if (sensorData.Locations.Length > 0)
                    {

                        var builderUpdateSensLocations = this._dataLayer.GetBuilder<MD.ISensorLocation>().Update();
                        builderUpdateSensLocations.SetValue(c => c.Status, "Z");
                        builderUpdateSensLocations.Where(c => c.SensorId, ConditionOperator.Equal, idSensor);
                        if (queryExecuter.Execute(builderUpdateSensLocations) > 0)
                        {
                            isSuccess = true;
                        }
                    }


                    if (sensorData.Locations != null)
                    {
                        foreach (var location in sensorData.Locations)
                        {
                            if (location.Status == "A")
                            {
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
                                    isSuccess = true;
                                    return true;
                                });

                            }
                        }
                    }
                    queryExecuter.CommitTransaction();
                }
            }
            catch (Exception e)
            {
                isSuccess = false;
                queryExecuter.RollbackTransaction();
                this._logger.Exception(Contexts.ThisComponent, e);
            }
            return isSuccess;
        }

        Sensor[] IRepository<Sensor,int?>.LoadAllObjects()
        {
            var listSensors = new List<Sensor>();
            Sensor val = null;
            var queryExecuter = this._dataLayer.Executor<SdrnServerDeviceDataContext>();
            var builderSelectSensor = this._dataLayer.GetBuilder<MD.ISensor>().From();
            builderSelectSensor.Select(c => c.Administration);
            builderSelectSensor.Select(c => c.Agl);
            builderSelectSensor.Select(c => c.Azimuth);
            builderSelectSensor.Select(c => c.BiuseDate);
            builderSelectSensor.Select(c => c.CreatedBy);
            builderSelectSensor.Select(c => c.CustData1);
            builderSelectSensor.Select(c => c.CustNbr1);
            builderSelectSensor.Select(c => c.CustTxt1);
            builderSelectSensor.Select(c => c.DateCreated);
            builderSelectSensor.Select(c => c.Elevation);
            builderSelectSensor.Select(c => c.EouseDate);
            builderSelectSensor.Select(c => c.IdSysArgus);
            builderSelectSensor.Select(c => c.Id);
            builderSelectSensor.Select(c => c.Name);
            builderSelectSensor.Select(c => c.NetworkId);
            builderSelectSensor.Select(c => c.OpDays);
            builderSelectSensor.Select(c => c.OpHhFr);
            builderSelectSensor.Select(c => c.OpHhTo);
            builderSelectSensor.Select(c => c.Remark);
            builderSelectSensor.Select(c => c.RxLoss);
            builderSelectSensor.Select(c => c.Status);
            builderSelectSensor.Select(c => c.StepMeasTime);
            builderSelectSensor.Select(c => c.TypeSensor);
            builderSelectSensor.Where(c => c.Id, ConditionOperator.GreaterThan, 0);
            builderSelectSensor.OrderByDesc(c => c.Id);
            queryExecuter.Fetch(builderSelectSensor, reader =>
            {
                while (reader.Read())
                {
                    val = new Sensor();
                    val.Administration = reader.GetValue(c => c.Administration);
                    val.BiuseDate = reader.GetValue(c => c.BiuseDate);
                    val.CreatedBy = reader.GetValue(c => c.CreatedBy);
                    val.CustDate1 = reader.GetValue(c => c.CustData1);
                    val.CustNbr1 = reader.GetValue(c => c.CustNbr1);
                    val.CustTxt1 = reader.GetValue(c => c.CustTxt1);
                    val.Created = reader.GetValue(c => c.DateCreated);
                    val.EouseDate = reader.GetValue(c => c.EouseDate);
                    val.Name = reader.GetValue(c => c.Name);
                    val.NetworkId = reader.GetValue(c => c.NetworkId);
                    val.Remark = reader.GetValue(c => c.Remark);
                    val.RxLoss = reader.GetValue(c => c.RxLoss);
                    val.Status = reader.GetValue(c => c.Status);
                    val.StepMeasTime = reader.GetValue(c => c.StepMeasTime);
                    val.Type = reader.GetValue(c => c.TypeSensor);


                    val.Antenna = new SensorAntenna();
                    var builderSelectSensorAntenna = this._dataLayer.GetBuilder<MD.ISensorAntenna>().From();
                    builderSelectSensorAntenna.Select(c => c.AddLoss);
                    builderSelectSensorAntenna.Select(c => c.AntClass);
                    builderSelectSensorAntenna.Select(c => c.AntDir);
                    builderSelectSensorAntenna.Select(c => c.Category);
                    builderSelectSensorAntenna.Select(c => c.Code);
                    builderSelectSensorAntenna.Select(c => c.CustData1);
                    builderSelectSensorAntenna.Select(c => c.CustNbr1);
                    builderSelectSensorAntenna.Select(c => c.CustTxt1);
                    builderSelectSensorAntenna.Select(c => c.GainMax);
                    builderSelectSensorAntenna.Select(c => c.GainType);
                    builderSelectSensorAntenna.Select(c => c.HbeamWidth);
                    builderSelectSensorAntenna.Select(c => c.Id);
                    builderSelectSensorAntenna.Select(c => c.LowerFreq);
                    builderSelectSensorAntenna.Select(c => c.Manufacturer);
                    builderSelectSensorAntenna.Select(c => c.Name);
                    builderSelectSensorAntenna.Select(c => c.Polarization);
                    builderSelectSensorAntenna.Select(c => c.Remark);
                    builderSelectSensorAntenna.Select(c => c.SensorId);
                    builderSelectSensorAntenna.Select(c => c.Slewang);
                    builderSelectSensorAntenna.Select(c => c.TechId);
                    builderSelectSensorAntenna.Select(c => c.UpperFreq);
                    builderSelectSensorAntenna.Select(c => c.UseType);
                    builderSelectSensorAntenna.Select(c => c.VbeamWidth);
                    builderSelectSensorAntenna.Select(c => c.Xpd);
                    builderSelectSensorAntenna.Where(c => c.SensorId, ConditionOperator.Equal, reader.GetValue(c => c.Id));

                    queryExecuter.Fetch(builderSelectSensorAntenna, readerSensorAntenna =>
                    {
                        while (readerSensorAntenna.Read())
                        {
                            if (readerSensorAntenna.GetValue(c => c.AddLoss) != null) val.Antenna.AddLoss = readerSensorAntenna.GetValue(c => c.AddLoss).Value;
                            val.Antenna.Class = readerSensorAntenna.GetValue(c => c.AntClass);
                            AntennaDirectivity outAntennaDir;
                            if (Enum.TryParse<AntennaDirectivity>(readerSensorAntenna.GetValue(c => c.AntDir) != null ? readerSensorAntenna.GetValue(c => c.AntDir).ToString() : "", out outAntennaDir))
                            {
                                val.Antenna.Direction = outAntennaDir;
                            }
                            val.Antenna.Category = readerSensorAntenna.GetValue(c => c.Category);
                            val.Antenna.Code = readerSensorAntenna.GetValue(c => c.Code);
                            //if (readerSensorAntenna.GetValue(c => c.CustData1) != null) val.Antenna.CustDate1 = readerSensorAntenna.GetValue(c => c.CustData1).Value.ToShortDateString();
                            if (readerSensorAntenna.GetValue(c => c.CustData1) != null)
                            {
                                val.Antenna.CustDate1 = readerSensorAntenna.GetValue(c => c.CustData1).Value;
                            }
                            if (readerSensorAntenna.GetValue(c => c.CustNbr1) != null) val.Antenna.CustNbr1 = readerSensorAntenna.GetValue(c => c.CustNbr1).Value;
                            val.Antenna.CustTxt1 = readerSensorAntenna.GetValue(c => c.CustTxt1);
                            if (readerSensorAntenna.GetValue(c => c.GainMax) != null) val.Antenna.GainMax = readerSensorAntenna.GetValue(c => c.GainMax).Value;
                            val.Antenna.GainType = readerSensorAntenna.GetValue(c => c.GainType);
                            val.Antenna.HBeamwidth = readerSensorAntenna.GetValue(c => c.HbeamWidth);
                            val.Antenna.LowerFreq_MHz = readerSensorAntenna.GetValue(c => c.LowerFreq);
                            val.Antenna.Manufacturer = readerSensorAntenna.GetValue(c => c.Manufacturer);
                            val.Antenna.Name = readerSensorAntenna.GetValue(c => c.Name);
                            AntennaPolarization outAntennaPolar;
                            if (Enum.TryParse<AntennaPolarization>(readerSensorAntenna.GetValue(c => c.Polarization) != null ? readerSensorAntenna.GetValue(c => c.Polarization).ToString() : "", out outAntennaPolar))
                            {
                                val.Antenna.Polarization = outAntennaPolar;
                            }
                            val.Antenna.Remark = readerSensorAntenna.GetValue(c => c.Remark);
                            val.Antenna.SlewAng = readerSensorAntenna.GetValue(c => c.Slewang);
                            val.Antenna.TechId = readerSensorAntenna.GetValue(c => c.TechId);
                            val.Antenna.UpperFreq_MHz = readerSensorAntenna.GetValue(c => c.UpperFreq);
                            val.Antenna.UseType = readerSensorAntenna.GetValue(c => c.UseType);
                            val.Antenna.VBeamwidth = readerSensorAntenna.GetValue(c => c.VbeamWidth);
                            val.Antenna.XPD = readerSensorAntenna.GetValue(c => c.Xpd);


                            var antennaPattern = new List<AntennaPattern>();

                            var builderSelectAntennaPattern = this._dataLayer.GetBuilder<MD.IAntennaPattern>().From();
                            builderSelectAntennaPattern.Select(c => c.DiagA);
                            builderSelectAntennaPattern.Select(c => c.DiagH);
                            builderSelectAntennaPattern.Select(c => c.DiagV);
                            builderSelectAntennaPattern.Select(c => c.Freq);
                            builderSelectAntennaPattern.Select(c => c.Gain);
                            builderSelectAntennaPattern.Where(c => c.SensorAntennaId, ConditionOperator.Equal, readerSensorAntenna.GetValue(c => c.Id));

                            queryExecuter.Fetch(builderSelectAntennaPattern, readerIAntennaPattern =>
                            {
                                while (readerIAntennaPattern.Read())
                                {
                                    var patt = new AntennaPattern();
                                    patt.DiagA = readerIAntennaPattern.GetValue(c => c.DiagA);
                                    patt.DiagH = readerIAntennaPattern.GetValue(c => c.DiagH);
                                    patt.DiagV = readerIAntennaPattern.GetValue(c => c.DiagV);
                                    if (readerIAntennaPattern.GetValue(c => c.Freq) != null) patt.Freq_MHz = readerIAntennaPattern.GetValue(c => c.Freq).Value;
                                    if (readerIAntennaPattern.GetValue(c => c.Gain) != null) patt.Freq_MHz = readerIAntennaPattern.GetValue(c => c.Gain).Value;
                                    antennaPattern.Add(patt);
                                }
                                return true;
                            });
                            val.Antenna.Patterns = antennaPattern.ToArray();
                        }
                        return true;
                    });


                    val.Equipment = new SensorEquipment();
                    var builderSelectEquipment = this._dataLayer.GetBuilder<MD.ISensorEquipment>().From();
                    builderSelectEquipment.Select(c => c.Category);
                    builderSelectEquipment.Select(c => c.Code);
                    builderSelectEquipment.Select(c => c.CustData1);
                    builderSelectEquipment.Select(c => c.CustNbr1);
                    builderSelectEquipment.Select(c => c.CustTxt1);
                    builderSelectEquipment.Select(c => c.EquipClass);
                    builderSelectEquipment.Select(c => c.Family);
                    builderSelectEquipment.Select(c => c.FftPointMax);
                    builderSelectEquipment.Select(c => c.Id);
                    builderSelectEquipment.Select(c => c.LowerFreq);
                    builderSelectEquipment.Select(c => c.Manufacturer);
                    builderSelectEquipment.Select(c => c.Mobility);
                    builderSelectEquipment.Select(c => c.Name);
                    builderSelectEquipment.Select(c => c.OperationMode);
                    builderSelectEquipment.Select(c => c.RbwMax);
                    builderSelectEquipment.Select(c => c.RbwMin);
                    builderSelectEquipment.Select(c => c.RefLevelDbm);
                    builderSelectEquipment.Select(c => c.Remark);
                    builderSelectEquipment.Select(c => c.SensorId);
                    builderSelectEquipment.Select(c => c.TechId);
                    builderSelectEquipment.Select(c => c.TuningStep);
                    builderSelectEquipment.Select(c => c.Type);
                    builderSelectEquipment.Select(c => c.UpperFreq);
                    builderSelectEquipment.Select(c => c.UserType);
                    builderSelectEquipment.Select(c => c.VbwMax);
                    builderSelectEquipment.Select(c => c.VbwMin);
                    builderSelectEquipment.Select(c => c.Version);
                    builderSelectEquipment.Where(c => c.SensorId, ConditionOperator.Equal, reader.GetValue(c => c.Id));
                    queryExecuter.Fetch(builderSelectEquipment, readerEquipment =>
                    {
                        while (readerEquipment.Read())
                        {
                            val.Equipment.Category = readerEquipment.GetValue(c => c.Category);
                            val.Equipment.Code = readerEquipment.GetValue(c => c.Code);
                            val.Equipment.CustDate1 = readerEquipment.GetValue(c => c.CustData1);
                            val.Equipment.CustNbr1 = readerEquipment.GetValue(c => c.CustNbr1);
                            val.Equipment.CustTxt1 = readerEquipment.GetValue(c => c.CustTxt1);
                            val.Equipment.Class = readerEquipment.GetValue(c => c.EquipClass);
                            val.Equipment.Family = readerEquipment.GetValue(c => c.Family);
                            val.Equipment.FFTPointMax = readerEquipment.GetValue(c => c.FftPointMax);
                            val.Equipment.LowerFreq_MHz = readerEquipment.GetValue(c => c.LowerFreq);
                            val.Equipment.Manufacturer = readerEquipment.GetValue(c => c.Manufacturer);
                            val.Equipment.Mobility = readerEquipment.GetValue(c => c.Mobility);
                            val.Equipment.Name = readerEquipment.GetValue(c => c.Name);
                            val.Equipment.OperationMode = readerEquipment.GetValue(c => c.OperationMode);
                            val.Equipment.RBWMax_kHz = readerEquipment.GetValue(c => c.RbwMax);
                            val.Equipment.RBWMin_kHz = readerEquipment.GetValue(c => c.RbwMin);
                            val.Equipment.MaxRefLevel_dBm = readerEquipment.GetValue(c => c.RefLevelDbm);
                            val.Equipment.Remark = readerEquipment.GetValue(c => c.Remark);
                            val.Equipment.TechId = readerEquipment.GetValue(c => c.TechId);
                            val.Equipment.TuningStep_Hz = readerEquipment.GetValue(c => c.TuningStep);
                            val.Equipment.Type = readerEquipment.GetValue(c => c.Type);
                            val.Equipment.UpperFreq_MHz = readerEquipment.GetValue(c => c.UpperFreq);
                            val.Equipment.UseType = readerEquipment.GetValue(c => c.UserType);
                            val.Equipment.VBWMax_kHz = readerEquipment.GetValue(c => c.VbwMax);
                            val.Equipment.VBWMin_kHz = readerEquipment.GetValue(c => c.VbwMin);
                            val.Equipment.Version = readerEquipment.GetValue(c => c.Version);




                            var sensitivytes = new List<EquipmentSensitivity>();
                            var builderSelectSensorSensitivites = this._dataLayer.GetBuilder<MD.ISensorSensitivites>().From();
                            builderSelectSensorSensitivites.Select(c => c.AddLoss);
                            builderSelectSensorSensitivites.Select(c => c.Freq);
                            builderSelectSensorSensitivites.Select(c => c.FreqStability);
                            builderSelectSensorSensitivites.Select(c => c.Id);
                            builderSelectSensorSensitivites.Select(c => c.Ktbf);
                            builderSelectSensorSensitivites.Select(c => c.Noisef);
                            builderSelectSensorSensitivites.Select(c => c.SensorEquipId);
                            builderSelectSensorSensitivites.Where(c => c.SensorEquipId, ConditionOperator.Equal, readerEquipment.GetValue(c => c.Id));
                            queryExecuter.Fetch(builderSelectSensorSensitivites, readerSensorSensitivites =>
                            {
                                while (readerSensorSensitivites.Read())
                                {
                                    var sens = new EquipmentSensitivity();
                                    sens.AddLoss = readerSensorSensitivites.GetValue(c => c.AddLoss);
                                    if (readerSensorSensitivites.GetValue(c => c.Freq) != null) sens.Freq_MHz = readerSensorSensitivites.GetValue(c => c.Freq).Value;
                                    if (readerSensorSensitivites.GetValue(c => c.FreqStability) != null) sens.FreqStability = readerSensorSensitivites.GetValue(c => c.FreqStability);
                                    if (readerSensorSensitivites.GetValue(c => c.Ktbf) != null) sens.KTBF_dBm = readerSensorSensitivites.GetValue(c => c.Ktbf);
                                    if (readerSensorSensitivites.GetValue(c => c.Noisef) != null) sens.NoiseF = readerSensorSensitivites.GetValue(c => c.Noisef);
                                    sensitivytes.Add(sens);
                                }
                                return true;
                            });
                            val.Equipment.Sensitivities = sensitivytes.ToArray();
                        }
                        return true;
                    });


                    var sensorLocation = new List<SensorLocation>();
                    var builderSelectsensorLocation = this._dataLayer.GetBuilder<MD.ISensorLocation>().From();
                    builderSelectsensorLocation.Select(c => c.Asl);
                    builderSelectsensorLocation.Select(c => c.DateCreated);
                    builderSelectsensorLocation.Select(c => c.DateFrom);
                    builderSelectsensorLocation.Select(c => c.DateTo);
                    builderSelectsensorLocation.Select(c => c.Id);
                    builderSelectsensorLocation.Select(c => c.Lat);
                    builderSelectsensorLocation.Select(c => c.Lon);
                    builderSelectsensorLocation.Select(c => c.Status);

                    builderSelectsensorLocation.Where(c => c.SensorId, ConditionOperator.Equal, reader.GetValue(c => c.Id));
                    builderSelectsensorLocation.Where(c => c.Status, ConditionOperator.Equal, "A");
                    queryExecuter.Fetch(builderSelectsensorLocation, readersensorLocation =>
                    {
                        while (readersensorLocation.Read())
                        {
                            var sensLocation = new SensorLocation();
                            sensLocation.ASL = readersensorLocation.GetValue(c => c.Asl);
                            sensLocation.Created = readersensorLocation.GetValue(c => c.DateCreated);
                            sensLocation.From = readersensorLocation.GetValue(c => c.DateFrom);
                            sensLocation.To = readersensorLocation.GetValue(c => c.DateTo);
                            if (readersensorLocation.GetValue(c => c.Lat) != null)
                            {
                                sensLocation.Lat = readersensorLocation.GetValue(c => c.Lat).Value;
                            }
                            if (readersensorLocation.GetValue(c => c.Lon) != null)
                            {
                                sensLocation.Lon = readersensorLocation.GetValue(c => c.Lon).Value;
                            }
                            sensLocation.Status = readersensorLocation.GetValue(c => c.Status);
                            sensorLocation.Add(sensLocation);
                        }
                        return true;
                    });
                    val.Locations = sensorLocation.ToArray();

                    var sensLoc = new SensorPolygon();
                    var sensorListGeoPoint = new List<GeoPoint>();
                    var builderSelectSensorPolygonPoint = this._dataLayer.GetBuilder<MD.ISensorPolygon>().From();
                    builderSelectSensorPolygonPoint.Select(c => c.Id);
                    builderSelectSensorPolygonPoint.Select(c => c.Lat);
                    builderSelectSensorPolygonPoint.Select(c => c.Lon);
                    builderSelectSensorPolygonPoint.Where(c => c.SensorId, ConditionOperator.Equal, reader.GetValue(c => c.Id));
                    queryExecuter.Fetch(builderSelectSensorPolygonPoint, readersensorPolygonPoint =>
                    {
                        while (readersensorPolygonPoint.Read())
                        {
                            var geoPoint = new GeoPoint();
                            geoPoint.Lon = readersensorPolygonPoint.GetValue(c => c.Lon);
                            geoPoint.Lat = readersensorPolygonPoint.GetValue(c => c.Lat);
                            sensorListGeoPoint.Add(geoPoint);
                        }
                        return true;
                    });
                    sensLoc.Points = sensorListGeoPoint.ToArray();
                    val.Polygon = sensLoc;
                }
                return true;
            });
            if (val != null)
            {
                listSensors.Add(val);
            }
            return listSensors.ToArray();
        }
    }
}


