namespace RiskIt.Main.AttackHandlers
{
    public class NormalAttackHandler : IAttackHandler
    {
        private IDice _dice;

        public NormalAttackHandler(IDice dice)
        {
            _dice = dice;
        }

        public (int AttackingTroops, int DefendingTroops) BattleResult(int troopsAtk, int troopsDef)
        {
            int attackingTroops = troopsAtk;
            int defendingTroops = troopsDef;

            while (attackingTroops > 0 && defendingTroops > 0)
            {
                int currAtk, currDef;

                currAtk = GetDiceToRoll(attackingTroops);
                currDef = GetDiceToRoll(defendingTroops);

                IEnumerable<int> attackerRolls = GetRolls(currAtk).OrderDescending();
                IEnumerable<int> defenderRolls = GetRolls(currDef).OrderDescending();

                var attackEnumerator = attackerRolls.GetEnumerator();
                var defenceEnumerator = defenderRolls.GetEnumerator();

                while (true)
                {
                    if (!attackEnumerator.MoveNext() || !defenceEnumerator.MoveNext())
                        break;

                    if (attackingTroops < 1 || defendingTroops < 1)
                        return (attackingTroops, defendingTroops);

                    if (attackEnumerator.Current > defenceEnumerator.Current)
                        defendingTroops--;
                    else
                        attackingTroops--;
                }
            }
            return (attackingTroops, defendingTroops);
        }

        private List<int> GetRolls(int rolls)
        {
            List<int> ret = new List<int>();
            int c = 0;
            while (c < rolls)
            {
                ret.Add(_dice.Next());
                c++;
            }

            return ret;
        }
        private static int GetDiceToRoll(int troops)
        {
            if (troops >= 3)
                return 3;

            if (troops >= 2)
                return 2;

            return 1;
        }
    }
}
