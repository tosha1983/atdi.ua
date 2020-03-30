using Atdi.DataModels.EntityOrm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrn.CalcServer.Entities
{
	[EntityPrimaryKey]
	public interface IClientContextDucting_PK
	{
		long ContextId { get; set; }
	}
	[Entity]
	public interface IClientContextDucting : IClientContextDucting_PK
	{
		bool Available { get; set; }
	}
	
}
