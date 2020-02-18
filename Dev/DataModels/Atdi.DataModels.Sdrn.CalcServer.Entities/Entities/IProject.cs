using Atdi.DataModels.EntityOrm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrn.CalcServer.Entities
{
	[EntityPrimaryKey]
	public interface IProject_PK
	{
		long Id { get; set; }
	}
	[Entity]
	public interface IProject : IProject_PK
	{
		string Name { get; set; }

		string Desc { get; set; }

		string OwnerInstance { get; set; }

		Guid OwnerProjectId { get; set; }

		DateTimeOffset CreatedDate { get; set; }

	}
}
