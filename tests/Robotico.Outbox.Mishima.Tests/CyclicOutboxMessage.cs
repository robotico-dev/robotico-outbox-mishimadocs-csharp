namespace Robotico.Outbox.Mishima.Tests;

/// <summary>Forces a JSON serialization cycle so <see cref="System.Text.Json"/> throws.</summary>
public sealed class CyclicOutboxMessage
{
    public CyclicOutboxMessage? Self { get; set; }
}
