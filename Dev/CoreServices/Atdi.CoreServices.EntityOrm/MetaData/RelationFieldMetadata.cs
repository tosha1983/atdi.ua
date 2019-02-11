﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.DataModels.DataConstraint;

namespace Atdi.Contracts.CoreServices.EntityOrm.Metadata
{
    public class RelationFieldMetadata : IRelationFieldMetadata
    {
        public IEntityMetadata RelatedEntity { get; set; }

        public Condition RelationCondition { get; set; }

        public string Name { get; set; }

        public string Title { get; set; }

        public string Desc { get; set; }

        public bool Required { get; set; }

        public FieldSourceType SourceType { get; set; }

        public IDataTypeMetadata DataType { get; set; }

        public IUnitMetadata Unit { get; set; }

        public string SourceName { get; set; }

    }
}