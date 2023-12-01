using LibraryTools.Types;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace LibraryTools
{
    public class ToolsBuilder : IToolsBuilder
    {
        public IServiceCollection Services { get; }
        public IConfiguration Configuration { get; }

        public ToolsBuilder(IServiceCollection services, IConfiguration configuration)
        {
            Services = services;
            Configuration = configuration;
        }
    }
}
