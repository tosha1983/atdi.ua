using Atdi.Contracts.Sdrn.DeviceServer;
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
                    Status = AdapterDriver.bbInitiate(_Device_ID, (uint)DeviceMode, 0);
                    IsRuning = true;
                    /// включем устройство
                    /// иницируем его параметрами сконфигурации
                    /// проверяем к чем оно готово

                    /// сообщаем инфраструктуре что мы готовы обрабатывать комманду MesureGpsLocationExampleCommand
                    /// и при этом возвращать оезультат в типе MesureGpsLocationExampleAdapterResult
                    //host.RegisterHandler<COM.MesureGpsLocationExampleCommand, MesureGpsLocationExampleAdapterResult>(MesureGpsLocationExampleCommandHandler);
                    host.RegisterHandler<COM.MesureTraceCommand, COMR.MesureTraceResult>(MesureTraceCommandHandler);
                    host.RegisterHandler<COM.MesureIQStreamCommand, COMR.MesureIQStreamResult>(MesureIQStreamCommandHandler);
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
                // желательно записать влог
                _logger.Exception(Contexts.ThisComponent, e);
                // этот вызов обязательный в случаи обрыва
                context.Abort(e);
                // дальше кода быть не должно, освобождаем поток
            }

        }

        public void MesureIQStreamCommandHandler(COM.MesureIQStreamCommand command, IExecutionContext context)
        {
            try
            {
                if (IsRuning)
                {
                    //context.Lock(CommandType.MesureIQStream);
                    context.Lock();


                    Status = AdapterDriver.bbAbort(_Device_ID);
                    if (Status != EN.Status.NoError)
                    {
                        _logger.Warning(Contexts.ThisComponent, AdapterDriver.bbGetStatusString(Status));
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
                    Debug.WriteLine(Status);

                    Status = AdapterDriver.bbConfigureIO(_Device_ID, 0, (uint)EN.Port2.InTriggerRisingEdge);
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


                    //инициализация перед пуском, подготавливаемся к приему данных 
                    COMR.MesureIQStreamResult result = new COMR.MesureIQStreamResult(0, CommandResultStatus.Final); //ReceivedIQStream riq = new ReceivedIQStream();
                    TempIQData tiq = new TempIQData();
                    long timestart = command.Parameter.TimeStart;
                    InitialReceivedIQStream(ref result, ref tiq, BlockDuration, ReceiveTime, timestart);
                    //закончили подготовку

                    DateTime dt = DateTime.Now;//удалить
                    long ttt = dt.Ticks;//удалить
                    Debug.WriteLine(dt.ToString() + " Start");//удалить

                    //готовимся к нужному времени
                    long tickto = command.Parameter.TimeStart + new DateTime(1970, 1, 1, 0, 0, 0, System.DateTimeKind.Utc).Ticks - DateTime.UtcNow.Ticks;
                    while (tickto > 10000)
                    {
                        tickto = command.Parameter.TimeStart + new DateTime(1970, 1, 1, 0, 0, 0, System.DateTimeKind.Utc).Ticks - DateTime.UtcNow.Ticks;
                        if (tickto > 2000000)
                        {
                            Thread.Sleep(200);
                        }
                        else if (tickto <= 2000000 && tickto > 100000)
                        {
                            Thread.Sleep(10);
                        }
                        else if (tickto < 100000 && tickto > 10000)
                        {
                            Thread.Sleep(1);
                        }
                    }
                    Debug.WriteLine(new TimeSpan(DateTime.Now.Ticks - ttt).ToString() + " StartMeasIQ");//удалить
                    Debug.WriteLine((DateTime.UtcNow.Ticks - new DateTime(1970, 1, 1, 0, 0, 0, System.DateTimeKind.Utc).Ticks).ToString() + " Ticks");//удалить
                    Debug.WriteLine(new TimeSpan(DateTime.Now.Ticks).ToString() + " StartMeasIQ");//удалить


                    //психуем и принимаем все
                    if (GetIQStream2(ref result, tiq, command.Parameter.MandatoryPPS, command.Parameter.MandatorySignal))
                    {
                        Status = AdapterDriver.bbAbort(_Device_ID);
                        //пушаем
                        context.PushResult(result);
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
        private EN.Rejection Rejection = EN.Rejection.SpurReject;
        private EN.RBWShape RBWShape = EN.RBWShape.Shape_FlatTop;
        public decimal RBWMax = 10000000;
        public decimal RBW = 10000;

        public decimal VBWMax = 10000000;
        public decimal VBW = 10000;
        #endregion RBW / VBW

        #region Sweep
        public decimal SweepTimeMin = 0.00001m;
        public decimal SweepTimeMax = 1;

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
        private bool NewTrace = false;

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

        /// <summary>
        /// температура приемника от нее пляшем с калибровкой
        /// </summary>
        public double Device_BoardTemp
        {
            get { return _Device_BoardTemp; }
            set { _Device_BoardTemp = value; }
        }
        private double _Device_BoardTemp = 10000;
        /// <summary>
        /// последняя температура калибровки
        /// </summary>
        private double Device_LastCalcBoardTemp = 10000;

        public double Device_USBVoltage
        {
            get { return _Device_USBVoltage; }
            set { _Device_USBVoltage = value; }
        }
        private double _Device_USBVoltage = 10000;

        public double Device_USBCurrent
        {
            get { return _Device_USBCurrent; }
            set { _Device_USBCurrent = value; }
        }
        private double _Device_USBCurrent = 10000;

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

        private int LastPPSTime_sec;
        private int LastPPSTime_nano = -1;
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
                if (TracePoints != newLength || (Math.Abs(TraceFreqStart - (decimal)freqStart) >= (decimal)step))
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

        private bool GetIQStream(ref ReceivedIQStream receivedIQStream, double BlockDuration, double ReceivTime, bool AfterPPS = false, bool JustWithSignal = false)
        {
            bool done = false;
            try
            {
                // константа
                int max_count = 500000;
                // конец констант

                int NumberPass = 0;
                // расчет количества шагов которое мы должны записать. NumberPass
                if (BlockDuration < 0) { NumberPass = 1; } else { NumberPass = (int)Math.Ceiling(BlockDuration * samples_per_sec / return_len); }

                // Формирование пустого места для записи данных
                receivedIQStream = new ReceivedIQStream();
                receivedIQStream.TimeMeasStart = DateTime.Now;
                receivedIQStream.iq_samples = new List<float[]>();
                receivedIQStream.triggers = new List<int[]>();
                receivedIQStream.dataRemainings = new List<int>();
                receivedIQStream.sampleLosses = new List<int>();
                receivedIQStream.iqSeces = new List<int>();
                receivedIQStream.iqNanos = new List<int>();
                List<float[]> IQData = new List<float[]>();
                List<int[]> TrData = new List<int[]>();//данные тригеров, PPS
                List<int> dataRemainings = new List<int>();
                List<int> sampleLosses = new List<int>();
                List<int> iqSeces = new List<int>();
                List<int> iqNanos = new List<int>();
                for (int i = 0; i < NumberPass; i++)
                {
                    float[] iqSamplesX = new float[return_len * 2];
                    int[] triggersX = new int[71];
                    IQData.Add(iqSamplesX);
                    TrData.Add(triggersX);
                    dataRemainings.Add(-1);
                    sampleLosses.Add(-1);
                    iqSeces.Add(-1);
                    iqNanos.Add(-1);
                }
                // сформировано пустое место

                int dataRemaining = 0, sampleLoss = 0, iqSec = 0, iqNano = 0;
                int count = 0; int number_bloks = 0;

                for (int i = 0; i < NumberPass; i++)
                {
                    // снятие данных
                    AdapterDriver.bbGetIQUnpacked(_Device_ID, IQData[i], return_len, TrData[i], 71, 1,
                            ref dataRemaining, ref sampleLoss, ref iqSec, ref iqNano);
                    // конец снятия данных
                    dataRemainings[i] = dataRemaining;
                    sampleLosses[i] = sampleLoss;
                    iqSeces[i] = iqSec;
                    iqNanos[i] = iqNano;


                    if (TrData[i][0] != 0)
                    {
                        AfterPPS = false;
                        LastPPSTime_sec = iqSec;
                        LastPPSTime_nano = iqNano + TrData[i][0] * (DownsampleFactor * 25);
                        //if (JustWithSignal)
                        //{max_count = NumberPass; count = 0;}
                    }
                    else
                    {
                        if (LastPPSTime_nano != -1)
                        {// В данном случае у нас был PPS ранее
                            if (iqSec - LastPPSTime_sec < 10)
                            {// еще не успело сильно растроиться время
                                TrData[i][0] = -(int)((iqNano - LastPPSTime_nano) / (DownsampleFactor * 25));
                                if (TrData[i][0] > 0) { TrData[i][0] = (-1000000000 / (DownsampleFactor * 25)) + TrData[i][0]; }
                            }
                        }
                    }
                    if (AfterPPS)
                    {
                        i--;
                    }
                    if ((JustWithSignal) && (!AfterPPS))
                    {
                        // Константы
                        double noise = 0.00001; // уровень шума в mW^2
                        double SN = 10; // превышение шума в разах 
                        // Конец констант 
                        bool signal = false;
                        int step = (int)(IQData[i].Length / 1000);
                        if (step < 1) { step = 1; }
                        double TrigerLevel = noise * SN;
                        for (int j = 0; IQData[i].Length - 6 > j; j = j + step)
                        {
                            if ((IQData[i][j] >= TrigerLevel) || (IQData[i][j + 1] >= TrigerLevel))
                            {
                                if ((IQData[i][j + 2] >= TrigerLevel) || (IQData[i][j + 3] >= TrigerLevel))
                                {
                                    if ((IQData[i][j + 4] >= TrigerLevel) || (IQData[i][j + 5] >= TrigerLevel))
                                    {
                                        signal = true;
                                        break;
                                    }
                                }
                            }
                        }
                        if (!signal)
                        {
                            i--;
                            TrData[i][0] = 0;
                        }
                    }
                    count++;
                    number_bloks = i + 1;
                    if (count >= max_count) { break; }
                }

                // перегрупировка данных в нужный формат
                for (int i = 0; i < number_bloks; i++)
                {
                    float[] iq_sample = new float[return_len * 2];
                    int[] trigger = new int[80];
                    for (int j = 0; j < return_len * 2; j++)
                    {
                        iq_sample[j] = IQData[i][j];
                    }
                    for (int j = 0; j < 10; j++)
                    {
                        trigger[j] = TrData[i][j];
                        if (trigger[j] == 0) { break; }
                    }
                    receivedIQStream.iq_samples.Add(iq_sample);
                    receivedIQStream.triggers.Add(trigger);
                    receivedIQStream.dataRemainings.Add(dataRemainings[i]);
                    receivedIQStream.sampleLosses.Add(sampleLosses[i]);
                    receivedIQStream.iqSeces.Add(iqSeces[i]);
                    receivedIQStream.iqNanos.Add(iqNanos[i]);
                }
                receivedIQStream.durationReceiving_sec = BlockDuration;
                done = true;
            }
            catch
            {
                receivedIQStream = null;
            }

            return done;
        }



        private void InitialReceivedIQStream(ref COMR.MesureIQStreamResult IQStreamResult, ref TempIQData tempIQStream, double blockDuration, double receivTime, long timeStart)
        {
            // Формирование пустого места для записи данных

            //if (tempIQStream == null) tempIQStream = new TempIQData();
            tempIQStream.BlocksCount = (int)Math.Ceiling(blockDuration * samples_per_sec / return_len);
            tempIQStream.BlocksAll = (int)Math.Ceiling(receivTime * samples_per_sec / return_len);
            tempIQStream.TimeStart = timeStart;
            //tempIQStream.TimeLength = (long)(blockDuration * 10000000);
            tempIQStream.IQData = new List<float[]>();
            tempIQStream.TrData = new List<int[]>();//данные тригеров, PPS
            tempIQStream.dataRemainings = new List<int>();
            tempIQStream.sampleLosses = new List<int>();
            tempIQStream.iqTime = new List<long>();
            tempIQStream.iqdelta = new List<long>();
            tempIQStream.OneSempleDuration = 1000000000 / samples_per_sec;
            for (int i = 0; i < tempIQStream.BlocksCount; i++)
            {
                float[] iqSamplesX = new float[return_len * 2];
                int[] triggersX = new int[71];
                tempIQStream.IQData.Add(iqSamplesX);
                tempIQStream.TrData.Add(triggersX);
                tempIQStream.dataRemainings.Add(-1);
                tempIQStream.sampleLosses.Add(-1);
                tempIQStream.iqTime.Add(-1);
            }
            // сформировано пустое место
        }
        private bool GetIQStream2(ref COMR.MesureIQStreamResult IQStreamResult, TempIQData tempIQStream, bool WithPPS, bool JustWithSignal)
        {
            bool done = false;
            try
            {
                Debug.WriteLine(new TimeSpan(DateTime.Now.Ticks).ToString() + " Start MeasIQ");//удалить


                // расчет количества шагов которое мы должны записать. 

                int dataRemaining = 0, sampleLoss = 0, iqSec = 0, iqNano = 0;
                int count = 0; int number_bloks = 0;

                // Константы
                float noise = 0.000005f; // уровень шума в mW^2
                float SN = 10; // превышение шума в разах 
                float TrigerLevel = noise * SN;
                // Конец констант 

                bool SignalFound = false;
                int step = tempIQStream.IQData[0].Length / 1000;
                if (step < 1)
                {
                    step = 1;
                }
                for (int i = 0; i < tempIQStream.BlocksCount; i++)
                {
                    // снятие данных
                    AdapterDriver.bbGetIQUnpacked(_Device_ID, tempIQStream.IQData[i], return_len, tempIQStream.TrData[i], 71, 1,
                            ref dataRemaining, ref sampleLoss, ref iqSec, ref iqNano);
                    // конец снятия данных
                    tempIQStream.dataRemainings[i] = dataRemaining;
                    tempIQStream.sampleLosses[i] = sampleLoss;
                    tempIQStream.iqTime[i] = ((long)iqSec) * 1000000000 + iqNano;
                    if (i > 0) tempIQStream.iqdelta.Add(tempIQStream.iqTime[i] - tempIQStream.iqTime[i - 1]);

                    #region
                    //if (TrData[i][0] != 0)
                    //{
                    //    AfterPPS = false;
                    //    LastPPSTime_sec = iqSec;
                    //    LastPPSTime_nano = iqNano + TrData[i][0] * (DownsampleFactor * 25);
                    //    //if (JustWithSignal)
                    //    //{max_count = NumberPass; count = 0;}
                    //}
                    //else
                    //{
                    //    if (LastPPSTime_nano != -1)
                    //    {// В данном случае у нас был PPS ранее
                    //        if (iqSec - LastPPSTime_sec < 10)
                    //        {// еще не успело сильно растроиться время
                    //            TrData[i][0] = -(int)((iqNano - LastPPSTime_nano) / (DownsampleFactor * 25));
                    //            if (TrData[i][0] > 0) { TrData[i][0] = (-1000000000 / (DownsampleFactor * 25)) + TrData[i][0]; }
                    //        }
                    //    }
                    //}
                    //if (AfterPPS)
                    //{
                    //    i--;
                    //}
                    #endregion
                    if (JustWithSignal && !SignalFound) //проверяем наличие сигнала пока его не обнаружили//|| !AfterPPS)
                    {
                        for (int j = 0; tempIQStream.IQData[i].Length - 6 > j; j += step)
                        {
                            if ((tempIQStream.IQData[i][j] >= TrigerLevel) || (tempIQStream.IQData[i][j + 1] >= TrigerLevel))
                            {
                                if ((tempIQStream.IQData[i][j + 2] >= TrigerLevel) || (tempIQStream.IQData[i][j + 3] >= TrigerLevel))
                                {
                                    if ((tempIQStream.IQData[i][j + 4] >= TrigerLevel) || (tempIQStream.IQData[i][j + 5] >= TrigerLevel))
                                    {
                                        SignalFound = true;
                                        break;
                                    }
                                }
                            }
                        }
                        if (!SignalFound)//небыло сигнала то уменьшим индекс и перезапишем
                        {
                            DateTime dt2 = new DateTime(1970, 1, 1, 0, 0, 0, System.DateTimeKind.Utc);//удалить
                            Debug.WriteLine(count + " " + "Нет сигнала " + tempIQStream.iqTime[i] + "  " + new TimeSpan(dt2.Ticks + tempIQStream.iqTime[i] / 100).ToString());//удалить
                            i--;
                        }
                        else
                        {
                            //Thread.Sleep(2000);
                            DateTime dt2 = new DateTime(1970, 1, 1, 0, 0, 0, System.DateTimeKind.Utc);//удалить
                            Debug.WriteLine(count + " " + "Есть сигнал " + tempIQStream.iqTime[i] + "  " + new TimeSpan(dt2.Ticks + tempIQStream.iqTime[i] / 100).ToString());//удалить
                        }
                    }
                    count++;

                    if (count >= tempIQStream.BlocksAll)//Вышли за отведенное время прослушки
                    {
                        Debug.WriteLine(count + " " + new TimeSpan(DateTime.Now.Ticks).ToString() + " Вышли за время");//удалить
                        if (SignalFound)//Есть сигнал
                        {
                            if (i == tempIQStream.BlocksCount - 1)//дослушаем необходимое время прослушки и выйдем
                            {
                                Debug.WriteLine(count + " " + new TimeSpan(DateTime.Now.Ticks).ToString() + " Закончили прием сигнала по максимальному времени приема");//удалить
                                break;
                            }
                        }
                        else//нету сигнала то выходим
                        {
                            Debug.WriteLine(count + " " + new TimeSpan(DateTime.Now.Ticks).ToString() + " Закончили недождались");//удалить
                            break;
                        }
                    }
                    if (tempIQStream.iqdelta.Count > 1 && tempIQStream.OneSempleDuration != tempIQStream.iqdelta[tempIQStream.iqdelta.Count - 1] / return_len)
                    {
                        Debug.WriteLine(count + " " + new TimeSpan(DateTime.Now.Ticks).ToString() + " Жопа, провал во времени между блоками");//удалить
                    }
                }
                #region обработка полученных данных
                if (JustWithSignal && !SignalFound)
                {
                    throw new Exception("Signal not detected");
                }
                if (WithPPS)
                {
                    //ищем PPS 
                    bool DetectPPS = false;
                    int block = 0, trigerindex = 0;
                    for (int i = 0; i < tempIQStream.BlocksCount; i++)
                    {
                        for (int k = 0; k < tempIQStream.TrData[i].Length; k++)
                        {
                            if (!DetectPPS && tempIQStream.TrData[i][k] > 0)
                            {
                                block = i;
                                trigerindex = k;
                                DetectPPS = false;
                                break;
                            }
                        }
                        if (DetectPPS)
                        {
                            break;
                        }
                    }
                    if (DetectPPS)
                    {
                        long starttimeblock = ((block - 1) * (tempIQStream.IQData.Count / 2) + trigerindex) * tempIQStream.OneSempleDuration;
                        Debug.WriteLine("PPS detected: Block " + block + " trigerindex " + trigerindex);//удалить
                    }
                    else
                    {
                        throw new Exception("PPS not detected");
                    }
                }
                IQStreamResult.TimeStamp = tempIQStream.iqTime[0] / 100;// DateTime.UtcNow.Ticks - new DateTime(1970, 1, 1, 0, 0, 0, System.DateTimeKind.Utc).Ticks;
                IQStreamResult.OneSempleDuration_ns = tempIQStream.OneSempleDuration;
                IQStreamResult.iq_samples = tempIQStream.IQData.ToArray();
                #endregion обработка полученных данных                
                Debug.WriteLine(new TimeSpan(DateTime.Now.Ticks).ToString() + " Stop MeasIQ");//удалить
                done = true;
            }
            catch (Exception e)
            {
                IQStreamResult = null;
            }

            return done;
        }

        #endregion Private Method


        private class ReceivedIQStream
        {
            #region parameters
            public List<float[]> iq_samples;
            public List<int[]> triggers;
            public List<float[]> Ampl;
            public List<int> dataRemainings;
            public List<int> sampleLosses;
            public List<int> iqSeces;
            public List<int> iqNanos;
            public double MinLevel;
            public double MaxLevel;
            public DateTime TimeMeasStart; // время начала измерения потока IQ
            public double durationReceiving_sec;
            #endregion
            public void CalcAmpl(bool BB60C)
            {
                Ampl = new List<float[]>();
                MinLevel = 1000;
                MaxLevel = 0;
                if (BB60C)
                {
                    for (int i = 0; i < iq_samples.Count; i++)
                    {
                        float[] arrAmpl = new float[iq_samples[i].Length / 2];
                        for (int j = 0; j < iq_samples[i].Length; j = j + 2)
                        {
                            float _arrAmpl = (float)(iq_samples[i][j] * iq_samples[i][j] + iq_samples[i][j + 1] * iq_samples[i][j + 1]);
                            if (MinLevel > _arrAmpl) { MinLevel = _arrAmpl; }
                            if (MaxLevel < _arrAmpl) { MaxLevel = _arrAmpl; }
                            arrAmpl[j / 2] = _arrAmpl;
                        }
                        Ampl.Add(arrAmpl);
                    }
                }
                else
                {
                    for (int i = 0; i < iq_samples.Count; i++)
                    {
                        float[] arrAmpl = new float[iq_samples[i].Length / 2];
                        for (int j = 0; j < iq_samples[i].Length; j = j + 2)
                        {
                            float _arrAmpl = (float)Math.Sqrt(iq_samples[i][j] * iq_samples[i][j] + iq_samples[i][j + 1] * iq_samples[i][j + 1]);
                            if (MinLevel > _arrAmpl) { MinLevel = _arrAmpl; }
                            if (MaxLevel < _arrAmpl) { MaxLevel = _arrAmpl; }
                            arrAmpl[j / 2] = _arrAmpl;
                        }
                        Ampl.Add(arrAmpl);
                    }
                }
            }
        }

        private class TempIQData
        {
            #region parameters
            public int BlocksCount;//Количевство первых блоков, с уговнем или вообще
            public int BlocksAll; // Всего блоков
            public long TimeStart;//Время начала приема в Тики относительно new DateTime(1970, 1, 1, 0, 0, 0, System.DateTimeKind.Utc)
            public long OneSempleDuration; //Дительность одного семпла
            public List<float[]> IQData;
            public List<int[]> TrData;//данные тригеров, PPS
            public List<int> dataRemainings;
            public List<int> sampleLosses;
            public List<long> iqTime; //времени в наносекндах относительно new DateTime(1970, 1, 1, 0, 0, 0, System.DateTimeKind.Utc)
            public List<long> iqdelta; //разница времен
            #endregion

        }
    }

}
