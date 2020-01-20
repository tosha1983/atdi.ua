using System;
using System.Collections.Concurrent;
using System.Threading;

namespace Atdi.Platform.Data
{
    public sealed class ObjectPool<T> : IObjectPool<T>
    {
        private readonly ObjectPoolDescriptor<T> _descriptor;
        private readonly ConcurrentQueue<T> _pool;
        private readonly T[] _table;
        private int _count;
        private int _created;

        public ObjectPool(ObjectPoolDescriptor<T> descriptor)
        {
            this._descriptor = descriptor ?? throw new ArgumentNullException(nameof(descriptor));
            if (descriptor.MinSize < 0)
            {
                throw new ArgumentException($"Incorrect the MinSize value '{descriptor.MinSize.ToString()}'", nameof(descriptor));
            }
            if (descriptor.MaxSize <= 0)
            {
                throw new ArgumentException($"Incorrect the MaxSize value '{descriptor.MinSize.ToString()}'", nameof(descriptor));
            }
            if (descriptor.MinSize > descriptor.MaxSize)
            {
                throw new ArgumentException($"Incorrect the MaxSize value '{descriptor.MaxSize.ToString()}.");
            }

            if (_descriptor.Factory == null)
            {
                throw new ArgumentException("Undefined an object factory", nameof(descriptor));
            }
            this._table = new T[descriptor.MaxSize];
            this._pool = new ConcurrentQueue<T>();
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
                _pool.Enqueue(item);
                _table[i] = item;
            }

            _count = _descriptor.MinSize;
            _created = _count;
        }

        public ObjectPoolDescriptor<T> Descriptor => _descriptor;

        public bool TryTake(out T item)
        {
            if (_pool.TryDequeue(out item))
            {
                Interlocked.Decrement(ref _count);
                return true;
            }

            var currentCreated = Interlocked.Increment(ref _created);
            if (_created > _descriptor.MaxSize)
            {
                Interlocked.Decrement(ref _created);
                return false;
            }

            try
            {
                item = this.CreateInstance();
                _table[currentCreated - 1] = item;
                return true;
            }
            catch (Exception e)
            {
                Interlocked.Decrement(ref _count);
                throw new InvalidOperationException($"Failed to create object of type '{typeof(T).AssemblyQualifiedName}'", e);
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
                System.Diagnostics.Debug.Assert(false);
                return;
            }
            _pool.Enqueue(item);
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

        public void Dispose()
        {
            try
            {
                foreach (var item in _table)
                {
                    if (item is IDisposable disposable)
                    {
                        try
                        {
                            disposable.Dispose();
                        }
                        catch (Exception e)
                        {
                            System.Diagnostics.Debug.Fail($"ObjectPool<{typeof(T).Name}>.Dispose: Exception", e.ToString());
                        }
                        
                    }
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.Fail($"ObjectPool<{typeof(T).Name}>.Dispose: Exception", e.ToString());
            }
        }
    }
}