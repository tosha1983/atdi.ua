using System;
using Atdi.Platform.Logging;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace Atdi.AppUnits.Sdrn.DeviceServer.Repositories
{
    public class SaveMessages : IDisposable
    {
        private bool disposedValue = false;
        private long _fileCounter = 0;
        private string _additionalParams = null;
        private readonly string _messageFolder;
        private readonly ILogger _logger;


        public SaveMessages(long fileCounter, string additionalParams, ILogger logger, string messageFolder)
        {
            this._fileCounter = fileCounter;
            this._messageFolder = messageFolder;
            this._additionalParams = additionalParams;
            this._logger = logger;
        }


        public string SaveObject<T>(T receivedMessage)
        {
            try
            {
                var fileName = MakeFileName(typeof(T).Name.ToString());
                var fullPath = Path.Combine(this._messageFolder, fileName);
                lock (fileName)
                {
                    IFormatter formatter = new BinaryFormatter();
                    using (MemoryStream stream = new MemoryStream())
                    {
                        formatter.Serialize(stream, receivedMessage);
                        File.WriteAllBytes(fullPath, stream.ToArray());
                    }
                }
                return fullPath;
            }
            catch (Exception e)
            {
                this._logger.Exception(Contexts.ThisComponent, Categories.ObjectWasNotSaved, e);
                throw new InvalidOperationException($"The object was not saved to {this._messageFolder}", e);
            }
        }

        public bool DeleteObject(string fileName)
        {
            if (fileName == null)
            {
                throw new ArgumentNullException(nameof(fileName));
            }

            try
            {
                if (File.Exists(fileName))
                {
                    lock (fileName)
                    {
                        File.Delete(fileName);
                    }
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception e)
            {
                this._logger.Exception(Contexts.ThisComponent, Categories.TheFileWasNotDeleted, e);
                throw new InvalidOperationException($"The file {fileName} was not deleted", e);
            }
        }

        public bool UpdateObject<T>(string fileName, T receivedMessage)
        {
            if (fileName == null)
            {
                throw new ArgumentNullException(nameof(fileName));
            }

            try
            {
                lock (fileName)
                {
                    var fullPath = Path.Combine(this._messageFolder, fileName);
                    if (File.Exists(fullPath))
                    {
                        IFormatter formatter = new BinaryFormatter();
                        using (MemoryStream stream = new MemoryStream())
                        {
                            formatter.Serialize(stream, receivedMessage);
                            File.WriteAllBytes(fullPath, stream.ToArray());
                        }
                    }
                }
                return true;
            }
            catch (Exception e)
            {
                this._logger.Exception(Contexts.ThisComponent, Categories.TheFileWasNotUpdated, e);
                throw new InvalidOperationException($"The file {fileName} was not updated", e);
            }
        }

        private string MakeFileName(string messageType)
        {
            var id = System.Threading.Thread.CurrentThread.ManagedThreadId;
            var d = DateTime.Now;

            var timeFormat = "yyyyMMdd_HHmmss";
            var timeString = d.ToString(timeFormat);


            var timeFormat3 = "FFFFFFF";
            var timeString3 = d.ToString(timeFormat3).PadRight(timeFormat3.Length, '0');

            return timeString + "_" + timeString3 + "_" + id.ToString().PadLeft(3, '0') + "_" + (_fileCounter).ToString().PadLeft(10, '0') + "_" + this._additionalParams +"_"+ messageType + ".data";
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
