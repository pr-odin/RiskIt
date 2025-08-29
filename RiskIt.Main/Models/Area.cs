namespace RiskIt.Main.Models
{
    public class Area<T> where T : IComparable<T>
    {
        public T Id { get; set; }
        public string? Name { get; set; }
        // belongs to which player
        public Player? Player { get; set; }
        //debug helper
        private string _playerText => Player is null ? "Unoccupied" : Player.ToString();
        public int Troops { get; set; } = 0;
        private IDictionary<T, Area<T>> Connections;

        public Area(T id)
        {
            Id = id;
            Connections = new Dictionary<T, Area<T>>();
        }

        public void AddConnection(Area<T> other)
        {
            Connections[other.Id] = other;
            other.Connections[Id] = this;
        }

        public override string ToString()
        {
            return $"Id: {Id.ToString()}, Troops: {Troops.ToString()}, Player: {_playerText}";
        }
    }
}
