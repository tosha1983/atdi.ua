using Atdi.Contracts.Sdrn.DeviceServer;
using Atdi.DataModels.Sdrn.DeviceServer;
using Atdi.DataModels.Sdrn.DeviceServer.Adapters;
using Atdi.Platform.Logging;
using System;
using System.Collections.Generic;
using COM = Atdi.DataModels.Sdrn.DeviceServer.Commands;
using COMR = Atdi.DataModels.Sdrn.DeviceServer.Commands.Results;
using EN = Atdi.AppUnits.Sdrn.DeviceServer.Adapters.SpectrumAnalyzer.Enums;
using MEN = Atdi.DataModels.Sdrn.DeviceServer.Adapters.Enums;

namespace Atdi.AppUnits.Sdrn.DeviceServer.Adapters.SpectrumAnalyzer
{
    /// <summary>
    /// Пример реализации объект аадаптера
    /// </summary>
    //public class Adapter : IAdapter
    //{
    //    private readonly ILogger _logger;
    //    private readonly AdapterConfig _adapterConfig;
    //    private LocalParametersConverter LPC;
    //    /// <summary>
    //    /// Все объекты адаптера создаются через DI-контейнер 
    //    /// Запрашиваем через конструктор необходимые сервисы
    //    /// </summary>
    //    /// <param name="adapterConfig"></param>
    //    /// <param name="logger"></param>
    //    public Adapter(AdapterConfig adapterConfig, ILogger logger)
    //    {
    //        this._logger = logger;
    //        this._adapterConfig = adapterConfig;
    //        LPC = new LocalParametersConverter();
    //        FreqArr = new double[TracePoints];
    //        LevelArr = new float[TracePoints];
    //        for (int i = 0; i < TracePoints; i++)
    //        {

    //            //FreqArr[i] = (double)(FreqStart + FreqStep * i);
    //            //LevelArr[i] = -100;

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
    //            //host.RegisterHandler<COM.MesureGpsLocationExampleCommand, MesureGpsLocationExampleAdapterResult>(MesureGpsLocationExampleCommandHandler);
    //            host.RegisterHandler<COM.MesureTraceCommand, COMR.MesureTraceResult>(MesureTraceParameterHandler);
    //        }
    //        #region Exception
    //        catch (Exception exp)
    //        {
    //            _logger.Exception(Contexts.ThisComponent, exp);
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

    //                // важно: если измерение идет в поток еадаптера то в принцпе в явных блокировках смысла нет - адаптер полностью занят и другие кломаныд обработаить не сможет
    //                // функции блокировки имеют смысл если мы для измерений создает отдельные потоки а этот освобождаем для прослушивани яследующих комманд

    //                // сценарйи в данном случаи за разарбочиком адаптера

    //                // что то меряем

    //                //if (FreqStart != command.Parameter.FreqStart_Hz || FreqStop != command.Parameter.FreqStop_Hz)
    //                //{
    //                //    FreqStart = LPC.FreqStart(this, command.Parameter.FreqStart_Hz);
    //                //    FreqStop = LPC.FreqStop(this, command.Parameter.FreqStop_Hz);
                        
    //                //}

    //                //EN.Attenuator att = LPC.Attenuator(command.Parameter.Att_dB);
    //                //if (Attenuator != att || RefLevel != command.Parameter.RefLevel_dBm)
    //                //{
    //                //    Attenuator = att;
    //                //    RefLevel = command.Parameter.RefLevel_dBm;
    //                //    Status = AdapterDriver.bbConfigureLevel(_Device_ID, (double)RefLevel, (double)Attenuator);
    //                //    if (Status != EN.Status.NoError)
    //                //    {
    //                //        _logger.Warning(Contexts.ThisComponent, AdapterDriver.bbGetStatusString(Status));
    //                //    }
    //                //}

    //                //EN.Gain gain = LPC.Gain(command.Parameter.PreAmp_dB);
    //                //if (gain != Gain)
    //                //{
    //                //    Gain = LPC.Gain(command.Parameter.PreAmp_dB);
    //                //    Status = AdapterDriver.bbConfigureGain(_Device_ID, (int)Gain);
    //                //    if (Status != EN.Status.NoError)
    //                //    {
    //                //        _logger.Warning(Contexts.ThisComponent, AdapterDriver.bbGetStatusString(Status));
    //                //    }
    //                //}

    //                //decimal rbw = LPC.RBW(this, (decimal)command.Parameter.RBW_Hz);
    //                //decimal vbw = LPC.VBW(this, (decimal)command.Parameter.VBW_Hz);
    //                //if (RBW != rbw || VBW != vbw || SweepTime != (decimal)command.Parameter.SweepTime_s)
    //                //{
    //                //    RBW = rbw;
    //                //    VBW = vbw;
    //                //    SweepTime = (decimal)command.Parameter.SweepTime_s;
    //                //    Status = AdapterDriver.bbConfigureSweepCoupling(_Device_ID, (double)RBW, (double)VBW, (double)SweepTime, (uint)RBWShape, (uint)Rejection);
    //                //    if (Status != EN.Status.NoError)
    //                //    {
    //                //        _logger.Warning(Contexts.ThisComponent, AdapterDriver.bbGetStatusString(Status));
    //                //    }
    //                //}

