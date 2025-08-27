using RiskIt.ConsoleGame.Commands;

namespace RiskIt.ConsoleGame
{
    public class ConsoleParser
    {
        public ICommand? Parse(string? input)
        {
            if (input == null) return null;

            input = input.ToLowerInvariant();
            string[] items = input.Split(" ");

            ICommand command;

            if (items[0] == "?")
            {
                command = new DisplayCommand();
                command.Parse(items);
            }
            else if (items[0] == "disp")
            {
                command = new DisplayCommand();
                command.Parse(items[1..^0]);
            }
            else if (items[0] == "server")
            {
                command = new ServerCommand();
                command.Parse(items[1..^0]);
            }
            else
            {
                command = new GameCommand();
                command.Parse(items);
            }

            return command;
        }
    }
}
