namespace RiskIt.Main.Events
{
    public class GameStartEvent : GameEvent
    {
        public int NextPlayerId;

        public GameStartEvent(int nextPlayerId)
        {
            NextPlayerId = nextPlayerId;
        }

    }
}
