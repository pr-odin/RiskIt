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
