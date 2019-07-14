using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.EntityOrm.Api
{
    public interface IDataSetRequest : IEntityRequest
    {
        string[] Select { get; set; }

        string[] OrderBy { get; set; }

        string[] Filter { get; set; }

        long Top { get; set; }
    }
}
