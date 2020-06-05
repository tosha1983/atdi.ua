using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.DataModels.Sdrn.DeepServices.Gis;
using Atdi.DataModels.Sdrn.DeepServices.RadioSystem.Stations;
using Atdi.DataModels.Sdrn.CalcServer.Internal.Clients;

namespace Atdi.DataModels.Sdrn.CalcServer.Internal.Iterations
{
    public static class RulesStatusParameters
    {

        public static List<CalibrationStatusParameters> CalibrationSecondStatusParameter = new List<Iterations.CalibrationStatusParameters>
            {
                 new CalibrationStatusParameters()
                 {
                     StatusStation = ClientContextStationType.A,
                     PointForCorrelation = true,
                     ExceptionParameter = true,
                     ResultStationStatus = StationStatusResult.NS,
                     ResultDriveTestStatus = DriveTestStatusResult.LS,
                     ModeStatusParameters = ModeStatusParameters.SecondStatusParameter
                  },
                 new CalibrationStatusParameters()
                 {
                     StatusStation = ClientContextStationType.A,
                     PointForCorrelation = true,
                     ExceptionParameter = false,
                     ResultStationStatus = StationStatusResult.CS,
                     ResultDriveTestStatus = DriveTestStatusResult.LS,
                     ModeStatusParameters = ModeStatusParameters.SecondStatusParameter
                  },
                 new CalibrationStatusParameters()
                 {
                     StatusStation = ClientContextStationType.A,
                     PointForCorrelation = false,
                     ExceptionParameter = true,
                     ResultStationStatus = StationStatusResult.UN,
                     ResultDriveTestStatus = DriveTestStatusResult.UN,
                     ModeStatusParameters = ModeStatusParameters.SecondStatusParameter
                  },
                 new CalibrationStatusParameters()
                 {
                     StatusStation = ClientContextStationType.A,
                     PointForCorrelation = false,
                     ExceptionParameter = false,
                     ResultStationStatus = StationStatusResult.UN,
                     ResultDriveTestStatus = DriveTestStatusResult.UN,
                     ModeStatusParameters = ModeStatusParameters.SecondStatusParameter
                  },
                 new CalibrationStatusParameters()
                 {
                     StatusStation = ClientContextStationType.I,
                     PointForCorrelation = true,
                     ExceptionParameter = true,
                     ResultStationStatus = StationStatusResult.IT,
                     ResultDriveTestStatus = DriveTestStatusResult.LS,
                     ModeStatusParameters = ModeStatusParameters.SecondStatusParameter
                  }
                 ,
                 new CalibrationStatusParameters()
                 {
                     StatusStation = ClientContextStationType.I,
                     PointForCorrelation = true,
                     ExceptionParameter = false,
                     ResultStationStatus = StationStatusResult.IT,
                     ResultDriveTestStatus = DriveTestStatusResult.LS,
                     ModeStatusParameters = ModeStatusParameters.SecondStatusParameter
                  }
                 ,
                 new CalibrationStatusParameters()
                 {
                     StatusStation = ClientContextStationType.I,
                     PointForCorrelation = false,
                     ExceptionParameter = true,
                     ResultStationStatus = StationStatusResult.UN,
                     ResultDriveTestStatus = DriveTestStatusResult.UN,
                     ModeStatusParameters = ModeStatusParameters.SecondStatusParameter
                  }
                 ,
                 new CalibrationStatusParameters()
                 {
                     StatusStation = ClientContextStationType.I,
                     PointForCorrelation = false,
                     ExceptionParameter = false,
                     ResultStationStatus = StationStatusResult.UN,
                     ResultDriveTestStatus = DriveTestStatusResult.UN,
                     ModeStatusParameters = ModeStatusParameters.SecondStatusParameter
                  }
            };

