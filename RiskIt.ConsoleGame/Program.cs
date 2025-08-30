using RiskIt.ConsoleGame.Commands;
using RiskIt.ConsoleGame.Models;
using RiskIt.Main;
using RiskIt.Main.MapGeneration;
using RiskIt.Main.Models;
using Newtonsoft.Json;
using RiskIt.Main.Persist;

namespace RiskIt.ConsoleGame
{
    public class Program
    {
        public static readonly int PLAYER_COUNT = 2;
        public static readonly int MAP_ID = 1;
        public static readonly int MAP_VISUALIZE_DIM = 5;

        public static void Main(string[] args)
        {
            GameServer<string> gameServer = new GameServer<string>();

            GameClient[] gameClients = new GameClient[PLAYER_COUNT];

            // FIX: This actually points to a null
            GameClient activePlayer = gameClients[0];

            AreaEnumeratorFactory<string> areaEnumeratorFactory = new AreaEnumeratorFactory<string>();
            MapSeeder<string> mapSeeder = new MapSeeder<string>(areaEnumeratorFactory);
            ConsoleParser parser = new ConsoleParser();


            /*Console.WriteLine(Console.BufferWidth);*/  // 120
            /*Console.WriteLine(Console.BufferHeight);*/ // 9001
            Console.BufferWidth = 120; // default 120
            /*Console.BufferHeight = 900;*/
            Console.SetWindowSize(Console.BufferWidth, 40);

            Console.WriteLine("Some generic message that the loop has been entered");

            while (true)
            {

                string? input;
                if (!gameServer.GameStarted()) input = "server startgame"; // quick start game to see map
                else input = Console.ReadLine();

                if (input.Trim().ToLowerInvariant() == "exit") return;

                ICommand? comm = parser.Parse(input);

                if (comm is null) continue;

                if (!comm.GetType().Equals(typeof(ServerCommand)) && !gameServer.GameStarted())
                {
                    Console.WriteLine("No game started");
                    continue;
                }

                Type commandType = comm.GetType();

                if (commandType.Equals(typeof(ServerCommand)))
                {
                    var serverComm = comm as ServerCommand;
                    try
                    {
                    switch (serverComm.CommandType)
                    {
                        case ServerCommandType.StartGame:

                            GameConfig cfg = serverComm.GameConfig;
                            cfg.PlayerCount = PLAYER_COUNT;
                            cfg.MapId = MAP_ID;

                            MapGenerator<string> mapGenerator = GetMapGeneratorById(cfg.MapId);

                            Player[] players = CreatePlayers(cfg.PlayerCount);

                                // TODO: should the flow be 
                                // 1. setup client
                                // 2. registers clients on server
                                // 3. setup game
                                // or is this fine ?
                                gameServer.SetupGame(cfg, mapSeeder, mapGenerator);

                                gameClients = players.Select(p => new GameClient(gameServer, p)).ToArray();
                                activePlayer = gameClients[0];

                                foreach (GameClient gameClient in gameClients)
                                {
                                    gameServer.RegisterGameClient(gameClient.HandleEvent);
                                }


                                // TODO: Implement back that a new game has started etc
                                // Console.WriteLine("New game started with id \"{0}\"", game.Id);
                                // Console.WriteLine("Using seed \"{0}\" for dice", diceSeed);


                            // print state of start game
                            Console.WriteLine(GetStateAsString(activePlayer));
                                PrintPaintAreasToConsole(
                                        MapVisualizer.PrintMap(
                                            gameServer.GetGameMap(),
                                            CreateMapId1(MAP_VISUALIZE_DIM)));

                            break;
                        case ServerCommandType.EndGame:

                                // TODO: Should this end the game or just persist it to file ?
                                GameRecord<string> gameResult = gameServer.GetGameRecord();

                                SaveGameToFile(gameResult);

                                Console.WriteLine("Game with id \"{0}\" has been saved (maybe terminated)", gameResult.GameId);
                            break;
                        default:
                            break;
                    }
                    continue;
                }
                    catch (Exception e)
                    {
#if DEBUG
                        Console.WriteLine(e.Message);
#endif
                        comm = DisplayCommand.CreateUnknownCommand();
                    }
                }

                if (commandType.Equals(typeof(GameCommand)))
                {
                    GameClient currClient = GetCurrentGameClient(gameClients);

                    GameCommand gameCommand = (GameCommand)comm;
                    gameCommand.GameClient = currClient;

                    comm = currClient.HandleGameCommand(gameCommand) ?? comm;

                        Console.WriteLine(GetStateAsString(activePlayer));
                    }

                if (comm.GetType().Equals(typeof(DisplayCommand)))
                {
                    DisplayCommand dispComm = (DisplayCommand)comm;
                    switch (dispComm.DisplayCommandType)
                    {
                        case DisplayCommandType.Map:
                            PrintPaintAreasToConsole(
                                    MapVisualizer.PrintMap(gameServer.GetGameMap(),
                                        CreateMapId1(MAP_VISUALIZE_DIM)));
                            break;
                        default:
                            Console.WriteLine(dispComm.Text);
                            break;
                    }
                    continue;
                }

                }
        }

        private static void SaveGameToFile(GameRecord<string> gameResult)
                {
            string path = AppDomain.CurrentDomain.BaseDirectory;

            string fileName = gameResult.GameId + ".txt";

            path = path + "GamesLog\\";

            Directory.CreateDirectory(path);

            string fullFileName = path + fileName;

            FileHandler.WriteToFile(path: fullFileName,
                                   content: JsonConvert.SerializeObject(gameResult));
        }
        private static string GetStateAsString(GameClient gameClient)
        {
            var playerTurn = gameClient.PlayerTurn;

            return $"Active player: {playerTurn.Player.ToString()} | Phase: {playerTurn.Turn.Phase}";
        }

        private static GameClient GetCurrentGameClient(GameClient[] gameClients)
        {
            Player currPlayer = gameClients[0].PlayerTurn.Player;

            return gameClients.Where(gc => gc.Player.Id == currPlayer.Id).FirstOrDefault();

        }

        private static Player[] CreatePlayers(int playerCount)
        {
            var res = new Player[playerCount];

            for (int i = 0; i < playerCount; i++)
            {
                res[i] = new Player { Id = i };
            }

            return res;
        }


        private static (int y, int x)[] CreateMapId1(int dim)
        {
            // we will use the start point which is top right of the figure
            // from that we can calculate everything else by using the square
            // principle

            (int y, int x)[] mapAreas = new (int, int)[5];
            mapAreas[0] = (1 * dim, 0 * dim);
            mapAreas[1] = (0 * dim, 2 * dim);
            mapAreas[2] = (1 * dim, 4 * dim);
            mapAreas[3] = (3 * dim, 3 * dim);
            mapAreas[4] = (3 * dim, 1 * dim);

            return mapAreas;
        }

        private static void PrintPaintAreasToConsole(IEnumerable<PaintArea>[] lines)
        {
            Console.WriteLine();
            foreach (var line in lines)
            {
                foreach (var item in line)
                {
                    /*Console.BackgroundColor = item.Background;*/
                    Console.ForegroundColor = item.Foreground;

                    string s = item.Word;
                    Console.Write(s);
                }
                Console.ResetColor();
                Console.WriteLine();
            }

            Console.ResetColor();
            Console.WriteLine();
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
