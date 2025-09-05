namespace RiskIt.Main.Models
{
    public class PlayerTurn
    {
        public Turn Turn { get; set; }
        public Player Player { get; set; }

        public override string ToString()
        {
            return $"Active player: {Player.ToString()} | Phase: {Turn.Phase}";
        }
    }
}
