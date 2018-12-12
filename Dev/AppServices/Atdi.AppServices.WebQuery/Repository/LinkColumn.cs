using Atdi.DataModels.WebQuery;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.AppServices.WebQuery
{
    public sealed class LinkColumn
    {
        public string TableName { get; set; }
        public string LinkFieldName { get; set; }
        public object ValueLinkId { get; set; }
        public Type TypeColumn { get; set; }
        public string FullSourceName { get; set; }
        public bool IsUpdated { get; set; }
        public bool IsMandatory { get; set; }
    }
}

