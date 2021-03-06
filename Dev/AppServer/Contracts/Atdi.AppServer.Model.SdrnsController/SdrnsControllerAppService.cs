﻿using System;
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

        public sealed class GetMeasTaskHeaderAppOperation : AppOperationBase<SdrnsControllerAppService>
        {
            public GetMeasTaskHeaderAppOperation() : base("GetMeasTaskHeader")
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

        public sealed class GetShortMeasResultsSpecialAppOperation : AppOperationBase<SdrnsControllerAppService>
        {
            public GetShortMeasResultsSpecialAppOperation() : base("GetShortMeasResultsSpecial")
            { }
        }


        public sealed class GetMeasResultsHeaderSpecialAppOperation : AppOperationBase<SdrnsControllerAppService>
        {
            public GetMeasResultsHeaderSpecialAppOperation() : base("GetMeasResultsHeaderSpecial")
            { }
        }

    
        public sealed class GetShortMeasResultsByTypeAndTaskIdAppOperation : AppOperationBase<SdrnsControllerAppService>
        {
            public GetShortMeasResultsByTypeAndTaskIdAppOperation() : base("GetShortMeasResultsByTypeAndTaskId")
            { }
        }


        public sealed class GetShortMeasResultsByIdAppOperation : AppOperationBase<SdrnsControllerAppService>
        {
            public GetShortMeasResultsByIdAppOperation() : base("GetShortMeasResultsById")
            { }
        }

        public sealed class GetShortMeasResStationAppOperation : AppOperationBase<SdrnsControllerAppService>
        {
            public GetShortMeasResStationAppOperation() : base("GetShortMeasResStation")
            { }
        }
        public sealed class GetRoutesAppOperation : AppOperationBase<SdrnsControllerAppService>
        {
            public GetRoutesAppOperation() : base("GetRoutes")
            { }
        }

        public sealed class GetSensorPoligonPointAppOperation : AppOperationBase<SdrnsControllerAppService>
        {
            public GetSensorPoligonPointAppOperation() : base("GetSensorPoligonPoint")
            { }
        }

        public sealed class GetResMeasStationAppOperation : AppOperationBase<SdrnsControllerAppService>
        {
            public GetResMeasStationAppOperation() : base("GetResMeasStation")
            { }
        }

        public sealed class GetResMeasStationByIdAppOperation : AppOperationBase<SdrnsControllerAppService>
        {
            public GetResMeasStationByIdAppOperation() : base("GetResMeasStationById")
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

        public sealed class GetSOformMeasResultStationAppOperation : AppOperationBase<SdrnsControllerAppService>
        {
            public GetSOformMeasResultStationAppOperation() : base("GetSOformMeasResultStation")
            { }
        }

        public sealed class GetShortMeasResultsByDatesAppOperation : AppOperationBase<SdrnsControllerAppService>
        {
            public GetShortMeasResultsByDatesAppOperation() : base("GetShortMeasResultsByDates")
            { }
        }

        public sealed class GetMeasResultsHeaderByTaskIdAppOperation : AppOperationBase<SdrnsControllerAppService>
        {
            public GetMeasResultsHeaderByTaskIdAppOperation() : base("GetMeasResultsHeaderByTaskId")
            { }
        }

        public sealed class GetResMeasStationHeaderByResIdAppOperation : AppOperationBase<SdrnsControllerAppService>
        {
            public GetResMeasStationHeaderByResIdAppOperation() : base("GetResMeasStationHeaderByResId")
            { }
        }


        public sealed class GetMeasurementResultByResIdAppOperation : AppOperationBase<SdrnsControllerAppService>
        {
            public GetMeasurementResultByResIdAppOperation() : base("GetMeasurementResultByResId")
            { }
        }


        public sealed class GetStationDataForMeasurementsByTaskIdAppOperation : AppOperationBase<SdrnsControllerAppService>
        {
            public GetStationDataForMeasurementsByTaskIdAppOperation() : base("GetStationDataForMeasurementsByTaskId")
            { }
        }

        public sealed class GetStationLevelsByTaskIdAppOperation : AppOperationBase<SdrnsControllerAppService>
        {
            public GetStationLevelsByTaskIdAppOperation() : base("GetStationLevelsByTask")
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
                     new DeleteMeasResultsAppOperation(),
                     new GetShortMeasResultsSpecialAppOperation(),
                     new GetShortMeasResStationAppOperation(),
                     new GetRoutesAppOperation(),
                     new GetSensorPoligonPointAppOperation(),
                     new GetResMeasStationAppOperation(),
                     new GetSOformMeasResultStationAppOperation(),
                     new GetSOformMeasResultStationAppOperation(),
                     new GetShortMeasResultsByDatesAppOperation(),
                     new GetMeasResultsHeaderByTaskIdAppOperation(),
                     new GetResMeasStationByIdAppOperation(),
                     new GetResMeasStationHeaderByResIdAppOperation(),
                     new GetMeasurementResultByResIdAppOperation(),
                     new GetMeasTaskHeaderAppOperation(),
                     new GetStationDataForMeasurementsByTaskIdAppOperation(),
                     new GetStationLevelsByTaskIdAppOperation(),
                     new GetShortMeasResultsByTypeAndTaskIdAppOperation(),
                     new GetMeasResultsHeaderSpecialAppOperation()
                }
                );
        }
    }
}
