//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//using AC = Atdi.Common;
//using Atdi.Contracts.Sdrn.DeviceServer;
//using Atdi.DataModels.Sdrn.DeviceServer;
//using Atdi.DataModels.Sdrn.DeviceServer.Adapters;
//using Atdi.Platform.Logging;


//using CFG = Atdi.DataModels.Sdrn.DeviceServer.Adapters.Config;
//using COM = Atdi.DataModels.Sdrn.DeviceServer.Commands;
//using COMR = Atdi.DataModels.Sdrn.DeviceServer.Commands.Results;
//using System.Threading;

//namespace Atdi.AppUnits.Sdrn.DeviceServer.Adapters.RSReceiver
//{
//    public class Adapter
//    {
//        private readonly ITimeService _timeService;
//        private readonly ILogger _logger;
//        private readonly AdapterConfig _adapterConfig;
//        private readonly IWorkScheduler _workScheduler;
//        private LocalParametersConverter LPC;
//        private CFG.ThisAdapterConfig TAC;
//        private CFG.AdapterMainConfig MainConfig;

//        /// <summary>
//        /// Все объекты адаптера создаются через DI-контейнер 
//        /// Запрашиваем через конструктор необходимые сервисы
//        /// </summary>
//        /// <param name="adapterConfig"></param>
//        /// <param name="logger"></param>
//        /// <param name="timeService"></param>
//        public Adapter(AdapterConfig adapterConfig, ILogger logger, ITimeService timeService, IWorkScheduler workScheduler)
//        {
//            this._logger = logger;
//            this._adapterConfig = adapterConfig;
//            this._timeService = timeService;
//            this._workScheduler = workScheduler;
//            LPC = new LocalParametersConverter();

//        }

//        /// <summary>
//        /// Метод будет вызван при инициализации потока воркера адаптера
//        /// Адаптеру необходимо зарегестрировать свои обработчики комманд 
//        /// </summary>
//        /// <param name="host"></param>
//        public void Connect(IAdapterHost host)
//        {
//            try
//            {
//                /// включем устройство
//                /// иницируем его параметрами сконфигурации
//                /// проверяем к чем оно готово

//                if (Connect(_adapterConfig.IPAddress, _adapterConfig.Port))
//                {
//                    string filename = new Atdi.DataModels.Sdrn.DeviceServer.Adapters.InstrManufacrures().RuS.UI + "_" + SerialNumber + ".xml";
//                    TAC = new CFG.ThisAdapterConfig() { };
//                    if (!TAC.GetThisAdapterConfig(filename))
//                    {
//                        MainConfig = new CFG.AdapterMainConfig() { };
//                        SetDefaulConfig(ref MainConfig);
//                        TAC.SetThisAdapterConfig(MainConfig, filename);
//                    }
//                    else
//                    {
//                        MainConfig = TAC.Main;
//                    }
//                    (MesureTraceDeviceProperties mtdp, MesureIQStreamDeviceProperties midp) = GetProperties(MainConfig);
//                    host.RegisterHandler<COM.MesureTraceCommand, COMR.MesureTraceResult>(MesureSystemInfoHandler, mtdp);
//                }
//            }
//            #region Exception
//            catch (Exception exp)
//            {
//                _logger.Exception(Contexts.ThisComponent, exp);
//                throw new InvalidOperationException("Invalid initialize/connect adapter", exp);
//            }
//            #endregion
//        }

//        /// <summary>
//        /// Метод вызывается контрллером когда необходимо выгрузит адаптер с памяти
//        /// </summary>
//        public void Disconnect()
//        {
//            try
//            {
//                /// освобождаем ресурсы и отключаем устройство
//                //TAC.Save();

//            }
//            #region Exception
//            catch (Exception exp)
//            {
//                _logger.Exception(Contexts.ThisComponent, exp);
//            }
//            #endregion
//        }

//        public void MesureTraceCommandHandler(COM.MesureTraceCommand command, IExecutionContext context)
//        {
//            try
//            {
//                if (IsRuning)
//                {
//                    /// если нужно проводим блокировку комманд который мы не сможем обслужить пока что то меряем в течении выполнени яэтой комманды
//                    /// и подсказывая этим инфраструктуре что этот устрйостов некоторое время не сможет обрабатываить такие комманды
//                    //context.Lock(CommandType.MesureTrace);

//                    // если нужно заблокировать выполняему комманду то достатчоно вызвать метод без параметров и блокируется комманда которая выполняется
//                    context.Lock();

//                    //Найдем в каком режиме работать
//                    if (command.Parameter.FreqStop_Hz - command.Parameter.FreqStart_Hz > UniqueData.FFMSpan[UniqueData.FFMSpan.Length - 1].BW)
//                    {
//                        //хотим PSCAN
//                        if (UniqueData.LoadedInstrOption.Exists(x => x.Type == "PS"))//есть
//                        {
//                            if (PScanFreqStart != command.Parameter.FreqStart_Hz)
//                            {
//                                PScanFreqStart = LPC.PScanFreqStart(UniqueData, command.Parameter.FreqStart_Hz);
//                                SetFreqStart(FreqStart);
//                            }
//                            if (PScanFreqStop != command.Parameter.FreqStop_Hz)
//                            {
//                                PScanFreqStop = LPC.PScanFreqStop(UniqueData, command.Parameter.FreqStop_Hz);
//                                SetFreqStop(FreqStop);
//                            }
//                        }
//                    }
//                    else
//                    {
//                        //хотим FFM
//                        (FFMFreqCentrToSet, FFMFreqSpan) = LPC.FFMFreqCentrSpan(UniqueData, command.Parameter.FreqStart_Hz, command.Parameter.FreqStop_Hz);
//                        SetFFMFreqCentr(FFMFreqCentrToSet);
//                    }


//                    //Установим опорный уровень
//                    if (RefLevel != command.Parameter.RefLevel_dBm)
//                    {
//                        //типа авто...
//                        if (command.Parameter.RefLevel_dBm == 1000000000)
//                        {
//                            RefLevel = LPC.RefLevel(UniqueData, -20);
//                        }
//                        else
//                        {
//                            RefLevel = LPC.RefLevel(UniqueData, command.Parameter.RefLevel_dBm);
//                        }
//                        SetRefLevel(RefLevel);
//                    }

//                    //Установим предусилитель при наличии
//                    if (UniqueData.PreAmp)//доступен ли вообще предусилитель
//                    {
//                        bool preamp = LPC.PreAmp(UniqueData, command.Parameter.PreAmp_dB);
//                        if (preamp != PreAmp)
//                        {
//                            PreAmp = preamp;
//                            SetPreAmp(PreAmp);
//                        }
//                    }

//                    //Установим аттенюатор
//                    (decimal att, bool auto) = LPC.Attenuator(UniqueData, command.Parameter.Att_dB);
//                    if (auto)
//                    {
//                        SetAutoAtt(auto);
//                    }
//                    else
//                    {
//                        if (AttLevel != att)
//                        {
//                            AttLevel = att;
//                            SetAttLevel(AttLevel);
//                        }
//                    }





//                    //кол-во точек посчитать по RBW
//                    if (command.Parameter.TracePoint == -1 && command.Parameter.RBW_Hz > 0)
//                    {
//                        //RBW ближайшее меньшее
//                        RBW = LPC.RBW(UniqueData, (decimal)command.Parameter.RBW_Hz);
//                        SetRBW(RBW);
//                        if (command.Parameter.VBW_Hz < 0) //Если авто то VBW = RBW
//                        {
//                            VBW = LPC.VBW(UniqueData, RBW);// но проверим на доступность
//                            SetVBW(VBW);
//                        }
//                        else
//                        {
//                            VBW = LPC.VBW(UniqueData, (decimal)command.Parameter.VBW_Hz);
//                            SetVBW(VBW);
//                        }

//                        int needsweeppints = (int)(FreqSpan / RBW);
//                        int sweeppoints = LPC.SweepPoints(UniqueData, needsweeppints);
//                        SetSweepPoints(sweeppoints);
//                    }
//                    //сколько точек понятно по нему и посчитать RBW
//                    else if (command.Parameter.TracePoint > 0 && command.Parameter.RBW_Hz == -1)
//                    {
//                        int sweeppoints = LPC.SweepPoints(UniqueData, command.Parameter.TracePoint);
//                        SetSweepPoints(sweeppoints);

//                        decimal needrbw = LPC.RBW(UniqueData, FreqSpan / ((decimal)(sweeppoints - 1)));
//                        SetRBW(needrbw);
//                        if (command.Parameter.VBW_Hz < 0) //Если авто то VBW = RBW
//                        {
//                            VBW = LPC.VBW(UniqueData, RBW);// но проверим на доступность
//                            SetVBW(VBW);
//                        }
//                        else
//                        {
//                            VBW = LPC.VBW(UniqueData, (decimal)command.Parameter.VBW_Hz);
//                            SetVBW(VBW);
//                        }
//                    }
//                    //сколько точек понятно по нему и посчитать RBW
//                    else if (command.Parameter.TracePoint > 0 && command.Parameter.RBW_Hz > 0)
//                    {
//                        int sweeppoints = LPC.SweepPoints(UniqueData, command.Parameter.TracePoint);
//                        SetSweepPoints(sweeppoints);

//                        RBW = LPC.RBW(UniqueData, (decimal)command.Parameter.RBW_Hz);
//                        SetRBW(RBW);
//                        if (command.Parameter.VBW_Hz < 0) //Если авто то VBW = RBW
//                        {
//                            VBW = LPC.VBW(UniqueData, RBW);// но проверим на доступность
//                            SetVBW(VBW);
//                        }
//                        else
//                        {
//                            VBW = LPC.VBW(UniqueData, (decimal)command.Parameter.VBW_Hz);
//                            SetVBW(VBW);
//                        }
//                    }

//                    #region DetectorType
//                    ParamWithId DetId = LPC.DetectorType(UniqueData, command.Parameter.DetectorType);
//                    if (Trace1Detector != DetId)
//                    {
//                        SetDetectorType(DetId);
//                    }
//                    #endregion DetectorType

//                    #region TraceType
//                    (ParamWithId TTId, EN.TraceType res) = LPC.TraceType(UniqueData, command.Parameter.TraceType);
//                    //на прибор
//                    if (Trace1Type != TTId)
//                    {
//                        SetTraceType(TTId);
//                    }
//                    //Такой результат
//                    if (TraceTypeResult != res)
//                    {
//                        TraceTypeResult = res;
//                    }
//                    #endregion TraceType

//                    #region LevelUnit
//                    (ParamWithId lu, MEN.LevelUnit lur) = LPC.LevelUnit(UniqueData, command.Parameter.LevelUnit);
//                    if (lur != LevelUnitsResult || LevelUnits != lu)
//                    {
//                        LevelUnitsResult = lur;
//                        SetLevelUnit(lu);
//                    }
//                    //для ускорения усреднения установим levelunit в Ваты 
//                    if (TraceTypeResult == EN.TraceType.Average)
//                    {
//                        for (int i = 0; i < UniqueData.LevelUnits.Count; i++)
//                        {
//                            if (UniqueData.LevelUnits[i].Id == (int)MEN.LevelUnit.Watt)
//                            {
//                                SetLevelUnit(UniqueData.LevelUnits[i]);
//                            }
//                        }
//                    }
//                    #endregion LevelUnit

//                    #region SweepTime
//                    //Если установленно в настройках адаптера OnlyAutoSweepTime
//                    if (_adapterConfig.OnlyAutoSweepTime)
//                    {
//                        SetAutoSweepTime(_adapterConfig.OnlyAutoSweepTime);
//                    }
//                    //Если SweepTime можно установить
//                    else
//                    {
//                        (decimal swt, bool autoswt) = LPC.SweepTime(UniqueData, (decimal)command.Parameter.SweepTime_s);
//                        if (autoswt)
//                        {
//                            SetAutoSweepTime(_adapterConfig.OnlyAutoSweepTime);
//                        }
//                        else
//                        {
//                            SetSweepTime(swt);
//                        }
//                    }
//                    #endregion SweepTime

//                    #region Костыль от некоректных данных спектра от прошлого измерения
//                    if (UniqueData.InstrManufacture == 1)
//                    {
//                        if (UniqueData.InstrModel.Contains("FPL"))
//                        {
//                            //Thread.Sleep((int)(SweepTime * 4000 + 20));
//                        }
//                        else
//                        {
//                            Thread.Sleep((int)(SweepTime * 4000 + 10));
//                        }
//                    }
//                    else if (UniqueData.InstrManufacture == 2)
//                    {
//                        Thread.Sleep((int)(SweepTime * 4000 + 30));
//                    }
//                    else if (UniqueData.InstrManufacture == 3)
//                    {
//                        Thread.Sleep((int)(SweepTime * 4000 + 30));
//                    }
//                    #endregion


//                    //Устанавливаем сколько трейсов хотим
//                    if (command.Parameter.TraceCount > 0)
//                    {
//                        TraceCountToMeas = (ulong)command.Parameter.TraceCount;
//                        TraceCount = 0;
//                        TracePoints = command.Parameter.TracePoint;
//                    }
//                    else
//                    {
//                        throw new Exception("TraceCount must be set greater than zero.");
//                    }

//                    //если надо изменяем размер буфера
//                    int length = SweepPoints * 4 + SweepPoints.ToString().Length + 100;
//                    if (session.DefaultBufferSize != length)
//                    {
//                        session.DefaultBufferSize = length;
//                    }

//                    //Меряем
//                    //Если TraceType ClearWrite то пушаем каждый результат                    
//                    if (TraceTypeResult == EN.TraceType.ClearWrite)
//                    {
//                        bool newres = false;

//                        for (ulong i = 0; i < TraceCountToMeas; i++)
//                        {
//                            newres = GetTrace();
//                            if (newres)
//                            {
//                                // пушаем результат
//                                var result = new COMR.MesureTraceResult(TraceCount, CommandResultStatus.Next);
//                                TraceCount++;
//                                if (TraceCountToMeas == TraceCount)
//                                {
//                                    result = new COMR.MesureTraceResult(TraceCount, CommandResultStatus.Final);
//                                }
//                                result.Att_dB = (int)AttLevel;
//                                result.RefLevel_dBm = (int)RefLevel;
//                                result.PreAmp_dB = PreAmp ? 1 : 0;
//                                result.RBW_Hz = (double)RBW;
//                                result.VBW_Hz = (double)VBW;
//                                result.Freq_Hz = new double[FreqArr.Length];
//                                result.Level = new float[FreqArr.Length];
//                                for (int j = 0; j < FreqArr.Length; j++)
//                                {
//                                    result.Freq_Hz[j] = FreqArr[j];
//                                    result.Level[j] = LevelArr[j];
//                                }
//                                //result.TimeStamp = _timeService.TimeStamp.Ticks - new DateTime(1970, 1, 1, 0, 0, 0, System.DateTimeKind.Utc).Ticks;//неюзабельно
//                                result.TimeStamp = DateTime.UtcNow.Ticks - new DateTime(1970, 1, 1, 0, 0, 0, System.DateTimeKind.Utc).Ticks;

//                                context.PushResult(result);
//                            }
//                            else
//                            {
//                                i--;
//                            }
//                            // иногда нужно проверять токен окончания работы комманды
//                            if (context.Token.IsCancellationRequested)
//                            {
//                                // все нужно остановиться

//                                // если есть порция данных возвращаем ее в обработчки только говрим что поток результатов не законченный и больше уже не будет поступать
//                                var result2 = new COMR.MesureTraceResult(TraceCount, CommandResultStatus.Ragged);

//                                context.PushResult(result2);


//                                // подтверждаем факт обработки отмены
//                                context.Cancel();
//                                // освобождаем поток 
//                                return;
//                            }
//                        }

//                    }
//                    //Если TraceType Average/MinHold/MaxHold то делаем измерений сколько сказали и пушаем только готовый результат
//                    else
//                    {
//                        TraceReset = true;///сбросим предыдущие результаты
//                        if (TraceTypeResult == EN.TraceType.Average)//назначим сколько усреднять
//                        {
//                            TraceAveraged.AveragingCount = (int)TraceCountToMeas;
//                        }
//                        bool newres = false;
//                        for (ulong i = 0; i < TraceCountToMeas; i++)
//                        {
//                            newres = GetTrace();
//                            if (newres)
//                            {
//                                TraceCount++;
//                            }
//                            else
//                            {
//                                i--;
//                            }
//                            // иногда нужно проверять токен окончания работы комманды
//                            if (context.Token.IsCancellationRequested)
//                            {
//                                // все нужно остановиться

//                                // если есть порция данных возвращаем ее в обработчки только говрим что поток результатов не законченный и больше уже не будет поступать

//                                var result2 = new COMR.MesureTraceResult(TraceCount, CommandResultStatus.Ragged);
//                                //Скорее нет результатов
//                                context.PushResult(result2);

//                                // подтверждаем факт обработки отмены
//                                context.Cancel();
//                                // освобождаем поток 
//                                return;
//                            }
//                        }
//                        if (TraceCountToMeas == TraceCount)
//                        {
//                            var result = new COMR.MesureTraceResult(0, CommandResultStatus.Final)
//                            {
//                                Freq_Hz = new double[FreqArr.Length],
//                                Level = new float[FreqArr.Length]
//                            };
//                            for (int j = 0; j < FreqArr.Length; j++)
//                            {
//                                result.Freq_Hz[j] = FreqArr[j];
//                                result.Level[j] = LevelArr[j];
//                            }
//                            result.Att_dB = (int)AttLevel;
//                            result.RefLevel_dBm = (int)RefLevel;
//                            result.PreAmp_dB = PreAmp ? 1 : 0;
//                            result.RBW_Hz = (double)RBW;
//                            result.VBW_Hz = (double)VBW;
//                            //result.TimeStamp = _timeService.TimeStamp.Ticks - new DateTime(1970, 1, 1, 0, 0, 0, System.DateTimeKind.Utc).Ticks;//неюзабельно
//                            result.TimeStamp = DateTime.UtcNow.Ticks - new DateTime(1970, 1, 1, 0, 0, 0, System.DateTimeKind.Utc).Ticks;

//                            context.PushResult(result);
//                        }
//                    }

//                    context.Unlock();

//                    // что то делаем еще 


//                    // подтверждаем окончание выполнения комманды 
//                    // важно: всн ранее устапнволеные в контексте обработки текущей команыд блокировки снимаются автоматически
//                    context.Finish();
//                    // дальше кода быть не должно, освобождаем поток
//                }
//                else
//                {
//                    throw new Exception("The device with serial number " + UniqueData.SerialNumber + " does not work");
//                }
//            }
//            catch (Exception e)
//            {
//                // желательно записать влог
//                _logger.Exception(Contexts.ThisComponent, e);
//                // этот вызов обязательный в случаи обрыва
//                context.Abort(e);
//                // дальше кода быть не должно, освобождаем поток
//            }

//        }



//        #region
//        CommandConnection tc = null;
//        UdpStreaming uc = null;
//        private delegate void DoubMod();
//        private DoubMod UdpDM;
//        public Thread UdpTr;

//        private string IPAddress;
//        private string MyIPAddress = "";
//        private int TCPPort;
//        private int UDPPort = 23023;

//        private int Manufacrure = 0;
//        private string Model = "";
//        private string SerialNumber = "";
//        private LocalRSReceiverInfo UniqueData = new LocalRSReceiverInfo { };

