namespace RiskTheTest.ConsoleGame.Tests
{
    public class GameConfigParse
    {
        [Test]
        public void EmptyDefaultsToGame()
        {
            var gameConfig = new GameConfig();

            var parseArgs = new string[] { "" };

            gameConfig.Parse(parseArgs);

            Assert.That(gameConfig.MapId, Is.EqualTo(1));
            Assert.That(gameConfig.PlayerCount, Is.EqualTo(2));
        }

        [Test]
        public void Map5AndPlayers4()
        {
            var gameConfig = new GameConfig();

            var parseArgs = new string[] { "map:5", "players:4" };

            gameConfig.Parse(parseArgs);

            Assert.That(gameConfig.MapId, Is.EqualTo(5));
            Assert.That(gameConfig.PlayerCount, Is.EqualTo(4));
        }

        [Test]
        public void ReverseOrderOfArgumentsGiven_Player6AndMap4()
        {
            var gameConfig = new GameConfig();

            var parseArgs = new string[] { "players:6", "map:4" };

            gameConfig.Parse(parseArgs);

            Assert.That(gameConfig.MapId, Is.EqualTo(4));
            Assert.That(gameConfig.PlayerCount, Is.EqualTo(6));
        }

        [Test]
        public void NegativePlayers_ThrowsException()
        {
            var gameConfig = new GameConfig();

            var parseArgs = new string[] { "players:-4", "map:4" };

            Assert.Throws<OverflowException>(() => gameConfig.Parse(parseArgs));
        }
    }
}
