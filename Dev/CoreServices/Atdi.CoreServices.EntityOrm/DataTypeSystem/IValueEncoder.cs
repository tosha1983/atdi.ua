using Atdi.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.CoreServices.EntityOrm
{
    public interface IValueEncoder : IValueAdapter
    {
        object Encode(ColumnValue columnValue);
    }
    public interface IValueTypedEncoder<TStoreType> : IValueEncoder
    {
        TStoreType EncodeAs(ColumnValue columnValue);
    }
}
