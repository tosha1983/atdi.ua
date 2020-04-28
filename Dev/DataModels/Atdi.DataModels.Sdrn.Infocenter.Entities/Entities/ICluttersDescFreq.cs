using Atdi.DataModels.EntityOrm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrn.Infocenter.Entities
{
    [EntityPrimaryKey]
    public interface ICluttersDescFreq_PK
	{
        long Id { get; set; }
	}

    [Entity]
    public interface ICluttersDescFreq : ICluttersDescFreq_PK
	{
		ICluttersDesc CLUTTERS_DESC { get; set; }

		double Freq_MHz { get; set; }

		string Note { get; set; }
	}
}
