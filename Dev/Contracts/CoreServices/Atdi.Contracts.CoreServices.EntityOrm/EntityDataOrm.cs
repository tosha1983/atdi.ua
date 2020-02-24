using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.Contracts.CoreServices.DataLayer;
using Atdi.DataModels.EntityOrm;

namespace Atdi.Contracts.CoreServices.EntityOrm
{
    public class EntityDataOrm : DataOrmBase
    {
        public EntityDataOrm() : base("Entity ORM")
        {
            
        }
        public EntityDataOrm(string context) : base("Entity ORM", context)
        {

        }
	}

    public sealed class EntityDataOrm<TContext> : EntityDataOrm
		where TContext : EntityOrmContext, new()
	{
	    public EntityDataOrm() : base( new TContext().ContextName)
	    {

	    }
    }
}
