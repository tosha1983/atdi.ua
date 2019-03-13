using Atdi.Contracts.Api.Sdrn.MessageBus;
using Atdi.Contracts.Sdrn.DeviceServer;
using Atdi.DataModels.Sdrn.DeviceServer.Processing;
using Atdi.DataModels.Sdrns.BusMessages.Server;
using Atdi.Platform.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DM = Atdi.DataModels.Sdrns.Device;
using Atdi.DataModels.EntityOrm;


namespace Atdi.AppUnits.Sdrn.DeviceServer.Messaging.Handlers
{
    class SendCommandHandler : MessageHandlerBase<DM.DeviceCommand, SendCommandMessage>
    {
        private readonly IProcessingDispatcher _processingDispatcher;
        private readonly ITimeService _timeService;
        private readonly ITaskStarter _taskStarter;
        private readonly ILogger _logger;
        private readonly IRepository<TaskParameters, int?> _repositoryTaskParametersByInt;
        private readonly IRepository<TaskParameters, string> _repositoryTaskParametersByString;
        private readonly IRepository<LastUpdate, int?> _repositoryLastUpdateByInt;

        public SendCommandHandler(ITimeService timeService,
            IProcessingDispatcher processingDispatcher,
            IRepository<TaskParameters, int?> repositoryTaskParametersByInt,
            IRepository<LastUpdate, int?> repositoryLastUpdateByInt,
            IRepository<TaskParameters, string> repositoryTaskParametersBystring,
            ITaskStarter taskStarter, ILogger logger)
        {
            this._processingDispatcher = processingDispatcher;
            this._timeService = timeService;
            this._taskStarter = taskStarter;
            this._logger = logger;
            this._repositoryTaskParametersByInt = repositoryTaskParametersByInt;
            this._repositoryTaskParametersByString = repositoryTaskParametersBystring;
            this._repositoryLastUpdateByInt = repositoryLastUpdateByInt;
        }

        public override void OnHandle(IReceivedMessage<DM.DeviceCommand> message)
        {
            _logger.Verbouse(Contexts.ThisComponent, Categories.Handling, Events.MessageIsBeingHandled.With(message.Token.Type));
            try
            {
                DM.DeviceCommand deviceCommand = message.Data;

                if (deviceCommand.CustNbr1 != null)
                {
                    int idTask = (int)deviceCommand.CustNbr1;
                    if (idTask > 0)
                    {
                        var taskParams = this._repositoryTaskParametersByString.LoadObject(idTask.ToString());
                        if (taskParams != null)
                        {
                            if (deviceCommand.Command == TypeMeasTask.RunMeasTask.ToString())
                            {
                                taskParams.status = StatusTask.A.ToString();
                            }
                            else if (deviceCommand.Command == TypeMeasTask.DelMeasTask.ToString())
                            {
                                taskParams.status = StatusTask.Z.ToString();
                            }
                            else if (deviceCommand.Command == TypeMeasTask.StopMeasTask.ToString())
                            {
                                taskParams.status = StatusTask.F.ToString();
                            }
                            // обновление TaskParameters в БД
                            this._repositoryTaskParametersByInt.Update(taskParams);

                            var lastUpdate = new LastUpdate()
                            {
                                TableName = "XBS_TASKPARAMETERS",
                                LastDateTimeUpdate = DateTime.Now,
                                Status = "N"
                            };
                            var allTablesLastUpdated = this._repositoryLastUpdateByInt.LoadAllObjects();
                            if ((allTablesLastUpdated != null) && (allTablesLastUpdated.Length > 0))
                            {
                                var listAlTables = allTablesLastUpdated.ToList();
                                var findTableProperties = listAlTables.Find(z => z.TableName == "XBS_TASKPARAMETERS");
                                if (findTableProperties != null)
                                {
                                    this._repositoryLastUpdateByInt.Update(lastUpdate);
                                }
                                else
                                {
                                    this._repositoryLastUpdateByInt.Create(lastUpdate);
                                }
                            }
                            else
                            {
                                this._repositoryLastUpdateByInt.Create(lastUpdate);
                            }
                            this._logger.Info(Contexts.ThisComponent, Categories.SendCommandHandlerHandlerStart, Events.UpdateTaskParameters);
                        }
                    }
                }
                message.Result = MessageHandlingResult.Confirmed;
            }
            catch (Exception e)
            {
                message.Result = MessageHandlingResult.Error;
                this._logger.Error(Contexts.ThisComponent, Exceptions.UnknownErrorsInSendCommandHandler, e.Message);
            }
        }
    }
}
