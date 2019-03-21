using Atdi.Contracts.Sdrn.DeviceServer;
using Atdi.DataModels.Sdrn.DeviceServer.Adapters;
using CFG = Atdi.DataModels.Sdrn.DeviceServer.Adapters.Config;
using Atdi.DataModels.Sdrn.DeviceServer;
using Atdi.Platform.Logging;
using System;
using System.Collections.Generic;
using COM = Atdi.DataModels.Sdrn.DeviceServer.Commands;
using COMR = Atdi.DataModels.Sdrn.DeviceServer.Commands.Results;
using EN = Atdi.AppUnits.Sdrn.DeviceServer.Adapters.SpectrumAnalyzer.Enums;
using MEN = Atdi.DataModels.Sdrn.DeviceServer.Adapters.Enums;
using NationalInstruments.VisaNS;

namespace Atdi.AppUnits.Sdrn.DeviceServer.Adapters.SpectrumAnalyzer
{
    /// <summary>
    /// Пример реализации объект аадаптера
    /// </summary>
    //public class Adapter : IAdapter
    //{
    //    /// <summary>
    //    /// подумать на тему защиты прибора пока я ничего не делаю
    //    /// </summary>
    //    #region интересные команды
    //    /// <summary>
    //    /// стр 217 FORMat:DEXPort:DSEParator COMMa/POINt неожиданно нарыл разделитель дробной части
    //    /// стр 144 TRACe:IQ:SET NORM, 0, <SampleRate>, <TriggerMode>, <TriggerSlope>,<PretriggerSamp>, <NumberSamples>
    //    /// стр 145 TRACe:IQ:TPISample?
    //    /// стр 149 [SENSe:]ADJust:LEVel
    //    /// стр 206 TRACe:IQ:DATA? в старом мануале
    //    /// стр 212 TRACe:IQ:DATA:FORMat IQPair
    //    /// стр 212 TRACe:IQ:DATA:MEMory?  (Обратите внимание, однако, что команда TRAC: IQ: DATA? 
    //    /// стр 214 FORMat[:DATA] FORM REAL,32 FORM REAL,16 FORM REAL,64
    //    /// Инициирует новое измерение перед возвратом захваченных значений, а не с возвратом существующих данных в памяти.)
    //    /// </summary>
    //        #endregion

    //    #region Adapter
    //    private readonly ITimeService _timeService;
    //    private readonly ILogger _logger;
    //    private readonly AdapterConfig _adapterConfig;


    //    private LocalParametersConverter LPC;
    //    private CFG.ThisAdapterConfig TAC;
    //    private CFG.AdapterMainConfig MainConfig;

    //    /// <summary>
    //    /// Все объекты адаптера создаются через DI-контейнер 
    //    /// Запрашиваем через конструктор необходимые сервисы
    //    /// </summary>
    //    /// <param name="adapterConfig"></param>
    //    /// <param name="logger"></param>
    //    /// <param name="timeService"></param>
    //    public Adapter(AdapterConfig adapterConfig, ILogger logger, ITimeService timeService)
    //    {
    //        this._logger = logger;
    //        this._adapterConfig = adapterConfig;
    //        this._timeService = timeService;
    //        LPC = new LocalParametersConverter();
    //        FreqArr = new double[TracePoints];
    //        LevelArr = new float[TracePoints];
    //        for (int i = 0; i < TracePoints; i++)
    //        {
    //            FreqArr[i] = (double)(FreqStart + 1000 * i);
    //            LevelArr[i] = -100;
    //        }
    //    }

    //    /// <summary>
    //    /// Метод будет вызван при инициализации потока воркера адаптера
    //    /// Адаптеру необходимо зарегестрировать свои обработчики комманд 
    //    /// </summary>
    //    /// <param name="host"></param>
    //    public void Connect(IAdapterHost host)
    //    {
    //        try
    //        {

    //            /// включем устройство
    //            /// иницируем его параметрами сконфигурации
    //            /// проверяем к чем оно готово

    //            /// сообщаем инфраструктуре что мы готовы обрабатывать комманду MesureGpsLocationExampleCommand
    //            /// и при этом возвращать оезультат в типе MesureGpsLocationExampleAdapterResult
    //            if (SetConnect())
    //            {
    //                string filename = "SpectrumAnalyzer_" + UniqueData.SerialNumber + ".xml";
    //                TAC = new CFG.ThisAdapterConfig() { };
    //                if (!TAC.GetThisAdapterConfig(filename))
    //                {
    //                    MainConfig = new CFG.AdapterMainConfig() { };
    //                    SetDefaulConfig(ref MainConfig);
    //                    TAC.SetThisAdapterConfig(MainConfig, filename);
    //                }
    //                else
    //                {
    //                    MainConfig = TAC.Main;
    //                }
    //                host.RegisterHandler<COM.MesureTraceCommand, COMR.MesureTraceResult>(MesureTraceParameterHandler);
    //            }

    //        }
    //        #region Exception
    //        catch (Exception exp)
    //        {
    //            _logger.Exception(Contexts.ThisComponent, exp);
    //            throw new InvalidOperationException("Invalid initialize/connect adapter", exp);
    //        }
    //        #endregion
    //    }

    //    /// <summary>
    //    /// Метод вызывается контрллером когда необходимо выгрузит адаптер с памяти
    //    /// </summary>
    //    public void Disconnect()
    //    {
    //        try
    //        {
    //            /// освобождаем ресурсы и отключаем устройство
    //            IsRuning = false;

    //        }
    //        #region Exception
    //        catch (Exception exp)
    //        {
    //            _logger.Exception(Contexts.ThisComponent, exp);
    //        }
    //        #endregion
    //    }


    //    public void MesureTraceParameterHandler(COM.MesureTraceCommand command, IExecutionContext context)
    //    {
    //        try
    //        {
    //            if (IsRuning)
    //            {
    //                /// если нужно проводим блокировку комманд который мы не сможем обслужить пока что то меряем в течении выполнени яэтой комманды
    //                /// и подсказывая этим инфраструктуре что этот устрйостов некоторое время не сможет обрабатываить такие комманды
    //                //context.Lock(CommandType.MesureTrace);

    //                // если нужно заблокировать выполняему комманду то достатчоно вызвать метод без параметров и блокируется комманда которая выполняется
    //                context.Lock();

    //                if (FreqStart != command.Parameter.FreqStart_Hz)
    //                {
    //                    FreqStart = LPC.FreqStart(this, command.Parameter.FreqStart_Hz);
    //                    SetFreqStart(FreqStart);
    //                }
    //                if (FreqStop != command.Parameter.FreqStop_Hz)
    //                {
    //                    FreqStop = LPC.FreqStop(this, command.Parameter.FreqStop_Hz);
    //                    SetFreqStop(FreqStop);
    //                }

    //                decimal att = LPC.Attenuator(UniqueData, command.Parameter.Att_dB);
    //                if (AttLevel != att)
    //                {
    //                    AttLevel = att;
    //                    SetAttLevel(AttLevel);
    //                }

    //                if (RefLevel != command.Parameter.RefLevel_dBm)
    //                {
    //                    //типа авто...
    //                    if (command.Parameter.RefLevel_dBm == 1000000000)
    //                    {
    //                        RefLevel = LPC.RefLevel(UniqueData, -40);
    //                    }
    //                    else
    //                    {
    //                        RefLevel = LPC.RefLevel(UniqueData, command.Parameter.RefLevel_dBm);
    //                    }
    //                    SetRefLevel(RefLevel);
    //                }

    //                bool preamp = LPC.PreAmp(command.Parameter.PreAmp_dB);
    //                if (preamp != PreAmp)
    //                {
    //                    PreAmp = preamp;
    //                    SetPreAmp(PreAmp);
    //                }

    //                if (command.Parameter.RBW_Hz < 0) //непонятно
    //                {
    //                    SetRBW(RBW);
    //                }
    //                else
    //                {
    //                    RBW = LPC.RBW(UniqueData, (decimal)command.Parameter.RBW_Hz);
    //                    SetRBW(RBW);
    //                }
    //                if (command.Parameter.VBW_Hz < 0) //непонятно
    //                {
    //                    SetVBW(VBW);
    //                }
    //                else
    //                {
    //                    VBW = LPC.VBW(UniqueData, (decimal)command.Parameter.VBW_Hz);
    //                    SetVBW(VBW);
    //                }
    //                if (command.Parameter.TraceCount > 0)
    //                {
    //                    TraceCountToMeas = (ulong)command.Parameter.TraceCount;
    //                    TraceCount = 0;
    //                    TracePoints = command.Parameter.TracePoint;
    //                }
    //                else
    //                {
    //                    throw new Exception("TraceCount must be set greater than zero.");
    //                }






    //                context.Unlock();

    //                // что то делаем еще 


    //                // подтверждаем окончание выполнения комманды 
    //                // важно: всн ранее устапнволеные в контексте обработки текущей команыд блокировки снимаются автоматически
    //                context.Finish();
    //                // дальше кода быть не должно, освобождаем поток
    //            }
    //            else
    //            {
    //                throw new Exception("The device with serial number " + UniqueData.SerialNumber + " does not work");
    //            }
    //        }
    //        catch (VisaException v_exp)
    //        {
    //            // желательно записать влог
    //            _logger.Exception(Contexts.ThisComponent, v_exp);
    //            // этот вызов обязательный в случаи обрыва
    //            context.Abort(v_exp);
    //            // дальше кода быть не должно, освобождаем поток
    //        }
    //        catch (Exception e)
    //        {
    //            // желательно записать влог
    //            _logger.Exception(Contexts.ThisComponent, e);
    //            // этот вызов обязательный в случаи обрыва
    //            context.Abort(e);
    //            // дальше кода быть не должно, освобождаем поток
    //        }

    //    }

    //    #endregion

