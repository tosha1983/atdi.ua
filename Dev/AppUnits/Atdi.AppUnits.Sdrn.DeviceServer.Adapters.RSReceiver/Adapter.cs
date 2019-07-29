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
//            try
//            {
//                //Ахахахаааа
//                //грохнем процесс рудика он же romes он же vicom, а вдруг лапки
//                System.Diagnostics.Process[] ps2 = System.Diagnostics.Process.GetProcessesByName("RuSWorkerDllLoaderPhysicalLayer"); //Имя процесса
//                foreach (System.Diagnostics.Process p1 in ps2)
//                {
//                    p1.Kill();
//                }
//            }
//            catch (Exception e)
//            {
//                _logger.Exception(Contexts.ThisComponent, e);
//            }
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
//                    MesureSysInfoDeviceProperties msidp = GetProperties(MainConfig);
//                    host.RegisterHandler<COM.MesureSystemInfoCommand, COMR.MesureSystemInfoResult>(MesureSystemInfoHandler, msidp);
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
//                GpsDisconnect();
//                if (GSMIsRuning)
//                {
//                    GsmDisconnect();
//                }
//                if (CDMAIsRuning)
//                {
//                    CDMADisconnect();
//                }
//                if (UMTSIsRuning)
//                {
//                    UmtsDisconnect();
//                }
//                if (LTEIsRuning)
//                {
//                    LTEDisconnect();
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
//                InstrManufacrure = 1, InstrModel = "EM100", InstrOption = new List<DeviceOption>()
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
//                FFMStepBW = new decimal[] { 0.625m, 1.25m, 3.125m, 6.25m, 12.5m, 31.25m, 62.5m, 125, 312.5m, 625, 1250, 3125, 6250},
//                DFBW = new decimal[] { 50, 100, 150, 300, 600, 1000, 1500, 2100, 2400, 2700, 3100, 4000, 4800, 6000, 8333, 9000, 12000, 15000, 25000, 30000, 50000, 75000, 120000, 150000, 250000, 300000, 500000, 800000, 1000000, 1250000, 1500000, 2000000 },
//                PSCANStepBW = new decimal[] { 125, 250, 500, 625, 1250, 2500, 3125, 6250, 12500m, 25000m, 50000, 100000},
//                FFMSpanBW = new decimal[] { 1000, 2000, 5000, 10000, 20000, 50000, 100000, 200000, 500000, 1000000, 2000000, 5000000, 10000000},
//                FFTModes = new List<ParamWithUI>
//                {
//                    new ParamWithUI() {UI = "MINIMUM", Parameter = "MIN"},
//                    new ParamWithUI() {UI = "MAXIMUM", Parameter = "MAX"},
//                    new ParamWithUI() {UI = "AVERAGE", Parameter = "SCAL"},
//                    new ParamWithUI() {UI = "CLEAR WRITE", Parameter = "OFF"},
//                },
//                SelectivityChangeable = false,
//                SelectivityModes = new List<ParamWithUI>()
//                {
//                    new ParamWithUI() {UI = "AUTO", Parameter = "AUTO"},
//                    new ParamWithUI() {UI = "NORMAL", Parameter = "NORM"},
//                    new ParamWithUI() {UI = "NARROW", Parameter = "NARR"},
//                    new ParamWithUI() {UI = "SHRP", Parameter = "SHAR"},
//                },
//                Detectors = new List<ParamWithUI>()
//                {
//                    new ParamWithUI() {UI = "AVG", Parameter = "AVG"},
//                    new ParamWithUI() {UI = "SAMPLE", Parameter = "FAST"},
//                    new ParamWithUI() {UI = "PEAK", Parameter = "PEAK"},
//                    new ParamWithUI() {UI = "RMS", Parameter = "RMS"},
//                },
//                RFModeChangeable = false,
//                RFModes= new List<ParamWithUI>(){ },
//                ATTFix = true,
//                AttMax = 10,
//                AttStep = 10,

//                RefLevelMAX = 110,
//                RefLevelMIN = -29,
//                RangeLevelMAX = 140,
//                RangeLevelMIN = 10,
//                RangeLevelStep = 10,
//                DFSQUModes = new List<ParamWithUI>()
//                {
//                    new ParamWithUI() {UI = "OFF", Parameter = "OFF"},
//                    new ParamWithUI() {UI = "GATE", Parameter = "GATE"},
//                    new ParamWithUI() {UI = "NORM", Parameter = "NORM"},
//                },
//            },
//            #endregion
//            #region PR100
//            new LocalRSReceiverInfo
//            {
//                InstrManufacrure = 1, InstrModel = "PR100", InstrOption = new List<DeviceOption>()
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
//                FFMStepBW = new decimal[] { 0.625m, 1.25m, 3.125m, 6.25m, 12.5m, 31.25m, 62.5m, 125, 312.5m, 625, 1250, 3125, 6250},
//                DFBW = new decimal[] { 50, 100, 150, 300, 600, 1000, 1500, 2100, 2400, 2700, 3100, 4000, 4800, 6000, 8333, 9000, 12000, 15000, 25000, 30000, 50000, 75000, 120000, 150000, 250000, 300000, 500000, 800000, 1000000, 1250000, 1500000, 2000000 },
//                PSCANStepBW = new decimal[] { 125, 250, 500, 625, 1250, 2500, 3125, 6250, 12500m, 25000m, 50000, 100000},
//                FFMSpanBW = new decimal[] { 1000, 2000, 5000, 10000, 20000, 50000, 100000, 200000, 500000, 1000000, 2000000, 5000000, 10000000},
//                FFTModes = new List<ParamWithUI>
//                {
//                    new ParamWithUI() {UI = "MINIMUM", Parameter = "MIN"},
//                    new ParamWithUI() {UI = "MAXIMUM", Parameter = "MAX"},
//                    new ParamWithUI() {UI = "AVERAGE", Parameter = "SCAL"},
//                    new ParamWithUI() {UI = "CLEAR WRITE", Parameter = "OFF"},
//                },
//                SelectivityChangeable = false,
//                SelectivityModes = new List<ParamWithUI>()
//                {
//                    new ParamWithUI() {UI = "AUTO", Parameter = "AUTO"},
//                    new ParamWithUI() {UI = "NORMAL", Parameter = "NORM"},
//                    new ParamWithUI() {UI = "NARROW", Parameter = "NARR"},
//                    new ParamWithUI() {UI = "SHRP", Parameter = "SHAR"},
//                },
//                Detectors = new List<ParamWithUI>()
//                {
//                    new ParamWithUI() {UI = "AVG", Parameter = "AVG"},
//                    new ParamWithUI() {UI = "SAMPLE", Parameter = "FAST"},
//                    new ParamWithUI() {UI = "PEAK", Parameter = "PEAK"},
//                    new ParamWithUI() {UI = "RMS", Parameter = "RMS"},
//                },
//                RFModeChangeable = false,
//                RFModes= new List<ParamWithUI>(){ },
//                ATTFix = true,
//                AttMax = 10,
//                AttStep = 10,

