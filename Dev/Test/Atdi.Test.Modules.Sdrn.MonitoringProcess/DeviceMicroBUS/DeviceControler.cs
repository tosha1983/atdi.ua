using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Test.Modules.Sdrn.MonitoringProcess.DeviceControler
{
    #region input
    enum DeviceMeasType
    {
        GPSLocation, // получение координат и времени с GPS
        Trace, // базовое измерение - получение спектрограммы
        SysInfo, // получение информации со служебного канала (идентификация сигнала)
        IQStream, // получение цифрового потока после демодулятора костаса. 
        DF, // получение азимутов на источник излучения
        RealTime, // получение вероятностной характеристики спектра сигнала. 
        SignalParameters, // (Level, BW, Freq, MOD + фильтр, bitrate, ClassEm, )
        Audio, // получение параметров аудио сигнала
        ZeroSpan // получение временной зависимости амплитуды сигнала.
    }
    class DeviceParametersConfiguration // базовый класс
    {
        double RBW_Hz; // -1 = auto; 
        double VBW_Hz; // -1 = auto; 
        decimal FreqStart_Hz; // mandatory 
        decimal FreqStop_Hz;  // mandatory 
        double SweepTime_s; //-1 = auto
        int Att_dB; //-1 = auto, 
        int PreAmp_dB; //-1 = auto, 
        int RefLevel_dBm; // -1 = auto
        DetectorType DetectorType1; //
        TraceType traceType1; // 
        int TraceCount; // количество трейсов  // mandatory 
        int TracePoint; // -1 = auto; 
        SweepType SweepType;
        RecieverMode RecieverMode;
        MeasMode MeasMode;
        LevelUnit LevelUnit;
    }
    class DFConfiguration : DeviceParametersConfiguration
    {
        decimal DFBW_Hz;
        int DFSQUMode; //0 = off, 1 - Normal, 2 - Gate
        double SQULevel_dBm;
        double DFQuality; // 0-100
    }
    class TreceСonfiguration : DeviceParametersConfiguration
    {

    }
    class IQStreamСonfiguration : DeviceParametersConfiguration
    {
        double BitRate_MBs; // скорость потока IQ
        double IQBlockDuration_s; // длительность ожидаемого блока с IQ
        double IQReceivTime_s; // общее временное окно отводимое для получения потока IQ
        bool MandatoryPPS; // снятие потока только при наличии наличия PPS
        bool MandatorySignal; //  снятие потока только при наличии наличия Сигнала
    }
    class RealTimeСonfiguration : DeviceParametersConfiguration
    {
        double DurationFrame_s; // длительность фрейма 
    }
    class SysInfoСonfiguration
    {
        decimal[] Freqs_Hz;
        double block_duration_s; // длительность времени накопления ответов
    }
    class AudioConfiguration
    {
        decimal Freq_Hz;
        decimal DemodBW_Hz;
        string TypeModulatio; //"AM or FM"
    }
    class SignalParametersConfiguration : DeviceParametersConfiguration //для BW, Level, .....
    {
        decimal FreqCentral;
        decimal BW;
    }
    class ZeroSpanConfiguration : DeviceParametersConfiguration
    {
        double DeltaTime_s;
    }
    enum DetectorType
    {
        Auto,
        AutoSelect, // AN
        AutoPeak, // AN
        MaxPeak, // AN, TSMX
        MinPeak, // AN, TSMX
        Average, // AN, PR
        RMS, // AN, PR, TSMX
        Semple, // AN
        Fast, // PR
        Peak, // PR
        Normal // AN (Kesyte, Anr)
    }
    enum TraceType
    {
        Auto, // 
        ClearWhrite, //AN, PR
        Average, //AN, PR
        MaxHold, //AN, PR
        MinHold, //AN, PR
        Blank,  //AN, PR
        View, //AN, PR
        Treking // 
    }
    enum SweepType
    {
        Auto,
        Sweep,
        FFT
    }
    enum RecieverMode
    {
        Auto,
        FFM,
        PSCAN
    }
    enum MeasMode
    {
        Auto,
        Periodic,
        Continuos
    }
    enum LevelUnit // Базовый класс ВЕЗДЕ
    {
        dBm,
        dBmkV,
        dBmkVm,
        mkV,
    }
    #endregion
    #region output
    class GPSLocation
    {
        double Lon; //DEC
        double Lat; //DEC
        int ASL; //m
        long TimeStamp; //Тики относительно new DateTime(1970, 1, 1, 0, 0, 0, System.DateTimeKind.Utc)
    }
    class Trace
    {
        float[] Level;
        double[] Freq_Hz;
        long TimeStamp; //Тики относительно new DateTime(1970, 1, 1, 0, 0, 0, System.DateTimeKind.Utc)
    }
    class ZeroSpan
    {
        float[] Level;
        double[] Time_s;
        long TimeStamp; //Тики относительно new DateTime(1970, 1, 1, 0, 0, 0, System.DateTimeKind.Utc)
    }
    class ChannelPower
    {
        double Result; // мощность в канале в юнитах
        decimal BW_Hz; // полоса сигнала который мы измерели
        decimal Freq_Hz; // Центральная частота
    }
    class DeviceBW
    {
        decimal BWResult_Hz;
        int T1; // индекс на trace (левый)
        int T2; // индекс на trace (правый)
        int MarkerIndex; // индекс на trace (центральный)
        TypeEstimationBW TypeEstimationBW;
    }
    class Audio
    {
        string AudioData;
    }
    class SysInfo
    {
        GSMBTSData[] GSMBTSDatas;
        UMTSBTSData[] UMTSBTSDatas;
        CDMABTSData[] CDMABTSDatas;
        LTEBTSData[] LTEBTSDatas;
    }
    enum TypeEstimationBW
    {
        beta,
        NdB
    }
    class GSMBTSData
    {
        decimal Freq_Hz;
        int BSIC; // c TSMX код идентификации БС BCC + NCC уникальное в рамках 
        int MCC; // с TSMX код страны
        int MNC; // с TSMX код оператора
        int LAC; // с TSMX код региона
        int CID; // с TSMX cell ID
        int SectorFromCID;
        string GCID;  // с TSMX Global CID
        public int CIDToDB; // Женя притягивает к БД ICSM
        float Powers; //dBm -> LevelCar
        long TimeStamps; //Тики относительно new DateTime(1970, 1, 1, 0, 0, 0, System.DateTimeKind.Utc)
        float Strenghts;// напряженность этого сигнала
        float CarToInts;
        LevelCar[] LevelCars;
    }
    class UMTSBTSData
    {
        decimal Freq_Hz;
        decimal FreqUp_Hz;
        int SC; // сркемлинг кода
        int MCC;
        int MNC;
        int LAC;
        int UCID; // UTRAN cell ID - > RNC + CID
        int RNC; // Radionetwork Controler ID
        int CID;
        int SectorFromCID;
        int CarrierFromCID; // номер несущей из идентификатора
        string GCID;
        int SectorIDToDB;
        float ISCPs; // c TSMX интегральный уровень сигнала
        float RSCPs; // c TSMX  уровень сигнала -> LevelCar
        long TimeStamps;
        float Strenghts;
        float InbandPowers; // P total
        float CodePowers; //TSMX  уровень сигнала 
        float IcIos; // TSMX  C/I 
        LevelCar[] LevelCars;
    }
    public class CDMABTSData
    {
        decimal Freq_Hz;
        decimal FreqUp_Hz;
        bool Type;// true = EVDO false = CDMAO
        int PN; // Пилот намбер
        int SID; // оператор id 
        int NID; // netwokr ID
        int MCC;
        int MNC;
        int BaseID; //Идентификатор БС
        string GCID;
        float RSCPs; // Уровень в LevelCar 
        float Strenghts;
        float PTotals; //
        float AverageInbandPowers; //
        float IcIos;
        long TimeStamps;
        LevelCar[] LevelCars;
    }
    public class LTEBTSData
    {
        decimal Freq_Hz;
        decimal FreqUp_Hz;
        decimal Bandwidth;
        int MCC;
        int MNC;
        int TAC; // trcing area code (LAC)
        int CelId28; // спец формат 
        int eNodeBId; // Идентификатор БС 20 бит от CelId28
        int CID; // Идентификатор БС 8 бит от CelId28 (последние)
        int PCI; // Физикал сел ID
        int CIDToDB;
        string MIMO_2x2;
        string eNodeB_Name; // ????
        float CIRofCPs; // CtoI
        string GCID;
        float Powers; // - > Некий уровень
        float InbandPowers; // - > Некий уровень
        float WB_RS_RSSIs; // - > Некий уровень
        float RSRPs; // - > Level CAR
        float Strenghts;
        float WB_RSRPs; // - > Некий уровень
        float RSRQs; // - > Некий уровень
        float WB_RSRQs; // - > Некий уровень
        long TimeStamps;
        LevelCar[] LevelCars;
    }
    class LevelCar
    {
        float Level;
        long TimeStamps;
    }
    class DF : Trace
    {
        float Az; // град
        float AzAnt; // град
        float Quality; // проценты
    }
    class SignalParameters:Trace
    {
        ChannelPower ChannelPower;
        DeviceBW DeviceBW;
        string ModType;

    }
    class RealTime:Trace
    {
        float[] frame;
        int frameWidth;
        int frameHeight;
    }
    #endregion
}


