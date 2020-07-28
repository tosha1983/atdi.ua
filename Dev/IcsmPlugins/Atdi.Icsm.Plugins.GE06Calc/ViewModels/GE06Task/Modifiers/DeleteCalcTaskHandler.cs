using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.DataModels.Api.EntityOrm.WebClient;
using Atdi.DataModels.Sdrn.CalcServer.Entities;
using Atdi.DataModels.Sdrn.CalcServer.Entities.Tasks;
using Atdi.Icsm.Plugins.GE06Calc.ViewModels.GE06Task.Events;
using Atdi.Platform.Cqrs;
using Atdi.Platform.Events;
using Atdi.Platform.Logging;

namespace Atdi.Icsm.Plugins.GE06Calc.ViewModels.GE06Task.Modifiers
{
    public class DeleteCalcTaskHandler : ICommandHandler<DeleteCalcTask>
    {
        private readonly AppComponentConfig _config;
        private readonly CalcServerDataLayer _dataLayer;
        private readonly IEventBus _eventBus;
        private readonly ILogger _logger;

        public DeleteCalcTaskHandler(AppComponentConfig config, CalcServerDataLayer dataLayer, IEventBus eventBus, ILogger logger)
        {
            _config = config;
            _dataLayer = dataLayer;
            _eventBus = eventBus;
            _logger = logger;
        }
        public void Handle(DeleteCalcTask command)
        {
            try
            {
                var query = _dataLayer.GetBuilder<ICalcTask>()
                    .Delete()
                    .Filter(c => c.Id, command.Id);
                _dataLayer.Executor.Execute(query);

                _eventBus.Send(new OnDeletedCalcTask { CalcTaskId = command.Id });
            }
            catch (EntityOrmWebApiException e)
            {
                if (e.ServerException != null)
                {
                    _logger.Exception((EventContext)"ICommandHandler", (EventCategory)"Execute", new Exception($"ExceptionMessage: {e.ServerException.ExceptionMessage}; ExceptionType: {e.ServerException.ExceptionType}; InnerException: {e.ServerException.InnerException}; Message: {e.ServerException.Message}!"));
                }
            }
            catch (Exception e)
            {
                _logger.Exception((EventContext)"ICommandHandler", (EventCategory)"Execute", e);
                throw;
            }
        }
    }
}
