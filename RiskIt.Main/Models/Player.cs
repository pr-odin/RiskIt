namespace RiskIt.Main.Models
{
    public class Player
    {
        public int Id { get; set; }

        public static bool operator ==(Player a, Player b) => a.Id == b.Id;
        public static bool operator !=(Player a, Player b) => a.Id != b.Id;

        public override string ToString()
        {
            return $"{Id}";
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(this, obj))
                return true;

            return false;

        }
    }
}
