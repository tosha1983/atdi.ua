using Atdi.DataModels.DataConstraint;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Contracts.CoreServices.EntityOrm.Metadata
{
    public interface IPrimaryKeyFieldRefMetadata
    {
        IPrimaryKeyFieldRefMetadata Field { get; set; }

        SortDirection SortOrder { get; set; }
    }
}
