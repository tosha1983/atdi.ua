using Atdi.DataModels.Sdrns.Device;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrn.DeviceServer.Processing
{
    public class LevelTask : MeasurementTaskBase
    {
        public LevelResult LevelResult;//  результат измерения
        public long durationForSendResultLevel;  // в миллисекундах из файла конфигурации - время в течении которого мы посылаем один результат (10 мин по умолчанию)
        public long durationForMeasLevel_ms; // в миллисекундах из файла конфигурации - максимальное время, выделяемое для выполнения измерения Level
    }
}
