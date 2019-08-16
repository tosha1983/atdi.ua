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
    public sealed class MeasResultsByStringRepository : IRepository<DM.MeasResults, string>
    {
        private readonly ILogger _logger;
        private readonly ConfigRepositories _configRepositories;
        private long _fileCounter = 0;


        public MeasResultsByStringRepository(ConfigRepositories configRepositories, ILogger logger)
        {
            this._logger = logger;
            this._configRepositories = configRepositories;
        }

        public MeasResultsByStringRepository()
        {

        }

        public void RemoveOldObjects()
        {
            throw new NotImplementedException();
        }

        public DM.MeasResults LoadObject(string additionalParameters)
        {
            throw new NotImplementedException();
        }

        public string Create(DM.MeasResults item)
        {
            var pathResults = "";
            try
            {
                var additionalParameters = string.Format("{0}_{1}_", item.Measurement.ToString(), item.TaskId);
                var messagesBus = new SaveMessages(++_fileCounter, additionalParameters, this._logger, this._configRepositories.FolderMeasResults);
                pathResults = messagesBus.SaveObject<DM.MeasResults>(item);
            }
            catch (Exception e)
            {
                this._logger.Exception(Contexts.ThisComponent, e);
            }
            return pathResults;
        }

        public bool Update(DM.MeasResults item)
        {
            throw new NotImplementedException();
        }

        public bool Delete(string fileName)
        {
            SaveMessages saveMessages = new SaveMessages(_fileCounter, null, this._logger, this._configRepositories.FolderMeasResults);
            return saveMessages.DeleteObject(fileName);
        }

        public void Dispose()
        {
            //throw new NotImplementedException();
        }

        DM.MeasResults[] IRepository<DM.MeasResults, string>.LoadAllObjects()
        {
            DM.MeasResults[] measResults = null;
            try
            {
                var loadMessages = new LoadMessages<DM.MeasResults>(this._logger, this._configRepositories.FolderMeasResults);
                measResults = loadMessages.GetAllMessages();
            }
            catch (Exception e)
            {
                this._logger.Exception(Contexts.ThisComponent, e);
            }
            return measResults;
        }


        public DM.MeasResults[] LoadObjectsWithRestrict(ref List<string> listFileNames)
        {
            string[] fileNames = null;
            DM.MeasResults[] measResults = null;
            try
            {
                var loadMessages = new LoadMessages<DM.MeasResults>(this._logger, this._configRepositories.FolderMeasResults);
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



