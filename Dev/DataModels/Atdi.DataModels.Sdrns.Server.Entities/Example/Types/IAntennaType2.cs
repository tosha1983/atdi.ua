using Atdi.DataModels.EntityOrm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrns.Server.Entities.Types
{
    [Entity]
    public interface IAntennaType2
    {
        int Id2 { get; }

        string Name2 { get; set; }

    }
}
