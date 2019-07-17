using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.EntityOrm.Api
{
    public interface IDataRecordRequest : IEntityRequest
    {
        string PrimaryKey { get; set; }

        string[] Select { get; set; }

        string[] Filter { get; set; }
    }
}
