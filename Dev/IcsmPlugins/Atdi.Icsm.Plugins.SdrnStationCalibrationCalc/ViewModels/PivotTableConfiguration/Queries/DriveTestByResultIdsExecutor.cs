using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.DataModels.Sdrn.CalcServer.Entities;
using Atdi.Platform.Cqrs;
using CS_ES = Atdi.DataModels.Sdrn.CalcServer.Entities.Tasks;
using Atdi.DataModels.Sdrn.Infocenter.Entities.SdrnServer;

namespace Atdi.Icsm.Plugins.SdrnStationCalibrationCalc.ViewModels.PivotTableConfiguration.Queries
{
    public class DriveTestByResultIdsExecutor : IReadQueryExecutor<DriveTestByResultIds, long[]>
    {
        private readonly AppComponentConfig _config;
        private readonly CalcServerDataLayer _dataLayer;

        public DriveTestByResultIdsExecutor(AppComponentConfig config, CalcServerDataLayer dataLayer)
        {
            _config = config;
            _dataLayer = dataLayer;
        }
        public long[] Read(DriveTestByResultIds criterion)
        {
            var listIds = new List<long>();
            var query = _dataLayer.GetBuilder<CS_ES.IStationCalibrationDriveTestResult>()
               .Read()
               .Select(c => c.Id)
               .Filter(f => f.CalibrationResultId, DataModels.Api.EntityOrm.WebClient.FilterOperator.Equal, criterion.ResultId);

            var reader = _dataLayer.Executor.ExecuteReader(query);
            while (reader.Read())
            {
                listIds.Add(reader.GetValue(c => c.Id));
            }
            return listIds.ToArray();
        }
    }
}
