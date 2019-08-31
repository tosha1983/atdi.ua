using Atdi.Contracts.Sdrn.DeviceServer;
using Atdi.DataModels.Sdrn.DeviceServer.Commands;
using Atdi.DataModels.Sdrn.DeviceServer.Commands.Results;
using Atdi.DataModels.Sdrn.DeviceServer.Processing;
using DM = Atdi.DataModels.Sdrns.Device;
using System;
using System.Linq;
using System.Collections.Generic;
using Atdi.DataModels.Sdrn.DeviceServer;
using Atdi.DataModels.Sdrns.Device;
using Atdi.DataModels.Sdrns.Device.OnlineMeasurement;
using Atdi.AppUnits.Sdrn.DeviceServer.OnlineMeasurement.Tasks;
using System.Collections;
using System.ComponentModel;



namespace Atdi.AppUnits.Sdrn.DeviceServer.OnlineMeasurement.Results
{
    public class LevelResultHandler : IResultHandler<MesureTraceCommand, MesureTraceResult, ClientReadyTakeMeasResultTask, OnlineMeasurementProcess>
    {
        private readonly AppServerComponentConfig _config;

        public LevelResultHandler(AppServerComponentConfig config)
        {
            this._config = config;
        }

        public void Handle(MesureTraceCommand command, MesureTraceResult result, DataModels.Sdrn.DeviceServer.ITaskContext<ClientReadyTakeMeasResultTask, OnlineMeasurementProcess> taskContext)
        {
            if (result != null)
            {
                try
                {
                    Atdi.DataModels.Sdrns.Device.OnlineMeasurement.TraceType traceType = TraceType.Unknown;
                    var levelResult = new DeviceServerResultLevel();
                    levelResult.Index = taskContext.Process.CountMeasurementDone;
                    switch (command.Parameter.TraceType)
                    {
                        case DataModels.Sdrn.DeviceServer.Commands.Parameters.TraceType.Auto:
                            traceType = TraceType.Auto;
                            break;
                        case DataModels.Sdrn.DeviceServer.Commands.Parameters.TraceType.Average:
                            traceType = TraceType.Average;
                            break;
                        case DataModels.Sdrn.DeviceServer.Commands.Parameters.TraceType.ClearWhrite:
                            traceType = TraceType.ClearWhrite;
                            break;
                        case DataModels.Sdrn.DeviceServer.Commands.Parameters.TraceType.MaxHold:
                            traceType = TraceType.MaxHold;
                            break;
                        case DataModels.Sdrn.DeviceServer.Commands.Parameters.TraceType.MinHold:
                            traceType = TraceType.MinHold;
                            break;
                        case DataModels.Sdrn.DeviceServer.Commands.Parameters.TraceType.Unknown:
                            traceType = TraceType.Unknown;
                            break;
                    }
                    

                    levelResult.Level = CutArray(result.Level, traceType, this._config.MaxCountPoint.Value);
                    if (taskContext.Process.CountMeasurementDone == 0) { levelResult.Freq_Hz = CutArray(result.Freq_Hz, this._config.MaxCountPoint.Value); }
                    levelResult.Att_dB = result.Att_dB;
                    levelResult.PreAmp_dB = result.PreAmp_dB;
                    levelResult.RBW_kHz = result.RBW_Hz / 1000.0;
                    levelResult.RefLevel_dBm = result.RefLevel_dBm;
                    if (result.DeviceStatus == DataModels.Sdrn.DeviceServer.Commands.Results.Enums.DeviceStatus.RFOverload) { levelResult.Overload = true; } else { levelResult.Overload = false; }
                    if (levelResult != null)
                    {
                        taskContext.SetEvent(levelResult);
                    }
                    else
                    {
                        taskContext.SetEvent<ExceptionProcessLevel>(new ExceptionProcessLevel(CommandFailureReason.Exception, new Exception("Level is null")));
                    }
                    taskContext.Process.CountMeasurementDone++;
                }
                catch (Exception ex)
                {
                    taskContext.SetEvent<ExceptionProcessLevel>(new ExceptionProcessLevel(CommandFailureReason.Exception, ex));
                }
            }
        }

        public static double[] CutArray(double[] arr, int CountPoint)
        {
            if (arr.Length <= CountPoint)
            {
                return arr;
            }
            else
            {
                var k = (int)Math.Round((double)(arr.Length / CountPoint));
                var reducedArray = new double[CountPoint];
                int reducedIndex = 0;
                for (int i = 0; i < arr.Length; i += k)
                {
                    if (reducedIndex > CountPoint - 1) break;
                    reducedArray[reducedIndex++] = arr[i];
                }
                return reducedArray;
            }
        }

        public static float[] CutArray(float[] arr, TraceType traceType, int CountPoint)
        {
            if (arr.Length <= CountPoint)
            {
                return arr;
            }
            else
            {
                var k = (int)Math.Round((double)(arr.Length / CountPoint));
                int newpoint = (int)Math.Ceiling((double)(arr.Length/k));
                var reducedArray = new float[newpoint];
                int reducedIndex = 0;
                for (int i = 0; i < arr.Length; i += k)
                {
                    if (reducedIndex > newpoint - 1) break;
                    float sum = 0;
                    int count = 0;
                    switch (traceType)
                    {
                        case TraceType.Auto:
                        case TraceType.ClearWhrite:
                        case TraceType.MaxHold:
                        case TraceType.Unknown:
                            //Max
                            {
                            
                                float temp = arr[i];
                                for (int j = 0; j < k && (i + j) < arr.Length; j++)
                                {
                                    if (temp < arr[i + j])
                                    {
                                        temp = arr[i + j];
                                    }
                                }
                                reducedArray[reducedIndex++] = temp;
                            }

                            break;
                        case TraceType.Average:
                            //Average
                            {
                                for (int j = 0; j < k && (i + j) < arr.Length; j++)
                                {
                                    sum += (float)Math.Pow(10.0,(double)(arr[i + j]/10.0));
                                    count++;
                                }
                                sum = 10 * (float)Math.Log10(sum);
                                reducedArray[reducedIndex++] = sum / count;
                            }
                            break;
                        case TraceType.MinHold:
                            //Min
                            {
                                float temp = arr[i];
                                for (int j = 0; j < k && (i + j) < arr.Length; j++)
                                {
                                    if (temp > arr[i + j])
                                    {
                                        temp = arr[i + j];
                                    }
                                }
                                reducedArray[reducedIndex++] = temp;
                            }
                            break;
                    }
                }
                return reducedArray;
            }
        }
    }
}
