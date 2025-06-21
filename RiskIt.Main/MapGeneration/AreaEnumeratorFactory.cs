using RiskIt.Main.Models;
using RiskIt.Main.Models.Enums;

namespace RiskIt.Main.MapGeneration
{
    public class AreaEnumeratorFactory<T> where T : IComparable<T>
    {
        public IAreaEnumerator<T> Create(ICollection<Area<T>> areas, AreaDistributionType type)
        {
            switch (type)
            {
                case AreaDistributionType.Simple:
                    return new SimpleAreaEnumerator<T>(areas);
                default:
                    throw new Exception("Doing expections later");
            }
        }
    }
}
