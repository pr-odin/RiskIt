using RiskIt.Main.Models;

namespace RiskIt.Main.Actions
{
    public class AttackAction : IAction
    {
        public string Attacker { get; set; }
        public int AttackingTroops { get; set; }
        public string Defender { get; set; }
        public int DefenderTroops { get; set; }
    }
}