    //    private TcpipSession session;
    //    #region Param
    //    private LocalSpectrumAnalyzerInfo UniqueData { get; set; } = new LocalSpectrumAnalyzerInfo { };
    //    private List<LocalSpectrumAnalyzerInfo> AllUniqueData = new List<LocalSpectrumAnalyzerInfo>
    //    {
    //        #region FSW
    //        new LocalSpectrumAnalyzerInfo
    //        {
    //            InstrManufacture = 1, NumberOfTrace = 3, ChangeableSweepType = true, SweepPointFix = false, InstrModel = "FSW",
    //            HiSpeed = true,
    //            InstrOption = new List<DeviceOption>()
    //            {
    //                 new DeviceOption(){Type = "B4", Designation = "OCXO, Precision Reference Frequency", GlobalType = "OCXO, Precision Reference Frequency"},
    //                 new DeviceOption(){Type = "B8", Designation = "Resolution Bandwidths > 10 MHz (for R&S®FSW8/13/26)", GlobalType = "Resolution Bandwidths > 10 MHz (for R&S®FSW8/13/26)"},
    //                 new DeviceOption(){Type = "B8", Designation = "Resolution Bandwidths > 10 MHz (for R&S®FSW43/50/67/85)", GlobalType = "Resolution Bandwidths > 10 MHz (for R&S®FSW43/50/67/85)"},
    //                 new DeviceOption(){Type = "B10", Designation = "External Generator Control", GlobalType = "External Generator Control"},
    //                 new DeviceOption(){Type = "B13", Designation = "Highpass Filters for Harmonic Measurements", GlobalType = "Highpass Filters for Harmonic Measurements"},
    //                 new DeviceOption(){Type = "B17", Designation = "Digital Baseband Interface", GlobalType = "Digital Baseband Interface"},
    //                 new DeviceOption(){Type = "B18", Designation = "Solid State Drive (SSD, removable)", GlobalType = "SSD"},
    //                 new DeviceOption(){Type = "B21", Designation = "LO/IF Ports for External Mixers (for R&S®FSW26)", GlobalType = "External Mixers"},
    //                 new DeviceOption(){Type = "B21", Designation = "LO/IF Ports for External Mixers (for R&S®FSW43/50/67)", GlobalType = "External Mixers"},
    //                 new DeviceOption(){Type = "B21", Designation = "LO/IF Ports for External Mixers (for R&S®FSW85)", GlobalType = "External Mixers"},
    //                 new DeviceOption(){Type = "B24", Designation = "Preamplifier", GlobalType = "Preamplifier"},
    //                 new DeviceOption(){Type = "B25", Designation = "Electronic Attenuator (1 dB steps)", GlobalType = "Attenuator 1 dB"},
    //                 new DeviceOption(){Type = "B33", Designation = "USB Mass Memory Write Protection", GlobalType = "USB Mass Memory Write Protection"},
    //                 new DeviceOption(){Type = "B28", Designation = "28 MHz Analysis Bandwidth", GlobalType = "28 MHz Analysis Bandwidth"},
    //                 new DeviceOption(){Type = "B40", Designation = "40 MHz Analysis Bandwidth", GlobalType = "40 MHz Analysis Bandwidth"},
    //                 new DeviceOption(){Type = "B71", Designation = "Analog Baseband Inputs", GlobalType = "Analog Baseband Inputs"},
    //                 new DeviceOption(){Type = "B71E", Designation = "80 MHz Bandwidth for Analog Baseband Inputs", GlobalType = "80 MHz Bandwidth for Analog Baseband Inputs"},
    //                 new DeviceOption(){Type = "B80", Designation = "80 MHz Analysis Bandwidth", GlobalType = "80 MHz Analysis Bandwidth"},
    //                 new DeviceOption(){Type = "B160", Designation = "160 MHz Analysis Bandwidth", GlobalType = "160 MHz Analysis Bandwidth"},
    //                 new DeviceOption(){Type = "B160R", Designation = "Real-Time Spectrum Analyzer, 160 MHz", GlobalType = "Real-Time Spectrum Analyzer, 160 MHz"},
    //                 new DeviceOption(){Type = "B320", Designation = "320 MHz Analysis Bandwidth", GlobalType = "320 MHz Analysis Bandwidth"},
    //                 new DeviceOption(){Type = "B512", Designation = "512 MHz Analysis Bandwidth", GlobalType = "512 MHz Analysis Bandwidth"},
    //                 new DeviceOption(){Type = "B512R", Designation = "Real-Time Spectrum Analyzer, 512 MHz", GlobalType = "Real-Time Spectrum Analyzer, 512 MHz"},
    //                 new DeviceOption(){Type = "B1200", Designation = "1.2 GHz Analysis Bandwidth", GlobalType = "1.2 GHz Analysis Bandwidth"},
    //                 new DeviceOption(){Type = "B2000", Designation = "2 GHz Analysis Bandwidth", GlobalType = "2 GHz Analysis Bandwidth"},
    //                 new DeviceOption(){Type = "K0", Designation = "DEMO OPTION (All inclusive)", GlobalType = "DEMO OPTION (All inclusive)"},
    //                 new DeviceOption(){Type = "K6", Designation = "Pulse Measurements", GlobalType = "Pulse Measurements"},
    //                 new DeviceOption(){Type = "K6S", Designation = "Time Sidelobe Measurement", GlobalType = "Time Sidelobe Measurement"},
    //                 new DeviceOption(){Type = "K7", Designation = "Analog Modulation Analysis (AM/FM/φM)", GlobalType = "Analog Modulation Analysis"},
    //                 new DeviceOption(){Type = "K10", Designation = "GSM, EDGE, EDGE Evolution and VAMOS Measurements", GlobalType = "GSM, EDGE, EDGE Evolution and VAMOS Measurements"},
    //                 new DeviceOption(){Type = "K15", Designation = "VOR/ILS Measurements", GlobalType = "VOR/ILS Measurements"},
    //                 new DeviceOption(){Type = "K17", Designation = "Multicarrier Group Delay Measurements", GlobalType = "Multicarrier Group Delay Measurements"},
    //                 new DeviceOption(){Type = "K18", Designation = "Amplifier Measurements", GlobalType = "Amplifier Measurements"},
    //                 new DeviceOption(){Type = "K18D", Designation = "Direct DPD Measurements", GlobalType = "Direct DPD Measurements"},
    //                 new DeviceOption(){Type = "K30", Designation = "Noise Figure Measurements", GlobalType = "Noise Figure Measurements"},
    //                 new DeviceOption(){Type = "K33", Designation = "Security Write Protection of solid state drive", GlobalType = "Security Write Protection of solid state drive"},
    //                 new DeviceOption(){Type = "K40", Designation = "Phase Noise Measurements", GlobalType = "Phase Noise Measurements"},
    //                 new DeviceOption(){Type = "K50", Designation = "Spurious Measurements", GlobalType = "Spurious Measurements"},
    //                 new DeviceOption(){Type = "K54", Designation = "EMI Measurements", GlobalType = "EMI Measurements"},
    //                 new DeviceOption(){Type = "K60", Designation = "Transient Measurement Application", GlobalType = "Transient Measurement Application"},
    //                 new DeviceOption(){Type = "K60C", Designation = "Transient Chirp Measurement", GlobalType = "Transient Chirp Measurement"},
    //                 new DeviceOption(){Type = "K60H", Designation = "Transient Hop Measurement", GlobalType = "Transient Hop Measurement"},
    //                 new DeviceOption(){Type = "K70", Designation = "Vector Signal Analysis", GlobalType = "Vector Signal Analysis"},
    //                 new DeviceOption(){Type = "K72", Designation = "3GPP FDD (WCDMA) BS Measurements (incl. HSDPA and HSDPA+)", GlobalType = "3GPP FDD (WCDMA) BS Measurements (incl. HSDPA and HSDPA+)"},
    //                 new DeviceOption(){Type = "K73", Designation = "3GPP FDD (WCDMA) UE Measurements (incl. HSUPA and HSUPA+)", GlobalType = "3GPP FDD (WCDMA) UE Measurements (incl. HSUPA and HSUPA+)"},
    //                 new DeviceOption(){Type = "K76", Designation = "3GPP TDD (TD-SCDMA) BS Measurements", GlobalType = "3GPP TDD (TD-SCDMA) BS Measurements"},
    //                 new DeviceOption(){Type = "K77", Designation = "3GPP TDD (TD-SCDMA) UE Measurements", GlobalType = "3GPP TDD (TD-SCDMA) UE Measurements"},
    //                 new DeviceOption(){Type = "K82", Designation = "CDMA2000® BS (DL) Analysis", GlobalType = "CDMA2000® BS (DL) Analysis"},
    //                 new DeviceOption(){Type = "K83", Designation = "CDMA2000® MS (UL) Measurements", GlobalType = "CDMA2000® MS (UL) Measurements"},
    //                 new DeviceOption(){Type = "K84", Designation = "1xEV-DO BS (DL) Analysis", GlobalType = "1xEV-DO BS (DL) Analysis"},
    //                 new DeviceOption(){Type = "K85", Designation = "1xEV-DO MS (UL) Measurements", GlobalType = "1xEV-DO MS (UL) Measurements"},
    //                 new DeviceOption(){Type = "K91", Designation = "WLAN IEEE802.11a/b/g Measurements", GlobalType = "WLAN IEEE802.11a/b/g Measurements"},
    //                 new DeviceOption(){Type = "K91N", Designation = "WLAN IEEE802.11n Measurements", GlobalType = "WLAN IEEE802.11n Measurements"},
    //                 new DeviceOption(){Type = "K91AC", Designation = "WLAN IEEE802.11ac Measurements", GlobalType = "WLAN IEEE802.11ac Measurements"},
    //                 new DeviceOption(){Type = "K91AX", Designation = "WLAN IEEE802.11ax Measurements", GlobalType = "WLAN IEEE802.11ax Measurements"},
    //                 new DeviceOption(){Type = "K95", Designation = "WLAN IEEE802.11ad Measurements", GlobalType = "WLAN IEEE802.11ad Measurements"},
    //                 new DeviceOption(){Type = "K100", Designation = "EUTRA/LTE FDD Downlink Analysis", GlobalType = "EUTRA/LTE FDD Downlink Analysis"},
    //                 new DeviceOption(){Type = "K101", Designation = "EUTRA/LTE FDD Uplink Analysis", GlobalType = "EUTRA/LTE FDD Uplink Analysis"},
    //                 new DeviceOption(){Type = "K102", Designation = "EUTRA/LTE Downlink MIMO Analysis", GlobalType = "EUTRA/LTE Downlink MIMO Analysis"},
    //                 new DeviceOption(){Type = "K103", Designation = "EUTRA/LTE UL Advanced UL Measurements", GlobalType = "EUTRA/LTE UL Advanced UL Measurements"},
    //                 new DeviceOption(){Type = "K104", Designation = "EUTRA/LTE TDD Downlink Analysis", GlobalType = "EUTRA/LTE TDD Downlink Analysis"},
    //                 new DeviceOption(){Type = "K105", Designation = "EUTRA/LTE TDD Uplink Analysis", GlobalType = "EUTRA/LTE TDD Uplink Analysis"},
    //                 new DeviceOption(){Type = "K96", Designation = "OFDM Vector Signal Analysis Software", GlobalType = "OFDM Vector Signal Analysis Software"},
    //                 new DeviceOption(){Type = "K192", Designation = "DOCSIS 3.1 OFDM Downstream", GlobalType = "DOCSIS 3.1 OFDM Downstream"},
    //                 new DeviceOption(){Type = "K193", Designation = "DOCSIS 3.1 OFDM Upstream", GlobalType = "DOCSIS 3.1 OFDM Upstream"},
    //                 new DeviceOption(){Type = "K160RE", Designation = "160 MHz Real-Time Measurement Application, POI > 15 µs", GlobalType = "160 MHz Real-Time Measurement Application, POI > 15 µs"},
    //            },
    //            DefaultInstrOption = new List<DeviceOption>() { },
    //            LoadedInstrOption = new List<DeviceOption>() { },
    //            TraceType = new List<ParamWithId>
    //            {
    //                new ParamWithId {Id = (int)EN.TraceType.ClearWrite, Parameter = "WRIT" },
    //                new ParamWithId {Id = (int)EN.TraceType.Average, Parameter = "AVER" },
    //                new ParamWithId {Id = (int)EN.TraceType.MaxHold, Parameter = "MAXH" },
    //                new ParamWithId {Id = (int)EN.TraceType.MinHold, Parameter = "MINH" },
    //                new ParamWithId {Id = (int)EN.TraceType.View, Parameter = "VIEW" },
    //                new ParamWithId {Id = (int)EN.TraceType.Blank, Parameter = "BLAN" }
    //            },
    //            TraceDetector = new List<ParamWithId>
    //            {
    //                new ParamWithId {Id = (int)EN.TraceDetector.AutoSelect, Parameter = "Auto Select" },
    //                new ParamWithId {Id = (int)EN.TraceDetector.AutoPeak, Parameter = "APE" },
    //                new ParamWithId {Id = (int)EN.TraceDetector.Average, Parameter = "AVER" },
    //                new ParamWithId {Id = (int)EN.TraceDetector.MaxPeak, Parameter = "POS" },
    //                new ParamWithId {Id = (int)EN.TraceDetector.MinPeak, Parameter = "NEG" },
    //                new ParamWithId {Id = (int)EN.TraceDetector.Sample, Parameter = "SAMP" },
    //                new ParamWithId {Id = (int)EN.TraceDetector.RMS, Parameter = "RMS" }
    //            },
    //            SweepType =  new List<ParamWithId>
    //            {
    //                new ParamWithId {Id = (int)EN.SweepType.Auto, Parameter = "AUTO" },
    //                new ParamWithId {Id = (int)EN.SweepType.Sweep, Parameter = "SWE" },
    //                new ParamWithId {Id = (int)EN.SweepType.FFT, Parameter = "FFT" }
    //            },
    //            LevelUnits = new List<ParamWithId>
    //            {
    //                new ParamWithId {Id = (int)MEN.LevelUnit.dBm, Parameter = "DBM" },
    //                new ParamWithId {Id = (int)MEN.LevelUnit.dBmV, Parameter = "DBMV" },
    //                new ParamWithId {Id = (int)MEN.LevelUnit.dBµV, Parameter = "DBUV" },
    //                new ParamWithId {Id = (int)MEN.LevelUnit.dBµVm, Parameter = "DBUV/M" }
    //            },
    //            SweepPointArr = new int[]{ 101, 125, 155, 173, 201, 251, 301, 313, 345, 401, 501, 601, 625, 691, 701, 801, 901, 1001, 1251, 1383, 1999, 2001, 2501, 2765, 3001, 4001, 5001, 6001, 7001, 8001, 9001, 10001, 11001, 12001, 13001, 14001, 15001, 16001, 17001, 18001, 19001, 20001, 21001, 22001, 23001, 24001, 25001, 26001, 27001, 28001, 29001, 30001, 31001, 32001 },
    //            DefaultSweepPoint = 1001,
    //            RBWArr = new decimal[]{ 1, 2, 3, 5, 10, 20, 30, 50, 100, 200, 300, 500, 1000, 2000, 3000, 5000, 6250, 10000, 20000, 30000, 50000, 100000, 200000, 300000, 500000, 1000000, 2000000, 3000000, 5000000, 10000000 },
    //            VBWArr = new decimal[]{ 1, 2, 3, 5, 10, 20, 30, 50, 100, 200, 300, 500, 1000, 2000, 3000, 5000, 10000, 20000, 30000, 50000, 100000, 200000, 300000, 500000, 1000000, 2000000, 3000000, 5000000, 10000000, 20000000, 28000000, 40000000 },
    //            CouplingRatio = true,
    //            AttMax = 70,
    //            AttStep = 5,
    //            PreAmp = false,
    //            Battery = false,
    //            NdB = true,
    //            OBW = true,
    //            ChnPow= true
    //        },
    //        #endregion FSW
    //        #region FSV
    //        new LocalSpectrumAnalyzerInfo
    //        {
    //            InstrManufacture = 1,  NumberOfTrace = 3, ChangeableSweepType = true, SweepPointFix = false, InstrModel = "FSV",
    //            HiSpeed = true,
    //            InstrOption = new List<DeviceOption>()
    //            {
    //                 new DeviceOption(){Type = "B1", Designation = "Ruggedized Housing", GlobalType = "Ruggedized Housing"},
    //                 new DeviceOption(){Type = "B3", Designation = "AM/FM Audio Demodulator", GlobalType = "Audio Demodulator"},
    //                 new DeviceOption(){Type = "B4", Designation = "OCXO, Precision Reference Frequency", GlobalType = "OCXO, Precision Reference Frequency"},
    //                 new DeviceOption(){Type = "B4", Designation = "OCXO, Enhanced Frequency Stability", GlobalType = "OCXO, Enhanced Frequency Stability"},
    //                 new DeviceOption(){Type = "B5", Designation = "Additional Interfaces (IF/video/AM/FM output, AUX port, trigger output, two additional USB ports)", GlobalType = "Additional Interfaces"},
    //                 new DeviceOption(){Type = "B9", Designation = "Tracking Generator, 100 kHz to 4 GHz/7 GHz", GlobalType = "Tracking Generator"},
    //                 new DeviceOption(){Type = "B10", Designation = "External Generator Control", GlobalType = "External Generator Control"},
    //                 new DeviceOption(){Type = "B17", Designation = "Digital Baseband Interface", GlobalType = "Digital Baseband Interface"},
    //                 new DeviceOption(){Type = "B18", Designation = "Solid State Drive (SSD, removable)", GlobalType = "SSD"},
    //                 new DeviceOption(){Type = "B19", Designation = "Spare Hard Disk Drive (HDD, removable)", GlobalType = "HDD"},
    //                 new DeviceOption(){Type = "B21", Designation = "LO/IF Ports for External Mixers", GlobalType = "External Mixers"},
    //                 new DeviceOption(){Type = "B22", Designation = "RF Preamplifier (100 kHz to 7 GHz)", GlobalType = "Preamplifier"},
    //                 new DeviceOption(){Type = "B24", Designation = "Preamplifier, 9 kHz to 13.6 GHz", GlobalType = "Preamplifier"},
    //                 new DeviceOption(){Type = "B24", Designation = "Preamplifier, 9 kHz to 30 GHz", GlobalType = "Preamplifier"},
    //                 new DeviceOption(){Type = "B24", Designation = "Preamplifier, 9 kHz to 40 GHz", GlobalType = "Preamplifier"},
    //                 new DeviceOption(){Type = "B25", Designation = "Electronic Attenuator (1 dB steps)", GlobalType = "Attenuator 1 dB"},
    //                 new DeviceOption(){Type = "B30", Designation = "DC Power Supply, 12 V to 15 V", GlobalType = "DC Power Supply"},
    //                 new DeviceOption(){Type = "B32", Designation = "Lithium-Ion Battery Pack", GlobalType = "Lithium-Ion Battery Pack"},
    //                 new DeviceOption(){Type = "B34", Designation = "Battery Charger, for R&S®FSV-B32 Li-ion battery pack", GlobalType = "Battery Charger"},
    //                 new DeviceOption(){Type = "B70", Designation = "40 MHz Analysis Bandwidth", GlobalType = "40 MHz Analysis Bandwidth"},
    //                 new DeviceOption(){Type = "B160", Designation = "160 MHz Analysis Bandwidth (for R&S®FSV4 and R&S®FSV7)", GlobalType = "160 MHz Analysis Bandwidth"},
    //                 new DeviceOption(){Type = "B160", Designation = "160 MHz Analysis Bandwidth (for R&S®FSV13)", GlobalType = "160 MHz Analysis Bandwidth"},
    //                 new DeviceOption(){Type = "B160", Designation = "160 MHz Analysis Bandwidth (for R&S®FSV30 und R&S®FSV40)", GlobalType = "160 MHz Analysis Bandwidth"},
    //                 new DeviceOption(){Type = "K0", Designation = "DEMO OPTION (All inclusive)", GlobalType = "DEMO OPTION (All inclusive)"},
    //                 new DeviceOption(){Type = "K7", Designation = "Analog Modulation Analysis (AM/FM/φM)", GlobalType = "Analog Modulation Analysis"},
    //                 new DeviceOption(){Type = "K7S", Designation = "FM Stereo Measurements (for R&S®FSV-K7)", GlobalType = "FM Stereo Measurements"},
    //                 new DeviceOption(){Type = "K8", Designation = "Bluetooth®/EDR Measurement Application", GlobalType = "Bluetooth®/EDR Measurement Application"},
    //                 new DeviceOption(){Type = "K9", Designation = "Power Sensor Support (power measurement with the R&S®NRP-Zxx power sensors)", GlobalType = "Power Sensor Support"},
    //                 new DeviceOption(){Type = "K10", Designation = "GSM/EDGE/EDGE Evolution Analysis", GlobalType = "GSM/EDGE/EDGE Evolution Analysis"},
    //                 new DeviceOption(){Type = "K14", Designation = "Spectrogram Measurements", GlobalType = "Spectrogram Measurements"},
    //                 new DeviceOption(){Type = "K30", Designation = "Noise Figure and Gain Measurements", GlobalType = "Noise Figure and Gain Measurements"},
    //                 new DeviceOption(){Type = "K40", Designation = "Phase Noise Measurements", GlobalType = "Phase Noise Measurements"},
    //                 new DeviceOption(){Type = "K70", Designation = "Vector Signal Analysis", GlobalType = "Vector Signal Analysis"},
    //                 new DeviceOption(){Type = "K72", Designation = "3GPP BS (DL) Analysis, incl. HSDPA and HSDPA+", GlobalType = "3GPP BS (DL) Analysis, incl. HSDPA and HSDPA+"},
    //                 new DeviceOption(){Type = "K73", Designation = "3GPP UE (UL) Analysis, incl. HSUPA", GlobalType = "3GPP UE (UL) Analysis, incl. HSUPA"},
    //                 new DeviceOption(){Type = "K76", Designation = "TD-SCDMA BS Measurements", GlobalType = "TD-SCDMA BS Measurements"},
    //                 new DeviceOption(){Type = "K77", Designation = "TD-SCDMA UE Measurements", GlobalType = "TD-SCDMA UE Measurements"},
    //                 new DeviceOption(){Type = "K82", Designation = "CDMA2000® BS (DL) Analysis", GlobalType = "CDMA2000® BS (DL) Analysis"},
    //                 new DeviceOption(){Type = "K83", Designation = "CDMA2000® MS (UL) Measurements", GlobalType = "CDMA2000® MS (UL) Measurements"},
    //                 new DeviceOption(){Type = "K84", Designation = "1xEV-DO BS (DL) Analysis", GlobalType = "1xEV-DO BS (DL) Analysis"},
    //                 new DeviceOption(){Type = "K85", Designation = "1xEV-DO MS (UL) Measurements", GlobalType = "1xEV-DO MS (UL) Measurements"},
    //                 new DeviceOption(){Type = "K91", Designation = "WLAN IEEE802.11a/b/g/j Analysis", GlobalType = "WLAN IEEE802.11a/b/g/j Analysis"},
    //                 new DeviceOption(){Type = "K91N", Designation = "WLAN IEEE802.11n Analysis", GlobalType = "WLAN IEEE802.11n Analysis"},
    //                 new DeviceOption(){Type = "K91AC", Designation = "WLAN IEEE802.11ac Analysis", GlobalType = "WLAN IEEE802.11ac Analysis"},
    //                 new DeviceOption(){Type = "K91P", Designation = "WLAN IEEE802.11ac Analysis", GlobalType = "WLAN IEEE802.11ac Analysis"},
    //                 new DeviceOption(){Type = "K93", Designation = "WiMAX™ IEEE802.16e OFDM/OFDMA Analysis", GlobalType = "WiMAX™ IEEE802.16e OFDM/OFDMA Analysis"},
    //                 new DeviceOption(){Type = "K100", Designation = "EUTRA/LTE FDD Downlink Analysis", GlobalType = "EUTRA/LTE FDD Downlink Analysis"},
    //                 new DeviceOption(){Type = "K101", Designation = "EUTRA/LTE FDD Uplink Analysis", GlobalType = "EUTRA/LTE FDD Uplink Analysis"},
    //                 new DeviceOption(){Type = "K102", Designation = "EUTRA/LTE Downlink MIMO Analysis", GlobalType = "EUTRA/LTE Downlink MIMO Analysis"},
    //                 new DeviceOption(){Type = "K104", Designation = "EUTRA/LTE TDD Downlink Analysis", GlobalType = "EUTRA/LTE TDD Downlink Analysis"},
    //                 new DeviceOption(){Type = "K105", Designation = "EUTRA/LTE TDD Uplink Analysis", GlobalType = "EUTRA/LTE TDD Uplink Analysis"},
    //                 new DeviceOption(){Type = "K96", Designation = "OFDM Vector Signal Analysis Software", GlobalType = "OFDM Vector Signal Analysis Software"},
    //                 new DeviceOption(){Type = "K130PC", Designation = "Distortion Analysis Software", GlobalType = "Distortion Analysis Software"},
    //            },
    //            DefaultInstrOption = new List<DeviceOption>() { },
    //            LoadedInstrOption = new List<DeviceOption>() { },
    //            TraceType = new List<ParamWithId>
    //            {
    //                new ParamWithId {Id = (int)EN.TraceType.ClearWrite, Parameter = "WRIT" },
    //                new ParamWithId {Id = (int)EN.TraceType.Average, Parameter = "AVER" },
    //                new ParamWithId {Id = (int)EN.TraceType.MaxHold, Parameter = "MAXH" },
    //                new ParamWithId {Id = (int)EN.TraceType.MinHold, Parameter = "MINH" },
    //                new ParamWithId {Id = (int)EN.TraceType.View, Parameter = "VIEW" },
    //                new ParamWithId {Id = (int)EN.TraceType.Blank, Parameter = "BLAN" }
    //            },
    //            TraceDetector = new List<ParamWithId>
    //            {
    //                new ParamWithId {Id = (int)EN.TraceDetector.AutoSelect, Parameter = "Auto Select" },
    //                new ParamWithId {Id = (int)EN.TraceDetector.AutoPeak, Parameter = "APE" },
    //                new ParamWithId {Id = (int)EN.TraceDetector.Average, Parameter = "AVER" },
    //                new ParamWithId {Id = (int)EN.TraceDetector.MaxPeak, Parameter = "POS" },
    //                new ParamWithId {Id = (int)EN.TraceDetector.MinPeak, Parameter = "NEG" },
    //                new ParamWithId {Id = (int)EN.TraceDetector.Sample, Parameter = "SAMP" },
    //                new ParamWithId {Id = (int)EN.TraceDetector.RMS, Parameter = "RMS" }
    //            },
    //            SweepType =  new List<ParamWithId>
    //            {
    //                new ParamWithId {Id = (int)EN.SweepType.Auto, Parameter = "AUTO" },
    //                new ParamWithId {Id = (int)EN.SweepType.Sweep, Parameter = "SWE" },
    //                new ParamWithId {Id = (int)EN.SweepType.FFT, Parameter = "FFT" }
    //            },
    //            LevelUnits = new List<ParamWithId>
    //            {
    //                new ParamWithId {Id = (int)MEN.LevelUnit.dBm, Parameter = "DBM" },
    //                new ParamWithId {Id = (int)MEN.LevelUnit.dBmV, Parameter = "DBMV" },
    //                new ParamWithId {Id = (int)MEN.LevelUnit.dBµV, Parameter = "DBUV" },
    //                new ParamWithId {Id = (int)MEN.LevelUnit.dBµVm, Parameter = "DBUV/M" }
    //            },
    //            SweepPointArr = new int[]{ 101, 125, 155, 173, 201, 251, 301, 313, 345, 401, 501, 601, 625, 691, 701, 801, 901, 1001, 1251, 1383, 1999, 2001, 2501, 2765, 3001, 4001, 5001, 6001, 7001, 8001, 9001, 10001, 11001, 12001, 13001, 14001, 15001, 16001, 17001, 18001, 19001, 20001, 21001, 22001, 23001, 24001, 25001, 26001, 27001, 28001, 29001, 30001, 31001, 32001 },
    //            DefaultSweepPoint = 691,
    //            RBWArr = new decimal[]{ 1, 2, 3, 5, 10, 20, 30, 50, 100, 200, 300, 500, 1000, 2000, 3000, 5000, 6250, 10000, 20000, 30000, 50000, 100000, 200000, 300000, 500000, 1000000, 2000000, 3000000, 5000000, 10000000 },
    //            VBWArr = new decimal[]{ 1, 2, 3, 5, 10, 20, 30, 50, 100, 200, 300, 500, 1000, 2000, 3000, 5000, 10000, 20000, 30000, 50000, 100000, 200000, 300000, 500000, 1000000, 2000000, 3000000, 5000000, 10000000, 20000000, 28000000, 40000000 },
    //            CouplingRatio = true,
    //            AttMax = 70,
    //            AttStep = 5,
    //            PreAmp = false,
    //            Battery = false,
    //            NdB = true,
    //            OBW = true,
    //            ChnPow= true
    //        },
    //        #endregion FSV
    //        #region FSVA
    //        new LocalSpectrumAnalyzerInfo
    //        {
    //            InstrManufacture = 1, NumberOfTrace = 3, ChangeableSweepType = true, SweepPointFix = false, InstrModel = "FSVA",
    //            HiSpeed = true,
    //            InstrOption = new List<DeviceOption>()
    //            {
    //                new DeviceOption(){Type = "B1", Designation = "Ruggedized Housing", GlobalType = "Ruggedized Housing"},
    //                new DeviceOption(){Type = "B3", Designation = "AM/FM Audio Demodulator", GlobalType = "Audio Demodulator"},
    //                new DeviceOption(){Type = "B4", Designation = "OCXO, Precision Reference Frequency", GlobalType = "OCXO, Precision Reference Frequency"},
    //                new DeviceOption(){Type = "B4", Designation = "OCXO, Enhanced Frequency Stability", GlobalType = "OCXO, Enhanced Frequency Stability"},
    //                new DeviceOption(){Type = "B5", Designation = "Additional Interfaces (IF/video/AM/FM output, AUX port, trigger output, two additional USB ports)", GlobalType = "Additional Interfaces"},
    //                new DeviceOption(){Type = "B9", Designation = "Tracking Generator, 100 kHz to 4 GHz/7 GHz", GlobalType = "Tracking Generator"},
    //                new DeviceOption(){Type = "B10", Designation = "External Generator Control", GlobalType = "External Generator Control"},
    //                new DeviceOption(){Type = "B11", Designation = "YIG Preselector Bypass for R&S®FSVA13", GlobalType = "YIG Preselector Bypass for R&S®FSVA13"},
    //                new DeviceOption(){Type = "B11", Designation = "YIG Preselector Bypass for R&S®FSVA30", GlobalType = "YIG Preselector Bypass for R&S®FSVA30"},
    //                new DeviceOption(){Type = "B11", Designation = "YIG Preselector Bypass for R&S®FSVA40", GlobalType = "YIG Preselector Bypass for R&S®FSVA40"},
    //                new DeviceOption(){Type = "B14", Designation = "Ultra-High Precision Frequency Reference", GlobalType = "Ultra-High Precision Frequency Reference"},
    //                new DeviceOption(){Type = "B17", Designation = "Digital Baseband Interface", GlobalType = "Digital Baseband Interface"},
    //                new DeviceOption(){Type = "B18", Designation = "Solid State Drive (SSD, removable)", GlobalType = "SSD"},
    //                new DeviceOption(){Type = "B19", Designation = "Spare Hard Disk Drive (HDD, removable)", GlobalType = "HDD"},
    //                new DeviceOption(){Type = "B21", Designation = "LO/IF Ports for External Mixers", GlobalType = "External Mixers"},
    //                new DeviceOption(){Type = "B22", Designation = "RF Preamplifier (100 kHz to 7 GHz)", GlobalType = "Preamplifier"},
    //                new DeviceOption(){Type = "B24", Designation = "Preamplifier, 9 kHz to 13.6 GHz", GlobalType = "Preamplifier"},
    //                new DeviceOption(){Type = "B24", Designation = "Preamplifier, 9 kHz to 30 GHz", GlobalType = "Preamplifier"},
    //                new DeviceOption(){Type = "B24", Designation = "Preamplifier, 9 kHz to 40 GHz", GlobalType = "Preamplifier"},
    //                new DeviceOption(){Type = "B25", Designation = "Electronic Attenuator (1 dB steps)", GlobalType = "Attenuator 1 dB"},
    //                new DeviceOption(){Type = "B30", Designation = "DC Power Supply, 12 V to 15 V", GlobalType = "DC Power Supply"},
    //                new DeviceOption(){Type = "B32", Designation = "Lithium-Ion Battery Pack", GlobalType = "Lithium-Ion Battery Pack"},
    //                new DeviceOption(){Type = "B33", Designation = "USB Mass Memory Write Protection", GlobalType = "USB Mass Memory Write Protection"},
    //                new DeviceOption(){Type = "B34", Designation = "Battery Charger, for R&S®FSV-B32 Li-ion battery pack", GlobalType = "Battery Charger"},
    //                new DeviceOption(){Type = "B40", Designation = "40 MHz Analysis Bandwidth", GlobalType = "40 MHz Analysis Bandwidth"},
    //                new DeviceOption(){Type = "B160", Designation = "160 MHz Analysis Bandwidth (for R&S®FSVA4 and R&S®FSVA7)", GlobalType = "160 MHz Analysis Bandwidth"},
    //                new DeviceOption(){Type = "B160", Designation = "160 MHz Analysis Bandwidth (for R&S®FSVA13)", GlobalType = "160 MHz Analysis Bandwidth"},
    //                new DeviceOption(){Type = "B160", Designation = "160 MHz Analysis Bandwidth (for R&S®FSVA30 und R&S®FSVA40)", GlobalType = "160 MHz Analysis Bandwidth"},
    //                new DeviceOption(){Type = "K0", Designation = "DEMO OPTION (All inclusive)", GlobalType = "DEMO OPTION (All inclusive)"},
    //                new DeviceOption(){Type = "K7", Designation = "Analog Modulation Analysis (AM/FM/φM)", GlobalType = "Analog Modulation Analysis"},
    //                new DeviceOption(){Type = "K7S", Designation = "FM Stereo Measurements (for R&S®FSV-K7)", GlobalType = "FM Stereo Measurements"},
    //                new DeviceOption(){Type = "K8", Designation = "Bluetooth®/EDR Measurement Application", GlobalType = "Bluetooth®/EDR Measurement Application"},
    //                new DeviceOption(){Type = "K9", Designation = "Power Sensor Support (power measurement with the R&S®NRP-Zxx power sensors)", GlobalType = "Power Sensor Support"},
    //                new DeviceOption(){Type = "K10", Designation = "GSM/EDGE/EDGE Evolution Analysis", GlobalType = "GSM/EDGE/EDGE Evolution Analysis"},
    //                new DeviceOption(){Type = "K14", Designation = "Spectrogram Measurements", GlobalType = "Spectrogram Measurements"},
    //                new DeviceOption(){Type = "K30", Designation = "Noise Figure and Gain Measurements", GlobalType = "Noise Figure and Gain Measurements"},
    //                new DeviceOption(){Type = "K40", Designation = "Phase Noise Measurements", GlobalType = "Phase Noise Measurements"},
    //                new DeviceOption(){Type = "K54", Designation = "EMI Measurement Application", GlobalType = "EMI Measurement Application"},
    //                new DeviceOption(){Type = "K70", Designation = "Vector Signal Analysis", GlobalType = "Vector Signal Analysis"},
    //                new DeviceOption(){Type = "K72", Designation = "3GPP BS (DL) Analysis, incl. HSDPA and HSDPA+", GlobalType = "3GPP BS (DL) Analysis, incl. HSDPA and HSDPA+"},
    //                new DeviceOption(){Type = "K73", Designation = "3GPP UE (UL) Analysis, incl. HSUPA", GlobalType = "3GPP UE (UL) Analysis, incl. HSUPA"},
    //                new DeviceOption(){Type = "K76", Designation = "TD-SCDMA BS Measurements", GlobalType = "TD-SCDMA BS Measurements"},
    //                new DeviceOption(){Type = "K77", Designation = "TD-SCDMA UE Measurements", GlobalType = "TD-SCDMA UE Measurements"},
    //                new DeviceOption(){Type = "K82", Designation = "CDMA2000® BS (DL) Analysis", GlobalType = "CDMA2000® BS (DL) Analysis"},
    //                new DeviceOption(){Type = "K83", Designation = "CDMA2000® MS (UL) Measurements", GlobalType = "CDMA2000® MS (UL) Measurements"},
    //                new DeviceOption(){Type = "K84", Designation = "1xEV-DO BS (DL) Analysis", GlobalType = "1xEV-DO BS (DL) Analysis"},
    //                new DeviceOption(){Type = "K85", Designation = "1xEV-DO MS (UL) Measurements", GlobalType = "1xEV-DO MS (UL) Measurements"},
    //                new DeviceOption(){Type = "K91", Designation = "WLAN IEEE802.11a/b/g/j Analysis", GlobalType = "WLAN IEEE802.11a/b/g/j Analysis"},
    //                new DeviceOption(){Type = "K91N", Designation = "WLAN IEEE802.11n Analysis", GlobalType = "WLAN IEEE802.11n Analysis"},
    //                new DeviceOption(){Type = "K91AC", Designation = "WLAN IEEE802.11ac Analysis", GlobalType = "WLAN IEEE802.11ac Analysis"},
    //                new DeviceOption(){Type = "K91P", Designation = "WLAN IEEE802.11ac Analysis", GlobalType = "WLAN IEEE802.11ac Analysis"},
    //                new DeviceOption(){Type = "K93", Designation = "WiMAX™ IEEE802.16e OFDM/OFDMA Analysis", GlobalType = "WiMAX™ IEEE802.16e OFDM/OFDMA Analysis"},
    //                new DeviceOption(){Type = "K100", Designation = "EUTRA/LTE FDD Downlink Analysis", GlobalType = "EUTRA/LTE FDD Downlink Analysis"},
    //                new DeviceOption(){Type = "K101", Designation = "EUTRA/LTE FDD Uplink Analysis", GlobalType = "EUTRA/LTE FDD Uplink Analysis"},
    //                new DeviceOption(){Type = "K102", Designation = "EUTRA/LTE Downlink MIMO Analysis", GlobalType = "EUTRA/LTE Downlink MIMO Analysis"},
    //                new DeviceOption(){Type = "K104", Designation = "EUTRA/LTE TDD Downlink Analysis", GlobalType = "EUTRA/LTE TDD Downlink Analysis"},
    //                new DeviceOption(){Type = "K105", Designation = "EUTRA/LTE TDD Uplink Analysis", GlobalType = "EUTRA/LTE TDD Uplink Analysis"},
    //                new DeviceOption(){Type = "K96PC", Designation = "OFDM Vector Signal Analysis Software", GlobalType = "OFDM Vector Signal Analysis Software"},
    //                new DeviceOption(){Type = "K130PC", Designation = "Distortion Analysis Software", GlobalType = "Distortion Analysis Software"},
    //            },
    //            DefaultInstrOption = new List<DeviceOption>() { },
    //            LoadedInstrOption = new List<DeviceOption>() { },
    //            TraceType = new List<ParamWithId>
    //            {
    //                new ParamWithId {Id = (int)EN.TraceType.ClearWrite, Parameter = "WRIT" },
    //                new ParamWithId {Id = (int)EN.TraceType.Average, Parameter = "AVER" },
    //                new ParamWithId {Id = (int)EN.TraceType.MaxHold, Parameter = "MAXH" },
    //                new ParamWithId {Id = (int)EN.TraceType.MinHold, Parameter = "MINH" },
    //                new ParamWithId {Id = (int)EN.TraceType.View, Parameter = "VIEW" },
    //                new ParamWithId {Id = (int)EN.TraceType.Blank, Parameter = "BLAN" }
    //            },
    //            TraceDetector = new List<ParamWithId>
    //            {
    //                new ParamWithId {Id = (int)EN.TraceDetector.AutoSelect, Parameter = "Auto Select" },
    //                new ParamWithId {Id = (int)EN.TraceDetector.AutoPeak, Parameter = "APE" },
    //                new ParamWithId {Id = (int)EN.TraceDetector.Average, Parameter = "AVER" },
    //                new ParamWithId {Id = (int)EN.TraceDetector.MaxPeak, Parameter = "POS" },
    //                new ParamWithId {Id = (int)EN.TraceDetector.MinPeak, Parameter = "NEG" },
    //                new ParamWithId {Id = (int)EN.TraceDetector.Sample, Parameter = "SAMP" },
    //                new ParamWithId {Id = (int)EN.TraceDetector.RMS, Parameter = "RMS" }
    //            },
    //            SweepType =  new List<ParamWithId>
    //            {
    //                new ParamWithId {Id = (int)EN.SweepType.Auto, Parameter = "AUTO" },
    //                new ParamWithId {Id = (int)EN.SweepType.Sweep, Parameter = "SWE" },
    //                new ParamWithId {Id = (int)EN.SweepType.FFT, Parameter = "FFT" }
    //            },
    //            LevelUnits = new List<ParamWithId>
    //            {
    //                new ParamWithId {Id = (int)MEN.LevelUnit.dBm, Parameter = "DBM" },
    //                new ParamWithId {Id = (int)MEN.LevelUnit.dBmV, Parameter = "DBMV" },
    //                new ParamWithId {Id = (int)MEN.LevelUnit.dBµV, Parameter = "DBUV" },
    //                new ParamWithId {Id = (int)MEN.LevelUnit.dBµVm, Parameter = "DBUV/M" }
    //            },
    //            SweepPointArr = new int[]{ 101, 125, 155, 173, 201, 251, 301, 313, 345, 401, 501, 601, 625, 691, 701, 801, 901, 1001, 1251, 1383, 1999, 2001, 2501, 2765, 3001, 4001, 5001, 6001, 7001, 8001, 9001, 10001, 11001, 12001, 13001, 14001, 15001, 16001, 17001, 18001, 19001, 20001, 21001, 22001, 23001, 24001, 25001, 26001, 27001, 28001, 29001, 30001, 31001, 32001 },
    //            DefaultSweepPoint = 691,
    //            RBWArr = new decimal[]{ 1, 2, 3, 5, 10, 20, 30, 50, 100, 200, 300, 500, 1000, 2000, 3000, 5000, 6250, 10000, 20000, 30000, 50000, 100000, 200000, 300000, 500000, 1000000, 2000000, 3000000, 5000000, 10000000 },
    //            VBWArr = new decimal[]{ 1, 2, 3, 5, 10, 20, 30, 50, 100, 200, 300, 500, 1000, 2000, 3000, 5000, 10000, 20000, 30000, 50000, 100000, 200000, 300000, 500000, 1000000, 2000000, 3000000, 5000000, 10000000, 20000000, 28000000, 40000000 },
    //            CouplingRatio = true,
    //            AttMax = 70,
    //            AttStep = 5,
    //            PreAmp = false,
    //            Battery = false,
    //            NdB = true,
    //            OBW = true,
    //            ChnPow= true
    //         },
    //        #endregion FSVA
    //        #region ESRP
    //        new LocalSpectrumAnalyzerInfo
    //        {
    //            InstrManufacture = 1, NumberOfTrace = 3, ChangeableSweepType = true, SweepPointFix = false, InstrModel = "ESRP",
    //            HiSpeed = true,
    //            InstrOption = new List<DeviceOption>()
    //            {
    //                new DeviceOption(){Type = "B1", Designation = "Ruggedized Housing", GlobalType = "Ruggedized Housing"},
    //                new DeviceOption(){Type = "B2", Designation = "Preselection and RF Preamplifier", GlobalType = "Preselection and Preamplifier"},
    //                new DeviceOption(){Type = "B22", Designation = "RF Preamplifier (100 kHz to 7 GHz)", GlobalType = "Preamplifier"},
    //                new DeviceOption(){Type = "B4", Designation = "OCXO, Precision Reference Frequency", GlobalType = "OCXO, Precision Reference Frequency"},
    //                new DeviceOption(){Type = "B4", Designation = "OCXO, Enhanced Frequency Stability", GlobalType = "OCXO, Enhanced Frequency Stability"},
    //                new DeviceOption(){Type = "B9", Designation = "Tracking Generator, 9 kHz to 7 GHz", GlobalType = "Tracking Generator"},
    //                new DeviceOption(){Type = "B18", Designation = "Solid State Drive (SSD, removable)", GlobalType = "SSD"},
    //                new DeviceOption(){Type = "B19", Designation = "Spare Hard Disk Drive (HDD, removable)", GlobalType = "HDD"},
    //                new DeviceOption(){Type = "B29", Designation = "Frequency Extension 10 Hz, including EMI bandwidths in decade steps", GlobalType = "Low Start Frequency"},
    //                new DeviceOption(){Type = "B30", Designation = "DC Power Supply, 12 V to 15 V", GlobalType = "DC Power Supply"},
    //                new DeviceOption(){Type = "B32", Designation = "Lithium-Ion Battery Pack", GlobalType = "Lithium-Ion Battery Pack"},
    //                new DeviceOption(){Type = "K0", Designation = "DEMO OPTION (All inclusive)", GlobalType = "DEMO OPTION (All inclusive)"},
    //                new DeviceOption(){Type = "K53", Designation = "Time Domain Scan", GlobalType = "Time Domain Scan"},
    //                new DeviceOption(){Type = "K56", Designation = "IF Analysis", GlobalType = "IF Analysis"},
    //            },
    //            DefaultInstrOption = new List<DeviceOption>() { },
    //            LoadedInstrOption = new List<DeviceOption>() { },
    //            TraceType = new List<ParamWithId>
    //            {
    //                new ParamWithId {Id = (int)EN.TraceType.ClearWrite, Parameter = "WRIT" },
    //                new ParamWithId {Id = (int)EN.TraceType.Average, Parameter = "AVER" },
    //                new ParamWithId {Id = (int)EN.TraceType.MaxHold, Parameter = "MAXH" },
    //                new ParamWithId {Id = (int)EN.TraceType.MinHold, Parameter = "MINH" },
    //                new ParamWithId {Id = (int)EN.TraceType.Blank, Parameter = "BLAN" },
    //                new ParamWithId {Id = (int)EN.TraceType.View, Parameter = "VIEW" },
    //            },
    //            TraceDetector = new List<ParamWithId>
    //            {
    //                new ParamWithId {Id = (int)EN.TraceDetector.AutoSelect, Parameter = "Auto Select" },
    //                new ParamWithId {Id = (int)EN.TraceDetector.AutoPeak, Parameter = "APE" },
    //                new ParamWithId {Id = (int)EN.TraceDetector.Average, Parameter = "AVER" },
    //                new ParamWithId {Id = (int)EN.TraceDetector.MaxPeak, Parameter = "POS" },
    //                new ParamWithId {Id = (int)EN.TraceDetector.MinPeak, Parameter = "NEG" },
    //                new ParamWithId {Id = (int)EN.TraceDetector.Sample, Parameter = "SAMP" },
    //                new ParamWithId {Id = (int)EN.TraceDetector.RMS, Parameter = "RMS" }
    //            },
    //            SweepType =  new List<ParamWithId>
    //            {
    //                new ParamWithId {Id = (int)EN.SweepType.Auto, Parameter = "AUTO" },
    //                new ParamWithId {Id = (int)EN.SweepType.Sweep, Parameter = "SWE" },
    //                new ParamWithId {Id = (int)EN.SweepType.FFT, Parameter = "FFT" }
    //            },
    //            LevelUnits = new List<ParamWithId>
    //            {
    //                new ParamWithId {Id = (int)MEN.LevelUnit.dBm, Parameter = "DBM" },
    //                new ParamWithId {Id = (int)MEN.LevelUnit.dBmV, Parameter = "DBMV" },
    //                new ParamWithId {Id = (int)MEN.LevelUnit.dBµV, Parameter = "DBUV" },
    //                new ParamWithId {Id = (int)MEN.LevelUnit.dBµVm, Parameter = "DBUV/M" }
    //            },
    //            SweepPointArr = new int[]{ 101, 125, 155, 173, 201, 251, 301, 313, 345, 401, 501, 601, 625, 691, 701, 801, 901, 1001, 1251, 1383, 1999, 2001, 2501, 2765, 3001, 4001, 5001, 6001, 7001, 8001, 9001, 10001, 11001, 12001, 13001, 14001, 15001, 16001, 17001, 18001, 19001, 20001, 21001, 22001, 23001, 24001, 25001, 26001, 27001, 28001, 29001, 30001, 31001, 32001 },
    //            DefaultSweepPoint = 691,
    //            RBWArr = new decimal[]{ 1, 2, 3, 5, 10, 20, 30, 50, 100, 200, 300, 500, 1000, 2000, 3000, 5000, 6250, 10000, 20000, 30000, 50000, 100000, 200000, 300000, 500000, 1000000, 2000000, 3000000, 5000000, 10000000 },
    //            VBWArr = new decimal[]{ 1, 2, 3, 5, 10, 20, 30, 50, 100, 200, 300, 500, 1000, 2000, 3000, 5000, 10000, 20000, 30000, 50000, 100000, 200000, 300000, 500000, 1000000, 2000000, 3000000, 5000000, 10000000, 20000000, 28000000, 40000000 },
    //            CouplingRatio = true,
    //            AttMax = 70,
    //            AttStep = 5,
    //            PreAmp = false,
    //            Battery = false,
    //            NdB = true,
    //            OBW = true,
    //            ChnPow= true
    //        },
    //        #endregion ESRP
    //        #region FPH
    //        new LocalSpectrumAnalyzerInfo
    //        {
    //            InstrManufacture = 1, NumberOfTrace = 1, ChangeableSweepType = false, SweepPointFix = true, InstrModel = "FPH",
    //            HiSpeed = false,
    //            InstrOption = new List<DeviceOption>()
    //            {
    //                new DeviceOption(){Type = "B22", Designation = "Preamplifier", GlobalType = "Preamplifier"},
    //                new DeviceOption(){Type = "B3", Designation = "Frequency Range to 3 GHz", GlobalType = "Frequency Range to 3 GHz"},
    //                new DeviceOption(){Type = "B4", Designation = "Frequency Range to 4 GHz", GlobalType = "Frequency Range to 4 GHz"},
    //                new DeviceOption(){Type = "K0", Designation = "DEMO OPTION (All inclusive)", GlobalType = "DEMO OPTION (All inclusive)"},
    //                new DeviceOption(){Type = "K7", Designation = "Analog Modulation Analysis AM/FM", GlobalType = "Analog Modulation Analysis AM/FM"},
    //                new DeviceOption(){Type = "K9", Designation = "Power Sensor Support", GlobalType = "Power Sensor Support"},
    //                new DeviceOption(){Type = "K15", Designation = "Interference Analysis", GlobalType = "Interference Analysis"},
    //                new DeviceOption(){Type = "K16", Designation = "Signal Strength Mapping", GlobalType = "Signal Strength Mapping"},
    //                new DeviceOption(){Type = "K19", Designation = "Channel Power Meter", GlobalType = "Channel Power Meter"},
    //                new DeviceOption(){Type = "K29", Designation = "Pulse Measurements with Power Sensor", GlobalType = "Pulse Measurements with Power Sensor"},
    //                new DeviceOption(){Type = "K43", Designation = "Receiver Mode", GlobalType = "Receiver Mode"},
    //            },
    //            DefaultInstrOption = new List<DeviceOption>() { },
    //            LoadedInstrOption = new List<DeviceOption>() { },
    //            TraceType = new List<ParamWithId>
    //            {
    //                new ParamWithId {Id = (int)EN.TraceType.ClearWrite, Parameter = "WRIT" },
    //                new ParamWithId {Id = (int)EN.TraceType.Average, Parameter = "AVER" },
    //                new ParamWithId {Id = (int)EN.TraceType.MaxHold, Parameter = "MAXH" },
    //                new ParamWithId {Id = (int)EN.TraceType.MinHold, Parameter = "MINH" },
    //                new ParamWithId {Id = (int)EN.TraceType.View, Parameter = "VIEW" },
    //                new ParamWithId {Id = (int)EN.TraceType.Blank, Parameter = "BLAN" }
    //            },
    //            TraceDetector = new List<ParamWithId>
    //            {
    //                new ParamWithId {Id = (int)EN.TraceDetector.AutoSelect, Parameter = "Auto Select" },
    //                new ParamWithId {Id = (int)EN.TraceDetector.AutoPeak, Parameter = "APE" },
    //                new ParamWithId {Id = (int)EN.TraceDetector.MaxPeak, Parameter = "POS" },
    //                new ParamWithId {Id = (int)EN.TraceDetector.MinPeak, Parameter = "NEG" },
    //                new ParamWithId {Id = (int)EN.TraceDetector.Sample, Parameter = "SAMP" },
    //                new ParamWithId {Id = (int)EN.TraceDetector.RMS, Parameter = "RMS" }
    //            },
    //            LevelUnits = new List<ParamWithId>
    //            {
    //                new ParamWithId {Id = (int)MEN.LevelUnit.dBm, Parameter = "DBM" },
    //                new ParamWithId {Id = (int)MEN.LevelUnit.dBmV, Parameter = "DBMV" },
    //                new ParamWithId {Id = (int)MEN.LevelUnit.dBµV, Parameter = "DBUV" },
    //            },
    //            SweepType = new List<ParamWithId> {},
    //            SweepPointArr = new int[]{ 711 },
    //            DefaultSweepPoint = 711,
    //            RBWArr = new decimal[]{ 1, 3, 10, 30, 100, 300, 1000, 3000, 10000, 30000, 100000, 300000, 1000000, 3000000 },
    //            VBWArr = new decimal[]{ 1, 3, 10, 30, 100, 300, 1000, 3000, 10000, 30000, 100000, 300000, 1000000, 3000000 },
    //            CouplingRatio = false,
    //            AttMax = 40,
    //            AttStep = 5,
    //            RangeArr = new decimal[] {5, 10, 20, 30, 50, 100, 120, 130, 150 },
    //            PreAmp = false,
    //            Battery = true,
    //            NdB = true,
    //            OBW = true,
    //            ChnPow= true,
    //            RangeFixed = true,
    //        },
    //        #endregion FPH
    //        #region FSH
    //        new LocalSpectrumAnalyzerInfo
    //        {
    //            InstrManufacture = 1, NumberOfTrace = 1, ChangeableSweepType = false, SweepPointFix = true, InstrModel = "FSH",
    //            HiSpeed = false,
    //            InstrOption = new List<DeviceOption>()
    //            {
    //                 new DeviceOption(){Type = "B22", Designation = "Preamplifier", GlobalType = "Preamplifier"},
    //                 new DeviceOption(){Type = "K1", Designation = "Spectrum Analysis Application", GlobalType = "Spectrum Analysis Application"},
    //                 new DeviceOption(){Type = "K0", Designation = "DEMO OPTION (All inclusive)", GlobalType = "DEMO OPTION (All inclusive)"},
    //                 new DeviceOption(){Type = "K9", Designation = "Power Meter Measurement Application with R&S®FSH-Zxx or R&S®NRP-Zxx power sensors", GlobalType = "Power Sensor Support"},
    //                 new DeviceOption(){Type = "K14", Designation = "Spectrogram Measurement Application", GlobalType = "Spectrogram Measurement Application"},
    //                 new DeviceOption(){Type = "K19", Designation = "Channel Power Meter", GlobalType = "Channel Power Meter"},
    //                 new DeviceOption(){Type = "K29", Designation = "Pulse Measurements with Power Sensor", GlobalType = "Pulse Measurements with Power Sensor"},
    //                 new DeviceOption(){Type = "K39", Designation = "Transmission Measurement Application", GlobalType = "Transmission Measurement Application"},
    //                 new DeviceOption(){Type = "K40", Designation = "Remote Control via USB or LAN Application", GlobalType = "Remote Control"},
    //                 new DeviceOption(){Type = "K42", Designation = "Vector Network Analysis Application", GlobalType = "Vector Network Analysis Application"},
    //                 new DeviceOption(){Type = "K45", Designation = "Vector Voltmeter Measurement Application", GlobalType = "Vector Voltmeter Measurement Application"},
    //            },
    //            DefaultInstrOption = new List<DeviceOption>()
    //            {
    //                new DeviceOption(){Type = "B22", Designation = "Preamplifier", GlobalType = "Preamplifier"}
    //            },
    //            LoadedInstrOption = new List<DeviceOption>(),
    //            TraceType = new List<ParamWithId>
    //            {
    //                new ParamWithId {Id = (int)EN.TraceType.ClearWrite, Parameter = "WRIT" },
    //                new ParamWithId {Id = (int)EN.TraceType.Average, Parameter = "AVER" },
    //                new ParamWithId {Id = (int)EN.TraceType.MaxHold, Parameter = "MAXH" },
    //                new ParamWithId {Id = (int)EN.TraceType.MinHold, Parameter = "MINH" },
    //                new ParamWithId {Id = (int)EN.TraceType.View, Parameter = "VIEW" },
    //                new ParamWithId {Id = (int)EN.TraceType.Blank, Parameter = "BLAN" }
    //            },
    //            TraceDetector = new List<ParamWithId>
    //            {
    //                new ParamWithId {Id = (int)EN.TraceDetector.AutoSelect, Parameter = "Auto Select" },
    //                new ParamWithId {Id = (int)EN.TraceDetector.AutoPeak, Parameter = "APE" },
    //                new ParamWithId {Id = (int)EN.TraceDetector.MaxPeak, Parameter = "POS" },
    //                new ParamWithId {Id = (int)EN.TraceDetector.MinPeak, Parameter = "NEG" },
    //                new ParamWithId {Id = (int)EN.TraceDetector.Sample, Parameter = "SAMP" },
    //                new ParamWithId {Id = (int)EN.TraceDetector.RMS, Parameter = "RMS" }
    //            },
    //            LevelUnits = new List<ParamWithId>
    //            {
    //                new ParamWithId {Id = (int)MEN.LevelUnit.dBm, Parameter = "DBM" },
    //                new ParamWithId {Id = (int)MEN.LevelUnit.dBmV, Parameter = "DBMV" },
    //                new ParamWithId {Id = (int)MEN.LevelUnit.dBµV, Parameter = "DBUV" },
    //            },
    //            SweepType =  new List<ParamWithId> {},
    //            SweepPointArr = new int[]{ 550 },
    //            DefaultSweepPoint = 550,
    //            RBWArr = new decimal[]{ 1, 3, 10, 30, 100, 300, 1000, 3000, 10000, 30000, 100000, 300000, 1000000, 3000000 },
    //            VBWArr = new decimal[]{ 1, 3, 10, 30, 100, 300, 1000, 3000, 10000, 30000, 100000, 300000, 1000000, 3000000 },
    //            CouplingRatio = false,
    //            AttMax = 40,
    //            AttStep = 5,
    //            PreAmp = false,
    //            Battery = true,
    //            NdB = true,
    //            OBW = false,
    //            ChnPow= false
    //        },
    //        #endregion FSH
    //        #region FPL
    //        new LocalSpectrumAnalyzerInfo
    //        {
    //            InstrManufacture = 1,  NumberOfTrace = 3, ChangeableSweepType = true, SweepPointFix = false, InstrModel = "FPL",
    //            HiSpeed = true,
    //            InstrOption = new List<DeviceOption>()
    //            {
    //                 new DeviceOption(){Type = "B4", Designation = "OCXO Reference Frequency", GlobalType = "OCXO Reference"},
    //                 new DeviceOption(){Type = "B5", Designation = "Additional Interfaces", GlobalType = "Additional Interfaces"},
    //                 new DeviceOption(){Type = "B10", Designation = "GPIB Interface", GlobalType = "GPIB Interface"},
    //                 new DeviceOption(){Type = "B19", Designation = "Second Hard Disk (SSD)", GlobalType = "SSD"},
    //                 new DeviceOption(){Type = "B22", Designation = "RF Preamplifier", GlobalType = "Preamplifier"},
    //                 new DeviceOption(){Type = "B25", Designation = "1 dB Steps for Electronic Attenuator", GlobalType = "Attenuator 1 dB"},
    //                 new DeviceOption(){Type = "B30", Designation = "DC Power Supply 12/24 V", GlobalType = "DC Power Supply"},
    //                 new DeviceOption(){Type = "B31", Designation = "Internal Li-Ion Battery", GlobalType = "Internal Li-Ion Battery"},
    //                 new DeviceOption(){Type = "B40", Designation = "40 MHz Analysis Bandwidth", GlobalType = "40 MHz Analysis Bandwidth"},
    //                 new DeviceOption(){Type = "K7", Designation = "Analog Modulation Analysis (AM/FM/φM)", GlobalType = "Analog Modulation Analysis"},
    //                 new DeviceOption(){Type = "K9", Designation = "Power Sensor Measurement with R&S®NRP Power Sensors", GlobalType =  "Power Sensor Support"},
    //                 new DeviceOption(){Type = "K30", Designation = "Noise Figure Measurement Application", GlobalType =  "Noise Figure Measurement Application"},

