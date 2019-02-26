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


namespace Atdi.CoreServices.Device.EntityOrm
{ 
    public sealed class RepositorySensors : IRepository<Sensor>
    {
        private readonly IDataLayer<EntityDataOrm> _dataLayer;
        private readonly ILogger _logger;


        public RepositorySensors(IDataLayer<EntityDataOrm> dataLayer, ILogger logger)
        {
            this._dataLayer = dataLayer;
            this._logger = logger;
        }

        public int? Create(Sensor item)
        {
            throw new NotImplementedException();
        }

        public bool Delete(int id)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }


     
        public Sensor LoadObject(int id)
        {
            throw new NotImplementedException();
        }

        public bool Update(Sensor item)
        {
            throw new NotImplementedException();
        }

        Sensor[] IRepository<Sensor>.LoadAllObjects()
        {
            var listSensors = new List<Sensor>();
            var val = new Sensor();
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
            listSensors.Add(val);
            return listSensors.ToArray();
        }
    }
}


