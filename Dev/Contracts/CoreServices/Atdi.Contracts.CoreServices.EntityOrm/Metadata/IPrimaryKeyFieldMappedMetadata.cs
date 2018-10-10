using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Contracts.CoreServices.EntityOrm.Metadata
{
    public interface IPrimaryKeyFieldMappedMetadata
    {
        IFieldMetadata KeyField { get; }

        PrimaryKeyMappedMatchWith MatchWith { get; }
    }

    public interface IValuePrimaryKeyFieldMappedMetadata : IPrimaryKeyFieldMappedMetadata
    {
        object Value { get; }
    }

    public interface IFieldPrimaryKeyFieldMappedMetadata : IPrimaryKeyFieldMappedMetadata
    {
        IFieldMetadata EntityField { get; }
    }

    public interface ISourceNamePrimaryKeyFieldMappedMetadata : IPrimaryKeyFieldMappedMetadata
    {
        string SourceName { get; }
    }
}
