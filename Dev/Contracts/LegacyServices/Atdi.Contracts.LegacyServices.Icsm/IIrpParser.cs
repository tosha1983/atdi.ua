using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.DataModels.WebQuery;


namespace Atdi.Contracts.LegacyServices.Icsm
{
    public interface IIrpParser
    {
        IrpDescriptor ExecuteParseQuery(byte[] value);
        IrpDescriptor ExecuteParseQuery(string value);
    }
}
