using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.DataModels.EntityOrm;
using Atdi.DataModels;

namespace Atdi.DataModels.Sdrns.Server.Entities
{
    [Entity]
    public interface IAntenna : IAntennaBase
    {
        double? FrequencyMHz { get; set; }

        IAntennaExten1 EXT1 { get; }

        IAntennaPosition POS { get; }

        DataType? Prop3DataType { get; }

        DataType[] DataTypeArray { get; }
    }
}
