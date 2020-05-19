using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.DataModels.EntityOrm;

namespace Atdi.DataModels.Sdrn.CalcServer.Entities.Tasks
{
	[EntityPrimaryKey]
	public interface ICalibrationStatusParameters_PK
    {
		long TaskId { get; set; }
	}

	[Entity]
	public interface ICalibrationStatusParameters : ICalibrationStatusParameters_PK
    {
        ICalcTask TASK { get; set; }

        string StatusStation { get; set; }

        bool Corellation { get; set; }

        bool TrustOldResults { get; set; }

        bool PointForCorrelation { get; set; }

        bool ExceptionParameter { get; set; }

        string ResultStationStatus { get; set; }

        string ResultDriveTestStatus { get; set; }

        string BlockName { get; set; }
    }
	
}
