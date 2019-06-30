using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Atdi.Contracts.CoreServices.EntityOrm.Metadata
{
    [Serializable]
    public abstract class FieldMetadata : IFieldMetadata
    {

        public FieldMetadata(IEntityMetadata entity, IEntityMetadata baseEntity, string name, FieldSourceType sourceType)
        {
            this.Entity = entity;
            this.BaseEntity = baseEntity;
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

        public IEntityMetadata BaseEntity { get; }

        public IFieldDefaultMetadata Default { get; set; }

        public override string ToString()
        {
            return $"Name = '{Name}': SourceType = '{this.SourceType}', SourceName = {this.SourceName}, DataType = {DataType?.Name}, Entity = '{Entity.Name}'";
        }

        public abstract FieldMetadata CopyTo(IEntityMetadata owner);
    }
}
