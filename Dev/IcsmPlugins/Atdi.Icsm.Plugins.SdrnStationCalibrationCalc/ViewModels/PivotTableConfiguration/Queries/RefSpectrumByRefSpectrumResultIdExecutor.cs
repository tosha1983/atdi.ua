using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.DataModels.Api.EntityOrm.WebClient;
using Atdi.DataModels.Sdrn.CalcServer.Entities;
using Atdi.Platform.Cqrs;
using CS_ES = Atdi.DataModels.Sdrn.CalcServer.Entities.Tasks;

namespace Atdi.Icsm.Plugins.SdrnStationCalibrationCalc.ViewModels.PivotTableConfiguration.Queries
{
    public class RefSpectrumByRefSpectrumResultIdExecutor : IReadQueryExecutor<RefSpectrumByRefSpectrumResultId, RefSpectrumResultModel[]>
    {
        private readonly AppComponentConfig _config;
        private readonly CalcServerDataLayer _dataLayer;

        public RefSpectrumByRefSpectrumResultIdExecutor(AppComponentConfig config, CalcServerDataLayer dataLayer)
        {
            _config = config;
            _dataLayer = dataLayer;
        }
        public RefSpectrumResultModel[] Read(RefSpectrumByRefSpectrumResultId criterion)
        {
            var listRefSpectreum = new List<RefSpectrumResultModel>();
            var query = _dataLayer.GetBuilder<CS_ES.IRefSpectrumByDriveTestsDetailResult>()
                .Read()
                .Select(c => c.Id)
                .Select(c => c.TableIcsmName)
                .Select(c => c.IdIcsm)
                .Select(c => c.OrderId)
                .Select(c => c.IdSensor)
                .Select(c => c.GlobalCID)
                .Select(c => c.Freq_MHz)
                .Select(c => c.Level_dBm)
                .Select(c => c.Percent)
                .Select(c => c.DateMeas)
                .Filter(c => c.RESULT_REF_SPECTRUM.Id, criterion.RefSpectrumResultId);

            var reader = _dataLayer.Executor.ExecuteReader(query);
            while (reader.Read())
            {
                listRefSpectreum.Add(new RefSpectrumResultModel()
                {
                    OrderId = reader.GetValue(c => c.OrderId),
                    TableIcsmName = reader.GetValue(c => c.TableIcsmName),
                    IdIcsm = reader.GetValue(c => c.IdIcsm),
                    IdSensor = reader.GetValue(c => c.IdSensor),
                    GlobalCID = reader.GetValue(c => c.GlobalCID),
                    Level_dBm = reader.GetValue(c => c.Level_dBm),
                    Freq_MHz = reader.GetValue(c => c.Freq_MHz),
                    Percent = reader.GetValue(c => c.Percent),
                    DateMeas = reader.GetValue(c => c.DateMeas)
                });
            }
            return listRefSpectreum.ToArray();
        }
    }
}
