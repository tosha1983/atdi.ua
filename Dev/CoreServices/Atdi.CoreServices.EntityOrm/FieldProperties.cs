using Atdi.Platform.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.Platform;
using Atdi.Contracts.CoreServices.EntityOrm;
using Atdi.Contracts.CoreServices.EntityOrm.Metadata;

namespace Atdi.CoreServices.EntityOrm
{
    public class FieldProperties
    {
        public string Alias { get; set; }
        public string FieldName { get; set; }
        public string DBFieldName { get; set; }
        public string DBTableName { get; set; }
        public string TableName { get; set; }
        public bool isPrimaryKey { get; set; }
        public FieldSourceType SourceType { get; set; }
        public List<IEntityMetadata> entityMetadataLinks { get; set; }

    }
}