    //                //if (command.Parameter.TraceCount > 0)
    //                //{
    //                //    TraceCountToMeas = (ulong)command.Parameter.TraceCount;
    //                //    TraceCount = 0;
    //                //    TracePoints = command.Parameter.TracePoint;
    //                //}
    //                //else
    //                //{
    //                //    throw new Exception("TraceCount must be set greater than zero.");
    //                //}

    //                //Status = AdapterDriver.bbInitiate(_Device_ID, (uint)DeviceMode, 0);
    //                //if (Status != EN.Status.NoError)
    //                //{
    //                //    _logger.Warning(Contexts.ThisComponent, AdapterDriver.bbGetStatusString(Status));
    //                //}

    //                //for (ulong i = 0; i < TraceCountToMeas; i++)
    //                //{
    //                //    if (GetTrace())
    //                //    {
    //                //        // пушаем результат
    //                //        var result = new COMR.MesureTraceResult(TraceCount, CommandResultStatus.Next);
    //                //        TraceCount++;
    //                //        if (TraceCountToMeas == TraceCount)
    //                //        {
    //                //            result = new COMR.MesureTraceResult(TraceCount, CommandResultStatus.Final);
    //                //        }
    //                //        result.Freq_Hz = new double[FreqArr.Length];
    //                //        result.Level = new float[FreqArr.Length];
    //                //        for (int j = 0; j < FreqArr.Length; j++)
    //                //        {
    //                //            result.Freq_Hz[j] = FreqArr[j];
    //                //            result.Level[j] = LevelArr[j];
    //                //        }
    //                //        result.TimeStamp = 0; //пофиксить

    //                //        context.PushResult(result);
    //                //    }

    //                //    // иногда нужно проверять токен окончания работы комманды
    //                //    if (context.Token.IsCancellationRequested)
    //                //    {
    //                //        // все нужно остановиться

    //                //        // если есть порция данных возвращаем ее в обработчки только говрим что поток результатов не законченный и больше уже не будет поступать
    //                //        var result2 = new COMR.MesureTraceResult(TraceCount, CommandResultStatus.Ragged);

    //                //        context.PushResult(result2);


    //                //        // подтверждаем факт обработки отмены
    //                //        context.Cancel();
    //                //        // освобождаем поток 
    //                //        return;
    //                //    }

    //                //}

    //                // снимаем блокировку с текущей команды
    //                context.Unlock();

    //                // что то делаем еще 


