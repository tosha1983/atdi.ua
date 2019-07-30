using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Platform.Caching
{
    public interface IDataCacheSite
    {
        IDataCache<TKey, TData> Ensure<TKey, TData>(IDataCacheDescriptor<TKey, TData> descriptor);
    }
}