//        private List<LocalRSReceiverInfo> AllUniqueData = new List<LocalRSReceiverInfo>
//        {
//            #region EM100 
//            new LocalRSReceiverInfo
//            {
//                InstrManufacrure = 1, InstrModel = "EM100",
//                EB200Protocol = true,
//                XMLProtocol = false,
//                InstrOption = new List<DeviceOption>()
//                {
//                    new DeviceOption() { Type = "PS", Designation = "Panorama Scan", GlobalType = "Panorama Scan"},
//                    new DeviceOption() { Type = "IR", Designation = "Internal Recording", GlobalType = "Internal Recording"},
//                    new DeviceOption() { Type = "RC", Designation = "Remote Control", GlobalType = "Remote Control"},
//                    new DeviceOption() { Type = "ET", Designation = "External Triggered Measurement", GlobalType = "External Triggered Measurement"},
//                    new DeviceOption() { Type = "FS", Designation = "Fieldstrength Measurement", GlobalType = "Fieldstrength Measurement"},
//                    new DeviceOption() { Type = "FP", Designation = "Frequency Processing SHF", GlobalType = "Frequency Processing SHF"},
//                    new DeviceOption() { Type = "GP", Designation = "GPS Compass", GlobalType = "GPS Compass"},
//                    new DeviceOption() { Type = "FE", Designation = "Frequency Extension", GlobalType = "Frequency Extension"},
//                    new DeviceOption() { Type = "DF", Designation = "Direction Finder", GlobalType = "Direction Finder"},
//                },
//                LoadedInstrOption = new List<DeviceOption>() { },
//                Modes = new List<LocalRSReceiverInfo.Mode>
//                {
//                    new LocalRSReceiverInfo.Mode {ModeName = "FFM", MeasAppl = "RX", FreqMode = "CW"},
//                    new LocalRSReceiverInfo.Mode {ModeName = "DF", MeasAppl = "DF", FreqMode = "DF"},
//                    new LocalRSReceiverInfo.Mode {ModeName = "PSCAN", MeasAppl = "RX", FreqMode = "PSC"},
//                },
//                DemodFreq = false,
//                DemodBW = new decimal[] { 150, 300, 600, 1500, 2400, 6000, 9000, 12000, 15000, 30000, 50000, 120000, 150000, 250000, 300000, 500000 },
//                Demod =  new string[] { "FM", "AM", "PULSE", "CW", "USB", "LSB", "IQ", "ISB", "PM" },
//                AllStepBW = new decimal[] { 0.625m, 1.25m, 3.125m, 6.25m, 12.5m, 31.25m, 62.5m, 125, 312.5m, 625, 1250, 3125, 6250},
//                DFBW = new decimal[] { 50, 100, 150, 300, 600, 1000, 1500, 2100, 2400, 2700, 3100, 4000, 4800, 6000, 8333, 9000, 12000, 15000, 25000, 30000, 50000, 75000, 120000, 150000, 250000, 300000, 500000, 800000, 1000000, 1250000, 1500000, 2000000 },
//                PSCANStepBW = new decimal[] { 125, 250, 500, 625, 1250, 2500, 3125, 6250, 12500m, 25000m, 50000, 100000},
//                FFMSpan = new LocalRSReceiverInfo.FFMSpanBW[]
//                {
//                    new LocalRSReceiverInfo.FFMSpanBW(){ BW = 1000, AvailableStepBW = new decimal[]{ 0.625m} },
//                    new LocalRSReceiverInfo.FFMSpanBW(){ BW = 2000, AvailableStepBW = new decimal[]{ 1.25m } },
//                    new LocalRSReceiverInfo.FFMSpanBW(){ BW = 5000, AvailableStepBW = new decimal[]{ 3.125m} },
//                    new LocalRSReceiverInfo.FFMSpanBW(){ BW = 10000, AvailableStepBW = new decimal[]{ 6.25m} },
//                    new LocalRSReceiverInfo.FFMSpanBW(){ BW = 20000, AvailableStepBW = new decimal[]{ 12.5m } },
//                    new LocalRSReceiverInfo.FFMSpanBW(){ BW = 50000, AvailableStepBW = new decimal[]{ 31.25m } },
//                    new LocalRSReceiverInfo.FFMSpanBW(){ BW = 100000, AvailableStepBW = new decimal[]{ 62.5m } },
//                    new LocalRSReceiverInfo.FFMSpanBW(){ BW = 200000, AvailableStepBW = new decimal[]{ 125 } },
//                    new LocalRSReceiverInfo.FFMSpanBW(){ BW = 500000, AvailableStepBW = new decimal[]{ 312.5m } },
//                    new LocalRSReceiverInfo.FFMSpanBW(){ BW = 1000000, AvailableStepBW = new decimal[]{ 625 } },
//                    new LocalRSReceiverInfo.FFMSpanBW(){ BW = 2000000, AvailableStepBW = new decimal[]{ 1250} },
//                    new LocalRSReceiverInfo.FFMSpanBW(){ BW = 5000000, AvailableStepBW = new decimal[]{ 3125 } },
//                    new LocalRSReceiverInfo.FFMSpanBW(){ BW = 10000000, AvailableStepBW = new decimal[]{ 6250 } },
//                },
//                FFTModes = new List<ParamWithId>
//                {
//                    new ParamWithId() {Id = (int)Enums.FFTMode.Minimum, Parameter = "MIN"},
//                    new ParamWithId() {Id = (int)Enums.FFTMode.Maximum, Parameter = "MAX"},
//                    new ParamWithId() {Id = (int)Enums.FFTMode.Average, Parameter = "SCAL"},
//                    new ParamWithId() {Id = (int)Enums.FFTMode.ClearWrite, Parameter = "OFF"},
//                },
//                SelectivityChangeable = false,
//                SelectivityModes = new List<ParamWithId>()
//                {
//                    new ParamWithId() {Id = (int)Enums.SelectivityMode.Auto, Parameter = "AUTO"},
//                    new ParamWithId() {Id = (int)Enums.SelectivityMode.Normal, Parameter = "NORM"},
//                    new ParamWithId() {Id = (int)Enums.SelectivityMode.Narrow, Parameter = "NARR"},
//                    new ParamWithId() {Id = (int)Enums.SelectivityMode.Sharp, Parameter = "SHAR"},
//                },
//                Detectors = new List<ParamWithId>()
//                {
//                    new ParamWithId() {Id = (int)Enums.Detector.Average, Parameter = "PAV"},
//                    new ParamWithId() {Id = (int)Enums.Detector.Peak, Parameter = "POS"},
//                    new ParamWithId() {Id = (int)Enums.Detector.Fast, Parameter = "FAST"},
//                    new ParamWithId() {Id = (int)Enums.Detector.RMS, Parameter = "RMS"},
//                },
//                RFModeChangeable = false,
//                RFModes= new List<ParamWithId>(){ },
//                ATTFix = true,
//                AttMax = 10,
//                AttStep = 10,

//                RefLevelMAX = 110,
//                RefLevelMIN = -29,
//                RangeLevelMAX = 140,
//                RangeLevelMIN = 10,
//                RangeLevelStep = 10,
//                DFSQUModes = new List<ParamWithId>()
//                {
//                    new ParamWithId() {Id = (int)Enums.DFSQUMode.Off, Parameter = "OFF"},
//                    new ParamWithId() {Id = (int)Enums.DFSQUMode.Gate, Parameter = "GATE"},
//                    new ParamWithId() {Id = (int)Enums.DFSQUMode.Normal, Parameter = "NORM"},
//                },
//                MeasTimeMAX = 0.0005M,
//                MeasTimeMIN = 900,
//            },
//            #endregion
//            #region PR100
//            new LocalRSReceiverInfo
//            {
//                InstrManufacrure = 1, InstrModel = "PR100",
//                EB200Protocol = true,
//                XMLProtocol = false,
//                InstrOption = new List<DeviceOption>()
//                {
//                    new DeviceOption() { Type = "PS", Designation = "Panorama Scan", GlobalType = "Panorama Scan"},
//                    new DeviceOption() { Type = "IR", Designation = "Internal Recording", GlobalType = "Internal Recording"},
//                    new DeviceOption() { Type = "RC", Designation = "Remote Control", GlobalType = "Remote Control"},
//                    new DeviceOption() { Type = "ET", Designation = "External Triggered Measurement", GlobalType = "External Triggered Measurement"},
//                    new DeviceOption() { Type = "FS", Designation = "Fieldstrength Measurement", GlobalType = "Fieldstrength Measurement"},
//                    new DeviceOption() { Type = "FP", Designation = "Frequency Processing SHF", GlobalType = "Frequency Processing SHF"},
//                    new DeviceOption() { Type = "GP", Designation = "GPS Compass", GlobalType = "GPS Compass"},
//                    new DeviceOption() { Type = "FE", Designation = "Frequency Extension", GlobalType = "Frequency Extension"},
//                    new DeviceOption() { Type = "DF", Designation = "Direction Finder", GlobalType = "Direction Finder"},
//                },
//                LoadedInstrOption = new List<DeviceOption>() { },
//                Modes = new List<LocalRSReceiverInfo.Mode>
//                {
//                    new LocalRSReceiverInfo.Mode {ModeName = "FFM", MeasAppl = "RX", FreqMode = "CW"},
//                    new LocalRSReceiverInfo.Mode {ModeName = "DF", MeasAppl = "DF", FreqMode = "DF"},
//                    new LocalRSReceiverInfo.Mode {ModeName = "PSCAN", MeasAppl = "RX", FreqMode = "PSC"},
//                },
//                DemodFreq = false,
//                DemodBW = new decimal[] { 150, 300, 600, 1500, 2400, 6000, 9000, 12000, 15000, 30000, 50000, 120000, 150000, 250000, 300000, 500000 },
//                Demod =  new string[] { "FM", "AM", "PULSE", "CW", "USB", "LSB", "IQ", "ISB", "PM" },
//                AllStepBW = new decimal[] { 0.625m, 1.25m, 3.125m, 6.25m, 12.5m, 31.25m, 62.5m, 125, 312.5m, 625, 1250, 3125, 6250},
//                DFBW = new decimal[] { 50, 100, 150, 300, 600, 1000, 1500, 2100, 2400, 2700, 3100, 4000, 4800, 6000, 8333, 9000, 12000, 15000, 25000, 30000, 50000, 75000, 120000, 150000, 250000, 300000, 500000, 800000, 1000000, 1250000, 1500000, 2000000 },
//                PSCANStepBW = new decimal[] { 125, 250, 500, 625, 1250, 2500, 3125, 6250, 12500m, 25000m, 50000, 100000},
//                FFMSpan = new LocalRSReceiverInfo.FFMSpanBW[]
//                {
//                    new LocalRSReceiverInfo.FFMSpanBW(){ BW = 1000, AvailableStepBW = new decimal[]{ 0.625m} },
//                    new LocalRSReceiverInfo.FFMSpanBW(){ BW = 2000, AvailableStepBW = new decimal[]{ 1.25m } },
//                    new LocalRSReceiverInfo.FFMSpanBW(){ BW = 5000, AvailableStepBW = new decimal[]{ 3.125m} },
//                    new LocalRSReceiverInfo.FFMSpanBW(){ BW = 10000, AvailableStepBW = new decimal[]{ 6.25m} },
//                    new LocalRSReceiverInfo.FFMSpanBW(){ BW = 20000, AvailableStepBW = new decimal[]{ 12.5m } },
//                    new LocalRSReceiverInfo.FFMSpanBW(){ BW = 50000, AvailableStepBW = new decimal[]{ 31.25m } },
//                    new LocalRSReceiverInfo.FFMSpanBW(){ BW = 100000, AvailableStepBW = new decimal[]{ 62.5m } },
//                    new LocalRSReceiverInfo.FFMSpanBW(){ BW = 200000, AvailableStepBW = new decimal[]{ 125 } },
//                    new LocalRSReceiverInfo.FFMSpanBW(){ BW = 500000, AvailableStepBW = new decimal[]{ 312.5m } },
//                    new LocalRSReceiverInfo.FFMSpanBW(){ BW = 1000000, AvailableStepBW = new decimal[]{ 625 } },
//                    new LocalRSReceiverInfo.FFMSpanBW(){ BW = 2000000, AvailableStepBW = new decimal[]{ 1250} },
//                    new LocalRSReceiverInfo.FFMSpanBW(){ BW = 5000000, AvailableStepBW = new decimal[]{ 3125 } },
//                    new LocalRSReceiverInfo.FFMSpanBW(){ BW = 10000000, AvailableStepBW = new decimal[]{ 6250 } },
//                },
//                FFTModes = new List<ParamWithId>
//                {
//                    new  ParamWithId() {Id = (int)Enums.FFTMode.Minimum, Parameter = "MIN"},
//                    new  ParamWithId() {Id = (int)Enums.FFTMode.Maximum, Parameter = "MAX"},
//                    new  ParamWithId() {Id = (int)Enums.FFTMode.Average, Parameter = "SCAL"},
//                    new  ParamWithId() {Id = (int)Enums.FFTMode.ClearWrite, Parameter = "OFF"},
//                },
//                SelectivityChangeable = false,
//                SelectivityModes = new List<ParamWithId>()
//                {
//                    new ParamWithId() {Id = (int)Enums.SelectivityMode.Auto, Parameter = "AUTO"},
//                    new ParamWithId() {Id = (int)Enums.SelectivityMode.Normal, Parameter = "NORM"},
//                    new ParamWithId() {Id = (int)Enums.SelectivityMode.Narrow, Parameter = "NARR"},
//                    new ParamWithId() {Id = (int)Enums.SelectivityMode.Sharp, Parameter = "SHAR"},
//                },
//                Detectors = new List<ParamWithId>()
//                {
//                    new ParamWithId() {Id = (int)Enums.Detector.Average, Parameter = "PAV"},
//                    new ParamWithId() {Id = (int)Enums.Detector.Peak, Parameter = "POS"},
//                    new ParamWithId() {Id = (int)Enums.Detector.Fast, Parameter = "FAST"},
//                    new ParamWithId() {Id = (int)Enums.Detector.RMS, Parameter = "RMS"},
//                },
//                RFModeChangeable = false,
//                RFModes= new List<ParamWithId>(){ },
//                ATTFix = true,
//                AttMax = 10,
//                AttStep = 10,

//                RefLevelMAX = 110,
//                RefLevelMIN = -29,
//                RangeLevelMAX = 140,
//                RangeLevelMIN = 10,
//                RangeLevelStep = 10,
//                DFSQUModes = new List<ParamWithId>()
//                {
//                    new ParamWithId() {Id = (int)Enums.DFSQUMode.Off, Parameter = "OFF"},
//                    new ParamWithId() {Id = (int)Enums.DFSQUMode.Gate, Parameter = "GATE"},
//                    new ParamWithId() {Id = (int)Enums.DFSQUMode.Normal, Parameter = "NORM"},
//                },
//                MeasTimeMAX = 0.0005M,
//                MeasTimeMIN = 900,
//            },
//            #endregion
//            #region DDF007
//            new LocalRSReceiverInfo
//            {
//                InstrManufacrure = 1, InstrModel = "DDF007",
//                EB200Protocol = true,
//                XMLProtocol = false,
//                InstrOption = new List<DeviceOption>()
//                {
//                    new DeviceOption() { Type = "PS", Designation = "Panorama Scan", GlobalType = "Panorama Scan"},
//                    new DeviceOption() { Type = "IR", Designation = "Internal Recording", GlobalType = "Internal Recording"},
//                    new DeviceOption() { Type = "RC", Designation = "Remote Control", GlobalType = "Remote Control"},
//                    new DeviceOption() { Type = "ET", Designation = "External Triggered Measurement", GlobalType = "External Triggered Measurement"},
//                    new DeviceOption() { Type = "FS", Designation = "Fieldstrength Measurement", GlobalType = "Fieldstrength Measurement"},
//                    new DeviceOption() { Type = "FP", Designation = "Frequency Processing SHF", GlobalType = "Frequency Processing SHF"},
//                    new DeviceOption() { Type = "GP", Designation = "GPS Compass", GlobalType = "GPS Compass"},
//                    new DeviceOption() { Type = "FE", Designation = "Frequency Extension", GlobalType = "Frequency Extension"},
//                    new DeviceOption() { Type = "DF", Designation = "Direction Finder", GlobalType = "Direction Finder"},
//                },
//                LoadedInstrOption = new List<DeviceOption>() { },
//                Modes = new List<LocalRSReceiverInfo.Mode>
//                {
//                    new LocalRSReceiverInfo.Mode {ModeName = "FFM", MeasAppl = "RX", FreqMode = "CW"},
//                    new LocalRSReceiverInfo.Mode {ModeName = "DF", MeasAppl = "DF", FreqMode = "DF"},
//                    new LocalRSReceiverInfo.Mode {ModeName = "PSCAN", MeasAppl = "RX", FreqMode = "PSC"},
//                },
//                DemodFreq = false,
//                DemodBW = new decimal[] { 150, 300, 600, 1500, 2400, 6000, 9000, 12000, 15000, 30000, 50000, 120000, 150000, 250000, 300000, 500000 },
//                Demod =  new string[] { "FM", "AM", "PULSE", "CW", "USB", "LSB", "IQ", "ISB", "PM" },
//                AllStepBW = new decimal[] { 0.625m, 1.25m, 3.125m, 6.25m, 12.5m, 31.25m, 62.5m, 125, 312.5m, 625, 1250, 3125, 6250},
//                DFBW = new decimal[] { 50, 100, 150, 300, 600, 1000, 1500, 2100, 2400, 2700, 3100, 4000, 4800, 6000, 8333, 9000, 12000, 15000, 25000, 30000, 50000, 75000, 120000, 150000, 250000, 300000, 500000, 800000, 1000000, 1250000, 1500000, 2000000 },
//                PSCANStepBW = new decimal[] { 125, 250, 500, 625, 1250, 2500, 3125, 6250, 12500m, 25000m, 50000, 100000},
//                FFMSpan = new LocalRSReceiverInfo.FFMSpanBW[]
//                {
//                    new LocalRSReceiverInfo.FFMSpanBW(){ BW = 1000, AvailableStepBW = new decimal[]{ 0.625m} },
//                    new LocalRSReceiverInfo.FFMSpanBW(){ BW = 2000, AvailableStepBW = new decimal[]{ 1.25m } },
//                    new LocalRSReceiverInfo.FFMSpanBW(){ BW = 5000, AvailableStepBW = new decimal[]{ 3.125m} },
//                    new LocalRSReceiverInfo.FFMSpanBW(){ BW = 10000, AvailableStepBW = new decimal[]{ 6.25m} },
//                    new LocalRSReceiverInfo.FFMSpanBW(){ BW = 20000, AvailableStepBW = new decimal[]{ 12.5m } },
//                    new LocalRSReceiverInfo.FFMSpanBW(){ BW = 50000, AvailableStepBW = new decimal[]{ 31.25m } },
//                    new LocalRSReceiverInfo.FFMSpanBW(){ BW = 100000, AvailableStepBW = new decimal[]{ 62.5m } },
//                    new LocalRSReceiverInfo.FFMSpanBW(){ BW = 200000, AvailableStepBW = new decimal[]{ 125 } },
//                    new LocalRSReceiverInfo.FFMSpanBW(){ BW = 500000, AvailableStepBW = new decimal[]{ 312.5m } },
//                    new LocalRSReceiverInfo.FFMSpanBW(){ BW = 1000000, AvailableStepBW = new decimal[]{ 625 } },
//                    new LocalRSReceiverInfo.FFMSpanBW(){ BW = 2000000, AvailableStepBW = new decimal[]{ 1250} },
//                    new LocalRSReceiverInfo.FFMSpanBW(){ BW = 5000000, AvailableStepBW = new decimal[]{ 3125 } },
//                    new LocalRSReceiverInfo.FFMSpanBW(){ BW = 10000000, AvailableStepBW = new decimal[]{ 6250 } },
//                },
//                FFTModes = new List<ParamWithId>
//                {
//                    new  ParamWithId() {Id = (int)Enums.FFTMode.Minimum, Parameter = "MIN"},
//                    new  ParamWithId() {Id = (int)Enums.FFTMode.Maximum, Parameter = "MAX"},
//                    new  ParamWithId() {Id = (int)Enums.FFTMode.Average, Parameter = "SCAL"},
//                    new  ParamWithId() {Id = (int)Enums.FFTMode.ClearWrite, Parameter = "OFF"},
//                },
//                SelectivityChangeable = false,
//                SelectivityModes = new List<ParamWithId>()
//                {
//                    new ParamWithId() {Id = (int)Enums.SelectivityMode.Auto, Parameter = "AUTO"},
//                    new ParamWithId() {Id = (int)Enums.SelectivityMode.Normal, Parameter = "NORM"},
//                    new ParamWithId() {Id = (int)Enums.SelectivityMode.Narrow, Parameter = "NARR"},
//                    new ParamWithId() {Id = (int)Enums.SelectivityMode.Sharp, Parameter = "SHAR"},
//                },
//                Detectors = new List<ParamWithId>()
//                {
//                    new ParamWithId() {Id = (int)Enums.Detector.Average, Parameter = "PAV"},
//                    new ParamWithId() {Id = (int)Enums.Detector.Peak, Parameter = "POS"},
//                    new ParamWithId() {Id = (int)Enums.Detector.Fast, Parameter = "FAST"},
//                    new ParamWithId() {Id = (int)Enums.Detector.RMS, Parameter = "RMS"},
//                },
//                RFModeChangeable = false,
//                RFModes= new List<ParamWithId>(){ },
//                ATTFix = true,
//                AttMax = 10,
//                AttStep = 10,

