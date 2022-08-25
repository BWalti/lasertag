using Orleans;

namespace GrainInterfaces;

public interface ISomeMartenJournaledGrain : IGrainWithGuidKey
{
    Task RaiseAmount(int amount);
    Task<int> GetAmount();
    Task DecreaseAmount(int amount);
}