//using Atdi.Contracts.Sdrn.DeviceServer;
//using Atdi.Platform.Logging;
//using CFG = Atdi.DataModels.Sdrn.DeviceServer.Adapters.Config;
//using COM = Atdi.DataModels.Sdrn.DeviceServer.Commands;
//using COMR = Atdi.DataModels.Sdrn.DeviceServer.Commands.Results;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

////using Ivi.Visa;
//using RohdeSchwarz.Visa;
//using Atdi.DataModels.Sdrn.DeviceServer.Adapters;
//using EN = Atdi.AppUnits.Sdrn.DeviceServer.Adapters.RSFPL.Enums;
//using MEN = Atdi.DataModels.Sdrn.DeviceServer.Adapters.Enums;
//using Atdi.DataModels.Sdrn.DeviceServer;

//namespace Atdi.AppUnits.Sdrn.DeviceServer.Adapters.RSFPL
//{
//    public class Adapter
//    {
//        private readonly ITimeService timeService;
//        private readonly ILogger logger;
//        private readonly AdapterConfig adapterConfig;

//        private LocalParametersConverter lpc;
//        private CFG.ThisAdapterConfig tac;
//        private CFG.AdapterMainConfig mainConfig;

//        public Adapter(AdapterConfig adapterConfig, ILogger logger, ITimeService timeService)
//        {
//            this.logger = logger;
//            this.adapterConfig = adapterConfig;
//            this.timeService = timeService;
//            lpc = new LocalParametersConverter();
//            //FreqArr = new double[TracePoints];
//            //LevelArr = new float[TracePoints];
//            //for (int i = 0; i < TracePoints; i++)
//            //{
//            //    FreqArr[i] = (double)(FreqStart + 1000 * i);
//            //    LevelArr[i] = -100;
//            //}
//        }


//        public void Connect(IAdapterHost host)
//        {
//            try
//            {

//                /// включем устройство
//                /// иницируем его параметрами сконфигурации
//                /// проверяем к чем оно готово

//                /// сообщаем инфраструктуре что мы готовы обрабатывать комманду MesureGpsLocationExampleCommand
//                /// и при этом возвращать оезультат в типе MesureGpsLocationExampleAdapterResult
//                if (SetConnect())
//                {
//                    string filename = "RSFPL" + serialNumber + ".xml";
//                    tac = new CFG.ThisAdapterConfig() { };
//                    if (!tac.GetThisAdapterConfig(filename))
//                    {
//                        mainConfig = new CFG.AdapterMainConfig() { };
//                        SetDefaulConfig(ref mainConfig);
//                        tac.SetThisAdapterConfig(mainConfig, filename);
//                    }
//                    else
//                    {
//                        mainConfig = tac.Main;
//                    }
//                    (MesureTraceDeviceProperties mtdp, MesureIQStreamDeviceProperties miqdp) = GetProperties(mainConfig);
//                    host.RegisterHandler<COM.MesureTraceCommand, COMR.MesureTraceResult>(MesureTraceCommandHandler, mtdp);
//                    if (UniqueData.IQAvailable)
//                    {
//                        //недопилено пока нет определенности со временем 
//                        //host.RegisterHandler<COM.MesureIQStreamCommand, COMR.MesureIQStreamResult>(MesureIQStreamCommandHandler, miqdp);
//                    }
//                }

//            }
//            #region Exception
//            catch (Exception exp)
//            {
//                logger.Exception(Contexts.ThisComponent, exp);
//                throw new InvalidOperationException("Invalid initialize/connect adapter", exp);
//            }
//            #endregion
//        }

//        public void Disconnect()
//        {
//            try
//            {
//                /// освобождаем ресурсы и отключаем устройство
//                session.Dispose();
//                rm.Dispose();
//            }
//            #region Exception
//            catch (Exception exp)
//            {
//                logger.Exception(Contexts.ThisComponent, exp);
//            }
//            #endregion
//        }
//        public void MesureTraceCommandHandler(COM.MesureTraceCommand command, IExecutionContext context)
//        {
//            try
//            {
//                /// если нужно проводим блокировку комманд который мы не сможем обслужить пока что то меряем в течении выполнени яэтой комманды
//                /// и подсказывая этим инфраструктуре что этот устрйостов некоторое время не сможет обрабатываить такие комманды
//                //context.Lock(CommandType.MesureTrace);

//                // если нужно заблокировать выполняему комманду то достатчоно вызвать метод без параметров и блокируется комманда которая выполняется
//                context.Lock();
//                //Переключимся на Spectrum
//                if (!mode)
//                {
//                    SetWindowType(true);
//                }
//                ValidateAndSetFreqStartStop(command.Parameter.FreqStart_Hz, command.Parameter.FreqStop_Hz);

//                ValidateAndSetRefLevel(command.Parameter.RefLevel_dBm);

//                ValidateAndSetPreAmp(command.Parameter.PreAmp_dB);

//                ValidateAndSetATT(command.Parameter.Att_dB);





//                //кол-во точек посчитать по RBW
//                if (command.Parameter.TracePoint == -1 && command.Parameter.RBW_Hz > 0)
//                {
//                    //RBW ближайшее меньшее
//                    RBW = LPC.RBW(UniqueData, (decimal)command.Parameter.RBW_Hz);
//                    SetRBW(RBW);
//                    if (command.Parameter.VBW_Hz < 0) //Если авто то VBW = RBW
//                    {
//                        VBW = LPC.VBW(UniqueData, RBW);// но проверим на доступность
//                        SetVBW(VBW);
//                    }
//                    else
//                    {
//                        VBW = LPC.VBW(UniqueData, (decimal)command.Parameter.VBW_Hz);
//                        SetVBW(VBW);
//                    }

//                    int needsweeppints = (int)(FreqSpan / RBW);
//                    int sweeppoints = LPC.SweepPoints(UniqueData, needsweeppints);
//                    SetSweepPoints(sweeppoints);
//                }
//                //сколько точек понятно по нему и посчитать RBW
//                else if (command.Parameter.TracePoint > 0 && command.Parameter.RBW_Hz == -1)
//                {
//                    int sweeppoints = LPC.SweepPoints(UniqueData, command.Parameter.TracePoint);
//                    SetSweepPoints(sweeppoints);

//                    decimal needrbw = LPC.RBW(UniqueData, FreqSpan / ((decimal)(sweeppoints - 1)));
//                    SetRBW(needrbw);
//                    if (command.Parameter.VBW_Hz < 0) //Если авто то VBW = RBW
//                    {
//                        VBW = LPC.VBW(UniqueData, RBW);// но проверим на доступность
//                        SetVBW(VBW);
//                    }
//                    else
//                    {
//                        VBW = LPC.VBW(UniqueData, (decimal)command.Parameter.VBW_Hz);
//                        SetVBW(VBW);
//                    }
//                }
//                //сколько точек понятно по нему и посчитать RBW
//                else if (command.Parameter.TracePoint > 0 && command.Parameter.RBW_Hz > 0)
//                {
//                    int sweeppoints = LPC.SweepPoints(UniqueData, command.Parameter.TracePoint);
//                    SetSweepPoints(sweeppoints);

//                    RBW = LPC.RBW(UniqueData, (decimal)command.Parameter.RBW_Hz);
//                    SetRBW(RBW);
//                    if (command.Parameter.VBW_Hz < 0) //Если авто то VBW = RBW
//                    {
//                        VBW = LPC.VBW(UniqueData, RBW);// но проверим на доступность
//                        SetVBW(VBW);
//                    }
//                    else
//                    {
//                        VBW = LPC.VBW(UniqueData, (decimal)command.Parameter.VBW_Hz);
//                        SetVBW(VBW);
//                    }
//                }

//                #region DetectorType
//                ParamWithId DetId = LPC.DetectorType(UniqueData, command.Parameter.DetectorType);
//                if (Trace1Detector != DetId)
//                {
//                    SetDetectorType(DetId);
//                }
//                #endregion DetectorType

//                #region TraceType
//                (ParamWithId TTId, EN.TraceType res) = LPC.TraceType(UniqueData, command.Parameter.TraceType);
//                //на прибор
//                if (Trace1Type != TTId)
//                {
//                    SetTraceType(TTId);
//                }
//                //Такой результат
//                if (TraceTypeResult != res)
//                {
//                    TraceTypeResult = res;
//                }
//                #endregion TraceType

