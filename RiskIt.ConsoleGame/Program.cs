using RiskIt.ConsoleGame.Commands;
using RiskIt.Main;
using RiskIt.Main.Actions;
using RiskIt.Main.AttackHandlers;
using RiskIt.Main.Events;
using RiskIt.Main.MapGeneration;
using RiskIt.Main.Models;
using RiskIt.Main.Models.Enums;

namespace RiskIt.ConsoleGame
{
    public class Program
    {
        public static readonly int PLAYER_COUNT = 2;
        public static readonly int MAP_ID = 1;

        public static void Main(string[] args)
        {
            /*GetObjectInMap();*/
            Game<string>? game = null;
            GameClient[] gameClients = new GameClient[PLAYER_COUNT];
            GameClient activePlayer = gameClients[0];

            List<GameEvent> gameEvents = new List<GameEvent>();

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
                    // TODO:: Extract everything into "GameServer" layer ?
                    void HandleGameEvent(GameEvent gameEvent)
                    {
                        gameEvents.Add(gameEvent);
                        PropagateEvent(gameEvent: gameEvent, gameClients: gameClients, activePlayer: ref activePlayer);
                    }

                    var serverComm = comm as ServerCommand;
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
                            PrintMap(game.GetMapAreas());

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
                    PrintMap(game.GetMapAreas());
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

        private static void PrintMap(List<Area<string>> gameMap)
        {

            // all items are 2dim x 2dim (aka a square)
            // in the grid where top left corner is 0,0 going positive down and right
            // the figures create a 5 sided star
            int dim = 5;

            (int y, int x)[] mapAreas = CreateMapId1(dim);

            // yes, I checked that this corresponds with the connections
            // and the order in the map
            int MAX_LENGTH = dim * 6;

            PaintArea[,] grid = new PaintArea[MAX_LENGTH + 1, MAX_LENGTH + 1];


            LinkedList<PaintArea>[] lines = new LinkedList<PaintArea>[MAX_LENGTH];

            for (int i = 0; i < lines.Length; i++)
            {
                PaintArea empty = new PaintArea
                {
                    Background = ConsoleColor.Black,
                    Foreground = ConsoleColor.White,
                    Char = ' ',
                    Length = MAX_LENGTH
                };
                lines[i] = new LinkedList<PaintArea>();
                lines[i].AddFirst(empty);
            }

            for (int mapAreaIndex = 0; mapAreaIndex < mapAreas.Length; mapAreaIndex++)
            {
                var count = 2 * dim;

                (int y, int x) mapArea = mapAreas[mapAreaIndex];
                Area<string> gameMapArea = gameMap.FirstOrDefault(area => area.Id == mapAreaIndex.ToString());

                PaintArea pa = new PaintArea
                {
                    Foreground = gameMapArea.Player.Id == 0 ? ConsoleColor.DarkRed : ConsoleColor.DarkBlue,
                    Char = 'X',
                    Length = 2 * dim
                };

                for (int lineY = mapArea.y; lineY < mapArea.y + (2 * dim); lineY++)
                {
                    LinkedList<PaintArea> currLine = lines[lineY];

                    int acc = 0;
                    LinkedListNode<PaintArea>? node = currLine.First;

                    while (node is not null)
                    {
                        var lineItem = node.Value;

                        if ((lineItem.Length + acc) > mapArea.x)
                        {
                            // so we need to insert the item "in" the item

                            PaintArea newPA = new PaintArea
                            {
                                Background = pa.Background,
                                Foreground = pa.Foreground,
                                Char = pa.Char,
                                Length = pa.Length
                            };

                            // diff between where we are now (acc)
                            // and the start of the figure
                            var newNode = InsertNewItem(node: node,
                                          currItemConfig: lineItem,
                                          startPosCurrItem: acc,
                                          startPosNewItem: mapArea.x,
                                          newItem: newPA);

                            if (lineY > mapArea.y && ((mapArea.y + (2 * dim) - 1) > lineY))
                                InsertItemInto(node: newNode);

                            break;
                        }
                        acc += lineItem.Length;
                        node = node.Next;
                    }
                }
            }
            PrintPaintAreasToConsole(lines);
        }

        private static void InsertItemInto(LinkedListNode<PaintArea> node)
        {
            int margin = 1;

            PaintArea item = node.Value;

#if DEBUG
            int totalLen = item.Length;
#endif

            PaintArea newEmpty = new PaintArea
            {
                Background = ConsoleColor.Black,
                Foreground = ConsoleColor.White,
                Char = ' ',
                Length = item.Length - (margin * 2)
            };


            item.Length = margin;

            PaintArea newDuplicateItem = CreateDuplicatePA(item);

#if DEBUG
            if (totalLen != item.Length + newEmpty.Length + newDuplicateItem.Length)
                throw new Exception("DEBUG! Length didn't match");
#endif

            node.List?.AddAfter(node, newDuplicateItem);
            node.List?.AddAfter(node, newEmpty);
        }

        private static PaintArea CreateDuplicatePA(PaintArea item)
        {
            return new PaintArea
            {
                Background = item.Background,
                Foreground = item.Foreground,
                Char = item.Char,
                Length = item.Length
            };
        }
        private static LinkedListNode<PaintArea> InsertNewItem(LinkedListNode<PaintArea> node,
                                                               PaintArea currItemConfig,
                                                               int startPosCurrItem,
                                                               int startPosNewItem,
                                                               PaintArea newItem)
        {
            int newPrevItemLength = startPosNewItem - startPosCurrItem;


            PaintArea newEmpty = new PaintArea
            {
                Background = currItemConfig.Background,
                Foreground = currItemConfig.Foreground,
                Char = currItemConfig.Char,
                Length = currItemConfig.Length - newPrevItemLength - newItem.Length
            };



            currItemConfig.Length = newPrevItemLength;

            node.List?.AddAfter(node, newEmpty);
            return node.List?.AddAfter(node, newItem);
        }

        // old list implementation
        // not actually in use, left for reference for now...
        private static void GetObjectInMap()
        {
            // all items are 2dim x 2dim (aka a square)
            // in the grid where top left corner is 0,0 going positive down and right
            // the figures create a 5 sided star
            int dim = 5;

            (int y, int x)[] mapAreas = CreateMapId1(dim);

            // yes, I checked that this corresponds with the connections
            // and the order in the map
            int MAX_LENGTH = dim * 6;

            PaintArea[,] grid = new PaintArea[MAX_LENGTH + 1, MAX_LENGTH + 1];


            List<PaintArea>[] lines = new List<PaintArea>[MAX_LENGTH];

            for (int i = 0; i < lines.Length; i++)
            {
                PaintArea empty = new PaintArea
                {
                    Background = ConsoleColor.Black,
                    Foreground = ConsoleColor.White,
                    Char = ' ',
                    Length = MAX_LENGTH
                };
                lines[i] = new List<PaintArea> { empty };
            }

            foreach (var mapArea in mapAreas)
            {
                var count = 2 * dim;


                PaintArea pa = new PaintArea
                {
                    Background = ConsoleColor.Cyan,
                    Foreground = ConsoleColor.DarkRed,
                    Char = 'X',
                    Length = 2 * dim
                };

                for (int lineY = mapArea.y; lineY < mapArea.y + (2 * dim); lineY++)
                {
                    var currLine = lines[lineY];

                    int acc = 0;
                    for (int i = 0; i < currLine.Count; i++)
                    {
                        var lineItem = currLine[i];
                        if ((lineItem.Length + acc) > mapArea.x)
                        {
                            // so we need to insert the item "in" the item

                            PaintArea newPA = new PaintArea
                            {
                                Background = pa.Background,
                                Foreground = pa.Foreground,
                                Char = pa.Char,
                                Length = pa.Length
                            };

                            // diff between where we are now (acc)
                            // and the start of the figure
                            int x_delta = mapArea.x - acc;


                            PaintArea newEmpty = new PaintArea
                            {
                                Background = lineItem.Background,
                                Foreground = lineItem.Foreground,
                                Char = lineItem.Char,
                                Length = lineItem.Length - x_delta - newPA.Length
                            };



                            lineItem.Length = x_delta;

                            currLine.Insert(i + 1, newEmpty);
                            currLine.Insert(i + 1, newPA);
                            break;
                        }
                        acc += lineItem.Length;
                    }
                }
            }

            PrintPaintAreasToConsole(lines);

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

                    string s = string.Join("", Enumerable.Repeat(item.Char, item.Length));
                    Console.Write(s);
                }
                Console.ResetColor();
                Console.WriteLine();
            }

            Console.ResetColor();
            Console.WriteLine();
        }
        public class PaintArea
        {
            public ConsoleColor Background;
            public ConsoleColor Foreground;
            public char Char;
            public int Length;
            public override string ToString()
            {
                return $"{Char} ({Length}) - {Foreground}";
            }
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