    //            },
    //            DefaultInstrOption = new List<DeviceOption>() { },
    //            LoadedInstrOption = new List<DeviceOption>() { },
    //            TraceType = new List<ParamWithId>
    //            {
    //                new ParamWithId {Id = (int)EN.TraceType.ClearWrite, Parameter = "WRIT" },
    //                new ParamWithId {Id = (int)EN.TraceType.Average, Parameter = "AVER" },
    //                new ParamWithId {Id = (int)EN.TraceType.MaxHold, Parameter = "MAXH" },
    //                new ParamWithId {Id = (int)EN.TraceType.MinHold, Parameter = "MINH" },
    //                new ParamWithId {Id = (int)EN.TraceType.View, Parameter = "VIEW" },
    //                new ParamWithId {Id = (int)EN.TraceType.Blank, Parameter = "BLAN" }
    //            },
    //            TraceDetector = new List<ParamWithId>
    //            {
    //                new ParamWithId {Id = (int)EN.TraceDetector.AutoSelect, Parameter = "Auto Select" },
    //                new ParamWithId {Id = (int)EN.TraceDetector.AutoPeak, Parameter = "APE" },
    //                new ParamWithId {Id = (int)EN.TraceDetector.Average, Parameter = "AVER" },
    //                new ParamWithId {Id = (int)EN.TraceDetector.MaxPeak, Parameter = "POS" },
    //                new ParamWithId {Id = (int)EN.TraceDetector.MinPeak, Parameter = "NEG" },
    //                new ParamWithId {Id = (int)EN.TraceDetector.Sample, Parameter = "SAMP" },
    //                new ParamWithId {Id = (int)EN.TraceDetector.RMS, Parameter = "RMS" }
    //            },
    //            SweepType =  new List<ParamWithId>
    //            {
    //                new ParamWithId {Id = (int)EN.SweepType.Auto, Parameter = "AUTO" },
    //                new ParamWithId {Id = (int)EN.SweepType.Sweep, Parameter = "SWE" },
    //                new ParamWithId {Id = (int)EN.SweepType.FFT, Parameter = "FFT" }
    //            },
    //            LevelUnits = new List<ParamWithId>
    //            {
    //                new ParamWithId {Id = (int)MEN.LevelUnit.dBm, Parameter = "DBM" },
    //                new ParamWithId {Id = (int)MEN.LevelUnit.dBmV, Parameter = "DBMV" },
    //                new ParamWithId {Id = (int)MEN.LevelUnit.dBµV, Parameter = "DBUV" },
    //                new ParamWithId {Id = (int)MEN.LevelUnit.dBµVm, Parameter = "DBUV/M" }
    //            },
    //            SweepPointArr = new int[]{ 101, 125, 155, 173, 201, 251, 301, 313, 345, 401, 501, 601, 625, 691, 701, 801, 901, 1001, 1251, 1383, 1999, 2001, 2501, 2765, 3001, 4001, 5001, 6001, 7001, 8001, 9001, 10001, 11001, 12001, 13001, 14001, 15001, 16001, 17001, 18001, 19001, 20001, 21001, 22001, 23001, 24001, 25001, 26001, 27001, 28001, 29001, 30001, 31001, 32001 },
    //            DefaultSweepPoint = 691,
    //            RBWArr = new decimal[]{ 1, 2, 3, 5, 10, 20, 30, 50, 100, 200, 300, 500, 1000, 2000, 3000, 5000, 6250, 10000, 20000, 30000, 50000, 100000, 200000, 300000, 500000, 1000000, 2000000, 3000000, 5000000, 10000000 },
    //            VBWArr = new decimal[]{ 1, 2, 3, 5, 10, 20, 30, 50, 100, 200, 300, 500, 1000, 2000, 3000, 5000, 10000, 20000, 30000, 50000, 100000, 200000, 300000, 500000, 1000000, 2000000, 3000000, 5000000, 10000000 },
    //            SWTMin = 0.000075,
    //            SWTMax = 8000,
    //            RefLevelMax = 20,
    //            RefLevelMin = -200,

