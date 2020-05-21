﻿using Atdi.DataModels.EntityOrm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrns.Server.Entities
{
    public class SdrnServerEntityOrmContext : EntityOrmContext
	{
		public SdrnServerEntityOrmContext() 
			: base("SdrnServer")
		{
		}
	}
}