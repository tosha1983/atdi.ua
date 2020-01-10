using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrn.DeviceServer.Commands.Parameters
{
    public class MesureIQStreamParameter
    {
        public decimal FreqStart_Hz; // mandatory 
        public decimal FreqStop_Hz;  // mandatory 
        public int Att_dB; //-1 = auto, 
        public int PreAmp_dB; //-1 = auto, 
        public int RefLevel_dBm; // -1 = auto
        public double BitRate_MBs; // скорость потока IQ;  -1 = максимальная
        public double IQBlockDuration_s; // длительность ожидаемого блока с IQ
        public double IQReceivTime_s; // общее временное окно отводимое для получения потока IQ
        public bool MandatoryPPS; // обязательная привязка к PPS 
        public bool MandatorySignal; //  Снятие потока только при наличии наличия сигнала
        public long TimeStart; // Время старта записи IQ stream
        public float TriggerLevel_dBm; // Пороговый уровень определения наличия сигнала
    }
}

