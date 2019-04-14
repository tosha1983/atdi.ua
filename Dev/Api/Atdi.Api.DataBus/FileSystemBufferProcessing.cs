using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using Atdi.DataModels.Api.DataBus;
using System.IO;
using System.Diagnostics;

namespace Atdi.Api.DataBus
{
    class FileSystemBufferProcessing : IBufferProcessing
    {
        private readonly IBusConfig _config;
        private readonly AmqpPublisher _amqpPublisher;
        private EventWaitHandle _eventWaiter;
        private readonly BusLogger _logger;
        private readonly MessagePacker _messagePacker;
        private Stopwatch _timer;
        private volatile int _fileCounter = 0;
        private Task _task;
        private CancellationTokenSource _tokenSource;

        private string[] _files;
        private int _fileIndex = -1;

        /// <summary>
        /// Время ожидания завершения таска процесса после полпытке его отменить
        /// </summary>
        private int _canceledTaskTimeout = 30 * 1000;
        /// <summary>
        /// Таймаут ожидания файла(ов) на диске
        /// </summary>
        private int _filesReadingTimeout = 10 * 1000;
        /// <summary>
        /// Таймаут между неудачными попытками процесинга файла
        /// </summary>
        private int _processingFileTimeout = 5 * 1000;
        /// <summary>
        /// Таймаут между неудачными попытками отпраки сообщения
        /// </summary>
        private int _sendingTimeout = 5 * 1000;
        /// <summary>
        /// Таймаут между неудачными попытками удаления файла
        /// </summary>
        private int _fileDeletingTimeout = 30 * 1000;

        public FileSystemBufferProcessing(IBusConfig config, AmqpPublisher amqpPublisher, MessagePacker messagePacker, BusLogger logger)
        {
            this._config = config;
            this._amqpPublisher = amqpPublisher;
            this._messagePacker = messagePacker;
            this._logger = logger;

            _logger.Verbouse(BusContexts.Initialization, $"The Buffer object was created: {_config.Buffer}", this);
        }

        public void Save(Message message)
        {
            var fileName = string.Empty;

            if (this._config.Buffer.ContentType == ContentType.Json)
            {
                var jsonConvertor = this._messagePacker.GetConvertor(ContentType.Json);
                var json = (string)jsonConvertor.Serialize(message, typeof(Message));

                fileName = MakeFileName("json");
                var fullPath = Path.Combine(this._config.Buffer.OutboxFolder, fileName);

                File.WriteAllText(fullPath, json);
            }
            else if (this._config.Buffer.ContentType == ContentType.Xml)
            {
                var xmlConvertor = this._messagePacker.GetConvertor(ContentType.Xml);
                var xml = (string)xmlConvertor.Serialize(message, typeof(Message));

                fileName = MakeFileName("xml");
                var fullPath = Path.Combine(this._config.Buffer.OutboxFolder, fileName);

                File.WriteAllText(fullPath, xml);
            }
            else if (this._config.Buffer.ContentType == ContentType.Binary)
            {
                var binaryConvertor = this._messagePacker.GetConvertor(ContentType.Binary);
                var binary = (byte[])binaryConvertor.Serialize(message, typeof(Message));

                fileName = MakeFileName("bin");
                var fullPath = Path.Combine(this._config.Buffer.OutboxFolder, fileName);

                File.WriteAllBytes(fullPath, binary);
            }

            this._logger.Verbouse(BusContexts.FileProcessing, $"The file {fileName} was created in '{this._config.Buffer.OutboxFolder}' for the message: {message}", this);

            // сигналим что есть файл
            if (_eventWaiter != null && _task != null)
            {
                // есть сценарий при котором мы можем споймань нулреференс оксепшен
                try
                {
                    _eventWaiter.Set();
                }
                catch (NullReferenceException) { }
            }
            
            //var result = File.ReadAllText(fullPath);
            //var m = this._messagePacker.MessageFromJson(result);
            //var mb = this._messagePacker.UnpackBody(m);
            //var mt = this._messagePacker.UnpackMessageType(mb);
            //var dlo = this._messagePacker.UnpackDeliveryObject(mb);
        }

