using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.DataModels.EntityOrm;

namespace Atdi.DataModels.Sdrn.CalcServer.Entities.Tasks
{
	[EntityPrimaryKey]
	public interface ICalibrationGeneralResult_PK
    {
		long ResultId { get; set; }
	}

	[Entity]
	public interface ICalibrationGeneralResult : ICalibrationGeneralResult_PK
    {
		ICalcResult RESULT { get; set; }
        DateTimeOffset TimeStart { get; set; }
        string AreaName { get; set; }
        ICalibrationParameters Parameters { get; set; }
        int CountStations { get; set; }
        int CountStationsInContour { get; set; }
        int CountStations_CS { get; set; }
        int CountStations_NS { get; set; }
        int CountStations_IT { get; set; }
        int CountStations_NF { get; set; }
        int CountStations_UN { get; set; }
        int CountGSID { get; set; }
        int CountGSID_LS { get; set; }
        int CountGSID_IT { get; set; }
    }

}