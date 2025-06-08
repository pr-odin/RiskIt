using RiskIt.Main.Models;
using System.Reflection.Metadata.Ecma335;

namespace RiskIt.Main
{
    public class MapGenerator
    {
        private IDictionary<int, Area> Areas;
        private IDictionary<string, Area> AreasByName;
        private int Counter;

        public MapGenerator()
        {
            Areas = new Dictionary<int, Area>();
            AreasByName = new Dictionary<string, Area>();
            Counter = -1;
        }
        public void AddArea(string name)
        {
            int id = ++Counter;
            Area area = CreateArea(++Counter, name);
            Areas[id] = area;
            AreasByName[name] = area;
        }

        // mostly for debugging
        public void AddArea(int id)
        {
            if (Counter != -1) throw new Exception("Will do exceptions later");

            Areas[id] = new Area(id, id.ToString());
        }

        public void AddArea(int id, string name)
        {
            if (Counter != -1) throw new Exception("Will do exceptions later");

            Areas[id] = CreateArea(id, name);
        }

        private Area CreateArea(int id, string Name) => new Area(id, Name);

        public void AddConnection(string a, string b)
        {
            AddConnection(AreasByName[a].Id, AreasByName[b].Id);
        }
        public void AddConnection(int a, int b)
        {
            Areas[a].AddConnection(Areas[b]);
        }

        public IDictionary<int, Area> ExportMap()
        {
            // TODO: Check if whole map is connected aka. check for islands
            return Areas;
        }
    }
}
