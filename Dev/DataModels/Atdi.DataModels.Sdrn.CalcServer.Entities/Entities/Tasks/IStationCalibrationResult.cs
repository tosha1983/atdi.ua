using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.DataModels.EntityOrm;

namespace Atdi.DataModels.Sdrn.CalcServer.Entities.Tasks
{
	[EntityPrimaryKey]
	public interface IStationCalibrationResult_PK
    {
        long Id { get; set; }
	}

	[Entity]
	public interface IStationCalibrationResult : IStationCalibrationResult_PK
    {
		ICalcResult RESULT { get; set; }
        IStationCalibrationArgs PARAMETERS { get; set; }
        //long ParametersId { get; set; }
        DateTimeOffset TimeStart { get; set; }
        string Standard { get; set; }
        string AreaName { get; set; }
        int NumberStation { get; set; }
        int NumberStationInContour { get; set; }
        int CountStation_CS { get; set; }
        int CountStation_NS { get; set; }
        int CountStation_IT { get; set; }
        int CountStation_NF { get; set; }
        int CountStation_UN { get; set; }
        int CountMeasGSID { get; set; }
        int CountMeasGSID_LS { get; set; }
        int CountMeasGSID_IT { get; set; }
        int PercentComplete { get; set; }

    }

}