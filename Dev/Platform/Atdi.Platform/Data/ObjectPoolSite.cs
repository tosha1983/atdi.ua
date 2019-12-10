using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Atdi.Platform.Data
{
    public sealed class ObjectPoolSite : IObjectPoolSite
    {
        private readonly ConcurrentDictionary<string, IObjectPool> _pools;

        public ObjectPoolSite()
        {
            this._pools = new ConcurrentDictionary<string, IObjectPool>();
        }

        private static string BuildPoolKey<T>(string key)
        {
            return BuildPoolKey(key, typeof(T));
        }
        private static string BuildPoolKey(string key, Type type)
        {
            return string.Concat(type.AssemblyQualifiedName, "|", key);
        }

        public IObjectPool<T> Register<T>(ObjectPoolDescriptor<T> descriptor)
        {
            var poolKey = BuildPoolKey<T>(descriptor.Key);
            if (_pools.TryGetValue(poolKey, out var pool))
            {
                return (IObjectPool<T>)pool;
            }

            pool = new ObjectPool<T>(descriptor);
            if (!_pools.TryAdd(poolKey, pool))
            {
                if (!_pools.TryGetValue(poolKey, out pool))
                {
                    throw new InvalidOperationException("Failed to create object pool");
                }
            }

            return (IObjectPool<T>)pool;
        }

        public IObjectPool<T> GetPool<T>(string key)
        {
            var poolKey = BuildPoolKey<T>(key);
            if (_pools.TryGetValue(poolKey, out var pool))
            {
                return (IObjectPool<T>)pool;
            }
            throw new KeyNotFoundException($"There is no pool of objects for the key '{poolKey}'");
        }

        public IObjectPool GetPool(string key, Type type)
        {
            var poolKey = BuildPoolKey(key, type);
            if (_pools.TryGetValue(poolKey, out var pool))
            {
                return pool;
            }
            throw new KeyNotFoundException($"There is no pool of objects for the key '{poolKey}'");
        }
    }
}