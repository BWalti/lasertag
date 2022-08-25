namespace Grains;

public interface ISomeMartenLogEvents
{
}

public class SomeRaiseEvent : ISomeMartenLogEvents
{
    public int Amount { get; set; }
}

public class SomeDecreaseEvent : ISomeMartenLogEvents
{
    public int Amount { get; set; }
}