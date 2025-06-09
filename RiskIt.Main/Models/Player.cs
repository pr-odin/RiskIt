namespace RiskIt.Main.Models
{
    public class Player
    {
        public int Id { get; set; }

        public override string ToString()
        {
            return $"{Id}";
        }
    }
}