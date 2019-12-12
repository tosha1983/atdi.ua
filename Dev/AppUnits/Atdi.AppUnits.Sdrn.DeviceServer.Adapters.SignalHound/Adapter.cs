using Atdi.Contracts.Sdrn.DeviceServer;
using CFG = Atdi.DataModels.Sdrn.DeviceServer.Adapters.Config;
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
using PEN = Atdi.DataModels.Sdrn.DeviceServer.Commands.Parameters;

namespace Atdi.AppUnits.Sdrn.DeviceServer.Adapters.SignalHound
{
    /// <summary>
    /// Пример реализации объект аадаптера
    /// </summary>
    public class Adapter : IAdapter
    {
        private readonly ITimeService timeService;
        private readonly ILogger logger;
        private readonly AdapterConfig adapterConfig;
        private LocalParametersConverter lpc;
        private CFG.ThisAdapterConfig tac;
        private CFG.AdapterMainConfig mainConfig;

        /// <summary>
        /// Все объекты адаптера создаются через DI-контейнер 
        /// Запрашиваем через конструктор необходимые сервисы
        /// </summary>
        /// <param name="adapterConfig"></param>
        /// <param name="logger"></param>
        /// <param name="timeService"></param>
        public Adapter(AdapterConfig adapterConfig, ILogger logger, ITimeService timeService)
        {
            this.logger = logger;
            this.timeService = timeService;
            if (ValidateAdapterConfig(adapterConfig))
            {
                this.adapterConfig = adapterConfig;
            }

            lpc = new LocalParametersConverter(this.logger);

            LevelArr = new float[tracePoints];
            for (int i = 0; i < tracePoints; i++)
            {
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

                if (Connect(adapterConfig.SerialNumber))
                {
                    //deviceSerialNumber = 16319373;
                    string fileName = new Atdi.DataModels.Sdrn.DeviceServer.Adapters.InstrManufacrures().SignalHound.UI + "_" + deviceSerialNumber + ".xml";
                    tac = new CFG.ThisAdapterConfig() { };
                    if (!tac.GetThisAdapterConfig(fileName))
                    {
                        mainConfig = new CFG.AdapterMainConfig() { };
                        SetDefaulConfig(ref mainConfig);
                        tac.SetThisAdapterConfig(mainConfig, fileName);
                    }
                    else
                    {

                        if (tac.Main.AdapterTraceResultPools.Length == 0)
                        {
                            SetDefaulTraceResultPoolsConfig(ref tac.Main);
                            tac.SetThisAdapterConfig(tac.Main, fileName);
                        }
                        mainConfig = tac.Main;
                    }
                    (MesureTraceDeviceProperties mtdp, MesureIQStreamDeviceProperties miqdp) = GetProperties(mainConfig);

                    IResultPoolDescriptor<COMR.MesureTraceResult>[] rpd = ValidateAdapterTracePoolMainConfig(mainConfig.AdapterTraceResultPools, fileName);

                    host.RegisterHandler<COM.MesureTraceCommand, COMR.MesureTraceResult>(MesureTraceCommandHandler, rpd, mtdp);
                    host.RegisterHandler<COM.MesureIQStreamCommand, COMR.MesureIQStreamResult>(MesureIQStreamCommandHandler, miqdp);
                }
            }
            #region Exception
            catch (Exception exp)
            {
                logger.Exception(Contexts.ThisComponent, exp);
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
                //TAC.Save();
                isRuning = false;
                StatusError(AdapterDriver.bbCloseDevice(deviceId));
            }
            #region Exception
            catch (Exception exp)
            {
                logger.Exception(Contexts.ThisComponent, exp);
            }
            #endregion
        }