    //                // подтверждаем окончание выполнения комманды 
    //                // важно: всн ранее устапнволеные в контексте обработки текущей команыд блокировки снимаются автоматически
    //                context.Finish();
    //                // дальше кода быть не должно, освобождаем поток
    //            }
    //            else
    //            {
    //                throw new Exception("The device with serial number " + Device_SerialNumber + " does not work");
    //            }
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
    //                #region 
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
    //                #endregion
    //            },
    //            DefaultInstrOption = new List<DeviceOption>() { },
    //            LoadedInstrOption = new List<DeviceOption>() { },
    //            TraceType = new List<ParamWithId>
    //            {
    //                #region
    //                new ParamWithId {Id = (int)EN.TraceType.ClearWrite, Parameter = "WRIT" },
    //                new ParamWithId {Id = (int)EN.TraceType.Average, Parameter = "AVER" },
    //                new ParamWithId {Id = (int)EN.TraceType.MaxHold, Parameter = "MAXH" },
    //                new ParamWithId {Id = (int)EN.TraceType.MinHold, Parameter = "MINH" },
    //                new ParamWithId {Id = (int)EN.TraceType.View, Parameter = "VIEW" },
    //                new ParamWithId {Id = (int)EN.TraceType.Blank, Parameter = "BLAN" }
    //                #endregion
    //            },
    //            TraceDetector = new List<ParamWithId>
    //            {
    //                #region
    //                new ParamWithId {Id = (int)EN.TraceDetector.AutoSelect, Parameter = "Auto Select" },
    //                new ParamWithId {Id = (int)EN.TraceDetector.AutoPeak, Parameter = "APE" },
    //                new ParamWithId {Id = (int)EN.TraceDetector.Average, Parameter = "AVER" },
    //                new ParamWithId {Id = (int)EN.TraceDetector.MaxPeak, Parameter = "POS" },
    //                new ParamWithId {Id = (int)EN.TraceDetector.MinPeak, Parameter = "NEG" },
    //                new ParamWithId {Id = (int)EN.TraceDetector.Sample, Parameter = "SAMP" },
    //                new ParamWithId {Id = (int)EN.TraceDetector.RMS, Parameter = "RMS" }
    //                #endregion
    //            },
    //            SweepType =  new List<ParamWithId>
    //            {
    //                #region
    //                new ParamWithId {Id = (int)EN.SweepType.Auto, Parameter = "AUTO" },
    //                new ParamWithId {Id = (int)EN.SweepType.Sweep, Parameter = "SWE" },
    //                new ParamWithId {Id = (int)EN.SweepType.FFT, Parameter = "FFT" }
    //                #endregion
    //            },
    //            LevelUnits = new List<ParamWithId>
    //            {
    //                #region
    //                new ParamWithId {Id = (int)MEN.LevelUnit.dBm, Parameter = "DBM" },
    //                new ParamWithId {Id = (int)MEN.LevelUnit.dBmV, Parameter = "DBMV" },
    //                new ParamWithId {Id = (int)MEN.LevelUnit.dBµV, Parameter = "DBUV" },
    //                new ParamWithId {Id = (int)MEN.LevelUnit.dBµVm, Parameter = "DBUV/M" }
    //                #endregion
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
    //                #region 
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
    //                #endregion
    //            },
    //            DefaultInstrOption = new List<DeviceOption>() { },
    //            LoadedInstrOption = new List<DeviceOption>() { },
    //            TraceType = new List<ParamWithId>
    //            {
    //                #region
    //                new ParamWithId {Id = (int)EN.TraceType.ClearWrite, Parameter = "WRIT" },
    //                new ParamWithId {Id = (int)EN.TraceType.Average, Parameter = "AVER" },
    //                new ParamWithId {Id = (int)EN.TraceType.MaxHold, Parameter = "MAXH" },
    //                new ParamWithId {Id = (int)EN.TraceType.MinHold, Parameter = "MINH" },
    //                new ParamWithId {Id = (int)EN.TraceType.View, Parameter = "VIEW" },
    //                new ParamWithId {Id = (int)EN.TraceType.Blank, Parameter = "BLAN" }
    //                #endregion
    //            },
    //            TraceDetector = new List<ParamWithId>
    //            {
    //                #region
    //                new ParamWithId {Id = (int)EN.TraceDetector.AutoSelect, Parameter = "Auto Select" },
    //                new ParamWithId {Id = (int)EN.TraceDetector.AutoPeak, Parameter = "APE" },
    //                new ParamWithId {Id = (int)EN.TraceDetector.Average, Parameter = "AVER" },
    //                new ParamWithId {Id = (int)EN.TraceDetector.MaxPeak, Parameter = "POS" },
    //                new ParamWithId {Id = (int)EN.TraceDetector.MinPeak, Parameter = "NEG" },
    //                new ParamWithId {Id = (int)EN.TraceDetector.Sample, Parameter = "SAMP" },
    //                new ParamWithId {Id = (int)EN.TraceDetector.RMS, Parameter = "RMS" }
    //                #endregion
    //            },
    //            SweepType =  new List<ParamWithId>
    //            {
    //                #region
    //                new ParamWithId {Id = (int)EN.SweepType.Auto, Parameter = "AUTO" },
    //                new ParamWithId {Id = (int)EN.SweepType.Sweep, Parameter = "SWE" },
    //                new ParamWithId {Id = (int)EN.SweepType.FFT, Parameter = "FFT" }
    //                #endregion
    //            },
    //            LevelUnits = new List<ParamWithId>
    //            {
    //                #region
    //                new ParamWithId {Id = (int)MEN.LevelUnit.dBm, Parameter = "DBM" },
    //                new ParamWithId {Id = (int)MEN.LevelUnit.dBmV, Parameter = "DBMV" },
    //                new ParamWithId {Id = (int)MEN.LevelUnit.dBµV, Parameter = "DBUV" },
    //                new ParamWithId {Id = (int)MEN.LevelUnit.dBµVm, Parameter = "DBUV/M" }
    //                #endregion
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
    //                #region 
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
    //                #endregion
    //            },
    //            DefaultInstrOption = new List<DeviceOption>() { },
    //            LoadedInstrOption = new List<DeviceOption>() { },
    //            TraceType = new List<ParamWithId>
    //            {
    //                #region
    //                new ParamWithId {Id = (int)EN.TraceType.ClearWrite, Parameter = "WRIT" },
    //                new ParamWithId {Id = (int)EN.TraceType.Average, Parameter = "AVER" },
    //                new ParamWithId {Id = (int)EN.TraceType.MaxHold, Parameter = "MAXH" },
    //                new ParamWithId {Id = (int)EN.TraceType.MinHold, Parameter = "MINH" },
    //                new ParamWithId {Id = (int)EN.TraceType.View, Parameter = "VIEW" },
    //                new ParamWithId {Id = (int)EN.TraceType.Blank, Parameter = "BLAN" }
    //                #endregion
    //            },
    //            TraceDetector = new List<ParamWithId>
    //            {
    //                #region
    //                new ParamWithId {Id = (int)EN.TraceDetector.AutoSelect, Parameter = "Auto Select" },
    //                new ParamWithId {Id = (int)EN.TraceDetector.AutoPeak, Parameter = "APE" },
    //                new ParamWithId {Id = (int)EN.TraceDetector.Average, Parameter = "AVER" },
    //                new ParamWithId {Id = (int)EN.TraceDetector.MaxPeak, Parameter = "POS" },
    //                new ParamWithId {Id = (int)EN.TraceDetector.MinPeak, Parameter = "NEG" },
    //                new ParamWithId {Id = (int)EN.TraceDetector.Sample, Parameter = "SAMP" },
    //                new ParamWithId {Id = (int)EN.TraceDetector.RMS, Parameter = "RMS" }
    //                #endregion
    //            },
    //            SweepType =  new List<ParamWithId>
    //            {
    //                #region
    //                new ParamWithId {Id = (int)EN.SweepType.Auto, Parameter = "AUTO" },
    //                new ParamWithId {Id = (int)EN.SweepType.Sweep, Parameter = "SWE" },
    //                new ParamWithId {Id = (int)EN.SweepType.FFT, Parameter = "FFT" }
    //                #endregion
    //            },
    //            LevelUnits = new List<ParamWithId>
    //            {
    //                #region
    //                new ParamWithId {Id = (int)MEN.LevelUnit.dBm, Parameter = "DBM" },
    //                new ParamWithId {Id = (int)MEN.LevelUnit.dBmV, Parameter = "DBMV" },
    //                new ParamWithId {Id = (int)MEN.LevelUnit.dBµV, Parameter = "DBUV" },
    //                new ParamWithId {Id = (int)MEN.LevelUnit.dBµVm, Parameter = "DBUV/M" }
    //                #endregion
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
    //                #region 
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
    //                #endregion
    //            },
    //            DefaultInstrOption = new List<DeviceOption>() { },
    //            LoadedInstrOption = new List<DeviceOption>() { },
    //            TraceType = new List<ParamWithId>
    //            {
    //                #region
    //                new ParamWithId {Id = (int)EN.TraceType.ClearWrite, Parameter = "WRIT" },
    //                new ParamWithId {Id = (int)EN.TraceType.Average, Parameter = "AVER" },
    //                new ParamWithId {Id = (int)EN.TraceType.MaxHold, Parameter = "MAXH" },
    //                new ParamWithId {Id = (int)EN.TraceType.MinHold, Parameter = "MINH" },
    //                new ParamWithId {Id = (int)EN.TraceType.Blank, Parameter = "BLAN" },
    //                new ParamWithId {Id = (int)EN.TraceType.View, Parameter = "VIEW" },
    //                #endregion
    //            },
    //            TraceDetector = new List<ParamWithId>
    //            {
    //                #region
    //                new ParamWithId {Id = (int)EN.TraceDetector.AutoSelect, Parameter = "Auto Select" },
    //                new ParamWithId {Id = (int)EN.TraceDetector.AutoPeak, Parameter = "APE" },
    //                new ParamWithId {Id = (int)EN.TraceDetector.Average, Parameter = "AVER" },
    //                new ParamWithId {Id = (int)EN.TraceDetector.MaxPeak, Parameter = "POS" },
    //                new ParamWithId {Id = (int)EN.TraceDetector.MinPeak, Parameter = "NEG" },
    //                new ParamWithId {Id = (int)EN.TraceDetector.Sample, Parameter = "SAMP" },
    //                new ParamWithId {Id = (int)EN.TraceDetector.RMS, Parameter = "RMS" }
    //                #endregion
    //            },
    //            SweepType =  new List<ParamWithId>
    //            {
    //                #region
    //                new ParamWithId {Id = (int)EN.SweepType.Auto, Parameter = "AUTO" },
    //                new ParamWithId {Id = (int)EN.SweepType.Sweep, Parameter = "SWE" },
    //                new ParamWithId {Id = (int)EN.SweepType.FFT, Parameter = "FFT" }
    //                #endregion
    //            },
    //            LevelUnits = new List<ParamWithId>
    //            {
    //                #region
    //                new ParamWithId {Id = (int)MEN.LevelUnit.dBm, Parameter = "DBM" },
    //                new ParamWithId {Id = (int)MEN.LevelUnit.dBmV, Parameter = "DBMV" },
    //                new ParamWithId {Id = (int)MEN.LevelUnit.dBµV, Parameter = "DBUV" },
    //                new ParamWithId {Id = (int)MEN.LevelUnit.dBµVm, Parameter = "DBUV/M" }
    //                #endregion
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
    //                #region 
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
    //                #endregion
    //            },
    //            DefaultInstrOption = new List<DeviceOption>() { },
    //            LoadedInstrOption = new List<DeviceOption>() { },
    //            TraceType = new List<ParamWithId>
    //            {
    //                #region
    //                new ParamWithId {Id = (int)EN.TraceType.ClearWrite, Parameter = "WRIT" },
    //                new ParamWithId {Id = (int)EN.TraceType.Average, Parameter = "AVER" },
    //                new ParamWithId {Id = (int)EN.TraceType.MaxHold, Parameter = "MAXH" },
    //                new ParamWithId {Id = (int)EN.TraceType.MinHold, Parameter = "MINH" },
    //                new ParamWithId {Id = (int)EN.TraceType.View, Parameter = "VIEW" },
    //                new ParamWithId {Id = (int)EN.TraceType.Blank, Parameter = "BLAN" }    
    //                #endregion
    //            },
    //            TraceDetector = new List<ParamWithId>
    //            {
    //                #region
    //                new ParamWithId {Id = (int)EN.TraceDetector.AutoSelect, Parameter = "Auto Select" },
    //                new ParamWithId {Id = (int)EN.TraceDetector.AutoPeak, Parameter = "APE" },
    //                new ParamWithId {Id = (int)EN.TraceDetector.MaxPeak, Parameter = "POS" },
    //                new ParamWithId {Id = (int)EN.TraceDetector.MinPeak, Parameter = "NEG" },
    //                new ParamWithId {Id = (int)EN.TraceDetector.Sample, Parameter = "SAMP" },
    //                new ParamWithId {Id = (int)EN.TraceDetector.RMS, Parameter = "RMS" }
    //                #endregion
    //            },
    //            LevelUnits = new List<ParamWithId>
    //            {
    //                #region
    //                new ParamWithId {Id = (int)MEN.LevelUnit.dBm, Parameter = "DBM" },
    //                new ParamWithId {Id = (int)MEN.LevelUnit.dBmV, Parameter = "DBMV" },
    //                new ParamWithId {Id = (int)MEN.LevelUnit.dBµV, Parameter = "DBUV" },
    //                #endregion
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
    //                #region 
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
    //                 #endregion
    //            },
    //            DefaultInstrOption = new List<DeviceOption>()
    //            {
    //                #region 
    //                new DeviceOption(){Type = "B22", Designation = "Preamplifier", GlobalType = "Preamplifier"}
    //                #endregion
    //            },
    //            LoadedInstrOption = new List<DeviceOption>(),
    //            TraceType = new List<ParamWithId>
    //            {
    //                #region
    //                new ParamWithId {Id = (int)EN.TraceType.ClearWrite, Parameter = "WRIT" },
    //                new ParamWithId {Id = (int)EN.TraceType.Average, Parameter = "AVER" },
    //                new ParamWithId {Id = (int)EN.TraceType.MaxHold, Parameter = "MAXH" },
    //                new ParamWithId {Id = (int)EN.TraceType.MinHold, Parameter = "MINH" },
    //                new ParamWithId {Id = (int)EN.TraceType.View, Parameter = "VIEW" },
    //                new ParamWithId {Id = (int)EN.TraceType.Blank, Parameter = "BLAN" }
    //                #endregion
    //            },
    //            TraceDetector = new List<ParamWithId>
    //            {
    //                #region
    //                new ParamWithId {Id = (int)EN.TraceDetector.AutoSelect, Parameter = "Auto Select" },
    //                new ParamWithId {Id = (int)EN.TraceDetector.AutoPeak, Parameter = "APE" },
    //                new ParamWithId {Id = (int)EN.TraceDetector.MaxPeak, Parameter = "POS" },
    //                new ParamWithId {Id = (int)EN.TraceDetector.MinPeak, Parameter = "NEG" },
    //                new ParamWithId {Id = (int)EN.TraceDetector.Sample, Parameter = "SAMP" },
    //                new ParamWithId {Id = (int)EN.TraceDetector.RMS, Parameter = "RMS" }
    //                #endregion
    //            },
    //            LevelUnits = new List<ParamWithId>
    //            {
    //                #region
    //                new ParamWithId {Id = (int)MEN.LevelUnit.dBm, Parameter = "DBM" },
    //                new ParamWithId {Id = (int)MEN.LevelUnit.dBmV, Parameter = "DBMV" },
    //                new ParamWithId {Id = (int)MEN.LevelUnit.dBµV, Parameter = "DBUV" },
    //                #endregion
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
    //        #region ZVH
    //        new LocalSpectrumAnalyzerInfo
    //        {
    //            InstrManufacture = 1, NumberOfTrace = 1, ChangeableSweepType = false, SweepPointFix = true, InstrModel = "ZVH",
    //            HiSpeed = false,
    //            InstrOption = new List<DeviceOption>()
    //            {
    //                #region 
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
    //                #endregion
    //            },
    //            DefaultInstrOption = new List<DeviceOption>()
    //            {
    //                #region 
    //                new DeviceOption(){Type = "B22", Designation = "Preamplifier", GlobalType = "Preamplifier"}
    //                #endregion
    //            },
    //            LoadedInstrOption = new List<DeviceOption>(),
    //            TraceType = new List<ParamWithId>
    //            {
    //                #region
    //                new ParamWithId {Id = (int)EN.TraceType.ClearWrite, Parameter = "WRIT" },
    //                new ParamWithId {Id = (int)EN.TraceType.Average, Parameter = "AVER" },
    //                new ParamWithId {Id = (int)EN.TraceType.MaxHold, Parameter = "MAXH" },
    //                new ParamWithId {Id = (int)EN.TraceType.MinHold, Parameter = "MINH" },
    //                new ParamWithId {Id = (int)EN.TraceType.View, Parameter = "VIEW" },
    //                new ParamWithId {Id = (int)EN.TraceType.Blank, Parameter = "BLAN" }
    //                #endregion
    //            },
    //            TraceDetector = new List<ParamWithId>
    //            {
    //                #region
    //                new ParamWithId {Id = (int)EN.TraceDetector.AutoSelect, Parameter = "Auto Select" },
    //                new ParamWithId {Id = (int)EN.TraceDetector.AutoPeak, Parameter = "APE" },
    //                new ParamWithId {Id = (int)EN.TraceDetector.MaxPeak, Parameter = "POS" },
    //                new ParamWithId {Id = (int)EN.TraceDetector.MinPeak, Parameter = "NEG" },
    //                new ParamWithId {Id = (int)EN.TraceDetector.Sample, Parameter = "SAMP" },
    //                new ParamWithId {Id = (int)EN.TraceDetector.RMS, Parameter = "RMS" }
    //                #endregion
    //            },
    //            LevelUnits = new List<ParamWithId>
    //            {
    //                #region
    //                new ParamWithId {Id = (int)MEN.LevelUnit.dBm, Parameter = "DBM" },
    //                new ParamWithId {Id = (int)MEN.LevelUnit.dBmV, Parameter = "DBMV" },
    //                new ParamWithId {Id = (int)MEN.LevelUnit.dBµV, Parameter = "DBUV" },
    //                #endregion
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
    //                #region
    //                new ParamWithId {Id = (int)EN.TraceType.ClearWrite, Parameter = "CLRW" },
    //                new ParamWithId {Id = (int)EN.TraceType.Average, Parameter = "AVG" },
    //                new ParamWithId {Id = (int)EN.TraceType.MaxHold, Parameter = "MAXH" },
    //                new ParamWithId {Id = (int)EN.TraceType.MinHold, Parameter = "MINH" },
    //                new ParamWithId {Id = (int)EN.TraceType.View, Parameter = "VIEW" },
    //                new ParamWithId {Id = (int)EN.TraceType.Blank, Parameter = "BLANk" }
    //                #endregion
    //            },
    //            TraceDetector = new List<ParamWithId>
    //            {
    //                #region
    //                new ParamWithId {Id = (int)EN.TraceDetector.AutoSelect, Parameter = "AUTO" },
    //                new ParamWithId {Id = (int)EN.TraceDetector.Average, Parameter = "AVER" },
    //                new ParamWithId {Id = (int)EN.TraceDetector.MaxPeak, Parameter = "POS" },
    //                new ParamWithId {Id = (int)EN.TraceDetector.MinPeak, Parameter = "NEG" },
    //                new ParamWithId {Id = (int)EN.TraceDetector.Sample, Parameter = "SAMP" },
    //                new ParamWithId {Id = (int)EN.TraceDetector.Normal, Parameter = "  NORMal" }
    //                #endregion
    //            },
    //            SweepType =  new List<ParamWithId>
    //            {
    //                #region
    //                new ParamWithId {Id = (int)EN.SweepType.Auto, Parameter = "AUTO" },
    //                new ParamWithId {Id = (int)EN.SweepType.Sweep, Parameter = "STEP" },
    //                new ParamWithId {Id = (int)EN.SweepType.FFT, Parameter = "FFT" }
    //                #endregion
    //            },
    //            LevelUnits = new List<ParamWithId>
    //            {
    //                #region
    //                new ParamWithId {Id = (int)MEN.LevelUnit.dBm, Parameter = "DBM" },
    //                new ParamWithId {Id = (int)MEN.LevelUnit.dBmV, Parameter = "DBMV" },
    //                new ParamWithId {Id = (int)MEN.LevelUnit.dBµV, Parameter = "DBUV" },
    //                #endregion
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
    //                #region
    //                new ParamWithId {Id = (int)EN.TraceType.ClearWrite, Parameter = "NORM" },
    //                new ParamWithId {Id = (int)EN.TraceType.Average, Parameter = "AVER" },
    //                new ParamWithId {Id = (int)EN.TraceType.MaxHold, Parameter = "MAXH" },
    //                new ParamWithId {Id = (int)EN.TraceType.MinHold, Parameter = "MINH" },
    //                new ParamWithId {Id = (int)EN.TraceType.Blank, Parameter = "NONE" }
    //                #endregion
    //            },
    //            TraceDetector = new List<ParamWithId>
    //            {
    //                #region
    //                new ParamWithId {Id = (int)EN.TraceDetector.AutoSelect, Parameter = "AUTO" },
    //                new ParamWithId {Id = (int)EN.TraceDetector.Average, Parameter = "AVER" },
    //                new ParamWithId {Id = (int)EN.TraceDetector.MaxPeak, Parameter = "POS" },
    //                new ParamWithId {Id = (int)EN.TraceDetector.MinPeak, Parameter = "NEG" },
    //                new ParamWithId {Id = (int)EN.TraceDetector.Sample, Parameter = "SAMP" },
    //                new ParamWithId {Id = (int)EN.TraceDetector.Normal, Parameter = "NORMal" }
    //                #endregion
    //            },
    //            SweepType =  new List<ParamWithId>
    //            {
    //                #region
    //                new ParamWithId {Id = (int)EN.SweepType.Fast, Parameter = "FAST" },
    //                new ParamWithId {Id = (int)EN.SweepType.Performance, Parameter = "PERF" },
    //                new ParamWithId {Id = (int)EN.SweepType.NoFFT, Parameter = "NOFF" }
    //                #endregion
    //            },
    //            LevelUnits = new List<ParamWithId>
    //            {
    //                #region
    //                new ParamWithId {Id = (int)MEN.LevelUnit.dBm, Parameter = "dBm" },
    //                new ParamWithId {Id = (int)MEN.LevelUnit.dBmV, Parameter = "dBmV" },
    //                new ParamWithId {Id = (int)MEN.LevelUnit.dBµV, Parameter = "dBuV" },
    //                #endregion
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

