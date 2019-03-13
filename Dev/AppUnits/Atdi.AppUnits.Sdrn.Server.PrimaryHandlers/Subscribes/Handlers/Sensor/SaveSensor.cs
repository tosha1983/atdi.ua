using Atdi.Contracts.Api.EventSystem;
using Atdi.Platform.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.Contracts.Sdrn.Server;
using MSG = Atdi.DataModels.Sdrns.BusMessages;
using DEV = Atdi.DataModels.Sdrns.Device;
using Atdi.Contracts.CoreServices.DataLayer;
using Atdi.Contracts.CoreServices.EntityOrm;
using Atdi.DataModels.DataConstraint;
using MD = Atdi.DataModels.Sdrns.Server.Entities;
using Atdi.Contracts.WcfServices.Sdrn.Server;




namespace Atdi.AppUnits.Sdrn.Server.PrimaryHandlers.Subscribes
{

    public class SaveSensor
    {
        private readonly ILogger _logger;
        private readonly IDataLayer<EntityDataOrm> _dataLayer;

        public SaveSensor(IDataLayer<EntityDataOrm> dataLayer,  ILogger logger)
        {
            this._logger = logger;
            this._dataLayer = dataLayer;
        }


        public bool UpdateStatus(int id, string status)
        {
            bool isSuccess = false;
            var query = this._dataLayer.GetBuilder<MD.ISensor>()
                .Update()
                .SetValue(c => c.Status, status)
                .SetValue(c => c.LastActivity, DateTime.Now)
                .Where(c => c.Id, ConditionOperator.Equal, id);

            var cntUpdateRec = this._dataLayer.Executor<SdrnServerDataContext>()
            .Execute(query);

            if (cntUpdateRec > 0)
            {
                isSuccess = true;
            }
            return isSuccess;
        }
    }
}

