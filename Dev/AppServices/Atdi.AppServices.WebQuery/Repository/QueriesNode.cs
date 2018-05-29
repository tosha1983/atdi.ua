using Atdi.DataModels.WebQuery;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.AppServices.WebQuery
{
    internal sealed class QueriesNode
    {
        public int NodeId { get; set; }

        public QueryToken QueryToken { get; set; }

        public string Name { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public QueriesNode[] Nodes { get; set; }
    }
}
