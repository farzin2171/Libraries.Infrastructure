using WT.Logging.ApplicationInsights;
using WT.Logging.Graylog;
using WT.Logging.LoggingService;
using WT.Logging.Seq;

namespace WT.Logging
{
    public class LoggerOptions
    {
        public LoggingServiceLoggerOptions Service { get; set; } = new LoggingServiceLoggerOptions();
        public ApplicationInsightsLoggerOptions ApplicationInsights { get; set; } = new ApplicationInsightsLoggerOptions();
        public GraylogLoggerOptions Graylog { get; set; } = new GraylogLoggerOptions();
        public SeqLoggerOptions Seq { get; set; } = new SeqLoggerOptions();

        public string Division { get; set; } = "WebTechCo";
        public string Environment { get; set; } = "Development";
    }
}
