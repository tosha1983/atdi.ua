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
    public sealed class SensorsDataAdapter : EntityDataAdapter<CS_ES.IDriveTest, SensorModel>
    {
        public long[] DriveTestIds;
        public SensorsDataAdapter(InfocenterDataLayer dataLayer, ILogger logger) : base(dataLayer.Origin, logger)
        {
        }
        protected override void PrepareQuery(IReadQuery<CS_ES.IDriveTest> query)
        {
            query.Select(
                c => c.Id,
                c => c.RESULT.SENSOR.SensorIdentifierId,
                c => c.RESULT.SENSOR.Status,
                c => c.RESULT.SENSOR.Name,
                c => c.RESULT.SENSOR.BiuseDate,
                c => c.RESULT.SENSOR.EouseDate,
                c => c.RESULT.SENSOR.Azimuth,
                c => c.RESULT.SENSOR.Elevation,
                c => c.RESULT.SENSOR.Azimuth,
                c => c.RESULT.SENSOR.Agl,
                c => c.RESULT.SENSOR.RxLoss,
                c => c.RESULT.SENSOR.TechId)
                .Filter(f => f.Id, DataModels.Api.EntityOrm.WebClient.FilterOperator.In, DriveTestIds)
                .Distinct();
        }
        protected override SensorModel ReadData(IDataReader<CS_ES.IDriveTest> reader, int index)
        {
            return new SensorModel
            {
                Id = reader.GetValue(c => c.Id),
                SensorIdentifierId = reader.GetValue(c => c.RESULT.SENSOR.SensorIdentifierId),
                Status = reader.GetValue(c => c.RESULT.SENSOR.Status),
                Name = reader.GetValue(c => c.RESULT.SENSOR.Name),
                BiuseDate = reader.GetValue(c => c.RESULT.SENSOR.BiuseDate),
                EouseDate = reader.GetValue(c => c.RESULT.SENSOR.EouseDate),
                Azimuth = reader.GetValue(c => c.RESULT.SENSOR.Azimuth),
                Elevation = reader.GetValue(c => c.RESULT.SENSOR.Elevation),
                Agl = reader.GetValue(c => c.RESULT.SENSOR.Agl),
                RxLoss = reader.GetValue(c => c.RESULT.SENSOR.RxLoss),
                TechId = reader.GetValue(c => c.RESULT.SENSOR.TechId)
            };
        }
    }
}
