using Atdi.Contracts.CoreServices.EntityOrm.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.CoreServices.EntityOrm
{
    public sealed class FieldReferenceDescriptor
    {
        // очень важно хранить пару  - сущность и поле, так как поле может быть получено по наследству от базовой сущности
        public IEntityMetadata Entity { get; set; }

        public IFieldMetadata Field { get; set; }

        public FieldReferenceDescriptor Next { get; set; }

        public FieldReferenceDescriptor Prev { get; set; }

        public string Path { get; set; }

        public override string ToString()
        {
            var result = $"[{Entity.Name}.{Field.Name}]";
            if (this.Next == null)
            {
                return result;
            }
            return $"{result} => {this.Next}";
        }
    }
}
