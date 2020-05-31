using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.DataModels.Sdrn.CalcServer.Entities;
using Atdi.Icsm.Plugins.SdrnStationCalibrationCalc.ViewModels.ManagementTasksCalibration.Events;
using Atdi.Platform.Cqrs;
using Atdi.Platform.Events;

namespace Atdi.Icsm.Plugins.SdrnStationCalibrationCalc.ViewModels.ManagementTasksCalibration.Modifiers
{
    public class EditCalcTaskHandler : ICommandHandler<EditCalcTask>
    {
        private readonly AppComponentConfig _config;
        private readonly CalcServerDataLayer _dataLayer;
        private readonly IEventBus _eventBus;

        public EditCalcTaskHandler(AppComponentConfig config, CalcServerDataLayer dataLayer, IEventBus eventBus)
        {
            _config = config;
            _dataLayer = dataLayer;
            _eventBus = eventBus;
        }
        public void Handle(EditCalcTask command)
        {
            var query = _dataLayer.GetBuilder<ICalcTask>()
                .Update()
                .SetValue(c => c.MapName, command.MapName)
                .SetValue(c => c.TypeCode, (byte)command.TypeCode)
                .SetValue(c => c.TypeName, ((CalcTaskTypeCode)(command.TypeCode)).ToString())
                .Filter(c => c.Id, command.Id);
            _dataLayer.Executor.Execute(query);

            _eventBus.Send(new OnEditedCalcTask { CalcTasktId = command.Id });
        }
    }
}
