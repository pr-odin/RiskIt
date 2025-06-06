using RiskTheTest.Models;

namespace RiskTheTest.Actions
{
    public class PlacementAction : IAction
    {
        public Area Area { get; set; }
        public int Troops { get; set; }
    }
}
