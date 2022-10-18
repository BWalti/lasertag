using Lasertag.Api;
using Lasertag.DomainModel;
using Microsoft.AspNetCore.Mvc;

namespace Admin.Api;

public static class WebApplicationExtensions
{
    public static WebApplication MapGameStatisticsEndpoints(this WebApplication app)
    {
        app.MapGet("/api/gameRound/{gameRoundId}/stats",
            async(IGameRoundQueries gameRoundQueries, [FromRoute] Guid gameRoundId) => await gameRoundQueries.GetStats(gameRoundId))
            .Produces<ApiResult<ScoreBoard>>()
            .WithDisplayName("Get ScoreBoard");
        return app;
    }

    public static WebApplication MapGameRoundEndpoints(this WebApplication app)
    {
        app.MapPost("/api/gameRound/{gameRoundId}/lasertagSet/{sourceLasertagSetId}/shotFired",
            async(IGameRoundCommands gameRoundCommands, [FromRoute] Guid gameRoundId,
                [FromRoute] Guid sourceLasertagSetId) =>
            {
            return await gameRoundCommands.Fire(gameRoundId, sourceLasertagSetId);
        }).WithDisplayName("LasertagSet Fired Shot");

        app.MapPost(
            "/api/gameRound/{gameRoundId}/lasertagSet/{sourceLasertagSetId}/got-hit-from-lasertagSet/{targetLasertagSetId}",
            async(IGameRoundCommands gameRoundCommands, [FromRoute] Guid gameRoundId,
                [FromRoute] Guid sourceLasertagSetId,
                [FromRoute] Guid targetLasertagSetId) =>
            {
            return await gameRoundCommands.Hit(gameRoundId, sourceLasertagSetId, targetLasertagSetId);
        }).WithDisplayName("LasertagSet Got Hit by other LasertagSet");

        return app;
    }

    public static WebApplication MapGameEndpoints(this WebApplication app)
    {
        app.MapPost("/api/game/init",
                async (IGameCommands gameCommands) => { return await gameCommands.InitializeGame(Guid.NewGuid()); })
            .WithDisplayName("Initialize Game");

        app.MapPost("/api/game/{gameId}/{gameSetId}/connect",
            async(IGameCommands gameCommands, [FromRoute] Guid gameId, [FromRoute] Guid gameSetId) =>
            {
            return await gameCommands.ConnectGameSet(gameId, gameSetId);
        }).WithDisplayName("Connect GameSet");

        app.MapPost("/api/game/{gameId}/createLobby",
            async(IGameCommands gameCommands, [FromRoute] Guid gameId, [FromQuery] int ? numberOfGroups) =>
            {
            return await gameCommands.CreateLobby(gameId, numberOfGroups ?? 2);
        }).WithDisplayName("Open Lobby");

        app.MapPost("/api/game/{gameId}/{gameSetId}/activate/{playerId}",
            async(IGameCommands gameCommands, [FromRoute] Guid gameId, [FromRoute] Guid gameSetId,
                [FromRoute] Guid playerId) =>
            {
            return await gameCommands.ActivateGameSet(gameId, gameSetId, playerId);
        }).WithDisplayName("Activate GameSet");

        app.MapPost("/api/game/{gameId}/start",
            async(IGameCommands gameCommands, [FromRoute] Guid gameId) =>
            {
            var startGameRound = await gameCommands.StartGameRound(gameId);

            return new GameRoundStartResult(startGameRound.Item1, startGameRound.Item2);
        }).WithDisplayName("Start GameRound");

        return app;
    }

    public static WebApplication UseApiFallback404(this WebApplication app)
    {
        app.Use((ctx, next) =>
        {
            if (ctx.Request.Path.StartsWithSegments("/api"))
            {
                ctx.Response.StatusCode = 404;
                return Task.CompletedTask;
            }

            return next();
        });

        return app;
    }

    public static WebApplication UseDevelopmentDefaults(this WebApplication app)
    {
        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseHttpsRedirection();
            app.UseRouting();

            app.UseCors(corsBuilder =>
            {
                corsBuilder.WithOrigins("http://127.0.0.1:5173").AllowAnyMethod().AllowAnyHeader().AllowCredentials();
                corsBuilder.WithOrigins("http://localhost:5173").AllowAnyMethod().AllowAnyHeader().AllowCredentials();
            });

            app.UseSwagger();
            app.UseSwaggerUI();

            app.UseEndpoints(_ => { });
            app.UseApiFallback404();

#pragma warning disable S1075
            app.UseSpa(x => { x.UseProxyToSpaDevelopmentServer("http://localhost:5173"); });
# pragma warning restore S1075
        }

        return app;
    }
}