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
    public sealed class DeviceCommandResultByStringRepository : IRepository<DM.DeviceCommandResult, string>
    {
        private readonly ILogger _logger;
        private readonly ConfigRepositories _configRepositories;
        private long _fileCounter = 0;


        public DeviceCommandResultByStringRepository(ConfigRepositories configRepositories, ILogger logger)
        {
            this._logger = logger;
            this._configRepositories = configRepositories;
        }

        public DeviceCommandResultByStringRepository()
        {

        }


        public DM.DeviceCommandResult LoadObject(string additionalParameters)
        {
            throw new NotImplementedException();
        }

        public string Create(DM.DeviceCommandResult item)
        {
            var pathResults = "";
            try
            {
                var additionalParameters = item.CommandId+"_"+ item.CustTxt1+"_";
                var messagesBus = new SaveMessages(++_fileCounter, additionalParameters, this._logger, this._configRepositories.DeviceCommandResult);
                pathResults = messagesBus.SaveObject<DM.DeviceCommandResult>(item);
            }
            catch (Exception e)
            {
                this._logger.Exception(Contexts.ThisComponent, e);
            }
            return pathResults;
        }

        public bool Update(DM.DeviceCommandResult item)
        {
            throw new NotImplementedException();
        }

        public bool Delete(string fileName)
        {
            SaveMessages saveMessages = new SaveMessages(_fileCounter, null, this._logger, this._configRepositories.DeviceCommandResult);
            return saveMessages.DeleteObject(fileName);
        }

        public void Dispose()
        {
            //throw new NotImplementedException();
        }

        DM.DeviceCommandResult[] IRepository<DM.DeviceCommandResult, string>.LoadAllObjects()
        {
            DM.DeviceCommandResult[] measResults = null;
            try
            {
                var loadMessages = new LoadMessages<DM.DeviceCommandResult>(this._logger, this._configRepositories.DeviceCommandResult);
                measResults = loadMessages.GetAllMessages();
            }
            catch (Exception e)
            {
                this._logger.Exception(Contexts.ThisComponent, e);
            }
            return measResults;
        }


        public DM.DeviceCommandResult[] LoadObjectsWithRestrict(ref List<string> listFileNames)
        {
            string[] fileNames = null;
            DM.DeviceCommandResult[] measResults = null;
            try
            {
                var loadMessages = new LoadMessages<DM.DeviceCommandResult>(this._logger, this._configRepositories.DeviceCommandResult);
                measResults = loadMessages.GetAllMessages(out fileNames);
                listFileNames = fileNames.ToList();
            }
            catch (Exception e)
            {
                this._logger.Exception(Contexts.ThisComponent, e);
            }
            return measResults;
        }
      
    }
}