//                RefLevelMAX = 110,
//                RefLevelMIN = -29,
//                RangeLevelMAX = 140,
//                RangeLevelMIN = 10,
//                RangeLevelStep = 10,
//                DFSQUModes = new List<ParamWithId>()
//                {
//                    new ParamWithId() {Id = (int)Enums.DFSQUMode.Off, Parameter = "OFF"},
//                    new ParamWithId() {Id = (int)Enums.DFSQUMode.Gate, Parameter = "GATE"},
//                    new ParamWithId() {Id = (int)Enums.DFSQUMode.Normal, Parameter = "NORM"},
//                },
//                MeasTimeMAX = 0.0005M,
//                MeasTimeMIN = 900,
//            },
//            #endregion
//            #region ESMD
//            new LocalRSReceiverInfo
//            {
//                InstrManufacrure = 1, InstrModel = "ESMD",
//                EB200Protocol = true,
//                XMLProtocol = false,
//                InstrOption = new List<DeviceOption>()
//                {
//                    new DeviceOption() { Type = "PS", Designation = "Panorama Scan", GlobalType = "Panorama Scan"},
//                    new DeviceOption() { Type = "IR", Designation = "Internal Recording", GlobalType = "Internal Recording"},
//                    new DeviceOption() { Type = "RC", Designation = "Remote Control", GlobalType = "Remote Control"},
//                    new DeviceOption() { Type = "ET", Designation = "External Triggered Measurement", GlobalType = "External Triggered Measurement"},
//                    new DeviceOption() { Type = "FS", Designation = "Fieldstrength Measurement", GlobalType = "Fieldstrength Measurement"},
//                    new DeviceOption() { Type = "FP", Designation = "Frequency Processing SHF", GlobalType = "Frequency Processing SHF"},
//                    new DeviceOption() { Type = "GP", Designation = "GPS Compass", GlobalType = "GPS Compass"},
//                    new DeviceOption() { Type = "FE", Designation = "Frequency Extension", GlobalType = "Frequency Extension"},
//                    new DeviceOption() { Type = "DF", Designation = "Direction Finder", GlobalType = "Direction Finder"},
//                    new DeviceOption() { Type = "WB", Designation = "Wideband 80 MHz", GlobalType = "Wideband 80 MHz"},
//                },
//                LoadedInstrOption = new List<DeviceOption>() { },
//                Modes = new List<LocalRSReceiverInfo.Mode>
//                {
//                    new LocalRSReceiverInfo.Mode {ModeName = "FFM", MeasAppl = "RX", FreqMode = "CW"},
//                    new LocalRSReceiverInfo.Mode {ModeName = "DF", MeasAppl = "DF", FreqMode = "CW"},
//                    new LocalRSReceiverInfo.Mode {ModeName = "PSCAN", MeasAppl = "RX", FreqMode = "PSC"},
//                },
//                DemodFreq = true,
//                DemodBW = new decimal[] { 100, 150, 300, 600, 1000, 1500, 2100, 2400, 2700, 3100, 4000, 4800, 6000, 8300, 9000, 12000, 15000, 25000, 30000, 50000, 75000, 120000, 150000, 250000, 300000, 500000, 800000, 1000000, 1250000, 1500000, 2000000, 5000000, 8000000, 10000000, 12500000, 15000000, 20000000 },
//                Demod =  new string[] { "AM", "FM", "PULSE", "PM", "IQ", "ISB", "TV" },
//                AllStepBW = new decimal[] { 0.625m, 1.25m, 2.5m, 3.125m, 6.25m, 12.5m, 25, 31.25m, 50, 62.5m, 100, 125, 200, 250, 312.5m, 500, 625, 1000, 1250, 2000, 2500, 3125, 5000, 6250, 8333, 10000, 12500, 20000, 25000, 50000, 100000, 200000, 500000, 1000000, 2000000 },
//                DFBW = new decimal[] { 50, 100, 150, 300, 600, 1000, 1500, 2100, 2400, 2700, 3100, 4000, 4800, 6000, 8333, 9000, 12000, 15000, 25000, 30000, 50000, 75000, 120000, 150000, 250000, 300000, 500000, 800000, 1000000, 1250000, 1500000, 2000000 },
//                PSCANStepBW = new decimal[] { 0.625m, 1.25m, 2.5m, 3.125m, 6.25m, 12.5m, 25, 31.25m, 50, 62.5m, 100, 125, 200, 250, 312.5m, 500, 625, 1000, 1250, 2000, 2500, 3125, 5000, 6250, 8333, 10000, 12500, 20000, 25000, 50000, 100000, 200000, 500000, 1000000, 2000000 },
//                FFMSpan = new LocalRSReceiverInfo.FFMSpanBW[]
//                {
//                    new LocalRSReceiverInfo.FFMSpanBW(){ BW = 1000, AvailableStepBW = new decimal[]{ 0.625m, 1.25m, 2.5m, 3.125m, 6.25m, 12.5m, 25, 31.25m, 50, 62.5m, 100, 125 } },
//                    new LocalRSReceiverInfo.FFMSpanBW(){ BW = 2000, AvailableStepBW = new decimal[]{ 0.625m, 1.25m, 2.5m, 3.125m, 6.25m, 12.5m, 25, 31.25m, 50, 62.5m, 100, 125, 200, 250 } },
//                    new LocalRSReceiverInfo.FFMSpanBW(){ BW = 5000, AvailableStepBW = new decimal[]{ 2.5m, 3.125m, 6.25m, 12.5m, 25, 31.25m, 50, 62.5m, 100, 125, 200, 250, 312.5m, 500, 625 } },
//                    new LocalRSReceiverInfo.FFMSpanBW(){ BW = 10000, AvailableStepBW = new decimal[]{ 3.125m, 6.25m, 12.5m, 25, 31.25m, 50, 62.5m, 100, 125, 200, 250, 312.5m, 500, 625, 1000, 1250 } },
//                    new LocalRSReceiverInfo.FFMSpanBW(){ BW = 20000, AvailableStepBW = new decimal[]{ 6.25m, 12.5m, 25, 31.25m, 50, 62.5m, 100, 125, 200, 250, 312.5m, 500, 625, 1000, 1250, 2000, 2500 } },
//                    new LocalRSReceiverInfo.FFMSpanBW(){ BW = 50000, AvailableStepBW = new decimal[]{ 25, 31.25m, 50, 62.5m, 100, 125, 200, 250, 312.5m, 500, 625, 1000, 1250, 2000, 2500, 3125, 5000, 6250 } },
//                    new LocalRSReceiverInfo.FFMSpanBW(){ BW = 100000, AvailableStepBW = new decimal[]{ 31.25m, 50, 62.5m, 100, 125, 200, 250, 312.5m, 500, 625, 1000, 1250, 2000, 2500, 3125, 5000, 6250, 8333, 10000, 12500 } },
//                    new LocalRSReceiverInfo.FFMSpanBW(){ BW = 200000, AvailableStepBW = new decimal[]{ 62.5m, 100, 125, 200, 250, 312.5m, 500, 625, 1000, 1250, 2000, 2500, 3125, 5000, 6250, 8333, 10000, 12500, 20000, 25000 } },
//                    new LocalRSReceiverInfo.FFMSpanBW(){ BW = 500000, AvailableStepBW = new decimal[]{ 200, 250, 312.5m, 500, 625, 1000, 1250, 2000, 2500, 3125, 5000, 6250, 8333, 10000, 12500, 20000, 25000, 50000 } },
//                    new LocalRSReceiverInfo.FFMSpanBW(){ BW = 1000000, AvailableStepBW = new decimal[]{ 312.5m, 500, 625, 1000, 1250, 2000, 2500, 3125, 5000, 6250, 8333, 10000, 12500, 20000, 25000, 50000, 100000 } },
//                    new LocalRSReceiverInfo.FFMSpanBW(){ BW = 2000000, AvailableStepBW = new decimal[]{ 625, 1000, 1250, 2000, 2500, 3125, 5000, 6250, 8333, 10000, 12500, 20000, 25000, 50000, 100000, 200000 } },
//                    new LocalRSReceiverInfo.FFMSpanBW(){ BW = 5000000, AvailableStepBW = new decimal[]{ 2000, 2500, 3125, 5000, 6250, 8333, 10000, 12500, 20000, 25000, 50000, 100000, 200000, 500000 } },
//                    new LocalRSReceiverInfo.FFMSpanBW(){ BW = 10000000, AvailableStepBW = new decimal[]{ 3125, 5000, 6250, 8333, 10000, 12500, 20000, 25000, 50000, 100000, 200000, 500000, 1000000 } },
//                    new LocalRSReceiverInfo.FFMSpanBW(){ BW = 20000000, AvailableStepBW = new decimal[]{ 6250, 8333, 10000, 12500, 20000, 25000, 50000, 100000, 200000, 500000, 1000000, 2000000 } }
//                },
//                FFTModes = new List<ParamWithId>
//                {
//                    new  ParamWithId() {Id = (int)Enums.FFTMode.Minimum, Parameter = "MIN"},
//                    new  ParamWithId() {Id = (int)Enums.FFTMode.Maximum, Parameter = "MAX"},
//                    new  ParamWithId() {Id = (int)Enums.FFTMode.Average, Parameter = "SCAL"},
//                    new  ParamWithId() {Id = (int)Enums.FFTMode.ClearWrite, Parameter = "OFF"},
//                },
//                SelectivityChangeable = true,
//                SelectivityModes = new List<ParamWithId>()
//                {
//                    new ParamWithId() {Id = (int)Enums.SelectivityMode.Auto, Parameter = "AUTO"},
//                    new ParamWithId() {Id = (int)Enums.SelectivityMode.Normal, Parameter = "NORM"},
//                    new ParamWithId() {Id = (int)Enums.SelectivityMode.Narrow, Parameter = "NARR"},
//                    new ParamWithId() {Id = (int)Enums.SelectivityMode.Sharp, Parameter = "SHAR"},
//                },
//                Detectors = new List<ParamWithId>()
//                {
//                    new ParamWithId() {Id = (int)Enums.Detector.Average, Parameter = "PAV"},
//                    new ParamWithId() {Id = (int)Enums.Detector.Peak, Parameter = "POS"},
//                    new ParamWithId() {Id = (int)Enums.Detector.Fast, Parameter = "FAST"},
//                    new ParamWithId() {Id = (int)Enums.Detector.RMS, Parameter = "RMS"},
//                },
//                RFModeChangeable = true,
//                RFModes= new List<ParamWithId>()
//                {
//                    new ParamWithId() {Id = (int)Enums.RFMode.Normal, Parameter = "NORM"},
//                    new ParamWithId() {Id = (int)Enums.RFMode.LowNoise, Parameter = "LOWN"},
//                    new ParamWithId() {Id = (int)Enums.RFMode.LowDistortion, Parameter = "LOWD"},
//                },
//                ATTFix = false,
//                AttMax = 70,
//                AttStep = 1,

//                RefLevelMAX = 130,
//                RefLevelMIN = -29,
//                RangeLevelMAX = 200,
//                RangeLevelMIN = 10,
//                RangeLevelStep = 1,
//                DFSQUModes = new List<ParamWithId>()
//                {
//                    new ParamWithId() {Id = (int)Enums.DFSQUMode.Off, Parameter = "OFF"},
//                    new ParamWithId() {Id = (int)Enums.DFSQUMode.Gate, Parameter = "GATE"},
//                    new ParamWithId() {Id = (int)Enums.DFSQUMode.Normal, Parameter = "NORM"},
//                },
//                MeasTimeMAX = 0.0005M,
//                MeasTimeMIN = 900,
//            },
//            #endregion
//            #region DDF205
//            new LocalRSReceiverInfo
//            {
//                InstrManufacrure = 1, InstrModel = "DDF2",
//                EB200Protocol = true,
//                XMLProtocol = false,
//                InstrOption = new List<DeviceOption>()
//                {
//                    new DeviceOption() { Type = "PS", Designation = "Panorama Scan", GlobalType = "Panorama Scan"},
//                    new DeviceOption() { Type = "IR", Designation = "Internal Recording", GlobalType = "Internal Recording"},
//                    new DeviceOption() { Type = "RC", Designation = "Remote Control", GlobalType = "Remote Control"},
//                    new DeviceOption() { Type = "ET", Designation = "External Triggered Measurement", GlobalType = "External Triggered Measurement"},
//                    new DeviceOption() { Type = "FS", Designation = "Fieldstrength Measurement", GlobalType = "Fieldstrength Measurement"},
//                    new DeviceOption() { Type = "FP", Designation = "Frequency Processing SHF", GlobalType = "Frequency Processing SHF"},
//                    new DeviceOption() { Type = "GP", Designation = "GPS Compass", GlobalType = "GPS Compass"},
//                    new DeviceOption() { Type = "FE", Designation = "Frequency Extension", GlobalType = "Frequency Extension"},
//                    new DeviceOption() { Type = "DF", Designation = "Direction Finder", GlobalType = "Direction Finder"},
//                },
//                LoadedInstrOption = new List<DeviceOption>() { },
//                Modes = new List<LocalRSReceiverInfo.Mode>
//                {
//                    new LocalRSReceiverInfo.Mode {ModeName = "FFM", MeasAppl = "RX", FreqMode = "CW"},
//                    new LocalRSReceiverInfo.Mode {ModeName = "DF", MeasAppl = "DF", FreqMode = "CW"},
//                    new LocalRSReceiverInfo.Mode {ModeName = "PSCAN", MeasAppl = "RX", FreqMode = "PSC"},
//                },
//                DemodFreq = true,
//                DemodBW = new decimal[] { 100, 150, 300, 600, 1000, 1500, 2100, 2400, 2700, 3100, 4000, 4800, 6000, 8300, 9000, 12000, 15000, 25000, 30000, 50000, 75000, 120000, 150000, 250000, 300000, 500000, 800000, 1000000, 1250000, 1500000, 2000000, 5000000, 8000000, 10000000, 12500000, 15000000, 20000000 },
//                Demod =  new string[] { "AM", "FM", "PULSE", "PM", "IQ", "ISB", "TV" },
//                AllStepBW = new decimal[] { 0.625m, 1.25m, 2.5m, 3.125m, 6.25m, 12.5m, 25, 31.25m, 50, 62.5m, 100, 125, 200, 250, 312.5m, 500, 625, 1000, 1250, 2000, 2500, 3125, 5000, 6250, 8333, 10000, 12500, 20000, 25000, 50000, 100000, 200000, 500000, 1000000, 2000000 },
//                DFBW = new decimal[] { 50, 100, 150, 300, 600, 1000, 1500, 2100, 2400, 2700, 3100, 4000, 4800, 6000, 8333, 9000, 12000, 15000, 25000, 30000, 50000, 75000, 120000, 150000, 250000, 300000, 500000, 800000, 1000000, 1250000, 1500000, 2000000 },
//                PSCANStepBW = new decimal[] { 0.625m, 1.25m, 2.5m, 3.125m, 6.25m, 12.5m, 25, 31.25m, 50, 62.5m, 100, 125, 200, 250, 312.5m, 500, 625, 1000, 1250, 2000, 2500, 3125, 5000, 6250, 8333, 10000, 12500, 20000, 25000, 50000, 100000, 200000, 500000, 1000000, 2000000 },
//                FFMSpan = new LocalRSReceiverInfo.FFMSpanBW[]
//                {
//                    new LocalRSReceiverInfo.FFMSpanBW(){ BW = 1000, AvailableStepBW = new decimal[]{ 0.625m, 1.25m, 2.5m, 3.125m, 6.25m, 12.5m, 25, 31.25m, 50, 62.5m, 100, 125 } },
//                    new LocalRSReceiverInfo.FFMSpanBW(){ BW = 2000, AvailableStepBW = new decimal[]{ 0.625m, 1.25m, 2.5m, 3.125m, 6.25m, 12.5m, 25, 31.25m, 50, 62.5m, 100, 125, 200, 250 } },
//                    new LocalRSReceiverInfo.FFMSpanBW(){ BW = 5000, AvailableStepBW = new decimal[]{ 2.5m, 3.125m, 6.25m, 12.5m, 25, 31.25m, 50, 62.5m, 100, 125, 200, 250, 312.5m, 500, 625 } },
//                    new LocalRSReceiverInfo.FFMSpanBW(){ BW = 10000, AvailableStepBW = new decimal[]{ 3.125m, 6.25m, 12.5m, 25, 31.25m, 50, 62.5m, 100, 125, 200, 250, 312.5m, 500, 625, 1000, 1250 } },
//                    new LocalRSReceiverInfo.FFMSpanBW(){ BW = 20000, AvailableStepBW = new decimal[]{ 6.25m, 12.5m, 25, 31.25m, 50, 62.5m, 100, 125, 200, 250, 312.5m, 500, 625, 1000, 1250, 2000, 2500 } },
//                    new LocalRSReceiverInfo.FFMSpanBW(){ BW = 50000, AvailableStepBW = new decimal[]{ 25, 31.25m, 50, 62.5m, 100, 125, 200, 250, 312.5m, 500, 625, 1000, 1250, 2000, 2500, 3125, 5000, 6250 } },
//                    new LocalRSReceiverInfo.FFMSpanBW(){ BW = 100000, AvailableStepBW = new decimal[]{ 31.25m, 50, 62.5m, 100, 125, 200, 250, 312.5m, 500, 625, 1000, 1250, 2000, 2500, 3125, 5000, 6250, 8333, 10000, 12500 } },
//                    new LocalRSReceiverInfo.FFMSpanBW(){ BW = 200000, AvailableStepBW = new decimal[]{ 62.5m, 100, 125, 200, 250, 312.5m, 500, 625, 1000, 1250, 2000, 2500, 3125, 5000, 6250, 8333, 10000, 12500, 20000, 25000 } },
//                    new LocalRSReceiverInfo.FFMSpanBW(){ BW = 500000, AvailableStepBW = new decimal[]{ 200, 250, 312.5m, 500, 625, 1000, 1250, 2000, 2500, 3125, 5000, 6250, 8333, 10000, 12500, 20000, 25000, 50000 } },
//                    new LocalRSReceiverInfo.FFMSpanBW(){ BW = 1000000, AvailableStepBW = new decimal[]{ 312.5m, 500, 625, 1000, 1250, 2000, 2500, 3125, 5000, 6250, 8333, 10000, 12500, 20000, 25000, 50000, 100000 } },
//                    new LocalRSReceiverInfo.FFMSpanBW(){ BW = 2000000, AvailableStepBW = new decimal[]{ 625, 1000, 1250, 2000, 2500, 3125, 5000, 6250, 8333, 10000, 12500, 20000, 25000, 50000, 100000, 200000 } },
//                    new LocalRSReceiverInfo.FFMSpanBW(){ BW = 5000000, AvailableStepBW = new decimal[]{ 2000, 2500, 3125, 5000, 6250, 8333, 10000, 12500, 20000, 25000, 50000, 100000, 200000, 500000 } },
//                    new LocalRSReceiverInfo.FFMSpanBW(){ BW = 10000000, AvailableStepBW = new decimal[]{ 3125, 5000, 6250, 8333, 10000, 12500, 20000, 25000, 50000, 100000, 200000, 500000, 1000000 } },
//                    new LocalRSReceiverInfo.FFMSpanBW(){ BW = 20000000, AvailableStepBW = new decimal[]{ 6250, 8333, 10000, 12500, 20000, 25000, 50000, 100000, 200000, 500000, 1000000, 2000000 } }
//                },
//                FFTModes = new List<ParamWithId>
//                {
//                    new  ParamWithId() {Id = (int)Enums.FFTMode.Minimum, Parameter = "MIN"},
//                    new  ParamWithId() {Id = (int)Enums.FFTMode.Maximum, Parameter = "MAX"},
//                    new  ParamWithId() {Id = (int)Enums.FFTMode.Average, Parameter = "SCAL"},
//                    new  ParamWithId() {Id = (int)Enums.FFTMode.ClearWrite, Parameter = "OFF"},
//                },
//                SelectivityChangeable = true,
//                SelectivityModes = new List<ParamWithId>()
//                {
//                    new ParamWithId() {Id = (int)Enums.SelectivityMode.Auto, Parameter = "AUTO"},
//                    new ParamWithId() {Id = (int)Enums.SelectivityMode.Normal, Parameter = "NORM"},
//                    new ParamWithId() {Id = (int)Enums.SelectivityMode.Narrow, Parameter = "NARR"},
//                    new ParamWithId() {Id = (int)Enums.SelectivityMode.Sharp, Parameter = "SHAR"},
//                },
//                Detectors = new List<ParamWithId>()
//                {
//                    new ParamWithId() {Id = (int)Enums.Detector.Average, Parameter = "PAV"},
//                    new ParamWithId() {Id = (int)Enums.Detector.Peak, Parameter = "POS"},
//                    new ParamWithId() {Id = (int)Enums.Detector.Fast, Parameter = "FAST"},
//                    new ParamWithId() {Id = (int)Enums.Detector.RMS, Parameter = "RMS"},
//                },
//                RFModeChangeable = true,
//                RFModes= new List<ParamWithId>()
//                {
//                    new ParamWithId() {Id = (int)Enums.RFMode.Normal, Parameter = "NORM"},
//                    new ParamWithId() {Id = (int)Enums.RFMode.LowNoise, Parameter = "LOWN"},
//                    new ParamWithId() {Id = (int)Enums.RFMode.LowDistortion, Parameter = "LOWD"},
//                },
//                ATTFix = false,
//                AttMax = 70,
//                AttStep = 1,

