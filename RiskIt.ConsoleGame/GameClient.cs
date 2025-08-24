using RiskIt.Main.Models;

namespace RiskIt.ConsoleGame
{
    public class GameClient
    {
        public Player Player { get; private set; }
        public PlayerTurn PlayerTurn { get; set; } // need to make sure this is set

        public GameClient(Player player)
        {
            Player = player;
            PlayerTurn = new PlayerTurn();
            PlayerTurn.Player = player;
            PlayerTurn.Turn = new Turn();
        }

    }
}