//                #region LevelUnit
//                (ParamWithId lu, MEN.LevelUnit lur) = LPC.LevelUnit(UniqueData, command.Parameter.LevelUnit);
//                if (lur != LevelUnitsResult || LevelUnits != lu)
//                {
//                    LevelUnitsResult = lur;
//                    SetLevelUnit(lu);
//                }
//                //для ускорения усреднения установим levelunit в Ваты 
//                if (TraceTypeResult == EN.TraceType.Average)
//                {
//                    for (int i = 0; i < UniqueData.LevelUnits.Count; i++)
//                    {
//                        if (UniqueData.LevelUnits[i].Id == (int)MEN.LevelUnit.Watt)
//                        {
//                            SetLevelUnit(UniqueData.LevelUnits[i]);
//                        }
//                    }
//                }
//                #endregion LevelUnit

//                #region SweepTime
//                //Если установленно в настройках адаптера OnlyAutoSweepTime
//                if (_adapterConfig.OnlyAutoSweepTime)
//                {
//                    SetAutoSweepTime(_adapterConfig.OnlyAutoSweepTime);
//                }
//                //Если SweepTime можно установить
//                else
//                {
//                    (decimal swt, bool autoswt) = LPC.SweepTime(UniqueData, (decimal)command.Parameter.SweepTime_s);
//                    if (autoswt)
//                    {
//                        SetAutoSweepTime(_adapterConfig.OnlyAutoSweepTime);
//                    }
//                    else
//                    {
//                        SetSweepTime(swt);
//                    }
//                }
//                #endregion SweepTime

//                #region Костыль от некоректных данных спектра от прошлого измерения
//                if (UniqueData.InstrManufacture == 1)
//                {
//                    if (UniqueData.InstrModel.Contains("FPL"))
//                    {
//                        //Thread.Sleep((int)(SweepTime * 4000 + 20));
//                    }
//                    else
//                    {
//                        Thread.Sleep((int)(SweepTime * 4000 + 10));
//                    }
//                }
//                else if (UniqueData.InstrManufacture == 2)
//                {
//                    Thread.Sleep((int)(SweepTime * 4000 + 30));
//                }
//                else if (UniqueData.InstrManufacture == 3)
//                {
//                    Thread.Sleep((int)(SweepTime * 4000 + 30));
//                }
//                #endregion


//                //Устанавливаем сколько трейсов хотим
//                if (command.Parameter.TraceCount > 0)
//                {
//                    TraceCountToMeas = (ulong)command.Parameter.TraceCount + 1;// +1 маленький костыль для предотвращения кривого трейса
//                    TraceCount = 0;
//                    TracePoints = command.Parameter.TracePoint;
//                }
//                else
//                {
//                    throw new Exception("TraceCount must be set greater than zero.");
//                }

//                //Меряем
//                //Если TraceType ClearWrite то пушаем каждый результат                    
//                if (TraceTypeResult == EN.TraceType.ClearWrite)
//                {
//                    bool newres = false;


//                    for (ulong i = 0; i < TraceCountToMeas; i++)
//                    {
//                        newres = GetTrace();
//                        if (newres)
//                        {
//                            if (i > 0)// +1 маленький костыль для предотвращения кривого трейса
//                            {
//                                // пушаем результат
//                                var result = new COMR.MesureTraceResult(TraceCount, CommandResultStatus.Next);
//                                TraceCount++;
//                                if (TraceCountToMeas == TraceCount)
//                                {
//                                    result = new COMR.MesureTraceResult(TraceCount, CommandResultStatus.Final);
//                                }
//                                result.Att_dB = (int)AttLevelSpec;
//                                result.RefLevel_dBm = (int)RefLevelSpec;
//                                result.PreAmp_dB = PreAmpSpec ? 1 : 0;
//                                result.RBW_Hz = (double)RBW;
//                                result.VBW_Hz = (double)VBW;
//                                result.Freq_Hz = new double[FreqArr.Length];
//                                result.Level = new float[FreqArr.Length];
//                                for (int j = 0; j < FreqArr.Length; j++)
//                                {
//                                    result.Freq_Hz[j] = FreqArr[j];
//                                    result.Level[j] = LevelArr[j];
//                                }
//                                result.TimeStamp = _timeService.GetGnssUtcTime().Ticks - UTCOffset;// new DateTime(1970, 1, 1, 0, 0, 0, System.DateTimeKind.Utc).Ticks;
//                                                                                                   //result.TimeStamp = DateTime.UtcNow.Ticks - new DateTime(1970, 1, 1, 0, 0, 0, System.DateTimeKind.Utc).Ticks;
//                                if (PowerRegister != EN.PowerRegister.Normal)
//                                {
//                                    result.DeviceStatus = COMR.Enums.DeviceStatus.RFOverload;
//                                }
//                                context.PushResult(result);
//                            }
//                        }
//                        else
//                        {
//                            i--;
//                        }
//                        // иногда нужно проверять токен окончания работы комманды
//                        if (context.Token.IsCancellationRequested)
//                        {
//                            // все нужно остановиться

//                            // если есть порция данных возвращаем ее в обработчки только говрим что поток результатов не законченный и больше уже не будет поступать
//                            var result2 = new COMR.MesureTraceResult(TraceCount, CommandResultStatus.Ragged);

//                            context.PushResult(result2);


//                            // подтверждаем факт обработки отмены
//                            context.Cancel();
//                            // освобождаем поток 
//                            return;
//                        }
//                    }


//                }
//                //Если TraceType Average/MinHold/MaxHold то делаем измерений сколько сказали и пушаем только готовый результат
//                else
//                {
//                    TraceReset = true;///сбросим предыдущие результаты
//                    if (TraceTypeResult == EN.TraceType.Average)//назначим сколько усреднять
//                    {
//                        TraceAveraged.AveragingCount = (int)TraceCountToMeas - 1;
//                    }
//                    bool _RFOverload = false;
//                    bool newres = false;
//                    for (ulong i = 0; i < TraceCountToMeas; i++)
//                    {
//                        newres = GetTrace();
//                        if (newres)
//                        {
//                            if (i == 0)
//                            {
//                                TraceReset = true;
//                            }
//                            TraceCount++;
//                            if (PowerRegister != EN.PowerRegister.Normal)
//                            {
//                                _RFOverload = true;
//                            }
//                        }
//                        else
//                        {
//                            i--;
//                        }
//                        // иногда нужно проверять токен окончания работы комманды
//                        if (context.Token.IsCancellationRequested)
//                        {
//                            // все нужно остановиться

//                            // если есть порция данных возвращаем ее в обработчки только говрим что поток результатов не законченный и больше уже не будет поступать

//                            var result2 = new COMR.MesureTraceResult(TraceCount, CommandResultStatus.Ragged);
//                            //Скорее нет результатов
//                            context.PushResult(result2);

//                            // подтверждаем факт обработки отмены
//                            context.Cancel();
//                            // освобождаем поток 
//                            return;
//                        }
//                    }
//                    if (TraceCountToMeas == TraceCount)
//                    {
//                        var result = new COMR.MesureTraceResult(0, CommandResultStatus.Final)
//                        {
//                            Freq_Hz = new double[FreqArr.Length],
//                            Level = new float[FreqArr.Length]
//                        };
//                        for (int j = 0; j < FreqArr.Length; j++)
//                        {
//                            result.Freq_Hz[j] = FreqArr[j];
//                            result.Level[j] = LevelArr[j];
//                        }
//                        result.Att_dB = (int)AttLevelSpec;
//                        result.RefLevel_dBm = (int)RefLevelSpec;
//                        result.PreAmp_dB = PreAmpSpec ? 1 : 0;
//                        result.RBW_Hz = (double)RBW;
//                        result.VBW_Hz = (double)VBW;
//                        result.TimeStamp = _timeService.GetGnssUtcTime().Ticks - UTCOffset;// new DateTime(1970, 1, 1, 0, 0, 0, System.DateTimeKind.Utc).Ticks;
//                                                                                           //result.TimeStamp = DateTime.UtcNow.Ticks - new DateTime(1970, 1, 1, 0, 0, 0, System.DateTimeKind.Utc).Ticks;
//                        if (_RFOverload)
//                        {
//                            result.DeviceStatus = COMR.Enums.DeviceStatus.RFOverload;
//                        }
//                        else
//                        {
//                            result.DeviceStatus = COMR.Enums.DeviceStatus.Normal;
//                        }
//                        context.PushResult(result);
//                    }
//                }
//                context.Unlock();

