using RiskIt.Main.Actions;
using RiskIt.Main.AttackHandlers;
using RiskIt.Main.Events;
using RiskIt.Main.Models;
using RiskIt.Main.Models.Enums;

namespace RiskIt.Main
{
    // TODO: Cards ?
    public class Game<T> where T : IComparable<T>
    {
        private IDictionary<T, Area<T>> _map { get; set; }
        private List<Player> _players { get; set; }
        public PlayerTurn GameTurn { get; private set; }
        private IAttackHandler _attackHandler;
        private Action<GameEvent> _eventCallBack;

        private static readonly int PLACEMENT_TROOPS = 4;
        private PlacementHandler _placementHandler;

        public Game(IDictionary<T, Area<T>> map,
            IEnumerable<Player> players,
            IAttackHandler attackHandler,
            Action<GameEvent> EventCallBack)
        {
            if (players.Count() < 2) throw new Exception("Smth like too few players");
            _map = map;
            _players = players.ToList();

            GameTurn = new PlayerTurn { Player = _players.FirstOrDefault()!, Turn = new Turn() };
            _placementHandler = new PlacementHandler(PLACEMENT_TROOPS);

            _attackHandler = attackHandler;
            _eventCallBack = EventCallBack;


            var gameStartEvent = new GameStartEvent(GameTurn.Player.Id);
            EventCallBack(gameStartEvent);
        }

        private bool GameHasEnded()
        {
            int firstPlayerId = _map.Values.FirstOrDefault().Player.Id;
            if (_map.Values.All(area => area.Player.Id.Equals(firstPlayerId)))
                return true;

            return false;
        }

        public GameplayValidationType HandleAction(GameAction<T> action)
        {
            // TODO: put this BEFORE sending the final event
            // so probably where we advance turn
            if (GameHasEnded()) return GameplayValidationType.GameEnded;

            GameplayValidationType? retVal = null;
            switch (action.GetType())
            {
                case var type when type == typeof(PlacementAction<T>):
                    retVal = HandlePlacementAction((PlacementAction<T>)action);

                    if (retVal == GameplayValidationType.Success
                        && (GameTurn.Turn.Phase != Phase.Placement || _placementHandler.IsFinished))
                        AdvanceTurn();

                    break;

                case var type when type == typeof(AttackAction<T>):
                    retVal = HandleAttackAction((AttackAction<T>)action);
                    break;

                case var type when type == typeof(FortifyAction<T>):
                    retVal = HandleFortifyAction((FortifyAction<T>)action);

                    if (retVal == GameplayValidationType.Success)
                        AdvanceTurn();

                    break;
                case var type when type == typeof(SkipTurnAction<T>):
                    retVal = HandleFinishTurnAction((SkipTurnAction<T>)action);

                    if (retVal == GameplayValidationType.Success)
                        AdvanceTurn();

                    break;
            }

            if (GameHasEnded())
            {
                // the dude who won... not sure how else to write that
                // this only works because there are no other players with areas
                int WonPlayerId = _map.Values.FirstOrDefault().Player.Id;
                GameEndedEvent gameEndedEvent = new GameEndedEvent(WonPlayerId);
                _eventCallBack(gameEndedEvent);
            }

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

            if (action.Troops > _placementHandler.AvailableTroops)
                return GameplayValidationType.TooManyTroops;

            area.Troops += action.Troops;
            _placementHandler.AvailableTroops -= action.Troops;

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

            if (!attacker.IsAdjecent(defender))
                return GameplayValidationType.AreaUnreachable;


            // insert some attacking here
            var br = _attackHandler.BattleResult(action.AttackingTroops, defender.Troops);

            HandleBattleResult(br, action.AttackingTroops, attacker, defender);

            return GameplayValidationType.Success;
        }

        public static void HandleBattleResult(
                (int AttackingTroops, int DefendingTroops) battleResult,
                int usedAttackingTroops,
                Area<T> attacker,
                Area<T> defender)
        {
            attacker.Troops -= usedAttackingTroops;
            defender.Troops = battleResult.DefendingTroops;

            // aka attacker won
            if (defender.Troops == 0)
            {
                defender.Player = attacker.Player;
                defender.Troops = battleResult.AttackingTroops;
            }
        }


        private GameplayValidationType HandleFinishTurnAction(SkipTurnAction<T> action)
        {
            if (GameTurn.Turn.Phase == Phase.Placement)
                return GameplayValidationType.WrongPhase;

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

            if (!from.CanReach(to))
                return GameplayValidationType.AreaUnreachable;

            from.Troops -= action.Amount;
            to.Troops += action.Amount;

            return GameplayValidationType.Success;
        }

        public List<Area<T>> GetMapAreas() => _map.Values.ToList();

        public int CalculateNextPlayerTurn(Player player)
        {
            int playerId = player.Id;
            var playerIndex = _players.FindIndex(p => p.Id == playerId);

            if (playerIndex == _players.Count - 1)
                return 0;

            return ++playerIndex;
        }

        public void AdvanceTurn()
        {
            var newEvent = new PhaseAdvancedEvent();
            if (GameTurn is null || !GameTurn.Turn.AdvanceTurn())
            {
                var nextPlayer = _players[CalculateNextPlayerTurn(GameTurn!.Player)];
                PlayerTurn newPlayerTurn = new PlayerTurn
                {
                    Player = nextPlayer,
                    Turn = new Turn()
                };
                newEvent = new PlayerTurnChangedEvent(nextPlayer.Id);

                GameTurn = newPlayerTurn;
                _placementHandler = new PlacementHandler(PLACEMENT_TROOPS);
            }
            _eventCallBack(newEvent);
        }
    }
}
