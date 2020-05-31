using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.DataModels.Sdrn.CalcServer.Entities;
using Atdi.DataModels.Sdrn.CalcServer.Entities.Tasks;
using Atdi.Icsm.Plugins.SdrnStationCalibrationCalc.ViewModels.ManagementTasksCalibration.Events;
using Atdi.Platform.Cqrs;
using Atdi.Platform.Events;

namespace Atdi.Icsm.Plugins.SdrnStationCalibrationCalc.ViewModels.ManagementTasksCalibration.Modifiers
{
    public class CreateCalcTaskHandler : ICommandHandler<CreateCalcTask>
    {
        private readonly AppComponentConfig _config;
        private readonly CalcServerDataLayer _dataLayer;
        private readonly IEventBus _eventBus;

        public CreateCalcTaskHandler(AppComponentConfig config, CalcServerDataLayer dataLayer, IEventBus eventBus)
        {
            _config = config;
            _dataLayer = dataLayer;
            _eventBus = eventBus;
        }
        public void Handle(CreateCalcTask command)
        {
            var query = _dataLayer.GetBuilder<ICalcTask>()
                .Create()
                .SetValue(c => c.CONTEXT.Id, command.ContextId)
                .SetValue(c => c.OwnerTaskId, command.OwnerId)
                .SetValue(c => c.OwnerInstance, _config.Instance)
                .SetValue(c => c.MapName, command.MapName)
                .SetValue(c => c.TypeCode, (byte)command.TypeCode)
                .SetValue(c => c.TypeName, ((CalcTaskTypeCode)(command.TypeCode)).ToString()) // Enum.GetValues(typeof(CalcTaskTypeCode)).GetValue(command.TypeCode).ToString())
                .SetValue(c => c.StatusCode, (byte)CalcTaskStatusCode.Created)
                .SetValue(c => c.StatusName, "Created");

            var pk = _dataLayer.Executor.Execute<ICalcTask_PK>(query);

            var queryArgs = _dataLayer.GetBuilder<IStationCalibrationArgs>()
                .Create()
                .SetValue(c => c.TASK.Id, pk.Id);
            _dataLayer.Executor.Execute(queryArgs);

            _eventBus.Send(new OnCreatedCalcTask { CalcTasktId = pk.Id });
        }
    }
}
