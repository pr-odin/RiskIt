using RiskIt.Main.Actions;
using RiskIt.Main.Persist;

namespace RiskIt.Main
{
    public class ReplayLibrary<T> where T : IComparable<T>
    {
        private HashSet<Guid> _replays;
        private Func<string, GameRecord<T>> fetchGameRecordById;

        public ReplayLibrary(string[] gameIds,
                             Func<string, GameRecord<T>> fetchGameActionsById)
        {
            _replays = new HashSet<Guid>(gameIds.Select(e => Guid.Parse(e)));
            this.fetchGameRecordById = fetchGameActionsById;

        }

        public GameRecord<T> GetGameRecord(Guid gameId)
        {
            if (!_replays.Contains(gameId))
            {
                throw new Exception("Could not find game id");
            }
            GameRecord<T> gameRecord = fetchGameRecordById(gameId.ToString());

            return gameRecord;
        }

    }
}


