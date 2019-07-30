using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Platform.Caching
{
    class DataCacheDescriptor<TKey, TData> : IDataCacheDescriptor<TKey, TData>
    {
        public DataCacheDescriptor(string name, DataCacheOptions options)
        {
            this.Name = name;
            this.KeyType = typeof(TKey);
            this.DataType = typeof(TData);
            this.Options = options;
        }

        public string Name { get; }

        public Type DataType { get; }

        public Type KeyType { get; }

        public DataCacheOptions Options { get; }

        public override bool Equals(object obj)
        {
            return this.Name.Equals(obj);
        }

        public override int GetHashCode()
        {
            return this.Name.GetHashCode();
        }

        public override string ToString()
        {
            return this.Name.ToString();
        }
    }

}
