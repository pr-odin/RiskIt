namespace RiskIt.Main.AttackHandlers
{
    public enum AttackHandlerType
    {
        Simple,
        Normal
    }
    public static class AttackHandlerTypeMethods
    {
        public static AttackHandlerType Parse(string s)
        {
            return s switch
            {
                "Simple" => AttackHandlerType.Simple,
                "Normal" => AttackHandlerType.Normal,
                _ => throw new Exception("Case not handled " + s)
            };
        }
    }

}
