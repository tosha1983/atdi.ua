using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrn.DeviceServer.Commands.Results
{
    [Serializable]
    public class EstimateRefLevelResult : CommandResultPartBase
    {
        public EstimateRefLevelResult(ulong partIndex, CommandResultStatus status)
               : base(partIndex, status)
        {
        }
        public EstimateRefLevelResult()
            :base()
        { }

        public float[] Level;
        public double FrequencyStart_Hz; //Фактическая начальная частота
        public double FrequencyStep_Hz; // Реальный шаг в сетке частот
        public int LevelMaxIndex; // последний значимый элемент в индексе массивов
        public long TimeStamp; //Тики относительно new DateTime(1970, 1, 1, 0, 0, 0, System.DateTimeKind.Utc)
        public int RefLevel_dBm;//Возвращает в любом случаее текущий установленный
        public int Att_dB;//Возвращает в любом случаее текущий установленный
        public int PreAmp_dB;//Возвращает в любом случаее текущий установленный
        public double RBW_Hz;//Возвращает в любом случаее текущий установленный
        public double VBW_Hz;//Возвращает в любом случаее текущий установленный
        public Enums.DeviceStatus DeviceStatus;
    }
}
