using Atdi.DataModels.Sdrn.DeviceServer;
using Atdi.Platform.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Atdi.UnitTest.Sdrn.DeviceServer
{
    public class DummyExecutionContext : IExecutionContext
    {
        private readonly ILogger _logger;

        public DummyExecutionContext(ILogger logger)
        {
            this._logger = logger;
        }

        public CancellationToken Token { get; set; }

        public void Abort(Exception e)
        {
            this._logger.Verbouse("DummyExecutionContext", "Call method", $"Abort");
        }

        public void Cancel()
        {
            this._logger.Verbouse("DummyExecutionContext", "Call method", $"Cancel");
        }

        public void Finish()
        {
            this._logger.Verbouse("DummyExecutionContext", "Call method", $"Finish");
        }

        public void Lock(params CommandType[] types)
        {
            this._logger.Verbouse("DummyExecutionContext", "Call method", $"Lock");
        }

        public void Lock(params Type[] commandType)
        {
            this._logger.Verbouse("DummyExecutionContext", "Call method", $"Lock");
        }

        public void Lock()
        {
            this._logger.Verbouse("DummyExecutionContext", "Call method", $"Lock");
        }

        public void PushResult(ICommandResultPart result)
        {
            this._logger.Verbouse("DummyExecutionContext", "Call method", $"PushResult");

            //Task.Run(() =>
            //{
            //    var myResult = result as MyResultObject

            //    for (int i = 0; i < length; i++)
            //    {
            //        var data = myResult.myProperty[i];
            //        if (data > 10000)
            //        {
            //            this._logger.Critical("Test", "Result", $"Не соотвествие данных");
            //        }
            //    }
            //});
        }

        public void Unlock(params CommandType[] types)
        {
            this._logger.Verbouse("DummyExecutionContext", "Call method", $"Unlock");
        }

        public void Unlock(params Type[] commandType)
        {
            this._logger.Verbouse("DummyExecutionContext", "Call method", $"Unlock");
        }

        public void Unlock()
        {
            this._logger.Verbouse("DummyExecutionContext", "Call method", $"Unlock");
        }
    }
}
