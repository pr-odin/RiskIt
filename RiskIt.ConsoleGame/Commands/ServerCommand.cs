using RiskIt.Main;

namespace RiskIt.ConsoleGame.Commands
{
    public class ServerCommand : ICommand
    {
        public GameConfig GameConfig { get; set; }

        public ServerCommandType CommandType { get; private set; }


        public void Parse(string[] args)
        {
            var inputArgs = args[0];

            switch (inputArgs)
            {
                case "startgame":
                    GameConfig = new GameConfig();
                    GameConfig.Parse(args[1..^0]);
                    return;
                case "endgame":
                    CommandType = ServerCommandType.EndGame;
                    return;
                default:
                    throw new Exception("Somethings off");
            }
        }
    }
    public enum ServerCommandType
    {
        StartGame,
        EndGame
    }
}
