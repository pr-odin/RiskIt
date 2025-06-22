using RiskIt.Main.AttackHandlers;
using RiskIt.Main.MapGeneration;
using RiskIt.Main.Models;
using RiskIt.Main.Models.Enums;

namespace RiskIt.Main
{
    public class GameBuilder<T> where T : IComparable<T>
    {
        public MapGenerator<T>? MapGenerator { get; set; }
        public MapSeeder<T>? MapSeeder { get; set; }
        public IEnumerable<Player>? Players { get; set; }
        public uint PlayerStartingTroops { get; set; }
        public AreaDistributionType AreaDistributionType { get; set; }
        public IDice? Dice { get; set; }
        public AttackHandlerType AttackHandlerType { get; set; }


        public Game<T> Build()
        {
            var map = MapGenerator?.ExportMap();
            if (map is null)
                throw new Exception("...Still doing exceptions later");

            LinkedList<(Player, uint)> playerTroops =
                new LinkedList<(Player, uint)>(
                    Players.Select(player => (player, PlayerStartingTroops))
                    );

            MapSeeder.Seed(areas: map.Values,
                playerTroops: playerTroops,
                areaDistributionType: AreaDistributionType);

            IAttackHandler attackHandler;

            attackHandler = CreateAttackHandler(AttackHandlerType);

            return new Game<T>(map, Players, attackHandler);
        }

        private IAttackHandler CreateAttackHandler(AttackHandlerType type)
        {
            return type switch
            {
                AttackHandlerType.Simple => new SimpleAttackHandler(Dice),
                AttackHandlerType.Normal => new NormalAttackHandler(Dice),
                _ => new SimpleAttackHandler(Dice)            };
        }
    }
}
