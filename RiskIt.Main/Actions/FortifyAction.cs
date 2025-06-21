using RiskIt.Main.Models;

namespace RiskIt.Main.Actions
{
    public class FortifyAction<T> : GameAction<T> where T : IComparable<T>
    {
        public T From { get; set; }
        public T To { get; set; }
        public int Amount { get; set; }
    }
}
