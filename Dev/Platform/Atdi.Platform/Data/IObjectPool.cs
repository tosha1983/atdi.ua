using System;

namespace Atdi.Platform.Data
{
    public interface IObjectPool : IDisposable
    {
        object TakeObject();

        void PutObject(object item);
    }

    public interface IObjectPool<T> : IObjectPool
    {
        ObjectPoolDescriptor<T> Descriptor { get; }

        bool TryTake(out T item);

        T Take();

        void Put(T item);
    }
}