//                // что то делаем еще 


//                // подтверждаем окончание выполнения комманды 
//                // важно: всн ранее устапнволеные в контексте обработки текущей команыд блокировки снимаются автоматически
//                context.Finish();
//                // дальше кода быть не должно, освобождаем поток
//            }
//            catch (VisaException v_exp)
//            {
//                // желательно записать влог
//                logger.Exception(Contexts.ThisComponent, v_exp);
//                // этот вызов обязательный в случаи обрыва
//                context.Abort(v_exp);
//                // дальше кода быть не должно, освобождаем поток
//            }
//            catch (Exception e)
//            {
//                // желательно записать влог
//                logger.Exception(Contexts.ThisComponent, e);
//                // этот вызов обязательный в случаи обрыва
//                context.Abort(e);
//                // дальше кода быть не должно, освобождаем поток
//            }

//        }

//        //неживое но работет
//        public void MesureIQStreamCommandHandler(COM.MesureIQStreamCommand command, IExecutionContext context)
//        {
//            try
//            {
//                /// если нужно проводим блокировку комманд который мы не сможем обслужить пока что то меряем в течении выполнени яэтой комманды
//                /// и подсказывая этим инфраструктуре что этот устрйостов некоторое время не сможет обрабатываить такие комманды
//                context.Lock(CommandType.MesureIQStream);
//                // если нужно заблокировать выполняему комманду то достатчоно вызвать метод без параметров и блокируется комманда которая выполняется
//                context.Lock();
//                //Переключимся на IQ
//                if (Mode != EN.Mode.IQAnalyzer)
//                {
//                    SetWindowType(EN.Mode.IQAnalyzer);
//                }

//                //FreqCentrIQ
//                if (FreqCentrIQ != (command.Parameter.FreqStart_Hz + command.Parameter.FreqStop_Hz) / 2)
//                {
//                    FreqCentrIQ = LPC.FreqCentrIQ(UniqueData, command.Parameter.FreqStart_Hz, command.Parameter.FreqStop_Hz);
//                    SetFreqCentrIQ(FreqCentrIQ);
//                }

//                //ATT
//                (decimal att, bool auto) = LPC.Attenuator(UniqueData, command.Parameter.Att_dB);
//                if (auto)
//                {
//                    SetAutoAtt(auto);
//                }
//                else
//                {
//                    if (AttLevelIQ != att)
//                    {
//                        SetAttLevel(att);
//                    }
//                }

//                //PreAmp
//                bool preamp = LPC.PreAmp(UniqueData, command.Parameter.Att_dB);
//                if (PreAmpIQ != preamp)
//                {
//                    SetPreAmp(preamp);
//                }

//                //Reflevel
//                if (RefLevelIQ != command.Parameter.RefLevel_dBm)
//                {
//                    if (command.Parameter.RefLevel_dBm == 1000000000)
//                    {
//                        RefLevelIQ = -20;
//                    }
//                    else
//                    {
//                        RefLevelIQ = command.Parameter.RefLevel_dBm;
//                    }
//                    SetRefLevel(RefLevelIQ);
//                }

//                //FreqSpanIQ установит полосу просмотра IQ оно же зафиксирует скорость семплирования
//                if (IQBW != command.Parameter.FreqStop_Hz - command.Parameter.FreqStart_Hz)
//                {
//                    IQBW = LPC.FreqSpanIQ(UniqueData, command.Parameter.FreqStart_Hz, command.Parameter.FreqStop_Hz);
//                    SetIQBW(IQBW);
//                }
//                //OR закоментировать что-то одно
//                //FreqSpanIQ установит скорость семплирования IQ оно же зафиксирует полосу просмотра 
//                //decimal samplespeed = LPC.SampleSpeed(UniqueData, ((decimal)command.Parameter.BitRate_MBs) * 1000000);
//                //if (SampleSpeed != samplespeed)
//                //{
//                //    SetSampleSpeed(samplespeed);
//                //}

//                COMR.MesureIQStreamResult result = new COMR.MesureIQStreamResult(0, CommandResultStatus.Final)
//                {
//                    DeviceStatus = COMR.Enums.DeviceStatus.Normal
//                };
//                if (GetIQStream(ref result, command))
//                {
//                    context.PushResult(result);
//                }
//                /////////////
//                //////////long timestop = _timeService.GetGnssTime().Ticks;
//                context.Unlock();
//                context.Finish();
//            }
//            catch (Exception e)
//            {
//                // желательно записать влог
//                logger.Exception(Contexts.ThisComponent, e);
//                // этот вызов обязательный в случаи обрыва
//                context.Abort(e);
//                // дальше кода быть не должно, освобождаем поток
//            }
//        }



//        #region Param
//        private ResourceManager rm = null;
//        private readonly long UTCOffset = 621355968000000000;
//        private readonly string decimalSeparator = System.Globalization.NumberFormatInfo.CurrentInfo.CurrencyDecimalSeparator;
//        private TcpipSession session = null;


//        #region Instr
//        private readonly List<DeviceOption> instrOption = new List<DeviceOption>()
//        {
//             new DeviceOption() { Type = "B4", Designation = "OCXO Reference Frequency", GlobalType = "OCXO Reference"},
//             new DeviceOption() { Type = "B5", Designation = "Additional Interfaces", GlobalType = "Additional Interfaces"},
//             new DeviceOption() { Type = "B10", Designation = "GPIB Interface", GlobalType = "GPIB Interface"},
//             new DeviceOption() { Type = "B19", Designation = "Second Hard Disk (SSD)", GlobalType = "SSD"},
//             new DeviceOption() { Type = "B22", Designation = "RF Preamplifier", GlobalType = "Preamplifier"},
//             new DeviceOption() { Type = "B25", Designation = "1 dB Steps for Electronic Attenuator", GlobalType = "Attenuator 1 dB"},
//             new DeviceOption() { Type = "B30", Designation = "DC Power Supply 12/24 V", GlobalType = "DC Power Supply"},
//             new DeviceOption() { Type = "B31", Designation = "Internal Li-Ion Battery", GlobalType = "Internal Li-Ion Battery"},
//             new DeviceOption() { Type = "B40", Designation = "40 MHz Analysis Bandwidth", GlobalType = "40 MHz Analysis Bandwidth"},
//             new DeviceOption() { Type = "K7", Designation = "Analog Modulation Analysis (AM/FM/φM)", GlobalType = "Analog Modulation Analysis"},
//             new DeviceOption() { Type = "K9", Designation = "Power Sensor Measurement with R&S®NRP Power Sensors", GlobalType = "Power Sensor Support"},
//             new DeviceOption() { Type = "K30", Designation = "Noise Figure Measurement Application", GlobalType = "Noise Figure Measurement Application"},
//        };
//        private List<DeviceOption> loadedInstrOption;

//        /// <summary>
//        /// true = SpectrumAnalyzer
//        /// false = IQAnalyzer
//        /// </summary>
//        private bool mode = true;

//        private string instrModel = "";
//        private string serialNumber = "";
//        #region Freqs
//        public decimal FreqMin = 9000;
//        public decimal FreqMax = 3000000000;

//        public decimal FreqCentr
//        {
//            get { return _FreqCentr; }
//            set
//            {
//                _FreqCentr = value;
//                _FreqStart = _FreqCentr - _FreqSpan / 2;
//                _FreqStop = _FreqCentr + _FreqSpan / 2;
//            }
//        }
//        private decimal _FreqCentr = 1000000000;

//        public decimal FreqSpan
//        {
//            get { return _FreqSpan; }
//            set
//            {
//                _FreqSpan = value;
//                _FreqStart = _FreqCentr - _FreqSpan / 2;
//                _FreqStop = _FreqCentr + _FreqSpan / 2;
//            }
//        }
//        private decimal _FreqSpan = 20000000;

