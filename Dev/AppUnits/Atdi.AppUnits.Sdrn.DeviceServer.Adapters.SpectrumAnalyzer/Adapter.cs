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
using System.Diagnostics;
using System.Threading;

namespace Atdi.AppUnits.Sdrn.DeviceServer.Adapters.SpectrumAnalyzer
{
    /// <summary>
    /// Пример реализации объект аадаптера
    /// </summary>
    public class Adapter : IAdapter
    {
        /// <summary>
        /// подумать на тему защиты прибора пока я ничего не делаю
        /// </summary>
        #region интересные команды
        /// <summary>
        /// стр 217 FORMat:DEXPort:DSEParator COMMa/POINt неожиданно нарыл разделитель дробной части
        /// стр 144 TRACe:IQ:SET NORM, 0, <SampleRate>, <TriggerMode>, <TriggerSlope>,<PretriggerSamp>, <NumberSamples>
        /// стр 145 TRACe:IQ:TPISample?
        /// стр 149 [SENSe:]ADJust:LEVel
        /// стр 206 TRACe:IQ:DATA? в старом мануале
        /// стр 212 TRACe:IQ:DATA:FORMat IQPair
        /// стр 212 TRACe:IQ:DATA:MEMory?  (Обратите внимание, однако, что команда TRAC: IQ: DATA? 
        /// Инициирует новое измерение перед возвратом захваченных значений, а не с возвратом существующих данных в памяти.)
        /// стр 214 FORMat[:DATA] FORM REAL,32 FORM REAL,16 FORM REAL,64
        /// </summary>
        #endregion

        #region Adapter
        private readonly ITimeService _timeService;
        private readonly ILogger _logger;
        private readonly AdapterConfig _adapterConfig;


        private LocalParametersConverter LPC;
        private CFG.ThisAdapterConfig TAC;
        private CFG.AdapterMainConfig MainConfig;

        /// <summary>
        /// Все объекты адаптера создаются через DI-контейнер 
        /// Запрашиваем через конструктор необходимые сервисы
        /// </summary>
        /// <param name="adapterConfig"></param>
        /// <param name="logger"></param>
        /// <param name="timeService"></param>
        public Adapter(AdapterConfig adapterConfig, ILogger logger, ITimeService timeService)
        {
            this._logger = logger;
            this._adapterConfig = adapterConfig;
            this._timeService = timeService;
            LPC = new LocalParametersConverter();
            FreqArr = new double[TracePoints];
            LevelArr = new float[TracePoints];
            for (int i = 0; i < TracePoints; i++)
            {
                FreqArr[i] = (double)(FreqStart + 1000 * i);
                LevelArr[i] = -100;
            }
        }

        /// <summary>
        /// Метод будет вызван при инициализации потока воркера адаптера
        /// Адаптеру необходимо зарегестрировать свои обработчики комманд 
        /// </summary>
        /// <param name="host"></param>
        public void Connect(IAdapterHost host)
        {
            try
            {

                /// включем устройство
                /// иницируем его параметрами сконфигурации
                /// проверяем к чем оно готово

                /// сообщаем инфраструктуре что мы готовы обрабатывать комманду MesureGpsLocationExampleCommand
                /// и при этом возвращать оезультат в типе MesureGpsLocationExampleAdapterResult
                if (SetConnect())
                {
                    string filename = "SpectrumAnalyzer_" + UniqueData.SerialNumber + ".xml";
                    TAC = new CFG.ThisAdapterConfig() { };
                    if (!TAC.GetThisAdapterConfig(filename))
                    {
                        MainConfig = new CFG.AdapterMainConfig() { };
                        SetDefaulConfig(ref MainConfig);
                        TAC.SetThisAdapterConfig(MainConfig, filename);
                    }
                    else
                    {
                        MainConfig = TAC.Main;
                    }
                    (MesureTraceDeviceProperties mtdp, MesureIQStreamDeviceProperties miqdp) = GetProperties(MainConfig);
                    host.RegisterHandler<COM.MesureTraceCommand, COMR.MesureTraceResult>(MesureTraceCommandHandler, mtdp);
                    if (UniqueData.IQAvailable)
                    {
                        //недопилено пока нет определенности со временем 
                        //host.RegisterHandler<COM.MesureIQStreamCommand, COMR.MesureIQStreamResult>(MesureIQStreamCommandHandler, miqdp);
                    }
                }

            }
            #region Exception
            catch (Exception exp)
            {
                _logger.Exception(Contexts.ThisComponent, exp);
                throw new InvalidOperationException("Invalid initialize/connect adapter", exp);
            }
            #endregion
        }

        /// <summary>
        /// Метод вызывается контрллером когда необходимо выгрузит адаптер с памяти
        /// </summary>
        public void Disconnect()
        {
            try
            {
                /// освобождаем ресурсы и отключаем устройство
                session.Dispose();
                IsRuning = false;

            }
            #region Exception
            catch (Exception exp)
            {
                _logger.Exception(Contexts.ThisComponent, exp);
            }
            #endregion
        }


        public void MesureTraceCommandHandler(COM.MesureTraceCommand command, IExecutionContext context)
        {
            try
            {
                if (IsRuning)
                {
                    /// если нужно проводим блокировку комманд который мы не сможем обслужить пока что то меряем в течении выполнени яэтой комманды
                    /// и подсказывая этим инфраструктуре что этот устрйостов некоторое время не сможет обрабатываить такие комманды
                    //context.Lock(CommandType.MesureTrace);

                    // если нужно заблокировать выполняему комманду то достатчоно вызвать метод без параметров и блокируется комманда которая выполняется
                    context.Lock();
                    //Переключимся на Spectrum
                    if (Mode != EN.Mode.SpectrumAnalyzer)
                    {
                        SetWindowType(EN.Mode.SpectrumAnalyzer);
                    }

                    if (FreqStart != command.Parameter.FreqStart_Hz)
                    {
                        FreqStart = LPC.FreqStart(UniqueData, command.Parameter.FreqStart_Hz);
                        SetFreqStart(FreqStart);
                    }
                    if (FreqStop != command.Parameter.FreqStop_Hz)
                    {
                        FreqStop = LPC.FreqStop(UniqueData, command.Parameter.FreqStop_Hz);
                        SetFreqStop(FreqStop);
                    }

                    //Установим опорный уровень
                    if (RefLevelSpec != command.Parameter.RefLevel_dBm)
                    {
                        //типа авто...
                        if (command.Parameter.RefLevel_dBm == 1000000000)
                        {
                            RefLevelSpec = LPC.RefLevel(UniqueData, -20);
                        }
                        else
                        {
                            RefLevelSpec = LPC.RefLevel(UniqueData, command.Parameter.RefLevel_dBm);
                        }
                        SetRefLevel(RefLevelSpec);
                    }

                    //Установим предусилитель при наличии
                    if (UniqueData.PreAmp)//доступен ли вообще предусилитель
                    {
                        bool preamp = LPC.PreAmp(UniqueData, command.Parameter.PreAmp_dB);
                        if (preamp != PreAmpSpec)
                        {
                            SetPreAmp(preamp);
                        }
                    }

                    //Установим аттенюатор
                    (decimal att, bool auto) = LPC.Attenuator(UniqueData, command.Parameter.Att_dB);
                    if (auto)
                    {
                        SetAutoAtt(auto);
                    }
                    else
                    {
                        if (AttLevelSpec != att)
                        {
                            SetAttLevel(att);
                        }
                    }





                    //кол-во точек посчитать по RBW
                    if (command.Parameter.TracePoint == -1 && command.Parameter.RBW_Hz > 0)
                    {
                        //RBW ближайшее меньшее
                        RBW = LPC.RBW(UniqueData, (decimal)command.Parameter.RBW_Hz);
                        SetRBW(RBW);
                        if (command.Parameter.VBW_Hz < 0) //Если авто то VBW = RBW
                        {
                            VBW = LPC.VBW(UniqueData, RBW);// но проверим на доступность
                            SetVBW(VBW);
                        }
                        else
                        {
                            VBW = LPC.VBW(UniqueData, (decimal)command.Parameter.VBW_Hz);
                            SetVBW(VBW);
                        }

                        int needsweeppints = (int)(FreqSpan / RBW);
                        int sweeppoints = LPC.SweepPoints(UniqueData, needsweeppints);
                        SetSweepPoints(sweeppoints);
                    }
                    //сколько точек понятно по нему и посчитать RBW
                    else if (command.Parameter.TracePoint > 0 && command.Parameter.RBW_Hz == -1)
                    {
                        int sweeppoints = LPC.SweepPoints(UniqueData, command.Parameter.TracePoint);
                        SetSweepPoints(sweeppoints);

                        decimal needrbw = LPC.RBW(UniqueData, FreqSpan / ((decimal)(sweeppoints - 1)));
                        SetRBW(needrbw);
                        if (command.Parameter.VBW_Hz < 0) //Если авто то VBW = RBW
                        {
                            VBW = LPC.VBW(UniqueData, RBW);// но проверим на доступность
                            SetVBW(VBW);
                        }
                        else
                        {
                            VBW = LPC.VBW(UniqueData, (decimal)command.Parameter.VBW_Hz);
                            SetVBW(VBW);
                        }
                    }
                    //сколько точек понятно по нему и посчитать RBW
                    else if (command.Parameter.TracePoint > 0 && command.Parameter.RBW_Hz > 0)
                    {
                        int sweeppoints = LPC.SweepPoints(UniqueData, command.Parameter.TracePoint);
                        SetSweepPoints(sweeppoints);

                        RBW = LPC.RBW(UniqueData, (decimal)command.Parameter.RBW_Hz);
                        SetRBW(RBW);
                        if (command.Parameter.VBW_Hz < 0) //Если авто то VBW = RBW
                        {
                            VBW = LPC.VBW(UniqueData, RBW);// но проверим на доступность
                            SetVBW(VBW);
                        }
                        else
                        {
                            VBW = LPC.VBW(UniqueData, (decimal)command.Parameter.VBW_Hz);
                            SetVBW(VBW);
                        }
                    }

                    #region DetectorType
                    ParamWithId DetId = LPC.DetectorType(UniqueData, command.Parameter.DetectorType);
                    if (Trace1Detector != DetId)
                    {
                        SetDetectorType(DetId);
                    }
                    #endregion DetectorType

                    #region TraceType
                    (ParamWithId TTId, EN.TraceType res) = LPC.TraceType(UniqueData, command.Parameter.TraceType);
                    //на прибор
                    if (Trace1Type != TTId)
                    {
                        SetTraceType(TTId);
                    }
                    //Такой результат
                    if (TraceTypeResult != res)
                    {
                        TraceTypeResult = res;
                    }
                    #endregion TraceType

                    #region LevelUnit
                    (ParamWithId lu, MEN.LevelUnit lur) = LPC.LevelUnit(UniqueData, command.Parameter.LevelUnit);
                    if (lur != LevelUnitsResult || LevelUnits != lu)
                    {
                        LevelUnitsResult = lur;
                        SetLevelUnit(lu);
                    }
                    //для ускорения усреднения установим levelunit в Ваты 
                    if (TraceTypeResult == EN.TraceType.Average)
                    {
                        for (int i = 0; i < UniqueData.LevelUnits.Count; i++)
                        {
                            if (UniqueData.LevelUnits[i].Id == (int)MEN.LevelUnit.Watt)
                            {
                                SetLevelUnit(UniqueData.LevelUnits[i]);
                            }
                        }
                    }
                    #endregion LevelUnit

                    #region SweepTime
                    //Если установленно в настройках адаптера OnlyAutoSweepTime
                    if (_adapterConfig.OnlyAutoSweepTime)
                    {
                        SetAutoSweepTime(_adapterConfig.OnlyAutoSweepTime);
                    }
                    //Если SweepTime можно установить
                    else
                    {
                        (decimal swt, bool autoswt) = LPC.SweepTime(UniqueData, (decimal)command.Parameter.SweepTime_s);
                        if (autoswt)
                        {
                            SetAutoSweepTime(_adapterConfig.OnlyAutoSweepTime);
                        }
                        else
                        {
                            SetSweepTime(swt);
                        }
                    }
                    #endregion SweepTime

                    #region Костыль от некоректных данных спектра от прошлого измерения
                    if (UniqueData.InstrManufacture == 1)
                    {
                        if (UniqueData.InstrModel.Contains("FPL"))
                        {
                            //Thread.Sleep((int)(SweepTime * 4000 + 20));
                        }
                        else
                        {
                            Thread.Sleep((int)(SweepTime * 4000 + 10));
                        }
                    }
                    else if (UniqueData.InstrManufacture == 2)
                    {
                        Thread.Sleep((int)(SweepTime * 4000 + 30));
                    }
                    else if (UniqueData.InstrManufacture == 3)
                    {
                        Thread.Sleep((int)(SweepTime * 4000 + 30));
                    }
                    #endregion


                    //Устанавливаем сколько трейсов хотим
                    if (command.Parameter.TraceCount > 0)
                    {
                        TraceCountToMeas = (ulong)command.Parameter.TraceCount;
                        TraceCount = 0;
                        TracePoints = command.Parameter.TracePoint;
                    }
                    else
                    {
                        throw new Exception("TraceCount must be set greater than zero.");
                    }

                    //если надо изменяем размер буфера
                    int length = SweepPoints * 4 + SweepPoints.ToString().Length + 100;
                    if (session.DefaultBufferSize != length)
                    {
                        session.DefaultBufferSize = length;
                    }

                    //Меряем
                    //Если TraceType ClearWrite то пушаем каждый результат                    
                    if (TraceTypeResult == EN.TraceType.ClearWrite)
                    {
                        bool newres = false;

                        for (ulong i = 0; i < TraceCountToMeas; i++)
                        {
                            newres = GetTrace();
                            if (newres)
                            {
                                // пушаем результат
                                var result = new COMR.MesureTraceResult(TraceCount, CommandResultStatus.Next);
                                TraceCount++;
                                if (TraceCountToMeas == TraceCount)
                                {
                                    result = new COMR.MesureTraceResult(TraceCount, CommandResultStatus.Final);
                                }
                                result.Att_dB = (int)AttLevelSpec;
                                result.RefLevel_dBm = (int)RefLevelSpec;
                                result.PreAmp_dB = PreAmpSpec ? 1 : 0;
                                result.RBW_Hz = (double)RBW;
                                result.VBW_Hz = (double)VBW;
                                result.Freq_Hz = new double[FreqArr.Length];
                                result.Level = new float[FreqArr.Length];
                                for (int j = 0; j < FreqArr.Length; j++)
                                {
                                    result.Freq_Hz[j] = FreqArr[j];
                                    result.Level[j] = LevelArr[j];
                                }
                                //result.TimeStamp = _timeService.TimeStamp.Ticks - new DateTime(1970, 1, 1, 0, 0, 0, System.DateTimeKind.Utc).Ticks;//неюзабельно
                                result.TimeStamp = DateTime.UtcNow.Ticks - new DateTime(1970, 1, 1, 0, 0, 0, System.DateTimeKind.Utc).Ticks;
                                if (PowerRegister != EN.PowerRegister.Normal)
                                {
                                    result.DeviceStatus = COMR.Enums.DeviceStatus.RFOverload;
                                }
                                context.PushResult(result);
                            }
                            else
                            {
                                i--;
                            }
                            // иногда нужно проверять токен окончания работы комманды
                            if (context.Token.IsCancellationRequested)
                            {
                                // все нужно остановиться

                                // если есть порция данных возвращаем ее в обработчки только говрим что поток результатов не законченный и больше уже не будет поступать
                                var result2 = new COMR.MesureTraceResult(TraceCount, CommandResultStatus.Ragged);

                                context.PushResult(result2);


                                // подтверждаем факт обработки отмены
                                context.Cancel();
                                // освобождаем поток 
                                return;
                            }
                        }

                    }
                    //Если TraceType Average/MinHold/MaxHold то делаем измерений сколько сказали и пушаем только готовый результат
                    else
                    {
                        TraceReset = true;///сбросим предыдущие результаты
                        if (TraceTypeResult == EN.TraceType.Average)//назначим сколько усреднять
                        {
                            TraceAveraged.AveragingCount = (int)TraceCountToMeas;
                        }
                        bool _RFOverload = false;
                        bool newres = false;
                        for (ulong i = 0; i < TraceCountToMeas; i++)
                        {
                            newres = GetTrace();
                            if (newres)
                            {
                                TraceCount++;
                                if (PowerRegister != EN.PowerRegister.Normal)
                                {
                                    _RFOverload = true;
                                }
                            }
                            else
                            {
                                i--;
                            }
                            // иногда нужно проверять токен окончания работы комманды
                            if (context.Token.IsCancellationRequested)
                            {
                                // все нужно остановиться

                                // если есть порция данных возвращаем ее в обработчки только говрим что поток результатов не законченный и больше уже не будет поступать

                                var result2 = new COMR.MesureTraceResult(TraceCount, CommandResultStatus.Ragged);
                                //Скорее нет результатов
                                context.PushResult(result2);

                                // подтверждаем факт обработки отмены
                                context.Cancel();
                                // освобождаем поток 
                                return;
                            }
                        }
                        if (TraceCountToMeas == TraceCount)
                        {
                            var result = new COMR.MesureTraceResult(0, CommandResultStatus.Final)
                            {
                                Freq_Hz = new double[FreqArr.Length],
                                Level = new float[FreqArr.Length]
                            };
                            for (int j = 0; j < FreqArr.Length; j++)
                            {
                                result.Freq_Hz[j] = FreqArr[j];
                                result.Level[j] = LevelArr[j];
                            }
                            result.Att_dB = (int)AttLevelSpec;
                            result.RefLevel_dBm = (int)RefLevelSpec;
                            result.PreAmp_dB = PreAmpSpec ? 1 : 0;
                            result.RBW_Hz = (double)RBW;
                            result.VBW_Hz = (double)VBW;
                            //result.TimeStamp = _timeService.TimeStamp.Ticks - new DateTime(1970, 1, 1, 0, 0, 0, System.DateTimeKind.Utc).Ticks;//неюзабельно
                            result.TimeStamp = DateTime.UtcNow.Ticks - new DateTime(1970, 1, 1, 0, 0, 0, System.DateTimeKind.Utc).Ticks;
                            if (_RFOverload)
                            {
                                result.DeviceStatus = COMR.Enums.DeviceStatus.RFOverload;
                            }
                            context.PushResult(result);
                        }
                    }

                    context.Unlock();

                    // что то делаем еще 


                    // подтверждаем окончание выполнения комманды 
                    // важно: всн ранее устапнволеные в контексте обработки текущей команыд блокировки снимаются автоматически
                    context.Finish();
                    // дальше кода быть не должно, освобождаем поток
                }
                else
                {
                    throw new Exception("The device with serial number " + UniqueData.SerialNumber + " does not work");
                }
            }
            catch (VisaException v_exp)
            {
                // желательно записать влог
                _logger.Exception(Contexts.ThisComponent, v_exp);
                // этот вызов обязательный в случаи обрыва
                context.Abort(v_exp);
                // дальше кода быть не должно, освобождаем поток
            }
            catch (Exception e)
            {
                // желательно записать влог
                _logger.Exception(Contexts.ThisComponent, e);
                // этот вызов обязательный в случаи обрыва
                context.Abort(e);
                // дальше кода быть не должно, освобождаем поток
            }

        }

        //неживое но работет
        public void MesureIQStreamCommandHandler(COM.MesureIQStreamCommand command, IExecutionContext context)
        {
            try
            {
                /// если нужно проводим блокировку комманд который мы не сможем обслужить пока что то меряем в течении выполнени яэтой комманды
                /// и подсказывая этим инфраструктуре что этот устрйостов некоторое время не сможет обрабатываить такие комманды
                context.Lock(CommandType.MesureIQStream);
                // если нужно заблокировать выполняему комманду то достатчоно вызвать метод без параметров и блокируется комманда которая выполняется
                context.Lock();
                //Переключимся на IQ
                if (Mode != EN.Mode.IQAnalyzer)
                {
                    SetWindowType(EN.Mode.IQAnalyzer);
                }

                //FreqCentrIQ
                if (FreqCentrIQ != (command.Parameter.FreqStart_Hz + command.Parameter.FreqStop_Hz) / 2)
                {
                    FreqCentrIQ = LPC.FreqCentrIQ(UniqueData, command.Parameter.FreqStart_Hz, command.Parameter.FreqStop_Hz);
                    SetFreqCentrIQ(FreqCentrIQ);
                }

                //ATT
                (decimal att, bool auto) = LPC.Attenuator(UniqueData, command.Parameter.Att_dB);
                if (auto)
                {
                    SetAutoAtt(auto);
                }
                else
                {
                    if (AttLevelIQ != att)
                    {
                        SetAttLevel(att);
                    }
                }

                //PreAmp
                bool preamp = LPC.PreAmp(UniqueData, command.Parameter.Att_dB);
                if (PreAmpIQ != preamp)
                {
                    SetPreAmp(preamp);
                }

                //Reflevel
                if (RefLevelIQ != command.Parameter.RefLevel_dBm)
                {
                    if (command.Parameter.RefLevel_dBm == 1000000000)
                    {
                        RefLevelIQ = -20;
                    }
                    else
                    {
                        RefLevelIQ = command.Parameter.RefLevel_dBm;
                    }
                    SetRefLevel(RefLevelIQ);
                }

                //FreqSpanIQ установит полосу просмотра IQ оно же зафиксирует скорость семплирования
                if (IQBW != command.Parameter.FreqStop_Hz - command.Parameter.FreqStart_Hz)
                {
                    IQBW = LPC.FreqSpanIQ(UniqueData, command.Parameter.FreqStart_Hz, command.Parameter.FreqStop_Hz);
                    SetIQBW(IQBW);
                }
                //OR закоментировать что-то одно
                //FreqSpanIQ установит скорость семплирования IQ оно же зафиксирует полосу просмотра 
                //decimal samplespeed = LPC.SampleSpeed(UniqueData, ((decimal)command.Parameter.BitRate_MBs) * 1000000);
                //if (SampleSpeed != samplespeed)
                //{
                //    SetSampleSpeed(samplespeed);
                //}





                long time = _timeService.GetGnssTime().Ticks;
                long ToNextSecond = (time / 10000000) * 10000000 - time + 10000000;

                //IQMeasTime
                decimal delay = Math.Abs(((decimal)ToNextSecond) / 10000000) - 0.03m;// - 0.5m; //время насколько раньше тригерра будут собранны данные всегда отрицательное
                //ищем ближайшее целое по отношени к длительности семпла
                int divisor = -1 + (int)Math.Floor((0 - delay) / SampleTimeLength);

                IQMeasTime = (decimal)command.Parameter.IQBlockDuration_s;
                IQMeasTimeAll = IQMeasTime;
                SampleLength = (int)(SampleSpeed * IQMeasTimeAll);
                //SetSampleLength(SampleLength);
                SetTriggerOffsetAndSampleLength(divisor * SampleTimeLength, (int)(SampleSpeed * IQMeasTimeAll));
                //SetIQMeasTime(IQMeasTimeAll);

                COMR.MesureIQStreamResult result = new COMR.MesureIQStreamResult(0, CommandResultStatus.Final)
                {
                    DeviceStatus = COMR.Enums.DeviceStatus.Normal
                };
                if (GetIQStream(ref result, IQMeasTimeAll, SampleLength, command))
                {
                    context.PushResult(result);
                    //Debug.WriteLine("\r\n" + new TimeSpan(_timeService.GetGnssTime().Ticks).ToString() + " Result");
                }
                /////////////
                //////////long timestop = _timeService.GetGnssTime().Ticks;
                Debug.WriteLine("\r\n" + TriggerOffset.ToString() + " delay2");
                //////////Debug.WriteLine(new TimeSpan(time).ToString(@"hh\:mm\:ss\.fffffff") + " delay2");
                //////////long dddd = (long)(Math.Abs(delay2) * 10000000);
                //////////Debug.WriteLine(new TimeSpan(time + dddd).ToString(@"hh\:mm\:ss\.fffffff") + " delay2");
                //////////Debug.WriteLine(new TimeSpan((time / 10000000) * 10000000 + 10000000).ToString(@"hh\:mm\:ss\.fffffff") + " delay2");


                //////////Debug.WriteLine("\r\n" + new TimeSpan(timestop).ToString(@"hh\:mm\:ss\.fffffff") + " Result");
                context.Unlock();
                context.Finish();
            }
            catch (Exception e)
            {
                // желательно записать влог
                _logger.Exception(Contexts.ThisComponent, e);
                // этот вызов обязательный в случаи обрыва
                context.Abort(e);
                // дальше кода быть не должно, освобождаем поток
            }
        }
        #endregion