//                RefLevelMAX = 110,
//                RefLevelMIN = -29,
//                RangeLevelMAX = 140,
//                RangeLevelMIN = 10,
//                RangeLevelStep = 10,
//                DFSQUModes = new List<ParamWithUI>()
//                {
//                    new ParamWithUI() {UI = "OFF", Parameter = "OFF"},
//                    new ParamWithUI() {UI = "GATE", Parameter = "GATE"},
//                    new ParamWithUI() {UI = "NORM", Parameter = "NORM"},
//                },
//            },
//            #endregion
//            #region DDF007
//            new LocalRSReceiverInfo
//            {
//                InstrManufacrure = 1, InstrModel = "DDF007", InstrOption = new List<DeviceOption>()
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
//                FFMStepBW = new decimal[] { 0.625m, 1.25m, 3.125m, 6.25m, 12.5m, 31.25m, 62.5m, 125, 312.5m, 625, 1250, 3125, 6250},
//                DFBW = new decimal[] { 50, 100, 150, 300, 600, 1000, 1500, 2100, 2400, 2700, 3100, 4000, 4800, 6000, 8333, 9000, 12000, 15000, 25000, 30000, 50000, 75000, 120000, 150000, 250000, 300000, 500000, 800000, 1000000, 1250000, 1500000, 2000000 },
//                PSCANStepBW = new decimal[] { 125, 250, 500, 625, 1250, 2500, 3125, 6250, 12500m, 25000m, 50000, 100000},
//                FFMSpanBW = new decimal[] { 1000, 2000, 5000, 10000, 20000, 50000, 100000, 200000, 500000, 1000000, 2000000, 5000000, 10000000},
//                FFTModes = new List<ParamWithUI>
//                {
//                    new ParamWithUI() {UI = "MINIMUM", Parameter = "MIN"},
//                    new ParamWithUI() {UI = "MAXIMUM", Parameter = "MAX"},
//                    new ParamWithUI() {UI = "AVERAGE", Parameter = "SCAL"},
//                    new ParamWithUI() {UI = "CLEAR WRITE", Parameter = "OFF"},
//                },
//                SelectivityChangeable = false,
//                SelectivityModes = new List<ParamWithUI>()
//                {
//                    new ParamWithUI() {UI = "AUTO", Parameter = "AUTO"},
//                    new ParamWithUI() {UI = "NORMAL", Parameter = "NORM"},
//                    new ParamWithUI() {UI = "NARROW", Parameter = "NARR"},
//                    new ParamWithUI() {UI = "SHRP", Parameter = "SHAR"},
//                },
//                Detectors = new List<ParamWithUI>()
//                {
//                    new ParamWithUI() {UI = "AVG", Parameter = "AVG"},
//                    new ParamWithUI() {UI = "SAMPLE", Parameter = "FAST"},
//                    new ParamWithUI() {UI = "PEAK", Parameter = "PEAK"},
//                    new ParamWithUI() {UI = "RMS", Parameter = "RMS"},
//                },
//                RFModeChangeable = false,
//                RFModes= new List<ParamWithUI>(){ },
//                ATTFix = true,
//                AttMax = 10,
//                AttStep = 10,

//                RefLevelMAX = 110,
//                RefLevelMIN = -29,
//                RangeLevelMAX = 140,
//                RangeLevelMIN = 10,
//                RangeLevelStep = 10,
//                DFSQUModes = new List<ParamWithUI>()
//                {
//                    new ParamWithUI() {UI = "OFF", Parameter = "OFF"},
//                    new ParamWithUI() {UI = "GATE", Parameter = "GATE"},
//                    new ParamWithUI() {UI = "NORM", Parameter = "NORM"},
//                },
//            },
//            #endregion
//            #region ESMD
//            new LocalRSReceiverInfo
//            {
//                InstrManufacrure = 1, InstrModel = "ESMD", InstrOption = new List<DeviceOption>()
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
//                FFMStepBW = new decimal[] { 0.625m, 1.25m, 2.5m, 3.125m, 6.25m, 12.5m, 25, 31.25m, 50, 62.5m, 100, 125, 200, 250, 312.5m, 500, 625, 1000, 1250, 2000, 2500, 3125, 5000, 6250, 8333, 10000, 12500, 20000, 25000, 50000, 100000, 200000, 500000, 1000000, 2000000 },
//                DFBW = new decimal[] { 50, 100, 150, 300, 600, 1000, 1500, 2100, 2400, 2700, 3100, 4000, 4800, 6000, 8333, 9000, 12000, 15000, 25000, 30000, 50000, 75000, 120000, 150000, 250000, 300000, 500000, 800000, 1000000, 1250000, 1500000, 2000000 },
//                PSCANStepBW = new decimal[] { 0.625m, 1.25m, 2.5m, 3.125m, 6.25m, 12.5m, 25, 31.25m, 50, 62.5m, 100, 125, 200, 250, 312.5m, 500, 625, 1000, 1250, 2000, 2500, 3125, 5000, 6250, 8333, 10000, 12500, 20000, 25000, 50000, 100000, 200000, 500000, 1000000, 2000000 },
//                FFMSpanBW = new decimal[] { 1000, 2000, 5000, 10000, 20000, 50000, 100000, 200000, 500000, 1000000, 2000000, 5000000, 10000000, 20000000, 40000000, 80000000 },
//                FFTModes = new List<ParamWithUI>
//                {
//                    new ParamWithUI() {UI = "MINIMUM", Parameter = "MIN"},
//                    new ParamWithUI() {UI = "MAXIMUM", Parameter = "MAX"},
//                    new ParamWithUI() {UI = "AVERAGE", Parameter = "SCAL"},
//                    new ParamWithUI() {UI = "CLEAR WRITE", Parameter = "OFF"},
//                },
//                SelectivityChangeable = true,
//                SelectivityModes = new List<ParamWithUI>()
//                {
//                    new ParamWithUI() {UI = "AUTO", Parameter = "AUTO"},
//                    new ParamWithUI() {UI = "NORMAL", Parameter = "NORM"},
//                    new ParamWithUI() {UI = "NARROW", Parameter = "NARR"},
//                    new ParamWithUI() {UI = "SHRP", Parameter = "SHAR"},
//                },
//                Detectors = new List<ParamWithUI>()
//                {
//                    new ParamWithUI() {UI = "AVG", Parameter = "PAV"},
//                    new ParamWithUI() {UI = "PEAK", Parameter = "POS"},
//                    new ParamWithUI() {UI = "FAST", Parameter = "FAST"},
//                    new ParamWithUI() {UI = "RMS", Parameter = "RMS"},
//                },
//                RFModeChangeable = true,
//                RFModes= new List<ParamWithUI>()
//                {
//                    new ParamWithUI() {UI = "NORMAL", Parameter = "NORM"},
//                    new ParamWithUI() {UI = "LOW NOISE", Parameter = "LOWN"},
//                    new ParamWithUI() {UI = "LOW DISTORTION", Parameter = "LOWD"},
//                },
//                ATTFix = false,
//                AttMax = 70,
//                AttStep = 1,

