using RiskIt.Main.Actions;
using RiskIt.Main.AttackHandlers;
using RiskIt.Main.Events;
using RiskIt.Main.MapGeneration;
using RiskIt.Main.Models;
using RiskIt.Main.Models.Enums;

namespace RiskIt.Main
{
    public class ReplayInstance<T> where T : IComparable<T>
    {
        private Game<T>? _game;
        public Guid GameId { get; private set; }
        public Guid ReplayId { get; private set; }
        private Action<GameEvent>? _gameEventHandler;
        private MapGenerator<T> _mapGenerator;

        private LinkedListNode<GameAction<T>>? _gameAction;

        public bool GameStarted() => _gameStarted;
        // this used to be _game == null, but this way its a single bit
        // not sure if this actually matters but in my brain it make more
        // sense
        private bool _gameStarted;
        public List<Area<T>> GetGameMap()
        {
            return _game.GetMapAreas();
        }

        public ReplayInstance(Guid gameId,
                              Guid replayId,
                              MapGenerator<T> mapGenerator,
                              LinkedListNode<GameAction<T>>? firstGameAction)
        {
            GameId = gameId;
            ReplayId = replayId;
            _mapGenerator = mapGenerator;
            _gameAction = firstGameAction;
        }

        public GameplayValidationType NextAction()
        {
            if (_gameAction == null) return GameplayValidationType.GameEnded;

            GameplayValidationType validation = _game.HandleAction(_gameAction.Value);
            _gameAction = _gameAction.Next;

            if (validation == GameplayValidationType.GameEnded)
            {
                // TODO: extract to method to later be
                // able to get call who won the game etc
                _game = null;
                _gameStarted = false;

            }

            return validation;

        }

        public bool RegisterGameClient(Action<GameEvent> gameEventHandler)
        {
            _gameEventHandler = gameEventHandler;
            return true;
        }


        // TODO: implement on end ?
        public void HandleGameEvent(GameEvent gameEvent)
        {

        }

        // TODO: fix the signature to not include everyone and their friends
        public GameSetupResult SetupReplay(GameConfig cfg,
                                           int diceSeed,
                                           int playerCount)
        {
            if (GameStarted()) throw new Exception("Smth like 'An active game is already ongoing!!', I think");

            Player[] players = new Player[playerCount];

            for (int i = 0; i < playerCount; i++)
                players[i] = new Player { Id = i };


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
            builder.Dice = new RandomDice(diceSeed);
            builder.OnEventCallBack = HandleGameEvent;

            _game = builder.Build();
            _gameStarted = true;

            return new GameSetupResult { GameId = this.GameId, DiceSeed = diceSeed };
        }
    }
}

