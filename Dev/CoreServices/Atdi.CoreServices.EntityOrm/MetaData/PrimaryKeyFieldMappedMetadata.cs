using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Atdi.Contracts.CoreServices.EntityOrm.Metadata
{
    public class PrimaryKeyFieldMappedMetadata : IPrimaryKeyFieldMappedMetadata
    {
        public IFieldMetadata KeyField { get; set; }

        public PrimaryKeyMappedMatchWith MatchWith { get; set; }

    }

    public class ValuePrimaryKeyFieldMappedMetadata : IValuePrimaryKeyFieldMappedMetadata
    {
        public object Value { get; set; }

        public IFieldMetadata KeyField { get; set; }

        public PrimaryKeyMappedMatchWith MatchWith { get; set; }
    }

    public class FieldPrimaryKeyFieldMappedMetadata : IFieldPrimaryKeyFieldMappedMetadata
    {
        public IFieldMetadata EntityField { get; set; }

        public IFieldMetadata KeyField { get; set; }

        public PrimaryKeyMappedMatchWith MatchWith { get; set; }
    }

    public class SourceNamePrimaryKeyFieldMappedMetadata : ISourceNamePrimaryKeyFieldMappedMetadata
    {
        public string SourceName { get; set; }

        public IFieldMetadata KeyField { get; set; }

        public PrimaryKeyMappedMatchWith MatchWith { get; set; }
    }
}
