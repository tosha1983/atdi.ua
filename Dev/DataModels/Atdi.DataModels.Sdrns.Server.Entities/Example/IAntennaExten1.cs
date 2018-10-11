using Atdi.DataModels.EntityOrm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrns.Server.Entities
{
    [Entity]
    public interface IAntennaExten1
    {
        int Id { get; }

        string ShortName { get; set; }

        string FullName { get; set; }

        IAntenna EXTENDED { get; set; }
    }
}
