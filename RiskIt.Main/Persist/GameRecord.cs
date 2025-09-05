namespace RiskIt.Main.Persist
{
    public class GameRecord<T> where T : IComparable<T>
    {
        public Guid GameId { get; }
        public int DiceSeed { get; }
        public int PlayerCount { get; }

        public GameConfig GameConfig { get; }

        public IEnumerable<TypeWrapper<T>> Actions { get; }

        public GameRecord(Guid gameId,
                          int diceSeed,
                          GameConfig gameConfig,
                          IEnumerable<TypeWrapper<T>> actions,
                          int playerCount)
        {
            GameId = gameId;
            DiceSeed = diceSeed;
            GameConfig = gameConfig;
            Actions = actions;
            PlayerCount = playerCount;
        }

        public override bool Equals(object? obj)
        {
            return obj is GameRecord<T> other &&
                   GameId.Equals(other.GameId) &&
                   DiceSeed == other.DiceSeed &&
                   EqualityComparer<IEnumerable<TypeWrapper<T>>>.Default.Equals(Actions, other.Actions);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(GameId, DiceSeed, Actions);
        }
    }
}
