
using Atdi.DataModels.Api.DataBus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Api.DataBus
{
    internal class BusEvent : IBusEvent
    {
        internal BusEvent()
        {
            this.Id = Guid.NewGuid();
            this.Created = DateTime.Now;
            this.ManagedThread = System.Threading.Thread.CurrentThread.ManagedThreadId;
        }

        internal BusEvent(Guid id, DateTime created, int managedThread)
        {
            this.Id = id;
            this.Created = created;
            this.ManagedThread = managedThread;
        }

        public Guid Id { get; private set; }

        public int Code { get; set; }

        public DateTime Created { get; private set; }

        public BusEventLevel Level { get; set; }

        public string Context { get; set; }

        public string Text { get; set; }

        public int ManagedThread { get; private set; }

        public string Source { get; set; }

        public Exception Exception { get; set; }

        public override string ToString()
        {
            var t = string.Empty;
            var space = "        ";
            if (!string.IsNullOrEmpty(Text))
            {
                t = Text.Replace(Environment.NewLine, space);
            }
            
            return $"({ManagedThread}) {Created} {Code} [{Level.ToString()}] {Context}({Source}): {Environment.NewLine + space + t}";
        }
    }

    internal static class BusEvents
    {
        public static readonly int VerbouseEvent = 1;
        public static readonly int ExceptionEvent = 2;
        public static readonly int ConfigParameterError = 2;
        public static readonly int NotEstablishConnectionToRabbit = 3;
        public static readonly int NotEstablishRabbitSharedChannel = 4;
        public static readonly int NotDeclareExchange = 5;
        public static readonly int NotCreateConsumer = 6;


    }

    internal static class BusContexts
    {
        public static readonly string EnvelopeSending = "EnvelopeSending";
        public static readonly string Initialization = "Initialization";
        public static readonly string FileProcessing = "FileProcessing";
        public static readonly string FileProcessStopping = "FileProcessStopping";
        public static readonly string ConnectionEstablishing = "ConnectionEstablishing";
        public static readonly string MessageSending = "MessageSending";
        public static readonly string Disposing = "Disposing";
        public static readonly string ConsumerProcessing = "ConsumerProcessing";
    }
}
