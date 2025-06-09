using RiskIt.Main.Models;

namespace RiskIt.Main.Tests
{
    public class MapSeedSeedRandom
    {

        [Test]
        public void NoEmptyAndUnOccupiedAreas()
        {
            ICollection<Area<int>> areas = GetTestMap().Values;

            LinkedList<(Player, int)> playerTroops = new();
            playerTroops.AddFirst((new Player() { Id = 0 }, 20));
            playerTroops.AddLast((new Player() { Id = 1 }, 20));

            MapSeed<int> mapSeeder = new MapSeed<int>(areas);

            mapSeeder.SeedRandom(playerTroops);

            Assert.IsNotNull(areas);
            foreach (var area in areas)
            {
                Assert.That(area.Player, Is.Not.Null);
                Assert.That(area.Troops, Is.GreaterThanOrEqualTo(1));
            }
        }

        [Test]
        public void AllPlayersHaveEqualTroops()
        {
            ICollection<Area<int>> areas = GetTestMap().Values;

            int amountTroopsPerPlayer = 20;

            LinkedList<(Player, int)> playerTroops = new();
            playerTroops.AddFirst((new Player() { Id = 0 }, amountTroopsPerPlayer));
            playerTroops.AddLast((new Player() { Id = 1 }, amountTroopsPerPlayer));

            MapSeed<int> mapSeeder = new MapSeed<int>(areas);
            mapSeeder.SeedRandom(playerTroops);

            Assert.IsNotNull(areas);


            int[] actualPlayerTroops = new int[2];
            foreach (var area in areas)
            {
                actualPlayerTroops[area.Player.Id] += area.Troops;
            }

            foreach (var actualPlayerTroop in actualPlayerTroops)
            {
                Assert.That(actualPlayerTroop, Is.EqualTo(amountTroopsPerPlayer));
            }

        }


        private static IDictionary<int, Area<int>> GetTestMap()
        {
            var map = CreateTestMap().ExportMap();
            return map;
        }

        private static MapGenerator<int> CreateTestMap()
        {
            // circle/star
            var mg = new MapGenerator<int>();

            mg.AddArea(0);
            mg.AddArea(1);
            mg.AddArea(2);
            mg.AddArea(3);
            mg.AddArea(4);

            mg.AddConnection(0, 1);
            mg.AddConnection(1, 2);
            mg.AddConnection(2, 3);
            mg.AddConnection(3, 4);
            mg.AddConnection(4, 0);

            return mg;

        }
    }
}