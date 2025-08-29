namespace RiskIt.Main
{
    public class PlacementHandler
    {
        public int AvailableTroops { get; set; }
        public bool IsFinished => AvailableTroops == 0;

        public PlacementHandler(int availableTroops)
        {
            AvailableTroops = availableTroops;
        }
    }
}
