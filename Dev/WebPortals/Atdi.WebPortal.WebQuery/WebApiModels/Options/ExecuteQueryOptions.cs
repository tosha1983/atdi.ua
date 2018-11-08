using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Atdi.WebPortal.WebQuery.WebApiModels.Options
{
    public class ExecuteQueryOptions
    {
        public UserToken UserToken { get; set; }

        public QueryToken QueryToken { get; set; }

        public Guid OptionId { get; set; }

        public DataSetStructure ResultStructure { get; set; }

        public string[] Columns { get; set; }

        public Filter Filter { get; set; }

        public OrderExpression[] Orders { get; set; }

        public DataLimit Limit { get; set; }
    }
}
