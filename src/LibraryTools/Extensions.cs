using LibraryTools.StartupTasks;
using LibraryTools.Types;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace LibraryTools
{
    public static class Extensions
    {
        public static IToolsBuilder AddDigitalInsuranceTools(this IServiceCollection services, IConfiguration configuration, string appOptionsSectionName = "app")
        {
            var builder = new ToolsBuilder(services, configuration);
            return services.GetIToolsBuilder(builder, appOptionsSectionName);
        }

        private static IToolsBuilder GetIToolsBuilder(this IServiceCollection services, IToolsBuilder builder, string appOptionsSectionName)
        {
            var productInformation = new ProductInformation();
            var appOptions = builder.GetOptions<AppOptions>(appOptionsSectionName);
            services.AddSingleton(appOptions);

            builder.AddStartupTask<InformationStartupTask>();

            if (appOptions.DisplayBanner && !string.IsNullOrWhiteSpace(productInformation.Name))
            {
                // The try catch is to prevent the application from crashing when the render of the banner fails because yes it happens...
                // Some fonts like 'Doom' will crash with certain character sequences such as '7.' or '/.' are present in the string
                // passed in parameter. I'm guessing it's due to how the Figgle characters are put together and can't overlap well.
                // I left the banner since most of the time it works and put a fallback for when it crashes since this is mostly only
                // used when running in local.
                try
                {
                    Console.WriteLine(Figgle.FiggleFonts.Doom.Render($"{productInformation.Name} {productInformation.Version}"));
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Failed to display banner - exception: {e.Message}");
                    Console.WriteLine($"{productInformation.Name} {productInformation.Version}");
                }
            }

            return builder;
        }

        /// <summary>
        /// Returns the options from the configuration
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <param name="configuration"></param>
        /// <param name="sectionName"></param>
        /// <returns></returns>
        public static TModel GetOptions<TModel>(this IConfiguration configuration, string sectionName) where TModel : new()
        {
            var model = new TModel();
            configuration.GetSection(sectionName).Bind(model);
            return model;
        }

        /// <summary>
        /// Returns the options from the configuration
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <param name="builder"></param>
        /// <param name="settingsSectionName"></param>
        /// <returns></returns>
        public static TModel GetOptions<TModel>(this IToolsBuilder builder, string settingsSectionName) where TModel : new()
        {
            return builder.Configuration.GetOptions<TModel>(settingsSectionName);
        }

        /// <summary>
        /// Determines the deployment mode from the configuration properties
        /// </summary>
        /// <param name="configuration">The set of key/value application configuration properties.</param>
        /// <returns></returns>
        public static DeploymentMode GetDeploymentMode(this IConfiguration configuration)
        {
            if (Enum.TryParse(typeof(DeploymentMode), configuration.GetValue<string>(ConfigurationKeysConstants.DeploymentMode), out var deploymentMode))
            {
                return (DeploymentMode)deploymentMode;
            }

            return DeploymentMode.OnPremises;
        }

        /// <summary>
        /// Register object configuration in singleton in DI
        /// </summary>
        /// <typeparam name="T">Any concrete class</typeparam>
        /// <param name="services">The <see cref="T:Microsoft.Extensions.DependencyInjection.IServiceCollection" /> to add the services to.</param>
        /// <param name="configuration">The <see cref="IConfiguration"/> to source to populate <typeparam name="T"></typeparam></param>
        /// <param name="sectionName">Name of the section in the config</param>
        /// <returns>The <see cref="IServiceCollection" /> so that additional calls can be chained.</returns>
        public static IServiceCollection ConfigureSettings<T>(this IServiceCollection services, IConfiguration configuration, string sectionName) where T : class, new()
        {
            var config = new T();
            configuration.Bind(sectionName, config);
            services.AddSingleton(config);

            return services;
        }

        public static void RunWithTasks(this IHost host)
        {
            using var serviceScope = host.Services.CreateScope();

            var startupTasks = serviceScope.ServiceProvider.GetServices<IStartupTask>();

            foreach (var startupTask in startupTasks)
            {
                startupTask.Execute();
            }

            host.Run();
        }

        /// <summary>
        /// Registers startup tasks to be run on startup
        /// </summary>
        public static IToolsBuilder AddStartupTask<T>(this IToolsBuilder builder) where T : class, IStartupTask
        {
            builder.Services.AddTransient<IStartupTask, T>();

            return builder;
        }
    }
}
