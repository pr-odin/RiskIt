using RiskIt.Main.Models;
using RiskIt.Main.Models.Enums;

namespace RiskIt.Main
{
    public class AreaEnumeratorFactory<T> where T : IComparable<T>
    {
        public IAreaEnumerator<T> Create(ICollection<Area<T>> areas, AreaEnumeratorType type)
        {
            switch (type)
            {
                case (AreaEnumeratorType.Simple):
                    return new SimpleAreaEnumerator<T>(areas);
                default:
                    throw new Exception("Doing expections later");
            }
        }
    }
}