//                RefLevelMAX = 130,
//                RefLevelMIN = -29,
//                RangeLevelMAX = 200,
//                RangeLevelMIN = 10,
//                RangeLevelStep = 1,
//                DFSQUModes = new List<ParamWithUI>()
//                {
//                    new ParamWithUI() {UI = "OFF", Parameter = "OFF"},
//                    new ParamWithUI() {UI = "GATE", Parameter = "GATE"},
//                    new ParamWithUI() {UI = "NORM", Parameter = "NORM"},
//                },
//            },
//            #endregion
//            #region DDF2
//            new LocalRSReceiverInfo
//            {
//                InstrManufacrure = 1, InstrModel = "DDF2", InstrOption = new List<DeviceOption>()
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
//                FFMStepBW = new decimal[] { 0.625m, 1.25m, 2.5m, 3.125m, 6.25m, 12.5m, 25, 31.25m, 50, 62.5m, 100, 125, 200, 250, 312.5m, 500, 625, 1000, 1250, 2000, 2500, 3125, 5000, 6250, 8333, 10000, 12500, 20000, 25000, 50000, 100000, 200000, 500000, 1000000, 2000000 },
//                DFBW = new decimal[] { 50, 100, 150, 300, 600, 1000, 1500, 2100, 2400, 2700, 3100, 4000, 4800, 6000, 8333, 9000, 12000, 15000, 25000, 30000, 50000, 75000, 120000, 150000, 250000, 300000, 500000, 800000, 1000000, 1250000, 1500000, 2000000 },
//                PSCANStepBW = new decimal[] { 0.625m, 1.25m, 2.5m, 3.125m, 6.25m, 12.5m, 25, 31.25m, 50, 62.5m, 100, 125, 200, 250, 312.5m, 500, 625, 1000, 1250, 2000, 2500, 3125, 5000, 6250, 8333, 10000, 12500, 20000, 25000, 50000, 100000, 200000, 500000, 1000000, 2000000 },
//                FFMSpanBW = new decimal[] { 1000, 2000, 5000, 10000, 20000, 50000, 100000, 200000, 500000, 1000000, 2000000, 5000000, 10000000, 20000000, 40000000, 80000000 },
//                FFTModes = new List<ParamWithUI>
//                {
//                    new ParamWithUI() {UI = "MINIMUM", Parameter = "MIN"},
//                    new ParamWithUI() {UI = "MAXIMUM", Parameter = "MAX"},
//                    new ParamWithUI() {UI = "AVERAGE", Parameter = "SCAL"},
//                    new ParamWithUI() {UI = "CLEAR WRITE", Parameter = "OFF"},
//                },
//                SelectivityChangeable = true,
//                SelectivityModes = new List<ParamWithUI>()
//                {
//                    new ParamWithUI() {UI = "AUTO", Parameter = "AUTO"},
//                    new ParamWithUI() {UI = "NORMAL", Parameter = "NORM"},
//                    new ParamWithUI() {UI = "NARROW", Parameter = "NARR"},
//                    new ParamWithUI() {UI = "SHRP", Parameter = "SHAR"},
//                },
//                Detectors = new List<ParamWithUI>()
//                {
//                    new ParamWithUI() {UI = "AVG", Parameter = "PAV"},
//                    new ParamWithUI() {UI = "PEAK", Parameter = "POS"},
//                    new ParamWithUI() {UI = "FAST", Parameter = "FAST"},
//                    new ParamWithUI() {UI = "RMS", Parameter = "RMS"},
//                },
//                RFModeChangeable = true,
//                RFModes= new List<ParamWithUI>()
//                {
//                    new ParamWithUI() {UI = "NORMAL", Parameter = "NORM"},
//                    new ParamWithUI() {UI = "LOW NOISE", Parameter = "LOWN"},
//                    new ParamWithUI() {UI = "LOW DISTORTION", Parameter = "LOWD"},
//                },
//                ATTFix = false,
//                AttMax = 70,
//                AttStep = 1,

//                RefLevelMAX = 130,
//                RefLevelMIN = -29,
//                RangeLevelMAX = 200,
//                RangeLevelMIN = 10,
//                RangeLevelStep = 1,
//                DFSQUModes = new List<ParamWithUI>()
//                {
//                    new ParamWithUI() {UI = "OFF", Parameter = "OFF"},
//                    new ParamWithUI() {UI = "GATE", Parameter = "GATE"},
//                    new ParamWithUI() {UI = "NORM", Parameter = "NORM"},
//                },
//            },
//            #endregion
//            #region EB500
//            new LocalRSReceiverInfo
//            {
//                InstrManufacrure = 1, InstrModel = "EB500", InstrOption = new List<DeviceOption>()
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
//                FFMStepBW = new decimal[] { 0.625m, 1.25m, 2.5m, 3.125m, 6.25m, 12.5m, 25, 31.25m, 50, 62.5m, 100, 125, 200, 250, 312.5m, 500, 625, 1000, 1250, 2000, 2500, 3125, 5000, 6250, 8333, 10000, 12500, 20000, 25000, 50000, 100000, 200000, 500000, 1000000, 2000000 },
//                DFBW = new decimal[] { 50, 100, 150, 300, 600, 1000, 1500, 2100, 2400, 2700, 3100, 4000, 4800, 6000, 8333, 9000, 12000, 15000, 25000, 30000, 50000, 75000, 120000, 150000, 250000, 300000, 500000, 800000, 1000000, 1250000, 1500000, 2000000 },
//                PSCANStepBW = new decimal[] { 0.625m, 1.25m, 2.5m, 3.125m, 6.25m, 12.5m, 25, 31.25m, 50, 62.5m, 100, 125, 200, 250, 312.5m, 500, 625, 1000, 1250, 2000, 2500, 3125, 5000, 6250, 8333, 10000, 12500, 20000, 25000, 50000, 100000, 200000, 500000, 1000000, 2000000 },
//                FFMSpanBW = new decimal[] { 1000, 2000, 5000, 10000, 20000, 50000, 100000, 200000, 500000, 1000000, 2000000, 5000000, 10000000, 20000000 },
//                FFTModes = new List<ParamWithUI>
//                {
//                    new ParamWithUI() {UI = "MINIMUM", Parameter = "MIN"},
//                    new ParamWithUI() {UI = "MAXIMUM", Parameter = "MAX"},
//                    new ParamWithUI() {UI = "AVERAGE", Parameter = "SCAL"},
//                    new ParamWithUI() {UI = "CLEAR WRITE", Parameter = "OFF"},
//                },
//                SelectivityChangeable = true,
//                SelectivityModes = new List<ParamWithUI>()
//                {
//                    new ParamWithUI() {UI = "AUTO", Parameter = "AUTO"},
//                    new ParamWithUI() {UI = "NORMAL", Parameter = "NORM"},
//                    new ParamWithUI() {UI = "NARROW", Parameter = "NARR"},
//                    new ParamWithUI() {UI = "SHRP", Parameter = "SHAR"},
//                },
//                Detectors = new List<ParamWithUI>()
//                {
//                    new ParamWithUI() {UI = "AVG", Parameter = "PAV"},
//                    new ParamWithUI() {UI = "PEAK", Parameter = "POS"},
//                    new ParamWithUI() {UI = "FAST", Parameter = "FAST"},
//                    new ParamWithUI() {UI = "RMS", Parameter = "RMS"},
//                },
//                RFModeChangeable = true,
//                RFModes= new List<ParamWithUI>()
//                {
//                    new ParamWithUI() {UI = "NORMAL", Parameter = "NORM"},
//                    new ParamWithUI() {UI = "LOW NOISE", Parameter = "LOWN"},
//                    new ParamWithUI() {UI = "LOW DISTORTION", Parameter = "LOWD"},
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
//                DFSQUModes = new List<ParamWithUI>()
//                {
//                    new ParamWithUI() {UI = "OFF", Parameter = "OFF"},
//                    new ParamWithUI() {UI = "GATE", Parameter = "GATE"},
//                    new ParamWithUI() {UI = "NORM", Parameter = "NORM"},
//                },
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
//        #endregion
//        #endregion

