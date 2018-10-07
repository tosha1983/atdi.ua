using Atdi.DataModels.EntityOrm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrns.Server.Entities
{
    [Entity]
    public interface IAntennaPosition
    {
        int Id { get; }

        string TableName { get; set; }

        string PosType { get; set; }

        double? PosX { get; set; }

        double? PosY { get; set; }
    }
}
