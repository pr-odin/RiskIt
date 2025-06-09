using RiskIt.Main.Models;

namespace RiskIt.Main
{
    public class MapSeed<T> where T : IComparable<T>
    {

        private bool _hasEmpty;
        private ICollection<Area<T>> _areas;
        public SimpleAreaEnumerator<T> areaEnumerator; 

        public MapSeed(ICollection<Area<T>> areas)
        {
            _hasEmpty = true;
            _areas = areas;
            areaEnumerator = new SimpleAreaEnumerator<T>(_areas);
        }

        public ICollection<Area<T>> SeedRandom(LinkedList<(Player player, int troops)> playerTroops)
        {
            var currNode = playerTroops.First;

            while (playerTroops.Count != 0)
            {
                var playerTroop = currNode.Value;

                Area<T> currArea = areaEnumerator.Next(playerTroop.player);

                if (currArea.Player is null)
                    currArea.Player = playerTroop.player;


                currArea.Troops++;
                playerTroop.troops--;

                // tuple returns a copy of the value, so we need to add it back
                currNode.Value = playerTroop;

                if (playerTroop.troops == 0)
                {
                    var nodeToDelete = currNode;
                    currNode = currNode.Next;
                    playerTroops.Remove(nodeToDelete);
                }
                else
                    currNode = currNode.Next;

                // now its a circular linked list
                if (currNode is null)
                    currNode = playerTroops.First;
            }
            return _areas;
        }
#if (DEBUG)
        private string PrintAreas()
        {
            var res = "";
            foreach (var area in _areas)
            {
                res += area.ToString() + Environment.NewLine;
            }
            return res;
        }
#endif
    }
}
