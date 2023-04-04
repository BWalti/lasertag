namespace Admin.Api.Domain.Lasertag;

public class GameStatistics
{
    public Guid Id { get; set; }
    public int Version { get; set; }

    public int ShotsFired { get; set; }
}