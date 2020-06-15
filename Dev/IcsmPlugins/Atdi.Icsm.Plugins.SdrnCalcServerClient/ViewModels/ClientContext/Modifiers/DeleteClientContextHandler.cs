using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.DataModels.Sdrn.CalcServer.Entities;
using Atdi.Icsm.Plugins.SdrnCalcServerClient.ViewModels.ClientContext.Events;
using Atdi.Platform.Cqrs;
using Atdi.Platform.Events;

namespace Atdi.Icsm.Plugins.SdrnCalcServerClient.ViewModels.ClientContext.Modifiers
{
    public class DeleteClientContextHandler : ICommandHandler<DeleteClientContext>
    {
        private readonly AppComponentConfig _config;
        private readonly CalcServerDataLayer _dataLayer;
        private readonly IEventBus _eventBus;

        public DeleteClientContextHandler(AppComponentConfig config, CalcServerDataLayer dataLayer, IEventBus eventBus)
        {
            _config = config;
            _dataLayer = dataLayer;
            _eventBus = eventBus;
        }
        public void Handle(DeleteClientContext command)
        {
            var query = _dataLayer.GetBuilder<IClientContext>()
                .Delete()
                .Filter(c => c.Id, command.Id);
            _dataLayer.Executor.Execute(query);

            _eventBus.Send(new OnDeletedClientContext { ClientContextId = command.Id });
        }
    }
}
