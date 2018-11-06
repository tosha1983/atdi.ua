using Atdi.DataModels.EntityOrm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrns.Server.Entities2
{
    [Entity]
    public interface IAntennaExten1
    {

        string ShortName { get; set; }

        string FullName { get; set; }

        string PosType { get; set; }

        IAntennaExten2 EXT2 { get; }

        IAntenna EXTENDED { get; set; }
    }
}
