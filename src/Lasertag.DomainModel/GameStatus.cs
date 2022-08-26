namespace Lasertag.DomainModel
{
    public enum GameStatus : short
    {
        None = -1,

        Initialized = 0,

        LobyOpened = 1,
        GameStarted = 2,
        GameFinished = 3
    }
}