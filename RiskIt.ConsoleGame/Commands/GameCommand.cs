using RiskIt.ConsoleGame.Models.Enums;
using RiskIt.Main.Actions;
using RiskIt.Main.Models.Enums;

namespace RiskIt.ConsoleGame.Commands
{
    public class GameCommand : ICommand
    {

        public GameClient? GameClient;
        public GameCommandType? _commandType;
        public string[]? _args;

        public void Parse(string[] args)
        {
            _args = args;
        }

        public GameAction<string> ToAction()
        {
            if (_commandType is null)
                _commandType = GetGameCommandType();

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

                    if (_args[0] == "s" || _args[0] == "skip")
                        return new FinishTurnAction<string>();

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

                    if (_args[0] == "s" || _args[0] == "skip")
                        return new FinishTurnAction<string>();

                    string fromName = _args[0];
                    int amount = int.Parse(_args[1]);
                    string toName = _args[2];
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

        private GameCommandType GetGameCommandType()
        {
            if (GameClient is null) throw new Exception("Writing these later");

            switch (GameClient.PlayerTurn.Turn.Phase)
            {
                case (Phase.Placement):
                    return GameCommandType.Placement;
                case (Phase.Attack):
                    return GameCommandType.Attack;
                case (Phase.Fortify):
                    return GameCommandType.Fortify;
                default:
                    throw new Exception("Writing these later");
            }
        }
    }
}

