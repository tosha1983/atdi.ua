using System;
using System.Linq;
using System.Threading;
using System.IO;
using System.Diagnostics;
using Atdi.Modules.Sdrn.DeviceBus;

namespace Atdi.Api.Sdrn.Device.BusController
{
    internal sealed class FileSystemBufferProcessing : IBufferProcessing
    {
        private readonly DeviceBusConfig _config;
        private readonly AmqpPublisher _amqpPublisher;
        private EventWaitHandle _eventWaiter;
        private readonly BusLogger _logger;
        private readonly BusMessagePacker _messagePacker;
        private Stopwatch _timer;
        private volatile int _fileCounter;
        private Thread _thread;
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

        public FileSystemBufferProcessing(DeviceBusConfig config, AmqpPublisher amqpPublisher, BusMessagePacker messagePacker, BusLogger logger)
        {
            this._config = config;
            this._amqpPublisher = amqpPublisher;
            this._messagePacker = messagePacker;
            this._logger = logger;
        }

        public void Save(BusMessage message)
        {
            var fileName = string.Empty;

            if (this._config.BufferConfig.ContentType == ContentType.Json)
            {
                var jsonConvertor = BusMessagePacker.GetConvertor(ContentType.Json);
                var json = (string)jsonConvertor.Serialize(message, typeof(BusMessage));

                fileName = MakeFileName("json");
                var fullPath = Path.Combine(this._config.BufferConfig.OutboxFolder, fileName);

                File.WriteAllText(fullPath, json);
            }
            else if (this._config.BufferConfig.ContentType == ContentType.Xml)
            {
                var xmlConvertor = BusMessagePacker.GetConvertor(ContentType.Xml);
                var xml = (string)xmlConvertor.Serialize(message, typeof(BusMessage));

                fileName = MakeFileName("xml");
                var fullPath = Path.Combine(this._config.BufferConfig.OutboxFolder, fileName);

                File.WriteAllText(fullPath, xml);
            }
            else if (this._config.BufferConfig.ContentType == ContentType.Binary)
            {
                var binaryConvertor = BusMessagePacker.GetConvertor(ContentType.Binary);
                var binary = (byte[])binaryConvertor.Serialize(message, typeof(BusMessage));

                fileName = MakeFileName("bin");
                var fullPath = Path.Combine(this._config.BufferConfig.OutboxFolder, fileName);

                File.WriteAllBytes(fullPath, binary);
            }

            this._logger.Verbouse("DeviceBus.FileProcessing", $"The message file is created: File='{fileName}', {this._config.BufferConfig}, Message={message.Id}", this);

            // сигналим что есть файл
            if (_eventWaiter != null && _thread != null)
            {
                // есть сценарий при котором мы можем споймань нулреференс оксепшен
                try
                {
                    _eventWaiter.Set();
                }
                catch (NullReferenceException) { }
            }

        }

        private string MakeFileName(string fileType)
        {
            var id = Thread.CurrentThread.ManagedThreadId;
            var d = DateTime.Now;

            const string timeFormat = "yyyyMMdd_HHmmss";
            var timeString = d.ToString(timeFormat);

            var timeString3 = _timer.ElapsedTicks.ToString().PadLeft(15, '0') + "_H" + this.GetHashCode().ToString().PadLeft(10, '0');

            Interlocked.Increment(ref this._fileCounter);

            return "MSG_" + timeString + "_" + timeString3  + "_C" + (_fileCounter).ToString().PadLeft(10, '0') + "_T" + id.ToString().PadLeft(3, '0') + "." + fileType;
        }

        public void Start()
        {
            if (this._thread != null)
            {
                return;
            }
            this._timer = Stopwatch.StartNew();
            this._eventWaiter = new AutoResetEvent(false);
            this._tokenSource = new CancellationTokenSource();
            this._thread = new Thread(this.Process)
            {
                Name = "Atdi.DeviceBus.FileProcessing"
            };
            this._thread.Start();
        }

