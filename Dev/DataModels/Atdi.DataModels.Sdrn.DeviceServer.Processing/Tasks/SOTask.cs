using Atdi.DataModels.Sdrns.Device;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.DataModels.Sdrn.DeviceServer.Commands.Parameters;

namespace Atdi.DataModels.Sdrn.DeviceServer.Processing
{
    public class SOTask : MeasurementTaskBase
    {
        public MeasResults MeasResults; //  результат измерения
        public SpectrumOcupationResult lastResultParameters;
        public DateTime? LastTimeSend = null;
        public SensorParameters sensorParameters;
        public long maximumTimeForWaitingResultSO; // в миллисекундах из файла конфигурации - время в течении которого будем ждать результат измерения и расчета SO для одного трейса
        public long durationForSendResultSO;  // в миллисекундах из файла конфигурации - время в течении которого мы посылаем один результат (10 мин по умолчанию)
    }
}
