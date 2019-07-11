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

namespace Atdi.AppUnits.Sdrn.DeviceServer.Processing
{
    public static class ConvertTaskParametersToMesureSystemInfoParameterForSysInfo
    {
        /// <summary>
        /// Конвертор из TaskParameters в MesureSystemInfoParameter(объект на основе которого выполняется отправка команды в адаптер)
        /// </summary>
        /// <param name="taskParameters"></param>
        /// <returns></returns>
        public static MesureSystemInfoParameter[] ConvertForMesureSystemInfoParameter(this TaskParameters taskParameters)
        {
            // Пока используем временное решение. Т.е. если есть пересечение с диапазонами то выбираем соответсвующий формат
            List<MesureSystemInfoParameter> mesureSystemInfoParameters = new List<MesureSystemInfoParameter>();
            if (ChechGSM900(taskParameters)||ChechGSM1800(taskParameters))
            {
                MesureSystemInfoParameter mesureSystemInfoParameter = new MesureSystemInfoParameter();
                if (ChechGSM900(taskParameters) && ChechGSM1800(taskParameters))
                { mesureSystemInfoParameter.Bands = new string [2];
                    mesureSystemInfoParameter.Bands[0] = Atdi.DataModels.Sdrn.DeviceServer.Commands.Parameters.MesureSystemInfo.GSMBands.P_GSM900.ToString();
                    mesureSystemInfoParameter.Bands[0] = Atdi.DataModels.Sdrn.DeviceServer.Commands.Parameters.MesureSystemInfo.GSMBands.GSM1800.ToString();
                }
                else if (ChechGSM900(taskParameters))
                {
                    mesureSystemInfoParameter.Bands = new string[1];
                    mesureSystemInfoParameter.Bands[0] = Atdi.DataModels.Sdrn.DeviceServer.Commands.Parameters.MesureSystemInfo.GSMBands.P_GSM900.ToString();
                }
                else
                {
                    mesureSystemInfoParameter.Bands = new string[1];
                    mesureSystemInfoParameter.Bands[0] = Atdi.DataModels.Sdrn.DeviceServer.Commands.Parameters.MesureSystemInfo.GSMBands.GSM1800.ToString();
                }
                //mesureSystemInfoParameter.CDMAEVDOFreqTypes
                mesureSystemInfoParameter.DelayToSendResult_s = 30;
                //mesureSystemInfoParameter.Freqs_Hz
                mesureSystemInfoParameter.FreqType = Atdi.DataModels.Sdrn.DeviceServer.Commands.Parameters.MesureSystemInfo.FreqType.New;
                mesureSystemInfoParameter.PeriodicResult = true;
                mesureSystemInfoParameter.ResultOnlyWithGCID = false;
                //mesureSystemInfoParameter.RFInput;
                mesureSystemInfoParameter.Standart = "GSM";
                mesureSystemInfoParameters.Add(mesureSystemInfoParameter);
            }
            if (ChechUMTS(taskParameters))
            {
                MesureSystemInfoParameter mesureSystemInfoParameter = new MesureSystemInfoParameter();
                //mesureSystemInfoParameter.Bands;
                //mesureSystemInfoParameter.CDMAEVDOFreqTypes
                mesureSystemInfoParameter.DelayToSendResult_s = 30;
                mesureSystemInfoParameter.Freqs_Hz = UMTSFreqs_Hz();
                mesureSystemInfoParameter.FreqType = Atdi.DataModels.Sdrn.DeviceServer.Commands.Parameters.MesureSystemInfo.FreqType.New;
                mesureSystemInfoParameter.PeriodicResult = true;
                mesureSystemInfoParameter.ResultOnlyWithGCID = false;
                //mesureSystemInfoParameter.RFInput;
                mesureSystemInfoParameter.Standart = "UMTS";
                mesureSystemInfoParameters.Add(mesureSystemInfoParameter);
            }
            return mesureSystemInfoParameters.ToArray();
        }
        private static bool ChechGSM900(TaskParameters taskParameters)
        {
            double GSM900Min_MHz = 925;
            double GSM900Max_MHz = 960;
            if (!(taskParameters.MinFreq_MHz > GSM900Max_MHz) || (taskParameters.MaxFreq_MHz < GSM900Min_MHz)) { return true; }
            return false;
        }
        private static bool ChechGSM1800(TaskParameters taskParameters)
        {
            double GSM1800Min_MHz = 1805;
            double GSM1800Max_MHz = 1880;
            if (!(taskParameters.MinFreq_MHz > GSM1800Max_MHz) || (taskParameters.MaxFreq_MHz < GSM1800Min_MHz)) { return true; }
            return false;
        }
        private static bool ChechUMTS(TaskParameters taskParameters)
        {
            double UMTSMin_MHz = 2110;
            double UMTSMax_MHz = 2170;
            if (!(taskParameters.MinFreq_MHz > UMTSMax_MHz) || (taskParameters.MaxFreq_MHz < UMTSMin_MHz)) { return true; }
            return false;
        }
        private static decimal[] UMTSFreqs_Hz()
        {
            decimal[] Freq_Hz = new decimal[12] {2112800000, 2117600000, 2122400000, 2127400000, 2132400000, 2137400000,
                2142400000, 2147400000, 2152400000, 2157400000, 2162400000, 2167200000};
            return Freq_Hz;
        }

    }
}
