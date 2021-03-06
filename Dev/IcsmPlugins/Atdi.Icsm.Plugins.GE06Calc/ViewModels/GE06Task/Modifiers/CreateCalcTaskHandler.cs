﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.DataModels.Api.EntityOrm.WebClient;
using Atdi.DataModels.Sdrn.CalcServer.Entities;
using Atdi.DataModels.Sdrn.CalcServer.Entities.Tasks;
using Atdi.Icsm.Plugins.GE06Calc.ViewModels.GE06Task.Events;
using Atdi.Platform.Cqrs;
using Atdi.Platform.Events;
using Atdi.Platform.Logging;

namespace Atdi.Icsm.Plugins.GE06Calc.ViewModels.GE06Task.Modifiers
{
    public class CreateCalcTaskHandler : ICommandHandler<CreateCalcTask>
    {
        private readonly AppComponentConfig _config;
        private readonly CalcServerDataLayer _dataLayer;
        private readonly IEventBus _eventBus;
        private readonly ILogger _logger;

        public CreateCalcTaskHandler(AppComponentConfig config, CalcServerDataLayer dataLayer, IEventBus eventBus, ILogger logger)
        {
            _config = config;
            _dataLayer = dataLayer;
            _eventBus = eventBus;
            _logger = logger;
        }
        public void Handle(CreateCalcTask command)
        {
            try
            { 
                var query = _dataLayer.GetBuilder<ICalcTask>()
                    .Create()
                    .SetValue(c => c.CONTEXT.Id, command.ContextId)
                    .SetValue(c => c.MapName, command.MapName)
                    .SetValue(c => c.OwnerTaskId, command.OwnerId)
                    .SetValue(c => c.OwnerInstance, _config.Instance)
                    .SetValue(c => c.TypeCode, (byte)CalcTaskTypeCode.Gn06CalcTask)
                    .SetValue(c => c.TypeName, "Gn06CalcTask")
                    .SetValue(c => c.StatusCode, (byte)CalcTaskStatusCode.Created)
                    .SetValue(c => c.StatusName, "Created");

                var pk = _dataLayer.Executor.Execute<ICalcTask_PK>(query);

                var queryArgs = _dataLayer.GetBuilder<IGn06Args>()
                    .Create()
                    .SetValue(c => c.TASK.Id, pk.Id)
                    .SetValue(c => c.AzimuthStep_deg, command.AzimuthStep_deg)
                    .SetValue(c => c.AdditionalContoursByDistances, command.AdditionalContoursByDistances)
                    .SetValue(c => c.Distances, command.Distances)
                    .SetValue(c => c.ContureByFieldStrength, command.ContureByFieldStrength)
                    .SetValue(c => c.FieldStrength, command.FieldStrength)
                    .SetValue(c => c.SubscribersHeight, command.SubscribersHeight)
                    .SetValue(c => c.PercentageTime, command.PercentageTime)
                    .SetValue(c => c.UseEffectiveHeight, command.UseEffectiveHeight)
                    .SetValue(c => c.CalculationTypeCode, command.CalculationTypeCode)
                    .SetValue(c => c.CalculationTypeName, command.CalculationTypeName)
                    .SetValue(c => c.StepBetweenBoundaryPoints, command.StepBetweenBoundaryPoints)
                    .SetValueAsJson(c => c.BroadcastingExtend, command.BroadcastingExtend);
                var pk_StationCalibrationArgs = _dataLayer.Executor.Execute<IGn06Args_PK>(queryArgs);

                _eventBus.Send(new OnCreatedCalcTask { CalcTaskId = pk.Id });
            }
            catch (EntityOrmWebApiException e)
            {
                if (e.ServerException != null)
                {
                    _logger.Exception((EventContext)"ICommandHandler", (EventCategory)"Execute", new Exception($"ExceptionMessage: {e.ServerException.ExceptionMessage}; ExceptionType: {e.ServerException.ExceptionType}; InnerException: {e.ServerException.InnerException}; Message: {e.ServerException.Message}!"));
                }
            }
            catch (Exception e)
            {
                _logger.Exception((EventContext)"ICommandHandler", (EventCategory)"Execute", e);
                throw;
            }
        }
    }
}
