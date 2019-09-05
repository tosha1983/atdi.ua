using Atdi.Contracts.CoreServices.DataLayer;
using Atdi.Contracts.CoreServices.EntityOrm;
using Atdi.Contracts.Sdrn.Server;
using Atdi.DataModels.DataConstraint;
using Atdi.Platform.Logging;
using System;
using MD = Atdi.DataModels.Sdrns.Server.Entities;
using System.Linq;
using Atdi.DataModels.Sdrns.Server;

namespace Atdi.AppUnits.Sdrn.Server.PrimaryHandlers.PipelineHandlers
{
    public class SaveResults 
    {
        private readonly IDataLayer<EntityDataOrm> _dataLayer;
        private readonly ILogger _logger;



        public SaveResults(IDataLayer<EntityDataOrm> dataLayer, ILogger logger)
        {
            this._dataLayer = dataLayer;
            this._logger = logger;
        }

        public CommonOperation DeleteResultFromDB(long MeasTaskId, string status)
        {
            var result = new CommonOperation();
            try
            {
                this._logger.Info(Contexts.ThisComponent, Categories.MessageProcessing, Events.HandlerCallDeleteResultFromDBMethod.Text);
                using (var scope = this._dataLayer.CreateScope<SdrnServerDataContext>())
                {
                    scope.BeginTran();

                    bool isContainResult = false;
                    var builderFromResMeas = this._dataLayer.GetBuilder<MD.IResMeas>().From();
                    builderFromResMeas.Where(c => c.SUBTASK_SENSOR.SUBTASK.MEAS_TASK.Id, ConditionOperator.Equal, MeasTaskId);
                    builderFromResMeas.Select(c => c.Id);
                    scope.Executor.Fetch(builderFromResMeas, reader =>
                    {
                        while (reader.Read())
                        {
                            isContainResult = true;
                            break;
                        }
                    return true;
                    });

                    if (isContainResult == true)
                    {
                        var builderUpdateResMeas = this._dataLayer.GetBuilder<MD.IResMeas>().Update();
                        builderUpdateResMeas.Where(c => c.SUBTASK_SENSOR.SUBTASK.MEAS_TASK.Id, ConditionOperator.Equal, MeasTaskId);
                        builderUpdateResMeas.SetValue(c => c.Status, status);
                        if (scope.Executor.Execute(builderUpdateResMeas) > 0)
                        {
                            result.State = CommonOperationState.Success;
                        }
                        else
                        {
                            result.State = CommonOperationState.Fault;
                        }
                    }
                    else
                    {
                        result.State = CommonOperationState.Success;
                    }
                    scope.Commit();
                }
            }
            catch (Exception e)
            {
                result.State = CommonOperationState.Fault;
                this._logger.Exception(Contexts.ThisComponent, e);
            }
            return result;
        }
      
    }
}