//        public void Connect(string iPAddress, int tCPPort)
//        {
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
//                                //Console.WriteLine(ni.Name);
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
//                }
//                else
//                {
//                    IsRuning = false;

//                }
//            }
//            else
//            {
//                IsRuning = false;

//            }
//        }

//        private void SameWorkUDP()
//        {
//            try
//            {
//                Thread.Sleep(1);
//            }
//            catch { }
//        }
//        private void ReaderStream()
//        {
//            Thread.Sleep(new TimeSpan(10));
//            if (uc.IsOpen && _DataCycle)
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


//                    if (streammode == 101)//FFM IFPan
//                    {
//                        #region
//                        //byte[] bfreq = new byte[8]; Array.Copy(t, 28, bfreq, 0, 4); Array.Copy(t, 44, bfreq, 4, 4);
//                        //UInt64 freqCentr = BitConverter.ToUInt64(bfreq, 0);
//                        //UInt32 freqSpan = BitConverter.ToUInt32(t, 32);
//                        //decimal freqstart = freqCentr - (freqSpan / 2);
//                        //decimal freqstop = freqCentr + (freqSpan / 2);
//                        //decimal step = freqSpan / ((Int16)((t[20] << 8) | (t[21])) - 1);
//                        //for (int i = 0; i < UniqueData.FFMStepBW.Length; i++)
//                        //{
//                        //    if (step == (int)UniqueData.FFMStepBW[i])
//                        //    { step = UniqueData.FFMStepBW[i]; }
//                        //}

//                        ////TraceTemp = new tracepoint[traceDataLength];

//                        ////FFMStrengthLevel = Math.Round(LM.MeasChannelPower(temp, freqCentr, DemodBW) + 107, 2); //(decimal)((Int16)((t[91] << 8) | (t[90])));
//                        ////Trace1 = temp;
//                        //if (FFMFreqCentr != freqCentr || FreqCentr != freqCentr) { FFMFreqCentr = freqCentr; FreqCentr = freqCentr; }
//                        //if (FFMFreqSpan != freqSpan || FreqSpan != freqSpan)
//                        //{
//                        //    if (System.Array.IndexOf(UniqueData.FFMSpanBW, freqSpan) > 0)
//                        //    { FFMFreqSpanInd = System.Array.IndexOf(UniqueData.FFMSpanBW, freqSpan); FreqSpan = freqSpan; }
//                        //}
//                        //if (UniqueData.FFMStepBW[FFMStepInd] != step) { FFMStepInd = System.Array.IndexOf(UniqueData.FFMStepBW, step); }
//                        //SetTraceData(TraceTemp, step, true);
//                        //IsRuningUDP = true;
//                        #endregion
//                    }
//                    else if (streammode == 201)//FFM IFPan
//                    {
//                        #region
//                        //byte[] bfreq = new byte[8]; Array.Copy(t, 28, bfreq, 0, 4); Array.Copy(t, 44, bfreq, 4, 4);
//                        //UInt64 freqCentr = BitConverter.ToUInt64(bfreq, 0);
//                        //UInt32 freqSpan = BitConverter.ToUInt32(t, 32);
//                        //decimal freqstart = freqCentr - (freqSpan / 2);
//                        //decimal freqstop = freqCentr + (freqSpan / 2);
//                        //decimal step = freqSpan / ((Int16)((t[20] << 8) | (t[21])) - 1);
//                        //for (int i = 0; i < UniqueData.FFMStepBW.Length; i++)
//                        //{
//                        //    if (step == (int)UniqueData.FFMStepBW[i])
//                        //    { step = UniqueData.FFMStepBW[i]; }
//                        //}

//                        ////TraceTemp = new tracepoint[traceDataLength];

//                        ////FFMStrengthLevel = Math.Round(LM.MeasChannelPower(temp, freqCentr, DemodBW) + 107, 2); //(decimal)((Int16)((t[91] << 8) | (t[90])));
//                        ////Trace1 = temp;
//                        //if (FFMFreqCentr != freqCentr || FreqCentr != freqCentr) { FFMFreqCentr = freqCentr; FreqCentr = freqCentr; }
//                        //if (FFMFreqSpan != freqSpan || FreqSpan != freqSpan)
//                        //{
//                        //    if (System.Array.IndexOf(UniqueData.FFMSpanBW, freqSpan) > 0)
//                        //    { FFMFreqSpanInd = System.Array.IndexOf(UniqueData.FFMSpanBW, freqSpan); FreqSpan = freqSpan; }
//                        //}
//                        //if (UniqueData.FFMStepBW[FFMStepInd] != step) { FFMStepInd = System.Array.IndexOf(UniqueData.FFMStepBW, step); }
//                        //SetTraceData(TraceTemp, step, true);
//                        //IsRuningUDP = true;
//                        #endregion
//                    }
//                    else if (streammode == 401)//Audio
//                    {
//                        #region
//                        string str = Convert.ToBase64String(t);
//                        byte[] bytes = Convert.FromBase64String(str);
//                        string aud = "Audio";
//                        using (System.IO.StreamWriter file = new System.IO.StreamWriter(aud + ".txt", true))
//                        {
//                            file.WriteLine(Helpers.WinAPITime.GetTimeStamp() + ";");
//                            file.WriteLine(str + ";");
//                        }
//                        #endregion
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
//                        for (int i = 0; i < UniqueData.FFMStepBW.Length; i++)
//                        {
//                            if (step == (int)UniqueData.FFMStepBW[i])
//                            { step = UniqueData.FFMStepBW[i]; }
//                        }
//                        if (freqstart != TraceTemp[0].freq || TraceTemp.Length != traceDataLength || freqstop != TraceTemp[TraceTemp.Length - 1].freq || step != (TraceTemp[1].freq - TraceTemp[0].freq))
//                        {
//                            #region reset
//                            TraceTemp = new tracepoint[traceDataLength];
//                            if (LevelUnitInd == 0)
//                            {
//                                for (int i = 0; i < TraceTemp.Length; i++)
//                                {
//                                    tracepoint p = new tracepoint()
//                                    {
//                                        freq = freqstart + step * i,
//                                        level = (double)((Int16)((t[i * 2 + traceDataFrom + 1] << 8) | (t[i * 2 + traceDataFrom]))) / 10
//                                    };
//                                    TraceTemp[i] = p;
//                                }
//                            }
//                            else if (LevelUnitInd == 1)
//                            {
//                                for (int i = 0; i < traceDataLength; i++)
//                                {
//                                    tracepoint p = new tracepoint()
//                                    {
//                                        freq = freqstart + step * i,
//                                        level = ((double)((Int16)((t[i * 2 + traceDataFrom + 1] << 8) | (t[i * 2 + traceDataFrom]))) / 10) - 107
//                                    };
//                                    TraceTemp[i] = p;
//                                }
//                            }
//                            #endregion
//                        }
//                        else
//                        {
//                            #region without reset
//                            if (LevelUnitInd == 0)
//                            {

//                                for (int i = 0; i < TraceTemp.Length; i++)
//                                {
//                                    TraceTemp[i].level = (double)((Int16)((t[i * 2 + traceDataFrom + 1] << 8) | (t[i * 2 + traceDataFrom]))) / 10;
//                                }
//                            }
//                            else if (LevelUnitInd == 1)
//                            {
//                                for (int i = 0; i < TraceTemp.Length; i++)
//                                {
//                                    TraceTemp[i].level = ((double)((Int16)((t[i * 2 + traceDataFrom + 1] << 8) | (t[i * 2 + traceDataFrom]))) / 10) - 107;
//                                }
//                            }
//                            #endregion
//                        }
//                        //TraceTemp = new tracepoint[traceDataLength];

