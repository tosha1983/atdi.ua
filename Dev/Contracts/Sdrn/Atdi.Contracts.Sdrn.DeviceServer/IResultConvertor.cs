using Atdi.DataModels.Sdrn.DeviceServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Contracts.Sdrn.DeviceServer
{
    public interface IResultConvertor
    {
        ICommandResultPart Convert(ICommandResultPart result, ICommand command);
    }

    public interface IResultConvertor<TFrom, TResult>
        where TFrom : ICommandResultPart
        where TResult : ICommandResultPart
    {
        TResult Convert(TFrom result, ICommand command);
    }
}
