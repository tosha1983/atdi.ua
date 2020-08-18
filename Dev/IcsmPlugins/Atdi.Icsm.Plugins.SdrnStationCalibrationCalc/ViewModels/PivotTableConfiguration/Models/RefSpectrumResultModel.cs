using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Icsm.Plugins.SdrnStationCalibrationCalc.ViewModels.PivotTableConfiguration
{
    public class RefSpectrumResultModel
    {
        public long RefSpectrumResultId;
        public long OrderId;
        public string TableIcsmName;
        public long IdIcsm;
        public long IdSensor;
        public string GlobalCID;
        public double Freq_MHz;
        public double Level_dBm;
        public double Percent;
        public DateTimeOffset DateMeas;
    }
}
