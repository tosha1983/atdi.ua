using Atdi.DataModels.EntityOrm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrns.Server.Entities
{
    [Entity]
    public interface IResLevMeasOnline
    {
        int Id { get; set; }
        double? Value { get; set; }
        int? ResMeasId { get; set; }
        IResMeas RESMEAS { get; set; }
    }
}
