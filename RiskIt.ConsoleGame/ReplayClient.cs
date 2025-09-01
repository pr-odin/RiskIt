using RiskIt.Main;
using RiskIt.Main.Actions;
using RiskIt.Main.Events;
using RiskIt.Main.Models.Enums;

namespace RiskIt.ConsoleGame
{
    public class ReplayClient
    {
        private GameServer<string> gameServer;
        private GameAction<string>[] gameActions;
        private int index;


        public ReplayClient(GameServer<string> gameServer, GameAction<string>[] gameActions)
        {
            this.gameServer = gameServer;
            this.gameActions = gameActions;
            index = 0;
        }

        public bool NextAction()
        {
            if (gameActions.Length <= index)
                return false;

            GameAction<string> nextAction = gameActions[index++];
            GameplayValidationType validation = gameServer.ProcessGameAction(nextAction);


            // TODO: When we have only valid events, this should probably do something
            if (validation != GameplayValidationType.Success)
            {
                Console.WriteLine(validation);
            }

            return true;
        }

        // empty as we don't care about events when playing back
        public void HandleEvent(GameEvent gameEvent)
        {
        }

    }
}

