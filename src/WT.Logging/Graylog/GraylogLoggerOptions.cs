namespace WT.Logging.Graylog
{
    public class GraylogLoggerOptions
    {
        public bool Enabled { get; set; }
        public string Hostname { get; set; } = String.Empty;
        public int Port { get; set; } = 9000;
    }
}
