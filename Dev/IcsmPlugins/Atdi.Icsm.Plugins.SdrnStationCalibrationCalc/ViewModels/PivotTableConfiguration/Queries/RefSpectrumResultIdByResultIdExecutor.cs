using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.DataModels.Sdrn.CalcServer.Entities;
using Atdi.Platform.Cqrs;
using CS_ES = Atdi.DataModels.Sdrn.CalcServer.Entities.Tasks;

namespace Atdi.Icsm.Plugins.SdrnStationCalibrationCalc.ViewModels.PivotTableConfiguration.Queries
{
    public class RefSpectrumResultIdByResultIdExecutor : IReadQueryExecutor<RefSpectrumResultIdByResultId, long?>
    {
        private readonly AppComponentConfig _config;
        private readonly CalcServerDataLayer _dataLayer;

        public RefSpectrumResultIdByResultIdExecutor(AppComponentConfig config, CalcServerDataLayer dataLayer)
        {
            _config = config;
            _dataLayer = dataLayer;
        }
        public long? Read(RefSpectrumResultIdByResultId criterion)
        {
            var query = _dataLayer.GetBuilder<CS_ES.IRefSpectrumByDriveTestsResult>()
                .Read()
                .Select(c => c.Id)
                .Filter(c => c.RESULT.Id, criterion.ResultId);

            var reader = _dataLayer.Executor.ExecuteReader(query);
            if (!reader.Read())
            {
                return null;
            }

            return reader.GetValue(c => c.Id);
        }
    }
}