    //    #region Device info
    //    public decimal FreqMin = 9000;
    //    public decimal FreqMax = 6400000000;

    //    /// <summary>
    //    /// температура приемника от нее пляшем с калибровкой
    //    /// </summary>
    //    public double Device_BoardTemp
    //    {
    //        get { return _Device_BoardTemp; }
    //        set { _Device_BoardTemp = value; }
    //    }
    //    private double _Device_BoardTemp = 10000;
    //    /// <summary>
    //    /// последняя температура калибровки
    //    /// </summary>
    //    private double Device_LastCalcBoardTemp = 10000;

    //    public double Device_USBVoltage
    //    {
    //        get { return _Device_USBVoltage; }
    //        set { _Device_USBVoltage = value; }
    //    }
    //    private double _Device_USBVoltage = 10000;

    //    public double Device_USBCurrent
    //    {
    //        get { return _Device_USBCurrent; }
    //        set { _Device_USBCurrent = value; }
    //    }
    //    private double _Device_USBCurrent = 10000;

    //    public int Device_ID
    //    {
    //        get { return _Device_ID; }
    //        set { _Device_ID = value; }
    //    }
    //    private int _Device_ID = -1;

    //    public string Device_Type
    //    {
    //        get { return _Device_Type; }
    //        set { _Device_Type = value; }
    //    }
    //    private string _Device_Type = "";

