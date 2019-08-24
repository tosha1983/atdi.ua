using Atdi.DataModels.Sdrns.Device;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.DataModels.Sdrn.DeviceServer.Commands.Parameters;

namespace Atdi.DataModels.Sdrn.DeviceServer.Processing
{
    public class BandWidthTask : MeasurementTaskBase
    {
        public bool Smooth;
        public string BandwidthEstimationType;
        public double X_Beta;
        public int MaximumIgnorPoint;
        public BWResult MeasBWResults; //  результат измерения
        public long maximumTimeForWaitingResultBandWidth; // (максимальное время ожидания результата)
        public DateTime? LastTimeSend = null;
        public long durationForSendResultBandWidth;  // в миллисекундах из файла конфигурации - время в течении которого мы посылаем один результат (10 мин по умолчанию)
        public long durationForMeasBW_ms; // в миллисекундах из файла конфигурации - максимальное время, выделяемое для выполнения измерения BW


    }
}
