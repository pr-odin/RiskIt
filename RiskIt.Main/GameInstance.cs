using RiskIt.Main.Actions;
using RiskIt.Main.AttackHandlers;
using RiskIt.Main.Events;
using RiskIt.Main.MapGeneration;
using RiskIt.Main.Models;
using RiskIt.Main.Models.Enums;
using RiskIt.Main.Persist;

namespace RiskIt.Main
{
    public class GameInstance<T> where T : IComparable<T>
    {
        private Game<T>? _game;
        private int _diceSeed;
        private GameConfig? _gameConfig;

        private List<Player> _players;
        public int PlayerCount => _players.Count();

        public Guid GameId { get; private set; }
        private MapGenerator<T> _mapGenerator { get; }

        // the log of all actions done to the game
        private List<GameAction<T>> _gameActions;

        // probably not actually needed
        // a log of all events sent from the game to clients
        // TODO: make this into a static array upon ready
        // to start game to not be a memory pig
        private List<Action<GameEvent>> _gameEventHandlers;

        // probably only for debug and not actually useful
        // keeping while refactoring (hi me from 1 year later)
        private List<GameEvent> _gameEvents;

        private Action<GameAction<T>[]> _endOfGameAction = null;

        public bool GameStarted() => _gameStarted;
        // this used to be _game == null, but this way its a single bit
        // not sure if this actually matters but in my brain it make more
        // sense
        private bool _gameStarted;
        private int _index;

        private void ValidateGame()
        {
            if (!GameStarted())
                throw new Exception("No active game found");
        }

        public List<Area<T>> GetGameMap()
        {
            ValidateGame();

            return _game.GetMapAreas();
        }

        public GameRecord<T> GetGameRecord()
        {
            return new GameRecord<T>(
                   gameId: GameId,
                   diceSeed: _diceSeed,
                   gameConfig: _gameConfig,
                   playerCount: PlayerCount,
                   actions: _gameActions
                       .Select(action => TypeWrapper<T>.WrapAction(action))
           );
        }



        public GameInstance(Guid gameId,
                            Action<GameAction<T>[]> endOfGameAction,
                            MapGenerator<T> mapGenerator)
        {
            _gameActions = new List<GameAction<T>>();
            _endOfGameAction = endOfGameAction;
            _mapGenerator = mapGenerator;
            _gameEvents = new List<GameEvent>();
            _gameEventHandlers = new List<Action<GameEvent>>();
            _gameStarted = false;
            GameId = gameId;
            _players = new List<Player>();
            _index = 0;
        }





        public GameplayValidationType ProcessGameAction(GameAction<T> gameAction)
        {
            ValidateGame();

            // TODO: Only write the actions that succeeded to the log
            _gameActions.Add(gameAction);

            GameplayValidationType validation = _game.HandleAction(gameAction);

            if (validation == GameplayValidationType.GameEnded)
            {
                // TODO: extract to method to later be
                // able to get call who won the game etc
                _game = null;
                _gameStarted = false;

                _endOfGameAction(_gameActions.ToArray());
            }

            return validation;

        }

        private void HandleGameEvent(GameEvent gameEvent)
        {
            _gameEvents.Add(gameEvent);

            if (gameEvent is GameEndedEvent)
                _endOfGameAction(_gameActions.ToArray());

            foreach (var func in _gameEventHandlers)
            {
                func(gameEvent);
            }
        }

        #region SETUP

        public int RegisterGameClient(Action<GameEvent> gameEventHandler)
        {
            _gameEventHandlers.Add(gameEventHandler);
            int playerId = _index++;
            _players.Add(new Player { Id = playerId });
            return playerId;
        }

        public void RegisterSaveGameActions(Action<GameAction<T>[]> saveGameEvents)
        {
            this._endOfGameAction = saveGameEvents;
        }

        public GameSetupResult SetupGame(GameConfig cfg)
        {
            if (GameStarted()) throw new Exception("Smth like 'An active game is already ongoing!!', I think");

            Random rand = new Random();
            _diceSeed = rand.Next();

            Player[] players = _players.ToArray();


            AreaEnumeratorFactory<T> areaEnumeratorFactory = new AreaEnumeratorFactory<T>();

            GameBuilder<T> builder = new GameBuilder<T>();
            builder.Players = players;
            builder.MapGenerator = _mapGenerator;
            builder.MapSeeder = new MapSeeder<T>(areaEnumeratorFactory);
            builder.PlayerStartingTroops = (uint)cfg.StartingTroops;
            builder.AreaDistributionType =
                AreaDistributionTypeMethods.Parse(cfg.AreaDistributionType);
            builder.AttackHandlerType =
                AttackHandlerTypeMethods.Parse(cfg.AttackHandlerType);
            builder.Dice = new RandomDice(_diceSeed);
            builder.OnEventCallBack = HandleGameEvent;

            _game = builder.Build();
            _gameStarted = true;
            _gameConfig = cfg;

            return new GameSetupResult { GameId = this.GameId, DiceSeed = _diceSeed };
        }

        #endregion
    }

    public class GameSetupResult
    {
        public Guid GameId { get; set; }
        public int DiceSeed { get; set; }
    }
}
