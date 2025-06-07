namespace RiskIt.Main.Models
{
    public class Area
    {
        public int Id { get; set; }
        public int Troops { get; set; }
        private IDictionary<int, Area> Connections;

        public Area(int id)
        {
            Id = id;
            Connections = new Dictionary<int, Area>();
        }

        public void AddConnection(Area other)
        {
            Connections[other.Id] = other;
            other.Connections[Id] = this;
        }
    }
}
