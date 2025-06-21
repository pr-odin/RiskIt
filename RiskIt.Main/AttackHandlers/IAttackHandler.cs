namespace RiskIt.Main.AttackHandlers
{
    public interface IAttackHandler
    {
        (int AttackingTroops, int DefendingTroops) BattleResult(int troopsAtk, int troopsDef);
    }
}