        private string MakeFileName(string fileType)
        {
            var id = Thread.CurrentThread.ManagedThreadId;
            var d = DateTime.Now;

            var timeFormat = "yyyyMMdd_HHmmss";
            var timeString = d.ToString(timeFormat);

            var timeString3 = _timer.ElapsedTicks.ToString().PadLeft(15, '0') + "_H" + this.GetHashCode().ToString().PadLeft(10, '0');

            Interlocked.Increment(ref this._fileCounter);

            return "MSG_" + timeString + "_" + timeString3  + "_C" + (_fileCounter).ToString().PadLeft(10, '0') + "_T" + id.ToString().PadLeft(3, '0') + "." + fileType;
        }

        public void Start()
        {
            if (this._task != null)
            {
                return;
            }
            this._timer = Stopwatch.StartNew();
            this._eventWaiter = new AutoResetEvent(false);
            this._tokenSource = new CancellationTokenSource();
            this._task = Task.Run(() => Process());
        }

        public void Stop()
        {
            if (this._task == null)
            {
                return;
            }
            this._tokenSource.Cancel();
            if (!this._task.Wait(_canceledTaskTimeout))
            {
                this._logger.Error(0, BusContexts.FileProcessStopping, "Timeout waiting for file processing to exceeded", this);
            }
            this._tokenSource = null;
            this._task = null;
            if (_eventWaiter != null)
            {
                _eventWaiter.Close();
                _eventWaiter = null;
            }

            this._timer.Stop();
            this._timer = null;
        }

        private void Process()
        {
            
            this._logger.Verbouse(BusContexts.FileProcessing, $"The File Processing Thread is started", this);

            try
            {
                this.ProcessFiles();
            }
            catch (OperationCanceledException)
            {
                this._logger.Verbouse(BusContexts.FileProcessing, $"The File Processing Thread was canceled", this);
            }
            catch (ThreadAbortException e)
            {
                Thread.ResetAbort();
                this._logger.Exception(BusContexts.FileProcessing, "Abort the thread of the File Processing. Please fix the problem and restart the service.", e, this);
            }
            catch (Exception e)
            {
                this._logger.Exception(BusContexts.FileProcessing, "Abort the thread of the File Processing. Please fix the problem and restart the service.", e, this);
            }
        }

        private void ProcessFiles()
        {
            while (true)
            {
                _tokenSource.Token.ThrowIfCancellationRequested();

                var fileName = string.Empty;
                try
                {
                    fileName = this.GetNextMessageFile();

                    this._logger.Verbouse(BusContexts.FileProcessing, $"Attempt to process file '{fileName}'", this);

                    this.TryToProcessFile(fileName);
                }
                catch (OperationCanceledException)
                {
                    throw;
                }
                catch (Exception e)
                {
                    this._logger.Exception(BusContexts.FileProcessing, $"An error occurred while processing the file '{fileName}'", e, this);
                }
                
            }
        }

        private void TryToProcessFile(string fileName)
        {
            while (true)
            {
                _tokenSource.Token.ThrowIfCancellationRequested();

                try
                {
                    var message = this.ReadMessageFromFile(fileName);
                    if (message != null)
                    {
                        this.SendMessage(message, fileName);
                        ++this._fileIndex;
                        this.DeleteFile(fileName);
                    }
                    else
                    {
                        ++this._fileIndex;
                    }
                    
                    return;
                }
                catch(OperationCanceledException)
                {
                    throw;
                }
                catch (Exception e)
                {
                    this._logger.Exception(BusContexts.FileProcessing, $"An error occurred while sending the message by file '{fileName}'", e, this);
                }

                Thread.Sleep(this._processingFileTimeout);
            }
        }

