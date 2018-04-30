using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Platform.Logging
{
    /// <summary>
    /// Represent the config of logger 
    /// </summary>
    public interface ILogConfig
    {

        bool IsAllowed(EventLevel level);

        void Disable(EventLevel level);

        void Enable(EventLevel level);

        /// <summary>
        /// Gets a value of property from a configuration by key
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="defaulValue"></param>
        /// <returns></returns>
        object this[string key] { get; set; } 
    }
}
