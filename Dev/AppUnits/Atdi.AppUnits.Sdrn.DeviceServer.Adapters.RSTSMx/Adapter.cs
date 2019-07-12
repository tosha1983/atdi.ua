using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;
using System.ComponentModel;


using AC = Atdi.Common;
using Atdi.Contracts.Sdrn.DeviceServer;
using Atdi.DataModels.Sdrn.DeviceServer;
using Atdi.Platform.Logging;

using AD = Atdi.DataModels.Sdrn.DeviceServer.Adapters;
using CFG = Atdi.DataModels.Sdrn.DeviceServer.Adapters.Config;
using COM = Atdi.DataModels.Sdrn.DeviceServer.Commands;
using COMR = Atdi.DataModels.Sdrn.DeviceServer.Commands.Results;
using COMRMSI = Atdi.DataModels.Sdrn.DeviceServer.Commands.Results.MesureSystemInfo;
using MEN = Atdi.DataModels.Sdrn.DeviceServer.Adapters.Enums;
using LGSM = Atdi.AppUnits.Sdrn.DeviceServer.Adapters.RSTSMx.GSM;
using LUMTS = Atdi.AppUnits.Sdrn.DeviceServer.Adapters.RSTSMx.UMTS;
using LLTE = Atdi.AppUnits.Sdrn.DeviceServer.Adapters.RSTSMx.LTE;
using LCDMA = Atdi.AppUnits.Sdrn.DeviceServer.Adapters.RSTSMx.CDMAEVDO;
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

