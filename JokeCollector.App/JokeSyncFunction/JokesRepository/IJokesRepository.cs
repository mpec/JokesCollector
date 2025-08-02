
namespace JokeCollector.App.JokeSyncFunction.JokesRepository
{
    public interface IJokesRepository
    {
        Task<int> InsertJoke(Joke joke);
    }
}