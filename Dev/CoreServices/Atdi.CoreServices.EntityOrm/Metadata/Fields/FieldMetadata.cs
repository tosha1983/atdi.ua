using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Atdi.Contracts.CoreServices.EntityOrm.Metadata
{
    [Serializable]
    public class FieldMetadata : IFieldMetadata
    {
        public FieldMetadata(IEntityMetadata entityMetadata, string name)
            : this(entityMetadata, name, FieldSourceType.Column)
        {
        }

        public FieldMetadata(IEntityMetadata entityMetadata, string name, FieldSourceType sourceType)
        {
            this.Entity = entityMetadata;
            this.Name = name;
            this.SourceType = sourceType;
        }

        public string Name { get; }

        public string Title { get; set; }

        public string Desc { get; set; }

        public bool Required { get; set; }

        public FieldSourceType SourceType { get;  }

        public IDataTypeMetadata DataType { get; set; }

        public IUnitMetadata Unit { get; set; }

        public string SourceName { get; set; }

        public IEntityMetadata Entity { get; }

        public override string ToString()
        {
            return $"{Name}: SourceType = '{this.SourceType}', SourceName = {this.SourceName}, DataType = {DataType?.Name}";
        }
    }
}
