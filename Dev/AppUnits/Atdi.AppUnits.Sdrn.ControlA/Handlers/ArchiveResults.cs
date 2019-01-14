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
            if (cntSeconds> Config._TimeArchiveResult)
            {
                timer.Enabled = false;
                SaveMeasSDRResults msres = new SaveMeasSDRResults();
                LoadDataMeasTask loadDataMeasTask = new LoadDataMeasTask();
                List<SM.MeasSdrResults> MRes = loadDataMeasTask.LoadActiveTaskSdrResults();
                if (MRes != null)
                {
                    foreach (SM.MeasSdrResults item in MRes.ToArray())
                    {
                        if (item.FindAndDestroyObject(2 * Config._TimeArchiveResult, "C"))
                        {
                            msres.SaveStatusResultSDR(item, "Z");
                            cntSeconds = 0;
                        }
                    }
                }
                MRes.Clear();
                timer.Enabled = true;
            }
        }

    }
}
