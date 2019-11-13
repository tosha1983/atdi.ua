using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace Atdi.AppUnits.Icsm.CoverageEstimation.Models
{
    [Serializable]
    public enum Operation
    {
        CreateEWX,
        CreateTempTifFiles,
        CreateFinalCoverage,
        SaveFinalCoverageToDB
    }
}