//        public decimal FreqStart
//        {
//            get { return _FreqStart; }
//            set
//            {
//                _FreqStart = value;
//                _FreqCentr = (_FreqStart + _FreqStop) / 2;
//                _FreqSpan = _FreqStop - _FreqStart;
//            }
//        }
//        private decimal _FreqStart = 990000000;

//        public decimal FreqStop
//        {
//            get { return _FreqStop; }
//            set
//            {
//                _FreqStop = value;
//                _FreqCentr = (_FreqStart + _FreqStop) / 2;
//                _FreqSpan = _FreqStop - _FreqStart;
//            }
//        }
//        private decimal _FreqStop = 1010000000;
//        #endregion Freqs

//        #region RBW / VBW
//        private readonly decimal[] RBWArr = new decimal[] { 1, 2, 3, 5, 10, 20, 30, 50, 100, 200, 300, 500, 1000, 2000, 3000, 5000, 6250, 10000, 20000, 30000, 50000, 100000, 200000, 300000, 500000, 1000000, 2000000, 3000000, 5000000, 10000000 };
//        private readonly decimal[] VBWArr = new decimal[] { 1, 2, 3, 5, 10, 20, 30, 50, 100, 200, 300, 500, 1000, 2000, 3000, 5000, 10000, 20000, 30000, 50000, 100000, 200000, 300000, 500000, 1000000, 2000000, 3000000, 5000000, 10000000 };
//        public decimal RBW;

//        private int RBWIndex
//        {
//            get { return _RBWIndex; }
//            set
//            {
//                if (value > RBWArr.Length - 1) _RBWIndex = RBWArr.Length - 1;
//                else if (value < 0) _RBWIndex = 0;
//                else _RBWIndex = value;
//                RBW = RBWArr[_RBWIndex];
//            }
//        }
//        private int _RBWIndex = 0;

//        public bool AutoRBW;


//        public decimal VBW;

//        private int VBWIndex
//        {
//            get { return _VBWIndex; }
//            set
//            {
//                if (value > VBWArr.Length - 1) _VBWIndex = VBWArr.Length - 1;
//                else if (value < 0) _VBWIndex = 0;
//                else _VBWIndex = value;
//                VBW = VBWArr[_VBWIndex];
//            }
//        }
//        private int _VBWIndex = 0;

//        public bool AutoVBW;
//        #endregion RBW / VBW

//        #region Sweep
//        private const decimal SWTMin = 0.000075m;
//        private const decimal SWTMax = 8000;

//        public decimal SweepTime;

//        public bool AutoSweepTime;

//        public int TracePoints;

//        public int SweepPoints;
//        public int SweepPointsIndex
//        {
//            get { return _SweepPointsIndex; }
//            set
//            {
//                if (value > sweepPointArr.Length - 1) SweepPointsIndex = sweepPointArr.Length - 1;
//                else if (value < 0) SweepPointsIndex = 0;
//                else _SweepPointsIndex = value;
//                SweepPoints = sweepPointArr[SweepPointsIndex];
//            }
//        }
//        private int _SweepPointsIndex = 0;

//        public ParamWithId SweepTypeSelected { get; set; } = new ParamWithId { Id = 0, Parameter = "" };

//        private EN.Optimization Optimization
//        {
//            get { return _Optimization; }
//            set
//            {
//                for (int i = 0; i < optimization.Count; i++)
//                {
//                    if ((int)value == optimization[i].Id)
//                    {
//                        _Optimization = value;
//                        OptimizationSelected = optimization[i];
//                    }
//                }
//            }
//        }
//        private EN.Optimization _Optimization = EN.Optimization.Auto;
//        private ParamWithId OptimizationSelected { get; set; } = new ParamWithId { Id = 0, Parameter = "" };

//        private readonly List<ParamWithId> optimization = new List<ParamWithId>
//        {
//            new ParamWithId {Id = (int) EN.Optimization.Auto, Parameter = "AUTO" },
//            new ParamWithId {Id = (int) EN.Optimization.Dynamic, Parameter = "DYN" },
//            new ParamWithId {Id = (int) EN.Optimization.Speed, Parameter = "SPE" }
//        };
//        private readonly int[] sweepPointArr = new int[]{ 101, 125, 155, 173, 201, 251, 301, 313, 345, 401, 501, 601, 625, 691, 701, 801, 901, 1001,
//                    1251, 1383, 1999, 2001, 2501, 2765, 3001, 4001, 5001, 6001, 7001, 8001, 9001,
//                    10001, 11001, 12001, 13001, 14001, 15001, 16001, 17001, 18001, 19001,
//                    20001, 21001, 22001, 23001, 24001, 25001, 26001, 27001, 28001, 29001,
//                    30001, 31001, 32001, 33001, 34001, 35001, 36001, 37001, 38001, 39001,
//                    40001, 41001, 42001, 43001, 44001, 45001, 46001, 47001, 48001, 49001,
//                    50001, 51001, 52001, 53001, 54001, 55001, 56001, 57001, 58001, 59001,
//                    60001, 61001, 62001, 63001, 64001, 65001, 66001, 67001, 68001, 69001,
//                    70001, 71001, 72001, 73001, 74001, 75001, 76001, 77001, 78001, 79001,
//                    80001, 81001, 82001, 83001, 84001, 85001, 86001, 87001, 88001, 89001,
//                    90001, 91001, 92001, 93001, 94001, 95001, 96001, 97001, 98001, 99001,
//                    100001 };

//        private readonly List<ParamWithId> SweepType = new List<ParamWithId>
//        {
//            new ParamWithId {Id = (int) EN.SweepType.Auto, Parameter = "AUTO" },
//            new ParamWithId {Id = (int) EN.SweepType.Sweep, Parameter = "SWE" },
//            new ParamWithId {Id = (int) EN.SweepType.FFT, Parameter = "FFT" }
//        };
//        #endregion

//        #region Level
//        public EN.PowerRegister PowerRegister = EN.PowerRegister.Normal;
//        private readonly int attMax = 45;
//        private int attStep = 5;
//        private bool preAmpAvailable;
//        private const decimal RefLevelMax = 30;
//        private const decimal RefLevelMin = -130;
//        private const decimal RefLevelStep = 1;

//        public decimal RefLevelSpec = -40;
//        public decimal RefLevelIQ = -40;

//        public decimal RangeSpec = 100;
//        public decimal RangeIQ = 100;

//        private decimal AttLevelSpec
//        {
//            get { return attLevelSpec; }
//            set
//            {
//                if (value > attMax) attLevelSpec = attMax;
//                else if (value < 0) attLevelSpec = 0;
//                else attLevelSpec = value;
//            }
//        }
//        private decimal attLevelSpec = 0;

//        private decimal AttLevelIQ
//        {
//            get { return attLevelIQ; }
//            set
//            {
//                if (value > attMax) attLevelIQ = attMax;
//                else if (value < 0) attLevelIQ = 0;
//                else attLevelIQ = value;
//            }
//        }
//        private decimal attLevelIQ = 0;

//        private bool attAutoSpec;
//        private bool attAutoIQ;

//        private bool preAmpSpec;
//        private bool preAmpIQ;

//        private ParamWithId levelUnit { get; set; } = new ParamWithId() { Id = 0, Parameter = "" };

//        private MEN.LevelUnit levelUnitResult = MEN.LevelUnit.dBm;

//        private readonly List<ParamWithId> levelUnits = new List<ParamWithId>
//        {
//            new ParamWithId {Id = (int) MEN.LevelUnit.dBm, Parameter = "DBM" },
//            new ParamWithId {Id = (int) MEN.LevelUnit.dBmV, Parameter = "DBMV" },
//            new ParamWithId {Id = (int) MEN.LevelUnit.dBµV, Parameter = "DBUV" },
//            new ParamWithId {Id = (int) MEN.LevelUnit.Watt, Parameter = "WATT" },
//        };
//        #endregion Level

//        #region Trace Data

//        private ulong traceCountToMeas = 1;
//        private ulong traceCount = 1;

//        public double[] FreqArr;
//        public float[] LevelArr;
//        private float[] levelArrTemp;//нужед ля сравнения пришедших трейсов

//        private ParamWithId trace1Type { get; set; } = new ParamWithId { Id = 0, Parameter = "BLAN" };