    //    public string Device_SerialNumber
    //    {
    //        get { return _Device_SerialNumber; }
    //        set { _Device_SerialNumber = value; }
    //    }
    //    private string _Device_SerialNumber = "";

    //    public string Device_FirmwareVersion
    //    {
    //        get { return _Device_FirmwareVersion; }
    //        set { _Device_FirmwareVersion = value; }
    //    }
    //    private string _Device_FirmwareVersion = "";

    //    public string Device_APIVersion
    //    {
    //        get { return _Device_APIVersion; }
    //        set { _Device_APIVersion = value; }
    //    }
    //    private string _Device_APIVersion = "";


    //    #endregion
    //    #endregion Param

    //    #region Private Method
    //    //private void GetSystemInfo()
    //    //{
    //    //    try
    //    //    {
    //    //        float temp = 0.0F, voltage = 0.0F, current = 0.0F;
    //    //        Status = AdapterDriver.bbGetDeviceDiagnostics(_Device_ID, ref temp, ref voltage, ref current);
    //    //    }
    //    //    #region Exception
    //    //    catch (Exception exp)
    //    //    {
    //    //        _logger.Exception(Contexts.ThisComponent, exp);
    //    //    }
    //    //    #endregion
    //    //}
    //    //private void SetTraceDetectorAndScale()
    //    //{
    //    //    try
    //    //    {
    //    //        Status = AdapterDriver.bbConfigureAcquisition(_Device_ID, (uint)Detector, (uint)Scale);
    //    //    }
    //    //    #region Exception
    //    //    catch (Exception exp)
    //    //    {
    //    //        _logger.Exception(Contexts.ThisComponent, exp);
    //    //    }
    //    //    #endregion
    //    //}
    //    //private void SetFreqCentrSpan()
    //    //{
    //    //    try
    //    //    {
    //    //        Status = AdapterDriver.bbConfigureCenterSpan(_Device_ID, (double)FreqCentr, (double)FreqSpan);
    //    //    }
    //    //    #region Exception
    //    //    catch (Exception exp)
    //    //    {
    //    //        _logger.Exception(Contexts.ThisComponent, exp);
    //    //    }
    //    //    #endregion
    //    //}
    //    //private void SetRefATT()
    //    //{
    //    //    try
    //    //    {
    //    //        Status = AdapterDriver.bbConfigureLevel(_Device_ID, (double)RefLevel, (double)Attenuator);
    //    //    }
    //    //    #region Exception
    //    //    catch (Exception exp)
    //    //    {
    //    //        _logger.Exception(Contexts.ThisComponent, exp);
    //    //    }
    //    //    #endregion
    //    //}
    //    //private void SetGain()
    //    //{
    //    //    try
    //    //    {
    //    //        Status = AdapterDriver.bbConfigureGain(_Device_ID, (int)Gain);
    //    //    }
    //    //    #region Exception
    //    //    catch (Exception exp)
    //    //    {
    //    //        _logger.Exception(Contexts.ThisComponent, exp);
    //    //    }
    //    //    #endregion
    //    //}
    //    //private void SetRbwVbwSweepTimeRbwType()
    //    //{
    //    //    try
    //    //    {
    //    //        Status = AdapterDriver.bbConfigureSweepCoupling(_Device_ID, (double)RBW, (double)VBW, (double)SweepTime, (uint)RBWShape, (uint)Rejection);

