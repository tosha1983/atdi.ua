using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.DataModels.Sdrn.CalcServer.Entities;
using Atdi.DataModels.Sdrn.CalcServer.Entities.Tasks;
using Atdi.Icsm.Plugins.GE06Calc.ViewModels.GE06Settings.Events;
using Atdi.Platform.Cqrs;
using Atdi.Platform.Events;

namespace Atdi.Icsm.Plugins.GE06Calc.ViewModels.GE06Settings.Modifiers
{
    public class PrepareClientContextHandler : ICommandHandler<PrepareClientContext>
    {
        private readonly AppComponentConfig _config;
        private readonly CalcServerDataLayer _dataLayer;
        private readonly IEventBus _eventBus;

        public PrepareClientContextHandler(AppComponentConfig config, CalcServerDataLayer dataLayer, IEventBus eventBus)
        {
            _config = config;
            _dataLayer = dataLayer;
            _eventBus = eventBus;
        }
        public void Handle(PrepareClientContext command)
        {
            var query = _dataLayer.GetBuilder<IClientContext>()
              .Update()
              .SetValue(c => c.StatusCode, (byte)ClientContextStatusCode.Prepared)
              .SetValue(c => c.StatusName, ClientContextStatusCode.Prepared.ToString())
              .Filter(c => c.Id, command.ContextId);
            _dataLayer.Executor.Execute(query);

            _eventBus.Send(new OnPreparedClientContext { ClientContextId = command.ContextId });
        }
    }
}