//                RefLevelMAX = 130,
//                RefLevelMIN = -29,
//                RangeLevelMAX = 200,
//                RangeLevelMIN = 10,
//                RangeLevelStep = 1,
//                DFSQUModes = new List<ParamWithId>()
//                {
//                    new ParamWithId() {Id = (int)Enums.DFSQUMode.Off, Parameter = "OFF"},
//                    new ParamWithId() {Id = (int)Enums.DFSQUMode.Gate, Parameter = "GATE"},
//                    new ParamWithId() {Id = (int)Enums.DFSQUMode.Normal, Parameter = "NORM"},
//                },
//                MeasTimeMAX = 0.0005M,
//                MeasTimeMIN = 900,
//            },
//            #endregion
//            #region DDF255
//            new LocalRSReceiverInfo
//            {
//                InstrManufacrure = 1, InstrModel = "DDF255",
//                EB200Protocol = true,
//                XMLProtocol = false,
//                InstrOption = new List<DeviceOption>()
//                {
//                    new DeviceOption() { Type = "PS", Designation = "Panorama Scan", GlobalType = "Panorama Scan"},
//                    new DeviceOption() { Type = "IR", Designation = "Internal Recording", GlobalType = "Internal Recording"},
//                    new DeviceOption() { Type = "RC", Designation = "Remote Control", GlobalType = "Remote Control"},
//                    new DeviceOption() { Type = "ET", Designation = "External Triggered Measurement", GlobalType = "External Triggered Measurement"},
//                    new DeviceOption() { Type = "FS", Designation = "Fieldstrength Measurement", GlobalType = "Fieldstrength Measurement"},
//                    new DeviceOption() { Type = "FP", Designation = "Frequency Processing SHF", GlobalType = "Frequency Processing SHF"},
//                    new DeviceOption() { Type = "GP", Designation = "GPS Compass", GlobalType = "GPS Compass"},
//                    new DeviceOption() { Type = "FE", Designation = "Frequency Extension", GlobalType = "Frequency Extension"},
//                    new DeviceOption() { Type = "DF", Designation = "Direction Finder", GlobalType = "Direction Finder"},
//                     new DeviceOption() { Type = "WB", Designation = "Wideband 80 MHz", GlobalType = "Wideband 80 MHz"},
//                },
//                LoadedInstrOption = new List<DeviceOption>() { },
//                Modes = new List<LocalRSReceiverInfo.Mode>
//                {
//                    new LocalRSReceiverInfo.Mode {ModeName = "FFM", MeasAppl = "RX", FreqMode = "CW"},
//                    new LocalRSReceiverInfo.Mode {ModeName = "DF", MeasAppl = "DF", FreqMode = "CW"},
//                    new LocalRSReceiverInfo.Mode {ModeName = "PSCAN", MeasAppl = "RX", FreqMode = "PSC"},
//                },
//                DemodFreq = true,
//                DemodBW = new decimal[] { 100, 150, 300, 600, 1000, 1500, 2100, 2400, 2700, 3100, 4000, 4800, 6000, 8300, 9000, 12000, 15000, 25000, 30000, 50000, 75000, 120000, 150000, 250000, 300000, 500000, 800000, 1000000, 1250000, 1500000, 2000000, 5000000, 8000000, 10000000, 12500000, 15000000, 20000000 },
//                Demod =  new string[] { "AM", "FM", "PULSE", "PM", "IQ", "ISB", "TV" },
//                AllStepBW = new decimal[] { 0.625m, 1.25m, 2.5m, 3.125m, 6.25m, 12.5m, 25, 31.25m, 50, 62.5m, 100, 125, 200, 250, 312.5m, 500, 625, 1000, 1250, 2000, 2500, 3125, 5000, 6250, 8333, 10000, 12500, 20000, 25000, 50000, 100000, 200000, 500000, 1000000, 2000000 },
//                DFBW = new decimal[] { 50, 100, 150, 300, 600, 1000, 1500, 2100, 2400, 2700, 3100, 4000, 4800, 6000, 8333, 9000, 12000, 15000, 25000, 30000, 50000, 75000, 120000, 150000, 250000, 300000, 500000, 800000, 1000000, 1250000, 1500000, 2000000 },
//                PSCANStepBW = new decimal[] { 0.625m, 1.25m, 2.5m, 3.125m, 6.25m, 12.5m, 25, 31.25m, 50, 62.5m, 100, 125, 200, 250, 312.5m, 500, 625, 1000, 1250, 2000, 2500, 3125, 5000, 6250, 8333, 10000, 12500, 20000, 25000, 50000, 100000, 200000, 500000, 1000000, 2000000 },
//                FFMSpan = new LocalRSReceiverInfo.FFMSpanBW[]
//                {
//                    new LocalRSReceiverInfo.FFMSpanBW(){ BW = 1000, AvailableStepBW = new decimal[]{ 0.625m, 1.25m, 2.5m, 3.125m, 6.25m, 12.5m, 25, 31.25m, 50, 62.5m, 100, 125 } },
//                    new LocalRSReceiverInfo.FFMSpanBW(){ BW = 2000, AvailableStepBW = new decimal[]{ 0.625m, 1.25m, 2.5m, 3.125m, 6.25m, 12.5m, 25, 31.25m, 50, 62.5m, 100, 125, 200, 250 } },
//                    new LocalRSReceiverInfo.FFMSpanBW(){ BW = 5000, AvailableStepBW = new decimal[]{ 2.5m, 3.125m, 6.25m, 12.5m, 25, 31.25m, 50, 62.5m, 100, 125, 200, 250, 312.5m, 500, 625 } },
//                    new LocalRSReceiverInfo.FFMSpanBW(){ BW = 10000, AvailableStepBW = new decimal[]{ 3.125m, 6.25m, 12.5m, 25, 31.25m, 50, 62.5m, 100, 125, 200, 250, 312.5m, 500, 625, 1000, 1250 } },
//                    new LocalRSReceiverInfo.FFMSpanBW(){ BW = 20000, AvailableStepBW = new decimal[]{ 6.25m, 12.5m, 25, 31.25m, 50, 62.5m, 100, 125, 200, 250, 312.5m, 500, 625, 1000, 1250, 2000, 2500 } },
//                    new LocalRSReceiverInfo.FFMSpanBW(){ BW = 50000, AvailableStepBW = new decimal[]{ 25, 31.25m, 50, 62.5m, 100, 125, 200, 250, 312.5m, 500, 625, 1000, 1250, 2000, 2500, 3125, 5000, 6250 } },
//                    new LocalRSReceiverInfo.FFMSpanBW(){ BW = 100000, AvailableStepBW = new decimal[]{ 31.25m, 50, 62.5m, 100, 125, 200, 250, 312.5m, 500, 625, 1000, 1250, 2000, 2500, 3125, 5000, 6250, 8333, 10000, 12500 } },
//                    new LocalRSReceiverInfo.FFMSpanBW(){ BW = 200000, AvailableStepBW = new decimal[]{ 62.5m, 100, 125, 200, 250, 312.5m, 500, 625, 1000, 1250, 2000, 2500, 3125, 5000, 6250, 8333, 10000, 12500, 20000, 25000 } },
//                    new LocalRSReceiverInfo.FFMSpanBW(){ BW = 500000, AvailableStepBW = new decimal[]{ 200, 250, 312.5m, 500, 625, 1000, 1250, 2000, 2500, 3125, 5000, 6250, 8333, 10000, 12500, 20000, 25000, 50000 } },
//                    new LocalRSReceiverInfo.FFMSpanBW(){ BW = 1000000, AvailableStepBW = new decimal[]{ 312.5m, 500, 625, 1000, 1250, 2000, 2500, 3125, 5000, 6250, 8333, 10000, 12500, 20000, 25000, 50000, 100000 } },
//                    new LocalRSReceiverInfo.FFMSpanBW(){ BW = 2000000, AvailableStepBW = new decimal[]{ 625, 1000, 1250, 2000, 2500, 3125, 5000, 6250, 8333, 10000, 12500, 20000, 25000, 50000, 100000, 200000 } },
//                    new LocalRSReceiverInfo.FFMSpanBW(){ BW = 5000000, AvailableStepBW = new decimal[]{ 2000, 2500, 3125, 5000, 6250, 8333, 10000, 12500, 20000, 25000, 50000, 100000, 200000, 500000 } },
//                    new LocalRSReceiverInfo.FFMSpanBW(){ BW = 10000000, AvailableStepBW = new decimal[]{ 3125, 5000, 6250, 8333, 10000, 12500, 20000, 25000, 50000, 100000, 200000, 500000, 1000000 } },
//                    new LocalRSReceiverInfo.FFMSpanBW(){ BW = 20000000, AvailableStepBW = new decimal[]{ 6250, 8333, 10000, 12500, 20000, 25000, 50000, 100000, 200000, 500000, 1000000, 2000000 } }
//                },
//                FFTModes = new List<ParamWithId>
//                {
//                    new  ParamWithId() {Id = (int)Enums.FFTMode.Minimum, Parameter = "MIN"},
//                    new  ParamWithId() {Id = (int)Enums.FFTMode.Maximum, Parameter = "MAX"},
//                    new  ParamWithId() {Id = (int)Enums.FFTMode.Average, Parameter = "SCAL"},
//                    new  ParamWithId() {Id = (int)Enums.FFTMode.ClearWrite, Parameter = "OFF"},
//                },
//                SelectivityChangeable = true,
//                SelectivityModes = new List<ParamWithId>()
//                {
//                    new ParamWithId() {Id = (int)Enums.SelectivityMode.Auto, Parameter = "AUTO"},
//                    new ParamWithId() {Id = (int)Enums.SelectivityMode.Normal, Parameter = "NORM"},
//                    new ParamWithId() {Id = (int)Enums.SelectivityMode.Narrow, Parameter = "NARR"},
//                    new ParamWithId() {Id = (int)Enums.SelectivityMode.Sharp, Parameter = "SHAR"},
//                },
//                Detectors = new List<ParamWithId>()
//                {
//                    new ParamWithId() {Id = (int)Enums.Detector.Average, Parameter = "PAV"},
//                    new ParamWithId() {Id = (int)Enums.Detector.Peak, Parameter = "POS"},
//                    new ParamWithId() {Id = (int)Enums.Detector.Fast, Parameter = "FAST"},
//                    new ParamWithId() {Id = (int)Enums.Detector.RMS, Parameter = "RMS"},
//                },
//                RFModeChangeable = true,
//                RFModes= new List<ParamWithId>()
//                {
//                    new ParamWithId() {Id = (int)Enums.RFMode.Normal, Parameter = "NORM"},
//                    new ParamWithId() {Id = (int)Enums.RFMode.LowNoise, Parameter = "LOWN"},
//                    new ParamWithId() {Id = (int)Enums.RFMode.LowDistortion, Parameter = "LOWD"},
//                },
//                ATTFix = false,
//                AttMax = 70,
//                AttStep = 1,

