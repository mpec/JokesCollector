namespace JokeCollector.App.JokeSyncFunction.Processing
{
    public class MaxApiRetriesException: Exception
    {
        public MaxApiRetriesException() : base()
        {
        }

        public MaxApiRetriesException(string? message) : base(message)
        {
        }

        public MaxApiRetriesException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}
