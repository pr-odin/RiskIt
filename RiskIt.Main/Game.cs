using RiskIt.Main.Actions;
using RiskIt.Main.Models;

namespace RiskIt.Main
{
    public class Game
    {
        public Guid Id { get; set; }
        private IDictionary<int, Area> Map { get; set; }
        private IDictionary<string, Area> AreasByName { get; set; }
        private List<Player> Players { get; set; }
        public PlayerTurn? GameTurn { get; set; }

        public Game(IDictionary<int, Area> map, IEnumerable<Player> players)
        {
            Id = Guid.NewGuid();
            Map = map;
            AreasByName = new Dictionary<string, Area>(
                map.Values.Select(e =>
                {
                    return new KeyValuePair<string, Area>(key: e.Name, value: e);
                })
            );
            Players = players.ToList();
        }

        public void HandleAction(IAction action)
        {
            throw new NotImplementedException();
        }

        public int CalculateNextPlayerTurn(Player player)
        {
            int playerId = player.Id;
            var playerIndex = Players.FindIndex(p => p.Id == playerId);

            if (playerIndex == Players.Count - 1)
                return 0;

            return playerIndex++;
        }

        public void AdvanceTurn()
        {
            if (GameTurn is null || !GameTurn.Turn.AdvanceTurn())
            {
                PlayerTurn newPlayerTurn = new PlayerTurn
                {
                    Player = Players[CalculateNextPlayerTurn(GameTurn!.Player)],
                    Turn = new Turn()
                };

                GameTurn = newPlayerTurn;
            }
        }
        private Area GetArea(string name)
        {
            return AreasByName[name];
        }
    }
}
