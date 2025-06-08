using RiskIt.Main.Models;
using System.Reflection.Metadata.Ecma335;

namespace RiskIt.Main
{
    public class MapGenerator<T> where T : IComparable<T>
    {
        private IDictionary<T, Area<T>> Areas;
        private int Counter;

        public MapGenerator()
        {
            Areas = new Dictionary<T, Area<T>>();
            Counter = -1;
        }
        public void AddArea(T id)
        {
            Area<T> area = new Area<T>(id);
            Areas[id] = area;
        }

        public void AddConnection(T a, T b)
        {
            Areas[a].AddConnection(Areas[b]);
        }

        public IDictionary<T, Area<T>> ExportMap()
        {
            // TODO: Check if whole map is connected aka. check for islands
            return Areas;
        }
    }
}
