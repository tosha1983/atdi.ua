using System.Collections.Generic;
using Atdi.Contracts.CoreServices.DataLayer;
using Atdi.Contracts.CoreServices.EntityOrm;
using Atdi.Contracts.Sdrn.Server;
using Atdi.DataModels.DataConstraint;
using Atdi.Platform.Logging;
using System;
using MD = Atdi.DataModels.Sdrns.Server.Entities;
using Atdi.DataModels.Sdrns.Server;


namespace Atdi.AppUnits.Sdrn.Server.PrimaryHandlers.PipelineHandlers
{
    /// <summary>
    /// Загрузка сведений по сенсорам
    /// </summary>
    public class LoadSensor 
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

        public long? LoadSensorId(long Id, out string SensorName, out string SensorTechId)
        {
            long? valIdentifier = null;
            var sensorName = "";
            var sensorTechId = "";
            try
            {
                this._logger.Info(Contexts.ThisComponent, Categories.MessageProcessing, Events.HandlerCallLoadObjectSensorMethod.Text);
                var queryExecuter = this._dataLayer.Executor<SdrnServerDataContext>();
                var builderSelectSensor = this._dataLayer.GetBuilder<MD.ISensor>().From();
                builderSelectSensor.Select(c => c.Id);
                builderSelectSensor.Select(c => c.Name);
                builderSelectSensor.Select(c => c.TechId);
                builderSelectSensor.Where(c => c.Id, ConditionOperator.Equal, Id);
                builderSelectSensor.OrderByDesc(c => c.Id);
                queryExecuter.Fetch(builderSelectSensor, reader =>
                {
                    while (reader.Read())
                    {
                        valIdentifier = reader.GetValue(c => c.Id);
                        sensorName = reader.GetValue(c => c.Name);
                        sensorTechId = reader.GetValue(c => c.TechId);
                    }
                    return true;
                });
            }
            catch (Exception e)
            {
                this._logger.Exception(Contexts.ThisComponent, e);
            }
            SensorName = sensorName;
            SensorTechId = sensorTechId;
            return valIdentifier;
        }

        public long[] LoadAllSensorIds()
        {
            var listAllSensors = new List<long>();
            try
            {
                this._logger.Info(Contexts.ThisComponent, Categories.MessageProcessing, Events.HandlerCallLoadObjectSensorMethod.Text);
                var queryExecuter = this._dataLayer.Executor<SdrnServerDataContext>();
                var builderSelectSensor = this._dataLayer.GetBuilder<MD.ISensor>().From();
                builderSelectSensor.Select(c => c.Id);
                builderSelectSensor.Select(c => c.Name);
                builderSelectSensor.Select(c => c.TechId);
                builderSelectSensor.OrderByDesc(c => c.Id);
                queryExecuter.Fetch(builderSelectSensor, reader =>
                {
                    while (reader.Read())
                    {
                        long valIdentifier = reader.GetValue(c => c.Id);
                        listAllSensors.Add(valIdentifier);
                    }
                    return true;
                });
            }
            catch (Exception e)
            {
                this._logger.Exception(Contexts.ThisComponent, e);
            }
          
            return listAllSensors.ToArray();
        }
    }
}


