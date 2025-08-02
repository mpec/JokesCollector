using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace JokeCollector.App.JokeSyncFunction.ChuckNorrisJokesClient
{
    public class ChuckNorrisJokesClient(HttpClient httpClient, ILogger<ChuckNorrisJokesClient> logger, IOptions<ChuckNorrisApiOptions> options) : IJokesClient
    {
        public async Task<Joke> GetJokeAsync()
        {
            var response = await httpClient.GetAsync(options.Value.RandomJokeEndpoint);
            if (!response.IsSuccessStatusCode)
            {
                logger.LogError("There was an issue getting the joke, returned status: {statusCode}", response.StatusCode);

                response.EnsureSuccessStatusCode();
            }

            var responseBody = await response.Content.ReadAsStringAsync();
            var chuckNorrisJoke = JsonSerializer.Deserialize<ChuckNorrisJoke>(responseBody);

            var text = chuckNorrisJoke?.Value ?? string.Empty;

            logger.LogInformation("Received joke: {joke}", text);

            return new Joke { Text =  text };
        }
    }
}
