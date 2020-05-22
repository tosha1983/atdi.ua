using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.DataModels.Sdrn.CalcServer.Entities;
using Atdi.Icsm.Plugins.SdrnCalcServerClient.ViewModels.EntityOrmTest.Events;
using Atdi.Platform.Cqrs;
using Atdi.Platform.Events;

namespace Atdi.Icsm.Plugins.SdrnCalcServerClient.ViewModels.ProjectManager.Modifiers
{
    public class CreateProjectHandler : ICommandHandler<CreateProject>
    {
        private readonly AppComponentConfig _config;
        private readonly CalcServerDataLayer _dataLayer;
        private readonly IEventBus _eventBus;

        public CreateProjectHandler(AppComponentConfig config, CalcServerDataLayer dataLayer, IEventBus eventBus)
        {
            _config = config;
            _dataLayer = dataLayer;
            _eventBus = eventBus;
        }

        public void Handle(CreateProject command)
        {
            var query = _dataLayer.GetBuilder<IProject>()
                .Create()
                .SetValue(c => c.OwnerProjectId, command.OwnerId)
                .SetValue(c => c.OwnerInstance, _config.Instance)
                .SetValue(c => c.Projection, command.Projection)
                .SetValue(c => c.StatusCode, (byte)ProjectStatusCode.Created)
                .SetValue(c => c.StatusName, "Created")
                .SetValue(c => c.Name, command.Name)
                .SetValue(c => c.Note, command.Note);

            var projectPk = _dataLayer.Executor.Execute<IProject_PK>(query);

            _eventBus.Send(new OnCreatedProject
            {
                ProjectId = projectPk.Id
            });

        }
    }
}
