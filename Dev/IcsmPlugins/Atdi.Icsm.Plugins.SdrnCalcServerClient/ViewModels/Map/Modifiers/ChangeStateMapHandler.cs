using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.DataModels.Sdrn.CalcServer.Entities;
using Atdi.Icsm.Plugins.SdrnCalcServerClient.ViewModels.Map.Events;
using Atdi.Platform.Cqrs;
using Atdi.Platform.Events;

namespace Atdi.Icsm.Plugins.SdrnCalcServerClient.ViewModels.Map.Modifiers
{
    public class ChangeStateMapHandler : ICommandHandler<ChangeStateMap>
    {
        private readonly AppComponentConfig _config;
        private readonly CalcServerDataLayer _dataLayer;
        private readonly IEventBus _eventBus;

        public ChangeStateMapHandler(AppComponentConfig config, CalcServerDataLayer dataLayer, IEventBus eventBus)
        {
            _config = config;
            _dataLayer = dataLayer;
            _eventBus = eventBus;
        }
        public void Handle(ChangeStateMap command)
        {
            var query = _dataLayer.GetBuilder<IProjectMap>()
                .Update()
                .SetValue(c => c.StatusCode, (byte)command.StatusCode)
                .SetValue(c => c.StatusName, Enum.GetValues(typeof(ProjectMapStatusCode)).GetValue(command.StatusCode).ToString())
                .Filter(c => c.Id, command.Id);
            _dataLayer.Executor.Execute(query);

            _eventBus.Send(new OnEditedMap { MapId = command.Id });
        }
    }
}
