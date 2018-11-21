using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.AppServer.Contracts.Sdrns;

using System.Collections.Concurrent;
using NLog;

namespace Atdi.SDR.Server.Bus.Classes
{
    public static class GlobalInit
    {
        public static Logger log;
       

        static GlobalInit()
        {
            log = LogManager.GetCurrentClassLogger();
            NLog.Targets.FileTarget tr = (NLog.Targets.FileTarget)LogManager.Configuration.FindTargetByName("f");
            tr.DeleteOldFileOnStartup = false;
        }



    }
}
