using JokeCollector.App.JokeSyncFunction;
using JokeCollector.App.JokeSyncFunction.JokesRepository;
using JokeCollector.App.JokeSyncFunction.Processing;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;

namespace JokesCollector.App.Tests
{
    public class JokeCollectorTests
    {
        private readonly Mock<IJokesClient> jokesClientMock;
        private readonly Mock<IJokesRepository> jokesRepositoryMock;
        private readonly Mock<ILogger<JokeProcessor>> loggerMock;

        public JokeCollectorTests()
        {
            jokesClientMock = new Mock<IJokesClient>();
            jokesRepositoryMock = new Mock<IJokesRepository>();
            loggerMock = new Mock<ILogger<JokeProcessor>>();
        }

        [Theory]
        [InlineData(1)]
        [InlineData(5)]
        [InlineData(10)]
        public async Task ExecuteAsync_AddsExpectedNumberOfJokes_BasedOnOptions(int batchSize)
        {
            // Arrange

            var options = Options.Create(new JokeProcessorOptions
            {
                JokeCollectorBatchSize = batchSize,
                MaxApiCallsForOneBatch = 1
            });

            var joke = new Joke { Text = "Chuck Norris Joke" };

            jokesClientMock
                .Setup(x => x.GetJokeAsync())
                .ReturnsAsync(joke); //we do not care about duplicates as the repo will always claim it's new

            jokesRepositoryMock
                .Setup(x => x.InsertJoke(It.IsAny<Joke>()))
                .ReturnsAsync(1);

            var sut = new JokeProcessor(
                jokesClientMock.Object,
                jokesRepositoryMock.Object,
                options,
                loggerMock.Object
            );

            // Act
            await sut.ExecuteAsync();

            // Assert
            jokesRepositoryMock.Verify(x => x.InsertJoke(It.IsAny<Joke>()), Times.Exactly(batchSize));
        }

        [Fact]
        public async Task ExecuteAsync_ThrowsMaxApiRetriesException_WhenRetriesExhausted()
        {
            var options = Options.Create(new JokeProcessorOptions
            {
                JokeCollectorBatchSize = 1,
                MaxApiCallsForOneBatch = 3
            });

            jokesClientMock
                .SetupSequence(x => x.GetJokeAsync())
                .ReturnsAsync(new Joke { Text = "This text is over 200 characters long. This text is over 200 characters long. This text is over 200 characters long. This text is over 200 characters long. This text is over 200 characters long. This text is over 200 characters long. This text is over 200 characters long." })
                .ReturnsAsync(new Joke { Text = "I am a duplicate"})
                .ReturnsAsync(new Joke { Text = string.Empty});

            jokesRepositoryMock
                .Setup(x => x.InsertJoke(It.IsAny<Joke>()))
                .ReturnsAsync(0);
            
            var sut = new JokeProcessor(
                 jokesClientMock.Object,
                 jokesRepositoryMock.Object,
                 options,
                 loggerMock.Object
             );

            // Act & Assert
            await Assert.ThrowsAsync<MaxApiRetriesException>(() => sut.ExecuteAsync());
        }

        [Fact]
        public async Task ExecuteAsync_RetiresJokeCollection_WhenJokeIsTooLong()
        {
            var options = Options.Create(new JokeProcessorOptions
            {
                JokeCollectorBatchSize = 1,
                MaxApiCallsForOneBatch = 3
            });

            jokesClientMock
                .SetupSequence(x => x.GetJokeAsync())
                .ReturnsAsync(new Joke { Text = "This text is over 200 characters long. This text is over 200 characters long. This text is over 200 characters long. This text is over 200 characters long. This text is over 200 characters long. This text is over 200 characters long. This text is over 200 characters long." })
                .ReturnsAsync(new Joke { Text = "This text is over 200 characters long. This text is over 200 characters long. This text is over 200 characters long. This text is over 200 characters long. This text is over 200 characters long. This text is over 200 characters long. This text is over 200 characters long. #2" })
                .ReturnsAsync(new Joke { Text = "This is inserted" });

            jokesRepositoryMock
                .Setup(x => x.InsertJoke(It.IsAny<Joke>()))
                .ReturnsAsync(1);

            var sut = new JokeProcessor(
                 jokesClientMock.Object,
                 jokesRepositoryMock.Object,
                 options,
                 loggerMock.Object
             );

            await sut.ExecuteAsync();

            jokesRepositoryMock.Verify(x => x.InsertJoke(It.IsAny<Joke>()), Times.Exactly(1));
            jokesClientMock.Verify(x => x.GetJokeAsync(), Times.Exactly(3));
            loggerMock.Verify(
                x => x.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString() == "It took 3 calls to get the requested joke"),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }

        [Fact]
        public async Task ExecuteAsync_RetiresJokeCollection_WhenJokeIsDuplicated()
        {
            var options = Options.Create(new JokeProcessorOptions
            {
                JokeCollectorBatchSize = 1,
                MaxApiCallsForOneBatch = 6
            });

            var joke = new Joke { Text = "Chuck Norris Joke" };

            jokesClientMock
                .Setup(x => x.GetJokeAsync())
                .ReturnsAsync(joke); //we do not care about duplicates as the repo will decide if it's new

            jokesRepositoryMock
                .SetupSequence(x => x.InsertJoke(It.IsAny<Joke>()))
                .ReturnsAsync(0)
                .ReturnsAsync(0)
                .ReturnsAsync(0)
                .ReturnsAsync(1);

            var sut = new JokeProcessor(
                 jokesClientMock.Object,
                 jokesRepositoryMock.Object,
                 options,
                 loggerMock.Object
             );

            await sut.ExecuteAsync();

            jokesRepositoryMock.Verify(x => x.InsertJoke(It.IsAny<Joke>()), Times.Exactly(4));
            jokesClientMock.Verify(x => x.GetJokeAsync(), Times.Exactly(4));
            loggerMock.Verify(
                x => x.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString() == "It took 4 calls to get the requested joke"),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }
    }
}