//                RefLevelMAX = 130,
//                RefLevelMIN = -29,
//                RangeLevelMAX = 200,
//                RangeLevelMIN = 10,
//                RangeLevelStep = 1,
//                DFSQUModes = new List<ParamWithId>()
//                {
//                    new ParamWithId() {Id = (int)Enums.DFSQUMode.Off, Parameter = "OFF"},
//                    new ParamWithId() {Id = (int)Enums.DFSQUMode.Gate, Parameter = "GATE"},
//                    new ParamWithId() {Id = (int)Enums.DFSQUMode.Normal, Parameter = "NORM"},
//                },
//                MeasTimeMAX = 0.0005M,
//                MeasTimeMIN = 900,
//            },
//            #endregion
//            #region EB500
//            new LocalRSReceiverInfo
//            {
//                InstrManufacrure = 1, InstrModel = "EB500",
//                EB200Protocol = true,
//                XMLProtocol = false,
//                InstrOption = new List<DeviceOption>()
//                {
//                    new DeviceOption() { Type = "PS", Designation = "Panorama Scan", GlobalType = "Panorama Scan"},
//                    new DeviceOption() { Type = "IR", Designation = "Internal Recording", GlobalType = "Internal Recording"},
//                    new DeviceOption() { Type = "RC", Designation = "Remote Control", GlobalType = "Remote Control"},
//                    new DeviceOption() { Type = "ET", Designation = "External Triggered Measurement", GlobalType = "External Triggered Measurement"},
//                    new DeviceOption() { Type = "FS", Designation = "Fieldstrength Measurement", GlobalType = "Fieldstrength Measurement"},
//                    new DeviceOption() { Type = "FP", Designation = "Frequency Processing SHF", GlobalType = "Frequency Processing SHF"},
//                    new DeviceOption() { Type = "GP", Designation = "GPS Compass", GlobalType = "GPS Compass"},
//                    new DeviceOption() { Type = "FE", Designation = "Frequency Extension", GlobalType = "Frequency Extension"},
//                    new DeviceOption() { Type = "DF", Designation = "Direction Finder", GlobalType = "Direction Finder"},
//                },
//                LoadedInstrOption = new List<DeviceOption>() { },
//                Modes = new List<LocalRSReceiverInfo.Mode>
//                {
//                    new LocalRSReceiverInfo.Mode {ModeName = "FFM", MeasAppl = "RX", FreqMode = "CW"},
//                    new LocalRSReceiverInfo.Mode {ModeName = "DF", MeasAppl = "DF", FreqMode = "CW"},
//                    new LocalRSReceiverInfo.Mode {ModeName = "PSCAN", MeasAppl = "RX", FreqMode = "PSC"},
//                },
//                DemodFreq = false,
//                DemodBW = new decimal[] { 100, 150, 300, 600, 1000, 1500, 2100, 2400, 2700, 3100, 4000, 4800, 6000, 8300, 9000, 12000, 15000, 25000, 30000, 50000, 75000, 120000, 150000, 250000, 300000, 500000, 800000, 1000000, 1250000, 1500000, 2000000},
//                Demod =  new string[] { "AM", "FM", "PULSE", "PM", "IQ", "ISB", "TV" },
//                AllStepBW = new decimal[] { 0.625m, 1.25m, 2.5m, 3.125m, 6.25m, 12.5m, 25, 31.25m, 50, 62.5m, 100, 125, 200, 250, 312.5m, 500, 625, 1000, 1250, 2000, 2500, 3125, 5000, 6250, 8333, 10000, 12500, 20000, 25000, 50000, 100000, 200000, 500000, 1000000, 2000000 },
//                DFBW = new decimal[] { 50, 100, 150, 300, 600, 1000, 1500, 2100, 2400, 2700, 3100, 4000, 4800, 6000, 8333, 9000, 12000, 15000, 25000, 30000, 50000, 75000, 120000, 150000, 250000, 300000, 500000, 800000, 1000000, 1250000, 1500000, 2000000 },
//                PSCANStepBW = new decimal[] { 0.625m, 1.25m, 2.5m, 3.125m, 6.25m, 12.5m, 25, 31.25m, 50, 62.5m, 100, 125, 200, 250, 312.5m, 500, 625, 1000, 1250, 2000, 2500, 3125, 5000, 6250, 8333, 10000, 12500, 20000, 25000, 50000, 100000, 200000, 500000, 1000000, 2000000 },
//                FFMSpan = new LocalRSReceiverInfo.FFMSpanBW[]
//                {
//                    new LocalRSReceiverInfo.FFMSpanBW(){ BW = 1000, AvailableStepBW = new decimal[]{ 0.625m, 1.25m, 2.5m, 3.125m, 6.25m, 12.5m, 25, 31.25m, 50, 62.5m, 100, 125 } },
//                    new LocalRSReceiverInfo.FFMSpanBW(){ BW = 2000, AvailableStepBW = new decimal[]{ 0.625m, 1.25m, 2.5m, 3.125m, 6.25m, 12.5m, 25, 31.25m, 50, 62.5m, 100, 125, 200, 250 } },
//                    new LocalRSReceiverInfo.FFMSpanBW(){ BW = 5000, AvailableStepBW = new decimal[]{ 2.5m, 3.125m, 6.25m, 12.5m, 25, 31.25m, 50, 62.5m, 100, 125, 200, 250, 312.5m, 500, 625 } },
//                    new LocalRSReceiverInfo.FFMSpanBW(){ BW = 10000, AvailableStepBW = new decimal[]{ 3.125m, 6.25m, 12.5m, 25, 31.25m, 50, 62.5m, 100, 125, 200, 250, 312.5m, 500, 625, 1000, 1250 } },
//                    new LocalRSReceiverInfo.FFMSpanBW(){ BW = 20000, AvailableStepBW = new decimal[]{ 6.25m, 12.5m, 25, 31.25m, 50, 62.5m, 100, 125, 200, 250, 312.5m, 500, 625, 1000, 1250, 2000, 2500 } },
//                    new LocalRSReceiverInfo.FFMSpanBW(){ BW = 50000, AvailableStepBW = new decimal[]{ 25, 31.25m, 50, 62.5m, 100, 125, 200, 250, 312.5m, 500, 625, 1000, 1250, 2000, 2500, 3125, 5000, 6250 } },
//                    new LocalRSReceiverInfo.FFMSpanBW(){ BW = 100000, AvailableStepBW = new decimal[]{ 31.25m, 50, 62.5m, 100, 125, 200, 250, 312.5m, 500, 625, 1000, 1250, 2000, 2500, 3125, 5000, 6250, 8333, 10000, 12500 } },
//                    new LocalRSReceiverInfo.FFMSpanBW(){ BW = 200000, AvailableStepBW = new decimal[]{ 62.5m, 100, 125, 200, 250, 312.5m, 500, 625, 1000, 1250, 2000, 2500, 3125, 5000, 6250, 8333, 10000, 12500, 20000, 25000 } },
//                    new LocalRSReceiverInfo.FFMSpanBW(){ BW = 500000, AvailableStepBW = new decimal[]{ 200, 250, 312.5m, 500, 625, 1000, 1250, 2000, 2500, 3125, 5000, 6250, 8333, 10000, 12500, 20000, 25000, 50000 } },
//                    new LocalRSReceiverInfo.FFMSpanBW(){ BW = 1000000, AvailableStepBW = new decimal[]{ 312.5m, 500, 625, 1000, 1250, 2000, 2500, 3125, 5000, 6250, 8333, 10000, 12500, 20000, 25000, 50000, 100000 } },
//                    new LocalRSReceiverInfo.FFMSpanBW(){ BW = 2000000, AvailableStepBW = new decimal[]{ 625, 1000, 1250, 2000, 2500, 3125, 5000, 6250, 8333, 10000, 12500, 20000, 25000, 50000, 100000, 200000 } },
//                    new LocalRSReceiverInfo.FFMSpanBW(){ BW = 5000000, AvailableStepBW = new decimal[]{ 2000, 2500, 3125, 5000, 6250, 8333, 10000, 12500, 20000, 25000, 50000, 100000, 200000, 500000 } },
//                    new LocalRSReceiverInfo.FFMSpanBW(){ BW = 10000000, AvailableStepBW = new decimal[]{ 3125, 5000, 6250, 8333, 10000, 12500, 20000, 25000, 50000, 100000, 200000, 500000, 1000000 } },
//                    new LocalRSReceiverInfo.FFMSpanBW(){ BW = 20000000, AvailableStepBW = new decimal[]{ 6250, 8333, 10000, 12500, 20000, 25000, 50000, 100000, 200000, 500000, 1000000, 2000000 } }
//                },
//                FFTModes = new List<ParamWithId>
//                {
//                    new  ParamWithId() {Id = (int)Enums.FFTMode.Minimum, Parameter = "MIN"},
//                    new  ParamWithId() {Id = (int)Enums.FFTMode.Maximum, Parameter = "MAX"},
//                    new  ParamWithId() {Id = (int)Enums.FFTMode.Average, Parameter = "SCAL"},
//                    new  ParamWithId() {Id = (int)Enums.FFTMode.ClearWrite, Parameter = "OFF"},
//                },
//                SelectivityChangeable = true,
//                SelectivityModes = new List<ParamWithId>()
//                {
//                    new ParamWithId() {Id = (int)Enums.SelectivityMode.Auto, Parameter = "AUTO"},
//                    new ParamWithId() {Id = (int)Enums.SelectivityMode.Normal, Parameter = "NORM"},
//                    new ParamWithId() {Id = (int)Enums.SelectivityMode.Narrow, Parameter = "NARR"},
//                    new ParamWithId() {Id = (int)Enums.SelectivityMode.Sharp, Parameter = "SHAR"},
//                },
//                Detectors = new List<ParamWithId>()
//                {
//                    new ParamWithId() {Id = (int)Enums.Detector.Average, Parameter = "PAV"},
//                    new ParamWithId() {Id = (int)Enums.Detector.Peak, Parameter = "POS"},
//                    new ParamWithId() {Id = (int)Enums.Detector.Fast, Parameter = "FAST"},
//                    new ParamWithId() {Id = (int)Enums.Detector.RMS, Parameter = "RMS"},
//                },
//                RFModeChangeable = true,
//                RFModes= new List<ParamWithId>()
//                {
//                    new ParamWithId() {Id = (int)Enums.RFMode.Normal, Parameter = "NORM"},
//                    new ParamWithId() {Id = (int)Enums.RFMode.LowNoise, Parameter = "LOWN"},
//                    new ParamWithId() {Id = (int)Enums.RFMode.LowDistortion, Parameter = "LOWD"},
//                },
//                ATTFix = false,
//                AttMax = 70,
//                AttStep = 1,

//                DFSQUMAX = 130,
//                DFSQUMIN = -50,
//                DFMeasTimeMAX = 10000,
//                DFMeasTimeMIN = 0.1m,

//                RefLevelMAX = 130,
//                RefLevelMIN = -29,
//                RangeLevelMAX = 200,
//                RangeLevelMIN = 10,
//                RangeLevelStep = 1,
//                DFSQUModes = new List<ParamWithId>()
//                {
//                    new ParamWithId() {Id = (int)Enums.DFSQUMode.Off, Parameter = "OFF"},
//                    new ParamWithId() {Id = (int)Enums.DFSQUMode.Gate, Parameter = "GATE"},
//                    new ParamWithId() {Id = (int)Enums.DFSQUMode.Normal, Parameter = "NORM"},
//                },
//                MeasTimeMAX = 0.0005M,
//                MeasTimeMIN = 900,
//            }
//            #endregion
//        };
//        #region Run
//        private bool _IsRuningTCP;
//        private bool IsRuningTCP
//        {
//            get { return _IsRuningTCP; }
//            set
//            {
//                _IsRuningTCP = value;
//                if (_IsRuningTCP && _IsRuningUDP)
//                { IsRuning = true; }
//                else { IsRuning = false; }
//            }
//        }
//        private bool _IsRuningUDP;
//        private bool IsRuningUDP
//        {
//            get { return _IsRuningUDP; }
//            set
//            {
//                _IsRuningUDP = value;
//                if (_IsRuningTCP && _IsRuningUDP)
//                { IsRuning = true; }
//                else { IsRuning = false; }
//            }
//        }
//        private bool IsRuning;
//        private bool DataCycle;

//        #endregion
//        private LocalRSReceiverInfo.Mode _Mode = new LocalRSReceiverInfo.Mode { ModeName = "FFM", MeasAppl = "RX", FreqMode = "CW" };
//        private LocalRSReceiverInfo.Mode Mode
//        {
//            get { return _Mode; }
//            set
//            {
//                LastMode = _Mode;
//                _Mode = value;
//                if (_Mode.ModeName != "PSCAN")
//                { PScanRun = false; }
//                SetStreamingMode();
//            }
//        }
//        private LocalRSReceiverInfo.Mode LastMode;

//        #region Level Out
//        private decimal RefLevel = 80;

//        private decimal Range = 110;

//        private decimal LowestLevel = -30;
//        #endregion
//        /// <summary>
//        /// true = CONT, false = PER
//        /// </summary>
//        private bool MeasMode;

//        private decimal MeasTime
//        {
//            get { return _MeasTime; }
//            set { if (value >= 0.0005M && MeasTime <= 900) _MeasTime = value; }
//        }
//        private decimal _MeasTime = 0.0005M;
//        private bool MeasTimeAuto;
//        private bool AFCState;
//        private ParamWithId SelectivityMode = new ParamWithId() { Id = (int)Enums.SelectivityMode.Auto, Parameter = "AUTO" };

//        #region Demode
//        private int DemodBWInd
//        {
//            get { return _DemodBWInd; }
//            set
//            {
//                if (value < 0) _DemodBWInd = UniqueData.DemodBW.Length - 1;
//                else if (value > UniqueData.DemodBW.Length - 1) _DemodBWInd = 0;
//                else _DemodBWInd = value;
//                DemodBW = UniqueData.DemodBW[_DemodBWInd];
//            }
//        }
//        private int _DemodBWInd = 0;

//        private decimal DemodBW = 0;

//        private int DemodInd
//        {
//            get { return _DemodInd; }
//            set
//            {
//                if (value < 0) _DemodInd = UniqueData.Demod.Length - 1;
//                else if (value > UniqueData.Demod.Length - 1) _DemodInd = 0;
//                else _DemodInd = value;
//                Demod = UniqueData.Demod[_DemodInd];
//            }
//        }
//        private int _DemodInd = 0;

//        private string Demod = "";
//        #endregion Demod

//        #region Detector
//        private int DetectorInd
//        {
//            get { return _DetectorInd; }
//            set
//            {
//                if (value < 0) _DetectorInd = UniqueData.Detectors.Count - 1;
//                else if (value > UniqueData.Detectors.Count - 1) _DetectorInd = 0;
//                else _DetectorInd = value;
//                Detector = UniqueData.Detectors[_DetectorInd];
//            }
//        }
//        private int _DetectorInd = 0;

//        private ParamWithId Detector = new ParamWithId() { Id = (int)Enums.Detector.Average, Parameter = "PAV" };
//        #endregion Detector

//        #region ATT
//        private bool ATTFixState;

//        private int ATT
//        {
//            get { return _ATT; }
//            set
//            {
//                if (value < 0) { _ATT = UniqueData.AttMax; }
//                else if (value > UniqueData.AttMax) { _ATT = 0; }
//                else _ATT = value;
//            }
//        }
//        private int _ATT = 0;

//        private bool ATTAutoState;
//        #endregion

//        #region MGC
//        private int MGC
//        {
//            get { return _MGC; }
//            set
//            {
//                _MGC = value;
//                if (_MGC < -30) { _MGC = -30; }
//                else if (_MGC > 110) { _MGC = 110; }
//            }
//        }
//        private int _MGC = 0;

//        private bool MGCAutoState = false;
//        #endregion MGC

//        #region SQU
//        private int SQU
//        {
//            get { return _SQU; }
//            set
//            {
//                _SQU = value;
//                if (_SQU < -30) { _SQU = -30; }
//                else if (_SQU > 130) { _SQU = 130; }
//            }
//        }
//        private int _SQU = 0;
//        private bool SQUState;
//        #endregion SQU

//        #region FFM
//        private ParamWithId FFMFFTMode = new ParamWithId { Id = (int)Enums.FFTMode.ClearWrite, Parameter = "OFF" };

//        #region FreqCentr
//        private decimal FFMFreqCentr = 100000000;
//        public decimal FFMFreqCentrToSet
//        {
//            get { return _FFMFreqCentrToSet; }
//            set
//            {
//                if (FFMFreqCentrToSet < UniqueData.FreqMin) _FFMFreqCentrToSet = UniqueData.FreqMin;
//                else if (FFMFreqCentrToSet > UniqueData.FreqMax) _FFMFreqCentrToSet = UniqueData.FreqMax;
//                else _FFMFreqCentrToSet = value;
//            }
//        }
//        private decimal _FFMFreqCentrToSet = 100000000;
//        #endregion FreqCentr

//        #region FreqSpan
//        private int FFMFreqSpanInd
//        {
//            get { return _FFMFreqSpanInd; }
//            set { if (value > -1 && value < UniqueData.FFMSpan.Length) _FFMFreqSpanInd = value; FFMFreqSpan = UniqueData.FFMSpan[_FFMFreqSpanInd].BW; }
//        }
//        private int _FFMFreqSpanInd = 1;

//        private int FFMFreqSpanIndToSet
//        {
//            get { return _FFMFreqSpanIndToSet; }
//            set { if (value > -1 && value < UniqueData.FFMSpan.Length) _FFMFreqSpanIndToSet = value; }
//        }
//        private int _FFMFreqSpanIndToSet = 1;

//        private decimal FFMFreqSpan
//        {
//            get { return _FFMFreqSpan; }
//            set { _FFMFreqSpan = value; FreqSpan = FFMFreqSpan; }
//        }
//        private decimal _FFMFreqSpan = 10000000;
//        #endregion FreqSpan

//        #region FFMStep
//        private int FFMStepInd
//        {
//            get { return _FFMStepInd; }
//            set
//            {
//                if (value > -1 && value < UniqueData.AllStepBW.Length)
//                {
//                    _FFMStepInd = value;
//                    FFMStep = UniqueData.AllStepBW[FFMStepInd];
//                }
//            }
//        }
//        private int _FFMStepInd = 1;

//        private int FFMStepIndToSet
//        {
//            get { return _FFMStepIndToSet; }
//            set { if (value > -1 && value < UniqueData.AllStepBW.Length) _FFMStepIndToSet = value; }
//        }
//        private int _FFMStepIndToSet = 1;

//        private decimal FFMStep = 0;

//        private bool FFMStepAuto;
//        #endregion FFMStep



//        #region Level

//        public decimal FFMStrengthLevel = -1;

//        public decimal FFMRefLevel
//        {
//            get { return _FFMRefLevel; }
//            set
//            {
//                if (value > UniqueData.RefLevelMAX) _FFMRefLevel = UniqueData.RefLevelMAX;
//                else if (value < UniqueData.RefLevelMIN) _FFMRefLevel = UniqueData.RefLevelMIN;
//                else _FFMRefLevel = value;
//                _FFMLowestLevel = _FFMRefLevel - FFMRangeLevel;
//                SetLevelFromMode();
//            }
//        }
//        private decimal _FFMRefLevel = 80;

//        public decimal FFMRangeLevel
//        {
//            get { return _FFMRangeLevel; }
//            set
//            {
//                if (value > UniqueData.RangeLevelMAX) { FFMRangeLevel = UniqueData.RangeLevelMAX; }
//                else if (value < UniqueData.RangeLevelMIN) { FFMRangeLevel = UniqueData.RangeLevelMIN; }
//                else _FFMRangeLevel = value;
//                _FFMLowestLevel = _FFMRefLevel - FFMRangeLevel;
//                SetLevelFromMode();
//            }
//        }
//        private decimal _FFMRangeLevel = 110;

//        public decimal FFMLowestLevel
//        {
//            get { return _FFMLowestLevel; }
//            set { _FFMLowestLevel = value; }
//        }
//        private decimal _FFMLowestLevel = -30;
//        #endregion
//        #endregion FFM

//        #region PSCAN
//        private ParamWithId PScanFFTMode = new ParamWithId { Id = (int)Enums.FFTMode.ClearWrite, Parameter = "OFF" };
//        private bool FinishPscan = false;
//        #region Freq
//        public decimal PScanFreqCentr
//        {
//            get { return _PScanFreqCentr; }
//            set
//            {
//                if (value < UniqueData.FreqMin) _PScanFreqCentr = UniqueData.FreqMin;
//                else if (value > UniqueData.FreqMax) _PScanFreqCentr = UniqueData.FreqMax;
//                else _PScanFreqCentr = value;
//                _PScanFreqStart = _PScanFreqCentr - _PScanFreqSpan / 2;
//                _PScanFreqStop = _PScanFreqCentr + _PScanFreqSpan / 2;
//            }
//        }
//        private decimal _PScanFreqCentr = 950000000;

//        public decimal PScanFreqSpan
//        {
//            get { return _PScanFreqSpan; }
//            set
//            {
//                _PScanFreqSpan = value;
//                if (PScanFreqCentr - value / 2 < UniqueData.FreqMin) PScanFreqCentr = (PScanFreqCentr - UniqueData.FreqMin) * 2;
//                else if (PScanFreqCentr + value / 2 > UniqueData.FreqMax) PScanFreqCentr = (UniqueData.FreqMax - PScanFreqCentr) * 2;
//                _PScanFreqStart = _PScanFreqCentr - _PScanFreqSpan / 2;
//                _PScanFreqStop = _PScanFreqCentr + _PScanFreqSpan / 2;
//            }
//        }
//        private decimal _PScanFreqSpan = 5000000;

//        public decimal PScanFreqStart
//        {
//            get { return _PScanFreqStart; }
//            set
//            {
//                if (value < UniqueData.FreqMin) _PScanFreqStart = UniqueData.FreqMin;
//                else if (value > UniqueData.FreqMax) _PScanFreqStart = UniqueData.FreqMax;
//                else _PScanFreqStart = value;
//                _PScanFreqCentr = (_PScanFreqStart + _PScanFreqStop) / 2;
//                _PScanFreqSpan = _PScanFreqStop - _PScanFreqStart;
//            }
//        }
//        private decimal _PScanFreqStart = 947500000;

//        public decimal PScanFreqStop
//        {
//            get { return _PScanFreqStop; }
//            set
//            {
//                if (value < UniqueData.FreqMin) _PScanFreqStop = UniqueData.FreqMin;
//                else if (value > UniqueData.FreqMax) _PScanFreqStop = UniqueData.FreqMax;
//                else _PScanFreqStop = value;
//                _PScanFreqCentr = (_PScanFreqStart + _PScanFreqStop) / 2;
//                _PScanFreqSpan = _PScanFreqStop - _PScanFreqStart;
//            }
//        }
//        private decimal _PScanFreqStop = 952500000;
//        #endregion Freq

