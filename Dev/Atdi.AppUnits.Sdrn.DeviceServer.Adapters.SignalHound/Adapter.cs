using Atdi.Contracts.Sdrn.DeviceServer;
using Atdi.DataModels.Sdrn.DeviceServer;
using Atdi.Platform.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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

                    Status = AdapterDriver.bbInitiate(_Device_ID, (uint)DeviceMode, 0);
                    if (Status != EN.Status.NoError)
                    {
                        _logger.Warning(Contexts.ThisComponent, AdapterDriver.bbGetStatusString(Status));
                    }

                    LPC.DetectorType(this, command.Parameter.DetectorType);
                    Status = AdapterDriver.bbConfigureAcquisition(_Device_ID, (uint)DetectorToSet, (uint)Scale);
                    if (Status != EN.Status.NoError)
                    {
                        _logger.Warning(Contexts.ThisComponent, AdapterDriver.bbGetStatusString(Status));
                    }
                    TraceType = LPC.TraceType(command.Parameter.TraceType);
                    LevelUnit = LPC.LevelUnit(command.Parameter.LevelUnit);

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
        


        #region Param

        public EN.Status Status = EN.Status.NoError;
        private EN.Mode DeviceMode = EN.Mode.Sweeping;

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
        #endregion Private Method



    }

}
