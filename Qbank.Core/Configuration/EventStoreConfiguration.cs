namespace Qbank.Core.Configuration
{
    public class EventStoreConfiguration
    {
        public string UserName { get; set; }
        public string UserPassword { get; set; }
        public string Ip { get; set; }
        public int Port { get; set; }
        public int SnapchotLimit { get; set; }
    }
}