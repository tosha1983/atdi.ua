using Atdi.Contracts.Api.EventSystem;
using Atdi.Contracts.CoreServices.DataLayer;
using Atdi.Contracts.CoreServices.EntityOrm;
using Atdi.Contracts.Sdrn.Server;
using Atdi.DataModels.DataConstraint;
using Atdi.DataModels.Sdrns;
using Atdi.Platform;
using Atdi.Platform.Logging;
using MD = Atdi.DataModels.Sdrns.Server.Entities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Atdi.DataModels.Sdrns.Server.Events;

namespace Atdi.AppUnits.Sdrn.AggregationServer.PrimaryHandlers
{
    public class MeasResultWorker : IDisposable
    {
        private readonly object _locker = new object();

        private readonly AppComponentConfig _config;
        private readonly ILogger _logger;
        private readonly IEventEmitter _eventEmitter;
        private readonly IQueryExecutor _queryExecutor;
        private readonly IDataLayer<EntityDataOrm> _dataLayer;
        private readonly ISdrnServerEnvironment _environment;
        private volatile bool _isDisposing;
        private Thread _thread;
        private CancellationTokenSource _tokenSource;
        private readonly int _timeout = 3600000;
        private readonly int _qtyDayAwaitNextResult = 1;

        public MeasResultWorker(AppComponentConfig config, IEventEmitter eventEmitter, IDataLayer<EntityDataOrm> dataLayer, ILogger logger, ISdrnServerEnvironment environment)
        {
            this._config = config;
            this._logger = logger;
            this._dataLayer = dataLayer;
            this._environment = environment;
            this._eventEmitter = eventEmitter;
            this._queryExecutor = this._dataLayer.Executor<SdrnServerDataContext>();
            this._timeout = config.MeasResultSignalizationWorkerTimeout ?? 3600000;
            this._qtyDayAwaitNextResult = config.MeasResultSignalizationWorkerQtyDayAwaitNextResult ?? 1;
            this._tokenSource = new CancellationTokenSource();
        }
        public void Dispose()
        {
            if (!_isDisposing)
            {
                _isDisposing = true;
                _thread.Abort();
            }
        }
        public void Run()
        {
            this._thread = new Thread(this.Process)
            {
                Name = $"ATDI.Sdrn.AggregationServer.MeasResultWorker",
                Priority = ThreadPriority.Lowest
            };

            this._thread.Start();
        }

        private void Process()
        {
            try
            {
                while (!_tokenSource.Token.IsCancellationRequested)
                {
                    _logger.Debug(Contexts.ThisComponent, Categories.EventProcessing, "Start processing MeasResultWorker");
                    this.SendMeasResult();
                    _logger.Debug(Contexts.ThisComponent, Categories.EventProcessing, "Start processing MeasResultWorker");
                    Thread.Sleep(_timeout);
                }
            }
            catch (ThreadAbortException)
            {
                Thread.ResetAbort();
                if (_isDisposing)
                {
                    // this is normal process
                }
            }
            catch (Exception e)
            {
                this._logger.Exception(Contexts.ThisComponent, Categories.Processing, e, this);
            }
        }
        private void SendMeasResult()
        {
            var measResults = new List<MyMeasResult>();
            var lastDate = DateTime.Today;
            var lastWorkedDate = DateTime.Today;
            long lastSensor = 0;
            int i = 0;
            var builderResMeas = this._dataLayer.GetBuilder<MD.IResMeasSignaling>().From();
            builderResMeas.Select(c => c.RES_MEAS.TimeMeas, c => c.RES_MEAS.Id, c => c.RES_MEAS.SUBTASK_SENSOR.SENSOR.Id);
            builderResMeas.Where(c => c.IsSend, ConditionOperator.Equal, false);
            builderResMeas.Where(c => c.RES_MEAS.TypeMeasurements, ConditionOperator.Equal, MeasurementType.Signaling.ToString());
            builderResMeas.Where(c => c.RES_MEAS.TimeMeas, ConditionOperator.LessThan, DateTime.Now);
            builderResMeas.OrderByAsc(c => c.RES_MEAS.SUBTASK_SENSOR.SENSOR.Id);
            builderResMeas.OrderByDesc(c => c.RES_MEAS.TimeMeas);
            this._queryExecutor.Fetch(builderResMeas, readerResMeas =>
            {
                while (readerResMeas.Read())
                {
                    var timeMeas = (readerResMeas.GetValue(c => c.RES_MEAS.TimeMeas));
                    if (timeMeas.HasValue)
                        measResults.Add(new MyMeasResult() { Id = readerResMeas.GetValue(c => c.RES_MEAS.Id), SensorId = readerResMeas.GetValue(c => c.RES_MEAS.SUBTASK_SENSOR.SENSOR.Id), MeasDate = timeMeas.Value });
                }
                return true;
            });

            foreach (var measResult in measResults)
            {
                if (lastSensor != measResult.SensorId)
                {
                    lastSensor = measResult.SensorId;
                    i = 0;
                    lastDate = DateTime.Today;
                    lastWorkedDate = DateTime.Today;
                }
                else
                    continue;

                if (i == 0)
                    lastDate = measResult.MeasDate.Date;
                i++;

                if ((measResult.MeasDate.Date == lastDate && measResult.MeasDate > DateTime.Now.AddDays(-this._qtyDayAwaitNextResult)) || measResult.MeasDate.Date == lastWorkedDate)
                    continue;
                else
                {
                    lastWorkedDate = measResult.MeasDate.Date;

                    var busEvent = new SGMeasResultAggregated($"OnSGMeasResultAggregated", "OnSGMeasResultAppeared") { MeasResultId = measResult.Id };
                    _eventEmitter.Emit(busEvent);
                    _logger.Debug(Contexts.ThisComponent, Categories.Processing, "MeasResultWorker - SendEvent OnSGMeasResultAggregated, ResultId = " + measResult.Id.ToString());

                    using (var scope = this._dataLayer.CreateScope<SdrnServerDataContext>())
                    {
                        var builderUpdateIResMeas = this._dataLayer.GetBuilder<MD.IResMeasSignaling>().Update();
                        builderUpdateIResMeas.SetValue(c => c.IsSend, true);
                        builderUpdateIResMeas.Where(c => c.RES_MEAS.TimeMeas, ConditionOperator.Between, lastWorkedDate.Date, new DateTime(lastWorkedDate.Year, lastWorkedDate.Month, lastWorkedDate.Day, 23, 59, 59));
                        builderUpdateIResMeas.Where(c => c.RES_MEAS.SUBTASK_SENSOR.SENSOR.Id, ConditionOperator.Equal, lastSensor);
                        scope.Executor.Execute(builderUpdateIResMeas);
                    }
                }
            }
        }
    }
    class MyMeasResult
    {
        public long Id;
        public long SensorId;
        public DateTime MeasDate;
    }
}
