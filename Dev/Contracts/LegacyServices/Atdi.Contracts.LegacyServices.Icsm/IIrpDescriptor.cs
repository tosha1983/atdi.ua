using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.DataModels.WebQuery;

namespace Atdi.Contracts.LegacyServices.Icsm
{
    public sealed class IrpDescriptor 
    {
       public List<IrpColumn> irpColumns { get; set; }
       public string TableName { get; set; }
       public string[] PrimaryKey { get; set; }
       public ColumnProperties[] MandatoryColumns { get; set; }
       public ColumnProperties[] PrimaryColumns { get; set; }
    }
}
