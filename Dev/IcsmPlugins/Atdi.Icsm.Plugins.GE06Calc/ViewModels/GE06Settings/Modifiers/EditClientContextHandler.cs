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
    public class EditClientContextHandler : ICommandHandler<EditClientContext>
    {
        private readonly AppComponentConfig _config;
        private readonly CalcServerDataLayer _dataLayer;
        private readonly IEventBus _eventBus;
        private readonly ILogger _logger;

        public EditClientContextHandler(AppComponentConfig config, CalcServerDataLayer dataLayer, IEventBus eventBus, ILogger logger)
        {
            _config = config;
            _dataLayer = dataLayer;
            _eventBus = eventBus;
            _logger = logger;
        }
        public void Handle(EditClientContext command)
        {
            try
            { 
                var query = _dataLayer.GetBuilder<IClientContext>()
                    .Update()
                    .SetValue(c => c.Name, command.Name)
                    .SetValue(c => c.Note, command.Note)
                    .Filter(c => c.Id, command.Id);
                _dataLayer.Executor.Execute(query);

                if (command.ActiveContext)
                {
                    Properties.Settings.Default.ActiveContext = command.Id;
                    Properties.Settings.Default.Save();
                }
                else
                {
                    if (Properties.Settings.Default.ActiveContext == command.Id)
                    {
                        Properties.Settings.Default.ActiveContext = 0;
                        Properties.Settings.Default.Save();
                    }
                }

                _eventBus.Send(new OnEditedClientContext { ClientContextId = command.Id });
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
