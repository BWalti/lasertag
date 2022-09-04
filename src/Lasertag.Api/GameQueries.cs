using Orleans;
using Orleans.Concurrency;

namespace Lasertag.Api;

[Reentrant]
[StatelessWorker]
public class GameQueries : Grain, IGameQueries
{
}