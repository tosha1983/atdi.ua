using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;
using System.ComponentModel;


using Atdi.Contracts.Sdrn.DeviceServer;
using Atdi.DataModels.Sdrn.DeviceServer;
using Atdi.Platform.Logging;

using AD = Atdi.DataModels.Sdrn.DeviceServer.Adapters;
using CFG = Atdi.DataModels.Sdrn.DeviceServer.Adapters.Config;
using COM = Atdi.DataModels.Sdrn.DeviceServer.Commands;
using COMR = Atdi.DataModels.Sdrn.DeviceServer.Commands.Results;
using COMRMSI = Atdi.DataModels.Sdrn.DeviceServer.Commands.Results.MesureSystemInfo;
using MEN = Atdi.DataModels.Sdrn.DeviceServer.Adapters.Enums;
using RohdeSchwarz.ViCom;
using RohdeSchwarz.ViCom.Net;
using RohdeSchwarz.ViCom.Net.GSM;
using RohdeSchwarz.ViCom.Net.LTE;
using RohdeSchwarz.ViCom.Net.CDMA;
using RohdeSchwarz.ViCom.Net.WCDMA;
using RohdeSchwarz.ViCom.Net.RFPOWERSCAN;
using RohdeSchwarz.ViCom.Net.GPS;
using RohdeSchwarz.ViCom.Net.ACD;

using System.Diagnostics;
using System.Collections.Generic;

namespace Atdi.AppUnits.Sdrn.DeviceServer.Adapters.RSTSMx
{
    public class Adapter : IAdapter
    {
        private readonly ITimeService _timeService;
        private readonly ILogger _logger;
        private readonly AdapterConfig _adapterConfig;
        private LocalParametersConverter LPC;
        private CFG.ThisAdapterConfig TAC;
        private CFG.AdapterMainConfig MainConfig;

        string ViComBinPath = "";


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
            GSMBTS = new List<GSMBTSData>() { };
            ViComBinPath = _adapterConfig.RSViComPath + @"\bin\"; //@"c:\RuS\RS-ViCom-Pro-16.25.0.743\bin\";//@"c:\RuS\RS-ViCom-16.5.0.0\bin\";
            string newPath = Environment.GetEnvironmentVariable("PATH") + @";" + ViComBinPath + ";";
            Environment.SetEnvironmentVariable("PATH", newPath);
            _DeviceType = _adapterConfig.DeviceType;
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

                if (Connect(_adapterConfig.IPAddress))
                {
                    string filename = new Atdi.DataModels.Sdrn.DeviceServer.Adapters.InstrManufacrures().RuS.UI + "_" + SerialNumber + ".xml";
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
                    //(MesureTraceDeviceProperties mtdp, MesureIQStreamDeviceProperties miqdp) = GetProperties(MainConfig);
                    //host.RegisterHandler<COM.MesureTraceCommand, COMR.MesureTraceResult>(MesureTraceCommandHandler, mtdp);
                    //host.RegisterHandler<COM.MesureIQStreamCommand, COMR.MesureIQStreamResult>(MesureIQStreamCommandHandler, miqdp);
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
                //TAC.Save();
                GpsDisconnect();
                GsmDisconnect();
            }
            #region Exception
            catch (Exception exp)
            {
                _logger.Exception(Contexts.ThisComponent, exp);
            }
            #endregion
        }