//                        //FFMStrengthLevel = Math.Round(LM.MeasChannelPower(temp, freqCentr, DemodBW) + 107, 2); //(decimal)((Int16)((t[91] << 8) | (t[90])));
//                        //Trace1 = temp;
//                        if (FFMFreqCentr != freqCentr || FreqCentr != freqCentr) { FFMFreqCentr = freqCentr; FreqCentr = freqCentr; }
//                        if (FFMFreqSpan != freqSpan || FreqSpan != freqSpan)
//                        {
//                            if (System.Array.IndexOf(UniqueData.FFMSpanBW, freqSpan) > 0)
//                            { FFMFreqSpanInd = System.Array.IndexOf(UniqueData.FFMSpanBW, freqSpan); FreqSpan = freqSpan; }
//                        }
//                        if (UniqueData.FFMStepBW[FFMStepInd] != step) { FFMStepInd = System.Array.IndexOf(UniqueData.FFMStepBW, step); }
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
//                        if (start != TraceTemp[0].freq || TraceTemp.Length != points || stop != TraceTemp[TraceTemp.Length - 1].freq || freqStep != (TraceTemp[1].freq - TraceTemp[0].freq))
//                        {
//                            TraceTemp = new tracepoint[points];
//                            for (int i = 0; i < points; i++)
//                            {
//                                tracepoint p = new tracepoint()
//                                {
//                                    freq = start + (UInt64)freqStep * (UInt64)i,
//                                    level = -1000
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
//                            if (TraceTemp[j].freq == freq) freqindex = j;
//                        }
//                        for (int i = 0; i < ind; i++)
//                        {
//                            if (freqindex < TraceTemp.Length)
//                            {
//                                if (LevelUnitInd == 0)
//                                {
//                                    TraceTemp[freqindex].level = (double)((Int16)((t[i * 2 + traceDataFrom + 1] << 8) | (t[i * 2 + traceDataFrom]))) / 10;
//                                }
//                                else if (LevelUnitInd == 1)
//                                {
//                                    TraceTemp[freqindex].level = (double)((Int16)((t[i * 2 + traceDataFrom + 1] << 8) | (t[i * 2 + traceDataFrom]))) / 10 - 107;
//                                }
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
//                            for (int i = 0; i < UniqueData.FFMStepBW.Length; i++)
//                            {
//                                if (step == (int)UniqueData.FFMStepBW[i])
//                                { step = UniqueData.FFMStepBW[i]; }
//                            }

//                            if (InstrModel == "EM100" || InstrModel == "PR100")
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
//                                //MainWindow.gps.UTCTime = dt;
//                                //MainWindow.gps.LocalTime = dt.ToLocalTime();


//                                ////decimal dfq = ((decimal)BitConverter.ToInt16(t, 136)) / 10;
//                                ////if (dfq < 0 || dfq > 100) DFQuality = 0;

//                                ////else if (dfq >= 0 && dfq <= 100) DFQuality = dfq;
//                                ////if (dfq > DFQualitySQU)
//                                ////{
//                                ////    decimal az = ((decimal)BitConverter.ToInt16(t, 134)) / 10;
//                                ////    if (az >= 0 && az <= 360) { DFAzimuth = az; }
//                                ////}
//                                //decimal az = ((decimal)BitConverter.ToInt16(t, 134)) / 10;
//                                //if (az >= 0 && az <= 360) { DFAzimuth = az; }
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
//                            if (freqstart != TraceTemp[0].freq || TraceTemp.Length != traceDataLength || freqstop != TraceTemp[TraceTemp.Length - 1].freq || step != (TraceTemp[1].freq - TraceTemp[0].freq))
//                            {
//                                #region reset
//                                TraceTemp = new tracepoint[traceDataLength];
//                                if (LevelUnitInd == 0)
//                                {
//                                    for (int i = 0; i < TraceTemp.Length; i++)
//                                    {
//                                        tracepoint p = new tracepoint()
//                                        {
//                                            freq = freqstart + step * i,
//                                            level = (double)((Int16)((t[i * 2 + traceDataFrom + 1] << 8) | (t[i * 2 + traceDataFrom]))) / 10
//                                        };
//                                        TraceTemp[i] = p;
//                                    }
//                                }
//                                else if (LevelUnitInd == 1)
//                                {
//                                    for (int i = 0; i < traceDataLength; i++)
//                                    {
//                                        tracepoint p = new tracepoint()
//                                        {
//                                            freq = freqstart + step * i,
//                                            level = ((double)((Int16)((t[i * 2 + traceDataFrom + 1] << 8) | (t[i * 2 + traceDataFrom]))) / 10) - 107
//                                        };
//                                        TraceTemp[i] = p;
//                                    }
//                                }
//                                #endregion
//                            }
//                            else
//                            {
//                                #region without reset
//                                if (LevelUnitInd == 0)
//                                {

//                                    for (int i = 0; i < TraceTemp.Length; i++)
//                                    {
//                                        TraceTemp[i].level = (double)((Int16)((t[i * 2 + traceDataFrom + 1] << 8) | (t[i * 2 + traceDataFrom]))) / 10;
//                                    }
//                                }
//                                else if (LevelUnitInd == 1)
//                                {
//                                    for (int i = 0; i < TraceTemp.Length; i++)
//                                    {
//                                        TraceTemp[i].level = ((double)((Int16)((t[i * 2 + traceDataFrom + 1] << 8) | (t[i * 2 + traceDataFrom]))) / 10) - 107;
//                                    }
//                                }
//                                #endregion
//                            }

//                            //Trace1 = temp;
//                            if (FFMFreqCentr != freqCentr || FreqCentr != freqCentr) { FFMFreqCentr = freqCentr; FreqCentr = FFMFreqCentr; FreqStart = FFMFreqCentr - freqSpan / 2; FreqStop = FFMFreqCentr + freqSpan / 2; }
//                            if (FFMFreqSpan != freqSpan) { FFMFreqSpanInd = System.Array.IndexOf(UniqueData.FFMSpanBW, freqSpan); FreqSpan = freqSpan; FreqStart = FFMFreqCentr - freqSpan / 2; FreqStop = FFMFreqCentr + freqSpan / 2; }
//                            if (UniqueData.FFMStepBW[FFMStepInd] != step) { FFMStepInd = System.Array.IndexOf(UniqueData.FFMStepBW, step); }
//                            IsRuningUDP = true;
//                            SetTraceData(TraceTemp, step, true);
//                            //Temp = System.Text.Encoding.UTF8.GetString(t);
//                        }
//                        #endregion
//                        #region
//                        //Temp = ""; Temp1 = Ind.ToString();
//                        //Temp += t.Length.ToString() + " " + traceDataFrom.ToString() + " " + (traceDataFrom + traceDataLength * 2 + (traceDataLength / 2) * 2).ToString() + " " + (traceDataFrom + (traceDataLength / 2) * 10 + 4).ToString() + "\r\n" +
//                        //    t[0 + Ind].ToString() + t[1 + Ind].ToString() + t[2 + Ind].ToString() + t[3 + Ind].ToString() + t[4 + Ind].ToString() + "\r\n" +
//                        //    "DF_Squelch_Mode " + DFSquelchMode.ToString() + "\r\n" +
//                        //    "DF_Squelch_Value " + DFSquelchValue.ToString() + "\r\n" +
//                        //    "DFBandwidth " + DFBandwidth.ToString() + "\r\n" +
//                        //    "StepWidth " + StepWidth.ToString() + "\r\n" +
//                        //    "DFMeasureTime " + DFMeasureTime.ToString() + "\r\n" +
//                        //    "DFOption " + DFOption.ToString() + "\r\n" +
//                        //    "CompassHeading " + CompassHeading.ToString() + "\r\n" +
//                        //    "CompassHeadingType " + CompassHeadingType.ToString() + "\r\n" +
//                        //    "AntennaFactor " + AntennaFactor.ToString() + "\r\n" +
//                        //    "DemodFreqChannel " + DemodFreqChannel.ToString() + "\r\n" +
//                        //    "DemodFreq " + DemodFreq.ToString() + "\r\n" +
//                        //    "dt " + dt.ToString() + "\r\n" +





