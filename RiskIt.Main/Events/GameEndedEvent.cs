namespace RiskIt.Main.Events
{
    public class GameEndedEvent : GameEvent
    {
        public int WonPlayerId { get; set; }

        public GameEndedEvent(int wonPlayerId)
        {
            WonPlayerId = wonPlayerId;
        }

    }
}