    //    //    }
    //    //    #region Exception
    //    //    catch (Exception exp)
    //    //    {
    //    //        _logger.Exception(Contexts.ThisComponent, exp);
    //    //    }
    //    //    #endregion
    //    //}
    //    //private void SetVideoUnits()
    //    //{
    //    //    try
    //    //    {
    //    //        Status = AdapterDriver.bbConfigureProcUnits(_Device_ID, (uint)VideoUnit);
    //    //    }
    //    //    #region Exception
    //    //    catch (Exception exp)
    //    //    {
    //    //        _logger.Exception(Contexts.ThisComponent, exp);
    //    //    }
    //    //    #endregion
    //    //}
    //    //private void SetOverLoad(EN.Status value)
    //    //{
    //    //    if (value == EN.Status.ADCOverflow) { RFOverload = 1; }
    //    //    else { RFOverload = 0; }
    //    //}

    //    //private bool GetTrace()
    //    //{
    //    //    bool res = true;
    //    //    //получаем спектр
    //    //    uint trace_len = 0;
    //    //    double bin_size = 0.0;
    //    //    double start_freq = 0.0;
    //    //    Status = AdapterDriver.bbQueryTraceInfo(_Device_ID, ref trace_len, ref bin_size, ref start_freq);
    //    //    SetOverLoad(Status);

