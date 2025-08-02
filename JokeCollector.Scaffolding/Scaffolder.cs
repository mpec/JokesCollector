using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;

namespace JokeCollector.Scaffolding
{
    public static class Scaffolder
    {
        public static async Task ExecuteAsync(IConfiguration configuration) //no point in options pattern in such a small app
        {
            var connectionString = configuration.GetConnectionString("JokesDb");
            CreateFolderIfNeeded(connectionString);
            await ScaffoldDb(connectionString);
        }

        private static async Task ScaffoldDb(string? connectionString)
        {
            await using var connection = new SqliteConnection(connectionString);
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText = """
                CREATE TABLE IF NOT EXISTS Jokes (
                    Id TEXT PRIMARY KEY,
                    Value TEXT
                );

                CREATE UNIQUE INDEX "UX_Jokes_Text" ON "Jokes" (
                    "Value"
                );
                """;

            await command.ExecuteNonQueryAsync();
        }

        private static void CreateFolderIfNeeded(string? connectionString)
        {
            var dir = Path.GetDirectoryName(connectionString);
            if (string.IsNullOrEmpty(dir) || Directory.Exists(dir))
            {
                return;
            }

            Directory.CreateDirectory(dir);
        }
    }
}