//        private bool _PScanRun = false;
//        public bool PScanRun
//        {
//            get { return _PScanRun; }
//            set
//            {
//                _PScanRun = value;
//                if (_PScanRun)
//                {
//                    SetPScanRun();
//                }
//                else if (!_PScanRun)
//                {
//                    SetPScanStop();
//                }
//            }
//        }

//        public int PScanStepInd
//        {
//            get { return _PScanStepInd; }
//            set
//            {
//                if (value < 0) _PScanStepInd = 0;
//                else if (value > UniqueData.PSCANStepBW.Length - 1) _PScanStepInd = UniqueData.PSCANStepBW.Length - 1;
//                else _PScanStepInd = value;
//                PScanStep = UniqueData.PSCANStepBW[_PScanStepInd];
//            }
//        }
//        private int _PScanStepInd;

//        private decimal PScanStep = 0;

//        #region Level 
//        public decimal PScanRefLevel
//        {
//            get { return _PScanRefLevel; }
//            set
//            {
//                if (value > UniqueData.RefLevelMAX) _PScanRefLevel = UniqueData.RefLevelMAX;
//                else if (value < UniqueData.RefLevelMIN) _PScanRefLevel = UniqueData.RefLevelMIN;
//                else _PScanRefLevel = value;
//                PScanLowestLevel = _PScanRefLevel - PScanRangeLevel;
//                SetLevelFromMode();
//            }
//        }
//        private decimal _PScanRefLevel = 80;

//        public decimal PScanRangeLevel
//        {
//            get { return _PScanRangeLevel; }
//            set
//            {
//                if (value > UniqueData.RangeLevelMAX) { PScanRangeLevel = UniqueData.RangeLevelMAX; }
//                else if (value < UniqueData.RangeLevelMIN) { PScanRangeLevel = UniqueData.RangeLevelMIN; }
//                else _PScanRangeLevel = value;
//                PScanLowestLevel = _PScanRefLevel - PScanRangeLevel;
//                SetLevelFromMode();
//            }
//        }
//        private decimal _PScanRangeLevel = 110;

//        private decimal PScanLowestLevel = -30;
//        #endregion
//        #endregion PSCAN

//        #region DF
//        private decimal DFMeasureTime = -1;
//        private decimal DFSquelchValue = -1;
//        private decimal DFBandwidth = -1;
//        private int _DFBandwidthInd = 1;
//        private bool DFBandwidthAuto = false;

//        #endregion DF

//        TracePoint[] TraceTemp = new TracePoint[1601];
//        #endregion

//        private bool Connect(string iPAddress, int tCPPort)
//        {
//            bool res = true;
//            IPAddress = iPAddress;
//            TCPPort = tCPPort;

//            if (IPAddress != "")
//            {
//                if (TCPPort != 0)
//                {
//                    tc = new CommandConnection();
//                    tc.Open(IPAddress, TCPPort);
//                    if (tc.IsOpen)
//                    {
//                        string[] temp = tc.Query("*IDN?").Trim('"').Replace(" ", "").ToUpper().Split(',');
//                        if (temp[0].ToLower() == "rohde&schwarz") Manufacrure = 1;
//                        else if (temp[0] == "Keysight Technologies") Manufacrure = 2;
//                        else if (temp[0] == "Anritsu") Manufacrure = 3;
//                        Model = temp[1];
//                        SerialNumber = temp[2];
//                        string[] op = tc.Query("*OPT?").TrimEnd().Replace(" ", "").ToUpper().Split(',');
//                        foreach (LocalRSReceiverInfo a in AllUniqueData)
//                        {
//                            if (a.InstrManufacrure == Manufacrure)
//                            {
//                                if (Model.ToLower().Contains(a.InstrModel.ToLower()))
//                                {
//                                    List<DeviceOption> Loaded = new List<DeviceOption>() { };
//                                    if (op.Length > 0)
//                                    {
//                                        foreach (string s in op)
//                                        {
//                                            foreach (DeviceOption io in a.InstrOption)
//                                            {
//                                                if (io.Type.ToLower() == s.ToLower())
//                                                    Loaded.Add(io);
//                                            }
//                                        }
//                                    }
//                                    a.LoadedInstrOption = Loaded;
//                                    foreach (DeviceOption io in a.LoadedInstrOption)
//                                    {
//                                        if (io.Type.ToLower() == "WB")
//                                        {

//                                            List<LocalRSReceiverInfo.FFMSpanBW> bw = a.FFMSpan.ToList();
//                                            bw.Add(new LocalRSReceiverInfo.FFMSpanBW() { BW = 40000000, AvailableStepBW = new decimal[] { 12500, 20000, 25000, 50000, 100000, 200000, 500000, 1000000, 2000000 } });
//                                            bw.Add(new LocalRSReceiverInfo.FFMSpanBW() { BW = 80000000, AvailableStepBW = new decimal[] { 25000, 50000, 100000, 200000, 500000, 1000000, 2000000 } });
//                                            a.FFMSpan = bw.ToArray();
//                                        }
//                                    }
//                                    UniqueData = a;
//                                }
//                            }
//                        }

//                        UniqueData.FreqMax = decimal.Parse(tc.Query("SENS:FREQ? MAX").Replace('.', ','));
//                        UniqueData.FreqMin = decimal.Parse(tc.Query("SENS:FREQ? MIN").Replace('.', ','));

//                        foreach (System.Net.NetworkInformation.NetworkInterface ni in System.Net.NetworkInformation.NetworkInterface.GetAllNetworkInterfaces())
//                        {
//                            if (ni.NetworkInterfaceType == System.Net.NetworkInformation.NetworkInterfaceType.Ethernet)
//                            {
//                                foreach (System.Net.NetworkInformation.UnicastIPAddressInformation ipinf in ni.GetIPProperties().UnicastAddresses)
//                                {
//                                    if (ipinf.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
//                                    {
//                                        if (ipinf.Address.ToString().Substring(0, ipinf.Address.ToString().LastIndexOf('.')) == IPAddress.Substring(0, IPAddress.LastIndexOf('.')))
//                                        {
//                                            MyIPAddress = ipinf.Address.ToString();
//                                        }
//                                    }
//                                }
//                            }
//                        }
//                        tc.WriteLine("TRAC:UDP:DEL \"" + MyIPAddress + "\", " + UDPPort.ToString());
//                        tc.WriteLine("FORM:DATA PACK;:FORM:MEM PACK");

//                        Thread.Sleep(100);
//                        GetSettingsOnConnecting();
//                        Thread.Sleep(100);

//                        uc = new UdpStreaming();
//                        uc.Open(MyIPAddress, UDPPort);

//                        UdpDM = SameWorkUDP;
//                        UdpDM += ReaderStream;
//                        UdpTr = new Thread(AllUdpTimeWorks);
//                        UdpTr.Name = "RsReceiverUdpThread";
//                        UdpTr.IsBackground = true;

//                        UdpTr.Start();
//                        SetStreamingMode();
//                        IsRuning = true;
//                    }
//                    else
//                    {
//                        res = false;
//                    }
//                }
//                else
//                {
//                    IsRuning = false;
//                    res = false;
//                }
//            }
//            else
//            {
//                IsRuning = false;
//                res = false;
//            }
//            return res;
//        }

//        private void SameWorkUDP()
//        {
//            try
//            {
//                Thread.Sleep(1);
//            }
//            catch { }
//        }
//        private void AllUdpTimeWorks()
//        {
//            while (DataCycle)
//            {
//                if (UdpDM != null)
//                    UdpDM();
//            }
//            uc.Close();
//            UdpTr.Abort();
//            IsRuningUDP = false;
//        }

//        private void ReaderStream()
//        {
//            Thread.Sleep(new TimeSpan(10));
//            if (uc.IsOpen && DataCycle)
//            {
//                Byte[] t = uc.ByteRead();
//                if (t.Length > 0)
//                {

//                    UInt16 streammode = (UInt16)((t[16] << 8) | (t[17]));

//                    UInt16 HeaderLength = 28;
//                    UInt16 optHeaderLength = (UInt16)((t[22] << 8) | (t[23]));
//                    UInt16 traceDataFrom = (UInt16)(optHeaderLength + HeaderLength);
//                    UInt16 traceDataLength = (UInt16)((t[20] << 8) | (t[21]));
//                    FinishPscan = false;


//                    if (streammode == 401)//Audio
//                    {

//                    }
//                    else if (streammode == 501)//FFM IFPan
//                    {
//                        #region
//                        byte[] bfreq = new byte[8]; Array.Copy(t, 28, bfreq, 0, 4); Array.Copy(t, 44, bfreq, 4, 4);
//                        UInt64 freqCentr = BitConverter.ToUInt64(bfreq, 0);
//                        UInt32 freqSpan = BitConverter.ToUInt32(t, 32);
//                        decimal freqstart = freqCentr - (freqSpan / 2);
//                        decimal freqstop = freqCentr + (freqSpan / 2);
//                        decimal step = freqSpan / ((Int16)((t[20] << 8) | (t[21])) - 1);
//                        for (int i = 0; i < UniqueData.AllStepBW.Length; i++)
//                        {
//                            if (step == (int)UniqueData.AllStepBW[i])
//                            { step = UniqueData.AllStepBW[i]; }
//                        }
//                        if (freqstart != TraceTemp[0].Freq || TraceTemp.Length != traceDataLength || freqstop != TraceTemp[TraceTemp.Length - 1].Freq || step != (TraceTemp[1].Freq - TraceTemp[0].Freq))
//                        {
//                            TraceTemp = new TracePoint[traceDataLength];
//                            for (int i = 0; i < TraceTemp.Length; i++)
//                            {
//                                TracePoint p = new TracePoint()
//                                {
//                                    Freq = freqstart + step * i,
//                                    Level = (double)((Int16)((t[i * 2 + traceDataFrom + 1] << 8) | (t[i * 2 + traceDataFrom]))) / 10
//                                };
//                                TraceTemp[i] = p;
//                            }
//                        }
//                        else
//                        {
//                            #region without reset
//                            for (int i = 0; i < TraceTemp.Length; i++)
//                            {
//                                TraceTemp[i].Level = (double)((Int16)((t[i * 2 + traceDataFrom + 1] << 8) | (t[i * 2 + traceDataFrom]))) / 10;
//                            }
//                            #endregion
//                        }

//                        //FFMStrengthLevel = Math.Round(LM.MeasChannelPower(temp, freqCentr, DemodBW) + 107, 2); //(decimal)((Int16)((t[91] << 8) | (t[90])));
//                        //Trace1 = temp;
//                        if (FFMFreqCentr != freqCentr || FreqCentr != freqCentr) { FFMFreqCentr = freqCentr; FreqCentr = freqCentr; }
//                        if (FFMFreqSpan != freqSpan || FreqSpan != freqSpan)
//                        {
//                            if (System.Array.IndexOf(UniqueData.FFMSpanBW, freqSpan) > 0)
//                            { FFMFreqSpanInd = System.Array.IndexOf(UniqueData.FFMSpanBW, freqSpan); FreqSpan = freqSpan; }
//                        }
//                        if (UniqueData.AllStepBW[FFMStepInd] != step) { FFMStepInd = System.Array.IndexOf(UniqueData.AllStepBW, step); }
//                        SetTraceData(TraceTemp, step, true);
//                        IsRuningUDP = true;
//                        //Temp = System.Text.Encoding.UTF8.GetString(t);
//                        #endregion
//                    }
//                    else if (streammode == 1201)//PSCan
//                    {
//                        #region                        
//                        decimal freqStep = BitConverter.ToUInt32(t, 36);
//                        for (int i = 0; i < UniqueData.PSCANStepBW.Length; i++)
//                        {
//                            if (freqStep == (int)UniqueData.PSCANStepBW[i])
//                            { freqStep = UniqueData.PSCANStepBW[i]; PScanStepInd = i; }
//                        }
//                        byte[] bstart = new byte[8]; Array.Copy(t, 28, bstart, 0, 4); Array.Copy(t, 40, bstart, 4, 4);
//                        byte[] bstop = new byte[8]; Array.Copy(t, 32, bstop, 0, 4); Array.Copy(t, 44, bstop, 4, 4);
//                        UInt64 start = BitConverter.ToUInt64(bstart, 0);
//                        UInt64 stop = BitConverter.ToUInt64(bstop, 0);
//                        Int16 shift = (Int16)((t[traceDataLength * 2 + traceDataFrom - 1] << 8) | (t[traceDataLength * 2 + traceDataFrom - 2]));
//                        int ind = traceDataLength;
//                        if (shift == 2000)
//                        {
//                            FinishPscan = true;
//                            ind--;
//                        }
//                        int points = (int)(((stop - start) / freqStep) + 1);

//                        #region set level data on this band                       
//                        #region reset trace freq
//                        if (start != TraceTemp[0].Freq || TraceTemp.Length != points || stop != TraceTemp[TraceTemp.Length - 1].Freq || freqStep != (TraceTemp[1].Freq - TraceTemp[0].Freq))
//                        {
//                            TraceTemp = new TracePoint[points];
//                            for (int i = 0; i < points; i++)
//                            {
//                                TracePoint p = new TracePoint()
//                                {
//                                    Freq = start + (UInt64)freqStep * (UInt64)i,
//                                    Level = -1000
//                                };
//                                TraceTemp[i] = p;
//                            }
//                        }
//                        #endregion
//                        int itd = traceDataFrom + traceDataLength * 2;
//                        int itu = traceDataFrom + traceDataLength * 2 + traceDataLength * 4;
//                        byte[] bfreq = new byte[8];
//                        Array.Copy(t, itd, bfreq, 0, 4);
//                        Array.Copy(t, itu, bfreq, 4, 4);
//                        decimal freq = BitConverter.ToUInt64(bfreq, 0);
//                        int freqindex = 0;
//                        for (int j = 0; j < TraceTemp.Length; j++)
//                        {
//                            if (TraceTemp[j].Freq == freq) freqindex = j;
//                        }
//                        for (int i = 0; i < ind; i++)
//                        {
//                            if (freqindex < TraceTemp.Length)
//                            {
//                                TraceTemp[freqindex].Level = (double)((Int16)((t[i * 2 + traceDataFrom + 1] << 8) | (t[i * 2 + traceDataFrom]))) / 10;
//                                freqindex++;
//                            }
//                        }
//                        #endregion
//                        if (PScanFreq_CentrSpan_StartStop == false)
//                        {
//                            if (start != PScanFreqStart || FreqStart != start) { PScanFreqStart = start; FreqStart = start; }
//                            if (stop != PScanFreqStop || FreqStop != stop) { PScanFreqStop = stop; ; FreqStop = stop; }
//                        }
//                        else if (PScanFreq_CentrSpan_StartStop == true)
//                        {
//                            if ((stop + start) / 2 != PScanFreqCentr || (stop + start) / 2 != FreqCentr) { PScanFreqCentr = (stop + start) / 2; FreqCentr = (stop + start) / 2; }
//                            if (stop - start != PScanFreqSpan || stop - start != FreqSpan) { PScanFreqSpan = stop - start; FreqSpan = stop - start; }
//                        }

//                        SetTraceData(TraceTemp, freqStep, FinishPscan);// true); // temp[1].freq - temp[0].freq);
//                        IsRuningUDP = true;
//                        #endregion
//                    }
//                    else if (streammode == 1401)//DFPan
//                    {
//                        #region
//                        byte[] bfreq = new byte[8]; Array.Copy(t, 28, bfreq, 0, 4); Array.Copy(t, 32, bfreq, 4, 4);
//                        decimal freqCentr = (decimal)BitConverter.ToUInt64(bfreq, 0);
//                        decimal freqSpan = (decimal)BitConverter.ToUInt32(t, 36);

//                        if (freqCentr > 0 && freqSpan > 0)
//                        {
//                            decimal freqstart = freqCentr - freqSpan / 2;
//                            decimal freqstop = freqCentr + freqSpan / 2;
//                            decimal step = freqSpan / ((Int16)((t[20] << 8) | (t[21])) - 1);
//                            for (int i = 0; i < UniqueData.AllStepBW.Length; i++)
//                            {
//                                if (step == (int)UniqueData.AllStepBW[i])
//                                { step = UniqueData.AllStepBW[i]; }
//                            }

//                            if (UniqueData.InstrModel.Contains("EM100") || UniqueData.InstrModel.Contains("PR100") || UniqueData.InstrModel.Contains("DDF007"))
//                            {
//                                DFSQUModeIndex = (int)BitConverter.ToUInt32(t, 40);//++
//                                DFSquelchValue = (decimal)BitConverter.ToInt32(t, 44);//++
//                                if (!DFSquelchValueSliderState) { DFSquelchValueSlider = (int)DFSquelchValue; }
//                                DFBandwidth = (decimal)BitConverter.ToUInt32(t, 48);//++
//                                DFBandwidthInd = System.Array.IndexOf(UniqueData.DFBW, DFBandwidth);
//                                StepWidth = (decimal)BitConverter.ToUInt32(t, 52);
//                                DFMeasureTime = ((decimal)BitConverter.ToUInt32(t, 56)) / 1000;
//                                DFOption = (int)BitConverter.ToUInt32(t, 60);
//                                CompassHeading = ((decimal)BitConverter.ToUInt16(t, 64)) / 10;
//                                CompassHeadingType = (int)BitConverter.ToUInt16(t, 66);
//                                DFAntennaFactor = ((decimal)BitConverter.ToUInt32(t, 68)) / 10;
//                                DemodFreqChannel = (decimal)BitConverter.ToUInt32(t, 72);
//                                byte[] bDemodFreq = new byte[8]; Array.Copy(t, 76, bDemodFreq, 0, 4); Array.Copy(t, 80, bDemodFreq, 4, 4);
//                                DemodFreq = (decimal)BitConverter.ToUInt64(bDemodFreq, 0);
//                                UInt64 TimeStamp = BitConverter.ToUInt64(t, 84);
//                                DateTime dt = new DateTime(1970, 1, 1, 0, 0, 0, System.DateTimeKind.Utc);
//                                dt = dt.AddTicks((long)TimeStamp / 100);

//                                DFLevel = ((decimal)BitConverter.ToInt16(t, Ind)) / 10;

