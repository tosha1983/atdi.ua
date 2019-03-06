﻿using Atdi.Contracts.Sdrn.DeviceServer;
using Atdi.DataModels.Sdrn.DeviceServer;
using Atdi.Platform.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using COM = Atdi.DataModels.Sdrn.DeviceServer.Commands;
using COMR = Atdi.DataModels.Sdrn.DeviceServer.Commands.Results;
using EN = Atdi.AppUnits.Sdrn.DeviceServer.Adapters.SignalHound.Enums;
using MEN = Atdi.DataModels.Sdrn.DeviceServer.Adapters.Enums;

namespace Atdi.AppUnits.Sdrn.DeviceServer.Adapters.SignalHound
{
    /// <summary>
    /// Пример реализации объект аадаптера
    /// </summary>
    public class Adapter : IAdapter
    {
        private readonly ITimeService _timeService;
        private readonly ILogger _logger;
        private readonly AdapterConfig _adapterConfig;
        private LocalParametersConverter LPC;
        /// <summary>
        /// Все объекты адаптера создаются через DI-контейнер 
        /// Запрашиваем через конструктор необходимые сервисы
        /// </summary>
        /// <param name="adapterConfig"></param>
        /// <param name="logger"></param>
        public Adapter(AdapterConfig adapterConfig, ILogger logger)
        {
            this._logger = logger;
            this._adapterConfig = adapterConfig;
            LPC = new LocalParametersConverter();
            FreqArr = new double[TracePoints];
            LevelArr = new float[TracePoints];
            for (int i = 0; i < TracePoints; i++)
            {
                FreqArr[i] = (double)(FreqStart + FreqStep * i);
                LevelArr[i] = -100;
            }
        }
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
                FreqArr[i] = (double)(FreqStart + FreqStep * i);
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
                Status = AdapterDriver.bbOpenDevice(ref _Device_ID);
                if (Status != EN.Status.NoError)
                {
                    throw new Exception("Error: Unable to open BB60. Status:" + AdapterDriver.bbGetStatusString(Status));
                }
                else
                {
                    Device_Type = AdapterDriver.bbGetDeviceName(_Device_ID);
                    Device_SerialNumber = AdapterDriver.bbGetSerialString(_Device_ID);
                    Device_APIVersion = AdapterDriver.bbGetAPIString();
                    Device_FirmwareVersion = AdapterDriver.bbGetFirmwareString(_Device_ID);


                    GetSystemInfo();
                    SetTraceDetectorAndScale();
                    SetFreqCentrSpan();
                    SetRefATT();
                    SetGain();
                    SetRbwVbwSweepTimeRbwType();
                    SetPortType();
                    Status = AdapterDriver.bbInitiate(_Device_ID, (uint)DeviceMode, 0);
                    IsRuning = true;
                    IdleState = true;
                    /// включем устройство
                    /// иницируем его параметрами сконфигурации
                    /// проверяем к чем оно готово

                    /// сообщаем инфраструктуре что мы готовы обрабатывать комманду MesureGpsLocationExampleCommand
                    /// и при этом возвращать оезультат в типе MesureGpsLocationExampleAdapterResult                   
                    StandardDeviceProperties sdp = new StandardDeviceProperties()
                    {
                        AttMax_dB = 30,
                        AttMin_dB = 0,
                        FreqMax_Hz = FreqMax,
                        FreqMin_Hz = FreqMin,
                        PreAmpMax_dB = 30,
                        PreAmpMin_dB = 0,
                        RefLevelMax_dBm = 20,
                        RefLevelMin_dBm = -130,
                        EquipmentInfo = new EquipmentInfo()
                        {
                            AntennaCode = "Omni",//S/N  В конфиг
                            AntennaManufacturer = "3anet",//В конфиг
                            AntennaName = "BC600",//В конфиг
                            EquipmentManufacturer = new Atdi.DataModels.Sdrn.DeviceServer.Adapters.InstrManufacrures().SignalHound.UI,
                            EquipmentName = Device_Type,
                            EquipmentFamily = "SDR",//SDR/SpecAn/MonRec
                            EquipmentCode = Device_SerialNumber,//S/N

                        },
                        RadioPathParameters = new RadioPathParameters[]
                        {
                            new RadioPathParameters()
                            {
                                Freq_Hz = 1*1000000,
                                KTBF_dBm = -147,//уровень своих шумов на Гц
                                FeederLoss_dB = 2,//потери фидера
                                Gain= 10, //коэф усиления
                                DiagA = "HV",
                                DiagH = "POINT 0 0 90 3 180 6 270 3",//от нуля В конфиг
                                DiagV = "POINT -90 20 0 0 90 10"//от -90  до 90 В конфиг
                            },
                            new RadioPathParameters()
                            {
                                Freq_Hz = 1000*1000000,
                                KTBF_dBm = -147,//уровень своих шумов на Гц
                                FeederLoss_dB = 2,//потери фидера
                                Gain= 10, //коэф усиления
                                DiagA = "HV",
                                DiagH = "POINT 0 0 90 3 180 6 270 3",//от нуля В конфиг
                                DiagV = "POINT -90 20 0 0 90 10"//от -90  до 90 В конфиг
                            }
                        }
                    };
                    MesureTraceDeviceProperties mtdp = new MesureTraceDeviceProperties()
                    {
                        RBWMax_Hz = (double)RBWMax,
                        RBWMin_Hz = 3,
                        SweepTimeMin_s = (double)SweepTimeMin,
                        SweepTimeMax_s = (double)SweepTimeMax,
                        StandardDeviceProperties = sdp,
                        //DeviceId ничего не писать, ID этого экземпляра адаптера
                    };
                    host.RegisterHandler<COM.MesureTraceCommand, COMR.MesureTraceResult>(MesureTraceCommandHandler, mtdp);

                    MesureIQStreamDeviceProperties miqdp = new MesureIQStreamDeviceProperties()
                    {
                        AvailabilityPPS = true, // В конфиг
                        BitRateMax_MBs = 40,
                        //DeviceId ничего не писать, ID этого экземпляра адаптера
                        standartDeviceProperties = sdp,
                    };
                    host.RegisterHandler<COM.MesureIQStreamCommand, COMR.MesureIQStreamResult>(MesureIQStreamCommandHandler, miqdp);
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
        /// Метод вызывается контрллером когда необходимо выгрузит адаптер с памяти
        /// </summary>
        public void Disconnect()
        {
            try
            {
                /// освобождаем ресурсы и отключаем устройство
                IsRuning = false;
                Status = AdapterDriver.bbCloseDevice(_Device_ID);
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

                    // важно: если измерение идет в поток еадаптера то в принцпе в явных блокировках смысла нет - адаптер полностью занят и другие кломаныд обработаить не сможет
                    // функции блокировки имеют смысл если мы для измерений создает отдельные потоки а этот освобождаем для прослушивани яследующих комманд

                    // сценарйи в данном случаи за разарбочиком адаптера

                    // что то меряем
                    if (IdleState)
                    {
                        Status = AdapterDriver.bbAbort(_Device_ID);
                        if (Status != EN.Status.NoError)
                        {
                            _logger.Warning(Contexts.ThisComponent, AdapterDriver.bbGetStatusString(Status));
                        }
                        IdleState = false;
                    }
                    if (FreqStart != command.Parameter.FreqStart_Hz || FreqStop != command.Parameter.FreqStop_Hz)
                    {
                        FreqStart = LPC.FreqStart(this, command.Parameter.FreqStart_Hz);
                        FreqStop = LPC.FreqStop(this, command.Parameter.FreqStop_Hz);
                        Status = AdapterDriver.bbConfigureCenterSpan(_Device_ID, (double)FreqCentr, (double)FreqSpan);
                        if (Status != EN.Status.NoError)
                        {
                            _logger.Warning(Contexts.ThisComponent, AdapterDriver.bbGetStatusString(Status));
                        }
                    }

                    EN.Attenuator att = LPC.Attenuator(command.Parameter.Att_dB);
                    if (Attenuator != att || RefLevel != command.Parameter.RefLevel_dBm)
                    {
                        Attenuator = att;
                        if (command.Parameter.RefLevel_dBm == -1)
                        {
                            RefLevel = -40;
                        }
                        else
                        {
                            RefLevel = command.Parameter.RefLevel_dBm;
                        }
                        
                        Status = AdapterDriver.bbConfigureLevel(_Device_ID, RefLevel, (double)Attenuator);
                        if (Status != EN.Status.NoError)
                        {
                            _logger.Warning(Contexts.ThisComponent, AdapterDriver.bbGetStatusString(Status));
                        }
                    }

                    EN.Gain gain = LPC.Gain(command.Parameter.PreAmp_dB);
                    if (gain != Gain)
                    {
                        Gain = LPC.Gain(command.Parameter.PreAmp_dB);
                        Status = AdapterDriver.bbConfigureGain(_Device_ID, (int)Gain);
                        if (Status != EN.Status.NoError)
                        {
                            _logger.Warning(Contexts.ThisComponent, AdapterDriver.bbGetStatusString(Status));
                        }
                    }

                    if (command.Parameter.RBW_Hz < 0)
                    {
                        int[] ar = new int[] { 1, 2, 4, 8, 16, 32, 64, 128, 256, 512, 1024, 2048, 4096, 8192, 16384, 32768, 65536, 131072, 262144, 524288, 1048576, 2097152, 4194304, 8388608, 16777216, 33554432 };
                        decimal magic = 38.146966101334357m;
                        decimal m1 = FreqSpan / command.Parameter.TracePoint;//хотим
                        int m2 = (int)(m1 / magic);
                        int delta = int.MaxValue, index = 0;
                        for (int i = 0; i < ar.Length; i++)
                        {
                            if (Math.Abs(ar[i] - m2) < delta)
                            {
                                delta = Math.Abs(ar[i] - m2);
                                index = i;
                            }
                        }
                        m2 = ar[index];
                        decimal d = FreqSpan / magic / m2;
                        if (((int)d) < command.Parameter.TracePoint)
                        {
                            m2 = ar[index - 1];
                        }                        
                        decimal rbw = magic * m2 * 4;// (FreqSpan / command.Parameter.TracePoint) * 4.0m;
                        if (rbw > RBWMax)
                        {
                            rbw = RBWMax;
                        }
                        decimal vbw = 0;
                        if (command.Parameter.RBW_Hz < 0)
                        {
                            vbw = rbw;
                        }
                        else
                        {
                            vbw = LPC.VBW(this, (decimal)command.Parameter.VBW_Hz);
                        }
                        RBW = rbw;
                        VBW = vbw;
                        SweepTime = (decimal)command.Parameter.SweepTime_s;
                        Status = AdapterDriver.bbConfigureSweepCoupling(_Device_ID, (double)RBW, (double)VBW, (double)SweepTime, (uint)RBWShape, (uint)Rejection);
                        if (Status != EN.Status.NoError)
                        {
                            _logger.Warning(Contexts.ThisComponent, AdapterDriver.bbGetStatusString(Status));
                        }
                    }
                    else
                    {
                        decimal rbw = LPC.RBW(this, (decimal)command.Parameter.RBW_Hz);
                        decimal vbw = LPC.VBW(this, (decimal)command.Parameter.VBW_Hz);
                        if (RBW != rbw || VBW != vbw || SweepTime != (decimal)command.Parameter.SweepTime_s)
                        {
                            RBW = rbw;
                            VBW = vbw;
                            SweepTime = (decimal)command.Parameter.SweepTime_s;
                            Status = AdapterDriver.bbConfigureSweepCoupling(_Device_ID, (double)RBW, (double)VBW, (double)SweepTime, (uint)RBWShape, (uint)Rejection);
                            if (Status != EN.Status.NoError)
                            {
                                _logger.Warning(Contexts.ThisComponent, AdapterDriver.bbGetStatusString(Status));
                            }
                        }
                    }

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
                    LPC.DetectorType(this, command.Parameter.DetectorType);
                    Status = AdapterDriver.bbConfigureAcquisition(_Device_ID, (uint)DetectorToSet, (uint)Scale);
                    if (Status != EN.Status.NoError)
                    {
                        _logger.Warning(Contexts.ThisComponent, AdapterDriver.bbGetStatusString(Status));
                    }
                    TraceType = LPC.TraceType(command.Parameter.TraceType);
                    LevelUnit = LPC.LevelUnit(command.Parameter.LevelUnit);

                    if (DeviceMode != EN.Mode.Sweeping || FlagMode != EN.Flag.StreamIQ)
                    {
                        DeviceMode = EN.Mode.Sweeping;
                        FlagMode = EN.Flag.StreamIQ;
                    }
                    Status = AdapterDriver.bbInitiate(_Device_ID, (uint)DeviceMode, (uint)FlagMode);
                    if (Status != EN.Status.NoError)
                    {
                        _logger.Warning(Contexts.ThisComponent, AdapterDriver.bbGetStatusString(Status));
                    }
                    IdleState = true;
                    //Меряем
                    //Если TraceType ClearWrite то пушаем каждый результат
                    if (TraceType == EN.TraceType.ClearWrite)
                    {
                        for (ulong i = 0; i < TraceCountToMeas; i++)
                        {
                            if (GetTrace())
                            {
                                // пушаем результат
                                var result = new COMR.MesureTraceResult(TraceCount, CommandResultStatus.Next);
                                TraceCount++;
                                if (TraceCountToMeas == TraceCount)
                                {
                                    result = new COMR.MesureTraceResult(TraceCount, CommandResultStatus.Final);
                                }
                                result.Freq_Hz = new double[FreqArr.Length];
                                result.Level = new float[FreqArr.Length];
                                for (int j = 0; j < FreqArr.Length; j++)
                                {
                                    result.Freq_Hz[j] = FreqArr[j];
                                    result.Level[j] = LevelArr[j];
                                }
                                result.TimeStamp = DateTime.UtcNow.Ticks - new DateTime(1970, 1, 1, 0, 0, 0, System.DateTimeKind.Utc).Ticks;

                                context.PushResult(result);
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
                        if (TraceType == EN.TraceType.Average)//назначим сколько усреднять
                        {
                            TraceAveraged.AveragingCount = (int)TraceCountToMeas;
                        }
                        for (ulong i = 0; i < TraceCountToMeas; i++)
                        {
                            if (GetTrace())
                            {
                                TraceCount++;
                            }
                            // иногда нужно проверять токен окончания работы комманды
                            if (context.Token.IsCancellationRequested)
                            {
                                // все нужно остановиться

                                // если есть порция данных возвращаем ее в обработчки только говрим что поток результатов не законченный и больше уже не будет поступать

                                var result2 = new COMR.MesureTraceResult(TraceCount, CommandResultStatus.Ragged);
                                //Скорее нет результатов
                                //result2.Freq_Hz = new double[FreqArr.Length];
                                //result2.Level = new float[FreqArr.Length];
                                //for (int j = 0; j < FreqArr.Length; j++)
                                //{
                                //    result2.Freq_Hz[j] = FreqArr[j];
                                //    result2.Level[j] = LevelArr[j];
                                //}
                                //result2.TimeStamp = DateTime.UtcNow.Ticks - new DateTime(1970, 1, 1, 0, 0, 0, System.DateTimeKind.Utc).Ticks; //пофиксить
                                context.PushResult(result2);


                                // подтверждаем факт обработки отмены
                                context.Cancel();
                                // освобождаем поток 
                                return;
                            }
                        }
                        if (TraceCountToMeas == TraceCount)
                        {
                            var result = new COMR.MesureTraceResult(0, CommandResultStatus.Final);
                            result.Freq_Hz = new double[FreqArr.Length];
                            result.Level = new float[FreqArr.Length];
                            for (int j = 0; j < FreqArr.Length; j++)
                            {
                                result.Freq_Hz[j] = FreqArr[j];
                                result.Level[j] = LevelArr[j];
                            }
                            result.TimeStamp = DateTime.UtcNow.Ticks - new DateTime(1970, 1, 1, 0, 0, 0, System.DateTimeKind.Utc).Ticks;

                            context.PushResult(result);
                        }
                    }

                    if (IdleState)
                    {
                        Status = AdapterDriver.bbAbort(_Device_ID);
                        if (Status != EN.Status.NoError)
                        {
                            _logger.Warning(Contexts.ThisComponent, AdapterDriver.bbGetStatusString(Status));
                        }
                        IdleState = false;
                    }
                    // снимаем блокировку с текущей команды
                    context.Unlock();

                    // что то делаем еще 


                    // подтверждаем окончание выполнения комманды 
                    // важно: всн ранее устапнволеные в контексте обработки текущей команыд блокировки снимаются автоматически
                    context.Finish();
                    // дальше кода быть не должно, освобождаем поток
                }
                else
                {
                    throw new Exception("The device with serial number " + Device_SerialNumber + " does not work");
                }
            }
            catch (Exception e)
            {
                try//знаю что плохо но нужно освободить железо если че
                {
                    if (IdleState)
                    {
                        Status = AdapterDriver.bbAbort(_Device_ID);
                        if (Status != EN.Status.NoError)
                        {
                            _logger.Warning(Contexts.ThisComponent, AdapterDriver.bbGetStatusString(Status));
                        }
                        IdleState = false;
                    }
                }
                catch { }
                // желательно записать влог
                _logger.Exception(Contexts.ThisComponent, e);
                // этот вызов обязательный в случаи обрыва
                context.Abort(e);
                // дальше кода быть не должно, освобождаем поток
            }

        }
        /// <summary>
        /// Получает IQ Данные
        /// 
        ///
        /// Пример
        /// var command = new CMD.MesureIQStreamCommand();
        /// command.Parameter.FreqStart_Hz = 424.625m * 1000000;//424.650
        /// command.Parameter.FreqStop_Hz = 424.675m * 1000000;
        /// command.Parameter.Att_dB = 0; //установили аттенюатор
        /// command.Parameter.PreAmp_dB = 30;
        /// command.Parameter.RefLevel_dBm = -40;
        /// command.Parameter.BitRate_MBs = 0.8;
        /// command.Parameter.IQBlockDuration_s = 0.5;
        /// command.Parameter.IQReceivTime_s = 0.6;
        /// command.Parameter.MandatoryPPS = false; принимать ли сигналы PPS
        /// Если false то работает не воспринимае данных PPS
        /// Если true то суммарное время отсрочки и время прослушки должно быть больше 1 сек, для принятия сигнала PPS
        /// Если сигнал PPS небыл "услышан" за все время получения данных, то будит сгенерированна ошибка 
        /// В результате PPSTimeDifference_ns определяет разницу во времени приема PPS и первым семплом в IQ данных
        /// В результате PPSTimeDifference_ns положительный означает что сигнал PPS был принят во время приема сигнала
        /// PPSTimeDifference_ns отрицательный если сигнал PPS был принят до приема сигнала.
        /// 
        /// command.Parameter.MandatorySignal = true; ожидать ли сигнал, т.е. в первом блоке присутствует начало сигнала
        /// Если false то IQ данные сохраняются первые полученные за IQBlockDuration_s,
        /// Если true то IQ данные сохраняются при наличии сигнала с установленным порогом(пока забита фиксированный порог)
        /// Если сигнал за время прослушки не детектирован, то будит сгенерированна ошибка.
        /// Если Сигнал принят под конец интевала IQReceivTime_s, то время сбора сигнала будит продлено для полного сбора данных за время IQBlockDuration_s
        /// 
        /// command.Parameter.TimeStart = (DateTime.UtcNow.Ticks - new DateTime(1970, 1, 1, 0, 0, 0, System.DateTimeKind.Utc).Ticks);
        /// command.Parameter.TimeStart += 2*10000000; установили время отсрочки старта, 
        /// по факту как только начнет выполняться команда, то начнется прослушка блоков IQ, 
        /// для того чтобы увеличить время срабатывания PPS сигнала, Но данные не будут сохранятся!
        /// Данные начнут сохраняться если текущее системное время попало в однин из принятых блоков
        /// Если по каким либо причинам запуск будит после этого времени и не попадет в блок IQ то будит сгенерированна ошибка
        /// 
        /// 
        /// adapter.MesureIQStreamCommandHandler(command, context);
        ///
        ///  </summary>
        /// <param name="command"></param>
        /// <param name="context"></param>
        public void MesureIQStreamCommandHandler(COM.MesureIQStreamCommand command, IExecutionContext context)
        {
            try
            {
                if (IsRuning)
                {
                    //context.Lock(CommandType.MesureIQStream);
                    context.Lock();

                    if (IdleState)
                    {
                        Status = AdapterDriver.bbAbort(_Device_ID);
                        if (Status != EN.Status.NoError)
                        {
                            _logger.Warning(Contexts.ThisComponent, AdapterDriver.bbGetStatusString(Status));
                        }
                        IdleState = false;
                    }
                    if (FreqStart != command.Parameter.FreqStart_Hz || FreqStop != command.Parameter.FreqStop_Hz)
                    {
                        (FreqStart, FreqStop) = LPC.IQFreqStartStop(this, command.Parameter.FreqStart_Hz, command.Parameter.FreqStop_Hz);

                        Status = AdapterDriver.bbConfigureCenterSpan(_Device_ID, (double)FreqCentr, (double)FreqSpan);
                        if (Status != EN.Status.NoError)
                        {
                            _logger.Warning(Contexts.ThisComponent, AdapterDriver.bbGetStatusString(Status));
                        }
                    }

                    EN.Attenuator att = LPC.Attenuator(command.Parameter.Att_dB);
                    if (Attenuator != att || RefLevel != command.Parameter.RefLevel_dBm)
                    {
                        Attenuator = att;
                        RefLevel = command.Parameter.RefLevel_dBm;
                        Status = AdapterDriver.bbConfigureLevel(_Device_ID, RefLevel, (double)Attenuator);
                        if (Status != EN.Status.NoError)
                        {
                            _logger.Warning(Contexts.ThisComponent, AdapterDriver.bbGetStatusString(Status));
                        }
                    }

                    EN.Gain gain = LPC.Gain(command.Parameter.PreAmp_dB);
                    if (gain != Gain)
                    {
                        Gain = LPC.Gain(command.Parameter.PreAmp_dB);
                        Status = AdapterDriver.bbConfigureGain(_Device_ID, (int)Gain);
                        if (Status != EN.Status.NoError)
                        {
                            _logger.Warning(Contexts.ThisComponent, AdapterDriver.bbGetStatusString(Status));
                        }
                    }

                    DownsampleFactor = LPC.IQDownsampleFactor(command.Parameter.BitRate_MBs, FreqSpan);
                    Status = AdapterDriver.bbConfigureIQ(_Device_ID, DownsampleFactor, (double)FreqSpan);
                    if (Status != EN.Status.NoError)
                    {
                        _logger.Warning(Contexts.ThisComponent, AdapterDriver.bbGetStatusString(Status));
                    }

                    if (DeviceMode != EN.Mode.Streaming || FlagMode != EN.Flag.StreamIQ)
                    {
                        DeviceMode = EN.Mode.Streaming;
                        FlagMode = EN.Flag.StreamIQ;
                    }
                    Status = AdapterDriver.bbInitiate(_Device_ID, (uint)DeviceMode, (uint)FlagMode);
                    if (Status != EN.Status.NoError)
                    {
                        _logger.Warning(Contexts.ThisComponent, AdapterDriver.bbGetStatusString(Status));
                    }

                    (double BlockDuration, double ReceiveTime) = LPC.IQTimeParameters(command.Parameter.IQBlockDuration_s, command.Parameter.IQReceivTime_s);


                    return_len = 0; samples_per_sec = 0; bandwidth = 0.0;
                    Status = AdapterDriver.bbQueryStreamInfo(_Device_ID, ref return_len, ref bandwidth, ref samples_per_sec);
                    if (Status != EN.Status.NoError)
                    {
                        _logger.Warning(Contexts.ThisComponent, AdapterDriver.bbGetStatusString(Status));
                    }
                    IdleState = true;

                    //инициализация перед пуском, подготавливаемся к приему данных 
                    COMR.MesureIQStreamResult result = new COMR.MesureIQStreamResult(0, CommandResultStatus.Final); //ReceivedIQStream riq = new ReceivedIQStream();
                    TempIQData tiq = new TempIQData();
                    InitialReceivedIQStream(ref result, ref tiq, BlockDuration, ReceiveTime, command.Parameter.TimeStart * 100);
                    //закончили подготовку


                    //психуем и принимаем все
                    if (GetIQStream(ref result, tiq, context, command.Parameter.MandatoryPPS, command.Parameter.MandatorySignal))
                    {
                        //пушаем
                        context.PushResult(result);
                    }
                    if (IdleState)
                    {
                        Status = AdapterDriver.bbAbort(_Device_ID);
                        if (Status != EN.Status.NoError)
                        {
                            _logger.Warning(Contexts.ThisComponent, AdapterDriver.bbGetStatusString(Status));
                        }
                        IdleState = false;
                    }
                    context.Unlock();
                    context.Finish();
                }
                else
                {
                    throw new Exception("The device with serial number " + Device_SerialNumber + " does not work");
                }
            }
            catch (Exception e)
            {
                try//знаю что плохо но нужно освободить железо если че
                {
                    if (IdleState)
                    {
                        Status = AdapterDriver.bbAbort(_Device_ID);
                        if (Status != EN.Status.NoError)
                        {
                            _logger.Warning(Contexts.ThisComponent, AdapterDriver.bbGetStatusString(Status));
                        }
                        IdleState = false;
                    }
                }
                catch { }
                // желательно записать влог
                _logger.Exception(Contexts.ThisComponent, e);
                // этот вызов обязательный в случаи обрыва
                context.Abort(e);

                // дальше кода быть не должно, освобождаем поток
            }

        }
        #region Param

        public EN.Status Status = EN.Status.NoError;
        private EN.Mode DeviceMode = EN.Mode.Sweeping;
        private EN.Flag FlagMode = EN.Flag.StreamIQ;
        #region Freqs
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
        private EN.Rejection Rejection = EN.Rejection.NoSpurReject;
        private EN.RBWShape RBWShape = EN.RBWShape.Shape_FlatTop;
        public decimal RBWMax = 10000000;
        public decimal RBW = 10000;
        public decimal[] RBWArr = new decimal[] { 1, 3, 30, 100, 300, 1000, 3000, 10000, 30000, 100000, 300000, 1000000, 3000000, 10000000 };

        public decimal VBWMax = 10000000;
        public decimal VBW = 10000;
        #endregion RBW / VBW

        #region Sweep
        public decimal SweepTimeMin = 0.00001m;
        public decimal SweepTimeMax = 0.1m;

        public decimal SweepTime
        {
            get { return _SweepTime; }
            set
            {
                if (value >= SweepTimeMin && value <= SweepTimeMax) _SweepTime = value;
                else if (value < SweepTimeMin) _SweepTime = SweepTimeMin;
                else if (value > SweepTimeMax) _SweepTime = SweepTimeMax;
            }
        }
        private decimal _SweepTime = 0.00001m;
        #endregion

        #region Levels
        /// <summary>
        /// Перегруз прибора
        /// 0 = normal
        /// 1 = RF Overload
        /// </summary>
        public int RFOverload = 0;

        #region уровни отображения
        public double RefLevel
        {
            get { return _RefLevel; }
            set
            {
                _RefLevel = value;
                LowestLevel = _RefLevel - Range;
            }
        }
        private double _RefLevel = -40;

        public double Range
        {
            get { return _Range; }
            set
            {
                _Range = value;
                LowestLevel = _RefLevel - Range;
            }
        }
        private double _Range = 100;

        public double LowestLevel { get; set; } = -140;
        #endregion
        private EN.Scale Scale = EN.Scale.LogScale;
        public MEN.LevelUnit LevelUnit = MEN.LevelUnit.dBm;
        private EN.Gain Gain = EN.Gain.Gain_AUTO;
        private EN.Attenuator Attenuator = EN.Attenuator.Atten_AUTO;
        #endregion

        #region Trace Data
        private bool NewTrace;

        private decimal FreqStep = 10000;

        private int TracePoints = 1601;

        private ulong TraceCountToMeas = 1;
        private ulong TraceCount = 1;

        public double[] FreqArr;
        public float[] LevelArr;

        public float[] RealTimeFrame;

        bool TraceReset;
        private AveragedTrace TraceAveraged = new AveragedTrace();

        public int RealTimeFrameWidth = 1;

        public int RealTimeFrameHeight = 1;

        decimal TraceFreqStart = 0;
        double TraceFreqStop = 0;

        #region Trace
        public EN.Detector DetectorUse = EN.Detector.MaxOnly;
        public EN.Detector DetectorToSet = EN.Detector.MinAndMax;
        private EN.TraceType TraceType = EN.TraceType.ClearWrite;
        private EN.Unit VideoUnit = EN.Unit.Log;
        #endregion
        #endregion

        #region runs
        private bool IsRuning;
        private long LastUpdate;
        #endregion

        #region Device info
        public decimal FreqMin = 9000;
        public decimal FreqMax = 6400000000;


        bool IdleState = false;
        /// <summary>
        /// температура приемника от нее пляшем с калибровкой
        /// </summary>
        public double Device_BoardTemp;
        /// <summary>
        /// последняя температура калибровки
        /// </summary>
        private double Device_LastCalcBoardTemp;

        public double Device_USBVoltage;

        public double Device_USBCurrent;

        public int Device_ID
        {
            get { return _Device_ID; }
            set { _Device_ID = value; }
        }
        private int _Device_ID = -1;

        public string Device_Type
        {
            get { return _Device_Type; }
            set { _Device_Type = value; }
        }
        private string _Device_Type = "";

        public string Device_SerialNumber
        {
            get { return _Device_SerialNumber; }
            set { _Device_SerialNumber = value; }
        }
        private string _Device_SerialNumber = "";

        public string Device_FirmwareVersion
        {
            get { return _Device_FirmwareVersion; }
            set { _Device_FirmwareVersion = value; }
        }
        private string _Device_FirmwareVersion = "";

        public string Device_APIVersion
        {
            get { return _Device_APIVersion; }
            set { _Device_APIVersion = value; }
        }
        private string _Device_APIVersion = "";


        #endregion

        #region IQStream
        private int DownsampleFactor;
        private int return_len;
        private int samples_per_sec;
        private double bandwidth;
        #endregion IQStream

        #endregion Param

        #region Private Method
        private void GetSystemInfo()
        {
            try
            {
                float temp = 0.0F, voltage = 0.0F, current = 0.0F;
                Status = AdapterDriver.bbGetDeviceDiagnostics(_Device_ID, ref temp, ref voltage, ref current);
            }
            #region Exception
            catch (Exception exp)
            {
                _logger.Exception(Contexts.ThisComponent, exp);
            }
            #endregion
        }
        private void SetTraceDetectorAndScale()
        {
            try
            {
                Status = AdapterDriver.bbConfigureAcquisition(_Device_ID, (uint)DetectorToSet, (uint)Scale);
            }
            #region Exception
            catch (Exception exp)
            {
                _logger.Exception(Contexts.ThisComponent, exp);
            }
            #endregion
        }
        private void SetFreqCentrSpan()
        {
            try
            {
                Status = AdapterDriver.bbConfigureCenterSpan(_Device_ID, (double)FreqCentr, (double)FreqSpan);
            }
            #region Exception
            catch (Exception exp)
            {
                _logger.Exception(Contexts.ThisComponent, exp);
            }
            #endregion
        }
        private void SetRefATT()
        {
            try
            {
                Status = AdapterDriver.bbConfigureLevel(_Device_ID, (double)RefLevel, (double)Attenuator);
            }
            #region Exception
            catch (Exception exp)
            {
                _logger.Exception(Contexts.ThisComponent, exp);
            }
            #endregion
        }
        private void SetGain()
        {
            try
            {
                Status = AdapterDriver.bbConfigureGain(_Device_ID, (int)Gain);
            }
            #region Exception
            catch (Exception exp)
            {
                _logger.Exception(Contexts.ThisComponent, exp);
            }
            #endregion
        }
        private void SetRbwVbwSweepTimeRbwType()
        {
            try
            {
                Status = AdapterDriver.bbConfigureSweepCoupling(_Device_ID, (double)RBW, (double)VBW, (double)SweepTime, (uint)RBWShape, (uint)Rejection);
            }
            #region Exception
            catch (Exception exp)
            {
                _logger.Exception(Contexts.ThisComponent, exp);
            }
            #endregion
        }
        private void SetPortType()
        {
            try
            {
                Status = AdapterDriver.bbConfigureIO(_Device_ID, 0, (uint)EN.Port2.InTriggerRisingEdge);
                if (Status != EN.Status.NoError)
                {
                    _logger.Warning(Contexts.ThisComponent, AdapterDriver.bbGetStatusString(Status));
                }
            }
            #region Exception
            catch (Exception exp)
            {
                _logger.Exception(Contexts.ThisComponent, exp);
            }
            #endregion
        }
        private void SetVideoUnits()
        {
            try
            {
                Status = AdapterDriver.bbConfigureProcUnits(_Device_ID, (uint)VideoUnit);
            }
            #region Exception
            catch (Exception exp)
            {
                _logger.Exception(Contexts.ThisComponent, exp);
            }
            #endregion
        }
        private void SetOverLoad(EN.Status value)
        {
            if (value == EN.Status.ADCOverflow) { RFOverload = 1; }
            else { RFOverload = 0; }
        }

        private bool GetTrace()
        {
            bool res = true;
            //получаем спектр
            uint trace_len = 0;
            double bin_size = 0.0;
            double start_freq = 0.0;
            Status = AdapterDriver.bbQueryTraceInfo(_Device_ID, ref trace_len, ref bin_size, ref start_freq);
            SetOverLoad(Status);

            FreqStep = (decimal)bin_size;

            if (Status != EN.Status.DeviceConnectionErr ||
                Status != EN.Status.DeviceInvalidErr ||
                Status != EN.Status.DeviceNotOpenErr ||
                Status != EN.Status.USBTimeoutErr)
            {
                IsRuning = true;
            }

            float[] sweep_max, sweep_min;
            sweep_max = new float[trace_len];
            sweep_min = new float[trace_len];

            Status = AdapterDriver.bbFetchTrace_32f(_Device_ID, unchecked((int)trace_len), sweep_min, sweep_max);
            if (Status == EN.Status.DeviceConnectionErr)
            {
                res = false;
            }
            else
            {
                SetOverLoad(Status);
                SetTraceData((int)trace_len, sweep_min, sweep_max, (decimal)start_freq, (decimal)bin_size);
                LastUpdate = DateTime.Now.Ticks;
            }

            return res;
        }
        private void SetTraceData(int newLength, float[] mintrace, float[] maxtrace, decimal freqStart, decimal step)
        {
            if (maxtrace.Length > 0 && newLength > 0 && step > 0)
            {
                if (FreqArr.Length != newLength || (Math.Abs(TraceFreqStart - (decimal)freqStart) >= (decimal)step))
                {
                    TraceFreqStart = freqStart;
                    TracePoints = newLength;
                    FreqArr = new double[newLength];
                    LevelArr = new float[newLength];
                    if (LevelUnit == MEN.LevelUnit.dBm)
                    {
                        if (DetectorUse == EN.Detector.Average || DetectorUse == EN.Detector.MaxOnly)
                        {
                            for (int i = 0; i < newLength; i++)
                            {
                                FreqArr[i] = (double)(freqStart + step * i);
                                LevelArr[i] = maxtrace[i];
                            }
                        }
                        else if (DetectorUse == EN.Detector.MinOnly)
                        {
                            for (int i = 0; i < newLength; i++)
                            {
                                FreqArr[i] = (double)(freqStart + step * i);
                                LevelArr[i] = mintrace[i];
                            }
                        }
                    }
                    else if (LevelUnit == MEN.LevelUnit.dBµV)
                    {
                        if (DetectorUse == EN.Detector.Average || DetectorUse == EN.Detector.MaxOnly)
                        {
                            for (int i = 0; i < newLength; i++)
                            {
                                FreqArr[i] = (double)(freqStart + step * i);
                                LevelArr[i] = maxtrace[i] + 107;
                            }
                        }
                        else if (DetectorUse == EN.Detector.MinOnly)
                        {
                            for (int i = 0; i < newLength; i++)
                            {
                                FreqArr[i] = (double)(freqStart + step * i);
                                LevelArr[i] = mintrace[i] + 107;
                            }
                        }
                    }
                    TraceFreqStop = FreqArr[FreqArr.Length - 1];
                }
                if (TraceType == EN.TraceType.ClearWrite)
                {
                    if (LevelUnit == MEN.LevelUnit.dBm)
                    {
                        if (DetectorUse == EN.Detector.Average || DetectorUse == EN.Detector.MaxOnly)
                        {
                            for (int i = 0; i < newLength; i++)
                            {
                                LevelArr[i] = maxtrace[i];
                            }
                        }
                        else if (DetectorUse == EN.Detector.MinOnly)
                        {
                            for (int i = 0; i < newLength; i++)
                            {
                                LevelArr[i] = mintrace[i];
                            }
                        }
                    }
                    else if (LevelUnit == MEN.LevelUnit.dBµV)
                    {
                        if (DetectorUse == EN.Detector.Average || DetectorUse == EN.Detector.MaxOnly)
                        {
                            for (int i = 0; i < newLength; i++)
                            {
                                LevelArr[i] = maxtrace[i] + 107;
                            }
                        }
                        else if (DetectorUse == EN.Detector.MinOnly)
                        {
                            for (int i = 0; i < newLength; i++)
                            {
                                LevelArr[i] = mintrace[i] + 107;
                            }
                        }
                    }
                }
                else if (TraceType == EN.TraceType.Average)//Average
                {
                    float[] levels = new float[newLength];
                    if (LevelUnit == MEN.LevelUnit.dBm)
                    {
                        if (DetectorUse == EN.Detector.Average || DetectorUse == EN.Detector.MaxOnly)
                        {
                            for (int i = 0; i < newLength; i++)
                            {
                                levels[i] = maxtrace[i];
                            }
                        }
                        else if (DetectorUse == EN.Detector.MinOnly)
                        {
                            for (int i = 0; i < newLength; i++)
                            {
                                levels[i] = mintrace[i];
                            }
                        }
                    }
                    else if (LevelUnit == MEN.LevelUnit.dBµV)
                    {
                        if (DetectorUse == EN.Detector.Average || DetectorUse == EN.Detector.MaxOnly)
                        {
                            for (int i = 0; i < newLength; i++)
                            {
                                levels[i] = maxtrace[i] + 107;
                            }
                        }
                        else if (DetectorUse == EN.Detector.MinOnly)
                        {
                            for (int i = 0; i < newLength; i++)
                            {
                                levels[i] = mintrace[i] + 107;
                            }
                        }
                    }
                    if (TraceReset) { TraceAveraged.Reset(); TraceReset = false; }
                    TraceAveraged.AddTraceToAverade(FreqArr, levels, LevelUnit);
                    LevelArr = TraceAveraged.AveragedLevels;

                }
                else if (TraceType == EN.TraceType.MaxHold)
                {
                    if (TraceReset == false)
                    {
                        if (LevelUnit == MEN.LevelUnit.dBm)
                        {
                            if (DetectorUse == EN.Detector.Average || DetectorUse == EN.Detector.MaxOnly)
                            {
                                for (int i = 0; i < newLength; i++)
                                {
                                    if (maxtrace[i] > LevelArr[i]) LevelArr[i] = maxtrace[i];
                                }
                            }
                            else if (DetectorUse == EN.Detector.MinOnly)
                            {
                                for (int i = 0; i < newLength; i++)
                                {
                                    if (mintrace[i] > LevelArr[i]) LevelArr[i] = mintrace[i];
                                }
                            }
                        }
                        else if (LevelUnit == MEN.LevelUnit.dBµV)
                        {
                            if (DetectorUse == EN.Detector.Average || DetectorUse == EN.Detector.MaxOnly)
                            {
                                for (int i = 0; i < newLength; i++)
                                {
                                    if (maxtrace[i] + 107 > LevelArr[i]) LevelArr[i] = maxtrace[i] + 107;
                                }
                            }
                            else if (DetectorUse == EN.Detector.MinOnly)
                            {
                                for (int i = 0; i < newLength; i++)
                                {
                                    if (mintrace[i] + 107 > LevelArr[i]) LevelArr[i] = mintrace[i] + 107;
                                }
                            }
                        }
                    }
                    else
                    {
                        if (LevelUnit == MEN.LevelUnit.dBm)
                        {
                            if (DetectorUse == EN.Detector.Average || DetectorUse == EN.Detector.MaxOnly)
                            {
                                for (int i = 0; i < newLength; i++)
                                {
                                    LevelArr[i] = maxtrace[i];
                                }
                            }
                            else if (DetectorUse == EN.Detector.MinOnly)
                            {
                                for (int i = 0; i < newLength; i++)
                                {
                                    LevelArr[i] = mintrace[i];
                                }
                            }
                        }
                        else if (LevelUnit == MEN.LevelUnit.dBµV)
                        {
                            if (DetectorUse == EN.Detector.Average || DetectorUse == EN.Detector.MaxOnly)
                            {
                                for (int i = 0; i < newLength; i++)
                                {
                                    LevelArr[i] = maxtrace[i] + 107;
                                }
                            }
                            else if (DetectorUse == EN.Detector.MinOnly)
                            {
                                for (int i = 0; i < newLength; i++)
                                {
                                    LevelArr[i] = mintrace[i] + 107;
                                }
                            }
                        }
                        TraceReset = false;
                    }
                }
                else if (TraceType == EN.TraceType.MinHold)
                {
                    if (TraceReset == false)
                    {
                        if (LevelUnit == MEN.LevelUnit.dBm)
                        {
                            if (DetectorUse == EN.Detector.Average || DetectorUse == EN.Detector.MaxOnly)
                            {
                                for (int i = 0; i < newLength; i++)
                                {
                                    if (maxtrace[i] < LevelArr[i]) LevelArr[i] = maxtrace[i];
                                }
                            }
                            else if (DetectorUse == EN.Detector.MinOnly)
                            {
                                for (int i = 0; i < newLength; i++)
                                {
                                    if (mintrace[i] < LevelArr[i]) LevelArr[i] = mintrace[i];
                                }
                            }
                        }
                        else if (LevelUnit == MEN.LevelUnit.dBµV)
                        {
                            if (DetectorUse == EN.Detector.Average || DetectorUse == EN.Detector.MaxOnly)
                            {
                                for (int i = 0; i < newLength; i++)
                                {
                                    if (maxtrace[i] + 107 < LevelArr[i]) LevelArr[i] = maxtrace[i] + 107;
                                }
                            }
                            else if (DetectorUse == EN.Detector.MinOnly)
                            {
                                for (int i = 0; i < newLength; i++)
                                {
                                    if (mintrace[i] + 107 < LevelArr[i]) LevelArr[i] = mintrace[i] + 107;
                                }
                            }
                        }
                    }
                    else
                    {
                        if (LevelUnit == MEN.LevelUnit.dBm)
                        {
                            if (DetectorUse == EN.Detector.Average || DetectorUse == EN.Detector.MaxOnly)
                            {
                                for (int i = 0; i < newLength; i++)
                                {
                                    LevelArr[i] = maxtrace[i];
                                }
                            }
                            else if (DetectorUse == EN.Detector.MinOnly)
                            {
                                for (int i = 0; i < newLength; i++)
                                {
                                    LevelArr[i] = mintrace[i];
                                }
                            }
                        }
                        else if (LevelUnit == MEN.LevelUnit.dBµV)
                        {
                            if (DetectorUse == EN.Detector.Average || DetectorUse == EN.Detector.MaxOnly)
                            {
                                for (int i = 0; i < newLength; i++)
                                {
                                    LevelArr[i] = maxtrace[i] + 107;
                                }
                            }
                            else if (DetectorUse == EN.Detector.MinOnly)
                            {
                                for (int i = 0; i < newLength; i++)
                                {
                                    LevelArr[i] = mintrace[i] + 107;
                                }
                            }
                        }
                        TraceReset = false;
                    }
                }
                NewTrace = true;
            }
        }

        private void InitialReceivedIQStream(ref COMR.MesureIQStreamResult IQStreamResult, ref TempIQData tempIQStream, double blockDuration, double receivTime, long timeStart)
        {
            // Формирование пустого места для записи данных

            //if (tempIQStream == null) tempIQStream = new TempIQData();
            tempIQStream.BlocksCount = (int)Math.Ceiling(blockDuration * samples_per_sec / return_len);
            tempIQStream.BlocksAll = (int)Math.Ceiling(receivTime * samples_per_sec / return_len);
            tempIQStream.TimeStart = timeStart;
            tempIQStream.IQData = new float[tempIQStream.BlocksCount][];
            tempIQStream.dataRemainings = new int[tempIQStream.BlocksCount];
            tempIQStream.sampleLosses = new int[tempIQStream.BlocksCount];
            tempIQStream.OneSempleDuration = 1000000000 / samples_per_sec;
            tempIQStream.BlockTime = new long[100000];
            tempIQStream.BlockTimeDelta = new long[100000];
            tempIQStream.IQDataTemp = new float[return_len * 2];
            tempIQStream.TrDataTemp = new int[71];
            for (int i = 0; i < tempIQStream.BlocksCount; i++)
            {
                float[] iqSamplesX = new float[return_len * 2];
                tempIQStream.IQData[i] = iqSamplesX;
            }
            // сформировано пустое место
        }
        private bool GetIQStream(ref COMR.MesureIQStreamResult IQStreamResult, TempIQData tempIQStream, IExecutionContext context, bool WithPPS, bool JustWithSignal)
        {
            bool done = false;
            bool IsCancellationRequested = false;
            // расчет количества шагов которое мы должны записать. 

            int dataRemaining = 0, sampleLoss = 0, iqSec = 0, iqNano = 0;

            // Константы
            float noise = 0.000001f; // уровень шума в mW^2
            float SN = 10; // превышение шума в разах 
            float TrigerLevel = noise * SN;
            // Конец констант 

            bool SignalFound = false; //был ли сигнал
            int step = tempIQStream.IQData[0].Length / 1000;//шаг проверки уровней на предмет детектирования сигнала
            if (step < 1)
            {
                step = 1;
            }

            bool GetBlockOnTime = false;//тру то принимаем блоки до конца, фолс если недостигли времени приема то пока принимаем PPS
            long BlockTime = return_len * tempIQStream.OneSempleDuration; //Длительность одного блока в нс

            bool PPSDetected = false;//Детектирован ли PPS
            long TimeToStartBlockWithPPS = 0;//Разница времени старта блока и PPS в нс, всегда положителен  (в блоке с PPS)            

            long PrevBlockTime = 0;
            long ThisBlockTime = 0;


            int dTPPSIndex = 0;//индекс PPS в BlockTime
            int IQStartIndex = 0;// иендекс Первого нужного IQ в BlockTime
            int IQStopIndex = 0;// иендекс Последнего нужного IQ в BlockTime

            int AllBlockIndex = -1; //текущий принятый 
            int NecessaryBlockIndex = -1;//Индекс нужного блока, т.е. того который в результаты

            bool ReceivedBlockWithErrors = false; //Есть ли ошибки в нужных блоках

            for (int i = 0; i <= tempIQStream.BlocksAll; i++)
            {
                AllBlockIndex++;
                // снятие данных
                #region
                //полезные данные и до принимаем тут
                if (NecessaryBlockIndex < tempIQStream.BlocksCount - 1)
                {
                    NecessaryBlockIndex++;
                    if (NecessaryBlockIndex == 0)
                    {
                        IQStartIndex = AllBlockIndex;
                    }
                    Status = AdapterDriver.bbGetIQUnpacked(_Device_ID, tempIQStream.IQData[NecessaryBlockIndex], return_len, tempIQStream.TrDataTemp, 71, 1,
                        ref dataRemaining, ref sampleLoss, ref iqSec, ref iqNano);

                    if (!ReceivedBlockWithErrors && IQStopIndex == 0 && NecessaryBlockIndex == tempIQStream.BlocksCount - 1)
                    {
                        IQStopIndex = AllBlockIndex;
                    }
                }
                else//уже приняли нужные данные, то сюда
                {
                    Status = AdapterDriver.bbGetIQUnpacked(_Device_ID, tempIQStream.IQDataTemp, return_len, tempIQStream.TrDataTemp, 71, 1,
                       ref dataRemaining, ref sampleLoss, ref iqSec, ref iqNano);
                }

                //Если вдруг принимаем данные с ошибками то генерируем ошибки, т.к. данные некоректны
                if (Status != EN.Status.NoError)
                {
                    throw new Exception(AdapterDriver.bbGetStatusString(Status));
                }
                #endregion

                tempIQStream.BlockTime[AllBlockIndex] = ((long)iqSec) * 1000000000 + iqNano;
                PrevBlockTime = ThisBlockTime;
                ThisBlockTime = tempIQStream.BlockTime[AllBlockIndex];
                if (PrevBlockTime != 0)
                {
                    tempIQStream.BlockTimeDelta[AllBlockIndex] = ThisBlockTime - PrevBlockTime;
                }
                //определяем когда нужно начинать пытаться принять данные, попал ли этот блок на время начала приема
                if (WithPPS)
                {
                    if (tempIQStream.TrDataTemp[0] > 0)//заново задетектили PPS то все сбросим и начнем считать занов
                    {
                        PPSDetected = true;
                        TimeToStartBlockWithPPS = tempIQStream.TrDataTemp[0] * tempIQStream.OneSempleDuration;

                        dTPPSIndex = AllBlockIndex;///установили в каком блоке был ППС
                    }
                }
                if (!GetBlockOnTime)
                {
                    // Этот блок попал на время старта, начинаем слушать и сюда не возвращаемся
                    if (tempIQStream.BlockTime[AllBlockIndex] <= tempIQStream.TimeStart && tempIQStream.TimeStart <= tempIQStream.BlockTime[AllBlockIndex] + return_len * tempIQStream.OneSempleDuration)
                    {
                        GetBlockOnTime = true;
                    }
                    else
                    {
                        //провтыкали время старта
                        if (tempIQStream.BlockTime[AllBlockIndex] + return_len * tempIQStream.OneSempleDuration > tempIQStream.TimeStart)
                        {
                            throw new Exception("The task was started after the required start time of the task.");//Задача была запущена после необходимого времени старта задачи
                        }
                        NecessaryBlockIndex--;//т.к. не началось время прослушки данных то уменьшим индексм и на то же место запишем болок заново
                        i--;
                    }
                }



                if (GetBlockOnTime)
                {
                    //проверяем наличие сигнала пока его не обнаружили
                    if (JustWithSignal && !SignalFound)
                    {
                        for (int j = 0; tempIQStream.IQData[NecessaryBlockIndex].Length - 6 > j; j += step)
                        {
                            if ((tempIQStream.IQData[NecessaryBlockIndex][j] >= TrigerLevel) || (tempIQStream.IQData[NecessaryBlockIndex][j + 1] >= TrigerLevel))
                            {
                                if ((tempIQStream.IQData[NecessaryBlockIndex][j + 2] >= TrigerLevel) || (tempIQStream.IQData[NecessaryBlockIndex][j + 3] >= TrigerLevel))
                                {
                                    if ((tempIQStream.IQData[NecessaryBlockIndex][j + 4] >= TrigerLevel) || (tempIQStream.IQData[NecessaryBlockIndex][j + 5] >= TrigerLevel))
                                    {
                                        SignalFound = true;//Есть сигнал 
                                        break;
                                    }
                                }
                            }
                        }
                        if (!SignalFound)//небыло сигнала то уменьшим индекс и перезапишем этот блок
                        {
                            NecessaryBlockIndex--;
                        }
                    }

                    //Вышли за отведенное время прослушки
                    if (i >= tempIQStream.BlocksAll)
                    {
                        //хотели c сигналом 
                        if (JustWithSignal)
                        {
                            if (SignalFound)//сигнал есть
                            {
                                if (NecessaryBlockIndex != tempIQStream.BlocksCount - 1)//дослушаем необходимое время прослушки и выйдем
                                {
                                    i--;
                                }
                                else
                                {
                                    if (WithPPS && PPSDetected)
                                    {
                                        break;
                                    }
                                }
                            }
                        }
                        else //Не хотели с сигналом и выйдем
                        {
                            if (WithPPS && PPSDetected)
                            {
                                break;
                            }
                        }
                    }
                    //В этом блоке есть пропук во времени, установим индекс конца блоков и продолжим слушать, может еще PPS нежен
                    if (tempIQStream.BlockTimeDelta.Length > 1 && tempIQStream.OneSempleDuration != tempIQStream.BlockTimeDelta[AllBlockIndex] / return_len)
                    {
                        if (!ReceivedBlockWithErrors && IQStopIndex == 0)
                        {
                            IQStopIndex = AllBlockIndex - 1;
                        }
                        //IsCancellationRequested = true;
                        ReceivedBlockWithErrors = true;
                    }
                }
            }
            //если не попросили завершить раньше времени то пилим результат
            if (!IsCancellationRequested)
            {
                #region обработка полученных данных
                if (ReceivedBlockWithErrors)
                {
                    IQStreamResult = new COMR.MesureIQStreamResult(0, CommandResultStatus.Ragged);
                    IQStreamResult.iq_samples = new float[IQStopIndex - IQStartIndex][];
                    Array.Copy(tempIQStream.IQData, IQStreamResult.iq_samples, IQStopIndex - IQStartIndex);
                }
                else
                {
                    IQStreamResult.iq_samples = tempIQStream.IQData;
                }
                IQStreamResult.TimeStamp = tempIQStream.BlockTime[IQStartIndex] / 100;// DateTime.UtcNow.Ticks - new DateTime(1970, 1, 1, 0, 0, 0, System.DateTimeKind.Utc).Ticks;
                IQStreamResult.OneSempleDuration_ns = tempIQStream.OneSempleDuration;

                if (JustWithSignal && !SignalFound) //Хотели сигнал но его небыло, согласно договоренности генерируем екзепшен
                {
                    throw new Exception("Signal not detected");
                }
                if (WithPPS)//хотим с PPS
                {
                    if (PPSDetected)// PPS был за все время прослушки блоков
                    {
                        IQStreamResult.PPSTimeDifference_ns = TimeToStartBlockWithPPS;
                        if (dTPPSIndex < IQStartIndex)
                        {
                            for (int t = dTPPSIndex; t < IQStartIndex; t++)
                            {
                                IQStreamResult.PPSTimeDifference_ns -= tempIQStream.BlockTimeDelta[t];
                            }
                        }
                        else if (IQStartIndex < dTPPSIndex)
                        {
                            for (int t = IQStartIndex; t < dTPPSIndex; t++)
                            {
                                IQStreamResult.PPSTimeDifference_ns += tempIQStream.BlockTimeDelta[t];
                            }
                        }
                    }
                    else//Сигнал PPS не был детектирован за время приема, согласно договоренности генерируем екзепшен
                    {
                        throw new Exception("No PPS signal was detected during reception.");
                    }
                }
                #endregion обработка полученных данных  
                done = true;
            }
            return done;
        }
        #endregion Private Method


        private class TempIQData
        {
            #region parameters
            public int BlocksCount;//Количевство первых блоков, с уговнем или вообще
            public int BlocksAll; // Всего блоков
            public long TimeStart;//Время начала приема в нс относительно new DateTime(1970, 1, 1, 0, 0, 0, System.DateTimeKind.Utc)
            public long OneSempleDuration; //Дительность одного семпла
            public float[][] IQData;
            public float[] IQDataTemp;//для прослушки PPS писать сюда            
            public int[] TrDataTemp;//данные тригеров, PPS
            public int[] dataRemainings;
            public int[] sampleLosses;

            public long[] BlockTime;//время с железа в наносекндах относительно new DateTime(1970, 1, 1, 0, 0, 0, System.DateTimeKind.Utc)
            public long[] BlockTimeDelta;//Фактическая длительность этого блока (вообще всех)
            #endregion

        }
    }

}
