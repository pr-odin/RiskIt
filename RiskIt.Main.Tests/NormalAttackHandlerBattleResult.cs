using RiskIt.Main.AttackHandlers;

namespace RiskIt.Main.Tests
{
    public class NormalAttackHandlerBattleResult
    {
        [Test]
        public void Simple1v1AttackerWins()
        {
            int[] diceRolls = { 6, 1 };
            IDice dice = new PredictableDice(diceRolls);
            NormalAttackHandler attackHandler = new NormalAttackHandler(dice);

            int attackingTroops = 1;
            int defendingTroops = 1;
            var br = attackHandler.BattleResult(attackingTroops, defendingTroops);

            Assert.That(br.AttackingTroops, Is.EqualTo(1));
            Assert.That(br.DefendingTroops, Is.EqualTo(0));
        }

        [Test]
        public void Simple3v1AttackerWins_NoLoses()
        {
            int[] diceRolls = { 6, 6, 6, 1 };
            IDice dice = new PredictableDice(diceRolls);
            NormalAttackHandler attackHandler = new NormalAttackHandler(dice);

            int attackingTroops = 3;
            int defendingTroops = 1;
            var br = attackHandler.BattleResult(attackingTroops, defendingTroops);

            Assert.That(br.AttackingTroops, Is.EqualTo(3));
            Assert.That(br.DefendingTroops, Is.EqualTo(0));
        }

        [Test]
        public void A3v1AttackerWins_2nd3rdDice_NoLoses()
        {
            int[] diceRolls = { 1, 6, 6, 3 };
            IDice dice = new PredictableDice(diceRolls);
            NormalAttackHandler attackHandler = new NormalAttackHandler(dice);

            int attackingTroops = 3;
            int defendingTroops = 1;
            var br = attackHandler.BattleResult(attackingTroops, defendingTroops);

            Assert.That(br.AttackingTroops, Is.EqualTo(3));
            Assert.That(br.DefendingTroops, Is.EqualTo(0));
        }

        [Test]
        public void A3v1AttackerWins_1Tie1Win_1Loss()
        {
            int[] diceRolls = { 1, 3, 3, 3, 6, 6, 1 };
            IDice dice = new PredictableDice(diceRolls);
            NormalAttackHandler attackHandler = new NormalAttackHandler(dice);

            int attackingTroops = 3;
            int defendingTroops = 1;
            var br = attackHandler.BattleResult(attackingTroops, defendingTroops);

            Assert.That(br.AttackingTroops, Is.EqualTo(2));
            Assert.That(br.DefendingTroops, Is.EqualTo(0));
        }

        [Test]
        public void A3v2AttackerWins_1Win1Tie_1Loss()
        {
            int[] diceRolls = { 6, 3, 1, 5, 3, 6, 6, 1 };
            IDice dice = new PredictableDice(diceRolls);
            NormalAttackHandler attackHandler = new NormalAttackHandler(dice);

            int attackingTroops = 3;
            int defendingTroops = 2;
            var br = attackHandler.BattleResult(attackingTroops, defendingTroops);

            Assert.That(br.AttackingTroops, Is.EqualTo(2));
            Assert.That(br.DefendingTroops, Is.EqualTo(0));
        }

        [Test]
        public void A4v2AttackerWins_1Win1Loss_1Win_1Loss()
        {
            int[] diceRolls = { 6, 2, 1, 5, 3, 6, 6, 6, 1 };
            IDice dice = new PredictableDice(diceRolls);
            NormalAttackHandler attackHandler = new NormalAttackHandler(dice);

            int attackingTroops = 4;
            int defendingTroops = 2;
            var br = attackHandler.BattleResult(attackingTroops, defendingTroops);

            Assert.That(br.AttackingTroops, Is.EqualTo(3));
            Assert.That(br.DefendingTroops, Is.EqualTo(0));
        }

        public void A2v1AttackerLoses_1Loss_1Tie_1Defender()
        {
            // nice 2v1 noob
            int[] diceRolls = { 3, 2, 3,
                                2, 2 };
            IDice dice = new PredictableDice(diceRolls);
            NormalAttackHandler attackHandler = new NormalAttackHandler(dice);

            int attackingTroops = 2;
            int defendingTroops = 1;
            var br = attackHandler.BattleResult(attackingTroops, defendingTroops);

            Assert.That(br.AttackingTroops, Is.EqualTo(0));
            Assert.That(br.DefendingTroops, Is.EqualTo(1));
        }

    }
}