//                        //    "Azimuth " + Azimuth.ToString() + "\r\n" +
//                        //    "DFQuality " + DFQuality.ToString() + "\r\n" +


//                        //            //"uint64 " + ((UInt64)((UInt64)(t[28]) + (UInt64)(t[29] << 8) + (UInt64)(t[30] << 16) + (UInt64)(t[31] << 24) + (UInt64)(t[40] << 32) + (UInt64)(t[41] << 40) + (UInt64)(t[42] << 48) + (UInt64)(t[43] << 56))).ToString() + "\r\n" +
//                        //            "uint8 " + ((t[0 + Ind])).ToString() + "\r\n" +
//                        //            "int16 " + ((double)((Int16)((t[0 + Ind] << 8) | (t[1 + Ind])))).ToString() + "\r\n" +
//                        //            "int16 " + ((double)((Int16)((t[1 + Ind] << 8) | (t[0 + Ind])))).ToString() + "\r\n" +
//                        //            "uint16 " + ((double)((UInt16)((t[0 + Ind] << 8) | (t[1 + Ind])))).ToString() + "\r\n" +
//                        //            "uint16 " + ((double)((UInt16)((t[1 + Ind] << 8) | (t[0 + Ind])))).ToString() + "\r\n" +
//                        //            "uint32 " + ((UInt32)((UInt32)(t[3 + Ind]) + (UInt32)(t[2 + Ind] << 8) + (UInt32)(t[2 + Ind] << 16) + (UInt32)(t[1 + Ind] << 24))).ToString() + "\r\n" +
//                        //            "uint32 " + ((UInt32)((UInt32)(t[0 + Ind]) + (UInt32)(t[1 + Ind] << 8) + (UInt32)(t[2 + Ind] << 16) + (UInt32)(t[3 + Ind] << 24))).ToString() + "  " + "\r\n" +
//                        //            "UInt32 " + BitConverter.ToUInt32(t, Ind).ToString() + "\r\n" +
//                        //            "Int16 " + ((double)BitConverter.ToInt16(t, Ind)).ToString() + "\r\n" +
//                        //            "Float " + BitConverter.ToSingle(t, Ind).ToString() + "\r\n" +
//                        //            "UInt64 " + BitConverter.ToUInt64(t, Ind).ToString() + "\r\n" +
//                        //            "uint64 " + ((UInt64)((UInt64)(t[28]) + (UInt64)(t[29] << 8) + (UInt64)(t[30] << 16) + (UInt64)(t[31] << 24) + (UInt64)(t[40] << 32) + (UInt64)(t[41] << 40) + (UInt64)(t[42] << 48) + (UInt64)(t[43] << 56))).ToString();

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
//                        //Temp = System.Text.Encoding.UTF8.GetString(t);
//                        #endregion
//                    }

//                    //if (Markers[0].Level != null && ChannelPower)
//                    //{
//                    //    //if (Markers[0].OnTrace == 1) { ChannelPowerResult = Math.Round(LM.MeasChannelPower(Trace, (FreqStart + FreqStop) / 2, ChannelPowerBW, 0), 2); }
//                    //    //else if (Markers[0].OnTrace == 2) { ChannelPowerResult = Math.Round(LM.MeasChannelPower(TraceMaxHold, (FreqStart + FreqStop) / 2, ChannelPowerBW, 0), 2); }
//                    //    //else if (Markers[0].OnTrace == 3) { ChannelPowerResult = Math.Round(LM.MeasChannelPower(TraceAverage, (FreqStart + FreqStop) / 2, ChannelPowerBW, 0), 2); }
//                    //}
//                    //if (NdBStateEst == true)
//                    //{
//                    //    //GetNDB((int)Markers[0].Freq, (decimal)NdBLevel);
//                    //}
//                    //if (IsMeasMon && ((FinishPscan && Mode.Mode == "PSCAN") || Mode.Mode == "FFM")) { ThisMeasCount++; }
//                    if (IsMeasMon && ((FinishPscan && Mode.Mode == "PSCAN") || Mode.Mode == "FFM") && MeasMonItem != null && MeasMonItem.AllTraceCountToMeas > MeasMonItem.AllTraceCount) // MeasTraceCount > -1)
//                    {
//                        #region
//                        bool t1 = MeasMonItem.FreqDN == FreqCentr;
//                        bool t2 = MeasMonItem.SpecData.FreqSpan == FreqSpan;
//                        bool t3 = ((FinishPscan && Mode.Mode == "PSCAN") || Mode.Mode == "FFM");
//                        //decimal ndblevel = App.Sett.MeasMons_Settings.MeasMons.Where(x => x.Techonology.ToString() == MeasMonItem.Techonology).First().NdBLevel;

