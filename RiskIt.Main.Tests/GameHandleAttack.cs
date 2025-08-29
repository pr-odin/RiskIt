using RiskIt.Main.Models;

namespace RiskIt.Main.Tests
{
    public class GameHandleAttack
    {
        [Test]
        public void HandleBattleResult_5Att_0Def_Used10_AttackersMovedToNewTeritory()
        {

            Player player0 = new Player() { Id = 0 };
            Player player1 = new Player() { Id = 1 };

            (int AttackingTroops, int DefendingTroops) battleResult = (5, 0);
            int usedAttackingTroops = 10;

            Area<string> attacker = new Area<string>("0");
            attacker.Player = player0;
            attacker.Troops = 11;

            Area<string> defender = new Area<string>("1");
            defender.Player = player1;
            defender.Troops = 5;

            Game<string>.HandleBattleResult(battleResult, usedAttackingTroops, attacker, defender);

            Assert.That(attacker.Player, Is.EqualTo(defender.Player));
            Assert.That(attacker.Troops, Is.EqualTo(11 - usedAttackingTroops));
            Assert.That(defender.Troops, Is.EqualTo(battleResult.AttackingTroops));
        }
    }
}
