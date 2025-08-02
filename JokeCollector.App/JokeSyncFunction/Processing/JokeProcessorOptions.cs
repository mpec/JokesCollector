namespace JokeCollector.App.JokeSyncFunction.Processing
{
    public class JokeProcessorOptions
    {
        public int JokeCollectorBatchSize { get; set; }
        public int MaxApiCallsForOneBatch { get; set; } = 5;
    }
}