        #region Param
        private string decimalSeparator = System.Globalization.NumberFormatInfo.CurrentInfo.CurrencyDecimalSeparator;
        private TcpipSession session;
        private LocalSpectrumAnalyzerInfo UniqueData { get; set; } = new LocalSpectrumAnalyzerInfo { };
        private List<LocalSpectrumAnalyzerInfo> AllUniqueData = new List<LocalSpectrumAnalyzerInfo>
        {
            #region FSW
            new LocalSpectrumAnalyzerInfo
            {
                InstrManufacture = 1, NumberOfTrace = 3, ChangeableSweepType = true, SweepPointFix = false, InstrModel = "FSW",
                HiSpeed = true,
                InstrOption = new List<DeviceOption>()
                {
                     new DeviceOption(){Type = "B4", Designation = "OCXO, Precision Reference Frequency", GlobalType = "OCXO, Precision Reference Frequency"},
                     new DeviceOption(){Type = "B8", Designation = "Resolution Bandwidths > 10 MHz (for R&S®FSW8/13/26)", GlobalType = "Resolution Bandwidths > 10 MHz (for R&S®FSW8/13/26)"},
                     new DeviceOption(){Type = "B8", Designation = "Resolution Bandwidths > 10 MHz (for R&S®FSW43/50/67/85)", GlobalType = "Resolution Bandwidths > 10 MHz (for R&S®FSW43/50/67/85)"},
                     new DeviceOption(){Type = "B10", Designation = "External Generator Control", GlobalType = "External Generator Control"},
                     new DeviceOption(){Type = "B13", Designation = "Highpass Filters for Harmonic Measurements", GlobalType = "Highpass Filters for Harmonic Measurements"},
                     new DeviceOption(){Type = "B17", Designation = "Digital Baseband Interface", GlobalType = "Digital Baseband Interface"},
                     new DeviceOption(){Type = "B18", Designation = "Solid State Drive (SSD, removable)", GlobalType = "SSD"},
                     new DeviceOption(){Type = "B21", Designation = "LO/IF Ports for External Mixers (for R&S®FSW26)", GlobalType = "External Mixers"},
                     new DeviceOption(){Type = "B21", Designation = "LO/IF Ports for External Mixers (for R&S®FSW43/50/67)", GlobalType = "External Mixers"},
                     new DeviceOption(){Type = "B21", Designation = "LO/IF Ports for External Mixers (for R&S®FSW85)", GlobalType = "External Mixers"},
                     new DeviceOption(){Type = "B24", Designation = "Preamplifier", GlobalType = "Preamplifier"},
                     new DeviceOption(){Type = "B25", Designation = "Electronic Attenuator (1 dB steps)", GlobalType = "Attenuator 1 dB"},
                     new DeviceOption(){Type = "B33", Designation = "USB Mass Memory Write Protection", GlobalType = "USB Mass Memory Write Protection"},
                     new DeviceOption(){Type = "B28", Designation = "28 MHz Analysis Bandwidth", GlobalType = "28 MHz Analysis Bandwidth"},
                     new DeviceOption(){Type = "B40", Designation = "40 MHz Analysis Bandwidth", GlobalType = "40 MHz Analysis Bandwidth"},
                     new DeviceOption(){Type = "B71", Designation = "Analog Baseband Inputs", GlobalType = "Analog Baseband Inputs"},
                     new DeviceOption(){Type = "B71E", Designation = "80 MHz Bandwidth for Analog Baseband Inputs", GlobalType = "80 MHz Bandwidth for Analog Baseband Inputs"},
                     new DeviceOption(){Type = "B80", Designation = "80 MHz Analysis Bandwidth", GlobalType = "80 MHz Analysis Bandwidth"},
                     new DeviceOption(){Type = "B160", Designation = "160 MHz Analysis Bandwidth", GlobalType = "160 MHz Analysis Bandwidth"},
                     new DeviceOption(){Type = "B160R", Designation = "Real-Time Spectrum Analyzer, 160 MHz", GlobalType = "Real-Time Spectrum Analyzer, 160 MHz"},
                     new DeviceOption(){Type = "B320", Designation = "320 MHz Analysis Bandwidth", GlobalType = "320 MHz Analysis Bandwidth"},
                     new DeviceOption(){Type = "B512", Designation = "512 MHz Analysis Bandwidth", GlobalType = "512 MHz Analysis Bandwidth"},
                     new DeviceOption(){Type = "B512R", Designation = "Real-Time Spectrum Analyzer, 512 MHz", GlobalType = "Real-Time Spectrum Analyzer, 512 MHz"},
                     new DeviceOption(){Type = "B1200", Designation = "1.2 GHz Analysis Bandwidth", GlobalType = "1.2 GHz Analysis Bandwidth"},
                     new DeviceOption(){Type = "B2000", Designation = "2 GHz Analysis Bandwidth", GlobalType = "2 GHz Analysis Bandwidth"},
                     new DeviceOption(){Type = "K0", Designation = "DEMO OPTION (All inclusive)", GlobalType = "DEMO OPTION (All inclusive)"},
                     new DeviceOption(){Type = "K6", Designation = "Pulse Measurements", GlobalType = "Pulse Measurements"},
                     new DeviceOption(){Type = "K6S", Designation = "Time Sidelobe Measurement", GlobalType = "Time Sidelobe Measurement"},
                     new DeviceOption(){Type = "K7", Designation = "Analog Modulation Analysis (AM/FM/φM)", GlobalType = "Analog Modulation Analysis"},
                     new DeviceOption(){Type = "K10", Designation = "GSM, EDGE, EDGE Evolution and VAMOS Measurements", GlobalType = "GSM, EDGE, EDGE Evolution and VAMOS Measurements"},
                     new DeviceOption(){Type = "K15", Designation = "VOR/ILS Measurements", GlobalType = "VOR/ILS Measurements"},
                     new DeviceOption(){Type = "K17", Designation = "Multicarrier Group Delay Measurements", GlobalType = "Multicarrier Group Delay Measurements"},
                     new DeviceOption(){Type = "K18", Designation = "Amplifier Measurements", GlobalType = "Amplifier Measurements"},
                     new DeviceOption(){Type = "K18D", Designation = "Direct DPD Measurements", GlobalType = "Direct DPD Measurements"},
                     new DeviceOption(){Type = "K30", Designation = "Noise Figure Measurements", GlobalType = "Noise Figure Measurements"},
                     new DeviceOption(){Type = "K33", Designation = "Security Write Protection of solid state drive", GlobalType = "Security Write Protection of solid state drive"},
                     new DeviceOption(){Type = "K40", Designation = "Phase Noise Measurements", GlobalType = "Phase Noise Measurements"},
                     new DeviceOption(){Type = "K50", Designation = "Spurious Measurements", GlobalType = "Spurious Measurements"},
                     new DeviceOption(){Type = "K54", Designation = "EMI Measurements", GlobalType = "EMI Measurements"},
                     new DeviceOption(){Type = "K60", Designation = "Transient Measurement Application", GlobalType = "Transient Measurement Application"},
                     new DeviceOption(){Type = "K60C", Designation = "Transient Chirp Measurement", GlobalType = "Transient Chirp Measurement"},
                     new DeviceOption(){Type = "K60H", Designation = "Transient Hop Measurement", GlobalType = "Transient Hop Measurement"},
                     new DeviceOption(){Type = "K70", Designation = "Vector Signal Analysis", GlobalType = "Vector Signal Analysis"},
                     new DeviceOption(){Type = "K72", Designation = "3GPP FDD (WCDMA) BS Measurements (incl. HSDPA and HSDPA+)", GlobalType = "3GPP FDD (WCDMA) BS Measurements (incl. HSDPA and HSDPA+)"},
                     new DeviceOption(){Type = "K73", Designation = "3GPP FDD (WCDMA) UE Measurements (incl. HSUPA and HSUPA+)", GlobalType = "3GPP FDD (WCDMA) UE Measurements (incl. HSUPA and HSUPA+)"},
                     new DeviceOption(){Type = "K76", Designation = "3GPP TDD (TD-SCDMA) BS Measurements", GlobalType = "3GPP TDD (TD-SCDMA) BS Measurements"},
                     new DeviceOption(){Type = "K77", Designation = "3GPP TDD (TD-SCDMA) UE Measurements", GlobalType = "3GPP TDD (TD-SCDMA) UE Measurements"},
                     new DeviceOption(){Type = "K82", Designation = "CDMA2000® BS (DL) Analysis", GlobalType = "CDMA2000® BS (DL) Analysis"},
                     new DeviceOption(){Type = "K83", Designation = "CDMA2000® MS (UL) Measurements", GlobalType = "CDMA2000® MS (UL) Measurements"},
                     new DeviceOption(){Type = "K84", Designation = "1xEV-DO BS (DL) Analysis", GlobalType = "1xEV-DO BS (DL) Analysis"},
                     new DeviceOption(){Type = "K85", Designation = "1xEV-DO MS (UL) Measurements", GlobalType = "1xEV-DO MS (UL) Measurements"},
                     new DeviceOption(){Type = "K91", Designation = "WLAN IEEE802.11a/b/g Measurements", GlobalType = "WLAN IEEE802.11a/b/g Measurements"},
                     new DeviceOption(){Type = "K91N", Designation = "WLAN IEEE802.11n Measurements", GlobalType = "WLAN IEEE802.11n Measurements"},
                     new DeviceOption(){Type = "K91AC", Designation = "WLAN IEEE802.11ac Measurements", GlobalType = "WLAN IEEE802.11ac Measurements"},
                     new DeviceOption(){Type = "K91AX", Designation = "WLAN IEEE802.11ax Measurements", GlobalType = "WLAN IEEE802.11ax Measurements"},
                     new DeviceOption(){Type = "K95", Designation = "WLAN IEEE802.11ad Measurements", GlobalType = "WLAN IEEE802.11ad Measurements"},
                     new DeviceOption(){Type = "K100", Designation = "EUTRA/LTE FDD Downlink Analysis", GlobalType = "EUTRA/LTE FDD Downlink Analysis"},
                     new DeviceOption(){Type = "K101", Designation = "EUTRA/LTE FDD Uplink Analysis", GlobalType = "EUTRA/LTE FDD Uplink Analysis"},
                     new DeviceOption(){Type = "K102", Designation = "EUTRA/LTE Downlink MIMO Analysis", GlobalType = "EUTRA/LTE Downlink MIMO Analysis"},
                     new DeviceOption(){Type = "K103", Designation = "EUTRA/LTE UL Advanced UL Measurements", GlobalType = "EUTRA/LTE UL Advanced UL Measurements"},
                     new DeviceOption(){Type = "K104", Designation = "EUTRA/LTE TDD Downlink Analysis", GlobalType = "EUTRA/LTE TDD Downlink Analysis"},
                     new DeviceOption(){Type = "K105", Designation = "EUTRA/LTE TDD Uplink Analysis", GlobalType = "EUTRA/LTE TDD Uplink Analysis"},
                     new DeviceOption(){Type = "K96", Designation = "OFDM Vector Signal Analysis Software", GlobalType = "OFDM Vector Signal Analysis Software"},
                     new DeviceOption(){Type = "K192", Designation = "DOCSIS 3.1 OFDM Downstream", GlobalType = "DOCSIS 3.1 OFDM Downstream"},
                     new DeviceOption(){Type = "K193", Designation = "DOCSIS 3.1 OFDM Upstream", GlobalType = "DOCSIS 3.1 OFDM Upstream"},
                     new DeviceOption(){Type = "K160RE", Designation = "160 MHz Real-Time Measurement Application, POI > 15 µs", GlobalType = "160 MHz Real-Time Measurement Application, POI > 15 µs"},
                },
                DefaultInstrOption = new List<DeviceOption>() { },
                LoadedInstrOption = new List<DeviceOption>() { },
                TraceType = new List<ParamWithId>
                {
                    new ParamWithId {Id = (int)EN.TraceType.ClearWrite, Parameter = "WRIT" },
                    new ParamWithId {Id = (int)EN.TraceType.Average, Parameter = "AVER" },
                    new ParamWithId {Id = (int)EN.TraceType.MaxHold, Parameter = "MAXH" },
                    new ParamWithId {Id = (int)EN.TraceType.MinHold, Parameter = "MINH" },
                    new ParamWithId {Id = (int)EN.TraceType.View, Parameter = "VIEW" },
                    new ParamWithId {Id = (int)EN.TraceType.Blank, Parameter = "BLAN" }
                },
                TraceDetector = new List<ParamWithId>
                {
                    new ParamWithId {Id = (int)EN.TraceDetector.AutoSelect, Parameter = "Auto Select" },
                    new ParamWithId {Id = (int)EN.TraceDetector.AutoPeak, Parameter = "APE" },
                    new ParamWithId {Id = (int)EN.TraceDetector.Average, Parameter = "AVER" },
                    new ParamWithId {Id = (int)EN.TraceDetector.MaxPeak, Parameter = "POS" },
                    new ParamWithId {Id = (int)EN.TraceDetector.MinPeak, Parameter = "NEG" },
                    new ParamWithId {Id = (int)EN.TraceDetector.Sample, Parameter = "SAMP" },
                    new ParamWithId {Id = (int)EN.TraceDetector.RMS, Parameter = "RMS" }
                },
                SweepType =  new List<ParamWithId>
                {
                    new ParamWithId {Id = (int)EN.SweepType.Auto, Parameter = "AUTO" },
                    new ParamWithId {Id = (int)EN.SweepType.Sweep, Parameter = "SWE" },
                    new ParamWithId {Id = (int)EN.SweepType.FFT, Parameter = "FFT" }
                },
                LevelUnits = new List<ParamWithId>
                {
                    new ParamWithId {Id = (int)MEN.LevelUnit.dBm, Parameter = "DBM" },
                    new ParamWithId {Id = (int)MEN.LevelUnit.dBmV, Parameter = "DBMV" },
                    new ParamWithId {Id = (int)MEN.LevelUnit.dBµV, Parameter = "DBUV" },
                },
                SweepPointArr = new int[]{ 101, 125, 155, 173, 201, 251, 301, 313, 345, 401, 501, 601, 625, 691, 701, 801, 901, 1001, 1251, 1383, 1999, 2001, 2501, 2765, 3001, 4001, 5001, 6001, 7001, 8001, 9001, 10001, 11001, 12001, 13001, 14001, 15001, 16001, 17001, 18001, 19001, 20001, 21001, 22001, 23001, 24001, 25001, 26001, 27001, 28001, 29001, 30001, 31001, 32001 },
                DefaultSweepPoint = 1001,
                RBWArr = new decimal[]{ 1, 2, 3, 5, 10, 20, 30, 50, 100, 200, 300, 500, 1000, 2000, 3000, 5000, 6250, 10000, 20000, 30000, 50000, 100000, 200000, 300000, 500000, 1000000, 2000000, 3000000, 5000000, 10000000 },
                VBWArr = new decimal[]{ 1, 2, 3, 5, 10, 20, 30, 50, 100, 200, 300, 500, 1000, 2000, 3000, 5000, 10000, 20000, 30000, 50000, 100000, 200000, 300000, 500000, 1000000, 2000000, 3000000, 5000000, 10000000, 20000000, 28000000, 40000000 },
                CouplingRatio = true,
                AttMax = 70,
                AttStep = 5,
                PreAmp = false,
                Battery = false,


                IQAvailable = true,
            },
            #endregion FSW
            #region FSV
            new LocalSpectrumAnalyzerInfo
            {
                InstrManufacture = 1,  NumberOfTrace = 3, ChangeableSweepType = true, SweepPointFix = false, InstrModel = "FSV",
                HiSpeed = true,
                InstrOption = new List<DeviceOption>()
                {
                     new DeviceOption(){Type = "B1", Designation = "Ruggedized Housing", GlobalType = "Ruggedized Housing"},
                     new DeviceOption(){Type = "B3", Designation = "AM/FM Audio Demodulator", GlobalType = "Audio Demodulator"},
                     new DeviceOption(){Type = "B4", Designation = "OCXO, Precision Reference Frequency", GlobalType = "OCXO, Precision Reference Frequency"},
                     new DeviceOption(){Type = "B4", Designation = "OCXO, Enhanced Frequency Stability", GlobalType = "OCXO, Enhanced Frequency Stability"},
                     new DeviceOption(){Type = "B5", Designation = "Additional Interfaces (IF/video/AM/FM output, AUX port, trigger output, two additional USB ports)", GlobalType = "Additional Interfaces"},
                     new DeviceOption(){Type = "B9", Designation = "Tracking Generator, 100 kHz to 4 GHz/7 GHz", GlobalType = "Tracking Generator"},
                     new DeviceOption(){Type = "B10", Designation = "External Generator Control", GlobalType = "External Generator Control"},
                     new DeviceOption(){Type = "B17", Designation = "Digital Baseband Interface", GlobalType = "Digital Baseband Interface"},
                     new DeviceOption(){Type = "B18", Designation = "Solid State Drive (SSD, removable)", GlobalType = "SSD"},
                     new DeviceOption(){Type = "B19", Designation = "Spare Hard Disk Drive (HDD, removable)", GlobalType = "HDD"},
                     new DeviceOption(){Type = "B21", Designation = "LO/IF Ports for External Mixers", GlobalType = "External Mixers"},
                     new DeviceOption(){Type = "B22", Designation = "RF Preamplifier (100 kHz to 7 GHz)", GlobalType = "Preamplifier"},
                     new DeviceOption(){Type = "B24", Designation = "Preamplifier, 9 kHz to 13.6 GHz", GlobalType = "Preamplifier"},
                     new DeviceOption(){Type = "B24", Designation = "Preamplifier, 9 kHz to 30 GHz", GlobalType = "Preamplifier"},
                     new DeviceOption(){Type = "B24", Designation = "Preamplifier, 9 kHz to 40 GHz", GlobalType = "Preamplifier"},
                     new DeviceOption(){Type = "B25", Designation = "Electronic Attenuator (1 dB steps)", GlobalType = "Attenuator 1 dB"},
                     new DeviceOption(){Type = "B30", Designation = "DC Power Supply, 12 V to 15 V", GlobalType = "DC Power Supply"},
                     new DeviceOption(){Type = "B32", Designation = "Lithium-Ion Battery Pack", GlobalType = "Lithium-Ion Battery Pack"},
                     new DeviceOption(){Type = "B34", Designation = "Battery Charger, for R&S®FSV-B32 Li-ion battery pack", GlobalType = "Battery Charger"},
                     new DeviceOption(){Type = "B70", Designation = "40 MHz Analysis Bandwidth", GlobalType = "40 MHz Analysis Bandwidth"},
                     new DeviceOption(){Type = "B160", Designation = "160 MHz Analysis Bandwidth (for R&S®FSV4 and R&S®FSV7)", GlobalType = "160 MHz Analysis Bandwidth"},
                     new DeviceOption(){Type = "B160", Designation = "160 MHz Analysis Bandwidth (for R&S®FSV13)", GlobalType = "160 MHz Analysis Bandwidth"},
                     new DeviceOption(){Type = "B160", Designation = "160 MHz Analysis Bandwidth (for R&S®FSV30 und R&S®FSV40)", GlobalType = "160 MHz Analysis Bandwidth"},
                     new DeviceOption(){Type = "K0", Designation = "DEMO OPTION (All inclusive)", GlobalType = "DEMO OPTION (All inclusive)"},
                     new DeviceOption(){Type = "K7", Designation = "Analog Modulation Analysis (AM/FM/φM)", GlobalType = "Analog Modulation Analysis"},
                     new DeviceOption(){Type = "K7S", Designation = "FM Stereo Measurements (for R&S®FSV-K7)", GlobalType = "FM Stereo Measurements"},
                     new DeviceOption(){Type = "K8", Designation = "Bluetooth®/EDR Measurement Application", GlobalType = "Bluetooth®/EDR Measurement Application"},
                     new DeviceOption(){Type = "K9", Designation = "Power Sensor Support (power measurement with the R&S®NRP-Zxx power sensors)", GlobalType = "Power Sensor Support"},
                     new DeviceOption(){Type = "K10", Designation = "GSM/EDGE/EDGE Evolution Analysis", GlobalType = "GSM/EDGE/EDGE Evolution Analysis"},
                     new DeviceOption(){Type = "K14", Designation = "Spectrogram Measurements", GlobalType = "Spectrogram Measurements"},
                     new DeviceOption(){Type = "K30", Designation = "Noise Figure and Gain Measurements", GlobalType = "Noise Figure and Gain Measurements"},
                     new DeviceOption(){Type = "K40", Designation = "Phase Noise Measurements", GlobalType = "Phase Noise Measurements"},
                     new DeviceOption(){Type = "K70", Designation = "Vector Signal Analysis", GlobalType = "Vector Signal Analysis"},
                     new DeviceOption(){Type = "K72", Designation = "3GPP BS (DL) Analysis, incl. HSDPA and HSDPA+", GlobalType = "3GPP BS (DL) Analysis, incl. HSDPA and HSDPA+"},
                     new DeviceOption(){Type = "K73", Designation = "3GPP UE (UL) Analysis, incl. HSUPA", GlobalType = "3GPP UE (UL) Analysis, incl. HSUPA"},
                     new DeviceOption(){Type = "K76", Designation = "TD-SCDMA BS Measurements", GlobalType = "TD-SCDMA BS Measurements"},
                     new DeviceOption(){Type = "K77", Designation = "TD-SCDMA UE Measurements", GlobalType = "TD-SCDMA UE Measurements"},
                     new DeviceOption(){Type = "K82", Designation = "CDMA2000® BS (DL) Analysis", GlobalType = "CDMA2000® BS (DL) Analysis"},
                     new DeviceOption(){Type = "K83", Designation = "CDMA2000® MS (UL) Measurements", GlobalType = "CDMA2000® MS (UL) Measurements"},
                     new DeviceOption(){Type = "K84", Designation = "1xEV-DO BS (DL) Analysis", GlobalType = "1xEV-DO BS (DL) Analysis"},
                     new DeviceOption(){Type = "K85", Designation = "1xEV-DO MS (UL) Measurements", GlobalType = "1xEV-DO MS (UL) Measurements"},
                     new DeviceOption(){Type = "K91", Designation = "WLAN IEEE802.11a/b/g/j Analysis", GlobalType = "WLAN IEEE802.11a/b/g/j Analysis"},
                     new DeviceOption(){Type = "K91N", Designation = "WLAN IEEE802.11n Analysis", GlobalType = "WLAN IEEE802.11n Analysis"},
                     new DeviceOption(){Type = "K91AC", Designation = "WLAN IEEE802.11ac Analysis", GlobalType = "WLAN IEEE802.11ac Analysis"},
                     new DeviceOption(){Type = "K91P", Designation = "WLAN IEEE802.11ac Analysis", GlobalType = "WLAN IEEE802.11ac Analysis"},
                     new DeviceOption(){Type = "K93", Designation = "WiMAX™ IEEE802.16e OFDM/OFDMA Analysis", GlobalType = "WiMAX™ IEEE802.16e OFDM/OFDMA Analysis"},
                     new DeviceOption(){Type = "K100", Designation = "EUTRA/LTE FDD Downlink Analysis", GlobalType = "EUTRA/LTE FDD Downlink Analysis"},
                     new DeviceOption(){Type = "K101", Designation = "EUTRA/LTE FDD Uplink Analysis", GlobalType = "EUTRA/LTE FDD Uplink Analysis"},
                     new DeviceOption(){Type = "K102", Designation = "EUTRA/LTE Downlink MIMO Analysis", GlobalType = "EUTRA/LTE Downlink MIMO Analysis"},
                     new DeviceOption(){Type = "K104", Designation = "EUTRA/LTE TDD Downlink Analysis", GlobalType = "EUTRA/LTE TDD Downlink Analysis"},
                     new DeviceOption(){Type = "K105", Designation = "EUTRA/LTE TDD Uplink Analysis", GlobalType = "EUTRA/LTE TDD Uplink Analysis"},
                     new DeviceOption(){Type = "K96", Designation = "OFDM Vector Signal Analysis Software", GlobalType = "OFDM Vector Signal Analysis Software"},
                     new DeviceOption(){Type = "K130PC", Designation = "Distortion Analysis Software", GlobalType = "Distortion Analysis Software"},
                },
                DefaultInstrOption = new List<DeviceOption>() { },
                LoadedInstrOption = new List<DeviceOption>() { },
                TraceType = new List<ParamWithId>
                {
                    new ParamWithId {Id = (int)EN.TraceType.ClearWrite, Parameter = "WRIT" },
                    new ParamWithId {Id = (int)EN.TraceType.Average, Parameter = "AVER" },
                    new ParamWithId {Id = (int)EN.TraceType.MaxHold, Parameter = "MAXH" },
                    new ParamWithId {Id = (int)EN.TraceType.MinHold, Parameter = "MINH" },
                    new ParamWithId {Id = (int)EN.TraceType.View, Parameter = "VIEW" },
                    new ParamWithId {Id = (int)EN.TraceType.Blank, Parameter = "BLAN" }
                },
                TraceDetector = new List<ParamWithId>
                {
                    new ParamWithId {Id = (int)EN.TraceDetector.AutoSelect, Parameter = "Auto Select" },
                    new ParamWithId {Id = (int)EN.TraceDetector.AutoPeak, Parameter = "APE" },
                    new ParamWithId {Id = (int)EN.TraceDetector.Average, Parameter = "AVER" },
                    new ParamWithId {Id = (int)EN.TraceDetector.MaxPeak, Parameter = "POS" },
                    new ParamWithId {Id = (int)EN.TraceDetector.MinPeak, Parameter = "NEG" },
                    new ParamWithId {Id = (int)EN.TraceDetector.Sample, Parameter = "SAMP" },
                    new ParamWithId {Id = (int)EN.TraceDetector.RMS, Parameter = "RMS" }
                },
                SweepType =  new List<ParamWithId>
                {
                    new ParamWithId {Id = (int)EN.SweepType.Auto, Parameter = "AUTO" },
                    new ParamWithId {Id = (int)EN.SweepType.Sweep, Parameter = "SWE" },
                    new ParamWithId {Id = (int)EN.SweepType.FFT, Parameter = "FFT" }
                },
                LevelUnits = new List<ParamWithId>
                {
                    new ParamWithId {Id = (int)MEN.LevelUnit.dBm, Parameter = "DBM" },
                    new ParamWithId {Id = (int)MEN.LevelUnit.dBmV, Parameter = "DBMV" },
                    new ParamWithId {Id = (int)MEN.LevelUnit.dBµV, Parameter = "DBUV" },
                },
                SweepPointArr = new int[]{ 101, 125, 155, 173, 201, 251, 301, 313, 345, 401, 501, 601, 625, 691, 701, 801, 901, 1001, 1251, 1383, 1999, 2001, 2501, 2765, 3001, 4001, 5001, 6001, 7001, 8001, 9001, 10001, 11001, 12001, 13001, 14001, 15001, 16001, 17001, 18001, 19001, 20001, 21001, 22001, 23001, 24001, 25001, 26001, 27001, 28001, 29001, 30001, 31001, 32001 },
                DefaultSweepPoint = 691,
                RBWArr = new decimal[]{ 1, 2, 3, 5, 10, 20, 30, 50, 100, 200, 300, 500, 1000, 2000, 3000, 5000, 6250, 10000, 20000, 30000, 50000, 100000, 200000, 300000, 500000, 1000000, 2000000, 3000000, 5000000, 10000000 },
                VBWArr = new decimal[]{ 1, 2, 3, 5, 10, 20, 30, 50, 100, 200, 300, 500, 1000, 2000, 3000, 5000, 10000, 20000, 30000, 50000, 100000, 200000, 300000, 500000, 1000000, 2000000, 3000000, 5000000, 10000000, 20000000, 28000000, 40000000 },
                RangeArr = new decimal[]{1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 15, 20, 30, 40, 50, 60, 70, 80, 90, 100, 110, 120, 130, 140, 150, 160, 170, 180, 190, 200 },
                CouplingRatio = true,
                AttMax = 70,
                AttStep = 5,
                PreAmp = false,
                Battery = false,

                IQAvailable = true,
            },
            #endregion FSV
            #region FSVA
            new LocalSpectrumAnalyzerInfo
            {
                InstrManufacture = 1, NumberOfTrace = 3, ChangeableSweepType = true, SweepPointFix = false, InstrModel = "FSVA",
                HiSpeed = true,
                InstrOption = new List<DeviceOption>()
                {
                    new DeviceOption(){Type = "B1", Designation = "Ruggedized Housing", GlobalType = "Ruggedized Housing"},
                    new DeviceOption(){Type = "B3", Designation = "AM/FM Audio Demodulator", GlobalType = "Audio Demodulator"},
                    new DeviceOption(){Type = "B4", Designation = "OCXO, Precision Reference Frequency", GlobalType = "OCXO, Precision Reference Frequency"},
                    new DeviceOption(){Type = "B4", Designation = "OCXO, Enhanced Frequency Stability", GlobalType = "OCXO, Enhanced Frequency Stability"},
                    new DeviceOption(){Type = "B5", Designation = "Additional Interfaces (IF/video/AM/FM output, AUX port, trigger output, two additional USB ports)", GlobalType = "Additional Interfaces"},
                    new DeviceOption(){Type = "B9", Designation = "Tracking Generator, 100 kHz to 4 GHz/7 GHz", GlobalType = "Tracking Generator"},
                    new DeviceOption(){Type = "B10", Designation = "External Generator Control", GlobalType = "External Generator Control"},
                    new DeviceOption(){Type = "B11", Designation = "YIG Preselector Bypass for R&S®FSVA13", GlobalType = "YIG Preselector Bypass for R&S®FSVA13"},
                    new DeviceOption(){Type = "B11", Designation = "YIG Preselector Bypass for R&S®FSVA30", GlobalType = "YIG Preselector Bypass for R&S®FSVA30"},
                    new DeviceOption(){Type = "B11", Designation = "YIG Preselector Bypass for R&S®FSVA40", GlobalType = "YIG Preselector Bypass for R&S®FSVA40"},
                    new DeviceOption(){Type = "B14", Designation = "Ultra-High Precision Frequency Reference", GlobalType = "Ultra-High Precision Frequency Reference"},
                    new DeviceOption(){Type = "B17", Designation = "Digital Baseband Interface", GlobalType = "Digital Baseband Interface"},
                    new DeviceOption(){Type = "B18", Designation = "Solid State Drive (SSD, removable)", GlobalType = "SSD"},
                    new DeviceOption(){Type = "B19", Designation = "Spare Hard Disk Drive (HDD, removable)", GlobalType = "HDD"},
                    new DeviceOption(){Type = "B21", Designation = "LO/IF Ports for External Mixers", GlobalType = "External Mixers"},
                    new DeviceOption(){Type = "B22", Designation = "RF Preamplifier (100 kHz to 7 GHz)", GlobalType = "Preamplifier"},
                    new DeviceOption(){Type = "B24", Designation = "Preamplifier, 9 kHz to 13.6 GHz", GlobalType = "Preamplifier"},
                    new DeviceOption(){Type = "B24", Designation = "Preamplifier, 9 kHz to 30 GHz", GlobalType = "Preamplifier"},
                    new DeviceOption(){Type = "B24", Designation = "Preamplifier, 9 kHz to 40 GHz", GlobalType = "Preamplifier"},
                    new DeviceOption(){Type = "B25", Designation = "Electronic Attenuator (1 dB steps)", GlobalType = "Attenuator 1 dB"},
                    new DeviceOption(){Type = "B30", Designation = "DC Power Supply, 12 V to 15 V", GlobalType = "DC Power Supply"},
                    new DeviceOption(){Type = "B32", Designation = "Lithium-Ion Battery Pack", GlobalType = "Lithium-Ion Battery Pack"},
                    new DeviceOption(){Type = "B33", Designation = "USB Mass Memory Write Protection", GlobalType = "USB Mass Memory Write Protection"},
                    new DeviceOption(){Type = "B34", Designation = "Battery Charger, for R&S®FSV-B32 Li-ion battery pack", GlobalType = "Battery Charger"},
                    new DeviceOption(){Type = "B40", Designation = "40 MHz Analysis Bandwidth", GlobalType = "40 MHz Analysis Bandwidth"},
                    new DeviceOption(){Type = "B160", Designation = "160 MHz Analysis Bandwidth (for R&S®FSVA4 and R&S®FSVA7)", GlobalType = "160 MHz Analysis Bandwidth"},
                    new DeviceOption(){Type = "B160", Designation = "160 MHz Analysis Bandwidth (for R&S®FSVA13)", GlobalType = "160 MHz Analysis Bandwidth"},
                    new DeviceOption(){Type = "B160", Designation = "160 MHz Analysis Bandwidth (for R&S®FSVA30 und R&S®FSVA40)", GlobalType = "160 MHz Analysis Bandwidth"},
                    new DeviceOption(){Type = "K0", Designation = "DEMO OPTION (All inclusive)", GlobalType = "DEMO OPTION (All inclusive)"},
                    new DeviceOption(){Type = "K7", Designation = "Analog Modulation Analysis (AM/FM/φM)", GlobalType = "Analog Modulation Analysis"},
                    new DeviceOption(){Type = "K7S", Designation = "FM Stereo Measurements (for R&S®FSV-K7)", GlobalType = "FM Stereo Measurements"},
                    new DeviceOption(){Type = "K8", Designation = "Bluetooth®/EDR Measurement Application", GlobalType = "Bluetooth®/EDR Measurement Application"},
                    new DeviceOption(){Type = "K9", Designation = "Power Sensor Support (power measurement with the R&S®NRP-Zxx power sensors)", GlobalType = "Power Sensor Support"},
                    new DeviceOption(){Type = "K10", Designation = "GSM/EDGE/EDGE Evolution Analysis", GlobalType = "GSM/EDGE/EDGE Evolution Analysis"},
                    new DeviceOption(){Type = "K14", Designation = "Spectrogram Measurements", GlobalType = "Spectrogram Measurements"},
                    new DeviceOption(){Type = "K30", Designation = "Noise Figure and Gain Measurements", GlobalType = "Noise Figure and Gain Measurements"},
                    new DeviceOption(){Type = "K40", Designation = "Phase Noise Measurements", GlobalType = "Phase Noise Measurements"},
                    new DeviceOption(){Type = "K54", Designation = "EMI Measurement Application", GlobalType = "EMI Measurement Application"},
                    new DeviceOption(){Type = "K70", Designation = "Vector Signal Analysis", GlobalType = "Vector Signal Analysis"},
                    new DeviceOption(){Type = "K72", Designation = "3GPP BS (DL) Analysis, incl. HSDPA and HSDPA+", GlobalType = "3GPP BS (DL) Analysis, incl. HSDPA and HSDPA+"},
                    new DeviceOption(){Type = "K73", Designation = "3GPP UE (UL) Analysis, incl. HSUPA", GlobalType = "3GPP UE (UL) Analysis, incl. HSUPA"},
                    new DeviceOption(){Type = "K76", Designation = "TD-SCDMA BS Measurements", GlobalType = "TD-SCDMA BS Measurements"},
                    new DeviceOption(){Type = "K77", Designation = "TD-SCDMA UE Measurements", GlobalType = "TD-SCDMA UE Measurements"},
                    new DeviceOption(){Type = "K82", Designation = "CDMA2000® BS (DL) Analysis", GlobalType = "CDMA2000® BS (DL) Analysis"},
                    new DeviceOption(){Type = "K83", Designation = "CDMA2000® MS (UL) Measurements", GlobalType = "CDMA2000® MS (UL) Measurements"},
                    new DeviceOption(){Type = "K84", Designation = "1xEV-DO BS (DL) Analysis", GlobalType = "1xEV-DO BS (DL) Analysis"},
                    new DeviceOption(){Type = "K85", Designation = "1xEV-DO MS (UL) Measurements", GlobalType = "1xEV-DO MS (UL) Measurements"},
                    new DeviceOption(){Type = "K91", Designation = "WLAN IEEE802.11a/b/g/j Analysis", GlobalType = "WLAN IEEE802.11a/b/g/j Analysis"},
                    new DeviceOption(){Type = "K91N", Designation = "WLAN IEEE802.11n Analysis", GlobalType = "WLAN IEEE802.11n Analysis"},
                    new DeviceOption(){Type = "K91AC", Designation = "WLAN IEEE802.11ac Analysis", GlobalType = "WLAN IEEE802.11ac Analysis"},
                    new DeviceOption(){Type = "K91P", Designation = "WLAN IEEE802.11ac Analysis", GlobalType = "WLAN IEEE802.11ac Analysis"},
                    new DeviceOption(){Type = "K93", Designation = "WiMAX™ IEEE802.16e OFDM/OFDMA Analysis", GlobalType = "WiMAX™ IEEE802.16e OFDM/OFDMA Analysis"},
                    new DeviceOption(){Type = "K100", Designation = "EUTRA/LTE FDD Downlink Analysis", GlobalType = "EUTRA/LTE FDD Downlink Analysis"},
                    new DeviceOption(){Type = "K101", Designation = "EUTRA/LTE FDD Uplink Analysis", GlobalType = "EUTRA/LTE FDD Uplink Analysis"},
                    new DeviceOption(){Type = "K102", Designation = "EUTRA/LTE Downlink MIMO Analysis", GlobalType = "EUTRA/LTE Downlink MIMO Analysis"},
                    new DeviceOption(){Type = "K104", Designation = "EUTRA/LTE TDD Downlink Analysis", GlobalType = "EUTRA/LTE TDD Downlink Analysis"},
                    new DeviceOption(){Type = "K105", Designation = "EUTRA/LTE TDD Uplink Analysis", GlobalType = "EUTRA/LTE TDD Uplink Analysis"},
                    new DeviceOption(){Type = "K96PC", Designation = "OFDM Vector Signal Analysis Software", GlobalType = "OFDM Vector Signal Analysis Software"},
                    new DeviceOption(){Type = "K130PC", Designation = "Distortion Analysis Software", GlobalType = "Distortion Analysis Software"},
                },
                DefaultInstrOption = new List<DeviceOption>() { },
                LoadedInstrOption = new List<DeviceOption>() { },
                TraceType = new List<ParamWithId>
                {
                    new ParamWithId {Id = (int)EN.TraceType.ClearWrite, Parameter = "WRIT" },
                    new ParamWithId {Id = (int)EN.TraceType.Average, Parameter = "AVER" },
                    new ParamWithId {Id = (int)EN.TraceType.MaxHold, Parameter = "MAXH" },
                    new ParamWithId {Id = (int)EN.TraceType.MinHold, Parameter = "MINH" },
                    new ParamWithId {Id = (int)EN.TraceType.View, Parameter = "VIEW" },
                    new ParamWithId {Id = (int)EN.TraceType.Blank, Parameter = "BLAN" }
                },
                TraceDetector = new List<ParamWithId>
                {
                    new ParamWithId {Id = (int)EN.TraceDetector.AutoSelect, Parameter = "Auto Select" },
                    new ParamWithId {Id = (int)EN.TraceDetector.AutoPeak, Parameter = "APE" },
                    new ParamWithId {Id = (int)EN.TraceDetector.Average, Parameter = "AVER" },
                    new ParamWithId {Id = (int)EN.TraceDetector.MaxPeak, Parameter = "POS" },
                    new ParamWithId {Id = (int)EN.TraceDetector.MinPeak, Parameter = "NEG" },
                    new ParamWithId {Id = (int)EN.TraceDetector.Sample, Parameter = "SAMP" },
                    new ParamWithId {Id = (int)EN.TraceDetector.RMS, Parameter = "RMS" }
                },
                SweepType =  new List<ParamWithId>
                {
                    new ParamWithId {Id = (int)EN.SweepType.Auto, Parameter = "AUTO" },
                    new ParamWithId {Id = (int)EN.SweepType.Sweep, Parameter = "SWE" },
                    new ParamWithId {Id = (int)EN.SweepType.FFT, Parameter = "FFT" }
                },
                LevelUnits = new List<ParamWithId>
                {
                    new ParamWithId {Id = (int)MEN.LevelUnit.dBm, Parameter = "DBM" },
                    new ParamWithId {Id = (int)MEN.LevelUnit.dBmV, Parameter = "DBMV" },
                    new ParamWithId {Id = (int)MEN.LevelUnit.dBµV, Parameter = "DBUV" },
                },
                SweepPointArr = new int[]{ 101, 125, 155, 173, 201, 251, 301, 313, 345, 401, 501, 601, 625, 691, 701, 801, 901, 1001, 1251, 1383, 1999, 2001, 2501, 2765, 3001, 4001, 5001, 6001, 7001, 8001, 9001, 10001, 11001, 12001, 13001, 14001, 15001, 16001, 17001, 18001, 19001, 20001, 21001, 22001, 23001, 24001, 25001, 26001, 27001, 28001, 29001, 30001, 31001, 32001 },
                DefaultSweepPoint = 691,
                RBWArr = new decimal[]{ 1, 2, 3, 5, 10, 20, 30, 50, 100, 200, 300, 500, 1000, 2000, 3000, 5000, 6250, 10000, 20000, 30000, 50000, 100000, 200000, 300000, 500000, 1000000, 2000000, 3000000, 5000000, 10000000 },
                VBWArr = new decimal[]{ 1, 2, 3, 5, 10, 20, 30, 50, 100, 200, 300, 500, 1000, 2000, 3000, 5000, 10000, 20000, 30000, 50000, 100000, 200000, 300000, 500000, 1000000, 2000000, 3000000, 5000000, 10000000, 20000000, 28000000, 40000000 },
                CouplingRatio = true,
                AttMax = 70,
                AttStep = 5,
                PreAmp = false,
                Battery = false,

                IQAvailable = true,
             },
            #endregion FSVA
            #region ESRP
            new LocalSpectrumAnalyzerInfo
            {
                InstrManufacture = 1, NumberOfTrace = 3, ChangeableSweepType = true, SweepPointFix = false, InstrModel = "ESRP",
                HiSpeed = true,
                InstrOption = new List<DeviceOption>()
                {
                    new DeviceOption(){Type = "B1", Designation = "Ruggedized Housing", GlobalType = "Ruggedized Housing"},
                    new DeviceOption(){Type = "B2", Designation = "Preselection and RF Preamplifier", GlobalType = "Preselection and Preamplifier"},
                    new DeviceOption(){Type = "B22", Designation = "RF Preamplifier (100 kHz to 7 GHz)", GlobalType = "Preamplifier"},
                    new DeviceOption(){Type = "B4", Designation = "OCXO, Precision Reference Frequency", GlobalType = "OCXO, Precision Reference Frequency"},
                    new DeviceOption(){Type = "B4", Designation = "OCXO, Enhanced Frequency Stability", GlobalType = "OCXO, Enhanced Frequency Stability"},
                    new DeviceOption(){Type = "B9", Designation = "Tracking Generator, 9 kHz to 7 GHz", GlobalType = "Tracking Generator"},
                    new DeviceOption(){Type = "B18", Designation = "Solid State Drive (SSD, removable)", GlobalType = "SSD"},
                    new DeviceOption(){Type = "B19", Designation = "Spare Hard Disk Drive (HDD, removable)", GlobalType = "HDD"},
                    new DeviceOption(){Type = "B29", Designation = "Frequency Extension 10 Hz, including EMI bandwidths in decade steps", GlobalType = "Low Start Frequency"},
                    new DeviceOption(){Type = "B30", Designation = "DC Power Supply, 12 V to 15 V", GlobalType = "DC Power Supply"},
                    new DeviceOption(){Type = "B32", Designation = "Lithium-Ion Battery Pack", GlobalType = "Lithium-Ion Battery Pack"},
                    new DeviceOption(){Type = "K0", Designation = "DEMO OPTION (All inclusive)", GlobalType = "DEMO OPTION (All inclusive)"},
                    new DeviceOption(){Type = "K53", Designation = "Time Domain Scan", GlobalType = "Time Domain Scan"},
                    new DeviceOption(){Type = "K56", Designation = "IF Analysis", GlobalType = "IF Analysis"},
                },
                DefaultInstrOption = new List<DeviceOption>() { },
                LoadedInstrOption = new List<DeviceOption>() { },
                TraceType = new List<ParamWithId>
                {
                    new ParamWithId {Id = (int)EN.TraceType.ClearWrite, Parameter = "WRIT" },
                    new ParamWithId {Id = (int)EN.TraceType.Average, Parameter = "AVER" },
                    new ParamWithId {Id = (int)EN.TraceType.MaxHold, Parameter = "MAXH" },
                    new ParamWithId {Id = (int)EN.TraceType.MinHold, Parameter = "MINH" },
                    new ParamWithId {Id = (int)EN.TraceType.Blank, Parameter = "BLAN" },
                    new ParamWithId {Id = (int)EN.TraceType.View, Parameter = "VIEW" },
                },
                TraceDetector = new List<ParamWithId>
                {
                    new ParamWithId {Id = (int)EN.TraceDetector.AutoSelect, Parameter = "Auto Select" },
                    new ParamWithId {Id = (int)EN.TraceDetector.AutoPeak, Parameter = "APE" },
                    new ParamWithId {Id = (int)EN.TraceDetector.Average, Parameter = "AVER" },
                    new ParamWithId {Id = (int)EN.TraceDetector.MaxPeak, Parameter = "POS" },
                    new ParamWithId {Id = (int)EN.TraceDetector.MinPeak, Parameter = "NEG" },
                    new ParamWithId {Id = (int)EN.TraceDetector.Sample, Parameter = "SAMP" },
                    new ParamWithId {Id = (int)EN.TraceDetector.RMS, Parameter = "RMS" }
                },
                SweepType =  new List<ParamWithId>
                {
                    new ParamWithId {Id = (int)EN.SweepType.Auto, Parameter = "AUTO" },
                    new ParamWithId {Id = (int)EN.SweepType.Sweep, Parameter = "SWE" },
                    new ParamWithId {Id = (int)EN.SweepType.FFT, Parameter = "FFT" }
                },
                LevelUnits = new List<ParamWithId>
                {
                    new ParamWithId {Id = (int)MEN.LevelUnit.dBm, Parameter = "DBM" },
                    new ParamWithId {Id = (int)MEN.LevelUnit.dBmV, Parameter = "DBMV" },
                    new ParamWithId {Id = (int)MEN.LevelUnit.dBµV, Parameter = "DBUV" },
                },
                SweepPointArr = new int[]{ 101, 125, 155, 173, 201, 251, 301, 313, 345, 401, 501, 601, 625, 691, 701, 801, 901, 1001, 1251, 1383, 1999, 2001, 2501, 2765, 3001, 4001, 5001, 6001, 7001, 8001, 9001, 10001, 11001, 12001, 13001, 14001, 15001, 16001, 17001, 18001, 19001, 20001, 21001, 22001, 23001, 24001, 25001, 26001, 27001, 28001, 29001, 30001, 31001, 32001 },
                DefaultSweepPoint = 691,
                RBWArr = new decimal[]{ 1, 2, 3, 5, 10, 20, 30, 50, 100, 200, 300, 500, 1000, 2000, 3000, 5000, 6250, 10000, 20000, 30000, 50000, 100000, 200000, 300000, 500000, 1000000, 2000000, 3000000, 5000000, 10000000 },
                VBWArr = new decimal[]{ 1, 2, 3, 5, 10, 20, 30, 50, 100, 200, 300, 500, 1000, 2000, 3000, 5000, 10000, 20000, 30000, 50000, 100000, 200000, 300000, 500000, 1000000, 2000000, 3000000, 5000000, 10000000, 20000000, 28000000, 40000000 },
                CouplingRatio = true,
                AttMax = 70,
                AttStep = 5,
                PreAmp = false,
                Battery = false,


                IQAvailable = false,
            },
            #endregion ESRP
            #region FPH
            new LocalSpectrumAnalyzerInfo
            {
                InstrManufacture = 1, NumberOfTrace = 1, ChangeableSweepType = false, SweepPointFix = true, InstrModel = "FPH",
                HiSpeed = false,
                InstrOption = new List<DeviceOption>()
                {
                    new DeviceOption(){Type = "B22", Designation = "Preamplifier", GlobalType = "Preamplifier"},
                    new DeviceOption(){Type = "B3", Designation = "Frequency Range to 3 GHz", GlobalType = "Frequency Range to 3 GHz"},
                    new DeviceOption(){Type = "B4", Designation = "Frequency Range to 4 GHz", GlobalType = "Frequency Range to 4 GHz"},
                    new DeviceOption(){Type = "K0", Designation = "DEMO OPTION (All inclusive)", GlobalType = "DEMO OPTION (All inclusive)"},
                    new DeviceOption(){Type = "K7", Designation = "Analog Modulation Analysis AM/FM", GlobalType = "Analog Modulation Analysis AM/FM"},
                    new DeviceOption(){Type = "K9", Designation = "Power Sensor Support", GlobalType = "Power Sensor Support"},
                    new DeviceOption(){Type = "K15", Designation = "Interference Analysis", GlobalType = "Interference Analysis"},
                    new DeviceOption(){Type = "K16", Designation = "Signal Strength Mapping", GlobalType = "Signal Strength Mapping"},
                    new DeviceOption(){Type = "K19", Designation = "Channel Power Meter", GlobalType = "Channel Power Meter"},
                    new DeviceOption(){Type = "K29", Designation = "Pulse Measurements with Power Sensor", GlobalType = "Pulse Measurements with Power Sensor"},
                    new DeviceOption(){Type = "K43", Designation = "Receiver Mode", GlobalType = "Receiver Mode"},
                },
                DefaultInstrOption = new List<DeviceOption>() { },
                LoadedInstrOption = new List<DeviceOption>() { },
                TraceType = new List<ParamWithId>
                {
                    new ParamWithId {Id = (int)EN.TraceType.ClearWrite, Parameter = "WRIT" },
                    new ParamWithId {Id = (int)EN.TraceType.Average, Parameter = "AVER" },
                    new ParamWithId {Id = (int)EN.TraceType.MaxHold, Parameter = "MAXH" },
                    new ParamWithId {Id = (int)EN.TraceType.MinHold, Parameter = "MINH" },
                    new ParamWithId {Id = (int)EN.TraceType.View, Parameter = "VIEW" },
                    new ParamWithId {Id = (int)EN.TraceType.Blank, Parameter = "BLAN" }
                },
                TraceDetector = new List<ParamWithId>
                {
                    new ParamWithId {Id = (int)EN.TraceDetector.AutoSelect, Parameter = "Auto Select" },
                    new ParamWithId {Id = (int)EN.TraceDetector.AutoPeak, Parameter = "APE" },
                    new ParamWithId {Id = (int)EN.TraceDetector.MaxPeak, Parameter = "POS" },
                    new ParamWithId {Id = (int)EN.TraceDetector.MinPeak, Parameter = "NEG" },
                    new ParamWithId {Id = (int)EN.TraceDetector.Sample, Parameter = "SAMP" },
                    new ParamWithId {Id = (int)EN.TraceDetector.RMS, Parameter = "RMS" }
                },
                LevelUnits = new List<ParamWithId>
                {
                    new ParamWithId {Id = (int)MEN.LevelUnit.dBm, Parameter = "DBM" },
                    new ParamWithId {Id = (int)MEN.LevelUnit.dBmV, Parameter = "DBMV" },
                    new ParamWithId {Id = (int)MEN.LevelUnit.dBµV, Parameter = "DBUV" },
                },
                SweepType = new List<ParamWithId> {},
                SweepPointArr = new int[]{ 711 },
                DefaultSweepPoint = 711,
                RBWArr = new decimal[]{ 1, 3, 10, 30, 100, 300, 1000, 3000, 10000, 30000, 100000, 300000, 1000000, 3000000 },
                VBWArr = new decimal[]{ 1, 3, 10, 30, 100, 300, 1000, 3000, 10000, 30000, 100000, 300000, 1000000, 3000000 },
                CouplingRatio = false,
                AttMax = 40,
                AttStep = 5,
                RangeArr = new decimal[] {5, 10, 20, 30, 50, 100, 120, 130, 150 },
                PreAmp = false,
                Battery = true,
                RangeFixed = true,

                IQAvailable = false,
            },
            #endregion FPH
            #region FSH
            new LocalSpectrumAnalyzerInfo
            {
                InstrManufacture = 1, NumberOfTrace = 1, ChangeableSweepType = false, SweepPointFix = true, InstrModel = "FSH",
                HiSpeed = false,
                InstrOption = new List<DeviceOption>()
                {
                     new DeviceOption(){Type = "B22", Designation = "Preamplifier", GlobalType = "Preamplifier"},
                     new DeviceOption(){Type = "K1", Designation = "Spectrum Analysis Application", GlobalType = "Spectrum Analysis Application"},
                     new DeviceOption(){Type = "K0", Designation = "DEMO OPTION (All inclusive)", GlobalType = "DEMO OPTION (All inclusive)"},
                     new DeviceOption(){Type = "K9", Designation = "Power Meter Measurement Application with R&S®FSH-Zxx or R&S®NRP-Zxx power sensors", GlobalType = "Power Sensor Support"},
                     new DeviceOption(){Type = "K14", Designation = "Spectrogram Measurement Application", GlobalType = "Spectrogram Measurement Application"},
                     new DeviceOption(){Type = "K19", Designation = "Channel Power Meter", GlobalType = "Channel Power Meter"},
                     new DeviceOption(){Type = "K29", Designation = "Pulse Measurements with Power Sensor", GlobalType = "Pulse Measurements with Power Sensor"},
                     new DeviceOption(){Type = "K39", Designation = "Transmission Measurement Application", GlobalType = "Transmission Measurement Application"},
                     new DeviceOption(){Type = "K40", Designation = "Remote Control via USB or LAN Application", GlobalType = "Remote Control"},
                     new DeviceOption(){Type = "K42", Designation = "Vector Network Analysis Application", GlobalType = "Vector Network Analysis Application"},
                     new DeviceOption(){Type = "K45", Designation = "Vector Voltmeter Measurement Application", GlobalType = "Vector Voltmeter Measurement Application"},
                },
                DefaultInstrOption = new List<DeviceOption>()
                {
                    new DeviceOption(){Type = "B22", Designation = "Preamplifier", GlobalType = "Preamplifier"}
                },
                LoadedInstrOption = new List<DeviceOption>(),
                TraceType = new List<ParamWithId>
                {
                    new ParamWithId {Id = (int)EN.TraceType.ClearWrite, Parameter = "WRIT" },
                    new ParamWithId {Id = (int)EN.TraceType.Average, Parameter = "AVER" },
                    new ParamWithId {Id = (int)EN.TraceType.MaxHold, Parameter = "MAXH" },
                    new ParamWithId {Id = (int)EN.TraceType.MinHold, Parameter = "MINH" },
                    new ParamWithId {Id = (int)EN.TraceType.View, Parameter = "VIEW" },
                    new ParamWithId {Id = (int)EN.TraceType.Blank, Parameter = "BLAN" }
                },
                TraceDetector = new List<ParamWithId>
                {
                    new ParamWithId {Id = (int)EN.TraceDetector.AutoSelect, Parameter = "Auto Select" },
                    new ParamWithId {Id = (int)EN.TraceDetector.AutoPeak, Parameter = "APE" },
                    new ParamWithId {Id = (int)EN.TraceDetector.MaxPeak, Parameter = "POS" },
                    new ParamWithId {Id = (int)EN.TraceDetector.MinPeak, Parameter = "NEG" },
                    new ParamWithId {Id = (int)EN.TraceDetector.Sample, Parameter = "SAMP" },
                    new ParamWithId {Id = (int)EN.TraceDetector.RMS, Parameter = "RMS" }
                },
                LevelUnits = new List<ParamWithId>
                {
                    new ParamWithId {Id = (int)MEN.LevelUnit.dBm, Parameter = "DBM" },
                    new ParamWithId {Id = (int)MEN.LevelUnit.dBmV, Parameter = "DBMV" },
                    new ParamWithId {Id = (int)MEN.LevelUnit.dBµV, Parameter = "DBUV" },
                },
                SweepType =  new List<ParamWithId> {},
                SweepPointArr = new int[]{ 550 },
                DefaultSweepPoint = 550,
                RBWArr = new decimal[]{ 1, 3, 10, 30, 100, 300, 1000, 3000, 10000, 30000, 100000, 300000, 1000000, 3000000 },
                VBWArr = new decimal[]{ 1, 3, 10, 30, 100, 300, 1000, 3000, 10000, 30000, 100000, 300000, 1000000, 3000000 },
                CouplingRatio = false,
                AttMax = 40,
                AttStep = 5,
                PreAmp = false,
                Battery = true,

                IQAvailable = false,
            },
            #endregion FSH
            #region FPL
            new LocalSpectrumAnalyzerInfo
            {
                InstrManufacture = 1,  NumberOfTrace = 3, ChangeableSweepType = true, SweepPointFix = false, InstrModel = "FPL",
                HiSpeed = true,
                InstrOption = new List<DeviceOption>()
                {
                     new DeviceOption(){Type = "B4", Designation = "OCXO Reference Frequency", GlobalType = "OCXO Reference"},
                     new DeviceOption(){Type = "B5", Designation = "Additional Interfaces", GlobalType = "Additional Interfaces"},
                     new DeviceOption(){Type = "B10", Designation = "GPIB Interface", GlobalType = "GPIB Interface"},
                     new DeviceOption(){Type = "B19", Designation = "Second Hard Disk (SSD)", GlobalType = "SSD"},
                     new DeviceOption(){Type = "B22", Designation = "RF Preamplifier", GlobalType = "Preamplifier"},
                     new DeviceOption(){Type = "B25", Designation = "1 dB Steps for Electronic Attenuator", GlobalType = "Attenuator 1 dB"},
                     new DeviceOption(){Type = "B30", Designation = "DC Power Supply 12/24 V", GlobalType = "DC Power Supply"},
                     new DeviceOption(){Type = "B31", Designation = "Internal Li-Ion Battery", GlobalType = "Internal Li-Ion Battery"},
                     new DeviceOption(){Type = "B40", Designation = "40 MHz Analysis Bandwidth", GlobalType = "40 MHz Analysis Bandwidth"},
                     new DeviceOption(){Type = "K7", Designation = "Analog Modulation Analysis (AM/FM/φM)", GlobalType = "Analog Modulation Analysis"},
                     new DeviceOption(){Type = "K9", Designation = "Power Sensor Measurement with R&S®NRP Power Sensors", GlobalType =  "Power Sensor Support"},
                     new DeviceOption(){Type = "K30", Designation = "Noise Figure Measurement Application", GlobalType =  "Noise Figure Measurement Application"},

                },
                DefaultInstrOption = new List<DeviceOption>() { },
                LoadedInstrOption = new List<DeviceOption>() { },
                TraceType = new List<ParamWithId>
                {
                    new ParamWithId {Id = (int)EN.TraceType.ClearWrite, Parameter = "WRIT" },
                    new ParamWithId {Id = (int)EN.TraceType.Average, Parameter = "AVER" },
                    new ParamWithId {Id = (int)EN.TraceType.MaxHold, Parameter = "MAXH" },
                    new ParamWithId {Id = (int)EN.TraceType.MinHold, Parameter = "MINH" },
                    new ParamWithId {Id = (int)EN.TraceType.View, Parameter = "VIEW" },
                    new ParamWithId {Id = (int)EN.TraceType.Blank, Parameter = "BLAN" }
                },
                TraceDetector = new List<ParamWithId>
                {
                    new ParamWithId {Id = (int)EN.TraceDetector.AutoSelect, Parameter = "Auto Select" },
                    new ParamWithId {Id = (int)EN.TraceDetector.AutoPeak, Parameter = "APE" },
                    new ParamWithId {Id = (int)EN.TraceDetector.Average, Parameter = "AVER" },
                    new ParamWithId {Id = (int)EN.TraceDetector.MaxPeak, Parameter = "POS" },
                    new ParamWithId {Id = (int)EN.TraceDetector.MinPeak, Parameter = "NEG" },
                    new ParamWithId {Id = (int)EN.TraceDetector.Sample, Parameter = "SAMP" },
                    new ParamWithId {Id = (int)EN.TraceDetector.RMS, Parameter = "RMS" }
                },
                SweepType =  new List<ParamWithId>
                {
                    new ParamWithId {Id = (int)EN.SweepType.Auto, Parameter = "AUTO" },
                    new ParamWithId {Id = (int)EN.SweepType.Sweep, Parameter = "SWE" },
                    new ParamWithId {Id = (int)EN.SweepType.FFT, Parameter = "FFT" }
                },
                LevelUnits = new List<ParamWithId>
                {
                    new ParamWithId {Id = (int)MEN.LevelUnit.dBm, Parameter = "DBM" },
                    new ParamWithId {Id = (int)MEN.LevelUnit.dBmV, Parameter = "DBMV" },
                    new ParamWithId {Id = (int)MEN.LevelUnit.dBµV, Parameter = "DBUV" },
                    new ParamWithId {Id = (int)MEN.LevelUnit.Watt, Parameter = "WATT" },
                },
                SweepPointArr = new int[]{ 101, 125, 155, 173, 201, 251, 301, 313, 345, 401, 501, 601, 625, 691, 701, 801, 901, 1001,
                    1251, 1383, 1999, 2001, 2501, 2765, 3001, 4001, 5001, 6001, 7001, 8001, 9001,
                    10001, 11001, 12001, 13001, 14001, 15001, 16001, 17001, 18001, 19001,
                    20001, 21001, 22001, 23001, 24001, 25001, 26001, 27001, 28001, 29001,
                    30001, 31001, 32001, 33001, 34001, 35001, 36001, 37001, 38001, 39001,
                    40001, 41001, 42001, 43001, 44001, 45001, 46001, 47001, 48001, 49001,
                    50001, 51001, 52001, 53001, 54001, 55001, 56001, 57001, 58001, 59001,
                    60001, 61001, 62001, 63001, 64001, 65001, 66001, 67001, 68001, 69001,
                    70001, 71001, 72001, 73001, 74001, 75001, 76001, 77001, 78001, 79001,
                    80001, 81001, 82001, 83001, 84001, 85001, 86001, 87001, 88001, 89001,
                    90001, 91001, 92001, 93001, 94001, 95001, 96001, 97001, 98001, 99001,
                    100001 },
                DefaultSweepPoint = 1001,
                RBWArr = new decimal[]{ 1, 2, 3, 5, 10, 20, 30, 50, 100, 200, 300, 500, 1000, 2000, 3000, 5000, 6250, 10000, 20000, 30000, 50000, 100000, 200000, 300000, 500000, 1000000, 2000000, 3000000, 5000000, 10000000 },
                VBWArr = new decimal[]{ 1, 2, 3, 5, 10, 20, 30, 50, 100, 200, 300, 500, 1000, 2000, 3000, 5000, 10000, 20000, 30000, 50000, 100000, 200000, 300000, 500000, 1000000, 2000000, 3000000, 5000000, 10000000 },
                RangeArr = new decimal[]{1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 15, 20, 30, 40, 50, 60, 70, 80, 90, 100, 110, 120, 130, 140, 150, 160, 170, 180, 190, 200 },
                RangeFixed = false,
                SWTMin = 0.000075m,
                SWTMax = 8000,
                RefLevelMax = 30,
                RefLevelMin = -130,
                RefLevelStep = 1,

                CouplingRatio = true,
                AttMax = 45,
                AttStep = 5,
                PreAmp = false,
                Battery = false,

                IQAvailable = true,
                IQMaxSampleSpeed = 16 * 1000000,
                IQMinSampleSpeed = 80,
                IQMaxSampleLength = 25 * 1000000,
                TriggerOffsetMax = 20,
                OptimizationAvailable = true,
                Optimization =  new List<ParamWithId>
                {
                    new ParamWithId {Id = (int)EN.Optimization.Auto, Parameter = "AUTO" },
                    new ParamWithId {Id = (int)EN.Optimization.Dynamic, Parameter = "DYN" },
                    new ParamWithId {Id = (int)EN.Optimization.Speed, Parameter = "SPE" }
                },

                FreqMin = 5000,
                FreqMax = 3000000000,
            },
            #endregion FSV
            #region ZVH
            new LocalSpectrumAnalyzerInfo
            {
                InstrManufacture = 1, NumberOfTrace = 1, ChangeableSweepType = false, SweepPointFix = true, InstrModel = "ZVH",
                HiSpeed = false,
                InstrOption = new List<DeviceOption>()
                {
                    new DeviceOption(){Type = "K0", Designation = "DEMO OPTION (All inclusive)", GlobalType = "DEMO OPTION (All inclusive)"},
                    new DeviceOption(){Type = "K1", Designation = "Spectrum Analysis Application", GlobalType = "Spectrum Analysis Application"},
                    new DeviceOption(){Type = "K9", Designation = "Power Meter Measurement Application with R&S®FSH-Zxx or R&S®NRP-Zxx power sensors", GlobalType = "Power Sensor Support"},
                    new DeviceOption(){Type = "K14", Designation = "Spectrogram Measurement Application", GlobalType = "Spectrogram Measurement Application"},
                    new DeviceOption(){Type = "K19", Designation = "Channel Power Meter", GlobalType = "Channel Power Meter"},
                    new DeviceOption(){Type = "K29", Designation = "Pulse Measurements with Power Sensor", GlobalType = "Pulse Measurements with Power Sensor"},
                    new DeviceOption(){Type = "K39", Designation = "Transmission Measurement Application", GlobalType = "Transmission Measurement Application"},
                    new DeviceOption(){Type = "K40", Designation = "Remote Control via USB or LAN Application", GlobalType = "Remote Control"},
                    new DeviceOption(){Type = "K42", Designation = "Vector Network Analysis Application", GlobalType = "Vector Network Analysis Application"},
                    new DeviceOption(){Type = "K45", Designation = "Vector Voltmeter Measurement Application", GlobalType = "Vector Voltmeter Measurement Application"},
                },
                DefaultInstrOption = new List<DeviceOption>()
                {
                    new DeviceOption(){Type = "B22", Designation = "Preamplifier", GlobalType = "Preamplifier"}
                },
                LoadedInstrOption = new List<DeviceOption>(),
                TraceType = new List<ParamWithId>
                {
                    new ParamWithId {Id = (int)EN.TraceType.ClearWrite, Parameter = "WRIT" },
                    new ParamWithId {Id = (int)EN.TraceType.Average, Parameter = "AVER" },
                    new ParamWithId {Id = (int)EN.TraceType.MaxHold, Parameter = "MAXH" },
                    new ParamWithId {Id = (int)EN.TraceType.MinHold, Parameter = "MINH" },
                    new ParamWithId {Id = (int)EN.TraceType.View, Parameter = "VIEW" },
                    new ParamWithId {Id = (int)EN.TraceType.Blank, Parameter = "BLAN" }
                },
                TraceDetector = new List<ParamWithId>
                {
                    new ParamWithId {Id = (int)EN.TraceDetector.AutoSelect, Parameter = "Auto Select" },
                    new ParamWithId {Id = (int)EN.TraceDetector.AutoPeak, Parameter = "APE" },
                    new ParamWithId {Id = (int)EN.TraceDetector.MaxPeak, Parameter = "POS" },
                    new ParamWithId {Id = (int)EN.TraceDetector.MinPeak, Parameter = "NEG" },
                    new ParamWithId {Id = (int)EN.TraceDetector.Sample, Parameter = "SAMP" },
                    new ParamWithId {Id = (int)EN.TraceDetector.RMS, Parameter = "RMS" }
                },
                LevelUnits = new List<ParamWithId>
                {
                    new ParamWithId {Id = (int)MEN.LevelUnit.dBm, Parameter = "DBM" },
                    new ParamWithId {Id = (int)MEN.LevelUnit.dBmV, Parameter = "DBMV" },
                    new ParamWithId {Id = (int)MEN.LevelUnit.dBµV, Parameter = "DBUV" },
                },
                SweepType =  new List<ParamWithId> {},
                SweepPointArr = new int[]{ 631 },
                DefaultSweepPoint = 631,
                RBWArr = new decimal[]{ 1, 3, 10, 30, 100, 300, 1000, 3000, 10000, 30000, 100000, 300000, 1000000, 3000000 },
                VBWArr = new decimal[]{ 1, 3, 10, 30, 100, 300, 1000, 3000, 10000, 30000, 100000, 300000, 1000000, 3000000 },
                CouplingRatio = false,
                AttMax = 40,
                AttStep = 5,
                PreAmp = false,
                Battery = true,

                IQAvailable = false,
            },
            #endregion ZVH
            #region N99
            new LocalSpectrumAnalyzerInfo
            {
                InstrManufacture = 2, NumberOfTrace = 1, ChangeableSweepType = true, SweepPointFix = false, InstrModel = "N99",
                HiSpeed = false,
                InstrOption = new List<DeviceOption>(),
                DefaultInstrOption = new List<DeviceOption>(),
                LoadedInstrOption = new List<DeviceOption>(),
                TraceType = new List<ParamWithId>
                {
                    new ParamWithId {Id = (int)EN.TraceType.ClearWrite, Parameter = "CLRW" },
                    new ParamWithId {Id = (int)EN.TraceType.Average, Parameter = "AVG" },
                    new ParamWithId {Id = (int)EN.TraceType.MaxHold, Parameter = "MAXH" },
                    new ParamWithId {Id = (int)EN.TraceType.MinHold, Parameter = "MINH" },
                    new ParamWithId {Id = (int)EN.TraceType.View, Parameter = "VIEW" },
                    new ParamWithId {Id = (int)EN.TraceType.Blank, Parameter = "BLANk" }
                },
                TraceDetector = new List<ParamWithId>
                {
                    new ParamWithId {Id = (int)EN.TraceDetector.AutoSelect, Parameter = "AUTO" },
                    new ParamWithId {Id = (int)EN.TraceDetector.Average, Parameter = "AVER" },
                    new ParamWithId {Id = (int)EN.TraceDetector.MaxPeak, Parameter = "POS" },
                    new ParamWithId {Id = (int)EN.TraceDetector.MinPeak, Parameter = "NEG" },
                    new ParamWithId {Id = (int)EN.TraceDetector.Sample, Parameter = "SAMP" },
                    new ParamWithId {Id = (int)EN.TraceDetector.Normal, Parameter = "  NORMal" }
                },
                SweepType =  new List<ParamWithId>
                {
                    new ParamWithId {Id = (int)EN.SweepType.Auto, Parameter = "AUTO" },
                    new ParamWithId {Id = (int)EN.SweepType.Sweep, Parameter = "STEP" },
                    new ParamWithId {Id = (int)EN.SweepType.FFT, Parameter = "FFT" }
                },
                LevelUnits = new List<ParamWithId>
                {
                    new ParamWithId {Id = (int)MEN.LevelUnit.dBm, Parameter = "DBM" },
                    new ParamWithId {Id = (int)MEN.LevelUnit.dBmV, Parameter = "DBMV" },
                    new ParamWithId {Id = (int)MEN.LevelUnit.dBµV, Parameter = "DBUV" },
                },
                SweepPointArr = new int[]{ 101, 201, 301, 401, 501, 601, 625, 691, 701, 801, 901, 1001, 2001, 3001, 4001, 5001, 6001, 7001, 8001, 9001, 10001 },
                DefaultSweepPoint = 401,
                RBWArr = new decimal[]{ 1, 3, 10, 30, 100, 300, 1000, 3000, 10000, 30000, 100000, 300000, 1000000, 3000000, 5000000 },
                VBWArr = new decimal[]{ 1, 3, 10, 30, 100, 300, 1000, 3000, 10000, 30000, 100000, 300000, 1000000, 3000000, 5000000 },
                CouplingRatio = false,
                AttMax = 70,
                AttStep = 5,
                PreAmp = false,
                Battery = true,

                IQAvailable = false,
            },
            #endregion N99
            #region MS27
            new LocalSpectrumAnalyzerInfo
            {
                InstrManufacture = 3, NumberOfTrace = 1, ChangeableSweepType = true, SweepPointFix = true, InstrModel = "MS27",
                HiSpeed = false,
                InstrOption = new List<DeviceOption>(),
                DefaultInstrOption = new List<DeviceOption>(),
                LoadedInstrOption = new List<DeviceOption>(),
                TraceType = new List<ParamWithId>
                {
                    new ParamWithId {Id = (int)EN.TraceType.ClearWrite, Parameter = "NORM" },
                    new ParamWithId {Id = (int)EN.TraceType.Average, Parameter = "AVER" },
                    new ParamWithId {Id = (int)EN.TraceType.MaxHold, Parameter = "MAXH" },
                    new ParamWithId {Id = (int)EN.TraceType.MinHold, Parameter = "MINH" },
                    new ParamWithId {Id = (int)EN.TraceType.Blank, Parameter = "NONE" }
                },
                TraceDetector = new List<ParamWithId>
                {
                    new ParamWithId {Id = (int)EN.TraceDetector.AutoSelect, Parameter = "AUTO" },
                    new ParamWithId {Id = (int)EN.TraceDetector.Average, Parameter = "AVER" },
                    new ParamWithId {Id = (int)EN.TraceDetector.MaxPeak, Parameter = "POS" },
                    new ParamWithId {Id = (int)EN.TraceDetector.MinPeak, Parameter = "NEG" },
                    new ParamWithId {Id = (int)EN.TraceDetector.Sample, Parameter = "SAMP" },
                    new ParamWithId {Id = (int)EN.TraceDetector.Normal, Parameter = "NORMal" }
                },
                SweepType =  new List<ParamWithId>
                {
                    new ParamWithId {Id = (int)EN.SweepType.Fast, Parameter = "FAST" },
                    new ParamWithId {Id = (int)EN.SweepType.Performance, Parameter = "PERF" },
                    new ParamWithId {Id = (int)EN.SweepType.NoFFT, Parameter = "NOFF" }
                },
                LevelUnits = new List<ParamWithId>
                {
                    new ParamWithId {Id = (int)MEN.LevelUnit.dBm, Parameter = "dBm" },
                    new ParamWithId {Id = (int)MEN.LevelUnit.dBmV, Parameter = "dBmV" },
                    new ParamWithId {Id = (int)MEN.LevelUnit.dBµV, Parameter = "dBuV" },
                },
                SweepPointArr = new int[] {551},
                DefaultSweepPoint = 551,
                RBWArr = new decimal[]{ 1, 3, 10, 30, 100, 300, 1000, 3000, 10000, 30000, 100000, 300000, 1000000, 3000000, 5000000 },
                VBWArr = new decimal[]{ 1, 3, 10, 30, 100, 300, 1000, 3000, 10000, 30000, 100000, 300000, 1000000, 3000000 },
                CouplingRatio = false,
                AttMax = 70,
                AttStep = 5,
                PreAmp = false,
                Battery = true,

                IQAvailable = false,
            }
            #endregion MS27
        };

