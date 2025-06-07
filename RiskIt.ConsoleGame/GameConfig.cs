namespace RiskIt.ConsoleGame
{
    public class GameConfig
    {
        public int MapId { get; set; }
        public uint PlayerCount { get; set; }

        public void Parse(string[] additionalArgs)
        {
            SetDefaults();

            foreach (var addArg in additionalArgs)
            {
                if (addArg.Length < 1 || !addArg.Contains(':')) return;

                var itemAndArgument = addArg.Split(":");

                var item = itemAndArgument[0];
                var arg = itemAndArgument[1];

                SetItem(item, arg);
            }
        }

        private void SetDefaults()
        {
            MapId = 1;
            PlayerCount = 2;
        }

        private void SetItem(string item, string arg)
        {
            switch (item)
            {
                case "map":
                    MapId = Convert.ToInt32(arg);
                    return;
                case "players":
                    PlayerCount = Convert.ToUInt32(arg);
                    return;
                default:
                    throw new Exception("Somethings off");
            }

        }
    }
}
