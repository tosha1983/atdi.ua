using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SVC = XICSM.ICSControlClient.WcfServiceClients;
using Atdi.Contracts.WcfServices.Sdrn.Server;
using Atdi.DataModels.DataConstraint;
using XICSM.ICSControlClient.Models.Views;
using XICSM.ICSControlClient.Models.WcfDataApadters;

namespace XICSM.ICSControlClient.Models
{
    public class DataStore
    {
        public delegate void OnBeginInvokeEventHandler(string description);
        public delegate void OnEndInvokeEventHandler(string description);

        public event OnBeginInvokeEventHandler OnBeginInvoke;
        public event OnEndInvokeEventHandler OnEndInvoke;

        private static DataStore _instance = new DataStore();

        private ConcurrentDictionary<int, MeasurementResults[]> _GetMeasResultsHeaderByTaskIdCache;
        private ConcurrentDictionary<int, ShortSensor[]> _GetShortSensorsByTaskCache;
        private ShortSensor[] _shortSensors = null;

        private ConcurrentDictionary<int, MeasTaskViewModel> _GetMeasTaskHeaderByIdCache;
        private ConcurrentDictionary<int, StationDataForMeasurements[]> _GetStationDataForMeasurementsByTaskIdCache;
        private ConcurrentDictionary<int, Sensor> _GetSensorByIdCache;
        private ConcurrentDictionary<int, ResultsMeasurementsStation[]> _GetResMeasStationHeaderByResIdCache;
        private ConcurrentDictionary<int, MeasurementResults> _GetMeasurementResultByResIdCache;

        public static DataStore GetStore()
        {
            return DataStore._instance;
        }

        private DataStore()
        {
            this.Reset();
        }

        public MeasurementResults[] GetMeasResultsHeaderByTaskId(int taskId)
        {
            
            var cache = this._GetMeasResultsHeaderByTaskIdCache;

            if (!cache.TryGetValue(taskId, out MeasurementResults[] data))
            {
                var decription = $"Measurement Results for task with ID #{taskId}";
                this.OnBeginInvoke?.Invoke(decription);
                data = SVC.SdrnsControllerWcfClient.GetMeasResultsHeaderByTaskId(taskId);
                this.OnEndInvoke?.Invoke(decription);

                if (!cache.TryAdd(taskId, data))
                {
                    if (cache.TryGetValue(taskId, out MeasurementResults[] data2))
                    {
                        return data2;
                    }
                }
            }
            return data;
        }

        public ShortSensor[] GetShortSensors()
        {
            if (this._shortSensors == null)
            {
                var decription = $"Short info of all sensors";
                this.OnBeginInvoke?.Invoke(decription);
                this._shortSensors = SVC.SdrnsControllerWcfClient.GetShortSensors();
                this.OnEndInvoke?.Invoke(decription);
            }
            return this._shortSensors;
        }

        public ShortSensor[] GetShortSensorsByTask(MeasTaskViewModel measTask)
        {
            var allSensors = this.GetShortSensors();
            if (measTask == null)
            {
                return allSensors;
            }

            var cache = this._GetShortSensorsByTaskCache;

            if (!cache.TryGetValue(measTask.Id, out ShortSensor[] data))
            {
                if (measTask.Stations != null && measTask.Stations.Length > 0)
                {
                    data = allSensors
                        .Where(sdrSensor => measTask.Stations
                                .FirstOrDefault(s => s.StationId.Value == sdrSensor.Id.Value) != null
                            )
                        .ToArray();
                }
                else
                {
                    data = new ShortSensor[] { };
                }

                if (!cache.TryAdd(measTask.Id, data))
                {
                    if (cache.TryGetValue(measTask.Id, out ShortSensor[] data2))
                    {
                        return data2;
                    }
                }
            }
            return data;
        }

        public MeasTaskViewModel GetMeasTaskHeaderById(int taskId)
        {
            var cache = this._GetMeasTaskHeaderByIdCache;

            if (taskId == 0)
            {
                return null;
            }

            if (!cache.TryGetValue(taskId, out MeasTaskViewModel data))
            {
                var decription = $"Measurement Task Header with ID #{taskId}";
                this.OnBeginInvoke?.Invoke(decription);
                var task = SVC.SdrnsControllerWcfClient.GetMeasTaskHeaderById(taskId);
                this.OnEndInvoke?.Invoke(decription);

                data = Mappers.Map(task);
                if (!cache.TryAdd(taskId, data))
                {
                    if (cache.TryGetValue(taskId, out MeasTaskViewModel data2))
                    {
                        return data2;
                    }
                }
            }
            return data;
        }

