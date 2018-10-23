using Atdi.DataModels.EntityOrm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrns.Server.Entities
{
    [Entity]
    public interface IAntennaExten2
    {

        string ShortName2 { get; set; }

        string FullName2 { get; set; }

        string PosType2 { get; set; }

        IAntenna EXTENDED { get; set; }
    }
}
