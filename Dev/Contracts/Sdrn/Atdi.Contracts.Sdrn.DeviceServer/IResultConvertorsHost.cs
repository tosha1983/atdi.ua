using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Contracts.Sdrn.DeviceServer
{
    public interface IResultConvertorsHost
    {
        void Register(Type convertorType);

        IResultConvertor GetConvertor(Type fromType, Type toType);
    }
}
