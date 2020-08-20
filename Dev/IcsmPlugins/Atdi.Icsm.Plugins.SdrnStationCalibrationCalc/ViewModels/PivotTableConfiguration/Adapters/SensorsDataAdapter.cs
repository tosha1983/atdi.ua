using Atdi.Icsm.Plugins.Core;
using Atdi.Platform.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.Contracts.Api.EntityOrm.WebClient;
using CS_ES = Atdi.DataModels.Sdrn.Infocenter.Entities.SdrnServer;

namespace Atdi.Icsm.Plugins.SdrnStationCalibrationCalc.ViewModels.PivotTableConfiguration.Adapters
{
    public sealed class SensorsDataAdapter : EntityDataAdapter<CS_ES.ISensor, SensorModel>
    {
        public long[] SensorIds;
        public SensorsDataAdapter(InfocenterDataLayer dataLayer, ILogger logger) : base(dataLayer.Origin, logger)
        {
        }
        protected override void PrepareQuery(IReadQuery<CS_ES.ISensor> query)
        {
            query.Select(
                c => c.Id,
                c => c.SensorIdentifierId,
                c => c.Status,
                c => c.Name,
                c => c.BiuseDate,
                c => c.EouseDate,
                c => c.Azimuth,
                c => c.Elevation,
                c => c.Azimuth,
                c => c.Agl,
                c => c.RxLoss,
                c => c.TechId)
                //.Filter(f => f.Id, DataModels.Api.EntityOrm.WebClient.FilterOperator.In, SensorIds)
                .Distinct();
        }
        protected override SensorModel ReadData(IDataReader<CS_ES.ISensor> reader, int index)
        {
            return new SensorModel
            {
                Id = reader.GetValue(c => c.Id),
                SensorIdentifierId = reader.GetValue(c => c.SensorIdentifierId),
                Status = reader.GetValue(c => c.Status),
                Name = reader.GetValue(c => c.Name),
                BiuseDate = reader.GetValue(c => c.BiuseDate),
                EouseDate = reader.GetValue(c => c.EouseDate),
                Azimuth = reader.GetValue(c => c.Azimuth),
                Elevation = reader.GetValue(c => c.Elevation),
                Agl = reader.GetValue(c => c.Agl),
                RxLoss = reader.GetValue(c => c.RxLoss),
                TechId = reader.GetValue(c => c.TechId)
            };
        }
    }
}