        EN.Mode Mode = EN.Mode.SpectrumAnalyzer;

        #region Freqs
        //public decimal FreqMin = 9000;
        //public decimal FreqMax = 3000000000;



        public decimal FreqCentr
        {
            get { return _FreqCentr; }
            set
            {
                _FreqCentr = value;
                _FreqStart = _FreqCentr - _FreqSpan / 2;
                _FreqStop = _FreqCentr + _FreqSpan / 2;
            }
        }
        private decimal _FreqCentr = 1000000000;

        public decimal FreqSpan
        {
            get { return _FreqSpan; }
            set
            {
                _FreqSpan = value;
                _FreqStart = _FreqCentr - _FreqSpan / 2;
                _FreqStop = _FreqCentr + _FreqSpan / 2;
            }
        }
        private decimal _FreqSpan = 20000000;

        public decimal FreqStart
        {
            get { return _FreqStart; }
            set
            {
                _FreqStart = value;
                _FreqCentr = (_FreqStart + _FreqStop) / 2;
                _FreqSpan = _FreqStop - _FreqStart;
            }
        }
        private decimal _FreqStart = 990000000;

        public decimal FreqStop
        {
            get { return _FreqStop; }
            set
            {
                _FreqStop = value;
                _FreqCentr = (_FreqStart + _FreqStop) / 2;
                _FreqSpan = _FreqStop - _FreqStart;
            }
        }
        private decimal _FreqStop = 1010000000;
        #endregion Freqs

