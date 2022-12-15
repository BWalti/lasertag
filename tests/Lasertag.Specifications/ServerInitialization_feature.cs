using LightBDD.Framework;
using LightBDD.Framework.Scenarios;
using LightBDD.XUnit2;

namespace Lasertag.Specifications;
#pragma warning disable S101

public class ServerInitialization_feature : FeatureFixture
{
    [Scenario]
    [ScenarioCategory(Categories.Infrastructure)]
    public async Task Init()
    {
        await Runner
            .WithContext<GameContext>()
            .AddAsyncSteps(
                context => context.Given_gameId_is_set(),
                context => context.When_Initializing_Game(),
                context => context.Then_Game_Is_Created())
            .RunAsync();
    }

    [Scenario]
    [ScenarioCategory(Categories.Infrastructure)]
    public async Task RegisterGameSets()
    {
        await Runner
            .WithContext<GameContext>()
            .RunScenarioAsync(
                context => context.Given_gameId_is_set(),
                context => context.Given_game_is_initialized(),
                context => context.When_I_register_gameSets(4),
                context => context.Then_gameSets_are_registered(4));
    }
}