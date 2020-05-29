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
    public class EditProjectHandler : ICommandHandler<EditProject>
    {
        private readonly AppComponentConfig _config;
        private readonly CalcServerDataLayer _dataLayer;
        private readonly IEventBus _eventBus;

        public EditProjectHandler(AppComponentConfig config, CalcServerDataLayer dataLayer, IEventBus eventBus)
        {
            _config = config;
            _dataLayer = dataLayer;
            _eventBus = eventBus;
        }
        public void Handle(EditProject command)
        {
            var query = _dataLayer.GetBuilder<IProject>()
                .Update()
                .SetValue(c => c.Projection, command.Projection)
                .SetValue(c => c.Name, command.Name)
                .SetValue(c => c.Note, command.Note)
                .Filter(c => c.Id, command.Id);
            _dataLayer.Executor.Execute(query);

            _eventBus.Send(new OnEditedProject { ProjectId = command.Id });
        }
    }
}
