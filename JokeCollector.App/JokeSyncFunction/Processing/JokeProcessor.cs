using JokeCollector.App.JokeSyncFunction.JokesRepository;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace JokeCollector.App.JokeSyncFunction.Processing
{
    public class JokeProcessor(IJokesClient jokesClient,
        IJokesRepository jokesRepository,
        IOptions<JokeProcessorOptions> options,
        ILogger<JokeProcessor> logger) : IJokeProcessor
    {
        public async Task ExecuteAsync()
        {
            var batchSize = options.Value.JokeCollectorBatchSize;

            logger.LogInformation("Collecting {batchSize} jokes", batchSize);

            for (int i = 0; i < options.Value.JokeCollectorBatchSize; i++)
            {
                await CollectSingleJoke();
            }

            logger.LogInformation("Collecting {batchSize} jokes done", batchSize);
        }

        private async Task CollectSingleJoke()
        {
            var calls = 1;
            while (calls <= options.Value.MaxApiCallsForOneBatch)
            {
                var joke = await jokesClient.GetJokeAsync();
                var jokeLength = joke.Text.Length;

                if (jokeLength < 0 || jokeLength > 200)
                {
                    logger.LogWarning("Joke length is {jokeLength}, retrying...", jokeLength);
                    calls++;
                    continue;
                }

                var affectedRows = await jokesRepository.InsertJoke(joke);

                if (affectedRows == 0)
                {
                    logger.LogWarning("Joke with text \"{text}\" already exists, retrying...", joke.Text);
                    calls++;
                    continue;
                }

                logger.LogInformation("It took {retries} calls to get the requested joke", calls);

                return;
            }

            throw new MaxApiRetriesException($"Failed to collect a valid joke after {options.Value.MaxApiCallsForOneBatch} calls.");
        }
    }
}
