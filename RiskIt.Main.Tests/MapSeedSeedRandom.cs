using RiskIt.Main.Models;
using RiskIt.Main.Models.Enums;

namespace RiskIt.Main.Tests
{
    public class MapSeedSeedRandom
    {

        [Test]
        public void NoEmptyAndUnOccupiedAreas()
        {
            ICollection<Area<int>> areas = GetTestMap().Values;
            
            uint amountTroopsPerPlayer = 20;

            LinkedList<(Player, uint)> playerTroops = new();
            playerTroops.AddFirst((new Player() { Id = 0 }, amountTroopsPerPlayer));
            playerTroops.AddLast((new Player() { Id = 1 }, amountTroopsPerPlayer));

            var playersCount = playerTroops.Count;

            AreaEnumeratorFactory<int> areaEnumeratorFactory = new AreaEnumeratorFactory<int>();
            MapSeeder<int> mapSeeder = new MapSeeder<int>(areaEnumeratorFactory);

            mapSeeder.Seed(
                areas: areas,
                playerTroops: playerTroops,
                areaDistributionType: AreaDistributionType.Simple);

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

            uint amountTroopsPerPlayer = 20;

            LinkedList<(Player, uint)> playerTroops = new();
            playerTroops.AddFirst((new Player() { Id = 0 }, amountTroopsPerPlayer));
            playerTroops.AddLast((new Player() { Id = 1 }, amountTroopsPerPlayer));

            var playersCount = playerTroops.Count;

            AreaEnumeratorFactory<int> areaEnumeratorFactory = new AreaEnumeratorFactory<int>();
            MapSeeder<int> mapSeeder = new MapSeeder<int>(areaEnumeratorFactory);

            mapSeeder.Seed(
                areas: areas,
                playerTroops: playerTroops,
                areaDistributionType: AreaDistributionType.Simple);

            Assert.IsNotNull(areas);


            int[] actualPlayerTroops = new int[playersCount];
            foreach (var area in areas)
            {
                actualPlayerTroops[area.Player.Id] += area.Troops;
            }

            foreach (var actualPlayerTroop in actualPlayerTroops)
            {
                Assert.That(actualPlayerTroop, Is.EqualTo(amountTroopsPerPlayer));
            }

        }

        [Test]
        public void NoPlayerHasMoreThan50PercentMoreAreasThanOther()
        {
            ICollection<Area<int>> areas = GetTestMap().Values;

            uint amountTroopsPerPlayer = 20;

            LinkedList<(Player, uint)> playerTroops = new();
            playerTroops.AddFirst((new Player() { Id = 0 }, amountTroopsPerPlayer));
            playerTroops.AddLast((new Player() { Id = 1 }, amountTroopsPerPlayer));

            var playersCount = playerTroops.Count;

            AreaEnumeratorFactory<int> areaEnumeratorFactory = new AreaEnumeratorFactory<int>();
            MapSeeder<int> mapSeeder = new MapSeeder<int>(areaEnumeratorFactory);

            mapSeeder.Seed(
                areas: areas,
                playerTroops: playerTroops,
                areaDistributionType: AreaDistributionType.Simple);

            Assert.IsNotNull(areas);

            int[] actualPlayerAreasCount = new int[playersCount];
            foreach (var area in areas)
            {
                actualPlayerAreasCount[area.Player.Id]++;
            }

            int max = -1;
            int min = int.MaxValue;
            foreach (var actualPlayerTroop in actualPlayerAreasCount)
            {
                if (actualPlayerTroop > max) max = actualPlayerTroop;
                if (actualPlayerTroop < min) min = actualPlayerTroop;

            }
            Assert.That((max - min), Is.Not.GreaterThanOrEqualTo(min));

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