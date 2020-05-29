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
    public class CreateClientContextHandler : ICommandHandler<CreateClientContext>
    {
        private readonly AppComponentConfig _config;
        private readonly CalcServerDataLayer _dataLayer;
        private readonly IEventBus _eventBus;

        public CreateClientContextHandler(AppComponentConfig config, CalcServerDataLayer dataLayer, IEventBus eventBus)
        {
            _config = config;
            _dataLayer = dataLayer;
            _eventBus = eventBus;
        }
        public void Handle(CreateClientContext command)
        {
            var query = _dataLayer.GetBuilder<IClientContext>()
                .Create()
                .SetValue(c => c.PROJECT.Id, command.ProjectId)
                .SetValue(c => c.OwnerContextId, command.OwnerId)
                .SetValue(c => c.OwnerInstance, _config.Instance)
                .SetValue(c => c.Name, command.Name)
                .SetValue(c => c.Note, command.Note)
                .SetValue(c => c.TypeCode, (byte)command.TypeCode)
                .SetValue(c => c.TypeName, Enum.GetValues(typeof(ClientContextTypeCode)).GetValue(command.TypeCode).ToString())
                .SetValue(c => c.StatusCode, (byte)ClientContextStatusCode.Created)
                .SetValue(c => c.StatusName, "Created");

            var contextPk = _dataLayer.Executor.Execute<IClientContext_PK>(query);

            _eventBus.Send(new OnCreatedClientContext { ClientContextId = contextPk.Id });
        }
    }
}
