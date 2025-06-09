namespace RiskIt.Main.Models
{
    public class Area<T> where T : IComparable<T>
    {
        public T Id { get; set; }
        public string Name { get; set; }
        // belongs to which player
        public Player? Player { get; set; }
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
    }
}
