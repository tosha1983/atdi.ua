using Atdi.DataModels.EntityOrm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrn.CalcServer.Entities
{
    [EntityPrimaryKeyAttribute]
    public interface ICluttersDescFreqClutter_PK
	{
        long FreqId { get; set; }

        byte Code { get; set; }
	}

    [Entity]
    public interface ICluttersDescFreqClutter : ICluttersDescFreqClutter_PK
	{
		ICluttersDescFreq FREQ { get; set; }

		float? LinearLoss_dBkm { get; set; }

		float? FlatLoss_dB { get; set; }

		float? Reflection { get; set; }
		
		string Note { get; set; }
		
	}
}
