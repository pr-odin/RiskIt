using RiskIt.Main;
using RiskIt.Main.Actions;
using RiskIt.Main.Events;
using RiskIt.Main.Models.Enums;

namespace RiskIt.ConsoleGame
{
    public class ReplayClient
    {
        private GameServer<string> gameServer;
        private GameAction<string>[] gameActions;
        private int index;

        public Guid ClientId { get; }

        private Action<int> _gameEnded;

        public ReplayClient(GameServer<string> gameServer,
                            GameAction<string>[] gameActions,
                            Action<int> gameEnded,
                            Guid clientId)
        {
            this.gameServer = gameServer;
            this.gameActions = gameActions;
            index = 0;

            ClientId = clientId;

            _gameEnded = gameEnded;
        }

        public bool NextAction()
        {
            GameplayValidationType validation = gameServer.ReplayNext(ClientId);

            // TODO: When we have only valid events, this should probably do something
            if (validation != GameplayValidationType.Success)
            {
                Console.WriteLine(validation);
            }

            return validation != GameplayValidationType.GameEnded;
        }

        // empty as we don't care about events when playing back
        public void HandleEvent(GameEvent gameEvent)
        {
            switch (gameEvent.GetType())
            {
                case var type when type == typeof(GameEndedEvent):
                    GameEndedEvent gameEndedEvent = (GameEndedEvent)gameEvent;
                    _gameEnded(gameEndedEvent.WonPlayerId);
                    break;
            }

        }

    }
}

