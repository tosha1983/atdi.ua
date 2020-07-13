using Atdi.DataModels.EntityOrm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Brific.Entities
{
    public class BrificEntityOrmContext : EntityOrmContext
	{
		public BrificEntityOrmContext() 
			: base("Brific")
		{
		}
	}
}
