using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.DataModels.Sdrn.CalcServer.Entities;
using Atdi.Platform.Cqrs;

namespace Atdi.Icsm.Plugins.SdrnStationCalibrationCalc.ViewModels.ManagementTasksCalibration.Queries
{
    public class GetClientContextByIdExecutor : IReadQueryExecutor<GetClientContextById, ClientContextModel>
    {
        private readonly AppComponentConfig _config;
        private readonly CalcServerDataLayer _dataLayer;

        public GetClientContextByIdExecutor(AppComponentConfig config, CalcServerDataLayer dataLayer)
        {
            _config = config;
            _dataLayer = dataLayer;
        }
        public ClientContextModel Read(GetClientContextById criterion)
        {
            var query = _dataLayer.GetBuilder<IClientContext>()
                .Read()
                .Select(
                    c => c.Id,
                    c => c.Name,
                    c => c.Note,
                    c => c.TypeCode,
                    c => c.BASE_CONTEXT.Name)
                .Filter(c => c.Id, criterion.Id);

            var reader = _dataLayer.Executor.ExecuteReader(query);
            if (!reader.Read())
            {
                return null;
            }

            return new ClientContextModel()
            {
                Id = reader.GetValue(c => c.Id),
                Name = reader.GetValue(c => c.Name),
                Note = reader.GetValue(c => c.Note),
                TypeCode = reader.GetValue(c => c.TypeCode),
                BaseContextName = reader.GetValue(c => c.BASE_CONTEXT.Name)
            };
        }
    }
}
