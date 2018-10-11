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
    public interface IAntennaBase
    {
        int Id { get; }

        string Name { get; set; }

        Types.IAntennaType TYPE { get; }
    }
}
