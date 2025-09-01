namespace RiskIt.Main.Actions
{
    public class SkipTurnAction<T> : GameAction<T> where T : IComparable<T>
    {
        public override string TypeAsString()
        {
            return "SkipTurnAction";
        }
    }
}

