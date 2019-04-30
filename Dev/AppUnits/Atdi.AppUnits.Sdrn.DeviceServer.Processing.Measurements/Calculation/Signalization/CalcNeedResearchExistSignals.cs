using Atdi.Contracts.Sdrn.DeviceServer;
using Atdi.DataModels.Sdrn.DeviceServer.Processing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.DataModels.Sdrns.Device;
using Atdi.DataModels.Sdrn.DeviceServer;
using Atdi.DataModels.Sdrn.DeviceServer.Commands.Parameters;
using Atdi.DataModels.Sdrn.DeviceServer.Commands.Results;

namespace Atdi.AppUnits.Sdrn.DeviceServer.Processing.Measurements
{
    public static class CalcNeedResearchExistSignals
    {
        // константы
        private static int MaxTaskFromOneTime = 1;
        private static double DurationWithoutDetailScan_s = 100;
        private static double MaxDurationFromEnableSignals_ms = 100;
        private static int TraceCountForBW = 10;
        // конец констант

        /// <summary>
        /// Функция которая определяет логику по постановке задачь на детальное сканирование 
        /// </summary>
        /// <param name="EmittingsSummury"></param>
        /// <param name="taskParameters"></param>
        /// <returns></returns>
        public static bool NeedResearchExistSignals(Emitting[] EmittingsSummury, out TaskParameters[] taskParameters)
        {
            List<int> EmittingForAnalyzeID = new List<int>();
            DateTime CurTime = DateTime.Now;
            // поиск тех измерений которые не детальные
            if (EmittingsSummury != null)
            {
                for (int i = 0; i < EmittingsSummury.Length; i++)
                {
                    if (EmittingsSummury[i].WorkTimes != null)
                    {
                        TimeSpan DeltaTime = CurTime - EmittingsSummury[i].WorkTimes[EmittingsSummury[i].WorkTimes.Length - 1].StopEmitting;
                        if ((!EmittingsSummury[i].SpectrumIsDetailed) && (DeltaTime.TotalMilliseconds < MaxDurationFromEnableSignals_ms))
                        {
                            EmittingForAnalyzeID.Add(i);
                        }
                    }
                }
                if (EmittingForAnalyzeID.Count != MaxTaskFromOneTime)
                {
                    if (EmittingForAnalyzeID.Count > MaxTaskFromOneTime)
                    {// не обследованных излучений у нас больше чем можно обследовать тупа перфорируем
                        Random rnd = new Random();
                        do
                        {
                            int value = rnd.Next(0, EmittingForAnalyzeID.Count - 1);
                            EmittingForAnalyzeID.RemoveAt(value);
                        }
                        while (EmittingForAnalyzeID.Count > MaxTaskFromOneTime);
                    }
                    else
                    {// не обследованных излучений у нас мало, а обследовать можно больше так что придеться добавлять
                        int LastIndex = EmittingForAnalyzeID.Count - 1;
                        for (int i = 0; i < EmittingsSummury.Length; i++)
                        {
                            if (EmittingsSummury[i].WorkTimes != null)
                            {
                                TimeSpan DeltaTime = CurTime - EmittingsSummury[i].WorkTimes[EmittingsSummury[i].WorkTimes.Length - 1].StopEmitting;
                                TimeSpan DeltaTime1 = CurTime - EmittingsSummury[i].LastDetaileMeas;
                                if ((EmittingsSummury[i].SpectrumIsDetailed) && (DeltaTime.TotalMilliseconds < MaxDurationFromEnableSignals_ms) && (DeltaTime1.TotalSeconds > DurationWithoutDetailScan_s))
                                {
                                    EmittingForAnalyzeID.Add(i);
                                }
                            }
                        }
                        if (EmittingForAnalyzeID.Count > MaxTaskFromOneTime)
                        {// не обследованных излучений у нас больше чем можно обследовать тупа перфорируем
                            Random rnd = new Random();
                            do
                            {
                                int value = rnd.Next(LastIndex + 1, EmittingForAnalyzeID.Count - 1);
                                EmittingForAnalyzeID.RemoveAt(value);
                            }
                            while (EmittingForAnalyzeID.Count > MaxTaskFromOneTime);
                        }
                    }
                }
            }
            // формирование taskParameters по масиву
            taskParameters = GetTaskParameters(EmittingsSummury, EmittingForAnalyzeID);
            if (taskParameters == null)
            { return false;}
            return true;
        }
        private static TaskParameters[] GetTaskParameters(Emitting[] EmittingsSummury, List<int> ListID)
        { // НЕ ПРОВЕРЕННО
            if (ListID.Count > 0)
            {
                if (EmittingsSummury != null)
                {
                    TaskParameters[] TaskParameters = new TaskParameters[ListID.Count];
                    for (int i = 0; ListID.Count > i; i++)
                    {
                        var Emitting = EmittingsSummury[ListID[i]];
                        TaskParameters TaskParameter = new TaskParameters();
                        TaskParameter.MinFreq_MHz = Emitting.Spectrum.SpectrumStartFreq_MHz;
                        TaskParameter.MaxFreq_MHz = Emitting.Spectrum.SpectrumStartFreq_MHz + Emitting.Spectrum.SpectrumSteps_kHz * (Emitting.Spectrum.Levels_dBm.Length - 1) / 1000;
                        TaskParameter.NCount = TraceCountForBW;
                        TaskParameter.SweepTime_s = 0.001;
                        TaskParameters[i] = TaskParameter;
                    }
                    return TaskParameters;
                }
                else { return null; }
            }
            else { return null; }
        }
    }
}
