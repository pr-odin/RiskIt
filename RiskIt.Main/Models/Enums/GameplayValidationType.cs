namespace RiskIt.Main.Models.Enums
{
    public enum GameplayValidationType
    {
        GameEnded = -1,
        Success = 0,
        DefaultCase = 1,
        AreaNotFound,
        WrongPhase,
        NotPlayerTurn,
        SamePlayersArea,
        TooManyTroops,
        AreaUnoccupied,
    }
}
