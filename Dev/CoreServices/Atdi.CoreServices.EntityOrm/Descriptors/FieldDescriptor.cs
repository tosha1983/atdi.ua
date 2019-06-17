using Atdi.Contracts.CoreServices.EntityOrm.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.CoreServices.EntityOrm
{
    public sealed class FieldDescriptor
    {
        public FieldReferenceDescriptor Reference { get; set; }
        // очень важно хранить пару  - сущность и поле, так как поле может быть получено по наследству от базовой сущности
        public IEntityMetadata Entity { get; set; }

        public IFieldMetadata Field { get; set; }

        public string Path { get; set; }

        public string Alias { get; set; }

        public override string ToString()
        {
            return $"Field = '{Field.Name}', Path = '{this.Path}', Entity = '{Field.Entity.QualifiedName}', IsRefrence = {this.Reference != null} ";
        }
    }
}