        public void MesureTraceCommandHandler(COM.MesureTraceCommand command, IExecutionContext context)
        {
            try
            {
                if (isRuning)
                {
                    /// если нужно проводим блокировку комманд который мы не сможем обслужить пока что то меряем в течении выполнени яэтой комманды
                    /// и подсказывая этим инфраструктуре что этот устрйостов некоторое время не сможет обрабатываить такие комманды
                    //context.Lock(CommandType.MesureTrace);

                    // если нужно заблокировать выполняему комманду то достатчоно вызвать метод без параметров и блокируется комманда которая выполняется
                    context.Lock();

                    if (idleState)
                    {
                        StatusError(AdapterDriver.bbAbort(deviceId), context);
                        idleState = false;
                    }

                    ValidateAndSetFreqStartStop(command.Parameter.FreqStart_Hz, command.Parameter.FreqStop_Hz, context);

                    ValidateAndSetAttRefLevel(command.Parameter.Att_dB, command.Parameter.RefLevel_dBm, context);

                    ValidateAndSetGain(command.Parameter.PreAmp_dB, context);

                    ValidateAndSetRBWVBWTracePointSweepTime(command.Parameter.RBW_Hz, command.Parameter.VBW_Hz, command.Parameter.TracePoint, command.Parameter.SweepTime_s, context);

                    ValidateAndSetTraceCount(command.Parameter.TraceCount, context);

                    ValidateAndSetTraceDetectors(command.Parameter.DetectorType, context);


                    traceType = lpc.TraceType(command.Parameter.TraceType);
                    LevelUnit = lpc.LevelUnit(command.Parameter.LevelUnit);

                    if (deviceMode != EN.Mode.Sweeping || flagMode != EN.Flag.StreamIQ)
                    {
                        deviceMode = EN.Mode.Sweeping;
                        flagMode = EN.Flag.StreamIQ;
                    }
                    StatusError(AdapterDriver.bbInitiate(deviceId, (uint)deviceMode, (uint)flagMode), context);

                    idleState = true;
                    //Меряем

                    GetAndPushTraceResults(command, context);

                    if (idleState)
                    {
                        StatusError(AdapterDriver.bbAbort(deviceId), context);
                        idleState = false;
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
                    throw new Exception("The device with serial number " + deviceSerialNumber + " does not work");
                }
            }
            catch (Exception e)
            {
                try//знаю что плохо но нужно освободить железо если че
                {
                    if (idleState)
                    {
                        StatusError(AdapterDriver.bbAbort(deviceId), context);
                        idleState = false;
                    }
                }
                catch { }
                // желательно записать влог
                logger.Exception(Contexts.ThisComponent, e);
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
                if (isRuning)
                {
                    //context.Lock(CommandType.MesureIQStream);
                    context.Lock();

                    if (idleState)
                    {
                        StatusError(AdapterDriver.bbAbort(deviceId), context);
                        idleState = false;
                    }
                    ValidateAndSetFreqStartStop(command.Parameter.FreqStart_Hz, command.Parameter.FreqStop_Hz, context);

                    ValidateAndSetAttRefLevel(command.Parameter.Att_dB, command.Parameter.RefLevel_dBm, context);

                    ValidateAndSetGain(command.Parameter.PreAmp_dB, context);

                    downsampleFactor = lpc.IQDownsampleFactor(command.Parameter.BitRate_MBs, FreqSpan);
                    StatusError(AdapterDriver.bbConfigureIQ(deviceId, downsampleFactor, (double)FreqSpan), context);

                    if (deviceMode != EN.Mode.Streaming || flagMode != EN.Flag.StreamIQ)
                    {
                        deviceMode = EN.Mode.Streaming;
                        flagMode = EN.Flag.StreamIQ;
                    }
                    StatusError(AdapterDriver.bbInitiate(deviceId, (uint)deviceMode, (uint)flagMode), context);

                    (double BlockDuration, double ReceiveTime) = lpc.IQTimeParameters(command.Parameter.IQBlockDuration_s, command.Parameter.IQReceivTime_s);

                    //если доступенн ППС или если недоступен и ненужен
                    if ((command.Parameter.MandatoryPPS && mainConfig.AvailabilityPPS) || !command.Parameter.MandatoryPPS)
                    {
                        return_len = 0; samples_per_sec = 0; bandwidth = 0.0;
                        StatusError(AdapterDriver.bbQueryStreamInfo(deviceId, ref return_len, ref bandwidth, ref samples_per_sec), context);

                        idleState = true;

                        //инициализация перед пуском, подготавливаемся к приему данных 
                        COMR.MesureIQStreamResult result = new COMR.MesureIQStreamResult(0, CommandResultStatus.Final)
                        {
                            DeviceStatus = COMR.Enums.DeviceStatus.Normal
                        };
                        TempIQData tiq = new TempIQData();
                        InitialReceivedIQStream(ref result, ref tiq, BlockDuration, ReceiveTime, command.Parameter.TimeStart * 100);
                        //закончили подготовку

                        //запустимся как сможем пораньше и принимаем все
                        if (GetIQStream(ref result, tiq, context, command.Parameter.MandatoryPPS, command.Parameter.MandatorySignal))
                        {
                            //пушаем
                            context.PushResult(result);
                        }
                    }
                    else
                    {
                        throw new Exception("According to the PPS configuration is not available");
                    }
                    if (idleState)
                    {
                        StatusError(AdapterDriver.bbAbort(deviceId), context);
                        idleState = false;
                    }
                    context.Unlock();
                    context.Finish();
                }
                else
                {
                    throw new Exception("The device with serial number " + deviceSerialNumber + " does not work");
                }
            }
            catch (Exception e)
            {
                try//знаю что плохо но нужно освободить железо если че
                {
                    if (idleState)
                    {
                        StatusError(AdapterDriver.bbAbort(deviceId), context);
                        idleState = false;
                    }
                }
                catch { }
                // желательно записать влог
                logger.Exception(Contexts.ThisComponent, e);
                // этот вызов обязательный в случаи обрыва
                context.Abort(e);
                // дальше кода быть не должно, освобождаем поток
            }

        }

        #region Param
        private readonly long uTCOffset = 621355968000000000;
        public EN.Status Status = EN.Status.NoError;
        private EN.Mode deviceMode = EN.Mode.Sweeping;
        private EN.Flag flagMode = EN.Flag.StreamIQ;
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
        private EN.Rejection rejection = EN.Rejection.NoSpurReject;
        private EN.RBWShape rBWShape = EN.RBWShape.Shape_FlatTop;
        public double RBWMax = 10000000;
        public double RBW = 10000;
        public double[] RBWArr = new double[] { 1, 3, 30, 100, 300, 1000, 3000, 10000, 30000, 100000, 300000, 1000000, 3000000, 10000000 };

        public readonly double VBWMax = 10000000;
        public double VBW = 10000;
        #endregion RBW / VBW

        #region Sweep
        public readonly double SweepTimeMin = 0.00001;
        public readonly double SweepTimeMax = 1.0;
        public double SweepTime = 0.00001;
        #endregion

        #region Levels
        /// <summary>
        /// Перегруз прибора
        /// 0 = normal
        /// 1 = RF Overload
        /// </summary>
        public int RFOverload = 0;

        #region уровни отображения
        public double RefLevel = -40;


        private double Range = 100;

        public double LowestLevel { get; set; } = -140;
        #endregion
        private EN.Scale Scale = EN.Scale.LogScale;
        public MEN.LevelUnit LevelUnit = MEN.LevelUnit.dBm;
        private EN.Gain Gain = EN.Gain.Gain_AUTO;
        private EN.Attenuator Attenuator = EN.Attenuator.Atten_AUTO;
        #endregion

        #region Trace Data
        //private bool NewTrace;
        private int tracePointsMaxPool = 0;
        private double resFreqStart = 10000;
        private double resFreqStep = 10000;
        private double traceFreqStart = 0; //предвдущего прохода
        private int tracePoints = 1601;

        private ulong traceCountToMeas = 1;
        private ulong traceCount = 1;

        private float[] sweepMax = new float[10000], sweepMin = new float[10000];
        //public double[] FreqArr;
        public float[] LevelArr;

        private bool traceReset;
        private AveragedTrace traceAveraged = new AveragedTrace();



        #region Trace
        public EN.Detector DetectorUse = EN.Detector.MaxOnly;
        public EN.Detector DetectorToSet = EN.Detector.MinAndMax;
        private EN.TraceType traceType = EN.TraceType.ClearWrite;
        private EN.Unit videoUnit = EN.Unit.Log;
        #endregion
        #endregion

        #region runs
        private bool isRuning;
        private long lastUpdate;
        #endregion

        #region Device info
        public readonly decimal FreqMin = 9000;
        public readonly decimal FreqMax = 6000000000;//6400000000;


        private bool idleState = false;

        public float DeviceBoardTemp;
        public float DeviceUSBVoltage;
        public float DeviceUSBCurrent;

        private int deviceId = -1;

        private string deviceType = "";

        private uint deviceSerialNumber = 0;

        private int deviceFirmwareVersion = 0;

        private int[] deviceFirmwareVersionActual = new int[] { 7, 8 };

        private string deviceAPIVersion = "";

        private readonly string deviceAPIVersionActual = "4.2.0";
        #endregion

        #region IQStream
        private int downsampleFactor;
        private int return_len;
        private int samples_per_sec;
        private double bandwidth;
        public decimal TriggerOffset = 10000;
        public decimal OneSampleDuration;
        #endregion IQStream

        #endregion Param

        #region Private Method
        private bool ValidateAdapterConfig(AdapterConfig adapterConfig)
        {
            bool res = true;
            if (adapterConfig.SerialNumber == int.MinValue ||
                adapterConfig.SerialNumber == int.MaxValue ||
                adapterConfig.SerialNumber == 0)
            {
                logger.Exception(Contexts.ThisComponent, new Exception("The serial number must be set correctly."));
                res = false;
            }
            return res;
        }

        private IResultPoolDescriptor<COMR.MesureTraceResult>[] ValidateAdapterTracePoolMainConfig(CFG.AdapterResultPool[] adapterTraceResultPools, string fileName)
        {
            IResultPoolDescriptor<COMR.MesureTraceResult>[] rpd = new IResultPoolDescriptor<COMR.MesureTraceResult>[adapterTraceResultPools.Length];
            for (int i = 0; i < adapterTraceResultPools.Length; i++)
            {
                if (adapterTraceResultPools[i].Size >= tracePointsMaxPool)
                {
                    tracePointsMaxPool = adapterTraceResultPools[i].Size;
                }
                bool find = false;
                for (int n = 0; n < adapterTraceResultPools.Length; n++)
                {
                    if ((n!=i) && (adapterTraceResultPools[n].KeyName == adapterTraceResultPools[i].KeyName))
                    {
                        find = true;
                    }
                }
                if (!find)
                {
                    var cnt = adapterTraceResultPools[i].Size;
                    rpd[i] = new ResultPoolDescriptor<COMR.MesureTraceResult>()
                    {
                        Key = adapterTraceResultPools[i].KeyName,
                        MinSize = adapterTraceResultPools[i].MinSize,
                        MaxSize = adapterTraceResultPools[i].MaxSize,
                        Factory = () =>
                        {
                            var result = new COMR.MesureTraceResult()
                            {
                                Level = new float[cnt]
                            };
                            return result;
                        }
                    };
                }
                else
                {
                    throw new Exception("An element with a duplicate name was found in the AdapterTraceResultPools configuration. File Name \"" + fileName + "\"");
                }
            }
            return rpd;
        }

        private void ValidateAndSetFreqStartStop(decimal freqStart, decimal freqStop, IExecutionContext context)
        {
            if (FreqStart != freqStart || FreqStop != freqStop)
            {
                FreqStart = lpc.FreqStart(this, freqStart);
                FreqStop = lpc.FreqStop(this, freqStop);
                StatusError(AdapterDriver.bbConfigureCenterSpan(deviceId, (double)FreqCentr, (double)FreqSpan), context);
            }
        }

        private void ValidateAndSetAttRefLevel(int att, int refLefel, IExecutionContext context)
        {
            EN.Attenuator at = lpc.Attenuator(att);
            double refl = lpc.RefLevel(refLefel);
            if (Attenuator != at || RefLevel != refl)
            {
                Attenuator = at;
                RefLevel = refl;

                StatusError(AdapterDriver.bbConfigureLevel(deviceId, RefLevel, (double)Attenuator), context);
            }
        }

        private void ValidateAndSetGain(int gain, IExecutionContext context)
        {
            EN.Gain g = lpc.Gain(gain);
            if (g != Gain)
            {
                Gain = g;
                StatusError(AdapterDriver.bbConfigureGain(deviceId, (int)Gain), context);
            }
        }

        private void ValidateAndSetRBWVBWTracePointSweepTime(double rbw, double vbw, int tracePoint, double sweepTime, IExecutionContext context)
        {
            sweepTime = lpc.SweepTime(this, sweepTime);
            if (tracePoint > tracePointsMaxPool)
            {
                throw new Exception("TracePoint exceeds pool size. Max TracePoint " + tracePointsMaxPool.ToString());
            }

            if (rbw < 0)
            {
                //decimal[] natrbw = new decimal[] { 0.301m, 0.602m, 1.204m, 2.4m, 4.81m, 9.63m, 19.26m, 38.52m, 77.05m, 154.11m, 308.22m, 616.45m, 1232, 2465, 4931, 9863, 19720, 39450, 78900, 157100, 315600, 631200, 1262000, 2525000, 5050000, 10100000};
                double[] ar = new double[] { 0.0078125, 0.015625, 0.03125, 0.0625, 0.125, 0.25, 0.5, 1, 2, 4, 8, 16, 32, 64, 128, 256, 512, 1024, 2048, 4096, 8192, 16384, 32768, 65536, 131072, 262144, 524288, 1048576, 2097152, 4194304, 8388608, 16777216, 33554432 };
                double magic = 38.146966101334357; //9.536741525333588m;
                double m1 = (double)FreqSpan / tracePoint;//хотим
                double m2 = m1 / magic;
                double delta = double.MaxValue;
                int index = 0;
                for (int i = 0; i < ar.Length; i++)
                {
                    double temp = Math.Abs(ar[i] - m2);
                    if (temp < delta)
                    {
                        delta = temp;
                        index = i;
                    }
                }
                m2 = ar[index];
                double d = (double)FreqSpan / magic / m2;
                if (((int)d) < tracePoint && index > 0)
                {
                    m2 = ar[index - 1];
                }
                double rbw2 = magic * m2 * 4;// (FreqSpan / command.Parameter.TracePoint) * 4.0m;
                if (rbw2 > RBWMax)
                {
                    rbw2 = RBWMax;
                }
                double vbw2 = 0;
                if (vbw < 0)
                {
                    vbw2 = rbw2;
                }
                else
                {
                    vbw2 = lpc.VBW(this, vbw);
                }
                RBW = rbw2;
                VBW = vbw2;
                if (sweepTime != SweepTime)
                {
                    SweepTime = sweepTime;
                }
                StatusError(AdapterDriver.bbConfigureSweepCoupling(deviceId, RBW, VBW, SweepTime, (uint)rBWShape, (uint)rejection));

            }
            else
            {
                double rbw2 = lpc.RBW(this, rbw);
                double vbw2 = lpc.VBW(this, vbw);
                if (RBW != rbw2 || VBW != vbw2 || SweepTime != sweepTime)
                {
                    RBW = rbw2;
                    VBW = vbw2;
                    SweepTime = sweepTime;
                    StatusError(AdapterDriver.bbConfigureSweepCoupling(deviceId, RBW, VBW, SweepTime, (uint)rBWShape, (uint)rejection), context);
                }
            }
        }

        private void ValidateAndSetTraceCount(int traceCount, IExecutionContext context)
        {
            if (traceCount > 0)
            {
                traceCountToMeas = (ulong)traceCount;//т.к. отсюда берется индекс результата
                this.traceCount = 0;
            }
            else
            {
                throw new Exception("TraceCount must be set greater than zero.");
            }
        }

        private void ValidateAndSetTraceDetectors(PEN.DetectorType detectorType, IExecutionContext context)
        {
            (DetectorUse, DetectorToSet) = lpc.DetectorType(detectorType);
            StatusError(AdapterDriver.bbConfigureAcquisition(deviceId, (uint)DetectorToSet, (uint)Scale), context);
        }


        private bool Connect(int serialNumber)
        {
            bool res = false;
            /// включем устройство
            /// иницируем его параметрами сконфигурации
            /// проверяем к чем оно готово

            bool err = StatusError(AdapterDriver.bbOpenDeviceBySerialNumber(ref deviceId, serialNumber));
            if (!err)
            {
                throw new Exception("Error: Unable to open BB60. Status: " + AdapterDriver.bbGetStatusString(Status));
            }
            else
            {
                deviceType = AdapterDriver.bbGetDeviceName(deviceId);

                StatusError(AdapterDriver.bbGetSerialNumber(deviceId, ref deviceSerialNumber));
                deviceAPIVersion = AdapterDriver.bbGetAPIString();
                if (deviceAPIVersion != deviceAPIVersionActual)
                {
                    throw new Exception("Unsupported version of API SignalHound");
                }
                StatusError(AdapterDriver.bbGetFirmwareVersion(deviceId, ref deviceFirmwareVersion));

                bool Firmware = false;
                for (int i = 0; i < deviceFirmwareVersionActual.Length; i++)
                {
                    if (!Firmware && deviceFirmwareVersion == deviceFirmwareVersionActual[i])
                    {
                        Firmware = true;
                    }
                }
                if (!Firmware)
                {
                    throw new Exception("Unsupported firmware version");
                }

                GetSystemInfo();
                SetTraceDetectorAndScale();
                SetFreqCentrSpan();
                SetRefATT();
                SetGain();
                SetRbwVbwSweepTimeRbwType();
                SetPortType();

                StatusError(AdapterDriver.bbInitiate(deviceId, (uint)deviceMode, (uint)EN.Flag.TimeStamp));
                isRuning = true;
                idleState = true;
                res = true;
            }
            return res;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="status"></param>
        /// <param name="context"></param>
        /// <returns>true = NoErrore, false = AnyError</returns>
        private bool StatusError(EN.Status status, IExecutionContext context)
        {
            Status = status;
            bool res = true;
            if (status != EN.Status.NoError)
            {
                logger.Warning(Contexts.ThisComponent, AdapterDriver.bbGetStatusString(Status));
                res = false;
                if (status == EN.Status.ADCOverflow)
                {
                    RFOverload = 1;
                }
            }
            else
            {
                RFOverload = 0;
            }
            //Приближенно при этих ошибках необходимо переподключить устройство, возможно еще какие-то есть
            if (status == EN.Status.DeviceConnectionErr || status == EN.Status.DeviceInvalidErr)
            {
                //оповестим о проблемах с устройством, и перезапустим его
                var result = new COMR.MesureTraceResult(0, CommandResultStatus.Ragged)
                {
                    DeviceStatus = COMR.Enums.DeviceStatus.StartReset
                };
                context.PushResult(result);

                try
                {
                    //отключим SignalHound по USB 
                    DeviceReset.SignalHoundBB60StatePrepared(adapterConfig.SerialNumber.ToString(), false);
                    Thread.Sleep(5000);//надо подождать пока система все сделает
                }
                catch (Exception exp)
                {
                    logger.Exception(Contexts.ThisComponent, exp);
                }

                try
                {
                    //подключим SignalHound по USB 
                    DeviceReset.SignalHoundBB60StatePrepared(adapterConfig.SerialNumber.ToString(), true);
                    Thread.Sleep(5000);//надо подождать пока система все сделает
                }
                catch (Exception exp)
                {
                    logger.Exception(Contexts.ThisComponent, exp);
                }

                try
                {
                    Disconnect();
                    if (Connect(adapterConfig.SerialNumber))
                    {
                        //оповестим о завершении перезапуска устройства
                        var result2 = new COMR.MesureTraceResult(0, CommandResultStatus.Final)
                        {
                            DeviceStatus = COMR.Enums.DeviceStatus.FinishReset
                        };
                        context.PushResult(result2);
                    }
                }
                catch (Exception exp)
                {
                    logger.Exception(Contexts.ThisComponent, exp);
                    context.Abort(exp);
                }
            }
            return res;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="status"></param>
        /// <returns>true = NoErrore, false = AnyError</returns>
        private bool StatusError(EN.Status status)
        {
            Status = status;
            bool res = true;
            if (status != EN.Status.NoError)
            {
                logger.Warning(Contexts.ThisComponent, AdapterDriver.bbGetStatusString(Status));
                res = false;
                if (status == EN.Status.ADCOverflow)
                {
                    RFOverload = 1;
                }
            }
            else
            {
                RFOverload = 0;
            }
            //Приближенно при этих ошибках необходимо переподключить устройство, возможно еще какие-то есть
            if (status == EN.Status.DeviceConnectionErr || status == EN.Status.DeviceInvalidErr)
            {
                try
                {
                    //отключим SignalHound по USB 
                    DeviceReset.SignalHoundBB60StatePrepared(adapterConfig.SerialNumber.ToString(), false);
                    Thread.Sleep(5000);//надо подождать пока система все сделает
                }
                catch (Exception exp)
                {
                    logger.Exception(Contexts.ThisComponent, exp);
                }

                try
                {
                    //подключим SignalHound по USB 
                    DeviceReset.SignalHoundBB60StatePrepared(adapterConfig.SerialNumber.ToString(), true);
                    Thread.Sleep(5000);//надо подождать пока система все сделает
                }
                catch (Exception exp)
                {
                    logger.Exception(Contexts.ThisComponent, exp);
                }

                try
                {
                    Disconnect();
                    Connect(adapterConfig.SerialNumber);
                }
                catch (Exception exp)
                {
                    logger.Exception(Contexts.ThisComponent, exp);
                }
            }
            return res;
        }

        private void GetSystemInfo()
        {
            try
            {
                StatusError(AdapterDriver.bbGetDeviceDiagnostics(deviceId, ref DeviceBoardTemp, ref DeviceUSBVoltage, ref DeviceUSBCurrent));
            }
            #region Exception
            catch (Exception exp)
            {
                logger.Exception(Contexts.ThisComponent, exp);
            }
            #endregion
        }
        private void SetTraceDetectorAndScale()
        {
            try
            {
                StatusError(AdapterDriver.bbConfigureAcquisition(deviceId, (uint)DetectorToSet, (uint)Scale));
            }
            #region Exception
            catch (Exception exp)
            {
                logger.Exception(Contexts.ThisComponent, exp);
            }
            #endregion
        }
        private void SetFreqCentrSpan()
        {
            try
            {
                StatusError(AdapterDriver.bbConfigureCenterSpan(deviceId, (double)FreqCentr, (double)FreqSpan));
            }
            #region Exception
            catch (Exception exp)
            {
                logger.Exception(Contexts.ThisComponent, exp);
            }
            #endregion
        }
        private void SetRefATT()
        {
            try
            {
                StatusError(AdapterDriver.bbConfigureLevel(deviceId, RefLevel, (double)Attenuator));
            }
            #region Exception
            catch (Exception exp)
            {
                logger.Exception(Contexts.ThisComponent, exp);
            }
            #endregion
        }
        private void SetGain()
        {
            try
            {
                StatusError(AdapterDriver.bbConfigureGain(deviceId, (int)Gain));
            }
            #region Exception
            catch (Exception exp)
            {
                logger.Exception(Contexts.ThisComponent, exp);
            }
            #endregion
        }
        private void SetRbwVbwSweepTimeRbwType()
        {
            try
            {
                StatusError(AdapterDriver.bbConfigureSweepCoupling(deviceId, RBW, VBW, SweepTime, (uint)rBWShape, (uint)rejection));
            }
            #region Exception
            catch (Exception exp)
            {
                logger.Exception(Contexts.ThisComponent, exp);
            }
            #endregion
        }
        private void SetPortType()
        {
            try
            {
                uint port1 = 0;
                uint port2 = 0;
                if (adapterConfig.GPSPPSConnected)
                {
                    port2 = (uint)EN.Port2.InTriggerRisingEdge;
                }
                if (adapterConfig.Reference10MHzConnected)
                {
                    port1 = (uint)EN.Port1.ExtRefIn;
                }
                StatusError(AdapterDriver.bbConfigureIO(deviceId, port1, port2));
                //пока неработает
                //if (_adapterConfig.SyncCPUtoGPS)
                //{
                //    if (_adapterConfig.GPSPortNumber > -1 && _adapterConfig.GPSPortBaudRate > 1000)
                //    {
                //        StatusError(AdapterDriver.bbSyncCPUtoGPS(_adapterConfig.GPSPortNumber, _adapterConfig.GPSPortBaudRate));
                //        DeviceMode = EN.Mode.Streaming;
                //        StatusError(AdapterDriver.bbInitiate(_Device_ID, (uint)DeviceMode, (uint)EN.Flag.StreamIQ | (uint)EN.Flag.TimeStamp));

                //        //int dataRemaining = 0, sampleLoss = 0, iqSec = 0, iqNano = 0;
                //        //StatusError(AdapterDriver.bbQueryStreamInfo(_Device_ID, ref return_len, ref bandwidth, ref samples_per_sec));
                //        //float[] iqSamplesX = new float[return_len * 2];
                //        //int[] TrDataTemp = new int[71];
                //        //AdapterDriver.bbIQPacket bbIQPacket = new AdapterDriver.bbIQPacket()
                //        //{
                //        //    iqData = new float[return_len * 2],
                //        //    triggers = new int[71],
                //        //    iqCount = 0,
                //        //    triggerCount = 0,
                //        //    purge = true,
                //        //    dataRemaining = 0,
                //        //    iqNano = 0,
                //        //    iqSec = 0,
                //        //    sampleLoss = 0
                //        //};
                //        //Thread.Sleep(2000);
                //        //StatusError(AdapterDriver.bbGetIQ(_Device_ID, ref bbIQPacket));
                //    }
                //}
            }
            #region Exception
            catch (Exception exp)
            {
                logger.Exception(Contexts.ThisComponent, exp);
            }
            #endregion
        }
        private void SetVideoUnits()
        {
            try
            {
                StatusError(AdapterDriver.bbConfigureProcUnits(deviceId, (uint)videoUnit));
            }
            #region Exception
            catch (Exception exp)
            {
                logger.Exception(Contexts.ThisComponent, exp);
            }
            #endregion
        }

        private void GetAndPushTraceResults(COM.MesureTraceCommand command, IExecutionContext context)
        {
            float adj = (float)(10 * Math.Log10(resFreqStep / RBW));
            string poolKeyName = "";
            bool poolKeyFind = false;
            //Если TraceType ClearWrite то пушаем каждый результат
            if (traceType == EN.TraceType.ClearWrite)
            {
                for (ulong i = 0; i < traceCountToMeas; i++)
                {
                    if (GetTrace())
                    {
                        // пушаем результат
                        traceCount++;
                        if (!poolKeyFind)
                        {
                            FindTracePoolName(LevelArr.Length, ref poolKeyFind, ref poolKeyName);
                        }
                        COMR.MesureTraceResult result;
                        if (traceCountToMeas == traceCount)
                        {
                            result = context.TakeResult<COMR.MesureTraceResult>(poolKeyName, traceCount, CommandResultStatus.Final);
                        }
                        else
                        {
                            result = context.TakeResult<COMR.MesureTraceResult>(poolKeyName, traceCount, CommandResultStatus.Next);
                        }

                        if (command.Parameter.RBW_Hz == -2)
                        {
                            for (int j = 0; j < LevelArr.Length; j++)
                            {
                                result.Level[j] = LevelArr[j] + adj;
                            }
                        }
                        else
                        {
                            for (int j = 0; j < LevelArr.Length; j++)
                            {
                                result.Level[j] = LevelArr[j];
                            }
                        }
                        result.LevelMaxIndex = LevelArr.Length;
                        result.FrequencyStart_Hz = resFreqStart;
                        result.FrequencyStep_Hz = resFreqStep;

                        result.TimeStamp = timeService.GetGnssUtcTime().Ticks - uTCOffset;// new DateTime(1970, 1, 1, 0, 0, 0, System.DateTimeKind.Utc).Ticks;//неюзабельно
                                                                                          //result.TimeStamp = DateTime.UtcNow.Ticks - new DateTime(1970, 1, 1, 0, 0, 0, System.DateTimeKind.Utc).Ticks;
                        result.Att_dB = lpc.Attenuator(Attenuator);
                        result.PreAmp_dB = lpc.Gain(Gain);
                        result.RefLevel_dBm = (int)RefLevel;
                        result.RBW_Hz = RBW;
                        result.VBW_Hz = VBW;
                        result.DeviceStatus = COMR.Enums.DeviceStatus.Normal;
                        if (RFOverload == 1)
                        {
                            result.DeviceStatus = COMR.Enums.DeviceStatus.RFOverload;
                        }
                        context.PushResult(result);
                    }

                    // иногда нужно проверять токен окончания работы комманды
                    if (context.Token.IsCancellationRequested)
                    {
                        // все нужно остановиться

                        // если есть порция данных возвращаем ее в обработчки только говрим что поток результатов не законченный и больше уже не будет поступать
                        var result2 = context.TakeResult<COMR.MesureTraceResult>(poolKeyName, traceCount, CommandResultStatus.Ragged);
                        result2.DeviceStatus = COMR.Enums.DeviceStatus.Normal;
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
                traceReset = true;///сбросим предыдущие результаты
                if (traceType == EN.TraceType.Average)//назначим сколько усреднять
                {
                    traceAveraged.AveragingCount = (int)traceCountToMeas;
                }
                bool _RFOverload = false;
                for (ulong i = 0; i < traceCountToMeas; i++)
                {

                    if (GetTrace())
                    {
                        traceCount++;
                        if (RFOverload == 1)
                        {
                            _RFOverload = true;
                        }
                    }
                    // иногда нужно проверять токен окончания работы комманды
                    if (context.Token.IsCancellationRequested)
                    {
                        // все нужно остановиться

                        // если есть порция данных возвращаем ее в обработчки только говрим что поток результатов не законченный и больше уже не будет поступать
                        if (!poolKeyFind)
                        {
                            FindTracePoolName(LevelArr.Length, ref poolKeyFind, ref poolKeyName);
                        }
                        var result2 = context.TakeResult<COMR.MesureTraceResult>(poolKeyName, traceCount, CommandResultStatus.Ragged);
                        result2.DeviceStatus = COMR.Enums.DeviceStatus.Normal;
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
                    if (!poolKeyFind)
                    {
                        FindTracePoolName(LevelArr.Length, ref poolKeyFind, ref poolKeyName);
                    }
                    var result = context.TakeResult<COMR.MesureTraceResult>(poolKeyName, traceCount, CommandResultStatus.Final);

                    if (command.Parameter.RBW_Hz == -2)
                    {
                        for (int j = 0; j < LevelArr.Length; j++)
                        {
                            result.Level[j] = LevelArr[j] + adj;
                        }
                    }
                    else
                    {
                        for (int j = 0; j < LevelArr.Length; j++)
                        {
                            result.Level[j] = LevelArr[j];
                        }
                    }
                    result.LevelMaxIndex = LevelArr.Length;
                    result.FrequencyStart_Hz = resFreqStart;
                    result.FrequencyStep_Hz = resFreqStep;

                    result.TimeStamp = timeService.GetGnssUtcTime().Ticks - uTCOffset;// new DateTime(1970, 1, 1, 0, 0, 0, System.DateTimeKind.Utc).Ticks;//неюзабельно
                                                                                      //result.TimeStamp = DateTime.UtcNow.Ticks - new DateTime(1970, 1, 1, 0, 0, 0, System.DateTimeKind.Utc).Ticks;
                    result.Att_dB = lpc.Attenuator(Attenuator);
                    result.PreAmp_dB = lpc.Gain(Gain);
                    result.RefLevel_dBm = (int)RefLevel;
                    result.RBW_Hz = RBW;
                    result.VBW_Hz = VBW;

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
        }
        private void FindTracePoolName(int size, ref bool state, ref string name)
        {
            if (size > tracePointsMaxPool)
            {
                throw new Exception("TracePoint exceeds pool size. Max TracePoint " + tracePointsMaxPool.ToString());
            }
            else
            {
                for (int p = 0; p < mainConfig.AdapterTraceResultPools.Length; p++)
                {
                    if (mainConfig.AdapterTraceResultPools[p].Size >= size && !state)
                    {
                        name = mainConfig.AdapterTraceResultPools[p].KeyName;
                        state = true;
                        break;
                    }
                }
            }
        }

        private bool GetTrace()
        {
            bool res = true;
            //получаем спектр
            uint traceLen = 0;

            StatusError(AdapterDriver.bbQueryTraceInfo(deviceId, ref traceLen, ref resFreqStep, ref resFreqStart));

            if (Status != EN.Status.DeviceConnectionErr ||
                Status != EN.Status.DeviceInvalidErr ||
                Status != EN.Status.DeviceNotOpenErr ||
                Status != EN.Status.USBTimeoutErr)
            {
                isRuning = true;
            }
            if (traceLen != sweepMax.Length)
            {
                sweepMax = new float[traceLen];
                sweepMin = new float[traceLen];
            }
            StatusError(AdapterDriver.bbFetchTrace_32f(deviceId, unchecked((int)traceLen), sweepMin, sweepMax));

            if (Status == EN.Status.DeviceConnectionErr)
            {
                res = false;
            }
            else
            {
                SetTraceData((int)traceLen, sweepMin, sweepMax, resFreqStart, resFreqStep);
                lastUpdate = DateTime.Now.Ticks;
            }

            return res;
        }
        private void SetTraceData(int newLength, float[] minTrace, float[] maxTrace, double freqStart, double step)
        {
            if (maxTrace.Length > 0 && newLength > 0 && step > 0)
            {
                if (LevelArr.Length != newLength || (Math.Abs(traceFreqStart - freqStart) >= step))
                {
                    traceFreqStart = freqStart;
                    tracePoints = newLength;
                    LevelArr = new float[newLength];
                    if (LevelUnit == MEN.LevelUnit.dBm)
                    {
                        if (DetectorUse == EN.Detector.Average || DetectorUse == EN.Detector.MaxOnly)
                        {
                            for (int i = 0; i < newLength; i++)
                            {
                                LevelArr[i] = maxTrace[i];
                            }
                        }
                        else if (DetectorUse == EN.Detector.MinOnly)
                        {
                            for (int i = 0; i < newLength; i++)
                            {
                                LevelArr[i] = minTrace[i];
                            }
                        }
                    }
                    else if (LevelUnit == MEN.LevelUnit.dBµV)
                    {
                        if (DetectorUse == EN.Detector.Average || DetectorUse == EN.Detector.MaxOnly)
                        {
                            for (int i = 0; i < newLength; i++)
                            {
                                LevelArr[i] = maxTrace[i] + 107;
                            }
                        }
                        else if (DetectorUse == EN.Detector.MinOnly)
                        {
                            for (int i = 0; i < newLength; i++)
                            {
                                LevelArr[i] = minTrace[i] + 107;
                            }
                        }
                    }
                }
                if (traceType == EN.TraceType.ClearWrite)
                {
                    if (LevelUnit == MEN.LevelUnit.dBm)
                    {
                        if (DetectorUse == EN.Detector.Average || DetectorUse == EN.Detector.MaxOnly)
                        {
                            for (int i = 0; i < newLength; i++)
                            {
                                LevelArr[i] = maxTrace[i];
                            }
                        }
                        else if (DetectorUse == EN.Detector.MinOnly)
                        {
                            for (int i = 0; i < newLength; i++)
                            {
                                LevelArr[i] = minTrace[i];
                            }
                        }
                    }
                    else if (LevelUnit == MEN.LevelUnit.dBµV)
                    {
                        if (DetectorUse == EN.Detector.Average || DetectorUse == EN.Detector.MaxOnly)
                        {
                            for (int i = 0; i < newLength; i++)
                            {
                                LevelArr[i] = maxTrace[i] + 107;
                            }
                        }
                        else if (DetectorUse == EN.Detector.MinOnly)
                        {
                            for (int i = 0; i < newLength; i++)
                            {
                                LevelArr[i] = minTrace[i] + 107;
                            }
                        }
                    }
                }
                else if (traceType == EN.TraceType.Average)//Average
                {
                    float[] levels = new float[newLength];
                    if (LevelUnit == MEN.LevelUnit.dBm)
                    {
                        if (DetectorUse == EN.Detector.Average || DetectorUse == EN.Detector.MaxOnly)
                        {
                            for (int i = 0; i < newLength; i++)
                            {
                                levels[i] = maxTrace[i];
                            }
                        }
                        else if (DetectorUse == EN.Detector.MinOnly)
                        {
                            for (int i = 0; i < newLength; i++)
                            {
                                levels[i] = minTrace[i];
                            }
                        }
                    }
                    else if (LevelUnit == MEN.LevelUnit.dBµV)
                    {
                        if (DetectorUse == EN.Detector.Average || DetectorUse == EN.Detector.MaxOnly)
                        {
                            for (int i = 0; i < newLength; i++)
                            {
                                levels[i] = maxTrace[i] + 107;
                            }
                        }
                        else if (DetectorUse == EN.Detector.MinOnly)
                        {
                            for (int i = 0; i < newLength; i++)
                            {
                                levels[i] = minTrace[i] + 107;
                            }
                        }
                    }
                    if (traceReset) { traceAveraged.Reset(); traceReset = false; }
                    traceAveraged.AddTraceToAverade(freqStart, step, levels, LevelUnit);
                    LevelArr = traceAveraged.AveragedLevels;

                }
                else if (traceType == EN.TraceType.MaxHold)
                {
                    if (traceReset == false)
                    {
                        if (LevelUnit == MEN.LevelUnit.dBm)
                        {
                            if (DetectorUse == EN.Detector.Average || DetectorUse == EN.Detector.MaxOnly)
                            {
                                for (int i = 0; i < newLength; i++)
                                {
                                    if (maxTrace[i] > LevelArr[i]) LevelArr[i] = maxTrace[i];
                                }
                            }
                            else if (DetectorUse == EN.Detector.MinOnly)
                            {
                                for (int i = 0; i < newLength; i++)
                                {
                                    if (minTrace[i] > LevelArr[i]) LevelArr[i] = minTrace[i];
                                }
                            }
                        }
                        else if (LevelUnit == MEN.LevelUnit.dBµV)
                        {
                            if (DetectorUse == EN.Detector.Average || DetectorUse == EN.Detector.MaxOnly)
                            {
                                for (int i = 0; i < newLength; i++)
                                {
                                    if (maxTrace[i] + 107 > LevelArr[i]) LevelArr[i] = maxTrace[i] + 107;
                                }
                            }
                            else if (DetectorUse == EN.Detector.MinOnly)
                            {
                                for (int i = 0; i < newLength; i++)
                                {
                                    if (minTrace[i] + 107 > LevelArr[i]) LevelArr[i] = minTrace[i] + 107;
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
                                    LevelArr[i] = maxTrace[i];
                                }
                            }
                            else if (DetectorUse == EN.Detector.MinOnly)
                            {
                                for (int i = 0; i < newLength; i++)
                                {
                                    LevelArr[i] = minTrace[i];
                                }
                            }
                        }
                        else if (LevelUnit == MEN.LevelUnit.dBµV)
                        {
                            if (DetectorUse == EN.Detector.Average || DetectorUse == EN.Detector.MaxOnly)
                            {
                                for (int i = 0; i < newLength; i++)
                                {
                                    LevelArr[i] = maxTrace[i] + 107;
                                }
                            }
                            else if (DetectorUse == EN.Detector.MinOnly)
                            {
                                for (int i = 0; i < newLength; i++)
                                {
                                    LevelArr[i] = minTrace[i] + 107;
                                }
                            }
                        }
                        traceReset = false;
                    }
                }
                else if (traceType == EN.TraceType.MinHold)
                {
                    if (traceReset == false)
                    {
                        if (LevelUnit == MEN.LevelUnit.dBm)
                        {
                            if (DetectorUse == EN.Detector.Average || DetectorUse == EN.Detector.MaxOnly)
                            {
                                for (int i = 0; i < newLength; i++)
                                {
                                    if (maxTrace[i] < LevelArr[i]) LevelArr[i] = maxTrace[i];
                                }
                            }
                            else if (DetectorUse == EN.Detector.MinOnly)
                            {
                                for (int i = 0; i < newLength; i++)
                                {
                                    if (minTrace[i] < LevelArr[i]) LevelArr[i] = minTrace[i];
                                }
                            }
                        }
                        else if (LevelUnit == MEN.LevelUnit.dBµV)
                        {
                            if (DetectorUse == EN.Detector.Average || DetectorUse == EN.Detector.MaxOnly)
                            {
                                for (int i = 0; i < newLength; i++)
                                {
                                    if (maxTrace[i] + 107 < LevelArr[i]) LevelArr[i] = maxTrace[i] + 107;
                                }
                            }
                            else if (DetectorUse == EN.Detector.MinOnly)
                            {
                                for (int i = 0; i < newLength; i++)
                                {
                                    if (minTrace[i] + 107 < LevelArr[i]) LevelArr[i] = minTrace[i] + 107;
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
                                    LevelArr[i] = maxTrace[i];
                                }
                            }
                            else if (DetectorUse == EN.Detector.MinOnly)
                            {
                                for (int i = 0; i < newLength; i++)
                                {
                                    LevelArr[i] = minTrace[i];
                                }
                            }
                        }
                        else if (LevelUnit == MEN.LevelUnit.dBµV)
                        {
                            if (DetectorUse == EN.Detector.Average || DetectorUse == EN.Detector.MaxOnly)
                            {
                                for (int i = 0; i < newLength; i++)
                                {
                                    LevelArr[i] = maxTrace[i] + 107;
                                }
                            }
                            else if (DetectorUse == EN.Detector.MinOnly)
                            {
                                for (int i = 0; i < newLength; i++)
                                {
                                    LevelArr[i] = minTrace[i] + 107;
                                }
                            }
                        }
                        traceReset = false;
                    }
                }
            }
        }

        private void InitialReceivedIQStream(ref COMR.MesureIQStreamResult iQStreamResult, ref TempIQData tempIQStream, double blockDuration, double receivTime, long timeStart)
        {
            // Формирование пустого места для записи данных
            tempIQStream.BlocksCount = (int)Math.Ceiling(blockDuration * samples_per_sec / return_len);
            tempIQStream.BlocksAll = (int)Math.Ceiling(receivTime * samples_per_sec / return_len);
            tempIQStream.TimeStart = timeStart;
            tempIQStream.IQData = new float[tempIQStream.BlocksCount][];
            tempIQStream.dataRemainings = new int[tempIQStream.BlocksCount];
            tempIQStream.sampleLosses = new int[tempIQStream.BlocksCount];

            tempIQStream.OneSempleDuration = 1000000000 / samples_per_sec;
            OneSampleDuration = ((decimal)tempIQStream.OneSempleDuration) / 1000000000;
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
#if DEBUG
        public float[] IQArr = new float[] { -1, -1, -1, -1 };//del
#endif
        private bool GetIQStream(ref COMR.MesureIQStreamResult iQStreamResult, TempIQData tempIQStream, IExecutionContext context, bool withPPS, bool justWithSignal)
        {
            bool done = false;
            bool isCancellationRequested = false;
            // расчет количества шагов которое мы должны записать. 

            int dataRemaining = 0, sampleLoss = 0, iqSec = 0, iqNano = 0;

            // Константы
            float noise = 0.0000004f; // уровень шума в mW^2
            float SN = 10; // превышение шума в разах 
            float trigerLevelPL = noise * SN;
            float trigerLevelMN = 0 - trigerLevelPL;
            // Конец констант 

            bool signalFound = false; //был ли сигнал
            int step = tempIQStream.IQData[0].Length / 1000;//шаг проверки уровней на предмет детектирования сигнала
            if (step < 1)
            {
                step = 1;
            }

            bool getBlockOnTime = false;//тру то принимаем блоки до конца, фолс если недостигли времени приема то пока принимаем PPS
            long blockTime = return_len * tempIQStream.OneSempleDuration; //Длительность одного блока в нс

            bool pPSDetected = false;//Детектирован ли PPS
            long timeToStartBlockWithPPS = 0;//Разница времени старта блока и PPS в нс, всегда положителен  (в блоке с PPS)            

            long prevBlockTime = 0;
            long thisBlockTime = 0;

            long gNSSOffset = timeService.TimeCorrection * 100;
            int dTPPSIndex = 0;//индекс PPS в BlockTime
            int iQStartIndex = 0;// иендекс Первого нужного IQ в BlockTime
            int iQStopIndex = 0;// иендекс Последнего нужного IQ в BlockTime

            int allBlockIndex = -1; //текущий принятый 
            int necessaryBlockIndex = -1;//Индекс нужного блока, т.е. того который в результаты

            bool receivedBlockWithErrors = false; //Есть ли ошибки в нужных блоках

            for (int i = 0; i <= tempIQStream.BlocksAll; i++)
            {
                allBlockIndex++;
                // снятие данных
                #region
                //полезные данные и до принимаем тут
                if (necessaryBlockIndex < tempIQStream.BlocksCount - 1)
                {
                    necessaryBlockIndex++;
                    if (necessaryBlockIndex == 0)
                    {
                        iQStartIndex = allBlockIndex;

                    }
                    StatusError(AdapterDriver.bbGetIQUnpacked(deviceId, tempIQStream.IQData[necessaryBlockIndex], return_len, tempIQStream.TrDataTemp, 71, 1,
                        ref dataRemaining, ref sampleLoss, ref iqSec, ref iqNano));

                    if (!receivedBlockWithErrors && iQStopIndex == 0 && necessaryBlockIndex == tempIQStream.BlocksCount - 1)
                    {
                        iQStopIndex = allBlockIndex;
                    }
                }
                else//уже приняли нужные данные, то сюда
                {
                    StatusError(AdapterDriver.bbGetIQUnpacked(deviceId, tempIQStream.IQDataTemp, return_len, tempIQStream.TrDataTemp, 71, 1,
                       ref dataRemaining, ref sampleLoss, ref iqSec, ref iqNano), context);
                }

                //Если вдруг принимаем данные с ошибками то генерируем ошибки, т.к. данные некоректны
                if (Status != EN.Status.NoError)
                {
                    throw new Exception(AdapterDriver.bbGetStatusString(Status));
                }
                #endregion

                tempIQStream.BlockTime[allBlockIndex] = ((long)iqSec) * 1000000000 + iqNano + gNSSOffset;
                prevBlockTime = thisBlockTime;
                thisBlockTime = tempIQStream.BlockTime[allBlockIndex];
                if (prevBlockTime != 0)
                {
                    tempIQStream.BlockTimeDelta[allBlockIndex] = thisBlockTime - prevBlockTime;
                }
                //определяем когда нужно начинать пытаться принять данные, попал ли этот блок на время начала приема
                if (withPPS)
                {
                    if (tempIQStream.TrDataTemp[0] > 0)//заново задетектили PPS то все сбросим и начнем считать занов
                    {
                        pPSDetected = true;
                        timeToStartBlockWithPPS = tempIQStream.TrDataTemp[0] * tempIQStream.OneSempleDuration;

                        dTPPSIndex = allBlockIndex;///установили в каком блоке был ППС
                    }
                }
                if (!getBlockOnTime)
                {
                    // Этот блок попал на время старта, начинаем слушать и сюда не возвращаемся
                    if (tempIQStream.BlockTime[allBlockIndex] <= tempIQStream.TimeStart && tempIQStream.TimeStart <= tempIQStream.BlockTime[allBlockIndex] + return_len * tempIQStream.OneSempleDuration)
                    {
                        getBlockOnTime = true;
                        iQStartIndex = allBlockIndex;
                    }
                    else
                    {
                        //провтыкали время старта
                        if (tempIQStream.BlockTime[allBlockIndex] + return_len * tempIQStream.OneSempleDuration > tempIQStream.TimeStart)
                        {
                            throw new Exception("The task was started after the required start time of the task.");//Задача была запущена после необходимого времени старта задачи
                        }
                        necessaryBlockIndex--;//т.к. не началось время прослушки данных то уменьшим индексм и на то же место запишем болок заново
                        i--;
                    }
                }

                if (getBlockOnTime)
                {
                    //проверяем наличие сигнала пока его не обнаружили
                    if (justWithSignal && !signalFound)
                    {
                        for (int j = 0; tempIQStream.IQData[necessaryBlockIndex].Length - 6 > j; j += step)
                        {
                            if (tempIQStream.IQData[necessaryBlockIndex][j] > trigerLevelPL || tempIQStream.IQData[necessaryBlockIndex][j + 1] > trigerLevelPL ||
                                tempIQStream.IQData[necessaryBlockIndex][j] < trigerLevelMN || tempIQStream.IQData[necessaryBlockIndex][j + 1] < trigerLevelMN)
                            {
                                if (tempIQStream.IQData[necessaryBlockIndex][j + 2] > trigerLevelPL || tempIQStream.IQData[necessaryBlockIndex][j + 3] > trigerLevelPL ||
                                    tempIQStream.IQData[necessaryBlockIndex][j + 2] < trigerLevelMN || tempIQStream.IQData[necessaryBlockIndex][j + 3] < trigerLevelMN)
                                {
                                    if (tempIQStream.IQData[necessaryBlockIndex][j + 4] > trigerLevelPL || tempIQStream.IQData[necessaryBlockIndex][j + 5] > trigerLevelPL ||
                                        tempIQStream.IQData[necessaryBlockIndex][j + 4] > trigerLevelMN || tempIQStream.IQData[necessaryBlockIndex][j + 5] > trigerLevelMN)
                                    {
                                        signalFound = true;//Есть сигнал 
                                        iQStartIndex = allBlockIndex;
                                        break;
                                    }
                                }
                            }
                        }
                        if (!signalFound)//небыло сигнала то уменьшим индекс и перезапишем этот блок
                        {
                            necessaryBlockIndex--;
                        }
                    }

                    //Вышли за отведенное время прослушки
                    if (i >= tempIQStream.BlocksAll)
                    {
                        //хотели c сигналом 
                        if (justWithSignal)
                        {
                            if (signalFound)//сигнал есть
                            {
                                if (necessaryBlockIndex != tempIQStream.BlocksCount - 1)//дослушаем необходимое время прослушки и выйдем
                                {
                                    i--;
                                }
                                else
                                {
                                    if (withPPS && pPSDetected)
                                    {
                                        break;
                                    }
                                }
                            }
                        }
                        else //Не хотели с сигналом и выйдем
                        {
                            if (withPPS && pPSDetected)
                            {
                                break;
                            }
                        }
                    }
                    //В этом блоке есть пропук во времени, установим индекс конца блоков и продолжим слушать, может еще PPS нежен
                    if (allBlockIndex > 0 && tempIQStream.OneSempleDuration != tempIQStream.BlockTimeDelta[allBlockIndex] / return_len)
                    {
                        if (!receivedBlockWithErrors && iQStopIndex == 0)
                        {
                            //хотим с сигналом и есть сигнал!!! до этого момента нам всеравно
                            if (justWithSignal && signalFound)
                            {
                                iQStopIndex = allBlockIndex - 1;
                                receivedBlockWithErrors = true;
                            }
                            else if (justWithSignal && !signalFound)//хотим с сигналом и до того как появился сигнал нам на ошибку всеравно!!!
                            {
                            }
                            else if (!justWithSignal) // просто хотим записать
                            {
                                iQStopIndex = allBlockIndex - 1;
                                receivedBlockWithErrors = true;
                            }
                        }

                        //IsCancellationRequested = true;
                    }
                }
            }
            //если не попросили завершить раньше времени то пилим результат
            if (!isCancellationRequested)
            {
                #region обработка полученных данных
                if (receivedBlockWithErrors)
                {
                    iQStreamResult = new COMR.MesureIQStreamResult(0, CommandResultStatus.Ragged)
                    {
                        iq_samples = new float[iQStopIndex - iQStartIndex][]
                    };
                    Array.Copy(tempIQStream.IQData, iQStreamResult.iq_samples, iQStopIndex - iQStartIndex);
                    if (iQStartIndex > iQStopIndex)
                    {
                        throw new Exception("Data received with error, no valid data.");//Задача была запущена после необходимого времени старта задачи
                    }
                    else
                    {

                    }
                }
                else
                {
                    iQStreamResult.iq_samples = tempIQStream.IQData;
                }

                iQStreamResult.TimeStamp = tempIQStream.BlockTime[iQStartIndex] / 100;// DateTime.UtcNow.Ticks - new DateTime(1970, 1, 1, 0, 0, 0, System.DateTimeKind.Utc).Ticks;
                iQStreamResult.OneSempleDuration_ns = tempIQStream.OneSempleDuration;
                iQStreamResult.DeviceStatus = COMR.Enums.DeviceStatus.Normal;
                if (justWithSignal && !signalFound) //Хотели сигнал но его небыло, согласно договоренности генерируем екзепшен
                {
                    throw new Exception("Signal not detected");
                }
                if (withPPS)//хотим с PPS
                {
                    if (pPSDetected)// PPS был за все время прослушки блоков
                    {
                        iQStreamResult.PPSTimeDifference_ns = timeToStartBlockWithPPS;
                        if (dTPPSIndex < iQStartIndex)
                        {
                            for (int t = dTPPSIndex; t < iQStartIndex; t++)
                            {
                                iQStreamResult.PPSTimeDifference_ns -= tempIQStream.BlockTimeDelta[t];
                            }
                        }
                        else if (iQStartIndex < dTPPSIndex)
                        {
                            for (int t = iQStartIndex; t < dTPPSIndex; t++)
                            {
                                iQStreamResult.PPSTimeDifference_ns += tempIQStream.BlockTimeDelta[t];
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
#if DEBUG
                IQArr = new float[tempIQStream.IQData.Length * tempIQStream.IQData[0].Length];
                for (int i = 0; i < iQStreamResult.iq_samples.Length; i++)
                {
                    Array.Copy(iQStreamResult.iq_samples[i], 0, IQArr, 0 + i * iQStreamResult.iq_samples[i].Length, iQStreamResult.iq_samples[i].Length);
                }
                TriggerOffset = ((decimal)iQStreamResult.PPSTimeDifference_ns) / 1000000000;
#endif
            }
            return done;
        }

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

            SetDefaulTraceResultPoolsConfig(ref config);
        }
        private void SetDefaulTraceResultPoolsConfig(ref CFG.AdapterMainConfig config)
        {
            config.AdapterTraceResultPools = new CFG.AdapterResultPool[]
            {
                new CFG.AdapterResultPool()
                {
                    KeyName = "small",
                    MinSize = 50,
                    MaxSize = 100,
                    Size = 5000
                },
                new CFG.AdapterResultPool()
                {
                    KeyName = "small10k",
                    MinSize = 50,
                    MaxSize = 100,
                    Size = 10000
                },
                new CFG.AdapterResultPool()
                {
                    KeyName = "small20k",
                    MinSize = 50,
                    MaxSize = 100,
                    Size = 20000
                },
                new CFG.AdapterResultPool()
                {
                    KeyName = "middle40k",
                    MinSize = 25,
                    MaxSize = 50,
                    Size = 40000
                },
                new CFG.AdapterResultPool()
                {
                    KeyName = "middle100k",
                    MinSize = 25,
                    MaxSize = 50,
                    Size = 100000
                },
                new CFG.AdapterResultPool()
                {
                    KeyName = "middle200k",
                    MinSize = 10,
                    MaxSize = 20,
                    Size = 200000
                },
                new CFG.AdapterResultPool()
                {
                    KeyName = "huge500k",
                    MinSize = 10,
                    MaxSize = 20,
                    Size = 500000
                },

            };
        }

        private (MesureTraceDeviceProperties, MesureIQStreamDeviceProperties) GetProperties(CFG.AdapterMainConfig config)
        {
            RadioPathParameters[] rrps = ConvertRadioPathParameters(config);
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
                    AntennaCode = config.AdapterEquipmentInfo.AntennaSN,// "Omni",//S/N  В конфиг
                    AntennaManufacturer = config.AdapterEquipmentInfo.AntennaManufacturer,//"3anet",//В конфиг
                    AntennaName = config.AdapterEquipmentInfo.AntennaName,//"BC600",//В конфиг
                    EquipmentManufacturer = new Atdi.DataModels.Sdrn.DeviceServer.Adapters.InstrManufacrures().SignalHound.UI,
                    EquipmentName = deviceType,
                    EquipmentFamily = "SDR",//SDR/SpecAn/MonRec
                    EquipmentCode = deviceSerialNumber.ToString(),//S/N
                },
                RadioPathParameters = rrps
            };
            MesureTraceDeviceProperties mtdp = new MesureTraceDeviceProperties()
            {
                RBWMax_Hz = RBWMax,
                RBWMin_Hz = 3,
                SweepTimeMin_s = SweepTimeMin,
                SweepTimeMax_s = SweepTimeMax,
                StandardDeviceProperties = sdp,
                //DeviceId ничего не писать, ID этого экземпляра адаптера
            };
            MesureIQStreamDeviceProperties miqdp = new MesureIQStreamDeviceProperties()
            {
                AvailabilityPPS = config.AvailabilityPPS, // В конфиг
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
