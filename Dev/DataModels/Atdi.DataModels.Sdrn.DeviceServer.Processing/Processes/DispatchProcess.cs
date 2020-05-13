using Atdi.DataModels.Sdrns.Device;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DM = Atdi.DataModels.Sdrns.Device;
using System.Collections.Concurrent;

namespace Atdi.DataModels.Sdrn.DeviceServer.Processing
{
    public class DispatchProcess : ProcessBase
    {
        private object _syncLat = new object();
        private object _syncLon = new object();
        private object _syncAsl = new object();
        private object _syncActiveSensor = new object();
        private double _lat;
        private double _lon;
        private double _asl;
        private DM.Sensor _activeSensor;
        public ConcurrentBag<ITaskContext<SOTask, SpectrumOccupationProcess>> contextSOTasks;
        public ConcurrentBag<ITaskContext<SignalizationTask, SignalizationProcess>> contextSignalizationTasks;
        public ConcurrentBag<ITaskContext<BandWidthTask, BandWidthProcess>> contextBandWidthTasks;
        public ConcurrentBag<ITaskContext<SysInfoTask, SysInfoProcess>> contextSysInfoTasks;
        public ConcurrentBag<TaskParameters> listDeferredTasks;
                 
        public double Lat
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

        public double Lon
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

        public double Asl
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


        public DispatchProcess() : base("Dispatch process")
        {
            listDeferredTasks = new ConcurrentBag<TaskParameters>();
            contextSOTasks = new ConcurrentBag<ITaskContext<SOTask, SpectrumOccupationProcess>>();
            contextSignalizationTasks = new ConcurrentBag<ITaskContext<SignalizationTask, SignalizationProcess>>();
            contextBandWidthTasks = new ConcurrentBag<ITaskContext<BandWidthTask, BandWidthProcess>>();
            contextSysInfoTasks = new ConcurrentBag<ITaskContext<SysInfoTask, SysInfoProcess>>();
        }
    }
}
