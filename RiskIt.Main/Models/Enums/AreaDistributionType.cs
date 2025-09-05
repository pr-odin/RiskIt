namespace RiskIt.Main.Models.Enums
{
    public enum AreaDistributionType
    {
        Simple,
    }

    public static class AreaDistributionTypeMethods
    {
        public static AreaDistributionType Parse(string s)
        {
            return s switch
            {
                "Simple" => AreaDistributionType.Simple,
                _ => throw new Exception("Case not handled " + s)
            };
        }

    }

}
