﻿namespace Admin.Api.Domain.Lasertag;

public class Server
{
    public int Id { get; set; }
    public string Name { get; set; } = "Lasertag Server";
    public List<GameSet> GameSets { get; set; } = new();
}