using Atdi.DataModels.EntityOrm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrns.Server.Entities.Types
{
    [Entity]
    public interface IAntennaType
    {
        int Id { get; }

        string Name { get; set; }

        Types.IAntennaType2 TYPE2 { get; }
    }
}
