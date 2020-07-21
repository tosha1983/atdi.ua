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
    public class RunCalcTaskHandler : ICommandHandler<RunCalcTask>
    {
        private readonly AppComponentConfig _config;
        private readonly CalcServerDataLayer _dataLayer;
        private readonly IEventBus _eventBus;
        private readonly ILogger _logger;

        public RunCalcTaskHandler(AppComponentConfig config, CalcServerDataLayer dataLayer, IEventBus eventBus, ILogger logger)
        {
            _config = config;
            _dataLayer = dataLayer;
            _eventBus = eventBus;
            _logger = logger;
        }
        public void Handle(RunCalcTask command)
        {
            try
            {
                var updQuery = _dataLayer.GetBuilder<ICalcTask>()
                  .Update()
                  .SetValue(c => c.StatusCode, (byte)CalcTaskStatusCode.Available)
                  .SetValue(c => c.StatusName, CalcTaskStatusCode.Available.ToString())
                  .SetValue(c => c.StatusNote, "The task was made available")
                  .Filter(c => c.Id, command.Id);

                if (_dataLayer.Executor.Execute(updQuery) > 0)
                {
                    var insQuery = _dataLayer.GetBuilder<ICalcResult>()
                        .Create()
                        .SetValue(c => c.CreatedDate, DateTimeOffset.Now)
                        .SetValue(c => c.TASK.Id, command.Id)
                        .SetValue(c => c.CallerInstance, _config.Instance)
                        .SetValue(c => c.CallerResultId, Guid.NewGuid())
                        .SetValue(c => c.StatusCode, (byte)CalcResultStatusCode.Created)
                        .SetValue(c => c.StatusName, CalcResultStatusCode.Created.ToString())
                        .SetValue(c => c.StatusNote, "The result was created by the client");
                    var resultPk = _dataLayer.Executor.Execute<ICalcResult_PK>(insQuery);

                    if (resultPk != null)
                    {
                        var updCalcResultQuery = _dataLayer.GetBuilder<ICalcResult>()
                            .Update()
                            .SetValue(c => c.StatusCode, (byte)CalcResultStatusCode.Pending)
                            .SetValue(c => c.StatusName, CalcResultStatusCode.Pending.ToString())
                            .SetValue(c => c.StatusNote, "The result was ran by the client")
                            .Filter(c => c.CallerInstance, _config.Instance)
                            .Filter(c => c.TASK.Id, command.Id)
                            .Filter(c => c.Id, resultPk.Id);
                        var count = _dataLayer.Executor.Execute(updCalcResultQuery);
                    }

                    _eventBus.Send(new OnRunedCalcTask { Id = command.Id });
                }
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
