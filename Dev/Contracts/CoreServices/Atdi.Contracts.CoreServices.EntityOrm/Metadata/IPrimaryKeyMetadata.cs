using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Contracts.CoreServices.EntityOrm.Metadata
{
    public interface IPrimaryKeyMetadata
    {
        bool? Clustered { get; }

        IReadOnlyDictionary<string, IPrimaryKeyFieldRefMetadata> FieldRefs { get; }
    }
}
