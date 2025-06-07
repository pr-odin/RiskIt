using RiskIt.Main.Models.Enums;

namespace RiskIt.Main.Models
{
    public class Turn
    {
        public Phase Phase { get; set; }

        public Turn() => Phase = Phase.Placement;

        public bool AdvanceTurn()
        {
            if (Phase == Phase.Fortify)
                return false;

            Phase++;
            return true;
        }
    }
}