    //    //    FreqStep = (decimal)bin_size;

    //    //    if (Status != EN.Status.DeviceConnectionErr ||
    //    //        Status != EN.Status.DeviceInvalidErr ||
    //    //        Status != EN.Status.DeviceNotOpenErr ||
    //    //        Status != EN.Status.USBTimeoutErr)
    //    //    {
    //    //        IsRuning = true;
    //    //    }

    //    //    float[] sweep_max, sweep_min;
    //    //    sweep_max = new float[trace_len];
    //    //    sweep_min = new float[trace_len];

    //    //    Status = AdapterDriver.bbFetchTrace_32f(_Device_ID, unchecked((int)trace_len), sweep_min, sweep_max);
    //    //    if (Status == EN.Status.DeviceConnectionErr)
    //    //    {
    //    //        res = false;
    //    //    }
    //    //    else
    //    //    {
    //    //        SetOverLoad(Status);
    //    //        SetTraceData((int)trace_len, sweep_min, sweep_max, (decimal)start_freq, (decimal)bin_size);
    //    //        LastUpdate = DateTime.Now.Ticks;
    //    //    }

    //    //    return res;
    //    //}
    //    //private void SetTraceData(int newLength, float[] mintrace, float[] maxtrace, decimal freqStart, decimal step)
    //    //{
    //    //    if (maxtrace.Length > 0 && newLength > 0 && step > 0)
    //    //    {
    //    //        if (TracePoints != newLength || (Math.Abs(TraceFreqStart - (decimal)freqStart) >= (decimal)step))
    //    //        {
    //    //            TraceFreqStart = freqStart;
    //    //            TracePoints = newLength;
    //    //            FreqArr = new double[newLength];
    //    //            LevelArr = new float[newLength];
    //    //            for (int i = 0; i < newLength; i++)
    //    //            {
    //    //                FreqArr[i] = (double)(freqStart + step * i);
    //    //                LevelArr[i] = maxtrace[i];
    //    //            }
    //    //            TraceFreqStop = FreqArr[FreqArr.Length - 1];
    //    //        }
    //    //        if (TraceType == EN.TraceType.ClearWrite)
    //    //        {
    //    //            for (int i = 0; i < newLength; i++)
    //    //            {
    //    //                LevelArr[i] = maxtrace[i];
    //    //            }
    //    //        }
    //    //        //else if (TraceType == )//Average
    //    //        //{
    //    //        //    TracePoint[] temp = new TracePoint[newLength];
    //    //        //    for (int i = 0; i < newLength; i++)
    //    //        //    {
    //    //        //        decimal freq = freqStart + step * i;
    //    //        //        temp[i] = new TracePoint() { Freq = freq, Level = maxtrace[i] };
    //    //        //    }
    //    //        //}
    //    //        else if (TraceType == EN.TraceType.MaxHold)
    //    //        {
    //    //            for (int i = 0; i < newLength; i++)
    //    //            {
    //    //                if (maxtrace[i] > LevelArr[i]) LevelArr[i] = maxtrace[i];
    //    //            }
    //    //        }
    //    //        else if (TraceType == EN.TraceType.MinHold)
    //    //        {
    //    //            for (int i = 0; i < newLength; i++)
    //    //            {
    //    //                if (maxtrace[i] < LevelArr[i]) LevelArr[i] = maxtrace[i];
    //    //            }
    //    //        }

    //    //        NewTrace = true;
    //    //    }
    //    //}
    //    #endregion Private Method



    //}
}
