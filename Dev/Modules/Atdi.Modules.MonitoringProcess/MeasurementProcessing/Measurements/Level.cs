﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using Atdi.AppServer.Contracts.Sdrns;
using Atdi.Modules.MonitoringProcess;
using Atdi.Modules.MonitoringProcess.ProcessSignal;

namespace Atdi.Modules.MonitoringProcess.Measurement
{
    
    public class Level
    {
        public SemplFreq[] fSemples;
        public Level(ISDR SDR, TaskParameters taskParameters, SensorParameters sensorParameters, LastResultParameters lastResultParameters)
        {
            // const 
            //double TriggerLevel_dBm  = -100;
            // end const
            
            // получение потока данных
            Trace trace = new Trace(SDR, taskParameters, sensorParameters, lastResultParameters);
            // измерение произведено и находится в trace.fSemples
            // заполнение результатов по сути
            fSemples = trace.fSemples;
        }
    }
}