        public StationDataForMeasurements[] GetStationDataForMeasurementsByTaskId(int taskId)
        {
            var cache = this._GetStationDataForMeasurementsByTaskIdCache;

            if (!cache.TryGetValue(taskId, out StationDataForMeasurements[] data))
            {
                var decription = $"Station Data For Measurements by task with ID #{taskId}";
                this.OnBeginInvoke?.Invoke(decription);
                data = SVC.SdrnsControllerWcfClient.GetStationDataForMeasurementsByTaskId(taskId);
                this.OnEndInvoke?.Invoke(decription);

                if (!cache.TryAdd(taskId, data))
                {
                    if (cache.TryGetValue(taskId, out StationDataForMeasurements[] data2))
                    {
                        return data2;
                    }
                }
            }
            return data;
        }

        public Sensor GetSensorById(int sensorId)
        {
            var cache = this._GetSensorByIdCache;

            if (sensorId == 0)
            {
                return null;
            }

            if (!cache.TryGetValue(sensorId, out Sensor data))
            {
                var decription = $"Sensor Full Info with ID #{sensorId}";
                this.OnBeginInvoke?.Invoke(decription);
                data = SVC.SdrnsControllerWcfClient.GetSensorById(sensorId);
                this.OnEndInvoke?.Invoke(decription);

                if (!cache.TryAdd(sensorId, data))
                {
                    if (cache.TryGetValue(sensorId, out Sensor data2))
                    {
                        return data2;
                    }
                }
            }
            return data;
        }

        public ResultsMeasurementsStation[] GetResMeasStationHeaderByResId(int resultsId)
        {
            var cache = this._GetResMeasStationHeaderByResIdCache;

            if (!cache.TryGetValue(resultsId, out ResultsMeasurementsStation[] data))
            {
                var decription = $"Result Measurement Station Header by the result set with ID #{resultsId}";
                this.OnBeginInvoke?.Invoke(decription);
                data = SVC.SdrnsControllerWcfClient.GetResMeasStationHeaderByResId(resultsId);
                this.OnEndInvoke?.Invoke(decription);

                if (!cache.TryAdd(resultsId, data))
                {
                    if (cache.TryGetValue(resultsId, out ResultsMeasurementsStation[] data2))
                    {
                        return data2;
                    }
                }
            }
            return data;
        }

        public MeasurementResults GetMeasurementResultByResId(int resultsId)
        {
            var cache = this._GetMeasurementResultByResIdCache;

            if (!cache.TryGetValue(resultsId, out MeasurementResults data))
            {
                var decription = $"Measurement Results by the result set with ID #{resultsId}";
                this.OnBeginInvoke?.Invoke(decription);
                data = SVC.SdrnsControllerWcfClient.GetMeasurementResultByResId(resultsId);
                this.OnEndInvoke?.Invoke(decription);

                if (!cache.TryAdd(resultsId, data))
                {
                    if (cache.TryGetValue(resultsId, out MeasurementResults data2))
                    {
                        return data2;
                    }
                }
            }
            return data;
        }

        public void Reset()
        {
            this._GetMeasResultsHeaderByTaskIdCache = new ConcurrentDictionary<int, MeasurementResults[]>();
            this._GetShortSensorsByTaskCache = new ConcurrentDictionary<int, ShortSensor[]>();
            this._shortSensors = null;
            this._GetMeasTaskHeaderByIdCache = new ConcurrentDictionary<int, MeasTaskViewModel>();
            this._GetStationDataForMeasurementsByTaskIdCache = new ConcurrentDictionary<int, StationDataForMeasurements[]>();
            this._GetSensorByIdCache = new ConcurrentDictionary<int, Sensor>();
            this._GetResMeasStationHeaderByResIdCache = new ConcurrentDictionary<int, ResultsMeasurementsStation[]>();
            this._GetMeasurementResultByResIdCache = new ConcurrentDictionary<int, MeasurementResults>();
        }
    }
}
