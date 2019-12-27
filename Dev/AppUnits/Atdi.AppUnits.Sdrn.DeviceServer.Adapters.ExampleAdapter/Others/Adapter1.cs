//using Atdi.Contracts.Sdrn.DeviceServer;
//using Atdi.DataModels.Sdrn.DeviceServer;
//using Atdi.DataModels.Sdrn.DeviceServer.Commands;
//using Atdi.Platform.Logging;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading;
//using System.Threading.Tasks;

//namespace Atdi.AppUnits.Sdrn.DeviceServer.Adapters.ExampleAdapter
//{
//    public class Adapter1 : IAdapter
//    {
//        private readonly ILogger _logger;
//        private readonly AdapterConfig _adapterConfig;

//        public Adapter1(AdapterConfig adapterConfig, ILogger logger)
//        {
//            this._logger = logger;
//            this._adapterConfig = adapterConfig;

//            this._logger.Debug(Contexts.Adapter1, Categories.Ctor, Events.Call);
//        }

//        public void Connect(IAdapterHost host)
//        {
//            this._logger.Debug(Contexts.Adapter1, Categories.Connect, Events.Call);

//            var propertiers = new MesureDFDeviceProperties
//            {
                
//            };

//            host.RegisterHandler<TestCommand1, Adapter1Result>(this.TestCommand1Handler);
//        }

//        public void Disconnect()
//        {
//            this._logger.Debug(Contexts.Adapter1, Categories.Disconnect, Events.Call);
//        }

//        public void TestCommand1Handler(TestCommand1 command, IExecutionContext context)
//        {
//            this._logger.Debug(Contexts.Adapter1, Categories.Handle, Events.HandleCommand.With(command.GetType().Name));

//            try
//            {
//                for (int i = 0; i < command.Parameter.Count; i++)
//                {
//                    var status = (i < (command.Parameter.Count - 1)) ? CommandResultStatus.Next : CommandResultStatus.Final;

//                    var isCancelled = context.Token.IsCancellationRequested;
//                    if (isCancelled)
//                    {
//                        status = CommandResultStatus.Final;
//                    }

//                    var adapterResult = new Adapter1Result((ulong)i, status);
//                    adapterResult.Value = (float)Math.PI;

//                    context.PushResult(adapterResult);

//                    if (isCancelled)
//                    {
//                        context.Cancel();
//                        return;
//                    }

//                    Thread.Sleep(command.Parameter.Delay);
//                }

//                context.Finish();
//            }
//            catch (Exception e)
//            {
//                context.Abort(e);
//            }
//        }
//    }
//}
