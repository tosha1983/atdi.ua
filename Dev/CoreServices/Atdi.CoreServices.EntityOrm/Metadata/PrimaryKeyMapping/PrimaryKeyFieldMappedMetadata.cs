using Atdi.DataModels.DataConstraint;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Atdi.Contracts.CoreServices.EntityOrm.Metadata
{
    [Serializable]
    public class PrimaryKeyFieldMappedMetadata : IPrimaryKeyFieldMappedMetadata
    {
        public PrimaryKeyFieldMappedMetadata(IFieldMetadata keyField, PrimaryKeyMappedMatchWith matchWith)
        {
            this.KeyField = keyField;
            this.MatchWith = matchWith;
        }

        public IFieldMetadata KeyField { get; set; }

        public PrimaryKeyMappedMatchWith MatchWith { get; set; }
    }

    [Serializable]
    public class ValuePrimaryKeyFieldMappedMetadata : PrimaryKeyFieldMappedMetadata, IValuePrimaryKeyFieldMappedMetadata
    {
        public ValuePrimaryKeyFieldMappedMetadata(IFieldMetadata keyField)
            : base(keyField, PrimaryKeyMappedMatchWith.Value)
        {
        }

        public ValueOperand Value { get; set; }
    }

    [Serializable]
    public class FieldPrimaryKeyFieldMappedMetadata : PrimaryKeyFieldMappedMetadata, IFieldPrimaryKeyFieldMappedMetadata
    {
        public FieldPrimaryKeyFieldMappedMetadata(IFieldMetadata keyField)
            : base(keyField, PrimaryKeyMappedMatchWith.Field)
        {
        }

        public IFieldMetadata EntityField { get; set; }
    }

    [Serializable]
    public class SourceNamePrimaryKeyFieldMappedMetadata : PrimaryKeyFieldMappedMetadata, ISourceNamePrimaryKeyFieldMappedMetadata
    {
        public SourceNamePrimaryKeyFieldMappedMetadata(IFieldMetadata keyField)
           : base(keyField, PrimaryKeyMappedMatchWith.SourceName)
        {
        }

        public string SourceName { get; set; }

    }
}
