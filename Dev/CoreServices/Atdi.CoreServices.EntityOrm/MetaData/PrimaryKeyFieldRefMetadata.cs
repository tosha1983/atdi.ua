using Atdi.DataModels.DataConstraint;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.Contracts.CoreServices.EntityOrm.Metadata;

namespace Atdi.CoreServices.EntityOrm.Metadata
{
    public class PrimaryKeyFieldRefMetadata : IPrimaryKeyFieldRefMetadata
    {
        public IPrimaryKeyFieldRefMetadata Field { get; set; }

        public SortDirection SortOrder { get; set; }
    }
}
