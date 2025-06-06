using RiskTheTest.Actions;

namespace RiskTheTest.ConsoleGame.Commands
{
    public class GameCommand : ICommand
    {
        public void Parse(string[] args)
        {
            return;
        }

        public IAction ToAction()
        {
            throw new NotImplementedException();
        }
    }
}
