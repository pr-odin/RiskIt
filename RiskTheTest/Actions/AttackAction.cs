using RiskTheTest.Models;

namespace RiskTheTest.Actions
{
    public class AttackAction : IAction
    {
        public Area Attacker { get; set; }
        public int AttackingTroops { get; set; }
        public Area Defender { get; set; }
        public int DefenderTroops { get; set; }
    }
}
