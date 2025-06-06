using RiskTheTest.Models;

namespace RiskTheTest.Actions
{
    public class FortifyAction : IAction
    {
        public Area From { get; set; }
        public Area To { get; set; }
        public int Amount { get; set; }
    }
}
