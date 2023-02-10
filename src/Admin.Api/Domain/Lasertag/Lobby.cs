namespace Admin.Api.Domain.Lasertag;

public class Lobby
{
    public LobbyConfiguration Configuration { get; set; } = new();

    public Team[] Teams { get; init; } = Array.Empty<Team>();
}

public record Team(int TeamId)
{
    readonly List<GameSet> _gameSets = new();

    public IEnumerable<GameSet> GameSets => _gameSets;

    public void Add(GameSet gameSet)
    {
        _gameSets.Add(gameSet);
    }
}