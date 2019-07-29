using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Platform.Caching
{
    public interface IDataCacheDescriptor
    {
        string Name { get; }

        Type DataType { get; }

        Type KeyType { get; }

        DataCacheOptions Options { get; }
    }

    public interface IDataCacheDescriptor<TKey, TData> : IDataCacheDescriptor
    {

    }

    
}
