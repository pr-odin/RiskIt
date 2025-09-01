namespace RiskIt.ConsoleGame.Commands
{
    public class ReplayCommand : ICommand
    {
        public ReplayCommandType? commandType;
        public int? Step;

        public void Parse(string[] args)
        {
            string firstArg = args[0];
            switch (firstArg)
            {
                case ("n"):
                case ("next"):
                    if (args.Length > 1 && args[1] != null)
                    {
                        Step = int.Parse(args[1]);
                    }
                    else
                        Step = 1;

                    commandType = ReplayCommandType.NextAction;
                    break;
            }

            return;
        }
    }
    public enum ReplayCommandType
    {
        NextAction,
    }
}


