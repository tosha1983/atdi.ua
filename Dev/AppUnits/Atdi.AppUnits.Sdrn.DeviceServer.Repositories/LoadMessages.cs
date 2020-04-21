using System;
using System.Collections.Generic;
using System.Linq;
using Atdi.Platform.Logging;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;



namespace Atdi.AppUnits.Sdrn.DeviceServer.Repositories
{

    public class LoadMessages<T> : IDisposable
    {
        private bool disposedValue = false;
        private string[] _files;
        private readonly ILogger _logger;
        private string _messageFolder;



        public LoadMessages(ILogger logger, string messageFolder)
        {
            this._messageFolder = messageFolder;
            this._logger = logger;
        }

        private string[] ReadFiles()
        {
            var files = Directory.GetFiles(this._messageFolder, $"*{typeof(T).Name}.data", SearchOption.TopDirectoryOnly);
            if (files != null && files.Length > 0)
            {
                return files.OrderBy(s => s).ToArray();
            }
            return new string[] { };
        }

        private string ReadFile(string additionalParamsKey)
        {
            var files = Directory.GetFiles(this._messageFolder, $"*{additionalParamsKey}_{typeof(T).Name}.data", SearchOption.TopDirectoryOnly);
            if (files != null && files.Length > 0)
            {
                return files.OrderBy(s => s).ToArray()[0];
            }
            return null;
        }

        public string GetFileName(string additionalParams)
        {
            return ReadFile(additionalParams);
        }

        public string[] GetAllFiles()
        {
            return ReadFiles();
        }

        public T GetMessage(string additionalParams, out string fileNameFinded)
        {
            if (additionalParams == null)
            {
                throw new ArgumentNullException(nameof(additionalParams));
            }

            T messageData = default(T);

            try
            {
                var fileName = ReadFile(additionalParams);

                fileNameFinded = fileName;

                if (string.IsNullOrEmpty(fileName))
                {
                    return default(T);
                }
                messageData = this.ReadMessageData(fileName);
            }
            catch (Exception e)
            {
                this._logger.Exception(Contexts.ThisComponent, Categories.ErrorGetMessageFromDB, e);
                throw new InvalidOperationException($"Error get message from DB: {e.StackTrace}");
            }
            return messageData;
        }

        public T GetMessage(string additionalParams)
        {
            if (additionalParams == null)
            {
                throw new ArgumentNullException(nameof(additionalParams));
            }

            T messageData = default(T);

            try
            {
                var fileName = ReadFile(additionalParams);

                if (string.IsNullOrEmpty(fileName))
                {
                    return default(T);
                }
                messageData = this.ReadMessageData(fileName);
            }
            catch (Exception e)
            {
                this._logger.Exception(Contexts.ThisComponent, Categories.ErrorGetMessageFromDB, e);
                throw new InvalidOperationException($"Error get message from DB: {e.StackTrace}");
            }
            return messageData;
        }

        public T[] GetAllMessages()
        {
            var values = new List<T>();
            try
            {
                this._files = ReadFiles();
                for (int i = 0; i < this._files.Length; i++)
                {
                    var fileName = this._files[i];
                    if (string.IsNullOrEmpty(fileName))
                    {
                        break;
                    }
                    var messageData = this.ReadMessageData(fileName);
                    if (messageData != null)
                    {
                        values.Add(messageData);
                    }
                }
            }
            catch (Exception e)
            {
                this._logger.Exception(Contexts.ThisComponent, Categories.ErrorGetMessageFromDB, e);
                throw new InvalidOperationException($"Error read messages from DB: {e.StackTrace}");
            }
            return values.ToArray();
        }

        public T[] GetAllMessages(out string[] fileNames)
        {
            var values = new List<T>();
            try
            {
                this._files = ReadFiles();
                for (int i = 0; i < this._files.Length; i++)
                {
                    var fileName = this._files[i];
                    if (string.IsNullOrEmpty(fileName))
                    {
                        break;
                    }
                    var messageData = this.ReadMessageData(fileName);
                    if (messageData != null)
                    {
                        values.Add(messageData);
                    }
                }
                fileNames = this._files;
            }
            catch (Exception e)
            {
                this._logger.Exception(Contexts.ThisComponent, Categories.ErrorGetMessageFromDB, e);
                throw new InvalidOperationException($"Error read messages from DB: {e.StackTrace}");
            }
            return values.ToArray();
        }


        private T ReadMessageData(string fileName)
        {
            T dataobj = default(T);
            if (!File.Exists(fileName))
            {
                return default(T);
            }
            try
            {
                //lock (fileName)
                {
                    using (FileStream file = new FileStream(fileName, FileMode.OpenOrCreate, FileAccess.Read, FileShare.Read))
                    using (StreamReader streamReader = new StreamReader(file))
                    {
                        IFormatter formatter = new BinaryFormatter();
                        formatter.Binder = new LocalBinder();
                        dataobj = (T)formatter.Deserialize(streamReader.BaseStream);
                        streamReader.Close();
                    }
                }
            }
            catch (Exception e)
            {
                this._logger.Exception(Contexts.ThisComponent, Categories.ErrorGetMessageFromDB, e);
                throw new InvalidOperationException($"Error read message from DB: {fileName} {e.StackTrace}");
            }
            return dataobj;
        }



        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {

                }
                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }

    }
}
