using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.DataModels.DataConstraint;

namespace Atdi.Contracts.CoreServices.EntityOrm.Metadata
{
    [Serializable]
    public class RelationFieldMetadata : FieldMetadata, IRelationFieldMetadata
    {
        public RelationFieldMetadata(IEntityMetadata entity, string name)
            : base(entity, entity, name, FieldSourceType.Relation)
        {
        }

        public RelationFieldMetadata(IEntityMetadata entity, IEntityMetadata baseEntity, string name) 
            : base(entity, baseEntity, name, FieldSourceType.Relation)
        {
        }

        public IEntityMetadata RelatedEntity { get; set; }

        public Condition RelationCondition { get; set; }

        public RelationJoinType JoinType { get; set; }

        public override FieldMetadata CopyTo(IEntityMetadata owner)
        {
            throw new InvalidOperationException("No support for copying this type of field");
        }
    }
}
