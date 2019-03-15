using Atdi.DataModels.Sdrns.Device;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.DataModels.Sdrn.DeviceServer.Commands.Parameters;

namespace Atdi.DataModels.Sdrn.DeviceServer.Processing
{
    public class BandWidthTask : TaskBase
    {
        public BWResult MeasResults; //  результат измерения
        public MesureTraceParameter mesureTraceParameter;
        public TaskParameters taskParameters;


        public long SleepTimePeriodForWaitingStartingMeas;  // засыпание потока на время SleepTimePeriodForWaitingStartingMeas_ms
        public int CountMeasurementDone = 0;
        public long SOKoeffWaitingDevice;  // в разах коэфициент который показывает в сколько раз больше и будем спать при проблемах с девайсом относительно времени между измерениями
        public long maximumTimeForWaitingResultBandWidth; // (максимальное время ожидания результата)
        public long durationForSendResult;  // в миллисекундах из файла конфигурации - время в течении которого мы посылаем один результат
        public DateTime? LastTimeSend = null;
    }
}
