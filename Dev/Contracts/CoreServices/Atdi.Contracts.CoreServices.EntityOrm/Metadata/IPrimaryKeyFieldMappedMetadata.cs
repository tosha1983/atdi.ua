using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Contracts.CoreServices.EntityOrm.Metadata
{
    public interface IPrimaryKeyFieldMappedMetadata
    {
        IFieldMetadata KeyField { get; set; }

        PrimaryKeyMappedMatchWith MatchWith { get; set; }
    }

    public interface IValuePrimaryKeyFieldMappedMetadata : IPrimaryKeyFieldMappedMetadata
    {
        object Value { get; set; }
    }

    public interface IFieldPrimaryKeyFieldMappedMetadata : IPrimaryKeyFieldMappedMetadata
    {
        IFieldMetadata EntityField { get; set; }
    }

    public interface ISourceNamePrimaryKeyFieldMappedMetadata : IPrimaryKeyFieldMappedMetadata
    {
        string SourceName { get; set; }
    }
}
