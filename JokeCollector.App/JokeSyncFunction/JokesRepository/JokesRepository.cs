using Microsoft.Data.Sqlite;

namespace JokeCollector.App.JokeSyncFunction.JokesRepository
{
    public class JokesRepository : IJokesRepository
    {
        public async Task<int> InsertJoke(Joke joke)
        {
            await using var connection = new SqliteConnection("Data Source=jokes.db");
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText = "INSERT OR IGNORE INTO Jokes (Id, Value) VALUES (@id, @value);"; //we can handle non duplicated entries in a couple of ways,
                                                                                                   //here we ignore duplicate errors and will check affected rows to know if we got a duplicate
            command.Parameters.AddWithValue("@id", Guid.NewGuid());
            command.Parameters.AddWithValue("@value", joke.Text);

            return await command.ExecuteNonQueryAsync();
        }
    }
}