namespace Atdi.AppUnits.Sdrn.DeviceServer.Adapters.RSTSMx
{
    /// <summary>
    /// Этот адаптер неможет существовать в нескольких экземплярах! Только Debug и x86
    /// </summary>
    public class Adapter : IAdapter
    {
        private readonly ITimeService _timeService;
        private readonly ILogger _logger;
        private readonly AdapterConfig _adapterConfig;
        private readonly IWorkScheduler _workScheduler;
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
        public Adapter(AdapterConfig adapterConfig, ILogger logger, ITimeService timeService, IWorkScheduler workScheduler)
        {
            this._logger = logger;
            this._adapterConfig = adapterConfig;
            this._timeService = timeService;
            this._workScheduler = workScheduler;
            LPC = new LocalParametersConverter();
            GSMBTS = new List<LGSM.BTSData>() { };
            UMTSBTS = new List<LUMTS.BTSData>() { };
            LTEBTS = new List<LLTE.BTSData>() { };
            CDMABTS = new List<LCDMA.BTSData>() { };
            ViComBinPath = _adapterConfig.RSViComPath + @"\bin\"; //@"c:\RuS\RS-ViCom-Pro-16.25.0.743\bin\";//@"c:\RuS\RS-ViCom-16.5.0.0\bin\";
            string newPath = Environment.GetEnvironmentVariable("PATH") + @";" + ViComBinPath + ";";
            Environment.SetEnvironmentVariable("PATH", newPath);
            _DeviceType = _adapterConfig.DeviceType;
            try
            {
                //Ахахахаааа
                //грохнем процесс рудика он же romes он же vicom, а вдруг лапки
                System.Diagnostics.Process[] ps2 = System.Diagnostics.Process.GetProcessesByName("RuSWorkerDllLoaderPhysicalLayer"); //Имя процесса
                foreach (System.Diagnostics.Process p1 in ps2)
                {
                    p1.Kill();
                }
            }
            catch (Exception e)
            {
                _logger.Exception(Contexts.ThisComponent, e);
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

                if (Connect(_adapterConfig.IPAddress))
                {
                    SetGSMBandData();
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
                    MesureSysInfoDeviceProperties msidp = GetProperties(MainConfig);
                    host.RegisterHandler<COM.MesureSystemInfoCommand, COMR.MesureSystemInfoResult>(MesureSystemInfoHandler, msidp);                   
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
                if (GSMIsRuning)
                {
                    GsmDisconnect();
                }
                if (CDMAIsRuning)
                {
                    CDMADisconnect();
                }
                if (UMTSIsRuning)
                {
                    UmtsDisconnect();
                }
                if (LTEIsRuning)
                {
                    LTEDisconnect();
                }
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
                if (IsRuning)
                {
                    context.Lock();
                    if (command.Parameter.RFInput == null || command.Parameter.RFInput != 1 || command.Parameter.RFInput != 2)
                    { command.Parameter.RFInput = 1; }
                    if (command.Parameter.DelayToSendResult_s == null || command.Parameter.DelayToSendResult_s == double.NaN || command.Parameter.DelayToSendResult_s < 30)
                    { command.Parameter.DelayToSendResult_s = 30; }
                    if (command.Parameter.DelayToSendResult_s == null || command.Parameter.DelayToSendResult_s == double.NaN ||command.Parameter.DelayToSendResult_s > 300)
                    { command.Parameter.DelayToSendResult_s = 300; }
                    if (command.Parameter.Standart.ToLower().Contains("gsm"))
                    {
                        RFInputGSM = command.Parameter.RFInput;
                        if (Option_GSM == 1)
                        {
                            List<decimal> freqs = new List<decimal>() { };
                            List<LGSM.BandData> bands = new List<LGSM.BandData>() { };
                            if (command.Parameter.Bands != null && command.Parameter.Bands.Length > 0)
                            {
                                if (command.Parameter.FreqType == COM.Parameters.MesureSystemInfo.FreqType.New)
                                {
                                    for (int i = 0; i < command.Parameter.Bands.Length; i++)
                                    {
                                        for (int j = 0; j < AllGSMBands.Count; j++)
                                        {
                                            if (command.Parameter.Bands[i] == AllGSMBands[j].Band.ToString())
                                            {
                                                bands.Add(AllGSMBands[j]);
                                            }
                                        }
                                    }
                                }
                            }
                            else if (command.Parameter.Freqs_Hz != null && command.Parameter.Freqs_Hz.Length > 0)
                            {
                                if (command.Parameter.FreqType == COM.Parameters.MesureSystemInfo.FreqType.New)
                                {
                                    for (int i = 0; i < command.Parameter.Freqs_Hz.Length; i++)
                                    {
                                        try
                                        {
                                            freqs.Add(GetGSMCHfromFreqDN(command.Parameter.Freqs_Hz[i]).FreqDn);
                                        }
                                        catch (Exception e)
                                        {
                                            // желательно записать влог
                                            _logger.Exception(Contexts.ThisComponent, e);
                                            // этот вызов обязательный в случаи обрыва
                                            context.Abort(e);
                                            // дальше кода быть не должно, освобождаем поток}
                                        }
                                    }
                                }
                            }
                            else if (command.Parameter.Freqs_Hz != null && command.Parameter.Bands != null && command.Parameter.Bands.Length > 0 && command.Parameter.Bands.Length > 0)
                            {
                                if (command.Parameter.FreqType == COM.Parameters.MesureSystemInfo.FreqType.New)
                                {
                                    for (int i = 0; i < command.Parameter.Bands.Length; i++)
                                    {
                                        for (int j = 0; j < AllGSMBands.Count; j++)
                                        {
                                            if (command.Parameter.Bands[i] == AllGSMBands[j].Band.ToString())
                                            {
                                                bands.Add(AllGSMBands[j]);
                                            }
                                        }
                                    }
                                    for (int i = 0; i < command.Parameter.Freqs_Hz.Length; i++)
                                    {
                                        try
                                        {
                                            freqs.Add(GetGSMCHfromFreqDN(command.Parameter.Freqs_Hz[i]).FreqDn);
                                        }
                                        catch (Exception e)
                                        {
                                            // желательно записать влог
                                            _logger.Exception(Contexts.ThisComponent, e);
                                            // этот вызов обязательный в случаи обрыва
                                            context.Abort(e);
                                            // дальше кода быть не должно, освобождаем поток}
                                        }
                                    }
                                }

                            }
                            GSMBTS.Clear();
                            GetUnifreqsGSM(freqs, bands);
                            GsmConnect();


                            if (!command.Parameter.PeriodicResult)
                            {
                                _workScheduler.Run(command.Parameter.Standart, (Action)(() =>
                                 {
                                     List<COMR.MesureSystemInfo.StationSystemInfo> res = new List<COMR.MesureSystemInfo.StationSystemInfo>() { };
                                     GSMUpdateData = false;
                                     for (int i = 0; i < GSMBTS.Count; i++)
                                     {
                                         if (command.Parameter.ResultOnlyWithGCID)
                                         {
                                             if (GSMBTS[i].FullData)
                                             {
                                                 res.Add(GSMBTS[i].StationSysInfo);
                                             }
                                         }
                                         else
                                         {
                                             res.Add(GSMBTS[i].StationSysInfo);
                                         }
                                     }
                                     COMR.MesureSystemInfoResult msir = new COMR.MesureSystemInfoResult(0, CommandResultStatus.Final)
                                     {
                                         DeviceStatus = COMR.Enums.DeviceStatus.Normal,
                                         SystemInfo = res.ToArray()
                                     };
                                     context.PushResult(msir);
                                     GsmDisconnect();
                                     context.Unlock();
                                     context.Finish();

                                 }), (int)command.Parameter.DelayToSendResult_s * 1000);
                            }
                            else
                            {
                                long lastpushrelults = Common.WinAPITime.GetTimeStamp();
                                _workScheduler.Run(command.Parameter.Standart, (Action)(() =>
                                {
                                    long step = (long)(command.Parameter.DelayToSendResult_s * 10000000);
                                    ulong resultid = 0;
                                    while (!context.Token.IsCancellationRequested)
                                    {
                                        Thread.Sleep(10);//чтоб не жрало проц
                                        if ((Common.WinAPITime.GetTimeStamp() - lastpushrelults) > step)
                                        {
                                            lastpushrelults = Common.WinAPITime.GetTimeStamp();
                                            List<COMR.MesureSystemInfo.StationSystemInfo> res = new List<COMR.MesureSystemInfo.StationSystemInfo>() { };

                                            for (int i = 0; i < GSMBTS.Count; i++)
                                            {
                                                if (command.Parameter.ResultOnlyWithGCID)
                                                {
                                                    if (GSMBTS[i].FullData)
                                                    {
                                                        res.Add(GSMBTS[i].StationSysInfo);
                                                    }
                                                }
                                                else
                                                {
                                                    res.Add(GSMBTS[i].StationSysInfo);
                                                }
                                            }
                                            COMR.MesureSystemInfoResult msir = new COMR.MesureSystemInfoResult(resultid, CommandResultStatus.Next)
                                            {
                                                DeviceStatus = COMR.Enums.DeviceStatus.Normal,
                                                SystemInfo = res.ToArray()
                                            };
                                            context.PushResult(msir);
                                            resultid++;
                                        }
                                    }
                                    GSMUpdateData = false;
                                    GsmDisconnect();
                                    context.Unlock();
                                    context.Finish();
                                }));
                            }
                        }
                        else
                        {
                            throw new Exception(command.Parameter.Standart + " option unavailable");
                        }
                    }
                    else if (command.Parameter.Standart.ToLower().Contains("umts"))
                    {
                        RFInputUMTS = command.Parameter.RFInput;
                        if (Option_UMTS == 1)
                        {
                            List<decimal> freqs = new List<decimal>() { };
                            if (command.Parameter.Freqs_Hz != null && command.Parameter.Freqs_Hz.Length > 0)
                            {
                                if (command.Parameter.FreqType == COM.Parameters.MesureSystemInfo.FreqType.New)
                                {
                                    for (int i = 0; i < command.Parameter.Freqs_Hz.Length; i++)
                                    {
                                        try
                                        {
                                            freqs.Add(GetUMTSCHFromFreqDN(command.Parameter.Freqs_Hz[i]).FreqDn);
                                        }
                                        catch (Exception e)
                                        {
                                            // желательно записать влог
                                            _logger.Exception(Contexts.ThisComponent, e);
                                            // этот вызов обязательный в случаи обрыва
                                            context.Abort(e);
                                            // дальше кода быть не должно, освобождаем поток}
                                        }
                                    }
                                }
                            }
                            UMTSBTS.Clear();
                            GetUnifreqsUMTS(freqs);
                            UmtsConnect();

                            if (!command.Parameter.PeriodicResult)
                            {
                                _workScheduler.Run(command.Parameter.Standart, (Action)(() =>
                            {
                                List<COMR.MesureSystemInfo.StationSystemInfo> res = new List<COMR.MesureSystemInfo.StationSystemInfo>() { };
                                UMTSUpdateData = false;
                                for (int i = 0; i < UMTSBTS.Count; i++)
                                {
                                    if (command.Parameter.ResultOnlyWithGCID)
                                    {
                                        if (UMTSBTS[i].FullData)
                                        {
                                            res.Add(UMTSBTS[i].StationSysInfo);
                                        }
                                    }
                                    else
                                    {
                                        res.Add(UMTSBTS[i].StationSysInfo);
                                    }
                                }
                                COMR.MesureSystemInfoResult msir = new COMR.MesureSystemInfoResult(0, CommandResultStatus.Final)
                                {
                                    DeviceStatus = COMR.Enums.DeviceStatus.Normal,
                                    SystemInfo = res.ToArray()
                                };
                                context.PushResult(msir);
                                UmtsDisconnect();
                                context.Unlock();
                                context.Finish();

                            }), (int)command.Parameter.DelayToSendResult_s * 1000);
                            }

                            else
                            {
                                long lastpushrelults = Common.WinAPITime.GetTimeStamp();
                                _workScheduler.Run(command.Parameter.Standart, (Action)(() =>
                                {
                                    long step = (long)(command.Parameter.DelayToSendResult_s * 10000000);
                                    ulong resultid = 0;
                                    while (!context.Token.IsCancellationRequested)
                                    {
                                        Thread.Sleep(10);//чтоб не жрало проц
                                        if ((Common.WinAPITime.GetTimeStamp() - lastpushrelults) > step)
                                        {
                                            lastpushrelults = Common.WinAPITime.GetTimeStamp();
                                            List<COMR.MesureSystemInfo.StationSystemInfo> res = new List<COMR.MesureSystemInfo.StationSystemInfo>() { };

                                            for (int i = 0; i < UMTSBTS.Count; i++)
                                            {
                                                if (command.Parameter.ResultOnlyWithGCID)
                                                {
                                                    if (UMTSBTS[i].FullData)
                                                    {
                                                        res.Add(UMTSBTS[i].StationSysInfo);
                                                    }
                                                }
                                                else
                                                {
                                                    res.Add(UMTSBTS[i].StationSysInfo);
                                                }
                                            }
                                            COMR.MesureSystemInfoResult msir = new COMR.MesureSystemInfoResult(resultid, CommandResultStatus.Next)
                                            {
                                                DeviceStatus = COMR.Enums.DeviceStatus.Normal,
                                                SystemInfo = res.ToArray()
                                            };
                                            context.PushResult(msir);
                                            resultid++;
                                        }
                                    }
                                    UMTSUpdateData = false;
                                    UmtsDisconnect();
                                    context.Unlock();
                                    context.Finish();
                                }));
                            }
                        }
                        else
                        {
                            throw new Exception(command.Parameter.Standart + " option unavailable");
                        }
                    }
                    else if (command.Parameter.Standart.ToLower().Contains("lte"))
                    {
                        RFInputLTE = command.Parameter.RFInput;
                        if (Option_LTE == 1)
                        {
                            List<decimal> freqs = new List<decimal>() { };
                            if (command.Parameter.Freqs_Hz != null && command.Parameter.Freqs_Hz.Length > 0)
                            {
                                if (command.Parameter.FreqType == COM.Parameters.MesureSystemInfo.FreqType.New)
                                {
                                    for (int i = 0; i < command.Parameter.Freqs_Hz.Length; i++)
                                    {
                                        try
                                        {
                                            freqs.Add(GetLTECHfromFreqDN(command.Parameter.Freqs_Hz[i]).FreqDn);
                                        }
                                        catch (Exception e)
                                        {
                                            // желательно записать влог
                                            _logger.Exception(Contexts.ThisComponent, e);
                                            // этот вызов обязательный в случаи обрыва
                                            context.Abort(e);
                                            // дальше кода быть не должно, освобождаем поток}
                                        }
                                    }
                                }
                            }
                            LTEBTS.Clear();
                            GetUnifreqsLTE(freqs);
                            LTEConnect();
                            if (!command.Parameter.PeriodicResult)
                            {
                                _workScheduler.Run(command.Parameter.Standart, (Action)(() =>
                            {
                                List<COMR.MesureSystemInfo.StationSystemInfo> res = new List<COMR.MesureSystemInfo.StationSystemInfo>() { };
                                LTEUpdateData = false;
                                for (int i = 0; i < LTEBTS.Count; i++)
                                {
                                    if (command.Parameter.ResultOnlyWithGCID)
                                    {
                                        if (LTEBTS[i].FullData)
                                        {
                                            res.Add(LTEBTS[i].StationSysInfo);
                                        }
                                    }
                                    else
                                    {
                                        res.Add(LTEBTS[i].StationSysInfo);
                                    }
                                }
                                COMR.MesureSystemInfoResult msir = new COMR.MesureSystemInfoResult(0, CommandResultStatus.Final)
                                {
                                    DeviceStatus = COMR.Enums.DeviceStatus.Normal,
                                    SystemInfo = res.ToArray()
                                };
                                context.PushResult(msir);
                                LTEDisconnect();
                                context.Unlock();
                                context.Finish();

                            }), (int)command.Parameter.DelayToSendResult_s * 1000);
                            }
                            else
                            {
                                long lastpushrelults = Common.WinAPITime.GetTimeStamp();
                                _workScheduler.Run(command.Parameter.Standart, (Action)(() =>
                                {
                                    long step = (long)(command.Parameter.DelayToSendResult_s * 10000000);
                                    ulong resultid = 0;
                                    while (!context.Token.IsCancellationRequested)
                                    {
                                        Thread.Sleep(10);//чтоб не жрало проц
                                        if ((Common.WinAPITime.GetTimeStamp() - lastpushrelults) > step)
                                        {
                                            lastpushrelults = Common.WinAPITime.GetTimeStamp();
                                            List<COMR.MesureSystemInfo.StationSystemInfo> res = new List<COMR.MesureSystemInfo.StationSystemInfo>() { };

                                            for (int i = 0; i < LTEBTS.Count; i++)
                                            {
                                                if (command.Parameter.ResultOnlyWithGCID)
                                                {
                                                    if (LTEBTS[i].FullData)
                                                    {
                                                        res.Add(LTEBTS[i].StationSysInfo);
                                                    }
                                                }
                                                else
                                                {
                                                    res.Add(LTEBTS[i].StationSysInfo);
                                                }
                                            }
                                            COMR.MesureSystemInfoResult msir = new COMR.MesureSystemInfoResult(resultid, CommandResultStatus.Next)
                                            {
                                                DeviceStatus = COMR.Enums.DeviceStatus.Normal,
                                                SystemInfo = res.ToArray()
                                            };
                                            context.PushResult(msir);
                                            resultid++;
                                        }
                                    }
                                    LTEUpdateData = false;
                                    LTEDisconnect();
                                    context.Unlock();
                                    context.Finish();
                                }));
                            }
                        }
                        else
                        {
                            throw new Exception(command.Parameter.Standart + " option unavailable");
                        }
                    }
                    else if (command.Parameter.Standart.ToLower().Contains("cdma") || command.Parameter.Standart.ToLower().Contains("evdo"))
                    {
                        RFInputCDMA = command.Parameter.RFInput;
                        List<LCDMA.Channel> freqs = new List<LCDMA.Channel>() { };
                        if (Option_CDMA == 1)
                        {
                            if (command.Parameter.Freqs_Hz != null && command.Parameter.Freqs_Hz.Length > 0 &&
                                command.Parameter.CDMAEVDOFreqTypes != null && command.Parameter.CDMAEVDOFreqTypes.Length > 0 &&
                                command.Parameter.CDMAEVDOFreqTypes.Length == command.Parameter.Freqs_Hz.Length)
                            {
                                if (command.Parameter.FreqType == COM.Parameters.MesureSystemInfo.FreqType.New)
                                {
                                    for (int i = 0; i < command.Parameter.Freqs_Hz.Length; i++)
                                    {
                                        try
                                        {
                                            if (!command.Parameter.CDMAEVDOFreqTypes[i])
                                            {
                                                freqs.Add(GetCDMACHfromFreqDN(command.Parameter.Freqs_Hz[i], command.Parameter.CDMAEVDOFreqTypes[i]));
                                            }
                                        }
                                        catch (Exception e)
                                        {
                                            // желательно записать влог
                                            _logger.Exception(Contexts.ThisComponent, e);
                                            // этот вызов обязательный в случаи обрыва
                                            context.Abort(e);
                                            // дальше кода быть не должно, освобождаем поток}
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            throw new Exception(command.Parameter.Standart + " option unavailable");
                        }
                        if (Option_EVDO == 1)
                        {
                            if (command.Parameter.Freqs_Hz != null && command.Parameter.Freqs_Hz.Length > 0 &&
                                   command.Parameter.CDMAEVDOFreqTypes != null && command.Parameter.CDMAEVDOFreqTypes.Length > 0 &&
                                   command.Parameter.CDMAEVDOFreqTypes.Length == command.Parameter.Freqs_Hz.Length)
                            {
                                if (command.Parameter.FreqType == COM.Parameters.MesureSystemInfo.FreqType.New)
                                {
                                    for (int i = 0; i < command.Parameter.Freqs_Hz.Length; i++)
                                    {
                                        try
                                        {
                                            if (command.Parameter.CDMAEVDOFreqTypes[i])
                                            {
                                                freqs.Add(GetCDMACHfromFreqDN(command.Parameter.Freqs_Hz[i], command.Parameter.CDMAEVDOFreqTypes[i]));
                                            }
                                        }
                                        catch (Exception e)
                                        {
                                            // желательно записать влог
                                            _logger.Exception(Contexts.ThisComponent, e);
                                            // этот вызов обязательный в случаи обрыва
                                            context.Abort(e);
                                            // дальше кода быть не должно, освобождаем поток}
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            throw new Exception(command.Parameter.Standart + " option unavailable");
                        }
                        if (Option_CDMA == 1 || Option_EVDO == 1)
                        {
                            CDMABTS.Clear();
                            GetUnifreqsCDMA(freqs);
                            CDMAConnect();
                            if (!command.Parameter.PeriodicResult)
                            {
                                _workScheduler.Run(command.Parameter.Standart, (Action)(() =>
                            {
                                List<COMR.MesureSystemInfo.StationSystemInfo> res = new List<COMR.MesureSystemInfo.StationSystemInfo>() { };
                                CDMAUpdateData = false;
                                for (int i = 0; i < CDMABTS.Count; i++)
                                {
                                    if (command.Parameter.ResultOnlyWithGCID)
                                    {
                                        if (CDMABTS[i].FullData)
                                        {
                                            res.Add(CDMABTS[i].StationSysInfo);
                                        }
                                    }
                                    else
                                    {
                                        res.Add(CDMABTS[i].StationSysInfo);
                                    }
                                }
                                COMR.MesureSystemInfoResult msir = new COMR.MesureSystemInfoResult(0, CommandResultStatus.Final)
                                {
                                    DeviceStatus = COMR.Enums.DeviceStatus.Normal,
                                    SystemInfo = res.ToArray()
                                };
                                context.PushResult(msir);
                                CDMADisconnect();
                                context.Unlock();
                                context.Finish();

                            }), (int)command.Parameter.DelayToSendResult_s * 1000);
                            }
                            else
                            {
                                long lastpushrelults = Common.WinAPITime.GetTimeStamp();
                                _workScheduler.Run(command.Parameter.Standart, (Action)(() =>
                                {
                                    long step = (long)(command.Parameter.DelayToSendResult_s * 10000000);
                                    ulong resultid = 0;
                                    while (!context.Token.IsCancellationRequested)
                                    {
                                        Thread.Sleep(10);//чтоб не жрало проц
                                        if ((Common.WinAPITime.GetTimeStamp() - lastpushrelults) > step)
                                        {
                                            lastpushrelults = Common.WinAPITime.GetTimeStamp();
                                            List<COMR.MesureSystemInfo.StationSystemInfo> res = new List<COMR.MesureSystemInfo.StationSystemInfo>() { };

                                            for (int i = 0; i < CDMABTS.Count; i++)
                                            {
                                                if (command.Parameter.ResultOnlyWithGCID)
                                                {
                                                    if (CDMABTS[i].FullData)
                                                    {
                                                        res.Add(CDMABTS[i].StationSysInfo);
                                                    }
                                                }
                                                else
                                                {
                                                    res.Add(CDMABTS[i].StationSysInfo);
                                                }
                                            }
                                            COMR.MesureSystemInfoResult msir = new COMR.MesureSystemInfoResult(resultid, CommandResultStatus.Next)
                                            {
                                                DeviceStatus = COMR.Enums.DeviceStatus.Normal,
                                                SystemInfo = res.ToArray()
                                            };
                                            context.PushResult(msir);
                                            resultid++;
                                        }
                                    }
                                    CDMAUpdateData = false;
                                    CDMADisconnect();
                                    context.Unlock();
                                    context.Finish();
                                }));
                            }
                        }
                    }
                }
                else
                {
                    throw new Exception("Invalid initialize/connect adapter");
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
        private const long TicksBefore1970 = 621355968000000000;

        private bool IsRuning; //GPS
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
        #region TechOnThisScaner
        /// <summary>
        /// 0: не подключались и незнаем 1: доступна 2: нет
        /// </summary>
        private int Option_GSM;


        /// <summary>
        /// 0: не подключались и незнаем 1: доступна 2: нет
        /// </summary>
        private int Option_UMTS;

        /// <summary>
        /// 0: не подключались и незнаем 1: доступна 2: нет
        /// </summary>
        private int Option_LTE;

        /// <summary>
        /// 0: не подключались и незнаем 1: доступна 2: нет
        /// </summary>
        private int Option_CDMA;

        /// <summary>
        /// 0: не подключались и незнаем 1: доступна 2: нет
        /// </summary>
        private int Option_EVDO;

        /// <summary>
        /// 0: не подключались и незнаем 1: доступна 2: нет
        /// </summary>
        private int Option_TETRA;

        /// <summary>
        /// 0: не подключались и незнаем 1: доступна 2: нет
        /// </summary>
        private int Option_RFPS;

        /// <summary>
        /// 0: не подключались и незнаем 1: доступна 2: нет
        /// </summary>
        private int Option_ACD;
        #endregion TechOnThisScaner

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

        static CViComLoader<CViComGsmInterface> gsmLoader;
        static CViComGsmInterface gsmInterface;
        static CViComBasicInterface gsmBasicInterface;
        static CViComGsmInterfaceDataProcessor GSMListener;

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


        static CViComLoader<CViComAcdInterface> acdLoader;
        static CViComAcdInterface acdInterface;
        static CViComBasicInterface acdBasicInterface;
        static CViComAcdInterfaceDataProcessor ACDListener;


        static SSweepSettings rSSweepSettings = new SSweepSettings();

        private static double DetectionLevelGSM = -100;
        private static uint RFInputGSM = 1;
        private static double DetectionLevelUMTS = -100;
        private static uint RFInputUMTS = 1;
        private static double DetectionLevelLTE = -100;
        private static uint RFInputLTE = 1;
        private static double DetectionLevelCDMA = -100;
        private static uint RFInputCDMA = 1;


        private static List<LGSM.BTSData> GSMBTS;
        static bool GSMIsRuning = false;
        static bool GSMUpdateData = false;
        private static List<decimal> GSMUniFreqSelected = new List<decimal>() { };
        public static List<LGSM.BandData> AllGSMBands = new List<LGSM.BandData>() { };
        public static List<LGSM.BandData> GSMBandFreqsSelected = new List<LGSM.BandData>() { };
        private List<SIType> GSMSITypes = new List<SIType>() { };

        private static List<LUMTS.BTSData> UMTSBTS;
        static bool UMTSIsRuning = false;
        static bool UMTSUpdateData = false;
        private static List<decimal> UMTSUniFreq = new List<decimal>() { };
        private List<SIType> UMTSSITypes = new List<SIType>() { };

        private static List<LLTE.BTSData> LTEBTS;
        static bool LTEIsRuning = false;
        static bool LTEUpdateData = false;
        private static List<decimal> LTEUniFreq = new List<decimal>() { };
        private List<SIType> LTESITypes = new List<SIType>() { };

        private static List<LCDMA.BTSData> CDMABTS;
        static bool CDMAIsRuning = false;
        static bool CDMAUpdateData = false;
        private static List<LCDMA.Channel> CDMAUniFreq = new List<LCDMA.Channel>() { };
        private List<SIType> CDMASITypes = new List<SIType>() { };
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
                        _SerialNumber = message.Substring(StartSn, StopSn - StartSn);
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
                //Debug.WriteLine(String.Format("T {0} {1}", DateTime.Now, text) + "\r\n");//System.Console.WriteLine(text);
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
            DetectScanner();
            res = true;
            return res;
        }
        private void DetectScanner()
        {
            try
            {
                // Load
                gpsLoader = new CViComLoader<CViComGpsInterface>(DeviceType);
                receiverListener = new CReceiverListener();
                IsRuning = gpsLoader.Connect(IPAddress, out error, receiverListener);
                if (IsRuning)
                {
                    GpsInterface = gpsLoader.GetInterface();
                    GpsBasicInterface = gpsLoader.GetBasicInterface();
                    Thread.Sleep(500);
                    myReceivers = GpsBasicInterface.GetConnectedReceivers(SDefs.dwDefaultTimeOutInMs);
                    if (myReceivers.Receivers != null & myReceivers.Receivers.Length > 0 && myReceivers.Receivers[0] != null)
                    { option = myReceivers.Receivers[0].ListOfDeviceOptions; }

                    Option_GSM = 0;
                    Option_UMTS = 0;
                    Option_LTE = 0;
                    Option_ACD = 0;
                    Option_CDMA = 0;
                    Option_EVDO = 0;
                    Option_TETRA = 0;
                    Option_RFPS = 0;

                    bool K0 = false;
                    foreach (SConnectedReceiverTable.SReceiver.SDeviceOption op in option)
                    {
                        if (op.boActiveOption && op.pcOptionType.Contains("-K0"))// временная на жире
                        {
                            K0 = true;//все хорошо и активируем все
                            Option_GSM = 1;
                            Option_UMTS = 1;
                            Option_LTE = 1;
                            Option_ACD = 1;
                            Option_CDMA = 1;
                            Option_EVDO = 1;
                            Option_TETRA = 1;
                            Option_RFPS = 1;
                        }
                    }
                    if (!K0)//нету демо опции на жире проверим все, как оказалось не все K0 это на жире...
                    {
                        Option_GSM = 2;
                        Option_UMTS = 2;
                        Option_LTE = 2;
                        Option_ACD = 2;
                        Option_CDMA = 2;
                        Option_EVDO = 2;
                        Option_TETRA = 2;
                        Option_RFPS = 2;
                        if (DeviceType == DeviceType.Tsme || DeviceType == DeviceType.Tsme6)
                        {
                            #region
                            foreach (SConnectedReceiverTable.SReceiver.SDeviceOption op in option)//проверим что есть
                            {
                                if (op.boActiveOption && op.pcOptionType.Contains("-K21"))//UMTS
                                {
                                    Option_UMTS = 1;
                                }
                                else if (op.boActiveOption && op.pcOptionType.Contains("-K22"))//CDMA
                                {
                                    Option_CDMA = 1;
                                }
                                else if (op.boActiveOption && op.pcOptionType.Contains("-K23"))//GSM
                                {
                                    Option_GSM = 1;
                                }
                                else if (op.boActiveOption && op.pcOptionType.Contains("-K24"))//EVDO
                                {
                                    Option_EVDO = 1;
                                }
                                else if (op.boActiveOption && op.pcOptionType.Contains("-K26"))//TETRA
                                {
                                    Option_TETRA = 1;
                                }
                                else if (op.boActiveOption && op.pcOptionType.Contains("-K27"))//RF Power Scan
                                {
                                    Option_RFPS = 1;
                                }
                                else if (op.boActiveOption && op.pcOptionType.Contains("-K29"))//LTE
                                {
                                    Option_LTE = 1;
                                }
                                else if (op.boActiveOption && op.pcOptionType.Contains("-K40"))//ACD
                                {
                                    Option_ACD = 1;
                                }
                            }
                            #endregion
                        }
                        else if (DeviceType == DeviceType.Tsmw)
                        {
                            #region
                            foreach (SConnectedReceiverTable.SReceiver.SDeviceOption op in option)//проверим что есть
                            {
                                if (op.boActiveOption && (op.pcOptionType.Contains("-K21") || op.pcOptionType.Contains("-K121") || op.pcOptionType.Contains("-K221")))//GSM UMTS
                                {
                                    Option_UMTS = 1; Option_GSM = 1;
                                }
                                else if (op.boActiveOption && (op.pcOptionType.Contains("-K22") || op.pcOptionType.Contains("-K122") || op.pcOptionType.Contains("-K222")))//CDMA EVDO
                                {
                                    Option_CDMA = 1;
                                }
                                else if (op.boActiveOption && (op.pcOptionType.Contains("-K27") || op.pcOptionType.Contains("-K127")))//RF Power Scan
                                {
                                    Option_RFPS = 1;
                                }
                                else if (op.boActiveOption && (op.pcOptionType.Contains("-K29") || op.pcOptionType.Contains("-K129")))//LTE
                                {
                                    Option_LTE = 1;
                                }
                                else if (op.boActiveOption && op.pcOptionType.Contains("-K40"))//ACD
                                {
                                    Option_ACD = 1;
                                }
                            }
                            #endregion
                        }
                    }
                    GpsConnect();

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
        #region GPS
        private void GpsConnect()
        {
            try
            {
                ViComMessageTracerRegistry.Register(new MessageTracer());
                //gpsLoader = new CViComLoader<CViComGpsInterface>(DeviceType);
                //gpsLoader.Connect(IPAddress, out error, receiverListener); //GPSIsRuning


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
                IsRuning = true;
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
               


        #region GSM
        private void SetGSMBandData()
        {
            int startarfcn = 0;
            int stoparfcn = 0;
            #region P_GSM900
            startarfcn = 1;
            stoparfcn = 124;
            List<LGSM.BandFreq> fd1 = new List<LGSM.BandFreq>() { };
            for (int i = startarfcn; i <= stoparfcn; i++)
            {
                fd1.Add(new LGSM.BandFreq() { ARFCN = i, FreqDn = (935.2m + 0.2m * (i - startarfcn)) * 1000000, FreqUp = (890.2m + 0.2m * (i - startarfcn)) * 1000000 });
            }
            LGSM.BandData t1 = new LGSM.BandData()
            {
                Band = COM.Parameters.MesureSystemInfo.GSMBands.P_GSM900,
                FreqData = fd1,
                ARFCNStart = fd1[0].ARFCN,
                ARFCNStop = fd1[fd1.Count - 1].ARFCN,
                FreqDnStart = fd1[0].FreqDn,
                FreqDnStop = fd1[fd1.Count - 1].FreqDn,
                FreqUpStart = fd1[0].FreqUp,
                FreqUpStop = fd1[fd1.Count - 1].FreqUp,
            };
            AllGSMBands.Add(t1);
            #endregion
            #region E_GSM900
            startarfcn = 975;
            stoparfcn = 1023;
            List<LGSM.BandFreq> fd2 = new List<LGSM.BandFreq>() { };
            for (int i = startarfcn; i <= stoparfcn; i++)
            {
                fd2.Add(new LGSM.BandFreq() { ARFCN = i, FreqDn = (925.2m + 0.2m * (i - startarfcn)) * 1000000, FreqUp = (880.2m + 0.2m * (i - startarfcn)) * 1000000 });
            }
            startarfcn = 0;
            stoparfcn = 124;
            for (int i = startarfcn; i <= stoparfcn; i++)
            {
                fd2.Add(new LGSM.BandFreq() { ARFCN = i, FreqDn = (935 + 0.2m * (i - startarfcn)) * 1000000, FreqUp = (890 + 0.2m * (i - startarfcn)) * 1000000 });
            }
            LGSM.BandData t2 = new LGSM.BandData()
            {
                Band = COM.Parameters.MesureSystemInfo.GSMBands.E_GSM900,
                FreqData = fd2,
                ARFCNStart = fd2[0].ARFCN,
                ARFCNStop = fd2[fd2.Count - 1].ARFCN,
                FreqDnStart = fd2[0].FreqDn,
                FreqDnStop = fd2[fd2.Count - 1].FreqDn,
                FreqUpStart = fd2[0].FreqUp,
                FreqUpStop = fd2[fd2.Count - 1].FreqUp,
            };
            AllGSMBands.Add(t2);
            #endregion
            #region GSM1800
            startarfcn = 512;
            stoparfcn = 885;
            List<LGSM.BandFreq> fd3 = new List<LGSM.BandFreq>() { };
            for (int i = startarfcn; i <= stoparfcn; i++)
            {
                fd3.Add(new LGSM.BandFreq() { ARFCN = i, FreqDn = (1805.2m + 0.2m * (i - startarfcn)) * 1000000, FreqUp = (1710.2m + 0.2m * (i - startarfcn)) * 1000000 });
            }

            LGSM.BandData t3 = new LGSM.BandData()
            {
                Band = COM.Parameters.MesureSystemInfo.GSMBands.GSM1800,
                FreqData = fd3,
                ARFCNStart = fd3[0].ARFCN,
                ARFCNStop = fd3[fd3.Count - 1].ARFCN,
                FreqDnStart = fd3[0].FreqDn,
                FreqDnStop = fd3[fd3.Count - 1].FreqDn,
                FreqUpStart = fd3[0].FreqUp,
                FreqUpStop = fd3[fd3.Count - 1].FreqUp,
            };
            AllGSMBands.Add(t3);
            #endregion
            #region GSM850
            startarfcn = 128;
            stoparfcn = 251;
            List<LGSM.BandFreq> fd4 = new List<LGSM.BandFreq>() { };
            for (int i = startarfcn; i <= stoparfcn; i++)
            {
                fd4.Add(new LGSM.BandFreq() { ARFCN = i, FreqDn = (869.2m + 0.2m * (i - startarfcn)) * 1000000, FreqUp = (824.2m + 0.2m * (i - startarfcn)) * 1000000 });
            }
            LGSM.BandData t4 = new LGSM.BandData()
            {
                Band = COM.Parameters.MesureSystemInfo.GSMBands.GSM850,
                FreqData = fd4,
                ARFCNStart = fd4[0].ARFCN,
                ARFCNStop = fd4[fd4.Count - 1].ARFCN,
                FreqDnStart = fd4[0].FreqDn,
                FreqDnStop = fd4[fd4.Count - 1].FreqDn,
                FreqUpStart = fd4[0].FreqUp,
                FreqUpStop = fd4[fd4.Count - 1].FreqUp,
            };
            AllGSMBands.Add(t4);
            #endregion
            #region GSM1900
            startarfcn = 512;
            stoparfcn = 810;
            List<LGSM.BandFreq> fd5 = new List<LGSM.BandFreq>() { };
            for (int i = startarfcn; i <= stoparfcn; i++)
            {
                fd5.Add(new LGSM.BandFreq() { ARFCN = i, FreqDn = (1930.2m + 0.2m * (i - startarfcn)) * 1000000, FreqUp = (1850.2m + 0.2m * (i - startarfcn)) * 1000000 });
            }
            LGSM.BandData t5 = new LGSM.BandData()
            {
                Band = COM.Parameters.MesureSystemInfo.GSMBands.GSM1900,
                FreqData = fd5,
                ARFCNStart = fd5[0].ARFCN,
                ARFCNStop = fd5[fd5.Count - 1].ARFCN,
                FreqDnStart = fd5[0].FreqDn,
                FreqDnStop = fd5[fd5.Count - 1].FreqDn,
                FreqUpStart = fd5[0].FreqUp,
                FreqUpStop = fd5[fd5.Count - 1].FreqUp,
            };
            AllGSMBands.Add(t5);
            #endregion
            #region R_GSM900
            startarfcn = 955;
            stoparfcn = 1023;
            List<LGSM.BandFreq> fd6 = new List<LGSM.BandFreq>() { };
            for (int i = startarfcn; i <= stoparfcn; i++)
            {
                fd6.Add(new LGSM.BandFreq() { ARFCN = i, FreqDn = (921.2m + 0.2m * (i - startarfcn)) * 1000000, FreqUp = (876.2m + 0.2m * (i - startarfcn)) * 1000000 });
            }
            startarfcn = 0;
            stoparfcn = 124;
            for (int i = startarfcn; i <= stoparfcn; i++)
            {
                fd6.Add(new LGSM.BandFreq() { ARFCN = i, FreqDn = (935 + 0.2m * (i - startarfcn)) * 1000000, FreqUp = (890 + 0.2m * (i - startarfcn)) * 1000000 });
            }
            LGSM.BandData t6 = new LGSM.BandData()
            {
                Band = COM.Parameters.MesureSystemInfo.GSMBands.R_GSM900,
                FreqData = fd6,
                ARFCNStart = fd6[0].ARFCN,
                ARFCNStop = fd6[fd6.Count - 1].ARFCN,
                FreqDnStart = fd6[0].FreqDn,
                FreqDnStop = fd6[fd6.Count - 1].FreqDn,
                FreqUpStart = fd6[0].FreqUp,
                FreqUpStop = fd6[fd6.Count - 1].FreqUp,
            };
            AllGSMBands.Add(t6);
            #endregion
            #region ER_GSM900
            startarfcn = 940;
            stoparfcn = 1023;
            List<LGSM.BandFreq> fd7 = new List<LGSM.BandFreq>() { };
            for (int i = startarfcn; i <= stoparfcn; i++)
            {
                fd7.Add(new LGSM.BandFreq() { ARFCN = i, FreqDn = (918.2m + 0.2m * (i - startarfcn)) * 1000000, FreqUp = (873.2m + 0.2m * (i - startarfcn)) * 1000000 });
            }
            startarfcn = 0;
            stoparfcn = 124;
            for (int i = startarfcn; i <= stoparfcn; i++)
            {
                fd7.Add(new LGSM.BandFreq() { ARFCN = i, FreqDn = (935 + 0.2m * (i - startarfcn)) * 1000000, FreqUp = (890 + 0.2m * (i - startarfcn)) * 1000000 });
            }
            LGSM.BandData t7 = new LGSM.BandData()
            {
                Band = COM.Parameters.MesureSystemInfo.GSMBands.ER_GSM900,
                FreqData = fd7,
                ARFCNStart = fd7[0].ARFCN,
                ARFCNStop = fd7[fd7.Count - 1].ARFCN,
                FreqDnStart = fd7[0].FreqDn,
                FreqDnStop = fd7[fd7.Count - 1].FreqDn,
                FreqUpStart = fd7[0].FreqUp,
                FreqUpStop = fd7[fd7.Count - 1].FreqUp,
            };
            AllGSMBands.Add(t7);
            #endregion
            #region GSM750
            startarfcn = 438;
            stoparfcn = 511;
            List<LGSM.BandFreq> fd8 = new List<LGSM.BandFreq>() { };
            for (int i = startarfcn; i <= stoparfcn; i++)
            {
                fd8.Add(new LGSM.BandFreq() { ARFCN = i, FreqDn = (747.2m + 0.2m * (i - startarfcn)) * 1000000, FreqUp = (777.2m + 0.2m * (i - startarfcn)) * 1000000 });
            }
            LGSM.BandData t8 = new LGSM.BandData()
            {
                Band = COM.Parameters.MesureSystemInfo.GSMBands.GSM750,
                FreqData = fd8,
                ARFCNStart = fd8[0].ARFCN,
                ARFCNStop = fd8[fd8.Count - 1].ARFCN,
                FreqDnStart = fd8[0].FreqDn,
                FreqDnStop = fd8[fd8.Count - 1].FreqDn,
                FreqUpStart = fd8[0].FreqUp,
                FreqUpStop = fd8[fd8.Count - 1].FreqUp,
            };
            AllGSMBands.Add(t8);
            #endregion
            #region GSM450
            startarfcn = 259;
            stoparfcn = 293;
            List<LGSM.BandFreq> fd9 = new List<LGSM.BandFreq>() { };
            for (int i = startarfcn; i <= stoparfcn; i++)
            {
                fd9.Add(new LGSM.BandFreq() { ARFCN = i, FreqDn = (460.6m + 0.2m * (i - startarfcn)) * 1000000, FreqUp = (450.6m + 0.2m * (i - startarfcn)) * 1000000 });
            }
            LGSM.BandData t9 = new LGSM.BandData()
            {
                Band = COM.Parameters.MesureSystemInfo.GSMBands.GSM450,
                FreqData = fd9,
                ARFCNStart = fd9[0].ARFCN,
                ARFCNStop = fd9[fd9.Count - 1].ARFCN,
                FreqDnStart = fd9[0].FreqDn,
                FreqDnStop = fd9[fd9.Count - 1].FreqDn,
                FreqUpStart = fd9[0].FreqUp,
                FreqUpStop = fd9[fd9.Count - 1].FreqUp,
            };
            AllGSMBands.Add(t9);
            #endregion
            #region GSM480
            startarfcn = 306;
            stoparfcn = 340;
            List<LGSM.BandFreq> fd10 = new List<LGSM.BandFreq>() { };
            for (int i = startarfcn; i <= stoparfcn; i++)
            {
                fd10.Add(new LGSM.BandFreq() { ARFCN = i, FreqDn = (489m + 0.2m * (i - startarfcn)) * 1000000, FreqUp = (479m + 0.2m * (i - startarfcn)) * 1000000 });
            }
            LGSM.BandData t10 = new LGSM.BandData()
            {
                Band = COM.Parameters.MesureSystemInfo.GSMBands.GSM480,
                FreqData = fd10,
                ARFCNStart = fd10[0].ARFCN,
                ARFCNStop = fd10[fd10.Count - 1].ARFCN,
                FreqDnStart = fd10[0].FreqDn,
                FreqDnStop = fd10[fd10.Count - 1].FreqDn,
                FreqUpStart = fd10[0].FreqUp,
                FreqUpStop = fd10[fd10.Count - 1].FreqUp,
            };
            AllGSMBands.Add(t10);
            #endregion
        }
        private void GsmConnect()
        {
            try
            {
                gsmLoader = new CViComLoader<RohdeSchwarz.ViCom.Net.GSM.CViComGsmInterface>(DeviceType);

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
                    channelSettings.dwFrontEndSelectionMask = RFInputGSM; //вроде как канал приемника 1/2
                    channelSettings.dwMeasRatePer1000Sec = 250000;// 50000;// ((RohdeSchwarz.ViCom.Net.SRange<uint>)gsmInterface.GetMeasRateLimits()).minimum; //rateLimit.maximum; //вроде как скорость сканирования
                    channelSettings.dwCount = (uint)GSMUniFreqSelected.Count();
                    channelSettings.pTableOfFrequencySetting = new RohdeSchwarz.ViCom.Net.GSM.SFrequencySetting[channelSettings.dwCount];


                    for (int i = 0; i < GSMUniFreqSelected.Count; i++)
                    {
                        channelSettings.pTableOfFrequencySetting[i] = new RohdeSchwarz.ViCom.Net.GSM.SFrequencySetting();
                        channelSettings.pTableOfFrequencySetting[i].dCenterFrequencyInHz = (double)GSMUniFreqSelected[i];
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
                    uint dwRequests = (uint)(siblist.Count * GSMUniFreqSelected.Count());
                    demod.dwFrontEndSelectionMask = RFInputGSM;
                    demod.lTotalPowerOffsetInDB10 = 100;

                    RohdeSchwarz.ViCom.Net.GSM.SDemodRequests MeasurementRequests = new RohdeSchwarz.ViCom.Net.GSM.SDemodRequests();
                    MeasurementRequests.dwCountOfRequests = dwRequests;
                    MeasurementRequests.pDemodRequests = new RohdeSchwarz.ViCom.Net.GSM.SDemodRequests.SDemodRequest[dwRequests];

                    for (int i = 0; i < GSMUniFreqSelected.Count(); i++)
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
                    GSMUpdateData = GSMIsRuning;
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
                        GSMIsRuning = !gsmLoader.Disconnect();
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
        private void GetUnifreqsGSM(List<decimal> freqs, List<LGSM.BandData> bands)
        {
            try
            {
                #region GSM 
                List<decimal> freq1 = freqs;
                GSMBandFreqsSelected = bands;
                foreach (LGSM.BandData t in GSMBandFreqsSelected)
                {
                    for (int i = 0; i < t.FreqData.Count; i++)
                    {
                        freq1.Add(t.FreqData[i].FreqDn);
                    }
                }
                System.Collections.Generic.HashSet<decimal> hs1 = new System.Collections.Generic.HashSet<decimal>();
                foreach (decimal al in freq1)
                {
                    hs1.Add(al);
                }
                GSMUniFreqSelected.Clear();
                GSMUniFreqSelected = new List<decimal>(hs1.OrderBy(i => i));


                GSMSITypes = new List<SIType>()
                {
                    new SIType()
                    {
                        SiType = RohdeSchwarz.ViCom.Net.GSM.Pdu.Type.SITYPE_1.ToString(),
                        Use = true
                    },
                    new SIType()
                    {
                        SiType = RohdeSchwarz.ViCom.Net.GSM.Pdu.Type.SITYPE_3.ToString(),
                        Use = true
                    }
                };
                #endregion
            }
            catch (Exception e)
            {

            }
        }
        #endregion GSM

        #region UMTS
        private void UmtsConnect()
        {
            try
            {
                wcdmaLoader = new CViComLoader<RohdeSchwarz.ViCom.Net.WCDMA.CViComWcdmaInterface>(DeviceType);

                ViComMessageTracerRegistry.Register(new MessageTracer());
                bool Runing = wcdmaLoader.Connect(IPAddress, out error, receiverListener);

                wcdmaInterface = wcdmaLoader.GetInterface();

                wcdmaBasicInterface = wcdmaLoader.GetBasicInterface();

                var channelConfig = new RohdeSchwarz.ViCom.Net.WCDMA.SChannelSettings();
                channelConfig.dwFrontEndSelectionMask = RFInputUMTS;
                channelConfig.eMeasurementMode = RohdeSchwarz.ViCom.Net.WCDMA.MeasurementMode.Type.HIGH_DYNAMIC;
                channelConfig.dwMeasRatePer1000Sec = 40000;// 5000;//((RohdeSchwarz.ViCom.Net.SRange<uint>)wcdmaInterface.GetMeasRateLimits()).defaultValue;//10000;//
                uint freqs = (uint)UMTSUniFreq.Count(); //UMTSUniFreq
                channelConfig.pTableOfFrequencySetting = new RohdeSchwarz.ViCom.Net.WCDMA.SFrequencySetting[freqs];
                channelConfig.dwCount = (uint)channelConfig.pTableOfFrequencySetting.Length;
                for (int i = 0; i < freqs; i++)
                {
                    channelConfig.pTableOfFrequencySetting[i] = new RohdeSchwarz.ViCom.Net.WCDMA.SFrequencySetting();
                    channelConfig.pTableOfFrequencySetting[i].dCenterFrequencyInHz = (double)(UMTSUniFreq[i]);
                }
                wcdmaInterface.SetFrequencyTable(channelConfig);


                List<RohdeSchwarz.ViCom.Net.WCDMA.Pdu.Type> siblist = new List<RohdeSchwarz.ViCom.Net.WCDMA.Pdu.Type>() { };
                for (int i = 0; i < UMTSSITypes.Count; i++)
                {
                    if (UMTSSITypes[i].Use)
                    {
                        siblist.Add((RohdeSchwarz.ViCom.Net.WCDMA.Pdu.Type)Enum.Parse(typeof(RohdeSchwarz.ViCom.Net.WCDMA.Pdu.Type), UMTSSITypes[i].SiType));
                    }
                }


                RohdeSchwarz.ViCom.Net.WCDMA.SDemodulationSettings demod = new RohdeSchwarz.ViCom.Net.WCDMA.SDemodulationSettings();
                demod.dwFrontEndSelectionMask = RFInputUMTS;
                demod.lEcToIoThresholdInDB100 = -1500;// -1500;
                demod.dwMaxNodeBHoldTimeInSec = RohdeSchwarz.ViCom.Net.WCDMA.SDemodulationSettings.dwMinMaxNodeBHoldTimeInSec;

                RohdeSchwarz.ViCom.Net.WCDMA.SDemodRequests MeasurementRequests = new RohdeSchwarz.ViCom.Net.WCDMA.SDemodRequests();


                uint dwRequests = (uint)siblist.Count * freqs;
                MeasurementRequests.dwCountOfRequests = dwRequests;

                MeasurementRequests.pDemodRequests = new RohdeSchwarz.ViCom.Net.WCDMA.SDemodRequests.SDemodRequest[dwRequests];

                for (int i = 0; i < freqs; i++)
                {
                    int dwRequestStartIndex = i * siblist.Count;
                    for (int idx = 0; idx < siblist.Count; ++idx)
                    {
                        int iR = dwRequestStartIndex + idx;
                        MeasurementRequests.pDemodRequests[iR] = new RohdeSchwarz.ViCom.Net.WCDMA.SDemodRequests.SDemodRequest();
                        MeasurementRequests.pDemodRequests[iR].dwChannelIndex = (uint)i;
                        MeasurementRequests.pDemodRequests[iR].ePDU = siblist[idx];
                        MeasurementRequests.pDemodRequests[iR].eDemodulationMode = RohdeSchwarz.ViCom.Net.WCDMA.DemodMode.Type.ONCE;
                        MeasurementRequests.pDemodRequests[iR].wRepetitionDelayIn100ms = 0;//1000;
                    }
                }
                demod.sStartMeasurementRequests = MeasurementRequests;
                wcdmaInterface.SetDemodulationSettings(demod);
                WCDMAListener = new MyWcdmaDataProcessor(_logger, _timeService);
                wcdmaInterface.RegisterResultDataListener(WCDMAListener);
                wcdmaBasicInterface.StartMeasurement();
                UMTSIsRuning = Runing;
                UMTSUpdateData = UMTSIsRuning;
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
        private void UmtsDisconnect()
        {
            try
            {
                if (wcdmaLoader != null)
                {
                    if (wcdmaLoader.GetBasicInterface().IsMeasurementStarted())
                    {
                        wcdmaLoader.GetBasicInterface().StopMeasurement();
                        wcdmaLoader.GetBasicInterface().HasMeasurementStopped(SDefs.dwDefaultTimeOutInMs);
                        wcdmaInterface.UnregisterResultDataListener(WCDMAListener);
                        UMTSIsRuning = !wcdmaLoader.Disconnect();
                        UMTSUpdateData = UMTSIsRuning;
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
        private static void UMTSSetFreqFromACD(ILogger _logger)
        {
            try
            {
                //if (wcdmaLoader != null && wcdmaLoader.GetBasicInterface().IsMeasurementStarted())
                //{
                //    wcdmaLoader.GetBasicInterface().StopMeasurement();
                //}
                //if (wcdmaLoader.GetBasicInterface().HasMeasurementStopped(SDefs.dwDefaultTimeOutInMs))
                //{
                //    var channelConfig = wcdmaInterface.GetSettings().ChannelSettings;
                //    channelConfig.dwFrontEndSelectionMask = GetDeviceRFInput(true, 1, "UMTS");
                //    channelConfig.eMeasurementMode = RohdeSchwarz.ViCom.Net.WCDMA.MeasurementMode.Type.HIGH_DYNAMIC;
                //    channelConfig.dwMeasRatePer1000Sec = 40000;// 5000;//((RohdeSchwarz.ViCom.Net.SRange<uint>)wcdmaInterface.GetMeasRateLimits()).defaultValue;//10000;//

                //    #region Freqs 
                //    List<decimal> freq = new List<decimal>() { };
                //    foreach (decimal t in UMTSUniFreq)
                //    {
                //        freq.Add(t);
                //    }
                //    for (int i = 0; i < IdentificationData.ACD.ACDData.Count(); i++)
                //    {
                //        if (IdentificationData.ACD.ACDData[i].Tech == 2 && IdentificationData.ACD.ACDData[i].Established == false)
                //        {
                //            freq.Add(IdentificationData.ACD.ACDData[i].Freq);
                //            IdentificationData.ACD.ACDData[i].Established = true;
                //        }
                //    }
                //    System.Collections.Generic.HashSet<decimal> hs2 = new System.Collections.Generic.HashSet<decimal>();
                //    foreach (decimal al in freq)
                //    {
                //        hs2.Add(al);
                //    }
                //    UMTSUniFreq.Clear();
                //    UMTSUniFreq = new ObservableCollection<decimal>(hs2);
                //    #endregion

                //    uint freqs = (uint)UMTSUniFreq.Count(); //UMTSUniFreq
                //    channelConfig.pTableOfFrequencySetting = new RohdeSchwarz.ViCom.Net.WCDMA.SFrequencySetting[freqs];
                //    channelConfig.dwCount = (uint)channelConfig.pTableOfFrequencySetting.Length;
                //    for (int i = 0; i < freqs; i++)
                //    {
                //        channelConfig.pTableOfFrequencySetting[i] = new RohdeSchwarz.ViCom.Net.WCDMA.SFrequencySetting();
                //        channelConfig.pTableOfFrequencySetting[i].dCenterFrequencyInHz = (double)(UMTSUniFreq[i]);
                //    }
                //    wcdmaInterface.SetFrequencyTable(channelConfig);


                //    List<RohdeSchwarz.ViCom.Net.WCDMA.Pdu.Type> siblist = new List<RohdeSchwarz.ViCom.Net.WCDMA.Pdu.Type>() { };
                //    for (int i = 0; i < App.Sett.TSMxReceiver_Settings.UMTS.SIBTypes.Count; i++)
                //    { if (App.Sett.TSMxReceiver_Settings.UMTS.SIBTypes[i].Use) { siblist.Add((RohdeSchwarz.ViCom.Net.WCDMA.Pdu.Type)Enum.Parse(typeof(RohdeSchwarz.ViCom.Net.WCDMA.Pdu.Type), App.Sett.TSMxReceiver_Settings.UMTS.SIBTypes[i].SibType)); } }


                //    RohdeSchwarz.ViCom.Net.WCDMA.SDemodulationSettings demod = wcdmaInterface.GetSettings().DemodulationSettings;
                //    demod.dwFrontEndSelectionMask = GetDeviceRFInput(true, 1, "UMTS");
                //    demod.lEcToIoThresholdInDB100 = -1500;
                //    demod.dwMaxNodeBHoldTimeInSec = RohdeSchwarz.ViCom.Net.WCDMA.SDemodulationSettings.dwMinMaxNodeBHoldTimeInSec;

                //    RohdeSchwarz.ViCom.Net.WCDMA.SDemodRequests MeasurementRequests = new RohdeSchwarz.ViCom.Net.WCDMA.SDemodRequests();
                //    //demod.sStartMeasurementRequests = new RohdeSchwarz.ViCom.Net.WCDMA.SDemodRequests();

                //    uint dwRequests = (uint)siblist.Count * freqs;
                //    MeasurementRequests.dwCountOfRequests = dwRequests;

                //    MeasurementRequests.pDemodRequests = new RohdeSchwarz.ViCom.Net.WCDMA.SDemodRequests.SDemodRequest[dwRequests];
                //    //for (int i = 0; i < dwRequests; i++)
                //    //{

                //    //}
                //    for (int i = 0; i < freqs; i++)
                //    {
                //        int dwRequestStartIndex = i * siblist.Count;
                //        for (int idx = 0; idx < siblist.Count; ++idx)
                //        {
                //            int iR = dwRequestStartIndex + idx;
                //            MeasurementRequests.pDemodRequests[iR] = new RohdeSchwarz.ViCom.Net.WCDMA.SDemodRequests.SDemodRequest();
                //            MeasurementRequests.pDemodRequests[iR].dwChannelIndex = (uint)i;
                //            MeasurementRequests.pDemodRequests[iR].ePDU = siblist[idx];
                //            MeasurementRequests.pDemodRequests[iR].eDemodulationMode = RohdeSchwarz.ViCom.Net.WCDMA.DemodMode.Type.ONCE;
                //            MeasurementRequests.pDemodRequests[iR].wRepetitionDelayIn100ms = 0;//1000;
                //        }
                //    }
                //    demod.sStartMeasurementRequests = MeasurementRequests;
                //    wcdmaInterface.SetDemodulationSettings(demod);

                //    wcdmaBasicInterface.StartMeasurement();
                //    DM -= UMTSSetFreqFromACD;
                //}
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
        private void GetUnifreqsUMTS(List<decimal> freqs)
        {
            #region UMTS 
            System.Collections.Generic.HashSet<decimal> hs2 = new System.Collections.Generic.HashSet<decimal>();
            foreach (decimal al in freqs)
            {
                hs2.Add(al);
            }
            UMTSUniFreq.Clear();
            UMTSUniFreq = new List<decimal>(hs2.OrderBy(i => i));

            UMTSSITypes = new List<SIType>()
            {
                new SIType()
                {
                    SiType = RohdeSchwarz.ViCom.Net.WCDMA.Pdu.Type.MIB.ToString(),
                    Use = true
                },
                new SIType()
                {
                    SiType = RohdeSchwarz.ViCom.Net.WCDMA.Pdu.Type.SIB1.ToString(),
                    Use = true
                },
                new SIType()
                {
                    SiType = RohdeSchwarz.ViCom.Net.WCDMA.Pdu.Type.SIB2.ToString(),
                    Use = true
                },
                new SIType()
                {
                    SiType = RohdeSchwarz.ViCom.Net.WCDMA.Pdu.Type.SIB3.ToString(),
                    Use = true
                }
            };
            #endregion
        }
        #endregion

        #region LTE        
        private void LTEConnect()
        {
            try
            {
                lteLoader = new CViComLoader<RohdeSchwarz.ViCom.Net.LTE.CViComLteInterface>(DeviceType);
                ViComMessageTracerRegistry.Register(new MessageTracer());
                bool Runing = lteLoader.Connect(IPAddress, out error, receiverListener);

                lteInterface = lteLoader.GetInterface();
                lteBasicInterface = lteLoader.GetBasicInterface();

                var resultBufferDepth = new SResultBufferDepth();
                resultBufferDepth.dwValue = 1024;
                lteBasicInterface.SetResultBufferDepth(resultBufferDepth);

                var channelSettings = new RohdeSchwarz.ViCom.Net.LTE.SChannelSettings();

                // Setup general DLAA operation
                //int[] dwarObservationInterval = new[] { 20, 60 }; // { 20, 60, 300, 600, 900 };                   
                //channelSettings.dlaaSettings.dwObservationCount = (uint)dwarObservationInterval.Length;
                //channelSettings.dlaaSettings.pTableOfObservationSettings
                //   = new RohdeSchwarz.ViCom.Net.LTE.SChannelSettings.SDlaaSettings.SObservationSettings[channelSettings.dlaaSettings.dwObservationCount];
                //for (uint dwI = 0; dwI < channelSettings.dlaaSettings.dwObservationCount; dwI++)
                //{
                //   channelSettings.dlaaSettings.pTableOfObservationSettings[dwI] = new RohdeSchwarz.ViCom.Net.LTE.SChannelSettings.SDlaaSettings.SObservationSettings();
                //   channelSettings.dlaaSettings.pTableOfObservationSettings[dwI].dwObservationIntervalInS = (uint)dwarObservationInterval[dwI];
                //}

                //
                // Setup TDD interference analysis
                //
                channelSettings.bTddInterferenceKpiThresholdInPct = 0;

                //
                // Setup Frequency table interference analysis
                //

                // Front End Selection Mask
                //uint dwFE_Mask = (uint)RohdeSchwarz.ViCom.Net.SRFPort.Type.RF_1;
                uint freqs = (uint)LTEUniFreq.Count(); //UMTSUniFreq
                channelSettings.pTableOfFrequencySetting = new RohdeSchwarz.ViCom.Net.LTE.SFrequencySetting[freqs];
                channelSettings.dwCount = freqs;

                // Measurement mask for NB Reference Signals
                int iNarrowbandRefSignalMeasMode = (RohdeSchwarz.ViCom.Net.LTE.SFrequencySetting.wNARROWBAND_RSRP_RSRQ |
                                                     RohdeSchwarz.ViCom.Net.LTE.SFrequencySetting.wCENTER_RSCINR_1x1080KHZ |
                                                     RohdeSchwarz.ViCom.Net.LTE.SFrequencySetting.w1MHZ_FILTER_NOISE_FOR_15RB);
                for (int i = 0; i < freqs; i++)
                {
                    #region freq set
                    channelSettings.pTableOfFrequencySetting[i] = new RohdeSchwarz.ViCom.Net.LTE.SFrequencySetting();
                    channelSettings.pTableOfFrequencySetting[i].dwFrontEndSelectionMask = RFInputLTE;
                    channelSettings.pTableOfFrequencySetting[i].dCenterFrequencyInHz = (double)LTEUniFreq[i];
                    channelSettings.pTableOfFrequencySetting[i].dwSymbolsPerSlotMask = RohdeSchwarz.ViCom.Net.LTE.SFrequencySetting.dwDefaultSymbolsPerSlot;
                    channelSettings.pTableOfFrequencySetting[i].enFrameStructureType = FrameStructureType.Type.FDD;
                    channelSettings.pTableOfFrequencySetting[i].bUpDownLinkMask = RohdeSchwarz.ViCom.Net.LTE.SFrequencySetting.bAllUpDownLinkConfigurations;
                    channelSettings.pTableOfFrequencySetting[i].wSpecialSubframe1ConfigurationMask = RohdeSchwarz.ViCom.Net.LTE.SFrequencySetting.wAllSpecialSubFrameConfigurations;
                    channelSettings.pTableOfFrequencySetting[i].wSpecialSubframe6ConfigurationMask = RohdeSchwarz.ViCom.Net.LTE.SFrequencySetting.wAllSpecialSubFrameConfigurations;
                    channelSettings.pTableOfFrequencySetting[i].dwAvgBlockCountPer1000Sec = 2000;// 300;//((RohdeSchwarz.ViCom.Net.SRange<uint>)lteInterface.GetMeasRateLimits()).minimum;////////////////////////;
                    channelSettings.pTableOfFrequencySetting[i].enSSyncToPSyncRatioType = RohdeSchwarz.ViCom.Net.LTE.SFrequencySetting.SSyncToPSyncRatioType.Type.RatioRange;
                    channelSettings.pTableOfFrequencySetting[i].SSyncToPSyncRatio = new RohdeSchwarz.ViCom.Net.LTE.SFrequencySetting.SSyncToPSyncRatioSettings();
                    channelSettings.pTableOfFrequencySetting[i].SSyncToPSyncRatio.RatioRange = new RohdeSchwarz.ViCom.Net.LTE.SFrequencySetting.SSyncToPSyncRatioSettings._StructRatioRange();
                    channelSettings.pTableOfFrequencySetting[i].SSyncToPSyncRatio.RatioRange.fLowerRatioInDB = -5;
                    channelSettings.pTableOfFrequencySetting[i].SSyncToPSyncRatio.RatioRange.fUpperRatioInDB = 0;
                    channelSettings.pTableOfFrequencySetting[i].wNarrowbandRefSignalMeasMode = (ushort)iNarrowbandRefSignalMeasMode;
                    channelSettings.pTableOfFrequencySetting[i].enBandwidthCtrlMode = RohdeSchwarz.ViCom.Net.LTE.SFrequencySetting.BandwidthCtrlMode.Type.BW_FROM_MIB_ONCE_EACH_CELL;
                    //channelSettings.pTableOfFrequencySetting[i].wNumberOfResourceBlocks = 50;
                    channelSettings.pTableOfFrequencySetting[i].WidebandRsCinrSettings = new RohdeSchwarz.ViCom.Net.LTE.SFrequencySetting.SWidebandRsCinrSettings();
                    channelSettings.pTableOfFrequencySetting[i].WidebandRsCinrSettings.wWidebandRsCinrMeasMode = RohdeSchwarz.ViCom.Net.LTE.SFrequencySetting.SWidebandRsCinrSettings.wWIDEBAND_RS_CINR;
                    channelSettings.pTableOfFrequencySetting[i].WidebandRsCinrSettings.dwAvgBlockCountPer1000Sec = 1000;// 300;//((RohdeSchwarz.ViCom.Net.SRange<uint>)lteInterface.GetWbMeasRateLimits()).minimum;////////////////////////;
                    channelSettings.pTableOfFrequencySetting[i].WidebandRsCinrSettings.wNumberOfRBsInSubband = RohdeSchwarz.ViCom.Net.LTE.SFrequencySetting.SWidebandRsCinrSettings.wMinRBsInSubband;
                    channelSettings.pTableOfFrequencySetting[i].WidebandRsCinrSettings.dwFrontEndSelectionMask = RFInputLTE;
                    channelSettings.pTableOfFrequencySetting[i].WidebandRsCinrSettings.bForceNoGap = true;
                    channelSettings.pTableOfFrequencySetting[i].WidebandRsCinrSettings.bMaxCountOfeNodeBs = 6;
                    channelSettings.pTableOfFrequencySetting[i].WidebandRsCinrSettings.bTransmitAntennaSelectionMask = 15;
                    channelSettings.pTableOfFrequencySetting[i].WidebandRsCinrSettings.sMinCenterRsrpInDBm100 = -13000;
                    channelSettings.pTableOfFrequencySetting[i].WidebandRsCinrSettings.wMaxRsrpDiffToBestCellInDB100 = 500;
                    channelSettings.pTableOfFrequencySetting[i].enMbmsConfigCtrlMode = RohdeSchwarz.ViCom.Net.LTE.SFrequencySetting.MbmsConfigCtrlMode.Type.MBMS_NOT_PRESENT;
                    channelSettings.pTableOfFrequencySetting[i].MimoSettings = new RohdeSchwarz.ViCom.Net.LTE.SFrequencySetting.SMimoSettings();
                    channelSettings.pTableOfFrequencySetting[i].MimoSettings.wMimoMeasMode = 0;
                    channelSettings.pTableOfFrequencySetting[i].MimoSettings.dwMimoResultMaskFor2x2 = 7;
                    channelSettings.pTableOfFrequencySetting[i].MimoSettings.dwMimoResultMaskFor2x4 = 0;
                    channelSettings.pTableOfFrequencySetting[i].MimoSettings.bMaxCountOfeNodeBs = 5;
                    channelSettings.pTableOfFrequencySetting[i].MimoSettings.sCinrThresholdForRankInDB100 = 0;
                    channelSettings.pTableOfFrequencySetting[i].MimoSettings.sMinCenterRsrpInDBm100 = -13000;
                    channelSettings.pTableOfFrequencySetting[i].MimoSettings.sMinRsCinrInDB100 = -1000;
                    channelSettings.pTableOfFrequencySetting[i].MimoSettings.wMaxRsrpDiffToBestCellInDB100 = 500;
                    channelSettings.pTableOfFrequencySetting[i].MimoSettings.wTimeResolutionInMs = 10;
                    // Setup Throughput estimation           
                    channelSettings.pTableOfFrequencySetting[i].MimoSettings.bEnableThroughputEstimation = false;
                    channelSettings.pTableOfFrequencySetting[i].RssiSettings = new RohdeSchwarz.ViCom.Net.LTE.SFrequencySetting.SRssiSettings();
                    channelSettings.pTableOfFrequencySetting[i].RssiSettings.dwFrontEndSelectionMask = RFInputLTE;
                    channelSettings.pTableOfFrequencySetting[i].RssiSettings.wRssiMeasMode = 0;
                    channelSettings.pTableOfFrequencySetting[i].MbmsSettings.wMbmsMeasMode = 0;
                    #endregion
                }
                channelSettings.dwCount = freqs;

                channelSettings.RsCinrChannelModel = new RohdeSchwarz.ViCom.Net.LTE.SChannelSettings.SRsCinrChannelModel();
                channelSettings.RsCinrChannelModel.dwDelaySpreadInNs = RohdeSchwarz.ViCom.Net.LTE.SChannelSettings.SRsCinrChannelModel.dwMinDelaySpreadInNs;
                channelSettings.RsCinrChannelModel.dwSpeedInKmPerHour = RohdeSchwarz.ViCom.Net.LTE.SChannelSettings.SRsCinrChannelModel.dwMinSpeedInKmh;

                channelSettings.MbmsRsCinrChannelModel.dwDelaySpreadInNs = RohdeSchwarz.ViCom.Net.LTE.SChannelSettings.SMbmsRsCinrChannelModel.dwMinDelaySpreadInNs;
                channelSettings.MbmsRsCinrChannelModel.dwSpeedInKmPerHour = RohdeSchwarz.ViCom.Net.LTE.SChannelSettings.SMbmsRsCinrChannelModel.dwMinSpeedInKmh;

                List<RohdeSchwarz.ViCom.Net.LTE.Pdu.Type> siblist = new List<RohdeSchwarz.ViCom.Net.LTE.Pdu.Type>() { };
                for (int i = 0; i < LTESITypes.Count; i++)
                { if (LTESITypes[i].Use) { siblist.Add((RohdeSchwarz.ViCom.Net.LTE.Pdu.Type)Enum.Parse(typeof(RohdeSchwarz.ViCom.Net.LTE.Pdu.Type), LTESITypes[i].SiType)); } }

                // Activate BCH demodulation
                uint dwRequests = (uint)siblist.Count * channelSettings.dwCount;
                RohdeSchwarz.ViCom.Net.LTE.SDemodulationSettings bchSettings = new RohdeSchwarz.ViCom.Net.LTE.SDemodulationSettings();
                bchSettings.sSINRThresholdDB100 = 100;// ((RohdeSchwarz.ViCom.Net.SRange<short>)lteInterface.GetDemodThresholdLimits()).minimum;
                bchSettings.dwFrontEndSelectionMask = RFInputLTE; //dwFE_Mask;

                bchSettings.sStartMeasurementRequests.dwCountOfRequests = dwRequests;
                bchSettings.sStartMeasurementRequests.pDemodRequests = new RohdeSchwarz.ViCom.Net.LTE.SDemodRequests.SDemodRequest[dwRequests];
                for (int i = 0; i < dwRequests; i++)
                {
                    bchSettings.sStartMeasurementRequests.pDemodRequests[i] = new RohdeSchwarz.ViCom.Net.LTE.SDemodRequests.SDemodRequest();
                }
                for (int i = 0; i < freqs; i++)
                {
                    int dwRequestStartIndex = i * siblist.Count;
                    for (int idx = 0; idx < siblist.Count; ++idx)
                    {
                        int iR = dwRequestStartIndex + idx;
                        bchSettings.sStartMeasurementRequests.pDemodRequests[iR].dwChannelIndex = (uint)i;
                        bchSettings.sStartMeasurementRequests.pDemodRequests[iR].ePDU = siblist[idx];
                        bchSettings.sStartMeasurementRequests.pDemodRequests[iR].eDemodulationMode = RohdeSchwarz.ViCom.Net.LTE.DemodMode.Type.ONCE;
                        bchSettings.sStartMeasurementRequests.pDemodRequests[iR].wRepetitionTimeOutInMs = 0;// 100;
                                                                                                            //bchSettings.sStartMeasurementRequests.pDemodRequests[iR].dwBtsId = 0;
                    }
                }

                lteInterface.SetFrequencyTable(channelSettings);

                lteInterface.SetDemodulationSettings(bchSettings);

                LteListener = new MyLteDataProcessor(_logger, _timeService);

                lteInterface.RegisterResultDataListener(LteListener);

                lteBasicInterface.StartMeasurement();
                LTEIsRuning = Runing;
                LTEUpdateData = LTEIsRuning;
            }
            catch (RohdeSchwarz.ViCom.Net.CViComError error)
            {
                _logger.Exception(Contexts.ThisComponent, new Exception("ErrorCode:" + error.ErrorCode + " ErrorString:" + error.ErrorString));
            }
            catch (Exception exp)
            {
                _logger.Exception(Contexts.ThisComponent, exp);
            }
        }
        private void LTEDisconnect()
        {
            try
            {
                if (lteLoader != null)
                {
                    if (lteLoader.GetBasicInterface().IsMeasurementStarted())
                    {
                        lteLoader.GetBasicInterface().StopMeasurement();
                        lteLoader.GetBasicInterface().HasMeasurementStopped(SDefs.dwDefaultTimeOutInMs);
                        lteInterface.UnregisterResultDataListener(LteListener);
                        LTEIsRuning = !lteLoader.Disconnect();
                        lteLoader.Dispose();
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
        private static void LTESetFreqFromACD()
        {
            //try
            //{
            //    ////IdentificationData.ACD.ACDData
            //    //if (lteLoader != null && lteLoader.GetBasicInterface().IsMeasurementStarted())
            //    //{
            //    //    lteLoader.GetBasicInterface().StopMeasurement();
            //    //}
            //    //if (lteLoader.GetBasicInterface().HasMeasurementStopped(SDefs.dwDefaultTimeOutInMs))
            //    //{
            //    //    var ChannelSettings = lteInterface.GetSettings().ChannelSettings;
            //    //    ChannelSettings.bTddInterferenceKpiThresholdInPct = 0;

            //    //    #region Freqs 
            //    //    List<decimal> freq = new List<decimal>() { };
            //    //    foreach (decimal t in LTEUniFreq)
            //    //    {
            //    //        freq.Add(t);
            //    //    }
            //    //    for (int i = 0; i < IdentificationData.ACD.ACDData.Count(); i++)
            //    //    {
            //    //        if (IdentificationData.ACD.ACDData[i].Tech == 5 && IdentificationData.ACD.ACDData[i].Established == false)
            //    //        {
            //    //            freq.Add(IdentificationData.ACD.ACDData[i].Freq);
            //    //            IdentificationData.ACD.ACDData[i].Established = true;
            //    //        }
            //    //    }
            //    //    System.Collections.Generic.HashSet<decimal> hs2 = new System.Collections.Generic.HashSet<decimal>();
            //    //    foreach (decimal al in freq)
            //    //    {
            //    //        hs2.Add(al);
            //    //    }
            //    //    LTEUniFreq.Clear();
            //    //    LTEUniFreq = new ObservableCollection<decimal>(hs2);
            //    //    #endregion

            //    //    uint freqs = (uint)LTEUniFreq.Count(); //UMTSUniFreq
            //    //    ChannelSettings.pTableOfFrequencySetting = new RohdeSchwarz.ViCom.Net.LTE.SFrequencySetting[freqs];
            //    //    ChannelSettings.dwCount = (uint)ChannelSettings.pTableOfFrequencySetting.Length;
            //    //    int iNarrowbandRefSignalMeasMode = (RohdeSchwarz.ViCom.Net.LTE.SFrequencySetting.wNARROWBAND_RSRP_RSRQ |
            //    //                                         RohdeSchwarz.ViCom.Net.LTE.SFrequencySetting.wCENTER_RSCINR_1x1080KHZ |
            //    //                                         RohdeSchwarz.ViCom.Net.LTE.SFrequencySetting.w1MHZ_FILTER_NOISE_FOR_15RB);

            //    //    for (int i = 0; i < freqs; i++)
            //    //    {
            //    //        #region freq set
            //    //        ChannelSettings.pTableOfFrequencySetting[i] = new RohdeSchwarz.ViCom.Net.LTE.SFrequencySetting();
            //    //        ChannelSettings.pTableOfFrequencySetting[i].dwFrontEndSelectionMask = GetDeviceRFInput(true, 1, "LTE");
            //    //        ChannelSettings.pTableOfFrequencySetting[i].dCenterFrequencyInHz = (double)LTEUniFreq[i];
            //    //        ChannelSettings.pTableOfFrequencySetting[i].dwSymbolsPerSlotMask = RohdeSchwarz.ViCom.Net.LTE.SFrequencySetting.dwDefaultSymbolsPerSlot;
            //    //        ChannelSettings.pTableOfFrequencySetting[i].enFrameStructureType = FrameStructureType.Type.FDD;
            //    //        ChannelSettings.pTableOfFrequencySetting[i].bUpDownLinkMask = RohdeSchwarz.ViCom.Net.LTE.SFrequencySetting.bAllUpDownLinkConfigurations;
            //    //        ChannelSettings.pTableOfFrequencySetting[i].wSpecialSubframe1ConfigurationMask = RohdeSchwarz.ViCom.Net.LTE.SFrequencySetting.wAllSpecialSubFrameConfigurations;
            //    //        ChannelSettings.pTableOfFrequencySetting[i].wSpecialSubframe6ConfigurationMask = RohdeSchwarz.ViCom.Net.LTE.SFrequencySetting.wAllSpecialSubFrameConfigurations;
            //    //        ChannelSettings.pTableOfFrequencySetting[i].dwAvgBlockCountPer1000Sec = 2000;// 300;//((RohdeSchwarz.ViCom.Net.SRange<uint>)lteInterface.GetMeasRateLimits()).minimum;////////////////////////;
            //    //        ChannelSettings.pTableOfFrequencySetting[i].enSSyncToPSyncRatioType = RohdeSchwarz.ViCom.Net.LTE.SFrequencySetting.SSyncToPSyncRatioType.Type.RatioRange;
            //    //        ChannelSettings.pTableOfFrequencySetting[i].SSyncToPSyncRatio = new RohdeSchwarz.ViCom.Net.LTE.SFrequencySetting.SSyncToPSyncRatioSettings();
            //    //        ChannelSettings.pTableOfFrequencySetting[i].SSyncToPSyncRatio.RatioRange = new RohdeSchwarz.ViCom.Net.LTE.SFrequencySetting.SSyncToPSyncRatioSettings._StructRatioRange();
            //    //        ChannelSettings.pTableOfFrequencySetting[i].SSyncToPSyncRatio.RatioRange.fLowerRatioInDB = -5;
            //    //        ChannelSettings.pTableOfFrequencySetting[i].SSyncToPSyncRatio.RatioRange.fUpperRatioInDB = 0;
            //    //        ChannelSettings.pTableOfFrequencySetting[i].wNarrowbandRefSignalMeasMode = (ushort)iNarrowbandRefSignalMeasMode;
            //    //        ChannelSettings.pTableOfFrequencySetting[i].enBandwidthCtrlMode = RohdeSchwarz.ViCom.Net.LTE.SFrequencySetting.BandwidthCtrlMode.Type.BW_FROM_MIB_ONCE_EACH_CELL;
            //    //        //ChannelSettings.pTableOfFrequencySetting[i].wNumberOfResourceBlocks = 50;
            //    //        ChannelSettings.pTableOfFrequencySetting[i].WidebandRsCinrSettings = new RohdeSchwarz.ViCom.Net.LTE.SFrequencySetting.SWidebandRsCinrSettings();
            //    //        ChannelSettings.pTableOfFrequencySetting[i].WidebandRsCinrSettings.wWidebandRsCinrMeasMode = RohdeSchwarz.ViCom.Net.LTE.SFrequencySetting.SWidebandRsCinrSettings.wWIDEBAND_RS_CINR;
            //    //        ChannelSettings.pTableOfFrequencySetting[i].WidebandRsCinrSettings.dwAvgBlockCountPer1000Sec = 1000;// 300;//((RohdeSchwarz.ViCom.Net.SRange<uint>)lteInterface.GetWbMeasRateLimits()).minimum;////////////////////////;
            //    //        ChannelSettings.pTableOfFrequencySetting[i].WidebandRsCinrSettings.wNumberOfRBsInSubband = RohdeSchwarz.ViCom.Net.LTE.SFrequencySetting.SWidebandRsCinrSettings.wMinRBsInSubband;
            //    //        ChannelSettings.pTableOfFrequencySetting[i].WidebandRsCinrSettings.dwFrontEndSelectionMask = GetDeviceRFInput(true, 1, "LTE");
            //    //        ChannelSettings.pTableOfFrequencySetting[i].WidebandRsCinrSettings.bForceNoGap = true;
            //    //        ChannelSettings.pTableOfFrequencySetting[i].WidebandRsCinrSettings.bMaxCountOfeNodeBs = 6;
            //    //        ChannelSettings.pTableOfFrequencySetting[i].WidebandRsCinrSettings.bTransmitAntennaSelectionMask = 15;
            //    //        ChannelSettings.pTableOfFrequencySetting[i].WidebandRsCinrSettings.sMinCenterRsrpInDBm100 = -13000;
            //    //        ChannelSettings.pTableOfFrequencySetting[i].WidebandRsCinrSettings.wMaxRsrpDiffToBestCellInDB100 = 500;
            //    //        ChannelSettings.pTableOfFrequencySetting[i].enMbmsConfigCtrlMode = RohdeSchwarz.ViCom.Net.LTE.SFrequencySetting.MbmsConfigCtrlMode.Type.MBMS_NOT_PRESENT;
            //    //        ChannelSettings.pTableOfFrequencySetting[i].MimoSettings = new RohdeSchwarz.ViCom.Net.LTE.SFrequencySetting.SMimoSettings();
            //    //        ChannelSettings.pTableOfFrequencySetting[i].MimoSettings.wMimoMeasMode = 0;
            //    //        ChannelSettings.pTableOfFrequencySetting[i].MimoSettings.dwMimoResultMaskFor2x2 = 7;
            //    //        ChannelSettings.pTableOfFrequencySetting[i].MimoSettings.dwMimoResultMaskFor2x4 = 0;
            //    //        ChannelSettings.pTableOfFrequencySetting[i].MimoSettings.bMaxCountOfeNodeBs = 5;
            //    //        ChannelSettings.pTableOfFrequencySetting[i].MimoSettings.sCinrThresholdForRankInDB100 = 0;
            //    //        ChannelSettings.pTableOfFrequencySetting[i].MimoSettings.sMinCenterRsrpInDBm100 = -13000;
            //    //        ChannelSettings.pTableOfFrequencySetting[i].MimoSettings.sMinRsCinrInDB100 = -1000;
            //    //        ChannelSettings.pTableOfFrequencySetting[i].MimoSettings.wMaxRsrpDiffToBestCellInDB100 = 500;
            //    //        ChannelSettings.pTableOfFrequencySetting[i].MimoSettings.wTimeResolutionInMs = 10;
            //    //        // Setup Throughput estimation           
            //    //        ChannelSettings.pTableOfFrequencySetting[i].MimoSettings.bEnableThroughputEstimation = false;
            //    //        ChannelSettings.pTableOfFrequencySetting[i].RssiSettings = new RohdeSchwarz.ViCom.Net.LTE.SFrequencySetting.SRssiSettings();
            //    //        ChannelSettings.pTableOfFrequencySetting[i].RssiSettings.dwFrontEndSelectionMask = GetDeviceRFInput(true, 1, "LTE");
            //    //        ChannelSettings.pTableOfFrequencySetting[i].RssiSettings.wRssiMeasMode = 0;
            //    //        ChannelSettings.pTableOfFrequencySetting[i].MbmsSettings.wMbmsMeasMode = 0;
            //    //        #endregion
            //    //    }
            //    //    ChannelSettings.dwCount = (uint)ChannelSettings.pTableOfFrequencySetting.Length;
            //    //    ChannelSettings.RsCinrChannelModel = new RohdeSchwarz.ViCom.Net.LTE.SChannelSettings.SRsCinrChannelModel();
            //    //    ChannelSettings.RsCinrChannelModel.dwDelaySpreadInNs = RohdeSchwarz.ViCom.Net.LTE.SChannelSettings.SRsCinrChannelModel.dwMinDelaySpreadInNs;
            //    //    ChannelSettings.RsCinrChannelModel.dwSpeedInKmPerHour = RohdeSchwarz.ViCom.Net.LTE.SChannelSettings.SRsCinrChannelModel.dwMinSpeedInKmh;

            //    //    ChannelSettings.MbmsRsCinrChannelModel.dwDelaySpreadInNs = RohdeSchwarz.ViCom.Net.LTE.SChannelSettings.SMbmsRsCinrChannelModel.dwMinDelaySpreadInNs;
            //    //    ChannelSettings.MbmsRsCinrChannelModel.dwSpeedInKmPerHour = RohdeSchwarz.ViCom.Net.LTE.SChannelSettings.SMbmsRsCinrChannelModel.dwMinSpeedInKmh;

            //    //    List<RohdeSchwarz.ViCom.Net.LTE.Pdu.Type> siblist = new List<RohdeSchwarz.ViCom.Net.LTE.Pdu.Type>() { };
            //    //    for (int i = 0; i < App.Sett.TSMxReceiver_Settings.LTE.SIBTypes.Count; i++)
            //    //    { if (App.Sett.TSMxReceiver_Settings.LTE.SIBTypes[i].Use) { siblist.Add((RohdeSchwarz.ViCom.Net.LTE.Pdu.Type)Enum.Parse(typeof(RohdeSchwarz.ViCom.Net.LTE.Pdu.Type), App.Sett.TSMxReceiver_Settings.LTE.SIBTypes[i].SibType)); } }

            //    //    // Activate BCH demodulation
            //    //    uint dwRequests = (uint)siblist.Count * ChannelSettings.dwCount;
            //    //    RohdeSchwarz.ViCom.Net.LTE.SDemodulationSettings bchSettings = new RohdeSchwarz.ViCom.Net.LTE.SDemodulationSettings();
            //    //    bchSettings.sSINRThresholdDB100 = 100;// ((RohdeSchwarz.ViCom.Net.SRange<short>)lteInterface.GetDemodThresholdLimits()).minimum;
            //    //    bchSettings.dwFrontEndSelectionMask = GetDeviceRFInput(true, 1, "LTE"); //dwFE_Mask;

            //    //    bchSettings.sStartMeasurementRequests.dwCountOfRequests = dwRequests;
            //    //    bchSettings.sStartMeasurementRequests.pDemodRequests = new RohdeSchwarz.ViCom.Net.LTE.SDemodRequests.SDemodRequest[dwRequests];
            //    //    for (int i = 0; i < dwRequests; i++)
            //    //    {
            //    //        bchSettings.sStartMeasurementRequests.pDemodRequests[i] = new RohdeSchwarz.ViCom.Net.LTE.SDemodRequests.SDemodRequest();
            //    //    }
            //    //    for (int i = 0; i < freqs; i++)
            //    //    {
            //    //        int dwRequestStartIndex = i * siblist.Count;
            //    //        for (int idx = 0; idx < siblist.Count; ++idx)
            //    //        {
            //    //            int iR = dwRequestStartIndex + idx;
            //    //            bchSettings.sStartMeasurementRequests.pDemodRequests[iR].dwChannelIndex = (uint)i;
            //    //            bchSettings.sStartMeasurementRequests.pDemodRequests[iR].ePDU = siblist[idx];
            //    //            bchSettings.sStartMeasurementRequests.pDemodRequests[iR].eDemodulationMode = RohdeSchwarz.ViCom.Net.LTE.DemodMode.Type.ONCE;
            //    //            bchSettings.sStartMeasurementRequests.pDemodRequests[iR].wRepetitionTimeOutInMs = 0;// 100;
            //    //                                                                                                //bchSettings.sStartMeasurementRequests.pDemodRequests[iR].dwBtsId = 0;
            //    //        }
            //    //    }

            //    //    lteInterface.SetFrequencyTable(ChannelSettings);
            //    //    lteInterface.SetDemodulationSettings(bchSettings);

            //    //    lteBasicInterface.StartMeasurement();
            //    //    DM -= LTESetFreqFromACD;
            //    //}





            //}
            //#region Exception
            //catch (RohdeSchwarz.ViCom.Net.CViComError error)
            //{
            //    _logger.Exception(Contexts.ThisComponent, new Exception("ErrorCode:" + error.ErrorCode + " ErrorString:" + error.ErrorString));
            //}
            //catch (Exception exp)
            //{
            //    _logger.Exception(Contexts.ThisComponent, exp);
            //}
            //#endregion

        }
        private void GetUnifreqsLTE(List<decimal> freqs)
        {
            #region LTE
            System.Collections.Generic.HashSet<decimal> hs3 = new System.Collections.Generic.HashSet<decimal>();
            foreach (decimal al in freqs)
            {
                hs3.Add(al);
            }
            LTEUniFreq.Clear();
            LTEUniFreq = new List<decimal>(hs3.OrderBy(i => i));

            LTESITypes = new List<SIType>()
            {
                new SIType()
                {
                    SiType = RohdeSchwarz.ViCom.Net.LTE.Pdu.Type.MIB.ToString(),
                    Use = true
                },
                new SIType()
                {
                    SiType = RohdeSchwarz.ViCom.Net.LTE.Pdu.Type.SIB1.ToString(),
                    Use = true
                }
            };
            #endregion
        }
        #endregion LTE

        #region CDMA        
        private static SEvdoControlSettings GetEvdoSettings()
        {
            var evdoSetting = new SEvdoControlSettings();
            evdoSetting.bStopCdma2000AfterSync = false;
            evdoSetting.dwFullSyncRatePer1000Sec = 10000;
            evdoSetting.dwShortSyncRatePer1000Sec = 10000;
            evdoSetting.dwMeasRatePer1000Sec = 10000;
            evdoSetting.dwShortSyncRangeInChips = 160;


            return evdoSetting;
        }
        private void CDMAConnect()
        {
            try
            {
                cdmaLoader = new CViComLoader<CViComCdmaInterface>(DeviceType);
                bool Runing = cdmaLoader.Connect(IPAddress, out error, receiverListener);

                cdmaInterface = cdmaLoader.GetInterface();
                cdmaBasicInterface = cdmaLoader.GetBasicInterface();

                var resbuf = new SResultBufferDepth();
                resbuf.dwValue = 1024;
                cdmaBasicInterface.SetResultBufferDepth(resbuf);

                //RohdeSchwarz.ViCom.Net.SRange<uint> rateLimit = (RohdeSchwarz.ViCom.Net.SRange<uint>)cdmaInterface.GetMeasRateLimits();

                //CDMAUniFreq
                uint freqs = (uint)CDMAUniFreq.Count();
                var settings = new RohdeSchwarz.ViCom.Net.CDMA.SChannelSettings();
                settings.dwFrontEndSelectionMask = RFInputCDMA;
                settings.dwCount = freqs;
                settings.dwMeasRatePer1000Sec = 1000;//1000;//((RohdeSchwarz.ViCom.Net.SRange<uint>)cdmaInterface.GetMeasRateLimits()).minimum; ;// rateLimit.defaultValue;

                settings.pTableOfFrequencySetting = new RohdeSchwarz.ViCom.Net.CDMA.SFrequencySetting[freqs];
                int cdmaFreqsCount = 0;
                int evdoFreqsCount = 0;
                for (int i = 0; i < freqs; i++)
                {
                    settings.pTableOfFrequencySetting[i] = new RohdeSchwarz.ViCom.Net.CDMA.SFrequencySetting();
                    settings.pTableOfFrequencySetting[i].bIsEvdoFrequency = CDMAUniFreq[i].EVDOvsCDMA;
                    settings.pTableOfFrequencySetting[i].dCenterFrequencyInHz = (double)(CDMAUniFreq[i].FreqDn);
                    settings.pTableOfFrequencySetting[i].bTableOfPNOffsetArbitraryLimitation = new bool[512];
                    if (CDMAUniFreq[i].EVDOvsCDMA == false)
                    {
                        for (int ii = 0; ii < settings.pTableOfFrequencySetting[i].bTableOfPNOffsetArbitraryLimitation.Length; ii++)
                        {
                            settings.pTableOfFrequencySetting[i].bTableOfPNOffsetArbitraryLimitation[ii] = true;
                        }
                        cdmaFreqsCount++;
                    }
                    else { evdoFreqsCount++; }
                }

                cdmaInterface.SetFrequencyTable(settings);

                cdmaInterface.SetEvdoSettings(GetEvdoSettings());

                CDMAListener = new MyCdmaDataProcessor(_logger, _timeService);

                cdmaInterface.RegisterResultDataListener(CDMAListener);
                #region
                //===============

                List<RohdeSchwarz.ViCom.Net.CDMA.Pdu.Type> CDMAsiblist = new List<RohdeSchwarz.ViCom.Net.CDMA.Pdu.Type>() { };
                List<RohdeSchwarz.ViCom.Net.CDMA.Pdu.Type> EVDOsiblist = new List<RohdeSchwarz.ViCom.Net.CDMA.Pdu.Type>() { };
                for (int i = 0; i < CDMASITypes.Count; i++)//foreach (RohdeSchwarz.ViCom.Net.CDMA.Pdu.Type pdu in Enum.GetValues(typeof(RohdeSchwarz.ViCom.Net.CDMA.Pdu.Type)))
                {
                    if (!CDMASITypes[i].ToString().Contains("UNKNOWN") && !CDMASITypes[i].ToString().Contains("NONE"))
                    {
                        if (CDMASITypes[i].Use && CDMASITypes[i].SiType.StartsWith("EVDO"))
                        { EVDOsiblist.Add((RohdeSchwarz.ViCom.Net.CDMA.Pdu.Type)Enum.Parse(typeof(RohdeSchwarz.ViCom.Net.CDMA.Pdu.Type), CDMASITypes[i].SiType)); }
                        else if (CDMASITypes[i].Use && !CDMASITypes[i].SiType.StartsWith("EVDO"))
                        { CDMAsiblist.Add((RohdeSchwarz.ViCom.Net.CDMA.Pdu.Type)Enum.Parse(typeof(RohdeSchwarz.ViCom.Net.CDMA.Pdu.Type), CDMASITypes[i].SiType)); }
                    }
                }
                RohdeSchwarz.ViCom.Net.CDMA.SDemodulationSettings demod = new RohdeSchwarz.ViCom.Net.CDMA.SDemodulationSettings();

                demod.dwFrontEndSelectionMask = RFInputCDMA;

                uint dwRequests = (uint)(CDMAsiblist.Count * cdmaFreqsCount + EVDOsiblist.Count * evdoFreqsCount);

                RohdeSchwarz.ViCom.Net.CDMA.SDemodRequests MeasurementRequests = new RohdeSchwarz.ViCom.Net.CDMA.SDemodRequests();

                //MeasurementRequests.dwCountOfRequests = dwRequests;
                MeasurementRequests.pDemodRequests = new RohdeSchwarz.ViCom.Net.CDMA.SDemodRequests.SDemodRequest[dwRequests];

                int idwRequestIndex = 0;
                for (int i = 0; i < settings.pTableOfFrequencySetting.Count(); i++)
                {
                    if (settings.pTableOfFrequencySetting[i].bIsEvdoFrequency == false)//cdma
                    {
                        for (int idx = 0; idx < CDMAsiblist.Count; idx++)
                        {
                            MeasurementRequests.pDemodRequests[idwRequestIndex] = new RohdeSchwarz.ViCom.Net.CDMA.SDemodRequests.SDemodRequest();
                            MeasurementRequests.pDemodRequests[idwRequestIndex].dwChannelIndex = (uint)i;
                            MeasurementRequests.pDemodRequests[idwRequestIndex].eDemodulationMode = RohdeSchwarz.ViCom.Net.CDMA.DemodMode.Type.ONCE;
                            MeasurementRequests.pDemodRequests[idwRequestIndex].PduSpec.ePDU = CDMAsiblist[idx];
                            MeasurementRequests.pDemodRequests[idwRequestIndex].PduSpec.eChannelType = ChannelType.Type.UNKNOWN;
                            MeasurementRequests.pDemodRequests[idwRequestIndex].PduSpec.eEvdoProtocol = EvdoProtocol.Type.UNKNOWN;
                            MeasurementRequests.pDemodRequests[idwRequestIndex].wRepetitionDelayIn100ms = 0;
                            idwRequestIndex++;
                        }
                    }
                    else if (settings.pTableOfFrequencySetting[i].bIsEvdoFrequency == true)//evdo
                    {
                        for (int idx = 0; idx < EVDOsiblist.Count; idx++)
                        {
                            MeasurementRequests.pDemodRequests[idwRequestIndex] = new RohdeSchwarz.ViCom.Net.CDMA.SDemodRequests.SDemodRequest();
                            MeasurementRequests.pDemodRequests[idwRequestIndex].dwChannelIndex = (uint)i;
                            MeasurementRequests.pDemodRequests[idwRequestIndex].eDemodulationMode = RohdeSchwarz.ViCom.Net.CDMA.DemodMode.Type.ONCE;
                            MeasurementRequests.pDemodRequests[idwRequestIndex].PduSpec.ePDU = EVDOsiblist[idx];
                            MeasurementRequests.pDemodRequests[idwRequestIndex].PduSpec.eEvdoProtocol = EvdoProtocol.Type.UNKNOWN;
                            MeasurementRequests.pDemodRequests[idwRequestIndex].PduSpec.eChannelType = ChannelType.Type.UNKNOWN;
                            MeasurementRequests.pDemodRequests[idwRequestIndex].wRepetitionDelayIn100ms = 0;
                            idwRequestIndex++;
                        }
                    }
                }
                MeasurementRequests.dwCountOfRequests = (uint)idwRequestIndex;
                demod.sStartMeasurementRequests = MeasurementRequests;
                demod.lEcToIoThresholdInDB100_for_EVDO = -1000;
                demod.lEcToIoThresholdInDB100_for_CDMA = -1000;

                SPPSSettings pps = new SPPSSettings() { };
                pps.iDelayOfPPSFallingEdgeIn100ns = SPPSSettings.iInvalidPPSDelayIn100ns;//10000000;

                cdmaInterface.SetPPSSettings(pps);
                cdmaInterface.SetSyncChannelDemodulationMode(SyncChannelDemodulationMode.Type.ALL);


                cdmaInterface.SetDemodulationSettings(demod);
                SMaxVelocity mv = new SMaxVelocity();
                mv.dMaxVelocityInKmPerHour = 120;
                cdmaInterface.SetMaxVelocity(mv);
                //=======
                #endregion
                cdmaBasicInterface.StartMeasurement();
                CDMAIsRuning = Runing;
                CDMAUpdateData = CDMAIsRuning;
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
        private void CDMADisconnect()
        {
            try
            {
                if (cdmaLoader != null)
                {
                    if (cdmaLoader.GetBasicInterface().IsMeasurementStarted())
                    {
                        cdmaLoader.GetBasicInterface().StopMeasurement();
                        cdmaLoader.GetBasicInterface().HasMeasurementStopped(SDefs.dwDefaultTimeOutInMs);
                        cdmaInterface.UnregisterResultDataListener(CDMAListener);
                        CDMAIsRuning = !cdmaLoader.Disconnect();
                        CDMAUpdateData = CDMAIsRuning;
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
        private static void CDMASetFreqFromACD()
        {
            //try
            //{
            //    //IdentificationData.ACD.ACDData
            //    if (cdmaLoader != null && cdmaLoader.GetBasicInterface().IsMeasurementStarted())
            //    {
            //        cdmaLoader.GetBasicInterface().StopMeasurement();
            //    }
            //    if (cdmaLoader.GetBasicInterface().HasMeasurementStopped(SDefs.dwDefaultTimeOutInMs))
            //    {
            //        var ChannelSettings = cdmaInterface.GetSettings().ChannelSettings;

            //        #region Freqs 
            //        List<Settings.CDMAFreqs_Set> freq = new List<Settings.CDMAFreqs_Set>() { };
            //        foreach (Settings.CDMAFreqs_Set t in CDMAUniFreq)
            //        {
            //            freq.Add(t);
            //        }
            //        for (int i = 0; i < IdentificationData.ACD.ACDData.Count(); i++)
            //        {
            //            if (IdentificationData.ACD.ACDData[i].Tech == 3 && IdentificationData.ACD.ACDData[i].Established == false)
            //            {
            //                bool find = false;
            //                foreach (Settings.CDMAFreqs_Set t in CDMAUniFreq)
            //                {
            //                    if (t.FreqDn == IdentificationData.ACD.ACDData[i].Freq && t.EVDOvsCDMA == false) find = true;
            //                }
            //                if (find == false)
            //                {
            //                    Settings.CDMAFreqs_Set fs = new Settings.CDMAFreqs_Set()
            //                    {
            //                        EVDOvsCDMA = false,
            //                        FreqDn = IdentificationData.ACD.ACDData[i].Freq,
            //                        Use = true,
            //                    };
            //                    freq.Add(fs);
            //                }
            //                IdentificationData.ACD.ACDData[i].Established = true;
            //            }
            //            if (IdentificationData.ACD.ACDData[i].Tech == 4 && IdentificationData.ACD.ACDData[i].Established == false)
            //            {
            //                bool find = false;
            //                foreach (Settings.CDMAFreqs_Set t in CDMAUniFreq)
            //                {
            //                    if (t.FreqDn == IdentificationData.ACD.ACDData[i].Freq && t.EVDOvsCDMA == true) find = true;
            //                }
            //                if (find == false)
            //                {
            //                    Settings.CDMAFreqs_Set fs = new Settings.CDMAFreqs_Set()
            //                    {
            //                        EVDOvsCDMA = true,
            //                        FreqDn = IdentificationData.ACD.ACDData[i].Freq,
            //                        Use = true,
            //                    };
            //                    freq.Add(fs);
            //                }
            //                IdentificationData.ACD.ACDData[i].Established = true;
            //            }
            //        }
            //        System.Collections.Generic.HashSet<Settings.CDMAFreqs_Set> hs = new System.Collections.Generic.HashSet<Settings.CDMAFreqs_Set>();
            //        foreach (Settings.CDMAFreqs_Set al in freq)
            //        {
            //            hs.Add(al);
            //        }
            //        CDMAUniFreq.Clear();
            //        CDMAUniFreq = new ObservableCollection<Settings.CDMAFreqs_Set>(hs);
            //        #endregion

            //        uint freqs = (uint)CDMAUniFreq.Count(); //UMTSUniFreq

            //        ChannelSettings.dwFrontEndSelectionMask = GetDeviceRFInput(true, 1, "CDMA");
            //        ChannelSettings.dwCount = freqs;
            //        ChannelSettings.dwMeasRatePer1000Sec = 1000;

            //        ChannelSettings.pTableOfFrequencySetting = new RohdeSchwarz.ViCom.Net.CDMA.SFrequencySetting[freqs];
            //        int cdmaFreqsCount = 0;
            //        int evdoFreqsCount = 0;
            //        for (int i = 0; i < freqs; i++)
            //        {
            //            ChannelSettings.pTableOfFrequencySetting[i] = new RohdeSchwarz.ViCom.Net.CDMA.SFrequencySetting();
            //            ChannelSettings.pTableOfFrequencySetting[i].bIsEvdoFrequency = CDMAUniFreq[i].EVDOvsCDMA;
            //            ChannelSettings.pTableOfFrequencySetting[i].dCenterFrequencyInHz = (double)(CDMAUniFreq[i].FreqDn);
            //            ChannelSettings.pTableOfFrequencySetting[i].bTableOfPNOffsetArbitraryLimitation = new bool[512];
            //            if (CDMAUniFreq[i].EVDOvsCDMA == false)
            //            {
            //                for (int ii = 0; ii < ChannelSettings.pTableOfFrequencySetting[i].bTableOfPNOffsetArbitraryLimitation.Length; ii++)
            //                {
            //                    ChannelSettings.pTableOfFrequencySetting[i].bTableOfPNOffsetArbitraryLimitation[ii] = true;
            //                }
            //                cdmaFreqsCount++;
            //            }
            //            else { evdoFreqsCount++; }
            //        }
            //        cdmaInterface.SetFrequencyTable(ChannelSettings);
            //        cdmaInterface.SetEvdoSettings(GetEvdoSettings());
            //        #region
            //        List<RohdeSchwarz.ViCom.Net.CDMA.Pdu.Type> CDMAsiblist = new List<RohdeSchwarz.ViCom.Net.CDMA.Pdu.Type>() { };
            //        List<RohdeSchwarz.ViCom.Net.CDMA.Pdu.Type> EVDOsiblist = new List<RohdeSchwarz.ViCom.Net.CDMA.Pdu.Type>() { };
            //        for (int i = 0; i < App.Sett.TSMxReceiver_Settings.CDMA.SITypes.Count; i++)//foreach (RohdeSchwarz.ViCom.Net.CDMA.Pdu.Type pdu in Enum.GetValues(typeof(RohdeSchwarz.ViCom.Net.CDMA.Pdu.Type)))
            //        {
            //            if (!App.Sett.TSMxReceiver_Settings.CDMA.SITypes[i].ToString().Contains("UNKNOWN") && !App.Sett.TSMxReceiver_Settings.CDMA.SITypes[i].ToString().Contains("NONE"))
            //            {
            //                if (App.Sett.TSMxReceiver_Settings.CDMA.SITypes[i].Use && App.Sett.TSMxReceiver_Settings.CDMA.SITypes[i].SiType.StartsWith("EVDO")) { EVDOsiblist.Add((RohdeSchwarz.ViCom.Net.CDMA.Pdu.Type)Enum.Parse(typeof(RohdeSchwarz.ViCom.Net.CDMA.Pdu.Type), App.Sett.TSMxReceiver_Settings.CDMA.SITypes[i].SiType)); }
            //                else if (App.Sett.TSMxReceiver_Settings.CDMA.SITypes[i].Use && !App.Sett.TSMxReceiver_Settings.CDMA.SITypes[i].SiType.StartsWith("EVDO")) { CDMAsiblist.Add((RohdeSchwarz.ViCom.Net.CDMA.Pdu.Type)Enum.Parse(typeof(RohdeSchwarz.ViCom.Net.CDMA.Pdu.Type), App.Sett.TSMxReceiver_Settings.CDMA.SITypes[i].SiType)); }
            //            }
            //        }
            //        RohdeSchwarz.ViCom.Net.CDMA.SDemodulationSettings demod = new RohdeSchwarz.ViCom.Net.CDMA.SDemodulationSettings();

            //        demod.dwFrontEndSelectionMask = GetDeviceRFInput(true, 1, "CDMA");

            //        uint dwRequests = (uint)(CDMAsiblist.Count * cdmaFreqsCount + EVDOsiblist.Count * evdoFreqsCount);

            //        RohdeSchwarz.ViCom.Net.CDMA.SDemodRequests MeasurementRequests = new RohdeSchwarz.ViCom.Net.CDMA.SDemodRequests();

            //        //MeasurementRequests.dwCountOfRequests = dwRequests;
            //        MeasurementRequests.pDemodRequests = new RohdeSchwarz.ViCom.Net.CDMA.SDemodRequests.SDemodRequest[dwRequests];

            //        int idwRequestIndex = 0;
            //        for (int i = 0; i < ChannelSettings.pTableOfFrequencySetting.Count(); i++)
            //        {
            //            if (ChannelSettings.pTableOfFrequencySetting[i].bIsEvdoFrequency == false)//cdma
            //            {
            //                for (int idx = 0; idx < CDMAsiblist.Count; idx++)
            //                {
            //                    MeasurementRequests.pDemodRequests[idwRequestIndex] = new RohdeSchwarz.ViCom.Net.CDMA.SDemodRequests.SDemodRequest();
            //                    MeasurementRequests.pDemodRequests[idwRequestIndex].dwChannelIndex = (uint)i;
            //                    MeasurementRequests.pDemodRequests[idwRequestIndex].eDemodulationMode = RohdeSchwarz.ViCom.Net.CDMA.DemodMode.Type.ONCE;
            //                    MeasurementRequests.pDemodRequests[idwRequestIndex].PduSpec.ePDU = CDMAsiblist[idx];
            //                    MeasurementRequests.pDemodRequests[idwRequestIndex].PduSpec.eChannelType = ChannelType.Type.UNKNOWN;
            //                    MeasurementRequests.pDemodRequests[idwRequestIndex].PduSpec.eEvdoProtocol = EvdoProtocol.Type.UNKNOWN;
            //                    MeasurementRequests.pDemodRequests[idwRequestIndex].wRepetitionDelayIn100ms = 0;
            //                    idwRequestIndex++;
            //                }
            //            }
            //            else if (ChannelSettings.pTableOfFrequencySetting[i].bIsEvdoFrequency == true)//evdo
            //            {
            //                for (int idx = 0; idx < EVDOsiblist.Count; idx++)
            //                {
            //                    MeasurementRequests.pDemodRequests[idwRequestIndex] = new RohdeSchwarz.ViCom.Net.CDMA.SDemodRequests.SDemodRequest();
            //                    MeasurementRequests.pDemodRequests[idwRequestIndex].dwChannelIndex = (uint)i;
            //                    MeasurementRequests.pDemodRequests[idwRequestIndex].eDemodulationMode = RohdeSchwarz.ViCom.Net.CDMA.DemodMode.Type.ONCE;
            //                    MeasurementRequests.pDemodRequests[idwRequestIndex].PduSpec.ePDU = EVDOsiblist[idx];
            //                    MeasurementRequests.pDemodRequests[idwRequestIndex].PduSpec.eEvdoProtocol = EvdoProtocol.Type.UNKNOWN;
            //                    MeasurementRequests.pDemodRequests[idwRequestIndex].PduSpec.eChannelType = ChannelType.Type.UNKNOWN;
            //                    MeasurementRequests.pDemodRequests[idwRequestIndex].wRepetitionDelayIn100ms = 0;
            //                    idwRequestIndex++;
            //                }
            //            }
            //        }
            //        MeasurementRequests.dwCountOfRequests = (uint)idwRequestIndex;
            //        demod.sStartMeasurementRequests = MeasurementRequests;
            //        demod.lEcToIoThresholdInDB100_for_EVDO = -1000;
            //        demod.lEcToIoThresholdInDB100_for_CDMA = -1000;

            //        SPPSSettings pps = new SPPSSettings() { };
            //        pps.iDelayOfPPSFallingEdgeIn100ns = SPPSSettings.iInvalidPPSDelayIn100ns;//10000000;

            //        cdmaInterface.SetPPSSettings(pps);
            //        cdmaInterface.SetSyncChannelDemodulationMode(SyncChannelDemodulationMode.Type.ALL);


            //        cdmaInterface.SetDemodulationSettings(demod);
            //        SMaxVelocity mv = new SMaxVelocity();
            //        mv.dMaxVelocityInKmPerHour = 120;
            //        cdmaInterface.SetMaxVelocity(mv);
            //        //=======
            //        #endregion
            //        cdmaBasicInterface.StartMeasurement();






            //        DM -= CDMASetFreqFromACD;
            //    }





            //}
            //#region Exception
            //catch (RohdeSchwarz.ViCom.Net.CViComError error)
            //{
            //    _logger.Exception(Contexts.ThisComponent, new Exception("ErrorCode:" + error.ErrorCode + " ErrorString:" + error.ErrorString));
            //}
            //catch (Exception exp)
            //{
            //    _logger.Exception(Contexts.ThisComponent, exp);
            //}
            //#endregion

        }
        private void GetUnifreqsCDMA(List<LCDMA.Channel> freqs)
        {
            #region CDMA 
            System.Collections.Generic.HashSet<LCDMA.Channel> hs = new System.Collections.Generic.HashSet<LCDMA.Channel>();
            foreach (LCDMA.Channel t in freqs)
            {
                hs.Add(t);
            }
            CDMAUniFreq.Clear();
            CDMAUniFreq = new List<LCDMA.Channel>(hs.OrderBy(i => i.FreqDn));

            CDMASITypes = new List<SIType>()
            {
                new SIType()
                {
                    SiType = RohdeSchwarz.ViCom.Net.CDMA.Pdu.Type.SYS_PARAMS.ToString(),
                    Use = true
                },
                new SIType()
                {
                    SiType = RohdeSchwarz.ViCom.Net.CDMA.Pdu.Type.EXT_SYS_PARAMS.ToString(),
                    Use = true
                },
                new SIType()
                {
                    SiType = RohdeSchwarz.ViCom.Net.CDMA.Pdu.Type.ACCESS_PARAMETERS.ToString(),
                    Use = true
                },
                new SIType()
                {
                    SiType = RohdeSchwarz.ViCom.Net.CDMA.Pdu.Type.SYNC_MESSAGE.ToString(),
                    Use = true
                },
                new SIType()
                {
                    SiType = RohdeSchwarz.ViCom.Net.CDMA.Pdu.Type.EVDO_SECTOR_PARAMETERS.ToString(),
                    Use = true
                }
            };
            #endregion
        }
        #endregion CDMA

        #region DataProcessors
        #region SIB parsers
        public static decimal[] ParseMIBLTE(string mib)
        {
            decimal[] outarray = new decimal[1] { 0 };
            string BW = "";
            int bw = 0;
            string[] stringSeparators = new string[] { "\r\n" };
            string[] sa = mib.Split(stringSeparators, StringSplitOptions.None);

            int bwstart = 0, bwstop = 0;
            decimal[] bwa = new decimal[] { 1.4m, 5, 10, 15, 20 };
            for (int i = 0; i < sa.Length; i++)
            {
                if (sa[i].Contains("dl_Bandwidth")) { bwstart = sa[i].IndexOf("(") + 1; bwstop = sa[i].IndexOf(")"); bw = Convert.ToInt32(sa[i].Substring(bwstart, bwstop - bwstart), 16); BW = sa[i].Substring(bwstart, bwstop - bwstart); }
            }
            outarray[0] = bwa[bw - 1] * 1000000;
            return outarray;
        }
        public static int[] ParseSIB1LTE(string sib)
        {
            int[] outarray = new int[6] { 0, 0, 0, 0, 0, 0 };
            string MCC = "", MNC = "";
            int mcc = 0, mnc = 0, tac = 0, celid28 = 0, enodbid = 0, celid = 0;
            string[] stringSeparators = new string[] { "\r\n" };
            string[] sa = sib.Split(stringSeparators, StringSplitOptions.None);
            int mccpos = 0, mcclenght = 0;
            int mncpos = 0, mnclenght = 0;
            int tacstart = 0, tacstop = 0;
            int celidstart = 0, celidstop = 0;
            for (int i = 0; i < sa.Length; i++)
            {
                if (sa[i].Contains("mcc")) { mccpos = i; mcclenght = int.Parse(sa[i].Replace("mcc", "").Replace(" ", "").TrimEnd()); }
                if (i > mccpos && i <= mccpos + mcclenght/*sa[i].Contains("MCC_MNC_Digit")*/) { MCC += sa[i].Replace("MCC_MNC_Digit", "").Replace(" ", "").TrimEnd(); }
                if (sa[i].Contains("mnc")) { mncpos = i; mnclenght = int.Parse(sa[i].Replace("mnc", "").Replace(" ", "").TrimEnd()); }
                if (i > mncpos && i <= mncpos + mnclenght/*sa[i].Contains("MCC_MNC_Digit")*/) { MNC += sa[i].Replace("MCC_MNC_Digit", "").Replace(" ", "").TrimEnd(); }
                if (sa[i].Contains("trackingAreaCode")) { tacstart = sa[i].IndexOf("(") + 1; tacstop = sa[i].IndexOf(")"); tac = Convert.ToInt32(sa[i].Substring(tacstart, tacstop - tacstart), 16); }
                if (sa[i].Contains("cellIdentity"))
                {
                    celidstart = sa[i].IndexOf("(0x") + 3; celidstop = sa[i].IndexOf(")");
                    celid28 = Convert.ToInt32(sa[i].Substring(celidstart, celidstop - celidstart), 16);
                    enodbid = Convert.ToInt32(sa[i].Substring(celidstart, celidstop - celidstart - 2), 16);
                    celid = Convert.ToInt16(sa[i].Substring(celidstop - 2, 2), 16);
                }
            }
            int.TryParse(MCC, out mcc);
            int.TryParse(MNC, out mnc);
            outarray[0] = mcc;
            outarray[1] = mnc;
            outarray[2] = tac;
            outarray[3] = celid28;
            outarray[4] = enodbid;
            outarray[5] = celid;
            return outarray;
        }
        /// <summary>
        /// парсим системную инфу
        /// </summary>
        /// <param name="si"></param>
        /// <returns>[SID, BaseID,]</returns>
        public static int[] ParseEVDO_SECTOR_PARAMETERS(string si)
        {
            int[] outarray = new int[2] { 0, 0 };

            int BaseID = 0;
            int SID = 0;
            string[] stringSeparators = new string[] { "\r\n" };
            string[] sa = si.Split(stringSeparators, StringSplitOptions.None);
            for (int i = 0; i < sa.Length; i++)
            {
                if (sa[i].Contains("SectorID"))
                {
                    int baseidstart = sa[i].IndexOf("(") + 1;
                    int baseidstop = sa[i].IndexOf(")");
                    string baseid = sa[i].Substring(baseidstart, baseidstop - baseidstart);
                    SID = (Convert.ToInt32(baseid.Substring(5, 4), 16)) / 4;
                    BaseID = Convert.ToInt32(baseid.Substring(baseid.Length - 5, 5), 16);
                }
            }
            outarray[0] = SID;
            outarray[1] = BaseID;
            return outarray;
        }
        /// <summary>
        /// парсим системную инфу
        /// </summary>
        /// <param name="si"></param>
        /// <returns>[BaseID,]</returns>
        public static decimal[] ParseCDMA_SYS_PARAMS(string si)
        {
            decimal[] outarray = new decimal[1] { 0 };

            int BaseID = 0;
            string[] stringSeparators = new string[] { "\r\n" };
            string[] sa = si.Split(stringSeparators, StringSplitOptions.None);
            int baseidstart = 0;
            for (int i = 0; i < sa.Length; i++)
            {
                if (sa[i].Contains("BASE_ID"))
                {
                    baseidstart = sa[i].IndexOf(":") + 1;
                    BaseID = Convert.ToInt32(sa[i].Substring(baseidstart));
                }
            }
            outarray[0] = BaseID;
            return outarray;
        }/// <summary>
         /// парсим системную инфу
         /// </summary>
         /// <param name="si"></param>
         /// <returns>[MCC,MNC]</returns>
        public static decimal[] ParseCDMA_EXT_SYS_PARAMS(string si)
        {
            decimal[] outarray = new decimal[2] { 0, 0 };

            int MCC = 0, MNC = 0;
            string[] stringSeparators = new string[] { "\r\n" };
            string[] sa = si.Split(stringSeparators, StringSplitOptions.None);
            for (int i = 0; i < sa.Length; i++)
            {
                if (sa[i].Contains("MCC"))
                {
                    int MCCstart = sa[i].IndexOf(":") + 1;
                    MCC = Convert.ToInt32(sa[i].Substring(MCCstart));
                }
                if (sa[i].Contains("IMSI_11_12"))
                {
                    int MNCstart = sa[i].IndexOf(":") + 1;
                    int MNCstop = sa[i].IndexOf("(");
                    MNC = Convert.ToInt32(sa[i].Substring(MNCstart, MNCstop - MNCstart));
                }
            }
            outarray[0] = MCC;
            outarray[1] = MNC;
            return outarray;
        }
        #endregion
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
                            //Console.WriteLine(mes);
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
                if (GSMIsRuning && GSMUpdateData)
                {
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
                                    LGSM.Channel t = GetGSMCHfromFreqDN(GSMUniFreqSelected[(int)ssch.dwFrequencyIndex]);
                                    LGSM.BTSData dt = new LGSM.BTSData()
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
                                    dt.GetStationInfo();
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
                                            GSMBTS[i].LastLevelUpdete = _timeService.GetGnssUtcTime().Ticks - TicksBefore1970;// LocalTime;

                                            GSMBTS[i].GetStationInfo();

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
                                        }
                                    }
                                    if (find == false)
                                    {
                                        LGSM.Channel t = GetGSMCHfromFreqDN(GSMUniFreqSelected[(int)pr.dwFrequencyIndex]);
                                        LGSM.BTSData dt = new LGSM.BTSData()
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
                                            LastLevelUpdete = _timeService.GetGnssUtcTime().Ticks - TicksBefore1970
                                        };
                                        dt.GetStationInfo();
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
                                            GSMBTS[ii].GetStationInfo();
                                        }
                                        find = true;
                                    }
                                }
                                if (find == false)
                                {
                                    LGSM.Channel t = GetGSMCHfromFreqDN(GSMUniFreqSelected[(int)pData.ListCellIdentResults[i].dwFrequencyIndex]);
                                    LGSM.BTSData dt = new LGSM.BTSData()
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
                                    dt.GetStationInfo();
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
                                            GSMBTS[i].GetStationInfo();
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


            }
        }

        private class MyWcdmaDataProcessor : RohdeSchwarz.ViCom.Net.WCDMA.CViComWcdmaInterfaceDataProcessor
        {
            private readonly ILogger _logger;
            private readonly ITimeService _timeService;
            public MyWcdmaDataProcessor(ILogger logger, ITimeService timeService)
            {
                _logger = logger;
                _timeService = timeService;
            }
            public override void RegisterScannerId(ushort dwScannerDataId)
            {
                //Console.WriteLine("WcdmaDataProcessor registered at Scanner ID {0}", dwScannerDataId.ToString());
            }
            public override void RemoveScannerId(ushort dwScannerDataId)
            {
                //Console.WriteLine("WcdmaDataProcessor unregistered from Scanner ID {0}", dwScannerDataId.ToString());
            }
            public override void OnScannerDataMeasured(RohdeSchwarz.ViCom.Net.WCDMA.SMeasResult pData)
            {
                _LastUpdate = DateTime.Now.Ticks;
                if (UMTSIsRuning && UMTSUpdateData)
                {
                    try
                    {
                        if (pData != null && pData.ListOfCPichCirs != null && pData.ListOfCPichCirs.Count > 0)
                        {
                            #region
                            foreach (var cpichResult in pData.ListOfCPichCirs)
                            {
                                bool find = false;
                                if (UMTSBTS.Count > 0)
                                {
                                    for (int i = 0; i < UMTSBTS.Count; i++)
                                    {
                                        if (UMTSBTS[i].FreqIndex == pData.dwChannelIndex && UMTSBTS[i].SC == cpichResult.ExtendedSC.wSC / 16 /*&& WCDMABTSfromDev[i].UCID == cpichResult.pBchCellIdentification.dwCI.ToString()*/)
                                        {
                                            UMTSBTS[i].RSCP = cpichResult.sRSCPInDBm100 * 0.01;
                                            UMTSBTS[i].LastLevelUpdete = _timeService.GetGnssUtcTime().Ticks - TicksBefore1970;
                                            UMTSBTS[i].GetStationInfo();
                                            if (UMTSBTS[i].RSCP > DetectionLevelUMTS)/////////////////////////////////////////////////////////////////////
                                            { UMTSBTS[i].LastDetectionLevelUpdete = _timeService.GetGnssUtcTime().Ticks; }
                                            //UMTSBTS[i].DeleteFromMeasMon = (UMTSBTS[i].RSCP < DetectionLevelUMTS - LevelDifferenceToRemove);

                                            UMTSBTS[i].ISCP = cpichResult.sISCPInDBm100 * 0.01;
                                            UMTSBTS[i].InbandPower = cpichResult.sInbandPowerInDBm100 * 0.01;

                                            if (cpichResult.psCodePowerInDBm100 != null) UMTSBTS[i].CodePower = (double)cpichResult.psCodePowerInDBm100 * 0.01;
                                            UMTSBTS[i].IcIo = Math.Round(cpichResult.sRSCPInDBm100 * 0.01 - cpichResult.sInbandPowerInDBm100 * 0.01, 2);
                                            ////////////////                                           

                                            if (cpichResult.pBchCellIdentification != null && cpichResult.pBchCellIdentification.wLAC != 65535)
                                            {
                                                if (UMTSBTS[i].MNC != Convert.ToInt32(cpichResult.pBchCellIdentification.wMNC.ToString("X")) ||
                                                    UMTSBTS[i].MCC != Convert.ToInt32(cpichResult.pBchCellIdentification.wMCC.ToString("X")) ||
                                                    UMTSBTS[i].LAC != cpichResult.pBchCellIdentification.wLAC ||
                                                    UMTSBTS[i].UCID != (int)cpichResult.pBchCellIdentification.dwCI
                                                    )
                                                {
                                                    UMTSBTS[i].MNC = Convert.ToInt32(cpichResult.pBchCellIdentification.wMNC.ToString("X"));//.ToString();
                                                    UMTSBTS[i].MCC = Convert.ToInt32(cpichResult.pBchCellIdentification.wMCC.ToString("X"));//.ToString();
                                                    UMTSBTS[i].LAC = cpichResult.pBchCellIdentification.wLAC;//.ToString();
                                                    UMTSBTS[i].UCID = (int)cpichResult.pBchCellIdentification.dwCI;//.ToString();
                                                    UMTSBTS[i].RNC = (int)(UMTSBTS[i].UCID / 65536);//.ToString();
                                                    UMTSBTS[i].CID = (int)(UMTSBTS[i].UCID % 65536);//.ToString();

                                                    bool fulldata = (UMTSBTS[i].MNC != -1 && UMTSBTS[i].MCC != -1 && UMTSBTS[i].LAC != -1 && UMTSBTS[i].CID != -1);
                                                    if (fulldata)
                                                    {
                                                        UMTSBTS[i].GCID = UMTSBTS[i].MCC.ToString() + " " + UMTSBTS[i].MNC.ToString() + " " +
                                                          string.Format("{0:00000}", UMTSBTS[i].LAC) + " " + string.Format("{0:00000}", UMTSBTS[i].CID);
                                                    }
                                                    #region                                                    
                                                    #endregion
                                                    UMTSBTS[i].FullData = fulldata;
                                                }
                                            }
                                            bool freqLevelMax = true;
                                            for (int l = 0; l < UMTSBTS.Count; l++)
                                            {
                                                if (UMTSBTS[l].FreqDn == UMTSBTS[i].FreqDn &&
                                                    UMTSBTS[l].GCID != UMTSBTS[i].GCID)
                                                {
                                                    if (UMTSBTS[l].RSCP + LevelIsMaxIfMoreBy < UMTSBTS[i].RSCP)
                                                        UMTSBTS[l].ThisIsMaximumSignalAtThisFrequency = false;
                                                    else { freqLevelMax = false; }
                                                }
                                            }
                                            UMTSBTS[i].ThisIsMaximumSignalAtThisFrequency = freqLevelMax;

                                            find = true;
                                        }
                                    }
                                }
                                #region добавляем новое
                                if (find == false)
                                {
                                    try
                                    {
                                        if (cpichResult.ExtendedSC != null)
                                        {
                                            uint FreqIndex = pData.dwChannelIndex;
                                            LUMTS.Channel t = GetUMTSCHFromFreqDN(UMTSUniFreq[(int)pData.dwChannelIndex]);
                                            int SC = cpichResult.ExtendedSC.wSC / 16;
                                            int MCC = -1;
                                            int MNC = -1;
                                            int LAC = -1;
                                            int UCID = -1;
                                            int RNC = -1;
                                            int CID = -1;
                                            if (cpichResult.pBchCellIdentification != null)
                                            {
                                                MCC = Convert.ToInt32(cpichResult.pBchCellIdentification.wMCC.ToString("X"));
                                                MNC = Convert.ToInt32(cpichResult.pBchCellIdentification.wMNC.ToString("X"));
                                                LAC = cpichResult.pBchCellIdentification.wLAC;//.ToString();
                                                if (LAC == 65535) LAC = -1;
                                                UCID = (int)cpichResult.pBchCellIdentification.dwCI;
                                                RNC = (int)(UCID / 65536);
                                                CID = (int)(UCID % 65536);//.ToString();
                                            }
                                            bool FullData = (MCC != -1 && MNC != -1 && LAC != -1 && CID != -1);
                                            string GCID = "";
                                            if (FullData) { GCID = MCC.ToString() + " " + MNC.ToString() + " " + string.Format("{0:00000}", LAC) + " " + string.Format("{0:00000}", CID); }

                                            double ISCP = cpichResult.sISCPInDBm100 * 0.01;
                                            double RSCP = cpichResult.sRSCPInDBm100 * 0.01;
                                            double InbandPower = cpichResult.sInbandPowerInDBm100 * 0.01;
                                            double CodePower = -1000;
                                            if (cpichResult.psCodePowerInDBm100 != null) CodePower = (double)cpichResult.psCodePowerInDBm100 * 0.01;
                                            double IcIo = Math.Round(cpichResult.sRSCPInDBm100 * 0.01 - cpichResult.sInbandPowerInDBm100 * 0.01, 2);
                                            long LastLevelUpdete = _timeService.GetGnssUtcTime().Ticks - TicksBefore1970;

                                            LUMTS.BTSData dt = new LUMTS.BTSData()
                                            {
                                                FreqIndex = FreqIndex,
                                                //Channel = t,
                                                UARFCN_DN = t.UARFCN_DN,
                                                UARFCN_UP = t.UARFCN_UP,
                                                FreqDn = t.FreqDn,
                                                FreqUp = t.FreqUp,
                                                StandartSubband = t.StandartSubband,
                                                SC = SC,
                                                MCC = MCC,
                                                MNC = MNC,
                                                LAC = LAC,
                                                FullData = FullData,
                                                RNC = RNC,
                                                CID = CID,
                                                UCID = UCID,
                                                GCID = GCID,
                                                ISCP = ISCP,
                                                RSCP = RSCP,
                                                InbandPower = InbandPower,
                                                CodePower = CodePower,
                                                IcIo = IcIo,
                                                LastLevelUpdete = LastLevelUpdete
                                            };
                                            dt.GetStationInfo();
                                            UMTSBTS.Add(dt);

                                        }
                                    }
                                    catch (RohdeSchwarz.ViCom.Net.CViComError error)
                                    {
                                        _logger.Exception(Contexts.ThisComponent, new Exception("ErrorCode:" + error.ErrorCode + " ErrorString:" + error.ErrorString));
                                    }
                                    catch (Exception exp)
                                    {
                                        _logger.Exception(Contexts.ThisComponent, exp);
                                    }
                                }
                                #endregion
                            }
                            #endregion
                        }
                        #region Sib
                        if (pData.pDemodResult != null && pData.pDemodResult.pbBitStream.Length > 0)
                        {
                            try
                            {
                                RohdeSchwarz.ViCom.Net.WCDMA.SL3DecoderRequest dec = new RohdeSchwarz.ViCom.Net.WCDMA.SL3DecoderRequest()
                                {
                                    dwBitCount = pData.pDemodResult.dwBitCount,
                                    ePDU = pData.pDemodResult.ePDU,
                                    pbBitStream = pData.pDemodResult.pbBitStream
                                };
                                RohdeSchwarz.ViCom.Net.WCDMA.SL3DecoderResult dr = new RohdeSchwarz.ViCom.Net.WCDMA.SL3DecoderResult();
                                dr = wcdmaInterface.RetrieveTextForPDU(dec, SDefs.dwDefaultTimeOutInMs);
                                if (pData.pDemodResult.pbBitStream.Length > 0)
                                {
                                    for (int i = 0; i < UMTSBTS.Count; i++)
                                    {
                                        if (pData.dwChannelIndex == UMTSBTS[i].FreqIndex && pData.pDemodResult.ExtendedSC.wSC / 16 == UMTSBTS[i].SC)
                                        {
                                            #region save sib data
                                            bool fib = false;
                                            List<COMRMSI.SystemInformationBlock> ibs = new List<COMRMSI.SystemInformationBlock>() { };
                                            if (UMTSBTS[i].SysInfoBlocks != null)
                                            {
                                                ibs = new List<COMRMSI.SystemInformationBlock>(UMTSBTS[i].SysInfoBlocks);
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
                                                ibs.Add(
                                                    new COMRMSI.SystemInformationBlock()
                                                    {
                                                        DataString = dr.pcPduText,
                                                        Type = pData.pDemodResult.ePDU.ToString()
                                                    });
                                                UMTSBTS[i].SysInfoBlocks = ibs.ToArray();
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
            }
        }

        private class MyLteDataProcessor : RohdeSchwarz.ViCom.Net.LTE.CViComLteInterfaceDataProcessor
        {
            private readonly ILogger _logger;
            private readonly ITimeService _timeService;
            public MyLteDataProcessor(ILogger logger, ITimeService timeService)
            {
                _logger = logger;
                _timeService = timeService;
            }
            public override void RegisterScannerId(ushort dwScannerDataId)
            {
                //Console.WriteLine("LteDataProcessor registered at Scanner ID {0} ", dwScannerDataId.ToString());
            }

            public override void RemoveScannerId(ushort dwScannerDataId)
            {
                //Console.WriteLine("LteDataProcessor unregistered from Scanner ID {0}", dwScannerDataId.ToString());
            }

            public override void OnScannerDataMeasured(RohdeSchwarz.ViCom.Net.LTE.SMeasResult pData)
            {
                if (LTEIsRuning && LTEUpdateData)
                {
                    _LastUpdate = DateTime.Now.Ticks;
                    if (pData.ListOfSignals != null)
                    {
                        foreach (var signalres in pData.ListOfSignals)
                        {
                            bool find = false;
                            for (int i = 0; i < LTEBTS.Count; i++)
                            {
                                if (pData.dwChannelIndex == LTEBTS[i].FreqIndex && signalres.dwScannerBtsIdent == LTEBTS[i].FreqBtsIndex)
                                {
                                    find = true;
                                    if (signalres.sRefSignal.sPBCHbasedRSRPinDBm100 != short.MaxValue)
                                    {
                                        LTEBTS[i].RSRP = signalres.sRefSignal.sPBCHbasedRSRPinDBm100 * 0.01;
                                        LTEBTS[i].LastLevelUpdete = _timeService.GetGnssUtcTime().Ticks - TicksBefore1970;
                                        LTEBTS[i].GetStationInfo();
                                        //if (LTEBTS[i].RSRP > DetectionLevelLTE)/////////////////////////////////////////////////////////////////////
                                        //{ LTEBTS[i].LastDetectionLevelUpdete = MainWindow.gps.LocalTime.Ticks; }
                                        bool freqLevelMax = true;
                                        for (int l = 0; l < LTEBTS.Count; l++)
                                        {
                                            if (LTEBTS[l].FreqDn == LTEBTS[i].FreqDn &&
                                                LTEBTS[l].GCID != LTEBTS[i].GCID)
                                            {
                                                if (LTEBTS[l].RSRP + LevelIsMaxIfMoreBy < LTEBTS[i].RSRP)
                                                    LTEBTS[l].ThisIsMaximumSignalAtThisFrequency = false;
                                                else { freqLevelMax = false; }
                                            }

                                        }
                                        LTEBTS[i].ThisIsMaximumSignalAtThisFrequency = freqLevelMax;
                                    }
                                    if (signalres.sRefSignal.sPBCHbasedRSRQinDB100 != short.MaxValue)
                                    { LTEBTS[i].RSRQ = signalres.sRefSignal.sPBCHbasedRSRQinDB100 * 0.01; }
                                }
                            }
                            if (find == false)
                            {
                                LLTE.Channel t = GetLTECHfromFreqDN(LTEUniFreq[(int)pData.dwChannelIndex]);
                                LLTE.BTSData ltebts = new LLTE.BTSData()
                                {
                                    FreqIndex = pData.dwChannelIndex,
                                    FreqBtsIndex = signalres.dwScannerBtsIdent,
                                    //Channel = t,
                                    EARFCN_DN = t.EARFCN_DN,
                                    EARFCN_UP = t.EARFCN_UP,
                                    FreqDn = t.FreqDn,
                                    FreqUp = t.FreqUp,
                                    StandartSubband = t.StandartSubband,
                                    PCI = signalres.wPhysicalCellId,
                                };
                                if (signalres.sRefSignal != null)
                                {
                                    if (signalres.sRefSignal.sPBCHbasedRSRPinDBm100 != short.MaxValue)
                                    {
                                        ltebts.RSRP = signalres.sRefSignal.sPBCHbasedRSRPinDBm100 * 0.01;
                                        ltebts.LastLevelUpdete = _timeService.GetGnssUtcTime().Ticks - TicksBefore1970;
                                    }
                                    if (signalres.sRefSignal.sPBCHbasedRSRQinDB100 != short.MaxValue)
                                        ltebts.RSRQ = signalres.sRefSignal.sPBCHbasedRSRQinDB100 * 0.01;
                                }
                                ltebts.GetStationInfo();
                                LTEBTS.Add(ltebts);

                            }
                        }
                    }
                    if (pData.ListOfWidebandRsCinrResults != null)
                    {
                        foreach (var signalres in pData.ListOfWidebandRsCinrResults)
                        {
                            for (int i = 0; i < LTEBTS.Count; i++)
                            {
                                if (pData.dwChannelIndex == LTEBTS[i].FreqIndex && signalres.dwScannerBtsIdent == LTEBTS[i].FreqBtsIndex)
                                {
                                    if (signalres.sRSRPinDBm100 != short.MaxValue)
                                        LTEBTS[i].WB_RSRP = signalres.sRSRPinDBm100 * 0.01;
                                    if (signalres.sRSRQinDB100 != short.MaxValue)
                                        LTEBTS[i].WB_RSRQ = signalres.sRSRQinDB100 * 0.01;
                                    if (signalres.sRsRssiInDBm100 != short.MaxValue)
                                        LTEBTS[i].WB_RS_RSSI = signalres.sRsRssiInDBm100 * 0.01;
                                }
                            }
                        }
                    }
                    #region Sib
                    if (pData.pDemodResult != null && pData.pDemodResult.pbBitStream.Length > 0)
                    {
                        try
                        {
                            RohdeSchwarz.ViCom.Net.LTE.SL3DecoderRequest dec = new RohdeSchwarz.ViCom.Net.LTE.SL3DecoderRequest()
                            {
                                dwBitCount = pData.pDemodResult.dwBitCount,
                                ePDU = pData.pDemodResult.ePDU,
                                pbBitStream = pData.pDemodResult.pbBitStream
                            };
                            RohdeSchwarz.ViCom.Net.LTE.SL3DecoderResult dr = new RohdeSchwarz.ViCom.Net.LTE.SL3DecoderResult();
                            dr = lteInterface.RetrieveTextForPDU(dec, SDefs.dwDefaultTimeOutInMs);
                            if (pData.pDemodResult.ePDU == RohdeSchwarz.ViCom.Net.LTE.Pdu.Type.MIB)
                            {
                                if (pData.pDemodResult.pbBitStream.Length > 0)
                                {
                                    decimal[] data = ParseMIBLTE(dr.pcPduText);
                                    for (int i = 0; i < LTEBTS.Count; i++)
                                    {
                                        if (pData.dwChannelIndex == LTEBTS[i].FreqIndex && pData.pDemodResult.dwBtsId == LTEBTS[i].FreqBtsIndex && pData.pDemodResult.wPhysicalCellId == LTEBTS[i].PCI)
                                        {
                                            LTEBTS[i].Bandwidth = data[0];
                                            #region save sib data
                                            List<COMRMSI.SystemInformationBlock> ibs = new List<COMRMSI.SystemInformationBlock>() { };
                                            bool fib = false;
                                            if (LTEBTS[i].SysInfoBlocks != null)
                                            {
                                                ibs = new List<COMRMSI.SystemInformationBlock>(LTEBTS[i].SysInfoBlocks);
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
                                                LTEBTS[i].SysInfoBlocks = ibs.ToArray();
                                            }
                                            #endregion
                                        }
                                    }
                                }
                            }
                            else if (pData.pDemodResult.ePDU == RohdeSchwarz.ViCom.Net.LTE.Pdu.Type.SIB1)
                            {
                                if (pData.pDemodResult.pbBitStream.Length > 0)
                                {
                                    int[] data = ParseSIB1LTE(dr.pcPduText);
                                    for (int i = 0; i < LTEBTS.Count; i++)
                                    {
                                        if (pData.dwChannelIndex == LTEBTS[i].FreqIndex &&
                                            pData.pDemodResult.dwBtsId == LTEBTS[i].FreqBtsIndex &&
                                            pData.pDemodResult.wPhysicalCellId == LTEBTS[i].PCI)
                                        {
                                            LTEBTS[i].MCC = data[0];
                                            LTEBTS[i].MNC = data[1];
                                            LTEBTS[i].TAC = data[2];
                                            LTEBTS[i].CelId28 = data[3];
                                            LTEBTS[i].eNodeBId = data[4];
                                            LTEBTS[i].CID = data[5];
                                            #region save sib data
                                            List<COMRMSI.SystemInformationBlock> ibs = new List<COMRMSI.SystemInformationBlock>() { };
                                            bool fib = false;
                                            if (LTEBTS[i].SysInfoBlocks != null)
                                            {
                                                ibs = new List<COMRMSI.SystemInformationBlock>(LTEBTS[i].SysInfoBlocks);
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
                                                LTEBTS[i].SysInfoBlocks = ibs.ToArray();
                                            }
                                            #endregion
                                            bool FullData = (LTEBTS[i].MCC != -1 && LTEBTS[i].MNC != -1 && LTEBTS[i].eNodeBId != -1 && LTEBTS[i].CID != -1);
                                            string GCID = "";
                                            if (FullData)
                                            {
                                                GCID = LTEBTS[i].MCC.ToString() + " " + LTEBTS[i].MNC.ToString() + " " +
                                                  string.Format("{0:000000}", LTEBTS[i].eNodeBId) + " " + string.Format("{0:000}", LTEBTS[i].CID);
                                                LTEBTS[i].GCID = GCID;
                                            }
                                            LTEBTS[i].FullData = FullData;
                                        }
                                    }
                                }
                            }
                            else
                            {
                                if (pData.pDemodResult.pbBitStream.Length > 0)
                                {
                                    int[] data = ParseSIB1LTE(dr.pcPduText);
                                    for (int i = 0; i < LTEBTS.Count; i++)
                                    {
                                        if (pData.dwChannelIndex == LTEBTS[i].FreqIndex && pData.pDemodResult.dwBtsId == LTEBTS[i].FreqBtsIndex && pData.pDemodResult.wPhysicalCellId == LTEBTS[i].PCI)
                                        {
                                            #region save sib data
                                            List<COMRMSI.SystemInformationBlock> ibs = new List<COMRMSI.SystemInformationBlock>() { };
                                            bool fib = false;
                                            if (LTEBTS[i].SysInfoBlocks != null)
                                            {
                                                for (int ib = 0; ib < ibs.Count; ib++)
                                                {
                                                    ibs = new List<COMRMSI.SystemInformationBlock>(LTEBTS[i].SysInfoBlocks);
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
                                                LTEBTS[i].SysInfoBlocks = ibs.ToArray();
                                            }
                                            #endregion
                                        }
                                    }
                                }
                            }
                        }
                        catch { }
                    }
                    #endregion
                }
            }
        }

        private class MyCdmaDataProcessor : RohdeSchwarz.ViCom.Net.CDMA.CViComCdmaInterfaceDataProcessor
        {
            private readonly ILogger _logger;
            private readonly ITimeService _timeService;
            public MyCdmaDataProcessor(ILogger logger, ITimeService timeService)
            {
                _logger = logger;
                _timeService = timeService;
            }
            public override void RegisterScannerId(ushort dwScannerDataId)
            {
                //Console.WriteLine("CdmaDataProcessor registered at Scanner ID {0}", dwScannerDataId.ToString());
            }
            public override void RemoveScannerId(ushort dwScannerDataId)
            {
                //Console.WriteLine("CdmaDataProcessor unregistered from Scanner ID {0}", dwScannerDataId.ToString());
            }
            public override void OnScannerDataMeasured(RohdeSchwarz.ViCom.Net.CDMA.SMeasResult pData)
            {
                if (CDMAIsRuning && CDMAUpdateData)
                {
                    try
                    {
                        _LastUpdate = DateTime.Now.Ticks;
                        //Settings.CDMAFreqs_Set t = CDMAUniFreq[(int)pData.dwChannelIndex];
                        LCDMA.Channel tt = GetCDMACHfromFreqDN(CDMAUniFreq[(int)pData.dwChannelIndex].FreqDn, CDMAUniFreq[(int)pData.dwChannelIndex].EVDOvsCDMA);
                        #region 
                        if (pData.ListOfFPichCirs != null)
                        {
                            foreach (var fpichResult in pData.ListOfFPichCirs)
                            {
                                if (fpichResult.ExtendedPNOffset.wPNOffset != 65535)
                                {
                                    bool find = false;
                                    for (int i = 0; i < CDMABTS.Count; i++)
                                    {
                                        if (CDMABTS[i].FreqIndex == pData.dwChannelIndex && CDMABTS[i].Indicator == fpichResult.ExtendedPNOffset.wIndicator && CDMABTS[i].PN == fpichResult.ExtendedPNOffset.wPNOffset)
                                        {
                                            find = true;
                                            CDMABTS[i].PTotal = fpichResult.sInbandPowerInDBm100 * 0.01;
                                            CDMABTS[i].RSCP = fpichResult.sRSCPInDBm100 * 0.01;
                                            CDMABTS[i].IcIo = 0 - Math.Round(fpichResult.sInbandPowerInDBm100 * 0.01 - fpichResult.sRSCPInDBm100 * 0.01, 2);
                                            CDMABTS[i].LastLevelUpdete = _timeService.GetGnssUtcTime().Ticks - TicksBefore1970;
                                            CDMABTS[i].GetStationInfo();
                                            //if (CDMABTS[i].RSCP > DetectionLevelCDMA)/////////////////////////////////////////////////////////////////////
                                            //{ CDMABTS[i].LastDetectionLevelUpdete = MainWindow.gps.LocalTime.Ticks; }
                                            //CDMABTS[i].DeleteFromMeasMon = (CDMABTS[i].RSCP < DetectionLevelCDMA - LevelDifferenceToRemove);
                                            bool freqLevelMax = true;
                                            for (int l = 0; l < CDMABTS.Count; l++)
                                            {
                                                if (CDMABTS[l].FreqDn == CDMABTS[i].FreqDn &&
                                                    CDMABTS[l].GCID != CDMABTS[i].GCID)
                                                {
                                                    if (CDMABTS[l].RSCP + LevelIsMaxIfMoreBy < CDMABTS[i].RSCP)
                                                        CDMABTS[l].ThisIsMaximumSignalAtThisFrequency = false;
                                                    else { freqLevelMax = false; }
                                                }

                                            }
                                            CDMABTS[i].ThisIsMaximumSignalAtThisFrequency = freqLevelMax;
                                        }
                                    }
                                    if (find == false)
                                    {
                                        LCDMA.BTSData dt = new LCDMA.BTSData()
                                        {
                                            FreqIndex = pData.dwChannelIndex,
                                            Indicator = fpichResult.ExtendedPNOffset.wIndicator,
                                            Type = tt.EVDOvsCDMA,
                                            //Channel = tt,
                                            Channel = tt.ChannelN,
                                            FreqDn = tt.FreqDn,
                                            FreqUp = tt.FreqUp,
                                            StandartSubband = tt.StandartSubband,
                                            PN = fpichResult.ExtendedPNOffset.wPNOffset,
                                            PTotal = fpichResult.sInbandPowerInDBm100 * 0.01,
                                            RSCP = fpichResult.sRSCPInDBm100 * 0.01,
                                            IcIo = 0 - Math.Round(fpichResult.sInbandPowerInDBm100 * 0.01 - fpichResult.sRSCPInDBm100 * 0.01, 2),
                                            LastLevelUpdete = _timeService.GetGnssUtcTime().Ticks - TicksBefore1970
                                        };
                                        dt.GetStationInfo();
                                        CDMABTS.Add(dt);
                                    }
                                }
                            }
                        }
                        #endregion
                        if (pData.pSyncChannelDemodulationResult != null)
                        {
                            bool find = false;
                            for (int i = 0; i < CDMABTS.Count; i++)
                            {
                                if (CDMABTS[i].FreqIndex == pData.dwChannelIndex &&
                                    CDMABTS[i].Indicator == pData.pDemodResult.wFirstBtsId &&
                                    CDMABTS[i].PN == pData.pSyncChannelDemodulationResult.wPILOT_PN)
                                {
                                    find = true;
                                    if (CDMABTS[i].SID != pData.pSyncChannelDemodulationResult.wSID ||
                                        CDMABTS[i].NID != pData.pSyncChannelDemodulationResult.wNID)
                                    {
                                        CDMABTS[i].SID = pData.pSyncChannelDemodulationResult.wSID;
                                        CDMABTS[i].NID = pData.pSyncChannelDemodulationResult.wNID;
                                    }
                                }
                            }
                            if (find == false)
                            {
                                LCDMA.BTSData dt = new LCDMA.BTSData()
                                {
                                    FreqIndex = pData.dwChannelIndex,
                                    Indicator = pData.pDemodResult.wBtsId,
                                    Channel = tt.ChannelN,
                                    FreqDn = tt.FreqDn,
                                    FreqUp = tt.FreqUp,
                                    StandartSubband = tt.StandartSubband,
                                    PN = pData.pSyncChannelDemodulationResult.wPILOT_PN,
                                    Type = tt.EVDOvsCDMA,
                                    SID = pData.pSyncChannelDemodulationResult.wSID,
                                    NID = pData.pSyncChannelDemodulationResult.wNID,
                                };
                                dt.GetStationInfo();
                                CDMABTS.Add(dt);
                            }
                        }
                        #region Sib
                        if (pData.pDemodResult != null)
                        {
                            try
                            {
                                RohdeSchwarz.ViCom.Net.CDMA.SL3DecoderRequest dec = new RohdeSchwarz.ViCom.Net.CDMA.SL3DecoderRequest()
                                {
                                    dwBitCount = pData.pDemodResult.dwBitCount,
                                    PduSpec = pData.pDemodResult.PduSpec,
                                    pbBitStream = pData.pDemodResult.pbBitStream
                                };
                                RohdeSchwarz.ViCom.Net.CDMA.SL3DecoderResult dr = new RohdeSchwarz.ViCom.Net.CDMA.SL3DecoderResult();
                                dr = cdmaInterface.RetrieveTextForPDU(dec, SDefs.dwDefaultTimeOutInMs);

                                for (int i = 0; i < CDMABTS.Count; i++)
                                {
                                    if (pData.dwChannelIndex == CDMABTS[i].FreqIndex && pData.pDemodResult.wFirstBtsId == CDMABTS[i].Indicator)
                                    {
                                        #region save sib data
                                        List<COMRMSI.SystemInformationBlock> ibs = new List<COMRMSI.SystemInformationBlock>() { };
                                        bool fib = false;
                                        if (CDMABTS[i].SysInfoBlocks != null)
                                        {
                                            ibs = new List<COMRMSI.SystemInformationBlock>(CDMABTS[i].SysInfoBlocks);
                                            for (int ib = 0; ib < ibs.Count(); ib++)
                                            {
                                                if (ibs[ib].Type == pData.pDemodResult.PduSpec.ePDU.ToString())
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
                                                Type = pData.pDemodResult.PduSpec.ePDU.ToString()
                                            };
                                            ibs.Add(sib);
                                            CDMABTS[i].SysInfoBlocks = ibs.ToArray();
                                        }
                                        #endregion save sib data
                                        #region parse
                                        if (dr.ePDU == RohdeSchwarz.ViCom.Net.CDMA.Pdu.Type.EVDO_SECTOR_PARAMETERS)
                                        {
                                            int[] data = ParseEVDO_SECTOR_PARAMETERS(dr.pcPduText);
                                            CDMABTS[i].SID = data[0];
                                            CDMABTS[i].NID = 0;
                                            CDMABTS[i].BaseID = data[1];

                                            bool FullData = CDMABTS[i].BaseID != -1 && CDMABTS[i].SID != -1 && CDMABTS[i].NID != -1;
                                            if (FullData)
                                            {
                                                string GCID = CDMABTS[i].NID.ToString() + " " + CDMABTS[i].SID.ToString() + " " +
                                                    string.Format("{0:00000}", CDMABTS[i].PN) + " " + string.Format("{0:000000}", CDMABTS[i].BaseID);
                                                CDMABTS[i].GCID = GCID;
                                            }
                                            CDMABTS[i].FullData = FullData;
                                        }
                                        if (dr.ePDU == RohdeSchwarz.ViCom.Net.CDMA.Pdu.Type.SYS_PARAMS)
                                        {
                                            decimal[] data = ParseCDMA_SYS_PARAMS(dr.pcPduText);
                                            CDMABTS[i].BaseID = (int)data[0];
                                            if (CDMABTS[i].BaseID != -1 && CDMABTS[i].SID != -1 && CDMABTS[i].NID != -1)
                                            {
                                                CDMABTS[i].GCID = CDMABTS[i].NID.ToString() + " " + CDMABTS[i].SID.ToString() + " " +
                                                      string.Format("{0:00000}", CDMABTS[i].PN) + " " + string.Format("{0:000000}", CDMABTS[i].BaseID);
                                                CDMABTS[i].FullData = true;
                                            }
                                        }
                                        if (dr.ePDU == RohdeSchwarz.ViCom.Net.CDMA.Pdu.Type.EXT_SYS_PARAMS)
                                        {
                                            decimal[] data = ParseCDMA_EXT_SYS_PARAMS(dr.pcPduText);
                                            CDMABTS[i].MCC = (int)data[0];
                                            CDMABTS[i].MNC = (int)data[1];
                                        }
                                        #endregion
                                        CDMABTS[i].GetStationInfo();
                                    }
                                }
                            }
                            catch (Exception e)
                            {

                            }
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
            }

        }

        private static LGSM.Channel GetGSMCHfromFreqDN(decimal freq_Dn)
        {
            bool find = false;
            freq_Dn = freq_Dn / 1000000;
            LGSM.Channel temp = new LGSM.Channel();
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
        private static LUMTS.Channel GetUMTSCHFromFreqDN(decimal freq_Dn)
        {
            bool find = false;
            freq_Dn = freq_Dn / 1000000;
            LUMTS.Channel temp = new LUMTS.Channel();
            //Settings.WCDMAFreqs_Set temp = new Settings.WCDMAFreqs_Set();
            if (find == false && freq_Dn >= 2112.4m && freq_Dn <= 2167.6m)
            {
                #region
                List<decimal> tf = new List<decimal>() { };
                for (int i = 10562; i <= 10838; i++)
                {
                    tf.Add(2112.4m + 0.2m * (i - 10562));
                }
                for (int i = 0; i < tf.Count; i++)
                {
                    if (tf[i] == freq_Dn)
                    {
                        find = true;
                        temp.FreqDn = freq_Dn;
                        temp.UARFCN_DN = (int)((temp.FreqDn - 2112.4m) * 5 + 10562);
                        temp.FreqUp = freq_Dn - 190;
                        temp.UARFCN_UP = (int)((temp.FreqUp - 1922.4m) * 5 + 9612);
                        temp.StandartSubband = "Band-1 2100";// Equipment.UMTSBands.Band_1_2100.ToString();
                    }
                }
                #endregion
            }
            else if (find == false && freq_Dn >= 1932.4m && freq_Dn <= 1987.6m)
            {
                #region
                List<decimal> tf = new List<decimal>() { };
                for (int i = 9662; i <= 9938; i++)
                {
                    tf.Add(1932.4m + 0.2m * (i - 9662));
                }
                for (int i = 0; i < tf.Count; i++)
                {
                    if (tf[i] == freq_Dn)
                    {
                        find = true;
                        temp.FreqDn = freq_Dn;
                        temp.UARFCN_DN = (int)((temp.FreqDn - 1932.4m) * 5 + 9662);
                        temp.FreqUp = freq_Dn - 80;
                        temp.UARFCN_UP = (int)((temp.FreqUp - 1852.4m) * 5 + 9262);
                        temp.StandartSubband = "Band-2 1900";// Equipment.UMTSBands.Band_2_1900.ToString();
                    }
                }
                if (find == false)
                {
                    tf.Clear();
                    for (int i = 412; i <= 687; i += 25)
                    {
                        tf.Add(1932.5m + 0.2m * (i - 412));
                    }
                    for (int i = 0; i < tf.Count; i++)
                    {
                        if (tf[i] == freq_Dn)
                        {
                            find = true;
                            temp.FreqDn = freq_Dn;
                            temp.UARFCN_DN = (int)(temp.FreqDn - 1932.5m) * 5 + 412;
                            temp.FreqUp = freq_Dn - 80;
                            temp.UARFCN_UP = (int)(temp.FreqUp - 1852.5m) * 5 + 12;
                            temp.StandartSubband = "Band-2 1900";//  Equipment.UMTSBands.Band_2_1900.ToString();
                        }
                    }
                }
                #endregion
            }
            else if (find == false && freq_Dn >= 1807.4m && freq_Dn <= 1877.6m)
            {
                #region
                List<decimal> tf = new List<decimal>() { };
                for (int i = 1162; i <= 1513; i++)
                {
                    tf.Add(1807.4m + 0.2m * (i - 1162));
                }
                for (int i = 0; i < tf.Count; i++)
                {
                    if (tf[i] == freq_Dn)
                    {
                        find = true;
                        temp.FreqDn = freq_Dn;
                        temp.UARFCN_DN = (int)((temp.FreqDn - 1932.4m) * 5 + 1162);
                        temp.FreqUp = freq_Dn - 95;
                        temp.UARFCN_UP = (int)((temp.FreqUp - 1712.4m) * 5 + 937);
                        temp.StandartSubband = "Band-3 1800";// Equipment.UMTSBands.Band_3_1800.ToString();
                    }
                }
                #endregion
            }
            else if (find == false && freq_Dn >= 2112.4m && freq_Dn <= 2152.6m)
            {
                #region
                List<decimal> tf = new List<decimal>() { };
                for (int i = 1537; i <= 1738; i++)
                {
                    tf.Add(2112.4m + 0.2m * (i - 1537));
                }
                for (int i = 0; i < tf.Count; i++)
                {
                    if (tf[i] == freq_Dn)
                    {
                        find = true;
                        temp.FreqDn = freq_Dn;
                        temp.UARFCN_DN = (int)((temp.FreqDn - 2112.4m) * 5 + 1537);
                        temp.FreqUp = freq_Dn - 400;
                        temp.UARFCN_UP = (int)((temp.FreqUp - 1712.4m) * 5 + 1312);
                        temp.StandartSubband = "Band-4 1700";// Equipment.UMTSBands.Band_4_1700.ToString();
                    }
                }
                if (find == false)
                {
                    tf.Clear();
                    for (int i = 1887; i <= 2087; i += 25)
                    {
                        tf.Add(2112.5m + 0.2m * (i - 1887));
                    }
                    for (int i = 0; i < tf.Count; i++)
                    {
                        if (tf[i] == freq_Dn)
                        {
                            find = true;
                            temp.FreqDn = freq_Dn;
                            temp.UARFCN_DN = (int)((temp.FreqDn - 2112.5m) * 5 + 1887);
                            temp.FreqUp = freq_Dn - 400;
                            temp.UARFCN_UP = (int)((temp.FreqUp - 1712.5m) * 5 + 1662);
                            temp.StandartSubband = "Band-4 1700";//  Equipment.UMTSBands.Band_4_1700.ToString();
                        }
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
        private static LLTE.Channel GetLTECHfromFreqDN(decimal freq_Dn)
        {
            bool find = false;
            freq_Dn = freq_Dn / 1000000;
            LLTE.Channel temp = new LLTE.Channel();
            if (find == false && freq_Dn >= 2620.0m && freq_Dn <= 2689.9m)
            {
                #region
                List<decimal> tf = new List<decimal>() { };
                for (int i = 2750; i <= 3449; i++)
                {
                    tf.Add(2620.0m + 0.1m * (i - 2750));
                }
                for (int i = 0; i < tf.Count; i++)
                {
                    if (tf[i] == freq_Dn)
                    {
                        find = true;
                        temp.FreqDn = freq_Dn;
                        temp.EARFCN_DN = (int)((temp.FreqDn - 2620.0m) * 10 + 2750);
                        temp.FreqUp = freq_Dn - 120;
                        temp.EARFCN_UP = (int)((temp.FreqUp - 2500.0m) * 10 + 20750);
                        temp.StandartSubband = "Band-7 2600";// Equipment.UMTSBands.Band_1_2100.ToString();
                    }
                }
                #endregion
            }
            else if (find == false && freq_Dn >= 1805.0m && freq_Dn <= 1879.9m)
            {
                #region
                List<decimal> tf = new List<decimal>() { };
                for (int i = 1200; i <= 1949; i++)
                {
                    tf.Add(1805.0m + 0.1m * (i - 1200));
                }
                for (int i = 0; i < tf.Count; i++)
                {
                    if (tf[i] == freq_Dn)
                    {
                        find = true;
                        temp.FreqDn = freq_Dn;
                        temp.EARFCN_DN = (int)((temp.FreqDn - 1805.0m) * 10 + 1200);
                        temp.FreqUp = freq_Dn - 95;
                        temp.EARFCN_UP = (int)((temp.FreqUp - 1710.0m) * 10 + 19200);
                        temp.StandartSubband = "Band-3 1800";// Equipment.UMTSBands.Band_2_1900.ToString();
                    }
                }
                #endregion
            }
            if (find == false)
                throw new Exception("Частота " + freq_Dn + " МГц не соответствует стандартным сеткам частот!");
            temp.FreqDn = temp.FreqDn * 1000000;
            temp.FreqUp = temp.FreqUp * 1000000;
            return temp;
        }
        private static LCDMA.Channel GetCDMACHfromFreqDN(decimal freq_Dn, bool EVDOvsCDMA)
        {
            bool find = false;
            freq_Dn = freq_Dn / 1000000;
            LCDMA.Channel temp = new LCDMA.Channel();
            if (find == false && freq_Dn >= 860.04m && freq_Dn <= 869.01m)
            {
                #region 0 800
                List<decimal> tf = new List<decimal>() { };
                for (int i = 1024; i <= 1323; i++)
                {
                    tf.Add(860.04m + 0.03m * (i - 1024));
                }
                for (int i = 0; i < tf.Count; i++)
                {
                    if (tf[i] == freq_Dn)
                    {
                        find = true;
                        temp.FreqDn = freq_Dn;
                        temp.ChannelN = (int)((temp.FreqDn - 860.04m) / 0.03m) + 1024;
                        temp.FreqUp = freq_Dn - 45;
                        temp.StandartSubband = "Band-0 800";// Equipment.UMTSBands.Band_2_1900.ToString();
                    }
                }
                #endregion
            }
            if (find == false && freq_Dn >= 869.04m && freq_Dn <= 870)
            {
                #region 0 800
                List<decimal> tf = new List<decimal>() { };
                for (int i = 991; i <= 1023; i++)
                {
                    tf.Add(869.04m + 0.03m * (i - 991));
                }
                for (int i = 0; i < tf.Count; i++)
                {
                    if (tf[i] == freq_Dn)
                    {
                        find = true;
                        temp.FreqDn = freq_Dn;
                        temp.ChannelN = (int)((temp.FreqDn - 869.04m) / 0.03m) + 991;
                        temp.FreqUp = freq_Dn - 45;
                        temp.StandartSubband = "Band-0 800";//  Equipment.UMTSBands.Band_2_1900.ToString();
                    }
                }
                #endregion
            }
            if (find == false && freq_Dn >= 870.03m && freq_Dn <= 893.97m)
            {
                #region 0 800
                List<decimal> tf = new List<decimal>() { };
                for (int i = 1; i <= 799; i++)
                {
                    tf.Add(870.03m + 0.03m * (i - 1));
                }
                for (int i = 0; i < tf.Count; i++)
                {
                    if (tf[i] == freq_Dn)
                    {
                        find = true;
                        temp.FreqDn = freq_Dn;
                        temp.ChannelN = (int)((temp.FreqDn - 870.03m) / 0.03m) + 1;
                        temp.FreqUp = freq_Dn - 45;
                        temp.StandartSubband = "Band-0 800";//  Equipment.UMTSBands.Band_2_1900.ToString();
                    }
                }
                #endregion
            }
            if (find == false && freq_Dn >= 1930 && freq_Dn <= 1989.95m)
            {
                #region 1 1900
                List<decimal> tf = new List<decimal>() { };
                for (int i = 0; i <= 1199; i++)
                {
                    tf.Add(1930 + 0.05m * (i - 0));
                }
                for (int i = 0; i < tf.Count; i++)
                {
                    if (tf[i] == freq_Dn)
                    {
                        find = true;
                        temp.FreqDn = freq_Dn;
                        temp.ChannelN = (int)((temp.FreqDn - 1930) / 0.05m) + 0;
                        temp.FreqUp = freq_Dn - 80;
                        temp.StandartSubband = "Band-1 1900";// Equipment.UMTSBands.Band_2_1900.ToString();
                    }
                }
                #endregion
            }
            if (find == false && freq_Dn >= 420.0m && freq_Dn <= 429.975m)
            {
                #region 5 450
                List<decimal> tf = new List<decimal>() { };
                for (int i = 472; i <= 871; i++)
                {
                    tf.Add(420 + 0.025m * (i - 1024));
                }
                for (int i = 0; i < tf.Count; i++)
                {
                    if (tf[i] == freq_Dn)
                    {
                        find = true;
                        temp.FreqDn = freq_Dn;
                        temp.ChannelN = (int)((temp.FreqDn - 420) / 0.025m) + 472;
                        temp.FreqUp = freq_Dn - 10;
                        temp.StandartSubband = "Band-5 450";// Equipment.UMTSBands.Band_2_1900.ToString();
                    }
                }
                #endregion
            }
            if (find == false && freq_Dn >= 461.31m && freq_Dn <= 469.99m)
            {
                #region 5 450
                List<decimal> tf = new List<decimal>() { };
                for (int i = 1039; i <= 1473; i++)
                {
                    tf.Add(461.31m + 0.02m * (i - 1039));
                }
                for (int i = 0; i < tf.Count; i++)
                {
                    if (tf[i] == freq_Dn)
                    {
                        find = true;
                        temp.FreqDn = freq_Dn;
                        temp.ChannelN = (int)((temp.FreqDn - 461.31m) / 0.02m) + 1039;
                        temp.FreqUp = freq_Dn - 10;
                        temp.StandartSubband = "Band-5 450";// Equipment.UMTSBands.Band_2_1900.ToString();
                    }
                }
                #endregion
            }
            if (find == false && freq_Dn >= 460.0m && freq_Dn <= 469.975m)
            {
                #region 5 450
                List<decimal> tf = new List<decimal>() { };
                for (int i = 1; i <= 400; i++)
                {
                    tf.Add(460.0m + 0.025m * (i - 1));
                }
                for (int i = 0; i < tf.Count; i++)
                {
                    if (tf[i] == freq_Dn)
                    {
                        find = true;
                        temp.FreqDn = freq_Dn;
                        temp.ChannelN = (int)((temp.FreqDn - 460.0m) / 0.025m) + 1;
                        temp.FreqUp = freq_Dn - 10;
                        temp.StandartSubband = "Band-5 450";// Equipment.UMTSBands.Band_2_1900.ToString();
                    }
                }
                #endregion
            }
            if (find == false && freq_Dn >= 489 && freq_Dn <= 493.475m)
            {
                #region 5 450
                List<decimal> tf = new List<decimal>() { };
                for (int i = 1536; i <= 1715; i++)
                {
                    tf.Add(489 + 0.025m * (i - 1536));
                }
                for (int i = 0; i < tf.Count; i++)
                {
                    if (tf[i] == freq_Dn)
                    {
                        find = true;
                        temp.FreqDn = freq_Dn;
                        temp.ChannelN = (int)((temp.FreqDn - 489) / 0.025m) + 1536;
                        temp.FreqUp = freq_Dn - 10;
                        temp.StandartSubband = "Band-5 450";// Equipment.UMTSBands.Band_2_1900.ToString();
                    }
                }
                #endregion
            }
            if (find == false && freq_Dn >= 489 && freq_Dn <= 493.480m)
            {
                #region 5 450
                List<decimal> tf = new List<decimal>() { };
                for (int i = 1792; i <= 2016; i++)
                {
                    tf.Add(489 + 0.02m * (i - 1792));
                }
                for (int i = 0; i < tf.Count; i++)
                {
                    if (tf[i] == freq_Dn)
                    {
                        find = true;
                        temp.FreqDn = freq_Dn;
                        temp.ChannelN = (int)((temp.FreqDn - 489) / 0.02m) + 1792;
                        temp.FreqUp = freq_Dn - 10;
                        temp.StandartSubband = "Band-5 450";// Equipment.UMTSBands.Band_2_1900.ToString();
                    }
                }
                #endregion
            }
            if (find == false && freq_Dn >= 1930 && freq_Dn <= 1994.95m)
            {
                #region 14 1900
                List<decimal> tf = new List<decimal>() { };
                for (int i = 0; i <= 1299; i++)
                {
                    tf.Add(1930 + 0.05m * (i - 0));
                }
                for (int i = 0; i < tf.Count; i++)
                {
                    if (tf[i] == freq_Dn)
                    {
                        find = true;
                        temp.FreqDn = freq_Dn;
                        temp.ChannelN = (int)((temp.FreqDn - 1930) / 0.05m) + 0;
                        temp.FreqUp = freq_Dn - 80;
                        temp.StandartSubband = "Band-14 1900";// Equipment.UMTSBands.Band_2_1900.ToString();
                    }
                }
                #endregion
            }
            if (find == false)
                throw new Exception("Частота " + freq_Dn + " МГц не соответствует стандартным сеткам частот!");
            temp.FreqDn = temp.FreqDn * 1000000;
            temp.FreqUp = temp.FreqUp * 1000000;
            temp.EVDOvsCDMA = EVDOvsCDMA;
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

        private MesureSysInfoDeviceProperties GetProperties(CFG.AdapterMainConfig config)
        {
            RadioPathParameters[] rrps = ConvertRadioPathParameters(config);
            StandardDeviceProperties sdp = null;
            if (DeviceType == DeviceType.Tsme || DeviceType == DeviceType.Tsme6)
            {
                sdp = new StandardDeviceProperties()
                {
                    AttMax_dB = 0,
                    AttMin_dB = 0,
                    FreqMax_Hz = 350000000,
                    FreqMin_Hz = 4400000000,
                    PreAmpMax_dB = 0, //типа включен/выключен, сколько по факту усиливает нигде не пишется кроме FSW где их два 15/30 и то два это опция
                    PreAmpMin_dB = 0,
                    RefLevelMax_dBm = 0,
                    RefLevelMin_dBm = 0,
                    EquipmentInfo = new EquipmentInfo()
                    {
                        AntennaCode = config.AdapterEquipmentInfo.AntennaSN,// "Omni",//S/N  В конфиг
                        AntennaManufacturer = config.AdapterEquipmentInfo.AntennaManufacturer,//"3anet",//В конфиг
                        AntennaName = config.AdapterEquipmentInfo.AntennaName,//"BC600",//В конфиг
                        EquipmentManufacturer = new Atdi.DataModels.Sdrn.DeviceServer.Adapters.InstrManufacrures().RuS.UI,
                        EquipmentName = DeviceType.ToString().ToUpper(),
                        EquipmentFamily = "R&S Network Analyzer TSMx",//SDR/SpecAn/MonRec
                        EquipmentCode = SerialNumber,//S/N
                    },
                    RadioPathParameters = rrps
                };
            }
            else if (DeviceType == DeviceType.Tsmw)
            {
                sdp = new StandardDeviceProperties()
                {
                    AttMax_dB = 0,
                    AttMin_dB = 0,
                    FreqMax_Hz = 20000000,
                    FreqMin_Hz = 6000000000,
                    PreAmpMax_dB = 0, //типа включен/выключен, сколько по факту усиливает нигде не пишется кроме FSW где их два 15/30 и то два это опция
                    PreAmpMin_dB = 0,
                    RefLevelMax_dBm = 0,
                    RefLevelMin_dBm = 0,
                    EquipmentInfo = new EquipmentInfo()
                    {
                        AntennaCode = config.AdapterEquipmentInfo.AntennaSN,// "Omni",//S/N  В конфиг
                        AntennaManufacturer = config.AdapterEquipmentInfo.AntennaManufacturer,//"3anet",//В конфиг
                        AntennaName = config.AdapterEquipmentInfo.AntennaName,//"BC600",//В конфиг
                        EquipmentManufacturer = new Atdi.DataModels.Sdrn.DeviceServer.Adapters.InstrManufacrures().RuS.UI,
                        EquipmentName = DeviceType.ToString().ToUpper(),
                        EquipmentFamily = "R&S Network Analyzer TSMx",//SDR/SpecAn/MonRec
                        EquipmentCode = SerialNumber,//S/N
                    },
                    RadioPathParameters = rrps
                };
            }
            List<string> tech = new List<string>() { };
            MesureSysInfoDeviceProperties msidp = new MesureSysInfoDeviceProperties()
            {
                StandardDeviceProperties = sdp,
            };
            if (Option_GSM == 1)
            {
                tech.Add("GSM");
            }
            if (Option_UMTS == 1)
            {
                tech.Add("UMTS");
            }
            if (Option_LTE == 1)
            {
                tech.Add("LTE");
            }
            if (Option_CDMA == 1)
            {
                tech.Add("CDMA");
            }
            if (Option_EVDO == 1)
            {
                tech.Add("EVDO");
            }
            msidp.AvailableStandards = tech.ToArray();
            MesureTraceDeviceProperties mtdp = new MesureTraceDeviceProperties()
            {
                //RBWMax_Hz = (double)UniqueData.RBWArr[UniqueData.RBWArr.Length - 1],
                //RBWMin_Hz = (double)UniqueData.RBWArr[0],
                //SweepTimeMin_s = (double)UniqueData.SWTMin,
                //SweepTimeMax_s = (double)UniqueData.SWTMax,
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


            return msidp;
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
