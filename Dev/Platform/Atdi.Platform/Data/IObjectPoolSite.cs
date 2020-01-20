using System;

namespace Atdi.Platform.Data
{
    public interface IObjectPoolSite : IDisposable
    {
        IObjectPool<T> Register<T>(ObjectPoolDescriptor<T> descriptor);

        IObjectPool<T> GetPool<T>(string key);

        IObjectPool GetPool(string key, Type type);
    }
}