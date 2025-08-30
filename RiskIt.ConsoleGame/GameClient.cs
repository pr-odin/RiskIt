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
        public Player Player { get; private set; }
        public PlayerTurn PlayerTurn { get; set; } // need to make sure this is set

        private GameServer<string> _gameServer;


        public GameClient(GameServer<string> gameServer, Player player)
        {
            _gameServer = gameServer;

            Player = player;
            PlayerTurn = new PlayerTurn();
            PlayerTurn.Player = player;
            PlayerTurn.Turn = new Turn();
        }

        public DisplayCommand? HandleGameCommand(GameCommand gameCommand)
        {
            GameAction<string> action;

            try
            {
                action = gameCommand.ToAction();

                GameplayValidationType validation = _gameServer.ProcessGameAction(action);

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

                // TODO: Do we send this from here or put it into a DisplayCommand ?
                // Console.WriteLine(GetStateAsString(activePlayer));
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

                default:
                    throw new Exception("Doing this later");
            }
        }

    }
}