        #region RBW / VBW
        public decimal RBW;

        private int RBWIndex
        {
            get { return _RBWIndex; }
            set
            {
                if (value > UniqueData.RBWArr.Length - 1) _RBWIndex = UniqueData.RBWArr.Length - 1;
                else if (value < 0) _RBWIndex = 0;
                else _RBWIndex = value;
                RBW = UniqueData.RBWArr[_RBWIndex];
            }
        }
        private int _RBWIndex = 0;

        public bool AutoRBW;


        public decimal VBW;

        private int VBWIndex
        {
            get { return _VBWIndex; }
            set
            {
                if (value > UniqueData.VBWArr.Length - 1) _VBWIndex = UniqueData.VBWArr.Length - 1;
                else if (value < 0) _VBWIndex = 0;
                else _VBWIndex = value;
                VBW = UniqueData.VBWArr[_VBWIndex];
            }
        }
        private int _VBWIndex = 0;

        public bool AutoVBW;
        #endregion RBW / VBW

        #region Sweep
        public decimal SweepTime;

        public bool AutoSweepTime;

        public int TracePoints;

        public int SweepPoints;
        public int SweepPointsIndex
        {
            get { return _SweepPointsIndex; }
            set
            {
                if (value > UniqueData.SweepPointArr.Length - 1) SweepPointsIndex = UniqueData.SweepPointArr.Length - 1;
                else if (value < 0) SweepPointsIndex = 0;
                else _SweepPointsIndex = value;
                SweepPoints = UniqueData.SweepPointArr[SweepPointsIndex];
            }
        }
        private int _SweepPointsIndex = 0;

        public ParamWithId SweepTypeSelected { get; set; } = new ParamWithId { Id = 0, Parameter = "" };

        private EN.Optimization Optimization
        {
            get { return _Optimization; }
            set
            {
                if (UniqueData.OptimizationAvailable)
                {
                    for (int i = 0; i < UniqueData.Optimization.Count; i++)
                    {
                        if ((int)value == UniqueData.Optimization[i].Id)
                        {
                            _Optimization = value;
                            OptimizationSelected = UniqueData.Optimization[i];
                        }
                    }
                }
            }
        }
        private EN.Optimization _Optimization = EN.Optimization.Auto;
        private ParamWithId OptimizationSelected { get; set; } = new ParamWithId { Id = 0, Parameter = "" };
        #endregion

        #region Level
        public EN.PowerRegister PowerRegister = EN.PowerRegister.Normal;


        public decimal RefLevelSpec = -40;
        public decimal RefLevelIQ = -40;

        public decimal RangeSpec = 100;
        public decimal RangeIQ = 100;

        private decimal AttLevelSpec
        {
            get { return _AttLevelSpec; }
            set
            {
                if (value > UniqueData.AttMax) _AttLevelSpec = UniqueData.AttMax;
                else if (value < 0) _AttLevelSpec = 0;
                else _AttLevelSpec = value;
            }
        }
        private decimal _AttLevelSpec = 0;

        private decimal AttLevelIQ
        {
            get { return _AttLevelIQ; }
            set
            {
                if (value > UniqueData.AttMax) _AttLevelIQ = UniqueData.AttMax;
                else if (value < 0) _AttLevelIQ = 0;
                else _AttLevelIQ = value;
            }
        }
        private decimal _AttLevelIQ = 0;

        private bool AttAutoSpec;
        private bool AttAutoIQ;

        private bool PreAmpSpec;
        private bool PreAmpIQ;

        public ParamWithId LevelUnits { get; set; } = new ParamWithId() { Id = 0, Parameter = "" };

        private MEN.LevelUnit LevelUnitsResult = MEN.LevelUnit.dBm;
        #endregion Level

        #region Trace Data

        private ulong TraceCountToMeas = 1;
        private ulong TraceCount = 1;

        public double[] FreqArr;
        public float[] LevelArr;
        private float[] LevelArrTemp;//нужед ля сравнения пришедших трейсов

        private ParamWithId Trace1Type { get; set; } = new ParamWithId { Id = 0, Parameter = "BLAN" };

        private EN.TraceType TraceTypeResult = EN.TraceType.ClearWrite;

        private ParamWithId Trace1Detector { get; set; } = new ParamWithId { Id = 0, Parameter = "AutoSelect" };

        bool TraceReset;
        private AveragedTrace TraceAveraged = new AveragedTrace();


        #endregion

        #region Battery

        private decimal BatteryCharge = 0;

        public bool BatteryCharging = false;
        #endregion Battery

        #region IQ
        private decimal FreqCentrIQ { get; set; } = 1000000000;
        //private decimal FreqSpanIQ { get; set; } = 100000;
        private decimal IQBW { get; set; } = 100000;
        private decimal SampleSpeed { get; set; } = 100000;
        private int SampleLength { get; set; } = 10000;
        /// <summary>
        /// Длительность одного семпла
        /// </summary>
        public decimal SampleTimeLength { get; set; } = 10000;
        private decimal IQMeasTime { get; set; } = 10000;
        private decimal IQMeasTimeAll { get; set; } = 10000;

        #endregion IQ
        public decimal TriggerOffset { get; set; } = 10000;
        private decimal TriggerOffsetInSample { get; set; } = 10000;
        #region runs
        private bool IsRuning;
        //private long LastUpdate;
        #endregion


