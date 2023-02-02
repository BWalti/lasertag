namespace Admin.Api.Contexts;

public record ServerStarted;
public class Server
{
    public Guid Id { get; set; } = Guid.Empty;
    public string Name { get; set; } = "Lasertag Server";

}