        private void SendMessage(Message message, string fileName)
        {
            while(true)
            {
                this._tokenSource.Token.ThrowIfCancellationRequested();

                if (this.TryToSendMessage(message, fileName))
                {
                    return;
                }

                Thread.Sleep(this._sendingTimeout);
            }
        }

        private bool TryToSendMessage(Message message, string fileName)
        {
            var result = false;

            try
            {
                result = _amqpPublisher.TryToSend(message);
                if (result)
                {
                    this._logger.Verbouse(BusContexts.FileProcessing, $"The message was sent by file {fileName}: {message}", this);
                }
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception e)
            {
                this._logger.Exception(BusContexts.FileProcessing, $"An error occurred while trying to send the message by file '{fileName}'", e, this);
                result = false;
            }

            return result;
        }

        private Message ReadMessageFromFile(string fileName)
        {
            if (!File.Exists(fileName))
            {
                this._logger.Error(0, BusContexts.FileProcessing, $"The file '{fileName}' not found. ", this);
                return null;
            }

            try
            {
                var fileExtention = Path.GetExtension(fileName);
                if (string.IsNullOrEmpty(fileExtention))
                {
                    this._logger.Error(0, BusContexts.FileProcessing, $"The file '{fileName}' does not contains an extension. ", this);
                    return null;
                }

                Message message = null;
                IContentTypeConvertor convertor = null;

                switch (fileExtention.ToLower())
                {
                    case ".json":
                        var json = File.ReadAllText(fileName);
                        convertor = this._messagePacker.GetConvertor(ContentType.Json);
                        message = (Message)convertor.Deserialize(json, typeof(Message));
                        break;
                    case ".xml":
                        var xml = File.ReadAllText(fileName);
                        convertor = this._messagePacker.GetConvertor(ContentType.Xml);
                        message = (Message)convertor.Deserialize(xml, typeof(Message));
                        break;
                    case ".bin":
                        var binary = File.ReadAllBytes(fileName);
                        convertor = this._messagePacker.GetConvertor(ContentType.Binary);
                        message = (Message)convertor.Deserialize(binary, typeof(Message));
                        break;
                    default:
                        this._logger.Error(0, BusContexts.FileProcessing, $"The file '{fileName}' contains an unsupported extension. ", this);
                        return null;
                }

                return message;
            }
            catch (Exception e)
            {
                this._logger.Exception(BusContexts.FileProcessing, $"An error occurred while reading the file '{fileName}'", e, this);
                throw;
            }
        }

        private void DeleteFile(string fileName)
        {
            while(true)
            {
                try
                {
                    File.Delete(fileName);
                    this._logger.Verbouse(BusContexts.FileProcessing, $"The file {fileName} was deleted", this);
                    return;
                }
                catch (Exception e)
                {
                    this._logger.Exception(BusContexts.FileProcessing, $"An error occurred while deleting the file '{fileName}'", e, this);
                }

                if (!File.Exists(fileName))
                {
                    this._logger.Warning(0, BusContexts.FileProcessing, $"The file {fileName} not found for deletion", this);
                    return;
                }

                Thread.Sleep(this._fileDeletingTimeout);

                _tokenSource.Token.ThrowIfCancellationRequested();
            }
        }

        private string GetNextMessageFile()
        {
            if (this._files != null && this._fileIndex < this._files.Length)
            {
                return this._files[this._fileIndex];
            }
            this._files = ReadFiles();

            while (this._files == null || this._files.Length == 0)
            {
                _tokenSource.Token.ThrowIfCancellationRequested();

                _eventWaiter.WaitOne(this._filesReadingTimeout);
                this._files = ReadFiles();
            }

            this._fileIndex = 0;
            return this._files[this._fileIndex];
        }

        private string[] ReadFiles()
        {
            var files = Directory.GetFiles(this._config.Buffer.OutboxFolder, "*.*", SearchOption.TopDirectoryOnly);
            if (files != null && files.Length > 0)
            {
                return files.OrderBy(s => s).ToArray();
            }
            return new string[] { };
        }
    }
}
