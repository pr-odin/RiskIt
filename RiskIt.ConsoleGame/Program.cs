using RiskIt.ConsoleGame.Commands;
using RiskIt.Main;
using RiskIt.Main.Actions;
using RiskIt.Main.Models;

namespace RiskIt.ConsoleGame
{
    public class Program
    {

        public static void Main(string[] args)
        {
            Game<int>? game = null;
            ConsoleParser parser = new ConsoleParser();

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
                            IDictionary<int, Area<int>> map = GetMapById(cfg.MapId);
                            Player[] players = CreatePlayers(cfg.PlayerCount);
                            game = new Game<int>(map, players);

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

        private static IDictionary<int, Area<int>> GetMapById(int mapId)
        {
            var map = CreateTestMap().ExportMap();
            return map;
        }

        private static MapGenerator<int> CreateTestMap()
        {
            // circle/star
            var mg = new MapGenerator<int>();

            mg.AddArea(0);
            mg.AddArea(1);
            mg.AddArea(2);
            mg.AddArea(3);
            mg.AddArea(4);

            mg.AddConnection(0, 1);
            mg.AddConnection(1, 2);
            mg.AddConnection(2, 3);
            mg.AddConnection(3, 4);
            mg.AddConnection(4, 0);

            return mg;

        }
    }
}