        #endregion Param
        #region Private Method
        private decimal DecimalParse(string str)
        {
            return decimal.Parse(str.Replace(".", decimalSeparator).Replace(",", decimalSeparator));
        }
        private bool SetConnect()
        {
            bool res = false;
            try
            {
                session = (TcpipSession)ResourceManager.GetLocalManager().Open(String.Concat("TCPIP0::", _adapterConfig.IPAddress, "::inst0::INSTR"));

                string[] temp = session.Query("*IDN?").Trim('"').Split(',');
                int InstrManufacrure = 0;
                string InstrModel = "", SerialNumber = "";
                if (temp[0].Contains("Rohde&Schwarz")) InstrManufacrure = 1;
                else if (temp[0].Contains("Keysight")) InstrManufacrure = 2;
                else if (temp[0].Contains("Anritsu")) InstrManufacrure = 3;
                InstrModel = temp[1];
                SerialNumber = temp[2];
                for (int i = 0; i < AllUniqueData.Count; i++)
                {
                    #region
                    if (AllUniqueData[i].InstrManufacture == InstrManufacrure)
                    {
                        if (InstrModel.Contains(AllUniqueData[i].InstrModel))
                        {
                            UniqueData = AllUniqueData[i];
                            List<DeviceOption> Loaded = new List<DeviceOption>() { };
                            UniqueData.LoadedInstrOption = new List<DeviceOption>();
                            foreach (DeviceOption dop in UniqueData.DefaultInstrOption)
                            {
                                Loaded.Add(dop);
                            }
                            string[] op = session.Query("*OPT?").TrimEnd().ToUpper().Split(',');
                            if (op.Length > 0 && op[0] != "0")
                            {
                                bool findDemoOption = false;
                                foreach (string s in op)
                                {
                                    if (s.ToUpper() == "K0")
                                    {
                                        findDemoOption = true;
                                        Loaded = UniqueData.InstrOption;
                                    }
                                }
                                if (findDemoOption == false)
                                {
                                    foreach (string s in op)
                                    {
                                        foreach (DeviceOption so in UniqueData.InstrOption)
                                        {
                                            if (so.Type == s)
                                            {
                                                Loaded.Add(so);
                                            }
                                        }

                                    }
                                }
                            }
                            UniqueData.LoadedInstrOption = Loaded;
                        }
                    }
                    #endregion
                }

                SetPreset();
                if (UniqueData.InstrManufacture == 1)
                {
                    #region
                    session.Write("FORMat:DEXPort:DSEParator COMM");// POIN");//разделитель дробной части
                    if (UniqueData.HiSpeed == true)
                    {
                        session.Write(":FORM:DATA REAL,32");// ASC");//передавать трейс в ASCII
                    }
                    else if (UniqueData.HiSpeed == false)
                    {
                        session.Write("FORM:DATA REAL,32"); session.Write("INST SAN");
                    }
                    if (_adapterConfig.DisplayUpdate)
                    {
                        session.Write(":SYST:DISP:UPD ON");
                    }
                    else
                    {
                        session.Write(":SYST:DISP:UPD OFF");
                    }
                    if (UniqueData.HiSpeed == true)
                    {
                        SweepPoints = int.Parse(session.Query(":SWE:POIN?"));
                    }
                    else if (UniqueData.HiSpeed == false)
                    {
                        SweepPoints = UniqueData.DefaultSweepPoint;
                        TracePoints = SweepPoints;
                    }
                    session.DefaultBufferSize = SweepPoints * 18 + 25; //увеличиваем буфер чтобы влезло 32001 точка трейса
                    UniqueData.FreqMin = DecimalParse(session.Query(":SENSe:FREQuency:STAR? MIN"));
                    UniqueData.FreqMax = DecimalParse(session.Query(":SENSe:FREQuency:STOP? MAX"));

                    UniqueData.PreAmp = false;
                    if (UniqueData.LoadedInstrOption != null && UniqueData.LoadedInstrOption.Count > 0)
                    {
                        for (int i = 0; i < UniqueData.LoadedInstrOption.Count; i++)
                        {
                            if (UniqueData.LoadedInstrOption[i].GlobalType == "Preamplifier") { UniqueData.PreAmp = true; }
                            if (UniqueData.LoadedInstrOption[i].Type == "B25") { UniqueData.AttStep = 1; }
                        }
                    }
                    if (!UniqueData.SweepPointFix)
                    {
                        SweepPointsIndex = System.Array.IndexOf(UniqueData.SweepPointArr, SweepPoints);
                    }
                    #endregion
                }
                else if (UniqueData.InstrManufacture == 2)
                {
                    #region
                    session.Write("FORM:DATA REAL,32"); //передавать трейс побайтово //в ASCII
                    SweepPoints = int.Parse(session.Query(":SENS:SWE:POIN?").Replace('.', ','));
                    SweepPointsIndex = System.Array.IndexOf(UniqueData.SweepPointArr, SweepPoints);

                    TracePoints = SweepPoints;
                    session.DefaultBufferSize = SweepPoints * 4 + 20;
                    if (_adapterConfig.DisplayUpdate)
                    {
                        session.Write(":DISP:ENAB 1");
                    }
                    else
                    {
                        session.Write(":DISP:ENAB 0");
                    }

                    #endregion
                }
                else if (UniqueData.InstrManufacture == 3)
                {
                    #region
                    session.Write("FORM:DATA REAL,32"); //передавать трейс побайтово
                    SweepPoints = 551;
                    TracePoints = 551;
                    SweepPointsIndex = 0;
                    session.DefaultBufferSize = 2204 + 20;
                    session.Write(":MMEMory:MSIS INT");
                    if (_adapterConfig.DisplayUpdate)
                    {
                        session.Write(":DISP:ENAB 1");
                    }
                    else
                    {
                        session.Write(":DISP:ENAB 0");
                    }
                    SweepPointsIndex = System.Array.IndexOf(UniqueData.SweepPointArr, SweepPoints);
                    #endregion
                }
                #region

                GetLevelUnit();
                GetFreqCentr();
                GetFreqSpan();
                GetRBW();
                GetAutoRBW();
                GetVBW();
                GetAutoVBW();

                GetOptimization();
                SetOptimization((EN.Optimization)_adapterConfig.Optimization);

                GetSweepTime();
                GetAutoSweepTime();
                GetSweepType();
                GetSweepPoints();

                GetRefLevel();
                GetRange();
                GetAttLevel();
                GetAutoAttLevel();
                GetPreAmp();
                GetTraceType();
                GetDetectorType();


                SetIQWindow();
                SetWindowType(EN.Mode.IQAnalyzer);
                GetRefLevel();
                GetRange();
                GetAttLevel();
                GetAutoAttLevel();
                GetPreAmp();
                SetWindowType(EN.Mode.SpectrumAnalyzer);


                res = true;
                IsRuning = true;
                #endregion
            }
            #region Exception
            catch (VisaException v_exp)
            {
                _logger.Exception(Contexts.ThisComponent, v_exp);
            }
            catch (Exception exp)
            {
                _logger.Exception(Contexts.ThisComponent, exp);
            }
            #endregion   
            return res;
        }

        #region AN To Command
        /// <summary>
        /// Получаем центральную частоту 
        /// </summary>
        private void GetFreqCentr()
        {
            try
            {
                if (UniqueData.InstrManufacture == 1)
                {
                    FreqCentr = DecimalParse(session.Query(":SENSe:FREQuency:CENTer?"));
                }
                else if (UniqueData.InstrManufacture == 2)
                {
                    FreqCentr = DecimalParse(session.Query(":FREQ:CENT?"));
                }
                else if (UniqueData.InstrManufacture == 3)
                {
                    FreqCentr = DecimalParse(session.Query(":SENSe:FREQuency:CENTer?"));
                }
            }
            #region Exception
            catch (VisaException v_exp)
            {
                _logger.Exception(Contexts.ThisComponent, v_exp);
            }
            catch (Exception exp)
            {
                _logger.Exception(Contexts.ThisComponent, exp);
            }
            #endregion
            GetFreqArr();
        }
        /// <summary>
        /// получаем span
        /// </summary>
        private void GetFreqSpan()
        {
            try
            {
                if (UniqueData.InstrManufacture == 1)
                {
                    FreqSpan = DecimalParse(session.Query(":SENSe:FREQuency:SPAN?"));
                }
                else if (UniqueData.InstrManufacture == 2)
                {
                    FreqSpan = DecimalParse(session.Query(":FREQuency:SPAN?"));
                }
                else if (UniqueData.InstrManufacture == 3)
                {
                    FreqSpan = DecimalParse(session.Query(":SENSe:FREQuency:SPAN?"));
                }
            }
            #region Exception
            catch (VisaException v_exp)
            {
                _logger.Exception(Contexts.ThisComponent, v_exp);
            }
            catch (Exception exp)
            {
                _logger.Exception(Contexts.ThisComponent, exp);
            }
            #endregion
            GetFreqArr();
        }
        /// <summary>
        /// Установка Начальной частоты просмотра 
        /// </summary>
        private bool SetFreqStart(decimal freqStart)
        {
            bool res = false;
            try
            {
                FreqStart = freqStart;
                if (UniqueData.InstrManufacture == 1)
                {
                    session.Write(":SENSe:FREQ:STAR " + FreqStart.ToString().Replace(',', '.'));
                    FreqStart = DecimalParse(session.Query(":SENSe:FREQ:STAR?"));
                }
                else if (UniqueData.InstrManufacture == 2)
                {
                    session.Write("FREQ:STAR " + FreqStart.ToString().Replace(',', '.'));
                    FreqStart = DecimalParse(session.Query(":FREQ:STAR?"));
                }
                else if (UniqueData.InstrManufacture == 3)
                {
                    session.Write("FREQ:STAR " + FreqStart.ToString().Replace(',', '.'));
                    FreqCentr = DecimalParse(session.Query(":SENSe:FREQuency:CENTer?"));
                }
                res = true;
            }
            #region Exception
            catch (VisaException v_exp)
            {
                _logger.Exception(Contexts.ThisComponent, v_exp);
            }
            catch (Exception exp)
            {
                _logger.Exception(Contexts.ThisComponent, exp);
            }
            #endregion
            GetFreqArr();
            if (AutoRBW == true) { GetRBW(); }
            if (AutoVBW == true) { GetVBW(); }
            if (AutoSweepTime == true) { GetSweepTime(); }

            return res;
        }

        /// <summary>
        /// Установка Конечной частоты просмотра 
        /// </summary>
        private bool SetFreqStop(decimal freqStop)
        {
            bool res = false;
            try
            {
                FreqStop = freqStop;
                if (UniqueData.InstrManufacture == 1)
                {
                    session.Write(":SENSe:FREQ:STOP " + FreqStop.ToString().Replace(',', '.'));
                    FreqStop = DecimalParse(session.Query(":SENSe:FREQ:STOP?"));
                }
                else if (UniqueData.InstrManufacture == 2)
                {
                    session.Write(":FREQ:STOP " + FreqStop.ToString().Replace(',', '.'));
                    FreqStop = DecimalParse(session.Query(":FREQ:STOP?"));
                }
                else if (UniqueData.InstrManufacture == 3)
                {
                    session.Write(":SENSe:FREQ:STOP " + FreqStop.ToString().Replace(',', '.'));
                    FreqCentr = DecimalParse(session.Query(":SENSe:FREQuency:CENTer?"));
                }
                res = true;
            }
            #region Exception
            catch (VisaException v_exp)
            {
                _logger.Exception(Contexts.ThisComponent, v_exp);
            }
            catch (Exception exp)
            {
                _logger.Exception(Contexts.ThisComponent, exp);
            }
            #endregion
            GetFreqArr();
            if (AutoRBW == true) { GetRBW(); }
            if (AutoVBW == true) { GetVBW(); }
            if (AutoSweepTime == true) { GetSweepTime(); }

            return res;
        }

        private void GetFreqArr()
        {
            try
            {
                if (UniqueData.InstrManufacture == 1)
                {
                    if (UniqueData.HiSpeed == true)
                    {
                        session.Write("TRAC1:X? TRACE1");
                        byte[] byteArray = session.ReadByteArray();
                        if (System.Text.Encoding.ASCII.GetString(byteArray, 0, 1) == "#")
                        {
                            int lengthPreamb = int.Parse(System.Text.Encoding.ASCII.GetString(byteArray, 1, 1));
                            int lengthData = int.Parse(System.Text.Encoding.ASCII.GetString(byteArray, 2, lengthPreamb));
                            double[] temp = new double[lengthData / 4 + 2];
                            for (int j = 0; j < lengthData / 4; j++)
                            {
                                temp[j + 1] = System.BitConverter.ToSingle(byteArray, lengthPreamb + 2 + j * 4);
                            }
                            temp[0] = (double)FreqStart;
                            temp[temp.Length - 1] = (double)FreqStop;
                            FreqArr = temp;
                            LevelArr = new float[FreqArr.Length];
                            LevelArrTemp = new float[FreqArr.Length];
                            for (int i = 0; i < FreqArr.Length; i++)
                            {
                                LevelArr[i] = -1000;
                                LevelArrTemp[i] = -1000;
                            }
                        }
                    }
                    else if (UniqueData.HiSpeed == false)
                    {
                        #region
                        FreqArr = new double[TracePoints];
                        LevelArr = new float[TracePoints];
                        LevelArrTemp = new float[LevelArr.Length];
                        decimal step = (decimal)FreqSpan / (TracePoints - 1);
                        for (int i = 0; i < TracePoints; i++)
                        {
                            FreqArr[i] = (double)(FreqStart + step * i);
                            LevelArr[i] = -1000;
                            LevelArrTemp[i] = -1000;
                        }
                        #endregion
                    }
                }
                else if (UniqueData.InstrManufacture == 2)
                {
                    #region
                    FreqArr = new double[TracePoints];
                    LevelArr = new float[TracePoints];
                    LevelArrTemp = new float[LevelArr.Length];
                    decimal step = (decimal)FreqSpan / (TracePoints - 1);
                    for (int i = 0; i < TracePoints; i++)
                    {
                        FreqArr[i] = (double)(FreqStart + step * i);
                        LevelArr[i] = -1000;
                        LevelArrTemp[i] = -1000;
                    }

                    #endregion
                }
                else if (UniqueData.InstrManufacture == 3)
                {
                    #region
                    FreqArr = new double[TracePoints];
                    LevelArr = new float[TracePoints];
                    LevelArrTemp = new float[LevelArr.Length];
                    decimal step = (decimal)FreqSpan / (TracePoints - 1);
                    for (int i = 0; i < TracePoints; i++)
                    {
                        FreqArr[i] = (double)(FreqStart + step * i);
                        LevelArr[i] = -1000;
                        LevelArrTemp[i] = -1000;
                    }

                    #endregion
                }
            }
            #region Exception
            catch (Exception exp)
            {
                _logger.Exception(Contexts.ThisComponent, exp);
            }
            #endregion
        }
        /// <summary>
        /// Установка Центральной частоты просмотра IQ
        /// </summary>
        private bool SetFreqCentrIQ(decimal freqCentrIQ)
        {
            bool res = false;
            try
            {
                FreqCentrIQ = freqCentrIQ;
                if (UniqueData.InstrManufacture == 1)
                {
                    session.Write($"FREQ:CENT {FreqCentrIQ.ToString().Replace(",", ".")}");
                    FreqCentrIQ = DecimalParse(session.Query(":FREQ:CENT?"));
                }
                else if (UniqueData.InstrManufacture == 2)
                {

                }
                else if (UniqueData.InstrManufacture == 3)
                {

                }
                res = true;
            }
            #region Exception
            catch (VisaException v_exp)
            {
                _logger.Exception(Contexts.ThisComponent, v_exp);
            }
            catch (Exception exp)
            {
                _logger.Exception(Contexts.ThisComponent, exp);
            }
            #endregion
            return res;
        }

        private void SetAutoAttLevel(bool attAuto)
        {
            try
            {
                if (Mode == EN.Mode.SpectrumAnalyzer)
                {
                    AttAutoSpec = attAuto;
                    if (UniqueData.InstrManufacture == 1)
                    {
                        if (AttAutoSpec == true)
                        {
                            session.Write(":INP:ATT:AUTO 1");
                        }
                        else
                        {
                            session.Write(":INP:ATT:AUTO 0");
                        }
                        AttLevelSpec = DecimalParse(session.Query(":INP:ATT?"));
                    }
                    else if (UniqueData.InstrManufacture == 2)
                    {
                        if (AttAutoSpec == true)
                        {
                            session.Write(":POW:ATT:AUTO 1");
                        }
                        else
                        {
                            session.Write(":POW:ATT:AUTO 0");
                        }
                        AttLevelSpec = DecimalParse(session.Query(":POW:ATT?"));
                    }
                    else if (UniqueData.InstrManufacture == 3)
                    {
                        if (AttAutoSpec == true)
                        {
                            session.Write(":POW:ATT:AUTO 1");
                        }
                        else
                        {
                            session.Write(":POW:ATT:AUTO 0");
                        }
                        AttLevelSpec = DecimalParse(session.Query(":POW:ATT?"));
                    }
                }
                else if (Mode == EN.Mode.IQAnalyzer)
                {
                    AttAutoIQ = attAuto;
                    if (UniqueData.InstrManufacture == 1)
                    {
                        if (AttAutoIQ == true)
                        {
                            session.Write(":INP:ATT:AUTO 1");
                        }
                        else
                        {
                            session.Write(":INP:ATT:AUTO 0");
                        }
                        AttLevelIQ = DecimalParse(session.Query(":INP:ATT?"));
                    }
                    else if (UniqueData.InstrManufacture == 2)
                    {
                        if (AttAutoIQ == true)
                        {
                            session.Write(":POW:ATT:AUTO 1");
                        }
                        else
                        {
                            session.Write(":POW:ATT:AUTO 0");
                        }
                        AttLevelIQ = DecimalParse(session.Query(":POW:ATT?"));
                    }
                    else if (UniqueData.InstrManufacture == 3)
                    {
                        if (AttAutoIQ == true)
                        {
                            session.Write(":POW:ATT:AUTO 1");
                        }
                        else
                        {
                            session.Write(":POW:ATT:AUTO 0");
                        }
                        AttLevelIQ = DecimalParse(session.Query(":POW:ATT?"));
                    }
                }
            }
            #region Exception
            catch (VisaException v_exp)
            {
                _logger.Exception(Contexts.ThisComponent, v_exp);
            }
            catch (Exception exp)
            {
                _logger.Exception(Contexts.ThisComponent, exp);
            }
            #endregion
        }
        /// <summary>
        /// Вкл/Выкл атоматического аттенюатора зависящего от опорного уровня, при выкл
        /// АвтоАТТ изменяем настройку аттенюатора
        /// </summary>
        private bool SetAttLevel(decimal attLevel)
        {
            bool res = false;
            try
            {
                if (Mode == EN.Mode.SpectrumAnalyzer)
                {
                    AttLevelSpec = attLevel;
                    if (AttAutoSpec)
                    {
                        AttAutoSpec = false;
                    }
                    if (UniqueData.InstrManufacture == 1)
                    {
                        session.Write(":INP:ATT " + AttLevelSpec.ToString().Replace(',', '.')); //INP:ATT:AUTO
                    }
                    else if (UniqueData.InstrManufacture == 2)
                    {
                        session.Write(":POW:ATT " + AttLevelSpec.ToString().Replace(',', '.'));
                    }
                }
                else if (Mode == EN.Mode.IQAnalyzer)
                {
                    AttLevelIQ = attLevel;
                    if (AttAutoIQ)
                    {
                        AttAutoIQ = false;
                    }
                    if (UniqueData.InstrManufacture == 1)
                    {
                        session.Write(":INP:ATT " + AttLevelIQ.ToString().Replace(',', '.')); //INP:ATT:AUTO
                    }
                    else if (UniqueData.InstrManufacture == 2)
                    {
                        session.Write(":POW:ATT " + AttLevelIQ.ToString().Replace(',', '.'));
                    }
                }
                res = true;
            }
            #region Exception
            catch (VisaException v_exp)
            {
                _logger.Exception(Contexts.ThisComponent, v_exp);
            }
            catch (Exception exp)
            {
                _logger.Exception(Contexts.ThisComponent, exp);
            }
            #endregion

            return res;
        }
        /// <summary>
        /// Получаем настройку аттенюатора
        /// </summary>
        private void GetAttLevel()
        {
            try
            {
                if (Mode == EN.Mode.SpectrumAnalyzer)
                {
                    if (UniqueData.InstrManufacture == 1)
                    {
                        AttLevelSpec = DecimalParse(session.Query(":INP:ATT?"));
                    }
                    else if (UniqueData.InstrManufacture == 2)
                    {
                        AttLevelSpec = DecimalParse(session.Query(":POW:ATT?"));
                    }
                    else if (UniqueData.InstrManufacture == 3)
                    {
                        string t = session.Query(":POW:ATT?");
                        if (t != "0.0") { AttLevelSpec = DecimalParse(t); }
                        else AttLevelSpec = 0;
                    }
                }
                else if (Mode == EN.Mode.IQAnalyzer)
                {
                    if (UniqueData.InstrManufacture == 1)
                    {
                        AttLevelIQ = DecimalParse(session.Query(":INP:ATT?"));
                    }
                    else if (UniqueData.InstrManufacture == 2)
                    {
                        AttLevelIQ = DecimalParse(session.Query(":POW:ATT?"));
                    }
                    else if (UniqueData.InstrManufacture == 3)
                    {
                        string t = session.Query(":POW:ATT?");
                        if (t != "0.0") { AttLevelIQ = DecimalParse(t); }
                        else AttLevelIQ = 0;
                    }
                }
            }
            #region Exception
            catch (VisaException v_exp)
            {
                _logger.Exception(Contexts.ThisComponent, v_exp);
            }
            catch (Exception exp)
            {
                _logger.Exception(Contexts.ThisComponent, exp);
            }
            #endregion
        }
        private bool SetAutoAtt(bool attauto)
        {
            bool res = false;
            try
            {
                if (Mode == EN.Mode.SpectrumAnalyzer)
                {
                    AttAutoSpec = attauto;
                    if (UniqueData.InstrManufacture == 1)
                    {
                        if (AttAutoSpec == true) session.Write(":INP:ATT:AUTO 1");
                        else session.Write(":INP:ATT:AUTO 0");
                        AttLevelSpec = DecimalParse(session.Query(":INP:ATT?"));
                    }
                    else if (UniqueData.InstrManufacture == 2)
                    {
                        if (AttAutoSpec == true) session.Write(":POW:ATT:AUTO 1");
                        else session.Write(":POW:ATT:AUTO 0");
                        AttLevelSpec = DecimalParse(session.Query(":POW:ATT?"));
                    }
                    else if (UniqueData.InstrManufacture == 3)
                    {
                        if (AttAutoSpec == true) session.Write(":POW:ATT:AUTO 1");
                        else session.Write(":POW:ATT:AUTO 0");
                        AttLevelSpec = DecimalParse(session.Query(":POW:ATT?"));
                    }
                }
                else if (Mode == EN.Mode.IQAnalyzer)
                {
                    AttAutoIQ = attauto;
                    if (UniqueData.InstrManufacture == 1)
                    {
                        if (AttAutoIQ == true) session.Write(":INP:ATT:AUTO 1");
                        else session.Write(":INP:ATT:AUTO 0");
                        AttLevelIQ = DecimalParse(session.Query(":INP:ATT?"));
                    }
                    else if (UniqueData.InstrManufacture == 2)
                    {
                        if (AttAutoIQ == true) session.Write(":POW:ATT:AUTO 1");
                        else session.Write(":POW:ATT:AUTO 0");
                        AttLevelIQ = DecimalParse(session.Query(":POW:ATT?"));
                    }
                    else if (UniqueData.InstrManufacture == 3)
                    {
                        if (AttAutoIQ == true) session.Write(":POW:ATT:AUTO 1");
                        else session.Write(":POW:ATT:AUTO 0");
                        AttLevelIQ = DecimalParse(session.Query(":POW:ATT?"));
                    }
                }
                res = true;
            }
            #region Exception
            catch (VisaException v_exp)
            {
                _logger.Exception(Contexts.ThisComponent, v_exp);
            }
            catch (Exception exp)
            {
                _logger.Exception(Contexts.ThisComponent, exp);
            }
            #endregion
            return res;
        }
        private void GetAutoAttLevel()
        {
            try
            {
                if (Mode == EN.Mode.SpectrumAnalyzer)
                {
                    string temp = "";
                    if (UniqueData.InstrManufacture == 1)
                    {
                        temp = session.Query(":INP:ATT:AUTO?").TrimEnd();
                        if (temp.Contains("1")) { AttAutoSpec = true; }
                        else if (temp.Contains("0")) { AttAutoSpec = false; }
                    }
                    else if (UniqueData.InstrManufacture == 2)
                    {
                        temp = session.Query(":POW:ATT:AUTO?").TrimEnd();
                        if (temp.Contains("1")) { AttAutoSpec = true; }
                        else if (temp.Contains("0")) { AttAutoSpec = false; }
                    }
                    else if (UniqueData.InstrManufacture == 3)
                    {
                        temp = session.Query(":POW:ATT:AUTO?").TrimEnd();
                        if (temp.Contains("1")) { AttAutoSpec = true; }
                        else if (temp.Contains("0")) { AttAutoSpec = false; }
                    }
                }
                else if (Mode == EN.Mode.IQAnalyzer)
                {
                    string temp = "";
                    if (UniqueData.InstrManufacture == 1)
                    {
                        temp = session.Query(":INP:ATT:AUTO?");
                        if (temp.Contains("1")) { AttAutoIQ = true; }
                        else if (temp.Contains("0")) { AttAutoIQ = false; }
                    }
                    else if (UniqueData.InstrManufacture == 2)
                    {
                        temp = session.Query(":POW:ATT:AUTO?");
                        if (temp.Contains("1")) { AttAutoIQ = true; }
                        else if (temp.Contains("0")) { AttAutoIQ = false; }
                    }
                    else if (UniqueData.InstrManufacture == 3)
                    {
                        temp = session.Query(":POW:ATT:AUTO?");
                        if (temp.Contains("1")) { AttAutoIQ = true; }
                        else if (temp.Contains("0")) { AttAutoIQ = false; }
                    }
                }
            }
            #region Exception
            catch (VisaException v_exp)
            {
                _logger.Exception(Contexts.ThisComponent, v_exp);
            }
            catch (Exception exp)
            {
                _logger.Exception(Contexts.ThisComponent, exp);
            }
            #endregion
        }
        /// <summary>
        /// Вкл/Выкл предусилителя 
        /// </summary>
        private bool SetPreAmp(bool preAmp)
        {
            bool res = false;
            try
            {
                if (Mode == EN.Mode.SpectrumAnalyzer)
                {
                    PreAmpSpec = preAmp;
                    if (UniqueData.InstrManufacture == 1 && UniqueData.PreAmp == true)
                    {
                        if (PreAmpSpec == true)
                        {
                            session.Write(":INP:GAIN:STAT 1");
                        }
                        else
                        {
                            session.Write(":INP:GAIN:STAT 0");
                        }
                    }
                    else if (UniqueData.InstrManufacture == 2)
                    {
                        if (PreAmpSpec == true)
                        {
                            session.Write(":POW:GAIN 1");
                        }
                        else
                        {
                            session.Write(":POW:GAIN 0");
                        }
                    }
                    else if (UniqueData.InstrManufacture == 3)
                    {
                        if (PreAmpSpec == true)
                        {
                            session.Write(":POW:GAIN 1");
                        }
                        else
                        {
                            session.Write(":POW:GAIN 0");
                        }
                    }
                }
                else if (Mode == EN.Mode.IQAnalyzer)
                {
                    PreAmpIQ = preAmp;
                    if (UniqueData.InstrManufacture == 1 && UniqueData.PreAmp == true)
                    {
                        if (PreAmpIQ == true)
                        {
                            session.Write(":INP:GAIN:STAT 1");
                        }
                        {
                            session.Write(":INP:GAIN:STAT 0");
                        }
                    }
                    else if (UniqueData.InstrManufacture == 2)
                    {
                        if (PreAmpIQ == true)
                        {
                            session.Write(":POW:GAIN 1");
                        }
                        else
                        {
                            session.Write(":POW:GAIN 0");
                        }
                    }
                    else if (UniqueData.InstrManufacture == 3)
                    {
                        if (PreAmpIQ == true)
                        {
                            session.Write(":POW:GAIN 1");
                        }
                        else
                        {
                            session.Write(":POW:GAIN 0");
                        }
                    }
                }
                res = true;
            }
            #region Exception
            catch (VisaException v_exp)
            {
                _logger.Exception(Contexts.ThisComponent, v_exp);
            }
            catch (Exception exp)
            {
                _logger.Exception(Contexts.ThisComponent, exp);
            }
            #endregion
            return res;
        }
        /// <summary>
        /// Узнаем состояние предусилителя 
        /// </summary>
        private void GetPreAmp()
        {
            try
            {
                if (Mode == EN.Mode.SpectrumAnalyzer)
                {
                    if (UniqueData.InstrManufacture == 1 && UniqueData.PreAmp == true)
                    {
                        string temp = session.Query(":INP:GAIN:STAT?").TrimEnd();
                        if (temp.Contains("1")) { PreAmpSpec = true; }
                        else if (temp.Contains("0")) { PreAmpSpec = false; }
                    }
                    else if (UniqueData.InstrManufacture == 2)
                    {
                        string temp = session.Query(":POW:GAIN:STAT?").TrimEnd();
                        if (temp.Contains("1")) { PreAmpSpec = true; }
                        else if (temp.Contains("0")) { PreAmpSpec = false; }
                    }
                    else if (UniqueData.InstrManufacture == 3)
                    {
                        string temp = session.Query(":POW:GAIN:STAT?").TrimEnd();
                        if (temp.Contains("1")) { PreAmpSpec = true; }
                        else if (temp.Contains("0")) { PreAmpSpec = false; }
                    }
                }
                else if (Mode == EN.Mode.IQAnalyzer)
                {
                    if (UniqueData.InstrManufacture == 1 && UniqueData.PreAmp == true)
                    {
                        string temp = session.Query(":INP:GAIN:STAT?").TrimEnd();
                        if (temp.Contains("1")) { PreAmpIQ = true; }
                        else if (temp.Contains("0")) { PreAmpIQ = false; }
                    }
                    else if (UniqueData.InstrManufacture == 2)
                    {
                        string temp = session.Query(":POW:GAIN:STAT?").TrimEnd();
                        if (temp.Contains("1")) { PreAmpIQ = true; }
                        else if (temp.Contains("0")) { PreAmpIQ = false; }
                    }
                    else if (UniqueData.InstrManufacture == 3)
                    {
                        string temp = session.Query(":POW:GAIN:STAT?").TrimEnd();
                        if (temp.Contains("1")) { PreAmpIQ = true; }
                        else if (temp.Contains("0")) { PreAmpIQ = false; }
                    }
                }
            }
            #region Exception
            catch (VisaException v_exp)
            {
                _logger.Exception(Contexts.ThisComponent, v_exp);
            }
            catch (Exception exp)
            {
                _logger.Exception(Contexts.ThisComponent, exp);
            }
            #endregion
        }
        /// <summary>
        /// Установка опорного уровня 
        /// </summary>
        private bool SetRefLevel(decimal refLevel)
        {
            bool res = false;
            try
            {
                if (Mode == EN.Mode.SpectrumAnalyzer)
                {
                    RefLevelSpec = refLevel;
                    if (UniqueData.InstrManufacture == 1)
                    {
                        if (UniqueData.HiSpeed)
                        {
                            session.Write(":DISPlay:WINDow:TRACe:Y:SCALe:RLEVel " + RefLevelSpec.ToString().Replace(',', '.'));
                        }
                        else
                        {
                            session.Write(":DISP:TRAC:Y:RLEV " + RefLevelSpec.ToString().Replace(',', '.'));
                        }
                    }
                    else if (UniqueData.InstrManufacture == 2)
                    {
                        session.Write("DISP:WIND:TRAC:Y:SCAL:RLEV " + RefLevelSpec.ToString().Replace(',', '.'));
                    }
                    else if (UniqueData.InstrManufacture == 3)
                    {
                        session.Write(":DISPlay:WINDow:TRACe:Y:SCALe:RLEVel " + RefLevelSpec.ToString());
                    }
                    if (AttAutoSpec == true) { GetAttLevel(); }
                }
                else if (Mode == EN.Mode.IQAnalyzer)
                {
                    RefLevelIQ = refLevel;
                    if (UniqueData.InstrManufacture == 1)
                    {
                        if (UniqueData.HiSpeed)
                        {
                            session.Write(":DISPlay:WINDow:TRACe:Y:SCALe:RLEVel " + RefLevelIQ.ToString().Replace(',', '.'));
                        }
                        else
                        {
                            session.Write(":DISP:TRAC:Y:RLEV " + RefLevelIQ.ToString().Replace(',', '.'));
                        }
                    }
                    else if (UniqueData.InstrManufacture == 2)
                    {
                        session.Write("DISP:WIND:TRAC:Y:SCAL:RLEV " + RefLevelIQ.ToString().Replace(',', '.'));
                    }
                    else if (UniqueData.InstrManufacture == 3)
                    {
                        session.Write(":DISPlay:WINDow:TRACe:Y:SCALe:RLEVel " + RefLevelIQ.ToString());
                    }
                    if (AttAutoIQ == true) { GetAttLevel(); }
                }
                res = true;
            }
            #region Exception
            catch (VisaException v_exp)
            {
                _logger.Exception(Contexts.ThisComponent, v_exp);
            }
            catch (Exception exp)
            {
                _logger.Exception(Contexts.ThisComponent, exp);
            }
            #endregion

            return res;
        }
        /// <summary>
        /// Получаем опорный уровнь 
        /// </summary>
        private void GetRefLevel()
        {
            if (Mode == EN.Mode.SpectrumAnalyzer)
            {
                if (UniqueData.InstrManufacture == 1)
                {
                    if (UniqueData.HiSpeed)
                    {
                        RefLevelSpec = Math.Round(DecimalParse(session.Query(":DISPlay:WINDow:TRACe:Y:SCALe:RLEVel?")));
                    }
                    else { RefLevelSpec = Math.Round(DecimalParse(session.Query(":DISP:TRAC:Y:RLEV?"))); }
                }
                else if (UniqueData.InstrManufacture == 2)
                {
                    RefLevelSpec = Math.Round(DecimalParse(session.Query(":DISP:WIND:TRAC:Y:SCAL:RLEV?")));
                }
                else if (UniqueData.InstrManufacture == 3)
                {
                    RefLevelSpec = Math.Round(DecimalParse(session.Query(":DISP:WIND:TRAC:Y:SCAL:RLEV?")));
                }
            }
            else if (Mode == EN.Mode.IQAnalyzer)
            {
                if (UniqueData.InstrManufacture == 1)
                {
                    if (UniqueData.HiSpeed)
                    {
                        RefLevelIQ = Math.Round(DecimalParse(session.Query(":DISPlay:WINDow:TRACe:Y:SCALe:RLEVel?")));
                    }
                    else { RefLevelIQ = Math.Round(DecimalParse(session.Query(":DISP:TRAC:Y:RLEV?"))); }
                }
                else if (UniqueData.InstrManufacture == 2)
                {
                    RefLevelIQ = Math.Round(DecimalParse(session.Query(":DISP:WIND:TRAC:Y:SCAL:RLEV?")));
                }
                else if (UniqueData.InstrManufacture == 3)
                {
                    RefLevelIQ = Math.Round(DecimalParse(session.Query(":DISP:WIND:TRAC:Y:SCAL:RLEV?")));
                }
            }
        }
        private void SetRange()
        {
            try
            {
                if (Mode == EN.Mode.SpectrumAnalyzer)
                {
                    if (UniqueData.InstrManufacture == 1)
                    {
                        session.Write(":DISP:TRAC:Y " + RangeSpec.ToString().Replace(',', '.'));
                    }
                    else if (UniqueData.InstrManufacture == 2)
                    {
                        //session.Write("DISP:WIND:TRAC:Y:SCAL:RLEV " + RefLevel.ToString().Replace(',', '.'));
                    }
                    else if (UniqueData.InstrManufacture == 3)
                    {
                        //session.Write(":DISPlay:WINDow:TRACe:Y:SCALe:RLEVel " + RefLevel.ToString());
                    }
                    if (AttAutoSpec == true) { GetAttLevel(); }
                }
                else if (Mode == EN.Mode.IQAnalyzer)
                {
                    if (UniqueData.InstrManufacture == 1)
                    {
                        session.Write(":DISP:TRAC:Y " + RangeIQ.ToString().Replace(',', '.'));
                    }
                    else if (UniqueData.InstrManufacture == 2)
                    {
                        //session.Write("DISP:WIND:TRAC:Y:SCAL:RLEV " + RefLevel.ToString().Replace(',', '.'));
                    }
                    else if (UniqueData.InstrManufacture == 3)
                    {
                        //session.Write(":DISPlay:WINDow:TRACe:Y:SCALe:RLEVel " + RefLevel.ToString());
                    }
                    if (AttAutoIQ == true) { GetAttLevel(); }
                }
            }
            #region Exception
            catch (VisaException v_exp)
            {
                _logger.Exception(Contexts.ThisComponent, v_exp);
            }
            catch (Exception exp)
            {
                _logger.Exception(Contexts.ThisComponent, exp);
            }
            #endregion

        }
        private void GetRange()
        {
            if (Mode == EN.Mode.SpectrumAnalyzer)
            {
                if (UniqueData.InstrManufacture == 1)
                {
                    RangeSpec = Math.Round(DecimalParse(session.Query(":DISP:TRAC:Y?")));
                }
                else if (UniqueData.InstrManufacture == 2)
                {
                    //Range = Math.Round(DecimalParse(session.Query(":DISP:WIND:TRAC:Y:SCAL:RLEV?")));
                }
                else if (UniqueData.InstrManufacture == 3)
                {
                    //Range = Math.Round(DecimalParse(session.Query(":DISP:WIND:TRAC:Y:SCAL:RLEV?")));
                }
            }
            else if (Mode == EN.Mode.IQAnalyzer)
            {
                if (UniqueData.InstrManufacture == 1)
                {
                    RangeIQ = Math.Round(DecimalParse(session.Query(":DISP:TRAC:Y?")));
                }
                else if (UniqueData.InstrManufacture == 2)
                {
                    //Range = Math.Round(DecimalParse(session.Query(":DISP:WIND:TRAC:Y:SCAL:RLEV?")));
                }
                else if (UniqueData.InstrManufacture == 3)
                {
                    //Range = Math.Round(DecimalParse(session.Query(":DISP:WIND:TRAC:Y:SCAL:RLEV?")));
                }
            }

        }


