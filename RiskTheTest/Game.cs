using RiskTheTest.Actions;
using RiskTheTest.Models;
using System.ComponentModel.DataAnnotations;

namespace RiskTheTest
{
    public class Game
    {
        public Guid Id { get; set; }
        private Map Map { get; set; }
        private List<Player> Players { get; set; }
        public PlayerTurn? GameTurn { get; set; }

        public Game(Map map, IEnumerable<Player> players)
        {
            Id = Guid.NewGuid();
            Map = map;
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
