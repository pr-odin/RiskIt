using RiskIt.ConsoleGame.Commands;
using RiskIt.Main;
using RiskIt.Main.Actions;
using RiskIt.Main.AttackHandlers;
using RiskIt.Main.MapGeneration;
using RiskIt.Main.Models;
using RiskIt.Main.Models.Enums;

namespace RiskIt.ConsoleGame
{
    public class Program
    {

        public static void Main(string[] args)
        {
            Game<string>? game = null;
            AreaEnumeratorFactory<string> areaEnumeratorFactory = new AreaEnumeratorFactory<string>();
            MapSeeder<string> mapSeeder = new MapSeeder<string>(areaEnumeratorFactory);
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
                            MapGenerator<string> mapGenerator = GetMapGeneratorById(cfg.MapId);
                            Player[] players = CreatePlayers(cfg.PlayerCount);

                            Random rand = new Random();
                            var diceSeed = rand.Next();

                            GameBuilder<string> builder = new GameBuilder<string>();
                            builder.Players = players;
                            builder.MapGenerator = mapGenerator;
                            builder.MapSeeder = mapSeeder;
                            builder.PlayerStartingTroops = 20;
                            builder.AreaDistributionType = AreaDistributionType.Simple;
                            builder.AttackHandlerType = AttackHandlerType.Normal;
                            builder.Dice = new RandomDice(diceSeed);

                            game = builder.Build();

                            Console.WriteLine("New game started with id \"{0}\"", game.Id);
                            Console.WriteLine("Using seed \"{0}\" for dice", diceSeed);
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
                    GameAction<string> action = (comm as GameCommand).ToAction();
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

        private static MapGenerator<string> GetMapGeneratorById(int mapId)
        {
            return CreateTestMap();
        }

        private static MapGenerator<string> CreateTestMap()
        {
            // circle/star
            var mg = new MapGenerator<string>();

            mg.AddArea(0.ToString());
            mg.AddArea(1.ToString());
            mg.AddArea(2.ToString());
            mg.AddArea(3.ToString());
            mg.AddArea(4.ToString());

            mg.AddConnection(0.ToString(), 1.ToString());
            mg.AddConnection(1.ToString(), 2.ToString());
            mg.AddConnection(2.ToString(), 3.ToString());
            mg.AddConnection(3.ToString(), 4.ToString());
            mg.AddConnection(4.ToString(), 0.ToString());

            return mg;

        }
    }
}
