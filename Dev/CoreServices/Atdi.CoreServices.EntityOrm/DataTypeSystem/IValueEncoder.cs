using Atdi.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.CoreServices.EntityOrm
{
    /// <summary>
    /// Кодировщик значения в тип хранилища
    /// </summary>
    public interface IValueEncoder : IValueAdapter
    {
        object Encode(ColumnValue columnValue);
    }

    /// <summary>
    /// Кодировщик значения в тип хранилища
    /// </summary>
    public interface IValueTypedEncoder<TStoreType> : IValueEncoder
    {
        TStoreType EncodeAs(ColumnValue columnValue);
    }
}