//                                DFQuality = ((decimal)((Int16)((t[1 + 136] << 8) | (t[0 + 136])))) / 10;// ((decimal)BitConverter.ToInt16(t, 136)) / 10;
//                                DFAzimuth = ((decimal)BitConverter.ToInt16(t, 134)) / 10;
//                                DFLevelStrength = DFLevel + DFAntennaFactor;
//                            }
//                            else if (UniqueData.InstrModel.Contains("DDF2") || UniqueData.InstrModel.Contains("EB5") || UniqueData.InstrModel == "ESMD")
//                            {
//                                #region
//                                DFSQUModeIndex = (int)BitConverter.ToUInt32(t, 40);//++
//                                DFSquelchValue = (decimal)BitConverter.ToInt32(t, 44);//++
//                                if (!DFSquelchValueSliderState) { DFSquelchValueSlider = (int)DFSquelchValue; }
//                                DFBandwidth = (decimal)BitConverter.ToUInt32(t, 48);//++
//                                DFBandwidthInd = System.Array.IndexOf(UniqueData.DFBW, DFBandwidth);
//                                StepWidth = (decimal)BitConverter.ToUInt32(t, 52);
//                                DFMeasureTime = ((decimal)BitConverter.ToUInt32(t, 56)) / 1000;
//                                DFOption = (int)BitConverter.ToUInt32(t, 60);
//                                CompassHeading = ((decimal)BitConverter.ToUInt16(t, 64)) / 10;
//                                CompassHeadingType = (int)BitConverter.ToUInt16(t, 66);
//                                DFAntennaFactor = ((decimal)BitConverter.ToUInt32(t, 68)) / 10;
//                                DemodFreqChannel = (decimal)BitConverter.ToUInt32(t, 72);
//                                byte[] bDemodFreq = new byte[8]; Array.Copy(t, 76, bDemodFreq, 0, 4); Array.Copy(t, 80, bDemodFreq, 4, 4);
//                                DemodFreq = (decimal)BitConverter.ToUInt64(bDemodFreq, 0);
//                                UInt64 TimeStamp = BitConverter.ToUInt64(t, 84);
//                                DateTime dt = new DateTime(1970, 1, 1, 0, 0, 0, System.DateTimeKind.Utc);
//                                dt = dt.AddTicks((long)TimeStamp / 100);
//                                DFLevel = ((decimal)BitConverter.ToInt16(t, 132)) / 10;
//                                //MainWindow.gps.UTCTime = dt;
//                                //MainWindow.gps.LocalTime = dt.ToLocalTime();

//                                //Azimuth = ((decimal)BitConverter.ToUInt16(t, traceDataFrom + traceDataLength * 2 + (traceDataLength / 2) * 2)) / 10;
//                                //decimal dfq = ((decimal)BitConverter.ToUInt16(t, traceDataFrom + (traceDataLength / 2) * 10 + 4)) / 10;
//                                decimal dfq = ((decimal)BitConverter.ToInt16(t, 136)) / 10;
//                                if (dfq < 0 || dfq > 100) DFQuality = 0;
//                                //if (dfq > 100) DFQuality = 0;
//                                else if (dfq >= 0 && dfq <= 100) DFQuality = dfq;
//                                if (dfq > DFQualitySQU)
//                                {
//                                    decimal az = ((decimal)BitConverter.ToInt16(t, 134)) / 10;
//                                    if (az >= 0 && az <= 360) { DFAzimuth = az; }
//                                }

//                                DFLevelStrength = DFLevel + DFAntennaFactor;
//                                #endregion
//                            }
//                            if (freqstart != TraceTemp[0].Freq || TraceTemp.Length != traceDataLength || freqstop != TraceTemp[TraceTemp.Length - 1].Freq || step != (TraceTemp[1].Freq - TraceTemp[0].Freq))
//                            {
//                                TraceTemp = new TracePoint[traceDataLength];
//                                for (int i = 0; i < TraceTemp.Length; i++)
//                                {
//                                    TracePoint p = new TracePoint()
//                                    {
//                                        Freq = freqstart + step * i,
//                                        Level = (double)((Int16)((t[i * 2 + traceDataFrom + 1] << 8) | (t[i * 2 + traceDataFrom]))) / 10
//                                    };
//                                    TraceTemp[i] = p;
//                                }
//                            }
//                            else
//                            {
//                                for (int i = 0; i < TraceTemp.Length; i++)
//                                {
//                                    TraceTemp[i].Level = (double)((Int16)((t[i * 2 + traceDataFrom + 1] << 8) | (t[i * 2 + traceDataFrom]))) / 10;
//                                }
//                            }

//                            //Trace1 = temp;
//                            if (FFMFreqCentr != freqCentr || FreqCentr != freqCentr) { FFMFreqCentr = freqCentr; FreqCentr = FFMFreqCentr; FreqStart = FFMFreqCentr - freqSpan / 2; FreqStop = FFMFreqCentr + freqSpan / 2; }
//                            if (FFMFreqSpan != freqSpan) { FFMFreqSpanInd = System.Array.IndexOf(UniqueData.FFMSpanBW, freqSpan); FreqSpan = freqSpan; FreqStart = FFMFreqCentr - freqSpan / 2; FreqStop = FFMFreqCentr + freqSpan / 2; }
//                            if (UniqueData.AllStepBW[FFMStepInd] != step) { FFMStepInd = System.Array.IndexOf(UniqueData.AllStepBW, step); }
//                            IsRuningUDP = true;
//                            SetTraceData(TraceTemp, step, true);
//                            //Temp = System.Text.Encoding.UTF8.GetString(t);
//                        }
//                        #endregion

//                    }
//                    else if (streammode == 1801)//GPS Data
//                    {
//                        #region
//                        MainWindow.gps.GPSIsValid = Convert.ToBoolean(BitConverter.ToUInt16(t, 40));
//                        if (MainWindow.gps.GPSIsValid)
//                        {
//                            try
//                            {
//                                MainWindow.gps.NumbSat = BitConverter.ToUInt16(t, 42);
//                                MainWindow.gps.Sats = MainWindow.gps.NumbSat.ToString();

//                                MainWindow.gps._LastLatitude = MainWindow.gps.LatitudeDecimal;
//                                MainWindow.gps._LastLongitude = MainWindow.gps.LongitudeDecimal;
//                                MainWindow.gps.LatitudeDecimal = (decimal)Math.Round(BitConverter.ToUInt16(t, 46) + BitConverter.ToSingle(t, 48) / 60, 6);
//                                MainWindow.gps.LongitudeDecimal = (decimal)Math.Round(BitConverter.ToUInt16(t, 54) + BitConverter.ToSingle(t, 56) / 60, 6); // + " " + tok[5];
//                                MainWindow.gps.LatitudeStr = BitConverter.ToUInt16(t, 46).ToString() + "° " + (int)BitConverter.ToSingle(t, 48) + "' " +
//                                    Math.Round((BitConverter.ToSingle(t, 48) - (int)BitConverter.ToSingle(t, 48)) * 60, 1) + "\" " + Convert.ToChar(BitConverter.ToUInt16(t, 44));

//                                MainWindow.gps.LongitudeStr = BitConverter.ToUInt16(t, 54).ToString() + "° " + (int)BitConverter.ToSingle(t, 56) + "' " +
//                                    Math.Round((BitConverter.ToSingle(t, 56) - (int)BitConverter.ToSingle(t, 56)) * 60, 1) + "\" " + Convert.ToChar(BitConverter.ToUInt16(t, 52));

//                                double dist = 0, ang = 0;
//                                MainWindow.help.calcDistance((double)MainWindow.gps._LastLatitude, (double)MainWindow.gps._LastLongitude, (double)MainWindow.gps.LatitudeDecimal, (double)MainWindow.gps.LongitudeDecimal, out dist, out ang);
//                                MainWindow.gps.AngleCourse = ang;
//                                MainWindow.gps.Horizontaldilution = BitConverter.ToSingle(t, 60);
//                                UInt64 TimeStamp = BitConverter.ToUInt64(t, 28);
//                                DateTime dt = new DateTime(1970, 1, 1, 0, 0, 0, System.DateTimeKind.Utc);
//                                dt = dt.AddTicks((long)TimeStamp / 100);
//                                MainWindow.gps.UTCTime = dt;
//                                MainWindow.gps.LocalTime = dt.ToLocalTime();
//                                bool AntennaValid = Convert.ToBoolean(BitConverter.ToUInt16(t, 64));//+
//                                bool AntennaTiltOver = Convert.ToBoolean(BitConverter.ToUInt16(t, 66));//+
//                                int AntennaElevation = BitConverter.ToInt16(t, 68);//+
//                                int AntennaRoll = BitConverter.ToInt16(t, 70);//+
//                                double CompassHeading = ((double)BitConverter.ToUInt16(t, 36)) / 10;//+
//                                int CompassHeadingType = BitConverter.ToInt16(t, 38);//+
//                            }
//                            catch { }
//                        }

//                        #endregion
//                    }


//                }

//            }
//        }

//        public void GetSettingsOnConnecting()
//        {
//            if (tc.IsOpen)
//            {
//                string receive = "";
//                try
//                {
//                    if (UniqueData.InstrModel == "EM100" || UniqueData.InstrModel == "PR100" || UniqueData.InstrModel == "DDF007")
//                    {
//                        #region
//                        Thread.Sleep(10);
//                        string send = ":DISPlay:IFPan:LEVel:REFerence?" +
//                            ";:DISPlay:IFPan:LEVel:RANGe?" +
//                            ";:DISPl:PSCAN:LEV:REF?" +
//                            ";:DISPlay:PSCAN:LEVel:RANGe?" +
//                            ";:FREQ?" +
//                            ";:FREQ:SPAN?" +
//                            ";:CALC:IFP:AVER:TYPE?" +
//                            ";:CALC:PSC:AVER:TYPE?" +
//                            ";:MEAS:MODE?" +
//                            ";:CALC:IFP:SEL?" +
//                            ";:MEAS:TIME?" +
//                            ";:FREQ:PSC:STAR?" +
//                            ";:FREQ:PSC:STOP?" +
//                            ";:FREQ:PSC:CENT?" +
//                            ";:FREQ:PSC:SPAN?" +
//                            ";:PSC:STEP?" +
//                            ";:BAND?" +
//                            ";:DEM?" +
//                            ";:DET?" +
//                            ";:FREQ:AFC?" +
//                            ";:GCON?" +
//                            ";:GCON:MODE?" +
//                            ";:OUTP:SQU:THR?" +
//                            ";:OUTP:SQU?" +
//                            ";:INPut:ATTenuation:STATe?";

//                        receive = tc.Query(send).Replace('.', ',').TrimEnd();

//                        string[] data = receive.Split(';');
//                        FFMRefLevel = decimal.Parse(data[0]);
//                        FFMRangeLevel = decimal.Parse(data[1]);
//                        PScanRefLevel = decimal.Parse(data[2]);
//                        PScanRangeLevel = decimal.Parse(data[3]);
//                        FFMFreqCentr = decimal.Parse(data[4]);
//                        decimal d = decimal.Parse(data[5]);
//                        for (int i = 0; i < UniqueData.FFMSpan.Length; i++)
//                        {
//                            if (UniqueData.FFMSpan[i].BW == d)
//                            {
//                                FFMFreqSpanInd = i;
//                                break;
//                            }
//                        }
//                        FFMStepAuto = true;
//                        for (int i = 0; i < UniqueData.FFTModes.Count; i++)
//                        {
//                            if (data[6].Contains(UniqueData.FFTModes[i].Parameter)) { FFMFFTMode = UniqueData.FFTModes[i]; }///запилить проверку после изменения
//                            if (data[7].Contains(UniqueData.FFTModes[i].Parameter)) { PScanFFTMode = UniqueData.FFTModes[i]; }///запилить проверку после изменения
//                        }
//                        if (data[8].Contains("CONT")) { MeasMode = true; }
//                        else if (data[8].Contains("PER")) { MeasMode = false; }
//                        if (data[9].Contains("DEF")) { MeasTime = 0.0005m; MeasTimeAuto = true; }
//                        else { MeasTime = decimal.Parse(data[9]); }

//                        PScanFreqStart = decimal.Parse(data[10]);
//                        PScanFreqStop = decimal.Parse(data[11]);
//                        PScanFreqCentr = decimal.Parse(data[12]);
//                        PScanFreqSpan = decimal.Parse(data[13]);
//                        PScanStepInd = System.Array.IndexOf(UniqueData.PSCANStepBW, decimal.Parse(data[14]));
//                        DemodBW = Decimal.Parse(data[15]);
//                        for (int i = 0; i < UniqueData.DemodBW.Length; i++)
//                        {
//                            if (DemodBW == UniqueData.DemodBW[i]) { DemodBWInd = i; }
//                        }
//                        for (int i = 0; i < UniqueData.Demod.Length; i++)
//                        {
//                            if (data[16].ToUpper() == UniqueData.Demod[i]) { DemodInd = i; }
//                        }
//                        for (int i = 0; i < UniqueData.Detectors.Count; i++)
//                        {
//                            if (data[17].ToUpper() == UniqueData.Detectors[i].Parameter) { DetectorInd = i; }//запилить для каждой железки
//                        }
//                        if (data[18].Contains("1")) { AFCState = true; }
//                        else if (data[18].Contains("0")) { AFCState = false; }
//                        MGC = Int32.Parse(data[19]);
//                        if (data[20].Contains("AUTO")) { MGCAutoState = true; }
//                        else if (data[20].Contains("FIX")) { MGCAutoState = false; }
//                        SQU = Int32.Parse(data[21]);
//                        if (data[22].Contains("1")) { SQUState = true; }
//                        else if (data[22].Contains("0")) { SQUState = false; }
//                        if (data[23].Contains("1")) { ATTFixState = true; }
//                        else if (data[23].Contains("0")) { ATTFixState = false; }

//                        //tc.WriteLine("system:audio:remote:mode " + AudioFormat.Id);
//                        FreqStart = FFMFreqCentr - FFMFreqSpan / 2;
//                        FreqStop = FFMFreqCentr + FFMFreqSpan / 2;
//                        #endregion
//                    }
//                    if (UniqueData.InstrModel.Contains("DDF2") || UniqueData.InstrModel.Contains("EB5") || UniqueData.InstrModel.Contains("ESMD") || UniqueData.InstrModel.Contains("DDF550"))
//                    {
//                        #region
//                        Thread.Sleep(10);
//                        string send = ":CALCulate:PIFPan:RLEVel?;" + //++
//                            ":CALCulate:PIFPan:LRANge?;" +
//                            ":FREQ?;" +
//                            ":FREQ:SPAN?;" +
//                            ":CALC:IFP:STEP?;" +
//                            ":CALC:IFP:STEP:AUTO?;" +
//                            ":CALC:IFP:AVER:TYPE?;" +
//                            ":CALC:PSC:AVER:TYPE?;" +
//                            ":MEAS:MODE?;" +
//                            ":CALC:IFP:SEL?;" +
//                            ":MEAS:TIME?;" +
//                            ":MEASure:DFINder:TIME?;" +
//                            ":FREQ:PSC:STAR?;" +
//                            ":FREQ:PSC:STOP?;" +
//                            ":FREQ:PSC:CENT?;" +
//                            ":FREQ:PSC:SPAN?;" +
//                            ":PSC:STEP?;" +
//                            ":BAND?;" +
//                            ":DEM?;" +
//                            ":DET?;" +
//                            ":FREQ:AFC?;" +
//                            ":GCON?;" +
//                            ":GCON:MODE?;" +
//                            ":OUTP:SQU:THR?;" +
//                            ":OUTP:SQU?;" +
//                            ":MEASure:DF:THReshold?;" +
//                            ":BAND:DF:RES:AUTO?;" +
//                            ":INP:ATT:AUTO?;" +
//                            ":INP:ATT?;";
//                        receive = tc.Query(send).Replace('.', ',').TrimEnd();
//                        string[] data = receive.Split(';');

//                        #region
//                        FFMRefLevel = decimal.Parse(data[0]);
//                        FFMRangeLevel = decimal.Parse(data[1]);
//                        FFMFreqCentr = decimal.Parse(data[2]);
//                        decimal d = decimal.Parse(data[3]);
//                        for (int i = 0; i < UniqueData.FFMSpan.Length; i++)
//                        {
//                            if (UniqueData.FFMSpan[i].BW == d)
//                            {
//                                FFMFreqSpanInd = i;
//                                break;
//                            }
//                        }
//                        FFMStepInd = System.Array.IndexOf(UniqueData.AllStepBW, decimal.Parse(data[4]));
//                        if (data[5] == "1") { FFMStepAuto = true; }
//                        else if (data[5] == "0") { FFMStepAuto = false; }
//                        for (int j = 0; j < UniqueData.FFTModes.Count; j++)
//                        {
//                            if (data[6].Contains(UniqueData.FFTModes[j].Parameter)) { FFMFFTMode = UniqueData.FFTModes[j]; }///запилить проверку после изменения
//                            if (data[6].Contains(UniqueData.FFTModes[j].Parameter)) { PScanFFTMode = UniqueData.FFTModes[j]; }///запилить проверку после изменения
//                        }

//                        if (data[7].Contains("CONT")) { MeasMode = true; }
//                        else if (data[7].Contains("PER")) { MeasMode = false; }
//                        for (int j = 0; j < UniqueData.SelectivityModes.Count; j++)
//                        {
//                            if (data[8].Contains(UniqueData.SelectivityModes[j].Parameter)) { SelectivityMode = UniqueData.SelectivityModes[j]; }
//                        }

//                        string ms = data[9];
//                        if (ms.Contains("DEF")) { MeasTime = 0.0005m; MeasTimeAuto = true; }
//                        else { MeasTime = decimal.Parse(ms); }
//                        DFMeasureTime = decimal.Parse(data[10]);
//                        PScanFreqStart = decimal.Parse(data[11]);
//                        PScanFreqStop = decimal.Parse(data[12]);
//                        PScanFreqCentr = decimal.Parse(data[13]);
//                        PScanFreqSpan = decimal.Parse(data[14]);
//                        PScanStepInd = System.Array.IndexOf(UniqueData.PSCANStepBW, decimal.Parse(data[15]));
//                        DemodBW = Decimal.Parse(data[16]);
//                        for (int j = 0; j < UniqueData.DemodBW.Length; j++)
//                        {
//                            if (DemodBW == UniqueData.DemodBW[j]) { DemodBWInd = j; }
//                        }
//                        for (int j = 0; j < UniqueData.Demod.Length; j++)
//                        {
//                            if (data[17].ToUpper() == UniqueData.Demod[j]) { DemodInd = j; }
//                        }

//                        for (int j = 0; j < UniqueData.Detectors.Count; j++)
//                        {
//                            if (data[18].ToUpper() == UniqueData.Detectors[j].Parameter) { DetectorInd = j; }//запилить для каждой железки
//                        }

//                        if (data[19].Contains("1")) { AFCState = true; }
//                        else if (data[19].Contains("0")) { AFCState = false; }

//                        MGC = Int32.Parse(data[20]);
//                        if (data[21].Contains("AUTO")) { MGCAutoState = true; }
//                        else if (data[21].Contains("FIX")) { MGCAutoState = false; }

//                        SQU = Int32.Parse(data[22]);
//                        if (data[23].Contains("1")) { SQUState = true; }
//                        else if (data[23].Contains("0")) { SQUState = false; }
//                        DFSquelchValue = decimal.Parse(data[24]);

//                        if (data[25].Contains("1") || data[25].Contains("ON")) { DFBandwidthAuto = true; }
//                        else if (data[25].Contains("0") || data[25].Contains("OFF")) { DFBandwidthAuto = false; }

//                        if (data[26].Contains("1")) { ATTAutoState = true; }
//                        else if (data[26].Contains("0")) { ATTAutoState = false; }
//                        ATT = Int32.Parse(data[27]);
//                        #endregion


//                        FreqStart = FFMFreqCentr - FFMFreqSpan / 2;
//                        FreqStop = FFMFreqCentr + FFMFreqSpan / 2;
//                        #endregion
//                    }
//                }
//                #region Exception
//                catch (Exception exp)
//                {
//                    _logger.Exception(Contexts.ThisComponent, exp);
//                }
//                #endregion
//            }
//        }

