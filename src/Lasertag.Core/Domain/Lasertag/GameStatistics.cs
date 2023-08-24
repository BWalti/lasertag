namespace Lasertag.Core.Domain.Lasertag;

public class GameStatistics
{
    public int ShotsFired => Teams.Sum(t => t.ShotsFired);
    public int GotHit => Teams.Sum(t => t.GotHit);

    public Dictionary<int, GameSetStatistics> GameSetLookup { get; set; } = new();

    public IEnumerable<TeamStatistics> Teams { get; set; } = Array.Empty<TeamStatistics>();

    public void Apply(LasertagEvents.GameSetFiredShot @event)
    {
        var player = GameSetLookup[@event.GameSetId];
        player.ShotsFired++;
    }

    public void Apply(LasertagEvents.GameSetGotHit @event)
    {
        var player = GameSetLookup[@event.GameSetId];
        player.GotHit++;
    }

    public void Apply(LasertagEvents.GamePrepared prepared)
    {
        var gameSetLookup = new Dictionary<int, GameSetStatistics>();

        Teams = prepared.Lobby.Teams
            .Select(t => new TeamStatistics(
                t.Key,
                t.Value.GameSets.Select(gs =>
                {
                    var gameSetStatistics = new GameSetStatistics(gs.Id);
                    gameSetLookup.Add(gs.Id, gameSetStatistics);

                    return gameSetStatistics;
                }).ToArray())).ToArray();

        GameSetLookup = gameSetLookup;
    }
}

public class TeamStatistics
{
    public TeamStatistics(int teamId, GameSetStatistics[] gameSetStatistics)
    {
        TeamId = teamId;
        GameSetStatistics = gameSetStatistics;
    }

    public int TeamId { get; }
    public GameSetStatistics[] GameSetStatistics { get; }
    public int ShotsFired => GameSetStatistics.Sum(p => p.ShotsFired);
    public int GotHit => GameSetStatistics.Sum(p => p.GotHit);
}

public class GameSetStatistics
{
    public GameSetStatistics(int id)
    {
        Id = id;
    }

    public int Id { get; }
    public int ShotsFired { get; set; }
    public int GotHit { get; set; }
}