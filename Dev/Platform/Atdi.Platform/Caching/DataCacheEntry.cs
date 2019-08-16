using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Platform.Caching
{
    class DataCacheEntry
    {
        public long Hits;

        public long ElapsedMilliseconds;
        
        public TData GetData<TData>()
        {
            return ((DataCacheEntry<TData>)this).Data;
        }
    }

    class DataCacheEntry<TData> : DataCacheEntry
    {
        public TData Data;

        public override string ToString()
        {
            return $"Hits = {Hits}, Elapsed = {ElapsedMilliseconds}, Data = '{Data}'";
        }
    }
}
