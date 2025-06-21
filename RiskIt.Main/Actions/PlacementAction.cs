using RiskIt.Main.Models;

namespace RiskIt.Main.Actions
{
    public class PlacementAction<T> : GameAction<T> where T : IComparable<T>
    {
        public T Area { get; set; }
        public int Troops { get; set; }
    }
}
