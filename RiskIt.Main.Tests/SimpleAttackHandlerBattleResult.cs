using RiskIt.Main.AttackHandlers;

namespace RiskIt.Main.Tests
{
    public class SimpleAttackHandlerBattleResult
    {
        [Test]
        public void Simple1v1AttackerWins()
        {
            int[] diceRolls = { 6, 1 };
            IDice dice = new PredictableDice(diceRolls);
            SimpleAttackHandler attackHandler = new SimpleAttackHandler(dice);

            int attackingTroops = 1;
            int defendingTroops = 1;
            var br = attackHandler.BattleResult(attackingTroops, defendingTroops);

            Assert.That(br.AttackingTroops, Is.EqualTo(1));
            Assert.That(br.DefendingTroops, Is.EqualTo(0));
        }
    }
}
