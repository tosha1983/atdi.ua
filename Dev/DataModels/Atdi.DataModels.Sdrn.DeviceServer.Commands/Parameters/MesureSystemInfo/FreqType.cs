using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrn.DeviceServer.Commands.Parameters.MesureSystemInfo
{
    public enum FreqType : int
    {
        Add = 0,//добавить частоты к существуещему списку
        New = 1//список частот задается заново(предыдущие результаты будут удалены) 
    }
}
