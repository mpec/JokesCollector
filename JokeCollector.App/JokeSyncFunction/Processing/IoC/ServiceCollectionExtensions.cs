using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace JokeCollector.App.JokeSyncFunction.Processing.IoC
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddJokeCollector(this IServiceCollection services)
        {
            services.AddOptions<JokeProcessorOptions>()
                .Configure<IConfiguration>((options, config) => config.Bind(options));

            services.AddScoped<IJokeProcessor, JokeProcessor>();

            return services;
        }
    }
}
