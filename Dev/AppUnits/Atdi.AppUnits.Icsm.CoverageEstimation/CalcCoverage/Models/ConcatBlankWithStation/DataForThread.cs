using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OSGeo.GDAL;

namespace Atdi.AppUnits.Icsm.CoverageEstimation.Models
{
    public class DataForThread
    {
        public DataConfig  DataConfig { get; set; }
        public TFWParameter TFWBlank { get; set; }
        public string BlankFileName { get; set; }
        public string SourceFileName { get; set; }
        public Dataset DatasetBlank { get; set; }
        public string Projection { get; set; }
        public double[] GeoTransform { get; set; }
        public string NameEwxFile { get; set; }
    }
}
