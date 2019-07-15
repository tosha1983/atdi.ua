using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.EntityOrm.Api
{
    public interface IFieldValueRequest : IEntityRequest
    {
        string PrimaryKey { get; set; }

        string FieldPath { get; set; }

        string[] Filter { get; set; }
    }
}
