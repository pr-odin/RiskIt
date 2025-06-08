using RiskIt.Main.Actions;
using RiskIt.Main.Models;

namespace RiskIt.Main
{
    public class Game<T> where T : IComparable<T>
    {
        public Guid Id { get; set; }
        private IDictionary<T, Area<T>> Map { get; set; }
        private List<Player> Players { get; set; }
        public PlayerTurn? GameTurn { get; set; }

        public Game(IDictionary<T, Area<T>> map, IEnumerable<Player> players)
        {
            Id = Guid.NewGuid();
            Map = map;
            // TODO: Seed the map
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
    }
}
