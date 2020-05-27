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
    public class CreateMapHandler : ICommandHandler<CreateMap>
    {
        private readonly AppComponentConfig _config;
        private readonly CalcServerDataLayer _dataLayer;
        private readonly IEventBus _eventBus;

        public CreateMapHandler(AppComponentConfig config, CalcServerDataLayer dataLayer, IEventBus eventBus)
        {
            _config = config;
            _dataLayer = dataLayer;
            _eventBus = eventBus;
        }
        public void Handle(CreateMap command)
        {
            var query = _dataLayer.GetBuilder<IProjectMap>()
                .Create()
                .SetValue(c => c.PROJECT.Id, command.ProjectId)
                .SetValue(c => c.MapName, command.MapName)
                .SetValue(c => c.MapNote, command.MapNote)
                .SetValue(c => c.OwnerInstance, _config.Instance)
                .SetValue(c => c.OwnerMapId, command.OwnerId)
                .SetValue(c => c.StatusCode, (byte)ProjectMapStatusCode.Created)
                .SetValue(c => c.StatusName, "Created")
                .SetValue(c => c.StepUnit, command.StepUnit)
                .SetValue(c => c.OwnerAxisXNumber, command.OwnerAxisXNumber)
                .SetValue(c => c.OwnerAxisXStep, command.OwnerAxisXStep)
                .SetValue(c => c.OwnerAxisYNumber, command.OwnerAxisYNumber)
                .SetValue(c => c.OwnerAxisYStep, command.OwnerAxisYStep)
                .SetValue(c => c.OwnerUpperLeftX, command.OwnerUpperLeftX)
                .SetValue(c => c.OwnerUpperLeftY, command.OwnerUpperLeftY);

            var projectMapPk = _dataLayer.Executor.Execute<IProjectMap_PK>(query);

            _eventBus.Send(new OnCreatedMap { MapId = projectMapPk.Id });
        }
    }
}
