using Atdi.DataModels.EntityOrm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrn.Infocenter.Entities
{
    public class InfocenterEntityOrmContext : EntityOrmContext
	{
		public InfocenterEntityOrmContext() 
			: base("Infocenter")
		{
		}
	}
}
