using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Contracts.Sdrn.DeviceServer
{
    public interface IResultHandlersHost
    {
        void Register(Type handlerInstanceType);

        IResultHandler GetHandler(Type commandType, Type resultType, Type taskType, Type processType);
    }
}
