using RiskIt.Main.Models;
using RiskIt.Main.Models.Enums;

namespace RiskIt.Main
{
    public class MapSeeder<T> where T : IComparable<T>
    {

        public AreaEnumeratorFactory<T> AreaEnumeratorFactory { get; set; }

        public MapSeeder(AreaEnumeratorFactory<T> areaEnumeratorFactory)
        {
            AreaEnumeratorFactory = areaEnumeratorFactory;
        }

        public void Seed(ICollection<Area<T>> areas,
            LinkedList<(Player player, uint troops)> playerTroops,
            AreaEnumeratorType areaEnumeratorType)
        {
            IAreaEnumerator<T> areaEnumerator = AreaEnumeratorFactory.Create(areas, areaEnumeratorType);

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
        }
#if (DEBUG)
        private string PrintAreas(ICollection<Area<T>> areas)
        {
            var res = "";
            foreach (var area in areas)
            {
                res += area.ToString() + Environment.NewLine;
            }
            return res;
        }
#endif
    }
}
