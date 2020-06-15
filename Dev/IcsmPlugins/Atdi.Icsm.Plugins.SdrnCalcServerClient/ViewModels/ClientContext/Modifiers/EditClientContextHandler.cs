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
    public class EditClientContextHandler : ICommandHandler<EditClientContext>
    {
        private readonly AppComponentConfig _config;
        private readonly CalcServerDataLayer _dataLayer;
        private readonly IEventBus _eventBus;

        public EditClientContextHandler(AppComponentConfig config, CalcServerDataLayer dataLayer, IEventBus eventBus)
        {
            _config = config;
            _dataLayer = dataLayer;
            _eventBus = eventBus;
        }
        public void Handle(EditClientContext command)
        {
            var query = _dataLayer.GetBuilder<IClientContext>()
                .Update()
                .SetValue(c => c.Name, command.Name)
                .SetValue(c => c.Note, command.Note)
                .SetValue(c => c.TypeCode, (byte)command.TypeCode)
                .SetValue(c => c.TypeName, Enum.GetValues(typeof(ClientContextTypeCode)).GetValue(command.TypeCode).ToString())
                .Filter(c => c.Id, command.Id);
            _dataLayer.Executor.Execute(query);

            _eventBus.Send(new OnEditedClientContext { ClientContextId = command.Id });
        }
    }
}
