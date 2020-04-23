using Atdi.DataModels.EntityOrm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrn.Infocenter.Entities
{
    [EntityPrimaryKeyAttribute]
    public interface ICluttersDescClutter_PK
	{
        long CluttersDescId { get; set; }

        byte Code { get; set; }
	}

    [Entity]
    public interface ICluttersDescClutter : ICluttersDescClutter_PK
	{

        ICluttersDesc CLUTTERS_DESC { get; set; }

		string Name { get; set; }

		string Note { get; set; }

		long Height_m { get; set; }
	}
}
