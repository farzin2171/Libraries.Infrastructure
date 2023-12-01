using Serilog.Core;
using Serilog.Events;

namespace WT.Logging
{
    public class LoggingLevelSwitches
    {
        /// <summary>
        /// Logging level switch that will be used for the "Microsoft.EntityFrameworkCore" namespace
        /// </summary>
        public static LoggingLevelSwitch EntityFrameworkLevelSwitch = new LoggingLevelSwitch(LogEventLevel.Warning);
    }
}
