using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Atdi.Contracts.CoreServices.EntityOrm.Metadata
{
    public class FieldMetadata : IFieldMetadata
    {
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