    //            CouplingRatio = true,
    //            AttMax = 70,
    //            AttStep = 5,
    //            PreAmp = false,
    //            Battery = false,
    //            NdB = true,
    //            OBW = true,
    //            ChnPow= true,
    //        },
    //        #endregion FSV
    //        #region ZVH
    //        new LocalSpectrumAnalyzerInfo
    //        {
    //            InstrManufacture = 1, NumberOfTrace = 1, ChangeableSweepType = false, SweepPointFix = true, InstrModel = "ZVH",
    //            HiSpeed = false,
    //            InstrOption = new List<DeviceOption>()
    //            {
    //                new DeviceOption(){Type = "K0", Designation = "DEMO OPTION (All inclusive)", GlobalType = "DEMO OPTION (All inclusive)"},
    //                new DeviceOption(){Type = "K1", Designation = "Spectrum Analysis Application", GlobalType = "Spectrum Analysis Application"},
    //                new DeviceOption(){Type = "K9", Designation = "Power Meter Measurement Application with R&S®FSH-Zxx or R&S®NRP-Zxx power sensors", GlobalType = "Power Sensor Support"},
    //                new DeviceOption(){Type = "K14", Designation = "Spectrogram Measurement Application", GlobalType = "Spectrogram Measurement Application"},
    //                new DeviceOption(){Type = "K19", Designation = "Channel Power Meter", GlobalType = "Channel Power Meter"},
    //                new DeviceOption(){Type = "K29", Designation = "Pulse Measurements with Power Sensor", GlobalType = "Pulse Measurements with Power Sensor"},
    //                new DeviceOption(){Type = "K39", Designation = "Transmission Measurement Application", GlobalType = "Transmission Measurement Application"},
    //                new DeviceOption(){Type = "K40", Designation = "Remote Control via USB or LAN Application", GlobalType = "Remote Control"},
    //                new DeviceOption(){Type = "K42", Designation = "Vector Network Analysis Application", GlobalType = "Vector Network Analysis Application"},
    //                new DeviceOption(){Type = "K45", Designation = "Vector Voltmeter Measurement Application", GlobalType = "Vector Voltmeter Measurement Application"},
    //            },
    //            DefaultInstrOption = new List<DeviceOption>()
    //            {
    //                new DeviceOption(){Type = "B22", Designation = "Preamplifier", GlobalType = "Preamplifier"}
    //            },
    //            LoadedInstrOption = new List<DeviceOption>(),
    //            TraceType = new List<ParamWithId>
    //            {
    //                new ParamWithId {Id = (int)EN.TraceType.ClearWrite, Parameter = "WRIT" },
    //                new ParamWithId {Id = (int)EN.TraceType.Average, Parameter = "AVER" },
    //                new ParamWithId {Id = (int)EN.TraceType.MaxHold, Parameter = "MAXH" },
    //                new ParamWithId {Id = (int)EN.TraceType.MinHold, Parameter = "MINH" },
    //                new ParamWithId {Id = (int)EN.TraceType.View, Parameter = "VIEW" },
    //                new ParamWithId {Id = (int)EN.TraceType.Blank, Parameter = "BLAN" }
    //            },
    //            TraceDetector = new List<ParamWithId>
    //            {
    //                new ParamWithId {Id = (int)EN.TraceDetector.AutoSelect, Parameter = "Auto Select" },
    //                new ParamWithId {Id = (int)EN.TraceDetector.AutoPeak, Parameter = "APE" },
    //                new ParamWithId {Id = (int)EN.TraceDetector.MaxPeak, Parameter = "POS" },
    //                new ParamWithId {Id = (int)EN.TraceDetector.MinPeak, Parameter = "NEG" },
    //                new ParamWithId {Id = (int)EN.TraceDetector.Sample, Parameter = "SAMP" },
    //                new ParamWithId {Id = (int)EN.TraceDetector.RMS, Parameter = "RMS" }
    //            },
    //            LevelUnits = new List<ParamWithId>
    //            {
    //                new ParamWithId {Id = (int)MEN.LevelUnit.dBm, Parameter = "DBM" },
    //                new ParamWithId {Id = (int)MEN.LevelUnit.dBmV, Parameter = "DBMV" },
    //                new ParamWithId {Id = (int)MEN.LevelUnit.dBµV, Parameter = "DBUV" },
    //            },
    //            SweepType =  new List<ParamWithId> {},
    //            SweepPointArr = new int[]{ 631 },
    //            DefaultSweepPoint = 631,
    //            RBWArr = new decimal[]{ 1, 3, 10, 30, 100, 300, 1000, 3000, 10000, 30000, 100000, 300000, 1000000, 3000000 },
    //            VBWArr = new decimal[]{ 1, 3, 10, 30, 100, 300, 1000, 3000, 10000, 30000, 100000, 300000, 1000000, 3000000 },
    //            CouplingRatio = false,
    //            AttMax = 40,
    //            AttStep = 5,
    //            PreAmp = false,
    //            Battery = true,
    //            NdB = true,
    //            OBW = true,
    //            ChnPow= false
    //        },
    //        #endregion ZVH
    //        #region N99
    //        new LocalSpectrumAnalyzerInfo
    //        {
    //            InstrManufacture = 2, NumberOfTrace = 1, ChangeableSweepType = true, SweepPointFix = false, InstrModel = "N99",
    //            HiSpeed = false,
    //            InstrOption = new List<DeviceOption>(),
    //            DefaultInstrOption = new List<DeviceOption>(),
    //            LoadedInstrOption = new List<DeviceOption>(),
    //            TraceType = new List<ParamWithId>
    //            {
    //                new ParamWithId {Id = (int)EN.TraceType.ClearWrite, Parameter = "CLRW" },
    //                new ParamWithId {Id = (int)EN.TraceType.Average, Parameter = "AVG" },
    //                new ParamWithId {Id = (int)EN.TraceType.MaxHold, Parameter = "MAXH" },
    //                new ParamWithId {Id = (int)EN.TraceType.MinHold, Parameter = "MINH" },
    //                new ParamWithId {Id = (int)EN.TraceType.View, Parameter = "VIEW" },
    //                new ParamWithId {Id = (int)EN.TraceType.Blank, Parameter = "BLANk" }
    //            },
    //            TraceDetector = new List<ParamWithId>
    //            {
    //                new ParamWithId {Id = (int)EN.TraceDetector.AutoSelect, Parameter = "AUTO" },
    //                new ParamWithId {Id = (int)EN.TraceDetector.Average, Parameter = "AVER" },
    //                new ParamWithId {Id = (int)EN.TraceDetector.MaxPeak, Parameter = "POS" },
    //                new ParamWithId {Id = (int)EN.TraceDetector.MinPeak, Parameter = "NEG" },
    //                new ParamWithId {Id = (int)EN.TraceDetector.Sample, Parameter = "SAMP" },
    //                new ParamWithId {Id = (int)EN.TraceDetector.Normal, Parameter = "  NORMal" }
    //            },
    //            SweepType =  new List<ParamWithId>
    //            {
    //                new ParamWithId {Id = (int)EN.SweepType.Auto, Parameter = "AUTO" },
    //                new ParamWithId {Id = (int)EN.SweepType.Sweep, Parameter = "STEP" },
    //                new ParamWithId {Id = (int)EN.SweepType.FFT, Parameter = "FFT" }
    //            },
    //            LevelUnits = new List<ParamWithId>
    //            {
    //                new ParamWithId {Id = (int)MEN.LevelUnit.dBm, Parameter = "DBM" },
    //                new ParamWithId {Id = (int)MEN.LevelUnit.dBmV, Parameter = "DBMV" },
    //                new ParamWithId {Id = (int)MEN.LevelUnit.dBµV, Parameter = "DBUV" },
    //            },
    //            SweepPointArr = new int[]{ 101, 201, 301, 401, 501, 601, 625, 691, 701, 801, 901, 1001, 2001, 3001, 4001, 5001, 6001, 7001, 8001, 9001, 10001 },
    //            DefaultSweepPoint = 401,
    //            RBWArr = new decimal[]{ 1, 3, 10, 30, 100, 300, 1000, 3000, 10000, 30000, 100000, 300000, 1000000, 3000000, 5000000 },
    //            VBWArr = new decimal[]{ 1, 3, 10, 30, 100, 300, 1000, 3000, 10000, 30000, 100000, 300000, 1000000, 3000000, 5000000 },
    //            CouplingRatio = false,
    //            AttMax = 70,
    //            AttStep = 5,
    //            PreAmp = false,
    //            Battery = true,
    //            NdB = false,
    //            OBW = true,
    //            ChnPow= false
    //        },
    //        #endregion N99
    //        #region MS27
    //        new LocalSpectrumAnalyzerInfo
    //        {
    //            InstrManufacture = 3, NumberOfTrace = 1, ChangeableSweepType = true, SweepPointFix = true, InstrModel = "MS27",
    //            HiSpeed = false,
    //            InstrOption = new List<DeviceOption>(),
    //            DefaultInstrOption = new List<DeviceOption>(),
    //            LoadedInstrOption = new List<DeviceOption>(),
    //            TraceType = new List<ParamWithId>
    //            {
    //                new ParamWithId {Id = (int)EN.TraceType.ClearWrite, Parameter = "NORM" },
    //                new ParamWithId {Id = (int)EN.TraceType.Average, Parameter = "AVER" },
    //                new ParamWithId {Id = (int)EN.TraceType.MaxHold, Parameter = "MAXH" },
    //                new ParamWithId {Id = (int)EN.TraceType.MinHold, Parameter = "MINH" },
    //                new ParamWithId {Id = (int)EN.TraceType.Blank, Parameter = "NONE" }
    //            },
    //            TraceDetector = new List<ParamWithId>
    //            {
    //                new ParamWithId {Id = (int)EN.TraceDetector.AutoSelect, Parameter = "AUTO" },
    //                new ParamWithId {Id = (int)EN.TraceDetector.Average, Parameter = "AVER" },
    //                new ParamWithId {Id = (int)EN.TraceDetector.MaxPeak, Parameter = "POS" },
    //                new ParamWithId {Id = (int)EN.TraceDetector.MinPeak, Parameter = "NEG" },
    //                new ParamWithId {Id = (int)EN.TraceDetector.Sample, Parameter = "SAMP" },
    //                new ParamWithId {Id = (int)EN.TraceDetector.Normal, Parameter = "NORMal" }
    //            },
    //            SweepType =  new List<ParamWithId>
    //            {
    //                new ParamWithId {Id = (int)EN.SweepType.Fast, Parameter = "FAST" },
    //                new ParamWithId {Id = (int)EN.SweepType.Performance, Parameter = "PERF" },
    //                new ParamWithId {Id = (int)EN.SweepType.NoFFT, Parameter = "NOFF" }
    //            },
    //            LevelUnits = new List<ParamWithId>
    //            {
    //                new ParamWithId {Id = (int)MEN.LevelUnit.dBm, Parameter = "dBm" },
    //                new ParamWithId {Id = (int)MEN.LevelUnit.dBmV, Parameter = "dBmV" },
    //                new ParamWithId {Id = (int)MEN.LevelUnit.dBµV, Parameter = "dBuV" },
    //            },
    //            SweepPointArr = new int[] {551},
    //            DefaultSweepPoint = 551,
    //            RBWArr = new decimal[]{ 1, 3, 10, 30, 100, 300, 1000, 3000, 10000, 30000, 100000, 300000, 1000000, 3000000, 5000000 },
    //            VBWArr = new decimal[]{ 1, 3, 10, 30, 100, 300, 1000, 3000, 10000, 30000, 100000, 300000, 1000000, 3000000 },
    //            CouplingRatio = false,
    //            AttMax = 70,
    //            AttStep = 5,
    //            PreAmp = false,
    //            Battery = true,
    //            NdB = true,
    //            OBW = true,
    //            ChnPow= false
    //        }
    //        #endregion MS27
    //    };


    //    #region Freqs
    //    public decimal FreqMin = 9000;
    //    public decimal FreqMax = 3000000000;

    //    public decimal FreqCentr
    //    {
    //        get { return _FreqCentr; }
    //        set
    //        {
    //            _FreqCentr = value;
    //            _FreqStart = _FreqCentr - _FreqSpan / 2;
    //            _FreqStop = _FreqCentr + _FreqSpan / 2;
    //        }
    //    }
    //    private decimal _FreqCentr = 1000000000;

    //    public decimal FreqSpan
    //    {
    //        get { return _FreqSpan; }
    //        set
    //        {
    //            _FreqSpan = value;
    //            _FreqStart = _FreqCentr - _FreqSpan / 2;
    //            _FreqStop = _FreqCentr + _FreqSpan / 2;
    //        }
    //    }
    //    private decimal _FreqSpan = 20000000;

    //    public decimal FreqStart
    //    {
    //        get { return _FreqStart; }
    //        set
    //        {
    //            _FreqStart = value;
    //            _FreqCentr = (_FreqStart + _FreqStop) / 2;
    //            _FreqSpan = _FreqStop - _FreqStart;
    //        }
    //    }
    //    private decimal _FreqStart = 990000000;

    //    public decimal FreqStop
    //    {
    //        get { return _FreqStop; }
    //        set
    //        {
    //            _FreqStop = value;
    //            _FreqCentr = (_FreqStart + _FreqStop) / 2;
    //            _FreqSpan = _FreqStop - _FreqStart;
    //        }
    //    }
    //    private decimal _FreqStop = 1010000000;
    //    #endregion Freqs

    //    #region RBW / VBW
    //    public decimal RBW;

    //    private int RBWIndex
    //    {
    //        get { return _RBWIndex; }
    //        set
    //        {
    //            if (value > UniqueData.RBWArr.Length - 1) _RBWIndex = UniqueData.RBWArr.Length - 1;
    //            else if (value < 0) _RBWIndex = 0;
    //            else _RBWIndex = value;
    //            RBW = UniqueData.RBWArr[_RBWIndex];
    //        }
    //    }
    //    private int _RBWIndex = 0;

    //    public bool AutoRBW;


    //    public decimal VBW;

    //    private int VBWIndex
    //    {
    //        get { return _VBWIndex; }
    //        set
    //        {
    //            if (value > UniqueData.VBWArr.Length - 1) _VBWIndex = UniqueData.VBWArr.Length - 1;
    //            else if (value < 0) _VBWIndex = 0;
    //            else _VBWIndex = value;
    //            VBW = UniqueData.VBWArr[_VBWIndex];
    //        }
    //    }
    //    private int _VBWIndex = 0;

    //    public bool AutoVBW;
    //    #endregion RBW / VBW

    //    #region Sweep
    //    public decimal SweepTime;

    //    public bool AutoSweepTime;

    //    public int TracePoints;

    //    public int SweepPoints;

    //    private int SweepPointsIndex
    //    {
    //        get { return _SweepPointsIndex; }
    //        set
    //        {
    //            if (value > UniqueData.SweepPointArr.Length - 1) SweepPointsIndex = UniqueData.SweepPointArr.Length - 1;
    //            else if (value < 0) SweepPointsIndex = 0;
    //            else _SweepPointsIndex = value;
    //            SweepPoints = UniqueData.SweepPointArr[SweepPointsIndex];
    //        }
    //    }
    //    private int _SweepPointsIndex = 0;


    //    public ParamWithId SweepTypeSelected
    //    {
    //        get { return _SweepTypeSelected; }
    //        set { _SweepTypeSelected = value; }
    //    }
    //    private ParamWithId _SweepTypeSelected = new ParamWithId { Id = 0, Parameter = "" };
    //    #endregion

    //    #region Level
    //    private int PowerRegister
    //    {
    //        get { return _PowerRegister; }
    //        set
    //        {
    //            _PowerRegister = value;
    //            if (_PowerRegister == 0)
    //            {
    //                PowerRegisterStr = "";
    //            }
    //            else
    //            {
    //                if (_PowerRegister == 4) { PowerRegisterStr = "IF Overload"; }
    //                else if (_PowerRegister == 1) { PowerRegisterStr = "RF Overload"; }//правильно
    //                else if (_PowerRegister == 2) { PowerRegisterStr = ""; }
    //            }
    //        }
    //    }
    //    int _PowerRegister = 0;
    //    private string PowerRegisterStr = "";

    //    public decimal RefLevel = -40;

    //    public int RangeIndex
    //    {
    //        get { return _RangeIndex; }
    //        set
    //        {
    //            if (value > UniqueData.RangeArr[UniqueData.RangeArr.Length - 1]) _RangeIndex = UniqueData.RangeArr.Length - 1;
    //            else if (value < 0) _RangeIndex = 0;
    //            else _RangeIndex = value;
    //            Range = UniqueData.RangeArr[_RangeIndex];
    //        }
    //    }
    //    private int _RangeIndex = 5;

    //    public decimal Range = 100;

    //    public decimal LowestLevel = -140;

    //    public decimal AttLevel
    //    {
    //        get { return _AttLevel; }
    //        set
    //        {
    //            if (value > UniqueData.AttMax) _AttLevel = UniqueData.AttMax;
    //            else if (value < 0) _AttLevel = 0;
    //            else _AttLevel = value;
    //        }
    //    }
    //    decimal _AttLevel = 0;

    //    public bool AttAuto;

    //    public bool PreAmp;

    //    public ParamWithId LevelUnits
    //    {
    //        get { return _LevelUnits; }
    //        set { _LevelUnits = value; }
    //    }
    //    private ParamWithId _LevelUnits = new ParamWithId() { Id = 0, Parameter = "" };

    //    public int LevelUnitIndex
    //    {
    //        get { return _LevelUnitIndex; }
    //        set
    //        {
    //            if (value > UniqueData.LevelUnits.Count - 1) _LevelUnitIndex = UniqueData.LevelUnits.Count - 1;
    //            else if (value < 0) _LevelUnitIndex = 0;
    //            else _LevelUnitIndex = value;
    //        }
    //    }
    //    private int _LevelUnitIndex = 0;
    //    #endregion Level

    //    #region Trace Data

    //    private ulong TraceCountToMeas = 1;
    //    private ulong TraceCount = 1;

    //    public double[] FreqArr;
    //    public float[] LevelArr;

    //    public ParamWithId Trace1Type
    //    {
    //        get { return _Trace1Type; }
    //        set { _Trace1Type = value; }
    //    }
    //    private ParamWithId _Trace1Type = new ParamWithId { Id = 0, Parameter = "BLAN" };

    //    public ParamWithId Trace1Detector
    //    {
    //        get { return _Trace1Detector; }
    //        set { _Trace1Detector = value; }
    //    }
    //    private ParamWithId _Trace1Detector = new ParamWithId { Id = 0, Parameter = "AutoSelect" };

    //    public int AveragingCount
    //    {
    //        get { return _AveragingCount; }
    //        set
    //        {
    //            if (UniqueData.HiSpeed == true)
    //            {
    //                if (value > 30000) _AveragingCount = 30000;
    //                else if (value < 1) _AveragingCount = 1;
    //                else _AveragingCount = value;
    //            }
    //            else if (UniqueData.HiSpeed == false)
    //            {
    //                if (value > 999) _AveragingCount = 999;
    //                else if (value < 1) _AveragingCount = 1;
    //                else _AveragingCount = value;
    //            }
    //        }
    //    }
    //    private int _AveragingCount = 10;

    //    public int NumberOfSweeps = 0;
    //    #endregion

    //    #region Battery

    //    public decimal BatteryCharge = 0;

    //    public bool BatteryCharging = false;
    //    #endregion Battery


    //    #region runs
    //    private bool IsRuning;
    //    private long LastUpdate;
    //    #endregion


    //    #endregion Param
    //    #region Private Method


    //    #region AN To Command
    //    /// <summary>
    //    /// Установка Начальной частоты просмотра 
    //    /// </summary>
    //    private bool SetFreqStart(decimal freqStart)
    //    {
    //        bool res = false;
    //        try
    //        {
    //            FreqStart = freqStart;
    //            if (UniqueData.InstrManufacture == 1)
    //            {
    //                session.Write(":SENSe:FREQ:STAR " + FreqStart.ToString().Replace(',', '.'));
    //                FreqStart = decimal.Parse(session.Query(":SENSe:FREQ:STAR?").Replace('.', ','));
    //            }
    //            else if (UniqueData.InstrManufacture == 2)
    //            {
    //                session.Write("FREQ:STAR " + FreqStart.ToString().Replace(',', '.'));
    //                FreqStart = decimal.Parse(session.Query(":FREQ:STAR?").Replace('.', ','));
    //            }
    //            else if (UniqueData.InstrManufacture == 3)
    //            {
    //                session.Write("FREQ:STAR " + FreqStart.ToString().Replace(',', '.'));
    //                FreqCentr = decimal.Parse(session.Query(":SENSe:FREQuency:CENTer?"));
    //            }
    //            res = true;
    //        }
    //        #region Exception
    //        catch (VisaException v_exp)
    //        {
    //            _logger.Exception(Contexts.ThisComponent, v_exp);
    //        }
    //        catch (Exception exp)
    //        {
    //            _logger.Exception(Contexts.ThisComponent, exp);
    //        }
    //        #endregion
    //        GetFreqArr();
    //        if (AutoRBW == true) { GetRBW(); }
    //        if (AutoVBW == true) { GetVBW(); }
    //        if (AutoSweepTime == true) { GetSweepTime(); }

    //        return res;
    //    }
    //    /// <summary>
    //    /// Установка Конечной частоты просмотра 
    //    /// </summary>
    //    private bool SetFreqStop(decimal freqStop)
    //    {
    //        bool res = false;
    //        try
    //        {
    //            FreqStop = freqStop;
    //            if (UniqueData.InstrManufacture == 1)
    //            {
    //                session.Write(":SENSe:FREQ:STOP " + FreqStop.ToString().Replace(',', '.'));
    //                FreqStop = decimal.Parse(session.Query(":SENSe:FREQ:STOP?").Replace('.', ','));
    //            }
    //            else if (UniqueData.InstrManufacture == 2)
    //            {
    //                session.Write(":FREQ:STOP " + FreqStop.ToString().Replace(',', '.'));
    //                FreqStop = decimal.Parse(session.Query(":FREQ:STOP?").Replace('.', ','));
    //            }
    //            else if (UniqueData.InstrManufacture == 3)
    //            {
    //                session.Write(":SENSe:FREQ:STOP " + FreqStop.ToString().Replace(',', '.'));
    //                FreqCentr = decimal.Parse(session.Query(":SENSe:FREQuency:CENTer?"));
    //            }
    //            res = true;
    //        }
    //        #region Exception
    //        catch (VisaException v_exp)
    //        {
    //            _logger.Exception(Contexts.ThisComponent, v_exp);
    //        }
    //        catch (Exception exp)
    //        {
    //            _logger.Exception(Contexts.ThisComponent, exp);
    //        }
    //        #endregion
    //        GetFreqArr();
    //        if (AutoRBW == true) { GetRBW(); }
    //        if (AutoVBW == true) { GetVBW(); }
    //        if (AutoSweepTime == true) { GetSweepTime(); }

    //        return res;
    //    }

    //    /// <summary>
    //    /// Вкл/Выкл атоматического аттенюатора зависящего от опорного уровня, при выкл
    //    /// АвтоАТТ изменяем настройку аттенюатора
    //    /// </summary>
    //    private bool SetAttLevel(decimal attLevel)
    //    {
    //        bool res = false;
    //        try
    //        {
    //            AttLevel = attLevel;
    //            if (AttAuto)
    //            {
    //                AttAuto = false;
    //            }
    //            if (UniqueData.InstrManufacture == 1)
    //            {
    //                session.Write(":INP:ATT " + AttLevel.ToString().Replace(',', '.')); //INP:ATT:AUTO
    //            }
    //            else if (UniqueData.InstrManufacture == 2)
    //            {
    //                session.Write(":POW:ATT " + AttLevel.ToString().Replace(',', '.'));
    //            }
    //            res = true;
    //        }
    //        #region Exception
    //        catch (VisaException v_exp)
    //        {
    //            _logger.Exception(Contexts.ThisComponent, v_exp);
    //        }
    //        catch (Exception exp)
    //        {
    //            _logger.Exception(Contexts.ThisComponent, exp);
    //        }
    //        #endregion

    //        return res;
    //    }

    //    /// <summary>
    //    /// Вкл/Выкл предусилителя 
    //    /// </summary>
    //    private bool SetPreAmp(bool preAmp)
    //    {
    //        bool res = false;
    //        try
    //        {
    //            PreAmp = preAmp;
    //            if (UniqueData.InstrManufacture == 1 && UniqueData.PreAmp == true)
    //            {
    //                if (PreAmp == true)
    //                {
    //                    session.Write(":INP:GAIN:STAT 1");
    //                }
    //                if (PreAmp == false)
    //                {
    //                    session.Write(":INP:GAIN:STAT 0");
    //                }
    //            }
    //            else if (UniqueData.InstrManufacture == 2)
    //            {
    //                if (PreAmp == true)
    //                {
    //                    session.Write(":POW:GAIN 1");
    //                }
    //                else
    //                {
    //                    session.Write(":POW:GAIN 0");
    //                }
    //            }
    //            else if (UniqueData.InstrManufacture == 3)
    //            {
    //                if (PreAmp == true)
    //                {
    //                    session.Write(":POW:GAIN 1");
    //                }
    //                else
    //                {
    //                    session.Write(":POW:GAIN 0");
    //                }
    //            }
    //            res = true;
    //        }
    //        #region Exception
    //        catch (VisaException v_exp)
    //        {
    //            _logger.Exception(Contexts.ThisComponent, v_exp);
    //        }
    //        catch (Exception exp)
    //        {
    //            _logger.Exception(Contexts.ThisComponent, exp);
    //        }
    //        #endregion
    //        return res;
    //    }
    //    /// <summary>
    //    /// Установка опорного уровня 
    //    /// </summary>
    //    private bool SetRefLevel(decimal refLevel)
    //    {
    //        bool res = false;
    //        try
    //        {
    //            RefLevel = refLevel;
    //            if (UniqueData.InstrManufacture == 1)
    //            {
    //                if (UniqueData.HiSpeed)
    //                {
    //                    session.Write(":DISPlay:WINDow:TRACe:Y:SCALe:RLEVel " + RefLevel.ToString().Replace(',', '.'));
    //                }
    //                else
    //                {
    //                    session.Write(":DISP:TRAC:Y:RLEV " + RefLevel.ToString().Replace(',', '.'));
    //                }
    //            }
    //            else if (UniqueData.InstrManufacture == 2)
    //            {
    //                session.Write("DISP:WIND:TRAC:Y:SCAL:RLEV " + RefLevel.ToString().Replace(',', '.'));
    //            }
    //            else if (UniqueData.InstrManufacture == 3)
    //            {
    //                session.Write(":DISPlay:WINDow:TRACe:Y:SCALe:RLEVel " + RefLevel.ToString());
    //            }
    //            res = true;
    //        }
    //        #region Exception
    //        catch (VisaException v_exp)
    //        {
    //            _logger.Exception(Contexts.ThisComponent, v_exp);
    //        }
    //        catch (Exception exp)
    //        {
    //            _logger.Exception(Contexts.ThisComponent, exp);
    //        }
    //        #endregion
    //        if (AttAuto == true) { GetAttLevel(); }
    //        return res;
    //    }
    //    /// <summary>
    //    /// Установка RBW 
    //    /// </summary>
    //    private bool SetRBW(decimal rbw)
    //    {
    //        bool res = false;
    //        try
    //        {
    //            RBW = rbw;
    //            if (UniqueData.InstrManufacture == 1)
    //            {
    //                session.Write(":SENSe:BWIDth:RESolution " + RBW.ToString().Replace(',', '.'));
    //            }
    //            else if (UniqueData.InstrManufacture == 2)
    //            {
    //                session.Write(":SENS:BAND:RES " + RBW.ToString().Replace(',', '.'));
    //            }
    //            else if (UniqueData.InstrManufacture == 3)
    //            {
    //                session.Write(":SENSe:BWIDth:RESolution " + RBW.ToString().Replace(',', '.'));
    //            }
    //            res = true;
    //        }
    //        #region Exception
    //        catch (VisaException v_exp)
    //        {
    //            _logger.Exception(Contexts.ThisComponent, v_exp);
    //        }
    //        catch (Exception exp)
    //        {
    //            _logger.Exception(Contexts.ThisComponent, exp);
    //        }
    //        #endregion
    //        AutoRBW = false;
    //        if (AutoSweepTime == true) { GetSweepTime(); }
    //        if (AutoVBW == true) { GetVBW(); }
    //        GetSweepType();
    //        return res;
    //    }

