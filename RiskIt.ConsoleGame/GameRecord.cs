using RiskIt.ConsoleGame.Models;

namespace RiskIt.ConsoleGame
{
    public class GameRecord<T> where T : IComparable<T>
    {
        public Guid GameId { get; }
        public int DiceSeed { get; }
        public IEnumerable<TypeWrapper<T>> Actions { get; }

        public GameRecord(Guid gameId, int diceSeed, IEnumerable<TypeWrapper<T>> actions)
        {
            GameId = gameId;
            DiceSeed = diceSeed;
            Actions = actions;
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
