using Atdi.Contracts.Sdrn.DeviceServer;
using Atdi.Platform.Logging;
using CFG = Atdi.DataModels.Sdrn.DeviceServer.Adapters.Config;
using COM = Atdi.DataModels.Sdrn.DeviceServer.Commands;
using COMP = Atdi.DataModels.Sdrn.DeviceServer.Commands.Parameters;
using COMR = Atdi.DataModels.Sdrn.DeviceServer.Commands.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.DataModels.Sdrn.DeviceServer.Adapters;
using MEN = Atdi.DataModels.Sdrn.DeviceServer.Adapters.Enums;
using Atdi.DataModels.Sdrn.DeviceServer;

namespace Atdi.AppUnits.Sdrn.DeviceServer.Adapters.KTN6841A
{
    public class Adapter : IAdapter
    {
        private readonly ITimeService timeService;
        private readonly ILogger logger;
        private readonly AdapterConfig adapterConfig;

        private IExecutionContext executionContextGps;
        private ulong resultGPSpart = 0;
        private COMR.GpsResult resultGPS = null;
        private COM.Parameters.GpsMode stateGPS = COMP.GpsMode.Stop;
        private bool resultGPSPublished = false;

        private CFG.ThisAdapterConfig tac;
        private CFG.AdapterMainConfig mainConfig;

