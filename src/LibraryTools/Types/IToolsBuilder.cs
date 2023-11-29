using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace LibraryTools.Types
{
    public interface IToolsBuilder
    {
        IServiceCollection Services { get; }
        IConfiguration Configuration { get; }
    }
}
