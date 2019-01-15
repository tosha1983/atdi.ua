using Atdi.Contracts.Api.Sdrn.MessageBus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DM = Atdi.DataModels.Sdrns.Device;
using SM = Atdi.AppServer.Contracts.Sdrns;



namespace Atdi.AppUnits.Sdrn.ControlA.Handlers
{
    public class SensorActivity
    {
        public static System.Timers.Timer timer = new System.Timers.Timer();
        public static Int64 cntSeconds = 0;
        public static int _periodSendActivitySensorInt { get;set;}
        public SensorActivity(int periodSendActivitySensorInt)
        {
            _periodSendActivitySensorInt = periodSendActivitySensorInt;
            timer.Interval = 1000;
            timer.Elapsed += Timer_Elapsed;
            timer.Enabled = true;
        }

        private void Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            cntSeconds++;
            if (cntSeconds> _periodSendActivitySensorInt)
            {
                timer.Enabled = false;
                SensorDBExtension sensorAction = new SensorDBExtension();
                List<SM.Sensor> L_ser = sensorAction.LoadObjectSensor();
                if (L_ser != null)
                {
                    if (L_ser.Count > 0)
                    {
                        BusManager._messagePublisher.Send("SendActivitySensor", L_ser[0]);
                        cntSeconds = 0;
                    }
                }
                timer.Enabled = true;
            }
        }

    }
}
