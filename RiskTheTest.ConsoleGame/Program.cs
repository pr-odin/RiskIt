using RiskTheTest.Actions;
using RiskTheTest.ConsoleGame.Commands;

namespace RiskTheTest.ConsoleGame
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Game? game = null;
            Parser parser = new Parser();

            Console.WriteLine("Some generic message that the loop has been entered");

            while (true)
            {
                var input = Console.ReadLine();

                if (input.Trim().ToLowerInvariant() == "exit") return;

                ICommand? comm = parser.Parse(input);

                if (comm is null) continue;

                if (!comm.GetType().Equals(typeof(ServerCommand)) && game is null)
                {
                    Console.WriteLine("No game started");
                    continue;
                }

                if (comm.GetType().Equals(typeof(ServerCommand)))
                {
                    var serverComm = comm as ServerCommand;
                    switch (serverComm.CommandType)
                    {
                        case ServerCommandType.StartGame:
                            GameConfig cfg = serverComm.GameConfig;
                            Map map = GetMapById(cfg.MapId);
                            Player[] players = CreatePlayers(cfg.PlayerCount);
                            game = new Game(map, players);

                            Console.WriteLine("New game started with id \"{0}\"", game.Id);
                            break;
                        case ServerCommandType.EndGame:
                            // TODO: Persist game before destruction ?
                            Guid gameId = game!.Id;
                            game = null;

                            Console.WriteLine("Game with id \"{0}\" has been terminated", gameId);
                            break;
                        default:
                            break;
                    }
                    continue;
                }

                if (comm.GetType().Equals(typeof(GameCommand)))
                {
                    IAction action = (comm as GameCommand).ToAction();
                    game!.HandleAction(action);
                }
            }
        }

        private static Player[] CreatePlayers(uint playerCount)
        {
            var res = new Player[playerCount];

            for (int i = 0; i < playerCount; i++)
            {
                res[i] = new Player { Id = i };
            }

            return res;
        }

        private static Map GetMapById(int mapId)
        {
            return null;
        }
    }
}