    //    /// <summary>
    //    /// Установка VBW 
    //    /// </summary>
    //    private bool SetVBW(decimal vbw)
    //    {
    //        bool res = false;
    //        try
    //        {
    //            VBW = vbw;
    //            VBWIndex = System.Array.IndexOf(UniqueData.VBWArr, VBW);
    //            if (UniqueData.InstrManufacture == 1)
    //            {
    //                session.Write(":SENSe:BANDwidth:VIDeo " + UniqueData.VBWArr[VBWIndex].ToString().Replace(',', '.'));
    //            }
    //            else if (UniqueData.InstrManufacture == 2)
    //            {
    //                session.Write(":SENSe:BANDwidth:VIDeo " + UniqueData.VBWArr[VBWIndex].ToString().Replace(',', '.'));
    //            }
    //            else if (UniqueData.InstrManufacture == 3)
    //            {
    //                session.Write(":SENSe:BANDwidth:VIDeo " + UniqueData.VBWArr[VBWIndex].ToString());
    //            }
    //            res = true;
    //        }
    //        #region Exception
    //        catch (VisaException v_exp)
    //        {
    //            _logger.Exception(Contexts.ThisComponent, v_exp);
    //        }
    //        catch (Exception exp)
    //        {
    //            _logger.Exception(Contexts.ThisComponent, exp);
    //        }
    //        #endregion
    //        AutoVBW = false;
    //        if (AutoSweepTime == true && RBWIndex - VBWIndex > 0) { GetSweepTime(); }
    //        GetSweepType();
    //        return res;
    //    }
    //    #endregion

    //    #region AN Method
    //    private void GetTraceData()
    //    {
    //        #region Tr1
    //        if (Trace1Type.UI != "Blank")
    //        {
    //            Trace1New = false;
    //            if (UniqueData.InstrManufacture == 1)
    //            {
    //                if (UniqueData.HiSpeed == true)
    //                {
    //                    string r = session.Query("TRAC:DATA? TRACE1");//TRAC:DATA? TRACE1
    //                                                                  //MessageBox.Show(r.Split(',').Length.ToString());
    //                    string[] responseString = r.Split(',');
    //                    for (int i = 0; i < responseString.Length; i++)
    //                    {
    //                        double lev = double.Parse(responseString[i], System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture); //decimal.Parse(responseString[i].Replace('.', ','));
    //                        if (Trace1New == false && Trace1[i].level != lev) Trace1New = true;
    //                        Trace1[i].level = lev;
    //                    }
    //                    //string r = session.Query("TRAC:DATA? TRACE1");//TRAC:DATA? TRACE1
    //                    //                                              //MessageBox.Show(r.Split(',').Length.ToString());
    //                    //string[] responseString = r.Split(',');
    //                    //Trace1[0].level = double.Parse(responseString[0].Replace('.', ','));
    //                    //Trace1[1] = Trace1[0];
    //                    //Trace1[responseString.Length + 1].level = double.Parse(responseString[responseString.Length - 1].Replace('.', ','));
    //                    //for (int i = 1; i < responseString.Length; i++)
    //                    //{
    //                    //    double lev = double.Parse(responseString[i].Replace('.', ','));
    //                    //    if (Trace1New == false && Trace1[i].level != lev) Trace1New = true;
    //                    //    Trace1[i + 1].level = lev;
    //                    //}
    //                }
    //                else if (UniqueData.HiSpeed == false)
    //                {
    //                    session.Write("TRAC:DATA? TRACE1");
    //                    var byteArray = session.ReadByteArray();
    //                    int start = 6;
    //                    if ((byteArray.Length - start) / 4 == TracePoints)
    //                    {
    //                        double[] t = new double[((byteArray.Length - start) / 4)];
    //                        for (int i = 0; i < (byteArray.Length - start) / 4; i++)
    //                        {
    //                            Single tlev = (BitConverter.ToSingle(byteArray, i * 4 + start));
    //                            if (tlev != Single.MaxValue)
    //                            {
    //                                double lev = tlev;
    //                                if (Trace1New == false && Trace1[i].level != lev) Trace1New = true;
    //                                Trace1[i].level = lev;
    //                            }
    //                        }
    //                    }
    //                }
    //            }
    //            else if (UniqueData.InstrManufacture == 2)
    //            {

    //                //SWT = Double.Parse(session.Query(":SWE:TIME:ACT?").Replace('.', ','));
    //                session.Write("TRAC1:DATA?");
    //                string t = String.Concat("#4", (SweepPoints * 4));
    //                MessageBasedSessionReader reader = new MessageBasedSessionReader(session);
    //                reader.BinaryEncoding = BinaryEncoding.DefiniteLengthBlockData;
    //                reader.BinaryEncoding = BinaryEncoding.RawLittleEndian;
    //                var byteArray = session.ReadByteArray(t.Length);
    //                float[] l = reader.ReadSingles(SweepPoints * 4);
    //                for (int i = 0; i < l.Length; i++)
    //                {
    //                    double lev = l[i];
    //                    if (Trace1New == false && Trace1[i].level != lev) Trace1New = true;
    //                    Trace1[i].level = lev;
    //                }
    //            }
    //            else if (UniqueData.InstrManufacture == 3)
    //            {
    //                SweepTime = decimal.Parse(session.Query(":SWE:TIME:ACT?").Replace('.', ','));
    //                Thread.Sleep((int)(SweepTime * 1200));
    //                //if (SWT < 0.220) Thread.Sleep(350);
    //                //else Thread.Sleep((int)(SWT * 1000) + 100);
    //                session.Write("TRAC:DATA? 1"); //TRAC1:DATA?
    //                MessageBasedSessionReader reader = new MessageBasedSessionReader(session);
    //                reader.BinaryEncoding = BinaryEncoding.DefiniteLengthBlockData;
    //                reader.BinaryEncoding = BinaryEncoding.RawLittleEndian;
    //                var byteArray = session.ReadByteArray(6);
    //                float[] l = reader.ReadSingles(2204);
    //                for (int i = 0; i < l.Length; i++)
    //                {
    //                    double lev = l[i];
    //                    if (Trace1New == false && Trace1[i].level != lev) Trace1New = true;
    //                    Trace1[i].level = lev;
    //                }
    //            }
    //        }
    //        #endregion
    //    }


    //    public bool SetConnect()
    //    {
    //        bool res = false;
    //        try
    //        {
    //            session = (TcpipSession)ResourceManager.GetLocalManager().Open(String.Concat("TCPIP0::", _adapterConfig.IPAddress, "::inst0::INSTR"));

    //            string[] temp = session.Query("*IDN?").Trim('"').Split(',');
    //            int InstrManufacrure = 0;
    //            string InstrModel = "", SerialNumber = "";
    //            if (temp[0].Contains("Rohde&Schwarz")) InstrManufacrure = 1;
    //            else if (temp[0].Contains("Keysight")) InstrManufacrure = 2;
    //            else if (temp[0].Contains("Anritsu")) InstrManufacrure = 3;
    //            InstrModel = temp[1];
    //            SerialNumber = temp[2];
    //            bool st = false;
    //            for (int i = 0; i < AllUniqueData.Count; i++)
    //            {
    //                #region
    //                if (AllUniqueData[i].InstrManufacture == InstrManufacrure)
    //                {
    //                    if (InstrModel.Contains(AllUniqueData[i].InstrModel))
    //                    {
    //                        UniqueData = AllUniqueData[i];
    //                        List<DeviceOption> Loaded = new List<DeviceOption>() { };
    //                        UniqueData.LoadedInstrOption = new List<DeviceOption>();
    //                        foreach (DeviceOption dop in UniqueData.DefaultInstrOption)
    //                        {
    //                            Loaded.Add(dop);
    //                        }
    //                        string[] op = session.Query("*OPT?").TrimEnd().ToUpper().Split(',');
    //                        if (op.Length > 0 && op[0] != "0")
    //                        {
    //                            bool findDemoOption = false;
    //                            foreach (string s in op)
    //                            {
    //                                if (s.ToUpper() == "K0")
    //                                {
    //                                    findDemoOption = true;
    //                                    Loaded = UniqueData.InstrOption;
    //                                }
    //                            }
    //                            if (findDemoOption == false)
    //                            {
    //                                foreach (string s in op)
    //                                {
    //                                    foreach (DeviceOption so in UniqueData.InstrOption)
    //                                    {
    //                                        if (so.Type == s)
    //                                        {
    //                                            Loaded.Add(so);
    //                                        }
    //                                    }

    //                                }
    //                            }
    //                        }
    //                        UniqueData.LoadedInstrOption = Loaded;
    //                    }
    //                }
    //                #endregion
    //            }
    //            if (UniqueData.InstrManufacture == 1)
    //            {
    //                #region
    //                if (UniqueData.HiSpeed == true)
    //                {
    //                    session.Write(":FORM:DATA ASC");//передавать трейс в ASCII
    //                }
    //                else if (UniqueData.HiSpeed == false)
    //                {
    //                    session.Write("FORM:DATA REAL,32"); session.Write("INST SAN");
    //                }
    //                if (_adapterConfig.DisplayUpdate)
    //                {
    //                    session.Write(":SYST:DISP:UPD ON");
    //                }
    //                else
    //                {
    //                    session.Write(":SYST:DISP:UPD OFF");
    //                }
    //                if (UniqueData.HiSpeed == true)
    //                {
    //                    SweepPoints = int.Parse(session.Query(":SWE:POIN?").Replace('.', ','));
    //                }
    //                else if (UniqueData.HiSpeed == false)
    //                {
    //                    SweepPoints = UniqueData.DefaultSweepPoint;
    //                    TracePoints = SweepPoints;
    //                }
    //                session.DefaultBufferSize = SweepPoints * 18 + 25; //увеличиваем буфер чтобы влезло 32001 точка трейса
    //                FreqMin = decimal.Parse(session.Query(":SENSe:FREQuency:STAR? MIN").Replace('.', ','));
    //                FreqMax = decimal.Parse(session.Query(":SENSe:FREQuency:STOP? MAX").Replace('.', ','));

    //                UniqueData.PreAmp = false;
    //                if (UniqueData.LoadedInstrOption != null && UniqueData.LoadedInstrOption.Count > 0)
    //                {
    //                    for (int i = 0; i < UniqueData.LoadedInstrOption.Count; i++)
    //                    {
    //                        if (UniqueData.LoadedInstrOption[i].GlobalType == "Preamplifier") { UniqueData.PreAmp = true; }
    //                        if (UniqueData.LoadedInstrOption[i].Type == "B25") { UniqueData.AttStep = 1; }
    //                    }
    //                }
    //                if (!UniqueData.SweepPointFix)
    //                {
    //                    SweepPointsIndex = System.Array.IndexOf(UniqueData.SweepPointArr, SweepPoints);
    //                }
    //                #endregion
    //            }
    //            else if (UniqueData.InstrManufacture == 2)
    //            {
    //                #region
    //                session.Write("FORM:DATA REAL,32"); //передавать трейс побайтово //в ASCII
    //                SweepPoints = int.Parse(session.Query(":SENS:SWE:POIN?").Replace('.', ','));
    //                SweepPointsIndex = System.Array.IndexOf(UniqueData.SweepPointArr, SweepPoints);

    //                TracePoints = SweepPoints;
    //                session.DefaultBufferSize = SweepPoints * 4 + 20;
    //                if (_adapterConfig.DisplayUpdate)
    //                {
    //                    session.Write(":DISP:ENAB 1");
    //                }
    //                else
    //                {
    //                    session.Write(":DISP:ENAB 0");
    //                }

    //                #endregion
    //            }
    //            else if (UniqueData.InstrManufacture == 3)
    //            {
    //                #region
    //                session.Write("FORM:DATA REAL,32"); //передавать трейс побайтово
    //                SweepPoints = 551;
    //                TracePoints = 551;
    //                SweepPointsIndex = 0;
    //                session.DefaultBufferSize = 2204 + 20;
    //                session.Write(":MMEMory:MSIS INT");
    //                if (_adapterConfig.DisplayUpdate)
    //                {
    //                    session.Write(":DISP:ENAB 1");
    //                }
    //                else
    //                {
    //                    session.Write(":DISP:ENAB 0");
    //                }
    //                SweepPointsIndex = System.Array.IndexOf(UniqueData.SweepPointArr, SweepPoints);
    //                #endregion
    //            }
    //            #region
    //            GetLevelUnit();
    //            GetFreqCentr();
    //            GetFreqSpan();
    //            GetRBW();
    //            GetAutoRBW();
    //            GetVBW();
    //            GetAutoVBW();


    //            GetSweepTime();
    //            GetAutoSweepTime();
    //            GetSweepType();
    //            GetSweepPoints();
    //            GetRefLevel();
    //            GetRange();
    //            GetAttLevel();
    //            GetAutoAttLevel();
    //            GetPreAmp();
    //            GetTraceType();
    //            GetDetectorType();
    //            GetAverageCount();
    //            GetNumberOfSweeps();

    //            GetTransducer();
    //            GetSelectedTransducer();
    //            GetSetAnSysDateTime();
    //            res = true;
    //            //if (Sett.Screen_Settings.SaveScreenFromInstr)
    //            //{ dm += SetImageFormat; /*dm += SetNetworkDrive;*/ }                
    //            #endregion
    //        }
    //        #region Exception
    //        catch (VisaException v_exp)
    //        {
    //            _logger.Exception(Contexts.ThisComponent, v_exp);
    //        }
    //        catch (Exception exp)
    //        {
    //            _logger.Exception(Contexts.ThisComponent, exp);
    //        }
    //        #endregion   
    //        return res;
    //    }


    //    #region Freqs
    //    private void GetFreqArr()
    //    {
    //        try
    //        {
    //            if (UniqueData.InstrManufacture == 1)
    //            {
    //                if (UniqueData.HiSpeed == true)
    //                {
    //                    #region
    //                    //FreqArr = new double[SweepPoints];
    //                    //LevelArr = new float[SweepPoints];
    //                    //decimal step = (decimal)FreqSpan / (SweepPoints - 1);
    //                    //for (int i = 0; i < SweepPoints; i++)
    //                    //{
    //                    //    FreqArr[i] = (double)(FreqStart + step * i);
    //                    //    LevelArr[i] = -1000;
    //                    //}
    //                    #endregion
    //                    #region
    //                    decimal step = 0;
    //                    decimal[] FreqTemp = new decimal[SweepPoints + 2];
    //                    decimal[] Freq = new decimal[SweepPoints + 2];
    //                    step = (decimal)Math.Round((FreqSpan / (FreqTemp.Length - 2)), 2);
    //                    FreqTemp[0] = (decimal)(FreqCentr - FreqSpan / 2);
    //                    FreqTemp[1] = Math.Round(FreqTemp[0] + step / 2, 5);
    //                    FreqTemp[FreqTemp.Length - 1] = (decimal)(FreqCentr + FreqSpan / 2);
    //                    for (int i = 2; i < FreqTemp.Length; i++)
    //                    {
    //                        if (i > 1) { FreqTemp[i] = Math.Round(FreqTemp[i - 1] + step, 5); }
    //                        Freq[i] = Math.Round(FreqTemp[i] / 10, 0) * 10;
    //                    }
    //                    for (int i = 0; i < Freq.Length; i++)
    //                    {
    //                        FreqArr[i] = (double)Freq[i];
    //                        LevelArr[i] = -1000;
    //                    }
    //                    #endregion
    //                }
    //                else if (UniqueData.HiSpeed == false)
    //                {
    //                    #region
    //                    FreqArr = new double[TracePoints];
    //                    LevelArr = new float[TracePoints];
    //                    decimal step = (decimal)FreqSpan / (TracePoints - 1);
    //                    for (int i = 0; i < TracePoints; i++)
    //                    {
    //                        FreqArr[i] = (double)(FreqStart + step * i);
    //                        LevelArr[i] = -1000;
    //                    }
    //                    #endregion
    //                }
    //            }
    //            else if (UniqueData.InstrManufacture == 2)
    //            {
    //                #region
    //                FreqArr = new double[TracePoints];
    //                LevelArr = new float[TracePoints];
    //                decimal step = (decimal)FreqSpan / (TracePoints - 1);
    //                for (int i = 0; i < TracePoints; i++)
    //                {
    //                    FreqArr[i] = (double)(FreqStart + step * i);
    //                    LevelArr[i] = -1000;
    //                }

    //                #endregion
    //            }
    //            else if (UniqueData.InstrManufacture == 3)
    //            {
    //                #region
    //                FreqArr = new double[TracePoints];
    //                LevelArr = new float[TracePoints];
    //                decimal step = (decimal)FreqSpan / (TracePoints - 1);
    //                for (int i = 0; i < TracePoints; i++)
    //                {
    //                    FreqArr[i] = (double)(FreqStart + step * i);
    //                    LevelArr[i] = -1000;
    //                }

    //                #endregion
    //            }
    //        }
    //        #region Exception
    //        catch (Exception exp)
    //        {
    //            _logger.Exception(Contexts.ThisComponent, exp);
    //        }
    //        #endregion
    //    }
    //    /// <summary>
    //    /// Установка центральной частоты 
    //    /// </summary>
    //    private void SetFreqCentr()
    //    {
    //        try
    //        {
    //            if (UniqueData.InstrManufacture == 1)
    //            {
    //                session.Write(":SENSe:FREQuency:CENTer " + FreqCentr.ToString().Replace(',', '.'));
    //                FreqCentr = decimal.Parse(session.Query(":SENSe:FREQuency:CENTer?").Replace('.', ','));
    //            }
    //            else if (UniqueData.InstrManufacture == 2)
    //            {
    //                session.Write(":FREQ:CENT " + FreqCentr.ToString().Replace(',', '.'));
    //                FreqCentr = decimal.Parse(session.Query(":FREQ:CENT?").Replace('.', ','));
    //            }
    //            else if (UniqueData.InstrManufacture == 3)
    //            {
    //                session.Write(":SENSe:FREQuency:CENTer " + FreqCentr.ToString().Replace(',', '.'));
    //                FreqCentr = decimal.Parse(session.Query(":SENSe:FREQuency:CENTer?").Replace('.', ','));
    //            }
    //        }
    //        #region Exception
    //        catch (VisaException v_exp)
    //        {
    //            _logger.Exception(Contexts.ThisComponent, v_exp);
    //        }
    //        catch (Exception exp)
    //        {
    //            _logger.Exception(Contexts.ThisComponent, exp);
    //        }
    //        #endregion
    //        GetFreqArr();
    //        if (AutoRBW == true) { GetRBW(); }
    //        if (AutoVBW == true) { GetVBW(); }
    //        if (AutoSweepTime == true) { GetSweepTime(); }
    //    }
    //    /// <summary>
    //    /// Получаем центральную частоту 
    //    /// </summary>
    //    private void GetFreqCentr()
    //    {
    //        try
    //        {
    //            if (UniqueData.InstrManufacture == 1)
    //            {
    //                FreqCentr = decimal.Parse(session.Query(":SENSe:FREQuency:CENTer?").Replace('.', ','));
    //            }
    //            else if (UniqueData.InstrManufacture == 2)
    //            {
    //                FreqCentr = decimal.Parse(session.Query(":FREQ:CENT?").Replace('.', ','));
    //            }
    //            else if (UniqueData.InstrManufacture == 3)
    //            {
    //                FreqCentr = decimal.Parse(session.Query(":SENSe:FREQuency:CENTer?"));
    //            }
    //        }
    //        #region Exception
    //        catch (VisaException v_exp)
    //        {
    //            _logger.Exception(Contexts.ThisComponent, v_exp);
    //        }
    //        catch (Exception exp)
    //        {
    //            _logger.Exception(Contexts.ThisComponent, exp);
    //        }
    //        #endregion
    //        GetFreqArr();
    //    }
    //    /// <summary>
    //    /// Установка span
    //    /// </summary>
    //    private void SetFreqSpan()
    //    {
    //        try
    //        {
    //            if (UniqueData.InstrManufacture == 1)
    //            {
    //                session.Write(":SENSe:FREQuency:SPAN " + FreqSpan.ToString().Replace(',', '.'));
    //                FreqSpan = decimal.Parse(session.Query(":SENSe:FREQuency:SPAN?").Replace('.', ','));
    //            }
    //            else if (UniqueData.InstrManufacture == 2)
    //            {
    //                session.Write(":FREQuency:SPAN " + FreqSpan.ToString().Replace(',', '.'));
    //                FreqSpan = decimal.Parse(session.Query(":FREQuency:SPAN?").Replace('.', ','));
    //            }
    //            else if (UniqueData.InstrManufacture == 3)
    //            {
    //                session.Write(":SENSe:FREQuency:SPAN " + FreqSpan.ToString().Replace(',', '.'));
    //                FreqSpan = decimal.Parse(session.Query(":SENSe:FREQuency:SPAN?").Replace('.', ','));
    //            }
    //        }
    //        #region Exception
    //        catch (VisaException v_exp)
    //        {
    //            _logger.Exception(Contexts.ThisComponent, v_exp);
    //        }
    //        catch (Exception exp)
    //        {
    //            _logger.Exception(Contexts.ThisComponent, exp);
    //        }
    //        #endregion
    //        GetFreqArr();
    //        if (AutoRBW == true) { GetRBW(); }
    //        if (AutoVBW == true) { GetVBW(); }
    //        if (AutoSweepTime == true) { GetSweepTime(); }
    //    }
    //    /// <summary>
    //    /// получаем span
    //    /// </summary>
    //    private void GetFreqSpan()
    //    {
    //        try
    //        {
    //            if (UniqueData.InstrManufacture == 1)
    //            {
    //                FreqSpan = decimal.Parse(session.Query(":SENSe:FREQuency:SPAN?").Replace('.', ','));
    //            }
    //            else if (UniqueData.InstrManufacture == 2)
    //            {
    //                FreqSpan = decimal.Parse(session.Query(":FREQuency:SPAN?").Replace('.', ','));
    //            }
    //            else if (UniqueData.InstrManufacture == 3)
    //            {
    //                FreqSpan = decimal.Parse(session.Query(":SENSe:FREQuency:SPAN?"));
    //            }
    //        }
    //        #region Exception
    //        catch (VisaException v_exp)
    //        {
    //            _logger.Exception(Contexts.ThisComponent, v_exp);
    //        }
    //        catch (Exception exp)
    //        {
    //            _logger.Exception(Contexts.ThisComponent, exp);
    //        }
    //        #endregion
    //        GetFreqArr();
    //    }
    //    /// <summary>
    //    /// Установка Начальной частоты просмотра 
    //    /// </summary>
    //    private void SetFreqStart()
    //    {
    //        try
    //        {
    //            if (UniqueData.InstrManufacture == 1)
    //            {
    //                session.Write(":SENSe:FREQ:STAR " + FreqStart.ToString().Replace(',', '.'));
    //                FreqStart = decimal.Parse(session.Query(":SENSe:FREQ:STAR?").Replace('.', ','));
    //            }
    //            else if (UniqueData.InstrManufacture == 2)
    //            {
    //                session.Write("FREQ:STAR " + FreqStart.ToString().Replace(',', '.'));
    //                FreqStart = decimal.Parse(session.Query(":FREQ:STAR?").Replace('.', ','));
    //            }
    //            else if (UniqueData.InstrManufacture == 3)
    //            {
    //                FreqCentr = decimal.Parse(session.Query(":SENSe:FREQuency:CENTer?"));
    //                FreqSpan = decimal.Parse(session.Query(":SENSe:FREQuency:SPAN?"));
    //            }
    //        }
    //        #region Exception
    //        catch (VisaException v_exp)
    //        {
    //            _logger.Exception(Contexts.ThisComponent, v_exp);
    //        }
    //        catch (Exception exp)
    //        {
    //            _logger.Exception(Contexts.ThisComponent, exp);
    //        }
    //        #endregion
    //        GetFreqArr();
    //        if (AutoRBW == true) { GetRBW(); }
    //        if (AutoVBW == true) { GetVBW(); }
    //        if (AutoSweepTime == true) { GetSweepTime(); }
    //    }
    //    /// <summary>
    //    /// Получение Начальной частоты просмотра 
    //    /// </summary>
    //    private void GetFreqStart()
    //    {
    //        try
    //        {
    //            if (UniqueData.InstrManufacture == 1)
    //            {
    //                FreqStart = decimal.Parse(session.Query(":SENSe:FREQ:STAR?").Replace('.', ','));
    //            }
    //            else if (UniqueData.InstrManufacture == 2)
    //            {
    //                FreqStart = decimal.Parse(session.Query("FREQ:STAR?").Replace('.', ','));
    //            }
    //            else if (UniqueData.InstrManufacture == 3)
    //            {
    //                FreqCentr = decimal.Parse(session.Query(":SENSe:FREQuency:CENTer?"));
    //                FreqSpan = decimal.Parse(session.Query(":SENSe:FREQuency:SPAN?"));
    //            }
    //        }
    //        #region Exception
    //        catch (VisaException v_exp)
    //        {
    //            _logger.Exception(Contexts.ThisComponent, v_exp);
    //        }
    //        catch (Exception exp)
    //        {
    //            _logger.Exception(Contexts.ThisComponent, exp);
    //        }
    //        #endregion
    //        GetFreqArr();
    //    }
    //    /// <summary>
    //    /// Установка Конечной частоты просмотра 
    //    /// </summary>
    //    private void SetFreqStop()
    //    {
    //        try
    //        {
    //            if (UniqueData.InstrManufacture == 1)
    //            {
    //                session.Write(":SENSe:FREQ:STOP " + FreqStop.ToString().Replace(',', '.'));
    //                FreqStop = decimal.Parse(session.Query(":SENSe:FREQ:STOP?").Replace('.', ','));
    //            }
    //            else if (UniqueData.InstrManufacture == 2)
    //            {
    //                session.Write(":FREQ:STOP " + FreqStop.ToString().Replace(',', '.'));
    //                FreqStop = decimal.Parse(session.Query(":FREQ:STOP?").Replace('.', ','));
    //            }
    //            else if (UniqueData.InstrManufacture == 3)
    //            {
    //                FreqCentr = decimal.Parse(session.Query(":SENSe:FREQuency:CENTer?"));
    //                FreqSpan = decimal.Parse(session.Query(":SENSe:FREQuency:SPAN?"));
    //            }
    //        }
    //        #region Exception
    //        catch (VisaException v_exp)
    //        {
    //            _logger.Exception(Contexts.ThisComponent, v_exp);
    //        }
    //        catch (Exception exp)
    //        {
    //            _logger.Exception(Contexts.ThisComponent, exp);
    //        }
    //        #endregion
    //        GetFreqArr();
    //        if (AutoRBW == true) { GetRBW(); }
    //        if (AutoVBW == true) { GetVBW(); }
    //        if (AutoSweepTime == true) { GetSweepTime(); }
    //    }
    //    /// <summary>
    //    /// Получкние Конечной частоты просмотра 
    //    /// </summary>
    //    private void GetFreqStop()
    //    {
    //        try
    //        {
    //            if (UniqueData.InstrManufacture == 1)
    //            {
    //                FreqStop = decimal.Parse(session.Query("SENSe:FREQ:STOP?").Replace('.', ','));
    //            }
    //            else if (UniqueData.InstrManufacture == 2)
    //            {
    //                FreqStop = decimal.Parse(session.Query("FREQ:STOP?").Replace('.', ','));
    //            }
    //        }
    //        #region Exception
    //        catch (VisaException v_exp)
    //        {
    //            _logger.Exception(Contexts.ThisComponent, v_exp);
    //        }
    //        catch (Exception exp)
    //        {
    //            _logger.Exception(Contexts.ThisComponent, exp);
    //        }
    //        #endregion
    //        GetFreqArr();
    //        if (AutoRBW == true) { GetRBW(); }
    //        if (AutoVBW == true) { GetVBW(); }
    //        if (AutoSweepTime == true) { GetSweepTime(); }
    //    }
    //    #endregion Freqs

