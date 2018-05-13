using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Platform.Logging.DataConvertors
{
    public sealed class SimpleDataConvertor : IEventDataConvertor
    {
        public string Convert<T>(T value)
        {
            return value.ToString();
        }
    }
}
