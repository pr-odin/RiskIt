namespace RiskIt.Main.Events
{
    public class PlayerTurnChangedEvent : PhaseAdvancedEvent
    {
        public int NextPlayerId;

        public PlayerTurnChangedEvent(int nextPlayerId)
        {
            NextPlayerId = nextPlayerId;
        }
    }
}