    //    #region RBW/VBW
    //    #region RBW
    //    /// <summary>
    //    /// Установка RBW 
    //    /// </summary>
    //    private void SetRBW()
    //    {
    //        try
    //        {
    //            if (UniqueData.InstrManufacture == 1)
    //            {
    //                session.Write(":SENSe:BWIDth:RESolution " + RBW.ToString().Replace(',', '.'));
    //            }
    //            else if (UniqueData.InstrManufacture == 2)
    //            {
    //                session.Write(":SENS:BAND:RES " + RBW.ToString().Replace(',', '.'));
    //            }
    //            else if (UniqueData.InstrManufacture == 3)
    //            {
    //                session.Write(":SENSe:BWIDth:RESolution " + RBW.ToString().Replace(',', '.'));
    //            }
    //        }
    //        #region Exception
    //        catch (VisaException v_exp)
    //        {
    //            _logger.Exception(Contexts.ThisComponent, v_exp);
    //        }
    //        catch (Exception exp)
    //        {
    //            _logger.Exception(Contexts.ThisComponent, exp);
    //        }
    //        #endregion
    //        AutoRBW = false;
    //        if (AutoSweepTime == true) { GetSweepTime(); }
    //        if (AutoVBW == true) { GetVBW(); }
    //        GetSweepType();
    //    }
    //    /// <summary>
    //    /// Получение RBW 
    //    /// </summary>
    //    private void GetRBW()
    //    {
    //        try
    //        {
    //            if (UniqueData.InstrManufacture == 1)
    //            {
    //                RBW = decimal.Parse(session.Query(":SENSe:BWIDth:RESolution?").Replace('.', ','));
    //                RBWIndex = System.Array.IndexOf(UniqueData.RBWArr, RBW);
    //            }
    //            else if (UniqueData.InstrManufacture == 2)
    //            {
    //                RBW = decimal.Parse(session.Query(":SENS:BAND:RES?").Replace('.', ','));
    //                RBWIndex = System.Array.IndexOf(UniqueData.RBWArr, RBW);
    //            }
    //            else if (UniqueData.InstrManufacture == 3)
    //            {
    //                RBW = decimal.Parse(session.Query(":SENSe:BWIDth:RESolution?"));
    //                RBWIndex = System.Array.IndexOf(UniqueData.RBWArr, RBW);
    //            }
    //        }
    //        #region Exception
    //        catch (VisaException v_exp)
    //        {
    //            _logger.Exception(Contexts.ThisComponent, v_exp);
    //        }
    //        catch (Exception exp)
    //        {
    //            _logger.Exception(Contexts.ThisComponent, exp);
    //        }
    //        #endregion
    //        //if (AutoSWT == true) { GetSWT(); }
    //        //if (AutoVBW == true) { GetVBW(); }
    //    }
    //    /// <summary>
    //    /// Вкл/Выкл Auto RBW 
    //    /// </summary>
    //    private void SetAutoRBW()
    //    {
    //        try
    //        {
    //            if (UniqueData.InstrManufacture == 1)
    //            {
    //                if (AutoRBW == true) session.Write(":SENSe:BWIDth:RESolution:AUTO 1");
    //                if (AutoRBW == false) session.Write(":SENSe:BWIDth:RESolution:AUTO 0");
    //            }
    //            else if (UniqueData.InstrManufacture == 2)
    //            {
    //                if (AutoRBW == true) session.Write(":SENS:BWID:RES:AUTO 1");
    //                if (AutoRBW == false) session.Write(":SENS:BWID:RES:AUTO 0");
    //            }
    //            else if (UniqueData.InstrManufacture == 3)
    //            {
    //                if (AutoRBW == true) session.Write(":SENSe:BWIDth:RESolution:AUTO 1");
    //                if (AutoRBW == false) session.Write(":SENSe:BWIDth:RESolution:AUTO 0");
    //            }
    //        }
    //        #region Exception
    //        catch (VisaException v_exp)
    //        {
    //            _logger.Exception(Contexts.ThisComponent, v_exp);
    //        }
    //        catch (Exception exp)
    //        {
    //            _logger.Exception(Contexts.ThisComponent, exp);
    //        }
    //        #endregion
    //        if (AutoSweepTime == true) { GetSweepTime(); }
    //        GetRBW();
    //        GetVBW();
    //        GetSweepType();
    //    }
    //    /// <summary>
    //    /// Получаем состояние Auto RBW 
    //    /// </summary>
    //    private void GetAutoRBW()
    //    {
    //        try
    //        {
    //            if (UniqueData.InstrManufacture == 1)
    //            {
    //                string s = session.Query(":SENSe:BWIDth:RESolution:AUTO?").TrimEnd();
    //                if (s.Contains("1")) { AutoRBW = true; }
    //                else { AutoRBW = false; }
    //            }
    //            else if (UniqueData.InstrManufacture == 2)
    //            {
    //                //    if (session.Query(":SENS:BWID:RES:AUTO?") == "0\n") { AutoRBW = false; }
    //                //    else { AutoRBW = true; }
    //            }
    //            else if (UniqueData.InstrManufacture == 3)
    //            {
    //                AutoRBW = Boolean.Parse(session.Query(":SENSe:BWIDth:RESolution:AUTO?"));
    //            }
    //        }
    //        #region Exception
    //        catch (VisaException v_exp)
    //        {
    //            _logger.Exception(Contexts.ThisComponent, v_exp);
    //        }
    //        catch (Exception exp)
    //        {
    //            _logger.Exception(Contexts.ThisComponent, exp);
    //        }
    //        #endregion
    //        if (AutoSweepTime == true) { GetSweepTime(); }
    //    }
    //    #endregion RBW

    //    #region VBW
    //    /// <summary>
    //    /// Установка VBW 
    //    /// </summary>
    //    private void SetVBW()
    //    {
    //        try
    //        {
    //            if (UniqueData.InstrManufacture == 1)
    //            {
    //                VBWIndex = System.Array.IndexOf(UniqueData.VBWArr, VBW);
    //                session.Write(":SENSe:BANDwidth:VIDeo " + UniqueData.VBWArr[VBWIndex].ToString().Replace(',', '.'));
    //            }
    //            else if (UniqueData.InstrManufacture == 2)
    //            {
    //                VBWIndex = System.Array.IndexOf(UniqueData.VBWArr, VBW);
    //                session.Write(":SENSe:BANDwidth:VIDeo " + UniqueData.VBWArr[VBWIndex].ToString().Replace(',', '.'));
    //            }
    //            else if (UniqueData.InstrManufacture == 3)
    //            {
    //                VBWIndex = System.Array.IndexOf(UniqueData.VBWArr, VBW);
    //                session.Write(":SENSe:BANDwidth:VIDeo " + UniqueData.VBWArr[VBWIndex].ToString());
    //            }
    //        }
    //        #region Exception
    //        catch (VisaException v_exp)
    //        {
    //            _logger.Exception(Contexts.ThisComponent, v_exp);
    //        }
    //        catch (Exception exp)
    //        {
    //            _logger.Exception(Contexts.ThisComponent, exp);
    //        }
    //        #endregion
    //        AutoVBW = false;
    //        if (AutoSweepTime == true && RBWIndex - VBWIndex > 0) { GetSweepTime(); }
    //        if (AutoRBW == true) { GetRBW(); }
    //        GetSweepType();
    //    }
    //    /// <summary>
    //    /// Получение VBW 
    //    /// </summary>
    //    private void GetVBW()
    //    {
    //        try
    //        {
    //            if (UniqueData.InstrManufacture == 1)
    //            {
    //                VBW = decimal.Parse(session.Query(":SENSe:BANDwidth:VIDeo?").Replace('.', ','));
    //                VBWIndex = System.Array.IndexOf(UniqueData.VBWArr, VBW);
    //            }
    //            else if (UniqueData.InstrManufacture == 2)
    //            {
    //                VBW = decimal.Parse(session.Query(":SENS:BAND:VID?").Replace('.', ','));
    //                VBWIndex = System.Array.IndexOf(UniqueData.VBWArr, VBW);
    //            }
    //            else if (UniqueData.InstrManufacture == 3)
    //            {
    //                VBW = decimal.Parse(session.Query(":SENSe:BANDwidth:VIDeo?"));
    //                VBWIndex = System.Array.IndexOf(UniqueData.VBWArr, VBW);
    //            }
    //        }
    //        #region Exception
    //        catch (VisaException v_exp)
    //        {
    //            _logger.Exception(Contexts.ThisComponent, v_exp);
    //        }
    //        catch (Exception exp)
    //        {
    //            _logger.Exception(Contexts.ThisComponent, exp);
    //        }
    //        #endregion
    //        //if (AutoSWT == true && RBWIndex - VBWIndex > 0) { GetSWT(); }
    //    }
    //    /// <summary>
    //    /// Вкл Auto RBW 
    //    /// </summary>
    //    private void SetAutoVBW()
    //    {
    //        try
    //        {
    //            if (UniqueData.InstrManufacture == 1)
    //            {
    //                if (AutoVBW == true) session.Write(":SENSe:BANDwidth:VIDeo:AUTO 1");
    //                if (AutoVBW == false) session.Write(":SENSe:BANDwidth:VIDeo:AUTO 0");
    //            }
    //            else if (UniqueData.InstrManufacture == 2)
    //            {
    //                if (AutoVBW == true) session.Write(":SENS:BAND:VID:AUTO 1");
    //                if (AutoVBW == false) session.Write(":SENS:BAND:VID:AUTO 0");
    //            }
    //            else if (UniqueData.InstrManufacture == 3)
    //            {
    //                if (AutoVBW == true) session.Write(":SENSe:BWIDth:VIDeo:AUTO 1");
    //                if (AutoVBW == false) session.Write(":SENSe:BWIDth:VIDeo:AUTO 0");
    //            }
    //        }
    //        #region Exception
    //        catch (VisaException v_exp)
    //        {
    //            _logger.Exception(Contexts.ThisComponent, v_exp);
    //        }
    //        catch (Exception exp)
    //        {
    //            _logger.Exception(Contexts.ThisComponent, exp);
    //        }
    //        #endregion
    //        if (AutoVBW == true) { GetVBW(); }
    //        if (AutoSweepTime == true) { GetSweepTime(); }
    //        GetSweepType();
    //        //:SENSe:BANDwidth|BWIDth:VIDeo:AUTO? 1
    //        //:SENSe:BWIDth:VIDeo:RATio? 
    //    }
    //    /// <summary>
    //    /// Получаем состояние Auto VBW 
    //    /// </summary>
    //    private void GetAutoVBW()
    //    {
    //        try
    //        {
    //            if (UniqueData.InstrManufacture == 1)
    //            {
    //                if (session.Query(":SENSe:BANDwidth:VIDeo:AUTO?").Contains("1")) { AutoVBW = true; }
    //                else { AutoVBW = false; }
    //            }
    //            else if (UniqueData.InstrManufacture == 2)
    //            {
    //                if (session.Query(":SENS:BAND:VID:AUTO?") == "0\n") { AutoVBW = false; }
    //                else { AutoVBW = true; }
    //            }
    //            else if (UniqueData.InstrManufacture == 3)
    //            {
    //                AutoVBW = Boolean.Parse(session.Query(":SENSe:BWIDth:VIDeo:AUTO?"));
    //            }
    //        }
    //        #region Exception
    //        catch (VisaException v_exp)
    //        {
    //            _logger.Exception(Contexts.ThisComponent, v_exp);
    //        }
    //        catch (Exception exp)
    //        {
    //            _logger.Exception(Contexts.ThisComponent, exp);
    //        }
    //        #endregion
    //        //if (AutoSWT == true) { GetSWT(); }
    //    }
    //    #endregion VBW
    //    #endregion RBW/VBW

    //    #region Sweep
    //    /// <summary>
    //    /// Установка свиптайма 
    //    /// </summary>        
    //    private void SetSweepTime()
    //    {
    //        try
    //        {
    //            if (UniqueData.InstrManufacture == 1)
    //            {
    //                session.Write(":SWE:TIME " + SweepTime.ToString().Replace(',', '.'));
    //                SweepTime = decimal.Parse(session.Query(":SWE:TIME?").Replace('.', ','));
    //                AutoSweepTime = false;
    //            }
    //            else if (UniqueData.InstrManufacture == 2)
    //            {
    //                session.Write(":SWE:TIME " + SweepTime.ToString().Replace(',', '.'));
    //                SweepTime = decimal.Parse(session.Query(":SWE:TIME?").Replace('.', ','));
    //                AutoSweepTime = false;
    //            }
    //            else if (UniqueData.InstrManufacture == 3)
    //            {
    //                session.Write(":SWE:TIME:ACT " + SweepTime.ToString().Replace(',', '.'));
    //                SweepTime = decimal.Parse(session.Query(":SWE:TIME:ACT?").Replace('.', ','));
    //                AutoSweepTime = false;
    //            }
    //        }
    //        #region Exception
    //        catch (VisaException v_exp)
    //        {
    //            _logger.Exception(Contexts.ThisComponent, v_exp);
    //        }
    //        catch (Exception exp)
    //        {
    //            _logger.Exception(Contexts.ThisComponent, exp);
    //        }
    //        #endregion
    //    }
    //    /// <summary>
    //    /// Получаем свиптайм 
    //    /// </summary>
    //    private void GetSweepTime()
    //    {
    //        try
    //        {
    //            if (UniqueData.InstrManufacture == 1)
    //            {
    //                SweepTime = decimal.Parse(session.Query(":SWE:TIME?").Replace('.', ','));
    //            }
    //            else if (UniqueData.InstrManufacture == 2)
    //            {
    //                SweepTime = decimal.Parse(session.Query(":SWE:MTIM?").Replace('.', ','));
    //            }
    //            else if (UniqueData.InstrManufacture == 3)
    //            {
    //                SweepTime = decimal.Parse(session.Query(":SWE:TIME:ACT?").Replace('.', ','));
    //            }
    //        }
    //        #region Exception
    //        catch (VisaException v_exp)
    //        {
    //            _logger.Exception(Contexts.ThisComponent, v_exp);
    //        }
    //        catch (Exception exp)
    //        {
    //            _logger.Exception(Contexts.ThisComponent, exp);
    //        }
    //        #endregion
    //    }
    //    /// <summary>
    //    /// Вкл/Выкл Auto свиптайма 
    //    /// </summary>
    //    private void SetAutoSweepTime()
    //    {
    //        try
    //        {
    //            if (UniqueData.InstrManufacture == 1)
    //            {
    //                if (AutoSweepTime == true) session.Write(":SWE:TIME:AUTO 1");
    //                if (AutoSweepTime == false) session.Write(":SWE:TIME:AUTO 0");
    //            }
    //            else if (UniqueData.InstrManufacture == 2)
    //            {
    //                if (AutoSweepTime == true) session.Write(":SENS:SWE:ACQ:AUTO 1");
    //                if (AutoSweepTime == false) session.Write(":SENS:SWE:ACQ:AUTO 0");
    //            }
    //            else if (UniqueData.InstrManufacture == 3)
    //            {
    //                if (AutoSweepTime == true) session.Write(":SENS:SWE:AUTO ON");
    //                if (AutoSweepTime == false) session.Write(":SENS:SWE:AUTO OFF");
    //            }
    //        }
    //        #region Exception
    //        catch (VisaException v_exp)
    //        {
    //            _logger.Exception(Contexts.ThisComponent, v_exp);
    //        }
    //        catch (Exception exp)
    //        {
    //            _logger.Exception(Contexts.ThisComponent, exp);
    //        }
    //        #endregion
    //        GetSweepTime();
    //    }
    //    /// <summary>
    //    /// Получаем состояние Auto SWT 
    //    /// </summary>
    //    private void GetAutoSweepTime()
    //    {
    //        try
    //        {
    //            if (UniqueData.InstrManufacture == 1)
    //            {
    //                if (session.Query(":SWE:TIME:AUTO?") == "0\n") { AutoSweepTime = false; }
    //                else { AutoSweepTime = true; }
    //            }
    //            else if (UniqueData.InstrManufacture == 2)
    //            {
    //                if (session.Query(":SENS:SWE:ACQ:AUTO?") == "0\n") { AutoSweepTime = false; }
    //                else { AutoSweepTime = true; }
    //            }
    //        }
    //        #region Exception
    //        catch (VisaException v_exp)
    //        {
    //            _logger.Exception(Contexts.ThisComponent, v_exp);
    //        }
    //        catch (Exception exp)
    //        {
    //            _logger.Exception(Contexts.ThisComponent, exp);
    //        }
    //        #endregion
    //    }
    //    /// <summary>
    //    /// Установка метода свипирования 
    //    /// </summary>        
    //    private void SetSweepType()
    //    {
    //        try
    //        {
    //            if (UniqueData.InstrManufacture == 1)
    //            {
    //                session.Write(":SWE:TYPE " + SweepTypeSelected.Parameter);
    //                //if (SweepType == 0) { session.Write(":SWE:TYPE Auto"); SweepTypeStr = "Auto"; }
    //                //else if (SweepType == 1) { session.Write(":SWE:TYPE Sweep"); SweepTypeStr = "Sweep"; }
    //                //else if (SweepType == 2) { session.Write(":SWE:TYPE FFT"); SweepTypeStr = "FFT"; }
    //            }
    //            else if (UniqueData.InstrManufacture == 2)
    //            {
    //                session.Write(":SWE:TYPE " + SweepTypeSelected.Parameter);
    //                //if (SweepType == 0) { session.Write(":SWE:TYPE AUTO"); SweepTypeStr = "Auto"; }
    //                //else if (SweepType == 1) { session.Write(":SWE:TYPE STEP"); SweepTypeStr = "STEP"; }
    //                //else if (SweepType == 2) { session.Write(":SWE:TYPE FFT"); SweepTypeStr = "FFT"; }
    //            }
    //            else if (UniqueData.InstrManufacture == 3)
    //            {
    //                session.Write(":SWE:MODE " + SweepTypeSelected.Parameter);
    //            }
    //        }
    //        #region Exception
    //        catch (VisaException v_exp)
    //        {
    //            _logger.Exception(Contexts.ThisComponent, v_exp);
    //        }
    //        catch (Exception exp)
    //        {
    //            _logger.Exception(Contexts.ThisComponent, exp);
    //        }
    //        #endregion
    //    }
    //    /// <summary>
    //    /// Получаем метод свиптайма 
    //    /// </summary> 
    //    private void GetSweepType()
    //    {
    //        try
    //        {
    //            if (UniqueData.InstrManufacture == 1)
    //            {
    //                if (UniqueData.HiSpeed == true)
    //                {
    //                    string t = session.Query(":SWE:TYPE?");
    //                    foreach (SwpType ST in UniqueData.SweepType)
    //                    {
    //                        if (t.TrimEnd().ToLower() == ST.Parameter.ToLower()) { SweepTypeSelected = ST; }
    //                    }
    //                }
    //                else { SweepTypeSelected = new SwpType { UI = "FFT", Parameter = "FFT" }; }
    //            }
    //            else if (UniqueData.InstrManufacture == 2)
    //            {
    //                string temp = session.Query(":SWE:TYPE?");
    //                foreach (SwpType ST in UniqueData.SweepType)
    //                {
    //                    if (temp.Contains(ST.Parameter)) { SweepTypeSelected = ST; }
    //                }
    //            }
    //            else if (UniqueData.InstrManufacture == 3)
    //            {
    //                string temp = session.Query(":SWE:MODE?");
    //                foreach (SwpType ST in UniqueData.SweepType)
    //                {
    //                    if (temp.Contains(ST.Parameter)) { SweepTypeSelected = ST; }
    //                }
    //            }
    //        }
    //        #region Exception
    //        catch (VisaException v_exp)
    //        {
    //            _logger.Exception(Contexts.ThisComponent, v_exp);
    //        }
    //        catch (Exception exp)
    //        {
    //            _logger.Exception(Contexts.ThisComponent, exp);
    //        }
    //        #endregion
    //    }

    //    /// <summary>
    //    /// Установка количевства точек свипов 
    //    /// </summary>        
    //    private void SetSweepPoints()
    //    {
    //        try
    //        {
    //            if (UniqueData.InstrManufacture == 1)
    //            {
    //                session.Write(":SWE:POIN " + SweepPoints.ToString());
    //                session.DefaultBufferSize = SweepPoints * 18 + 25;
    //            }
    //            else if (UniqueData.InstrManufacture == 2)
    //            {
    //                session.Write(":SENS:SWE:POIN " + SweepPoints.ToString());
    //                session.DefaultBufferSize = SweepPoints * 4 + 20;
    //            }
    //        }
    //        #region Exception
    //        catch (VisaException v_exp)
    //        {
    //            _logger.Exception(Contexts.ThisComponent, v_exp);
    //        }
    //        catch (Exception exp)
    //        {
    //            _logger.Exception(Contexts.ThisComponent, exp);
    //        }
    //        #endregion
    //        GetFreqArr();
    //    }
    //    /// <summary>
    //    /// Получаем количевства точек свипов 
    //    /// </summary>
    //    private void GetSweepPoints()
    //    {
    //        try
    //        {
    //            if (UniqueData.InstrManufacture == 1)
    //            {
    //                if (UniqueData.SweepPointFix == false)
    //                {
    //                    SweepPoints = int.Parse(session.Query(":SWE:POIN?").Replace('.', ','));
    //                    SweepPointsIndex = System.Array.IndexOf(UniqueData.SweepPointArr, SweepPoints);
    //                }
    //                if (UniqueData.HiSpeed == true) { TracePoints = SweepPoints + 2; }
    //                if (UniqueData.HiSpeed == false) { TracePoints = SweepPoints; }
    //            }
    //            else if (UniqueData.InstrManufacture == 2)
    //            {
    //                SweepPoints = int.Parse(session.Query(":SENS:SWE:POIN?").Replace('.', ','));
    //                SweepPointsIndex = System.Array.IndexOf(UniqueData.SweepPointArr, SweepPoints);
    //                session.DefaultBufferSize = SweepPoints * 4 + 20;
    //                TracePoints = SweepPoints;
    //            }
    //        }
    //        #region Exception
    //        catch (VisaException v_exp)
    //        {
    //            _logger.Exception(Contexts.ThisComponent, v_exp);
    //        }
    //        catch (Exception exp)
    //        {
    //            _logger.Exception(Contexts.ThisComponent, exp);
    //        }
    //        #endregion
    //        GetFreqArr();
    //    }
    //    #endregion Sweep

