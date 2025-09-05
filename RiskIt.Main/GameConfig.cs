namespace RiskIt.Main
{
    public class GameConfig
    {
        public int? MapId { get; set; }
        public int? PlayerCount { get; set; }
        public int? StartingTroops { get; set; }
        public string? AreaDistributionType { get; set; }
        public string? AttackHandlerType { get; set; }

        public void Parse(string[] additionalArgs)
        {
            foreach (var addArg in additionalArgs)
            {
                if (addArg.Length < 1 || !addArg.Contains(':')) return;

                var itemAndArgument = addArg.Split(":");

                var item = itemAndArgument[0];
                var arg = itemAndArgument[1];

                SetItem(item, arg);
            }
        }

        private void SetItem(string item, string arg)
        {
            switch (item)
            {
                case "map":
                    MapId = Convert.ToInt32(arg);
                    return;
                case "players":
                    PlayerCount = Convert.ToInt32(arg);
                    if (PlayerCount < 1) throw new Exception("Too few players or wat");
                    return;
                default:
                    throw new Exception("Somethings off");
            }

        }
    }
}
