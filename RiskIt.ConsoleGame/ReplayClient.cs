using RiskIt.Main;
using RiskIt.Main.Events;
using RiskIt.Main.Models.Enums;

namespace RiskIt.ConsoleGame
{
    public class ReplayClient
    {
        private GameServer<string> gameServer;

        public Guid ClientId { get; }

        private Action<int> _gameEnded;

        public ReplayClient(GameServer<string> gameServer,
                            Action<int> gameEnded,
                            Guid clientId)
        {
            this.gameServer = gameServer;

            ClientId = clientId;

            _gameEnded = gameEnded;
        }

        public void StartReplay(string replayName)
        {
            gameServer.StartReplay(Guid.Parse(replayName),
                                   ClientId,
                                   HandleEvent);

            if (!NextAction())
                throw new Exception("No actions found for the replay");
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

