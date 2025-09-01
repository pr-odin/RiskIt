namespace RiskIt.ConsoleGame.Commands
{
    public class ReplayCommand : ICommand
    {
        public ReplayCommandType? commandType;

        public void Parse(string[] args)
        {
            string firstArg = args[0];
            if (firstArg == "next" || firstArg == "n")
                commandType = ReplayCommandType.NextAction;

            return;
        }
    }
    public enum ReplayCommandType
    {
        NextAction,
    }
}


