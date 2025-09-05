using RiskIt.Main.Actions;
using RiskIt.Main.Events;
using RiskIt.Main.MapGeneration;
using RiskIt.Main.Models;
using RiskIt.Main.Models.Enums;
using RiskIt.Main.Persist;

namespace RiskIt.Main
{
    public class GameServer<T> where T : IComparable<T>
    {
        public static readonly int MAP_ID = 1;
        private readonly MapGenerator<T> _mapGenerator;

        private Dictionary<Guid, GameInstance<T>> _games;
        private Dictionary<Guid, ConnectedClient> _gameClients;
        private Dictionary<Guid, List<Guid>> _gameToGameClients;

        private Dictionary<Guid, ReplayInstance<T>> _replays;
        private ReplayLibrary<T> _replayLibrary;

        public Action<GameRecord<T>>? WriteRecordToFile;

        public GameServer(MapGenerator<T> mapGenerator,
                          ReplayLibrary<T> replayLibrary)
        {
            _games = new Dictionary<Guid, GameInstance<T>>();
            _gameClients = new Dictionary<Guid, ConnectedClient>();
            _gameToGameClients = new Dictionary<Guid, List<Guid>>();
            _mapGenerator = mapGenerator;

            _replays = new Dictionary<Guid, ReplayInstance<T>>();
            _replayLibrary = replayLibrary;
        }

        public void RegisterWriteRecordToFile(Action<GameRecord<T>> writeRecordToFile)
        {
            WriteRecordToFile = writeRecordToFile;
        }

        #region Game

        // NOTE: this is handled in the specific game instance
        // not sure if this will be a regret later
        private void HandleGameEvent(GameEvent gameEvent)
        {
            throw new NotImplementedException();
        }

        public GameplayValidationType ProcessGameAction(Guid clientId, GameAction<T> gameAction)
        {
            GameInstance<T> gameServer = GetGameInstance(clientId);

            return gameServer.ProcessGameAction(gameAction);
        }

        public List<Area<T>> GetGameMap(Guid clientId)
        {
            GameInstance<T>? gameServer = GetGameInstance(clientId);
            if (gameServer != null)
                return gameServer.GetGameMap();

            ReplayInstance<T> replayInstance = GetReplayInstance(clientId);

            return replayInstance.GetGameMap();
        }

        // TODO: Add custom config passing - keep default for now
        public Guid CreateGame(Guid clientId)
        {

            Guid gameId = Guid.NewGuid();

            void EndOfGameAction(GameAction<T>[] gameActions)
            {
                GameInstance<T> endedGameInstance = GetGameInstanceByGameId(gameId);
                WriteRecordToFile(endedGameInstance.GetGameRecord());
                CleanAllGameReferences(gameId);
            }

            GameInstance<T> gameServer = new GameInstance<T>(gameId,
                                                             EndOfGameAction,
                                                             _mapGenerator);

            _games.Add(gameServer.GameId, gameServer);

            List<Guid> gameClientIds = new List<Guid>();
            _gameToGameClients.Add(gameId, gameClientIds);

            return gameServer.GameId;
        }

        private void CleanAllGameReferences(Guid gameId)
        {
            GameInstance<T> gameInstance = GetGameInstance(gameId);

            _gameToGameClients.Remove(gameId);
            // TODO: this will be on the todo list, maybe we want to keep the clients ?
            // _gameClients.Remove();
            _games.Remove(gameId);
        }

        public bool RegisterGameClient(Guid clientId, Guid gameId, Action<GameEvent> gameEventHandler)
        {
            ConnectedClient client = new ConnectedClient
            {
                GameId = gameId,
                GameEventHandler = gameEventHandler,
            };

            GameInstance<T> gameServer = GetGameInstanceByGameId(client.GameId);
            client.PlayerId = gameServer.RegisterGameClient(gameEventHandler);

            _gameClients.Add(clientId, client);

            List<Guid> gameClients;
            if (_gameToGameClients.TryGetValue(gameId, out gameClients))
            {
                gameClients.Add(clientId);
            }

            // TODO: Add more validation
            return true;
        }

        public bool StartGame(Guid clientId)
        {
            GameInstance<T> gameInstance = GetGameInstance(clientId);

            GameConfig cfg = new GameConfig();
            cfg.MapId = MAP_ID;
            cfg.AreaDistributionType = "Simple";
            cfg.AttackHandlerType = "Normal";
            cfg.PlayerCount = gameInstance.PlayerCount;
            cfg.StartingTroops = 20;

            gameInstance.SetupGame(cfg);

            return true;
        }

        #endregion

        #region Replays

        public Guid StartReplay(Guid gameId, Guid clientId)
        {
            // return linkedListActions.First;
            GameRecord<T> gameRecord = _replayLibrary.GetGameRecord(gameId);

            LinkedList<GameAction<T>> linkedListActions = new LinkedList<GameAction<T>>(
                    gameRecord
                        .Actions
                        .Select(wrapper => wrapper.UnwrapAction())
                    );

            Guid replayId = Guid.NewGuid();
            ReplayInstance<T> replayInstance = new ReplayInstance<T>(gameId: gameId,
                                                                     replayId: replayId,
                                                                     mapGenerator: _mapGenerator,
                                                                     firstGameAction: linkedListActions.First);

            replayInstance.SetupReplay(gameRecord.GameConfig,
                                       gameRecord.DiceSeed,
                                       gameRecord.PlayerCount);

            _replays.Add(clientId, replayInstance);


            return replayId;
        }

        public GameplayValidationType ReplayNext(Guid clientId)
        {
            ReplayInstance<T> replayInstance = GetReplayInstance(clientId);

            return replayInstance.NextAction();
        }

        private ReplayInstance<T> GetReplayInstance(Guid clientId)
        {
            ReplayInstance<T> replayInstance;
            if (!_replays.TryGetValue(clientId, out replayInstance))
            {
                throw new Exception(@"Should do exceptions soon.. This is basically unknown / not connected client");
            }


            return replayInstance;
        }

        #endregion
        private GameInstance<T>? GetGameInstance(Guid clientId)
        {
            ConnectedClient client;
            if (!_gameClients.TryGetValue(clientId, out client))
            {
                // client not found in active game, try replays
                return null;
            }

            GameInstance<T> gameServer = GetGameInstanceByGameId(client.GameId);

            return gameServer;
        }

        private GameInstance<T> GetGameInstanceByGameId(Guid gameId)
        {
            GameInstance<T> gameServer;
            if (!_games.TryGetValue(gameId, out gameServer))
            {
                throw new Exception(@"Should do exceptions soon.. This is basically unknown game");
            }

            return gameServer;
        }

    }
    public class ConnectedClient
    {
        public Action<GameEvent> GameEventHandler;
        public Guid GameId;
        public int? PlayerId;

    }
}

