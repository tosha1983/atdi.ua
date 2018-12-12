using Atdi.DataModels.WebQuery;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.DataModels;

namespace Atdi.AppServices.WebQuery
{
    public sealed class DescriptorColumns
    {
        public int? IdentValue { get; set; }
        public string TableName { get; set; }
        public string FieldJoinFrom { get; set; }
        public List<ColumnValue> listColumnValue { get; set; }
    }
}
