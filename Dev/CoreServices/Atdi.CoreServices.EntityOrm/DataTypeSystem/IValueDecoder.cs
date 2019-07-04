using Atdi.Contracts.CoreServices.DataLayer;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.CoreServices.EntityOrm
{
    /// <summary>
    /// Раскодировщик значения из типа хранилища
    /// </summary>
    public interface IValueDecoder : IValueAdapter
    {

        object Decode(object storeValue);

        object Decode(IEngineDataReader dataReader, int ordinal);

    }

    /// <summary>
    /// Раскодировщик значения из типа хранилища
    /// </summary>
    public interface IValueTypeDecoder<TCodeType> : IValueDecoder
    {

        TCodeType DecodeAs(object storeValue);

        TCodeType DecodeAs(IEngineDataReader dataReader, int ordinal);
    }
}
