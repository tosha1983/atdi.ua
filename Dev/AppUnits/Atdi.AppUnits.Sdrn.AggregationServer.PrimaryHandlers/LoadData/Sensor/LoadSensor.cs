﻿using Atdi.Contracts.Api.DataBus;
using Atdi.Platform.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.DataModels.Sdrns.Server;
using Atdi.Contracts.CoreServices.DataLayer;
using Atdi.Contracts.CoreServices.EntityOrm;
using Atdi.DataModels.DataConstraint;
using Atdi.Contracts.Sdrn.Server;
using MD = Atdi.DataModels.Sdrns.Server.Entities;

namespace Atdi.AppUnits.Sdrn.AggregationServer.PrimaryHandlers.Handlers
{
    /// <summary>
    /// Загрузка из БД AggregationServer сведений о сенсорах
    /// </summary>
    public sealed class LoadSensor
    {
        private readonly IDataLayer<EntityDataOrm> _dataLayer;
        private readonly ILogger _logger;



        public LoadSensor(IDataLayer<EntityDataOrm> dataLayer, ILogger logger)
        {
            this._dataLayer = dataLayer;
            this._logger = logger;
        }

        public bool GetAggregationServerBySensorId(long SensorId, out string AggregationServerInstance)
        {
            var isClusterMode = false;
            string aggregationServerInstance = null;
            var queryExecuter = this._dataLayer.Executor<SdrnServerDataContext>();
            var builderAggregationSensor = this._dataLayer.GetBuilder<MD.ILinkAggregationSensor>().From();
            builderAggregationSensor.Select(c => c.AggregationServerName);
            builderAggregationSensor.Select(c => c.Id);
            builderAggregationSensor.Where(c => c.SENSOR.Id, ConditionOperator.Equal, SensorId);
            queryExecuter.Fetch(builderAggregationSensor, readerAggregationSensor =>
            {
                while (readerAggregationSensor.Read())
                {
                    aggregationServerInstance = readerAggregationSensor.GetValue(c => c.AggregationServerName);
                    if (!string.IsNullOrEmpty(aggregationServerInstance))
                    {
                        isClusterMode = true;
                    }
                    break;
                }
                return true;
            });
            AggregationServerInstance = aggregationServerInstance;
            return isClusterMode;
        }