        /// <summary>
        /// Установка Units
        /// </summary>
        private bool SetLevelUnit(ParamWithId levelunits)
        {
            bool res = false;
            try
            {
                LevelUnits = levelunits;
                if (UniqueData.InstrManufacture == 1)
                {
                    session.Write(":UNIT:POWer " + LevelUnits.Parameter);
                }
                else if (UniqueData.InstrManufacture == 2)
                {
                    session.Write(":AMPL:UNIT " + LevelUnits.Parameter);
                }
                else if (UniqueData.InstrManufacture == 3)
                {
                    session.Write(":UNIT:POWer " + LevelUnits.Parameter);
                }
                res = true;
            }
            #region Exception
            catch (VisaException v_exp)
            {
                _logger.Exception(Contexts.ThisComponent, v_exp);
            }
            catch (Exception exp)
            {
                _logger.Exception(Contexts.ThisComponent, exp);
            }
            #endregion
            finally
            {
                //GetRefLevel();//хай будит потом разберусь                
            }
            return res;
        }
        /// <summary>
        /// Получаем Units 
        /// </summary>
        private void GetLevelUnit()
        {
            try
            {
                if (UniqueData.InstrManufacture == 1)
                {
                    string temp = session.Query(":UNIT:POWer?").TrimEnd();
                    for (int i = 0; i < UniqueData.LevelUnits.Count; i++)
                    {
                        if (temp.ToLower() == UniqueData.LevelUnits[i].Parameter.ToLower())
                        {
                            LevelUnits = UniqueData.LevelUnits[i];
                        }
                    }
                    //if (UniqueData.HiSpeed)
                    //{
                    //    for (int i = 0; i < UniqueData.LevelUnits.Count; i++)
                    //    {
                    //        if (LevelUnits[i].ind == 0) { LevelUnits[i].AnParameter = "DBM"; }
                    //        else if (LevelUnits[i].ind == 1) { LevelUnits[i].AnParameter = "DBMV"; }
                    //        else if (LevelUnits[i].ind == 2) { LevelUnits[i].AnParameter = "DBUV"; }
                    //        else if (LevelUnits[i].ind == 3) { LevelUnits[i].AnParameter = "DBUV/M"; }
                    //    }
                    //}
                    //else
                    //{
                    //    for (int i = 0; i < LevelUnits.Count(); i++)
                    //    {
                    //        if (LevelUnits[i].ind == 0) { LevelUnits[i].AnParameter = "DBM"; }
                    //        else if (LevelUnits[i].ind == 1) { LevelUnits[i].AnParameter = "DBMV"; }
                    //        else if (LevelUnits[i].ind == 2) { LevelUnits[i].AnParameter = "DBUV"; }
                    //        else if (LevelUnits[i].ind == 3) { LevelUnits[i].AnParameter = "DUVM"; }
                    //    }
                    //}

                    //string temp = session.Query(":UNIT:POWer?").TrimEnd();
                    //for (int i = 0; i < LevelUnits.Count(); i++)
                    //{
                    //    if (temp.ToLower() == LevelUnits[i].AnParameter.ToLower())
                    //    {
                    //        LevelUnitIndex = i;
                    //        if (LevelUnits[i].ind == 3) { LevelUnits[i].IsEnabled = true; }
                    //        else { LevelUnits[3].IsEnabled = false; }
                    //    }
                    //}
                }
                else if (UniqueData.InstrManufacture == 2)
                {
                    string temp = session.Query(":AMPL:UNIT?").TrimEnd();
                    for (int i = 0; i < UniqueData.LevelUnits.Count; i++)
                    {
                        if (temp.ToLower() == UniqueData.LevelUnits[i].Parameter.ToLower())
                        {
                            LevelUnits = UniqueData.LevelUnits[i];
                        }
                    }
                    //for (int i = 0; i < LevelUnits.Count(); i++)
                    //{
                    //    if (LevelUnits[i].ind == 0) { LevelUnits[i].AnParameter = "DBM"; }
                    //    else if (LevelUnits[i].ind == 1) { LevelUnits[i].AnParameter = "DBMV"; }
                    //    else if (LevelUnits[i].ind == 2) { LevelUnits[i].AnParameter = "DBUV"; }
                    //    else if (LevelUnits[i].ind == 3) { LevelUnits[i].IsEnabled = false; }
                    //}
                    //string temp = session.Query(":AMPL:UNIT?").TrimEnd();
                    //for (int i = 0; i < LevelUnits.Count(); i++)
                    //{
                    //    if (temp.ToLower() == LevelUnits[i].AnParameter.ToLower()) { LevelUnitIndex = i; }
                    //}
                }
                else if (UniqueData.InstrManufacture == 3)
                {
                    string temp = session.Query(":UNIT:POWer?").TrimEnd();
                    for (int i = 0; i < UniqueData.LevelUnits.Count; i++)
                    {
                        if (temp.ToLower() == UniqueData.LevelUnits[i].Parameter.ToLower())
                        {
                            LevelUnits = UniqueData.LevelUnits[i];
                        }
                    }
                    //for (int i = 0; i < LevelUnits.Count(); i++)
                    //{
                    //    if (LevelUnits[i].ind == 0) { LevelUnits[i].AnParameter = "dBm"; }
                    //    else if (LevelUnits[i].ind == 1) { LevelUnits[i].AnParameter = "dBmV"; }
                    //    else if (LevelUnits[i].ind == 2) { LevelUnits[i].AnParameter = "dBuV"; }
                    //    else if (LevelUnits[i].ind == 3) { LevelUnits[i].IsEnabled = false; }
                    //}
                    //string temp = session.Query(":UNIT:POWer?").TrimEnd();
                    //for (int i = 0; i < LevelUnits.Count(); i++)
                    //{
                    //    if (temp.ToLower() == LevelUnits[i].AnParameter.ToLower()) { LevelUnitIndex = i; }
                    //}
                }
            }
            #region Exception
            catch (VisaException v_exp)
            {
                _logger.Exception(Contexts.ThisComponent, v_exp);
            }
            catch (Exception exp)
            {
                _logger.Exception(Contexts.ThisComponent, exp);
            }
            #endregion            
        }

        /// <summary>
        /// Установка RBW 
        /// </summary>
        private bool SetRBW(decimal rbw)
        {
            bool res = false;
            try
            {
                RBW = rbw;
                if (UniqueData.InstrManufacture == 1)
                {
                    session.Write(":SENSe:BWIDth:RESolution " + RBW.ToString().Replace(',', '.'));
                }
                else if (UniqueData.InstrManufacture == 2)
                {
                    session.Write(":SENS:BAND:RES " + RBW.ToString().Replace(',', '.'));
                }
                else if (UniqueData.InstrManufacture == 3)
                {
                    session.Write(":SENSe:BWIDth:RESolution " + RBW.ToString().Replace(',', '.'));
                }
                res = true;
            }
            #region Exception
            catch (VisaException v_exp)
            {
                _logger.Exception(Contexts.ThisComponent, v_exp);
            }
            catch (Exception exp)
            {
                _logger.Exception(Contexts.ThisComponent, exp);
            }
            #endregion
            AutoRBW = false;
            if (AutoSweepTime == true) { GetSweepTime(); }
            if (AutoVBW == true) { GetVBW(); }
            GetSweepType();
            return res;
        }
        /// <summary>
        /// Получение RBW 
        /// </summary>
        private void GetRBW()
        {
            try
            {
                if (UniqueData.InstrManufacture == 1)
                {
                    RBW = DecimalParse(session.Query(":SENSe:BWIDth:RESolution?"));
                    RBWIndex = System.Array.IndexOf(UniqueData.RBWArr, RBW);
                }
                else if (UniqueData.InstrManufacture == 2)
                {
                    RBW = DecimalParse(session.Query(":SENS:BAND:RES?"));
                    RBWIndex = System.Array.IndexOf(UniqueData.RBWArr, RBW);
                }
                else if (UniqueData.InstrManufacture == 3)
                {
                    RBW = DecimalParse(session.Query(":SENSe:BWIDth:RESolution?"));
                    RBWIndex = System.Array.IndexOf(UniqueData.RBWArr, RBW);
                }
            }
            #region Exception
            catch (VisaException v_exp)
            {
                _logger.Exception(Contexts.ThisComponent, v_exp);
            }
            catch (Exception exp)
            {
                _logger.Exception(Contexts.ThisComponent, exp);
            }
            #endregion
            //if (AutoSWT == true) { GetSWT(); }
            //if (AutoVBW == true) { GetVBW(); }
        }
        /// <summary>
        /// Вкл/Выкл Auto RBW хай пока будит
        /// </summary>
        private void SetAutoRBW()
        {
            try
            {
                if (UniqueData.InstrManufacture == 1)
                {
                    if (AutoRBW == true) session.Write(":SENSe:BWIDth:RESolution:AUTO 1");
                    if (AutoRBW == false) session.Write(":SENSe:BWIDth:RESolution:AUTO 0");
                }
                else if (UniqueData.InstrManufacture == 2)
                {
                    if (AutoRBW == true) session.Write(":SENS:BWID:RES:AUTO 1");
                    if (AutoRBW == false) session.Write(":SENS:BWID:RES:AUTO 0");
                }
                else if (UniqueData.InstrManufacture == 3)
                {
                    if (AutoRBW == true) session.Write(":SENSe:BWIDth:RESolution:AUTO 1");
                    if (AutoRBW == false) session.Write(":SENSe:BWIDth:RESolution:AUTO 0");
                }
            }
            #region Exception
            catch (VisaException v_exp)
            {
                _logger.Exception(Contexts.ThisComponent, v_exp);
            }
            catch (Exception exp)
            {
                _logger.Exception(Contexts.ThisComponent, exp);
            }
            #endregion
            if (AutoSweepTime == true) { GetSweepTime(); }
            GetRBW();
            GetVBW();
            GetSweepType();
        }
        /// <summary>
        /// Получаем состояние Auto RBW 
        /// </summary>
        private void GetAutoRBW()
        {
            try
            {
                if (UniqueData.InstrManufacture == 1)
                {
                    string s = session.Query(":SENSe:BWIDth:RESolution:AUTO?").TrimEnd();
                    if (s.Contains("1")) { AutoRBW = true; }
                    else { AutoRBW = false; }
                }
                else if (UniqueData.InstrManufacture == 2)
                {
                    //    if (session.Query(":SENS:BWID:RES:AUTO?") == "0\n") { AutoRBW = false; }
                    //    else { AutoRBW = true; }
                }
                else if (UniqueData.InstrManufacture == 3)
                {
                    AutoRBW = Boolean.Parse(session.Query(":SENSe:BWIDth:RESolution:AUTO?"));
                }
            }
            #region Exception
            catch (VisaException v_exp)
            {
                _logger.Exception(Contexts.ThisComponent, v_exp);
            }
            catch (Exception exp)
            {
                _logger.Exception(Contexts.ThisComponent, exp);
            }
            #endregion
            if (AutoSweepTime == true) { GetSweepTime(); }
        }

        /// <summary>
        /// Установка VBW 
        /// </summary>
        private bool SetVBW(decimal vbw)
        {
            bool res = false;
            try
            {
                VBW = vbw;
                if (UniqueData.InstrManufacture == 1)
                {
                    session.Write(":SENSe:BANDwidth:VIDeo " + VBW.ToString());
                }
                else if (UniqueData.InstrManufacture == 2)
                {
                    session.Write(":SENSe:BANDwidth:VIDeo " + VBW.ToString().Replace(',', '.'));
                }
                else if (UniqueData.InstrManufacture == 3)
                {
                    session.Write(":SENSe:BANDwidth:VIDeo " + VBW.ToString());
                }
                res = true;
            }
            #region Exception
            catch (VisaException v_exp)
            {
                _logger.Exception(Contexts.ThisComponent, v_exp);
            }
            catch (Exception exp)
            {
                _logger.Exception(Contexts.ThisComponent, exp);
            }
            #endregion
            AutoVBW = false;
            if (AutoSweepTime == true && RBWIndex - VBWIndex > 0) { GetSweepTime(); }
            GetSweepType();
            return res;
        }
        /// <summary>
        /// Получение VBW 
        /// </summary>
        private void GetVBW()
        {
            try
            {
                if (UniqueData.InstrManufacture == 1)
                {
                    VBW = DecimalParse(session.Query(":SENSe:BANDwidth:VIDeo?"));
                    VBWIndex = System.Array.IndexOf(UniqueData.VBWArr, VBW);
                }
                else if (UniqueData.InstrManufacture == 2)
                {
                    VBW = DecimalParse(session.Query(":SENS:BAND:VID?"));
                    VBWIndex = System.Array.IndexOf(UniqueData.VBWArr, VBW);
                }
                else if (UniqueData.InstrManufacture == 3)
                {
                    VBW = DecimalParse(session.Query(":SENSe:BANDwidth:VIDeo?"));
                    VBWIndex = System.Array.IndexOf(UniqueData.VBWArr, VBW);
                }
            }
            #region Exception
            catch (VisaException v_exp)
            {
                _logger.Exception(Contexts.ThisComponent, v_exp);
            }
            catch (Exception exp)
            {
                _logger.Exception(Contexts.ThisComponent, exp);
            }
            #endregion
            //if (AutoSWT == true && RBWIndex - VBWIndex > 0) { GetSWT(); }
        }
        /// <summary>
        /// Вкл Auto RBW 
        /// </summary>
        private void SetAutoVBW()
        {
            try
            {
                if (UniqueData.InstrManufacture == 1)
                {
                    if (AutoVBW == true) session.Write(":SENSe:BANDwidth:VIDeo:AUTO 1");
                    if (AutoVBW == false) session.Write(":SENSe:BANDwidth:VIDeo:AUTO 0");
                }
                else if (UniqueData.InstrManufacture == 2)
                {
                    if (AutoVBW == true) session.Write(":SENS:BAND:VID:AUTO 1");
                    if (AutoVBW == false) session.Write(":SENS:BAND:VID:AUTO 0");
                }
                else if (UniqueData.InstrManufacture == 3)
                {
                    if (AutoVBW == true) session.Write(":SENSe:BWIDth:VIDeo:AUTO 1");
                    if (AutoVBW == false) session.Write(":SENSe:BWIDth:VIDeo:AUTO 0");
                }
            }
            #region Exception
            catch (VisaException v_exp)
            {
                _logger.Exception(Contexts.ThisComponent, v_exp);
            }
            catch (Exception exp)
            {
                _logger.Exception(Contexts.ThisComponent, exp);
            }
            #endregion
            if (AutoVBW == true) { GetVBW(); }
            if (AutoSweepTime == true) { GetSweepTime(); }
            GetSweepType();
            //:SENSe:BANDwidth|BWIDth:VIDeo:AUTO? 1
            //:SENSe:BWIDth:VIDeo:RATio? 
        }
        /// <summary>
        /// Получаем состояние Auto VBW 
        /// </summary>
        private void GetAutoVBW()
        {
            try
            {
                if (UniqueData.InstrManufacture == 1)
                {
                    if (session.Query(":SENSe:BANDwidth:VIDeo:AUTO?").Contains("1")) { AutoVBW = true; }
                    else { AutoVBW = false; }
                }
                else if (UniqueData.InstrManufacture == 2)
                {
                    if (session.Query(":SENS:BAND:VID:AUTO?") == "0\n") { AutoVBW = false; }
                    else { AutoVBW = true; }
                }
                else if (UniqueData.InstrManufacture == 3)
                {
                    AutoVBW = Boolean.Parse(session.Query(":SENSe:BWIDth:VIDeo:AUTO?"));
                }
            }
            #region Exception
            catch (VisaException v_exp)
            {
                _logger.Exception(Contexts.ThisComponent, v_exp);
            }
            catch (Exception exp)
            {
                _logger.Exception(Contexts.ThisComponent, exp);
            }
            #endregion
            //if (AutoSWT == true) { GetSWT(); }
        }


