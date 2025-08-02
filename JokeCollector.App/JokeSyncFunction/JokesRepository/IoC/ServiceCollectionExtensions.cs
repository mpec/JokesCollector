using Microsoft.Extensions.DependencyInjection;

namespace JokeCollector.App.JokeSyncFunction.JokesRepository.IoC
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddJokesRepository(this IServiceCollection services)
        {
            services.AddScoped<IJokesRepository, JokesRepository>();
            return services;
        }
    }
}
