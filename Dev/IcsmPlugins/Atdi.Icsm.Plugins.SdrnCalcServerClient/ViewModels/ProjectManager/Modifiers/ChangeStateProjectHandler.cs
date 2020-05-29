using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.DataModels.Sdrn.CalcServer.Entities;
using Atdi.Icsm.Plugins.SdrnCalcServerClient.ViewModels.ProjectManager.Events;
using Atdi.Platform.Cqrs;
using Atdi.Platform.Events;


namespace Atdi.Icsm.Plugins.SdrnCalcServerClient.ViewModels.ProjectManager.Modifiers
{
    public class ChangeStateProjectHandler : ICommandHandler<ChangeStateProject>
    {
        private readonly AppComponentConfig _config;
        private readonly CalcServerDataLayer _dataLayer;
        private readonly IEventBus _eventBus;

        public ChangeStateProjectHandler(AppComponentConfig config, CalcServerDataLayer dataLayer, IEventBus eventBus)
        {
            _config = config;
            _dataLayer = dataLayer;
            _eventBus = eventBus;
        }
        public void Handle(ChangeStateProject command)
        {
            var query = _dataLayer.GetBuilder<IProject>()
                .Update()
                .SetValue(c => c.StatusCode, (byte)command.StatusCode)
                .SetValue(c => c.StatusName, Enum.GetValues(typeof(ProjectStatusCode)).GetValue(command.StatusCode).ToString())
                .Filter(c => c.Id, command.Id);
            _dataLayer.Executor.Execute(query);

            _eventBus.Send(new OnEditedProject { ProjectId = command.Id });
        }
    }
}
