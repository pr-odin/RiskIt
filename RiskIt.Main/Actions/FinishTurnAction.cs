namespace RiskIt.Main.Actions
{
    public class FinishTurnAction<T> : GameAction<T> where T : IComparable<T>
    {
        public override string TypeAsString()
        {
            return "FinishTurnAction";
        }
    }
}

