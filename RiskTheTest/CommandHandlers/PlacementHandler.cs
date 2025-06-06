using RiskTheTest.Actions;

namespace RiskTheTest.CommandHandlers
{
    public class PlacementHandler
    {
        public void HandleCommand(PlacementAction cmd)
        {
            cmd.Area.Troops += cmd.Troops;
        }
    }
}
