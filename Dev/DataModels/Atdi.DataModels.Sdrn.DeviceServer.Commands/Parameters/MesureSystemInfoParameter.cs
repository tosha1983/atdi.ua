using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrn.DeviceServer.Commands.Parameters
{
    public class MesureSystemInfoParameter
    {
        public string Standart; //GSM/UMTS/CDMAEVDO/LTE/TETRA
        public decimal[] Freqs_Hz;//центральные частоты каналов, если нужен бенд то писать в Bands
        public bool[] CDMAEVDOFreqTypes;//Тип канала True = EVDO, False = CDMA, для каждой частоты должно быть установленно!!!, используется только с CDMAEVDO
        public string[] Bands;//бенды каналов по этой технологии
        public MesureSystemInfo.FreqType FreqType;
        public int RFInput;//Используется только для TSMW определяетс на каком канале работает данная технология
        public int Att_dB; //-1 = auto, Используется только для TSMW 
        public int PreAmp_dB; //-1 = auto, Используется только для TSMW 
        public int DelayToSendResult; //через сколько секунд после запуска измерения опубликовать результат
        public bool ResultOnlyWithGCID; //Тип возвращаемых результатов только с идентификационными данными или все
    }
}
