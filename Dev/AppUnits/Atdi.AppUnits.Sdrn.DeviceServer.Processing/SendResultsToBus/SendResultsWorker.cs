using Atdi.Common;
using Atdi.Contracts.Sdrn.DeviceServer;
using Atdi.DataModels.Sdrn.DeviceServer;
using Atdi.DataModels.Sdrn.DeviceServer.Commands;
using Atdi.DataModels.Sdrn.DeviceServer.Commands.Results;
using Atdi.DataModels.Sdrn.DeviceServer.Processing;
using Atdi.Platform.Logging;
using System;
using DM = Atdi.DataModels.Sdrns.Device;
using System.Threading;
using Atdi.Contracts.Api.Sdrn.MessageBus;
using Atdi.Platform.DependencyInjection;
using Atdi.DataModels.EntityOrm;
using System.Collections.Generic;



namespace Atdi.AppUnits.Sdrn.DeviceServer.Processing
{
    public class SendResultsWorker : ITaskWorker<SendResult, DispatchProcess, SingletonTaskWorkerLifetime>
    {
        private readonly IBusGate _busGate;
        private readonly ILogger _logger;
        private readonly ConfigProcessing _config;
        private readonly IRepository<DM.MeasResults, string> _measResultsByStringRepository;
        private readonly IRepository<DM.DeviceCommandResult, string> _repositoryDeviceCommandResult;

        public SendResultsWorker(
            ConfigProcessing config,
            ILogger logger,
            IRepository<DM.MeasResults, string> measResultsByStringRepository,
            IRepository<DM.DeviceCommandResult, string> repositoryDeviceCommandResult,
            IBusGate busGate)
        {

            this._logger = logger;
            this._busGate = busGate;
            this._config = config;
            this._measResultsByStringRepository = measResultsByStringRepository;
            this._repositoryDeviceCommandResult = repositoryDeviceCommandResult;
        }


        public void Run(ITaskContext<SendResult, DispatchProcess> context)
        {
            try
            {
                while (true)
                {
                    System.Threading.Thread.Sleep(this._config.SleepTimePeriodSendingBus_ms);
                    var allFileNamesSendMeasResults = new List<string>();
                    var resultsSendMeasResults = this._measResultsByStringRepository.LoadObjectsWithRestrict(ref allFileNamesSendMeasResults);
                    if (resultsSendMeasResults != null)
                    {
                        for (int i = 0; i < resultsSendMeasResults.Length; i++)
                        {
                            //Отправка результатов в шину 
                            IMessageToken messageToken = null;
                            try
                            {
                                var publisher = this._busGate.CreatePublisher("main");
                                messageToken = publisher.Send<DM.MeasResults>("SendMeasResults", resultsSendMeasResults[i]);
                                publisher.Dispose();
                            }
                            catch (Exception e)
                            {
                                messageToken = null;
                                _logger.Error(Contexts.SendResultsWorker, Categories.Processing, Exceptions.UnknownErrorSendResultsWorker, e.Message);
                                System.Threading.Thread.Sleep(this._config.SleepTimePeriodWaitingErrorSendingBus_ms);
                            }
                            if (messageToken != null)
                            {
                                if (!string.IsNullOrEmpty(messageToken.Id))
                                {
                                    this._measResultsByStringRepository.Delete(allFileNamesSendMeasResults[i]);
                                }
                            }
                        }
                    }


                    var allFileNamesSendCommandResult = new List<string>();
                    var resultsSendCommandResult = this._repositoryDeviceCommandResult.LoadObjectsWithRestrict(ref allFileNamesSendCommandResult);
                    if (resultsSendCommandResult != null)
                    {
                        for (int i = 0; i < resultsSendCommandResult.Length; i++)
                        {
                            //Отправка результатов в шину 
                            IMessageToken messageToken = null;
                            try
                            {
                                var publisher = this._busGate.CreatePublisher("main");
                                messageToken = publisher.Send<DM.DeviceCommandResult>("SendCommandResult", resultsSendCommandResult[i]);
                                publisher.Dispose();
                            }
                            catch (Exception e)
                            {
                                messageToken = null;
                                _logger.Error(Contexts.SendResultsWorker, Categories.Processing, Exceptions.UnknownErrorSendResultsWorker, e.Message);
                                System.Threading.Thread.Sleep(this._config.SleepTimePeriodWaitingErrorSendingBus_ms);
                            }
                            if (messageToken != null)
                            {
                                if (!string.IsNullOrEmpty(messageToken.Id))
                                {
                                    this._measResultsByStringRepository.Delete(allFileNamesSendCommandResult[i]);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                context.Abort(e);
            }
        }

      
    }
}
