using LightBDD.Framework;

namespace Lasertag.Specifications;

public static class CompositeStepExtensions
{
    public static Task<CompositeStep> AsTask(this CompositeStep step) =>
        Task.FromResult(step);
}