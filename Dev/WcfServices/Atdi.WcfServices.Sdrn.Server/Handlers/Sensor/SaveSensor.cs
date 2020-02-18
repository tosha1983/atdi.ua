using System.Collections.Generic;
using Atdi.Contracts.CoreServices.DataLayer;
using Atdi.Contracts.CoreServices.EntityOrm;
using Atdi.Contracts.Sdrn.Server;
using Atdi.DataModels.DataConstraint;
using Atdi.Platform.Logging;
using System;
using MD = Atdi.DataModels.Sdrns.Server.Entities;
using Atdi.Contracts.WcfServices.Sdrn.Server;


namespace Atdi.WcfServices.Sdrn.Server
{
    public class SaveSensor
    {
        private readonly IDataLayer<EntityDataOrm> _dataLayer;
        private readonly ILogger _logger;



        public SaveSensor(IDataLayer<EntityDataOrm> dataLayer, ILogger logger)
        {
            this._dataLayer = dataLayer;
            this._logger = logger;
        }
        public bool UpdateSensorTitle(long id, string title)
        {
            var isSuccess = true;
            this._logger.Info(Contexts.ThisComponent, Categories.Processing, Events.HandlerCallDeleteResultFromDBMethod.Text);
            try
            {
                using (var scope = this._dataLayer.CreateScope<SdrnServerDataContext>())
                {
                    scope.BeginTran();

                    var builderUpdateSensor = this._dataLayer.GetBuilder<MD.ISensor>().Update();
                    builderUpdateSensor.Where(c => c.Id, ConditionOperator.In, id);
                    builderUpdateSensor.SetValue(c => c.Title, title);
                    if (scope.Executor.Execute(builderUpdateSensor) > 0)
                        isSuccess = true;

                    scope.Commit();
                }
            }
            catch (Exception e)
            {
                isSuccess = false;
                this._logger.Exception(Contexts.ThisComponent, e);
            }
            return isSuccess;
        }
    }
}


