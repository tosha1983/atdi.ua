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

//using Ivi.Visa;
using RohdeSchwarz.Visa;
using Atdi.DataModels.Sdrn.DeviceServer.Adapters;
using EN = Atdi.AppUnits.Sdrn.DeviceServer.Adapters.RSFPL.Enums;
using MEN = Atdi.DataModels.Sdrn.DeviceServer.Adapters.Enums;
using Atdi.DataModels.Sdrn.DeviceServer;

namespace Atdi.AppUnits.Sdrn.DeviceServer.Adapters.RSFPL
{
    public class Adapter
    {
        private readonly ITimeService timeService;
        private readonly ILogger logger;
        private readonly AdapterConfig adapterConfig;

        private LocalParametersConverter lpc;
        private CFG.ThisAdapterConfig tac;
        private CFG.AdapterMainConfig mainConfig;

        public Adapter(AdapterConfig adapterConfig, ILogger logger, ITimeService timeService)
        {
            this.logger = logger;
            this.adapterConfig = adapterConfig;
            this.timeService = timeService;
            lpc = new LocalParametersConverter();

            LevelArr = new float[SweepPoints];
            //levelArrTemp = new float[SweepPoints];
            for (int i = 0; i < SweepPoints; i++)
            {
                LevelArr[i] = -1000;
                //levelArrTemp[i] = -1000;
            }
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
                    string fileName = "RSFPL_" + serialNumber + ".xml";
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
                    //SetTransducer(mainConfig);
                    (MesureTraceDeviceProperties mtdp, MesureIQStreamDeviceProperties miqdp) = GetProperties(mainConfig);

                    IResultPoolDescriptor<COMR.MesureTraceResult>[] rpd = ValidateAdapterTracePoolMainConfig(mainConfig.AdapterTraceResultPools, fileName);

                    host.RegisterHandler<COM.MesureTraceCommand, COMR.MesureTraceResult>(MesureTraceCommandHandler, rpd, mtdp);

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
                session.Dispose();
                rm.Dispose();
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
                /// если нужно проводим блокировку комманд который мы не сможем обслужить пока что то меряем в течении выполнени яэтой комманды
                /// и подсказывая этим инфраструктуре что этот устрйостов некоторое время не сможет обрабатываить такие комманды
                //context.Lock(CommandType.MesureTrace);

                // если нужно заблокировать выполняему комманду то достатчоно вызвать метод без параметров и блокируется комманда которая выполняется
                long t1 = timeService.TimeStamp.Ticks;
                context.Lock();
                //Переключимся на Spectrum
                if (!mode)
                {
                    SetWindowType(true);
                }
                bool frequencyChanged = false;

                decimal needRBW = 0, needVBW = 0;
                int needSweepPoints = 0;
                //защищаемся как можем
                if (command.Parameter.TracePoint == 0 || command.Parameter.RBW_Hz == 0 || command.Parameter.VBW_Hz == 0)
                {
                    //алахадбар
                }
                else
                {
                    //кол-во точек есть, посчитать по RBW
                    if (command.Parameter.TracePoint == -1 && command.Parameter.RBW_Hz > 0)
                    {
                        needRBW = (decimal)command.Parameter.RBW_Hz;
                        if (command.Parameter.VBW_Hz < 0) //Если авто то VBW = RBW
                        {
                            needVBW = needRBW;
                        }
                        else
                        {
                            needVBW = (decimal)command.Parameter.VBW_Hz;
                        }
                        needSweepPoints = (int)((command.Parameter.FreqStop_Hz - command.Parameter.FreqStart_Hz) / needRBW);
                    }
                    //сколько точек понятно почему и посчитать RBW
                    else if (command.Parameter.TracePoint > 0 && command.Parameter.RBW_Hz == -1)
                    {
                        needSweepPoints = command.Parameter.TracePoint;
                        needRBW = (command.Parameter.FreqStop_Hz - command.Parameter.FreqStart_Hz) / (needSweepPoints - 1);
                        if (command.Parameter.VBW_Hz < 0) //Если авто то VBW = RBW
                        {
                            needVBW = needRBW;
                        }
                        else
                        {
                            needVBW = (decimal)command.Parameter.VBW_Hz;
                        }
                    }
                    //сколько точек понятно почему и посчитать RBW
                    else if (command.Parameter.TracePoint > 0 && command.Parameter.RBW_Hz > 0)
                    {
                        needSweepPoints = command.Parameter.TracePoint;
                        needRBW = (decimal)command.Parameter.RBW_Hz;
                        if (command.Parameter.VBW_Hz < 0) //Если авто то VBW = RBW
                        {
                            needVBW = needRBW;
                        }
                        else
                        {
                            needVBW = (decimal)command.Parameter.VBW_Hz;
                        }
                    }
                }
                int stepsSweepPoints = 0, needActualSweepPoints = 0;
                if (needSweepPoints <= sweepPointsMax)//все норм 
                {
                    iWantMoreSweepPoints = false;//спихнем все на прибор
                    ValidateAndSetFreqStartStop(command.Parameter.FreqStart_Hz, command.Parameter.FreqStop_Hz, ref frequencyChanged);
                    ValidateAndSetSweepPoints(needSweepPoints);
                    ValidateAndSetVBW(needVBW);
                    ValidateAndSetRBW(needRBW);
                }
                else//странно но так надо
                {
                    iWantMoreSweepPoints = true;//мучаемся сами
                    //разобьем на столько шагов:
                    stepsSweepPoints = (int)Math.Ceiling((double)needSweepPoints / (double)sweepPointsMax);
                    //найдем ближайшее большее количевство точек на которые можно разбить

                    int delta = int.MaxValue;
                    int index = 0;
                    for (int i = 0; i < sweepPointArr.Length; i++)
                    {
                        if (Math.Abs(needSweepPoints - sweepPointArr[i] * stepsSweepPoints) < delta)
                        {
                            delta = Math.Abs(needSweepPoints - sweepPointArr[i] * stepsSweepPoints);
                            index = i;
                        }
                    }

                    if (index < sweepPointArr.Length - 2 && sweepPointArr[index] * stepsSweepPoints < needSweepPoints)
                    {
                        needActualSweepPoints = sweepPointArr[index + 1];
                    }
                    else
                    {
                        needActualSweepPoints = sweepPointArr[index];
                    }
                    //знаем все, как распределить и т.д., осталось запустить и собрать
                }

                #region old 
                //ValidateAndSetFreqStartStop(command.Parameter.FreqStart_Hz, command.Parameter.FreqStop_Hz, ref frequencyChanged);
                ////кол-во точек посчитать по RBW
                //if (command.Parameter.TracePoint == -1 && command.Parameter.RBW_Hz > 0)
                //{
                //    //RBW ближайшее меньшее
                //    ValidateAndSetRBW((decimal)command.Parameter.RBW_Hz);

                //    if (command.Parameter.VBW_Hz < 0) //Если авто то VBW = RBW
                //    {
                //        ValidateAndSetVBW(RBW);
                //    }
                //    else
                //    {
                //        ValidateAndSetVBW((decimal)command.Parameter.VBW_Hz);
                //    }

                //    int needsweeppints = (int)(FreqSpan / RBW);
                //    ValidateAndSetSweepPoints(needsweeppints);
                //}
                ////сколько точек понятно по нему и посчитать RBW
                //else if (command.Parameter.TracePoint > 0 && command.Parameter.RBW_Hz == -1)
                //{
                //    ValidateAndSetSweepPoints(command.Parameter.TracePoint);

                //    ValidateAndSetRBW(FreqSpan / (SweepPoints - 1));

                //    if (command.Parameter.VBW_Hz < 0) //Если авто то VBW = RBW
                //    {
                //        ValidateAndSetVBW(RBW);
                //    }
                //    else
                //    {
                //        ValidateAndSetVBW((decimal)command.Parameter.VBW_Hz);
                //    }
                //}
                ////сколько точек понятно по нему и посчитать RBW
                //else if (command.Parameter.TracePoint > 0 && command.Parameter.RBW_Hz > 0)
                //{
                //    ValidateAndSetSweepPoints(command.Parameter.TracePoint);

                //    ValidateAndSetRBW((decimal)command.Parameter.RBW_Hz);

                //    if (command.Parameter.VBW_Hz < 0) //Если авто то VBW = RBW
                //    {
                //        ValidateAndSetVBW(RBW);
                //    }
                //    else
                //    {
                //        ValidateAndSetVBW((decimal)command.Parameter.VBW_Hz);
                //    }
                //}
                #endregion Old
                ValidateAndSetRefLevel(command.Parameter.RefLevel_dBm);

                ValidateAndSetPreAmp(command.Parameter.PreAmp_dB);

                ValidateAndSetATT(command.Parameter.Att_dB);

                ValidateAndSetDetectorType(command.Parameter.DetectorType);




                ValidateAndSetSweepTime((decimal)command.Parameter.SweepTime_s);

                if (iWantMoreSweepPoints)
                {
                    ValidateAndSetTraceTypeWithAggregation(command.Parameter.TraceType);
                    ValidateAndSetLevelUnitWithAggregation(command.Parameter.LevelUnit, traceTypeResult);
                    ValidateAndSetVBW(needVBW);
                    ValidateAndSetRBW(needRBW);
                    (decimal freq1, decimal freq2) = ValidateFreqStartStop(command.Parameter.FreqStart_Hz, command.Parameter.FreqStop_Hz);
                    GetAndPushTraceResultsWithAggregation(command, context, freq1, freq2, stepsSweepPoints, needActualSweepPoints);



                }
                else
                {

                    //все спехнем на прибор
                    ValidateAndSetLevelUnit(command.Parameter.LevelUnit);
                    ValidateAndSetTraceType(command.Parameter.TraceType, frequencyChanged);
                    GetFreqArr();//узнаем что там с сеткой частот

                    //Устанавливаем сколько трейсов хотим
                    if (command.Parameter.TraceCount > 0)
                    {
                        traceCountToMeas = (ulong)command.Parameter.TraceCount;
                        traceCount = 0;
                        if (command.Parameter.TraceType == COMP.TraceType.ClearWhrite)
                        {
                            WriteString(":SENSe:AVERage:COUNt 1");
                        }
                        else
                        {
                            WriteString(":SENSe:AVERage:COUNt " + traceCountToMeas.ToString());
                        }
                    }
                    else
                    {
                        throw new Exception("TraceCount must be set greater than zero.");
                    }

                    GetAndPushTraceResults(command, context);
                }








                #region 
                //Меряем
                ////Если TraceType ClearWrite то пушаем каждый результат                    
                //bool newres = false;

                //if (trace1Type.Id == (int)EN.TraceType.ClearWrite)
                //{
                //    for (ulong i = 0; i < traceCountToMeas; i++)
                //    {
                //        newres = GetTrace();
                //        if (newres)
                //        {
                //            if (i > 0)// +1 маленький костыль для предотвращения кривого трейса
                //            {
                //                // пушаем результат
                //                var result = new COMR.MesureTraceResult(traceCount, CommandResultStatus.Next);
                //                traceCount++;
                //                if (traceCountToMeas == traceCount)
                //                {
                //                    result = new COMR.MesureTraceResult(traceCount, CommandResultStatus.Final);
                //                }
                //                result.LevelMaxIndex = LevelArr.Length;
                //                result.FrequencyStart_Hz = resFreqStart;
                //                result.FrequencyStep_Hz = resFreqStep;

                //                result.Att_dB = (int)AttLevelSpec;
                //                result.RefLevel_dBm = (int)RefLevelSpec;
                //                result.PreAmp_dB = preAmpSpec ? 1 : 0;
                //                result.RBW_Hz = (double)RBW;
                //                result.VBW_Hz = (double)VBW;
                //                result.Level = new float[FreqArr.Length];
                //                for (int j = 0; j < FreqArr.Length; j++)
                //                {
                //                    result.Level[j] = LevelArr[j];
                //                }
                //                result.TimeStamp = timeService.GetGnssUtcTime().Ticks - uTCOffset;// new DateTime(1970, 1, 1, 0, 0, 0, System.DateTimeKind.Utc).Ticks;
                //                                                                                  //result.TimeStamp = DateTime.UtcNow.Ticks - new DateTime(1970, 1, 1, 0, 0, 0, System.DateTimeKind.Utc).Ticks;
                //                if (PowerRegister != EN.PowerRegister.Normal)
                //                {
                //                    result.DeviceStatus = COMR.Enums.DeviceStatus.RFOverload;
                //                }
                //                context.PushResult(result);
                //            }
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
                //            var result2 = new COMR.MesureTraceResult(traceCount, CommandResultStatus.Ragged);

                //            context.PushResult(result2);


                //            // подтверждаем факт обработки отмены
                //            context.Cancel();
                //            // освобождаем поток 
                //            return;
                //        }
                //    }
                //}
                //else
                //{
                //    bool _RFOverload = false;
                //    for (ulong i = 0; i < traceCountToMeas; i++)
                //    {
                //        if (GetNumberOfSweeps())
                //        {
                //            newres = GetTrace();
                //            if (newres)
                //            {
                //                // пушаем результат
                //                var result = new COMR.MesureTraceResult(0, CommandResultStatus.Final);

                //                result.LevelMaxIndex = LevelArr.Length;
                //                result.FrequencyStart_Hz = resFreqStart;
                //                result.FrequencyStep_Hz = resFreqStep;

                //                result.Att_dB = (int)AttLevelSpec;
                //                result.RefLevel_dBm = (int)RefLevelSpec;
                //                result.PreAmp_dB = preAmpSpec ? 1 : 0;
                //                result.RBW_Hz = (double)RBW;
                //                result.VBW_Hz = (double)VBW;
                //                result.Level = new float[FreqArr.Length];
                //                for (int j = 0; j < FreqArr.Length; j++)
                //                {
                //                    result.Level[j] = LevelArr[j];
                //                }
                //                result.TimeStamp = timeService.GetGnssUtcTime().Ticks - uTCOffset;// new DateTime(1970, 1, 1, 0, 0, 0, System.DateTimeKind.Utc).Ticks;
                //                                                                                  //result.TimeStamp = DateTime.UtcNow.Ticks - new DateTime(1970, 1, 1, 0, 0, 0, System.DateTimeKind.Utc).Ticks;
                //                if (_RFOverload)
                //                {
                //                    result.DeviceStatus = COMR.Enums.DeviceStatus.RFOverload;
                //                }
                //                else
                //                {
                //                    result.DeviceStatus = COMR.Enums.DeviceStatus.Normal;
                //                }
                //                context.PushResult(result);
                //                break;
                //            }
                //        }
                //        else
                //        {
                //            i--;
                //        }
                //        if (PowerRegister != EN.PowerRegister.Normal)
                //        {
                //            _RFOverload = true;
                //        }
                //        // иногда нужно проверять токен окончания работы комманды
                //        if (context.Token.IsCancellationRequested)
                //        {
                //            // все нужно остановиться

                //            // если есть порция данных возвращаем ее в обработчки только говрим что поток результатов не законченный и больше уже не будет поступать
                //            var result2 = new COMR.MesureTraceResult(traceCount, CommandResultStatus.Ragged);

                //            context.PushResult(result2);


                //            // подтверждаем факт обработки отмены
                //            context.Cancel();
                //            // освобождаем поток 
                //            return;
                //        }
                //    }
                //}
                //Если TraceType Average/MinHold/MaxHold то делаем измерений сколько сказали и пушаем только готовый результат
                #endregion

                context.Unlock();

                // что то делаем еще 


                // подтверждаем окончание выполнения комманды 
                // важно: всн ранее устапнволеные в контексте обработки текущей команыд блокировки снимаются автоматически
                context.Finish();
                // дальше кода быть не должно, освобождаем поток
            }
            catch (Ivi.Visa.VisaException v_exp)
            {
                // желательно записать влог
                logger.Exception(Contexts.ThisComponent, v_exp);
                // этот вызов обязательный в случаи обрыва
                context.Unlock();
                context.Abort(v_exp);
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
        long t3;
        public void MesureIQStreamCommandHandler(COM.MesureIQStreamCommand command, IExecutionContext context)
        {
            try
            {
                //context.Lock(CommandType.MesureIQStream);
                context.Lock();
                //Переключимся на IQ
                if (mode)
                {
                    SetWindowType(false);
                }

                ValidateAndSetFreqCentrIQ(command.Parameter.FreqStart_Hz, command.Parameter.FreqStop_Hz);

                ValidateAndSetRefLevel(command.Parameter.RefLevel_dBm);

                ValidateAndSetPreAmp(command.Parameter.PreAmp_dB);

                ValidateAndSetATT(command.Parameter.Att_dB);

                ValidateAndSetFreqSpanIQ(command.Parameter.FreqStart_Hz, command.Parameter.FreqStop_Hz);
                //OR закоментировать что-то одно
                //FreqSpanIQ установит скорость семплирования IQ оно же зафиксирует полосу просмотра 
                //decimal samplespeed = LPC.SampleSpeed(UniqueData, ((decimal)command.Parameter.BitRate_MBs) * 1000000);
                //if (SampleSpeed != samplespeed)
                //{
                //    SetSampleSpeed(samplespeed);
                //}

                COMR.MesureIQStreamResult result = new COMR.MesureIQStreamResult(0, CommandResultStatus.Final)
                {
                    DeviceStatus = COMR.Enums.DeviceStatus.Normal
                };
                if (GetIQStream(ref result, command))
                {
                    context.PushResult(result);
                }
                /////////////
                //////////long timestop = _timeService.GetGnssTime().Ticks;
                context.Unlock();
                context.Finish();
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



        #region Param
        private ResourceManager rm = null;
        private readonly long uTCOffset = 621355968000000000;
        private readonly string decimalSeparator = System.Globalization.NumberFormatInfo.CurrentInfo.CurrencyDecimalSeparator;
        private TcpipSession session = null;
        private Ivi.Visa.IMessageBasedFormattedIO formattedIO = null;

        #region Instr
        private readonly List<DeviceOption> instrOption = new List<DeviceOption>()
        {
             new DeviceOption() { Type = "B4", Designation = "OCXO Reference Frequency", GlobalType = "OCXO Reference"},
             new DeviceOption() { Type = "B5", Designation = "Additional Interfaces", GlobalType = "Additional Interfaces"},
             new DeviceOption() { Type = "B10", Designation = "GPIB Interface", GlobalType = "GPIB Interface"},
             new DeviceOption() { Type = "B19", Designation = "Second Hard Disk (SSD)", GlobalType = "SSD"},
             new DeviceOption() { Type = "B22", Designation = "RF Preamplifier", GlobalType = "Preamplifier"},
             new DeviceOption() { Type = "B25", Designation = "1 dB Steps for Electronic Attenuator", GlobalType = "Attenuator 1 dB"},
             new DeviceOption() { Type = "B30", Designation = "DC Power Supply 12/24 V", GlobalType = "DC Power Supply"},
             new DeviceOption() { Type = "B31", Designation = "Internal Li-Ion Battery", GlobalType = "Internal Li-Ion Battery"},
             new DeviceOption() { Type = "B40", Designation = "40 MHz Analysis Bandwidth", GlobalType = "40 MHz Analysis Bandwidth"},
             new DeviceOption() { Type = "K7", Designation = "Analog Modulation Analysis (AM/FM/φM)", GlobalType = "Analog Modulation Analysis"},
             new DeviceOption() { Type = "K9", Designation = "Power Sensor Measurement with R&S®NRP Power Sensors", GlobalType = "Power Sensor Support"},
             new DeviceOption() { Type = "K30", Designation = "Noise Figure Measurement Application", GlobalType = "Noise Figure Measurement Application"},
        };
        private List<DeviceOption> loadedInstrOption;

        /// <summary>
        /// true = SpectrumAnalyzer
        /// false = IQAnalyzer
        /// </summary>
        private bool mode = true;

        private string instrModel = "";
        private string serialNumber = "";
        #region Freqs
        public decimal FreqMin = 9000;
        public decimal FreqMax = 3000000000;

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
        private readonly decimal[] RBWArr = new decimal[] { 1, 2, 3, 5, 10, 20, 30, 50, 100, 200, 300, 500, 1000, 2000, 3000, 5000, 6250, 10000, 20000, 30000, 50000, 100000, 200000, 300000, 500000, 1000000, 2000000, 3000000, 5000000, 10000000 };
        private readonly decimal[] VBWArr = new decimal[] { 1, 2, 3, 5, 10, 20, 30, 50, 100, 200, 300, 500, 1000, 2000, 3000, 5000, 10000, 20000, 30000, 50000, 100000, 200000, 300000, 500000, 1000000, 2000000, 3000000, 5000000, 10000000 };
        public decimal RBW;

        private int RBWIndex
        {
            get { return _RBWIndex; }
            set
            {
                if (value > RBWArr.Length - 1) _RBWIndex = RBWArr.Length - 1;
                else if (value < 0) _RBWIndex = 0;
                else _RBWIndex = value;
                RBW = RBWArr[_RBWIndex];
            }
        }
        private int _RBWIndex = 0;

        public bool AutoRBW;


        public decimal VBW;

        private int VBWIndex
        {
            get { return _VBWIndex; }
            set
            {
                if (value > VBWArr.Length - 1) _VBWIndex = VBWArr.Length - 1;
                else if (value < 0) _VBWIndex = 0;
                else _VBWIndex = value;
                VBW = VBWArr[_VBWIndex];
            }
        }
        private int _VBWIndex = 0;

        public bool AutoVBW;
        #endregion RBW / VBW

        #region Sweep
        private bool iWantMoreSweepPoints = false;

        private const decimal SWTMin = 0.000075m;
        private const decimal SWTMax = 8000;

        public decimal SweepTime;

        public bool AutoSweepTime;



        public int SweepPoints = 1001;
        private const int sweepPointsMax = 100001;
        private int sweepPointsIndex
        {
            get { return _sweepPointsIndex; }
            set
            {
                if (value > sweepPointArr.Length - 1) _sweepPointsIndex = sweepPointArr.Length - 1;
                else if (value < 0) _sweepPointsIndex = 0;
                else _sweepPointsIndex = value;
                SweepPoints = sweepPointArr[_sweepPointsIndex];
            }
        }
        private int _sweepPointsIndex = 0;

        public ParamWithId SweepTypeSelected { get; set; } = new ParamWithId { Id = 0, Parameter = "" };

        private EN.Optimization Optimization
        {
            get { return _Optimization; }
            set
            {
                for (int i = 0; i < optimization.Count; i++)
                {
                    if ((int)value == optimization[i].Id)
                    {
                        _Optimization = value;
                        OptimizationSelected = optimization[i];
                    }
                }
            }
        }
        private EN.Optimization _Optimization = EN.Optimization.Auto;
        private ParamWithId OptimizationSelected { get; set; } = new ParamWithId { Id = 0, Parameter = "" };

        private readonly List<ParamWithId> optimization = new List<ParamWithId>
        {
            new ParamWithId {Id = (int) EN.Optimization.Auto, Parameter = "AUTO" },
            new ParamWithId {Id = (int) EN.Optimization.Dynamic, Parameter = "DYN" },
            new ParamWithId {Id = (int) EN.Optimization.Speed, Parameter = "SPE" }
        };
        private readonly int[] sweepPointArr = new int[]{ 101, 125, 155, 173, 201, 251, 301, 313, 345, 401, 501, 601, 625, 691, 701, 801, 901, 1001,
                    1251, 1383, 1999, 2001, 2501, 2765, 3001, 4001, 5001, 6001, 7001, 8001, 9001,
                    10001, 11001, 12001, 13001, 14001, 15001, 16001, 17001, 18001, 19001,
                    20001, 21001, 22001, 23001, 24001, 25001, 26001, 27001, 28001, 29001,
                    30001, 31001, 32001, 33001, 34001, 35001, 36001, 37001, 38001, 39001,
                    40001, 41001, 42001, 43001, 44001, 45001, 46001, 47001, 48001, 49001,
                    50001, 51001, 52001, 53001, 54001, 55001, 56001, 57001, 58001, 59001,
                    60001, 61001, 62001, 63001, 64001, 65001, 66001, 67001, 68001, 69001,
                    70001, 71001, 72001, 73001, 74001, 75001, 76001, 77001, 78001, 79001,
                    80001, 81001, 82001, 83001, 84001, 85001, 86001, 87001, 88001, 89001,
                    90001, 91001, 92001, 93001, 94001, 95001, 96001, 97001, 98001, 99001,
                    100001 };

        private readonly List<ParamWithId> SweepType = new List<ParamWithId>
        {
            new ParamWithId {Id = (int) EN.SweepType.Auto, Parameter = "AUTO" },
            new ParamWithId {Id = (int) EN.SweepType.Sweep, Parameter = "SWE" },
            new ParamWithId {Id = (int) EN.SweepType.FFT, Parameter = "FFT" }
        };
        #endregion

        #region Level
        public EN.PowerRegister PowerRegister = EN.PowerRegister.Normal;
        private readonly int attMax = 45;
        private int attStep = 5;
        private bool preAmpAvailable;
        private const decimal RefLevelMax = 30;
        private const decimal RefLevelMin = -130;
        private const decimal RefLevelStep = 1;

        public decimal RefLevelSpec = -40;
        public decimal RefLevelIQ = -40;

        public decimal RangeSpec = 100;
        public decimal RangeIQ = 100;

        private decimal AttLevelSpec
        {
            get { return attLevelSpec; }
            set
            {
                if (value > attMax) attLevelSpec = attMax;
                else if (value < 0) attLevelSpec = 0;
                else attLevelSpec = value;
            }
        }
        private decimal attLevelSpec = 0;

        private decimal AttLevelIQ
        {
            get { return attLevelIQ; }
            set
            {
                if (value > attMax) attLevelIQ = attMax;
                else if (value < 0) attLevelIQ = 0;
                else attLevelIQ = value;
            }
        }
        private decimal attLevelIQ = 0;

        private bool attAutoSpec;
        private bool attAutoIQ;

        private bool preAmpSpec;
        private bool preAmpIQ;

        public ParamWithId LevelUnit { get; set; } = new ParamWithId() { Id = 0, Parameter = "" };

        private MEN.LevelUnit levelUnitResult = MEN.LevelUnit.dBm;

        private readonly List<ParamWithId> levelUnits = new List<ParamWithId>
        {
            new ParamWithId {Id = (int) MEN.LevelUnit.dBm, Parameter = "DBM" },
            new ParamWithId {Id = (int) MEN.LevelUnit.dBmV, Parameter = "DBMV" },
            new ParamWithId {Id = (int) MEN.LevelUnit.dBµV, Parameter = "DBUV" },
            new ParamWithId {Id = (int) MEN.LevelUnit.Watt, Parameter = "WATT" },
        };
        #endregion Level

        #region Trace Data
        private int tracePointsMaxPool = 0;
        private const int tracePointsMax = 100_001;

        private double resFreqStart = 10000;
        private double resFreqStep = 10000;

        private ulong traceCountToMeas = 1;
        private ulong traceCount = 1;
        private ulong numberOfSweeps = 0;

        //public double[] FreqArr;
        public float[] LevelArr;
        private float[] levelArrTemp;//нужед ля сравнения пришедших трейсов

        private ParamWithId trace1Type { get; set; } = new ParamWithId { Id = 0, Parameter = "BLAN" };
        private EN.TraceType traceTypeResult = EN.TraceType.ClearWrite;
        private ParamWithId trace1Detector { get; set; } = new ParamWithId { Id = 0, Parameter = "AutoSelect" };

        private bool traceReset;
        private AveragedTrace traceAveraged = new AveragedTrace();

        private readonly List<ParamWithId> traceTypes = new List<ParamWithId>
        {
            new ParamWithId {Id = (int) EN.TraceType.ClearWrite, Parameter = "WRIT" },
            new ParamWithId {Id = (int) EN.TraceType.Average, Parameter = "AVER" },
            new ParamWithId {Id = (int) EN.TraceType.MaxHold, Parameter = "MAXH" },
            new ParamWithId {Id = (int) EN.TraceType.MinHold, Parameter = "MINH" },
            new ParamWithId {Id = (int) EN.TraceType.View, Parameter = "VIEW" },
            new ParamWithId {Id = (int) EN.TraceType.Blank, Parameter = "BLAN" }
        };
        private readonly List<ParamWithId> traceDetectors = new List<ParamWithId>
        {
            new ParamWithId {Id = (int) EN.TraceDetector.AutoSelect, Parameter = "Auto Select" },
            new ParamWithId {Id = (int) EN.TraceDetector.AutoPeak, Parameter = "APE" },
            new ParamWithId {Id = (int) EN.TraceDetector.Average, Parameter = "AVER" },
            new ParamWithId {Id = (int) EN.TraceDetector.MaxPeak, Parameter = "POS" },
            new ParamWithId {Id = (int) EN.TraceDetector.MinPeak, Parameter = "NEG" },
            new ParamWithId {Id = (int) EN.TraceDetector.Sample, Parameter = "SAMP" },
            new ParamWithId {Id = (int) EN.TraceDetector.RMS, Parameter = "RMS" }
        };
        #endregion

        #region IQ
        private decimal freqCentrIQ { get; set; } = 1000000000;
        private decimal IQBW { get; set; } = 100000;
        private decimal sampleSpeed { get; set; } = 100000;
        private int sampleLength { get; set; } = 10000;
        /// <summary>
        /// Длительность одного семпла
        /// </summary>
        public decimal SampleTimeLength { get; set; } = 10000;
        private decimal IQMeasTime { get; set; } = 10000;
        private decimal IQMeasTimeAll { get; set; } = 10000;

        public float[] IQArr = new float[2] { -1, -1 };

        public decimal TriggerOffset { get; set; } = 10000;
        private decimal triggerOffsetInSample { get; set; } = 10000;
        private decimal triggerOffsetMax = 20;
        #endregion IQ
        #endregion Instr
        #endregion Param

        #region NI Visa
        private void WriteString(string Input)
        {
            //session.FormattedIO.WriteLine(Input);
            formattedIO.WriteLine(Input);

            //session.RawIO.Write(Input);
        }
        private string ReadString()
        {
            //return session.FormattedIO.ReadString();
            return formattedIO.ReadString();
        }
        private string QueryString(string Input)
        {
            formattedIO.WriteLine(Input);
            return formattedIO.ReadString();
            //session.FormattedIO.WriteLine(Input);
            //return session.FormattedIO.ReadString();
            ////session.RawIO.Write(Input);
            ////return session.RawIO.ReadString().TrimEnd();
        }
        private byte[] ReadByte()
        {
            //return session.FormattedIO.ReadBinaryBlockOfByte();
            return formattedIO.ReadBinaryBlockOfByte();
        }
        private byte[] QueryByte(string Input)
        {
            //session.FormattedIO.WriteLine(Input);
            //return session.FormattedIO.ReadBinaryBlockOfByte();
            formattedIO.WriteLine(Input);
            return formattedIO.ReadBinaryBlockOfByte();
        }
        //private float[] QueryFloat(string Input)
        //{
        //    session.FormattedIO.WriteLine(Input);
        //    byte[] byteArray = session.FormattedIO.ReadBinaryBlockOfByte();
        //    float[] temp = new float[byteArray.Length / 4];
        //    for (int j = 0; j < temp.Length / 4; j++)
        //    {
        //        temp[j] = System.BitConverter.ToSingle(byteArray, j * 4);
        //    }
        //    return temp;
        //}
        private decimal QueryDecimal(string Input)
        {
            //session.FormattedIO.WriteLine(Input);
            //return decimal.Parse(session.FormattedIO.ReadString().Replace(".", decimalSeparator));
            formattedIO.WriteLine(Input);//23216.65546465
            return decimal.Parse(formattedIO.ReadString().Replace(".", decimalSeparator));
        }
        private int QueryInt(string Input)
        {
            //session.FormattedIO.WriteLine(Input);
            //return int.Parse(session.FormattedIO.ReadString());
            formattedIO.WriteLine(Input);
            return int.Parse(formattedIO.ReadString());
        }
        private ulong QueryULong(string Input)
        {
            //session.FormattedIO.WriteLine(Input);
            //return ulong.Parse(session.FormattedIO.ReadString());
            formattedIO.WriteLine(Input);
            return ulong.Parse(formattedIO.ReadString());
        }
        #endregion NI Visa

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


        #region ValidateAndSet
        private void ValidateAndSetFreqStartStop(decimal freqStart, decimal freqStop, ref bool frequencyChanged)
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
                WriteString(":SENSe:FREQ:STAR " + FreqStart.ToString().Replace(decimalSeparator, "."));
                //FreqStart = QueryDecimal(":SENSe:FREQ:STAR?");

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
                WriteString(":SENSe:FREQ:STOP " + FreqStop.ToString().Replace(decimalSeparator, "."));
                frequencyChanged = true;
                //FreqStop = QueryDecimal(":SENSe:FREQ:STOP?");
            }
            else
            {
                frequencyChanged = false;
            }
        }
        private (decimal freqStart, decimal freqStop) ValidateFreqStartStop(decimal freqStart, decimal freqStop)
        {
            decimal freq1 = 0, freq2 = 0;
            if (freqStart < FreqMin)
            {
                freq1 = FreqMin;
                logger.Warning(Contexts.ThisComponent, "FreqStart set to limit value");
            }
            else if (freqStart > FreqMax)
            {
                freq1 = FreqMax;
                logger.Warning(Contexts.ThisComponent, "FreqStart set to limit value");
            }
            else freq1 = freqStart;

            if (freqStop > FreqMax)
            {
                freq2 = FreqMax;
                logger.Warning(Contexts.ThisComponent, "FreqStop set to limit value");
            }
            else if (freqStop < FreqMin)
            {
                freq2 = FreqMin + 1000000;
                logger.Warning(Contexts.ThisComponent, "FreqStop set to limit value");
            }
            else freq2 = freqStop;

            return (freq1, freq2);
        }
        private void ValidateAndSetRefLevel(int refLevel)
        {
            decimal refl = 0;
            if (refLevel == 1000000000)
            {
                refl = -20;
            }
            else
            {
                if (refLevel > RefLevelMax)
                {
                    refl = RefLevelMax;
                    logger.Warning(Contexts.ThisComponent, "Reference level set to limit value");
                }
                else if (refLevel < RefLevelMin)
                {
                    refl = RefLevelMin;
                    logger.Warning(Contexts.ThisComponent, "Reference level set to limit value");
                }
                else
                {
                    refl = refLevel;
                }
            }

            if (mode)
            {
                if (RefLevelSpec != refl)
                {
                    RefLevelSpec = refl;
                    WriteString(":DISPlay:WINDow:TRACe:Y:SCALe:RLEVel " + RefLevelSpec.ToString().Replace(decimalSeparator, "."));
                    if (attAutoSpec) { GetAttLevel(); }
                }
            }
            else
            {
                if (RefLevelIQ != refl)
                {
                    RefLevelIQ = refl;
                    WriteString(":DISPlay:WINDow:TRACe:Y:SCALe:RLEVel " + RefLevelIQ.ToString().Replace(decimalSeparator, "."));
                    if (attAutoIQ == true) { GetAttLevel(); }
                }
            }
        }
        private void ValidateAndSetPreAmp(int preAmp)
        {
            bool preamp = false;
            if (preAmp == -1)
            {
                preamp = false;//потому что нет альтернативы, в любом анализаторе спектра нет автоматического усилителя, 
                               //Если вы работете с анализатором спектра то понимаете что вы делаете
            }
            else if (preAmp == 0)
            {
                preamp = false;
            }
            else if (preAmp == 1)
            {
                preamp = true;
            }
            else if (preAmp > 1 || preAmp < -1)
            {
                logger.Warning(Contexts.ThisComponent, "PreAmp must be set to within limits.");
            }

            if (mode)
            {
                if (preAmpSpec != preamp)
                {
                    preAmpSpec = preamp;
                    if (preAmpAvailable)
                    {
                        if (preAmpSpec)
                        {
                            WriteString(":INP:GAIN:STAT 1");
                        }
                        else
                        {
                            WriteString(":INP:GAIN:STAT 0");
                        }
                    }
                    else
                    {
                        logger.Warning(Contexts.ThisComponent, "PreAmp unavailable");
                    }
                }
            }
            else
            {
                if (preAmpIQ != preamp)
                {
                    preAmpIQ = preamp;
                    if (preAmpAvailable)
                    {
                        if (preAmpIQ)
                        {
                            WriteString(":INP:GAIN:STAT 1");
                        }
                        else
                        {
                            WriteString(":INP:GAIN:STAT 0");
                        }
                    }
                    else
                    {
                        logger.Warning(Contexts.ThisComponent, "PreAmp unavailable");
                    }
                }
            }
        }
        private void ValidateAndSetATT(int att)
        {
            if (att > -1)
            {
                int res = 0;
                int delta = int.MaxValue;
                for (int i = 0; i <= attMax; i += attStep)
                {
                    if (Math.Abs(att - i) < delta)
                    {
                        delta = Math.Abs(att - i);
                        res = i;
                    }
                }
                if (mode)
                {
                    if (AttLevelSpec != res)
                    {
                        AttLevelSpec = res;
                        attAutoSpec = false;
                        WriteString(":INP:ATT " + AttLevelSpec.ToString().Replace(decimalSeparator, "."));
                    }
                }
                else
                {
                    if (AttLevelIQ != res)
                    {
                        AttLevelIQ = res;
                        attAutoIQ = false;
                        WriteString(":INP:ATT " + AttLevelIQ.ToString().Replace(decimalSeparator, "."));
                    }
                }
            }
            else
            {
                if (mode)
                {
                    attAutoSpec = true;
                    WriteString(":INP:ATT:AUTO 1");
                    AttLevelSpec = QueryDecimal(":INP:ATT?");
                }
                else
                {
                    attAutoIQ = true;
                    WriteString(":INP:ATT:AUTO 1");
                    AttLevelIQ = QueryDecimal(":INP:ATT?");
                }
            }
        }

        private void ValidateAndSetRBW(decimal rbw)
        {
            decimal res = 0;
            decimal delta = decimal.MaxValue;
            int index = 0;
            for (int i = 0; i < RBWArr.Length; i++)
            {
                if (Math.Abs(rbw - RBWArr[i]) < delta)
                {
                    delta = Math.Abs(rbw - RBWArr[i]);
                    index = i;
                }
            }
            if (index > 1 && RBWArr[index] > rbw)
            {
                res = RBWArr[index - 1];
            }
            else
            {
                res = RBWArr[index];
            }
            if (RBW != res)
            {
                RBW = res;
                AutoRBW = false;
                WriteString(":SENSe:BWIDth:RESolution " + RBW.ToString().Replace(decimalSeparator, "."));
            }
        }
        private void ValidateAndSetVBW(decimal vbw)
        {
            decimal res = 0;
            decimal delta = decimal.MaxValue;
            int index = 0;
            for (int i = 0; i < VBWArr.Length; i++)
            {
                if (Math.Abs(vbw - VBWArr[i]) < delta)
                {
                    delta = Math.Abs(vbw - VBWArr[i]);
                    index = i;
                }
            }
            if (index > 1 && VBWArr[index] > vbw)
            {
                res = VBWArr[index - 1];
            }
            else
            {
                res = VBWArr[index];
            }
            if (VBW != res)
            {
                VBW = res;
                AutoVBW = false;
                WriteString(":SENSe:BANDwidth:VIDeo " + VBW.ToString().Replace(decimalSeparator, "."));
            }
        }

        private void ValidateAndSetSweepPoints(int sweepPoint)
        {
            int res = 0;
            decimal delta = decimal.MaxValue;
            int index = 0;
            for (int i = 0; i < sweepPointArr.Length; i++)
            {
                if (Math.Abs(sweepPoint - sweepPointArr[i]) < delta)
                {
                    delta = Math.Abs(sweepPoint - sweepPointArr[i]);
                    index = i;
                }
            }
            if (index < sweepPointArr.Length - 2 && sweepPointArr[index] < sweepPoint)
            {
                res = sweepPointArr[index + 1];
            }
            else
            {
                res = sweepPointArr[index];
            }
            if (SweepPoints != res)
            {
                SweepPoints = res;
                WriteString(":SWE:POIN " + SweepPoints.ToString());
            }
        }

        private void ValidateAndSetDetectorType(COMP.DetectorType detectorType)
        {
            ParamWithId res = new ParamWithId { Id = 0, Parameter = "BLAN" };
            for (int i = 0; i < traceDetectors.Count; i++)
            {
                if (detectorType == COMP.DetectorType.Auto && traceDetectors[i].Id == (int)EN.TraceDetector.AutoPeak)
                {
                    res = traceDetectors[i];
                }
                else if (detectorType == COMP.DetectorType.Average && traceDetectors[i].Id == (int)EN.TraceDetector.Average)
                {
                    res = traceDetectors[i];
                }
                else if (detectorType == COMP.DetectorType.MaxPeak && traceDetectors[i].Id == (int)EN.TraceDetector.MaxPeak)
                {
                    res = traceDetectors[i];
                }
                else if (detectorType == COMP.DetectorType.MinPeak && traceDetectors[i].Id == (int)EN.TraceDetector.MinPeak)
                {
                    res = traceDetectors[i];
                }
                else if (detectorType == COMP.DetectorType.RMS && traceDetectors[i].Id == (int)EN.TraceDetector.RMS)
                {
                    res = traceDetectors[i];
                }
            }
            if (res.Parameter == "BLAN")
            {
                throw new Exception("The TraceDetector must be set to the available instrument range.");
            }
            if (trace1Detector != res)
            {
                trace1Detector = res;
                if (trace1Detector.Parameter == "Auto Select")
                {
                    WriteString(":SENSe:DET1:AUTO 1");
                }
                else
                {
                    WriteString(":SENSe:DET1 " + trace1Detector.Parameter);
                }
            }
        }

        private void ValidateAndSetTraceType(COMP.TraceType traceType, bool frequencyChanged)
        {
            ParamWithId res = new ParamWithId { Id = 0, Parameter = "BLAN" };
            for (int i = 0; i < traceTypes.Count; i++)
            {
                if (traceType == COMP.TraceType.ClearWhrite && traceTypes[i].Id == (int)EN.TraceType.ClearWrite)
                {
                    res = traceTypes[i];
                }
                else if (traceType == COMP.TraceType.Average && traceTypes[i].Id == (int)EN.TraceType.Average)
                {
                    res = traceTypes[i];
                }
                else if (traceType == COMP.TraceType.MaxHold && traceTypes[i].Id == (int)EN.TraceType.MaxHold)
                {
                    res = traceTypes[i];
                }
                else if (traceType == COMP.TraceType.MinHold && traceTypes[i].Id == (int)EN.TraceType.MinHold)
                {
                    res = traceTypes[i];
                }
                else if (traceType == COMP.TraceType.Auto && traceTypes[i].Id == (int)EN.TraceType.ClearWrite)
                {
                    //По результатам согласования принято такое решение
                    res = traceTypes[i];
                }
            }
            if (res.Parameter == "BLAN")
            {
                throw new Exception("The TraceDetector must be set to the available instrument range.");
            }
            if (trace1Type != res)
            {
                trace1Type = res;
                WriteString(":DISP:TRAC1:MODE " + trace1Type.Parameter);
            }
            if (!frequencyChanged)
            {
                WriteString(":DISP:TRAC1:MODE " + traceTypes[0].Parameter);
                WriteString(":DISP:TRAC1:MODE " + trace1Type.Parameter);
            }
        }
        private void ValidateAndSetTraceTypeWithAggregation(COMP.TraceType traceType)
        {
            ParamWithId res = new ParamWithId { Id = 0, Parameter = "BLAN" };
            traceTypeResult = EN.TraceType.ClearWrite;
            for (int i = 0; i < traceTypes.Count; i++)
            {
                //Всегда исходный ClearWrite
                if (traceTypes[i].Id == (int)EN.TraceType.ClearWrite)
                {
                    res = traceTypes[i];
                }
                //Результирующий по наявности
                if (traceType == COMP.TraceType.ClearWhrite && traceTypes[i].Id == (int)EN.TraceType.ClearWrite)
                {
                    traceTypeResult = EN.TraceType.ClearWrite;
                }
                else if (traceType == COMP.TraceType.Average && traceTypes[i].Id == (int)EN.TraceType.Average)
                {
                    traceTypeResult = EN.TraceType.Average;
                }
                else if (traceType == COMP.TraceType.MaxHold && traceTypes[i].Id == (int)EN.TraceType.MaxHold)
                {
                    traceTypeResult = EN.TraceType.MaxHold;
                }
                else if (traceType == COMP.TraceType.MinHold && traceTypes[i].Id == (int)EN.TraceType.MinHold)
                {
                    traceTypeResult = EN.TraceType.MinHold;
                }
                else if (traceType == COMP.TraceType.Auto && traceTypes[i].Id == (int)EN.TraceType.ClearWrite)
                {
                    //По результатам согласования принято такое решение
                    traceTypeResult = EN.TraceType.ClearWrite;
                }
            }
            if (res.Parameter == "BLAN")
            {
                throw new Exception("The TraceDetector must be set to the available instrument range.");
            }
            if (trace1Type != res)
            {
                trace1Type = res;
                WriteString(":DISP:TRAC1:MODE " + trace1Type.Parameter);
            }            
        }

        private void ValidateAndSetLevelUnit(COMP.LevelUnit levelUnit)
        {
            ParamWithId res = new ParamWithId { Id = 0, Parameter = "BLAN" };
            for (int i = 0; i < levelUnits.Count; i++)
            {
                if (levelUnit == COMP.LevelUnit.dBm && levelUnits[i].Id == (int)MEN.LevelUnit.dBm)
                {
                    levelUnitResult = MEN.LevelUnit.dBm;
                    res = levelUnits[i];
                }
                else if (levelUnit == COMP.LevelUnit.dBmkV && levelUnits[i].Id == (int)MEN.LevelUnit.dBµV)
                {
                    levelUnitResult = MEN.LevelUnit.dBµV;
                    res = levelUnits[i];
                }
            }
            if (res.Parameter == "BLAN")
            {
                throw new Exception("The LevelUnits must be set to the available instrument range.");
            }
            if (LevelUnit != res)
            {
                LevelUnit = res;
                WriteString(":UNIT:POWer " + LevelUnit.Parameter);
            }
        }
        private void ValidateAndSetLevelUnitWithAggregation(COMP.LevelUnit levelUnit, EN.TraceType tType)
        {
            ParamWithId res = new ParamWithId { Id = 0, Parameter = "BLAN" };
            levelUnitResult = MEN.LevelUnit.dBm;
            if (tType == EN.TraceType.Average)
            {
                for (int i = 0; i < levelUnits.Count; i++)
                {
                    if (levelUnits[i].Id == (int)MEN.LevelUnit.Watt)
                    {
                        res = levelUnits[i];
                    }
                    if (levelUnit == COMP.LevelUnit.dBm && levelUnits[i].Id == (int)MEN.LevelUnit.dBm)
                    {
                        levelUnitResult = MEN.LevelUnit.dBm;
                    }
                    else if (levelUnit == COMP.LevelUnit.dBmkV && levelUnits[i].Id == (int)MEN.LevelUnit.dBµV)
                    {
                        levelUnitResult = MEN.LevelUnit.dBµV;
                    }
                }
            }
            else
            {
                for (int i = 0; i < levelUnits.Count; i++)
                {
                    if (levelUnit == COMP.LevelUnit.dBm && levelUnits[i].Id == (int)MEN.LevelUnit.dBm)
                    {
                        res = levelUnits[i];
                        levelUnitResult = MEN.LevelUnit.dBm;
                    }
                    else if (levelUnit == COMP.LevelUnit.dBmkV && levelUnits[i].Id == (int)MEN.LevelUnit.dBµV)
                    {
                        res = levelUnits[i];
                        levelUnitResult = MEN.LevelUnit.dBµV;
                    }
                }
            }
            if (res.Parameter == "BLAN")
            {
                throw new Exception("The LevelUnits must be set to the available instrument range.");
            }
            if (LevelUnit != res)
            {
                LevelUnit = res;
                WriteString(":UNIT:POWer " + LevelUnit.Parameter);
            }
        }

        private void ValidateAndSetSweepTime(decimal sweepTime)
        {
            if (adapterConfig.OnlyAutoSweepTime)
            {
                if (!AutoSweepTime)
                {
                    AutoSweepTime = true;
                    WriteString(":SWE:TIME:AUTO 1");
                }
            }
            else
            {
                if (sweepTime <= 0)
                {
                    AutoSweepTime = true;
                    WriteString(":SWE:TIME:AUTO 1");
                }
                else
                {
                    if (sweepTime < SWTMin || sweepTime > SWTMax)
                    {
                        throw new Exception("The SweepTime must be set to the available range of the instrument.");
                    }
                    else
                    {
                        if (SweepTime != sweepTime)
                        {
                            AutoSweepTime = false;
                            SweepTime = sweepTime;
                            WriteString(":SWE:TIME " + SweepTime.ToString().Replace(decimalSeparator, "."));
                        }
                    }
                }
            }
            //SweepTime = QueryDecimal(":SWE:TIME?");
            //long t = timeService.TimeStamp.Ticks;

            //System.Diagnostics.Debug.WriteLine("111  " + new TimeSpan(timeService.TimeStamp.Ticks - t).ToString() );
        }

        private void ValidateAndSetFreqCentrIQ(decimal freqStart, decimal freqStop)
        {
            if (freqStart < FreqMin || freqStop > FreqMax)
            {
                throw new Exception("The stop frequency must be set to the available range of the instrument.");
            }
            else
            {
                decimal f = (freqStart + freqStop) / 2;
                if (f != freqCentrIQ)
                {
                    freqCentrIQ = f;
                    WriteString("FREQ:CENT " + freqCentrIQ.ToString().Replace(decimalSeparator, "."));
                    //freqCentrIQ = QueryDecimal(":FREQ:CENT?");
                }
            }
        }

        private void ValidateAndSetFreqSpanIQ(decimal freqStart, decimal freqStop)
        {
            if (freqStart < FreqMin || freqStop > FreqMax)
            {
                throw new Exception("The stop frequency must be set to the available range of the instrument.");
            }
            else
            {
                decimal f = freqStop - freqStart;
                if (f != IQBW)
                {
                    IQBW = f;
                    sampleSpeed = 1.25m * IQBW;
                    SampleTimeLength = 1 / sampleSpeed;
                    WriteString("TRAC:IQ:BWID " + IQBW.ToString().Replace(decimalSeparator, "."));
                }
            }
        }
        #endregion ValidateAndSet

        long t = 0;
        bool getSweeptime = false;
        private void GetAndPushTraceResults(COM.MesureTraceCommand command, IExecutionContext context)
        {
            getSweeptime = true;
            string poolKeyName = "";
            bool poolKeyFind = false;
            t = timeService.TimeStamp.Ticks;
            //Если TraceType ClearWrite то пушаем каждый результат
            if (trace1Type.Id == (int)EN.TraceType.ClearWrite)
            {
                for (ulong i = 0; i < traceCountToMeas; i++)
                {
                    MeasurementInitialization();
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
                            result = context.TakeResult<COMR.MesureTraceResult>(poolKeyName, traceCount - 1, CommandResultStatus.Final);
                        }
                        else
                        {
                            result = context.TakeResult<COMR.MesureTraceResult>(poolKeyName, traceCount - 1, CommandResultStatus.Next);
                        }

                        for (int j = 0; j < SweepPoints; j++)
                        {
                            result.Level[j] = LevelArr[j];
                        }
                        result.LevelMaxIndex = SweepPoints;
                        result.FrequencyStart_Hz = resFreqStart;
                        result.FrequencyStep_Hz = resFreqStep;

                        result.TimeStamp = timeService.GetGnssUtcTime().Ticks - uTCOffset;// new DateTime(1970, 1, 1, 0, 0, 0, System.DateTimeKind.Utc).Ticks;//неюзабельно
                                                                                          //result.TimeStamp = DateTime.UtcNow.Ticks - new DateTime(1970, 1, 1, 0, 0, 0, System.DateTimeKind.Utc).Ticks;
                        result.Att_dB = (int)AttLevelSpec;
                        result.PreAmp_dB = preAmpSpec ? 1 : 0;
                        result.RefLevel_dBm = (int)RefLevelSpec;
                        result.RBW_Hz = (double)RBW;
                        result.VBW_Hz = (double)VBW;
                        result.DeviceStatus = COMR.Enums.DeviceStatus.Normal;
                        if (PowerRegister != EN.PowerRegister.Normal)
                        {
                            result.DeviceStatus = COMR.Enums.DeviceStatus.RFOverload;
                        }

                        context.PushResult(result);
                    }
                    else
                    {
                        i--;
                    }

                    // иногда нужно проверять токен окончания работы комманды
                    if (context.Token.IsCancellationRequested)
                    {
                        // все нужно остановиться

                        // если есть порция данных возвращаем ее в обработчки только говрим что поток результатов не законченный и больше уже не будет поступать
                        var result2 = context.TakeResult<COMR.MesureTraceResult>(poolKeyName, traceCount - 1, CommandResultStatus.Ragged);
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
                bool _RFOverload = false;
                MeasurementInitialization();
                for (ulong i = 0; i < traceCountToMeas; i++)
                {
                    if (GetNumberOfSweeps())
                    {
                        if (GetTrace())
                        {
                            break;//таки новые данные можно публиковать
                        }
                    }
                    else
                    {
                        i--;
                    }
                    if (!_RFOverload && PowerRegister != EN.PowerRegister.Normal)
                    {
                        _RFOverload = true;
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
                if (!context.Token.IsCancellationRequested)
                {
                    if (!poolKeyFind)
                    {
                        FindTracePoolName(LevelArr.Length, ref poolKeyFind, ref poolKeyName);
                    }
                    COMR.MesureTraceResult result = context.TakeResult<COMR.MesureTraceResult>(poolKeyName, traceCountToMeas - 1, CommandResultStatus.Final);
                    result.LevelMaxIndex = SweepPoints;
                    result.FrequencyStart_Hz = resFreqStart;
                    result.FrequencyStep_Hz = resFreqStep;

                    result.Att_dB = (int)AttLevelSpec;
                    result.RefLevel_dBm = (int)RefLevelSpec;
                    result.PreAmp_dB = preAmpSpec ? 1 : 0;
                    result.RBW_Hz = (double)RBW;
                    result.VBW_Hz = (double)VBW;
                    for (int j = 0; j < SweepPoints; j++)
                    {
                        result.Level[j] = LevelArr[j];
                    }
                    result.TimeStamp = timeService.GetGnssUtcTime().Ticks - uTCOffset;// new DateTime(1970, 1, 1, 0, 0, 0, System.DateTimeKind.Utc).Ticks;
                                                                                      //result.TimeStamp = DateTime.UtcNow.Ticks - new DateTime(1970, 1, 1, 0, 0, 0, System.DateTimeKind.Utc).Ticks;

                    context.PushResult(result);
                }
            }
        }
        private void GetAndPushTraceResultsWithAggregation(COM.MesureTraceCommand command, IExecutionContext context,
            decimal freqStart, decimal freqStop, int steps, int actualSweepPoints)
        {
            getSweeptime = true;
            string poolKeyName = "";
            bool poolKeyFind = false;
            t = timeService.TimeStamp.Ticks;
            //Если TraceType ClearWrite то пушаем каждый результат
            if (trace1Type.Id == (int)EN.TraceType.ClearWrite)
            {
                for (ulong i = 0; i < traceCountToMeas; i++)
                {
                    if (GetTraceWithAggregation(freqStart, freqStop, steps, actualSweepPoints))
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
                            result = context.TakeResult<COMR.MesureTraceResult>(poolKeyName, traceCount - 1, CommandResultStatus.Final);
                        }
                        else
                        {
                            result = context.TakeResult<COMR.MesureTraceResult>(poolKeyName, traceCount - 1, CommandResultStatus.Next);
                        }

                        for (int j = 0; j < SweepPoints; j++)
                        {
                            result.Level[j] = LevelArr[j];
                        }
                        result.LevelMaxIndex = SweepPoints;
                        result.FrequencyStart_Hz = resFreqStart;
                        result.FrequencyStep_Hz = resFreqStep;

                        result.TimeStamp = timeService.GetGnssUtcTime().Ticks - uTCOffset;// new DateTime(1970, 1, 1, 0, 0, 0, System.DateTimeKind.Utc).Ticks;//неюзабельно
                                                                                          //result.TimeStamp = DateTime.UtcNow.Ticks - new DateTime(1970, 1, 1, 0, 0, 0, System.DateTimeKind.Utc).Ticks;
                        result.Att_dB = (int)AttLevelSpec;
                        result.PreAmp_dB = preAmpSpec ? 1 : 0;
                        result.RefLevel_dBm = (int)RefLevelSpec;
                        result.RBW_Hz = (double)RBW;
                        result.VBW_Hz = (double)VBW;
                        result.DeviceStatus = COMR.Enums.DeviceStatus.Normal;
                        if (PowerRegister != EN.PowerRegister.Normal)
                        {
                            result.DeviceStatus = COMR.Enums.DeviceStatus.RFOverload;
                        }

                        context.PushResult(result);
                    }
                    else
                    {
                        i--;
                    }

                    // иногда нужно проверять токен окончания работы комманды
                    if (context.Token.IsCancellationRequested)
                    {
                        // все нужно остановиться

                        // если есть порция данных возвращаем ее в обработчки только говрим что поток результатов не законченный и больше уже не будет поступать
                        var result2 = context.TakeResult<COMR.MesureTraceResult>(poolKeyName, traceCount - 1, CommandResultStatus.Ragged);
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
                bool _RFOverload = false;
                for (ulong i = 0; i < traceCountToMeas; i++)
                {
                    if (GetTraceWithAggregation(freqStart, freqStop, steps, actualSweepPoints))
                    {
                        //break;//таки новые данные можно публиковать
                    }
                    else
                    {
                        i--;
                    }
                    if (!_RFOverload && PowerRegister != EN.PowerRegister.Normal)
                    {
                        _RFOverload = true;
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
                if (!context.Token.IsCancellationRequested)
                {
                    if (!poolKeyFind)
                    {
                        FindTracePoolName(LevelArr.Length, ref poolKeyFind, ref poolKeyName);
                    }
                    COMR.MesureTraceResult result = context.TakeResult<COMR.MesureTraceResult>(poolKeyName, traceCountToMeas - 1, CommandResultStatus.Final);
                    result.LevelMaxIndex = SweepPoints;
                    result.FrequencyStart_Hz = resFreqStart;
                    result.FrequencyStep_Hz = resFreqStep;

                    result.Att_dB = (int)AttLevelSpec;
                    result.RefLevel_dBm = (int)RefLevelSpec;
                    result.PreAmp_dB = preAmpSpec ? 1 : 0;
                    result.RBW_Hz = (double)RBW;
                    result.VBW_Hz = (double)VBW;
                    for (int j = 0; j < SweepPoints; j++)
                    {
                        result.Level[j] = LevelArr[j];
                    }
                    result.TimeStamp = timeService.GetGnssUtcTime().Ticks - uTCOffset;// new DateTime(1970, 1, 1, 0, 0, 0, System.DateTimeKind.Utc).Ticks;
                                                                                      //result.TimeStamp = DateTime.UtcNow.Ticks - new DateTime(1970, 1, 1, 0, 0, 0, System.DateTimeKind.Utc).Ticks;

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
                        name = mainConfig.AdapterTraceResultPools[p].Size.ToString();
                        state = true;
                        break;
                    }
                }
            }
        }

        private bool SetConnect()
        {
            bool res = false;
            rm = new ResourceManager();

            session = (TcpipSession)rm.Open(String.Concat("TCPIP::", adapterConfig.IPAddress, "::hislip"));//, AccessModes.None, 20000);
            formattedIO = session.FormattedIO;
            string[] temp = QueryString("*IDN?").Trim('"').Split(',');
            if (temp[0].Contains("Rohde&Schwarz"))
            {
                if (temp[1].Contains("FPL"))
                {
                    instrModel = temp[1];
                    serialNumber = temp[2].Split('/')[1];

                    List<DeviceOption> Loaded = new List<DeviceOption>() { };
                    loadedInstrOption = new List<DeviceOption>();
                    string[] op = QueryString("*OPT?").ToUpper().Split(',');
                    if (op.Length > 0 && op[0] != "0")
                    {
                        bool findDemoOption = false;
                        foreach (string s in op)
                        {
                            if (s.ToUpper() == "K0")
                            {
                                findDemoOption = true;
                                Loaded = instrOption;
                            }
                        }
                        if (findDemoOption == false)
                        {
                            foreach (string s in op)
                            {
                                foreach (DeviceOption so in instrOption)
                                {
                                    if (so.Type == s)
                                    {
                                        Loaded.Add(so);
                                    }
                                }

                            }
                        }
                    }
                    loadedInstrOption = Loaded;


                    SetPreset();

                    WriteString("FORMat:DEXPort:DSEParator COMM");// POIN");//разделитель дробной части
                    WriteString(":FORM:DATA REAL,32");

                    if (adapterConfig.DisplayUpdate)
                    {
                        WriteString(":SYST:DISP:UPD ON");
                    }
                    else
                    {
                        WriteString(":SYST:DISP:UPD OFF");
                    }

                    SweepPoints = QueryInt(":SWE:POIN?");

                    FreqMin = QueryDecimal(":SENSe:FREQuency:STAR? MIN");
                    FreqMax = QueryDecimal(":SENSe:FREQuency:STOP? MAX");
                    WriteString("INIT:CONT OFF");/////////////////////////////////////////////////////////////////////////////////////////
                    preAmpAvailable = false;
                    if (loadedInstrOption.Count > 0)
                    {
                        for (int i = 0; i < loadedInstrOption.Count; i++)
                        {
                            if (loadedInstrOption[i].GlobalType == "Preamplifier") { preAmpAvailable = true; }
                            if (loadedInstrOption[i].Type == "B25") { attStep = 1; }
                        }
                    }
                    sweepPointsIndex = System.Array.IndexOf(sweepPointArr, SweepPoints);

                    #region

                    GetLevelUnit();
                    GetFreqCentr();
                    GetFreqSpan();
                    GetRBW();
                    GetAutoRBW();
                    GetVBW();
                    GetAutoVBW();

                    GetOptimization();
                    SetOptimization((EN.Optimization)adapterConfig.Optimization);

                    GetSweepTime();
                    GetAutoSweepTime();
                    GetSweepType();
                    GetSweepPoints();

                    GetRefLevel();
                    GetRange();
                    GetAttLevel();
                    GetAutoAttLevel();
                    GetPreAmp();
                    GetTraceType();
                    GetDetectorType();


                    SetIQWindow();//создали окно IQ
                    SetWindowType(false);
                    GetRefLevel();
                    GetRange();
                    GetAttLevel();
                    GetAutoAttLevel();
                    GetPreAmp();
                    SetWindowType(true);


                    res = true;
                    //IsRuning = true;
                    #endregion
                }
                else
                {
                    throw new Exception("The device is not Rohde&Schwarz FPL");
                }
            }
            else
            {
                throw new Exception("The device is not Rohde&Schwarz");
            }
            return res;
        }


        #region Get Data
        private void MeasurementInitialization()
        {
            WriteString(":INIT;*WAI");
        }
        private bool GetNumberOfSweeps()
        {
            numberOfSweeps = QueryULong(":SWE:COUN:CURR?");

            string pow = QueryString(":STAT:QUES:POW?");
            int pr = int.Parse(pow);
            if (pr == 0 || pr == 2) { PowerRegister = EN.PowerRegister.Normal; }
            else if (pr == 4) { PowerRegister = EN.PowerRegister.IFOverload; }
            else if (pr == 1) { PowerRegister = EN.PowerRegister.RFOverload; }//правильно
            return numberOfSweeps >= traceCountToMeas;
        }
        byte[] byteArray = new byte[400004];
        float[] leveltemp = new float[500005];
        private bool GetTrace()
        {
            bool res = true;
            try
            {
                bool newdata = true;
                //float[] temp = new float[0] { };



                WriteString(":TRAC:DATA? TRACE1");//;:STAT:QUES:POW?");
                //formattedIO.ReadBinaryBlockOfByte(byteArray, 0, SweepPoints * 4);
                formattedIO.ReadBinaryBlockOfByte(byteArray, 0, SweepPoints * 4 + 10);
                System.Diagnostics.Debug.WriteLine("GetTrace  " + new TimeSpan(timeService.TimeStamp.Ticks - t).ToString());
                //byte[] byteArray = formattedIO.ReadBinaryBlockOfByte(); //ReadByte();
                SweepTime = QueryDecimal(":SWE:TIME?");
                string pow = "";// QueryString(":STAT:QUES:POW?"); //formattedIO.ReadString(); //ReadString();
                System.Diagnostics.Debug.WriteLine("GetTrace1 " + new TimeSpan(timeService.TimeStamp.Ticks - t).ToString());
                //int pr = int.Parse(pow);
                if (pow.Contains("0") || pow.Contains("2")) { PowerRegister = EN.PowerRegister.Normal; }
                else if (pow.Contains("4")) { PowerRegister = EN.PowerRegister.IFOverload; }
                else if (pow.Contains("1")) { PowerRegister = EN.PowerRegister.RFOverload; }//правильно

                //temp = new float[SweepPoints];
                for (int j = 0; j < SweepPoints; j++)
                {
                    leveltemp[j] = System.BitConverter.ToSingle(byteArray, j * 4);
                }


                if (leveltemp.Length > 0)
                {
                    int k = 0;

                    //т.к. используем только ClearWhrite то проверяем полученный трейс с предыдущим временным на предмет полного отличия и полноценен он или нет
                    if (!float.IsNaN(leveltemp[0]) && leveltemp[0] % 1 != 0 &&
                        !float.IsNaN(leveltemp[SweepPoints - 1]) && leveltemp[SweepPoints - 1] % 1 != 0)
                    {
                        int len = (SweepPoints - 5) / 5 - 20;
                        for (int i = 0; i < len; i += 3)
                        {
                            if (LevelArr[i * 5] == leveltemp[i * 5] &&
                                LevelArr[i * 5 + 1] == leveltemp[i * 5 + 1] &&
                                LevelArr[i * 5 + 2] == leveltemp[i * 5 + 2] &&
                                LevelArr[i * 5 + 3] == leveltemp[i * 5 + 3] &&
                                LevelArr[i * 5 + 4] == leveltemp[i * 5 + 4] &&
                                LevelArr[i * 5 + 5] == leveltemp[i * 5 + 5])
                            {
                                k++;
                                if (k > SweepPoints / 20)
                                {
                                    newdata = false;
                                    //k = i;
                                    break;
                                }
                            }
                        }
                    }
                    else
                    {
                        //System.Diagnostics.Debug.Write(" => bad data in First point " + leveltemp[0].ToString());
                        //System.Diagnostics.Debug.Write(" => bad data in Last point " + leveltemp[SweepPoints - 1].ToString());
                        //System.Diagnostics.Debug.WriteLine("bad data " + k);
                        newdata = false;
                    }

                    //таки новый трейс полностью
                    if (newdata)
                    {
                        if (getSweeptime)
                        {
                            //SweepTime = QueryDecimal(":SWE:TIME?");
                            getSweeptime = false;

                        }
                        for (int l = 0; l < SweepPoints; l++)
                        {
                            LevelArr[l] = leveltemp[l];
                            //levelArrTemp = temp;
                        }

                    }
                    else
                    {
                        //System.Diagnostics.Debug.Write(" => trace data is not 5% unique");
                        res = false;
                    }
                    //System.Diagnostics.Debug.Write("\r\n");
                    //System.Diagnostics.Debug.WriteLine("kkkkkk " + k);

                }
                else
                {
                    res = false;
                }
            }
            #region Exception
            catch (Ivi.Visa.VisaException v_exp)
            {
                res = false;
                logger.Exception(Contexts.ThisComponent, v_exp);
            }
            catch (Exception exp)
            {
                res = false;
                logger.Exception(Contexts.ThisComponent, exp);
            }
            #endregion
            return res;
        }
        private bool GetTraceWithAggregation(decimal freqStart, decimal freqStop, int steps, int actualSweepPoints)
        {
            bool res = true;
            try
            {
                decimal step = (freqStop - freqStart) / steps;

                for (int i = 0; i < steps; i++)
                {
                    bool newdata = true;
                    WriteString(":SENSe:FREQ:STAR " + (FreqStart + step * i).ToString().Replace(decimalSeparator, ".") + ";:" +
                        "SENSe:FREQ:STOP " + (FreqStart + step * (i + 1)).ToString().Replace(decimalSeparator, ".") + ";:" +
                        "INIT;*WAI;:TRAC:DATA? TRACE1");//;:STAT:QUES:POW?");
                    formattedIO.ReadBinaryBlockOfByte(byteArray, 0, SweepPoints * 4 + 10);
                    //SweepTime = QueryDecimal(":SWE:TIME?");
                    string pow = QueryString(":STAT:QUES:POW?"); //formattedIO.ReadString(); //ReadString();
                                                                 //int pr = int.Parse(pow);
                    if (pow.Contains("0") || pow.Contains("2")) { PowerRegister = EN.PowerRegister.Normal; }
                    else if (pow.Contains("4")) { PowerRegister = EN.PowerRegister.IFOverload; }
                    else if (pow.Contains("1")) { PowerRegister = EN.PowerRegister.RFOverload; }//правильно


                    for (int j = 0; j < SweepPoints; j++)
                    {
                        leveltemp[j + actualSweepPoints * i] = System.BitConverter.ToSingle(byteArray, j * 4);
                    }

                }
                SetTraceDataWithAggregation(leveltemp, steps * actualSweepPoints);
            }
            #region Exception
            catch (Ivi.Visa.VisaException v_exp)
            {
                res = false;
                logger.Exception(Contexts.ThisComponent, v_exp);
            }
            catch (Exception exp)
            {
                res = false;
                logger.Exception(Contexts.ThisComponent, exp);
            }
            #endregion
            return res;
        }
        private bool GetTraceOLD()
        {
            bool res = true;
            try
            {
                bool newdata = true;
                //float[] temp = new float[0] { };



                WriteString(":TRAC:DATA? TRACE1");//;:STAT:QUES:POW?");
                //formattedIO.ReadBinaryBlockOfByte(byteArray, 0, SweepPoints * 4);
                formattedIO.ReadBinaryBlockOfByte(byteArray, 0, SweepPoints * 4 + 10);
                System.Diagnostics.Debug.WriteLine("GetTrace  " + new TimeSpan(timeService.TimeStamp.Ticks - t).ToString());
                //byte[] byteArray = formattedIO.ReadBinaryBlockOfByte(); //ReadByte();
                SweepTime = QueryDecimal(":SWE:TIME?");
                string pow = "";// QueryString(":STAT:QUES:POW?"); //formattedIO.ReadString(); //ReadString();
                System.Diagnostics.Debug.WriteLine("GetTrace1 " + new TimeSpan(timeService.TimeStamp.Ticks - t).ToString());
                //int pr = int.Parse(pow);
                if (pow.Contains("0") || pow.Contains("2")) { PowerRegister = EN.PowerRegister.Normal; }
                else if (pow.Contains("4")) { PowerRegister = EN.PowerRegister.IFOverload; }
                else if (pow.Contains("1")) { PowerRegister = EN.PowerRegister.RFOverload; }//правильно

                //temp = new float[SweepPoints];
                for (int j = 0; j < SweepPoints; j++)
                {
                    leveltemp[j] = System.BitConverter.ToSingle(byteArray, j * 4);
                }


                if (leveltemp.Length > 0)
                {
                    int k = 0;

                    //т.к. используем только ClearWhrite то проверяем полученный трейс с предыдущим временным на предмет полного отличия и полноценен он или нет
                    if (!float.IsNaN(leveltemp[0]) && leveltemp[0] % 1 != 0 &&
                        !float.IsNaN(leveltemp[SweepPoints - 1]) && leveltemp[SweepPoints - 1] % 1 != 0)
                    {
                        int len = (SweepPoints - 5) / 5 - 20;
                        for (int i = 0; i < len; i += 3)
                        {
                            if (LevelArr[i * 5] == leveltemp[i * 5] &&
                                LevelArr[i * 5 + 1] == leveltemp[i * 5 + 1] &&
                                LevelArr[i * 5 + 2] == leveltemp[i * 5 + 2] &&
                                LevelArr[i * 5 + 3] == leveltemp[i * 5 + 3] &&
                                LevelArr[i * 5 + 4] == leveltemp[i * 5 + 4] &&
                                LevelArr[i * 5 + 5] == leveltemp[i * 5 + 5])
                            {
                                k++;
                                if (k > SweepPoints / 20)
                                {
                                    newdata = false;
                                    //k = i;
                                    break;
                                }
                            }
                        }
                    }
                    else
                    {
                        //System.Diagnostics.Debug.Write(" => bad data in First point " + leveltemp[0].ToString());
                        //System.Diagnostics.Debug.Write(" => bad data in Last point " + leveltemp[SweepPoints - 1].ToString());
                        //System.Diagnostics.Debug.WriteLine("bad data " + k);
                        newdata = false;
                    }

                    //таки новый трейс полностью
                    if (newdata)
                    {
                        if (getSweeptime)
                        {
                            //SweepTime = QueryDecimal(":SWE:TIME?");
                            getSweeptime = false;

                        }
                        for (int l = 0; l < SweepPoints; l++)
                        {
                            LevelArr[l] = leveltemp[l];
                            //levelArrTemp = temp;
                        }

                    }
                    else
                    {
                        //System.Diagnostics.Debug.Write(" => trace data is not 5% unique");
                        res = false;
                    }
                    //System.Diagnostics.Debug.Write("\r\n");
                    //System.Diagnostics.Debug.WriteLine("kkkkkk " + k);

                }
                else
                {
                    res = false;
                }
            }
            #region Exception
            catch (Ivi.Visa.VisaException v_exp)
            {
                res = false;
                logger.Exception(Contexts.ThisComponent, v_exp);
            }
            catch (Exception exp)
            {
                res = false;
                logger.Exception(Contexts.ThisComponent, exp);
            }
            #endregion
            return res;
        }

        private void SetTraceDataWithAggregation(float[] trace, int points)
        {
            if (trace != null && trace.Length > 0)
            {
                if (LevelArr.Length != trace.Length)
                {
                    LevelArr = new float[trace.Length];
                    for (int i = 0; i < trace.Length; i++)
                    {
                        LevelArr[i] = trace[i];
                    }
                }
                if (traceTypeResult == EN.TraceType.ClearWrite)
                {
                    LevelArr = trace;
                }
                else if (traceTypeResult == EN.TraceType.Average)//Average
                {
                    if (traceReset) { traceAveraged.Reset(); traceReset = false; }
                    traceAveraged.AddTraceToAverade(resFreqStart, resFreqStep, trace, (MEN.LevelUnit)LevelUnit.Id, levelUnitResult);
                    LevelArr = traceAveraged.AveragedLevels;
                }
                else if (traceTypeResult == EN.TraceType.MaxHold)
                {
                    if (traceReset)
                    {
                        LevelArr = trace;
                        traceReset = false;
                    }
                    else
                    {
                        for (int i = 0; i < trace.Length; i++)
                        {
                            if (trace[i] > LevelArr[i]) LevelArr[i] = trace[i];
                        }
                    }
                }
                else if (traceTypeResult == EN.TraceType.MinHold)
                {
                    if (traceReset)
                    {
                        LevelArr = trace;
                        traceReset = false;
                    }
                    else
                    {
                        for (int i = 0; i < trace.Length; i++)
                        {
                            if (trace[i] < LevelArr[i]) LevelArr[i] = trace[i];
                        }
                    }
                }
                //LevelArrTemp = trace;
            }
        }

        private bool GetIQStream(ref COMR.MesureIQStreamResult result, COM.MesureIQStreamCommand command)
        {
            bool res = false;

#if DEBUG
            long delta = timeService.TimeStamp.Ticks;
#endif
            long time = command.Parameter.TimeStart;// _timeService.GetGnssTime().Ticks;
            long NextSecond = (time / 10000000) * 10000000 + 10000000;
            long ToNextSecond = NextSecond - time;

            //IQMeasTime
            decimal delay = Math.Abs(((decimal)ToNextSecond) / 10000000);// - 0.03m;// - 0.5m; //время насколько раньше тригерра будут собранны данные всегда отрицательное
                                                                         //ищем ближайшее целое по отношени к длительности семпла
            int divisor = -1 + (int)Math.Floor((0 - delay) / SampleTimeLength);


            IQMeasTimeAll = (decimal)command.Parameter.IQReceivTime_s;
            IQMeasTime = (decimal)command.Parameter.IQBlockDuration_s;
            sampleLength = (int)(sampleSpeed * IQMeasTimeAll);
            SetTriggerOffsetAndSampleLengthAndStartCollection(divisor * SampleTimeLength, (int)(sampleSpeed * IQMeasTimeAll));
#if DEBUG
            delta = timeService.TimeStamp.Ticks - delta;
#endif
            if (command.Parameter.TimeStart < timeService.GetGnssUtcTime().Ticks - uTCOffset)
            {
                //Debug.WriteLine("ВСе плохо");
            }
            //Debug.WriteLine(new TimeSpan(command.Parameter.TimeStart + UTCOffset - _timeService.GetGnssUtcTime().Ticks).ToString());
            int sleep = (int)((IQMeasTimeAll + Math.Abs(divisor * SampleTimeLength)) * 1000);
            if (sleep < 1)
            {
                sleep = 1;
            }
            System.Threading.Thread.Sleep(sleep);



            int step = sampleLength / 10;
            if (step > 5000000)
            {
                step = 5000000;
            }
            else if (step < 5000000)
            {
                step = 5000000;
            }
#if DEBUG
            long delta2 = timeService.TimeStamp.Ticks;
#endif
            float[] temp = new float[sampleLength * 2];
            long ddd = timeService.TimeStamp.Ticks;
            int pos = 0;
            for (int i = 0; i < sampleLength; i += step)
            {
                int from = i, to = i + step;
                if (to > sampleLength) to = sampleLength;
                WriteString($"TRAC:IQ:DATA:MEM? {from},{to - from}");


                byte[] byteArray = session.FormattedIO.ReadBinaryBlockOfByte();// ReadByte();

                float[] temp2 = new float[byteArray.Length / 4];

                for (int j = 0; j < temp2.Length; j++)
                {
                    temp2[j] = System.BitConverter.ToSingle(byteArray, j * 4);
                }
                Array.Copy(temp2, 0, temp, pos, temp2.Length);
                pos += temp2.Length;

            }
#if DEBUG
            delta2 = timeService.TimeStamp.Ticks - delta2;
            System.Diagnostics.Debug.WriteLine("delta " + (new TimeSpan(delta)).ToString());
            System.Diagnostics.Debug.WriteLine("delta2 " + (new TimeSpan(delta2)).ToString());
            System.Diagnostics.Debug.WriteLine("delta++ " + (new TimeSpan(delta2 + delta)).ToString());
#endif
            triggerOffsetInSample = QueryDecimal("TRACe:IQ:TPISample?");
            //Посчитаем когда точно был триггер относительно первого семпла
            TriggerOffset = Math.Abs(TriggerOffset) + triggerOffsetInSample;



            float noise = 1.5f / 10000000f; // уровень шума 
            float SN = 10; // превышение шума в разах 
            float TrigerLevelPL = noise * SN;
            float TrigerLevelMN = 0 - TrigerLevelPL;
            int IQStartIndex = 0;
            int IQStopIndex = temp.Length;
            bool SignalFound = false; //был ли сигнал
            int stepf = temp.Length / 1000;//шаг проверки уровней на предмет детектирования сигнала
            if (stepf < 1)
            {
                stepf = 1;
            }
            if (command.Parameter.MandatorySignal)
            {
                for (int j = 0; temp.Length - 6 > j; j += stepf)
                {
                    if (temp[j] >= TrigerLevelPL || temp[j + 1] >= TrigerLevelPL ||
                        temp[j] <= TrigerLevelMN || temp[j + 1] <= TrigerLevelMN)
                    {
                        if (temp[j + 2] >= TrigerLevelPL || temp[j + 3] >= TrigerLevelPL ||
                            temp[j + 2] <= TrigerLevelMN || temp[j + 3] <= TrigerLevelMN)
                        {
                            if (temp[j + 4] >= TrigerLevelPL || temp[j + 5] >= TrigerLevelPL ||
                                temp[j + 4] <= TrigerLevelMN || temp[j + 5] <= TrigerLevelMN)
                            {
                                SignalFound = true;//Есть сигнал 
                                IQStartIndex = j - (int)(((double)stepf) * 3.0);
                                if (IQStartIndex % 2 != 0) IQStartIndex = (IQStartIndex / 2) * 2 + 2;
                                if (IQStartIndex < 0)
                                    IQStartIndex = 0;

                                break;
                            }
                        }
                    }
                }
                if (!SignalFound) //Хотели сигнал но его небыло, согласно договоренности генерируем екзепшен
                {
                    throw new Exception("Signal not detected");
                }
            }

            int dddddddd = 2 * (int)Math.Ceiling(((decimal)command.Parameter.IQBlockDuration_s) / SampleTimeLength);
            if (dddddddd % 2 != 0) dddddddd = (dddddddd / 2) * 2 + 2;
            IQStopIndex = IQStartIndex + dddddddd;
            if (IQStopIndex > temp.Length) IQStopIndex = temp.Length;
            result.OneSempleDuration_ns = (long)(SampleTimeLength * 1000000000);
            result.PPSTimeDifference_ns = (long)(TriggerOffset * 1000000000 - IQStartIndex * SampleTimeLength * 500000000);
            result.TimeStamp = (NextSecond * 100 - result.PPSTimeDifference_ns) / 100;
            TriggerOffset = ((decimal)result.PPSTimeDifference_ns) / 1000000000;
            result.iq_samples = new float[1][];
            result.iq_samples[0] = new float[IQStopIndex - IQStartIndex];

            Array.Copy(temp, IQStartIndex, result.iq_samples[0], 0, IQStopIndex - IQStartIndex);
            //result.iq_samples[0] = temp;
            IQArr = result.iq_samples[0];

            //result.TimeStamp = ;
            //result.TimeStamp = tempIQStream.BlockTime[IQStartIndex] / 100;// надыбать время первого семпла
            //result.PPSTimeDifference_ns = TimeToStartBlockWithPPS;// когда был ппс точно относительно первого семпла



            res = true;

            return res;
        }
        #endregion Get Data

        #region AN To Command
        private void GetFreqCentr()
        {

            FreqCentr = QueryDecimal(":SENSe:FREQuency:CENTer?");

            GetFreqArr();
        }
        private void GetFreqSpan()
        {
            FreqSpan = QueryDecimal(":SENSe:FREQuency:SPAN?");
            GetFreqArr();
        }

        private void GetFreqArr()
        {
            resFreqStep = Math.Round((double)FreqSpan / SweepPoints, 2);
            resFreqStart = Math.Round((double)FreqStart + resFreqStep / 2, 5);
            LevelArr = new float[SweepPoints];
            levelArrTemp = new float[SweepPoints];
            for (int i = 0; i < SweepPoints; i++)
            {
                LevelArr[i] = -1000;
                levelArrTemp[i] = -1000;
            }

            //byteArray = QueryByte("TRAC1:X? TRACE1");
            //double stepsum = 0;
            //double[] temp = new double[byteArray.Length / 4];

            //float d1 = 0, d2 = 0;
            //d1 = System.BitConverter.ToSingle(byteArray, 0);
            //for (int j = 1; j < byteArray.Length / 4; j++)
            //{
            //    d2 = System.BitConverter.ToSingle(byteArray, j * 4);
            //    stepsum += d2 - d1 ;
            //    d1 = d2;
            //}
            //resFreqStep = stepsum / (byteArray.Length / 4 - 1);
            //resFreqStart = System.BitConverter.ToSingle(byteArray, 0);


            //temp[0] = System.BitConverter.ToSingle(byteArray, 0);

            //for (int j = 1; j < byteArray.Length / 4; j++)
            //{
            //    temp[j] = System.BitConverter.ToSingle(byteArray, j * 4);
            //    stepsum += temp[j] - temp[j - 1];
            //}
            ////resFreqStep = stepsum / (temp.Length - 1);
            ////resFreqStart = temp[0];
            //LevelArr = new float[byteArray.Length / 4];
            //levelArrTemp = new float[byteArray.Length / 4];
            //for (int i = 0; i < byteArray.Length / 4; i++)
            //{
            //    LevelArr[i] = -1000;
            //    levelArrTemp[i] = -1000;
            //}


            //decimal step = 0;
            //decimal[] FreqTemp = new decimal[SweepPoints + 2];
            //decimal[] Freq = new decimal[SweepPoints + 2];
            //step = (decimal)Math.Round((FreqSpan / SweepPoints), 2);
            //FreqTemp[0] = (decimal)(FreqCentr - FreqSpan / 2);
            //FreqTemp[1] = Math.Round(FreqTemp[0] + step / 2, 5);
            //FreqTemp[FreqTemp.Length - 1] = (decimal)(FreqCentr + FreqSpan / 2);
            //for (int i = 0; i < FreqTemp.Length; i++)
            //{
            //    if (i > 1) { FreqTemp[i] = Math.Round(FreqTemp[i - 1] + step, 5); }
            //    Freq[i] = Math.Round(FreqTemp[i] / 10, 0) * 10;
            //}
        }
        private void SetFreqCentrIQ(decimal freqCentrIQ)
        {
            this.freqCentrIQ = freqCentrIQ;
            WriteString($"FREQ:CENT {this.freqCentrIQ.ToString().Replace(decimalSeparator, ".")}");
            this.freqCentrIQ = QueryDecimal(":FREQ:CENT?");
        }

        private void GetAttLevel()
        {
            if (mode)
            {
                AttLevelSpec = QueryDecimal(":INP:ATT?");
            }
            else
            {
                AttLevelIQ = QueryDecimal(":INP:ATT?");
            }
        }

        private void GetAutoAttLevel()
        {
            if (mode)
            {
                string temp = "";
                temp = QueryString(":INP:ATT:AUTO?");
                if (temp.Contains("1"))
                {
                    attAutoSpec = true;
                }
                else if (temp.Contains("0"))
                {
                    attAutoSpec = false;
                }
            }
            else
            {
                string temp = "";
                temp = QueryString(":INP:ATT:AUTO?");
                if (temp.Contains("1"))
                {
                    attAutoIQ = true;
                }
                else if (temp.Contains("0"))
                {
                    attAutoIQ = false;
                }

            }
        }

        private void GetPreAmp()
        {
            if (mode)
            {
                if (preAmpAvailable)
                {
                    string temp = QueryString(":INP:GAIN:STAT?");
                    if (temp.Contains("1")) { preAmpSpec = true; }
                    else if (temp.Contains("0")) { preAmpSpec = false; }
                }
            }
            else
            {
                if (preAmpAvailable)
                {
                    string temp = QueryString(":INP:GAIN:STAT?");
                    if (temp.Contains("1")) { preAmpIQ = true; }
                    else if (temp.Contains("0")) { preAmpIQ = false; }
                }
            }
        }

        private void GetRefLevel()
        {
            if (mode)
            {
                RefLevelSpec = Math.Round(QueryDecimal(":DISPlay:WINDow:TRACe:Y:SCALe:RLEVel?"));
            }
            else
            {
                RefLevelIQ = Math.Round(QueryDecimal(":DISPlay:WINDow:TRACe:Y:SCALe:RLEVel?"));
            }
        }

        private void SetRange()
        {
            if (mode)
            {
                WriteString(":DISP:TRAC:Y " + RangeSpec.ToString().Replace(decimalSeparator, "."));
                if (attAutoSpec == true) { GetAttLevel(); }
            }
            else
            {
                WriteString(":DISP:TRAC:Y " + RangeIQ.ToString().Replace(decimalSeparator, "."));
                if (attAutoIQ == true) { GetAttLevel(); }
            }
        }

        private void GetRange()
        {
            if (mode)
            {
                RangeSpec = Math.Round(QueryDecimal(":DISP:TRAC:Y?"));
            }
            else
            {
                RangeIQ = Math.Round(QueryDecimal(":DISP:TRAC:Y?"));
            }
        }

        private void GetLevelUnit()
        {
            string temp = QueryString(":UNIT:POWer?");
            for (int i = 0; i < levelUnits.Count; i++)
            {
                if (temp.ToLower() == levelUnits[i].Parameter.ToLower())
                {
                    LevelUnit = levelUnits[i];
                }
            }
        }

        private void GetRBW()
        {
            RBW = QueryDecimal(":SENSe:BWIDth:RESolution?");
            RBWIndex = System.Array.IndexOf(RBWArr, RBW);
        }

        private void SetAutoRBW(bool autoRBV)
        {
            AutoRBW = autoRBV;
            if (AutoRBW)
            {
                WriteString(":SENSe:BWIDth:RESolution:AUTO 1");
            }
            else
            {
                WriteString(":SENSe:BWIDth:RESolution:AUTO 0");
            }

            if (AutoSweepTime == true) { GetSweepTime(); }
            GetRBW();
            GetVBW();
            GetSweepType();
        }
        private void GetAutoRBW()
        {
            string s = QueryString(":SENSe:BWIDth:RESolution:AUTO?");
            if (s.Contains("1"))
            {
                AutoRBW = true;
            }
            else
            {
                AutoRBW = false;
            }
            if (AutoSweepTime == true) { GetSweepTime(); }
        }

        private void GetVBW()
        {
            VBW = QueryDecimal(":SENSe:BANDwidth:VIDeo?");
            VBWIndex = System.Array.IndexOf(VBWArr, VBW);
        }

        private void SetAutoVBW(bool autoVBW)
        {
            AutoVBW = autoVBW;
            if (AutoVBW)
            {
                WriteString(":SENSe:BANDwidth:VIDeo:AUTO 1");
            }
            else
            {
                WriteString(":SENSe:BANDwidth:VIDeo:AUTO 0");
            }

            if (AutoVBW == true) { GetVBW(); }
            if (AutoSweepTime == true) { GetSweepTime(); }
            GetSweepType();
        }
        private void GetAutoVBW()
        {
            if (QueryString(":SENSe:BANDwidth:VIDeo:AUTO?").Contains("1"))
            {
                AutoVBW = true;
            }
            else
            {
                AutoVBW = false;
            }
        }

        private void SetSweepType()
        {
            WriteString(":SWE:TYPE " + SweepTypeSelected.Parameter);
        }
        private void GetSweepType()
        {
            string t = QueryString(":SWE:TYPE?");
            foreach (ParamWithId ST in SweepType)
            {
                if (t.TrimEnd().ToLower() == ST.Parameter.ToLower()) { SweepTypeSelected = ST; }
            }
        }

        private void GetSweepTime()
        {
            SweepTime = QueryDecimal(":SWE:TIME?");
        }

        private void GetAutoSweepTime()
        {
            if (QueryString(":SWE:TIME:AUTO?").Contains("0"))
            {
                AutoSweepTime = false;
            }
            else
            {
                AutoSweepTime = true;
            }
        }

        private void GetSweepPoints()
        {
            SweepPoints = QueryInt(":SWE:POIN?");
            sweepPointsIndex = System.Array.IndexOf(sweepPointArr, SweepPoints);
            GetFreqArr();
        }

        private void SetOptimization(EN.Optimization optimization)
        {
            Optimization = optimization;
            WriteString("SENSe:SWEep:OPTimize " + OptimizationSelected.Parameter);
        }
        private void GetOptimization()
        {
            string temp1 = string.Empty;
            temp1 = QueryString("SENSe:SWEep:OPTimize?");
            foreach (ParamWithId TT in optimization)
            {
                if (temp1.Contains(TT.Parameter))
                {
                    Optimization = (EN.Optimization)TT.Id;
                }
            }
        }

        private void GetTraceType()
        {
            string temp1 = string.Empty;
            if (QueryString(":DISP:TRAC1?").Contains("1"))
            {
                temp1 = QueryString(":DISP:TRAC1:MODE?");
            }
            else
            {
                temp1 = "BLAN";
            }
            foreach (ParamWithId TT in traceTypes)
            {
                if (temp1.Contains(TT.Parameter))
                {
                    trace1Type = TT;
                }
            }
        }

        private void GetDetectorType()
        {
            string temp1 = string.Empty;
            temp1 = QueryString(":SENSe:DET1?");

            foreach (ParamWithId TT in traceDetectors)
            {
                if (temp1.Contains(TT.Parameter)) { trace1Detector = TT; }
            }
        }

        private void SetIQBW(decimal iqbw)
        {
            IQBW = iqbw;
            sampleSpeed = 1.25m * IQBW;
            SampleTimeLength = 1 / sampleSpeed;
            string s = "TRAC:IQ:BWID " + IQBW.ToString().Replace(decimalSeparator, ".");
            WriteString(s);

        }
        private void SetSampleSpeed(decimal sampleSpeed)
        {
            this.sampleSpeed = sampleSpeed;
            IQBW = 0.8m * this.sampleSpeed;
            SampleTimeLength = 1 / this.sampleSpeed;
            WriteString("TRAC:IQ:SRAT " + this.sampleSpeed.ToString().Replace(decimalSeparator, "."));
        }
        private void SetSampleLength(int sampleLength)
        {
            this.sampleLength = sampleLength;
            WriteString("TRAC:IQ:RLEN " + this.sampleLength.ToString());
        }
        private void SetTriggerOffsetAndSampleLengthAndStartCollection(decimal triggeroffset, int samplelength)
        {
            TriggerOffset = triggeroffset;
            sampleLength = samplelength;

            if (Math.Abs(TriggerOffset) < triggerOffsetMax)
            {
                int length = sampleLength * 4 * 2 + sampleLength.ToString().Length + 100;
                //if (session.DefaultBufferSize != length)
                //{
                //    session.DefaultBufferSize = length;
                //}
                WriteString("TRIG:HOLD " + TriggerOffset.ToString().Replace(decimalSeparator, ".") + ";:TRAC:IQ:RLEN " + sampleLength.ToString() + ";:INIT;*WAI;");

                //session.Write("INIT;*WAI;");
            }

        }
        public void SetIQWindow()
        {
            string str = "INST:CRE IQ, 'IQ Analyzer'";
            WriteString(str);
            WriteString("INIT:CONT OFF");
            WriteString("TRIG:SOUR EXT");
            WriteString("TRACe:IQ:DATA:FORMat IQPair");
        }

        public void SetWindowType(bool mode)
        {
            this.mode = mode;
            string str = "";
            if (this.mode)
            {
                str = "INST SAN";
            }
            else
            {
                str = "INST IQ";
            }
            WriteString(str);
            WriteString("FORM:DATA REAL,32");//так надо
        }
        private void SetPreset()
        {
            WriteString(":SYSTem:PRES");
        }
        private void GetSetAnSysDateTime()
        {
            string[] d = QueryString("SYST:DATE?").TrimEnd().Trim(' ').Split(',');
            string[] t = QueryString("SYST:TIME?").TrimEnd().Trim(' ').Split(',');
            string time = d[0] + "-" + d[1] + "-" + d[2] + " " +
                t[0] + ":" + t[1] + ":" + t[2];
            DateTime andt = DateTime.Parse(time);
            if (new TimeSpan(DateTime.Now.Ticks - andt.Ticks) > new TimeSpan(0, 0, 1, 0, 0))
            {
                WriteString("SYST:DATE " + DateTime.Now.Year.ToString() + "," + DateTime.Now.Month.ToString() + "," + DateTime.Now.Day.ToString());
                WriteString("SYST:TIME " + DateTime.Now.Hour.ToString() + "," + DateTime.Now.Minute.ToString() + "," + DateTime.Now.Second.ToString());
            }
        }

        private void SetTransducer(CFG.AdapterMainConfig mainConfig)
        {
            WriteString("CORR:TRAN:SEL 'Control'");
            string str = "CORRection:TRANsducer:DATA ";
            for (int i = 0; i < mainConfig.AdapterRadioPathParameters.Length; i++)
            {
                str += mainConfig.AdapterRadioPathParameters[i].Freq.ToString() + "," +
                    (mainConfig.AdapterRadioPathParameters[i].Gain - mainConfig.AdapterRadioPathParameters[i].FeederLoss).ToString();
                if (i < mainConfig.AdapterRadioPathParameters.Length - 1)
                {
                    str += ",";
                }

            }
            WriteString(str);

            WriteString("CORR:TRAN ON");
        }
        #endregion AN To Command



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
            };
        }
        private (MesureTraceDeviceProperties, MesureIQStreamDeviceProperties) GetProperties(CFG.AdapterMainConfig config)
        {
            RadioPathParameters[] rrps = ConvertRadioPathParameters(config);
            StandardDeviceProperties sdp = new StandardDeviceProperties()
            {
                AttMax_dB = (int)attMax,
                AttMin_dB = 0,
                FreqMax_Hz = FreqMax,
                FreqMin_Hz = FreqMin,
                PreAmpMax_dB = 1, //типа включен/выключен, сколько по факту усиливает нигде не пишется кроме FSW где их два 15/30 и то два это опция
                PreAmpMin_dB = 0,
                RefLevelMax_dBm = (int)RefLevelMax,
                RefLevelMin_dBm = (int)RefLevelMin,
                EquipmentInfo = new EquipmentInfo()
                {
                    AntennaCode = config.AdapterEquipmentInfo.AntennaSN,// "Omni",//S/N  В конфиг
                    AntennaManufacturer = config.AdapterEquipmentInfo.AntennaManufacturer,//"3anet",//В конфиг
                    AntennaName = config.AdapterEquipmentInfo.AntennaName,//"BC600",//В конфиг
                    EquipmentManufacturer = new Atdi.DataModels.Sdrn.DeviceServer.Adapters.InstrManufacrures().RuS.UI,
                    EquipmentName = instrModel,
                    EquipmentFamily = "SpectrumAnalyzer",//SDR/SpecAn/MonRec
                    EquipmentCode = serialNumber,//S/N

                },
                RadioPathParameters = rrps
            };
            if (preAmpAvailable)
            {
                sdp.PreAmpMax_dB = 1;
            }
            else
            {
                sdp.PreAmpMax_dB = 0;
            }
            MesureTraceDeviceProperties mtdp = new MesureTraceDeviceProperties()
            {
                RBWMax_Hz = (double)RBWArr[RBWArr.Length - 1],
                RBWMin_Hz = (double)RBWArr[0],
                SweepTimeMin_s = (double)SWTMin,
                SweepTimeMax_s = (double)SWTMax,
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