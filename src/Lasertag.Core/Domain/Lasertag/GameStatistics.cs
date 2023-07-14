namespace Lasertag.Core.Domain.Lasertag;

public class GameStatistics
{
    public int ShotsFired { get; set; }
    public int GotHit { get; set; }

    public void Apply(LasertagEvents.GameSetFiredShot @event)
    {
        ShotsFired++;
    }

    public void Apply(LasertagEvents.GameSetGotHit @event)
    {
        GotHit++;
    }
}