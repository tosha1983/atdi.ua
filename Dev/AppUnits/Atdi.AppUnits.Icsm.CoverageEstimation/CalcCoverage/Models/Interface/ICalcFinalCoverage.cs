using Atdi.Contracts.CoreServices.DataLayer;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Atdi.AppUnits.Icsm.CoverageEstimation.Models
{
    internal interface ICalcFinalCoverage
    {
        void Run(DataConfig dataConfig, long iterationNumber);
    }
}
