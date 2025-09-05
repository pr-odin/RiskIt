using RiskIt.ConsoleGame.Commands;
using RiskIt.Main;
using RiskIt.Main.Actions;
using RiskIt.Main.Events;
using RiskIt.Main.Models;
using RiskIt.Main.Models.Enums;

namespace RiskIt.ConsoleGame
{
    public class GameClient
    {
        public readonly Guid ClientId;
        public Guid? GameId;

        // TODO: Remove player from here, we dont need it to play
        public Player Player { get; private set; }
        public PlayerTurn PlayerTurn { get; set; } // need to make sure this is set

        private GameServer<string> _gameServer;

        private Action<int> _gameEnded;


        public GameClient(GameServer<string> gameServer,
                          Player player,
                          Action<int> gameEnded)
        {
            _gameServer = gameServer;
            _gameEnded = gameEnded;

            Player = player;
            PlayerTurn = new PlayerTurn();
            PlayerTurn.Player = player;
            PlayerTurn.Turn = new Turn();

            ClientId = Guid.NewGuid();
        }

        public string GetStateAsString()
        {
            if (GameId is null)
                return "No active game";

            return PlayerTurn.ToString();
        }

        public Guid CreateGame()
        {
            GameId = _gameServer.CreateGame(ClientId);

            if (GameId is null) throw new Exception("No game id was returned");

            if (!_gameServer.RegisterGameClient(ClientId,
                                           GameId ?? Guid.Empty,
                                           HandleEvent))
                throw new Exception("Could not register to the given game on the server");

            return GameId ?? throw new Exception("We already checked for this dotnet..");
        }

        public void ConnectToGame(Guid gameId)
        {
            if (GameId != null)
                throw new Exception("A game has already been started");

            if (!_gameServer.RegisterGameClient(ClientId,
                                           gameId,
                                           HandleEvent))
                throw new Exception("Could not register to the given game on the server");
        }


        public void StartGame()
        {
            if (GameId is null) throw new Exception("No game has been started yet");

            _gameServer.StartGame(ClientId);
        }

        public DisplayCommand? HandleGameCommand(GameCommand gameCommand)
        {
            GameAction<string> action;

            try
            {
                action = gameCommand.ToAction();

                GameplayValidationType validation = _gameServer.ProcessGameAction(ClientId, action);

                if (validation != GameplayValidationType.Success)
                {
                    // TODO: Add handling of action if wrong action
                    // so far we just print it
                    return new DisplayCommand()
                    {
                        Text = validation.ToString(),
                        DisplayCommandType = DisplayCommandType.Display,
                    };
                }
            }
            catch (Exception e)
            {
#if DEBUG
                Console.WriteLine(e.Message);
#endif
                return DisplayCommand.CreateUnknownCommand();
            }

            return null;
        }

        public void HandleEvent(GameEvent gameEvent)
        {
            switch (gameEvent.GetType())
            {
                case var type when type == typeof(PhaseAdvancedEvent):
                    PlayerTurn.Turn.AdvanceTurn();
                    break;

                case var type when type == typeof(PlayerTurnChangedEvent):

                    int nextPlayerId = ((PlayerTurnChangedEvent)gameEvent).NextPlayerId;

                    PlayerTurn = new PlayerTurn
                    {
                        Player = new Player()
                        {
                            Id = nextPlayerId
                        },
                        Turn = new Turn()
                    };
                    break;

                case var type when type == typeof(GameStartEvent):

                    int firstPlayerId = ((GameStartEvent)gameEvent).NextPlayerId;

                    PlayerTurn = new PlayerTurn
                    {
                        Player = new Player()
                        {
                            Id = firstPlayerId
                        },
                        Turn = new Turn()
                    };

                    break;

                case var type when type == typeof(GameEndedEvent):
                    GameEndedEvent gameEndedEvent = (GameEndedEvent)gameEvent;
                    GameId = null;
                    _gameEnded(gameEndedEvent.WonPlayerId);
                    break;

                default:
                    throw new Exception("Doing this later");
            }
        }

    }
}
