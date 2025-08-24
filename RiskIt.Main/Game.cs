using RiskIt.Main.Actions;
using RiskIt.Main.AttackHandlers;
using RiskIt.Main.Models;
using RiskIt.Main.Models.Enums;

namespace RiskIt.Main
{
    public class Game<T> where T : IComparable<T>
    {
        public Guid Id { get; private set; }
        private IDictionary<T, Area<T>> _map { get; set; }
        private List<Player> _players { get; set; }
        public PlayerTurn GameTurn { get; private set; }
        private IAttackHandler _attackHandler;

        public Game(IDictionary<T, Area<T>> map,
            IEnumerable<Player> players,
            IAttackHandler attackHandler)
        {
            if (players.Count() < 2) throw new Exception("Smth like too few players");
            Id = Guid.NewGuid();
            _map = map;
            _players = players.ToList();
            GameTurn = new PlayerTurn { Player = _players.FirstOrDefault()!, Turn = new Turn() };
            _attackHandler = attackHandler;
        }

        public GameplayValidationType HandleAction(GameAction<T> action)
        {
            GameplayValidationType? retVal = null;
            switch (action.GetType())
            {
                case var type when type == typeof(PlacementAction<T>):
                    retVal = HandlePlacementAction((PlacementAction<T>)action);
                    break;

                case var type when type == typeof(AttackAction<T>):
                    retVal = HandleAttackAction((AttackAction<T>)action);
                    break;

                case var type when type == typeof(FortifyAction<T>):
                    retVal = HandleFortifyAction((FortifyAction<T>)action);
                    break;

            }

            if (retVal is null)
                return GameplayValidationType.DefaultCase;

            AdvanceTurn();
            // not actually default case ever, but to make compiler happy
            return retVal ?? GameplayValidationType.DefaultCase;
        }

        private GameplayValidationType HandlePlacementAction(PlacementAction<T> action)
        {
            if (GameTurn.Turn.Phase != Phase.Placement)
                return GameplayValidationType.WrongPhase;


            Area<T> area;
            if (!_map.TryGetValue(action.Area, out area))
                return GameplayValidationType.AreaNotFound;

            if (!area.Player.Equals(GameTurn.Player))
                return GameplayValidationType.NotPlayerTurn;

            area.Troops += action.Troops;
            return GameplayValidationType.Success;

        }
        private GameplayValidationType HandleAttackAction(AttackAction<T> action)
        {
            if (GameTurn.Turn.Phase != Phase.Attack)
                return GameplayValidationType.WrongPhase;

            Area<T> attacker, defender;

            if (!_map.TryGetValue(action.Attacker, out attacker))
                return GameplayValidationType.AreaNotFound;
            if (!_map.TryGetValue(action.Defender, out defender))
                return GameplayValidationType.AreaNotFound;

            if (!GameTurn.Player.Equals(attacker.Player))
                return GameplayValidationType.NotPlayerTurn;
            if (attacker.Player.Equals(defender.Player))
                return GameplayValidationType.SamePlayersArea;

            if (action.AttackingTroops + 1 > attacker.Troops)
                return GameplayValidationType.TooManyTroops;


            // insert some attacking here
            var br = _attackHandler.BattleResult(action.AttackingTroops, defender.Troops);

            attacker.Troops -= action.AttackingTroops + br.AttackingTroops;
            defender.Troops = br.DefendingTroops;

            if (defender.Troops == 0)
            {
                defender.Player = attacker.Player;
                // only have to move one troop to the next territory for now
                defender.Troops = 1;
                attacker.Troops -= 1;
            }

            return GameplayValidationType.Success;
        }
        private GameplayValidationType HandleFortifyAction(FortifyAction<T> action)
        {
            if (GameTurn.Turn.Phase != Phase.Fortify)
                return GameplayValidationType.WrongPhase;

            Area<T> from, to;
            if (!_map.TryGetValue(action.From, out from))
                return GameplayValidationType.AreaNotFound;
            if (!_map.TryGetValue(action.To, out to))
                return GameplayValidationType.AreaNotFound;

            if (action.Amount > from.Troops - 1)
                return GameplayValidationType.TooManyTroops;

            if (from.Player is null)
                return GameplayValidationType.AreaUnoccupied;
            if (!from.Player.Equals(to.Player))
                return GameplayValidationType.SamePlayersArea;

            if (!from.Player.Equals(GameTurn.Player))
                return GameplayValidationType.NotPlayerTurn;

            from.Troops -= action.Amount;
            to.Troops += action.Amount;

            return GameplayValidationType.Success;
        }

        public int CalculateNextPlayerTurn(Player player)
        {
            int playerId = player.Id;
            var playerIndex = _players.FindIndex(p => p.Id == playerId);

            if (playerIndex == _players.Count - 1)
                return 0;

            return playerIndex++;
        }

        public void AdvanceTurn()
        {
            if (GameTurn is null || !GameTurn.Turn.AdvanceTurn())
            {
                PlayerTurn newPlayerTurn = new PlayerTurn
                {
                    Player = _players[CalculateNextPlayerTurn(GameTurn!.Player)],
                    Turn = new Turn()
                };

                GameTurn = newPlayerTurn;
            }
        }
    }
}