//        private EN.TraceType traceTypeResult = EN.TraceType.ClearWrite;

//        private ParamWithId trace1Detector { get; set; } = new ParamWithId { Id = 0, Parameter = "AutoSelect" };

//        private bool traceReset;

//        private readonly List<ParamWithId> traceTypes = new List<ParamWithId>
//        {
//            new ParamWithId {Id = (int) EN.TraceType.ClearWrite, Parameter = "WRIT" },
//            new ParamWithId {Id = (int) EN.TraceType.Average, Parameter = "AVER" },
//            new ParamWithId {Id = (int) EN.TraceType.MaxHold, Parameter = "MAXH" },
//            new ParamWithId {Id = (int) EN.TraceType.MinHold, Parameter = "MINH" },
//            new ParamWithId {Id = (int) EN.TraceType.View, Parameter = "VIEW" },
//            new ParamWithId {Id = (int) EN.TraceType.Blank, Parameter = "BLAN" }
//        };
//        private readonly List<ParamWithId> traceDetectors = new List<ParamWithId>
//        {
//            new ParamWithId {Id = (int) EN.TraceDetector.AutoSelect, Parameter = "Auto Select" },
//            new ParamWithId {Id = (int) EN.TraceDetector.AutoPeak, Parameter = "APE" },
//            new ParamWithId {Id = (int) EN.TraceDetector.Average, Parameter = "AVER" },
//            new ParamWithId {Id = (int) EN.TraceDetector.MaxPeak, Parameter = "POS" },
//            new ParamWithId {Id = (int) EN.TraceDetector.MinPeak, Parameter = "NEG" },
//            new ParamWithId {Id = (int) EN.TraceDetector.Sample, Parameter = "SAMP" },
//            new ParamWithId {Id = (int) EN.TraceDetector.RMS, Parameter = "RMS" }
//        };
//        #endregion

//        #region IQ
//        private decimal freqCentrIQ { get; set; } = 1000000000;
//        private decimal IQBW { get; set; } = 100000;
//        private decimal sampleSpeed { get; set; } = 100000;
//        private int sampleLength { get; set; } = 10000;
//        /// <summary>
//        /// Длительность одного семпла
//        /// </summary>
//        public decimal SampleTimeLength { get; set; } = 10000;
//        private decimal IQMeasTime { get; set; } = 10000;
//        private decimal IQMeasTimeAll { get; set; } = 10000;

//        #endregion IQ
//        #endregion Instr
//        #endregion Param

//        #region NI Visa
//        private void WriteString(string Input)
//        {
//            session.FormattedIO.WriteLine(Input);
//            //session.RawIO.Write(Input);
//        }
//        private string ReadString()
//        {
//            return session.FormattedIO.ReadString();
//        }
//        private string QueryString(string Input)
//        {
//            session.FormattedIO.WriteLine(Input);
//            return session.FormattedIO.ReadString();
//            //session.RawIO.Write(Input);
//            //return session.RawIO.ReadString().TrimEnd();
//        }
//        private byte[] ReadByte()
//        {
//            return session.FormattedIO.ReadBinaryBlockOfByte();
//        }
//        private byte[] QueryByte(string Input)
//        {
//            session.FormattedIO.WriteLine(Input);
//            return session.FormattedIO.ReadBinaryBlockOfByte();
//        }
//        private float[] QueryFloat(string Input)
//        {
//            session.FormattedIO.WriteLine(Input);
//            byte[] byteArray = session.FormattedIO.ReadBinaryBlockOfByte();
//            float[] temp = new float[byteArray.Length / 4];
//            for (int j = 0; j < temp.Length / 4; j++)
//            {
//                temp[j] = System.BitConverter.ToSingle(byteArray, j * 4);
//            }
//            return temp;
//        }
//        private decimal QueryDecimal(string Input)
//        {
//            session.FormattedIO.WriteLine(Input);
//            return decimal.Parse(session.FormattedIO.ReadString().Replace(".", decimalSeparator).Replace(",", decimalSeparator));
//        }
//        private int QueryInt(string Input)
//        {
//            session.FormattedIO.WriteLine(Input);
//            return int.Parse(session.FormattedIO.ReadString());
//        }
//        #endregion NI Visa


//        private void ValidateAndSetFreqStartStop(decimal freqStart, decimal freqStop)
//        {
//            if (FreqStart != freqStart || FreqStop != freqStop)
//            {
//                if (freqStart < FreqMin)
//                {
//                    FreqStart = FreqMin;
//                    logger.Warning(Contexts.ThisComponent, "FreqStart set to limit value");
//                }
//                else if (freqStart > FreqMax)
//                {
//                    FreqStart = FreqMax;
//                    logger.Warning(Contexts.ThisComponent, "FreqStart set to limit value");
//                }
//                else FreqStart = freqStart;
//                SetFreqStart(FreqStart);
//                if (FreqStop > FreqMax)
//                {
//                    FreqStop = FreqMax;
//                    logger.Warning(Contexts.ThisComponent, "FreqStop set to limit value");
//                }
//                else if (FreqStop < FreqMin)
//                {
//                    FreqStop = FreqMin + 1000000;
//                    logger.Warning(Contexts.ThisComponent, "FreqStop set to limit value");
//                }
//                else FreqStop = freqStart;
//                SetFreqStop(FreqStop);
//            }
//        }
//        private void ValidateAndSetRefLevel(int refLevel)
//        {
//            decimal refl = 0;
//            if (refLevel == 1000000000)
//            {
//                refl = -20;
//            }
//            else
//            {
//                if (refLevel > RefLevelMax)
//                {
//                    refl = RefLevelMax;
//                    logger.Warning(Contexts.ThisComponent, "Reference level set to limit value");
//                }
//                else if (refLevel < RefLevelMin)
//                {
//                    refl = RefLevelMin;
//                    logger.Warning(Contexts.ThisComponent, "Reference level set to limit value");
//                }
//                else
//                {
//                    refl = refLevel;
//                }
//            }
//            SetRefLevel(refl);
//        }
//        private void ValidateAndSetPreAmp(int preAmp)
//        {
//            if (preAmpAvailable)
//            {
//                bool preamp = false;
//                if (preAmp == -1)
//                {
//                    preamp = false;//потому что нет альтернативы, в любом анализаторе спектра нет автоматического усилителя, 
//                    //Если вы работете с анализатором спектра то понимаете что вы делаете
//                }
//                else if (preAmp == 0)
//                {
//                    preamp = false;
//                }
//                else if (preAmp == 1)
//                {
//                    preamp = true;
//                }
//                else if (preAmp > 1 || preAmp < -1)
//                {
//                    logger.Warning(Contexts.ThisComponent, "PreAmp must be set to within limits.");
//                }
//                SetPreAmp(preamp);
//            }
//            else
//            {
//                logger.Warning(Contexts.ThisComponent, "PreAmp unavailable");
//            }
//        }
//        private void ValidateAndSetATT(int att)
//        {
//            if (att > -1)
//            {
//                int res = 0;
//                int delta = int.MaxValue;
//                for (int i = 0; i <= attMax; i += attStep)
//                {
//                    if (Math.Abs(att - i) < delta)
//                    {
//                        delta = Math.Abs(att - i);
//                        res = i;
//                    }
//                }
//                SetAttLevel(res);
//            }
//            else
//            {
//                SetAutoAtt(true);
//            }
//        }

//        private bool SetConnect()
//        {
//            bool res = false;
//            rm = new ResourceManager();

//            session = (TcpipSession)rm.Open(String.Concat("TCPIP::", adapterConfig.IPAddress, "::hislip"));//, AccessModes.None, 20000);

//            string[] temp = QueryString("*IDN?").Trim('"').Split(',');
//            if (temp[0].Contains("Rohde&Schwarz"))
//            {
//                if (temp[1].Contains("FPL"))
//                {
//                    instrModel = temp[1];
//                    serialNumber = temp[2];

