using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.DataModels.Sdrn.DeviceServer.Processing;
using Atdi.DataModels.Sdrn.DeviceServer.Commands.Results.MesureSystemInfo;
using Atdi.DataModels.Sdrn.DeviceServer.Commands.Results;
using Atdi.DataModels.Sdrns.Device;

namespace Atdi.AppUnits.Sdrn.DeviceServer.Processing.Measurements
{
    static class CommonConvertors
    {
        /// <summary>
        ///Вычисление задержки выполнения потока результатом является количество vмилисекунд на которое необходимо приостановить поток
        /// </summary>
        /// <param name="taskParameters">Параметры таска</param> 
        /// <param name="doneCount">Количество измерений которое было проведено</param>
        /// <returns></returns>
        public static long CalculateTimeSleep(TaskParameters taskParameters, int DoneCount)
        {
            DateTime dateTimeNow = DateTime.Now;
            if (dateTimeNow > taskParameters.StopTime.Value) { return -1; }
            TimeSpan interval = taskParameters.StopTime.Value - dateTimeNow;
            double interval_ms = interval.TotalMilliseconds;
            if (taskParameters.NCount <= DoneCount) { return -1; }
            long duration = (long)(interval_ms / (taskParameters.NCount - DoneCount));
            return duration;
        }

        public static string GetTaskId(string value)
        {
            var result = "";
            if (value != null)
            {
                var words = value.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
                if ((words != null) && (words.Length > 0))
                {
                    result = words[0];
                }
            }
            return result;
        }

        /// <summary>
        /// Конвертор из StationSystemInfo[] -> SysInfoResult[]
        /// </summary>
        /// <param name="stationSystemInfos"></param>
        /// <returns></returns>
        public static SysInfoResult ConvertToSysInfoResult(MesureSystemInfoResult mesureSystemInfoResult )
        {
            var measSysInfoResults = new SysInfoResult();
            if ((mesureSystemInfoResult.SystemInfo != null) && (mesureSystemInfoResult.SystemInfo.Length > 0))
            {
                measSysInfoResults.signalingSysInfo = new SignalingSysInfo[mesureSystemInfoResult.SystemInfo.Length];
                for (int i = 0; i < mesureSystemInfoResult.SystemInfo.Length; i++)
                {
                    var systemInfo = mesureSystemInfoResult.SystemInfo[i];
                    measSysInfoResults.signalingSysInfo[i] = new SignalingSysInfo();
                    var signalingSysInfo = measSysInfoResults.signalingSysInfo[i];
                    signalingSysInfo.BandWidth_Hz = systemInfo.BandWidth_Hz;
                    signalingSysInfo.BSIC = systemInfo.BSIC;
                    signalingSysInfo.ChannelNumber = systemInfo.ChannelNumber;
                    signalingSysInfo.CID = systemInfo.CID;
                    signalingSysInfo.CtoI = systemInfo.CtoI;
                    signalingSysInfo.Freq_Hz = systemInfo.Freq_Hz;
                    signalingSysInfo.LAC = systemInfo.LAC;
                    signalingSysInfo.Level_dBm = systemInfo.Level_dBm;
                    signalingSysInfo.MCC = systemInfo.MCC;
                    signalingSysInfo.MNC = systemInfo.MNC;
                    signalingSysInfo.Power = systemInfo.Power;
                    signalingSysInfo.RNC = systemInfo.RNC;
                    signalingSysInfo.Standard = systemInfo.Standart;
                    measSysInfoResults.signalingSysInfo[i] = signalingSysInfo;
                }
            }
            return measSysInfoResults;
        }
    }
}
