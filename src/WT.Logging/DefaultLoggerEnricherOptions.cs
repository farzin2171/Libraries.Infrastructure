namespace WT.Logging
{
    public class DefaultLoggerEnricherOptions
    {
        public string Application { get; set; } = String.Empty;
        public string ApplicationVersion { get; set; } = String.Empty ;
        public string ApplicationInformationalVersion { get; set; } = String .Empty ;
        public string Division { get; set; } = String.Empty;
        public string Environment { get; set; } = String.Empty;
    }
}
