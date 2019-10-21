namespace Atdi.Modules.AmqpBroker
{
    internal static class BrokerEvents
    {
        public const int VebouseEvent = 0;
        public const int ExceptionEvent = 1;
        public const int EstablishConnectionException = 3;
        public const int EstablishChannelException = 4;
        public const int DeclareExchangeException = 5;
        public const int DeclareQueueException = 6;
        public const int JoinConsumerException = 7;
        public const int UnjoinConsumerException = 8;
        public const int CloseConnectionException = 9;
        public const int DisposeConnectionException = 10;
        public const int CloseChannelException = 11;
        public const int DisposeChannelException = 12;
        public const int PublishException = 13;
        public const int UnjoinConsumersException = 14;
        public const int DeclareQueueBindingException = 15;
    }
}
