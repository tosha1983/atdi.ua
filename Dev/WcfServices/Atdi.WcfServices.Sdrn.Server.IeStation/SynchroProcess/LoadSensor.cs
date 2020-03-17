using System.Collections.Generic;
using Atdi.Contracts.CoreServices.DataLayer;
using Atdi.Contracts.CoreServices.EntityOrm;
using Atdi.Contracts.Sdrn.Server;
using Atdi.DataModels.DataConstraint;
using Atdi.Platform.Logging;
using System;
using MD = Atdi.DataModels.Sdrns.Server.Entities;
using Atdi.Contracts.WcfServices.Sdrn.Server.IeStation;


namespace Atdi.WcfServices.Sdrn.Server.IeStation
{
    public class LoadSensor 
    {
        private readonly IDataLayer<EntityDataOrm> _dataLayer;
        private readonly ILogger _logger;



        public LoadSensor(IDataLayer<EntityDataOrm> dataLayer, ILogger logger)
        {
            this._dataLayer = dataLayer;
            this._logger = logger;
        }

        /// <summary>
        /// Извлечение сведений по сенсору
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Sensor LoadBaseDateSensor(long id)
        {
            var val = new Sensor();
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
                builderSelectSensor.Select(c => c.Title);
                builderSelectSensor.Select(c => c.StepMeasTime);
                builderSelectSensor.Select(c => c.TypeSensor);
                builderSelectSensor.Where(c => c.Id, ConditionOperator.Equal, id);
                builderSelectSensor.OrderByDesc(c => c.Id);
                queryExecuter.Fetch(builderSelectSensor, reader =>
                {
                    while (reader.Read())
                    {

                        val.Name = reader.GetValue(c => c.Name);
                        val.Id = new SensorIdentifier();
                        val.Id.Value = reader.GetValue(c => c.Id);
                        val.Title = reader.GetValue(c => c.Title);

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

                        builderSelectsensorLocation.Where(c => c.SENSOR.Id, ConditionOperator.Equal, id);
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
                        builderSelectSensorPolygonPoint.Where(c => c.SENSOR.Id, ConditionOperator.Equal, id);

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
    }
}


