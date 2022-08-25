namespace Grains;

public class SomeLogConsistentState
{
    public Guid Id { get; set; }
    public int Sum { get; set; }

    public void Apply(SomeRaiseEvent @event)
    {
        Sum += @event.Amount;
    }

    public void Apply(SomeDecreaseEvent @event)
    {
        Sum -= @event.Amount;
    }
}