//                    List<DeviceOption> Loaded = new List<DeviceOption>() { };
//                    loadedInstrOption = new List<DeviceOption>();
//                    string[] op = QueryString("*OPT?").ToUpper().Split(',');
//                    if (op.Length > 0 && op[0] != "0")
//                    {
//                        bool findDemoOption = false;
//                        foreach (string s in op)
//                        {
//                            if (s.ToUpper() == "K0")
//                            {
//                                findDemoOption = true;
//                                Loaded = instrOption;
//                            }
//                        }
//                        if (findDemoOption == false)
//                        {
//                            foreach (string s in op)
//                            {
//                                foreach (DeviceOption so in instrOption)
//                                {
//                                    if (so.Type == s)
//                                    {
//                                        Loaded.Add(so);
//                                    }
//                                }

//                            }
//                        }
//                    }
//                    loadedInstrOption = Loaded;


//                    SetPreset();

//                    WriteString("FORMat:DEXPort:DSEParator COMM");// POIN");//разделитель дробной части
//                    WriteString(":FORM:DATA REAL,32");// ASC");//передавать трейс в ASCII

//                    if (adapterConfig.DisplayUpdate)
//                    {
//                        WriteString(":SYST:DISP:UPD ON");
//                    }
//                    else
//                    {
//                        WriteString(":SYST:DISP:UPD OFF");
//                    }

//                    SweepPoints = QueryInt(":SWE:POIN?");

//                    //session.DefaultBufferSize = SweepPoints * 18 + 25; //увеличиваем буфер чтобы влезло 32001 точка трейса
//                    FreqMin = QueryDecimal(":SENSe:FREQuency:STAR? MIN");
//                    FreqMax = QueryDecimal(":SENSe:FREQuency:STOP? MAX");

//                    preAmpAvailable = false;
//                    if (loadedInstrOption.Count > 0)
//                    {
//                        for (int i = 0; i < loadedInstrOption.Count; i++)
//                        {
//                            if (loadedInstrOption[i].GlobalType == "Preamplifier") { preAmpAvailable = true; }
//                            if (loadedInstrOption[i].Type == "B25") { attStep = 1; }
//                        }
//                    }
//                    SweepPointsIndex = System.Array.IndexOf(sweepPointArr, SweepPoints);

//                    #region

//                    GetLevelUnit();
//                    GetFreqCentr();
//                    GetFreqSpan();
//                    GetRBW();
//                    GetAutoRBW();
//                    GetVBW();
//                    GetAutoVBW();

//                    GetOptimization();
//                    SetOptimization((EN.Optimization)adapterConfig.Optimization);

//                    GetSweepTime();
//                    GetAutoSweepTime();
//                    GetSweepType();
//                    GetSweepPoints();

//                    GetRefLevel();
//                    GetRange();
//                    GetAttLevel();
//                    GetAutoAttLevel();
//                    GetPreAmp();
//                    GetTraceType();
//                    GetDetectorType();


//                    SetIQWindow();
//                    SetWindowType(false);
//                    GetRefLevel();
//                    GetRange();
//                    GetAttLevel();
//                    GetAutoAttLevel();
//                    GetPreAmp();
//                    SetWindowType(true);


//                    res = true;
//                    //IsRuning = true;
//                    #endregion
//                }
//                else
//                {
//                    throw new Exception("The device is not Rohde&Schwarz FPL");
//                }
//            }
//            else
//            {
//                throw new Exception("The device is not Rohde&Schwarz");
//            }
//            return res;
//        }




//        #region AN To Command

//        private void GetFreqCentr()
//        {

//            FreqCentr = QueryDecimal(":SENSe:FREQuency:CENTer?");

//            GetFreqArr();
//        }
//        private void GetFreqSpan()
//        {
//            FreqSpan = QueryDecimal(":SENSe:FREQuency:SPAN?");
//            GetFreqArr();
//        }
//        private bool SetFreqStart(decimal freqStart)
//        {
//            bool res = false;
//            FreqStart = freqStart;
//            WriteString(":SENSe:FREQ:STAR " + FreqStart.ToString().Replace(decimalSeparator, "."));
//            FreqStart = QueryDecimal(":SENSe:FREQ:STAR?");
//            res = true;

//            GetFreqArr();
//            if (AutoRBW == true) { GetRBW(); }
//            if (AutoVBW == true) { GetVBW(); }
//            if (AutoSweepTime == true) { GetSweepTime(); }

//            return res;
//        }
//        private bool SetFreqStop(decimal freqStop)
//        {
//            bool res = false;
//            FreqStop = freqStop;
//            WriteString(":SENSe:FREQ:STOP " + FreqStop.ToString().Replace(decimalSeparator, "."));
//            FreqStop = QueryDecimal(":SENSe:FREQ:STOP?");

//            res = true;
//            GetFreqArr();
//            if (AutoRBW == true) { GetRBW(); }
//            if (AutoVBW == true) { GetVBW(); }
//            if (AutoSweepTime == true) { GetSweepTime(); }

//            return res;
//        }

//        private void GetFreqArr()
//        {
//            byte[] byteArray = QueryByte("TRAC1:X? TRACE1");

//            double[] temp = new double[byteArray.Length / 4 + 2];
//            for (int j = 0; j < byteArray.Length / 4; j++)
//            {
//                temp[j + 1] = System.BitConverter.ToSingle(byteArray, j * 4);
//            }
//            temp[0] = (double)FreqStart;
//            temp[temp.Length - 1] = (double)FreqStop;
//            FreqArr = temp;
//            LevelArr = new float[FreqArr.Length];
//            levelArrTemp = new float[FreqArr.Length];
//            for (int i = 0; i < FreqArr.Length; i++)
//            {
//                LevelArr[i] = -1000;
//                levelArrTemp[i] = -1000;
//            }
//            TracePoints = LevelArr.Length;
//        }

//        private void SetFreqCentrIQ(decimal freqCentrIQ)
//        {
//            this.freqCentrIQ = freqCentrIQ;
//            WriteString($"FREQ:CENT {this.freqCentrIQ.ToString().Replace(decimalSeparator, ".")}");
//            this.freqCentrIQ = QueryDecimal(":FREQ:CENT?");
//        }

//        private void SetAutoAttLevel(bool attAuto)
//        {
//            if (mode)
//            {
//                attAutoSpec = attAuto;
//                if (attAutoSpec == true)
//                {
//                    WriteString(":INP:ATT:AUTO 1");
//                }
//                else
//                {
//                    WriteString(":INP:ATT:AUTO 0");
//                }
//                AttLevelSpec = QueryDecimal(":INP:ATT?");
//            }
//            else
//            {
//                attAutoIQ = attAuto;
//                if (attAutoIQ == true)
//                {
//                    WriteString(":INP:ATT:AUTO 1");
//                }
//                else
//                {
//                    WriteString(":INP:ATT:AUTO 0");
//                }
//                AttLevelIQ = QueryDecimal(":INP:ATT?");
//            }
//        }

//        private void SetAttLevel(decimal attLevel)
//        {
//            if (mode)
//            {
//                AttLevelSpec = attLevel;
//                attAutoSpec = false;
//                WriteString(":INP:ATT " + AttLevelSpec.ToString().Replace(decimalSeparator, ".")); //INP:ATT:AUTO

//            }
//            else
//            {
//                AttLevelIQ = attLevel;
//                attAutoIQ = false;
//                WriteString(":INP:ATT " + AttLevelIQ.ToString().Replace(decimalSeparator, ".")); //INP:ATT:AUTO
//            }
//        }

//        private void GetAttLevel()
//        {
//            if (mode)
//            {
//                AttLevelSpec = QueryDecimal(":INP:ATT?");
//            }
//            else
//            {
//                AttLevelIQ = QueryDecimal(":INP:ATT?");
//            }
//        }

//        private void SetAutoAtt(bool attauto)
//        {
//            if (mode)
//            {
//                attAutoSpec = attauto;
//                if (attAutoSpec == true)
//                {
//                    WriteString(":INP:ATT:AUTO 1");
//                }
//                else
//                {
//                    WriteString(":INP:ATT:AUTO 0");
//                }
//                AttLevelSpec = QueryDecimal(":INP:ATT?");
//            }
//            else
//            {
//                attAutoIQ = attauto;
//                if (attAutoIQ == true)
//                {
//                    WriteString(":INP:ATT:AUTO 1");
//                }
//                else
//                {
//                    WriteString(":INP:ATT:AUTO 0");
//                }
//                AttLevelIQ = QueryDecimal(":INP:ATT?");
//            }
//        }

