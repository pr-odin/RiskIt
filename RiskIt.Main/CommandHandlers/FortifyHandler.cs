using RiskIt.Main.Actions;

namespace RiskIt.Main.CommandHandlers
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
