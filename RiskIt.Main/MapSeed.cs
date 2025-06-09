using RiskIt.Main.Models;

namespace RiskIt.Main
{
    public class MapSeed<T> where T : IComparable<T>
    {
        // how to ensure that we generate a random list, but still testable
        public static void SeedRandom(ref ICollection<Area<T>> areas , LinkedList<(Player player, int troops)> playerTroops)
        {

            // if anything is empty, divide those to the players
            //DivideToEmptyTerritories(areas, playerTroops, ref cPlayerTroops);

            var areasEnumerator = areas.GetEnumerator();
            var emptyAreasEnumerator = areas.Where(area => area.Player is null).GetEnumerator();
            var currNode = playerTroops.First;

            bool hasEmpty = true;

            while (playerTroops.Count != 0)
            {
                Area<T> currArea;
                var playerTroop = currNode.Value;

                if (hasEmpty)
                {
                    hasEmpty = emptyAreasEnumerator.MoveNext();
                    // exists loop and goes into the else
                    if (!hasEmpty)
                    {
                        hasEmpty = false;
                        continue;
                    } 
                    currArea = emptyAreasEnumerator.Current;
                    currArea.Player = playerTroop.player;
                }
                else
                {
                    // circular
                    if (!areasEnumerator.MoveNext())
                    {
                        areasEnumerator.Reset();
                        areasEnumerator.MoveNext();
                    }
                    currArea = areasEnumerator.Current;
                }


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

        public static void DivideToEmptyTerritories(ICollection<Area<T>> areas, List<(Player, int)> playerTroops, ref int cPlayerTroops)
        {
            Area<T>? empty;
            while ((empty = areas.Where(e => e.Troops == 0).FirstOrDefault()) is not null
                && playerTroops.Count != 0)
            {
                var playerTroop = playerTroops[cPlayerTroops];
                empty.Player = playerTroop.Item1;
                empty.Troops++;
                playerTroop.Item2--;

                if (playerTroop.Item2 == 0)
                    playerTroops.RemoveAt(cPlayerTroops);

                if (++cPlayerTroops == playerTroops.Count)
                    cPlayerTroops = 0;
            }
        }
    }
}
