using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.DataModels.Sdrn.CalcServer.Entities.Tasks;
using Atdi.Icsm.Plugins.SdrnStationCalibrationCalc.ViewModels.StationCalibrationManager.Events;
using Atdi.Platform.Cqrs;
using Atdi.Platform.Events;

namespace Atdi.Icsm.Plugins.SdrnStationCalibrationCalc.ViewModels.StationCalibrationManager.Modifiers
{
    public class DeleteParamsCalculationHandler : ICommandHandler<DeleteParamsCalculation>
    {
        private readonly AppComponentConfig _config;
        private readonly CalcServerDataLayer _dataLayer;
        private readonly IEventBus _eventBus;

        public DeleteParamsCalculationHandler(AppComponentConfig config, CalcServerDataLayer dataLayer, IEventBus eventBus)
        {
            _config = config;
            _dataLayer = dataLayer;
            _eventBus = eventBus;
        }

        public void Handle(DeleteParamsCalculation command)
        {
            var query = _dataLayer.GetBuilder<IStationCalibrationArgs>()
               .Delete()
               .Filter(c => c.TASK.Id, command.TaskId);
            _dataLayer.Executor.Execute(query);

            _eventBus.Send(new OnDeleteParamsCalculation
            {
                TaskId = command.TaskId
            });
        }
    }
}
