using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Platform.Logging.EventsConsumers
{
    public sealed class FileEventsConsumerConfig : EventsConsumerConfig
    {

        public string FolderPath { get; private set; }

        public string FilePrefix { get; private set; }

        public FileEventsConsumerConfig(IConfigParameters parameters)
            : base(parameters)
        {
            this.FolderPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs");
            if (parameters.Has("FolderPath"))
            {
                var folderPath = (string)parameters["FolderPath"];
                if (!string.IsNullOrEmpty(folderPath))
                {
                    this.FolderPath = folderPath;
                }
            }

            this.FilePrefix = "";
            if (parameters.Has("FilePrefix"))
            {
                var filePrefix = (string)parameters["FilePrefix"];
                if (!string.IsNullOrEmpty(filePrefix))
                {
                    this.FilePrefix = filePrefix;
                }
            }
        }
    }
}
