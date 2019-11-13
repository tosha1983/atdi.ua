namespace Atdi.Modules.AmqpBroker
{
    public class ConnectionConfig
    {

        public string ConnectionName { get; set; }
        
        public string HostName { get; set; }

        public int? Port { get; set; }

        public string VirtualHost { get; set; }

        public string UserName { get; set; }

        public string Password { get; set; }

        public bool? AutoRecovery { get; set; }

        public override string ToString()
        {
            return $"Host='{HostName}', Port=#{Port}, VirtualHost='{VirtualHost}', User='{UserName}', Name='{ConnectionName}'";
        }
    }
}
