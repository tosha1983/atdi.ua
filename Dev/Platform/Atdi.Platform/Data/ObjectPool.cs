using System;
using System.Collections.Concurrent;
using System.Threading;

namespace Atdi.Platform.Data
{
    public sealed class ObjectPool<T> : IObjectPool<T>
    {
        private readonly ObjectPoolDescriptor<T> _descriptor;
        private readonly ConcurrentBag<T> _pool;
        private int _count;
        private int _created;

        public ObjectPool(ObjectPoolDescriptor<T> descriptor)
        {
            this._descriptor = descriptor ?? throw new ArgumentNullException(nameof(descriptor));
            if (_descriptor.MinSize > _descriptor.MaxSize)
            {
                throw new ArgumentException($"Incorrect the MaxSize value '{_descriptor.MaxSize.ToString()}.");
            }

            if (_descriptor.MinSize < 0)
            {
                throw new ArgumentException($"Incorrect the MinSize value '{_descriptor.MinSize.ToString()}'", nameof(descriptor));
            }

            if (_descriptor.Factory == null)
            {
                throw new ArgumentException("Undefined an object factory", nameof(descriptor));
            }

            this._pool = new ConcurrentBag<T>();
            this.Init();
        }

        private T CreateInstance()
        {
            return _descriptor.Factory();
        }
        private void Init()
        {
            for (var i = 0; i < _descriptor.MinSize; i++)
            {
                var item = this.CreateInstance();
                _pool.Add(item);
            }

            _count = _descriptor.MinSize;
            _created = _count;
        }

        public ObjectPoolDescriptor<T> Descriptor => _descriptor;

        public bool TryTake(out T item)
        {
            if (_pool.TryTake(out item))
            {
                Interlocked.Decrement(ref _count);
                return true;
            }

            Interlocked.Increment(ref _created);
            if (_created > _descriptor.MaxSize)
            {
                Interlocked.Decrement(ref _created);
                return false;
            }

            try
            {
                item = this.CreateInstance();
                return true;
            }
            catch (Exception)
            {
                Interlocked.Decrement(ref _count);
                throw;
            }
        }

        public T Take()
        {
            if (this.TryTake(out var item))
            {
                return item;
            }
            throw new MissingPoolObjectsException();
        }
        public void Put(T item)
        {
            Interlocked.Increment(ref _count);
            if (_count > _descriptor.MaxSize)
            {
                Interlocked.Decrement(ref _count);
                return;
            }
            _pool.Add(item);
        }

        public object TakeObject()
        {
            if (this.TryTake(out var item))
            {
                return item;
            }
            throw new MissingPoolObjectsException();
        }
        public void PutObject(object item)
        {
            this.Put((T)item);
        }
        public override string ToString()
        {
            return $"Key='{_descriptor.Key}', Count={_count.ToString()}, Created={_created.ToString()}, MinSize={_descriptor.MinSize.ToString()}, MaxSize={_descriptor.MaxSize.ToString()}, Type='{_descriptor.Type.FullName}'";
        }
    }
}