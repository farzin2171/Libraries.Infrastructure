using Serilog.Core;
using Serilog.Events;

namespace WT.Logging.Graylog
{
    public class GraylogLoggerEnricher : ILogEventEnricher
    {
        /// <summary>
        /// Enrich the log event.
        /// </summary>
        /// <param name="logEvent">The log event to enrich.</param>
        /// <param name="propertyFactory">Factory for creating new properties to add to the event.</param>
        public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
        {
            if (logEvent.Exception != null && logEvent.Exception.StackTrace == null && logEvent.Exception.GetType().FullName == "Serilog.Formatting.Compact.Reader.TextException")
            {
                //A Serilog.Formatting.Compact.Reader.TextException comes from LogEventReader.ReadFromJObject(jsonObj) and will not provide any stack trace information
                //Since the graylog sink does not call Exception.ToString() we need to add it as a new property if we want to log it
                //reference: https://github.com/serilog/serilog-formatting-compact-reader

                var property = propertyFactory.CreateProperty("ExceptionStackTrace", logEvent.Exception.ToString());

                logEvent.AddPropertyIfAbsent(property);
            }
        }
    }
}
