using RiskTheTest.ConsoleGame.Commands;

namespace RiskTheTest.ConsoleGame.Tests
{
    public class ParseTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void Server_ReturnsAServerCommand()
        {
            var parser = new Parser();

            var input = "server startgame";

            var res = parser.Parse(input);

            Assert.That(res, Is.TypeOf<ServerCommand>());
        }

        [Test]
        public void Default_ReturnsGameCommand()
        {
            var parser = new Parser();

            var input = "";

            var res = parser.Parse(input);

            Assert.That(res, Is.TypeOf<GameCommand>());
        }

        [Test]
        public void NullValueInput_ReturnsNull()
        {
            var parser = new Parser();

            string input = null;

            var res = parser.Parse(input);

            Assert.That(res, Is.Null);
        }

        [Test]
        public void SpongeBobText_BecomesLowerCaseCommand()
        {
            var parser = new Parser();

            var input = "SeRvEr sTaRtGaMe";

            var res = parser.Parse(input);

            Assert.That(res, Is.TypeOf<ServerCommand>());
        }
    }
}