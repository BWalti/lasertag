using System.Diagnostics;
using Humanizer;
using Xunit.Abstractions;

namespace Lasertag.Tests;

public class MeasureTime : IDisposable
{
    readonly ITestOutputHelper _outputHelper;
    readonly string _prefix;
    readonly Stopwatch _stopWatch;

    public MeasureTime(ITestOutputHelper outputHelper, string prefix = "")
    {
        _outputHelper = outputHelper;
        _prefix = prefix;
        _stopWatch = Stopwatch.StartNew();
    }

    public void Dispose()
    {
        var elapsed = TimeSpan.FromMilliseconds(_stopWatch.ElapsedMilliseconds);
        _stopWatch.Stop();

        var spacer = string.IsNullOrEmpty(_prefix) ? "" : " ";
        _outputHelper.WriteLine($"{_prefix}{spacer}Measured: {elapsed.Humanize(2)}");
    }
}