        public void MesureSystemInfoHandler(COM.MesureSystemInfoCommand command, IExecutionContext context)
        {
            try
            {
                //if (IsRuning)
                //{
                //    /// если нужно проводим блокировку комманд который мы не сможем обслужить пока что то меряем в течении выполнени яэтой комманды
                //    /// и подсказывая этим инфраструктуре что этот устрйостов некоторое время не сможет обрабатываить такие комманды
                //    //context.Lock(CommandType.MesureTrace);

                //    // если нужно заблокировать выполняему комманду то достатчоно вызвать метод без параметров и блокируется комманда которая выполняется
                //    context.Lock();
                //    for (ulong i = 0; i < TraceCountToMeas; i++)
                //    {
                //        newres = GetTrace();
                //        if (newres)
                //        {
                //            // пушаем результат
                //            var result = new COMR.MesureTraceResult(TraceCount, CommandResultStatus.Next);
                //            TraceCount++;
                //            if (TraceCountToMeas == TraceCount)
                //            {
                //                result = new COMR.MesureTraceResult(TraceCount, CommandResultStatus.Final);
                //            }
                //            result.Att_dB = (int)AttLevel;
                //            result.RefLevel_dBm = (int)RefLevel;
                //            result.PreAmp_dB = PreAmp ? 1 : 0;
                //            result.RBW_Hz = (double)RBW;
                //            result.VBW_Hz = (double)VBW;
                //            result.Freq_Hz = new double[FreqArr.Length];
                //            result.Level = new float[FreqArr.Length];
                //            for (int j = 0; j < FreqArr.Length; j++)
                //            {
                //                result.Freq_Hz[j] = FreqArr[j];
                //                result.Level[j] = LevelArr[j];
                //            }
                //            //result.TimeStamp = _timeService.TimeStamp.Ticks - new DateTime(1970, 1, 1, 0, 0, 0, System.DateTimeKind.Utc).Ticks;//неюзабельно
                //            result.TimeStamp = DateTime.UtcNow.Ticks - new DateTime(1970, 1, 1, 0, 0, 0, System.DateTimeKind.Utc).Ticks;

                //            context.PushResult(result);
                //        }
                //        else
                //        {
                //            i--;
                //        }
                //        // иногда нужно проверять токен окончания работы комманды
                //        if (context.Token.IsCancellationRequested)
                //        {
                //            // все нужно остановиться

                //            // если есть порция данных возвращаем ее в обработчки только говрим что поток результатов не законченный и больше уже не будет поступать
                //            var result2 = new COMR.MesureTraceResult(TraceCount, CommandResultStatus.Ragged);

                //            context.PushResult(result2);


                //            // подтверждаем факт обработки отмены
                //            context.Cancel();
                //            // освобождаем поток 
                //            return;
                //        }
                //    }



                //    context.Unlock();

                //    // что то делаем еще 


                //    // подтверждаем окончание выполнения комманды 
                //    // важно: всн ранее устапнволеные в контексте обработки текущей команыд блокировки снимаются автоматически
                //    context.Finish();
                //    // дальше кода быть не должно, освобождаем поток
                //}
                //else
                //{
                //    throw new Exception("The device with serial number " + SerialNumber + " does not work");
                //}
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
        private static string IPAddress = "192.168.0.2";
        private string SerialNumber
        {
            get { return _SerialNumber; }
            set { _SerialNumber = value; }
        }
        private static string _SerialNumber = "";
        private static long _LastUpdate;
        public long LastUpdate
        {
            get { return _LastUpdate; }
            set { _LastUpdate = value; }
        }
        private static int _DeviceType = 0;
        private static DeviceType DeviceType
        {
            get
            {
                if (_DeviceType == 0)
                { return DeviceType.Tsmw; }
                else if (_DeviceType == 1)
                { return DeviceType.Tsme; }
                else if (_DeviceType == 2)
                { return DeviceType.Tsme6; }
                else return DeviceType.Tsme;
            }
        }
        UserLowLevelErrorMessageHandler.LowLevelErrorHandlerRegistry LowLevelErrorHandlerRegistry;
        LowLevelErrorHandlerImplementation MyLowLevelErrorHandlerImplementation;
        MessageTracer rMessageTracer = new MessageTracer();
        CViComError error;
        CReceiverListener receiverListener;
        SConnectedReceiverTable myReceivers;
        public static List<SConnectedReceiverTable.SReceiver.SDeviceOption> option { get; set; }

        RohdeSchwarz.ViCom.Net.CViComBasicInterface BasicInterface;


        CViComLoader<CViComGpsInterface> gpsLoader;
        CViComGpsInterface GpsInterface;
        CViComBasicInterface GpsBasicInterface;
        CViComGpsInterfaceDataProcessor GPSListener;

        CViComLoader<CViComGsmInterface> gsmLoader;
        static CViComGsmInterface gsmInterface;
        CViComBasicInterface gsmBasicInterface;
        CViComGsmInterfaceDataProcessor GSMListener;

        static CViComLoader<CViComWcdmaInterface> wcdmaLoader;
        static CViComWcdmaInterface wcdmaInterface;
        static CViComBasicInterface wcdmaBasicInterface;
        static CViComWcdmaInterfaceDataProcessor WCDMAListener;

        static CViComLoader<CViComCdmaInterface> cdmaLoader;
        static CViComCdmaInterface cdmaInterface;
        static CViComBasicInterface cdmaBasicInterface;
        static CViComCdmaInterfaceDataProcessor CDMAListener;

        static CViComLoader<CViComLteInterface> lteLoader;
        static CViComLteInterface lteInterface;
        static CViComBasicInterface lteBasicInterface;
        static CViComLteInterfaceDataProcessor LteListener;

        CViComLoader<CViComRFPowerScanInterface> RFPowerScanLoader;
        static CViComRFPowerScanInterface RFPowerScanInterface;
        CViComBasicInterface RFPowerScanBasicInterface;
        CViComRFPowerScanInterfaceDataProcessor RFPowerScanListener;

        CViComLoader<CViComAcdInterface> acdLoader;
        CViComAcdInterface acdInterface;
        CViComBasicInterface acdBasicInterface;
        CViComAcdInterfaceDataProcessor ACDListener;


        static SSweepSettings rSSweepSettings = new SSweepSettings();

        public static double DetectionLevelGSM = -100;
        public static double DetectionLevelUMTS = -100;
        public static double DetectionLevelLTE = -100;
        public static double DetectionLevelCDMA = -100;


        private static List<GSMBTSData> GSMBTS;
        static bool GSMIsRuning = false;
        private static List<decimal> GSMUniFreq = new List<decimal>() { };
        private List<SIType> GSMSITypes = new List<SIType>() { };

        //private static List<UMTSBTSData> UMTSBTS;
        static bool UMTSIsRuning = false;
        private static List<decimal> UMTSUniFreq = new List<decimal>() { };
        private List<SIType> UMTSSITypes = new List<SIType>() { };
        #endregion

        #region Classes
        class CReceiverListener : RohdeSchwarz.ViCom.Net.CViComReceiverListener
        {
            public CReceiverListener()
            { }

            //* CViComReceiverListener interface implementation ************************************

            public override void OnConnectProgress(float progressInPct, String message)
            {
                if (message.Length > 0 && message.ToLower().Contains("connected"))
                {
                    int StartSn = 0;
                    int StopSn = 0;
                    StartSn = message.ToLower().IndexOf("s.n. ") + 5;
                    StopSn = message.ToLower().IndexOf(":", StartSn);
                    if (StartSn > 0 && StopSn > 0 && StartSn < StopSn)
                    {
                        string sn = message.Substring(StartSn, StopSn - StartSn);
                        //if (SerialNumberTemp != sn) SerialNumberTemp = sn;
                        Debug.WriteLine("connected " + sn);
                    }
                }
            }
            public override void OnWarning(Warning.Type warning, String message)
            {
                //Console.WriteLine("OnWarning: " + message);
            }
            public override void OnError(Error.Type error, String message)
            {
                //Console.WriteLine("OnError: " + message);
            }
        };
        class MessageTracer : ViComMessageTracer
        {
            public void OnMessage(string text)
            {
                Debug.WriteLine(String.Format("T {0} {1}", DateTime.Now, text) + "\r\n");//System.Console.WriteLine(text);
            }
        }

        class LowLevelErrorHandlerImplementation : UserLowLevelErrorMessageHandler.LowLevelErrorHandler
        {
            public void OnLowLevelError(string text, string module, uint nType)
            {
                //Console.WriteLine("LowLevelError: Module '{0}' reported error: {1}, nType={2}", module, text, nType);
            }
        }
        #endregion


        private bool Connect(string ipaddress)
        {
            IPAddress = ipaddress;
            bool res = false;
            Debug.WriteLine("0");
            GpsConnect();
            GsmConnect();
            res = true;
            return res;
        }
        #region GPS
        private void GpsConnect()
        {
            try
            {
                ViComMessageTracerRegistry.Register(new MessageTracer());
                gpsLoader = new CViComLoader<CViComGpsInterface>(DeviceType);
                gpsLoader.Connect(IPAddress, out error, receiverListener); //GPSIsRuning


                GpsInterface = gpsLoader.GetInterface();
                GpsBasicInterface = gpsLoader.GetBasicInterface();
                var resbuf = new SResultBufferDepth();
                resbuf.dwValue = SResultBufferDepth.dwMax;
                GpsBasicInterface.SetResultBufferDepth(resbuf);

                var settings = new SGPSDeviceSettings();
                settings.enGnssMode = GnssMode.Type.GPS;
                settings.enGPSMessageFormat = GPSMessageFormat.Type.VICOM_GPS_FORMAT_NMEA;
                settings.enResetMode = ResetMode.Type.NONE;
                settings.deadReckoningSettings.enState = SDeadReckoningSettings.State.Type.DISABLED;

                GpsInterface.SetGPSDeviceSettings(settings);

                GPSListener = new MyGpsDataProcessor(_logger);

                GpsInterface.RegisterResultDataListener(GPSListener);

                GpsBasicInterface.StartMeasurement();
            }
            #region Exception
            catch (RohdeSchwarz.ViCom.Net.CViComError error)
            {
                _logger.Exception(Contexts.ThisComponent, new Exception("ErrorCode:" + error.ErrorCode + " ErrorString:" + error.ErrorString));
            }
            catch (Exception exp)
            {
                _logger.Exception(Contexts.ThisComponent, exp);
            }
            #endregion
        }
        private void GpsDisconnect()
        {
            try
            {
                if (gpsLoader != null)
                {
                    if (gpsLoader.GetBasicInterface().IsMeasurementStarted())
                    {
                        gpsLoader.GetBasicInterface().StopMeasurement();
                        gpsLoader.GetBasicInterface().HasMeasurementStopped(SDefs.dwDefaultTimeOutInMs);

                        GpsInterface.UnregisterResultDataListener(GPSListener);
                        gpsLoader.Disconnect();
                    }
                }
            }
            #region Exception
            catch (RohdeSchwarz.ViCom.Net.CViComError error)
            {
                _logger.Exception(Contexts.ThisComponent, new Exception("ErrorCode:" + error.ErrorCode + " ErrorString:" + error.ErrorString));
            }
            catch (Exception exp)
            {
                _logger.Exception(Contexts.ThisComponent, exp);
            }
            #endregion

        }
        #endregion GPS     

        private static uint GetDeviceRFInput(bool type, uint rf, string tech)
        {
            uint rfin = 0;
            if (DeviceType == RohdeSchwarz.ViCom.Net.DeviceType.Tsmw)
            {
                //if (type == true)
                //{
                //    if (App.Sett.TSMxReceiver_Settings.TSMWRFInput == 1) rfin = 1;
                //    else if (App.Sett.TSMxReceiver_Settings.TSMWRFInput == 2) rfin = 2;
                //    else if (App.Sett.TSMxReceiver_Settings.TSMWRFInput == 3) rfin = rf;
                //    else if (App.Sett.TSMxReceiver_Settings.TSMWRFInput == 4)
                //    {
                //        if (tech == "GSM") rfin = (uint)App.Sett.TSMxReceiver_Settings.GSM.TSMWRfInput;
                //        else if (tech == "UMTS") rfin = (uint)App.Sett.TSMxReceiver_Settings.UMTS.TSMWRfInput;
                //        else if (tech == "LTE") rfin = (uint)App.Sett.TSMxReceiver_Settings.LTE.TSMWRfInput;
                //        else if (tech == "CDMA") rfin = (uint)App.Sett.TSMxReceiver_Settings.CDMA.TSMWRfInput;
                //    }
                //}
                //else if (type == false)
                //{
                //    if (App.Sett.TSMxReceiver_Settings.TSMWRFInput == 1) rfin = 2;
                //    else if (App.Sett.TSMxReceiver_Settings.TSMWRFInput == 2) rfin = 1;
                //    else if (App.Sett.TSMxReceiver_Settings.TSMWRFInput == 3) rfin = rf;
                //}
            }
            else if (DeviceType == RohdeSchwarz.ViCom.Net.DeviceType.Tsme)
            {
                rfin = 1;
            }
            else if (DeviceType == RohdeSchwarz.ViCom.Net.DeviceType.Tsme6)
            {
                rfin = 1;
            }

            if (DeviceType == RohdeSchwarz.ViCom.Net.DeviceType.Unknown)
            {
                //.Message = "Выберете модель приемника R&S TSMx";
            }
            //if (App.Sett.TSMxReceiver_Settings.TSMWRFInput == 0)
            //{
            //    //.Message = "Выберете тип подлючения антенных входов R&S TSMW";
            //}
            return rfin;
        }
        #region GSM

        private void GsmConnect()
        {
            try
            {
                gsmLoader = new CViComLoader<RohdeSchwarz.ViCom.Net.GSM.CViComGsmInterface>(DeviceType);
                GetUnifreqsGSM();
                bool Runing = gsmLoader.Connect(IPAddress, out error, receiverListener);
                if (Runing)
                {
                    //if (error != null) { Errors += "\r\n" + "error " + error.ErrorCode + error.ErrorString; }
                    gsmInterface = gsmLoader.GetInterface();
                    gsmBasicInterface = gsmLoader.GetBasicInterface();

                    var buf = new SResultBufferDepth();
                    buf.dwValue = 1024;
                    gsmBasicInterface.SetResultBufferDepth(buf);

                    //RohdeSchwarz.ViCom.Net.SRange<uint> rateLimit = (RohdeSchwarz.ViCom.Net.SRange<uint>)gsmInterface.GetMeasRateLimits();

                    var channelSettings = new RohdeSchwarz.ViCom.Net.GSM.SChannelSettings();
                    channelSettings.dwFrontEndSelectionMask = GetDeviceRFInput(true, 1, "GSM"); //вроде как канал приемника 1/2
                    channelSettings.dwMeasRatePer1000Sec = 250000;// 50000;// ((RohdeSchwarz.ViCom.Net.SRange<uint>)gsmInterface.GetMeasRateLimits()).minimum; //rateLimit.maximum; //вроде как скорость сканирования
                    channelSettings.dwCount = (uint)GSMUniFreq.Count();
                    channelSettings.pTableOfFrequencySetting = new RohdeSchwarz.ViCom.Net.GSM.SFrequencySetting[channelSettings.dwCount];


                    for (int i = 0; i < GSMUniFreq.Count; i++)
                    {
                        channelSettings.pTableOfFrequencySetting[i] = new RohdeSchwarz.ViCom.Net.GSM.SFrequencySetting();
                        channelSettings.pTableOfFrequencySetting[i].dCenterFrequencyInHz = (double)GSMUniFreq[i];
                    }

                    gsmInterface.SetFrequencyTable(channelSettings);

                    SMeasurementDetails det = new SMeasurementDetails();
                    det.ChannelPowerSpec = new SChannelPowerSpec();
                    det.ChannelPowerSpec.wCountOfResultsPerGSMTimeSlot = 16;
                    det.ChannelPowerSpec.wRMSLengthIn40ns = 900;
                    //det.SpectrumSpec = new SSpectrumSpec();
                    //det.SpectrumSpec.eFreqDetector = SSpectrumSpec.FreqDetector.Type.PEAK;
                    //det.SpectrumSpec.eTimeDetector = SSpectrumSpec.TimeDetector.Type.PEAK;
                    //det.SpectrumSpec.wCollectionTimeIn100us = 50;
                    //det.SpectrumSpec.wCountOfPowerValuesPerChannel = 1;

                    det.pTableOfChannelMeasSpec = new SChannelMeasSpec[channelSettings.pTableOfFrequencySetting.Count()];
                    det.dwCount = (uint)channelSettings.pTableOfFrequencySetting.Count();

                    for (int i = 0; i < channelSettings.pTableOfFrequencySetting.Count(); i++)
                    {
                        det.pTableOfChannelMeasSpec[i] = new SChannelMeasSpec();
                        det.pTableOfChannelMeasSpec[i].dwFrequencyIndex = (uint)i;
                        det.pTableOfChannelMeasSpec[i].bMEAS_CARRIER_TO_INTERFERENCE = true;
                        det.pTableOfChannelMeasSpec[i].bMEAS_CHANNEL_POWER = true;
                        det.pTableOfChannelMeasSpec[i].bMEAS_DB_POWER = false;
                        det.pTableOfChannelMeasSpec[i].bMEAS_DB_REMOVAL = false;
                        det.pTableOfChannelMeasSpec[i].bMEAS_REPORT_FAILED_TRIALS = false;
                        det.pTableOfChannelMeasSpec[i].bMEAS_SCH = true;
                        det.pTableOfChannelMeasSpec[i].bMEAS_SPECTRUM = false;
                        det.pTableOfChannelMeasSpec[i].bMEAS_TSC = true;
                    }
                    gsmInterface.SetMeasurementDetails(det);


                    List<RohdeSchwarz.ViCom.Net.GSM.Pdu.Type> siblist = new List<RohdeSchwarz.ViCom.Net.GSM.Pdu.Type>() { };
                    for (int i = 0; i < GSMSITypes.Count; i++)
                    {
                        if (GSMSITypes[i].Use)
                        {
                            siblist.Add((RohdeSchwarz.ViCom.Net.GSM.Pdu.Type)Enum.Parse(typeof(RohdeSchwarz.ViCom.Net.GSM.Pdu.Type), GSMSITypes[i].SiType));
                        }
                    }

                    RohdeSchwarz.ViCom.Net.GSM.SDemodulationSettings demod = new RohdeSchwarz.ViCom.Net.GSM.SDemodulationSettings();
                    uint dwRequests = (uint)(siblist.Count * GSMUniFreq.Count());
                    demod.dwFrontEndSelectionMask = GetDeviceRFInput(true, 1, "GSM");
                    demod.lTotalPowerOffsetInDB10 = 100;

                    RohdeSchwarz.ViCom.Net.GSM.SDemodRequests MeasurementRequests = new RohdeSchwarz.ViCom.Net.GSM.SDemodRequests();
                    MeasurementRequests.dwCountOfRequests = dwRequests;
                    MeasurementRequests.pDemodRequests = new RohdeSchwarz.ViCom.Net.GSM.SDemodRequests.SDemodRequest[dwRequests];

                    for (int i = 0; i < GSMUniFreq.Count(); i++)
                    {
                        int dwRequestStartIndex = i * siblist.Count;
                        for (int idx = 0; idx < siblist.Count; ++idx)
                        {
                            int iR = dwRequestStartIndex + idx;
                            MeasurementRequests.pDemodRequests[iR] = new RohdeSchwarz.ViCom.Net.GSM.SDemodRequests.SDemodRequest();
                            MeasurementRequests.pDemodRequests[iR].dwChannelIndex = (uint)i;
                            MeasurementRequests.pDemodRequests[iR].ePDU = siblist[idx];
                            MeasurementRequests.pDemodRequests[iR].eDemodulationMode = RohdeSchwarz.ViCom.Net.GSM.DemodMode.Type.ONCE;
                            //MeasurementRequests.pDemodRequests[iR].wRepetitionDelayIn100ms = 0;//1000;

                        }
                    }
                    demod.sStartMeasurementRequests = MeasurementRequests;
                    gsmInterface.SetDemodulationSettings(demod);


                    GSMListener = new MyGsmDataProcessor(_logger, _timeService);

                    gsmInterface.RegisterResultDataListener(GSMListener);

                    gsmBasicInterface.StartMeasurement();
                    GSMIsRuning = Runing;
                }
            }
            #region Exception
            catch (RohdeSchwarz.ViCom.Net.CViComError error)
            {
                _logger.Exception(Contexts.ThisComponent, new Exception("ErrorCode:" + error.ErrorCode + " ErrorString:" + error.ErrorString));
            }
            catch (Exception exp)
            {
                _logger.Exception(Contexts.ThisComponent, exp);
            }
            #endregion
        }
        private void GsmDisconnect()
        {
            try
            {
                if (gsmLoader != null)
                {
                    if (gsmLoader.GetBasicInterface().IsMeasurementStarted())
                    {
                        gsmLoader.GetBasicInterface().StopMeasurement();
                        gsmLoader.GetBasicInterface().HasMeasurementStopped(SDefs.dwDefaultTimeOutInMs);
                        gsmInterface.UnregisterResultDataListener(GSMListener);
                        gsmLoader.Disconnect();
                    }
                }
            }
            #region Exception
            catch (RohdeSchwarz.ViCom.Net.CViComError error)
            {
                _logger.Exception(Contexts.ThisComponent, new Exception("ErrorCode:" + error.ErrorCode + " ErrorString:" + error.ErrorString));
            }
            catch (Exception exp)
            {
                _logger.Exception(Contexts.ThisComponent, exp);
            }
            #endregion
        }
        private void GetUnifreqsGSM()
        {
            #region GSM 
            List<decimal> freq1 = new List<decimal>() { };
            int ind = 0;
            for (decimal i = 918200000; i <= 959800000; i += 200000)
            { freq1.Add(i); }
            for (decimal i = 1805200000; i <= 1879800000; i += 200000)
            { freq1.Add(i); }
            //foreach (Settings.GSMBand_Set t in App.Sett.TSMxReceiver_Settings.GSM.Bands)
            //{
            //    if (t.Use == true)
            //    {
            //        for (decimal i = t.FreqStart; i <= t.FreqStop; i += 200000)
            //        { freq1.Add(i); }
            //    }
            //}
            System.Collections.Generic.HashSet<decimal> hs1 = new System.Collections.Generic.HashSet<decimal>();
            foreach (decimal al in freq1)
            {
                hs1.Add(al);
            }
            GSMUniFreq.Clear();
            GSMUniFreq = new List<decimal>(hs1.OrderBy(i => i));


            GSMSITypes.Add(
                new SIType()
                {
                    SiType = RohdeSchwarz.ViCom.Net.GSM.Pdu.Type.SITYPE_1.ToString(),
                    Use = true
                }
                );
            GSMSITypes.Add(
                new SIType()
                {
                    SiType = RohdeSchwarz.ViCom.Net.GSM.Pdu.Type.SITYPE_3.ToString(),
                    Use = true
                }
                );
            #endregion
        }
        #endregion GSM

        #region DataProcessors

        static double LevelIsMaxIfMoreBy = 5;
        private class MyGpsDataProcessor : RohdeSchwarz.ViCom.Net.GPS.CViComGpsInterfaceDataProcessor
        {
            private readonly ILogger _logger;
            public MyGpsDataProcessor(ILogger logger)
            {
                _logger = logger;
            }
            public override void RegisterScannerId(ushort dwScannerDataId)
            {
                //MainWindow.exp.ExceptionData = new ExData() { ex = new Exception("ID"), ClassName = "TSMxReceiver", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name + "\r\n" + dwScannerDataId };
            }

            public override void RemoveScannerId(ushort dwScannerDataId)
            {
                //MainWindow.exp.ExceptionData = new ExData() { ex = new Exception("ID"), ClassName = "TSMxReceiver", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name + "\r\n" + dwScannerDataId };
            }

            public override void OnScannerDataMeasured(RohdeSchwarz.ViCom.Net.GPS.SMeasResult pData)
            {
                try
                {
                    _LastUpdate = DateTime.Now.Ticks;
                    foreach (SGPSMessage sgpsm in pData.ListOfMessages)
                    {
                        if (sgpsm.enMessageFormat == GPSMessageFormat.Type.VICOM_GPS_FORMAT_NMEA)
                        {
                            string mes = System.Text.UnicodeEncoding.UTF8.GetString(sgpsm.pbMessageText).TrimEnd();
                            Console.WriteLine(mes);
                            //ReadNMEAData(mes);
                            //GPSAntennaState = pData.sReceiverInfo.enAntennaState.ToString();
                        }
                    }
                }
                #region Exception
                catch (RohdeSchwarz.ViCom.Net.CViComError error)
                {
                    _logger.Exception(Contexts.ThisComponent, new Exception("ErrorCode:" + error.ErrorCode + " ErrorString:" + error.ErrorString));
                }
                catch (Exception exp)
                {
                    _logger.Exception(Contexts.ThisComponent, exp);
                }
                #endregion
            }
        }

        private class MyGsmDataProcessor : RohdeSchwarz.ViCom.Net.GSM.CViComGsmInterfaceDataProcessor
        {
            private readonly ILogger _logger;
            private readonly ITimeService _timeService;
            public MyGsmDataProcessor(ILogger logger, ITimeService timeService)
            {
                _logger = logger;

                _timeService = timeService;
            }
            public override void RegisterScannerId(ushort dwScannerDataId)
            {
                //MainWindow.exp.ExceptionData = new ExData() { ex = new Exception("ID"), ClassName = "TSMxReceiver", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name + "\r\n" + dwScannerDataId };
            }
            public override void RemoveScannerId(ushort dwScannerDataId)
            {
                //MainWindow.exp.ExceptionData = new ExData() { ex = new Exception("ID"), ClassName = "TSMxReceiver", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name + "\r\n" + dwScannerDataId };
            }
            public override void OnScannerDataMeasured(RohdeSchwarz.ViCom.Net.GSM.SMeasResult pData)
            {
                _LastUpdate = DateTime.Now.Ticks;
                if (GSMIsRuning)
                {
                    //App.Current.Dispatcher.BeginInvoke((Action)(() =>
                    //{
                    try
                    {
                        //BSIC
                        if (pData.ListSCHInfoResults.Count > 0)
                        {
                            foreach (var ssch in pData.ListSCHInfoResults)
                            {
                                #region
                                bool find = false;
                                for (int i = 0; i < GSMBTS.Count; i++)
                                {
                                    if (GSMBTS[i].FreqIndex == ssch.dwFrequencyIndex && GSMBTS[i].BSIC == Convert.ToInt32(Convert.ToString(ssch.wBSIC, 8)))
                                    {
                                        GSMBTS[i].IndSCHInfo = ssch.dwIndicatorOfSCHInfo;
                                        GSMBTS[i].IndFirstSCHInfo = ssch.dwIndicatorOfFirstSCHInfo;
                                        find = true;
                                    }
                                    #region
                                    else if (GSMBTS[i].FreqIndex == ssch.dwFrequencyIndex && GSMBTS[i].BSIC == -1)
                                    {
                                        GSMBTS[i].TimeOfSlotInSec = ssch.dTimeOfSlotInSec;
                                        GSMBTS[i].BSIC = Convert.ToInt32(Convert.ToString(ssch.wBSIC, 8));
                                        GSMBTS[i].IndSCHInfo = ssch.dwIndicatorOfSCHInfo;
                                        GSMBTS[i].IndFirstSCHInfo = ssch.dwIndicatorOfFirstSCHInfo;
                                        find = true;
                                    }
                                    #endregion
                                }
                                if (find == false)
                                {
                                    GSM_Channel t = GetGSMCHfromFreqDN(GSMUniFreq[(int)ssch.dwFrequencyIndex]);
                                    GSMBTSData dt = new GSMBTSData()
                                    {
                                        BSIC = Convert.ToInt32(Convert.ToString(ssch.wBSIC, 8)),
                                        IndSCHInfo = ssch.dwIndicatorOfSCHInfo,
                                        IndFirstSCHInfo = ssch.dwIndicatorOfFirstSCHInfo,
                                        FreqIndex = ssch.dwFrequencyIndex,
                                        ARFCN = t.ARFCN,
                                        FreqDn = t.FreqDn,
                                        FreqUp = t.FreqUp,
                                        StandartSubband = t.StandartSubband,
                                    };
                                    GSMBTS.Add(dt);
                                }
                                #endregion
                            }
                        }
                        //POWER
                        if (pData.ListPowerResults.Count > 0)
                        {
                            foreach (var pr in pData.ListPowerResults)
                            {
                                #region
                                if (pr.dwIndicatorOfSCHInfo != 4294967295)// && pr.eMeasMode == RohdeSchwarz.ViCom.Net.GSM.SMeasResult.SPowerResult.etMeasMode.MEASMODE_DEMOD)// && pr.eMeasType == RohdeSchwarz.ViCom.Net.GSM.SMeasResult.SPowerResult.etMeasType.MEASTYPE_POWCH)
                                {
                                    bool find = false;
                                    for (int i = 0; i < GSMBTS.Count; i++)
                                    {

                                        if (GSMBTS[i].FreqIndex == pr.dwFrequencyIndex && pr.dwIndicatorOfSCHInfo == GSMBTS[i].IndSCHInfo)
                                        {
                                            GSMBTS[i].Power = pr.sPowerInDBm100 * 0.01;
                                            GSMBTS[i].LastLevelUpdete = _timeService.GetGnssTime().Ticks;// LocalTime;
                                            if (GSMBTS[i].FullData)
                                            {
                                                GSMBTS[i].GetStationInfo();
                                            }
                                            if (GSMBTS[i].Power > DetectionLevelGSM)/////////////////////////////////////////////////////////////////////
                                            { GSMBTS[i].LastDetectionLevelUpdete = GSMBTS[i].LastLevelUpdete; }
                                            // GSMBTS[i].DeleteFromMeasMon = (GSMBTS[i].Power < DetectionLevelGSM - LevelDifferenceToRemove);

                                            bool freqLevelMax = true;
                                            for (int l = 0; l < GSMBTS.Count; l++)
                                            {
                                                if (GSMBTS[l].FreqDn == GSMBTS[i].FreqDn &&
                                                    GSMBTS[l].GCID != GSMBTS[i].GCID)
                                                {
                                                    if (GSMBTS[l].Power + LevelIsMaxIfMoreBy < GSMBTS[i].Power)
                                                        GSMBTS[l].ThisIsMaximumSignalAtThisFrequency = false;
                                                    else { freqLevelMax = false; }
                                                }

                                            }

                                            GSMBTS[i].ThisIsMaximumSignalAtThisFrequency = freqLevelMax;
                                            if (pr.psCarrierToInterferenceInDB100 != null)
                                                GSMBTS[i].CarToInt = (double)pr.psCarrierToInterferenceInDB100 * 0.01;
                                            GSMBTS[i].TimeOfSlotInSec = pr.dTimeOfSlotInSec - pData.u64DeviceTimeInNs / 1000000000;

                                            find = true;
                                            #region LevelMeasurementsCar
                                            //if (MainWindow.gps.Altitude != 0 && MainWindow.gps.LatitudeDecimal != 0 && MainWindow.gps.LongitudeDecimal != 0)
                                            //{
                                            //    double dist = 0, ang = 0;
                                            //    if (GSMBTS[i].level_results.Count > 0)
                                            //    {
                                            //        MainWindow.help.calcDistance(
                                            //              (double)GSMBTS[i].level_results[GSMBTS[i].level_results.Count() - 1].location.latitude,
                                            //              (double)GSMBTS[i].level_results[GSMBTS[i].level_results.Count() - 1].location.longitude,
                                            //              (double)MainWindow.gps.LatitudeDecimal,
                                            //              (double)MainWindow.gps.LongitudeDecimal,
                                            //              out dist, out ang);
                                            //    }
                                            //    if ((GSMBTS[i].level_results.Count == 0 ||
                                            //        (decimal)dist > MainWindow.db_v2.Atdi_LevelResults_DistanceStep &&
                                            //        GSMBTS[i].level_results[GSMBTS[i].level_results.Count - 1].level_dbm != GSMBTS[i].Power) ||
                                            //        new TimeSpan(MainWindow.gps.LocalTime.Ticks - GSMBTS[i].level_results[GSMBTS[i].level_results.Count() - 1].measurement_time.Ticks) > MainWindow.db_v2.Atdi_LevelsMeasurementsCar_TimeStep)
                                            //    {
                                            //        #region
                                            //        DB.localatdi_level_meas_result l = new DB.localatdi_level_meas_result()
                                            //        {
                                            //            difference_time_stamp_ns = (decimal)Equipment.GSMBTS[i].TimeOfSlotInSec,
                                            //            level_dbm = Equipment.GSMBTS[i].Power,
                                            //            //level_dbmkvm = 0,
                                            //            measurement_time = Equipment.GSMBTS[i].LastLevelUpdete,
                                            //            saved_in_db = false,
                                            //            saved_in_result = false,
                                            //            location = new DB.localatdi_geo_location()
                                            //            {
                                            //                //agl = 0,
                                            //                asl = MainWindow.gps.Altitude,
                                            //                latitude = (double)MainWindow.gps.LatitudeDecimal,
                                            //                longitude = (double)MainWindow.gps.LongitudeDecimal,
                                            //            }
                                            //        };
                                            //        GSMBTS[i].LR_NewDataToSave = true;
                                            //        GUIThreadDispatcher.Instance.Invoke(() =>
                                            //        {
                                            //            GSMBTS[i].level_results.Add(l);
                                            //        });

                                            //        #endregion
                                            //    }
                                            //}
                                            #endregion
                                        }
                                    }
                                    if (find == false)
                                    {
                                        GSM_Channel t = GetGSMCHfromFreqDN(GSMUniFreq[(int)pr.dwFrequencyIndex]);
                                        GSMBTSData dt = new GSMBTSData()
                                        {
                                            FreqIndex = pr.dwFrequencyIndex,
                                            ARFCN = t.ARFCN,
                                            FreqDn = t.FreqDn,
                                            FreqUp = t.FreqUp,
                                            StandartSubband = t.StandartSubband,
                                            Power = ((double)pr.sPowerInDBm100) * 0.01,
                                            BSIC = -1,
                                            IndSCHInfo = pr.dwIndicatorOfSCHInfo,
                                            TimeOfSlotInSec = pr.dTimeOfSlotInSec - pData.u64DeviceTimeInNs / 1000000000,
                                            LastLevelUpdete = _timeService.GetGnssTime().Ticks
                                        };
                                        GSMBTS.Add(dt);
                                    }
                                }
                                #endregion
                            }
                        }

                        //MCC MNC LAC CID
                        if (pData.ListCellIdentResults.Count > 0)
                        {
                            for (int i = 0; i < pData.ListCellIdentResults.Count; i++)
                            {
                                #region
                                bool find = false;
                                for (int ii = 0; ii < GSMBTS.Count; ii++)
                                {
                                    if (GSMBTS[ii].FreqIndex == pData.ListCellIdentResults[i].dwFrequencyIndex && GSMBTS[ii].BSIC == Convert.ToInt32(Convert.ToString(pData.ListSCHInfoResults[i].wBSIC, 8)) /*&& GSMBTSfromDev[ii].GCID == ""*/)
                                    {
                                        if (GSMBTS[ii].MCC != Convert.ToInt32(pData.ListCellIdentResults[i].wMCC.ToString("X")) ||
                                            GSMBTS[ii].MNC != Convert.ToInt32(pData.ListCellIdentResults[i].wMNC.ToString("X")) ||
                                            GSMBTS[ii].LAC != pData.ListCellIdentResults[i].wLAC ||
                                            GSMBTS[ii].CID != pData.ListCellIdentResults[i].wCI)
                                        {
                                            GSMBTS[ii].MNC = Convert.ToInt32(pData.ListCellIdentResults[i].wMNC.ToString("X"));
                                            GSMBTS[ii].MCC = Convert.ToInt32(pData.ListCellIdentResults[i].wMCC.ToString("X"));
                                            GSMBTS[ii].LAC = pData.ListCellIdentResults[i].wLAC;// string.Format("{0:00000}", pData.ListCellIdentResults[i].wLAC);//.ToString();
                                            GSMBTS[ii].CID = pData.ListCellIdentResults[i].wCI;//string.Format("{0:00000}", pData.ListCellIdentResults[i].wCI);//.ToString();

                                            bool FullData = (GSMBTS[ii].MNC.ToString() != "" && GSMBTS[ii].MCC.ToString() != "" && GSMBTS[ii].LAC.ToString() != "" && GSMBTS[ii].CID.ToString() != "");
                                            if (FullData == true && GSMBTS[ii].CID > -1)
                                            {
                                                GSMBTS[ii].GCID = pData.ListCellIdentResults[i].wMCC.ToString("X") + " " +
                                                   pData.ListCellIdentResults[i].wMNC.ToString("X") + " " +
                                                   string.Format("{0:00000}", pData.ListCellIdentResults[i].wLAC) + " " +//.ToString() + " " +
                                                   string.Format("{0:00000}", pData.ListCellIdentResults[i].wCI);//.ToString();
                                                #region

                                                #endregion
                                                GSMBTS[ii].FullData = FullData;
                                            }
                                        }
                                        find = true;
                                    }
                                }
                                if (find == false)
                                {
                                    GSM_Channel t = GetGSMCHfromFreqDN(GSMUniFreq[(int)pData.ListCellIdentResults[i].dwFrequencyIndex]);
                                    GSMBTSData dt = new GSMBTSData()
                                    {
                                        MCC = Convert.ToInt32(pData.ListCellIdentResults[i].wMCC.ToString("X")),
                                        MNC = Convert.ToInt32(pData.ListCellIdentResults[i].wMNC.ToString("X")),
                                        LAC = pData.ListCellIdentResults[i].wLAC,//string.Format("{0:00000}", pData.ListCellIdentResults[i].wLAC),// pData.ListCellIdentResults[i].wLAC.ToString(),
                                        CID = pData.ListCellIdentResults[i].wCI,//string.Format("{0:00000}", pData.ListCellIdentResults[i].wCI),//.ToString(),

                                        FullData = (pData.ListCellIdentResults[i].wMNC.ToString("X") != "" &&
                                        pData.ListCellIdentResults[i].wMCC.ToString("X") != "" &&
                                        pData.ListCellIdentResults[i].wLAC.ToString() != "" &&
                                        pData.ListCellIdentResults[i].wCI.ToString() != ""),
                                        GCID = pData.ListCellIdentResults[i].wMCC.ToString("X") + " " +
                                        pData.ListCellIdentResults[i].wMNC.ToString("X") + " " +
                                        string.Format("{0:00000}", pData.ListCellIdentResults[i].wLAC) + " " + // .ToString()+ " " + 
                                        string.Format("{0:00000}", pData.ListCellIdentResults[i].wCI),//.ToString(),
                                                                                                      //TimeOfSlotInSecssch = pData.ListCellIdentResults[i].dwIndicator,3
                                        IndSCHInfo = pData.ListCellIdentResults[i].dwIndicator,
                                        FreqIndex = pData.ListCellIdentResults[i].dwFrequencyIndex,
                                        ARFCN = t.ARFCN,
                                        FreqDn = t.FreqDn,
                                        FreqUp = t.FreqUp,
                                        StandartSubband = t.StandartSubband,
                                    };

                                    GSMBTS.Add(dt);
                                }
                                #endregion
                            }
                        }
                        #region Sib
                        if (pData.pDemodResult != null && pData.pDemodResult.pbBitStream.Length > 0)
                        {
                            try
                            {
                                RohdeSchwarz.ViCom.Net.GSM.SL3DecoderRequest dec = new RohdeSchwarz.ViCom.Net.GSM.SL3DecoderRequest()
                                {
                                    dwBitCount = pData.pDemodResult.dwBitCount,
                                    pbBitStream = pData.pDemodResult.pbBitStream
                                };
                                RohdeSchwarz.ViCom.Net.GSM.SL3DecoderResult dr = new RohdeSchwarz.ViCom.Net.GSM.SL3DecoderResult();
                                dr = gsmInterface.RetrieveTextForPDU(dec, SDefs.dwDefaultTimeOutInMs);
                                if (pData.pDemodResult.pbBitStream.Length > 0)
                                {
                                    for (int i = 0; i < GSMBTS.Count; i++)
                                    {
                                        if (pData.pDemodResult.dwFrequencyIndex == GSMBTS[i].FreqIndex && pData.pDemodResult.dwIndicatorOfSCHInfo == GSMBTS[i].IndFirstSCHInfo)
                                        {
                                            #region save sib data
                                            List<COMRMSI.SystemInformationBlock> ibs = new List<COMRMSI.SystemInformationBlock>() { };
                                            bool fib = false;
                                            if (GSMBTS[i].SysInfoBlocks != null)
                                            {
                                                ibs = new List<COMRMSI.SystemInformationBlock>(GSMBTS[i].SysInfoBlocks);
                                                for (int ib = 0; ib < ibs.Count(); ib++)
                                                {
                                                    if (ibs[ib].Type == pData.pDemodResult.ePDU.ToString())
                                                    {
                                                        fib = true;
                                                        ibs[ib].DataString = dr.pcPduText;
                                                    }
                                                }
                                            }

                                            if (fib == false)
                                            {
                                                COMRMSI.SystemInformationBlock sib = new COMRMSI.SystemInformationBlock()
                                                {
                                                    DataString = dr.pcPduText,
                                                    Type = pData.pDemodResult.ePDU.ToString()
                                                };
                                                ibs.Add(sib);
                                                GSMBTS[i].SysInfoBlocks = ibs.ToArray();
                                            }
                                            #endregion
                                        }
                                    }
                                }
                            }
                            catch { }
                        }
                        #endregion
                    }
                    #region Exception
                    catch (RohdeSchwarz.ViCom.Net.CViComError error)
                    {
                        _logger.Exception(Contexts.ThisComponent, new Exception("ErrorCode:" + error.ErrorCode + " ErrorString:" + error.ErrorString));
                    }
                    catch (Exception exp)
                    {
                        _logger.Exception(Contexts.ThisComponent, exp);
                    }
                    #endregion
                }
                Console.WriteLine(GSMBTS.Count);
            }
        }
        private static GSM_Channel GetGSMCHfromFreqDN(decimal freq_Dn)
        {
            bool find = false;
            freq_Dn = freq_Dn / 1000000;
            GSM_Channel temp = new GSM_Channel();
            if (find == false && freq_Dn >= 935.2m && freq_Dn <= 959.8m)
            {
                #region
                List<decimal> tf = new List<decimal>() { };
                for (int i = 1; i <= 124; i++)
                {
                    tf.Add(935.2m + 0.2m * (i - 1));
                }
                for (int i = 0; i < tf.Count; i++)
                {
                    if (tf[i] == freq_Dn)
                    {
                        find = true;
                        temp.FreqDn = freq_Dn;
                        temp.ARFCN = (int)((temp.FreqDn - 935.2m) * 5 + 1);
                        temp.FreqUp = freq_Dn - 45;
                        temp.StandartSubband = "P-GSM900";
                    }
                }
                #endregion
            }
            else if (find == false && freq_Dn >= 925.2m && freq_Dn <= 959.8m)
            {
                #region
                List<decimal> tf = new List<decimal>() { };
                for (int i = 975; i <= 1023; i++)
                {
                    tf.Add(925.2m + 0.2m * (i - 975));
                }
                for (int i = 0; i < tf.Count; i++)
                {
                    if (tf[i] == freq_Dn)
                    {
                        find = true;
                        temp.FreqDn = freq_Dn;
                        temp.ARFCN = (int)((temp.FreqDn - 925.2m) * 5 + 975);
                        temp.FreqUp = freq_Dn - 45;
                        temp.StandartSubband = "E-GSM900";
                    }
                }
                if (find == false)
                {
                    tf.Clear();
                    for (int i = 0; i <= 124; i++)
                    {
                        tf.Add(935 + 0.2m * (i - 0));
                    }
                    for (int i = 0; i < tf.Count; i++)
                    {
                        if (tf[i] == freq_Dn)
                        {
                            find = true;
                            temp.FreqDn = freq_Dn;
                            temp.ARFCN = (int)((temp.FreqDn - 935) * 5 + 0);
                            temp.FreqUp = freq_Dn - 45;
                            temp.StandartSubband = "E-GSM900";
                        }
                    }
                }
                #endregion
            }
            else if (find == false && freq_Dn >= 921.2m && freq_Dn <= 959.8m)
            {
                #region
                List<decimal> tf = new List<decimal>() { };
                for (int i = 955; i <= 1023; i++)
                {
                    tf.Add(921.2m + 0.2m * (i - 955));
                }
                for (int i = 0; i < tf.Count; i++)
                {
                    if (tf[i] == freq_Dn)
                    {
                        find = true;
                        temp.FreqDn = freq_Dn;
                        temp.ARFCN = (int)((temp.FreqDn - 921.2m) * 5 + 955);
                        temp.FreqUp = freq_Dn - 45;
                        temp.StandartSubband = "R-GSM900";
                    }
                }
                if (find == false)
                {
                    tf.Clear();
                    for (int i = 0; i <= 124; i++)
                    {
                        tf.Add(935 + 0.2m * (i - 0));
                    }
                    for (int i = 0; i < tf.Count; i++)
                    {
                        if (tf[i] == freq_Dn)
                        {
                            find = true;
                            temp.FreqDn = freq_Dn;
                            temp.ARFCN = (int)((temp.FreqDn - 935) * 5 + 0);
                            temp.FreqUp = freq_Dn - 45;
                            temp.StandartSubband = "R-GSM900";
                        }
                    }
                }
                #endregion
            }
            else if (find == false && freq_Dn >= 918.2m && freq_Dn <= 959.8m)
            {
                #region
                List<decimal> tf = new List<decimal>() { };
                for (int i = 940; i <= 1023; i++)
                {
                    tf.Add(918.2m + 0.2m * (i - 940));
                }
                for (int i = 0; i < tf.Count; i++)
                {
                    if (tf[i] == freq_Dn)
                    {
                        find = true;
                        temp.FreqDn = freq_Dn;
                        temp.ARFCN = (int)((temp.FreqDn - 918.2m) * 5 + 940);
                        temp.FreqUp = freq_Dn - 45;
                        temp.StandartSubband = "R-GSM900";
                    }
                }
                if (find == false)
                {
                    tf.Clear();
                    for (int i = 0; i <= 124; i++)
                    {
                        tf.Add(935 + 0.2m * (i - 0));
                    }
                    for (int i = 0; i < tf.Count; i++)
                    {
                        if (tf[i] == freq_Dn)
                        {
                            find = true;
                            temp.FreqDn = freq_Dn;
                            temp.ARFCN = (int)((temp.FreqDn - 935) * 5 + 0);
                            temp.FreqUp = freq_Dn - 45;
                            temp.StandartSubband = "R-GSM900";
                        }
                    }
                }
                #endregion
            }
            else if (find == false && freq_Dn >= 1805.2m && freq_Dn <= 1879.8m)
            {
                #region
                List<decimal> tf = new List<decimal>() { };
                for (int i = 512; i <= 885; i++)
                {
                    tf.Add(1805.2m + 0.2m * (i - 512));
                }
                for (int i = 0; i < tf.Count; i++)
                {
                    if (tf[i] == freq_Dn)
                    {
                        find = true;
                        temp.FreqDn = freq_Dn;
                        temp.ARFCN = (int)((temp.FreqDn - 1805.2m) * 5 + 512);
                        temp.FreqUp = freq_Dn - 95;
                        temp.StandartSubband = "GSM1800";
                    }
                }
                #endregion
            }
            else if (find == false)
                throw new Exception("Частота " + freq_Dn + " МГц не соответствует стандартным сеткам частот!");
            temp.FreqDn = temp.FreqDn * 1000000;
            temp.FreqUp = temp.FreqUp * 1000000;
            return temp;
        }


        #endregion DataProcessors


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
                //    AttMax_dB = (int)UniqueData.AttMax,
                //    AttMin_dB = 0,
                //    FreqMax_Hz = UniqueData.FreqMax,
                //    FreqMin_Hz = UniqueData.FreqMin,
                //    PreAmpMax_dB = 1, //типа включен/выключен, сколько по факту усиливает нигде не пишется кроме FSW где их два 15/30 и то два это опция
                //    PreAmpMin_dB = 0,
                //    RefLevelMax_dBm = (int)UniqueData.RefLevelMax,
                //    RefLevelMin_dBm = (int)UniqueData.RefLevelMin,
                //    EquipmentInfo = new EquipmentInfo()
                //    {
                //        AntennaCode = config.AdapterEquipmentInfo.AntennaSN,// "Omni",//S/N  В конфиг
                //        AntennaManufacturer = config.AdapterEquipmentInfo.AntennaManufacturer,//"3anet",//В конфиг
                //        AntennaName = config.AdapterEquipmentInfo.AntennaName,//"BC600",//В конфиг
                //        EquipmentManufacturer = new Atdi.DataModels.Sdrn.DeviceServer.Adapters.InstrManufacrures().RuS.UI,
                //        EquipmentName = UniqueData.InstrModel,
                //        EquipmentFamily = "SpectrumAnalyzer",//SDR/SpecAn/MonRec
                //        EquipmentCode = UniqueData.SerialNumber,//S/N

                //    },
                //    RadioPathParameters = rrps
            };
            //if (UniqueData.PreAmp)
            //{
            //    sdp.PreAmpMax_dB = 1;
            //}
            //else
            //{
            //    sdp.PreAmpMax_dB = 0;
            //}
            MesureTraceDeviceProperties mtdp = new MesureTraceDeviceProperties()
            {
                //RBWMax_Hz = (double)UniqueData.RBWArr[UniqueData.RBWArr.Length - 1],
                //RBWMin_Hz = (double)UniqueData.RBWArr[0],
                //SweepTimeMin_s = (double)UniqueData.SWTMin,
                //SweepTimeMax_s = (double)UniqueData.SWTMax,
                //StandardDeviceProperties = sdp,
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
