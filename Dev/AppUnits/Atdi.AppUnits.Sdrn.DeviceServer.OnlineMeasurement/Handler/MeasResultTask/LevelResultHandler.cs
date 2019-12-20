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
using Atdi.Common;





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
                    //var result = CopyHelper.CreateDeepCopy(tempResult);

                    Atdi.DataModels.Sdrns.Device.OnlineMeasurement.TraceType traceType = TraceType.Unknown;
                    var levelResult = new DeviceServerResultLevel();
                    levelResult.Index = taskContext.Process.CountMeasurementDone;
                    levelResult.Time = DateTime.Now;
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
                        default:
                            throw new Exception($"Not supported type {command.Parameter.TraceType}");
                    }

                    Array.Clear(taskContext.Process.ReducedArray, 0, taskContext.Process.ReducedArray.Length);
                    levelResult.Level = CutArray(taskContext.Process.ReducedArray, result.Level, result.LevelMaxIndex+1, traceType, this._config.MaxCountPoint.Value);
                    if (taskContext.Process.LevelResult_dBm == null) { taskContext.Process.LevelResult_dBm = levelResult.Level; }
                    else
                    {
                        var levelTemp = levelResult.Level;
                        var LevelResult_dBm = taskContext.Process.LevelResult_dBm;
                        UnionArray(ref levelTemp, ref LevelResult_dBm, traceType);
                        levelResult.Level = levelTemp;
                        taskContext.Process.LevelResult_dBm = LevelResult_dBm;
                    }
                    if (result.DeviceStatus == DataModels.Sdrn.DeviceServer.Commands.Results.Enums.DeviceStatus.RFOverload) { levelResult.Overload = true; } else { levelResult.Overload = false; }
                    taskContext.SetEvent(levelResult);
                    taskContext.Process.CountMeasurementDone++;
                }
                catch (Exception ex)
                {
                    taskContext.SetEvent<ExceptionProcessLevel>(new ExceptionProcessLevel(CommandFailureReason.Exception, ex));
                }
            }
        }

        public static double SDRGainFromFrequency(MesureTraceDeviceProperties MesureTraceDeviceProperties, double Frequency_Hz)
        {
            // Константа с файла конфигурации
            double GainByDefault = 3;
            // Конец констант

            if ((Frequency_Hz < 0.009) || (Frequency_Hz > 400000) || (MesureTraceDeviceProperties == null) || (MesureTraceDeviceProperties.StandardDeviceProperties == null) ||
                (MesureTraceDeviceProperties.StandardDeviceProperties.RadioPathParameters == null)) { return GainByDefault; }
            if ((double)MesureTraceDeviceProperties.StandardDeviceProperties.RadioPathParameters[0].Freq_Hz <= Frequency_Hz)
            { return MesureTraceDeviceProperties.StandardDeviceProperties.RadioPathParameters[0].Gain; }
            if ((double)MesureTraceDeviceProperties.StandardDeviceProperties.RadioPathParameters[MesureTraceDeviceProperties.StandardDeviceProperties.RadioPathParameters.Length].Freq_Hz >= Frequency_Hz)
            { return MesureTraceDeviceProperties.StandardDeviceProperties.RadioPathParameters[MesureTraceDeviceProperties.StandardDeviceProperties.RadioPathParameters.Length].Gain; }
            for (int i = 0; i < MesureTraceDeviceProperties.StandardDeviceProperties.RadioPathParameters.Length - 1; i++)
            {
                if (((double)MesureTraceDeviceProperties.StandardDeviceProperties.RadioPathParameters[i].Freq_Hz <= Frequency_Hz) && ((double)MesureTraceDeviceProperties.StandardDeviceProperties.RadioPathParameters[i + 1].Freq_Hz >= Frequency_Hz))
                {
                    double G = MesureTraceDeviceProperties.StandardDeviceProperties.RadioPathParameters[i].Gain +
                        (Frequency_Hz - (double)MesureTraceDeviceProperties.StandardDeviceProperties.RadioPathParameters[i].Freq_Hz) *
                        (MesureTraceDeviceProperties.StandardDeviceProperties.RadioPathParameters[i + 1].Gain - MesureTraceDeviceProperties.StandardDeviceProperties.RadioPathParameters[i].Gain) /
                        ((double)MesureTraceDeviceProperties.StandardDeviceProperties.RadioPathParameters[i + 1].Freq_Hz - (double)MesureTraceDeviceProperties.StandardDeviceProperties.RadioPathParameters[i].Freq_Hz);
                    return G;
                }
            }
            return GainByDefault;
        }

        public static void UnionArray(ref float[] CurLevel, ref float[] LevelBufer, TraceType traceType)
        {
            if (!((CurLevel.Length != LevelBufer.Length) || (traceType == TraceType.Auto) || (traceType == TraceType.Average) || (traceType == TraceType.ClearWhrite) || (traceType == TraceType.Unknown)))
            {
                if (traceType == TraceType.MaxHold)
                {
                    for (int i = 0; CurLevel.Length > i; i++)
                    {
                        if (LevelBufer[i] > CurLevel[i]) { CurLevel[i] = LevelBufer[i]; }
                    }
                }
                if (traceType == TraceType.MinHold)
                {
                    for (int i = 0; CurLevel.Length > i; i++)
                    {
                        if (LevelBufer[i] < CurLevel[i]) { CurLevel[i] = LevelBufer[i]; }
                    }
                }
            }
            LevelBufer = CurLevel;
            
        }
        public static double CalcPow(float[] Levels, MesureTraceDeviceProperties MeasTraceDeviceProperties, double Freq_Hz)
        {
            if ((Levels == null)||(Levels.Length < 1)) { return -999;}
            double pow = 0;
            for (int i = 0;  Levels.Length>i; i++)
            {
                pow = pow + Math.Pow(10,Levels[i]);
            }
            pow = 10 * Math.Log10(pow);
            return pow;
        }

        public static float[] CutArray(float[] reducedArray, float[] arr, int length, TraceType traceType, int CountPoint)
        {
            if (arr.Length <= CountPoint)
            {
                return arr;
            }
            else
            {
                var k = (int)Math.Round((double)(length / CountPoint));
                if (k==0)
                {
                    k = 1;
                }
                int newpoint = (int)Math.Ceiling((double)(length / k));
                int reducedIndex = 0;
                for (var i = 0; i < length; i += k)
                {
                    if (reducedIndex > newpoint - 1) break;
                    if (reducedIndex > reducedArray.Length - 1) break;
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
                                for (int j = 0; j < k && (i + j) < length; j++)
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
                                for (int j = 0; j < k && (i + j) < length; j++)
                                {
                                    sum += (float)Math.Pow(10.0, (double)(arr[i + j] / 10.0));
                                    count++;
                                }
                                sum = 10 * (float)Math.Log10(sum / count);
                                reducedArray[reducedIndex++] = sum;
                            }
                            break;
                        case TraceType.MinHold:
                            //Min
                            {
                                float temp = arr[i];
                                for (int j = 0; j < k && (i + j) < length; j++)
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
