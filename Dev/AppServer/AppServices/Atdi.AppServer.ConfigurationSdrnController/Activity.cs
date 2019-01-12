
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DM = Atdi.DataModels.Sdrns.Device;
using SM = Atdi.AppServer.Contracts.Sdrns;
using Atdi.SDNRS.AppServer.ManageDB.Adapters;

namespace Atdi.AppServer.ConfigurationSdrnController
{
    public class Activity
    {
        public static Int64 Constraint = 20;
        public static System.Timers.Timer timer = new System.Timers.Timer();
        public Int64 cntSeconds = 0;
        public ILogger _logger;
        public Activity(ILogger logger)
        {
            _logger = logger;
            timer.Interval = 1000;
            timer.Elapsed += Timer_Elapsed;
            timer.Enabled = true;
        }

        private void Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            cntSeconds++;
            if (cntSeconds > Constraint)
            {
                timer.Enabled = false;
                Concumer.CheckActivitySensor(null, false, _logger);
                cntSeconds = 0;
                timer.Enabled = true;
            }
        }


     
    }
}
