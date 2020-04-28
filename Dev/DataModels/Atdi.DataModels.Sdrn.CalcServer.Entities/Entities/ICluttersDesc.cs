﻿using Atdi.DataModels.EntityOrm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrn.CalcServer.Entities
{
    [EntityPrimaryKey]
    public interface ICluttersDesc_PK
	{
        long Id { get; set; }
    }

    [Entity]
    public interface ICluttersDesc : ICluttersDesc_PK
	{
		long InfocDescId { get; set; }

		IProjectMap MAP { get; set; }

        DateTimeOffset CreatedDate { get; set; }

        string Name { get; set; }

        string Note { get; set; }
	}
}
