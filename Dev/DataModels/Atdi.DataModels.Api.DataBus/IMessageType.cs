using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Api.DataBus
{


    public enum QueueType
    {
        /// <summary>
        /// Для передачи сообщения используется общая очередь
        /// </summary>
        Common,
        /// <summary>
        /// Для передачи сообщения используется приватная очередь, ассоцировнаная с конкретным типом сообщения
        /// </summary>
        Private,
        /// <summary>
        /// Для передачи сообщения используется специфическая очередь
        /// </summary>
        Specific
    }
    public interface IMessageType
    {
        /// <summary>
        /// Название типа сообщения
        /// </summary>
        string Name { get; }

        /// <summary>
        ///  Тип используемой очереди для передачи данного типа сообщений
        /// </summary>
        QueueType QueueType { get; }

        /// <summary>
        /// Название специфической очереди, используется в случаи QueueType равно Specific
        /// </summary>
        string SpecificQueue { get; }
    }

    public abstract class MessageTypeBase : IMessageType
    {
        public MessageTypeBase(string name, QueueType queueType = QueueType.Common, string specificQueue = null)
        {
            this.Name = name ?? throw new ArgumentNullException(nameof(name));
            this.QueueType = queueType;
            if (queueType == QueueType.Specific)
            {
                if (specificQueue == null)
                {
                    throw new ArgumentNullException(nameof(specificQueue));
                }
                if (string.IsNullOrEmpty(specificQueue))
                {
                    throw new ArgumentException("message", nameof(specificQueue));
                }
                this.SpecificQueue = specificQueue;
            }
            else if (queueType == QueueType.Private)
            {
                this.SpecificQueue = name;
            }
        }
        public string Name { get; }

        public QueueType QueueType { get; }

        public string SpecificQueue { get; }
    }
}
