using Atdi.DataModels.WebQuery;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.AppServices.WebQuery
{
    public sealed class GroupDescriptor
    {
        public int Id { get; set; }

        public QueryGroup Group { get; set; }

        public QueryTokenDescriptor[] QueryTokens { get; set; }
    }
}
