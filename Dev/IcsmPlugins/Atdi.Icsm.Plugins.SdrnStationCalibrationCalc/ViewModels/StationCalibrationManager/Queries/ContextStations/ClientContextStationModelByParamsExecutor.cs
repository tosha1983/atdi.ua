using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.DataModels.Sdrn.CalcServer.Entities;
using Atdi.Platform.Cqrs;
using CS_ES = Atdi.DataModels.Sdrn.CalcServer.Entities;
using Atdi.Icsm.Plugins.SdrnStationCalibrationCalc.ViewModels.StationCalibrationManager;
using Atdi.DataModels.Sdrn.Infocenter.Entities.SdrnServer;

namespace Atdi.Icsm.Plugins.SdrnStationCalibrationCalc.ViewModels.ProjectManager.Queries
{
    public class ClientContextStationModelByParamsExecutor : IReadQueryExecutor<ClientContextStationModelByParams, ClientContextStationModelByParamsResult>
    {
        private readonly AppComponentConfig _config;
        private readonly CalcServerDataLayer _dataLayer;

        public ClientContextStationModelByParamsExecutor(AppComponentConfig config, CalcServerDataLayer dataLayer)
        {
            _config = config;
            _dataLayer = dataLayer;
        }
        public ClientContextStationModelByParamsResult Read(ClientContextStationModelByParams criterion)
        {
            var clientContextStationModelByParamsResult = new ClientContextStationModelByParamsResult();

            var query = _dataLayer.GetBuilder<CS_ES.IContextStation>()
                .Read()
                .Select(c => c.Id)
                .Select(c => c.ModifiedDate)
                .Select(c => c.StateName)
                .Select(c => c.CONTEXT.Id)
                .Filter(c => c.CONTEXT.Id, criterion.ClientContextId)
                .Filter(c => c.ExternalCode, criterion.ExternalCode)
                .Filter(c => c.ExternalSource, criterion.ExternalSource)
                .Filter(c => c.Standard, criterion.Standard)
                ;

            var reader = _dataLayer.Executor.ExecuteReader(query);
            if (!reader.Read())
            {
                return null;
            }
            clientContextStationModelByParamsResult.DateModified = reader.GetValue(c => c.ModifiedDate);
            clientContextStationModelByParamsResult.StationId = reader.GetValue(c => c.Id);
            clientContextStationModelByParamsResult.Status= reader.GetValue(c => c.StateName);

            return clientContextStationModelByParamsResult;
        }
    }
}
