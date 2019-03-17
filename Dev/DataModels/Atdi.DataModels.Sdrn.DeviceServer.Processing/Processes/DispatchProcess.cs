using Atdi.DataModels.Sdrns.Device;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DM = Atdi.DataModels.Sdrns.Device;

namespace Atdi.DataModels.Sdrn.DeviceServer.Processing
{
    public class DispatchProcess : ProcessBase
    {
        private object _syncLat = new object();
        private object _syncLon = new object();
        private object _syncAsl = new object();
        private object _syncActiveSensor = new object();
        private object _syncListDeferredTasks = new object();
        private object _syncContextSOTask = new object();
        private object _syncContextSignalization = new object();
        private object _syncContextBandWidth = new object();


        private double _lat;
        private double _lon;
        private double _asl;
        private DM.Sensor _activeSensor;
        private List<ITaskContext<SOTask, SpectrumOccupationProcess>> _contextSOTasks;
        private List<ITaskContext<SignalizationTask, SignalizationProcess>> _contextSignalizationTasks;
        private List<ITaskContext<BandWidthTask, BandWidthProcess>> _contextBandWidthTasks;





        private List<TaskParameters> _listDeferredTasks;

         
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

        public List<ITaskContext<SignalizationTask, SignalizationProcess>> contextSignalizationTasks
        {
            get
            {
                lock (_syncContextSignalization)
                    return _contextSignalizationTasks;
            }
            set
            {
                lock (_syncContextSignalization)
                    _contextSignalizationTasks = value;
            }
        }

        public List<ITaskContext<BandWidthTask, BandWidthProcess>> contextBandWidthTasks
        {
            get
            {
                lock (_syncContextBandWidth)
                    return _contextBandWidthTasks;
            }
            set
            {
                lock (_syncContextBandWidth)
                    _contextBandWidthTasks = value;
            }
        }

        

        public DispatchProcess() : base("Dispatch process")
        {
            listDeferredTasks = new List<TaskParameters>();
            contextSOTasks = new List<ITaskContext<SOTask, SpectrumOccupationProcess>>();
            contextSignalizationTasks = new List<ITaskContext<SignalizationTask, SignalizationProcess>>();
            contextBandWidthTasks = new List<ITaskContext<BandWidthTask, BandWidthProcess>>();
        }
    }
}
