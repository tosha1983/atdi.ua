using System.Collections.Concurrent;
using System.Linq;
using SVC = XICSM.ICSControlClient.WcfServiceClients;
using Atdi.Contracts.WcfServices.Sdrn.Server;
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

        private static readonly DataStore Instance = new DataStore();

        private ConcurrentDictionary<long, MeasurementResults[]> _getMeasResultsHeaderByTaskIdCache;
        private ConcurrentDictionary<long, ShortSensor[]> _getShortSensorsByTaskCache;
        private ShortSensor[] _shortSensors;

        private ConcurrentDictionary<long, MeasTaskViewModel> _getMeasTaskHeaderByIdCache;
        private ConcurrentDictionary<long, StationDataForMeasurements[]> _getStationDataForMeasurementsByTaskIdCache;
        private ConcurrentDictionary<long, Sensor> _getSensorByIdCache;
        private ConcurrentDictionary<long, ResultsMeasurementsStation[]> _getResMeasStationHeaderByResIdCache;
        private ConcurrentDictionary<long, MeasurementResults> _getMeasurementResultByResIdCache;
        private ConcurrentDictionary<long, MeasurementResults> _getFullMeasurementResultByResIdCache;

        public static DataStore GetStore()
        {
            return DataStore.Instance;
        }

        private DataStore()
        {
            this.Reset();
        }

        public MeasurementResults[] GetMeasResultsHeaderByTaskId(long taskId)
        {
            
            var cache = this._getMeasResultsHeaderByTaskIdCache;

            if (!cache.TryGetValue(taskId, out MeasurementResults[] data))
            {
                var description = $"Measurement Results for task with ID #{taskId}";
                this.OnBeginInvoke?.Invoke(description);
                try
                {
                    data = SVC.SdrnsControllerWcfClient.GetMeasResultsHeaderByTaskId(taskId);
                }
                finally
                {
                    this.OnEndInvoke?.Invoke(description);
                }

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

        private ShortSensor[] GetShortSensors()
        {
            if (this._shortSensors == null)
            {
                const string description = "Short info of all sensors";
                this.OnBeginInvoke?.Invoke(description);
                this._shortSensors = SVC.SdrnsControllerWcfClient.GetShortSensors();
                this.OnEndInvoke?.Invoke(description);
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

            var cache = this._getShortSensorsByTaskCache;

            if (!cache.TryGetValue(measTask.Id, out var data))
            {
                if (measTask.Sensors != null && measTask.Sensors.Length > 0)
                {
                    data = allSensors
                        .Where(sdrSensor => measTask.Sensors
                                .FirstOrDefault(s => s.SensorId.Value == sdrSensor.Id.Value) != null
                            )
                        .ToArray();
                }
                else
                {
                    data = new ShortSensor[] { };
                }

                if (!cache.TryAdd(measTask.Id, data))
                {
                    if (cache.TryGetValue(measTask.Id, out var data2))
                    {
                        return data2;
                    }
                }
            }
            return data;
        }

        public MeasTaskViewModel GetMeasTaskHeaderById(long taskId)
        {
            var cache = this._getMeasTaskHeaderByIdCache;

            if (taskId == 0)
            {
                return null;
            }

            if (!cache.TryGetValue(taskId, out MeasTaskViewModel data))
            {
                var description = $"Measurement Task Header with ID #{taskId}";
                this.OnBeginInvoke?.Invoke(description);
                try
                {
                    var task = SVC.SdrnsControllerWcfClient.GetMeasTaskHeaderById(taskId);
                    data = Mappers.Map(task);
                }
                finally
                {
                    this.OnEndInvoke?.Invoke(description);
                }

                
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

        public StationDataForMeasurements[] GetStationDataForMeasurementsByTaskId(long taskId)
        {
            var cache = this._getStationDataForMeasurementsByTaskIdCache;

            if (!cache.TryGetValue(taskId, out var data))
            {
                var description = $"Station Data For Measurements by task with ID #{taskId}";
                this.OnBeginInvoke?.Invoke(description);
                try
                {
                    data = SVC.SdrnsControllerWcfClient.GetStationDataForMeasurementsByTaskId(taskId);
                }
                finally
                {
                    this.OnEndInvoke?.Invoke(description);
                }

                if (!cache.TryAdd(taskId, data))
                {
                    if (cache.TryGetValue(taskId, out var data2))
                    {
                        return data2;
                    }
                }
            }
            return data;
        }

        public Sensor GetSensorById(long sensorId)
        {
            var cache = this._getSensorByIdCache;

            if (sensorId == 0)
            {
                return null;
            }

            if (!cache.TryGetValue(sensorId, out var data))
            {
                var description = $"Sensor Full Info with ID #{sensorId}";
                this.OnBeginInvoke?.Invoke(description);
                try
                {
                    data = SVC.SdrnsControllerWcfClient.GetSensorById(sensorId);
                }
                finally
                {
                    this.OnEndInvoke?.Invoke(description);
                }

                if (!cache.TryAdd(sensorId, data))
                {
                    if (cache.TryGetValue(sensorId, out var data2))
                    {
                        return data2;
                    }
                }
            }
            return data;
        }

        public ResultsMeasurementsStation[] GetResMeasStationHeaderByResId(long resultsId)
        {
            var cache = this._getResMeasStationHeaderByResIdCache;

            if (!cache.TryGetValue(resultsId, out ResultsMeasurementsStation[] data))
            {
                var description = $"Result Measurement Station Header by the result set with ID #{resultsId}";
                this.OnBeginInvoke?.Invoke(description);
                try
                {
                    data = SVC.SdrnsControllerWcfClient.GetResMeasStationHeaderByResId(resultsId);
                }
                finally
                {
                    this.OnEndInvoke?.Invoke(description);
                }

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

        public MeasurementResults GetMeasurementResultByResId(long resultsId)
        {
            var cache = this._getMeasurementResultByResIdCache;

            // ReSharper disable once InvertIf
            if (!cache.TryGetValue(resultsId, out var data))
            {
                var description = $"Measurement Results by the result set with ID #{resultsId}";
                this.OnBeginInvoke?.Invoke(description);
                try
                {
                    data = SVC.SdrnsControllerWcfClient.GetMeasurementResultByResId(resultsId);
                }
                finally
                {
                    this.OnEndInvoke?.Invoke(description);
                }

                // ReSharper disable once InvertIf
                if (!cache.TryAdd(resultsId, data))
                {
                    if (cache.TryGetValue(resultsId, out var data2))
                    {
                        return data2;
                    }
                }
            }
            return data;
        }

        public MeasurementResults GetFullMeasurementResultByResId(long resultsId)
        {
            var cache = this._getFullMeasurementResultByResIdCache;

            if (!cache.TryGetValue(resultsId, out var data))
            {
                var description = $"Full Measurement Results by the result set with ID #{resultsId}";
                this.OnBeginInvoke?.Invoke(description);
                try
                {
                    data = SVC.SdrnsControllerWcfClient.GetMeasurementResultByResId(resultsId, null, null);
                }
                finally
                {
                    this.OnEndInvoke?.Invoke(description);
                }

                if (!cache.TryAdd(resultsId, data))
                {
                    if (cache.TryGetValue(resultsId, out var data2))
                    {
                        return data2;
                    }
                }
            }
            return data;
        }

        public Emitting[] GetEmittingsBy(long[] stations, string tableName)
        {
            const string description = "Emissions by the stations";
            try
            {
                this.OnBeginInvoke?.Invoke(description);
                var data = SVC.SdrnsControllerWcfClient.GetEmittingsByIcsmId(stations, tableName);
                return data;
            }
            finally
            {
                this.OnEndInvoke?.Invoke(description);
            }
        }

        public void Reset()
        {
            this._getMeasResultsHeaderByTaskIdCache = new ConcurrentDictionary<long, MeasurementResults[]>();
            this._getShortSensorsByTaskCache = new ConcurrentDictionary<long, ShortSensor[]>();
            this._shortSensors = null;
            this._getMeasTaskHeaderByIdCache = new ConcurrentDictionary<long, MeasTaskViewModel>();
            this._getStationDataForMeasurementsByTaskIdCache = new ConcurrentDictionary<long, StationDataForMeasurements[]>();
            this._getSensorByIdCache = new ConcurrentDictionary<long, Sensor>();
            this._getResMeasStationHeaderByResIdCache = new ConcurrentDictionary<long, ResultsMeasurementsStation[]>();
            this._getMeasurementResultByResIdCache = new ConcurrentDictionary<long, MeasurementResults>();
            this._getFullMeasurementResultByResIdCache = new ConcurrentDictionary<long, MeasurementResults>();
        }
    }
}
