namespace RiskIt.ConsoleGame.Commands
{
    public interface ICommand
    {
        public void Parse(string[] args);
    }
}