        /// <summary>
        /// Установка метода свипирования 
        /// </summary>        
        private void SetSweepType()
        {
            try
            {
                if (UniqueData.InstrManufacture == 1)
                {
                    session.Write(":SWE:TYPE " + SweepTypeSelected.Parameter);
                    //if (SweepType == 0) { session.Write(":SWE:TYPE Auto"); SweepTypeStr = "Auto"; }
                    //else if (SweepType == 1) { session.Write(":SWE:TYPE Sweep"); SweepTypeStr = "Sweep"; }
                    //else if (SweepType == 2) { session.Write(":SWE:TYPE FFT"); SweepTypeStr = "FFT"; }
                }
                else if (UniqueData.InstrManufacture == 2)
                {
                    session.Write(":SWE:TYPE " + SweepTypeSelected.Parameter);
                    //if (SweepType == 0) { session.Write(":SWE:TYPE AUTO"); SweepTypeStr = "Auto"; }
                    //else if (SweepType == 1) { session.Write(":SWE:TYPE STEP"); SweepTypeStr = "STEP"; }
                    //else if (SweepType == 2) { session.Write(":SWE:TYPE FFT"); SweepTypeStr = "FFT"; }
                }
                else if (UniqueData.InstrManufacture == 3)
                {
                    session.Write(":SWE:MODE " + SweepTypeSelected.Parameter);
                }
            }
            #region Exception
            catch (VisaException v_exp)
            {
                _logger.Exception(Contexts.ThisComponent, v_exp);
            }
            catch (Exception exp)
            {
                _logger.Exception(Contexts.ThisComponent, exp);
            }
            #endregion
        }
        /// <summary>
        /// Получаем метод свиптайма 
        /// </summary> 
        private void GetSweepType()
        {
            try
            {
                //if (UniqueData.InstrManufacture == 1)
                //{
                //    if (UniqueData.HiSpeed == true)
                //    {
                //        string t = session.Query(":SWE:TYPE?");
                //        foreach (SwpType ST in UniqueData.SweepType)
                //        {
                //            if (t.TrimEnd().ToLower() == ST.Parameter.ToLower()) { SweepTypeSelected = ST; }
                //        }
                //    }
                //    else { SweepTypeSelected = new SwpType { UI = "FFT", Parameter = "FFT" }; }
                //}
                //else if (UniqueData.InstrManufacture == 2)
                //{
                //    string temp = session.Query(":SWE:TYPE?");
                //    foreach (SwpType ST in UniqueData.SweepType)
                //    {
                //        if (temp.Contains(ST.Parameter)) { SweepTypeSelected = ST; }
                //    }
                //}
                //else if (UniqueData.InstrManufacture == 3)
                //{
                //    string temp = session.Query(":SWE:MODE?");
                //    foreach (SwpType ST in UniqueData.SweepType)
                //    {
                //        if (temp.Contains(ST.Parameter)) { SweepTypeSelected = ST; }
                //    }
                //}
            }
            #region Exception
            catch (VisaException v_exp)
            {
                _logger.Exception(Contexts.ThisComponent, v_exp);
            }
            catch (Exception exp)
            {
                _logger.Exception(Contexts.ThisComponent, exp);
            }
            #endregion
        }
        /// <summary>
        /// Получаем свиптайм 
        /// </summary>
        private void GetSweepTime()
        {
            try
            {
                if (UniqueData.InstrManufacture == 1)
                {
                    SweepTime = DecimalParse(session.Query(":SWE:TIME?"));
                }
                else if (UniqueData.InstrManufacture == 2)
                {
                    SweepTime = DecimalParse(session.Query(":SWE:MTIM?"));
                }
                else if (UniqueData.InstrManufacture == 3)
                {
                    SweepTime = DecimalParse(session.Query(":SWE:TIME:ACT?"));
                }
            }
            #region Exception
            catch (VisaException v_exp)
            {
                _logger.Exception(Contexts.ThisComponent, v_exp);
            }
            catch (Exception exp)
            {
                _logger.Exception(Contexts.ThisComponent, exp);
            }
            #endregion
        }
        private bool SetSweepTime(decimal sweeptime)
        {
            bool res = false;
            try
            {
                SweepTime = sweeptime;
                if (UniqueData.InstrManufacture == 1)
                {
                    session.Write(":SWE:TIME " + SweepTime.ToString());
                    SweepTime = DecimalParse(session.Query(":SWE:TIME?"));
                    AutoSweepTime = false;
                }
                else if (UniqueData.InstrManufacture == 2)
                {
                    session.Write(":SWE:TIME " + SweepTime.ToString().Replace(',', '.'));
                    SweepTime = DecimalParse(session.Query(":SWE:TIME?"));
                    AutoSweepTime = false;
                }
                else if (UniqueData.InstrManufacture == 3)
                {
                    session.Write(":SWE:TIME:ACT " + SweepTime.ToString().Replace(',', '.'));
                    SweepTime = DecimalParse(session.Query(":SWE:TIME:ACT?"));
                    AutoSweepTime = false;
                }
                res = true;
            }
            #region Exception
            catch (VisaException v_exp)
            {
                _logger.Exception(Contexts.ThisComponent, v_exp);
            }
            catch (Exception exp)
            {
                _logger.Exception(Contexts.ThisComponent, exp);
            }
            #endregion
            return res;
        }
        private bool SetAutoSweepTime(bool autosweeptime)
        {
            bool res = false;
            try
            {
                AutoSweepTime = autosweeptime;
                if (UniqueData.InstrManufacture == 1)
                {
                    if (AutoSweepTime == true) session.Write(":SWE:TIME:AUTO 1");
                    else session.Write(":SWE:TIME:AUTO 0");
                }
                else if (UniqueData.InstrManufacture == 2)
                {
                    if (AutoSweepTime == true) session.Write(":SENS:SWE:ACQ:AUTO 1");
                    else session.Write(":SENS:SWE:ACQ:AUTO 0");
                }
                else if (UniqueData.InstrManufacture == 3)
                {
                    if (AutoSweepTime == true) session.Write(":SENS:SWE:AUTO ON");
                    else session.Write(":SENS:SWE:AUTO OFF");
                }
                res = true;
            }
            #region Exception
            catch (VisaException v_exp)
            {
                _logger.Exception(Contexts.ThisComponent, v_exp);
            }
            catch (Exception exp)
            {
                _logger.Exception(Contexts.ThisComponent, exp);
            }
            #endregion
            GetSweepTime();
            return res;
        }
        /// <summary>
        /// Получаем состояние Auto SWT 
        /// </summary>
        private void GetAutoSweepTime()
        {
            try
            {
                if (UniqueData.InstrManufacture == 1)
                {
                    if (session.Query(":SWE:TIME:AUTO?") == "0\n") { AutoSweepTime = false; }
                    else { AutoSweepTime = true; }
                }
                else if (UniqueData.InstrManufacture == 2)
                {
                    if (session.Query(":SENS:SWE:ACQ:AUTO?") == "0\n") { AutoSweepTime = false; }
                    else { AutoSweepTime = true; }
                }
            }
            #region Exception
            catch (VisaException v_exp)
            {
                _logger.Exception(Contexts.ThisComponent, v_exp);
            }
            catch (Exception exp)
            {
                _logger.Exception(Contexts.ThisComponent, exp);
            }
            #endregion
        }

        private bool SetSweepPoints(int sweeppoints)
        {
            bool res = false;
            try
            {
                SweepPoints = sweeppoints;
                if (UniqueData.InstrManufacture == 1)
                {
                    session.Write(":SWE:POIN " + SweepPoints.ToString());
                }
                else if (UniqueData.InstrManufacture == 2)
                {
                    session.Write(":SENS:SWE:POIN " + SweepPoints.ToString());
                }
                session.DefaultBufferSize = SweepPoints * 4 + SweepPoints.ToString().Length + 100;
                res = true;
            }
            #region Exception
            catch (VisaException v_exp)
            {
                _logger.Exception(Contexts.ThisComponent, v_exp);
            }
            catch (Exception exp)
            {
                _logger.Exception(Contexts.ThisComponent, exp);
            }
            #endregion
            GetFreqArr();
            return res;
        }


        /// <summary>
        /// Получаем количевства точек свипов 
        /// </summary>
        private void GetSweepPoints()
        {
            try
            {
                if (UniqueData.InstrManufacture == 1)
                {
                    if (UniqueData.SweepPointFix == false)
                    {
                        SweepPoints = int.Parse(session.Query(":SWE:POIN?").Replace('.', ','));
                        SweepPointsIndex = System.Array.IndexOf(UniqueData.SweepPointArr, SweepPoints);
                    }
                    if (UniqueData.HiSpeed == true) { TracePoints = SweepPoints + 2; }
                    if (UniqueData.HiSpeed == false) { TracePoints = SweepPoints; }
                }
                else if (UniqueData.InstrManufacture == 2)
                {
                    SweepPoints = int.Parse(session.Query(":SENS:SWE:POIN?").Replace('.', ','));
                    SweepPointsIndex = System.Array.IndexOf(UniqueData.SweepPointArr, SweepPoints);
                    session.DefaultBufferSize = SweepPoints * 4 + 20;
                    TracePoints = SweepPoints;
                }
            }
            #region Exception
            catch (VisaException v_exp)
            {
                _logger.Exception(Contexts.ThisComponent, v_exp);
            }
            catch (Exception exp)
            {
                _logger.Exception(Contexts.ThisComponent, exp);
            }
            #endregion
            GetFreqArr();
        }



        private bool SetOptimization(EN.Optimization optimization)
        {
            bool res = false;
            try
            {
                if (UniqueData.OptimizationAvailable)
                {
                    Optimization = optimization;
                    if (UniqueData.InstrManufacture == 1)
                    {
                        session.Write("SENSe:SWEep:OPTimize " + OptimizationSelected.Parameter);
                    }
                    else if (UniqueData.InstrManufacture == 2)
                    {

                    }
                }
                res = true;
            }
            #region Exception
            catch (VisaException v_exp)
            {
                _logger.Exception(Contexts.ThisComponent, v_exp);
            }
            catch (Exception exp)
            {
                _logger.Exception(Contexts.ThisComponent, exp);
            }
            #endregion
            return res;
        }
        private void GetOptimization()
        {
            try
            {
                if (UniqueData.OptimizationAvailable)
                {
                    if (UniqueData.InstrManufacture == 1)
                    {
                        string temp1 = string.Empty;
                        temp1 = session.Query("SENSe:SWEep:OPTimize?");
                        foreach (ParamWithId TT in UniqueData.Optimization)
                        {
                            if (temp1.Contains(TT.Parameter))
                            {
                                Optimization = (EN.Optimization)TT.Id;
                            }
                        }
                    }
                    else if (UniqueData.InstrManufacture == 2)
                    {

                    }
                    else if (UniqueData.InstrManufacture == 3)
                    {

                    }
                }
            }
            #region Exception
            catch (VisaException v_exp)
            {
                _logger.Exception(Contexts.ThisComponent, v_exp);
            }
            catch (Exception exp)
            {
                _logger.Exception(Contexts.ThisComponent, exp);
            }
            #endregion
        }

        private bool SetTraceType(ParamWithId trace1type)
        {
            bool res = false;
            try
            {
                Trace1Type = trace1type;
                if (UniqueData.InstrManufacture == 1)
                {
                    session.Write(":DISP:TRAC1:MODE " + Trace1Type.Parameter);
                }
                else if (UniqueData.InstrManufacture == 2)
                {
                    session.Write(":TRAC1:TYPE " + Trace1Type.Parameter);
                }
                else if (UniqueData.InstrManufacture == 3)
                {
                    session.Write(":TRACe1:OPERation " + Trace1Type.Parameter);
                }
                res = true;
            }
            #region Exception
            catch (VisaException v_exp)
            {
                _logger.Exception(Contexts.ThisComponent, v_exp);
            }
            catch (Exception exp)
            {
                _logger.Exception(Contexts.ThisComponent, exp);
            }
            #endregion
            return res;
        }
        private void GetTraceType()
        {
            try
            {
                if (UniqueData.InstrManufacture == 1)
                {
                    string temp1 = string.Empty;
                    if (UniqueData.HiSpeed == true)
                    {
                        if (session.Query(":DISP:TRAC1?").Contains("1")) { temp1 = session.Query(":DISP:TRAC1:MODE?"); }
                        else { Trace1Type = new ParamWithId { Id = (int)EN.TraceType.Blank, Parameter = "BLAN" }; }
                    }
                    else if (UniqueData.HiSpeed == false)
                    {
                        temp1 = session.Query(":DISP:TRAC1:MODE?");
                    }
                    foreach (ParamWithId TT in UniqueData.TraceType)
                    {
                        if (temp1.Contains(TT.Parameter)) { Trace1Type = TT; }
                    }
                }
                else if (UniqueData.InstrManufacture == 2)
                {
                    string temp1 = session.Query(":TRACe1:TYPE?");
                    foreach (ParamWithId TT in UniqueData.TraceType)
                    {
                        if (temp1.Contains(TT.Parameter)) { Trace1Type = TT; }
                    }
                }
                else if (UniqueData.InstrManufacture == 3)
                {
                    string temp1 = session.Query(":TRACe1:OPERation?");
                    foreach (ParamWithId TT in UniqueData.TraceType)
                    {
                        if (temp1.Contains(TT.Parameter)) { Trace1Type = TT; }
                    }
                }
            }
            #region Exception
            catch (VisaException v_exp)
            {
                _logger.Exception(Contexts.ThisComponent, v_exp);
            }
            catch (Exception exp)
            {
                _logger.Exception(Contexts.ThisComponent, exp);
            }
            #endregion
        }
        private bool SetDetectorType(ParamWithId trace1detector)
        {
            bool res = false;
            try
            {
                Trace1Detector = trace1detector;
                if (UniqueData.InstrManufacture == 1)
                {
                    if (Trace1Detector.Parameter == "Auto Select") session.Write(":SENSe:DETector1:AUTO 1");
                    else
                    {
                        session.Write(":SENSe:DETector1 " + Trace1Detector.Parameter);
                    }

                }
                else if (UniqueData.InstrManufacture == 2)
                {
                    session.Write(":SENSe:DETector:FUNCtion " + Trace1Detector.Parameter);
                }
                else if (UniqueData.InstrManufacture == 3)
                {
                    if (Trace1Detector.Parameter == "Auto Select") session.Write(":SENSe:DETector1:AUTO 1");
                    else
                    {
                        session.Write(":SENSe:DETector:FUNCtion " + Trace1Detector.Parameter);
                    }
                }
                res = true;
            }
            #region Exception
            catch (VisaException v_exp)
            {
                _logger.Exception(Contexts.ThisComponent, v_exp);
            }
            catch (Exception exp)
            {
                _logger.Exception(Contexts.ThisComponent, exp);
            }
            #endregion

            return res;
        }
        /// <summary>
        /// Получение типа Trace Detecror 
        /// </summary>
        private void GetDetectorType()
        {
            try
            {
                if (UniqueData.InstrManufacture == 1)
                {

                    string temp1 = string.Empty;
                    if (UniqueData.HiSpeed == true)
                    {
                        temp1 = session.Query(":SENSe:DETector1?").TrimEnd();
                    }
                    else if (UniqueData.HiSpeed == false)
                    {
                        temp1 = session.Query(":SENSe:DETector1?").TrimEnd();
                    }
                    foreach (ParamWithId TT in UniqueData.TraceDetector)
                    {
                        if (temp1.Contains(TT.Parameter)) { Trace1Detector = TT; }
                    }
                }
                else if (UniqueData.InstrManufacture == 2)
                {
                    string temp1 = session.Query(":SENSe:DETector:FUNCtion?").TrimEnd();
                    foreach (ParamWithId TT in UniqueData.TraceDetector)
                    {
                        if (temp1.Contains(TT.Parameter)) { Trace1Detector = TT; }
                    }
                }
                else if (UniqueData.InstrManufacture == 3)
                {
                    string temp1 = session.Query(":SENSe:DETector:FUNCtion?").TrimEnd();
                    foreach (ParamWithId TT in UniqueData.TraceDetector)
                    {
                        if (temp1.Contains(TT.Parameter)) { Trace1Detector = TT; }
                    }
                }
            }
            #region Exception
            catch (VisaException v_exp)
            {
                _logger.Exception(Contexts.ThisComponent, v_exp);
            }
            catch (Exception exp)
            {
                _logger.Exception(Contexts.ThisComponent, exp);
            }
            #endregion
        }




        private bool SetIQBW(decimal iqbw)
        {
            bool res = false;
            try
            {
                IQBW = iqbw;
                if (UniqueData.InstrManufacture == 1)
                {
                    SampleSpeed = 1.25m * IQBW;
                    SampleTimeLength = 1 / SampleSpeed;
                    string s = $"TRAC:IQ:BWID " + IQBW.ToString().Replace(',', '.');
                    session.Write(s);
                }
                else if (UniqueData.InstrManufacture == 2)
                {

                }
                else if (UniqueData.InstrManufacture == 3)
                {

                }
                res = true;
            }
            #region Exception
            catch (VisaException v_exp)
            {
                _logger.Exception(Contexts.ThisComponent, v_exp);
            }
            catch (Exception exp)
            {
                _logger.Exception(Contexts.ThisComponent, exp);
            }
            #endregion            
            return res;
        }
        private bool SetSampleSpeed(decimal samplespeed)
        {
            bool res = false;
            try
            {
                SampleSpeed = samplespeed;
                if (UniqueData.InstrManufacture == 1)
                {
                    IQBW = 0.8m * SampleSpeed;
                    SampleTimeLength = 1 / SampleSpeed;
                    session.Write($"TRAC:IQ:SRAT " + SampleSpeed.ToString().Replace(',', '.'));
                }
                else if (UniqueData.InstrManufacture == 2)
                {
                }
                else if (UniqueData.InstrManufacture == 3)
                {
                }
                res = true;
            }
            #region Exception
            catch (VisaException v_exp)
            {
                _logger.Exception(Contexts.ThisComponent, v_exp);
            }
            catch (Exception exp)
            {
                _logger.Exception(Contexts.ThisComponent, exp);
            }
            #endregion            
            return res;
        }
        private bool SetSampleLength(int samplelength)
        {
            bool res = false;
            try
            {
                SampleLength = samplelength;
                if (UniqueData.InstrManufacture == 1)
                {
                    session.Write($"TRAC:IQ:RLEN " + SampleLength.ToString());
                }
                else if (UniqueData.InstrManufacture == 2)
                {
                }
                else if (UniqueData.InstrManufacture == 3)
                {
                }
                res = true;
            }
            #region Exception
            catch (VisaException v_exp)
            {
                _logger.Exception(Contexts.ThisComponent, v_exp);
            }
            catch (Exception exp)
            {
                _logger.Exception(Contexts.ThisComponent, exp);
            }
            #endregion            
            return res;
        }

        private bool SetTriggerOffset(decimal triggeroffset)
        {
            bool res = false;
            try
            {
                TriggerOffset = triggeroffset;
                if (UniqueData.InstrManufacture == 1)
                {
                    if (Math.Abs(TriggerOffset) < UniqueData.TriggerOffsetMax)
                    {
                        session.Write($"TRIG:HOLD " + TriggerOffset.ToString().Replace(',', '.'));
                    }
                }
                else if (UniqueData.InstrManufacture == 2)
                {
                }
                else if (UniqueData.InstrManufacture == 3)
                {
                }
                res = true;
            }
            #region Exception
            catch (VisaException v_exp)
            {
                _logger.Exception(Contexts.ThisComponent, v_exp);
            }
            catch (Exception exp)
            {
                _logger.Exception(Contexts.ThisComponent, exp);
            }
            #endregion            
            return res;
        }
        private bool SetTriggerOffsetAndSampleLength(decimal triggeroffset, int samplelength)
        {
            bool res = false;
            try
            {
                TriggerOffset = triggeroffset;
                SampleLength = samplelength;
                if (UniqueData.InstrManufacture == 1)
                {
                    if (Math.Abs(TriggerOffset) < UniqueData.TriggerOffsetMax)
                    {
                        session.Write("TRIG:HOLD " + TriggerOffset.ToString().Replace(',', '.') + ";:TRAC:IQ:RLEN " + SampleLength.ToString());
                    }
                }
                else if (UniqueData.InstrManufacture == 2)
                {
                }
                else if (UniqueData.InstrManufacture == 3)
                {
                }
                res = true;
            }
            #region Exception
            catch (VisaException v_exp)
            {
                _logger.Exception(Contexts.ThisComponent, v_exp);
            }
            catch (Exception exp)
            {
                _logger.Exception(Contexts.ThisComponent, exp);
            }
            #endregion            
            return res;
        }

        private bool GetTrace()
        {
            bool res = true;
            try
            {
                bool newdata = true;
                float[] temp = new float[0] { };
                if (UniqueData.InstrManufacture == 1)
                {
                    if (UniqueData.HiSpeed == true)
                    {
                        //string sss = session.Query("TRAC1:X? TRACE1");
                        session.Write(":STAT:QUES:POW?;:TRAC:DATA? TRACE1");
                        byte[] byteArray = session.ReadByteArray();
                        int tracedataoffset = 0;
                        for (int i = 0; i < 10; i++)
                        {
                            if (byteArray[i] == 59)
                            {
                                tracedataoffset = i + 1;
                                break;
                            }
                        }
                        int pr = int.Parse(System.Text.Encoding.ASCII.GetString(byteArray, 0, 1));
                        if (pr == 0 || pr == 2) { PowerRegister = EN.PowerRegister.Normal; }
                        else if (pr == 4) { PowerRegister = EN.PowerRegister.IFOverload; }
                        else if (pr == 1) { PowerRegister = EN.PowerRegister.RFOverload; }//правильно
                        if (System.Text.Encoding.ASCII.GetString(byteArray, tracedataoffset, 1) == "#")
                        {
                            int lengthPreamb = int.Parse(System.Text.Encoding.ASCII.GetString(byteArray, tracedataoffset + 1, 1));
                            int lengthData = int.Parse(System.Text.Encoding.ASCII.GetString(byteArray, tracedataoffset + 2, lengthPreamb));
                            temp = new float[lengthData / 4 + 2];

                            for (int j = 0; j < lengthData / 4; j++)
                            {
                                temp[j + 1] = System.BitConverter.ToSingle(byteArray, tracedataoffset + lengthPreamb + 2 + j * 4);
                            }
                            temp[0] = temp[1];
                            temp[temp.Length - 1] = temp[temp.Length - 2];
                        }
                    }
                    else
                    {
                        session.Write("TRAC:DATA? TRACE1");
                        byte[] byteArray = session.ReadByteArray();
                        if (System.Text.Encoding.ASCII.GetString(byteArray, 0, 1) == "#")
                        {
                            int lengthPreamb = int.Parse(System.Text.Encoding.ASCII.GetString(byteArray, 1, 1));
                            int lengthData = int.Parse(System.Text.Encoding.ASCII.GetString(byteArray, 2, lengthPreamb));
                            temp = new float[lengthData / 4];

                            for (int j = 0; j < temp.Length; j++)
                            {
                                temp[j] = System.BitConverter.ToSingle(byteArray, lengthPreamb + 2 + j * 4);
                            }
                        }
                    }
                }
                else if (UniqueData.InstrManufacture == 2)
                {
                    session.Write("TRAC1:DATA?");
                    byte[] byteArray = session.ReadByteArray();
                    if (System.Text.Encoding.ASCII.GetString(byteArray, 0, 1) == "#")
                    {
                        int lengthPreamb = int.Parse(System.Text.Encoding.ASCII.GetString(byteArray, 1, 1));
                        int lengthData = int.Parse(System.Text.Encoding.ASCII.GetString(byteArray, 2, lengthPreamb));
                        temp = new float[lengthData / 4];

                        for (int j = 0; j < temp.Length; j++)
                        {
                            temp[j] = System.BitConverter.ToSingle(byteArray, lengthPreamb + 2 + j * 4);
                        }
                    }
                }
                else if (UniqueData.InstrManufacture == 3)
                {
                    session.Write("TRAC1:DATA?");
                    byte[] byteArray = session.ReadByteArray();
                    if (System.Text.Encoding.ASCII.GetString(byteArray, 0, 1) == "#")
                    {
                        int lengthPreamb = int.Parse(System.Text.Encoding.ASCII.GetString(byteArray, 1, 1));
                        int lengthData = int.Parse(System.Text.Encoding.ASCII.GetString(byteArray, 2, lengthPreamb));
                        temp = new float[lengthData / 4];

                        for (int j = 0; j < temp.Length; j++)
                        {
                            temp[j] = System.BitConverter.ToSingle(byteArray, lengthPreamb + 2 + j * 4);
                        }
                    }
                }

                if (temp.Length > 0)
                {
                    //т.к. используем только ClearWhrite то проверяем полученный трейс с предыдущим временным на предмет полного отличия и полноценен он или нет
                    for (int i = 0; i < temp.Length; i++)
                    {
                        if (LevelArrTemp[i] == temp[i] || float.IsNaN(temp[i]) || temp[i] == -200 || temp[i] == -145)
                        {
                            newdata = false;
                            break;
                        }
                    }
                    //таки новый трейс полностью
                    if (newdata)
                    {
                        SetTraceData(temp);
                    }
                    else
                    {
                        res = false;
                    }
                }
                else
                {
                    res = false;
                }
            }
            #region Exception
            catch (VisaException v_exp)
            {
                res = false;
                _logger.Exception(Contexts.ThisComponent, v_exp);
            }
            catch (Exception exp)
            {
                res = false;
                _logger.Exception(Contexts.ThisComponent, exp);
            }
            #endregion
            return res;
        }
        private void SetTraceData(float[] trace)
        {
            if (trace != null && trace.Length > 0)
            {
                if (LevelArr.Length != trace.Length)
                {
                    LevelArr = new float[trace.Length];
                    for (int i = 0; i < trace.Length; i++)
                    {
                        LevelArr[i] = trace[i];
                    }
                }
                if (TraceTypeResult == EN.TraceType.ClearWrite)
                {
                    LevelArr = trace;
                }
                else if (TraceTypeResult == EN.TraceType.Average)//Average
                {

                    if (TraceReset) { TraceAveraged.Reset(); TraceReset = false; }
                    TraceAveraged.AddTraceToAverade(FreqArr, trace, (MEN.LevelUnit)LevelUnits.Id, LevelUnitsResult);
                    LevelArr = TraceAveraged.AveragedLevels;

                }
                else if (TraceTypeResult == EN.TraceType.MaxHold)
                {
                    if (TraceReset == false)
                    {
                        for (int i = 0; i < trace.Length; i++)
                        {
                            if (trace[i] > LevelArr[i]) LevelArr[i] = trace[i];
                        }
                    }
                    else
                    {
                        LevelArr = trace;
                        TraceReset = false;
                    }
                }
                else if (TraceTypeResult == EN.TraceType.MinHold)
                {
                    if (TraceReset == false)
                    {
                        for (int i = 0; i < trace.Length; i++)
                        {
                            if (trace[i] < LevelArr[i]) LevelArr[i] = trace[i];
                        }
                    }
                    else
                    {
                        LevelArr = trace;
                        TraceReset = false;
                    }
                }
                LevelArrTemp = trace;
            }
        }


