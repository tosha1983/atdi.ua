using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Platform.Caching
{
    public static class DataCacheDefiner
    {
        public static IDataCacheDescriptor<TKey, TData> Define<TKey, TData>(string name, DataCacheOptions options)
        {
            return new DataCacheDescriptor<TKey, TData>(name, options);
        }

        public static IDataCacheDescriptor<TKey, TData> Define<TKey, TData>(string name)
        {
            return new DataCacheDescriptor<TKey, TData>(name, DataCacheOptions.Default);
        }
    }
}
