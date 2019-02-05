using System;
using System.Collections.Generic;
using SM = Atdi.AppServer.Contracts.Sdrns;
using Atdi.AppUnits.Sdrn.ControlA.Bus;
using Atdi.AppUnits.Sdrn.ControlA.ManageDB;


namespace Atdi.AppUnits.Sdrn.ControlA.Process
{
    public class SensorActivity
    {
        public static System.Timers.Timer timer = new System.Timers.Timer();
        public static Int64 cntSeconds = 0;
        public SensorActivity()
        {
            timer.Interval = 1000;
            timer.Elapsed += Timer_Elapsed;
            timer.Enabled = true;
        }

        private void Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            cntSeconds++;
            if (cntSeconds> ConfigParameters.PeriodSendActivitySensor)
            {
                timer.Enabled = false;
                var sensorAction = new SensorDb();
                var sensor = sensorAction.LoadObjectSensor();
                if (sensor != null)
                {
                    if (sensor.Count > 0)
                    {
                        Launcher._messagePublisher.Send("SendActivitySensor", sensor[0]);
                        cntSeconds = 0;
                        Launcher._logger.Info(Contexts.ThisComponent, Categories.SendActivitySensor, Events.SendActivitySensor);
                    }
                }
                timer.Enabled = true;
            }
        }
    }
}
