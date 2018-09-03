using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.AppServer;

namespace Atdi.AppServer.Models.AppServices
{
    public sealed class SdrnsControllerAppService : AppServiceBase
    {
        public sealed class GetSensorAppOperation : AppOperationBase<SdrnsControllerAppService>
        {
            public GetSensorAppOperation() : base("GetSensor")
            { }
        }

        public sealed class GetSensorsAppOperation : AppOperationBase<SdrnsControllerAppService>
        {
            public GetSensorsAppOperation() : base("GetSensors")
            { }
        }

        public sealed class GetShortSensorAppOperation : AppOperationBase<SdrnsControllerAppService>
        {
            public GetShortSensorAppOperation() : base("GetShortSensor")
            { }
        }

        public sealed class GetShortSensorsAppOperation : AppOperationBase<SdrnsControllerAppService>
        {
            public GetShortSensorsAppOperation() : base("GetShortSensors")
            { }
        }

        public sealed class CreateMeasTaskAppOperation : AppOperationBase<SdrnsControllerAppService>
        {
            public CreateMeasTaskAppOperation() : base("CreateMeasTask")
            { }
        }

        public sealed class DeleteMeasTaskAppOperation : AppOperationBase<SdrnsControllerAppService>
        {
            public DeleteMeasTaskAppOperation() : base("DeleteMeasTask")
            { }
        }

        public sealed class RunMeasTaskAppOperation : AppOperationBase<SdrnsControllerAppService>
        {
            public RunMeasTaskAppOperation() : base("RunMeasTask")
            { }
        }

        public sealed class StopMeasTaskAppOperation : AppOperationBase<SdrnsControllerAppService>
        {
            public StopMeasTaskAppOperation() : base("StopMeasTask")
            { }
        }

        public sealed class GetMeasTaskAppOperation : AppOperationBase<SdrnsControllerAppService>
        {
            public GetMeasTaskAppOperation() : base("GetMeasTask")
            { }
        }

        public sealed class GetMeasTasksAppOperation : AppOperationBase<SdrnsControllerAppService>
        {
            public GetMeasTasksAppOperation() : base("GetMeasTasks")
            { }
        }

        public sealed class GetShortMeasTaskAppOperation : AppOperationBase<SdrnsControllerAppService>
        {
            public GetShortMeasTaskAppOperation() : base("GetShortMeasTask")
            { }
        }

        public sealed class GetShortMeasTasksAppOperation : AppOperationBase<SdrnsControllerAppService>
        {
            public GetShortMeasTasksAppOperation() : base("GetShortMeasTasks")
            { }
        }

        public sealed class GetMeasResultsAppOperation : AppOperationBase<SdrnsControllerAppService>
        {
            public GetMeasResultsAppOperation() : base("GetMeasResults")
            { }
        }

        public sealed class GetMeasResultsByIdAppOperation : AppOperationBase<SdrnsControllerAppService>
        {
            public GetMeasResultsByIdAppOperation() : base("GetMeasResultsById")
            { }
        }

        public sealed class GetMeasResultsByTaskIdAppOperation : AppOperationBase<SdrnsControllerAppService>
        {
            public GetMeasResultsByTaskIdAppOperation() : base("GetMeasResultsByTaskId")
            { }
        }

        public sealed class GetShortMeasResultsAppOperation : AppOperationBase<SdrnsControllerAppService>
        {
            public GetShortMeasResultsAppOperation() : base("GetShortMeasResults")
            { }
        }

        public sealed class GetShortMeasResultsByIdAppOperation : AppOperationBase<SdrnsControllerAppService>
        {
            public GetShortMeasResultsByIdAppOperation() : base("GetShortMeasResultsById")
            { }
        }

        public sealed class GetShortMeasResultsByTaskIdAppOperation : AppOperationBase<SdrnsControllerAppService>
        {
            public GetShortMeasResultsByTaskIdAppOperation() : base("GetShortMeasResultsByTaskId")
            { }
        }

        public sealed class DeleteMeasResultsAppOperation : AppOperationBase<SdrnsControllerAppService>
        {
            public DeleteMeasResultsAppOperation() : base("DeleteMeasResults")
            { }
        }



        public SdrnsControllerAppService() 
            : base("SdrnsController")
        {
            this._operations.AddRange(
                 new IAppOperation[] {
                     new GetSensorsAppOperation(),
                     new GetSensorAppOperation(),
                     new GetShortSensorsAppOperation(),
                     new GetShortSensorAppOperation(),
                     new CreateMeasTaskAppOperation(),
                     new DeleteMeasTaskAppOperation(),
                     new RunMeasTaskAppOperation(),
                     new StopMeasTaskAppOperation(),
                     new GetMeasTaskAppOperation(),
                     new GetMeasTasksAppOperation(),
                     new GetShortMeasTaskAppOperation(),
                     new GetShortMeasTasksAppOperation(),
                     new GetMeasResultsAppOperation(),
                     new GetMeasResultsByIdAppOperation(),
                     new GetMeasResultsByTaskIdAppOperation(),
                     new GetShortMeasResultsAppOperation(),
                     new GetShortMeasResultsByIdAppOperation(),
                     new GetShortMeasResultsByTaskIdAppOperation(),
                     new DeleteMeasResultsAppOperation()
                    }
                );
        }
    }
}
