
namespace JokeCollector.App.JokeSyncFunction.ChuckNorrisJokesClient
{
    public class ChuckNorrisApiOptions
    {
        public required string BaseUrl { get; init; } = "https://matchilling-chuck-norris-jokes-v1.p.rapidapi.com"; // until we know we need to alter this no need to put it in local.settings.json
                                                                                                                    // we can always override this via environment variables on deployed function
        public required string RandomJokeEndpoint { get; init; } = "jokes/random"; //depending on the approach this could just be hardcoded in the client
        public required string ApiHost { get; init; } = "matchilling-chuck-norris-jokes-v1.p.rapidapi.com";
        public required string ApiKey { get; init; }
    }
}