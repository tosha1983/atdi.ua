using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.SDR.Server.MeasurementProcessing
{
    public enum MeasError
    {
        NoError,
        Initialization,
        Calibration,
        ConfigurationTask,
        ConfigurationSDR,
        Measurements
    }
}
