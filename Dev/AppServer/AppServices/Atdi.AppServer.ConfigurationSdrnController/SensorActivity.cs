
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
    public class SensorActivity
    {
        public static System.Timers.Timer timer = new System.Timers.Timer();
        public Int64 cntSeconds = 0;
        public Atdi.AppServer.Contracts.Sdrns.Sensor Sensor_ { get; set; }
        public SensorActivity(Atdi.AppServer.Contracts.Sdrns.Sensor sensor)
        {
            timer.Interval = 1000;
            timer.Elapsed += Timer_Elapsed;
            timer.Enabled = true;
            Sensor_ = sensor;
        }

        private void Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            cntSeconds++;
            if (cntSeconds > 60)
            {
                timer.Enabled = false;
                Sensor_.Status = "F";
                ClassDBGetSensor.UpdateStatusSensor(Sensor_);
                cntSeconds = 0;
                timer.Enabled = true;
            }
        }


     
    }
}
