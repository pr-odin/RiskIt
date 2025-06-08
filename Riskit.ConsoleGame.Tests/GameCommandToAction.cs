using RiskIt.ConsoleGame.Commands;
using RiskIt.ConsoleGame.Models.Enums;
using RiskIt.Main.Actions;

namespace Riskit.ConsoleGame.Tests
{
    public class GameCommandToAction
    {
        public GameCommand PlacementCommand(string[] args) => new GameCommand() { _commandType = GameCommandType.Placement, _args = args };
        public GameCommand AttackCommand(string[] args) => new GameCommand() { _commandType = GameCommandType.Attack, _args = args };
        public GameCommand FortifyCommand(string[] args) => new GameCommand() { _commandType = GameCommandType.Fortify, _args = args };

        [Test]
        public void Placement_BaseCase()
        {
            string[]? args = ["Jenny", "10"];

            var cmd = PlacementCommand(args);

            PlacementAction res = cmd.ToAction() as PlacementAction;

            Assert.IsNotNull(res);
            Assert.That(res.Area, Is.EqualTo("Jenny"));
            Assert.That(res.Troops, Is.EqualTo(10));
        }

        //[Test]
        //public void Placement_NoArgsGiven_ThrowException()
        //{
        //    string[]? args = null;

        //    var cmd = GetCommand(args);

        //    Assert.That(() => cmd.ToAction(), Throws.Exception);
        //}

        //[Test]
        //public void Placement_OneArgGiven_ThrowsException()
        //{
        //    string[]? args = ["Jenny"];

        //    var cmd = GetCommand(args);

        //    Assert.That(() => cmd.ToAction(), Throws.Exception);
        //}

        //[Test]
        //public void Placement_NotIntGiven_ThrowsException()
        //{
        //    string[]? args = ["Jenny", "IsOkay"];

        //    var cmd = GetCommand(args);

        //    Assert.That(() => cmd.ToAction(), Throws.Exception);
        //}

        [Test]
        public void Attack_BaseCase()
        {
            string[]? args = ["Jenny", "3", "IsOkay", "1"];

            var cmd = AttackCommand(args);

            AttackAction res = cmd.ToAction() as AttackAction;

            Assert.IsNotNull(res);
            Assert.That(res.Attacker, Is.EqualTo("Jenny"));
            Assert.That(res.AttackingTroops, Is.EqualTo(3));
            Assert.That(res.Defender, Is.EqualTo("IsOkay"));
            Assert.That(res.DefenderTroops, Is.EqualTo(1));
        }


        [Test]
        public void Fortify_BaseCase()
        {
            string[]? args = ["Jenny", "IsOkay", "3"];

            var cmd = FortifyCommand(args);

            FortifyAction res = cmd.ToAction() as FortifyAction;

            Assert.IsNotNull(res);
            Assert.That(res.From, Is.EqualTo("Jenny"));
            Assert.That(res.To, Is.EqualTo("IsOkay"));
            Assert.That(res.Amount, Is.EqualTo(3));
        }

    }
}
