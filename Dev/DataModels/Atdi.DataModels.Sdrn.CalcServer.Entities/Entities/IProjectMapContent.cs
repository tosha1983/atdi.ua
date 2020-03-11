using Atdi.DataModels.EntityOrm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrn.CalcServer.Entities
{
	[EntityPrimaryKey]
	public interface IProjectMapContent_PK
	{
		long Id { get; set; }
	}
	[Entity]
	public interface IProjectMapContent : IProjectMapContent_PK
	{
		IProjectMap MAP { get; set; }

		DateTimeOffset CreatedDate { get; set; }

		byte TypeCode { get; set; }

		string TypeName { get; set; }


		string StepDataType { get; set; }

		byte StepDataSize { get; set; }

		int? SourceCount { get; set; }

		decimal? SourceCoverage { get; set; }

		int ContentSize { get; set; }

		string ContentType { get; set; }

		string ContentEncoding { get; set; }

		byte[] Content { get; set; }

	}

	
}
