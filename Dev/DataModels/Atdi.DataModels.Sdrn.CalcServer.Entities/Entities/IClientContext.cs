using Atdi.DataModels.EntityOrm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrn.CalcServer.Entities
{
	[EntityPrimaryKey]
	public interface IClientContext_PK
	{
		long Id { get; set; }
	}
	[Entity]
	public interface IClientContext : IClientContext_PK
	{
		IProject PROJECT { get; set; }

		string OwnerInstance { get; set; }

		Guid OwnerContextId { get; set; }

		DateTimeOffset CreatedDate { get; set; }

	}

}
