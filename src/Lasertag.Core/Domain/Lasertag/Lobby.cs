namespace Lasertag.Core.Domain.Lasertag;

public class Lobby
{
    public LobbyConfiguration Configuration { get; set; } = new();

    public Team[] Teams { get; init; } = Array.Empty<Team>();
}