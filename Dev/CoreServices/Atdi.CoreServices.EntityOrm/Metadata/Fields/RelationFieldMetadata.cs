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
        public RelationFieldMetadata(IEntityMetadata entityMetadata, string name) 
            : base(entityMetadata, name, FieldSourceType.Relation)
        {
        }

        public IEntityMetadata RelatedEntity { get; set; }

        public Condition RelationCondition { get; set; }

        public RelationJoinType JoinType { get; set; }
    }
}
