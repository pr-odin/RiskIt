using RiskIt.Main.Actions;
namespace RiskIt.Main.AttackHandlers
{
    public class SimpleAttackHandler : IAttackHandler
    {
        private IDice _dice;

        public SimpleAttackHandler(IDice dice)
        {
            _dice = dice;
        }
        public (int AttackingTroops, int DefendingTroops) BattleResult(int troopsAtk, int troopsDef)
        {
            // simple one dice vs one dice
            // for now...


            while (troopsAtk != 0 && troopsDef != 0)
            {
                var rollA = _dice.Next();
                var rollB = _dice.Next();

                if (rollA > rollB) // atk greater than def
                    troopsDef--;
                else //less or equal
                {
                    troopsAtk--;
                }
            }

            return (troopsAtk, troopsDef);
        }
    }
}