//        public void SetLevelFromMode()
//        {
//            if (Mode.ModeName == "FFM") { RefLevel = FFMRefLevel; Range = FFMRangeLevel; LowestLevel = FFMLowestLevel; }//FFM
//            else if (Mode.ModeName == "DF") { RefLevel = FFMRefLevel; Range = FFMRangeLevel; LowestLevel = FFMLowestLevel; }//DF
//            else if (Mode.ModeName == "PSCAN") { RefLevel = PScanRefLevel; Range = PScanRangeLevel; LowestLevel = PScanLowestLevel; }//PScan
//        }
//        public void SetStreamingMode()
//        {
//            try
//            {
//                if (tc.IsOpen)
//                {

//                    tc.WriteLine("MEAS:APPL " + Mode.MeasAppl);
//                    tc.WriteLine("SENSe:FREQ:MODE " + Mode.FreqMode);
//                    if (LastMode.ModeName == "FFM")
//                    {
//                        string tagoff = "TRAC:UDP:TAG:OFF \"" + MyIPAddress + "\", " + UDPPort.ToString() + ", IFPan";
//                        //if (UseIntegratedGPS && UniqueData.LoadedInstrOption.Exists(item => item.Type.ToLower() == "gp"))
//                        //{
//                        //    tagoff = "TRAC:UDP:TAG:OFF \"" + MyIPAddress + "\", " + UDPPort.ToString() + ", IFPan, gpsc";
//                        //}
//                        tc.WriteLine(tagoff);
//                        tc.WriteLine("TRAC:UDP:FLAG:OFF \"" + MyIPAddress + "\", " + UDPPort.ToString() + ", \"VOLT:AC\", \"FSTR\", \"swap\", \"opt\"");//\"FSTR\", 
//                        tc.WriteLine("SENS:FUNC:OFF \"VOLT:AC\"");
//                        tc.WriteLine("SENS:FUNC:OFF \"FSTRength\"");
//                    }
//                    else if (LastMode.ModeName == "DF")
//                    {
//                        string tagoff = "TRAC:UDP:TAG:OFF \"" + MyIPAddress + "\", " + UDPPort.ToString() + ", DFPan";
//                        //if (UseIntegratedGPS && UniqueData.LoadedInstrOption.Exists(item => item.Type.ToLower() == "gp"))
//                        //{
//                        //    tagoff = "TRAC:UDP:TAG:OFF \"" + MyIPAddress + "\", " + UDPPort.ToString() + ", DFPan, gpsc";
//                        //}
//                        tc.WriteLine(tagoff);
//                        if (UniqueData.InstrModel == "EM100" || UniqueData.InstrModel == "PR100" || UniqueData.InstrModel == "DDF007")
//                        {
//                            tc.WriteLine("TRAC:UDP:FLAG:OFF \"" + MyIPAddress + "\", " + UDPPort.ToString() + ", \"DFLevel\", \"AZImuth\", \"DFQuality\", \"opt\", \"swap\"");
//                            tc.WriteLine("SENS:FUNC:OFF \"VOLT:AC\"");
//                        }
//                        else if (UniqueData.InstrModel.Contains("DDF2") || UniqueData.InstrModel.Contains("EB5") || UniqueData.InstrModel.Contains("ESMD") || UniqueData.InstrModel.Contains("DDF550"))
//                        {
//                            tc.WriteLine("TRAC:UDP:FLAG:OFF \"" + MyIPAddress + "\", " + UDPPort.ToString() + ", 'DFLevel', 'AZImuth', 'DFQuality', 'swap', 'opt'");
//                            tc.WriteLine("SENS:FUNC:OFF \"VOLT:AC\"");
//                        }
//                    }
//                    else if (LastMode.ModeName == "PSCAN")
//                    {
//                        string tagoff = "TRAC:UDP:TAG:OFF \"" + MyIPAddress + "\", " + UDPPort.ToString() + ", PSC";
//                        //if (UseIntegratedGPS && UniqueData.LoadedInstrOption.Exists(item => item.Type.ToLower() == "gp"))
//                        //{
//                        //    tagoff = "TRAC:UDP:TAG:OFF \"" + MyIPAddress + "\", " + UDPPort.ToString() + ", PSC, gpsc";
//                        //}
//                        tc.WriteLine(tagoff);
//                        if (UniqueData.InstrModel == "EM100" || UniqueData.InstrModel == "PR100" || UniqueData.InstrModel == "DDF007")
//                        {
//                            tc.WriteLine("TRAC:UDP:FLAG:OFF \"" + MyIPAddress + "\", " + UDPPort.ToString() + ", 'freq:low:rx', 'freq:high:rx', 'volt:ac', 'swap', 'opt'");
//                        }
//                        else if (UniqueData.InstrModel.Contains("DDF2") || UniqueData.InstrModel.Contains("EB5") || UniqueData.InstrModel.Contains("ESMD") || UniqueData.InstrModel.Contains("DDF550"))
//                        {
//                            tc.WriteLine("TRAC:UDP:FLAG:OFF \"" + MyIPAddress + "\", " + UDPPort.ToString() + ", 'freq:rx', 'freq:high:rx', 'volt:ac', 'swap', 'opt'");
//                        }
//                        tc.WriteLine("SENS:FUNC:OFF \"VOLT:AC\"");
//                    }
//                    if (Mode.ModeName == "FFM")
//                    {
//                        string tagoff = "TRAC:UDP:TAG:ON \"" + MyIPAddress + "\", " + UDPPort.ToString() + ", IFPan";
//                        //if (UseIntegratedGPS && UniqueData.LoadedInstrOption.Exists(item => item.Type.ToLower() == "gp"))
//                        //{
//                        //    tagoff = "TRAC:UDP:TAG:ON \"" + MyIPAddress + "\", " + UDPPort.ToString() + ", IFPan, gpsc";
//                        //}
//                        tc.WriteLine(tagoff);
//                        tc.WriteLine("TRAC:UDP:FLAG:ON \"" + MyIPAddress + "\", " + UDPPort.ToString() + ", \"FSTR\", \"VOLT:AC\", \"swap\", \"opt\"");//\"FSTR\", 
//                        tc.WriteLine("SENS:FUNC:ON \"VOLT:AC\"");
//                        tc.WriteLine("SENS:FUNC:ON \"FSTRength\"");
//                    }
//                    else if (Mode.ModeName == "DF")
//                    {
//                        string tagoff = "TRAC:UDP:TAG:ON \"" + MyIPAddress + "\", " + UDPPort.ToString() + ", DFPan";
//                        //if (UseIntegratedGPS && UniqueData.LoadedInstrOption.Exists(item => item.Type.ToLower() == "gp"))
//                        //{
//                        //    tagoff = "TRAC:UDP:TAG:ON \"" + MyIPAddress + "\", " + UDPPort.ToString() + ", DFPan, gpsc";
//                        //}
//                        tc.WriteLine(tagoff);
//                        if (UniqueData.InstrModel == "EM100" || UniqueData.InstrModel == "PR100" || UniqueData.InstrModel == "DDF007")
//                        {
//                            tc.WriteLine("TRAC:UDP:FLAG:ON \"" + MyIPAddress + "\", " + UDPPort.ToString() + ", \"AZImuth\"");
//                            tc.WriteLine("TRAC:UDP:FLAG:ON \"" + MyIPAddress + "\", " + UDPPort.ToString() + ", \"DFQ\"");
//                            tc.WriteLine("TRAC:UDP:FLAG:ON \"" + MyIPAddress + "\", " + UDPPort.ToString() + ", \"DFL\"");
//                            tc.WriteLine("TRAC:UDP:FLAG:ON \"" + MyIPAddress + "\", " + UDPPort.ToString() + ", \"opt\"");
//                            tc.WriteLine("TRAC:UDP:FLAG:ON \"" + MyIPAddress + "\", " + UDPPort.ToString() + ", \"swap\"");
//                            tc.WriteLine("SENS:FUNC \"DFL\"");
//                            tc.WriteLine("SENS:FUNC \"AZIM\"");
//                            tc.WriteLine("SENS:FUNC \"DFQ\"");
//                            tc.WriteLine("SENS:FUNC:ON \"VOLT:AC\"");
//                        }
//                        else if (UniqueData.InstrModel.Contains("DDF2") || UniqueData.InstrModel.Contains("EB5") || UniqueData.InstrModel.Contains("ESMD") || UniqueData.InstrModel.Contains("DDF550"))
//                        {
//                            tc.WriteLine("TRAC:UDP:FLAG:ON \"" + MyIPAddress + "\", " + UDPPort.ToString() + ", 'DFLevel', 'AZImuth', 'DFQuality', 'swap', 'opt'");
//                            tc.WriteLine("SENS:FUNC:ON \"VOLT:AC\"");
//                        }


//                    }
//                    else if (Mode.ModeName == "PSCAN")
//                    {
//                        string tagoff = "TRAC:UDP:TAG:ON \"" + MyIPAddress + "\", " + UDPPort.ToString() + ", PSC";
//                        //if (UseIntegratedGPS && UniqueData.LoadedInstrOption.Exists(item => item.Type.ToLower() == "gp"))
//                        //{
//                        //    tagoff = "TRAC:UDP:TAG:ON \"" + MyIPAddress + "\", " + UDPPort.ToString() + ", PSC, gpsc";
//                        //}
//                        tc.WriteLine(tagoff);
//                        if (UniqueData.InstrModel == "EM100" || UniqueData.InstrModel == "PR100" || UniqueData.InstrModel == "DDF007")
//                        {
//                            tc.WriteLine("TRAC:UDP:FLAG:ON \"" + MyIPAddress + "\", " + UDPPort.ToString() + ", 'freq:low:rx', 'freq:high:rx', 'volt:ac', 'swap', 'opt'");
//                        }
//                        else if (UniqueData.InstrModel.Contains("DDF2") || UniqueData.InstrModel.Contains("EB5") || UniqueData.InstrModel.Contains("ESMD") || UniqueData.InstrModel.Contains("DDF550"))
//                        {
//                            tc.WriteLine("TRAC:UDP:FLAG:ON \"" + MyIPAddress + "\", " + UDPPort.ToString() + ", 'freq:rx', 'freq:high:rx', 'volt:ac', 'swap', 'opt'");
//                        }
//                        tc.WriteLine("SENS:FUNC:ON \"VOLT:AC\"");
//                    }
//                }
//            }
//            #region Exception
//            catch (Exception exp)
//            {
//                _logger.Exception(Contexts.ThisComponent, exp);
//            }
//            #endregion
//        }


//        #region 

//        private (LocalRSReceiverInfo.Mode, decimal, decimal, decimal) ModeSelector(decimal freqStart, decimal freqStop, decimal rBW)
//        {
//            LocalRSReceiverInfo.Mode mode = null;
//            decimal freqcentr = 0;
//            decimal freqspan = 0;
//            decimal rbw = 0;

//            decimal fc = (freqStop + freqStart) / 2;
//            decimal fs = freqStop - freqStart;

//            bool FFMFindSpan = false;
//            bool FFMFindRBW = false;
//            if (fs <= UniqueData.FFMSpan[UniqueData.FFMSpan.Length - 1].BW)//можем попасть в FFM
//            {
//                for (int i = 0; i < UniqueData.FFMSpan.Length; i++)
//                {
//                    if (!FFMFindSpan && UniqueData.FFMSpan[i].BW >= fs)
//                    {
//                        FFMFindSpan = true;
//                        //найдем rbw
//                        for (int j = UniqueData.FFMSpan[i].AvailableStepBW.Length - 1; j >= 0; j--)
//                        {
//                            if (!FFMFindRBW && UniqueData.FFMSpan[i].AvailableStepBW[j] <= rBW)
//                            {
//                                FFMFindRBW = true;
//                                mode = UniqueData.Modes.Where(x => x.ModeName == "FFM").First();
//                                freqcentr = fc;
//                                freqspan = fs;
//                                rbw = UniqueData.FFMSpan[i].AvailableStepBW[j];
//                                break;
//                            }
//                        }
//                        break;
//                    }
//                }
//            }
//            if (fs > UniqueData.FFMSpan[UniqueData.FFMSpan.Length - 1].BW || !FFMFindSpan || !FFMFindRBW) //только панорама
//            {
//                if (UniqueData.LoadedInstrOption.Exists(x => x.Type == "PS"))
//                {

//                }
//                else
//                {
//                    string error = "";
//                    if (fs > UniqueData.FFMSpan[UniqueData.FFMSpan.Length - 1].BW || !FFMFindSpan)//нет PSCAN
//                    {
//                        error = "The PSCAN option is not activated. Spectrum bandwidth over " + fs / 1000000 + " MHz not available ";
//                    }
//                    else if (FFMFindSpan && !FFMFindRBW)
//                    {
//                        error = "The PSCAN option is not activated. In FFM mode, " + fs / 1000000 + " MHz bandwidth not available RBW " + rBW / 1000 + " kHz.";
//                    }
//                    throw new Exception(error);
//                }
//            }

//            return (mode, freqcentr, freqspan, rbw);
//        }

//        private void SetFFMFreqCentr(decimal _FFMFreqCentr)
//        {
//            FFMFreqCentrToSet = _FFMFreqCentr;
//            try
//            {
//                if (tc.IsOpen)
//                {
//                    tc.WriteLine(":FREQ " + FFMFreqCentrToSet.ToString("G29").Replace(',', '.'));
//                }
//            }
//            #region Exception
//            catch (Exception exp)
//            {
//                _logger.Exception(Contexts.ThisComponent, exp);
//            }
//            #endregion
//        }
//        private void SetFFMFreqSpanFromIndex()
//        {
//            try
//            {
//                if (tc.IsOpen)
//                {
//                    tc.WriteLine("FREQ:SPAN " + UniqueData.FFMSpan[FFMFreqSpanIndToSet].BW.ToString("G29").Replace(',', '.'));
//                    decimal d = decimal.Parse(tc.Query("FREQ:SPAN?"));
//                    for (int i = 0; i < UniqueData.FFMSpan.Length; i++)
//                    {
//                        if (UniqueData.FFMSpan[i].BW == d)
//                        {
//                            FFMFreqSpanInd = i;
//                            break;
//                        }
//                    }
//                }
//            }
//            #region Exception
//            catch (Exception exp)
//            {
//                _logger.Exception(Contexts.ThisComponent, exp);
//            }
//            #endregion
//        }
//        #endregion





//        //ПАФИКСИТЬ
//        #region Adapter Properties
//        private void SetDefaulConfig(ref CFG.AdapterMainConfig config)
//        {
//            config.IQBitRateMax = 40;
//            config.AdapterEquipmentInfo = new CFG.AdapterEquipmentInfo()
//            {
//                AntennaManufacturer = "AntennaManufacturer",
//                AntennaName = "Omni",
//                AntennaSN = "123"
//            };
//            config.AdapterRadioPathParameters = new CFG.AdapterRadioPathParameter[]
//            {
//                new CFG.AdapterRadioPathParameter()
//                {
//                    Freq = 1*1000000,
//                    KTBF = -147,//уровень своих шумов на Гц
//                    FeederLoss = 2,//потери фидера
//                    Gain = 10, //коэф усиления
//                    DiagA = "HV",
//                    DiagH = "POINT 0 0 90 3 180 6 270 3",//от нуля В конфиг
//                    DiagV = "POINT -90 20 0 0 90 10"//от -90  до 90 В конфиг
//                },
//                new CFG.AdapterRadioPathParameter()
//                {
//                    Freq = 1000*1000000,
//                    KTBF = -147,//уровень своих шумов на Гц
//                    FeederLoss = 2,//потери фидера
//                    Gain = 10, //коэф усиления
//                    DiagA = "HV",
//                    DiagH = "POINT 0 0 90 3 180 6 270 3",//от нуля В конфиг
//                    DiagV = "POINT -90 20 0 0 90 10"//от -90  до 90 В конфиг
//                }
//            };
//        }

//        private (MesureTraceDeviceProperties, MesureIQStreamDeviceProperties) GetProperties(CFG.AdapterMainConfig config)
//        {
//            RadioPathParameters[] rrps = ConvertRadioPathParameters(config);
//            StandardDeviceProperties sdp = new StandardDeviceProperties()
//            {
//                AttMax_dB = 30,
//                AttMin_dB = 0,
//                FreqMax_Hz = UniqueData.FreqMax,
//                FreqMin_Hz = UniqueData.FreqMin,
//                PreAmpMax_dB = 30,
//                PreAmpMin_dB = 0,
//                RefLevelMax_dBm = (int)(UniqueData.RefLevelMAX - 107),
//                RefLevelMin_dBm = (int)(UniqueData.RefLevelMIN - 107),
//                EquipmentInfo = new EquipmentInfo()
//                {
//                    AntennaCode = config.AdapterEquipmentInfo.AntennaSN,// "Omni",//S/N  В конфиг
//                    AntennaManufacturer = config.AdapterEquipmentInfo.AntennaManufacturer,//"3anet",//В конфиг
//                    AntennaName = config.AdapterEquipmentInfo.AntennaName,//"BC600",//В конфиг
//                    EquipmentManufacturer = new Atdi.DataModels.Sdrn.DeviceServer.Adapters.InstrManufacrures().RuS.UI,
//                    EquipmentName = UniqueData.InstrModel,
//                    EquipmentFamily = "MonitoringReceiver",//SDR/SpecAn/MonRec
//                    EquipmentCode = UniqueData.SerialNumber,//S/N

//                },
//                RadioPathParameters = rrps
//            };
//            MesureTraceDeviceProperties mtdp = new MesureTraceDeviceProperties()
//            {
//                RBWMax_Hz = (double)RBWMax,
//                RBWMin_Hz = 3,
//                SweepTimeMin_s = (double)UniqueData.MeasTimeMIN,
//                SweepTimeMax_s = (double)UniqueData.MeasTimeMAX,
//                StandardDeviceProperties = sdp,
//                //DeviceId ничего не писать, ID этого экземпляра адаптера
//            };
//            MesureIQStreamDeviceProperties miqdp = new MesureIQStreamDeviceProperties()
//            {
//                AvailabilityPPS = config.AvailabilityPPS, // В конфиг
//                BitRateMax_MBs = config.IQBitRateMax,
//                //DeviceId ничего не писать, ID этого экземпляра адаптера
//                standartDeviceProperties = sdp,
//            };

//            return (mtdp, miqdp);
//        }

//        private RadioPathParameters[] ConvertRadioPathParameters(CFG.AdapterMainConfig config)
//        {
//            RadioPathParameters[] rpps = new RadioPathParameters[config.AdapterRadioPathParameters.Length];
//            for (int i = 0; i < config.AdapterRadioPathParameters.Length; i++)
//            {
//                rpps[i] = new RadioPathParameters()
//                {
//                    Freq_Hz = config.AdapterRadioPathParameters[i].Freq,
//                    KTBF_dBm = config.AdapterRadioPathParameters[i].KTBF,//уровень своих шумов на Гц
//                    FeederLoss_dB = config.AdapterRadioPathParameters[i].FeederLoss,//потери фидера
//                    Gain = config.AdapterRadioPathParameters[i].Gain, //коэф усиления
//                    DiagA = config.AdapterRadioPathParameters[i].DiagA,
//                    DiagH = config.AdapterRadioPathParameters[i].DiagH,//от нуля В конфиг
//                    DiagV = config.AdapterRadioPathParameters[i].DiagV//от -90  до 90 В конфиг
//                };
//            }
//            return rpps;
//        }
//        #endregion Adapter Properties
//    }
//}