        public static List<CalibrationStatusParameters> CalibrationFirstStatusParameters = new List<Iterations.CalibrationStatusParameters>
            {
                 new CalibrationStatusParameters()
                 {
                     StatusStation = ClientContextStationType.A,
                     Corellation = true,
                     TrustOldResults = true,
                     PointForCorrelation = true,
                     ExceptionParameter = true,
                     ResultStationStatus = StationStatusResult.NS,
                     ResultDriveTestStatus = DriveTestStatusResult.LS,
                     ModeStatusParameters = ModeStatusParameters.FirstStatusParameters
                  },
                 new CalibrationStatusParameters()
                 {
                     StatusStation = ClientContextStationType.A,
                     Corellation = true,
                     TrustOldResults = true,
                     PointForCorrelation = true,
                     ExceptionParameter = false,
                     ResultStationStatus = StationStatusResult.CS,
                     ResultDriveTestStatus = DriveTestStatusResult.LS,
                     ModeStatusParameters = ModeStatusParameters.FirstStatusParameters
                  }
                 ,
                 new CalibrationStatusParameters()
                 {
                     StatusStation = ClientContextStationType.A,
                     Corellation = true,
                     TrustOldResults = true,
                     PointForCorrelation = false,
                     ExceptionParameter = true,
                     ResultStationStatus = StationStatusResult.CS,
                     ResultDriveTestStatus = DriveTestStatusResult.LS,
                     ModeStatusParameters = ModeStatusParameters.FirstStatusParameters
                  }
                 ,
                 new CalibrationStatusParameters()
                 {
                     StatusStation = ClientContextStationType.A,
                     Corellation = true,
                     TrustOldResults = true,
                     PointForCorrelation = false,
                     ExceptionParameter = false,
                     ResultStationStatus = StationStatusResult.CS,
                     ResultDriveTestStatus = DriveTestStatusResult.LS,
                     ModeStatusParameters = ModeStatusParameters.FirstStatusParameters
                  }
                  ,
                 new CalibrationStatusParameters()
                 {
                     StatusStation = ClientContextStationType.A,
                     Corellation = true,
                     TrustOldResults = false,
                     PointForCorrelation = true,
                     ExceptionParameter = true,
                     ResultStationStatus = StationStatusResult.NS,
                     ResultDriveTestStatus = DriveTestStatusResult.LS,
                     ModeStatusParameters = ModeStatusParameters.FirstStatusParameters
                  }
                 ,
                 new CalibrationStatusParameters()
                 {
                     StatusStation = ClientContextStationType.A,
                     Corellation = true,
                     TrustOldResults = false,
                     PointForCorrelation = true,
                     ExceptionParameter = false,
                     ResultStationStatus = StationStatusResult.CS,
                     ResultDriveTestStatus = DriveTestStatusResult.LS,
                     ModeStatusParameters = ModeStatusParameters.FirstStatusParameters
                  }
                 ,
                 new CalibrationStatusParameters()
                 {
                     StatusStation = ClientContextStationType.A,
                     Corellation = true,
                     TrustOldResults = false,
                     PointForCorrelation = false,
                     ExceptionParameter = true,
                     ResultStationStatus = StationStatusResult.UN,
                     ResultDriveTestStatus = DriveTestStatusResult.LS,
                     ModeStatusParameters = ModeStatusParameters.FirstStatusParameters
                  }
                  ,
                 new CalibrationStatusParameters()
                 {
                     StatusStation = ClientContextStationType.A,
                     Corellation = true,
                     TrustOldResults = false,
                     PointForCorrelation = false,
                     ExceptionParameter = false,
                     ResultStationStatus = StationStatusResult.UN,
                     ResultDriveTestStatus = DriveTestStatusResult.LS,
                     ModeStatusParameters = ModeStatusParameters.FirstStatusParameters
                  }
                  ,
                 new CalibrationStatusParameters()
                 {
                     StatusStation = ClientContextStationType.A,
                     Corellation = false,
                     TrustOldResults = true,
                     PointForCorrelation = true,
                     ExceptionParameter = true,
                     ResultStationStatus = StationStatusResult.CS,
                     ResultDriveTestStatus = DriveTestStatusResult.LS,
                     ModeStatusParameters = ModeStatusParameters.FirstStatusParameters
                  }
                  ,
                 new CalibrationStatusParameters()
                 {
                     StatusStation = ClientContextStationType.A,
                     Corellation = false,
                     TrustOldResults = true,
                     PointForCorrelation = true,
                     ExceptionParameter = false,
                     ResultStationStatus = StationStatusResult.CS,
                     ResultDriveTestStatus = DriveTestStatusResult.LS,
                     ModeStatusParameters = ModeStatusParameters.FirstStatusParameters
                  }
                 ,
                 new CalibrationStatusParameters()
                 {
                     StatusStation = ClientContextStationType.A,
                     Corellation = false,
                     TrustOldResults = true,
                     PointForCorrelation = false,
                     ExceptionParameter = true,
                     ResultStationStatus = StationStatusResult.CS,
                     ResultDriveTestStatus = DriveTestStatusResult.LS,
                     ModeStatusParameters = ModeStatusParameters.FirstStatusParameters
                  }
                  ,
                 new CalibrationStatusParameters()
                 {
                     StatusStation = ClientContextStationType.A,
                     Corellation = false,
                     TrustOldResults = true,
                     PointForCorrelation = false,
                     ExceptionParameter = false,
                     ResultStationStatus = StationStatusResult.CS,
                     ResultDriveTestStatus = DriveTestStatusResult.LS,
                     ModeStatusParameters = ModeStatusParameters.FirstStatusParameters
                  }
                  ,
                 new CalibrationStatusParameters()
                 {
                     StatusStation = ClientContextStationType.A,
                     Corellation = false,
                     TrustOldResults = false,
                     PointForCorrelation = true,
                     ExceptionParameter = true,
                     ResultStationStatus = StationStatusResult.UN,
                     ResultDriveTestStatus = DriveTestStatusResult.LS,
                     ModeStatusParameters = ModeStatusParameters.FirstStatusParameters
                  }
                  ,
                 new CalibrationStatusParameters()
                 {
                     StatusStation = ClientContextStationType.A,
                     Corellation = false,
                     TrustOldResults = false,
                     PointForCorrelation = true,
                     ExceptionParameter = false,
                     ResultStationStatus = StationStatusResult.UN,
                     ResultDriveTestStatus = DriveTestStatusResult.LS,
                     ModeStatusParameters = ModeStatusParameters.FirstStatusParameters
                  }
                 ,
                 new CalibrationStatusParameters()
                 {
                     StatusStation = ClientContextStationType.A,
                     Corellation = false,
                     TrustOldResults = false,
                     PointForCorrelation = false,
                     ExceptionParameter = true,
                     ResultStationStatus = StationStatusResult.UN,
                     ResultDriveTestStatus = DriveTestStatusResult.LS,
                     ModeStatusParameters = ModeStatusParameters.FirstStatusParameters
                  }
                  ,
                 new CalibrationStatusParameters()
                 {
                     StatusStation = ClientContextStationType.A,
                     Corellation = false,
                     TrustOldResults = false,
                     PointForCorrelation = false,
                     ExceptionParameter = false,
                     ResultStationStatus = StationStatusResult.UN,
                     ResultDriveTestStatus = DriveTestStatusResult.LS,
                     ModeStatusParameters = ModeStatusParameters.FirstStatusParameters
                  }
                 ,
                 new CalibrationStatusParameters()
                 {
                     StatusStation = ClientContextStationType.I,
                     Corellation = true,
                     TrustOldResults = true,
                     PointForCorrelation = true,
                     ExceptionParameter = true,
                     ResultStationStatus = StationStatusResult.IT,
                     ResultDriveTestStatus = DriveTestStatusResult.LS,
                     ModeStatusParameters = ModeStatusParameters.FirstStatusParameters
                  },
                 new CalibrationStatusParameters()
                 {
                     StatusStation = ClientContextStationType.I,
                     Corellation = true,
                     TrustOldResults = true,
                     PointForCorrelation = true,
                     ExceptionParameter = false,
                     ResultStationStatus = StationStatusResult.IT,
                     ResultDriveTestStatus = DriveTestStatusResult.LS,
                     ModeStatusParameters = ModeStatusParameters.FirstStatusParameters
                  }
                 ,
                 new CalibrationStatusParameters()
                 {
                     StatusStation = ClientContextStationType.I,
                     Corellation = true,
                     TrustOldResults = true,
                     PointForCorrelation = false,
                     ExceptionParameter = true,
                     ResultStationStatus = StationStatusResult.IT,
                     ResultDriveTestStatus = DriveTestStatusResult.LS,
                     ModeStatusParameters = ModeStatusParameters.FirstStatusParameters
                  }
                 ,
                 new CalibrationStatusParameters()
                 {
                     StatusStation = ClientContextStationType.I,
                     Corellation = true,
                     TrustOldResults = true,
                     PointForCorrelation = false,
                     ExceptionParameter = false,
                     ResultStationStatus = StationStatusResult.IT,
                     ResultDriveTestStatus = DriveTestStatusResult.LS,
                     ModeStatusParameters = ModeStatusParameters.FirstStatusParameters
                  }
                  ,
                 new CalibrationStatusParameters()
                 {
                     StatusStation = ClientContextStationType.I,
                     Corellation = true,
                     TrustOldResults = false,
                     PointForCorrelation = true,
                     ExceptionParameter = true,
                     ResultStationStatus = StationStatusResult.IT,
                     ResultDriveTestStatus = DriveTestStatusResult.LS,
                     ModeStatusParameters = ModeStatusParameters.FirstStatusParameters
                  }
                 ,
                 new CalibrationStatusParameters()
                 {
                     StatusStation = ClientContextStationType.I,
                     Corellation = true,
                     TrustOldResults = false,
                     PointForCorrelation = true,
                     ExceptionParameter = false,
                     ResultStationStatus = StationStatusResult.IT,
                     ResultDriveTestStatus = DriveTestStatusResult.LS,
                     ModeStatusParameters = ModeStatusParameters.FirstStatusParameters
                  }
                 ,
                 new CalibrationStatusParameters()
                 {
                     StatusStation = ClientContextStationType.I,
                     Corellation = true,
                     TrustOldResults = false,
                     PointForCorrelation = false,
                     ExceptionParameter = true,
                     ResultStationStatus = StationStatusResult.IT,
                     ResultDriveTestStatus = DriveTestStatusResult.LS,
                     ModeStatusParameters = ModeStatusParameters.FirstStatusParameters
                  }
                  ,
                 new CalibrationStatusParameters()
                 {
                     StatusStation = ClientContextStationType.I,
                     Corellation = true,
                     TrustOldResults = false,
                     PointForCorrelation = false,
                     ExceptionParameter = false,
                     ResultStationStatus = StationStatusResult.IT,
                     ResultDriveTestStatus = DriveTestStatusResult.LS,
                     ModeStatusParameters = ModeStatusParameters.FirstStatusParameters
                  }
                  ,
                 new CalibrationStatusParameters()
                 {
                     StatusStation = ClientContextStationType.I,
                     Corellation = false,
                     TrustOldResults = true,
                     PointForCorrelation = true,
                     ExceptionParameter = true,
                     ResultStationStatus = StationStatusResult.IT,
                     ResultDriveTestStatus = DriveTestStatusResult.LS,
                     ModeStatusParameters = ModeStatusParameters.FirstStatusParameters
                  }
                  ,
                 new CalibrationStatusParameters()
                 {
                     StatusStation = ClientContextStationType.I,
                     Corellation = false,
                     TrustOldResults = true,
                     PointForCorrelation = true,
                     ExceptionParameter = false,
                     ResultStationStatus = StationStatusResult.IT,
                     ResultDriveTestStatus = DriveTestStatusResult.LS,
                     ModeStatusParameters = ModeStatusParameters.FirstStatusParameters
                  }
                 ,
                 new CalibrationStatusParameters()
                 {
                     StatusStation = ClientContextStationType.I,
                     Corellation = false,
                     TrustOldResults = true,
                     PointForCorrelation = false,
                     ExceptionParameter = true,
                     ResultStationStatus = StationStatusResult.IT,
                     ResultDriveTestStatus = DriveTestStatusResult.LS,
                     ModeStatusParameters = ModeStatusParameters.FirstStatusParameters
                  }
                  ,
                 new CalibrationStatusParameters()
                 {
                     StatusStation = ClientContextStationType.I,
                     Corellation = false,
                     TrustOldResults = true,
                     PointForCorrelation = false,
                     ExceptionParameter = false,
                     ResultStationStatus = StationStatusResult.IT,
                     ResultDriveTestStatus = DriveTestStatusResult.LS,
                     ModeStatusParameters = ModeStatusParameters.FirstStatusParameters
                  }
                  ,
                 new CalibrationStatusParameters()
                 {
                     StatusStation = ClientContextStationType.I,
                     Corellation = false,
                     TrustOldResults = false,
                     PointForCorrelation = true,
                     ExceptionParameter = true,
                     ResultStationStatus = StationStatusResult.IT,
                     ResultDriveTestStatus = DriveTestStatusResult.LS,
                     ModeStatusParameters = ModeStatusParameters.FirstStatusParameters
                  }
                  ,
                 new CalibrationStatusParameters()
                 {
                     StatusStation = ClientContextStationType.I,
                     Corellation = false,
                     TrustOldResults = false,
                     PointForCorrelation = true,
                     ExceptionParameter = false,
                     ResultStationStatus = StationStatusResult.IT,
                     ResultDriveTestStatus = DriveTestStatusResult.LS,
                     ModeStatusParameters = ModeStatusParameters.FirstStatusParameters
                  }
                 ,
                 new CalibrationStatusParameters()
                 {
                     StatusStation = ClientContextStationType.I,
                     Corellation = false,
                     TrustOldResults = false,
                     PointForCorrelation = false,
                     ExceptionParameter = true,
                     ResultStationStatus = StationStatusResult.IT,
                     ResultDriveTestStatus = DriveTestStatusResult.LS,
                     ModeStatusParameters = ModeStatusParameters.FirstStatusParameters
                  }
                  ,
                 new CalibrationStatusParameters()
                 {
                     StatusStation = ClientContextStationType.I,
                     Corellation = false,
                     TrustOldResults = false,
                     PointForCorrelation = false,
                     ExceptionParameter = false,
                     ResultStationStatus = StationStatusResult.IT,
                     ResultDriveTestStatus = DriveTestStatusResult.LS,
                     ModeStatusParameters = ModeStatusParameters.FirstStatusParameters
                  }
            };
    }
}
