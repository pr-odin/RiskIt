using RiskIt.Main.Actions;

namespace RiskIt.ConsoleGame.Models
{
    public class TypeWrapper<T> where T : IComparable<T>
    {
        public string? Type;
        public object? Data;

        public static TypeWrapper<T> WrapAction(GameAction<T> action)
        {
            var wrapper = new TypeWrapper<T>
            {
                Type = action.TypeAsString(),
                Data = action
            };
            return wrapper;
        }
    }
}
