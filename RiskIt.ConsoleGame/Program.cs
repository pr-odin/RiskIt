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
        public static readonly string REPLAY_PATH = AppDomain.CurrentDomain.BaseDirectory + "GamesLog\\";



        public static void Main(string[] args)
        {
            GameRecord<string> GetGameActions(string gameId)
            {
                GameRecord<string> gameRecord = LoadFromFile(REPLAY_PATH + gameId + ".json");

                return gameRecord;
            }

            MapGenerator<string> mapGenerator = MapGenerator<string>.CreateTestMap();
            ReplayLibrary<string> replayLibrary = new ReplayLibrary<string>(GetReplayFiles(),
                                                                            GetGameActions);

            GameServer<string> gameServer = new GameServer<string>(mapGenerator, replayLibrary);
            gameServer.RegisterWriteRecordToFile(SaveGameToFile);

            GameClient[] gameClients = new GameClient[PLAYER_COUNT];
            for (int i = 0; i < PLAYER_COUNT; i++)
            {
                gameClients[i] = new GameClient(gameServer: gameServer,
                                            player: new Player { Id = i },
                                            gameEnded: PrintGameEnded);
            }

            ReplayClient? replayClient = null;

            GameClient activePlayer = gameClients[0];

            ConsoleParser parser = new ConsoleParser();


            /*Console.WriteLine(Console.BufferWidth);*/  // 120
            /*Console.WriteLine(Console.BufferHeight);*/ // 9001
            Console.BufferWidth = 120; // default 120
            /*Console.BufferHeight = 900;*/
            Console.SetWindowSize(Console.BufferWidth, 40);

            Console.WriteLine("Some generic message that the loop has been entered");

            while (true)
            {
                string? input = Console.ReadLine();

                if (input.Trim().ToLowerInvariant() == "exit") return;

                ICommand? comm = parser.Parse(input);

                if (comm is null) continue;

                Type commandType = comm.GetType();

                if (commandType.Equals(typeof(ServerCommand)))
                {
                    var serverComm = comm as ServerCommand;
                    try
                    {
                        switch (serverComm.CommandType)
                        {
                            case ServerCommandType.StartGame:

                                Guid gameId = gameClients[0].CreateGame();

                                for (int i = 1; i < PLAYER_COUNT; i++)
                                {
                                    gameClients[i].ConnectToGame(gameId);
                                }

                                activePlayer.StartGame();

                                Console.WriteLine("New game started with id \"{0}\"", gameId);

                                // print state of start game
                                Console.WriteLine(GetStateAsString(activePlayer));
                                PrintPaintAreasToConsole(
                                        MapVisualizer.PrintMap(
                                            gameServer.GetGameMap(activePlayer.ClientId),
                                            CreateMapId1(MAP_VISUALIZE_DIM)));

                                break;
                            case ServerCommandType.ReplayGame:

                                ServerCommand serverCommand = ((ServerCommand)comm);

                                GameConfig replayCfg = new GameConfig();
                                replayCfg.PlayerCount = PLAYER_COUNT;
                                replayCfg.MapId = MAP_ID;

                                string path = REPLAY_PATH
                                    + serverCommand.ReplayName;

                                // GameRecord<string> gameRecord = LoadFromFile(path);
                                // GameAction<string>[] gameActions = gameRecord.Actions
                                //     .Select(typeWrapper => typeWrapper.UnwrapAction())
                                //     .ToArray<GameAction<string>>();

                                Guid replayClientId = Guid.NewGuid();

                                gameServer.StartReplay(Guid.Parse(serverCommand.ReplayName),
                                                       replayClientId);

                                replayClient = new ReplayClient(gameServer,
                                                                null,
                                                                PrintGameEnded,
                                                                replayClientId);

                                replayClient.NextAction();


                                // // TODO: probably make it a register REPLAY client
                                // gameServer.RegisterGameClient(replayClient.HandleEvent);


                                PrintPaintAreasToConsole(
                                        MapVisualizer.PrintMap(
                                            gameServer.GetGameMap(replayClient.ClientId),
                                            CreateMapId1(MAP_VISUALIZE_DIM)));

                                break;
                            case ServerCommandType.EndGame:
                                throw new NotImplementedException("Removed feature to just end game. Will add back as a leave client side ?");

                                // TODO: Should this end the game or just persist it to file ?
                                // GameRecord<string> gameResult = gameServer.GetGameRecord();
                                //
                                // SaveGameToFile(gameResult);
                                //
                                // Console.WriteLine("Game with id \"{0}\" has been saved (maybe terminated)", gameResult.GameId);
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
                if (commandType.Equals(typeof(ReplayCommand)))
                {
                    ReplayCommand replayCommand = ((ReplayCommand)comm);

                    // TODO: normal error handling and not crash/unknown command
                    if (replayClient is null)
                        throw new Exception("No replay client");

                    switch (replayCommand.commandType)
                    {
                        case ReplayCommandType.NextAction:
                            // yeah... but this bypasses weird reference/who can do what
                            int c = replayCommand.Step ?? 1;

                            while (c > 0)
                            {
                                // TODO: make it actually show the map every time it runs
                                // this comes when we start queueing commands (if we do that)
                                if (!replayClient.NextAction())
                                    Console.WriteLine("Replay has ended");
                                else
                                {
                                    comm = new DisplayCommand()
                                    {
                                        DisplayCommandType = DisplayCommandType.Map,
                                    };
                                }
                                c--;
                            }

                            break;
                        default:
                            comm = DisplayCommand.CreateUnknownCommand();
                            break;
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
                            Guid mapDrawingClientId = activePlayer.ClientId;

                            // FIX: a simple hardcode way to either do replays or game
                            if (replayClient != null)
                                mapDrawingClientId = replayClient.ClientId;

                            PrintPaintAreasToConsole(
                                    MapVisualizer.PrintMap(gameServer.GetGameMap(mapDrawingClientId),
                                                           CreateMapId1(MAP_VISUALIZE_DIM)));
                            break;
                        case DisplayCommandType.Replays:
                            PrintReplayFiles(REPLAY_PATH);
                            break;
                        default:
                            Console.WriteLine(dispComm.Text);
                            break;
                    }
                    continue;
                }

            }
        }

        private static void PrintGameEnded(int wonPlayerId)
        {
            Console.WriteLine($"Game has ended. Player with id {wonPlayerId} has won!");
        }

        private static void PrintReplayFiles(string replayPath)
        {
            DirectoryInfo di = new DirectoryInfo(replayPath);
            FileInfo[] files = di.GetFiles();

            foreach (var file in files)
            {
                Console.WriteLine(file.Name);
            }
        }
        private static string[] GetReplayFiles()
        {
            DirectoryInfo di = new DirectoryInfo(REPLAY_PATH);
            FileInfo[] files = di.GetFiles();

            return files.Select(e => e.Name.Split(".")[0]).ToArray();

        }

        private static void SaveGameToFile(GameRecord<string> gameResult)
        {
            string fileName = gameResult.GameId + ".json";


            Directory.CreateDirectory(REPLAY_PATH);

            string fullFileName = REPLAY_PATH + fileName;

            FileHandler.WriteToFile(path: fullFileName,
                                    content: JsonConvert.SerializeObject(gameResult));
        }

        private static GameRecord<string> LoadFromFile(string path)
        {
            string jsonDoc = FileHandler.ReadFromFile(path);

            GameRecord<string>? gameRecord = JsonConvert.DeserializeObject<GameRecord<string>>(jsonDoc);

            if (gameRecord is null)
                throw new Exception("Could not load game record");

            return gameRecord;
        }

        private static string GetStateAsString(GameClient gameClient)
        {
            var playerTurn = gameClient.PlayerTurn;
            return GetStateAsString(playerTurn);
        }
        private static string GetStateAsString(PlayerTurn playerTurn)
        {
            return $"Active player: {playerTurn.Player.ToString()} | Phase: {playerTurn.Turn.Phase}";
        }

        private static GameClient GetCurrentGameClient(GameClient[] gameClients)
        {
            Player currPlayer = gameClients[0].PlayerTurn.Player;

            return gameClients.Where(gc => gc.Player.Id == currPlayer.Id).FirstOrDefault();

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
    }
}