        public Adapter(AdapterConfig adapterConfig, ILogger logger, ITimeService timeService)
        {
            this.logger = logger;
            ValidateAndSetAdapterConfig(adapterConfig);
            this.adapterConfig = adapterConfig;
            this.timeService = timeService;
            //lpc = new LocalParametersConverter();
        }


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
                    string fileName = "KTN6841A_" + SensorInfo.serialNumber + ".xml";
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
                            logger.Verbouse(Contexts.ThisComponent, "AdapterTraceResultPools were not found in the configuration " +
                                "file and were replaced with the default values for this adapter.");
                        }
                        mainConfig = tac.Main;
                    }

                    ////SetTransducer(mainConfig);
                    (MesureTraceDeviceProperties mtdp, MesureIQStreamDeviceProperties miqdp) = GetProperties(mainConfig);

                    IResultPoolDescriptor<COMR.MesureTraceResult>[] rpd = ValidateAdapterTracePoolMainConfig(mainConfig.AdapterTraceResultPools, fileName);
                    LevelArr = new float[tracePointsMaxPool];
                    LevelArrTemp = new float[tracePointsMaxPool];
                    LevelArrTemp2 = new float[gSensorCapabilities.fftMaxBlocksize];


                    //levelArrTemp = new float[tracePointsMaxPool];
                    //LevelArrLength = tracePointsMaxPool;
                    //for (int i = 0; i < LevelArr.Length; i++)
                    //{
                    //    LevelArr[i] = -1000;
                    //    levelArrTemp[i] = -1000;
                    //}

                    if (gSensorCapabilities.supportsFrequencyData != 0)
                    {
                        //MessageBox.Show("This RF sensor firmware does not support the frequency data interface",
                        //    "Not supported");
                        host.RegisterHandler<COM.MesureTraceCommand, COMR.MesureTraceResult>(MesureTraceCommandHandler, rpd, mtdp);
                    }
                    //host.RegisterHandler<COM.MesureTraceCommand, COMR.MesureTraceResult>(MesureTraceCommandHandler, rpd, mtdp);
                    //host.RegisterHandler<COM.MesureIQStreamCommand, COMR.MesureIQStreamResult>(MesureIQStreamCommandHandler, miqdp);
                    //host.RegisterHandler<COM.MesureTraceCommand, COMR.MesureTraceResult>(EstimateRefLevelCommandHandler, mtdp);
                    host.RegisterHandler<COM.GpsCommand, COMR.GpsResult>(GPSCommandHandler);
                }
                else
                {
                    throw new Exception("Invalid initialize/connect adapter in connect");
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

        public void Disconnect()
        {
            try
            {
                /// освобождаем ресурсы и отключаем устройство
                if (gMeasHandle != IntPtr.Zero)
                {
                    AgSalLib.salClose(gMeasHandle);
                    gMeasHandle = IntPtr.Zero;
                }

                if (gSensorHandle != IntPtr.Zero)
                {
                    AgSalLib.salDisconnectSensor(gSensorHandle);
                    gSensorHandle = IntPtr.Zero;
                }

                if (gSmsHandle != IntPtr.Zero)
                {
                    AgSalLib.salClose(gSmsHandle);
                    gSmsHandle = IntPtr.Zero;
                }
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
                // если нужно проводим блокировку комманд который мы не сможем обслужить пока что то меряем в течении выполнени яэтой комманды
                /// и подсказывая этим инфраструктуре что этот устрйостов некоторое время не сможет обрабатываить такие комманды
                //context.Lock(CommandType.MesureTrace);

                // если нужно заблокировать выполняему комманду то достатчоно вызвать метод без параметров и блокируется комманда которая выполняется
                context.Lock();

                if (command.Parameter.TracePoint == 0)
                {
                    throw new Exception("TracePoint must be set greater than zero.");
                }
                else
                {
                    if (command.Parameter.RBW_Hz == 0)
                    {
                        throw new Exception("RBW must be set greater than zero.");
                    }
                    else
                    {
                        #region
                        ValidateAndSetTraceType(command.Parameter.TraceType);
                        ValidateAndSetTraceCount(command.Parameter.TraceCount);
                        double needRBW = 0;
                        int needSweepPoints = 0, fftsizeindex = 0, spanindex = 0, spansteps = 0;

                        ValidateAndSetFreqStartStop(command.Parameter.FreqStart_Hz, command.Parameter.FreqStop_Hz);
                        ValidateAndSetAttPreAmp(command.Parameter.Att_dB, command.Parameter.PreAmp_dB);

                        //кол-во точек посчитать по RBW
                        if (command.Parameter.TracePoint < 0 && command.Parameter.RBW_Hz > 0)
                        {
                            needRBW = command.Parameter.RBW_Hz;
                            (spanindex, spansteps) = ValidateAndFindSpanAndSteps((double)FreqSpan);
                            if (!FindNeedRBW(needRBW, ref fftsizeindex, ref spanindex, ref spansteps))
                            {
                                //алахадбар
                                throw new Exception("RBW unavailable.");
                            }
                        }
                        //сколько точек понятно почему и посчитать RBW ИЛИ забить на rbw считать по точкам
                        else if ((command.Parameter.TracePoint > 0 && command.Parameter.RBW_Hz < 0) ||
                            (command.Parameter.TracePoint > 0 && command.Parameter.RBW_Hz > 0))
                        {
                            needSweepPoints = command.Parameter.TracePoint;
                            (spanindex, spansteps) = ValidateAndFindSpanAndSteps((double)FreqSpan);
                            if (!FindNeedTracePoints(needSweepPoints, ref fftsizeindex, ref spanindex, ref spansteps))
                            {
                                //алахадбар
                                throw new Exception("TracePoints unavailable.");
                            }

                        }
                        else if (command.Parameter.TracePoint < 0 && command.Parameter.RBW_Hz < 0)
                        {
                            //алахадбар
                            throw new Exception("Auto TracePoint and Auto RBW not available together.");
                        }
                        //needStep = FreqSpan / needSweepPoints;

                        ////(double sampleRate, int spanIndex, uint steps, int numFftPointsIndex) = CalcSweepSetting((double)FreqSpan, needRBW, (uint)needSweepPoints);
                        ////double span = gSpans[spanIndex];
                        //(int spanIndex, int steps) = ValidateAndFindSpanAndSteps((double)FreqSpan);
                        //double span = gSpans[spanIndex];
                        ////if (span > gSensorCapabilities.maxSpan) span = gSensorCapabilities.maxSpan;

                        //double desiredSampleRate = span * gSensorCapabilities.sampleRateToSpanRatio;

                        //// *** WARNING: A workaround for an Electrum BUG... 
                        //// capabilities.maxSampleRate is wrongly set to 200kSa/s (i.e. DDC max sample rate),
                        //// while tuner's max sample rate is still 28MSa/s.
                        ////double sampleRate = gSensorCapabilities.maxSampleRate;
                        //if (gSensorCapabilities.maxSampleRate == 200.0e3) gSensorCapabilities.maxSampleRate = 28.0e6;
                        //double sampleRate = gSensorCapabilities.maxSampleRate;

                        //while (sampleRate / desiredSampleRate > 2)
                        //{
                        //    sampleRate /= 2;
                        //}

                        //uint numFftPoints = fftSize[fftSize.Length - 1];


                        //double rbw = sampleRate / numFftPoints * windowMult;

                        //uint numPoints = (uint)((double)numFftPoints * (span / sampleRate));


                        //(double needSampleRate, int needSpanIndex, uint needsteps, int FftPointsIndex) = CalcSweepSetting((double)FreqSpan, needRBW, (uint)needSweepPoints);
                        //Все настроили, теперь померяем и соберем результат

                        string poolKeyName = "";
                        bool poolKeyFind = false;
                        bool overload = false;
                        traceReset = true;
                        if (traceTypeResult == MEN.TraceType.Average)//назначим сколько усреднять
                        {
                            traceAveraged.AveragingCount = (int)traceCountToMeas;
                        }
                        int st = -1;
                        int points = 0;
                        double freqstep = 0;
                        double freqstart = 0;
                        double freqstop = (double)FreqStop;
                        FftPoints = fftSize[fftsizeindex];
                        fSpan = gSpans[spanindex];
                        resultGPSPublished = false;
                        bool freqResSet = false;
                        int resFreqStopIndex = 0;
                        for (ulong t = 0; t < traceCountToMeas; t++)
                        {
                            bool overloadonstep = false, overloadonsubstep = false;

                            for (int s = 0; s < spansteps; s++)//идем по частотам
                            {
                                if (st != s)
                                {
                                    gSetupChange = true;
                                    fCentr = (double)FreqStart + 0.5 * fSpan + s * fSpan;
                                    gSetupChange = true;
                                    st = s;
                                }
                                if (GetTraceData(ref LevelArrTemp2, ref points, ref overloadonsubstep, ref freqstart, ref freqstep))
                                {
                                    if (!freqResSet && s == 0)
                                    {
                                        resFreqStart = freqstart;
                                        resFreqStep = freqstep;
                                        freqResSet = true;                                       
                                        double ddd = 0;
                                        for (int rf = 0; rf < spansteps * points; rf++)
                                        {
                                            ddd = resFreqStart + rf * resFreqStep;
                                            if (ddd > freqstop)
                                            {
                                                resFreqStopIndex = rf;
                                                break;
                                            }
                                            //if (steps * points - 1 == rf)
                                            //{

                                            //}
                                        }
                                    }
                                    if (resFreqStopIndex == 0)
                                    {
                                        LevelArrLength = spansteps * points;
                                    }
                                    else
                                    {
                                        LevelArrLength = resFreqStopIndex;
                                    }
                                    if (!overloadonstep && overloadonsubstep)
                                    {
                                        overloadonstep = true;
                                    }
                                    if (freqstart <= freqstop)
                                    {
                                        double fcdel = Math.Abs(freqstart - fCentr + 0.5 * fSpan);
                                        if (fcdel < freqstep)
                                        {
                                            Array.Copy(LevelArrTemp2, 0, LevelArrTemp, 0 + points * s, points);
                                            //обновим координаты
                                            if (!resultGPSPublished && stateGPS == COMP.GpsMode.Start)
                                            {
                                                if (executionContextGps != null &&
                                                    gDataHeader.location.latitude != 0 &&
                                                    gDataHeader.location.longitude != 0 &&
                                                    gDataHeader.location.elevation != 0)
                                                {
                                                    resultGPS.Lat = gDataHeader.location.latitude;
                                                    resultGPS.Lon = gDataHeader.location.longitude;
                                                    resultGPS.Asl = gDataHeader.location.elevation;
                                                    executionContextGps.PushResult(resultGPS);
                                                    resultGPSPublished = true;
                                                }
                                            }
                                        }
                                        else
                                        {
                                            fCentr = (double)FreqStart + 0.5 * fSpan + s * fSpan;
                                            gSetupChange = true;
                                            s--;
                                        }
                                    }
                                    else
                                    {
                                        break;
                                    }
                                }
                                else
                                {
                                    s--;
                                }

                                // иногда нужно проверять токен окончания работы комманды
                                if (context.Token.IsCancellationRequested)
                                {
                                    // все нужно остановиться

                                    // если есть порция данных возвращаем ее в обработчки только говрим что поток результатов не законченный и больше уже не будет поступать
                                    if (!poolKeyFind)
                                    {
                                        FindTracePoolName(LevelArrLength, ref poolKeyFind, ref poolKeyName);
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
                            // иногда нужно проверять токен окончания работы комманды
                            if (context.Token.IsCancellationRequested)
                            {
                                // все уже остоновили и отправили результат, выйдем и тиз этого цикла                                                                                               
                                // подтверждаем факт обработки отмены
                                context.Cancel();
                                // освобождаем поток 
                                return;
                            }
                            if (!overload && overloadonstep)
                            {
                                overload = true;
                            }

                            SetTraceData(LevelArrTemp, LevelArrLength);
                            if (traceTypeResult == MEN.TraceType.ClearWrite)
                            {
                                //публикуем результат
                                traceCount++;
                                if (!poolKeyFind)
                                {
                                    FindTracePoolName(LevelArrLength, ref poolKeyFind, ref poolKeyName);
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

                                for (int j = 0; j < LevelArrLength; j++)
                                {
                                    result.Level[j] = LevelArr[j];
                                }
                                result.LevelMaxIndex = LevelArrLength;
                                result.FrequencyStart_Hz = resFreqStart;
                                result.FrequencyStep_Hz = resFreqStep;

                                result.TimeStamp = timeService.GetGnssUtcTime().Ticks - uTCOffset;// new DateTime(1970, 1, 1, 0, 0, 0, System.DateTimeKind.Utc).Ticks;//неюзабельно
                                                                                                  //result.TimeStamp = DateTime.UtcNow.Ticks - new DateTime(1970, 1, 1, 0, 0, 0, System.DateTimeKind.Utc).Ticks;
                                result.Att_dB = (int)AttLevel;
                                result.PreAmp_dB = preAmp;
                                result.RefLevel_dBm = 0;
                                result.RBW_Hz = RBW;
                                result.VBW_Hz = 0;
                                result.DeviceStatus = COMR.Enums.DeviceStatus.Normal;
                                if (overloadonstep)
                                {
                                    result.DeviceStatus = COMR.Enums.DeviceStatus.RFOverload;
                                }
                                context.PushResult(result);
                            }
                        }
                        if (gMeasHandle != IntPtr.Zero)
                        {
                            AgSalLib.salClose(gMeasHandle);
                            gMeasHandle = IntPtr.Zero;
                            gMeasState = MeasState.Idle;
                        }
                        if (!context.Token.IsCancellationRequested)
                        {
                            //Если TraceType Average/MinHold/MaxHold то делаем измерений сколько сказали и пушаем только готовый результат
                            if (traceTypeResult != MEN.TraceType.ClearWrite)
                            {
                                if (!poolKeyFind)
                                {
                                    FindTracePoolName(LevelArrLength, ref poolKeyFind, ref poolKeyName);
                                }
                                var result = context.TakeResult<COMR.MesureTraceResult>(poolKeyName, traceCount, CommandResultStatus.Final);


                                for (int j = 0; j < LevelArrLength; j++)
                                {
                                    result.Level[j] = LevelArr[j];
                                }
                                result.LevelMaxIndex = LevelArrLength;
                                result.FrequencyStart_Hz = resFreqStart;
                                result.FrequencyStep_Hz = resFreqStep;

                                result.TimeStamp = timeService.GetGnssUtcTime().Ticks - uTCOffset;// new DateTime(1970, 1, 1, 0, 0, 0, System.DateTimeKind.Utc).Ticks;//неюзабельно
                                                                                                  //result.TimeStamp = DateTime.UtcNow.Ticks - new DateTime(1970, 1, 1, 0, 0, 0, System.DateTimeKind.Utc).Ticks;
                                result.Att_dB = (int)AttLevel;
                                result.PreAmp_dB = preAmp;
                                result.RefLevel_dBm = 0;
                                result.RBW_Hz = RBW;
                                result.VBW_Hz = 0;

                                if (overload)
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
                        #endregion

                        // снимаем блокировку с текущей команды
                        context.Unlock();

                        // что то делаем еще 
                        // подтверждаем окончание выполнения комманды 
                        // важно: всн ранее устапнволеные в контексте обработки текущей команыд блокировки снимаются автоматически
                        context.Finish();
                        // дальше кода быть не должно, освобождаем поток
                    }
                }
            }
            catch (Exception e)
            {
                // желательно записать влог
                logger.Exception(Contexts.ThisComponent, e);
                // этот вызов обязательный в случаи обрыва
                context.Unlock();
                context.Abort(e);
                // дальше кода быть не должно, освобождаем поток
            }
        }
        public void MesureIQStreamCommandHandler(COM.MesureIQStreamCommand command, IExecutionContext context)
        { }
        public void GPSCommandHandler(COM.GpsCommand command, IExecutionContext context)
        {
            try
            {
                if (command.Parameter.GpsMode == COM.Parameters.GpsMode.Start)
                {
                    executionContextGps = context;
                    resultGPS = new COMR.GpsResult(resultGPSpart++, CommandResultStatus.Next);
                    stateGPS = COM.Parameters.GpsMode.Start;
                }
                else if (command.Parameter.GpsMode == COM.Parameters.GpsMode.Stop)
                {
                    stateGPS = COM.Parameters.GpsMode.Stop;
                }
            }
            catch (Exception e)
            {
                // желательно записать влог
                logger.Exception(Contexts.ThisComponent, e);
                // этот вызов обязательный в случаи обрыва
                context.Abort(e);
            }
        }
        #region Param
        private readonly long uTCOffset = 621355968000000000;
        private string gSensorName = "";
        private string gSmsHostname = "";

        // sensorHandle is used to talk to a specific sensor
        IntPtr gSmsHandle = IntPtr.Zero;     // handle to the Sensor Management Server
        IntPtr gSensorHandle = IntPtr.Zero;  // handle to the sensor
        IntPtr gMeasHandle = IntPtr.Zero;  // handle to the FFT measurement

        AgSalLib.SalError err;
        AgSalLib.SensorCapabilities gSensorCapabilities; // sensor frequency limits, etc
        AgSalLib.SensorInfo SensorInfo;

        bool gSetupChange = false;
        AgSalLib.AntennaType antennaType = AgSalLib.AntennaType.Antenna_1;
        AgSalLib.AverageType averageType = AgSalLib.AverageType.Average_off;

        enum MeasState
        {
            Idle,
            Starting, // sent start command, waiting for first segment
            Running,  // getting data
            Stopping, // sent stop command, still waiting for data
            Stopped  // got last segment
        }
        uint averageNumber = 0;
        private double[] gSpans = { 15.625e3, 31.25e3, 62.5e3, 125e3, 250e3, 500e3, 1e6, 2e6, 5e6, 10e6, 20e6 };
        private uint[] fftSize = null;
        private AgSalLib.WindowType window;
        private double windowMult;

        // gDataHeader is the data header for the last segment with valid data
        private AgSalLib.SegmentData gDataHeader;// = new AgSalLib.SegmentData();

        MeasState gMeasState = MeasState.Idle;

        // freqData contains the FFT of the time data
        public float[] LevelArrTemp = null;
        public float[] LevelArrTemp2 = null;
        public float[] LevelArr = null;
        public int LevelArrLength = 0;

        #region Freqs
        public decimal FreqMin = 20000000;
        public decimal FreqMax = 6000000000;

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

        #region Level
        private double attMax = 0;
        private double attMin = 0;
        private double attStep = 0;

        private double AttLevel
        {
            get { return attLevel; }
            set
            {
                if (value > attMax) attLevel = attMax;
                else if (value < 0) attLevel = 0;
                else attLevel = value;
            }
        }
        private double attLevel = 0;
        private int preAmp = 0;

        private double attLevelSet = 0;

        public MEN.LevelUnit LevelUnit = MEN.LevelUnit.dBm;
        #endregion Level

        public double RBW = 0;
        private int tracePointsMaxPool = 0;
        private const int tracePointsMax = 1_200_000;

        private double resFreqStart = 10000;
        private double resFreqStep = 10000;

        private MEN.TraceType traceTypeResult = MEN.TraceType.ClearWrite;
        private bool traceReset;
        private AveragedTrace traceAveraged = new AveragedTrace();

        private ulong traceCountToMeas = 1;
        private ulong traceCount = 1;
        #endregion Param

        #region ValidateAndSet
        private IResultPoolDescriptor<COMR.MesureTraceResult>[] ValidateAdapterTracePoolMainConfig(CFG.AdapterResultPool[] adapterTraceResultPools, string fileName)
        {
            IResultPoolDescriptor<COMR.MesureTraceResult>[] rpd = null;

            int poolMaxSize = 256_000_000;
            int poolSize = 0;
            for (int i = 0; i < adapterTraceResultPools.Length; i++)
            {
                if (adapterTraceResultPools[i].MinSize > 0)
                {
                    if (adapterTraceResultPools[i].MaxSize > 0)
                    {
                        if (adapterTraceResultPools[i].MinSize < adapterTraceResultPools[i].MaxSize)
                        {
                            if (adapterTraceResultPools[i].Size <= tracePointsMax)
                            {
                                poolSize += adapterTraceResultPools[i].MaxSize * adapterTraceResultPools[i].Size * sizeof(float);
                                if (adapterTraceResultPools[i].Size >= tracePointsMaxPool)//ищем максимальный размер одного пула, потом этим пользуемся чтобы не выстрелить в ногу
                                {
                                    tracePointsMaxPool = adapterTraceResultPools[i].Size;
                                }
                            }
                            else
                            {
                                throw new Exception("In the AdapterTraceResultPools element number " + i + ", " +
                                    "the Size value is exceeded the maximum value (" + tracePointsMax + ") for the SignalHound adapter. File Name \"" + fileName + "\"");
                            }
                        }
                        else
                        {
                            throw new Exception("In the AdapterTraceResultPools element number " + i + ", " +
                                "the MinSize value is greater than the MaxSize. File Name \"" + fileName + "\"");
                        }
                    }
                    else
                    {
                        throw new Exception("In the AdapterTraceResultPools item number " + i + ", " +
                            "the MaxSize value is less than or equal to zero. File Name \"" + fileName + "\"");
                    }
                }
                else
                {
                    throw new Exception("In the AdapterTraceResultPools item number " + i + ", " +
                        "the MinSize value is less than or equal to zero. File Name \"" + fileName + "\"");
                }
            }
            if (poolSize <= poolMaxSize)
            {
                rpd = new IResultPoolDescriptor<COMR.MesureTraceResult>[adapterTraceResultPools.Length];
                for (int i = 0; i < adapterTraceResultPools.Length; i++)
                {
                    int count = adapterTraceResultPools[i].Size;
                    rpd[i] = new ResultPoolDescriptor<COMR.MesureTraceResult>()
                    {
                        Key = count.ToString(),
                        MinSize = adapterTraceResultPools[i].MinSize,
                        MaxSize = adapterTraceResultPools[i].MaxSize,
                        Factory = () =>
                        {
                            var result = new COMR.MesureTraceResult()
                            {
                                Level = new float[count]
                            };
                            return result;
                        }
                    };

                }
            }
            else
            {
                throw new Exception("The AdapterTraceResultPools configuration exceeded the maximum pool size limit of " + poolMaxSize / 1000000 + " MB. File Name \"" + fileName + "\"");
            }
            return rpd;
        }
        private void ValidateAndSetAdapterConfig(AdapterConfig config)
        {
            if (config.SmsHostName == null)
            {
                gSmsHostname = "";
            }
            else
            {
                gSmsHostname = config.SmsHostName;
            }

            if (config.SensorName == null)
            {
                gSensorName = "";//патом найдем первый попавшийся
            }
            else
            {
                gSensorName = config.SensorName;
            }

            if (config.WindowType == 1)
            {
                window = AgSalLib.WindowType.Window_hann;
            }
            else if (config.WindowType == 2)
            {
                window = AgSalLib.WindowType.Window_gaussTop;
            }
            else if (config.WindowType == 4)
            {
                window = AgSalLib.WindowType.Window_flatTop;
            }
            else if (config.WindowType == 8)
            {
                window = AgSalLib.WindowType.Window_uniform;
            }
            else
            {
                throw new Exception("AdapterConfig.WindowType must be set to value available");
            }
            // Compute RBW based on FFT size, sample rate, and window
            switch (window)
            {
                case AgSalLib.WindowType.Window_hann: windowMult = 1.5; break;
                case AgSalLib.WindowType.Window_gaussTop: windowMult = 2.215349684; break;
                case AgSalLib.WindowType.Window_flatTop: windowMult = 3.822108760; break;
                case AgSalLib.WindowType.Window_uniform: windowMult = 1.0; break;
            }
        }

        private void ValidateAndSetFreqStartStop(decimal freqStart, decimal freqStop)
        {
            if (FreqStart != freqStart || FreqStop != freqStop)
            {
                if (freqStart < FreqMin)
                {
                    FreqStart = FreqMin;
                    logger.Warning(Contexts.ThisComponent, "FreqStart set to limit value");
                }
                else if (freqStart > FreqMax)
                {
                    FreqStart = FreqMax;
                    logger.Warning(Contexts.ThisComponent, "FreqStart set to limit value");
                }
                else FreqStart = freqStart;

                if (freqStop > FreqMax)
                {
                    FreqStop = FreqMax;
                    logger.Warning(Contexts.ThisComponent, "FreqStop set to limit value");
                }
                else if (freqStop < FreqMin)
                {
                    FreqStop = FreqMin + 1000000;
                    logger.Warning(Contexts.ThisComponent, "FreqStop set to limit value");
                }
                else FreqStop = freqStop;
            }
        }
        private void ValidateAndSetAttPreAmp(int att, int preamp)
        {
            if (preamp < 0)
            {
                preAmp = 0;//потому что нет альтернативы,
            }
            else if (preamp == 0)
            {
                preAmp = 0;
            }
            else if (preamp > 1)
            {
                if (FreqStart >= 750000000 && FreqStop <= 1800000000)
                {
                    preAmp = 1;
                }
                else
                {
                    preAmp = 0;
                }
            }
            if (att > -1)
            {
                double res = 0;
                double delta = double.MaxValue;
                for (double i = 0; i <= attMax; i += attStep)
                {
                    if (Math.Abs(att - i) < delta)
                    {
                        delta = Math.Abs(att - i);
                        res = i;
                    }
                }
                AttLevel = res;
            }
            else
            {
                AttLevel = 0;
            }
        }
        private void ValidateAndSetTraceType(COMP.TraceType traceType)
        {
            MEN.TraceType res = MEN.TraceType.Blank;
            for (int i = 0; i < Enum.GetNames(typeof(MEN.TraceType)).Length; i++)
            {
                if (traceType == COMP.TraceType.ClearWhrite)
                {
                    res = MEN.TraceType.ClearWrite;
                }
                else if (traceType == COMP.TraceType.Average)
                {
                    res = MEN.TraceType.Average;
                }
                else if (traceType == COMP.TraceType.MaxHold)
                {
                    res = MEN.TraceType.MaxHold;
                }
                else if (traceType == COMP.TraceType.MinHold)
                {
                    res = MEN.TraceType.MinHold;
                }
                else if (traceType == COMP.TraceType.Auto)
                {
                    //По результатам согласования принято такое решение
                    res = MEN.TraceType.ClearWrite;
                }
            }
            if (res == MEN.TraceType.Blank)
            {
                throw new Exception("The TraceType must be set to the available instrument range.");
            }
            else
            {
                traceTypeResult = res;
            }
        }
        private void ValidateAndSetTraceCount(int measCount)
        {
            if (measCount > 0)
            {
                traceCountToMeas = (ulong)measCount;//т.к. отсюда берется индекс результата
                this.traceCount = 0;
            }
            else
            {
                throw new Exception("TraceCount must be set greater than zero.");
            }
        }
        private (int SpanIndex, int Steps) ValidateAndFindSpanAndSteps(double span)
        {
            int SpanIndex = 0;
            int Steps = 0;
            if (span <= gSpans[gSpans.Length - 1])
            {
                int index = 0;
                double delta = double.MaxValue;
                for (int i = 0; i < gSpans.Length; i++)
                {
                    if (Math.Abs(span - gSpans[i]) < delta)
                    {
                        delta = Math.Abs(span - gSpans[i]);
                        index = i;
                    }
                }
                if (span > gSpans[index])
                {
                    SpanIndex = index + 1;
                }
                else
                {
                    SpanIndex = index;
                }
                Steps = 1;
            }
            else if (span > gSpans[gSpans.Length - 1])
            {
                Steps = (int)Math.Ceiling(span / gSpans[gSpans.Length - 1]);
                SpanIndex = gSpans.Length - 1;
            }
            else
            {
                throw new Exception("FreqSpan must be set to the available instrument range.");
            }

            return (SpanIndex, Steps);
        }
        #endregion ValidateAndSet

        private bool SetConnect()
        {
            err = AgSalLib.salOpenSms(out gSmsHandle, gSmsHostname, 0, null);
            if (SensorError(err, "salOpenSms: hostname = " + gSmsHostname))
            {
                return false;
            }

            if (gSensorName == "")//ищем первый попавшийся
            {
                IntPtr sensorList = IntPtr.Zero;
                AgSalLib.SensorStatus sensorStatus = new AgSalLib.SensorStatus();
                UInt32 numSensors = 0;
                // Get the list from the SMS 
                err = AgSalLib.salOpenSensorList2(out sensorList, gSmsHandle, out numSensors);

                if (numSensors > 0)
                {
                    for (int i = 0; i < numSensors; i++)
                    {
                        AgSalLib.salGetNextSensor(sensorList, out sensorStatus);
                        if (i == 0)
                        {
                            gSensorName = sensorStatus.name;
                        }
                    }
                }
                else
                {
                    throw new Exception("There are no sensors available on host " + gSmsHostname);
                }
            }


            err = AgSalLib.salConnectSensor2(out gSensorHandle, gSmsHandle, gSensorName, "Fft Demo", 0);
            if (SensorError(err, "salConnectSensor"))
            {
                return false;
            }

            IntPtr discoveredList = IntPtr.Zero;
            AgSalLib.SensorInfo sensorInfo = new AgSalLib.SensorInfo();
            UInt32 numSensors2 = 0;
            err = AgSalLib.salDiscoverSensors(out discoveredList, gSmsHandle, out numSensors2);

            if (numSensors2 > 0)
            {
                for (int i = 0; i < numSensors2; i++)
                {
                    AgSalLib.salGetNextDiscoveredSensor(discoveredList, out sensorInfo);
                    if (gSensorName == sensorInfo.hostName)
                    {
                        SensorInfo = sensorInfo;
                    }
                }
            }
            else
            {
                throw new Exception("There are no sensors available on host " + gSmsHostname);
            }

            err = AgSalLib.salGetSensorCapabilities(gSensorHandle, out gSensorCapabilities);
            if (SensorError(err, "salGetSensorCapabilities"))
            {
                return false;
            }

            if (gSensorCapabilities.supportsFrequencyData == 0)
            {
                //MessageBox.Show("This RF sensor firmware does not support the frequency data interface",
                //    "Not supported");
                //return false;
            }


            err = AgSalLib.salLockResource(gSensorHandle, AgSalLib.Resource.Tuner);
            if (SensorError(err, "salLockResource(Tuner)"))
            {
                return false;
            }

            err = AgSalLib.salLockResource(gSensorHandle, AgSalLib.Resource.Fft);
            if (SensorError(err, "salLockResource(FFT)"))
            {
                return false;
            }
            //if (!gEnableMonitorMode)
            //{

            //    // If we are not in monitor mode, we want exclusive access to the
            //    // sensor tuner and FFT engine. This keeps us from stopping another user's 
            //    // measurement and keeps other users from stopping ours
            //    err = AgSalLib.salLockResource(gSensorHandle, AgSalLib.Resource.Tuner);
            //    if (sensorError(err, "salLockResource(Tuner)"))
            //    {
            //        return false;
            //    }

            //    err = AgSalLib.salLockResource(gSensorHandle, AgSalLib.Resource.Fft);
            //    if (sensorError(err, "salLockResource(FFT)"))
            //    {
            //        return false;
            //    }
            //}

            //initFftSizeGui();

            //ALLOY-2017: The sensor's freq detection range may be different depending on whether or not a user
            //has connected a freq extender to the sensor. So instead of hardcoding the range of freqs in the GUI,
            //we set the min/max with the range from the sensor.
            //guiFrequencyMHz.Minimum = (decimal)(gSensorCapabilities.minFrequency / 1e6);
            //guiFrequencyMHz.Maximum = (decimal)(gSensorCapabilities.maxFrequency / 1e6);

            List<uint> fft = new List<uint> { };
            for (int i = gSensorCapabilities.fftMinBlocksize; i <= gSensorCapabilities.fftMaxBlocksize; i *= 2)
            {
                fft.Add((uint)i);
            }
            fftSize = fft.ToArray();
            return true;
        }
        /// check if there was an error; if there was, print a message and return "true"
        private bool SensorError(AgSalLib.SalError err, string functionName) // return true if there was an error
        {
            string message = "";
            if (err != AgSalLib.SalError.SAL_ERR_NONE)
            {

                if (message.Length == 0)
                {
                    message = functionName + " returned error " + err + " (" + AgSalLib.salGetErrorString(err, AgSalLib.Localization.English) + ")\n";
                }
                //MessageBox.Show(message, "RF Sensor Error");
                return true;
            }
            else return false;
        }

        private (double sampleRate, int spanIndex, int steps, int numFftPointsIndex) CalcSweepSetting(double needspan, double needrbw, uint needpoints /*, uint numFftPoints*/)
        {
            (int spanIndex, int steps) = ValidateAndFindSpanAndSteps((double)needspan);
            //if (span > gSensorCapabilities.maxSpan) span = gSensorCapabilities.maxSpan;

            double desiredSampleRate = gSpans[spanIndex] * gSensorCapabilities.sampleRateToSpanRatio;

            // *** WARNING: A workaround for an Electrum BUG... 
            // capabilities.maxSampleRate is wrongly set to 200kSa/s (i.e. DDC max sample rate),
            // while tuner's max sample rate is still 28MSa/s.
            //double sampleRate = gSensorCapabilities.maxSampleRate;
            if (gSensorCapabilities.maxSampleRate == 200.0e3) gSensorCapabilities.maxSampleRate = 28.0e6;
            double sampleRate = gSensorCapabilities.maxSampleRate;

            while (sampleRate / desiredSampleRate > 2)
            {
                sampleRate /= 2;
            }
            int numFftPointsIndex = fftSize.Length - 1;
            uint numFftPoints = fftSize[numFftPointsIndex];


            double rbw = sampleRate / numFftPoints * windowMult;

            uint numPointsOnStep = ((uint)((double)numFftPoints * (gSpans[spanIndex] / sampleRate)));
            bool exitLoop = false;
            double r = 0;

            while (!exitLoop)
            {
                if (needpoints < numPointsOnStep * steps) //мало точек, надо больше
                {
                    r = needrbw / rbw;
                    if (r >= 2)//надо увеличить rbw на один шаг
                    {
                        if (numFftPointsIndex > 0)// можем уменьшить fftsize
                        {
                            numFftPointsIndex--;
                            numFftPoints = fftSize[numFftPointsIndex];
                            steps *= 2;
                            //spanIndex--;
                        }
                        else//неможем уменьшить fftsize, уменьшим спан и увеличим количестко шагов
                        {
                            //значит уменьшим спан
                            spanIndex++;
                            steps /= 2;//соответственно нужно увеличить количество шагов
                        }
                    }
                    else if (r <= 0.5)//надо уменьшить rbw на один шаг
                    {
                        if (numFftPointsIndex < fftSize.Length - 1)//не можем увеличить fftsize
                        {
                            //значит уменьшим спан
                            spanIndex++;
                            steps *= 2;//соответственно нужно увеличить количество шагов
                        }
                        else
                        {
                            numFftPointsIndex++;
                            numFftPoints = fftSize[numFftPointsIndex];
                            steps /= 2;
                        }
                    }
                }
                else if (needpoints * 2 > numPointsOnStep * steps)//Много точек, надо меньше
                {
                    r = needrbw / rbw;
                    if (r >= 2)//надо увеличить rbw на один шаг
                    {
                        if (numFftPointsIndex == fftSize.Length - 1)//не можем увеличить fftsize
                        {
                            //значит уменьшим спан
                            spanIndex--;
                            steps *= 2;//соответственно нужно увеличить количество шагов
                        }
                        else
                        {
                            numFftPointsIndex++;
                            numFftPoints = fftSize[numFftPointsIndex];
                        }
                    }
                    else if (r <= 0.5)//надо уменьшить rbw на один шаг
                    {
                        if (numFftPointsIndex < fftSize.Length - 1)//не можем увеличить fftsize
                        {
                            //значит уменьшим спан
                            spanIndex++;
                            steps /= 2;//соответственно нужно увеличить количество шагов
                        }
                        else
                        {
                            numFftPointsIndex--;
                            numFftPoints = fftSize[numFftPointsIndex];
                        }
                    }
                }
                else //точек норм, не устраивает RBW
                {
                    r = needrbw / rbw;

                }
                (double rbw2, uint numPoints2) = Calc(spanIndex, numFftPointsIndex);
                exitLoop = ValSweepSett(needrbw, rbw2, (int)needpoints, (int)(numPoints2 * steps));
                if (!exitLoop)
                {
                    rbw = rbw2;
                    numPointsOnStep = numPoints2;
                }
            }
            #region
            //while (!exitLoop)
            //{
            //    if (needpoints < numPoints) //мало точек, надо больше
            //    {
            //        r = needrbw / rbw;
            //        if (r >= 2)//надо увеличить rbw на один шаг
            //        {
            //            if (numFftPointsIndex == fftSize.Length - 1)//не можем увеличить fftsize
            //            {
            //                //значит уменьшим спан
            //                spanIndex--;
            //                steps *= 2;//соответственно нужно увеличить количество шагов
            //            }
            //            else
            //            {
            //                numFftPointsIndex++;
            //                numFftPoints = fftSize[numFftPointsIndex];
            //            }
            //        }
            //        else if (r <= 0.5)//надо уменьшить rbw на один шаг
            //        {
            //            if (numFftPointsIndex < fftSize.Length - 1)//не можем увеличить fftsize
            //            {
            //                //значит уменьшим спан
            //                spanIndex++;
            //                steps /= 2;//соответственно нужно увеличить количество шагов
            //            }
            //            else
            //            {
            //                numFftPointsIndex--;
            //                numFftPoints = fftSize[numFftPointsIndex];
            //            }
            //        }
            //    }
            //    else if (needpoints * 2 > numPoints)//Много точек, надо меньше
            //    {

            //    }
            //    else //точек норм, не устраивает RBW
            //    {
            //        r = needrbw / rbw;
            //        if (r >= 2)//надо увеличить rbw, на один шаг
            //        {
            //            if (numFftPointsIndex == fftSize.Length - 1)//не можем увеличить fftsize
            //            {
            //                //значит уменьшим спан
            //                spanIndex--;
            //                steps *= 2;//соответственно нужно увеличить количество шагов
            //            }
            //            else
            //            {
            //                numFftPointsIndex++;
            //                numFftPoints = fftSize[numFftPointsIndex];
            //            }
            //        }
            //        else if (r <= 0.5)//надо уменьшить rbw на один шаг
            //        {
            //            if (numFftPointsIndex < fftSize.Length - 1)//не можем увеличить fftsize
            //            {
            //                //значит уменьшим спан
            //                spanIndex++;
            //                steps /= 2;//соответственно нужно увеличить количество шагов
            //            }
            //            else
            //            {
            //                numFftPointsIndex--;
            //                numFftPoints = fftSize[numFftPointsIndex];
            //            }
            //        }
            //        else //RBW норм, по идее все хорошо
            //        { }
            //    }
            //    (double rbw2, uint numPoints2) = Calc(spanIndex, numFftPointsIndex);
            //    exitLoop = ValSweepSett(needrbw, rbw2, (int)needpoints, (int)(numPoints2 * steps));
            //    if (!exitLoop)
            //    {
            //        rbw = rbw2;
            //        numPoints = numPoints2;
            //    }
            //}
            #endregion
            return (sampleRate, spanIndex, steps, numFftPointsIndex);
        }
        private (double rbw, uint numPoints) Calc(int spanindex, int numFftPointsIndex)
        {
            double desiredSampleRate = gSpans[spanindex] * gSensorCapabilities.sampleRateToSpanRatio;
            if (gSensorCapabilities.maxSampleRate == 200.0e3) gSensorCapabilities.maxSampleRate = 28.0e6;
            double sampleRate = gSensorCapabilities.maxSampleRate;

            while (sampleRate / desiredSampleRate > 2)
            {
                sampleRate /= 2;
            }
            uint numFftPoints = fftSize[numFftPointsIndex];
            double rbw = sampleRate / numFftPoints * windowMult;

            uint numPoints = (uint)((double)numFftPoints * (gSpans[spanindex] / sampleRate));
            return (rbw, numPoints);
        }



        private bool ValSweepSett(double needrbw, double rbw, int needpoints, int points)
        {
            if (points / needpoints >= 1 && points / needpoints < 2)
            {
                if (rbw / needrbw > 0.5 && rbw / needrbw <= 1)
                {
                    return true;
                }
            }
            return false;
        }


        /// <summary>
        /// ищет необходимое RBW при необходимости изменяет спан и количевство шагов
        /// </summary>
        /// <param name="needRBW"></param>
        /// <param name="fftSizeIndex"></param>
        /// <param name="needSpanIndex"></param>
        /// <param name="steps"></param>
        /// <returns></returns>
        private bool FindNeedRBW(double needRBW, ref int fftSizeIndex, ref int needSpanIndex, ref int steps)
        {
            double rbw = 0;
            uint numPoints = 0;
            for (int i = 0; i < fftSize.Length; i++)
            {
                (rbw, numPoints) = Calc(needSpanIndex, i);
                if (rbw < needRBW)
                {
                    fftSizeIndex = i;
                    return true;
                }
                if (i == fftSize.Length - 1 && needSpanIndex > 0)
                {
                    needSpanIndex--;
                    steps *= 2;
                    i--;
                }

            }
            return false;
        }
        /// <summary>
        /// ищет необходимое RBW при необходимости изменяет спан и количевство шагов
        /// </summary>
        /// <param name="needRBW"></param>
        /// <param name="fftSizeIndex"></param>
        /// <param name="needSpanIndex"></param>
        /// <param name="steps"></param>
        /// <returns></returns>
        private bool FindNeedTracePoints(int tracePonts, ref int fftSizeIndex, ref int needSpanIndex, ref int steps)
        {
            double rbw = 0;
            uint numPoints = 0;
            double step = 0;
            double startfreq = (double)FreqStart;
            double stopfreq = (double)FreqStop;
            int resFreqStopIndex = 0;
            int pointsonthis = 0;
            for (int i = 0; i < fftSize.Length; i++)
            {
                (rbw, numPoints) = Calc(needSpanIndex, i);
                step = gSpans[needSpanIndex] / numPoints;

                for (int rf = 0; rf < steps * numPoints; rf++)
                {
                    if (startfreq + rf * step > stopfreq)
                    {
                        resFreqStopIndex = rf;
                        break;
                    }
                }
                if (resFreqStopIndex == 0)
                {
                    pointsonthis = steps * (int)numPoints;
                }
                else
                {
                    pointsonthis = resFreqStopIndex;
                }


                if (pointsonthis > tracePonts)
                {
                    fftSizeIndex = i;
                    return true;
                }
                if (i == fftSize.Length - 1 && needSpanIndex > 0)
                {
                    needSpanIndex--;
                    steps *= 2;
                    i--;
                }

            }
            return false;
        }

        #region Works
        private void SetTraceData(float[] trace, int points)
        {
            if (trace != null && trace.Length > 0)
            {
                if (traceTypeResult == MEN.TraceType.ClearWrite)
                {
                    LevelArrLength = points;
                    for (uint i = 0; i < points; i++)
                    {
                        LevelArr[i] = trace[i];
                    }
                }
                else if (traceTypeResult == MEN.TraceType.Average)//Average
                {
                    if (traceReset || LevelArrLength != points) { traceAveraged.Reset(); LevelArrLength = points; traceReset = false; }
                    traceAveraged.AddTraceToAverade(resFreqStart, resFreqStep, trace, points, LevelUnit, LevelUnit);
                    if (traceAveraged.NumberOfSweeps == traceAveraged.AveragingCount)
                    {
                        for (uint i = 0; i < traceAveraged.TracePoints; i++)
                        {
                            LevelArr[i] = traceAveraged.AveragedLevels[i];
                        }
                    }
                    //LevelArr = traceAveraged.AveragedLevels;
                }
                else if (traceTypeResult == MEN.TraceType.MaxHold)
                {
                    if (traceReset || LevelArrLength != points)
                    {
                        LevelArrLength = points;
                        for (uint i = 0; i < points; i++)
                        {
                            LevelArr[i] = trace[i];
                        }
                        traceReset = false;
                    }
                    else
                    {
                        for (uint i = 0; i < points; i++)
                        {
                            if (trace[i] > LevelArr[i]) LevelArr[i] = trace[i];
                        }
                    }
                }
                else if (traceTypeResult == MEN.TraceType.MinHold)
                {
                    if (traceReset || LevelArrLength != points)
                    {
                        LevelArrLength = points;
                        for (uint i = 0; i < points; i++)
                        {
                            LevelArr[i] = trace[i];
                        }
                        traceReset = false;
                    }
                    else
                    {
                        for (uint i = 0; i < points; i++)
                        {
                            if (trace[i] < LevelArr[i]) LevelArr[i] = trace[i];
                        }
                    }
                }
            }
        }


        private TimeSpan sleep = new TimeSpan(10000);
        private bool GetTraceData(ref float[] trace, ref int points, ref bool overload, ref double freqstart, ref double freqstep)
        {
            bool res = false;
            AgSalLib.SegmentData dataHeader = new AgSalLib.SegmentData();
            AgSalLib.SalError err;
            uint numSweepsThisTime = 0;

            bool exitLoop = false;
            DateTime t0 = DateTime.Now;
            int maxDataReadMillisecs = 10; // how long to read data before exiting

            while ((!exitLoop) && (gMeasHandle != IntPtr.Zero))
            {

                TimeSpan elaspsedTime = DateTime.Now.Subtract(t0);
                if (elaspsedTime.Milliseconds > maxDataReadMillisecs)
                {
                    break;
                }

                //////////////////////////////////////
                // See if there is a new data block //   
                //////////////////////////////////////
                err = AgSalLib.salGetSegmentData(gMeasHandle, out dataHeader, trace, (uint)(trace.Length * 4));

                switch (err)
                {
                    case AgSalLib.SalError.SAL_ERR_NONE:
                        ////////////////////////////////////////////////
                        // there is new data, so do something with it //
                        ////////////////////////////////////////////////

                        if (dataHeader.errorNum != AgSalLib.SalError.SAL_ERR_NONE)
                        {
                            //string message = "Segment data header returned an error: \n\n";
                            //message += "errorNumber: " + dataHeader.errorNum.ToString() + "\n";
                            //message += "errorInfo:   " + dataHeader.errorInfo;
                            //MessageBox.Show(message, "RF Sensor Error");
                            break;
                        }

                        //if (dataHeader.sequenceNumber == 0)
                        //{
                        //    // sequence number == 0, so this is a new measurement
                        //    // force a rescale whenever measurement restarts
                        //    gSpectrumYMax = double.MinValue;
                        //    gSpectrumYMin = double.MaxValue;
                        //    gSweepCount.reset(); // reset our sweep counter/timer
                        //}

                        if (dataHeader.segmentIndex == 0)
                        {
                            // segmentIndex == 0, so this is the beginning of a new sweep
                            numSweepsThisTime++;
                            //gSweepCount.plusplus();
                        }

                        if (dataHeader.numPoints > 0)
                        {
                            gMeasState = MeasState.Running;
                            // Invalidate the spectrum display area; this will force a repaint
                            // with the new data
                            gDataHeader = dataHeader;
                            freqstart = dataHeader.startFrequency;
                            freqstep = dataHeader.frequencyStep;
                            points = (int)dataHeader.numPoints;
                            overload = (dataHeader.overload != 0);
                            res = true;
                        }

                        if (dataHeader.lastSegment != 0)
                        {
                            gMeasState = MeasState.Stopped;
                            exitLoop = true;
                            //System.Diagnostics.Debug.WriteLine("Stopped");
                        }


                        break;
                    case AgSalLib.SalError.SAL_ERR_NO_DATA_AVAILABLE:
                        System.Threading.Thread.Sleep(sleep);
                        // OK, data just not ready
                        //System.Diagnostics.Debug.WriteLine("SAL_ERR_NO_DATA_AVAILABLE");
                        break;
                    default:
                        // treat other errors as restart
                        if (SensorError(err, "salIqGetData"))
                        {

                            if (gMeasHandle != IntPtr.Zero)
                            {
                                AgSalLib.salClose(gMeasHandle);
                                gMeasHandle = IntPtr.Zero;
                            }

                            StartSweep();
                        }
                        //System.Diagnostics.Debug.WriteLine("default");
                        break;
                }

            }



            switch (gMeasState)
            {
                case MeasState.Idle:
                    gMeasState = MeasState.Starting;
                    StartSweep();
                    break;
                case MeasState.Starting:
                    break;
                case MeasState.Running:
                    if (gSetupChange)
                    {
                        // we have a GUI change - send the stop command
                        AgSalLib.salSendSweepCommand(gMeasHandle, AgSalLib.SweepCommand.SweepCommand_stop);
                        //System.Diagnostics.Debug.WriteLine("Stop");
                        gMeasState = MeasState.Stopping;
                    }
                    break;

                case MeasState.Stopping:
                    break;
                case MeasState.Stopped:
                    // we have stopped (for a GUI change); restart the measurement
                    gMeasState = MeasState.Starting;
                    StartSweep();
                    break;
            }

            return res;
        }
        double fCentr = 0, fSpan = 0;
        uint FftPoints = 0;
        private bool StartSweep() // returns true if no errors
        {
            //System.Diagnostics.Debug.WriteLine("SetNew");
            uint numSegments = 1;

            AgSalLib.SalError err;
            AgSalLib.TunerParms tunerParms = new AgSalLib.TunerParms();
            AgSalLib.SweepParms sweepParms = new AgSalLib.SweepParms();
            AgSalLib.FrequencySegment[] fs = new AgSalLib.FrequencySegment[numSegments];


            if (fCentr < gSensorCapabilities.minFrequency) fCentr = gSensorCapabilities.minFrequency;
            else if (fCentr > gSensorCapabilities.maxFrequency) fCentr = gSensorCapabilities.maxFrequency;

            if (fSpan > gSensorCapabilities.maxSpan) fSpan = gSensorCapabilities.maxSpan;

            double desiredSampleRate = fSpan * gSensorCapabilities.sampleRateToSpanRatio;

            // *** WARNING: A workaround for an Electrum BUG... 
            // capabilities.maxSampleRate is wrongly set to 200kSa/s (i.e. DDC max sample rate),
            // while tuner's max sample rate is still 28MSa/s.
            //double sampleRate = gSensorCapabilities.maxSampleRate;
            if (gSensorCapabilities.maxSampleRate == 200.0e3) gSensorCapabilities.maxSampleRate = 28.0e6;
            double sampleRate = gSensorCapabilities.maxSampleRate;

            while (sampleRate / desiredSampleRate > 2)
            {
                sampleRate /= 2;
            }

            // Setup tuner
            tunerParms.centerFrequency = fCentr;
            tunerParms.sampleRate = sampleRate;
            tunerParms.attenuation = 0;
            tunerParms.mixerLevel = 0;
            tunerParms.antenna = antennaType;
            tunerParms.preamp = preAmp;

            err = AgSalLib.salSetTuner(gSensorHandle, ref tunerParms);

            if (SensorError(err, "salSetTuner"))
            {
                return false;
            }

            RBW = sampleRate / FftPoints * windowMult;

            uint numPoints = (uint)((double)FftPoints * (fSpan / sampleRate));
            if ((numPoints % 2) == 0) numPoints++; // want odd number so CF is in middle of display

            uint firstPoint = (FftPoints - (numPoints - 1)) / 2;

            for (int i = 0; i < numSegments; i++)
            {
                fs[i].attenuation = 0;
                fs[i].antenna = antennaType;
                fs[i].centerFrequency = fCentr;
                fs[i].sampleRate = sampleRate;
                fs[i].numAverages = 0;// (uint)guiNumAvg.Value;
                fs[i].averageType = AgSalLib.AverageType.Average_off;
                fs[i].numFftPoints = FftPoints;
                fs[i].firstPoint = firstPoint;
                fs[i].numPoints = numPoints;
                fs[i].preamp = preAmp;
                //if (overlapCB.Checked == true)
                //    fs[i].overlapType = AgSalLib.OverlapType.OverlapType_on;
                //else
                fs[i].overlapType = AgSalLib.OverlapType.OverlapType_off;
                // Disable tuner setting with each segment
                // Alloy inner loop optimization. Alloy-1762
                //fs[i].noTunerChange = 1;
            }

            sweepParms.numSweeps = 0; // sweep forever
            sweepParms.numSegments = numSegments;
            sweepParms.sweepInterval = 0;
            sweepParms.window = window;
            sweepParms.userWorkspace = IntPtr.Zero;


            sweepParms.monitorMode = AgSalLib.MonitorMode.MonitorMode_off;
            sweepParms.monitorInterval = 0.0;

            gDataHeader.numPoints = 0; // we are restarting, so mark data as invalid

            // Setup pacing
            AgSalLib.salFlowControl flowControl = new AgSalLib.salFlowControl();
            flowControl.pacingPolicy = 1;
            flowControl.maxBacklogSeconds = 0.5F;
            flowControl.maxBacklogMessages = 50;
            flowControl.maxBytesPerSec = 0F;
            flowControl.pacingPolicy = 0;

            err = AgSalLib.salStartSweep2(out gMeasHandle, gSensorHandle, ref sweepParms, ref fs, ref flowControl, null);

            if (SensorError(err, "salStartSweep"))
            {
                return false;
            }

            gSetupChange = false;

            return true;
        }

        #endregion Works




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
                        name = mainConfig.AdapterTraceResultPools[p].Size.ToString();
                        state = true;
                        break;
                    }
                }
            }
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
            config.AutoRefLevel = new CFG.AdapterAutoRefLevel()
            {
                Start_dBm = 10,
                Stop_dBm = -80,
                Step_dB = 10,
                PersentOverload = 15,
                NumberScan = 10
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
                    MinSize = 50,
                    MaxSize = 100,
                    Size = 5001
                },
                new CFG.AdapterResultPool()
                {
                    MinSize = 50,
                    MaxSize = 100,
                    Size = 10001
                },
                new CFG.AdapterResultPool()
                {
                    MinSize = 50,
                    MaxSize = 100,
                    Size = 20001
                },
                new CFG.AdapterResultPool()
                {
                    MinSize = 25,
                    MaxSize = 50,
                    Size = 40001
                },
                new CFG.AdapterResultPool()
                {
                    MinSize = 25,
                    MaxSize = 50,
                    Size = 100001
                },
                new CFG.AdapterResultPool()
                {
                    MinSize = 10,
                    MaxSize = 20,
                    Size = 200002
                },
                new CFG.AdapterResultPool()
                {
                    MinSize = 10,
                    MaxSize = 20,
                    Size = 500005
                },
                 new CFG.AdapterResultPool()
                {
                    MinSize = 5,
                    MaxSize = 10,
                    Size = 1200000
                },
            };
        }
        private (MesureTraceDeviceProperties, MesureIQStreamDeviceProperties) GetProperties(CFG.AdapterMainConfig config)
        {
            RadioPathParameters[] rrps = ConvertRadioPathParameters(config);
            AgSalLib.salGetSensorAttribute(gSensorHandle, AgSalLib.SensorAttribute.ATTENUATION_MAX, out attMax);
            AgSalLib.salGetSensorAttribute(gSensorHandle, AgSalLib.SensorAttribute.ATTENUATION_MIN, out attMin);
            AgSalLib.salGetSensorAttribute(gSensorHandle, AgSalLib.SensorAttribute.ATTENUATION_STEP, out attStep);
            FreqMin = (decimal)gSensorCapabilities.minFrequency;
            FreqMax = (decimal)gSensorCapabilities.maxFrequency;
            StandardDeviceProperties sdp = new StandardDeviceProperties()
            {
                AttMax_dB = (int)attMax,
                AttMin_dB = (int)attMin,
                FreqMax_Hz = FreqMax, //6000000000
                FreqMin_Hz = FreqMin, //20000000
                PreAmpMax_dB = 1, //типа включен/выключен
                PreAmpMin_dB = 0,
                RefLevelMax_dBm = 0,//(int)RefLevelMax,
                RefLevelMin_dBm = 0,//(int)RefLevelMin,
                EquipmentInfo = new EquipmentInfo()
                {
                    AntennaCode = config.AdapterEquipmentInfo.AntennaSN,// "Omni",//S/N  В конфиг
                    AntennaManufacturer = config.AdapterEquipmentInfo.AntennaManufacturer,//"3anet",//В конфиг
                    AntennaName = config.AdapterEquipmentInfo.AntennaName,//"BC600",//В конфиг
                    EquipmentManufacturer = new Atdi.DataModels.Sdrn.DeviceServer.Adapters.InstrManufacrures().Keysight.UI,
                    EquipmentName = SensorInfo.modelNumber,
                    EquipmentFamily = "SpectrumAnalyzer",//SDR/SpecAn/MonRec
                    EquipmentCode = SensorInfo.serialNumber,//S/N

                },
                RadioPathParameters = rrps
            };
            //if (preAmpAvailable)
            //{
            //    sdp.PreAmpMax_dB = 1;
            //}
            //else
            //{
            //    sdp.PreAmpMax_dB = 0;
            //}
            //

            (double rbwmin, uint numPoints1) = Calc(0, fftSize.Length - 1);
            (double rbwmax, uint numPoints2) = Calc(gSpans.Length -1, 0);
            MesureTraceDeviceProperties mtdp = new MesureTraceDeviceProperties()
            {
                RBWMax_Hz = rbwmax,
                RBWMin_Hz = rbwmin,
                SweepTimeMin_s = 0.003125,
                SweepTimeMax_s = 0.3125,
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
