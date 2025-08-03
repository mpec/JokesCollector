using JokeCollector.App.JokeSyncFunction.Processing;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace JokeCollector.App.JokeSyncFunction;

public class JokeSyncFunction(ILogger<JokeSyncFunction> logger, IJokeProcessor jokeCollector)
{
    [Function(nameof(JokeSyncFunction))]
    public async Task RunAsync([TimerTrigger("%JokeCollectorCron%")] TimerInfo timerInfo)
    {
        try
        {
            logger.LogInformation("Executing {function}", nameof(JokeSyncFunction));
            await jokeCollector.ExecuteAsync();
            logger.LogInformation("Executed {function}", nameof(JokeSyncFunction));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error executing {function}", nameof(JokeSyncFunction));
            throw;
        }

    }
}