    //    #region Level
    //    /// <summary>
    //    /// Установка опорного уровня 
    //    /// </summary>
    //    private void SetRefLevel()
    //    {
    //        try
    //        {
    //            if (UniqueData.InstrManufacture == 1)
    //            {
    //                if (UniqueData.HiSpeed)
    //                { session.Write(":DISPlay:WINDow:TRACe:Y:SCALe:RLEVel " + RefLevel.ToString().Replace(',', '.')); }
    //                else { session.Write(":DISP:TRAC:Y:RLEV " + RefLevel.ToString().Replace(',', '.')); }
    //                if (AttAuto == true)
    //                {
    //                    AttLevel = decimal.Parse(session.Query(":INP:ATT?").Replace('.', ','));
    //                }

    //            }
    //            else if (UniqueData.InstrManufacture == 2)
    //            {
    //                session.Write("DISP:WIND:TRAC:Y:SCAL:RLEV " + RefLevel.ToString().Replace(',', '.'));
    //                if (AttAuto == true)
    //                {
    //                    AttLevel = decimal.Parse(session.Query(":POW:ATT?").Replace('.', ','));
    //                }
    //            }
    //            else if (UniqueData.InstrManufacture == 3)
    //            {
    //                session.Write(":DISPlay:WINDow:TRACe:Y:SCALe:RLEVel " + RefLevel.ToString());
    //            }
    //        }
    //        #region Exception
    //        catch (VisaException v_exp)
    //        {
    //            _logger.Exception(Contexts.ThisComponent, v_exp);
    //        }
    //        catch (Exception exp)
    //        {
    //            _logger.Exception(Contexts.ThisComponent, exp);
    //        }
    //        #endregion
    //        if (AttAuto == true) { GetAttLevel(); }
    //    }
    //    /// <summary>
    //    /// Получаем опорный уровнь 
    //    /// </summary>
    //    private void GetRefLevel()
    //    {
    //        try
    //        {
    //            if (UniqueData.InstrManufacture == 1)
    //            {
    //                if (UniqueData.HiSpeed)
    //                { RefLevel = Math.Round(decimal.Parse(session.Query(":DISPlay:WINDow:TRACe:Y:SCALe:RLEVel?").Replace('.', ','))); }
    //                else { RefLevel = Math.Round(decimal.Parse(session.Query(":DISP:TRAC:Y:RLEV?").Replace('.', ','))); }
    //            }
    //            else if (UniqueData.InstrManufacture == 2)
    //            {
    //                RefLevel = Math.Round(decimal.Parse(session.Query(":DISP:WIND:TRAC:Y:SCAL:RLEV?").Replace('.', ',')));
    //            }
    //            else if (UniqueData.InstrManufacture == 3)
    //            {
    //                RefLevel = Math.Round(decimal.Parse(session.Query(":DISP:WIND:TRAC:Y:SCAL:RLEV?").Replace('.', ',')));
    //            }
    //        }
    //        #region Exception
    //        catch (VisaException v_exp)
    //        {
    //            _logger.Exception(Contexts.ThisComponent, v_exp);
    //        }
    //        catch (Exception exp)
    //        {
    //            _logger.Exception(Contexts.ThisComponent, exp);
    //        }
    //        #endregion
    //    }
    //    private void SetRange()
    //    {
    //        try
    //        {
    //            if (UniqueData.InstrManufacture == 1)
    //            {
    //                session.Write(":DISP:TRAC:Y " + Range.ToString().Replace(',', '.'));
    //            }
    //            else if (UniqueData.InstrManufacture == 2)
    //            {
    //                //session.Write("DISP:WIND:TRAC:Y:SCAL:RLEV " + RefLevel.ToString().Replace(',', '.'));
    //            }
    //            else if (UniqueData.InstrManufacture == 3)
    //            {
    //                //session.Write(":DISPlay:WINDow:TRACe:Y:SCALe:RLEVel " + RefLevel.ToString());
    //            }
    //        }
    //        #region Exception
    //        catch (VisaException v_exp)
    //        {
    //            _logger.Exception(Contexts.ThisComponent, v_exp);
    //        }
    //        catch (Exception exp)
    //        {
    //            _logger.Exception(Contexts.ThisComponent, exp);
    //        }
    //        #endregion
    //        if (AttAuto == true) { GetAttLevel(); }
    //    }
    //    private void GetRange()
    //    {
    //        try
    //        {
    //            if (UniqueData.InstrManufacture == 1)
    //            {
    //                Range = Math.Round(decimal.Parse(session.Query(":DISP:TRAC:Y?").Replace('.', ',')));
    //                RangeIndex = Array.FindIndex(UniqueData.RangeArr, w => w == Range);
    //            }
    //            else if (UniqueData.InstrManufacture == 2)
    //            {
    //                //Range = Math.Round(decimal.Parse(session.Query(":DISP:WIND:TRAC:Y:SCAL:RLEV?").Replace('.', ',')));
    //            }
    //            else if (UniqueData.InstrManufacture == 3)
    //            {
    //                //Range = Math.Round(decimal.Parse(session.Query(":DISP:WIND:TRAC:Y:SCAL:RLEV?").Replace('.', ',')));
    //            }
    //        }
    //        #region Exception
    //        catch (VisaException v_exp)
    //        {
    //            _logger.Exception(Contexts.ThisComponent, v_exp);
    //        }
    //        catch (Exception exp)
    //        {
    //            _logger.Exception(Contexts.ThisComponent, exp);
    //        }
    //        #endregion
    //    }
    //    /// <summary>
    //    /// Вкл/Выкл атоматического аттенюатора зависящего от опорного уровня, при выкл
    //    /// АвтоАТТ изменяем настройку аттенюатора
    //    /// </summary>
    //    private void SetAttLevel()
    //    {
    //        try
    //        {
    //            if (UniqueData.InstrManufacture == 1)
    //            {
    //                session.Write(":INP:ATT " + AttLevel.ToString().Replace(',', '.')); //INP:ATT:AUTO
    //            }
    //            else if (UniqueData.InstrManufacture == 2)
    //            {
    //                session.Write(":POW:ATT " + AttLevel.ToString().Replace(',', '.'));
    //            }
    //        }
    //        #region Exception
    //        catch (VisaException v_exp)
    //        {
    //            _logger.Exception(Contexts.ThisComponent, v_exp);
    //        }
    //        catch (Exception exp)
    //        {
    //            _logger.Exception(Contexts.ThisComponent, exp);
    //        }
    //        #endregion
    //    }
    //    /// <summary>
    //    /// Получаем настройку аттенюатора
    //    /// </summary>
    //    private void GetAttLevel()
    //    {
    //        try
    //        {
    //            if (UniqueData.InstrManufacture == 1)
    //            {
    //                AttLevel = decimal.Parse(session.Query(":INP:ATT?").Replace('.', ','));
    //            }
    //            else if (UniqueData.InstrManufacture == 2)
    //            {
    //                AttLevel = decimal.Parse(session.Query(":POW:ATT?").Replace('.', ','));
    //            }
    //            else if (UniqueData.InstrManufacture == 3)
    //            {
    //                string t = session.Query(":POW:ATT?").Replace('.', ',');
    //                if (t != "0.0") { AttLevel = decimal.Parse(t); }
    //                else AttLevel = 0;
    //            }
    //        }
    //        #region Exception
    //        catch (VisaException v_exp)
    //        {
    //            _logger.Exception(Contexts.ThisComponent, v_exp);
    //        }
    //        catch (Exception exp)
    //        {
    //            _logger.Exception(Contexts.ThisComponent, exp);
    //        }
    //        #endregion
    //    }

    //    private void SetAutoAttLevel()
    //    {
    //        try
    //        {
    //            if (UniqueData.InstrManufacture == 1)
    //            {
    //                if (AttAuto == true) session.Write(":INP:ATT:AUTO 1");
    //                if (AttAuto == false) session.Write(":INP:ATT:AUTO 0");
    //                AttLevel = decimal.Parse(session.Query(":INP:ATT?").Replace('.', ','));
    //            }
    //            else if (UniqueData.InstrManufacture == 2)
    //            {
    //                if (AttAuto == true) session.Write(":POW:ATT:AUTO 1");
    //                if (AttAuto == false) session.Write(":POW:ATT:AUTO 0");
    //                AttLevel = decimal.Parse(session.Query(":POW:ATT?").Replace('.', ','));
    //            }
    //            else if (UniqueData.InstrManufacture == 3)
    //            {
    //                if (AttAuto == true) session.Write(":POW:ATT:AUTO 1");
    //                if (AttAuto == false) session.Write(":POW:ATT:AUTO 0");
    //                AttLevel = decimal.Parse(session.Query(":POW:ATT?").Replace('.', ','));
    //            }
    //        }
    //        #region Exception
    //        catch (VisaException v_exp)
    //        {
    //            _logger.Exception(Contexts.ThisComponent, v_exp);
    //        }
    //        catch (Exception exp)
    //        {
    //            _logger.Exception(Contexts.ThisComponent, exp);
    //        }
    //        #endregion
    //    }
    //    private void GetAutoAttLevel()
    //    {
    //        try
    //        {
    //            if (UniqueData.InstrManufacture == 1)
    //            {
    //                string temp = session.Query(":INP:ATT:AUTO?");
    //                if (temp == "1\n") { AttAuto = true; }
    //                else if (temp == "0\n") { AttAuto = false; }
    //            }
    //            else if (UniqueData.InstrManufacture == 2)
    //            {
    //                string temp = session.Query(":POW:ATT:AUTO?");
    //                if (temp == "1\n") { AttAuto = true; }
    //                else if (temp == "0\n") { AttAuto = false; }
    //            }
    //            else if (UniqueData.InstrManufacture == 3)
    //            {
    //                string temp = session.Query(":POW:ATT:AUTO?");
    //                if (temp == "1\n") { AttAuto = true; }
    //                else if (temp == "0\n") { AttAuto = false; }
    //            }
    //        }
    //        #region Exception
    //        catch (VisaException v_exp)
    //        {
    //            _logger.Exception(Contexts.ThisComponent, v_exp);
    //        }
    //        catch (Exception exp)
    //        {
    //            _logger.Exception(Contexts.ThisComponent, exp);
    //        }
    //        #endregion
    //    }
    //    /// <summary>
    //    /// Вкл/Выкл предусилителя 
    //    /// </summary>
    //    private void SetPreAmp()
    //    {
    //        try
    //        {
    //            if (UniqueData.InstrManufacture == 1 && UniqueData.PreAmp == true)
    //            {
    //                if (PreAmp == true) session.Write(":INP:GAIN:STAT 1");
    //                if (PreAmp == false) session.Write(":INP:GAIN:STAT 0");
    //            }
    //            else if (UniqueData.InstrManufacture == 2)
    //            {
    //                if (PreAmp == true) session.Write(":POW:GAIN 1");
    //                if (PreAmp == false) session.Write(":POW:GAIN 0");
    //            }
    //            else if (UniqueData.InstrManufacture == 3)
    //            {
    //                if (PreAmp == true) session.Write(":POW:GAIN 1");
    //                if (PreAmp == false) session.Write(":POW:GAIN 0");
    //            }
    //        }
    //        #region Exception
    //        catch (VisaException v_exp)
    //        {
    //            _logger.Exception(Contexts.ThisComponent, v_exp);
    //        }
    //        catch (Exception exp)
    //        {
    //            _logger.Exception(Contexts.ThisComponent, exp);
    //        }
    //        #endregion
    //    }
    //    /// <summary>
    //    /// Узнаем состояние предусилителя 
    //    /// </summary>
    //    private void GetPreAmp()
    //    {
    //        try
    //        {
    //            if (UniqueData.InstrManufacture == 1 && UniqueData.PreAmp == true)
    //            {
    //                string temp = session.Query(":INP:GAIN:STAT?");
    //                if (temp == "1\n") { PreAmp = true; }
    //                else if (temp == "0\n") { PreAmp = false; }
    //            }
    //            else if (UniqueData.InstrManufacture == 2)
    //            {
    //                string temp = session.Query(":POW:GAIN:STAT?");
    //                if (temp == "1\n") { PreAmp = true; }
    //                else if (temp == "0\n") { PreAmp = false; }
    //            }
    //            else if (UniqueData.InstrManufacture == 3)
    //            {
    //                string temp = session.Query(":POW:GAIN:STAT?");
    //                if (temp.Contains("1")) { PreAmp = true; }
    //                else if (temp.Contains("0")) { PreAmp = false; }
    //            }
    //        }
    //        #region Exception
    //        catch (VisaException v_exp)
    //        {
    //            _logger.Exception(Contexts.ThisComponent, v_exp);
    //        }
    //        catch (Exception exp)
    //        {
    //            _logger.Exception(Contexts.ThisComponent, exp);
    //        }
    //        #endregion
    //    }
    //    /// <summary>
    //    /// Установка Units
    //    /// </summary>
    //    private void SetLevelUnit()
    //    {
    //        try
    //        {
    //            if (UniqueData.InstrManufacture == 1)
    //            {
    //                session.Write(":UNIT:POWer " + LevelUnits[LevelUnitIndex].AnParameter);
    //                //if (LevelUnit == 0) session.Write(":UNIT:POWer DBM");
    //                //else if (LevelUnit == 1) session.Write(":UNIT:POWer DBMV");
    //                //else if (LevelUnit == 2) session.Write(":UNIT:POWer DBUV");
    //                //else if (LevelUnit == 2) session.Write(":UNIT:POWer DBUV/M");//dBuV/M
    //            }
    //            else if (UniqueData.InstrManufacture == 2)
    //            {
    //                session.Write(":AMPL:UNIT " + LevelUnits[LevelUnitIndex].AnParameter);
    //                //if (LevelUnit == 0) session.Write(":AMPL:UNIT DBM");
    //                //else if (LevelUnit == 1) session.Write(":AMPL:UNIT DBMV");
    //                //else if (LevelUnit == 2) session.Write(":AMPL:UNIT DBUV");
    //                //else if (LevelUnit == 2) session.Write(":AMPL:UNIT DBUV/M");//dBuV/M
    //            }
    //            else if (UniqueData.InstrManufacture == 3)
    //            {
    //                session.Write(":UNIT:POWer " + LevelUnits[LevelUnitIndex].AnParameter);
    //                //if (LevelUnit == 0) session.Write(":UNIT:POWer DBM");
    //                //else if (LevelUnit == 1) session.Write(":UNIT:POWer DBMV");
    //                //else if (LevelUnit == 2) session.Write(":UNIT:POWer DBUV");
    //                //else if (LevelUnit == 2) session.Write(":UNIT:POWer DBUV/M");//dBuV/M
    //            }
    //        }
    //        #region Exception
    //        catch (VisaException v_exp)
    //        {
    //            _logger.Exception(Contexts.ThisComponent, v_exp);
    //        }
    //        catch (Exception exp)
    //        {
    //            _logger.Exception(Contexts.ThisComponent, exp);
    //        }
    //        #endregion
    //        finally
    //        {
    //            GetRefLevel();
    //            GetLevelUnit();
    //        }
    //    }
    //    /// <summary>
    //    /// Получаем Units 
    //    /// </summary>
    //    private void GetLevelUnit()
    //    {
    //        try
    //        {
    //            if (UniqueData.InstrManufacture == 1)
    //            {
    //                if (UniqueData.HiSpeed)
    //                {
    //                    for (int i = 0; i < LevelUnits.Count(); i++)
    //                    {
    //                        if (LevelUnits[i].ind == 0) { LevelUnits[i].AnParameter = "DBM"; }
    //                        else if (LevelUnits[i].ind == 1) { LevelUnits[i].AnParameter = "DBMV"; }
    //                        else if (LevelUnits[i].ind == 2) { LevelUnits[i].AnParameter = "DBUV"; }
    //                        else if (LevelUnits[i].ind == 3) { LevelUnits[i].AnParameter = "DBUV/M"; }
    //                    }
    //                }
    //                else
    //                {
    //                    for (int i = 0; i < LevelUnits.Count(); i++)
    //                    {
    //                        if (LevelUnits[i].ind == 0) { LevelUnits[i].AnParameter = "DBM"; }
    //                        else if (LevelUnits[i].ind == 1) { LevelUnits[i].AnParameter = "DBMV"; }
    //                        else if (LevelUnits[i].ind == 2) { LevelUnits[i].AnParameter = "DBUV"; }
    //                        else if (LevelUnits[i].ind == 3) { LevelUnits[i].AnParameter = "DUVM"; }
    //                    }
    //                }

    //                string temp = session.Query(":UNIT:POWer?").TrimEnd();
    //                for (int i = 0; i < LevelUnits.Count(); i++)
    //                {
    //                    if (temp.ToLower() == LevelUnits[i].AnParameter.ToLower())
    //                    {
    //                        LevelUnitIndex = i;
    //                        if (LevelUnits[i].ind == 3) { LevelUnits[i].IsEnabled = true; }
    //                        else { LevelUnits[3].IsEnabled = false; }
    //                    }
    //                }
    //            }
    //            else if (UniqueData.InstrManufacture == 2)
    //            {
    //                for (int i = 0; i < LevelUnits.Count(); i++)
    //                {
    //                    if (LevelUnits[i].ind == 0) { LevelUnits[i].AnParameter = "DBM"; }
    //                    else if (LevelUnits[i].ind == 1) { LevelUnits[i].AnParameter = "DBMV"; }
    //                    else if (LevelUnits[i].ind == 2) { LevelUnits[i].AnParameter = "DBUV"; }
    //                    else if (LevelUnits[i].ind == 3) { LevelUnits[i].IsEnabled = false; }
    //                }
    //                string temp = session.Query(":AMPL:UNIT?").TrimEnd();
    //                for (int i = 0; i < LevelUnits.Count(); i++)
    //                {
    //                    if (temp.ToLower() == LevelUnits[i].AnParameter.ToLower()) { LevelUnitIndex = i; }
    //                }
    //            }
    //            else if (UniqueData.InstrManufacture == 3)
    //            {
    //                for (int i = 0; i < LevelUnits.Count(); i++)
    //                {
    //                    if (LevelUnits[i].ind == 0) { LevelUnits[i].AnParameter = "dBm"; }
    //                    else if (LevelUnits[i].ind == 1) { LevelUnits[i].AnParameter = "dBmV"; }
    //                    else if (LevelUnits[i].ind == 2) { LevelUnits[i].AnParameter = "dBuV"; }
    //                    else if (LevelUnits[i].ind == 3) { LevelUnits[i].IsEnabled = false; }
    //                }
    //                string temp = session.Query(":UNIT:POWer?").TrimEnd();
    //                for (int i = 0; i < LevelUnits.Count(); i++)
    //                {
    //                    if (temp.ToLower() == LevelUnits[i].AnParameter.ToLower()) { LevelUnitIndex = i; }
    //                }
    //            }
    //        }
    //        #region Exception
    //        catch (VisaException v_exp)
    //        {
    //            _logger.Exception(Contexts.ThisComponent, v_exp);
    //        }
    //        catch (Exception exp)
    //        {
    //            _logger.Exception(Contexts.ThisComponent, exp);
    //        }
    //        #endregion            
    //    }

    //    #endregion Level

    //    #region Trace

    //    private void SetTraceType()
    //    {
    //        try
    //        {
    //            if (UniqueData.InstrManufacture == 1)
    //            {
    //                session.Write(":DISP:TRAC1:MODE " + Trace1Type.Parameter);
    //                session.Write(":DISP:TRAC2:MODE " + Trace2Type.Parameter);
    //                session.Write(":DISP:TRAC3:MODE " + Trace3Type.Parameter);
    //            }
    //            else if (UniqueData.InstrManufacture == 2)
    //            {
    //                session.Write(":TRAC1:TYPE " + Trace1Type.Parameter);
    //                session.Write(":TRAC2:TYPE " + Trace2Type.Parameter);
    //                session.Write(":TRAC3:TYPE " + Trace3Type.Parameter);
    //            }
    //            else if (UniqueData.InstrManufacture == 3)
    //            {
    //                session.Write(":TRACe1:OPERation " + Trace1Type.Parameter);
    //                session.Write(":TRACe2:OPERation " + Trace2Type.Parameter);
    //                session.Write(":TRACe3:OPERation " + Trace3Type.Parameter);
    //            }
    //        }
    //        #region Exception
    //        catch (VisaException v_exp)
    //        {
    //            _logger.Exception(Contexts.ThisComponent, v_exp);
    //        }
    //        catch (Exception exp)
    //        {
    //            _logger.Exception(Contexts.ThisComponent, exp);
    //        }
    //        #endregion
    //    }
    //    private void GetTraceType()
    //    {
    //        try
    //        {
    //            if (UniqueData.InstrManufacture == 1)
    //            {
    //                string temp1 = string.Empty;
    //                string temp2 = string.Empty;
    //                string temp3 = string.Empty;
    //                if (UniqueData.HiSpeed == true)
    //                {
    //                    if (session.Query(":DISP:TRAC1?").Contains("1")) { temp1 = session.Query(":DISP:TRAC1:MODE?"); }
    //                    else { Trace1Type = new ParamWithUI { UI = "Blank", Parameter = "BLAN" }; }
    //                    if (session.Query(":DISP:TRAC2?").Contains("1")) { temp2 = session.Query(":DISP:TRAC2:MODE?"); }
    //                    else { Trace2Type = new ParamWithUI { UI = "Blank", Parameter = "BLAN" }; }
    //                    if (session.Query(":DISP:TRAC3?").Contains("1")) { temp3 = session.Query(":DISP:TRAC3:MODE?"); }
    //                    else { Trace3Type = new ParamWithUI { UI = "Blank", Parameter = "BLAN" }; }
    //                }
    //                else if (UniqueData.HiSpeed == false)
    //                {
    //                    temp1 = session.Query(":DISP:TRAC1:MODE?");
    //                    temp2 = "BLAN";
    //                    temp3 = "BLAN";
    //                }
    //                foreach (ParamWithUI TT in UniqueData.TraceType)
    //                {
    //                    if (temp1.Contains(TT.Parameter)) { Trace1Type = TT; }
    //                    if (temp2.Contains(TT.Parameter)) { Trace2Type = TT; }
    //                    if (temp3.Contains(TT.Parameter)) { Trace3Type = TT; }
    //                }
    //            }
    //            else if (UniqueData.InstrManufacture == 2)
    //            {
    //                string temp1 = session.Query(":TRACe1:TYPE?");
    //                //string temp2 = session.Query(":TRACe2:TYPE?");
    //                //string temp3 = session.Query(":TRACe3:TYPE?");
    //                foreach (ParamWithUI TT in UniqueData.TraceType)
    //                {
    //                    if (temp1.Contains(TT.Parameter)) { Trace1Type = TT; }
    //                    //if (temp2.Contains(TT.Parameter)) { Trace2Type = TT; }
    //                    //if (temp3.Contains(TT.Parameter)) { Trace3Type = TT; }
    //                }
    //            }
    //            else if (UniqueData.InstrManufacture == 3)
    //            {
    //                string temp1 = session.Query(":TRACe1:OPERation?");
    //                //string temp2 = session.Query(":TRACe2:OPERation?");
    //                //string temp3 = session.Query(":TRACe3:OPERation?");
    //                foreach (ParamWithUI TT in UniqueData.TraceType)
    //                {
    //                    if (temp1.Contains(TT.Parameter)) { Trace1Type = TT; }
    //                    //if (temp2.Contains(TT.Parameter)) { Trace2Type = TT; }
    //                    //if (temp3.Contains(TT.Parameter)) { Trace3Type = TT; }
    //                }
    //            }
    //        }
    //        #region Exception
    //        catch (VisaException v_exp)
    //        {
    //            _logger.Exception(Contexts.ThisComponent, v_exp);
    //        }
    //        catch (Exception exp)
    //        {
    //            _logger.Exception(Contexts.ThisComponent, exp);
    //        }
    //        #endregion
    //    }

