using RiskIt.ConsoleGame.Models.Enums;
using RiskIt.Main.Actions;

namespace RiskIt.ConsoleGame.Commands
{
    public class GameCommand : ICommand
    {

        public GameCommandType? _commandType;
        public string[]? _args;

        public void Parse(string[] args)
        {
            return;
        }

        public GameAction<string> ToAction()
        {
            switch (_commandType)
            {
                case GameCommandType.Placement:
                    // TODO: add some error handling here
                    string name = _args[0];
                    int troops = int.Parse(_args[1]);
                    return new PlacementAction<string>
                    {
                        Area = name,
                        Troops = troops
                    };

                case GameCommandType.Attack:
                    string attackerName = _args[0];
                    int attackerTroops = int.Parse(_args[1]);
                    string defenderName = _args[2];
                    return new AttackAction<string>
                    {
                        Attacker = attackerName,
                        AttackingTroops = attackerTroops,
                        Defender = defenderName,
                    };

                case GameCommandType.Fortify:
                    string fromName = _args[0];
                    string toName = _args[1];
                    int amount = int.Parse(_args[2]);
                    return new FortifyAction<string>
                    {
                        From = fromName,
                        To = toName,
                        Amount = amount
                    };
                default:
                    throw new Exception("Still doing exceptions later");
            }
        }
    }
}

