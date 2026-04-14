namespace Robotico.Outbox.Mishima.Tests;

/// <summary>Sample message for outbox tests.</summary>
public sealed class OutboxTestMessage
{
    public int Id { get; set; }

    public string Name { get; set; } = "";
}
