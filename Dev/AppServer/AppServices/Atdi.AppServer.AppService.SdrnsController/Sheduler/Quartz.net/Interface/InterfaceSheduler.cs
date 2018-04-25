using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.SDNRS.AppServer.Sheduler
{
    interface InterfaceSheduler
    {
        void ShedulerRepeatStart(int CountSeconds);
    }

    interface InterfaceShedulerWithParams
    {
        void ShedulerRepeatStart(int CountSeconds, object val);
    }

    interface InterfaceShedulerMeasTask
    {
        void ShedulerRepeatStart(int CountSeconds, object val_MeasTask, List<int> SensorIDS, string ActionType, bool isOnline);
    }

    
}
