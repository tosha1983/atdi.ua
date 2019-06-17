using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Atdi.Contracts.CoreServices.EntityOrm.Metadata
{
    [Serializable]
    public class EntityMetadata : IEntityMetadata
    {
        public string Name { get; set; }

        public string Title { get; set; }

        public string Desc { get; set; }

        public EntityType Type { get; set; }

        public IEntityMetadata BaseEntity { get; set; }

        public IDataSourceMetadata DataSource { get; set; }

        public IPrimaryKeyMetadata PrimaryKey { get; set; }

        public IReadOnlyDictionary<string, IFieldMetadata> Fields { get; set; }

        public string QualifiedName { get; set; }

        public override string ToString()
        {
            return $"{this.Name}: Type = '{Type}', Base = '{BaseEntity?.QualifiedName}', Count fields = {Fields.Count}";
        }
    }
}
