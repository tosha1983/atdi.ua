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

using System.Diagnostics;

namespace Atdi.AppUnits.Sdrn.DeviceServer.Adapters.RSTSMx.Test
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

                MesureSysInfoDeviceProperties msidp = new MesureSysInfoDeviceProperties();
                host.RegisterHandler<COM.MesureSystemInfoCommand, COMR.MesureSystemInfoResult>(MesureSystemInfoHandler, msidp);

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
                    COMR.MesureSystemInfoResult msir = new COMR.MesureSystemInfoResult(0, CommandResultStatus.Next)
                    {
                        DeviceStatus = COMR.Enums.DeviceStatus.Normal,
                        SystemInfo = new COMRMSI.StationSystemInfo[1]
                        {
                            new COMRMSI.StationSystemInfo()
                            {
                                  BandWidth_Hz =12,
                                   BaseId =1,
                                    BSIC =2,
                                     ChannelNumber=45,
                                      CID =4,
                                       CodePower=435,
                                        CtoI = 5,
                                         ECI = 4,
                                          Power = 4,
                                           LAC = 4
                            }
                        }

                    };
                    context.PushResult(msir);
                    context.Unlock();
                    context.Finish();
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

        private const long TicksBefore1970 = 621355968000000000;

        private bool IsRuning = true; //GPS
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


        private bool Connect(string ipaddress)
        {
            return true;
        }
    }
}
