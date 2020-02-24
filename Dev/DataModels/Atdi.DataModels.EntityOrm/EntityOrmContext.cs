using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.EntityOrm
{
	public abstract class EntityOrmContext
	{
		public string ContextName { get; }

		protected EntityOrmContext(string contextName)
		{
			this.ContextName = contextName;
		}
	}
}
