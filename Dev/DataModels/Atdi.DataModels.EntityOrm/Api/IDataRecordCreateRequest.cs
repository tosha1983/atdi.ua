using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.EntityOrm.Api
{
	public interface IDataRecordCreateRequest : IEntityRequest
	{
		string[] Fields { get; set; }

		object[] Values { get; set; }
	}

	public interface IDataRecordApplyRequest : IEntityRequest
	{
		string[] Filter { get; set; }

		string[] FieldsToCreate { get; set; }

		string[] FieldsToUpdate { get; set; }

		object[] ValuesToCreate { get; set; }

		object[] ValuesToUpdate { get; set; }
	}

	public interface IDataRecordUpdateRequest : IEntityRequest
	{
		string PrimaryKey { get; set; }

		string[] Fields { get; set; }

		object[] Values { get; set; }
	}

	public interface IDataRecordDeleteRequest : IEntityRequest
	{
		string PrimaryKey { get; set; }
		
	}
}