        public Sensor LoadObjectSensor(string SensorName, string TechId)
        {
            Sensor val = null;
            try
            {
                this._logger.Info(Contexts.ThisComponent, Categories.Processing, Events.HandlerCallLoadObjectSensorMethod.Text);
                var queryExecuter = this._dataLayer.Executor<SdrnServerDataContext>();
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
                builderSelectSensor.Where(c => c.Name, ConditionOperator.Equal, SensorName);
                builderSelectSensor.Where(c => c.TechId, ConditionOperator.Equal, TechId);
                builderSelectSensor.OrderByDesc(c => c.Id);
                queryExecuter.Fetch(builderSelectSensor, reader =>
                {
                    while (reader.Read())
                    {
                        val = new Sensor();
                        val.Administration = reader.GetValue(c => c.Administration);
                        val.AGL = reader.GetValue(c => c.Agl);
                        val.Azimuth = reader.GetValue(c => c.Azimuth);
                        val.BiuseDate = reader.GetValue(c => c.BiuseDate);
                        val.CreatedBy = reader.GetValue(c => c.CreatedBy);
                        val.CustDate1 = reader.GetValue(c => c.CustData1);
                        val.CustNbr1 = reader.GetValue(c => c.CustNbr1);
                        val.CustTxt1 = reader.GetValue(c => c.CustTxt1);
                        val.DateCreated = reader.GetValue(c => c.DateCreated);
                        val.Elevation = reader.GetValue(c => c.Elevation);
                        val.EouseDate = reader.GetValue(c => c.EouseDate);
                        val.IdSysARGUS = reader.GetValue(c => c.IdSysArgus);
                        val.Name = reader.GetValue(c => c.Name);
                        val.NetworkId = reader.GetValue(c => c.NetworkId);
                        val.OpDays = reader.GetValue(c => c.OpDays).HasValue ? reader.GetValue(c => c.OpDays).ToString() : null;
                        val.OpHHFr = reader.GetValue(c => c.OpHhFr);
                        val.OpHHTo = reader.GetValue(c => c.OpHhTo);
                        val.Remark = reader.GetValue(c => c.Remark);
                        val.RxLoss = reader.GetValue(c => c.RxLoss);
                        val.Status = reader.GetValue(c => c.Status);
                        val.StepMeasTime = reader.GetValue(c => c.StepMeasTime);
                        val.TypeSensor = reader.GetValue(c => c.TypeSensor);
                        val.Id = new SensorIdentifier();
                        val.Id.Value = reader.GetValue(c => c.Id);


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
                        builderSelectSensorAntenna.Select(c => c.SENSOR.Id);
                        builderSelectSensorAntenna.Select(c => c.Slewang);
                        builderSelectSensorAntenna.Select(c => c.TechId);
                        builderSelectSensorAntenna.Select(c => c.UpperFreq);
                        builderSelectSensorAntenna.Select(c => c.UseType);
                        builderSelectSensorAntenna.Select(c => c.VbeamWidth);
                        builderSelectSensorAntenna.Select(c => c.Xpd);
                        builderSelectSensorAntenna.Where(c => c.SENSOR.Id, ConditionOperator.Equal, reader.GetValue(c=>c.Id));

                        queryExecuter.Fetch(builderSelectSensorAntenna, readerSensorAntenna =>
                        {
                            while (readerSensorAntenna.Read())
                            {
                                if (readerSensorAntenna.GetValue(c => c.AddLoss) != null) val.Antenna.AddLoss = readerSensorAntenna.GetValue(c => c.AddLoss).Value;
                                val.Antenna.AntClass = readerSensorAntenna.GetValue(c => c.AntClass);
                                AntennaDirectional outAntennaDir;
                                if (Enum.TryParse<AntennaDirectional>(readerSensorAntenna.GetValue(c => c.AntDir) != null ? readerSensorAntenna.GetValue(c => c.AntDir).ToString() : "", out outAntennaDir))
                                {
                                    val.Antenna.AntDir = outAntennaDir;
                                }
                                val.Antenna.Category = readerSensorAntenna.GetValue(c => c.Category);
                                val.Antenna.Code = readerSensorAntenna.GetValue(c => c.Code);
                                DateTime? custDate1 = readerSensorAntenna.GetValue(c => c.CustData1);
                                if (custDate1 != null) val.Antenna.CustDate1 = custDate1;
                                if (readerSensorAntenna.GetValue(c => c.CustNbr1) != null) val.Antenna.CustNbr1 = readerSensorAntenna.GetValue(c => c.CustNbr1).Value;
                                val.Antenna.CustTxt1 = readerSensorAntenna.GetValue(c => c.CustTxt1);
                                if (readerSensorAntenna.GetValue(c => c.GainMax) != null) val.Antenna.GainMax = readerSensorAntenna.GetValue(c => c.GainMax).Value;
                                val.Antenna.GainType = readerSensorAntenna.GetValue(c => c.GainType);
                                val.Antenna.HBeamwidth = readerSensorAntenna.GetValue(c => c.HbeamWidth);
                                val.Antenna.LowerFreq = readerSensorAntenna.GetValue(c => c.LowerFreq);
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
                                val.Antenna.UpperFreq = readerSensorAntenna.GetValue(c => c.UpperFreq);
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
                                builderSelectAntennaPattern.Where(c => c.SENSOR_ANTENNA.Id, ConditionOperator.Equal, readerSensorAntenna.GetValue(c => c.Id));

                                queryExecuter.Fetch(builderSelectAntennaPattern, readerAntennaPattern =>
                                {
                                    while (readerAntennaPattern.Read())
                                    {
                                        var patt = new AntennaPattern();
                                        patt.DiagA = readerAntennaPattern.GetValue(c => c.DiagA);
                                        patt.DiagH = readerAntennaPattern.GetValue(c => c.DiagH);
                                        patt.DiagV = readerAntennaPattern.GetValue(c => c.DiagV);
                                        if (readerAntennaPattern.GetValue(c => c.Freq) != null) patt.Freq = readerAntennaPattern.GetValue(c => c.Freq).Value;
                                        if (readerAntennaPattern.GetValue(c => c.Gain) != null) patt.Gain = readerAntennaPattern.GetValue(c => c.Gain).Value;
                                        antennaPattern.Add(patt);
                                    }
                                    return true;
                                });
                                val.Antenna.AntennaPatterns = antennaPattern.ToArray();
                            }
                            return true;
                        });


                        val.Equipment = new SensorEquip();
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
                        builderSelectEquipment.Select(c => c.SENSOR.Id);
                        builderSelectEquipment.Select(c => c.TechId);
                        builderSelectEquipment.Select(c => c.TuningStep);
                        builderSelectEquipment.Select(c => c.Type);
                        builderSelectEquipment.Select(c => c.UpperFreq);
                        builderSelectEquipment.Select(c => c.UserType);
                        builderSelectEquipment.Select(c => c.VbwMax);
                        builderSelectEquipment.Select(c => c.VbwMin);
                        builderSelectEquipment.Select(c => c.Version);
                        builderSelectEquipment.Where(c => c.SENSOR.Id, ConditionOperator.Equal, reader.GetValue(c => c.Id));
                        queryExecuter.Fetch(builderSelectEquipment, readerEquipment =>
                        {
                            while (readerEquipment.Read())
                            {
                                val.Equipment.Category = readerEquipment.GetValue(c => c.Category);
                                val.Equipment.Code = readerEquipment.GetValue(c => c.Code);
                                val.Equipment.CustDate1 = readerEquipment.GetValue(c => c.CustData1);
                                val.Equipment.CustNbr1 = readerEquipment.GetValue(c => c.CustNbr1);
                                val.Equipment.CustTxt1 = readerEquipment.GetValue(c => c.CustTxt1);
                                val.Equipment.EquipClass = readerEquipment.GetValue(c => c.EquipClass);
                                val.Equipment.Family = readerEquipment.GetValue(c => c.Family);
                                val.Equipment.FFTPointMax = readerEquipment.GetValue(c => c.FftPointMax);
                                val.Equipment.LowerFreq = readerEquipment.GetValue(c => c.LowerFreq);
                                val.Equipment.Manufacturer = readerEquipment.GetValue(c => c.Manufacturer);
                                val.Equipment.Mobility = readerEquipment.GetValue(c => c.Mobility);
                                val.Equipment.Name = readerEquipment.GetValue(c => c.Name);
                                val.Equipment.OperationMode = readerEquipment.GetValue(c => c.OperationMode);
                                val.Equipment.RBWMax = readerEquipment.GetValue(c => c.RbwMax);
                                val.Equipment.RBWMin = readerEquipment.GetValue(c => c.RbwMin);
                                val.Equipment.RefLeveldBm = readerEquipment.GetValue(c => c.RefLevelDbm);
                                val.Equipment.Remark = readerEquipment.GetValue(c => c.Remark);
                                val.Equipment.TechId = readerEquipment.GetValue(c => c.TechId);
                                val.Equipment.TuningStep = readerEquipment.GetValue(c => c.TuningStep);
                                val.Equipment.Type = readerEquipment.GetValue(c => c.Type);
                                val.Equipment.UpperFreq = readerEquipment.GetValue(c => c.UpperFreq);
                                val.Equipment.UseType = readerEquipment.GetValue(c => c.UserType);
                                val.Equipment.VBWMax = readerEquipment.GetValue(c => c.VbwMax);
                                val.Equipment.VBWMin = readerEquipment.GetValue(c => c.VbwMin);
                                val.Equipment.Version = readerEquipment.GetValue(c => c.Version);




                                var sensitivytes = new List<SensorEquipSensitivity>();
                                var builderSelectSensorSensitivites = this._dataLayer.GetBuilder<MD.ISensorSensitivites>().From();
                                builderSelectSensorSensitivites.Select(c => c.AddLoss);
                                builderSelectSensorSensitivites.Select(c => c.Freq);
                                builderSelectSensorSensitivites.Select(c => c.FreqStability);
                                builderSelectSensorSensitivites.Select(c => c.Id);
                                builderSelectSensorSensitivites.Select(c => c.Ktbf);
                                builderSelectSensorSensitivites.Select(c => c.Noisef);
                                builderSelectSensorSensitivites.Select(c => c.SENSOR_EQUIP.Id);
                                builderSelectSensorSensitivites.Where(c => c.SENSOR_EQUIP.Id, ConditionOperator.Equal, readerEquipment.GetValue(c => c.Id));
                                queryExecuter.Fetch(builderSelectSensorSensitivites, readerSensorSensitivites =>
                                {
                                    while (readerSensorSensitivites.Read())
                                    {
                                        var sens = new SensorEquipSensitivity();
                                        sens.AddLoss = readerSensorSensitivites.GetValue(c => c.AddLoss);
                                        if (readerSensorSensitivites.GetValue(c => c.Freq) != null) sens.Freq = readerSensorSensitivites.GetValue(c => c.Freq).Value;
                                        if (readerSensorSensitivites.GetValue(c => c.FreqStability) != null) sens.FreqStability = readerSensorSensitivites.GetValue(c => c.FreqStability);
                                        if (readerSensorSensitivites.GetValue(c => c.Ktbf) != null) sens.KTBF = readerSensorSensitivites.GetValue(c => c.Ktbf);
                                        if (readerSensorSensitivites.GetValue(c => c.Noisef) != null) sens.NoiseF = readerSensorSensitivites.GetValue(c => c.Noisef);
                                        sensitivytes.Add(sens);
                                    }
                                    return true;
                                });
                                val.Equipment.SensorEquipSensitivities = sensitivytes.ToArray();
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

                        builderSelectsensorLocation.Where(c => c.SENSOR.Id, ConditionOperator.Equal, reader.GetValue(c => c.Id));
                        builderSelectsensorLocation.Where(c => c.Status, ConditionOperator.Equal, Status.A.ToString());
                        queryExecuter.Fetch(builderSelectsensorLocation, readersensorLocation =>
                        {
                            while (readersensorLocation.Read())
                            {
                                var sensLoc = new SensorLocation();
                                sensLoc.ASL = readersensorLocation.GetValue(c => c.Asl);
                                sensLoc.DataCreated = readersensorLocation.GetValue(c => c.DateCreated);
                                sensLoc.DataFrom = readersensorLocation.GetValue(c => c.DateFrom);
                                sensLoc.DataTo = readersensorLocation.GetValue(c => c.DateTo);
                                sensLoc.Lat = readersensorLocation.GetValue(c => c.Lat);
                                sensLoc.Lon = readersensorLocation.GetValue(c => c.Lon);
                                sensLoc.Status = readersensorLocation.GetValue(c => c.Status);
                                sensorLocation.Add(sensLoc);
                            }
                            return true;
                        });
                        val.Locations = sensorLocation.ToArray();


                        var sensorPolygonPoint = new List<SensorPoligonPoint>();
                        var builderSelectSensorPolygonPoint = this._dataLayer.GetBuilder<MD.ISensorPolygon>().From();
                        builderSelectSensorPolygonPoint.Select(c => c.Id);
                        builderSelectSensorPolygonPoint.Select(c => c.Lat);
                        builderSelectSensorPolygonPoint.Select(c => c.Lon);
                        builderSelectSensorPolygonPoint.Where(c => c.SENSOR.Id, ConditionOperator.Equal, reader.GetValue(c => c.Id));

                        queryExecuter.Fetch(builderSelectSensorPolygonPoint, readersensorPolygonPoint =>
                        {
                            while (readersensorPolygonPoint.Read())
                            {
                                var sensLoc = new SensorPoligonPoint();
                                sensLoc.Lon = readersensorPolygonPoint.GetValue(c => c.Lon);
                                sensLoc.Lat = readersensorPolygonPoint.GetValue(c => c.Lat);
                                sensorPolygonPoint.Add(sensLoc);
                            }
                            return true;
                        });
                        val.Poligon = sensorPolygonPoint.ToArray();
                    }
                    return true;
                });
            }
            catch (Exception e)
            {
                this._logger.Exception(Contexts.ThisComponent, e);
            }
            return val;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Sensor[] LoadAllSensors()
        {
            List<Sensor> listSensors = new List<Sensor>();
            try
            {
                this._logger.Info(Contexts.ThisComponent, Categories.Processing, Events.HandlerCallLoadAllSensorsMethod.Text);
                var queryExecuter = this._dataLayer.Executor<SdrnServerDataContext>();
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
                builderSelectSensor.OrderByDesc(c => c.Id);
                queryExecuter.Fetch(builderSelectSensor, reader =>
                {
                    while (reader.Read())
                    {
                        var val = new Sensor();
                        val.Administration = reader.GetValue(c => c.Administration);
                        val.AGL = reader.GetValue(c => c.Agl);
                        val.Azimuth = reader.GetValue(c => c.Azimuth);
                        val.BiuseDate = reader.GetValue(c => c.BiuseDate);
                        val.CreatedBy = reader.GetValue(c => c.CreatedBy);
                        val.CustDate1 = reader.GetValue(c => c.CustData1);
                        val.CustNbr1 = reader.GetValue(c => c.CustNbr1);
                        val.CustTxt1 = reader.GetValue(c => c.CustTxt1);
                        val.DateCreated = reader.GetValue(c => c.DateCreated);
                        val.Elevation = reader.GetValue(c => c.Elevation);
                        val.EouseDate = reader.GetValue(c => c.EouseDate);
                        val.IdSysARGUS = reader.GetValue(c => c.IdSysArgus);
                        val.Name = reader.GetValue(c => c.Name);
                        val.NetworkId = reader.GetValue(c => c.NetworkId);
                        val.OpDays = reader.GetValue(c => c.OpDays).HasValue ? reader.GetValue(c => c.OpDays).ToString() : null;
                        val.OpHHFr = reader.GetValue(c => c.OpHhFr);
                        val.OpHHTo = reader.GetValue(c => c.OpHhTo);
                        val.Remark = reader.GetValue(c => c.Remark);
                        val.RxLoss = reader.GetValue(c => c.RxLoss);
                        val.Status = reader.GetValue(c => c.Status);
                        val.StepMeasTime = reader.GetValue(c => c.StepMeasTime);
                        val.TypeSensor = reader.GetValue(c => c.TypeSensor);
                        val.Id = new SensorIdentifier();
                        val.Id.Value = reader.GetValue(c => c.Id);


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
                        builderSelectSensorAntenna.Select(c => c.SENSOR.Id);
                        builderSelectSensorAntenna.Select(c => c.Slewang);
                        builderSelectSensorAntenna.Select(c => c.TechId);
                        builderSelectSensorAntenna.Select(c => c.UpperFreq);
                        builderSelectSensorAntenna.Select(c => c.UseType);
                        builderSelectSensorAntenna.Select(c => c.VbeamWidth);
                        builderSelectSensorAntenna.Select(c => c.Xpd);
                        builderSelectSensorAntenna.Where(c => c.SENSOR.Id, ConditionOperator.Equal, reader.GetValue(c => c.Id));

                        queryExecuter.Fetch(builderSelectSensorAntenna, readerSensorAntenna =>
                        {
                            while (readerSensorAntenna.Read())
                            {
                                if (readerSensorAntenna.GetValue(c => c.AddLoss) != null) val.Antenna.AddLoss = readerSensorAntenna.GetValue(c => c.AddLoss).Value;
                                val.Antenna.AntClass = readerSensorAntenna.GetValue(c => c.AntClass);
                                AntennaDirectional outAntennaDir;
                                if (Enum.TryParse<AntennaDirectional>(readerSensorAntenna.GetValue(c => c.AntDir) != null ? readerSensorAntenna.GetValue(c => c.AntDir).ToString() : "", out outAntennaDir))
                                {
                                    val.Antenna.AntDir = outAntennaDir;
                                }
                                val.Antenna.Category = readerSensorAntenna.GetValue(c => c.Category);
                                val.Antenna.Code = readerSensorAntenna.GetValue(c => c.Code);
                                DateTime? custDate1 = readerSensorAntenna.GetValue(c => c.CustData1);
                                if (custDate1 != null) val.Antenna.CustDate1 = custDate1;
                                if (readerSensorAntenna.GetValue(c => c.CustNbr1) != null) val.Antenna.CustNbr1 = readerSensorAntenna.GetValue(c => c.CustNbr1).Value;
                                val.Antenna.CustTxt1 = readerSensorAntenna.GetValue(c => c.CustTxt1);
                                if (readerSensorAntenna.GetValue(c => c.GainMax) != null) val.Antenna.GainMax = readerSensorAntenna.GetValue(c => c.GainMax).Value;
                                val.Antenna.GainType = readerSensorAntenna.GetValue(c => c.GainType);
                                val.Antenna.HBeamwidth = readerSensorAntenna.GetValue(c => c.HbeamWidth);
                                val.Antenna.LowerFreq = readerSensorAntenna.GetValue(c => c.LowerFreq);
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
                                val.Antenna.UpperFreq = readerSensorAntenna.GetValue(c => c.UpperFreq);
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
                                builderSelectAntennaPattern.Where(c => c.SENSOR_ANTENNA.Id, ConditionOperator.Equal, readerSensorAntenna.GetValue(c => c.Id));

                                queryExecuter.Fetch(builderSelectAntennaPattern, readerIAntennaPattern =>
                                {
                                    while (readerIAntennaPattern.Read())
                                    {
                                        var patt = new AntennaPattern();
                                        patt.DiagA = readerIAntennaPattern.GetValue(c => c.DiagA);
                                        patt.DiagH = readerIAntennaPattern.GetValue(c => c.DiagH);
                                        patt.DiagV = readerIAntennaPattern.GetValue(c => c.DiagV);
                                        if (readerIAntennaPattern.GetValue(c => c.Freq) != null) patt.Freq = readerIAntennaPattern.GetValue(c => c.Freq).Value;
                                        if (readerIAntennaPattern.GetValue(c => c.Gain) != null) patt.Gain = readerIAntennaPattern.GetValue(c => c.Gain).Value;
                                        antennaPattern.Add(patt);
                                    }
                                    return true;
                                });
                                val.Antenna.AntennaPatterns = antennaPattern.ToArray();
                            }
                            return true;
                        });


                        val.Equipment = new SensorEquip();
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
                        builderSelectEquipment.Select(c => c.SENSOR.Id);
                        builderSelectEquipment.Select(c => c.TechId);
                        builderSelectEquipment.Select(c => c.TuningStep);
                        builderSelectEquipment.Select(c => c.Type);
                        builderSelectEquipment.Select(c => c.UpperFreq);
                        builderSelectEquipment.Select(c => c.UserType);
                        builderSelectEquipment.Select(c => c.VbwMax);
                        builderSelectEquipment.Select(c => c.VbwMin);
                        builderSelectEquipment.Select(c => c.Version);
                        builderSelectEquipment.Where(c => c.SENSOR.Id, ConditionOperator.Equal, reader.GetValue(c => c.Id));
                        queryExecuter.Fetch(builderSelectEquipment, readerEquipment =>
                        {
                            while (readerEquipment.Read())
                            {
                                val.Equipment.Category = readerEquipment.GetValue(c => c.Category);
                                val.Equipment.Code = readerEquipment.GetValue(c => c.Code);
                                val.Equipment.CustDate1 = readerEquipment.GetValue(c => c.CustData1);
                                val.Equipment.CustNbr1 = readerEquipment.GetValue(c => c.CustNbr1);
                                val.Equipment.CustTxt1 = readerEquipment.GetValue(c => c.CustTxt1);
                                val.Equipment.EquipClass = readerEquipment.GetValue(c => c.EquipClass);
                                val.Equipment.Family = readerEquipment.GetValue(c => c.Family);
                                val.Equipment.FFTPointMax = readerEquipment.GetValue(c => c.FftPointMax);
                                val.Equipment.LowerFreq = readerEquipment.GetValue(c => c.LowerFreq);
                                val.Equipment.Manufacturer = readerEquipment.GetValue(c => c.Manufacturer);
                                val.Equipment.Mobility = readerEquipment.GetValue(c => c.Mobility);
                                val.Equipment.Name = readerEquipment.GetValue(c => c.Name);
                                val.Equipment.OperationMode = readerEquipment.GetValue(c => c.OperationMode);
                                val.Equipment.RBWMax = readerEquipment.GetValue(c => c.RbwMax);
                                val.Equipment.RBWMin = readerEquipment.GetValue(c => c.RbwMin);
                                val.Equipment.RefLeveldBm = readerEquipment.GetValue(c => c.RefLevelDbm);
                                val.Equipment.Remark = readerEquipment.GetValue(c => c.Remark);
                                val.Equipment.TechId = readerEquipment.GetValue(c => c.TechId);
                                val.Equipment.TuningStep = readerEquipment.GetValue(c => c.TuningStep);
                                val.Equipment.Type = readerEquipment.GetValue(c => c.Type);
                                val.Equipment.UpperFreq = readerEquipment.GetValue(c => c.UpperFreq);
                                val.Equipment.UseType = readerEquipment.GetValue(c => c.UserType);
                                val.Equipment.VBWMax = readerEquipment.GetValue(c => c.VbwMax);
                                val.Equipment.VBWMin = readerEquipment.GetValue(c => c.VbwMin);
                                val.Equipment.Version = readerEquipment.GetValue(c => c.Version);




                                var sensitivytes = new List<SensorEquipSensitivity>();
                                var builderSelectSensorSensitivites = this._dataLayer.GetBuilder<MD.ISensorSensitivites>().From();
                                builderSelectSensorSensitivites.Select(c => c.AddLoss);
                                builderSelectSensorSensitivites.Select(c => c.Freq);
                                builderSelectSensorSensitivites.Select(c => c.FreqStability);
                                builderSelectSensorSensitivites.Select(c => c.Id);
                                builderSelectSensorSensitivites.Select(c => c.Ktbf);
                                builderSelectSensorSensitivites.Select(c => c.Noisef);
                                builderSelectSensorSensitivites.Select(c => c.SENSOR_EQUIP.Id);
                                builderSelectSensorSensitivites.Where(c => c.SENSOR_EQUIP.Id, ConditionOperator.Equal, readerEquipment.GetValue(c => c.Id));
                                queryExecuter.Fetch(builderSelectSensorSensitivites, readerSensorSensitivites =>
                                {
                                    while (readerSensorSensitivites.Read())
                                    {
                                        var sens = new SensorEquipSensitivity();
                                        sens.AddLoss = readerSensorSensitivites.GetValue(c => c.AddLoss);
                                        if (readerSensorSensitivites.GetValue(c => c.Freq) != null) sens.Freq = readerSensorSensitivites.GetValue(c => c.Freq).Value;
                                        if (readerSensorSensitivites.GetValue(c => c.FreqStability) != null) sens.FreqStability = readerSensorSensitivites.GetValue(c => c.FreqStability);
                                        if (readerSensorSensitivites.GetValue(c => c.Ktbf) != null) sens.KTBF = readerSensorSensitivites.GetValue(c => c.Ktbf);
                                        if (readerSensorSensitivites.GetValue(c => c.Noisef) != null) sens.NoiseF = readerSensorSensitivites.GetValue(c => c.Noisef);
                                        sensitivytes.Add(sens);
                                    }
                                    return true;
                                });
                                val.Equipment.SensorEquipSensitivities = sensitivytes.ToArray();
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

                        builderSelectsensorLocation.Where(c => c.SENSOR.Id, ConditionOperator.Equal, reader.GetValue(c => c.Id));
                        builderSelectsensorLocation.Where(c => c.Status, ConditionOperator.Equal, Status.A.ToString());
                        queryExecuter.Fetch(builderSelectsensorLocation, readersensorLocation =>
                        {
                            while (readersensorLocation.Read())
                            {
                                var sensLoc = new SensorLocation();
                                sensLoc.ASL = readersensorLocation.GetValue(c => c.Asl);
                                sensLoc.DataCreated = readersensorLocation.GetValue(c => c.DateCreated);
                                sensLoc.DataFrom = readersensorLocation.GetValue(c => c.DateFrom);
                                sensLoc.DataTo = readersensorLocation.GetValue(c => c.DateTo);
                                sensLoc.Lat = readersensorLocation.GetValue(c => c.Lat);
                                sensLoc.Lon = readersensorLocation.GetValue(c => c.Lon);
                                sensLoc.Status = readersensorLocation.GetValue(c => c.Status);
                                sensorLocation.Add(sensLoc);
                            }
                            return true;
                        });
                        val.Locations = sensorLocation.ToArray();


                        var sensorPolygonPoint = new List<SensorPoligonPoint>();
                        var builderSelectSensorPolygonPoint = this._dataLayer.GetBuilder<MD.ISensorPolygon>().From();
                        builderSelectSensorPolygonPoint.Select(c => c.Id);
                        builderSelectSensorPolygonPoint.Select(c => c.Lat);
                        builderSelectSensorPolygonPoint.Select(c => c.Lon);
                        builderSelectSensorPolygonPoint.Where(c => c.SENSOR.Id, ConditionOperator.Equal, reader.GetValue(c => c.Id));

                        queryExecuter.Fetch(builderSelectSensorPolygonPoint, readersensorPolygonPoint =>
                        {
                            while (readersensorPolygonPoint.Read())
                            {
                                var sensLoc = new SensorPoligonPoint();
                                sensLoc.Lon = readersensorPolygonPoint.GetValue(c => c.Lon);
                                sensLoc.Lat = readersensorPolygonPoint.GetValue(c => c.Lat);
                                sensorPolygonPoint.Add(sensLoc);
                            }
                            return true;
                        });
                        val.Poligon = sensorPolygonPoint.ToArray();
                        listSensors.Add(val);
                    }
                    return true;
                });
            }
            catch (Exception e)
            {
                this._logger.Exception(Contexts.ThisComponent, e);
            }
            return listSensors.ToArray();
        }
    }
}