//        private void GetAutoAttLevel()
//        {
//            if (mode)
//            {
//                string temp = "";
//                temp = QueryString(":INP:ATT:AUTO?");
//                if (temp.Contains("1"))
//                {
//                    attAutoSpec = true;
//                }
//                else if (temp.Contains("0"))
//                {
//                    attAutoSpec = false;
//                }
//            }
//            else
//            {
//                string temp = "";
//                temp = QueryString(":INP:ATT:AUTO?");
//                if (temp.Contains("1"))
//                {
//                    attAutoIQ = true;
//                }
//                else if (temp.Contains("0"))
//                {
//                    attAutoIQ = false;
//                }

//            }
//        }

//        private void SetPreAmp(bool preAmp)
//        {
//            if (mode)
//            {
//                preAmpSpec = preAmp;
//                if (preAmpAvailable == true)
//                {
//                    if (preAmpSpec == true)
//                    {
//                        WriteString(":INP:GAIN:STAT 1");
//                    }
//                    else
//                    {
//                        WriteString(":INP:GAIN:STAT 0");
//                    }
//                }
//            }
//            else
//            {
//                preAmpIQ = preAmp;
//                if (preAmpAvailable == true)
//                {
//                    if (preAmpIQ == true)
//                    {
//                        WriteString(":INP:GAIN:STAT 1");
//                    }
//                    {
//                        WriteString(":INP:GAIN:STAT 0");
//                    }
//                }
//            }
//        }

//        private void GetPreAmp()
//        {
//            if (mode)
//            {
//                if (preAmpAvailable)
//                {
//                    string temp = QueryString(":INP:GAIN:STAT?");
//                    if (temp.Contains("1")) { preAmpSpec = true; }
//                    else if (temp.Contains("0")) { preAmpSpec = false; }
//                }
//            }
//            else
//            {
//                if (preAmpAvailable)
//                {
//                    string temp = QueryString(":INP:GAIN:STAT?");
//                    if (temp.Contains("1")) { preAmpIQ = true; }
//                    else if (temp.Contains("0")) { preAmpIQ = false; }
//                }
//            }
//        }

//        private void SetRefLevel(decimal refLevel)
//        {
//            if (mode)
//            {
//                RefLevelSpec = refLevel;
//                WriteString(":DISPlay:WINDow:TRACe:Y:SCALe:RLEVel " + RefLevelSpec.ToString().Replace(decimalSeparator, "."));
//                if (attAutoSpec) { GetAttLevel(); }
//            }
//            else
//            {
//                RefLevelIQ = refLevel;
//                WriteString(":DISPlay:WINDow:TRACe:Y:SCALe:RLEVel " + RefLevelIQ.ToString().Replace(decimalSeparator, "."));
//                if (attAutoIQ == true) { GetAttLevel(); }
//            }
//        }

//        private void GetRefLevel()
//        {
//            if (mode)
//            {
//                RefLevelSpec = Math.Round(QueryDecimal(":DISPlay:WINDow:TRACe:Y:SCALe:RLEVel?"));
//            }
//            else
//            {
//                RefLevelIQ = Math.Round(QueryDecimal(":DISPlay:WINDow:TRACe:Y:SCALe:RLEVel?"));
//            }
//        }

//        private void SetRange()
//        {
//            if (mode)
//            {
//                WriteString(":DISP:TRAC:Y " + RangeSpec.ToString().Replace(decimalSeparator, "."));
//                if (attAutoSpec == true) { GetAttLevel(); }
//            }
//            else
//            {
//                WriteString(":DISP:TRAC:Y " + RangeIQ.ToString().Replace(decimalSeparator, "."));
//                if (attAutoIQ == true) { GetAttLevel(); }
//            }
//        }

//        private void GetRange()
//        {
//            if (mode)
//            {
//                RangeSpec = Math.Round(QueryDecimal(":DISP:TRAC:Y?"));
//            }
//            else
//            {
//                RangeIQ = Math.Round(QueryDecimal(":DISP:TRAC:Y?"));
//            }
//        }

//        private void SetLevelUnit(ParamWithId levelUnit)
//        {
//            this.levelUnit = levelUnit;
//            WriteString(":UNIT:POWer " + this.levelUnit.Parameter);
//        }

//        private void GetLevelUnit()
//        {
//            string temp = QueryString(":UNIT:POWer?");
//            for (int i = 0; i < levelUnits.Count; i++)
//            {
//                if (temp.ToLower() == levelUnits[i].Parameter.ToLower())
//                {
//                    levelUnit = levelUnits[i];
//                }
//            }
//        }

//        private void SetRBW(decimal rbw)
//        {
//            RBW = rbw;
//            WriteString(":SENSe:BWIDth:RESolution " + RBW.ToString().Replace(decimalSeparator, "."));

//            AutoRBW = false;
//            if (AutoSweepTime == true) { GetSweepTime(); }
//            if (AutoVBW == true) { GetVBW(); }
//            GetSweepType();
//        }
//        private void GetRBW()
//        {
//            RBW = QueryDecimal(":SENSe:BWIDth:RESolution?");
//            RBWIndex = System.Array.IndexOf(RBWArr, RBW);
//        }

//        private void SetAutoRBW(bool autoRBV)
//        {
//            AutoRBW = autoRBV;
//            if (AutoRBW)
//            {
//                WriteString(":SENSe:BWIDth:RESolution:AUTO 1");
//            }
//            else
//            {
//                WriteString(":SENSe:BWIDth:RESolution:AUTO 0");
//            }

//            if (AutoSweepTime == true) { GetSweepTime(); }
//            GetRBW();
//            GetVBW();
//            GetSweepType();
//        }
//        private void GetAutoRBW()
//        {
//            string s = QueryString(":SENSe:BWIDth:RESolution:AUTO?");
//            if (s.Contains("1"))
//            {
//                AutoRBW = true;
//            }
//            else
//            {
//                AutoRBW = false;
//            }
//            if (AutoSweepTime == true) { GetSweepTime(); }
//        }

//        private void SetVBW(decimal vbw)
//        {
//            VBW = vbw;
//            WriteString(":SENSe:BANDwidth:VIDeo " + VBW.ToString().Replace(decimalSeparator, "."));

//            AutoVBW = false;
//            GetSweepType();
//        }
//        private void GetVBW()
//        {
//            VBW = QueryDecimal(":SENSe:BANDwidth:VIDeo?");
//            VBWIndex = System.Array.IndexOf(VBWArr, VBW);
//        }

//        private void SetAutoVBW(bool autoVBW)
//        {
//            AutoVBW = autoVBW;
//            if (AutoVBW)
//            {
//                WriteString(":SENSe:BANDwidth:VIDeo:AUTO 1");
//            }
//            else
//            {
//                WriteString(":SENSe:BANDwidth:VIDeo:AUTO 0");
//            }

//            if (AutoVBW == true) { GetVBW(); }
//            if (AutoSweepTime == true) { GetSweepTime(); }
//            GetSweepType();
//        }
//        private void GetAutoVBW()
//        {
//            if (QueryString(":SENSe:BANDwidth:VIDeo:AUTO?").Contains("1"))
//            {
//                AutoVBW = true;
//            }
//            else
//            {
//                AutoVBW = false;
//            }
//        }

//        private void SetSweepType()
//        {
//            WriteString(":SWE:TYPE " + SweepTypeSelected.Parameter);
//        }
//        private void GetSweepType()
//        {
//            string t = QueryString(":SWE:TYPE?");
//            foreach (ParamWithId ST in SweepType)
//            {
//                if (t.TrimEnd().ToLower() == ST.Parameter.ToLower()) { SweepTypeSelected = ST; }
//            }
//        }

//        private void GetSweepTime()
//        {
//            SweepTime = QueryDecimal(":SWE:TIME?");
//        }
//        private void SetSweepTime(decimal sweepTime)
//        {
//            SweepTime = sweepTime;
//            WriteString(":SWE:TIME " + SweepTime.ToString());
//            SweepTime = QueryDecimal(":SWE:TIME?");
//            AutoSweepTime = false;
//        }

