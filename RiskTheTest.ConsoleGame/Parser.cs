using RiskTheTest.ConsoleGame.Commands;

namespace RiskTheTest.ConsoleGame
{
    public class Parser
    {
        public ICommand? Parse(string? input)
        {
            if (input == null) return null;

            input = input.ToLowerInvariant();
            string[] items = input.Split(" ");

            ICommand command = ParseCommand(items[0]);

            command.Parse(items[1..^0]);

            return command;
        }

        private ICommand ParseCommand(string input)
        {
            switch (input)
            {
                case "server":
                    return new ServerCommand();
                default:
                    return new GameCommand();

            }

            return null;

        }
    }
}