        public void Stop()
        {
            if (this._thread == null)
            {
                return;
            }
            this._tokenSource.Cancel();
            Thread.Sleep(_canceledTaskTimeout);
            if (_thread.IsAlive)
            {
                this._logger.Error(0, "DeviceBus.FileProcessStopping", "Timeout waiting for file processing to exceeded", this);
            }
            this._tokenSource = null;
            this._thread = null;
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
            
            this._logger.Info("DeviceBus.FileProcessing", $"The File Processing Thread starting ...", this);

            try
            {
                this.ProcessFiles();
                this._logger.Info("DeviceBus.FileProcessing", $"The File Processing Thread is finished normally", this);
            }
            catch (OperationCanceledException)
            {
                this._logger.Info("DeviceBus.FileProcessing", $"The File Processing Thread is finished normally", this);
            }
            catch (ThreadAbortException e)
            {
                Thread.ResetAbort();
                this._logger.Exception("DeviceBus.FileProcessing", "Abort the thread of the File Processing. Please fix the problem and restart the service.", e, this);
            }
            catch (Exception e)
            {
                this._logger.Critical("DeviceBus.FileProcessing", "Abort the thread of the File Processing. Please fix the problem and restart the service.", e, this);
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

                    this._logger.Verbouse("DeviceBus.FileProcessing", $"Attempt to process file: Name='{fileName}'", this);

                    this.TryToProcessFile(fileName);
                }
                catch (OperationCanceledException)
                {
                    throw;
                }
                catch (Exception e)
                {
                    this._logger.Exception("DeviceBus.FileProcessing", $"An error occurred while processing the file: Name='{fileName}'", e, this);
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
                    this._logger.Exception("DeviceBus.FileProcessing", $"An error occurred while sending the message by file: Name='{fileName}'", e, this);
                }

                Thread.Sleep(this._processingFileTimeout);
            }
        }

        private void SendMessage(BusMessage message, string fileName)
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

        private bool TryToSendMessage(BusMessage message, string fileName)
        {
            bool result;

            try
            {
                result = _amqpPublisher.TryToSend(message);
                if (result)
                {
                    this._logger.Verbouse("DeviceBus.FileProcessing", $"The message was sent by file: Name='{fileName}', {message}", this);
                }
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception e)
            {
                this._logger.Exception("DeviceBus.FileProcessing", $"An error occurred while trying to send the message by file: Name='{fileName}'", e, this);
                result = false;
            }

            return result;
        }

        private BusMessage ReadMessageFromFile(string fileName)
        {
            if (!File.Exists(fileName))
            {
                this._logger.Error(0, "DeviceBus.FileProcessing", $"The file not found: Name='{fileName}'", this);
                return null;
            }

            try
            {
                var fileExtenstion = Path.GetExtension(fileName);
                if (string.IsNullOrEmpty(fileExtenstion))
                {
                    this._logger.Error(0, "DeviceBus.FileProcessing", $"The file does not contains an extension: Name='{fileName}'", this);
                    return null;
                }

                BusMessage message;
                IContentTypeConvertor convertor;

                switch (fileExtenstion.ToLower())
                {
                    case ".json":
                        var json = File.ReadAllText(fileName);
                        convertor = BusMessagePacker.GetConvertor(ContentType.Json);
                        message = (BusMessage)convertor.Deserialize(json, typeof(BusMessage));
                        break;
                    case ".xml":
                        var xml = File.ReadAllText(fileName);
                        convertor = BusMessagePacker.GetConvertor(ContentType.Xml);
                        message = (BusMessage)convertor.Deserialize(xml, typeof(BusMessage));
                        break;
                    case ".bin":
                        var binary = File.ReadAllBytes(fileName);
                        convertor = BusMessagePacker.GetConvertor(ContentType.Binary);
                        message = (BusMessage)convertor.Deserialize(binary, typeof(BusMessage));
                        break;
                    default:
                        this._logger.Error(0, "DeviceBus.FileProcessing", $"The file contains an unsupported extension: Name='{fileName}'", this);
                        return null;
                }

                return message;
            }
            catch (Exception e)
            {
                this._logger.Exception("DeviceBus.FileProcessing", $"An error occurred while reading the file: Name='{fileName}'", e, this);
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
                    this._logger.Verbouse("DeviceBus.FileProcessing", $"The file is deleted: Name='{fileName}'", this);
                    return;
                }
                catch (Exception e)
                {
                    this._logger.Exception("DeviceBus.FileProcessing", $"An error occurred while deleting the file: Name='{fileName}'", e, this);
                }

                if (!File.Exists(fileName))
                {
                    this._logger.Warning(0, "DeviceBus.FileProcessing", $"The file not found for deletion: Name='{fileName}'", this);
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
            var files = Directory.GetFiles(this._config.BufferConfig.OutboxFolder, "*.*", SearchOption.TopDirectoryOnly);
            if (files.Length > 0)
            {
                return files.OrderBy(s => s).ToArray();
            }
            return new string[] { };
        }
    }
}
