using Atdi.DataModels.EntityOrm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrns.Server.Entities
{
    [Entity]
    public interface IStationSite
    {
        int Id { get; set; }
        double? Lon { get; set; }
        double Lat { get; set; }
        string Address { get; set; }
        string Region { get; set; }
    }
}
