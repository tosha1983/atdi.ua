using System.Collections.Generic;
using Atdi.Platform.Logging;
using System;
using Atdi.DataModels.EntityOrm;
using Atdi.Modules.Sdrn.DeviceServer;
using System.Xml;
using System.Linq;
using Atdi.DataModels.Sdrn.DeviceServer.Processing;


namespace Atdi.AppUnits.Sdrn.DeviceServer.Repositories
{
    public sealed class TaskParametersByStringRepository : IRepository<TaskParameters, string>
    {
        private readonly ILogger _logger;
        private readonly ConfigRepositories _configRepositories;
        private long _fileCounter = 0;


        public TaskParametersByStringRepository(ConfigRepositories configRepositories, ILogger logger)
        {
            this._logger = logger;
            this._configRepositories = configRepositories;
        }

        public TaskParametersByStringRepository()
        {
        }


        public TaskParameters LoadObject(string additionalParameters)
        {
            TaskParameters taskParameters = null;
            try
            {
                var loadMessages = new LoadMessages<TaskParameters>(this._logger, this._configRepositories.FolderTaskParameters);
                taskParameters = loadMessages.GetMessage(additionalParameters);
            }
            catch (Exception e)
            {
                this._logger.Exception(Contexts.ThisComponent, e);
            }
            return taskParameters;
        }

        public string Create(TaskParameters item)
        {
            var pathTaskParameters = "";
            try
            {
                var additionalParameters = string.Format("{0}_{1}_", item.MeasurementType.ToString(), item.SDRTaskId);
                var messagesBus = new SaveMessages(++_fileCounter, additionalParameters, this._logger, this._configRepositories.FolderTaskParameters);
                pathTaskParameters = messagesBus.SaveObject<TaskParameters>(item);
            }
            catch (Exception e)
            {
                this._logger.Exception(Contexts.ThisComponent, e);
            }
            return pathTaskParameters;
        }

        public bool Update(TaskParameters item)
        {
            bool isSuccess = false;
            try
            {
                var additionalParameters = string.Format("{0}_{1}_", item.MeasurementType.ToString(), item.SDRTaskId);
                var loadMessages = new LoadMessages<TaskParameters>(this._logger, this._configRepositories.FolderTaskParameters);
                var fileNameFinded = loadMessages.GetFileName(additionalParameters);
                if (!string.IsNullOrEmpty(fileNameFinded))
                {
                    var messagesBus = new SaveMessages(_fileCounter, additionalParameters, this._logger, this._configRepositories.FolderTaskParameters);
                    isSuccess = messagesBus.UpdateObject<TaskParameters>(fileNameFinded, item);
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

        TaskParameters[] IRepository<TaskParameters, string>.LoadAllObjects()
        {
            TaskParameters[] taskParameters = null;
            try
            {
                var loadMessages = new LoadMessages<TaskParameters>(this._logger, this._configRepositories.FolderTaskParameters);
                taskParameters = loadMessages.GetAllMessages();
            }
            catch (Exception e)
            {
                this._logger.Exception(Contexts.ThisComponent, e);
            }
            return taskParameters;
        }

        public void RemoveOldObjects()
        {
            try
            {
                var loadMessages = new LoadMessages<TaskParameters>(this._logger, this._configRepositories.FolderTaskParameters);
                var taskParameters = loadMessages.GetAllMessages();
                if ((taskParameters != null) && (taskParameters.Length > 0))
                {
                    for (int i = 0; i < taskParameters.Length; i++)
                    {
                        if ((taskParameters[i].status == StatusTask.C.ToString()) || (taskParameters[i].status == StatusTask.Z.ToString()) || (taskParameters[i].StopTime < DateTime.Now))
                        {
                            var additionalParameters = string.Format("{0}_{1}_", taskParameters[i].MeasurementType.ToString(), taskParameters[i].SDRTaskId);
                            var fileName = loadMessages.GetFileName(additionalParameters);
                            var messagesBus = new SaveMessages(_fileCounter, fileName, this._logger, this._configRepositories.FolderTaskParameters);
                            messagesBus.DeleteObject(fileName);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                this._logger.Exception(Contexts.ThisComponent, e);
            }
        }

        public TaskParameters[] LoadObjectsWithRestrict(ref List<string> listRunTask)
        {
            List<TaskParameters> listTaskParameters = new List<TaskParameters>();
            var listSDRTaskId = new List<string>();
            try
            {
                var loadMessages = new LoadMessages<TaskParameters>(this._logger, this._configRepositories.FolderTaskParameters);
                var taskParameters = loadMessages.GetAllMessages();
                if ((taskParameters!=null) && (taskParameters.Length > 0))
                {
                    for (int i = 0; i < taskParameters.Length; i++)
                    {
                        var param = string.Format("{0}_{1}_", taskParameters[i].MeasurementType.ToString(), taskParameters[i].SDRTaskId);
                        if ((taskParameters[i].status != StatusTask.C.ToString()) && (taskParameters[i].StopTime >= DateTime.Now))
                        {
                            if (!listSDRTaskId.Contains(param))
                            {
                                listSDRTaskId.Add(param);
                                listTaskParameters.Add(taskParameters[i]);
                            }
                        }
                        else if ((taskParameters[i].status == StatusTask.C.ToString()) || (taskParameters[i].status == StatusTask.Z.ToString()) || (taskParameters[i].StopTime < DateTime.Now))
                        {
                            var fileName = loadMessages.GetFileName(param);
                            var messagesBus = new SaveMessages(_fileCounter, fileName, this._logger, this._configRepositories.FolderTaskParameters);
                            messagesBus.DeleteObject(fileName);
                            listRunTask.Remove(param);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                this._logger.Exception(Contexts.ThisComponent, e);
            }
            return listTaskParameters.ToArray();
        }
      
    }
}



