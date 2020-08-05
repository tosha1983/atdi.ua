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
        long RefSpectrumResultId { get; set; }
        long OrderId { get; set; }
        string TableIcsmName { get; set; }
        long IdIcsm { get; set; }
        long IdSensor { get; set; }
        string GlobalCID { get; set; }
        double Freq_MHz { get; set; }
        float Level_dBm { get; set; }
        float Percent { get; set; }
        DateTimeOffset DateMeas { get; set; }
    }

}