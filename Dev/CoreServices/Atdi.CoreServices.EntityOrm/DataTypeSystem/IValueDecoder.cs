using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.CoreServices.EntityOrm
{
    public interface IValueDecoder : IValueAdapter
    {

        object Decode(object storeValue);

        object Decode(IDataReader dataReader, int ordinal);

    }

    public interface IValueTypeDecoder<TCodeType> : IValueDecoder
    {

        TCodeType DecodeAs(object storeValue);

        TCodeType DecodeAs(IDataReader dataReader, int ordinal);

    }
}
