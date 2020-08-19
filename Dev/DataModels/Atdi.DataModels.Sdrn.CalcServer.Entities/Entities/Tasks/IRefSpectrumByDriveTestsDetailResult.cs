using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.DataModels.EntityOrm;

namespace Atdi.DataModels.Sdrn.CalcServer.Entities.Tasks
{
	[EntityPrimaryKey]
	public interface IRefSpectrumByDriveTestsDetailResult_PK
    {
        long Id { get; set; }
	}

	[Entity]
	public interface IRefSpectrumByDriveTestsDetailResult : IRefSpectrumByDriveTestsDetailResult_PK
    {
        int OrderId { get; set; }
        string TableIcsmName { get; set; }
        int IdIcsm { get; set; }
        long IdSensor { get; set; }
        string GlobalCID { get; set; }
        double Freq_MHz { get; set; }
        double Level_dBm { get; set; }
        double Percent { get; set; }
        DateTime DateMeas { get; set; }
        IRefSpectrumByDriveTestsResult RESULT_REF_SPECTRUM { get; set; }
    }

}