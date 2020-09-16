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
using MEN = Atdi.DataModels.Sdrn.DeviceServer.Adapters.Enums;
using PEN = Atdi.DataModels.Sdrn.DeviceServer.Commands.Parameters;
using System.IO.Ports;

namespace Atdi.AppUnits.Sdrn.DeviceServer.Adapters.LERFSwitch
{
    public class Adapter : IAdapter
    {
        private readonly ITimeService timeService;
        private readonly ILogger logger;
        private readonly AdapterConfig adapterConfig;

        public Adapter(AdapterConfig adapterConfig, ILogger logger, ITimeService timeService)
        {
            this.logger = logger;
            //this.adapterConfig = ValidateAndSetConfig(adapterConfig);
            this.timeService = timeService;
        }

        public void Connect(IAdapterHost host)
        {
            try
            {
                port = new SerialPort
                {
                    PortName = "COM" + adapterConfig.PortNumber.ToString(),
                    BaudRate = 600,
                    Parity = Parity.None,
                    DataBits = 8,
                    StopBits = StopBits.One,
                    Handshake = Handshake.None
                };
                port.Open();
                

                //RotatorDeviceProperties rdp = GetProperties(adapterConfig);
                //host.RegisterHandler<COM.SetRFSwitchSettingsCommand, bool>(SetRFSwitchSettingsCommandHandler, rdp);
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
            port.Close();
            port.Dispose();
        }


        //public void SetSwitchSettingsCommandHandler(COM.SetSwitchSettingsCommand command, IExecutionContext context)
        //{

        //}


        #region param
        private readonly string decimalSeparator = System.Globalization.NumberFormatInfo.CurrentInfo.CurrencyDecimalSeparator;
        private const float errordg = 100000;
        private const int maxTimeStep = 3600000; //один час
        private const int sleepAfterSet = 600;

        SerialPort port;

        

        //временные
        private bool publicResultAfterSet = false;

        private int bufferIndex = 0;
        byte[] buffer = new byte[50];
        bool received = false;

        private byte[] read = new byte[] { 0x57, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x1F, 0x20 };
        private byte[] stop = new byte[] { 0x57, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x0F, 0x20 };
        #endregion param










        //private RotatorDeviceProperties GetProperties(AdapterConfig config)
        //{
        //    //if (config.ElevationIsPolarization)
        //    //{
        //    //    return new RotatorDeviceProperties()
        //    //    {
        //    //        ControlDeviceManufacturer = config.ControlDeviceManufacturer,
        //    //        ControlDeviceName = config.ControlDeviceName,
        //    //        ControlDeviceCode = config.ControlDeviceCode,
        //    //        RotationDeviceManufacturer = config.RotationDeviceManufacturer,
        //    //        RotationDeviceName = config.RotationDeviceName,
        //    //        RotationDeviceCode = config.RotationDeviceCode,
        //    //        AzimuthMin_dg = config.AzimuthMin_dg,
        //    //        AzimuthMax_dg = config.AzimuthMax_dg,
        //    //        ElevationMin_dg = 0,//нету, всего две оси вращения 
        //    //        ElevationMax_dg = 0,//нету, всего две оси вращения 
        //    //        PolarizationMin_dg = config.ElevationMin_dg,//елевацию используем как поляризацию
        //    //        PolarizationMax_dg = config.ElevationMax_dg,//елевацию используем как поляризацию
        //    //        AzimuthStep_dg = stepAz,
        //    //        ElevationStep_dg = 0,
        //    //        PolarizationStep_dg = stepEl,
        //    //        AzimuthSpeedAvailable = new int[] { 1 },//доступна одна скорость
        //    //        ElevationSpeedAvailable = null,//нету, всего две оси вращения
        //    //        PolarizationSpeedAvailable = new int[] { 1 }//доступна одна скорость
        //    //    };
        //    //}
        //    //else
        //    //{
        //    //    return new RotatorDeviceProperties()
        //    //    {
        //    //        ControlDeviceManufacturer = config.ControlDeviceManufacturer,
        //    //        ControlDeviceName = config.ControlDeviceName,
        //    //        ControlDeviceCode = config.ControlDeviceCode,
        //    //        RotationDeviceManufacturer = config.RotationDeviceManufacturer,
        //    //        RotationDeviceName = config.RotationDeviceName,
        //    //        RotationDeviceCode = config.RotationDeviceCode,
        //    //        AzimuthMin_dg = config.AzimuthMin_dg,
        //    //        AzimuthMax_dg = config.AzimuthMax_dg,
        //    //        ElevationMin_dg = config.ElevationMin_dg,
        //    //        ElevationMax_dg = config.ElevationMax_dg,
        //    //        PolarizationMin_dg = 0, //нету, всего две оси вращения
        //    //        PolarizationMax_dg = 0, //нету, всего две оси вращения
        //    //        AzimuthStep_dg = stepAz,
        //    //        ElevationStep_dg = stepEl,
        //    //        PolarizationStep_dg = 0,
        //    //        AzimuthSpeedAvailable = new int[] { 1 },//доступна одна скорость
        //    //        ElevationSpeedAvailable = new int[] { 1 },//доступна одна скорость
        //    //        PolarizationSpeedAvailable = null//нету, всего две оси вращения
        //    //    };
        //    //}
        //}
    }
}
