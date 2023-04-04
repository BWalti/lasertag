namespace Admin.Api.Domain.Lasertag;

public class GameSet
{
    public int Id { get; init; }

    public bool IsConnected { get; set; }

    public GameSet(int id)
    {
        Id = id;
    }
}