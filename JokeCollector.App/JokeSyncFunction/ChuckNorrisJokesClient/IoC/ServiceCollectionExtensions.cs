using System.Net.Http.Headers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace JokeCollector.App.JokeSyncFunction.ChuckNorrisJokesClient.IoC
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddChuckNorrisJokesApi(this IServiceCollection services)
        {
            services.AddOptions<ChuckNorrisApiOptions>()
                .Configure<IConfiguration>((options, config) => config.Bind(options));

            services
                .AddHttpClient<IJokesClient, ChuckNorrisJokesClient>((provider, client) =>
                {
                    var options = provider.GetRequiredService<IOptions<ChuckNorrisApiOptions>>().Value;
                    client.BaseAddress = new Uri(options.BaseUrl);
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client.DefaultRequestHeaders.Add("X-RapidAPI-Host", options.ApiHost);
                    client.DefaultRequestHeaders.Add("X-RapidAPI-Key", options.ApiKey); //secrets locally, key vault when deployed
                })
                .AddStandardResilienceHandler();
            return services;
        }
    }
}
