using Atdi.DataModels.Sdrns.Device;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DM = Atdi.DataModels.Sdrns.Device;

namespace Atdi.DataModels.Sdrn.DeviceServer.Processing
{
    public class MainProcess : ProcessBase
    {
        private object _syncLat = new object();
        private object _syncLon = new object();
        private object _syncAsl = new object();
        private object _syncActiveSensor = new object();
        private object _syncDeviceCommand = new object();
        private object _syncSensorRegistrationResult = new object();
        private object _syncContextRegisterSensorTask = new object();
        private object _syncContextQueueEventTask = new object();
        private object _syncListDeferredTasks = new object();
        private object _syncContextSOTask = new object();


        private float _lat;
        private float _lon;
        private float _asl;
        private DM.Sensor _activeSensor;
        private DM.DeviceCommand _deviceCommand;
        private DM.SensorRegistrationResult _sensorRegistrationResult;
        private ITaskContext<RegisterSensorTask, BaseContext> _contextRegisterSensorTask;
        private ITaskContext<QueueEventTask, BaseContext> _contextQueueEventTask;
        private List<ITaskContext<SOTask, SpectrumOccupationProcess>> _contextSOTasks;
        private List<TaskParameters> _listDeferredTasks;
        
         
        public float Lat
        {
            get
            {
                lock (_syncLat)
                    return _lat;
            }
            set
            {
                lock (_syncLat)
                    _lat = value;
            }
        }

        public float Lon
        {
            get
            {
                lock (_syncLon)
                    return _lon;
            }
            set
            {
                lock (_syncLon)
                    _lon = value;
            }
        }

        public float Asl
        {
            get
            {
                lock (_syncAsl)
                    return _asl;
            }
            set
            {
                lock (_syncAsl)
                    _asl = value;
            }
        }

        public DM.Sensor activeSensor
        {
            get
            {
                lock (_syncActiveSensor)
                    return _activeSensor;
            }
            set
            {
                lock (_syncActiveSensor)
                    _activeSensor = value;
            }
        }

        public DM.DeviceCommand deviceCommand
        {
            get
            {
                lock (_syncDeviceCommand)
                    return _deviceCommand;
            }
            set
            {
                lock (_syncDeviceCommand)
                    _deviceCommand = value;
            }
        }

        public DM.SensorRegistrationResult sensorRegistrationResult
        {
            get
            {
                lock (_syncSensorRegistrationResult)
                    return _sensorRegistrationResult;
            }
            set
            {
                lock (_syncSensorRegistrationResult)
                    _sensorRegistrationResult = value;
            }
        }


        public ITaskContext<RegisterSensorTask, BaseContext> contextRegisterSensorTask
        {
            get
            {
                lock (_syncContextRegisterSensorTask)
                    return _contextRegisterSensorTask;
            }
            set
            {
                lock (_syncContextRegisterSensorTask)
                    _contextRegisterSensorTask = value;
            }
        }

        public ITaskContext<QueueEventTask, BaseContext> contextQueueEventTask
        {
            get
            {
                lock (_syncContextQueueEventTask)
                    return _contextQueueEventTask;
            }
            set
            {
                lock (_syncContextQueueEventTask)
                    _contextQueueEventTask = value;
            }
        }



        public List<TaskParameters> listDeferredTasks
        {
            get
            {
                lock (_syncListDeferredTasks)
                    return _listDeferredTasks;
            }
            set
            {
                lock (_syncListDeferredTasks)
                    _listDeferredTasks = value;
            }
        }



        public List<ITaskContext<SOTask, SpectrumOccupationProcess>> contextSOTasks
        {
            get
            {
                lock (_syncContextSOTask)
                    return _contextSOTasks;
            }
            set
            {
                lock (_syncContextSOTask)
                    _contextSOTasks = value;
            }
        }
        


        public MainProcess() : base("Main process")
        {
            listDeferredTasks = new List<TaskParameters>();
            contextSOTasks = new List<ITaskContext<SOTask, SpectrumOccupationProcess>>();
        }
    }
}
