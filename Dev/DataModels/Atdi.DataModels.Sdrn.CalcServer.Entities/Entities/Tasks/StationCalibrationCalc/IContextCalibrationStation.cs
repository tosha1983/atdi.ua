using Atdi.DataModels.EntityOrm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrn.CalcServer.Entities
{
    [EntityPrimaryKey]
    public interface IContextCalibrationStation_PK
    {
        long Id { get; set; }
    }
    [Entity]
    public interface IContextCalibrationStation : IContextStation, IContextCalibrationStation_PK
    {
        long? IdStation { get; set; }
        string TableName { get; set; }
        DateTimeOffset? ModifiedDate { get; set; }
    }
}



