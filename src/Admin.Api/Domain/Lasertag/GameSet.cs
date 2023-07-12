namespace Admin.Api.Domain.Lasertag;

public class GameSet
{

    public GameSet(int id)
    {
        Id = id;
    }

    public int Id { get; init; }

    public bool IsConnected { get; set; }

    public bool IsActive { get; set; }
}