using Atdi.Contracts.Sdrn.DeviceServer;
using Atdi.DataModels.Sdrn.DeviceServer;
using Atdi.DataModels.Sdrn.DeviceServer.Commands;
using Atdi.Platform.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Atdi.AppUnits.Sdrn.DeviceServer.Adapters.ExampleAdapter
{
    public class Adapter2 : IAdapter
    {
        private readonly ILogger _logger;
        private readonly AdapterConfig _adapterConfig;

        public Adapter2(AdapterConfig adapterConfig, ILogger logger)
        {
            this._logger = logger;
            this._adapterConfig = adapterConfig;

            this._logger.Debug(Contexts.Adapter2, Categories.Ctor, Events.Call);
        }

        public void Connect(IAdapterHost host)
        {
            this._logger.Debug(Contexts.Adapter2, Categories.Connect, Events.Call);

            var properties = new MesureDFDeviceProperties
            {
                
            };

            host.RegisterHandler<TestCommand2, Adapter2Result>(this.TestCommand2Handler, properties);
        }

        public void Disconnect()
        {
            this._logger.Debug(Contexts.Adapter2, Categories.Disconnect, Events.Call);
        }

        public void TestCommand2Handler(TestCommand2 command, IExecutionContext context)
        {
            context.Finish();
            return;

            this._logger.Debug(Contexts.Adapter2, Categories.Handle, Events.HandleCommand.With(command.GetType().Name));

            try
            {

                var predel = command.Parameter.Predel;
                for (int i = 0; i < command.Parameter.Count; i++)
                {
                    var status = (i < (command.Parameter.Count - 1)) ? CommandResultStatus.Next : CommandResultStatus.Final;

                    var isCancelled = context.Token.IsCancellationRequested;
                    if (isCancelled)
                    {
                        status = CommandResultStatus.Final;
                    }

                    var adapterResult = new Adapter2Result((ulong)0, CommandResultStatus.Next);
                    adapterResult.Value = (float)Math.PI;

                    //if (i <= predel)
                    //{
                    //    context.PushResult(adapterResult);
                    //}
                    

                    if (isCancelled)
                    {
                        context.Cancel();
                        return;
                    }

                    //Thread.Sleep(command.Parameter.Delay);
                }

                context.Finish();
            }
            catch (Exception e)
            {
                context.Abort(e);
            }
        }
    }
}
