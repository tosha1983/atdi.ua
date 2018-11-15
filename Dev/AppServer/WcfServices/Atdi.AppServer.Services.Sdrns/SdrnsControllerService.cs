using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;

using Atdi.AppServer.Contracts;
using Atdi.AppServer.Contracts.Sdrns;
using Atdi.AppServer.Models.TechServices;
using Atdi.AppServer.Models.AppServices;
using Atdi.AppServer.Models.AppServices.SdrnsController;

namespace Atdi.AppServer.Services.Sdrns
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall, ConcurrencyMode = ConcurrencyMode.Multiple)]
    public class SdrnsControllerService : WcfServiceBase<SdrnsControllerAppService, ISdrnsController>, ISdrnsController
    {
        public SdrnsControllerService(IAppServiceInvokerFactory factory, ILogger logger) : base(factory, logger)
        {
        }

        MeasTaskIdentifier ISdrnsController.CreateMeasTask(MeasTask task, CommonOperationArguments otherArgs)
        {
            var result =
                Operation<SdrnsControllerAppService.CreateMeasTaskAppOperation, MeasTaskIdentifier>()
                    .Invoke(
                        new CreateMeasTaskAppOperationOptions
                        {
                            Task = task,
                            OtherArgs = otherArgs
                        },
                        this.OperationContext
                    );

            return result;
        }

        CommonOperationDataResult<int> ISdrnsController.DeleteMeasResults(MeasurementResultsIdentifier MeasResultsId, CommonOperationArguments otherArgs)
        {
            var result =
                Operation<SdrnsControllerAppService.DeleteMeasResultsAppOperation, CommonOperationDataResult<int>>()
                    .Invoke(
                        new DeleteMeasResultsAppOperationOptions
                        {
                            MeasResultsId = MeasResultsId,
                            OtherArgs = otherArgs
                        },
                        this.OperationContext
                    );

            return result;
        }

        CommonOperationResult ISdrnsController.DeleteMeasTask(MeasTaskIdentifier taskId, CommonOperationArguments otherArgs)
        {
            var result =
                Operation<SdrnsControllerAppService.DeleteMeasTaskAppOperation, CommonOperationResult>()
                    .Invoke(
                        new DeleteMeasTaskAppOperationOptions
                        {
                            TaskId = taskId,
                            OtherArgs = otherArgs
                        },
                        this.OperationContext
                    );

            return result;
        }

        MeasurementResults[] ISdrnsController.GetMeasResults(DataConstraint constraint, CommonOperationArguments otherArgs)
        {
            var result =
                Operation<SdrnsControllerAppService.GetMeasResultsAppOperation, MeasurementResults[]>()
                    .Invoke(
                        new GetMeasResultsAppOperationOptions
                        {
                            Constraint = constraint,
                            OtherArgs = otherArgs
                        },
                        this.OperationContext
                    );

            return result;
        }

        MeasTask ISdrnsController.GetMeasTask(MeasTaskIdentifier taskId, CommonOperationArguments otherArgs)
        {
            var result =
                Operation<SdrnsControllerAppService.GetMeasTaskAppOperation, MeasTask>()
                    .Invoke(
                        new GetMeasTaskAppOperationOptions
                        {
                            TaskId = taskId,
                            OtherArgs = otherArgs
                        },
                        this.OperationContext
                    );

            return result;
        }

        StationDataForMeasurements[] ISdrnsController.GetStationDataForMeasurementsByTaskId(MeasTaskIdentifier taskId, CommonOperationArguments otherArgs)
        {
            var result =
                Operation<SdrnsControllerAppService.GetStationDataForMeasurementsByTaskIdAppOperation, StationDataForMeasurements[]>()
                    .Invoke(
                        new GetMeasTaskAppOperationOptions
                        {
                            TaskId = taskId,
                            OtherArgs = otherArgs
                        },
                        this.OperationContext
                    );

            return result;
        }

        

        MeasTask ISdrnsController.GetMeasTaskHeader(MeasTaskIdentifier taskId, CommonOperationArguments otherArgs)
        {
            var result =
                Operation<SdrnsControllerAppService.GetMeasTaskHeaderAppOperation, MeasTask>()
                    .Invoke(
                        new GetMeasTaskAppOperationOptions
                        {
                            TaskId = taskId,
                            OtherArgs = otherArgs
                        },
                        this.OperationContext
                    );

            return result;
        }

        MeasTask[] ISdrnsController.GetMeasTasks(DataConstraint constraint, CommonOperationArguments otherArgs)
        {
            var result =
                Operation<SdrnsControllerAppService.GetMeasTasksAppOperation, MeasTask[]>()
                    .Invoke(
                        new GetMeasTasksAppOperationOptions
                        {
                            Constraint = constraint,
                            OtherArgs = otherArgs
                        },
                        this.OperationContext
                    );

            return result;
        }

        Sensor[] ISdrnsController.GetSensors(DataConstraint constraint, CommonOperationArguments otherArgs)
        {
            var result =
                Operation<SdrnsControllerAppService.GetSensorsAppOperation, Sensor[]>()
                    .Invoke(
                        new GetSensorsAppOperationOptions
                        {
                            Constraint = constraint,
                            OtherArgs = otherArgs
                        },
                        this.OperationContext
                    );

            return result;
        }

        ShortMeasurementResults[] ISdrnsController.GetShortMeasResultsSpecial(MeasurementType measurementType, CommonOperationArguments otherArgs)
        {
            var result =
                Operation<SdrnsControllerAppService.GetShortMeasResultsSpecialAppOperation, ShortMeasurementResults[]>()
                    .Invoke(
                        new GetShortMeasResultsSpecialAppOperationOptions
                        {
                            measurementType = measurementType,
                            OtherArgs = otherArgs
                        },
                        this.OperationContext
                    );

            return result;
        }

        ShortMeasurementResults[] ISdrnsController.GetShortMeasResultsByTypeAndTaskId(MeasurementType measurementType, int taskId, CommonOperationArguments otherArgs)
        {
            var result =
                Operation<SdrnsControllerAppService.GetShortMeasResultsByTypeAndTaskIdAppOperation , ShortMeasurementResults[]>()
                    .Invoke(
                        new GetShortMeasResultsByTypeAndTaskIdAppOperationOptions
                        {
                            measurementType = measurementType,
                            TaskId = taskId,
                            OtherArgs = otherArgs
                        },
                        this.OperationContext
                    );

            return result;
        }

        


        ShortMeasurementResults[] ISdrnsController.GetShortMeasResults(DataConstraint constraint, CommonOperationArguments otherArgs)
        {
            var result =
                Operation<SdrnsControllerAppService.GetShortMeasResultsAppOperation, ShortMeasurementResults[]>()
                    .Invoke(
                        new GetShortMeasResultsAppOperationOptions
                        {
                            Constraint = constraint,
                            OtherArgs = otherArgs
                        },
                        this.OperationContext
                    );

            return result;
        }

        ShortMeasTask[] ISdrnsController.GetShortMeasTasks(DataConstraint constraint, CommonOperationArguments otherArgs)
        {
            var result =
                Operation<SdrnsControllerAppService.GetShortMeasTasksAppOperation, ShortMeasTask[]>()
                    .Invoke(
                        new GetShortMeasTasksAppOperationOptions
                        {
                            Constraint = constraint,
                            OtherArgs = otherArgs
                        },
                        this.OperationContext
                    );

            return result;
        }

        ShortSensor[] ISdrnsController.GetShortSensors(DataConstraint constraint, CommonOperationArguments otherArgs)
        {
            var result =
                Operation<SdrnsControllerAppService.GetShortSensorsAppOperation, ShortSensor[]>()
                    .Invoke(
                        new GetShortSensorsAppOperationOptions
                        {
                            Constraint = constraint,
                            OtherArgs = otherArgs
                        },
                        this.OperationContext
                    );

            return result;
        }

        CommonOperationResult ISdrnsController.RunMeasTask(MeasTaskIdentifier taskId, CommonOperationArguments otherArgs)
        {
            var result =
                Operation<SdrnsControllerAppService.RunMeasTaskAppOperation, CommonOperationResult>()
                    .Invoke(
                        new RunMeasTaskAppOperationOptions
                        {
                            TaskId = taskId,
                            OtherArgs = otherArgs
                        },
                        this.OperationContext
                    );

            return result;
        }

        CommonOperationResult ISdrnsController.StopMeasTask(MeasTaskIdentifier taskId, CommonOperationArguments otherArgs)
        {
            var result =
                Operation<SdrnsControllerAppService.StopMeasTaskAppOperation, CommonOperationResult>()
                    .Invoke(
                        new StopMeasTaskAppOperationOptions
                        {
                            TaskId = taskId,
                            OtherArgs = otherArgs
                        },
                        this.OperationContext
                    );

            return result;
        }

        Sensor ISdrnsController.GetSensor(SensorIdentifier sensorId, CommonOperationArguments otherArgs)
        {
            var result =
                Operation<SdrnsControllerAppService.GetSensorAppOperation, Sensor>()
                    .Invoke(
                        new GetSensorAppOperationOptions
                        {
                            SensorId = sensorId,
                            OtherArgs = otherArgs
                        },
                        this.OperationContext
                    );

            return result;
        }

        ShortSensor ISdrnsController.GetShortSensor(SensorIdentifier sensorId, CommonOperationArguments otherArgs)
        {
            var result =
                Operation<SdrnsControllerAppService.GetShortSensorAppOperation, ShortSensor>()
                    .Invoke(
                        new GetShortSensorAppOperationOptions
                        {
                            SensorId = sensorId,
                            OtherArgs = otherArgs
                        },
                        this.OperationContext
                    );

            return result;
        }

        ShortMeasTask ISdrnsController.GetShortMeasTask(MeasTaskIdentifier taskId, CommonOperationArguments otherArgs)
        {
            var result =
                Operation<SdrnsControllerAppService.GetShortMeasTaskAppOperation, ShortMeasTask>()
                    .Invoke(
                        new GetShortMeasTaskAppOperationOptions
                        {
                            TaskId = taskId,
                            OtherArgs = otherArgs
                        },
                        this.OperationContext
                    );

            return result;
        }

        MeasurementResults ISdrnsController.GetMeasResultsById(MeasurementResultsIdentifier measResultsId, CommonOperationArguments otherArgs)
        {
            var result =
                Operation<SdrnsControllerAppService.GetMeasResultsByIdAppOperation, MeasurementResults>()
                    .Invoke(
                        new GetMeasResultsByIdAppOperationOptions
                        {
                            MeasResultsId = measResultsId,
                            OtherArgs = otherArgs
                        },
                        this.OperationContext
                    );

            return result;
        }

        MeasurementResults[] ISdrnsController.GetMeasResultsByTaskId(MeasTaskIdentifier taskId, CommonOperationArguments otherArgs)
        {
            var result =
                Operation<SdrnsControllerAppService.GetMeasResultsByTaskIdAppOperation, MeasurementResults[]>()
                    .Invoke(
                        new GetMeasResultsByTaskIdAppOperationOptions
                        {
                            TaskId  = taskId,
                            OtherArgs = otherArgs
                        },
                        this.OperationContext
                    );

            return result;
        }

        ShortMeasurementResults ISdrnsController.GetShortMeasResultsById(MeasurementResultsIdentifier measResultsId, CommonOperationArguments otherArgs)
        {
            var result =
                Operation<SdrnsControllerAppService.GetShortMeasResultsByIdAppOperation, ShortMeasurementResults>()
                    .Invoke(
                        new GetShortMeasResultsByIdAppOperationOptions
                        {
                            MeasResultsId = measResultsId,
                            OtherArgs = otherArgs
                        },
                        this.OperationContext
                    );

            return result;
        }

        ShortResultsMeasurementsStation[] ISdrnsController.GetShortMeasResStation(int measResultsId, CommonOperationArguments otherArgs)
        {
            var result =
                Operation<SdrnsControllerAppService.GetShortMeasResStationAppOperation, ShortResultsMeasurementsStation[]>()
                    .Invoke(
                        new GetShortMeasResStationAppOperationOptions
                        {
                            ResId = measResultsId,
                            OtherArgs = otherArgs
                        },
                        this.OperationContext
                    );

            return result;
        }

        Route[] ISdrnsController.GetRoutes(int measResultsId, CommonOperationArguments otherArgs)
        {
            var result =
                Operation<SdrnsControllerAppService.GetRoutesAppOperation, Route[]>()
                    .Invoke(
                        new GetShortMeasResStationAppOperationOptions
                        {
                            ResId = measResultsId,
                            OtherArgs = otherArgs
                        },
                        this.OperationContext
                    );

            return result;
        }

        SensorPoligonPoint[] ISdrnsController.GetSensorPoligonPoint(int measResultsId, CommonOperationArguments otherArgs)
        {
            var result =
                Operation<SdrnsControllerAppService.GetSensorPoligonPointAppOperation, SensorPoligonPoint[]>()
                    .Invoke(
                        new GetShortMeasResStationAppOperationOptions
                        {
                            ResId = measResultsId,
                            OtherArgs = otherArgs
                        },
                        this.OperationContext
                    );

            return result;
        }

        ResultsMeasurementsStation[] ISdrnsController.GetResMeasStation(int measResultsId, int StationId, CommonOperationArguments otherArgs)
        {
            var result =
                Operation<SdrnsControllerAppService.GetResMeasStationAppOperation, ResultsMeasurementsStation[]>()
                    .Invoke(
                        new GetResMeasStationAppOperationOptions
                        {
                            ResId = measResultsId,
                             StationId = StationId,
                            OtherArgs = otherArgs
                        },
                        this.OperationContext
                    );

            return result;
        }


        ResultsMeasurementsStation ISdrnsController.GetResMeasStationById(int StationId, CommonOperationArguments otherArgs)
        {
            var result =
                Operation<SdrnsControllerAppService.GetResMeasStationByIdAppOperation, ResultsMeasurementsStation>()
                    .Invoke(
                        new GetResMeasStationByIdAppOperationOptions
                        {
                            StationId = StationId,
                            OtherArgs = otherArgs
                        },
                        this.OperationContext
                    );

            return result;
        }

        ShortMeasurementResults[] ISdrnsController.GetShortMeasResultsByTaskId(MeasTaskIdentifier taskId, CommonOperationArguments otherArgs)
        {
            var result =
                Operation<SdrnsControllerAppService.GetShortMeasResultsByTaskIdAppOperation, ShortMeasurementResults[]>()
                    .Invoke(
                        new GetShortMeasResultsByTaskIdAppOperationOptions
                        {
                            TaskId = taskId,
                            OtherArgs = otherArgs
                        },
                        this.OperationContext
                    );

            return result;
        }

        SOFrequency[] ISdrnsController.GetSOformMeasResultStation(GetSOformMeasResultStationValue options, CommonOperationArguments otherArgs)
        {
            var result =
                Operation<SdrnsControllerAppService.GetSOformMeasResultStationAppOperation, SOFrequency[]>()
                    .Invoke(
                        new GetSOformMeasResultStationAppOperationOptions
                        {
                            val = options, 
                            
                            OtherArgs = otherArgs
                        },
                        this.OperationContext
                    );

            return result;
        }

        public ShortMeasurementResultsExtend[] GetShortMeasResultsByDate(GetShortMeasResultsByDateValue options, CommonOperationArguments otherArgs)
        {
            var result =
               Operation<SdrnsControllerAppService.GetShortMeasResultsByDatesAppOperation, ShortMeasurementResultsExtend[]>()
                   .Invoke(
                       new GetShortMeasResultsByDateAppOperationOptions
                       {
                           options = options,
                           OtherArgs = otherArgs
                       },
                       this.OperationContext
                   );

            return result;
        }

        MeasurementResults[] ISdrnsController.GetMeasResultsHeaderByTaskId(MeasTaskIdentifier taskId, CommonOperationArguments otherArgs)
        {
            var result =
              Operation<SdrnsControllerAppService.GetMeasResultsHeaderByTaskIdAppOperation, MeasurementResults[]>()
                  .Invoke(
                      new GetMeasResultsByTaskIdAppOperationOptions
                      {
                          TaskId = taskId,
                          OtherArgs = otherArgs
                      },
                      this.OperationContext
                  );

            return result;
        }


        ResultsMeasurementsStationExtended[] ISdrnsController.GetResMeasStationHeaderByResId(int measResultsId, CommonOperationArguments otherArgs)
        {
            var result =
              Operation<SdrnsControllerAppService.GetResMeasStationHeaderByResIdAppOperation, ResultsMeasurementsStationExtended[]>()
                  .Invoke(
                      new GetShortMeasResStationAppOperationOptions
                      {
                          ResId = measResultsId,
                          OtherArgs = otherArgs
                      },
                      this.OperationContext
                  );

            return result;
        }


        MeasurementResults ISdrnsController.GetMeasurementResultByResId(int measResultsId, CommonOperationArguments otherArgs)
        {
            var result =
              Operation<SdrnsControllerAppService.GetMeasurementResultByResIdAppOperation, MeasurementResults>()
                  .Invoke(
                      new GetShortMeasResStationAppOperationOptions
                      {
                          ResId = measResultsId,
                          OtherArgs = otherArgs
                      },
                      this.OperationContext
                  );

            return result;
        }



        StationLevelsByTask[] ISdrnsController.GetStationLevelsByTask(LevelsByTaskParams options, CommonOperationArguments otherArgs)
        {
            var result =
                Operation<SdrnsControllerAppService.GetStationLevelsByTaskIdAppOperation, StationLevelsByTask[]>()
                    .Invoke(
                        new GetStationLevelsByTaskIdAppOperationOptions
                        {
                            val = options,
                            OtherArgs = otherArgs
                        },
                        this.OperationContext
                    );

            return result;
        }



    }
}
