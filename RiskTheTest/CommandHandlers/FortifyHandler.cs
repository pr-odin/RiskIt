using RiskTheTest.Actions;

namespace RiskTheTest.CommandHandlers
{
    public class FortifyHandler
    {
        public void HandleCommand(FortifyAction cmd)
        {
            cmd.From.Troops -= cmd.Amount;
            cmd.To.Troops += cmd.Amount;
        }
    }
}
