using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.DataModels.Sdrn.CalcServer.Entities;
using Atdi.DataModels.Sdrn.CalcServer.Entities.Tasks;
using Atdi.Icsm.Plugins.SdrnStationCalibrationCalc.ViewModels.StationCalibrationManager.Events;
using Atdi.Icsm.Plugins.SdrnStationCalibrationCalc.ViewModels.ProjectManager.Queries;
using Atdi.Platform.Cqrs;
using Atdi.Platform.Events;

namespace Atdi.Icsm.Plugins.SdrnStationCalibrationCalc.ViewModels.StationCalibrationManager.Modifiers
{
    public class CreatePropagationModelsHandler : ICommandHandler<CreatePropagationModels>
    {
        private readonly AppComponentConfig _config;
        private readonly CalcServerDataLayer _dataLayer;
        private readonly IEventBus _eventBus;
        private readonly IObjectReader _objectReader;

        public CreatePropagationModelsHandler(AppComponentConfig config, CalcServerDataLayer dataLayer, IObjectReader objectReader, IEventBus eventBus)
        {
            _config = config;
            _dataLayer = dataLayer;
            _eventBus = eventBus;
            _objectReader = objectReader;
        }

        public void Handle(CreatePropagationModels commandParameters)
        {
            var clientContextId = commandParameters.ContextId;

            var mainQuery = _dataLayer.GetBuilder<IClientContextMainBlock>()
                .Apply()
                .Filter(c => c.ContextId, clientContextId)
                .CreateIfNotExists()
                    .SetValue(c => c.ContextId, clientContextId)
                    .SetValue(c => c.ModelTypeCode, (byte)MainBlockModelTypeCode.ITU525)
                    .SetValue(c => c.ModelTypeName, MainBlockModelTypeCode.ITU525.ToString())
                .UpdateIfExists()
                    .SetValue(c => c.ModelTypeCode, (byte)MainBlockModelTypeCode.ITU525)
                    .SetValue(c => c.ModelTypeName, MainBlockModelTypeCode.ITU525.ToString())
                ;
            var count = _dataLayer.Executor.Execute(mainQuery);
            if (count == 0)
            {
                throw new InvalidOperationException($"Can't apply record of MainBlock in the client context with ID #{clientContextId}");
            }

            var absorptionQuery = _dataLayer.GetBuilder<IClientContextAbsorption>()
                .Apply()
                .Filter(c => c.ContextId, clientContextId)
                .SetValue(c => c.ContextId, clientContextId)
                .SetValue(c => c.ModelTypeCode, (byte)AbsorptionModelTypeCode.FlatAndLinear)
                .SetValue(c => c.ModelTypeName, AbsorptionModelTypeCode.FlatAndLinear.ToString())
                .SetValue(c => c.Available, true)
                ;
            count = _dataLayer.Executor.Execute(absorptionQuery);
            if (count == 0)
            {
                throw new InvalidOperationException($"Can't apply record of Absorption in the client context with ID #{clientContextId}");
            }

            // в случаи не включения блока в расчет запись создавать не обязательно
            // но если нужно зарезервировать ее, то следует создавать так как показано ниже
            var additionalQuery = _dataLayer.GetBuilder<IClientContextAdditional>()
                    .Apply()
                    .Filter(c => c.ContextId, clientContextId)
                    .SetValue(c => c.ContextId, clientContextId)
                    .SetValue(c => c.ModelTypeCode, (byte)AdditionalModelTypeCode.Unknown)
                    .SetValue(c => c.ModelTypeName, AdditionalModelTypeCode.Unknown.ToString())
                    .SetValue(c => c.Available, false)
                ;
            count = _dataLayer.Executor.Execute(additionalQuery);
            if (count == 0)
            {
                throw new InvalidOperationException($"Can't apply record of Additional in the client context with ID #{clientContextId}");
            }

            var atmosphericQuery = _dataLayer.GetBuilder<IClientContextAtmospheric>()
                    .Apply()
                    .Filter(c => c.ContextId, clientContextId)
                    .SetValue(c => c.ContextId, clientContextId)
                    .SetValue(c => c.ModelTypeCode, (byte)AtmosphericModelTypeCode.Unknown)
                    .SetValue(c => c.ModelTypeName, AtmosphericModelTypeCode.Unknown.ToString())
                    .SetValue(c => c.Available, false)
                ;
            count = _dataLayer.Executor.Execute(atmosphericQuery);
            if (count == 0)
            {
                throw new InvalidOperationException($"Can't apply record of Atmospheric in the client context with ID #{clientContextId}");
            }

            var clutterQuery = _dataLayer.GetBuilder<IClientContextClutter>()
                    .Apply()
                    .Filter(c => c.ContextId, clientContextId)
                    .SetValue(c => c.ContextId, clientContextId)
                    .SetValue(c => c.ModelTypeCode, (byte)ClutterModelTypeCode.Unknown)
                    .SetValue(c => c.ModelTypeName, ClutterModelTypeCode.Unknown.ToString())
                    .SetValue(c => c.Available, false)
                ;
            count = _dataLayer.Executor.Execute(clutterQuery);
            if (count == 0)
            {
                throw new InvalidOperationException($"Can't apply record of Clutter in the client context with ID #{clientContextId}");
            }

            var diffractionQuery = _dataLayer.GetBuilder<IClientContextDiffraction>()
                    .Apply()
                    .Filter(c => c.ContextId, clientContextId)
                    .SetValue(c => c.ContextId, clientContextId)
                    .SetValue(c => c.ModelTypeCode, (byte)DiffractionModelTypeCode.Deygout91)
                    .SetValue(c => c.ModelTypeName, DiffractionModelTypeCode.Deygout91.ToString())
                    .SetValue(c => c.Available, true)
                ;
            count = _dataLayer.Executor.Execute(diffractionQuery);
            if (count == 0)
            {
                throw new InvalidOperationException($"Can't apply record of Diffraction in the client context with ID #{clientContextId}");
            }

            var ductingQuery = _dataLayer.GetBuilder<IClientContextDucting>()
                    .Apply()
                    .Filter(c => c.ContextId, clientContextId)
                    .SetValue(c => c.ContextId, clientContextId)
                    .SetValue(c => c.Available, false)
                ;
            count = _dataLayer.Executor.Execute(ductingQuery);
            if (count == 0)
            {
                throw new InvalidOperationException($"Can't apply record of Ducting in the client context with ID #{clientContextId}");
            }

            var reflectionQuery = _dataLayer.GetBuilder<IClientContextReflection>()
                    .Apply()
                    .Filter(c => c.ContextId, clientContextId)
                    .SetValue(c => c.ContextId, clientContextId)
                    .SetValue(c => c.Available, false)
                ;
            count = _dataLayer.Executor.Execute(reflectionQuery);
            if (count == 0)
            {
                throw new InvalidOperationException($"Can't apply record of Reflection in the client context with ID #{clientContextId}");
            }

            var subPathDiffractionQuery = _dataLayer.GetBuilder<IClientContextSubPathDiffraction>()
                    .Apply()
                    .Filter(c => c.ContextId, clientContextId)
                    .SetValue(c => c.ContextId, clientContextId)
                    .SetValue(c => c.ModelTypeCode, (byte)SubPathDiffractionModelTypeCode.Unknown)
                    .SetValue(c => c.ModelTypeName, SubPathDiffractionModelTypeCode.Unknown.ToString())
                    .SetValue(c => c.Available, false)
                ;
            count = _dataLayer.Executor.Execute(subPathDiffractionQuery);
            if (count == 0)
            {
                throw new InvalidOperationException($"Can't apply record of SubPathDiffraction in the client context with ID #{clientContextId}");
            }

            var tropoQuery = _dataLayer.GetBuilder<IClientContextTropo>()
                    .Apply()
                    .Filter(c => c.ContextId, clientContextId)
                    .SetValue(c => c.ContextId, clientContextId)
                    .SetValue(c => c.ModelTypeCode, (byte)TropoModelTypeCode.Unknown)
                    .SetValue(c => c.ModelTypeName, TropoModelTypeCode.Unknown.ToString())
                    .SetValue(c => c.Available, false)
                ;
            count = _dataLayer.Executor.Execute(tropoQuery);
            if (count == 0)
            {
                throw new InvalidOperationException($"Can't apply record of Tropo in the client context with ID #{clientContextId}");
            }

            var paramsQuery = _dataLayer.GetBuilder<IClientContextGlobalParams>()
                    .Apply()
                    .Filter(c => c.ContextId, clientContextId)
                    .SetValue(c => c.ContextId, clientContextId)
                    .SetValue(c => c.Time_pc, 50)
                    .SetValue(c => c.Location_pc, 50)
                    .SetValue(c => c.EarthRadius_km, 8500)
                ;
            count = _dataLayer.Executor.Execute(paramsQuery);
            if (count == 0)
            {
                throw new InvalidOperationException($"Can't apply record of GlobalParams in the client context with ID #{clientContextId}");
            }

            _eventBus.Send(new OnCreatePropagationModels
            {
                 ContextId = commandParameters.ContextId
            });
        }
    }
}
