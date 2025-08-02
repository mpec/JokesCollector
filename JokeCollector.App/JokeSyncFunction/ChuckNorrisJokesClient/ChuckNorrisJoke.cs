using System.Text.Json.Serialization;

namespace JokeCollector.App.JokeSyncFunction.ChuckNorrisJokesClient;

public class ChuckNorrisJoke
{
    [JsonPropertyName("value")]
    public required string Value { get; init; }
}