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
    public class AbortCalcTaskHandler : ICommandHandler<AbortCalcTask>
    {
        private readonly AppComponentConfig _config;
        private readonly CalcServerDataLayer _dataLayer;
        private readonly IEventBus _eventBus;

        public AbortCalcTaskHandler(AppComponentConfig config, CalcServerDataLayer dataLayer, IEventBus eventBus)
        {
            _config = config;
            _dataLayer = dataLayer;
            _eventBus = eventBus;
        }
        public void Handle(AbortCalcTask command)
        {
            var insQuery = _dataLayer.GetBuilder<ICalcCommand>()
                .Create()
                .SetValue(c => c.CreatedDate, DateTimeOffset.Now)
                .SetValue(c => c.CallerInstance, _config.Instance)
                .SetValue(c => c.CallerCommandId, Guid.NewGuid())
                // состоние - сразу отправляем клманду  на выполнение
                .SetValue(c => c.StatusCode, (byte)CalcCommandStatusCode.Pending)
                .SetValue(c => c.StatusName, CalcCommandStatusCode.Pending.ToString())
                // тип команды
                .SetValue(c => c.TypeCode, (byte)CalcCommandTypeCode.CancelCalcTask)
                .SetValue(c => c.TypeName, CalcCommandTypeCode.AbortCalcTask.ToString())
                // для данного типа есть обязательные аргументы
                .SetValueAsJson(c => c.ArgsJson, new AbortCalcTaskCommand
                {
                    ResultId = command.ResultId
                })
            ;
            var resultPk = _dataLayer.Executor.Execute<ICalcCommand_PK>(insQuery);

            _eventBus.Send(new OnAbortCalcTask { ResultId = command.ResultId });
        }
    }
}
