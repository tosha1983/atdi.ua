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
        double BitRate_MBs; // скорость потока IQ;  -1 = максимальная
        double IQBlockDuration_s; // длительность ожидаемого блока с IQ
        double IQReceivTime_s; // общее временное окно отводимое для получения потока IQ
        bool MandatoryPPS; // обязательная привязка к PPS 
        bool MandatorySignal; //  Снятие потока только при наличии наличия сигнала
        long TimeStart; // Время старта записи IQ stream
    }
}

