using RiskIt.Main.Actions;

namespace RiskIt.Main.CommandHandlers
{
    public class AttackHandler
    {
        public void HandleCommand(AttackAction comm)
        {
            var result = CalculateLostTroops(comm.AttackingTroops, comm.DefenderTroops);

            comm.Attacker.Troops -= result.Item1;
            comm.Defender.Troops -= result.Item2;
        }
        public (int, int) CalculateLostTroops(int troopsAtk, int troopsDef, int seed = 0)
        {
            // simple one dice vs one dice
            // for now...

            Random r;
            if (seed == 0)
                r = new Random();
            else
                r = new Random(seed);


            while (troopsAtk != 0 && troopsDef != 0)
            {
                var rollA = r.Next(5);
                var rollB = r.Next(5);

                if (rollA > rollB) // atk greater than def
                    troopsDef--;
                else //less or equal
                {
                    troopsAtk--;
                }
            }

            return (troopsAtk, troopsDef);
        }
    }
}
