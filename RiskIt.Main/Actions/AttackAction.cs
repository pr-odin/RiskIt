namespace RiskIt.Main.Actions
{
    public class AttackAction<T> : GameAction<T> where T : IComparable<T>
    {
        public T Attacker { get; set; }
        public int AttackingTroops { get; set; }
        public T Defender { get; set; }

        public override string TypeAsString()
        {
            return "AttackAction";
        }
    }
}
