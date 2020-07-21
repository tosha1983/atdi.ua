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
using System.IO.Ports;

namespace Atdi.AppUnits.Sdrn.DeviceServer.Adapters.SPIDRotator
{
    public class Adapter : IAdapter
    {
        private readonly ITimeService timeService;
        private readonly ILogger logger;
        private readonly AdapterConfig adapterConfig;

        public Adapter(AdapterConfig adapterConfig, ILogger logger, ITimeService timeService)
        {
            this.logger = logger;
            this.adapterConfig = ValidateAndSetConfig(adapterConfig);
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
                (azt, elt) = RequestPosition();
                SetAzimuthAndElevation(azt, elt);
               
                RotatorDeviceProperties rdp = GetProperties(adapterConfig);
                host.RegisterHandler<COM.SetRotatorPositionCommand, COMR.RotatorPositionResult>(SetRotatorPositionCommandHandler, rdp);
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
        public void SetRotatorPositionCommandHandler(COM.SetRotatorPositionCommand command, IExecutionContext context)
        {
            try
            {
                context.Lock();
                ulong partIndex = 0;
                if (command.Parameter.Mode == COMP.RotatorPositionMode.SetAndGet)
                {
                    (float setaz, float setel, float setpl) = ValidateAndSetNewAngles(command.Parameter);
                    (float stepaz, float stepel, float steppl) = ValidateAndSetSteps(command.Parameter);
                    (int sleepaz, int sleepel, int sleeppl) = ValidateAndSetTimeSteps(command.Parameter);
                    int sleep = 0;

                    publicResultAfterSet = command.Parameter.PublicResultAfterSet;
                    if (adapterConfig.ElevationIsPolarization)//т.е. используем данные из поляризации и у нас всего две плоскости
                    {
                        sleep = Math.Max(sleepaz, sleeppl);
                        if (stepaz != 0)//хотим с шагом по азмуту
                        {
                            if (steppl != 0)//все с шагом)) так сибе подход но пусть будит (первая заповедь радиолюбителя, не крути две ручки одновременно)
                            {
                                //будим менять все с доступным шагом
                                #region res0
                                COM.Results.RotatorPositionResult result0 = new COM.Results.RotatorPositionResult(partIndex, CommandResultStatus.Next)
                                {
                                    Established = true,
                                    Azimuth_dg = Azimuth,
                                    Polarization_dg = Polarization,
                                    Elevation_dg = Elevation
                                };
                                context.PushResult(result0);
                                partIndex++;
                                #endregion res0

                                //Ждем
                                System.Threading.Thread.Sleep(sleep);
                                //Перестали

                                #region res1
                                COM.Results.RotatorPositionResult result1 = new COM.Results.RotatorPositionResult(partIndex, CommandResultStatus.Next)
                                {
                                    Established = false,
                                    Azimuth_dg = Azimuth,
                                    Polarization_dg = Polarization,
                                    Elevation_dg = Elevation
                                };
                                context.PushResult(result1);
                                partIndex++;
                                #endregion res1

                                for (int i = 0; i < 1; i++)
                                {
                                    if (context.Token.IsCancellationRequested)
                                    {
                                        break;
                                    }
                                    if (setaz < Azimuth)
                                    {
                                        azimuthNext = Azimuth - stepaz;
                                        if (azimuthNext < setaz)
                                        {
                                            azimuthNext = setaz;
                                        }
                                    }
                                    else if (setaz > Azimuth)
                                    {
                                        azimuthNext = Azimuth + stepaz;
                                        if (azimuthNext > setaz)
                                        {
                                            azimuthNext = setaz;
                                        }
                                    }
                                    else
                                    {
                                        azimuthNext = Azimuth;
                                    }

                                    if (setpl < Polarization)
                                    {
                                        polarizationNext = Polarization - steppl;
                                        if (polarizationNext < setpl)
                                        {
                                            polarizationNext = setpl;
                                        }
                                    }
                                    else if (setpl > Polarization)
                                    {
                                        polarizationNext = Polarization + steppl;
                                        if (polarizationNext > setpl)
                                        {
                                            polarizationNext = setpl;
                                        }
                                    }
                                    else
                                    {
                                        polarizationNext = Polarization;
                                    }

                                    if (setaz != Azimuth || setpl != Polarization)
                                    {
                                        Send(azimuthNext, polarizationNext);
                                        System.Threading.Thread.Sleep(sleepAfterSet);
                                        while (!(polarizationNext == Polarization && azimuthNext == Azimuth))
                                        {
                                            if (context.Token.IsCancellationRequested)
                                            {
                                                port.Write(stop, 0, stop.Length);
                                                break;
                                            }
                                            (azt, elt) = RequestPosition();
                                            SetAzimuthAndElevation(azt, elt);
                                        }
                                        //встали на новый шаг
                                        #region res2
                                        COM.Results.RotatorPositionResult result2 = null;
                                        if (setaz == Azimuth && setpl == Polarization)
                                        {
                                            result2 = new COM.Results.RotatorPositionResult(partIndex, CommandResultStatus.Final);
                                        }
                                        else
                                        {
                                            result2 = new COM.Results.RotatorPositionResult(partIndex, CommandResultStatus.Next);
                                        }
                                        result2.Established = true;
                                        result2.Azimuth_dg = Azimuth;
                                        result2.Polarization_dg = Polarization;
                                        result2.Elevation_dg = Elevation;
                                        context.PushResult(result2);
                                        partIndex++;
                                        #endregion res2
                                        //Ждем
                                        System.Threading.Thread.Sleep(sleep);
                                        //Перестали
                                        //поехали дальше
                                    }
                                    if ((setaz != Azimuth || setpl != Polarization) && !context.Token.IsCancellationRequested)
                                    {
                                        //поехали дальше
                                        #region res3
                                        COM.Results.RotatorPositionResult result3 = new COM.Results.RotatorPositionResult(partIndex, CommandResultStatus.Next);
                                        result3.Established = false;
                                        result3.Azimuth_dg = Azimuth;
                                        result3.Polarization_dg = Polarization;
                                        result3.Elevation_dg = Elevation;
                                        context.PushResult(result3);
                                        partIndex++;
                                        #endregion res3
                                        i--;//продолжаем движение
                                    }
                                }
                            }
                            else//установим сначала поляризацию
                            {
                                //т.к. наклонную ось используем как полиризацию
                                if (setpl != Polarization)
                                {
                                    Send(Azimuth, setpl);
                                    System.Threading.Thread.Sleep(sleepAfterSet);
                                    while (setpl != Polarization)
                                    {
                                        if (context.Token.IsCancellationRequested)
                                        {
                                            port.Write(stop, 0, stop.Length);
                                            break;
                                        }
                                        (azt, elt) = RequestPosition();
                                        SetAzimuthAndElevation(azt, elt);
                                    }
                                }
                                if (!context.Token.IsCancellationRequested)
                                {
                                    #region res0
                                    COM.Results.RotatorPositionResult result0 = new COM.Results.RotatorPositionResult(partIndex, CommandResultStatus.Next)
                                    {
                                        Established = true,
                                        Azimuth_dg = Azimuth,
                                        Polarization_dg = Polarization,
                                        Elevation_dg = Elevation
                                    };
                                    context.PushResult(result0);
                                    partIndex++;
                                    #endregion res0

                                    //Ждем
                                    System.Threading.Thread.Sleep(sleepaz);
                                    //Перестали

                                    #region res1
                                    COM.Results.RotatorPositionResult result1 = new COM.Results.RotatorPositionResult(partIndex, CommandResultStatus.Next)
                                    {
                                        Established = false,
                                        Azimuth_dg = Azimuth,
                                        Polarization_dg = Polarization,
                                        Elevation_dg = Elevation
                                    };
                                    context.PushResult(result1);
                                    partIndex++;
                                    #endregion res1
                                    if (setaz > Azimuth)//по часовой стрелке
                                    {
                                        while (setaz != Azimuth)
                                        {
                                            if (context.Token.IsCancellationRequested)
                                            {
                                                port.Write(stop, 0, stop.Length);
                                                break;
                                            }
                                            azimuthNext = Azimuth + stepaz;
                                            if (azimuthNext > setaz)
                                            {
                                                azimuthNext = setaz;
                                            }
                                            Send(azimuthNext, setpl);
                                            System.Threading.Thread.Sleep(sleepAfterSet);
                                            while (azimuthNext != Azimuth)
                                            {
                                                if (context.Token.IsCancellationRequested)
                                                {
                                                    port.Write(stop, 0, stop.Length);
                                                    break;
                                                }
                                                (azt, elt) = RequestPosition();
                                                SetAzimuthAndElevation(azt, elt);
                                            }
                                            if (!context.Token.IsCancellationRequested)
                                            {
                                                //встали на новый шаг
                                                #region res2
                                                COM.Results.RotatorPositionResult result2 = null;
                                                if (setaz != Azimuth)
                                                {
                                                    result2 = new COM.Results.RotatorPositionResult(partIndex, CommandResultStatus.Next);
                                                }
                                                else
                                                {
                                                    result2 = new COM.Results.RotatorPositionResult(partIndex, CommandResultStatus.Final);
                                                }
                                                result2.Established = true;
                                                result2.Azimuth_dg = Azimuth;
                                                result2.Polarization_dg = Polarization;
                                                result2.Elevation_dg = Elevation;
                                                context.PushResult(result2);
                                                partIndex++;
                                                #endregion res2
                                                //Ждем
                                                System.Threading.Thread.Sleep(sleepaz);
                                                //Перестали
                                                //поехали дальше
                                                if (setaz != Azimuth)
                                                {
                                                    #region res3
                                                    COM.Results.RotatorPositionResult result3 = new COM.Results.RotatorPositionResult(partIndex, CommandResultStatus.Next);
                                                    result3.Established = false;
                                                    result3.Azimuth_dg = Azimuth;
                                                    result3.Polarization_dg = Polarization;
                                                    result3.Elevation_dg = Elevation;
                                                    context.PushResult(result3);
                                                    partIndex++;
                                                    #endregion res3
                                                }
                                            }
                                        }
                                    }
                                    else if (setaz < Azimuth)//по часовой стрелке
                                    {
                                        while (setaz != Azimuth)
                                        {
                                            if (context.Token.IsCancellationRequested)
                                            {
                                                port.Write(stop, 0, stop.Length);
                                                break;
                                            }
                                            azimuthNext = Azimuth - stepaz;
                                            if (azimuthNext < setaz)
                                            {
                                                azimuthNext = setaz;
                                            }
                                            Send(azimuthNext, setpl);
                                            System.Threading.Thread.Sleep(sleepAfterSet);
                                            while (azimuthNext != Azimuth)
                                            {
                                                if (context.Token.IsCancellationRequested)
                                                {
                                                    port.Write(stop, 0, stop.Length);
                                                    break;
                                                }
                                                (azt, elt) = RequestPosition();
                                                SetAzimuthAndElevation(azt, elt);
                                            }
                                            if (!context.Token.IsCancellationRequested)
                                            {
                                                //встали на новый шаг
                                                #region res2
                                                COM.Results.RotatorPositionResult result2 = null;
                                                if (setaz != Azimuth)
                                                {
                                                    result2 = new COM.Results.RotatorPositionResult(partIndex, CommandResultStatus.Next);
                                                }
                                                else
                                                {
                                                    result2 = new COM.Results.RotatorPositionResult(partIndex, CommandResultStatus.Final);
                                                }

                                                result2.Established = true;
                                                result2.Azimuth_dg = Azimuth;
                                                result2.Polarization_dg = Polarization;
                                                result2.Elevation_dg = Elevation;
                                                context.PushResult(result2);
                                                partIndex++;
                                                #endregion res2

                                                //ждем
                                                System.Threading.Thread.Sleep(sleepaz);
                                                //Перестали
                                                //поехали дальше
                                                if (setaz != Azimuth)
                                                {
                                                    #region res3
                                                    COM.Results.RotatorPositionResult result3 = new COM.Results.RotatorPositionResult(partIndex, CommandResultStatus.Next);
                                                    result3.Established = false;
                                                    result3.Azimuth_dg = Azimuth;
                                                    result3.Polarization_dg = Polarization;
                                                    result3.Elevation_dg = Elevation;
                                                    context.PushResult(result3);
                                                    partIndex++;
                                                    #endregion res3
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        else//идем без шагов
                        {
                            //т.к. наклонную ось используем как полиризацию
                            Send(setaz, setpl);

                            System.Threading.Thread.Sleep(sleepAfterSet);
                            while (!(setaz == Azimuth && setpl == Polarization))
                            {
                                if (context.Token.IsCancellationRequested)
                                {
                                    port.Write(stop, 0, stop.Length);
                                    break;
                                }
                                (azt, elt) = RequestPosition();
                                SetAzimuthAndElevation(azt, elt);

                                if (!publicResultAfterSet)
                                {
                                    COM.Results.RotatorPositionResult result = null;
                                    if (setaz == Azimuth && setpl == Polarization)
                                    {
                                        result = new COM.Results.RotatorPositionResult(partIndex, CommandResultStatus.Final);
                                        result.Established = true;
                                    }
                                    else
                                    {
                                        result = new COM.Results.RotatorPositionResult(partIndex, CommandResultStatus.Next);
                                        result.Established = false;
                                    }
                                    result.Azimuth_dg = Azimuth;
                                    result.Polarization_dg = Polarization;
                                    result.Elevation_dg = Elevation;

                                    context.PushResult(result);
                                    partIndex++;
                                }
                            }
                            if (publicResultAfterSet && !context.Token.IsCancellationRequested)
                            {
                                COM.Results.RotatorPositionResult result = new COM.Results.RotatorPositionResult(0, CommandResultStatus.Final);
                                result.Established = true;
                                result.Azimuth_dg = Azimuth;
                                result.Polarization_dg = Polarization;
                                result.Elevation_dg = Elevation;

                                context.PushResult(result);
                            }
                        }
                    }
                    else
                    {
                        sleep = Math.Max(sleepaz, sleepel);
                        if (stepaz != 0)//хотим с шагом по азмуту
                        {
                            if (stepel != 0)//все с шагом)) так сибе подход но пусть будит (первая заповедь радиолюбителя, не крути две ручки одновременно)
                            {
                                //будим менять все с доступным шагом
                                #region res0
                                COM.Results.RotatorPositionResult result0 = new COM.Results.RotatorPositionResult(partIndex, CommandResultStatus.Next)
                                {
                                    Established = true,
                                    Azimuth_dg = Azimuth,
                                    Polarization_dg = Polarization,
                                    Elevation_dg = Elevation
                                };
                                context.PushResult(result0);
                                partIndex++;
                                #endregion res0

                                //Ждем
                                System.Threading.Thread.Sleep(sleep);
                                //Перестали

                                #region res1
                                COM.Results.RotatorPositionResult result1 = new COM.Results.RotatorPositionResult(partIndex, CommandResultStatus.Next)
                                {
                                    Established = false,
                                    Azimuth_dg = Azimuth,
                                    Polarization_dg = Polarization,
                                    Elevation_dg = Elevation
                                };
                                context.PushResult(result1);
                                partIndex++;
                                #endregion res1

                                for (int i = 0; i < 1; i++)
                                {
                                    if (context.Token.IsCancellationRequested)
                                    {
                                        break;
                                    }
                                    if (setaz < Azimuth)
                                    {
                                        azimuthNext = Azimuth - stepaz;
                                        if (azimuthNext < setaz)
                                        {
                                            azimuthNext = setaz;
                                        }
                                    }
                                    else if (setaz > Azimuth)
                                    {
                                        azimuthNext = Azimuth + stepaz;
                                        if (azimuthNext > setaz)
                                        {
                                            azimuthNext = setaz;
                                        }
                                    }
                                    else
                                    {
                                        azimuthNext = Azimuth;
                                    }

                                    if (setel < Elevation)
                                    {
                                        elevationNext = Elevation - stepel;
                                        if (elevationNext < setel)
                                        {
                                            elevationNext = setel;
                                        }
                                    }
                                    else if (setel > Elevation)
                                    {
                                        elevationNext = Elevation + stepel;
                                        if (elevationNext > setel)
                                        {
                                            elevationNext = setel;
                                        }
                                    }
                                    else
                                    {
                                        elevationNext = Elevation;
                                    }

                                    if (setaz != Azimuth || setel != Elevation)
                                    {
                                        Send(azimuthNext, elevationNext);
                                        System.Threading.Thread.Sleep(sleepAfterSet);
                                        while (!(elevationNext == Elevation && azimuthNext == Azimuth))
                                        {
                                            if (context.Token.IsCancellationRequested)
                                            {
                                                port.Write(stop, 0, stop.Length);
                                                break;
                                            }
                                            (azt, elt) = RequestPosition();
                                            SetAzimuthAndElevation(azt, elt);
                                        }
                                        //встали на новый шаг
                                        #region res2
                                        COM.Results.RotatorPositionResult result2 = null;
                                        if (setaz == Azimuth && setel == Elevation)
                                        {
                                            result2 = new COM.Results.RotatorPositionResult(partIndex, CommandResultStatus.Final);
                                        }
                                        else
                                        {
                                            result2 = new COM.Results.RotatorPositionResult(partIndex, CommandResultStatus.Next);
                                        }
                                        result2.Established = true;
                                        result2.Azimuth_dg = Azimuth;
                                        result2.Polarization_dg = Polarization;
                                        result2.Elevation_dg = Elevation;
                                        context.PushResult(result2);
                                        partIndex++;
                                        #endregion res2
                                        //Ждем
                                        System.Threading.Thread.Sleep(sleep);
                                        //Перестали
                                        //поехали дальше
                                    }
                                    if ((setaz != Azimuth || setel != Elevation) && !context.Token.IsCancellationRequested)
                                    {
                                        //поехали дальше
                                        #region res3
                                        COM.Results.RotatorPositionResult result3 = new COM.Results.RotatorPositionResult(partIndex, CommandResultStatus.Next);
                                        result3.Established = false;
                                        result3.Azimuth_dg = Azimuth;
                                        result3.Polarization_dg = Polarization;
                                        result3.Elevation_dg = Elevation;
                                        context.PushResult(result3);
                                        partIndex++;
                                        #endregion res3
                                        i--;//продолжаем движение
                                    }
                                }
                            }
                            else//установим сначала елевацию
                            {
                                //т.к. наклонную ось используем как полиризацию
                                if (setel != Elevation)
                                {
                                    Send(Azimuth, setel);
                                    System.Threading.Thread.Sleep(sleepAfterSet);
                                    while (setel != Elevation)
                                    {
                                        if (context.Token.IsCancellationRequested)
                                        {
                                            port.Write(stop, 0, stop.Length);
                                            break;
                                        }
                                        (azt, elt) = RequestPosition();
                                        SetAzimuthAndElevation(azt, elt);
                                    }
                                }
                                if (!context.Token.IsCancellationRequested)
                                {
                                    #region res0
                                    COM.Results.RotatorPositionResult result0 = new COM.Results.RotatorPositionResult(partIndex, CommandResultStatus.Next)
                                    {
                                        Established = true,
                                        Azimuth_dg = Azimuth,
                                        Polarization_dg = Polarization,
                                        Elevation_dg = Elevation
                                    };
                                    context.PushResult(result0);
                                    partIndex++;
                                    #endregion res0

                                    //Ждем
                                    System.Threading.Thread.Sleep(sleepaz);
                                    //Перестали

                                    #region res1
                                    COM.Results.RotatorPositionResult result1 = new COM.Results.RotatorPositionResult(partIndex, CommandResultStatus.Next)
                                    {
                                        Established = false,
                                        Azimuth_dg = Azimuth,
                                        Polarization_dg = Polarization,
                                        Elevation_dg = Elevation
                                    };
                                    context.PushResult(result1);
                                    partIndex++;
                                    #endregion res1
                                    if (setaz > Azimuth)//по часовой стрелке
                                    {
                                        while (setaz != Azimuth)
                                        {
                                            if (context.Token.IsCancellationRequested)
                                            {
                                                port.Write(stop, 0, stop.Length);
                                                break;
                                            }
                                            azimuthNext = Azimuth + stepaz;
                                            if (azimuthNext > setaz)
                                            {
                                                azimuthNext = setaz;
                                            }
                                            Send(azimuthNext, setel);
                                            System.Threading.Thread.Sleep(sleepAfterSet);
                                            while (azimuthNext != Azimuth)
                                            {
                                                if (context.Token.IsCancellationRequested)
                                                {
                                                    port.Write(stop, 0, stop.Length);
                                                    break;
                                                }
                                                (azt, elt) = RequestPosition();
                                                SetAzimuthAndElevation(azt, elt);
                                            }
                                            if (!context.Token.IsCancellationRequested)
                                            {
                                                //встали на новый шаг
                                                #region res2
                                                COM.Results.RotatorPositionResult result2 = null;
                                                if (setaz != Azimuth)
                                                {
                                                    result2 = new COM.Results.RotatorPositionResult(partIndex, CommandResultStatus.Next);
                                                }
                                                else
                                                {
                                                    result2 = new COM.Results.RotatorPositionResult(partIndex, CommandResultStatus.Final);
                                                }
                                                result2.Established = true;
                                                result2.Azimuth_dg = Azimuth;
                                                result2.Polarization_dg = Polarization;
                                                result2.Elevation_dg = Elevation;
                                                context.PushResult(result2);
                                                partIndex++;
                                                #endregion res2
                                                //Ждем
                                                System.Threading.Thread.Sleep(sleepaz);
                                                //Перестали
                                                //поехали дальше
                                                if (setaz != Azimuth)
                                                {
                                                    #region res3
                                                    COM.Results.RotatorPositionResult result3 = new COM.Results.RotatorPositionResult(partIndex, CommandResultStatus.Next);
                                                    result3.Established = false;
                                                    result3.Azimuth_dg = Azimuth;
                                                    result3.Polarization_dg = Polarization;
                                                    result3.Elevation_dg = Elevation;
                                                    context.PushResult(result3);
                                                    partIndex++;
                                                    #endregion res3
                                                }
                                            }
                                        }
                                    }
                                    else if (setaz < Azimuth)//против часовой стрелке
                                    {
                                        while (setaz != Azimuth)
                                        {
                                            if (context.Token.IsCancellationRequested)
                                            {
                                                port.Write(stop, 0, stop.Length);
                                                break;
                                            }
                                            azimuthNext = Azimuth - stepaz;
                                            if (azimuthNext < setaz)
                                            {
                                                azimuthNext = setaz;
                                            }
                                            Send(azimuthNext, setpl);
                                            System.Threading.Thread.Sleep(sleepAfterSet);
                                            while (azimuthNext != Azimuth)
                                            {
                                                if (context.Token.IsCancellationRequested)
                                                {
                                                    port.Write(stop, 0, stop.Length);
                                                    break;
                                                }
                                                (azt, elt) = RequestPosition();
                                                SetAzimuthAndElevation(azt, elt);
                                            }
                                            if (!context.Token.IsCancellationRequested)
                                            {
                                                //встали на новый шаг
                                                #region res2
                                                COM.Results.RotatorPositionResult result2 = null;
                                                if (setaz != Azimuth)
                                                {
                                                    result2 = new COM.Results.RotatorPositionResult(partIndex, CommandResultStatus.Next);
                                                }
                                                else
                                                {
                                                    result2 = new COM.Results.RotatorPositionResult(partIndex, CommandResultStatus.Final);
                                                }

                                                result2.Established = true;
                                                result2.Azimuth_dg = Azimuth;
                                                result2.Polarization_dg = Polarization;
                                                result2.Elevation_dg = Elevation;
                                                context.PushResult(result2);
                                                partIndex++;
                                                #endregion res2

                                                //ждем
                                                System.Threading.Thread.Sleep(sleepaz);
                                                //Перестали
                                                //поехали дальше
                                                if (setaz != Azimuth)
                                                {
                                                    #region res3
                                                    COM.Results.RotatorPositionResult result3 = new COM.Results.RotatorPositionResult(partIndex, CommandResultStatus.Next);
                                                    result3.Established = false;
                                                    result3.Azimuth_dg = Azimuth;
                                                    result3.Polarization_dg = Polarization;
                                                    result3.Elevation_dg = Elevation;
                                                    context.PushResult(result3);
                                                    partIndex++;
                                                    #endregion res3
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            //т.к. наклонную ось используем как полиризацию
                            Send(setaz, setel);

                            System.Threading.Thread.Sleep(sleepAfterSet);
                            while (!(setaz == Azimuth && setel == Elevation))
                            {
                                if (context.Token.IsCancellationRequested)
                                {
                                    port.Write(stop, 0, stop.Length);
                                    break;
                                }
                                (azt, elt) = RequestPosition();
                                SetAzimuthAndElevation(azt, elt);

                                if (!publicResultAfterSet)
                                {
                                    COM.Results.RotatorPositionResult result = null;
                                    if (setaz == Azimuth && setel == Polarization)
                                    {
                                        result = new COM.Results.RotatorPositionResult(partIndex, CommandResultStatus.Final);
                                        result.Established = true;
                                    }
                                    else
                                    {
                                        result = new COM.Results.RotatorPositionResult(partIndex, CommandResultStatus.Next);
                                        result.Established = false;
                                    }
                                    result.Azimuth_dg = Azimuth;
                                    result.Polarization_dg = Polarization;
                                    result.Elevation_dg = Elevation;

                                    context.PushResult(result);
                                    partIndex++;
                                }
                            }
                            if (publicResultAfterSet && !context.Token.IsCancellationRequested)
                            {
                                COM.Results.RotatorPositionResult result = new COM.Results.RotatorPositionResult(0, CommandResultStatus.Final);
                                result.Established = true;
                                result.Azimuth_dg = Azimuth;
                                result.Polarization_dg = Polarization;
                                result.Elevation_dg = Elevation;

                                context.PushResult(result);
                            }
                        }
                    }
                }
                else
                {
                    (azt, elt) = RequestPosition();
                    if (azt == errordg || elt == errordg)
                    {
                        throw new Exception("The response time from the device has exceeded the limit.");
                    }
                    else
                    {
                        SetAzimuthAndElevation(azt, elt);
                        COM.Results.RotatorPositionResult result = new COM.Results.RotatorPositionResult(0, CommandResultStatus.Final)
                        {
                            Established = true,
                            Azimuth_dg = Azimuth,
                            Polarization_dg = Polarization,
                            Elevation_dg = Elevation
                        };
                        context.PushResult(result);
                    }
                }
                if (context.Token.IsCancellationRequested)
                {
                    //кажется странным, но нет, так надо иначе со старта считываем значение из буфера, которое было записанно не по факту остановки, а до того как оно остановилось и поняло где оно
                    //мк програмисты ну очень спицефичны
                    //System.Threading.Thread.Sleep(1000); непоможет
                    (azt, elt) = RequestPosition();
                    (azt, elt) = RequestPosition();
                    SetAzimuthAndElevation(azt, elt);
                    COM.Results.RotatorPositionResult result = new COM.Results.RotatorPositionResult(partIndex, CommandResultStatus.Ragged)
                    {
                        Established = true,
                        Azimuth_dg = Azimuth,
                        Polarization_dg = Polarization,
                        Elevation_dg = Elevation
                    };
                    context.PushResult(result);
                    // подтверждаем факт обработки отмены
                    context.Cancel();
                }
                context.Unlock();
                // что то делаем еще 
                // подтверждаем окончание выполнения комманды 
                // важно: всн ранее устапнволеные в контексте обработки текущей команыд блокировки снимаются автоматически
                context.Finish();
                // дальше кода быть не должно, освобождаем поток
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

        #region param
        private readonly string decimalSeparator = System.Globalization.NumberFormatInfo.CurrentInfo.CurrencyDecimalSeparator;
        private const float errordg = 100000;
        private const int maxTimeStep = 3600000; //один час
        private const int sleepAfterSet = 600;

        SerialPort port;

        public float Azimuth = 0f;
        public float Elevation = 0f;
        public float Polarization = 0f;

        private float azimuthPrev = 0f;
        private float elevationPrev = 0f;
        private float polarizationPrev = 0f;

        private float azimuthNext = 0f;
        private float elevationNext = 0f;
        private float polarizationNext = 0f;

        //временные
        private float azt = 0f;
        private float elt = 0f;
        private float plt = 0f;

        private bool publicResultAfterSet = false;

        Gear gearAz = Gear.Gear100;
        Gear gearEl = Gear.Gear100;
        Gear gearPl = Gear.Gear100;
        private float stepAz = 1;
        private float stepEl = 1;
        private float stepPl = 1;

        private int bufferIndex = 0;
        byte[] buffer = new byte[50];
        bool received = false;

        private byte[] read = new byte[] { 0x57, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x1F, 0x20 };
        private byte[] stop = new byte[] { 0x57, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x0F, 0x20 };
        #endregion param

        #region ValidateAndSet
        private AdapterConfig ValidateAndSetConfig(AdapterConfig config)
        {
            if (config.PortNumber < 1)
            {
                throw new Exception("Com port number must be greater than zero.");
            }
            if (config.AzimuthMin_dg < 0 || config.AzimuthMin_dg > 360)
            {
                throw new Exception("AzimuthMin_dg must be installed from 0 to 360 inclusive.");
            }
            if (config.AzimuthMax_dg < 0 || config.AzimuthMax_dg > 360)
            {
                throw new Exception("AzimuthMax_dg must be installed from 0 to 360 inclusive.");
            }
            if (config.AzimuthMin_dg > config.AzimuthMax_dg)
            {
                throw new Exception("AzimuthMin_dg must be smaller than AzimuthMax_dg.");
            }            
            if (config.ElevationMin_dg < -180 || config.ElevationMin_dg > 180)
            {
                throw new Exception("ElevationMin_dg must be installed from -180 to 180 inclusive.");
            }
            if (config.ElevationMax_dg < -180 || config.ElevationMax_dg > 180)
            {
                throw new Exception("ElevationMax_dg must be installed from -180 to 180 inclusive.");
            }
            if (config.ElevationMin_dg > config.ElevationMax_dg)
            {
                throw new Exception("ElevationMin_dg must be smaller than ElevationMax_dg.");
            }

            if (config.ControlDeviceManufacturer == null || config.ControlDeviceManufacturer == string.Empty)
            {
                throw new Exception("AdapterConfig.ControlDeviceManufacturer must be populated.");
            }
            if (config.ControlDeviceName == null && config.ControlDeviceName == string.Empty)
            {
                throw new Exception("AdapterConfig.ControlDeviceName must be populated.");
            }
            if (config.ControlDeviceCode == null && config.ControlDeviceCode == string.Empty)
            {
                throw new Exception("AdapterConfig.ControlDeviceCode must be populated.");
            }
            if (config.RotationDeviceManufacturer == null && config.RotationDeviceManufacturer == string.Empty)
            {
                throw new Exception("AdapterConfig.RotationDeviceManufacturer must be populated.");
            }
            if (config.RotationDeviceName == null && config.RotationDeviceName == string.Empty)
            {
                throw new Exception("AdapterConfig.RotationDeviceName must be populated.");
            }
            if (config.RotationDeviceCode == null && config.RotationDeviceCode == string.Empty)
            {
                throw new Exception("AdapterConfig.RotationDeviceCode must be populated.");
            }

            return config;
        }
        private (float az, float el, float pl) ValidateAndSetNewAngles(COMP.RotatorPositionParameter param)
        {
            float az = 0, el = 0, pl = 0;
            if (param.Azimuth_dg >= adapterConfig.AzimuthMin_dg && param.Azimuth_dg <= adapterConfig.AzimuthMax_dg)
            {
                az = param.Azimuth_dg;
            }
            else
            {
                throw new Exception("RotatorPositionParameter.Azimuth_dg must be set between AdapterConfig.AzimuthMin_dg and AzimuthMax_dg.");
            }

            //всего две оси
            if (adapterConfig.ElevationIsPolarization)//используем поляризацию но проверяем по ограничениям элевации
            {
                if (param.Polarization_dg >= adapterConfig.ElevationMin_dg && param.Polarization_dg <= adapterConfig.ElevationMax_dg)
                {
                    pl = param.Polarization_dg;
                }
                else
                {
                    throw new Exception("RotatorPositionParameter.Polarization_dg must be set between AdapterConfig.ElevationMin_dg and AdapterConfig.ElevationMax_dg.");
                }
            }
            else//используем элевацию
            {
                if (param.Elevation_dg >= adapterConfig.ElevationMin_dg && param.Elevation_dg <= adapterConfig.ElevationMax_dg)
                {
                    el = param.Elevation_dg;
                }
                else
                {
                    throw new Exception("RotatorPositionParameter.Elevation_dg must be set between AdapterConfig.ElevationMin_dg and AdapterConfig.ElevationMax_dg.");
                }
            }
            return (az, el, pl);
        }

        private (float az, float el, float pl) ValidateAndSetSteps(COMP.RotatorPositionParameter param)
        {
            float az = 0, el = 0, pl = 0;
            float azd = adapterConfig.AzimuthMax_dg - adapterConfig.AzimuthMin_dg, eld = adapterConfig.ElevationMax_dg - adapterConfig.ElevationMin_dg;
            if (param.AzimuthStep_dg >= 0 && param.AzimuthStep_dg <= azd)
            {
                az = (float)(stepAz * Math.Floor(param.AzimuthStep_dg / stepAz));
            }
            else
            {
                throw new Exception("RotatorPositionParameter.AzimuthStep_dg cannot be negative and must be less than or equal to AdapterConfig.AzimuthMax_dg - AdapterConfig.AzimuthMin_dg.");
            }
            //всего две оси
            if (adapterConfig.ElevationIsPolarization)//используем поляризацию но проверяем по ограничениям элевации
            {
                if (param.PolarizationStep_dg >= 0 && param.PolarizationStep_dg <= eld)
                {
                    pl = (float)(stepEl * Math.Floor(param.PolarizationStep_dg / stepEl));
                }
                else
                {
                    throw new Exception("RotatorPositionParameter.PolarizationStep_dg cannot be negative and must be less than or equal to AdapterConfig.ElevationMax_dg - AdapterConfig.ElevationMin_dg.");
                }
            }
            else//используем элевацию
            {
                if (param.ElevationStep_dg >= 0 && param.ElevationStep_dg <= eld)
                {
                    el = (float)(stepEl * Math.Floor(param.ElevationStep_dg / stepEl));
                }
                else
                {
                    throw new Exception("RotatorPositionParameter.ElevationStep_dg cannot be negative and must be less than or equal to AdapterConfig.ElevationMax_dg - AdapterConfig.ElevationMin_dg.");
                }
            }
            return (az, el, pl);
        }

        private (int az, int el, int pl) ValidateAndSetTimeSteps(COMP.RotatorPositionParameter param)
        {
            int az = 0, el = 0, pl = 0;
            if (param.AzimuthTimeStep_ms > -1 && param.AzimuthTimeStep_ms <= maxTimeStep)
            {
                az = param.AzimuthTimeStep_ms;
            }
            else
            {
                throw new Exception($"Movement into the past is not available, and cannot exceed 1 hour (RotatorPositionParameter.AzimuthTimeStep_ms = {param.AzimuthTimeStep_ms.ToString()}).");
            }
            //всего две оси
            if (adapterConfig.ElevationIsPolarization)//используем поляризацию но проверяем по ограничениям элевации
            {
                if (param.PolarizationTimeStep_ms >= -1 && param.PolarizationTimeStep_ms <= maxTimeStep)
                {
                    pl = param.PolarizationTimeStep_ms;
                }
                else
                {
                    throw new Exception($"Movement into the past is not available, and cannot exceed 1 hour (RotatorPositionParameter.PolarizationTimeStep_ms = {param.PolarizationTimeStep_ms.ToString()}).");
                }
            }
            else//используем элевацию
            {
                if (param.ElevationTimeStep_ms >= -1 && param.ElevationTimeStep_ms <= maxTimeStep)
                {
                    el = param.ElevationTimeStep_ms;
                }
                else
                {
                    throw new Exception($"Movement into the past is not available, and cannot exceed 1 hour (RotatorPositionParameter.ElevationTimeStep_ms = {param.ElevationTimeStep_ms.ToString()}).");
                }
            }
            return (az, el, pl);
        }
        #endregion ValidateAndSet


        private void Send(float az, float el)
        {
            byte[] data = new byte[] { 0x57, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x2F, 0x20 };
            byte[] baz = new byte[] { };
            byte[] bel = new byte[] { };
            if (gearAz == Gear.Gear100)
            {
                baz = ASCIIEncoding.ASCII.GetBytes(string.Format("{0:0000}", az + 360));
                data[5] = 0x01;
            }
            else if (gearAz == Gear.Gear050)
            {
                baz = ASCIIEncoding.ASCII.GetBytes(string.Format("{0:0000}", (az + 360) * 2));
                data[5] = 0x02;
            }
            Array.Copy(baz, 0, data, 1, baz.Length);

            if (gearEl == Gear.Gear100)
            {
                bel = ASCIIEncoding.ASCII.GetBytes(string.Format("{0:0000}", el + 360));
                data[10] = 0x01;
            }
            else if (gearEl == Gear.Gear050)
            {
                bel = ASCIIEncoding.ASCII.GetBytes(string.Format("{0:0000}", (el + 360) * 2));
                data[10] = 0x02;
            }
            Array.Copy(bel, 0, data, 6, bel.Length);
            port.Write(data, 0, data.Length);
        }

        private (float az, float el) RequestPosition()
        {
            port.Write(read, 0, read.Length);
            System.Threading.Thread.Sleep(300);
            bool end = false;
            byte[] lb = new byte[1];
            byte[] buff = new byte[15];
            int buffindex = 0;
            long time = timeService.TimeStamp.Ticks + 10000000;
            bool timeout = false;
            while (!end && !timeout)
            {
                timeout = timeService.TimeStamp.Ticks > time;
                if (!timeout)
                {
                    port.Read(lb, 0, 1);
                    if (lb[0] == 87)//W
                    {
                        buffindex = 0;
                        buff[buffindex] = lb[0];
                    }
                    else if (lb[0] == 32)//end
                    {
                        buffindex++;
                        buff[buffindex] = lb[0];
                        end = true;
                    }
                    else
                    {
                        buffindex++;
                        buff[buffindex] = lb[0];
                    }
                    System.Threading.Thread.Sleep(1);//эта железяка медленная нечиго грузить проц
                }
            }
            if (timeout)
            {
                throw new Exception("The response time from the device has exceeded the limit.");                
            }
            return ReadData(buff);
        }

        private void SetAzimuthAndElevation(float az, float el)
        {
            azimuthPrev = Azimuth;
            Azimuth = az;

            //т.к. для этого управляющего устройства доступно только две оси
            if (adapterConfig.ElevationIsPolarization)
            {
                polarizationPrev = Polarization;
                Polarization = el;
                elevationPrev = Elevation;
                Elevation = 0;
            }
            else
            {
                polarizationPrev = Polarization;
                Polarization = 0;
                elevationPrev = Elevation;
                Elevation = el;
            }
        }

        private (float az, float el) ReadData(byte[] data)
        {
            float az = 0, el = 0;
            if (data[5] == 1)
            {
                az = Convert.ToSingle(data[1].ToString() + data[2].ToString() + data[3].ToString()) - 360;
                if (gearAz != Gear.Gear100)
                {
                    gearAz = Gear.Gear100;
                    stepAz = 1;
                }
            }
            else if (data[5] == 2)
            {
                az = Convert.ToSingle(data[1].ToString() + data[2].ToString() + data[3].ToString() + decimalSeparator + data[4].ToString()) / 2 - 360;
                if (gearAz != Gear.Gear050)
                {
                    gearAz = Gear.Gear050;
                    stepAz = 0.5f;
                }
            }
            if (data[10] == 1)
            {
                el = Convert.ToSingle(data[6].ToString() + data[7].ToString() + data[8].ToString()) - 360;
                if (gearEl != Gear.Gear100)
                {
                    gearEl = Gear.Gear100;
                    stepEl = 1;
                    stepPl = 1;
                }
            }
            else if (data[10] == 2)
            {
                el = Convert.ToSingle(data[6].ToString() + data[7].ToString() + data[8].ToString() + decimalSeparator + data[9].ToString()) / 2 - 360;
                if (gearEl != Gear.Gear050)
                {
                    gearEl = Gear.Gear050;
                    stepEl = 0.5f;
                    stepPl = 0.5f;
                }
            }

            //System.Diagnostics.Debug.WriteLine(Azimuth + "   " + Elevation);
            //received = true;
            return (az, el);
        }



        private RotatorDeviceProperties GetProperties(AdapterConfig config)
        {
            if (config.ElevationIsPolarization)
            {
                return new RotatorDeviceProperties()
                {
                    ControlDeviceManufacturer = config.ControlDeviceManufacturer,
                    ControlDeviceName = config.ControlDeviceName,
                    ControlDeviceCode = config.ControlDeviceCode,
                    RotationDeviceManufacturer = config.RotationDeviceManufacturer,
                    RotationDeviceName = config.RotationDeviceName,
                    RotationDeviceCode = config.RotationDeviceCode,
                    AzimuthMin_dg = config.AzimuthMin_dg,
                    AzimuthMax_dg = config.AzimuthMax_dg,
                    ElevationMin_dg = 0,//нету, всего две оси вращения 
                    ElevationMax_dg = 0,//нету, всего две оси вращения 
                    PolarizationMin_dg = config.ElevationMin_dg,//елевацию используем как поляризацию
                    PolarizationMax_dg = config.ElevationMax_dg,//елевацию используем как поляризацию
                    AzimuthStep_dg = stepAz,
                    ElevationStep_dg = 0,
                    PolarizationStep_dg = stepEl,
                    AzimuthSpeedAvailable = new int[] { 1 },//доступна одна скорость
                    ElevationSpeedAvailable = null,//нету, всего две оси вращения
                    PolarizationSpeedAvailable = new int[] { 1 }//доступна одна скорость
                };
            }
            else
            {
                return new RotatorDeviceProperties()
                {
                    ControlDeviceManufacturer = config.ControlDeviceManufacturer,
                    ControlDeviceName = config.ControlDeviceName,
                    ControlDeviceCode = config.ControlDeviceCode,
                    RotationDeviceManufacturer = config.RotationDeviceManufacturer,
                    RotationDeviceName = config.RotationDeviceName,
                    RotationDeviceCode = config.RotationDeviceCode,
                    AzimuthMin_dg = config.AzimuthMin_dg,
                    AzimuthMax_dg = config.AzimuthMax_dg,
                    ElevationMin_dg = config.ElevationMin_dg,
                    ElevationMax_dg = config.ElevationMax_dg,
                    PolarizationMin_dg = 0, //нету, всего две оси вращения
                    PolarizationMax_dg = 0, //нету, всего две оси вращения
                    AzimuthStep_dg = stepAz,
                    ElevationStep_dg = stepEl,
                    PolarizationStep_dg = 0,
                    AzimuthSpeedAvailable = new int[] { 1 },//доступна одна скорость
                    ElevationSpeedAvailable = new int[] { 1 },//доступна одна скорость
                    PolarizationSpeedAvailable = null//нету, всего две оси вращения
                };
            }
        }

        private enum Gear : int
        {
            Gear100 = 0,
            Gear050 = 1,
            Gear025 = 2,
        }
    }
}
