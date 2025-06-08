using RiskIt.Main.Models;

namespace RiskIt.Main.Actions
{
    public class FortifyAction : IAction
    {
        public string From { get; set; }
        public string To { get; set; }
        public int Amount { get; set; }
    }
}
