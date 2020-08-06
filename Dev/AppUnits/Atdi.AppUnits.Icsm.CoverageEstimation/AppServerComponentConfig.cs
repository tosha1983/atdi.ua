using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.Platform.AppComponent;

namespace Atdi.AppUnits.Icsm.CoverageEstimation
{
    public sealed class AppServerComponentConfig
    {
        [ComponentConfigProperty("Telecom.CoverageConfigFileName")]
        public string CoverageConfigFileName { get; set; }

        [ComponentConfigProperty("Telecom.ProtocolOperationFileNameForMobStation")]
        public string ProtocolOperationFileNameForMobStation { get; set; }

        [ComponentConfigProperty("Telecom.ProtocolOperationFileNameForMobStation2")]
        public string ProtocolOperationFileNameForMobStation2 { get; set; }

        [ComponentConfigProperty("Telecom.IsRepeatable")]
        public bool IsRepeatable { get; set; }

        [ComponentConfigProperty("Telecom.IsSaveFinalCoverageToDB")]
        public bool IsSaveFinalCoverageToDB { get; set; }

        [ComponentConfigProperty("Telecom.HookBitBltWinAPIFunctionInjectDll")]
        public string HookBitBltWinAPIFunctionInjectDll { get; set; }

        [ComponentConfigProperty("Job.EnableMobStationCalculation")]
        public bool EnableMobStationCalculation { get; set; }

        [ComponentConfigProperty("Job.EnableMobStation2Calculation")]
        public bool EnableMobStation2Calculation { get; set; }

    }
}
