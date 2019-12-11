using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Atdi.Contracts.Sdrn.DeviceServer;
using Atdi.DataModels.Sdrn.DeviceServer;
using Atdi.DataModels.Sdrn.DeviceServer.Commands;
using Atdi.DataModels.Sdrn.DeviceServer.TestCommands;
using Atdi.Platform.Logging;

namespace Atdi.AppUnits.Sdrn.DeviceServer.Adapters.ExampleAdapter
{
    public class TraceAdapter : IAdapter
    {
        private static readonly Random DataMaker = new Random();

        private static readonly float[] BigFloatArray = new float[10_000_000];
        private static readonly double[] BigDoubleArray = new double[10_000_000];

        private readonly ILogger _logger;
        private readonly AdapterConfig _adapterConfig;

        public TraceAdapter(AdapterConfig adapterConfig, ILogger logger)
        {
            this._logger = logger;
            this._adapterConfig = adapterConfig;

            this._logger.Debug(Contexts.TraceAdapter, Categories.Ctor, Events.Call);
        }

        public void Connect(IAdapterHost host)
        {
            this._logger.Debug(Contexts.TraceAdapter, Categories.Connect, Events.Call);

            host.RegisterHandler<TraceCommand, TraceCommandResult>(this.TraceCommandHandler,
                new IResultPoolDescriptor<TraceCommandResult>[]
                {
                    new ResultPoolDescriptor<TraceCommandResult>()
                    {
                        Key = "small",
                        MinSize = 50,
                        MaxSize = 60,
                        Factory = () =>
                        {
                            var result = new TraceCommandResult
                            {
                                ValueAsFloats = new float[1_000],
                                ValuesAsDouble = new double[1_000]
                            };
                            return result;
                        }
                    },
                    new ResultPoolDescriptor<TraceCommandResult>()
                    {
                        Key = "middle",
                        MinSize = 20,
                        MaxSize = 40,
                        Factory = () =>
                        {
                            var result = new TraceCommandResult
                            {
                                ValueAsFloats = new float[10_000],
                                ValuesAsDouble = new double[10_000]
                            };
                            return result;
                        }
                    },
                    new ResultPoolDescriptor<TraceCommandResult>()
                    {
                        Key = "huge",
                        MinSize = 10,
                        MaxSize = 15,
                        Factory = () =>
                        {
                            var result = new TraceCommandResult
                            {
                                ValueAsFloats = new float[1_000_000],
                                ValuesAsDouble = new double[1_000_000]
                            };
                            return result;
                        }
                    }
                });
        }

        public void Disconnect()
        {
            this._logger.Debug(Contexts.TraceAdapter, Categories.Disconnect, Events.Call);
        }

        public void TraceCommandHandler(TraceCommand command, IExecutionContext context)
        {
            //this._logger.Debug(Contexts.TraceAdapter, Categories.Handle, Events.HandleCommand.With(command.GetType().Name));

            try
            {
                TraceCommandResult result = null;

                if (command.BlockSize <= 1_000)
                {
                    result = context.TakeResult<TraceCommandResult>("small", 0, CommandResultStatus.Final);
                }
                else if (command.BlockSize <= 10_000)
                {
                    result = context.TakeResult<TraceCommandResult>("middle", 0, CommandResultStatus.Final);
                }
                else if (command.BlockSize <= 1_000_000)
                {
                    result = context.TakeResult<TraceCommandResult>("huge", 0, CommandResultStatus.Final);
                }
                else
                {
                    throw new InvalidOperationException("Invalid the block size");
                }
                //Thread.Sleep(3);
                if (false)
                {
                    context.ReleaseResult(result);
                }

                //result.ValueAsFloats = GenerateFloatArray(command.BlockSize);
                //result.ValuesAsDouble = GenerateDoubleArray(command.BlockSize);

                // теперь так нельзя - есть пулл, исмпользуем  context.TakeResult

                // заполняем данными масивы
                //FillFloatArray(result.ValueAsFloats, command.BlockSize);
                //FillDoubleArray(result.ValuesAsDouble, command.BlockSize);
                //Thread.Sleep(20);
                //var result = new TraceCommandResult(0, CommandResultStatus.Final)
                //{
                //    ValueAsFloats = GenerateFloatArray(command.BlockSize),
                //    ValuesAsDouble = GenerateDoubleArray(command.BlockSize),
                //    Id = command.Id
                //};

                // стандартная отправк арезультата - тут ничего не меняется
                context.PushResult(result);
                context.Finish();
            }
            catch (Exception e)
            {
                this._logger.Exception(Contexts.TraceAdapter, Categories.Handle, e, this);
                context.Abort(e);
                
            }
        }
        public static void FillFloatArray(float[] block, int blockSize)
        {

            for (var i = 0; i < blockSize; i++)
            {
                block[i] = float.MaxValue;// (float)DataMaker.NextDouble();
            }

        }
        public static void FillDoubleArray(double[] block, int blockSize)
        {

            for (var i = 0; i < blockSize; i++)
            {
                block[i] = double.MaxValue;// DataMaker.NextDouble();
            }

        }
        public static float[] GenerateFloatArray(int blockSize)
        {

            var block = BigFloatArray; //new float[blockSize];
            for (var i = 0; i < blockSize; i++)
            {
                block[i] = float.MaxValue;// (float)DataMaker.NextDouble();
            }
            return block;
        }
        public static double[] GenerateDoubleArray(int blockSize)
        {

            var block = BigDoubleArray;// new double[blockSize];
            for (var i = 0; i < blockSize; i++)
            {
                block[i] = double.MaxValue;// DataMaker.NextDouble();
            }
            return block;
        }
    }
}
