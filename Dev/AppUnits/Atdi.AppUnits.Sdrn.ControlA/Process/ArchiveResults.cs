using System;
using System.Collections.Generic;
using SM = Atdi.AppServer.Contracts.Sdrns;
using Atdi.AppUnits.Sdrn.ControlA.Bus;
using Atdi.AppUnits.Sdrn.ControlA.ManageDB;


namespace Atdi.AppUnits.Sdrn.ControlA.Process
{
    public class ArchiveResults
    {
        public static System.Timers.Timer timer = new System.Timers.Timer();
        public static Int64 cntSeconds = 0;
        public ArchiveResults()
        {
            timer.Interval = 1000;
            timer.Elapsed += Timer_Elapsed;
            timer.Enabled = true;
        }

        private void Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            cntSeconds++;
            if (cntSeconds > ConfigParameters.TimeArchiveResult)
            {
                timer.Enabled = false;
                var msres = new SaveMeasSDRResults();
                var loadDataMeasTask = new LoadDataMeasTask();
                var mRes = loadDataMeasTask.LoadActiveTaskSdrResults();
                if (mRes != null)
                {
                    foreach (var item in mRes.ToArray())
                    {
                        msres.SaveStatusResultSDR(item, AllStatusSensor.Z.ToString());
                        cntSeconds = 0;
                        Launcher._logger.Info(Contexts.ThisComponent, Categories.ArchiveResult, Events.ArchiveResult);
                    }
                }
                mRes.Clear();
                timer.Enabled = true;
            }
        }

    }
}
