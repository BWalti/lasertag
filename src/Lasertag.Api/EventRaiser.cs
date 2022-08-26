using Lasertag.DomainModel.DomainEvents;
using Lasertag.DomainModel;
using Lasertag.Manager;
using Orleans;
using System.Runtime.CompilerServices;
using Microsoft.Extensions.Logging;

namespace Lasertag.Api;

public class EventRaiser
{
    public EventRaiser(ILogger log, IGrainFactory grainFactory)
    {
        Log = log;
        GrainFactory = grainFactory;
    }

    public ILogger Log { get; }
    public IGrainFactory GrainFactory { get; }
    
    public Task<ApiResult<Game>> RaiseEvent<TEvent>(Guid gameId, Func<TEvent> eventFactory,
        [CallerMemberName] string? callerName = null)
        where TEvent : IGameEventBase
    {
        return RaiseEventWithChecks(gameId, _ => eventFactory(), callerName);
    }

    public async Task<ApiResult<Game>> RaiseEventWithChecks<TEvent>(Guid gameId, Func<Game, TEvent> checkedEventFactory,
        [CallerMemberName] string? callerName = null)
        where TEvent : IGameEventBase
    {
        Log.LogInformation("{CallerName}: start", callerName);
        try
        {
            Log.LogInformation("{CallerName}: get manager", callerName);
            var mgr = GrainFactory.GetGrain<IGameManager>(gameId);

            Log.LogInformation("{CallerName}: Get Game", callerName);
            var game = await mgr.GetManagedState();

            Log.LogInformation("{CallerName}: Check conditions and create Event", callerName);
            var @event = checkedEventFactory(game);

            Log.LogInformation("{CallerName}: raising event", callerName);
            await mgr.RaiseEvent(@event);

            Log.LogInformation("{CallerName}: confirming events", callerName);
            await mgr.ConfirmEvents();

            Log.LogInformation("{CallerName}: returning GetManagedState", callerName);
            return new ApiResult<Game>(await mgr.GetManagedState());
        }
        catch (Exception ex)
        {
            Log.LogError(ex, "{CallerName}: exception", callerName);
            return new ApiResult<Game>(ex);
        }
    }

    public async Task<ApiResult<Game>> RaiseEvents<TEvent>(Guid gameId, Func<TEvent[]> eventFactory,
        [CallerMemberName] string? callerName = null)
        where TEvent : IGameEventBase
    {
        Log.LogInformation("{CallerName}: start", callerName);
        try
        {
            Log.LogInformation("{CallerName}: get manager", callerName);
            var mgr = GrainFactory.GetGrain<IGameManager>(gameId);

            Log.LogInformation("{CallerName}: raising events", callerName);
            await mgr.RaiseEvents(eventFactory());

            Log.LogInformation("{CallerName}: confirming events", callerName);
            await mgr.ConfirmEvents();

            Log.LogInformation("{CallerName}: returning GetManagedState", callerName);
            return new ApiResult<Game>(await mgr.GetManagedState());
        }
        catch (Exception ex)
        {
            Log.LogError(ex, "{CallerName}: exception", callerName);
            return new ApiResult<Game>(ex);
        }
    }
}