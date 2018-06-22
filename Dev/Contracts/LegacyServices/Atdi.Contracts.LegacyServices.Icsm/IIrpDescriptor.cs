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
       public List<KeyValuePair<ColumnMetadata, IrpColumn>> columnMetaData { get; set; }
       public string TableName { get; set; }
     
    }
}
