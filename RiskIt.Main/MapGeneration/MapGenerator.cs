using RiskIt.Main.Models;

namespace RiskIt.Main.MapGeneration
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

        public static MapGenerator<string> CreateTestMap()
        {
            // circle/star
            var mg = new MapGenerator<string>();

            mg.AddArea(0.ToString());
            mg.AddArea(1.ToString());
            mg.AddArea(2.ToString());
            mg.AddArea(3.ToString());
            mg.AddArea(4.ToString());

            mg.AddConnection(0.ToString(), 1.ToString());
            mg.AddConnection(1.ToString(), 2.ToString());
            mg.AddConnection(2.ToString(), 3.ToString());
            mg.AddConnection(3.ToString(), 4.ToString());
            mg.AddConnection(4.ToString(), 0.ToString());

            return mg;

        }
    }
}
