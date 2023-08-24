namespace Lasertag.Core.Domain.Lasertag;

public class Lobby
{
    public LobbyConfiguration Configuration { get; set; } = new();

    public Dictionary<int, Team> Teams { get; init; } = new();
}