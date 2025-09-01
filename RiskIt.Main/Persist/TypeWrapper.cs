using RiskIt.Main.Actions;

namespace RiskIt.Main.Persist
{
    public class TypeWrapper<T> where T : IComparable<T>
    {
        public string? Type;
        public dynamic? Data;

        public static TypeWrapper<T> WrapAction(GameAction<T> action)
        {
            var wrapper = new TypeWrapper<T>
            {
                Type = action.TypeAsString(),
                Data = action
            };
            return wrapper;
        }

        public GameAction<T> UnwrapAction()
        {
            if (Data is null)
                throw new Exception("Data is null");

            switch (Type)
            {
                case "PlacementAction":
                    PlacementAction<T> placementAction = new PlacementAction<T>
                    {
                        Area = Data.Area,
                        Troops = Data.Troops,
                    };
                    return placementAction;
                case "AttackAction":
                    AttackAction<T> attackAction = new AttackAction<T>
                    {
                        Attacker = Data.Attacker,
                        AttackingTroops = Data.AttackingTroops,
                        Defender = Data.Defender
                    };
                    return attackAction;
                case "FortifyAction":
                    FortifyAction<T> fortifyAction = new FortifyAction<T>
                    {
                        To = Data.To,
                        From = Data.From,
                        Amount = Data.Amount,
                    };
                    return fortifyAction;
                case "SkipTurnAction":
                    SkipTurnAction<T> skipTurnAction = new SkipTurnAction<T>
                    {

                    };
                    return skipTurnAction;

            }

            throw new Exception("Ye... we ended up here somehow");

        }
    }
}
