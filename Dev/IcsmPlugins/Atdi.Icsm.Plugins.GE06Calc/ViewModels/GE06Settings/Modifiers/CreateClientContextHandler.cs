using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.DataModels.Api.EntityOrm.WebClient;
using Atdi.DataModels.Sdrn.CalcServer.Entities;
using Atdi.DataModels.Sdrn.CalcServer.Entities.Tasks;
using Atdi.Icsm.Plugins.GE06Calc.ViewModels.GE06Settings.Events;
using Atdi.Platform.Cqrs;
using Atdi.Platform.Events;
using Atdi.Platform.Logging;

namespace Atdi.Icsm.Plugins.GE06Calc.ViewModels.GE06Settings.Modifiers
{
    public class CreateClientContextHandler : ICommandHandler<CreateClientContext>
    {
        private readonly AppComponentConfig _config;
        private readonly CalcServerDataLayer _dataLayer;
        private readonly IEventBus _eventBus;
        private readonly ILogger _logger;

        public CreateClientContextHandler(AppComponentConfig config, CalcServerDataLayer dataLayer, IEventBus eventBus, ILogger logger)
        {
            _config = config;
            _dataLayer = dataLayer;
            _eventBus = eventBus;
            _logger = logger;
        }
        public void Handle(CreateClientContext command)
        {
            try
            {
                var query = _dataLayer.GetBuilder<IClientContext>()
                    .Create()
                    .SetValue(c => c.PROJECT.Id, command.ProjectId)
                    .SetValue(c => c.BASE_CONTEXT.Id, command.BaseContextId)
                    .SetValue(c => c.OwnerContextId, command.OwnerId)
                    .SetValue(c => c.OwnerInstance, _config.Instance)
                    .SetValue(c => c.Name, command.Name)
                    .SetValue(c => c.Note, command.Note)
                    .SetValue(c => c.TypeCode, (byte)ClientContextTypeCode.Client)
                    .SetValue(c => c.TypeName, "Client")
                    .SetValue(c => c.StatusCode, (byte)ClientContextStatusCode.Created)
                    .SetValue(c => c.StatusName, "Created");

                var contextPk = _dataLayer.Executor.Execute<IClientContext_PK>(query);

                if (command.ActiveContext)
                {
                    Properties.Settings.Default.ActiveContext = contextPk.Id;
                    Properties.Settings.Default.Save();
                }

                _eventBus.Send(new OnCreatedClientContext { ClientContextId = contextPk.Id });
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