//                        if (t1 && t2 && t3)
//                        {
//                            #region
//                            if (MeasMonItem.SpecData.Trace[0].freq == Trace1[0].freq &&
//                                MeasMonItem.SpecData.Trace[MeasMonItem.SpecData.Trace.Length - 1].freq == Trace1[Trace1.Length - 1].freq &&
//                                MeasSpecOldLow != Trace1[0].level &&
//                                MeasSpecOldHi != Trace1[Trace1.Length - 1].level)
//                            { MeasSpec++; MeasSpecOldLow = Trace1[0].level; MeasSpecOldHi = Trace1[Trace1.Length - 1].level; }
//                            int dfc = 0;
//                            int ufc = 0;
//                            int cf = 0;
//                            double dfl = 0;
//                            double ufl = 0;
//                            double cfl = 0;
//                            if (MeasMonItem.Techonology == "GSM")
//                            {
//                                dfc = LM.FindMarkerIndOnTrace(Trace1, (decimal)(MeasMonItem.FreqDN - (decimal)(MeasMonItem.BWData.BWMeasMax / 2)));
//                                ufc = LM.FindMarkerIndOnTrace(Trace1, (decimal)(MeasMonItem.FreqDN + (decimal)(MeasMonItem.BWData.BWMeasMax / 2)));
//                                cf = LM.FindMarkerIndOnTrace(Trace1, (decimal)(MeasMonItem.FreqDN));
//                                dfl = LM.AverageLevelNearPoint(Trace1, dfc, 10);
//                                ufl = LM.AverageLevelNearPoint(Trace1, ufc, 10);
//                                cfl = LM.AverageLevelNearPoint(Trace1, cf, 10);
//                            }
//                            if (MeasMonItem.Techonology != "GSM" || (cfl > dfl + MeasMonItem.BWData.NdBLevel + 5 && cfl > ufl + MeasMonItem.BWData.NdBLevel + 5))
//                            {
//                                bool changeTrace = false;
//                                if (MeasMonItem.SpecData.Trace == null || MeasMonItem.SpecData.Trace[0] == null || MeasMonItem.SpecData.Trace.Length != TracePoints || MeasMonItem.SpecData.Trace[0].freq != Trace1[0].freq || MeasMonItem.SpecData.Trace[MeasMonItem.SpecData.Trace.Length - 1].freq != Trace1[Trace1.Length - 1].freq)
//                                {
//                                    MeasMonItem.SpecData.Trace = new tracepoint[TracePoints];
//                                    for (int i = 0; i < TracePoints; i++)
//                                    {
//                                        tracepoint p = new tracepoint() { freq = Trace1[i].freq, level = Trace1[i].level };
//                                        MeasMonItem.SpecData.Trace[i] = p;
//                                    }
//                                    MeasMonItem.SpecData.MeasStart = MainWindow.gps.LocalTime;
//                                    MeasMonItem.SpecData.MeasStop = MainWindow.gps.LocalTime;
//                                    MeasMonItem.SpecData.LastMeasLatitude = (double)MainWindow.gps.LatitudeDecimal;
//                                    MeasMonItem.SpecData.LastMeasLongitude = (double)MainWindow.gps.LongitudeDecimal;
//                                    MeasMonItem.SpecData.LastMeasAltitude = (double)MainWindow.gps.Altitude;
//                                    MeasMonItem.SpecData.TraceCount++;
//                                    //if (Mode.Mode == "PSCAN") { MeasMonItem.TraceStep = UniqueData.PSCANStepBW[PScanStepInd]; }
//                                    //if (Mode.Mode == "FFM") { MeasMonItem.TraceStep = UniqueData.FFMStepBW[FFMStepInd]; }
//                                    changeTrace = true;
//                                }
//                                else /*if (MeasMonItem.Trace != null && MeasMonItem.Trace[0] != null && MeasMonItem.Trace.Length == TracePoints && MeasMonItem.Trace[0].Freq == Trace[0].Freq && MeasMonItem.Trace[MeasMonItem.Trace.Length - 1].Freq == Trace[Trace.Length - 1].Freq)*/
//                                {
//                                    // если чето в накоплении этот трейс поменяет
//                                    for (int i = 0; i < Trace1.Length; i++)
//                                    {
//                                        if (Trace1[i].level >= MeasMonItem.SpecData.Trace[i].level)
//                                        { MeasMonItem.SpecData.Trace[i].level = Trace1[i].level; changeTrace = true; }
//                                    }
//                                    if (changeTrace)
//                                    {
//                                        MeasMonItem.SpecData.MeasStop = MainWindow.gps.LocalTime;
//                                        MeasMonItem.SpecData.LastMeasLatitude = (double)MainWindow.gps.LatitudeDecimal;
//                                        MeasMonItem.SpecData.LastMeasLongitude = (double)MainWindow.gps.LongitudeDecimal;
//                                        MeasMonItem.SpecData.LastMeasAltitude = (double)MainWindow.gps.Altitude;
//                                    }
//                                    MeasMonItem.SpecData.TraceCount++;
//                                }
//                                //if (MeasMonItem.Techonology == "UHF")
//                                //{
//                                //    for (int i = 0; i < MeasMonItem.Trace.Length; i++)
//                                //    {
//                                //        if (MeasMonItem.Trace[i].Freq > MeasMonItem.FreqDN - MeasMonItem.TraceStep / 2 && MeasMonItem.Trace[i].Freq < MeasMonItem.FreqDN + MeasMonItem.TraceStep / 2)
//                                //        {
//                                //            MeasMonItem.Power = MeasMonItem.Trace[i].Level;
//                                //            for (int y = 0; y < MainWindow.IdfData.UHFBTS.Count; y++)
//                                //            {
//                                //                if (MainWindow.IdfData.UHFBTS[y].PlanFreq_ID == MeasMonItem.PlanFreq_ID && MainWindow.IdfData.UHFBTS[y].Plan_ID == MeasMonItem.PLAN_ID && MainWindow.IdfData.UHFBTS[y].FreqDn == MeasMonItem.FreqDN)
//                                //                { MainWindow.IdfData.UHFBTS[y].Power = MeasMonItem.Power; }
//                                //            }
//                                //        }
//                                //    }
//                                //}
//                                //ищем пик уровня измерения NdB
//                                if (changeTrace)//&& MeasMonItem.AllTraceCountToMeas - 1 == MeasMonItem.AllTraceCount)
//                                {
//                                    int ind = -1;
//                                    double tl = double.MinValue;
//                                    decimal minf = (MeasMonItem.FreqDN - (MeasMonItem.BWData.BWMarPeak / 2));
//                                    decimal maxf = (MeasMonItem.FreqDN + (MeasMonItem.BWData.BWMarPeak / 2));
//                                    for (int i = 0; i < MeasMonItem.SpecData.Trace.Length; i++)
//                                    {
//                                        if (MeasMonItem.SpecData.Trace[i].freq > minf && MeasMonItem.SpecData.Trace[i].freq < maxf && MeasMonItem.SpecData.Trace[i].level > tl)
//                                        { tl = MeasMonItem.SpecData.Trace[i].level; ind = i; }
//                                        //if (MeasMonItem.Techonology == "UHF" && MeasMonItem.Trace[i].Freq > MeasMonItem.FreqDN - MeasMonItem.TraceStep / 2 && MeasMonItem.Trace[i].Freq < MeasMonItem.FreqDN + MeasMonItem.TraceStep / 2)
//                                        //{
//                                        //    MeasMonItem.Power = MeasMonItem.Trace[i].Level;
//                                        //    for (int y = 0; y < MainWindow.IdfData.UHFBTS.Count; y++)
//                                        //    {
//                                        //        if (MainWindow.IdfData.UHFBTS[y].PlanFreq_ID == MeasMonItem.PlanFreq_ID && MainWindow.IdfData.UHFBTS[y].Plan_ID == MeasMonItem.PLAN_ID && MainWindow.IdfData.UHFBTS[y].FreqDn == MeasMonItem.FreqDN)
//                                        //        { MainWindow.IdfData.UHFBTS[y].Power = MeasMonItem.Power; }
//                                        //    }
//                                        //}
//                                    }
//                                    MeasMonItem.BWData.NdBResult[0] = ind;
//                                    int[] mar = new int[3];
//                                    if (MeasMonItem.Techonology == "GSM")
//                                    {
//                                        mar = LM.GetMeasNDB(MeasMonItem.SpecData.Trace, MeasMonItem.BWData.NdBResult[0], MeasMonItem.BWData.NdBLevel, MeasMonItem.SpecData.FreqCentr, MeasMonItem.BWData.BWMeasMax, MeasMonItem.BWData.BWMeasMin);
//                                    }
//                                    else
//                                    {
//                                        mar = LM.GetMeasNDB(MeasMonItem.SpecData.Trace, MeasMonItem.BWData.NdBResult[0], MeasMonItem.BWData.NdBLevel, MeasMonItem.SpecData.FreqCentr, MeasMonItem.BWData.BWMeasMax, MeasMonItem.BWData.BWMeasMin);
//                                    }
//                                    if (mar != null && mar[1] > -1 && mar[2] > -1)
//                                    {
//                                        MeasMonItem.BWData.BWMeasured = (decimal)(MeasMonItem.SpecData.Trace[mar[2]].freq - MeasMonItem.SpecData.Trace[mar[1]].freq);
//                                        MeasMonItem.BWData.NdBResult = mar;
//                                        //MeasMonItem.MarkerT2Ind = mar[1];
//                                        MeasMonItem.DeltaFreqMeasured = Math.Round(((Math.Abs(((MeasMonItem.SpecData.Trace[mar[1]].freq + MeasMonItem.SpecData.Trace[mar[2]].freq) / 2) - (MeasMonItem.SpecData.FreqCentr))) / (MeasMonItem.SpecData.FreqCentr)) / 1000000, 3);
//                                        //if (Math.Abs(MeasMonItem.SpecData.Trace[MeasMonItem.MarkerInd].level - MeasMonItem.Trace[mar[0]].level - MeasMonItem.NdBLevel) < 2 && Math.Abs(MeasMonItem.Trace[MeasMonItem.MarkerInd].level - MeasMonItem.Trace[mar[1]].level - MeasMonItem.NdBLevel) < 2) MeasMonItem.Measured = true;
//                                        MeasMonItem.NewDataToSave = true;
//                                    }
//                                    else
//                                    {
//                                        MeasMonItem.BWData.NdBResult[1] = -1;
//                                        MeasMonItem.BWData.NdBResult[2] = -1;
//                                        //MeasMonItem.Measured = false;
//                                        MeasMonItem.NewDataToSave = true;
//                                    }
//                                    if (MeasMonItem.IdentificationData is GSMBTSData)
//                                    { MeasMonItem.station_sys_info = ((GSMBTSData)MeasMonItem.IdentificationData).GetStationInfo(); }
//                                    else if (MeasMonItem.IdentificationData is LTEBTSData)
//                                    { MeasMonItem.station_sys_info = ((LTEBTSData)MeasMonItem.IdentificationData).GetStationInfo(); }
//                                    else if (MeasMonItem.IdentificationData is UMTSBTSData)
//                                    { MeasMonItem.station_sys_info = ((UMTSBTSData)MeasMonItem.IdentificationData).GetStationInfo(); }
//                                    else if (MeasMonItem.IdentificationData is CDMABTSData)
//                                    { MeasMonItem.station_sys_info = ((CDMABTSData)MeasMonItem.IdentificationData).GetStationInfo(); }

