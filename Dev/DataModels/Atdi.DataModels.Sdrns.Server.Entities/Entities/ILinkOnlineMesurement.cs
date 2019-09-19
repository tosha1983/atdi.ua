using Atdi.DataModels.EntityOrm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrns.Server.Entities
{
    [EntityPrimaryKeyAttribute]
    public interface ILinkOnlineMesurement_PK
    {
        long Id { get; set; }
    }

    [Entity]
    public interface ILinkOnlineMesurement : ILinkOnlineMesurement_PK
    {
      IOnlineMesurement ONLINE_MEAS { get; set; }
      long OnlineMesurementMasterId { get; set; }
    }
}
