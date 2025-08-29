using RiskIt.ConsoleGame.Commands;
using RiskIt.ConsoleGame.Models;
using RiskIt.Main;
using RiskIt.Main.Actions;
using RiskIt.Main.AttackHandlers;
using RiskIt.Main.Events;
using RiskIt.Main.MapGeneration;
using RiskIt.Main.Models;
using RiskIt.Main.Models.Enums;
using Newtonsoft.Json;

namespace RiskIt.ConsoleGame
{
    public class Program
    {
        public static readonly int PLAYER_COUNT = 2;
        public static readonly int MAP_ID = 1;
        public static readonly int MAP_VISUALIZE_DIM = 5;

        public static void Main(string[] args)
        {
            Game<string>? game = null;
            GameClient[] gameClients = new GameClient[PLAYER_COUNT];
            GameClient activePlayer = gameClients[0];

            // probably not actually needed
            // a log of all events sent from the game to clients
            List<GameEvent> gameEvents = new List<GameEvent>();

            // the log of all actions done to the game
            List<GameAction<string>> gameActions = new List<GameAction<string>>();

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
                if (game is null) input = "server startgame"; // quick start game to see map
                else input = Console.ReadLine();

                if (input.Trim().ToLowerInvariant() == "exit") return;

                ICommand? comm = parser.Parse(input);

                if (comm is null) continue;

                if (!comm.GetType().Equals(typeof(ServerCommand)) && game is null)
                {
                    Console.WriteLine("No game started");
                    continue;
                }

                Type commandType = comm.GetType();

                if (commandType.Equals(typeof(ServerCommand)))
                {
                    // TODO:: Extract everything into "GameServer" layer ?
                    void HandleGameEvent(GameEvent gameEvent)
                    {
                        gameEvents.Add(gameEvent);
                        PropagateEvent(gameEvent: gameEvent, gameClients: gameClients, activePlayer: ref activePlayer);
                    }

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
                            gameClients = players.Select(p => new GameClient(p)).ToArray();

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
                            builder.OnEventCallBack = HandleGameEvent;

                            game = builder.Build();

                            Console.WriteLine("New game started with id \"{0}\"", game.Id);
                            Console.WriteLine("Using seed \"{0}\" for dice", diceSeed);

                            // print state of start game
                            Console.WriteLine(GetStateAsString(activePlayer));
                            PrintPaintAreasToConsole(MapVisualizer.PrintMap(game.GetMapAreas(), CreateMapId1(MAP_VISUALIZE_DIM)));

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
                    ((GameCommand)comm).GameClient = GetCurrentGameClient(gameClients);
                    GameAction<string> action;

                    try
                    {
                        action = (comm as GameCommand).ToAction();

                        // so far, just write all actions to the log
                        // TODO: Only write the actions that succeeded to the log
                        gameActions.Add(action);

                        var jsonAction = JsonConvert.SerializeObject(TypeWrapper<string>.SerializeWithType(action));
                        Console.WriteLine($"Action as json: {jsonAction}");

                        GameplayValidationType validation = game!.HandleAction(action);
                        if (validation != GameplayValidationType.Success)
                        {
                            // TODO: Add handling of action if wrong action
                            Console.WriteLine(validation.ToString());
                        }
                        Console.WriteLine(GetStateAsString(activePlayer));
                    }
                    catch (Exception e)
                    {
#if DEBUG
                        Console.WriteLine(e.Message);
#endif
                        comm = DisplayCommand.CreateUnknownCommand();
                    }
                }

                if (comm.GetType().Equals(typeof(DisplayCommand)))
                {
                    DisplayCommand dispComm = (DisplayCommand)comm;
                    switch (dispComm.DisplayCommandType)
                    {
                        case DisplayCommandType.Map:
                            PrintPaintAreasToConsole(
                                    MapVisualizer.PrintMap(game!.GetMapAreas(),
                                        CreateMapId1(MAP_VISUALIZE_DIM)));
                            break;
                        default:
                            Console.WriteLine(dispComm.Text);
                            break;
                    }
                    continue;

                }

                if (commandType.Equals(typeof(GameCommand)))
                {
                    ((GameCommand)comm).GameClient = GetCurrentGameClient(gameClients);
                    GameAction<string> action = (comm as GameCommand).ToAction();
                    //
                    GameplayValidationType validation = game!.HandleAction(action);
                    if (validation != GameplayValidationType.Success)
                    {
                        // TODO: Add handling of action if wrong action
                        Console.WriteLine(validation.ToString());
                    }
                    Console.WriteLine(GetStateAsString(activePlayer));
                }
            }
        }

        private static string GetStateAsString(GameClient gameClient)
        {
            var playerTurn = gameClient.PlayerTurn;

            return $"Active player: {playerTurn.Player.ToString()} | Phase: {playerTurn.Turn.Phase}";
        }

        private static void PropagateEvent(GameEvent gameEvent, GameClient[] gameClients, ref GameClient activePlayer)
        {
            switch (gameEvent.GetType())
            {

                case var type when type == typeof(PhaseAdvancedEvent):
                    foreach (var client in gameClients)
                    {
                        client.PlayerTurn.Turn.AdvanceTurn();
                    }
                    break;

                case var type when type == typeof(PlayerTurnChangedEvent):
                    activePlayer = gameClients
                        .Where(p => p.Player.Id == ((PlayerTurnChangedEvent)gameEvent).NextPlayerId)
                        .FirstOrDefault();

                    foreach (var client in gameClients)
                    {
                        client.PlayerTurn = new PlayerTurn
                        {
                            Player = activePlayer.Player,
                            Turn = new Turn()
                        };
                    }
                    break;

                case var type when type == typeof(GameStartEvent):
                    activePlayer = gameClients
                        .Where(p => p.Player.Id == ((GameStartEvent)gameEvent).NextPlayerId)
                        .FirstOrDefault();

                    foreach (var client in gameClients)
                    {
                        client.PlayerTurn = new PlayerTurn
                        {
                            Player = activePlayer.Player,
                            Turn = new Turn()
                        };
                        ;
                    }

                    break;

                default:
                    throw new Exception("Doing this later");
            }
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
