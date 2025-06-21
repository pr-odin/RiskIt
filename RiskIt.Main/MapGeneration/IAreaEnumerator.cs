using RiskIt.Main.Models;

namespace RiskIt.Main.MapGeneration
{
    public interface IAreaEnumerator<T> where T : IComparable<T>
    {
        Area<T> Next(Player player);
    }
}