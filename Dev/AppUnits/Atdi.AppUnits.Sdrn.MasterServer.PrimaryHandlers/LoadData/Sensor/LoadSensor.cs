using System.Collections.Generic;
using Atdi.Contracts.CoreServices.DataLayer;
using Atdi.Contracts.CoreServices.EntityOrm;
using Atdi.Contracts.Sdrn.Server;
using Atdi.DataModels.DataConstraint;
using Atdi.Platform.Logging;
using System;
using MD = Atdi.DataModels.Sdrns.Server.Entities;
using Atdi.DataModels.Sdrns.Server;


namespace Atdi.AppUnits.Sdrn.MasterServer.LoadData
{
    /// <summary>
    /// Загрузка сведений по сенсору с БД MasterServer
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


        public bool GetAggregationServerBySensorId(long sensorId, out string name, out string techId, out string aggregationServerInstance)
        {
            var isClusterMode = false;
            string aggregationServerInstanceTemp = null;
            string nameTemp = null;
            string techIdTemp = null;
            var queryExecuter = this._dataLayer.Executor<SdrnServerDataContext>();
            var builderAggregationSensor = this._dataLayer.GetBuilder<MD.ILinkAggregationSensor>().From();
            builderAggregationSensor.Select(c => c.AggregationServerName, c => c.Id, c => c.SENSOR.Name, c => c.SENSOR.TechId);
            builderAggregationSensor.Where(c => c.SENSOR.Id, ConditionOperator.Equal, sensorId);
            queryExecuter.Fetch(builderAggregationSensor, readerAggregationSensor =>
            {
                while (readerAggregationSensor.Read())
                {
                    aggregationServerInstanceTemp = readerAggregationSensor.GetValue(c => c.AggregationServerName);
                    nameTemp = readerAggregationSensor.GetValue(c => c.SENSOR.Name);
                    techIdTemp = readerAggregationSensor.GetValue(c => c.SENSOR.TechId);
                    if (!string.IsNullOrEmpty(aggregationServerInstanceTemp))
                    {
                        isClusterMode = true;
                    }
                    break;
                }
                return true;
            });
            aggregationServerInstance = aggregationServerInstanceTemp;
            name = nameTemp;
            techId = techIdTemp;
            return isClusterMode;
        }

       
    }
}


