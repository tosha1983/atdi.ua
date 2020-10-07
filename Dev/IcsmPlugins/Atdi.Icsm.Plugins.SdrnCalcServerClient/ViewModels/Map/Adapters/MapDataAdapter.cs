using Atdi.Icsm.Plugins.Core;
using Atdi.Platform.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.Contracts.Api.EntityOrm.WebClient;
using CS_ES = Atdi.DataModels.Sdrn.Infocenter.Entities;
using Atdi.Icsm.Plugins.SdrnCalcServerClient.ViewModels.ProjectManager;

namespace Atdi.Icsm.Plugins.SdrnCalcServerClient.ViewModels.Map.Adapters
{
    public sealed class MapDataAdapter : EntityDataAdapter<CS_ES.IMap, MapInfoModel>
    {
        public MapDataAdapter(InfocenterDataLayer dataLayer, ILogger logger) : base(dataLayer.Origin, logger)
        {
        }
        protected override void PrepareQuery(IReadQuery<CS_ES.IMap> query)
        {
            query.Select(
                c => c.Id,
                c => c.StatusCode,
                c => c.StatusName,
                c => c.StatusNote,
                c => c.CreatedDate,
                c => c.TypeCode,
                c => c.TypeName,
                c => c.Projection,
                c => c.StepUnit,
                c => c.StepDataType,
                c => c.StepDataSize,
                c => c.AxisXNumber,
                c => c.AxisXStep,
                c => c.AxisYNumber,
                c => c.AxisYStep,
                c => c.UpperLeftX,
                c => c.UpperLeftY,
                c => c.UpperRightX,
                c => c.UpperRightY,
                c => c.LowerLeftX,
                c => c.LowerLeftY,
                c => c.LowerRightX,
                c => c.LowerRightY,
                c => c.ContentSize,
                c => c.ContentSource,
                c => c.FileSize,
                c => c.FileName,
                c => c.MapName,
                c => c.MapNote,
                c => c.SectorsCount,
                c => c.SectorsXCount,
                c => c.SectorsYCount)
            .OrderByAsc(o => o.FileName);
        }
        protected override MapInfoModel ReadData(IDataReader<CS_ES.IMap> reader, int index)
        {
            var fileName = reader.GetValue(c => c.FileName);

            return new MapInfoModel
            {
                Id = reader.GetValue(c => c.Id),
                StatusCode = reader.GetValue(c => c.StatusCode),
                StatusName = reader.GetValue(c => c.StatusName),
                StatusNote = reader.GetValue(c => c.StatusNote),
                CreatedDate = reader.GetValue(c => c.CreatedDate),
                TypeCode = reader.GetValue(c => c.TypeCode),
                TypeName = reader.GetValue(c => c.TypeName),
                Projection = reader.GetValue(c => c.Projection),
                StepUnit = reader.GetValue(c => c.StepUnit),
                StepDataType = reader.GetValue(c => c.StepDataType),
                StepDataSize = reader.GetValue(c => c.StepDataSize),
                AxisXNumber = reader.GetValue(c => c.AxisXNumber),
                AxisXStep = reader.GetValue(c => c.AxisXStep),
                AxisYNumber = reader.GetValue(c => c.AxisYNumber),
                AxisYStep = reader.GetValue(c => c.AxisYStep),
                UpperLeftX = reader.GetValue(c => c.UpperLeftX),
                UpperLeftY = reader.GetValue(c => c.UpperLeftY),
                UpperRightX = reader.GetValue(c => c.UpperRightX),
                UpperRightY = reader.GetValue(c => c.UpperRightY),
                LowerLeftX = reader.GetValue(c => c.LowerLeftX),
                LowerLeftY = reader.GetValue(c => c.LowerLeftY),
                LowerRightX = reader.GetValue(c => c.LowerRightX),
                LowerRightY = reader.GetValue(c => c.LowerRightY),
                ContentSize = reader.GetValue(c => c.ContentSize),
                ContentSource = reader.GetValue(c => c.ContentSource),
                FileSize = reader.GetValue(c => c.FileSize),
                FileName = fileName,
                MapName = fileName, //reader.GetValue(c => c.MapName),
                MapNote = reader.GetValue(c => c.MapNote),
                SectorsCount = reader.GetValue(c => c.SectorsCount),
                SectorsXCount = reader.GetValue(c => c.SectorsXCount),
                SectorsYCount = reader.GetValue(c => c.SectorsYCount)
            };
        }
    }
}
