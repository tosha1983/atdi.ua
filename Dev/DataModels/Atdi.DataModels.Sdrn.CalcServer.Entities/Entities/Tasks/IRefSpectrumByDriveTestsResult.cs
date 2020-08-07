using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.DataModels.EntityOrm;

namespace Atdi.DataModels.Sdrn.CalcServer.Entities.Tasks
{
	[EntityPrimaryKey]
	public interface IRefSpectrumByDriveTestsResult_PK
    {
        long Id { get; set; }
	}

	[Entity]
	public interface IRefSpectrumByDriveTestsResult : IRefSpectrumByDriveTestsResult_PK
    {
		ICalcResult RESULT { get; set; }
        IRefSpectrumByDriveTestsArgs PARAMETERS { get; set; }
        DateTimeOffset DateCreated { get; set; }
    }

}