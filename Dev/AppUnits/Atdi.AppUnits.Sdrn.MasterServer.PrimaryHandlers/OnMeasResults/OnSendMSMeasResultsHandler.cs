using Atdi.Contracts.Api.DataBus;
using Atdi.DataModels.Api.DataBus;
using Atdi.DataModels.Sdrns.Server;
using Atdi.Platform.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.Platform.Workflows;
using SdrnsServer = Atdi.DataModels.Sdrns.Server;
using Atdi.DataModels.Sdrns.Device;
using Atdi.Contracts.CoreServices.DataLayer;
using Atdi.Contracts.CoreServices.EntityOrm;
using Atdi.AppUnits.Sdrn.MasterServer.PrimaryHandlers;
using MD = Atdi.DataModels.Sdrns.Server.Entities;
using Atdi.Contracts.Sdrn.Server;

namespace Atdi.AppUnits.Sdrn.MasterServer.PrimaryHandlers
{
    public class OnSendMSMeasResultsHandler : IMessageHandler<SendMeasResultMSToMasterServer, MeasResultContainer>
    {
        private readonly ILogger _logger;
        private readonly IDataLayer<EntityDataOrm> _dataLayer;
        public OnSendMSMeasResultsHandler(IDataLayer<EntityDataOrm> dataLayer, ILogger logger)
        {
            this._logger = logger;
            this._dataLayer = dataLayer;
        }
        public void Handle(IIncomingEnvelope<SendMeasResultMSToMasterServer, MeasResultContainer> envelope, IHandlingResult result)
        {
            using (this._logger.StartTrace(Contexts.ThisComponent, Categories.OnSendMSMeasResultsHandler, this))
            {
                var deliveryObject = envelope.DeliveryObject;
                SaveMeasResult(deliveryObject.MeasResult);
                result.Status = MessageHandlingStatus.Confirmed;
            }
        }
        private bool SaveMeasResult(MeasResults measResult)
        {
            try
            {
                if (measResult == null)
                    return true;

                using (var scope = this._dataLayer.CreateScope<SdrnServerDataContext>())
                {
                    var builderInsertIResMeas = this._dataLayer.GetBuilder<MD.IResMeas>().Insert();
                    builderInsertIResMeas.SetValue(c => c.MeasResultSID, measResult.ResultId);
                    builderInsertIResMeas.SetValue(c => c.TimeMeas, measResult.Measured);
                    builderInsertIResMeas.SetValue(c => c.Status, measResult.Status);
                    builderInsertIResMeas.SetValue(c => c.StartTime, measResult.StartTime);
                    builderInsertIResMeas.SetValue(c => c.StopTime, measResult.StopTime);
                    builderInsertIResMeas.SetValue(c => c.ScansNumber, measResult.ScansNumber);
                    builderInsertIResMeas.SetValue(c => c.TypeMeasurements, measResult.Measurement.ToString());
                    builderInsertIResMeas.SetValue(c => c.SUBTASK_SENSOR.Id, long.TryParse(measResult.TaskId, out long subMeasTaskSensorId) ? subMeasTaskSensorId : 0);
                    var resMeas = scope.Executor.Execute<MD.IResMeas_PK>(builderInsertIResMeas);
                    if (resMeas.Id > 0)
                    {

                    }
                }
                return true;
            }
            catch (Exception exp)
            {
                _logger.Exception(Contexts.ThisComponent, exp);
                return false;
            }
        }
    }
}
