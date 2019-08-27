using System.Collections.Generic;
using Atdi.Contracts.CoreServices.DataLayer;
using Atdi.Contracts.CoreServices.EntityOrm;
using Atdi.Contracts.Sdrn.Server;
using Atdi.DataModels.DataConstraint;
using Atdi.Platform.Logging;
using System;
using MD = Atdi.DataModels.Sdrns.Server.Entities;
using Atdi.DataModels.Sdrns.Server;

namespace Atdi.AppUnits.Sdrn.AggregationServer.PrimaryHandlers.Handlers
{
    /// <summary>
    /// Сохранение в БД AggregationServer сведений о сенсорах
    /// </summary>
    public class SaveSensor
    {
        private readonly IDataLayer<EntityDataOrm> _dataLayer;
        private readonly ILogger _logger;



        public SaveSensor(IDataLayer<EntityDataOrm> dataLayer, ILogger logger)
        {
            this._dataLayer = dataLayer;
            this._logger = logger;
        }

        public bool UpdateFieldAggregationServerInstanceInSensor(string sensorName, string equipmentTechId, string aggregationServerInstance)
        {
            var resultValue = false;
            try
            {
                using (var scope = this._dataLayer.CreateScope<SdrnServerDataContext>())
                {
                    scope.BeginTran();

                    long? sensorId = null;
                    var builderSelectSensor = this._dataLayer.GetBuilder<MD.ISensor>().From();
                    builderSelectSensor.Select(c => c.Id);
                    builderSelectSensor.Where(c => c.Name, ConditionOperator.Equal, sensorName);
                    builderSelectSensor.Where(c => c.TechId, ConditionOperator.Equal, equipmentTechId);
                     scope.Executor.Fetch(builderSelectSensor, reader =>
                     {
                         while (reader.Read())
                         {
                             sensorId = reader.GetValue(c => c.Id);
                         }
                         return true;
                     });

                    if (sensorId != null)
                    {
                        bool isAlreadyRecord = false;
                        var builderSelectLinkSensor = this._dataLayer.GetBuilder<MD.ILinkAggregationSensor>().From();
                        builderSelectLinkSensor.Select(c => c.Id);
                        builderSelectLinkSensor.Select(c => c.AggregationServerName);
                        builderSelectLinkSensor.Select(c => c.SENSOR.Id);
                        builderSelectLinkSensor.Where(c => c.SENSOR.Id, ConditionOperator.Equal, sensorId.Value);
                        scope.Executor.Fetch(builderSelectLinkSensor, reader =>
                        {
                            while (reader.Read())
                            {
                                isAlreadyRecord = true;
                                break;
                            }
                            return true;
                        });

                        if (isAlreadyRecord == true)
                        {
                            var builderUpdateLinkSensor = this._dataLayer.GetBuilder<MD.ILinkAggregationSensor>().Update();
                            builderUpdateLinkSensor.SetValue(c => c.AggregationServerName, aggregationServerInstance);
                            builderUpdateLinkSensor.Where(c => c.SENSOR.Id, ConditionOperator.Equal, sensorId.Value);
                            scope.Executor.Execute(builderUpdateLinkSensor);
                        }
                        else
                        {
                            var builderUpdateLinkSensor = this._dataLayer.GetBuilder<MD.ILinkAggregationSensor>().Insert();
                            builderUpdateLinkSensor.SetValue(c => c.AggregationServerName, aggregationServerInstance);
                            builderUpdateLinkSensor.SetValue(c => c.SENSOR.Id, sensorId.Value);
                            var linkSensorId = scope.Executor.Execute<MD.ILinkAggregationSensor_PK>(builderUpdateLinkSensor);
                        }
                    }
                    scope.Commit();
                    resultValue = true;
                }
            }
            catch (Exception ex)
            {
                resultValue = false;
                this._logger.Exception(Contexts.ThisComponent, Categories.UpdateFieldAggregationServerInstanceInSensor, ex);
            }
            return resultValue;
        }

       
    }
}


