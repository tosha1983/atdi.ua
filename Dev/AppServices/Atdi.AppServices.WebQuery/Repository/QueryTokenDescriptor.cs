using Atdi.DataModels.WebQuery;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.AppServices.WebQuery
{
    public sealed class QueryTokenDescriptor
    {
        public string Code { get; set; }

        public QueryToken Token { get; set; }

        public GroupDescriptor GroupDescriptor { get; set; }
    }
}
