using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XMLLibrary;
using CoreICSM.Logs;

namespace Atdi.SDNRS.AppServer.Sheduler
{
    public class ScanMeasTasks
    {
        public void StartReload()
        {
            try {
                //Sheduler_Up_Meas_SDRNS Quartz = new Sheduler_Up_Meas_SDRNS();
                //Quartz.ShedulerRepeatStart(BaseXMLConfiguration.xml_conf._ScanMeasTasks);
            }
            catch (Exception ex) { CoreICSM.Logs.CLogs.WriteError(ELogsWhat.Unknown, "[StartReload]:" + ex.Message); }
        }

        public void SendMeaskTaskToSDR()
        {
            try {
                //ShedulerSubmitMeasTask Quartz = new ShedulerSubmitMeasTask();
                //Quartz.ShedulerRepeatStart(BaseXMLConfiguration.xml_conf._TimerSendMeaskTaskToSDR);
            }
            catch (Exception ex) { CoreICSM.Logs.CLogs.WriteError(ELogsWhat.Unknown, "[SendMeaskTaskToSDR]:" + ex.Message); }
            
        }
    }
        
}
