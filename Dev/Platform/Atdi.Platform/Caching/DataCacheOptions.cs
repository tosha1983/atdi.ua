using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Platform.Caching
{
    public struct DataCacheOptions
    {
        public static readonly DataCacheOptions Default = new DataCacheOptions { };

        /// <summary>
        /// Размер кеша
        /// </summary>
        public long Size { get; set; }

        /// <summary>
        /// Период в милисекундах по истачению которых необходимо исключить элемент из кеша после последнего обращения.
        /// Фактически это время актуальности элемента в кэше
        /// </summary>
        public TimeSpan? SlidingExpiration { get; set; }

        /// <summary>
        /// Период в милисекундах по истачению которых необходимо исключить элемент из кеша после попадания элемента в кеш
        /// Фактичесик это время жизни элемента в кеше.
        /// </summary>
        public TimeSpan? AbsoluteExpiration { get; set; }


    }
}
