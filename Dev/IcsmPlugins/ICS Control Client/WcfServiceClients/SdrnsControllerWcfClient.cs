using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ICSM;
using XICSM.ICSControlClient.Environment;
using Atdi.AppServer.Contracts;
using Atdi.AppServer.Contracts.Sdrns;
using MD = XICSM.ICSControlClient.Metadata;

namespace XICSM.ICSControlClient.WcfServiceClients
{
    public class SdrnsControllerWcfClient : WcfServiceClientBase<ISdrnsController, SdrnsControllerWcfClient>
    {
        public SdrnsControllerWcfClient() 
            : base("SdrnsController")
        {
        }

        public static int CreateMeasTask(MeasTask task)
        {
            var otherArgs = new CommonOperationArguments()
            {
                UserId = MD.Employee.GetCurrentUserId()
            };

            var result = Execute(contract => contract.CreateMeasTask(task, otherArgs));


            if (result == null)
            {
                return IM.NullI;
            }
            return result.Value;
        }

        public static ShortSensor[] GetShortSensors(DataConstraint constraint)
        {
            var otherArgs = new CommonOperationArguments()
            {
                UserId = MD.Employee.GetCurrentUserId()
            };

            var result = Execute(contract => contract.GetShortSensors(constraint, otherArgs));

            if (result == null)
            {
                return new ShortSensor[] { };
            }
            return result;
        }

        public static Sensor[] GetSensors(DataConstraint constraint)
        {
            var otherArgs = new CommonOperationArguments()
            {
                UserId = MD.Employee.GetCurrentUserId()
            };

            var result = Execute(contract => contract.GetSensors(constraint, otherArgs));

            if (result == null)
            {
                return new Sensor[] { };
            }
            return result;
        }

        public static ShortMeasTask[] GetShortMeasTasks(DataConstraint constraint)
        {
            var otherArgs = new CommonOperationArguments()
            {
                UserId = MD.Employee.GetCurrentUserId()
            };

            var result = Execute(contract => contract.GetShortMeasTasks(constraint, otherArgs));

            if (result == null)
            {
                return new ShortMeasTask[] { };
            }
            return result;
        }

        public static ShortMeasurementResults[] GetShortMeasResultsByTask(int taskId)
        {
            var otherArgs = new CommonOperationArguments()
            {
                UserId = MD.Employee.GetCurrentUserId()
            };

            var result = Execute(contract => contract.GetShortMeasResultsByTaskId(new MeasTaskIdentifier { Value = taskId }, otherArgs));

            if (result == null)
            {
                return new ShortMeasurementResults[] { };
            }

            return result;
        }

        public static MeasurementResults[] GetMeasResultsByTask(int taskId)
        {
            var otherArgs = new CommonOperationArguments()
            {
                UserId = MD.Employee.GetCurrentUserId()
            };

            var result = Execute(contract => contract.GetMeasResultsByTaskId(new MeasTaskIdentifier { Value = taskId }, otherArgs));

            if (result == null)
            {
                return new MeasurementResults[] { };
            }

            return result;
        }

        public static MeasTask GetMeasTaskById(int taskId)
        {
            var otherArgs = new CommonOperationArguments()
            {
                UserId = MD.Employee.GetCurrentUserId()
            };

            var result = Execute(contract => contract.GetMeasTask(new MeasTaskIdentifier { Value = taskId }, otherArgs));
            
            return result;
        }

        public static MeasurementResults GetMeasResultsById(int measSdrResultsId, int measTaskId, int subMeasTaskId, int subMeasTaskStationId)
        {
            var otherArgs = new CommonOperationArguments()
            {
                UserId = MD.Employee.GetCurrentUserId()
            };

            var id = new MeasurementResultsIdentifier
            {
                MeasSdrResultsId = measSdrResultsId,
                MeasTaskId= new MeasTaskIdentifier {  Value = measTaskId },
                SubMeasTaskId = subMeasTaskId,
                SubMeasTaskStationId = subMeasTaskStationId
            };

            var result = Execute(contract => contract.GetMeasResultsById(id, otherArgs));

            return result;
        }

        public static Sensor GetSensorById(int sensorId)
        {
            var otherArgs = new CommonOperationArguments()
            {
                UserId = MD.Employee.GetCurrentUserId()
            };

            var result = Execute(contract => contract.GetSensor(new SensorIdentifier { Value = sensorId }, otherArgs));
            
            return result;
        }


        public static void DeleteMeasTaskById(int taskId)
        {
            var otherArgs = new CommonOperationArguments()
            {
                UserId = MD.Employee.GetCurrentUserId()
            };

            var result = Execute(contract => contract.DeleteMeasTask(new MeasTaskIdentifier { Value = taskId }, otherArgs));

            if (result.State == CommonOperationState.Fault)
            {
                System.Windows.Forms.MessageBox.Show(result.FaultCause ?? "Unknown error", $"Delete the meas task with  Id #{taskId}", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
            }
        }

        public static void RunMeasTask(int taskId)
        {
            var otherArgs = new CommonOperationArguments()
            {
                UserId = MD.Employee.GetCurrentUserId()
            };

            var result = Execute(contract => contract.RunMeasTask(new MeasTaskIdentifier { Value = taskId }, otherArgs));

            if (result.State == CommonOperationState.Fault)
            {
                System.Windows.Forms.MessageBox.Show(result.FaultCause ?? "Unknown error", $"Run the meas task with  Id #{taskId}", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
            }
        }

        public static void StopMeasTask(int taskId)
        {
            var otherArgs = new CommonOperationArguments()
            {
                UserId = MD.Employee.GetCurrentUserId()
            };

            var result = Execute(contract => contract.StopMeasTask(new MeasTaskIdentifier { Value = taskId }, otherArgs));

            if (result.State == CommonOperationState.Fault)
            {
                System.Windows.Forms.MessageBox.Show(result.FaultCause ?? "Unknown error", $"Stop the meas task with  Id #{taskId}", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
            }
        }
    }
}
