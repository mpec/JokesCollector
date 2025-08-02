using System.Reflection;
using JokeCollector.App.JokeSyncFunction.ChuckNorrisJokesClient.IoC;
using JokeCollector.App.JokeSyncFunction.JokesRepository.IoC;
using JokeCollector.App.JokeSyncFunction.Processing.IoC;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = FunctionsApplication.CreateBuilder(args);

builder.Configuration.AddUserSecrets(Assembly.GetExecutingAssembly(), optional: true); //azure key vault should be added as config source for deployed functions

builder.Services
    .AddApplicationInsightsTelemetryWorkerService()
    .ConfigureFunctionsApplicationInsights()
    .AddJokeCollector()
    .AddJokesRepository()
    .AddChuckNorrisJokesApi(); //swap for another client if needed

var host = builder.Build();

await host.RunAsync();
