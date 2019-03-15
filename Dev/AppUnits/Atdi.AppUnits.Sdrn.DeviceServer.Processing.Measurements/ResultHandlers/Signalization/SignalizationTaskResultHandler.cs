using Atdi.Contracts.Sdrn.DeviceServer;
using Atdi.DataModels.Sdrn.DeviceServer.Commands;
using Atdi.DataModels.Sdrn.DeviceServer.Commands.Results;
using Atdi.DataModels.Sdrn.DeviceServer.Processing;
using DM = Atdi.DataModels.Sdrns.Device;
using System;
using System.Linq;
using System.Collections.Generic;
using Atdi.DataModels.Sdrn.DeviceServer;
using Atdi.DataModels.Sdrns.Device;
using Atdi.Common;
using Atdi.Platform.Logging;
using System.Threading;
using Atdi.Contracts.Api.Sdrn.MessageBus;
using Atdi.AppUnits.Sdrn.DeviceServer.Messaging.Convertor;


namespace Atdi.AppUnits.Sdrn.DeviceServer.Processing.Measurements
{
    public class SignalizationTaskResultHandler : IResultHandler<MesureTraceCommand, MesureTraceResult, SignalizationTask, SignalizationProcess>
    {

        private readonly ILogger _logger;
        private readonly IProcessingDispatcher _processingDispatcher;
        private readonly ITaskStarter _taskStarter;
        private readonly ITimeService _timeService;
        private readonly IWorkScheduler _workScheduler;

        public SignalizationTaskResultHandler(ILogger logger,
            IProcessingDispatcher processingDispatcher,
            ITaskStarter taskStarter,
            IWorkScheduler workScheduler,
            ITimeService timeService)
        {
            this._logger = logger;
            this._processingDispatcher = processingDispatcher;
            this._taskStarter = taskStarter;
            this._timeService = timeService;
            this._workScheduler = workScheduler;
        }

        public void Handle(MesureTraceCommand command, MesureTraceResult result, DataModels.Sdrn.DeviceServer.ITaskContext<SignalizationTask, SignalizationProcess> taskContext)
        {
            if (result != null)
            {
                try
                {
                    MeasResults measResults = new MeasResults();

                    Emitting emitting = null;
                    Emitting emittingSummary = null;
                    if (!CheckMesureTraceResult(result))
                    {
                        //Первый расчет ?
                        if (result.PartIndex==0)
                        {
                            CalcReferenceLevels(result, taskContext);
                            emitting = CalcSearchInterruption(result, taskContext);
                        }
                        else
                        {
                            emitting = CalcSearchInterruption(result, taskContext);
                        }

                    }
                    else
                    {
                        emitting = CalcSearchingEmitting(result, taskContext);
                    }

                    emittingSummary = CalcGroupingEmitting(result, taskContext);

                    if (NeedCalcSpectrum(emittingSummary))
                    {
                        //ПРоверка и запуск задачи BandWidth на одном из этапов 
                        /////////////////////////////////////////////////////////
                        //
                        //  Формирование нового таска BW
                        //
                        /////////////////////////////////////////////////////////

                        var bandWidthProcess = _processingDispatcher.Start<BandWidthProcess>(taskContext.Process);

                        var bandWidtTask = new BandWidthTask();
                        bandWidtTask.taskParameters = taskContext.Task.taskParameters;
                        bandWidtTask.mesureTraceParameter = taskContext.Task.mesureTraceParameter;

                        _taskStarter.RunParallel(bandWidtTask, bandWidthProcess, taskContext);

                        MeasBandwidthResult outResultData = null;
                        //получение результата с BandWidthTaskResultHandler
                        bool isDown = taskContext.WaitEvent<MeasBandwidthResult>(out outResultData, 1000);
                        if (isDown == true) // таймут - результатов нет
                        {

                        }
                    }

                    
                   

                    // Отправка результата в Task Handler
                    taskContext.SetEvent(measResults);
                }
                catch (Exception ex)
                {
                    taskContext.SetEvent<ExceptionProcessSignalization>(new ExceptionProcessSignalization(CommandFailureReason.Exception, ex));
                }
            }
        }

        /// <summary>
        /// Метод, возвращающий первичную оценку полученного результата и возвращает True если дальнейшее исследование имеет смысл и FALSE если дальнейший анализ не нужен
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        public bool CheckMesureTraceResult(MesureTraceResult result)
        {
            return true;
        }

        public Emitting CalcSearchingEmitting(MesureTraceResult result, DataModels.Sdrn.DeviceServer.ITaskContext<SignalizationTask, SignalizationProcess> taskContext)
        {
            return new Emitting();
        }

        public ReferenceLevels CalcReferenceLevels(MesureTraceResult result, DataModels.Sdrn.DeviceServer.ITaskContext<SignalizationTask, SignalizationProcess> taskContext)
        {
            return new ReferenceLevels();
        }

        public Emitting CalcSearchInterruption(MesureTraceResult result, DataModels.Sdrn.DeviceServer.ITaskContext<SignalizationTask, SignalizationProcess> taskContext)
        {
            return new Emitting();
        }

        public Emitting CalcGroupingEmitting(MesureTraceResult result, DataModels.Sdrn.DeviceServer.ITaskContext<SignalizationTask, SignalizationProcess> taskContext)
        {
            return new Emitting();
        }

        public bool NeedCalcSpectrum(Emitting emitting)
        {
            return true;
        }
    }
}
