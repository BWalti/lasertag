using System.Runtime.CompilerServices;
using Lasertag.Manager;
using Microsoft.Extensions.Logging;

namespace Lasertag.Api;

public class EventRaiser<TDomainManager, TDomainModel, TState, TDomainEventBase>
    where TDomainManager : IDomainManager<TDomainModel, TState, TDomainEventBase>
    where TDomainModel : class
    where TDomainEventBase : class
{
    public EventRaiser(ILogger log, IGrainFactory grainFactory)
    {
        Log = log;
        GrainFactory = grainFactory;
    }

    public ILogger Log { get; }
    public IGrainFactory GrainFactory { get; }

    public Task<ApiResult<TDomainModel>> RaiseEvent<TEvent>(Guid streamId, Func<TEvent> eventFactory,
        [CallerMemberName] string? callerName = null)
        where TEvent : TDomainEventBase
    {
        return RaiseEventWithChecks(streamId, _ => eventFactory(), callerName);
    }

    public async Task<ApiResult<TDomainModel>> RaiseEventWithChecks<TEvent>(Guid streamId,
        Func<TDomainModel, TEvent> checkedEventFactory,
        [CallerMemberName] string? callerName = null)
        where TEvent : TDomainEventBase
    {
        Log.LogInformation("{CallerName}: start", callerName);
        try
        {
            Log.LogInformation("{CallerName}: get manager", callerName);
            var mgr = GrainFactory.GetGrain<TDomainManager>(streamId);

            Log.LogInformation("{CallerName}: Get Game", callerName);
            var game = await mgr.GetManagedState().ConfigureAwait(false);

            Log.LogInformation("{CallerName}: Check conditions and create Event", callerName);
            var @event = checkedEventFactory(game);

            Log.LogInformation("{CallerName}: raising event", callerName);
            await mgr.RaiseEvent(@event).ConfigureAwait(false);

            Log.LogInformation("{CallerName}: confirming events", callerName);
            await mgr.ConfirmEvents().ConfigureAwait(false);

            Log.LogInformation("{CallerName}: returning GetManagedState", callerName);
            return new ApiResult<TDomainModel>(await mgr.GetManagedState().ConfigureAwait(false));
        }
        catch (Exception ex)
        {
            Log.LogError(ex, "{CallerName}: exception", callerName);
            return new ApiResult<TDomainModel>(ex);
        }
    }
}