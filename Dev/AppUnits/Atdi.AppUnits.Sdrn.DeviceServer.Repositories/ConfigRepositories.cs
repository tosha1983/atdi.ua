using System;
using Atdi.Platform.AppComponent;
using System.Collections.Generic;



namespace Atdi.AppUnits.Sdrn.DeviceServer.Repositories
{
    public class ConfigRepositories
    {
        /// <summary>
        /// Директория для хранения объектов DeviceCommand (Остановка, запуск и удаление задач)
        /// </summary>
        public string FolderDeviceCommand { get; set; }

        /// <summary>
        /// Директория для хранения объектов TaskParameters
        /// </summary>
        public string FolderTaskParameters { get; set; }

        /// <summary>
        /// Директория для хранения объектов MeasResults (результаты измерений для передачи на sdrn сервис)
        /// </summary>
        public string FolderMeasResults { get; set; }

        /// <summary>
        /// Директория для хранения объектов DeviceCommandResult (объекты с обновленными статусами MeasTask для передачи на sdrn сервис)
        /// </summary>
        public string DeviceCommandResult { get; set; }



    }
}
