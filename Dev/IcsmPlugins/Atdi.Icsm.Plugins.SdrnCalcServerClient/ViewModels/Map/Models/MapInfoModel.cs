using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Icsm.Plugins.SdrnCalcServerClient.ViewModels.Map
{
    public class MapInfoModel
    {
        public long Id { get; set; }
        public byte StatusCode { get; set; }
        public string StatusName { get; set; }
        public string StatusNote { get; set; }
        public DateTimeOffset CreatedDate { get; set; }
        public byte TypeCode { get; set; }
        public string TypeName { get; set; }
        public string Projection { get; set; }
        public string StepUnit { get; set; }
        public string StepDataType { get; set; }
        public byte StepDataSize { get; set; }
        public int AxisXNumber { get; set; }
        public int AxisXStep { get; set; }
        public int AxisYNumber { get; set; }
        public int AxisYStep { get; set; }
        public int UpperLeftX { get; set; }
        public int UpperLeftY { get; set; }
        public int UpperRightX { get; set; }
        public int UpperRightY { get; set; }
        public int LowerLeftX { get; set; }
        public int LowerLeftY { get; set; }
        public int LowerRightX { get; set; }
        public int LowerRightY { get; set; }
        public int ContentSize { get; set; }
        public string ContentSource { get; set; }
        public int? FileSize { get; set; }
        public string FileName { get; set; }
        public string MapName { get; set; }
        public string MapNote { get; set; }
        public int SectorsCount { get; set; }
        public int SectorsXCount { get; set; }
        public int SectorsYCount { get; set; }
    }
}
