using Atdi.DataModels.Sdrns.Device;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.DataModels.Sdrn.DeviceServer.Commands.Parameters;

namespace Atdi.DataModels.Sdrn.DeviceServer.Processing
{
    public class SOTask : TaskBase
    {
        public StatusTask status;
        public MeasResults MeasResults; //  результат измерения
        public SpectrumOcupationResult lastResultParameters;
        public DateTime? LastTimeSend = null;
        public int CountMeasurementDone = 0;
        public MesureTraceParameter mesureTraceParameter;
        public TaskParameters taskParameters;
        public SensorParameters sensorParameters;
        public long durationForSendResult;  // в миллисекундах из файла конфигурации - время в течении которого мы посылаем один результат
        public long maximumTimeForWaitingResultSO; // в миллисекундах из файла конфигурации - время в течении которого будем ждать результат измерения и расчета SO для одного трейса
        public long SOKoeffWaitingDevice;  // в разах коэфициент который показывает в сколько раз больше и будем спать при проблемах с девайсом относительно времени между измерениями
    }
}
