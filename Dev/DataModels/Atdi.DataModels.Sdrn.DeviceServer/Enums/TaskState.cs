namespace Atdi.DataModels.Sdrn.DeviceServer
{
    public enum TaskState 
    {
        /// <summary>
        /// Начальное состояние задачи
        /// </summary>
        Created = 0,
        /// <summary>
        /// Задача в ожидании запуска, принимает этот статус на время подхготовки потока и/или при отложенном запуске
        /// </summary>
        Pending = 1,
        /// <summary>
        /// Задача выполняется
        /// </summary>
        Executing = 2,
        /// <summary>
        /// Команда выполнена
        /// </summary>
        Done = 3,
        /// <summary>
        /// Задача отменена
        /// </summary>
        Cancelled = 4,
        /// <summary>
        ///  Комманда отклонена по явной причине
        /// </summary>
        Rejected = 5,
        /// <summary>
        ///  Задача неожидано прекратила свое выполнение
        /// </summary>
        Aborted = 6
    }
}