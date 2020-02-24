using Atdi.DataModels.EntityOrm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrn.CalcServer.Entities
{
    public class CalcServerEntityOrmContext : EntityOrmContext
	{
		public CalcServerEntityOrmContext() 
			: base("CalcServer")
		{
		}
	}
}
