using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.DataModels.WebQuery;


namespace Atdi.Contracts.CoreServices.DataLayer
{
    public interface IIrpParser
    {
        ColumnMetadata[] ExecuteParseQuery(byte[] value);
        ColumnMetadata[] ExecuteParseQuery(string value);
    }
}
