using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Platform.Caching
{
    public interface IDataCache
    {
        /// <summary>
        /// Описатель кэша
        /// </summary>
        IDataCacheDescriptor Descriptor { get; }
    }

    public interface IDataCache<in TKey, TData> : IDataCache
    {
        /// <summary>
        /// Попытка получить элемент из кзша.
        /// </summary>
        /// <param name="key">ключ элемента</param>
        /// <param name="data">значение из кэша</param>
        /// <returns></returns>
        bool TryGet(TKey key, out TData data);

        /// <summary>
        /// Попытка установить значение в кэше
        /// </summary>
        /// <param name="key"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        bool TrySet(TKey key, ref TData data);

        /// <summary>
        ///  Метод добавляет элемент в кэш при условии что такого ключа в кэше еще нет.
        ///  В противном случаи будет сохранено ранее установленное значение.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="data"></param>
        void Set(TKey key, TData data);

        /// <summary>
        /// Удаление элемента из кеша по ключу.
        /// </summary>
        /// <param name="key"></param>
        void Remove(TKey key);
    }
}