//        private void SetAutoSweepTime(bool autoSweepTime)
//        {
//            AutoSweepTime = autoSweepTime;
//            if (AutoSweepTime)
//            {
//                WriteString(":SWE:TIME:AUTO 1");
//            }
//            else
//            {
//                WriteString(":SWE:TIME:AUTO 0");
//            }
//            GetSweepTime();
//        }
//        private void GetAutoSweepTime()
//        {
//            if (QueryString(":SWE:TIME:AUTO?").Contains("0"))
//            {
//                AutoSweepTime = false;
//            }
//            else
//            {
//                AutoSweepTime = true;
//            }
//        }

//        private void SetSweepPoints(int sweepPoints)
//        {
//            SweepPoints = sweepPoints;
//            WriteString(":SWE:POIN " + SweepPoints.ToString());
//            GetFreqArr();
//        }

//        private void GetSweepPoints()
//        {
//            SweepPoints = QueryInt(":SWE:POIN?");
//            SweepPointsIndex = System.Array.IndexOf(sweepPointArr, SweepPoints);
//            GetFreqArr();
//        }

//        private void SetOptimization(EN.Optimization optimization)
//        {
//            Optimization = optimization;
//            WriteString("SENSe:SWEep:OPTimize " + OptimizationSelected.Parameter);
//        }
//        private void GetOptimization()
//        {
//            string temp1 = string.Empty;
//            temp1 = QueryString("SENSe:SWEep:OPTimize?");
//            foreach (ParamWithId TT in optimization)
//            {
//                if (temp1.Contains(TT.Parameter))
//                {
//                    Optimization = (EN.Optimization)TT.Id;
//                }
//            }
//        }

//        private void SetTraceType(ParamWithId trace1Type)
//        {
//            this.trace1Type = trace1Type;
//            WriteString(":DISP:TRAC1:MODE " + this.trace1Type.Parameter);
//        }
//        private void GetTraceType()
//        {
//            string temp1 = string.Empty;
//            if (QueryString(":DISP:TRAC1?").Contains("1"))
//            {
//                temp1 = QueryString(":DISP:TRAC1:MODE?");
//            }
//            else
//            {
//                temp1 = "BLAN";
//            }
//            foreach (ParamWithId TT in traceTypes)
//            {
//                if (temp1.Contains(TT.Parameter))
//                {
//                    trace1Type = TT;
//                }
//            }
//        }

//        private void SetDetectorType(ParamWithId trace1detector)
//        {
//            this.trace1Detector = trace1detector;
//            if (this.trace1Detector.Parameter == "Auto Select") WriteString(":SENSe:DET1:AUTO 1");
//            else
//            {
//                WriteString(":SENSe:DET1 " + this.trace1Detector.Parameter);
//            }
//        }
//        private void GetDetectorType()
//        {
//            string temp1 = string.Empty;
//            temp1 = QueryString(":SENSe:DET1?");

//            foreach (ParamWithId TT in traceDetectors)
//            {
//                if (temp1.Contains(TT.Parameter)) { trace1Detector = TT; }
//            }
//        }

//        private void SetIQBW(decimal iqbw)
//        {
//            IQBW = iqbw;
//            sampleSpeed = 1.25m * IQBW;
//            SampleTimeLength = 1 / sampleSpeed;
//            string s = "TRAC:IQ:BWID " + IQBW.ToString().Replace(decimalSeparator, ".");
//            WriteString(s);

//        }
//        private void SetSampleSpeed(decimal sampleSpeed)
//        {
//            this.sampleSpeed = sampleSpeed;
//            IQBW = 0.8m * this.sampleSpeed;
//            SampleTimeLength = 1 / this.sampleSpeed;
//            WriteString("TRAC:IQ:SRAT " + this.sampleSpeed.ToString().Replace(decimalSeparator, "."));
//        }
//        private void SetSampleLength(int sampleLength)
//        {
//            this.sampleLength = sampleLength;
//            WriteString("TRAC:IQ:RLEN " + this.sampleLength.ToString());
//        }

//        public void SetIQWindow()
//        {
//            string str = "INST:CRE IQ, 'IQ Analyzer'";
//            WriteString(str);
//            WriteString("INIT:CONT OFF");
//            WriteString("TRIG:SOUR EXT");
//            WriteString("TRACe:IQ:DATA:FORMat IQPair");
//        }

//        public void SetWindowType(bool mode)
//        {
//            this.mode = mode;
//            string str = "";
//            if (this.mode)
//            {
//                str = "INST SAN";
//            }
//            else
//            {
//                str = "INST IQ";
//            }
//            WriteString(str);
//            WriteString("FORM:DATA REAL,32");//так надо
//        }
//        private void SetPreset()
//        {
//            WriteString(":SYSTem:PRES");
//        }
//        private void GetSetAnSysDateTime()
//        {
//            try
//            {
//                string[] d = QueryString("SYST:DATE?").TrimEnd().Trim(' ').Split(',');
//                string[] t = QueryString("SYST:TIME?").TrimEnd().Trim(' ').Split(',');
//                string time = d[0] + "-" + d[1] + "-" + d[2] + " " +
//                    t[0] + ":" + t[1] + ":" + t[2];
//                DateTime andt = DateTime.Parse(time);
//                if (new TimeSpan(DateTime.Now.Ticks - andt.Ticks) > new TimeSpan(0, 0, 1, 0, 0))
//                {
//                    WriteString("SYST:DATE " + DateTime.Now.Year.ToString() + "," + DateTime.Now.Month.ToString() + "," + DateTime.Now.Day.ToString());
//                    WriteString("SYST:TIME " + DateTime.Now.Hour.ToString() + "," + DateTime.Now.Minute.ToString() + "," + DateTime.Now.Second.ToString());
//                }
//            }
//            #region Exception
//            catch (VisaException v_exp)
//            {
//                logger.Exception(Contexts.ThisComponent, v_exp);
//            }
//            catch (Exception exp)
//            {
//                logger.Exception(Contexts.ThisComponent, exp);
//            }
//            #endregion
//        }
//        #endregion AN To Command

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
//                AttMax_dB = (int)attMax,
//                AttMin_dB = 0,
//                FreqMax_Hz = FreqMax,
//                FreqMin_Hz = FreqMin,
//                PreAmpMax_dB = 1, //типа включен/выключен, сколько по факту усиливает нигде не пишется кроме FSW где их два 15/30 и то два это опция
//                PreAmpMin_dB = 0,
//                RefLevelMax_dBm = (int)RefLevelMax,
//                RefLevelMin_dBm = (int)RefLevelMin,
//                EquipmentInfo = new EquipmentInfo()
//                {
//                    AntennaCode = config.AdapterEquipmentInfo.AntennaSN,// "Omni",//S/N  В конфиг
//                    AntennaManufacturer = config.AdapterEquipmentInfo.AntennaManufacturer,//"3anet",//В конфиг
//                    AntennaName = config.AdapterEquipmentInfo.AntennaName,//"BC600",//В конфиг
//                    EquipmentManufacturer = new Atdi.DataModels.Sdrn.DeviceServer.Adapters.InstrManufacrures().RuS.UI,
//                    EquipmentName = instrModel,
//                    EquipmentFamily = "SpectrumAnalyzer",//SDR/SpecAn/MonRec
//                    EquipmentCode = serialNumber,//S/N

//                },
//                RadioPathParameters = rrps
//            };
//            if (preAmpAvailable)
//            {
//                sdp.PreAmpMax_dB = 1;
//            }
//            else
//            {
//                sdp.PreAmpMax_dB = 0;
//            }
//            MesureTraceDeviceProperties mtdp = new MesureTraceDeviceProperties()
//            {
//                RBWMax_Hz = (double)RBWArr[RBWArr.Length - 1],
//                RBWMin_Hz = (double)RBWArr[0],
//                SweepTimeMin_s = (double)SWTMin,
//                SweepTimeMax_s = (double)SWTMax,
//                StandardDeviceProperties = sdp,
//                //DeviceId ничего не писать, ID этого экземпляра адаптера
//            };
//            MesureIQStreamDeviceProperties miqdp = new MesureIQStreamDeviceProperties()
//            {
//                AvailabilityPPS = false,// Т.к. нет у анализаторов спектра их, хотя через тригеры можно попробывать
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
