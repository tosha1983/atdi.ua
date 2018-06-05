using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.Contracts.CoreServices.Identity;
using Atdi.DataModels;
using Atdi.DataModels.DataConstraint;
using Atdi.DataModels.WebQuery;

namespace Atdi.AppServices.WebQuery
{
    internal sealed class QueryDescriptor
    {
        public QueryMetadata Metadata { get; set; }
       
        
        public bool CheckActionRight (UserTokenData tokenData, ActionType actionType)
        {
            return true;
        }

        public Condition[] GetConditions(UserTokenData tokenData, ActionType actionType)
        {
            return new Condition[] { };
        }
    }
}
