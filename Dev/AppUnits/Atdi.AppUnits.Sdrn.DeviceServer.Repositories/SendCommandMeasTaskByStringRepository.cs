using System.Collections.Generic;
using Atdi.Platform.Logging;
using System;
using Atdi.DataModels.EntityOrm;
using Atdi.Modules.Sdrn.DeviceServer;
using System.Xml;
using System.Linq;
using Atdi.DataModels.Sdrn.DeviceServer.Processing;
using DM = Atdi.DataModels.Sdrns.Device;


namespace Atdi.AppUnits.Sdrn.DeviceServer.Repositories
{
    public sealed class SendCommandMeasTaskByStringRepository : IRepository<DM.DeviceCommand, string>
    {
        private readonly ILogger _logger;
        private readonly ConfigRepositories _configRepositories;
        private long _fileCounter = 0;


        public SendCommandMeasTaskByStringRepository(ConfigRepositories configRepositories, ILogger logger)
        {
            this._logger = logger;
            this._configRepositories = configRepositories;
        }

        public SendCommandMeasTaskByStringRepository()
        {
        }


        public DM.DeviceCommand LoadObject(string additionalParameters)
        {
            DM.DeviceCommand taskParameters = null;
            try
            {
                var loadMessages = new LoadMessages<DM.DeviceCommand>(this._logger, this._configRepositories.FolderDeviceCommand);
                taskParameters = loadMessages.GetMessage(additionalParameters);
            }
            catch (Exception e)
            {
                this._logger.Exception(Contexts.ThisComponent, e);
            }
            return taskParameters;
        }

        public string Create(DM.DeviceCommand item)
        {
            var pathDeviceCommand = "";
            try
            {
                var additionalParameters = item.CommandId + "_" + item.CustTxt1 + "_";
                var messagesBus = new SaveMessages(++_fileCounter, additionalParameters, this._logger, this._configRepositories.FolderDeviceCommand);
                pathDeviceCommand = messagesBus.SaveObject<DM.DeviceCommand>(item);
            }
            catch (Exception e)
            {
                this._logger.Exception(Contexts.ThisComponent, e);
            }
            return pathDeviceCommand;
        }

        public bool Update(DM.DeviceCommand item)
        {
            bool isSuccess = false;
            try
            {
                var additionalParameters = item.CommandId + "_" + item.CustTxt1 + "_";
                var loadMessages = new LoadMessages<DM.DeviceCommand>(this._logger, this._configRepositories.FolderDeviceCommand);
                var fileNameFinded = loadMessages.GetFileName(additionalParameters);
                if (!string.IsNullOrEmpty(fileNameFinded))
                {
                    var messagesBus = new SaveMessages(_fileCounter, additionalParameters, this._logger, this._configRepositories.FolderDeviceCommand);
                    isSuccess = messagesBus.UpdateObject<DM.DeviceCommand>(fileNameFinded, item);
                }
            }
            catch (Exception e)
            {
                isSuccess = false;
                this._logger.Exception(Contexts.ThisComponent, e);
            }
            return isSuccess;
        }

        public bool Delete(string id)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            //throw new NotImplementedException();
        }

        DM.DeviceCommand[] IRepository<DM.DeviceCommand, string>.LoadAllObjects()
        {
            DM.DeviceCommand[] taskParameters = null;
            try
            {
                var loadMessages = new LoadMessages<DM.DeviceCommand>(this._logger, this._configRepositories.FolderDeviceCommand);
                taskParameters = loadMessages.GetAllMessages();
                if ((taskParameters != null) && (taskParameters.Length > 0))
                {
                    for (int i = 0; i < taskParameters.Length; i++)
                    {
                        var item = taskParameters[i];
                        var additionalParameters = item.CommandId + "_" + item.CustTxt1 + "_";
                        var fileName = loadMessages.GetFileName(additionalParameters);
                        var messagesBus = new SaveMessages(_fileCounter, fileName, this._logger, this._configRepositories.FolderDeviceCommand);
                        messagesBus.DeleteObject(fileName);
                    }
                }

            }
            catch (Exception e)
            {
                this._logger.Exception(Contexts.ThisComponent, e);
            }
            return taskParameters;
        }


        public DM.DeviceCommand[] LoadObjectsWithRestrict(ref List<string> listRunTask)
        {
             throw new NotImplementedException();
        }
      
    }
}



