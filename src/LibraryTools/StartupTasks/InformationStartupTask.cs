using LibraryTools.Types;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryTools.StartupTasks
{
    /// <summary>
    /// Represents the basic information about the executing assembly, such as version and name
    /// </summary>
    public class InformationStartupTask : IStartupTask
    {
        private readonly ILogger<InformationStartupTask> _logger;
        private readonly DeploymentMode _deploymentMode;

        public InformationStartupTask(ILogger<InformationStartupTask> logger, IConfiguration configuration)
        {
            _logger = logger;
            _deploymentMode = configuration.GetDeploymentMode();
        }

        public void Execute()
        {
            var productInformation = new ProductInformation();
            _logger.LogInformation("{ProductName} {Version} successfully started", productInformation.Name, productInformation.InformationalVersion ?? productInformation.Version);
            _logger.LogInformation("Deployment Mode: {DeploymentMode}", _deploymentMode);
        }
    }
}
