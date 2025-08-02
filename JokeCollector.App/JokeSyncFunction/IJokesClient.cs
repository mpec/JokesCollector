
namespace JokeCollector.App.JokeSyncFunction
{
    public interface IJokesClient
    {
        Task<Joke> GetJokeAsync();
    }
}