        private bool GetIQStream(ref COMR.MesureIQStreamResult result, decimal meastime, int sample, COM.MesureIQStreamCommand command)
        {
            bool res = false;
            if (UniqueData.InstrManufacture == 1)
            {
                //ели надо изменяем размер буфера
                int length = (SampleLength * 4) * 2 + SampleLength.ToString().Length + 100;
                if (session.DefaultBufferSize != length)
                {
                    session.DefaultBufferSize = length;
                }

                session.Write("INIT;*WAI;");
                int sleep = (int)(meastime * 1000 - 250);
                if (sleep < 1)
                {
                    sleep = 1;
                }
                Thread.Sleep(sleep);

                int step = sample / 10;
                if (step > 50000)
                {
                    step = 50000;
                }
                else if (step < 10000)
                {
                    step = 1000;
                }

                float[] temp = new float[sample * 2];
                long ddd = _timeService.TimeStamp.Ticks;
                int pos = 0;
                for (int i = 0; i < sample; i += step)
                {
                    int from = i, to = i + step;
                    if (to > sample) to = sample;
                    session.Write($"TRAC:IQ:DATA:MEM? {from},{to - from}");
                    byte[] byteArray = session.ReadByteArray();
                    if (System.Text.Encoding.ASCII.GetString(byteArray, 0, 1) == "#")
                    {
                        int lengthPreamb = int.Parse(System.Text.Encoding.ASCII.GetString(byteArray, 1, 1));
                        int lengthData = int.Parse(System.Text.Encoding.ASCII.GetString(byteArray, 2, lengthPreamb));
                        float[] temp2 = new float[lengthData / 4];

                        for (int j = 0; j < temp2.Length; j++)
                        {
                            temp2[j] = System.BitConverter.ToSingle(byteArray, lengthPreamb + 2 + j * 4);
                        }
                        //Debug.WriteLine(temp2.Length);
                        Array.Copy(temp2, 0, temp, pos, temp2.Length);
                        pos += temp2.Length;

                    }
                }
                string dfghkjdp = session.Query("TRACe:IQ:TPISample?").TrimEnd();
                TriggerOffsetInSample = DecimalParse(dfghkjdp);
                //Посчитаем когда точно был триггер относительно первого семпла
                TriggerOffset = Math.Abs(TriggerOffset) + TriggerOffsetInSample;
                Debug.WriteLine(TriggerOffset);
                Debug.WriteLine("\r\n" + new TimeSpan(command.Parameter.TimeStart).ToString());
                IQArr = temp;
                result.iq_samples = new float[1][];
                result.iq_samples[0] = temp;
                result.OneSempleDuration_ns = (long)(SampleTimeLength / 1000000000);
                //result.TimeStamp = ;
                //result.TimeStamp = tempIQStream.BlockTime[IQStartIndex] / 100;// надыбать время первого семпла
                //result.PPSTimeDifference_ns = TimeToStartBlockWithPPS;// когда был ппс точно относительно первого семпла

            }

            res = true;

            return res;
        }
        #endregion

        #region AN Method



        public void SetIQWindow()
        {
            try
            {
                if (UniqueData.InstrManufacture == 1)
                {
                    string str = "INST:CRE IQ, 'IQ Analyzer'";
                    session.Write(str);
                    session.Write("INIT:CONT OFF");
                    session.Write("TRIG:SOUR EXT");
                    session.Write("TRACe:IQ:DATA:FORMat IQPair");
                }
            }
            #region Exception
            catch (VisaException v_exp)
            {
                _logger.Exception(Contexts.ThisComponent, v_exp);
            }
            catch (Exception exp)
            {
                _logger.Exception(Contexts.ThisComponent, exp);
            }
            #endregion
        }

        public void SetWindowType(EN.Mode mode)
        {
            try
            {
                Mode = mode;
                if (UniqueData.InstrManufacture == 1)
                {
                    string str = ""; //MAGN                    
                    if (Mode == EN.Mode.IQAnalyzer)
                    {
                        str = "INST IQ";// "INST:CRE:REPL 'Spectrum',IQ,'IQAnalyzer'";//SANALYZER   IQANALYZER
                    }
                    else if (Mode == EN.Mode.SpectrumAnalyzer)
                    {
                        str = "INST SAN";// "INST:CRE:REPL 'IQAnalyzer',SANALYZER,'Spectrum'";//SANALYZER   IQANALYZER
                        session.DefaultBufferSize = SweepPoints * 18 + 25;
                    }
                    session.Write(str);
                    session.Write("FORM:DATA REAL,32");//так надо
                    //string res = session.Query(str);
                }
            }
            #region Exception
            catch (VisaException v_exp)
            {
                _logger.Exception(Contexts.ThisComponent, v_exp);
            }
            catch (Exception exp)
            {
                _logger.Exception(Contexts.ThisComponent, exp);
            }
            #endregion
        }
        public float[] IQArr = new float[] { -1, -1, -1, -1 };

        public string TriggerOfset = "";

        #region old IQ
        //public void GetIQ()
        //{
        //    int index = 0;
        //    int pos = 0;
        //    try
        //    {

        //        if (UniqueData.InstrManufacture == 1)
        //        {
        //            //FreqCentr = 935.20m * 1000000;//424.650m * 1000000;//
        //            //session.Write($"FREQ:CENT {FreqCentr.ToString().Replace(",", ".")}");
        //            session.Write("INIT:CONT OFF");
        //            session.Write("TRIG:HOLD -0.01");
        //            session.Write("TRACe:IQ:DATA:FORMat IQPair");

        //            #region тригер по iq
        //            //double trigger = -90;
        //            //session.Write($"TRIG:LEV:IQP {trigger.ToString().Replace(", ", ".")}");
        //            #endregion
        //            #region тригер по EXT
        //            //только этот тригер т.к. нужен PPS
        //            session.Write("TRIG:SOUR EXT");
        //            #endregion
        //            string str = "";
        //            int ABW = 271 * 1000;
        //            int Speed = (int)(1.25 * ABW);

        //            double meastime = 0.1;
        //            //if (Speed * meastime > 25 * 1000000)
        //            //{
        //            //    meastime = 25 * 1000000 / Speed;
        //            //}
        //            //session.Write($"SWEep:TIME {meastime.ToString().Replace(",", ".")}");

        //            int sample = (int)(Speed * meastime);
        //            //str = $"TRAC:IQ:SRAT {Speed}";
        //            //session.Write(str);

        //            //str = $"TRAC:IQ:RLEN {sample}";
        //            //session.Write(str);

        //            //string sss = session.Query("TRAC:IQ:BWID?");

        //            str = "INIT;*WAI;";// "INST:CRE:REPL 'IQAnalyzer',SANALYZER,'Spectrum'";//SANALYZER   IQANALYZER
        //            session.Write(str);
        //            session.DefaultBufferSize = (sample * 4) * 2 + sample.ToString().Length + 100;
        //            //нетрогать так надо, т.к. данные собираются на приборе и пока собираются ничего не ответит
        //            //Если спросить раньше то VISA может послать в таймаут задержки
        //            //_timeService.TimeStamp. 
        //            int sleep = (int)(meastime * 1000 - 250);
        //            if (sleep < 1)
        //            {
        //                sleep = 1;
        //            }
        //            Thread.Sleep(sleep);

        //            int step = sample / 10;
        //            if (step > 50000)
        //            {
        //                step = 50000;
        //            }
        //            else if (step < 10000)
        //            {
        //                step = 1000;
        //            }
        //            ////str = "TRAC:IQ:DATA:MEM?";// "TRAC:IQ:DATA?";
        //            ////session.Write(str);
        //            ////byte[] byteArray = session.ReadByteArray();
        //            ////float[] temp = new float[sample * 2];
        //            //////IQArr = new float[sample * 2];
        //            ////if (System.Text.Encoding.ASCII.GetString(byteArray, 0, 1) == "#")
        //            ////{
        //            ////    int lengthPreamb = int.Parse(System.Text.Encoding.ASCII.GetString(byteArray, 1, 1));
        //            ////    int lengthData = int.Parse(System.Text.Encoding.ASCII.GetString(byteArray, 2, lengthPreamb));
        //            ////    temp = new float[lengthData / 4];
        //            ////    for (int j = 0; j < temp.Length; j++)
        //            ////    {
        //            ////        index = j;
        //            ////        temp[j] = System.BitConverter.ToSingle(byteArray, lengthPreamb + 2 + j * 4);
        //            ////    }

        //            ////    //Debug.WriteLine(((double)(_timeService.TimeStamp.Ticks - ddd)) / 10000);
        //            ////}
        //            ////IQArr = temp;


        //            float[] temp = new float[sample * 2];
        //            long ddd = _timeService.TimeStamp.Ticks;
        //            for (int i = 0; i < sample; i += step)
        //            {
        //                int from = i, to = i + step;
        //                if (to > sample) to = sample;
        //                str = $"TRAC:IQ:DATA:MEM? {from},{to - from}";
        //                session.Write(str);
        //                byte[] byteArray = session.ReadByteArray();
        //                if (System.Text.Encoding.ASCII.GetString(byteArray, 0, 1) == "#")
        //                {
        //                    int lengthPreamb = int.Parse(System.Text.Encoding.ASCII.GetString(byteArray, 1, 1));
        //                    int lengthData = int.Parse(System.Text.Encoding.ASCII.GetString(byteArray, 2, lengthPreamb));
        //                    float[] temp2 = new float[lengthData / 4];

        //                    for (int j = 0; j < temp2.Length; j++)
        //                    {
        //                        temp2[j] = System.BitConverter.ToSingle(byteArray, lengthPreamb + 2 + j * 4);
        //                    }
        //                    //Debug.WriteLine(temp2.Length);
        //                    Array.Copy(temp2, 0, temp, pos, temp2.Length);
        //                    pos += temp2.Length;

        //                }
        //            }
        //            TriggerOffsetInSample = DecimalParse(session.Query("TRACe:IQ:TPISample?"));
        //            //Debug.WriteLine(((double)(_timeService.TimeStamp.Ticks - ddd)) / 10000);
        //            IQArr = temp;
        //        }
        //    }
        //    #region Exception
        //    catch (VisaException v_exp)
        //    {

        //        _logger.Exception(Contexts.ThisComponent, v_exp);
        //    }
        //    catch (Exception exp)
        //    {
        //        _logger.Exception(Contexts.ThisComponent, exp);
        //    }
        //    #endregion
        //}
        //public void GetIQ_old()
        //{
        //    int index = 0;
        //    try
        //    {

        //        if (UniqueData.InstrManufacture == 1)
        //        {
        //            session.Write("INIT:CONT OFF");
        //            //session.Write("TRACE:IQ ON");
        //            session.Write("TRACe:IQ:DATA:FORMat IQPair");

        //            double meastime = 0.01;
        //            session.Write($"SWEep:TIME {meastime.ToString().Replace(",", ".")}");
        //            double trigger = -90;
        //            session.Write($"TRIG:LEV:IQP {trigger.ToString().Replace(", ", ".")}");
        //            int ABW = 271 * 1000;
        //            int Speed = (int)(1.25 * ABW);

        //            int sample = (int)(Speed * meastime);
        //            string str = $"TRACE:IQ:SET NORM,0,{Speed},IMM,POS,0,{sample}";//IQP
        //            session.Write(str);

        //            //Configures the sample rate as 32 MHz, IQP trigger, positive trigger slope,
        //            //no pretrigger samples, 1000 samples to capture
        //            session.DefaultBufferSize = (sample * 4) * 2 + sample.ToString().Length + 100;

        //            ////str = "INIT;*WAI";
        //            ////session.Write(str);
        //            ////str = "TRAC:IQ:DATA?";
        //            ////session.Write(str);
        //            ////byte[] byteArray = session.ReadByteArray();
        //            ////float[] temp = new float[sample * 2];
        //            //////IQArr = new float[sample * 2];
        //            ////if (System.Text.Encoding.ASCII.GetString(byteArray, 0, 1) == "#")
        //            ////{
        //            ////    int lengthPreamb = int.Parse(System.Text.Encoding.ASCII.GetString(byteArray, 1, 1));
        //            ////    int lengthData = int.Parse(System.Text.Encoding.ASCII.GetString(byteArray, 2, lengthPreamb));
        //            ////    temp = new float[lengthData / 4];
        //            ////    long ddd = _timeService.TimeStamp.Ticks;
        //            ////    for (int j = 0; j < temp.Length; j++)
        //            ////    {
        //            ////        index = j;
        //            ////        temp[j] = System.BitConverter.ToSingle(byteArray, lengthPreamb + 2 + j * 4);
        //            ////    }

        //            ////    //Debug.WriteLine(((double)(_timeService.TimeStamp.Ticks - ddd)) / 10000);
        //            ////}
        //            ////IQArr = temp;





        //            str = "TRAC:IQ:DATA?; *WAI;";// "INST:CRE:REPL 'IQAnalyzer',SANALYZER,'Spectrum'";//SANALYZER   IQANALYZER
        //            session.Write(str);
        //            //session.DefaultBufferSize = sample * 4 + sample.ToString().Length + 10;


        //            int step = 1000;// sample / 1000;
        //            float[] temp = new float[sample];
        //            for (int i = 0; i < sample / 2; i += step)
        //            {
        //                int from = i, to = i + step;
        //                if (to > sample / 2) to = sample / 2;
        //                str = $"TRAC:IQ:DATA:MEM? {from},{to}";
        //                session.Write(str);
        //                byte[] byteArray = session.ReadByteArray();
        //                if (System.Text.Encoding.ASCII.GetString(byteArray, 0, 1) == "#")
        //                {
        //                    int lengthPreamb = int.Parse(System.Text.Encoding.ASCII.GetString(byteArray, 1, 1));
        //                    int lengthData = int.Parse(System.Text.Encoding.ASCII.GetString(byteArray, 2, lengthPreamb));
        //                    float[] temp2 = new float[lengthData / 4];
        //                    long ddd = _timeService.TimeStamp.Ticks;
        //                    for (int j = 0; j < temp2.Length; j++)
        //                    {
        //                        temp2[j] = System.BitConverter.ToSingle(byteArray, lengthPreamb + 2 + j * 4);
        //                    }
        //                    Array.Copy(temp2, 0, temp, from, to);
        //                    //Debug.WriteLine(((double)(_timeService.TimeStamp.Ticks - ddd)) / 10000);
        //                }
        //            }
        //            IQArr = temp;
        //        }
        //        //if (UniqueData.InstrManufacture == 1)
        //        //{
        //        //    session.Write("TRACe:IQ:DATA:FORMat IQPair");

        //        //    double meastime = 0.5;
        //        //    session.Write($"SWEep:TIME {meastime.ToString().Replace(",", ".")}");

        //        //    int speed = 10000000;
        //        //    int sample = (int)(meastime * speed);
        //        //    session.Write($"TRACE:IQ:SET NORM,0,{speed},IQP,POS,0,{sample}");
        //        //    //Configures the sample rate as 32 MHz, IQP trigger, positive trigger slope,
        //        //    //no pretrigger samples, 1000 samples to capture


        //        //    string str = ""; //MAGN                    

        //        //    str = "TRAC:IQ:DATA?; *WAI;";// "INST:CRE:REPL 'IQAnalyzer',SANALYZER,'Spectrum'";//SANALYZER   IQANALYZER
        //        //    session.Write(str);
        //        //    session.DefaultBufferSize = sample * 4 + sample.ToString().Length + 10;

        //        //    int step = 1000;
        //        //    IQArr = new float[sample];
        //        //    for (int i = 0; i < sample - step; i += step)
        //        //    {
        //        //        str = $"TRAC:IQ:DATA:MEM? {i},{i + step}";
        //        //        session.Write(str);
        //        //        byte[] byteArray = session.ReadByteArray();
        //        //        if (System.Text.Encoding.ASCII.GetString(byteArray, 0, 1) == "#")
        //        //        {
        //        //            int lengthPreamb = int.Parse(System.Text.Encoding.ASCII.GetString(byteArray, 1, 1));
        //        //            int lengthData = int.Parse(System.Text.Encoding.ASCII.GetString(byteArray, 2, lengthPreamb));
        //        //            float[] temp = new float[lengthData / 4];
        //        //            long ddd = _timeService.TimeStamp.Ticks;
        //        //            for (int j = 0; j < temp.Length; j++)
        //        //            {
        //        //                temp[j] = System.BitConverter.ToSingle(byteArray, lengthPreamb + 2 + j * 4);
        //        //            }
        //        //            Array.Copy(temp, 0, IQArr, i, i + step);
        //        //            Debug.WriteLine(((double)(_timeService.TimeStamp.Ticks - ddd)) / 10000);
        //        //        }
        //        //    }
        //        //    //string res = session.Query(str);

        //        //}
        //    }
        //    #region Exception
        //    catch (VisaException v_exp)
        //    {

        //        _logger.Exception(Contexts.ThisComponent, v_exp);
        //    }
        //    catch (Exception exp)
        //    {
        //        _logger.Exception(Contexts.ThisComponent, exp);
        //    }
        //    #endregion
        //}
        #endregion

        private void GetDeviceInfo()
        {
            try
            {
                if (UniqueData.InstrManufacture == 1)
                {
                    if (UniqueData.Battery)
                    {
                        string[] st = session.Query(":STAT:QUES:POW?;:SYSTem:POWer:STATus?").TrimEnd().Split(';');
                        //PowerRegister = int.Parse(st[0]);
                        double d = double.Parse(st[1], System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture);
                        if (d > 100) { BatteryCharging = true; }
                        else if (d > 0 && d <= 100) { BatteryCharging = false; BatteryCharge = (decimal)d; }
                    }
                    else
                    {
                        //PowerRegister = int.Parse(session.Query(":STAT:QUES:POW?"));
                    }
                }
                else if (UniqueData.InstrManufacture == 2)
                {
                    string t = session.Query(":SYST:BATT:ARTT?");
                    //BatteryAbsoluteCharge = int.Parse(session.Query(":SYST:BATT:ACUR?").Replace('.', ','));
                    //System.Windows.MessageBox.Show(t);
                }
            }
            #region Exception
            catch (VisaException v_exp)
            {
                _logger.Exception(Contexts.ThisComponent, v_exp);
            }
            catch (Exception exp)
            {
                _logger.Exception(Contexts.ThisComponent, exp);
            }
            #endregion
        }
        private void InstrShutdown()
        {
            //try
            //{
            //    if (UniqueData.InstrManufacture == 1)
            //    {
            //        session.Write(":SYSTem:SHUTdown");
            //    }
            //    else if (UniqueData.InstrManufacture == 2)
            //    {
            //        session.Write(":SYST:PWR:SHUT 1");
            //    }
            //    Run = false;
            //    //session.Dispose();
            //    //session = null;
            //}
            //#region Exception
            //catch (VisaException v_exp)
            //{
            //    _logger.Exception(Contexts.ThisComponent, v_exp);
            //}
            //catch (Exception exp)
            //{
            //    _logger.Exception(Contexts.ThisComponent, exp);
            //}
            //#endregion
        }

        /// <summary>
        /// Сбрасываем настойки прибора 
        /// </summary>
        private void SetPreset()
        {
            try
            {
                if (UniqueData.InstrManufacture == 1)
                {
                    session.Write(":SYSTem:PRES");
                }
                else if (UniqueData.InstrManufacture == 2)
                {
                    //session.Write(":SYSTem:PRES");
                }
                else if (UniqueData.InstrManufacture == 3)
                {
                    session.Write(":SYSTem:PRES");
                }
                //GetLevelUnit();
                //GetFreqCentr();
                //GetFreqSpan();
                //GetRBW();
                //GetAutoRBW();
                //GetVBW();
                //GetAutoVBW();

                ////an_dm += SetCouplingRatio;
                //GetSweepTime();
                //GetAutoSweepTime();
                //GetSweepType();
                //GetSweepPoints();
                //GetRefLevel();
                //GetAttLevel();
                //GetAutoAttLevel();
                //GetPreAmp();
                //GetTraceType();
                //GetDetectorType();
                //GetAverageCount();
                //GetNumberOfSweeps();


                //GetTransducer();
                //GetSelectedTransducer();
                //GetSetAnSysDateTime();
            }
            #region Exception
            catch (VisaException v_exp)
            {
                _logger.Exception(Contexts.ThisComponent, v_exp);
            }
            catch (Exception exp)
            {
                _logger.Exception(Contexts.ThisComponent, exp);
            }
            #endregion
        }

        private void GetSetAnSysDateTime()
        {
            try
            {
                if (UniqueData.InstrManufacture == 1)
                {
                    string[] d = session.Query("SYST:DATE?").TrimEnd().Trim(' ').Split(',');
                    string[] t = session.Query("SYST:TIME?").TrimEnd().Trim(' ').Split(',');
                    string time = d[0] + "-" + d[1] + "-" + d[2] + " " +
                        t[0] + ":" + t[1] + ":" + t[2];
                    DateTime andt = DateTime.Parse(time);
                    if (new TimeSpan(DateTime.Now.Ticks - andt.Ticks) > new TimeSpan(0, 0, 1, 0, 0))
                    {
                        session.Write("SYST:DATE " + DateTime.Now.Year.ToString() + "," + DateTime.Now.Month.ToString() + "," + DateTime.Now.Day.ToString());
                        session.Write("SYST:TIME " + DateTime.Now.Hour.ToString() + "," + DateTime.Now.Minute.ToString() + "," + DateTime.Now.Second.ToString());
                    }
                }
                else if (UniqueData.InstrManufacture == 2)
                {

                }
                else if (UniqueData.InstrManufacture == 3)
                {

                }
            }
            #region Exception
            catch (VisaException v_exp)
            {
                _logger.Exception(Contexts.ThisComponent, v_exp);
            }
            catch (Exception exp)
            {
                _logger.Exception(Contexts.ThisComponent, exp);
            }
            #endregion
        }
        #endregion AN Method

        #region Adapter Properties
        private void SetDefaulConfig(ref CFG.AdapterMainConfig config)
        {
            config.IQBitRateMax = 40;
            config.AdapterEquipmentInfo = new CFG.AdapterEquipmentInfo()
            {
                AntennaManufacturer = "AntennaManufacturer",
                AntennaName = "Omni",
                AntennaSN = "123"
            };
            config.AdapterRadioPathParameters = new CFG.AdapterRadioPathParameter[]
            {
                new CFG.AdapterRadioPathParameter()
                {
                    Freq = 1*1000000,
                    KTBF = -147,//уровень своих шумов на Гц
                    FeederLoss = 2,//потери фидера
                    Gain = 10, //коэф усиления
                    DiagA = "HV",
                    DiagH = "POINT 0 0 90 3 180 6 270 3",//от нуля В конфиг
                    DiagV = "POINT -90 20 0 0 90 10"//от -90  до 90 В конфиг
                },
                new CFG.AdapterRadioPathParameter()
                {
                    Freq = 1000*1000000,
                    KTBF = -147,//уровень своих шумов на Гц
                    FeederLoss = 2,//потери фидера
                    Gain = 10, //коэф усиления
                    DiagA = "HV",
                    DiagH = "POINT 0 0 90 3 180 6 270 3",//от нуля В конфиг
                    DiagV = "POINT -90 20 0 0 90 10"//от -90  до 90 В конфиг
                }
            };
        }

        private (MesureTraceDeviceProperties, MesureIQStreamDeviceProperties) GetProperties(CFG.AdapterMainConfig config)
        {
            RadioPathParameters[] rrps = ConvertRadioPathParameters(config);
            StandardDeviceProperties sdp = new StandardDeviceProperties()
            {
                AttMax_dB = (int)UniqueData.AttMax,
                AttMin_dB = 0,
                FreqMax_Hz = UniqueData.FreqMax,
                FreqMin_Hz = UniqueData.FreqMin,
                PreAmpMax_dB = 1, //типа включен/выключен, сколько по факту усиливает нигде не пишется кроме FSW где их два 15/30 и то два это опция
                PreAmpMin_dB = 0,
                RefLevelMax_dBm = (int)UniqueData.RefLevelMax,
                RefLevelMin_dBm = (int)UniqueData.RefLevelMin,
                EquipmentInfo = new EquipmentInfo()
                {
                    AntennaCode = config.AdapterEquipmentInfo.AntennaSN,// "Omni",//S/N  В конфиг
                    AntennaManufacturer = config.AdapterEquipmentInfo.AntennaManufacturer,//"3anet",//В конфиг
                    AntennaName = config.AdapterEquipmentInfo.AntennaName,//"BC600",//В конфиг
                    EquipmentManufacturer = new Atdi.DataModels.Sdrn.DeviceServer.Adapters.InstrManufacrures().RuS.UI,
                    EquipmentName = UniqueData.InstrModel,
                    EquipmentFamily = "SpectrumAnalyzer",//SDR/SpecAn/MonRec
                    EquipmentCode = UniqueData.SerialNumber,//S/N

                },
                RadioPathParameters = rrps
            };
            if (UniqueData.PreAmp)
            {
                sdp.PreAmpMax_dB = 1;
            }
            else
            {
                sdp.PreAmpMax_dB = 0;
            }
            MesureTraceDeviceProperties mtdp = new MesureTraceDeviceProperties()
            {
                RBWMax_Hz = (double)UniqueData.RBWArr[UniqueData.RBWArr.Length - 1],
                RBWMin_Hz = (double)UniqueData.RBWArr[0],
                SweepTimeMin_s = (double)UniqueData.SWTMin,
                SweepTimeMax_s = (double)UniqueData.SWTMax,
                StandardDeviceProperties = sdp,
                //DeviceId ничего не писать, ID этого экземпляра адаптера
            };
            MesureIQStreamDeviceProperties miqdp = new MesureIQStreamDeviceProperties()
            {
                AvailabilityPPS = false,// Т.к. нет у анализаторов спектра их, хотя через тригеры можно попробывать
                BitRateMax_MBs = config.IQBitRateMax,
                //DeviceId ничего не писать, ID этого экземпляра адаптера
                standartDeviceProperties = sdp,
            };


            return (mtdp, miqdp);
        }

        private RadioPathParameters[] ConvertRadioPathParameters(CFG.AdapterMainConfig config)
        {
            RadioPathParameters[] rpps = new RadioPathParameters[config.AdapterRadioPathParameters.Length];
            for (int i = 0; i < config.AdapterRadioPathParameters.Length; i++)
            {
                rpps[i] = new RadioPathParameters()
                {
                    Freq_Hz = config.AdapterRadioPathParameters[i].Freq,
                    KTBF_dBm = config.AdapterRadioPathParameters[i].KTBF,//уровень своих шумов на Гц
                    FeederLoss_dB = config.AdapterRadioPathParameters[i].FeederLoss,//потери фидера
                    Gain = config.AdapterRadioPathParameters[i].Gain, //коэф усиления
                    DiagA = config.AdapterRadioPathParameters[i].DiagA,
                    DiagH = config.AdapterRadioPathParameters[i].DiagH,//от нуля В конфиг
                    DiagV = config.AdapterRadioPathParameters[i].DiagV//от -90  до 90 В конфиг
                };
            }
            return rpps;
        }
        #endregion Adapter Properties
        #endregion Private Method



    }

}
