using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.DataModels.WebQuery;


namespace Atdi.Contracts.LegacyServices.Icsm
{
    public sealed class IrpCustomExpression
    {
       public string CustomExpression { get; set; }
       public string Name { get; set; }
       public string Title { get; set; }
    }
}
