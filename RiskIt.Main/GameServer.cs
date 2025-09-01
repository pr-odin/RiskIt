using RiskIt.Main.Actions;
using RiskIt.Main.AttackHandlers;
using RiskIt.Main.Events;
using RiskIt.Main.MapGeneration;
using RiskIt.Main.Models;
using RiskIt.Main.Models.Enums;
using RiskIt.Main.Persist;

namespace RiskIt.Main
{
    public class GameServer<T> where T : IComparable<T>
    {
        private Game<T>? _game;
        private int _diceSeed;

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

        private Action<GameAction<T>[]>? saveGameEvents = null;

        public bool GameStarted() => _gameStarted;
        // this used to be _game == null, but this way its a single bit
        // not sure if this actually matters but in my brain it make more
        // sense
        private bool _gameStarted;

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
            ValidateGame();

            return new GameRecord<T>(
                   _game.Id,
                   _diceSeed,
                   _gameActions.Select(action => TypeWrapper<T>.WrapAction(action))
           );
        }



        public GameServer()
        {
            _gameActions = new List<GameAction<T>>();
            _gameEvents = new List<GameEvent>();
            _gameEventHandlers = new List<Action<GameEvent>>();
            _gameStarted = false;
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

                if (saveGameEvents != null)
                    saveGameEvents(_gameActions.ToArray());
            }

            return validation;

        }

        private void HandleGameEvent(GameEvent gameEvent)
        {
            _gameEvents.Add(gameEvent);
            foreach (var func in _gameEventHandlers)
            {
                func(gameEvent);
            }
        }

        #region SETUP

        public void RegisterGameClient(Action<GameEvent> gameEventHandler)
        {
            _gameEventHandlers.Add(gameEventHandler);
        }

        public void RegisterSaveGameActions(Action<GameAction<T>[]> saveGameEvents)
        {
            this.saveGameEvents = saveGameEvents;
        }

        // TODO: fix the signature to not include everyone and their friends
        public GameSetupResult SetupReplay(GameConfig cfg,
                                           MapSeeder<T> mapSeeder,
                                           MapGenerator<T> mapGenerator,
                                           int diceSeed)
        {
            if (GameStarted()) throw new Exception("Smth like 'An active game is already ongoing!!', I think");

            Player[] players = CreatePlayers(cfg.PlayerCount);

            _diceSeed = diceSeed;

            GameBuilder<T> builder = new GameBuilder<T>();
            builder.Players = players;
            builder.MapGenerator = mapGenerator;
            builder.MapSeeder = mapSeeder;
            builder.PlayerStartingTroops = 20;
            builder.AreaDistributionType = AreaDistributionType.Simple;
            builder.AttackHandlerType = AttackHandlerType.Normal;
            builder.Dice = new RandomDice(_diceSeed);
            builder.OnEventCallBack = HandleGameEvent;

            _game = builder.Build();
            _gameStarted = true;

            return new GameSetupResult { GameId = _game.Id, DiceSeed = _diceSeed };
        }
        public GameSetupResult SetupGame(GameConfig cfg, MapSeeder<T> mapSeeder, MapGenerator<T> mapGenerator)
        {
            if (GameStarted()) throw new Exception("Smth like 'An active game is already ongoing!!', I think");

            Player[] players = CreatePlayers(cfg.PlayerCount);

            Random rand = new Random();
            _diceSeed = rand.Next();

            GameBuilder<T> builder = new GameBuilder<T>();
            builder.Players = players;
            builder.MapGenerator = mapGenerator;
            builder.MapSeeder = mapSeeder;
            builder.PlayerStartingTroops = 20;
            builder.AreaDistributionType = AreaDistributionType.Simple;
            builder.AttackHandlerType = AttackHandlerType.Normal;
            builder.Dice = new RandomDice(_diceSeed);
            builder.OnEventCallBack = HandleGameEvent;

            _game = builder.Build();
            _gameStarted = true;

            return new GameSetupResult { GameId = _game.Id, DiceSeed = _diceSeed };
        }

        // TODO: Make this... otherwise :D
        // No but srsly, make it based on clients connected
        private static Player[] CreatePlayers(int playerCount)
        {
            var res = new Player[playerCount];

            for (int i = 0; i < playerCount; i++)
            {
                res[i] = new Player { Id = i };
            }

            return res;
        }

        #endregion
    }

    public class GameSetupResult
    {
        public Guid GameId { get; set; }
        public int DiceSeed { get; set; }
    }
}