    //    private void SetResetTrace()
    //    {
    //        try
    //        {
    //            if (UniqueData.InstrManufacture == 1)
    //            {
    //                session.Write(":DISP:TRAC" + TraceNumberToReset.ToString() + ":MODE " + UniqueData.TraceType[0].Parameter);
    //                session.Write(":DISP:TRAC" + TraceNumberToReset.ToString() + ":MODE " + Trace1Type.Parameter);
    //            }
    //            else if (UniqueData.InstrManufacture == 2)
    //            {
    //                session.Write(":TRAC" + TraceNumberToReset.ToString() + ":TYPE " + UniqueData.TraceType[0].Parameter);
    //                session.Write(":TRAC" + TraceNumberToReset.ToString() + ":TYPE " + Trace1Type.Parameter);
    //            }
    //            else if (UniqueData.InstrManufacture == 3)
    //            {
    //                session.Write(":TRACe" + TraceNumberToReset.ToString() + ":OPERation " + UniqueData.TraceType[0].Parameter);
    //                session.Write(":TRACe" + TraceNumberToReset.ToString() + ":OPERation " + Trace1Type.Parameter);
    //            }
    //        }
    //        #region Exception
    //        catch (VisaException v_exp)
    //        {
    //            _logger.Exception(Contexts.ThisComponent, v_exp);
    //        }
    //        catch (Exception exp)
    //        {
    //            _logger.Exception(Contexts.ThisComponent, exp);
    //        }
    //        #endregion
    //    }
    //    private void SetDetectorType()
    //    {
    //        try
    //        {
    //            if (UniqueData.InstrManufacture == 1)
    //            {
    //                if (Trace1Detector.Parameter == "Auto Select") session.Write(":SENSe:DETector1:AUTO 1");
    //                else
    //                {
    //                    session.Write(":SENSe:DETector1 " + Trace1Detector.Parameter);
    //                }
    //                if (UniqueData.HiSpeed == true && Trace2Detector.Parameter == "Auto Select") session.Write(":SENSe:DETector2:AUTO 1");
    //                else
    //                {
    //                    session.Write(":SENSe:DETector2 " + Trace2Detector.Parameter);
    //                }
    //                if (UniqueData.HiSpeed == true && Trace3Detector.Parameter == "Auto Select") session.Write(":SENSe:DETector3:AUTO 1");
    //                else
    //                {
    //                    session.Write(":SENSe:DETector3 " + Trace3Detector.Parameter);
    //                }
    //            }
    //            else if (UniqueData.InstrManufacture == 2)
    //            {
    //                //if (Trace1Detector.UI == "Auto Select") session.Write(":SENSe:DETector:AUTO");
    //                //else
    //                //{
    //                session.Write(":SENSe:DETector:FUNCtion " + Trace1Detector.Parameter);
    //                //}
    //            }
    //            else if (UniqueData.InstrManufacture == 3)
    //            {
    //                if (Trace1Detector.Parameter == "Auto Select") session.Write(":SENSe:DETector1:AUTO 1");
    //                else
    //                {
    //                    session.Write(":SENSe:DETector:FUNCtion " + Trace1Detector.Parameter);
    //                }
    //            }
    //        }
    //        #region Exception
    //        catch (VisaException v_exp)
    //        {
    //            _logger.Exception(Contexts.ThisComponent, v_exp);
    //        }
    //        catch (Exception exp)
    //        {
    //            _logger.Exception(Contexts.ThisComponent, exp);
    //        }
    //        #endregion
    //        GetVBW();
    //    }
    //    /// <summary>
    //    /// Получение типа Trace Detecror 
    //    /// </summary>
    //    private void GetDetectorType()
    //    {
    //        try
    //        {
    //            if (UniqueData.InstrManufacture == 1)
    //            {

    //                string temp1 = string.Empty;
    //                string temp2 = string.Empty;
    //                string temp3 = string.Empty;
    //                if (UniqueData.HiSpeed == true)
    //                {
    //                    temp1 = session.Query(":SENSe:DETector1?").TrimEnd();
    //                    temp2 = session.Query(":SENSe:DETector2?");
    //                    temp3 = session.Query(":SENSe:DETector3?");
    //                }
    //                else if (UniqueData.HiSpeed == false)
    //                {
    //                    temp1 = session.Query(":SENSe:DETector1?").TrimEnd();
    //                    temp2 = "POS";
    //                    temp3 = "POS";
    //                }
    //                foreach (TrDetector TT in UniqueData.TraceDetector)
    //                {
    //                    if (temp1.Contains(TT.Parameter)) { Trace1Detector = TT; }
    //                    if (temp2.Contains(TT.Parameter)) { Trace2Detector = TT; }
    //                    if (temp3.Contains(TT.Parameter)) { Trace3Detector = TT; }
    //                }
    //            }
    //            else if (UniqueData.InstrManufacture == 2)
    //            {
    //                string temp1 = session.Query(":SENSe:DETector:FUNCtion?").TrimEnd();
    //                foreach (TrDetector TT in UniqueData.TraceDetector)
    //                {
    //                    if (temp1.Contains(TT.Parameter)) { Trace1Detector = TT; }
    //                }
    //            }
    //            else if (UniqueData.InstrManufacture == 3)
    //            {
    //                string temp1 = session.Query(":SENSe:DETector:FUNCtion?").TrimEnd();
    //                foreach (TrDetector TT in UniqueData.TraceDetector)
    //                {
    //                    if (temp1.Contains(TT.Parameter)) { Trace1Detector = TT; }
    //                }
    //            }
    //        }
    //        #region Exception
    //        catch (VisaException v_exp)
    //        {
    //            _logger.Exception(Contexts.ThisComponent, v_exp);
    //        }
    //        catch (Exception exp)
    //        {
    //            _logger.Exception(Contexts.ThisComponent, exp);
    //        }
    //        #endregion
    //    }


    //    /// <summary>
    //    /// Настраиваем усреднение
    //    /// </summary>
    //    private void SetAverageCount()
    //    {
    //        try
    //        {
    //            if (UniqueData.InstrManufacture == 1)
    //            {
    //                if (UniqueData.HiSpeed == true)
    //                {
    //                    if (AveragingCount > 30000) AveragingCount = 30000;
    //                    else if (AveragingCount < 1) AveragingCount = 1;
    //                    session.Write(":SENSe:AVERage:COUNt " + AveragingCount.ToString().Replace(',', '.'));
    //                }
    //                else if (UniqueData.HiSpeed == false)
    //                {
    //                    if (AveragingCount > 1000) AveragingCount = 999;
    //                    else if (AveragingCount < 1) AveragingCount = 1;
    //                    session.Write("SENSe:SWEep:COUNt " + AveragingCount.ToString().Replace(',', '.'));
    //                }
    //            }
    //            if (UniqueData.InstrManufacture == 2)
    //            {
    //                if (AveragingCount > 1000) AveragingCount = 1000;
    //                else if (AveragingCount < 1) AveragingCount = 1;
    //                session.Write(":SENSe:AVERage:COUNt " + AveragingCount.ToString().Replace(',', '.'));
    //            }
    //        }
    //        #region Exception
    //        catch (VisaException v_exp)
    //        {
    //            _logger.Exception(Contexts.ThisComponent, v_exp);
    //        }
    //        catch (Exception exp)
    //        {
    //            _logger.Exception(Contexts.ThisComponent, exp);
    //        }
    //        #endregion
    //    }
    //    /// <summary>
    //    /// Получаем усреднение
    //    /// </summary>
    //    private void GetAverageCount()
    //    {
    //        try
    //        {
    //            if (UniqueData.InstrManufacture == 1)
    //            {
    //                if (UniqueData.HiSpeed == true) { AveragingCount = int.Parse(session.Query(":SENSe:AVERage:COUNt?").Replace('.', ',')); }
    //                else if (UniqueData.HiSpeed == false) { AveragingCount = int.Parse(session.Query("SENSe:SWEep:COUNt?").Replace('.', ',')); }
    //            }
    //            if (UniqueData.InstrManufacture == 2)
    //            {
    //                AveragingCount = int.Parse(session.Query(":SENSe:AVERage:COUNt?").Replace('.', ','));
    //            }
    //        }
    //        #region Exception
    //        catch (VisaException v_exp)
    //        {
    //            _logger.Exception(Contexts.ThisComponent, v_exp);
    //        }
    //        catch (Exception exp)
    //        {
    //            _logger.Exception(Contexts.ThisComponent, exp);
    //        }
    //        #endregion
    //    }
    //    /// <summary>
    //    /// Получаем количевство трейсов в усреднении
    //    /// </summary>
    //    private void GetNumberOfSweeps()
    //    {
    //        try
    //        {
    //            if (UniqueData.InstrManufacture == 1)
    //            {
    //                if (UniqueData.HiSpeed == true)
    //                {
    //                    NumberOfSweeps = int.Parse(session.Query(":SWE:COUN:CURR?").Replace('.', ','));
    //                    if (NumberOfSweeps == AveragingCount) { NOSEquallyAC = true; }
    //                    else { NOSEquallyAC = false; }
    //                }
    //                else if (UniqueData.HiSpeed == false) { NumberOfSweeps = AveragingCount; }
    //            }
    //            else if (UniqueData.InstrManufacture == 2)
    //            {
    //                //NumberOfSweeps = int.Parse(session.Query(":SWE:COUN:CURR?").Replace('.', ','));
    //                //NumberOfSweepsStr = String.Concat(NumberOfSweeps, "/", AveragingCount);
    //                //if (NumberOfSweeps == AveragingCount) { NOSEquallyAC = true; }
    //                //else { NOSEquallyAC = false; }
    //            }
    //        }
    //        #region Exception
    //        catch (VisaException v_exp)
    //        {
    //            _logger.Exception(Contexts.ThisComponent, v_exp);
    //        }
    //        catch (Exception exp)
    //        {
    //            _logger.Exception(Contexts.ThisComponent, exp);
    //        }
    //        #endregion
    //    }
    //    #endregion Trace

    //    #region Markers
    //    private void SetMarkerAllOff()
    //    {
    //        try
    //        {
    //            if (UniqueData.InstrManufacture == 1)
    //            {
    //                session.Write(":CALC:MARK:AOFF");
    //                session.Write(":CALC:DELT:AOFF");
    //            }
    //            else if (UniqueData.InstrManufacture == 2)
    //            {
    //                session.Write(":CALC:MARK:AOFF");
    //                //session.Write(":CALC:DELT:AOFF");
    //            }
    //            else if (UniqueData.InstrManufacture == 3)
    //            {
    //                session.Write(":CALC:MARK:AOFF");
    //                //session.Write(":CALC:DELT:AOFF");
    //            }
    //        }
    //        #region Exception
    //        catch (VisaException v_exp)
    //        {
    //            _logger.Exception(Contexts.ThisComponent, v_exp);
    //        }
    //        catch (Exception exp)
    //        {
    //            _logger.Exception(Contexts.ThisComponent, exp);
    //        }
    //        #endregion
    //    }
    //    #endregion Markers

    //    private void GetDeviceInfo()
    //    {
    //        try
    //        {
    //            if (UniqueData.InstrManufacture == 1)
    //            {
    //                if (UniqueData.Battery)
    //                {
    //                    string[] st = session.Query(":STAT:QUES:POW?;:SYSTem:POWer:STATus?").TrimEnd().Split(';');
    //                    PowerRegister = int.Parse(st[0]);
    //                    double d = double.Parse(st[1], System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture);
    //                    if (d > 100) { BatteryCharging = true; }
    //                    else if (d > 0 && d <= 100) { BatteryCharging = false; BatteryCharge = (decimal)d; }
    //                }
    //                else
    //                {
    //                    PowerRegister = int.Parse(session.Query(":STAT:QUES:POW?"));
    //                }
    //            }
    //            else if (UniqueData.InstrManufacture == 2)
    //            {
    //                string t = session.Query(":SYST:BATT:ARTT?");
    //                //BatteryAbsoluteCharge = int.Parse(session.Query(":SYST:BATT:ACUR?").Replace('.', ','));
    //                //System.Windows.MessageBox.Show(t);
    //            }
    //        }
    //        #region Exception
    //        catch (VisaException v_exp)
    //        {
    //            _logger.Exception(Contexts.ThisComponent, v_exp);
    //        }
    //        catch (Exception exp)
    //        {
    //            _logger.Exception(Contexts.ThisComponent, exp);
    //        }
    //        #endregion
    //    }
    //    private void InstrShutdown()
    //    {
    //        try
    //        {
    //            if (UniqueData.InstrManufacture == 1)
    //            {
    //                session.Write(":SYSTem:SHUTdown");
    //            }
    //            else if (UniqueData.InstrManufacture == 2)
    //            {
    //                session.Write(":SYST:PWR:SHUT 1");
    //            }
    //            Run = false;
    //            //session.Dispose();
    //            //session = null;
    //        }
    //        #region Exception
    //        catch (VisaException v_exp)
    //        {
    //            _logger.Exception(Contexts.ThisComponent, v_exp);
    //        }
    //        catch (Exception exp)
    //        {
    //            _logger.Exception(Contexts.ThisComponent, exp);
    //        }
    //        #endregion
    //    }

    //    /// <summary>
    //    /// Сбрасываем настойки прибора 
    //    /// </summary>
    //    private void Preset()
    //    {
    //        try
    //        {
    //            if (UniqueData.InstrManufacture == 1)
    //            {
    //                session.Write(":SYSTem:PRES");
    //            }
    //            else if (UniqueData.InstrManufacture == 3)
    //            {
    //                session.Write(":SYSTem:PRES");
    //            }
    //            GetLevelUnit();
    //            GetFreqCentr();
    //            GetFreqSpan();
    //            GetRBW();
    //            GetAutoRBW();
    //            GetVBW();
    //            GetAutoVBW();

    //            //an_dm += SetCouplingRatio;
    //            GetSweepTime();
    //            GetAutoSweepTime();
    //            GetSweepType();
    //            GetSweepPoints();
    //            GetRefLevel();
    //            GetAttLevel();
    //            GetAutoAttLevel();
    //            GetPreAmp();
    //            GetTraceType();
    //            GetDetectorType();
    //            GetAverageCount();
    //            GetNumberOfSweeps();


    //            GetTransducer();
    //            GetSelectedTransducer();
    //            GetSetAnSysDateTime();
    //        }
    //        #region Exception
    //        catch (VisaException v_exp)
    //        {
    //            _logger.Exception(Contexts.ThisComponent, v_exp);
    //        }
    //        catch (Exception exp)
    //        {
    //            _logger.Exception(Contexts.ThisComponent, exp);
    //        }
    //        #endregion
    //    }

    //    private void GetSetAnSysDateTime()
    //    {
    //        try
    //        {
    //            if (UniqueData.InstrManufacture == 1)
    //            {
    //                string[] d = session.Query("SYST:DATE?").TrimEnd().Trim(' ').Split(',');
    //                string[] t = session.Query("SYST:TIME?").TrimEnd().Trim(' ').Split(',');
    //                string time = d[0] + "-" + d[1] + "-" + d[2] + " " +
    //                    t[0] + ":" + t[1] + ":" + t[2];
    //                DateTime andt = DateTime.Parse(time);
    //                if (new TimeSpan(DateTime.Now.Ticks - andt.Ticks) > new TimeSpan(0, 0, 1, 0, 0))
    //                {
    //                    session.Write("SYST:DATE " + DateTime.Now.Year.ToString() + "," + DateTime.Now.Month.ToString() + "," + DateTime.Now.Day.ToString());
    //                    session.Write("SYST:TIME " + DateTime.Now.Hour.ToString() + "," + DateTime.Now.Minute.ToString() + "," + DateTime.Now.Second.ToString());
    //                }
    //            }
    //            else if (UniqueData.InstrManufacture == 2)
    //            {

    //            }
    //            else if (UniqueData.InstrManufacture == 3)
    //            {

    //            }
    //        }
    //        #region Exception
    //        catch (VisaException v_exp)
    //        {
    //            _logger.Exception(Contexts.ThisComponent, v_exp);
    //        }
    //        catch (Exception exp)
    //        {
    //            _logger.Exception(Contexts.ThisComponent, exp);
    //        }
    //        #endregion
    //    }
    //    #region Transducer
    //    private void GetTransducer()
    //    {
    //        try
    //        {
    //            if (UniqueData.InstrManufacture == 1)
    //            {
    //                if (UniqueData.HiSpeed == true)
    //                {
    //                    string[] t = session.Query(@"MMEM:CAT? 'C:\R_S\Instr\trd\*.tdf'").Replace("'", "").Replace(".TDF", "").Split(',');
    //                    List<Transducer> temp = new List<Transducer> { };
    //                    for (int i = 0; i < t.Length; i++)
    //                    {
    //                        temp.Add(new Transducer { Name = t[i], Active = false });
    //                    }
    //                    UniqueData.Transducers = temp; //GetSelectedTransducer();
    //                }
    //                else if (UniqueData.HiSpeed == false && (UniqueData.InstrModel == "FPH" || UniqueData.InstrModel == "ZVH"))
    //                {
    //                    session.Write(@"MMEM:CDIR '\Public\Transducers'");
    //                    string s = session.Query(@"MMEM:CAT?");
    //                    //MessageBox.Show(s);
    //                    string[] t = s.Replace("'", "").Split(',');
    //                    List<Transducer> temp = new List<Transducer> { };
    //                    for (int i = 0; i < (int)(t.Length / 9); i++)
    //                    {
    //                        if (t[3 + 9 * i] == "pritrd" || t[3 + 9 * i] == "isotrd")
    //                        {
    //                            temp.Add(new Transducer { Name = t[2 + 9 * i], FileType = t[3 + 9 * i], Active = false });
    //                        }
    //                    }
    //                    UniqueData.Transducers = temp;
    //                }
    //            }
    //        }
    //        #region Exception
    //        catch (VisaException v_exp)
    //        {
    //            _logger.Exception(Contexts.ThisComponent, v_exp);
    //        }
    //        catch (Exception exp)
    //        {
    //            _logger.Exception(Contexts.ThisComponent, exp);
    //        }
    //        #endregion
    //    }
    //    private void GetSelectedTransducer()
    //    {
    //        try
    //        {
    //            if (UniqueData.InstrManufacture == 1)
    //            {
    //                if (UniqueData.HiSpeed == true)
    //                {
    //                    string s = session.Query("CORR:TRAN:SEL?").TrimEnd();
    //                    bool any = false;
    //                    foreach (Transducer tr in UniqueData.Transducers)
    //                    {
    //                        if (s.ToLower().Contains(tr.Name.ToLower()))
    //                        { TransducerSelected = tr; tr.Active = true; any = true; }
    //                        else { tr.Active = false; }
    //                    }
    //                    AnyTransducerSet = any;
    //                    //string temp;
    //                    //foreach (Transducer tr in UniqueData.Transducers)
    //                    //{
    //                    //    session.Write("CORR:TRAN:SEL '" + tr.Name + "'");
    //                    //    temp = session.Query("CORR:TRAN:STAT?").TrimEnd();
    //                    //        tr.Active = temp == "0" ? false : true;
    //                    //    //if (temp.Contains(tr.Name)) { tr.Active = true; }
    //                    //    //else { tr.Active = false; }
    //                    //}
    //                }
    //                else
    //                {
    //                    string s = session.Query("CORR:TRAN1:SEL?").TrimEnd();
    //                    bool any = false;
    //                    foreach (Transducer tr in UniqueData.Transducers)
    //                    {
    //                        if (s.ToLower().Contains(tr.Name.ToLower()))
    //                        { TransducerSelected = tr; tr.Active = true; any = true; }
    //                        else { tr.Active = false; }
    //                    }
    //                    AnyTransducerSet = any;
    //                }
    //            }
    //        }
    //        #region Exception
    //        catch (VisaException v_exp)
    //        {
    //            _logger.Exception(Contexts.ThisComponent, v_exp);
    //        }
    //        catch (Exception exp)
    //        {
    //            _logger.Exception(Contexts.ThisComponent, exp);
    //        }
    //        #endregion
    //    }
    //    private void SetSelectedTransducer()
    //    {
    //        try
    //        {
    //            if (UniqueData.InstrManufacture == 1)
    //            {

    //                if (UniqueData.HiSpeed == true)
    //                {
    //                    if (TransducerSelected.Active)
    //                    {
    //                        session.Write("CORR:TRAN OFF");
    //                        session.Write("CORR:TRAN:SEL '" + TransducerSelected.Name + "'");
    //                        session.Write("CORR:TRAN ON");
    //                    }
    //                    else { session.Write("CORR:TRAN:SEL '" + TransducerSelected.Name + "'"); session.Write("CORR:TRAN OFF"); }
    //                    AnyTransducerSet = TransducerSelected.Active;
    //                    foreach (Transducer tr in UniqueData.Transducers)
    //                    {
    //                        if (tr.Active == true && TransducerSelected.Name != tr.Name) { tr.Active = false; }
    //                    }
    //                }
    //                else
    //                {
    //                    if (TransducerSelected.Active)
    //                    {
    //                        session.Write("CORR:TRAN1 OFF");
    //                        session.Write("CORR:TRAN1:SEL '" + TransducerSelected.Name + "." + TransducerSelected.FileType + "'");
    //                        session.Write("CORR:TRAN1 ON");
    //                    }
    //                    else { session.Write("CORR:TRAN1 OFF"); }
    //                    AnyTransducerSet = TransducerSelected.Active;
    //                    foreach (Transducer tr in UniqueData.Transducers)
    //                    {
    //                        if (tr.Active == true && TransducerSelected.Name != tr.Name)
    //                        {
    //                            tr.Active = false;
    //                        }
    //                    }
    //                }

    //            }
    //        }
    //        #region Exception
    //        catch (VisaException v_exp)
    //        {
    //            _logger.Exception(Contexts.ThisComponent, v_exp);
    //        }
    //        catch (Exception exp)
    //        {
    //            _logger.Exception(Contexts.ThisComponent, exp);
    //        }
    //        #endregion
    //    }
    //    #endregion Transducer
    //    #endregion AN Method

    //    #region Adapter Properties
    //    private void SetDefaulConfig(ref CFG.AdapterMainConfig config)
    //    {
    //        config.IQBitRateMax = 40;
    //        config.AdapterEquipmentInfo = new CFG.AdapterEquipmentInfo()
    //        {
    //            AntennaManufacturer = "AntennaManufacturer",
    //            AntennaName = "Omni",
    //            AntennaSN = "123"
    //        };
    //        config.AdapterRadioPathParameters = new CFG.AdapterRadioPathParameter[]
    //        {
    //            new CFG.AdapterRadioPathParameter()
    //            {
    //                Freq = 1*1000000,
    //                KTBF = -147,//уровень своих шумов на Гц
    //                FeederLoss = 2,//потери фидера
    //                Gain = 10, //коэф усиления
    //                DiagA = "HV",
    //                DiagH = "POINT 0 0 90 3 180 6 270 3",//от нуля В конфиг
    //                DiagV = "POINT -90 20 0 0 90 10"//от -90  до 90 В конфиг
    //            },
    //            new CFG.AdapterRadioPathParameter()
    //            {
    //                Freq = 1000*1000000,
    //                KTBF = -147,//уровень своих шумов на Гц
    //                FeederLoss = 2,//потери фидера
    //                Gain = 10, //коэф усиления
    //                DiagA = "HV",
    //                DiagH = "POINT 0 0 90 3 180 6 270 3",//от нуля В конфиг
    //                DiagV = "POINT -90 20 0 0 90 10"//от -90  до 90 В конфиг
    //            }
    //        };
    //    }

    //    private (MesureTraceDeviceProperties, MesureIQStreamDeviceProperties) GetProperties(CFG.AdapterMainConfig config)
    //    {
    //        RadioPathParameters[] rrps = ConvertRadioPathParameters(config);
    //        StandardDeviceProperties sdp = new StandardDeviceProperties()
    //        {
    //            AttMax_dB = 30,
    //            AttMin_dB = 0,
    //            FreqMax_Hz = FreqMax,
    //            FreqMin_Hz = FreqMin,
    //            PreAmpMax_dB = 30,
    //            PreAmpMin_dB = 0,
    //            RefLevelMax_dBm = (int)UniqueData.RefLevelMax,
    //            RefLevelMin_dBm = (int)UniqueData.RefLevelMin,
    //            EquipmentInfo = new EquipmentInfo()
    //            {
    //                AntennaCode = config.AdapterEquipmentInfo.AntennaSN,// "Omni",//S/N  В конфиг
    //                AntennaManufacturer = config.AdapterEquipmentInfo.AntennaManufacturer,//"3anet",//В конфиг
    //                AntennaName = config.AdapterEquipmentInfo.AntennaName,//"BC600",//В конфиг
    //                EquipmentManufacturer = new Atdi.DataModels.Sdrn.DeviceServer.Adapters.InstrManufacrures().SignalHound.UI,
    //                EquipmentName = UniqueData.InstrModel,
    //                EquipmentFamily = "SpectrumAnalyzer",//SDR/SpecAn/MonRec
    //                EquipmentCode = UniqueData.SerialNumber,//S/N

    //            },
    //            RadioPathParameters = rrps
    //        };
    //        MesureTraceDeviceProperties mtdp = new MesureTraceDeviceProperties()
    //        {
    //            RBWMax_Hz = (double)UniqueData.RBWArr[UniqueData.RBWArr.Length - 1],
    //            RBWMin_Hz = (double)UniqueData.RBWArr[0],
    //            SweepTimeMin_s = UniqueData.SWTMin,
    //            SweepTimeMax_s = UniqueData.SWTMax,
    //            StandardDeviceProperties = sdp,
    //            //DeviceId ничего не писать, ID этого экземпляра адаптера
    //        };
    //        MesureIQStreamDeviceProperties miqdp = new MesureIQStreamDeviceProperties()
    //        {
    //            AvailabilityPPS = false,// Т.к. нет у анализаторов спектра их, хотя через тригеры можно попробывать
    //            BitRateMax_MBs = config.IQBitRateMax,
    //            //DeviceId ничего не писать, ID этого экземпляра адаптера
    //            standartDeviceProperties = sdp,
    //        };

    //        return (mtdp, miqdp);
    //    }

    //    private RadioPathParameters[] ConvertRadioPathParameters(CFG.AdapterMainConfig config)
    //    {
    //        RadioPathParameters[] rpps = new RadioPathParameters[config.AdapterRadioPathParameters.Length];
    //        for (int i = 0; i < config.AdapterRadioPathParameters.Length; i++)
    //        {
    //            rpps[i] = new RadioPathParameters()
    //            {
    //                Freq_Hz = config.AdapterRadioPathParameters[i].Freq,
    //                KTBF_dBm = config.AdapterRadioPathParameters[i].KTBF,//уровень своих шумов на Гц
    //                FeederLoss_dB = config.AdapterRadioPathParameters[i].FeederLoss,//потери фидера
    //                Gain = config.AdapterRadioPathParameters[i].Gain, //коэф усиления
    //                DiagA = config.AdapterRadioPathParameters[i].DiagA,
    //                DiagH = config.AdapterRadioPathParameters[i].DiagH,//от нуля В конфиг
    //                DiagV = config.AdapterRadioPathParameters[i].DiagV//от -90  до 90 В конфиг
    //            };
    //        }
    //        return rpps;
    //    }
    //    #endregion Adapter Properties
    //    #endregion Private Method



    //}

}
