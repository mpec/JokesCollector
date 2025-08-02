using JokeCollector.Scaffolding;
using Microsoft.Extensions.Configuration;

var configuration = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

await Scaffolder.ExecuteAsync(configuration);
