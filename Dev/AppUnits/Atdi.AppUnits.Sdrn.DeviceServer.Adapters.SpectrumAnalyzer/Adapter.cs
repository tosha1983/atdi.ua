﻿using Atdi.Contracts.Sdrn.DeviceServer;
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
using NationalInstruments.Visa;
using Ivi.Visa;
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
            LevelArr = new float[TracePoints];
            for (int i = 0; i < TracePoints; i++)
            {
                LevelArr[i] = -100;
            }
            if (adapterConfig.ConnectionMode == 0)
            {
                ConnectionMode = EN.ConnectionMode.Standard;
            }
            else if (adapterConfig.ConnectionMode == 1)
            {
                ConnectionMode = EN.ConnectionMode.HiSpeed;
            }
            else
            {
                throw new Exception("ConnectionMode parameter is not set in the AdapterConfig");
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
                        traceCountToMeas = (ulong)command.Parameter.TraceCount + 1;// +1 маленький костыль для предотвращения кривого трейса
                        traceCount = 0;
                        TracePoints = command.Parameter.TracePoint;
                    }
                    else
                    {
                        throw new Exception("TraceCount must be set greater than zero.");
                    }

                    //Меряем
                    //Если TraceType ClearWrite то пушаем каждый результат                    
                    if (TraceTypeResult == EN.TraceType.ClearWrite)
                    {
                        bool newres = false;


                        for (ulong i = 0; i < traceCountToMeas; i++)
                        {
                            newres = GetTrace();
                            if (newres)
                            {
                                if (i > 0)// +1 маленький костыль для предотвращения кривого трейса
                                {
                                    // пушаем результат
                                    var result = new COMR.MesureTraceResult(traceCount, CommandResultStatus.Next);
                                    traceCount++;
                                    if (traceCountToMeas == traceCount)
                                    {
                                        result = new COMR.MesureTraceResult(traceCount, CommandResultStatus.Final);
                                    }
                                    result.LevelMaxIndex = LevelArr.Length;
                                    result.FrequencyStart_Hz = resFreqStart;
                                    result.FrequencyStep_Hz = resFreqStep;
                                    result.Att_dB = (int)AttLevelSpec;
                                    result.RefLevel_dBm = (int)RefLevelSpec;
                                    result.PreAmp_dB = PreAmpSpec ? 1 : 0;
                                    result.RBW_Hz = (double)RBW;
                                    result.VBW_Hz = (double)VBW;
                                    result.Level = new float[LevelArr.Length];
                                    for (int j = 0; j < LevelArr.Length; j++)
                                    {
                                        result.Level[j] = LevelArr[j];
                                    }
                                    result.TimeStamp = _timeService.GetGnssUtcTime().Ticks - UTCOffset;// new DateTime(1970, 1, 1, 0, 0, 0, System.DateTimeKind.Utc).Ticks;
                                                                                                       //result.TimeStamp = DateTime.UtcNow.Ticks - new DateTime(1970, 1, 1, 0, 0, 0, System.DateTimeKind.Utc).Ticks;
                                    if (PowerRegister != EN.PowerRegister.Normal)
                                    {
                                        result.DeviceStatus = COMR.Enums.DeviceStatus.RFOverload;
                                    }
                                    context.PushResult(result);
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
                                var result2 = new COMR.MesureTraceResult(traceCount, CommandResultStatus.Ragged);

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
                            TraceAveraged.AveragingCount = (int)traceCountToMeas - 1;
                        }
                        bool _RFOverload = false;
                        bool newres = false;
                        for (ulong i = 0; i < traceCountToMeas; i++)
                        {
                            newres = GetTrace();
                            if (newres)
                            {
                                if (i == 0)
                                {
                                    TraceReset = true;
                                }
                                traceCount++;
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

                                var result2 = new COMR.MesureTraceResult(traceCount, CommandResultStatus.Ragged);
                                //Скорее нет результатов
                                context.PushResult(result2);

                                // подтверждаем факт обработки отмены
                                context.Cancel();
                                // освобождаем поток 
                                return;
                            }
                        }
                        if (traceCountToMeas == traceCount)
                        {
                            var result = new COMR.MesureTraceResult(0, CommandResultStatus.Final)
                            {
                                Level = new float[LevelArr.Length]
                            };
                            for (int j = 0; j < LevelArr.Length; j++)
                            {
                                result.Level[j] = LevelArr[j];
                            }
                            result.LevelMaxIndex = LevelArr.Length;
                            result.FrequencyStart_Hz = resFreqStart;
                            result.FrequencyStep_Hz = resFreqStep;

                            result.Att_dB = (int)AttLevelSpec;
                            result.RefLevel_dBm = (int)RefLevelSpec;
                            result.PreAmp_dB = PreAmpSpec ? 1 : 0;
                            result.RBW_Hz = (double)RBW;
                            result.VBW_Hz = (double)VBW;
                            result.TimeStamp = _timeService.GetGnssUtcTime().Ticks - UTCOffset;// new DateTime(1970, 1, 1, 0, 0, 0, System.DateTimeKind.Utc).Ticks;
                            //result.TimeStamp = DateTime.UtcNow.Ticks - new DateTime(1970, 1, 1, 0, 0, 0, System.DateTimeKind.Utc).Ticks;
                            if (_RFOverload)
                            {
                                result.DeviceStatus = COMR.Enums.DeviceStatus.RFOverload;
                            }
                            else
                            {
                                result.DeviceStatus = COMR.Enums.DeviceStatus.Normal;
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
                context.Unlock();
                context.Abort(v_exp);
                // дальше кода быть не должно, освобождаем поток
            }
            catch (Exception e)
            {
                // желательно записать влог
                _logger.Exception(Contexts.ThisComponent, e);
                // этот вызов обязательный в случаи обрыва
                context.Unlock();
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

                COMR.MesureIQStreamResult result = new COMR.MesureIQStreamResult(0, CommandResultStatus.Final)
                {
                    DeviceStatus = COMR.Enums.DeviceStatus.Normal
                };
                if (GetIQStream(ref result, command))
                {
                    context.PushResult(result);
                }
                /////////////
                //////////long timestop = _timeService.GetGnssTime().Ticks;
                context.Unlock();
                context.Finish();
            }
            catch (Exception e)
            {
                // желательно записать влог
                _logger.Exception(Contexts.ThisComponent, e);
                // этот вызов обязательный в случаи обрыва
                context.Unlock();
                context.Abort(e);
                // дальше кода быть не должно, освобождаем поток
            }
        }
        #endregion


        #region Param
        ResourceManager rm = null;
        private EN.ConnectionMode ConnectionMode = EN.ConnectionMode.HiSpeed;
        private long UTCOffset = 621355968000000000;
        private string decimalSeparator = System.Globalization.NumberFormatInfo.CurrentInfo.CurrencyDecimalSeparator;
        private TcpipSession session = null;
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
        private double resFreqStart = 10000;
        private double resFreqStep = 10000;

        private ulong traceCountToMeas = 1;
        private ulong traceCount = 1;

        //public double[] FreqArr;
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

        #region NI Visa
        private void WriteString(string Input)
        {
            session.FormattedIO.WriteLine(Input);
            //session.RawIO.Write(Input);
        }
        private string ReadString()
        {
            return session.FormattedIO.ReadString();
        }
        private string QueryString(string Input)
        {
            session.FormattedIO.WriteLine(Input);
            return session.FormattedIO.ReadString();
            //session.RawIO.Write(Input);
            //return session.RawIO.ReadString().TrimEnd();
        }
        private byte[] ReadByte()
        {
            return session.FormattedIO.ReadBinaryBlockOfByte();
        }
        private byte[] QueryByte(string Input)
        {
            session.FormattedIO.WriteLine(Input);
            return session.FormattedIO.ReadBinaryBlockOfByte();
        }
        private float[] QueryFloat(string Input)
        {
            session.FormattedIO.WriteLine(Input);
            byte[] byteArray = session.FormattedIO.ReadBinaryBlockOfByte();
            float[] temp = new float[byteArray.Length / 4];
            for (int j = 0; j < temp.Length / 4; j++)
            {
                temp[j] = System.BitConverter.ToSingle(byteArray, j * 4);
            }
            return temp;
        }
        private decimal QueryDecimal(string Input)
        {
            session.FormattedIO.WriteLine(Input);
            return decimal.Parse(session.FormattedIO.ReadString().Replace(".", decimalSeparator).Replace(",", decimalSeparator));
        }
        private int QueryInt(string Input)
        {
            session.FormattedIO.WriteLine(Input);
            return int.Parse(session.FormattedIO.ReadString());
        }
        #endregion NI Visa
        private decimal DecimalParse(string str)
        {
            return decimal.Parse(str.Replace(".", decimalSeparator).Replace(",", decimalSeparator));
        }
        private bool SetConnect()
        {
            bool res = false;
            rm = new ResourceManager();
            if (ConnectionMode == EN.ConnectionMode.Standard)
            {
                session = (TcpipSession)rm.Open(String.Concat("TCPIP::", _adapterConfig.IPAddress, "::INSTR"));//, AccessModes.None, 20000);
            }
            else if (ConnectionMode == EN.ConnectionMode.HiSpeed)
            {
                session = (TcpipSession)rm.Open(String.Concat("TCPIP::", _adapterConfig.IPAddress, "::hislip"));//, AccessModes.None, 20000);
            }
            string[] temp = QueryString("*IDN?").Trim('"').Split(',');
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
                        string[] op = QueryString("*OPT?").ToUpper().Split(',');
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
                WriteString("FORMat:DEXPort:DSEParator COMM");// POIN");//разделитель дробной части
                if (UniqueData.HiSpeed == true)
                {
                    WriteString(":FORM:DATA REAL,32");// ASC");//передавать трейс в ASCII
                }
                else if (UniqueData.HiSpeed == false)
                {
                    WriteString("FORM:DATA REAL,32"); WriteString("INST SAN");
                }
                if (_adapterConfig.DisplayUpdate)
                {
                    WriteString(":SYST:DISP:UPD ON");
                }
                else
                {
                    WriteString(":SYST:DISP:UPD OFF");
                }
                if (UniqueData.HiSpeed == true)
                {
                    SweepPoints = QueryInt(":SWE:POIN?");
                }
                else if (UniqueData.HiSpeed == false)
                {
                    SweepPoints = UniqueData.DefaultSweepPoint;
                    TracePoints = SweepPoints;
                }
                //session.DefaultBufferSize = SweepPoints * 18 + 25; //увеличиваем буфер чтобы влезло 32001 точка трейса
                UniqueData.FreqMin = QueryDecimal(":SENSe:FREQuency:STAR? MIN");
                UniqueData.FreqMax = QueryDecimal(":SENSe:FREQuency:STOP? MAX");

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
                WriteString("FORM:DATA REAL,32"); //передавать трейс побайтово //в ASCII
                SweepPoints = QueryInt(":SENS:SWE:POIN?");
                SweepPointsIndex = System.Array.IndexOf(UniqueData.SweepPointArr, SweepPoints);

                TracePoints = SweepPoints;
                //session.DefaultBufferSize = SweepPoints * 4 + 20;
                if (_adapterConfig.DisplayUpdate)
                {
                    WriteString(":DISP:ENAB 1");
                }
                else
                {
                    WriteString(":DISP:ENAB 0");
                }
            }
            else if (UniqueData.InstrManufacture == 3)
            {
                WriteString("FORM:DATA REAL,32"); //передавать трейс побайтово
                SweepPoints = 551;
                TracePoints = 551;
                SweepPointsIndex = 0;
                //session.DefaultBufferSize = 2204 + 20;
                WriteString(":MMEMory:MSIS INT");
                if (_adapterConfig.DisplayUpdate)
                {
                    WriteString(":DISP:ENAB 1");
                }
                else
                {
                    WriteString(":DISP:ENAB 0");
                }
                SweepPointsIndex = System.Array.IndexOf(UniqueData.SweepPointArr, SweepPoints);
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
            return res;
        }





        #region AN To Command
        /// <summary>
        /// Получаем центральную частоту 
        /// </summary>
        private void GetFreqCentr()
        {
            if (UniqueData.InstrManufacture == 1)
            {
                FreqCentr = QueryDecimal(":SENSe:FREQuency:CENTer?");
            }
            else if (UniqueData.InstrManufacture == 2)
            {
                FreqCentr = QueryDecimal(":FREQ:CENT?");
            }
            else if (UniqueData.InstrManufacture == 3)
            {
                FreqCentr = QueryDecimal(":SENSe:FREQuency:CENTer?");
            }
            GetFreqArr();
        }
        /// <summary>
        /// получаем span
        /// </summary>
        private void GetFreqSpan()
        {
            if (UniqueData.InstrManufacture == 1)
            {
                FreqSpan = QueryDecimal(":SENSe:FREQuency:SPAN?");
            }
            else if (UniqueData.InstrManufacture == 2)
            {
                FreqSpan = QueryDecimal(":FREQuency:SPAN?");
            }
            else if (UniqueData.InstrManufacture == 3)
            {
                FreqSpan = QueryDecimal(":SENSe:FREQuency:SPAN?");
            }
            GetFreqArr();
        }
        /// <summary>
        /// Установка Начальной частоты просмотра 
        /// </summary>
        private bool SetFreqStart(decimal freqStart)
        {
            bool res = false;
            FreqStart = freqStart;
            if (UniqueData.InstrManufacture == 1)
            {
                WriteString(":SENSe:FREQ:STAR " + FreqStart.ToString().Replace(decimalSeparator, "."));
                FreqStart = QueryDecimal(":SENSe:FREQ:STAR?");
            }
            else if (UniqueData.InstrManufacture == 2)
            {
                WriteString("FREQ:STAR " + FreqStart.ToString().Replace(decimalSeparator, "."));
                FreqStart = QueryDecimal(":FREQ:STAR?");
            }
            else if (UniqueData.InstrManufacture == 3)
            {
                WriteString("FREQ:STAR " + FreqStart.ToString().Replace(decimalSeparator, "."));
                FreqCentr = QueryDecimal(":SENSe:FREQuency:CENTer?");
            }
            res = true;

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
            FreqStop = freqStop;
            if (UniqueData.InstrManufacture == 1)
            {
                WriteString(":SENSe:FREQ:STOP " + FreqStop.ToString().Replace(decimalSeparator, "."));
                FreqStop = QueryDecimal(":SENSe:FREQ:STOP?");
            }
            else if (UniqueData.InstrManufacture == 2)
            {
                WriteString(":FREQ:STOP " + FreqStop.ToString().Replace(decimalSeparator, "."));
                FreqStop = QueryDecimal(":FREQ:STOP?");
            }
            else if (UniqueData.InstrManufacture == 3)
            {
                WriteString(":SENSe:FREQ:STOP " + FreqStop.ToString().Replace(decimalSeparator, "."));
                FreqCentr = QueryDecimal(":SENSe:FREQuency:CENTer?");
            }
            res = true;
            GetFreqArr();
            if (AutoRBW == true) { GetRBW(); }
            if (AutoVBW == true) { GetVBW(); }
            if (AutoSweepTime == true) { GetSweepTime(); }

            return res;
        }

        private void GetFreqArr()
        {
            if (UniqueData.InstrManufacture == 1)
            {
                if (UniqueData.HiSpeed == true)
                {
                    //WriteString("TRAC1:X? TRACE1");
                    byte[] byteArray = QueryByte("TRAC1:X? TRACE1");
                    double stepsum = 0;
                    double[] temp = new double[byteArray.Length / 4];
                    temp[0] = System.BitConverter.ToSingle(byteArray, 0);
                    for (int j = 1; j < temp.Length; j++)
                    {
                        temp[j] = System.BitConverter.ToSingle(byteArray, j * 4);
                        stepsum += temp[j] - temp[j - 1];
                    }
                    resFreqStep = stepsum / (temp.Length - 1);
                    resFreqStart = temp[0];

                    LevelArr = new float[temp.Length];
                    LevelArrTemp = new float[temp.Length];
                    for (int i = 0; i < temp.Length; i++)
                    {
                        LevelArr[i] = -1000;
                        LevelArrTemp[i] = -1000;
                    }

                    //if (System.Text.Encoding.ASCII.GetString(byteArray, 0, 1) == "#")
                    //{
                    //    int lengthPreamb = int.Parse(System.Text.Encoding.ASCII.GetString(byteArray, 1, 1));
                    //    int lengthData = int.Parse(System.Text.Encoding.ASCII.GetString(byteArray, 2, lengthPreamb));
                    //    double[] temp = new double[lengthData / 4 + 2];
                    //    for (int j = 0; j < lengthData / 4; j++)
                    //    {
                    //        temp[j + 1] = System.BitConverter.ToSingle(byteArray, lengthPreamb + 2 + j * 4);
                    //    }
                    //    temp[0] = (double)FreqStart;
                    //    temp[temp.Length - 1] = (double)FreqStop;
                    //    FreqArr = temp;
                    //    LevelArr = new float[FreqArr.Length];
                    //    LevelArrTemp = new float[FreqArr.Length];
                    //    for (int i = 0; i < FreqArr.Length; i++)
                    //    {
                    //        LevelArr[i] = -1000;
                    //        LevelArrTemp[i] = -1000;
                    //    }
                    //}
                }
                else if (UniqueData.HiSpeed == false)
                {
                    LevelArr = new float[TracePoints];
                    LevelArrTemp = new float[LevelArr.Length];
                    for (int i = 0; i < TracePoints; i++)
                    {
                        LevelArr[i] = -1000;
                        LevelArrTemp[i] = -1000;
                    }
                }
            }
            else if (UniqueData.InstrManufacture == 2)
            {
                LevelArr = new float[TracePoints];
                LevelArrTemp = new float[LevelArr.Length];
                for (int i = 0; i < TracePoints; i++)
                {
                    LevelArr[i] = -1000;
                    LevelArrTemp[i] = -1000;
                }
            }
            else if (UniqueData.InstrManufacture == 3)
            {
                LevelArr = new float[TracePoints];
                LevelArrTemp = new float[LevelArr.Length];
                for (int i = 0; i < TracePoints; i++)
                {
                    LevelArr[i] = -1000;
                    LevelArrTemp[i] = -1000;
                }
            }
        }
        /// <summary>
        /// Установка Центральной частоты просмотра IQ
        /// </summary>
        private void SetFreqCentrIQ(decimal freqCentrIQ)
        {
            FreqCentrIQ = freqCentrIQ;
            if (UniqueData.InstrManufacture == 1)
            {
                WriteString($"FREQ:CENT {FreqCentrIQ.ToString().Replace(decimalSeparator, ".")}");
                FreqCentrIQ = QueryDecimal(":FREQ:CENT?");
            }
            else if (UniqueData.InstrManufacture == 2)
            {

            }
            else if (UniqueData.InstrManufacture == 3)
            {

            }
        }

        private void SetAutoAttLevel(bool attAuto)
        {
            if (Mode == EN.Mode.SpectrumAnalyzer)
            {
                AttAutoSpec = attAuto;
                if (UniqueData.InstrManufacture == 1)
                {
                    if (AttAutoSpec == true)
                    {
                        WriteString(":INP:ATT:AUTO 1");
                    }
                    else
                    {
                        WriteString(":INP:ATT:AUTO 0");
                    }
                    AttLevelSpec = QueryDecimal(":INP:ATT?");
                }
                else if (UniqueData.InstrManufacture == 2)
                {
                    if (AttAutoSpec == true)
                    {
                        WriteString(":POW:ATT:AUTO 1");
                    }
                    else
                    {
                        WriteString(":POW:ATT:AUTO 0");
                    }
                    AttLevelSpec = QueryDecimal(":POW:ATT?");
                }
                else if (UniqueData.InstrManufacture == 3)
                {
                    if (AttAutoSpec == true)
                    {
                        WriteString(":POW:ATT:AUTO 1");
                    }
                    else
                    {
                        WriteString(":POW:ATT:AUTO 0");
                    }
                    AttLevelSpec = QueryDecimal(":POW:ATT?");
                }
            }
            else if (Mode == EN.Mode.IQAnalyzer)
            {
                AttAutoIQ = attAuto;
                if (UniqueData.InstrManufacture == 1)
                {
                    if (AttAutoIQ == true)
                    {
                        WriteString(":INP:ATT:AUTO 1");
                    }
                    else
                    {
                        WriteString(":INP:ATT:AUTO 0");
                    }
                    AttLevelIQ = QueryDecimal(":INP:ATT?");
                }
                else if (UniqueData.InstrManufacture == 2)
                {
                    if (AttAutoIQ == true)
                    {
                        WriteString(":POW:ATT:AUTO 1");
                    }
                    else
                    {
                        WriteString(":POW:ATT:AUTO 0");
                    }
                    AttLevelIQ = QueryDecimal(":POW:ATT?");
                }
                else if (UniqueData.InstrManufacture == 3)
                {
                    if (AttAutoIQ == true)
                    {
                        WriteString(":POW:ATT:AUTO 1");
                    }
                    else
                    {
                        WriteString(":POW:ATT:AUTO 0");
                    }
                    AttLevelIQ = QueryDecimal(":POW:ATT?");
                }
            }
        }
        /// <summary>
        /// Вкл/Выкл атоматического аттенюатора зависящего от опорного уровня, при выкл
        /// АвтоАТТ изменяем настройку аттенюатора
        /// </summary>
        private void SetAttLevel(decimal attLevel)
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
                    WriteString(":INP:ATT " + AttLevelSpec.ToString().Replace(decimalSeparator, ".")); //INP:ATT:AUTO
                }
                else if (UniqueData.InstrManufacture == 2)
                {
                    WriteString(":POW:ATT " + AttLevelSpec.ToString().Replace(decimalSeparator, "."));
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
                    WriteString(":INP:ATT " + AttLevelIQ.ToString().Replace(decimalSeparator, ".")); //INP:ATT:AUTO
                }
                else if (UniqueData.InstrManufacture == 2)
                {
                    WriteString(":POW:ATT " + AttLevelIQ.ToString().Replace(decimalSeparator, "."));
                }
            }
        }
        /// <summary>
        /// Получаем настройку аттенюатора
        /// </summary>
        private void GetAttLevel()
        {
            if (Mode == EN.Mode.SpectrumAnalyzer)
            {
                if (UniqueData.InstrManufacture == 1)
                {
                    AttLevelSpec = QueryDecimal(":INP:ATT?");
                }
                else if (UniqueData.InstrManufacture == 2)
                {
                    AttLevelSpec = QueryDecimal(":POW:ATT?");
                }
                else if (UniqueData.InstrManufacture == 3)
                {
                    string t = QueryString(":POW:ATT?");
                    if (t != "0.0") { AttLevelSpec = DecimalParse(t); }
                    else AttLevelSpec = 0;
                }
            }
            else if (Mode == EN.Mode.IQAnalyzer)
            {
                if (UniqueData.InstrManufacture == 1)
                {
                    AttLevelIQ = QueryDecimal(":INP:ATT?");
                }
                else if (UniqueData.InstrManufacture == 2)
                {
                    AttLevelIQ = QueryDecimal(":POW:ATT?");
                }
                else if (UniqueData.InstrManufacture == 3)
                {
                    string t = QueryString(":POW:ATT?");
                    if (t != "0.0") { AttLevelIQ = DecimalParse(t); }
                    else AttLevelIQ = 0;
                }
            }
        }
        private void SetAutoAtt(bool attauto)
        {
            if (Mode == EN.Mode.SpectrumAnalyzer)
            {
                AttAutoSpec = attauto;
                if (UniqueData.InstrManufacture == 1)
                {
                    if (AttAutoSpec == true) WriteString(":INP:ATT:AUTO 1");
                    else WriteString(":INP:ATT:AUTO 0");
                    AttLevelSpec = QueryDecimal(":INP:ATT?");
                }
                else if (UniqueData.InstrManufacture == 2)
                {
                    if (AttAutoSpec == true) WriteString(":POW:ATT:AUTO 1");
                    else WriteString(":POW:ATT:AUTO 0");
                    AttLevelSpec = QueryDecimal(":POW:ATT?");
                }
                else if (UniqueData.InstrManufacture == 3)
                {
                    if (AttAutoSpec == true) WriteString(":POW:ATT:AUTO 1");
                    else WriteString(":POW:ATT:AUTO 0");
                    AttLevelSpec = QueryDecimal(":POW:ATT?");
                }
            }
            else if (Mode == EN.Mode.IQAnalyzer)
            {
                AttAutoIQ = attauto;
                if (UniqueData.InstrManufacture == 1)
                {
                    if (AttAutoIQ == true) WriteString(":INP:ATT:AUTO 1");
                    else WriteString(":INP:ATT:AUTO 0");
                    AttLevelIQ = QueryDecimal(":INP:ATT?");
                }
                else if (UniqueData.InstrManufacture == 2)
                {
                    if (AttAutoIQ == true) WriteString(":POW:ATT:AUTO 1");
                    else WriteString(":POW:ATT:AUTO 0");
                    AttLevelIQ = QueryDecimal(":POW:ATT?");
                }
                else if (UniqueData.InstrManufacture == 3)
                {
                    if (AttAutoIQ == true) WriteString(":POW:ATT:AUTO 1");
                    else WriteString(":POW:ATT:AUTO 0");
                    AttLevelIQ = QueryDecimal(":POW:ATT?");
                }
            }
        }
        private void GetAutoAttLevel()
        {
            if (Mode == EN.Mode.SpectrumAnalyzer)
            {
                string temp = "";
                if (UniqueData.InstrManufacture == 1)
                {
                    temp = QueryString(":INP:ATT:AUTO?");
                    if (temp.Contains("1")) { AttAutoSpec = true; }
                    else if (temp.Contains("0")) { AttAutoSpec = false; }
                }
                else if (UniqueData.InstrManufacture == 2)
                {
                    temp = QueryString(":POW:ATT:AUTO?");
                    if (temp.Contains("1")) { AttAutoSpec = true; }
                    else if (temp.Contains("0")) { AttAutoSpec = false; }
                }
                else if (UniqueData.InstrManufacture == 3)
                {
                    temp = QueryString(":POW:ATT:AUTO?");
                    if (temp.Contains("1")) { AttAutoSpec = true; }
                    else if (temp.Contains("0")) { AttAutoSpec = false; }
                }
            }
            else if (Mode == EN.Mode.IQAnalyzer)
            {
                string temp = "";
                if (UniqueData.InstrManufacture == 1)
                {
                    temp = QueryString(":INP:ATT:AUTO?");
                    if (temp.Contains("1")) { AttAutoIQ = true; }
                    else if (temp.Contains("0")) { AttAutoIQ = false; }
                }
                else if (UniqueData.InstrManufacture == 2)
                {
                    temp = QueryString(":POW:ATT:AUTO?");
                    if (temp.Contains("1")) { AttAutoIQ = true; }
                    else if (temp.Contains("0")) { AttAutoIQ = false; }
                }
                else if (UniqueData.InstrManufacture == 3)
                {
                    temp = QueryString(":POW:ATT:AUTO?");
                    if (temp.Contains("1")) { AttAutoIQ = true; }
                    else if (temp.Contains("0")) { AttAutoIQ = false; }
                }
            }
        }
        /// <summary>
        /// Вкл/Выкл предусилителя 
        /// </summary>
        private void SetPreAmp(bool preAmp)
        {
            if (Mode == EN.Mode.SpectrumAnalyzer)
            {
                PreAmpSpec = preAmp;
                if (UniqueData.InstrManufacture == 1 && UniqueData.PreAmp == true)
                {
                    if (PreAmpSpec == true)
                    {
                        WriteString(":INP:GAIN:STAT 1");
                    }
                    else
                    {
                        WriteString(":INP:GAIN:STAT 0");
                    }
                }
                else if (UniqueData.InstrManufacture == 2)
                {
                    if (PreAmpSpec == true)
                    {
                        WriteString(":POW:GAIN 1");
                    }
                    else
                    {
                        WriteString(":POW:GAIN 0");
                    }
                }
                else if (UniqueData.InstrManufacture == 3)
                {
                    if (PreAmpSpec == true)
                    {
                        WriteString(":POW:GAIN 1");
                    }
                    else
                    {
                        WriteString(":POW:GAIN 0");
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
                        WriteString(":INP:GAIN:STAT 1");
                    }
                    {
                        WriteString(":INP:GAIN:STAT 0");
                    }
                }
                else if (UniqueData.InstrManufacture == 2)
                {
                    if (PreAmpIQ == true)
                    {
                        WriteString(":POW:GAIN 1");
                    }
                    else
                    {
                        WriteString(":POW:GAIN 0");
                    }
                }
                else if (UniqueData.InstrManufacture == 3)
                {
                    if (PreAmpIQ == true)
                    {
                        WriteString(":POW:GAIN 1");
                    }
                    else
                    {
                        WriteString(":POW:GAIN 0");
                    }
                }
            }
        }
        /// <summary>
        /// Узнаем состояние предусилителя 
        /// </summary>
        private void GetPreAmp()
        {
            if (Mode == EN.Mode.SpectrumAnalyzer)
            {
                if (UniqueData.InstrManufacture == 1 && UniqueData.PreAmp == true)
                {
                    string temp = QueryString(":INP:GAIN:STAT?");
                    if (temp.Contains("1")) { PreAmpSpec = true; }
                    else if (temp.Contains("0")) { PreAmpSpec = false; }
                }
                else if (UniqueData.InstrManufacture == 2)
                {
                    string temp = QueryString(":POW:GAIN:STAT?");
                    if (temp.Contains("1")) { PreAmpSpec = true; }
                    else if (temp.Contains("0")) { PreAmpSpec = false; }
                }
                else if (UniqueData.InstrManufacture == 3)
                {
                    string temp = QueryString(":POW:GAIN:STAT?");
                    if (temp.Contains("1")) { PreAmpSpec = true; }
                    else if (temp.Contains("0")) { PreAmpSpec = false; }
                }
            }
            else if (Mode == EN.Mode.IQAnalyzer)
            {
                if (UniqueData.InstrManufacture == 1 && UniqueData.PreAmp == true)
                {
                    string temp = QueryString(":INP:GAIN:STAT?");
                    if (temp.Contains("1")) { PreAmpIQ = true; }
                    else if (temp.Contains("0")) { PreAmpIQ = false; }
                }
                else if (UniqueData.InstrManufacture == 2)
                {
                    string temp = QueryString(":POW:GAIN:STAT?");
                    if (temp.Contains("1")) { PreAmpIQ = true; }
                    else if (temp.Contains("0")) { PreAmpIQ = false; }
                }
                else if (UniqueData.InstrManufacture == 3)
                {
                    string temp = QueryString(":POW:GAIN:STAT?");
                    if (temp.Contains("1")) { PreAmpIQ = true; }
                    else if (temp.Contains("0")) { PreAmpIQ = false; }
                }
            }
        }
        /// <summary>
        /// Установка опорного уровня 
        /// </summary>
        private void SetRefLevel(decimal refLevel)
        {
            if (Mode == EN.Mode.SpectrumAnalyzer)
            {
                RefLevelSpec = refLevel;
                if (UniqueData.InstrManufacture == 1)
                {
                    if (UniqueData.HiSpeed)
                    {
                        WriteString(":DISPlay:WINDow:TRACe:Y:SCALe:RLEVel " + RefLevelSpec.ToString().Replace(decimalSeparator, "."));
                    }
                    else
                    {
                        WriteString(":DISP:TRAC:Y:RLEV " + RefLevelSpec.ToString().Replace(decimalSeparator, "."));
                    }
                }
                else if (UniqueData.InstrManufacture == 2)
                {
                    WriteString("DISP:WIND:TRAC:Y:SCAL:RLEV " + RefLevelSpec.ToString().Replace(decimalSeparator, "."));
                }
                else if (UniqueData.InstrManufacture == 3)
                {
                    WriteString(":DISPlay:WINDow:TRACe:Y:SCALe:RLEVel " + RefLevelSpec.ToString());
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
                        WriteString(":DISPlay:WINDow:TRACe:Y:SCALe:RLEVel " + RefLevelIQ.ToString().Replace(decimalSeparator, "."));
                    }
                    else
                    {
                        WriteString(":DISP:TRAC:Y:RLEV " + RefLevelIQ.ToString().Replace(decimalSeparator, "."));
                    }
                }
                else if (UniqueData.InstrManufacture == 2)
                {
                    WriteString("DISP:WIND:TRAC:Y:SCAL:RLEV " + RefLevelIQ.ToString().Replace(decimalSeparator, "."));
                }
                else if (UniqueData.InstrManufacture == 3)
                {
                    WriteString(":DISPlay:WINDow:TRACe:Y:SCALe:RLEVel " + RefLevelIQ.ToString().Replace(decimalSeparator, "."));
                }
                if (AttAutoIQ == true) { GetAttLevel(); }
            }
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
                        RefLevelSpec = Math.Round(QueryDecimal(":DISPlay:WINDow:TRACe:Y:SCALe:RLEVel?"));
                    }
                    else { RefLevelSpec = Math.Round(QueryDecimal(":DISP:TRAC:Y:RLEV?")); }
                }
                else if (UniqueData.InstrManufacture == 2)
                {
                    RefLevelSpec = Math.Round(QueryDecimal(":DISP:WIND:TRAC:Y:SCAL:RLEV?"));
                }
                else if (UniqueData.InstrManufacture == 3)
                {
                    RefLevelSpec = Math.Round(QueryDecimal(":DISP:WIND:TRAC:Y:SCAL:RLEV?"));
                }
            }
            else if (Mode == EN.Mode.IQAnalyzer)
            {
                if (UniqueData.InstrManufacture == 1)
                {
                    if (UniqueData.HiSpeed)
                    {
                        RefLevelIQ = Math.Round(QueryDecimal(":DISPlay:WINDow:TRACe:Y:SCALe:RLEVel?"));
                    }
                    else { RefLevelIQ = Math.Round(QueryDecimal(":DISP:TRAC:Y:RLEV?")); }
                }
                else if (UniqueData.InstrManufacture == 2)
                {
                    RefLevelIQ = Math.Round(QueryDecimal(":DISP:WIND:TRAC:Y:SCAL:RLEV?"));
                }
                else if (UniqueData.InstrManufacture == 3)
                {
                    RefLevelIQ = Math.Round(QueryDecimal(":DISP:WIND:TRAC:Y:SCAL:RLEV?"));
                }
            }
        }
        private void SetRange()
        {
            if (Mode == EN.Mode.SpectrumAnalyzer)
            {
                if (UniqueData.InstrManufacture == 1)
                {
                    WriteString(":DISP:TRAC:Y " + RangeSpec.ToString().Replace(decimalSeparator, "."));
                }
                else if (UniqueData.InstrManufacture == 2)
                {
                    //WriteString("DISP:WIND:TRAC:Y:SCAL:RLEV " + RefLevel.ToString().Replace(decimalSeparator, "."));
                }
                else if (UniqueData.InstrManufacture == 3)
                {
                    //WriteString(":DISPlay:WINDow:TRACe:Y:SCALe:RLEVel " + RefLevel.ToString().Replace(decimalSeparator, "."));
                }
                if (AttAutoSpec == true) { GetAttLevel(); }
            }
            else if (Mode == EN.Mode.IQAnalyzer)
            {
                if (UniqueData.InstrManufacture == 1)
                {
                    WriteString(":DISP:TRAC:Y " + RangeIQ.ToString().Replace(decimalSeparator, "."));
                }
                else if (UniqueData.InstrManufacture == 2)
                {
                    //WriteString("DISP:WIND:TRAC:Y:SCAL:RLEV " + RefLevel.ToString().Replace(decimalSeparator, "."));
                }
                else if (UniqueData.InstrManufacture == 3)
                {
                    //WriteString(":DISPlay:WINDow:TRACe:Y:SCALe:RLEVel " + RefLevel.ToString().Replace(decimalSeparator, "."));
                }
                if (AttAutoIQ == true) { GetAttLevel(); }
            }
        }
        private void GetRange()
        {
            if (Mode == EN.Mode.SpectrumAnalyzer)
            {
                if (UniqueData.InstrManufacture == 1)
                {
                    RangeSpec = Math.Round(QueryDecimal(":DISP:TRAC:Y?"));
                }
                else if (UniqueData.InstrManufacture == 2)
                {
                    //Range = Math.Round(QueryDecimal(":DISP:WIND:TRAC:Y:SCAL:RLEV?"));
                }
                else if (UniqueData.InstrManufacture == 3)
                {
                    //Range = Math.Round(QueryDecimal(":DISP:WIND:TRAC:Y:SCAL:RLEV?"));
                }
            }
            else if (Mode == EN.Mode.IQAnalyzer)
            {
                if (UniqueData.InstrManufacture == 1)
                {
                    RangeIQ = Math.Round(QueryDecimal(":DISP:TRAC:Y?"));
                }
                else if (UniqueData.InstrManufacture == 2)
                {
                    //Range = Math.Round(QueryDecimal(":DISP:WIND:TRAC:Y:SCAL:RLEV?"));
                }
                else if (UniqueData.InstrManufacture == 3)
                {
                    //Range = Math.Round(QueryDecimal(":DISP:WIND:TRAC:Y:SCAL:RLEV?"));
                }
            }
        }


        /// <summary>
        /// Установка Units
        /// </summary>
        private void SetLevelUnit(ParamWithId levelunits)
        {
            LevelUnits = levelunits;
            if (UniqueData.InstrManufacture == 1)
            {
                WriteString(":UNIT:POWer " + LevelUnits.Parameter);
            }
            else if (UniqueData.InstrManufacture == 2)
            {
                WriteString(":AMPL:UNIT " + LevelUnits.Parameter);
            }
            else if (UniqueData.InstrManufacture == 3)
            {
                WriteString(":UNIT:POWer " + LevelUnits.Parameter);
            }
        }
        /// <summary>
        /// Получаем Units 
        /// </summary>
        private void GetLevelUnit()
        {
            if (UniqueData.InstrManufacture == 1)
            {
                string temp = QueryString(":UNIT:POWer?");
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
                string temp = QueryString(":AMPL:UNIT?");
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
                string temp = QueryString(":UNIT:POWer?");
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

        /// <summary>
        /// Установка RBW 
        /// </summary>
        private void SetRBW(decimal rbw)
        {
            RBW = rbw;
            if (UniqueData.InstrManufacture == 1)
            {
                WriteString(":SENSe:BWIDth:RESolution " + RBW.ToString().Replace(decimalSeparator, "."));
            }
            else if (UniqueData.InstrManufacture == 2)
            {
                WriteString(":SENS:BAND:RES " + RBW.ToString().Replace(decimalSeparator, "."));
            }
            else if (UniqueData.InstrManufacture == 3)
            {
                WriteString(":SENSe:BWIDth:RESolution " + RBW.ToString().Replace(decimalSeparator, "."));
            }

            AutoRBW = false;
            if (AutoSweepTime == true) { GetSweepTime(); }
            if (AutoVBW == true) { GetVBW(); }
            GetSweepType();
        }
        /// <summary>
        /// Получение RBW 
        /// </summary>
        private void GetRBW()
        {
            if (UniqueData.InstrManufacture == 1)
            {
                RBW = QueryDecimal(":SENSe:BWIDth:RESolution?");
                RBWIndex = System.Array.IndexOf(UniqueData.RBWArr, RBW);
            }
            else if (UniqueData.InstrManufacture == 2)
            {
                RBW = QueryDecimal(":SENS:BAND:RES?");
                RBWIndex = System.Array.IndexOf(UniqueData.RBWArr, RBW);
            }
            else if (UniqueData.InstrManufacture == 3)
            {
                RBW = QueryDecimal(":SENSe:BWIDth:RESolution?");
                RBWIndex = System.Array.IndexOf(UniqueData.RBWArr, RBW);
            }
        }
        /// <summary>
        /// Вкл/Выкл Auto RBW хай пока будит
        /// </summary>
        private void SetAutoRBW()
        {
            if (UniqueData.InstrManufacture == 1)
            {
                if (AutoRBW == true) WriteString(":SENSe:BWIDth:RESolution:AUTO 1");
                if (AutoRBW == false) WriteString(":SENSe:BWIDth:RESolution:AUTO 0");
            }
            else if (UniqueData.InstrManufacture == 2)
            {
                if (AutoRBW == true) WriteString(":SENS:BWID:RES:AUTO 1");
                if (AutoRBW == false) WriteString(":SENS:BWID:RES:AUTO 0");
            }
            else if (UniqueData.InstrManufacture == 3)
            {
                if (AutoRBW == true) WriteString(":SENSe:BWIDth:RESolution:AUTO 1");
                if (AutoRBW == false) WriteString(":SENSe:BWIDth:RESolution:AUTO 0");
            }
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
            if (UniqueData.InstrManufacture == 1)
            {
                string s = QueryString(":SENSe:BWIDth:RESolution:AUTO?");
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
                AutoRBW = Boolean.Parse(QueryString(":SENSe:BWIDth:RESolution:AUTO?"));
            }
            if (AutoSweepTime == true) { GetSweepTime(); }
        }

        /// <summary>
        /// Установка VBW 
        /// </summary>
        private void SetVBW(decimal vbw)
        {
            VBW = vbw;
            if (UniqueData.InstrManufacture == 1)
            {
                WriteString(":SENSe:BANDwidth:VIDeo " + VBW.ToString().Replace(decimalSeparator, "."));
            }
            else if (UniqueData.InstrManufacture == 2)
            {
                WriteString(":SENSe:BANDwidth:VIDeo " + VBW.ToString().Replace(decimalSeparator, "."));
            }
            else if (UniqueData.InstrManufacture == 3)
            {
                WriteString(":SENSe:BANDwidth:VIDeo " + VBW.ToString().Replace(decimalSeparator, "."));
            }
            AutoVBW = false;
            if (AutoSweepTime == true && RBWIndex - VBWIndex > 0) { GetSweepTime(); }
            GetSweepType();
        }
        /// <summary>
        /// Получение VBW 
        /// </summary>
        private void GetVBW()
        {
            if (UniqueData.InstrManufacture == 1)
            {
                VBW = QueryDecimal(":SENSe:BANDwidth:VIDeo?");
                VBWIndex = System.Array.IndexOf(UniqueData.VBWArr, VBW);
            }
            else if (UniqueData.InstrManufacture == 2)
            {
                VBW = QueryDecimal(":SENS:BAND:VID?");
                VBWIndex = System.Array.IndexOf(UniqueData.VBWArr, VBW);
            }
            else if (UniqueData.InstrManufacture == 3)
            {
                VBW = QueryDecimal(":SENSe:BANDwidth:VIDeo?");
                VBWIndex = System.Array.IndexOf(UniqueData.VBWArr, VBW);
            }
        }
        /// <summary>
        /// Вкл Auto RBW 
        /// </summary>
        private void SetAutoVBW()
        {
            if (UniqueData.InstrManufacture == 1)
            {
                if (AutoVBW) WriteString(":SENSe:BANDwidth:VIDeo:AUTO 1");
                else WriteString(":SENSe:BANDwidth:VIDeo:AUTO 0");
            }
            else if (UniqueData.InstrManufacture == 2)
            {
                if (AutoVBW) WriteString(":SENS:BAND:VID:AUTO 1");
                else WriteString(":SENS:BAND:VID:AUTO 0");
            }
            else if (UniqueData.InstrManufacture == 3)
            {
                if (AutoVBW) WriteString(":SENSe:BWIDth:VIDeo:AUTO 1");
                else WriteString(":SENSe:BWIDth:VIDeo:AUTO 0");
            }
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
            if (UniqueData.InstrManufacture == 1)
            {
                if (QueryString(":SENSe:BANDwidth:VIDeo:AUTO?").Contains("1")) { AutoVBW = true; }
                else { AutoVBW = false; }
            }
            else if (UniqueData.InstrManufacture == 2)
            {
                if (QueryString(":SENS:BAND:VID:AUTO?").Contains("0")) { AutoVBW = false; }
                else { AutoVBW = true; }
            }
            else if (UniqueData.InstrManufacture == 3)
            {
                AutoVBW = Boolean.Parse(QueryString(":SENSe:BWIDth:VIDeo:AUTO?"));
            }
        }


        /// <summary>
        /// Установка метода свипирования 
        /// </summary>        
        private void SetSweepType()
        {
            if (UniqueData.InstrManufacture == 1)
            {
                WriteString(":SWE:TYPE " + SweepTypeSelected.Parameter);
            }
            else if (UniqueData.InstrManufacture == 2)
            {
                WriteString(":SWE:TYPE " + SweepTypeSelected.Parameter);
            }
            else if (UniqueData.InstrManufacture == 3)
            {
                WriteString(":SWE:MODE " + SweepTypeSelected.Parameter);
            }
        }
        /// <summary>
        /// Получаем метод свиптайма 
        /// </summary> 
        private void GetSweepType()
        {
            if (UniqueData.InstrManufacture == 1)
            {
                if (UniqueData.HiSpeed == true)
                {
                    string t = QueryString(":SWE:TYPE?");
                    foreach (ParamWithId ST in UniqueData.SweepType)
                    {
                        if (t.TrimEnd().ToLower() == ST.Parameter.ToLower()) { SweepTypeSelected = ST; }
                    }
                }
                else { SweepTypeSelected = new ParamWithId { Id = (int)EN.SweepType.FFT, Parameter = "FFT" }; }
            }
            else if (UniqueData.InstrManufacture == 2)
            {
                string temp = QueryString(":SWE:TYPE?");
                foreach (ParamWithId ST in UniqueData.SweepType)
                {
                    if (temp.Contains(ST.Parameter)) { SweepTypeSelected = ST; }
                }
            }
            else if (UniqueData.InstrManufacture == 3)
            {
                string temp = QueryString(":SWE:MODE?");
                foreach (ParamWithId ST in UniqueData.SweepType)
                {
                    if (temp.Contains(ST.Parameter)) { SweepTypeSelected = ST; }
                }
            }
        }
        /// <summary>
        /// Получаем свиптайм 
        /// </summary>
        private void GetSweepTime()
        {
            if (UniqueData.InstrManufacture == 1)
            {
                SweepTime = QueryDecimal(":SWE:TIME?");
            }
            else if (UniqueData.InstrManufacture == 2)
            {
                SweepTime = QueryDecimal(":SWE:MTIM?");
            }
            else if (UniqueData.InstrManufacture == 3)
            {
                SweepTime = QueryDecimal(":SWE:TIME:ACT?");
            }
        }
        private void SetSweepTime(decimal sweeptime)
        {
            SweepTime = sweeptime;
            if (UniqueData.InstrManufacture == 1)
            {
                WriteString(":SWE:TIME " + SweepTime.ToString());
                SweepTime = QueryDecimal(":SWE:TIME?");
                AutoSweepTime = false;
            }
            else if (UniqueData.InstrManufacture == 2)
            {
                WriteString(":SWE:TIME " + SweepTime.ToString().Replace(',', '.'));
                SweepTime = QueryDecimal(":SWE:TIME?");
                AutoSweepTime = false;
            }
            else if (UniqueData.InstrManufacture == 3)
            {
                WriteString(":SWE:TIME:ACT " + SweepTime.ToString().Replace(',', '.'));
                SweepTime = QueryDecimal(":SWE:TIME:ACT?");
                AutoSweepTime = false;
            }
        }
        private void SetAutoSweepTime(bool autosweeptime)
        {
            AutoSweepTime = autosweeptime;
            if (UniqueData.InstrManufacture == 1)
            {
                if (AutoSweepTime) WriteString(":SWE:TIME:AUTO 1");
                else WriteString(":SWE:TIME:AUTO 0");
            }
            else if (UniqueData.InstrManufacture == 2)
            {
                if (AutoSweepTime) WriteString(":SENS:SWE:ACQ:AUTO 1");
                else WriteString(":SENS:SWE:ACQ:AUTO 0");
            }
            else if (UniqueData.InstrManufacture == 3)
            {
                if (AutoSweepTime) WriteString(":SENS:SWE:AUTO ON");
                else WriteString(":SENS:SWE:AUTO OFF");
            }
            GetSweepTime();
        }
        /// <summary>
        /// Получаем состояние Auto SWT 
        /// </summary>
        private void GetAutoSweepTime()
        {
            if (UniqueData.InstrManufacture == 1)
            {
                if (QueryString(":SWE:TIME:AUTO?").Contains("0")) { AutoSweepTime = false; }
                else { AutoSweepTime = true; }
            }
            else if (UniqueData.InstrManufacture == 2)
            {
                if (QueryString(":SENS:SWE:ACQ:AUTO?").Contains("0")) { AutoSweepTime = false; }
                else { AutoSweepTime = true; }
            }
        }

        private void SetSweepPoints(int sweeppoints)
        {
            SweepPoints = sweeppoints;
            if (UniqueData.InstrManufacture == 1)
            {
                WriteString(":SWE:POIN " + SweepPoints.ToString());
            }
            else if (UniqueData.InstrManufacture == 2)
            {
                WriteString(":SENS:SWE:POIN " + SweepPoints.ToString());
            }
            //session.DefaultBufferSize = SweepPoints * 4 + SweepPoints.ToString().Length + 100;
            GetFreqArr();
        }


        /// <summary>
        /// Получаем количевства точек свипов 
        /// </summary>
        private void GetSweepPoints()
        {
            if (UniqueData.InstrManufacture == 1)
            {
                if (UniqueData.SweepPointFix == false)
                {
                    SweepPoints = QueryInt(":SWE:POIN?");
                    SweepPointsIndex = System.Array.IndexOf(UniqueData.SweepPointArr, SweepPoints);
                }
                if (UniqueData.HiSpeed == true) { TracePoints = SweepPoints + 2; }
                if (UniqueData.HiSpeed == false) { TracePoints = SweepPoints; }
            }
            else if (UniqueData.InstrManufacture == 2)
            {
                SweepPoints = QueryInt(":SENS:SWE:POIN?");
                SweepPointsIndex = System.Array.IndexOf(UniqueData.SweepPointArr, SweepPoints);
                //session.DefaultBufferSize = SweepPoints * 4 + 20;
                TracePoints = SweepPoints;
            }
            GetFreqArr();
        }



        private void SetOptimization(EN.Optimization optimization)
        {
            if (UniqueData.OptimizationAvailable)
            {
                Optimization = optimization;
                if (UniqueData.InstrManufacture == 1)
                {
                    WriteString("SENSe:SWEep:OPTimize " + OptimizationSelected.Parameter);
                }
                else if (UniqueData.InstrManufacture == 2)
                {

                }
                else if (UniqueData.InstrManufacture == 3)
                {

                }
            }
        }
        private void GetOptimization()
        {
            if (UniqueData.OptimizationAvailable)
            {
                if (UniqueData.InstrManufacture == 1)
                {
                    string temp1 = string.Empty;
                    temp1 = QueryString("SENSe:SWEep:OPTimize?");
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

        private void SetTraceType(ParamWithId trace1type)
        {
            Trace1Type = trace1type;
            if (UniqueData.InstrManufacture == 1)
            {
                WriteString(":DISP:TRAC1:MODE " + Trace1Type.Parameter);
            }
            else if (UniqueData.InstrManufacture == 2)
            {
                WriteString(":TRAC1:TYPE " + Trace1Type.Parameter);
            }
            else if (UniqueData.InstrManufacture == 3)
            {
                WriteString(":TRACe1:OPERation " + Trace1Type.Parameter);
            }
        }
        private void GetTraceType()
        {
            if (UniqueData.InstrManufacture == 1)
            {
                string temp1 = string.Empty;
                if (UniqueData.HiSpeed == true)
                {
                    if (QueryString(":DISP:TRAC1?").Contains("1")) { temp1 = QueryString(":DISP:TRAC1:MODE?"); }
                    else { Trace1Type = new ParamWithId { Id = (int)EN.TraceType.Blank, Parameter = "BLAN" }; }
                }
                else if (UniqueData.HiSpeed == false)
                {
                    temp1 = QueryString(":DISP:TRAC1:MODE?");
                }
                foreach (ParamWithId TT in UniqueData.TraceType)
                {
                    if (temp1.Contains(TT.Parameter)) { Trace1Type = TT; }
                }
            }
            else if (UniqueData.InstrManufacture == 2)
            {
                string temp1 = QueryString(":TRACe1:TYPE?");
                foreach (ParamWithId TT in UniqueData.TraceType)
                {
                    if (temp1.Contains(TT.Parameter)) { Trace1Type = TT; }
                }
            }
            else if (UniqueData.InstrManufacture == 3)
            {
                string temp1 = QueryString(":TRACe1:OPERation?");
                foreach (ParamWithId TT in UniqueData.TraceType)
                {
                    if (temp1.Contains(TT.Parameter)) { Trace1Type = TT; }
                }
            }
        }
        private void SetDetectorType(ParamWithId trace1detector)
        {
            Trace1Detector = trace1detector;
            if (UniqueData.InstrManufacture == 1)
            {
                if (Trace1Detector.Parameter == "Auto Select") WriteString(":SENSe:DETector1:AUTO 1");
                else
                {
                    WriteString(":SENSe:DETector1 " + Trace1Detector.Parameter);
                }

            }
            else if (UniqueData.InstrManufacture == 2)
            {
                WriteString(":SENSe:DETector:FUNCtion " + Trace1Detector.Parameter);
            }
            else if (UniqueData.InstrManufacture == 3)
            {
                if (Trace1Detector.Parameter == "Auto Select") WriteString(":SENSe:DETector1:AUTO 1");
                else
                {
                    WriteString(":SENSe:DETector:FUNCtion " + Trace1Detector.Parameter);
                }
            }
        }
        /// <summary>
        /// Получение типа Trace Detecror 
        /// </summary>
        private void GetDetectorType()
        {
            if (UniqueData.InstrManufacture == 1)
            {

                string temp1 = string.Empty;
                if (UniqueData.HiSpeed == true)
                {
                    temp1 = QueryString(":SENSe:DETector1?");
                }
                else if (UniqueData.HiSpeed == false)
                {
                    temp1 = QueryString(":SENSe:DETector1?");
                }
                foreach (ParamWithId TT in UniqueData.TraceDetector)
                {
                    if (temp1.Contains(TT.Parameter)) { Trace1Detector = TT; }
                }
            }
            else if (UniqueData.InstrManufacture == 2)
            {
                string temp1 = QueryString(":SENSe:DETector:FUNCtion?");
                foreach (ParamWithId TT in UniqueData.TraceDetector)
                {
                    if (temp1.Contains(TT.Parameter)) { Trace1Detector = TT; }
                }
            }
            else if (UniqueData.InstrManufacture == 3)
            {
                string temp1 = QueryString(":SENSe:DETector:FUNCtion?");
                foreach (ParamWithId TT in UniqueData.TraceDetector)
                {
                    if (temp1.Contains(TT.Parameter)) { Trace1Detector = TT; }
                }
            }
        }




        private void SetIQBW(decimal iqbw)
        {
            IQBW = iqbw;
            if (UniqueData.InstrManufacture == 1)
            {
                SampleSpeed = 1.25m * IQBW;
                SampleTimeLength = 1 / SampleSpeed;
                string s = "TRAC:IQ:BWID " + IQBW.ToString().Replace(decimalSeparator, ".");
                WriteString(s);
            }
            else if (UniqueData.InstrManufacture == 2)
            {

            }
            else if (UniqueData.InstrManufacture == 3)
            {

            }
        }
        private void SetSampleSpeed(decimal samplespeed)
        {
            SampleSpeed = samplespeed;
            if (UniqueData.InstrManufacture == 1)
            {
                IQBW = 0.8m * SampleSpeed;
                SampleTimeLength = 1 / SampleSpeed;
                WriteString("TRAC:IQ:SRAT " + SampleSpeed.ToString().Replace(decimalSeparator, "."));
            }
            else if (UniqueData.InstrManufacture == 2)
            {
            }
            else if (UniqueData.InstrManufacture == 3)
            {
            }
        }
        private void SetSampleLength(int samplelength)
        {
            SampleLength = samplelength;
            if (UniqueData.InstrManufacture == 1)
            {
                WriteString("TRAC:IQ:RLEN " + SampleLength.ToString());
            }
            else if (UniqueData.InstrManufacture == 2)
            {
            }
            else if (UniqueData.InstrManufacture == 3)
            {
            }
        }

        private void SetTriggerOffset(decimal triggeroffset)
        {
            TriggerOffset = triggeroffset;
            if (UniqueData.InstrManufacture == 1)
            {
                if (Math.Abs(TriggerOffset) < UniqueData.TriggerOffsetMax)
                {
                    WriteString("TRIG:HOLD " + TriggerOffset.ToString().Replace(decimalSeparator, "."));
                }
            }
            else if (UniqueData.InstrManufacture == 2)
            {
            }
            else if (UniqueData.InstrManufacture == 3)
            {
            }
        }
        private void SetTriggerOffsetAndSampleLengthAndStartCollection(decimal triggeroffset, int samplelength)
        {
            TriggerOffset = triggeroffset;
            SampleLength = samplelength;
            if (UniqueData.InstrManufacture == 1)
            {
                if (Math.Abs(TriggerOffset) < UniqueData.TriggerOffsetMax)
                {
                    int length = (SampleLength * 4) * 2 + SampleLength.ToString().Length + 100;
                    //if (session.DefaultBufferSize != length)
                    //{
                    //    session.DefaultBufferSize = length;
                    //}
                    WriteString("TRIG:HOLD " + TriggerOffset.ToString().Replace(decimalSeparator, ".") + ";:TRAC:IQ:RLEN " + SampleLength.ToString() + ";:INIT;*WAI;");

                    //session.Write("INIT;*WAI;");
                }
            }
            else if (UniqueData.InstrManufacture == 2)
            {
            }
            else if (UniqueData.InstrManufacture == 3)
            {
            }
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
                        WriteString(":TRAC:DATA? TRACE1;:STAT:QUES:POW?");

                        byte[] byteArray = ReadByte();
                        string pow = ReadString().Replace(";", "");

                        int pr = int.Parse(pow);
                        if (pr == 0 || pr == 2) { PowerRegister = EN.PowerRegister.Normal; }
                        else if (pr == 4) { PowerRegister = EN.PowerRegister.IFOverload; }
                        else if (pr == 1) { PowerRegister = EN.PowerRegister.RFOverload; }//правильно

                        temp = new float[byteArray.Length / 4];
                        for (int j = 0; j < byteArray.Length / 4; j++)
                        {
                            temp[j] = System.BitConverter.ToSingle(byteArray, j * 4);
                        } 
                    }
                    else
                    {
                        //WriteString("TRAC:DATA? TRACE1");
                        //byte[] byteArray = ReadByte();
                        //temp = new float[byteArray.Length / 4];
                        //for (int j = 0; j < temp.Length / 4; j++)
                        //{
                        //    temp[j] = System.BitConverter.ToSingle(byteArray, j * 4);
                        //}
                        temp = QueryFloat("TRAC:DATA? TRACE1");
                    }
                }
                else if (UniqueData.InstrManufacture == 2)
                {
                    temp = QueryFloat("TRAC1:DATA?");
                }
                else if (UniqueData.InstrManufacture == 3)
                {
                    temp = QueryFloat("TRAC1:DATA?");
                }

                if (temp.Length > 0)
                {
                    //т.к. используем только ClearWhrite то проверяем полученный трейс с предыдущим временным на предмет полного отличия и полноценен он или нет
                    if (!float.IsNaN(temp[0]) && temp[0] % 1 != 0 &&
                        !float.IsNaN(temp[temp.Length - 1]) && temp[temp.Length - 1] % 1 != 0)
                    {
                        for (int i = 0; i < temp.Length; i++)
                        {
                            if (LevelArrTemp[i] == temp[i])
                            {
                                newdata = false;
                                break;
                            }
                        }
                    }
                    else
                    {
                        newdata = false;
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
                    TraceAveraged.AddTraceToAverade(resFreqStart, resFreqStep, trace, (MEN.LevelUnit)LevelUnits.Id, LevelUnitsResult);
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


        private bool GetIQStream(ref COMR.MesureIQStreamResult result, COM.MesureIQStreamCommand command)
        {
            bool res = false;
            if (UniqueData.InstrManufacture == 1)
            {
#if DEBUG
                long delta = _timeService.TimeStamp.Ticks;
#endif
                long time = command.Parameter.TimeStart;// _timeService.GetGnssTime().Ticks;
                long NextSecond = (time / 10000000) * 10000000 + 10000000;
                long ToNextSecond = NextSecond - time;

                //IQMeasTime
                decimal delay = Math.Abs(((decimal)ToNextSecond) / 10000000);// - 0.03m;// - 0.5m; //время насколько раньше тригерра будут собранны данные всегда отрицательное
                //ищем ближайшее целое по отношени к длительности семпла
                int divisor = -1 + (int)Math.Floor((0 - delay) / SampleTimeLength);


                IQMeasTimeAll = (decimal)command.Parameter.IQReceivTime_s;
                IQMeasTime = (decimal)command.Parameter.IQBlockDuration_s;
                SampleLength = (int)(SampleSpeed * IQMeasTimeAll);
                SetTriggerOffsetAndSampleLengthAndStartCollection(divisor * SampleTimeLength, (int)(SampleSpeed * IQMeasTimeAll));
#if DEBUG
                delta = _timeService.TimeStamp.Ticks - delta;
#endif
                if (command.Parameter.TimeStart < _timeService.GetGnssUtcTime().Ticks - UTCOffset)
                {
                    //Debug.WriteLine("ВСе плохо");
                }
                //Debug.WriteLine(new TimeSpan(command.Parameter.TimeStart + UTCOffset - _timeService.GetGnssUtcTime().Ticks).ToString());
                int sleep = (int)((IQMeasTimeAll + Math.Abs(divisor * SampleTimeLength)) * 1000);
                if (sleep < 1)
                {
                    sleep = 1;
                }
                Thread.Sleep(sleep);



                int step = SampleLength / 10;
                if (step > 5000000)
                {
                    step = 5000000;
                }
                else if (step < 5000000)
                {
                    step = 5000000;
                }
#if DEBUG
                long delta2 = _timeService.TimeStamp.Ticks;
#endif
                float[] temp = new float[SampleLength * 2];
                long ddd = _timeService.TimeStamp.Ticks;
                int pos = 0;
                for (int i = 0; i < SampleLength; i += step)
                {
                    int from = i, to = i + step;
                    if (to > SampleLength) to = SampleLength;
                    WriteString($"TRAC:IQ:DATA:MEM? {from},{to - from}");


                    byte[] byteArray = session.FormattedIO.ReadBinaryBlockOfByte();// ReadByte();

                    float[] temp2 = new float[byteArray.Length / 4];

                    for (int j = 0; j < temp2.Length; j++)
                    {
                        temp2[j] = System.BitConverter.ToSingle(byteArray, j * 4);
                    }
                    Array.Copy(temp2, 0, temp, pos, temp2.Length);
                    pos += temp2.Length;

                }
#if DEBUG
                delta2 = _timeService.TimeStamp.Ticks - delta2;
                Debug.WriteLine("delta " + (new TimeSpan(delta)).ToString());
                Debug.WriteLine("delta2 " + (new TimeSpan(delta2)).ToString());
                Debug.WriteLine("delta++ " + (new TimeSpan(delta2 + delta)).ToString());
#endif
                string dfghkjdp = QueryString("TRACe:IQ:TPISample?");
                TriggerOffsetInSample = DecimalParse(dfghkjdp);
                //Посчитаем когда точно был триггер относительно первого семпла
                TriggerOffset = Math.Abs(TriggerOffset) + TriggerOffsetInSample;



                float noise = 1.5f / 10000000f; // уровень шума 
                float SN = 10; // превышение шума в разах 
                float TrigerLevelPL = noise * SN;
                float TrigerLevelMN = 0 - TrigerLevelPL;
                int IQStartIndex = 0;
                int IQStopIndex = temp.Length;
                bool SignalFound = false; //был ли сигнал
                int stepf = temp.Length / 1000;//шаг проверки уровней на предмет детектирования сигнала
                if (stepf < 1)
                {
                    stepf = 1;
                }
                if (command.Parameter.MandatorySignal)
                {
                    for (int j = 0; temp.Length - 6 > j; j += stepf)
                    {
                        if (temp[j] >= TrigerLevelPL || temp[j + 1] >= TrigerLevelPL ||
                            temp[j] <= TrigerLevelMN || temp[j + 1] <= TrigerLevelMN)
                        {
                            if (temp[j + 2] >= TrigerLevelPL || temp[j + 3] >= TrigerLevelPL ||
                                temp[j + 2] <= TrigerLevelMN || temp[j + 3] <= TrigerLevelMN)
                            {
                                if (temp[j + 4] >= TrigerLevelPL || temp[j + 5] >= TrigerLevelPL ||
                                    temp[j + 4] <= TrigerLevelMN || temp[j + 5] <= TrigerLevelMN)
                                {
                                    SignalFound = true;//Есть сигнал 
                                    IQStartIndex = j - (int)(((double)stepf) * 3.0);
                                    if (IQStartIndex % 2 != 0) IQStartIndex = (IQStartIndex / 2) * 2 + 2;
                                    if (IQStartIndex < 0)
                                        IQStartIndex = 0;

                                    break;
                                }
                            }
                        }
                    }
                    if (!SignalFound) //Хотели сигнал но его небыло, согласно договоренности генерируем екзепшен
                    {
                        throw new Exception("Signal not detected");
                    }
                }

                int dddddddd = 2 * (int)Math.Ceiling(((decimal)command.Parameter.IQBlockDuration_s) / SampleTimeLength);
                if (dddddddd % 2 != 0) dddddddd = (dddddddd / 2) * 2 + 2;
                IQStopIndex = IQStartIndex + dddddddd;
                if (IQStopIndex > temp.Length) IQStopIndex = temp.Length;
                result.OneSempleDuration_ns = (long)(SampleTimeLength * 1000000000);
                result.PPSTimeDifference_ns = (long)(TriggerOffset * 1000000000 - IQStartIndex * SampleTimeLength * 500000000);
                result.TimeStamp = (NextSecond * 100 - result.PPSTimeDifference_ns) / 100;
                TriggerOffset = ((decimal)result.PPSTimeDifference_ns) / 1000000000;
                result.iq_samples = new float[1][];
                result.iq_samples[0] = new float[IQStopIndex - IQStartIndex];

                Array.Copy(temp, IQStartIndex, result.iq_samples[0], 0, IQStopIndex - IQStartIndex);
                //result.iq_samples[0] = temp;
                IQArr = result.iq_samples[0];

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
            if (UniqueData.InstrManufacture == 1)
            {
                string str = "INST:CRE IQ, 'IQ Analyzer'";
                WriteString(str);
                WriteString("INIT:CONT OFF");
                WriteString("TRIG:SOUR EXT");
                WriteString("TRACe:IQ:DATA:FORMat IQPair");
            }
        }

        public void SetWindowType(EN.Mode mode)
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
                                     //session.DefaultBufferSize = SweepPoints * 18 + 25;
                }
                WriteString(str);
                WriteString("FORM:DATA REAL,32");//так надо
                                                 //string res = session.Query(str);
            }
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
            if (UniqueData.InstrManufacture == 1)
            {
                if (UniqueData.Battery)
                {
                    string[] st = QueryString(":STAT:QUES:POW?;:SYSTem:POWer:STATus?").Split(';');
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
                string t = QueryString(":SYST:BATT:ARTT?");
                //BatteryAbsoluteCharge = int.Parse(session.Query(":SYST:BATT:ACUR?").Replace('.', ','));
                //System.Windows.MessageBox.Show(t);
            }
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
                    WriteString(":SYSTem:PRES");
                }
                else if (UniqueData.InstrManufacture == 2)
                {
                    //session.Write(":SYSTem:PRES");
                }
                else if (UniqueData.InstrManufacture == 3)
                {
                    WriteString(":SYSTem:PRES");
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
                    string[] d = QueryString("SYST:DATE?").TrimEnd().Trim(' ').Split(',');
                    string[] t = QueryString("SYST:TIME?").TrimEnd().Trim(' ').Split(',');
                    string time = d[0] + "-" + d[1] + "-" + d[2] + " " +
                        t[0] + ":" + t[1] + ":" + t[2];
                    DateTime andt = DateTime.Parse(time);
                    if (new TimeSpan(DateTime.Now.Ticks - andt.Ticks) > new TimeSpan(0, 0, 1, 0, 0))
                    {
                        WriteString("SYST:DATE " + DateTime.Now.Year.ToString() + "," + DateTime.Now.Month.ToString() + "," + DateTime.Now.Day.ToString());
                        WriteString("SYST:TIME " + DateTime.Now.Hour.ToString() + "," + DateTime.Now.Minute.ToString() + "," + DateTime.Now.Second.ToString());
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


        #endregion Private Method
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


    }

}