//                                    if (MeasMonItem.SpecData.MeasStart == DateTime.MinValue) MeasMonItem.SpecData.MeasStart = MainWindow.gps.LocalTime;
//                                }
//                                //MeasMonItem.TraceCount++;
//                                // (int)(MeasItem.Trace[MeasItem.Trace.Length - 1].Freq - MeasItem.Trace[0].Freq);// mar[1];
//                            }
//                            if (MeasMonItem.AllTraceCount < MeasMonItem.AllTraceCountToMeas)
//                            { MeasMonItem.AllTraceCount++; }

//                            #endregion
//                        }
//                        else if ((!t1 || !t2) && t3) { /*MeasMonItem.TraceCount++; */MeasMonItem.AllTraceCount++; }
//                        if (MeasMonItem.AllTraceCountToMeas == MeasMonItem.AllTraceCount)
//                        { MeasMonItem.SpecData.MeasDuration += new TimeSpan(DateTime.Now.Ticks - MeasMonTimeMeas).TotalSeconds; }
//                        //if (MeasMonItem.AllTraceCount == MeasMonItem.AllTraceCountToMeas || MeasMonItem.TraceCount == MeasTraceCount || ThisMeasCount >= 10 || MeasMonItem.Power < App.Sett.MeasMons_Settings.MeasMons.Where(x => x.Techonology == MeasMonItem.Techonology).First().DetectionLevel)
//                        //{
//                        //    ThisMeasCount = -1;
//                        //    MeasTraceCount = -1; /*MeasItem = null;*/
//                        //}
//                        //if (MeasMonItem.AllTraceCount == MeasTraceCount || MeasMonItem.TraceCount == MeasTraceCount || ThisMeasCount >= 10 || MeasMonItem.Power < App.Sett.MeasMons_Settings.MeasMons.Where(x => x.Techonology == MeasMonItem.Techonology).First().DetectionLevel)
//                        //{
//                        //    ThisMeasCount = -1;
//                        //    MeasTraceCount = -1; /*MeasItem = null;*/
//                        //}
//                        //if (MainWindow.db.MonMeas[MeasItem].TraceCount == MeasTraceCount + 100) MeasTraceCount = 0;
//                        #endregion

//                    }


//                }
//                #region
//                //Temp = "";
//                //Temp += t.Length.ToString() + "\r\n" +
//                //            "uint64 " + ((UInt64)((UInt64)(t[28]) + (UInt64)(t[29] << 8) + (UInt64)(t[30] << 16) + (UInt64)(t[31] << 24) + (UInt64)(t[40] << 32) + (UInt64)(t[41] << 40) + (UInt64)(t[42] << 48) + (UInt64)(t[43] << 56))).ToString() + "\r\n" +
//                //            "int16 " + ((double)((Int16)((t[0 + Ind] << 8) | (t[1 + Ind])))).ToString() + "\r\n" +
//                //            "int16 " + ((double)((Int16)((t[1 + Ind] << 8) | (t[0 + Ind])))).ToString() + "\r\n" +
//                //            "uint16 " + ((double)((UInt16)((t[0 + Ind] << 8) | (t[1 + Ind])))).ToString() + "\r\n" +
//                //            "uint16 " + ((double)((UInt16)((t[1 + Ind] << 8) | (t[0 + Ind])))).ToString() + "\r\n" +
//                //            "uint32 " + ((UInt32)((UInt32)(t[3 + Ind]) + (UInt32)(t[2 + Ind] << 8) + (UInt32)(t[2 + Ind] << 16) + (UInt32)(t[1 + Ind] << 24))).ToString() + "\r\n" +
//                //            "uint32 " + ((UInt32)((UInt32)(t[0 + Ind]) + (UInt32)(t[1 + Ind] << 8) + (UInt32)(t[2 + Ind] << 16) + (UInt32)(t[3 + Ind] << 24))).ToString() + "  " + "\r\n" +
//                //            "UInt32 " + BitConverter.ToUInt32(t, Ind).ToString() + "\r\n" +
//                //            "Int16 " + ((double)BitConverter.ToInt16(t, Ind)).ToString() + "\r\n" +
//                //            "Float " + BitConverter.ToSingle(t, Ind).ToString() + "\r\n" +
//                //            "UInt64 " + BitConverter.ToUInt64(t, Ind).ToString() + "\r\n" +
//                //            "uint64 " + ((UInt64)((UInt64)(t[28]) + (UInt64)(t[29] << 8) + (UInt64)(t[30] << 16) + (UInt64)(t[31] << 24) + (UInt64)(t[40] << 32) + (UInt64)(t[41] << 40) + (UInt64)(t[42] << 48) + (UInt64)(t[43] << 56))).ToString();

//                #endregion
//                //}
//                //#region Exception
//                //catch (Exception exp)
//                //{
//                //    MainWindow.exp.ExceptionData = new ExData() { ex = exp, ClassName = "RsReceiver", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
//                //}
//                //#endregion
//            }
//        }
//    }
//}
