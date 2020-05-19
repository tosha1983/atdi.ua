using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.DataModels.EntityOrm;

namespace Atdi.DataModels.Sdrn.CalcServer.Entities.Tasks
{
	[EntityPrimaryKey]
	public interface ICalibrationResult_PK
    {
		long ResultId { get; set; }
	}

	[Entity]
	public interface ICalibrationResult : ICalibrationResult_PK
    {
		ICalcResult RESULT { get; set; }
        DateTimeOffset TimeStart { get; set; }
        string AreaName { get; set; }
        ICalibrationParameters Parameters { get; set; }
        int CountStation { get; set; }
        int CountStationInContour { get; set; }
        int CountStation_CS { get; set; }
        int CountStation_NS { get; set; }
        int CountStation_IT { get; set; }
        int CountStation_NF { get; set; }
        int CountStation_UN { get; set; }
        int CountGSID { get; set; }
        int CountGSID_LS { get; set; }
        int CountGSID_IT { get; set; }
    }

}