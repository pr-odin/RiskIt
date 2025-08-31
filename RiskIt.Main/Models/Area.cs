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

        public bool IsAdjecent(Area<T> other)
        {
            return Connections.ContainsKey(other.Id);
        }

        public bool CanReach(Area<T> other)
        {
            if (IsAdjecent(other) && Player == other.Player)
                return true;

            HashSet<T> visited = new HashSet<T>();
            visited.Add(this.Id);

            return Search(other,
                          FriendlyAreas(other.Player),
                          visited);
        }

        private IEnumerable<Area<T>> FriendlyAreas(Player p) =>
            this.Connections.Values.Where(a => a.Player == p);

        private bool Search(Area<T> other,
                            IEnumerable<Area<T>> areas,
                            HashSet<T> visited)
        {
            if (areas.Count() == 0) return false;

            foreach (Area<T> area in areas)
            {
                if (area == other) return true;

                if (!visited.Add(area.Id)) continue;

                if (area.Search(other, area.FriendlyAreas(other.Player), visited)) return true;
            }

            return false;
        }

        public override string ToString()
        {
            return $"Id: {Id.ToString()}, Troops: {Troops.ToString()}, Player: {_playerText}";
        }

        public override bool Equals(object? obj)
        {
            return ReferenceEquals(this, obj);
        }
    }
}
