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
    public class DeleteMapHandler : ICommandHandler<DeleteMap>
    {
        private readonly AppComponentConfig _config;
        private readonly CalcServerDataLayer _dataLayer;
        private readonly IEventBus _eventBus;

        public DeleteMapHandler(AppComponentConfig config, CalcServerDataLayer dataLayer, IEventBus eventBus)
        {
            _config = config;
            _dataLayer = dataLayer;
            _eventBus = eventBus;
        }

        public void Handle(DeleteMap command)
        {
            var query = _dataLayer.GetBuilder<IProjectMap>()
                .Delete()
                .Filter(c => c.Id, command.Id);
            _dataLayer.Executor.Execute(query);

            _eventBus.Send(new OnDeletedMap
            {
                MapId = command.Id
            });

        }
    }
}
